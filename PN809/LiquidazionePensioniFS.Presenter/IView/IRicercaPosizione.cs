using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface IRicercaPosizione : IViewUI
    {
        RicercaPosizione RicercaPosizione { get; set; }
        RicercaPosizione RicercaDanteCausa { get; set; }
        List<SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ElencoDomande { get; set; }
        List<SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ElencoPensioni { get; set; }
        List<SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ElencoSinonimi { get; set; }
        SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica RiepilogoAnagrafica { get; set; }
        SvrLiquidazione.AreaRispostaRiepilogo.DatiEsitoCalcolo EsitoCalcolo { get; set; }
        SvrLiquidazione.AreaEsito.TipoEsito Esito { get; set; }
        UtilityTipoAppartenenza TipoAppRuolo { get; set; }
        UtilityRuolo Ruolo { get; set; }
        bool IsDomandaDB { get; set; }
        bool IsPaginaConferma { get; set; }
        bool IsDomandaCalcolataProvvisoria { get; set; }
        bool IsConsultazione { get; set; }
        string SedeDiversa { get; set; }
        bool IsRicercaManualeDA { get; set; }
        bool IsNuovoCertificatoGeneratoEnpals { get; set; }
        //ENG - Pensioni Ovunque: gestione nuovo pannello
        bool MostraPanelloMessBloccantePensioniOvunque { get; set; }
        string SedePensioneGP1ALZ6 { get; set; }
        string CodCategoriaPensione { get; set; }
        string CertificatoInseguimentoPensione { get; set; }
        //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
        bool IsPaginaVisualizzazioneStatoPratiche { get; set; }
        //ENG - Gestione Popup Memo 239
        bool MostraPopupMemo239 { get; set; }
        //ENG - Gestione Popup Memo 31/2023
        bool MostraPopupMemo312023 { get; set; }
    }

}
