using System;
using System.Collections.Generic;
using System.Collections;

using INPS.DNA.Data.HostIntegration;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using INPS.Pensioni.LiquidazioneFs.Data.HostResponse;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Data
{
    /// <summary>
    /// Invoca la transazione FSPL_FSRC: effettua il calcolo di una prima liquidata o di una ricostituzione
    /// </summary>
    public class FSPL_FSRC : BaseClass, ITransactionInfo
    {
        private HisLiquidazioneFs.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe FSPL_FSRC
        /// </summary>

        public FSPL_FSRC(string transazione, string tipoOperazione, string sottoTipo, string fase, int annoCompetenza)
        {
            this.Request = new HostRequest.FSPL_FSRCRequest();

            TransactionName = transazione;
            try
            {
                this.Request.FILLER = "   DSPYAAAA";
                this.Request.AR_TIPO = tipoOperazione;
                this.Request.AR_SUBT = sottoTipo;
                this.Request.AR_FASE = fase;
                this.Request.AR_DATA = annoCompetenza;
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
        public HostRequest.FSPL_FSRCRequest Request { get; set; }

        [HisComplexAreaInfoMapping(1, Direction = HostDirection.Output)]
        public HostResponse.FSPL_FSRCResponse Response { get; set; }
        #endregion Tracciato Host

        #region Properties
        public Data.CMSGTRA.AreaVariabile AreaInputVariabile { get; set; }
        public string Messaggio { get; private set; }
        public string MessaggioDaLoggare { get; private set; }
        public bool HasError { get; private set; }
        public bool UtilizzaNuovoTracciato { get; set; }
        #endregion Properties

        #region ITransactionInfo Members

        public string TransactionName
        {
            get;
            private set;
        }

        #endregion ITransactionInfo Members

        #region public Members
        public void Invoke()
        {
            try
            {
                int offset = 0;
                int lunghezzaIntestazione = 76;
                //Conversione dell'area di input
                if (this.Request.LISTBLOCCO != null && this.Request.LISTBLOCCO.Count > 0)
                    ConvertAreaDati(out offset);

                this.Request.AR_LNGR = offset + lunghezzaIntestazione;

                byte[] inputData = HostTransactionManager.AreaToHost<FSPL_FSRC>(this);

                List<Byte> FinalInput = new List<byte>();
                FinalInput.AddRange(inputData);
                //FinalInput.AddRange(this.Request.DATI_INPUT);
                //Rimuovo i LOW VALUE in coda
                for (int i = FinalInput.Count - 1; i >= 0; i--)
                {
                    if (FinalInput[i] == 0x00 && FinalInput[i - 1] == 0x00)
                        FinalInput.RemoveAt(i);
                    else if (FinalInput[i] == 0x00 && FinalInput[i - 1] != 0x00)
                    {
                        FinalInput.RemoveAt(i);
                        break;
                    }
                    else
                        break;
                }

                HisLiquidazioneFs.LiquidazioneFsClient proxy = new HisLiquidazioneFs.LiquidazioneFsClient();
                byte[] output = null;
                if (TransactionName == "FSPL")
                    output = proxy.FSPL(FinalInput.ToArray(), ref _ClientContext);
                else if (TransactionName == "FSRC")
                    output = proxy.FSRC(FinalInput.ToArray(), ref _ClientContext);
                else
                    return;

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
                    HasError = true;
                    return;
                }

                //Conversione dell'area di output
                HostTransactionManager.AreaFromHost<FSPL_FSRC>(this, output);

                DecodificaCodiceRitorno();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new INPS.DNA.DnaApplicationException("Puntamento errato al servizio His TI_PFS_R - " + TransactionName, ex);
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
                    HasError = true;
                    return;
                }
                else if (ex.Message.Contains("IMS error message text:"))
                {
                    INPS.DNA.Logging.Logger.LogException(ex);
                    MessaggioDaLoggare = "Transazione in abend - " + ex.Message.Substring(ex.Message.LastIndexOf("IMS error message text:", StringComparison.InvariantCulture));
                    MessaggioDaLoggare = string.Format("Errore durante l'esecuzione di " + TransactionName + ": {0}", MessaggioDaLoggare);
                    Messaggio = "KO: ERRORE DURANTE IL COLLOQUIO CON IL DATA BASE (errore 21). SE L’ERRORE CONTINUA, PREGASI SEGNALARE ALL'HELP DESK";
                    HasError = true;
                    return;
                }
                else
                    throw new INPS.DNA.DnaApplicationException("Errore di comunicazione con il servizio His TI_PFS_R - " + TransactionName, ex);
            }
            catch
            {
                throw;
            }
        }
        #endregion public Members

        #region Private
        private void SetHisContext()
        {
            _ClientContext = new HisLiquidazioneFs.ClientContext();
            HisContext hisContext = new HisContext(this.TransactionName);
            _ClientContext.User = hisContext.ImsUser;
            if (_ClientContext.User.Length == 4)
                _ClientContext.User += INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode.Substring(0, 4);
            _ClientContext.Password = hisContext.ImsPassword;
        }

        private void ConvertAreaDati(out int offset)
        {
            offset = 0;
            if ((AreaInputVariabile.ListaGp4INPDAP != null && AreaInputVariabile.ListaGp4INPDAP.Count > 0) || (AreaInputVariabile.ListaGp4IPOST != null && AreaInputVariabile.ListaGp4IPOST.Count > 0))
                this.Request.DATI_INPUT = new byte[99916];
            else
                this.Request.DATI_INPUT = new byte[32916];
            byte[] inputData = null;
            if (this.Request.LISTBLOCCO != null)
            {
                Hashtable hashIndici = new Hashtable();

                int fondo = 0;
                int ago = 0;
                for (int i = 0; i < this.Request.LISTBLOCCO.Count; i++)
                {
                    if (String.IsNullOrEmpty(this.Request.LISTBLOCCO[i].AR_ACCO))
                        break;

                    switch (this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant())
                    {
                        case "A":
                            if (AreaInputVariabile.ListaAnagrafica == null || AreaInputVariabile.ListaAnagrafica.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaAnagrafica.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Anagrafica>(AreaInputVariabile.ListaAnagrafica[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "B":
                            if (AreaInputVariabile.ListaDelegato == null || AreaInputVariabile.ListaDelegato.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaDelegato.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.DelegatoNew>(AreaInputVariabile.ListaDelegato[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "C":
                            if (AreaInputVariabile.ListaFamiliare == null || AreaInputVariabile.ListaFamiliare.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaFamiliare.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Familiare>(AreaInputVariabile.ListaFamiliare[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "D":
                            if (AreaInputVariabile.ListaDanteCausa == null || AreaInputVariabile.ListaDanteCausa.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaDanteCausa.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.DanteCausa>(AreaInputVariabile.ListaDanteCausa[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "E":
                            if (AreaInputVariabile.ListaSupplementi == null || AreaInputVariabile.ListaSupplementi.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaSupplementi.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Supplementi>(AreaInputVariabile.ListaSupplementi[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "F":
                            if (AreaInputVariabile.ListaTrattamentiFamiglia == null || AreaInputVariabile.ListaTrattamentiFamiglia.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaTrattamentiFamiglia.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.TrattamentiFamiglia>(AreaInputVariabile.ListaTrattamentiFamiglia[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "G":
                            if (AreaInputVariabile.ListaMinimo_PensInv == null || AreaInputVariabile.ListaMinimo_PensInv.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaMinimo_PensInv.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Minimo_PensInv>(AreaInputVariabile.ListaMinimo_PensInv[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "H":
                            if (AreaInputVariabile.ListaResidenza == null || AreaInputVariabile.ListaResidenza.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaResidenza.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Residenza>(AreaInputVariabile.ListaResidenza[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "I":
                            if (AreaInputVariabile.ListaMaggiorazioneLegge == null || AreaInputVariabile.ListaMaggiorazioneLegge.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaMaggiorazioneLegge.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.MaggiorazioneLegge>(AreaInputVariabile.ListaMaggiorazioneLegge[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "K":
                            if (AreaInputVariabile.ListaDelegheTutele == null || AreaInputVariabile.ListaDelegheTutele.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaDelegheTutele.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Deleghe_Tutele>(AreaInputVariabile.ListaDelegheTutele[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "L":
                            if (AreaInputVariabile.ListaRenditaINAIL == null || AreaInputVariabile.ListaRenditaINAIL.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaRenditaINAIL.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.RenditaINAIL>(AreaInputVariabile.ListaRenditaINAIL[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "M":
                            if (AreaInputVariabile.ListaTrattenuteLavAutonomi == null || AreaInputVariabile.ListaTrattenuteLavAutonomi.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaTrattenuteLavAutonomi.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.TrattenuteLavAutonomi>(AreaInputVariabile.ListaTrattenuteLavAutonomi[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "N":
                            if (AreaInputVariabile.ListaAgoTeorico == null || AreaInputVariabile.ListaAgoTeorico.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaAgoTeorico.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.AgoTeorico>(AreaInputVariabile.ListaAgoTeorico[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "P":
                            if (AreaInputVariabile.ListaMaggiorazioneSociale == null || AreaInputVariabile.ListaMaggiorazioneSociale.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaMaggiorazioneSociale.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.MaggiorazioneSociale>(AreaInputVariabile.ListaMaggiorazioneSociale[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "R":
                            if (AreaInputVariabile.ListaRedditi == null || AreaInputVariabile.ListaRedditi.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaRedditi.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Redditi>(AreaInputVariabile.ListaRedditi[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "W":
                            if (AreaInputVariabile.ListaDatiNonCalcolo == null || AreaInputVariabile.ListaDatiNonCalcolo.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaDatiNonCalcolo.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.DatiNonCalcolo>(AreaInputVariabile.ListaDatiNonCalcolo[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
                            RitornaAreaDaConvertire(ref offset, ref inputData);
                            break;
                        case "X":
                            if (AreaInputVariabile.ListaFondoCL != null && AreaInputVariabile.ListaFondoCL.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.CL>(AreaInputVariabile.ListaFondoCL[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoDZ != null && AreaInputVariabile.ListaFondoDZ.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.DZ>(AreaInputVariabile.ListaFondoDZ[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoEL != null && AreaInputVariabile.ListaFondoEL.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.EL>(AreaInputVariabile.ListaFondoEL[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoES != null && AreaInputVariabile.ListaFondoES.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.ES>(AreaInputVariabile.ListaFondoES[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoET != null && AreaInputVariabile.ListaFondoET.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.ET>(AreaInputVariabile.ListaFondoET[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoFS != null && AreaInputVariabile.ListaFondoFS.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.FS>(AreaInputVariabile.ListaFondoFS[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoFS_New != null && AreaInputVariabile.ListaFondoFS_New.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.FS_New>(AreaInputVariabile.ListaFondoFS_New[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoPT != null && AreaInputVariabile.ListaFondoPT.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.PT>(AreaInputVariabile.ListaFondoPT[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoPT_New != null && AreaInputVariabile.ListaFondoPT_New.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.PT_New>(AreaInputVariabile.ListaFondoPT_New[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoGAS != null && AreaInputVariabile.ListaFondoGAS.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.GAS>(AreaInputVariabile.ListaFondoGAS[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoPI != null && AreaInputVariabile.ListaFondoPI.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.PI>(AreaInputVariabile.ListaFondoPI[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoPM != null && AreaInputVariabile.ListaFondoPM.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.PM>(AreaInputVariabile.ListaFondoPM[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoTT != null && AreaInputVariabile.ListaFondoTT.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.TT>(AreaInputVariabile.ListaFondoTT[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoVL != null && AreaInputVariabile.ListaFondoVL.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.VL>(AreaInputVariabile.ListaFondoVL[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            else if (AreaInputVariabile.ListaFondoGDP != null && AreaInputVariabile.ListaFondoGDP.Count > fondo)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Fondo.GDP>(AreaInputVariabile.ListaFondoGDP[fondo]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                fondo++;
                            }
                            break;
                        case "Y":
                            if (AreaInputVariabile.ListaAgoDZ != null && AreaInputVariabile.ListaAgoDZ.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.DZ>(AreaInputVariabile.ListaAgoDZ[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoEL != null && AreaInputVariabile.ListaAgoEL.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.EL>(AreaInputVariabile.ListaAgoEL[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoES != null && AreaInputVariabile.ListaAgoES.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.ES>(AreaInputVariabile.ListaAgoES[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoET != null && AreaInputVariabile.ListaAgoET.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.ET>(AreaInputVariabile.ListaAgoET[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoFS != null && AreaInputVariabile.ListaAgoFS.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.FS>(AreaInputVariabile.ListaAgoFS[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoPT != null && AreaInputVariabile.ListaAgoPT.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.PT>(AreaInputVariabile.ListaAgoPT[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoGAS != null && AreaInputVariabile.ListaAgoGAS.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.GAS>(AreaInputVariabile.ListaAgoGAS[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoPI != null && AreaInputVariabile.ListaAgoPI.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.PI>(AreaInputVariabile.ListaAgoPI[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoPM != null && AreaInputVariabile.ListaAgoPM.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.PM>(AreaInputVariabile.ListaAgoPM[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoTT != null && AreaInputVariabile.ListaAgoTT.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.TT>(AreaInputVariabile.ListaAgoTT[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoVL != null && AreaInputVariabile.ListaAgoVL.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.VL>(AreaInputVariabile.ListaAgoVL[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            else if (AreaInputVariabile.ListaAgoGDP != null && AreaInputVariabile.ListaAgoGDP.Count > ago)
                            {
                                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Ago.GDP>(AreaInputVariabile.ListaAgoGDP[ago]);
                                RitornaAreaDaConvertire(ref offset, ref inputData);
                                ago++;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (AreaInputVariabile.ListaGp4INPDAP != null && AreaInputVariabile.ListaGp4INPDAP.Count > 0)
            {
                int offsetSenzaGp4 = offset;
                //l'area Gp4Inpdap deve partire da posizione fissa 26269
                int filler = 26269 - 1 - offset - 76;
                byte[] inputFiller = Encoding.ASCII.GetBytes(new string(' ', filler));
                RitornaAreaDaConvertire(ref offset, ref inputFiller);

                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Gp4INPDAP>(AreaInputVariabile.ListaGp4INPDAP[0]);
                RitornaAreaDaConvertire(ref offset, ref inputData);
                //va restituito l'offset privo dell'area Gp4 per il calcolo della lunghezza totale
                offset = offsetSenzaGp4;
            }

            if (AreaInputVariabile.ListaGp4IPOST != null && AreaInputVariabile.ListaGp4IPOST.Count > 0)
            {
                int offsetSenzaGp4 = offset;
                //l'area Gp4Ipost deve partire da posizione fissa 26269
                int filler = 26269 - 1 - offset - 76;
                byte[] inputFiller = Encoding.ASCII.GetBytes(new string(' ', filler));
                RitornaAreaDaConvertire(ref offset, ref inputFiller);

                inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.Gp4IPOST>(AreaInputVariabile.ListaGp4IPOST[0]);
                RitornaAreaDaConvertire(ref offset, ref inputData);
                //va restituito l'offset privo dell'area Gp4 per il calcolo della lunghezza totale
                offset = offsetSenzaGp4;
            }
        }

        private void RitornaAreaDaConvertire(ref int offset, ref byte[] inputData)
        {
            if (offset + inputData.Length >= this.Request.DATI_INPUT.Length)
                return;

            Buffer.BlockCopy(inputData, 0, this.Request.DATI_INPUT, offset, inputData.Length);
            offset += inputData.Length;
        }

        private void DecodificaCodiceRitorno()
        {
            switch (this.Response.Dati.RZ_ESITO)
            {
                case 0:
                    Messaggio = "OK STAMPA E AGGIORNAMENTO BASE INFORMATIVA";
                    break;
                case 1:
                    Messaggio = "OK STAMPA E AGGIORNAMENTO BASE INFORMATIVA + ANNOTAZIONI";
                    break;
                case 2:
                    Messaggio = "SCARTO DA CALCOLO";
                    break;
                case 3:
                    Messaggio = "PROBLEMI TECNICI PROCEDURA CENTRALE";
                    break;
                default:
                    Messaggio = "ERRORE PROCEDURA " + TransactionName + " - SEGNALARE CODICE " + this.Response.Dati.RZ_ESITO;
                    break;
            }
            Messaggio += RecuperaDettaglioErrore();
        }

        private string RecuperaDettaglioErrore()
        {
            StringBuilder dettaglioErrore = new StringBuilder();
            try
            {
                if (this.Response.Dati.LISTCodice != null && this.Response.Dati.LISTCodice.Count > 0)
                {
                    foreach (FSPL_FSRCResponse.AreaDati.Codice codice in this.Response.Dati.LISTCodice)
                    {
                        if (codice != null && codice.RZ_CODES != null && codice.RZ_CODES.Trim() != "")
                            dettaglioErrore.Append(". ERROR CODE: " + codice.RZ_CODES);
                    }
                }
                if (this.Response.Dati.SEZ_MSGE != null && this.Response.Dati.SEZ_MSGE.Trim() != "")
                    dettaglioErrore.Append(". DETTAGLIO: " + this.Response.Dati.SEZ_MSGE.Replace("\0", ""));
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return dettaglioErrore.ToString();
        }

        #endregion Private
    }
}
