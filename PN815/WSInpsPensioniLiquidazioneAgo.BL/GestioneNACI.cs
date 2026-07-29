using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazioneAgo.ServiceReferences.NACI;
using INPS.DNA.Logging;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Reflection;
using INPS.Pensioni.LiquidazioneAgo.Data;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneNACI
    {
        #region public methods
        public static bool VerificaProcedura(long numDomanda, string matricola, short codiceSede, short centroOperativo, out bool isNuovaProcedura, out string errori)
        {
            errori = string.Empty;
            isNuovaProcedura = false;
            verificaProceduraEESSIResponse dati = null;

            VerificaProceduraEESSI(numDomanda, matricola, codiceSede, centroOperativo, out dati, out errori);
            if (!string.IsNullOrEmpty(errori))
                return false;

            if (dati != null && dati.ModuloVerificaProceduraEESSIRisposta != null && dati.ModuloVerificaProceduraEESSIRisposta.DatiRisposta != null)
                isNuovaProcedura = dati.ModuloVerificaProceduraEESSIRisposta.DatiRisposta.PresenteInProcedureEESSI.GetValueOrDefault();

            return true;
        }

        public static bool GetListaStatiIstituzione(long numDomanda, string matricola, short codiceSede, short centroOperativo, out List<GestioneContrib.PrestazioneEsteraCumulo> listaPrestazioniEstere, out string errori)
        {
            errori = string.Empty;
            listaPrestazioniEstere = null;
            getDatiNaciResponse dati = null;

            GetDatiNACI(numDomanda, matricola, codiceSede, centroOperativo, out dati, out errori);
            if (!string.IsNullOrEmpty(errori))
                return false;

            if (dati != null && dati.ModuloGetDatiNaciRisposta != null && dati.ModuloGetDatiNaciRisposta.DatiRisposta != null)
                NormalizzaDatiIstituzione(dati.ModuloGetDatiNaciRisposta.DatiRisposta, out listaPrestazioniEstere, out errori);

            return true;
        }

        private static void NormalizzaDatiIstituzione(TipoGetDatiNaciRisposta dati, out List<GestioneContrib.PrestazioneEsteraCumulo> listaPrestazioniEstere, out string errori)
        {
            listaPrestazioniEstere = null;
            errori = string.Empty;

            if (dati != null)
            {
                if (dati.Stati != null && dati.Stati.Count() > 0)
                {
                    listaPrestazioniEstere = new List<GestioneContrib.PrestazioneEsteraCumulo>();
                    foreach (TipoStato stato in dati.Stati)
                    {
                        string codStatoIstituzione = stato.CodiceStato + stato.CodiceIstituzione;
                        if (!string.IsNullOrEmpty(codStatoIstituzione))
                        {
                            aciistit descPrestazioneEstera = null;
                            DAPrestazioniEstere.GetPrestazioneEstera(codStatoIstituzione.PadLeft(6, '0'), out descPrestazioneEstera);
                            if (descPrestazioneEstera != null)
                            {
                                if (dati.DecisioneItaliana != null && !String.IsNullOrEmpty(dati.DecisioneItaliana.Codice) && !String.IsNullOrEmpty(dati.DecisioneItaliana.Codice.Trim()) && (dati.DecisioneItaliana.Codice[0].ToString().ToUpperInvariant() == "X" || dati.DecisioneItaliana.Codice[0].ToString().ToUpperInvariant() == "0"))
                                {
                                    errori = "Provvedimento italiano mancante o errato per stato " + (descPrestazioneEstera.CDSTAIST.Length == 6 ? descPrestazioneEstera.CDSTAIST.Substring(0, 2) : "");
                                    return;
                                }

                                if (listaPrestazioniEstere == null)
                                    listaPrestazioniEstere = new List<INPS.Pensioni.LiquidazioneAgo.GestioneContrib.PrestazioneEsteraCumulo>();
                                listaPrestazioniEstere.Add(new INPS.Pensioni.LiquidazioneAgo.GestioneContrib.PrestazioneEsteraCumulo(descPrestazioneEstera.CDSTAIST, descPrestazioneEstera.SIGLISTI,
                                    descPrestazioneEstera.CITTAIST, descPrestazioneEstera.NOMESTAT, descPrestazioneEstera.SIGLASTAT, !String.IsNullOrEmpty(stato.Matricola) ? stato.Matricola.Trim() : "", descPrestazioneEstera.CODICONV, false));
                            }
                        }
                    }
                }
            }
        }
        #endregion public methods

        #region private methods
        private static void VerificaProceduraEESSI(long numDomanda, string matricola, short codiceSede, short centroOperativo, out verificaProceduraEESSIResponse risposta, out string errori)
        {
            risposta = null;
            errori = string.Empty;
            bool erroreTecnico = false;
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            WSSERVIZINACIServiceClient proxy = new WSSERVIZINACIServiceClient();
            verificaProceduraEESSI richiesta = new verificaProceduraEESSI();
            richiesta.ModuloVerificaProceduraEESSIRichiesta = new TipoModuloVerificaProceduraEESSIRichiesta();
            richiesta.ModuloVerificaProceduraEESSIRichiesta.DatiRichiesta = new TipoVerificaProceduraEESSIRichiesta();
            richiesta.ModuloVerificaProceduraEESSIRichiesta.DatiRichiesta.NumeroDomus = numDomanda.ToString();
            richiesta.ModuloVerificaProceduraEESSIRichiesta.MetadatiServizio = new TipoMetadatiServizio();
            richiesta.ModuloVerificaProceduraEESSIRichiesta.MetadatiServizio.NomeServizio = TipoNomeServizio.VerificaProceduraEESSIService;
            richiesta.ModuloVerificaProceduraEESSIRichiesta.MetadatiServizio.Mittente = "LIQPENS";
            richiesta.ModuloVerificaProceduraEESSIRichiesta.MetadatiServizio.Timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds; // Unix Epoch Time

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvNaci, Utility.MetodoServizio.VerificaProceduraEESSI, Utility.SOAPLogDirection.IN, numDomanda.ToString(), guid);
                    risposta = proxy.verificaProceduraEESSI(richiesta);

                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvNaci, Utility.MetodoServizio.VerificaProceduraEESSI, Utility.SOAPLogDirection.OUT, numDomanda.ToString(), guid);

                    if (risposta == null || risposta.ModuloVerificaProceduraEESSIRisposta == null || risposta.ModuloVerificaProceduraEESSIRisposta.DatiRisposta == null ||
                        risposta.ModuloVerificaProceduraEESSIRisposta.DatiRisposta.Messaggio == null)
                    {
                        errori = "Errore nella chiamata al servizio NACI, method VerificaProceduraEESSI: area vuota";
                    }
                    else if (risposta.ModuloVerificaProceduraEESSIRisposta.DatiRisposta.Messaggio.Codice != 0)
                    {
                        errori = risposta.ModuloVerificaProceduraEESSIRisposta.DatiRisposta.Messaggio.Codice.ToString() + " - " +
                                 risposta.ModuloVerificaProceduraEESSIRisposta.DatiRisposta.Messaggio.Descrizione;
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio NACI, method VerificaProceduraEESSI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio NACI, method VerificaProceduraEESSI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio NACI, method VerificaProceduraEESSI | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio NACI, method VerificaProceduraEESSI: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string parametri = string.Format("Matricola: {0}; Codice sede: {1}; Centro operativo: {2}", matricola, codiceSede, centroOperativo);
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, parametri, stackTrace);
                        errori = "Errore tecnico durante il recupero degli stati esteri";
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void GetDatiNACI(long nDomus, string matricola, short codiceSede, short centroOperativo, out getDatiNaciResponse risposta, out string errori)
        {
            risposta = null;
            errori = string.Empty;
            bool erroreTecnico = false;
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            WSSERVIZINACIServiceClient proxy = new WSSERVIZINACIServiceClient();
            getDatiNaci richiesta = new getDatiNaci();
            richiesta.ModuloGetDatiNaciRichiesta = new TipoModuloGetDatiNaciRichiesta();
            richiesta.ModuloGetDatiNaciRichiesta.DatiRichiesta = new TipoGetDatiNaciRichiesta();
            richiesta.ModuloGetDatiNaciRichiesta.DatiRichiesta.NumeroDomus = nDomus.ToString();
            richiesta.ModuloGetDatiNaciRichiesta.MetadatiServizio = new TipoMetadatiServizio();
            richiesta.ModuloGetDatiNaciRichiesta.MetadatiServizio.NomeServizio = TipoNomeServizio.GetDatiNaciService;
            richiesta.ModuloGetDatiNaciRichiesta.MetadatiServizio.Mittente = "LIQPENS";
            richiesta.ModuloGetDatiNaciRichiesta.MetadatiServizio.Timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds; // Unix Epoch Time

            using (new MethodExecutionTracer())
            {
                try
                {
                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvNaci, Utility.MetodoServizio.GetDatiNaci, Utility.SOAPLogDirection.IN, nDomus.ToString(), guid);
                    risposta = proxy.getDatiNaci(richiesta);

                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvNaci, Utility.MetodoServizio.GetDatiNaci, Utility.SOAPLogDirection.OUT, nDomus.ToString(), guid);

                    if (risposta == null || risposta.ModuloGetDatiNaciRisposta == null || risposta.ModuloGetDatiNaciRisposta.DatiRisposta == null ||
                        risposta.ModuloGetDatiNaciRisposta.DatiRisposta.Messaggio == null)
                    {
                        errori = "Errore nella chiamata al servizio NACI, method getDatiNaci: area vuota";
                    }
                    else if (risposta.ModuloGetDatiNaciRisposta.DatiRisposta.Messaggio.Codice != 0)
                    {
                        errori = risposta.ModuloGetDatiNaciRisposta.DatiRisposta.Messaggio.Codice.ToString() + " " +
                                 risposta.ModuloGetDatiNaciRisposta.DatiRisposta.Messaggio.Descrizione;
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio NACI, method getDatiNaci | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio NACI, method getDatiNaci | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio NACI, method getDatiNaci | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio NACI, method getDatiNaci: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string parametri = string.Format("Matricola: {0}; Codice sede: {1}; Centro operativo: {2}", matricola, codiceSede, centroOperativo);
                        GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, parametri, stackTrace);
                        errori = "Errore tecnico durante il recupero degli stati esteri";
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        #endregion private methods
    }
}
