using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaEliminazione
    {
        public static void GetDatiEliminazioneByIdPensione(ref ContenitoreObject contenitore, out GestionePensione.DatiEliminazione datiEliminazione)
        {
            datiEliminazione = contenitore.DatiEliminazione;
        }

        public static bool ControlsDatiEliminazione(GestionePensione.DatiEliminazione datiEliminazione, GestionePensione.DatiPensione datiPensione,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, bool? flagProvvisoria, DateTime? dataRinunciaTrattenutaInpdap, DateTime? scadenzaRevisioneSanitaria,
            byte? nRiconoscimentiInvalidita, DateTime dataSistema, bool isRiaperturaDomanda, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiEliminazione != null)
            {
                if (!GestioneCrossControls.AGO_CI_ControlsDatiEliminazione(datiPensione, datiEliminazione.CodiceMotivo, datiEliminazione.DecorrenzaEliminazione, datiEliminazione.DataEvento,
                    datiEliminazione.DataFineCalcoloArretrati, flagProvvisoria, dataRinunciaTrattenutaInpdap, scadenzaRevisioneSanitaria, datiPensione.DecorrenzaCalcoloArretrati, isRiaperturaDomanda, danteCausa, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.AGO_CI_ControlsConfermaInvalidita(datiPensione, datiEliminazione.DataEvento, nRiconoscimentiInvalidita, dataSistema, isRiaperturaDomanda, out messaggioVideo))
                {
                    messaggioVideo = "Liquidazione Pensione \\ Dati Generici: " + messaggioVideo;
                    return false;
                }
            }

            return true;
        }

        public static void StoreDatiEliminazione(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiEliminazione datiEliminazione)
        {
            if (datiEliminazione == null || datiEliminazione.Equals(new GestionePensione.DatiEliminazione()))
                return;

            GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;
            GestioneQuadri.GetQuadroEliminazioneByDatiPensione(datiPensione, out datiQuadroEliminazione);

            #region Gestione Semaforo Quadro Redditi
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.RedditoDRedd> lstRedditi = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(datiPensione.Id, out lstRedditi);

            GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi;
            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out datiQuadroRedditi);
            #endregion Gestione Semaforo Quadro Redditi

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensione.SalvaEliminazione(datiPensione.Id, datiEliminazione);

                datiQuadroEliminazione.TabEliminazione = 2;

                GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);

                #region Gestione Semaforo Redditi
                //20150107 - Rendere rosso il semaforo dei redditi nel caso in cui anno data evento maggiore max anno redditi
                string msg;
                if (!GestioneCrossControls.ALL_VerificaDecorrenzaEliminazioneWithRedditi(lstRedditi, datiEliminazione.DataEvento, out msg))
                {
                    switch (datiQuadroRedditi.Tipo.Value)
                    {
                        case 1:
                            datiQuadroRedditi.Tipo = 2;
                            datiQuadroRedditi.TabRedditi = 0;
                            break;
                        case 2:
                            datiQuadroRedditi.TabRedditi = 0;
                            break;
                        default:
                            break;
                    }
                    //datiQuadroRedditi.TabRedditi = 0;
                    GestioneQuadri.SalvaQuadroRedditi(datiPensione.Id, datiQuadroRedditi);
                }
                #endregion Gestione Semaforo Redditi

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiEliminazione(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiTitolare datiTitolare, GestioneIstruttoria.DatiIstruttoria datiIstruttoria,
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, DateTime dataSistema, bool isRiaperturaDomanda)
        {
            string msg;

            GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;
            GestioneQuadri.GetQuadroEliminazioneByDatiPensione(datiPensione, out datiQuadroEliminazione);

            bool isEliminazioneRossoPerConfermaInvalidita = !GestioneCrossControls.AGO_CI_ControlsEliminazioneConfermaInvalidita(datiPensione, null,
                (datiIstruttoria != null) ? datiIstruttoria.NRiconoscimentiInvalidita : null, dataSistema, isRiaperturaDomanda, out msg);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestionePensione.EliminaEliminazione(datiPensione.Id);

                if ((datiTitolare != null && datiTitolare.DataMorte.HasValue && datiPensione.DecorrenzaOriginaria.HasValue &&
                    Utility.DataSuccessivaA(datiTitolare.DataMorte.Value, datiPensione.DecorrenzaOriginaria.Value)) ||
                    isEliminazioneRossoPerConfermaInvalidita ||
                    (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) && datiPensioniDatiGenerici != null && datiPensioniDatiGenerici.ScadenzaAssegno.HasValue &&
                               Utility.DataSuccessivaA(Utility.FirstDayOfMonth(dataSistema), Utility.FirstDayOfMonth(datiPensioniDatiGenerici.ScadenzaAssegno.Value)))
                    || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
                {
                    datiQuadroEliminazione.Tipo = 2;
                    datiQuadroEliminazione.TabEliminazione = 0;
                }
                else
                {
                    datiQuadroEliminazione.Tipo = 1;
                    datiQuadroEliminazione.TabEliminazione = 1;
                }

                GestioneQuadri.SalvaQuadroEliminazione(datiPensione.Id, datiQuadroEliminazione);

                transactionScope.Complete();
            }
        }

        public static void GetListaCodiceEliminazione(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, out List<CodiceEliminazione> listaCodiceEliminazione)
        {
            listaCodiceEliminazione = new List<CodiceEliminazione>();
            List<GestioneDecodifica.CodiceEliminazione> elencoCodiceEliminazioneDB = null;
            GestioneDecodifica.GetCodiceEliminazioneByTipologia(out elencoCodiceEliminazioneDB, tipoAppartenenza);

            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo102", out controlloDinamico);

            if (elencoCodiceEliminazioneDB != null)
            {
                foreach (GestioneDecodifica.CodiceEliminazione codiceEliminazioneDB in elencoCodiceEliminazioneDB)
                {
                    CodiceEliminazione codiceEliminazione = new CodiceEliminazione();
                    if (Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) ||
                        Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria))
                    {
                        if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                        {
                            if (new List<char> { '1', '3', 'A' }.Contains(codiceEliminazioneDB.TraduzioneSuGP.GetValueOrDefault()))
                            {
                                Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                                listaCodiceEliminazione.Add(codiceEliminazione);
                            }
                        }
                        else
                        {
                            if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                            {
                                if (new List<char> { '1', '3', '6' }.Contains(codiceEliminazioneDB.TraduzioneSuGP.GetValueOrDefault()))
                                {
                                    Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                                    listaCodiceEliminazione.Add(codiceEliminazione);
                                }
                            }
                            else
                            {
                                if (new List<char> { '1', '3' }.Contains(codiceEliminazioneDB.TraduzioneSuGP.GetValueOrDefault()))
                                {
                                    Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                                    listaCodiceEliminazione.Add(codiceEliminazione);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (new List<char> { '1', '3', '4', 'A', 'N' }.Contains(codiceEliminazioneDB.TraduzioneSuGP.GetValueOrDefault()))
                        {
                            Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                            listaCodiceEliminazione.Add(codiceEliminazione);
                        }

                        if (Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) && codiceEliminazioneDB.Id == "8")
                        {
                            Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                            listaCodiceEliminazione.Add(codiceEliminazione);
                        }
                        //if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI)
                        //{
                        //    if (codiceEliminazioneDB.Id == "4")
                        //    {
                        //        Utility.ValorizzaOggetti(codiceEliminazioneDB, codiceEliminazione);
                        //        listaCodiceEliminazione.Add(codiceEliminazione);
                        //    }
                        //}
                    }
                }
            }
        }

        #region Cross Properties
        public static Dictionary<string, bool> GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiEliminazione datiEliminazione, bool isRiaperturaDomanda, out DateTime? dataFineCalcoloArretratiCalcolata)
        {
            dataFineCalcoloArretratiCalcolata = null;

            Dictionary<string, bool> lReturn = new Dictionary<string, bool>();
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) &&
                (Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVOESO(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) ||
                Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO29(datiPensione.SiglaCategoria)))
            {
                if (datiEliminazione.DataFineCalcoloArretrati.HasValue)
                    dataFineCalcoloArretratiCalcolata = datiEliminazione.DataFineCalcoloArretrati;
            }
            else
                dataFineCalcoloArretratiCalcolata = GetDataFineCalcoloArretrati(datiPensione, datiEliminazione, isRiaperturaDomanda);

            return lReturn;
        }

        private static DateTime? GetDataFineCalcoloArretrati(GestionePensione.DatiPensione datiPensione, GestionePensione.DatiEliminazione datiEliminazione, bool isRiaperturaDomanda)
        {
            DateTime? dataFineCalcoloArretrati = null;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
            {
                if (datiEliminazione != null)
                {
                    if (datiEliminazione.DataEvento.HasValue)
                        dataFineCalcoloArretrati = Utility.FirstDayOfMonth(datiEliminazione.DataEvento.Value);
                }
            }
            else if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI && Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda)) //ENG - RIC/TRF
            {
                if (datiEliminazione != null)
                {
                    if (datiEliminazione.DataFineCalcoloArretrati.HasValue)
                        dataFineCalcoloArretrati = datiEliminazione.DataFineCalcoloArretrati;
                }
            }
            return dataFineCalcoloArretrati;
        }


        #endregion Cross Properties

        #region nested class
        public class CodiceEliminazione
        {
            #region public properties
            public string Id { get; set; }

            public string Descrizione { get; set; }

            public string TestoVideo { get; set; }
            #endregion public properties
        }
        #endregion nested class
    }
}
