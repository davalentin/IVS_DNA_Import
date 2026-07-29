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
using INPS.Pensioni.Liquidazione.ServiceReferences.VerTitolIBAN;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneVerTitolIBAN
    {
        #region public members
        /// <summary>
        /// Ritorna lo stato e la nota relativi alla titolarità della coppia IBAN + CF
        /// </summary>
        /// <param name="areaTitolarita"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        public static bool GetStatoTitolarita(ref GestioneVerTitolIBAN.AreaTitolarita areaTitolarita, string matricola, string sede, out string errori)
        {
            errori = "";
            try
            {
                VerificaTitolaritaInput input = new VerificaTitolaritaInput();
                IdentChiamante identChiamante = new IdentChiamante();

                identChiamante.sistema = ConfigurationManager.AppSettings["Sistema"];
                identChiamante.matricola = matricola;
                identChiamante.sedeUtente = sede;

                input.codiceFiscale = areaTitolarita.CodiceFiscale;
                //ENG - Il servizio accetta l'IBAN solo in maiuscolo
                if (!String.IsNullOrEmpty(areaTitolarita.CodiceIban))
                {
                    input.codiceIban = areaTitolarita.CodiceIban.Trim().ToUpperInvariant();
                }
                input.identChiamante = identChiamante;
                VerificaTitolaritaOutput output = new VerificaTitolaritaOutput();
                if (!GetStatoTitolaritaFromWSVerTitolIBAN(input, areaTitolarita.NumDomanda, out output, out errori))
                    return false;

                areaTitolarita.Status = output.status;
                areaTitolarita.Note = output.note;
            }
            catch (Exception Ex)
            {
                errori = "Errore tecnico durante la ricerca dello stato della titolarità";
                string messaggio = string.Format("Errore nella ricerca dello stato della titolarità: {0}", Utility.GetMessageFromException(Ex));
                string parametri = string.Format("IBAN: {0}; CF: {1}", areaTitolarita.CodiceIban, areaTitolarita.CodiceFiscale);
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion public members

        #region private members
        /// <summary>
        /// Ritorna lo stato della titolarità della coppia IBAN + CF
        /// </summary>
        /// <param name="input"></param>
        /// <param name="numDomanda"></param>
        /// <param name="output"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool GetStatoTitolaritaFromWSVerTitolIBAN(VerificaTitolaritaInput input, string numDomanda, out VerificaTitolaritaOutput output, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WsVerTitolIBANClient proxy = null;
            string stackTrace = null;
            output = null;
            Guid guid = Guid.NewGuid();

            try
            {
                proxy = new WsVerTitolIBANClient();
                GestioneLogSoap.SalvaLogSoap(input, Utility.Servizio.SrvVerTitolIBAN, Utility.MetodoServizio.VerificaTitolarita, Utility.SOAPLogDirection.IN, numDomanda, guid);
                output = proxy.verificaTitolaritaBPens(input);
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio VerTitolIBAN | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio VerTitolIBAN | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio VerTitolIBAN | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio VerTitolIBAN: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico durante il recupero dello stato della titolarità.";
                    string parametri = string.Format("IBAN: {0}; CF: {1}", input.codiceIban, input.codiceFiscale);
                    GestioneLogGenerico.SalvaLogGenerico(long.Parse(numDomanda), MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                GestioneLogSoap.SalvaLogSoap(output, Utility.Servizio.SrvVerTitolIBAN, Utility.MetodoServizio.VerificaTitolarita, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                Utility.CloseClient(proxy);
            }

            return true;
        }
        #endregion private members

        #region nested class
        public class AreaTitolarita
        {
            public AreaTitolarita()
            {
            }

            #region private properties
            private string _NumDomanda;
            private string _CodiceIban;
            private string _CodiceFiscale;
            private string _Status;
            private string _Note;
            #endregion private properties

            #region public properties
            public string NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }
            public string CodiceIban { get { return _CodiceIban; } set { _CodiceIban = value; } }
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public string Status { get { return _Status; } set { _Status = value; } }
            public string Note { get { return _Note; } set { _Note = value; } }
            #endregion public properties

        }
        #endregion nested class
    }
}
