using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Bititolarita
{
    public partial class UCAltrePensioniAgo : CustomBaseUserControl, IBititolarita
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaAltrePensioni_Click(Object sender, EventArgs e)
        {
            areaDatiBititolaritaAgo = new AreaDatiBititolarita();
            areaDatiBititolaritaAgo = GetDatiAltrePensioni();

            PresenterBititolarita presenterBititolarita = new PresenterBititolarita();
            presenterBititolarita.SalvaAltrePensioniAgo(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza, null);
            RaiseShowAvviso(this, Cevent);
        }

        protected void EliminaAltrePensioni_Click(Object sender, EventArgs e)
        {
            PresenterBititolarita presenterBititolarita = new PresenterBititolarita();
            presenterBititolarita.EliminaAltrePensioniAgo(this);

            if (!this.HasError)
            {
                ClearForm();
                ValorizzaEtichetteAltrePensioni(this);
            }
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(this.domanda.TipoAppartenenza, null);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteAltrePensioni(IBititolarita bititolarita)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (Utility.IsDomandaVOAUT(this.domanda.Categoria))
            {
                lblTestoVOAUT.Visible = true;
            }

            LoadDecodificaData(bititolarita);

            #region GridView Dati Retributivi

            List<AltrePensioniLocalAgo> elencoAltrePensioni = new List<AltrePensioniLocalAgo>();

            if (bititolarita != null && bititolarita.areaDatiBititolaritaAgo != null)
            {
                if (bititolarita.areaDatiBititolaritaAgo.ElencoAltraPensione != null && bititolarita.areaDatiBititolaritaAgo.ElencoAltraPensione.ToList().Count() > 0)
                {
                    foreach (AltraPensione altraPensione in bititolarita.areaDatiBititolaritaAgo.ElencoAltraPensione.ToList())
                    {
                        AltrePensioniLocalAgo altraPensioneLocal = new AltrePensioniLocalAgo();
                        altraPensioneLocal.CodiceCategoria = !string.IsNullOrEmpty(altraPensione.Categoria) ? altraPensione.Categoria.Trim() : string.Empty;
                        altraPensioneLocal.Certificato = altraPensione.Certificato.HasValue ? altraPensione.Certificato.Value.ToString().PadLeft(8, '0') : string.Empty;
                        altraPensioneLocal.Cessazione = altraPensione.Cessazione.HasValue ? String.Format("{0:MM/yyyy}", altraPensione.Cessazione.Value) : string.Empty;
                        altraPensioneLocal.Decorrenza = altraPensione.Decorrenza.HasValue ? String.Format("{0:MM/yyyy}", altraPensione.Decorrenza.Value) : string.Empty;
                        altraPensioneLocal.CodiceImporto = altraPensione.CodiceImporto.HasValue ? altraPensione.CodiceImporto.Value.ToString() : string.Empty;
                        altraPensioneLocal.CodiceUC = altraPensione.CodiceUC.HasValue ? altraPensione.CodiceUC.Value.ToString() : string.Empty;
                        altraPensioneLocal.CodiceEnte = altraPensione.Ente.HasValue ? altraPensione.Ente.Value.ToString() : string.Empty;

                        elencoAltrePensioni.Add(altraPensioneLocal);
                    }
                }
            }


            ViewState[EnumViewState.ElencoAltrePensioni.ToString()] = elencoAltrePensioni;

            gvAltrePensioni_Load();

            #endregion GridView Dati Retributivi
        }

        internal AreaDatiBititolarita GetDatiAltrePensioni()
        {
            this.areaDatiBititolaritaAgo = new AreaDatiBititolarita();

            List<AltrePensioniLocalAgo> elencoAltrePensioni = ((List<AltrePensioniLocalAgo>)(ViewState[EnumViewState.ElencoAltrePensioni.ToString()]));
            removeItemBlank(ref elencoAltrePensioni);

            List<AltraPensione> elencoAltraPensioneSvr = null;
            if (elencoAltrePensioni != null && elencoAltrePensioni.Count > 0)
            {
                elencoAltraPensioneSvr = new List<AltraPensione>();
                foreach (AltrePensioniLocalAgo dati in elencoAltrePensioni)
                {
                    AltraPensione datiSvr = new AltraPensione();

                    datiSvr.Categoria = !string.IsNullOrEmpty(dati.CodiceCategoria) ? dati.CodiceCategoria.Trim().ToUpperInvariant() : string.Empty;
                    datiSvr.Certificato = !string.IsNullOrEmpty(dati.Certificato) ? int.Parse(dati.Certificato) : (int?)null;
                    datiSvr.Cessazione = !string.IsNullOrEmpty(dati.Cessazione) ? Utility.GetDateFromString(dati.Cessazione) : (DateTime?)null;
                    datiSvr.CodiceImporto = !string.IsNullOrEmpty(dati.CodiceImporto) ? char.Parse(dati.CodiceImporto) : (char?)null;
                    datiSvr.CodiceUC = !string.IsNullOrEmpty(dati.CodiceUC) ? char.Parse(dati.CodiceUC) : (char?)null;
                    datiSvr.Decorrenza = !string.IsNullOrEmpty(dati.Decorrenza) ? Utility.GetDateFromString(dati.Decorrenza) : (DateTime?)null;
                    datiSvr.Ente = !string.IsNullOrEmpty(dati.CodiceEnte) ? byte.Parse(dati.CodiceEnte) : (byte?)null;

                    elencoAltraPensioneSvr.Add(datiSvr);
                }
            }

            this.areaDatiBititolaritaAgo.ElencoAltraPensione = elencoAltraPensioneSvr != null ? elencoAltraPensioneSvr.ToArray() : null;

            return this.areaDatiBititolaritaAgo;
        }

        #region Grid View Altre Pensioni

        private void RimuoviDallaGriglia(ref List<AltrePensioniLocalAgo> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        protected void gvAltrePensioni_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            this.areaDatiBititolaritaAgo = ((AreaDatiBititolarita)ViewState["DatiBititolarita"]);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty() && !Convert.ToBoolean(modalitaEditAltrePensioni.Value))
                        {
                            gvAltrePensioni.EditIndex = 0;
                            modalitaEditAltrePensioni.Value = "true";
                            GestioneTastoSalva();

                            gvAltrePensioni.DataSource = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                            gvAltrePensioni.DataBind();
                        }
                        else if (IsEmptyEditableRow(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row);
                                EnableEditableMode(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteAltrePensioni")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceCategoria_item")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceCategoria;
                                ((Label)e.Row.FindControl("lblCertificato")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Certificato;
                                ((Label)e.Row.FindControl("lblCodiceEnte")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceEnte;
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblCodiceUC_item")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceUC;
                                ((Label)e.Row.FindControl("lblCodiceImporto")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceImporto;
                                ((Label)e.Row.FindControl("lblCessazione")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Cessazione;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5]);
                            }

                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row);
                                EnableEditableMode(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceCategoria_item")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceCategoria;
                                ((Label)e.Row.FindControl("lblCertificato")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Certificato;
                                ((Label)e.Row.FindControl("lblCodiceEnte")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceEnte;
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblCodiceUC_item")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceUC;
                                ((Label)e.Row.FindControl("lblCodiceImporto")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceImporto;
                                ((Label)e.Row.FindControl("lblCessazione")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Cessazione;
                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            GestioneDdls(e.Row);
                            EnableEditableMode(e.Row.Cells[0]);
                        }

                        else if (e.Row.DataItemIndex == ((List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));

                            if (((List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()]).Count == 6)
                                add.Text = string.Empty;
                            else
                            {
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblCodiceCategoria_item")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceCategoria;
                            ((Label)e.Row.FindControl("lblCertificato")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Certificato;
                            ((Label)e.Row.FindControl("lblCodiceEnte")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceEnte;
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Decorrenza;
                            ((Label)e.Row.FindControl("lblCodiceUC_item")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceUC;
                            ((Label)e.Row.FindControl("lblCodiceImporto")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).CodiceImporto;
                            ((Label)e.Row.FindControl("lblCessazione")).Text = ((AltrePensioniLocalAgo)(e.Row.DataItem)).Cessazione;
                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4]);
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
                throw new INPS.DNA.DnaApplicationException("UCAltrePensioni, Errore nel metodo gvAltrePensioni_RowDataBound " + ex);
            }
        }

        protected void gvAltrePensioni_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.AltrePensioni_HdnId);
                int index = listaAltrePensioniApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref listaAltrePensioniApp, index);

                this.modalitaEditAltrePensioni.Value = "false";
                gvAltrePensioni.EditIndex = -1;
                ViewState[EnumViewState.ElencoAltrePensioni.ToString()] = listaAltrePensioniApp;

                gvAltrePensioni_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditAltrePensioni.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.AltrePensioni_HdnId);
                    int index = listaAltrePensioniApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    listaAltrePensioniApp[index].CodiceCategoria = ((TextBox)r.FindControl(Keys.AltrePensioni_TxtCodiceCategoria)).Text;
                    listaAltrePensioniApp[index].Certificato = ((TextBox)r.FindControl(Keys.AltrePensioni_TxtCertificato)).Text;
                    listaAltrePensioniApp[index].CodiceEnte = GetCodEnteByCategoria(listaAltrePensioniApp[index].CodiceCategoria);
                    listaAltrePensioniApp[index].Decorrenza = ((TextBox)r.FindControl(Keys.AltrePensioni_TxtDecorrenza)).Text;
                    listaAltrePensioniApp[index].CodiceUC = ((DropDownList)r.FindControl(Keys.AltrePensioni_DdlCodiceUC)).SelectedValue;
                    listaAltrePensioniApp[index].CodiceImporto = ((DropDownList)r.FindControl(Keys.AltrePensioni_DdlCodiceImporto)).SelectedValue;
                    listaAltrePensioniApp[index].Cessazione = ((TextBox)r.FindControl(Keys.AltrePensioni_TxtCessazione)).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaAltrePensioniApp.Count - 1)
                        listaAltrePensioniApp.Add(new AltrePensioniLocalAgo(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                    gvAltrePensioni.EditIndex = -1;
                    ViewState[EnumViewState.ElencoAltrePensioni.ToString()] = listaAltrePensioniApp;

                    modalitaEditAltrePensioni.Value = "false";

                    gvAltrePensioni.DataSource = listaAltrePensioniApp;
                    gvAltrePensioni.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                if (!IsListaEmpty())
                {
                    modalitaEditAltrePensioni.Value = "false";

                    gvAltrePensioni.EditIndex = -1;
                    gvAltrePensioni.DataSource = listaAltrePensioniApp;
                    gvAltrePensioni.DataBind();
                }
            }

            GestioneTastoSalva();
        }

        protected void gvAltrePensioni_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAltrePensioni.EditIndex = e.NewEditIndex;
                List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                gvAltrePensioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAltrePensioni, Errore nel metodo gvAltrePensioni_RowEditing " + ex);
            }
        }

        protected void gvAltrePensioni_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvAltrePensioni.EditIndex = -1;

                List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                gvAltrePensioni.DataSource = listaAltrePensioniApp;
                gvAltrePensioni.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAltrePensioni, Errore nel metodo gvAltrePensioni_RowCancelingEdit " + ex);
            }
        }

        protected void gvAltrePensioni_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvAltrePensioni_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAltrePensioni.PageIndex = e.NewPageIndex;

                List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
                gvAltrePensioni.DataSource = listaAltrePensioniApp;
                gvAltrePensioni.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAltrePensioni, Errore nel metodo gvAltrePensioni_onPageIndexChanging: " + ex);
            }
        }

        #endregion Grid View Altre Pensioni

        #region Private Methods

        private void LoadDecodificaData(IBititolarita bititolarita)
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();

            AreaDecodifica.DatiCodiciImportoAltraPensione[] listaImportoAltraPensione = valoriDecodificati.ElencoCodiciImportoAltraPensione;
            ViewState["listaCodeImportoAltraPensione"] = listaImportoAltraPensione;

            ViewState["listaDecodificaCatEnte"] = bititolarita.areaDatiBititolaritaAgo.ElencoCatEnte;
        }

        private void GestioneTastoSalva()
        {
            if (modalitaEditAltrePensioni.Value == "false")
            {
                btnSalvaAltrePensioni.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
            else
            {
                btnSalvaAltrePensioni.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
        }

        private void GestioneDdls(GridViewRow row)
        {
            DropDownList ddlCodImporto = new DropDownList();
            ddlCodImporto = (DropDownList)row.FindControl(Keys.AltrePensioni_DdlCodiceImporto);

            AreaDecodifica.DatiCodiciImportoAltraPensione[] listaCodeImportoAltraPensione = (AreaDecodifica.DatiCodiciImportoAltraPensione[])ViewState["listaCodeImportoAltraPensione"];

            #region Lista Importo Altra Pensione
            List<AreaDecodifica.DatiCodiciImportoAltraPensione> listaCodeImportoAltraPensione_app = listaCodeImportoAltraPensione.ToList();
            listaCodeImportoAltraPensione_app.Sort((x, y) => string.Compare(x.Id.Trim(), y.Id.Trim(), false, CultureInfo.InvariantCulture));
            listaCodeImportoAltraPensione = listaCodeImportoAltraPensione_app.ToArray();

            foreach (AreaDecodifica.DatiCodiciImportoAltraPensione datiCodeImportoAltraPensione in listaCodeImportoAltraPensione)
            {
                ListItem li = new ListItem();
                li.Attributes.Add("title", datiCodeImportoAltraPensione.Descrizione);
                li.Text = datiCodeImportoAltraPensione.Id;
                li.Value = datiCodeImportoAltraPensione.Id;
                ddlCodImporto.Items.Add(li);
            }

            if (((AltrePensioniLocalAgo)(row.DataItem)).CodiceImporto.Trim() == string.Empty)
                ddlCodImporto.SelectedIndex = 0;
            else
                ddlCodImporto.Items.FindByValue(((AltrePensioniLocalAgo)(row.DataItem)).CodiceImporto.Trim()).Selected = true;
            #endregion Lista Importo Altra Pensione

            #region Lista Codice U/C
            DropDownList ddlCodeUC = new DropDownList();
            ddlCodeUC = (DropDownList)row.FindControl(Keys.AltrePensioni_DdlCodiceUC);
            ddlCodeUC.SelectedValue = ((AltrePensioniLocalAgo)(row.DataItem)).CodiceUC;
            #endregion Lista Codice U/C
        }

        private string GetCodEnteByCategoria(string codCategoria)
        {
            int categoriaNumerica = 0;
            int.TryParse(codCategoria, out categoriaNumerica);

            string codiceEnte = string.Empty;
            if (ViewState["listaDecodificaCatEnte"] != null)
            {
                GestioneBititolaritaDecCatEnte[] listaCatEnte = (GestioneBititolaritaDecCatEnte[])ViewState["listaDecodificaCatEnte"];
                if (listaCatEnte != null && listaCatEnte.Length > 0)
                {
                    GestioneBititolaritaDecCatEnte catEnte = listaCatEnte.FirstOrDefault(x => x.CodCategoria.Trim().ToUpperInvariant() == (categoriaNumerica != 0 ? categoriaNumerica.ToString() : codCategoria.Trim().ToUpperInvariant()));
                    if (catEnte != null)
                        codiceEnte = catEnte.CodEnte.ToString();
                }
            }
            return codiceEnte;
        }

        private void gvAltrePensioni_Load()
        {
            try
            {
                List<AltrePensioniLocalAgo> elencoAltrePensioni = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];

                AltrePensioniLocalAgo Empty = elencoAltrePensioni.Find(delegate (AltrePensioniLocalAgo code)
                {
                    return (code.CodiceCategoria == string.Empty && code.Certificato == string.Empty && code.CodiceEnte == string.Empty && code.Decorrenza == string.Empty && code.CodiceUC == string.Empty &&
                    code.CodiceImporto == string.Empty && code.Cessazione == string.Empty);
                }
                );

                if (Empty == null)
                    elencoAltrePensioni.Add(new AltrePensioniLocalAgo(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvAltrePensioni.DataSource = elencoAltrePensioni;
                gvAltrePensioni.DataBind();
            }
            catch (Exception)
            {
                throw;
            }
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
            save.ValidationGroup = "UCTabAltrePensioni";

        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteAltrePensioni")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private bool IsListaEmpty()
        {
            List<AltrePensioniLocalAgo> listaAltrePensioniApp = (List<AltrePensioniLocalAgo>)ViewState[EnumViewState.ElencoAltrePensioni.ToString()];
            if (listaAltrePensioniApp.Count == 1 && listaAltrePensioniApp[0].CodiceCategoria == string.Empty &&
                listaAltrePensioniApp[0].Certificato == string.Empty && listaAltrePensioniApp[0].CodiceEnte == string.Empty &&
                listaAltrePensioniApp[0].Decorrenza == string.Empty && listaAltrePensioniApp[0].CodiceUC == string.Empty &&
                listaAltrePensioniApp[0].CodiceImporto == string.Empty && listaAltrePensioniApp[0].Cessazione == string.Empty)
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl(Keys.AltrePensioni_TxtCodiceCategoria) != null && ((TextBox)row.FindControl(Keys.AltrePensioni_TxtCodiceCategoria)).Text != string.Empty)
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRow(GridViewRow row)
        {
            if (row.FindControl("lblCodiceCategoria_item") != null && ((Label)row.FindControl("lblCodiceCategoria_item")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void removeItemBlank(ref List<AltrePensioniLocalAgo> lista)
        {

            int index = lista.FindIndex(delegate (AltrePensioniLocalAgo code)
            {
                return (code.CodiceCategoria == string.Empty && code.Certificato == string.Empty && code.CodiceEnte == string.Empty && code.Decorrenza == string.Empty && code.CodiceUC == string.Empty &&
                    code.CodiceImporto == string.Empty && code.Cessazione == string.Empty);
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private List<AltrePensioniLocalAgo> AddRecord(List<AltrePensioniLocalAgo> listaRecord, string codiceCategoria, string certificato, string codiceEnte, string decorrenza, string codiceUC, string codiceImporto, string cessazione)
        {
            listaRecord.Add(new AltrePensioniLocalAgo(codiceCategoria, certificato, codiceEnte, decorrenza, codiceUC, codiceImporto, cessazione));
            return listaRecord;
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        #endregion Private Methods

        #region Events

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

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseShowError(object sender, Utility.CustomEventArgs e)
        {
            ShowError(sender, e);
        }

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event Utility.CustomEventHandler ShowError;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;

        #endregion Events

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IBititolarita
        public Presenter.SvrLiquidazioneAgo.AreaDatiBititolarita areaDatiBititolaritaAgo { get; set; }
        public Presenter.SvrLiquidazioneCi.AreaDatiBititolarita areaDatiBititolaritaCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IBititolarita

        #region Enum
        public enum EnumViewState
        {
            ElencoAltrePensioni
        }
        #endregion Enum

        #region Keys
        public class Keys
        {
            public const string AltrePensioni_HdnId = "hdnGUID";
            public const string AltrePensioni_TxtCodiceCategoria = "txtCodiceCategoria";
            public const string AltrePensioni_TxtCertificato = "txtCertificato";
            public const string AltrePensioni_TxtDecorrenza = "txtDecorrenza";
            public const string AltrePensioni_DdlCodiceUC = "ddlCodiceUC";
            public const string AltrePensioni_DdlCodiceImporto = "ddlCodiceImporto";
            public const string AltrePensioni_TxtCessazione = "txtCessazione";
        }
        #endregion Keys
    }

    [Serializable]
    public class AltrePensioniLocalAgo
    {
        public AltrePensioniLocalAgo()
        {
            this.Id = Guid.NewGuid();
        }

        public AltrePensioniLocalAgo(string codiceCategoria, string certificato, string codiceEnte, string decorrenza, string codiceUC, string codiceImporto, string cessazione)
        {
            this.Id = Guid.NewGuid();
            this._CodiceCategoria = codiceCategoria;
            this._Certificato = certificato;
            this._CodiceEnte = codiceEnte;
            this._Decorrenza = decorrenza;
            this._CodiceUC = codiceUC;
            this._CodiceImporto = codiceImporto;
            this._Cessazione = cessazione;
        }

        private string _CodiceCategoria;
        private string _Certificato;
        private string _CodiceEnte;
        private string _Decorrenza;
        private string _CodiceUC;
        private string _CodiceImporto;
        private string _Cessazione;

        public Guid Id { get; private set; }
        public string CodiceCategoria { get { return _CodiceCategoria; } set { _CodiceCategoria = value; } }
        public string Certificato { get { return _Certificato; } set { _Certificato = value; } }
        public string CodiceEnte { get { return _CodiceEnte; } set { _CodiceEnte = value; } }
        public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public string CodiceUC { get { return _CodiceUC; } set { _CodiceUC = value; } }
        public string CodiceImporto { get { return _CodiceImporto; } set { _CodiceImporto = value; } }
        public string Cessazione { get { return _Cessazione; } set { _Cessazione = value; } }
    }
}
