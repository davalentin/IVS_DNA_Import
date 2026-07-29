using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCBititolaritaINAIL : CustomBaseUserControl, ILiquidazionePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        internal void ValorizzaEtichetteBititolaritaINAIL(ILiquidazionePensione liquidazionePensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (liquidazionePensione != null && liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail != null)
            {
                this.txtDecorrenzaCessazione.Text = liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.CessazioneDirittoIntegrazioneMinimo.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.CessazioneDirittoIntegrazioneMinimo.Value) : string.Empty;
                this.txtDecorrenzaDiritto.Text = liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DecorrenzaDirittoIntegrazioneMinimo.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DecorrenzaDirittoIntegrazioneMinimo.Value) : string.Empty;
                this.txtSospensione.Text = liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.SospensionePensioneInvalidita.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.SospensionePensioneInvalidita.Value) : string.Empty;
                this.txtRipristino.Text = liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.RipristinoPensioneInvalidita.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.RipristinoPensioneInvalidita.Value) : string.Empty;
                this.txtImportoRecuperare.Text = liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.ImportoMensile.HasValue ? liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.ImportoMensile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                this.txtDecAssegno.Text = liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DecorrenzaAssegnoAccompangamento.HasValue ? String.Format("{0:MM/yyyy}", liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DecorrenzaAssegnoAccompangamento.Value) : string.Empty;
                this.ddlDiritto.SelectedIndex = (liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DirittoAssegnoAccompagnamento.HasValue && liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DirittoAssegnoAccompagnamento.Value) ? 1 : (liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DirittoAssegnoAccompagnamento.HasValue && !liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.DirittoAssegnoAccompagnamento.Value) ? 2 : 0;

                BindData(liquidazionePensione);
            }
            else
            {
                this.txtDecorrenzaCessazione.Text = string.Empty;
                this.txtDecorrenzaDiritto.Text = string.Empty;
                this.txtSospensione.Text = string.Empty;
                this.txtRipristino.Text = string.Empty;
                this.txtImportoRecuperare.Text = string.Empty;
                this.txtDecAssegno.Text = string.Empty;
                this.ddlDiritto.SelectedIndex = 0;
                List<BititolaritaINAIL> elencoBititolaritaINAIL = new List<BititolaritaINAIL>();
                ViewState["elencoBititolaritaINAIL"] = elencoBititolaritaINAIL;
                gvRenditaINAIL_Load();
            }
        }

        private void BindData(ILiquidazionePensione liquidazionePensione)
        {
            List<BititolaritaINAIL> elencoBititolaritaINAIL = new List<BititolaritaINAIL>();
            if (liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.LpensioniInail != null && liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.LpensioniInail.Count() > 0)
            {
                foreach (Presenter.SvrLiquidazioneFs.DatiBititolaritaInail.PensioniInail pi in liquidazionePensione.areaLiquidazionePensioneFS.DatiBititolaritaInail.LpensioniInail)
                {
                    BititolaritaINAIL bititolaritaINAIL = new BititolaritaINAIL();
                    bititolaritaINAIL.Decorrenza = pi.DecorrenzaRenditaInail.HasValue ? String.Format("{0:MM/yyyy}", pi.DecorrenzaRenditaInail.Value) : string.Empty;
                    bititolaritaINAIL.Evento = pi.Evento.HasValue && pi.Evento.Value == true ? "SI" : "NO";
                    bititolaritaINAIL.Importo = pi.ImportoMensileInail.HasValue ? pi.ImportoMensileInail.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    elencoBititolaritaINAIL.Add(bititolaritaINAIL);
                }
            }
            ViewState["elencoBititolaritaINAIL"] = elencoBititolaritaINAIL;
            gvRenditaINAIL_Load();
        }

        internal DatiBititolaritaInail GetValoriBititolaritaInail()
        {
            AreaLiquidazionePensione areaLiquidazionePensione = new AreaLiquidazionePensione();
            areaLiquidazionePensione.DatiBititolaritaInail = new DatiBititolaritaInail();
            if (!String.IsNullOrEmpty(this.txtDecorrenzaCessazione.Text))
                areaLiquidazionePensione.DatiBititolaritaInail.CessazioneDirittoIntegrazioneMinimo = Convert.ToDateTime(this.txtDecorrenzaCessazione.Text);
            if (!String.IsNullOrEmpty(this.txtDecorrenzaDiritto.Text))
                areaLiquidazionePensione.DatiBititolaritaInail.DecorrenzaDirittoIntegrazioneMinimo = Convert.ToDateTime(this.txtDecorrenzaDiritto.Text);
            if (!String.IsNullOrEmpty(this.txtSospensione.Text))
                areaLiquidazionePensione.DatiBititolaritaInail.SospensionePensioneInvalidita = Convert.ToDateTime(this.txtSospensione.Text);
            if (!String.IsNullOrEmpty(this.txtRipristino.Text))
                areaLiquidazionePensione.DatiBititolaritaInail.RipristinoPensioneInvalidita = Convert.ToDateTime(this.txtRipristino.Text);
            if (!String.IsNullOrEmpty(this.txtImportoRecuperare.Text))
                areaLiquidazionePensione.DatiBititolaritaInail.ImportoMensile = Convert.ToDecimal(this.txtImportoRecuperare.Text);
            if (!String.IsNullOrEmpty(this.txtDecAssegno.Text))
                areaLiquidazionePensione.DatiBititolaritaInail.DecorrenzaAssegnoAccompangamento = Convert.ToDateTime(this.txtDecAssegno.Text);
            if (this.ddlDiritto.SelectedIndex != 0)
                areaLiquidazionePensione.DatiBititolaritaInail.DirittoAssegnoAccompagnamento = Convert.ToBoolean(this.ddlDiritto.SelectedValue);

            List<DatiBititolaritaInail.PensioniInail> LpensioniInail = GetDatiRenditaInail();

            if (LpensioniInail != null)
                areaLiquidazionePensione.DatiBititolaritaInail.LpensioniInail = LpensioniInail.ToArray();

            return areaLiquidazionePensione.DatiBititolaritaInail;
        }

        private List<Presenter.SvrLiquidazioneFs.DatiBititolaritaInail.PensioniInail> GetDatiRenditaInail()
        {
            List<Presenter.SvrLiquidazioneFs.DatiBititolaritaInail.PensioniInail> LpensioniInail = null;
            List<BititolaritaINAIL> elencoBititolaritaINAIL = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];
            removeItemBlank(ref elencoBititolaritaINAIL);
            if (elencoBititolaritaINAIL != null)
            {
                LpensioniInail = new List<DatiBititolaritaInail.PensioniInail>();
                foreach (BititolaritaINAIL bititolaritaINAIL in elencoBititolaritaINAIL)
                {
                    DatiBititolaritaInail.PensioniInail pensioniInail = new DatiBititolaritaInail.PensioniInail();

                    if (!String.IsNullOrEmpty(bititolaritaINAIL.Decorrenza))
                        pensioniInail.DecorrenzaRenditaInail = Convert.ToDateTime(bititolaritaINAIL.Decorrenza);
                    if (!String.IsNullOrEmpty(bititolaritaINAIL.Evento))
                        pensioniInail.Evento = bititolaritaINAIL.Evento == "SI" ? true : false;
                    if (!String.IsNullOrEmpty(bititolaritaINAIL.Importo))
                        pensioniInail.ImportoMensileInail = Convert.ToDecimal(bititolaritaINAIL.Importo);

                    LpensioniInail.Add(pensioniInail);
                }
            }
            return LpensioniInail;
        }

        protected void SalvaBititolaritaInail_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiBititolaritaInail = GetValoriBititolaritaInail();
            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();

            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            presenterLiquidazionePensione.SalvaBititolaritaInail(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaBititolaritaInail_Click(Object sender, EventArgs e)
        {
            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            presenterLiquidazionePensione.EliminaBititolaritaInail(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati BititolaritaInail";
            }
            else
            {
                modalitaEditRenditaINAIL.Value = "false";
                ValorizzaEtichetteBititolaritaINAIL(null);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        #region Rendita INAIL

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
                            //btnSalvaImportiEsteri.Enabled = false;
                            gvRenditaINAIL.EditIndex = 0;
                            modalitaEditRenditaINAIL.Value = "true";

                            gvRenditaINAIL.DataSource = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];
                            gvRenditaINAIL.DataBind();
                        }
                        else if (IsEmptyEditableRow(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                DropDownList ddlEvento = new DropDownList();
                                ddlEvento = (DropDownList)e.Row.FindControl("ddlEvento");
                                ddlEvento.SelectedValue = ((BititolaritaINAIL)(e.Row.DataItem)).Evento;

                                EnableEditableMode(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[3].FindControl("btnDeleteRenditaINAIL")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((BititolaritaINAIL)(e.Row.DataItem)).Decorrenza);
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((BititolaritaINAIL)(e.Row.DataItem)).Importo;
                                ((Label)e.Row.FindControl("lblEvento_item")).Text = ((BititolaritaINAIL)(e.Row.DataItem)).Evento;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3]);
                            }

                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                DropDownList ddlEvento = new DropDownList();
                                ddlEvento = (DropDownList)e.Row.FindControl("ddlEvento");
                                ddlEvento.SelectedValue = ((BititolaritaINAIL)(e.Row.DataItem)).Evento;

                                EnableEditableMode(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((BititolaritaINAIL)(e.Row.DataItem)).Decorrenza);
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((BititolaritaINAIL)(e.Row.DataItem)).Importo;
                                ((Label)e.Row.FindControl("lblEvento_item")).Text = ((BititolaritaINAIL)(e.Row.DataItem)).Evento;
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
                            ddlEvento.SelectedValue = ((BititolaritaINAIL)(e.Row.DataItem)).Evento;

                            EnableEditableMode(e.Row.Cells[0]);
                        }

                        else if (e.Row.DataItemIndex == ((List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));

                            //controllo necessario per inserire al max 24 diverse occorrenze
                            if (((List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"]).Count == 24)
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
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((BititolaritaINAIL)(e.Row.DataItem)).Decorrenza);
                            ((Label)e.Row.FindControl("lblImporto")).Text = ((BititolaritaINAIL)(e.Row.DataItem)).Importo;
                            ((Label)e.Row.FindControl("lblEvento_item")).Text = ((BititolaritaINAIL)(e.Row.DataItem)).Evento;
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
                throw new INPS.DNA.DnaApplicationException("UCBititolaritaINAIL, Errore nel metodo gvRenditaINAIL_RowDataBound " + ex);
            }
        }

        protected void gvRenditaINAIL_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<BititolaritaINAIL> listaBititolaritaINAILApp = new List<BititolaritaINAIL>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string DecorrenzaApp = string.Empty;
                    string ImportoApp = string.Empty;
                    string EventoApp = string.Empty;

                    if (!IsEmptyReadableRow(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            DecorrenzaApp = ((Label)rApp.Cells[3].FindControl("lblDecorrenza")).Text;
                            ImportoApp = ((Label)rApp.Cells[4].FindControl("lblImporto")).Text;
                            EventoApp = ((Label)rApp.Cells[4].FindControl("lblEvento_item")).Text;

                            listaBititolaritaINAILApp = AddRecord(listaBititolaritaINAILApp, DecorrenzaApp, ImportoApp, EventoApp);
                        }
                    }
                }
                if (listaBititolaritaINAILApp.Count == 0)
                    this.modalitaEditRenditaINAIL.Value = "false";

                listaBititolaritaINAILApp.Add(new BititolaritaINAIL(string.Empty, string.Empty, string.Empty));

                removeItemBlank(ref listaBititolaritaINAILApp);
                ViewState["elencoBititolaritaINAIL"] = listaBititolaritaINAILApp;

                gvRenditaINAIL_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                // serve nel momento in cui il record vuoto non è presente
                List<BititolaritaINAIL> listaBititolaritaINAILApp = ((List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"]);
                removeItemBlank(ref listaBititolaritaINAILApp);
                listaBititolaritaINAILApp.Add(new BititolaritaINAIL(string.Empty, string.Empty, string.Empty));
                ViewState["elencoBititolaritaINAIL"] = listaBititolaritaINAILApp;
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<BititolaritaINAIL> listaBititolaritaINAILApp = new List<BititolaritaINAIL>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string DecorrenzaApp = string.Empty;
                        string ImportoApp = string.Empty;
                        string EventoApp = string.Empty;

                        if (!IsEmptyEditableRow(rApp))
                        {
                            DecorrenzaApp = ((TextBox)rApp.Cells[1].Controls[1]).Text;
                            ImportoApp = ((TextBox)rApp.Cells[2].Controls[1]).Text;
                            EventoApp = ((DropDownList)rApp.Cells[3].Controls[1]).SelectedValue;
                            listaBititolaritaINAILApp = AddRecord(listaBititolaritaINAILApp, DecorrenzaApp, ImportoApp, EventoApp);
                        }
                        else if (!IsEmptyReadableRow(rApp))
                        {
                            DecorrenzaApp = ((Label)rApp.Cells[1].FindControl("lblDecorrenza")).Text;
                            ImportoApp = ((Label)rApp.Cells[2].FindControl("lblImporto")).Text;
                            EventoApp = ((Label)rApp.Cells[3].FindControl("lblEvento_item")).Text;
                            listaBititolaritaINAILApp = AddRecord(listaBititolaritaINAILApp, DecorrenzaApp, ImportoApp, EventoApp);
                        }
                    }
                    listaBititolaritaINAILApp.Add(new BititolaritaINAIL(string.Empty, string.Empty, string.Empty));
                    gvRenditaINAIL.EditIndex = -1;
                    ViewState["elencoBititolaritaINAIL"] = listaBititolaritaINAILApp;

                    gvRenditaINAIL.DataSource = listaBititolaritaINAILApp;
                    gvRenditaINAIL.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<BititolaritaINAIL> listaBititolaritaINAILApp = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];
                if (!IsListaEmpty())
                {
                    gvRenditaINAIL.EditIndex = -1;
                    gvRenditaINAIL.DataSource = listaBititolaritaINAILApp;
                    gvRenditaINAIL.DataBind();
                }
            }
        }

        protected void gvRenditaINAIL_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRenditaINAIL.EditIndex = e.NewEditIndex;
                List<BititolaritaINAIL> listaBititolaritaINAILApp = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];
                gvRenditaINAIL.DataSource = listaBititolaritaINAILApp;
                gvRenditaINAIL.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCBititolaritaINAIL, Errore nel metodo gvRenditaINAIL_RowEditing " + ex);
            }
        }

        protected void gvRenditaINAIL_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvRenditaINAIL.EditIndex = -1;

                List<BititolaritaINAIL> listaBititolaritaINAILApp = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];
                gvRenditaINAIL.DataSource = listaBititolaritaINAILApp;
                gvRenditaINAIL.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCBititolaritaINAIL, Errore nel metodo gvImportiEsteri_RowCancelingEdit " + ex);
            }
        }

        protected void gvRenditaINAIL_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        #endregion Rendita INAIL

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
            List<BititolaritaINAIL> elencoBititolaritaINAIL = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];

            BititolaritaINAIL Empty = elencoBititolaritaINAIL.Find(delegate(BititolaritaINAIL code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty && code.Evento == string.Empty);
            }
            );

            if (Empty == null)
            {
                elencoBititolaritaINAIL.Add(new BititolaritaINAIL(string.Empty, string.Empty, string.Empty));
            }

            gvRenditaINAIL.DataSource = elencoBititolaritaINAIL;
            gvRenditaINAIL.DataBind();
        }

        private void removeItemBlank(ref List<BititolaritaINAIL> lista)
        {

            int index = lista.FindIndex(delegate(BititolaritaINAIL code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty && code.Evento == string.Empty);
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private bool IsListaEmpty()
        {
            List<BititolaritaINAIL> listaBititolaritaINAILApp = (List<BititolaritaINAIL>)ViewState["elencoBititolaritaINAIL"];
            if (listaBititolaritaINAILApp.Count == 1 && listaBititolaritaINAILApp[0].Decorrenza == string.Empty && listaBititolaritaINAILApp[0].Importo == string.Empty
                && listaBititolaritaINAILApp[0].Evento == string.Empty)
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
            save.ValidationGroup = "UCTabINAIL";
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteRenditaINAIL")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private List<BititolaritaINAIL> AddRecord(List<BititolaritaINAIL> listaRecord, String decorrenza, String importo, String evento)
        {
            listaRecord.Add(new BititolaritaINAIL(decorrenza, importo, evento));
            return listaRecord;
        }

        #endregion Private Methods


        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensione
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

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
        public class BititolaritaINAIL
        {
            public BititolaritaINAIL()
            { }

            public BititolaritaINAIL(string decorrenza, string importo, string evento)
            {
                this._Decorrenza = decorrenza;
                this._Importo = importo;
                this._Evento = evento;
            }

            private string _Decorrenza;
            private string _Importo;
            private string _Evento;

            public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public string Importo { get { return _Importo; } set { _Importo = value; } }
            public string Evento { get { return _Evento; } set { _Evento = value; } }
        }
    }
}
