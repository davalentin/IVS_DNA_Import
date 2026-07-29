using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Service_Reference
{
    public class msSistemaPensioni
    {
        public msSistemaPensioni(WebClient httpClient)
        {
            _httpClient = httpClient;
        }

        private const string Config_ApiSistemaPensioniUrl = "ApiSistemaPensioniUrl";
        private WebClient _httpClient;
        private string _baseUrl = ConfigurationManager.AppSettings[Config_ApiSistemaPensioniUrl];
        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        public void InsertOrUpdateFlusso(AreaDomandeAutomatizzateRequest body)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/msSistemaPensioni/InsertOrUpdateDomandeAutomatizzate");
            _httpClient.BaseAddress = BaseUrl;
            string jsonBody = JsonConvert.SerializeObject(body);
            string result = Encoding.ASCII.GetString(_httpClient.UploadData(urlBuilder_.ToString(), "POST", Encoding.Default.GetBytes(jsonBody)));
            _httpClient.Dispose();

            AreaDomandeAutomatizzateResponse output = JsonConvert.DeserializeObject<AreaDomandeAutomatizzateResponse>(result);
            //if (output != null && output.Esito.ListaEsiti.Count > 0)
            //    throw new Exception(output.Esito.ListaEsiti.ToString());
            return;
        }
        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class AreaDomandeAutomatizzateRequest
        {
            [Newtonsoft.Json.JsonProperty("utente", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public AreaUtente Utente { get; set; }

            [Newtonsoft.Json.JsonProperty("domanda", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public DomandaAutomatizzataEnitity Domanda { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class AreaUtente
        {
            [Newtonsoft.Json.JsonProperty("session_Id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Session_Id { get; set; }

            [Newtonsoft.Json.JsonProperty("utente", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Utente { get; set; }

            [Newtonsoft.Json.JsonProperty("idConsumer", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int? IdConsumer { get; set; }

            [Newtonsoft.Json.JsonProperty("indirizzoIp", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string IndirizzoIp { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class DomandaAutomatizzataEnitity
        {
            [Newtonsoft.Json.JsonProperty("idFlusso", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int IdFlusso { get; set; }

            [Newtonsoft.Json.JsonProperty("idTipoAutomazione", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int IdTipoAutomazione { get; set; }

            [Newtonsoft.Json.JsonProperty("numDomus", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long NumDomus { get; set; }

            [Newtonsoft.Json.JsonProperty("codiceFiscale", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CodiceFiscale { get; set; }

            [Newtonsoft.Json.JsonProperty("nome", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Nome { get; set; }

            [Newtonsoft.Json.JsonProperty("cognome", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Cognome { get; set; }

            [Newtonsoft.Json.JsonProperty("dataNascita", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTimeOffset? DataNascita { get; set; }

            [Newtonsoft.Json.JsonProperty("gruppo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Gruppo { get; set; }

            [Newtonsoft.Json.JsonProperty("prodotto", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Prodotto { get; set; }

            [Newtonsoft.Json.JsonProperty("tipo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Tipo { get; set; }

            [Newtonsoft.Json.JsonProperty("gestione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Gestione { get; set; }

            [Newtonsoft.Json.JsonProperty("fondo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Fondo { get; set; }

            [Newtonsoft.Json.JsonProperty("ente", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Ente { get; set; }

            [Newtonsoft.Json.JsonProperty("indconvint", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public bool Indconvint { get; set; }

            [Newtonsoft.Json.JsonProperty("filtroWebdom", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string FiltroWebdom { get; set; }

            [Newtonsoft.Json.JsonProperty("centroOperativo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CentroOperativo { get; set; }

            [Newtonsoft.Json.JsonProperty("codiceSedeMeta", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CodiceSedeMeta { get; set; }

            [Newtonsoft.Json.JsonProperty("codSede", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CodSede { get; set; }

            [Newtonsoft.Json.JsonProperty("chiavePensione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string ChiavePensione { get; set; }

            [Newtonsoft.Json.JsonProperty("prog", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int? Prog { get; set; }

            [Newtonsoft.Json.JsonProperty("decorrenzaPensione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTimeOffset? DecorrenzaPensione { get; set; }

            [Newtonsoft.Json.JsonProperty("dataAcquisizione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTimeOffset? DataAcquisizione { get; set; }

            [Newtonsoft.Json.JsonProperty("descrizioneEsito", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string DescrizioneEsito { get; set; }

            [Newtonsoft.Json.JsonProperty("sezione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Sezione { get; set; }

            [Newtonsoft.Json.JsonProperty("dettaglio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Dettaglio { get; set; }

            [Newtonsoft.Json.JsonProperty("dataElaborazione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTimeOffset? DataElaborazione { get; set; }

            [Newtonsoft.Json.JsonProperty("descrizioneEsitoOperatore", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string DescrizioneEsitoOperatore { get; set; }

            [Newtonsoft.Json.JsonProperty("idTipoEsitoOperatore", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int? IdTipoEsitoOperatore { get; set; }

            [Newtonsoft.Json.JsonProperty("dettaglioOperatore", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string DettaglioOperatore { get; set; }

            [Newtonsoft.Json.JsonProperty("dataElaborazioneOperatore", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTimeOffset? DataElaborazioneOperatore { get; set; }

            [Newtonsoft.Json.JsonProperty("dataPresentazione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTimeOffset? DataPresentazione { get; set; }

            [Newtonsoft.Json.JsonProperty("matricolaOperatore", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string MatricolaOperatore { get; set; }
        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class AreaDomandeAutomatizzateResponse
        {
            [Newtonsoft.Json.JsonProperty("esito", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public AreaEsito Esito { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class AreaEsito
        {
            [Newtonsoft.Json.JsonProperty("tipoMessaggio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
            public TipoMessaggio TipoMessaggio { get; set; }

            [Newtonsoft.Json.JsonProperty("listaEsiti", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.Collections.Generic.ICollection<AreaEsitoBase> ListaEsiti { get; set; }

        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public enum TipoMessaggio
        {
            [System.Runtime.Serialization.EnumMember(Value = @"ErroreTecnico")]
            ErroreTecnico = 0,

            [System.Runtime.Serialization.EnumMember(Value = @"Warning")]
            Warning = 1,

            [System.Runtime.Serialization.EnumMember(Value = @"Info")]
            Info = 2,

            [System.Runtime.Serialization.EnumMember(Value = @"ErroreValidazione")]
            ErroreValidazione = 3,

        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class AreaEsitoBase
        {
            [Newtonsoft.Json.JsonProperty("codiceEsito", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CodiceEsito { get; set; }

            [Newtonsoft.Json.JsonProperty("messaggio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Messaggio { get; set; }


        }
    }
}
