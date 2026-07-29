using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCArt11e14GAS_ES : CustomBaseUserControl, IDatiContributivi
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            RenderControls();
        }

        protected void btnSalvaArt11_14_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiArt11_14GAS(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaArt11_14_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiArt11_14GAS(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Art. 11 e 14";
            else
            {
                ClearForm();
                ValorizzaEtichette();
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        public void ValorizzaEtichette()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiArt11e14 != null)
            {
                switch (domanda.Tipofondo)
                {
                    case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:

                        if (this.areaDatiContributivi.DatiArt11e14.ContributiTotaliSupplementoDPR143271.HasValue)
                            txtContributiTotaliSupplementoDPR143271.Text = this.areaDatiContributivi.DatiArt11e14.ContributiTotaliSupplementoDPR143271.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.ContribuzioneEsclusivaDPR143271.HasValue)
                            txtContribuzioneEsclusivaDPR143271.Text = this.areaDatiContributivi.DatiArt11e14.ContribuzioneEsclusivaDPR143271.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.CCTotaliArt14.HasValue)
                            txtCCTotaliArt14.Text = this.areaDatiContributivi.DatiArt11e14.CCTotaliArt14.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.ContribuzioneEsclusiva.HasValue)
                            txtContribuzioneEsclusiva.Text = this.areaDatiContributivi.DatiArt11e14.ContribuzioneEsclusiva.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.DecDPCM.HasValue)
                            txtDecDPCM.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiArt11e14.DecDPCM);

                        if (this.areaDatiContributivi.DatiArt11e14.RMSArt14.HasValue)
                            txtRMSArt14.Text = this.areaDatiContributivi.DatiArt11e14.RMSArt14.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.RMSSent72.HasValue)
                            txtRMSSent72.Text = this.areaDatiContributivi.DatiArt11e14.RMSSent72.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.CCTotaliArt11.HasValue)
                            txtCCTotaliArt11.Text = this.areaDatiContributivi.DatiArt11e14.CCTotaliArt11.ToString();

                        if (this.areaDatiContributivi.DatiArt11e14.CCEsclusivaArt11.HasValue)
                            txtCCEsclusivaArt11.Text = this.areaDatiContributivi.DatiArt11e14.CCEsclusivaArt11.ToString();
                        break;
                    case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                        if (this.areaDatiContributivi.DatiArt11e14.DecDPCM.HasValue)
                            txtboxES_DecDPCM.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiArt11e14.DecDPCM);
                        if (this.areaDatiContributivi.DatiArt11e14.RMSArt14.HasValue)
                            txtboxES_RmsDCPM.Text = this.areaDatiContributivi.DatiArt11e14.RMSArt14.ToString();
                        if (this.areaDatiContributivi.DatiArt11e14.RMSSent72.HasValue)
                            txtboxES_Sent74Rms.Text = this.areaDatiContributivi.DatiArt11e14.RMSSent72.ToString();
                        break;
                }

            }
        }

        public void RecuperaCampi()
        {
            if (this.domanda == null)
                this.domanda =(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda) Session["Domanda"];
            switch (domanda.Tipofondo)
            {
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:

                    if (this.areaDatiContributivi == null)
                        this.areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();

                    if (this.areaDatiContributivi.DatiArt11e14 == null)
                        this.areaDatiContributivi.DatiArt11e14 = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribDatiArt11e14();

                    if (!string.IsNullOrEmpty(txtContributiTotaliSupplementoDPR143271.Text))
                        this.areaDatiContributivi.DatiArt11e14.ContributiTotaliSupplementoDPR143271 = decimal.Parse(txtContributiTotaliSupplementoDPR143271.Text);

                    if (!string.IsNullOrEmpty(txtContribuzioneEsclusivaDPR143271.Text))
                        this.areaDatiContributivi.DatiArt11e14.ContribuzioneEsclusivaDPR143271 = decimal.Parse(txtContribuzioneEsclusivaDPR143271.Text);

                    if (!string.IsNullOrEmpty(txtCCTotaliArt14.Text))
                        this.areaDatiContributivi.DatiArt11e14.CCTotaliArt14 = decimal.Parse(txtCCTotaliArt14.Text);

                    if (!string.IsNullOrEmpty(txtContribuzioneEsclusiva.Text))
                        this.areaDatiContributivi.DatiArt11e14.ContribuzioneEsclusiva = decimal.Parse(txtContribuzioneEsclusiva.Text);

                    if (!string.IsNullOrEmpty(txtDecDPCM.Text))
                        this.areaDatiContributivi.DatiArt11e14.DecDPCM = DateTime.Parse(txtDecDPCM.Text);

                    if (!string.IsNullOrEmpty(txtRMSArt14.Text))
                        this.areaDatiContributivi.DatiArt11e14.RMSArt14 = decimal.Parse(txtRMSArt14.Text);

                    if (!string.IsNullOrEmpty(txtRMSSent72.Text))
                        this.areaDatiContributivi.DatiArt11e14.RMSSent72 = decimal.Parse(txtRMSSent72.Text);

                    if (!string.IsNullOrEmpty(txtCCTotaliArt11.Text))
                        this.areaDatiContributivi.DatiArt11e14.CCTotaliArt11 = decimal.Parse(txtCCTotaliArt11.Text);

                    if (!string.IsNullOrEmpty(txtCCEsclusivaArt11.Text))
                        this.areaDatiContributivi.DatiArt11e14.CCEsclusivaArt11 = decimal.Parse(txtCCEsclusivaArt11.Text);
                    break;
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                     if (this.areaDatiContributivi == null)
                        this.areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();
                    if (this.areaDatiContributivi.DatiArt11e14 == null)
                        this.areaDatiContributivi.DatiArt11e14 = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribDatiArt11e14();

                    if (!string.IsNullOrEmpty(txtboxES_DecDPCM.Text))
                        this.areaDatiContributivi.DatiArt11e14.DecDPCM = DateTime.Parse(txtboxES_DecDPCM.Text);
                    if (!string.IsNullOrEmpty(txtboxES_RmsDCPM.Text))
                        this.areaDatiContributivi.DatiArt11e14.RMSArt14 = decimal.Parse(txtboxES_RmsDCPM.Text);
                    if (!string.IsNullOrEmpty(txtboxES_Sent74Rms.Text))
                        this.areaDatiContributivi.DatiArt11e14.RMSSent72 = decimal.Parse(txtboxES_Sent74Rms.Text);
                    break;
            }
        }

        internal void EnableDisableBtnSalva(bool enable)
        {
            this.btnSalvaArt11_14.Enabled = enable;
            this.btnEliminaArt11_14.Enabled = enable;
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

        private void RenderControls()
        {
            if (this.domanda == null)
                this.domanda =(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda) Session["Domanda"];
            switch (domanda.Tipofondo)
            {
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    divDatiArt11e14.Visible = false;
                    divES_DatiArt11e14.Visible = true;
                    divSuppArt11.Visible = false;
                    break;
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    divDatiArt11e14.Visible = true;
                    divES_DatiArt11e14.Visible = false;
                    divSuppArt11.Visible = true;
                    break;
            }
        }


        #region Event

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion Event

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi
    }
}