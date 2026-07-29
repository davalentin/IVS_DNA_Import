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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi
{
    public partial class UCDatiPostDecOriginariaCi : CustomBaseUserControl, IDatiContributiviCi
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.areaDatiContributiviCi != null)
                {
                    ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;

                    if (this.areaDatiContributiviCi.LDatiPostDecOriginaria != null)
                    {
                        ValorizzaEtichette(this.areaDatiContributiviCi.LDatiPostDecOriginaria.ToList());
                        gvDatiPostDecOriginaria_Load();
                    }
                    else
                    {
                        List<DatiPostDecOriginaria> elencoDatiPostDecOriginaria = new List<DatiPostDecOriginaria>();
                        ViewState["elencoDatiPostDecOriginaria"] = elencoDatiPostDecOriginaria;
                        gvDatiPostDecOriginaria_Load();
                    }
                }
            }
        }

        protected void btnSalvaDatiPostDecOriginaria_Click(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();

            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();

            GetDatiPostDecOriginariaTab();

            presenterDatiContributiviCi.SalvaTabDatiPostDecOriginariaCi(this);

            if (!this.HasError)
            {
                ValorizzaEtichette(areaDatiContributiviCi.LDatiPostDecOriginaria.ToList());
            }
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO;
            }

            gvDatiPostDecOriginaria_Load();

            RaiseShowAvvisoDatiPostDecOriginaria(this, null);
        }

        protected void btnEliminaDatiPostDecOriginaria_Click(object sendere, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            GetDatiPostDecOriginariaTab();

            PresenterDatiContributiviCI presenterDatiContributivi = new PresenterDatiContributiviCI();
            presenterDatiContributivi.EliminaTabDatiPostDecOriginariaCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Importi Esteri";
            }
            else
            {
                modalitaEditDatiPostDecOriginaria.Value = "false";
                ((List<DatiPostDecOriginaria>)(ViewState["elencoDatiPostDecOriginaria"])).Clear();
                ViewState["DatiContributiviCi"] = null;
                ValorizzaEtichette(null);
            }

            RaiseShowAvvisoEliminaDatiPostDecOriginaria(this, null);
        }

        protected void gvDatiPostDecOriginaria_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty() && !Convert.ToBoolean(modalitaEditDatiPostDecOriginaria.Value))
                        {
                            btnSalvaDatiPostDecOriginaria.Enabled = false;
                            gvDatiPostDecOriginaria.EditIndex = 0;
                            modalitaEditDatiPostDecOriginaria.Value = "true";

                            gvDatiPostDecOriginaria.DataSource = (List<DatiPostDecOriginaria>)ViewState["elencoDatiPostDecOriginaria"];
                            gvDatiPostDecOriginaria.DataBind();
                        }
                        else if (IsEmptyEditableRow(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableMode(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[7].FindControl("btnDelete")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblCTR")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).CTR;
                                ((Label)e.Row.FindControl("lblIVS")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).IVS;
                                ((Label)e.Row.FindControl("lblSettimaneRetributive")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).SettimaneRetributive;
                                ((Label)e.Row.FindControl("lblSettimaneVV")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).SettimaneVV;
                                ((Label)e.Row.FindControl("lblRMS")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).RMS;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
                            }

                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableMode(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblCTR")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).CTR;
                                ((Label)e.Row.FindControl("lblIVS")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).IVS;
                                ((Label)e.Row.FindControl("lblSettimaneRetributive")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).SettimaneRetributive;
                                ((Label)e.Row.FindControl("lblSettimaneVV")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).SettimaneVV;
                                ((Label)e.Row.FindControl("lblRMS")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).RMS;
                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            EnableEditableMode(e.Row.Cells[0]);
                        }

                        else if (e.Row.DataItemIndex == ((List<DatiPostDecOriginaria>)ViewState["elencoDatiPostDecOriginaria"]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).Decorrenza;
                            ((Label)e.Row.FindControl("lblCTR")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).CTR;
                            ((Label)e.Row.FindControl("lblIVS")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).IVS;
                            ((Label)e.Row.FindControl("lblSettimaneRetributive")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).SettimaneRetributive;
                            ((Label)e.Row.FindControl("lblSettimaneVV")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).SettimaneVV;
                            ((Label)e.Row.FindControl("lblRMS")).Text = ((DatiPostDecOriginaria)(e.Row.DataItem)).RMS;
                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7]);
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
                throw new INPS.DNA.DnaApplicationException("UCDatiPostDecOriginariaCi, Errore nel metodo gvDatiPostDecOriginaria_RowDataBound " + ex);
            }
        }

        protected void gvDatiPostDecOriginaria_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiPostDecOriginaria> listaDatiPostDecOriginariaApp = new List<DatiPostDecOriginaria>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string DecorrenzaApp = string.Empty;
                    string CTRApp = string.Empty;
                    string IVSApp = string.Empty;
                    string SettimaneRetributiveApp = string.Empty;
                    string SettimaneVVApp = string.Empty;
                    string RMSApp = string.Empty;

                    if (!IsEmptyReadableRow(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            DecorrenzaApp = ((Label)rApp.Cells[3].FindControl("lblDecorrenza")).Text;
                            CTRApp = ((Label)rApp.Cells[4].FindControl("lblCTR")).Text;
                            IVSApp = ((Label)rApp.Cells[4].FindControl("lblIVS")).Text;
                            SettimaneRetributiveApp = ((Label)rApp.Cells[4].FindControl("lblSettimaneRetributive")).Text;
                            SettimaneVVApp = ((Label)rApp.Cells[4].FindControl("lblSettimaneVV")).Text;
                            RMSApp = ((Label)rApp.Cells[4].FindControl("lblRMS")).Text;

                            listaDatiPostDecOriginariaApp = AddRecord(listaDatiPostDecOriginariaApp, DecorrenzaApp, CTRApp, IVSApp, SettimaneRetributiveApp, SettimaneVVApp, RMSApp);
                        }
                    }
                }
                if (listaDatiPostDecOriginariaApp.Count == 0)
                    this.modalitaEditDatiPostDecOriginaria.Value = "false";

                listaDatiPostDecOriginariaApp.Add(new DatiPostDecOriginaria(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                btnSalvaDatiPostDecOriginaria.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);

                RemoveItemBlank(ref listaDatiPostDecOriginariaApp);
                ViewState["elencoDatiPostDecOriginaria"] = listaDatiPostDecOriginariaApp;

                gvDatiPostDecOriginaria_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                btnSalvaDatiPostDecOriginaria.Enabled = false;
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiPostDecOriginaria> listaDatiPostDecOriginariaApp = new List<DatiPostDecOriginaria>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string DecorrenzaApp = string.Empty;
                        string CTRApp = string.Empty;
                        string IVSApp = string.Empty;
                        string SettimaneRetributiveApp = string.Empty;
                        string SettimaneVVApp = string.Empty;
                        string RMSApp = string.Empty;

                        if (!IsEmptyEditableRow(rApp))
                        {
                            DecorrenzaApp = ((TextBox)rApp.Cells[1].Controls[1]).Text;
                            CTRApp = ((TextBox)rApp.Cells[2].Controls[1]).Text;
                            IVSApp = ((TextBox)rApp.Cells[3].Controls[1]).Text;
                            SettimaneRetributiveApp = ((TextBox)rApp.Cells[4].Controls[1]).Text;
                            SettimaneVVApp = ((TextBox)rApp.Cells[5].Controls[1]).Text;
                            RMSApp = ((TextBox)rApp.Cells[6].Controls[1]).Text;

                            listaDatiPostDecOriginariaApp = AddRecord(listaDatiPostDecOriginariaApp, DecorrenzaApp, CTRApp, IVSApp, SettimaneRetributiveApp, SettimaneVVApp, RMSApp);
                        }
                        else if (!IsEmptyReadableRow(rApp))
                        {
                            DecorrenzaApp = ((Label)rApp.Cells[3].FindControl("lblDecorrenza")).Text;
                            CTRApp = ((Label)rApp.Cells[4].FindControl("lblCTR")).Text;
                            IVSApp = ((Label)rApp.Cells[4].FindControl("lblIVS")).Text;
                            SettimaneRetributiveApp = ((Label)rApp.Cells[4].FindControl("lblSettimaneRetributive")).Text;
                            SettimaneVVApp = ((Label)rApp.Cells[4].FindControl("lblSettimaneVV")).Text;
                            RMSApp = ((Label)rApp.Cells[4].FindControl("lblRMS")).Text;

                            listaDatiPostDecOriginariaApp = AddRecord(listaDatiPostDecOriginariaApp, DecorrenzaApp, CTRApp, IVSApp, SettimaneRetributiveApp, SettimaneVVApp, RMSApp);
                        }
                    }
                    listaDatiPostDecOriginariaApp.Add(new DatiPostDecOriginaria(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    RaiseAbilitaTastoSalva(this, null);
                    btnSalvaDatiPostDecOriginaria.Enabled = true;
                    gvDatiPostDecOriginaria.EditIndex = -1;
                    ViewState["elencoDatiPostDecOriginaria"] = listaDatiPostDecOriginariaApp;

                    gvDatiPostDecOriginaria.DataSource = listaDatiPostDecOriginariaApp;
                    gvDatiPostDecOriginaria.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiPostDecOriginaria> listaDatiPostDecOriginariaApp = (List<DatiPostDecOriginaria>)ViewState["elencoDatiPostDecOriginaria"];
                if (!IsListaEmpty())
                {
                    RaiseAbilitaTastoSalva(this, null);
                    btnSalvaDatiPostDecOriginaria.Enabled = true;
                    gvDatiPostDecOriginaria.EditIndex = -1;
                    gvDatiPostDecOriginaria.DataSource = listaDatiPostDecOriginariaApp;
                    gvDatiPostDecOriginaria.DataBind();
                }
            }
        }

        protected void gvDatiPostDecOriginaria_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiPostDecOriginaria.EditIndex = e.NewEditIndex;
                List<DatiPostDecOriginaria> listaDatiPostDecOriginariaApp = (List<DatiPostDecOriginaria>)ViewState["elencoDatiPostDecOriginaria"];
                gvDatiPostDecOriginaria.DataSource = listaDatiPostDecOriginariaApp;
                gvDatiPostDecOriginaria.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiPostDecOriginariaCi, Errore nel metodo gvDatiPostDecOriginaria_RowEditing " + ex);
            }
        }

        #region private methods

        private bool IsListaEmpty()
        {
            List<DatiPostDecOriginaria> listaDatiPostDecOriginariaApp = (List<DatiPostDecOriginaria>)ViewState["elencoDatiPostDecOriginaria"];
            if (listaDatiPostDecOriginariaApp.Count == 1 && listaDatiPostDecOriginariaApp[0].Decorrenza == string.Empty && listaDatiPostDecOriginariaApp[0].CTR == string.Empty &&
                 listaDatiPostDecOriginariaApp[0].IVS == string.Empty && listaDatiPostDecOriginariaApp[0].SettimaneRetributive == string.Empty &&
                 listaDatiPostDecOriginariaApp[0].SettimaneVV == string.Empty && listaDatiPostDecOriginariaApp[0].RMS == string.Empty)
                return true;
            else
                return false;
        }

        private void ValorizzaEtichette(List<GestioneContribDatiPostDecOriginaria> datiPostDecOriginaria)
        {
            if (datiPostDecOriginaria == null)
            {
                gvDatiPostDecOriginaria_Load();
            }
            else
            {
                List<DatiPostDecOriginaria> elencoDatiPostDecOriginaria = new List<DatiPostDecOriginaria>();

                for (int i = 0; i < datiPostDecOriginaria.Count; i++)
                {
                    elencoDatiPostDecOriginaria.Add(new DatiPostDecOriginaria(String.Format("{0:MM/yyyy}", datiPostDecOriginaria[i].Decorrenza.Value),
                        datiPostDecOriginaria[i].CTR.ToString(), datiPostDecOriginaria[i].IVS.ToString(), datiPostDecOriginaria[i].SettimaneRetributive.ToString(),
                        datiPostDecOriginaria[i].SettimaneVV.ToString(), datiPostDecOriginaria[i].RMS.ToString()));
                }

                ViewState["elencoDatiPostDecOriginaria"] = elencoDatiPostDecOriginaria;
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
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabDatiPostDecOriginaria";
            save.CommandName = "Salva";
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDelete")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
            delete.ToolTip = "Elimina";
            delete.CommandName = "Elimina";
        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl("txtDecorrenza") != null && ((TextBox)row.FindControl("txtDecorrenza")).Text != string.Empty &&
                row.FindControl("txtCTR") != null && ((TextBox)row.FindControl("txtCTR")).Text != string.Empty &&
                row.FindControl("txtIVS") != null && ((TextBox)row.FindControl("txtIVS")).Text != string.Empty &&
                row.FindControl("txtSettimaneRetributive") != null && ((TextBox)row.FindControl("txtSettimaneRetributive")).Text != string.Empty &&
                row.FindControl("txtSettimaneVV") != null && ((TextBox)row.FindControl("txtSettimaneVV")).Text != string.Empty &&
                row.FindControl("txtRMS") != null && ((TextBox)row.FindControl("txtRMS")).Text != string.Empty)

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRow(GridViewRow row)
        {
            if (row.FindControl("lblDecorrenza") != null && ((Label)row.FindControl("lblDecorrenza")).Text != string.Empty &&
                row.FindControl("lblCTR") != null && ((Label)row.FindControl("lblCTR")).Text != string.Empty &&
                row.FindControl("lblIVS") != null && ((Label)row.FindControl("lblIVS")).Text != string.Empty &&
                row.FindControl("lblSettimaneRetributive") != null && ((Label)row.FindControl("lblSettimaneRetributive")).Text != string.Empty &&
                row.FindControl("lblSettimaneVV") != null && ((Label)row.FindControl("lblSettimaneVV")).Text != string.Empty &&
                row.FindControl("lblRMS") != null && ((Label)row.FindControl("lblRMS")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void AddItemBlank(ref List<DatiPostDecOriginaria> elencoDatiPostDecOriginaria)
        {
            int index = elencoDatiPostDecOriginaria.FindIndex(delegate(DatiPostDecOriginaria code)
            {
                return (string.IsNullOrEmpty(code.Decorrenza) && string.IsNullOrEmpty(code.CTR) && string.IsNullOrEmpty(code.IVS) && string.IsNullOrEmpty(code.SettimaneRetributive) &&
                  string.IsNullOrEmpty(code.SettimaneVV) && string.IsNullOrEmpty(code.RMS));
            });

            if (index < 0)
                elencoDatiPostDecOriginaria.Add(new DatiPostDecOriginaria());
        }

        private void RemoveItemBlank(ref List<DatiPostDecOriginaria> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate(DatiPostDecOriginaria code)
                {
                    return (string.IsNullOrEmpty(code.Decorrenza) && string.IsNullOrEmpty(code.CTR) && string.IsNullOrEmpty(code.IVS) && string.IsNullOrEmpty(code.SettimaneRetributive) &&
                    string.IsNullOrEmpty(code.SettimaneVV) && string.IsNullOrEmpty(code.RMS));
                });

                if (index >= 0)
                    lista.RemoveAt(index);
            }
        }

        private List<DatiPostDecOriginaria> AddRecord(List<DatiPostDecOriginaria> listaRecord, string decorrenza, string ctr, string ivs, string settimaneRetributive, string settimaneVV, string rms)
        {
            listaRecord.Add(new DatiPostDecOriginaria(decorrenza, ctr, ivs, settimaneRetributive, settimaneVV, rms));
            return listaRecord;
        }

        private void gvDatiPostDecOriginaria_Load()
        {
            try
            {
                List<DatiPostDecOriginaria> elencoDatiPostDecOriginaria = (List<DatiPostDecOriginaria>)ViewState["elencoDatiPostDecOriginaria"];

                DatiPostDecOriginaria Empty = elencoDatiPostDecOriginaria.Find(delegate(DatiPostDecOriginaria code)
                {
                    return (code.Decorrenza == string.Empty && code.CTR == string.Empty && code.IVS == string.Empty && code.SettimaneRetributive == string.Empty &&
                        code.SettimaneVV == string.Empty && code.RMS == string.Empty);
                }
                );

                if (Empty == null)
                {
                    elencoDatiPostDecOriginaria.Add(new DatiPostDecOriginaria(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                }

                gvDatiPostDecOriginaria.DataSource = elencoDatiPostDecOriginaria;
                gvDatiPostDecOriginaria.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiPostDecOriginaria, Errore nel metodo GvDatiPostDecOriginaria_Load" + ex);
            }

        }

        private GestioneContribDatiPostDecOriginaria[] GetDatiPostDecOriginariaTab()
        {
            this.areaDatiContributiviCi = new AreaDatiContributivi();
            List<GestioneContribDatiPostDecOriginaria> LDatiPostDecOriginariaSvcCI = null;

            List<DatiPostDecOriginaria> elencoDatiPostDecOriginaria = ((List<DatiPostDecOriginaria>)(ViewState["elencoDatiPostDecOriginaria"]));
            if (elencoDatiPostDecOriginaria != null)
                RemoveItemBlank(ref elencoDatiPostDecOriginaria);

            if (elencoDatiPostDecOriginaria != null && elencoDatiPostDecOriginaria.Count > 0)
            {
                this.areaDatiContributiviCi.LDatiPostDecOriginaria = new GestioneContribDatiPostDecOriginaria[elencoDatiPostDecOriginaria.Count];

                LDatiPostDecOriginariaSvcCI = new List<GestioneContribDatiPostDecOriginaria>();

                foreach (DatiPostDecOriginaria datiPostDecOriginaria in elencoDatiPostDecOriginaria)
                {
                    GestioneContribDatiPostDecOriginaria datiPostDecOriginariaSvcCI = new GestioneContribDatiPostDecOriginaria();

                    if (datiPostDecOriginaria.Decorrenza == string.Empty)
                        datiPostDecOriginariaSvcCI.Decorrenza = null;
                    else
                        datiPostDecOriginariaSvcCI.Decorrenza = Utility.GetDateFromString(datiPostDecOriginaria.Decorrenza);

                    if (datiPostDecOriginaria.CTR == string.Empty)
                        datiPostDecOriginariaSvcCI.CTR = null;
                    else
                        datiPostDecOriginariaSvcCI.CTR = int.Parse(datiPostDecOriginaria.CTR);

                    if (datiPostDecOriginaria.IVS == string.Empty)
                        datiPostDecOriginariaSvcCI.IVS = null;
                    else
                        datiPostDecOriginariaSvcCI.IVS = decimal.Parse(datiPostDecOriginaria.IVS);

                    if (datiPostDecOriginaria.SettimaneRetributive == string.Empty)
                        datiPostDecOriginariaSvcCI.SettimaneRetributive = null;
                    else
                        datiPostDecOriginariaSvcCI.SettimaneRetributive = int.Parse(datiPostDecOriginaria.SettimaneRetributive);

                    if (datiPostDecOriginaria.SettimaneVV == string.Empty)
                        datiPostDecOriginariaSvcCI.SettimaneVV = null;
                    else
                        datiPostDecOriginariaSvcCI.SettimaneVV = int.Parse(datiPostDecOriginaria.SettimaneVV);

                    if (datiPostDecOriginaria.RMS == string.Empty)
                        datiPostDecOriginariaSvcCI.RMS = null;
                    else
                        datiPostDecOriginariaSvcCI.RMS = decimal.Parse(datiPostDecOriginaria.RMS);

                    LDatiPostDecOriginariaSvcCI.Add(datiPostDecOriginariaSvcCI);
                }

                this.areaDatiContributiviCi.LDatiPostDecOriginaria = LDatiPostDecOriginariaSvcCI.ToArray();
            }
            if (LDatiPostDecOriginariaSvcCI != null)
                return LDatiPostDecOriginariaSvcCI.ToArray();
            else
                return null;
        }

        #endregion private methods

        #region EventHandler

        public event EventHandler AbilitaTastoSalva;
        public event EventHandler ShowAvvisoDatiPostDecOriginaria;
        public event EventHandler ShowAvvisoEliminaDatiPostDecOriginaria;

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }
        protected void RaiseShowAvvisoDatiPostDecOriginaria(object sender, EventArgs e)
        {
            ShowAvvisoDatiPostDecOriginaria(sender, e);
        }
        protected void RaiseShowAvvisoEliminaDatiPostDecOriginaria(object sender, EventArgs e)
        {
            ShowAvvisoEliminaDatiPostDecOriginaria(sender, e);
        }
        #endregion EventHandler

        #region IDatiContributiviCi Members
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaDatiContributivi areaDatiContributiviCi { get; set; }
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviCi Members

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members
    }

    [Serializable]
    public class DatiPostDecOriginaria
    {
        public DatiPostDecOriginaria()
        {
        }

        public DatiPostDecOriginaria(string decorrenza, string ctr, string ivs, string settimaneRetributive, string settimaneVV, string rms)
        {
            this._Decorrenza = decorrenza;
            this._CTR = ctr;
            this._IVS = ivs;
            this._SettimaneRetributive = settimaneRetributive;
            this._SettimaneVV = settimaneVV;
            this._RMS = rms;
        }

        private string _Decorrenza;
        private string _CTR;
        private string _IVS;
        private string _SettimaneRetributive;
        private string _SettimaneVV;
        private string _RMS;

        public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public string CTR { get { return _CTR; } set { _CTR = value; } }
        public string IVS { get { return _IVS; } set { _IVS = value; } }
        public string SettimaneRetributive { get { return _SettimaneRetributive; } set { _SettimaneRetributive = value; } }
        public string SettimaneVV { get { return _SettimaneVV; } set { _SettimaneVV = value; } }
        public string RMS { get { return _RMS; } set { _RMS = value; } }
    }
}