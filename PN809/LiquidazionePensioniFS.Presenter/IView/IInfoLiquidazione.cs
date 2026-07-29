using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IInfoLiquidazione : IViewUI
    {
        InfoLiquidazione InfoLiquidazione { get; set; }
    }
}