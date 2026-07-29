using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IStampa : IViewUI
    {
        AreaTitolare.DatiPensione datiPensione { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        MemoryStream msPDF { get; set; }
        AreaEsito areaEsito { get; set; }
    }
}
