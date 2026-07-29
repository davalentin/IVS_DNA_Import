using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiNoCalcolo
{
    public partial class UCRecordDatiNoCalcolo : CustomBaseUserControl, IDatiNoCalcolo
    {

        #region IDatiNoCalcolo
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public long IdRecordNoCalcolo { get; set; }
        public AreaNoCalcolo AreaNoCalcolo { get; set; }
        #endregion IDatiNoCalcolo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(Presenter.SvrLiquidazioneFs.AreaNoCalcolo areaNoCalcolo)
        {
            ViewState[VS_UCRecordDatiNoCalcolo.AreaDatiNoCalcolo] = areaNoCalcolo;
            LoadGv(areaNoCalcolo);
        }

        private void LoadGv(Presenter.SvrLiquidazioneFs.AreaNoCalcolo areaNoCalcolo)
        {
            if (areaNoCalcolo != null && areaNoCalcolo.LstRecordNoCalcolo != null && areaNoCalcolo.LstRecordNoCalcolo.Count() > 0)
            {
                gvRecordNoCalcolo.DataSource = areaNoCalcolo.LstRecordNoCalcolo;
                gvRecordNoCalcolo.DataBind();
            }
            else
            {
                gvRecordNoCalcolo.DataSource = null;
                gvRecordNoCalcolo.DataBind();
 
            }
        }

        #region Griglia Registrazioni No Calcolo
        protected void gvRegistrazioniNoCalcolo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();

            if (e.CommandName == "Elimina")
            {
                
                this.IdRecordNoCalcolo = long.Parse(((HiddenField)((GridViewRow)((Control)e.CommandSource).NamingContainer).FindControl("hdnIdRecordNoCalcolo")).Value);

                presenter.DeleteRecordNoCalcolo(this);
                ValorizzaEtichette(this.AreaNoCalcolo);
                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Record eliminato correttamente.";
                    RaiseShowAvviso(this, null);
                   
                }
            }
            else if (e.CommandName == "Modifica")
            {
                this.IdRecordNoCalcolo = long.Parse(((HiddenField)((GridViewRow)((Control)e.CommandSource).NamingContainer).FindControl("hdnIdRecordNoCalcolo")).Value);

                presenter.GetDatiNoCalcoloByIdRecord(this);

                RaiseShowPulsanteSalva(this, null);
                RaiseRecordSelezionato(this, null);
            }
        }

        protected void gvRegistrazioniNoCalcolo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ValorizzaSemaforoRecord(e.Row);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRecordNoCalcolo, Errore nel metodo gvRecordNoCalcolo_RowDataBound: " + ex);
            }
        }

        protected void GvRecordNoCalcolo_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRecordNoCalcolo.PageIndex = e.NewPageIndex;
                AreaNoCalcolo areaNoCalcolo =(AreaNoCalcolo)ViewState[VS_UCRecordDatiNoCalcolo.AreaDatiNoCalcolo];
                LoadGv(areaNoCalcolo);                 
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRecordNoCalcolo, Errore nel metodo gvPratiche_onPageIndexChanging" + ex);
            }
        }
        #endregion Griglia Registrazioni Dati No Calcolo

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler ShowPulsanteSalva;
        public event EventHandler RecordSelezionato;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowPulsanteSalva(object sender, EventArgs e)
        {
            ShowPulsanteSalva(sender, e);
        }

        protected void RaiseRecordSelezionato(object sender, EventArgs e)
        {
            RecordSelezionato(sender, e);
        }
        #endregion Event Handlers

        #region Private Methods
        private void ValorizzaSemaforoRecord(GridViewRow row)
        {
            string currentTheme = Page.Theme;
            DatiRecordNoCalcolo datiRecordFondo = (DatiRecordNoCalcolo)row.DataItem;

            if ((datiRecordFondo.TabNoCalcolo.HasValue && datiRecordFondo.TabNoCalcolo.Value == 0))
            {
                ((Image)row.FindControl("imgRecordDatiNoCalcolo")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                ((Image)row.FindControl("imgRecordDatiNoCalcolo")).ToolTip = "Record non completato";
            }
            else
            {
                ((Image)row.FindControl("imgRecordDatiNoCalcolo")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                ((Image)row.FindControl("imgRecordDatiNoCalcolo")).ToolTip = "Record completato";
            }
        }
        #endregion Private Methods

        protected void btnAggiungiRegistrazione_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();
            presenter.AddRecordNoCalcolo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
               
                //RaiseShowPulsanteSalva(this, null);
                RaiseRecordSelezionato(this, null);
                //LoadGv(this.AreaNoCalcolo);
            }
        }

        protected void btnEliminaRegistrazioni_Click(object sender, EventArgs e)
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.AreaNoCalcolo = new AreaNoCalcolo();
            this.AreaNoCalcolo.DatiNoCalcolo = new Presenter.SvrLiquidazioneFs.DatiNoCalcolo();

            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();
            presenter.DeleteAllRecordNoCalcolo(this);
            ValorizzaEtichette(this.AreaNoCalcolo);
            if (!this.HasError)
                this.ErrorMessage = "Dati No Calcolo eliminati correttamente.";

            RaiseShowAvviso(this, null);   
        }

        private static class VS_UCRecordDatiNoCalcolo
        {
            public const string AreaDatiNoCalcolo = "AreaDatiNoCalcolo";
        }

    }
}