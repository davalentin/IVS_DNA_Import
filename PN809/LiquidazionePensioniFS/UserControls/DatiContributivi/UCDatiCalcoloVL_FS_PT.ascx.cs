using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiCalcoloVL_FS_PT : CustomBaseUserControl, IDatiContributivi, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            RecuperaCampi(this.domanda.Tipofondo);
            SetSettimaneTotali();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiCalcolo(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiCalcolo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Calcolo";
            else
            {
                ClearForm();
                RaiseCaricaDatiCalcolo(this.areaDatiContributivi, null);
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteDatiCalcolo()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ViewState["TipoCalcolo"] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
            ViewState["RiduzioneRetrib"] = this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue ? this.areaDatiContributivi.IsRiduzioneRetribVisible.Value : (bool?)null;
            ViewState["ContribL214"] = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : (bool?)null;

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            FlagUnicarpe.Value = Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ? "SI" : "NO";  // Lettura_L è il termine di paragone assumendo che per Lettura_C i datiCalcolo da AggPeco sono non valorizzati

            RenderControlsFromTipoCalcolo_TipoFondo();
            RenderControlsFromFelpe_FSPT();

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    ValorizzaEtichetteDatiCalcoloVL();
                    ValorizzaEtichetteDatiCalcoloFondoVL();
                    ValorizzaEtichetteComma707(this.areaDatiContributivi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    ValorizzaEtichetteDatiCalcoloFS_PT();
                    break;
            }
            GestioneEtichetteIsUnicarpe(this.domanda.Tipofondo, datiPensione);

            if (areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.RetributivoMonti)
                ViewState["RetributivoMonti"] = "SI";

            HdnFondo.Value = this.domanda.Tipofondo.ToString().ToUpperInvariant();
            ManageButtons();
            GestioneEtichetteRic(datiPensione);
        }

        internal void RecuperaCampi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? fondo)
        {
            bool isContributivo = false;
            bool isRetributivo = false;

            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = (GestioneContribTipoCalcolo)ViewState["TipoCalcolo"];
            this.areaDatiContributivi.IsContribL214Visible = (bool?)ViewState["ContribL214"];
            this.areaDatiContributivi.IsRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];

            switch (fondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    RecuperaCampiDatiCalcoloVL(ref isContributivo, ref isRetributivo);
                    RecuperaCampiDatiCalcoloFondoVL(ref isContributivo, ref isRetributivo);
                    RecuperaCampiComma707(areaDatiContributivi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    RecuperaCampiDatiCalcoloFS_PT(ref isContributivo, ref isRetributivo);
                    break;
            }
            if (isContributivo && isRetributivo)
            {
                if (ViewState["RetributivoMonti"] != null && ViewState["RetributivoMonti"].ToString() == "SI")
                    this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.RetributivoMonti;
                else
                    this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Misto;
            }
            else if (isContributivo)
                this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Contributivo;
            else if (isRetributivo)
                this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Retributivo;
            else
                this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.NonValido;

            this.areaDatiContributivi.DatiCalcolo.IsCalcoloValido = true;
        }

        internal bool ManageButtonRiduzioneRetributiva(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            if (!tipoFondo.HasValue || tipoFondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || tipoFondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                return false;

            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            AreaTitolare.DatiPensione DatiPensione = this.GetDatiPensione(this);

            if (titolare != null && DatiPensione != null)
            {
                if (titolare.DataNascita.HasValue && DatiPensione.DecorrenzaOriginaria.HasValue)
                {
                    if (!(DateTime.Compare(titolare.DataNascita.Value.AddYears(57), DatiPensione.DecorrenzaOriginaria.Value) < 0) &&
                        (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal void EnableDisableBtnSalva(bool enable)
        {
            this.btnSalvaDatiCalcolo.Enabled = enable;
            this.btnPopUp.Enabled = enable;
            this.btnSalvaDatiCalcoloNoRiduzione.Enabled = enable;
            this.btnEliminaDatiCalcolo.Enabled = enable;
        }

        #region Private Methods

        private void RenderControlsFromTipoFondo()
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    pnlSettimane_VL.Visible = true;
                    if (this.areaDatiContributivi.IsAnteArmonizzazione.GetValueOrDefault())
                    {
                        pnlDatiCalcolo.Visible = false;
                        pnlAnteArmonizzazioneVL.Visible = true;
                    }
                    else
                        pnlDatiCommonVL.Visible = true;
                    switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                    {
                        case GestioneContribTipoCalcolo.Contributivo:
                            pnlDatiContributiviVL.Visible = true;
                            pnlDatiContributiviVLNoFelpe.Visible = true;
                            pnlDatiContributiviVLFelpe.Visible = false;
                            break;
                        case GestioneContribTipoCalcolo.Retributivo:
                            pnlDatiRetributiviVL.Visible = true;
                            pnlDatiRetributiviCustomVL.Visible = true;
                            ManageRiduzioneRetributiva();
                            break;
                        case GestioneContribTipoCalcolo.Misto:
                            pnlDatiContributiviVL.Visible = true;
                            pnlDatiRetributiviVL.Visible = true;
                            ManageRiduzioneRetributiva();
                            break;
                        case GestioneContribTipoCalcolo.RetributivoMonti:
                            pnlDatiRetributiviVL.Visible = true;
                            pnlDatiRetributiviCustomVL.Visible = true;
                            pnlDatiContributiviVL.Visible = false;
                            ManageRiduzioneRetributiva();
                            break;
                        case GestioneContribTipoCalcolo.NonValido:
                            break;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    revSettimaneUtiliDiritto.Enabled = false;
                    pnlDatiCommonFS_PT.Visible = true;
                    pnl336FS.Visible = true;
                    switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                    {
                        case GestioneContribTipoCalcolo.Contributivo:
                            pnlDatiContributiviFS_PT.Visible = true;
                            pnlDatiRetributiviFS_PT.Visible = false;
                            break;
                        case GestioneContribTipoCalcolo.Retributivo:
                            pnlDatiRetributiviFS_PT.Visible = true;
                            pnlDatiContributiviFS_PT.Visible = false;
                            break;
                        case GestioneContribTipoCalcolo.Misto:
                            pnlDatiContributiviFS_PT.Visible = true;
                            pnlDatiRetributiviFS_PT.Visible = true;
                            break;
                        case GestioneContribTipoCalcolo.RetributivoMonti:
                            pnlDatiRetributiviFS_PT.Visible = true;
                            break;
                        case GestioneContribTipoCalcolo.NonValido:
                            break;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    revSettimaneUtiliDiritto.Enabled = false;
                    pnlDatiCommonFS_PT.Visible = true;
                    switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                    {
                        case GestioneContribTipoCalcolo.Contributivo:
                            pnlDatiContributiviFS_PT.Visible = true;
                            pnlDatiRetributiviFS_PT.Visible = false;
                            break;
                        case GestioneContribTipoCalcolo.Retributivo:
                            pnlDatiRetributiviFS_PT.Visible = true;
                            pnlDatiContributiviFS_PT.Visible = false;
                            break;
                        case GestioneContribTipoCalcolo.Misto:
                            pnlDatiContributiviFS_PT.Visible = true;
                            pnlDatiRetributiviFS_PT.Visible = true;
                            break;
                        case GestioneContribTipoCalcolo.RetributivoMonti:
                            pnlDatiRetributiviFS_PT.Visible = true;
                            break;
                        case GestioneContribTipoCalcolo.NonValido:
                            break;
                    }
                    break;
                default:
                    break;
            }

            //ENG - Memo 79
            pnlNSettimane_OrganizzazioniInternazionali.Visible = Utility.IsDomandaOrganizzazioniInternazionali(datiPensione);
            if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
                lblNumeroSettimane.InnerText = "Numero Settimane Italiane";

        }

        private void RenderControlsFromTipoCalcolo_TipoFondo()
        {
            pnlComma707.Visible = areaDatiContributivi.IsSettimane707Visible ?? false;
            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    pnlDatiContributivi.Visible = true;
                    ManagePnlContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.Retributivo:
                    pnlDatiRetributivi.Visible = true;
                    ManagePnlContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.Misto:
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    pnlDatiContributivi.Visible = true;
                    pnlDatiRetributivi.Visible = true;
                    ManagePnlContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }
            RenderControlsFromTipoFondo();
        }

        private void RenderControlsFromFelpe_FSPT()
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        pnlAnniServizioUtiliDirittoPerAutomatiche.Visible = true;
                        txtPensioneAnnuaLorda.Enabled = false;
                        txtAnniServUtiliDirittoAA.Enabled = false;
                        txtAnniServUtiliDirittoMM.Enabled = false;
                        txtAnniServUtiliDirittoGG.Enabled = false;
                        //Quota A
                        txtServizioUtileAAQtaA.Enabled = false;
                        txtServizioUtileMMQtaA.Enabled = false;
                        txtServizioUtileGGQtaA.Enabled = false;
                        txtRetribuzioneQtaA.Enabled = false;
                        txtQuotaRetributivaAnnua.Enabled = false;
                        pnlQuotaRetributivaAnnua.Visible = true;
                        //Quota B1
                        txtServizioUtileAAQtaB1.Enabled = false;
                        txtServizioUtileMMQtaB1.Enabled = false;
                        txtServizioUtileGGQtaB1.Enabled = false;
                        txtRMSQtaB1.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB94.Enabled = false;
                        pnlQuotaRetributivaAnnuaB94.Visible = true;
                        //Quota B2
                        txtServizioUtileAAQtaB2.Enabled = false;
                        txtServizioUtileMMQtaB2.Enabled = false;
                        txtServizioUtileGGQtaB2.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB95.Enabled = false;
                        pnlQuotaPensioneRetributivaAnnuaB95.Visible = true;
                        //Quota B3
                        txtServizioUtileAAQtaB3.Enabled = false;
                        txtServizioUtileMMQtaB3.Enabled = false;
                        txtServizioUtileGGQtaB3.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB97.Enabled = false;
                        pnlQuotaPensioneRetributivaAnnuaB97.Visible = true;
                        //Quota B4 - Cessazione
                        txtServizioUtileCessazioneAA.Enabled = false;
                        txtServizioUtileCessazioneMM.Enabled = false;
                        txtServizioUtileCessazioneGG.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaCessazione.Enabled = false;
                        pnlQuotaPensioneRetributivaAnnuaCessazione.Visible = true;
                        //Quota C - DL335
                        txtImportoContributivoTotaleFS_PT.Enabled = false;
                        txtSettimaneFS_PT.Enabled = false;
                        txtMontanteFS_PT.Enabled = false;
                        txtImportoQuotaCFS_PT.Enabled = false;
                        //Quota D - DL214
                        txtImportoContribTotaleQuotaDL214.Enabled = false;
                        txtNSettimaneQuotaDL214.Enabled = false;
                        txtMontanteQuotaDL214.Enabled = false;
                        txtQuotaPensioneContributivaAnnuaDL214.Enabled = false;
                    }

                    //Ex comma 707
                    pnlComma707.Visible = false;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        if (areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto.HasValue)
                            txtSettimaneUtiliDiritto.Enabled = false;
                    }
                    break;
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? fondo, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.NonValido:
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        if (areaDatiContributivi.TipologiaSalvaguardia.HasValue || (areaDatiContributivi.IsUsuranti.HasValue && areaDatiContributivi.IsUsuranti.Value))
                        {
                            ddlRiduzioneRetributiva.Enabled = false;
                            txtRiduzioneRetributiva.Enabled = false;
                        }
                        else
                        {
                            ddlRiduzioneRetributiva.Enabled = true;
                            txtRiduzioneRetributiva.Enabled = true;
                        }
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        if (areaDatiContributivi.TipologiaSalvaguardia.HasValue || (areaDatiContributivi.IsUsuranti.HasValue && areaDatiContributivi.IsUsuranti.Value))
                        {
                            ddlRiduzioneRetributiva.Enabled = false;
                            txtRiduzioneRetributiva.Enabled = false;
                        }
                        else
                        {
                            ddlRiduzioneRetributiva.Enabled = true;
                            txtRiduzioneRetributiva.Enabled = true;
                        }
                        txtImportoContribTotaleQuotaDL214.Enabled = false;
                        txtMontanteQuotaDL214.Enabled = false;
                        txtNSettimaneQuotaDL214.Enabled = false;
                        break;
                }

                switch (fondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        GestioneEtichetteIsUnicarpeVL(areaDatiContributivi.DatiCalcolo.TipoCalcolo);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        GestioneEtichetteIsUnicarpeFS_PT(areaDatiContributivi.DatiCalcolo.TipoCalcolo);
                        break;
                }
                txtQuotaA1Comma707.Enabled = false;
                txtQuotaA2Comma707.Enabled = false;
                txtQuotaBComma707.Enabled = false;
                txtQuotaC1Comma707.Enabled = false;
                txtQuotaC2Comma707.Enabled = false;
                txtQuotaDComma707.Enabled = false;
            }
            else if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && areaDatiContributivi != null &&
                ((areaDatiContributivi.TipologiaSalvaguardia.HasValue) ||
                (areaDatiContributivi.IsUsuranti.HasValue && areaDatiContributivi.IsUsuranti.Value)))
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        ddlRiduzioneRetributiva.Enabled = false;
                        txtRiduzioneRetributiva.Enabled = false;
                        break;
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.NonValido:
                        break;
                }
            }

        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            // popolamento dei controls html con i valori di default (es.: txtPippo.Text = "mm/aaaa";)
        }

        private void ManageRiduzioneRetributiva()
        {
            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
                pnlRiduzioneRetributiva.Visible = this.areaDatiContributivi.IsRiduzioneRetribVisible.Value;

            bool IsRiduzionePresent = ManageButtonRiduzioneRetributiva(this.domanda.Tipofondo);
            //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
            if (IsRiduzionePresent && this.areaDatiContributivi != null &&
                ((this.areaDatiContributivi.IsUsuranti.HasValue && this.areaDatiContributivi.IsUsuranti.Value) ||
                (this.areaDatiContributivi.TipologiaSalvaguardia.HasValue) ||
                (this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.Value)))
                IsRiduzionePresent = false;

            btnSalvaDatiCalcoloNoRiduzione.Visible = !IsRiduzionePresent;
            btnPopUp.Visible = IsRiduzionePresent;
            btnSalvaDatiCalcolo.Visible = IsRiduzionePresent;

            if (this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.Value)
            {
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
        }

        private void ManagePnlContributivoLegge214()
        {
            pnlDatiContributivi.Visible = !pnlDatiContributivi.Visible ? this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false : true;
            pnlDatiCalcoloContributiviLegge214_VL_FS_PT.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
        }

        private void ManageButtons()
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (this.TitolarePensione.Pensione.TipoLetturaUnicarpe != 'L' && Utility.DataSuccessivaA(this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
            {
                btnPopUpContributivi.Style.Remove("display");
                btnPopUp.Style.Remove("display");
                btnSalvaDatiCalcolo.Style.Remove("display");
                btnSalvaDatiCalcoloNoRiduzione.Style.Remove("display");

                btnPopUp.Style.Add("display", "none");
                btnSalvaDatiCalcolo.Style.Add("display", "none");
                btnSalvaDatiCalcoloNoRiduzione.Style.Add("display", "none");

                RaiseShowPopUp(this, null);
                return;
            }

            btnPopUpContributivi.Style.Remove("display");
            btnPopUp.Style.Remove("display");
            btnSalvaDatiCalcolo.Style.Remove("display");
            btnSalvaDatiCalcoloNoRiduzione.Style.Remove("display");

            btnPopUpContributivi.Style.Add("display", "none");
            btnSalvaDatiCalcolo.Style.Add("display", "none");

            RaiseHidePopUp(this, null);
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
            {
                txtSettimaneUtiliDiritto.Enabled = false;
                txtRetribuzioneMediaSettADatiRetrib.Enabled = false;
                txtSettimaneA1DatiRetrib.Enabled = false;
                txtSettimaneA2DatiRetrib.Enabled = false;
                txtRetribuzioneMediaSettBDatiRetrib.Enabled = false;
                txtSettimaneBDatiRetrib.Enabled = false;
                txtSettimaneC1DatiRetrib.Enabled = false;
                txtSettimaneC2DatiRetrib.Enabled = false;
                txtRetribuzioneMediaSettDDatiRetrib.Enabled = false;
                txtSettimaneDDatiRetrib.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
                txtImportTotale335_VL.Enabled = false;
                txtMontante_VL.Enabled = false;
                txtMontanteDa0196a0697_VL.Enabled = false;
                txtA96_VL.Enabled = false;
                txtM96_VL.Enabled = false;
                txtG96_VL.Enabled = false;
                txtMontanteDa0697_VL.Enabled = false;
                txtA97_VL.Enabled = false;
                txtM97_VL.Enabled = false;
                txtG97_VL.Enabled = false;
                txtImportoContribTotaleQuotaDL214.Enabled = false;
                txtImportoContribTotaleQuotaDL214RF.Enabled = false;
                txtNSettimaneQuotaDL214.Enabled = false;
                RequiredFieldValidator2.Enabled = false;
                txtMontanteQuotaDL214.Enabled = false;
                RequiredFieldValidator1.Enabled = false;
                txtQuotaPensioneContributivaAnnuaDL214.Enabled = false;
                chkLavoratorePrecoce.Enabled = false;
                txtQuotaA1Comma707.Enabled = false;
                txtQuotaA2Comma707.Enabled = false;
                txtQuotaBComma707.Enabled = false;
                txtQuotaC1Comma707.Enabled = false;
                txtQuotaC2Comma707.Enabled = false;
                txtQuotaDComma707.Enabled = false;
                txtRetrPensAnnuaQuotaA.Enabled = false;
                RFVtxtRetrPensAnnuaQuotaA.Enabled = false;
                txtControcodiceRetributivoQuotaA.Enabled = false;
                txtServizioUtileAnte271188AA.Enabled = false;
                RFV.Enabled = false;
                txtServizioUtileAnte271188MM.Enabled = false;
                RFVtxtServizioUtileAnte271188MM.Enabled = false;
                txtServizioUtileAnte271188GG.Enabled = false;
                RFVtxtServizioUtileAnte271188GG.Enabled = false;
                txtServizioUtileAnte93AA.Enabled = false;
                RFVtxtServizioUtileAnte93AA.Enabled = false;
                txtServizioUtileAnte93MM.Enabled = false;
                RFVtxtServizioUtileAnte93MM.Enabled = false;
                txtServizioUtileAnte93GG.Enabled = false;
                RFVtxtServizioUtileAnte93GG.Enabled = false;
                txtRetrPensAnnuaQuotaB.Enabled = false;
                txtControcodiceRetributivoQuotaB.Enabled = false;
                txtServizioUtilePost311292AA.Enabled = false;
                txtServizioUtilePost311292MM.Enabled = false;
                txtServizioUtilePost311292GG.Enabled = false;
                txtServizioUtilePost94AA.Enabled = false;
                txtServizioUtilePost94MM.Enabled = false;
                txtServizioUtilePost94GG.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }
        }
        #endregion Private Methods

        #region VL

        private void ValorizzaEtichetteDatiCalcoloVL()
        {
            txtSettimaneUtiliDiritto.Text = areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto.HasValue ? areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto.ToString() : string.Empty;
            txtNumeroSettimaneOI.Text = areaDatiContributivi.DatiCalcolo.SettimaneUtiliDirittoOI.HasValue ? areaDatiContributivi.DatiCalcolo.SettimaneUtiliDirittoOI.ToString() : string.Empty;
            txtRetribuzioneMediaSettADatiRetrib.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaA.Value.ToString("0.0000") : string.Empty;
            txtSettimaneA1DatiRetrib.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.Value.ToString() : string.Empty;
            txtSettimaneA2DatiRetrib.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA2.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA2.Value.ToString() : string.Empty;
            txtRetribuzioneMediaSettBDatiRetrib.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaB.Value.ToString("0.0000") : string.Empty;
            txtSettimaneBDatiRetrib.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.Value.ToString() : string.Empty;
            txtSettimaneC1DatiRetrib.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC.Value.ToString() : string.Empty;
            txtSettimaneC2DatiRetrib.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC2.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC2.Value.ToString() : string.Empty;
            txtRetribuzioneMediaSettDDatiRetrib.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimaneDDatiRetrib.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.Value.ToString() : string.Empty;

            txtImportTotale335_VL.Text = areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            //commentato a seguito della mail del 30-10-2013 RE: Reeng Pensioni FS - Segnalazione di produzione fondo Volo
            //if (FlagUnicarpe.Value.Equals("SI"))
            //{
            //    txtMontante_VL.Text = areaDatiContributivi.DatiCalcolo.Montante.HasValue ? areaDatiContributivi.DatiCalcolo.Montante.Value.ToString() : string.Empty;
            //}
            //else
            //{
            //    txtMontanteDa0196a0697_VL.Text = areaDatiContributivi.DatiCalcolo.MontanteAnte0697.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteAnte0697.Value.ToString() : string.Empty;
            //    txtA96_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697AA.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697AA.Value.ToString() : string.Empty;
            //    txtM96_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697MM.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697MM.Value.ToString() : string.Empty;
            //    txtG96_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697GG.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697GG.Value.ToString() : string.Empty;
            //    txtMontanteDa0697_VL.Text = areaDatiContributivi.DatiCalcolo.Montante.HasValue ? areaDatiContributivi.DatiCalcolo.Montante.Value.ToString() : string.Empty;
            //    txtA97_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaPost0697AA.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaPost0697AA.Value.ToString() : string.Empty;
            //    txtM97_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaPost0697MM.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaPost0697MM.Value.ToString() : string.Empty;
            //    txtG97_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaPost0697GG.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaPost0697GG.Value.ToString() : string.Empty;
            //} 

            txtMontante_VL.Text = areaDatiContributivi.DatiCalcolo.Montante.HasValue ? areaDatiContributivi.DatiCalcolo.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontanteDa0196a0697_VL.Text = areaDatiContributivi.DatiCalcolo.MontanteAnte0697.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteAnte0697.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtA96_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697AA.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697AA.Value.ToString() : string.Empty;
            txtM96_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697MM.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697MM.Value.ToString() : string.Empty;
            txtG96_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697GG.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697GG.Value.ToString() : string.Empty;
            txtMontanteDa0697_VL.Text = areaDatiContributivi.DatiCalcolo.Montante.HasValue ? areaDatiContributivi.DatiCalcolo.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtA97_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaPost0697AA.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaPost0697AA.Value.ToString() : string.Empty;
            txtM97_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaPost0697MM.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaPost0697MM.Value.ToString() : string.Empty;
            txtG97_VL.Text = areaDatiContributivi.DatiCalcolo.AnzianitaPost0697GG.HasValue ? areaDatiContributivi.DatiCalcolo.AnzianitaPost0697GG.Value.ToString() : string.Empty;

            if (areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva)
                ddlRiduzioneRetributiva.SelectedValue = "SI";
            else ddlRiduzioneRetributiva.SelectedValue = "NO";
            txtRiduzioneRetributiva.Text = areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.HasValue ? areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

            txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString() : string.Empty;

            SetSettimaneTotali();
        }

        public void ValorizzaEtichetteComma707(AreaDatiContributivi areaDatiContributivi)
        {
            //Comma 707
            #region Comma 707
            txtQuotaA1Comma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707.Value.ToString() : string.Empty;
            txtQuotaA2Comma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaA2707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA2707.Value.ToString() : string.Empty;
            txtQuotaBComma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707.Value.ToString() : string.Empty;
            txtQuotaC1Comma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaC707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaC707.Value.ToString() : string.Empty;
            txtQuotaC2Comma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaC2707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaC2707.Value.ToString() : string.Empty;
            txtQuotaDComma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaD707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaD707.Value.ToString() : string.Empty;
            #endregion Comma 707
        }

        private void ValorizzaEtichetteDatiCalcoloFondoVL()
        {
            chkLavoratorePrecoce.Checked = areaDatiContributivi.DatiCalcolo.fondoVL != null && areaDatiContributivi.DatiCalcolo.fondoVL.LavoratorePrecoce.HasValue ? areaDatiContributivi.DatiCalcolo.fondoVL.LavoratorePrecoce.Value : false;

            if (this.areaDatiContributivi.DatiCalcolo.fondoVL != null && this.areaDatiContributivi.DatiCalcolo.fondoVL.LServizioUtile != null &&
                this.areaDatiContributivi.DatiCalcolo.fondoVL.LServizioUtile.Count() > 0)
            {
                foreach (GestioneContribDatiServizioUtile servizioUtile in this.areaDatiContributivi.DatiCalcolo.fondoVL.LServizioUtile)
                {
                    switch (servizioUtile.Quota)
                    {
                        // Servizio Utile Ante 27/11/88
                        case "A":
                            if (servizioUtile.RetribuzionePensionabile.HasValue)
                                txtRetrPensAnnuaQuotaA.Text = servizioUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                            if (servizioUtile.ServizioUtileAA.HasValue)
                                txtServizioUtileAnte271188AA.Text = servizioUtile.ServizioUtileAA.Value.ToString();
                            if (servizioUtile.ServizioUtileMM.HasValue)
                                txtServizioUtileAnte271188MM.Text = servizioUtile.ServizioUtileMM.Value.ToString();
                            if (servizioUtile.ServizioUtileGG.HasValue)
                                txtServizioUtileAnte271188GG.Text = servizioUtile.ServizioUtileGG.Value.ToString();
                            if (servizioUtile.ControCodiceRetributivo.HasValue)
                                txtControcodiceRetributivoQuotaA.Text = servizioUtile.ControCodiceRetributivo.Value.ToString();
                            break;
                        // Servizio Utile Ante '93
                        case "A2":
                            if (servizioUtile.ServizioUtileAA.HasValue)
                                txtServizioUtileAnte93AA.Text = servizioUtile.ServizioUtileAA.Value.ToString();
                            if (servizioUtile.ServizioUtileMM.HasValue)
                                txtServizioUtileAnte93MM.Text = servizioUtile.ServizioUtileMM.Value.ToString();
                            if (servizioUtile.ServizioUtileGG.HasValue)
                                txtServizioUtileAnte93GG.Text = servizioUtile.ServizioUtileGG.Value.ToString();
                            break;
                        // Servizio Utile quota B
                        case "B":
                            if (servizioUtile.RetribuzionePensionabile.HasValue)
                                txtRetrPensAnnuaQuotaB.Text = servizioUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                            if (servizioUtile.ServizioUtileAA.HasValue)
                                txtServizioUtilePost311292AA.Text = servizioUtile.ServizioUtileAA.Value.ToString();
                            if (servizioUtile.ServizioUtileMM.HasValue)
                                txtServizioUtilePost311292MM.Text = servizioUtile.ServizioUtileMM.Value.ToString();
                            if (servizioUtile.ServizioUtileGG.HasValue)
                                txtServizioUtilePost311292GG.Text = servizioUtile.ServizioUtileGG.Value.ToString();
                            if (servizioUtile.ControCodiceRetributivo.HasValue)
                                txtControcodiceRetributivoQuotaB.Text = servizioUtile.ControCodiceRetributivo.Value.ToString();
                            break;
                        // Servizio utile post '94
                        case "C":
                            if (servizioUtile.ServizioUtileAA.HasValue)
                                txtServizioUtilePost94AA.Text = servizioUtile.ServizioUtileAA.Value.ToString();
                            if (servizioUtile.ServizioUtileMM.HasValue)
                                txtServizioUtilePost94MM.Text = servizioUtile.ServizioUtileMM.Value.ToString();
                            if (servizioUtile.ServizioUtileGG.HasValue)
                                txtServizioUtilePost94GG.Text = servizioUtile.ServizioUtileGG.Value.ToString();
                            break;
                    }
                }
            }
        }

        private void RecuperaCampiContributivoLegge214()
        {
            this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214 = !string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text) ? Convert.ToDecimal(txtImportoContribTotaleQuotaDL214.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = !string.IsNullOrEmpty(txtMontanteQuotaDL214.Text) ? Convert.ToDecimal(txtMontanteQuotaDL214.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = !string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text) ? Convert.ToInt32(txtNSettimaneQuotaDL214.Text) : (int?)null;
        }

        //private void RecuperaCampiContributivoLegge335()
        //{
        //    //this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale = !string.IsNullOrEmpty(txtImportoContributivoTotale.Text) ? Convert.ToDecimal(txtImportoContributivoTotale.Text) : (decimal?)null;
        //    //this.areaDatiContributivi.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontante.Text) ? Convert.ToDecimal(txtMontante.Text) : !string.IsNullOrEmpty(txtMontanteDa0697DatiMisto.Text) ? Convert.ToDecimal(txtMontanteDa0697DatiMisto.Text) : (decimal?)null;
        //    //this.areaDatiContributivi.DatiCalcolo.NSettimane = !string.IsNullOrEmpty(txtSettimane.Text) ? Convert.ToInt32(txtSettimane.Text) : (int?)null;
        //}

        private void RecuperaCampiContributivoLegge335()
        {
            this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale = !string.IsNullOrEmpty(txtImportTotale335_VL.Text) ? Convert.ToDecimal(txtImportTotale335_VL.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontanteDa0697_VL.Text) ? Convert.ToDecimal(txtMontanteDa0697_VL.Text) : !string.IsNullOrEmpty(txtMontante_VL.Text) ? Convert.ToDecimal(txtMontante_VL.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.MontanteAnte0697 = !string.IsNullOrEmpty(txtMontanteDa0196a0697_VL.Text) ? Convert.ToDecimal(txtMontanteDa0196a0697_VL.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697AA = !string.IsNullOrEmpty(txtA96_VL.Text) ? Convert.ToInt16(txtA96_VL.Text) : (short?)null;
            this.areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697MM = !string.IsNullOrEmpty(txtM96_VL.Text) ? Convert.ToInt16(txtM96_VL.Text) : (short?)null;
            this.areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697GG = !string.IsNullOrEmpty(txtG96_VL.Text) ? Convert.ToInt16(txtG96_VL.Text) : (short?)null;
            this.areaDatiContributivi.DatiCalcolo.AnzianitaPost0697AA = !string.IsNullOrEmpty(txtA97_VL.Text) ? Convert.ToInt16(txtA97_VL.Text) : (short?)null;
            this.areaDatiContributivi.DatiCalcolo.AnzianitaPost0697MM = !string.IsNullOrEmpty(txtM97_VL.Text) ? Convert.ToInt16(txtM97_VL.Text) : (short?)null;
            this.areaDatiContributivi.DatiCalcolo.AnzianitaPost0697GG = !string.IsNullOrEmpty(txtG97_VL.Text) ? Convert.ToInt16(txtG97_VL.Text) : (short?)null;
        }

        private void RecuperaCampiRetributivo(bool IsQuotaDVisible)
        {
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaA = !string.IsNullOrEmpty(txtRetribuzioneMediaSettADatiRetrib.Text) ? Convert.ToDecimal(txtRetribuzioneMediaSettADatiRetrib.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA = !string.IsNullOrEmpty(txtSettimaneA1DatiRetrib.Text) ? Convert.ToInt32(txtSettimaneA1DatiRetrib.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA2 = !string.IsNullOrEmpty(txtSettimaneA2DatiRetrib.Text) ? Convert.ToInt32(txtSettimaneA2DatiRetrib.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaB = !string.IsNullOrEmpty(txtRetribuzioneMediaSettBDatiRetrib.Text) ? Convert.ToDecimal(txtRetribuzioneMediaSettBDatiRetrib.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB = !string.IsNullOrEmpty(txtSettimaneBDatiRetrib.Text) ? Convert.ToInt32(txtSettimaneBDatiRetrib.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC = !string.IsNullOrEmpty(txtSettimaneC1DatiRetrib.Text) ? Convert.ToInt32(txtSettimaneC1DatiRetrib.Text) : (int?)null;
            if (IsQuotaDVisible)
            {
                this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC2 = !string.IsNullOrEmpty(txtSettimaneC2DatiRetrib.Text) ? Convert.ToInt32(txtSettimaneC2DatiRetrib.Text) : (int?)null;
                this.areaDatiContributivi.DatiCalcolo.RMSQuotaD = !string.IsNullOrEmpty(txtRetribuzioneMediaSettDDatiRetrib.Text) ? Convert.ToDecimal(txtRetribuzioneMediaSettDDatiRetrib.Text) : (decimal?)null;
                this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD = !string.IsNullOrEmpty(txtSettimaneDDatiRetrib.Text) ? Convert.ToInt32(txtSettimaneDDatiRetrib.Text) : (int?)null;
            }
        }

        private void RecuperaCampiRiduzioneRetributiva()
        {
            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
            {
                if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = true;
                else if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = false;
            }
            this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;

        }

        private void RecuperaCampiDatiCalcoloVL(ref bool isContributivo, ref bool isRetributivo)
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            this.areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto = !string.IsNullOrEmpty(txtSettimaneUtiliDiritto.Text) ? Convert.ToInt32(txtSettimaneUtiliDiritto.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.SettimaneUtiliDirittoOI = !string.IsNullOrEmpty(txtNumeroSettimaneOI.Text) ? Convert.ToInt32(txtNumeroSettimaneOI.Text) : (int?)null;

            switch (this.areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    RecuperaCampiContributivoLegge335();

                    if (this.areaDatiContributivi.IsContribL214Visible.HasValue && this.areaDatiContributivi.IsContribL214Visible.Value)
                        RecuperaCampiContributivoLegge214();

                    break;
                case GestioneContribTipoCalcolo.Retributivo:

                    RecuperaCampiRetributivo(true);
                    RecuperaCampiRiduzioneRetributiva();

                    break;
                case GestioneContribTipoCalcolo.Misto:

                    RecuperaCampiRetributivo(false);
                    RecuperaCampiContributivoLegge335();

                    if (this.areaDatiContributivi.IsContribL214Visible.HasValue && this.areaDatiContributivi.IsContribL214Visible.Value)
                        RecuperaCampiContributivoLegge214();

                    RecuperaCampiRiduzioneRetributiva();
                    break;
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    RecuperaCampiRetributivo(true);
                    RecuperaCampiContributivoLegge214();
                    RecuperaCampiRiduzioneRetributiva();
                    break;
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }

            //if (this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue || this.areaDatiContributivi.DatiCalcolo.Montante.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimane.HasValue)
            //    isContributivo = true;
            if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA2.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC2.HasValue || this.areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue)
                isRetributivo = true;
            if (this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.MontanteAnte0697.HasValue || this.areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697AA.HasValue || this.areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697MM.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.AnzianitaAnte0697GG.HasValue || this.areaDatiContributivi.DatiCalcolo.Montante.HasValue || this.areaDatiContributivi.DatiCalcolo.AnzianitaPost0697AA.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.AnzianitaPost0697MM.HasValue || this.areaDatiContributivi.DatiCalcolo.AnzianitaPost0697GG.HasValue || this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue)
                isContributivo = true;
        }

        private void RecuperaCampiDatiCalcoloFondoVL(ref bool isContributivo, ref bool isRetributivo)
        {
            if (areaDatiContributivi.DatiCalcolo.fondoVL == null)
                areaDatiContributivi.DatiCalcolo.fondoVL = new GestioneContribFondoVL();

            this.areaDatiContributivi.DatiCalcolo.fondoVL.LavoratorePrecoce = chkLavoratorePrecoce.Checked ? true : (bool?)null;

            #region Servizio Utile
            List<GestioneContribDatiServizioUtile> listaServizioUtile = null;
            // Quota A - Servizio utile ante 27/11/88
            if (!string.IsNullOrEmpty(txtRetrPensAnnuaQuotaA.Text) || !string.IsNullOrEmpty(txtServizioUtileAnte271188AA.Text) ||
                !string.IsNullOrEmpty(txtServizioUtileAnte271188MM.Text) || !string.IsNullOrEmpty(txtServizioUtileAnte271188GG.Text))
            {
                if (listaServizioUtile == null)
                    listaServizioUtile = new List<GestioneContribDatiServizioUtile>();

                GestioneContribDatiServizioUtile servizioUtile = new GestioneContribDatiServizioUtile();
                servizioUtile.Quota = "A";
                if (!string.IsNullOrEmpty(txtRetrPensAnnuaQuotaA.Text))
                    servizioUtile.RetribuzionePensionabile = CodeUtility.StringToNullableDecimal(txtRetrPensAnnuaQuotaA.Text);
                if (!string.IsNullOrEmpty(txtControcodiceRetributivoQuotaA.Text))
                    servizioUtile.ControCodiceRetributivo = CodeUtility.StringToNullableShort(txtControcodiceRetributivoQuotaA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileAnte271188AA.Text))
                    servizioUtile.ServizioUtileAA = CodeUtility.StringToNullableShort(txtServizioUtileAnte271188AA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileAnte271188MM.Text))
                    servizioUtile.ServizioUtileMM = CodeUtility.StringToNullableShort(txtServizioUtileAnte271188MM.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileAnte271188GG.Text))
                    servizioUtile.ServizioUtileGG = CodeUtility.StringToNullableShort(txtServizioUtileAnte271188GG.Text);

                listaServizioUtile.Add(servizioUtile);
            }

            // Quota A2 - Servizio utile ante '93
            if (!string.IsNullOrEmpty(txtServizioUtileAnte93AA.Text) || !string.IsNullOrEmpty(txtServizioUtileAnte93MM.Text) || !string.IsNullOrEmpty(txtServizioUtileAnte93GG.Text))
            {
                if (listaServizioUtile == null)
                    listaServizioUtile = new List<GestioneContribDatiServizioUtile>();

                GestioneContribDatiServizioUtile servizioUtile = new GestioneContribDatiServizioUtile();
                servizioUtile.Quota = "A2";
                if (!string.IsNullOrEmpty(txtServizioUtileAnte93AA.Text))
                    servizioUtile.ServizioUtileAA = CodeUtility.StringToNullableShort(txtServizioUtileAnte93AA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileAnte93MM.Text))
                    servizioUtile.ServizioUtileMM = CodeUtility.StringToNullableShort(txtServizioUtileAnte93MM.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileAnte93GG.Text))
                    servizioUtile.ServizioUtileGG = CodeUtility.StringToNullableShort(txtServizioUtileAnte93GG.Text);

                listaServizioUtile.Add(servizioUtile);
            }

            // Quota B - Servizio utile post '92
            if (!string.IsNullOrEmpty(txtRetrPensAnnuaQuotaB.Text) || !string.IsNullOrEmpty(txtServizioUtilePost311292AA.Text) ||
                !string.IsNullOrEmpty(txtServizioUtilePost311292MM.Text) || !string.IsNullOrEmpty(txtServizioUtilePost311292MM.Text))
            {
                if (listaServizioUtile == null)
                    listaServizioUtile = new List<GestioneContribDatiServizioUtile>();

                GestioneContribDatiServizioUtile servizioUtile = new GestioneContribDatiServizioUtile();
                servizioUtile.Quota = "B";
                if (!string.IsNullOrEmpty(txtRetrPensAnnuaQuotaB.Text))
                    servizioUtile.RetribuzionePensionabile = CodeUtility.StringToNullableDecimal(txtRetrPensAnnuaQuotaB.Text);
                if (!string.IsNullOrEmpty(txtControcodiceRetributivoQuotaB.Text))
                    servizioUtile.ControCodiceRetributivo = CodeUtility.StringToNullableShort(txtControcodiceRetributivoQuotaB.Text);
                if (!string.IsNullOrEmpty(txtServizioUtilePost311292AA.Text))
                    servizioUtile.ServizioUtileAA = CodeUtility.StringToNullableShort(txtServizioUtilePost311292AA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtilePost311292MM.Text))
                    servizioUtile.ServizioUtileMM = CodeUtility.StringToNullableShort(txtServizioUtilePost311292MM.Text);
                if (!string.IsNullOrEmpty(txtServizioUtilePost311292GG.Text))
                    servizioUtile.ServizioUtileGG = CodeUtility.StringToNullableShort(txtServizioUtilePost311292GG.Text);

                listaServizioUtile.Add(servizioUtile);
            }

            // Quota C - Servizio utile post '94
            if (!string.IsNullOrEmpty(txtServizioUtilePost94AA.Text) || !string.IsNullOrEmpty(txtServizioUtilePost94MM.Text) || !string.IsNullOrEmpty(txtServizioUtilePost94GG.Text))
            {
                if (listaServizioUtile == null)
                    listaServizioUtile = new List<GestioneContribDatiServizioUtile>();

                GestioneContribDatiServizioUtile servizioUtile = new GestioneContribDatiServizioUtile();
                servizioUtile.Quota = "C";
                if (!string.IsNullOrEmpty(txtServizioUtilePost94AA.Text))
                    servizioUtile.ServizioUtileAA = CodeUtility.StringToNullableShort(txtServizioUtilePost94AA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtilePost94MM.Text))
                    servizioUtile.ServizioUtileMM = CodeUtility.StringToNullableShort(txtServizioUtilePost94MM.Text);
                if (!string.IsNullOrEmpty(txtServizioUtilePost94GG.Text))
                    servizioUtile.ServizioUtileGG = CodeUtility.StringToNullableShort(txtServizioUtilePost94GG.Text);

                listaServizioUtile.Add(servizioUtile);
            }

            if (listaServizioUtile != null && listaServizioUtile.Count > 0)
                this.areaDatiContributivi.DatiCalcolo.fondoVL.LServizioUtile = listaServizioUtile.ToArray();
            #endregion Servizio Utile

            if (listaServizioUtile != null && listaServizioUtile.Count > 0)
                isRetributivo = true;
        }

        private void GestioneEtichetteIsUnicarpeVL(GestioneContribTipoCalcolo tipoCalcolo)
        {
            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    pnlDatiContributiviVLFelpe.Visible = true;
                    pnlDatiContributiviVLNoFelpe.Visible = false;
                    txtImportTotale335_VL.Enabled = false;
                    txtMontanteDa0196a0697_VL.Enabled = false;
                    txtA96_VL.Enabled = false;
                    txtM96_VL.Enabled = false;
                    txtG96_VL.Enabled = false;
                    txtMontanteDa0697_VL.Enabled = false;
                    txtA97_VL.Enabled = false;
                    txtM97_VL.Enabled = false;
                    txtG97_VL.Enabled = false;
                    txtMontante_VL.Enabled = false;
                    txtImportoContribTotaleQuotaDL214.Enabled = false;
                    txtMontanteQuotaDL214.Enabled = false;
                    txtNSettimaneQuotaDL214.Enabled = false;
                    break;
                case GestioneContribTipoCalcolo.Retributivo:
                    txtRetribuzioneMediaSettADatiRetrib.Enabled = false;
                    txtSettimaneA1DatiRetrib.Enabled = false;
                    txtSettimaneA2DatiRetrib.Enabled = false;
                    txtRetribuzioneMediaSettBDatiRetrib.Enabled = false;
                    txtSettimaneBDatiRetrib.Enabled = false;
                    txtSettimaneC1DatiRetrib.Enabled = false;
                    txtSettimaneC2DatiRetrib.Enabled = false;
                    txtRetribuzioneMediaSettDDatiRetrib.Enabled = false;
                    txtSettimaneDDatiRetrib.Enabled = false;
                    break;
                case GestioneContribTipoCalcolo.Misto:
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    txtRetribuzioneMediaSettADatiRetrib.Enabled = false;
                    txtSettimaneA1DatiRetrib.Enabled = false;
                    txtSettimaneA2DatiRetrib.Enabled = false;
                    txtRetribuzioneMediaSettBDatiRetrib.Enabled = false;
                    txtSettimaneBDatiRetrib.Enabled = false;
                    txtSettimaneC1DatiRetrib.Enabled = false;
                    txtSettimaneC2DatiRetrib.Enabled = false;
                    txtRetribuzioneMediaSettDDatiRetrib.Enabled = false;
                    txtSettimaneDDatiRetrib.Enabled = false;
                    txtImportTotale335_VL.Enabled = false;
                    txtMontanteDa0196a0697_VL.Enabled = false;
                    txtA96_VL.Enabled = false;
                    txtM96_VL.Enabled = false;
                    txtG96_VL.Enabled = false;
                    txtMontanteDa0697_VL.Enabled = false;
                    txtA97_VL.Enabled = false;
                    txtM97_VL.Enabled = false;
                    txtG97_VL.Enabled = false;
                    break;
            }
        }

        public void RecuperaCampiComma707(AreaDatiContributivi areaContributivi)
        {
            if (areaDatiContributivi == null)
                areaDatiContributivi = new AreaDatiContributivi();

            if (!string.IsNullOrEmpty(txtQuotaA1Comma707.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA707 = short.Parse(txtQuotaA1Comma707.Text);
            if (!string.IsNullOrEmpty(txtQuotaA2Comma707.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA2707 = short.Parse(txtQuotaA2Comma707.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707.Text))
                areaDatiContributivi.DatiCalcolo.QuotaB707 = short.Parse(txtQuotaBComma707.Text);
            if (!string.IsNullOrEmpty(txtQuotaC1Comma707.Text))
                areaDatiContributivi.DatiCalcolo.QuotaC707 = short.Parse(txtQuotaC1Comma707.Text);
            if (!string.IsNullOrEmpty(txtQuotaC2Comma707.Text))
                areaDatiContributivi.DatiCalcolo.QuotaC2707 = short.Parse(txtQuotaC2Comma707.Text);
            if (!string.IsNullOrEmpty(txtQuotaDComma707.Text))
                areaDatiContributivi.DatiCalcolo.QuotaD707 = short.Parse(txtQuotaDComma707.Text);
        }


        #endregion VL

        #region FS PT

        private void ValorizzaEtichetteDatiCalcoloFS_PT()
        {
            decimal? pensioneAnnulaLorda = null;
            short? servizioUtileDirittoAA = null;
            short? servizioUtileDirittoMM = null;
            short? servizioUtileDirittoGG = null;
            List<GestioneContribDatiServizioUtile> listaDatiServizioUtile = null;

            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                case GestioneContribTipoCalcolo.Misto:
                    if (areaDatiContributivi.IsContribL214Visible.GetValueOrDefault())
                    {
                        txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        txtQuotaPensioneContributivaAnnuaDL214.Text = areaDatiContributivi.DatiCalcolo.QuotaContributivaAnnua.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaContributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    txtImportoContributivoTotaleFS_PT.Text = areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    txtMontanteFS_PT.Text = areaDatiContributivi.DatiCalcolo.Montante.HasValue ? areaDatiContributivi.DatiCalcolo.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    txtImportoQuotaCFS_PT.Text = areaDatiContributivi.DatiCalcolo.MontanteContributivo.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    txtSettimaneFS_PT.Text = areaDatiContributivi.DatiCalcolo.NSettimane.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimane.Value.ToString() : string.Empty;
                    break;
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    txtQuotaPensioneContributivaAnnuaDL214.Text = areaDatiContributivi.DatiCalcolo.QuotaContributivaAnnua.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaContributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    break;
            }

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    listaDatiServizioUtile = areaDatiContributivi.DatiCalcolo.fondoFST != null && (areaDatiContributivi.DatiCalcolo.fondoFST.lDatiServizioUtile != null && areaDatiContributivi.DatiCalcolo.fondoFST.lDatiServizioUtile.Count() > 0) ? areaDatiContributivi.DatiCalcolo.fondoFST.lDatiServizioUtile.ToList() : null;
                    pensioneAnnulaLorda = areaDatiContributivi.DatiCalcolo.fondoFST != null ? areaDatiContributivi.DatiCalcolo.fondoFST.PensioneAnnuaLorda : null;
                    servizioUtileDirittoAA = areaDatiContributivi.DatiCalcolo.fondoFST != null ? areaDatiContributivi.DatiCalcolo.fondoFST.ServizioUtileDirittoAA : null;
                    servizioUtileDirittoMM = areaDatiContributivi.DatiCalcolo.fondoFST != null ? areaDatiContributivi.DatiCalcolo.fondoFST.ServizioUtileDirittoMM : null;
                    servizioUtileDirittoGG = areaDatiContributivi.DatiCalcolo.fondoFST != null ? areaDatiContributivi.DatiCalcolo.fondoFST.ServizioUtileDirittoGG : null;
                    txtRetribuzioneSenzaBenefici336.Text = (areaDatiContributivi.DatiCalcolo.fondoFST != null && areaDatiContributivi.DatiCalcolo.fondoFST.RMSSenzaLegge33670QA.HasValue) ? areaDatiContributivi.DatiCalcolo.fondoFST.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    listaDatiServizioUtile = areaDatiContributivi.DatiCalcolo.fondoPT != null && (areaDatiContributivi.DatiCalcolo.fondoPT.lDatiServizioUtile != null && areaDatiContributivi.DatiCalcolo.fondoPT.lDatiServizioUtile.Count() > 0) ? areaDatiContributivi.DatiCalcolo.fondoPT.lDatiServizioUtile.ToList() : null;
                    pensioneAnnulaLorda = areaDatiContributivi.DatiCalcolo.fondoPT != null ? areaDatiContributivi.DatiCalcolo.fondoPT.PensioneAnnuaLorda : null;
                    servizioUtileDirittoAA = areaDatiContributivi.DatiCalcolo.fondoPT != null ? areaDatiContributivi.DatiCalcolo.fondoPT.ServizioUtileDirittoAA : null;
                    servizioUtileDirittoMM = areaDatiContributivi.DatiCalcolo.fondoPT != null ? areaDatiContributivi.DatiCalcolo.fondoPT.ServizioUtileDirittoMM : null;
                    servizioUtileDirittoGG = areaDatiContributivi.DatiCalcolo.fondoPT != null ? areaDatiContributivi.DatiCalcolo.fondoPT.ServizioUtileDirittoGG : null;
                    break;
            }

            txtPensioneAnnuaLorda.Text = pensioneAnnulaLorda.HasValue ? pensioneAnnulaLorda.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtAnniServUtiliDirittoAA.Text = servizioUtileDirittoAA.HasValue ? servizioUtileDirittoAA.Value.ToString() : string.Empty;
            txtAnniServUtiliDirittoMM.Text = servizioUtileDirittoMM.HasValue ? servizioUtileDirittoMM.Value.ToString() : string.Empty;
            txtAnniServUtiliDirittoGG.Text = servizioUtileDirittoGG.HasValue ? servizioUtileDirittoGG.Value.ToString() : string.Empty;

            if ((areaDatiContributivi.DatiCalcolo.TipoCalcolo != GestioneContribTipoCalcolo.Contributivo) && (listaDatiServizioUtile != null && listaDatiServizioUtile.Count() > 0))
            {
                foreach (GestioneContribDatiServizioUtile servUtile in listaDatiServizioUtile)
                {
                    switch (servUtile.Quota)
                    {
                        case "A":
                            txtServizioUtileAAQtaA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                            txtServizioUtileMMQtaA.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                            txtServizioUtileGGQtaA.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                            txtRetribuzioneQtaA.Text = servUtile.Retribuzione.HasValue ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            txtQuotaArt14QtaA.Text = servUtile.QuoteArt14.HasValue ? servUtile.QuoteArt14.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            txtImpIndenIntegrSpecQtaA.Text = servUtile.ImportoIndennitaIntegrativaSpeciale.HasValue ? servUtile.ImportoIndennitaIntegrativaSpeciale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            txtQuotaRetributivaAnnua.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            break;
                        case "B1":
                            txtServizioUtileAAQtaB1.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                            txtServizioUtileMMQtaB1.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                            txtServizioUtileGGQtaB1.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                            txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            txtQuotaPensioneRetributivaAnnuaB94.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            break;
                        case "B2":
                            txtServizioUtileAAQtaB2.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                            txtServizioUtileMMQtaB2.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                            txtServizioUtileGGQtaB2.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                            txtQuotaPensioneRetributivaAnnuaB95.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            break;
                        case "B3":
                            txtServizioUtileAAQtaB3.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                            txtServizioUtileMMQtaB3.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                            txtServizioUtileGGQtaB3.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                            txtQuotaPensioneRetributivaAnnuaB97.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            break;
                        case "B4": // cessazione
                            txtServizioUtileCessazioneAA.Text = servUtile.ServizioUtileCessazioneAA.HasValue ? servUtile.ServizioUtileCessazioneAA.Value.ToString() : string.Empty;
                            txtServizioUtileCessazioneMM.Text = servUtile.ServizioUtileCessazioneMM.HasValue ? servUtile.ServizioUtileCessazioneMM.Value.ToString() : string.Empty;
                            txtServizioUtileCessazioneGG.Text = servUtile.ServizioUtileCessazioneGG.HasValue ? servUtile.ServizioUtileCessazioneGG.Value.ToString() : string.Empty;
                            txtQuotaPensioneRetributivaAnnuaCessazione.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                            break;
                    }
                }
            }
        }

        private void RecuperaCampiDatiCalcoloFS_PT(ref bool isContributivo, ref bool isRetributivo)
        {
            decimal? pensioneAnnulaLorda = null;
            short? servizioUtileDirittoAA = null;
            short? servizioUtileDirittoMM = null;
            short? servizioUtileDirittoGG = null;
            decimal? rmsSenzaLegge33670QA = null;

            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            this.areaDatiContributivi.IsContribL214Visible = (bool?)ViewState["ContribL214"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale = !string.IsNullOrEmpty(txtImportoContributivoTotaleFS_PT.Text) ? Convert.ToDecimal(txtImportoContributivoTotaleFS_PT.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontanteFS_PT.Text) ? Convert.ToDecimal(txtMontanteFS_PT.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.MontanteContributivo = !string.IsNullOrEmpty(txtImportoQuotaCFS_PT.Text) ? Convert.ToDecimal(txtImportoQuotaCFS_PT.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.NSettimane = !string.IsNullOrEmpty(txtSettimaneFS_PT.Text) ? Convert.ToInt32(txtSettimaneFS_PT.Text) : (int?)null;

            if (this.areaDatiContributivi.IsContribL214Visible.HasValue && this.areaDatiContributivi.IsContribL214Visible.Value)
            {
                areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214 = !string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text) ? CodeUtility.StringToNullableDecimal(txtImportoContribTotaleQuotaDL214.Text) : null;
                areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = !string.IsNullOrEmpty(txtMontanteQuotaDL214.Text) ? CodeUtility.StringToNullableDecimal(txtMontanteQuotaDL214.Text) : null;
                areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = !string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text) ? CodeUtility.StringToNullableInt(txtNSettimaneQuotaDL214.Text) : null;
                areaDatiContributivi.DatiCalcolo.QuotaContributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneContributivaAnnuaDL214.Text) ? CodeUtility.StringToNullableDecimal(txtQuotaPensioneContributivaAnnuaDL214.Text) : null;
            }

            // Per le domande di RIC o Riapertura, il campo non è obbligatorio e può essere lasciato vuoto anche nel caso di tipo calcolo contributivo, per questo motivo viene considerata solo la visibilità
            if (this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue || this.areaDatiContributivi.DatiCalcolo.Montante.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.NSettimane.HasValue || this.areaDatiContributivi.DatiCalcolo.MontanteContributivo.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.QuotaContributivaAnnua.HasValue ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && txtMontanteFS_PT.Visible))
                isContributivo = true;

            pensioneAnnulaLorda = !string.IsNullOrEmpty(txtPensioneAnnuaLorda.Text) ? Convert.ToDecimal(txtPensioneAnnuaLorda.Text) : (decimal?)null;
            servizioUtileDirittoAA = !string.IsNullOrEmpty(txtAnniServUtiliDirittoAA.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoAA.Text) : (short?)null;
            servizioUtileDirittoMM = !string.IsNullOrEmpty(txtAnniServUtiliDirittoMM.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoMM.Text) : (short?)null;
            servizioUtileDirittoGG = !string.IsNullOrEmpty(txtAnniServUtiliDirittoGG.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoGG.Text) : (short?)null;
            rmsSenzaLegge33670QA = !string.IsNullOrEmpty(txtRetribuzioneSenzaBenefici336.Text) ? Convert.ToDecimal(txtRetribuzioneSenzaBenefici336.Text) : (decimal?)null;

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoFST == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoFST = new GestioneContribFondoFST();
                    areaDatiContributivi.DatiCalcolo.fondoFST.PensioneAnnuaLorda = pensioneAnnulaLorda;
                    areaDatiContributivi.DatiCalcolo.fondoFST.ServizioUtileDirittoAA = servizioUtileDirittoAA;
                    areaDatiContributivi.DatiCalcolo.fondoFST.ServizioUtileDirittoMM = servizioUtileDirittoMM;
                    areaDatiContributivi.DatiCalcolo.fondoFST.ServizioUtileDirittoGG = servizioUtileDirittoGG;
                    areaDatiContributivi.DatiCalcolo.fondoFST.lDatiServizioUtile = null;
                    areaDatiContributivi.DatiCalcolo.fondoFST.RMSSenzaLegge33670QA = rmsSenzaLegge33670QA;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoPT == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoPT = new GestioneContribFondoPT();
                    areaDatiContributivi.DatiCalcolo.fondoPT.PensioneAnnuaLorda = pensioneAnnulaLorda;
                    areaDatiContributivi.DatiCalcolo.fondoPT.ServizioUtileDirittoAA = servizioUtileDirittoAA;
                    areaDatiContributivi.DatiCalcolo.fondoPT.ServizioUtileDirittoMM = servizioUtileDirittoMM;
                    areaDatiContributivi.DatiCalcolo.fondoPT.ServizioUtileDirittoGG = servizioUtileDirittoGG;
                    areaDatiContributivi.DatiCalcolo.fondoPT.lDatiServizioUtile = null;
                    break;
            }

            List<GestioneContribDatiServizioUtile> lDatiServUtile = new List<GestioneContribDatiServizioUtile>();
            GestioneContribDatiServizioUtile datiServUtile = null;

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ||
               !String.IsNullOrEmpty(txtRetribuzioneQtaA.Text) || !String.IsNullOrEmpty(txtQuotaArt14QtaA.Text) || !String.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "A";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) ? Convert.ToInt16(txtServizioUtileAAQtaA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) ? Convert.ToInt16(txtServizioUtileMMQtaA.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ? Convert.ToInt16(txtServizioUtileGGQtaA.Text) : (short?)null;
                datiServUtile.QuoteArt14 = !string.IsNullOrEmpty(txtQuotaArt14QtaA.Text) ? Convert.ToDecimal(txtQuotaArt14QtaA.Text) : (decimal?)null;
                datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRetribuzioneQtaA.Text) ? Convert.ToDecimal(txtRetribuzioneQtaA.Text) : (decimal?)null;
                datiServUtile.ImportoIndennitaIntegrativaSpeciale = !string.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text) ? Convert.ToDecimal(txtImpIndenIntegrSpecQtaA.Text) : (decimal?)null;
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaRetributivaAnnua.Text) ? Convert.ToDecimal(txtQuotaRetributivaAnnua.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ||
                !String.IsNullOrEmpty(txtRMSQtaB1.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "B1";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB1.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB1.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB1.Text) : (short?)null;
                datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRMSQtaB1.Text) ? Convert.ToDecimal(txtRMSQtaB1.Text) : (decimal?)null;
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB94.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB94.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "B2";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB2.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB2.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB2.Text) : (short?)null;
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB95.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB95.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "B3";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB3.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB3.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB3.Text) : (short?)null;
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB97.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB97.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "B4";
                datiServUtile.ServizioUtileCessazioneAA = !string.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) ? Convert.ToInt16(txtServizioUtileCessazioneAA.Text) : (short?)null;
                datiServUtile.ServizioUtileCessazioneMM = !string.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) ? Convert.ToInt16(txtServizioUtileCessazioneMM.Text) : (short?)null;
                datiServUtile.ServizioUtileCessazioneGG = !string.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text) ? Convert.ToInt16(txtServizioUtileCessazioneGG.Text) : (short?)null;
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaCessazione.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaCessazione.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
            {
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        areaDatiContributivi.DatiCalcolo.fondoFST.lDatiServizioUtile = lDatiServUtile.ToArray();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        areaDatiContributivi.DatiCalcolo.fondoPT.lDatiServizioUtile = lDatiServUtile.ToArray();
                        break;

                }
                isRetributivo = true;
            }
        }

        private void GestioneEtichetteIsUnicarpeFS_PT(GestioneContribTipoCalcolo tipoCalcolo)
        { }

        #endregion FS PT

        #region Event

        public event EventHandler CaricaDatiCalcolo;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event EventHandler ShowPopUp;
        public event EventHandler HidePopUp;

        protected void RaiseShowPopUp(object sender, EventArgs args)
        {
            ShowPopUp(sender, args);
        }

        protected void RaiseHidePopUp(object sender, EventArgs args)
        {
            HidePopUp(sender, args);
        }

        protected void RaiseCaricaDatiCalcolo(object sender, EventArgs e)
        {
            CaricaDatiCalcolo(sender, e);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion Event

        protected void nSettimaneOI_TextChanged(object sender, EventArgs e)
        {
            SetSettimaneTotali();
        }

        protected void  nSettimaneUtiliDiritto_TextChanged(object sender, EventArgs e)
        {
            SetSettimaneTotali();
        }

        private void SetSettimaneTotali()
        {
            //ENG - Memo 79
            if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
            {
                int settimaneOI = string.IsNullOrEmpty(txtNumeroSettimaneOI.Text) ? 0 : int.Parse(txtNumeroSettimaneOI.Text);
                int settimaneOBG = string.IsNullOrEmpty(txtSettimaneUtiliDiritto.Text) ? 0 : int.Parse(txtSettimaneUtiliDiritto.Text);
                txtNumeroSettimaneTot.Text = (settimaneOI + settimaneOBG).ToString();
            }
        }
    }
}
