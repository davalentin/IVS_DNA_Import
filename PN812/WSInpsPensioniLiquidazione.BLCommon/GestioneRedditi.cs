using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneRedditi
    {
        public static void GetRedditiDReddByIdPensione(long idPensione, out List<RedditoDRedd> redditiDRedd)
        {
            List<RedditiDRedd> redditiDReddDA = null;
            redditiDRedd = null;
            DAGestioneRedditi.GetRedditiDReddByIdPensione(idPensione, out redditiDReddDA);
            if (redditiDReddDA == null || redditiDReddDA.Count == 0)
                return;
            redditiDRedd = new List<RedditoDRedd>();
            foreach (RedditiDRedd redditoDReddDA in redditiDReddDA)
                redditiDRedd.Add(new RedditoDRedd(redditoDReddDA));
        }

        public static void SalvaRedditiDRedd(GestionePensione.DatiPensione datiPensione, List<RedditoDRedd> redditiDRedd)
        {
            List<RedditiDRedd> redditiDReddOriginali = null;

            DAGestioneRedditi.GetRedditiDReddByIdPensione(datiPensione.Id, out redditiDReddOriginali);
            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);
            GestioneQuadri.DatiQuadroRichiestaBonus datiQuadroRichiestaBonus = null;
            GestioneQuadri.GetQuadroRichiestaBonusByDatiPensione(datiPensione, out datiQuadroRichiestaBonus);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (redditiDRedd != null && redditiDRedd.Count > 0)
                {
                    List<RedditiDRedd> listaRedditiDReddDA = new List<RedditiDRedd>();

                    foreach (RedditoDRedd redditoDRedd in redditiDRedd)
                    {
                        RedditiDRedd redditoDReddDA = new RedditiDRedd();

                        Utility.ValorizzaOggetti(redditoDRedd, redditoDReddDA);
                        redditoDReddDA.IdPensione = datiPensione.Id;

                        listaRedditiDReddDA.Add(redditoDReddDA);
                    }
                    if (listaRedditiDReddDA.Count > 0)
                    {
                        DAGestioneRedditi.SalvaRedditiDRedd(datiPensione.Id, listaRedditiDReddDA, redditiDReddOriginali);
                        datiQuadroRedditi.TabRedditi = 2;
                        GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                    }
                    else
                    {
                        DAGestioneRedditi.EliminaAllRedditiDReddByIdPensione(datiPensione.Id);
                        datiQuadroRedditi.TabRedditi = 2;
                        GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                    }
                }
                else
                {
                    DAGestioneRedditi.EliminaAllRedditiDReddByIdPensione(datiPensione.Id);
                    datiQuadroRedditi.TabRedditi = 2;
                    GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                }

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.ControlloDinamico sediDaControllare = null;

                if (Utility.IsBonusBooking(datiPensione))
                {
                    if (datiPensione.Tipo == "0167") //BONUS 14°
                    {
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBooking" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out ctrl);
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonusBookingSedi" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out sediDaControllare);
                    }
                    else //BONUS 154
                    {
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out ctrl);
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneBonus154Sedi" + Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), out sediDaControllare);
                    }
                }

                if (ctrl != null && ctrl.ValoreControllo == "SI" &&
                    (sediDaControllare != null && (string.IsNullOrEmpty(sediDaControllare.ValoreControllo) ||
                     sediDaControllare.ValoreControllo.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0')))) &&
                    Utility.IsRicostituzioneOrRiapertura(datiPensione, false) && (datiPensione.Prodotto == "0101" || datiPensione.Prodotto == "0301" || datiPensione.Prodotto == "0401"))
                {
                    //Al variare dei redditi si deve rendere obbligatoria la riacquisizione degli anni bonus
                    BLCommon.GestioneAnniRichiestaBonus.EliminaAnniRichiestaBonusByIdPensione(datiPensione.Id);
                    datiQuadroRichiestaBonus.TabRichiestaBonus = 0;
                    BLCommon.GestioneQuadri.SalvaQuadroRichiestaBonus(datiPensione.Id, datiQuadroRichiestaBonus);
                }

                transactionScope.Complete();
            }
        }

        public static void EliminaAllRedditiDRedd(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRedditi.EliminaAllRedditiDReddByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaRedditoDRedd(long idPensione, RedditoDRedd redditoDRedd)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                RedditiDRedd redditoDReddDA = new RedditiDRedd();
                Utility.ValorizzaOggetti(redditoDRedd, redditoDReddDA);
                DAGestioneRedditi.EliminaRedditoDRedd(idPensione, redditoDReddDA);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class RedditoDRedd
        {
            public RedditoDRedd()
            { }
            public RedditoDRedd(short annoReddito, string rilevanza)
            {
                this._AnnoReddito = annoReddito;
                this._Rilevanza = rilevanza;
            }

            public RedditoDRedd(RedditiDRedd redditiDRedd)
            {
                this._AnnoReddito = redditiDRedd.AnnoReddito;
                this._Rilevanza = redditiDRedd.Rilevanza;
            }

            #region private properties
            private short _AnnoReddito;

            private string _Rilevanza;
            #endregion private properties

            #region public properties
            public short AnnoReddito { get { return _AnnoReddito; } set { _AnnoReddito = value; } }

            public string Rilevanza { get { return _Rilevanza; } set { _Rilevanza = value; } }
            #endregion public properties

            public override bool Equals(object obj)
            {
                RedditoDRedd reddito = (RedditoDRedd)obj;
                try
                {
                    if (this._AnnoReddito != reddito._AnnoReddito ||
                        (this._Rilevanza != null ? this._Rilevanza.Trim() : null) != (reddito._Rilevanza != null ? reddito._Rilevanza.Trim() : null))
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion nested class
    }
}



