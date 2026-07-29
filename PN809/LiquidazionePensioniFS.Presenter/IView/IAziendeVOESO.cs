using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IAziendeVOESO : IViewUI
    {
        AreaAziendeVOESO AziendeVOESO { get; set; }
    }
}
