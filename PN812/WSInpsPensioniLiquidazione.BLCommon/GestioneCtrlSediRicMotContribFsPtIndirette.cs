using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlSediRicMotContribFsPtIndirette
    {
        public static bool IsDomandaLavorabileRicMotContribFsPtIndirette(bool? indConvInt, string gestione,short sede, string categoria)
        {
            List<DataCommon.CtrlSediRicMotContribFsPtIndirette> ctrlAbilitate = new List<DataCommon.CtrlSediRicMotContribFsPtIndirette>();

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(indConvInt, gestione, categoria);
            string fondo = tipoFondo.HasValue ? tipoFondo.Value.ToString().Trim() : null;

            DAGestioneCtrlSediRicMotContribFsPtIndirette.GetCtrlSediRicMotContribFsPtIndirette(sede, fondo, out ctrlAbilitate);

            if (ctrlAbilitate != null && ctrlAbilitate.Count > 0)
                return true;

            return false;
        }

    }
}
