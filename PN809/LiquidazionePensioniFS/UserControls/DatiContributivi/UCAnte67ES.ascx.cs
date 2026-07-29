using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCAnte67ES : CustomBaseUserControl, IDatiContributivi
    {

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        protected void Page_Load(object sender, EventArgs e)
        {
            if (domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        private void RenderControls()
        {
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);

        }

        public void ValorizzaEtichette()
        {
            if (domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            switch (domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    if (this.areaDatiContributivi.DatiAnte67 != null)
                    {
                        var entityAnte67 = this.areaDatiContributivi.DatiAnte67;
                        if (entityAnte67.ContributiLegge37758Art24.HasValue)
                            this.txtContributiArt24.Text = entityAnte67.ContributiLegge37758Art24.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (entityAnte67.DecorrenzaArticolo24.HasValue)
                            this.txtDecorrenzaArt24.Text = entityAnte67.DecorrenzaArticolo24.Value.ToString("MM/yyyy");
                        if (entityAnte67.ImportoInPagamentoPre67.HasValue)
                            this.txtPensioneInPagamento.Text = entityAnte67.ImportoInPagamentoPre67.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (entityAnte67.PensioneFondoAl67.HasValue)
                            this.txtPensioneFondo.Text = entityAnte67.PensioneFondoAl67.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (entityAnte67.CodicePensioneInPagamentoPre67.HasValue)
                            this.ddlCodicePensione.SelectedValue = entityAnte67.CodicePensioneInPagamentoPre67.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        //DataGridView
                        List<ElementoArt57> dest = null;
                        ElementoArt57.MapToLocalObject(entityAnte67, ref dest);
                        gvElementArt57.DataSource = dest;

                        if (dest.Count == 0)
                        {
                            gvElementArt57.EditIndex = 0;
                        }

                        ViewState["elementiArt57"] = dest;

                        //add empty element
                        inserisciElementoCalcolo();
                        GvElementiArt57_Load();

                    }
                    break;
            }
        }

        public void RecuperaCampi()
        {
            if (areaDatiContributivi == null)
                areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();

            if (domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            switch (domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:

                    if (areaDatiContributivi.DatiAnte67 == null)
                        areaDatiContributivi.DatiAnte67 = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribDatiAnte67();
                    var entityAnte67 = this.areaDatiContributivi.DatiAnte67;

                    if (!String.IsNullOrEmpty(this.txtContributiArt24.Text))
                        entityAnte67.ContributiLegge37758Art24 = decimal.Parse(this.txtContributiArt24.Text);// .ToString();
                    if (!string.IsNullOrEmpty(this.txtDecorrenzaArt24.Text))
                        entityAnte67.DecorrenzaArticolo24 = DateTime.Parse(this.txtDecorrenzaArt24.Text);//.Value.ToString("{0:MM/yyyy}");
                    if (!string.IsNullOrEmpty(this.txtPensioneInPagamento.Text))
                        entityAnte67.ImportoInPagamentoPre67 = decimal.Parse(this.txtPensioneInPagamento.Text);
                    if (!string.IsNullOrEmpty(this.txtPensioneFondo.Text))
                        entityAnte67.PensioneFondoAl67 = decimal.Parse(this.txtPensioneFondo.Text);
                    if (!string.IsNullOrEmpty(this.ddlCodicePensione.SelectedValue))
                        entityAnte67.CodicePensioneInPagamentoPre67 = char.Parse(this.ddlCodicePensione.SelectedValue);
                    List<ElementoArt57> lst = (List<ElementoArt57>)ViewState["elementiArt57"];

                    ElementoArt57.MapFromLocalObject(lst, ref entityAnte67);

                    break;
            }
        }

        #region Grid Elementi Art 57

        protected void gvElementiArt57_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            List<ElementoArt57> lstElemCalc = (List<ElementoArt57>)ViewState["elementiArt57"];
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                lstElemCalc.RemoveAt(r.DataItemIndex);
                if (lstElemCalc.Count == 1)
                {
                    gvElementArt57.EditIndex = 0;
                }

                ViewState["elementiArt57"] = lstElemCalc;
                GvElementiArt57_Load();

                //if (btnSalva.Enabled == false)
                //    btnSalva.Enabled = true;

                //RaiseAbilitaTastoSalva(this, null);
            }
            else if (e.CommandName == "Edit")
            {
                //if (elencoStatoCivile[0].Decorrenza == "")
                //{
                //    RaiseGetDecorrenzaPensione(this, null);
                //    elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;

                //}
                //if (btnSalva.Enabled == true)
                //    btnSalva.Enabled = false;
                //RaiseDisabilitaTastoSalva(this, null);

            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                int idxRow = r.RowIndex;

                string sContributi = ((TextBox)(r.Cells[1].Controls[1])).Text;
                string sDecorrenza = ((TextBox)(r.Cells[2].Controls[1])).Text;
                decimal contrib = decimal.Parse(sContributi);
                DateTime decor = DateTime.Parse(sDecorrenza);
                ElementoArt57 elemCalcolo = new ElementoArt57(contrib, decor);

                if (idxRow == lstElemCalc.Count - 1)
                {
                    //elem nuovo
                    lstElemCalc.Insert(lstElemCalc.Count - 1, elemCalcolo);
                }
                else
                {
                    //edit elemento esistente
                    lstElemCalc.RemoveAt(idxRow);
                    lstElemCalc.Insert(idxRow, elemCalcolo);
                }
                ViewState["elementiArt57"] = lstElemCalc;
                gvElementArt57.EditIndex = -1;
                GvElementiArt57_Load();
            }
            else if (e.CommandName == "Cancel")
            {
                if (lstElemCalc.Count == 1)
                {
                    gvElementArt57.EditIndex = 0;
                }
                else
                    gvElementArt57.EditIndex = -1;
                GvElementiArt57_Load();
            }
        }

        protected void gvElementiArt57_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //try
            //{
            //    List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];

            //    //Reset the edit index.
            //    //btnAnnulla.Visible = false;
            //    //Bind data to the GridView control.
            //    GvStatoCivile_Load();

            //    if (elencoStatoCivile.Count == 1)
            //    {
            //        btnSalva.Enabled = false;
            //        RaiseDisabilitaTastoSalva(this, null);
            //    }
            //    else
            //    {
            //        btnSalva.Enabled = true;
            //        RaiseAbilitaTastoSalva(this, null);
            //    }

            //    //if (btnSalva.Enabled == false)
            //    //    btnSalva.Enabled = true;
            //    ////RaiseAnnullaStatoCivile(this, null);
            //    //RaiseAbilitaTastoSalva(this, null);

            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowCancelingEdit " + ex);
            //}

        }

        protected void gvElementiArt57_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvElementArt57.EditIndex = e.NewEditIndex;
                GvElementiArt57_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowEditing " + ex);
            }
        }

        protected void gvElementiArt57_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //try
            //{
            //    List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
            //    GridViewRow row = gvStatoCivile.Rows[e.RowIndex];
            //    if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
            //    {
            //        int i = ((gvStatoCivile.PageIndex * 10) + e.RowIndex);

            //        if (elencoStatoCivile.Count != i + 1)
            //            elencoStatoCivile.RemoveAt(elencoStatoCivile.Count - 1);
            //        gvStatoCivile.EditIndex = -1;
            //        ViewState["elencoStatoCivile"] = elencoStatoCivile;
            //        GvStatoCivile_Load();
            //        //RaiseAnnullaStatoCivile(this, null);
            //    }
            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            //}
        }

        protected void gvElementiArt57_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<ElementoArt57> elencoArt57 = (List<ElementoArt57>)ViewState["elementiArt57"];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCAnte67ES_Ante57";
                        save.CommandName = "Salva";

                        //DropDownList ddlSC = new DropDownList();

                        //TextBox txtDecorrenza = (TextBox)e.Row.FindControl("txtDecorrenza");
                        //txtDecorrenza.Text = DateTime.Parse(txtDecorrenza.Text).ToString("MM/yyyy");

                        //ddlSC.SelectedValue = ((StatoCivile)e.Row.DataItem).CodSCivile;

                        //if (btnSalvaDatiFondo.Enabled == true)
                        //    btnSalvaDatiFondo.Enabled = false;
                        // RaiseDisabilitaTastoSalva(this, null);

                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoArt57.Count - 1)
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
                            if (index >= 0 && index <= elencoArt57.Count - 2)
                            {
                                edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                edit.ToolTip = "Modifica";

                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
                            }
                        }
                    }
                }
                if (e.Row.RowIndex >= 3)
                {
                    e.Row.Visible = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowDataBound " + ex);
            }
        }

        protected void gvElementiArt57_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<ElementoArt57> elencoArt57 = (List<ElementoArt57>)ViewState["elementiArt57"];
                if (elencoArt57.Count < 1)
                    inserisciElementoCalcolo();
                GvElementiArt57_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowDeleting " + ex);
            }
        }

        protected void gvElementiArt57_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            //try
            //{
            //    gvStatoCivile.PageIndex = e.NewPageIndex;
            //    GvStatoCivile_Load();
            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_onPageIndexChanging" + ex);
            //}
        }

        private void GvElementiArt57_Load()
        {
            try
            {
                List<ElementoArt57> elencoElementiCalcolo = (List<ElementoArt57>)ViewState["elementiArt57"];
                gvElementArt57.DataSource = elencoElementiCalcolo;
                //gvElementiDiCalcolo.DataKeyNames = new string[] { "CodSCivile" };
                gvElementArt57.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo GvStatoCivile_Load " + ex);
            }
        }

        private void inserisciElementoCalcolo()
        {
            try
            {
                List<ElementoArt57> elencoStatoCivile = (List<ElementoArt57>)ViewState["elementiArt57"];
                AddItemBlank(ref elencoStatoCivile);
                ViewState["elementiArt57"] = (List<ElementoArt57>)elencoStatoCivile;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo InserisciStatoCivile " + ex);
            }
        }

        private void AddItemBlank(ref List<ElementoArt57> elencoArt57)
        {
            int index = elencoArt57.FindIndex(
                delegate (ElementoArt57 code)
                {
                    return (!code.Contributi.HasValue && !code.Decorrenza.HasValue);
                });

            if (index < 0)
                elencoArt57.Add(new ElementoArt57(null, null));
        }


        #endregion Grid Elementi Art 57


        #region Event

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion Event


        #region Nestled class
        [Serializable]
        private class ElementoArt57
        {
            public decimal? Contributi { get; set; }
            public DateTime? Decorrenza { get; set; }

            public ElementoArt57(decimal? contributi, DateTime? decorrenza)
            {
                this.Contributi = contributi;
                this.Decorrenza = decorrenza;
            }
            public static void MapToLocalObject(GestioneContribDatiAnte67 objBL, ref List<ElementoArt57> lst)
            {
                lst = new List<ElementoArt57>();

                if (objBL.ContributiLegge37758Art57Periodo1.HasValue && objBL.DecorrenzaLegge37758Art57Pre67Periodo1.HasValue)
                {
                    lst.Add(new ElementoArt57(objBL.ContributiLegge37758Art57Periodo1, objBL.DecorrenzaLegge37758Art57Pre67Periodo1));
                }
                if (objBL.ContributiLegge37758Art57Periodo2.HasValue && objBL.DecorrenzaLegge37758Art57Pre67Periodo2.HasValue)
                {
                    lst.Add(new ElementoArt57(objBL.ContributiLegge37758Art57Periodo2, objBL.DecorrenzaLegge37758Art57Pre67Periodo2));
                }
                if (objBL.ContributiLegge37758Art57Periodo3.HasValue && objBL.DecorrenzaLegge37758Art57Pre67Periodo3.HasValue)
                {
                    lst.Add(new ElementoArt57(objBL.ContributiLegge37758Art57Periodo3, objBL.DecorrenzaLegge37758Art57Pre67Periodo3));
                }

            }

            public static void MapFromLocalObject(List<ElementoArt57> lstUI, ref GestioneContribDatiAnte67 objBL)
            {
                if (lstUI == null)
                    return;

                if (lstUI.Count > 0 && lstUI[0].Contributi.HasValue && lstUI[0].Decorrenza.HasValue)
                {
                    objBL.ContributiLegge37758Art57Periodo1 = lstUI[0].Contributi;
                    objBL.DecorrenzaLegge37758Art57Pre67Periodo1 = lstUI[0].Decorrenza;
                }
                if (lstUI.Count > 1 && lstUI[1].Contributi.HasValue && lstUI[1].Decorrenza.HasValue)
                {
                    objBL.ContributiLegge37758Art57Periodo2 = lstUI[1].Contributi;
                    objBL.DecorrenzaLegge37758Art57Pre67Periodo2 = lstUI[1].Decorrenza;

                }
                if (lstUI.Count > 2 && lstUI[2].Contributi.HasValue && lstUI[2].Decorrenza.HasValue)
                {
                    objBL.ContributiLegge37758Art57Periodo3 = lstUI[2].Contributi;
                    objBL.DecorrenzaLegge37758Art57Pre67Periodo3 = lstUI[2].Decorrenza;
                }
            }
        }

        #endregion Nestled class

        protected void btnSalvaAnte67_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiAnte67(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaAnte67_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiAnte67(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Fondo";
            else
            {
                ClearForm();
                ValorizzaEtichette();
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);

        }



    }


}