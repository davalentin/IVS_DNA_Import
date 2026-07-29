using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface ISupplementi : IViewUI 
    {
         long numDomanda { get; set; }
         AreaSupplementi lstSupplementi { get; set; }
         Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
         AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; } 
    }
}


