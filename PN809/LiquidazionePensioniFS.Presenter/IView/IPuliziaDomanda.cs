using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IPuliziaDomanda : IViewUI
    {
        AreaPuliziaDomanda areaPuliziaDomanda { get; set; }
        long numeroDomanda { get; set; }
        UtilityTipoAppartenenza TipoAppOperatore { get; set; }
        UtilityRuolo RuoloOperatore { get; set; }
    }
}
