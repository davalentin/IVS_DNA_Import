using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneErroriPrelievo
    {
        public static void GetErroriPrelievo(string codice, Utility.TipoAppartenenza? tipoAppartenenza, out GestioneErroriPrelievo.ErroriPrelievo erroriCalcolo)
        {
            erroriCalcolo = null;
            DataCommon.ErroriPrelievo objDB;
            DataCommon.DAGestioneErroriPrelievo.GetErroriPrelievo(codice, tipoAppartenenza.GetValueOrDefault().ToString(), out objDB);
            if (objDB != null)
                erroriCalcolo = new ErroriPrelievo(objDB);
        }

        #region Nestled Class
        public class ErroriPrelievo
        {
            public ErroriPrelievo(INPS.Pensioni.Liquidazione.DataCommon.ErroriPrelievo objDB)
            {
                Utility.ValorizzaOggetti(objDB, this);
            }

            public long Id { get; set; }
            public string Codice { get; set; }
            public string Descrizione { get; set; }
            public string TipoAppartenenza { get; set; }
        }
        #endregion Nested Class
    }
}
