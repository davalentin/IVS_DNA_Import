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
    public partial class UCDatiAgoAltraPensione_ET : CustomBaseUserControl, IDatiContributivi
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSalvaDatiAgo_Click(object sender, EventArgs e)
        {
            if(this.domanda==null)
                this.domanda = this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new Presenter.SvrLiquidazioneFs.AreaDatiContributivi();
            if (this.areaDatiContributivi.DatiAgoAltraPensione == null)
                this.areaDatiContributivi.DatiAgoAltraPensione = new Presenter.SvrLiquidazioneFs.GestioneContribDatiAgoAltraPensione();
            this.areaDatiContributivi.DatiAgoAltraPensione = RecuperaCampi();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiAgoAltraPensione(this);
            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaDatiAgo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiAgoAltraPensione(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Ago";
            else
            {
                ClearForm();
            }
            RaiseShowAvvisoElimina(this, null);
            //Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            //RaiseShowAvvisoElimina(this, Cevent);
        }

        public void ValorizzaEtichette(INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi)
        {
            if (areaDatiContributivi != null &&  areaDatiContributivi.DatiAgoAltraPensione!=null)
            {
                Presenter.SvrLiquidazioneFs.GestioneContribDatiAgoAltraPensione datiAgoAltraPensione = areaDatiContributivi.DatiAgoAltraPensione;
                this.ddlCategoria.SelectedValue = datiAgoAltraPensione.CategoriaAltraPensione;
                this.txtCertificato.Text = datiAgoAltraPensione.CertificatoAltraPensione.ToString();
                this.txtBase.Text = datiAgoAltraPensione.BaseAltraPensione.ToString();
                this.txtTipoLiquidazione.Text = datiAgoAltraPensione.TipoLiquidazione.ToString();
                this.txtDecorrenza.Text = datiAgoAltraPensione.DecorrenzaAltraPensione != null ? datiAgoAltraPensione.DecorrenzaAltraPensione.Value.ToString("MM/yyyy") : string.Empty;
                this.txtRmsImp.Text = datiAgoAltraPensione.RmsImpAltraPensione.ToString();
                this.txtSetAnzTot.Text = datiAgoAltraPensione.SetAnzTotAltraPensione.ToString();
                this.txtRev.Text = datiAgoAltraPensione.RevAltraPensione.ToString();
                //supplementi
                this.txtDecorrenzaPrimoSupp.Text = datiAgoAltraPensione.DecorrenzaPrimoSupplemento.HasValue ? datiAgoAltraPensione.DecorrenzaPrimoSupplemento.Value.ToString("MM/yyyy") : string.Empty;
                this.txtImportoContribPrimoSupp.Text = datiAgoAltraPensione.ImpContribPrimoSupplemento.ToString();
                this.txtDecorrenzaSecondoSupp.Text = datiAgoAltraPensione.DecorrenzaSecondoSupplemento.HasValue ? datiAgoAltraPensione.DecorrenzaSecondoSupplemento.Value.ToString("MM/yyyy") : string.Empty;
                this.txtImportoContribSecondoSupp.Text = datiAgoAltraPensione.ImpContribSecondoSupplemento.ToString();
            }
        }

        public Presenter.SvrLiquidazioneFs.GestioneContribDatiAgoAltraPensione RecuperaCampi()
        {
            Presenter.SvrLiquidazioneFs.GestioneContribDatiAgoAltraPensione entity = new Presenter.SvrLiquidazioneFs.GestioneContribDatiAgoAltraPensione();
            if (!string.IsNullOrEmpty(this.ddlCategoria.Text))
                entity.CategoriaAltraPensione = this.ddlCategoria.SelectedValue;
            if (!string.IsNullOrEmpty(this.txtCertificato.Text))
                entity.CertificatoAltraPensione = int.Parse(this.txtCertificato.Text);
            if (!string.IsNullOrEmpty(this.txtBase.Text))
                entity.BaseAltraPensione = decimal.Parse(this.txtBase.Text);
            if (!string.IsNullOrEmpty(this.txtTipoLiquidazione.Text))
                entity.TipoLiquidazione = byte.Parse(this.txtTipoLiquidazione.Text);
            if(!string.IsNullOrEmpty(this.txtDecorrenza.Text))
                entity.DecorrenzaAltraPensione = DateTime.Parse(this.txtDecorrenza.Text);
            if (!string.IsNullOrEmpty(this.txtRmsImp.Text))
                entity.RmsImpAltraPensione = decimal.Parse(this.txtRmsImp.Text);
            if (!string.IsNullOrEmpty(this.txtSetAnzTot.Text))
                entity.SetAnzTotAltraPensione = short.Parse(this.txtSetAnzTot.Text);
            if (!string.IsNullOrEmpty(this.txtRev.Text))
                entity.RevAltraPensione = short.Parse(this.txtRev.Text);
                //supplementi
            if (!string.IsNullOrEmpty(this.txtDecorrenzaPrimoSupp.Text))
                entity.DecorrenzaPrimoSupplemento = DateTime.Parse(this.txtDecorrenzaPrimoSupp.Text);
            if (!string.IsNullOrEmpty(this.txtImportoContribPrimoSupp.Text))
                entity.ImpContribPrimoSupplemento = decimal.Parse(this.txtImportoContribPrimoSupp.Text);
           if (!string.IsNullOrEmpty(this.txtDecorrenzaSecondoSupp.Text))
               entity.DecorrenzaSecondoSupplemento = DateTime.Parse(this.txtDecorrenzaSecondoSupp.Text);
            if (!string.IsNullOrEmpty(this.txtImportoContribSecondoSupp.Text))
                entity.ImpContribSecondoSupplemento = decimal.Parse(this.txtImportoContribSecondoSupp.Text);
            return entity;
        }

        #region Private Methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }
        #endregion Private Methods

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
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi
    }
}