using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCDatiCalcoloDZ_New : CustomBaseUserControl, IDatiContributivi, ITitolarePensione, IDatiFondo
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


        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDatiFondo { get; set; }

        #endregion IDatiFondo

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


        #region Event Handlers
        public event EventHandler UpdateSemaforoDatiCalcolo;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;


        protected void RaiseUpdateSemaforoDatiCalcolo(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiCalcolo != null)
                UpdateSemaforoDatiCalcolo(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            if (HidePulsanteSalva != null)
                HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            if (TornaARegistrazioniFondo != null)
                TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers


        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }


        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState["IdRecordFondo"];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.StoreDatiCalcoloByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiCalcolo(this, null);
            }
        }

        //protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        //{
        //    RecuperaCampi();
        //    PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
        //    presenterDatiContributivi.SalvaTabDatiCalcolo(this);

        //    Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
        //    RaiseShowAvviso(this, Cevent);
        //}

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiCalcolo(this);

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState["IdRecordFondo"];
            this.areaDatiFondo.IsPrimoRecord = (bool)ViewState["IsPrimoRecord"];

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Calcolo";
            else
            {
                ClearForm();
                RaiseCaricaDatiCalcolo(this.areaDatiFondo, null);
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteDatiCalcolo(AreaTitolare.DatiPensione datiPensione, AreaDatiFondo areaDatiFondo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaDatiFondo = areaDatiFondo;
            if (this.areaDatiFondo.DatiCalcoloDZ != null)
            {
                ViewState["IdRecordFondo"] = this.areaDatiFondo.IdRecordFondo;
                ViewState["IsPrimoRecord"] = areaDatiFondo.IsPrimoRecord;
                ViewState["TipoCalcolo"] = this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo;
                if (this.areaDatiFondo.CrossDataDZ != null)
                {
                    ViewState["RiduzioneRetrib"] = this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.HasValue ? this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.Value : (bool?)null;
                    ViewState["ContribL214"] = this.areaDatiFondo.CrossDataDZ.IsContribL214Visible.HasValue ? this.areaDatiFondo.CrossDataDZ.IsContribL214Visible.Value : (bool?)null;
                }
                RenderControlsFromTipoCalcolo_TipoFondo(this.domanda.Tipofondo);

                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                        ValorizzaEtichetteDatiCalcoloDZ(datiPensione);
                        ValorizzaEtichetteDatiCalcoloFondoDZ();
                        break;
                }
                GestioneEtichetteIsUnicarpe(this.domanda.Tipofondo, datiPensione);

                if (areaDatiFondo.DatiCalcoloDZ.TipoCalcolo == GestioneContribTipoCalcolo.RetributivoMonti)
                    ViewState["RetributivoMonti"] = "SI";

                ucDoppioCalcolo.ValorizzaEtichetteComma707(this.areaDatiFondo);
                GestioneEtichetteRic(datiPensione);

                if ((bool)Session["IsNewRecord"])
                {
                    PulisciCampi();
                    PulisciCampiServizioUtile();
                    Session["IsNewRecord"] = false;
                }
            }
        }

        internal Presenter.SvrLiquidazioneFs.GestioneContribDatiCalcolo RecuperaCampi()
        {
            bool isContributivo = false;
            bool isRetributivo = false;

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();

            if (this.areaDatiFondo.DatiCalcoloDZ == null)
                this.areaDatiFondo.DatiCalcoloDZ = new GestioneContribDatiCalcolo();

            if (this.areaDatiFondo.CrossDataDZ == null)
                this.areaDatiFondo.CrossDataDZ = new AreaDatiContributivi();

            this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo = (GestioneContribTipoCalcolo)ViewState["TipoCalcolo"];
            this.areaDatiFondo.CrossDataDZ.IsContribL214Visible = (bool?)ViewState["ContribL214"];
            this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];

            RecuperaCampiDatiCalcoloDZ(ref isContributivo, ref isRetributivo);
            RecuperaCampiDatiCalcoloFondoDZ();
            ucDoppioCalcolo.RecuperaCampiComma707(this.areaDatiFondo);

            if (isContributivo && isRetributivo)
            {
                if (ViewState["RetributivoMonti"] != null && ViewState["RetributivoMonti"].ToString() == "SI")
                    this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo = GestioneContribTipoCalcolo.RetributivoMonti;
                else
                    this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo = GestioneContribTipoCalcolo.Misto;
            }
            else if (isContributivo)
                this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo = GestioneContribTipoCalcolo.Contributivo;
            else if (isRetributivo)
                this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo = GestioneContribTipoCalcolo.Retributivo;
            else
                this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo = GestioneContribTipoCalcolo.NonValido;

            this.areaDatiFondo.DatiCalcoloDZ.IsCalcoloValido = true;

            return this.areaDatiFondo.DatiCalcoloDZ;
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
                    switch (areaDatiFondo.DatiCalcoloDZ.TipoCalcolo)
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
                    if (areaDatiFondo.CrossDataDZ.IsSettimane707Visible.GetValueOrDefault())
                    {
                        ucDoppioCalcolo.Visible = true;
                        ucDoppioCalcolo.SetValidationGroup("UCTabDatiCalcoloDZ");
                    }
                    break;
            }
        }

        private void ManageRiduzioneRetributiva()
        {
            if (this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.HasValue && this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.Value)
            {
                pnlRiduzioneRetributiva.Visible = this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.Value;
                pnlRiduzioneRetributiva.Enabled = false;
            }

            bool IsRiduzionePresent = ManageButtonRiduzioneRetributiva(this.domanda.Tipofondo);
            //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
            if (IsRiduzionePresent && this.areaDatiFondo != null &&
                ((this.areaDatiFondo.IsUsuranti.HasValue && this.areaDatiFondo.IsUsuranti.Value) ||
                (this.areaDatiFondo.TipologiaSalvaguardia.HasValue) ||
                (this.areaDatiFondo.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiFondo.IsRiduzioneRetributivaEnabled.Value)))
                IsRiduzionePresent = false;

            btnSalvaDatiCalcoloNoRiduzione.Visible = !IsRiduzionePresent;
            btnPopUp.Visible = IsRiduzionePresent;
            btnSalvaDatiCalcolo.Visible = IsRiduzionePresent;

            if (this.areaDatiFondo.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiFondo.IsRiduzioneRetributivaEnabled.Value)
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
                        (this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.HasValue && this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ManagePnlContributivoLegge214()
        {
            pnlContributiviL214_DZ.Visible = this.areaDatiFondo.CrossDataDZ.IsContribL214Visible.HasValue ? this.areaDatiFondo.CrossDataDZ.IsContribL214Visible.Value : false;
        }

        private void ValorizzaEtichetteDatiCalcoloDZ(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.areaDatiFondo.TipoPensione != null)
            {
                lblTipoPensione.Text = this.areaDatiFondo.TipoPensione.First().Key;
                hdnTipoPensione.Value = this.areaDatiFondo.TipoPensione.First().Value.ToString();
            }

            if(this.areaDatiFondo.DatiFondo.DecorrenzaValidita.HasValue)
            {
                txtDecorrenzaRegistrazione.Enabled = !this.areaDatiFondo.IsPrimoRecord.Value;
                txtDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaValidita.Value);
                if (this.areaDatiFondo.IsPrimoRecord.Value)
                    txtDecorrenzaRegistrazione.CssClass = "tb8 txtUppercase";
                else
                    txtDecorrenzaRegistrazione.CssClass = "txtUppercase tb8 date-picker-base dateGGmmAAAA";
            }

            if (this.areaDatiFondo.DatiCalcoloDZ != null)
            {
                if (this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaA.HasValue)
                    txtRMSQuotaA.Text = this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaA.Value.ToString("0.0000");

                if (this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaA.HasValue)
                    txtNSettimaneQuotaA.Text = this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaA.ToString();

                if (this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaB.HasValue)
                    txtRMSQuotaB.Text = this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaB.Value.ToString("0.0000");

                if (this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaB.HasValue)
                    txtNSettimaneQuotaB.Text = this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaB.ToString();

                if (areaDatiFondo.DatiCalcoloDZ.RiduzioneRetributiva)
                    ddlRiduzioneRetributiva.SelectedValue = "SI";
                else ddlRiduzioneRetributiva.SelectedValue = "NO";
                txtRiduzioneRetributiva.Text = areaDatiFondo.DatiCalcoloDZ.RiduzioneRetributivaPercentuale.HasValue ? areaDatiFondo.DatiCalcoloDZ.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                txtImportoContribTotaleQuotaDL214.Text = areaDatiFondo.DatiCalcoloDZ.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiFondo.DatiCalcoloDZ.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtMontanteQuotaDL214.Text = areaDatiFondo.DatiCalcoloDZ.MontanteQuotaDL214.HasValue ? areaDatiFondo.DatiCalcoloDZ.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtNSettimaneQuotaDL214.Text = areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaDL214.HasValue ? areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaDL214.Value.ToString() : string.Empty;
            }

            
        }

        private void ValorizzaEtichetteDatiCalcoloFondoDZ()
        {
            if (this.areaDatiFondo.DatiCalcoloDZ != null && this.areaDatiFondo.DatiCalcoloDZ.fondoDZ != null)
            {
                List<GestioneContribDatiServizioUtile> listaDatiServizioUtile = null;

                if (this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.Sospensione.HasValue)
                    txtSospensione.Text = String.Format("{0:MM/yyyy}", this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.Sospensione);

                if (this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.PensioneBaseAnnua.HasValue)
                    txtPensioneBaseAnnua.Text = this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.PensioneBaseAnnua.ToString();

                if (this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.lDatiServizioUtile != null && this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.lDatiServizioUtile.Count() > 0)
                    listaDatiServizioUtile = this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.lDatiServizioUtile.ToList();

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
                else
                {
                    PulisciCampiServizioUtile();
                }
            }
        }

        private void PulisciCampi()
        {
            txtDecorrenzaRegistrazione.Text = string.Empty;
            txtDecorrenzaRegistrazione.CssClass = "txtUppercase tb8 date-picker-base dateGGmmAAAA";
            txtDecorrenzaRegistrazione.Enabled = true;
            txtSospensione.Text = string.Empty;
            txtPensioneBaseAnnua.Text = string.Empty;
            txtControcodice_QuotaA.Text = string.Empty;
            txtRMSQuotaA.Text = string.Empty;
            txtNSettimaneQuotaA.Text = string.Empty;
            txtRMSQuotaB.Text = string.Empty;
            txtNSettimaneQuotaB.Text = string.Empty;
            txtRiduzioneRetributiva.Text = string.Empty;
            txtImportoContribTotaleQuotaDL214.Text = string.Empty;
            txtMontanteQuotaDL214.Text = string.Empty;
            txtNSettimaneQuotaDL214.Text = string.Empty;
        }

        private void PulisciCampiServizioUtile()
        { 
            txtServizioUtileAA_QuotaA.Text = string.Empty;
            txtServizioUtileMM_QuotaA.Text = string.Empty;
            txtRetribuzionePensionabile_QuotaA.Text = string.Empty;
            txtControcodice_QuotaA.Text = string.Empty;
            txtServizioUtileAA_QuotaB.Text = string.Empty;
            txtServizioUtileMM_QuotaB.Text = string.Empty;
            txtRetribuzionePensionabile_QuotaB.Text = string.Empty;
            txtControcodice_QuotaB.Text = string.Empty;
        }

        private void GestioneEtichetteIsUnicarpe(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? fondo, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                switch (areaDatiFondo.DatiCalcoloDZ.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.NonValido:
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        if (areaDatiFondo.TipologiaSalvaguardia.HasValue || (areaDatiFondo.IsUsuranti.HasValue && areaDatiFondo.IsUsuranti.Value))
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
                        if (areaDatiFondo.TipologiaSalvaguardia.HasValue || (areaDatiFondo.IsUsuranti.HasValue && areaDatiFondo.IsUsuranti.Value))
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
                        GestioneEtichetteIsUnicarpeDZ(areaDatiFondo.DatiCalcoloDZ.TipoCalcolo);
                        break;
                }
            }
            else if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && areaDatiFondo != null &&
                ((areaDatiFondo.TipologiaSalvaguardia.HasValue) ||
                (areaDatiFondo.IsUsuranti.HasValue && areaDatiFondo.IsUsuranti.Value)))
            {
                switch (areaDatiFondo.DatiCalcoloDZ.TipoCalcolo)
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
            switch (areaDatiFondo.DatiCalcoloDZ.TipoCalcolo)
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
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();

            if (this.areaDatiFondo.DatiCalcoloDZ == null)
                this.areaDatiFondo.DatiCalcoloDZ = new GestioneContribDatiCalcolo();

            if (this.areaDatiFondo.CrossDataDZ == null)
                this.areaDatiFondo.CrossDataDZ = new AreaDatiContributivi();

            switch (this.areaDatiFondo.DatiCalcoloDZ.TipoCalcolo)
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

            if (this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaA.HasValue || this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaA.HasValue || this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaB.HasValue ||
                this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaB.HasValue || this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaD.HasValue || this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaD.HasValue)
                isRetributivo = true;
            if (this.areaDatiFondo.DatiCalcoloDZ.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiFondo.DatiCalcoloDZ.MontanteQuotaDL214.HasValue || this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaDL214.HasValue)
                isContributivo = true;
        }

        private void RecuperaCampiDatiCalcoloFondoDZ()
        {
            if (areaDatiFondo.DatiCalcoloDZ.fondoDZ == null)
                areaDatiFondo.DatiCalcoloDZ.fondoDZ = new GestioneContribFondoDZ();

            if (!string.IsNullOrEmpty(txtDecorrenzaRegistrazione.Text))
            {
                this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.DecorrenzaValidita = Utility.GetDateFromString(txtDecorrenzaRegistrazione.Text);
            }
                
            if (!string.IsNullOrEmpty(txtSospensione.Text))
                this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.Sospensione = Utility.GetDateFromString(txtSospensione.Text);
            if (!string.IsNullOrEmpty(txtPensioneBaseAnnua.Text))
                this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.PensioneBaseAnnua = decimal.Parse(txtPensioneBaseAnnua.Text);

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
                this.areaDatiFondo.DatiCalcoloDZ.fondoDZ.lDatiServizioUtile = lDatiServUtile.ToArray();
        }

        private void RecuperaCampiRetributivo()
        {
            this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaA = !string.IsNullOrEmpty(txtRMSQuotaA.Text) ? Convert.ToDecimal(txtRMSQuotaA.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaA = !string.IsNullOrEmpty(txtNSettimaneQuotaA.Text) ? Convert.ToInt32(txtNSettimaneQuotaA.Text) : (int?)null;
            this.areaDatiFondo.DatiCalcoloDZ.RMSQuotaB = !string.IsNullOrEmpty(txtRMSQuotaB.Text) ? Convert.ToDecimal(txtRMSQuotaB.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaB = !string.IsNullOrEmpty(txtNSettimaneQuotaB.Text) ? Convert.ToInt32(txtNSettimaneQuotaB.Text) : (int?)null;
        }

        private void RecuperaCampiRiduzioneRetributiva()
        {
            if (this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.HasValue && this.areaDatiFondo.CrossDataDZ.IsRiduzioneRetribVisible.Value)
            {
                if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    this.areaDatiFondo.DatiCalcoloDZ.RiduzioneRetributiva = true;
                else if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    this.areaDatiFondo.DatiCalcoloDZ.RiduzioneRetributiva = false;

                this.areaDatiFondo.DatiCalcoloDZ.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;
            }
        }

        private void RecuperaCampiContributivoLegge214()
        {
            this.areaDatiFondo.DatiCalcoloDZ.ImportoContribTotaleQuotaDL214 = !string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text) ? Convert.ToDecimal(txtImportoContribTotaleQuotaDL214.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcoloDZ.MontanteQuotaDL214 = !string.IsNullOrEmpty(txtMontanteQuotaDL214.Text) ? Convert.ToDecimal(txtMontanteQuotaDL214.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcoloDZ.NSettimaneQuotaDL214 = !string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text) ? Convert.ToInt32(txtNSettimaneQuotaDL214.Text) : (int?)null;
        }

        #endregion private properties
    }
}