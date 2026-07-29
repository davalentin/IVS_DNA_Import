using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo
{
    public partial class UCDatiFondo : CustomBaseUserControl, IDatiFondoAgo, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        #endregion IDatiFondo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = this.GetDatiPensione(this);

            ManageDecorrenzaForReversibilita(this.TitolarePensione.Pensione, areaDatiFondo.DecorrenzaPensioneDirettaDC);

            if (this.TitolarePensione.Pensione != null && this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenzaPensione.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value);

            this.areaDatiFondo = areaDatiFondo;

            RenderControls();

            if (areaDatiFondo != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;
                ViewState[EnumViewState.IsPrimoRecord.ToString()] = areaDatiFondo.IsPrimoRecord;

                if (areaDatiFondo.IsPrimoRecord.HasValue && areaDatiFondo.IsPrimoRecord.Value)
                {
                    txtDecorrenzaRegistrazione.Visible = false;
                    lblDecorrenzaRegistrazione.Visible = true;
                    RFVtxtDecorrenzaRegistrazione.Enabled = false;

                    if (((DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()]).HasValue)
                        lblDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", (DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()]);
                }
                else
                {
                    txtDecorrenzaRegistrazione.Visible = true;
                    lblDecorrenzaRegistrazione.Visible = false;
                    RFVtxtDecorrenzaRegistrazione.Enabled = true;
                    lblDecorrenzaRegistrazione.Text = "";

                    if (areaDatiFondo.DatiFondo.DecorrenzaValidita.HasValue)
                        txtDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaValidita.Value);
                }

                if (areaDatiFondo.DatiFondo != null)
                {
                    lblTipoPensione.Text = areaDatiFondo.DatiFondo.TipoPensione;

                    if (areaDatiFondo.DatiFondo.DecorrenzaCalcolo.HasValue)
                        lblDecorrenzaCalcolo.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaCalcolo.Value);

                    if (areaDatiFondo.DatiFondo.TrediciMensilita.HasValue)
                    {
                        if (areaDatiFondo.DatiFondo.TrediciMensilita.Value)
                            ddlTredicesimaMens.SelectedValue = "SI";
                        else
                            ddlTredicesimaMens.SelectedValue = "NO";
                    }
                    else
                        ddlTredicesimaMens.ClearSelection();

                    if (areaDatiFondo.DatiFondo.IntegrazioneMinimo.HasValue)
                    {
                        if (areaDatiFondo.DatiFondo.IntegrazioneMinimo.Value)
                            ddlIntegrazioneMinimo.SelectedValue = "SI";
                        else
                            ddlIntegrazioneMinimo.SelectedValue = "NO";
                    }
                    else
                        ddlIntegrazioneMinimo.ClearSelection();

                    if (areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata.HasValue)
                    {
                        if (areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata.Value)
                            ddlIndennIntegrSpecConglobata.SelectedValue = "SI";
                        else
                            ddlIndennIntegrSpecConglobata.SelectedValue = "NO";
                    }
                    else
                        ddlIndennIntegrSpecConglobata.ClearSelection();
                }
            }
        }

        internal Presenter.SvrLiquidazioneAgo.DatiFondo RecuperaCampi()
        {
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiFondo = new Presenter.SvrLiquidazioneAgo.DatiFondo();

            if (!string.IsNullOrEmpty(txtDecorrenzaRegistrazione.Text))
                this.areaDatiFondo.DatiFondo.DecorrenzaValidita = Convert.ToDateTime(txtDecorrenzaRegistrazione.Text);
            else
                if (!string.IsNullOrEmpty(lblDecorrenzaRegistrazione.Text))
                    this.areaDatiFondo.DatiFondo.DecorrenzaValidita = Convert.ToDateTime(lblDecorrenzaRegistrazione.Text);

            if (!string.IsNullOrEmpty(lblDecorrenzaCalcolo.Text))
                this.areaDatiFondo.DatiFondo.DecorrenzaCalcolo = Convert.ToDateTime(lblDecorrenzaCalcolo.Text);

            if (String.Equals(ddlTredicesimaMens.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.TrediciMensilita = true;
            else if (String.Equals(ddlTredicesimaMens.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.TrediciMensilita = false;

            if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.IntegrazioneMinimo = true;
            else if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.IntegrazioneMinimo = false;

            if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata = true;
            else if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata = false;

            return this.areaDatiFondo.DatiFondo;
        }

        protected void SalvaDatiFondo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.StoreDatiFondoByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Fondo salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiFondo(this, null);
            }
        }

        protected void btnEliminaDatiFondo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];
            this.areaDatiFondo.IsPrimoRecord = (bool)ViewState[EnumViewState.IsPrimoRecord.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.EliminaDatiFondoByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Fondo eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiFondo(this, null);

                ClearForm();

                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiFondo;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiFondo(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiFondo != null)
                UpdateSemaforoDatiFondo(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiFondo != null)
                HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            if (TornaARegistrazioniFondo != null)
                TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers

        #region private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        private void RenderControls()
        {
            if (this.areaDatiFondo != null && this.areaDatiFondo.DatiFondo != null)
            {
                if (this.areaDatiFondo.IsDecPensAnteAgosto95.HasValue)
                    pnlIntegrazioneMinimo.Visible = this.areaDatiFondo.IsDecPensAnteAgosto95.Value;
            }
        }

        private void ManageDecorrenzaForReversibilita(AreaTitolare.DatiPensione datiPensione, DateTime? decorrenzaPensioneDirettaDC)
        {
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
                ViewState[EnumViewState.DecorrenzaPensione.ToString()] = decorrenzaPensioneDirettaDC;
            else
                ViewState[EnumViewState.DecorrenzaPensione.ToString()] = datiPensione.DecorrenzaOriginaria;
        }
        #endregion private methods

        enum EnumViewState
        {
            IdRecordFondo,
            IsPrimoRecord,
            DecorrenzaPensione
        }
    }
}