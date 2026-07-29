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
    public partial class UCVittimeCi : CustomBaseUserControl, IMaggiorazioneBeneficiCi, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBeneficiCi
        public Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion IMaggiorazioneBeneficiCi

        #region ITitolarePensione
        public AreaTitolare TitolarePensione { get; set; }

        #endregion ITitolarePensione

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SalvaVittime_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo = RecuperCampi();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaVittimeCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaVittime_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaVittimeCi(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Vittime";
            else
            {
                ClearForm();
                ValorizzaEtichette(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        internal void ValorizzaEtichette(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            LoadDdl(maggiorazioneBenefici);

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario.HasValue)
                    ddlSoggettoBeneficiario.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.SoggettoBeneficiario.Value.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.CodiceEvento.HasValue)
                    ddlCodiceEvento.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.CodiceEvento.Value.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.HasValue)
                    txtDataEventoTerroristico.Text = string.Format("{0:dd/MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.DataEventoTerroristico.Value);

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione.HasValue)
                    ddlTipologiaPrestazione.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.TipologiaPrestazione.Value.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio.HasValue)
                    ddlTipologiaBeneficioTerrorismo.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBeneficioVittimeTerrorismo.TipologiaBeneficio.Value.ToString();
            }

            GestionePrevalorizzazioneEtichette();
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioVittimeTerrorismo.GetValueOrDefault())
            {
                pnlVittime.Enabled = false;
                btnEliminaVittime.Enabled = false;
            }
        }

        internal Presenter.SvrLiquidazioneCi.DatiBeneficioVittimeTerrorismo RecuperCampi()
        {
            Presenter.SvrLiquidazioneCi.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = new Presenter.SvrLiquidazioneCi.DatiBeneficioVittimeTerrorismo();

            if (!string.IsNullOrEmpty(ddlSoggettoBeneficiario.SelectedValue))
                datiBeneficioVittimeTerrorismo.SoggettoBeneficiario = long.Parse(ddlSoggettoBeneficiario.SelectedValue);

            if (!string.IsNullOrEmpty(ddlCodiceEvento.SelectedValue))
                datiBeneficioVittimeTerrorismo.CodiceEvento = char.Parse(ddlCodiceEvento.SelectedValue);

            if (!string.IsNullOrEmpty(txtDataEventoTerroristico.Text))
                datiBeneficioVittimeTerrorismo.DataEventoTerroristico = Utility.GetDateFromString(txtDataEventoTerroristico.Text);

            if (!string.IsNullOrEmpty(ddlTipologiaPrestazione.SelectedValue))
                datiBeneficioVittimeTerrorismo.TipologiaPrestazione = long.Parse(ddlTipologiaPrestazione.SelectedValue);

            if (!string.IsNullOrEmpty(ddlTipologiaBeneficioTerrorismo.SelectedValue))
                datiBeneficioVittimeTerrorismo.TipologiaBeneficio = long.Parse(ddlTipologiaBeneficioTerrorismo.SelectedValue);

            return datiBeneficioVittimeTerrorismo;
        }

        #region private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            txtDataEventoTerroristico.Text = "GG/MM/AAAA";
        }

        private void LoadDdl(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaSoggettoBeneficiario != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaSoggettoBeneficiario.Count() > 0)
                {
                    ddlSoggettoBeneficiario.Items.Clear();
                    CodeUtility.SetItemBlankDdl(ddlSoggettoBeneficiario);
                    foreach (Presenter.SvrLiquidazioneCi.SoggettoBeneficiario soggettoBeneficiario in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaSoggettoBeneficiario)
                        CodeUtility.SetValueDdl(ddlSoggettoBeneficiario, soggettoBeneficiario.Descrizione, soggettoBeneficiario.Descrizione, soggettoBeneficiario.Id.ToString());
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipologiaPrestazione != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipologiaPrestazione.Count() > 0)
                {
                    ddlTipologiaPrestazione.Items.Clear();
                    CodeUtility.SetItemBlankDdl(ddlTipologiaPrestazione);
                    foreach (Presenter.SvrLiquidazioneCi.TipologiaPrestazione tipologiaPrestazione in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipologiaPrestazione)
                    {
                        if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || (!Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) && tipologiaPrestazione.Id != 4))
                            CodeUtility.SetValueDdl(ddlTipologiaPrestazione, tipologiaPrestazione.Descrizione, tipologiaPrestazione.Descrizione, tipologiaPrestazione.Id.ToString());
                    }
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipologiaBeneficioTerrorismo != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipologiaBeneficioTerrorismo.Count() > 0)
                {
                    ddlTipologiaBeneficioTerrorismo.Items.Clear();
                    CodeUtility.SetItemBlankDdl(ddlTipologiaBeneficioTerrorismo);
                    foreach (Presenter.SvrLiquidazioneCi.TipologiaBeneficioTerrorismo tipologiaBeneficioTerrorismo in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipologiaBeneficioTerrorismo)
                        CodeUtility.SetValueDdl(ddlTipologiaBeneficioTerrorismo, tipologiaBeneficioTerrorismo.Descrizione, tipologiaBeneficioTerrorismo.Descrizione, tipologiaBeneficioTerrorismo.Id.ToString());
                }
            }
        }

        private void GestionePrevalorizzazioneEtichette()
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80)
            {
                // Vittima con invalidità => 80% 
                ddlSoggettoBeneficiario.SelectedValue = "1";
                ddlSoggettoBeneficiario.Enabled = false;
                // art. 4  L. 206/2004
                ddlTipologiaPrestazione.SelectedValue = "1";
                ddlTipologiaPrestazione.Enabled = false;
                // benefici art. 4 con decorrenza anche dal 1° gennaio 2008
                ddlTipologiaBeneficioTerrorismo.SelectedValue = "7";
                ddlTipologiaBeneficioTerrorismo.Enabled = false;
            }

            if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                trSoggettoBeneficiario.Visible = false;
                trTipologiaDelBeneficio.Visible = false;
                trCodiceEvento.Visible = false;
                ddlTipologiaPrestazione.SelectedValue = "4";
                ddlTipologiaPrestazione.Enabled = false;
                btnSalva.Enabled = false;
                btnEliminaVittime.Enabled = false;
            }
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
    }
}