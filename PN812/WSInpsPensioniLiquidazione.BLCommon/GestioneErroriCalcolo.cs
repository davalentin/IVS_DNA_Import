using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneErroriCalcolo
    {
        public static void GetErroriCalcolo(int codice, Procedura procedura, Gestione gestione, out GestioneErroriCalcolo.ErroriCalcolo erroriCalcolo)
        {
            erroriCalcolo = null;
            DataCommon.ErroriCalcolo objDB;
            DataCommon.DAGestioneErroriCalcolo.GetErroriCalcolo(codice,procedura.ToString(),gestione.ToString(), out objDB);
            if (objDB != null)
                erroriCalcolo = new ErroriCalcolo(objDB);
        }

        #region Nestled Class
        public class ErroriCalcolo
        {
            public ErroriCalcolo(INPS.Pensioni.Liquidazione.DataCommon.ErroriCalcolo objDB)
            {
                Utility.ValorizzaOggetti(objDB, this);
            }

            public long Id {get;set;}
            public int Codice { get; set; }
            public string Descrizione { get; set; }
            public string Procedura { get; set; }
            public string Gestione { get; set; }
        }

        #endregion Nestled Class

        #region Enum
        public enum Procedura { ALL };
        public enum Gestione { ALL };
        #endregion Enum
    }
}
