using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri
{
    public partial class UCPrepensionamento : CustomBaseUserControl, IOneri
    {

        #region private methods
        private void RenderControls(IOneri oneri)
        {
            if (oneri != null && oneri.areaOneri != null)
                pnlAmianto.Visible = oneri.areaOneri.IsBeneficioAmianto;
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
        #endregion IViewUI

        #region IOneri
        public Presenter.SvrLiquidazione.AreaOneri areaOneri { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IOneri

        protected void Page_Load(object sender, EventArgs e)
        {
            if(this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaPrepensionamento_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaOneri = new Presenter.SvrLiquidazione.AreaOneri();
            this.areaOneri.DatiPrepensionamento = GetValoriPrepensionamento();

            PresenterOneri presenterOneri = new PresenterOneri();
            presenterOneri.SalvaPrepensionamento(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaPrepensionamento_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterOneri presenterOneri = new PresenterOneri();
            presenterOneri.EliminaPrepensionamento(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Prepensionamento";
            else
            {
                ClearForm();
                ValorizzaEtichettePrepensionamento(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        internal void ValorizzaEtichettePrepensionamento(IOneri oneri)
        {
            RenderControls(oneri);

            if (oneri != null && oneri.areaOneri != null && oneri.areaOneri.DatiPrepensionamento != null)
            {
                if (oneri.areaOneri.DatiPrepensionamento.CodiceLegge.HasValue)
                    txtCodiceLegge.Text = oneri.areaOneri.DatiPrepensionamento.CodiceLegge.ToString().PadLeft(4, '0');

                if (oneri.areaOneri.DatiPrepensionamento.SettimaneUtiliDiritto.HasValue)
                    txtSettimaneUtiliDiritto.Text = oneri.areaOneri.DatiPrepensionamento.SettimaneUtiliDiritto.ToString();

                if (oneri.areaOneri.DatiPrepensionamento.SettimaneUtiliMisura.HasValue)
                    txtSettimaneUtiliMisura.Text = oneri.areaOneri.DatiPrepensionamento.SettimaneUtiliMisura.ToString();

                if (oneri.areaOneri.DatiPrepensionamento.SettimaneMaggioreAnzianita.HasValue)
                    txtSettimaneMaggioreAnzianita.Text = oneri.areaOneri.DatiPrepensionamento.SettimaneMaggioreAnzianita.ToString();

                if (oneri.areaOneri.DatiPrepensionamento.OnereMancataContribuzione.HasValue)
                    txtOnereMancataContribuzione.Text = oneri.areaOneri.DatiPrepensionamento.OnereMancataContribuzione.ToString();

                if (oneri.areaOneri.DatiPrepensionamento.CodiceAzienda.HasValue)
                    txtCodiceAzienda.Text = oneri.areaOneri.DatiPrepensionamento.CodiceAzienda.ToString();

                if (oneri.areaOneri.DatiPrepensionamento.CessazioneBeneficioPrepensionamento.HasValue)
                    txtCessazioneBeneficioPrepensionamento.Text = String.Format("{0:MM/yyyy}", oneri.areaOneri.DatiPrepensionamento.CessazioneBeneficioPrepensionamento.Value);

                if (oneri.areaOneri.DatiPrepensionamento.SettimaneAmianto.HasValue)
                    txtSettimaneAmianto.Text = oneri.areaOneri.DatiPrepensionamento.SettimaneAmianto.ToString();

                if (oneri.areaOneri.DatiPrepensionamento.CessazioneAmianto.HasValue)
                    txtCessazioneAmianto.Text = String.Format("{0:MM/yyyy}", oneri.areaOneri.DatiPrepensionamento.CessazioneAmianto.Value);  
            }
        }

        internal Presenter.SvrLiquidazione.DatiPrepensionamento GetValoriPrepensionamento()
        {
            if (this.areaOneri == null)
                this.areaOneri = new AreaOneri();

            if (this.areaOneri.DatiPrepensionamento == null)
                this.areaOneri.DatiPrepensionamento = new DatiPrepensionamento();

            if (!string.IsNullOrEmpty(txtCodiceLegge.Text))
                areaOneri.DatiPrepensionamento.CodiceLegge = int.Parse(txtCodiceLegge.Text);

            if (!string.IsNullOrEmpty(txtSettimaneUtiliDiritto.Text))
                areaOneri.DatiPrepensionamento.SettimaneUtiliDiritto = int.Parse(txtSettimaneUtiliDiritto.Text);

            if (!string.IsNullOrEmpty(txtSettimaneUtiliMisura.Text))
                areaOneri.DatiPrepensionamento.SettimaneUtiliMisura = int.Parse(txtSettimaneUtiliMisura.Text);

            if (!string.IsNullOrEmpty(txtSettimaneMaggioreAnzianita.Text))
                areaOneri.DatiPrepensionamento.SettimaneMaggioreAnzianita = int.Parse(txtSettimaneMaggioreAnzianita.Text);

            if (!string.IsNullOrEmpty(txtOnereMancataContribuzione.Text))
                areaOneri.DatiPrepensionamento.OnereMancataContribuzione = decimal.Parse(txtOnereMancataContribuzione.Text);

            if (!string.IsNullOrEmpty(txtCodiceAzienda.Text))
                areaOneri.DatiPrepensionamento.CodiceAzienda = long.Parse(txtCodiceAzienda.Text);

            if (!string.IsNullOrEmpty(txtCessazioneBeneficioPrepensionamento.Text))
                areaOneri.DatiPrepensionamento.CessazioneBeneficioPrepensionamento = Utility.GetDateFromString(txtCessazioneBeneficioPrepensionamento.Text);

            if (!string.IsNullOrEmpty(txtSettimaneAmianto.Text))
                areaOneri.DatiPrepensionamento.SettimaneAmianto = int.Parse(txtSettimaneAmianto.Text);

            if (!string.IsNullOrEmpty(txtCessazioneAmianto.Text))
                areaOneri.DatiPrepensionamento.CessazioneAmianto = Utility.GetDateFromString(txtCessazioneAmianto.Text);

            return this.areaOneri.DatiPrepensionamento;
        }

     
    }
}