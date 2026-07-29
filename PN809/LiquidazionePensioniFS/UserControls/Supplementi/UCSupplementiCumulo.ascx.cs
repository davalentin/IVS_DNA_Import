using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi
{
    public partial class UCSupplementiCumulo : CustomBaseUserControl, ISupplementi, ITitolarePensione
    {
        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region ISupplementi
        public long numDomanda { get; set; }
        public AreaSupplementi lstSupplementi { get; set; }
        public Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ISupplementi

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region View State Variables

        /// <summary>
        /// Lista quote Supplementi contenuta nel ViewState
        /// </summary>
        private List<QuoteSupplementiLocal> VS_ElencoQuoteSupplementi
        {
            get { return (List<QuoteSupplementiLocal>)ViewState["elencoQuoteSupplementi"]; }
            set { ViewState["elencoQuoteSupplementi"] = value; }

        }

        /// <summary>
        /// Decodifica Quote
        /// </summary>
        private List<Presenter.SvrLiquidazione.DecEnteGestioneFondo> VS_DecEnteGestioneFondo
        {
            get { return (List<Presenter.SvrLiquidazione.DecEnteGestioneFondo>)ViewState["decodificaEnteGestioneFondo"]; }
            set { ViewState["decodificaEnteGestioneFondo"] = value; }

        }

        private bool VS_IsModalitaEdit
        {
            get { return (bool)ViewState["IsModalitaEdit"]; }
            set { ViewState["IsModalitaEdit"] = value; }
        }

        private bool VS_IsRicostituzione
        {
            get { return (bool)ViewState["IsRicostituzione"]; }
            set { ViewState["IsRicostituzione"] = value; }
        }
        #endregion View State Variables

        protected void Page_Load(object sender, EventArgs e)
        {
            RaiseHideAvviso(this, null);
        }

        #region User Control Events
        public event Utility.CustomEventHandler SalvaSupplementi;
        public event Utility.CustomEventHandler EliminaSupplementi;
        public event EventHandler ErrorSalvaSupplementi;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler ShowPopUp;
        public event EventHandler HidePopUp;
        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;
        public event EventHandler InitializeData;

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        protected void RaiseDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (DisabilitaTastoSalva != null)
                DisabilitaTastoSalva(sender, e);
        }

        protected void RaiseSalvaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            if (SalvaSupplementi != null)
                SalvaSupplementi(sender, e);
        }

        protected void RaiseEliminaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            if (EliminaSupplementi != null)
                EliminaSupplementi(sender, e);
        }

        protected void RaiseErrorSalvaSupplementi(object sender, EventArgs e)
        {
            if (ErrorSalvaSupplementi != null)
                ErrorSalvaSupplementi(sender, e);
        }

        protected void RaiseShowPopUp(object sender, EventArgs e)
        {
            if (ShowPopUp != null)
                ShowPopUp(sender, e);
        }

        protected void RaiseHidePopUp(object sender, EventArgs e)
        {
            if (HidePopUp != null)
                HidePopUp(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            if (InitializeData != null)
                InitializeData(sender, e);
        }
        #endregion User Control Events

        #region Grid Quote Supplementi
        protected void gvQuoteSupplementi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gvQuoteSupplementi = (GridView)sender;
                gvQuoteSupplementi.EditIndex = e.NewEditIndex;
                VS_ElencoQuoteSupplementi.RemoveAll(x => x.IsEmpty());
                VS_IsModalitaEdit = true;
                gvQuoteSupplementi.DataSource = VS_ElencoQuoteSupplementi;
                gvQuoteSupplementi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuoteSupplementi, Errore nel metodo gvQuoteSupplementi_RowEditing " + ex);
            }
        }

        protected void gvQuoteSupplementi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvQuoteSupplementi = (GridView)sender;
                GridViewRow row = gvQuoteSupplementi.Rows[e.RowIndex];
                QuoteSupplementiLocal quotaSupplementi = VS_ElencoQuoteSupplementi[e.RowIndex];
                quotaSupplementi.EnteGestioneFondo = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedItem.Text;
                quotaSupplementi.IdEnteGestioneFondo = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedValue;
                quotaSupplementi.ImportoQuota = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text;
                quotaSupplementi.Settimane = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtSettimane))).Text;
                quotaSupplementi.DescrizioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Codice == quotaSupplementi.EnteGestioneFondo).Ente;
                quotaSupplementi.Decorrenza = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtDecorrenzaQuota))).Text;
                if (Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaVOCUM(domanda.Categoria) && !Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione))
                {
                    quotaSupplementi.AdeguamentoProQuotaCasse = quotaSupplementi.AdeguamentoProQuotaCasse;
                }
                else
                {
                    quotaSupplementi.AdeguamentoProQuotaCasse = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlAdeguamentoProQuotaCasse))).SelectedValue;
                }
                VS_ElencoQuoteSupplementi[e.RowIndex] = quotaSupplementi;
                gvQuoteSupplementi.EditIndex = -1;
                VS_IsModalitaEdit = false;
                gvQuoteSupplementi.DataSource = VS_ElencoQuoteSupplementi;
                gvQuoteSupplementi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiCumulo, Errore nel metodo gvQuoteSupplementi_RowUpdating " + ex);
            }
        }

        protected void gvQuoteSupplementi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridView gvQuoteSupplementi = (GridView)sender;
                VS_ElencoQuoteSupplementi.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoQuoteSupplementi.Count == 0)
                {
                    VS_ElencoQuoteSupplementi.Add(QuoteSupplementiLocal.GetEmptyQuotaSupplementi());
                    gvQuoteSupplementi.EditIndex = 0;
                }
                else
                {
                    VS_IsModalitaEdit = false;
                    gvQuoteSupplementi.EditIndex = -1;
                }
                gvQuoteSupplementi.DataSource = VS_ElencoQuoteSupplementi;
                gvQuoteSupplementi.DataBind();
                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiCumulo, Errore nel metodo gvQuoteSupplementi_RowCancelingEdit/ " + ex);
            }
        }

        protected void gvQuoteSupplementi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                RaiseHideAvviso(this, null);
                if (e.CommandName == "Aggiungi")
                {
                    GridView gvQuoteSupplementi = (GridView)sender;
                    VS_ElencoQuoteSupplementi.Add(QuoteSupplementiLocal.GetEmptyQuotaSupplementi());
                    VS_IsModalitaEdit = true;
                    gvQuoteSupplementi.EditIndex = VS_ElencoQuoteSupplementi.Count - 1;
                    gvQuoteSupplementi.DataSource = VS_ElencoQuoteSupplementi;
                    gvQuoteSupplementi.DataBind();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiCumulo, Errore nel metodo gvQuoteSupplementi_RowCommand " + ex);
            }
        }

        protected void gvQuoteSupplementi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvQuoteSupplementi = (GridView)sender;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowIndex == gvQuoteSupplementi.EditIndex)
                    {
                        LoadDdlEnteFondo(e.Row);
                        LoadDdlAdeguamentoProQuotaCasse(e.Row);
                        ((RequiredFieldValidator)e.Row.FindControl(Utility.GetDescription(EnumControlli.RFVddlAdeguamentoProQuotaCasse))).Enabled = (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione)) ? true : false;
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCGvQuoteSupplementi", Page.Theme, false);
                        if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica)
                        {
                            ((DropDownList)e.Row.FindControl(Utility.GetDescription(EnumControlli.ddlAdeguamentoProQuotaCasse))).SelectedValue = "SI";
                            ((DropDownList)e.Row.FindControl(Utility.GetDescription(EnumControlli.ddlAdeguamentoProQuotaCasse))).Enabled = false;
                        }
                    }
                    else
                    {
                        QuoteSupplementiLocal row = VS_ElencoQuoteSupplementi[e.Row.DataItemIndex];
                        CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[e.Row.Cells.Count - 1], Page.Theme, row.IsEditable, row.IsEditable);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo))).Text = row.EnteGestioneFondo;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblDescrizioneFondo))).Text = row.DescrizioneFondo;
                        if (row.Decorrenza != string.Format("{0:MM/yyyy}", new DateTime(9999, 1, 1)))
                            ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblDecorrenzaQuota))).Text = row.Decorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblValueDecorrenzaQuota))).Text = row.Decorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblSettimane))).Text = row.Settimane;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota))).Text = row.ImportoQuota;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblAdeguamentoProQuotaCasse))).Text = row.AdeguamentoProQuotaCasse;
                        if (row.TipoVariazione.HasValue)
                        {
                            switch (row.TipoVariazione.Value)
                            {
                                case 0:
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblTipoVariazione))).Text = "Modifica";
                                    break;
                                case 1:
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblTipoVariazione))).Text = "Cancellazione";
                                    break;
                                default:
                                    ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblTipoVariazione))).Text = string.Empty;
                                    break;
                            }
                        }
                        else
                            ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblTipoVariazione))).Text = string.Empty;
                    }
                }
                else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Visible = gvQuoteSupplementi.EditIndex == -1;
                    if (e.Row.Visible)
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
                                    cell.ColumnSpan = e.Row.Cells.Count - 1;
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiCumulo, Errore nel metodo gvQuoteSupplementi_RowDataBound " + ex);
            }
        }

        private void LoadDdlAdeguamentoProQuotaCasse(GridViewRow row)
        {
            DropDownList ddlAdeguamentoProQuotaCasse = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlAdeguamentoProQuotaCasse));
            string AdeguamentoProQuotaCasse = ((QuoteSupplementiLocal)(row.DataItem)).AdeguamentoProQuotaCasse;
            if (!String.IsNullOrEmpty(AdeguamentoProQuotaCasse) && !String.IsNullOrEmpty(AdeguamentoProQuotaCasse.Trim()))
                ddlAdeguamentoProQuotaCasse.Items.FindByValue(((QuoteSupplementiLocal)(row.DataItem)).AdeguamentoProQuotaCasse.Trim()).Selected = true;
            else
                ddlAdeguamentoProQuotaCasse.SelectedIndex = 0;
        }

        protected void gvQuoteSupplementi_DataBinding(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GridView gvQuoteSupplementi = (GridView)sender;
            gvQuoteSupplementi.ShowFooter = !datiPensione.IsDomandaCumuloAutomatica && gvQuoteSupplementi.EditIndex == -1;
        }

        protected void gvQuoteSupplementi_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            GridView gvQuoteSupplementi = (GridView)sender;
            gvQuoteSupplementi.Columns[GVQuoteSupplementiColumn.ModificaQuota.GetHashCode()].Visible = !datiPensione.IsDomandaCumuloAutomatica;
            gvQuoteSupplementi.Columns[GVQuoteSupplementiColumn.EliminaQuota.GetHashCode()].Visible = !datiPensione.IsDomandaCumuloAutomatica;
            gvQuoteSupplementi.Columns[GVQuoteSupplementiColumn.AdeguamentoProQuotaCasse.GetHashCode()].Visible = (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione)) ? true : false;
            //ENG - Memo 32_a/2018
            if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(datiPensione, this.domanda.Categoria))
            {
                gvQuoteSupplementi.Columns[GVQuoteSupplementiColumn.TipoVariazione.GetHashCode()].Visible = true;
            }



            ManagePulsanti();
        }

        protected void gvQuoteSupplementi_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gvQuoteSupplementi = (GridView)sender;
                VS_ElencoQuoteSupplementi.RemoveAt(e.RowIndex);
                //rimozione di eventuali record vuoti
                VS_ElencoQuoteSupplementi.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoQuoteSupplementi.Count == 0)
                {
                    VS_ElencoQuoteSupplementi.Add(QuoteSupplementiLocal.GetEmptyQuotaSupplementi());
                    gvQuoteSupplementi.EditIndex = 0;
                    VS_IsModalitaEdit = true;
                }
                else
                    gvQuoteSupplementi.EditIndex = -1;
                gvQuoteSupplementi.DataSource = VS_ElencoQuoteSupplementi;
                gvQuoteSupplementi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiCumulo, Errore nel metodo gvQuoteSupplementi_RowDeleting " + ex);
            }
        }
        #endregion

        private void ManagePulsanti()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (VS_ElencoQuoteSupplementi != null && VS_ElencoQuoteSupplementi.Where(x => !x.IsEmpty()).Count() > 0 && !VS_IsModalitaEdit)
            {
                btnSalvaSupplementiCumulo.Enabled = true;
                AbilitaTastoSalva(this, null);
            }
            else
            {
                btnSalvaSupplementiCumulo.Enabled = false;
                DisabilitaTastoSalva(this, null);
            }

            if (VS_ElencoQuoteSupplementi != null && VS_ElencoQuoteSupplementi.Where(x => !x.IsEmpty()).Count() > 0 &&
                VS_IsRicostituzione == false && !VS_IsModalitaEdit)
                btnEliminaSupplementiCumulo.Enabled = true;
            else
                btnEliminaSupplementiCumulo.Enabled = false;

            if (this.domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S"))
                btnEliminaSupplementiCumulo.Enabled = false;

            if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(this.TitolarePensione.Pensione))
            {
                btnSalvaSupplementiCumulo.Text = "Salva Pro Quota";
                btnEliminaSupplementiCumulo.Text = "Elimina Pro Quota";
            }
        }

        private void LoadDdlEnteFondo(GridViewRow row)
        {
            DropDownList ddlEnteGestioneQuota = (DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo));
            ddlEnteGestioneQuota.Items.Add(new ListItem(string.Empty, string.Empty));
            List<Presenter.SvrLiquidazione.DecEnteGestioneFondo> listaEnteFondo = VS_DecEnteGestioneFondo;
            IEnumerable<Presenter.SvrLiquidazione.DecEnteGestioneFondo> listaOrdinata = listaEnteFondo.OrderBy(x => x.Codice);
            foreach (Presenter.SvrLiquidazione.DecEnteGestioneFondo enteFondo in listaOrdinata)
            {
                ListItem li = new ListItem();
                li.Attributes.Add("title", enteFondo.Ente);
                li.Text = enteFondo.Codice;
                li.Value = enteFondo.Id.ToString();
                ddlEnteGestioneQuota.Items.Add(li);
            }
            if (((QuoteSupplementiLocal)(row.DataItem)).IdEnteGestioneFondo.Trim() == string.Empty)
                ddlEnteGestioneQuota.SelectedIndex = 0;
            else
                ddlEnteGestioneQuota.Items.FindByValue(((QuoteSupplementiLocal)(row.DataItem)).IdEnteGestioneFondo.Trim()).Selected = true;
        }

        public void ValorizzaEtichette(ISupplementi iSupplementi)
        {
            AreaSupplementi areaDatiSupplementi = iSupplementi.risposta;
            VS_DecEnteGestioneFondo = areaDatiSupplementi.ListaDecEnteGestioneFondo.ToList();
            SetViewStateIsRicostituzione();
            if (areaDatiSupplementi.ListaDatiSupplementiCumulo != null && areaDatiSupplementi.ListaDatiSupplementiCumulo.Count() > 0)
            {
                VS_IsModalitaEdit = false;
                InitGvQuoteSupplementi(areaDatiSupplementi.ListaDatiSupplementiCumulo.ToList());
            }
            else // dati non presenti
            {
                VS_IsModalitaEdit = true;
                InitGvQuoteSupplementi(null);
            }
            if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
            {
                lblSupplementiCumulo.Text = "Quote Supplementi Totalizzazione";
            }
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione))
            {
                lblSupplementiCumulo.Text = "Adeguamento Pro Quota";
            }
            ManagePulsanti();
        }

        protected void btnSalvaSupplementiCumulo_Click(object sender, EventArgs e)
        {

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();
            this.lstSupplementi = GetDatiUcSupplementi();
            presenterSupplementi.SalvaSupplementiCumulo(this);
            if (!this.HasError)
                RaiseInitializeData(this, e);
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO;
            }
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseSalvaSupplementi(this, Cevent);
        }

        internal AreaSupplementi GetDatiUcSupplementi()
        {
            AreaSupplementi areaSupplementi = new AreaSupplementi();
            areaSupplementi.ListaDatiSupplementiCumulo = MapLocalToServiceObject(VS_ElencoQuoteSupplementi).ToArray();
            return areaSupplementi;
        }

        protected void btnEliminaSupplementiCumulo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();
            presenterSupplementi.EliminaSupplementiCumulo(this);
            if (this.HasError == true)
                this.ErrorMessage = "Errore durante la cancellazione delle Quote Supplementi";
            else
                RaiseInitializeData(this, e);
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseEliminaSupplementi(this, Cevent);
        }

        private void InitGvQuoteSupplementi(List<DatiSupplementiCumulo> lstServer)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            List<QuoteSupplementiLocal> lstQuoteSupplementi = new List<QuoteSupplementiLocal>();

            if (lstServer != null && lstServer.Count() > 0)
            {
                lstQuoteSupplementi.AddRange(MapServiceToLocalObject(lstServer));
            }
            if (lstQuoteSupplementi.Count == 0)
                lstQuoteSupplementi.Add(QuoteSupplementiLocal.GetEmptyQuotaSupplementi());
            VS_ElencoQuoteSupplementi = lstQuoteSupplementi;
            if (lstQuoteSupplementi.Count == 1 && lstQuoteSupplementi[0].IsEmpty())
                gvQuoteSupplementi.EditIndex = 0;
            gvQuoteSupplementi.DataSource = lstQuoteSupplementi;
            gvQuoteSupplementi.DataBind();
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

        public List<DatiSupplementiCumulo> MapLocalToServiceObject(List<QuoteSupplementiLocal> lstLocal)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<DatiSupplementiCumulo> lstService = new List<DatiSupplementiCumulo>();
            if (lstLocal != null && lstLocal.Count() > 0)
            {
                foreach (QuoteSupplementiLocal elem in lstLocal)
                {
                    if (!elem.IsEmpty())
                    {
                        DatiSupplementiCumulo elemService = new DatiSupplementiCumulo();
                        elemService.Importo = decimal.Parse(elem.ImportoQuota);
                        elemService.Settimane = !string.IsNullOrEmpty(elem.Settimane) ? int.Parse(elem.Settimane) : (int?)null;
                        elemService.EnteGestioneFondo = long.Parse(elem.IdEnteGestioneFondo);
                        elemService.Decorrenza = Utility.GetDateFromString(elem.Decorrenza);
                        if (!String.IsNullOrEmpty(elem.AdeguamentoProQuotaCasse) && !String.IsNullOrEmpty(elem.AdeguamentoProQuotaCasse.Trim()))
                        {
                            elemService.AdeguamentoProQuotaCasse = (elem.AdeguamentoProQuotaCasse.Trim() == "SI") ? true : false;
                        }
                        elemService.TipoVariazione = elem.TipoVariazione;
                        lstService.Add(elemService);
                    }
                }
            }
            return lstService;
        }

        public List<QuoteSupplementiLocal> MapServiceToLocalObject(List<DatiSupplementiCumulo> lstService)
        {
            List<QuoteSupplementiLocal> lstLocal = new List<QuoteSupplementiLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (DatiSupplementiCumulo elemS in lstService)
                {
                    QuoteSupplementiLocal elemL = new QuoteSupplementiLocal();
                    elemL.ImportoQuota = elemS.Importo.ToString();
                    elemL.Settimane = elemS.Settimane.ToString();
                    elemL.EnteGestioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Codice;
                    elemL.DescrizioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Ente;
                    elemL.IdEnteGestioneFondo = elemS.EnteGestioneFondo.ToString();
                    elemL.Decorrenza = string.Format("{0:MM/yyyy}", elemS.Decorrenza);
                    if (elemS.AdeguamentoProQuotaCasse.HasValue)
                    {

                        elemL.AdeguamentoProQuotaCasse = (elemS.AdeguamentoProQuotaCasse.Value) ? "SI" : "NO";
                    }
                    elemL.TipoVariazione = elemS.TipoVariazione;
                    lstLocal.Add(elemL);
                }
            }
            return lstLocal;
        }


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
            [Description("btnEliminaQuoteSupplementi")]
            btnEliminaQuoteSupplementi,
            [Description("lblDescrizioneFondo_item")]
            lblDescrizioneFondo,
            [Description("lblDecorrenzaQuota_item")]
            lblDecorrenzaQuota,
            [Description("txtDecorrenzaQuota")]
            txtDecorrenzaQuota,
            [Description("lblValueDecorrenzaQuota")]
            lblValueDecorrenzaQuota,
            [Description("lblIdQuota")]
            lblIdQuota,
            [Description("txtAnnoCompetenza")]
            txtAnnoCompetenza,
            [Description("lblAdeguamentoProQuotaCasse_item")]
            lblAdeguamentoProQuotaCasse,
            [Description("ddlAdeguamentoProQuotaCasse")]
            ddlAdeguamentoProQuotaCasse,
            [Description("RFVddlAdeguamentoProQuotaCasse")]
            RFVddlAdeguamentoProQuotaCasse,
            [Description("lblTipoVariazione_item")]
            lblTipoVariazione
        }

        public enum GVQuoteSupplementiColumn
        {
            ModificaQuota = 0,
            Decorrenza = 3,
            EliminaQuota = 8,
            AdeguamentoProQuotaCasse = 6,
            TipoVariazione = 7
        }
        #endregion Enum

        #region Nestled Class
        [Serializable]
        public class QuoteSupplementiLocal
        {
            public QuoteSupplementiLocal()
            {
                this.Id = Guid.NewGuid();
                this.IsEditable = true;
            }
            public QuoteSupplementiLocal(string enteGestioneFondo, string settimane, string importoQuota, string id, string descrizioneFondo, string decorrenza, string AdeguamentoProQuotaCasse, int? tipoVariazione)
            {
                this.Id = Guid.NewGuid();
                this.IdEnteGestioneFondo = id;
                this.ImportoQuota = importoQuota;
                this.Settimane = settimane;
                this.EnteGestioneFondo = enteGestioneFondo;
                this.DescrizioneFondo = descrizioneFondo;
                this.Decorrenza = decorrenza;
                this.AdeguamentoProQuotaCasse = AdeguamentoProQuotaCasse;
                this.IsEditable = true;
                this.TipoVariazione = tipoVariazione;

            }

            public bool IsEmpty()
            {
                bool ret = false;
                if (string.IsNullOrEmpty(this.IdEnteGestioneFondo) && string.IsNullOrEmpty(this.ImportoQuota) && string.IsNullOrEmpty(this.Settimane) && string.IsNullOrEmpty(this.EnteGestioneFondo) &&
                    string.IsNullOrEmpty(this.Decorrenza) && string.IsNullOrEmpty(this.AdeguamentoProQuotaCasse) && !this.TipoVariazione.HasValue)
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
            public bool IsEditable { get; set; }
            public string AdeguamentoProQuotaCasse { get; set; }
            public int? TipoVariazione { get; set; }

            public static QuoteSupplementiLocal GetEmptyQuotaSupplementi()
            {
                return new QuoteSupplementiLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null);
            }
        }
        #endregion Nestled Class
    }
}