using System;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiCalcoloPI : CustomBaseUserControl, IDatiContributivi, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiCalcolo(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiCalcolo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Calcolo";
            else
            {
                ClearForm();
                RaiseCaricaDatiCalcolo(this.areaDatiContributivi, null);
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        #region internal methods
        internal void RecuperaCampi()
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            if (this.areaDatiContributivi.DatiCalcolo.fondoPI == null)
                this.areaDatiContributivi.DatiCalcolo.fondoPI = new GestioneContribFondoPI();

            this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = (GestioneContribTipoCalcolo)ViewState[EnumViewState.TipoCalcolo.ToString()];

            if (!string.IsNullOrEmpty(txtStipendioAnnuo.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.StipendioAnnuo = CodeUtility.StringToNullableDecimal(txtStipendioAnnuo.Text);

            if (!string.IsNullOrEmpty(txtStipendioBase.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.StipendioBase = CodeUtility.StringToNullableDecimal(txtStipendioBase.Text);

            if (!string.IsNullOrEmpty(txtImportoIIS.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.ImportoIIS = CodeUtility.StringToNullableDecimal(txtImportoIIS.Text);

            if (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null)
            {
                switch (((UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()]).Value)
                {
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.U:
                        RecuperaCampiU();
                        RecuperaCampiHidden();
                        break;
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.V:
                        RecuperaCampiV();
                        break;
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.A:
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.B:
                        RecuperaCampiA();
                        RecuperaCampiHidden();
                        break;
                    default:
                        RecuperaCampiHidden();
                        break;
                }
            }
        }

        internal void ValorizzaEtichette()
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (this.areaDatiContributivi != null)
            {
                ViewState[EnumViewState.CategoriaFondoPI.ToString()] = this.areaDatiContributivi.CategoriaFondoPI;

                RenderControls();
                LoadDdls();

                if (this.areaDatiContributivi.DatiCalcolo != null)
                {
                    ViewState[EnumViewState.TipoCalcolo.ToString()] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;

                    if (this.areaDatiContributivi.DatiCalcolo.fondoPI != null)
                    {
                        if (this.areaDatiContributivi.DatiCalcolo.fondoPI.StipendioAnnuo.HasValue)
                            txtStipendioAnnuo.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.StipendioAnnuo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                        if (this.areaDatiContributivi.DatiCalcolo.fondoPI.StipendioBase.HasValue)
                            txtStipendioBase.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.StipendioBase.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                        if (this.areaDatiContributivi.DatiCalcolo.fondoPI.ImportoIIS.HasValue)
                            txtImportoIIS.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.ImportoIIS.Value.ToString();

                        if (this.areaDatiContributivi.CategoriaFondoPI.HasValue)
                        {
                            switch (this.areaDatiContributivi.CategoriaFondoPI.Value)
                            {
                                case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.U:
                                    ValorizzaEtichetteCatU();
                                    ValorizzaEtichetteHidden();
                                    break;
                                case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.V:
                                    ValorizzaEtichetteCatV();
                                    break;
                                case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.A:
                                case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.B:
                                    ValorizzaEtichetteCatA();
                                    ValorizzaEtichetteHidden();
                                    break;
                                default:
                                    ValorizzaEtichetteHidden();
                                    break;
                            }

                        }
                    }
                }
                GestioneEtichetteRic(datiPensione);
                //rimosso per tutte le PI il 22/01/2026
                RFVtxtStipendioBase.Enabled = false;
            }
        }
        #endregion internal methods

        #region Private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);

        }

        private void RenderControls()
        {
            if (this.areaDatiContributivi.CategoriaFondoPI.HasValue)
            {
                switch (this.areaDatiContributivi.CategoriaFondoPI.Value)
                {
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.U:
                        pnlCatU.Visible = true;
                        break;
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.V:
                        pnlCatV.Visible = true;
                        trLblPensioneFacoltativaMensile.Visible = true;
                        trTxtPensioneFacoltativaMensile.Visible = true;
                        break;
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.A:
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.B:
                        if(this.areaDatiContributivi.IsPIAPIBAnte99.GetValueOrDefault())
                            pnlCatAB.Visible = true;
                        break;
                }
            }
        }

        private void LoadDdls()
        {
            ddlAttCon.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttCon);
            if (this.areaDatiContributivi.ListaAttCon != null && this.areaDatiContributivi.ListaAttCon.Count() > 0)
            {
                foreach (AttCon attCon in this.areaDatiContributivi.ListaAttCon)
                    CodeUtility.SetValueDdl(ddlAttCon, string.Format("{0} - {1}", attCon.Id.ToString(), attCon.Descrizione), attCon.Descrizione, attCon.Id.ToString());
            }
        }

        #region Cat U
        private void ValorizzaEtichetteCatU()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoPI != null)
            {
                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.HasValue)
                    ddlAttCon.SelectedValue = this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.Value.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.HasValue)
                    txtPercentualeCapitalizzazione.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.CodiceMaggiorazione.HasValue)
                    ddlCodiceMaggiorazione.SelectedValue = this.areaDatiContributivi.DatiCalcolo.fondoPI.CodiceMaggiorazione.Value.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.PensComplRiv1_95.HasValue)
                    txtPensComplRiv1_95.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.PensComplRiv1_95.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private void RecuperaCampiU()
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            if (this.areaDatiContributivi.DatiCalcolo.fondoPI == null)
                this.areaDatiContributivi.DatiCalcolo.fondoPI = new GestioneContribFondoPI();

            if (!string.IsNullOrEmpty(ddlAttCon.SelectedValue))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon = CodeUtility.StringToNullableChar(ddlAttCon.SelectedValue);

            if (!string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione = CodeUtility.StringToNullableDecimal(txtPercentualeCapitalizzazione.Text);

            if (!string.IsNullOrEmpty(ddlCodiceMaggiorazione.SelectedValue))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.CodiceMaggiorazione = CodeUtility.StringToNullableChar(ddlCodiceMaggiorazione.SelectedValue);

            if (!string.IsNullOrEmpty(txtPensComplRiv1_95.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.PensComplRiv1_95 = CodeUtility.StringToNullableDecimal(txtPensComplRiv1_95.Text);

            if (!string.IsNullOrEmpty(txtControCodiceRetribuzione.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.ControCodiceRetribuzione = CodeUtility.StringToNullableShort(txtControCodiceRetribuzione.Text);
        }
        #endregion Cat U

        #region Cat V
        private void ValorizzaEtichetteCatV()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoPI != null)
            {
                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.RMSQuotaA.HasValue)
                    txtRMSQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.RMSQuotaB.HasValue)
                    txtRMSQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.NSettimaneQuotaA.HasValue)
                    txtNSettimaneQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.NSettimaneQuotaA.Value.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.NSettimaneQuotaB.HasValue)
                    txtNSettimaneQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.NSettimaneQuotaB.Value.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.PensioneFacoltativaMensile.HasValue)
                    txtPensioneFacoltativaMensile.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.PensioneFacoltativaMensile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private void RecuperaCampiV()
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            if (this.areaDatiContributivi.DatiCalcolo.fondoPI == null)
                this.areaDatiContributivi.DatiCalcolo.fondoPI = new GestioneContribFondoPI();

            if (!string.IsNullOrEmpty(txtRMSQuotaA.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.RMSQuotaA = CodeUtility.StringToNullableDecimal(txtRMSQuotaA.Text);

            if (!string.IsNullOrEmpty(txtRMSQuotaB.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.RMSQuotaB = CodeUtility.StringToNullableDecimal(txtRMSQuotaB.Text);

            if (!string.IsNullOrEmpty(txtNSettimaneQuotaA.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.NSettimaneQuotaA = CodeUtility.StringToNullableShort(txtNSettimaneQuotaA.Text);

            if (!string.IsNullOrEmpty(txtNSettimaneQuotaB.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.NSettimaneQuotaB = CodeUtility.StringToNullableShort(txtNSettimaneQuotaB.Text);

            if (!string.IsNullOrEmpty(txtPensioneFacoltativaMensile.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.PensioneFacoltativaMensile = CodeUtility.StringToNullableDecimal(txtPensioneFacoltativaMensile.Text);
        }
        #endregion Cat V

        #region Cat A
        private void ValorizzaEtichetteCatA()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoPI != null)
            {
                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.HasValue)
                    txtPercentualeCapitalizzazione.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private void RecuperaCampiA()
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            if (this.areaDatiContributivi.DatiCalcolo.fondoPI == null)
                this.areaDatiContributivi.DatiCalcolo.fondoPI = new GestioneContribFondoPI();
           
            if (!string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text))
                this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione = CodeUtility.StringToNullableDecimal(txtPercentualeCapitalizzazione.Text);
        }

        #endregion

        #region altre Cat

        private void ValorizzaEtichetteHidden()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoPI != null)
            {
                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.HasValue && hdnAttCon != null && string.IsNullOrEmpty(hdnAttCon.Value))
                    hdnAttCon.Value = this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.Value.ToString();

                if (this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.HasValue && string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text))
                    txtPercentualeCapitalizzazione.Text = this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private void RecuperaCampiHidden()
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (hdnAttCon != null && !string.IsNullOrEmpty(hdnAttCon.Value) && !this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.HasValue)
                this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon = Convert.ToChar(hdnAttCon.Value);

            if (!string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text) && !this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione.HasValue)
                this.areaDatiContributivi.DatiCalcolo.fondoPI.PercentualeCapitalizzazione = CodeUtility.StringToNullableDecimal(txtPercentualeCapitalizzazione.Text);

        }
        #endregion

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                txtRMSQuotaA.Enabled = false;
                txtNSettimaneQuotaA.Enabled = false;
                txtRMSQuotaB.Enabled = false;
                txtNSettimaneQuotaB.Enabled = false;
                txtStipendioAnnuo.Enabled = false;
                RFVtxtStipendioAnnuo.Enabled = false;
                txtStipendioBase.Enabled = false;
                RFVtxtStipendioBase.Enabled = false;
                txtImportoIIS.Enabled = false;
                txtPensioneFacoltativaMensile.Enabled = false;
                ddlAttCon.Enabled = false;
                txtPercentualeCapitalizzazione.Enabled = false;
                ddlCodiceMaggiorazione.Enabled = false;
                txtPensComplRiv1_95.Enabled = false;
                txtControCodiceRetribuzione.Enabled = false;
                RFVtxtControCodiceRetribuzione.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }
        }
        #endregion Private methods

        #region Event

        public event EventHandler CaricaDatiCalcolo;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

        protected void RaiseCaricaDatiCalcolo(object sender, EventArgs e)
        {
            CaricaDatiCalcolo(sender, e);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion Event

        #region Enum
        public enum EnumViewState
        {
            CategoriaFondoPI,
            TipoCalcolo
        }
        #endregion Enum
    }
}