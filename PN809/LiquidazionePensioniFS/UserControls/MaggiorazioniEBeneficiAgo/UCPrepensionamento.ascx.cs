using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo
{
    public partial class UCPrepensionamento : CustomBaseUserControl, IMaggiorazioneBeneficiAgo
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaPrepensionamento_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiPrepensionamento = GetValoriPrepensionamento();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaPrepensionamentoAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaPrepensionamento_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaPrepensionamentoAgo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Prepensionamento";
            else
            {
                ClearForm();
                ValorizzaEtichettePrepensionamento(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        internal void ValorizzaEtichettePrepensionamento(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            RenderControls(maggiorazioneBenefici);

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CodiceLegge.HasValue)
                    txtCodiceLegge.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CodiceLegge.ToString().PadLeft(4, '0');

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneUtiliDiritto.HasValue)
                    txtSettimaneUtiliDiritto.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneUtiliDiritto.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneUtiliMisura.HasValue)
                    txtSettimaneUtiliMisura.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneUtiliMisura.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneMaggioreAnzianita.HasValue)
                    txtSettimaneMaggioreAnzianita.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneMaggioreAnzianita.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.OnereMancataContribuzione.HasValue)
                    txtOnereMancataContribuzione.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.OnereMancataContribuzione.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CodiceAzienda.HasValue)
                    txtCodiceAzienda.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CodiceAzienda.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CessazioneBeneficioPrepensionamento.HasValue)
                    txtCessazioneBeneficioPrepensionamento.Text = String.Format("{0:MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CessazioneBeneficioPrepensionamento.Value);
 
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneAmianto.HasValue)
                    txtSettimaneAmianto.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneAmianto.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CessazioneAmianto.HasValue)
                    txtCessazioneAmianto.Text = String.Format("{0:MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiPrepensionamento.CessazioneAmianto.Value);  
            }
        }

        internal Presenter.SvrLiquidazioneAgo.DatiPrepensionamento GetValoriPrepensionamento()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiPrepensionamento == null)
                this.areaMaggiorazioneBenefici.DatiPrepensionamento = new DatiPrepensionamento();

            if (!string.IsNullOrEmpty(txtCodiceLegge.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.CodiceLegge = int.Parse(txtCodiceLegge.Text);

            if (!string.IsNullOrEmpty(txtSettimaneUtiliDiritto.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneUtiliDiritto = int.Parse(txtSettimaneUtiliDiritto.Text);

            if (!string.IsNullOrEmpty(txtSettimaneUtiliMisura.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneUtiliMisura = int.Parse(txtSettimaneUtiliMisura.Text);

            if (!string.IsNullOrEmpty(txtSettimaneMaggioreAnzianita.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneMaggioreAnzianita = int.Parse(txtSettimaneMaggioreAnzianita.Text);

            if (!string.IsNullOrEmpty(txtOnereMancataContribuzione.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.OnereMancataContribuzione = decimal.Parse(txtOnereMancataContribuzione.Text);

            if (!string.IsNullOrEmpty(txtCodiceAzienda.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.CodiceAzienda = long.Parse(txtCodiceAzienda.Text);

            if (!string.IsNullOrEmpty(txtCessazioneBeneficioPrepensionamento.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.CessazioneBeneficioPrepensionamento = Utility.ConvertString2Data_MMAAAA(txtCessazioneBeneficioPrepensionamento.Text);

            if (!string.IsNullOrEmpty(txtSettimaneAmianto.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.SettimaneAmianto = int.Parse(txtSettimaneAmianto.Text);

            if (!string.IsNullOrEmpty(txtCessazioneAmianto.Text))
                areaMaggiorazioneBenefici.DatiPrepensionamento.CessazioneAmianto = Utility.ConvertString2Data_MMAAAA(txtCessazioneAmianto.Text);

            return this.areaMaggiorazioneBenefici.DatiPrepensionamento;
        }

        #region private methods
        private void RenderControls(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioAmianto.HasValue)
                pnlAmianto.Visible = maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioAmianto.Value;
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {

        }
        #endregion private methods

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }
        #endregion Events

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion
    }
}