using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCSL336_ES : CustomBaseUserControl, IDatiContributivi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiSL336(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnElimina_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiSL336(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati S.L. 336";
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

            Presenter.SvrLiquidazioneFs.GestioneContribDatiSL33670 datiSL336 = areaDatiContributivi.DatiSL336;

            switch (domanda.Tipofondo)
            {

                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:

                    if (areaDatiContributivi != null && datiSL336 != null)
                    {
                        if (datiSL336.RMSSenzaLegge33670QA.HasValue)
                            txtQuotaA.Text = datiSL336.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (datiSL336.NSettimaneAnzianitaTotaliSenzaLegge33670.HasValue)
                            txtSettAnzTot.Text = datiSL336.NSettimaneAnzianitaTotaliSenzaLegge33670.Value.ToString();
                        if (datiSL336.NSettimaneSenzaLegge33670Art24QuotaA.HasValue)
                            txtSettArt24.Text = datiSL336.NSettimaneSenzaLegge33670Art24QuotaA.Value.ToString();
                        if (datiSL336.NSettimaneSenzaLegge33670Art57QuotaA.HasValue)
                            txtSettArt57.Text = datiSL336.NSettimaneSenzaLegge33670Art57QuotaA.Value.ToString();

                        if (datiSL336.RMSSenzaLegge33670QB.HasValue)
                            txtQuotaB.Text = datiSL336.RMSSenzaLegge33670QB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                        if (datiSL336.ContributiTotaliSenzaLegge33670.HasValue)
                            txtContributiTotali.Text = datiSL336.ContributiTotaliSenzaLegge33670.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (datiSL336.CCArt14SenzaLegge33670.HasValue)
                            txtContributiArt14.Text = datiSL336.CCArt14SenzaLegge33670.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (datiSL336.ContributiSupplementoAgo.HasValue)
                            txtContributiAGO.Text = datiSL336.ContributiSupplementoAgo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (datiSL336.ContributiSupplementoFondo.HasValue)
                            txtSupplementoFondo.Text = datiSL336.ContributiSupplementoFondo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    }
                    break;
            }
        }

        private void RenderControls()
        {
            throw new NotImplementedException();
        }

        public void RecuperaCampi()
        {
            if (areaDatiContributivi == null)
                areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (domanda.Tipofondo)
            {

                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    if (areaDatiContributivi.DatiSL336 == null)
                        areaDatiContributivi.DatiSL336 = new GestioneContribDatiSL33670();
                    GestioneContribDatiSL33670 entity = areaDatiContributivi.DatiSL336;
                    if (!string.IsNullOrEmpty(txtQuotaA.Text))
                        entity.RMSSenzaLegge33670QA = decimal.Parse(txtQuotaA.Text);
                    if (!string.IsNullOrEmpty(txtSettAnzTot.Text))
                        entity.NSettimaneAnzianitaTotaliSenzaLegge33670 = int.Parse(txtSettAnzTot.Text);
                    if (!string.IsNullOrEmpty(txtSettArt24.Text))
                        entity.NSettimaneSenzaLegge33670Art24QuotaA = int.Parse(txtSettArt24.Text);
                    if (!string.IsNullOrEmpty(txtSettArt57.Text))
                        entity.NSettimaneSenzaLegge33670Art57QuotaA = int.Parse(txtSettArt57.Text);

                    if (!string.IsNullOrEmpty(txtQuotaB.Text))
                        entity.RMSSenzaLegge33670QB = decimal.Parse(txtQuotaB.Text);

                    if (!string.IsNullOrEmpty(txtContributiTotali.Text))
                        entity.ContributiTotaliSenzaLegge33670 = decimal.Parse(txtContributiTotali.Text);
                    if (!string.IsNullOrEmpty(txtContributiArt14.Text))
                        entity.CCArt14SenzaLegge33670 = decimal.Parse(txtContributiArt14.Text);
                    if (!string.IsNullOrEmpty(txtContributiAGO.Text))
                        entity.ContributiSupplementoAgo = decimal.Parse(txtContributiAGO.Text);
                    if (!string.IsNullOrEmpty(txtSupplementoFondo.Text))
                        entity.ContributiSupplementoFondo = decimal.Parse(txtSupplementoFondo.Text);

                    break;
            }
        }



        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);

        }

    }
}