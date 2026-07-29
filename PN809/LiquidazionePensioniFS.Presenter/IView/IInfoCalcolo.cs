using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IInfoCalcolo : IViewUI
    {
        AreaTitolare.DatiPensione datiPensione { get; set; }
        SvrLiquidazione.AreaEsito areaEsito { get; set; }
        string statoPensione { get; set; }
        int certificato { get; set; }
        string chiavePensione { get; set; }
        bool IsVerify { get; set; }
        bool IsConsultazioniANFVerificate { get; set; }
        List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariConsultazioneUnificataANF> ListaConsultazioniANF { get; set; }
        bool IsReingegnerizzato { get; set; }
        List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneAnniRichiestaBonusDatiPrenotazioneElaborazioni> ListaPrenotazioneElaborazioni { get; set; }
        bool IsNuovoCalcolo { get; set; }
        string FlagIndennizzo { get; set; }
        bool BloccaInvio { get; set; }
    }
}
