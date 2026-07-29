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
    public interface ILiquidazioniAbilitate : IViewUI
    {
        SvrLiquidazione.AreaLiquidazioniAbilitate LiquidazioniAbilitate { get; set; }
        Presenter.SvrLiquidazione.AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata { get; set; }
        UtilityTipoAppartenenza tipoAppRuolo { get; set; }
    }
}
