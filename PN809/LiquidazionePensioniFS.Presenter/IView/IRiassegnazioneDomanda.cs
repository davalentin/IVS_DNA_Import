using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IRiassegnazioneDomanda : IViewUI
    {
        AreaEsito.TipoEsito Esito { get; set; }
        long NumeroDomanda { get; set; }
        string StatoPensione { get; set; }
        string VecchiaMatricola { get; set; }
        string NuovaMatricola { get; set; }
        UtilityRuolo? Ruolo { get; set; }
        UtilityTipoAppartenenza? TipoAppOperatore { get; set; }
        UtilityTipoOperazione? TipoOperazione { get; set; }
        string SedeDiversa { get; set; }
    }
}
