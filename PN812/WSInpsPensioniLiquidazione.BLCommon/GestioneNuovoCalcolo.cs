using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon.CalcoloCentraleConfiguratore;
using INPS.Pensioni.Liquidazione.DataCommon;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneNuovoCalcolo
    {
        public static void GetRispostaNuovoCalcoloByNDomus(long Ndomus, out DatiEsitoNuovoCalcolo datiEsitoNuovoCalcolo)
        {
            EsitoNuovoCalcolo esitoNuovoCalcolo = null;
            datiEsitoNuovoCalcolo = null;

            DAGestioneNuovoCalcolo.GetRispostaNuovoCalcoloByNDomus(Ndomus, out esitoNuovoCalcolo);
            if (esitoNuovoCalcolo == null)
                return;

            datiEsitoNuovoCalcolo = new DatiEsitoNuovoCalcolo();
            Utility.ValorizzaOggetti(esitoNuovoCalcolo, datiEsitoNuovoCalcolo);

        }
        public static void GetRispostaNuovoCalcoloByTransactionId(string TransactionId, out DatiEsitoNuovoCalcolo datiEsitoNuovoCalcolo)
        {
            EsitoNuovoCalcolo esitoNuovoCalcolo = null;
            datiEsitoNuovoCalcolo = null;

            DAGestioneNuovoCalcolo.GetRispostaNuovoCalcoloByTransactionId(TransactionId, out esitoNuovoCalcolo);
            if (esitoNuovoCalcolo == null || string.IsNullOrEmpty(esitoNuovoCalcolo.Risposta))
                return;

            datiEsitoNuovoCalcolo = new DatiEsitoNuovoCalcolo();
            Utility.ValorizzaOggetti(esitoNuovoCalcolo, datiEsitoNuovoCalcolo);
        }

        public static void InsertOrUpdateNuovoCalcolo(DatiEsitoNuovoCalcolo datiEsitoNuovoCalcolo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                EsitoNuovoCalcolo esitoNuovoCalcolo = new EsitoNuovoCalcolo();
                Utility.ValorizzaOggetti(datiEsitoNuovoCalcolo, esitoNuovoCalcolo);
                DAGestioneNuovoCalcolo.InsertOrUpdateNuovoCalcolo(esitoNuovoCalcolo);
                transactionScope.Complete();
            }
        }

        public static void GetCtrlFlowConf(out DateTime? dataAggiornamento, out string sFlowConf)
        {
            CtrlFlowConf ctrlFlowConf = null;
            sFlowConf = null;
            dataAggiornamento = null;
            DAGestioneNuovoCalcolo.GetCtrlFlowConf(out ctrlFlowConf);
            if (ctrlFlowConf == null || string.IsNullOrEmpty(ctrlFlowConf.FlowConf))
                return;

            sFlowConf = ctrlFlowConf.FlowConf;
            dataAggiornamento = ctrlFlowConf.DataAggiornamento;
        }

        public static void GetCtrlSedeTransazioneNuovoCalcoloBySede(string sede, out DatiCtrlSedeTransazioneNuovoCalcolo datiCtrlSede)
        {
            CtrlSedeTransazioneNuovoCalcolo ctrlSede = null;
            datiCtrlSede = null;
            DAGestioneNuovoCalcolo.GetCtrlSedeTransazioneNuovoCalcoloBySede(sede, out ctrlSede);
            if (ctrlSede == null)
                return;

            datiCtrlSede = new DatiCtrlSedeTransazioneNuovoCalcolo();
            Utility.ValorizzaOggetti(ctrlSede, datiCtrlSede);
        }

        public static void InsertOrUpdateNuovoCalcolo(string ctrlFlowConf)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneNuovoCalcolo.InsertOrUpdateCtrlFlow(ctrlFlowConf);
                transactionScope.Complete();
            }
        }

        public static List<GestioneNuovoCalcolo.FlowConf> GetConfigurazioneDinamica(long? Ndomus, string matricolaUtenteAcquisizione)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DurataCacheNuovoCalcolo", out controlloDinamico);
            int minutiCache = 0;
            DateTime? dataAggiornamento;
            string sFlowConf;
            bool callServizio = true;
            List<GestioneNuovoCalcolo.FlowConf> lstConfigurazione = new List<GestioneNuovoCalcolo.FlowConf>();
            GestioneNuovoCalcolo.GetCtrlFlowConf(out dataAggiornamento, out sFlowConf);
            if (!string.IsNullOrEmpty(sFlowConf))
            {
                try
                {
                    lstConfigurazione = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GestioneNuovoCalcolo.FlowConf>>(sFlowConf);
                    if (lstConfigurazione == null || lstConfigurazione.Count == 0)
                        callServizio = true;
                    else
                    {
                        if (dataAggiornamento != null && controlloDinamico != null && int.TryParse(controlloDinamico.ValoreControllo, out minutiCache))
                        {
                            if (DateTime.Compare(DateTime.Now, dataAggiornamento.Value.AddMinutes(minutiCache)) <= 0)
                            {
                                callServizio = false;
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    callServizio = true;
                    GestioneLogGenerico.SalvaLogGenerico(Ndomus != null ? (long)Ndomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : "", null, Ex != null ? Ex.StackTrace : null);
                }

            }
            if (callServizio)
            {
                try
                {

                    Guid guid = Guid.NewGuid();
                    CalcoloCentraleConfiguratoreClient client = new CalcoloCentraleConfiguratoreClient(SetApiIdentity(matricolaUtenteAcquisizione));
                    GestioneLogSoap.SalvaLogSoap("GetAllFlowConf", Utility.Servizio.SrvNuovoCalcolo, Utility.MetodoServizio.GetAllFlowConf, Utility.SOAPLogDirection.IN, Ndomus != null ? Ndomus.ToString() : null, guid);
                    var response = client.GetAllFlowConf();
                    lstConfigurazione = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GestioneNuovoCalcolo.FlowConf>>(response);
                    //aggiorna DB
                    GestioneNuovoCalcolo.InsertOrUpdateNuovoCalcolo(response);
                }
                catch (Exception Ex)
                {
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : "", null, Ex != null ? Ex.StackTrace : null);
                }
            }
            return lstConfigurazione;
        }

        public static bool GetRisposteValideNuovoCalcoloByNDomus(long Ndomus, int timeout)
        {
            EsitoNuovoCalcolo esitoNuovoCalcolo = null;
            List<EsitoNuovoCalcolo> lstEsitoNuovoCalcolo = null;
            bool esitoInattesa = false;
            //datiEsitoNuovoCalcolo = null;

            DAGestioneNuovoCalcolo.GetRisposteValideNuovoCalcoloByNDomus(Ndomus, out lstEsitoNuovoCalcolo);
            if (lstEsitoNuovoCalcolo != null )
            {
                //richiesto ulteriore delay modulabile da applicare al timeout base
                GestioneControlliDinamici.ControlloDinamico controlloDinamico;
                int secondiDelay = 0;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DelayTimeoutNuovoCalcolo", out controlloDinamico);
                if (controlloDinamico != null && int.TryParse(controlloDinamico.ValoreControllo, out secondiDelay))
                {
                    timeout = timeout + secondiDelay;
                }

                //se ci sono transaction id senza risposta ancora non scaduti (data inserimento + timeout > data di oggi ) -> pulsante bloccato
                esitoInattesa = lstEsitoNuovoCalcolo.Exists(x => x.Risposta == null && x.DataInserimento.Value.AddSeconds(timeout) > DateTime.Now);

            }

            return esitoInattesa;
        }

        public static void UpdateScadutoEsistoNuovoCalcolo(long numeroDomanda)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneNuovoCalcolo.UpdateScadutoEsistoNuovoCalcolo(numeroDomanda);
                transactionScope.Complete();
            }
        }

        private static WebClient SetApiIdentity(string matricola)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient client = new WebClient();
            string token = string.Empty;
            TokenIdentity identity = new TokenIdentity();
            identity.UserId = matricola ?? identity.UserId;
            var plainIdentity = Newtonsoft.Json.JsonConvert.SerializeObject(identity);
            var encoding = Encoding.UTF8;
            var bytesIdentity = encoding.GetBytes(plainIdentity);
            token = string.Concat(TokenBearer, TokenHeader, ".", Convert.ToBase64String(bytesIdentity), ".");
            client.Headers.Add(ApiClientId, ConfigurationManager.AppSettings[Config_ApiClientId] ?? string.Empty);
            client.Headers.Add(ApiClientSecret, ConfigurationManager.AppSettings[Config_ApiClientSecret] ?? string.Empty);
            client.Headers.Add(ApiAuthorization, token);
            client.Headers.Add("Content-Type", "application/json");

            return client;
        }

        private class TokenIdentity
        {
            public TokenIdentity()
            {
                IdentityProvider = ConfigurationManager.AppSettings[Config_ApiNuovoCalcoloProvider] != null ? ConfigurationManager.AppSettings[Config_ApiNuovoCalcoloProvider].ToString() : string.Empty;
                UserId = ConfigurationManager.AppSettings[Config_ApiNuovoCalcoloUserId] != null ? ConfigurationManager.AppSettings[Config_ApiNuovoCalcoloUserId].ToString() : string.Empty;
                CodiceEnte = string.Empty;
                CodiceUfficio = string.Empty;
            }
            public string UserId { get; set; }
            public string IdentityProvider { get; set; }
            public string CodiceEnte { get; set; }
            public string CodiceUfficio { get; set; }
        }

        private const string TokenHeader = "eyJhbGciOiJub25lIn0";
        private const string TokenBearer = "Bearer ";
        private const string ApiClientId = "X-IBM-Client-Id";
        private const string ApiClientSecret = "X-IBM-Client-Secret";
        private const string ApiAuthorization = "Authorization";
        private const string Config_ApiClientId = "ApiClientId";
        private const string Config_ApiClientSecret = "ApiClientSecret";
        private const string Config_ApiNuovoCalcoloProvider = "ApiNuovoCalcoloProvider";
        private const string Config_ApiNuovoCalcoloUserId = "ApiNuovoCalcoloUserId";
        #region nested class

        public class DatiEsitoNuovoCalcolo
        {

            public DatiEsitoNuovoCalcolo() { }

            public DatiEsitoNuovoCalcolo(long NDomus, string TransactionId, string Risposta)
            {
                this._NDomus = NDomus;
                this._TransactionId = TransactionId;
                this._Risposta = Risposta;
            }

            #region private properties

            private long? _NDomus;
            private string _TransactionId;
            private DateTime? _DataInserimento;
            private string _Risposta;
            private DateTime? _DataRisposta;

            #endregion private properties

            #region public properties

            public long? NDomus { get { return _NDomus; } set { _NDomus = value; } }
            public string TransactionId { get { return _TransactionId; } set { _TransactionId = value; } }
            public DateTime? DataInserimento { get { return _DataInserimento; } set { _DataInserimento = value; } }
            public string Risposta { get { return _Risposta; } set { _Risposta = value; } }
            public DateTime? DataRisposta { get { return _DataRisposta; } set { _DataRisposta = value; } }

            #endregion public properties 

        }

        public class DatiCtrlSedeTransazioneNuovoCalcolo
        {

            public string Sede;
            public bool? GAIN { get; set; }
            public bool? GAPL { get; set; }
            public bool? GARC { get; set; }
            public bool? FSPR { get; set; }
            public bool? FSPL { get; set; }
            public bool? FSRC { get; set; }
            public bool? GACI { get; set; }
            public bool? CI01 { get; set; }
            public bool? CI02 { get; set; }
            public bool? Attiva { get; set; }
        }
        #endregion nested class

        #region Risposta

        public class RispostaJson
        {
            public Metadata metadata { get; set; }
            public Error[] errors { get; set; }
            public Payload payload { get; set; }
        }

        public class Metadata
        {
            public string schemaVersion { get; set; }
            public long messageDate { get; set; }
            public string username { get; set; }
            public string transactionId { get; set; }
            public Messagewriter messageWriter { get; set; }
        }

        public class Messagewriter
        {
            public string code { get; set; }
            public string version { get; set; }
            public string schemaType { get; set; }
        }


        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
            public string tipologia { get; set; }
            public string dettaglio { get; set; }
            public string ambito { get; set; }
        }

        public class Payload
        {
            public string esito { get; set; }
            public DateTime dCalc { get; set; }
            public string codGruppo { get; set; }
            public string codProdotto { get; set; }
            public string codTipo { get; set; }
            public string codGestione { get; set; }
            public string codFondo { get; set; }
            public string flagTipoRichiesta { get; set; }
            public string codFase { get; set; }
            public string indConVint { get; set; }
            public Chiavepensione chiavePensione { get; set; }
            public long numDomus { get; set; }
        }

        public class Chiavepensione
        {
            public string codCategoria { get; set; }
            public string codSede { get; set; }
            public string numCert { get; set; }
        }

        #endregion Risposta

        #region Conf

        public class FlowConf
        {
            public string CodGruppo { get; set; }
            public string CodProdotto { get; set; }
            public string CodTipo { get; set; }
            public string CodGestione { get; set; }
            public string CodFondo { get; set; }
            public string TipoRichiesta { get; set; }
            public string Fase { get; set; }
            public string IndConvInt { get; set; }
            public string FlowCode { get; set; }
            public string SistemiInvocati { get; set; }
            public DateTime? DecOrig { get; set; }
            public string Descrizione { get; set; }
            public string CodCategoria { get; set; }
            public string DescCategoria { get; set; }
            public List<string> CodiciTipoRichiesta { get; set; }
            public int? TimeoutElaborazione { get; set; }
        }
        #endregion
    }
}
