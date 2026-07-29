using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiCalcoloPM : CustomBaseUserControl, IDatiContributivi, ITitolarePensione
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

        #region Event Page

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? fondo = domanda.Tipofondo;
            RecuperaCampi();

            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiCalcolo(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);

        }

        #endregion Event Page

        #region Private Method

        private void RecuperaCampiDatiCalcoloPM(ref bool isContributivo, ref bool isRetributivo)
        {
            //Dati Retributivi 
            //quota A
            if (!string.IsNullOrEmpty(txtRMSQuotaA.Text))
                areaDatiContributivi.DatiCalcolo.RMSQuotaA = decimal.Parse(txtRMSQuotaA.Text);
            if (!string.IsNullOrEmpty(txtSettimaneEsclusiveQuotaA.Text))
                areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaA = int.Parse(txtSettimaneEsclusiveQuotaA.Text);
            if (!string.IsNullOrEmpty(txtSettimaneQuotaA.Text))
                areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA = int.Parse(txtSettimaneQuotaA.Text);
            //quota B
            if (!string.IsNullOrEmpty(txtRMSQuotaB.Text))
                areaDatiContributivi.DatiCalcolo.RMSQuotaB = decimal.Parse(txtRMSQuotaB.Text);
            if (!string.IsNullOrEmpty(txtSettimaneEsclusiveQuotaB.Text))
                areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaB = int.Parse(txtSettimaneEsclusiveQuotaB.Text);
            if (!string.IsNullOrEmpty(txtSettimaneQuotaB.Text))
                areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB = int.Parse(txtSettimaneQuotaB.Text);
            //Dati Contributivi L335
            if (!string.IsNullOrEmpty(txtSettimaneL335.Text))
                areaDatiContributivi.DatiCalcolo.NSettimane = int.Parse(txtSettimaneL335.Text);
            if (!string.IsNullOrEmpty(txtMontateTotaleL335.Text))
                areaDatiContributivi.DatiCalcolo.Montante = decimal.Parse(txtMontateTotaleL335.Text);
            //Dati Contributivi L214
            if (!string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text))
                areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = int.Parse(txtNSettimaneQuotaDL214.Text);
            if (!string.IsNullOrEmpty(txtMontanteQuotaDL214.Text))
                areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = decimal.Parse(txtMontanteQuotaDL214.Text);
            if (!string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text))
                areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214 = decimal.Parse(txtImportoContribTotaleQuotaDL214.Text);

            if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue || this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue ||
                this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue || this.areaDatiContributivi.DatiCalcolo.RMSQuotaD.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaD.HasValue)
                isRetributivo = true;
            if (this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue || this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue
                || areaDatiContributivi.DatiCalcolo.NSettimane.HasValue || areaDatiContributivi.DatiCalcolo.Montante.HasValue)
                isContributivo = true;


        }

        internal void RecuperaCampi()
        {

            if (areaDatiContributivi == null)
                areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            bool isContributivo = false;
            bool isRetributivo = false;

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = (GestioneContribTipoCalcolo)ViewState["TipoCalcolo"];
            this.areaDatiContributivi.IsContribL214Visible = (bool?)ViewState["ContribL214"];
            this.areaDatiContributivi.IsRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    RecuperaCampiDatiCalcoloPM(ref isContributivo, ref isRetributivo);
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

        internal void ValorizzaEtichette(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ViewState["TipoCalcolo"] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
            ViewState["RiduzioneRetrib"] = this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue ? this.areaDatiContributivi.IsRiduzioneRetribVisible.Value : (bool?)null;
            ViewState["ContribL214"] = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : (bool?)null;

            RenderControls();

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    ValorizzaEtichetteDatiCalcoloPM(this.areaDatiContributivi);
                    ucDoppioCalcolo.ValorizzaEtichetteComma707(this.areaDatiContributivi);
                    break;
            }

            //GestioneEtichetteIsUnicarpe(this.domanda.Tipofondo, datiPensione);

            if (areaDatiContributivi.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.RetributivoMonti)
                ViewState["RetributivoMonti"] = "SI";

            GestioneEtichetteRic(datiPensione);
        }

        private void ValorizzaEtichetteDatiCalcoloPM(AreaDatiContributivi area)
        {
            if (area != null && area.DatiCalcolo != null)
            {
                //Dati Retributivi 
                //quota A
                txtRMSQuotaA.Text = (area.DatiCalcolo.RMSQuotaA.HasValue) ? area.DatiCalcolo.RMSQuotaA.Value.ToString("0.0000") : string.Empty;
                txtSettimaneEsclusiveQuotaA.Text = (area.DatiCalcolo.NSettimaneEsclusiveQuotaA.HasValue) ? area.DatiCalcolo.NSettimaneEsclusiveQuotaA.Value.ToString() : string.Empty;
                txtSettimaneQuotaA.Text = (area.DatiCalcolo.NSettimaneQuotaA.HasValue) ? area.DatiCalcolo.NSettimaneQuotaA.Value.ToString() : string.Empty;
                //quota B
                txtRMSQuotaB.Text = (area.DatiCalcolo.RMSQuotaB.HasValue) ? area.DatiCalcolo.RMSQuotaB.Value.ToString("0.0000") : string.Empty;
                txtSettimaneEsclusiveQuotaB.Text = (area.DatiCalcolo.NSettimaneEsclusiveQuotaB.HasValue) ? area.DatiCalcolo.NSettimaneEsclusiveQuotaB.Value.ToString() : string.Empty;
                txtSettimaneQuotaB.Text = (area.DatiCalcolo.NSettimaneQuotaB.HasValue) ? area.DatiCalcolo.NSettimaneQuotaB.Value.ToString() : string.Empty;
                //Dati Contributivi L335
                txtSettimaneL335.Text = (area.DatiCalcolo.NSettimane.HasValue) ? area.DatiCalcolo.NSettimane.Value.ToString() : string.Empty;
                txtMontateTotaleL335.Text = (area.DatiCalcolo.Montante.HasValue) ? area.DatiCalcolo.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                //Dati Contributivi L214
                txtNSettimaneQuotaDL214.Text = (area.DatiCalcolo.NSettimaneQuotaDL214.HasValue) ? area.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString() : string.Empty;
                txtMontanteQuotaDL214.Text = (area.DatiCalcolo.MontanteQuotaDL214.HasValue) ? area.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtImportoContribTotaleQuotaDL214.Text = (area.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue) ? area.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            }
        }

        private void RenderControls()
        {
            divDatiRetributivi.Visible = false;
            divDatiContributiviL335.Visible = false;
            divContributiviL214.Visible = false;

            ManageSettimaneEscusive(this.areaDatiContributivi);

            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    divDatiContributiviL335.Visible = true;
                    //L214
                    divContributiviL214.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                case GestioneContribTipoCalcolo.Retributivo:
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    divDatiRetributivi.Visible = true;
                    //L214
                    divContributiviL214.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                case GestioneContribTipoCalcolo.Misto:
                    divDatiRetributivi.Visible = true;
                    divDatiContributiviL335.Visible = true;
                    //L214
                    divContributiviL214.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }

            if (this.areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
            {
                ucDoppioCalcolo.Visible = true;
                ucDoppioCalcolo.SetValidationGroup("UCTabDatiCalcoloPM");
            }
        }

        private void ManageSettimaneEscusive(AreaDatiContributivi areaDatiContributivi)
        {
            bool hideSettEscusive = areaDatiContributivi.IsDecorrenzaSuccSett1989.HasValue ? !areaDatiContributivi.IsDecorrenzaSuccSett1989.Value : false;
            lblSettEsclusiveQuotaA.Visible = lblSettEsclusiveQuotaB.Visible = hideSettEscusive;
            txtSettimaneEsclusiveQuotaA.Visible = txtSettimaneEsclusiveQuotaB.Visible = hideSettEscusive;
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                txtRMSQuotaA.Enabled = false;
                txtSettimaneQuotaA.Enabled = false;
                txtSettimaneEsclusiveQuotaA.Enabled = false;
                txtRMSQuotaB.Enabled = false;
                txtSettimaneQuotaB.Enabled = false;
                txtSettimaneEsclusiveQuotaB.Enabled = false;
                txtMontateTotaleL335.Enabled = false;
                RFVMontanteTotaleL555.Enabled = false;
                txtSettimaneL335.Enabled = false;
                RFVtxtSettimaneL335.Enabled = false;
                txtImportoContribTotaleQuotaDL214.Enabled = false;
                txtImportoContribTotaleQuotaDL214RF.Enabled = false;
                txtNSettimaneQuotaDL214.Enabled = false;
                RFVtxtNSettimaneQuotaDL214.Enabled = false;
                txtMontanteQuotaDL214.Enabled = false;
                RFVtxtMontanteQuotaDL214.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }
        }
        #endregion Private Method

        internal void EnableDisableBtnSalva(bool bReturn)
        {
            this.btnSalvaDatiCalcolo.Enabled = bReturn;

            this.btnEliminaDatiCalcolo.Enabled = bReturn;
        }
    }
}