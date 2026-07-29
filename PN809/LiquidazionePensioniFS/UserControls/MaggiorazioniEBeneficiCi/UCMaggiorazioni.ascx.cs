using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi
{
    public partial class UCMaggiorazioni : CustomBaseUserControl, IMaggiorazioneBeneficiCi, ITitolarePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SalvaMaggiorazioni_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni = GetValoriMaggiorazioni();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaMaggiorazioniCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaMaggiorazioni_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaMaggiorazioniCi(this);

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

        internal void ValorizzaEtichetteMaggiorazioni(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            LoadDdl(maggiorazioneBenefici);

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

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneLegge140.HasValue)
                    txtDecorrenzaMaggiorazioneLegge140.Text = String.Format("{0:MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneLegge140.Value);
                else
                    txtDecorrenzaMaggiorazioneLegge140.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02.HasValue)
                    txtAnniRiduzioneEta.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02.Value.ToString();
                else
                    txtAnniRiduzioneEta.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.CodiceRequisitiLegge50392Art2.HasValue)
                    ddlReqArt2Com3DL50392.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiMaggiorazioni.CodiceRequisitiLegge50392Art2.Value.ToString();
                else
                    ddlReqArt2Com3DL50392.SelectedIndex = 0;
            }
            else
            {
                txtDecorrenza.Text = CodeUtility.GetPrevalDecForExCombattente_Maggiorazione();
                //txtDecorrenza.Text = string.Empty;
                //txtCessazione.Text = string.Empty;
            }

            ManageCessazione();
            ManageAnniRiduzioneEta();
        }

        internal Presenter.SvrLiquidazioneCi.DatiMaggiorazioni GetValoriMaggiorazioni()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiMaggiorazioni = new Presenter.SvrLiquidazioneCi.DatiMaggiorazioni();

            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneSociale = !string.IsNullOrEmpty(txtDecorrenza.Text) ? Utility.GetDateFromString(txtDecorrenza.Text) : (DateTime?)null;
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.CessazioneMaggiorazioneSociale = !string.IsNullOrEmpty(txtCessazione.Text) ? Utility.GetDateFromString(txtCessazione.Text) : (DateTime?)null;
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.DecorrenzaMaggiorazioneLegge140 = !string.IsNullOrEmpty(txtDecorrenzaMaggiorazioneLegge140.Text) ? Utility.GetDateFromString(txtDecorrenzaMaggiorazioneLegge140.Text) : (DateTime?)null;
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.AnniRiduzioneBeneficiArt38Legge02 = !string.IsNullOrEmpty(txtAnniRiduzioneEta.Text) ? short.Parse(txtAnniRiduzioneEta.Text) : (short?)null;
            this.areaMaggiorazioneBenefici.DatiMaggiorazioni.CodiceRequisitiLegge50392Art2 = !string.IsNullOrEmpty(ddlReqArt2Com3DL50392.SelectedValue) ? byte.Parse(ddlReqArt2Com3DL50392.SelectedValue) : (byte?)null;

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

        private void LoadDdl(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            CodeUtility.SetValueDdl(ddlReqArt2Com3DL50392, string.Empty, string.Empty);
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaCodiceRequisitiLegge50392 != null)
                    foreach (Presenter.SvrLiquidazioneCi.CodiceRequisitiLegge50392 codiceRequisitiLegge50392 in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaCodiceRequisitiLegge50392)
                        CodeUtility.SetValueDdl(ddlReqArt2Com3DL50392, codiceRequisitiLegge50392.Descrizione, codiceRequisitiLegge50392.Descrizione, codiceRequisitiLegge50392.Id);
            }
        }

        private void ManageCessazione()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if ((datiPensione != null && datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione) || this.domanda.IsDomandaRiapertura )
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

        private void ManageAnniRiduzioneEta()
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (datiPensione != null && datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Superstiti)
            {
                tdAnniRiduzioneEta.Visible = true;
                tdLblAnniRiduzioneEta.Visible = true;
            }
            else
            {
                tdAnniRiduzioneEta.Visible = false;
                tdLblAnniRiduzioneEta.Visible = false;
            }
        }

        

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        #region IMaggiorazioneBenefici
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion IMaggiorazioneBenefici

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolarePensione
        public AreaTitolare TitolarePensione { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolarePensione
    }
}