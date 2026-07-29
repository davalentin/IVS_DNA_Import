using INPS.Pensioni.Liquidazione.ServiceReferences.SrvCalcoloQuote;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Channels;

namespace INPS.Pensioni.Liquidazione
{
    public class IdentitySrv
    {
        public string AppName { get; set; }
        public string AppKey { get; set; }
        public string UserId { get; set; }
        public string IdentityProvider { get; set; }
    }
    public class GestioneCalcoloQuote
    {
        public static bool Calcola(string codiceFiscale, GestionePensione.DatiPensione datiPensione, BLCommon.Entity.DatiSupplementi[] listaSupplementi, DateTime? inizioAssicurazione, out BLCommon.Entity.DatiSupplementi[] listaSupplementiOut, bool isRiapertura, out string errori, out bool erroreTemporaneo)
        {
            bool erroreTecnico = false;
            errori = "";
            erroreTemporaneo = false;
            //CalcoloQuoteClient proxy = new CalcoloQuoteClient();
            string stackTrace = null;
            listaSupplementiOut = null;
            Guid guid = Guid.NewGuid();
            CalcoloQuoteRequest request = ValorizzaCalcoloQuoteRequest(codiceFiscale, datiPensione, inizioAssicurazione, listaSupplementi, isRiapertura);
            GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvCalcoloQuote, Utility.MetodoServizio.CalcolaQuote, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
            CalcoloQuoteResponse response = null;
            using (var proxy = new CalcoloQuoteClient())
            using (var scope = new OperationContextScope(proxy.InnerChannel))
            {
                try
                {
                    IdentitySrv identity = new IdentitySrv();
                    identity.AppName = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppName"] : "";
                    identity.AppKey = ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] != null ? ConfigurationManager.AppSettings["SrvCalcolaQuote_AppKey"] : "";
                    identity.UserId = datiPensione.MatricolaUtenteAcquisizione;
                    identity.IdentityProvider = "AD";

                    MessageHeader identityHeader = MessageHeader.CreateHeader("Identity", "https://inps.it", identity);
                    OperationContext.Current.OutgoingMessageHeaders.Add(identityHeader);

                    response = proxy.Calcola(request);
                    if (response.CodEsito != 0 && response.CodEsito != 100001 && response.CodEsito != 10000)
                    {
                        errori = response.DescrizioneEsito;
                        //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvCalcoloQuote, Utility.MetodoServizio.CalcolaQuote, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                        return false;
                    }
                    else if (response.CodEsito == 100001 || response.CodEsito == 10000)
                    {
                        errori = response.DescrizioneEsito;
                        erroreTemporaneo = true;
                        //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvCalcoloQuote, Utility.MetodoServizio.CalcolaQuote, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                        return false;
                    }
                    else
                    {
                        List<BLCommon.Entity.DatiSupplementi> newList = new List<BLCommon.Entity.DatiSupplementi>();
                        newList = listaSupplementi.ToList();
                        //Mappare response
                        foreach (var quota in response.Quote.Select((value, i) => new { i, value }))
                        {
                            BLCommon.Entity.DatiSupplementi suppl = new BLCommon.Entity.DatiSupplementi();
                            suppl.CodGestioneSupplemento = quota.value.CodiceGestione;
                            suppl.AmmontareContributivo = quota.value.AmmontareContributi;
                            suppl.NSettimaneSupplemento = quota.value.Settimane;
                            suppl.MontanteSupplemento = quota.value.MontanteIvs;
                            suppl.CodiceLiquidazione = quota.value.CodiceLiquidazione == "C" ? 3 : (quota.value.CodiceLiquidazione == "D" ? 4 : (byte?)null);
                            suppl.TipoSupplemento = 'C';
                            suppl.DecorrenzaSupplemento = response.DataDecorrenza;
                            newList.Add(suppl);
                        }
                        listaSupplementiOut = newList.ToArray();
                    }
                    //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvCalcoloQuote, Utility.MetodoServizio.CalcolaQuote, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Calcolo quote | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio Calcolo quote | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio Calcolo quote | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio Calcolo quote: {0}", Utility.GetMessageFromException(Ex));
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
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda";
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }               
            }
            GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvCalcoloQuote, Utility.MetodoServizio.CalcolaQuote, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);

            return true;
        }

        private static CalcoloQuoteRequest ValorizzaCalcoloQuoteRequest(string codiceFiscale, GestionePensione.DatiPensione datiPensione, DateTime? inizioAssicurazione, BLCommon.Entity.DatiSupplementi[] listaSupplementiIn, bool isRiapertura)
        {
            CalcoloQuoteRequest request = new CalcoloQuoteRequest();
            try
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                request.DataInizioAssicurazione = inizioAssicurazione != null ? (DateTime)inizioAssicurazione : DateTime.MinValue;
                request.DataDecorrenza = new DateTime(datiPensione.DataPresentazioneDomanda.AddMonths(1).Year, datiPensione.DataPresentazioneDomanda.AddMonths(1).Month, 1);
                request.SiglaCategoria = datiPensione.SiglaCategoria;
                request.CodiceSede = Utility.GetCodiceSedeLavorazione(datiPensione, isRiapertura).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, isRiapertura).ToString().PadLeft(2, '0');

                var listaSupplementi = listaSupplementiIn.ToList();
                if (listaSupplementi != null && listaSupplementi.Count() > 0)
                {
                    for (int i = 0; i < listaSupplementi.Count; i++)
                    {
                        if (Utility.ConfrontaOggetti(listaSupplementi[i], new BLCommon.Entity.DatiSupplementi()))
                        {
                            listaSupplementi.RemoveAt(i);
                            i--;
                        }
                    }
                    if (listaSupplementi.Count() > 0)
                    {
                        var listaSupplementiOrdered = listaSupplementi.OrderByDescending(x => x.DecorrenzaSupplemento).ToList();
                        request.DataInizioSupplemento = (DateTime)listaSupplementiOrdered.First().DecorrenzaSupplemento;
                        request.DataFineSupplemento = new DateTime(datiPensione.DataPresentazioneDomanda.AddYears(-2).Year, 12, 31);
                        List<string> listaCodiciAmmessi = new List<string> { "2", "3", "4", "I", "M", "N" };

                        var index = listaSupplementiOrdered.Find(delegate
                            (BLCommon.Entity.DatiSupplementi suppl)
                        {
                            return (!String.IsNullOrEmpty(suppl.CodGestioneSupplemento) && listaCodiciAmmessi.Contains(suppl.CodGestioneSupplemento.Trim()));
                        });
                        if (index != null)
                            request.SupplementoGestioneSpeciale = true;

                        List<string> listaCodiciAut = new List<string> { "I", "M", "N" };
                        var supplAut = listaSupplementiOrdered.Find(x => !String.IsNullOrEmpty(x.CodGestioneSupplemento) && listaCodiciAut.Contains(x.CodGestioneSupplemento.Trim()));
                        if (supplAut != null)
                            request.DataAutSuppPrec = supplAut.CodGestioneSupplemento;
                    }
                    else
                    {
                        request.DataInizioSupplemento = (DateTime)datiPensione.DecorrenzaOriginaria;
                        request.DataFineSupplemento = new DateTime(datiPensione.DataPresentazioneDomanda.AddYears(-2).Year, 12, 31);
                    }
                }
                else
                {
                    request.DataInizioSupplemento = (DateTime)datiPensione.DecorrenzaOriginaria;
                    request.DataFineSupplemento = new DateTime(datiPensione.DataPresentazioneDomanda.AddYears(-2).Year, 12, 31);
                }

                request.CodiceFiscale = codiceFiscale;
            }
            catch (Exception ex)
            {
            }
            return request;
        }
    }
}
