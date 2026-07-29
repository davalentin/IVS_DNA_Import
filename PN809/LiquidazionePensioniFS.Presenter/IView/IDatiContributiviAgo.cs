using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDatiContributiviAgo : IViewUI
    {
        SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
