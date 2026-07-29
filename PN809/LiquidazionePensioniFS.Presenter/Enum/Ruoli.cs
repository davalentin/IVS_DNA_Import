using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.Enum
{
    public enum Ruoli
    {
        /// <summary>
        /// Operatore
        /// </summary>
        [Description("OPERATORE")]
        P4697,
        /// <summary>
        /// Amministratore FS
        /// </summary>
        [Description("AMMINISTRATORE FS/GDP")]
        P4677,
        /// <summary>
        /// Amministratore AGO
        /// </summary>
        [Description("AMMINISTRATORE AGO")]
        P8854,
        /// <summary>
        /// Amministratore CI
        /// </summary>
        [Description("AMMINISTRATORE CI")]
        P8855,
        /// <summary>
        /// Gestore FS
        /// </summary>
        [Description("UTENTE FS/GDP")]
        P4678,
        /// <summary>
        /// Gestore AGO
        /// </summary>
        [Description("UTENTE AGO")]
        P8856,
        /// <summary>
        /// Gestore CI
        /// </summary>
        [Description("UTENTE CI")]
        P8857,
        /// <summary>
        /// Direttore_RdP FS
        /// </summary>
        [Description("DIRETTORE/CAPO PROCESSO FS/GDP")]
        P8974,
        /// <summary>
        /// Direttore_RdP AGO
        /// </summary>
        [Description("DIRETTORE/CAPO PROCESSO AGO")]
        P8975,
        /// <summary>
        /// Direttore_RdP CI
        /// </summary>
        [Description("DIRETTORE/CAPO PROCESSO CI")]
        P8976,
    }
}
