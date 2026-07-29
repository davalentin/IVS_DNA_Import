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
    public partial class UCArticolo2 : CustomBaseUserControl, IMaggiorazioneBenefici
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

        protected void btnSalvaArt2_Click(object sender, EventArgs e)
        {
            areaMaggiorazioneBenefici = new AreaMaggiorazioniBenefici();
            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            areaMaggiorazioneBenefici.DatiArticolo2 = GetValoriDatiArticolo2();
            presenterMaggiorazioneBenefici.SalvaTabDatiArticolo2(this);

            RaiseShowAvviso(this, null);
        }

        internal Presenter.SvrLiquidazioneFs.DatiArticolo2 GetValoriDatiArticolo2()
        {
            AreaMaggiorazioniBenefici areaMaggiorazioniBenefici = new AreaMaggiorazioniBenefici();
            areaMaggiorazioniBenefici.DatiArticolo2 = new DatiArticolo2();
            areaMaggiorazioniBenefici.DatiArticolo2.DataInzioBeneficioArt2 = !String.IsNullOrEmpty(txtDataInizioBeneficio.Text) && !String.Equals(txtDataInizioBeneficio.Text.ToUpperInvariant(), "GG/MM/AAAA") ? Utility.GetDateFromString(txtDataInizioBeneficio.Text) : (DateTime?)null;
            areaMaggiorazioniBenefici.DatiArticolo2.DataFineBeneficioArt2  = !String.IsNullOrEmpty(txtDataInizioBeneficio.Text) && !String.Equals(txtDataFineBeneficio.Text.ToUpperInvariant(), "GG/MM/AAAA") ? Utility.GetDateFromString(txtDataFineBeneficio.Text) : (DateTime?)null;
            return areaMaggiorazioniBenefici.DatiArticolo2;
        }

        protected void btnEliminaArt2_Click(object sender, EventArgs e)
        {
            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaTabDatiArticolo2(this);

            if (!this.HasError)
            {
                ClearForm();
                ValorizzaEtichetteArticolo2(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {

        }

        internal void ValorizzaEtichetteArticolo2(IMaggiorazioneBenefici maggBen)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaMaggiorazioneBenefici = maggBen.areaMaggiorazioneBenefici;
            if (this.areaMaggiorazioneBenefici.DatiArticolo2 != null)
            {
                txtDataInizioBeneficio.Text = this.areaMaggiorazioneBenefici.DatiArticolo2.DataInzioBeneficioArt2.HasValue ? String.Format("{0:dd/MM/yyyy}", this.areaMaggiorazioneBenefici.DatiArticolo2.DataInzioBeneficioArt2.Value) : string.Empty;
                txtDataFineBeneficio.Text   = this.areaMaggiorazioneBenefici.DatiArticolo2.DataFineBeneficioArt2.HasValue ? String.Format("{0:dd/MM/yyyy}", this.areaMaggiorazioneBenefici.DatiArticolo2.DataFineBeneficioArt2.Value) : string.Empty;
            }
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