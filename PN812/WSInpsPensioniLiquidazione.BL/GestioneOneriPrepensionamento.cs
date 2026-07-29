using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneOneriPrepensionamento
    {
        #region public methods
        public static bool AggiornaOneri(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            bool isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento degli Oneri";
                return false;
            }

            if (!AggiornaOneri(datiPensione, out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri);
                return false;
            }

            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
            {
                if (!GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, GestioneSAI.GetTipoRichiestaPAG(datiPensione), out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSAI;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                    messaggioVideo = "Aggiornamento Oneri riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SAI. " + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (!GestioneINPDAP.AggiornaINPDAP(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSIN;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSIN);
                    messaggioVideo = "Aggiornamento Oneri riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SIN. " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaPianiDiPagamento(datiPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNo6Scatti;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNo6Scatti);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaEquoIndennizzo(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaIndennitaSpeciale(datiPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                if (!GestioneTotalIvs.AggiornaCumulo(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTotal;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal);
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento TOTAL (Cumulo). " + messaggioVideo;
                }                
            }

            if (Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && datiPensione.IsTotAutomatica.GetValueOrDefault())
            {
                if (!GestioneTotalIvs.AggiornaTot(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTot;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTot);
                    messaggioVideo = "Aggiornamento WebDom riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento TOTAL (Totalizzazione). " + messaggioVideo;
                }
            }

            if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
                return false;
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        internal static bool AggiornaOneri(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
            {
                if ((datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.Calcolata ||
                    datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.CalcolataNoWebDom ||
                    datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.CalcolataNoOneri)
                    && Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione))
                {
                    GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = null;
                    GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(datiPensione.Id, out datiPrepensionamento);

                    if (datiPrepensionamento != null)
                    {
                        GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                        GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagrafica);

                        GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                        GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

                        List<GestionePrepensionamento.DatiPrepensionamento> listaDatiPrepensionamento = null;
                        GestionePrepensionamento.SelectTOPPL03(datiPensione, out listaDatiPrepensionamento, ref messaggioVideo);

                        // Verifico se sulla TOPPL03 ci sono già dei dati e se questi sono uguali a quelli che si vuole salvare
                        // Se sono uguali significa che è stato già effettuato un inserimento, ma non è stato aggiornato correttamente lo stato della domanda
                        if (listaDatiPrepensionamento != null && listaDatiPrepensionamento.Count > 0)
                        {
                            if (listaDatiPrepensionamento.First().Equals(datiPrepensionamento))
                                return true;
                            else
                            {
                                string msg = "Dati già presenti su TOPPL03 con valori diversi";
                                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? msg : messaggioVideo + " - " + msg;
                            }
                        }
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Questo blocco di codice è stato commentato perchè al momento non è richiesto aggiornare i dati del prepensionamento da DB2, poichè per le ricostituzioni non è possibile 
                        // acquisire i dati
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //if (listaDatiPrepensionamento != null && listaDatiPrepensionamento.Count > 0)
                        //    GestionePrepensionamento.UpdateTOPPL03(datiPensione, anagrafica, datiIstruttoria, datiPrepensionamento, ref messaggioVideo);
                        //else
                        GestionePrepensionamento.InsertTOPPL03(datiPensione, anagrafica, datiIstruttoria, datiPrepensionamento, ref messaggioVideo);

                        //messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? "Scrittura Oneri al momento non disponibile" : messaggioVideo + " - " + "Scrittura Oneri al momento non disponibile";
                    }
                }
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
                return false;

            return true;
        }
        #endregion public methods

        #region private methods
        private static bool ControllaStatoPensionePerAggiornamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoOneri)
                return true;
            else
                return false;
        }
        #endregion private methods
    }
}
