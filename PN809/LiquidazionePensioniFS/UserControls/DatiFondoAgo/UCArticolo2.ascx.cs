using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo
{
    public partial class UCArticolo2 : CustomBaseUserControl, IDatiFondoAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            if (areaDatiFondo != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;

                if (areaDatiFondo.DatiArticolo2 != null)
                {
                    if (areaDatiFondo.DatiArticolo2.ScadenzaBenefici.HasValue)
                        txtScadenzaBenefici.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiArticolo2.ScadenzaBenefici.Value);

                    if (areaDatiFondo.DatiArticolo2.PALConBenefici.HasValue)
                        txtPALConBenefici.Text = areaDatiFondo.DatiArticolo2.PALConBenefici.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (areaDatiFondo.DatiArticolo2.ScadenzaIllimitata.HasValue)
                        chkScadenzaIllimitata.Checked = areaDatiFondo.DatiArticolo2.ScadenzaIllimitata.Value;
                    else
                        chkScadenzaIllimitata.Checked = false;
                }
            }
        }

        internal Presenter.SvrLiquidazioneAgo.DatiArticolo2 RecuperaCampi()
        {
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiArticolo2 = new DatiArticolo2();

            if (!string.IsNullOrEmpty(txtScadenzaBenefici.Text))
                this.areaDatiFondo.DatiArticolo2.ScadenzaBenefici = Convert.ToDateTime(txtScadenzaBenefici.Text);

            if (!string.IsNullOrEmpty(txtPALConBenefici.Text))
                this.areaDatiFondo.DatiArticolo2.PALConBenefici = decimal.Parse(txtPALConBenefici.Text);

            this.areaDatiFondo.DatiArticolo2.ScadenzaIllimitata = chkScadenzaIllimitata.Checked;

            return this.areaDatiFondo.DatiArticolo2;
        }

        protected void btnSalvaArticolo2_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.StoreDatiArticolo2ByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Articolo 2 salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiArticolo2(this, null);
            }
        }

        protected void btnEliminaArticolo2_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.EliminaDatiArticolo2ByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Articolo 2 eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiArticolo2(this, null);
                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }
        #endregion private methods

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiArticolo2;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiArticolo2(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiArticolo2 != null)
                UpdateSemaforoDatiArticolo2(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            if (HidePulsanteSalva != null)
                HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            if (TornaARegistrazioniFondo != null)
                TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers

        enum EnumViewState
        {
            IdRecordFondo
        }
    }
}