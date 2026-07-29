using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IRuoli : IViewUI
    {
        Dictionary<string, string> RuoliAbilitati { get; set; }
        Ruoli SelectedRuolo { get; set; }
    }
}
