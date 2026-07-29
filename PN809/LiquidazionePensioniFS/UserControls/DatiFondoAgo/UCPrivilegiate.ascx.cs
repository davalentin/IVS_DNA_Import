using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo
{
    public partial class UCPrivilegiate : CustomBaseUserControl, IDatiFondoAgo
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

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiFondo = areaDatiFondo;

            LoadDdl();
            if (areaDatiFondo != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;

                if (areaDatiFondo.DatiPrivilegiate != null)
                {
                    if (areaDatiFondo.DatiPrivilegiate.IndennitaAusiliaria.HasValue)
                        this.ddlIndennitaAusiliaria.SelectedValue = areaDatiFondo.DatiPrivilegiate.IndennitaAusiliaria.Value.ToString();
                    if (areaDatiFondo.DatiPrivilegiate.IndennitaParaplegici.HasValue)
                        this.ddlIndennitaParaplegici.SelectedValue = areaDatiFondo.DatiPrivilegiate.IndennitaParaplegici.Value.ToString();
                    if (areaDatiFondo.DatiPrivilegiate.IndennitaSpeciale.HasValue)
                        this.ddlIndennitaSpeciale.SelectedValue = areaDatiFondo.DatiPrivilegiate.IndennitaSpeciale.Value.ToString();
                }
            }
        }

        internal DatiPrivilegiate RecuperaCampi()
        {
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiPrivilegiate = new DatiPrivilegiate();

            this.areaDatiFondo.DatiPrivilegiate.IndennitaAusiliaria = !string.IsNullOrEmpty(ddlIndennitaAusiliaria.SelectedValue) ? Convert.ToInt32(ddlIndennitaAusiliaria.SelectedValue) : (int?)null;
            this.areaDatiFondo.DatiPrivilegiate.IndennitaParaplegici = !string.IsNullOrEmpty(ddlIndennitaParaplegici.SelectedValue) ? Convert.ToInt32(ddlIndennitaParaplegici.SelectedValue) : (int?)null;
            this.areaDatiFondo.DatiPrivilegiate.IndennitaSpeciale = !string.IsNullOrEmpty(ddlIndennitaSpeciale.SelectedValue) ? Convert.ToInt32(ddlIndennitaSpeciale.SelectedValue) : (int?)null;

            return this.areaDatiFondo.DatiPrivilegiate;
        }

        protected void btnSalvaPrivilegiate_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
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

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
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
            //ddlIndennitaAusiliaria.Items.Clear();
            //ddlIndennitaParaplegici.Items.Clear();
            //ddlIndennitaSpeciale.Items.Clear();

            //if (this.areaDatiFondo.ListaCodicePensioniPrivilegiate != null && this.areaDatiFondo.ListaCodicePensioniPrivilegiate.Count() > 0)
            //{
            //    List<CodicePensioniPrivilegiate> ListaCodicePensioniPrivilegiate = this.areaDatiFondo.ListaCodicePensioniPrivilegiate.ToList().FindAll(x => x.Fondo == null);

            //    foreach (CodicePensioniPrivilegiate codicePensioniPrivilegiate in ListaCodicePensioniPrivilegiate)
            //    {
            //        if (codicePensioniPrivilegiate.Posizione == 1)
            //            CodeUtility.SetValueDdl(ddlIndennitaAusiliaria, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
            //        else if (codicePensioniPrivilegiate.Posizione == 2)
            //            CodeUtility.SetValueDdl(ddlIndennitaParaplegici, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
            //        else if (codicePensioniPrivilegiate.Posizione == 3)
            //            CodeUtility.SetValueDdl(ddlIndennitaSpeciale, codicePensioniPrivilegiate.Descrizione, codicePensioniPrivilegiate.Id.ToString());
            //    }
            //}
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
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