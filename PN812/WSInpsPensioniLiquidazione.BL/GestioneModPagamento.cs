using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.ModPagamento;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneModPagamento
    {
        #region internal members
        internal static bool GetPagamentoByDomanda(Int64 numeroDomanda, out AreaPagamento datiPagamento, out string errori)
        {
            datiPagamento = null;
            errori = "";
            try
            {
                RichiestaModPagamento richiesta = null;
                ValorizzaRichiesta(numeroDomanda, out richiesta);
                if (richiesta == null)
                {
                    errori = "Area richiesta modalita pagamento non valorizzata correttamente";
                    return false;
                }

                if (!GetPagamentoFromSrvModPagamento(richiesta, out datiPagamento, out errori))
                    return false;
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio ModPagamento: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        
        internal static bool GetSindacatoByDomanda(Int64 numeroDomanda, out AreaSindacato datiSindacato, out string errori)
        {
            datiSindacato = null;
            errori = "";
            try
            {
                RichiestaModPagamento richiesta = null;
                ValorizzaRichiesta(numeroDomanda, out richiesta);
                if (richiesta == null)
                {
                    errori = "Area richiesta modalita pagamento non valorizzata correttamente";
                    return false;
                }

                if (!GetSindacatoFromSrvModPagamento(richiesta, out datiSindacato, out errori))
                    return false;
            }
            catch (Exception Ex)
            {
                errori = "Errore nella chiamata al servizio ModPagamento: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion

        #region private members
        private static bool GetPagamentoFromSrvModPagamento(RichiestaModPagamento richiesta, out AreaPagamento risposta, out string errori)
        {
            errori = "";
            risposta = null;
            WsModalitaPagamentoSoapClient proxy = new WsModalitaPagamentoSoapClient();
            tRisposta esito = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    esito = proxy.getModalitaPagamento(richiesta.CodiceApplicazione.ToString(), richiesta.CodiceUtente.ToString(), richiesta.NumeroDomanda.ToString());

                    if (esito.CodiceRisposta == "0")
                    {
                        if (esito.DatiModalitaPagamento == null)
                        {
                            errori = "Area Modalità pagamento nulla";
                            return false;
                        }

                        string modPagamento = "";
                        string statoPagEstero = "";
                        string iban = "";
                        string bic = "";
                        string abi = "";
                        string cab = "";
                        string frazionario = "";
                        string numeroLibretto = "";
                        string tipoPagamento = "";

                        if (esito.DatiModalitaPagamento.Abi != null)
                            abi = esito.DatiModalitaPagamento.Abi.Trim();
                        if (esito.DatiModalitaPagamento.Bic != null)
                            bic = esito.DatiModalitaPagamento.Bic.Trim();
                        if (esito.DatiModalitaPagamento.Cab != null)
                            cab = esito.DatiModalitaPagamento.Cab.Trim();
                        if (esito.DatiModalitaPagamento.Iban != null)
                            iban = esito.DatiModalitaPagamento.Iban.Trim().ToUpper();
                        if (esito.DatiModalitaPagamento.TipoPagamento != null)
                            modPagamento = esito.DatiModalitaPagamento.TipoPagamento.Trim().ToUpper();
                        if (esito.DatiModalitaPagamento.StatoPagamentoEstero != null)
                            statoPagEstero = esito.DatiModalitaPagamento.StatoPagamentoEstero.Trim().ToUpper();
                        //if (esito.DatiModalitaPagamento.DescStatoPagamentoEstero != null)
                        //    descPagEstero = esito.DatiModalitaPagamento.DescStatoPagamentoEstero.Trim().ToUpper();
                        if (esito.DatiModalitaPagamento.LibrettoUfficioPostale != null)
                            numeroLibretto = esito.DatiModalitaPagamento.LibrettoUfficioPostale.Trim();
                        if (esito.DatiModalitaPagamento.ModalitaPagamento != null)
                            tipoPagamento = esito.DatiModalitaPagamento.ModalitaPagamento.Trim();

                        if ((abi == null || abi.Trim() == "") && (bic == null || bic.Trim() == "") && (cab == null || cab.Trim() == "") &&
                            (iban == null || iban.Trim() == "") && (modPagamento == null || modPagamento.Trim() == "") &&
                            (tipoPagamento == null || tipoPagamento.Trim() == "") && (statoPagEstero == null || statoPagEstero.Trim() == ""))
                        {
                            errori = "Area Pagamento non valorizzata";
                            return false;
                        }

                        if (modPagamento != "")
                        {
                            switch (modPagamento)
                            {
                                case "1":
                                    modPagamento = "S";
                                    break;
                                case "2":
                                    modPagamento = "C";
                                    break;
                                case "3":
                                    modPagamento = "L";
                                    break;
                                case "4":
                                    modPagamento = "K";
                                    break;
                                case "5":
                                    modPagamento = "A";
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (tipoPagamento != "")
                        {
                            switch (tipoPagamento)
                            {
                                case "1":
                                    tipoPagamento = "B";
                                    break;
                                case "2":
                                    tipoPagamento = "P";
                                    break;
                                case "3":
                                    tipoPagamento = "E";
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (bic != "")
                            bic = bic.PadLeft(11, '0');

                        if (abi != "")
                            abi = abi.PadLeft(5, '0');

                        if (cab != "")
                            cab = cab.PadLeft(7, '0');

                        if (iban != "" && tipoPagamento != "E" && iban.Length != 27)
                        {
                            errori = "Codice IBAN errato";
                            return false;
                        }
    
                        if (numeroLibretto != "")
                            numeroLibretto = numeroLibretto.PadLeft(12, '0');

                        if (tipoPagamento == "P")
                        {
                            frazionario = cab;
                            cab = "";
                        }
                        risposta = new AreaPagamento(abi, bic, cab, frazionario, iban, modPagamento, tipoPagamento, statoPagEstero, numeroLibretto);
                    }
                    else
                    {
                        if (esito.CodiceRisposta == "1" && esito.DescrizioneRisposta == "DATI_INESISTENTI")
                            return true;
                        else
                        {
                            errori = "Il servizio ModPagamento ha restituito il seguente errore: " + esito.CodiceRisposta + " " + esito.DescrizioneRisposta;
                            return false;
                        }
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = exception.Message;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
                {
                    errori = "Si è verificato un errore di sicurezza nel consumo del servizio ModPagamento";
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = "Puntamento errato al servizio ModPagamento";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = "Errore di comunicazione con il servizio ModPagamento";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = "Errore nella chiamata al servizio ModPagamento: " + Ex.Message;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    try
                    {
                        if (proxy.State != CommunicationState.Closed &&
                           proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close(); // may throw exception while closing
                        }
                        else
                        {
                            proxy.Abort();
                        }
                    }
                    catch (CommunicationException)
                    {
                        proxy.Abort();
                    }
                    catch (Exception)
                    {
                    }
                }

            }
            return true;
        }

        private static bool GetSindacatoFromSrvModPagamento(RichiestaModPagamento richiesta, out AreaSindacato risposta, out string errori)
        {
            errori = "";
            risposta = null;
            WsModalitaPagamentoSoapClient proxy = new WsModalitaPagamentoSoapClient();
            tRispostaSemplice esito = null;
            using (new MethodExecutionTracer())
            {
                try
                {
                    esito = proxy.getCodiceSindacato(richiesta.CodiceApplicazione.ToString(), richiesta.CodiceUtente.ToString(), richiesta.NumeroDomanda.ToString());
                    if (esito.CodiceRisposta == "0")
                    {
                        if (esito == null || esito.Valore == null || esito.Valore.Trim() == "")
                        {
                            errori = "Area Sindacato non valorizzata";
                            return false;
                        }
                        risposta = new AreaSindacato(esito.Valore.Trim().PadRight(2, ' ').ToUpper());
                    }
                    else
                    {
                        if (esito.CodiceRisposta == "1" && esito.DescrizioneRisposta == "DATI_INESISTENTI")
                            return true;
                        else
                        {
                            errori = "Il servizio ModPagamento ha restituito il seguente errore: " + esito.CodiceRisposta + " " + esito.DescrizioneRisposta;
                            return false;
                        }
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = exception.Message;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
                {
                    errori = "Si è verificato un errore di sicurezza nel consumo del servizio ModPagamento";
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = "Puntamento errato al servizio ModPagamento";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = "Errore di comunicazione con il servizio ModPagamento";
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = "Errore nella chiamata al servizio ModPagamento: " + Ex.Message;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    return false;
                }
                finally
                {
                    try
                    {
                        if (proxy.State != CommunicationState.Closed &&
                           proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close(); // may throw exception while closing
                        }
                        else
                        {
                            proxy.Abort();
                        }
                    }
                    catch (CommunicationException)
                    {
                        proxy.Abort();
                    }
                    catch (Exception)
                    {
                    }
                }

            }
            return true;
        }

        private static void ValorizzaRichiesta(Int64 numeroDomanda, out RichiestaModPagamento richiesta)
        {
            richiesta = new RichiestaModPagamento();
            richiesta.CodiceApplicazione = 1;
            richiesta.CodiceUtente = 1;
            richiesta.NumeroDomanda = numeroDomanda;
        }
        #endregion

        #region nested class
        public class RichiestaModPagamento
        {
            public RichiestaModPagamento()
            { }
            public RichiestaModPagamento(short codiceApplicazione, short codiceUtente, Int64 numeroDomanda)
            {
                this._CodiceApplicazione = codiceApplicazione;
                this._CodiceUtente = codiceUtente;
                this._NumeroDomanda = numeroDomanda;
            }
            #region private properties
            private short _CodiceApplicazione;
            private short _CodiceUtente;
            private Int64 _NumeroDomanda;
            #endregion

            #region public properties
            public short CodiceApplicazione { get { return _CodiceApplicazione; } set { _CodiceApplicazione = value; } }
            public short CodiceUtente { get { return _CodiceUtente; } set { _CodiceUtente = value; } }
            public Int64 NumeroDomanda { get { return _NumeroDomanda; } set { _NumeroDomanda = value; } }
            #endregion
        }

        public class AreaPagamento
        {
            public AreaPagamento()
            {

            }

            public AreaPagamento(string abi, string bic, string cab, string frazionario, string iban, string modPagamento, string tipoPagamento, string statoPagamentoEstero, string numLibretto)
            {
                this._Abi = Utility.StringToNullableInt(abi);
                this._Bic = bic;
                this._Cab = Utility.StringToNullableInt(cab);
                this._Frazionario = Utility.StringToNullableInt(frazionario);
                this._Iban = iban;
                this._ModalitaPagamento = modPagamento.ToCharArray(0,1)[0];
                this._TipoPagamento = tipoPagamento.ToCharArray(0, 1)[0];
                this._StatoPagamentoEstero = statoPagamentoEstero;
                this._NumeroLibretto = numLibretto;
            }

            #region private properties
            private System.Nullable<int> _Abi;
            private string _Bic;
            private System.Nullable<int> _Cab;
            private System.Nullable<int> _Frazionario;
            private string _Iban;
            private char _ModalitaPagamento;
            private char _TipoPagamento;
            private string _StatoPagamentoEstero;
            private string _NumeroLibretto;
            #endregion

            #region public properties
            public System.Nullable<int> Abi { get { return _Abi; } set { _Abi = value; } }
            public string Bic { get { return _Bic; } set { _Bic = value; } }
            public System.Nullable<int> Cab { get { return _Cab; } set { _Cab = value; } }
            public System.Nullable<int> Frazionario { get { return _Frazionario; } set { _Frazionario = value; } }
            public string Iban { get { return _Iban; } set { _Iban = value; } }
            public char ModalitaPagamento { get { return _ModalitaPagamento; } set { _ModalitaPagamento = value; } }
            public char TipoPagamento { get { return _TipoPagamento; } set { _TipoPagamento = value; } }
            public string StatoPagamentoEstero { get { return _StatoPagamentoEstero; } set { _StatoPagamentoEstero = value; } }
            public string NumeroLibretto { get { return _NumeroLibretto; } set { _NumeroLibretto = value; } }
            #endregion

        }

        public class AreaSindacato
        {
            public AreaSindacato()
            {

            }

            public AreaSindacato(string sindacato)
            {
                this._Sindacato = sindacato;
            }

            #region private properties
            private string _Sindacato;
            #endregion

            #region public properties
            public string Sindacato { get { return _Sindacato; } set { _Sindacato = value; } }
            #endregion

        }
        #endregion
    }
}
