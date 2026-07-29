using System;
using System.Collections.Generic;
using System.Collections;

using INPS.DNA.Data.HostIntegration;
using INPS.DNA.Data.HostIntegration.HisMapper;
using INPS.DNA.Data.HostIntegration.HisMapper.Attributes;
using INPS.Pensioni.LiquidazioneFs.Data.HostResponse;
using System.Text;

using System.Net;
using System.Configuration;
using Newtonsoft.Json;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.ServiceModel;
using System.Linq;

namespace INPS.Pensioni.LiquidazioneFs.Data
{
    /// <summary>
    /// Invoca la transazione FSPL_FSRC: effettua il calcolo di una prima liquidata o di una ricostituzione
    /// </summary>
    public class FSPL_FSRCNew : BaseClass, ITransactionInfo
    {
        private HisLiquidazioneFs.ClientContext _ClientContext;

        #region Constructor
        /// <summary>
        /// Crea un'instanza della classe FSPL_FSRC
        /// </summary>

        public FSPL_FSRCNew(string transazione, string tipoOperazione, string sottoTipo, string fase, int annoCompetenza)
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
        public HostResponse.FSPL_FSRCResponseNew Response { get; set; }
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

                byte[] inputData = HostTransactionManager.AreaToHost<FSPL_FSRCNew>(this);

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
                HostTransactionManager.AreaFromHost<FSPL_FSRCNew>(this, output);

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
                        case "S":
                            if (AreaInputVariabile.ListaMiglioramentiContrattuali == null || AreaInputVariabile.ListaMiglioramentiContrattuali.Count == 0)
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "nullo");
                            if (!hashIndici.Contains(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()))
                                hashIndici.Add(this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant(), 0);
                            else
                                hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()] = int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()) + 1;
                            if (AreaInputVariabile.ListaMiglioramentiContrattuali.Count < int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString()))
                                throw new INPS.DNA.DnaApplicationException("Errore nella valorizzazione dell'area di input variabile: blocco " +
                                    this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant() + "richiesto in input non disponibile");
                            inputData = HostTransactionManager.AreaToHost<Data.CMSGTRA.MiglioramentiContrattuali>(AreaInputVariabile.ListaMiglioramentiContrattuali[int.Parse(hashIndici[this.Request.LISTBLOCCO[i].AR_ACCO.ToUpperInvariant()].ToString())]);
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
                    foreach (FSPL_FSRCResponseNew.AreaDati.Codice codice in this.Response.Dati.LISTCodice)
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

        #region nuovo calcolo


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

        public string CallMiddleware(GestionePensione.DatiPensione datiPensione, out string jsonStringRequest, out string errori, out string codiciErrore, out string eccezioni, out string jsonStringResponse)
        {
            string transactionId = string.Empty;
            jsonStringRequest = string.Empty;
            errori = string.Empty;
            codiciErrore = string.Empty;
            eccezioni = string.Empty;
            jsonStringResponse = string.Empty;
            try
            {
                swaggerMiddlewareClient client = new swaggerMiddlewareClient(SetApiIdentity(this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(), ""));
                swaggerMiddlewareClient.RequestGestionePubblicaDTO requestDTO = new swaggerMiddlewareClient.RequestGestionePubblicaDTO();

                #region request
                requestDTO.Request = new swaggerMiddlewareClient.FsplFsrcrRequestDTO()
                {
                    AgoGDP = new List<swaggerMiddlewareClient.AgoGDPDTO>(),
                    AgoTeorico = new List<swaggerMiddlewareClient.AgoTeoricoGestPubbDTO>(),
                    Anagrafica = new List<swaggerMiddlewareClient.AnagraficaGestPubbDTO>(),
                    DatiNonCalcolo = new List<swaggerMiddlewareClient.DatiNonCalcoloGestPubbDTO>(),
                    Delegato = new List<swaggerMiddlewareClient.DelegatoGestPubbDTO>(),
                    DelegatoNew = new List<swaggerMiddlewareClient.DelegatoNewGestPubbDTO>(),
                    DelegheTutele = new List<swaggerMiddlewareClient.DelegheTuteleGestPubbDTO>(),
                    Familiare = new List<swaggerMiddlewareClient.FamiliareGestPubbDTO>(),
                    FondoGDP = new List<swaggerMiddlewareClient.FondoGDPDTO>(),
                    Gp4INPDAP = new List<swaggerMiddlewareClient.Gp4InpdapGestPubbDTO>(),
                    Gp4IPOST = new List<swaggerMiddlewareClient.Gp4IpostGestPubbDTO>(),
                    MaggiorazioneLegge = new List<swaggerMiddlewareClient.MaggiorazioneLeggeGestPubbDTO>(),
                    MaggiorazioneSociale = new List<swaggerMiddlewareClient.MaggSocialeGestPubbDTO>(),
                    Minimo_PensInv = new List<swaggerMiddlewareClient.MinimoPensInvGestPubbDTO>(),
                    Redditi = new List<swaggerMiddlewareClient.RedditiGestPubbDTO>(),
                    RenditaINAIL = new List<swaggerMiddlewareClient.RenditaINAILGestPubbDTO>(),
                    Residenza = new List<swaggerMiddlewareClient.ResidenzaGestPubbDTO>(),
                    Supplementi = new List<swaggerMiddlewareClient.SupplementiGestPubbDTO>(),
                    TrattamentiFamiglia = new List<swaggerMiddlewareClient.TrattamentiFamigliaGestPubbDTO>(),
                    TrattenuteLavAutonomi = new List<swaggerMiddlewareClient.TrattLavAutGestPubbDTO>()
                    //manca dante causa per quando si dovranno gestire superstiti/spacchettate
                };


                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaAgoGDP, requestDTO.Request.AgoGDP);
                if (this.AreaInputVariabile.ListaAgoGDP != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaAgoGDP)
                    {
                        var res = new swaggerMiddlewareClient.AgoGDPDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.AgoGDP.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaAgoTeorico, requestDTO.Request.AgoTeorico);
                if (this.AreaInputVariabile.ListaAgoTeorico != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaAgoTeorico)
                    {
                        var res = new swaggerMiddlewareClient.AgoTeoricoGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.AgoTeorico.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaAnagrafica, requestDTO.Request.Anagrafica);
                if (this.AreaInputVariabile.ListaAnagrafica != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaAnagrafica)
                    {
                        var res = new swaggerMiddlewareClient.AnagraficaGestPubbDTO();
                        
                        Utility.ValorizzaOggettiBis(req, res);
                        res.TRASELIQ = req.TRASELIQ.ToString();
                        requestDTO.Request.Anagrafica.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaDatiNonCalcolo, requestDTO.Request.DatiNonCalcolo);
                if (this.AreaInputVariabile.ListaDatiNonCalcolo != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaDatiNonCalcolo)
                    {
                        var res = new swaggerMiddlewareClient.DatiNonCalcoloGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.DatiNonCalcolo.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaDelegato, requestDTO.Request.Delegato);
                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaDelegato, requestDTO.Request.DelegatoNew);
                if (this.AreaInputVariabile.ListaDelegato != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaDelegato)
                    {
                        var res = new swaggerMiddlewareClient.DelegatoNewGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.DelegatoNew.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaDelegheTutele, requestDTO.Request.DelegheTutele);
                if (this.AreaInputVariabile.ListaDelegheTutele != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaDelegheTutele)
                    {
                        var res = new swaggerMiddlewareClient.DelegheTuteleGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.DelegheTutele.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaFamiliare, requestDTO.Request.Familiare);
                if (this.AreaInputVariabile.ListaFamiliare != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaFamiliare)
                    {
                        var res = new swaggerMiddlewareClient.FamiliareGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRCCONTI != null)
                        {
                            res.LISTTRCCONTI = new List<swaggerMiddlewareClient.TabCodMaggGestPubbDTO>();

                            foreach (var req2 in req.LISTTRCCONTI)
                            {
                                var res2 = new swaggerMiddlewareClient.TabCodMaggGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTTRCCONTI.Add(res2);
                            }
                        }

                        requestDTO.Request.Familiare.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaFondoGDP, requestDTO.Request.FondoGDP);
                if (this.AreaInputVariabile.ListaFondoGDP != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaFondoGDP)
                    {
                        var res = new swaggerMiddlewareClient.FondoGDPDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.FondoGDP.Add(res);
                    }
                }

                if (this.AreaInputVariabile.ListaGp4INPDAP != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaGp4INPDAP)
                    {
                        var res = new swaggerMiddlewareClient.Gp4InpdapGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTK_GP4DB00 != null)
                        {
                            res.LISTK_GP4DB00 = new List<swaggerMiddlewareClient.InpdapDb00GestPubbDTO>();

                            foreach (var req2 in req.LISTK_GP4DB00)
                            {
                                var res2 = new swaggerMiddlewareClient.InpdapDb00GestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);

                                if (req2.LISTK_GP4DC00 != null)
                                {
                                    res2.LISTK_GP4DC00 = new List<swaggerMiddlewareClient.InpdapDc00GestPubbDTO>();

                                    foreach (var req3 in req2.LISTK_GP4DC00)
                                    {
                                        var res3 = new swaggerMiddlewareClient.InpdapDc00GestPubbDTO();
                                        Utility.ValorizzaOggettiBis(req3, res3);

                                        res2.LISTK_GP4DC00.Add(res3);
                                    }
                                }

                                res.LISTK_GP4DB00.Add(res2);
                            }
                        }

                        requestDTO.Request.Gp4INPDAP.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaGp4IPOST, requestDTO.Request.Gp4IPOST);
                if (this.AreaInputVariabile.ListaGp4IPOST != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaGp4IPOST)
                    {
                        var res = new swaggerMiddlewareClient.Gp4IpostGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTK_GP4DB00 != null)
                        {
                            res.LISTK_GP4DB00 = new List<swaggerMiddlewareClient.IpostDb00GestPubbDTO>();

                            foreach (var req2 in req.LISTK_GP4DB00)
                            {
                                var res2 = new swaggerMiddlewareClient.IpostDb00GestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);

                                if (req2.LISTK_GP4DC00 != null)
                                {
                                    res2.LISTK_GP4DC00 = new List<swaggerMiddlewareClient.IpostDc00GestPubbDTO>();

                                    foreach (var req3 in req2.LISTK_GP4DC00)
                                    {
                                        var res3 = new swaggerMiddlewareClient.IpostDc00GestPubbDTO();
                                        Utility.ValorizzaOggettiBis(req3, res3);

                                        res2.LISTK_GP4DC00.Add(res3);
                                    }
                                }

                                res.LISTK_GP4DB00.Add(res2);
                            }
                        }

                        requestDTO.Request.Gp4IPOST.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaMaggiorazioneLegge, requestDTO.Request.MaggiorazioneLegge);
                if (this.AreaInputVariabile.ListaMaggiorazioneLegge != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaMaggiorazioneLegge)
                    {
                        var res = new swaggerMiddlewareClient.MaggiorazioneLeggeGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.MaggiorazioneLegge.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaMaggiorazioneSociale, requestDTO.Request.MaggiorazioneSociale);
                if (this.AreaInputVariabile.ListaMaggiorazioneSociale != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaMaggiorazioneSociale)
                    {
                        var res = new swaggerMiddlewareClient.MaggSocialeGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRPELERD != null)
                        {
                            res.LISTTRPELERD = new List<swaggerMiddlewareClient.MaggSocElerdGestPubbDTO>();

                            foreach (var req2 in req.LISTTRPELERD)
                            {
                                var res2 = new swaggerMiddlewareClient.MaggSocElerdGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTTRPELERD.Add(res2);
                            }
                        }

                        requestDTO.Request.MaggiorazioneSociale.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaMinimo_PensInv, requestDTO.Request.Minimo_PensInv);
                if (this.AreaInputVariabile.ListaMinimo_PensInv != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaMinimo_PensInv)
                    {
                        var res = new swaggerMiddlewareClient.MinimoPensInvGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRGELERD != null)
                        {
                            res.LISTTRGELERD = new List<swaggerMiddlewareClient.MinPensElerdGestPubbDTO>();

                            foreach (var req2 in req.LISTTRGELERD)
                            {
                                var res2 = new swaggerMiddlewareClient.MinPensElerdGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTTRGELERD.Add(res2);
                            }
                        }

                        requestDTO.Request.Minimo_PensInv.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaRedditi, requestDTO.Request.Redditi);
                if (this.AreaInputVariabile.ListaRedditi != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaRedditi)
                    {
                        var res = new swaggerMiddlewareClient.RedditiGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);
                        requestDTO.Request.Redditi.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaRenditaINAIL, requestDTO.Request.RenditaINAIL);
                if (this.AreaInputVariabile.ListaRenditaINAIL != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaRenditaINAIL)
                    {
                        var res = new swaggerMiddlewareClient.RenditaINAILGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRGELERD != null)
                        {
                            res.TRGELERD = new List<swaggerMiddlewareClient.TabInailGestPubbDTO>();

                            foreach (var req2 in req.LISTTRGELERD)
                            {
                                var res2 = new swaggerMiddlewareClient.TabInailGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.TRGELERD.Add(res2);
                            }
                        }

                        requestDTO.Request.RenditaINAIL.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaResidenza, requestDTO.Request.Residenza);
                if (this.AreaInputVariabile.ListaResidenza != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaResidenza)
                    {
                        var res = new swaggerMiddlewareClient.ResidenzaGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRHELERD != null)
                        {
                            res.TRHELERD = new List<swaggerMiddlewareClient.VarResEsteroGestPubbDTO>();

                            foreach (var req2 in req.LISTTRHELERD)
                            {
                                var res2 = new swaggerMiddlewareClient.VarResEsteroGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.TRHELERD.Add(res2);
                            }
                        }
                        if (req.LISTTRHONERE != null)
                        {
                            res.TRHONERE = new List<swaggerMiddlewareClient.DatiPrepensAltriBenefGestPubbDTO>();

                            foreach (var req3 in req.LISTTRHONERE)
                            {
                                var res3 = new swaggerMiddlewareClient.DatiPrepensAltriBenefGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req3, res3);
                                res.TRHONERE.Add(res3);
                            }
                        }

                        requestDTO.Request.Residenza.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaSupplementi, requestDTO.Request.Supplementi);
                if (this.AreaInputVariabile.ListaSupplementi != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaSupplementi)
                    {
                        var res = new swaggerMiddlewareClient.SupplementiGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRE_SUP14 != null)
                        {
                            res.LISTTRE_SUP14 = new List<swaggerMiddlewareClient.TabSupplementiGestPubbDTO>();

                            foreach (var req2 in req.LISTTRE_SUP14)
                            {
                                var res2 = new swaggerMiddlewareClient.TabSupplementiGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTTRE_SUP14.Add(res2);
                            }
                        }

                        requestDTO.Request.Supplementi.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaTrattamentiFamiglia, requestDTO.Request.TrattamentiFamiglia);
                if (this.AreaInputVariabile.ListaTrattamentiFamiglia != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaTrattamentiFamiglia)
                    {
                        var res = new swaggerMiddlewareClient.TrattamentiFamigliaGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRFELENU != null)
                        {
                            res.LISTTRFELENU = new List<swaggerMiddlewareClient.TrattFamElenuGestPubbDTO>();

                            foreach (var req2 in req.LISTTRFELENU)
                            {
                                var res2 = new swaggerMiddlewareClient.TrattFamElenuGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTTRFELENU.Add(res2);
                            }
                        }
                        if (req.LISTTRFELERD != null)
                        {
                            res.LISTTRFELERD = new List<swaggerMiddlewareClient.TrattFamElerdGestPubbDTO>();

                            foreach (var req3 in req.LISTTRFELERD)
                            {
                                var res3 = new swaggerMiddlewareClient.TrattFamElerdGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req3, res3);
                                res.LISTTRFELERD.Add(res3);
                            }
                        }

                        requestDTO.Request.TrattamentiFamiglia.Add(res);
                    }
                }

                //Utility.ValorizzaOggettiBis(this.AreaInputVariabile.ListaTrattenuteLavAutonomi, requestDTO.Request.TrattenuteLavAutonomi);
                if (this.AreaInputVariabile.ListaTrattenuteLavAutonomi != null)
                {
                    foreach (var req in this.AreaInputVariabile.ListaTrattenuteLavAutonomi)
                    {
                        var res = new swaggerMiddlewareClient.TrattLavAutGestPubbDTO();
                        Utility.ValorizzaOggettiBis(req, res);

                        if (req.LISTTRM_AUTON1 != null)
                        {
                            res.LISTTRM_AUTON1 = new List<swaggerMiddlewareClient.TabTrattLavAutGestPubbDTO>();

                            foreach (var req2 in req.LISTTRM_AUTON1)
                            {
                                var res2 = new swaggerMiddlewareClient.TabTrattLavAutGestPubbDTO();
                                Utility.ValorizzaOggettiBis(req2, res2);
                                res.LISTTRM_AUTON1.Add(res2);
                            }
                        }

                        requestDTO.Request.TrattenuteLavAutonomi.Add(res);
                    }
                }



                //requestDTO.FlowCode = "01";
                requestDTO.CodGestione = datiPensione.Gestione;
                requestDTO.CodFondo = datiPensione.Fondo;
                requestDTO.IndConvInt = datiPensione.IndConvInt.GetValueOrDefault() ? "1" : "0";
                requestDTO.TipoRichiesta = datiPensione.FlagVerify.HasValue ? datiPensione.FlagVerify.Value ? "1" : "0" : "1";
                requestDTO.Fase = Utility.IsRiaperturaDomanda(datiPensione.Id) ? "RIAPERTURA" : "NORIAPERTURA";
                requestDTO.User = this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString();

                #endregion request

                jsonStringRequest = JsonConvert.SerializeObject(requestDTO);

                Guid guid = Guid.NewGuid();
               
                Dictionary<string, string> headers = SetApiIdentityBis(requestDTO.User, "");
                HttpStatusCode statusCode = HttpStatusCode.Continue;
                swaggerMiddlewareClient.ResponseDTO response = null;

                if (this.TransactionName == "FSPL")
                {
                    GestioneLogSoap.SalvaLogSoap(requestDTO, Utility.Servizio.SrvLiquidazioneFs, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                    response = client.IvsInvocationGestionePubblica(requestDTO, headers, out statusCode);
                    GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvLiquidazioneFs, Utility.MetodoServizio.IvsInvocation, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString().ToString(), guid);
                }
                transactionId = response != null ? response.TransactionId : string.Empty;
                errori = response != null && response.Errors != null && response.Errors.Count > 0 ? string.Join(";", response.Errors.Select(e => !string.IsNullOrEmpty(e.Message) ? e.Code.ToString() + " " + e.Message.ToString() : "").ToArray()) : (!((int)statusCode >= 200 && (int)statusCode < 300) ? statusCode.ToString() : null);
                codiciErrore = response != null && response.Errors != null && response.Errors.Count > 0 ? string.Join(";", response.Errors.Select(e => e.Code.ToString()).ToArray()) : string.Empty;
                eccezioni = !((int)statusCode >= 200 && (int)statusCode < 300) ? statusCode.ToString() : null;
                jsonStringResponse = JsonConvert.SerializeObject(response);

                //Aggiornamenti
                if (!string.IsNullOrEmpty(transactionId))
                {
                    GestioneNuovoCalcolo.UpdateScadutoEsistoNuovoCalcolo(datiPensione.NDomus);
                    GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo dati = new GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo();
                    dati.TransactionId = transactionId;
                    dati.NDomus = datiPensione.NDomus;
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
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, Utility.MetodoServizio.IvsInvocation.ToString(), Utility.TipoLogGenerico.ErroreApplicativo, ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), null, ex.StackTrace);
            }

            return transactionId;
        }

        public string CallMainframe(GestionePensione.DatiPensione datiPensione, string transactionId, string jsonString, string codErrore, string descrError, DateTimeOffset dataInizio, DateTimeOffset dataFine, DateTimeOffset dataNuovo, bool esito, GestioneNuovoCalcolo.FlowConf confFiltrata)
        {
            try
            {
                msCCQualityDataCheckerClient client = new msCCQualityDataCheckerClient(SetApiIdentity(this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(), "QualityDataChecker"));
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
                    CodCertificato = datiPensione.NCertificato != null ? datiPensione.NCertificato.ToString() : "",
                    CodFiscale = this.AreaInputVariabile.ListaAnagrafica[0] != null ? this.AreaInputVariabile.ListaAnagrafica[0].TRACOFIS : "",
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
                    DescrEsitoMF = esito ? "OK" : "KO",
                    CodiceErroreMF = codErrore,
                    DescrizioneErroreMF = descrError,
                    CodUtente = this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(),
                    CodCategoriaPensione = confFiltrata != null ? confFiltrata.CodCategoria : "",
                    DescCategoriaPensione = confFiltrata != null ? confFiltrata.DescCategoria : "",
                    FlowCode = confFiltrata != null ? confFiltrata.FlowCode : "",
                    NomeTupla = confFiltrata != null ? confFiltrata.Descrizione : ""
                };

                string jsonString2 = JsonConvert.SerializeObject(requestDTO);

                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(requestDTO, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Mainframe, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                Dictionary<string, string> headers = SetApiIdentityBis(this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(), "QualityDataChecker");
                var response = client.Mainframe(requestDTO, headers);
                //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Mainframe, Utility.SOAPLogDirection.OUT, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);           
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, Utility.MetodoServizio.Mainframe.ToString(), Utility.TipoLogGenerico.ErroreApplicativo, ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), null, ex.StackTrace);
            }

            return transactionId;
        }

        public string CallAbaco(GestionePensione.DatiPensione datiPensione, string transactionId, string jsonStringRequest, string codErrore, string descrError, DateTimeOffset dataInizio, DateTimeOffset dataFine, DateTimeOffset dataNuovo, bool esito, string errori, string codiciErrore, GestioneNuovoCalcolo.FlowConf confFiltrata, string eccezioni, string jsonStringResponse)
        {
            try
            {
                msCCQualityDataCheckerClient client = new msCCQualityDataCheckerClient(SetApiIdentity(this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(), "QualityDataChecker"));
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
                    CodFiscale = this.AreaInputVariabile.ListaAnagrafica[0] != null ? this.AreaInputVariabile.ListaAnagrafica[0].TRACOFIS : "",
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
                    CodUtente = this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(),
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
                GestioneLogSoap.SalvaLogSoap(requestDTO, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Abaco, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);

                Dictionary<string, string> headers = SetApiIdentityBis(this.AreaInputVariabile.ListaAnagrafica[0].TRAMATRI.ToString(), "QualityDataChecker");
                var response = client.Abaco(requestDTO, headers);
                //GestioneLogSoap.SalvaLogSoap(response, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.Abaco, Utility.SOAPLogDirection.OUT, this.RequestNew.DatiGenerici.T_NDOMUS.ToString(), guid);           
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, Utility.MetodoServizio.Abaco.ToString(), Utility.TipoLogGenerico.ErroreApplicativo, ex.Message + "-" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), null, ex.StackTrace);
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

        #endregion
    }
}
