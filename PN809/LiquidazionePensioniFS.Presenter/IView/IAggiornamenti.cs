using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IAggiornamenti : IViewUI
    {
        AreaAggiornamenti areaAggiornamenti { get; set; }
        UtilityTipoAppartenenza? tipoApp { get; set; }
    }
}
