using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDatiContributivi: IViewUI
    {
        SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
