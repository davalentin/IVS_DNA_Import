using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IPagamento: IViewUI
    {
        AreaPagamento pagamentoPensione { get; set; }
        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        RichiestaUfficiPagatori richiestaUfficiPagatori { get; set; }
        UfficioPagatore[] ufficioPagatore { get; set; }
    }
}
