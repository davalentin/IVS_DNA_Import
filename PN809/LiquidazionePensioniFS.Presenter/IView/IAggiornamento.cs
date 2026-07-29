using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IAggiornamento: IViewUI
    {
        UtilityTipoAppartenenza? TipoApp { get; set; }
        AreaAggiornamento areaAggiornamento { get; set; }
    }
}
