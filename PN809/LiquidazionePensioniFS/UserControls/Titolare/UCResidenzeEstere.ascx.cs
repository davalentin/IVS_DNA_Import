using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Titolare
{
    public partial class UCResidenzeEstere : CustomBaseUserControl, ITitolarePensione
    {
        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterTitolare presenterTitolare = new PresenterTitolare();
            this.TitolarePensione = this.GetDatiUcResidenzeEstere();
            presenterTitolare.SalvaDatiTabResidenzeEstere(this);
            RaiseShowAvvisoResidenzeEstere(this, null);
        }

        protected void btnElimina_Click(object sender, EventArgs e)
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterTitolare presenterTitolare = new PresenterTitolare();
            presenterTitolare.EliminaDatiTabResidenzeEstere(this);
            if (!this.HasError)
            {
                List<ResidenzeEstere> ResidenzeEstereApp = new List<ResidenzeEstere>();
                ManageStartupGrid(ResidenzeEstereApp);
                GestionePulsanti(false);
            }

            RaiseShowAvvisoDeleteResidenzeEstere(this, null);
        }

        internal void ValorizzaEtichette()
        {
            ValorizzaViewStateFromService();

            List<ResidenzeEstere> elencoResidenzeEstere = (List<ResidenzeEstere>)ViewState["elencoResidenzeEstere"];

            if (elencoResidenzeEstere.Count == 0)
            {
                ManageStartupGrid(elencoResidenzeEstere);
                GestionePulsanti(false);
            }
            else
            {
                AddItemBlank(ref elencoResidenzeEstere);
                ViewState["elencoResidenzeEstere"] = elencoResidenzeEstere;
                gvResidenzeEstere_Load();
                GestionePulsanti(true);
            }
        }

        internal AreaTitolare GetDatiUcResidenzeEstere()
        {
            try
            {
                List<ResidenzeEstere> elencoResidenzeEstere = (List<ResidenzeEstere>)ViewState["elencoResidenzeEstere"];
                RemoveItemBlank(ref elencoResidenzeEstere);
                List<AreaTitolare.DatiResidenzaEsteroTitolare> elencoResidenzeEstereCod = new List<AreaTitolare.DatiResidenzaEsteroTitolare>();
                foreach (ResidenzeEstere residenzaEstera in elencoResidenzeEstere)
                {
                    if (!string.IsNullOrEmpty(residenzaEstera.CodStatoEstero) && !string.IsNullOrEmpty(residenzaEstera.Decorrenza))
                    {
                        AreaDecodifica.DatiStatoEstero reDecod = ConvertGv2SaveDataSource(residenzaEstera);
                        AreaTitolare.DatiResidenzaEsteroTitolare reCod = new AreaTitolare.DatiResidenzaEsteroTitolare();
                        reCod.CodCatastaleStatoEE = reDecod.CodCatastale;
                        reCod.Decorrenza = Utility.GetDateFromString(residenzaEstera.Decorrenza);
                        elencoResidenzeEstereCod.Add(reCod);
                    }
                }

                if (this.TitolarePensione == null)
                    this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                elencoResidenzeEstereCod.Sort(delegate (AreaTitolare.DatiResidenzaEsteroTitolare c1, AreaTitolare.DatiResidenzaEsteroTitolare c2) { return c1.Decorrenza.Value.CompareTo(c2.Decorrenza); });
                TitolarePensione.ElencoResidenzeEstereTitolare = elencoResidenzeEstereCod.ToArray();

                TitolarePensione.Pensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

                return TitolarePensione;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo GetDatiUcResidenzeEstere " + ex);
            }
        }

        internal void UpdateViewState(ITitolarePensione titolare)
        {
            ViewState["TitolarePensione"] = titolare.TitolarePensione;
        }

        #region Grid Residenze Estere

        protected void gvResidenzeEstere_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<ResidenzeEstere> elencoResidenzeEstere = (List<ResidenzeEstere>)ViewState["elencoResidenzeEstere"];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        EnableEditableMode(e.Row.Cells[0]);
                        DropDownList ddlSC = new DropDownList();
                        ddlSC = (DropDownList)e.Row.FindControl("ddlStatoEstero");
                        LoadDdl(ddlSC);
                        TextBox txtDecorrenza = (TextBox)e.Row.FindControl("txtDecorrenzaStatoEstero");
                        if (e.Row.RowIndex == 0 || !string.IsNullOrEmpty(txtDecorrenza.Text))
                            ddlSC.SelectedValue = ((ResidenzeEstere)e.Row.DataItem).CodStatoEstero;
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoResidenzeEstere.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            LinkButton delete = (LinkButton)e.Row.FindControl("btnDelete");
                            int index = e.Row.DataItemIndex;
                            if (index >= 0 && index <= elencoResidenzeEstere.Count - 2)
                            {
                                edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                edit.ToolTip = "Modifica";

                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
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
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo gvResidenzeEstere_RowDataBound" + ex);
            }
        }

        protected void gvResidenzeEstere_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            bool isEnable = false;
            Boolean okDecorrenza;
            Boolean okResidenzaEstera;
            List<ResidenzeEstere> elencoResidenzeEstere = (List<ResidenzeEstere>)ViewState["elencoResidenzeEstere"];

            if (e.CommandName == "Delete")
            {
                isEnable = true;
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                AddItemBlank(ref elencoResidenzeEstere);
                elencoResidenzeEstere.RemoveAt(r.DataItemIndex);
                if (elencoResidenzeEstere.Count == 1)
                {
                    RemoveItemBlank(ref elencoResidenzeEstere);
                    ManageStartupGrid(elencoResidenzeEstere);
                    isEnable = false;
                }
                else
                {
                    ViewState["elencoResidenzeEstere"] = elencoResidenzeEstere;
                    gvResidenzeEstere.EditIndex = -1;
                    gvResidenzeEstere_Load();
                }
            }
            else if (e.CommandName == "Edit")
            {
                RemoveItemBlank(ref elencoResidenzeEstere);
                if (elencoResidenzeEstere.Count == 1)
                {
                    ManagerFirstRecord(ref elencoResidenzeEstere);
                    AddItemBlank(ref elencoResidenzeEstere);
                    ViewState["elencoResidenzeEstere"] = elencoResidenzeEstere;
                }
                else
                    AddItemBlank(ref elencoResidenzeEstere);

                isEnable = false;
            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                if (elencoResidenzeEstere.Count() == 0)
                    AddItemBlank(ref elencoResidenzeEstere);

                if ((String.IsNullOrEmpty(((TextBox)(r.Cells[1].Controls[1])).Text)) || ((((TextBox)(r.Cells[1].Controls[1])).Text) == ""))
                    okDecorrenza = false;
                else
                    okDecorrenza = true;

                if ((String.IsNullOrEmpty(((DropDownList)(r.Cells[2].Controls[1])).Text)))
                    okResidenzaEstera = false;
                else
                    okResidenzaEstera = true;

                if (okDecorrenza && okResidenzaEstera)
                {
                    isEnable = true;
                    if ((r.DataItemIndex - 1) == (elencoResidenzeEstere.Count - 2))    //aggiunta riga (non si tratta di una modifica)
                    {
                        ResidenzeEstere residenze = new ResidenzeEstere();
                        residenze.Decorrenza = ((TextBox)(r.Cells[1].Controls[1])).Text;
                        residenze.StatoEstero = ((DropDownList)(r.Cells[2].Controls[1])).SelectedItem.Text;
                        residenze.CodStatoEstero = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue + string.Empty;
                        elencoResidenzeEstere.RemoveAt(elencoResidenzeEstere.Count - 1);
                        elencoResidenzeEstere.Add(residenze);
                        AddItemBlank(ref elencoResidenzeEstere);
                    }
                    else
                        saveValueRow(elencoResidenzeEstere, e, r);

                    gvResidenzeEstere.EditIndex = -1;
                    RaiseSalvaResidenzeEstere(this, null);
                }
                else
                    RaiseErrorSalvaResidenzeEstere(this, null);

                ViewState["elencoResidenzeEstere"] = elencoResidenzeEstere;
                gvResidenzeEstere_Load();
            }
            else if (e.CommandName == "Annulla")
            {
                if (elencoResidenzeEstere.Count() > 1)
                {
                    gvResidenzeEstere.EditIndex = -1;
                    gvResidenzeEstere_Load();
                    isEnable = true;
                }
            }

            GestionePulsanti(isEnable);
        }

        protected void gvResidenzeEstere_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvResidenzeEstere.EditIndex = -1;
                //Bind data to the GridView control.
                gvResidenzeEstere_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo gvResidenzeEstere_RowCancelingEdit" + ex);
            }
        }

        protected void gvResidenzeEstere_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvResidenzeEstere.PageIndex = e.NewPageIndex;
                gvResidenzeEstere_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo gvResidenzeEstere_onPageIndexChanging" + ex);
            }
        }

        protected void gvResidenzeEstere_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvResidenzeEstere.EditIndex = e.NewEditIndex;
                gvResidenzeEstere_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo gvResidenzeEstere_RowEditing" + ex);
            }
        }

        protected void gvResidenzeEstere_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<ResidenzeEstere> elencoResidenzeEstere = (List<ResidenzeEstere>)ViewState["elencoResidenzeEstere"];
                GridViewRow row = gvResidenzeEstere.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvResidenzeEstere.PageIndex * 10) + e.RowIndex);
                    //Se il record che sto editando è diverso dall'ultimo elemento della lista allora cancello l'ultimo elemento
                    if (elencoResidenzeEstere.Count != i + 1)
                        elencoResidenzeEstere.RemoveAt(elencoResidenzeEstere.Count - 1);
                    gvResidenzeEstere.EditIndex = -1;
                    ViewState["elencoResidenzeEstere"] = elencoResidenzeEstere;
                    gvResidenzeEstere_Load();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo gvResidenzeEstere_RowUpdating" + ex);
            }
        }

        protected void gvResidenzeEstere_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo gvResidenzeEstere_RowDeleting" + ex);
            }
        }

        #endregion Grid Residenze Estere

        #region Private Methods

        private void ValorizzaViewStateFromService()
        {
            if (ViewState["TitolarePensione"] == null)
                ViewState["TitolarePensione"] = this.TitolarePensione;

            if (ViewState["elencoResidenzeEstere"] == null)
            {
                List<AreaTitolare.DatiResidenzaEsteroTitolare> elencoResidenzeEstereCod = this.TitolarePensione.ElencoResidenzeEstereTitolare.ToList();
                List<ResidenzeEstere> elencoResidenzeEstere = new List<ResidenzeEstere>();

                foreach (AreaTitolare.DatiResidenzaEsteroTitolare residenzaEsteraCod in elencoResidenzeEstereCod)
                {
                    if (!string.IsNullOrEmpty(residenzaEsteraCod.Decorrenza.ToString()))
                    {
                        AreaDecodifica.DatiStatoEstero seDec = ConvertSave2GvDataSource(residenzaEsteraCod);
                        ResidenzeEstere re = new ResidenzeEstere();
                        if (seDec != null)
                        {
                            re.CodStatoEstero = seDec.CodCatastale;
                            re.StatoEstero = seDec.Descrizione;
                        }
                        re.Decorrenza = String.Format("{0:MM/yyyy}", residenzaEsteraCod.Decorrenza);

                        elencoResidenzeEstere.Add(re);
                    }
                }

                ViewState["elencoResidenzeEstere"] = elencoResidenzeEstere;
            }
        }

        private void ManageStartupGrid(List<ResidenzeEstere> elencoResidenzeEstere)
        {
            ManagerFirstRecord(ref elencoResidenzeEstere);
            gvResidenzeEstere.EditIndex = 0;

            List<ResidenzeEstere> ResidenzeEstereApp = new List<ResidenzeEstere>();
            ResidenzeEstereApp.Add(new ResidenzeEstere());
            ViewState["elencoResidenzeEstere"] = ResidenzeEstereApp;

            gvResidenzeEstere.DataSource = elencoResidenzeEstere;
            gvResidenzeEstere.DataKeyNames = new string[] { "CodStatoEstero" };
            gvResidenzeEstere.DataBind();
        }

        private void ManagerFirstRecord(ref List<ResidenzeEstere> elencoResidenzeEstere)
        {
            if (elencoResidenzeEstere.Count() == 0)
            {
                RaiseGetDecorrenzaPensione(this, null);
                RaiseGetResidenzaEstera(this, null);
                ResidenzeEstere re = new ResidenzeEstere();
                re.CodStatoEstero = hdn_lblCodiceComuneResidenza.Value;
                re.Decorrenza = string.Format("{0:MM/yyyy}", hdn_txtDecorrenzaPensione.Value);
                elencoResidenzeEstere.Add(re);
            }
        }

        private AreaDecodifica.DatiStatoEstero ConvertSave2GvDataSource(AreaTitolare.DatiResidenzaEsteroTitolare statoEsteroCod)
        {
            try
            {
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero[] listStatiEsteri = areaDecodifica.GetValuesDecodifica().ElencoStatiEsteri;
                AreaDecodifica.DatiStatoEstero seDec = listStatiEsteri.ToList().Find(
                    delegate (AreaDecodifica.DatiStatoEstero statoEstero)
                    { return statoEstero.CodCatastale == statoEsteroCod.CodCatastaleStatoEE; });
                return seDec;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo ConvertSave2GvDataSource" + ex);
            }
        }

        private void gvResidenzeEstere_Load()
        {
            try
            {
                List<ResidenzeEstere> elencoResidenzeEstere = (List<ResidenzeEstere>)ViewState["elencoResidenzeEstere"];
                gvResidenzeEstere.DataSource = elencoResidenzeEstere;
                gvResidenzeEstere.DataKeyNames = new string[] { "CodStatoEstero" };
                gvResidenzeEstere.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo GvResidenzeEstere_Load" + ex);
            }

        }

        private void LoadDdl(DropDownList ddlSC)
        {
            try
            {
                CodeUtility areaDecodifica = new CodeUtility();
                //Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero[] listStatiEsteri = areaDecodifica.GetValuesDecodifica().ElencoStatiEsteri;

                List<Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero> lStatiEsteri = areaDecodifica.GetValuesDecodifica().ElencoStatiEsteri.ToList();
                var idx = lStatiEsteri.FindIndex(x => x.CodCatastale == "Z000");
                var item = lStatiEsteri[idx];
                lStatiEsteri.RemoveAt(idx);
                lStatiEsteri.Insert(1, item);

                foreach (AreaDecodifica.DatiStatoEstero statoEstero in lStatiEsteri)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", statoEstero.Descrizione);
                    li.Text = statoEstero.Descrizione;
                    li.Value = statoEstero.CodCatastale;
                    ddlSC.Items.Add(li);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo LoadDdl" + ex);
            }
        }

        private AreaDecodifica.DatiStatoEstero ConvertGv2SaveDataSource(ResidenzeEstere re)
        {
            try
            {
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero[] listStatiEsteri = areaDecodifica.GetValuesDecodifica().ElencoStatiEsteri;
                AreaDecodifica.DatiStatoEstero reDec = listStatiEsteri.ToList().Find(
                    delegate (AreaDecodifica.DatiStatoEstero statoEstero)
                    {
                        return statoEstero.CodCatastale == re.CodStatoEstero;
                    }
                    );
                return reDec;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCResidenzeEstere, Errore nel metodo ConvertGv2SaveDataSource " + ex);
            }
        }

        private void AddItemBlank(ref List<ResidenzeEstere> elencoResidenzeEstere)
        {
            int index = elencoResidenzeEstere.FindIndex(delegate (ResidenzeEstere code)
            { return (string.IsNullOrEmpty(code.CodStatoEstero) && string.IsNullOrEmpty(code.Decorrenza) && string.IsNullOrEmpty(code.StatoEstero)); });

            if (index < 0)
                elencoResidenzeEstere.Add(new ResidenzeEstere());
        }

        private void RemoveItemBlank(ref List<ResidenzeEstere> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (ResidenzeEstere code)
                { return (string.IsNullOrEmpty(code.CodStatoEstero) && string.IsNullOrEmpty(code.Decorrenza) && string.IsNullOrEmpty(code.StatoEstero)); });

                if (index >= 0)
                    lista.RemoveAt(index);
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
            save.ValidationGroup = "UCTabResidenzeEstere";
            save.CommandName = "Salva";
        }

        private void saveValueRow(List<ResidenzeEstere> elencoResidenzeEstere, GridViewCommandEventArgs e, GridViewRow r)
        {
            if (elencoResidenzeEstere != null && elencoResidenzeEstere[r.DataItemIndex] != null)
            {
                if (!String.IsNullOrEmpty((((TextBox)(r.Cells[1].Controls[1])).Text)))
                    elencoResidenzeEstere[r.DataItemIndex].Decorrenza = ((TextBox)(r.Cells[1].Controls[1])).Text;
                if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)))
                    elencoResidenzeEstere[r.DataItemIndex].StatoEstero = ((DropDownList)(r.Cells[2].Controls[1])).SelectedItem.Text;
                if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)))
                    elencoResidenzeEstere[r.DataItemIndex].CodStatoEstero = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue + string.Empty;
            }
        }

        private void GestionePulsanti(bool isEnable)
        {
            btnSalva.Enabled = isEnable;
            btnElimina.Enabled = isEnable;
        }

        #endregion Private Methods

        #region Events

        public event EventHandler SalvaResidenzeEstere;
        public event EventHandler ErrorSalvaResidenzeEstere;
        public event EventHandler GetDecorrenzaPensione;
        public event EventHandler GetResidenzaEstera;
        public event EventHandler ShowAvvisoResidenzeEstere;
        public event EventHandler ShowAvvisoDeleteResidenzeEstere;

        protected void RaiseSalvaResidenzeEstere(object sender, EventArgs e)
        {
            if (SalvaResidenzeEstere != null)
                SalvaResidenzeEstere(sender, e);
        }

        protected void RaiseErrorSalvaResidenzeEstere(object sender, EventArgs e)
        {
            if (ErrorSalvaResidenzeEstere != null)
                ErrorSalvaResidenzeEstere(sender, e);
        }
        
        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        protected void RaiseGetResidenzaEstera(object sender, EventArgs e)
        {
            if (GetResidenzaEstera != null)
                GetResidenzaEstera(sender, e);
        }

        protected void RaiseShowAvvisoResidenzeEstere(object sender, EventArgs e)
        {
            ShowAvvisoResidenzeEstere(sender, e);
        }

        protected void RaiseShowAvvisoDeleteResidenzeEstere(object sender, EventArgs e)
        {
            ShowAvvisoDeleteResidenzeEstere(sender, e);
        }

        #endregion Events
    }
    [Serializable]
    public class ResidenzeEstere
    {
        public string Decorrenza { get; set; }
        public string StatoEstero { get; set; }
        public string CodStatoEstero { get; set; }
    }

}

