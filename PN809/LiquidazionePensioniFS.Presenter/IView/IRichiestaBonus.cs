using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IRichiestaBonus : IViewUI
    {
        SvrLiquidazione.AreaRichiestaBonus areaRichiestaBonus { get; set; }
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
