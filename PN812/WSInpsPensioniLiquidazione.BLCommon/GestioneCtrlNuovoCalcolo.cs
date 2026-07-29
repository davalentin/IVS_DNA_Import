using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlNuovoCalcolo
    {
        public static void GetCtrlNuovoCalcolo(long ndomus, out NuovoCalcolo NuovoCalcolo)
        {
            NuovoCalcolo = null;
            CtrlNuovoCalcolo ctrlNuovoCalcolo = null;
            DACtrlNuovoCalcolo.GetCtrlNuovoCalcolo(ndomus, out ctrlNuovoCalcolo);
            if (ctrlNuovoCalcolo != null)
            {
                NuovoCalcolo = new NuovoCalcolo();
                Utility.ValorizzaOggetti(ctrlNuovoCalcolo, NuovoCalcolo);
            }
        }

        public static void GetDomandePuntualiCtrlNuovoCalcolo(long ndomus, out NuovoCalcolo NuovoCalcolo)
        {
            NuovoCalcolo = null;
            CtrlDomandePuntualiNuovoCalcolo ctrlNuovoCalcolo = null;
            DACtrlNuovoCalcolo.GetDomandePuntualiCtrlNuovoCalcolo(ndomus, out ctrlNuovoCalcolo);
            if (ctrlNuovoCalcolo != null)
            {
                NuovoCalcolo = new NuovoCalcolo();
                Utility.ValorizzaOggetti(ctrlNuovoCalcolo, NuovoCalcolo);
            }
        }
    }

    public class NuovoCalcolo
    {
        public long NDomus { get; set; }

        public string FlagVerifyDef { get; set; }

        public string FlagDoppiaChiamata { get; set; }
    }
}