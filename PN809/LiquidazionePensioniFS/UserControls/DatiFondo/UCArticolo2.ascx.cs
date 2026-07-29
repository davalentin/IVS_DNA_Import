using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using System.Collections.Generic;
using System.Linq;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCArticolo2 : CustomBaseUserControl, IDatiFondo, ITitolarePensione, ILiquidazionePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaTitolare TitolarePensione { get; set; }

        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        #endregion IDatiFondo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            RenderControls();

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = this.GetDatiPensione(this);

            if (areaDatiFondo != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;

                if (areaDatiFondo.DatiArticolo2 != null)
                {
                    if (areaDatiFondo.DatiArticolo2.ScadenzaBenefici.HasValue)
                    {
                        if (this.domanda.IsDomandaINPDAP)
                            txtScadenzaBeneficiINPDAP.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiArticolo2.ScadenzaBenefici.Value);
                        else
                            txtScadenzaBenefici.Text = String.Format("{0:MM/yyyy}", areaDatiFondo.DatiArticolo2.ScadenzaBenefici.Value);
                    }

                    if (this.domanda.IsDomandaINPDAP && (Utility.IsDomandaInabilitaLegge335(this.TitolarePensione.Pensione) || isRicDomandaInabilitaLegge335()))
                    {
                        pnlPAL.Visible = false;
                        idNoPAL.Visible = true;
                    }
                    else if (areaDatiFondo.DatiArticolo2.PALConBenefici.HasValue)
                        txtPALConBenefici.Text = areaDatiFondo.DatiArticolo2.PALConBenefici.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (areaDatiFondo.DatiArticolo2.ScadenzaIllimitata.HasValue)
                        chkScadenzaIllimitata.Checked = areaDatiFondo.DatiArticolo2.ScadenzaIllimitata.Value;
                    else
                        chkScadenzaIllimitata.Checked = false;

                    if (this.domanda.IsDomandaINPDAP && Utility.IsDomandaInabilitaLegge335(this.TitolarePensione.Pensione) && 
                        Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica 
                        && (this.TitolarePensione.Pensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG || this.TitolarePensione.Pensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI))
                    {
                        txtScadenzaBeneficiINPDAP.Enabled = false;
                        chkScadenzaIllimitata.Enabled = false;
                        btnEliminaArticolo2.Enabled = false;
                    }
                }
            }
        }

        internal DatiArticolo2ForDatiFondo RecuperaCampi()
        {
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiArticolo2 = new DatiArticolo2ForDatiFondo();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaINPDAP)
            {
                if (!string.IsNullOrEmpty(txtScadenzaBeneficiINPDAP.Text))
                    this.areaDatiFondo.DatiArticolo2.ScadenzaBenefici = Utility.GetDateFromString(txtScadenzaBeneficiINPDAP.Text);
            }
            else
            {
                if (!string.IsNullOrEmpty(txtScadenzaBenefici.Text))
                    this.areaDatiFondo.DatiArticolo2.ScadenzaBenefici = Utility.GetDateFromString(txtScadenzaBenefici.Text);
            }

            if (!string.IsNullOrEmpty(txtPALConBenefici.Text))
                this.areaDatiFondo.DatiArticolo2.PALConBenefici = decimal.Parse(txtPALConBenefici.Text);

            this.areaDatiFondo.DatiArticolo2.ScadenzaIllimitata = chkScadenzaIllimitata.Checked;

            return this.areaDatiFondo.DatiArticolo2;
        }

        protected void btnSalvaArticolo2_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            PresenterDatiFondo presenter = new PresenterDatiFondo();
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
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            PresenterDatiFondo presenter = new PresenterDatiFondo();
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

        private void RenderControls()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaINPDAP)
                tdScadenzaAssegnoINPDAP.Visible = true;
            else
                tdScadenzaAssegnoFS.Visible = true;
        }

        private byte? GetIdCodiceSpecificoByTraduzioneSuGP(ILiquidazionePensione liquidazione, char traduzioneSuGP)
        {
            CodiceSpecifico codiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().Find(delegate (CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                  code.Fondo == "DAP" && code.TraduzioneGp == traduzioneSuGP);
            });
            return codiceSpecifico != null ? codiceSpecifico.Id : null;
        }

        public bool isRicDomandaInabilitaLegge335()
        {
            if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && this.domanda.Categoria.ToUpperInvariant().StartsWith("I"))
            {
                PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
                presenterLiquidazione.GetLiquidazionePensione(this);
                if (areaLiquidazionePensioneFS != null && areaLiquidazionePensioneFS.DatiAssicurativiINPDAP != null && areaLiquidazionePensioneFS.ListaCodiceSpecifico != null)
                {                                      
                        var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(this, 'F');
                        if (areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CodiceSpecifico == idCodiceSpecifico)
                            return true;
                }
            }
            return false;
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