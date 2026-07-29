using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IDatiPensioneFondoPI : IViewUI
    {

        long IdFondo { get; set; }
        long? IdRecordFondo { get; set; }
        string NumDomanda { get; set; }
        short? ControCodiceRetribuzione { get; set; }
        AreaDatiPensioneFondoPI areaDatiPensioneFondoPI { get; set; }
    }
}
