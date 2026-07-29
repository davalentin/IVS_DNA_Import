using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiCalcoloDZ : CustomBaseUserControl, IDatiContributivi, ITitolarePensione
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

        #region Event

        public event EventHandler CaricaDatiCalcolo;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            RecuperaCampi(this.domanda.Tipofondo);
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

        internal void ValorizzaEtichetteDatiCalcolo(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ViewState["TipoCalcolo"] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
            ViewState["RiduzioneRetrib"] = this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue ? this.areaDatiContributivi.IsRiduzioneRetribVisible.Value : (bool?)null;
            ViewState["ContribL214"] = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : (bool?)null;

            RenderControlsFromTipoCalcolo_TipoFondo(this.domanda.Tipofondo);

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    ValorizzaEtichetteDatiCalcoloDZ(datiPensione);
                    ValorizzaEtichetteDatiCalcoloFondoDZ();
                    break;
            }
            GestioneEtichetteIsUnicarpe(this.domanda.Tipofondo, datiPensione);

            if (areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.RetributivoMonti)
                ViewState["RetributivoMonti"] = "SI";

            ucDoppioCalcolo.ValorizzaEtichetteComma707(this.areaDatiContributivi);
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
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    RecuperaCampiDatiCalcoloDZ(ref isContributivo, ref isRetributivo);
                    RecuperaCampiDatiCalcoloFondoDZ();
                    ucDoppioCalcolo.RecuperaCampiComma707(this.areaDatiContributivi);
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

        #region private properties

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            // popolamento dei controls html con i valori di default (es.: txtPippo.Text = "mm/aaaa";)
        }

        private void RenderControlsFromTipoCalcolo_TipoFondo(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? fondo)
        {
            switch (fondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                    {
                        case GestioneContribTipoCalcolo.Retributivo:
                            pnlHeader_DZ.Visible = true;
                            pnlServizioUtile_DZ.Visible = true;
                            pnlRetributivi_DZ.Visible = true;
                            ManageRiduzioneRetributiva();
                            break;
                        case GestioneContribTipoCalcolo.RetributivoMonti:
                            pnlHeader_DZ.Visible = true;
                            pnlRetributivi_DZ.Visible = true;
                            ManageRiduzioneRetributiva();
                            ManagePnlContributivoLegge214();
                            break;
                        case GestioneContribTipoCalcolo.NonValido:
                            break;
                    }
                    if (areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
                    {
                        ucDoppioCalcolo.Visible = true;
                        ucDoppioCalcolo.SetValidationGroup("UCTabDatiCalcoloDZ");

                    }
                    break;
            }
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

        private void ManagePnlContributivoLegge214()
        {
            pnlContributiviL214_DZ.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
        }

        private void ValorizzaEtichetteDatiCalcoloDZ(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.areaDatiContributivi.TipoPensione != null)
            {
                lblTipoPensione.Text = this.areaDatiContributivi.TipoPensione.First().Key;
                hdnTipoPensione.Value = this.areaDatiContributivi.TipoPensione.First().Value.ToString();
            }

            if (!datiPensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenza.Text = string.Empty;
            else
                lblDecorrenza.Text = Convert.ToString(datiPensione.DecorrenzaOriginaria).Substring(3, 7);

            if (this.areaDatiContributivi.DatiCalcolo != null)
            {
                if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue)
                    txtRMSQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.Value.ToString("0.0000");

                if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue)
                    txtNSettimaneQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue)
                    txtRMSQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.Value.ToString("0.0000");

                if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue)
                    txtNSettimaneQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.ToString();

                if (areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva)
                    ddlRiduzioneRetributiva.SelectedValue = "SI";
                else ddlRiduzioneRetributiva.SelectedValue = "NO";
                txtRiduzioneRetributiva.Text = areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.HasValue ? areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString() : string.Empty;
            }
        }

        private void ValorizzaEtichetteDatiCalcoloFondoDZ()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoDZ != null)
            {
                List<GestioneContribDatiServizioUtile> listaDatiServizioUtile = null;

                if (this.areaDatiContributivi.DatiCalcolo.fondoDZ.Sospensione.HasValue)
                    txtSospensione.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiCalcolo.fondoDZ.Sospensione);

                if (this.areaDatiContributivi.DatiCalcolo.fondoDZ.PensioneBaseAnnua.HasValue)
                    txtPensioneBaseAnnua.Text = this.areaDatiContributivi.DatiCalcolo.fondoDZ.PensioneBaseAnnua.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.fondoDZ.lDatiServizioUtile != null && this.areaDatiContributivi.DatiCalcolo.fondoDZ.lDatiServizioUtile.Count() > 0)
                    listaDatiServizioUtile = this.areaDatiContributivi.DatiCalcolo.fondoDZ.lDatiServizioUtile.ToList();

                if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count > 0)
                {
                    foreach (GestioneContribDatiServizioUtile datiServizioUtile in listaDatiServizioUtile)
                    {
                        switch (datiServizioUtile.Quota)
                        {
                            case "A":
                                txtServizioUtileAA_QuotaA.Text = datiServizioUtile.ServizioUtileAA.HasValue ? datiServizioUtile.ServizioUtileAA.ToString() : string.Empty;
                                txtServizioUtileMM_QuotaA.Text = datiServizioUtile.ServizioUtileMM.HasValue ? datiServizioUtile.ServizioUtileMM.ToString() : string.Empty;
                                txtRetribuzionePensionabile_QuotaA.Text = datiServizioUtile.RetribuzionePensionabile.HasValue ? datiServizioUtile.RetribuzionePensionabile.ToString() : string.Empty;
                                txtControcodice_QuotaA.Text = datiServizioUtile.ControCodiceRetributivo.HasValue ? datiServizioUtile.ControCodiceRetributivo.ToString() : string.Empty;
                                break;
                            case "B":
                                txtServizioUtileAA_QuotaB.Text = datiServizioUtile.ServizioUtileAA.HasValue ? datiServizioUtile.ServizioUtileAA.ToString() : string.Empty;
                                txtServizioUtileMM_QuotaB.Text = datiServizioUtile.ServizioUtileMM.HasValue ? datiServizioUtile.ServizioUtileMM.ToString() : string.Empty;
                                txtRetribuzionePensionabile_QuotaB.Text = datiServizioUtile.RetribuzionePensionabile.HasValue ? datiServizioUtile.RetribuzionePensionabile.ToString() : string.Empty;
                                txtControcodice_QuotaB.Text = datiServizioUtile.ControCodiceRetributivo.HasValue ? datiServizioUtile.ControCodiceRetributivo.ToString() : string.Empty;
                                break;
                        }
                    }
                }
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
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                        GestioneEtichetteIsUnicarpeDZ(areaDatiContributivi.DatiCalcolo.TipoCalcolo);
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

        private void GestioneEtichetteIsUnicarpeDZ(GestioneContribTipoCalcolo tipoCalcolo)
        {
            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                case GestioneContribTipoCalcolo.Retributivo:
                case GestioneContribTipoCalcolo.Misto:
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    break;
            }
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                txtSospensione.Enabled = false;
                txtPensioneBaseAnnua.Enabled = false;
                txtServizioUtileAA_QuotaA.Enabled = false;
                txtServizioUtileMM_QuotaA.Enabled = false;
                txtRetribuzionePensionabile_QuotaA.Enabled = false;
                txtControcodice_QuotaA.Enabled = false;
                txtServizioUtileAA_QuotaB.Enabled = false;
                txtServizioUtileMM_QuotaB.Enabled = false;
                txtRetribuzionePensionabile_QuotaB.Enabled = false;
                txtControcodice_QuotaB.Enabled = false;
                txtRMSQuotaA.Enabled = false;
                txtNSettimaneQuotaA.Enabled = false;
                txtRMSQuotaB.Enabled = false;
                txtNSettimaneQuotaB.Enabled = false;
                txtImportoContribTotaleQuotaDL214.Enabled = false;
                txtImportoContribTotaleQuotaDL214RF.Enabled = false;
                txtNSettimaneQuotaDL214.Enabled = false;
                RFVtxtNSettimaneQuotaDL214.Enabled = false;
                txtMontanteQuotaDL214.Enabled = false;
                RFVtxtMontanteQuotaDL214.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }
        }

        private void RecuperaCampiDatiCalcoloDZ(ref bool isContributivo, ref bool isRetributivo)
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            switch (this.areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Retributivo:

                    RecuperaCampiRetributivo();
                    RecuperaCampiRiduzioneRetributiva();

                    break;
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    RecuperaCampiRetributivo();
                    RecuperaCampiContributivoLegge214();
                    RecuperaCampiRiduzioneRetributiva();
                    break;
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }

            if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue || this.areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue)
                isRetributivo = true;
            if (this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                isContributivo = true;
        }

        private void RecuperaCampiDatiCalcoloFondoDZ()
        {
            if (areaDatiContributivi.DatiCalcolo.fondoDZ == null)
                areaDatiContributivi.DatiCalcolo.fondoDZ = new GestioneContribFondoDZ();

            if (!string.IsNullOrEmpty(txtSospensione.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoDZ.Sospensione = Utility.GetDateFromString(txtSospensione.Text);
            if (!string.IsNullOrEmpty(txtPensioneBaseAnnua.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoDZ.PensioneBaseAnnua = decimal.Parse(txtPensioneBaseAnnua.Text);

            List<GestioneContribDatiServizioUtile> lDatiServUtile = new List<GestioneContribDatiServizioUtile>();
            GestioneContribDatiServizioUtile datiServUtile = null;

            if (!String.IsNullOrEmpty(txtServizioUtileAA_QuotaA.Text) || !String.IsNullOrEmpty(txtServizioUtileMM_QuotaA.Text) || !String.IsNullOrEmpty(txtRetribuzionePensionabile_QuotaA.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "A";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAA_QuotaA.Text) ? Convert.ToInt16(txtServizioUtileAA_QuotaA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMM_QuotaA.Text) ? Convert.ToInt16(txtServizioUtileMM_QuotaA.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtRetribuzionePensionabile_QuotaA.Text) ? Convert.ToDecimal(txtRetribuzionePensionabile_QuotaA.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtControcodice_QuotaA.Text) ? Convert.ToInt16(txtControcodice_QuotaA.Text) : (short?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAA_QuotaB.Text) || !String.IsNullOrEmpty(txtServizioUtileMM_QuotaB.Text) || !String.IsNullOrEmpty(txtRetribuzionePensionabile_QuotaB.Text))
            {
                datiServUtile = new GestioneContribDatiServizioUtile();
                datiServUtile.Quota = "B";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAA_QuotaB.Text) ? Convert.ToInt16(txtServizioUtileAA_QuotaB.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMM_QuotaB.Text) ? Convert.ToInt16(txtServizioUtileMM_QuotaB.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtRetribuzionePensionabile_QuotaB.Text) ? Convert.ToDecimal(txtRetribuzionePensionabile_QuotaB.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtControcodice_QuotaB.Text) ? Convert.ToInt16(txtControcodice_QuotaB.Text) : (short?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (lDatiServUtile != null && lDatiServUtile.Count > 0)
                this.areaDatiContributivi.DatiCalcolo.fondoDZ.lDatiServizioUtile = lDatiServUtile.ToArray();
        }

        private void RecuperaCampiRetributivo()
        {
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaA = !string.IsNullOrEmpty(txtRMSQuotaA.Text) ? Convert.ToDecimal(txtRMSQuotaA.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA = !string.IsNullOrEmpty(txtNSettimaneQuotaA.Text) ? Convert.ToInt32(txtNSettimaneQuotaA.Text) : (int?)null;
            this.areaDatiContributivi.DatiCalcolo.RMSQuotaB = !string.IsNullOrEmpty(txtRMSQuotaB.Text) ? Convert.ToDecimal(txtRMSQuotaB.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB = !string.IsNullOrEmpty(txtNSettimaneQuotaB.Text) ? Convert.ToInt32(txtNSettimaneQuotaB.Text) : (int?)null;
        }

        private void RecuperaCampiRiduzioneRetributiva()
        {
            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
            {
                if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = true;
                else if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = false;

                this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;
            }
        }

        private void RecuperaCampiContributivoLegge214()
        {
            this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214 = !string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text) ? Convert.ToDecimal(txtImportoContribTotaleQuotaDL214.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = !string.IsNullOrEmpty(txtMontanteQuotaDL214.Text) ? Convert.ToDecimal(txtMontanteQuotaDL214.Text) : (decimal?)null;
            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = !string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text) ? Convert.ToInt32(txtNSettimaneQuotaDL214.Text) : (int?)null;
        }

        #endregion private properties
    }
}