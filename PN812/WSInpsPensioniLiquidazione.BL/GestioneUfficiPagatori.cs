using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.VarUfficioPag;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.ServiceReferences.UfficiPagatoriNew;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneUfficiPagatori
    {
        #region public members
        public static bool ValidaUfficioPagatorePerEstero(string modalitaPagamento, string iban, string nazione, string bic, string codCatastale, out string errori)
        {
            errori = string.Empty;

            if (string.IsNullOrEmpty(codCatastale))
            {
                List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
                GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);

                if (listaStatiEsteri != null)
                {
                    if (nazione.Equals("ANTILLE FRANCESI (MARTINICA - GUADALUPA)"))
                        nazione = "MARTINICA";
                    GestioneDecodifica.StatoEstero statoEstero = listaStatiEsteri.Find(x => x.Descrizione == nazione);
                    if (statoEstero != null && !string.IsNullOrEmpty(statoEstero.CodCatastale))
                        return ValidateUfficioPagatoreWithValidaUfficioPagatore(modalitaPagamento, iban, string.Empty, statoEstero.CodCatastale, bic, string.Empty, string.Empty, string.Empty, out errori);
                }
            }
            else
                return ValidateUfficioPagatoreWithValidaUfficioPagatore(modalitaPagamento, iban, string.Empty, codCatastale, bic, string.Empty, string.Empty, string.Empty, out errori);

            errori = "Stato estero non riconosciuto";
            return false;
        }

        public static bool ValidaUfficioPagatorePerItalia(string modalitaPagamento, string iban, string bic, string abi, string frazionario, string libretto, out string errori)
        {
            errori = string.Empty;
            return ValidateUfficioPagatoreWithValidaUfficioPagatore(modalitaPagamento, iban, "Italia", string.Empty, bic, abi, frazionario, libretto, out errori);
        }

        public static bool GetStatiEsteri(out List<GestioneAreaPagamento.DatiStatoEstero> elencoStatiEsteri, out string errori)
        {
            elencoStatiEsteri = null;
            errori = string.Empty;

            try
            {
                if (!GetListeStatiEsteriFromSrvUfficiPagatori(out elencoStatiEsteri, out errori))
                    return false;
            }
            catch (Exception Ex)
            {
                string messaggio = string.Format("Errore nella ricerca degli uffici pagatori: {0}", Utility.GetMessageFromException(Ex));
                errori = "Errore tecnico durante il recupero della lista di stati esteri.";
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool ValidaUfficioPagatore(GestionePagamento.DatiPagamento datiPagamento, out string errori)
        {
            errori = "Modalità di pagamento errata.";
            List<GestioneUfficiPagatori.AreaUfficioPagatore> elencoUfficiPagatoriBL = null;

            if (datiPagamento == null)
                return true;

            if (datiPagamento.TipoPagamento == 'B')
            {
                if (datiPagamento.ABI == 07601 || (datiPagamento.ABI == 36081 && datiPagamento.CAB == 05138))
                {
                    datiPagamento = null;
                    return false;
                }
            }

            if (datiPagamento.TipoPagamento == 'P')
            {
                if (datiPagamento.ABI != 07601 && !(datiPagamento.ABI == 36081 && datiPagamento.CAB == 05138))
                {
                    datiPagamento = null;
                    return false;
                }

                if (datiPagamento.ModalitaPagamento == 'X')
                {
                    if (datiPagamento.CAB != 0099999)
                    {
                        datiPagamento = null;
                        return false;
                    }
                }
            }

            // Abi_Cab
            // Cassa
            if ((datiPagamento.TipoPagamento == 'B' && datiPagamento.ModalitaPagamento == 'S') || (datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'X') ||
                (datiPagamento.TipoPagamento == 'C' && datiPagamento.ModalitaPagamento == 'P'))
            {
                if (datiPagamento.ABI.GetValueOrDefault() == 0 || datiPagamento.CAB.GetValueOrDefault() == 0)
                {
                    datiPagamento = null;
                    return false;
                }

                if (datiPagamento.TipoPagamento == 'C' && datiPagamento.ModalitaPagamento == 'P')
                {
                    if (datiPagamento.ABI != 99999)
                    {
                        datiPagamento = null;
                        return false;
                    }
                }

                //Banca d'Italia
                if (datiPagamento.ABI == 1000 && datiPagamento.CAB == 03203)
                {
                    datiPagamento.CAB = 6603203;
                }
                if (datiPagamento.TipoPagamento == 'B')
                {
                    if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(datiPagamento.ModalitaPagamento.ToString(), datiPagamento.IBAN, datiPagamento.BIC, datiPagamento.ABI.ToString(),
                        datiPagamento.CAB.ToString(), datiPagamento.Libretto, out errori))
                    {
                        datiPagamento = null;
                        return false;
                    }
                }
                else if (datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'X')
                {
                    if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia("S", datiPagamento.IBAN, datiPagamento.BIC, datiPagamento.ABI.ToString(), datiPagamento.Frazionario.ToString(),
                        datiPagamento.Libretto, out errori))
                    {
                        datiPagamento = null;
                        return false;
                    }
                }

                if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(datiPagamento.ABI.GetValueOrDefault(), datiPagamento.CAB.GetValueOrDefault(), out elencoUfficiPagatoriBL, out errori))
                {
                    datiPagamento = null;
                    return false;
                }

            }
            // Abi_Frazionario
            else if ((datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'S') ||
                (datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'L' && !string.IsNullOrEmpty(datiPagamento.Libretto)))
            {
                if (datiPagamento.ABI.GetValueOrDefault() == 0 || datiPagamento.Frazionario.GetValueOrDefault() == 0)
                {
                    datiPagamento = null;
                    return false;
                }
                if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(datiPagamento.ModalitaPagamento.ToString(), datiPagamento.IBAN, datiPagamento.BIC, datiPagamento.ABI.ToString(),
                        datiPagamento.Frazionario.ToString(), datiPagamento.Libretto, out errori))
                {
                    datiPagamento = null;
                    return false;
                }
                if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(datiPagamento.ABI.GetValueOrDefault(), datiPagamento.Frazionario.GetValueOrDefault(), out elencoUfficiPagatoriBL, out errori))
                {
                    datiPagamento = null;
                    return false;
                }

            }
            // Iban_Banca
            else if ((datiPagamento.TipoPagamento == 'B' && datiPagamento.ModalitaPagamento == 'C') || (datiPagamento.TipoPagamento == 'B' && datiPagamento.ModalitaPagamento == 'L') ||
                (datiPagamento.TipoPagamento == 'B' && datiPagamento.ModalitaPagamento == 'K'))
            {
                if (String.IsNullOrEmpty(datiPagamento.IBAN) || datiPagamento.IBAN.Length < 27)
                {
                    datiPagamento = null;
                    return false;
                }
                if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(datiPagamento.ModalitaPagamento.ToString(), datiPagamento.IBAN, datiPagamento.BIC, datiPagamento.ABI.ToString(),
                    datiPagamento.CAB.ToString(), datiPagamento.Libretto, out errori))
                {
                    datiPagamento = null;
                    return false;
                }
                if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(datiPagamento.ABI.GetValueOrDefault(), datiPagamento.CAB.GetValueOrDefault(), out elencoUfficiPagatoriBL, out errori))
                {
                    datiPagamento = null;
                    return false;
                }

            }
            // Iban_Posta
            else if ((datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'C') ||
                (datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'L' && !string.IsNullOrEmpty(datiPagamento.IBAN)) ||
                (datiPagamento.TipoPagamento == 'P' && datiPagamento.ModalitaPagamento == 'K'))
            {
                if (String.IsNullOrEmpty(datiPagamento.IBAN) || datiPagamento.IBAN.Length < 27 || 
                    (datiPagamento.IBAN.ToUpperInvariant().Substring(5, 5) == "07601" && datiPagamento.Frazionario.GetValueOrDefault() == 0) ||
                    (datiPagamento.IBAN.ToUpperInvariant().Substring(5, 5) != "07601" && !(datiPagamento.IBAN.ToUpperInvariant().Substring(5, 5) == "36081" && datiPagamento.IBAN.ToUpperInvariant().Substring(10, 5) == "05138")) ||
                    (datiPagamento.IBAN.ToUpperInvariant().Substring(10, 5) == "03384" && (datiPagamento.ModalitaPagamento == 'C' || datiPagamento.ModalitaPagamento == 'K')) ||
                    (datiPagamento.IBAN.ToUpperInvariant().Substring(10, 5) != "03384" && datiPagamento.ModalitaPagamento == 'L') ||
                    (datiPagamento.IBAN.ToUpperInvariant().Substring(10, 5) == "05138" && (datiPagamento.ModalitaPagamento == 'C' || datiPagamento.ModalitaPagamento == 'L')) ||
                    (datiPagamento.IBAN.ToUpperInvariant().Substring(10, 5) != "05138" && datiPagamento.ModalitaPagamento == 'K'))
                {
                    datiPagamento = null;
                    return false;
                }

                int? cab_frazionario = datiPagamento.IBAN.ToUpperInvariant().Substring(5, 5) == "07601" ? datiPagamento.Frazionario : datiPagamento.CAB;
                if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(datiPagamento.ModalitaPagamento.ToString(), datiPagamento.IBAN, datiPagamento.BIC, datiPagamento.IBAN.ToUpperInvariant().Substring(5, 5),
                    cab_frazionario.GetValueOrDefault().ToString(), datiPagamento.Libretto, out errori))
                {
                    datiPagamento = null;
                    return false;
                }
                if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(datiPagamento.ABI.GetValueOrDefault(), cab_frazionario.GetValueOrDefault(), out elencoUfficiPagatoriBL, out errori))
                {
                    datiPagamento = null;
                    return false;
                }
            }
            // Estero
            else if ((datiPagamento.TipoPagamento == 'E' && datiPagamento.ModalitaPagamento == 'C') || (datiPagamento.TipoPagamento == 'E' && datiPagamento.ModalitaPagamento == 'S') ||
                (datiPagamento.TipoPagamento == 'E' && datiPagamento.ModalitaPagamento == 'A'))
            {
                if (String.IsNullOrEmpty(datiPagamento.CittaUfficioPagatore))
                {
                    datiPagamento = null;
                    return false;
                }
                if (!string.IsNullOrEmpty(datiPagamento.IBAN))
                {
                    if (!GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(datiPagamento.ModalitaPagamento.ToString(), datiPagamento.IBAN, datiPagamento.CittaUfficioPagatore, datiPagamento.BIC,
                        datiPagamento.CodCatastaleEstero, out errori))
                    {
                        datiPagamento = null;
                        return false;
                    }
                }
                if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(datiPagamento.ABI.GetValueOrDefault(), datiPagamento.CAB.GetValueOrDefault(), out elencoUfficiPagatoriBL, out errori))
                {
                    datiPagamento = null;
                    return false;
                }
            }

            if (elencoUfficiPagatoriBL == null || elencoUfficiPagatoriBL.Count == 0)
            {
                datiPagamento = null;
                return false;
            }

            errori = string.Empty;
            return true;
        }

        /// <summary>
        /// Recupera l'elenco di uffici pagatori dal nuovo servizio UfficiPagatori
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="cab"></param>
        /// <param name="elencoUfficiPagatori"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        public static bool GetUfficiPagatoriNew(int abi, int cab, out List<AreaUfficioPagatore> elencoUfficiPagatori, out string errori)
        {
            elencoUfficiPagatori = null;
            errori = "";
            try
            {
                AreaUfficioPagatore ufficioPagatore = null;
                if (!GetUfficioPagatoreFromSrvUfficiPagatori(abi, cab, out ufficioPagatore, out errori))
                    return false;
                if (ufficioPagatore != null)
                {
                    elencoUfficiPagatori = new List<AreaUfficioPagatore>();
                    elencoUfficiPagatori.Add(ufficioPagatore);
                }
            }
            catch (Exception Ex)
            {
                errori = "Errore tecnico durante la ricerca dell'ufficio pagatore";
                string messaggio = string.Format("Errore nella ricerca degli uffici pagatori: {0}", Utility.GetMessageFromException(Ex));
                string parametri = string.Format("ABI: {0:00000}; CAB: {1:0000000}", abi, cab);
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        #endregion public members

        #region internal members
        internal static bool GetListaCassaSedeNew(out List<AreaUfficioPagatore> elencoUfficiPagatori, out string errori)
        {
            elencoUfficiPagatori = null;
            errori = "";
            try
            {
                if (!GetListaUfficioPagatorePerCassaSede(out elencoUfficiPagatori, out errori))
                    return false;
            }
            catch (Exception Ex)
            {
                string messaggio = string.Format("Errore nella chiamata al servizio UfficiPagatoriNew: {0}", Utility.GetMessageFromException(Ex));
                errori = "Errore tecnico durante il recupero della lista cassa sede.";
                GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion internal members

        #region private members
        private static bool ValidateUfficioPagatoreFromSrvVarUfficioPag(string iban, string nazione, string codCatastale, string bic, string frazionario, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            VariazioneUfficioPagatoreClient proxy = new VariazioneUfficioPagatoreClient();
            string stackTrace = null;
            try
            {
                ValidazioneCoordinateBancarieDTORequest richiesta = new ValidazioneCoordinateBancarieDTORequest();
                richiesta.CoordinateBancarie = new CoordinateBancarieType();
                richiesta.CoordinateBancarie.IBAN = iban.ToUpperInvariant();
                richiesta.CoordinateBancarie.BIC = bic.ToUpperInvariant();
                if (!string.IsNullOrEmpty(frazionario))
                {
                    richiesta.CoordinateBancarie.ABI = iban.ToUpperInvariant().Substring(5, 5);
                    richiesta.CoordinateBancarie.CAB = frazionario.PadLeft(7, '0');
                }
                else
                {
                    richiesta.CoordinateBancarie.ABI = string.Empty;
                    richiesta.CoordinateBancarie.CAB = string.Empty;
                }
                richiesta.Nazione = new NazioneType();
                richiesta.Nazione.CodiceISO = string.Empty;
                if (!string.IsNullOrEmpty(nazione))
                {
                    richiesta.Nazione.Denominazione = nazione;
                    richiesta.Nazione.CodiceCatastale = string.Empty;
                }
                else if (!string.IsNullOrEmpty(codCatastale))
                {
                    richiesta.Nazione.CodiceCatastale = codCatastale;
                    richiesta.Nazione.Denominazione = string.Empty;
                }
                ValidazioneCoordinateBancarieDTOResponse risposta = proxy.ValidazioneCoordinateBancarie(richiesta);
                if (risposta != null && risposta.Esito != null)
                {
                    if (risposta.Esito.Risultato == "OK")
                        return true;
                    else
                    {
                        errori = "Errore in fase di validazione del metodo di pagamento: " + risposta.Esito.Descrizione;
                        return false;
                    }
                }
                else
                {
                    errori = "Errore in fase di validazione del metodo di pagamento: nessuna risposta rilevata";
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio VarUfficioPag | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio UfficiPagatori | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio VarUfficioPag | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio VarUfficioPag: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico in fase di validazione del metodo di pagamento";
                    string parametri = string.Format("IBAN: {0}; Nazione: {1}; Codice catastale: {2}; BIC: {3}; Frazionario: {4}", iban, nazione, codCatastale, bic, frazionario);
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                }
                Utility.CloseClient(proxy);
            }
        }

        /// <summary>
        /// Valida l'ufficio pagatore tramite il web method ValidaUfficioPagatore
        /// </summary>
        /// <param name="modalitaPagamento">
        ///     C = conto corrente
        ///     L = Libretto
        ///     K = Carta prepagata
        ///     A = Assegno
        ///     S = Sportello
        /// </param>
        /// <param name="iban"></param>
        /// <param name="nazione"></param>
        /// <param name="codCatastale"></param>
        /// <param name="bic"></param>
        /// <param name="cab_Frazionario"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool ValidateUfficioPagatoreWithValidaUfficioPagatore(string modalitaPagamento, string iban, string nazione, string codCatastale, string bic, string abi, string cab_Frazionario,
            string libretto, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            VariazioneUfficioPagatoreClient proxy = new VariazioneUfficioPagatoreClient();
            string stackTrace = null;
            try
            {
                ValidaUfficioPagatoreDTORequest richiesta = new ValidaUfficioPagatoreDTORequest();
                richiesta.CoordinateBancarie = new CoordinateBancarieType();
                if (!string.IsNullOrEmpty(iban))
                    richiesta.CoordinateBancarie.IBAN = iban.ToUpperInvariant();
                if (!string.IsNullOrEmpty(bic))
                    richiesta.CoordinateBancarie.BIC = bic.ToUpperInvariant();
                if (!string.IsNullOrEmpty(abi))
                    richiesta.CoordinateBancarie.ABI = abi.PadLeft(5, '0');
                if (!string.IsNullOrEmpty(cab_Frazionario))
                    richiesta.CoordinateBancarie.CAB = cab_Frazionario.PadLeft(7, '0');
                if (!string.IsNullOrEmpty(libretto) && !string.IsNullOrEmpty(libretto.Trim()))
                    richiesta.NumeroLibretto = libretto;

                richiesta.Nazione = new NazioneType();
                if (!string.IsNullOrEmpty(nazione))
                    richiesta.Nazione.Denominazione = nazione;
                else if (!string.IsNullOrEmpty(codCatastale))
                    richiesta.Nazione.CodiceCatastale = codCatastale;

                richiesta.ModalitaPagamento = modalitaPagamento;

                ValidaUfficioPagatoreDTOResponse risposta = proxy.ValidaUfficioPagatore(richiesta);
                if (risposta != null && risposta.Esito != null)
                {
                    if (risposta.Esito.Risultato == "OK")
                        return true;
                    else
                    {
                        errori = "Errore in fase di validazione del metodo di pagamento: " + risposta.Esito.Descrizione;
                        return false;
                    }
                }
                else
                {
                    errori = "Errore in fase di validazione del metodo di pagamento: nessuna risposta rilevata";
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio VarUfficioPag | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio UfficiPagatori | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio VarUfficioPag | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio VarUfficioPag: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico in fase di validazione del metodo di pagamento";
                    string parametri = string.Format("Modalità pagamento: {0}; IBAN: {1}; Nazione: {2}; Codice catastale: {3}; BIC: {4}; ABI: {5}; CAB/Frazionario: {6}; Libretto: {7}", 
                        modalitaPagamento, iban, nazione, codCatastale, bic, abi, cab_Frazionario, libretto);
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                }
                Utility.CloseClient(proxy);
            }
        }

        /// <summary>
        /// Recupera la lista di Stati Esteri per la modalità di pagamento estero dal nuovo servizio UfficiPagatori
        /// </summary>
        /// <param name="elencoStatiEsteri"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool GetListeStatiEsteriFromSrvUfficiPagatori(out List<GestioneAreaPagamento.DatiStatoEstero> elencoStatiEsteri, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            elencoStatiEsteri = null;
            UffPagatoriClient proxy = null;
            string stackTrace = null;

            try
            {
                proxy = new UffPagatoriClient();

                AUCC[] listaStatiEsteri = proxy.LetturaEsteroCat();

                if (listaStatiEsteri != null && listaStatiEsteri.Count() > 0)
                {
                    elencoStatiEsteri = new List<GestioneAreaPagamento.DatiStatoEstero>();

                    foreach (AUCC stato in listaStatiEsteri)
                    {
                        if (stato.Esito != null && stato.Esito.CodErrore != "0")
                        {
                            elencoStatiEsteri = null;
                            errori = stato.Esito.DescErrore;
                            return false;
                        }

                        if (stato.CAB.StartsWith("44") || stato.CAB.StartsWith("77"))
                        {
                            GestioneAreaPagamento.DatiStatoEstero statoEstero = new GestioneAreaPagamento.DatiStatoEstero();
                            statoEstero.NomeStato = stato.DENOM;
                            statoEstero.ABI = stato.ABI;
                            statoEstero.CAB = stato.CAB;
                            statoEstero.CodCatastale = stato.CODCAT;

                            elencoStatiEsteri.Add(statoEstero);
                        }
                    }
                }

                if (elencoStatiEsteri != null && elencoStatiEsteri.Count > 0)
                    elencoStatiEsteri.Sort((x, y) => string.Compare(x.NomeStato, y.NomeStato, true, System.Globalization.CultureInfo.CurrentUICulture));
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio UfficiPagatoriNew: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico durante il recupero della lista di stati esteri.";
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                }
                Utility.CloseClient(proxy);
            }

            return true;
        }

        /// <summary>
        /// Recupera l'elenco di uffici pagatori dal nuovo servizio UfficiPagatori
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="cab"></param>
        /// <param name="ufficio"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool GetUfficioPagatoreFromSrvUfficiPagatori(int abi, int cab, out AreaUfficioPagatore ufficio, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            ufficio = null;
            UffPagatoriClient proxy = null;
            string stackTrace = null;

            try
            {
                proxy = new UffPagatoriClient();

                if (abi == 1000 && cab == 3203)
                    cab = 6603203;

                AUPN[] elencoUffici = proxy.ElencoNuoveCoordinate(abi.ToString().PadLeft(5, '0'), cab.ToString().PadLeft(7, '0'));

                if (elencoUffici != null && elencoUffici.Count() > 0)
                {
                    AUPN uff = elencoUffici.First();

                    if (uff.Esito != null && uff.Esito.CodErrore != "0")
                    {
                        errori = uff.Esito.DescErrore;
                        return false;
                    }

                    ufficio = new AreaUfficioPagatore();
                    ufficio.Nome = !string.IsNullOrEmpty(uff.RAGSO) ? uff.RAGSO.Trim() : string.Empty;
                    ufficio.Agenzia = !string.IsNullOrEmpty(uff.DESCR) ? uff.DESCR.Trim() : string.Empty;
                    ufficio.Cap = !string.IsNullOrEmpty(uff.CAP) ? uff.CAP.Trim() : string.Empty;
                    ufficio.Citta = !string.IsNullOrEmpty(uff.COMUNE) ? uff.COMUNE.Trim() : string.Empty;
                    ufficio.Indirizzo = !string.IsNullOrEmpty(uff.INDIR) ? uff.INDIR.Trim() : string.Empty;
                    ufficio.CodiceMeccanizzazione = !string.IsNullOrEmpty(uff.CODME) ? uff.CODME.Trim() : string.Empty;

                    if ((cab.ToString().StartsWith("44") || cab.ToString().StartsWith("77")) && cab.ToString().Length >= 7)
                    {
                        ufficio.Citta = ufficio.Agenzia.Contains('-') ? ufficio.Agenzia.Substring(ufficio.Agenzia.IndexOf('-') + 1) : ufficio.Citta;
                    }

                    ufficio.Abi = Utility.StringToNullableInt(uff.ABI).GetValueOrDefault();
                    if (ufficio.Abi == 07601)
                        ufficio.Frazionario = Utility.StringToNullableInt(uff.CAB).GetValueOrDefault();
                    else
                        ufficio.Cab = Utility.StringToNullableInt(uff.CAB).GetValueOrDefault();
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio UfficiPagatoriNew: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico durante il recupero dell'ufficio pagatore.";
                    string parametri = string.Format("ABI: {0:00000}; CAB: {1:0000000}", abi, cab);
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                Utility.CloseClient(proxy);
            }

            return true;
        }

        /// <summary>
        /// Recupera l'elenco di uffici pagatori per cassa sede dal nuovo servizio UfficiPagatori
        /// Se il cab è vuoto viene restituita la lista con tutti gli uffici pagatori per cassa sede
        /// </summary>
        /// <param name="cab"></param>
        /// <param name="ufficio"></param>
        /// <param name="errori"></param>
        /// <returns></returns>
        private static bool GetListaUfficioPagatorePerCassaSede(out List<AreaUfficioPagatore> elencoUfficiPagatori, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            elencoUfficiPagatori = null;
            UffPagatoriClient proxy = null;
            string stackTrace = null;

            try
            {
                proxy = new UffPagatoriClient();

                AUPN[] elencoUffici = proxy.ElencoCassaSede("I", string.Empty);

                if (elencoUffici != null && elencoUffici.Count() > 0)
                {
                    foreach (AUPN ufficio in elencoUffici)
                    {
                        if (ufficio.Esito != null && ufficio.Esito.CodErrore != "0")
                        {
                            elencoUfficiPagatori = null;
                            errori = ufficio.Esito.DescErrore;
                            return false;
                        }

                        AreaUfficioPagatore ufficioPagatore = new AreaUfficioPagatore();
                        ufficioPagatore.Nome = !string.IsNullOrEmpty(ufficio.RAGSO) ? ufficio.RAGSO.Trim() : null;
                        ufficioPagatore.Agenzia = !string.IsNullOrEmpty(ufficio.DESCR) ? ufficio.DESCR.Trim() : null;
                        ufficioPagatore.Cap = !string.IsNullOrEmpty(ufficio.CAP) ? ufficio.CAP.Trim() : null;
                        ufficioPagatore.Citta = !string.IsNullOrEmpty(ufficio.COMUNE) ? ufficio.COMUNE.Trim() : null;
                        ufficioPagatore.Indirizzo = !string.IsNullOrEmpty(ufficio.INDIR) ? ufficio.INDIR.Trim() : null;
                        ufficioPagatore.CodiceMeccanizzazione = !string.IsNullOrEmpty(ufficio.CODME) ? ufficio.CODME.Trim() : null;
                        ufficioPagatore.Abi = !string.IsNullOrEmpty(ufficio.ABI) ? int.Parse(ufficio.ABI.Trim()) : 0;
                        if (ufficioPagatore.Abi == 07601 || (ufficio.ABI == "36081" && ufficio.CAB == "05138"))
                            ufficioPagatore.Frazionario = !string.IsNullOrEmpty(ufficio.CAB) ? int.Parse(ufficio.CAB.Trim()) : 0;
                        else
                            ufficioPagatore.Cab = !string.IsNullOrEmpty(ufficio.CAB) ? int.Parse(ufficio.CAB.Trim()) : 0;

                        if (elencoUfficiPagatori == null)
                            elencoUfficiPagatori = new List<AreaUfficioPagatore>();
                        elencoUfficiPagatori.Add(ufficioPagatore);
                    }
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
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.WriteError(errori);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio UfficiPagatoriNew | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                INPS.DNA.Logging.Logger.LogException(Ex);
                erroreTecnico = true;
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio UfficiPagatoriNew: {0}", Utility.GetMessageFromException(Ex));
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
                    errori = "Errore tecnico durante il recupero della lista cassa sede.";
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                }
                Utility.CloseClient(proxy);
            }

            return true;
        }

        #endregion private members

        #region nested class
        public class RichiestaUfficiPagatori
        {
            public RichiestaUfficiPagatori()
            { }
            public RichiestaUfficiPagatori(string tipoRichiesta, int abi, int cab, string cap, string comune, string provincia, string descrizione)
            {
                this._TipoRichiesta = tipoRichiesta;
                this._Abi = abi;
                this._Cab = cab;
                this._Cap = cap;
                this._Comune = comune;
                this._Provincia = provincia;
                this._Descrizione = descrizione;
            }
            #region private properties
            private string _TipoRichiesta;
            private int _Abi;
            private int _Cab;
            private string _Cap;
            private string _Comune;
            private string _Provincia;
            private string _Descrizione;
            #endregion private properties

            #region public properties
            public string TipoRichiesta { get { return _TipoRichiesta; } set { _TipoRichiesta = value; } }
            public int Abi { get { return _Abi; } set { _Abi = value; } }
            public int Cab { get { return _Cab; } set { _Cab = value; } }
            public string Cap { get { return _Cap; } set { _Cap = value; } }
            public string Comune { get { return _Comune; } set { _Comune = value; } }
            public string Provincia { get { return _Provincia; } set { _Provincia = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties
        }
        public class AreaUfficioPagatore
        {
            public AreaUfficioPagatore(string nome, string agenzia, string cap, string citta, string indirizzo, string codiceMeccanizzazione, int abi, int cab, int frazionario)
            {
                this._Nome = nome;
                this._Agenzia = agenzia;
                this._Cap = cap;
                this._Citta = citta;
                this._Indirizzo = indirizzo;
                this._CodiceMeccanizzazione = codiceMeccanizzazione;
                this._Abi = abi;
                this._Cab = cab;
                this._Frazionario = frazionario;
            }

            public AreaUfficioPagatore()
            {
            }

            #region private properties

            private string _Nome;
            private string _Agenzia;
            private string _Cap;
            private string _Citta;
            private string _Indirizzo;
            private string _CodiceMeccanizzazione;
            private int _Abi;
            private int _Cab;
            private int _Frazionario;
            #endregion private properties

            #region public properties
            public string Nome { get { return _Nome; } set { _Nome = value; } }
            public string Agenzia { get { return _Agenzia; } set { _Agenzia = value; } }
            public string Cap { get { return _Cap; } set { _Cap = value; } }
            public string Citta { get { return _Citta; } set { _Citta = value; } }
            public string Indirizzo { get { return _Indirizzo; } set { _Indirizzo = value; } }
            public string CodiceMeccanizzazione { get { return _CodiceMeccanizzazione; } set { _CodiceMeccanizzazione = value; } }
            public int Abi { get { return _Abi; } set { _Abi = value; } }
            public int Cab { get { return _Cab; } set { _Cab = value; } }
            public int Frazionario { get { return _Frazionario; } set { _Frazionario = value; } }
            #endregion public properties

        }

        public enum TipoRicerca
        {
            Abi_Cab,
            Abi_Cap,
            Estero,
            Cassa
        };
        #endregion nested class
    }
}
