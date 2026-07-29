using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IAziendeVESO29 : IViewUI
    {
        AreaAziendeVESO29 AziendeVESO29 { get; set; }
    }
}
