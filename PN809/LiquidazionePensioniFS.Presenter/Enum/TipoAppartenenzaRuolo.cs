using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.Enum
{
    public enum TipoAppartenenzaRuolo
    {
        /// <summary>
        /// FS
        /// </summary>
        [Description("FS")]
        FS,
        /// <summary>
        /// AGO
        /// </summary>
        [Description("AGO")]
        AGO,
        /// <summary>
        /// CI
        /// </summary>
        [Description("CI")]
        CI,
        /// <summary>
        /// Assente
        /// </summary>
        [Description("ASSENTE")]
        ASSENTE,
    }
}
