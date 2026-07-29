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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi
{
    public partial class UCExCombattente : CustomBaseUserControl, IMaggiorazioneBeneficiCi
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
            }
        }

        protected void SalvaExCombattente_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaMaggiorazioneBenefici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();
            areaMaggiorazioneBenefici.DatiExCombattente = GetValoriExCombattente();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaExCombattenteCi(this);

            if (!this.HasError)
                ValorizzaEtichetteExCombattente(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaExCombattente_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaExCombattenteCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Ex Combattente";
            }
            else
            {
                ValorizzaEtichetteExCombattente(null);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        internal Presenter.SvrLiquidazioneCi.DatiExCombattente GetValoriExCombattente()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiExCombattente = new Presenter.SvrLiquidazioneCi.DatiExCombattente();

            //Legge 140
            if (string.IsNullOrEmpty(ddlExCombattente.SelectedValue))
                this.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco = null;
            else
                this.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco = byte.Parse(ddlExCombattente.SelectedValue);

            if (string.IsNullOrEmpty(txtDecorrenza.Text) || txtDecorrenza.Text.Equals("MM/AAAA"))
                this.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6 = null;
            else
                this.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6 = Utility.GetDateFromString(txtDecorrenza.Text);

            return this.areaMaggiorazioneBenefici.DatiExCombattente;
        }

        internal void ValorizzaEtichetteExCombattente(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            LoadDdl(maggiorazioneBenefici);

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente != null)
            {
                //Legge 140
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco.HasValue ||
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue)
                {
                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco.HasValue)
                        ddlExCombattente.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco.Value.ToString();
                    else
                        ddlExCombattente.SelectedIndex = 0;
                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue)
                        txtDecorrenza.Text = String.Format("{0:MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6.Value);
                }
                else
                    txtDecorrenza.Text = CodeUtility.GetPrevalDecForExCombattente_Maggiorazione();

            }
            else
            {
                txtDecorrenza.Text = CodeUtility.GetPrevalDecForExCombattente_Maggiorazione();
                //txtDecorrenza.Text = CodeUtility.GetP
                //Legge 140
                //ddlExCombattente.SelectedIndex = 0;
                //txtDecorrenza.Text = "MM/AAAA";
            }
        }
        
        private void LoadDdl(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                //Legge 140
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaCodiceCieco != null)
                    foreach (Presenter.SvrLiquidazioneCi.CodiceCieco codicecieco in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaCodiceCieco)
                        CodeUtility.SetValueDdl(ddlExCombattente, codicecieco.Id + " - " + codicecieco.Descrizione, codicecieco.Descrizione, codicecieco.Id);
            }
        }

 
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion IMaggiorazioneBenefici
    }
}