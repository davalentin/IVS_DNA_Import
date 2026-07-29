using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici
{
    public partial class UCPrivilegiate : CustomBaseUserControl, IMaggiorazioneBenefici
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IMaggiorazioneBenefici


        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalvaPrivilegiate_Click(object sender, EventArgs e)
        {
            areaMaggiorazioneBenefici = new AreaMaggiorazioniBenefici();
            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            areaMaggiorazioneBenefici.DatiPrivilegiate = GetValoriPrivilegiate();
            presenterMaggiorazioneBenefici.SalvaTabDatiPrivilegiate(this);
            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaPrivilegiate_Click(object sender, EventArgs e)
        {
            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaTabDatiPrivilegiate(this);

            if (!this.HasError)
            {
                ClearForm();
                ValorizzaEtichettePrivilegiate(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        internal void ValorizzaEtichettePrivilegiate(IMaggiorazioneBenefici maggBen)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaMaggiorazioneBenefici = maggBen.areaMaggiorazioneBenefici;

            LoadDdlWithFondo();
            if (this.areaMaggiorazioneBenefici.DatiPrivilegiate != null)
            {
                this.ddlAssegnoCura.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.AssegnoCura.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.AssegnoCura.Value.ToString() : string.Empty;
                this.ddlAssegnoIntegrativo.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.AssegnoIntegrativo.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.AssegnoIntegrativo.Value.ToString() : string.Empty;
                this.ddlCumulo.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.CumuloInfermita.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.CumuloInfermita.Value.ToString() : string.Empty;
                this.ddlIndennitaAccompagno.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.Value.ToString() : string.Empty;
                this.ddlIndennitaSpeciale.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.IndennitaSpecialeAnnua.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.IndennitaSpecialeAnnua.Value.ToString() : string.Empty;
                this.ddlInfermita.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.Categoria2aInfermita.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.Categoria2aInfermita.Value.ToString() : string.Empty;
                this.ddlIntegrazioneIndennita.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.IntegrazioneIndennitaAssistenza.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.IntegrazioneIndennitaAssistenza.Value.ToString() : string.Empty;
                this.ddlInvalidita.SelectedValue = this.areaMaggiorazioneBenefici.DatiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.Value.ToString() : string.Empty;
            }
        }

        private void LoadDdlWithFondo()
        {
            ddlAssegnoCura.Items.Clear();
            ddlAssegnoIntegrativo.Items.Clear();
            ddlCumulo.Items.Clear();
            ddlIndennitaAccompagno.Items.Clear();
            ddlIndennitaSpeciale.Items.Clear();
            ddlInfermita.Items.Clear();
            ddlIntegrazioneIndennita.Items.Clear();
            ddlInvalidita.Items.Clear();

            if (this.areaMaggiorazioneBenefici.ListaCodicePensioniPrivilegiate != null && this.areaMaggiorazioneBenefici.ListaCodicePensioniPrivilegiate.Count() > 0 && this.domanda.Tipofondo.HasValue)
            {
                List<CodicePensioniPrivilegiate> ListaCodicePensioniPrivilegiate = this.areaMaggiorazioneBenefici.ListaCodicePensioniPrivilegiate.ToList().FindAll(x => x.Fondo == this.domanda.Tipofondo.Value.ToString());

                foreach (CodicePensioniPrivilegiate codicePensioniPrivilegiate in ListaCodicePensioniPrivilegiate)
                {
                    if (codicePensioniPrivilegiate.Posizione == 1)
                        CodeUtility.SetValueDdl(ddlInvalidita, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 2)
                        CodeUtility.SetValueDdl(ddlAssegnoIntegrativo, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 3)
                        CodeUtility.SetValueDdl(ddlIntegrazioneIndennita, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 4)
                        CodeUtility.SetValueDdl(ddlIndennitaAccompagno, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 5)
                        CodeUtility.SetValueDdl(ddlCumulo, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 6)
                        CodeUtility.SetValueDdl(ddlInfermita, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 7)
                        CodeUtility.SetValueDdl(ddlAssegnoCura, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    else if (codicePensioniPrivilegiate.Posizione == 8)
                        CodeUtility.SetValueDdl(ddlIndennitaSpeciale, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                }
            }
        }

        internal Presenter.SvrLiquidazioneFs.DatiPrivilegiate GetValoriPrivilegiate()
        {
            AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            areaMaggiorazioniBenefici.DatiPrivilegiate = new DatiPrivilegiate();
            areaMaggiorazioniBenefici.DatiPrivilegiate.AssegnoCura                          = !String.IsNullOrEmpty(ddlAssegnoCura.SelectedValue) ? Convert.ToInt32(ddlAssegnoCura.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.AssegnoIntegrativo                   = !String.IsNullOrEmpty(ddlAssegnoIntegrativo.SelectedValue) ? Convert.ToInt32(ddlAssegnoIntegrativo.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.Categoria2aInfermita                 = !String.IsNullOrEmpty(ddlInfermita.SelectedValue) ? Convert.ToInt32(ddlInfermita.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.CumuloInfermita                      = !String.IsNullOrEmpty(ddlCumulo.SelectedValue) ? Convert.ToInt32(ddlCumulo.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.IndennitaAccompagnamentoAggiuntiva   = !String.IsNullOrEmpty(ddlIndennitaAccompagno.SelectedValue) ? Convert.ToInt32(ddlIndennitaAccompagno.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.IndennitaSpecialeAnnua               = !String.IsNullOrEmpty(ddlIndennitaSpeciale.SelectedValue) ? Convert.ToInt32(ddlIndennitaSpeciale.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.IntegrazioneIndennitaAssistenza      = !String.IsNullOrEmpty(ddlIntegrazioneIndennita.SelectedValue) ? Convert.ToInt32(ddlIntegrazioneIndennita.SelectedValue) : (int?)null;
            areaMaggiorazioniBenefici.DatiPrivilegiate.PrivilegiataSuperinvaliditaIndennita = !String.IsNullOrEmpty(ddlInvalidita.SelectedValue) ? Convert.ToInt32(ddlInvalidita.SelectedValue) : (int?)null;
            return areaMaggiorazioniBenefici.DatiPrivilegiate;
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }
    }
}