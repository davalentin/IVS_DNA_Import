using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Service_Reference
{
    public class msScriwoClient
    {
        public msScriwoClient(WebClient httpClient)
        {
            _httpClient = httpClient;
        }

        private const string Config_ApiScriwoUrl = "ApiScriwoUrl";
        private WebClient _httpClient;
        private string _baseUrl = ConfigurationManager.AppSettings[Config_ApiScriwoUrl];
        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }
        public void AggiornaStatoLavorazione(StatoLavorazioneRequest body)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/msScriwo/AggiornaStatoLavorazione");
            _httpClient.BaseAddress = BaseUrl;
            string jsonBody = JsonConvert.SerializeObject(body);
            string result = Encoding.ASCII.GetString(_httpClient.UploadData(urlBuilder_.ToString(), "POST", Encoding.Default.GetBytes(jsonBody)));
            _httpClient.Dispose();

            StatoLavorazioneResponse output = JsonConvert.DeserializeObject<StatoLavorazioneResponse>(result);
            if (output != null && output.Esito.CodiceEsito == "KO")
                throw new Exception(output.Esito.Messaggio);
            return;
        }
        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class StatoLavorazioneRequest
        {
            [Newtonsoft.Json.JsonProperty("tipoLavorazione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
            public TipoLavorazione TipoLavorazione { get; set; }

            [Newtonsoft.Json.JsonProperty("chiaveLavorazione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string ChiaveLavorazione { get; set; }

            [Newtonsoft.Json.JsonProperty("sistemaChiamante", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string SistemaChiamante { get; set; }

            [Newtonsoft.Json.JsonProperty("lavorazioni", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.Collections.Generic.ICollection<Lavorazione> Lavorazioni { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class StatoLavorazioneResponse
        {
            [Newtonsoft.Json.JsonProperty("esito", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public AreaEsitoBase Esito { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public enum TipoLavorazione
        {
            [System.Runtime.Serialization.EnumMember(Value = @"Domanda")]
            Domanda = 0,

        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class Lavorazione
        {
            [Newtonsoft.Json.JsonProperty("infoScriwo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public InfoScriwo InfoScriwo { get; set; }

            [Newtonsoft.Json.JsonProperty("infoSistemaChiamante", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public InfoSistemaChiamante InfoSistemaChiamante { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class InfoScriwo
        {
            [Newtonsoft.Json.JsonProperty("stepScriwo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
            public StepScriwo StepScriwo { get; set; }

            [Newtonsoft.Json.JsonProperty("statoScriwo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
            public StatoScriwo StatoScriwo { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class InfoSistemaChiamante
        {
            [Newtonsoft.Json.JsonProperty("codiceStato", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CodiceStato { get; set; }

            [Newtonsoft.Json.JsonProperty("descrizioneStato", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string DescrizioneStato { get; set; }

            [Newtonsoft.Json.JsonProperty("matricola", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Matricola { get; set; }

            [Newtonsoft.Json.JsonProperty("dataLavorazione", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.DateTime DataLavorazione { get; set; }

            [Newtonsoft.Json.JsonProperty("progFase", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string ProgFase { get; set; }

            [Newtonsoft.Json.JsonProperty("isProvvisoria", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public bool? IsProvvisoria { get; set; }

            [Newtonsoft.Json.JsonProperty("datoAggiuntivo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public System.Collections.Generic.ICollection<DatiAggiuntivi> DatoAggiuntivo { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class AreaEsitoBase
        {
            [Newtonsoft.Json.JsonProperty("codiceEsito", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string CodiceEsito { get; set; }

            [Newtonsoft.Json.JsonProperty("messaggio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Messaggio { get; set; }


        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public enum StepScriwo
        {
            [System.Runtime.Serialization.EnumMember(Value = @"VerificaDomanda")]
            VerificaDomanda = 0,

            [System.Runtime.Serialization.EnumMember(Value = @"GestioneEstero")]
            GestioneEstero = 1,

            [System.Runtime.Serialization.EnumMember(Value = @"GestioneConto")]
            GestioneConto = 2,

            [System.Runtime.Serialization.EnumMember(Value = @"VerificaDiritto")]
            VerificaDiritto = 3,

            [System.Runtime.Serialization.EnumMember(Value = @"CalcoloMisura")]
            CalcoloMisura = 4,

            [System.Runtime.Serialization.EnumMember(Value = @"CalcoloPensione")]
            CalcoloPensione = 5,

            [System.Runtime.Serialization.EnumMember(Value = @"Liquidazione")]
            Liquidazione = 6,

            [System.Runtime.Serialization.EnumMember(Value = @"Acquisizione")]
            Acquisizione = 7,

            [System.Runtime.Serialization.EnumMember(Value = @"AccettazioneDomanda")]
            AccettazioneDomanda = 8,

            [System.Runtime.Serialization.EnumMember(Value = @"CalcoloPAL")]
            CalcoloPAL = 9,

            [System.Runtime.Serialization.EnumMember(Value = @"VerificaCalcoloPAL")]
            VerificaCalcoloPAL = 10,

            [System.Runtime.Serialization.EnumMember(Value = @"Approvazione")]
            Approvazione = 11,

            [System.Runtime.Serialization.EnumMember(Value = @"Determina")]
            Determina = 12,

        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public enum StatoScriwo
        {
            [System.Runtime.Serialization.EnumMember(Value = @"Disattivo")]
            Disattivo = 0,

            [System.Runtime.Serialization.EnumMember(Value = @"NonAvviato")]
            NonAvviato = 1,

            [System.Runtime.Serialization.EnumMember(Value = @"InElaborazione")]
            InElaborazione = 2,

            [System.Runtime.Serialization.EnumMember(Value = @"Completato")]
            Completato = 3,

        }

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.22.0 (Newtonsoft.Json v11.0.0.0)")]
        public partial class DatiAggiuntivi
        {
            [Newtonsoft.Json.JsonProperty("chiave", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Chiave { get; set; }

            [Newtonsoft.Json.JsonProperty("valore", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Valore { get; set; }


        }
    }
}
