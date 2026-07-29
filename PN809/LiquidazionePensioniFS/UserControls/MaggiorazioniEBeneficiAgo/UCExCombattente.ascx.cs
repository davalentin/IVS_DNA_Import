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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo
{
    public partial class UCExCombattente : CustomBaseUserControl, IMaggiorazioneBeneficiAgo
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (IsPostBack)
            {
            }
        }

        protected void SalvaExCombattente_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaMaggiorazioneBenefici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();
            areaMaggiorazioneBenefici.DatiExCombattente = GetValoriExCombattente();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaExCombattenteAgo(this);

            if (!this.HasError)
                ValorizzaEtichetteExCombattente(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaExCombattente_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaExCombattenteAgo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Ex Combattente";
            else
            {
                ClearForm();
                ValorizzaEtichetteExCombattente(this);
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

        internal Presenter.SvrLiquidazioneAgo.DatiExCombattente GetValoriExCombattente()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiExCombattente = new Presenter.SvrLiquidazioneAgo.DatiExCombattente();

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

        internal void ValorizzaEtichetteExCombattente(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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
                {
                    txtDecorrenza.Text = CodeUtility.GetPrevalDecForExCombattente_Maggiorazione();
                }
            }
            else
                txtDecorrenza.Text = CodeUtility.GetPrevalDecForExCombattente_Maggiorazione();
            //else
            //{
            //    //Legge 140
            //    ddlExCombattente.SelectedIndex = 0;
            //    txtDecorrenza.Text = string.Empty;
            //}

            GestioneENPALS(maggiorazioneBenefici);

            if (Utility.IsDomandaAUT(this.domanda.Categoria))
            {
                ddlExCombattente.SelectedValue = "8";
                ddlExCombattente.Enabled = false;
            }
        }

     
        private void LoadDdl(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {               
                //Legge 140
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaCodiceCieco != null)
                {
                    ddlExCombattente.Items.Clear();
                    foreach (Presenter.SvrLiquidazioneAgo.CodiceCieco codicecieco in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaCodiceCieco)
                        CodeUtility.SetValueDdl(ddlExCombattente, codicecieco.Id + " - " + codicecieco.Descrizione, codicecieco.Descrizione, codicecieco.Id);
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
            txtDecorrenza.Text = "MM/AAAA";
        }

        private void GestioneENPALS(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaENPALS)
            {
                pnlENPALS.Visible = true;
                if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente != null)
                {
                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.NumeroContributiNLNonVedenti.HasValue)
                        txtNumeroContributiNLNonVedenti.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.NumeroContributiNLNonVedenti.ToString();
                }
            }
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IMaggiorazioneBenefici
    }
}