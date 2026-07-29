using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Net;
using System.Text;
using INPS.Pensioni.Liquidazione.Service_Reference;
using System.Reflection;
using System.Linq;


namespace INPS.Pensioni.Liquidazione
{
    public class GestioneMsIndebiti
    {
        #region public

        public static void GetAnteprimaDebito(long numeroDomanda, string matricola, out RootIndebitoDto anteprimaIndebito)
        {
            anteprimaIndebito = null;
            try
            {
                GetAnteprimaDebitoPrivate(numeroDomanda, matricola, out anteprimaIndebito);
                return;
            }
            catch (Exception ex)
            {
                DNA.Logging.Logger.LogException(ex);
            }
        }

        public static void AggiornaCasuali(long numeroDomanda, string matricola, List<ContoRicDto> contiRic, out bool success)
        {
            try
            {
                success = AggiornaCasualiPrivate(numeroDomanda, matricola, contiRic);
                return;
            }
            catch (Exception ex)
            {
                DNA.Logging.Logger.LogException(ex);
                success = false;
            }
        }

        public static void NotificaTE08(long numeroDomanda, string matricola, bool accodaTE08IND, out bool success)
        {
            try
            {
                success = NotificaTE08Private(numeroDomanda, matricola, accodaTE08IND);
                return;
            }
            catch (Exception ex)
            {
                DNA.Logging.Logger.LogException(ex);
                success = false;
            }
        }
        #endregion

        #region private

        private static void GetAnteprimaDebitoPrivate(long numeroDomanda, string matricola, out RootIndebitoDto anteprimaIndebito)
        {
            anteprimaIndebito = new RootIndebitoDto();
            try
            {
                msIndebiti apiIndebiti = new msIndebiti(SetApiIdentity(matricola));

                Guid guid = Guid.NewGuid();
                anteprimaIndebito = apiIndebiti.GetAnteprimaDebito(numeroDomanda.ToString());
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
            }
        }

        private static bool AggiornaCasualiPrivate(long numeroDomanda, string matricola, List<ContoRicDto> contiRic)
        {
            try
            {
                msIndebiti apiIndebiti = new msIndebiti(SetApiIdentity(matricola));

                Guid guid = Guid.NewGuid();
                return apiIndebiti.AggiornaCausali(numeroDomanda, matricola, contiRic);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);

                return false;
            }
        }

        private static bool NotificaTE08Private(long numeroDomanda, string matricola, bool accodaTE08IND)
        {
            try
            {
                msIndebiti apiIndebiti = new msIndebiti(SetApiIdentity(matricola));

                Guid guid = Guid.NewGuid();
                return apiIndebiti.NotificaTE08(numeroDomanda, accodaTE08IND);
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);

                return false;
            }
        }

        #endregion

        private static WebClient SetApiIdentity(string matricola)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient client = new WebClient();
            string token = string.Empty;
            TokenIdentity identity = new TokenIdentity();
            identity.UserId = matricola ?? identity.UserId;
            var plainIdentity = JsonConvert.SerializeObject(identity);
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
                IdentityProvider = ConfigurationManager.AppSettings[Config_ApiScriwoProvider] != null ? ConfigurationManager.AppSettings[Config_ApiScriwoProvider].ToString() : string.Empty;
                UserId = ConfigurationManager.AppSettings[Config_ApiScriwoUserId] != null ? ConfigurationManager.AppSettings[Config_ApiScriwoUserId].ToString() : string.Empty;
                CorrelationId = Guid.NewGuid().ToString();
            }
            public string UserId { get; set; }
            public string IdentityProvider { get; set; }
            public string CorrelationId { get; set; }
        }

        private const string TokenHeader = "eyJhbGciOiJub25lIn0";
        private const string TokenBearer = "Bearer ";
        private const string ApiClientId = "X-IBM-Client-Id";
        private const string ApiClientSecret = "X-IBM-Client-Secret";
        private const string ApiAuthorization = "Authorization";
        private const string Config_ApiClientId = "ApiClientId";
        private const string Config_ApiClientSecret = "ApiClientSecret";
        private const string Config_ApiScriwoProvider = "ApiScriwoProvider";
        private const string Config_ApiScriwoUserId = "ApiScriwoUserId";
    }
}
