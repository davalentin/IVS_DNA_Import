using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.ServiceReferences.RichiestaBonus;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneRichiestaBonus
    {
        #region public members
        /// <summary>
        /// Ritorna la lista di tutti gli anni(nel range da data attivazione bonus a data attuale) contenenti l'esito di verifica bonus
        /// </summary>
        /// <param name="areaBooking"></param>
        /// <param name="numDomanda"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        public static bool GetAnniDirittoAlBonus(ref GestioneRichiestaBonus.AreaRichiestaBonus areaRichiestaBonus, string numDomanda, long idPensione)
        {
            string errori = "";
            try
            {
                DatiVerificaAnni[] output;
                GetAnniDirittoAlBonusFromWSBookingBonus(areaRichiestaBonus, numDomanda, out output, out errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    areaRichiestaBonus.MessaggioVideo = errori;
                    areaRichiestaBonus.Esito = TipoRitornoRichiestaBonus.Errore;
                    return false;
                }

                if(output.Count() > 0)
                {
                    List<GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> datiAnniRichiestaBonus = new List<GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus>();
                    foreach (DatiVerificaAnni annoVerifica in output)
                    {
                        GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus anno = new GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus();
                        anno.Anno = annoVerifica.Anno;
                        anno.CodiceEsitoMessaggio = annoVerifica.Codice_Esito_Messaggio;
                        anno.DescrizioneEsitoMessaggio = annoVerifica.Descrizione_Esito_messaggio;
                        anno.EsitoCalcoloBeneficio = annoVerifica.Esito_calcolo_beneficio;
                        anno.Prescrizione = annoVerifica.Prescrizione;
                        anno.IdPensione = idPensione;
                        datiAnniRichiestaBonus.Add(anno);
                    }
                    areaRichiestaBonus.DatiAnniRichiestaBonus = datiAnniRichiestaBonus.OrderByDescending(x => x.Anno).ToList();
                }
            }
            catch (Exception Ex)
            {
                areaRichiestaBonus.MessaggioVideo = "Errore tecnico durante la ricerca degli anni relativi al bonus.";
                areaRichiestaBonus.Esito = TipoRitornoRichiestaBonus.Errore;
                string messaggio = string.Format("Errore nella ricerca degli anni relativi al bonus: {0}", Utility.GetMessageFromException(Ex));
                string parametri = string.Format("Categoria: {0}; Sede: {1}; Certificato: {2}; TipoBonus: {3};", areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede,
                    areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus);
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Acquisizione delle prenotazioni
        /// </summary>
        /// <param name="areaBooking"></param>
        /// <param name="numDomanda"></param>
        /// <param name="matricolaOperatore"></param>
        /// <param name="sedeOperatore"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        public static bool GetPrenotazioneElaborazioni(ref GestioneRichiestaBonus.AreaRichiestaBonus areaRichiestaBonus, string matricolaOperatore, string sedeOperatore, long idPensione) 
        {
            string errori = "";
            try
            {
                DatiPrenotazione[] output;
                GetPrenotazioneElaborazioniFromWSBookingBonus(areaRichiestaBonus, matricolaOperatore, sedeOperatore, out output, out errori);
                if (!string.IsNullOrEmpty(errori))
                {
                    areaRichiestaBonus.MessaggioVideo = errori;
                    areaRichiestaBonus.Esito = TipoRitornoRichiestaBonus.Errore;
                    return false;
                }
                if (output.Count() > 0)
                {
                    List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> datiPrenotazioneElaborazioni = new List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni>();
                    foreach (DatiPrenotazione datiPrenotazione in output)
                    {
                        GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni prenotazioneElaborazione = new GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni();
                        prenotazioneElaborazione.AnnoRichiesto = datiPrenotazione.Anno_Richiesto;
                        prenotazioneElaborazione.DataInserimento = datiPrenotazione.Data_Inserimento;
                        prenotazioneElaborazione.DecorrenzaPresaInCarico = datiPrenotazione.Decorrenza_presa_in_carico;
                        prenotazioneElaborazione.EsitoCalcoloBeneficio = datiPrenotazione.Esito_calcolo_beneficio;
                        prenotazioneElaborazione.DescrizioneEsito = datiPrenotazione.Descrizione_Esito;
                        prenotazioneElaborazione.TipoElaborazione = datiPrenotazione.Tipo_Elaborazione;
                        prenotazioneElaborazione.IdPensione = idPensione;
                        datiPrenotazioneElaborazioni.Add(prenotazioneElaborazione);
                    }
                    areaRichiestaBonus.DatiPrenotazioneElaborazioni = datiPrenotazioneElaborazioni.OrderByDescending(x => x.AnnoRichiesto).ToList();
                }
            }
            catch (Exception Ex)
            {
                //errori = "Errore tecnico durante il recupero delle prenotazioni.";
                areaRichiestaBonus.MessaggioVideo = "Errore tecnico durante il recupero delle prenotazioni.";
                areaRichiestaBonus.Esito = TipoRitornoRichiestaBonus.Errore;
                string messaggio = string.Format("Errore nella prenotazione elaborazioni: {0}", Utility.GetMessageFromException(Ex));
                string parametri = string.Format("Categoria: {0}; Sede: {1}; Certificato: {2}; TipoBonus: {3};", areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede,
                   areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus);
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion public members

        #region private members
        /// <summary>
        /// Ritorna la lista di tutti gli anni(nel range da data attivazione bonus a data attuale) contenenti l'esito di verifica bonus
        /// </summary>
        /// <param name="areaBooking"></param>
        /// <param name="numDomanda"></param>
        /// <param name="response"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool GetAnniDirittoAlBonusFromWSBookingBonus(GestioneRichiestaBonus.AreaRichiestaBonus areaRichiestaBonus, string numDomanda, out DatiVerificaAnni[] output, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSBookingBonusServiceClient proxy = null;
            OperationResponse response = new OperationResponse();
            output = null;
            string stackTrace = null;           
            Guid guid = Guid.NewGuid();

            try
            {
                proxy = new WSBookingBonusServiceClient();
                GestioneLogSoap.SalvaLogSoap(areaRichiestaBonus, Utility.Servizio.SrvBooking, Utility.MetodoServizio.VerificaAnniDirittoAlBonus, Utility.SOAPLogDirection.IN, numDomanda, guid);
                output = proxy.VerificaAnniDirittoAlBonus(out response, areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede, areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus);
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Booking | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio Booking | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio Booking | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio Booking: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            finally
            {
                if ((!string.IsNullOrEmpty(errori) && erroreTecnico))
                {
                    string messaggio = errori;
                    errori = "Errore tecnico durante il recupero degli anni relativi al bonus.";
                    string parametri = string.Format("Categoria: {0}; Sede: {1}; Certificato: {2}; TipoBonus: {3};", areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede,
                        areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus);
                    GestioneLogGenerico.SalvaLogGenerico(long.Parse(numDomanda), MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                else if (response.ReturnValue == ReturnValues.Exception)
                {
                    string messaggio = response.Message;
                    errori = "Errore tecnico durante il recupero degli anni relativi al bonus.";
                    string parametri = string.Format("Categoria: {0}; Sede: {1}; Certificato: {2}; TipoBonus: {3};", areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede,
                        areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus);
                    GestioneLogGenerico.SalvaLogGenerico(long.Parse(numDomanda), MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                GestioneLogSoap.SalvaLogSoap(output, Utility.Servizio.SrvBooking, Utility.MetodoServizio.VerificaAnniDirittoAlBonus, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                Utility.CloseClient(proxy);
            }

            return true;
        }

        /// <summary>
        /// Acquisizione delle prenotazioni
        /// </summary>
        /// <param name="areaBooking"></param>
        /// <param name="matricolaOperatore"></param>
        /// <param name="sedeOperatore"></param>
        /// <param name="numDomanda"></param>
        /// <param name="response"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool GetPrenotazioneElaborazioniFromWSBookingBonus(GestioneRichiestaBonus.AreaRichiestaBonus areaRichiestaBonus, string matricolaOperatore, string sedeOperatore, out DatiPrenotazione[] output, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSBookingBonusServiceClient proxy = null;
            OperationResponse response = new OperationResponse();
            output = null;
            string stackTrace = null;
            Guid guid = Guid.NewGuid();

            try
            {
                proxy = new WSBookingBonusServiceClient();
                GestioneLogSoap.SalvaLogSoap(areaRichiestaBonus, Utility.Servizio.SrvBooking, Utility.MetodoServizio.PrenotazioneElaborazioni, Utility.SOAPLogDirection.IN, areaRichiestaBonus.NumDomanda, guid);
                output = proxy.PrenotazioneElaborazioni(out response, matricolaOperatore, sedeOperatore, areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede, areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus, areaRichiestaBonus.Anni, areaRichiestaBonus.NumDomanda);
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Booking | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio Booking | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio Booking | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio Booking: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico durante la prenotazione elaborazioni.";
                    string parametri = string.Format("Categoria: {0}; Sede: {1}; Certificato: {2}; TipoBonus: {3}; Anni: {4}", areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede,
                        areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus, areaRichiestaBonus.Anni);
                    GestioneLogGenerico.SalvaLogGenerico(long.Parse(areaRichiestaBonus.NumDomanda), MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                else if (response.ReturnValue == ReturnValues.Exception)
                {
                    string messaggio = response.Message;
                    errori = "Errore tecnico durante la prenotazione elaborazioni.";
                    string parametri = string.Format("Categoria: {0}; Sede: {1}; Certificato: {2}; TipoBonus: {3}; Anni: {4}", areaRichiestaBonus.Categoria, areaRichiestaBonus.Sede,
                        areaRichiestaBonus.Certificato, areaRichiestaBonus.TipoBonus, areaRichiestaBonus.Anni);
                    GestioneLogGenerico.SalvaLogGenerico(long.Parse(areaRichiestaBonus.NumDomanda), MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                GestioneLogSoap.SalvaLogSoap(output, Utility.Servizio.SrvBooking, Utility.MetodoServizio.PrenotazioneElaborazioni, Utility.SOAPLogDirection.OUT, areaRichiestaBonus.NumDomanda, guid);
                Utility.CloseClient(proxy);
            }

            return true;
        }
        #endregion private members

        #region nested class
        public class AreaRichiestaBonus
        {
            public AreaRichiestaBonus()
            {
            }

            #region private properties
            private string _Categoria;
            private string _Sede;
            private string _Certificato;
            private string _TipoBonus;
            private string _Anni;
            private TipoRitornoRichiestaBonus _Esito;
            private string _MessaggioVideo;
            private List<BLCommon.GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> _DatiAnniRichiestaBonus;
            private List<BLCommon.GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> _DatiPrenotazioneElaborazioni;
            private string _NumDomanda;
            private int _AnnoInizioBonus;
            private bool _IsDataFromDB;
            #endregion private properties

            #region public properties
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            public string Certificato { get { return _Certificato; } set { _Certificato = value; } }
            public string TipoBonus { get { return _TipoBonus; } set { _TipoBonus = value; } }
            public string Anni { get { return _Anni; } set { _Anni = value; } }
            public TipoRitornoRichiestaBonus Esito { get { return _Esito; } set { _Esito = value; } }
            public string MessaggioVideo { get { return _MessaggioVideo; } set { _MessaggioVideo = value; } }
            public List<BLCommon.GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> DatiAnniRichiestaBonus { get { return _DatiAnniRichiestaBonus; } set { _DatiAnniRichiestaBonus = value; } }
            public List<BLCommon.GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> DatiPrenotazioneElaborazioni { get { return _DatiPrenotazioneElaborazioni; } set { _DatiPrenotazioneElaborazioni = value; } }
            public string NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }
            public int AnnoInizioBonus { get { return _AnnoInizioBonus; } set { _AnnoInizioBonus = value; } }
            public bool IsDataFromDB { get { return _IsDataFromDB; } set { _IsDataFromDB = value; } }
            #endregion public properties
        }

        public enum TipoRitornoRichiestaBonus
        {
            NessunErrore,
            Errore
        };

        #endregion nested class
    }
}
