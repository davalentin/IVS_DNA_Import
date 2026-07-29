using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDataLimitePrepensionamentoLetteraB : IViewUI
    {
        AreaStoricoDataLimitePrepensionementoLetteraB storicoDataLimitePrepensionamentoLetteraB { get; set; }
        AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB dataLimitePrepensionamentoLetteraB { get; set; }
        UtilityTipoAppartenenza tipoAppRuolo { get; set; }
    }
}
