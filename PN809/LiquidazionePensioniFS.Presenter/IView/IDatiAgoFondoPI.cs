using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDatiAgoFondoPI : IViewUI
    {
        long? IdDatiAgoFondoPI { get; set; }

        AreaDatiAgoFondoPI areaDatiAgoFondoPI { get; set; }
    }
}
