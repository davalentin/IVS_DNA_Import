using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCSentenzaArt4 : CustomBaseUserControl, ILiquidazionePensioneAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaLiquidazionePensione areaLiquidazionePensioneAgo)
        {
            if (areaLiquidazionePensioneAgo != null && areaLiquidazionePensioneAgo.DatiSentenzaArt4 != null)
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                List<DatiSentenzaArt4.SentenzaArt4> listaSentenzaArt4 = areaLiquidazionePensioneAgo.DatiSentenzaArt4.lDatiSentenzaArt4 != null ? areaLiquidazionePensioneAgo.DatiSentenzaArt4.lDatiSentenzaArt4.ToList() : null;
                if (listaSentenzaArt4 == null)
                    listaSentenzaArt4 = new List<DatiSentenzaArt4.SentenzaArt4>();
                AddItemBlank(ref listaSentenzaArt4);
                ViewState[EnumViewState.ElencoSentenzaArt4.ToString()] = listaSentenzaArt4;

                GvSentenzaArt4_Load();
                if (gvSentenzaArt4.EditIndex != -1)
                    btnSalva.Enabled = false;
            }
        }

        internal DatiSentenzaArt4 GetDatiUcSentenzaArt4()
        {
            try
            {
                DatiSentenzaArt4 datiSentArt4 = new DatiSentenzaArt4();
                List<DatiSentenzaArt4.SentenzaArt4> listaDatiSentenzaArt4 = ((List<DatiSentenzaArt4.SentenzaArt4>)ViewState[EnumViewState.ElencoSentenzaArt4.ToString()]).ToList();
                RemoveItemBlank(ref listaDatiSentenzaArt4);
                datiSentArt4.lDatiSentenzaArt4 = listaDatiSentenzaArt4.ToArray();

                datiSentArt4.lDatiSentenzaArt4 = datiSentArt4.lDatiSentenzaArt4.ToList().OrderBy(x => x.DecorrenzaSentenza).ToArray();

                return datiSentArt4;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSentenzaArt4, Errore nel metodo GetDatiUcSentenzaArt4 " + ex);
            }
        }

        internal void UpdateViewState(ILiquidazionePensioneAgo liquidazione)
        {
            ViewState[EnumViewState.LiquidazionePensioneAgo.ToString()] = liquidazione.areaLiquidazionePensioneAgo;
        }

        #region Grid Sentenza Art. 4

        protected void gvSentenzaArt4_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiSentenzaArt4.SentenzaArt4> elencoSentenzaArt4 = (List<DatiSentenzaArt4.SentenzaArt4>)ViewState[EnumViewState.ElencoSentenzaArt4.ToString()];
            if (e.CommandName == "Elimina")
            {
                elencoSentenzaArt4 = new List<DatiSentenzaArt4.SentenzaArt4>();
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    if (rApp.DataItemIndex != r.DataItemIndex)
                    {
                        DatiSentenzaArt4.SentenzaArt4 se = new DatiSentenzaArt4.SentenzaArt4();
                        if (rApp.DataItemIndex == gvSentenzaArt4.EditIndex)
                        {
                            se.DecorrenzaSentenza = Utility.GetDateFromString(((TextBox)(rApp.FindControl("txtDecorrenzaSentenzaArt4"))).Text);
                            se.ImportoSentenza = CodeUtility.StringToNullableDecimal(((TextBox)(rApp.FindControl("txtImportoSentenzaArt4"))).Text);
                        }
                        else
                        {
                            se.DecorrenzaSentenza = Utility.GetDateFromString(((Label)(rApp.FindControl("lblDecorrenzaSentenzaArt4"))).Text);
                            se.ImportoSentenza = CodeUtility.StringToNullableDecimal(((Label)(rApp.FindControl("lblImportoSentenzaArt4"))).Text);
                        }
                        se.IsFromGP = CodeUtility.StringToNullableBool(((HiddenField)(rApp.FindControl("hdnIsFromGP"))).Value).GetValueOrDefault();

                        elencoSentenzaArt4.Add(se);
                    }
                }

                AddItemBlank(ref elencoSentenzaArt4);
                ViewState[EnumViewState.ElencoSentenzaArt4.ToString()] = elencoSentenzaArt4;
                if (elencoSentenzaArt4.Count > 1)
                    gvSentenzaArt4.EditIndex = -1;
                else
                    gvSentenzaArt4.EditIndex = 0;
                GvSentenzaArt4_Load();

            }
            else if (e.CommandName == "Edit")
            {
                if (btnSalva.Enabled == true)
                    btnSalva.Enabled = false;

            }
            else if (e.CommandName == "Salva")
            {
                elencoSentenzaArt4 = new List<DatiSentenzaArt4.SentenzaArt4>();
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    DatiSentenzaArt4.SentenzaArt4 se = new DatiSentenzaArt4.SentenzaArt4();
                    if (rApp.DataItemIndex == gvSentenzaArt4.EditIndex)
                    {
                        se.DecorrenzaSentenza = Utility.GetDateFromString(((TextBox)(rApp.FindControl("txtDecorrenzaSentenzaArt4"))).Text);
                        se.ImportoSentenza = CodeUtility.StringToNullableDecimal(((TextBox)(rApp.FindControl("txtImportoSentenzaArt4"))).Text);
                    }
                    else
                    {
                        se.DecorrenzaSentenza = Utility.GetDateFromString(((Label)(rApp.FindControl("lblDecorrenzaSentenzaArt4"))).Text);
                        se.ImportoSentenza = CodeUtility.StringToNullableDecimal(((Label)(rApp.FindControl("lblImportoSentenzaArt4"))).Text);
                    }
                    se.IsFromGP = CodeUtility.StringToNullableBool(((HiddenField)(rApp.FindControl("hdnIsFromGP"))).Value).GetValueOrDefault();
                    elencoSentenzaArt4.Add(se);
                }

                AddItemBlank(ref elencoSentenzaArt4);
                ViewState[EnumViewState.ElencoSentenzaArt4.ToString()] = elencoSentenzaArt4;
                gvSentenzaArt4.EditIndex = -1;
                GvSentenzaArt4_Load();

            }
            else if (e.CommandName == "Annulla")
            {
                if (elencoSentenzaArt4.Count == 1)
                {
                    elencoSentenzaArt4[0].DecorrenzaSentenza = null;
                    elencoSentenzaArt4[0].ImportoSentenza = null;
                    gvSentenzaArt4.EditIndex = 0;
                }
                else
                    gvSentenzaArt4.EditIndex = -1;

                GvSentenzaArt4_Load();
            }
            if (gvSentenzaArt4.EditIndex == -1 && e.CommandName != "Edit")
            {
                btnSalva.Enabled = true;
            }
            else btnSalva.Enabled = false;
        }

        protected void gvSentenzaArt4_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSentenzaArt4.EditIndex = e.NewEditIndex;
                GvSentenzaArt4_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSentenzaArt4, Errore nel metodo gvSentenzaArt4_RowEditing " + ex);
            }
        }

        protected void gvSentenzaArt4_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<DatiSentenzaArt4.SentenzaArt4> elencoSentenzaArt4 = (List<DatiSentenzaArt4.SentenzaArt4>)ViewState[EnumViewState.ElencoSentenzaArt4.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabSentenzaArt4", Page.Theme);
                        if (elencoSentenzaArt4[e.Row.DataItemIndex].IsFromGP == true)
                        {
                            TextBox txtDecorrenzaSentenza = (TextBox)e.Row.FindControl("txtDecorrenzaSentenzaArt4");
                            txtDecorrenzaSentenza.Enabled = false;
                        }
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoSentenzaArt4.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                            edit.ToolTip = "Modifica";

                            LinkButton delete = (LinkButton)(e.Row.FindControl("btnDelete"));
                            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                            delete.ToolTip = "Elimina";

                            if (elencoSentenzaArt4[e.Row.DataItemIndex].IsFromGP == true)
                            {
                                delete.Visible = false;
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
                throw new INPS.DNA.DnaApplicationException("UCSentenzaArt4, Errore nel metodo gvSentenzaArt4_RowDataBound " + ex);
            }
        }

        protected void gvSentenzaArt4_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSentenzaArt4.PageIndex = e.NewPageIndex;
                GvSentenzaArt4_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSentenzaArt4, Errore nel metodo gvSentenzaArt4_onPageIndexChanging" + ex);
            }
        }

        #endregion Grid Sentenza Art. 4

        #region Events

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        protected void btnSalvaSentenzaArt4_Click(object sender, EventArgs e)
        {
            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            this.areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();

            this.areaLiquidazionePensioneAgo.DatiSentenzaArt4 = this.GetDatiUcSentenzaArt4();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            presenterLiquidazionePensione.SalvaDatiTabSentenzaArt4(this);

            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaSentenzaArt4_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaLiquidazionePensioneAgo == null)
                this.areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();

            PresenterLiquidazionePensione presenter = new PresenterLiquidazionePensione();
            presenter.EliminaDatiTabSentenzaArt4(this);

            if (this.HasError)
                RaiseShowAvvisoElimina(this, null);
            else
            {
                RaiseShowAvvisoElimina(this, null);
                ValorizzaEtichette(this.areaLiquidazionePensioneAgo);
            }
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            if (ShowAvvisoElimina != null)
                ShowAvvisoElimina(sender, e);
        }
        #endregion Events

        #region Privete Methods

        private void GvSentenzaArt4_Load()
        {
            try
            {
                List<DatiSentenzaArt4.SentenzaArt4> elencoSentenzaArt4 = (List<DatiSentenzaArt4.SentenzaArt4>)ViewState[EnumViewState.ElencoSentenzaArt4.ToString()];
                gvSentenzaArt4.DataSource = elencoSentenzaArt4;
                if (elencoSentenzaArt4.Count == 1)
                    gvSentenzaArt4.EditIndex = 0;
                gvSentenzaArt4.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSentenzaArt4, Errore nel metodo GvSentenzaArt4_Load " + ex);
            }
        }

        private void AddItemBlank(ref List<DatiSentenzaArt4.SentenzaArt4> elencoSentenzaArt4)
        {
            int index = elencoSentenzaArt4.FindIndex(delegate (DatiSentenzaArt4.SentenzaArt4 code)
            {
                return (!code.DecorrenzaSentenza.HasValue && !code.ImportoSentenza.HasValue);
            });

            if (index < 0)
                elencoSentenzaArt4.Add(new DatiSentenzaArt4.SentenzaArt4());
        }

        private void RemoveItemBlank(ref List<DatiSentenzaArt4.SentenzaArt4> elencoSentenzaArt4)
        {
            if (elencoSentenzaArt4 != null && elencoSentenzaArt4.Count() > 0)
            {
                int index = elencoSentenzaArt4.FindIndex(delegate (DatiSentenzaArt4.SentenzaArt4 code)
                { return (!code.DecorrenzaSentenza.HasValue && !code.ImportoSentenza.HasValue); });

                if (index >= 0)
                    elencoSentenzaArt4.RemoveAt(index);
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        #endregion Private Methods

        #region Enum

        enum EnumViewState
        {
            ElencoSentenzaArt4,
            LiquidazionePensioneAgo,
            AreaLiquidazionePensione
        }
        #endregion Enum
    }
}