using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDetrazioni: IViewUI
    {
        SvrLiquidazione.AreaDetrazioni detrazioniPensione { get; set; }
        SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
