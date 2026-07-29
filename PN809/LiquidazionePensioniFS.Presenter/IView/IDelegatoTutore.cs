using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDelegatoTutore : IViewUI
    {
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo delegato { get; set; }
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo tutore { get; set; }
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
