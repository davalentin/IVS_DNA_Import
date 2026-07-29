using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiCalcoloEL_TT_ET : CustomBaseUserControl, IDatiContributivi, ITitolarePensione
    {
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
            ViewState[EnumViewstate.IsAnteArmonizzazione.ToString()] = this.areaDatiContributivi.IsAnteArmonizzazione.HasValue ? this.areaDatiContributivi.IsAnteArmonizzazione.Value : (bool?)null;

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            RenderControlsFromTipoCalcolo_TipoFondo(datiPensione);
            ValorizzaEtichetteCommon();

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    ValorizzaEtichetteEL_TT();
                    GestioneEtichetteIsUnicarpeEL_TT(areaDatiContributivi.DatiCalcolo.TipoCalcolo, datiPensione);
                    ValorizzaEtichetteAnteArmonizzazione(areaDatiContributivi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    ValorizzaEtichetteET();
                    GestioneEtichetteIsUnicarpeET(areaDatiContributivi.DatiCalcolo.TipoCalcolo, datiPensione);
                    ValorizzaEtichetteAnteArmonizzazione(areaDatiContributivi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    ValorizzaEtichetteEL_TT();
                    ValorizzaEtichetteTT();
                    GestioneEtichetteIsUnicarpeEL_TT(areaDatiContributivi.DatiCalcolo.TipoCalcolo, datiPensione);
                    GestioneEtichetteIsUnicarpeTT(areaDatiContributivi.DatiCalcolo.TipoCalcolo, datiPensione);
                    ValorizzaEtichetteAnteArmonizzazione(areaDatiContributivi);
                    break;
            }
            GestioneEtichetteIsUnicarpeCommon(datiPensione);

            ManageButtons();
            GestioneEtichetteRic(datiPensione);

            //ENG - PL CONTRIBUZIONE POST 2011
            if (areaDatiContributivi.IsContribuzioneL335NonObbligatoria.HasValue && areaDatiContributivi.IsContribuzioneL335NonObbligatoria.Value)
            {
                RequiredFieldValidator3.Enabled = false;
                RequiredFieldValidator4.Enabled = false;
                RequiredFieldValidator5.Enabled = false;
            }
        }

        internal void RecuperaCampi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? fondo)
        {
            bool isContributivo = false;
            bool isRetributivo = false;

            RecuperaCampiCommon(ref isContributivo, ref isRetributivo);

            switch (fondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    RecuperaCampiEL_TT(ref isContributivo, ref isRetributivo);
                    RecuperaDatiAnteArmonizzazione(ref isRetributivo);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    RecuperaCampiET(ref isContributivo, ref isRetributivo);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    RecuperaCampiEL_TT(ref isContributivo, ref isRetributivo);
                    RecuperaCampiTT(ref isContributivo, ref isRetributivo);
                    RecuperaDatiAnteArmonizzazione(ref isRetributivo);
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

        internal void EnableDisableBtnSalva(bool enable)
        {
            this.btnSalvaDatiCalcolo.Enabled = enable;
            this.btnPopUp.Enabled = enable;
            this.btnSalvaDatiCalcoloNoRiduzione.Enabled = enable;
            this.btnEliminaDatiCalcolo.Enabled = enable;
        }

        internal bool ManageButtonRiduzioneRetributiva()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            AreaTitolare.DatiPensione DatiPensione = this.GetDatiPensione(this);

            if (titolare != null && DatiPensione != null)
            {
                if (titolare.DataNascita.HasValue && DatiPensione.DecorrenzaOriginaria.HasValue)
                {
                    if (!(DateTime.Compare(titolare.DataNascita.Value.AddYears(62), DatiPensione.DecorrenzaOriginaria.Value) < 0) &&
                        (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Common

        private void ValorizzaEtichetteCommon()
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            FlagUnicarpe.Value = Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ? "SI" : "NO";  // Lettura_L è il termine di paragone assumendo che per Lettura_C i datiCalcolo da AggPeco sono non valorizzati

            txtSettimaneUtiliDiritto.Text = areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto.HasValue ? areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto.Value.ToString() : string.Empty;
            //Memo 79
            txtSettimaneUtiliDirittoOI.Text = areaDatiContributivi.DatiCalcolo.SettimaneUtiliDirittoOI.HasValue ? areaDatiContributivi.DatiCalcolo.SettimaneUtiliDirittoOI.Value.ToString() : string.Empty;
            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
                SetSettimaneTotali();

            txtImportoContributivoTotale.Text = areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontante.Text = areaDatiContributivi.DatiCalcolo.Montante.HasValue ? areaDatiContributivi.DatiCalcolo.Montante.Value.ToString("0.0000") : string.Empty;
            txtSettimane.Text = areaDatiContributivi.DatiCalcolo.NSettimane.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimane.Value.ToString() : string.Empty;

            txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString() : string.Empty;

            txtRMSA.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaA.Value.ToString("0.0000") : string.Empty;
            txtRMSB.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaB.Value.ToString("0.0000") : string.Empty;
            txtSettimaneA.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.Value.ToString() : string.Empty;
            txtSettimaneB.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.Value.ToString() : string.Empty;
            txtRetribuzioneAgoAnnua.Text = areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAnnua.HasValue ? areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            if (areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva)
                ddlRiduzioneRetributiva.SelectedValue = "SI";
            else ddlRiduzioneRetributiva.SelectedValue = "NO";
            txtRiduzioneRetributiva.Text = areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.HasValue ? areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

            if (areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.RetributivoMonti)
                ViewState["RetributivoMonti"] = "SI";

            //ENG - Memo 79
            bool blnIsOrganizzazioniInternazionali = Utility.IsDomandaOrganizzazioniInternazionali(datiPensione);
            pnlNSettimane_OrganizzazioniInternazionali.Visible = blnIsOrganizzazioniInternazionali;
            if (blnIsOrganizzazioniInternazionali)
            {
                lblNumeroSettimane.InnerText = "Settimane Utili al Diritto Italiane";
            }
        }

        private void RecuperaCampiCommon(ref bool isContributivo, ref bool isRetributivo)
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = (GestioneContribTipoCalcolo)ViewState["TipoCalcolo"];
            this.areaDatiContributivi.IsContribL214Visible = (bool?)ViewState["ContribL214"];
            this.areaDatiContributivi.IsRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];
            this.areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto = !string.IsNullOrEmpty(txtSettimaneUtiliDiritto.Text) ? Convert.ToInt32(txtSettimaneUtiliDiritto.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.SettimaneUtiliDirittoOI = !string.IsNullOrEmpty(txtSettimaneUtiliDirittoOI.Text) ? Convert.ToInt32(txtSettimaneUtiliDirittoOI.Text) : (int?)null;

            switch (this.areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    RecuperaCampiContributivoLegge335();
                    if (this.areaDatiContributivi.IsContribL214Visible.HasValue && this.areaDatiContributivi.IsContribL214Visible.Value)
                        RecuperaCampiContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.Retributivo:
                    RecuperaCampiRetributivo();
                    break;
                case GestioneContribTipoCalcolo.Misto:
                    RecuperaCampiRetributivo();
                    RecuperaCampiContributivoLegge335();
                    if (this.areaDatiContributivi.IsContribL214Visible.HasValue && this.areaDatiContributivi.IsContribL214Visible.Value)
                        RecuperaCampiContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    RecuperaCampiRetributivo();
                    RecuperaCampiContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }

            if (this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue || this.areaDatiContributivi.DatiCalcolo.Montante.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimane.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                isContributivo = true;
            if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAnnua.HasValue)
                isRetributivo = true;
        }

        private void RecuperaCampiContributivoLegge335()
        {
            this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale = !string.IsNullOrEmpty(txtImportoContributivoTotale.Text) ? Convert.ToDecimal(txtImportoContributivoTotale.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontante.Text) ? Convert.ToDecimal(txtMontante.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimane = !string.IsNullOrEmpty(txtSettimane.Text) ? Convert.ToInt32(txtSettimane.Text) : (int?)null;
        }

        private void RecuperaCampiContributivoLegge214()
        {
            this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214 = !string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text) ? Convert.ToDecimal(txtImportoContribTotaleQuotaDL214.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = !string.IsNullOrEmpty(txtMontanteQuotaDL214.Text) ? Convert.ToDecimal(txtMontanteQuotaDL214.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = !string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text) ? Convert.ToInt32(txtNSettimaneQuotaDL214.Text) : (int?)null;
        }

        private void RecuperaCampiRetributivo()
        {
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaA = !string.IsNullOrEmpty(txtRMSA.Text) ? Convert.ToDecimal(txtRMSA.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaB = !string.IsNullOrEmpty(txtRMSB.Text) ? Convert.ToDecimal(txtRMSB.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA = !string.IsNullOrEmpty(txtSettimaneA.Text) ? Convert.ToInt32(txtSettimaneA.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB = !string.IsNullOrEmpty(txtSettimaneB.Text) ? Convert.ToInt32(txtSettimaneB.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAnnua = !string.IsNullOrEmpty(txtRetribuzioneAgoAnnua.Text) ? Convert.ToDecimal(txtRetribuzioneAgoAnnua.Text) : (decimal?)null;

            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
            {
                if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = true;
                else if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = false;
            }
            this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;
        }

        private void RenderControlsFromTipoCalcolo_TipoFondo(AreaTitolare.DatiPensione datiPensione)
        {
            bool IsRiduzionePresent = false;
            bool? isAnteArmonizzazione = (bool?)ViewState[EnumViewstate.IsAnteArmonizzazione.ToString()];
            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    pnlDatiCalcoloContributivi_EL_TT_ET.Visible = true;
                    pnlDatiCalcoloContributiviLegge335_EL_TT_ET.Visible = true;
                    ManagePnlContributivoLegge214();
                    break;
                case GestioneContribTipoCalcolo.Retributivo:
                    pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                    ManageRiduzioneRetributiva(IsRiduzionePresent);
                    ManagePnlContributivoLegge214();
                    RenderControlsFromTipoFondo();
                    RenderControlForAnteArm(areaDatiContributivi);
                    break;
                case GestioneContribTipoCalcolo.Misto:
                    pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                    rigaD.Visible = false;
                    ManageRiduzioneRetributiva(IsRiduzionePresent);

                    pnlDatiCalcoloContributivi_EL_TT_ET.Visible = true;
                    pnlDatiCalcoloContributiviLegge335_EL_TT_ET.Visible = true;

                    ManagePnlContributivoLegge214();
                    RenderControlsFromTipoFondo();

                    if (Utility.IsDomandaReversibilita(datiPensione) && isAnteArmonizzazione.GetValueOrDefault() && this.domanda.Tipofondo.HasValue &&
                       (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT))
                        RenderControlForAnteArm(areaDatiContributivi);
                    break;
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                    rigaD.Visible = true;

                    ManageRiduzioneRetributiva(IsRiduzionePresent);

                    pnlDatiCalcoloContributivi_EL_TT_ET.Visible = true;
                    ManagePnlContributivoLegge214();

                    RenderControlsFromTipoFondo();
                    break;
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }

            // Render Controls ex comma 707
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    if (areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
                    {
                        pnlComma707.Visible = true;
                        tblComma707EL_TT.Visible = true;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    if (areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
                    {
                        pnlComma707.Visible = true;
                        tblComma707ET.Visible = true;
                    }
                    break;
            }

            // TODO: Rimuovere nel momento in cui verranno portati in produzione i fondi con il comma 707
            // Disabilitazione Required Field Validator per comma 707
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    RFVtxtRetribuzionePonderataComma707.Enabled = false;
                    break;
            }

            pnlSettimane_EL_TT_ET.Visible = true;
        }

        private void RenderControlForAnteArm(AreaDatiContributivi areaDatiContributivi)
        {
            bool? isAnteArmonizzazione = (bool?)ViewState[EnumViewstate.IsAnteArmonizzazione.ToString()];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    if (isAnteArmonizzazione.GetValueOrDefault())
                    {
                        pnlELAnteArmonizzazione.Visible = true;
                        pnlAnteArmonizzazioneCommon.Visible = true;
                        pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = false;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    if (isAnteArmonizzazione.GetValueOrDefault())
                    {
                        pnlTTAnteArmonizzazione.Visible = true;
                        pnlAnteArmonizzazioneCommon.Visible = true;
                        pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = false;
                        if (Utility.IsDomandaReversibilita(datiPensione))
                        {
                            RFVtxtTTAnteArmRetrUAnno.Enabled = false;
                            RFVtxtTTAnteArmRetrBiennio.Enabled = false;
                            RFVtxtTTAnteArmControCodiceRetrQtA.Enabled = false;
                            txtImportoContribTotaleQuotaDL214RF.Enabled = false;
                            RFV_txtMontanteQuotaDL214.Enabled = false;
                            RFV_txtNSettimaneQuotaDL214.Enabled = false;
                        }
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    if (areaDatiContributivi.IsAnteArmonizzazione.GetValueOrDefault())
                    {
                        pnlELAnteArmonizzazione.Visible = false;
                        pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                        pnlDecretoCross.Visible = false;
                    }
                    break;
            }
        }

        private void ManageRiduzioneRetributiva(bool IsRiduzionePresent)
        {
            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
                pnlRiduzioneRetributiva.Visible = true;
            else
                pnlRiduzioneRetributiva.Visible = false;

            IsRiduzionePresent = ManageButtonRiduzioneRetributiva();
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
            pnlDatiCalcoloContributivi_EL_TT_ET.Visible = !pnlDatiCalcoloContributivi_EL_TT_ET.Visible ? (this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false) : true;
            pnlDatiCalcoloContributiviLegge214_EL_TT_ET.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
        }

        private void SetData(){}

        private void SetSettimaneTotali()
        {
            //ENG - Memo 79
            if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
            {
                int ServUtiliDirittoTot = (string.IsNullOrEmpty(txtSettimaneUtiliDiritto.Text) ? 0 : int.Parse(txtSettimaneUtiliDiritto.Text)) + (string.IsNullOrEmpty(txtSettimaneUtiliDirittoOI.Text) ? 0 : int.Parse(txtSettimaneUtiliDirittoOI.Text));
                txtSettimaneUtiliDirittoTot.Text = ServUtiliDirittoTot.ToString();
            }
        }

        private void RenderControlsFromTipoFondo()
        {
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    pnlDatiCalcoloRetributiviET.Visible = true;
                    RenderControlsQuotaA();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                    pnlDatiCalcoloRetributiviTT.Visible = true;
                    break;
            }

           
                
        }

        private void RenderControlsQuotaA()
        {
            if ((this.areaDatiContributivi.IsAnzianita.HasValue && this.areaDatiContributivi.IsAnzianita.Value) ||
                (this.areaDatiContributivi.IsInvaliditaSpecifica.HasValue && this.areaDatiContributivi.IsInvaliditaSpecifica.Value) ||
                (this.areaDatiContributivi.IsVecchiaiaSpecifica.HasValue && this.areaDatiContributivi.IsVecchiaiaSpecifica.Value))
            {
                pnlRigaA.Visible = false;
                tdRigaA707ET_lbl.Visible = false;
                tdRigaA707ET_txt.Visible = false;
            }
        }

        private void GestioneEtichetteIsUnicarpeCommon(AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (areaDatiContributivi.DatiCalcolo.SettimaneUtiliDiritto.HasValue)
                    txtSettimaneUtiliDiritto.Enabled = false;

                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        txtImportoContributivoTotale.Enabled = false;
                        txtMontante.Enabled = false;
                        txtSettimane.Enabled = false;
                        txtImportoContribTotaleQuotaDL214.Enabled = false;
                        txtMontanteQuotaDL214.Enabled = false;
                        txtNSettimaneQuotaDL214.Enabled = false;
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        txtRetribuzioneAgoAnnua.Enabled = false;
                        txtRMSA.Enabled = false;
                        txtRMSB.Enabled = false;
                        txtSettimaneA.Enabled = false;
                        txtSettimaneB.Enabled = false;
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
                        txtImportoContributivoTotale.Enabled = false;
                        txtMontante.Enabled = false;
                        txtSettimane.Enabled = false;
                        txtRetribuzioneAgoAnnua.Enabled = false;
                        txtRMSA.Enabled = false;
                        txtRMSB.Enabled = false;
                        txtSettimaneA.Enabled = false;
                        txtSettimaneB.Enabled = false;
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
                    case GestioneContribTipoCalcolo.NonValido:
                        break;
                }
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

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                //COMMON
                txtSettimaneUtiliDiritto.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
                txtImportoContributivoTotale.Enabled = false;
                RequiredFieldValidator3.Enabled = false;
                txtMontante.Enabled = false;
                RequiredFieldValidator4.Enabled = false;
                txtSettimane.Enabled = false;
                RequiredFieldValidator5.Enabled = false;
                txtImportoContribTotaleQuotaDL214.Enabled = false;
                txtImportoContribTotaleQuotaDL214RF.Enabled = false;
                txtMontanteQuotaDL214.Enabled = false;
                RFV_txtMontanteQuotaDL214.Enabled = false;
                txtNSettimaneQuotaDL214.Enabled = false;
                RFV_txtNSettimaneQuotaDL214.Enabled = false;
                txtRetribuzionePonderataComma707.Enabled = false;
                RFVtxtRetribuzionePonderataComma707.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT)
                {
                    txtSettimaneC.Enabled = false;
                    txtRMSD.Enabled = false;
                    txtSettimaneD.Enabled = false;
                    txtAnteArm_RetrPondAGO.Enabled = false;
                    txtQuotaAComma707EL_TT.Enabled = false;
                    txtQuotaBComma707EL_TT.Enabled = false;
                    txtQuotaCComma707EL_TT.Enabled = false;
                    txtQuotaDComma707EL_TT.Enabled = false;
                }

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET)
                {
                    txtELAnteArmQtaA_AA.Enabled = false;
                    txtELAnteArmQtaA_MM.Enabled = false;
                    txtELAnteArmQtaA_RetrPens.Enabled = false;
                    txtELAnteArmQtaA_CC.Enabled = false;
                    txtELAnteArmQtaB_AA.Enabled = false;
                    txtELAnteArmQtaB_MM.Enabled = false;
                    txtELAnteArmQtaB_RetrPens.Enabled = false;
                    txtELAnteArmQtaB_CC.Enabled = false;
                    txtELAnteArmQtaC_AA.Enabled = false;
                    txtELAnteArmQtaC_MM.Enabled = false;
                }

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET)
                {
                    txtServizioUtileAAQtaA.Enabled = false;
                    txtServizioUtileMMQtaA.Enabled = false;
                    txtServizioUtileGGQtaA.Enabled = false;
                    txtRetribPensionabileQtaA.Enabled = false;
                    txtControCodiceRetrQtaA.Enabled = false;
                    txtServizioUtileAAQtaB.Enabled = false;
                    txtServizioUtileMMQtaB.Enabled = false;
                    txtServizioUtileGGQtaB.Enabled = false;
                    txtRetribPensionabileQtaB.Enabled = false;
                    txtControCodiceRetrQtaB.Enabled = false;
                    txtServizioUtileAAQtaC.Enabled = false;
                    txtServizioUtileMMQtaC.Enabled = false;
                    txtServizioUtileGGQtaC.Enabled = false;
                    txtRMSA.Enabled = false;
                    txtSettimaneA.Enabled = false;
                    txtRMSB.Enabled = false;
                    txtSettimaneB.Enabled = false;
                    txtRetribuzioneAgoAnnua.Enabled = false;
                    txtQuotaAComma707ETAA.Enabled = false;
                    txtQuotaAComma707ETMM.Enabled = false;
                    txtQuotaAComma707ETGG.Enabled = false;
                    txtQuotaAComma707ET.Enabled = false;
                    txtQuotaBComma707ETAA.Enabled = false;
                    txtQuotaBComma707ETMM.Enabled = false;
                    txtQuotaBComma707ETGG.Enabled = false;
                    txtQuotaBComma707ET.Enabled = false;
                    txtQuotaCComma707ETAA.Enabled = false;
                    txtQuotaCComma707ETMM.Enabled = false;
                    txtQuotaCComma707ETGG.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT)
                {
                    txtRetribUltimoAnnoRetrib.Enabled = false;
                    txtRetribuzioneBiennio.Enabled = false;
                    txtTTAnteArmQtaA_AA.Enabled = false;
                    txtTTAnteArmQtaA_MM.Enabled = false;
                    txtTTAnteArmQtaARid_AA.Enabled = false;
                    txtTTAnteArmQtaARid_MM.Enabled = false;
                    txtTTAnteArmPensioneAl53.Enabled = false;
                    txtTTAnteArmRetrUAnno.Enabled = false;
                    RFVtxtTTAnteArmRetrUAnno.Enabled = false;
                    txtTTAnteArmRetrBiennio.Enabled = false;
                    RFVtxtTTAnteArmRetrBiennio.Enabled = false;
                    txtTTAnteArmElAccess.Enabled = false;
                    txtTTAnteArmRetrSup.Enabled = false;
                    txtTTAnteArmControCodiceRetrQtA.Enabled = false;
                    RFVtxtTTAnteArmControCodiceRetrQtA.Enabled = false;
                    txtTTAnteArmQtB_AA.Enabled = false;
                    txtTTAnteArmQtB_MM.Enabled = false;
                    txtTTAnteArmQtBRid_AA.Enabled = false;
                    txtTTAnteArmQtBRid_MM.Enabled = false;
                    txtTTAnteArmRetrPensionabileQtB.Enabled = false;
                    txtTTAnteArmControCodiceRetrQtB.Enabled = false;
                    txtTTAnteArmQtC_AA.Enabled = false;
                    txtTTAnteArmQtC_MM.Enabled = false;
                    txtTTAnteArmQtCRid_AA.Enabled = false;
                    txtTTAnteArmQtCRid_MM.Enabled = false;
                    txtTTAnteArmQtD_AA.Enabled = false;
                    txtTTAnteArmQtD_MM.Enabled = false;
                    txtTTAnteArmQtDRid_AA.Enabled = false;
                    txtTTAnteArmQtDRid_MM.Enabled = false;
                    txtTTAnteArmRetrPensionabileQtD.Enabled = false;
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

        #endregion Common

        #region Common EL_TT

        private void ValorizzaEtichetteEL_TT()
        {
            lblTitoloDatiRetributivi.Text = "Decreto Legislativo 562";
            txtRMSD.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimaneC.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC.Value.ToString() : string.Empty;
            txtSettimaneD.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.Value.ToString() : string.Empty;
            txtRMSD.Text = areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue ? areaDatiContributivi.DatiCalcolo.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimaneD.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.Value.ToString() : string.Empty;

            txtQuotaAComma707EL_TT.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707.Value.ToString() : string.Empty;
            txtQuotaBComma707EL_TT.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707.Value.ToString() : string.Empty;
            txtQuotaCComma707EL_TT.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaC707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaC707.Value.ToString() : string.Empty;
            txtQuotaDComma707EL_TT.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaD707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaD707.Value.ToString() : string.Empty;

            txtRetribuzionePonderataComma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707.HasValue ? areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void RecuperaCampiEL_TT(ref bool isContributivo, ref bool isRetributivo)
        {
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaD = !string.IsNullOrEmpty(txtRMSD.Text) ? Convert.ToDecimal(txtRMSD.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC = !string.IsNullOrEmpty(txtSettimaneC.Text) ? Convert.ToInt32(txtSettimaneC.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD = !string.IsNullOrEmpty(txtSettimaneD.Text) ? Convert.ToInt32(txtSettimaneD.Text) : (int?)null;

            if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaC.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue)
                isRetributivo = true;

            if (!string.IsNullOrEmpty(txtQuotaAComma707EL_TT.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA707 = short.Parse(txtQuotaAComma707EL_TT.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707EL_TT.Text))
                areaDatiContributivi.DatiCalcolo.QuotaB707 = short.Parse(txtQuotaBComma707EL_TT.Text);
            if (!string.IsNullOrEmpty(txtQuotaCComma707EL_TT.Text))
                areaDatiContributivi.DatiCalcolo.QuotaC707 = short.Parse(txtQuotaCComma707EL_TT.Text);
            if (!string.IsNullOrEmpty(txtQuotaDComma707EL_TT.Text))
                areaDatiContributivi.DatiCalcolo.QuotaD707 = short.Parse(txtQuotaDComma707EL_TT.Text);

            if (!string.IsNullOrEmpty(txtRetribuzionePonderataComma707.Text))
                areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707 = decimal.Parse(txtRetribuzionePonderataComma707.Text);
        }

        private void GestioneEtichetteIsUnicarpeEL_TT(GestioneContribTipoCalcolo tipoCalcolo, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        txtRMSD.Enabled = false;
                        txtSettimaneC.Enabled = false;
                        txtSettimaneD.Enabled = false;
                        break;
                }

                txtQuotaAComma707EL_TT.Enabled = false;
                txtQuotaBComma707EL_TT.Enabled = false;
                txtQuotaCComma707EL_TT.Enabled = false;
                txtQuotaDComma707EL_TT.Enabled = false;
                txtRetribuzionePonderataComma707.Enabled = false;
            }
        }

        #endregion Common EL_TT

        #region TT

        private void ValorizzaEtichetteTT()
        {
            lblTitoloDatiRetributivi.Text = "Decreto Legislativo 658";
            htxtRetribUltimoAnnoRetrib.Value = txtRetribUltimoAnnoRetrib.Text = (areaDatiContributivi.DatiCalcolo.fondoTT != null && areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue) ? areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtRetribuzioneBiennio.Text = (areaDatiContributivi.DatiCalcolo.fondoTT != null && areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio.HasValue) ? areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void RecuperaCampiTT(ref bool isContributivo, ref bool isRetributivo)
        {
            if (areaDatiContributivi.DatiCalcolo.fondoTT == null)
                areaDatiContributivi.DatiCalcolo.fondoTT = new GestioneContribFondoTT();

            decimal? dNull = null;
            areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA = !string.IsNullOrEmpty(htxtRetribUltimoAnnoRetrib.Value) ? Convert.ToDecimal(htxtRetribUltimoAnnoRetrib.Value) : dNull;
            areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio = !string.IsNullOrEmpty(txtRetribuzioneBiennio.Text) ? Convert.ToDecimal(txtRetribuzioneBiennio.Text) : dNull;
        }

        private void GestioneEtichetteIsUnicarpeTT(GestioneContribTipoCalcolo tipoCalcolo, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        txtRetribuzioneBiennio.Enabled = false;
                        break;
                }
            }
        }

        #endregion TT

        #region ET

        private void ValorizzaEtichetteET()
        {
            if (areaDatiContributivi != null && areaDatiContributivi.DatiCalcolo != null)
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:

                        lblTitoloDatiRetributivi.Text = "Decreto Legislativo 414";
                        List<GestioneContribDatiServizioUtile> lDatiServizioUtile = null;
                        if (areaDatiContributivi.DatiCalcolo.fondoET == null || areaDatiContributivi.DatiCalcolo.fondoET.lDatiServizioUtile == null || areaDatiContributivi.DatiCalcolo.fondoET.lDatiServizioUtile.Count() == 0)
                        {
                            lDatiServizioUtile = new List<GestioneContribDatiServizioUtile>();
                            GestioneContribDatiServizioUtile DatiServizioUtile = new GestioneContribDatiServizioUtile();
                            DatiServizioUtile.Quota = "A";
                            lDatiServizioUtile.Add(DatiServizioUtile);
                            DatiServizioUtile = new GestioneContribDatiServizioUtile();
                            DatiServizioUtile.Quota = "B";
                            lDatiServizioUtile.Add(DatiServizioUtile);
                            DatiServizioUtile = new GestioneContribDatiServizioUtile();
                            DatiServizioUtile.Quota = "C";
                            lDatiServizioUtile.Add(DatiServizioUtile);
                        }
                        else
                            lDatiServizioUtile = areaDatiContributivi.DatiCalcolo.fondoET.lDatiServizioUtile.ToList();

                        foreach (GestioneContribDatiServizioUtile servUtile in lDatiServizioUtile)
                        {
                            switch (servUtile.Quota)
                            {
                                case "A":
                                    txtServizioUtileAAQtaA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaA.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaA.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRetribPensionabileQtaA.Text = servUtile.RetribuzionePensionabile.HasValue ? servUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtControCodiceRetrQtaA.Text = servUtile.ControCodiceRetributivo.HasValue ? servUtile.ControCodiceRetributivo.Value.ToString() : string.Empty;
                                    break;
                                case "B":
                                    txtServizioUtileAAQtaB.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRetribPensionabileQtaB.Text = servUtile.RetribuzionePensionabile.HasValue ? servUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtControCodiceRetrQtaB.Text = servUtile.ControCodiceRetributivo.HasValue ? servUtile.ControCodiceRetributivo.Value.ToString() : string.Empty;
                                    break;
                                case "C":
                                    txtServizioUtileAAQtaC.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaC.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaC.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    break;
                            }
                        }

                        txtQuotaAComma707ETAA.Text = areaDatiContributivi.DatiCalcolo.QuotaA707AA.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707AA.Value.ToString() : string.Empty;
                        txtQuotaAComma707ETMM.Text = areaDatiContributivi.DatiCalcolo.QuotaA707MM.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707MM.Value.ToString() : string.Empty;
                        txtQuotaAComma707ETGG.Text = areaDatiContributivi.DatiCalcolo.QuotaA707GG.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707GG.Value.ToString() : string.Empty;
                        txtQuotaAComma707ET.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707.Value.ToString() : string.Empty;
                        txtQuotaBComma707ETAA.Text = areaDatiContributivi.DatiCalcolo.QuotaB707AA.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707AA.Value.ToString() : string.Empty;
                        txtQuotaBComma707ETMM.Text = areaDatiContributivi.DatiCalcolo.QuotaB707MM.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707MM.Value.ToString() : string.Empty;
                        txtQuotaBComma707ETGG.Text = areaDatiContributivi.DatiCalcolo.QuotaB707GG.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707GG.Value.ToString() : string.Empty;
                        txtQuotaBComma707ET.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707.Value.ToString() : string.Empty;
                        txtQuotaCComma707ETAA.Text = areaDatiContributivi.DatiCalcolo.QuotaC707AA.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaC707AA.Value.ToString() : string.Empty;
                        txtQuotaCComma707ETMM.Text = areaDatiContributivi.DatiCalcolo.QuotaC707MM.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaC707MM.Value.ToString() : string.Empty;
                        txtQuotaCComma707ETGG.Text = areaDatiContributivi.DatiCalcolo.QuotaC707GG.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaC707GG.Value.ToString() : string.Empty;

                        txtRetribuzionePonderataComma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707.HasValue ? areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                        if (!lDatiServizioUtile.Exists(x => (x.Quota == "A" && (x.ServizioUtileAA.HasValue || x.ServizioUtileMM.HasValue || x.ServizioUtileGG.HasValue || x.RetribuzionePensionabile.HasValue || x.ControCodiceRetributivo.HasValue)) ||
                                                            (x.Quota == "B" && (x.ServizioUtileAA.HasValue || x.ServizioUtileMM.HasValue || x.ServizioUtileGG.HasValue || x.RetribuzionePensionabile.HasValue || x.ControCodiceRetributivo.HasValue))))
                            validateTxtRetribuzioneAgoAnnuaObbl.Enabled = false;
                        break;
                }
        }

        private void RecuperaCampiET(ref bool isContributivo, ref bool isRetributivo)
        {
            short? sNull = null;
            decimal? dNull = null;

            if (areaDatiContributivi.DatiCalcolo.fondoET == null)
                areaDatiContributivi.DatiCalcolo.fondoET = new GestioneContribFondoET();

            areaDatiContributivi.DatiCalcolo.fondoET.lDatiServizioUtile = null;

            List<GestioneContribDatiServizioUtile> lDatiServUtile = new List<GestioneContribDatiServizioUtile>();
            GestioneContribDatiServizioUtile datiServUtile = null;

            if (!string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) || !string.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) ||
                !string.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) || !string.IsNullOrEmpty(txtRetribPensionabileQtaA.Text) ||
                !string.IsNullOrEmpty(txtControCodiceRetrQtaA.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) ? Convert.ToInt16(txtServizioUtileAAQtaA.Text) : sNull;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) ? Convert.ToInt16(txtServizioUtileMMQtaA.Text) : sNull;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ? Convert.ToInt16(txtServizioUtileGGQtaA.Text) : sNull;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtRetribPensionabileQtaA.Text) ? Convert.ToDecimal(txtRetribPensionabileQtaA.Text) : dNull;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtControCodiceRetrQtaA.Text) ? Convert.ToInt16(txtControCodiceRetrQtaA.Text) : sNull;
                datiServUtile.Quota = "A";
                lDatiServUtile.Add(datiServUtile);
                isRetributivo = true;
            }

            if (!string.IsNullOrEmpty(txtServizioUtileAAQtaB.Text) || !string.IsNullOrEmpty(txtServizioUtileMMQtaB.Text) ||
                !string.IsNullOrEmpty(txtServizioUtileGGQtaB.Text) || !string.IsNullOrEmpty(txtRetribPensionabileQtaB.Text) ||
                !string.IsNullOrEmpty(txtControCodiceRetrQtaB.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB.Text) : sNull;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB.Text) : sNull;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB.Text) : sNull;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtRetribPensionabileQtaB.Text) ? Convert.ToDecimal(txtRetribPensionabileQtaB.Text) : dNull;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtControCodiceRetrQtaB.Text) ? Convert.ToInt16(txtControCodiceRetrQtaB.Text) : sNull;
                datiServUtile.Quota = "B";
                lDatiServUtile.Add(datiServUtile);
                isRetributivo = true;
            }

            if (!string.IsNullOrEmpty(txtServizioUtileAAQtaC.Text) || !string.IsNullOrEmpty(txtServizioUtileMMQtaC.Text) ||
                !string.IsNullOrEmpty(txtServizioUtileGGQtaC.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaC.Text) ? Convert.ToInt16(txtServizioUtileAAQtaC.Text) : sNull;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaC.Text) ? Convert.ToInt16(txtServizioUtileMMQtaC.Text) : sNull;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaC.Text) ? Convert.ToInt16(txtServizioUtileGGQtaC.Text) : sNull;
                datiServUtile.Quota = "C";
                lDatiServUtile.Add(datiServUtile);
                isRetributivo = true;
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
                areaDatiContributivi.DatiCalcolo.fondoET.lDatiServizioUtile = lDatiServUtile.ToArray();

            if (!string.IsNullOrEmpty(txtQuotaAComma707ETAA.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA707AA = byte.Parse(txtQuotaAComma707ETAA.Text);
            if (!string.IsNullOrEmpty(txtQuotaAComma707ETMM.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA707MM = byte.Parse(txtQuotaAComma707ETMM.Text);
            if (!string.IsNullOrEmpty(txtQuotaAComma707ETGG.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA707GG = byte.Parse(txtQuotaAComma707ETGG.Text);
            if (!string.IsNullOrEmpty(txtQuotaAComma707ET.Text))
                areaDatiContributivi.DatiCalcolo.QuotaA707 = short.Parse(txtQuotaAComma707ET.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707ETAA.Text))
                areaDatiContributivi.DatiCalcolo.QuotaB707AA = byte.Parse(txtQuotaBComma707ETAA.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707ETMM.Text))
                areaDatiContributivi.DatiCalcolo.QuotaB707MM = byte.Parse(txtQuotaBComma707ETMM.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707ETGG.Text))
                areaDatiContributivi.DatiCalcolo.QuotaB707GG = byte.Parse(txtQuotaBComma707ETGG.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707ET.Text))
                areaDatiContributivi.DatiCalcolo.QuotaB707 = short.Parse(txtQuotaBComma707ET.Text);
            if (!string.IsNullOrEmpty(txtQuotaCComma707ETAA.Text))
                areaDatiContributivi.DatiCalcolo.QuotaC707AA = byte.Parse(txtQuotaCComma707ETAA.Text);
            if (!string.IsNullOrEmpty(txtQuotaCComma707ETMM.Text))
                areaDatiContributivi.DatiCalcolo.QuotaC707MM = byte.Parse(txtQuotaCComma707ETMM.Text);
            if (!string.IsNullOrEmpty(txtQuotaCComma707ETGG.Text))
                areaDatiContributivi.DatiCalcolo.QuotaC707GG = byte.Parse(txtQuotaCComma707ETGG.Text);

            if (!string.IsNullOrEmpty(txtRetribuzionePonderataComma707.Text))
                areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707 = decimal.Parse(txtRetribuzionePonderataComma707.Text);
        }

        private void GestioneEtichetteIsUnicarpeET(GestioneContribTipoCalcolo tipoCalcolo, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        txtServizioUtileAAQtaA.Enabled = false;
                        txtServizioUtileMMQtaA.Enabled = false;
                        txtServizioUtileGGQtaA.Enabled = false;
                        txtRetribPensionabileQtaA.Enabled = false;
                        txtServizioUtileAAQtaB.Enabled = false;
                        txtServizioUtileMMQtaB.Enabled = false;
                        txtServizioUtileGGQtaB.Enabled = false;
                        txtRetribPensionabileQtaB.Enabled = false;
                        txtServizioUtileAAQtaC.Enabled = false;
                        txtServizioUtileMMQtaC.Enabled = false;
                        txtServizioUtileGGQtaC.Enabled = false;

                        pnlControCodiceRetribQtaA.Visible = false;
                        pnlControCodiceRetribQtaB.Visible = false;
                        break;
                }

                txtQuotaAComma707ETAA.Enabled = false;
                txtQuotaAComma707ETMM.Enabled = false;
                txtQuotaAComma707ETGG.Enabled = false;
                txtQuotaAComma707ET.Enabled = false;
                txtQuotaBComma707ETAA.Enabled = false;
                txtQuotaBComma707ETMM.Enabled = false;
                txtQuotaBComma707ETGG.Enabled = false;
                txtQuotaBComma707ET.Enabled = false;
                txtQuotaCComma707ETAA.Enabled = false;
                txtQuotaCComma707ETMM.Enabled = false;
                txtQuotaCComma707ETGG.Enabled = false;

                txtRetribuzionePonderataComma707.Enabled = false;
            }
        }

        #endregion ET

        #region Ante Armonizzazione
        private void ValorizzaEtichetteAnteArmonizzazione(AreaDatiContributivi areaDatiContributivi)
        {
            if (!((bool?)ViewState[EnumViewstate.IsAnteArmonizzazione.ToString()]).GetValueOrDefault())
                return;

            List<GestioneContribDatiServizioUtile> lDatiServizioUtile = null;

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    if (areaDatiContributivi != null && areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.fondoEL != null)
                    {
                        txtAnteArm_RetrPondAGO.Text = areaDatiContributivi.DatiCalcolo.fondoEL.RetrPondAnnuaAGOLimite.ToString();

                        lDatiServizioUtile = areaDatiContributivi.DatiCalcolo.fondoEL.LServizioUtile != null ? areaDatiContributivi.DatiCalcolo.fondoEL.LServizioUtile.ToList() : null;
                        if (lDatiServizioUtile != null)
                        {
                            foreach (GestioneContribDatiServizioUtile servUtile in lDatiServizioUtile)
                            {
                                switch (servUtile.Quota)
                                {
                                    case "A":
                                        txtELAnteArmQtaA_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtELAnteArmQtaA_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        txtELAnteArmQtaA_RetrPens.Text = servUtile.RetribuzionePensionabile.ToString();
                                        txtELAnteArmQtaA_CC.Text = servUtile.ControCodiceRetributivo.HasValue ? servUtile.ControCodiceRetributivo.ToString() : string.Empty;
                                        break;
                                    case "B":
                                        txtELAnteArmQtaB_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtELAnteArmQtaB_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        txtELAnteArmQtaB_RetrPens.Text = servUtile.RetribuzionePensionabile.ToString();
                                        txtELAnteArmQtaB_CC.Text = servUtile.ControCodiceRetributivo.HasValue ? servUtile.ControCodiceRetributivo.ToString() : string.Empty;
                                        break;
                                    case "C":
                                        txtELAnteArmQtaC_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtELAnteArmQtaC_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    if (areaDatiContributivi != null && areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.fondoTT != null)
                    {
                        txtAnteArm_RetrPondAGO.Text = areaDatiContributivi.DatiCalcolo.fondoTT.RetrPondAnnuaAGOLimite.ToString();

                        lDatiServizioUtile = areaDatiContributivi.DatiCalcolo.fondoTT.lDatiServizioUtile != null ? areaDatiContributivi.DatiCalcolo.fondoTT.lDatiServizioUtile.ToList() : null;
                        if (lDatiServizioUtile != null)
                        {
                            foreach (GestioneContribDatiServizioUtile servUtile in lDatiServizioUtile)
                            {
                                switch (servUtile.Quota)
                                {
                                    case "A":
                                        txtTTAnteArmQtaA_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtaA_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                    case "A2":
                                        txtTTAnteArmQtaARid_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtaARid_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                    case "B":
                                        txtTTAnteArmQtB_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtB_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        txtTTAnteArmRetrPensionabileQtB.Text = servUtile.RetribuzionePensionabile.HasValue ? servUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                        break;
                                    case "B2":
                                        txtTTAnteArmQtBRid_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtBRid_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                    case "C":
                                        txtTTAnteArmQtC_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtC_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                    case "C2":
                                        txtTTAnteArmQtCRid_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtCRid_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                    case "D":
                                        txtTTAnteArmQtD_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtD_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        txtTTAnteArmRetrPensionabileQtD.Text = servUtile.RetribuzionePensionabile.HasValue ? servUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                        break;
                                    case "D2":
                                        txtTTAnteArmQtDRid_AA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                        txtTTAnteArmQtDRid_MM.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                        break;
                                }
                            }
                        }

                        if (areaDatiContributivi.DatiCalcolo.fondoTT.PensioneMensileAl53.HasValue)
                            txtTTAnteArmPensioneAl53.Text = areaDatiContributivi.DatiCalcolo.fondoTT.PensioneMensileAl53.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue)
                            txtTTAnteArmRetrUAnno.Text = areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio.HasValue)
                            txtTTAnteArmRetrBiennio.Text = areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (areaDatiContributivi.DatiCalcolo.fondoTT.ElementiAccessori.HasValue)
                            txtTTAnteArmElAccess.Text = areaDatiContributivi.DatiCalcolo.fondoTT.ElementiAccessori.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneSupplementi.HasValue)
                            txtTTAnteArmRetrSup.Text = areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneSupplementi.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    {
                        if (areaDatiContributivi.IsAnteArmonizzazione.GetValueOrDefault())
                            validateTxtRetribuzioneAgoAnnuaObbl.Enabled = false;
                    }
                    break;
            }
        }

        private void RecuperaDatiAnteArmonizzazione(ref bool retributivo)
        {
            if (!((bool?)ViewState[EnumViewstate.IsAnteArmonizzazione.ToString()]).GetValueOrDefault())
                return;

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    RecuperaDatiAnteArmonizzazioneEL(ref retributivo);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    RecuperaDatiAnteArmonizzazioneTT(ref retributivo);
                    break;
            }
        }

        private bool RecuperaDatiAnteArmonizzazioneEL(ref bool retributivo)
        {
            if (areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            if (areaDatiContributivi.DatiCalcolo.fondoEL == null)
                areaDatiContributivi.DatiCalcolo.fondoEL = new GestioneContribFondoEL();

            areaDatiContributivi.DatiCalcolo.fondoEL.LServizioUtile = null;

            List<GestioneContribDatiServizioUtile> lDatiServUtile = new List<GestioneContribDatiServizioUtile>();
            GestioneContribDatiServizioUtile datiServUtile = null;

            if (!string.IsNullOrEmpty(txtELAnteArmQtaA_AA.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaA_MM.Text)
                || !string.IsNullOrEmpty(txtELAnteArmQtaA_RetrPens.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaA_CC.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtELAnteArmQtaA_AA.Text) ? Convert.ToInt16(txtELAnteArmQtaA_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtELAnteArmQtaA_MM.Text) ? Convert.ToInt16(txtELAnteArmQtaA_MM.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtELAnteArmQtaA_RetrPens.Text) ? Convert.ToDecimal(txtELAnteArmQtaA_RetrPens.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtELAnteArmQtaA_CC.Text) ? Convert.ToInt16(txtELAnteArmQtaA_CC.Text) : (short?)null;
                datiServUtile.Quota = "A";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtELAnteArmQtaB_AA.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaB_MM.Text)
                || !string.IsNullOrEmpty(txtELAnteArmQtaB_RetrPens.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaB_CC.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtELAnteArmQtaB_AA.Text) ? Convert.ToInt16(txtELAnteArmQtaB_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtELAnteArmQtaB_MM.Text) ? Convert.ToInt16(txtELAnteArmQtaB_MM.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtELAnteArmQtaB_RetrPens.Text) ? Convert.ToDecimal(txtELAnteArmQtaB_RetrPens.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtELAnteArmQtaB_CC.Text) ? Convert.ToInt16(txtELAnteArmQtaB_CC.Text) : (short?)null;
                datiServUtile.Quota = "B";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtELAnteArmQtaC_AA.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaC_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtELAnteArmQtaC_AA.Text) ? Convert.ToInt16(txtELAnteArmQtaC_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtELAnteArmQtaC_MM.Text) ? Convert.ToInt16(txtELAnteArmQtaC_MM.Text) : (short?)null;
                datiServUtile.Quota = "C";
                lDatiServUtile.Add(datiServUtile);
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
                areaDatiContributivi.DatiCalcolo.fondoEL.LServizioUtile = lDatiServUtile.ToArray();

            areaDatiContributivi.DatiCalcolo.fondoEL.RetrPondAnnuaAGOLimite = !string.IsNullOrEmpty(txtAnteArm_RetrPondAGO.Text) ? decimal.Parse(txtAnteArm_RetrPondAGO.Text) : (decimal?)null;

            if ((lDatiServUtile != null && lDatiServUtile.Count > 0) || areaDatiContributivi.DatiCalcolo.fondoEL.RetrPondAnnuaAGOLimite != null)
                retributivo = true;
            return retributivo;
        }

        private bool RecuperaDatiAnteArmonizzazioneTT(ref bool retributivo)
        {
            if (areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            if (areaDatiContributivi.DatiCalcolo.fondoTT == null)
                areaDatiContributivi.DatiCalcolo.fondoTT = new GestioneContribFondoTT();

            areaDatiContributivi.DatiCalcolo.fondoTT.lDatiServizioUtile = null;

            List<GestioneContribDatiServizioUtile> lDatiServUtile = new List<GestioneContribDatiServizioUtile>();
            GestioneContribDatiServizioUtile datiServUtile = null;

            if (!string.IsNullOrEmpty(txtTTAnteArmQtaA_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtaA_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtaA_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtaA_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtaA_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtaA_MM.Text) : (short?)null;
                datiServUtile.Quota = "A";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtaARid_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtaARid_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtaARid_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtaARid_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtaARid_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtaARid_MM.Text) : (short?)null;
                datiServUtile.Quota = "A2";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtB_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtB_MM.Text) || !string.IsNullOrEmpty(txtTTAnteArmRetrPensionabileQtB.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtB_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtB_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtB_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtB_MM.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtTTAnteArmRetrPensionabileQtB.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmRetrPensionabileQtB.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtTTAnteArmControCodiceRetrQtB.Text) ? CodeUtility.StringToNullableShort(txtTTAnteArmControCodiceRetrQtB.Text) : (short?)null;
                datiServUtile.Quota = "B";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtBRid_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtBRid_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtBRid_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtBRid_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtBRid_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtBRid_MM.Text) : (short?)null;
                datiServUtile.Quota = "B2";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtC_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtC_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtC_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtC_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtC_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtC_MM.Text) : (short?)null;
                datiServUtile.Quota = "C";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtCRid_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtCRid_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtCRid_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtCRid_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtCRid_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtCRid_MM.Text) : (short?)null;
                datiServUtile.Quota = "C2";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtD_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtD_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtD_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtD_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtD_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtD_MM.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtTTAnteArmRetrPensionabileQtD.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmRetrPensionabileQtD.Text) : (decimal?)null;
                datiServUtile.Quota = "D";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtTTAnteArmQtDRid_AA.Text) || !string.IsNullOrEmpty(txtTTAnteArmQtDRid_MM.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtTTAnteArmQtDRid_AA.Text) ? Convert.ToInt16(txtTTAnteArmQtDRid_AA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtTTAnteArmQtDRid_MM.Text) ? Convert.ToInt16(txtTTAnteArmQtDRid_MM.Text) : (short?)null;
                datiServUtile.Quota = "D2";
                lDatiServUtile.Add(datiServUtile);
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
                areaDatiContributivi.DatiCalcolo.fondoTT.lDatiServizioUtile = lDatiServUtile.ToArray();

            areaDatiContributivi.DatiCalcolo.fondoTT.RetrPondAnnuaAGOLimite = !string.IsNullOrEmpty(txtAnteArm_RetrPondAGO.Text) ? CodeUtility.StringToNullableDecimal(txtAnteArm_RetrPondAGO.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.fondoTT.PensioneMensileAl53 = !string.IsNullOrEmpty(txtTTAnteArmPensioneAl53.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmPensioneAl53.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA = !string.IsNullOrEmpty(txtTTAnteArmRetrUAnno.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmRetrUAnno.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio = !string.IsNullOrEmpty(txtTTAnteArmRetrBiennio.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmRetrBiennio.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.fondoTT.ElementiAccessori = !string.IsNullOrEmpty(txtTTAnteArmElAccess.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmElAccess.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneSupplementi = !string.IsNullOrEmpty(txtTTAnteArmRetrSup.Text) ? CodeUtility.StringToNullableDecimal(txtTTAnteArmRetrSup.Text) : (decimal?)null;
            areaDatiContributivi.DatiCalcolo.fondoTT.ControCodiceRetrQtaA = !string.IsNullOrEmpty(txtTTAnteArmControCodiceRetrQtA.Text) ? CodeUtility.StringToNullableInt(txtTTAnteArmControCodiceRetrQtA.Text) : (int?)null;

            if ((lDatiServUtile != null && lDatiServUtile.Count > 0) || areaDatiContributivi.DatiCalcolo.fondoTT.RetrPondAnnuaAGOLimite.HasValue ||
                areaDatiContributivi.DatiCalcolo.fondoTT.PensioneMensileAl53.HasValue || areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue ||
                areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneBiennio.HasValue || areaDatiContributivi.DatiCalcolo.fondoTT.ElementiAccessori.HasValue ||
                areaDatiContributivi.DatiCalcolo.fondoTT.RetribuzioneSupplementi.HasValue || areaDatiContributivi.DatiCalcolo.fondoTT.ControCodiceRetrQtaA.HasValue)
                retributivo = true;

            return retributivo;
        }

        #endregion Ante Armonizzazione

        #region Event

        public event EventHandler CaricaDatiCalcolo;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event EventHandler ShowPopUp;
        public event EventHandler HidePopUp;

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

        protected void RaiseShowPopUp(object sender, EventArgs e)
        {
            if (ShowPopUp != null)
                ShowPopUp(sender, e);
        }

        protected void RaiseHidePopUp(object sender, EventArgs e)
        {
            if (HidePopUp != null)
                HidePopUp(sender, e);
        }

        #endregion Event

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

        #region Enum
        public enum EnumViewstate
        {
            IsAnteArmonizzazione,
        }
        #endregion Enum
    }
}
