using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCRegistrazioniFondo : CustomBaseUserControl, IDatiFondo, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            ViewState[EnumViewState.DecorrenzaRegistrazione.ToString()] = this.TitolarePensione.Pensione.DecorrenzaOriginaria;
            ViewState[EnumViewState.IsConsultazione.ToString()] = CodeUtility.IsConsultazione((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], Session["Ruolo"]);
            this.areaDatiFondo = areaDatiFondo;
            GvLoad();

            if (((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                && (Utility.IsRicostituzione_TrattamentoDiFamiglia(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_Reddituale(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(this.TitolarePensione.Pensione)))
                || (this.domanda.IsDomandaINPDAP && !CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione)))
            {
                btnAggiungiRegistrazione.Enabled = false;
                btnEliminaRegistrazioni.Enabled = false;
            }
            else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ)
            {
                if (this.domanda.Categoria.Trim() == "SDZ")
                {
                    btnAggiungiRegistrazione.Enabled = true;
                }
                else
                {
                    btnAggiungiRegistrazione.Enabled = false;
                }
            }
        }


        protected void btnAggiungiRegistrazione_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Session["IsNewRecord"] = true;
            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
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
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
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
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();

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
            else if (e.CommandName == "Modifica" || e.CommandName == "Consulta")
            {
                if (this.areaDatiFondo == null)
                    this.areaDatiFondo = new AreaDatiFondo();
                this.areaDatiFondo.IdRecordFondo = long.Parse(((HiddenField)((GridViewRow)((Control)e.CommandSource).NamingContainer).FindControl("hdnIdRecordFondo")).Value);
                this.areaDatiFondo.IsPrimoRecord = ((GridViewRow)((Control)e.CommandSource).NamingContainer).DataItemIndex == 0;

                presenter.GetRegistrazioneFondoByIdRecordFondo(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                {
                    RaiseShowPulsanteSalva(this, null);
                    RaiseRecordSelezionato(this, null);
                }
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
                    else
                    {
                        if (this.domanda == null)
                            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                        if (this.TitolarePensione == null)
                            this.TitolarePensione = new AreaTitolare();
                        if (this.TitolarePensione.Pensione == null)
                            this.TitolarePensione.Pensione = GetDatiPensione(this);

                        if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                            && (Utility.IsRicostituzione_TrattamentoDiFamiglia(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_Reddituale(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(this.TitolarePensione.Pensione)))
                            ((Button)e.Row.FindControl("btnElimina")).Visible = false;
                    }

                    if (ViewState[EnumViewState.IsConsultazione.ToString()] != null && (bool)ViewState[EnumViewState.IsConsultazione.ToString()])
                    {
                        ((Button)e.Row.FindControl("btnModifica")).Visible = false;
                        ((Button)e.Row.FindControl("btnConsulta")).Visible = true;
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
            string currentTheme = Page.Theme;
            DatiRegistrazioneFondo.DatiRecordFondo datiRecordFondo = (DatiRegistrazioneFondo.DatiRecordFondo)row.DataItem;

            if ((datiRecordFondo.TabDatiFondo.HasValue && datiRecordFondo.TabDatiFondo.Value == 0) ||
                (datiRecordFondo.TabDatiCalcolo.HasValue && datiRecordFondo.TabDatiCalcolo.Value == 0) ||
                (datiRecordFondo.TabDatiCalcolo707.HasValue && datiRecordFondo.TabDatiCalcolo707.Value == 0) ||
                (datiRecordFondo.TabLegge460.HasValue && datiRecordFondo.TabLegge460.Value == 0) ||
                (datiRecordFondo.TabPrivilegiate.HasValue && datiRecordFondo.TabPrivilegiate.Value == 0) ||
                (datiRecordFondo.TabArticolo2.HasValue && datiRecordFondo.TabArticolo2.Value == 0) ||
                (datiRecordFondo.TabDatiCalcoloDZ.HasValue && datiRecordFondo.TabDatiCalcoloDZ.Value == 0))
            {
                ((Image)row.FindControl("imgSemaforo")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                ((Image)row.FindControl("imgSemaforo")).ToolTip = "Record non completato";
            }
            else if ((datiRecordFondo.TabDatiFondo.HasValue && datiRecordFondo.TabDatiFondo.Value == 2) ||
                (datiRecordFondo.TabDatiCalcolo.HasValue && datiRecordFondo.TabDatiCalcolo.Value == 2) ||
                (datiRecordFondo.TabDatiCalcolo707.HasValue && datiRecordFondo.TabDatiCalcolo707.Value == 2) ||
                (datiRecordFondo.TabLegge460.HasValue && datiRecordFondo.TabLegge460.Value == 2) ||
                (datiRecordFondo.TabPrivilegiate.HasValue && datiRecordFondo.TabPrivilegiate.Value == 2) ||
                (datiRecordFondo.TabArticolo2.HasValue && datiRecordFondo.TabArticolo2.Value == 2) ||
                (datiRecordFondo.TabDatiCalcoloDZ.HasValue && datiRecordFondo.TabDatiCalcoloDZ.Value == 2))
            {
                ((Image)row.FindControl("imgSemaforo")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                ((Image)row.FindControl("imgSemaforo")).ToolTip = "Record completato";
            }
            else
            {
                ((Image)row.FindControl("imgSemaforo")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
                ((Image)row.FindControl("imgSemaforo")).ToolTip = "Record non completato";
            }

            if (datiRecordFondo.TabLegge460.HasValue)
            {
                if (datiRecordFondo.TabLegge460.Value == 2)
                    ((Image)row.FindControl("imgLegge460")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verified.png";
                else if (datiRecordFondo.TabLegge460.Value == 0)
                    ((Image)row.FindControl("imgLegge460")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso.png";
                else
                    ((Image)row.FindControl("imgLegge460")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
            }

            if (datiRecordFondo.TabArticolo2.HasValue)
            {
                if (datiRecordFondo.TabArticolo2.Value == 2)
                    ((Image)row.FindControl("imgArticolo2")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verified.png";
                else if (datiRecordFondo.TabArticolo2.Value == 0)
                    ((Image)row.FindControl("imgArticolo2")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso.png";
                else
                    ((Image)row.FindControl("imgArticolo2")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
            }

            if (datiRecordFondo.TabPrivilegiate.HasValue)
            {
                if (datiRecordFondo.TabPrivilegiate.Value == 2)
                    ((Image)row.FindControl("imgPrivilegiata")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verified.png";
                else if (datiRecordFondo.TabPrivilegiate.Value == 0)
                    ((Image)row.FindControl("imgPrivilegiata")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso.png";
                else
                    ((Image)row.FindControl("imgPrivilegiata")).ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
            }
        }

        private void RenderColumns(GridViewRow row)
        {
            DatiRegistrazioneFondo.DatiRecordFondo datiRecordFondo = (DatiRegistrazioneFondo.DatiRecordFondo)row.DataItem;

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            /* 28/10/2021 -Fondi FS/PT: per le RIC NON CONTRIBUTIVE (trattamento di famiglia, reddituali e documentali) e RIC CONTRIBUTIVE cambiata la gestione semaforica del tab legge 4/60. 
             Per queste tipologie di domande la colonna relativa al tab legge 4/60 (griglia Registrazione Fondi) deve essere visibile se c'è un record fondo che prevede il tab.
             * Per i restanti flussi tutto rimane invariato (se c'è un record che non prevede il tab, la colonna viene nascosta)*/
            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS) &&
                (Utility.IsRicostituzione_TrattamentoDiFamiglia(this.TitolarePensione.Pensione) || Utility.IsRicostituzione_Reddituale(this.TitolarePensione.Pensione) ||
                Utility.IsRicostituzione_MotiviDocumentali(this.TitolarePensione.Pensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(this.TitolarePensione.Pensione) || (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && !CodeUtility.IsRicostituzioneNonContributiva(this.TitolarePensione.Pensione))))
            {
                DatiRegistrazioneFondo.DatiRecordFondo[] listaDatiRecordFondo = (DatiRegistrazioneFondo.DatiRecordFondo[])gvRegistrazioniFondo.DataSource;
                if (listaDatiRecordFondo != null && listaDatiRecordFondo.Count() > 0)
                {
                    bool esisteRecordConDatiLegge460 = listaDatiRecordFondo.ToList().Exists(x => x.TabLegge460.HasValue);
                    if (esisteRecordConDatiLegge460)
                        gvRegistrazioniFondo.Columns[colonneGvRegistrazioniFondo.Legge460.GetHashCode()].Visible = true;
                    else
                        gvRegistrazioniFondo.Columns[colonneGvRegistrazioniFondo.Legge460.GetHashCode()].Visible = false;
                }
            }
            else
                if (!datiRecordFondo.TabLegge460.HasValue)
                    gvRegistrazioniFondo.Columns[colonneGvRegistrazioniFondo.Legge460.GetHashCode()].Visible = false;

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
            Legge460 = 2,
            Articolo2 = 3,
            Privilegiata = 4
        }

        public enum EnumViewState
        {
            ListaRecordFondo,
            DecorrenzaRegistrazione,
            IsConsultazione,
        }
    }
}