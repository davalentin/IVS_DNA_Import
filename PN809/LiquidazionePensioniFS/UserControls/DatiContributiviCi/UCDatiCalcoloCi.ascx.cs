using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi
{
    public partial class UCDatiCalcoloCi1 : CustomBaseUserControl, IDatiContributiviCi, ITitolarePensione
    {
        #region Enum
        public enum ColonneGvDatiRetributiviCi { Sett707 = 5 };
        #endregion Enum

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributiviCi
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaDatiContributivi areaDatiContributiviCi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviCi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.areaDatiContributiviCi != null)
                {
                    ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;

                    if (this.areaDatiContributiviCi.DatiCalcolo != null)
                        InitData();
                }
            }

            ckbCTRMaternitaExAcna.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            ckbCTRMaternitaExAcna.InputAttributes.Add("EnableClass", "onClassMaternitaExAcna");
        }

        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();

            GetDatiCalcoloTab();

            List<DatiRetributivi> listaDatiRetribApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
            List<DatiContributivi> listaDatiContribApp = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];

            if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0) || (listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
            {
                if (this.areaDatiContributiviCi == null)
                    this.areaDatiContributiviCi = new AreaDatiContributivi();

                if (this.areaDatiContributiviCi.DatiCalcolo == null)
                    this.areaDatiContributiviCi.DatiCalcolo = new GestioneContribDatiCalcolo();

                this.areaDatiContributiviCi.DatiCalcolo.IdPensione = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]).DatiCalcolo.IdPensione;
                this.areaDatiContributiviCi.DatiCalcolo.IsUnicarpe = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]).DatiCalcolo.IsUnicarpe;

                if ((listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
                {
                    List<GestioneAggiornamentoPECODatiContributivi> listContr = GetDataContributiviToSave(listaDatiContribApp);
                    int nDatiContributivi = listContr.Count();
                    this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi = new GestioneAggiornamentoPECODatiContributivi[nDatiContributivi];
                    this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi = listContr.ToArray();
                }

                if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0))
                {
                    List<GestioneAggiornamentoPECODatiRetributivi> listRetr = GetDataRetributiviToSave(listaDatiRetribApp);
                    int nDatiRetributivi = listRetr.Count();
                    this.areaDatiContributiviCi.DatiCalcolo.LDatiRetributivi = new GestioneAggiornamentoPECODatiRetributivi[nDatiRetributivi];
                    this.areaDatiContributiviCi.DatiCalcolo.LDatiRetributivi = listRetr.ToArray();
                }

                presenterDatiContributiviCi.SalvaTabDatiCalcoloCi(this);

                if (this.HasError)
                {
                    btnEliminaDatiCalcolo.Enabled = true;
                    ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;
                    //ReLoadData(this.areaDatiContributiviCi.DatiCalcolo.LDatiRetributivi != null ?
                    //    MapDatiRetributiviForView(this.areaDatiContributiviCi) : null,
                    //    this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi != null ?
                    //    MapDatiContributiviForView(this.areaDatiContributiviCi) : null);

                    RaiseShowErrorDatiCalcolo(this, null);
                    GestioneMaternitaAcna();
                }
                else
                    RaiseShowAvvisoDatiCalcolo(this, null);
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Calcolo da salvare";
                RaiseShowAvvisoDatiCalcolo(this, null);
            }
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviCI presenterDatiContributivi = new PresenterDatiContributiviCI();

            List<DatiRetributivi> listaDatiRetribApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
            List<DatiContributivi> listaDatiContribApp = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];

            GetDatiCalcoloTab();

            if ((listaDatiRetribApp != null && listaDatiRetribApp.Count() > 0) || (listaDatiContribApp != null && listaDatiContribApp.Count() > 0))
            {
                presenterDatiContributivi.EliminaTabDatiCalcoloCi(this);

                if (!this.HasError)
                {
                    modalitaEditRetributivi.Value = "false";
                    modalitaEditContributivi.Value = "false";
                    modalitaEditContributiviEsteri.Value = "false";

                    if (string.IsNullOrEmpty(this.ErrorMessage))
                        ResetDatiCalcolo();
                }
                else
                    this.ErrorMessage = "Errore durante l'eliminazione dei Dati Calcolo";
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Calcolo da eliminare";
            }

            RaiseShowAvvisoEliminaDatiCalcolo(this, null);
        }

        internal void GestioneMaternitaAcna()
        {
            if (!this.ckbCTRMaternitaExAcna.Checked)
            {
                Presenter.PresenterDatiContributiviCI presenterDatiContributiviCI = new PresenterDatiContributiviCI();
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                presenterDatiContributiviCI.GetDatiContributivi(this);
                if (HasError)
                    return;
                List<Presenter.SvrLiquidazioneCi.GestioneContribMaternitaAcna> listDatiMaternitaAcna = null;
                if (this.areaDatiContributiviCi != null)
                    listDatiMaternitaAcna = this.areaDatiContributiviCi.LMaternitaAcna != null ? this.areaDatiContributiviCi.LMaternitaAcna.ToList() : null;

                if (listDatiMaternitaAcna != null && listDatiMaternitaAcna.Count > 0)
                    this.ckbCTRMaternitaExAcna.Checked = true;
            }
        }

        internal GestioneContribDatiCalcolo GetDatiCalcolo()
        {
            return GetDatiCalcoloTab();
        }

        #region Dati Contributivi

        protected void gvDatiContributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiContributivi.EditIndex = e.NewEditIndex;
                List<DatiContributivi> listaDatiContrApp = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];
                gvDatiContributivi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowEditing " + ex);
            }
        }

        protected void gvDatiContributivi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvDatiContributivi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiContributivi.EditIndex = -1;

                List<DatiContributivi> listaDatiContrApp = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];
                gvDatiContributivi.DataSource = listaDatiContrApp;
                gvDatiContributivi.DataBind();

                SetBtnShowPopUpContrib(GetDatiPensione(this), listaDatiContrApp);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiContributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiContributivi> listaDatiContrApp = new List<DatiContributivi>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string CodGestioneApp = string.Empty;
                    string QuotaApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string AmmontareContrApp = string.Empty;
                    string MontanteContrApp = string.Empty;

                    if (!IsEmptyReadableRowContr(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            QuotaApp = ((Label)rApp.Cells[2].FindControl("lblQuota_item")).Text;
                            SettimaneApp = ((Label)rApp.Cells[3].FindControl("lblSettimane")).Text;
                            AmmontareContrApp = ((Label)rApp.Cells[4].FindControl("lblAmmontareContributivo")).Text;
                            MontanteContrApp = ((Label)rApp.Cells[5].FindControl("lblMontanteContributivo")).Text;
                            CodGestioneApp = ((Label)rApp.Cells[6].FindControl("lblIdCodeGestione")).Text;

                            listaDatiContrApp = AddRecordContributivi(listaDatiContrApp, CodGestioneApp, QuotaApp, SettimaneApp, AmmontareContrApp, MontanteContrApp);
                        }
                    }
                }
                if (listaDatiContrApp.Count == 0)
                {
                    this.modalitaEditContributivi.Value = "false";
                    GestioneTastoSalva();
                }

                listaDatiContrApp.Add(new DatiContributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                removeItemBlankDatiContributivi(ref listaDatiContrApp);
                ViewState["elencoDatiContributivi"] = listaDatiContrApp;
                gvDatiContributivi_Load();

                bool isStandard = (bool)ViewState["TipoGestione"];
                if (isStandard)
                {
                    List<DatiRetributivi> listaDatiRetribApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                    if (!listaDatiRetribApp.Exists(x => x.Quota == "A"))
                    {
                        if (!gvDatiContributiviEsteri.Visible)
                        {
                            gvDatiContributiviEsteri.Visible = true;
                            List<DatiContributiviEsteri> elencoDatiContributiviEsteri = new List<DatiContributiviEsteri>();
                            ViewState["elencoDatiContributiviEsteri"] = elencoDatiContributiviEsteri;
                            gvDatiContributiviEsteri_Load();
                        }
                    }
                    else
                    {
                        if (gvDatiContributiviEsteri.Visible)
                        {
                            gvDatiContributiviEsteri.Visible = false;
                            modalitaEditContributiviEsteri.Value = "false";
                            ViewState["elencoDatiContributiviEsteri"] = null;
                        }
                    }
                }

                if (gvDatiContributiviEsteri.Visible && Convert.ToBoolean(modalitaEditContributiviEsteri.Value))
                    gvDatiContributiviEsteri_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditContributivi.Value = "true";
                GestioneTastoSalva();
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowContr((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiContributivi> listaDatiContrApp = new List<DatiContributivi>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string CodGestioneApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string AmmontareContrApp = string.Empty;
                        string MontanteContrApp = string.Empty;
                        string QuotaApp = string.Empty;

                        if (!IsEmptyEditableRowContr(rApp))
                        {
                            CodGestioneApp = ((DropDownList)rApp.Cells[1].FindControl("ddlCodiceGestione")).SelectedValue;
                            QuotaApp = ((DropDownList)rApp.Cells[2].FindControl("ddlQuota")).SelectedValue;
                            SettimaneApp = ((TextBox)rApp.Cells[3].Controls[1]).Text;
                            AmmontareContrApp = ((TextBox)rApp.Cells[4].Controls[1]).Text;
                            MontanteContrApp = ((TextBox)rApp.Cells[5].Controls[1]).Text;
                            listaDatiContrApp = AddRecordContributivi(listaDatiContrApp, CodGestioneApp, QuotaApp, SettimaneApp, AmmontareContrApp, MontanteContrApp);
                        }
                        else if (!IsEmptyReadableRowContr(rApp))
                        {
                            QuotaApp = ((Label)rApp.Cells[2].FindControl("lblQuota_item")).Text;
                            SettimaneApp = ((Label)rApp.Cells[3].FindControl("lblSettimane")).Text;
                            AmmontareContrApp = ((Label)rApp.Cells[4].FindControl("lblAmmontareContributivo")).Text;
                            MontanteContrApp = ((Label)rApp.Cells[5].FindControl("lblMontanteContributivo")).Text;
                            CodGestioneApp = ((Label)rApp.Cells[6].FindControl("lblIdCodeGestione")).Text;
                            
                            listaDatiContrApp = AddRecordContributivi(listaDatiContrApp, CodGestioneApp, QuotaApp, SettimaneApp, AmmontareContrApp, MontanteContrApp);
                        }
                    }
                    listaDatiContrApp.Add(new DatiContributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                    gvDatiContributivi.EditIndex = -1;
                    ViewState["elencoDatiContributivi"] = listaDatiContrApp;

                    modalitaEditContributivi.Value = "false";
                    GestioneTastoSalva();

                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();

                    SetBtnShowPopUpContrib(GetDatiPensione(this), listaDatiContrApp);

                    bool isStandard = (bool)ViewState["TipoGestione"];
                    if (isStandard)
                    {
                        List<DatiRetributivi> listaDatiRetribApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                        if (!listaDatiRetribApp.Exists(x => x.Quota == "A"))
                        {
                            if (!gvDatiContributiviEsteri.Visible)
                            {
                                gvDatiContributiviEsteri.Visible = true;
                                List<DatiContributiviEsteri> elencoDatiContributiviEsteri = new List<DatiContributiviEsteri>();
                                ViewState["elencoDatiContributiviEsteri"] = elencoDatiContributiviEsteri;
                                gvDatiContributiviEsteri_Load();
                            }
                        }
                        else
                        {
                            if (gvDatiContributiviEsteri.Visible)
                            {
                                gvDatiContributiviEsteri.Visible = false;
                                modalitaEditContributiviEsteri.Value = "false";
                                ViewState["elencoDatiContributiviEsteri"] = null;
                            }
                        }
                    }

                    if (gvDatiContributiviEsteri.Visible && Convert.ToBoolean(modalitaEditContributiviEsteri.Value))
                        gvDatiContributiviEsteri_Load();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiContributivi> listaDatiContrApp = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];
                if (!IsListaEmpty(false, false))
                {
                    modalitaEditContributivi.Value = "false";
                    GestioneTastoSalva();

                    gvDatiContributivi.EditIndex = -1;
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();

                    SetBtnShowPopUpContrib(GetDatiPensione(this), listaDatiContrApp);
                }
            }
        }

        protected void gvDatiContributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            this.areaDatiContributiviCi = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.areaDatiContributiviCi.DatiCalcolo.IsUnicarpe)
                    {
                        gvDatiContributivi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributivi)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributivi)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributivi)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributivi)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).AmmontareContributivo;
                        ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).MontanteContributivo;

                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    else
                    {
                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaEmpty(false, false) && !Convert.ToBoolean(modalitaEditContributivi.Value))
                            {
                                gvDatiContributivi.EditIndex = 0;
                                modalitaEditContributivi.Value = "true";
                                GestioneTastoSalva();

                                gvDatiContributivi.DataSource = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];
                                gvDatiContributivi.DataBind();
                            }
                            else if (IsEmptyEditableRowContr(e.Row))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false, false);
                                    EnableEditableModeContr(e.Row.Cells[0]);
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[6].FindControl("btnDeleteContributivi")));
                                    delete.Text = string.Empty;
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributivi)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributivi)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributivi)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributivi)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).AmmontareContributivo;
                                    ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).MontanteContributivo;

                                    EnableReadableModeContr(e.Row.Cells[0], e.Row.Cells[5]);
                                }

                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, false, false);
                                    EnableEditableModeContr(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributivi)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributivi)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributivi)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributivi)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).AmmontareContributivo;
                                    ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).MontanteContributivo;

                                    EnableReadableModeContr(e.Row.Cells[0], e.Row.Cells[5]);
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, false, false);
                                EnableEditableModeContr(e.Row.Cells[0]);
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiContributivi>)ViewState["elencoDatiContributivi"]).Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));

                                //if (((List<DatiContributivi>)ViewState["elencoDatiContributivi"]).Count == 2)
                                //{
                                //    add.Text = string.Empty;
                                //}
                                //else
                                //{ 
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                                //}
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributivi)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributivi)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributivi)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributivi)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).AmmontareContributivo;
                                ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = ((DatiContributivi)(e.Row.DataItem)).MontanteContributivo;

                                EnableReadableModeContr(e.Row.Cells[0], e.Row.Cells[5]);
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
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowDataBound " + ex);
            }
        }

        #endregion Dati Contributivi

        #region Dati Retributivi

        protected void gvDatiRetributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            this.areaDatiContributiviCi = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.areaDatiContributiviCi.DatiCalcolo.IsUnicarpe)
                    {
                        gvDatiRetributivi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributivi)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributivi)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributivi)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiRetributivi)(e.Row.DataItem)).Decorrenza;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = ((DatiRetributivi)(e.Row.DataItem)).RedditoRetribuzioneMedia;
                        ((Label)e.Row.FindControl("lblSettimane707CI")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane707;

                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    else
                    {
                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaEmpty(true, false) && !Convert.ToBoolean(modalitaEditRetributivi.Value))
                            {
                                gvDatiRetributivi.EditIndex = 0;
                                modalitaEditRetributivi.Value = "true";
                                GestioneTastoSalva();

                                gvDatiRetributivi.DataSource = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                                gvDatiRetributivi.DataBind();
                            }
                            else if (IsEmptyEditableRowRetrib(e.Row))
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true, false);
                                    EnableEditableModeRetr(e.Row.Cells[0]);
                                    LinkButton delete = ((LinkButton)(e.Row.Cells[6].FindControl("btnDeleteRetributivi")));
                                    delete.Text = string.Empty;
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributivi)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributivi)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributivi)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiRetributivi)(e.Row.DataItem)).Decorrenza;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = ((DatiRetributivi)(e.Row.DataItem)).RedditoRetribuzioneMedia;
                                    ((Label)e.Row.FindControl("lblSettimane707CI")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane707;
                                    EnableReadableModeRetr(e.Row.Cells[0], e.Row.Cells[5]);
                                }

                            }
                            else  //prima riga non vuota
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    GestioneDdls(e.Row, true, false);
                                    EnableEditableModeRetr(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributivi)(e.Row.DataItem)).Gestione);
                                    ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributivi)(e.Row.DataItem)).Gestione;
                                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributivi)(e.Row.DataItem)).Quota;
                                    ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiRetributivi)(e.Row.DataItem)).Decorrenza;
                                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane;
                                    ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = ((DatiRetributivi)(e.Row.DataItem)).RedditoRetribuzioneMedia;
                                    ((Label)e.Row.FindControl("lblSettimane707CI")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane707;
                                    EnableReadableModeRetr(e.Row.Cells[0], e.Row.Cells[5]);
                                }
                            }
                        }
                        else  // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, true, false);
                                EnableEditableModeRetr(e.Row.Cells[0]);
                            }

                            else if (e.Row.DataItemIndex == ((List<DatiRetributivi>)ViewState["elencoDatiRetributivi"]).Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));

                                //if (((List<DatiRetributivi>)ViewState["elencoDatiRetributivi"]).Count == 3)
                                //    add.Text = string.Empty;
                                //else
                                //{
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                                //}
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributivi)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributivi)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributivi)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiRetributivi)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = ((DatiRetributivi)(e.Row.DataItem)).RedditoRetribuzioneMedia;
                                ((Label)e.Row.FindControl("lblSettimane707CI")).Text = ((DatiRetributivi)(e.Row.DataItem)).Settimane707;
                                EnableReadableModeRetr(e.Row.Cells[0], e.Row.Cells[5]);
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
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiRetributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiRetributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiRetributivi> listaDatiRetribApp = new List<DatiRetributivi>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string GestioneApp = string.Empty;
                    string QuotaApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string DecorrenzaApp = string.Empty;
                    string RetribuzioneMediaApp = string.Empty;
                    string Sett707Ci = string.Empty;

                    if (!IsEmptyReadableRowRetrib(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            GestioneApp = ((Label)rApp.Cells[6].FindControl("lblIdCodeGestione")).Text;
                            QuotaApp = ((Label)rApp.Cells[2].FindControl("lblQuota_item")).Text;
                            DecorrenzaApp = ((Label)rApp.FindControl("lblDecorrenza")).Text;
                            SettimaneApp = ((Label)rApp.Cells[4].FindControl("lblSettimane")).Text;
                            RetribuzioneMediaApp = ((Label)rApp.Cells[5].FindControl("lblRetribuzioneMedia")).Text;
                            Sett707Ci = ((Label)rApp.FindControl("lblSettimane707CI")).Text;

                            listaDatiRetribApp = AddRecordRetributivi(listaDatiRetribApp, GestioneApp, QuotaApp, DecorrenzaApp, SettimaneApp, RetribuzioneMediaApp, Sett707Ci);
                        }
                    }
                }
                if (listaDatiRetribApp.Count == 0)
                {
                    this.modalitaEditRetributivi.Value = "false";
                    GestioneTastoSalva();
                }

                listaDatiRetribApp.Add(new DatiRetributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                removeItemBlankDatiRetributivi(ref listaDatiRetribApp);
                ViewState["elencoDatiRetributivi"] = listaDatiRetribApp;

                gvDatiRetributivi_Load();

                bool isStandard = (bool)ViewState["TipoGestione"];
                if (isStandard)
                {
                    if (!listaDatiRetribApp.Exists(x => x.Quota == "A"))
                    {
                        if (!gvDatiContributiviEsteri.Visible)
                        {
                            gvDatiContributiviEsteri.Visible = true;
                            List<DatiContributiviEsteri> elencoDatiContributiviEsteri = new List<DatiContributiviEsteri>();
                            ViewState["elencoDatiContributiviEsteri"] = elencoDatiContributiviEsteri;
                            gvDatiContributiviEsteri_Load();
                        }
                    }
                    else
                    {
                        if (gvDatiContributiviEsteri.Visible)
                        {
                            gvDatiContributiviEsteri.Visible = false;
                            modalitaEditContributiviEsteri.Value = "false";
                            ViewState["elencoDatiContributiviEsteri"] = null;
                        }
                    }
                }

                if (gvDatiContributiviEsteri.Visible && Convert.ToBoolean(modalitaEditContributiviEsteri.Value))
                    gvDatiContributiviEsteri_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditRetributivi.Value = "true";
                GestioneTastoSalva();
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRetrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiRetributivi> listaDatiRetrApp = new List<DatiRetributivi>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string GestioneApp = string.Empty;
                        string QuotaApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string DecorrenzaApp = string.Empty;
                        string RetribuzioneMediaApp = string.Empty;
                        string Sett707Ci = string.Empty;

                        if (!IsEmptyEditableRowRetrib(rApp))
                        {
                            GestioneApp = ((DropDownList)rApp.Cells[1].FindControl("ddlCodiceGestione")).SelectedValue;
                            QuotaApp = ((DropDownList)rApp.Cells[2].FindControl("ddlQuota")).SelectedValue;
                            DecorrenzaApp = ((Label)rApp.FindControl("lblDecorrenza")).Text;
                            SettimaneApp = ((TextBox)rApp.Cells[4].Controls[1]).Text;
                            RetribuzioneMediaApp = ((TextBox)rApp.Cells[5].Controls[1]).Text;

                            Sett707Ci = ((TextBox)rApp.FindControl("txtSettimaneRetributive707CI")).Text;

                            listaDatiRetrApp = AddRecordRetributivi(listaDatiRetrApp, GestioneApp, QuotaApp, DecorrenzaApp, SettimaneApp, RetribuzioneMediaApp, Sett707Ci);
                        }
                        else if (!IsEmptyReadableRowRetrib(rApp))
                        {
                            GestioneApp = ((Label)rApp.Cells[1].FindControl("lblIdCodeGestione")).Text;
                            QuotaApp = ((Label)rApp.Cells[2].FindControl("lblQuota_item")).Text;
                            SettimaneApp = ((Label)rApp.Cells[4].FindControl("lblSettimane")).Text;
                            DecorrenzaApp = ((Label)rApp.FindControl("lblDecorrenza")).Text;
                            RetribuzioneMediaApp = ((Label)rApp.Cells[5].FindControl("lblRetribuzioneMedia")).Text;
                            Sett707Ci = ((Label)rApp.FindControl("lblSettimane707CI")).Text;

                            listaDatiRetrApp = AddRecordRetributivi(listaDatiRetrApp, GestioneApp, QuotaApp, DecorrenzaApp, SettimaneApp, RetribuzioneMediaApp, Sett707Ci);
                        }
                    }

                    listaDatiRetrApp.Add(new DatiRetributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvDatiRetributivi.EditIndex = -1;
                    ViewState["elencoDatiRetributivi"] = listaDatiRetrApp;

                    modalitaEditRetributivi.Value = "false";
                    GestioneTastoSalva();

                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();


                    bool isStandard = (bool)ViewState["TipoGestione"];
                    if (isStandard)
                    {
                        if (!listaDatiRetrApp.Exists(x => x.Quota == "A"))
                        {
                            if (!gvDatiContributiviEsteri.Visible)
                            {
                                gvDatiContributiviEsteri.Visible = true;
                                List<DatiContributiviEsteri> elencoDatiContributiviEsteri = new List<DatiContributiviEsteri>();
                                ViewState["elencoDatiContributiviEsteri"] = elencoDatiContributiviEsteri;
                                gvDatiContributiviEsteri_Load();
                            }
                        }
                        else
                        {
                            if (gvDatiContributiviEsteri.Visible)
                            {
                                gvDatiContributiviEsteri.Visible = false;
                                modalitaEditContributiviEsteri.Value = "false";
                                ViewState["elencoDatiContributiviEsteri"] = null;
                            }
                        }
                    }

                    if (gvDatiContributiviEsteri.Visible && Convert.ToBoolean(modalitaEditContributiviEsteri.Value))
                        gvDatiContributiviEsteri_Load();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiRetributivi> listaDatiRetrApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                if (!IsListaEmpty(true, false))
                {
                    modalitaEditRetributivi.Value = "false";
                    GestioneTastoSalva();

                    gvDatiRetributivi.EditIndex = -1;
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();
                }
            }
        }

        protected void gvDatiRetributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiRetributivi.EditIndex = e.NewEditIndex;
                List<DatiRetributivi> listaDatiRetrApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                gvDatiRetributivi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiRetributivi_RowEditing " + ex);
            }
        }

        protected void gvDatiRetributivi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiRetributivi.EditIndex = -1;

                List<DatiRetributivi> listaDatiRetrApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                gvDatiRetributivi.DataSource = listaDatiRetrApp;
                gvDatiRetributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiRetributiviCi, Errore nel metodo gvDatiRetributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiRetributivi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvDatiRetributivi_LoadEvent(object sender, EventArgs e)
        {
            this.areaDatiContributiviCi = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]);
            if (areaDatiContributiviCi != null)
                gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributiviCi.Sett707].Visible = areaDatiContributiviCi.IsSettimane707Visible;
        }

        #endregion Dati Retributivi

        #region Dati Contributivi Esteri

        protected void gvDatiContributiviEsteri_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            this.areaDatiContributiviCi = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty(false, true) && !Convert.ToBoolean(modalitaEditContributiviEsteri.Value))
                        {
                            gvDatiContributiviEsteri.EditIndex = 0;
                            modalitaEditContributiviEsteri.Value = "true";

                            gvDatiContributiviEsteri.DataSource = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];
                            gvDatiContributiviEsteri.DataBind();
                        }
                        else if (IsEmptyEditableRowContrEsteri(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                if (!Convert.ToBoolean(modalitaEditContributivi.Value) || !Convert.ToBoolean(modalitaEditRetributivi.Value))
                                    GestioneDdls(e.Row, false, true);

                                EnableEditableModeContrEsteri(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[3].FindControl("btnDeleteContributiEsteri")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContrEsteri(((DatiContributiviEsteri)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Settimane;

                                EnableReadableModeContrEsteri(e.Row.Cells[0], e.Row.Cells[4]);
                            }

                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                if (!Convert.ToBoolean(modalitaEditContributivi.Value) || !Convert.ToBoolean(modalitaEditRetributivi.Value))
                                    GestioneDdls(e.Row, false, true);
                                EnableEditableModeContrEsteri(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContrEsteri(((DatiContributiviEsteri)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Decorrenza;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Settimane;
                                EnableReadableModeContrEsteri(e.Row.Cells[0], e.Row.Cells[3]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            GestioneDdls(e.Row, false, true);
                            EnableEditableModeContrEsteri(e.Row.Cells[0]);
                        }
                        else if (e.Row.DataItemIndex == ((List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContrEsteri(((DatiContributiviEsteri)(e.Row.DataItem)).Gestione);
                            ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Gestione;
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Decorrenza;
                            ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviEsteri)(e.Row.DataItem)).Settimane;
                            EnableReadableModeContrEsteri(e.Row.Cells[0], e.Row.Cells[3]);
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
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributiviEsteri_RowDataBound " + ex);
            }
        }

        protected void gvDatiContributiviEsteri_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiContributiviEsteri> listaDatiContribEsteriApp = new List<DatiContributiviEsteri>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string GestioneApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string DecorrenzaApp = string.Empty;

                    if (!IsEmptyReadableRowContrEsteri(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            GestioneApp = ((Label)rApp.Cells[1].FindControl("lblIdCodeGestione")).Text;
                            DecorrenzaApp = ((Label)rApp.Cells[2].FindControl("lblDecorrenza")).Text;
                            SettimaneApp = ((Label)rApp.Cells[3].FindControl("lblSettimane")).Text;

                            listaDatiContribEsteriApp = AddRecordContributiviEsteri(listaDatiContribEsteriApp, GestioneApp, DecorrenzaApp, SettimaneApp);
                        }
                    }
                }
                if (listaDatiContribEsteriApp.Count == 0)
                    this.modalitaEditContributiviEsteri.Value = "false";

                listaDatiContribEsteriApp.Add(new DatiContributiviEsteri(string.Empty, string.Empty, string.Empty));

                removeItemBlankDatiContributiviEsteri(ref listaDatiContribEsteriApp);
                ViewState["elencoDatiContributiviEsteri"] = listaDatiContribEsteriApp;

                gvDatiContributiviEsteri_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowContrEsteri((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiContributiviEsteri> listaDatiContribEsteriApp = new List<DatiContributiviEsteri>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string GestioneApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string DecorrenzaApp = string.Empty;

                        if (!IsEmptyEditableRowContrEsteri(rApp))
                        {
                            GestioneApp = ((DropDownList)rApp.Cells[1].FindControl("ddlCodiceGestione")).SelectedValue;
                            DecorrenzaApp = ((TextBox)rApp.Cells[2].Controls[1]).Text;
                            SettimaneApp = ((TextBox)rApp.Cells[3].Controls[1]).Text;
                            listaDatiContribEsteriApp = AddRecordContributiviEsteri(listaDatiContribEsteriApp, GestioneApp, DecorrenzaApp, SettimaneApp);
                        }
                        else if (!IsEmptyReadableRowContrEsteri(rApp))
                        {
                            GestioneApp = ((Label)rApp.Cells[1].FindControl("lblIdCodeGestione")).Text;
                            SettimaneApp = ((Label)rApp.Cells[2].FindControl("lblSettimane")).Text;
                            DecorrenzaApp = ((Label)rApp.Cells[3].FindControl("lblDecorrenza")).Text;

                            listaDatiContribEsteriApp = AddRecordContributiviEsteri(listaDatiContribEsteriApp, GestioneApp, DecorrenzaApp, SettimaneApp);
                        }
                    }
                    //listaDatiContribEsteriApp.Add(new DatiContributiviEsteri(string.Empty, string.Empty, string.Empty));
                    gvDatiContributiviEsteri.EditIndex = -1;
                    ViewState["elencoDatiContributiviEsteri"] = listaDatiContribEsteriApp;

                    gvDatiContributiviEsteri.DataSource = listaDatiContribEsteriApp;
                    gvDatiContributiviEsteri.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiContributiviEsteri> listaDatiContribEsteriApp = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];
                if (!IsListaEmpty(false, true))
                {
                    if (listaDatiContribEsteriApp == null)
                        listaDatiContribEsteriApp = new List<DatiContributiviEsteri>();
                    if (listaDatiContribEsteriApp.Count == 0)
                    {
                        listaDatiContribEsteriApp.Add(new DatiContributiviEsteri(string.Empty, string.Empty, string.Empty));
                        gvDatiContributiviEsteri.EditIndex = 0;
                    }
                    else
                        gvDatiContributiviEsteri.EditIndex = -1;
                    gvDatiContributiviEsteri.DataSource = listaDatiContribEsteriApp;
                    gvDatiContributiviEsteri.DataBind();
                }
            }
        }

        protected void gvDatiContributiviEsteri_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiContributiviEsteri.EditIndex = e.NewEditIndex;
                //List<DatiContributiviEsteri> listaDatiContribEsteriApp = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];
                gvDatiContributiviEsteri_Load();
                //gvDatiContributiviEsteri.DataSource = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];
                //gvDatiContributiviEsteri.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributiviEsteri_RowEditing " + ex);
            }
        }

        protected void gvDatiContributiviEsteri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiContributiviEsteri.EditIndex = -1;

                List<DatiContributiviEsteri> listaDatiContribEsteriApp = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];
                gvDatiContributiviEsteri.DataSource = listaDatiContribEsteriApp;
                gvDatiContributiviEsteri.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributiviEsteri_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiContributiviEsteri_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        #endregion Dati Contributivi Esteri

        #region Methods Private

        private void InitData()
        {
            //controllo implementato in seguito alle mod. riportate nel doc di specifiche (v.02)
            if (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
            {
                ViewState["TipoGestione"] = GetCategoria(domanda);

                AreaTitolare titolare = new AreaTitolare();
                titolare.Pensione = GetDatiPensione(this);
                GestioneEtichetteIsUnicarpe(titolare.Pensione);

                LoadDecodificaData(this.areaDatiContributiviCi);

                ValorizzaEtichette(this.areaDatiContributiviCi.DatiCalcolo);


            }
        }

        private void ResetDatiCalcolo()
        {
            bool isStandard = (bool)ViewState["TipoGestione"];
            this.areaDatiContributiviCi.DatiCalcolo.ContributiItalianiEdEsteriAl1295 = null;
            this.areaDatiContributiviCi.DatiCalcolo.MontanteInvalidita = null;

            if ((List<DatiContributivi>)(ViewState["elencoDatiContributivi"]) != null)
                ((List<DatiContributivi>)(ViewState["elencoDatiContributivi"])).Clear();
            this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi = null;

            if ((List<DatiRetributivi>)(ViewState["elencoDatiRetributivi"]) != null)
                ((List<DatiRetributivi>)(ViewState["elencoDatiRetributivi"])).Clear();
            this.areaDatiContributiviCi.DatiCalcolo.LDatiRetributivi = null;

            if ((List<DatiContributiviEsteri>)(ViewState["elencoDatiContributiviEsteri"]) != null)
                ((List<DatiContributiviEsteri>)(ViewState["elencoDatiContributiviEsteri"])).Clear();
            this.areaDatiContributiviCi.DatiCalcolo.LDatiContributiEsteri = null;

            if (this.areaDatiContributiviCi.DatiCalcolo.IsUnicarpe)
            {
                this.txtContributiItalianiEsteri.Text = string.Empty;
                if (!isStandard)
                    gvDatiContributiviEsteri_Load();

                ValorizzaEtichetteEmpty();

                RaiseInitializeData(null, null);
                ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;

                ValorizzaEtichette(this.areaDatiContributiviCi.DatiCalcolo);
            }
            else
            {
                ValorizzaEtichetteEmpty();
                this.areaDatiContributiviCi.DatiCalcolo = new GestioneContribDatiCalcolo();
                this.areaDatiContributiviCi.DatiCalcolo.IsUnicarpe = (bool)((AreaTitolare.DatiPensione)Session["DatiPensione"]).FlagUnicarpe;
                ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;
            }
        }

        private void GestioneTastoSalva()
        {
            if (modalitaEditContributivi.Value == "false" || modalitaEditRetributivi.Value == "false")
            {
                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
            else
            {
                btnSalvaDatiCalcolo.Enabled = btnPopUp.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
        }

        private bool GetCategoria(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            bool isNormale = false;

            //gestione "normale"
            if (domanda.Categoria.Trim() == "VOS" || domanda.Categoria.Trim() == "IOS" || domanda.Categoria.Trim() == "SOS")
                isNormale = true;
            //commentato in seguito alle mod. riportate nel doc di specifiche (v.02)
            //else
            //{
            //    //gestione "speciale"
            //    //if (domanda.Categoria.Trim().EndsWith("S")) //commentato in seguito alle mod. riportate nel doc di specifiche (v.02)
            //    categoria = false;
            //}

            return isNormale;
        }

        private void GestioneDdls(GridViewRow row, bool IsRetrib, bool isContrEsteri)
        {
            //recupero la gestione dal viewstate
            bool isStandard = (bool)ViewState["TipoGestione"];

            DropDownList ddlGestione = new DropDownList();
            ddlGestione = (DropDownList)row.FindControl("ddlCodiceGestione");
            if (IsRetrib)
            {
                ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState["listaCodeGestioneCalcoloRetrib"];

                List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib_app = listaCodeGestioneCalcoloRetrib.ToList();
                listaCodeGestioneCalcoloRetrib_app.Sort((x, y) => string.Compare(x.TraduzioneSuGP.Trim(), y.TraduzioneSuGP.Trim(), false, System.Globalization.CultureInfo.InvariantCulture));
                listaCodeGestioneCalcoloRetrib = listaCodeGestioneCalcoloRetrib_app.ToArray();

                if (isStandard)
                {
                    foreach (DecodificaGestioneCalcoloRetributivo datiCodeGestioneCalcoloRetrib in listaCodeGestioneCalcoloRetrib)
                    {
                        if (datiCodeGestioneCalcoloRetrib.Id == 1)
                        {
                            ListItem li = new ListItem();
                            li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                            li.Text = datiCodeGestioneCalcoloRetrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloRetrib.Descrizione;
                            li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                            ddlGestione.Items.Add(li);
                        }
                    }
                    ddlGestione.SelectedIndex = 1;
                }
                else
                {
                    foreach (DecodificaGestioneCalcoloRetributivo datiCodeGestioneCalcoloRetrib in listaCodeGestioneCalcoloRetrib)
                    {
                        ListItem li = new ListItem();
                        li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                        li.Text = datiCodeGestioneCalcoloRetrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloRetrib.Descrizione;
                        li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                        ddlGestione.Items.Add(li);
                    }


                    if (((DatiRetributivi)(row.DataItem)).Gestione.Trim() == string.Empty)
                        ddlGestione.SelectedIndex = 0;
                    else
                        ddlGestione.Items.FindByValue(((DatiRetributivi)(row.DataItem)).Gestione.Trim()).Selected = true;

                }

                DropDownList ddlQuota = new DropDownList();
                ddlQuota = (DropDownList)row.FindControl("ddlQuota");
                ddlQuota.SelectedValue = ((DatiRetributivi)(row.DataItem)).Quota;
            }
            else
            {
                ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"];
                DecodificaCodeGestione[] listaCodeGestione = (DecodificaCodeGestione[])ViewState["listaCodeGestione"];

                List<DecodificaGestioneCalcoloContributivo> listaCodeGestioneContr_app = listaCodeGestioneCalcoloContrib.ToList();
                listaCodeGestioneContr_app.Sort((x, y) => string.Compare(x.TraduzioneSuGP.Trim(), y.TraduzioneSuGP.Trim(), false, System.Globalization.CultureInfo.InvariantCulture));
                listaCodeGestioneCalcoloContrib = listaCodeGestioneContr_app.ToArray();

                List<DecodificaCodeGestione> listaCodeGestione_app = listaCodeGestione.ToList();
                listaCodeGestione_app.Sort((x, y) => string.Compare(x.TraduzioneSuGP.ToString().Trim(), y.TraduzioneSuGP.ToString().Trim(), false, System.Globalization.CultureInfo.InvariantCulture));
                listaCodeGestione = listaCodeGestione_app.ToArray();


                if (isContrEsteri)
                {
                    List<DatiRetributivi> listDatiRetrbutivi = ((List<DatiRetributivi>)ViewState["elencoDatiRetributivi"]);
                    List<DatiContributivi> listDatiContributivi = ((List<DatiContributivi>)(ViewState["elencoDatiContributivi"]));

                    foreach (DecodificaCodeGestione datiCodeGestione in listaCodeGestione)
                    {
                        ListItem li = new ListItem();
                        li.Attributes.Add("title", datiCodeGestione.Descrizione + " - Legge " + datiCodeGestione.Legge);
                        switch (datiCodeGestione.Legge)
                        {
                            case "335":
                                if (listDatiContributivi.Find(x => (x.Gestione.Equals((datiCodeGestione.TraduzioneSuGP).ToString()) || x.Gestione.Equals((datiCodeGestione.TraduzioneSuGP).ToString() + "H")) && (x.Quota == "C" || x.Quota == "D")) == null)
                                    continue;
                                li.Text = datiCodeGestione.TraduzioneSuGP.ToString() + " - " + datiCodeGestione.Descrizione + " – Quote C/D";
                                break;
                            case "503":
                                if (listDatiRetrbutivi.Find(x => (x.Gestione.Equals((datiCodeGestione.TraduzioneSuGP - 60).ToString()) || x.Gestione.Equals((datiCodeGestione.TraduzioneSuGP - 60).ToString() + "H")) && x.Quota == "B") == null)
                                    continue;
                                li.Text = datiCodeGestione.TraduzioneSuGP.ToString() + " - " + datiCodeGestione.Descrizione + " – Quota B";
                                break;
                            case "233":
                                if (listDatiRetrbutivi.Find(x => (x.Gestione.Equals((datiCodeGestione.TraduzioneSuGP - 70).ToString()) || x.Gestione.Equals((datiCodeGestione.TraduzioneSuGP - 70).ToString() + "H")) && x.Quota == "A") == null)
                                    continue;
                                li.Text = datiCodeGestione.TraduzioneSuGP.ToString() + " - " + datiCodeGestione.Descrizione + " – Quota A";
                                break;
                        }

                        li.Value = datiCodeGestione.Id.ToString();
                        ddlGestione.Items.Add(li);
                    }

                    if (((DatiContributiviEsteri)(row.DataItem)).Gestione.Trim() == string.Empty || ddlGestione.Items.FindByValue(((DatiContributiviEsteri)(row.DataItem)).Gestione.Trim()) == null)
                        ddlGestione.SelectedIndex = 0;
                    else
                        ddlGestione.Items.FindByValue(((DatiContributiviEsteri)(row.DataItem)).Gestione.Trim()).Selected = true;

                    if (ddlGestione.Items.Count > 1)
                        ddlGestione.Enabled = true;
                }
                else
                {
                    if (isStandard)
                    {
                        foreach (DecodificaGestioneCalcoloContributivo datiCodeGestioneCalcoloContrib in listaCodeGestioneCalcoloContrib)
                        {
                            if (datiCodeGestioneCalcoloContrib.Id == 1)
                            {
                                ListItem li = new ListItem();
                                li.Attributes.Add("title", datiCodeGestioneCalcoloContrib.Descrizione);
                                li.Text = datiCodeGestioneCalcoloContrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloContrib.Descrizione;
                                li.Value = datiCodeGestioneCalcoloContrib.Id.ToString();
                                ddlGestione.Items.Add(li);
                            }
                        }
                        ddlGestione.SelectedIndex = 1;
                    }
                    else
                    {
                        foreach (DecodificaGestioneCalcoloContributivo datiCodeGestioneCalcoloContrib in listaCodeGestioneCalcoloContrib)
                        {
                            ListItem li = new ListItem();
                            li.Attributes.Add("title", datiCodeGestioneCalcoloContrib.Descrizione);
                            li.Text = datiCodeGestioneCalcoloContrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloContrib.Descrizione;
                            li.Value = datiCodeGestioneCalcoloContrib.Id.ToString();
                            ddlGestione.Items.Add(li);
                        }

                        if (((DatiContributivi)(row.DataItem)).Gestione.Trim() == string.Empty)
                            ddlGestione.SelectedIndex = 0;
                        else
                            ddlGestione.Items.FindByValue(((DatiContributivi)(row.DataItem)).Gestione.Trim()).Selected = true;
                    }

                    DropDownList ddlQuota = new DropDownList();
                    ddlQuota = (DropDownList)row.FindControl("ddlQuota");
                    ddlQuota.SelectedValue = ((DatiContributivi)(row.DataItem)).Quota;

                    //TODO WILLIAM: verificare se inserire questo filtro come controllo (rimozione tipo calcolo per CI)
                    //Dati contributivi: se il sistema di calcolo è retributivo, il codice quota C  non deve essere presente nel menu di scelta
                    //if ((((AreaDatiContributivi)ViewState["DatiContributiviCi"]).DatiCalcolo.TipoCalcolo) == GestioneContribTipoCalcolo.Retributivo)
                    //    ddlQuota.Items.Remove(ddlQuota.Items.FindByValue("C"));
                }
            }
        }

        //in base al booleano ricevuto in input, andiamo a valorizzare solamente le textbox che verranno visualizzate nella pagina
        private void ValorizzaEtichette(GestioneContribDatiCalcolo datiCalcolo)
        {
            bool isNormale = (bool)ViewState["TipoGestione"];

            DateTime? dataDecorrenza = (DateTime?)((AreaTitolare.DatiPensione)Session["DatiPensione"]).DecorrenzaOriginaria;

            ViewState["DatiContributiviCi"] = this.areaDatiContributiviCi;
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024 = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null)
                controlloDinamicoMemo123_2024 = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out controlloDinamicoMemo123_2024);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024"] = controlloDinamicoMemo123_2024;
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024OpzioneContrib = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null)
                controlloDinamicoMemo123_2024OpzioneContrib = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out controlloDinamicoMemo123_2024OpzioneContrib);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] = controlloDinamicoMemo123_2024OpzioneContrib;
            }

            if (datiCalcolo == null)
                ValorizzaEtichetteEmpty();
            else
            {
                #region GridView Dati Contributivi
                List<DatiContributivi> elencoDatiContributivi = new List<DatiContributivi>();
                if (datiCalcolo != null)
                {
                    if (datiCalcolo.MontanteInvalidita != null)
                        txtCMSM.Text = datiCalcolo.MontanteInvalidita.ToString();
                    else
                        txtCMSM.Text = string.Empty;

                    if (datiCalcolo.LDatiContributivi != null)
                    {
                        foreach (GestioneAggiornamentoPECODatiContributivi dati in datiCalcolo.LDatiContributivi)
                        {
                            if (dati.Quota.HasValue && dati.Quota.Value.ToString() == "C")
                                elencoDatiContributivi.Add(new DatiContributivi(dati.CodiceGestione.GetValueOrDefault().ToString(), dati.Quota.GetValueOrDefault().ToString(), dati.Nsettimane.GetValueOrDefault().ToString(),
                                    dati.ImportoContributivoTotale.GetValueOrDefault().ToString(System.Globalization.CultureInfo.CurrentUICulture), dati.MontanteContributivo.GetValueOrDefault().ToString(System.Globalization.CultureInfo.CurrentUICulture)));

                            if (dati.Quota.HasValue && dati.Quota.Value.ToString() == "D")
                                elencoDatiContributivi.Add(new DatiContributivi(dati.CodiceGestione.GetValueOrDefault().ToString(), dati.Quota.GetValueOrDefault().ToString(), dati.SettimaneQuotaD.GetValueOrDefault().ToString(),
                                    dati.ImportoContributivoQuotaD.GetValueOrDefault().ToString(System.Globalization.CultureInfo.CurrentUICulture), dati.MontanteContributivoQuotaD.GetValueOrDefault().ToString(System.Globalization.CultureInfo.CurrentUICulture)));
                        }

                        //for (int i = 0; i < datiCalcolo.LDatiContributivi.Length; i++)
                        //{
                        //        elencoDatiContributivi.Add(new DatiContributivi(datiCalcolo.LDatiContributivi[i].CodiceGestione.ToString(),
                        //        datiCalcolo.LDatiContributivi[i].Quota.ToString(),
                        //        datiCalcolo.LDatiContributivi[i].Nsettimane.ToString(),
                        //        datiCalcolo.LDatiContributivi[i].ImportoContributivoTotale.ToString(),
                        //        datiCalcolo.LDatiContributivi[i].MontanteContributivo.ToString()));
                        //}
                    }
                }

                ViewState["elencoDatiContributivi"] = elencoDatiContributivi;

                #endregion GridView Dati Contributivi

                #region GridView Dati Retributivi

                List<DatiRetributivi> elencoDatiRetributivi = new List<DatiRetributivi>();
                if (datiCalcolo != null)
                {
                    if (datiCalcolo.LDatiRetributivi != null)
                    {
                        DateTime? decorrenza = new DateTime?();
                        string decorrenzaOriginariaPensione = string.Empty;

                        for (int i = 0; i < datiCalcolo.LDatiRetributivi.Length; i++)
                        {
                            decorrenza = datiCalcolo.LDatiRetributivi[i].DecorrenzaOriginariaPensione;
                            if (decorrenza.HasValue)
                                decorrenzaOriginariaPensione = decorrenza.Value.ToString("MM/yyyy");
                            else
                                decorrenzaOriginariaPensione = string.Empty;

                            if (datiCalcolo.LDatiRetributivi[i].QuotePrimeLiquidate.ToString() == "A")
                            {
                                elencoDatiRetributivi.Add(new DatiRetributivi(datiCalcolo.LDatiRetributivi[i].CodiceGestione.ToString(),
                                    datiCalcolo.LDatiRetributivi[i].QuotePrimeLiquidate.ToString(),
                                    decorrenzaOriginariaPensione,
                                    datiCalcolo.LDatiRetributivi[i].NSettimaneQuotaA.ToString(),
                                    datiCalcolo.LDatiRetributivi[i].RMSQuotaA.ToString(),
                                    datiCalcolo.LDatiRetributivi[i].Nsettimane707.ToString()));
                            }

                            if (datiCalcolo.LDatiRetributivi[i].QuotePrimeLiquidate.ToString() == "B")
                            {
                                elencoDatiRetributivi.Add(new DatiRetributivi(datiCalcolo.LDatiRetributivi[i].CodiceGestione.ToString(),
                                    datiCalcolo.LDatiRetributivi[i].QuotePrimeLiquidate.ToString(),
                                    decorrenzaOriginariaPensione,
                                    datiCalcolo.LDatiRetributivi[i].NSettimaneQuotaB.ToString(),
                                    datiCalcolo.LDatiRetributivi[i].RMSQuotaB.ToString(),
                                    datiCalcolo.LDatiRetributivi[i].Nsettimane707.ToString()));
                            }
                        }
                    }
                }
                if (CodeUtility.IsContributivaPura(datiPensione) || this.areaDatiContributiviCi.IsPensioneTipoContributivo) 
                    gvDatiRetributivi.Enabled = false;
                //opzione donna e sperimentale donna
                if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaOpzioneDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaCalcoloContributivoSperimentaleLavoratriciOrRicostrituzione(datiPensione))
                    gvDatiRetributivi.Enabled = false;
                ViewState["elencoDatiRetributivi"] = elencoDatiRetributivi;


                #endregion GridView Dati Retributivi

                this.areaDatiContributiviCi.DatiCalcolo = datiCalcolo;

                gvDatiContributivi_Load();
                gvDatiRetributivi_Load();

                #region GridView Dati Contributivi Esteri

                if (datiCalcolo != null)
                {
                    if (datiCalcolo.ContributiItalianiEdEsteriAl1295 != null)
                        txtContributiItalianiEsteri.Text = datiCalcolo.ContributiItalianiEdEsteriAl1295.ToString();
                    else
                        txtContributiItalianiEsteri.Text = string.Empty;

                    List<DatiContributiviEsteri> elencoDatiContributiviEsteri = new List<DatiContributiviEsteri>();
                    if (!isNormale || !elencoDatiRetributivi.Exists(x => x.Quota == "A"))
                    {
                        gvDatiContributiviEsteri.Visible = true;
                        if (datiCalcolo.LDatiContributiEsteri != null)
                        {
                            DateTime dec = new DateTime();
                            string decorrenza = string.Empty;

                            for (int i = 0; i < datiCalcolo.LDatiContributiEsteri.Length; i++)
                            {
                                dec = (DateTime)datiCalcolo.LDatiContributiEsteri[i].Decorrenza;
                                decorrenza = dec.ToString("MM/yyyy");

                                elencoDatiContributiviEsteri.Add(new DatiContributiviEsteri(datiCalcolo.LDatiContributiEsteri[i].CodiceGestione.ToString(),
                                    decorrenza,
                                    datiCalcolo.LDatiContributiEsteri[i].Settimane.ToString()));
                            }
                        }

                        ViewState["elencoDatiContributiviEsteri"] = elencoDatiContributiviEsteri;
                        gvDatiContributiviEsteri_Load();
                    }
                }
                else if (!isNormale)
                {
                    // bind grid edit
                    gvDatiContributiviEsteri.Visible = true;
                    List<DatiContributiviEsteri> elencoDatiContributiviEsteri = new List<DatiContributiviEsteri>();
                    ViewState["elencoDatiContributiviEsteri"] = elencoDatiContributiviEsteri;
                    gvDatiContributiviEsteri_Load();
                }

                if (CodeUtility.IsContributivaPura(datiPensione) || this.areaDatiContributiviCi.IsPensioneTipoContributivo)
                {
                    txtContributiItalianiEsteri.Text = "0";
                    txtContributiItalianiEsteri.Enabled = false;
                }
                #endregion GridView Dati Contributivi Esteri

                #region Gestione Comune 2

                if (datiCalcolo.CTRMaternitaAcna.HasValue)
                    ckbCTRMaternitaExAcna.Checked = (bool)datiCalcolo.CTRMaternitaAcna;

                #endregion Gestione Comune 2
            }
            if (datiPensione.CodeGruppo == "0001" && this.areaDatiContributiviCi.IsFineAssicurazionePost2012 && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
            {
                lblQuotaD.Visible = true;
            }

            if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                pnlGridViewDatiRetributivi.Visible = false;
        }

        private void ValorizzaEtichetteEmpty()
        {
            bool isStandard = (bool)ViewState["TipoGestione"];
            txtCMSM.Text = string.Empty;
            txtContributiItalianiEsteri.Text = string.Empty;
            ckbCTRMaternitaExAcna.Checked = false;

            gvDatiRetributivi_Load();
            gvDatiContributivi_Load();
            List<DatiRetributivi> elencoDatiRetributivi = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
            if (!isStandard || !elencoDatiRetributivi.Exists(x => x.Quota == "A"))
                gvDatiContributiviEsteri_Load();
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                txtContributiItalianiEsteri.Enabled = false;
            }
        }

        private GestioneContribDatiCalcolo GetDatiCalcoloTab()
        {
            if (ViewState["DatiContributiviCi"] != null)
                this.areaDatiContributiviCi = (AreaDatiContributivi)ViewState["DatiContributiviCi"];
            else
            {
                this.areaDatiContributiviCi = new AreaDatiContributivi();
                this.areaDatiContributiviCi.DatiCalcolo = new GestioneContribDatiCalcolo();
            }

            #region Dati TextBox

            this.areaDatiContributiviCi.DatiCalcolo.CTRMaternitaAcna = ckbCTRMaternitaExAcna.Checked;

            #endregion Dati TextBox

            #region Dati Contributivi

            List<DatiContributivi> elencoDatiContributivi = ((List<DatiContributivi>)(ViewState["elencoDatiContributivi"]));
            removeItemBlankDatiContributivi(ref elencoDatiContributivi);

            this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi = new GestioneAggiornamentoPECODatiContributivi[elencoDatiContributivi.Count];
            List<GestioneAggiornamentoPECODatiContributivi> listDatiContributivi = new List<GestioneAggiornamentoPECODatiContributivi>();
            foreach (DatiContributivi datiContributivi in elencoDatiContributivi)
            {
                GestioneAggiornamentoPECODatiContributivi dati = new GestioneAggiornamentoPECODatiContributivi();
                if (!string.IsNullOrEmpty(datiContributivi.Gestione))
                    dati.CodiceGestione = Convert.ToInt64(datiContributivi.Gestione);
                else dati.CodiceGestione = null;

                if (!string.IsNullOrEmpty(datiContributivi.Quota))
                    dati.Quota = char.Parse(datiContributivi.Quota);
                else dati.Quota = null;

                if (dati.Quota.HasValue && dati.Quota == 'C')
                {
                    if (!string.IsNullOrEmpty(datiContributivi.Settimane))
                        dati.Nsettimane = int.Parse(datiContributivi.Settimane);
                    else dati.Nsettimane = null;

                    if (!string.IsNullOrEmpty(datiContributivi.AmmontareContributivo))
                        dati.ImportoContributivoTotale = decimal.Parse(datiContributivi.AmmontareContributivo);
                    else dati.ImportoContributivoTotale = null;

                    if (!string.IsNullOrEmpty(datiContributivi.MontanteContributivo))
                        dati.MontanteContributivo = decimal.Parse(datiContributivi.MontanteContributivo);
                    else dati.MontanteContributivo = null;
                }
                else if (dati.Quota.HasValue && dati.Quota == 'D')
                {
                    if (!string.IsNullOrEmpty(datiContributivi.Settimane))
                        dati.SettimaneQuotaD = int.Parse(datiContributivi.Settimane);
                    else dati.SettimaneQuotaD = null;

                    if (!string.IsNullOrEmpty(datiContributivi.AmmontareContributivo))
                        dati.ImportoContributivoQuotaD = decimal.Parse(datiContributivi.AmmontareContributivo);
                    else dati.ImportoContributivoQuotaD = null;

                    if (!string.IsNullOrEmpty(datiContributivi.MontanteContributivo))
                        dati.MontanteContributivoQuotaD = decimal.Parse(datiContributivi.MontanteContributivo);
                    else dati.MontanteContributivoQuotaD = null;
                }

                //this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi.ToList().Add(dati);
                listDatiContributivi.Add(dati);
            }
            this.areaDatiContributiviCi.DatiCalcolo.LDatiContributivi = listDatiContributivi.ToArray();

            #endregion Dati Contributivi

            #region Dati Retributivi

            List<DatiRetributivi> elencoDatiRetributivi = ((List<DatiRetributivi>)(ViewState["elencoDatiRetributivi"]));
            removeItemBlankDatiRetributivi(ref elencoDatiRetributivi);
            this.areaDatiContributiviCi.DatiCalcolo.LDatiRetributivi = new GestioneAggiornamentoPECODatiRetributivi[elencoDatiRetributivi.Count];
            List<GestioneAggiornamentoPECODatiRetributivi> listDatiRetributivi = new List<GestioneAggiornamentoPECODatiRetributivi>();
            foreach (DatiRetributivi datiRetributivi in elencoDatiRetributivi)
            {
                GestioneAggiornamentoPECODatiRetributivi dati = new GestioneAggiornamentoPECODatiRetributivi();

                if (!string.IsNullOrEmpty(datiRetributivi.Gestione))
                    dati.CodiceGestione = Convert.ToInt64(datiRetributivi.Gestione);
                else dati.CodiceGestione = null;

                if (!string.IsNullOrEmpty(datiRetributivi.Quota))
                    dati.QuotePrimeLiquidate = char.Parse(datiRetributivi.Quota);
                else dati.QuotePrimeLiquidate = null;

                if (datiRetributivi.Quota.Trim() == "A")
                {
                    if (!string.IsNullOrEmpty(datiRetributivi.Settimane))
                    {
                        dati.NSettimaneQuotaA = int.Parse(datiRetributivi.Settimane);
                        dati.NSettimaneQuotaB = null;
                    }
                    else
                    {
                        dati.NSettimaneQuotaA = null;
                        dati.NSettimaneQuotaB = null;
                    }

                    if (!string.IsNullOrEmpty(datiRetributivi.RedditoRetribuzioneMedia))
                    {
                        dati.RMSQuotaA = decimal.Parse(datiRetributivi.RedditoRetribuzioneMedia);
                        dati.RMSQuotaB = null;
                    }
                    else
                    {
                        dati.RMSQuotaA = null;
                        dati.RMSQuotaB = null;
                    }
                }
                if (datiRetributivi.Quota.Trim() == "B")
                {
                    if (!string.IsNullOrEmpty(datiRetributivi.Settimane))
                    {
                        dati.NSettimaneQuotaB = int.Parse(datiRetributivi.Settimane);
                        dati.NSettimaneQuotaA = null;
                    }
                    else
                    {
                        dati.NSettimaneQuotaB = null;
                        dati.NSettimaneQuotaA = null;
                    }

                    if (!string.IsNullOrEmpty(datiRetributivi.RedditoRetribuzioneMedia))
                    {
                        dati.RMSQuotaB = decimal.Parse(datiRetributivi.RedditoRetribuzioneMedia);
                        dati.RMSQuotaA = null;
                    }
                    else
                    {
                        dati.RMSQuotaB = null;
                        dati.RMSQuotaA = null;
                    }
                }

                if (!string.IsNullOrEmpty(datiRetributivi.Settimane707))
                    dati.Nsettimane707 = int.Parse(datiRetributivi.Settimane707);

                if (!string.IsNullOrEmpty(datiRetributivi.Decorrenza))
                    dati.DecorrenzaOriginariaPensione = Utility.GetDateFromString(datiRetributivi.Decorrenza);
                else dati.DecorrenzaOriginariaPensione = null;

                listDatiRetributivi.Add(dati);
            }
            this.areaDatiContributiviCi.DatiCalcolo.LDatiRetributivi = listDatiRetributivi.ToArray();

            if (!string.IsNullOrEmpty(txtCMSM.Text))
                this.areaDatiContributiviCi.DatiCalcolo.MontanteInvalidita = decimal.Parse(txtCMSM.Text);
            else
                this.areaDatiContributiviCi.DatiCalcolo.MontanteInvalidita = null;
         

                #endregion Dati Retributivi

                #region Dati Contributivi Esteri

                if (!string.IsNullOrEmpty(txtContributiItalianiEsteri.Text))
                this.areaDatiContributiviCi.DatiCalcolo.ContributiItalianiEdEsteriAl1295 = int.Parse(txtContributiItalianiEsteri.Text);
            else this.areaDatiContributiviCi.DatiCalcolo.ContributiItalianiEdEsteriAl1295 = null;

            List<DatiContributiviEsteri> elencoDatiContributiviEsteri = ((List<DatiContributiviEsteri>)(ViewState["elencoDatiContributiviEsteri"]));
            List<GestioneAggiornamentoPECODatiContributiEsteri> listDatiContributiviEsteri = new List<GestioneAggiornamentoPECODatiContributiEsteri>();
            if (elencoDatiContributiviEsteri != null && elencoDatiContributiviEsteri.Count > 0)
            {
                removeItemBlankDatiContributiviEsteri(ref elencoDatiContributiviEsteri);

                this.areaDatiContributiviCi.DatiCalcolo.LDatiContributiEsteri = new GestioneAggiornamentoPECODatiContributiEsteri[elencoDatiContributiviEsteri.Count];
                foreach (DatiContributiviEsteri datiContributiviEsteri in elencoDatiContributiviEsteri)
                {
                    GestioneAggiornamentoPECODatiContributiEsteri dati = new GestioneAggiornamentoPECODatiContributiEsteri();
                    if (!string.IsNullOrEmpty(datiContributiviEsteri.Gestione))
                        dati.CodiceGestione = long.Parse(datiContributiviEsteri.Gestione);
                    else dati.CodiceGestione = null;

                    if (!string.IsNullOrEmpty(datiContributiviEsteri.Settimane))
                        dati.Settimane = int.Parse(datiContributiviEsteri.Settimane);
                    else dati.Settimane = null;

                    if (!string.IsNullOrEmpty(datiContributiviEsteri.Decorrenza))
                        dati.Decorrenza = Utility.GetDateFromString(datiContributiviEsteri.Decorrenza);
                    else dati.Decorrenza = null;

                    listDatiContributiviEsteri.Add(dati);
                }
            }
            this.areaDatiContributiviCi.DatiCalcolo.LDatiContributiEsteri = listDatiContributiviEsteri.ToArray();
            #endregion Dati Contributivi Esteri

            return this.areaDatiContributiviCi.DatiCalcolo;
        }

        private void LoadDecodificaData(AreaDatiContributivi area)
        {
            //CodeUtility valuesDecodifica = new CodeUtility();
            //AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();

            //AreaDecodifica.DatiCodeGestioneCalcoloRetrib[] listaCodeGestioneCalcoloRetrib = null;
            //listaCodeGestioneCalcoloRetrib =   valoriDecodificati.ElencoCodeGestioneCalcoloRetrib;
            ViewState["listaCodeGestioneCalcoloRetrib"] = area.ListaDecodificaGestioneCalcoloRetributivo;

            //AreaDecodifica.DatiCodeGestioneCalcoloContrib[] listaCodeGestioneCalcoloContrib = null;
            //listaCodeGestioneCalcoloContrib = valoriDecodificati.ElencoCodeGestioneCalcoloContrib;
            ViewState["listaCodeGestioneCalcoloContrib"] = area.ListaDecodificaGestioneCalcoloContributivo;

            //AreaDecodifica.CodeGestione[] listaCodeGestione = null;
            //listaCodeGestione = valoriDecodificati.ElencoCodiceGestione;
            ViewState["listaCodeGestione"] = area.ListaDecodificaCodeGestione;
        }

        private void removeItemBlankDatiContributivi(ref List<DatiContributivi> lista)
        {

            int index = lista.FindIndex(delegate(DatiContributivi code)
            {
                return (code.Gestione == string.Empty && code.Quota == string.Empty && code.Settimane == string.Empty && code.MontanteContributivo == string.Empty && code.AmmontareContributivo == string.Empty);
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private void removeItemBlankDatiRetributivi(ref List<DatiRetributivi> lista)
        {

            int index = lista.FindIndex(delegate(DatiRetributivi code)
            {
                return (code.Gestione == string.Empty && code.Settimane == string.Empty && code.RedditoRetribuzioneMedia == string.Empty && code.Decorrenza == string.Empty);
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private void removeItemBlankDatiContributiviEsteri(ref List<DatiContributiviEsteri> lista)
        {

            int index = lista.FindIndex(delegate(DatiContributiviEsteri code)
            {
                return (code.Gestione == string.Empty && code.Settimane == string.Empty && code.Decorrenza == string.Empty);
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
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

        private void gvDatiContributivi_Load()
        {
            List<DatiContributivi> elencoDatiContributivi = /*new List<DatiContributivi>(); //*/(List<DatiContributivi>)ViewState["elencoDatiContributivi"];

            //if (((AreaDatiContributivi)ViewState["DatiContributiviCi"]).DatiCalcolo.LDatiContributivi != null)
            //    elencoDatiContributivi = MapDatiContributiviForView((AreaDatiContributivi)ViewState["DatiContributiviCi"]);

            DatiContributivi Empty = elencoDatiContributivi.Find(delegate(DatiContributivi code)
            {
                return (code.Gestione == string.Empty && code.Quota == string.Empty && code.Settimane == string.Empty && code.AmmontareContributivo == string.Empty && code.MontanteContributivo == string.Empty);
            }
            );

            if (Empty == null)
            {
                elencoDatiContributivi.Add(new DatiContributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }

            gvDatiContributivi.DataSource = elencoDatiContributivi;
            //ViewState["elencoDatiContributivi"] = elencoDatiContributivi;
            gvDatiContributivi.DataBind();

            SetBtnShowPopUpContrib(GetDatiPensione(this), elencoDatiContributivi);
        }

        private void gvDatiRetributivi_Load()
        {
            List<DatiRetributivi> elencoDatiRetributivi = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];

            DatiRetributivi Empty = elencoDatiRetributivi.Find(delegate(DatiRetributivi code)
            {
                return (code.Gestione == string.Empty && code.Quota == string.Empty && code.Decorrenza == string.Empty && code.Settimane == string.Empty && code.RedditoRetribuzioneMedia == string.Empty);
            }
            );

            if (Empty == null)
            {
                elencoDatiRetributivi.Add(new DatiRetributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }

            gvDatiRetributivi.DataSource = elencoDatiRetributivi;
            gvDatiRetributivi.DataBind();
        }

        private void gvDatiContributiviEsteri_Load()
        {
            List<DatiContributiviEsteri> elencoDatiContributiviEsteri = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];

            DatiContributiviEsteri Empty = elencoDatiContributiviEsteri.Find(delegate(DatiContributiviEsteri code)
            {
                return (code.Gestione == string.Empty && code.Decorrenza == string.Empty && code.Settimane == string.Empty);
            }
            );

            if (Empty == null && elencoDatiContributiviEsteri.Count == 0)
            {
                elencoDatiContributiviEsteri.Add(new DatiContributiviEsteri(string.Empty, string.Empty, string.Empty));
            }

            gvDatiContributiviEsteri.DataSource = elencoDatiContributiviEsteri;
            gvDatiContributiviEsteri.DataBind();
        }

        private void EnableEditableModeContr(TableCell cell_CancelSave)
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
            save.ValidationGroup = "UCTabDatiCalcoloContrCI";

        }

        private void EnableReadableModeContr(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteContributivi")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private void EnableEditableModeRetr(TableCell cell_CancelSave)
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
            save.ValidationGroup = "UCTabDatiCalcoloRetrCI";

        }

        private void EnableReadableModeRetr(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteRetributivi")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private void EnableEditableModeContrEsteri(TableCell cell_CancelSave)
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
            save.ValidationGroup = "UCTabDatiCalcoloContrEsteriCI";

        }

        private void EnableReadableModeContrEsteri(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteContributiEsteri")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private bool IsListaEmpty(bool IsRetr, bool isContrEsteri)
        {
            if (IsRetr)
            {
                List<DatiRetributivi> listaDatiRetrApp = (List<DatiRetributivi>)ViewState["elencoDatiRetributivi"];
                if (listaDatiRetrApp.Count == 1 && listaDatiRetrApp[0].Decorrenza == string.Empty &&
                    listaDatiRetrApp[0].Gestione == string.Empty && listaDatiRetrApp[0].Quota == string.Empty &&
                    listaDatiRetrApp[0].RedditoRetribuzioneMedia == string.Empty && listaDatiRetrApp[0].Settimane == string.Empty)
                    return true;
                else
                    return false;
            }
            else
            {
                if (isContrEsteri)
                {
                    List<DatiContributiviEsteri> listaDatiContrEsteriApp = (List<DatiContributiviEsteri>)ViewState["elencoDatiContributiviEsteri"];
                    if (listaDatiContrEsteriApp.Count == 1 && listaDatiContrEsteriApp[0].Gestione == string.Empty &&
                        listaDatiContrEsteriApp[0].Decorrenza == string.Empty && listaDatiContrEsteriApp[0].Settimane == string.Empty)
                        return true;
                    else
                        return false;
                }
                else
                {
                    List<DatiContributivi> listaDatiContrApp = (List<DatiContributivi>)ViewState["elencoDatiContributivi"];
                    if (listaDatiContrApp.Count == 1 && listaDatiContrApp[0].AmmontareContributivo == string.Empty &&
                        listaDatiContrApp[0].Gestione == string.Empty && listaDatiContrApp[0].Quota == string.Empty && listaDatiContrApp[0].MontanteContributivo == string.Empty &&
                        listaDatiContrApp[0].Settimane == string.Empty)
                        return true;
                    else
                        return false;
                }
            }
        }

        private bool IsEmptyEditableRowContr(GridViewRow row)
        {
            if ((row.FindControl("txtAmmontareContributivo") != null && ((TextBox)row.FindControl("txtAmmontareContributivo")).Text != string.Empty) ||
                (row.FindControl("txtMontanteContributivo") != null && ((TextBox)row.FindControl("txtMontanteContributivo")).Text != string.Empty) ||
                row.FindControl("txtSettimaneContributive") != null && ((TextBox)row.FindControl("txtSettimaneContributive")).Text != string.Empty ||
                (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0) ||
                (row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0))
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("txtRetribuzioneMedia") != null && ((TextBox)row.FindControl("txtRetribuzioneMedia")).Text != string.Empty) ||
                (row.FindControl("txtSettimaneRetributive") != null && ((TextBox)row.FindControl("txtSettimaneRetributive")).Text != string.Empty) ||
                //(row.FindControl("txtDecorrenza") != null && ((TextBox)row.FindControl("txtDecorrenza")).Text != string.Empty) ||
                (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0) ||
                (row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0) ||
                (row.FindControl("txtSettimaneRetributive707CI") != null && ((TextBox)row.FindControl("txtSettimaneRetributive707CI")).Text != string.Empty))

                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowContrEsteri(GridViewRow row)
        {
            if ((row.FindControl("txtSettimane") != null && ((TextBox)row.FindControl("txtSettimane")).Text != string.Empty) ||
               (row.FindControl("txtDecorrenza") != null && ((TextBox)row.FindControl("txtDecorrenza")).Text != string.Empty) ||
               (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0))

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("lblCodiceGestione_item") != null && ((Label)row.FindControl("lblCodiceGestione_item")).Text != string.Empty) ||
                (row.FindControl("lblQuota_item") != null && ((Label)row.FindControl("lblQuota_item")).Text != string.Empty) ||
                (row.FindControl("lblDecorrenza") != null && ((Label)row.FindControl("lblDecorrenza")).Text != string.Empty) ||
                (row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty) ||
                (row.FindControl("lblRetribuzioneMedia") != null && ((Label)row.FindControl("lblRetribuzioneMedia")).Text != string.Empty) ||
                (row.FindControl("lblSettimane707CI") != null && ((Label)row.FindControl("lblSettimane707CI")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowContrEsteri(GridViewRow row)
        {
            if (row.FindControl("lblCodiceGestione_item") != null && ((Label)row.FindControl("lblCodiceGestione_item")).Text != string.Empty &&
                row.FindControl("lblDecorrenza") != null && ((Label)row.FindControl("lblDecorrenza")).Text != string.Empty &&
                row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowContr(GridViewRow row)
        {
            if (row.FindControl("lblCodiceGestione_item") != null && ((Label)row.FindControl("lblCodiceGestione_item")).Text != string.Empty &&
                row.FindControl("lblQuota_item") != null && ((Label)row.FindControl("lblQuota_item")).Text != string.Empty &&
                row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty &&
                row.FindControl("lblAmmontareContributivo") != null && ((Label)row.FindControl("lblAmmontareContributivo")).Text != string.Empty &&
                row.FindControl("lblMontanteContributivo") != null && ((Label)row.FindControl("lblMontanteContributivo")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private string GetValueFromIdRetr(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState["listaCodeGestioneCalcoloRetrib"];
                DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate(DecodificaGestioneCalcoloRetributivo code)
                { return (code.Id == index); });
                return app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            else
                return string.Empty;


        }

        private string GetValueFromIdContr(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"];
                DecodificaGestioneCalcoloContributivo app = listaCodeGestioneCalcoloContrib.ToList().Find(delegate(DecodificaGestioneCalcoloContributivo code)
                { return (code.Id == index); });
                return app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            else
                return string.Empty;
        }

        private string GetValueFromIdContrEsteri(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                string codGestione = string.Empty;
                int index = Convert.ToInt32(id);
                DecodificaCodeGestione[] listaCodeGestioneCalcoloContribEsteri = (DecodificaCodeGestione[])ViewState["listaCodeGestione"];
                DecodificaCodeGestione app = listaCodeGestioneCalcoloContribEsteri.ToList().Find(delegate(DecodificaCodeGestione code)
                { return (code.Id == index); });

                switch (app.Legge)
                {
                    case "335":
                        codGestione = app.TraduzioneSuGP + " - " + app.Descrizione + " – Quote C/D";
                        break;
                    case "503":
                        codGestione = app.TraduzioneSuGP + " - " + app.Descrizione + " – Quota B";
                        break;
                    case "233":
                        codGestione = app.TraduzioneSuGP + " - " + app.Descrizione + " – Quota A";
                        break;
                }

                return codGestione;
            }
            else
                return string.Empty;
        }

        private List<DatiRetributivi> AddRecordRetributivi(List<DatiRetributivi> listaRecord, String gestione, String quota, String decorrenza, String settimane, String retribuzioneMedia, String settimane707Ci)
        {
            listaRecord.Add(new DatiRetributivi(gestione, quota, decorrenza, settimane, retribuzioneMedia, settimane707Ci));
            return listaRecord;
        }

        private List<DatiContributivi> AddRecordContributivi(List<DatiContributivi> listaRecord, String gestione, String quota, String settimane, String ammontareContributivo, String montanteContributivo)
        {
            listaRecord.Add(new DatiContributivi(gestione, quota, settimane, ammontareContributivo, montanteContributivo));
            return listaRecord;
        }

        private List<DatiContributiviEsteri> AddRecordContributiviEsteri(List<DatiContributiviEsteri> listaRecord, String gestione, String decorrenza, String settimane)
        {
            listaRecord.Add(new DatiContributiviEsteri(gestione, decorrenza, settimane));
            return listaRecord;
        }

        private List<GestioneAggiornamentoPECODatiContributivi> GetDataContributiviToSave(List<DatiContributivi> lDatiContributivi)
        {
            List<GestioneAggiornamentoPECODatiContributivi> lContr = new List<GestioneAggiornamentoPECODatiContributivi>();

            // eliminazione record vuoto
            int? index = lDatiContributivi.FindIndex(delegate(DatiContributivi code)
            {
                return (code.AmmontareContributivo == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty &&
                        code.MontanteContributivo == string.Empty && code.Settimane == string.Empty);
            });

            if (index > -1)
                lDatiContributivi.RemoveAt(index.Value);

            DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState["listaCodeGestioneCalcoloContrib"];

            foreach (DatiContributivi datiContributivi in lDatiContributivi)
            {
                GestioneAggiornamentoPECODatiContributivi Contr = new GestioneAggiornamentoPECODatiContributivi();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiContributivi.Gestione.Trim() != string.Empty)
                    Contr.CodiceGestione = Convert.ToInt64(datiContributivi.Gestione.Trim());
                else
                    Contr.CodiceGestione = null;

                Contr.Quota = !String.IsNullOrEmpty(datiContributivi.Quota) ? Convert.ToChar(datiContributivi.Quota) : (char?)null;

                if (Contr.Quota.HasValue && Contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                {
                    Contr.ImportoContributivoTotale = datiContributivi.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributivi.AmmontareContributivo.Trim()) : (decimal?)null;
                    Contr.MontanteContributivo = datiContributivi.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributivi.MontanteContributivo.Trim()) : (decimal?)null;
                    Contr.Nsettimane = datiContributivi.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiContributivi.Settimane.Trim()) : (int?)null;
                }
                else if (Contr.Quota.HasValue && Contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                {
                    Contr.ImportoContributivoQuotaD = datiContributivi.AmmontareContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributivi.AmmontareContributivo.Trim()) : (decimal?)null;
                    Contr.MontanteContributivoQuotaD = datiContributivi.MontanteContributivo.Trim() != string.Empty ? Convert.ToDecimal(datiContributivi.MontanteContributivo.Trim()) : (decimal?)null;
                    Contr.SettimaneQuotaD = datiContributivi.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiContributivi.Settimane.Trim()) : (int?)null;
                }

                lContr.Add(Contr);
            }
            return lContr;
        }

        private List<GestioneAggiornamentoPECODatiRetributivi> GetDataRetributiviToSave(List<DatiRetributivi> lDatiRetributivi)
        {
            List<GestioneAggiornamentoPECODatiRetributivi> lRetr = new List<GestioneAggiornamentoPECODatiRetributivi>();

            // eliminazione record vuoto
            int? index = lDatiRetributivi.FindIndex(delegate(DatiRetributivi code)
            {
                return (code.Decorrenza == string.Empty && code.Gestione == string.Empty &&
                        code.Quota == string.Empty && code.RedditoRetribuzioneMedia == string.Empty && code.Settimane == string.Empty && code.Settimane707 == string.Empty);
            });

            if (index > -1)
                lDatiRetributivi.RemoveAt(index.Value);

            DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState["listaCodeGestioneCalcoloRetrib"];

            foreach (DatiRetributivi datiRetributivi in lDatiRetributivi)
            {
                GestioneAggiornamentoPECODatiRetributivi Retr = new GestioneAggiornamentoPECODatiRetributivi();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiRetributivi.Gestione.Trim() != string.Empty)
                    Retr.CodiceGestione = Convert.ToInt64(datiRetributivi.Gestione.Trim());
                else
                    Retr.CodiceGestione = null;

                Retr.DecorrenzaOriginariaPensione = datiRetributivi.Decorrenza.Trim() != string.Empty ? Convert.ToDateTime(datiRetributivi.Decorrenza.Trim()) : (DateTime?)null;
                Retr.QuotePrimeLiquidate = datiRetributivi.Quota.Trim() != string.Empty ? Convert.ToChar(datiRetributivi.Quota.Trim()) : (char?)null;

                if (datiRetributivi.Quota != string.Empty && datiRetributivi.Quota.Trim().ToUpperInvariant() == "A")
                {
                    Retr.RMSQuotaA = datiRetributivi.RedditoRetribuzioneMedia.Trim() != string.Empty ? Convert.ToDecimal(datiRetributivi.RedditoRetribuzioneMedia.Trim()) : (decimal?)null;
                    Retr.NSettimaneQuotaA = datiRetributivi.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiRetributivi.Settimane.Trim()) : (int?)null;
                }
                else if (datiRetributivi.Quota != string.Empty && datiRetributivi.Quota.Trim().ToUpperInvariant() == "B")
                {
                    Retr.RMSQuotaB = datiRetributivi.RedditoRetribuzioneMedia.Trim() != string.Empty ? Convert.ToDecimal(datiRetributivi.RedditoRetribuzioneMedia.Trim()) : (decimal?)null;
                    Retr.NSettimaneQuotaB = datiRetributivi.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiRetributivi.Settimane.Trim()) : (int?)null;

                }

                if (!string.IsNullOrEmpty(datiRetributivi.Settimane707))
                    Retr.Nsettimane707 = int.Parse(datiRetributivi.Settimane707);

                lRetr.Add(Retr);
            }
            return lRetr;
        }

        private void ReLoadData(List<DatiRetributivi> listaDatiRetribApp, List<DatiContributivi> listaDatiContribApp)
        {
            if (listaDatiRetribApp != null)
            {
                DatiRetributivi EmptyRetr = listaDatiRetribApp.Find(delegate(DatiRetributivi code)
                {
                    return (code.Decorrenza == string.Empty && code.Gestione == string.Empty && code.RedditoRetribuzioneMedia == string.Empty &&
                            code.Settimane == string.Empty && code.Quota == string.Empty);
                });

                if (EmptyRetr == null && !(((AreaDatiContributivi)ViewState["DatiContributiviCi"]).DatiCalcolo.IsUnicarpe))
                    listaDatiRetribApp.Add(new DatiRetributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvDatiRetributivi.DataSource = listaDatiRetribApp;
                ViewState["elencoDatiRetributivi"] = listaDatiRetribApp;
                gvDatiRetributivi.DataBind();
            }

            if (listaDatiContribApp != null)
            {
                DatiContributivi EmptyContr = listaDatiContribApp.Find(delegate(DatiContributivi code)
                {
                    return (code.AmmontareContributivo == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty &&
                            code.MontanteContributivo == string.Empty && code.Settimane == string.Empty);
                });

                if (EmptyContr == null && !(((AreaDatiContributivi)ViewState["DatiContributiviCi"]).DatiCalcolo.IsUnicarpe))
                    listaDatiContribApp.Add(new DatiContributivi(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                gvDatiContributivi.DataSource = listaDatiContribApp;
                ViewState["elencoDatiContributivi"] = listaDatiContribApp;
                gvDatiContributivi.DataBind();

                SetBtnShowPopUpContrib(GetDatiPensione(this), listaDatiContribApp);


            }
        }

        private static List<DatiRetributivi> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributivi> elencoDatiRetributivi = new List<DatiRetributivi>();
            foreach (GestioneAggiornamentoPECODatiRetributivi retr in areaDatiContributivi.DatiCalcolo.LDatiRetributivi.ToList<GestioneAggiornamentoPECODatiRetributivi>())
            {
                string settimana = string.Empty;
                string rmsQuota = string.Empty;
                string decorrenza = string.Empty;

                if (retr.QuotePrimeLiquidate.HasValue)
                {
                    if (retr.QuotePrimeLiquidate.HasValue && retr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                    {
                        settimana = retr.NSettimaneQuotaA.HasValue ? retr.NSettimaneQuotaA.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaA.HasValue ? retr.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (retr.QuotePrimeLiquidate.HasValue && retr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                    {
                        settimana = retr.NSettimaneQuotaB.HasValue ? retr.NSettimaneQuotaB.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaB.HasValue ? retr.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }

                decorrenza = retr.DecorrenzaOriginariaPensione.HasValue ? String.Format("{0:MM/yyyy}", retr.DecorrenzaOriginariaPensione.Value) : string.Empty;
                elencoDatiRetributivi.Add(new DatiRetributivi(retr.CodiceGestione.HasValue ? retr.CodiceGestione.Value.ToString() : string.Empty,
                    retr.QuotePrimeLiquidate.HasValue ? retr.QuotePrimeLiquidate.Value.ToString() : string.Empty,
                    decorrenza, settimana, rmsQuota, retr.Nsettimane707.HasValue ? retr.Nsettimane707.Value.ToString() : string.Empty));
            }
            return elencoDatiRetributivi;
        }

        private static List<DatiContributivi> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributivi> elencoDatiContributivi = new List<DatiContributivi>();
            foreach (GestioneAggiornamentoPECODatiContributivi contr in areaDatiContributivi.DatiCalcolo.LDatiContributivi.ToList<GestioneAggiornamentoPECODatiContributivi>())
            {
                string settimana = string.Empty;
                string importo = string.Empty;
                string montante = string.Empty;

                if (contr.Quota.HasValue)
                {
                    if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        settimana = contr.Nsettimane.HasValue ? contr.Nsettimane.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivoTotale.HasValue ? contr.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteContributivo.HasValue ? contr.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        settimana = contr.SettimaneQuotaD.HasValue ? contr.SettimaneQuotaD.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivoQuotaD.HasValue ? contr.ImportoContributivoQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteContributivoQuotaD.HasValue ? contr.MontanteContributivoQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }
                elencoDatiContributivi.Add(new DatiContributivi(contr.CodiceGestione.HasValue ? contr.CodiceGestione.Value.ToString() : string.Empty,
                    contr.Quota.HasValue ? contr.Quota.Value.ToString() : string.Empty,
                    settimana, importo, montante));
            }
            return elencoDatiContributivi;
        }

        #endregion Methods Private

        #region EventHandler

        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler ShowAvvisoDatiCalcolo;
        public event EventHandler ShowErrorDatiCalcolo;
        public event EventHandler ShowAvvisoEliminaDatiCalcolo;
        public event EventHandler InitializeData;
        public event EventHandler AbilitaPopUpDatiContributivi;
        public event EventHandler DisabilitaPopUpDatiContributivi;

        protected void RaiseAbilitaPopUpDatiContributivi(object sender, EventArgs e)
        {
            AbilitaPopUpDatiContributivi(sender, e);
        }

        protected void RaiseDisabilitaPopUpDatiContributivi(object sender, EventArgs e)
        {
            DisabilitaPopUpDatiContributivi(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        protected void RaiseShowAvvisoDatiCalcolo(object sender, EventArgs e)
        {
            ShowAvvisoDatiCalcolo(sender, e);
        }

        protected void RaiseShowErrorDatiCalcolo(object sender, EventArgs e)
        {
            ShowErrorDatiCalcolo(sender, e);
        }

        protected void RaiseShowAvvisoEliminaDatiCalcolo(object sender, EventArgs e)
        {
            ShowAvvisoEliminaDatiCalcolo(sender, e);
        }

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

        #endregion EventHandler

        public void SetBtnShowPopUpContrib(AreaTitolare.DatiPensione datiPensione, List<DatiContributivi> lstDatiContributivi)
        {
            if (datiPensione.DecorrenzaOriginaria >= new DateTime(2015, 1, 1) && datiPensione.TipoLetturaUnicarpe != 'L')
            {
                DatiContributivi recordContrib = lstDatiContributivi.Where(x => (x.Quota ?? "").Trim() == "C").FirstOrDefault();
                if (recordContrib != null)
                {
                    decimal montante;
                    decimal ammontare;
                    if (decimal.TryParse(recordContrib.MontanteContributivo, out montante) &&
                        decimal.TryParse(recordContrib.AmmontareContributivo, out ammontare) &&
                        montante < ammontare)
                    {
                        btnPopUp.Style.Add("display", "inline-block");
                        btnSalvaDatiCalcolo.Style.Add("display", "none");
                        RaiseAbilitaPopUpDatiContributivi(this, null);
                    }
                    else
                    {
                        btnPopUp.Style.Add("display", "none");
                        btnSalvaDatiCalcolo.Style.Add("display", "inline-block");
                        RaiseDisabilitaPopUpDatiContributivi(this, null);
                    }
                }
                else
                {
                    btnPopUp.Style.Add("display", "none");
                    btnSalvaDatiCalcolo.Style.Add("display", "inline-block");
                    RaiseDisabilitaPopUpDatiContributivi(this, null);
                }

            }
        }
    }

    [Serializable]
    public class DatiContributivi
    {
        public DatiContributivi()
        { }

        public DatiContributivi(string gestione, string quota, string settimane, string ammontareContributivo, string montanteContributivo)
        {
            this._Gestione = gestione;
            this._Quota = quota;
            this._Settimane = settimane;
            this._AmmontareContributivo = ammontareContributivo;
            this._MontanteContributivo = montanteContributivo;
        }

        private string _Gestione;
        private string _Quota;
        private string _Settimane;
        private string _AmmontareContributivo;
        private string _MontanteContributivo;

        public string Gestione { get { return _Gestione; } set { _Gestione = value; } }
        public string Quota { get { return _Quota; } set { _Quota = value; } }
        public string Settimane { get { return _Settimane; } set { _Settimane = value; } }
        public string AmmontareContributivo { get { return _AmmontareContributivo; } set { _AmmontareContributivo = value; } }
        public string MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }
    }

    [Serializable]
    public class DatiRetributivi
    {
        public DatiRetributivi()
        { }

        public DatiRetributivi(string gestione, string quota, string decorrenza, string settimane, string redditoRetribuzioneMedia, string settimane707Ci)
        {
            this._Gestione = gestione;
            this._Quota = quota;
            this._Decorrenza = decorrenza;
            this._Settimane = settimane;
            this._RedditoRetribuzioneMedia = redditoRetribuzioneMedia;
            this._Settimane707 = settimane707Ci;
        }

        private string _Gestione;
        private string _Quota;
        private string _Decorrenza;
        private string _Settimane;
        private string _RedditoRetribuzioneMedia;
        private string _Settimane707;

        public string Gestione { get { return _Gestione; } set { _Gestione = value; } }
        public string Quota { get { return _Quota; } set { _Quota = value; } }
        public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public string Settimane { get { return _Settimane; } set { _Settimane = value; } }
        public string RedditoRetribuzioneMedia { get { return _RedditoRetribuzioneMedia; } set { _RedditoRetribuzioneMedia = value; } }
        public string Settimane707 { get { return _Settimane707; } set { _Settimane707 = value; } }

    }

    [Serializable]
    public class DatiContributiviEsteri
    {
        public DatiContributiviEsteri()
        { }

        public DatiContributiviEsteri(string gestione, string decorrenza, string settimane)
        {
            this._Gestione = gestione;
            this._Decorrenza = decorrenza;
            this._Settimane = settimane;
        }

        private string _Gestione;
        private string _Decorrenza;
        private string _Settimane;

        public string Gestione { get { return _Gestione; } set { _Gestione = value; } }
        public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public string Settimane { get { return _Settimane; } set { _Settimane = value; } }
    }
}
