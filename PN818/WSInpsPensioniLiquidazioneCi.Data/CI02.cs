using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using INPS.DNA.Data.HostIntegration;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using INPS.Pensioni.LiquidazioneCi.Data.HostRequest;
using INPS.Pensioni.LiquidazioneCi.Data.HostResponse;

namespace INPS.Pensioni.LiquidazioneCi.Data
{
    /// <summary>
    /// Invoca la transazione CI02: si occupa del calcolo delle ricostituzioni
    /// </summary>
    [Serializable]
    public class CI02 : BaseClass, ITransactionInfo
    {
        private HisLiquidazioneCi.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe CI02
        /// </summary>
        public CI02(PCIINPU7.Gruppo1 gruppo1, PCIINPU7.Gruppo2 gruppo2, PCIINPU7.Gruppo3 gruppo3, PCIINPU7.Gruppo4 gruppo4)
        {
            this.Request = new HostRequest.CI01_CI02Request();
            this.Request.Gruppo1 = gruppo1;
            this.Request.Gruppo2 = gruppo2;
            this.Request.Gruppo3 = gruppo3;
            this.Request.Gruppo4 = gruppo4;

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
        public HostRequest.CI01_CI02Request Request { get; set; }

        [HisComplexAreaInfoMapping(1, Direction = HostDirection.Output)]
        public HostResponse.CI02Response Response { get; set; }
        #endregion Tracciato Host

        #region Properties
        public string Messaggio { get; private set; }
        public string MessaggioDaLoggare { get; private set; }
        #endregion Properties

        #region ITransactionInfo Members

        public string TransactionName
        {
            get { return "CI02"; }
        }

        #endregion ITransactionInfo Members

        public void Invoke()
        {
            try
            {
                //Conversione dell'area di input
                byte[] inputData = HostTransactionManager.AreaToHost<CI02>(this);

                HisLiquidazioneCi.LiquidazioneCiClient proxy = new HisLiquidazioneCi.LiquidazioneCiClient();
                byte[] output = proxy.CI02(inputData, ref _ClientContext);

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
                    Messaggio = "KO: ERRORE DURANTE IL COLLOQUIO CON IL DATA BASE (errore 21). SE L’ERRORE CONTINUA, PREGASI SEGNALARE ALL'HELP DESK";
                    return;
                }

                //Conversione dell'area di output
                HostTransactionManager.AreaFromHost<CI02>(this, output);

                if (this.Response.RISP_UNTRASLATED != null && this.Response.RISP_UNTRASLATED.Length >= 4)
                {
                    byte[] data = new byte[4];
                    Buffer.BlockCopy(this.Response.RISP_UNTRASLATED, 0, data, 0, 4);

                    if (INPS.DNA.Data.HostIntegration.Conversion.ASCII.GetString(data) == "REDD")
                    {
                        this.Response.PresenzaREDD = new AreaPresenzaREDD();
                        HostTransactionManager.AreaFromHost<AreaPresenzaREDD>(this.Response.PresenzaREDD, this.Response.RISP_UNTRASLATED);
                    }
                    else
                    {
                        this.Response.NonPresenzaREDD = new CI02NonPresenzaREDD();
                        HostTransactionManager.AreaFromHost<CI02NonPresenzaREDD>(this.Response.NonPresenzaREDD, this.Response.RISP_UNTRASLATED);
                    }
                }

            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new INPS.DNA.DnaApplicationException("Puntamento errato al servizio His TI_PCI_R - " + TransactionName, ex);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
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
                    throw new INPS.DNA.DnaApplicationException("Errore di comunicazione con il servizio His TI_PCI_R - " + TransactionName, ex);
            }
            catch
            {
                throw;
            }
        }

        #region Private
        private void SetHisContext()
        {
            _ClientContext = new HisLiquidazioneCi.ClientContext();
            HisContext hisContext = new HisContext(this.TransactionName);
            _ClientContext.User = hisContext.ImsUser;
            if (_ClientContext.User.Length == 4)
                _ClientContext.User += INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.Substring(0, 4);
            _ClientContext.Password = hisContext.ImsPassword;
        }
        #endregion Private
    }
}
