using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IAziendeESOTRA : IViewUI
    {
        AreaAziendeESOTRA AziendeESOTRA { get; set; }
    }
}
