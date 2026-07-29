using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using INPS.Pensioni.Liquidazione.ServiceReferences.ARCA;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneARCA
    {
        #region public members
        #endregion public members

        #region internal members
        internal static bool GetAreaArcaByCodiceFiscale(RichiestaARCA richiestaArca, string numDomanda, out DataTable anagrafica, out DataTable pensioniRiferimento, out string errori)
        {
            bool erroreTecnico = false;
            anagrafica = null;
            pensioniRiferimento = null;
            errori = "";
            DataSet datiAnagrafici = null;

            DatiRisposta risposta = new DatiRisposta();
            RichiestaPerCodiceFiscale richiesta = new RichiestaPerCodiceFiscale();
            ArcaIntraWSClient proxy = new ArcaIntraWSClient();
            tProfilo Profilo = new tProfilo();
            tCodiceFiscale CodiceFiscale = new tCodiceFiscale();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    Profilo.Applicazione = richiestaArca.Applicazione;
                    Profilo.CodiceFiscaleRichiedente = richiestaArca.CodiceFiscaleRichiedente != null ? richiestaArca.CodiceFiscaleRichiedente.ToUpperInvariant() : null;
                    //Profilo.IpClient = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Last().ToString();
                    Profilo.Matricola = richiestaArca.Matricola;
                    Profilo.Password = "";
                    Profilo.PIN = "";
                    Profilo.Provenienza = richiestaArca.Provenienza;
                    Profilo.Ruolo = richiestaArca.Ruolo;
                    richiesta.Profilo = Profilo;

                    int anno = 0;
                    if (!string.IsNullOrEmpty(richiestaArca.CodiceFiscale))
                        int.TryParse(richiestaArca.CodiceFiscale.Substring(6, 2), out anno);

                    CodiceFiscale.Anno = anno;
                    CodiceFiscale.Cognome = !string.IsNullOrEmpty(richiestaArca.CodiceFiscale) ? richiestaArca.CodiceFiscale.Substring(0, 3).ToUpperInvariant() : string.Empty;
                    CodiceFiscale.Comune = !string.IsNullOrEmpty(richiestaArca.CodiceFiscale) ? richiestaArca.CodiceFiscale.Substring(11, 4).ToUpperInvariant() : string.Empty;
                    CodiceFiscale.Giorno = !string.IsNullOrEmpty(richiestaArca.CodiceFiscale) ? richiestaArca.CodiceFiscale.Substring(9, 2).ToUpperInvariant() : string.Empty;
                    CodiceFiscale.Mese = !string.IsNullOrEmpty(richiestaArca.CodiceFiscale) ? richiestaArca.CodiceFiscale.Substring(8, 1).ToUpperInvariant() : string.Empty;
                    CodiceFiscale.Nome = !string.IsNullOrEmpty(richiestaArca.CodiceFiscale) ? richiestaArca.CodiceFiscale.Substring(3, 3).ToUpperInvariant() : string.Empty;
                    CodiceFiscale.CodControllo = !string.IsNullOrEmpty(richiestaArca.CodiceFiscale) ? richiestaArca.CodiceFiscale.Substring(15, 1).ToUpperInvariant() : string.Empty;
                    CodiceFiscale.AnnoSpecified = true;
                    richiesta.CodiceFiscale = CodiceFiscale;

                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvARCA, Utility.MetodoServizio.RicercaPerCodiceFiscale, Utility.SOAPLogDirection.IN, numDomanda, guid, richiestaArca.CodiceFiscale);
                    risposta = proxy.ricercaPerCodiceFiscale(richiesta);
                    if (risposta.Esito.ReturnCode == "WS-OK" && risposta.Dettaglio != null && risposta.Dettaglio.AllProfile != null &&
                        (risposta.Dettaglio.AllProfile.DatiIndirizzo != null || risposta.Dettaglio.AllProfile.DatiIndirizzoEstero != null) && risposta.Dettaglio.AllProfile.DatiPersonali != null)
                    {
                        datiAnagrafici = GetDatiArca(risposta.Dettaglio);
                        if (datiAnagrafici != null && datiAnagrafici.Tables.Count > 0)
                        {
                            if (datiAnagrafici.Tables.Contains("ANAGRAFICA") &&
                                datiAnagrafici.Tables["ANAGRAFICA"] != null && datiAnagrafici.Tables["ANAGRAFICA"].Rows.Count > 0)
                                anagrafica = datiAnagrafici.Tables["ANAGRAFICA"];
                            if (datiAnagrafici.Tables.Contains("PENSIONERIFERIMENTO") &&
                                datiAnagrafici.Tables["PENSIONERIFERIMENTO"] != null && datiAnagrafici.Tables["PENSIONERIFERIMENTO"].Rows.Count > 0)
                                pensioniRiferimento = datiAnagrafici.Tables["PENSIONERIFERIMENTO"];
                        }
                    }
                    else if (risposta.Esito.ReturnCode != "WS-OK")
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + risposta.Esito.Descrizione;
                        return true;
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio ARCA: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                        string parametri = string.Format("Codice Fiscale: {0}", richiestaArca.CodiceFiscaleRichiedente);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvARCA, Utility.MetodoServizio.RicercaPerCodiceFiscale, Utility.SOAPLogDirection.OUT, numDomanda, guid, richiestaArca.CodiceFiscale);
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        internal static bool GetAreaArcaByCodiceSoggetto(RichiestaARCA richiestaArca, string numDomanda, out DataTable anagrafica, out DataTable pensioniRiferimento, out string errori)
        {
            bool erroreTecnico = false;
            anagrafica = null;
            pensioniRiferimento = null;
            errori = "";
            DataSet datiAnagrafici = null;

            DatiRisposta risposta = new DatiRisposta();
            RichiestaPerCodiceIndividuale richiesta = new RichiestaPerCodiceIndividuale();
            ArcaIntraWSClient proxy = new ArcaIntraWSClient();
            tProfilo Profilo = new tProfilo();
            tCodiceIndividualeIn codiceIndividuale = new tCodiceIndividualeIn();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    Profilo.Applicazione = richiestaArca.Applicazione;
                    Profilo.CodiceFiscaleRichiedente = richiestaArca.CodiceFiscaleRichiedente != null ? richiestaArca.CodiceFiscaleRichiedente.ToUpperInvariant() : null;
                    //Profilo.IpClient = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Last().ToString();
                    Profilo.Matricola = richiestaArca.Matricola;
                    Profilo.Password = "";
                    Profilo.PIN = "";
                    Profilo.Provenienza = richiestaArca.Provenienza;
                    Profilo.Ruolo = richiestaArca.Ruolo;
                    richiesta.Profilo = Profilo;

                    codiceIndividuale.CodiceIndividuale = richiestaArca.CSog.GetValueOrDefault().ToString();
                    codiceIndividuale.FonteArchivio = new tFonteArchivio();
                    codiceIndividuale.FonteArchivio.Archivio = "100";
                    codiceIndividuale.FonteArchivio.Progetto = "A";
                    richiesta.ChiaveIndividuale = codiceIndividuale;

                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvARCA, Utility.MetodoServizio.RicercaPerCodiceIndividuale, Utility.SOAPLogDirection.IN, numDomanda, guid, richiestaArca.CSog.ToString());
                    risposta = proxy.ricercaPerCodiceIndividuale(richiesta);
                    if (risposta.Esito.ReturnCode == "WS-OK" && risposta.Dettaglio != null && risposta.Dettaglio.AllProfile != null &&
                        (risposta.Dettaglio.AllProfile.DatiIndirizzo != null || risposta.Dettaglio.AllProfile.DatiIndirizzoEstero != null) && risposta.Dettaglio.AllProfile.DatiPersonali != null)
                    {
                        datiAnagrafici = GetDatiArca(risposta.Dettaglio);
                        if (datiAnagrafici != null && datiAnagrafici.Tables.Count > 0)
                        {
                            if (datiAnagrafici.Tables.Contains("ANAGRAFICA") &&
                                datiAnagrafici.Tables["ANAGRAFICA"] != null && datiAnagrafici.Tables["ANAGRAFICA"].Rows.Count > 0)
                                anagrafica = datiAnagrafici.Tables["ANAGRAFICA"];
                            if (datiAnagrafici.Tables.Contains("PENSIONERIFERIMENTO") &&
                                datiAnagrafici.Tables["PENSIONERIFERIMENTO"] != null && datiAnagrafici.Tables["PENSIONERIFERIMENTO"].Rows.Count > 0)
                                pensioniRiferimento = datiAnagrafici.Tables["PENSIONERIFERIMENTO"];
                        }
                    }
                    else if (risposta.Esito.ReturnCode != "WS-OK")
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + risposta.Esito.Descrizione;
                        return true;
                    }
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio ARCA: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                        string parametri = string.Format("CSog: {0}", richiestaArca != null ? richiestaArca.CSog.GetValueOrDefault().ToString() : null);
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvARCA, Utility.MetodoServizio.RicercaPerCodiceIndividuale, Utility.SOAPLogDirection.OUT, numDomanda, guid, richiestaArca.CSog.ToString());
                    Utility.CloseClient(proxy);
                }

            }
            return true;
        }

        internal static bool GetAreaArcaByDatiPersonaliParziali(RichiestaARCA richiestaArca, string numDomanda, out DataTable anagrafica, out DataTable pensioniRiferimento, out DataTable sinonimi,
            out string errori)
        {
            bool erroreTecnico = false;
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                anagrafica = null;
                pensioniRiferimento = null;
                sinonimi = null;
                errori = "";
                DataSet datiAnagrafici = null;

                RichiestaPerDatiAnagraficiParziali richiesta = new RichiestaPerDatiAnagraficiParziali();
                ArcaIntraWSClient proxy = new ArcaIntraWSClient();
                tProfilo profilo = new tProfilo();
                tDatiPersonaliParziali2 dati = new tDatiPersonaliParziali2();
                DatiRisposta risposta = new DatiRisposta();
                Guid guid = Guid.NewGuid();
                try
                {
                    profilo.Applicazione = richiestaArca.Applicazione;
                    profilo.CodiceFiscaleRichiedente = richiestaArca.CodiceFiscaleRichiedente != null ? richiestaArca.CodiceFiscaleRichiedente.ToUpperInvariant() : null;
                    //profilo.IpClient = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Last().ToString();
                    profilo.Matricola = richiestaArca.Matricola;
                    profilo.Password = "";
                    profilo.PIN = "";
                    profilo.Provenienza = richiestaArca.Provenienza;
                    profilo.Ruolo = richiestaArca.Ruolo;
                    richiesta.Profilo = profilo;
                    dati.Cognome = richiestaArca.Cognome != null ? richiestaArca.Cognome.ToUpperInvariant() : null;
                    dati.Nome = richiestaArca.Nome != null ? richiestaArca.Nome.ToUpperInvariant() : null;
                    dati.Sesso = richiestaArca.Sesso != null ? richiestaArca.Sesso.ToUpperInvariant() : null;
                    if (richiestaArca.DataNascita != null && richiestaArca.DataNascita.HasValue)
                    {
                        dati.DataNascita = new tData2();
                        dati.DataNascita.Anno = richiestaArca.DataNascita.Value.Year.ToString().PadLeft(4, '0');
                        dati.DataNascita.Mese = richiestaArca.DataNascita.Value.Month.ToString().PadLeft(2, '0');
                        dati.DataNascita.Giorno = richiestaArca.DataNascita.Value.Day.ToString().PadLeft(2, '0');
                    }

                    richiesta.DatiPersonali = dati;
                    GestioneLogSoap.SalvaLogSoap(richiesta, Utility.Servizio.SrvARCA, Utility.MetodoServizio.RicercaPerDatiPersonaliParziali, Utility.SOAPLogDirection.IN, numDomanda, guid, string.Format("{0}_{1}", richiestaArca.Cognome, richiestaArca.Nome));
                    risposta = proxy.ricercaPerDatiPersonaliParziali(richiesta);
                    if (risposta.Esito.ReturnCode != "WS-OK")
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + risposta.Esito.Descrizione;
                        return true;
                    }
                    if (risposta.Dettaglio == null)
                    {
                        if (risposta.Esito.TotaleSinonimiRestituiti > 1)
                        {
                            sinonimi = GetSinonimi(risposta.Sinonimi);
                            if (sinonimi == null || sinonimi.Rows.Count == 0)
                            {
                                errori = "E' presente un'incongruenza sui sinonimi trovati.";
                                return false;
                            }
                            return true;
                        }
                        else
                            return true;
                    }

                    datiAnagrafici = GetDatiArca(risposta.Dettaglio);
                    if (datiAnagrafici != null && datiAnagrafici.Tables.Count > 0)
                    {
                        if (datiAnagrafici.Tables.Contains("ANAGRAFICA") &&
                            datiAnagrafici.Tables["ANAGRAFICA"] != null && datiAnagrafici.Tables["ANAGRAFICA"].Rows.Count > 0)
                            anagrafica = datiAnagrafici.Tables["ANAGRAFICA"];
                        if (datiAnagrafici.Tables.Contains("PENSIONERIFERIMENTO") &&
                            datiAnagrafici.Tables["PENSIONERIFERIMENTO"] != null && datiAnagrafici.Tables["PENSIONERIFERIMENTO"].Rows.Count > 0)
                            pensioniRiferimento = datiAnagrafici.Tables["PENSIONERIFERIMENTO"];
                    }
                    return true;
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio ARCA | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore durante il recupero dei dati anagrafici da ARCA: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                        string parametri = string.Format("Cognome: {0}; Nome: {1}; Data Nascita: {2:dd/MM/yyyy}; Sesso: {3}",
                            richiestaArca != null ? richiestaArca.Cognome : null,
                            richiestaArca != null ? richiestaArca.Nome : null,
                            richiestaArca != null ? richiestaArca.DataNascita : null,
                            richiestaArca != null ? richiestaArca.Sesso : null
                            );
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(risposta, Utility.Servizio.SrvARCA, Utility.MetodoServizio.RicercaPerDatiPersonaliParziali, Utility.SOAPLogDirection.OUT, numDomanda, guid, string.Format("{0}_{1}", richiestaArca.Cognome, richiestaArca.Nome));
                    Utility.CloseClient(proxy);
                }
            }
        }

        internal static bool GetAnagraficaArcaByCodiceFiscale(RichiestaARCA richiestaArca, string numDomanda, out Entity.Anagrafica anagrafica, out string errori)
        {
            anagrafica = null;
            errori = "";
            try
            {
                DataTable anag = null;
                DataTable pensioniRiferimento = null;
                if (!GetAreaArcaByCodiceFiscale(richiestaArca, numDomanda, out anag, out pensioniRiferimento, out errori))
                    return false;
                if (!string.IsNullOrEmpty(errori))
                    return true;
                if (anag != null && anag.Rows.Count > 0)
                    NormalizzaAnagraficaArcaToDB(anag.Rows[0], out anagrafica);

                if (anagrafica == null)
                {
                    errori = "Errore nel recupero dell'anagrafica: Soggetto con codice fiscale " + (richiestaArca.CodiceFiscale != null ? richiestaArca.CodiceFiscale.ToUpperInvariant() : string.Empty) + 
                        " non presente sugli archivi";
                    return true;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + Ex.Message;
                return false;
            }
            catch (Exception Ex)
            {
                string messaggio = string.Format("Errore nel recupero dell'anagrafica da ARCA: {0}", Utility.GetMessageFromException(Ex));
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                string parametri = string.Format("Codice Fiscale: {0}", richiestaArca != null ? richiestaArca.CodiceFiscale : null);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        internal static bool GetAnagraficaArcaByCodiceSoggetto(RichiestaARCA richiestaArca, string numDomanda, out Entity.Anagrafica anagrafica, out string errori)
        {
            anagrafica = null;
            errori = "";
            try
            {
                DataTable anag = null;
                DataTable pensioniRiferimento = null;
                if (!GetAreaArcaByCodiceSoggetto(richiestaArca, numDomanda, out anag, out pensioniRiferimento, out errori))
                    return false;
                if (!string.IsNullOrEmpty(errori))
                    return true;
                if (anag != null && anag.Rows.Count > 0)
                    NormalizzaAnagraficaArcaToDB(anag.Rows[0], out anagrafica);

                if (anagrafica == null)
                {
                    errori = "Errore nel recupero dell'anagrafica: Soggetto con codice fiscale " + (richiestaArca.CodiceFiscale != null ? richiestaArca.CodiceFiscale.ToUpperInvariant() : string.Empty) + " non presente sugli archivi";
                    return true;
                }
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + Ex.Message;
                return false;
            }
            catch (Exception Ex)
            {
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                string messaggio = string.Format("Errore nel recupero dell'anagrafica da ARCA: {0}", Utility.GetMessageFromException(Ex));
                string parametri = string.Format("Codice Soggetto: {0}", richiestaArca != null ? richiestaArca.CSog : null);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        internal static bool GetAreaArcaByCodiceFiscale(RichiestaARCA richiestaArca, string numDomanda, out Entity.Anagrafica anagrafica, out List<Entity.Pensione> elencoPensioni, out string errori)
        {
            anagrafica = null;
            elencoPensioni = null;
            errori = string.Empty;
            try
            {
                DataTable anag = null;
                DataTable pensioniRiferimento = null;
                if (!GetAreaArcaByCodiceFiscale(richiestaArca, numDomanda, out anag, out pensioniRiferimento, out errori))
                    return false;
                if (!string.IsNullOrEmpty(errori))
                    return true;
                if (anag != null && anag.Rows.Count > 0)
                    NormalizzaAnagraficaArcaToDB(anag.Rows[0], out anagrafica);

                if (anagrafica == null)
                {
                    errori = "Errore nel recupero dell'anagrafica: Soggetto con codice fiscale " + (richiestaArca.CodiceFiscale != null ? richiestaArca.CodiceFiscale.ToUpperInvariant() : string.Empty) + " non presente sugli archivi";
                    return true;
                }

                if (pensioniRiferimento != null && pensioniRiferimento.Rows.Count > 0)
                    if (!GestioneAreaRiepilogo.ValorizzaPensioni(pensioniRiferimento, out elencoPensioni, out errori))
                        return false;

            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + Ex.Message;
                return false;
            }
            catch (Exception Ex)
            {
                string messaggio = string.Format("Errore nel recupero dell'anagrafica da ARCA: {0}", Utility.GetMessageFromException(Ex));
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                string parametri = string.Format("Codice Fiscale: {0}", richiestaArca != null ? richiestaArca.CodiceFiscale : null);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        internal static bool GetAreaArcaByCodiceSoggetto(RichiestaARCA richiestaArca, string numDomanda, out Entity.Anagrafica anagrafica, out List<Entity.Pensione> elencoPensioni, out string errori)
        {
            anagrafica = null;
            elencoPensioni = null;
            errori = string.Empty;
            try
            {
                DataTable anag = null;
                DataTable pensioniRiferimento = null;
                if (!GetAreaArcaByCodiceSoggetto(richiestaArca, numDomanda, out anag, out pensioniRiferimento, out errori))
                    return false;
                if (!string.IsNullOrEmpty(errori))
                    return true;
                if (anag != null && anag.Rows.Count > 0)
                    NormalizzaAnagraficaArcaToDB(anag.Rows[0], out anagrafica);

                if (anagrafica == null)
                {
                    errori = "Errore nel recupero dell'anagrafica: Soggetto con CSog " + richiestaArca.CSog + " non presente sugli archivi";
                    return true;
                }

                if (pensioniRiferimento != null && pensioniRiferimento.Rows.Count > 0)
                    if (!GestioneAreaRiepilogo.ValorizzaPensioni(pensioniRiferimento, out elencoPensioni, out errori))
                        return false;
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche: " + Ex.Message;
                return false;
            }
            catch (Exception Ex)
            {
                errori = "Errore tecnico durante il recupero delle informazioni anagrafiche";
                string messaggio = string.Format("Errore nel recupero dell'anagrafica da ARCA: {0}", Utility.GetMessageFromException(Ex));
                string parametri = string.Format("Codice Soggetto: {0}", richiestaArca != null ? richiestaArca.CSog : null);
                long numeroDomanda = 0;
                long.TryParse(numDomanda, out numeroDomanda);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        #endregion internal members

        #region private members
        private static DataSet GetDatiArca(tDettaglio dettaglio)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable datiAnagrafici = new DataTable();
                datiAnagrafici.TableName = "ANAGRAFICA";
                #region aggiunta colonne
                datiAnagrafici.Columns.Add("MatricolaArca");
                datiAnagrafici.Columns.Add("CodiceFiscale");
                datiAnagrafici.Columns.Add("Cognome");
                datiAnagrafici.Columns.Add("CognomeAcquisito");
                datiAnagrafici.Columns.Add("Nome");
                datiAnagrafici.Columns.Add("Sesso");
                datiAnagrafici.Columns.Add("DataNascita");
                datiAnagrafici.Columns.Add("CodComuneNascita");
                datiAnagrafici.Columns.Add("ComuneNascita");
                datiAnagrafici.Columns.Add("SiglaProvinciaNascita");
                datiAnagrafici.Columns.Add("StatoNascita");
                datiAnagrafici.Columns.Add("CodComuneResidenza");
                datiAnagrafici.Columns.Add("ComuneResidenza");
                datiAnagrafici.Columns.Add("SiglaProvinciaResidenza");
                datiAnagrafici.Columns.Add("CapResidenza");
                datiAnagrafici.Columns.Add("SiglaNazioneResidenza");
                datiAnagrafici.Columns.Add("FrazioneResidenza");
                datiAnagrafici.Columns.Add("NumeroCivico");
                datiAnagrafici.Columns.Add("Indirizzo");
                datiAnagrafici.Columns.Add("IndResidenteEstero");
                datiAnagrafici.Columns.Add("Cittadinanza");
                datiAnagrafici.Columns.Add("NazioneCittadinanza");
                datiAnagrafici.Columns.Add("SiglaNazioneCittadinanza");
                datiAnagrafici.Columns.Add("StatoCivile");
                datiAnagrafici.Columns.Add("DataMorte");
                datiAnagrafici.Columns.Add("CodLingua");
                datiAnagrafici.Columns.Add("ProgProtocollo");
                datiAnagrafici.Columns.Add("Nazionalità");
                #endregion aggiunta colonne
                DataRow rigaAnagrafica = datiAnagrafici.NewRow();

                rigaAnagrafica["MatricolaArca"] = dettaglio.ChiaveArca.Codice + dettaglio.ChiaveArca.Progressivo.ToString().PadLeft(8, '0');
                tCodiceFiscale cf = dettaglio.AllProfile.CodiceFiscale;
                rigaAnagrafica["CodiceFiscale"] = cf.Cognome + cf.Nome + cf.Anno.ToString().PadLeft(2, '0') + cf.Mese + cf.Giorno.PadLeft(2, '0') + cf.Comune + cf.CodControllo;
                tDatiPersonali dp = dettaglio.AllProfile.DatiPersonali;
                rigaAnagrafica["Cognome"] = dp.Cognome;
                rigaAnagrafica["Nome"] = dp.Nome;
                if (dp.Sesso == "Maschio")
                    rigaAnagrafica["Sesso"] = "M";
                else
                    rigaAnagrafica["Sesso"] = "F";
                tData d = dp.DataNascita;
                rigaAnagrafica["DataNascita"] = d.Anno.ToString() + d.Mese.ToString().PadLeft(2, '0') + d.Giorno.ToString().PadLeft(2, '0');
                // Verifica comune e provincia di nascita (oppure stato estero)
                if (dp.ComuneNascita != null)
                {
                    rigaAnagrafica["CodComuneNascita"] = dp.ComuneNascita.Codice;
                    rigaAnagrafica["ComuneNascita"] = dp.ComuneNascita.Nome;
                    rigaAnagrafica["SiglaProvinciaNascita"] = dp.ComuneNascita.Provincia;
                    rigaAnagrafica["StatoNascita"] = "ITA";
                }
                else    // se si tratta di stato estero
                {
                    rigaAnagrafica["CodComuneNascita"] = dp.StatoNascita.Codice;
                    rigaAnagrafica["ComuneNascita"] = dp.StatoNascita.Nome;
                    rigaAnagrafica["SiglaProvinciaNascita"] = dp.Paese.Trim();
                    rigaAnagrafica["StatoNascita"] = "EST";
                }

                if (dettaglio.AllProfile.DatiIndirizzo != null)
                {
                    tDatiIndirizzo di = dettaglio.AllProfile.DatiIndirizzo;
                    rigaAnagrafica["CodComuneResidenza"] = di.ComuneResidenza.Codice;
                    rigaAnagrafica["ComuneResidenza"] = di.ComuneResidenza.Nome;
                    rigaAnagrafica["SiglaProvinciaResidenza"] = di.ComuneResidenza.Provincia;
                    rigaAnagrafica["CapResidenza"] = di.Cap;
                    if (di.ComuneResidenza.Codice.StartsWith("Z"))
                        rigaAnagrafica["SiglaNazioneResidenza"] = "EST";
                    else
                        rigaAnagrafica["SiglaNazioneResidenza"] = "ITA";
                    rigaAnagrafica["NumeroCivico"] = di.Civico;
                    string indirizzo = di.Indirizzo.PrimaParte;
                    if (di.Indirizzo.SecondaParte != "")
                        indirizzo += " " + di.Indirizzo.SecondaParte;
                    if (di.Indirizzo.TerzaParte != "")
                        indirizzo += " " + di.Indirizzo.TerzaParte;
                    if (di.Indirizzo.QuartaParte != "")
                        indirizzo += " " + di.Indirizzo.QuartaParte;
                    rigaAnagrafica["Indirizzo"] = indirizzo;
                    rigaAnagrafica["IndResidenteEstero"] = "N";
                    rigaAnagrafica["FrazioneResidenza"] = di.Frazione;
                }
                else
                {
                    tDatiIndirizzoEstero di = dettaglio.AllProfile.DatiIndirizzoEstero;
                    rigaAnagrafica["CodComuneResidenza"] = di.StatoResidenza.Codice;
                    rigaAnagrafica["ComuneResidenza"] = di.StatoResidenza.Descrizione;
                    rigaAnagrafica["SiglaProvinciaResidenza"] = di.Paese.Trim();
                    rigaAnagrafica["CapResidenza"] = di.Cap;
                    rigaAnagrafica["SiglaNazioneResidenza"] = "EST";
                    rigaAnagrafica["NumeroCivico"] = di.Civico;
                    string indirizzo = di.Indirizzo.PrimaParte;
                    if (di.Indirizzo.SecondaParte != "")
                        indirizzo += " " + di.Indirizzo.SecondaParte;
                    if (di.Indirizzo.TerzaParte != "")
                        indirizzo += " " + di.Indirizzo.TerzaParte;
                    rigaAnagrafica["Indirizzo"] = indirizzo;
                    rigaAnagrafica["IndResidenteEstero"] = "S";
                    rigaAnagrafica["FrazioneResidenza"] = di.CityName;
                }

                tUlterioriDatiPersonali ud = dettaglio.AllProfile.UlterioriDatiPersonali;
                if (!String.IsNullOrEmpty(ud.Cittadinanza.Codice) && ud.Cittadinanza.Codice == "0")
                {
                    rigaAnagrafica["Cittadinanza"] = "I";
                    rigaAnagrafica["NazioneCittadinanza"] = "ITALIA";
                    rigaAnagrafica["SiglaNazioneCittadinanza"] = "";
                    rigaAnagrafica["Nazionalità"] = "I";
                }
                else if (!String.IsNullOrEmpty(ud.Cittadinanza.Codice) && ud.Cittadinanza.Codice.Trim() != "")
                {
                    rigaAnagrafica["Cittadinanza"] = "E";
                    rigaAnagrafica["NazioneCittadinanza"] = "";
                    rigaAnagrafica["SiglaNazioneCittadinanza"] = "";
                    if (!string.IsNullOrEmpty(ud.Nazionalita))
                        rigaAnagrafica["Nazionalità"] = ud.Nazionalita.Trim();
                }
                else
                {
                    rigaAnagrafica["Cittadinanza"] = "";
                    rigaAnagrafica["NazioneCittadinanza"] = "";
                    rigaAnagrafica["SiglaNazioneCittadinanza"] = "";
                }
                rigaAnagrafica["StatoCivile"] = ud.StatoCivile.Codice;
                rigaAnagrafica["DataMorte"] = ud.DataMorte;
                rigaAnagrafica["CognomeAcquisito"] = ud.CognomeAcquisito;
                rigaAnagrafica["CodLingua"] = "ITA";
                rigaAnagrafica["ProgProtocollo"] = 0;
                datiAnagrafici.Rows.Add(rigaAnagrafica);
                ds.Tables.Add(datiAnagrafici);

                DataTable datiPensioniRiferimento = null;
                if (dettaglio.AllProfile.Riferimenti != null && dettaglio.AllProfile.Riferimenti.Length > 0)
                {
                    datiPensioniRiferimento = new DataTable();
                    datiPensioniRiferimento.TableName = "PENSIONERIFERIMENTO";
                    datiPensioniRiferimento.Columns.Add("Sede");
                    datiPensioniRiferimento.Columns.Add("Categoria");
                    datiPensioniRiferimento.Columns.Add("CodCategoria");
                    datiPensioniRiferimento.Columns.Add("NumeroCertificato");
                    datiPensioniRiferimento.Columns.Add("FlagEliminazione");
                    datiPensioniRiferimento.Columns.Add("TipoComponente");
                    datiPensioniRiferimento.Columns.Add("Decorrenza");

                    foreach (tRiferimenti r in dettaglio.AllProfile.Riferimenti)
                    {
                        if (r.ChiaveIndividuale.CodiceIndividualePensione != null)
                        {
                            DataRow rigaPensioneRiferimento = datiPensioniRiferimento.NewRow();
                            rigaPensioneRiferimento["Sede"] = r.ChiaveIndividuale.CodiceIndividualePensione.Sede;
                            rigaPensioneRiferimento["Categoria"] = r.ChiaveIndividuale.CodiceIndividualePensione.Categoria.Descrizione.Replace("/", "");
                            rigaPensioneRiferimento["CodCategoria"] = r.ChiaveIndividuale.CodiceIndividualePensione.Categoria.Codice;
                            rigaPensioneRiferimento["NumeroCertificato"] = r.ChiaveIndividuale.CodiceIndividualePensione.Certificato;
                            rigaPensioneRiferimento["FlagEliminazione"] = r.ChiaveIndividuale.CodiceIndividualePensione.FlagEliminazione;
                            rigaPensioneRiferimento["TipoComponente"] = r.ChiaveIndividuale.CodiceIndividualePensione.TipoComponente.Codice;
                            rigaPensioneRiferimento["Decorrenza"] = r.ChiaveIndividuale.CodiceIndividualePensione.Decorrenza;
                            datiPensioniRiferimento.Rows.Add(rigaPensioneRiferimento);
                        }
                    }
                }

                if (datiPensioniRiferimento != null && datiPensioniRiferimento.Rows.Count > 0)
                    ds.Tables.Add(datiPensioniRiferimento);

                return ds;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DataTable GetSinonimi(tSinonimi[] elencoSinonimi)
        {
            try
            {
                DataTable sinonimi = new DataTable();
                sinonimi.Columns.Add("ChiaveArca");
                sinonimi.Columns.Add("CodiceFiscale");
                sinonimi.Columns.Add("Cognome");
                sinonimi.Columns.Add("Nome");
                sinonimi.Columns.Add("DataNascita");
                sinonimi.Columns.Add("CodiceComuneNascita");

                foreach (tSinonimi sin in elencoSinonimi)
                {
                    DataRow dr = sinonimi.NewRow();
                    dr["ChiaveArca"] = sin.ChiaveArca.Codice + sin.ChiaveArca.Progressivo.ToString().PadLeft(8, '0');
                    dr["CodiceFiscale"] = GetCodiceFiscale(sin.LowProfile.CodiceFiscale);
                    dr["Cognome"] = sin.LowProfile.DatiPersonali.Cognome;
                    dr["Nome"] = sin.LowProfile.DatiPersonali.Nome;
                    tData d = sin.LowProfile.DatiPersonali.DataNascita;
                    dr["DataNascita"] = d.Anno.ToString() + d.Mese.ToString().PadLeft(2, '0') + d.Giorno.ToString().PadLeft(2, '0');
                    if (sin.LowProfile.DatiPersonali.ComuneNascita != null)
                        dr["CodiceComuneNascita"] = sin.LowProfile.DatiPersonali.ComuneNascita.Codice;
                    sinonimi.Rows.Add(dr);
                }
                if (sinonimi.Rows.Count > 0)
                    return sinonimi;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetCodiceFiscale(tCodiceFiscale codiceFiscale)
        {
            try
            {
                return codiceFiscale.Cognome.Trim().ToUpperInvariant() + codiceFiscale.Nome.Trim().ToUpperInvariant() +
                    codiceFiscale.Anno.ToString().Trim().ToUpperInvariant().PadLeft(2, '0') + codiceFiscale.Mese.Trim().ToUpperInvariant() +
                    codiceFiscale.Giorno.Trim().ToUpperInvariant().PadLeft(2, '0') + codiceFiscale.Comune.Trim().ToUpperInvariant() +
                    codiceFiscale.CodControllo.Trim().ToUpperInvariant();
            }
            catch (Exception)
            {
                return "";
            }
        }

        internal static void NormalizzaAnagraficaArcaToDB(DataRow soggetto, out Entity.Anagrafica anagrafica)
        {
            anagrafica = null;
            try
            {
                GestioneAnagrafica.DatiAnagrafici datiAnagrafici = new GestioneAnagrafica.DatiAnagrafici();
                datiAnagrafici.CodiceFiscale = soggetto["CodiceFiscale"] != DBNull.Value && soggetto["CodiceFiscale"] != null ? soggetto["CodiceFiscale"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.Cognome = soggetto["Cognome"] != DBNull.Value && soggetto["Cognome"] != null ? soggetto["Cognome"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.Nome = soggetto["Nome"] != DBNull.Value && soggetto["Nome"] != null ? soggetto["Nome"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.CognomeAcquisito = soggetto["CognomeAcquisito"] != DBNull.Value && soggetto["CognomeAcquisito"] != null ? soggetto["CognomeAcquisito"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.Sesso = soggetto["Sesso"] != DBNull.Value && soggetto["Sesso"] != null ? Utility.StringToNullableChar(soggetto["Sesso"].ToString().ToUpperInvariant().Trim()) : null;
                datiAnagrafici.DataNascita = soggetto["DataNascita"] != DBNull.Value && soggetto["DataNascita"] != null ? Utility.DataFromString(soggetto["DataNascita"].ToString(), Utility.FormatoData.AAAAmmGG) : null;
                datiAnagrafici.CodiceComuneNascita = soggetto["CodComuneNascita"] != DBNull.Value && soggetto["CodComuneNascita"] != null ? soggetto["CodComuneNascita"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.ComuneNascita = soggetto["ComuneNascita"] != DBNull.Value && soggetto["ComuneNascita"] != null ? soggetto["ComuneNascita"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.ProvinciaNascita = soggetto["SiglaProvinciaNascita"] != DBNull.Value && soggetto["SiglaProvinciaNascita"] != null ? soggetto["SiglaProvinciaNascita"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.CodiceComuneResidenza = soggetto["CodComuneResidenza"] != DBNull.Value && soggetto["CodComuneResidenza"] != null ? soggetto["CodComuneResidenza"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.ComuneResidenza = soggetto["ComuneResidenza"] != DBNull.Value && soggetto["ComuneResidenza"] != null ? soggetto["ComuneResidenza"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.ProvinciaResidenza = soggetto["SiglaProvinciaResidenza"] != DBNull.Value && soggetto["SiglaProvinciaResidenza"] != null ? soggetto["SiglaProvinciaResidenza"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.FrazioneResidenza = soggetto["FrazioneResidenza"] != DBNull.Value && soggetto["FrazioneResidenza"] != null ? soggetto["FrazioneResidenza"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.CAP = soggetto["CapResidenza"] != DBNull.Value && soggetto["CapResidenza"] != null ? soggetto["CapResidenza"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.NCivico = soggetto["NumeroCivico"] != DBNull.Value && soggetto["NumeroCivico"] != null ? soggetto["NumeroCivico"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.Indirizzo = soggetto["Indirizzo"] != DBNull.Value && soggetto["Indirizzo"] != null ? soggetto["Indirizzo"].ToString().ToUpperInvariant().Trim() : null;
                datiAnagrafici.ResidenzaEstero = soggetto["IndResidenteEstero"] != DBNull.Value && soggetto["IndResidenteEstero"] != null ? soggetto["IndResidenteEstero"].ToString() == "S" ? (bool?)true : (bool?)false : null;
                datiAnagrafici.CodiceStatoCivile = soggetto["StatoCivile"] != DBNull.Value && soggetto["StatoCivile"] != null && soggetto["StatoCivile"].ToString() != "0" && soggetto["StatoCivile"].ToString() != "9"
                    ? Utility.StringToNullableChar(soggetto["StatoCivile"].ToString().ToUpperInvariant().Trim()) : null;
                if (datiAnagrafici.CodiceStatoCivile.HasValue)
                {
                    List<char> elencoStatiCiviliAmmessi = new List<char> { '1', '2', '3', '4', '5', '7', '8', 'C' };
                    if (!elencoStatiCiviliAmmessi.Contains(datiAnagrafici.CodiceStatoCivile.Value))
                        datiAnagrafici.CodiceStatoCivile = null;
                }
                datiAnagrafici.DataMorte = soggetto["DataMorte"] != DBNull.Value && soggetto["DataMorte"] != null ? Utility.DataFromString(soggetto["DataMorte"].ToString(), Utility.FormatoData.AAAAmmGG) : null;
                string codCatastale = string.Empty;
                if (string.IsNullOrEmpty(datiAnagrafici.CodiceComuneNascita))
                {
                    if (!string.IsNullOrEmpty(datiAnagrafici.ComuneNascita) && !string.IsNullOrEmpty(datiAnagrafici.ProvinciaNascita))
                    {

                        GestioneDecodifica.GetCodiceCatastalePerComune_Provincia(datiAnagrafici.ComuneNascita, datiAnagrafici.ProvinciaNascita, out codCatastale);
                        if (!string.IsNullOrEmpty(codCatastale))
                            datiAnagrafici.CodiceComuneNascita = codCatastale;
                    }
                }
                if (string.IsNullOrEmpty(datiAnagrafici.CodiceComuneResidenza))
                {
                    if (!string.IsNullOrEmpty(datiAnagrafici.ComuneResidenza) && !string.IsNullOrEmpty(datiAnagrafici.ProvinciaResidenza))
                    {
                        codCatastale = string.Empty;
                        GestioneDecodifica.GetCodiceCatastalePerComune_Provincia(datiAnagrafici.ComuneResidenza, datiAnagrafici.ProvinciaResidenza, out codCatastale);
                        if (!string.IsNullOrEmpty(codCatastale))
                            datiAnagrafici.CodiceComuneResidenza = codCatastale;
                    }
                    else if (!string.IsNullOrEmpty(datiAnagrafici.CAP))
                    {
                        codCatastale = string.Empty;
                        GestioneDecodifica.GetCodiceCatastalePerCap(datiAnagrafici.CAP, out codCatastale);
                        if (!string.IsNullOrEmpty(codCatastale))
                            datiAnagrafici.CodiceComuneResidenza = codCatastale;
                    }
                }
                if (soggetto["Nazionalità"] != DBNull.Value && soggetto["Nazionalità"] != null && !string.IsNullOrEmpty(soggetto["Nazionalità"].ToString()))
                {
                    if (soggetto["Nazionalità"].ToString() == "I")
                        datiAnagrafici.Cittadinanza = "Z000";
                    else
                    {
                        List<GestioneDecodifica.StatoEstero> elencoStatiEsteri = null;
                        GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);

                        if (elencoStatiEsteri != null && elencoStatiEsteri.Count > 0)
                        {
                            List<GestioneDecodifica.StatoEstero> listStatiEsteri = elencoStatiEsteri.FindAll(x => x.Sigla == soggetto["Nazionalità"].ToString());
                            if (listStatiEsteri != null && listStatiEsteri.Count == 1)
                                datiAnagrafici.Cittadinanza = listStatiEsteri.First().CodCatastale;
                        }
                    }
                }
                if (datiAnagrafici != null)
                {
                    List<Entity.Anagrafica> anag = null;
                    string errori = "";
                    if (!GestioneAreaRiepilogo.ValorizzaAnagraficaFromDB(datiAnagrafici, out anag, out errori))
                        throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'anagrafica proveniente dal DB");
                    anagrafica = anag[0];
                }
            }
            catch (Exception Ex)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante la normalizzazione dell'anagrafica proveniente da ARCA: " + Ex.Message);
            }
        }
        #endregion private members

        #region nested class
        public class RichiestaARCA
        {
            #region private properties
            private string _CodiceFiscale;
            private string _CodiceFiscaleRichiedente;
            private string _Applicazione;
            private string _Matricola;
            private string _Provenienza;
            private string _Ruolo;
            private string _Cognome;
            private string _Nome;
            private string _Sesso;
            private DateTime? _DataNascita;
            private int? _CSog;
            #endregion private properties

            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public string CodiceFiscaleRichiedente { get { return _CodiceFiscaleRichiedente; } set { _CodiceFiscaleRichiedente = value; } }
            public string Applicazione { get { return _Applicazione; } set { _Applicazione = value; } }
            public string Matricola { get { return _Matricola; } set { _Matricola = value; } }
            public string Provenienza { get { return _Provenienza; } set { _Provenienza = value; } }
            public string Ruolo { get { return _Ruolo; } set { _Ruolo = value; } }
            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
            public string Nome { get { return _Nome; } set { _Nome = value; } }
            public string Sesso { get { return _Sesso; } set { _Sesso = value; } }
            public DateTime? DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }
            public int? CSog { get { return _CSog; } set { _CSog = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
