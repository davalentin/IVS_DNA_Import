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
    /// Invoca la transazione GACI: effettua la prenotazione di una ricostituzione
    /// </summary>
    public class GACINew : BaseClass, ITransactionInfo
    {
        private HisLiquidazioneCi.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe GACI
        /// </summary>

        public GACINew(string sede, string categoria, string certificato, string codice_af, string codice_as, string altriDati, int annoCompetenza)
        {
            this.Request = new HostRequest.GACIRequestNew();

            //this.Request.W_LUNG = 70;
            //this.Request.W_TRAN = "GACI";
            this.Request.W_DATI_DSO = " DSOY...";
            this.Request.W_DATI_SEDE = sede;
            this.Request.W_DATI_CAT = categoria;
            this.Request.W_DATI_CERT = certificato;
            this.Request.W_DATI_AF = codice_af;
            this.Request.W_DATI_AS = codice_as;
            this.Request.W_DATI = altriDati;
            this.Request.ANNO_COMPETENZA = annoCompetenza;
            this.Request.COD_PROCEDURA = "R";
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
        public HostRequest.GACIRequestNew Request { get; set; }

        [HisComplexAreaInfoMapping(1, Direction = HostDirection.Output)]
        public HostResponse.GACIResponseNew Response { get; set; }
        #endregion Tracciato Host

        #region Properties
        public GACIAreaDecompressa FinalResponse { get; private set; }
        public string Messaggio { get; private set; }
        public string MessaggioDaLoggare { get; private set; }
        public bool HasError { get; private set; }
        public GACIAreaDecompressaBis FinalResponseBis { get; private set; }
        public bool IsRic { get; set; }
        #endregion Properties

        #region ITransactionInfo Members

        public string TransactionName
        {
            get { return "GACI"; }
        }

        #endregion ITransactionInfo Members

        public void Invoke()
        {
            try
            {
                //Conversione dell'area di input
                byte[] inputData = HostTransactionManager.AreaToHost<GACINew>(this);

                HisLiquidazioneCi.LiquidazioneCiClient proxy = new HisLiquidazioneCi.LiquidazioneCiClient();
                byte[] output = proxy.GACI(inputData, ref _ClientContext);

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
                HostTransactionManager.AreaFromHost<GACINew>(this, output);
                ConvertAreaDati();
                DecodificaCodiceRitorno();
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
                    Messaggio = "Errore tecnico durante la comunicazione con l'archivio pensione. Riprovare più tardi.";
                    HasError = true;
                    return;
                }
                else if (ex.Message.Contains("IMS error message text:"))
                {
                    INPS.DNA.Logging.Logger.LogException(ex);
                    MessaggioDaLoggare = "Transazione in abend - " + ex.Message.Substring(ex.Message.LastIndexOf("IMS error message text:", StringComparison.InvariantCulture));
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "Errore tecnico durante la comunicazione con l'archivio pensione. Riprovare più tardi.";
                    HasError = true;
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
            GACIAreaCompressa areaCompressa = new GACIAreaCompressa();

            HostTransactionManager.AreaFromHost<GACIAreaCompressa>(areaCompressa, this.Response.Dati.RISP_COMPR);

            FinalResponseBis = new GACIAreaDecompressaBis();
            HostTransactionManager.AreaFromHost<GACIAreaDecompressaBis>(FinalResponseBis, Convert(this.Response.Dati.RISP_COMPR, areaCompressa.AREA_COMPRESSIONE));

            FinalResponse = new GACIAreaDecompressa();
            HostTransactionManager.AreaFromHost<GACIAreaDecompressa>(FinalResponse, Convert(this.Response.Dati.RISP_COMPR, areaCompressa.AREA_COMPRESSIONE));
        }

        private void DecodificaCodiceRitorno()
        {

            if (this.FinalResponseBis.Gruppo1.AreaTP11.ESITO == "9999999999" || this.FinalResponseBis.Gruppo1.AreaTP11.ESITO == "8888888888")
            {
                if (!this.IsRic)
                {

                    if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("1")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA NON PRESENTE NEL D.B. CENTRALE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("2")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA NON LIQUIDATA CON CALCOLO PASSANTE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("3")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA DA RICOSTITUIRE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("4")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA CON REG-LIQUIDAZIONE  A/B (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("5")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA CON ANNO 105 PARI O ANTE DATA MORTE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("6")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA CON ANNO 335 PARI O ANTE DATA MORTE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("7")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "PENS.DIRETTA CON COD.PART.DIRITTO = 6 (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("8")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "DATA MORTE DIVERSA DA DATA ELIMIN.DIRETTA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("F")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: DATI MANCANTI TABELLA RINNOVO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("G")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: OPER.NON CONSENTITA IN FASE DI RINNOVO  (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("H")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: ANNO COMPETENZA NON PIU' GESTIBILE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("I")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN:OPER. NON CONSEN.APERTURA NUOVA COMPETENZA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("L")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN:OPER.NON CONSENTITA MANCANZA DATI RINNOVATI (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("M")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: ANNO COMPETENZA NON ANCORA APERTO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("N")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "CHIAVE PENSIONE NON VALORIZZATA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("O")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "ANNO NON VALORIZZATO CORRETTAMENTE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("P")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "CODICE OPERAZIONE SCONOSCIUTO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("Q")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "TABELLA NATDATE NON TROVATA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("R")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "ERRORE ACCESSO DB - TABELLA NATDATE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("S")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "DATA FINE NON E > DI DATA INIZIO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else
                    {
                        this.Response.Dati.PresenzaPensione = false;
                        Messaggio = "Nessuna pensione presente";
                        HasError = true;
                    }
                }
                else
                {
                    int numero = 0;
                    if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("1")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "La pensione richiesta non esiste (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("2")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "ELIMINATA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("3")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "REGIME LIQUIDAZIONE A /B (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("4")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "ERRORE DI LETTURA D.B. (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("5")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "NON RINNOVATA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("6")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIMB.FORFET.2000 (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("A")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "GIA' RICOSTITUITA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("E")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RES.EST. CON MANDATO AGGIUNTIVO ART.38 L.448/2001  (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("F")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: DATI MANCANTI TABELLA RINNOVO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("G")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: OPER.NON CONSENTITA IN FASE DI RINNOVO  (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("H")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: ANNO COMPETENZA NON PIU' GESTIBILE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("I")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN:OPER. NON CONSEN.APERTURA NUOVA COMPETENZA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("L")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN:OPER.NON CONSENTITA MANCANZA DATI RINNOVATI (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("M")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "RIC-RIN: ANNO COMPETENZA NON ANCORA APERTO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("N")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "CHIAVE PENSIONE NON VALORIZZATA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("O")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "ANNO NON VALORIZZATO CORRETTAMENTE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("P")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "CODICE OPERAZIONE SCONOSCIUTO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("Q")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "TABELLA NATDATE NON TROVATA (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("R")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "ERRORE ACCESSO DB - TABELLA NATDATE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.StartsWith("S")) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "DATA FINE NON E > DI DATA INIZIO (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else if (!string.IsNullOrEmpty(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE) && int.TryParse(this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1), out numero) && numero > 6) { this.Response.Dati.PresenzaPensione = false; HasError = true; Messaggio = "NON DISPONIBILE (Cod." + this.FinalResponseBis.Gruppo1.AreaTP11.PRESENZA_PENSIONE.Substring(0, 1) + ")"; }
                    else
                    {
                        this.Response.Dati.PresenzaPensione = false;
                        Messaggio = "Nessuna pensione presente";
                        HasError = true;
                    }
                }
            }
            else
            {
                if (this.Request.W_DATI_AF == "SO")
                {
                    if (this.FinalResponse.Gruppo1.AreaTP12.TP1COMDC == 0)
                    {
                        this.Response.Dati.PresenzaPensione = false;
                        Messaggio = "Nessuna pensione presente";
                        HasError = true;
                    }
                    else
                        this.Response.Dati.PresenzaPensione = true;
                }
                else
                {
                    if (this.FinalResponse.Gruppo1.AreaTP11.TP1CO == 0)
                    {
                        this.Response.Dati.PresenzaPensione = false;
                        //Messaggio = "Nessuna pensione presente";
                        HasError = true;
                    }
                    else
                        this.Response.Dati.PresenzaPensione = true;
                }

            }


        }
        #endregion Private
    }
}

