using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDanteCausa : IViewUI
    {
        long numDomanda { get; set; }
        AreaDanteCausa areaDanteCausa { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; } 
    }
}


