using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.Pensioni.LiquidazioneCi.ServiceReferences.AllegatiConvenzioni;
using System.ServiceModel;
using System.Reflection;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneAllegatiConvenzioni
    {
        #region public methods

        public static void GetPrestazioneEstereByNumeroDomanda(long nDomus, string matricola, short codiceSede, short centroOperativo, out List<INPS.Pensioni.LiquidazioneCi.GestioneContrib.PrestazioneEstera> listaPrestazioniEstere, out string cittadinanzaTitolare, out string errori)
        {
            listaPrestazioniEstere = null;
            cittadinanzaTitolare = string.Empty;
            errori = string.Empty;

            TipoModuloLetturaCI05Risposta dati;
            GetLetturaCI05ByNumeroDomanda(nDomus, matricola, codiceSede, centroOperativo, out dati, out errori);
            if (dati != null && string.IsNullOrEmpty(errori))
            {
                NormalizzaAreaFromAllegatiConvezioni(dati, out listaPrestazioniEstere, out cittadinanzaTitolare, out errori);
            }
        }

        #endregion public methods

        #region private methods

        private static void NormalizzaAreaFromAllegatiConvezioni(TipoModuloLetturaCI05Risposta dati, out List<INPS.Pensioni.LiquidazioneCi.GestioneContrib.PrestazioneEstera> listaPrestazioniEstere,
            out string cittadinanzaTitolare, out string errori)
        {
            listaPrestazioniEstere = null;
            cittadinanzaTitolare = string.Empty;
            errori = string.Empty;

            if (dati != null && dati.DatiRisposta != null && dati.DatiRisposta.Ciead75 != null && dati.DatiRisposta.Ciead75.AreaRichiedente != null &&
                dati.DatiRisposta.Ciead75.AreaRichiedente.AreaCi2005 != null)
            {
                if (dati.DatiRisposta.Ciead75.AreaRichiedente.AreaCi2005.AreaStatiEsteri != null)
                {
                    TipoAreaStatiEsteri risposta = dati.DatiRisposta.Ciead75.AreaRichiedente.AreaCi2005.AreaStatiEsteri;
                    for (int i = 1; i < 7; i++)
                    {
                        Type myType = typeof(TipoAreaStatiEsteri); // Get the PropertyInfo object by passing the property name. 
                        PropertyInfo myPropInfo = myType.GetProperty("Stis0" + i);
                        int stis = (int)myPropInfo.GetValue(risposta, null);
                        myPropInfo = myType.GetProperty("Matre" + i);
                        string matre = (string)myPropInfo.GetValue(risposta, null);
                        myPropInfo = myType.GetProperty("Pi" + i);
                        string pi = (string)myPropInfo.GetValue(risposta, null);
                        myPropInfo = myType.GetProperty("Preste" + i);
                        string preste = (string)myPropInfo.GetValue(risposta, null);
                      
                        if (stis != 0)
                        {
                            Data.aciistit descPrestazioneEstera = null;
                            Data.DAPrestazioniEstere.GetPrestazioneEstera(stis.ToString().PadLeft(6, '0'), out descPrestazioneEstera);
                            if (descPrestazioneEstera != null)
                            {
                                if ((String.IsNullOrEmpty(pi) && String.IsNullOrEmpty(preste)) || (String.IsNullOrEmpty(pi.Trim()) && String.IsNullOrEmpty(preste.Trim())) || 
                                    pi.ToUpperInvariant() != "A")
                                {
                                    errori = "Provvedimento italiano mancante o errato per stato " + (descPrestazioneEstera.CDSTAIST.Length == 6 ? descPrestazioneEstera.CDSTAIST.Substring(0, 2) : "");
                                    return;
                                }

                                if (listaPrestazioniEstere == null)
                                    listaPrestazioniEstere = new List<INPS.Pensioni.LiquidazioneCi.GestioneContrib.PrestazioneEstera>();
                                listaPrestazioniEstere.Add(new INPS.Pensioni.LiquidazioneCi.GestioneContrib.PrestazioneEstera(descPrestazioneEstera.CDSTAIST, descPrestazioneEstera.SIGLISTI,
                                    descPrestazioneEstera.CITTAIST, descPrestazioneEstera.NOMESTAT, descPrestazioneEstera.SIGLASTAT, descPrestazioneEstera.CODICONV,
                                    !String.IsNullOrEmpty(matre) ? matre.Trim() : "", string.IsNullOrEmpty(pi) ? (char?)null : pi[0], false));
                            }
                        }
                    }
                }
                if (dati.DatiRisposta.Ciead75.AreaRichiedente.AreaCi2005.AreaCarCor != null)
                {
                    string siglaNazione = dati.DatiRisposta.Ciead75.AreaRichiedente.AreaCi2005.AreaCarCor.Citt1;
                    if (!string.IsNullOrEmpty(siglaNazione) && siglaNazione.Trim() != string.Empty)
                    {
                        List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
                        GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);
                        if (listaStatiEsteri != null)
                        {
                            // Viene effettuata questa modifica perchè dal CI05 nel caso di Italia ci arriva "I", mentre nella listaStatiEsteri abbiamo "ITA"
                            if (siglaNazione.Trim() == "I")
                                siglaNazione = "ITA";

                            List<GestioneDecodifica.StatoEstero> statiEsteri = listaStatiEsteri.FindAll(x => x.Sigla.Trim().ToUpperInvariant() == siglaNazione.Trim().ToUpperInvariant());
                            if (statiEsteri != null && statiEsteri.Count == 1 && statiEsteri[0] != null)
                            {
                                cittadinanzaTitolare = statiEsteri[0].CodCatastale;
                            }
                        }
                    }
                }
            }
        }

        private static bool GetLetturaCI05ByNumeroDomanda(long nDomus, string matricola, short codiceSede, short centroOperativo, out TipoModuloLetturaCI05Risposta dati, out string errori)
        {
            errori = string.Empty;
            dati = null;
            Guid guid = Guid.NewGuid();

            try
            {
                TipoModuloLetturaCI05Richiesta input = new TipoModuloLetturaCI05Richiesta();
                input.MetadatiServizio = new TipoMetadatiServizio();
                input.DatiRichiesta = new TipoLetturaCI05Richiesta();

                input.MetadatiServizio.NomeServizio = TipoNomeServizio.LetturaCI05Service;
                input.MetadatiServizio.Mittente = "LIQPENS";
                input.MetadatiServizio.Timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds; // Unix Epoch Time
                input.DatiRichiesta.Comarea = new TipoComareaPcics10();

                input.DatiRichiesta.Comarea.MatricolaOp = matricola;
                input.DatiRichiesta.Comarea.Sede = codiceSede.ToString().PadLeft(4, '0') + centroOperativo.ToString().PadLeft(2, '0');
                input.DatiRichiesta.Comarea.Cpgmdas = "PCICS10";
                input.DatiRichiesta.Comarea.Chiavi = new TipoChiaviPcics10();
                input.DatiRichiesta.Comarea.Chiavi.KeyFrom = Convert.ToString(nDomus);
                input.DatiRichiesta.Comarea.Chiavi.Filler = string.Empty;
                input.DatiRichiesta.Comarea.Chiavi.Filtro = string.Empty;
                input.DatiRichiesta.Comarea.Chiavi.Filtro01 = string.Empty;
                input.DatiRichiesta.Comarea.CheFare = "0";
                input.DatiRichiesta.Comarea.Continua = string.Empty;
                input.DatiRichiesta.Comarea.Prova = string.Empty;
                input.DatiRichiesta.Comarea.Altri = string.Empty;

                GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvAllegatiConvenzioni, Utility.MetodoServizio.letturaCI05, Utility.SOAPLogDirection.IN, nDomus.ToString(), guid);

                LetturaCI05(input, out dati, out errori);

                GestioneLogSoap.SalvaLogSoap(dati, Utility.Servizio.SrvAllegatiConvenzioni, Utility.MetodoServizio.letturaCI05, Utility.SOAPLogDirection.OUT, nDomus.ToString(), guid);

                if (!String.IsNullOrEmpty(errori) || (dati.DatiRisposta != null && dati.DatiRisposta.Messaggio != null && dati.DatiRisposta.Messaggio.Codice != 0))
                    dati = null;
                return true;
            }
            catch (Exception ex)
            {
                errori = "Errore tecnico durante il recupero degli stati esteri";
                string messaggio = Utility.GetMessageFromException(ex);
                string parametri = string.Format("Matricola: {0}; Codice sede: {1}; Centro operativo: {2}", matricola, codiceSede, centroOperativo);
                GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, ex.StackTrace);
                return false;
            }
        }

        private static void LetturaCI05(TipoModuloLetturaCI05Richiesta tipoModuloLetturaCI05Richiesta, out TipoModuloLetturaCI05Risposta risposta, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            risposta = null;
            string stackTrace = null;

            AllegatiConvenzioniServiceClient proxy = new AllegatiConvenzioniServiceClient();

            using (new MethodExecutionTracer())
            {
                try
                {
                    risposta = proxy.letturaCI05(tipoModuloLetturaCI05Richiesta);
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
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio AllegatiConvenzioni, method letturaCI05 | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio AllegatiConvenzioni, method letturaCI05 | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio AllegatiConvenzioni method letturaCI05 | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nella chiamata al servizio AllegatiConvenzioni method letturaCI05: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        long numeroDomanda = 0;
                        try
                        {
                            long.TryParse(tipoModuloLetturaCI05Richiesta.DatiRichiesta.Comarea.Chiavi.KeyFrom, out numeroDomanda);
                        }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, errori, null, stackTrace);
                        errori = "Errore tecnico durante il recupero degli stati esteri";
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        #endregion private methods
    }
}
