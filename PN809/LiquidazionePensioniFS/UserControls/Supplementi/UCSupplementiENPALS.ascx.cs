using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi
{
    public partial class UCSupplementiENPALS : CustomBaseUserControl, ISupplementi, ITitolarePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ISup
        public long numDomanda { get; set; }
        public AreaSupplementi lstSupplementi { get; set; }
        public Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public DatiContribuzioneEnpals datiContribuzioneEnpals { get; set; }
        #endregion ISup

        #region ITitolarePensione
        public AreaTitolare TitolarePensione { get; set; }

        #endregion ITitolarePensione

        protected void Page_Load(object sender, EventArgs e)
        {
            RaiseHideAvviso(this, null);
        }

        protected void btnSalvaDettaglioSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            // Get
            this.lstSupplementi = GetEtichetteDettaglio();

            // Salvataggio
            PresenterSupplementi presenter = new PresenterSupplementi();
            presenter.SalvaDettaglioSupplementi(this);
            ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] = this.lstSupplementi.DatiSuppRecordENPALS;
            ValorizzaDettaglio(this.lstSupplementi);
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseSalvaSupplementi(this, Cevent);
        }

        protected void btnEliminaDettaglioSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.lstSupplementi = new AreaSupplementi();
            this.lstSupplementi.DatiSuppRecordENPALS = (DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()];
            PresenterSupplementi presenter = new PresenterSupplementi();
            presenter.EliminaDettaglioSupplementi(this);
            ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] = this.lstSupplementi.DatiSuppRecordENPALS;

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Supplementi";
            else
                ValorizzaDettaglio(this.lstSupplementi);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseEliminaSupplementi(this, Cevent);
        }

        protected void btnEliminaRecordSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterSupplementi presenter = new PresenterSupplementi();
            presenter.EliminaTabSupplementiByDomanda(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei record Supplementi";
            else
                ValorizzaEtichette(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
            RaiseEliminaSupplementi(this, Cevent);
        }

        protected void TornaElencoSupplementi_Click(object sender, EventArgs e)
        {
            ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] = new List<DatiSupplementiENPALS>();
            ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] = new List<DatiSupplementiENPALS>();
            ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] = null;
            modalitaEditENPALS.Value = "false";
            modalitaEditContribENPALS.Value = "false";

            btnEliminaDettaglioSupplementi.Visible = true;

            ClearForm();
            RaiseInitData(this, null);
        }

        internal void ValorizzaEtichette(ISupplementi supplementi)
        {
            List<DatiSuppRecordENPALS> listaDatiSuppRecordENPALS = new List<DatiSuppRecordENPALS>();

            pnlRecordSupplementiEnpals.Visible = true;
            pnlSupplementiEnpals.Visible = false;

            if (supplementi != null && supplementi.risposta != null && supplementi.risposta.ListaDatiSuppRecordENPALS != null && supplementi.risposta.ListaDatiSuppRecordENPALS.Count() > 0)
                listaDatiSuppRecordENPALS = supplementi.risposta.ListaDatiSuppRecordENPALS.ToList();

            AddItemBlankRecordSupplementiENPALS(ref listaDatiSuppRecordENPALS);
            ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] = listaDatiSuppRecordENPALS;
            GvRecordSupplementiENPALS_Load();

            RaiseHideTastoSalva(this, null);
        }

        internal AreaSupplementi GetEtichetteDettaglio()
        {
            AreaSupplementi areaSupplementi = new AreaSupplementi();

            List<DatiSupplementiENPALS> elencoSupplementiRetrib = (List<DatiSupplementiENPALS>)ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()];
            List<DatiSupplementiENPALS> elencoSupplementiContrib = (List<DatiSupplementiENPALS>)ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()];
            removeItemBlankENPALS(ref elencoSupplementiRetrib);
            removeItemBlankContributiviENPALS(ref elencoSupplementiContrib);

            var result = elencoSupplementiRetrib.Concat(elencoSupplementiContrib);
            List<DatiSupplementiENPALS> elencoSupplementiToSave = result.ToList();

            areaSupplementi.ListDatiSupplementiENPALS = elencoSupplementiToSave.ToArray();

            areaSupplementi.DatiSuppRecordENPALS = (DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()];


            areaSupplementi.DatiSuppRecordENPALS.RenditaFacoltativaOrdinaria = !string.IsNullOrEmpty(txtRenditaFacolOrdinaria.Text) ? decimal.Parse(txtRenditaFacolOrdinaria.Text) : (decimal?)null;

            areaSupplementi.DatiSuppRecordENPALS.RenditaFacoltativaConvenzionale = !string.IsNullOrEmpty(txtRenditafacolConv.Text) ? decimal.Parse(txtRenditafacolConv.Text) : (decimal?)null;

            areaSupplementi.IntegrazioneArt11 = new IntegrazioneArt11();

            if (!string.IsNullOrEmpty(txtDecorrenza.Text))
                areaSupplementi.IntegrazioneArt11.Decorrenza = Utility.GetDateFromString(txtDecorrenza.Text);

            if (!string.IsNullOrEmpty(txtImportoIVS.Text))
                areaSupplementi.IntegrazioneArt11.ImportoIVS = decimal.Parse(txtImportoIVS.Text);

            return areaSupplementi;
        }

        #region private methods

        private void GestioneTastoSalva()
        {
            List<DatiSuppRecordENPALS> elencoRecordSupplementi = (List<DatiSuppRecordENPALS>)ViewState[EnumViewState.ElencoRecordSupplementi.ToString()];
            List<DatiSupplementiENPALS> elencoSupplementiRetrib = (List<DatiSupplementiENPALS>)ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()];
            List<DatiSupplementiENPALS> elencoSupplementiContrib = (List<DatiSupplementiENPALS>)ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()];

            if (modalitaEditRecordSupplementi.Value == "true")
            {
                btnSalvaDettaglioSupplementi.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
            else if (modalitaEditENPALS.Value == "true")
            {
                if (elencoSupplementiRetrib.Count > 1)
                {
                    btnSalvaDettaglioSupplementi.Enabled = false;
                    RaiseDisabilitaTastoSalva(this, null);
                }
                else
                {
                    if (modalitaEditContribENPALS.Value == "true")
                    {
                        btnSalvaDettaglioSupplementi.Enabled = false;
                        RaiseDisabilitaTastoSalva(this, null);
                    }
                    else
                    {
                        btnSalvaDettaglioSupplementi.Enabled = true;
                        RaiseAbilitaTastoSalva(this, null);
                    }
                }
            }
            else if (modalitaEditContribENPALS.Value == "true")
            {
                if (elencoSupplementiContrib.Count > 1)
                {
                    btnSalvaDettaglioSupplementi.Enabled = false;
                    RaiseDisabilitaTastoSalva(this, null);
                }
                else
                {
                    btnSalvaDettaglioSupplementi.Enabled = true;
                    RaiseAbilitaTastoSalva(this, null);
                }
            }
            else
            {
                btnSalvaDettaglioSupplementi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            cell_Edit.Width = new Unit(40, UnitType.Pixel);
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";

            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDelete")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
            delete.OnClientClick = "if(!window.confirm('Sei sicuro di voler eliminare i Supplementi?')) return false; else BlockUI();";
            delete.Visible = true;
        }

        private void EnableDeletableMode(GridViewRow row)
        {
            LinkButton delete = ((LinkButton)(row.FindControl("btnDelete")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
            delete.OnClientClick = "if(!window.confirm('Sei sicuro di voler eliminare i Supplementi?')) return false; else BlockUI();";
            delete.Visible = true;
        }

        private void ValorizzaDettaglio(AreaSupplementi areaSupplementi)
        {
            if (areaSupplementi != null)
            {
                List<DatiSupplementiENPALS> elencoSupplementiContrib = new List<DatiSupplementiENPALS>();
                List<DatiSupplementiENPALS> elencoSupplementiRetrib = new List<DatiSupplementiENPALS>();

                if (areaSupplementi.DatiSuppRecordENPALS != null)
                {
                    if (areaSupplementi.DatiSuppRecordENPALS.RenditaFacoltativaOrdinaria.HasValue)
                        txtRenditaFacolOrdinaria.Text = Convert.ToString(areaSupplementi.DatiSuppRecordENPALS.RenditaFacoltativaOrdinaria.Value);

                    if (areaSupplementi.DatiSuppRecordENPALS.RenditaFacoltativaConvenzionale.HasValue)
                        txtRenditafacolConv.Text = Convert.ToString(areaSupplementi.DatiSuppRecordENPALS.RenditaFacoltativaConvenzionale.Value);

                    ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] = areaSupplementi.DatiSuppRecordENPALS;
                }

                if (areaSupplementi.ListDatiSupplementiENPALS != null && areaSupplementi.ListDatiSupplementiENPALS.Count() > 0)
                {
                    foreach (DatiSupplementiENPALS datiSupp in areaSupplementi.ListDatiSupplementiENPALS)
                    {
                        switch (datiSupp.TipoSupplemento)
                        {
                            case 'C':
                                elencoSupplementiContrib.Add(datiSupp);
                                break;
                            case 'R':
                                elencoSupplementiRetrib.Add(datiSupp);
                                break;
                        }
                    }
                }

                if (areaSupplementi.DatiSuppRecordENPALS != null)
                {
                    if (!areaSupplementi.DatiSuppRecordENPALS.IsFromSas && !areaSupplementi.DatiSuppRecordENPALS.IsFromGP)
                    {
                        AddItemBlankENPALS(ref elencoSupplementiRetrib);
                        AddItemBlankContributiviENPALS(ref elencoSupplementiContrib);
                        pnlGridSupplementiContribEnpals.Visible = true;
                        pnlGridSupplementiRetribEnpals.Visible = true;
                    }
                    else
                    {
                        if (elencoSupplementiContrib == null || elencoSupplementiContrib.Count == 0)
                            pnlGridSupplementiContribEnpals.Visible = false;
                        else
                            pnlGridSupplementiContribEnpals.Visible = true;
                        if (elencoSupplementiRetrib == null || elencoSupplementiRetrib.Count == 0)
                            pnlGridSupplementiRetribEnpals.Visible = false;
                        else 
                            pnlGridSupplementiRetribEnpals.Visible = true;

                        btnEliminaDettaglioSupplementi.Visible = false;
                    }
                }

                ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] = elencoSupplementiRetrib;
                GvSupplementiENPALS_Load();

                ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] = elencoSupplementiContrib;
                GvSupplementiContributiviENPALS_Load();

                if (areaSupplementi.IntegrazioneArt11 != null)
                {
                    if (areaSupplementi.IntegrazioneArt11.Decorrenza.HasValue)
                        txtDecorrenza.Text = String.Format("{0:MM/yyyy}", areaSupplementi.IntegrazioneArt11.Decorrenza.Value);

                    if (areaSupplementi.IntegrazioneArt11.ImportoIVS.HasValue)
                        txtImportoIVS.Text = areaSupplementi.IntegrazioneArt11.ImportoIVS.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, -1);
        }

        #endregion private methods

        #region Grid Record Supplementi ENPALS

        protected void gvRecordSupplementiENPALS_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string currentTheme = Page.Theme;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.FindControl("img").Visible = false;
                    e.Row.FindControl("btnDettaglio").Visible = false;
                    e.Row.FindControl("btnDelete").Visible = false;

                    if (((DatiSuppRecordENPALS)(e.Row.DataItem)).IsFromSas || ((DatiSuppRecordENPALS)(e.Row.DataItem)).IsFromGP)
                    {
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).Decorrenza.HasValue ? string.Format("{0:MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).Decorrenza) : string.Empty;
                        ((Label)e.Row.FindControl("lblInizioSupplemento")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).InizioSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).InizioSupplemento) : string.Empty;
                        ((Label)e.Row.FindControl("lblFineSupplemento")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).FineSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).FineSupplemento) : string.Empty;

                        e.Row.Cells[0].Width = new Unit(40, UnitType.Pixel);
                        LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        edit.Text = "";

                        Image img = (Image)e.Row.FindControl("img");
                        img.Visible = true;

                        if (((DatiSuppRecordENPALS)(e.Row.DataItem)).DettaglioSalvato)
                        {
                            img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                            img.ToolTip = "Salvato";
                        }
                        else
                        {
                            img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                            img.ToolTip = "Non Salvato";
                        }

                        e.Row.FindControl("btnDettaglio").Visible = true;

                        if (((DatiSuppRecordENPALS)(e.Row.DataItem)).IsFromGP)
                            EnableDeletableMode(e.Row);
                    }
                    else
                    {
                        int num = ((List<DatiSuppRecordENPALS>)ViewState[EnumViewState.ElencoRecordSupplementi.ToString()]).Count;

                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaRecordSupplementiEmpty() && !Convert.ToBoolean(modalitaEditRecordSupplementi.Value))
                            {
                                gvRecordSupplementiEnpals.EditIndex = 0;
                                modalitaEditRecordSupplementi.Value = "true";
                                GvRecordSupplementiENPALS_Load();
                                GestioneTastoSalva();
                            }
                            else
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    setCampiEditRecordSupplementiENPALS(e);
                                    EnableEditableModeRecordSupplementiENPALS(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).Decorrenza.HasValue ? string.Format("{0:MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).Decorrenza) : string.Empty;
                                    ((Label)e.Row.FindControl("lblInizioSupplemento")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).InizioSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).InizioSupplemento) : string.Empty;
                                    ((Label)e.Row.FindControl("lblFineSupplemento")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).FineSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).FineSupplemento) : string.Empty;

                                    EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5]);

                                    Image img = (Image)e.Row.FindControl("img");
                                    img.Visible = true;

                                    if (((DatiSuppRecordENPALS)(e.Row.DataItem)).DettaglioSalvato)
                                    {
                                        img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                        img.ToolTip = "Salvato";
                                    }
                                    else
                                    {
                                        img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                        img.ToolTip = "Non Salvato";
                                    }

                                    e.Row.FindControl("btnDettaglio").Visible = true;
                                }
                            }
                        }
                        else // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEditRecordSupplementiENPALS(e);
                                EnableEditableModeRecordSupplementiENPALS(e.Row.Cells[0]);
                            }
                            else if (e.Row.DataItemIndex == num - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).Decorrenza.HasValue ? string.Format("{0:MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).Decorrenza) : string.Empty;
                                ((Label)e.Row.FindControl("lblInizioSupplemento")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).InizioSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).InizioSupplemento) : string.Empty;
                                ((Label)e.Row.FindControl("lblFineSupplemento")).Text = ((DatiSuppRecordENPALS)(e.Row.DataItem)).FineSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)(e.Row.DataItem)).FineSupplemento) : string.Empty;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5]);

                                Image img = (Image)e.Row.FindControl("img");
                                img.Visible = true;

                                if (((DatiSuppRecordENPALS)(e.Row.DataItem)).DettaglioSalvato)
                                {
                                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                    img.ToolTip = "Salvato";
                                }
                                else
                                {
                                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                    img.ToolTip = "Non Salvato";
                                }

                                e.Row.FindControl("btnDettaglio").Visible = true;
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvRecordSupplementiENPALS_RowDataBound " + ex);
            }
        }

        protected void gvRecordSupplementiENPALS_DataBound(object sender, EventArgs e)
        {
            GridView app = (GridView)sender;
            app.Columns[colonnegvRecordSupplementiENPALS.Semaforo.GetHashCode()].Visible = false;
            app.Columns[colonnegvRecordSupplementiENPALS.Dettaglio.GetHashCode()].Visible = false;
            app.Columns[colonnegvRecordSupplementiENPALS.Delete.GetHashCode()].Visible = false;

            foreach (GridViewRow row in app.Rows)
                if (row.FindControl("img").Visible)
                {
                    app.Columns[colonnegvRecordSupplementiENPALS.Semaforo.GetHashCode()].Visible = true;
                    break;
                }

            foreach (GridViewRow row in app.Rows)
                if (row.FindControl("btnDettaglio").Visible)
                {
                    app.Columns[colonnegvRecordSupplementiENPALS.Dettaglio.GetHashCode()].Visible = true;
                    break;
                }

            foreach (GridViewRow row in app.Rows)
                if (row.FindControl("btnDelete").Visible)
                {
                    app.Columns[colonnegvRecordSupplementiENPALS.Delete.GetHashCode()].Visible = true;
                    break;
                }
        }

        protected void gvRecordSupplementiENPALS_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiSuppRecordENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] as List<DatiSuppRecordENPALS>;
            int num = elencoSupplementi.Count;

            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                if (this.lstSupplementi == null)
                    this.lstSupplementi = new AreaSupplementi();
                this.lstSupplementi.DatiSuppRecordENPALS = elencoSupplementi[r.DataItemIndex];
                PresenterSupplementi presenter = new PresenterSupplementi();
                presenter.EliminaRecordSupplemento(this);

                if (!this.HasError)
                {
                    this.modalitaEditRecordSupplementi.Value = "false";
                    elencoSupplementi.RemoveAt(r.DataItemIndex);

                    if (elencoSupplementi.Count > 1)
                        gvRecordSupplementiEnpals.EditIndex = -1;

                    ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] = elencoSupplementi;
                    GvRecordSupplementiENPALS_Load();
                }
                Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
                RaiseEliminaSupplementi(this, Cevent);

            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditRecordSupplementi.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                modalitaEditRecordSupplementi.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                DatiSuppRecordENPALS supp = new DatiSuppRecordENPALS();
                if (!string.IsNullOrEmpty(((TextBox)r.FindControl("txtDecorrenza")).Text))
                    supp.Decorrenza = Utility.GetDateFromString(((TextBox)r.FindControl("txtDecorrenza")).Text);
                if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtInizioSupplemento"))).Text))
                    supp.InizioSupplemento = Utility.GetDateFromString(((TextBox)(r.FindControl("txtInizioSupplemento"))).Text);
                if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtFineSupplemento"))).Text))
                    supp.FineSupplemento = Utility.GetDateFromString(((TextBox)(r.FindControl("txtFineSupplemento"))).Text);
                if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtFineSupplemento"))).Text))
                    supp.FineSupplemento = Utility.GetDateFromString(((TextBox)(r.FindControl("txtFineSupplemento"))).Text);
                if (!string.IsNullOrEmpty(((Label)(r.FindControl("lblImporto"))).Text))
                    supp.Importo = CodeUtility.StringToNullableDecimal(((Label)(r.FindControl("lblImporto"))).Text);

                if (this.lstSupplementi == null)
                    this.lstSupplementi = new AreaSupplementi();

                if ((r.DataItemIndex - 1) == (num - 2))
                    this.lstSupplementi.DatiSuppRecordENPALS = supp;
                else
                {
                    supp.IdSuppRecordEnpals = elencoSupplementi[r.DataItemIndex].IdSuppRecordEnpals;
                    supp.DettaglioSalvato = elencoSupplementi[r.DataItemIndex].DettaglioSalvato;
                    supp.RenditaFacoltativaConvenzionale = elencoSupplementi[r.DataItemIndex].RenditaFacoltativaConvenzionale;
                    supp.RenditaFacoltativaOrdinaria = elencoSupplementi[r.DataItemIndex].RenditaFacoltativaOrdinaria;
                    this.lstSupplementi.DatiSuppRecordENPALS = supp;
                }
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                PresenterSupplementi presenter = new PresenterSupplementi();
                presenter.SalvaRecordSupplemento(this);
                supp = this.lstSupplementi.DatiSuppRecordENPALS;
                if (!this.HasError)
                {
                    if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                    {
                        elencoSupplementi.RemoveAt(num - 1);
                        elencoSupplementi.Add(supp);
                        AddItemBlankRecordSupplementiENPALS(ref elencoSupplementi);
                    }
                    else
                        elencoSupplementi[r.DataItemIndex] = supp;

                    gvRecordSupplementiEnpals.EditIndex = -1;
                    ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] = elencoSupplementi;
                    GvRecordSupplementiENPALS_Load();
                }
                Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza.Value, null);
                RaiseSalvaSupplementi(this, Cevent);
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaRecordSupplementiEmpty())
                {
                    modalitaEditRecordSupplementi.Value = "false";
                    gvRecordSupplementiEnpals.EditIndex = -1;
                }

                GvRecordSupplementiENPALS_Load();
            }
            else if (e.CommandName == "Dettaglio")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                pnlRecordSupplementiEnpals.Visible = false;
                pnlSupplementiEnpals.Visible = true;
                modalitaEditRecordSupplementi.Value = "false";

                // Get dei dati e valorizzaEtichette
                this.lstSupplementi = new AreaSupplementi() { DatiSuppRecordENPALS = elencoSupplementi[r.DataItemIndex] };
                PresenterSupplementi presenter = new PresenterSupplementi();
                presenter.GetDettagliSupplementiEnpals(this);
                /////////////////////////////////////
                ValorizzaDettaglio(this.lstSupplementi);
            }

            GestioneTastoSalva();
        }

        protected void gvRecordSupplementiENPALS_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvRecordSupplementiEnpals.EditIndex = -1;

                //Bind data to the GridView control.
                GvRecordSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvRecordSupplementiENPALS_RowCancelingEdit " + ex);
            }
        }

        protected void gvRecordSupplementiENPALS_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRecordSupplementiEnpals.EditIndex = e.NewEditIndex;
                GvRecordSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvRecordSupplementiENPALS_RowEditing " + ex);
            }
        }

        protected void gvRecordSupplementiENPALS_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<DatiSuppRecordENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] as List<DatiSuppRecordENPALS>;

                if (elencoSupplementi.Count < 1)
                    inserisciRecordSupplementiENPALS();
                GvRecordSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvRecordSupplementiENPALS_RowDeleting " + ex);
            }
        }

        protected void gvRecordSupplementiENPALS_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRecordSupplementiEnpals.PageIndex = e.NewPageIndex;
                GvRecordSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvRecordSupplementiENPALS_onPageIndexChanging" + ex);
            }
        }

        #endregion Grid Record Supplementi ENPALS

        #region Private Methods Grid Record Supplementi
        private bool IsListaRecordSupplementiEmpty()
        {
            List<DatiSuppRecordENPALS> listaDatiSuppl = ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] as List<DatiSuppRecordENPALS>;

            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].Decorrenza.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].InizioSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].FineSupplemento.ToString()))
                return true;
            else
                return false;
        }

        private void GvRecordSupplementiENPALS_Load()
        {
            try
            {
                List<DatiSuppRecordENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] as List<DatiSuppRecordENPALS>;

                //if (elencoSupplementi != null && elencoSupplementi.Count > 1)
                //    gvRecordSupplementiEnpals.Columns[colonnegvRecordSupplementiENPALS.Dettaglio.GetHashCode()].Visible = true;

                //if (elencoSupplementi != null && elencoSupplementi.Count(x => !x.IsFromSasGp && x.IdSuppRecordEnpals != 0) > 0)
                //    gvRecordSupplementiEnpals.Columns[colonnegvRecordSupplementiENPALS.Delete.GetHashCode()].Visible = true;

                gvRecordSupplementiEnpals.DataSource = elencoSupplementi;
                gvRecordSupplementiEnpals.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo GvRecordSupplementiENPALS_Load " + ex);
            }
        }

        private void setCampiEditRecordSupplementiENPALS(GridViewRowEventArgs e)
        {
            TextBox txtDecorrenza = (TextBox)e.Row.FindControl("txtDecorrenza");
            TextBox txtInizioSupplemento = (TextBox)e.Row.FindControl("txtInizioSupplemento");
            TextBox txtFineSupplemento = (TextBox)e.Row.FindControl("txtFineSupplemento");

            txtDecorrenza.Text = ((DatiSuppRecordENPALS)e.Row.DataItem).Decorrenza.HasValue ? string.Format("{0:MM/yyyy}", ((DatiSuppRecordENPALS)e.Row.DataItem).Decorrenza) : string.Empty;
            txtInizioSupplemento.Text = ((DatiSuppRecordENPALS)e.Row.DataItem).InizioSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)e.Row.DataItem).InizioSupplemento) : string.Empty;
            txtFineSupplemento.Text = ((DatiSuppRecordENPALS)e.Row.DataItem).FineSupplemento.HasValue ? string.Format("{0:dd/MM/yyyy}", ((DatiSuppRecordENPALS)e.Row.DataItem).FineSupplemento) : string.Empty;
        }

        private void EnableEditableModeRecordSupplementiENPALS(TableCell cell_CancelSave)
        {
            cell_CancelSave.Width = new Unit(40, UnitType.Pixel);

            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.OnClientClick = "if(Page_ClientValidate('UCTabRecordSupplementiENPALS')){aspnetForm.target ='_self'; BlockUI();}";
        }

        private void AddItemBlankRecordSupplementiENPALS(ref List<DatiSuppRecordENPALS> lista)
        {
            if (lista != null)
            {
                int index = lista.FindIndex(delegate (DatiSuppRecordENPALS code)
                {
                    return (string.IsNullOrEmpty(code.Decorrenza.ToString()) && string.IsNullOrEmpty(code.InizioSupplemento.ToString()) &&
                        string.IsNullOrEmpty(code.FineSupplemento.ToString()));
                }
                    );

                if (index < 0)
                {
                    lista.Add(new DatiSuppRecordENPALS());
                }
            }
        }

        private void removeItemBlankRecordSupplementiENPALS(ref List<DatiSuppRecordENPALS> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (DatiSuppRecordENPALS code)
                {
                    return (string.IsNullOrEmpty(code.Decorrenza.ToString()) && string.IsNullOrEmpty(code.InizioSupplemento.ToString()) &&
                        string.IsNullOrEmpty(code.FineSupplemento.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        private void inserisciRecordSupplementiENPALS()
        {
            try
            {
                List<DatiSuppRecordENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] as List<DatiSuppRecordENPALS>;
                DatiSuppRecordENPALS supp = new DatiSuppRecordENPALS();
                elencoSupplementi.Add(supp);
                ViewState[EnumViewState.ElencoRecordSupplementi.ToString()] = elencoSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo inserisciRecordSupplementiENPALS " + ex);
            }
        }
        #endregion Private Methods Grid Record Supplementi

        #region Grid Retributivi ENPALS
        protected void gvSupplementiENPALS_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((DatiSupplementiENPALS)(e.Row.DataItem)).IsFromSAS || ((DatiSupplementiENPALS)(e.Row.DataItem)).IsFromGP)
                    {
                        ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblNSettimane")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Periodi.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Periodi.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblRM")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).RM.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).RM.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblImporto")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Importo.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Importo.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblImportoProRataTemporis")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoProRataTemporis.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoProRataTemporis.ToString() : string.Empty;

                        e.Row.Cells[0].Width = new Unit(40, UnitType.Pixel);
                        LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        edit.Text = "";
                    }
                    else
                    {
                        int num = ((List<DatiSupplementiENPALS>)ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()]).Count;

                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaENPALSEmpty() && !Convert.ToBoolean(modalitaEditENPALS.Value))
                            {
                                gvSupplementiENPALS.EditIndex = 0;
                                modalitaEditENPALS.Value = "true";
                                GvSupplementiENPALS_Load();
                                GestioneTastoSalva();
                            }
                            else
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    setCampiEditENPALS(e);
                                    EnableEditableModeENPALS(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblNSettimane")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Periodi.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Periodi.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblRM")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).RM.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).RM.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblImporto")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Importo.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Importo.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblImportoProRataTemporis")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoProRataTemporis.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoProRataTemporis.ToString() : string.Empty;

                                    EnableReadableMode(e.Row.Cells[0], e.Row.Cells[6]);
                                }
                            }
                        }
                        else // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEditENPALS(e);
                                EnableEditableModeENPALS(e.Row.Cells[0]);
                            }
                            else if (e.Row.DataItemIndex == num - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblNSettimane")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Periodi.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Periodi.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblRM")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).RM.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).RM.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Importo.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Importo.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblImportoProRataTemporis")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoProRataTemporis.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoProRataTemporis.ToString() : string.Empty;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[6]);
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_RowDataBound " + ex);
            }
        }

        protected void gvSupplementiENPALS_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] as List<DatiSupplementiENPALS>;
            int num = elencoSupplementi.Count;

            if (e.CommandName == "Delete")
            {
                this.modalitaEditENPALS.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                elencoSupplementi.RemoveAt(r.DataItemIndex);

                if (elencoSupplementi.Count > 1)
                    gvSupplementiENPALS.EditIndex = -1;

                ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] = elencoSupplementi;
                GvSupplementiENPALS_Load();
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditENPALS.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                modalitaEditENPALS.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    DatiSupplementiENPALS supp = new DatiSupplementiENPALS();
                    if (!string.IsNullOrEmpty(((DropDownList)(r.FindControl("ddlQuota"))).SelectedValue))
                        supp.Quota = char.Parse(((DropDownList)(r.FindControl("ddlQuota"))).SelectedValue);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtNSettimane"))).Text))
                        supp.Periodi = short.Parse(((TextBox)(r.FindControl("txtNSettimane"))).Text);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtNTotaleContributiCalcolo"))).Text))
                        supp.NTotaleContributiCalcolo = short.Parse(((TextBox)(r.FindControl("txtNTotaleContributiCalcolo"))).Text);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtRM"))).Text))
                        supp.RM = decimal.Parse(((TextBox)(r.FindControl("txtRM"))).Text);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtImporto"))).Text))
                        supp.Importo = decimal.Parse(((TextBox)(r.FindControl("txtImporto"))).Text);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtImportoProRataTemporis"))).Text))
                        supp.ImportoProRataTemporis = decimal.Parse(((TextBox)(r.FindControl("txtImportoProRataTemporis"))).Text);
                    if (!string.IsNullOrEmpty(((Label)r.FindControl("lblDecorrenza")).Text))
                        supp.Decorrenza = Utility.GetDateFromString(((Label)r.FindControl("lblDecorrenza")).Text);

                    supp.TipoSupplemento = 'R';

                    elencoSupplementi.RemoveAt(num - 1);
                    elencoSupplementi.Add(supp);
                    AddItemBlankENPALS(ref elencoSupplementi);
                }
                else
                    saveValueRowENPALS(elencoSupplementi, e, r);

                gvSupplementiENPALS.EditIndex = -1;
                ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] = elencoSupplementi;
                GvSupplementiENPALS_Load();
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaENPALSEmpty())
                {
                    modalitaEditENPALS.Value = "false";
                    gvSupplementiENPALS.EditIndex = -1;
                    GvSupplementiENPALS_Load();
                }
            }

            GestioneTastoSalva();
        }

        protected void gvSupplementiENPALS_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvSupplementiENPALS.EditIndex = -1;

                //Bind data to the GridView control.
                GvSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_RowCancelingEdit " + ex);
            }
        }

        protected void gvSupplementiENPALS_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSupplementiENPALS.EditIndex = e.NewEditIndex;
                GvSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_RowEditing " + ex);
            }
        }

        protected void gvSupplementiENPALS_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] as List<DatiSupplementiENPALS>;

                if (elencoSupplementi.Count < 1)
                    inserisciSupplementiENPALS();
                GvSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_RowDeleting " + ex);
            }
        }

        protected void gvSupplementiENPALS_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSupplementiENPALS.PageIndex = e.NewPageIndex;
                GvSupplementiENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_onPageIndexChanging" + ex);
            }
        }
        #endregion Grid Retributivi ENPALS

        #region Private Methods Grid Retributivi
        private bool IsListaENPALSEmpty()
        {
            List<DatiSupplementiENPALS> listaDatiSuppl = ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] as List<DatiSupplementiENPALS>;

            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].Quota.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].Periodi.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].NTotaleContributiCalcolo.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].RM.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].Importo.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].ImportoProRataTemporis.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].Decorrenza.ToString()))
                return true;
            else
                return false;
        }

        private void GvSupplementiENPALS_Load()
        {
            try
            {
                List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] as List<DatiSupplementiENPALS>;
                gvSupplementiENPALS.DataSource = elencoSupplementi;
                gvSupplementiENPALS.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo GvSupplementiENPALS_Load " + ex);
            }
        }

        private void setCampiEditENPALS(GridViewRowEventArgs e)
        {
            DropDownList ddlQuota = (DropDownList)e.Row.FindControl("ddlQuota");
            TextBox txtNSettimane = (TextBox)e.Row.FindControl("txtNSettimane");
            TextBox txtNTotaleContributiCalcolo = (TextBox)e.Row.FindControl("txtNTotaleContributiCalcolo");
            TextBox txtRM = (TextBox)e.Row.FindControl("txtRM");
            TextBox txtImporto = (TextBox)e.Row.FindControl("txtImporto");
            TextBox txtImportoProRataTemporis = (TextBox)e.Row.FindControl("txtImportoProRataTemporis");
            Label lblDecorrenza = (Label)e.Row.FindControl("lblDecorrenza");

            ddlQuota.SelectedValue = ((DatiSupplementiENPALS)e.Row.DataItem).Quota.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).Quota.ToString() : string.Empty;
            txtNSettimane.Text = ((DatiSupplementiENPALS)e.Row.DataItem).Periodi.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).Periodi.ToString() : string.Empty;
            txtNTotaleContributiCalcolo.Text = ((DatiSupplementiENPALS)e.Row.DataItem).NTotaleContributiCalcolo.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).NTotaleContributiCalcolo.ToString() : string.Empty;
            txtRM.Text = ((DatiSupplementiENPALS)e.Row.DataItem).RM.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).RM.ToString() : string.Empty;
            txtImporto.Text = ((DatiSupplementiENPALS)e.Row.DataItem).Importo.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).Importo.ToString() : string.Empty;
            txtImportoProRataTemporis.Text = ((DatiSupplementiENPALS)e.Row.DataItem).ImportoProRataTemporis.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).ImportoProRataTemporis.ToString() : string.Empty;

            if ((DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] != null)
                lblDecorrenza.Text = string.Format("{0:MM/yyyy}", ((DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()]).Decorrenza);
        }

        private void EnableEditableModeENPALS(TableCell cell_CancelSave)
        {
            cell_CancelSave.Width = new Unit(40, UnitType.Pixel);

            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabSupplementiENPALSRetrib";

        }

        private void AddItemBlankENPALS(ref List<DatiSupplementiENPALS> lista)
        {
            if (lista != null)
            {
                int index = lista.FindIndex(delegate (DatiSupplementiENPALS code)
                {
                    return (string.IsNullOrEmpty(code.Quota.ToString()) && string.IsNullOrEmpty(code.Periodi.ToString()) &&
                        string.IsNullOrEmpty(code.NTotaleContributiCalcolo.ToString()) && string.IsNullOrEmpty(code.RM.ToString()) &&
                        string.IsNullOrEmpty(code.Importo.ToString()) && string.IsNullOrEmpty(code.ImportoProRataTemporis.ToString()) && string.IsNullOrEmpty(code.Decorrenza.ToString()));
                }
                    );

                if (index < 0)
                {
                    lista.Add(new DatiSupplementiENPALS());
                }
            }
        }

        private void removeItemBlankENPALS(ref List<DatiSupplementiENPALS> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (DatiSupplementiENPALS code)
                {
                    return (string.IsNullOrEmpty(code.Quota.ToString()) && string.IsNullOrEmpty(code.Periodi.ToString()) &&
                        string.IsNullOrEmpty(code.NTotaleContributiCalcolo.ToString()) && string.IsNullOrEmpty(code.RM.ToString()) &&
                        string.IsNullOrEmpty(code.Importo.ToString()) && string.IsNullOrEmpty(code.ImportoProRataTemporis.ToString()) && string.IsNullOrEmpty(code.Decorrenza.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        private void saveValueRowENPALS(List<DatiSupplementiENPALS> elencoSupplementi, GridViewCommandEventArgs e, GridViewRow r)
        {
            if (!String.IsNullOrEmpty((((DropDownList)(r.FindControl("ddlQuota"))).SelectedValue)))
                elencoSupplementi[r.DataItemIndex].Quota = char.Parse(((DropDownList)(r.FindControl("ddlQuota"))).SelectedValue);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtNSettimane"))).Text)))
                elencoSupplementi[r.DataItemIndex].Periodi = short.Parse(((TextBox)(r.FindControl("txtNSettimane"))).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtNTotaleContributiCalcolo"))).Text)))
                elencoSupplementi[r.DataItemIndex].NTotaleContributiCalcolo = short.Parse(((TextBox)(r.FindControl("txtNTotaleContributiCalcolo"))).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtRM"))).Text)))
                elencoSupplementi[r.DataItemIndex].RM = Convert.ToDecimal(((TextBox)(r.FindControl("txtRM"))).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtImporto"))).Text)))
                elencoSupplementi[r.DataItemIndex].Importo = Convert.ToDecimal(((TextBox)(r.FindControl("txtImporto"))).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtImportoProRataTemporis"))).Text)))
                elencoSupplementi[r.DataItemIndex].ImportoProRataTemporis = Convert.ToDecimal(((TextBox)(r.FindControl("txtImportoProRataTemporis"))).Text);
        }

        private void inserisciSupplementiENPALS()
        {
            try
            {
                List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] as List<DatiSupplementiENPALS>;
                DatiSupplementiENPALS supp = new DatiSupplementiENPALS();
                elencoSupplementi.Add(supp);
                ViewState[EnumViewState.ElencoSupplementiENPALSRetrib.ToString()] = elencoSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo InserisciSupplementiENPALS " + ex);
            }
        }
        #endregion Private Methods Grid Retributivi

        #region Grid Contributivi ENPALS
        protected void gvSupplementiContributiviENPALS_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((DatiSupplementiENPALS)(e.Row.DataItem)).IsFromSAS || ((DatiSupplementiENPALS)(e.Row.DataItem)).IsFromGP)
                    {
                        ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoContributivoTotale.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Montante.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Montante.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.ToString() : string.Empty;
                        ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.ToString() : string.Empty;

                        e.Row.Cells[0].Width = new Unit(40, UnitType.Pixel);
                        LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        edit.Text = "";
                    }
                    else
                    {
                        int num = ((List<DatiSupplementiENPALS>)ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()]).Count;

                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaEmptyContribENPALS() && !Convert.ToBoolean(modalitaEditContribENPALS.Value))
                            {
                                gvSupplementiContributiviENPALS.EditIndex = 0;
                                modalitaEditContribENPALS.Value = "true";
                                GvSupplementiContributiviENPALS_Load();
                                GestioneTastoSalva();
                            }
                            else
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    setCampiEditContribENPALS(e);
                                    EnableEditableModeContribENPALS(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoContributivoTotale.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Montante.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Montante.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.ToString() : string.Empty;
                                    ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.ToString() : string.Empty;

                                    EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4]);
                                }
                            }
                        }
                        else // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEditContribENPALS(e);
                                EnableEditableModeContribENPALS(e.Row.Cells[0]);
                            }

                            else if (e.Row.DataItemIndex == num - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).ImportoContributivoTotale.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Montante.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Montante.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.ToString() : string.Empty;
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.HasValue ? ((DatiSupplementiENPALS)(e.Row.DataItem)).Quota.ToString() : string.Empty;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4]);
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiContributiviENPALS_RowDataBound " + ex);
            }
        }

        protected void gvSupplementiContributiviENPALS_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] as List<DatiSupplementiENPALS>;
            removeItemBlankContributiviENPALS(ref elencoSupplementi);
            AddItemBlankContributiviENPALS(ref elencoSupplementi);

            int num = elencoSupplementi.Count;
            if (e.CommandName == "Delete")
            {
                this.modalitaEditContribENPALS.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                elencoSupplementi.RemoveAt(r.DataItemIndex);

                if (elencoSupplementi.Count > 1)
                    gvSupplementiContributiviENPALS.EditIndex = -1;

                ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] = elencoSupplementi;
                GvSupplementiContributiviENPALS_Load();
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditContribENPALS.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                this.modalitaEditContribENPALS.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    DatiSupplementiENPALS supp = new DatiSupplementiENPALS();
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtImportoContributivoTotale"))).Text))
                        supp.ImportoContributivoTotale = Convert.ToDecimal(((TextBox)(r.FindControl("txtImportoContributivoTotale"))).Text);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtMontante"))).Text))
                        supp.Montante = Convert.ToDecimal(((TextBox)(r.FindControl("txtMontante"))).Text);
                    if (!string.IsNullOrEmpty(((TextBox)(r.FindControl("txtCoefficienteTrasformazione"))).Text))
                        supp.CoefficienteTrasformazione = Convert.ToDecimal(((TextBox)(r.FindControl("txtCoefficienteTrasformazione"))).Text);
                    if (!string.IsNullOrEmpty(((Label)(r.FindControl("lblDecorrenza"))).Text))
                        supp.Decorrenza = Utility.GetDateFromString(((Label)(r.FindControl("lblDecorrenza"))).Text);
                    if (!string.IsNullOrEmpty(((DropDownList)(r.FindControl("ddlQuota"))).Text))
                        supp.Quota = char.Parse(((DropDownList)(r.FindControl("ddlQuota"))).SelectedValue);

                    supp.TipoSupplemento = 'C';

                    elencoSupplementi.RemoveAt(num - 1);
                    elencoSupplementi.Add(supp);
                    AddItemBlankContributiviENPALS(ref elencoSupplementi);
                }
                else
                    saveValueRowContribENPALS(elencoSupplementi, e, r);

                gvSupplementiContributiviENPALS.EditIndex = -1;
                ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] = elencoSupplementi;
                GvSupplementiContributiviENPALS_Load();
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmptyContribENPALS())
                {
                    modalitaEditContribENPALS.Value = "false";
                    gvSupplementiContributiviENPALS.EditIndex = -1;
                    GvSupplementiContributiviENPALS_Load();
                }
            }

            GestioneTastoSalva();
        }

        protected void gvSupplementiContributiviENPALS_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvSupplementiContributiviENPALS.EditIndex = -1;
                //Bind data to the GridView control.
                GvSupplementiContributiviENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiContributiviENPALS_RowCancelingEdit " + ex);
            }
        }

        protected void gvSupplementiContributiviENPALS_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSupplementiContributiviENPALS.EditIndex = e.NewEditIndex;
                GvSupplementiContributiviENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_RowEditing " + ex);
            }
        }

        protected void gvSupplementiContributiviENPALS_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] as List<DatiSupplementiENPALS>;

                if (elencoSupplementi.Count < 1)
                    inserisciSupplementiContribENPALS();
                GvSupplementiContributiviENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiENPALS_RowDeleting " + ex);
            }
        }

        protected void gvSupplementiContributiviENPALS_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSupplementiContributiviENPALS.PageIndex = e.NewPageIndex;
                GvSupplementiContributiviENPALS_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo gvSupplementiContributiviENPALS_onPageIndexChanging" + ex);
            }
        }

        #endregion Grid Contributivi ENPALS

        #region Private Methods Grid Contributivi ENPALS
        private bool IsListaEmptyContribENPALS()
        {
            List<DatiSupplementiENPALS> listaDatiSuppl = ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] as List<DatiSupplementiENPALS>;

            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].ImportoContributivoTotale.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].Montante.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].CoefficienteTrasformazione.ToString())
                && string.IsNullOrEmpty(listaDatiSuppl[0].Decorrenza.ToString()))
                return true;
            else
                return false;
        }

        private void GvSupplementiContributiviENPALS_Load()
        {
            try
            {
                List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] as List<DatiSupplementiENPALS>;
                gvSupplementiContributiviENPALS.DataSource = elencoSupplementi;
                gvSupplementiContributiviENPALS.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo GvSupplementiENPALS_Load " + ex);
            }
        }

        private void setCampiEditContribENPALS(GridViewRowEventArgs e)
        {
            TextBox txtImportoContributivoTotale = (TextBox)e.Row.FindControl("txtImportoContributivoTotale");
            TextBox txtMontante = (TextBox)e.Row.FindControl("txtMontante");
            TextBox txtCoefficienteTrasformazione = (TextBox)e.Row.FindControl("txtCoefficienteTrasformazione");
            Label lblDecorrenza = (Label)e.Row.FindControl("lblDecorrenza");
            DropDownList ddlQuota = (DropDownList)e.Row.FindControl("ddlQuota");

            txtImportoContributivoTotale.Text = ((DatiSupplementiENPALS)e.Row.DataItem).ImportoContributivoTotale.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).ImportoContributivoTotale.ToString() : string.Empty;
            txtMontante.Text = ((DatiSupplementiENPALS)e.Row.DataItem).Montante.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).Montante.ToString() : string.Empty;
            txtCoefficienteTrasformazione.Text = ((DatiSupplementiENPALS)e.Row.DataItem).CoefficienteTrasformazione.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).CoefficienteTrasformazione.ToString() : string.Empty;
            ddlQuota.SelectedValue = ((DatiSupplementiENPALS)e.Row.DataItem).Quota.HasValue ? ((DatiSupplementiENPALS)e.Row.DataItem).Quota.ToString() : string.Empty;

            if ((DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] != null)
                lblDecorrenza.Text = string.Format("{0:MM/yyyy}", ((DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()]).Decorrenza);
        }

        private void EnableEditableModeContribENPALS(TableCell cell_CancelSave)
        {
            cell_CancelSave.Width = new Unit(40, UnitType.Pixel);

            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabSupplementiENPALSContrib";

        }

        private void saveValueRowContribENPALS(List<DatiSupplementiENPALS> elencoSupplementi, GridViewCommandEventArgs e, GridViewRow r)
        {
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtImportoContributivoTotale"))).Text)))
                elencoSupplementi[r.DataItemIndex].ImportoContributivoTotale = decimal.Parse(((TextBox)(r.FindControl("txtImportoContributivoTotale"))).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtMontante"))).Text)))
                elencoSupplementi[r.DataItemIndex].Montante = Decimal.Parse(((TextBox)(r.FindControl("txtMontante"))).Text);
            if (!String.IsNullOrEmpty((((TextBox)(r.FindControl("txtCoefficienteTrasformazione"))).Text)))
                elencoSupplementi[r.DataItemIndex].CoefficienteTrasformazione = Decimal.Parse(((TextBox)(r.FindControl("txtCoefficienteTrasformazione"))).Text);
            if (!String.IsNullOrEmpty((((DropDownList)(r.FindControl("ddlQuota"))).Text)))
                elencoSupplementi[r.DataItemIndex].Quota = char.Parse(((DropDownList)(r.FindControl("ddlQuota"))).Text);
        }

        private void inserisciSupplementiContribENPALS()
        {
            try
            {
                List<DatiSupplementiENPALS> elencoSupplementi = ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] as List<DatiSupplementiENPALS>;
                DatiSupplementiENPALS supp = new DatiSupplementiENPALS();
                elencoSupplementi.Add(supp);
                ViewState[EnumViewState.ElencoSupplementiContribENPALS.ToString()] = elencoSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementiENPALS, Errore nel metodo InserisciSupplementiContribENPALS " + ex);
            }
        }

        private void AddItemBlankContributiviENPALS(ref List<DatiSupplementiENPALS> lista)
        {
            if (lista != null)
            {
                int index = lista.FindIndex(delegate (DatiSupplementiENPALS code)
                {
                    return (string.IsNullOrEmpty(code.ImportoContributivoTotale.ToString()) && string.IsNullOrEmpty(code.Montante.ToString()) &&
                        string.IsNullOrEmpty(code.CoefficienteTrasformazione.ToString()) && string.IsNullOrEmpty(code.Decorrenza.ToString()));
                }
                    );

                if (index < 0)
                {
                    lista.Add(new DatiSupplementiENPALS());
                }
            }
        }

        private void removeItemBlankContributiviENPALS(ref List<DatiSupplementiENPALS> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (DatiSupplementiENPALS code)
                {
                    return (string.IsNullOrEmpty(code.ImportoContributivoTotale.ToString()) && string.IsNullOrEmpty(code.Montante.ToString()) &&
                        string.IsNullOrEmpty(code.CoefficienteTrasformazione.ToString()) && string.IsNullOrEmpty(code.Decorrenza.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        #endregion Private Methods Grid Contributivi ENPALS

        #region EventHandler

        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler HideTastoSalva;
        public event EventHandler ShowTastoSalva;
        public event EventHandler InitData;
        public event Utility.CustomEventHandler SalvaSupplementi;
        public event Utility.CustomEventHandler EliminaSupplementi;
        public event EventHandler HideAvviso;

        protected void RaiseDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (DisabilitaTastoSalva != null)
                DisabilitaTastoSalva(sender, e);
        }

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        protected void RaiseHideTastoSalva(object sender, EventArgs e)
        {
            if (HideTastoSalva != null)
                HideTastoSalva(sender, e);
        }

        protected void RaiseShowTastoSalva(object sender, EventArgs e)
        {
            if (ShowTastoSalva != null)
                ShowTastoSalva(sender, e);
        }

        protected void RaiseInitData(object sender, EventArgs e)
        {
            if (InitData != null)
                InitData(sender, e);
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

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        #endregion EventHandler

        #region Enum

        enum EnumViewState
        {
            ElencoSupplementiENPALSRetrib,
            ElencoSupplementiContribENPALS,
            ElencoRecordSupplementi,
            RecordSupplementiSelezionato,
        }

        enum colonnegvRecordSupplementiENPALS
        {
            Semaforo = 0,
            Dettaglio = 5,
            Delete = 6,
        }

        #endregion Enum

        internal bool IsPageDettaglioVisible()
        {
            return ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] != null;
        }

        internal bool IsDettaglioSalvato()
        {
            bool ret = false;
            DatiSuppRecordENPALS record = (DatiSuppRecordENPALS)ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()];
            if (record != null)
                ret = record.DettaglioSalvato;
            return ret;
        }

        internal bool IsDettaglioDalSAISelezionato()
        {
            DatiSuppRecordENPALS recordSelezionato = ViewState[EnumViewState.RecordSupplementiSelezionato.ToString()] as DatiSuppRecordENPALS;
            if (recordSelezionato != null)
                if (recordSelezionato.IsFromSas)
                    return true;

            return false;
        }
    }
}