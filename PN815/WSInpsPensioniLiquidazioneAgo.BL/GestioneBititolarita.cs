using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneBititolarita
    {
        public static void GetDatiAltraPensioneByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out List<Entity.AltraPensione> LaltraPensione)
        {
            LaltraPensione = null;
            if (contenitore.DatiPensione == null || contenitore.ListaAltraPensione == null)
                return;

            LaltraPensione = new List<Entity.AltraPensione>();
            foreach (GestioneAltrePensioni.AltraPensione altraPensioneBL in contenitore.ListaAltraPensione)
            {
                Entity.AltraPensione altraPensione = new Entity.AltraPensione();
                Utility.ValorizzaOggetti(altraPensioneBL, altraPensione);
                LaltraPensione.Add(altraPensione);
            }
        }

        public static void StoreDatiAltraPensione(ref EntityBLCommon.ContenitoreObject contenitore, List<Entity.AltraPensione> LaltraPensione)
        {
            if (LaltraPensione == null || LaltraPensione.Count == 0)
                return;
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = contenitore.DatiQuadroBititolarita;
            //----------------------------------------------------------------
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
            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiQuadroBititolarita = datiQuadroBititolarita;
            //--------------------------------------------------------------------
        }

        public static bool ControlsDatiAltraPensione(ref EntityBLCommon.ContenitoreObject contenitore, ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, List<Entity.AltraPensione> LaltraPensione, out string msg)
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

            foreach (Entity.AltraPensione dati in LaltraPensione)
            {
                if (string.IsNullOrEmpty(dati.Categoria) || dati.Categoria.Trim() == string.Empty)
                {
                    msg = "Il 'Codice Categoria' è obbligatorio";
                    return false;
                }
                int categoriaNumerica = 0;
                int.TryParse(dati.Categoria, out categoriaNumerica);
                if (categoriaNumerica != 0)
                {
                    if (dati.Categoria.Length != 3)
                    {
                        msg = "Il 'Codice Categoria' deve essere lungo 3";
                        return false;
                    }
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

                if (contenitoreDecodifica.ElencoCatEnteAltraPensione != null && contenitoreDecodifica.ElencoCatEnteAltraPensione.Count > 0)
                {
                    if (contenitoreDecodifica.ElencoCatEnteAltraPensione.Find(x => x.CodCategoria.Trim() == (categoriaNumerica != 0 ? categoriaNumerica.ToString() : dati.Categoria.ToUpperInvariant().Trim())) == null)
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

            if (Utility.IsDomandaAnte96(contenitore.DatiPensione, contenitore.DatiPensione, contenitore.DatiDanteCausa, contenitore.IsRiaperturaDomanda) == null)
            {
                if (!GestioneControlli.ControlsBititolarita(contenitore.DatiPensione, LaltraPensione, contenitoreDecodifica, out msg))
                    return false;
            }
            else
            {
                if (!GestioneControlli.ControlsBititolaritaAnte96(contenitore.DatiPensione, LaltraPensione, out msg))
                    return false;
            }

            return true;
        }

        public static void DeleteDatiAltraPensione(ref EntityBLCommon.ContenitoreObject contenitore)
        {
            // Con queste istruzioni forzo la get dei dati
            //----------------------------------------------------------------
            GestionePensione.DatiPensione datiPensione = contenitore.DatiPensione;
            GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = contenitore.DatiQuadroBititolarita;
            //----------------------------------------------------------------
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneAltrePensioni.DeleteAltraPensioneByIdPensione(datiPensione.Id);
                datiQuadroBititolarita.Tipo = 2;
                datiQuadroBititolarita.TabAltrePensioni = 0;
                GestioneQuadri.SalvaQuadroBititolarita(datiPensione.Id, datiQuadroBititolarita);
                transactionScope.Complete();
            }
            // Aggiorno i dati sul contenitore
            //--------------------------------------------------------------------
            contenitore.DatiPensione = datiPensione;
            contenitore.DatiQuadroBititolarita = datiQuadroBititolarita;
            //--------------------------------------------------------------------
        }


        #region GetListeDecodifica

        public static void GetListeDecodificaEnte(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecodificaEnte> LdecodificaEnte)
        {
            LdecodificaEnte = null;

            if (contenitoreDecodifica.ElencoDecodificaEnte != null && contenitoreDecodifica.ElencoDecodificaEnte.Count > 0)
            {
                LdecodificaEnte = new List<DecodificaEnte>();
                foreach (GestioneDecodifica.DecodeEnte ente in contenitoreDecodifica.ElencoDecodificaEnte)
                {
                    DecodificaEnte enteAgo = new DecodificaEnte();
                    Utility.ValorizzaOggetti(ente, enteAgo);
                    LdecodificaEnte.Add(enteAgo);
                }
            }
        }

        public static void GetListeDecCatEnte(ref EntityBLCommon.ContenitoreDecodifica contenitoreDecodifica, out List<DecCatEnte> LdecCatEnte)
        {
            LdecCatEnte = null;

            if (contenitoreDecodifica.ElencoCatEnteAltraPensione != null && contenitoreDecodifica.ElencoCatEnteAltraPensione.Count > 0)
            {
                LdecCatEnte = new List<DecCatEnte>();
                foreach (GestioneDecodifica.CatEnteAltraPensione catEnte in contenitoreDecodifica.ElencoCatEnteAltraPensione.FindAll(x => x.TipoApp == Utility.TipoAppartenenza.AGO.ToString()))
                {
                    DecCatEnte CatEnteAgo = new DecCatEnte();
                    Utility.ValorizzaOggetti(catEnte, CatEnteAgo);
                    LdecCatEnte.Add(CatEnteAgo);
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
