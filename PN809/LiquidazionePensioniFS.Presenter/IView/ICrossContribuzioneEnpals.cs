using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    /// <summary>
    /// Questa interfaccia è condivisa tra AreaLiquidazionePensioneAgo e Supplimenti
    /// </summary>
    public interface ICrossContribuzioneEnpals : IViewUI
    {
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        DatiContribuzioneEnpals DatiContribuzioneEnpals { get; set; }
        TipologiaContribuzioneEnpals Tipologia { get; set; }
        bool IsContribuzioneEnpalsRetributivaVisible { get; set; }
        bool IsContribuzioneEnpalsContributivaVisible { get; set; }
    }
}
