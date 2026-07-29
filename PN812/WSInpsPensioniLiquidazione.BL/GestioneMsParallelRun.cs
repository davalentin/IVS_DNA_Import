using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Service_Reference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneMsParallelRun
    {

        public static void TrasferimentoPensione(long nDomus, string categoriaPensione, string codiceSede, string numCertificato, string matricola)
        {
            try
            {
                TrasferimentoPensionePrivate(nDomus, categoriaPensione, codiceSede, numCertificato, matricola);
                return;
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(nDomus, "TrasferimentoPensione", Utility.TipoLogGenerico.ErroreApplicativo, ex.Message,
                    null, ex.StackTrace);
            }
        }



        #region private
        private static void TrasferimentoPensionePrivate(long nDomus, string categoriaPensione, string codiceSede, string numCertificato, string matricola)
        {
            var request = new msParallelRunClient.ChiavePensioneDTO()
            {
                DecCategPens = categoriaPensione,
                CodSede = codiceSede,
                NumCert = numCertificato
            };

            Guid guid = Guid.NewGuid();
            GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvParallelRun, Utility.MetodoServizio.TrasferimentoPensione, 
                Utility.SOAPLogDirection.IN, nDomus.ToString() , guid);

            var client = new msParallelRunClient(new WebClient());
            Dictionary<string, string> headers = SetApiIdentityBis(matricola, "");

            var response = client.TrasferimentoPensione(request, headers);

            GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvParallelRun, Utility.MetodoServizio.TrasferimentoPensione,
                Utility.SOAPLogDirection.OUT, nDomus.ToString(), guid);
        }

        private static Dictionary<string, string> SetApiIdentityBis(string matricola, string servizio)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // TLS 1.2
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string token = string.Empty;
            TokenIdentity identity = new TokenIdentity { UserId = matricola ?? string.Empty };
            var plainIdentity = JsonConvert.SerializeObject(identity);
            var encoding = Encoding.UTF8;
            var bytesIdentity = encoding.GetBytes(plainIdentity);
            token = string.Concat(TokenBearer, TokenHeader, ".", Convert.ToBase64String(bytesIdentity), ".");

            // Creiamo un dizionario con gli header
            var headers = new Dictionary<string, string>();
           
            headers[ApiClientId] = ConfigurationManager.AppSettings[Config_ApiClientId] ?? string.Empty;
            headers[ApiClientSecret] = ConfigurationManager.AppSettings[Config_ApiClientSecret] ?? string.Empty;
    
            headers[ApiAuthorization] = token;

            return headers;
        }

        private class TokenIdentity
        {
            public TokenIdentity()
            {
                IdentityProvider = ConfigurationManager.AppSettings[Config_ApiParallelRunProvider] != null ? ConfigurationManager.AppSettings[Config_ApiParallelRunProvider].ToString() : string.Empty;
                UserId = ConfigurationManager.AppSettings[Config_ApiParallelRunUserId] != null ? ConfigurationManager.AppSettings[Config_ApiParallelRunUserId].ToString() : string.Empty;
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
        private const string Config_ApiParallelRunProvider = "ApiNuovoCalcoloProvider";
        private const string Config_ApiParallelRunUserId = "ApiNuovoCalcoloUserId";

        #endregion

    }
}
