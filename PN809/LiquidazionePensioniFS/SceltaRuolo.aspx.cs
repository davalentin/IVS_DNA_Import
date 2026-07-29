using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Any, CheckSequenceOnPostBack = false)]
    public partial class SceltaRuolo : BasePage, IRuoli, IVersioni
    {

        #region IRuoli Members
        public Dictionary<string, string> RuoliAbilitati { get; set; }
        public Ruoli SelectedRuolo { get; set; }
        #endregion IRuoli Members

        #region IVersioni
        public Dictionary<string, string> listaVersioni { get; set; }
        #endregion IVersioni

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                checkRuoli();
            }           
        }

        private void checkRuoli()
        {
            RuoliAbilitati = CodeUtility.GetRuoliAbilitati();
            if (Session["Ruolo"] == null)
            {
                if (RuoliAbilitati != null)
                {
                    foreach (var item in RuoliAbilitati)
                        ddlRuoli.Items.Add(new ListItem(item.Value, item.Key));
                }
                else
                {
                    pnlScelta.Enabled = false;
                    UCAvviso.Tipo = TipoAvviso.Warning;
                    UCAvviso.Visible = true;
                    UCAvviso.Messaggio = "Nessun ruolo abilitato";
                }
            }
            else
            {
                string nextPage = "Default.aspx";
                if (Session["UrlUnicarpe"] != null)
                {
                    nextPage = Session["UrlUnicarpe"].ToString();
                }
                else if (Session["UrlWebDom"] != null)
                {
                    nextPage = Session["UrlWebDom"].ToString();
                }
                else if (Session["UrlPrevisan"] != null)
                {
                    nextPage = Session["UrlPrevisan"].ToString();
                }
                Session.Remove("UrlUnicarpe");
                Session.Remove("UrlWebDom");
                Session.Remove("UrlPrevisan");
                Response.Redirect(nextPage, true);
            }
        }

        protected void btnSceltaRuolo_Click(object sender, EventArgs e)
        {
            PresenterMenuLeft presenter = new PresenterMenuLeft();
            Ruoli ruolo = (Ruoli)Enum.Parse(typeof(Ruoli), ddlRuoli.SelectedValue);

            switch ((UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo(ruolo))
            {
                case UtilityTipoAppartenenza.FS:
                    presenter.GetListaVersioniFS(this);
                    if (this.listaVersioni != null && this.listaVersioni.Count > 0)
                        CodeUtility.SetVersioni(this.listaVersioni);
                    break;
                case UtilityTipoAppartenenza.AGO:
                    presenter.GetListaVersioniAGO(this);
                    if (this.listaVersioni != null && this.listaVersioni.Count > 0)
                        CodeUtility.SetVersioni(this.listaVersioni);
                    break;
                case UtilityTipoAppartenenza.CI:
                    presenter.GetListaVersioniCI(this);
                    if (this.listaVersioni != null && this.listaVersioni.Count > 0)
                        CodeUtility.SetVersioni(this.listaVersioni);
                    break;
            }
            
            if (Session["Ruolo"] == null)
                Session.Add("Ruolo", ruolo);
            else
                Session["Ruolo"] = ruolo;
            checkSedePerRuolo();
            checkRuoli();
            //ricaricamento controllo
            UCChangeRuolo uc = (UCChangeRuolo)((UCIntestazione)Master.FindControl("UCIntestazione")).FindControl("ucChangeRuolo");
            if (uc != null) uc.ReloadControl();
        }

        private void checkSedePerRuolo()
        {
            if (!CodeUtility.IsAmministratore(Session["Ruolo"]) && INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice != null)
            {
                List<string> sediAbilitate = null;
                if (Session["Ruolo"] != null)
                {
                    PresenterSedi presenter = new PresenterSedi();
                    sediAbilitate = presenter.GetOfficeAspnCodeAbilitati(INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(((Ruoli)Session["Ruolo"]).ToString()).ToList<string>());
                }
                if (sediAbilitate == null || sediAbilitate.Count == 0)
                    INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = null;
                else
                { 
                    bool presente = false;
                    foreach (string s in sediAbilitate)
                    {
                        if (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode == s)
                        {
                            presente = true;
                            break;
                        }
                    }
                    if(!presente)
                        INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = null;
                }
            }
        }
     
    }
}

