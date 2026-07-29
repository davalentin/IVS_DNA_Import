
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiGenericiVL_FS_PT : CustomBaseUserControl, ILiquidazionePensione, ITitolarePensione
    {
        #region ILiquidazionePensione
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            BindClick();
            AddInputClass();
        }

        protected void SalvaDatiGenerici_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiGenerici = GetDatiGenerici(this.domanda.Tipofondo);

            if (ViewState["DatiStoricoGP"] != null)
                areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico = (DatiLiquidazionePensioneStorico)ViewState["DatiStoricoGP"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiGenerici(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);

            if (!HasError)
            {
                ClearBonusSection();
                RaiseShowAvviso(this, Cevent);
                //ricarica ddl codiceComunicazione3
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
                SetDdlCodiceComunicazione3(datiDecodifica, this);
            }
            else
            {
                RaiseShowError(this, Cevent);
                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
                    GestioneAOI();
            }
        }

        protected void EliminaDatiGenerici_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiGenerici(this);

            if (!this.HasError)
            {
                ClearForm();
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                if (datiPensione.DecorrenzaOriginaria.HasValue)
                    lblDecorrenzaPensioneData.Text = datiPensione.DecorrenzaOriginaria.ToString().Substring(3, 7);
                bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);
                ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteDatiGenerici(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - MEMO 50/2023 
            string valoreControlloMemo50_2023 = string.Empty;
            if (ViewState["AbilitazioneMemo50_2023"] != null)
                valoreControlloMemo50_2023 = (string)ViewState["AbilitazioneMemo50_2023"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out valoreControlloMemo50_2023);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloMemo50_2023) && !String.IsNullOrEmpty(valoreControlloMemo50_2023.Trim()))
                    ViewState["AbilitazioneMemo50_2023"] = valoreControlloMemo50_2023.Trim();
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024 = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null)
                controlloDinamicoMemo123_2024 = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out controlloDinamicoMemo123_2024);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024"] = controlloDinamicoMemo123_2024;
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024OpzioneContrib = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null)
                controlloDinamicoMemo123_2024OpzioneContrib = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out controlloDinamicoMemo123_2024OpzioneContrib);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] = controlloDinamicoMemo123_2024OpzioneContrib;
            }

            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.HasValue &&
                           liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.Value)
                ViewState["IsProvvisoriaVisible"] = liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.Value;
            ViewState["DatiStoricoGP"] = (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null) ? (DatiLiquidazionePensioneStorico)liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico : null;
            LoadDdlCommon(liquidazione, this.domanda.Tipofondo, datiDecodifica, datiPensione);
            LoadDdlFromTipoFondo(this.domanda.Tipofondo, liquidazione);
            RenderControlsFromTipoFondo(liquidazione, datiPensione);
            RenderControlsCommon(IsDomandaSperDonna, liquidazione, datiPensione);

            #region switch TipoFondo

            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        ValorizzaEtichetteCommonFomPensioneFondoVL(IsDomandaSperDonna, liquidazione, datiPensione);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        ValorizzaEtichetteCommonFomPensioneFondoFS(IsDomandaSperDonna, liquidazione, datiPensione);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        ValorizzaEtichetteCommonFomPensioneFondoPT(IsDomandaSperDonna, liquidazione, datiPensione);
                        break;
                }
            }
            ValorizzaEtichetteCommon(IsDomandaSperDonna, liquidazione, datiPensione);

            #endregion switch TipoFondo

            if (datiPensione.FlagUnicarpe.HasValue)
                GestioneEtichetteIsUnicarpe(datiPensione);

            //Gestione ricostituzioni
            if (datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura)
                GestioneEtichetteRic(datiPensione);

            //Gestione ripristini
            if (datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ripristino)
                GestioneEtichetteRipristini(datiPensione);


            // Gestione Trasformazione di AOI
            if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
            {
                if ((liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.Value) ||
                    (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.GetValueOrDefault() && Utility.IsDomandaReversibilita(datiPensione)))
                {
                    ddlCodNatura3DG.SelectedValue = "H";
                    ddlCodNatura3DG.Enabled = false;
                    chkTrasfAOI.Checked = true;
                    chkTrasfAOI.Enabled = false;
                }
            }

            ManageSperDonna(IsDomandaSperDonna, liquidazione);

            ManageForPensioniVecchiaiaCalcoloContrib(liquidazione, datiPensione);

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                    ddlTipoCalcolo.Enabled = false;

                //ENG - per le pensioni della nuova opzione donna (tipo 0190) il secondo byte del codice natura "O" deve essere sempre selezionato e bloccato
                if (liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.HasValue)
                {
                    CodeUtility.DisableCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292(ddlCodNatura2DG, liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.Value);
                }

                //ENG - sulle ricostituzioni della nuova opzione donna rendere non editabili tutti i campi del pannello liquidazione pensione – generici ad eccezione della “data completezza”, “decorrenza arretrati” e primo codice natura
                if (CodeUtility.IsRicostituzione(datiPensione) && liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                {
                    ddlCodNatura1DG.Enabled = true;
                    ddlCodNatura3DG.Enabled = false;
                    ddlDeroga.Enabled = false;
                    txtDecorrenzaEconomica.Enabled = false;
                    ddlCodiciArretrati.Enabled = false;
                    txtScadRevSanitaria.Enabled = false;
                    txtInteressiLegali.Enabled = false;
                    ddlTipoCalcolo.Enabled = false;
                    ddlCausaCarico.Enabled = false;
                    if (!liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                        ddlCodComunicazioni4.Enabled = false;
                    txtFinestraMobile.Enabled = false;
                    chkExCombattente.Enabled = false;
                    chkBenefici.Enabled = false;
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = false;
                    chkRichiestaBonus.Enabled = false;
                    chkTrasfAOI.Enabled = false;
                    chkPensionePrivilegiata.Enabled = false;
                    chkArt2.Enabled = false;
                }

                string CodFase = GetCodFase(domanda.NumeroDomanda);
                if (Utility.IsRicostituzione(datiPensione) || Utility.IsRiaperturaDomanda(CodFase))
                {
                    ddlElContCodice.Enabled = false;
                    txtElContDecorrenza.Enabled = false;
                    txtElContDataEvento.Enabled = false;
                }

                //ENG - TFR della nuova opzione donna rendere non editabile il terzo byte del codice natura
                if (this.domanda.IsDomandaRiapertura && liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                {
                    ddlCodNatura3DG.Enabled = false;
                }

                //ENG - PL o TRF 024 fondi FS, PT ddlCodComunicazioni4 disabled se il titolare non è residente all'estero
                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS) &&
                    Utility.IsDomandaPL(datiPensione, false) && !liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                {
                    ddlCodComunicazioni4.Enabled = false;
                }

                //ENG - RIC 024 fondi FS, PT ddlCodComunicazioni3 disabled, ddlCodComunicazioni4 disabled se il titolare non è residente all'estero
                //ENG - RIC/TRF 024: il campo deve essere sempre sbloccato
                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS) &&
                    (CodeUtility.IsRicostituzione(datiPensione) || domanda.IsDomandaRiapertura))
                {
                    //ddlCodComunicazioni3.Enabled = false;
                    ddlCodComunicazioni4.Enabled = true;
                }
            }
            //ENG - MEMO 50/2023
            if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && CodeUtility.IsRicostituzioneSupplemento(datiPensione)))
                ddlTipoCalcolo.Enabled = false;

            //ENG - Reversibilità 024
            if (Utility.IsDomandaReversibilita(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS))
            {
                ddlTipoCalcolo.Enabled = false;
                ddlCodComunicazioni1.Enabled = false;
                ddlCodComunicazioni2.Enabled = false;
                txtCodComunicazioni2.Enabled = false;
                ddlCodComunicazioni3.Enabled = false;
                if (!liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                    ddlCodComunicazioni4.Enabled = false;
            }

            //ENG - 024 - RIC CONCESSIONE ALTRA PENSIONE
            if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS))
                ddlTipoCalcolo.Enabled = false;

            CodeUtility.ManageRecordEsenzioneFiscale(ref ddlCodComunicazioni4, Utility.IsRicostituzione(datiPensione), Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione));
        }

        internal DatiGenerici GetDatiGenerici(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiGenerici = new DatiGenerici();
            areaLiquidazionePensioneFS.DatiGenerici = GetDatiGenericiCommon();

            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    areaLiquidazionePensioneFS.DatiGenerici = GetDatiGenericiToPensioneFondoVL(areaLiquidazionePensioneFS.DatiGenerici);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    areaLiquidazionePensioneFS.DatiGenerici = GetDatiGenericiToPensioneFondoFS(areaLiquidazionePensioneFS.DatiGenerici);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    areaLiquidazionePensioneFS.DatiGenerici = GetDatiGenericiToPensioneFondoPT(areaLiquidazionePensioneFS.DatiGenerici);
                    break;
            }
            return areaLiquidazionePensioneFS.DatiGenerici;
        }

        internal void GestioneAOI()
        {
            if (!this.chkTrasfAOI.Checked)
            {
                Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                presenterLiquidazione.GetLiquidazionePensione(this);
                if (HasError)
                    return;
                if (this.areaLiquidazionePensioneFS.DatiPrecedentePensione != null)
                    this.chkTrasfAOI.Checked = true;
            }
        }

        internal void ClearBonusSection()
        {
            if (!String.Equals(ddlCodNatura2DG.SelectedValue, "Y"))
            {
                ddlAttribuzioneBonus.SelectedIndex = 0;
                txtDataInizioBonus.Text = "MM/AAAA";
                txtDataFineBonus.Text = "MM/AAAA";
            }
        }

        #region private

        internal void SetDdlCodiceComunicazione3(AreaDecodifica datiDecodifica, ILiquidazionePensione liquidazione)
        {
            ddlCodComunicazioni3.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampo3 comunicazioneCampo3 in datiDecodifica.ElencoComunicazioneCampo3)
            {
                switch (comunicazioneCampo3.Id)
                {
                    case "Y":
                        if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3.Equals('Y'))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "":
                        CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "P":
                        if (ViewState["IsProvvisoriaVisible"] != null && (bool)ViewState["IsProvvisoriaVisible"] &&
                            ((liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals("P"))
                            ||
                            ((liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiGenerici == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3.HasValue) &&
                            (liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico == null || !liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.HasValue
                            || liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals("P")))))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    default:
                        if (ViewState["IsProvvisoriaVisible"] != null && (bool)ViewState["IsProvvisoriaVisible"] &&
                            (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals(comunicazioneCampo3.Id.Trim().ToUpperInvariant()))
                            ||
                            ((liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiGenerici == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3.HasValue) &&
                            (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.HasValue
                            && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals(comunicazioneCampo3.Id.Trim().ToUpperInvariant()))))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                }
            }
            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null
                           && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3 != null)
                ddlCodComunicazioni3.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3.ToString();

            if (checkMemo74_88())
            {
                ddlCodComunicazioni3.SelectedValue = "P";
                ddlCodComunicazioni3.Enabled = false;
            }
        }

        private string GetCodFase(string NumeroDomanda)
        {
            string CodFase = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Presenter.SvrLiquidazione.AreaEsito esito = objWS.GetCodFaseByNDomus(out CodFase, NumeroDomanda);
            return CodFase;
        }

        private bool checkMemo74_88()
        {
            bool retVal = false;
            string CodFase = GetCodFase(domanda.NumeroDomanda);
            string Gruppo = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).CodeGruppo;
            string Prodotto = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).CodeProdotto;
            string Caratterizzazione = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).Caratterizzazione;
            string TipoLetturaUnicarpe = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoLetturaUnicarpe.ToString();

            if (Utility.checkMemo74_88(CodFase, Gruppo, Prodotto, Caratterizzazione, TipoLetturaUnicarpe))
            {
                retVal = true;
            }
            return retVal;
        }

        private void BindClick()
        {
            chkTrasfAOI.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkBenefici.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkExCombattente.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            txtInteressiLegali.Attributes.Add("onFocus", "setDataInteressiLegali()");
            chkPensionePrivilegiata.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkArt2.Attributes.Add("onclick", "javascript:SetCheckBox(this)");

            if (this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                ddlCodNatura2DG.Attributes.Add("onChange", "javascript:getDDLCodNatura2Value()");
        }

        private void AddInputClass()
        {
            chkTrasfAOI.InputAttributes.Add("EnableClass", "onClassTrasfAOI");
            chkBenefici.InputAttributes.Add("EnableClass", "onClassBenefici");
            chkExCombattente.InputAttributes.Add("EnableClass", "onClassExCombattente");
            chkPensionePrivilegiata.InputAttributes.Add("EnableClass", "onClassPensPrivil");
            chkArt2.InputAttributes.Add("EnableClass", "onClassArt2");
        }

        private void RenderControlsFromTipoFondo(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    pnlCustomCheck_VL.Visible = true;
                    pnlCommonRequisitiAnteSperDonna.Visible = true;
                    pnlCustomRequsitiVecchiaia_VL.Visible = true;
                    pnlDecorrenzaGiuridica.Visible = true;
                    rowCausaCarico.Visible = true;
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value)
                        ddlCodNatura2DG.Enabled = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;
                    if (liquidazione.areaLiquidazionePensioneFS.IsDomandaAnteArmonizzazione.GetValueOrDefault())
                    {
                        ddlTipoCalcolo.ClearSelection();
                        if (ddlTipoCalcolo.Items.FindByText("Retributivo") != null)
                            ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Retributivo").Value;

                        if (!(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && CodeUtility.IsRicostituzione(datiPensione)))
                            ddlTipoCalcolo.Enabled = false;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    pnlDecorrenzaEconomica_FS_PT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlCustomCheck_FS_PT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlCommonRequisitiAnteSperDonna.Visible = true;
                    rowCausaCarico.Visible = true;
                    pnlDecorrenzaGiuridica.Visible = true;

                    txtDecorrenzaEconomica.Enabled = false;
                    txtDecorrenzaEconomica.CssClass = txtDecorrenzaEconomica.CssClass.Replace("date-picker-base", string.Empty);
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value)
                        ddlCodNatura2DG.Enabled = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    pnlDecorrenzaEconomica_FS_PT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlCustomCheck_FS_PT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    //// Commentato a seguito della mail del 03/02/2014 con Oggetto: RE: Reeng Pensioni - Richiesta Modifiche applicative
                    // pnlCustomCheckArt2_PT.Visible = liquidazione.areaLiquidazionePensioneFS.IsVisibleArt2.Value;
                    pnlCommonRequisitiAnteSperDonna.Visible = true;
                    //pnlCustomFinestraMobile_PT.Visible = true;
                    rowCausaCarico.Visible = true;
                    pnlDecorrenzaGiuridica.Visible = liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value)
                        ddlCodNatura2DG.Enabled = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;
                    break;
            }
        }

        private void ValorizzaEtichetteCommon(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (!datiPensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenzaPensioneData.Text = string.Empty;
            else
                lblDecorrenzaPensioneData.Text = Convert.ToString(datiPensione.DecorrenzaOriginaria).Substring(3, 7);
            //valorizza causa carico
            string causaCarico = (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null) ? (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CausaCarico.ToString()) : (string.Empty);
            bool causaCaricoEnabled;
            ddlCausaCarico.SelectedValue = CodeUtility.FS_SelectedValueDdlCausaCaricoByTipoDomanda(GetDatiPensione(this), causaCarico, out causaCaricoEnabled);
            ddlCausaCarico.Enabled = causaCaricoEnabled;

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.IsRichiestaBonusBookingAbilitata.GetValueOrDefault() || liquidazione.areaLiquidazionePensioneFS.IsRichiestaBonus154Abilitata.GetValueOrDefault())
                {
                    ManageRichiestaBonus(datiPensione, liquidazione.areaLiquidazionePensioneFS.DatiGenerici);
                    if (!chkRichiestaBonus.Checked)
                        chkRichiestaBonus.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.IsRichiestaBonus.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.IsRichiestaBonus.Value ? true : false;
                }

                txtDecorrenzaArretrati.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DecorrenzaCalcoloArretrati.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DecorrenzaCalcoloArretrati) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceArretrati.HasValue)
                    ddlCodiciArretrati.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceArretrati.ToString();
                else ddlCodiciArretrati.SelectedIndex = 0;

                txtDataCompletezza.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataCompletezza.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataCompletezza) : string.Empty;
                txtInteressiLegali.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataInteressiLegali.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataInteressiLegali) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TipoCalcolo.HasValue)
                {
                    try
                    {
                        ddlTipoCalcolo.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TipoCalcolo.Value.ToString();
                        if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TipoCalcolo.Value.ToString()) &&
                            liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TipoCalcolo.Value.ToString().Trim() != string.Empty &&
                            (string.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue) || ddlTipoCalcolo.SelectedValue.Trim() == string.Empty))
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
                        this.HasError = true;
                        this.ErrorMessage = "Tipo calcolo precedentemente salvato non compatibile con l'attuale valore della data perfezionamento requisiti";
                        RaiseShowAvviso(this, Cevent);
                    }
                }
                else ddlTipoCalcolo.SelectedIndex = 0;



                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo1.HasValue)
                    ddlCodComunicazioni1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo1.ToString();
                else ddlCodComunicazioni1.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo2.HasValue)
                    ddlCodComunicazioni2.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo2.ToString();
                else ddlCodComunicazioni2.SelectedIndex = 0;

                if (ddlCodComunicazioni1.SelectedValue == "1" || ddlCodComunicazioni1.SelectedValue == "2")
                    ddlCodComunicazioni2.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo2.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo4.HasValue)
                {
                    ddlCodComunicazioni4.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo4.ToString();

                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleVittima.GetValueOrDefault())
                    {
                        if (ddlCodComunicazioni4.SelectedValue == "1")
                        {
                            ddlCodComunicazioni4.Enabled = false;
                        }
                    }

                }
                else
                {
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    {
                        //tutte le domande di trasformazione e ricostituzione
                        if (liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleEsteroFromDetrazioni.GetValueOrDefault())
                        {
                            if (ddlCodComunicazioni4.Items.FindByValue("2") != null)
                                ddlCodComunicazioni4.SelectedValue = ddlCodComunicazioni4.Items.FindByValue("2").Value;
                        }

                        if (liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleVittima.GetValueOrDefault())
                        {
                            if (ddlCodComunicazioni4.Items.FindByValue("1") != null)
                            {
                                ddlCodComunicazioni4.SelectedValue = ddlCodComunicazioni4.Items.FindByValue("1").Value;
                                ddlCodComunicazioni4.Enabled = false;
                            }
                        }
                    }
                    else
                        ddlCodComunicazioni4.SelectedIndex = 0;
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceMotivo.HasValue)
                    ddlElContCodice.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceMotivo.ToString();
                else ddlElContCodice.SelectedIndex = 0;

                txtElContDecorrenza.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DecorrenzaEliminazione.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DecorrenzaEliminazione) : string.Empty;
                txtElContDataEvento.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataEvento.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataEvento) : string.Empty;
                txtScadRevSanitaria.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.ScadenzaRevisioneSanitaria.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.ScadenzaRevisioneSanitaria) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.Benefici.HasValue)
                {
                    chkBenefici.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.Benefici.Value;
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrattamentoDisagi.GetValueOrDefault())
                        chkBenefici.ToolTip = "È presente una richiesta di maggiorazione sociale in domanda";
                }
                else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrattamentoDisagi.GetValueOrDefault())
                {
                    chkBenefici.Checked = true;
                    chkBenefici.ToolTip = "È presente una richiesta di maggiorazione sociale in domanda";
                }
                bool? trattamentoDisagi = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrattamentoDisagi;
                HiddenTrattamentoDisagi.Value = !trattamentoDisagi.HasValue ? "" : (trattamentoDisagi.Value == true ? "true" : "false");

                if (Utility.IsRicostituzione_Reddituale(datiPensione) && datiPensione.CodeTipo == "0101")
                    chkBenefici.Checked = true;

                chkExCombattente.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.ExCombattente.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.ExCombattente.Value ? true : false;
                chkTrasfAOI.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.Value ? true : false;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus.HasValue)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus == true)
                    {
                        ddlAttribuzioneBonus.SelectedValue = "SI";
                        //ENG - nel caso di pensioni di reversibilità VL e con AttribuzioneBonus il secondo byte del codice natura deve essere valorizzato a "Y" e bloccato.
                        if (Utility.IsDomandaReversibilita(datiPensione) && this.domanda.Tipofondo.HasValue && this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
                        {
                            ddlCodNatura2DG.SelectedValue = "Y";
                            ddlCodNatura2DG.Enabled = false;
                        }
                    }
                    else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus == false)
                        ddlAttribuzioneBonus.SelectedValue = "NO";
                }
                else ddlAttribuzioneBonus.SelectedIndex = 0;

                txtDataInizioBonus.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.InizioBonus.HasValue && !String.Equals(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.InizioBonus.ToString().ToLowerInvariant(), "mm/aaaa") ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.InizioBonus.Value) : string.Empty;
                txtDataFineBonus.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.FineBonus.HasValue && !String.Equals(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.FineBonus.ToString().ToLowerInvariant(), "mm/aaaa") ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.FineBonus.Value) : string.Empty;
                chkReqAnz1294.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.RequisitiAl1294.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.RequisitiAl1294.Value ? true : false;
                chkReqVecch996.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.RequisitiAl996.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.RequisitiAl996.Value ? true : false;
                chkReqVecch1294.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.RequisitiVecchiaiaAl1294.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.RequisitiVecchiaiaAl1294.Value ? true : false;
                chkPensionePrivilegiata.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.Privilegiate.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.Privilegiate.Value ? true : false;
                chkArt2.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.Articolo2.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.Articolo2.Value ? true : false;

                if (pnlINPDAP.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataRinunciaTrattenutaInpdap.HasValue)
                        txtDecTrattINPDAP.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.DataRinunciaTrattenutaInpdap.Value);

                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrattenutaInpdap.HasValue)
                    {
                        if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrattenutaInpdap.Value)
                            ddlTrattINPDAP.SelectedValue = "SI";
                        else
                            ddlTrattINPDAP.SelectedValue = "NO";
                    }
                    else
                        ddlTrattINPDAP.SelectedIndex = 0;

                    if (pnlINPDAP.Visible && CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsDomandaEccezioneMemo86(this.domanda.Categoria, datiPensione.NaturaPensione, datiPensione) && datiPensione.DataPresentazioneDomanda != null &&
                  Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda.Value, new DateTime(2022, 02, 20)))
                    {
                        HiddenFieldIsRICPost20022022.Value = "SI";

                        if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)
                            && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                        {
                            hdnRicNonContributiva024.Value = "SI";
                        }
                        else
                        {

                            //ENG - Aggiornamento Memo86
                            string controlloDinamicoAggiornamentoMemo86 = string.Empty;
                            Presenter.PresenterControlliDinamici presenterAggiornamentoMemo86 = new PresenterControlliDinamici();
                            Presenter.SvrLiquidazione.AreaEsito esitoCaricamentoControlloDinamicoAggiornamentoMemo86 = presenterAggiornamentoMemo86.GetControlloDinamicoByNomeControllo("DataAttivazioneMemo86Del12_06_2023", out controlloDinamicoAggiornamentoMemo86);
                            if (esitoCaricamentoControlloDinamicoAggiornamentoMemo86 != null
                                && esitoCaricamentoControlloDinamicoAggiornamentoMemo86.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                                && !String.IsNullOrEmpty(controlloDinamicoAggiornamentoMemo86) && !String.IsNullOrEmpty(controlloDinamicoAggiornamentoMemo86.Trim())
                                && liquidazione.areaLiquidazionePensioneFS.DataPrelievoDomanda.HasValue
                                && Utility.DataSuccessivaA(liquidazione.areaLiquidazionePensioneFS.DataPrelievoDomanda.Value, Utility.DataFromString(controlloDinamicoAggiornamentoMemo86.Trim(), Utility.FormatoData.AAAAmmGG).Value))
                            {
                                VerificaAdesioneFondoCreditoAggiornamentoMemo86(liquidazione.areaLiquidazionePensioneFS.IsPresenteTrattenutaFondoCreditoDaPrelievo, liquidazione.areaLiquidazionePensioneFS.IsDataRinunciaTrattenutaInpdapStorico);
                            }
                            else
                            {
                                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault())
                                {
                                    ddlTrattINPDAP.Enabled = false;
                                    txtDecTrattINPDAP.Enabled = false;
                                }
                                else
                                    VerificaAdesioneFondoCredito();
                            }

                        }
                    }
                }
            }

            if (IsDomandaSperDonna)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TipoCalcolo.HasValue)
                {
                    ddlTipoCalcolo.ClearSelection();
                    if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                        ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                }
                if (!(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && CodeUtility.IsRicostituzione(datiPensione)))
                    ddlTipoCalcolo.Enabled = false;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici == null)
                {
                    ddlSperimentaleDonna.ClearSelection();
                    ddlSperimentaleDonna.SelectedValue = "SI";
                    txtAnzAnniSperDonna.Text = "35";
                }
                ddlSperimentaleDonna.Enabled = false;
            }

            if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivo.GetValueOrDefault()
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione
                || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo) //ENG - MEMO 166/2023
            {
                {
                    var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                    if (itemTipoCalcolo != null)
                    {
                        ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                        if (!(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && CodeUtility.IsRicostituzione(datiPensione)))
                            ddlTipoCalcolo.Enabled = false;
                    }
                }
            }

            //ENG - Memo 123/2024
            if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
            {
                {
                    var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                    if (itemTipoCalcolo != null)
                    {
                        ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                        ddlTipoCalcolo.Enabled = false;
                    }
                }
            }

            if (CodeUtility.IsTipoContributivoConOpzione(datiPensione, liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivoConOpzione) || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
            {
                ddlCodNatura2DG.ClearSelection();
                if (ddlCodNatura2DG.Items.FindByValue("J") != null)
                    ddlCodNatura2DG.SelectedValue = "J";
                ddlCodNatura2DG.Enabled = false;
            }

            if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.SceltaLavoratriciMadri.HasValue || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                chkBenefici.Checked = true;

            //024: per le RIC non Contributive i campi ddlTrattINPDAP e txtDecTrattINPDAP devono essere bloccati (memo 86/2021 Jira L2_IST_LIQ-1875)
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && CodeUtility.IsRicostituzione(datiPensione) &&
                ((liquidazione.areaLiquidazionePensioneFS.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault() && this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL) ||
                (!Utility.IsRicostituzione_MotiviContributivi(datiPensione) && this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)))
            {
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }

        }

        private DatiGenerici GetDatiGenericiCommon()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiGenerici = new DatiGenerici();

            areaLiquidazionePensioneFS.DatiGenerici.DecorrenzaCalcoloArretrati = (!String.IsNullOrEmpty(txtDecorrenzaArretrati.Text)) && (!String.Equals(txtDecorrenzaArretrati.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtDecorrenzaArretrati.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.CodiceArretrati = !String.IsNullOrEmpty(ddlCodiciArretrati.SelectedValue) ? byte.Parse(ddlCodiciArretrati.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneFS.DatiGenerici.ScadenzaRevisioneSanitaria = (!String.IsNullOrEmpty(txtScadRevSanitaria.Text)) && (!String.Equals(txtScadRevSanitaria.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtScadRevSanitaria.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.DataCompletezza = !String.Equals(txtDataCompletezza.Text.ToLowerInvariant(), "gg/mm/aaaa") ? Utility.GetDateFromString(txtDataCompletezza.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.DataInteressiLegali = (!String.IsNullOrEmpty(txtInteressiLegali.Text)) && (!String.Equals(txtInteressiLegali.Text.ToLowerInvariant(), "gg/mm/aaaa")) ? Utility.GetDateFromString(txtInteressiLegali.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.TipoCalcolo = !String.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue) ? byte.Parse(ddlTipoCalcolo.SelectedValue) : (byte?)null;

            string naturaPensione = "";
            naturaPensione = String.Concat(ddlCodNatura1DG.SelectedValue, ddlCodNatura2DG.SelectedValue, ddlCodNatura3DG.SelectedValue);
            areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione = naturaPensione;

            if (ddlAttribuzioneBonus.SelectedValue == "SI")
                areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus = true;
            else if (ddlAttribuzioneBonus.SelectedValue == "NO")
                areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus = false;
            else
                areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus = null;

            areaLiquidazionePensioneFS.DatiGenerici.InizioBonus = (!String.IsNullOrEmpty(txtDataInizioBonus.Text)) && (!String.Equals(txtDataInizioBonus.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtDataInizioBonus.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.FineBonus = (!String.IsNullOrEmpty(txtDataFineBonus.Text)) && (!String.Equals(txtDataFineBonus.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtDataFineBonus.Text) : null;

            if (!String.Equals(ddlCodNatura2DG.SelectedValue, "Y"))
            {
                areaLiquidazionePensioneFS.DatiGenerici.AttribuzioneBonus = null;
                areaLiquidazionePensioneFS.DatiGenerici.InizioBonus = null;
                areaLiquidazionePensioneFS.DatiGenerici.FineBonus = null;
            }

            areaLiquidazionePensioneFS.DatiGenerici.CausaCarico = !String.IsNullOrEmpty(ddlCausaCarico.SelectedValue) ? byte.Parse(ddlCausaCarico.SelectedValue) : (byte?)null;

            if (!String.IsNullOrEmpty(ddlCodComunicazioni1.SelectedValue))
            {
                areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo1 = byte.Parse(ddlCodComunicazioni1.SelectedValue);
                if (String.Equals(ddlCodComunicazioni1.SelectedValue, "1"))
                {
                    if (!String.IsNullOrEmpty(ddlCodComunicazioni2.Text))
                    {
                        if (!String.Equals(ddlCodComunicazioni2.Text, " "))
                            areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo2 = char.Parse(ddlCodComunicazioni2.SelectedValue);
                    }
                }
                else if (String.Equals(ddlCodComunicazioni1.SelectedValue, "2"))
                {
                    if (!String.IsNullOrEmpty(ddlCodComunicazioni2.Text))
                        if (!String.Equals(ddlCodComunicazioni2.Text, " "))
                            areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo2 = char.Parse(ddlCodComunicazioni2.SelectedValue);
                }
            }


            if (!String.IsNullOrEmpty(ddlCodComunicazioni3.SelectedValue))
                areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3 = char.Parse(ddlCodComunicazioni3.SelectedValue);

            //areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo3 = !String.IsNullOrEmpty(ddlCodComunicazioni3.SelectedValue) ? char.Parse(ddlCodComunicazioni3.SelectedValue) : (char?)null;
            areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo4 = !String.IsNullOrEmpty(ddlCodComunicazioni4.SelectedValue) ? byte.Parse(ddlCodComunicazioni4.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneFS.DatiGenerici.CodiceMotivo = !String.IsNullOrEmpty(ddlElContCodice.SelectedValue) ? byte.Parse(ddlElContCodice.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneFS.DatiGenerici.DecorrenzaEliminazione = (!String.Equals(txtElContDecorrenza.Text.ToLowerInvariant(), "mm/aaaa")) && (!(String.IsNullOrEmpty(txtElContDecorrenza.Text))) ? Utility.GetDateFromString(txtElContDecorrenza.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.DataEvento = (!String.Equals(txtElContDataEvento.Text.ToLowerInvariant(), "gg/mm/aaaa")) && (!(String.IsNullOrEmpty(txtElContDataEvento.Text))) ? Utility.GetDateFromString(txtElContDataEvento.Text) : null;
            areaLiquidazionePensioneFS.DatiGenerici.Benefici = chkBenefici.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.ExCombattente = chkExCombattente.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI = chkTrasfAOI.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.RequisitiAl1294 = chkReqAnz1294.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.RequisitiAl996 = chkReqVecch996.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.RequisitiVecchiaiaAl1294 = chkReqVecch1294.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.Privilegiate = chkPensionePrivilegiata.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenerici.Articolo2 = chkArt2.Checked ? true : false;

            if (!String.IsNullOrEmpty(ddlDeroga.SelectedValue))
                areaLiquidazionePensioneFS.DatiGenerici.CodiceParticolareSoggettoDerogato = long.Parse(ddlDeroga.SelectedValue);
            areaLiquidazionePensioneFS.DatiGenerici.TrattamentoDisagi = HiddenTrattamentoDisagi.Value == "true" ? true : (HiddenTrattamentoDisagi.Value == "false" ? false : (bool?)null);
            if (pnlRichiestaBonus.Visible)
            {
                areaLiquidazionePensioneFS.DatiGenerici.IsRichiestaBonus = chkRichiestaBonus.Checked == true ? chkRichiestaBonus.Checked : false;
                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                datiPensione.IsRichiestaBonus = areaLiquidazionePensioneFS.DatiGenerici.IsRichiestaBonus;
                if (datiPensione.CodeTipo != "0167" && chkRichiestaBonus.Checked == true)
                    areaLiquidazionePensioneFS.DatiGenerici.AnnoDecorrenzaBonus = !String.IsNullOrEmpty(txtAnnoBonus.Text) && !txtAnnoBonus.Text.ToUpperInvariant().Equals("AAAA") ? txtAnnoBonus.Text : string.Empty;
                else
                    areaLiquidazionePensioneFS.DatiGenerici.AnnoDecorrenzaBonus = !String.IsNullOrEmpty(hdnAnnoRichiestaBonus14.Value) ? hdnAnnoRichiestaBonus14.Value : string.Empty;
                Session["DatiPensione"] = datiPensione;
            }

            if (pnlINPDAP.Visible)
            {
                if (String.Equals(ddlTrattINPDAP.SelectedValue, "SI"))
                    areaLiquidazionePensioneFS.DatiGenerici.TrattenutaInpdap = true;
                else if (String.Equals(ddlTrattINPDAP.SelectedValue, "NO"))
                    areaLiquidazionePensioneFS.DatiGenerici.TrattenutaInpdap = false;
                else if (String.Equals(ddlTrattINPDAP.SelectedValue, ""))
                    areaLiquidazionePensioneFS.DatiGenerici.TrattenutaInpdap = null;

                if (!string.IsNullOrEmpty(txtDecTrattINPDAP.Text) && !txtDecTrattINPDAP.Text.ToUpperInvariant().Equals("MM/AAAA"))
                    areaLiquidazionePensioneFS.DatiGenerici.DataRinunciaTrattenutaInpdap = Utility.GetDateFromString(txtDecTrattINPDAP.Text);
                else
                    areaLiquidazionePensioneFS.DatiGenerici.DataRinunciaTrattenutaInpdap = null;
            }

            return areaLiquidazionePensioneFS.DatiGenerici;
        }

        private void RenderControlsCommon(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            pnlCommon_VL_FS_PT.Visible = true;
            pnlCommonHeader.Visible = true;
            pnlCommonCheck_VL_FS_PT.Visible = true;

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Substring(0, 1) == "I")  // pensione Invalidità
            {
                pnlRequisitiAnte247.Visible = false;
                pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                this.pnlSperimentaleDonna.Visible = false;
            }
            else
            {
                if (IsDomandaSperDonna)
                {
                    pnlRequisitiAnte247.Visible = false;
                    pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;

                    if (!liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.HasValue || !liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.Value)
                        this.pnlSperimentaleDonna.Visible = false;
                    else
                        this.pnlSperimentaleDonna.Visible = true;
                }
                else
                {
                    if (!liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.HasValue || !liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.Value)
                    {
                        pnlRequisitiAnte247.Visible = false;
                        pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                    }
                    else
                    {
                        pnlRequisitiAnte247.Visible = true;
                        pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = true;
                    }

                    this.pnlSperimentaleDonna.Visible = false;
                }
            }

            if (!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Substring(0, 1) == "S")  // pensione ai Superstiti
            {
                pnlRequisitiAnte247.Visible = false;
                pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
            }

            if (((!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Substring(0, 1) == "V") || (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto != "0011") ||
                (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione && !datiPensione.IsPLInvalidita.GetValueOrDefault())) &&
                !Utility.IsDomandaEccezioneMemo86(this.domanda.Categoria, datiPensione.NaturaPensione, datiPensione))
                pnlINPDAP.Visible = true;

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
            {
                rowCausaCarico.Visible = false;

                if (tipologiaProdottoPensione != CodeUtility.TipologiaPensioneProdotto.pr_VariazioneDecorrenza)
                {
                    pnlRequisitiAnte247.Visible = false;
                    pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                }
            }

            ManageScadenzaRevSanitaria_Privilegiate(datiPensione, liquidazione);
            ManageProvvisoria(liquidazione);
            ManageDeroga(liquidazione);

            if (datiPensione.DecorrenzaOriginaria > Utility.GetDateFromString("1/2009"))
                this.pnlRequisitiVecchiaia.Visible = false;

            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2DG, liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.Value);

            if (liquidazione.areaLiquidazionePensioneFS.IsTrimestreAnzianitaRequisitiNoInvaliditaVisible.GetValueOrDefault())
                pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = true;

            if (liquidazione.areaLiquidazionePensioneFS.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() ||
                liquidazione.areaLiquidazionePensioneFS.IsBeneficioApePrecociFromFELPE.GetValueOrDefault() ||
                datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione ||
                datiPensione.SceltaLavoratriciMadri.HasValue || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                chkBenefici.Enabled = false;

            if (liquidazione.areaLiquidazionePensioneFS != null && ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                liquidazione.areaLiquidazionePensioneFS.IsBeneficioNonVedente.GetValueOrDefault()) ||
                liquidazione.areaLiquidazionePensioneFS.IsBeneficioNonVedenteFromStorico.GetValueOrDefault()))
            {
                chkBenefici.Checked = true;
                chkBenefici.Enabled = false;
            }
        }

        private void ManageScadenzaRevSanitaria_Privilegiate(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensione liquidazione)
        {
            //if (datiPensione.CodeGruppo.Equals("0002")) //invalidità
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Inabilita_Invalidita)
            {
                this.TrScadenzaRevisioneSanitaria.Visible = true;
                pnlCustomCheckPrivilegiate_FS_PT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
            }

            if ((tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura) && this.domanda.Categoria.StartsWith("I"))
            {
                pnlCustomCheckPrivilegiate_FS_PT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
            }
        }

        private void ManageRichiestaBonus(AreaTitolare.DatiPensione datiPensione, DatiGenerici datiGenerici)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, false) && (datiPensione.CodeProdotto == "0101" || datiPensione.CodeProdotto == "0301" || datiPensione.CodeProdotto == "0401"))
            {
                pnlRichiestaBonus.Visible = true;
                if (datiPensione.CodeTipo == "0167")
                {
                    lblRichiestaBonus.Text = "Bonus 14°:";
                    chkRichiestaBonus.Checked = true;
                    chkRichiestaBonus.Enabled = false;
                    hdnAnnoRichiestaBonus14.Value = datiGenerici.AnnoDecorrenzaBonus;
                }
                else
                {
                    lblRichiestaBonus.Text = "Bonus 154:";
                }
                HiddenAnnoBonusBooking.Value = "SI";

                if (datiPensione.IsRichiestaBonus.HasValue && datiPensione.IsRichiestaBonus.Value)
                {
                    chkRichiestaBonus.Checked = true;
                    if (datiPensione.CodeTipo != "0167" && !string.IsNullOrEmpty(datiGenerici.AnnoDecorrenzaBonus))
                    {
                        txtAnnoBonus.Text = datiGenerici.AnnoDecorrenzaBonus;
                    }
                }
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                ddlTipoCalcolo.Enabled = false;
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            switch (datiPensione.CodeProdotto)
            {
                case "0101":
                case "0102":
                case "0104":
                case "0108":
                case "0111":
                case "0112":
                case "0120":
                case "0301":
                case "0302":
                case "0304":
                case "0308":
                case "0311":
                case "0312":
                case "0320":
                case "0401":
                case "0402":
                case "0404":
                case "0408":
                case "0411":
                case "0412":
                case "0420":
                    TrScadenzaRevisioneSanitaria.Visible = false;
                    if (!(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && CodeUtility.IsRicostituzione(datiPensione)))
                        ddlTipoCalcolo.Enabled = false;
                    if (!(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                            this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                        ddlCodNatura1DG.Enabled = false;
                    ddlCodNatura2DG.Enabled = false;
                    ddlCodNatura3DG.Enabled = false;
                    chkTrasfAOI.Enabled = false;
                    pdivElimContestuale.Visible = false;
                    pnlRequisitiAnte247.Visible = false;
                    pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                    break;
                default:
                    break;
            }

            ddlCodiciArretrati.Enabled = false;
            ddlCodiciArretrati.SelectedValue = "8";
            ddlCausaCarico.Enabled = false;

            if (trBonus2432004.Visible)
            {
                if (datiPensione.CodeProdotto != "0107" && datiPensione.CodeProdotto != "0307" && datiPensione.CodeProdotto != "0407" &&
                    datiPensione.CodeProdotto != "0102" && datiPensione.CodeProdotto != "0302" && datiPensione.CodeProdotto != "0402" &&
                    datiPensione.CodeProdotto != "0108" && datiPensione.CodeProdotto != "0308" && datiPensione.CodeProdotto != "0408" &&
                    !this.domanda.IsDomandaRiapertura)
                {
                    CodeUtility.BloccaForm((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], pnlBonus2432004);
                    //ENG - RIC fondi FS, PT (prodotto 0109/0309/0409 e tipo= 0130 o prodotto= 0104/0304/0404 e tipo= 0001) rendere editabile il campo "Data Fine Bonus" per le domande che hanno il pannello L.243/2004
                    if (this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && (((datiPensione.CodeProdotto == "0109" || datiPensione.CodeProdotto == "0309" || datiPensione.CodeProdotto == "0409") && datiPensione.CodeTipo == "0130") ||
                        ((datiPensione.CodeProdotto == "0104" || datiPensione.CodeProdotto == "0304" || datiPensione.CodeProdotto == "0404") && datiPensione.CodeTipo == "0001")))
                    {
                        txtDataFineBonus.CssClass += (" dateMMaaaa date-picker");
                        txtDataFineBonus.Enabled = true;
                    }
                }
                else if ((datiPensione.CodeProdotto == "0108" || datiPensione.CodeProdotto == "0308" || datiPensione.CodeProdotto == "0408") ||
                         ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                          (datiPensione.CodeProdotto == "0107" || datiPensione.CodeProdotto == "0307" || datiPensione.CodeProdotto == "0407")))
                {
                    ddlAttribuzioneBonus.Enabled = false;
                }
            }

            btnEliminaDatiGenerici.Enabled = false;

            if (CodeUtility.IsRicostituzioneVariazioneDatiContitolari(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                ddlTipoCalcolo.Enabled = false;

            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                ddlCodComunicazioni3.Enabled = false;
            }
        }

        private void GestioneEtichetteRipristini(AreaTitolare.DatiPensione datiPensione)
        {
            ddlTipoCalcolo.Enabled = false;
            chkTrasfAOI.Enabled = false;
        }

        private void LoadDdlFromTipoFondo(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura.Count() > 0)
            {
                ddlCodNatura1DG.Items.Clear();
                ddlCodNatura2DG.Items.Clear();
                ddlCodNatura3DG.Items.Clear();
                CodeUtility.SetValueDdl(ddlCodNatura2DG, string.Empty, string.Empty, " ");
                if (tipoFondo.HasValue && tipoFondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
                    CodeUtility.SetValueDdl(ddlCodNatura3DG, string.Empty, string.Empty, " ");

                foreach (Presenter.SvrLiquidazioneFs.CodiciNatura codiceNatura in liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura)
                    if (codiceNatura.Posizione == 1)
                        CodeUtility.SetValueDdl(ddlCodNatura1DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                    else if (codiceNatura.Posizione == 2)
                        CodeUtility.SetValueDdl(ddlCodNatura2DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                    else
                        CodeUtility.SetValueDdl(ddlCodNatura3DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
            }
        }

        private void LoadDdlCommon(ILiquidazionePensione liquidazione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica, AreaTitolare.DatiPensione datiPensione)
        {
            ddlTipoCalcolo.Items.Clear();
            ddlTipoCalcolo.Items.Add(new ListItem(string.Empty, " "));
            foreach (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.TipoCalcolo tipoCalcolo in liquidazione.areaLiquidazionePensioneFS.ListaTipoCalcolo)
                CodeUtility.SetValueDdl(ddlTipoCalcolo, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);

            //load ddl causa carico
            Presenter.SvrLiquidazione.AreaDecodifica.DatiCausaCarico[] listaCausaCarico = CodeUtility.FS_GetDdlCausaCaricoByTipoDomanda(GetDatiPensione(this), datiDecodifica.ElencoCausaCarico).ToArray();
            ddlCausaCarico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCausaCarico);
            foreach (AreaDecodifica.DatiCausaCarico causaCarico in listaCausaCarico)
                CodeUtility.SetValueDdl(ddlCausaCarico, causaCarico.Descrizione, causaCarico.Descrizione, causaCarico.Id);

            AreaDecodifica.DatiComunicazioneCampi1_2[] listaComunicazioneC1_2 = datiDecodifica.ElencoComunicazioneCampi1_2;
            ddlCodComunicazioni2.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampi1_2 comunicazioneC1_2 in listaComunicazioneC1_2)
                CodeUtility.SetValueDdl(ddlCodComunicazioni2, comunicazioneC1_2.Campo2.ToString(), comunicazioneC1_2.Descrizione, comunicazioneC1_2.Campo2.ToString());

            SetDdlCodiceComunicazione3(datiDecodifica, liquidazione);

            AreaDecodifica.DatiComunicazioneCampo4[] listaComunicazioneC4 = datiDecodifica.ElencoComunicazioneCampo4;
            ddlCodComunicazioni4.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampo4 comunicazioneCampo4 in listaComunicazioneC4)
            {
                if (comunicazioneCampo4.Id == string.Empty)
                    comunicazioneCampo4.Descrizione = "NESSUNA ESENZIONE";

                if (CodeUtility.LoadRecordEsenzioneFiscaleFS(comunicazioneCampo4.Id, liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleEstero, liquidazione.areaLiquidazionePensioneFS.IsCodComunicazioniEsenzioneFiscaleVittimaVisibile, tipoFondo, this.domanda.IsDomandaINPDAP, this.domanda.IsDomandaRiapertura, datiPensione))
                    if (!this.domanda.Categoria.StartsWith("S") && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)
                            && comunicazioneCampo4.Id == "1")
                        CodeUtility.SetValueDdl(ddlCodComunicazioni4, "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", comunicazioneCampo4.Id);
                    else
                        if ((Utility.IsRicostituzione(datiPensione) || this.domanda.IsDomandaRiapertura)
                            && (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                            && comunicazioneCampo4.Id == "2")
                        {
                            if (liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                                CodeUtility.SetValueDdl(ddlCodComunicazioni4, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
                        }
                        else
                            CodeUtility.SetValueDdl(ddlCodComunicazioni4, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
            }


            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceEliminazione != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceEliminazione.Count() > 0)
                {
                    if (ddlElContCodice.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlElContCodice, string.Empty, string.Empty, string.Empty);
                        foreach (Presenter.SvrLiquidazioneFs.CodiceEliminazione codeEliminazione in liquidazione.areaLiquidazionePensioneFS.ListaCodiceEliminazione)
                            CodeUtility.SetValueDdl(ddlElContCodice, codeEliminazione.TestoVideo, codeEliminazione.Descrizione, codeEliminazione.Id);
                    }
                }

                if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare.Count() > 0)
                {
                    if (ddlDeroga.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlDeroga, string.Empty, string.Empty, string.Empty);
                        foreach (Presenter.SvrLiquidazioneFs.CodiceParticolare codeParticolare in liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare)
                            CodeUtility.SetValueDdl(ddlDeroga, codeParticolare.TraduzioneSuGp.GetValueOrDefault().ToString() + " - " + codeParticolare.Descrizione, codeParticolare.Descrizione, codeParticolare.Id.ToString());
                    }
                }
                //ENG - 024 - Esezione Fiscale Vittima
                if ((CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) || Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
                    && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS))
                {
                    if (liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.HasValue && liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.Value &&
                        (liquidazione.areaLiquidazionePensioneFS.DatiGenerici == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo4.HasValue || liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceComunicazioneCampo4.Value != 1))
                    {
                        if (ddlCodComunicazioni4.Items.FindByValue("1") != null)
                        {
                            ddlCodComunicazioni4.Items.Remove(ddlCodComunicazioni4.Items.FindByValue("1"));
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        internal void SetHiddenPrecedentePensioneValue(string value)
        {
            this.HiddenPrecedentePensione.Value = value;
        }
        private void SetDefaultValue()
        {
            txtScadRevSanitaria.Text = "mm/aaaa";
            txtDataInizioBonus.Text = "mm/aaaa";
            txtDataFineBonus.Text = "mm/aaaa";
            txtTrimestreRequisiti.Text = "aaaa";
            txtAnzAnni.Text = "aa";
            txtSemestreRequisiti.Text = "aaaa";
            txtAnzAnniSperDonna.Text = "aa";
        }

        private void ManageProvvisoria(ILiquidazionePensione liquidazione)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null)
            {
                if (this.domanda.Tipofondo.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                    ddlCodComunicazioni3.Enabled = false;
                else
                {
                    if (!checkMemo74_88())
                    {
                        ddlCodComunicazioni3.Enabled = !Utility.IsDomandaProvvisoria(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.IsProvvisoria);
                    }
                    
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceComunicazione3Visible.HasValue)
            {
                ddlCodComunicazioni3.Visible = liquidazione.areaLiquidazionePensioneFS.IsCodiceComunicazione3Visible.Value;
            }
        }

        private void ManageDeroga(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null &&
                liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceParticolareSoggettoDerogato.HasValue)
            {
                pnlDeroga.Visible = true;
                ddlDeroga.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.CodiceParticolareSoggettoDerogato.Value.ToString();
            }
        }

        private void ManageCodNatura3(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
            {
                if (!ddlCodNatura3DG.Items.Contains(new ListItem("Z", "Z")))
                    ddlCodNatura3DG.Items.Add(new ListItem("Z", "Z"));
                ddlCodNatura3DG.SelectedValue = "Z";
                ddlCodNatura3DG.Enabled = false;
            }

            //ENG - RIC Lavoratori Faticosi e Pesanti
            if (datiPensione != null && CodeUtility.IsRicostituzione(datiPensione) && datiPensione.IdTipoPLPerRIC == 32 &&
                (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                if (!ddlCodNatura3DG.Items.Contains(new ListItem("Z", "Z")))
                    ddlCodNatura3DG.Items.Add(new ListItem("Z", "Z"));

                if (!String.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Length >= 3 && datiPensione.NaturaPensione.Substring(2, 1).ToUpperInvariant() == "Z")
                {
                    ddlCodNatura3DG.SelectedValue = "Z";
                    ddlCodNatura3DG.Enabled = false;
                }
            }
        }

        private void ManageSperDonna(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione)
        {
            if (IsDomandaSperDonna)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && (liquidazione.areaLiquidazionePensioneFS.DatiGenerici == null || string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione)))
                {
                    ddlCodNatura2DG.ClearSelection();
                    if (ddlCodNatura2DG.Items.FindByValue("O") != null)
                        ddlCodNatura2DG.SelectedValue = "O";
                }
                ddlCodNatura2DG.Enabled = false;
            }
        }

        private void ManageForPensioniVecchiaiaCalcoloContrib(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            //FG - Controlli tipo contributivo - solo per fondi ET-TT-EL-VL
            if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivo.GetValueOrDefault()
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione
                || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo) //ENG - MEMO 166/2023
            {
                var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                if (itemTipoCalcolo != null)
                {
                    ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                    if (!(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL && CodeUtility.IsRicostituzione(datiPensione)))
                        ddlTipoCalcolo.Enabled = false;
                }
            }
        }

        #endregion private

        #region VL

        private void ValorizzaEtichetteCommonFomPensioneFondoVL(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblDecorrenzaPensione.Text = "Decorrenza Pensione:";

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL != null)
            {
                ddlCodNatura1DG.ClearSelection();
                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione))
                {
                    try
                    {
                        ddlCodNatura1DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(0, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura2DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(1, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura3DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(2, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                }
                else
                {
                    ddlCodNatura1DG.SelectedIndex = 0;
                    ddlCodNatura2DG.SelectedIndex = 0;
                    ddlCodNatura3DG.SelectedIndex = 0;
                }

                if (pnlRequisitiAnte247.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.Value)
                        ddlReqAnte247.SelectedValue = "SI";
                    else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.Value)
                        ddlReqAnte247.SelectedValue = "NO";

                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.NumeroTriSemRequisiti.HasValue)
                        ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.NumeroTriSemRequisiti.ToString();

                    txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnnoRequisiti.Value.ToString() : string.Empty;
                    txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnzianitaAnni.ToString() : string.Empty;
                }
                else if (IsDomandaSperDonna)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.Value)
                        ddlSperimentaleDonna.SelectedValue = "SI";
                    else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.Requisiti247_243.Value)
                        ddlSperimentaleDonna.SelectedValue = "NO";

                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.NumeroTriSemRequisiti.HasValue)
                        ddlSemestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.NumeroTriSemRequisiti.ToString();

                    txtSemestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnnoRequisiti.ToString() : string.Empty;
                    txtAnzAnniSperDonna.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnzianitaAnni.ToString() : string.Empty;
                }
                else if (pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.NumeroTriSemRequisiti.HasValue)
                        ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.NumeroTriSemRequisiti.ToString();

                    txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnnoRequisiti.Value.ToString() : string.Empty;
                    txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoVL.AnzianitaAnni.ToString() : string.Empty;
                }

                //Eng - Ricostituzioni che hanno i dati della Legge 336 sul tab Maggiorazioni
                //sul Pannello Liquidazione Pensione il flag “Ex-combattente” deve essere preselezionato e bloccato
                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.ExCombattente != null)
                    if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenerici.ExCombattente)
                    {
                        if (CodeUtility.IsRicostituzione(datiPensione))
                        {
                            if (liquidazione.areaLiquidazionePensioneFS.isSenzaLegge33670.HasValue)
                            {
                                chkExCombattente.Checked = true;
                                chkExCombattente.Enabled = false;
                            }
                        }
                    }
            }

            ManageCodNatura3(liquidazione, datiPensione);
        }

        private DatiGenerici GetDatiGenericiToPensioneFondoVL(DatiGenerici datiGenerici)
        {
            datiGenerici.fondoVL = new DatiGenerici.FondoVL();

            if (pnlRequisitiAnte247.Visible)
            {
                if (String.Equals(ddlReqAnte247.SelectedValue, "SI"))
                    datiGenerici.fondoVL.Requisiti247_243 = true;
                else if (String.Equals(ddlReqAnte247.SelectedValue, "NO"))
                    datiGenerici.fondoVL.Requisiti247_243 = false;

                datiGenerici.fondoVL.NumeroTriSemRequisiti = !String.IsNullOrEmpty(ddlTrimestreRequisiti.SelectedValue) ? byte.Parse(ddlTrimestreRequisiti.SelectedValue) : (!string.IsNullOrEmpty(hdnTrimesteRequisiti.Value) ? byte.Parse(hdnTrimesteRequisiti.Value) : (byte?)null);
                datiGenerici.fondoVL.AnnoRequisiti = (!String.Equals(txtTrimestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtTrimestreRequisiti.Text))) ? Int16.Parse(txtTrimestreRequisiti.Text) : (!string.IsNullOrEmpty(hdnTrimesteRequisitiAnno.Value) ? Int16.Parse(hdnTrimesteRequisitiAnno.Value) : (short?)null);
                datiGenerici.fondoVL.AnzianitaAnni = (!String.Equals(txtAnzAnni.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnni.Text))) ? Int32.Parse(txtAnzAnni.Text) : (int?)null;
            }
            else if (this.pnlSperimentaleDonna.Visible)
            {
                if (String.Equals(ddlSperimentaleDonna.SelectedValue, "SI"))
                    datiGenerici.fondoVL.Requisiti247_243 = true;
                else if (String.Equals(ddlSperimentaleDonna.SelectedValue, "NO"))
                    datiGenerici.fondoVL.Requisiti247_243 = false;

                datiGenerici.fondoVL.NumeroTriSemRequisiti = !String.IsNullOrEmpty(ddlSemestreRequisiti.SelectedValue) ? byte.Parse(ddlSemestreRequisiti.SelectedValue) : (byte?)null;
                datiGenerici.fondoVL.AnnoRequisiti = (!String.Equals(txtSemestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtSemestreRequisiti.Text))) ? Int16.Parse(txtSemestreRequisiti.Text) : (short?)null;
                datiGenerici.fondoVL.AnzianitaAnni = (!String.Equals(txtAnzAnniSperDonna.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnniSperDonna.Text))) ? Int32.Parse(txtAnzAnniSperDonna.Text) : (int?)null;
            }
            else if (pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible)
            {
                datiGenerici.fondoVL.NumeroTriSemRequisiti = !String.IsNullOrEmpty(ddlTrimestreRequisiti.SelectedValue) ? byte.Parse(ddlTrimestreRequisiti.SelectedValue) : (byte?)null;
                datiGenerici.fondoVL.AnnoRequisiti = (!String.Equals(txtTrimestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtTrimestreRequisiti.Text))) ? Int16.Parse(txtTrimestreRequisiti.Text) : (short?)null;
                datiGenerici.fondoVL.AnzianitaAnni = (!String.Equals(txtAnzAnni.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnni.Text))) ? Int32.Parse(txtAnzAnni.Text) : (int?)null;
            }

            return datiGenerici;
        }

        #endregion VL

        #region FS

        private void ValorizzaEtichetteCommonFomPensioneFondoFS(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblDecorrenzaPensione.Text = "Decorrenza Giuridica:";
            txtDecorrenzaEconomica.Text = String.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria);
            lblAnzAnniSperDonnaSemestre.Text = "Anni servizio 247:";
            lblAnzAnniSperDonnaTrimestre.Text = "Anni servizio 247:";

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null)
            {
                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione))
                {
                    try
                    {
                        ddlCodNatura1DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(0, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura2DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(1, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura3DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(2, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                }
                else
                {
                    ddlCodNatura1DG.SelectedIndex = 0;
                    ddlCodNatura2DG.SelectedIndex = 0;
                    ddlCodNatura3DG.SelectedIndex = 0;
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST != null)
                {
                    //if(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.DecorrenzaEconomica.HasValue)
                    //    txtDecorrenzaEconomica.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.DecorrenzaEconomica.Value);
                    //txtDecorrenzaEconomica.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.DecorrenzaEconomica.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.DecorrenzaEconomica.Value) : Convert.ToString(datiPensione.DecorrenzaOriginaria);

                    if (pnlRequisitiAnte247.Visible)
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.Value)
                            ddlReqAnte247.SelectedValue = "SI";
                        else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.Value)
                            ddlReqAnte247.SelectedValue = "NO";

                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.TrimesteRequisiti.HasValue)
                            ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.TrimesteRequisiti.ToString();

                        txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnnoRequisiti.Value.ToString() : string.Empty;
                        txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnzianitaAnni.ToString() : string.Empty;
                    }
                    else
                    {
                        if (IsDomandaSperDonna)
                        {
                            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.Value)
                                ddlSperimentaleDonna.SelectedValue = "SI";
                            else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.RequisitiAnte247.Value)
                                ddlSperimentaleDonna.SelectedValue = "NO";

                            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.TrimesteRequisiti.HasValue)
                                ddlSemestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.TrimesteRequisiti.ToString();

                            txtSemestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnnoRequisiti.ToString() : string.Empty;
                            txtAnzAnniSperDonna.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoFST.AnzianitaAnni.ToString() : string.Empty;
                        }
                    }
                }
            }

            ManageCodNatura3(liquidazione, datiPensione);
        }

        private DatiGenerici GetDatiGenericiToPensioneFondoFS(DatiGenerici datiGenerici)
        {
            datiGenerici.fondoFST = new DatiGenerici.FondoFST();

            datiGenerici.fondoFST.DecorrenzaEconomica = !String.IsNullOrEmpty(txtDecorrenzaEconomica.Text) && !String.Equals(txtDecorrenzaEconomica.Text.ToUpperInvariant(), "GG/MM/AAAA") ? Utility.GetDateFromString(txtDecorrenzaEconomica.Text) : null;

            if (pnlRequisitiAnte247.Visible)
            {
                if (String.Equals(ddlReqAnte247.SelectedValue, "SI"))
                    datiGenerici.fondoFST.RequisitiAnte247 = true;
                else if (String.Equals(ddlReqAnte247.SelectedValue, "NO"))
                    datiGenerici.fondoFST.RequisitiAnte247 = false;

                datiGenerici.fondoFST.TrimesteRequisiti = !String.IsNullOrEmpty(ddlTrimestreRequisiti.SelectedValue) ? byte.Parse(ddlTrimestreRequisiti.SelectedValue) : (!string.IsNullOrEmpty(hdnTrimesteRequisiti.Value) ? byte.Parse(hdnTrimesteRequisiti.Value) : (byte?)null);
                datiGenerici.fondoFST.AnnoRequisiti = (!String.Equals(txtTrimestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtTrimestreRequisiti.Text))) ? Int16.Parse(txtTrimestreRequisiti.Text) : (!string.IsNullOrEmpty(hdnTrimesteRequisitiAnno.Value) ? Int16.Parse(hdnTrimesteRequisitiAnno.Value) : (short?)null);
                datiGenerici.fondoFST.AnzianitaAnni = (!String.Equals(txtAnzAnni.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnni.Text))) ? Int32.Parse(txtAnzAnni.Text) : (int?)null;
            }
            else
            {
                if (this.pnlSperimentaleDonna.Visible)
                {
                    if (String.Equals(ddlSperimentaleDonna.SelectedValue, "SI"))
                        datiGenerici.fondoFST.RequisitiAnte247 = true;
                    else if (String.Equals(ddlSperimentaleDonna.SelectedValue, "NO"))
                        datiGenerici.fondoFST.RequisitiAnte247 = false;

                    datiGenerici.fondoFST.TrimesteRequisiti = !String.IsNullOrEmpty(ddlSemestreRequisiti.SelectedValue) ? byte.Parse(ddlSemestreRequisiti.SelectedValue) : (byte?)null;
                    datiGenerici.fondoFST.AnnoRequisiti = (!String.Equals(txtSemestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtSemestreRequisiti.Text))) ? Int16.Parse(txtSemestreRequisiti.Text) : (short?)null;
                    datiGenerici.fondoFST.AnzianitaAnni = (!String.Equals(txtAnzAnniSperDonna.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnniSperDonna.Text))) ? Int32.Parse(txtAnzAnniSperDonna.Text) : (int?)null;
                }
            }

            return datiGenerici;
        }

        #endregion FS

        #region PT

        private void ValorizzaEtichetteCommonFomPensioneFondoPT(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblDecorrenzaPensione.Text = "Decorrenza Giuridica:";
            txtDecorrenzaEconomica.Text = String.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria);
            lblAnzAnniSperDonnaSemestre.Text = "Anni servizio 247:";
            lblAnzAnniSperDonnaTrimestre.Text = "Anni servizio 247:";

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.DecorrenzaEconomica.HasValue)
                    txtDecorrenzaEconomica.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.DecorrenzaEconomica.Value);
                //txtDecorrenzaEconomica.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.DecorrenzaEconomica.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.DecorrenzaEconomica.Value) : Convert.ToString(datiPensione.DecorrenzaOriginaria);
                txtFinestraMobile.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.FinestraMobile.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.FinestraMobile.Value) : string.Empty;

                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione))
                {
                    try
                    {
                        ddlCodNatura1DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(0, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura2DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(1, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura3DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.NaturaPensione.Substring(2, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                }
                else
                {
                    ddlCodNatura1DG.SelectedIndex = 0;
                    ddlCodNatura2DG.SelectedIndex = 0;
                    ddlCodNatura3DG.SelectedIndex = 0;
                }

                if (pnlRequisitiAnte247.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.Value)
                        ddlReqAnte247.SelectedValue = "SI";
                    else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.Value)
                        ddlReqAnte247.SelectedValue = "NO";

                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.TrimesteRequisiti.HasValue)
                        ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.TrimesteRequisiti.ToString();

                    txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnnoRequisiti.Value.ToString() : string.Empty;
                    txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnzianitaAnni.ToString() : string.Empty;
                }
                else
                {
                    if (IsDomandaSperDonna)
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.Value)
                            ddlSperimentaleDonna.SelectedValue = "SI";
                        else if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.RequisitiAnte247.Value)
                            ddlSperimentaleDonna.SelectedValue = "NO";

                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.TrimesteRequisiti.HasValue)
                            ddlSemestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.TrimesteRequisiti.ToString();

                        txtSemestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnnoRequisiti.ToString() : string.Empty;
                        txtAnzAnniSperDonna.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenerici.fondoPT.AnzianitaAnni.ToString() : string.Empty;
                    }
                }
            }

            ManageCodNatura3(liquidazione, datiPensione);
        }

        private DatiGenerici GetDatiGenericiToPensioneFondoPT(DatiGenerici datiGenerici)
        {
            datiGenerici.fondoPT = new DatiGenerici.FondoPT();

            datiGenerici.fondoPT.DecorrenzaEconomica = !String.IsNullOrEmpty(txtDecorrenzaEconomica.Text) && !String.Equals(txtDecorrenzaEconomica.Text.ToUpperInvariant(), "GG/MM/AAAA") ? Utility.GetDateFromString(txtDecorrenzaEconomica.Text) : null;
            datiGenerici.fondoPT.FinestraMobile = !String.IsNullOrEmpty(txtFinestraMobile.Text) && !String.Equals(txtFinestraMobile.Text.ToUpperInvariant(), "GG/MM/AAAA") ? Utility.GetDateFromString(txtFinestraMobile.Text) : null;

            if (pnlRequisitiAnte247.Visible)
            {
                if (String.Equals(ddlReqAnte247.SelectedValue, "SI"))
                    datiGenerici.fondoPT.RequisitiAnte247 = true;
                else if (String.Equals(ddlReqAnte247.SelectedValue, "NO"))
                    datiGenerici.fondoPT.RequisitiAnte247 = false;

                datiGenerici.fondoPT.TrimesteRequisiti = !String.IsNullOrEmpty(ddlTrimestreRequisiti.SelectedValue) ? byte.Parse(ddlTrimestreRequisiti.SelectedValue) : (!string.IsNullOrEmpty(hdnTrimesteRequisiti.Value) ? byte.Parse(hdnTrimesteRequisiti.Value) : (byte?)null);
                datiGenerici.fondoPT.AnnoRequisiti = (!String.Equals(txtTrimestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtTrimestreRequisiti.Text))) ? Int16.Parse(txtTrimestreRequisiti.Text) : (!string.IsNullOrEmpty(hdnTrimesteRequisitiAnno.Value) ? Int16.Parse(hdnTrimesteRequisitiAnno.Value) : (short?)null);
                datiGenerici.fondoPT.AnzianitaAnni = (!String.Equals(txtAnzAnni.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnni.Text))) ? Int32.Parse(txtAnzAnni.Text) : (int?)null;
            }
            else
            {
                if (this.pnlSperimentaleDonna.Visible)
                {
                    if (String.Equals(ddlSperimentaleDonna.SelectedValue, "SI"))
                        datiGenerici.fondoPT.RequisitiAnte247 = true;
                    else if (String.Equals(ddlSperimentaleDonna.SelectedValue, "NO"))
                        datiGenerici.fondoPT.RequisitiAnte247 = false;

                    datiGenerici.fondoPT.TrimesteRequisiti = !String.IsNullOrEmpty(ddlSemestreRequisiti.SelectedValue) ? byte.Parse(ddlSemestreRequisiti.SelectedValue) : (byte?)null;
                    datiGenerici.fondoPT.AnnoRequisiti = (!String.Equals(txtSemestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtSemestreRequisiti.Text))) ? Int16.Parse(txtSemestreRequisiti.Text) : (short?)null;
                    datiGenerici.fondoPT.AnzianitaAnni = (!String.Equals(txtAnzAnniSperDonna.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnniSperDonna.Text))) ? Int32.Parse(txtAnzAnniSperDonna.Text) : (int?)null;
                }
            }

            return datiGenerici;
        }

        #endregion PT

        protected void VerificaAdesioneFondoCredito()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.VerificaAdesioneFondoCredito(titolare.CodiceFiscale, this);

            if (this.HasError)
            {
                ddlTrattINPDAP.SelectedValue = "NO";
                txtDecTrattINPDAP.Text = "MM/AAAA";
                txtDecTrattINPDAP.Enabled = false;
                ddlTrattINPDAP.Enabled = false;
            }
            else
            {
                ddlTrattINPDAP.SelectedValue = "SI";
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = true;
            }
        }

        //ENG - Aggiornamento Memo86
        protected void VerificaAdesioneFondoCreditoAggiornamentoMemo86(bool? isPresenteTrattenutaFondoCreditoDaPrelievo, bool? IsDataRinunciaTrattenutaInpdapStorico)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.VerificaAdesioneFondoCredito(titolare.CodiceFiscale, this);
            bool isPresenteAdesioneFondoCredito = (this.HasError) ? false : true;

            //casistica blank, casistica discordante, casistica concordante NO
            if (!isPresenteTrattenutaFondoCreditoDaPrelievo.HasValue ||
                isPresenteTrattenutaFondoCreditoDaPrelievo.Value != isPresenteAdesioneFondoCredito ||
                (!isPresenteTrattenutaFondoCreditoDaPrelievo.Value && !isPresenteAdesioneFondoCredito))
            {
                if (!isPresenteAdesioneFondoCredito)
                {
                    ddlTrattINPDAP.SelectedValue = "NO";
                    txtDecTrattINPDAP.Text = "MM/AAAA";
                    txtDecTrattINPDAP.Enabled = false;
                    ddlTrattINPDAP.Enabled = false;
                }
                else
                {
                    ddlTrattINPDAP.SelectedValue = "SI";
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = true;
                }

                if (isPresenteTrattenutaFondoCreditoDaPrelievo.HasValue && isPresenteTrattenutaFondoCreditoDaPrelievo.Value != isPresenteAdesioneFondoCredito)
                    RaiseShowAvvisoTrattenutaFondoCredito(this, null);
            }
            else if (isPresenteTrattenutaFondoCreditoDaPrelievo.Value && isPresenteAdesioneFondoCredito) //casistica concordante SI
            {
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        #region Events

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event Utility.CustomEventHandler ShowError;
        //ENG - Aggiornamento Memo86
        public event EventHandler ShowAvvisoTrattenutaFondoCredito;

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseShowError(object sender, Utility.CustomEventArgs e)
        {
            ShowError(sender, e);
        }

        //ENG - Aggiornamento Memo86
        protected void RaiseShowAvvisoTrattenutaFondoCredito(object sender, EventArgs e)
        {
            ShowAvvisoTrattenutaFondoCredito(sender, e);
        }

        #endregion Events
    }
}
