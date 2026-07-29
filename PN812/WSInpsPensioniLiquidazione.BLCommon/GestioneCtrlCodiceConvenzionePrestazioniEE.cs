using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlCodiceConvenzionePrestazioniEE
    {
        public static void GetListaCtrlCodiceConvenzionePrestazioniEE(out List<DatiCtrlCodiceConvenzionePrestazioniEE> listaCtrlCodiceConvenzionePrestazioniEE)
        {
            listaCtrlCodiceConvenzionePrestazioniEE = null;

            List<CtrlCodiceConvenzionePrestazioniEE> ctrl = null;
            DAGestioneCtrlCodiceConvenzionePrestazioniEE.GetCtrlCodiceConvenzionePrestazioniEE(out ctrl);

            if (ctrl != null && ctrl.Count > 0)
            {
                listaCtrlCodiceConvenzionePrestazioniEE = new List<DatiCtrlCodiceConvenzionePrestazioniEE>();
                foreach (CtrlCodiceConvenzionePrestazioniEE datiCtrl in ctrl)
                {
                    DatiCtrlCodiceConvenzionePrestazioniEE obj = new DatiCtrlCodiceConvenzionePrestazioniEE();
                    Utility.ValorizzaOggetti(datiCtrl, obj);
                    listaCtrlCodiceConvenzionePrestazioniEE.Add(obj);
                }
            }
        }

        public static void GetListaCodiceConvenzionePerStato(string codiceStato, DateTime? decorrenzaOriginaria, out List<DatiCtrlCodiceConvenzionePrestazioniEE> listaCtrlCodiceConvenzionePrestazioniEE)
        {
            listaCtrlCodiceConvenzionePrestazioniEE = null;

            List<CtrlCodiceConvenzionePrestazioniEE> ctrl = null;
            DAGestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePrestazioniEE(codiceStato, decorrenzaOriginaria, out ctrl);
            if (ctrl != null && ctrl.Count > 0)
            {
                listaCtrlCodiceConvenzionePrestazioniEE = new List<DatiCtrlCodiceConvenzionePrestazioniEE>();
                foreach (CtrlCodiceConvenzionePrestazioniEE datiCtrl in ctrl)
                {
                    DatiCtrlCodiceConvenzionePrestazioniEE obj = new DatiCtrlCodiceConvenzionePrestazioniEE();
                    Utility.ValorizzaOggetti(datiCtrl, obj);
                    listaCtrlCodiceConvenzionePrestazioniEE.Add(obj);
                }
            }
        }

        public static void GetListaStatiByConvenzione(byte? codiceConvenzione, out List<DatiCtrlCodiceConvenzionePrestazioniEE> listaCtrlCodiceConvenzionePrestazioniEE)
        {
            listaCtrlCodiceConvenzionePrestazioniEE = null;

            List<CtrlCodiceConvenzionePrestazioniEE> ctrl = null;
            DAGestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceStatoPrestazioniEE(codiceConvenzione, out ctrl);

            if (ctrl != null && ctrl.Count > 0)
            {
                listaCtrlCodiceConvenzionePrestazioniEE = new List<DatiCtrlCodiceConvenzionePrestazioniEE>();
                foreach (CtrlCodiceConvenzionePrestazioniEE datiCtrl in ctrl)
                {
                    DatiCtrlCodiceConvenzionePrestazioniEE obj = new DatiCtrlCodiceConvenzionePrestazioniEE();
                    Utility.ValorizzaOggetti(datiCtrl, obj);
                    listaCtrlCodiceConvenzionePrestazioniEE.Add(obj);
                }
            }
        }

        public class DatiCtrlCodiceConvenzionePrestazioniEE
        {
            public string CodiceStato { get; set; }
            public byte CodiceConvenzione { get; set; }
            public System.Nullable<System.DateTime> DataInizio { get; set; }
            public System.Nullable<System.DateTime> DataFine { get; set; }
            public bool IsConvenzioneConAltroStato { get; set; }
        }
    }
}
