using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IAziendeESOTEL : IViewUI
    {
        AreaAziendeESOTEL AziendeESOTEL { get; set; }
    }
}
