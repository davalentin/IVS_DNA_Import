using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.LavorazioneManualeAutomatiche
{
    public partial class UCLavorazioneManualeAutomatiche : CustomBaseUserControl, ILavorazioneManualeAutomatiche
    {
        #region ILavorazioneManualeAutomatiche
        public AreaLavorazioneManualeAutomatiche LavorazioneManualeAutomatiche { get; set; }
        public AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche datiLavorazioneManualeAutomatiche { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        #endregion ILavorazioneManualeAutomatiche

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            if (!IsPostBack)
            {
                int paginaDaVisualizzare = 1;
                ValorizzaGriglia(paginaDaVisualizzare);
            }
        }

        #region private members
        private void ValorizzaGriglia(int paginaDaVisualizzare)
        {
            Presenter.PresenterLavorazioneManualeAutomatiche presenter = new Presenter.PresenterLavorazioneManualeAutomatiche();
            string utente = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            
            PresenterSedi presenterSedi = new PresenterSedi();

            List<string> sediAbilitateString = presenterSedi.GetOfficeAspnCodeAbilitati(INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(((Ruoli)Session["Ruolo"]).ToString()).ToList<string>());
            List<string> sediAbilitateSubString = new List<string>();
            foreach (string sd in sediAbilitateString)
            {
                sediAbilitateSubString.Add(sd.Substring(0, 4));
            }
            List<Int16> sediAbilitate = sediAbilitateSubString.Select(s => Convert.ToInt16(s)).ToList();

            if (CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
            {
                presenter.CaricaLavorazioneManualeAutomatiche(this);
            }
            else if (CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.DIRETTORE_RDP)
            {
                presenter.CaricaLavorazioneManualeAutomaticheByCodiceSede(null, sediAbilitate, this);
            }
            else if (CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.UTENTE)
            {
                presenter.CaricaLavorazioneManualeAutomaticheByCodiceSede(utente, sediAbilitate, this);
            }

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            ValorizzaViewStateLavorazioneManualeAutomatiche();
            gvLavorazioneManualeAutomatiche_Load(paginaDaVisualizzare);
        }

        private void ValorizzaViewStateLavorazioneManualeAutomatiche()
        {
            if (this.LavorazioneManualeAutomatiche != null)
            {
                ViewState["ElencoLavorazioneManualeAutomatiche"] = this.LavorazioneManualeAutomatiche.ListLavorazioneManualeAutomatiche.ToList();
            }
        }

        private void gvLavorazioneManualeAutomatiche_Load(int paginaDaVisualizzare)
        {
            try
            {
                gvLavorazioneManualeAutomatiche.DataSource = (List<AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche>)ViewState["ElencoLavorazioneManualeAutomatiche"];
                gvLavorazioneManualeAutomatiche.PageIndex = paginaDaVisualizzare < 1 ? 0 : paginaDaVisualizzare - 1;//paginaDaVisualizzare - 1 perchè la prima pagina ha PageIndex = 0 
                gvLavorazioneManualeAutomatiche.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLavorazioneManualeAutomatiche, Errore nel metodo gvLavorazioneManualeAutomatiche_Load " + ex);
            }
        }
        #endregion private members

        #region Grid
        protected void gvLavorazioneManualeAutomatiche_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                List<AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche> elencoLavorazioneManualeAutomatiche = (List<AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche>)ViewState["ElencoLavorazioneManualeAutomatiche"];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblDecorrenzaOriginaria = (Label)e.Row.FindControl("lblDecorrenzaOriginaria");
                    DropDownList ddlAutorizzazioneManuale = (DropDownList)e.Row.FindControl("ddlAutorizzazioneManuale");

                    int index = e.Row.DataItemIndex;
                    if (index >= 0 && index <= elencoLavorazioneManualeAutomatiche.Count - 1)
                    {
                        if (lblDecorrenzaOriginaria != null && elencoLavorazioneManualeAutomatiche[index].DecorrenzaOriginaria != null)
                            lblDecorrenzaOriginaria.Text = elencoLavorazioneManualeAutomatiche[index].DecorrenzaOriginaria.HasValue ? ((DateTime)elencoLavorazioneManualeAutomatiche[index].DecorrenzaOriginaria).ToString("dd/MM/yyyy") : null;

                        if (ddlAutorizzazioneManuale != null)
                        {
                            if (elencoLavorazioneManualeAutomatiche[index].AutorizzazioneManuale != null && elencoLavorazioneManualeAutomatiche[index].AutorizzazioneManuale.Value)
                            {
                                ddlAutorizzazioneManuale.SelectedValue = "1";
                                ddlAutorizzazioneManuale.Enabled = false;
                            }
                            else if (elencoLavorazioneManualeAutomatiche[index].AutorizzazioneManuale != null && elencoLavorazioneManualeAutomatiche[index].AutorizzazioneManuale.Value == false)
                            {
                                ddlAutorizzazioneManuale.SelectedValue = "2";
                                ddlAutorizzazioneManuale.Enabled = false;
                            }
                            if (CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.UTENTE)
                            {
                                ddlAutorizzazioneManuale.Enabled = false;
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
                throw new INPS.DNA.DnaApplicationException("UCLavorazioneManualeAutomatiche, Errore nel metodo gvLavorazioneManualeAutomatiche_RowDataBound " + ex);
            }
        }

        protected void DdlAutorizzazioneManuale_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (ddl.SelectedIndex != 0)
            {
                string commandArgs = ddl.Attributes["CommandArgument"].ToString();
                long id = 0;
                long.TryParse(commandArgs, out id);
                this.datiLavorazioneManualeAutomatiche = new AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche();
                this.datiLavorazioneManualeAutomatiche.Id = id;
                if(ddl.SelectedValue == "1")
                    this.datiLavorazioneManualeAutomatiche.AutorizzazioneManuale = true;
                else if(ddl.SelectedValue == "2")
                    this.datiLavorazioneManualeAutomatiche.AutorizzazioneManuale = false;

                PresenterLavorazioneManualeAutomatiche presenter = new PresenterLavorazioneManualeAutomatiche();
                presenter.SalvaLavorazioneManualeAutomatiche(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Cambio stato di lavorazione manuale salvato";
                    RaiseShowAvviso(this, null);
                }
            }
            int paginaDaVisualizzare = 1;
            ValorizzaGriglia(paginaDaVisualizzare);
        }

        protected void gvLavorazioneManualeAutomatiche_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
        }

        protected void gvLavorazioneManualeAutomatiche_onPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvLavorazioneManualeAutomatiche.EditIndex = -1;
                int paginaDaVisualizzare = e.NewPageIndex + 1;//e.NewPageIndex + 1  perchè la  prima pagina ha PageIndex = 0 
                gvLavorazioneManualeAutomatiche_Load(paginaDaVisualizzare);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCLavorazioneManualeAutomatiche, Errore nel metodo gvLavorazioneManualeAutomatiche_onPageIndexChanging" + ex);
            }
        }

        protected void gvLavorazioneManualeAutomatiche_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
        #endregion Grid

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler HideInfo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }
        #endregion Event Handlers
    }
}