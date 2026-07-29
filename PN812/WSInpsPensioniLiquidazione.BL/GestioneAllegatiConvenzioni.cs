using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.ServiceReferences.AllegatiConvenzioni;
using INPS.DNA.Logging;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAllegatiConvenzioni
    {
        #region public methods
        public static bool AggiornaCI05ByNumeroDomanda(GestionePensione.DatiPensione datiPensione, string matricola, short codiceSede, short centroOperativo, out string errori)
        {
            errori = string.Empty;
            TipoModuloAggiornaCI05Risposta dati = null;
            try
            {
                TipoModuloAggiornaCI05Richiesta input = new TipoModuloAggiornaCI05Richiesta();

                input.MetadatiServizio = new TipoMetadatiServizio();
                input.MetadatiServizio.NomeServizio = TipoNomeServizio.AggiornamentoCI05Service;
                input.MetadatiServizio.Mittente = "LIQPENS";
                input.MetadatiServizio.Timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds; // Unix Epoch Time
                input.DatiRichiesta = new TipoAggiornaCI05Richiesta();
                input.DatiRichiesta.Comarea = new TipoComareaPcics11();

                input.DatiRichiesta.Comarea.Cpgmdas = "PCICS11";
                input.DatiRichiesta.Comarea.MatricolaOp = matricola;
                input.DatiRichiesta.Comarea.Sede = codiceSede.ToString().PadLeft(4, '0') + centroOperativo.ToString().PadLeft(2, '0');
                input.DatiRichiesta.Comarea.Chiavi = new TipoChiaviPcics11();
                input.DatiRichiesta.Comarea.Chiavi.Numedoma = Convert.ToString(datiPensione.NDomus);
                input.DatiRichiesta.Comarea.Chiavi.TipoElab = "EX";
                input.DatiRichiesta.Comarea.Chiavi.EsitoCI = "A";
                input.DatiRichiesta.Comarea.Chiavi.DataEsito = int.Parse(Utility.GetDataElaborazionePensione(datiPensione).ToString("yyyyMMdd"));
                input.DatiRichiesta.Comarea.Chiavi.Filler01 = string.Empty;
                input.DatiRichiesta.Comarea.Chiavi.Filler02 = string.Empty;
                input.DatiRichiesta.Comarea.Chiavi.Filler03 = string.Empty;
                input.DatiRichiesta.Comarea.AltriD = string.Empty;
                input.DatiRichiesta.Comarea.CheFare = string.Empty;
                input.DatiRichiesta.Comarea.Prova = string.Empty;
                input.DatiRichiesta.Comarea.Altri = string.Empty;

                AggiornaCI05(input, out dati, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (dati.DatiRisposta != null && dati.DatiRisposta.Messaggio != null && dati.DatiRisposta.Messaggio.Codice != 0)
                {
                    errori = dati.DatiRisposta.Messaggio.Descrizione;
                    return false;
                }

                if (dati != null && dati.DatiRisposta != null && dati.DatiRisposta.ErroreAggiornamento != 0)
                {
                    errori = dati.DatiRisposta.ErroreAggiornamento + " - " + dati.DatiRisposta.ErroriEstesi;
                    return false;   
                }

                return true;
            }
            catch (Exception ex)
            {
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                errori = "Errore tecnico durante l'aggiornamento della stazione di lavoro";
                return false;
            }
        }

        public static bool AggiornaCI05(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, 
            ref string statoPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            bool isCodiceEsito9 = false;

            //Verifico che l'ultima attività su webdom sia di attesa calcolo (e non in liquidazione), se non è così effettuo l'aggiornamento delle attività
            if (!GestioneWebDom.VerificaFaseAttivitaPerAggiornamentoStazioneLavoro(datiPensione, matricolaOperatore, sedeOperatore, out errore))
            {
                messaggioVideo = errore;
                return false;
            }

            GestioneAllegatiConvenzioni.AggiornaCI05ByNumeroDomanda(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoStazLavoro;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoStazLavoro);
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
            GestionePensione.SalvaPensione(datiPensione);
            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom);

            GestioneWebDom.AggiornamentoFaseAttivita(datiPensione, datiPensione.MatricolaUtenteAcquisizione, sedeOperatore, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom);
                messaggioVideo = "Aggiornamento Staz. Lavoro riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento WebDom. " + messaggioVideo;
                return false;
            }

             //ENG - ENPALS: TRF MANUALI PRECOCI
            if (!(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && Utility.IsDomandaENPALS(datiPensione.Gestione) && Utility.IsDomandaAPEPrecoci(datiPensione)))
            {
                if (!GestioneCalcoloDomanda.AggiornaFelpeDopoWebDom(datiPensione, matricolaOperatore, sedeOperatore, ref messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoFelpe;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoFelpe);
                    messaggioVideo = "Aggiornamento WebDom riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Felpe. " + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneOneriPrepensionamento.AggiornaOneri(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoOneri;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri);
                messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Oneri. " + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
            {
                if (!GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, GestioneSAI.GetTipoRichiestaPAG(datiPensione), out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSAI;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SAI. " + messaggioVideo;
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
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SIN. " + messaggioVideo;
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

        #endregion public methods

        #region private methods
        private static void AggiornaCI05(TipoModuloAggiornaCI05Richiesta tipoModuloAggiornaCI05Richiesta, out TipoModuloAggiornaCI05Risposta risposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            risposta = null;

            AllegatiConvenzioniServiceClient proxy = new AllegatiConvenzioniServiceClient();
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(tipoModuloAggiornaCI05Richiesta, Utility.Servizio.SrvAllegatiConvenzioni, Utility.MetodoServizio.AggiornaCI05, Utility.SOAPLogDirection.IN,
                        tipoModuloAggiornaCI05Richiesta.DatiRichiesta.Comarea.Chiavi.Numedoma, guid);
                    risposta = proxy.aggiornaCI05(tipoModuloAggiornaCI05Richiesta);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AllegatiConvenzioni, method aggiornaCI05 | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AllegatiConvenzioni, method aggiornaCI05 | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AllegatiConvenzioni method aggiornaCI05 | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AllegatiConvenzioni method aggiornaCI05: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        long.TryParse(tipoModuloAggiornaCI05Richiesta.DatiRichiesta.Comarea.Chiavi.Numedoma, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante l'aggiornamento della stazione di lavoro";
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvAllegatiConvenzioni, Utility.MetodoServizio.AggiornaCI05, Utility.SOAPLogDirection.OUT,
                               tipoModuloAggiornaCI05Richiesta.DatiRichiesta.Comarea.Chiavi.Numedoma, guid);
                    Utility.CloseClient(proxy);
                }
            }
        }
        #endregion private methods
    }
}
