using System;
using System.ServiceModel;
using System.Linq;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.StampeWeb;
using INPS.Pensioni.Liquidazione.ServiceReferences.StampeWebDP;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneStampeWeb
    {
        #region public members
        public static bool GetStampaDomanda(GestionePensione.DatiPensione datiPensione, out byte[] datiStampa, out string errori)
        {
            errori = string.Empty;
            datiStampa = null;

            if (datiPensione == null)
                return true;

            string codCat = datiPensione.GetCodCategoria();
            if (codCat != null && codCat.Length > 3)
                codCat = codCat.Substring(codCat.Length - 3, 3);
            short codiceSede = 0;
            byte? centroOperativo = null;
            int certificato = 0;
            bool isVerify = false;
            string descrMod = null;
            string nDomus = string.Empty;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
            {
                if (datiPensione.CodiceSedeDestinazione.HasValue)
                {
                    codiceSede = datiPensione.CodiceSedeDestinazione.Value;
                    if (Utility.IsPensioniOvunqueAttiva(tipoAppartenenza) && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                        centroOperativo = (byte)datiPensione.GP1AV11.GetValueOrDefault();
                    else if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && datiPensione.CodiceSedeDestinazione == 9901) //sedi virtuali
                        centroOperativo = datiPensione.CentroOperativo;
                }
                else
                {
                    codiceSede = datiPensione.CodiceSede;
                    centroOperativo = (Utility.IsPensioniOvunqueAttiva(tipoAppartenenza) && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))) ? (byte)datiPensione.GP1AV11.GetValueOrDefault() : datiPensione.CentroOperativo;
                }
                if (!Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.StatoPensione.HasValue && datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.CalcoloVerify)
                    certificato = datiPensione.NCertificatoProvvisorio.HasValue ? datiPensione.NCertificatoProvvisorio.Value : 0;
                else
                    certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            }
            else if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                if (datiPensione.CodiceSedeDestinazione.HasValue && !(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI && Utility.IsDomandaAPEPrecoci(datiPensione)))
                {
                    codiceSede = datiPensione.CodiceSedeDestinazione.Value;
                    centroOperativo = (Utility.IsPensioniOvunqueAttiva(tipoAppartenenza) && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))) ? (byte)datiPensione.GP1AV11.GetValueOrDefault() : datiPensione.CentroOperativoDestinazione.HasValue ? datiPensione.CentroOperativoDestinazione.Value : (byte)0;
                }
                else
                {
                    codiceSede = datiPensione.CodiceSede;
                    centroOperativo = (Utility.IsPensioniOvunqueAttiva(tipoAppartenenza) && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))) ? (byte)datiPensione.GP1AV11.GetValueOrDefault() : datiPensione.CentroOperativo;
                }
                certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            }
            else
            {
                codiceSede = datiPensione.CodiceSede;
                centroOperativo = (Utility.IsPensioniOvunqueAttiva(tipoAppartenenza) && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))) ? (byte)datiPensione.GP1AV11.GetValueOrDefault() : datiPensione.CentroOperativo;
                certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            }

            if (ConfigurationManager.AppSettings["UsaDataPower"] == null ||
                            ConfigurationManager.AppSettings["UsaDataPower"] != "SI")
            {
                if (!GetStampaWeb(codiceSede, centroOperativo, codCat, certificato, out datiStampa, out errori))
                    return false;
            }
            else
            {
                if (!GetStampaWebDP(datiPensione, codiceSede, centroOperativo, codCat, certificato, datiPensione.IsNuovoCalcolo.GetValueOrDefault(), out isVerify, out descrMod, out nDomus, out datiStampa, out errori))
                    return false;
            }

            return true;
        }

        public static bool GetStampaDomanda(string siglaCategoria, string codiceSede, string certificato, out byte[] datiStampa, out string errori)
        {
            errori = string.Empty;
            datiStampa = null;

            string codCat = string.Empty;
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(siglaCategoria, out codCat);
            if (codCat != null && codCat.Length > 3)
                codCat = codCat.Substring(codCat.Length - 3, 3);
            short codSede = 0;
            int cert = 0;
            short.TryParse(codiceSede, out codSede);
            int.TryParse(certificato, out cert);
            bool isVerify = false;
            string descrMod = null;
            string nDomus = string.Empty;
            if (ConfigurationManager.AppSettings["UsaDataPower"] == null ||
                            ConfigurationManager.AppSettings["UsaDataPower"] != "SI")
            {
                if (!GetStampaWeb(codSede, null, codCat, cert, out datiStampa, out errori))
                    return false;
            }
            else
            {
                GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
                if (!GetStampaWebDP(datiPensione, codSede, null, codCat, cert, false, out isVerify, out descrMod, out nDomus, out datiStampa, out errori))
                    return false;
            }

            return true;
        }

        public static bool GetStampaDomanda(GestionePensione.DatiPensione datiPensione, string siglaCategoria, string codiceSede, string certificato, out byte[] datiStampa, out string errori)
        {
            errori = string.Empty;
            datiStampa = null;

            string codCat = string.Empty;
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(siglaCategoria, out codCat);
            if (codCat != null && codCat.Length > 3)
                codCat = codCat.Substring(codCat.Length - 3, 3);
            short codSede = 0;
            int cert = 0;
            short.TryParse(codiceSede, out codSede);
            int.TryParse(certificato, out cert);
            bool isVerify = false;
            string descrMod = null;
            string nDomus = string.Empty;
            if (ConfigurationManager.AppSettings["UsaDataPower"] == null ||
                            ConfigurationManager.AppSettings["UsaDataPower"] != "SI")
            {
                if (!GetStampaWeb(codSede, null, codCat, cert, out datiStampa, out errori))
                    return false;
            }
            else
            {
                if (!GetStampaWebDP(datiPensione, codSede, null, codCat, cert, false, out isVerify, out descrMod, out nDomus, out datiStampa, out errori))
                    return false;
            }

            return true;
        }

        public static bool IsDomandaConStampaGenerata(GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            short codiceSede = 0;
            byte? centroOperativo = null;
            int certificato = 0;
            byte[] datiStampa = null;
            bool isVerify = false;
            string descrMod = null;
            string nDomus = string.Empty;
            string codCat = datiPensione.GetCodCategoria();
            if (codCat != null && codCat.Length > 3)
                codCat = codCat.Substring(codCat.Length - 3, 3);

            if (datiPensione == null)
                return false;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
            {
                if (datiPensione.CodiceSedeDestinazione.HasValue)
                {
                    codiceSede = datiPensione.CodiceSedeDestinazione.Value;
                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && !Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) && datiPensione.CodiceSedeDestinazione == 9901) //sedi virtuali
                        centroOperativo = datiPensione.CentroOperativo;
                }
                else
                {
                    codiceSede = datiPensione.CodiceSede;
                    centroOperativo = datiPensione.CentroOperativo;
                }
                if (!Utility.IsDomandaINPDAP(datiPensione.Gestione) && datiPensione.StatoPensione.HasValue && datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.CalcoloVerify)
                    certificato = datiPensione.NCertificatoProvvisorio.HasValue ? datiPensione.NCertificatoProvvisorio.Value : 0;
                else
                    certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            }
            else if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione))
            {
                if (datiPensione.CodiceSedeDestinazione.HasValue)
                {
                    codiceSede = datiPensione.CodiceSedeDestinazione.Value;
                    centroOperativo = datiPensione.CentroOperativoDestinazione.HasValue ? datiPensione.CentroOperativoDestinazione.Value : (byte)0;
                }
                else
                {
                    codiceSede = datiPensione.CodiceSede;
                    centroOperativo = datiPensione.CentroOperativo;
                }
                certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            }
            else
            {
                codiceSede = datiPensione.CodiceSede;
                centroOperativo = datiPensione.CentroOperativo;
                certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
            }

            if (!GetStampaWebDP(datiPensione, codiceSede, centroOperativo, codCat, certificato, datiPensione.IsNuovoCalcolo.GetValueOrDefault(), out isVerify, out descrMod, out nDomus, out datiStampa, out errori))
                return false;

            bool? isRicostituzioneOrRiapertura = !string.IsNullOrEmpty(descrMod) ? (descrMod.StartsWith("P.L.") ? false : (descrMod == "RIC." ? true : (bool?)null)) : (bool?)null;

            if (datiStampa == null || isVerify || nDomus != datiPensione.NDomus.ToString() || !isRicostituzioneOrRiapertura.HasValue || isRicostituzioneOrRiapertura.Value != Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura))
            {
                string message = "Pensione presente sul GP con stampa incongruente";
                string parameters = string.Format("DatiStampa presenti: {0}, IsVerify: {1}, NDomus: {2}, DescrMod: {3}", datiStampa != null ? "SI" : "NO", isVerify.ToString(), nDomus, descrMod);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.Informativo, message, parameters, null);
                return false;
            }

            return true;
        }
        #endregion public members

        #region internal members
        private static bool GetStampaWeb(short sede, byte? centroOperativo, string categoria, int certificato, out byte[] datiStampa, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            Service1SoapClient proxy = null;
            datiStampa = null;
            string tipoModello = "TP150";
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    proxy = new Service1SoapClient();
                    datiStampa = proxy.RichiestaModelliDB2(sede.ToString().PadLeft(4, '0') + (centroOperativo.HasValue ? centroOperativo.Value.ToString().PadLeft(2, '0') : "00"),
                        categoria, certificato.ToString().PadLeft(8, '0'),
                        ConfigurationManager.AppSettings["StampeWeb-Applicazione"] != null ? ConfigurationManager.AppSettings["StampeWeb-Applicazione"] : "",
                        ConfigurationManager.AppSettings["StampeWeb-UserID"] != null ? ConfigurationManager.AppSettings["StampeWeb-UserID"] : "",
                        ConfigurationManager.AppSettings["StampeWeb-Password"] != null ? ConfigurationManager.AppSettings["StampeWeb-Password"] : "",
                        tipoModello);
                    if (datiStampa == null)
                    {
                        errori = "Errore dal servizio stampeWeb: Nessuna stampa disponibile";
                        return false;
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio StampeWeb | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio StampeWeb | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio StampeWeb | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio StampeWeb: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero della stampa";
                        string parametri = string.Format("Codice sede: {0}; Centro operativo: {1}, Categoria: {2}; Certificato: {3}", sede, centroOperativo, categoria, certificato);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }

            }
            return true;
        }

        private static bool GetStampaWebDP(GestionePensione.DatiPensione datiPensione, short sede, byte? centroOperativo, string categoria, int certificato, bool isNuovoCalcolo, out bool isVerify, out string descrMod, out string nDomus, out byte[] datiStampa, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            ServizioRichiesteSoapClient proxy = null;
            DocumentoTE08 risposta = null;
            isVerify = false;
            descrMod = null;
            nDomus = string.Empty;
            datiStampa = null;
            string stackTrace = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    proxy = new ServizioRichiesteSoapClient();

                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitazione_NuovoMetodoStampe = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("Abilitazione_NuovoMetodoStampe", out ctrlAbilitazione_NuovoMetodoStampe);
                    Guid guid = Guid.NewGuid();
                    if (ctrlAbilitazione_NuovoMetodoStampe != null && !String.IsNullOrEmpty(ctrlAbilitazione_NuovoMetodoStampe.ValoreControllo) && !String.IsNullOrEmpty(ctrlAbilitazione_NuovoMetodoStampe.ValoreControllo.Trim()) &&
                        ctrlAbilitazione_NuovoMetodoStampe.ValoreControllo == "SI")
                    {                      
                        string[] flagsIndebito = new string[4] { "I", "C", "A", "M" };
                        bool flagStampe;
                        if (isNuovoCalcolo)
                            flagStampe = true;
                        else
                        {
                            if (datiPensione.FlagVerify == false && (datiPensione.StatoPensione == 4 || (datiPensione.StatoPensione >= 8 && datiPensione.StatoPensione <= 19) || datiPensione.StatoPensione >= 22) && flagsIndebito.Contains(datiPensione.FlagIndebito))
                                flagStampe = true;
                            else
                                flagStampe = false;
                        }

                        GestioneLogSoap.SalvaLogSoap(categoria + sede.ToString().PadLeft(4, '0') + certificato.ToString().PadLeft(8, '0') + ", centro op: " + (centroOperativo.HasValue ? centroOperativo.Value.ToString().PadLeft(2, '0') : "00") + ", flagStampe: " + flagStampe.ToString(),
                             Utility.Servizio.SrvStampeWeb, Utility.MetodoServizio.RichiestaTE08DB2NewIVS, Utility.SOAPLogDirection.IN, categoria + sede.ToString().PadLeft(4, '0') + certificato.ToString().PadLeft(8, '0'), guid);

                        risposta = proxy.RichiestaTE08DB2NewIVS(sede.ToString().PadLeft(4, '0') + (centroOperativo.HasValue ? centroOperativo.Value.ToString().PadLeft(2, '0') : "00"),
                        categoria, certificato.ToString().PadLeft(8, '0'), flagStampe,
                        ConfigurationManager.AppSettings["StampeWeb-Applicazione"] != null ? ConfigurationManager.AppSettings["StampeWeb-Applicazione"] : "",
                        ConfigurationManager.AppSettings["StampeWeb-UserID"] != null ? ConfigurationManager.AppSettings["StampeWeb-UserID"] : "",
                        ConfigurationManager.AppSettings["StampeWeb-Password"] != null ? ConfigurationManager.AppSettings["StampeWeb-Password"] : "");
                    }
                    else
                        risposta = proxy.RichiestaTE08DB2(sede.ToString().PadLeft(4, '0') + (centroOperativo.HasValue ? centroOperativo.Value.ToString().PadLeft(2, '0') : "00"),
                        categoria, certificato.ToString().PadLeft(8, '0'),
                        ConfigurationManager.AppSettings["StampeWeb-Applicazione"] != null ? ConfigurationManager.AppSettings["StampeWeb-Applicazione"] : "",
                        ConfigurationManager.AppSettings["StampeWeb-UserID"] != null ? ConfigurationManager.AppSettings["StampeWeb-UserID"] : "",
                        ConfigurationManager.AppSettings["StampeWeb-Password"] != null ? ConfigurationManager.AppSettings["StampeWeb-Password"] : "");

                    if (risposta == null)
                    {             
                        errori = "Errore nel recupero della stampa: Nessuna stampa disponibile";
                        return false;
                    }
                    else if (risposta.CodiceRitorno != "0")
                    {
                        GestioneLogSoap.SalvaLogSoap(risposta.Messaggio,
                                Utility.Servizio.SrvStampeWeb, Utility.MetodoServizio.RichiestaTE08DB2NewIVS, Utility.SOAPLogDirection.OUT, categoria + sede.ToString().PadLeft(4, '0') + certificato.ToString().PadLeft(8, '0'), guid);
                        errori = "Errore nel recupero della stampa: " + risposta.Messaggio;
                        return false;
                    }
                    else
                    {
                        datiStampa = risposta.DocumentoPDF;
                        isVerify = risposta.IsVerify;
                        nDomus = risposta.NumDomus;
                        descrMod = risposta.DescrMod;
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio StampeWeb | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio StampeWeb | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio StampeWeb | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio StampeWeb: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero della stampa";
                        string parametri = string.Format("Codice sede: {0}; Centro operativo: {1}, Categoria: {2}; Certificato: {3}", sede, centroOperativo, categoria, certificato);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }

            }
            return true;
        }
        #endregion internal members
    }
}
