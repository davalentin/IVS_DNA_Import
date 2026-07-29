using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;



namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface ILiquidazionePensioneCi : IViewUI
    {
        AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
    }
}