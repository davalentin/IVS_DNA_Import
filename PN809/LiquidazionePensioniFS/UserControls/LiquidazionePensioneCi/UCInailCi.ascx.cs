using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi
{
    public partial class UCInailCi : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneCi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneCi
        public AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion ILiquidazionePensioneCi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        }

        internal void ValorizzaEtichette(ILiquidazionePensioneCi liquidazionePensioneCi)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (liquidazionePensioneCi != null && liquidazionePensioneCi.areaLiquidazionePensioneCi != null && liquidazionePensioneCi.areaLiquidazionePensioneCi.DatiInail != null &&
                liquidazionePensioneCi.areaLiquidazionePensioneCi.DatiInail.Count() > 0)
            {
                List<Inail> datiInail = new List<Inail>();
                foreach (Presenter.SvrLiquidazioneCi.DatiInail datiInailTemp in liquidazionePensioneCi.areaLiquidazionePensioneCi.DatiInail)
                {
                    Inail Inail = new Inail();
                    Inail.Decorrenza = datiInailTemp.DecorrenzaRenditaInail.HasValue ? String.Format("{0:MM/yyyy}", datiInailTemp.DecorrenzaRenditaInail.Value) : string.Empty;
                    Inail.Evento = datiInailTemp.Evento.HasValue && datiInailTemp.Evento.Value ? "SI" : "NO";
                    Inail.Importo = datiInailTemp.ImportoMensileInail.HasValue ? datiInailTemp.ImportoMensileInail.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    datiInail.Add(Inail);
                }

                ViewState["DatiInail"] = datiInail;
                gvRenditaINAIL_Load();
            }
            else
            {
                List<Inail> datiInail = new List<Inail>();
                ViewState["DatiInail"] = datiInail;
                gvRenditaINAIL_Load();
            }
        }

        private bool IsListaEmpty()
        {
            List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];
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

        private void RimuoviDallaGriglia(ref List<Inail> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        private void removeItemBlank(ref List<Inail> lista)
        {
            int index = lista.FindIndex(delegate(Inail code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty && code.Evento == string.Empty);
            });

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        public class Keys
        {
            public const string RenditaInail_HdnId = "hdnGUID";
            public const string RenditaInail_TxtDecorrenza = "txtDecorrenza";
            public const string RenditaInail_TxtImporto = "txtImporto";
            public const string RenditaInail_DdlEvento = "ddlEvento";
        }

        private void gvRenditaINAIL_Load()
        {
            List<Inail> elencoInail = (List<Inail>)ViewState["DatiInail"];

            Inail Empty = elencoInail.Find(delegate(Inail code)
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

                            gvRenditaINAIL.DataSource = (List<Inail>)ViewState["DatiInail"];
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

                                if (btnSalvaBititolaritaInail.Enabled == true)
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

                                if (btnSalvaBititolaritaInail.Enabled == true)
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

                            if (btnSalvaBititolaritaInail.Enabled == true)
                                btnSalvaBititolaritaInail.Enabled = false;
                        }

                        else if (e.Row.DataItemIndex == ((List<Inail>)ViewState["DatiInail"]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));

                            //controllo necessario per inserire al max 45 diverse occorrenze
                            if (((List<Inail>)ViewState["DatiInail"]).Count == 46)
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
                List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.RenditaInail_HdnId);
                int index = listaInailApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref listaInailApp, index);

                this.modalitaEditRenditaINAIL.Value = "false";
                gvRenditaINAIL.EditIndex = -1;
                ViewState["DatiInail"] = listaInailApp;

                gvRenditaINAIL_Load();
                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                // serve nel momento in cui il record vuoto non è presente
                List<Inail> listaInailApp = ((List<Inail>)ViewState["DatiInail"]);
                removeItemBlank(ref listaInailApp);
                listaInailApp.Add(new Inail(string.Empty, string.Empty, string.Empty));
                ViewState["DatiInail"] = listaInailApp;

                if (btnSalvaBititolaritaInail.Enabled == true)
                    btnSalvaBititolaritaInail.Enabled = false;
            }
            else if (e.CommandName == "Salva")
            {

                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.RenditaInail_HdnId);
                    int index = listaInailApp.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    listaInailApp[index].Decorrenza = ((TextBox)r.FindControl(Keys.RenditaInail_TxtDecorrenza)).Text;
                    listaInailApp[index].Importo = ((TextBox)r.FindControl(Keys.RenditaInail_TxtImporto)).Text;
                    listaInailApp[index].Evento = ((DropDownList)r.FindControl(Keys.RenditaInail_DdlEvento)).Text;

                    // Sto inserendo un nuovo record
                    if (index == listaInailApp.Count - 1)
                        listaInailApp.Add(new Inail(string.Empty, string.Empty, string.Empty));

                    gvRenditaINAIL.EditIndex = -1;
                    ViewState["DatiInail"] = listaInailApp;

                    gvRenditaINAIL.DataSource = listaInailApp;
                    gvRenditaINAIL.DataBind();

                    if (btnSalvaBititolaritaInail.Enabled == false)
                        btnSalvaBititolaritaInail.Enabled = true;
                }

            }
            else if (e.CommandName == "Annulla")
            {
                List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];          

                if (!IsListaEmpty())
                {
                    gvRenditaINAIL.EditIndex = -1;
                    gvRenditaINAIL.DataSource = listaInailApp;
                    gvRenditaINAIL.DataBind();

                    if (btnSalvaBititolaritaInail.Enabled == false)
                        btnSalvaBititolaritaInail.Enabled = true;
                }                
            }
        }

        protected void gvRenditaINAIL_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvRenditaINAIL.EditIndex = -1;

                List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];
                gvRenditaINAIL.DataSource = listaInailApp;
                gvRenditaINAIL.DataBind();

                if (btnSalvaBititolaritaInail.Enabled == false)
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

        protected void gvRenditaINAIL_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRenditaINAIL.EditIndex = e.NewEditIndex;
                List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];
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

        protected void gvRenditaINAIL_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRenditaINAIL.PageIndex = e.NewPageIndex;

                List<Inail> listaInailApp = (List<Inail>)ViewState["DatiInail"];
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

        internal DatiInail[] GetDatiInail()
        {
            AreaLiquidazionePensione areaLiquidazionePensione = new AreaLiquidazionePensione();
            List<Presenter.SvrLiquidazioneCi.DatiInail> LpensioniInail = null;

            List<Inail> elencoInail = (List<Inail>)ViewState["DatiInail"];
            removeItemBlank(ref elencoInail);

            if (elencoInail != null)
            {
                LpensioniInail = new List<DatiInail>();
                foreach (Inail Inail in elencoInail)
                {
                    DatiInail pensioniInail = new DatiInail();

                    if (!String.IsNullOrEmpty(Inail.Decorrenza))
                        pensioniInail.DecorrenzaRenditaInail = Convert.ToDateTime(Inail.Decorrenza);
                    if (!String.IsNullOrEmpty(Inail.Evento))
                        pensioniInail.Evento = Inail.Evento == "SI" ? true : false;
                    if (!String.IsNullOrEmpty(Inail.Importo))
                        pensioniInail.ImportoMensileInail = Convert.ToDecimal(Inail.Importo);

                    LpensioniInail.Add(pensioniInail);
                }
            }

            areaLiquidazionePensione.DatiInail = (LpensioniInail != null) ? LpensioniInail.ToArray() : null;
            return areaLiquidazionePensione.DatiInail;
        }

        protected void SalvaInail_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiInail = GetDatiInail();

            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            presenterLiquidazionePensione.SalvaDatiInailCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaInail_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            presenterLiquidazionePensione.EliminaDatiInaiCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Inail";
            }
            else
            {
                modalitaEditRenditaINAIL.Value = "false";
                Inail Inail = new Inail();
                List<Inail> elencoInail = new List<Inail>();
                elencoInail.Add(Inail);
                ViewState["DatiInail"] = elencoInail;
                gvRenditaINAIL_Load();

                ValorizzaEtichette(null);
            }

            RaiseShowAvvisoElimina(this, null);
        }
    }
}