using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo
{
    public partial class UCMaggiorazioni : CustomBaseUserControl, IMaggiorazioneBeneficiAgo, ITitolarePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaMaggiorazioni_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni = GetValoriMaggiorazioni();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaMaggiorazioniAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaMaggiorazioni_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaMaggiorazioniAgo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Maggiorazioni";
            else
            {
                ClearForm();
                ValorizzaEtichetteMaggiorazioni(this);
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

        internal void ValorizzaEtichetteMaggiorazioni(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RenderControls(maggiorazioneBenefici);

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneSociale.HasValue)
                    txtDecorrenza.Text = String.Format("{0:MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneSociale.Value);
                else
                    txtDecorrenza.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.CessazioneMaggiorazioneSociale.HasValue)
                    txtCessazione.Text = String.Format("{0:MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.CessazioneMaggiorazioneSociale.Value);
                else
                    txtCessazione.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02.HasValue)
                    txtAARidbenArt38.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02.Value.ToString();
                else
                    txtAARidbenArt38.Text = string.Empty;
            }
            else if (!Utility.IsDomandaAUT(this.domanda.Categoria) && !Utility.IsDomandaSPED(this.domanda.Categoria))
                txtDecorrenza.Text = CodeUtility.GetPrevalDecForExCombattente_Maggiorazione();

            //else
            //{
            //    txtDecorrenza.Text = string.Empty;
            //    txtCessazione.Text = string.Empty;
            //    txtAARidbenArt38.Text = string.Empty;
            //}

            ManageCessazione();
        }

        internal Presenter.SvrLiquidazioneAgo.DatiMaggiorazioni GetValoriMaggiorazioni()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiMaggiorazioni = new Presenter.SvrLiquidazioneAgo.DatiMaggiorazioni();

            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneSociale = !string.IsNullOrEmpty(txtDecorrenza.Text) ? Utility.GetDateFromString(txtDecorrenza.Text) : (DateTime?)null;
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.CessazioneMaggiorazioneSociale = !string.IsNullOrEmpty(txtCessazione.Text) ? Utility.GetDateFromString(txtCessazione.Text) : (DateTime?)null;
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02 = !string.IsNullOrEmpty(txtAARidbenArt38.Text) ? short.Parse(txtAARidbenArt38.Text) : (short?)null;

            return this.areaMaggiorazioneBenefici.DatiMaggiorazioni;
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            txtDecorrenza.Text = "MM/AAAA";
            txtCessazione.Text = "MM/AAAA";
        }

        private void ManageCessazione()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if ((datiPensione != null && datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione) || this.domanda.IsDomandaRiapertura)
            {
                tdCessazione.Visible = true;
                tdLblCessazione.Visible = true;
            }
            else
            {
                tdCessazione.Visible = false;
                tdLblCessazione.Visible = false;
            }
        }

        private void RenderControls(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsVisiblePerSuperstitiOrPMO.GetValueOrDefault())
                pnlAnniRid.Visible = true;

            DateTime? decMaggiorazioneStorico = maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioneBeneficiStorico != null ?
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioneBeneficiStorico.DecorrenzaMaggiorazioneSociale : null;

            if (!CodeUtility.IsRicostituzione(datiPensione))
            {
                if (decMaggiorazioneStorico != null)
                {
                    DisableDecorrenza(txtDecorrenza, decMaggiorazioneStorico);
                }

                if (Utility.IsDomandaAUT(this.domanda.Categoria) || Utility.IsDomandaSPED(this.domanda.Categoria))
                {
                    DisableDecorrenza(txtDecorrenza, null);
                    txtAARidbenArt38.Enabled = false;
                }
               
            }

            if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                DisableDecorrenza(txtDecorrenza, null);
                txtAARidbenArt38.Enabled = false;
            }
            if (Utility.IsDomandaRiliquidazione(datiPensione))
            {
                txtDecorrenza.Enabled = true;
            }
        }

        private void DisableDecorrenza(TextBox txtDecorrenza, DateTime? decMaggiorazioneStorico)
        {
            txtDecorrenza.Enabled = false;
            if (decMaggiorazioneStorico != null)
                txtDecorrenza.Text = decMaggiorazioneStorico.ToString();
            else
                txtDecorrenza.Text = string.Empty;
            txtDecorrenza_RF.Enabled = false;
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

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare
    }
}