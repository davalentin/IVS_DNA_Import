using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IInfoPostCalcolo : IViewUI
    {
        AreaTitolare.DatiPensione datiPensione { get; set; }
        SvrLiquidazione.AreaEsito areaEsito { get; set; }
        string statoPensione { get; set; }
    }
}
