using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface ISbloccoDomanda : IViewUI
    {
        long numDomanda { get; set; }
        AreaEsito areaEsito { get; set; }
        UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        string sedeDiversa { get; set; }
    }
}
