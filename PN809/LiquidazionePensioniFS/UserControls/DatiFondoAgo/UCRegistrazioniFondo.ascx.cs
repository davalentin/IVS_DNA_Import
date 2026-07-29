using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo
{
    public partial class UCRegistrazioniFondo : CustomBaseUserControl, IDatiFondoAgo, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IDatiFondoAgo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            ViewState[EnumViewState.DecorrenzaRegistrazione.ToString()] = this.TitolarePensione.Pensione.DecorrenzaOriginaria;

            this.areaDatiFondo = areaDatiFondo;
            GvLoad();
        }

        protected void btnAggiungiRegistrazione_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.AddRegistrazioneFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                RaiseShowPulsanteSalva(this, null);
                RaiseRecordSelezionato(this, null);
            }
        }

        protected void btnEliminaRegistrazioni_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.CancelAllRegistrazioneFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Record eliminati correttamente.";
                RaiseShowAvviso(this, null);
                GvLoad();
            }
        }

        #region Griglia Registrazioni Fondo
        protected void gvRegistrazioniFondo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();

            if (e.CommandName == "Elimina")
            {
                if (this.areaDatiFondo == null)
                    this.areaDatiFondo = new AreaDatiFondo();
                this.areaDatiFondo.IdRecordFondo = long.Parse(((HiddenField)((GridViewRow)((Control)e.CommandSource).NamingContainer).FindControl("hdnIdRecordFondo")).Value);

                presenter.CancelRegistrazioneFondo(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    this.ErrorMessage = "Record eliminato correttamente.";
                    RaiseShowAvviso(this, null);
                    GvLoad();
                }
            }
            else if (e.CommandName == "Modifica")
            {
                if (this.areaDatiFondo == null)
                    this.areaDatiFondo = new AreaDatiFondo();
                this.areaDatiFondo.IdRecordFondo = long.Parse(((HiddenField)((GridViewRow)((Control)e.CommandSource).NamingContainer).FindControl("hdnIdRecordFondo")).Value);
                this.areaDatiFondo.IsPrimoRecord = ((GridViewRow)((Control)e.CommandSource).NamingContainer).DataItemIndex == 0;

                presenter.GetRegistrazioneFondoByIdRecordFondo(this);

                RaiseShowPulsanteSalva(this, null);
                RaiseRecordSelezionato(this, null);
            }
        }

        protected void gvRegistrazioniFondo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        ((Button)e.Row.FindControl("btnElimina")).Visible = false;

                        if (!((DatiRegistrazioneFondo.DatiRecordFondo)e.Row.DataItem).DecorrenzaValiditaDati.HasValue)
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:dd/MM/yyyy}", (DateTime?)ViewState[EnumViewState.DecorrenzaRegistrazione.ToString()]);
                    }

                    ValorizzaSemaforoRecord(e.Row);
                    RenderColumns(e.Row);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRegistrazioniFondo, Errore nel metodo gvRegistrazioniFondo_RowDataBound: " + ex);
            }
        }

        #endregion Griglia Registrazioni Fondo

        #region private methods
        private void ValorizzaSemaforoRecord(GridViewRow row)
        {
            DatiRegistrazioneFondo.DatiRecordFondo datiRecordFondo = (DatiRegistrazioneFondo.DatiRecordFondo)row.DataItem;

            if ((datiRecordFondo.TabDatiFondo.HasValue && datiRecordFondo.TabDatiFondo.Value == 0) ||
                (datiRecordFondo.TabDatiCalcolo.HasValue && datiRecordFondo.TabDatiCalcolo.Value == 0) ||
                (datiRecordFondo.TabPrivilegiate.HasValue && datiRecordFondo.TabPrivilegiate.Value == 0) ||
                (datiRecordFondo.TabArticolo2.HasValue && datiRecordFondo.TabArticolo2.Value == 0))
            {
                ((Image)row.FindControl("imgSemaforo")).ImageUrl = "~/App_Themes/BlueINPS1/Images/rosso_tab.png";
                ((Image)row.FindControl("imgSemaforo")).ToolTip = "Record non completato";
            }
            else
            {
                ((Image)row.FindControl("imgSemaforo")).ImageUrl = "~/App_Themes/BlueINPS1/Images/verde_tab.png";
                ((Image)row.FindControl("imgSemaforo")).ToolTip = "Record non completato";
            }

            if (datiRecordFondo.TabArticolo2.HasValue)
            {
                if (datiRecordFondo.TabArticolo2.Value == 2)
                    ((Image)row.FindControl("imgArticolo2")).ImageUrl = "~/App_Themes/BlueINPS1/Images/verified.png";
                else if (datiRecordFondo.TabArticolo2.Value == 0)
                    ((Image)row.FindControl("imgArticolo2")).ImageUrl = "~/App_Themes/BlueINPS1/Images/rosso.png";
                else
                    ((Image)row.FindControl("imgArticolo2")).ImageUrl = "~/App_Themes/BlueINPS1/Images/arancione_tab.png";
            }

            if (datiRecordFondo.TabPrivilegiate.HasValue)
            {
                if (datiRecordFondo.TabPrivilegiate.Value == 2)
                    ((Image)row.FindControl("imgPrivilegiata")).ImageUrl = "~/App_Themes/BlueINPS1/Images/verified.png";
                else if (datiRecordFondo.TabPrivilegiate.Value == 0)
                    ((Image)row.FindControl("imgPrivilegiata")).ImageUrl = "~/App_Themes/BlueINPS1/Images/rosso.png";
                else
                    ((Image)row.FindControl("imgPrivilegiata")).ImageUrl = "~/App_Themes/BlueINPS1/Images/arancione_tab.png";
            }
        }

        private void RenderColumns(GridViewRow row)
        {
            DatiRegistrazioneFondo.DatiRecordFondo datiRecordFondo = (DatiRegistrazioneFondo.DatiRecordFondo)row.DataItem;

            if (!datiRecordFondo.TabArticolo2.HasValue)
                gvRegistrazioniFondo.Columns[colonneGvRegistrazioniFondo.Articolo2.GetHashCode()].Visible = false;
            if (!datiRecordFondo.TabPrivilegiate.HasValue)
                gvRegistrazioniFondo.Columns[colonneGvRegistrazioniFondo.Privilegiata.GetHashCode()].Visible = false;
        }

        private void GvLoad()
        {
            if (areaDatiFondo != null && areaDatiFondo.DatiRegistrazioniFondo != null && areaDatiFondo.DatiRegistrazioniFondo.lRecordFondo != null)
            {
                ViewState[EnumViewState.ListaRecordFondo.ToString()] = areaDatiFondo.DatiRegistrazioniFondo.lRecordFondo;

                gvRegistrazioniFondo.DataSource = areaDatiFondo.DatiRegistrazioniFondo.lRecordFondo;
                gvRegistrazioniFondo.DataBind();
            }
        }
        #endregion private methods

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler ShowPulsanteSalva;
        public event EventHandler RecordSelezionato;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowPulsanteSalva(object sender, EventArgs e)
        {
            if (ShowPulsanteSalva != null)
                ShowPulsanteSalva(sender, e);
        }

        protected void RaiseRecordSelezionato(object sender, EventArgs e)
        {
            if (RecordSelezionato != null)
                RecordSelezionato(sender, e);
        }
        #endregion Event Handlers

        enum colonneGvRegistrazioniFondo
        {
            Articolo2 = 2,
            Privilegiata = 3
        }

        public enum EnumViewState
        {
            ListaRecordFondo,
            DecorrenzaRegistrazione
        }
    }
}