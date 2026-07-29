using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCQuotePensione : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione
    {
        public string ValidationGroupname { get; set; }

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region View State Variables

        /// <summary>
        /// Lista quote pensione contenuta nel ViewState
        /// </summary>
        private List<QuotePensioneLocal> VS_ElencoQuotePensione
        {
            get { return (List<QuotePensioneLocal>)ViewState["elencoQuotePensione"]; }
            set { ViewState["elencoQuotePensione"] = value; }

        }

        private List<QuotePensioneLocal> VS_ElencoQuotePensioneInizio
        {
            get { return (List<QuotePensioneLocal>)ViewState["elencoQuotePensioneInizio"]; }
            set { ViewState["elencoQuotePensioneInizio"] = value; }

        }
        /// <summary>
        /// Decodifica Quote
        /// </summary>
        private List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo> VS_DecEnteGestoneFondo
        {
            get { return (List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo>)ViewState["decodificaEnteGestioneFondo"]; }
            set { ViewState["decodificaEnteGestioneFondo"] = value; }

        }

        /// <summary>
        /// Decodifica Trattenute
        /// </summary>
        private List<DecCodiceTrattenute> VS_DecCodiceTrattenute
        {
            get { return (List<DecCodiceTrattenute>)ViewState["decodificaCodiceTrattenute"]; }
            set { ViewState["decodificaCodiceTrattenute"] = value; }
        }


        private bool VS_IsRicostituzione
        {
            get { return (bool)ViewState["IsRicostituzione"]; }
            set { ViewState["IsRicostituzione"] = value; }
        }

        private bool VS_IsModalitaEdit
        {
            get { return (bool)ViewState["IsModalitaEdit"]; }
            set { ViewState["IsModalitaEdit"] = value; }
        }

        private AreaDatiContributivi VS_AreaDatiContributivi
        {
            get { return (AreaDatiContributivi)ViewState["AreaDatiContributivi"]; }
            set { ViewState["AreaDatiContributivi"] = value; }
        }

        private bool VS_IsScaricoTrattenuteCumulo
        {
            get { return (bool)ViewState["IsScaricoTrattenuteCumulo"]; }
            set { ViewState["IsScaricoTrattenuteCumulo"] = value; }
        }

        #endregion View State Variables

        #region Web Form Events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void btnSalvaQuotePensione_Click(object sender, EventArgs args)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            this.areaDatiContributiviAgo = new AreaDatiContributivi();
            RecuperaCampi(this.areaDatiContributiviAgo);
            PresenterDatiContributiviAGO presenter = new PresenterDatiContributiviAGO();
            presenter.SalvaDatiCalcolo(this);

            // serve per il salvataggio completo (rivisitabile)
            try
            {
                ((Web.DatiContributiviAgo)sender).HasError = this.HasError;
                ((Web.DatiContributiviAgo)sender).ErrorMessage = this.ErrorMessage;
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
            if (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica)
                btnEliminaQuotePensione.Enabled = true;
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        public void btnEliminaQuotePensione_Click(object sender, EventArgs args)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            presenterDatiContributiviAgo.EliminaDatiCalcolo(this);

            if (!this.HasError)
            {
                RaiseInitializeData(sender, args);
            }
            else
                this.ErrorMessage = "Errore durante la la cancellazione delle Quote Pensione";
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        internal void DisabilitaPulsanti()
        {
            btnSalvaQuotePensione.Enabled = false;
            btnEliminaQuotePensione.Enabled = false;
        }
        #endregion Web Form Events

        #region Grid View Quote Pensione
        protected void gvQuotePensione_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gvQuotePensione = (GridView)sender;
                gvQuotePensione.EditIndex = e.NewEditIndex;
                VS_ElencoQuotePensione.RemoveAll(x => x.IsEmpty());
                VS_IsModalitaEdit = true;
                gvQuotePensione.DataSource = VS_ElencoQuotePensione;
                gvQuotePensione.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuotePensione, Errore nel metodo gvQuotePensione_RowEditing " + ex);
            }
        }

        protected void gvQuotePensione_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvQuotePensione = (GridView)sender;
                GridViewRow row = gvQuotePensione.Rows[e.RowIndex];
                QuotePensioneLocal quotaPensione = VS_ElencoQuotePensione[row.DataItemIndex];
                if (((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedItem.Text != quotaPensione.EnteGestioneFondo)
                    quotaPensione.ListaTrattenute = null;
                quotaPensione.EnteGestioneFondo = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedItem.Text;
                quotaPensione.IdEnteGestioneFondo = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedValue;
                decimal d = 0;
                decimal.TryParse(((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text, out d);
                quotaPensione.ImportoQuota = string.Format("{0:F7}", d);
                quotaPensione.Settimane = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtSettimane))).Text;
                quotaPensione.DescrizioneFondo = VS_DecEnteGestoneFondo.Find(x => x.Codice == quotaPensione.EnteGestioneFondo).Ente;
                if (gvQuotePensione.Columns[GVQuotePensioneColumn.Decorrenza.GetHashCode()].Visible)
                    quotaPensione.Decorrenza = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtDecorrenzaQuota))).Text;
                quotaPensione.IsTrattenute = VS_DecEnteGestoneFondo.Find(x => x.Codice == quotaPensione.EnteGestioneFondo).IsTrattenuteAmmesse.GetValueOrDefault();
                if (VS_IsScaricoTrattenuteCumulo && quotaPensione.IsTrattenute)
                {
                    if (quotaPensione.ListaTrattenute == null)
                        quotaPensione.ListaTrattenute = new List<QuotePensioneLocal.TrattenuteLocal>();
                    if (quotaPensione.ListaTrattenute.Count == 0)
                        quotaPensione.ListaTrattenute.Add(QuotePensioneLocal.TrattenuteLocal.GetEmptyTrattenuta(quotaPensione.Id));
                }
                VS_ElencoQuotePensione[e.RowIndex] = quotaPensione;
                gvQuotePensione.EditIndex = -1;
                VS_IsModalitaEdit = false;
                gvQuotePensione.DataSource = VS_ElencoQuotePensione;
                gvQuotePensione.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvQuotePensione_RowUpdating " + ex);
            }
        }

        protected void gvQuotePensione_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridView gvQuotePensione = (GridView)sender;
                VS_ElencoQuotePensione.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoQuotePensione.Count == 0)
                {
                    VS_ElencoQuotePensione.Add(QuotePensioneLocal.GetEmptyQuotaPensione());
                    gvQuotePensione.EditIndex = 0;
                }
                else
                {
                    VS_IsModalitaEdit = false;
                    gvQuotePensione.EditIndex = -1;
                }
                gvQuotePensione.DataSource = VS_ElencoQuotePensione;
                gvQuotePensione.DataBind();
                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuotePensione, Errore nel metodo gvQuotePensione_RowCancelingEdit/ " + ex);
            }
        }

        protected void gvQuotePensione_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                RaiseHideAvviso(this, null);
                if (e.CommandName == "Aggiungi")
                {
                    GridView gvQuotePensione = (GridView)sender;
                    VS_ElencoQuotePensione.Add(QuotePensioneLocal.GetEmptyQuotaPensione());
                    VS_IsModalitaEdit = true;
                    gvQuotePensione.EditIndex = VS_ElencoQuotePensione.Count - 1;
                    gvQuotePensione.DataSource = VS_ElencoQuotePensione;
                    gvQuotePensione.DataBind();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvQuotePensione_RowCommand " + ex);
            }
        }

        protected void gvQuotePensione_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvQuotePensione = (GridView)sender;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowIndex == gvQuotePensione.EditIndex)
                    {
                        LoadDdlEnteFondo(e.Row, datiPensione);
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCGvQuotePensione", Page.Theme, false);
                        ((Image)e.Row.FindControl(Utility.GetDescription(EnumControlli.imgVisualizzaTrattenute))).Visible = false;
                    }
                    else
                    {
                        QuotePensioneLocal row = VS_ElencoQuotePensione[e.Row.RowIndex];
                        CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[e.Row.Cells.Count - 2], Page.Theme, row.IsEditable, !VS_IsRicostituzione);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo))).Text = row.EnteGestioneFondo;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblDescrizioneFondo))).Text = row.DescrizioneFondo;
                        if (!datiPensione.IsDomandaCumuloAutomatica || row.Decorrenza != string.Format("{0:dd/MM/yyyy}", new DateTime(9999, 1, 1)))
                            ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblDecorrenzaQuota))).Text = row.Decorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblValueDecorrenzaQuota))).Text = row.Decorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblSettimane))).Text = row.Settimane;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota))).Text = row.ImportoQuota;
                        if (VS_IsScaricoTrattenuteCumulo)
                        {
                            List<QuotePensioneLocal.TrattenuteLocal> listaTrattenute = row.ListaTrattenute;
                            if (!datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica
                                && !((Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria)) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)))
                            {
                                ((Image)e.Row.FindControl(Utility.GetDescription(EnumControlli.imgVisualizzaTrattenute))).Visible = row.IsTrattenute;
                                if (row.IsTrattenute)
                                {
                                    if (listaTrattenute == null || listaTrattenute.Count == 0)
                                    {
                                        listaTrattenute = new List<QuotePensioneLocal.TrattenuteLocal>();
                                        listaTrattenute.Add(QuotePensioneLocal.TrattenuteLocal.GetEmptyTrattenuta(row.Id));
                                        VS_ElencoQuotePensione[e.Row.RowIndex].ListaTrattenute = listaTrattenute;
                                    }
                                    GridView gvTrattenute = ((GridView)e.Row.FindControl(Utility.GetDescription(EnumControlli.gvTrattenute)));
                                    if (listaTrattenute.Count == 1 && listaTrattenute[0].IsEmpty())
                                        gvTrattenute.EditIndex = 0;
                                    gvTrattenute.DataSource = VS_ElencoQuotePensione[e.Row.RowIndex].ListaTrattenute;
                                    gvTrattenute.DataBind();
                                }
                            }
                            else
                            {
                                if (listaTrattenute != null && listaTrattenute.Count > 0)
                                {
                                    ((Image)e.Row.FindControl(Utility.GetDescription(EnumControlli.imgVisualizzaTrattenute))).Visible = true;
                                    GridView gvTrattenute = ((GridView)e.Row.FindControl(Utility.GetDescription(EnumControlli.gvTrattenute)));
                                    gvTrattenute.DataSource = listaTrattenute;
                                    gvTrattenute.DataBind();
                                }
                                else
                                    ((Image)e.Row.FindControl(Utility.GetDescription(EnumControlli.imgVisualizzaTrattenute))).Visible = false;
                            }
                        }
                    }
                }
                else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Visible = !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && gvQuotePensione.EditIndex == -1;
                    if (!datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && e.Row.Visible)
                    {
                        foreach (TableCell cell in e.Row.Cells)
                        {
                            switch (e.Row.Cells.GetCellIndex(cell))
                            {
                                case 1:
                                    LinkButton add = (LinkButton)cell.FindControl("btnAggiungiQuote");
                                    add.Text = "<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                    add.ToolTip = "Aggiungi";
                                    break;
                                case 2:
                                    cell.ColumnSpan = e.Row.Cells.Count - 2;
                                    break;
                                default:
                                    cell.Visible = false;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvQuotePensione_RowDataBound " + ex);
            }
        }

        protected void gvQuotePensione_DataBinding(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GridView gvQuotePensione = (GridView)sender;
            gvQuotePensione.ShowFooter = !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && gvQuotePensione.EditIndex == -1;
        }

        protected void gvQuotePensione_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            this.areaDatiContributiviAgo = VS_AreaDatiContributivi;
            GridView gvQuotePensione = (GridView)sender;
            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.TipoCumulo.HasValue && !this.areaDatiContributiviAgo.TipoCumulo.Value)
                if (!(Utility.IsDomandaIOCUM(this.domanda.Categoria) || Utility.IsDomandaSOCUM(this.domanda.Categoria)))
                    gvQuotePensione.Columns[GVQuotePensioneColumn.Decorrenza.GetHashCode()].Visible = true;
            gvQuotePensione.Columns[GVQuotePensioneColumn.ModificaQuota.GetHashCode()].Visible = (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica);
            gvQuotePensione.Columns[GVQuotePensioneColumn.EliminaQuota.GetHashCode()].Visible = !VS_IsRicostituzione && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica;
            gvQuotePensione.Columns[GVQuotePensioneColumn.VisualizzaTrattenute.GetHashCode()].Visible = IsVisualizzaTrattenute();
            ManagePulsanti();
        }

        protected void gvQuotePensione_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gvQuotePensione = (GridView)sender;
                VS_ElencoQuotePensione.RemoveAt(e.RowIndex);
                //rimozione di eventuali record vuoti
                VS_ElencoQuotePensione.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoQuotePensione.Count == 0)
                {
                    VS_ElencoQuotePensione.Add(QuotePensioneLocal.GetEmptyQuotaPensione());
                    gvQuotePensione.EditIndex = 0;
                    VS_IsModalitaEdit = true;
                }
                else
                    gvQuotePensione.EditIndex = -1;
                gvQuotePensione.DataSource = VS_ElencoQuotePensione;
                gvQuotePensione.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowDeleting " + ex);
            }
        }

        protected void gvTrattenute_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GridView gvTrattenute = (GridView)sender;
            gvTrattenute.Columns[GVTrattenuteColumn.ModificaTrattenute.GetHashCode()].Visible = (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica);
            gvTrattenute.Columns[GVTrattenuteColumn.EliminaTrattenute.GetHashCode()].Visible = !VS_IsRicostituzione && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica;
        }

        protected void gvTrattenute_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Aggiungi")
            {
                GridView gvTrattenute = (GridView)sender;
                string strIdQuota = ((Label)gvTrattenute.Rows[0].FindControl(Utility.GetDescription(EnumControlli.lblIdQuota))).Text;
                Guid idQuota = new Guid(strIdQuota);
                VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.Add(QuotePensioneLocal.TrattenuteLocal.GetEmptyTrattenuta(idQuota));
                gvTrattenute.EditIndex = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.Count - 1;
                gvTrattenute.DataSource = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute;
                gvTrattenute.DataBind();
            }
        }

        protected void gvTrattenute_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gvTrattenute = (GridView)sender;
                GridViewRow row = gvTrattenute.Rows[e.NewEditIndex];
                if (row != null)
                {
                    Guid idQuota = new Guid(((Label)(row.FindControl(Utility.GetDescription(EnumControlli.lblIdQuota)))).Text);
                    VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.RemoveAll(x => x.IsEmpty());
                    gvTrattenute.DataSource = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute;
                    gvTrattenute.EditIndex = e.NewEditIndex;
                    gvTrattenute.DataBind();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowEditing " + ex);
            }
        }

        protected void gvTrattenute_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridView gvTrattenute = (GridView)sender;
                GridViewRow row = gvTrattenute.Rows[e.RowIndex];
                Guid idQuota = new Guid(((Label)(row.FindControl(Utility.GetDescription(EnumControlli.lblIdQuota)))).Text);
                VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.Count == 0)
                {
                    VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.Add(QuotePensioneLocal.TrattenuteLocal.GetEmptyTrattenuta(idQuota));
                    gvTrattenute.EditIndex = 0;
                }
                else
                    gvTrattenute.EditIndex = -1;
                gvTrattenute.DataSource = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute;
                gvTrattenute.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowCancelingEdit " + ex);
            }
        }

        protected void gvTrattenute_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridView gvTrattenute = (GridView)sender;
                GridViewRow row = gvTrattenute.Rows[e.RowIndex];
                Guid idQuota = new Guid(((Label)(row.FindControl(Utility.GetDescription(EnumControlli.lblIdQuota)))).Text);
                QuotePensioneLocal.TrattenuteLocal trattenute = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute[e.RowIndex];
                trattenute.IdQuota = idQuota;
                trattenute.AnnoCompetenza = ((TextBox)(row.FindControl(Utility.GetDescription(EnumControlli.txtAnnoCompetenza)))).Text;
                trattenute.CodiceTrattenute = ((DropDownList)(row.FindControl(Utility.GetDescription(EnumControlli.ddlCodiceTrattenute)))).SelectedValue;
                trattenute.ImportoTrattenute = ((TextBox)(row.FindControl(Utility.GetDescription(EnumControlli.txtImportoTrattenute)))).Text;
                VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute[e.RowIndex] = trattenute;
                gvTrattenute.DataSource = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute;
                gvTrattenute.EditIndex = -1;
                gvTrattenute.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowUpdating " + ex);
            }
        }

        protected void gvTrattenute_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                if (!datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica)
                {
                    GridView gvTrattenute = (GridView)sender;
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        QuotePensioneLocal.TrattenuteLocal row = e.Row.DataItem as QuotePensioneLocal.TrattenuteLocal;
                        QuotePensioneLocal quota = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == row.IdQuota);
                        if (e.Row.RowIndex == gvTrattenute.EditIndex)
                        {
                            LoadDdlCodiceTrattenute(e.Row, quota);
                            this.ValidationGroupname = "UCGvTrattenute" + row.IdQuota.ToString();
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], this.ValidationGroupname, Page.Theme, false);
                            ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RFVtxtAnnoCompetenza))).ValidationGroup = this.ValidationGroupname;
                            ((RegularExpressionValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.REVtxtAnnoCompetenza))).ValidationGroup = this.ValidationGroupname;
                            ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RFVddlCodiceTrattenute))).ValidationGroup = this.ValidationGroupname;
                            ((RegularExpressionValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.REVtxtImportoTrattenute))).ValidationGroup = this.ValidationGroupname;
                            ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RFVtxtImportoTrattenute))).ValidationGroup = this.ValidationGroupname;
                            RaiseAddValidationGroupname(this, null);
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[e.Row.Cells.Count - 1], Page.Theme, quota.IsEditable, !VS_IsRicostituzione);
                        }
                    }
                    else if (e.Row.RowType == DataControlRowType.Footer)
                    {
                        if (!datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && e.Row.Visible)
                        {
                            foreach (TableCell cell in e.Row.Cells)
                            {
                                switch (e.Row.Cells.GetCellIndex(cell))
                                {
                                    case 1:
                                        LinkButton add = (LinkButton)cell.FindControl("btnAggiungiTrattenute");
                                        add.Text = "<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                        add.ToolTip = "Aggiungi";
                                        break;
                                    case 2:
                                        cell.ColumnSpan = e.Row.Cells.Count - 2;
                                        break;
                                    default:
                                        cell.Visible = false;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowDataBound " + ex);
            }
        }

        protected void gvTrattenute_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gvTrattenute = (GridView)sender;
                GridViewRow row = gvTrattenute.Rows[e.RowIndex];
                if (row != null)
                {
                    Guid idQuota = new Guid(((Label)(row.FindControl(Utility.GetDescription(EnumControlli.lblIdQuota)))).Text);
                    VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.RemoveAt(e.RowIndex);
                    //rimozione di eventuali record vuoti
                    VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.RemoveAll(x => x.IsEmpty());
                    if (VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.Count == 0)
                    {
                        VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute.Add(QuotePensioneLocal.TrattenuteLocal.GetEmptyTrattenuta(idQuota));
                        gvTrattenute.EditIndex = 0;
                    }
                    else
                        gvTrattenute.EditIndex = -1;
                    gvTrattenute.DataSource = VS_ElencoQuotePensione.FirstOrDefault(x => x.Id == idQuota).ListaTrattenute;
                    gvTrattenute.DataBind();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowDeleting " + ex);
            }
        }

        private bool IsVisualizzaTrattenute()
        {
            bool isVisible = false;
            if (VS_IsScaricoTrattenuteCumulo)
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                if (!datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica)
                {
                    if (VS_IsRicostituzione)
                        isVisible = VS_ElencoQuotePensione != null && VS_ElencoQuotePensione.Count > 0 && VS_ElencoQuotePensione.Any(x => x.IsTrattenute && x.ListaTrattenute != null && x.ListaTrattenute.Count > 0);
                    else
                        isVisible = true;
                }
                else
                    isVisible = VS_ElencoQuotePensione != null && VS_ElencoQuotePensione.Count > 0 && VS_ElencoQuotePensione.Any(x => x.ListaTrattenute != null && x.ListaTrattenute.Count > 0);
            }
            return isVisible;
        }

        protected void gvTrattenute_DataBinding(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GridView gvTrattenute = (GridView)sender;
            gvTrattenute.ShowFooter = !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && gvTrattenute.EditIndex == -1;
        }

        private bool IsEmptyEditableRowQuotaPensione(GridViewRow row)
        {
            if (row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.txtSettimane)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo)) != null && ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        private bool IsEmptyRedableRowQuotaPensione(GridViewRow row)
        {
            if (row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.lblSettimane)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.lblSettimane))).Text != string.Empty
               )
                return false;
            else
                return true;
        }

        #endregion Grid View Quote Pensione

        #region Private Methods

        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            if (areaDatiContributiviAgo.DatiCalcoloQuotePensione == null)
                areaDatiContributiviAgo.DatiCalcoloQuotePensione = new GestioneContribDatiCalcoloQuotePensione();
            areaDatiContributiviAgo.DatiCalcoloQuotePensione.LQuotePensione = MapLocalToServiceObject(VS_ElencoQuotePensione).ToArray();

            //ENG - MEMO 74_2023
            if (pnlContributiItaEdEsteriAl1295.Visible == true)
            {
                if (!string.IsNullOrEmpty(txtContributiItalianiEsteri.Text))
                    areaDatiContributiviAgo.DatiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295 = int.Parse(txtContributiItalianiEsteri.Text);
                else
                    areaDatiContributiviAgo.DatiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295 = null;
            }
        }

        private void ManagePulsanti()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (VS_ElencoQuotePensione != null && VS_ElencoQuotePensione.Where(x => !x.IsEmpty()).Count() > 0 && !VS_IsModalitaEdit)
            {
                btnSalvaQuotePensione.Enabled = true;
                RaiseGestisciTastoSalva(this, null);
            }
            else
            {
                btnSalvaQuotePensione.Enabled = false;
                RaiseGestisciTastoSalva(this, null);
            }


            if (VS_ElencoQuotePensione != null && VS_ElencoQuotePensione.Where(x => !x.IsEmpty()).Count() > 0 &&
                VS_IsRicostituzione == false && !VS_IsModalitaEdit)
                btnEliminaQuotePensione.Enabled = true;
            else
                btnEliminaQuotePensione.Enabled = false;

            if (this.domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S"))
                btnEliminaQuotePensione.Enabled = false;

            if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(this.TitolarePensione.Pensione) || this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica == true ||
                this.TitolarePensione.Pensione.IsDomandaTotAutomatica == true || Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                btnEliminaQuotePensione.Enabled = false;
        }

        public void ValorizzaEtichette(IDatiContributiviAgo idatiContributivi)
        {
            AreaDatiContributivi areaDatiContrib = idatiContributivi.areaDatiContributiviAgo;
            VS_AreaDatiContributivi = areaDatiContrib;
            VS_DecEnteGestoneFondo = areaDatiContrib.listaDecEnteGestioneFondo != null ? areaDatiContrib.listaDecEnteGestioneFondo.ToList() : null;
            VS_DecCodiceTrattenute = areaDatiContrib.ListaDecCodiceTrattenute != null ? areaDatiContrib.ListaDecCodiceTrattenute.ToList() : null;
            VS_IsScaricoTrattenuteCumulo = areaDatiContributiviAgo.IsScaricoTrattenuteCumulo.GetValueOrDefault();
            SetViewStateIsRicostituzione();
            if (areaDatiContrib.DatiCalcoloQuotePensione != null && areaDatiContrib.DatiCalcoloQuotePensione.LQuotePensione != null)
            {
                VS_IsModalitaEdit = false;
                InitGvQuotePensione(areaDatiContrib.DatiCalcoloQuotePensione.LQuotePensione.ToList());
            }
            else // dati non presenti
            {
                VS_IsModalitaEdit = true;
                InitGvQuotePensione(null);
            }
            ManagePulsanti();

            //ENG - MEMO 74_2023
            if (areaDatiContrib != null && areaDatiContrib.IsMemo74_2023Abilitato.GetValueOrDefault())
            {
                pnlContributiItaEdEsteriAl1295.Visible = true;
                if (areaDatiContrib.DatiCalcoloQuotePensione != null && areaDatiContrib.DatiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295 != null)
                    txtContributiItalianiEsteri.Text = areaDatiContrib.DatiCalcoloQuotePensione.ContributiItalianiEdEsteriAl1295.ToString();
                else
                    txtContributiItalianiEsteri.Text = string.Empty;
            }
        }

        private void InitGvQuotePensione(List<GestioneContribDatiQuotePensione> lstServer)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            List<QuotePensioneLocal> lstQuotePensione = new List<QuotePensioneLocal>();

            if (lstServer != null && lstServer.Count() > 0)
            {
                lstQuotePensione.AddRange(MapServiceToLocalObject(lstServer));
            }
            if (lstQuotePensione.Count == 0 && !VS_IsRicostituzione && !(Utility.IsDomandaCumulo(this.domanda.Categoria) && datiPensione.IsDomandaCumuloAutomatica) && !(Utility.IsDomandaTotalizzazione(this.domanda.Categoria) && datiPensione.IsDomandaTotAutomatica))
                lstQuotePensione.Add(QuotePensioneLocal.GetEmptyQuotaPensione());
            VS_ElencoQuotePensione = lstQuotePensione;
            //View state dove salvo la lista quote al primo caricamento del quadro (non verrà modificata)
            VS_ElencoQuotePensioneInizio = lstQuotePensione;
            if (lstQuotePensione.Count == 1 && lstQuotePensione[0].IsEmpty())
                gvQuotePensione.EditIndex = 0;
            gvQuotePensione.DataSource = lstQuotePensione;
            gvQuotePensione.DataBind();
        }

        private void LoadDdlEnteFondo(GridViewRow row, AreaTitolare.DatiPensione datiPensione)
        {
            string ctrlAbilitazioneMemo93 = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo93", out ctrlAbilitazioneMemo93);

            DropDownList ddlEnteGestioneQuota = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo));
            ddlEnteGestioneQuota.Items.Add(new ListItem(string.Empty, string.Empty));
            List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo> listaEnteFondo = VS_DecEnteGestoneFondo;
            this.areaDatiContributiviAgo = VS_AreaDatiContributivi;
            IEnumerable<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo> listaOrdinata = listaEnteFondo.OrderBy(x => x.Codice);
            var listaDaRimuovere = new List<string>();

            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(ctrlAbilitazioneMemo93) && ctrlAbilitazioneMemo93.Trim().ToUpperInvariant() == "SI")
            {
                if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.TipoCumulo.HasValue && this.areaDatiContributiviAgo.TipoCumulo.Value)
                    listaDaRimuovere.AddRange(new List<string> { "F1", "G0", "H0", "I0", "J0", "K0", "L0", "N0", "O0", "P0", "Q0", "R0", "S0", "T0", "U0", "V0", "Z0", "Z1", "PR", "SI" });
            }
            else
            {
                if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.TipoCumulo.HasValue && this.areaDatiContributiviAgo.TipoCumulo.Value)
                    listaDaRimuovere.AddRange(new List<string> { "F0", "F1", "G0", "H0", "I0", "J0", "K0", "L0", "N0", "O0", "P0", "Q0", "R0", "S0", "T0", "U0", "V0", "Z0", "Z1", "PR" });
            }

            var sblocca = false;
            if ((Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria)) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                Dictionary<string, List<string>> codiciDaSostituire = new Dictionary<string, List<string>>();
                codiciDaSostituire.Add("C0", new List<string>() { "C1", "C2", "C3", "C4", "C5" });
                codiciDaSostituire.Add("E0", new List<string>() { "E1", "E2" });
                codiciDaSostituire.Add("D0", new List<string>() { "D1" });
                //Considero anche la lista quote prima della modifica per poter sbloccare le quote C0,D0 ed E0 anche se sono state modificate
                //Per questa tipologia di domande non è possibile aggiungere/cancellare quote, quindi l'indice rimane lo stesso
                if (codiciDaSostituire.ContainsKey(((QuotePensioneLocal)(row.DataItem)).EnteGestioneFondo) || codiciDaSostituire.ContainsKey(VS_ElencoQuotePensioneInizio.ElementAt(row.DataItemIndex).EnteGestioneFondo))
                {
                    var listaCodiciDaMostrare = codiciDaSostituire.FirstOrDefault(y => ((QuotePensioneLocal)(row.DataItem)).EnteGestioneFondo == y.Key || VS_ElencoQuotePensioneInizio.ElementAt(row.DataItemIndex).EnteGestioneFondo == y.Key).Value;
                    listaOrdinata = listaOrdinata.Where(x => listaCodiciDaMostrare.Contains(x.Codice));
                    ddlEnteGestioneQuota.Enabled = true;
                    sblocca = true;
                }
            }

            foreach (Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo enteFondo in listaOrdinata)
            {
                if (!listaDaRimuovere.Contains(enteFondo.Codice))
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", enteFondo.Ente);
                    li.Text = enteFondo.Codice;
                    li.Value = enteFondo.Id.ToString();
                    ddlEnteGestioneQuota.Items.Add(li);
                }
            }
            if (((QuotePensioneLocal)(row.DataItem)).IdEnteGestioneFondo.Trim() == string.Empty || ddlEnteGestioneQuota.Items.FindByValue(((QuotePensioneLocal)(row.DataItem)).IdEnteGestioneFondo.Trim()) == null)
                ddlEnteGestioneQuota.SelectedIndex = 0;
            else
                ddlEnteGestioneQuota.Items.FindByValue(((QuotePensioneLocal)(row.DataItem)).IdEnteGestioneFondo.Trim()).Selected = true;

            if (VS_IsRicostituzione && !sblocca)
                ddlEnteGestioneQuota.Enabled = false;
        }

        private void LoadDdlCodiceTrattenute(GridViewRow row, QuotePensioneLocal quota)
        {
            DropDownList ddlCodiceTrattenute = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlCodiceTrattenute));
            List<DecCodiceTrattenute> listaCodiceTrattenuteOrdinata = VS_DecCodiceTrattenute.OrderBy(x => x.CodiceTrattenute).ToList();
            foreach (DecCodiceTrattenute codiceTrattenute in listaCodiceTrattenuteOrdinata.FindAll(x => x.CodiceEnteGestioneFondo == quota.EnteGestioneFondo))
            {
                ListItem li = new ListItem();
                li.Attributes.Add("title", codiceTrattenute.CodiceTrattenute);
                li.Text = codiceTrattenute.CodiceTrattenute;
                li.Value = codiceTrattenute.CodiceTrattenute;
                ddlCodiceTrattenute.Items.Add(li);
            }
            if (((QuotePensioneLocal.TrattenuteLocal)(row.DataItem)).CodiceTrattenute.Trim() == string.Empty)
            {
                if (ddlCodiceTrattenute.Items.Count == 2)
                    ddlCodiceTrattenute.SelectedIndex = 1;
                else
                    ddlCodiceTrattenute.SelectedIndex = 0;
            }
            else
                ddlCodiceTrattenute.Items.FindByValue(((QuotePensioneLocal.TrattenuteLocal)(row.DataItem)).CodiceTrattenute.Trim()).Selected = true;

            if (VS_IsRicostituzione)
                ddlCodiceTrattenute.Enabled = false;
        }

        public List<GestioneContribDatiQuotePensione> MapLocalToServiceObject(List<QuotePensioneLocal> lstLocal)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<GestioneContribDatiQuotePensione> lstService = new List<GestioneContribDatiQuotePensione>();
            if (lstLocal != null && lstLocal.Count() > 0)
            {
                foreach (QuotePensioneLocal elem in lstLocal)
                {
                    if (!elem.IsEmpty())
                    {
                        GestioneContribDatiQuotePensione elemService = new GestioneContribDatiQuotePensione();
                        elemService.Importo = decimal.Parse(elem.ImportoQuota);
                        elemService.Settimane = int.Parse(elem.Settimane);
                        elemService.EnteGestioneFondo = long.Parse(elem.IdEnteGestioneFondo);
                        elemService.Decorrenza = Utility.GetDateFromString(elem.Decorrenza);
                        elemService.IsQuotaProgressiva = CodeUtility.IsRicostituzioneCumuloProgressiva(datiPensione, this.domanda.Categoria) ? elem.IsEditable : !elem.IsEditable;
                        if (elem.ListaTrattenute != null && elem.ListaTrattenute.Count > 0)
                        {
                            List<GestioneContribDatiQuotePensione.DatiTrattenute> listaTrattenute = new List<GestioneContribDatiQuotePensione.DatiTrattenute>();
                            foreach (QuotePensioneLocal.TrattenuteLocal subElem in elem.ListaTrattenute)
                            {
                                if (!subElem.IsEmpty())
                                {
                                    GestioneContribDatiQuotePensione.DatiTrattenute subElemService = new GestioneContribDatiQuotePensione.DatiTrattenute();
                                    subElemService.AnnoCompetenza = short.Parse(subElem.AnnoCompetenza);
                                    subElemService.CodiceTrattenute = subElem.CodiceTrattenute;
                                    subElemService.ImportoTrattenute = decimal.Parse(subElem.ImportoTrattenute);
                                    listaTrattenute.Add(subElemService);
                                }
                            }
                            if (listaTrattenute.Count > 0)
                                elemService.ListaTrattenute = listaTrattenute.ToArray();
                        }
                        lstService.Add(elemService);
                    }
                }
            }
            return lstService;
        }

        public List<QuotePensioneLocal> MapServiceToLocalObject(List<GestioneContribDatiQuotePensione> lstService)
        {
            List<QuotePensioneLocal> lstLocal = new List<QuotePensioneLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (GestioneContribDatiQuotePensione elemS in lstService)
                {
                    QuotePensioneLocal elemL = new QuotePensioneLocal();
                    elemL.ImportoQuota = elemS.Importo.ToString();
                    elemL.Settimane = elemS.Settimane.ToString();
                    elemL.EnteGestioneFondo = VS_DecEnteGestoneFondo.Find(x => x.Id == elemS.EnteGestioneFondo) != null ? VS_DecEnteGestoneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Codice : string.Empty;
                    elemL.DescrizioneFondo = VS_DecEnteGestoneFondo.Find(x => x.Id == elemS.EnteGestioneFondo) != null ? VS_DecEnteGestoneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Ente : string.Empty;
                    elemL.IsTrattenute = VS_DecEnteGestoneFondo.Find(x => x.Id == elemS.EnteGestioneFondo) != null ? VS_DecEnteGestoneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).IsTrattenuteAmmesse.GetValueOrDefault() : false;
                    elemL.IdEnteGestioneFondo = elemS.EnteGestioneFondo.ToString();
                    elemL.Decorrenza = string.Format("{0:dd/MM/yyyy}", elemS.Decorrenza);
                    elemL.IsEditable = IsButtonEditVisible(elemS.IsQuotaProgressiva);
                    if (elemS.ListaTrattenute != null && elemS.ListaTrattenute.Count() > 0)
                    {
                        elemL.ListaTrattenute = new List<QuotePensioneLocal.TrattenuteLocal>();
                        foreach (GestioneContribDatiQuotePensione.DatiTrattenute subElemS in elemS.ListaTrattenute)
                        {
                            QuotePensioneLocal.TrattenuteLocal subElemL = new QuotePensioneLocal.TrattenuteLocal();
                            subElemL.IdQuota = elemL.Id;
                            subElemL.AnnoCompetenza = subElemS.AnnoCompetenza.ToString();
                            subElemL.CodiceTrattenute = subElemS.CodiceTrattenute;
                            subElemL.ImportoTrattenute = subElemS.ImportoTrattenute.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                            elemL.ListaTrattenute.Add(subElemL);
                        }
                    }
                    lstLocal.Add(elemL);
                }
            }
            return lstLocal;
        }

        internal void SetViewStateIsRicostituzione()
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                VS_IsRicostituzione = true;
            else
                VS_IsRicostituzione = false;
        }

        public bool IsButtonEditVisible(bool isQuotaProgressiva)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (isQuotaProgressiva)
            {
                if (Utility.IsDomandaVOCUM(this.domanda.Categoria) && CodeUtility.IsRicostituzioneCumuloProgressiva(datiPensione, this.domanda.Categoria))
                    return true;
                else
                    return false;
            }
            else
            {
                if (Utility.IsDomandaVOCUM(this.domanda.Categoria) && CodeUtility.IsRicostituzioneCumuloProgressiva(datiPensione, this.domanda.Categoria))
                    return false;
                else
                    return true;
            }
        }

        #endregion Private Methods

        #region Enum

        public enum EnumControlli
        {
            [Description("lblEnteGestioneFondo_item")]
            lblEnteGestioneFondo,
            [Description("ddlEnteGestioneFondo")]
            ddlEnteGestioneFondo,
            [Description("lblSettimane_item")]
            lblSettimane,
            [Description("txtSettimane")]
            txtSettimane,
            [Description("lblImportoQuota")]
            lblImportoQuota,
            [Description("txtImportoQuota")]
            txtImportoQuota,
            [Description("lblIdCodeGestione")]
            lblIdCodeGestione,
            [Description("btnEliminaQuotePensioni")]
            btnEliminaQuotePensioni,
            [Description("lblDescrizioneFondo_item")]
            lblDescrizioneFondo,
            [Description("lblDecorrenzaQuota_item")]
            lblDecorrenzaQuota,
            [Description("txtDecorrenzaQuota")]
            txtDecorrenzaQuota,
            [Description("lblValueDecorrenzaQuota")]
            lblValueDecorrenzaQuota,
            [Description("gvTrattenute")]
            gvTrattenute,
            [Description("imgVisualizzaTrattenute")]
            imgVisualizzaTrattenute,
            [Description("lblIdQuota")]
            lblIdQuota,
            [Description("txtAnnoCompetenza")]
            txtAnnoCompetenza,
            [Description("txtImportoTrattenute")]
            txtImportoTrattenute,
            [Description("REVtxtAnnoCompetenza")]
            REVtxtAnnoCompetenza,
            [Description("RFVtxtAnnoCompetenza")]
            RFVtxtAnnoCompetenza,
            [Description("REVtxtImportoTrattenute")]
            REVtxtImportoTrattenute,
            [Description("RFVtxtImportoTrattenute")]
            RFVtxtImportoTrattenute,
            [Description("RFVddlCodiceTrattenute")]
            RFVddlCodiceTrattenute,
            [Description("ddlCodiceTrattenute")]
            ddlCodiceTrattenute,
        }

        public enum GVQuotePensioneColumn
        {
            ModificaQuota = 0,
            Decorrenza = 3,
            EliminaQuota = 6,
            VisualizzaTrattenute
        }

        public enum GVTrattenuteColumn
        {
            ModificaTrattenute = 0,
            EliminaTrattenute = 5,
        }

        #endregion Enum

        #region User Control Events
        public event EventHandler ShowAvviso;
        public event EventHandler InitializeData;
        public event EventHandler GestisciTastoSalva;
        public event EventHandler HideAvviso;
        public event EventHandler AddValidationGroupname;

        protected void RaiseGestisciTastoSalva(object sender, EventArgs e)
        {
            if (GestisciTastoSalva != null)
                GestisciTastoSalva(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseAddValidationGroupname(object sender, EventArgs e)
        {
            if (AddValidationGroupname != null)
                AddValidationGroupname(sender, e);
        }

        #endregion User Control Events

        #region Nestled Class
        [Serializable]
        public class QuotePensioneLocal
        {
            public QuotePensioneLocal()
            {
                this.Id = Guid.NewGuid();
                this.IsEditable = true;
            }
            public QuotePensioneLocal(string enteGestioneFondo, string settimane, string importoQuota, string id, string descrizioneFondo, string decorrenza, bool isTrattenute = false)
            {
                this.Id = Guid.NewGuid();
                this.IdEnteGestioneFondo = id;
                this.ImportoQuota = importoQuota;
                this.Settimane = settimane;
                this.EnteGestioneFondo = enteGestioneFondo;
                this.DescrizioneFondo = descrizioneFondo;
                this.Decorrenza = decorrenza;
                this.IsTrattenute = isTrattenute;
                this.IsEditable = true;
            }
            public bool IsEmpty()
            {
                bool ret = false;
                if (string.IsNullOrEmpty(this.IdEnteGestioneFondo) && string.IsNullOrEmpty(this.ImportoQuota) && string.IsNullOrEmpty(this.Settimane) && string.IsNullOrEmpty(this.EnteGestioneFondo) &&
                    string.IsNullOrEmpty(this.Decorrenza))
                    ret = true;
                return ret;
            }

            public Guid Id { get; private set; }
            public string IdEnteGestioneFondo { get; set; }
            public string Settimane { get; set; }
            public string ImportoQuota { get; set; }
            public string EnteGestioneFondo { get; set; }
            public string DescrizioneFondo { get; set; }
            public string Decorrenza { get; set; }
            public bool IsTrattenute { get; set; }
            public bool IsEditable { get; set; }
            public List<TrattenuteLocal> ListaTrattenute { get; set; }

            public static QuotePensioneLocal GetEmptyQuotaPensione()
            {
                return new QuotePensioneLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            }

            [Serializable]
            public class TrattenuteLocal
            {
                public Guid? IdQuota { get; set; }
                public string AnnoCompetenza { get; set; }
                public string CodiceTrattenute { get; set; }
                public string ImportoTrattenute { get; set; }
                public bool IsEmpty()
                {
                    bool ret = false;
                    if (string.IsNullOrEmpty(this.AnnoCompetenza) && string.IsNullOrEmpty(this.CodiceTrattenute) && string.IsNullOrEmpty(ImportoTrattenute))
                        ret = true;
                    return ret;
                }

                public TrattenuteLocal() { }
                public TrattenuteLocal(Guid? idquota, string annoCompetenza, string codiceTrattenute, string importoTrattenute)
                {
                    this.IdQuota = idquota;
                    this.AnnoCompetenza = annoCompetenza;
                    this.CodiceTrattenute = codiceTrattenute;
                    this.ImportoTrattenute = importoTrattenute;
                }

                internal static TrattenuteLocal GetEmptyTrattenuta(Guid? idQuota)
                {
                    return new TrattenuteLocal(idQuota, string.Empty, string.Empty, string.Empty);
                }
            }

        }
        #endregion Nestled Class
    }
}
