using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCPrivilegiate : CustomBaseUserControl, IDatiFondo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

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

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiFondo = areaDatiFondo;

            RenderControls();

            LoadDdl();
            if (areaDatiFondo != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;

                if (areaDatiFondo.DatiPrivilegiate != null)
                {
                    if (this.domanda.IsDomandaINPDAP)
                    {
                        //Memo 282/2024
                        AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                        pnlEquoIndenizzo.Visible = true;
                        txtEnteEquoIndennizzo.Text = areaDatiFondo.DatiPrivilegiate.EnteEquoInd;
                        txtImportoEquoIndennizzo.Text = areaDatiFondo.DatiPrivilegiate.ImpEquoInd.HasValue ? areaDatiFondo.DatiPrivilegiate.ImpEquoInd.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;

                        bool bloccaCampiMemo = (Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) || Utility.IsDomandaReversibilita(datiPensione);
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica || bloccaCampiMemo)
                        {
                            pnlEquoIndenizzo.Enabled = false;
                        }
                    }
                    if (!(this.domanda.IsDomandaINPDAP) || (this.domanda.Categoria != null && this.domanda.Categoria.Contains("CTPS")))
                    {
                        if (areaDatiFondo.DatiPrivilegiate.AssegnoCura.HasValue)
                            this.ddlAssegnoCura.SelectedValue = areaDatiFondo.DatiPrivilegiate.AssegnoCura.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.AssegnoIntegrativo.HasValue)
                            this.ddlAssegnoIntegrativo.SelectedValue = areaDatiFondo.DatiPrivilegiate.AssegnoIntegrativo.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.CumuloInfermita.HasValue)
                            this.ddlCumulo.SelectedValue = areaDatiFondo.DatiPrivilegiate.CumuloInfermita.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.HasValue)
                            this.ddlIndennitaAccompagno.SelectedValue = areaDatiFondo.DatiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.IndennitaSpecialeAnnua.HasValue)
                            this.ddlIndennitaSpeciale.SelectedValue = areaDatiFondo.DatiPrivilegiate.IndennitaSpecialeAnnua.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.Categoria2aInfermita.HasValue)
                            this.ddlInfermita.SelectedValue = areaDatiFondo.DatiPrivilegiate.Categoria2aInfermita.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.IntegrazioneIndennitaAssistenza.HasValue)
                            this.ddlIntegrazioneIndennita.SelectedValue = areaDatiFondo.DatiPrivilegiate.IntegrazioneIndennitaAssistenza.Value.ToString();
                        if (areaDatiFondo.DatiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.HasValue)
                            this.ddlInvalidita.SelectedValue = areaDatiFondo.DatiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.Value.ToString();
                    }

                }
            }
        }

        internal DatiPrivilegiate RecuperaCampi()
        {
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiPrivilegiate = new DatiPrivilegiate();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaINPDAP)
            {              
                if (!string.IsNullOrEmpty(txtImportoEquoIndennizzo.Text))
                    this.areaDatiFondo.DatiPrivilegiate.ImpEquoInd = decimal.Parse(txtImportoEquoIndennizzo.Text);
                if (!string.IsNullOrEmpty(txtEnteEquoIndennizzo.Text))
                    this.areaDatiFondo.DatiPrivilegiate.EnteEquoInd = txtEnteEquoIndennizzo.Text;
            }
            if (!(this.domanda.IsDomandaINPDAP) || (this.domanda.Categoria != null && this.domanda.Categoria.Contains("CTPS")))
            {
                this.areaDatiFondo.DatiPrivilegiate.AssegnoCura = !String.IsNullOrEmpty(ddlAssegnoCura.SelectedValue) ? Convert.ToInt32(ddlAssegnoCura.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.AssegnoIntegrativo = !String.IsNullOrEmpty(ddlAssegnoIntegrativo.SelectedValue) ? Convert.ToInt32(ddlAssegnoIntegrativo.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.Categoria2aInfermita = !String.IsNullOrEmpty(ddlInfermita.SelectedValue) ? Convert.ToInt32(ddlInfermita.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.CumuloInfermita = !String.IsNullOrEmpty(ddlCumulo.SelectedValue) ? Convert.ToInt32(ddlCumulo.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.IndennitaAccompagnamentoAggiuntiva = !String.IsNullOrEmpty(ddlIndennitaAccompagno.SelectedValue) ? Convert.ToInt32(ddlIndennitaAccompagno.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.IndennitaSpecialeAnnua = !String.IsNullOrEmpty(ddlIndennitaSpeciale.SelectedValue) ? Convert.ToInt32(ddlIndennitaSpeciale.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.IntegrazioneIndennitaAssistenza = !String.IsNullOrEmpty(ddlIntegrazioneIndennita.SelectedValue) ? Convert.ToInt32(ddlIntegrazioneIndennita.SelectedValue) : (int?)null;
                this.areaDatiFondo.DatiPrivilegiate.PrivilegiataSuperinvaliditaIndennita = !String.IsNullOrEmpty(ddlInvalidita.SelectedValue) ? Convert.ToInt32(ddlInvalidita.SelectedValue) : (int?)null;
            }
            return this.areaDatiFondo.DatiPrivilegiate;
        }

        protected void btnSalvaPrivilegiate_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.StoreDatiPrivilegiateByIdRecordFondo(this);
            

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Privilegiate salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiPrivilegiate(this, null);
            }
        }

        protected void btnEliminaPrivilegiate_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.EliminaDatiPrivilegiateByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Privilegiate eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiPrivilegiate(this, null);
                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region private methods

        private void LoadDdl()
        {
            if (this.domanda.IsDomandaINPDAP && this.domanda.Categoria != null && !this.domanda.Categoria.Contains("CTPS"))
            {

            }
            else if (this.domanda.Categoria != null && !(this.domanda.IsDomandaINPDAP) || this.domanda.Categoria.Contains("CTPS"))
            {
                ddlAssegnoCura.Items.Clear();
                ddlAssegnoIntegrativo.Items.Clear();
                ddlCumulo.Items.Clear();
                ddlIndennitaAccompagno.Items.Clear();
                ddlIndennitaSpeciale.Items.Clear();
                ddlInfermita.Items.Clear();
                ddlIntegrazioneIndennita.Items.Clear();
                ddlInvalidita.Items.Clear();

                if (this.areaDatiFondo.ListaCodicePensioniPrivilegiate != null && this.areaDatiFondo.ListaCodicePensioniPrivilegiate.Count() > 0 && (this.domanda.Tipofondo.HasValue || (this.domanda.Categoria != null && this.domanda.Categoria.Contains("CTPS"))))
                {
                    List<CodicePensioniPrivilegiate> ListaCodicePensioniPrivilegiate = new List<CodicePensioniPrivilegiate>();
                    if (!this.domanda.Categoria.Contains("CTPS"))
                        ListaCodicePensioniPrivilegiate = this.areaDatiFondo.ListaCodicePensioniPrivilegiate.ToList().FindAll(x => x.Fondo == this.domanda.Tipofondo.Value.ToString());
                    else
                        ListaCodicePensioniPrivilegiate = this.areaDatiFondo.ListaCodicePensioniPrivilegiate.ToList().FindAll(x => x.Fondo == "DAP");

                    foreach (CodicePensioniPrivilegiate codicePensioniPrivilegiate in ListaCodicePensioniPrivilegiate)
                    {
                        if (codicePensioniPrivilegiate.Posizione == 1)
                            CodeUtility.SetValueDdl(ddlInvalidita, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 2)
                            CodeUtility.SetValueDdl(ddlAssegnoIntegrativo, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 3)
                            CodeUtility.SetValueDdl(ddlIntegrazioneIndennita, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 4)
                            CodeUtility.SetValueDdl(ddlIndennitaAccompagno, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 5)
                            CodeUtility.SetValueDdl(ddlCumulo, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 6)
                            CodeUtility.SetValueDdl(ddlInfermita, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 7)
                            CodeUtility.SetValueDdl(ddlAssegnoCura, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                        else if (codicePensioniPrivilegiate.Posizione == 8)
                            CodeUtility.SetValueDdl(ddlIndennitaSpeciale, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
                    }
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        private void RenderControls()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!(this.domanda.IsDomandaINPDAP) || (this.domanda.Categoria != null && this.domanda.Categoria.Contains("CTPS") && (ConfigurationManager.AppSettings["AbilitaPannelloPrivilegiateIOCTPS"] != null && ConfigurationManager.AppSettings["AbilitaPannelloPrivilegiateIOCTPS"] == "SI")))
                pnlPrivilegiate.Visible = true;
        }

        #endregion private methods

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiPrivilegiate;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiPrivilegiate(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiPrivilegiate != null)
                UpdateSemaforoDatiPrivilegiate(sender, e);
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