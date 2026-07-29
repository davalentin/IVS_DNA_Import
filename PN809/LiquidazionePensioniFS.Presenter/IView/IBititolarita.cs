using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IBititolarita : IViewUI
    {
        Presenter.SvrLiquidazioneAgo.AreaDatiBititolarita areaDatiBititolaritaAgo { get; set; }
        Presenter.SvrLiquidazioneCi.AreaDatiBititolarita areaDatiBititolaritaCi { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
    }
}
