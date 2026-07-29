using INPS.Pensioni.Liquidazione.BLCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
//using static INPS.Pensioni.Liquidazione.GestioneMsIndebiti;

namespace INPS.Pensioni.Liquidazione.Service_Reference
{
    public class msIndebiti
    {
        private const string Config_ApiIndebitiUrl = "ApiIndebitiUrl";
        private readonly WebClient _authClient;   // contiene headers autenticazione
        private string _baseUrl = ConfigurationManager.AppSettings[Config_ApiIndebitiUrl];

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        public msIndebiti(WebClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            _authClient = httpClient;
        }

        public RootIndebitoDto GetAnteprimaDebito(string numeroDomanda)
        {
            var url = string.Format("{0}/Ricostituzione/AnteprimaDebitoByDomus/{1}",
                BaseUrl != null ? BaseUrl.TrimEnd('/') : "",
                Uri.EscapeDataString(numeroDomanda));

            LogRequest(url, string.Empty, numeroDomanda, Utility.MetodoServizio.GetAnteprimaDebito);

            string bodyResponse;
            HttpStatusCode statusCode = EseguiHttpRequest(url, "GET", null, out bodyResponse);

            LogResponse(url, statusCode.ToString(), bodyResponse, numeroDomanda, Utility.MetodoServizio.GetAnteprimaDebito);

            RootIndebitoDto deserializedOutput = JsonConvert.DeserializeObject<RootIndebitoDto>(bodyResponse);

            return deserializedOutput;
        }

        public bool AggiornaCausali(long numeroDomanda, string matricola, List<ContoRicDto> contiRic)
        {
            var url = string.Format("{0}/Ricostituzione/AggiornaCausali?Domus={1}&matricola={2}",
                BaseUrl != null ? BaseUrl.TrimEnd('/') : "",
                numeroDomanda,
                matricola);

            string body = JsonConvert.SerializeObject(contiRic);

            LogRequest(url, body, numeroDomanda.ToString(), Utility.MetodoServizio.AggiornaCasuali);

            string bodyResult;
            HttpStatusCode statusCodeResult = EseguiHttpRequest(url, "POST", body, out bodyResult);

            LogResponse(url, statusCodeResult.ToString(), bodyResult, numeroDomanda.ToString(), Utility.MetodoServizio.AggiornaCasuali);

            BooleanResult response = JsonConvert.DeserializeObject<BooleanResult>(bodyResult);
            return response.success;
        }

        public bool NotificaTE08(long numeroDomanda, bool accodaTE08IND)
        {
            var url = string.Format("{0}/Ricostituzione/NotificaTE08?Domus={1}&accodaTE08IND={2}",
                BaseUrl != null ? BaseUrl.TrimEnd('/') : "",
                numeroDomanda,
                accodaTE08IND);

            LogRequest(url, string.Empty, numeroDomanda.ToString(), Utility.MetodoServizio.NotificaTE08);

            string resultBody;
            HttpStatusCode resultStatusCode = EseguiHttpRequest(url, "POST", string.Empty, out resultBody);

            LogResponse(url, resultStatusCode.ToString(), resultBody, numeroDomanda.ToString(), Utility.MetodoServizio.NotificaTE08);

            BooleanResult response = JsonConvert.DeserializeObject<BooleanResult>(resultBody);
            return response.success;
        }

        #region Helper Methods

        private HttpStatusCode EseguiHttpRequest(string url, string method, string body, out string bodyResponse)
        {
            bodyResponse = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            // Copio le header dal WebClient passato al costruttore (quelle impostate da SetApiIdentity)
            ApplyAuthHeadersToRequest(request, _authClient);

            if (!string.IsNullOrEmpty(body))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                request.ContentLength = bytes.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    bodyResponse = reader.ReadToEnd();
                    return response.StatusCode;
                }
            }
            catch (WebException wex)
            {
                var resp = wex.Response as HttpWebResponse;
                if (resp != null)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        bodyResponse = reader.ReadToEnd();
                    }
                    return resp.StatusCode;
                }
                throw;
            }
        }

        private void ApplyAuthHeadersToRequest(HttpWebRequest request, WebClient clientWithHeaders)
        {
            if (clientWithHeaders == null) return;

            NameValueCollection headers = clientWithHeaders.Headers;
            if (headers == null) return;

            foreach (var key in headers.AllKeys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                string value = headers[key];

                switch (key.ToLowerInvariant())
                {
                    case "content-type":
                        request.ContentType = value;
                        break;
                    case "accept":
                        request.Accept = value;
                        break;
                    case "user-agent":
                        request.UserAgent = value;
                        break;
                    case "referer":
                        request.Referer = value;
                        break;
                    default:
                        request.Headers[key] = value;
                        break;
                }
            }
        }

        private void LogRequest(string url, string body, string numeroDomanda, Utility.MetodoServizio metodo)
        {
            LogSoap oggettoLog = new LogSoap(url, body, string.Empty);
            GestioneLogSoap.SalvaLogSoap(oggettoLog, Utility.Servizio.SrvSistemaPensioni, metodo, Utility.SOAPLogDirection.IN, numeroDomanda);
        }

        private void LogResponse(string url, string statusCode, string body, string numeroDomanda, Utility.MetodoServizio metodo)
        {
            LogSoap oggettoLog = new LogSoap(url, body, statusCode);
            GestioneLogSoap.SalvaLogSoap(oggettoLog, Utility.Servizio.SrvSistemaPensioni, metodo, Utility.SOAPLogDirection.OUT, numeroDomanda);
        }

        #endregion
    }

    [DataContractAttribute]
    public class LogSoap
    {
        public LogSoap(string Url, string Body, string StatusCode)
        {
            this.Url = Url;
            this.Body = Body;
            this.StatusCode = StatusCode;
        }

        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public string StatusCode { get; set; }
    }

    public class RootIndebitoDto
    {
        public IndebitoDto Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int Code { get; set; }
    }

    public class IndebitoDto
    {
        public string SedeGestione { get; set; }
        public string ChiavePrestazione { get; set; }
        public string SedePrestazione { get; set; }
        public int TipoPratica { get; set; }
        public string Categoria { get; set; }
        public string CodiceFiscaleTitolare { get; set; }
        public string NomeDebitore { get; set; }
        public string CognomeDebitore { get; set; }
        public DateTime DataInizioDebito { get; set; }
        public DateTime DataFineDebito { get; set; }
        public decimal Importo { get; set; }
        public decimal ImponibileAnnoInCorso { get; set; }
        public decimal ImponibileAnniPrecedenti { get; set; }
        public decimal NonImponibile { get; set; }
        public StatoIndebitoDto StatoIndebito { get; set; }
        public DateTime DataRicostituzione { get; set; }
        public string Ndompro { get; set; }
        public List<ContoRicDto> ContiRic { get; set; }
    }

    public class StatoIndebitoDto
    {
        public int IdStatoIndebito { get; set; }
        public string StatoEnum { get; set; }
        public string StatoStr { get; set; }
        public string Descrizione { get; set; }
        public int TipoStato { get; set; }
    }

    public class ContoRicDto
    {
        public ContoRecuperoDto ContoRecupero { get; set; }
        public bool IsModificabile { get; set; }
        public List<CausaleDto> CausaliAmmesse { get; set; }
        public CausaleDto CausaleSelezionata { get; set; }
    }

    public class ContoRecuperoDto
    {
        public int IdConto { get; set; }
        public int Codice { get; set; }
        public string Nome { get; set; }
        public string CodiceBilancio { get; set; }
        public CausaleDto Causale { get; set; }
        public decimal Importo { get; set; }
        public decimal Residuo { get; set; }
    }

    public class CausaleDto
    {
        public int Sintetica { get; set; }
        public int Analitica { get; set; }
        public string Descrizione { get; set; }
    }

    public class BooleanResult
    {
        public bool data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int code { get; set; }
    }
}

