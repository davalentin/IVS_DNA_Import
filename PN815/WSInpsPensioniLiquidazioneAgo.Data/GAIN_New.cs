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

namespace INPS.Pensioni.LiquidazioneAgo.Data
{
    /// <summary>
    /// Invoca la transazione GAIN: effettua la prenotazione e la sprenotazione di una ricostituzione
    /// </summary>
    [Serializable]
    public class GAIN_New : ITransactionInfo
    {
        private HisLiquidazioneAgo.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe GAIN
        /// </summary>

        public GAIN_New(string tipoRichiesta, string codiceCategoria, short sede , int certificato, int annoCompetenza)
        {
            this.Request = new HostRequest.GAINRequest();
            this.Request.Controllo.TIPO_RICHIESTA = tipoRichiesta;
            this.Request.Controllo.COD_CATEGORIA = codiceCategoria;
            this.Request.Controllo.COD_SEDE = sede;
            this.Request.Controllo.CERTIFICATO = certificato;
            this.Request.Controllo.CATEGORIA_CHIARO = "";
            this.Request.Controllo.ANNO_ELAB = annoCompetenza;
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
        public HostRequest.GAINRequest Request { get; set; }

        [HisComplexAreaInfoMapping(1, Direction = HostDirection.Output)]
        public HostResponse.GAINResponseNew ResponseNew { get; set; }
        #endregion Tracciato Host

        #region Properties
        public string Messaggio { get; private set; }
        public string MessaggioDaLoggare { get; private set; }
        public bool HasError { get; private set; }
        #endregion Properties

        #region ITransactionInfo Members

        public string TransactionName
        {
            get { return "GAIN"; }
        }

        #endregion ITransactionInfo Members

        public void Invoke()
        {
            try
            {
                //Conversione dell'area di input
                byte[] inputData = HostTransactionManager.AreaToHost<GAIN_New>(this);

                HisLiquidazioneAgo.LiquidazioneAgoClient proxy = new HisLiquidazioneAgo.LiquidazioneAgoClient();
                byte[] output = proxy.GAIN(inputData, ref _ClientContext);

                //Gestione errori -  Gestione dell'abend: il messaggio comincia con 'DFS'
                if ((output[0] == 0xC4 && output[1] == 0xC6 && output[2] == 0xE2) || (output[1] == 0xC4 && output[2] == 0xC6 && output[3] == 0xE2))
                {
                    byte[] data = output;
                    if (output.Length > 155)
                    {
                        data = new byte[155];
                        Buffer.BlockCopy(output, 0, data, 0, 155);
                    }
                    MessaggioDaLoggare = INPS.DNA.Data.HostIntegration.Conversion.ASCII.GetString(data);
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "Errore tecnico durante la comunicazione con l'archivio pensione. Riprovare più tardi.";
                    HasError = true;
                    return;
                }

                //Conversione dell'area di output
                HostTransactionManager.AreaFromHost<GAIN_New>(this, output);

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
                    Messaggio = "Errore tecnico durante la comunicazione con l'archivio pensione. Riprovare più tardi.";
                    return;
                }
                else if (ex.Message.Contains("IMS error message text:"))
                {
                    INPS.DNA.Logging.Logger.LogException(ex);
                    MessaggioDaLoggare = "Transazione in abend - " + ex.Message.Substring(ex.Message.LastIndexOf("IMS error message text:", StringComparison.InvariantCulture));
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "Errore tecnico durante la comunicazione con l'archivio pensione. Riprovare più tardi.";
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
            HisContext hisContext = new HisContext(this.TransactionName);
            _ClientContext.User = hisContext.ImsUser;
            if (_ClientContext.User.Length == 4)
                _ClientContext.User += INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.Substring(0, 4);
            _ClientContext.Password = hisContext.ImsPassword;
        }

        private void DecodificaCodiceRitorno()
        {
            switch (this.ResponseNew.Controllo.COD_RIT)
            {
                case "00":
                    Messaggio = "";
                    break;
                default:
                    HasError = true;
                    break;
            }
        }

        #endregion Private
    }
}


