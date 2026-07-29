using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi
{
    public partial class UCIntegrazioneVirtuale : CustomBaseUserControl, IDatiContributiviCi, ITitolarePensione
    {
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

                    if (this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale != null)
                        ValorizzaEtichette();
                }
            }
        }

        protected void btnSalvaIntegrazioneVirtuale_Click(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();

            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();

            GetDatiRedditiIVTab();

            presenterDatiContributiviCi.SalvaTabIntegrazioneVirtuale(this);

            if (!this.HasError)
            {
                ValorizzaEtichette();
            }
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO;
            }

            RaiseShowAvvisoIntegrazioneVirtuale(this, null);
        }

        protected void btnEliminaIntegrazioneVirtuale_Click(object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            GetDatiRedditiIVTab();

            PresenterDatiContributiviCI presenterDatiContributivi = new PresenterDatiContributiviCI();
            presenterDatiContributivi.EliminaTabIntegrazioneVirtuale(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Redditi per Integrazione Virtuale";
            }
            else
            {
                modalitaEditTitolare.Value = "false";
                modalitaEditConiuge.Value = "false";
                if (string.IsNullOrEmpty(this.ErrorMessage))
                    ValorizzaEtichette();
            }

            RaiseShowAvvisoEliminaIntegrazioneVirtuale(this, null);
        }

        #region Dati Titolare

        protected void gvRedditiPerIntegrazioneVirtualeTitolare_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            this.areaDatiContributiviCi = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        if (IsEmptyEditableRowRedditiPerIntegrazioneVirtualeTit(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableModeRedditiPerIntegrazioneVirtuale(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[1].FindControl("btnDeleteTitolare")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnnoReddTitolare")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Anno;
                                ((Label)e.Row.FindControl("lblRedditoTitolare")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Reddito;

                                EnableReadableModeTitolare(e.Row.Cells[0]);
                            }

                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableModeRedditiPerIntegrazioneVirtuale(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnnoReddTitolare")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Anno;
                                ((Label)e.Row.FindControl("lblRedditoTitolare")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Reddito;
                                EnableReadableModeTitolare(e.Row.Cells[0]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            EnableEditableModeRedditiPerIntegrazioneVirtuale(e.Row.Cells[0]);
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblAnnoReddTitolare")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Anno;
                            ((Label)e.Row.FindControl("lblRedditoTitolare")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Reddito;
                            EnableReadableModeTitolare(e.Row.Cells[0]);
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
                throw new INPS.DNA.DnaApplicationException("UCTabIntegrazioneVirtuale, Errore nel metodo gvRedditiPerIntegrazioneVirtualeTitolare_RowDataBound " + ex);
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeTitolare_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeTit = new List<RedditiPerIntegrazioneVirtuale>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string AnnoReddTitolare = string.Empty;
                    string RedditoTitolare = string.Empty;

                    if (!IsEmptyReadableRowRedditiPerIntegrazioneVirtualeTit(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            AnnoReddTitolare = ((Label)rApp.Cells[1].FindControl("lblAnnoReddTitolare")).Text;
                            RedditoTitolare = ((Label)rApp.Cells[2].FindControl("lblRedditoTitolare")).Text;

                            listaReddPerIntegrazioneVirtualeTit = AddRedditiPerIntegrazioneVirtuale(listaReddPerIntegrazioneVirtualeTit, AnnoReddTitolare, RedditoTitolare);
                        }
                    }
                }
                if (listaReddPerIntegrazioneVirtualeTit.Count == 0)
                {
                    this.modalitaEditTitolare.Value = "false";
                    GestioneTastoSalva();
                }

                listaReddPerIntegrazioneVirtualeTit.Add(new RedditiPerIntegrazioneVirtuale(string.Empty, string.Empty));

                removeItemBlankRedditiPerIntegrazioneVirtuale(ref listaReddPerIntegrazioneVirtualeTit);
                ViewState["redditiPerIntegrazVirtTitolare"] = listaReddPerIntegrazioneVirtualeTit;

                gvRedditiTitolare_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditTitolare.Value = "true";
                GestioneTastoSalva();
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRedditiPerIntegrazioneVirtualeTit((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeTit = new List<RedditiPerIntegrazioneVirtuale>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string AnnoReddTitolare = string.Empty;
                        string RedditoTitolare = string.Empty;

                        if (!IsEmptyEditableRowRedditiPerIntegrazioneVirtualeTit(rApp))
                        {
                            AnnoReddTitolare = ((TextBox)rApp.Cells[1].Controls[1]).Text;
                            RedditoTitolare = ((TextBox)rApp.Cells[2].Controls[1]).Text;
                            listaReddPerIntegrazioneVirtualeTit = AddRedditiPerIntegrazioneVirtuale(listaReddPerIntegrazioneVirtualeTit, AnnoReddTitolare, RedditoTitolare);
                        }
                        else if (!IsEmptyReadableRowRedditiPerIntegrazioneVirtualeTit(rApp))
                        {
                            AnnoReddTitolare = ((Label)rApp.Cells[1].FindControl("lblAnnoReddTitolare")).Text;
                            RedditoTitolare = ((Label)rApp.Cells[2].FindControl("lblRedditoTitolare")).Text;

                            listaReddPerIntegrazioneVirtualeTit = AddRedditiPerIntegrazioneVirtuale(listaReddPerIntegrazioneVirtualeTit, AnnoReddTitolare, RedditoTitolare);
                        }
                    }
                    modalitaEditTitolare.Value = "false";
                    GestioneTastoSalva();

                    gvRedditiPerIntegrazioneVirtualeTitolare.EditIndex = -1;
                    ViewState["redditiPerIntegrazVirtTitolare"] = listaReddPerIntegrazioneVirtualeTit;

                    gvRedditiPerIntegrazioneVirtualeTitolare.DataSource = listaReddPerIntegrazioneVirtualeTit;
                    gvRedditiPerIntegrazioneVirtualeTitolare.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeTit = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtTitolare"];
                if (!IsListaEmptyTitolare())
                {
                    modalitaEditTitolare.Value = "false";
                    GestioneTastoSalva();

                    if (listaReddPerIntegrazioneVirtualeTit == null)
                        listaReddPerIntegrazioneVirtualeTit = new List<RedditiPerIntegrazioneVirtuale>();
                    if (listaReddPerIntegrazioneVirtualeTit.Count == 0)
                    {
                        listaReddPerIntegrazioneVirtualeTit.Add(new RedditiPerIntegrazioneVirtuale(string.Empty, string.Empty));
                        gvRedditiPerIntegrazioneVirtualeTitolare.EditIndex = 0;
                    }
                    else
                        gvRedditiPerIntegrazioneVirtualeTitolare.EditIndex = -1;
                    gvRedditiPerIntegrazioneVirtualeTitolare.DataSource = listaReddPerIntegrazioneVirtualeTit;
                    gvRedditiPerIntegrazioneVirtualeTitolare.DataBind();
                }
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeTitolare_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRedditiPerIntegrazioneVirtualeTitolare.EditIndex = e.NewEditIndex;
                gvRedditiTitolare_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTabIntegrazioneVirtuale, Errore nel metodo gvRedditiPerIntegrazioneVirtualeTitolare_RowEditing " + ex);
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeTitolare_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvRedditiPerIntegrazioneVirtualeTitolare.EditIndex = -1;

                List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeTit = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtTitolare"];
                gvRedditiPerIntegrazioneVirtualeTitolare.DataSource = listaReddPerIntegrazioneVirtualeTit;
                gvRedditiPerIntegrazioneVirtualeTitolare.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTabIntegrazioneVirtuale, Errore nel metodo ggvRedditiPerIntegrazioneVirtualeTitolare_RowCancelingEdit " + ex);
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeTitolare_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        #endregion Dati Titolare

        #region Dati Coniuge

        protected void gvRedditiPerIntegrazioneVirtualeConiuge_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            this.areaDatiContributiviCi = ((AreaDatiContributivi)ViewState["DatiContributiviCi"]);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        if (IsEmptyEditableRowRedditiPerIntegrazioneVirtualeConiuge(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableModeRedditiPerIntegrazioneVirtuale(e.Row.Cells[0]);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[1].FindControl("btnDeleteConiuge")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnnoRedditoConiuge")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Anno;
                                ((Label)e.Row.FindControl("lblRedditoConiuge")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Reddito;

                                EnableReadableModeConiuge(e.Row.Cells[0]);
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableModeRedditiPerIntegrazioneVirtuale(e.Row.Cells[0]);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnnoRedditoConiuge")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Anno;
                                ((Label)e.Row.FindControl("lblRedditoConiuge")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Reddito;

                                EnableReadableModeConiuge(e.Row.Cells[0]);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            EnableEditableModeRedditiPerIntegrazioneVirtuale(e.Row.Cells[0]);
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblAnnoRedditoConiuge")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Anno;
                            ((Label)e.Row.FindControl("lblRedditoConiuge")).Text = ((RedditiPerIntegrazioneVirtuale)(e.Row.DataItem)).Reddito;

                            EnableReadableModeConiuge(e.Row.Cells[0]);
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
                throw new INPS.DNA.DnaApplicationException("UCTabIntegrazioneVirtuale, Errore nel metodo gvRedditiPerIntegrazioneVirtualeConiuge_RowDataBound " + ex);
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeConiuge_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeConiuge = new List<RedditiPerIntegrazioneVirtuale>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string AnnoReddConiuge = string.Empty;
                    string RedditoConiuge = string.Empty;

                    if (!IsEmptyReadableRowRedditiPerIntegrazioneVirtualeConiuge(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            AnnoReddConiuge = ((Label)rApp.Cells[1].FindControl("lblAnnoRedditoConiuge")).Text;
                            RedditoConiuge = ((Label)rApp.Cells[2].FindControl("lblRedditoConiuge")).Text;

                            listaReddPerIntegrazioneVirtualeConiuge = AddRedditiPerIntegrazioneVirtuale(listaReddPerIntegrazioneVirtualeConiuge, AnnoReddConiuge, RedditoConiuge);
                        }
                    }
                }
                if (listaReddPerIntegrazioneVirtualeConiuge.Count == 0)
                {
                    this.modalitaEditConiuge.Value = "false";
                    GestioneTastoSalva();
                }

                listaReddPerIntegrazioneVirtualeConiuge.Add(new RedditiPerIntegrazioneVirtuale(string.Empty, string.Empty));

                removeItemBlankRedditiPerIntegrazioneVirtuale(ref listaReddPerIntegrazioneVirtualeConiuge);
                ViewState["redditiPerIntegrazVirtConiuge"] = listaReddPerIntegrazioneVirtualeConiuge;

                gvRedditiConiuge_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditConiuge.Value = "true";
                GestioneTastoSalva();
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRedditiPerIntegrazioneVirtualeConiuge((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeConiuge = new List<RedditiPerIntegrazioneVirtuale>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string AnnoReddConiuge = string.Empty;
                        string RedditoConiuge = string.Empty;

                        if (!IsEmptyEditableRowRedditiPerIntegrazioneVirtualeConiuge(rApp))
                        {
                            AnnoReddConiuge = ((TextBox)rApp.Cells[1].Controls[1]).Text;
                            RedditoConiuge = ((TextBox)rApp.Cells[2].Controls[1]).Text;
                            listaReddPerIntegrazioneVirtualeConiuge = AddRedditiPerIntegrazioneVirtuale(listaReddPerIntegrazioneVirtualeConiuge, AnnoReddConiuge, RedditoConiuge);
                        }
                        else if (!IsEmptyReadableRowRedditiPerIntegrazioneVirtualeConiuge(rApp))
                        {
                            AnnoReddConiuge = ((Label)rApp.Cells[1].FindControl("lblAnnoRedditoConiuge")).Text;
                            RedditoConiuge = ((Label)rApp.Cells[2].FindControl("lblRedditoConiuge")).Text;

                            listaReddPerIntegrazioneVirtualeConiuge = AddRedditiPerIntegrazioneVirtuale(listaReddPerIntegrazioneVirtualeConiuge, AnnoReddConiuge, RedditoConiuge);
                        }
                    }
                    gvRedditiPerIntegrazioneVirtualeConiuge.EditIndex = -1;
                    ViewState["redditiPerIntegrazVirtConiuge"] = listaReddPerIntegrazioneVirtualeConiuge;

                    modalitaEditConiuge.Value = "false";
                    GestioneTastoSalva();

                    gvRedditiPerIntegrazioneVirtualeConiuge.DataSource = listaReddPerIntegrazioneVirtualeConiuge;
                    gvRedditiPerIntegrazioneVirtualeConiuge.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeConiuge = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtConiuge"];
                if (!IsListaEmptyConiuge())
                {
                    modalitaEditConiuge.Value = "false";
                    GestioneTastoSalva();

                    if (listaReddPerIntegrazioneVirtualeConiuge == null)
                        listaReddPerIntegrazioneVirtualeConiuge = new List<RedditiPerIntegrazioneVirtuale>();
                    if (listaReddPerIntegrazioneVirtualeConiuge.Count == 0)
                    {
                        listaReddPerIntegrazioneVirtualeConiuge.Add(new RedditiPerIntegrazioneVirtuale(string.Empty, string.Empty));
                        gvRedditiPerIntegrazioneVirtualeConiuge.EditIndex = 0;
                    }
                    else
                        gvRedditiPerIntegrazioneVirtualeConiuge.EditIndex = -1;
                    gvRedditiPerIntegrazioneVirtualeConiuge.DataSource = listaReddPerIntegrazioneVirtualeConiuge;
                    gvRedditiPerIntegrazioneVirtualeConiuge.DataBind();
                }
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeConiuge_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRedditiPerIntegrazioneVirtualeConiuge.EditIndex = e.NewEditIndex;
                gvRedditiConiuge_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTabIntegrazioneVirtuale, Errore nel metodo gvRedditiPerIntegrazioneVirtualeConiuge_RowEditing " + ex);
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeConiuge_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvRedditiPerIntegrazioneVirtualeConiuge.EditIndex = -1;

                List<RedditiPerIntegrazioneVirtuale> listaReddPerIntegrazioneVirtualeConiuge = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtConiuge"];
                gvRedditiPerIntegrazioneVirtualeConiuge.DataSource = listaReddPerIntegrazioneVirtualeConiuge;
                gvRedditiPerIntegrazioneVirtualeConiuge.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTabIntegrazioneVirtuale, Errore nel metodo gvRedditiPerIntegrazioneVirtualeConiuge_RowCancelingEdit " + ex);
            }
        }

        protected void gvRedditiPerIntegrazioneVirtualeConiuge_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        #endregion Dati Coniuge

        private void ValorizzaEtichette()
        {
            List<RedditiPerIntegrazioneVirtuale> redditiPerIntegrazVirtTitolare = new List<RedditiPerIntegrazioneVirtuale>();
            List<RedditiPerIntegrazioneVirtuale> redditiPerIntegrazVirtConiuge = new List<RedditiPerIntegrazioneVirtuale>();

            if (areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale != null && areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale.Count() > 0)
            {
                foreach (GestioneContribRedditiPerIntegrazioneVirtuale reddito in areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale)
                {
                    if (reddito.IsTitolare)
                    {
                        redditiPerIntegrazVirtTitolare.Add(new RedditiPerIntegrazioneVirtuale(reddito.Anno.ToString(),
                       reddito.Reddito.ToString()));
                    }
                    else
                    {
                        redditiPerIntegrazVirtConiuge.Add(new RedditiPerIntegrazioneVirtuale(reddito.Anno.ToString(),
                       reddito.Reddito.ToString()));
                    }
                }
            }

            ViewState["redditiPerIntegrazVirtTitolare"] = redditiPerIntegrazVirtTitolare;
            gvRedditiTitolare_Load();

            ViewState["redditiPerIntegrazVirtConiuge"] = redditiPerIntegrazVirtConiuge;
            gvRedditiConiuge_Load();

        }

        internal void GetDatiRedditiPerIntegrazioneVirtuale(GestioneContribProRata datiProrata)
        {
            GetListaAnni(datiProrata);
            ValorizzaEtichette();
        }

        internal List<GestioneContribRedditiPerIntegrazioneVirtuale> GetListaAnni(GestioneContribProRata datiProrata)
        {
            this.areaDatiContributiviCi = (Presenter.SvrLiquidazioneCi.AreaDatiContributivi)ViewState["DatiContributiviCi"];

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);

            List<RedditiPerIntegrazioneVirtuale> elencoReddIVTitolare = ((List<RedditiPerIntegrazioneVirtuale>)(ViewState["redditiPerIntegrazVirtTitolare"]));
            List<RedditiPerIntegrazioneVirtuale> elencoReddIVConiuge = ((List<RedditiPerIntegrazioneVirtuale>)(ViewState["redditiPerIntegrazVirtConiuge"]));

            List<GestioneContribRedditiPerIntegrazioneVirtuale> LreddIntegrVirtuale = new List<GestioneContribRedditiPerIntegrazioneVirtuale>();

            foreach (RedditiPerIntegrazioneVirtuale reddIVTitolare in elencoReddIVTitolare)
            {
                GestioneContribRedditiPerIntegrazioneVirtuale reddTitolare = new GestioneContribRedditiPerIntegrazioneVirtuale();
                reddTitolare.IsTitolare = true;

                reddTitolare.Anno = int.Parse(reddIVTitolare.Anno);
                if (!string.IsNullOrEmpty(reddIVTitolare.Reddito))
                    reddTitolare.Reddito = decimal.Parse(reddIVTitolare.Reddito);
                LreddIntegrVirtuale.Add(reddTitolare);
            }

            foreach (RedditiPerIntegrazioneVirtuale reddIVConiuge in elencoReddIVConiuge)
            {
                GestioneContribRedditiPerIntegrazioneVirtuale reddConiuge = new GestioneContribRedditiPerIntegrazioneVirtuale();

                reddConiuge.Anno = int.Parse(reddIVConiuge.Anno);
                if (!string.IsNullOrEmpty(reddIVConiuge.Reddito))
                    reddConiuge.Reddito = decimal.Parse(reddIVConiuge.Reddito);
                LreddIntegrVirtuale.Add(reddConiuge);
            }

            List<int> anniProrata = new List<int>();
            // RECUPERO ANNI DI TUTTI GLI STATI DEL PRORATA
            foreach (GestioneContribStatoEstero stato in datiProrata.ElencoStatiEsteri)
            {
                foreach (GestioneDatiContributiviCiPensioniCiImportiEsteri importo in stato.ElencoImportiEsteri)
                {
                    if (!anniProrata.Exists(x => x == importo.DecorrenzaPrestazioneEE.Value.Year))
                        anniProrata.Add(importo.DecorrenzaPrestazioneEE.Value.Year);
                }
            }
            // AGGIUNTA ANNI TAB INTEGRAZIONE VIRTUALE
            foreach (int anno in anniProrata)
            {
                if (anno != datiPensione.DecorrenzaOriginaria.Value.Year)
                {
                    GestioneContribRedditiPerIntegrazioneVirtuale redditoIVTitolare = new GestioneContribRedditiPerIntegrazioneVirtuale();
                    redditoIVTitolare.Anno = anno;
                    redditoIVTitolare.IsTitolare = true;
                    if (!LreddIntegrVirtuale.Exists(x => x.Anno == redditoIVTitolare.Anno && x.IsTitolare))
                        LreddIntegrVirtuale.Add(redditoIVTitolare);

                    GestioneContribRedditiPerIntegrazioneVirtuale redditoIVConiuge = new GestioneContribRedditiPerIntegrazioneVirtuale();
                    redditoIVConiuge.Anno = anno;
                    if (!LreddIntegrVirtuale.Exists(x => x.Anno == redditoIVConiuge.Anno && !x.IsTitolare))
                        LreddIntegrVirtuale.Add(redditoIVConiuge);
                }
            }

            List<GestioneContribRedditiPerIntegrazioneVirtuale> LreddIntegrVirtualeApp = new List<GestioneContribRedditiPerIntegrazioneVirtuale>();
            foreach (GestioneContribRedditiPerIntegrazioneVirtuale red in LreddIntegrVirtuale)
            {
                GestioneContribRedditiPerIntegrazioneVirtuale redApp = new GestioneContribRedditiPerIntegrazioneVirtuale();
                redApp.Anno = red.Anno;
                LreddIntegrVirtualeApp.Add(redApp);
            }
            // ELIMINAZIONE ANNI TAB INTEGRAZIONE VIRTUALE
            foreach (GestioneContribRedditiPerIntegrazioneVirtuale redd in LreddIntegrVirtualeApp)
            {
                if (redd.Anno != datiPensione.DecorrenzaOriginaria.Value.Year && !anniProrata.Exists(x => x == redd.Anno))
                    LreddIntegrVirtuale.RemoveAll(x => x.Anno == redd.Anno);
            }

            this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = LreddIntegrVirtuale.ToArray();

            return LreddIntegrVirtuale;
        }

        internal GestioneContribRedditiPerIntegrazioneVirtuale[] GetRedditiPerIntegrazioneVirtuale()
        {
            return GetDatiRedditiIVTab();
        }

        private GestioneContribRedditiPerIntegrazioneVirtuale[] GetDatiRedditiIVTab()
        {
            this.areaDatiContributiviCi = new AreaDatiContributivi();
            List<GestioneContribRedditiPerIntegrazioneVirtuale> listRedditiIV = null;

            List<RedditiPerIntegrazioneVirtuale> elencoReddIVTitolare = ((List<RedditiPerIntegrazioneVirtuale>)(ViewState["redditiPerIntegrazVirtTitolare"]));
            if (elencoReddIVTitolare != null)
                removeItemBlankRedditiPerIntegrazioneVirtuale(ref elencoReddIVTitolare);

            List<RedditiPerIntegrazioneVirtuale> elencoReddIVConiuge = ((List<RedditiPerIntegrazioneVirtuale>)(ViewState["redditiPerIntegrazVirtConiuge"]));
            if (elencoReddIVConiuge != null)
                removeItemBlankRedditiPerIntegrazioneVirtuale(ref elencoReddIVConiuge);

            if (elencoReddIVTitolare != null && elencoReddIVTitolare.Count > 0 && elencoReddIVConiuge != null && elencoReddIVConiuge.Count > 0)
            {
                listRedditiIV = new List<GestioneContribRedditiPerIntegrazioneVirtuale>();

                foreach (RedditiPerIntegrazioneVirtuale reddIVTitolare in elencoReddIVTitolare)
                {
                    GestioneContribRedditiPerIntegrazioneVirtuale reddTitolare = new GestioneContribRedditiPerIntegrazioneVirtuale();
                    reddTitolare.IsTitolare = true;

                    if (reddIVTitolare.Anno == string.Empty)
                        reddTitolare.Anno = 0;
                    else
                        reddTitolare.Anno = int.Parse(reddIVTitolare.Anno);

                    if (reddIVTitolare.Reddito == string.Empty)
                        reddTitolare.Reddito = null;
                    else
                        reddTitolare.Reddito = decimal.Parse(reddIVTitolare.Reddito);

                    listRedditiIV.Add(reddTitolare);
                }

                foreach (RedditiPerIntegrazioneVirtuale reddIVConiuge in elencoReddIVConiuge)
                {
                    GestioneContribRedditiPerIntegrazioneVirtuale reddConiuge = new GestioneContribRedditiPerIntegrazioneVirtuale();

                    if (reddIVConiuge.Anno == string.Empty)
                        reddConiuge.Anno = 0;
                    else
                        reddConiuge.Anno = int.Parse(reddIVConiuge.Anno);

                    if (reddIVConiuge.Reddito == string.Empty)
                        reddConiuge.Reddito = null;
                    else
                        reddConiuge.Reddito = decimal.Parse(reddIVConiuge.Reddito);

                    listRedditiIV.Add(reddConiuge);
                }

                this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = new GestioneContribRedditiPerIntegrazioneVirtuale[listRedditiIV.Count];
                this.areaDatiContributiviCi.LRedditiPerIntegrazioneVirtuale = listRedditiIV.ToArray();
            }
            if (listRedditiIV != null && listRedditiIV.Count() > 0)
                return listRedditiIV.ToArray();
            else
                return null;
        }

        private void GestioneTastoSalva()
        {
            if (modalitaEditTitolare.Value == "false" && modalitaEditConiuge.Value == "false")
            {
                btnSalvaRedditiPerIntegrazioneVirtuale.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
            else
            {
                btnSalvaRedditiPerIntegrazioneVirtuale.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
        }

        private List<RedditiPerIntegrazioneVirtuale> AddRedditiPerIntegrazioneVirtuale(List<RedditiPerIntegrazioneVirtuale> listaRecord, string anno, string reddito)
        {
            listaRecord.Add(new RedditiPerIntegrazioneVirtuale(anno, reddito));
            return listaRecord;
        }

        private void removeItemBlankRedditiPerIntegrazioneVirtuale(ref List<RedditiPerIntegrazioneVirtuale> lista)
        {
            int index = lista.FindIndex(delegate(RedditiPerIntegrazioneVirtuale code)
            {
                return (code.Anno == string.Empty && code.Reddito == string.Empty);
            }
               );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private void EnableEditableModeRedditiPerIntegrazioneVirtuale(TableCell cell_CancelSave)
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
            save.ValidationGroup = "UCTabIntegrazioneVirtuale";
        }

        private void gvRedditiTitolare_Load()
        {
            List<RedditiPerIntegrazioneVirtuale> elencoRedditiPerIntegrazioneVirtuale = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtTitolare"];

            RedditiPerIntegrazioneVirtuale Empty = elencoRedditiPerIntegrazioneVirtuale.Find(delegate(RedditiPerIntegrazioneVirtuale code)
            {
                return (code.Anno == string.Empty && code.Reddito == string.Empty);
            }
            );

            if (Empty == null && elencoRedditiPerIntegrazioneVirtuale.Count == 0)
            {
                elencoRedditiPerIntegrazioneVirtuale.Add(new RedditiPerIntegrazioneVirtuale(string.Empty, string.Empty));
            }

            gvRedditiPerIntegrazioneVirtualeTitolare.DataSource = elencoRedditiPerIntegrazioneVirtuale;
            gvRedditiPerIntegrazioneVirtualeTitolare.DataBind();
        }

        private void EnableReadableModeTitolare(TableCell cell_Edit)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
        }

        private bool IsListaEmptyTitolare()
        {
            List<RedditiPerIntegrazioneVirtuale> listaDatiReddPerIntegrazVirtualeTit = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtTitolare"];
            if (listaDatiReddPerIntegrazVirtualeTit.Count == 1 && listaDatiReddPerIntegrazVirtualeTit[0].Anno == string.Empty &&
                listaDatiReddPerIntegrazVirtualeTit[0].Reddito == string.Empty)
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRowRedditiPerIntegrazioneVirtualeTit(GridViewRow row)
        {
            if ((row.FindControl("txtAnnoReddTitolare") != null && ((TextBox)row.FindControl("txtAnnoReddTitolare")).Text != string.Empty) ||
               (row.FindControl("txtRedditoTitolare") != null && ((TextBox)row.FindControl("txtRedditoTitolare")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowRedditiPerIntegrazioneVirtualeTit(GridViewRow row)
        {
            if (row.FindControl("lblAnnoReddTitolare") != null && ((Label)row.FindControl("lblAnnoReddTitolare")).Text != string.Empty ||
               row.FindControl("lblRedditoTitolare") != null && ((Label)row.FindControl("lblRedditoTitolare")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void gvRedditiConiuge_Load()
        {
            List<RedditiPerIntegrazioneVirtuale> elencoRedditiPerIntegrazioneVirtuale = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtConiuge"];

            RedditiPerIntegrazioneVirtuale Empty = elencoRedditiPerIntegrazioneVirtuale.Find(delegate(RedditiPerIntegrazioneVirtuale code)
            {
                return (code.Anno == string.Empty && code.Reddito == string.Empty);
            }
            );

            if (Empty == null && elencoRedditiPerIntegrazioneVirtuale.Count == 0)
            {
                elencoRedditiPerIntegrazioneVirtuale.Add(new RedditiPerIntegrazioneVirtuale(string.Empty, string.Empty));
            }

            gvRedditiPerIntegrazioneVirtualeConiuge.DataSource = elencoRedditiPerIntegrazioneVirtuale;
            gvRedditiPerIntegrazioneVirtualeConiuge.DataBind();
        }

        private void EnableReadableModeConiuge(TableCell cell_Edit)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
        }

        private bool IsListaEmptyConiuge()
        {
            List<RedditiPerIntegrazioneVirtuale> listaDatiReddPerIntegrazVirtualeCon = (List<RedditiPerIntegrazioneVirtuale>)ViewState["redditiPerIntegrazVirtConiuge"];
            if (listaDatiReddPerIntegrazVirtualeCon.Count == 1 && listaDatiReddPerIntegrazVirtualeCon[0].Anno == string.Empty &&
                listaDatiReddPerIntegrazVirtualeCon[0].Reddito == string.Empty)
                return true;
            else
                return false;
        }

        private bool IsEmptyEditableRowRedditiPerIntegrazioneVirtualeConiuge(GridViewRow row)
        {
            if ((row.FindControl("txtAnnoRedditoConiuge") != null && ((TextBox)row.FindControl("txtAnnoRedditoConiuge")).Text != string.Empty) ||
               (row.FindControl("txtRedditoConiuge") != null && ((TextBox)row.FindControl("txtRedditoConiuge")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowRedditiPerIntegrazioneVirtualeConiuge(GridViewRow row)
        {
            if (row.FindControl("lblAnnoRedditoConiuge") != null && ((Label)row.FindControl("lblAnnoRedditoConiuge")).Text != string.Empty ||
               row.FindControl("lblRedditoConiuge") != null && ((Label)row.FindControl("lblRedditoConiuge")).Text != string.Empty)
                return false;
            else
                return true;
        }

        #region EventHandler

        public event EventHandler ShowIntegrazioneVirtuale;
        public event EventHandler ShowAvvisoEliminaIntegrazioneVirtuale;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;

        protected void RaiseShowAvvisoIntegrazioneVirtuale(object sender, EventArgs e)
        {
            ShowIntegrazioneVirtuale(sender, e);
        }

        protected void RaiseShowAvvisoEliminaIntegrazioneVirtuale(object sender, EventArgs e)
        {
            ShowAvvisoEliminaIntegrazioneVirtuale(sender, e);
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

        [Serializable]
        public class RedditiPerIntegrazioneVirtuale
        {
            public RedditiPerIntegrazioneVirtuale()
            { }

            public RedditiPerIntegrazioneVirtuale(string anno, string reddito)
            {
                this._Anno = anno;
                this._Reddito = reddito;
            }

            private string _Anno;
            private string _Reddito;

            public string Anno { get { return _Anno; } set { _Anno = value; } }
            public string Reddito { get { return _Reddito; } set { _Reddito = value; } }
        }
    }
}