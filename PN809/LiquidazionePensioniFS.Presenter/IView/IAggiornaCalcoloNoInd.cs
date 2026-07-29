using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract.AggiornaCalcoloNoInd;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Collections.Generic;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.InterfacceViews
{
    public interface IAggiornaCalcoloNoInd
    {
        List<CausaleDebito> CausaliDebito { get; set; }
        List<CausaleDtoLite> LegendaCausaliAmmesse { get; set; }
        RootIndebitoDto Indebito { get; set; }
        AreaQuadri areaQuadri { get; set; }
        AreaInfoPratica areaInfoPratica { get; set; }
        AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        AreaTitolare.DatiPensione datiPensione { get; set; }
        InfoLiquidazione InfoLiquidazione { get; set; }
        AreaEsito areaEsito { get; set; }
        bool? BloccoValidazioneCausali { get; set; }

        #region Componenti Grafiche
        string MessaggioCodaPannelloValutazioneEventualeScelta { get; set; }
        #endregion

        void MostraElencoCasualiDebito();
        void MostraValutazioneEventualeScelta();
        void LeggiInfoLiquidazione();
        void CaricaDomandaDaSessione();
        void PreparaAreaInfoPratica();
        void ApplicaSemaforiUI ();
        void ScriviDomandaSessione(AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda);
        void AccogliDomandaMostraEsitoPositivo();
        void AccogliDomandaMostraEsitoNegativo();
        void ElencoCausaliMetodoAccogliDomandaVisualizzazioneEsitoPositivo();
        void ElencoCausaliMetodoAccogliDomandaVisualizzazioneEsitoNegativo();
        void LeggiDatiPensioneDaSessione();
        void LeggiIndebitoDaSessione();
        void CaricaCausaliDabitoDaSessione();
        void MostraAvviso(string Message);
        void NascondiAvviso();
        void ValidaCausaliMostraEsitoPositivo();
        void ValidaCausaliMostraEsitoNegativo();
    }
}
