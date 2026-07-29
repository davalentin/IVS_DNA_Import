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
    /// Invoca la transazione CI05: effettua la lettura della posizione pensionistica
    /// </summary>
    public class CI05Lettura : BaseClass, ITransactionInfo
    {
        private HisLiquidazioneCi.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe CI05
        /// </summary>

        public CI05Lettura(string programma, string matricolaOp, string sede, string cheFare, long numeroDomanda)
        {
            this.Request = new HostRequest.CI05RequestLettura();

            //Area controllo
            this.Request.Controllo.CDAS = "CI2005";
            this.Request.Controllo.CTRNIMS = "CI05";
            this.Request.Controllo.NLMXARECOM = 3500;
            this.Request.Controllo.NLMXAREINP = 3400;
            this.Request.Controllo.FSGNDBG = "0";
            this.Request.Controllo.NLMXTOT = 3500;

            //Area dati
            this.Request.Dati.CPGMDAS = programma;
            this.Request.Dati.MATRICOLA_OP = matricolaOp;
            this.Request.Dati.SEDE = sede;
            this.Request.Dati.CHEFARE = cheFare;
            this.Request.Dati.NUMEDOMA = numeroDomanda;

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
        public HostRequest.CI05RequestLettura Request { get; set; }

        [HisComplexAreaInfoMapping(1, Direction = HostDirection.Output)]
        public HostResponse.CI05Response Response { get; set; }
        #endregion Tracciato Host

        #region Properties
        public CI05AreaDecompressa FinalResponse { get; private set; }
        public string Messaggio { get; private set; }
        #endregion Properties

        #region ITransactionInfo Members

        public string TransactionName
        {
            get { return "CI05"; }
        }

        #endregion ITransactionInfo Members

        public void Invoke()
        {
            try
            {
                //Conversione dell'area di input
                byte[] inputData = HostTransactionManager.AreaToHost<CI05Lettura>(this);

                HisLiquidazioneCi.LiquidazioneCiClient proxy = new HisLiquidazioneCi.LiquidazioneCiClient();
                byte[] output = proxy.CI05(inputData, ref _ClientContext);

                //Gestione errori -  Gestione dell'abend: il messaggio comincia con 'DFS'
                if ((output[0] == 0xC4 && output[1] == 0xC6 && output[2] == 0xE2) || (output[1] == 0xC4 && output[2] == 0xC6 && output[3] == 0xE2))
                {
                    byte[] data = output;
                    if (output.Length > 155)
                    {
                        data = new byte[155];
                        Buffer.BlockCopy(output, 0, data, 0, 155);
                    }
                    Messaggio = INPS.DNA.Data.HostIntegration.Conversion.ASCII.GetString(data);
                    Messaggio = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", Messaggio);
                    return;
                }

                //Conversione dell'area di output
                HostTransactionManager.AreaFromHost<CI05Lettura>(this, output);

                DecodificaCodiceRitorno();

                if (!String.IsNullOrEmpty(this.Messaggio))
                    return;

                ConvertAreaDati();

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
                    Messaggio = "Transazione stoppata";
                    Messaggio = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", Messaggio);
                    return;
                }
                else if (ex.Message.Contains("IMS error message text:"))
                {
                    INPS.DNA.Logging.Logger.LogException(ex);
                    Messaggio = "Transazione in abend - " + ex.Message.Substring(ex.Message.LastIndexOf("IMS error message text:", StringComparison.InvariantCulture));
                    Messaggio = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", Messaggio);
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

        /// <summary>
        /// Conversione area output
        /// </summary>
        private void ConvertAreaDati()
        {
            CI05AreaCompressa areaCompressa = new CI05AreaCompressa();

            HostTransactionManager.AreaFromHost<CI05AreaCompressa>(areaCompressa, this.Response.Dati.RISP_COMPR);

            FinalResponse = new CI05AreaDecompressa();
            HostTransactionManager.AreaFromHost<CI05AreaDecompressa>(FinalResponse, Convert(this.Response.Dati.RISP_COMPR, areaCompressa.AREA_COMPRESSIONE));
        }

        private void DecodificaCodiceRitorno()
        {
            switch (this.Response.Dati.RISP_RC)
            {
                case 0:
                    Messaggio = "";
                    break;
                case 9:
                    Messaggio = "DOMANDA NON TROVATA";
                    break;
                default:
                    Messaggio = "ERRORE NON CODIFICATO: " + this.Response.Dati.RISP_RC.ToString();
                    break;
            }
        }

        #endregion Private
    }
}
