using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.ServiceReferences.DatiPensioni;
using INPS.Pensioni.Liquidazione.ServiceReferences.INPDAP;
using INPS.Pensioni.Liquidazione.ServiceReferences.OrchSerPen;
using INPS.Pensioni.Liquidazione.ServiceReferences.SrvPianiDiPagamento;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Services.Protocols;
//using static INPS.Pensioni.Liquidazione.BLCommon.GestionePensioneINPDAP;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneINPDAP
    {
        public static bool AggiornaINPDAP(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            bool isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento INPDAP";
                return false;
            }

            if (!AggiornaINPDAP(datiPensione, out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSIN);
                return false;
            }

            if (!AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
            }

            if (!AggiornaPianiDiPagamento(datiPensione, out messaggioVideo, out isCodiceEsito9))
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

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        internal static bool AggiornaINPDAP(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN)
            {
                AggiornaINPDAP_Private(datiPensione, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }
            else if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
            {
                AggiornaINPDAP_SPI_Private(datiPensione, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }
            return true;
        }

        private static void AggiornaINPDAP_Private(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;

            IntPenSisWSClient proxy = new IntPenSisWSClient();
            Guid guid = Guid.NewGuid();
            Approvazione_INPDAP_Request input = new Approvazione_INPDAP_Request();
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            if (datiAnagrafici != null)
                input.CODFISC = datiAnagrafici.CodiceFiscale;
            input.CODSED = datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(0, 2);
            input.CODZON = datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(2, 2);
            input.CODSEDNEW = datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(0, 2);
            input.CODZONNEW = datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(2, 2);
            input.MATRICOLA = datiPensione.MatricolaUtenteAcquisizione;
            input.NUMCERTIFICATO = datiPensione.NCertificato.GetValueOrDefault().ToString().PadLeft(8, '0');
            input.NUMDOMUS = datiPensione.NDomus.ToString();
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                input.TIPOEMIS = "51";
            else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                input.TIPOEMIS = "5";
            else
                input.TIPOEMIS = "1";

            string output = string.Empty;
            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvSIN, Utility.MetodoServizio.Approvazione_INPDAP, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                    output = proxy.Approvazione_INPDAP(input.CODFISC, input.CODSED, input.CODZON, input.NUMCERTIFICATO, ref input.NUMDOMUS, input.TIPOEMIS, input.CODSEDNEW, input.CODZONNEW, input.MATRICOLA, out messaggioVideo);
                }
                catch (SoapException exception)
                {
                    messaggioVideo = exception.Message + exception.Detail != null ? exception.Detail.InnerText : string.Empty;
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio SIN | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio SIN | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio SIN | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nella chiamata al servizio SIN: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi INPDAP";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(string.Concat(output, " - ", messaggioVideo), Utility.Servizio.SrvSIN, Utility.MetodoServizio.Approvazione_INPDAP, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                }
            }
            if (string.IsNullOrEmpty(output) || output.Trim().ToUpper() != "OK")
            {
                messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi INPDAP";
                return;
            }
            else
                messaggioVideo = string.Empty;
        }
        private static void AggiornaINPDAP_SPI_Private(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            //Approvazione_INPPAP con SPI
            //Flusso attualmente non da implementare, requisiti cambiati
        }

        internal static bool AggiornaNoteDiDebito(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);
            decimal? rmsSenzaLegge33670QA = (!Utility.IsDomandaMiglioramentiContrattuali(datiPensione) && listaRecordDatiFondoINPDAP != null && listaRecordDatiFondoINPDAP.Count > 0) ? listaRecordDatiFondoINPDAP.FirstOrDefault().RMSSenzaLegge33670QA : 0;
            bool isGDPMiglioramentiContrattuali = false;
            List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> datiQuoteMiglioramentiContrattuali = null;

            GestioneMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiByIdPensione(datiPensione.Id, out datiQuoteMiglioramentiContrattuali);

            if (Utility.AbilitaFlussoNoteDiDebito() && datiQuoteMiglioramentiContrattuali != null && datiQuoteMiglioramentiContrattuali.Count > 0 && Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                isGDPMiglioramentiContrattuali = true;
            }
            if (Utility.AbilitaFlussoNoteDiDebito() && Utility.IsFlussoNoteDiDebito(datiPensione, rmsSenzaLegge33670QA))
            {
                AggiornaNoteDiDebitoPrivate(datiPensione, listaRecordDatiFondoINPDAP != null ? listaRecordDatiFondoINPDAP.FirstOrDefault() : null, false, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }
            if (isGDPMiglioramentiContrattuali)
            {
                AggiornaNoteDiDebitoPrivate(datiPensione, listaRecordDatiFondoINPDAP != null ? listaRecordDatiFondoINPDAP.FirstOrDefault() : null, isGDPMiglioramentiContrattuali, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }

            return true;
        }

        internal static bool AggiornaPianiDiPagamento(GestionePensione.DatiPensione datiPensione, out string messaggioVideo, out bool isCodiceEsito9)
        {
            messaggioVideo = string.Empty;
            isCodiceEsito9 = false;
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);
            if (Utility.AbilitaFlussoSeiScatti() && Utility.IsFlusso6Scatti(datiPensione, listaRecordDatiFondoINPDAP.FirstOrDefault())) //TODO Utility per entrare nel flusso 
            {
                AggiornaPianiDiPagamentoPrivate(datiPensione, listaRecordDatiFondoINPDAP, out messaggioVideo, out isCodiceEsito9);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }

            return true;
        }

        internal static bool AggiornaEquoIndennizzo(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);
            if (listaRecordDatiFondoINPDAP != null && Utility.IsFlussoEquoIndennizzo(datiPensione, listaRecordDatiFondoINPDAP.FirstOrDefault()))
            {
                AggiornaEquoIndennizzoPrivate(datiPensione, listaRecordDatiFondoINPDAP, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }
            return true;
        }

        internal static bool AggiornaIndennitaSpeciale(GestionePensione.DatiPensione datiPensione, out string messaggioVideo, out bool isCodiceEsito9)
        {
            messaggioVideo = string.Empty;
            isCodiceEsito9 = false;
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(datiPensione.Id, out listaRecordDatiFondoINPDAP);
            if (listaRecordDatiFondoINPDAP != null && Utility.IsFlussoIndennitaSpeciale(datiPensione, listaRecordDatiFondoINPDAP.FirstOrDefault()))
            {
                AggiornaIndennitaSpecialePrivate(datiPensione, listaRecordDatiFondoINPDAP, out messaggioVideo, out isCodiceEsito9);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return false;
            }
            return true;
        }

        public static bool AggiornaNoteDiDebito(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            bool isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamentoNoteDiDebito(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento Note di Debito";
                return false;
            }

            if (!AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                return false;
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && !AggiornaPianiDiPagamento(datiPensione, out messaggioVideo, out isCodiceEsito9))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNo6Scatti;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNo6Scatti);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                return false;
            }

            if (!AggiornaEquoIndennizzo(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                return false;
            }

            if (!AggiornaIndennitaSpeciale(datiPensione, out messaggioVideo, out isCodiceEsito9))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                return false;
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

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool AggiornaPianiDiPagamento(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo, out bool isCodiceEsito9)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamentoPianiDiPagamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento Piani Di Pagamento";
                return false;
            }

            if (!AggiornaPianiDiPagamento(datiPensione, out messaggioVideo, out isCodiceEsito9))
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

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool AggiornaEquoIndennizzo(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            bool isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamentoEquoInd(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento Piani Di Pagamento";
                return false;
            }

            if (!AggiornaEquoIndennizzo(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                return false;
            }

            if (!AggiornaIndennitaSpeciale(datiPensione, out messaggioVideo, out isCodiceEsito9))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                return false;
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

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool AggiornaIndennitaSpeciale(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo, out bool isCodiceEsito9)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            isCodiceEsito9 = false;

            if (!ControllaStatoPensionePerAggiornamentoIndennSpec(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento Piani Di Pagamento";
                return false;
            }

            if (!AggiornaIndennitaSpeciale(datiPensione, out messaggioVideo, out isCodiceEsito9))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                return false;
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

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        private static void AggiornaNoteDiDebitoPrivate(GestionePensione.DatiPensione datiPensione, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, bool isGDPMiglioramentiContrattuali, out string messaggioVideo)
        {
            //TODO: completare con chiamata a servizio
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;
            string input = string.Empty;
            string output = string.Empty;

            OrchServPensWebServiceClient proxy = new OrchServPensWebServiceClient();
            Guid guid = Guid.NewGuid();

            using (new MethodExecutionTracer())
            {
                try
                {
                    AcquisizioneEventoRequest request = ValorizzaAcquisizioneEventoRequest(datiPensione, recordDatiFondoINPDAP, isGDPMiglioramentiContrattuali);
                    if ((request != null && !Utility.IsDomandaMiglioramentiContrattuali(datiPensione)) || (request != null && request.parametriInput.importi != null && request.parametriInput.importi.Any(x => x.pensioneInPagamento > 0)))
                    {
                        input = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvOrchServPens, Utility.MetodoServizio.AcquisizioneEvento, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                        output = proxy.acquisizioneEvento(input);
                        AcquisizioneEventoResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<AcquisizioneEventoResponse>(output);
                        if (response == null || (response != null && response.codiceEsito == "KO"))
                        {
                            messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi OrchServPens";
                            return;
                        }
                    }
                }
                catch (SoapException exception)
                {
                    messaggioVideo = exception.Message + exception.Detail != null ? exception.Detail.InnerText : string.Empty;
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio OrchServPens | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio OrchServPens | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio OrchServPens | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nella chiamata al servizio OrchServPens: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi OrchServPens";
                        string parametri = string.Format("GUID per LogSoap: {0}", guid);
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(string.Concat(output, " - ", messaggioVideo), Utility.Servizio.SrvOrchServPens, Utility.MetodoServizio.AcquisizioneEvento, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                }
            }
        }

        private static void AggiornaPianiDiPagamentoPrivate(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP, out string messaggioVideo, out bool isCodiceEsito9)
        {
            //TODO: completare con chiamata a servizio
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;
            string input = string.Empty;
            string output = string.Empty;
            isCodiceEsito9 = false;
            //InserisciConguaglio_Gruppo03_Response response = null;
            GestioneConguaglio_6scatti_Response responseGestioneConguaglio = null;
            Guid guid = Guid.NewGuid();
            try
            {
                //if (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || Utility.IsRiaperturaDomanda(datiPensione.Id))
                //{
                GestioneConguaglio_6scatti_Request requestGestioneConguaglio = new GestioneConguaglio_6scatti_Request();
                requestGestioneConguaglio = ValorizzaGestioneConguaglioRequest(datiPensione, recordDatiFondoINPDAP);
                GestioneLogSoap.SalvaLogSoap(requestGestioneConguaglio, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.GestioneCong_6scatti, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                using (var proxy = new WSPianiDiPagamentoClient())
                using (var scope = new OperationContextScope(proxy.InnerChannel))
                {
                    IdentitySrv identity = new IdentitySrv();
                    identity.AppName = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] : "";
                    identity.AppKey = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] : "";
                    identity.UserId = datiPensione.MatricolaUtenteAcquisizione;
                    identity.IdentityProvider = "AD";


                    MessageHeader identityHeader = MessageHeader.CreateHeader("Identity", "https://inps.it", identity);
                    OperationContext.Current.OutgoingMessageHeaders.Add(identityHeader);

                    responseGestioneConguaglio = proxy.GestioneCong_6scatti(requestGestioneConguaglio);

                    if (responseGestioneConguaglio == null || (responseGestioneConguaglio != null && responseGestioneConguaglio.Esito != null && responseGestioneConguaglio.Esito.Risultato == RisultatoGestioneConguaglio.NOK))
                    {
                        messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                        return;
                    }
                    else if (responseGestioneConguaglio != null && responseGestioneConguaglio.Esito != null && responseGestioneConguaglio.Esito.Risultato == RisultatoGestioneConguaglio.OK && responseGestioneConguaglio.Esito.Codice == 9)
                    {
                        isCodiceEsito9 = true;
                    }

                }
                //}
                //else
                //{
                //    InserisciConguaglio_Gruppo03_Request request = new InserisciConguaglio_Gruppo03_Request();
                //    request = ValorizzaInserisciConguaglioRequest(datiPensione, recordDatiFondoINPDAP);
                //    GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.InsertCONG003, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                //    using (var proxy = new WSPianiDiPagamentoClient())
                //    using (var scope = new OperationContextScope(proxy.InnerChannel))
                //    {
                //        IdentitySrv identity = new IdentitySrv();
                //        identity.AppName = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] : "";
                //        identity.AppKey = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] : "";
                //        identity.UserId = datiPensione.MatricolaUtenteAcquisizione;
                //        identity.IdentityProvider = "AD";

                //        MessageHeader identityHeader = MessageHeader.CreateHeader("Identity", "https://inps.it", identity);
                //        OperationContext.Current.OutgoingMessageHeaders.Add(identityHeader);

                //        response = proxy.InsertCONG003(request);

                //        if (response == null || (response != null && response.Esito != null && response.Esito.Risultato == RisultatoInserisciConguaglio.NOK))
                //        {
                //            messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                //            return;
                //        }
                //    }
                //}
            }
            catch (SoapException exception)
            {
                messaggioVideo = exception.Message + exception.Detail != null ? exception.Detail.InnerText : string.Empty;
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                messaggioVideo = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                erroreTecnico = true;
                return;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                messaggioVideo = string.Format("Puntamento errato al servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                messaggioVideo = string.Format("Errore di comunicazione con il servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return;
            }
            catch (Exception Ex)
            {
                messaggioVideo = string.Format("Errore nella chiamata al servizio piani di pagamento: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                erroreTecnico = true;
                return;
            }
            finally
            {
                if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                {
                    string messaggio = messaggioVideo;
                    messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                    string parametri = string.Format("GUID per LogSoap: {0}", guid);
                    GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                //if (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || Utility.IsRiaperturaDomanda(datiPensione.Id))
                //{
                GestioneLogSoap.SalvaLogSoap(responseGestioneConguaglio, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.GestioneCong_6scatti, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                //}
                //else
                //{
                //    GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.InsertCONG003, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                //}
            }
        }

        private static void AggiornaEquoIndennizzoPrivate(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP, out string messaggioVideo)
        {

            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;
            string input = string.Empty;
            string output = string.Empty;
            InserisciConguaglio_Gruppo05_Response responseInserisciConguaglio05 = null;
            Guid guid = Guid.NewGuid();
            try
            {
                InserisciConguaglio_Gruppo05_Request requestInserisciConguaglio05 = new InserisciConguaglio_Gruppo05_Request();
                requestInserisciConguaglio05 = ValorizzaInserisciConguaglio05Request(datiPensione, recordDatiFondoINPDAP);
                GestioneLogSoap.SalvaLogSoap(requestInserisciConguaglio05, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.InsertCong005, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                using (var proxy = new WSPianiDiPagamentoClient())
                using (var scope = new OperationContextScope(proxy.InnerChannel))
                {
                    IdentitySrv identity = new IdentitySrv();
                    identity.AppName = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] : "";
                    identity.AppKey = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] : "";
                    identity.UserId = datiPensione.MatricolaUtenteAcquisizione;
                    identity.IdentityProvider = "AD";


                    MessageHeader identityHeader = MessageHeader.CreateHeader("Identity", "https://inps.it", identity);
                    OperationContext.Current.OutgoingMessageHeaders.Add(identityHeader);

                    responseInserisciConguaglio05 = proxy.InsertCONG005(requestInserisciConguaglio05);

                    if (responseInserisciConguaglio05 == null || (responseInserisciConguaglio05 != null && responseInserisciConguaglio05.Esito != null && responseInserisciConguaglio05.Esito.Risultato == RisultatoInserisciConguaglio.NOK))
                    {
                        messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                        return;
                    }

                }

            }
            catch (SoapException exception)
            {
                messaggioVideo = exception.Message + exception.Detail != null ? exception.Detail.InnerText : string.Empty;
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                messaggioVideo = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                erroreTecnico = true;
                return;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                messaggioVideo = string.Format("Puntamento errato al servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                messaggioVideo = string.Format("Errore di comunicazione con il servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return;
            }
            catch (Exception Ex)
            {
                messaggioVideo = string.Format("Errore nella chiamata al servizio piani di pagamento: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                erroreTecnico = true;
                return;
            }
            finally
            {
                if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                {
                    string messaggio = messaggioVideo;
                    messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                    string parametri = string.Format("GUID per LogSoap: {0}", guid);
                    GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                GestioneLogSoap.SalvaLogSoap(responseInserisciConguaglio05, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.InsertCong005, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
            }
        }

        private static void AggiornaIndennitaSpecialePrivate(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP, out string messaggioVideo, out bool isCodiceEsito9)
        {

            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            string stackTrace = null;
            string input = string.Empty;
            string output = string.Empty;
            isCodiceEsito9 = false;
            GestioneConguaglio_Indennizzo_Response responseGestCongIndennizzo = null;
            Guid guid = Guid.NewGuid();
            try
            {
                GestioneConguaglio_Indennizzo_Request requestGestCongIndennizzo = new GestioneConguaglio_Indennizzo_Request();
                requestGestCongIndennizzo = ValorizzaIndennitaSpeciale(datiPensione, recordDatiFondoINPDAP);
                GestioneLogSoap.SalvaLogSoap(requestGestCongIndennizzo, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.GestioneCong_Indennizzo, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                using (var proxy = new WSPianiDiPagamentoClient())
                using (var scope = new OperationContextScope(proxy.InnerChannel))
                {
                    IdentitySrv identity = new IdentitySrv();
                    identity.AppName = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] : "";
                    identity.AppKey = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] : "";
                    identity.UserId = datiPensione.MatricolaUtenteAcquisizione;
                    identity.IdentityProvider = "AD";


                    MessageHeader identityHeader = MessageHeader.CreateHeader("Identity", "https://inps.it", identity);
                    OperationContext.Current.OutgoingMessageHeaders.Add(identityHeader);

                    responseGestCongIndennizzo = proxy.GestioneCong_Indennizzo(requestGestCongIndennizzo);

                    if (responseGestCongIndennizzo == null || (responseGestCongIndennizzo != null && responseGestCongIndennizzo.Esito != null && responseGestCongIndennizzo.Esito.Risultato == RisultatoGestioneConguaglio.NOK)) //CONTROLLA
                    {
                        messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                        return;
                    }
                    else if (responseGestCongIndennizzo != null && responseGestCongIndennizzo.Esito != null && responseGestCongIndennizzo.Esito.Risultato == RisultatoGestioneConguaglio.OK && responseGestCongIndennizzo.Esito.Codice == 9)
                    {
                        isCodiceEsito9 = true;
                    }

                }

            }
            catch (SoapException exception)
            {
                messaggioVideo = exception.Message + exception.Detail != null ? exception.Detail.InnerText : string.Empty;
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                messaggioVideo = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                erroreTecnico = true;
                return;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                messaggioVideo = string.Format("Puntamento errato al servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                messaggioVideo = string.Format("Errore di comunicazione con il servizio piani di pagamento | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return;
            }
            catch (Exception Ex)
            {
                messaggioVideo = string.Format("Errore nella chiamata al servizio piani di pagamento: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                erroreTecnico = true;
                return;
            }
            finally
            {
                if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                {
                    string messaggio = messaggioVideo;
                    messaggioVideo = "Errore nel processo di aggiornamento della domanda sui sistemi piani di pagamento";
                    string parametri = string.Format("GUID per LogSoap: {0}", guid);
                    GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                GestioneLogSoap.SalvaLogSoap(responseGestCongIndennizzo, Utility.Servizio.SrvPianiDiPagamento, Utility.MetodoServizio.GestioneCong_Indennizzo, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
            }
        }

        private static bool ControllaStatoPensionePerAggiornamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoSIN)
                return true;
            else
                return false;
        }

        private static bool ControllaStatoPensionePerAggiornamentoNoteDiDebito(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoNoteDebito)
                return true;
            else
                return false;
        }

        private static bool ControllaStatoPensionePerAggiornamentoPianiDiPagamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNo6Scatti)
                return true;
            else
                return false;
        }

        private static bool ControllaStatoPensionePerAggiornamentoEquoInd(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoEquoInd)
                return true;
            else
                return false;
        }

        private static bool ControllaStatoPensionePerAggiornamentoIndennSpec(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoIndennSpec)
                return true;
            else
                return false;
        }

        public static AcquisizioneEventoRequest ValorizzaAcquisizioneEventoRequest(GestionePensione.DatiPensione datiPensione, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, bool isGDPMiglioramentiContrattuali)
        {
            AcquisizioneEventoRequest request = new AcquisizioneEventoRequest();
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiFondoINPDAP = null;
            GestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoINPDAP);
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;


            if (Utility.IsDomandaMiglioramentiContrattuali(datiPensione) || isGDPMiglioramentiContrattuali)
            {
                MiglioramentiContrattuali datiMiglioramentiContrattuali = null;
                List<PensioneINPDAP> listaDatiPensioneINPDAP = new List<PensioneINPDAP>();
                List<GestioneDecodifica.DecodificaCausaCessazione> elencoDecodificaCausaCessazione = new List<GestioneDecodifica.DecodificaCausaCessazione>();
                List<QuoteMiglioramentiContrattuali> lstQuoteMiglioramentiContrattuali = new List<QuoteMiglioramentiContrattuali>();
                List<QuotePensione> lstQuotePensione = new List<QuotePensione>();
                DAMiglioramentiContrattuali.GetDatiMiglioramentiContrattualiByIdPensione(datiPensione.Id, out datiMiglioramentiContrattuali);
                DAMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiByIdPensione(datiPensione.Id, out lstQuoteMiglioramentiContrattuali);
                string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                         datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");
                string errori = string.Empty;
                float importoPensione = 0;
                float pensioneInPagamento = 0;
                float pensioneOriginariaPerequata = 0;
                float importoQuota = 0;
                //string importoPensioneStringa = string.Empty;
                if (lstQuoteMiglioramentiContrattuali != null && lstQuoteMiglioramentiContrattuali.Count() > 0 && (datiMiglioramentiContrattuali != null || isGDPMiglioramentiContrattuali))
                {
                    DAGestioneCalcolo.GetQuotePensioneByIdPensione(datiPensione.Id, out lstQuotePensione);
                    DAGestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(datiPensione.Id, out listaDatiPensioneINPDAP);
                    ElementoDatiTGP5[] listaDatiTGP5 = null;
                    GestioneDatiPensioni.GetDatiTGP5ByChiavePensione(datiPensione.NDomus, chiavePensione, out listaDatiTGP5, out errori);

                    request.tipoEvento = "M01";
                    request.parametriInput = new Parametriinput();
                    request.parametriInput.operatore = datiPensione.MatricolaUtenteAcquisizione;
                    request.parametriInput.codiceSede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
                    request.parametriInput.tipoLav = datiPensione.IsCumuloAutomatica.GetValueOrDefault() ? "A" : "M";
                    if (!isGDPMiglioramentiContrattuali)
                    {
                        request.parametriInput.idEnteSostImp = datiMiglioramentiContrattuali != null ? datiMiglioramentiContrattuali.CodiceEnte : null;
                        request.parametriInput.motivoCessazioneServizio = datiMiglioramentiContrattuali != null ? (datiMiglioramentiContrattuali.CodiceCessazione + " - " + datiMiglioramentiContrattuali.MotivoCessazione) : null;
                    }
                    else
                    {
                        if (listaDatiPensioneINPDAP != null && listaDatiPensioneINPDAP.Count > 0)
                        {
                            request.parametriInput.idEnteSostImp = listaDatiPensioneINPDAP.FirstOrDefault().CfAmministrazione != null ? listaDatiPensioneINPDAP.FirstOrDefault().CfAmministrazione : "";
                        }

                        GestioneDecodifica.GetElencoCodiciCausaCessazione(out elencoDecodificaCausaCessazione);
                        if (elencoDecodificaCausaCessazione != null && elencoDecodificaCausaCessazione.Count > 0)
                        {
                            request.parametriInput.motivoCessazioneServizio = elencoDecodificaCausaCessazione.FirstOrDefault().Descrizione;
                        }
                    }
                    request.parametriInput.idSedeCompetenza = null;

                    List<GestioneDecodifica.DecEnteGestioneFondo> listaDecEnteGestioneFondo = null;
                    GestioneDecodifica.GetDecEnteGestioneFondo(out listaDecEnteGestioneFondo);
                    QuotePensione quotaPensione = null;
                    if (lstQuotePensione != null && lstQuotePensione.Count > 0)
                    {
                        quotaPensione = lstQuotePensione.FirstOrDefault(x => x.EnteGestioneFondo == 15 || x.EnteGestioneFondo == 16 || x.EnteGestioneFondo == 17 || x.EnteGestioneFondo == 18 || x.EnteGestioneFondo == 19);
                    }
                    if (quotaPensione != null)
                    {
                        switch (quotaPensione.EnteGestioneFondo)
                        {
                            case 15:
                                request.parametriInput.idCassa = "CTPS";
                                break;
                            case 16:
                                request.parametriInput.idCassa = "CPDEL";
                                break;
                            case 17:
                                request.parametriInput.idCassa = "CPI";
                                break;
                            case 18:
                                request.parametriInput.idCassa = "CPS";
                                break;
                            case 19:
                                request.parametriInput.idCassa = "CPUG";
                                break;
                            default:
                                request.parametriInput.idCassa = ""; //CHIEDI VALORE DI DEFAULT
                                break;

                        }
                    }
                    else
                    {
                        request.parametriInput.idCassa = "";// chiedere valore di def
                    }

                    request.parametriInput.nomeCognome = datiAnagrafici.Nome + " " + datiAnagrafici.Cognome;
                    request.parametriInput.dataDec = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0') + "/" +
                        datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') + "/" + datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0')) : null;

                    request.parametriInput.dataNascita = datiAnagrafici.DataNascita.Value.Day.ToString().PadLeft(2, '0') + "/" +
                        datiAnagrafici.DataNascita.Value.Month.ToString().PadLeft(2, '0') + "/" + datiAnagrafici.DataNascita.Value.Year.ToString().PadLeft(4, '0');

                    request.parametriInput.codFiscale = datiAnagrafici.CodiceFiscale;
                    request.parametriInput.numeroDomus = datiPensione.NDomus.ToString(); //sul doc corrisponde a numPOsizione
                    request.parametriInput.numIscrizione = null;
                    request.parametriInput.cassaCess = null;
                    request.parametriInput.tipoPens = Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria) || Utility.IsDomandaIOCUM(datiPensione.SiglaCategoria) ? "1" : (Utility.IsDomandaSOCUM(datiPensione.SiglaCategoria) && Utility.IsDomandaPensioneIndiretta(datiPensione)) ? "2" : null; // sul doc è tipoPensione
                    request.parametriInput.dataCessazioneServizio = datiPensione.FineAssicurazione.Value.Day.ToString().PadLeft(2, '0') + "/" +
                        datiPensione.FineAssicurazione.Value.Month.ToString().PadLeft(2, '0') + "/" + datiPensione.FineAssicurazione.Value.Year.ToString().PadLeft(4, '0');
                   
                    //if (listaDatiTGP5 != null && listaDatiTGP5.Count() > 0)

                    //{
                    //    ElementoDatiTGP5 datoTGP5 = null;
                    //    int index = -1;
                    //    DateTime decorrenzaR1 = new DateTime();
                    //    DateTime.TryParse(lstQuoteMiglioramentiContrattuali[0].DataDecorrenza, out decorrenzaR1);
                    //    if (decorrenzaR1 != null)
                    //    {
                    //        string decorrenzaStr = decorrenzaR1.ToString("yyyyMM");

                    //        datoTGP5 = decorrenzaStr != null ? listaDatiTGP5.FirstOrDefault(el => GestioneDatiPensioni.GetValueDatoGP(el.GP5HC01Z) == decorrenzaStr) : null;
                    //        index = datoTGP5 != null ? Array.IndexOf(listaDatiTGP5, datoTGP5) : -1;
                    //    }


                    //    if (datoTGP5 != null && index >= 0 && index <= listaDatiTGP5.Count())
                    //    {

                    //        if (listaDatiTGP5[index].GP5HG00 != null && listaDatiTGP5[index].GP5HG00.Count() > 0)
                    //        {
                    //            GP5HG00Type risultatoDatoTGP5 = null;
                    //            int codiceNumerico;
                    //            //var risultatoDatoTGP5 = listaDatiTGP5[index].GP5HG00.FirstOrDefault(x => int.TryParse(x.GP5HG01.Valore.Codice, out int codiceNumerico) && codiceNumerico >= 742 && codiceNumerico <= 746);
                    //            foreach (var elemento in listaDatiTGP5[index].GP5HG00)
                    //            {
                    //                if (int.TryParse(elemento.GP5HG01.Valore.Codice, out codiceNumerico))
                    //                {
                    //                    if (codiceNumerico >= 742 && codiceNumerico <= 746)
                    //                    {
                    //                        risultatoDatoTGP5 = elemento;
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            int indice = risultatoDatoTGP5 != null ? Array.IndexOf(listaDatiTGP5[index].GP5HG00, risultatoDatoTGP5) : -1;
                    //            if (risultatoDatoTGP5 != null && indice >= 0)
                    //            {

                    //                importoPensioneStringa = GestioneDatiPensioni.GetValueDatoGP(listaDatiTGP5[index].GP5HG00[indice].GP5HG02E);

                    //            }
                    //        }
                    //    }

                    //    if (importoPensioneStringa != null && importoPensioneStringa != "" && float.TryParse(importoPensioneStringa, NumberStyles.Float, CultureInfo.InvariantCulture, out importoPensione))
                    //    {
                    //        request.parametriInput.importoPensione = importoPensione;
                    //    }
                    //}

                    request.parametriInput.totaleQuoteCaricoAtt = 0;
                    request.parametriInput.totaleQuoteCaricoPrec = 0;
                    request.parametriInput.totaleQuote = 0;
                    List<QuoteMiglioramentiContrattuali> quoteValide = new List<QuoteMiglioramentiContrattuali>();

                    foreach (var quota in lstQuoteMiglioramentiContrattuali)
                    {
                        DateTime data;
                        if (DateTime.TryParse(quota.DataDecorrenza, out data))
                        {
                            quoteValide.Add(quota);
                        }
                    }
                    var quotaMiglioramentiContrattualePiuRecente = quoteValide.OrderByDescending(x => DateTime.Parse(x.DataDecorrenza)).FirstOrDefault();
                    DateTime dataMiglioramentiPiuRecente = new DateTime();
                    if (quotaMiglioramentiContrattualePiuRecente != null && quotaMiglioramentiContrattualePiuRecente.DataDecorrenza != null)
                    {
                        DateTime.TryParse(quotaMiglioramentiContrattualePiuRecente.DataDecorrenza, out dataMiglioramentiPiuRecente);
                        request.parametriInput.dataRiferimento = dataMiglioramentiPiuRecente != null ? (dataMiglioramentiPiuRecente.Day.ToString().PadLeft(2, '0') + "/" +
                            dataMiglioramentiPiuRecente.Month.ToString().PadLeft(2, '0') + "/" + dataMiglioramentiPiuRecente.Year.ToString().PadLeft(4, '0')) : "";
                    }

                    request.parametriInput.dataNascitaTit = null;
                    request.parametriInput.etaAnni = null;
                    request.parametriInput.coefTab = 0;
                    request.parametriInput.importoQuotaACarico = 0;
                    request.parametriInput.valoreCapitaleRisult = 0;
                    request.parametriInput.valoreCapitalePrec = 0;
                    request.parametriInput.valoreCapitale = 0;

                    request.parametriInput.importi = new List<Importi>();

                    string importoPensioneStringaPerPensioneInPagamento = "";
                    float importoPensionePerPensioneInPagam = 0;
                    foreach (var quota in lstQuoteMiglioramentiContrattuali)
                    {
                        if (listaDatiTGP5 != null && listaDatiTGP5.Count() > 0)
                        {
                            DateTime decorrenzaQuota = new DateTime();
                            DateTime.TryParse(quota.DataDecorrenza, out decorrenzaQuota);
                            var datoTGP5 = decorrenzaQuota != null ? listaDatiTGP5.FirstOrDefault(x => GestioneDatiPensioni.GetValueDatoGP(x.GP5HC01Z) == decorrenzaQuota.ToString("yyyyMM")) : null;
                            int index = datoTGP5 != null ? Array.IndexOf(listaDatiTGP5, datoTGP5) : -1;
                            if (datoTGP5 != null && index >= 0 && index <= listaDatiTGP5.Count())
                            {
                                if (listaDatiTGP5[index].GP5HG00 != null && listaDatiTGP5[index].GP5HG00.Count() > 0)
                                {
                                    GP5HG00Type risultatoDatoTGP5 = null;
                                    int codiceNumerico;

                                    foreach (var elemento in listaDatiTGP5[index].GP5HG00)
                                    {
                                        if (int.TryParse(elemento.GP5HG01.Valore.Codice, out codiceNumerico))
                                        {
                                            if (codiceNumerico >= 742 && codiceNumerico <= 746)
                                            {
                                                risultatoDatoTGP5 = elemento;
                                                break;
                                            }
                                        }
                                    }
                                    int indice = risultatoDatoTGP5 != null ? Array.IndexOf(listaDatiTGP5[index].GP5HG00, risultatoDatoTGP5) : -1;
                                    if (risultatoDatoTGP5 != null && indice >= 0)
                                    {
                                        importoPensioneStringaPerPensioneInPagamento = GestioneDatiPensioni.GetValueDatoGP(listaDatiTGP5[index].GP5HG00[indice].GP5HG02E);
                                    }
                                }


                                if (importoPensioneStringaPerPensioneInPagamento != null && importoPensioneStringaPerPensioneInPagamento != "")
                                {
                                    DateTime decorrenzaR1 = new DateTime();
                                    float.TryParse(importoPensioneStringaPerPensioneInPagamento, NumberStyles.Float, CultureInfo.InvariantCulture, out importoPensionePerPensioneInPagam);
                                    if (DateTime.TryParse(lstQuoteMiglioramentiContrattuali[0].DataDecorrenza, out decorrenzaR1) && decorrenzaR1 == decorrenzaQuota)
                                    {
                                        importoPensione = importoPensionePerPensioneInPagam;
                                        request.parametriInput.importoPensione = importoPensione;
                                    }
                                }

                                var elementoDatoTGP5perPensioneInPagamento = listaDatiTGP5[index].GP5HG00.FirstOrDefault(x => x.GP5HG01.Valore.Codice == "A51");
                                int indicePerPensioneInPagamento = elementoDatoTGP5perPensioneInPagamento != null ? Array.IndexOf(listaDatiTGP5[index].GP5HG00, elementoDatoTGP5perPensioneInPagamento) : -1;
                                if (elementoDatoTGP5perPensioneInPagamento != null && indicePerPensioneInPagamento >= 0)
                                {
                                    string pensioneOriginariaPerequataStringa = GestioneDatiPensioni.GetValueDatoGP(listaDatiTGP5[index].GP5HG00[indicePerPensioneInPagamento].GP5HG02E);
                                    if (importoPensioneStringaPerPensioneInPagamento != null && importoPensioneStringaPerPensioneInPagamento != string.Empty && !string.IsNullOrEmpty(pensioneOriginariaPerequataStringa) && importoPensionePerPensioneInPagam != 0 && float.TryParse(pensioneOriginariaPerequataStringa, NumberStyles.Float, CultureInfo.InvariantCulture, out pensioneOriginariaPerequata))
                                    {
                                        pensioneInPagamento = importoPensionePerPensioneInPagam - pensioneOriginariaPerequata;
                                    }
                                    else if (elementoDatoTGP5perPensioneInPagamento == null && indicePerPensioneInPagamento < 0 && importoPensionePerPensioneInPagam > 0)
                                    {
                                        pensioneInPagamento = importoPensionePerPensioneInPagam;
                                    }
                                }



                                float.TryParse(quota.Quota, out importoQuota);
                                var importo = new Importi
                                {
                                    decorrenza = quota.DataDecorrenza,
                                    pensioneConMiglioramentiContrattuali = importoQuota,
                                    aliquotaNucleo = 0,
                                    pensioneInPagamento = pensioneInPagamento,
                                    pensioneFinoAl = 0,
                                    pensioneDalAl = 0,
                                    pensioneOriginariaPerequata = pensioneOriginariaPerequata,
                                    ratei = null,
                                    quotaCarico = 0,
                                    precedenteQuotaCarico = 0,
                                };
                                request.parametriInput.importi.Add(importo);
                            }
                        }
                    }
                    if (importoPensione == 0)
                    {
                        ElementoDatiTGP6[] listaDatiTGP6 = null;
                        DatiTGP6Request requestDatiTGP6 = new DatiTGP6Request();
                        requestDatiTGP6.ChiavePensione = chiavePensione;
                        GestioneDatiPensioni.GetDatiTGP6ByChiavePensione(datiPensione.NDomus, chiavePensione, out listaDatiTGP6, out errori);

                        //if (listaDatiTGP6 != null && listaDatiTGP6.Count() > 0)
                        //{
                        //    DateTime decorrenzaR1 = new DateTime();
                        //    DateTime.TryParse(lstQuoteMiglioramentiContrattuali[0].DataDecorrenza, out decorrenzaR1);
                        //    var datoTGP6 = decorrenzaR1 != null ? listaDatiTGP6.FirstOrDefault(x => GestioneDatiPensioni.GetValueDatoGP(x.GP6KC01Z) == decorrenzaR1.ToString("yyyyMM")) : null;
                        //    int index = datoTGP6 != null ? Array.IndexOf(listaDatiTGP6, datoTGP6) : -1;
                        //    if (datoTGP6 != null && index >= 0 && index <= listaDatiTGP6.Count())
                        //    {

                        //        if (listaDatiTGP6[index].GP6HG00 != null && listaDatiTGP6[index].GP6HG00.Count() > 0)
                        //        {
                        //            GP6HG00Type risultatoDatoTGP6 = null;
                        //            int codiceNumerico;
                        //            foreach (var elemento in listaDatiTGP6[index].GP6HG00)
                        //            {
                        //                if (int.TryParse(elemento.GP6HG01.Valore.Codice, out codiceNumerico))
                        //                {
                        //                    if (codiceNumerico >= 742 && codiceNumerico <= 746)
                        //                    {
                        //                        risultatoDatoTGP6 = elemento;
                        //                        break;
                        //                    }
                        //                }
                        //            }
                        //            //var risultatoDatoTGP6 = listaDatiTGP6[index].GP6HG00.FirstOrDefault(x => int.TryParse(x.GP6HG01.Valore.Codice, out int codiceNumerico) && codiceNumerico >= 742 && codiceNumerico <= 746);
                        //            int indice = risultatoDatoTGP6 != null ? Array.IndexOf(listaDatiTGP6[index].GP6HG00, risultatoDatoTGP6) : -1;
                        //            if (risultatoDatoTGP6 != null && indice >= 0)
                        //            {

                        //                importoPensioneStringa = GestioneDatiPensioni.GetValueDatoGP(listaDatiTGP6[index].GP6HG00[indice].GP6HG02E);

                        //            }
                        //        }
                        //    }

                        //    if (importoPensioneStringa != null && importoPensioneStringa != "" && float.TryParse(importoPensioneStringa, NumberStyles.Float, CultureInfo.InvariantCulture, out importoPensione))
                        //    {
                        //        request.parametriInput.importoPensione = importoPensione;
                        //    }

                        foreach (var quota in lstQuoteMiglioramentiContrattuali)
                        {
                            if (listaDatiTGP6 != null && listaDatiTGP6.Count() > 0)
                            {
                                DateTime decorrenzaQuota = new DateTime();
                                DateTime.TryParse(quota.DataDecorrenza, out decorrenzaQuota);
                                var TGP6dato = decorrenzaQuota != null ? listaDatiTGP6.FirstOrDefault(x => GestioneDatiPensioni.GetValueDatoGP(x.GP6KC01Z) == decorrenzaQuota.ToString("yyyyMM")) : null;
                                int indexElemento = TGP6dato != null ? Array.IndexOf(listaDatiTGP6, TGP6dato) : -1;
                                if (TGP6dato != null && indexElemento >= 0 && indexElemento <= listaDatiTGP6.Count())
                                {
                                    GP6HG00Type risultatoDatoTGP6 = null;
                                    int codiceNumerico;

                                    //var risultatoDatoTGP5 = listaDatiTGP5[index].GP5HG00.FirstOrDefault(x => int.TryParse(x.GP5HG01.Valore.Codice, out int codiceNumerico) && codiceNumerico >= 742 && codiceNumerico <= 746);
                                    foreach (var elemento in listaDatiTGP6[indexElemento].GP6HG00)
                                    {
                                        if (int.TryParse(elemento.GP6HG01.Valore.Codice, out codiceNumerico))
                                        {
                                            if (codiceNumerico >= 742 && codiceNumerico <= 746)
                                            {
                                                risultatoDatoTGP6 = elemento;
                                                break;
                                            }
                                        }
                                    }
                                    int indice = risultatoDatoTGP6 != null ? Array.IndexOf(listaDatiTGP6[indexElemento].GP6HG00, risultatoDatoTGP6) : -1;
                                    if (risultatoDatoTGP6 != null && indice >= 0)
                                    {

                                        importoPensioneStringaPerPensioneInPagamento = GestioneDatiPensioni.GetValueDatoGP(listaDatiTGP6[indexElemento].GP6HG00[indice].GP6HG02E);

                                    }


                                    if (importoPensioneStringaPerPensioneInPagamento != null && importoPensioneStringaPerPensioneInPagamento != "")
                                    {
                                        DateTime decorrenzaR1 = new DateTime();
                                        float.TryParse(importoPensioneStringaPerPensioneInPagamento, NumberStyles.Float, CultureInfo.InvariantCulture, out importoPensionePerPensioneInPagam);
                                        if (DateTime.TryParse(lstQuoteMiglioramentiContrattuali[0].DataDecorrenza, out decorrenzaR1) && decorrenzaR1 == decorrenzaQuota)
                                        {
                                            importoPensione = importoPensionePerPensioneInPagam;
                                            request.parametriInput.importoPensione = importoPensione;
                                        }
                                    }

                                    var elementoDatoTGP6perPensioneInPagamento = listaDatiTGP6[indexElemento].GP6HG00.FirstOrDefault(x => x.GP6HG01.Valore.Codice == "A51");
                                    int indicePerPensioneInPagamento = TGP6dato != null ? Array.IndexOf(listaDatiTGP6[indexElemento].GP6HG00, elementoDatoTGP6perPensioneInPagamento) : -1;
                                    if (elementoDatoTGP6perPensioneInPagamento != null && indicePerPensioneInPagamento >= 0)
                                    {
                                        string pensioneOriginariaPerequataStringa = GestioneDatiPensioni.GetValueDatoGP(listaDatiTGP6[indexElemento].GP6HG00[indicePerPensioneInPagamento].GP6HG02E);
                                        if (importoPensioneStringaPerPensioneInPagamento != null && importoPensioneStringaPerPensioneInPagamento != string.Empty && !string.IsNullOrEmpty(pensioneOriginariaPerequataStringa) && importoPensione != 0 && float.TryParse(pensioneOriginariaPerequataStringa, NumberStyles.Float, CultureInfo.InvariantCulture, out pensioneOriginariaPerequata))
                                        {
                                            pensioneInPagamento = importoPensionePerPensioneInPagam - pensioneOriginariaPerequata;
                                        }
                                    }
                                    else if (elementoDatoTGP6perPensioneInPagamento == null && indicePerPensioneInPagamento < 0 && importoPensionePerPensioneInPagam > 0)
                                    {
                                        pensioneInPagamento = importoPensionePerPensioneInPagam;

                                    }

                                    float.TryParse(quota.Quota, out importoQuota);
                                    var importo = new Importi
                                    {
                                        decorrenza = quota.DataDecorrenza,
                                        pensioneConMiglioramentiContrattuali = importoQuota,
                                        aliquotaNucleo = 0,
                                        pensioneInPagamento = pensioneInPagamento,
                                        pensioneFinoAl = 0,
                                        pensioneDalAl = 0,
                                        pensioneOriginariaPerequata = pensioneOriginariaPerequata,
                                        ratei = null,
                                        quotaCarico = 0,
                                        precedenteQuotaCarico = 0,
                                    };
                                    request.parametriInput.importi.Add(importo);
                                }
                            }
                        }

                    }

                }
            }

            else
            {
                request.tipoEvento = datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG || datiPensione.TipoFelpe == null ? "B01" : "B02";
                request.parametriInput = new Parametriinput();
                request.parametriInput.operatore = datiPensione.MatricolaUtenteAcquisizione;
                request.parametriInput.codiceSede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
                request.parametriInput.tipoLav = datiPensione.TipoFelpe != null ? "A" : "M";

                if (datiPensione.SiglaCategoria.StartsWith("S"))
                {
                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    INPS.Pensioni.Liquidazione.Entity.DanteCausaEntity entityDanteCausa = new INPS.Pensioni.Liquidazione.Entity.DanteCausaEntity();
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                    if (datiDanteCausa != null)
                    {
                        entityDanteCausa.AnagraficaDC = new Entity.AnagraficaDC();
                        GestioneDanteCausa.GetDatiAnagraficaDCByIdPensione(datiPensione.Id, ref entityDanteCausa);
                        Utility.ValorizzaOggetti(datiDanteCausa, entityDanteCausa.AnagraficaDC);
                        request.parametriInput.titolare = entityDanteCausa.AnagraficaDC.CodiceFiscale;
                        List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                        List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
                        GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(datiPensione.Id, out listaAventiDiritto, out listaAnagrafiche);

                        if (listaAventiDiritto != null && listaAventiDiritto.Count > 0)
                        {
                            request.parametriInput.nucleo = new Nucleo[listaAventiDiritto.Count];
                            var i = 0;

                            foreach (var fam in listaAventiDiritto)
                            {
                                request.parametriInput.nucleo[i] = new Nucleo();
                                request.parametriInput.nucleo[i].cf = listaAnagrafiche.Find(x => x.Id == fam.IdAnagrafica) != null ? listaAnagrafiche.Find(x => x.Id == fam.IdAnagrafica).CodiceFiscale : null;

                                switch (fam.DecParentelaDA)
                                {
                                    case 'C':
                                        request.parametriInput.nucleo[i].legame = "V";
                                        break;
                                    case 'A':
                                        request.parametriInput.nucleo[i].legame = "G";
                                        break;
                                    case 'I':
                                    case 'M':
                                    case 'S':
                                    case 'U':
                                        request.parametriInput.nucleo[i].legame = "F";
                                        break;
                                    default:
                                        request.parametriInput.nucleo[i].legame = "C";
                                        break;
                                }
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    request.parametriInput.titolare = datiAnagrafici.CodiceFiscale;
                }
                request.parametriInput.dataDec = datiPensione.DecorrenzaOriginaria.HasValue ? (datiPensione.DecorrenzaOriginaria.Value.Day.ToString().PadLeft(2, '0') + "/" +
                    datiPensione.DecorrenzaOriginaria.Value.Month.ToString().PadLeft(2, '0') + "/" + datiPensione.DecorrenzaOriginaria.Value.Year.ToString().PadLeft(4, '0')) : null;

                request.parametriInput.tipoPens = datiPensione.SiglaCategoria.StartsWith("V") || datiPensione.SiglaCategoria.StartsWith("I") ? "D" : "I";
                string categoriaNumerica = datiPensione.GetCodCategoria();
                if (categoriaNumerica.Length == 4)
                    categoriaNumerica = categoriaNumerica.Substring(1, 3);
                request.parametriInput.certificatoPens = categoriaNumerica + "-" + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                    datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + "-" + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");

                if (Utility.IsRicostituzione(datiPensione.Gruppo))
                    request.parametriInput.tipoLiq = "R";
                else if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                    request.parametriInput.tipoLiq = "T";
                else
                {
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                    if (datiIstruttoria != null && datiIstruttoria.CodiceComunicazioneCampo3.HasValue)
                        request.parametriInput.tipoLiq = "P"; //CodiceComunicazioneCampo3 istruttoria
                    else
                        request.parametriInput.tipoLiq = "L";
                }

                request.parametriInput.dataAtto = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.Day.ToString().PadLeft(2, '0') + "/" +
                    datiPensione.DataElaborazione.Value.Month.ToString().PadLeft(2, '0') + "/" + datiPensione.DataElaborazione.Value.Year.ToString().PadLeft(4, '0') : string.Empty;
                request.parametriInput.numeroDomus = datiPensione.NDomus.ToString();

                if (listaDatiFondoINPDAP != null && listaDatiFondoINPDAP.Count > 0)
                {
                    request.parametriInput.cfSedeApp = listaDatiFondoINPDAP.FirstOrDefault().CfAmministrazione != null ? listaDatiFondoINPDAP.FirstOrDefault().CfAmministrazione.PadLeft(11, '0') : "";
                    request.parametriInput.prgSedeApp = listaDatiFondoINPDAP.FirstOrDefault().ProgAmministrazione != null ? listaDatiFondoINPDAP.FirstOrDefault().ProgAmministrazione.PadLeft(5, '0') : "";
                }
                request.parametriInput.idCassa = datiPensione.SiglaCategoria.Substring(2, datiPensione.SiglaCategoria.Length - 2).Trim();
                request.parametriInput.importoRec = recordDatiFondoINPDAP != null ? (float)recordDatiFondoINPDAP.RMSSenzaLegge33670QA.Value : 0;

                //PAL prendere la minore disponibile a meno che non sia 0
                if (recordDatiFondoINPDAP != null && recordDatiFondoINPDAP.PensioneAnnuaLorda != null)
                {
                    request.parametriInput.pal = (float)(recordDatiFondoINPDAP.PensioneAnnuaLorda707 != null && recordDatiFondoINPDAP.PensioneAnnuaLorda707 > 0 ?
                        (recordDatiFondoINPDAP.PensioneAnnuaLorda < recordDatiFondoINPDAP.PensioneAnnuaLorda707 ? recordDatiFondoINPDAP.PensioneAnnuaLorda : recordDatiFondoINPDAP.PensioneAnnuaLorda707) : recordDatiFondoINPDAP.PensioneAnnuaLorda);
                }
            }
            return request;
        }


        public static InserisciConguaglio_Gruppo03_Request ValorizzaInserisciConguaglioRequest(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP)
        {
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
            if (string.IsNullOrEmpty(datiAnagraficiTitolare.CodiceFiscale))
                datiAnagraficiTitolare.CodiceFiscale = string.Empty;

            string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                           datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");

            InserisciConguaglio_Gruppo03_Request request = new InserisciConguaglio_Gruppo03_Request();
            request.ProceduraChiamante = "PN812";
            request.PianoDiPagamento = new PianoDiPagamentoInserisciConguaglio03();
            request.PianoDiPagamento.Pensione = new PensioneInserisciConguaglio() { categoria = datiPensione.GetCodCategoria().Substring(1), certificato = datiPensione.NCertificato.ToString(), sede = datiPensione.CodiceSedeDestinazione.ToString().Substring(0, 2), zona = datiPensione.CodiceSedeDestinazione.ToString().Substring(2, 2) };
            DateTime DP = (DateTime)(datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria : DateTime.MinValue);
            string dataInizioTrattenuta;
            string rataMensile;
            string errori;
            DateTime GPRata = new DateTime();
            DateTime data = new DateTime();
            DateTime DataInizioTrattenuta = new DateTime(0001, 01, 01);
            DatiTGP1Response risposta = new DatiTGP1Response();
            GestioneDatiPensioni.GetDatiTGP8(datiPensione.NDomus, chiavePensione, out errori, out dataInizioTrattenuta, out rataMensile);
            GestioneDatiPensioni.GetDatiTGP1ByChiavePensione(datiPensione.NDomus.ToString(), chiavePensione, out risposta, out errori);
            if (risposta.ElementoDatiTGP1.GP1AZ51Z != null && risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice != "9999" && risposta.ElementoDatiTGP1.GP1ALZ5 != null && risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice != "00")
            {
                string dataString = risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice + risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice;
                data = DateTime.ParseExact(dataString, "MMyyyy", CultureInfo.InvariantCulture);
                GPRata = data.AddMonths(1);
            }
            else if (dataInizioTrattenuta != null && dataInizioTrattenuta != String.Empty && DateTime.TryParseExact(dataInizioTrattenuta, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DataInizioTrattenuta))
                GPRata = DataInizioTrattenuta;

            int numMesiArretrati = 0;
            if (recordDatiFondoINPDAP != null && recordDatiFondoINPDAP.Count > 0 && Utility.DataSuccessivaA(DP, GPRata))
            {
                request.PianoDiPagamento.DatiPiani = new DatiPianoInserisciConguaglio03[1];
                request.PianoDiPagamento.DatiPiani[0] = new DatiPianoInserisciConguaglio03
                {
                    IdentificativoChiamante = null,
                    CodiceClasseConguaglio = "B77",
                    Prodotto = ProdottoInserisciConguaglio.DEBITO,
                    TipoDeducibilitaTassazione = "3",
                    ImportoComplessivo = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0) * (decimal)(recordDatiFondoINPDAP[0].NumeroRate.HasValue ? recordDatiFondoINPDAP[0].NumeroRate : 0), //CONTROLLA
                    RataMensile = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0), //controll
                    Pensionato = new BeneficiarioPensionatoInserisciConguaglio
                    {
                        codiceFiscale = datiAnagraficiTitolare.CodiceFiscale
                    },
                    Utenza = null,
                    Validatore = null,
                    MatricolaDipendente = datiPensione.MatricolaUtenteAcquisizione,
                    ImportoDeducibileAnnoCorrente = null,
                    ImportoDeducibileAnnoPrecedente = null,
                    ImportoNonDeducibile = null
                };
            }
            else if (recordDatiFondoINPDAP != null && (recordDatiFondoINPDAP.Count > 0) && Utility.DataStrettamenteSuccessivaA(GPRata, DP))
            {

                numMesiArretrati = (((GPRata.Year - DP.Year) * 12) + (GPRata.Month - DP.Month));

                request.PianoDiPagamento.DatiPiani = new DatiPianoInserisciConguaglio03[2];
                request.PianoDiPagamento.DatiPiani[0] = new DatiPianoInserisciConguaglio03
                {
                    IdentificativoChiamante = null,
                    CodiceClasseConguaglio = "B77",
                    Prodotto = ProdottoInserisciConguaglio.DEBITO,
                    TipoDeducibilitaTassazione = "3",
                    ImportoComplessivo = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0) * (decimal)((recordDatiFondoINPDAP[0].NumeroRate.HasValue ? recordDatiFondoINPDAP[0].NumeroRate : 0) - numMesiArretrati),
                    RataMensile = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0),
                    Pensionato = new BeneficiarioPensionatoInserisciConguaglio
                    {
                        codiceFiscale = datiAnagraficiTitolare.CodiceFiscale
                    },
                    Utenza = null,
                    Validatore = null,
                    MatricolaDipendente = datiPensione.MatricolaUtenteAcquisizione,
                    DataInizioTrattenuta = GPRata.ToString("yyyyMM"),
                    ImportoDeducibileAnnoCorrente = null,
                    ImportoDeducibileAnnoPrecedente = null,
                    ImportoNonDeducibile = null
                };
                request.PianoDiPagamento.DatiPiani[1] = new DatiPianoInserisciConguaglio03
                {
                    IdentificativoChiamante = null,
                    CodiceClasseConguaglio = "E59",
                    Prodotto = ProdottoInserisciConguaglio.DEBITO,
                    TipoDeducibilitaTassazione = "3",
                    ImportoComplessivo = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0) * numMesiArretrati,
                    RataMensile = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0) * numMesiArretrati,
                    Pensionato = new BeneficiarioPensionatoInserisciConguaglio
                    {
                        codiceFiscale = datiAnagraficiTitolare.CodiceFiscale
                    },
                    Utenza = null,
                    Validatore = null,
                    MatricolaDipendente = datiPensione.MatricolaUtenteAcquisizione,
                    ImportoDeducibileAnnoCorrente = null,
                    ImportoDeducibileAnnoPrecedente = null,
                    ImportoNonDeducibile = null
                };
            }

            return request;
        }

        public static GestioneConguaglio_6scatti_Request ValorizzaGestioneConguaglioRequest(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP)
        {
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
            if (string.IsNullOrEmpty(datiAnagraficiTitolare.CodiceFiscale))
                datiAnagraficiTitolare.CodiceFiscale = string.Empty;

            string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                           datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");

            GestioneConguaglio_6scatti_Request request = new GestioneConguaglio_6scatti_Request();
            request.ProceduraChiamante = "PN812";
            request.PianoDiPagamento = new PianoDiPagamentoGestioneConguaglio();
            request.PianoDiPagamento.Pensione = new PensioneGestioneConguaglio { categoria = datiPensione.GetCodCategoria().Substring(1), certificato = datiPensione.NCertificato.ToString(), sede = datiPensione.CodiceSedeDestinazione.ToString().Substring(0, 2), zona = datiPensione.CodiceSedeDestinazione.ToString().Substring(2, 2) };
            DateTime DP = (DateTime)(datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria : DateTime.MinValue);
            string dataInizioTrattenuta;
            string rataMensile;
            string errori;
            DateTime GPRata = new DateTime();
            DateTime data = new DateTime();
            DateTime DataInizioTrattenuta = new DateTime(0001, 01, 01);
            DatiTGP1Response risposta = new DatiTGP1Response();
            GestioneDatiPensioni.GetDatiTGP8(datiPensione.NDomus, chiavePensione, out errori, out dataInizioTrattenuta, out rataMensile);
            GestioneDatiPensioni.GetDatiTGP1ByChiavePensione(datiPensione.NDomus.ToString(), chiavePensione, out risposta, out errori);
            if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1AZ51Z != null && risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice != "9999" && risposta.ElementoDatiTGP1.GP1ALZ5 != null && risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice != "00")
            {
                string dataString = risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice + risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice;
                data = DateTime.ParseExact(dataString, "MMyyyy", CultureInfo.InvariantCulture);
                GPRata = data.AddMonths(1);
            }
            else if (dataInizioTrattenuta != null && dataInizioTrattenuta != String.Empty && DateTime.TryParseExact(dataInizioTrattenuta, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DataInizioTrattenuta))
                GPRata = DataInizioTrattenuta;

            request.PianoDiPagamento.DatiPiano = new DatiPianoGestioneConguaglio
            {
                IdentificativoChiamante = null,

                RataMensile = (decimal)(recordDatiFondoINPDAP[0].ImportoSingolaRata.HasValue ? recordDatiFondoINPDAP[0].ImportoSingolaRata : 0), //controll
                Pensionato = new BeneficiarioPensionatoGestioneConguaglio
                {
                    codiceFiscale = datiAnagraficiTitolare.CodiceFiscale
                },

                MatricolaDipendente = datiPensione.MatricolaUtenteAcquisizione,
                NumeroRateRecupero = (recordDatiFondoINPDAP[0].NumeroRate.HasValue ? recordDatiFondoINPDAP[0].NumeroRate : 0).ToString(),
                DecorrenzaPensione = DP.ToString("yyyyMM"),
                RataDispostoPagamento = GPRata != null ? GPRata.ToString("yyyyMM") : "",

            };

            return request;
        }

        public static InserisciConguaglio_Gruppo05_Request ValorizzaInserisciConguaglio05Request(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP)
        {
            string inizioTrattenuta;
            string errori;
            string rataMensile;
            decimal RataMensile = 0;
            string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                           datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
            if (string.IsNullOrEmpty(datiAnagraficiTitolare.CodiceFiscale))
            {
                datiAnagraficiTitolare.CodiceFiscale = string.Empty;
            }

            bool callGP6 = false;
            ElementoDatiTGP5[] listaDatiTGP5 = null;
            GestioneDatiPensioni.GetDatiTGP5ByChiavePensione(datiPensione.NDomus, chiavePensione, out listaDatiTGP5, out errori);
            if (listaDatiTGP5 != null && listaDatiTGP5.Count() > 0)
            {
                var datoTGP5 = listaDatiTGP5.Where(x => GestioneDatiPensioni.GetValueDatoGP(x.GP5HC01Z).Length == 6 && !GestioneDatiPensioni.GetValueDatoGP(x.GP5HC01Z).Substring(4, 2).Equals("13"))
                    .OrderByDescending(x => GestioneDatiPensioni.GetValueDatoGP(x.GP5HC01Z)).FirstOrDefault();
                if (datoTGP5 != null && Decimal.TryParse(GestioneDatiPensioni.GetValueDatoGP(datoTGP5.GP5KC10E), NumberStyles.Any, CultureInfo.InvariantCulture, out RataMensile))
                {
                    RataMensile = RataMensile > 0 ? Math.Round(RataMensile / 10, 2, MidpointRounding.AwayFromZero): 0; 
                }
                else callGP6 = true;
            }
            else callGP6 = true;

            if (callGP6)
            {
                ElementoDatiTGP6[] listaDatiTGP6 = null;
                GestioneDatiPensioni.GetDatiTGP6ByChiavePensione(datiPensione.NDomus, chiavePensione, out listaDatiTGP6, out errori);
                if (listaDatiTGP6 != null && listaDatiTGP6.Count() > 0)
                {
                    var datoTGP6 = listaDatiTGP6.Where(x => GestioneDatiPensioni.GetValueDatoGP(x.GP6KC01Z).Length == 6 && !GestioneDatiPensioni.GetValueDatoGP(x.GP6KC01Z).Substring(4, 2).Equals("13"))
                    .OrderByDescending(x => GestioneDatiPensioni.GetValueDatoGP(x.GP6KC01Z)).FirstOrDefault();
                    if (datoTGP6 != null && Decimal.TryParse(GestioneDatiPensioni.GetValueDatoGP(datoTGP6.GP6KC10E), NumberStyles.Any, CultureInfo.InvariantCulture, out RataMensile))
                    {
                        RataMensile = RataMensile > 0 ? Math.Round(RataMensile / 10, 2, MidpointRounding.AwayFromZero) : 0;
                    }
                }
            }

            GestioneDatiPensioni.GetDatiTGP8(datiPensione.NDomus, chiavePensione, out errori, out inizioTrattenuta, out rataMensile);
            //if (Decimal.TryParse(rataMensile, out RataMensile))
            //{
            //    RataMensile /= 10;
            //}


            DatiTGP1Response risposta = new DatiTGP1Response();
            GestioneDatiPensioni.GetDatiTGP1ByChiavePensione(datiPensione.NDomus.ToString(), chiavePensione, out risposta, out errori);
            string dInizioTrattenuta = "";
            DateTime data = new DateTime();
            DateTime DataInizioTrattenuta = new DateTime(0001, 01, 01);
            if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1AZ51Z != null && risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice != "9999" && risposta.ElementoDatiTGP1.GP1ALZ5 != null && risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice != "00")
            {
                string dataString = risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice + risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice;
                if (dataString != null && dataString != String.Empty && DateTime.TryParseExact(dataString, "MMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                {
                    dInizioTrattenuta = data.AddMonths(1).ToString("yyyyMM");
                }
            }
            else if (inizioTrattenuta != null && inizioTrattenuta != String.Empty && DateTime.TryParseExact(inizioTrattenuta, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DataInizioTrattenuta))
                dInizioTrattenuta = DataInizioTrattenuta.ToString("yyyyMM");


            InserisciConguaglio_Gruppo05_Request request = new InserisciConguaglio_Gruppo05_Request();
            request.ProceduraChiamante = "PN812";
            request.PianoDiPagamento = new PianoDiPagamentoInserisciConguaglio05();
            request.PianoDiPagamento.Pensione = new PensioneInserisciConguaglio() { categoria = datiPensione.GetCodCategoria().Substring(1), certificato = datiPensione.NCertificato.ToString(), sede = datiPensione.CodiceSedeDestinazione.ToString().Substring(0, 2), zona = datiPensione.CodiceSedeDestinazione.ToString().Substring(2, 2) };
            request.PianoDiPagamento.DatiPiani = new DatiPianoInserisciConguaglio05[1];
            request.PianoDiPagamento.DatiPiani[0] = new DatiPianoInserisciConguaglio05
            {
                IdentificativoChiamante = null,
                Prodotto = ProdottoInserisciConguaglio.DEBITO,
                CodiceClasseConguaglio = "E69",
                TipoDeducibilitaTassazione = "0",
                ImportoComplessivo = recordDatiFondoINPDAP[0].ImpEquoInd.HasValue ? recordDatiFondoINPDAP[0].ImpEquoInd.GetValueOrDefault() : 0,
                RataMensile = RataMensile,
                MatricolaDipendente = datiPensione.MatricolaUtenteAcquisizione,
                DataInizioTrattenuta = dInizioTrattenuta,
                Pensionato = new BeneficiarioPensionatoInserisciConguaglio()
                {
                    codiceFiscale = datiAnagraficiTitolare.CodiceFiscale,
                },
                Beneficiario = new BeneficiarioPensionatoInserisciConguaglio()
                {
                    codiceFiscale = (recordDatiFondoINPDAP[0].EnteEquoInd != null && recordDatiFondoINPDAP[0].EnteEquoInd != "") ? recordDatiFondoINPDAP[0].EnteEquoInd : "",
                },

            };
            return request;
        }

        public static GestioneConguaglio_Indennizzo_Request ValorizzaIndennitaSpeciale(GestionePensione.DatiPensione datiPensione, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> recordDatiFondoINPDAP)
        {
            string dataInizioTrattenuta;
            string rataMensile;
            string errori;
            
            string chiavePensione = datiPensione.GetCodCategoria().Substring(1) + (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') :
                                           datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + (datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "");
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
            if (string.IsNullOrEmpty(datiAnagraficiTitolare.CodiceFiscale))
            {
                datiAnagraficiTitolare.CodiceFiscale = string.Empty;
            }
            DateTime GPRata = new DateTime();
            DateTime data = new DateTime();
            DateTime DataInizioTrattenuta = new DateTime(0001, 01, 01);
            DatiTGP1Response risposta = new DatiTGP1Response();
            GestioneDatiPensioni.GetDatiTGP1ByChiavePensione(datiPensione.NDomus.ToString(), chiavePensione, out risposta, out errori);
            if (risposta != null && risposta.ElementoDatiTGP1 != null && risposta.ElementoDatiTGP1.GP1AZ51Z != null && risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice != "9999" && risposta.ElementoDatiTGP1.GP1ALZ5 != null && risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice != "00")
            {
                string dataString = risposta.ElementoDatiTGP1.GP1ALZ5.Valore.Codice + risposta.ElementoDatiTGP1.GP1AZ51Z.Valore.Codice;
                data = DateTime.ParseExact(dataString, "MMyyyy", CultureInfo.InvariantCulture);
                GPRata = data.AddMonths(1);
            }
            else
            {
                GestioneDatiPensioni.GetDatiTGP8(datiPensione.NDomus, chiavePensione, out errori, out dataInizioTrattenuta, out rataMensile);
                if (dataInizioTrattenuta != null && dataInizioTrattenuta != String.Empty && DateTime.TryParseExact(dataInizioTrattenuta, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DataInizioTrattenuta))
                    GPRata = DataInizioTrattenuta;
            }
            GestioneConguaglio_Indennizzo_Request request = new GestioneConguaglio_Indennizzo_Request();
            request.ProceduraChiamante = "PN812";
            request.PianoDiPagamento = new PianoDiPagamentoGestioneConguaglioIndennizzo();
            request.PianoDiPagamento.Pensione = new PensioneGestioneConguaglio () { categoria = datiPensione.GetCodCategoria().Substring(1), certificato = datiPensione.NCertificato.ToString(), sede = datiPensione.CodiceSedeDestinazione.ToString().Substring(0, 2), zona = datiPensione.CodiceSedeDestinazione.ToString().Substring(2, 2) };
            request.PianoDiPagamento.DatiPiano = new DatiPianoGestioneConguaglioIndennizzo

            {
                IdentificativoChiamante = null,
                Pensionato = new BeneficiarioPensionatoGestioneConguaglio ()
                {
                    codiceFiscale = datiAnagraficiTitolare.CodiceFiscale,
                },
                RataMensileIniziale = recordDatiFondoINPDAP[0].ImpRataIniz.HasValue? recordDatiFondoINPDAP[0].ImpRataIniz : 0,
                RataMensileFinale = recordDatiFondoINPDAP[0].ImpRataFin.HasValue? recordDatiFondoINPDAP[0].ImpRataFin : 0,
                RataMensileOrdinaria = recordDatiFondoINPDAP[0].ImpRataOrd.HasValue? recordDatiFondoINPDAP[0].ImpRataOrd :0,
                MatricolaDipendente = datiPensione.MatricolaUtenteAcquisizione,
                NumeroRate = (recordDatiFondoINPDAP[0].NumRate.HasValue? recordDatiFondoINPDAP[0].NumRate : 0).ToString(),
                DecorrenzaIndennizzo = recordDatiFondoINPDAP[0].DataInizioInd.HasValue? recordDatiFondoINPDAP[0].DataInizioInd.GetValueOrDefault().ToString("yyyyMM") : "",
                TipologiaIndennizzo = 1,
                CodiceIndennizzo = recordDatiFondoINPDAP[0].CodInd != null? recordDatiFondoINPDAP[0].CodInd.Trim() : "",
                ScadenzaIndennizzo = recordDatiFondoINPDAP[0].DataCessInd.HasValue? recordDatiFondoINPDAP[0].DataCessInd.GetValueOrDefault().ToString("yyyyMM") : "",
                ImportoIndennizzo = recordDatiFondoINPDAP[0].ImpInd.HasValue? recordDatiFondoINPDAP[0].ImpInd : 0,
                RataDispostoPagamento = GPRata != null ? GPRata.ToString("yyyyMM") : "",

            };
            return request;
        }


        public class AcquisizioneEventoRequest
        {
            public string tipoEvento { get; set; }
            public Parametriinput parametriInput { get; set; }

        }

        public class Importi
        {
            public string decorrenza { get; set; }
            public float pensioneConMiglioramentiContrattuali { get; set; }
            public float aliquotaNucleo { get; set; }
            public float pensioneInPagamento { get; set; }
            public float pensioneFinoAl { get; set; }
            public float pensioneDalAl { get; set; }
            public float pensioneOriginariaPerequata { get; set; }
            public string ratei { get; set; }
            public float quotaCarico { get; set; }
            public float precedenteQuotaCarico { get; set; }
        }

        public class Parametriinput
        {
            public string operatore { get; set; }
            public string codiceSede { get; set; }
            public string tipoLav { get; set; }
            public string titolare { get; set; }
            public float pal { get; set; }
            public string dataDec { get; set; }
            public string tipoPens { get; set; }
            public string certificatoPens { get; set; }
            public string tipoLiq { get; set; }
            public string idAtto { get; set; }
            public string dataAtto { get; set; }
            public string numeroDomus { get; set; }
            public string cfSedeApp { get; set; }
            public string prgSedeApp { get; set; }
            public string idCassa { get; set; }
            public float importoRec { get; set; }
            public Nucleo[] nucleo { get; set; }
            public string idEnteSostImp { get; set; }
            public string idSedeCompetenza { get; set; }
            public string nomeCognome { get; set; }
            public string dataNascita { get; set; }
            public string codFiscale { get; set; }
            public string numIscrizione { get; set; }
            public string cassaCess { get; set; }
            public string tipoPensione { get; set; }
            public string dataCessazioneServizio { get; set; }
            public string motivoCessazioneServizio { get; set; }
            public float importoPensione { get; set; }
            public float totaleQuoteCaricoAtt { get; set; }
            public float totaleQuoteCaricoPrec { get; set; }
            public float totaleQuote { get; set; }
            public string dataRiferimento { get; set; }
            public string dataNascitaTit { get; set; }
            public string etaAnni { get; set; }
            public float coefTab { get; set; }
            public float importoQuotaACarico { get; set; }
            public float valoreCapitaleRisult { get; set; }
            public float valoreCapitalePrec { get; set; }
            public float valoreCapitale { get; set; }
            public List<Importi> importi { get; set; }






        }

        public class Nucleo
        {
            public string cf { get; set; }
            public string legame { get; set; }
        }


        //public class AcquisizioneEventoResponse
        //{
        //    public Codiceesito codiceEsito { get; set; }
        //    public Descrizioneesito descrizioneEsito { get; set; }
        //}

        //public class Codiceesito
        //{
        //    public string title { get; set; }
        //    public string[] _enum { get; set; }
        //}

        //public class Descrizioneesito
        //{
        //    public string title { get; set; }
        //    public string type { get; set; }
        //}

        public class AcquisizioneEventoResponse
        {
            public string esito { get; set; }
            public string codiceEsito { get; set; }
            public string descrizioneEsito { get; set; }
        }

    }
}
