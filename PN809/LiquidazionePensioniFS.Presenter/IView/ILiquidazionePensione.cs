using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;


namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface ILiquidazionePensione: IViewUI
    {
        INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
