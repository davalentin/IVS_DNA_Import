using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.TotalIvs;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneTotalIvs
    {
        #region public methods

        public static bool AggiornaTotalIVS(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;

            if (!ControllaStatoPensionePerAggiornamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento TOTAL";
                return false;
            }

            if (!AggiornaCumulo(datiPensione, out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal);
                return false;
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool AggiornaCumulo(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            try
            {
                string categoriaNum = datiPensione.GetCodCategoria();

                if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsCumuloAutomatica.GetValueOrDefault() &&
                    GestioneCtrlControlliApplicativi.CheckControlloApplicativoAttivoByData(GestioneCtrlControlliApplicativi.EnumNomeControllo.AGO.BLOCCO_RIC_CUMULO_AUTOMATICHE, Utility.DataSistemaAgo))
                {
                    if (!AggiornaRicostituzioneCUMUL(datiPensione.NDomus.ToString(), 1, datiPensione.DataElaborazione, out messaggioVideo))
                        return false;
                }
                else
                {
                    if (!AggiornaPensioneCUMUL(datiPensione.NDomus.ToString(), categoriaNum, datiPensione.CodiceSede.ToString().PadLeft(4, '0'), datiPensione.NCertificato.GetValueOrDefault().ToString().PadLeft(8, '0'),
                        datiPensione.DataElaborazione, out messaggioVideo))
                        return false;
                }
            }
            catch (Exception ex)
            {
                messaggioVideo = ex.Message;
                return false;
            }

            return true;
        }

        public static bool GetDatiCumulIVS(long nDomus, out clsDatiCumulo risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                EstrazioneDatiCumulIVS(nDomus.ToString(), out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.objErrori != null && risposta.objErrori.CodiceErrore != "0")
                    risposta = null;

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico le recupero dei dati del cumulo";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        public static bool AggiornaPensioneCUMUL(string nDomus, string categoriaNumerica, string codiceSede, string certificato, DateTime? dataLiquidazione, out string errori)
        {
            errori = string.Empty;
            clsDati risposta = null;

            try
            {
                AggiornaKeyPensioneCUMUL(nDomus, categoriaNumerica, codiceSede, certificato, dataLiquidazione, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.objErrori != null && risposta.objErrori.CodiceErrore != "0")
                {
                    errori = "Errore durante l'aggiornamento Total: " + risposta.objErrori.DescrErrore;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico durante l'aggiornamento Total";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Categoria numerica: {0}; Codice sede: {1}; Certificato: {2}; Data liquidazione: {3:dd/MM/yyyy}",
                           categoriaNumerica, codiceSede, certificato, dataLiquidazione);
                long numeroDomanda = 0;
                long.TryParse(nDomus, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        public static bool GetDatiCumulRicostituzioneIVS(long nDomus, out clsDatiCumulo risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                EstrazioneDatiCumulRicostituzioneIVS(nDomus.ToString(), out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.objErrori != null && risposta.objErrori.CodiceErrore != "0")
                    risposta = null;

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico le recupero dei dati del cumulo";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        public static bool AggiornaRicostituzioneCUMUL(string nDomus, int flagIVS, DateTime? dataLiquidazione, out string errori)
        {
            errori = string.Empty;
            clsDati risposta = null;

            try
            {
                AggiornaKeyRicostituzioneCUMUL(nDomus, flagIVS, dataLiquidazione, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.objErrori != null && risposta.objErrori.CodiceErrore != "0")
                {
                    errori = "Errore durante l'aggiornamento Total: " + risposta.objErrori.DescrErrore;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico durante l'aggiornamento Total";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("FlagIVS: {0}; Data liquidazione: {1:dd/MM/yyyy}", flagIVS, dataLiquidazione);
                long numeroDomanda = 0;
                long.TryParse(nDomus, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        #region Domande TOT
        public static bool AggiornaTotIVS(GestionePensione.DatiPensione datiPensione, out string statoPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;

            if (!ControllaStatoPensionePerAggiornamentoTot(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento TOTAL (Totalizzazioni)";
                return false;
            }

            if (!AggiornaTot(datiPensione, out messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTot);
                return false;
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool AggiornaTot(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            try
            {
                string categoriaNum = datiPensione.GetCodCategoria();

                if (!AggiornaPensioneTOTAL(datiPensione.NDomus.ToString(), categoriaNum, datiPensione.CodiceSede.ToString().PadLeft(4, '0'), datiPensione.NCertificato.GetValueOrDefault().ToString().PadLeft(8, '0'),
                    datiPensione.DataElaborazione, out messaggioVideo))
                    return false;
            }
            catch (Exception ex)
            {
                messaggioVideo = ex.Message;
                return false;
            }

            return true;
        }

        public static bool GetDatiTotalIVS(long nDomus, out clsDati risposta, out string errori)
        {
            errori = string.Empty;
            risposta = null;

            try
            {
                EstrazioneDatiTotalIVS(nDomus.ToString(), out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.objErrori != null && risposta.objErrori.CodiceErrore != "0")
                    risposta = null;

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico le recupero dei dati del cumulo";
                string messaggio = Utility.GetMessageFromException(ex);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, ex.StackTrace);
                return false;
            }
        }

        public static bool AggiornaPensioneTOTAL(string nDomus, string categoriaNumerica, string codiceSede, string certificato, DateTime? dataLiquidazione, out string errori)
        {
            errori = string.Empty;
            clsDati risposta = null;

            try
            {
                AggiornaKeyPensioneTOTAL(nDomus, categoriaNumerica, codiceSede, certificato, dataLiquidazione, out risposta, out errori);
                if (!String.IsNullOrEmpty(errori))
                    return false;

                if (risposta != null && risposta.objErrori != null && risposta.objErrori.CodiceErrore != "0")
                {
                    errori = "Errore durante l'aggiornamento Total: " + risposta.objErrori.DescrErrore;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico durante l'aggiornamento Total";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Categoria numerica: {0}; Codice sede: {1}; Certificato: {2}; Data liquidazione: {3:dd/MM/yyyy}",
                           categoriaNumerica, codiceSede, certificato, dataLiquidazione);
                long numeroDomanda = 0;
                long.TryParse(nDomus, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }
        #endregion
        #endregion public methods

        #region private methods

        private static bool EstrazioneDatiCumulIVS(string numDomanda, out clsDatiCumulo datiCumuloResponse, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiCumuloResponse = null;

            WSTotalIvsSoapClient proxy = new WSTotalIvsSoapClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    datiCumuloResponse = proxy.EstrazioneDatiCumulIVS(numDomanda);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WSTotalIVS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore nel recupero dei dati del cumulo";
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiCumuloResponse, Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.EstrazioneDatiCumulIVS, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool AggiornaKeyPensioneCUMUL(string numDomanda, string categoriaNumerica, string codiceSede, string certificato, DateTime? dataLiquidazione, out clsDati datiResponse, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiResponse = null;

            WSTotalIvsSoapClient proxy = new WSTotalIvsSoapClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    string sede = !string.IsNullOrEmpty(codiceSede) ? codiceSede.Substring(0, 2) : string.Empty;
                    string zona = !string.IsNullOrEmpty(codiceSede) ? codiceSede.Substring(2, 2) : string.Empty;
                    categoriaNumerica = categoriaNumerica.PadLeft(4, '0').Substring(1, 3);

                    GestioneLogSoap.SalvaLogSoap(new AreaInputAggiornaKeyPensioneCUMUL(numDomanda, categoriaNumerica, sede, zona, certificato, dataLiquidazione.GetValueOrDefault()), Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.AggiornaKeyPensioneCUMUL, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    datiResponse = proxy.AggiornaKeyPensioneCUMUL(numDomanda, categoriaNumerica, sede, zona, certificato, dataLiquidazione.GetValueOrDefault());
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WSTotalIVS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante l'aggiornamento Total";
                        string parametri = string.Format("Categoria numerica: {0}; Codice sede: {1}; Certificato: {2}; Data liquidazione: {3:dd/MM/yyyy}",
                            categoriaNumerica, codiceSede, certificato, dataLiquidazione);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiResponse, Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.AggiornaKeyPensioneCUMUL, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool ControllaStatoPensionePerAggiornamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoTotal)
                return true;
            else
                return false;
        }

        private static bool EstrazioneDatiCumulRicostituzioneIVS(string numDomanda, out clsDatiCumulo datiCumuloResponse, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiCumuloResponse = null;

            WSTotalIvsSoapClient proxy = new WSTotalIvsSoapClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    datiCumuloResponse = proxy.EstrazioneDatiCumulRicostituzioneIVS(numDomanda);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WSTotalIVS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore nel recupero dei dati del cumulo";
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiCumuloResponse, Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.EstrazioneDatiCumulRicostituzioneIVS, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool AggiornaKeyRicostituzioneCUMUL(string numDomanda, int flagIVS, DateTime? dataLiquidazione, out clsDati datiResponse, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiResponse = null;

            WSTotalIvsSoapClient proxy = new WSTotalIvsSoapClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(new AreaInputAggiornaKeyRicostituzioneCUMUL(numDomanda, flagIVS, dataLiquidazione.GetValueOrDefault()), Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.AggiornaKeyRicostituzioneCUMUL, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    datiResponse = proxy.AggiornaKeyRicostituzioneCUMUL(numDomanda, flagIVS, dataLiquidazione.GetValueOrDefault());
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WSTotalIVS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante l'aggiornamento Total";
                        string parametri = string.Format("FlagIVS: {0}; Data liquidazione: {1:dd/MM/yyyy}", flagIVS, dataLiquidazione);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiResponse, Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.AggiornaKeyRicostituzioneCUMUL, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        #region domande TOT
        private static bool ControllaStatoPensionePerAggiornamentoTot(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoTot)
                return true;
            else
                return false;
        }

        private static bool EstrazioneDatiTotalIVS(string numDomanda, out clsDati datiTotResponse, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiTotResponse = null;

            WSTotalIvsSoapClient proxy = new WSTotalIvsSoapClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    datiTotResponse = proxy.EstrazioneDatiTotalIVS(numDomanda);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WSTotalIVS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore nel recupero dei dati del cumulo";
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiTotResponse, Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.EstrazioneDatiTotalIVS, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool AggiornaKeyPensioneTOTAL(string numDomanda, string categoriaNumerica, string codiceSede, string certificato, DateTime? dataLiquidazione, out clsDati datiResponse, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            datiResponse = null;

            WSTotalIvsSoapClient proxy = new WSTotalIvsSoapClient();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    string sede = !string.IsNullOrEmpty(codiceSede) ? codiceSede.Substring(0, 2) : string.Empty;
                    string zona = !string.IsNullOrEmpty(codiceSede) ? codiceSede.Substring(2, 2) : string.Empty;
                    categoriaNumerica = categoriaNumerica.PadLeft(4, '0').Substring(1, 3);
                    string dataLiquidazioneGGMMAAA = string.Empty;
                    if (dataLiquidazione.HasValue)
                        dataLiquidazioneGGMMAAA = dataLiquidazione.Value.Day.ToString().PadLeft(2, '0') + dataLiquidazione.Value.Month.ToString().PadLeft(2, '0') + dataLiquidazione.Value.Year.ToString().PadLeft(4, '0');

                    GestioneLogSoap.SalvaLogSoap(new AreaInputAggiornaKeyPensioneCUMUL(numDomanda, categoriaNumerica, sede, zona, certificato, dataLiquidazione.GetValueOrDefault()), Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.AggiornaKeyPensioneTOTAL, Utility.SOAPLogDirection.IN, numDomanda, guid);

                    datiResponse = proxy.AggiornaKeyPensioneTOTAL(numDomanda, categoriaNumerica, sede, zona, certificato, dataLiquidazioneGGMMAAA);
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WSTotalIVS | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WSTotalIVS: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante l'aggiornamento Total";
                        string parametri = string.Format("Categoria numerica: {0}; Codice sede: {1}; Certificato: {2}; Data liquidazione: {3:dd/MM/yyyy}",
                            categoriaNumerica, codiceSede, certificato, dataLiquidazione);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiResponse, Utility.Servizio.SrvWSTotalIvs, Utility.MetodoServizio.AggiornaKeyPensioneTOTAL, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        #endregion
        #endregion private methods

        #region nested class

        [Serializable]
        private class AreaInputAggiornaKeyPensioneCUMUL
        {
            public AreaInputAggiornaKeyPensioneCUMUL(string numeroDomanda, string siglaCategoria, string sede, string zona, string certificato, DateTime dataLiquidazione)
            {
                this.NumeroDomanda = numeroDomanda;
                this.SiglaCategoria = siglaCategoria;
                this.Sede = sede;
                this.Zona = zona;
                this.Certificato = certificato;
                this.DataLiquidazione = dataLiquidazione;
            }

            public string NumeroDomanda { get; set; }
            public string SiglaCategoria { get; set; }
            public string Sede { get; set; }
            public string Zona { get; set; }
            public string Certificato { get; set; }
            public DateTime DataLiquidazione { get; set; }
        }

        [Serializable]
        private class AreaInputAggiornaKeyRicostituzioneCUMUL
        {
            public AreaInputAggiornaKeyRicostituzioneCUMUL(string numeroDomanda, int flagIVS, DateTime dataLiquidazione)
            {
                this.NumeroDomanda = numeroDomanda;
                this.FlagIVS = flagIVS;
                this.DataLiquidazione = dataLiquidazione;
            }
            public string NumeroDomanda { get; set; }
            public int FlagIVS { get; set; }
            public DateTime DataLiquidazione { get; set; }
        }
        #endregion nested class
    }
}
