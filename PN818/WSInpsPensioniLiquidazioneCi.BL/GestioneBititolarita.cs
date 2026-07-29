using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneBititolarita
    {
        public static void GetDatiAltraPensioneByIdPensione(long idPensione, out List<Entity.AltraPensione> LaltraPensione)
        {
            LaltraPensione = null;

            List<GestioneAltrePensioni.AltraPensione> LaltraPensioneBL = null;
            GestioneAltrePensioni.GetAltraPensioneByIdPensione(idPensione, out LaltraPensioneBL);
            if (LaltraPensioneBL == null)
                return;

            LaltraPensione = new List<Entity.AltraPensione>();
            foreach (GestioneAltrePensioni.AltraPensione altraPensioneBL in LaltraPensioneBL)
            {
                Entity.AltraPensione altraPensione = new Entity.AltraPensione();
                Utility.ValorizzaOggetti(altraPensioneBL, altraPensione);
                LaltraPensione.Add(altraPensione);
            }
        }

        public static void StoreDatiAltraPensione(GestionePensione.DatiPensione datiPensione, List<Entity.AltraPensione> LaltraPensione)
        {
            if (LaltraPensione == null || LaltraPensione.Count == 0)
                return;

            GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = null;
            GestioneQuadri.GetQuadroBititolaritaByDatiPensione(datiPensione, out datiQuadroBititolarita);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneAltrePensioni.DeleteAltraPensioneByIdPensione(datiPensione.Id);
                foreach (Entity.AltraPensione altraPensione in LaltraPensione)
                {
                    if (!altraPensione.IsDatiAltraPensioneNull())
                    {
                        altraPensione.IdPensione = datiPensione.Id;
                        GestioneAltrePensioni.AltraPensione altraPensioneBL = new GestioneAltrePensioni.AltraPensione();
                        Utility.ValorizzaOggetti(altraPensione, altraPensioneBL);
                        GestioneAltrePensioni.SalvaAltraPensione(altraPensioneBL);
                    }
                }
                datiQuadroBititolarita.Tipo = 2;
                datiQuadroBititolarita.TabAltrePensioni = 2;

                GestioneQuadri.SalvaQuadroBititolarita(datiPensione.Id, datiQuadroBititolarita);
                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiAltraPensione(GestionePensione.DatiPensione datiPensione, List<Entity.AltraPensione> LaltraPensione, out string msg)
        {
            msg = string.Empty;

            if (LaltraPensione == null || LaltraPensione.Count == 0)
            {
                msg = "Dati obbligatori mancanti";
                return false;
            }

            if (LaltraPensione.Count > 5)
            {
                msg = "E' possibile inserire al massimo 5 record di tipo 'Altra Pensione'";
                return false;
            }

            List<GestioneDecodifica.CatEnteAltraPensione> listaCatEnte = null;
            GestioneDecodifica.GetCatEnteAltrePensioni(out listaCatEnte);

            foreach (Entity.AltraPensione dati in LaltraPensione)
            {
                if (string.IsNullOrEmpty(dati.Categoria) || dati.Categoria.Trim() == string.Empty)
                {
                    msg = "Il 'Codice Categoria' è obbligatorio";
                    return false;
                }

                if (!dati.Decorrenza.HasValue)
                {
                    msg = "La 'Decorrenza' è obbligatoria";
                    return false;
                }

                if (!dati.CodiceUC.HasValue)
                {
                    msg = "Il 'Codice U/C' è obbligatorio";
                    return false;
                }

                if (!dati.CodiceImporto.HasValue)
                {
                    msg = "Il 'Codice Importo' è obbligatorio";
                    return false;
                }

                if ((dati.Decorrenza.HasValue && dati.Cessazione.HasValue) && (dati.Cessazione.Value < dati.Decorrenza.Value))
                {
                    msg = "La 'Cessazione' deve essere successiva alla 'Decorrenza'";
                    return false;
                }

                if (listaCatEnte != null && listaCatEnte.Count > 0)
                {
                    int categoriaNumerica = 0;
                    int.TryParse(dati.Categoria.ToUpperInvariant().Trim(), out categoriaNumerica);
                    if (listaCatEnte.Find(x => x.CodCategoria.Trim() == (categoriaNumerica != 0 ? categoriaNumerica.ToString() : dati.Categoria.ToUpperInvariant().Trim())) == null)
                    {
                        msg = "Codice Categoria (" + dati.Categoria.ToUpperInvariant() + ") non riconosciuto.";
                        return false;
                    }
                }

                if ((dati.Categoria == "070" || dati.Categoria == "071" || dati.Categoria == "072") && dati.CodiceImporto.Value != '0')
                {
                    msg = "Per il 'Codice Categoria' (" + dati.Categoria.ToUpperInvariant() + ") il 'Codice Importo' deve essere 0";
                    return false;
                }
            }

            if (!ControlsCrossDatiAltraPensione(datiPensione, LaltraPensione, out msg))
                return false;

            return true;
        }

        public static bool ControlsCrossDatiAltraPensione(GestionePensione.DatiPensione datiPensione, List<Entity.AltraPensione> LaltraPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            string categoria = datiPensione.GetCodCategoria();

            if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (!datiPensione.NaturaPensione.Substring(0, 1).Equals("2") && !datiPensione.NaturaPensione.Substring(0, 1).Equals("4") &&
                !datiPensione.NaturaPensione.Substring(0, 1).Equals("5") && !datiPensione.NaturaPensione.Substring(0, 1).Equals("6") && !datiPensione.NaturaPensione.Substring(0, 1).Equals("9")))
            {
                messaggioVideo = "Non è possibile acquisire le bititolarità se il primo codice natura è pari a '" + (!string.IsNullOrEmpty(datiPensione.NaturaPensione) ? datiPensione.NaturaPensione.Substring(0, 1) : " ") + "'";
                return false;
            }

            //foreach (Entity.AltraPensione altraPensione in LaltraPensione)
            //{
            //    if (!GestioneControlli.CI_ControlsCategoriaWithCodiceEnteAltraPensione(altraPensione.Categoria, altraPensione.Ente, out messaggioVideo))
            //        return false;

            //    if (!GestioneControlli.CI_ControlsCategoriaWithCodiceUCAltraPensione(altraPensione.Categoria, altraPensione.CodiceUC, out messaggioVideo))
            //        return false;

            //    if (!GestioneControlli.CI_ControlsCategoriaWithCodiceImportoAltraPensione(altraPensione.Categoria, altraPensione.CodiceImporto, out messaggioVideo))
            //        return false;
            //}

            if (!GestioneControlli.ControlsBititolarita(datiPensione, LaltraPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaAltraPensioneWithCategoriaPensione(LaltraPensione, categoria, datiPensione, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaAltraPensioneWithNaturaPensione(LaltraPensione, datiPensione.NaturaPensione, out messaggioVideo))
                return false;

            return true;
        }

        public static void DeleteDatiAltraPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = null;
            GestioneQuadri.GetQuadroBititolaritaByDatiPensione(datiPensione, out datiQuadroBititolarita);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneAltrePensioni.DeleteAltraPensioneByIdPensione(datiPensione.Id);

                datiQuadroBititolarita.Tipo = 2;
                datiQuadroBititolarita.TabAltrePensioni = 0;
                GestioneQuadri.SalvaQuadroBititolarita(datiPensione.Id, datiQuadroBititolarita);

                transactionScope.Complete();
            }
        }


        #region GetListeDecodifica

        public static void GetListeDecodificaEnte(out List<DecodificaEnte> LdecodificaEnte)
        {
            LdecodificaEnte = null;
            List<GestioneDecodifica.DecodeEnte> LDecodificaEnte = null;
            GestioneDecodifica.GetElencoEnte(out LDecodificaEnte);
            if (LDecodificaEnte != null && LDecodificaEnte.Count > 0)
            {
                LdecodificaEnte = new List<DecodificaEnte>();
                foreach (GestioneDecodifica.DecodeEnte ente in LDecodificaEnte)
                {
                    DecodificaEnte enteCi = new DecodificaEnte();
                    Utility.ValorizzaOggetti(ente, enteCi);
                    LdecodificaEnte.Add(enteCi);
                }
            }
        }

        public static void GetListeDecCatEnte(out List<DecCatEnte> LdecCatEnte)
        {
            LdecCatEnte = null;
            List<GestioneDecodifica.CatEnteAltraPensione> LCatEnteAltraPensione = null;
            GestioneDecodifica.GetCatEnteAltrePensioni(out LCatEnteAltraPensione);
            if (LCatEnteAltraPensione != null && LCatEnteAltraPensione.Count > 0)
            {
                LCatEnteAltraPensione = LCatEnteAltraPensione.FindAll(x => x.TipoApp == Utility.TipoAppartenenza.CI.ToString());
                LdecCatEnte = new List<DecCatEnte>();
                foreach (GestioneDecodifica.CatEnteAltraPensione catEnte in LCatEnteAltraPensione)
                {
                    DecCatEnte CatEnteCi = new DecCatEnte();
                    Utility.ValorizzaOggetti(catEnte, CatEnteCi);
                    LdecCatEnte.Add(CatEnteCi);
                }
            }
        }

        #endregion GetListeDecodifica

        #region Nested Class

        public class DecodificaEnte
        {
            #region public properties

            public byte Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties
            private byte _Id;
            private string _Descrizione;
            #endregion private properties
        }

        public class DecCatEnte
        {
            #region public properties

            public string CodCategoria { get { return _CodCategoria; } set { _CodCategoria = value; } }
            public char CodEnte { get { return _CodEnte; } set { _CodEnte = value; } }

            #endregion public properties

            #region private properties
            private string _CodCategoria;
            private char _CodEnte;
            #endregion private properties
        }
        #endregion Nested Class
    }
}
