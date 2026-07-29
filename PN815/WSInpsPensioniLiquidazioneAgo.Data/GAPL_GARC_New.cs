using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using INPS.DNA.Data.HostIntegration;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using INPS.Pensioni.LiquidazioneAgo.Data.HostRequest;
using INPS.Pensioni.LiquidazioneAgo.Data.HostResponse;
using System.Net;
using System.Configuration;
using Newtonsoft.Json;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazioneAgo.Data
{
    /// <summary>
    /// Invoca la transazione GAPL_GARC_New: effettua la prenotazione e la sprenotazione di una ricostituzione
    /// </summary>
    public class GAPL_GARC_New : ITransactionInfo
    {
        private HisLiquidazioneAgo.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe GAPL_GARC
        /// </summary>

        public GAPL_GARC_New(string transazione)
        {
            this.RequestNew = new HostRequest.GAPL_GARCRequestNew();

            TransactionName = transazione;
            try
            {
                SetHisContext();
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Impossibile impostare il contesto di His", ex);
            }
        }
        #endregion Constructor

        #region Tracciato Host
        [HisComplexAreaInfoMapping(0, Direction = HostDirection.Input)]
        public HostRequest.GAPL_GARCRequestNew RequestNew { get; set; }

        [HisComplexAreaInfoMapping(1, Direction = HostDirection.Output)]
        public HostResponse.GAPL_GARCResponse Response { get; set; }
        #endregion Tracciato Host

        #region Properties
        public string Messaggio { get; set; }
        public string MessaggioDaLoggare { get; set; }
        public List<short> CodiciErrore { get; private set; }

        public bool HasError { get; set; }

        #endregion Properties

        #region ITransactionInfo Members

        public string TransactionName
        {
            get;
            private set;
        }

        #endregion ITransactionInfo Members

        public void Invoke()
        {
            try
            {
                //Conversione dell'area di input
                byte[] inputData = HostTransactionManager.AreaToHost<GAPL_GARC_New>(this);

                HisLiquidazioneAgo.LiquidazioneAgoClient proxy = new HisLiquidazioneAgo.LiquidazioneAgoClient();
                byte[] output = null;
                if (TransactionName == "GAPL")
                {
                    output = proxy.GAPL(inputData, ref _ClientContext);
                }
                else if (TransactionName == "GARC" || TransactionName == "CARC")
                {
                    if (inputData == null)
                        throw new INPS.DNA.DnaApplicationException("Area di input nulla - " + TransactionName);
                    byte[] newInputData = new byte[inputData.Length + 8];
                    inputData.CopyTo(newInputData, 8);
                    for (int i = 0; i < 8; i++)
                        newInputData[i] = 64;
                    newInputData[0] = 0x4B;
                    newInputData[1] = 0x4B;
                    newInputData[2] = 0x4B;

                    if (TransactionName == "CARC")
                        output = proxy.CARC(newInputData, ref _ClientContext);
                    else
                        output = proxy.GARC(newInputData, ref _ClientContext);
                }
                else
                    return;
                //Gestione errori -  Gestione dell'abend: il messaggio comincia con 'DFS'
                if ((output[0] == 0xC4 && output[1] == 0xC6 && output[2] == 0xE2) || (output[1] == 0xC4 && output[2] == 0xC6 && output[3] == 0xE2))
                {
                    HasError = true;

                    byte[] data = output;
                    if (output.Length > 155)
                    {
                        data = new byte[155];
                        Buffer.BlockCopy(output, 0, data, 0, 155);
                    }
                    MessaggioDaLoggare = INPS.DNA.Data.HostIntegration.Conversion.ASCII.GetString(data);
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "KO: ERRORE DURANTE IL COLLOQUIO CON IL DATA BASE (errore 21). SE L’ERRORE CONTINUA, PREGASI SEGNALARE ALL'HELP DESK";
                    return;
                }

                //Conversione dell'area di output
                HostTransactionManager.AreaFromHost<GAPL_GARC_New>(this, output);

                DecodificaCodiceRitorno();

            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new INPS.DNA.DnaApplicationException("Puntamento errato al servizio His TI_PNL_R - " + TransactionName, ex);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                HasError = true;

                if (ex.Message.Contains("STOPPED"))
                {
                    INPS.DNA.Logging.Logger.LogException(ex);
                    MessaggioDaLoggare = "Transazione stoppata";
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "KO: ERRORE DURANTE IL COLLOQUIO CON IL DATA BASE (errore 21). SE L’ERRORE CONTINUA, PREGASI SEGNALARE ALL'HELP DESK";
                    return;
                }
                else if (ex.Message.Contains("IMS error message text:"))
                {
                    INPS.DNA.Logging.Logger.LogException(ex);
                    MessaggioDaLoggare = "Transazione in abend - " + ex.Message.Substring(ex.Message.LastIndexOf("IMS error message text:", StringComparison.InvariantCulture));
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "KO: ERRORE DURANTE IL COLLOQUIO CON IL DATA BASE (errore 21). SE L’ERRORE CONTINUA, PREGASI SEGNALARE ALL'HELP DESK";
                    return;
                }
                else
                    throw new INPS.DNA.DnaApplicationException("Errore di comunicazione con il servizio His TI_PNL_R - " + TransactionName, ex);
            }
            catch
            {
                throw;
            }
        }

        #region Private
        private void SetHisContext()
        {
            _ClientContext = new HisLiquidazioneAgo.ClientContext();
            HisContext hisContext = new HisContext(this.TransactionName != "CARC" ? this.TransactionName : "GARC");
            _ClientContext.User = hisContext.ImsUser;
            if (_ClientContext.User.Length == 4)
                _ClientContext.User += INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.Substring(0, 4);
            _ClientContext.Password = hisContext.ImsPassword;
        }

        private void DecodificaCodiceRitorno()
        {
            CodiciErrore = new List<short>();
            StringBuilder messaggio = new StringBuilder();

            switch (this.Response.Controllo.PER_CODTECNICI)
            {
                case 0:
                    messaggio.Append("Calcolo eseguito correttamente");
                    break;
                default:
                    if (this.Response.Controllo.PER_CODESITO == "S" && this.Response.Controllo.LISTPER_TABERR != null &&
                        this.Response.Controllo.LISTPER_TABERR.Count > 0)
                    {
                        foreach (GAPL_GARCResponse.AreaControllo.PER_TABERR per_err in this.Response.Controllo.LISTPER_TABERR)
                        {
                            if (per_err.PER_ERR != 0)
                            {
                                if (String.IsNullOrEmpty(messaggio.ToString()))
                                    messaggio.Append("CODICI ERRORE: ");
                                else
                                    messaggio.Append(" - ");
                                messaggio.Append(per_err.PER_ERR.ToString());
                                CodiciErrore.Add(per_err.PER_ERR);

                                if (per_err.PER_ERR == 851 || per_err.PER_ERR == 852)
                                {
                                    if (this.Response.Controllo.LISTPER_TABERR_QRED != null && this.Response.Controllo.LISTPER_TABERR_QRED.Count > 0)
                                    {

                                        foreach (GAPL_GARCResponse.AreaControllo.PER_TABERR_QRED per_qred in this.Response.Controllo.LISTPER_TABERR_QRED)
                                        {
                                            if (per_qred.PER_ANNOERR_QRED != 0)
                                            {
                                                messaggio.Append(" CAT.: " + per_qred.PER_KEYERR_QRED.ToString().PadLeft(15, '0').Substring(0, 3));
                                                messaggio.Append(" SEDE: " + per_qred.PER_KEYERR_QRED.ToString().PadLeft(15, '0').Substring(3, 4));
                                                messaggio.Append(" CERTIFICATO: " + per_qred.PER_KEYERR_QRED.ToString().PadLeft(15, '0').Substring(7));
                                                messaggio.Append(" ANNO: " + per_qred.PER_ANNOERR_QRED.ToString());
                                                messaggio.Append(" TIPO ERRORE: " + per_qred.PER_TIPOERR_QRED);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        if (String.IsNullOrEmpty(messaggio.ToString()))
                        {
                            messaggio.Append("ERRORE TECNICO - CODICE ERRORE " + this.Response.Controllo.PER_CODTECNICI.ToString());
                        }
                        if (!string.IsNullOrEmpty(this.Response.Controllo.WEB_COD) || !string.IsNullOrEmpty(this.Response.Controllo.WEB_ERR))
                        {
                            messaggio.Append(". ERRORE WEBDOM: " + this.Response.Controllo.WEB_COD.Trim() + " - " + this.Response.Controllo.WEB_ERR.Trim());
                        }
                    }
                    else
                    {
                        messaggio.Append("ERRORE TECNICO - CODICE ERRORE " + this.Response.Controllo.PER_CODTECNICI.ToString());
                        if (!string.IsNullOrEmpty(this.Response.Controllo.WEB_COD) || !string.IsNullOrEmpty(this.Response.Controllo.WEB_ERR))
                        {
                            messaggio.Append(". ERRORE WEBDOM: " + this.Response.Controllo.WEB_COD.Trim() + " - " + this.Response.Controllo.WEB_ERR.Trim());
                        }
                    }
                    break;
            }
            Messaggio = messaggio.ToString();
        }

        #endregion Private

        private static WebClient SetApiIdentity(string matricola, string servizio)
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
            if (servizio == "QualityDataChecker")
            {
                client.Headers.Add(ApiClientId, ConfigurationManager.AppSettings[Config_ApiClientIdQualityDataChecker] ?? string.Empty);
                client.Headers.Add(ApiClientSecret, ConfigurationManager.AppSettings[Config_ApiClientSecretQualityDataChecker] ?? string.Empty);
            }
            else
            {
                client.Headers.Add(ApiClientId, ConfigurationManager.AppSettings[Config_ApiClientId] ?? string.Empty);
                client.Headers.Add(ApiClientSecret, ConfigurationManager.AppSettings[Config_ApiClientSecret] ?? string.Empty);
            }
            client.Headers.Add(ApiAuthorization, token);
            client.Headers.Add("Content-Type", "application/json");

            return client;
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

            if (servizio == "QualityDataChecker")
            {
                headers[ApiClientId] = ConfigurationManager.AppSettings[Config_ApiClientIdQualityDataChecker] ?? string.Empty;
                headers[ApiClientSecret] = ConfigurationManager.AppSettings[Config_ApiClientSecretQualityDataChecker] ?? string.Empty;
            }
            else
            {
                headers[ApiClientId] = ConfigurationManager.AppSettings[Config_ApiClientId] ?? string.Empty;
                headers[ApiClientSecret] = ConfigurationManager.AppSettings[Config_ApiClientSecret] ?? string.Empty;
            }

            headers[ApiAuthorization] = token;

            return headers;
        }

        public string CallMiddleware(GestionePensione.DatiPensione datiPensione, string descComuneNazione, List<GestioneAnagrafica.DatiAnagrafici> ListaAnagraficaFamiliari, out string jsonStringRequest, out string errori, out string codiciErrore, out string eccezioni, out string jsonStringResponse)
        {
            string transactionId = string.Empty;
            jsonStringRequest = string.Empty;
            errori = string.Empty;
            codiciErrore = string.Empty;
            eccezioni = string.Empty;
            jsonStringResponse = string.Empty;
            try
            {
                swaggerMiddlewareClient client = new swaggerMiddlewareClient(SetApiIdentity(this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(), ""));
                swaggerMiddlewareClient.RequestDTO requestDTO = new swaggerMiddlewareClient.RequestDTO();

                #region request
                requestDTO.Request = new swaggerMiddlewareClient.GaplGarcRequestDTO()
                {
                    Bititolarieta = new swaggerMiddlewareClient.BititolarietaRequestDTO() { LISTT_GP2A15 = new List<swaggerMiddlewareClient.TabBitRequestDTO>() },
                    Coda = new swaggerMiddlewareClient.CodaRequestDTO()
                    {
                        Dati2006 = new swaggerMiddlewareClient.Dati2006RequestDTO(),
                        Dati2007 = new swaggerMiddlewareClient.Dati2007RequestDTO()
                        {
                            LISTT_ELTAB_GP7LC = new List<swaggerMiddlewareClient.ElemTabRequestDTO>()
                        },
                        Dati2008 = new swaggerMiddlewareClient.Dati2008RequestDTO()
                        {
                            LISTT_ELTAB_GP2PB = new List<swaggerMiddlewareClient.EltabRequestDTO>()
                        },
                        Dati2009 = new swaggerMiddlewareClient.Dati2009RequestDTO(),
                        Dati2010 = new swaggerMiddlewareClient.Dati2010RequestDTO(),
                        Dati2011 = new swaggerMiddlewareClient.Dati2011RequestDTO(),
                        Dati2012 = new swaggerMiddlewareClient.Dati2012RequestDTO()
                        {
                            LISTT_GP2BM10 = new List<swaggerMiddlewareClient.DatiAttLavRequestDTO>()
                        },
                        Dati2013 = new swaggerMiddlewareClient.Dati2013RequestDTO()
                        {
                            LISTT_GP2IC30 = new List<swaggerMiddlewareClient.PerPermEsteroRequestDTO>(),
                            LISTT_GP2IC40 = new List<swaggerMiddlewareClient.IndennFreqPerScolRequestDTO>()
                        },
                        Dati2014 = new swaggerMiddlewareClient.Dati2014RequestDTO()
                        {
                            LISTT_TABTRATTOT = new List<swaggerMiddlewareClient.TabTratTotRequestDTO>()
                        },
                        Dati2015 = new swaggerMiddlewareClient.Dati2015RequestDTO()
                        {
                            LISTT_TABINGR = new List<swaggerMiddlewareClient.TabIngrRequestDTO>()
                        },
                        Dati2016 = new swaggerMiddlewareClient.Dati2016RequestDTO()
                        {
                            LISTT_GP1FLAGS = new List<swaggerMiddlewareClient.CodVarDecPensRequestDTO>()
                        },
                        Dati2017 = new swaggerMiddlewareClient.Dati2017RequestDTO(),
                        Dati2018 = new swaggerMiddlewareClient.Dati2018RequestDTO(),
                        Dati2019 = new swaggerMiddlewareClient.Dati2019RequestDTO(),
                        Dati2020 = new swaggerMiddlewareClient.Dati2020RequestDTO(),
                        Dati2021 = new swaggerMiddlewareClient.Dati2021RequestDTO()
                    },
                    DanteCausa = new swaggerMiddlewareClient.DanteCausaRequestDTO(),
                    DatiGenerici = new swaggerMiddlewareClient.DatiGenericiRequestDTO(),
                    DatiNuovi = new swaggerMiddlewareClient.DatiNuoviRequestDTO(),
                    DatiRetributivi_Contributivi = new swaggerMiddlewareClient.DatiRetributiviContributiviRequestDTO()
                    {
                        LISTT_GP2BC00 = new List<swaggerMiddlewareClient.TabPensRetrRequestDTO>()
                    },
                    Delegato = new swaggerMiddlewareClient.DelegatoRequestDTO(),
                    Errori = new swaggerMiddlewareClient.ErroriRequestDTO(),
                    Familiari = new swaggerMiddlewareClient.FamiliariRequestDTO()
                    {
                        LISTT_GP3 = new List<swaggerMiddlewareClient.TabFamRequestDTO>()
                    },
                    INAIL_Accompagnamento = new swaggerMiddlewareClient.InailAccompagnamentoRequestDTO() { LISTT_GP2BINA = new List<swaggerMiddlewareClient.TabInailRequestDTO>() },
                    IntegrazioneArticolo11 = new swaggerMiddlewareClient.IntegrazioneArticolo11RequestDTO() { LISTGPINTAR11 = new List<swaggerMiddlewareClient.GpintarRequestDTO>() },
                    Intestazione = new swaggerMiddlewareClient.IntestazioneRequestDTO(),
                    Invciv = new swaggerMiddlewareClient.InvcivRequestDTO() { LISTT_GP2IC10 = new List<swaggerMiddlewareClient.Gp2ic10RequestDTO>() },
                    Istruttoria = new swaggerMiddlewareClient.IstruttoriaRequestDTO()
                    {
                        LISTT_GP2BG10 = new List<swaggerMiddlewareClient.TabSindacatoRequestDTO>()
                    },
                    Pagamento = new swaggerMiddlewareClient.PagamentoRequestDTO(),
                    PannelloContributivo = new swaggerMiddlewareClient.PannelloContributivoRequestDTO()
                    {
                        LISTT_GP2BB03 = new List<swaggerMiddlewareClient.TabPensContrRequestDTO>()
                    },
                    Pensionato = new swaggerMiddlewareClient.PensionatoRequestDTO(),
                    PensioniAbbinate = new swaggerMiddlewareClient.PensioniAbbinateRequestDTO(),
                    Redditi = new swaggerMiddlewareClient.RedditiRequestDTO()
                    {
                        RedditiMaggiorazione = new swaggerMiddlewareClient.MaggiorazioneRequestDTO() { LISTT_GP2KM50 = new List<swaggerMiddlewareClient.GP2KM50RequestDTO>() },
                        RedditiSentenza240_94 = new swaggerMiddlewareClient.Sentenza24094RequestDTO(),
                        RedditiSentenza495_93 = new swaggerMiddlewareClient.Sentenza49593RequestDTO()
                    },
                    ResidenzeEstero = new swaggerMiddlewareClient.ResidenzeEsteroRequestDTO() { LISTT_GP2BS00 = new List<swaggerMiddlewareClient.VarResEsteroRequestDTO>() },
                    Ricoveri = new swaggerMiddlewareClient.RicoveriRequestDTO() { LISTT_GP2IC20 = new List<swaggerMiddlewareClient.Gp2ic20RequestDTO>() },
                    Sentenze = new swaggerMiddlewareClient.SentenzeRequestDTO() { LISTT_GP2SEN0 = new List<swaggerMiddlewareClient.TabSentenzeRequestDTO>() },
                    StatoCivile = new swaggerMiddlewareClient.StatoCivileRequestDTO()
                    {
                        LISTT_GP2KM7A = new List<swaggerMiddlewareClient.StatoCivPensRequestDTO>()
                    },
                    Supplementi = new swaggerMiddlewareClient.SupplementiRequestDTO() { LISTT_GP2BE00 = new List<swaggerMiddlewareClient.Gp2be00RequestDTO>() },
                    Tutore = new swaggerMiddlewareClient.TutoreRequestDTO(),
                    SPRDSC21 = new swaggerMiddlewareClient.Sprdsc21RequestDTO(),
                    DatiRetributiviBIS = new swaggerMiddlewareClient.DatiRetributiviBisRequestDTO()
                    {
                        LISTT_GP2BC00_BIS = new List<swaggerMiddlewareClient.TabPensRetributivaRequestDTO>()
                    }
                };
                requestDTO.Request.Intestazione = new swaggerMiddlewareClient.IntestazioneRequestDTO();

                Utility.ValorizzaOggettiBis(this.RequestNew.Bititolarieta, requestDTO.Request.Bititolarieta);
                if (this.RequestNew.Bititolarieta.LISTT_GP2A15 != null)
                {
                    foreach (var req in this.RequestNew.Bititolarieta.LISTT_GP2A15)
                    {
                        var res = new swaggerMiddlewareClient.TabBitRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Bititolarieta.LISTT_GP2A15.Add(res);
                    }
                }
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda, requestDTO.Request.Coda);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2006, requestDTO.Request.Coda.Dati2006);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2007, requestDTO.Request.Coda.Dati2007);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2008, requestDTO.Request.Coda.Dati2008);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2009, requestDTO.Request.Coda.Dati2009);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2010, requestDTO.Request.Coda.Dati2010);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2011, requestDTO.Request.Coda.Dati2011);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2012, requestDTO.Request.Coda.Dati2012);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2013, requestDTO.Request.Coda.Dati2013);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2014, requestDTO.Request.Coda.Dati2014);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2015, requestDTO.Request.Coda.Dati2015);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2016, requestDTO.Request.Coda.Dati2016);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2017, requestDTO.Request.Coda.Dati2017);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2018, requestDTO.Request.Coda.Dati2018);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2019, requestDTO.Request.Coda.Dati2019);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2020, requestDTO.Request.Coda.Dati2020);
                Utility.ValorizzaOggettiBis(this.RequestNew.Coda.AreaDati2021, requestDTO.Request.Coda.Dati2021);
                //TODO mappare novi dati 2024

                if (this.RequestNew.Coda != null)
                {
                    if (this.RequestNew.Coda.AreaDati2007 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2007.LISTT_ELTAB_GP7LC != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2007.LISTT_ELTAB_GP7LC)
                            {
                                var res = new swaggerMiddlewareClient.ElemTabRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2007.LISTT_ELTAB_GP7LC.Add(res);
                            }
                        }
                    }
                    if (this.RequestNew.Coda.AreaDati2008 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2008.LISTT_ELTAB_GP2PB != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2008.LISTT_ELTAB_GP2PB)
                            {
                                var res = new swaggerMiddlewareClient.EltabRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2008.LISTT_ELTAB_GP2PB.Add(res);
                            }
                        }
                    }

                    if (this.RequestNew.Coda.AreaDati2012 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2012.LISTT_GP2BM10 != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2012.LISTT_GP2BM10)
                            {
                                var res = new swaggerMiddlewareClient.DatiAttLavRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2012.LISTT_GP2BM10.Add(res);
                            }
                        }
                    }

                    if (this.RequestNew.Coda.AreaDati2013 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2013.LISTT_GP2IC30 != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2013.LISTT_GP2IC30)
                            {
                                var res = new swaggerMiddlewareClient.PerPermEsteroRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2013.LISTT_GP2IC30.Add(res);
                            }
                        }

                        if (this.RequestNew.Coda.AreaDati2013.LISTT_GP2IC40 != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2013.LISTT_GP2IC40)
                            {
                                var res = new swaggerMiddlewareClient.IndennFreqPerScolRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2013.LISTT_GP2IC40.Add(res);
                            }
                        }
                    }

                    if (this.RequestNew.Coda.AreaDati2014 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2014.LISTT_TABTRATTOT != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2014.LISTT_TABTRATTOT)
                            {
                                var res = new swaggerMiddlewareClient.TabTratTotRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2014.LISTT_TABTRATTOT.Add(res);
                            }
                        }
                    }

                    if (this.RequestNew.Coda.AreaDati2015 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2015.LISTT_TABINGR != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2015.LISTT_TABINGR)
                            {
                                var res = new swaggerMiddlewareClient.TabIngrRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2015.LISTT_TABINGR.Add(res);
                            }
                        }
                    }

                    if (this.RequestNew.Coda.AreaDati2016 != null)
                    {
                        if (this.RequestNew.Coda.AreaDati2016.LISTGP1FLAGS != null)
                        {
                            foreach (var req in this.RequestNew.Coda.AreaDati2016.LISTGP1FLAGS)
                            {
                                var res = new swaggerMiddlewareClient.CodVarDecPensRequestDTO();
                                Utility.ValorizzaOggettiBis(req, res);
                                requestDTO.Request.Coda.Dati2016.LISTT_GP1FLAGS.Add(res);
                            }
                        }
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.DanteCausa, requestDTO.Request.DanteCausa);
                Utility.ValorizzaOggettiBis(this.RequestNew.DatiGenerici, requestDTO.Request.DatiGenerici);
                Utility.ValorizzaOggettiBis(this.RequestNew.DatiNuovi, requestDTO.Request.DatiNuovi);

                //Utility.ValorizzaOggettiBis(this.Request.DatiRetributivi_Contributivi, requestDTO.Request.DatiRetributivi_Contributivi);
                if (this.RequestNew.DatiRetributivi_Contributivi.LISTT_GP2BC00 != null)
                {
                    foreach (var req in this.RequestNew.DatiRetributivi_Contributivi.LISTT_GP2BC00)
                    {
                        var res = new swaggerMiddlewareClient.TabPensRetrRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.DatiRetributivi_Contributivi.LISTT_GP2BC00.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Delegato, requestDTO.Request.Delegato);
                Utility.ValorizzaOggettiBis(this.RequestNew.Errori, requestDTO.Request.Errori);

                //Utility.ValorizzaOggettiBis(this.Request.Familiari, requestDTO.Request.Familiari);
                if (this.RequestNew.Familiari.LISTT_GP3 != null)
                {
                    foreach (var req in this.RequestNew.Familiari.LISTT_GP3)
                    {
                        var res = new swaggerMiddlewareClient.TabFamRequestDTO();
                        res.LISTT_GP3CK = new List<swaggerMiddlewareClient.TabCodMaggRequestDTO>();
                        if (req.LISTT_GP3CK != null)
                        {
                            foreach (var req2 in req.LISTT_GP3CK)
                            {
                                var res2 = new swaggerMiddlewareClient.TabCodMaggRequestDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTT_GP3CK.Add(res2);
                            }
                        }
                        Utility.ValorizzaOggettiBis(req, res);

                        if (ListaAnagraficaFamiliari != null && ListaAnagraficaFamiliari.Count > 0)
                        {
                            GestioneDecodifica.StatoEstero statoEstero = null;
                            GestioneAnagrafica.DatiAnagrafici datiAnagFam = ListaAnagraficaFamiliari.Find(x => x.CodiceFiscale == res.T_GP3CB08);
                            if (datiAnagFam != null)
                            {
                                GestioneDecodifica.GetStatoEsteroPerCodiceCatastale(datiAnagFam.CodiceComuneNascita.Trim(), out statoEstero);
                                res.GP3STATONASCITA = statoEstero != null && statoEstero.Descrizione != null ? statoEstero.Descrizione.Trim() : "ITALIA";
                            }
                        }
                        requestDTO.Request.Familiari.LISTT_GP3.Add(res);
                    }
                }


                Utility.ValorizzaOggettiBis(this.RequestNew.INAIL_Accompagnamento, requestDTO.Request.INAIL_Accompagnamento);
                if (this.RequestNew.INAIL_Accompagnamento.LISTT_GP2BINA != null)
                {
                    foreach (var req in this.RequestNew.INAIL_Accompagnamento.LISTT_GP2BINA)
                    {
                        var res = new swaggerMiddlewareClient.TabInailRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.INAIL_Accompagnamento.LISTT_GP2BINA.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.IntegrazioneArticolo11, requestDTO.Request.IntegrazioneArticolo11);
                if (this.RequestNew.IntegrazioneArticolo11.LISTGPINTAR11 != null)
                {
                    foreach (var req in this.RequestNew.IntegrazioneArticolo11.LISTGPINTAR11)
                    {
                        var res = new swaggerMiddlewareClient.GpintarRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.IntegrazioneArticolo11.LISTGPINTAR11.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Intestazione, requestDTO.Request.Intestazione);

                Utility.ValorizzaOggettiBis(this.RequestNew.Invciv, requestDTO.Request.Invciv);
                if (this.RequestNew.Invciv.LISTT_GP2IC10 != null)
                {
                    foreach (var req in this.RequestNew.Invciv.LISTT_GP2IC10)
                    {
                        var res = new swaggerMiddlewareClient.Gp2ic10RequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Invciv.LISTT_GP2IC10.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Istruttoria, requestDTO.Request.Istruttoria);
                if (this.RequestNew.Istruttoria.LISTT_GP2BG10 != null)
                {
                    foreach (var req in this.RequestNew.Istruttoria.LISTT_GP2BG10)
                    {
                        var res = new swaggerMiddlewareClient.TabSindacatoRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Istruttoria.LISTT_GP2BG10.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Pagamento, requestDTO.Request.Pagamento);

                Utility.ValorizzaOggettiBis(this.RequestNew.PannelloContributivo, requestDTO.Request.PannelloContributivo);
                if (this.RequestNew.PannelloContributivo.LISTT_GP2BB03 != null)
                {
                    foreach (var req in this.RequestNew.PannelloContributivo.LISTT_GP2BB03)
                    {
                        var res = new swaggerMiddlewareClient.TabPensContrRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.PannelloContributivo.LISTT_GP2BB03.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Pensionato, requestDTO.Request.Pensionato);
                requestDTO.Request.Pensionato.GP3STATONASCITA = descComuneNazione;              

                Utility.ValorizzaOggettiBis(this.RequestNew.PensioniAbbinate, requestDTO.Request.PensioniAbbinate);

                Utility.ValorizzaOggettiBis(this.RequestNew.Redditi.RedditiMaggiorazione, requestDTO.Request.Redditi.RedditiMaggiorazione);
                if (this.RequestNew.Redditi.RedditiMaggiorazione.LISTT_GP2KM50 != null)
                {
                    foreach (var req in this.RequestNew.Redditi.RedditiMaggiorazione.LISTT_GP2KM50)
                    {
                        var res = new swaggerMiddlewareClient.GP2KM50RequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Redditi.RedditiMaggiorazione.LISTT_GP2KM50.Add(res);
                    }
                }
                if (this.RequestNew.Redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z != null)
                {
                    foreach (var req in this.RequestNew.Redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z)
                    {
                        var res = new swaggerMiddlewareClient.GP7LKE0ZRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Redditi.RedditiSentenza495_93.LISTT_GP7LKE0Z.Add(res);
                    }
                }
                if (this.RequestNew.Redditi.RedditiSentenza240_94.LISTT_GP2RS00 != null)
                {
                    foreach (var req in this.RequestNew.Redditi.RedditiSentenza240_94.LISTT_GP2RS00)
                    {
                        var res = new swaggerMiddlewareClient.GP2RS00RequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Redditi.RedditiSentenza240_94.LISTT_GP2RS00.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.ResidenzeEstero, requestDTO.Request.ResidenzeEstero);
                if (this.RequestNew.ResidenzeEstero.LISTT_GP2BS00 != null)
                {
                    foreach (var req in this.RequestNew.ResidenzeEstero.LISTT_GP2BS00)
                    {
                        var res = new swaggerMiddlewareClient.VarResEsteroRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.ResidenzeEstero.LISTT_GP2BS00.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Ricoveri, requestDTO.Request.Ricoveri);
                if (this.RequestNew.Ricoveri.LISTT_GP2IC20 != null)
                {
                    foreach (var req in this.RequestNew.Ricoveri.LISTT_GP2IC20)
                    {
                        var res = new swaggerMiddlewareClient.Gp2ic20RequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Ricoveri.LISTT_GP2IC20.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Sentenze, requestDTO.Request.Sentenze);
                if (this.RequestNew.Sentenze.LISTT_GP2SEN0 != null)
                {
                    foreach (var req in this.RequestNew.Sentenze.LISTT_GP2SEN0)
                    {
                        var res = new swaggerMiddlewareClient.TabSentenzeRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Sentenze.LISTT_GP2SEN0.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.Request.StatoCivile, requestDTO.Request.StatoCivile);
                if (this.RequestNew.StatoCivile.LISTT_GP2KM7A != null)
                {
                    foreach (var req in this.RequestNew.StatoCivile.LISTT_GP2KM7A)
                    {
                        var res = new swaggerMiddlewareClient.StatoCivPensRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.StatoCivile.LISTT_GP2KM7A.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Supplementi, requestDTO.Request.Supplementi);
                if (this.RequestNew.Supplementi.LISTT_GP2BE00 != null)
                {
                    foreach (var req in this.RequestNew.Supplementi.LISTT_GP2BE00)
                    {
                        var res = new swaggerMiddlewareClient.Gp2be00RequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Supplementi.LISTT_GP2BE00.Add(res);
                    }
                }

                Utility.ValorizzaOggettiBis(this.RequestNew.Tutore, requestDTO.Request.Tutore);

                Utility.ValorizzaOggettiBis(this.RequestNew.SPRDSC21, requestDTO.Request.SPRDSC21);
                if (this.RequestNew.SPRDSC21 != null && this.RequestNew.SPRDSC21.LISTT_GP4DB00 != null)
                {
                    requestDTO.Request.SPRDSC21.LISTT_GP4DB00 = new List<swaggerMiddlewareClient.Gp4db00RequestDTO>();
                    foreach (var req in this.RequestNew.SPRDSC21.LISTT_GP4DB00)
                    {
                        var res = new swaggerMiddlewareClient.Gp4db00RequestDTO();
                        res.LISTT_GP4DC00 = new List<swaggerMiddlewareClient.Gp4dc00RequestDTO>();
                        if (req.LISTT_GP4DC00 != null)
                        {
                            foreach (var req2 in req.LISTT_GP4DC00)
                            {
                                var res2 = new swaggerMiddlewareClient.Gp4dc00RequestDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTT_GP4DC00.Add(res2);
                            }
                        }
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.SPRDSC21.LISTT_GP4DB00.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.Request.DatiRetributiviBIS, requestDTO.Request.DatiRetributiviBIS);
                if (this.RequestNew.DatiRetributiviBIS.LISTT_GP2BC00_BIS != null)
                {
                    foreach (var req in this.RequestNew.DatiRetributiviBIS.LISTT_GP2BC00_BIS)
                    {
                        var res = new swaggerMiddlewareClient.TabPensRetributivaRequestDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.DatiRetributiviBIS.LISTT_GP2BC00_BIS.Add(res);
                    }
                }


                //requestDTO.FlowCode = "01";
                requestDTO.codGestione = datiPensione.Gestione;
                requestDTO.codFondo = datiPensione.Fondo;
                requestDTO.indConvInt = datiPensione.IndConvInt.GetValueOrDefault() ? "1" : "0";
                requestDTO.tipoRichiesta = datiPensione.FlagVerify.HasValue ? datiPensione.FlagVerify.Value ? "1" : "0" : "1";
                requestDTO.fase = Utility.IsRiaperturaDomanda(datiPensione.Id) ? "RIAPERTURA" : "NORIAPERTURA";
                requestDTO.User = this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString();

                #endregion request

                jsonStringRequest = JsonConvert.SerializeObject(requestDTO);

                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(requestDTO, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.IN, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);

                //var response = client.IvsInvocation(requestDTO);

                Dictionary<string, string> headers = SetApiIdentityBis(this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(), "");

                HttpStatusCode statusCode = HttpStatusCode.Continue;
                swaggerMiddlewareClient.ResponseDTO response = null;
                if (TransactionName == "GAPL") 
                {
                    response = client.IvsInvocation(requestDTO, headers, out statusCode);
                }
                else if (TransactionName == "GARC")
                {
                    response = client.IvsInvocationRic(requestDTO, headers, out statusCode);
                }

                GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.OUT, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);
                transactionId = response != null ? response.TransactionId : string.Empty;
                errori = response != null && response.Errors != null && response.Errors.Count > 0 ? string.Join(";", response.Errors.Select(e => !string.IsNullOrEmpty(e.Message) ? e.Code.ToString() + " " + e.Message.ToString() : "").ToArray()) : (!((int)statusCode >= 200 && (int)statusCode < 300) ? statusCode.ToString() : null);
                codiciErrore = response != null && response.Errors != null && response.Errors.Count > 0 ? string.Join(";", response.Errors.Select(e => e.Code.ToString()).ToArray()) : string.Empty;
                eccezioni = !((int)statusCode >= 200 && (int)statusCode < 300) ? statusCode.ToString() : null;
                jsonStringResponse = JsonConvert.SerializeObject(response);

                if (!string.IsNullOrEmpty(transactionId))
                {
                    GestioneNuovoCalcolo.UpdateScadutoEsistoNuovoCalcolo(this.RequestNew.DatiGenerici.T_NDOMUS);
                    GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo dati = new GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo();
                    dati.TransactionId = transactionId;
                    dati.NDomus = this.RequestNew.DatiGenerici.T_NDOMUS;
                    GestioneNuovoCalcolo.InsertOrUpdateNuovoCalcolo(dati);
                }
            }
            catch (CommunicationException ex)
            {
                eccezioni = ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : "CommunicationException");
            }
            catch (TimeoutException ex)
            {
                eccezioni = ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : "TimeoutException");
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(this.RequestNew.DatiGenerici.T_NDOMUS, Utility.MetodoServizio.IvsInvocation.ToString(), Utility.TipoLogGenerico.ErroreApplicativo, ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), null, ex.StackTrace);
            }

            return transactionId;
        }

        public string CallMainframe(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, string transactionId, string jsonString, string codErrore, string descrError, DateTimeOffset dataInizio, DateTimeOffset dataFine, DateTimeOffset dataNuovo, bool esito, GestioneNuovoCalcolo.FlowConf confFiltrata)
        {
            try
            {
                msCCQualityDataCheckerClient client = new msCCQualityDataCheckerClient(SetApiIdentity(this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(), "QualityDataChecker"));
                //var bodyResponse = Utility.GetXmlFromObject(this.Response);
                //var bodyRequest = Utility.GetXmlFromObject(this.RequestNew);

                string codCat = datiPensione.GetCodCategoria(); 

                msCCQualityDataCheckerClient.OutcomeRequestDTO requestDTO =
                new msCCQualityDataCheckerClient.OutcomeRequestDTO
                {
                    TransactionId = !string.IsNullOrEmpty(transactionId) ? transactionId : "",
                    NumDomanda = datiPensione.NDomus,
                    CodCategoria = codCat.Length > 3 ? codCat.Substring(1, 3) : codCat,
                    CodSede = datiPensione.CodiceSede.ToString().PadLeft(4, '0'),
                    CodCertificato = datiPensione.NCertificato != null ? datiPensione.NCertificato.ToString(): "",
                    CodFiscale = datiAnagrafici != null? datiAnagrafici.CodiceFiscale: "", 
                    CodGruppo = datiPensione.Gruppo,
                    CodProdotto = datiPensione.Prodotto,
                    CodTipo = datiPensione.Tipo,
                    CodGestione = datiPensione.Gestione,
                    CodFondo = datiPensione.Fondo,
                    CodIndconvint = datiPensione.IndConvInt.GetValueOrDefault() ? "1" : "0",
                    DescFase = Utility.IsRiaperturaDomanda(datiPensione.Id) ? "RIAPERTURA" : "NORIAPERTURA",  
                    CodTipoRichiesta = datiPensione.FlagVerify.HasValue ? datiPensione.FlagVerify.Value ? "1" : "0" : "1",
                    BodyRequest = jsonString, 
                    DataInvocazioneAbaco = dataNuovo,
                    DataInizioMF = dataInizio,
                    DataFineMF = dataFine,
                    DescrEsitoMF = esito ? "OK":"KO",
                    CodiceErroreMF = codErrore, 
                    DescrizioneErroreMF = descrError, 
                    CodUtente = this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(),
                    CodCategoriaPensione = confFiltrata != null ? confFiltrata.CodCategoria : "",
                    DescCategoriaPensione = confFiltrata != null ? confFiltrata.DescCategoria : "",
                    FlowCode = confFiltrata != null ? confFiltrata.FlowCode : "",
                    NomeTupla = confFiltrata != null ? confFiltrata.Descrizione : ""
                };

                string jsonString2 = JsonConvert.SerializeObject(requestDTO);

                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(requestDTO, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Mainframe, Utility.SOAPLogDirection.IN, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);
                
                Dictionary<string, string> headers = SetApiIdentityBis(this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(), "QualityDataChecker");
                var response = client.Mainframe(requestDTO, headers);
                //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Mainframe, Utility.SOAPLogDirection.OUT, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);           
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(this.RequestNew.DatiGenerici.T_NDOMUS, Utility.MetodoServizio.Mainframe.ToString(), Utility.TipoLogGenerico.ErroreApplicativo, ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), null, ex.StackTrace);
            }

            return transactionId;
        }

        public string CallAbaco(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici datiAnagrafici, string transactionId, string jsonStringRequest, string codErrore, string descrError, DateTimeOffset dataInizio, DateTimeOffset dataFine, DateTimeOffset dataNuovo, bool esito, string errori, string codiciErrore, GestioneNuovoCalcolo.FlowConf confFiltrata, string eccezioni, string jsonStringResponse)
        {
            try
            {
                msCCQualityDataCheckerClient client = new msCCQualityDataCheckerClient(SetApiIdentity(this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(), "QualityDataChecker"));
                //var bodyResponse = Utility.GetXmlFromObject(this.Response);
                //var bodyRequest = Utility.GetXmlFromObject(this.RequestNew);

                string codCat = datiPensione.GetCodCategoria();

                msCCQualityDataCheckerClient.FaultOutcomeRequestDTO requestDTO =
                new msCCQualityDataCheckerClient.FaultOutcomeRequestDTO
                {
                    NumDomanda = datiPensione.NDomus,
                    CodCategoria = codCat.Length > 3 ? codCat.Substring(1, 3) : codCat,
                    CodSede = datiPensione.CodiceSede.ToString().PadLeft(4, '0'),
                    CodCertificato = datiPensione.NCertificato != null ? datiPensione.NCertificato.ToString() : "",
                    CodFiscale = datiAnagrafici != null ? datiAnagrafici.CodiceFiscale : "",
                    CodGruppo = datiPensione.Gruppo,
                    CodProdotto = datiPensione.Prodotto,
                    CodTipo = datiPensione.Tipo,
                    CodGestione = datiPensione.Gestione,
                    CodFondo = datiPensione.Fondo,
                    CodIndconvint = datiPensione.IndConvInt.GetValueOrDefault() ? "1" : "0",
                    DescFase = Utility.IsRiaperturaDomanda(datiPensione.Id) ? "RIAPERTURA" : "NORIAPERTURA",
                    TipoRichiesta = datiPensione.FlagVerify.HasValue ? datiPensione.FlagVerify.Value ? "1" : "0" : "1",
                    BodyRequest = jsonStringRequest,
                    DataInvocazioneAbaco = dataNuovo,
                    DataInizioMF = dataInizio,
                    DataFineMF = dataFine,
                    DescrEsitoMF = esito ? "OK" : "KO",
                    CodiceErroreMF = codErrore,
                    DescrizioneErroreMF = descrError,
                    CodUtente = this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(),
                    DescrEsitoAbaco = !string.IsNullOrEmpty(eccezioni) ? eccezioni : "",
                    CodiceErroreAbaco = codiciErrore,
                    DescrizioneErroreAbaco = errori,
                    BodyResponse = jsonStringResponse,
                    CodCategoriaPensione = confFiltrata != null ? confFiltrata.CodCategoria : "",
                    DescCategoriaPensione = confFiltrata != null ? confFiltrata.DescCategoria : "",
                    FlowCode = confFiltrata != null ? confFiltrata.FlowCode : "",
                    NomeTupla = confFiltrata != null ? confFiltrata.Descrizione : "",
                };
            
                string jsonString2 = JsonConvert.SerializeObject(requestDTO);

                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(requestDTO, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Abaco, Utility.SOAPLogDirection.IN, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);

                Dictionary<string, string> headers = SetApiIdentityBis(this.RequestNew.DatiGenerici.T_TP1MATRICOLA.ToString(), "QualityDataChecker");
                var response = client.Abaco(requestDTO, headers);
                //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Abaco, Utility.SOAPLogDirection.OUT, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);           
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(this.RequestNew.DatiGenerici.T_NDOMUS, Utility.MetodoServizio.Abaco.ToString(), Utility.TipoLogGenerico.ErroreApplicativo, ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), null, ex.StackTrace);
            }

            return transactionId;
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
        private const string Config_ApiClientIdQualityDataChecker = "ApiClientIdQualityDataChecker";
        private const string Config_ApiClientSecretQualityDataChecker = "ApiClientSecretQualityDataChecker";
    }


}




