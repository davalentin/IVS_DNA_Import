using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface ITrasformazioniAbilitate : IViewUI
    {
        SvrLiquidazione.AreaTrasformazioniAbilitate TrasformazioniAbilitate { get; set; }
        Presenter.SvrLiquidazione.AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata { get; set; }
        UtilityTipoAppartenenza tipoAppRuolo { get; set; }
    }
}
