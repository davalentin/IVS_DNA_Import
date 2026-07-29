using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCInail : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        internal void ValorizzaEtichetteInail(ILiquidazionePensioneAgo liquidazionePensione)
        {
            if (this.domanda != null && this.domanda.IsDomandaINPDAP)
                pnlAssegnoAcc.Visible = false;
            else
                ManageAssegnoAccompagnamento(liquidazionePensione);

            if (liquidazionePensione != null && liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail != null)
            {
                this.txtDecAssegno.Text = liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.DecorrenzaAssegnoAccompangamento.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.DecorrenzaAssegnoAccompangamento.Value) : string.Empty;
                this.txtCesAssegno.Text = liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.CessazioneAssegnoAccompangamento.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.CessazioneAssegnoAccompangamento.Value) : string.Empty;
                this.ddlDiritto.SelectedIndex = (liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.DirittoAssegnoAccompagnamento.HasValue && liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.DirittoAssegnoAccompagnamento.Value) ? 1 : (liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.DirittoAssegnoAccompagnamento.HasValue && !liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.DirittoAssegnoAccompagnamento.Value) ? 2 : 0;

                BindData(liquidazionePensione);
            }
            else
            {
                this.txtDecAssegno.Text = string.Empty;
                this.txtCesAssegno.Text = string.Empty;
                this.ddlDiritto.SelectedIndex = 0;
                List<Inail> elencoInail = new List<Inail>();
                ViewState[EnumViewState.ElencoInail.ToString()] = elencoInail;
                gvRenditaINAIL_Load();
            }
        }

        private void BindData(ILiquidazionePensioneAgo liquidazionePensione)
        {
            List<Inail> elencoInail = new List<Inail>();
            if (liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.LpensioniInail != null && liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.LpensioniInail.Count() > 0)
            {
                foreach (Presenter.SvrLiquidazioneAgo.DatiInail.PensioniInail pi in liquidazionePensione.areaLiquidazionePensioneAgo.DatiInail.LpensioniInail)
                {
                    Inail Inail = new Inail();
                    Inail.Decorrenza = pi.DecorrenzaRenditaInail.HasValue ? String.Format("{0:MM/yyyy}", pi.DecorrenzaRenditaInail.Value) : string.Empty;
                    Inail.Evento = pi.Evento.HasValue && pi.Evento.Value == true ? "SI" : "NO";
                    Inail.Importo = pi.ImportoMensileInail.HasValue ? pi.ImportoMensileInail.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    elencoInail.Add(Inail);
                }
            }
            ViewState[EnumViewState.ElencoInail.ToString()] = elencoInail;
            gvRenditaINAIL_Load();
        }

        internal DatiInail GetValoriInail()
        {
            AreaLiquidazionePensione areaLiquidazionePensione = new AreaLiquidazionePensione();
            areaLiquidazionePensione.DatiInail = new DatiInail();
            if (!String.IsNullOrEmpty(this.txtDecAssegno.Text))
                areaLiquidazionePensione.DatiInail.DecorrenzaAssegnoAccompangamento = Convert.ToDateTime(this.txtDecAssegno.Text);
            if (!String.IsNullOrEmpty(this.txtCesAssegno.Text))
                areaLiquidazionePensione.DatiInail.CessazioneAssegnoAccompangamento = Convert.ToDateTime(this.txtCesAssegno.Text);
            if (this.ddlDiritto.SelectedIndex != 0)
                areaLiquidazionePensione.DatiInail.DirittoAssegnoAccompagnamento = Convert.ToBoolean(this.ddlDiritto.SelectedValue);

            List<DatiInail.PensioniInail> LpensioniInail = GetDatiRenditaInail();

            if (LpensioniInail != null)
                areaLiquidazionePensione.DatiInail.LpensioniInail = LpensioniInail.ToArray();

            return areaLiquidazionePensione.DatiInail;
        }

        private List<DatiInail.PensioniInail> GetDatiRenditaInail()
        {
            List<Presenter.SvrLiquidazioneAgo.DatiInail.PensioniInail> LpensioniInail = null;
            List<Inail> elencoInail = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
            removeItemBlank(ref elencoInail);
            if (elencoInail != null)
            {
                LpensioniInail = new List<DatiInail.PensioniInail>();
                foreach (Inail Inail in elencoInail)
                {
                    DatiInail.PensioniInail pensioniInail = new DatiInail.PensioniInail();

                    if (!String.IsNullOrEmpty(Inail.Decorrenza))
                        pensioniInail.DecorrenzaRenditaInail = Convert.ToDateTime(Inail.Decorrenza);
                    if (!String.IsNullOrEmpty(Inail.Evento))
                        pensioniInail.Evento = Inail.Evento == "SI" ? true : false;
                    if (!String.IsNullOrEmpty(Inail.Importo))
                        pensioniInail.ImportoMensileInail = Convert.ToDecimal(Inail.Importo);

                    LpensioniInail.Add(pensioniInail);
                }
            }
            return LpensioniInail;
        }

        protected void SalvaInail_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiInail = GetValoriInail();

            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            presenterLiquidazionePensione.SalvaDatiInailAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaInail_Click(Object sender, EventArgs e)
        {
            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            presenterLiquidazionePensione.EliminaDatiInailAgo(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Inail/Accompagnamento";
            }
            else
            {
                modalitaEditRenditaINAIL.Value = "false";
                Inail Inail = new Inail();
                List<Inail> elencoInail = new List<Inail>();
                elencoInail.Add(Inail);
                ViewState[EnumViewState.ElencoInail.ToString()] = elencoInail;
                gvRenditaINAIL_Load();

                ValorizzaEtichetteInail(null);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        #region Private Methods

        private void RenderVisibleControls(GridViewRow row, bool btnSave, bool btnEdit, bool btnInsert, bool btnDelete, int numColums)
        {
            ((LinkButton)(row.Cells[0].FindControl("btnSave"))).Visible = btnSave;
            ((LinkButton)(row.Cells[0].FindControl("btnEdit"))).Visible = btnEdit;
            ((LinkButton)(row.Cells[0].FindControl("btnAnnulla"))).Visible = btnSave;
            ((LinkButton)(row.Cells[0].FindControl("btnInsert"))).Visible = btnInsert;
            ((LinkButton)(row.Cells[numColums].FindControl("btnDelete"))).Visible = btnDelete;

            for (int i = 1; i < numColums; i++)
            {
                if (i % 2 == 0)
                    row.Cells[i].Visible = btnSave;
                else
                    row.Cells[i].Visible = btnDelete;
            }
        }

        private void gvRenditaINAIL_Load()
        {
            List<Inail> elencoInail = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];

            Inail Empty = elencoInail.Find(delegate (Inail code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty && code.Evento == string.Empty);
            }
            );

            if (Empty == null)
            {
                elencoInail.Add(new Inail(string.Empty, string.Empty, string.Empty));
            }

            gvRenditaINAIL.DataSource = elencoInail;
            gvRenditaINAIL.DataBind();
        }

        private void removeItemBlank(ref List<Inail> lista)
        {
            int index = lista.FindIndex(delegate (Inail code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty && code.Evento == string.Empty);
            });

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private bool IsListaEmpty()
        {
            List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
            if (listaInailApp.Count == 1 && listaInailApp[0].Decorrenza == string.Empty && listaInailApp[0].Importo == string.Empty
                && listaInailApp[0].Evento == string.Empty)
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl("txtDecorrenza") != null && ((TextBox)row.FindControl("txtDecorrenza")).Text != string.Empty &&
                row.FindControl("txtImporto") != null && ((TextBox)row.FindControl("txtImporto")).Text != string.Empty &&
                row.FindControl("ddlEvento") != null && ((DropDownList)row.FindControl("ddlEvento")).SelectedIndex != 0)

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRow(GridViewRow row)
        {
            if (row.FindControl("lblDecorrenza") != null && ((Label)row.FindControl("lblDecorrenza")).Text != string.Empty &&
                row.FindControl("lblImporto") != null && ((Label)row.FindControl("lblImporto")).Text != string.Empty &&
                row.FindControl("lblEvento_item") != null && ((Label)row.FindControl("lblEvento_item")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void EnableEditableMode(TableCell cell_CancelSave)
        {
            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabGridINAIL";
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteRenditaInail")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private List<Inail> AddRecord(List<Inail> listaRecord, String decorrenza, String importo, String evento)
        {
            listaRecord.Add(new Inail(decorrenza, importo, evento));
            return listaRecord;
        }

        private void ManageAssegnoAccompagnamento(ILiquidazionePensioneAgo liquidazionePensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            //TRF - Domande con gruppo 0002 oppure 0003 nel caso di TRF, cioè alle stesse condizioni delle PL
            //RIC - Domande con gruppo 0031 e prodotto che inizia per “03xx” && primo byte del codice natura è pari a 3 o 4
            if ((tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione ||
                (datiPensione.CodeGruppo == "0031" && datiPensione.CodeProdotto.StartsWith("03") &&
                (liquidazionePensione != null && liquidazionePensione.areaLiquidazionePensioneAgo != null && liquidazionePensione.areaLiquidazionePensioneAgo.DatiGenerici != null &&
                liquidazionePensione.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione != null &&
                (liquidazionePensione.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.StartsWith("3") || liquidazionePensione.areaLiquidazionePensioneAgo.DatiGenerici.NaturaPensione.StartsWith("4")))))
                && !Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
            {
                pnlAssegnoAcc.Visible = true;
            }
            else
            {
                pnlAssegnoAcc.Visible = false;
            }
        }
        #endregion Private Methods

        #region Rendita Inail

        private void RimuoviDallaGriglia(ref List<Inail> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        protected void gvRenditaINAIL_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty() && !Convert.ToBoolean(modalitaEditRenditaINAIL.Value))
                        {
                            gvRenditaINAIL.EditIndex = 0;
                            modalitaEditRenditaINAIL.Value = "true";

                            gvRenditaINAIL.DataSource = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                            gvRenditaINAIL.DataBind();
                        }
                        else if (IsEmptyEditableRow(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                DropDownList ddlEvento = new DropDownList();
                                ddlEvento = (DropDownList)e.Row.FindControl("ddlEvento");
                                ddlEvento.SelectedValue = ((Inail)(e.Row.DataItem)).Evento;

                                EnableEditableMode(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[3].FindControl("btnDeleteRenditaInail")));
                                delete.Text = string.Empty;

                                if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == true)
                                    btnSalvaBititolaritaInail.Enabled = false;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((Inail)(e.Row.DataItem)).Decorrenza);
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((Inail)(e.Row.DataItem)).Importo;
                                ((Label)e.Row.FindControl("lblEvento_item")).Text = ((Inail)(e.Row.DataItem)).Evento;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3]);
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                DropDownList ddlEvento = new DropDownList();
                                ddlEvento = (DropDownList)e.Row.FindControl("ddlEvento");
                                ddlEvento.SelectedValue = ((Inail)(e.Row.DataItem)).Evento;

                                EnableEditableMode(e.Row.Cells[0]);

                                if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == true)
                                    btnSalvaBititolaritaInail.Enabled = false;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((Inail)(e.Row.DataItem)).Decorrenza);
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((Inail)(e.Row.DataItem)).Importo;
                                ((Label)e.Row.FindControl("lblEvento_item")).Text = ((Inail)(e.Row.DataItem)).Evento;
                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            DropDownList ddlEvento = new DropDownList();
                            ddlEvento = (DropDownList)e.Row.FindControl("ddlEvento");
                            ddlEvento.SelectedValue = ((Inail)(e.Row.DataItem)).Evento;

                            EnableEditableMode(e.Row.Cells[0]);

                            if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == true)
                                btnSalvaBititolaritaInail.Enabled = false;
                        }

                        else if (e.Row.DataItemIndex == ((List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));

                            //controllo necessario per inserire al max 45 diverse occorrenze
                            if (((List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()]).Count == 46)
                            {
                                add.Text = string.Empty;
                            }
                            else
                            {
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((Inail)(e.Row.DataItem)).Decorrenza);
                            ((Label)e.Row.FindControl("lblImporto")).Text = ((Inail)(e.Row.DataItem)).Importo;
                            ((Label)e.Row.FindControl("lblEvento_item")).Text = ((Inail)(e.Row.DataItem)).Evento;
                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3]);
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
                throw new INPS.DNA.DnaApplicationException("UCInail, Errore nel metodo gvRenditaINAIL_RowDataBound " + ex);
            }
        }

        protected void gvRenditaINAIL_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.RenditaInail_HdnId);
                int index = listaInailApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref listaInailApp, index);

                this.modalitaEditRenditaINAIL.Value = "false";
                gvRenditaINAIL.EditIndex = -1;
                ViewState[EnumViewState.ElencoInail.ToString()] = listaInailApp;

                gvRenditaINAIL_Load();
                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                // serve nel momento in cui il record vuoto non è presente
                List<Inail> listaInailApp = ((List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()]);
                removeItemBlank(ref listaInailApp);
                listaInailApp.Add(new Inail(string.Empty, string.Empty, string.Empty));
                ViewState[EnumViewState.ElencoInail.ToString()] = listaInailApp;

                if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == true)
                    btnSalvaBititolaritaInail.Enabled = false;
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.RenditaInail_HdnId);
                    int index = listaInailApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    listaInailApp[index].Decorrenza = ((TextBox)r.FindControl(Keys.RenditaInail_TxtDecorrenza)).Text;
                    listaInailApp[index].Importo = ((TextBox)r.FindControl(Keys.RenditaInail_TxtImporto)).Text;
                    listaInailApp[index].Evento = ((DropDownList)r.FindControl(Keys.RenditaInail_DdlEvento)).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaInailApp.Count - 1)
                        listaInailApp.Add(new Inail(string.Empty, string.Empty, string.Empty));

                    gvRenditaINAIL.EditIndex = -1;
                    ViewState[EnumViewState.ElencoInail.ToString()] = listaInailApp;

                    gvRenditaINAIL.DataSource = listaInailApp;
                    gvRenditaINAIL.DataBind();

                    if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == false)
                        btnSalvaBititolaritaInail.Enabled = true;
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                if (!IsListaEmpty())
                {
                    gvRenditaINAIL.EditIndex = -1;
                    gvRenditaINAIL.DataSource = listaInailApp;
                    gvRenditaINAIL.DataBind();

                    if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == false)
                        btnSalvaBititolaritaInail.Enabled = true;
                }
            }
        }

        protected void gvRenditaINAIL_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRenditaINAIL.EditIndex = e.NewEditIndex;
                List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                gvRenditaINAIL.DataSource = listaInailApp;
                gvRenditaINAIL.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCInail, Errore nel metodo gvRenditaINAIL_RowEditing " + ex);
            }
        }

        protected void gvRenditaINAIL_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvRenditaINAIL.EditIndex = -1;

                List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                gvRenditaINAIL.DataSource = listaInailApp;
                gvRenditaINAIL.DataBind();

                if (!pnlAssegnoAcc.Visible && btnSalvaBititolaritaInail.Enabled == false)
                    btnSalvaBititolaritaInail.Enabled = true;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCInail, Errore nel metodo gvRenditaINAIL_RowCancelingEdit " + ex);
            }
        }

        protected void gvRenditaINAIL_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvRenditaINAIL_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRenditaINAIL.PageIndex = e.NewPageIndex;

                List<Inail> listaInailApp = (List<Inail>)ViewState[EnumViewState.ElencoInail.ToString()];
                gvRenditaINAIL.DataSource = listaInailApp;
                gvRenditaINAIL.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Aggiornamenti, Errore nel metodo gvRenditaINAIL_onPageIndexChanging: " + ex);
            }
        }

        #endregion Rendita Inail

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        [Serializable]
        public class Inail
        {
            public Inail()
            {
                this.Id = Guid.NewGuid();
            }

            public Inail(string decorrenza, string importo, string evento)
            {
                this.Id = Guid.NewGuid();
                this._Decorrenza = decorrenza;
                this._Importo = importo;
                this._Evento = evento;
            }

            private string _Decorrenza;
            private string _Importo;
            private string _Evento;

            public Guid Id { get; private set; }
            public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public string Importo { get { return _Importo; } set { _Importo = value; } }
            public string Evento { get { return _Evento; } set { _Evento = value; } }
        }

        #region enums
        public enum EnumViewState
        {
            ElencoInail
        }
        #endregion enums

        #region Keys
        public class Keys
        {
            public const string RenditaInail_HdnId = "hdnGUID";
            public const string RenditaInail_TxtDecorrenza = "txtDecorrenza";
            public const string RenditaInail_TxtImporto = "txtImporto";
            public const string RenditaInail_DdlEvento = "ddlEvento";
        }
        #endregion Keys
    }
}