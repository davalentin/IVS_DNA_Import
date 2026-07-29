using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using System.Transactions;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Configuration;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneControlliDinamici
    {
        #region public members
        public static void GetControlloDinamicoByNomeControllo(string nomeControllo, out GestioneControlliDinamici.ControlloDinamico controlloDinamico)
        {
            controlloDinamico = null;
            ControlliDinamici controlliDinamicoDB = null;
            DAGestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControllo, out controlliDinamicoDB);
            if (controlliDinamicoDB != null)
                controlloDinamico = new ControlloDinamico(controlliDinamicoDB);
        }

        public static void GetControlliDinamici(out List<GestioneControlliDinamici.ControlloDinamico> elencoControlliDinamici)
        {
            elencoControlliDinamici = null;
            List<ControlliDinamici> elencoControlliDinamiciDB = null;
            DAGestioneControlliDinamici.GetControlliDinamici(out elencoControlliDinamiciDB);
            if (elencoControlliDinamiciDB != null && elencoControlliDinamiciDB.Count > 0)
            {
                elencoControlliDinamici = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneControlliDinamici.ControlloDinamico>();
                foreach (ControlliDinamici controlloDinamicoDB in elencoControlliDinamiciDB)
                {
                    elencoControlliDinamici.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneControlliDinamici.ControlloDinamico(controlloDinamicoDB));
                }
            }
        }

        public static void GetAnnoCompetenza(Utility.TipoAppartenenza? tipoAppartenenza, out int annoCompetenza)
        {
            annoCompetenza = 0;
            ControlloDinamico controlloDinamico = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataRinnovo" + tipoAppartenenza.GetValueOrDefault().ToString(), out controlloDinamico);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);
            DateTime dataRinnovo = Utility.DataFromString(controlloDinamico.ValoreControllo, Utility.FormatoData.AAAAmmGG).Value;
            //logica:
            //1)ValoreControllo
            //2) se Now è successiva a ValoreControllo
            // 2.1) se anno now e anno ValoreControllo sono uguali --> WsPensione.AnnoCompetenza = dataSistema.Year + 1;
            // 2.2) altrimenti WsPensione.AnnoCompetenza = dataSistema.Year;
            //3) se Now è antecedente a ValoreControllo --> WsPensione.AnnoCompetenza = dataSistema.Year;
            if (Utility.DataSuccessivaA(dataSistema, dataRinnovo))
                if (dataSistema.Year == dataRinnovo.Year)
                    annoCompetenza = dataSistema.Year + 1;
                else
                {
                    annoCompetenza = dataSistema.Year;
                    DateTime nuovaDataRinnovo = dataRinnovo.AddYears(dataSistema.Year - dataRinnovo.Year);
                    controlloDinamico.ValoreControllo = nuovaDataRinnovo.Year.ToString().PadLeft(4, '0') +
                        nuovaDataRinnovo.Month.ToString().PadLeft(2, '0') + nuovaDataRinnovo.Day.ToString().PadLeft(2, '0');
                    SalvaControlloDinamico(controlloDinamico);
                }
            else
            {
                if (dataSistema.Year == dataRinnovo.Year)
                    annoCompetenza = dataSistema.Year;
                else
                {
                    annoCompetenza = dataSistema.Year;
                    DateTime nuovaDataRinnovo = dataRinnovo.AddYears(dataSistema.Year - dataRinnovo.Year);
                    controlloDinamico.ValoreControllo = nuovaDataRinnovo.Year.ToString().PadLeft(4, '0') +
                        nuovaDataRinnovo.Month.ToString().PadLeft(2, '0') + nuovaDataRinnovo.Day.ToString().PadLeft(2, '0');
                    SalvaControlloDinamico(controlloDinamico);
                }
            }
        }

        public static DateTime GetDataSistema(Utility.TipoAppartenenza? tipoAppartenenza)
        {
            if (ConfigurationManager.AppSettings["DataSistema"] == "SI" && tipoAppartenenza != null)
            {
                GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
                string nomeControlloDinamico = "DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString();
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);
                if (controlloDinamico != null && !string.IsNullOrEmpty(controlloDinamico.ValoreControllo))
                {
                    DateTime? dataSistema = Utility.DataFromString(controlloDinamico.ValoreControllo, INPS.Pensioni.Liquidazione.BLCommon.Utility.FormatoData.AAAAmmGG);
                    if (dataSistema.HasValue && dataSistema != DateTime.MinValue)
                        return dataSistema.Value;
                    else
                        return DateTime.Now;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static void SetDataSistema(Utility.TipoAppartenenza? tipoAppartenenza, DateTime? dataSistema)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = new ControlloDinamico();
            controlloDinamico.NomeControllo = "DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString();
            if (dataSistema.HasValue)
                controlloDinamico.ValoreControllo = dataSistema.Value.Year.ToString().PadLeft(4, '0') +
                                                    dataSistema.Value.Month.ToString().PadLeft(2, '0') + dataSistema.Value.Day.ToString().PadLeft(2, '0');
            else
                controlloDinamico.ValoreControllo = null;
            SalvaControlloDinamico(controlloDinamico);
        }

        public static void GetDecorrenzaProvvisoriaObbligatoria(Utility.TipoAppartenenza? tipoAppartenenza, out DateTime? decorrenzaProvvisoriaObbligatoria)
        {
            decorrenzaProvvisoriaObbligatoria = null;

            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            string nomeControlloDinamico = "DecorrenzaProvvisoriaObbligatoria" + tipoAppartenenza.GetValueOrDefault().ToString();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);
            if (controlloDinamico != null && !string.IsNullOrEmpty(controlloDinamico.ValoreControllo))
            {
                decorrenzaProvvisoriaObbligatoria = Utility.DataFromString(controlloDinamico.ValoreControllo, INPS.Pensioni.Liquidazione.BLCommon.Utility.FormatoData.AAAAmmGG);
                if (!decorrenzaProvvisoriaObbligatoria.HasValue || decorrenzaProvvisoriaObbligatoria == DateTime.MinValue)
                    decorrenzaProvvisoriaObbligatoria = null;
            }
        }

        public static void SetDecorrenzaProvvisoriaObbligatoria(Utility.TipoAppartenenza? tipoAppartenenza, DateTime? decorrenzaProvvisoriaObbligatoria)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = new ControlloDinamico();
            controlloDinamico.NomeControllo = "DecorrenzaProvvisoriaObbligatoria" + tipoAppartenenza.GetValueOrDefault().ToString();
            if (decorrenzaProvvisoriaObbligatoria.HasValue)
                controlloDinamico.ValoreControllo = decorrenzaProvvisoriaObbligatoria.Value.Year.ToString().PadLeft(4, '0') +
                                                    decorrenzaProvvisoriaObbligatoria.Value.Month.ToString().PadLeft(2, '0') + decorrenzaProvvisoriaObbligatoria.Value.Day.ToString().PadLeft(2, '0');
            else
                controlloDinamico.ValoreControllo = null;
            SalvaControlloDinamico(controlloDinamico);
        }

        public static void SetDataCalcoloDefinitivoINDCOM(DateTime dataCalcoloDefinitivoINDCOM)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = new ControlloDinamico();

            controlloDinamico.NomeControllo = "DataCalcoloDefinitivoINDCOM";
            controlloDinamico.ValoreControllo = dataCalcoloDefinitivoINDCOM.Year.ToString().PadLeft(4, '0') +
                                                    dataCalcoloDefinitivoINDCOM.Month.ToString().PadLeft(2, '0') + dataCalcoloDefinitivoINDCOM.Day.ToString().PadLeft(2, '0');
          
            SalvaControlloDinamico(controlloDinamico);
            
        }

        public static void SetDataCalcoloPoligraficiLetteraB(DateTime dataCalcoloPoligraficiLetteraB)
        {
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = new ControlloDinamico();

            controlloDinamico.NomeControllo = "DataCalcoloPoligraficiEBA";
            controlloDinamico.ValoreControllo = dataCalcoloPoligraficiLetteraB.Year.ToString().PadLeft(4, '0') +
                                                    dataCalcoloPoligraficiLetteraB.Month.ToString().PadLeft(2, '0') + dataCalcoloPoligraficiLetteraB.Day.ToString().PadLeft(2, '0');

            SalvaControlloDinamico(controlloDinamico);

        }

        public static void GetListaVersioni(out Dictionary<string, string> listaVersioni)
        {
            listaVersioni = new Dictionary<string, string>();
            List<ControlloDinamico> elencoControlliDinamici = null;
            GestioneControlliDinamici.GetControlliDinamici(out elencoControlliDinamici);

            foreach (ControlloDinamico ctrl in elencoControlliDinamici)
            {
                switch (ctrl.NomeControllo)
                {
                    case "VersioneWA":
                    case "VersioneWCF":
                    case "VersioneWCFFS":
                    case "VersioneWCFAGO":
                    case "VersioneWCFCI":
                        listaVersioni.Add(ctrl.NomeControllo, ctrl.ValoreControllo);
                        break;
                }
            }
        }

        public static void SalvaControlloDinamico(GestioneControlliDinamici.ControlloDinamico datiControlloDinamico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                ControlliDinamici controlloDinamico = new ControlliDinamici();
                Utility.ValorizzaOggetti(datiControlloDinamico, controlloDinamico);
                DAGestioneControlliDinamici.SalvaControlloDinamico(controlloDinamico);
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// interpreta il controllo dinamico si/no in true/false
        /// </summary>
        /// <returns></returns>
        public static bool IsPolarizzazioneENPALSAttiva()
        {
            ControlloDinamico controlloDinamico = null;
            string nomeControlloDinamico = Keys.PolarizzazioneENPALSAttiva;
            GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);
            if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                return true;
            return false;
        }

        /// <summary>
        /// interpreta il controllo dinamico si/no in true/false
        /// </summary>
        /// <returns></returns>
        public static bool IsPolarizzazioneSuperstitiENPALSAttiva()
        {
            ControlloDinamico controlloDinamico = null;
            string nomeControlloDinamico = Keys.PolarizzazioneSuperstitiENPALSAttiva;
            GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);
            if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                return true;
            return false;
        }

        public static bool IsEnpalsManualeAbilitata()
        {
            ControlloDinamico controlloDinamico = null;
            string nomeControlloDinamico = Keys.EnpalsManualeAbilitata;
            GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);
            if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                return true;
            return false;
        }
        #endregion public members

        #region nested class
        public class ControlloDinamico
        {
            public ControlloDinamico(ControlliDinamici controlloDinamicoDB)
            {
                this._NomeControllo = controlloDinamicoDB.NomeControllo;
                this._ValoreControllo = controlloDinamicoDB.ValoreControllo;
            }

            public ControlloDinamico()
            { }

            #region public properties
            public string NomeControllo { get { return _NomeControllo; } set { _NomeControllo = value; } }

            public string ValoreControllo { get { return _ValoreControllo; } set { _ValoreControllo = value; } }
            #endregion public properties

            #region private properties
            private string _NomeControllo;

            private string _ValoreControllo;
            #endregion private properties
        }

        public class Keys
        {
            public const string PolarizzazioneENPALSAttiva = "PolarizzazioneENPALSAttiva";
            public const string PolarizzazioneSuperstitiENPALSAttiva = "PolarizzazioneSuperstitiENPALSAttiva";
            public const string EnpalsManualeAbilitata = "EnpalsManualeAbilitata";
        }
        #endregion nested class
    }
}

