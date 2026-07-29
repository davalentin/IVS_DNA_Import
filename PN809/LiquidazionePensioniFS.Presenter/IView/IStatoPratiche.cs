using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IStatoPratiche: IViewUI
    {
        StatoPratica StatoPratica { get; set; }
        List<SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> ElencoStatoPratiche { get; set; }
        
        
        
        //List<SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ElencoDomande { get; set; }
        //List<SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ElencoPensioni { get; set; }
        //List<SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ElencoSinonimi { get; set; }
        //SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica RiepilogoAnagrafica { get; set; }
        
        SvrLiquidazione.AreaEsito.TipoEsito Esito { get; set; }
        UtilityTipoAppartenenza TipoAppRuolo { get; set; }
        UtilityRuolo Ruolo { get; set; }
    }
}
