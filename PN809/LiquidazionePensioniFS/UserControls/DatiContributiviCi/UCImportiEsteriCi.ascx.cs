using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi
{
    public partial class UCImportiEsteriCi : CustomBaseUserControl, IDatiContributiviCi
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.areaDatiContributiviCi != null)
                {
                    ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;

                    if (this.areaDatiContributiviCi.LimportiEsteriValuta != null)
                    {
                        ValorizzaEtichette(this.areaDatiContributiviCi.LimportiEsteriValuta.ToList());
                        gvImportiEsteri_Load();
                    }
                    else
                    {
                        List<ImportiEsteri> elencoImportiEsteri = new List<ImportiEsteri>();
                        ViewState["elencoImportiEsteri"] = elencoImportiEsteri;
                        gvImportiEsteri_Load();
                    }
                }
            }
        }

        protected void btnSalvaImportiEsteri_Click(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();

            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();

            GetDatiImportiEsteriTab();

            presenterDatiContributiviCi.SalvaTabImportiEsteriCi(this);

            if (!this.HasError)
            {
                ValorizzaEtichette(areaDatiContributiviCi.LimportiEsteriValuta.ToList());
            }
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO;
            }

            gvImportiEsteri_Load();

            RaiseShowAvvisoImportiEsteri(this, null);
        }

        protected void btnEliminaImportiEsteri_Click(object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            GetDatiImportiEsteriTab();

            PresenterDatiContributiviCI presenterDatiContributivi = new PresenterDatiContributiviCI();
            presenterDatiContributivi.EliminaTabImportiEsteriCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Importi Esteri";
            }
            else
            {
                modalitaEditImportiEsteri.Value = "false";
                ((List<ImportiEsteri>)(ViewState["elencoImportiEsteri"])).Clear();
                ViewState["DatiContributiviCi"] = null;
                ValorizzaEtichette(null);
            }

            RaiseShowAvvisoEliminaImportiEsteri(this, null);
        }

        internal GestioneContribPensioniCiImportiValuta[] GetDatiImportiEsteri()
        {
            return GetDatiImportiEsteriTab();
        }

        #region Importi Esteri

        protected void gvImportiEsteri_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty() && !Convert.ToBoolean(modalitaEditImportiEsteri.Value))
                        {
                            btnSalvaImportiEsteri.Enabled = false;
                            gvImportiEsteri.EditIndex = 0;
                            modalitaEditImportiEsteri.Value = "true";

                            gvImportiEsteri.DataSource = (List<ImportiEsteri>)ViewState["elencoImportiEsteri"];
                            gvImportiEsteri.DataBind();
                        }
                        else if (IsEmptyEditableRow(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableMode(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[3].FindControl("btnDeleteImportiEsteri")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((ImportiEsteri)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((ImportiEsteri)(e.Row.DataItem)).Importo;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3]);
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
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((ImportiEsteri)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((ImportiEsteri)(e.Row.DataItem)).Importo;
                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            EnableEditableMode(e.Row.Cells[0]);
                        }

                        else if (e.Row.DataItemIndex == ((List<ImportiEsteri>)ViewState["elencoImportiEsteri"]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((ImportiEsteri)(e.Row.DataItem)).Decorrenza;
                            ((Label)e.Row.FindControl("lblImporto")).Text = ((ImportiEsteri)(e.Row.DataItem)).Importo;
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
                throw new INPS.DNA.DnaApplicationException("UCImportiEsteriCi, Errore nel metodo gvImportiEsteri_RowDataBound " + ex);
            }
        }

        protected void gvImportiEsteri_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<ImportiEsteri> listaImportiEsteriApp = new List<ImportiEsteri>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string DecorrenzaApp = string.Empty;
                    string ImportoApp = string.Empty;

                    if (!IsEmptyReadableRow(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            DecorrenzaApp = ((Label)rApp.Cells[3].FindControl("lblDecorrenza")).Text;
                            ImportoApp = ((Label)rApp.Cells[4].FindControl("lblImporto")).Text;

                            listaImportiEsteriApp = AddRecord(listaImportiEsteriApp, DecorrenzaApp, ImportoApp);
                        }
                    }
                }
                if (listaImportiEsteriApp.Count == 0)
                    this.modalitaEditImportiEsteri.Value = "false";

                listaImportiEsteriApp.Add(new ImportiEsteri(string.Empty, string.Empty));
                btnSalvaImportiEsteri.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);

                removeItemBlank(ref listaImportiEsteriApp);
                ViewState["elencoImportiEsteri"] = listaImportiEsteriApp;

                gvImportiEsteri_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                btnSalvaImportiEsteri.Enabled = false;
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<ImportiEsteri> listaImportiEsteriApp = new List<ImportiEsteri>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string DecorrenzaApp = string.Empty;
                        string ImportoApp = string.Empty;

                        if (!IsEmptyEditableRow(rApp))
                        {
                            DecorrenzaApp = ((TextBox)rApp.Cells[1].Controls[1]).Text;
                            ImportoApp = ((TextBox)rApp.Cells[2].Controls[1]).Text;
                            listaImportiEsteriApp = AddRecord(listaImportiEsteriApp, DecorrenzaApp, ImportoApp);
                        }
                        else if (!IsEmptyReadableRow(rApp))
                        {
                            DecorrenzaApp = ((Label)rApp.Cells[1].FindControl("lblDecorrenza")).Text;
                            ImportoApp = ((Label)rApp.Cells[2].FindControl("lblImporto")).Text;

                            listaImportiEsteriApp = AddRecord(listaImportiEsteriApp, DecorrenzaApp, ImportoApp);
                        }
                    }
                    listaImportiEsteriApp.Add(new ImportiEsteri(string.Empty, string.Empty));
                    RaiseAbilitaTastoSalva(this, null);
                    btnSalvaImportiEsteri.Enabled = true;
                    gvImportiEsteri.EditIndex = -1;
                    ViewState["elencoImportiEsteri"] = listaImportiEsteriApp;

                    gvImportiEsteri.DataSource = listaImportiEsteriApp;
                    gvImportiEsteri.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<ImportiEsteri> listaImportiEsteriApp = (List<ImportiEsteri>)ViewState["elencoImportiEsteri"];
                if (!IsListaEmpty())
                {
                    RaiseAbilitaTastoSalva(this, null);
                    btnSalvaImportiEsteri.Enabled = true;
                    gvImportiEsteri.EditIndex = -1;
                    gvImportiEsteri.DataSource = listaImportiEsteriApp;
                    gvImportiEsteri.DataBind();
                }
            }
        }

        protected void gvImportiEsteri_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvImportiEsteri.EditIndex = e.NewEditIndex;
                List<ImportiEsteri> listaImportiEsteriApp = (List<ImportiEsteri>)ViewState["elencoImportiEsteri"];
                gvImportiEsteri.DataSource = listaImportiEsteriApp;
                gvImportiEsteri.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCImportiEsteriCi, Errore nel metodo gvImportiEsteri_RowEditing " + ex);
            }
        }

        protected void gvImportiEsteri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvImportiEsteri.EditIndex = -1;

                List<ImportiEsteri> listaImportiEsteriApp = (List<ImportiEsteri>)ViewState["elencoImportiEsteri"];
                gvImportiEsteri.DataSource = listaImportiEsteriApp;
                gvImportiEsteri.DataBind();

                btnSalvaImportiEsteri.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCImportiEsteriCi, Errore nel metodo gvImportiEsteri_RowCancelingEdit " + ex);
            }
        }

        protected void gvImportiEsteri_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        #endregion Importi Esteri

        #region Methods Private

        private void ValorizzaEtichette(List<GestioneContribPensioniCiImportiValuta> importiEsteri)
        {
            if (importiEsteri == null)
            {
                gvImportiEsteri_Load();
            }
            else
            {
                List<ImportiEsteri> elencoImportiEsteri = new List<ImportiEsteri>();

                for (int i = 0; i < importiEsteri.Count; i++)
                {
                    elencoImportiEsteri.Add(new ImportiEsteri(String.Format("{0:MM/yyyy}", importiEsteri[i].DecorrenzaPrestazioneEE.Value),
                        importiEsteri[i].ImportoPrestazioneEE.ToString()));
                }

                ViewState["elencoImportiEsteri"] = elencoImportiEsteri;
            }
        }

        private GestioneContribPensioniCiImportiValuta[] GetDatiImportiEsteriTab()
        {
            this.areaDatiContributiviCi = new AreaDatiContributivi();
            List<GestioneContribPensioniCiImportiValuta> listElencoImportiEE = null;

            List<ImportiEsteri> elencoImportiEsteri = ((List<ImportiEsteri>)(ViewState["elencoImportiEsteri"]));
            if (elencoImportiEsteri != null)
                removeItemBlank(ref elencoImportiEsteri);

            if (elencoImportiEsteri != null && elencoImportiEsteri.Count > 0)
            {
                this.areaDatiContributiviCi.LimportiEsteriValuta = new GestioneContribPensioniCiImportiValuta[elencoImportiEsteri.Count];

                listElencoImportiEE = new List<GestioneContribPensioniCiImportiValuta>();

                foreach (ImportiEsteri impEsteri in elencoImportiEsteri)
                {
                    GestioneContribPensioniCiImportiValuta importiEE = new GestioneContribPensioniCiImportiValuta();

                    if (impEsteri.Decorrenza == string.Empty)
                        importiEE.DecorrenzaPrestazioneEE = null;
                    else
                        importiEE.DecorrenzaPrestazioneEE = Utility.GetDateFromString(impEsteri.Decorrenza);

                    if (impEsteri.Importo == string.Empty)
                        importiEE.ImportoPrestazioneEE = null;
                    else
                        importiEE.ImportoPrestazioneEE = decimal.Parse(impEsteri.Importo);

                    listElencoImportiEE.Add(importiEE);
                }

                this.areaDatiContributiviCi.LimportiEsteriValuta = listElencoImportiEE.ToArray();
            }
            if (listElencoImportiEE != null)
                return listElencoImportiEE.ToArray();
            else
                return null;
        }

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

        private void gvImportiEsteri_Load()
        {
            List<ImportiEsteri> elencoImportiEsteri = (List<ImportiEsteri>)ViewState["elencoImportiEsteri"];

            ImportiEsteri Empty = elencoImportiEsteri.Find(delegate (ImportiEsteri code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty);
            }
            );

            if (Empty == null)
            {
                elencoImportiEsteri.Add(new ImportiEsteri(string.Empty, string.Empty));
            }

            gvImportiEsteri.DataSource = elencoImportiEsteri;
            gvImportiEsteri.DataBind();
        }

        private void removeItemBlank(ref List<ImportiEsteri> lista)
        {

            int index = lista.FindIndex(delegate (ImportiEsteri code)
            {
                return (code.Decorrenza == string.Empty && code.Importo == string.Empty);
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private bool IsListaEmpty()
        {
            List<ImportiEsteri> listaImportiEsteriApp = (List<ImportiEsteri>)ViewState["elencoImportiEsteri"];
            if (listaImportiEsteriApp.Count == 1 && listaImportiEsteriApp[0].Decorrenza == string.Empty && listaImportiEsteriApp[0].Importo == string.Empty)
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl("txtDecorrenza") != null && ((TextBox)row.FindControl("txtDecorrenza")).Text != string.Empty &&
                row.FindControl("txtImporto") != null && ((TextBox)row.FindControl("txtImporto")).Text != string.Empty)

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRow(GridViewRow row)
        {
            if (row.FindControl("lblDecorrenza") != null && ((Label)row.FindControl("lblDecorrenza")).Text != string.Empty &&
                row.FindControl("lblImporto") != null && ((Label)row.FindControl("lblImporto")).Text != string.Empty)
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
            save.ValidationGroup = "UCTabImportiEsteriCI";

        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteImportiEsteri")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private List<ImportiEsteri> AddRecord(List<ImportiEsteri> listaRecord, String decorrenza, String importo)
        {
            listaRecord.Add(new ImportiEsteri(decorrenza, importo));
            return listaRecord;
        }

        #endregion Methods Private

        #region EventHandler

        public event EventHandler AbilitaTastoSalva;
        public event EventHandler ShowAvvisoImportiEsteri;
        public event EventHandler ShowAvvisoEliminaImportiEsteri;

        protected void RaiseShowAvvisoImportiEsteri(object sender, EventArgs e)
        {
            ShowAvvisoImportiEsteri(sender, e);
        }

        protected void RaiseShowAvvisoEliminaImportiEsteri(object sender, EventArgs e)
        {
            ShowAvvisoEliminaImportiEsteri(sender, e);
        }

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        #endregion EventHandler

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributiviCi
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaDatiContributivi areaDatiContributiviCi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviCi
    }

    [Serializable]
    public class ImportiEsteri
    {
        public ImportiEsteri()
        { }

        public ImportiEsteri(string decorrenza, string importo)
        {
            this._Decorrenza = decorrenza;
            this._Importo = importo;
        }

        private string _Decorrenza;
        private string _Importo;

        public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public string Importo { get { return _Importo; } set { _Importo = value; } }
    }
}