using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class SceltaSede : BasePage, ISedi
    {

        #region ISedi Members
        public string CommaSeparatedSedi { get; set; }
        public Dictionary<string, string> DictionaryOfficeList { get; set; }
        public string Sede { get; set; }
        public List<string> SediAbilitate { get; set; }
        public INPS.DNA.Office SelectedOffice { get; set; }
        #endregion ISedi Members
        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                checkCurrentOffice();
            }
        }

        private void checkCurrentOffice()
        {
            if (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice == null)
            {
                PresenterSedi presenter = new PresenterSedi();
                if (CodeUtility.IsAmministratore(Session["Ruolo"]))
                {
                    presenter.GetCommaSeparatedSedi(this);
                    HiddenFieldSedi.Value = string.Empty;
                    HiddenFieldSedi.Value = CommaSeparatedSedi;
                    pnlSceltaOpe.Visible = false;
                    pnlSceltaAdmin.Visible = true;
                    txtSceltaSede.Focus();
                    if (string.IsNullOrEmpty(txtSceltaSede.Text))
                    {
                        txtSceltaSede.Text = GetSedeDefaultAdmin();
                        txtSceltaSede.Attributes.Add("onfocus", "this.select();");
                    }
                }
                else
                {
                    var lista = loadSedi();
                    if (lista.Length > 0)
                    {
                        ddlSedi.Items.AddRange(lista.OrderBy(x => x.Text).ToArray());
                        pnlSceltaOpe.Visible = true;
                        pnlSceltaAdmin.Visible = false;
                    }
                    else if (!string.IsNullOrEmpty(((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode))
                    {
                        lista = loadSede();
                        if (lista.Length > 0)
                        {
                            ddlSedi.Items.AddRange(lista.OrderBy(x => x.Text).ToArray());
                            pnlSceltaOpe.Visible = true;
                            pnlSceltaAdmin.Visible = false;
                        }
                        else
                        {
                            pnlSceltaOpe.Visible = true;
                            pnlSceltaOpe.Enabled = false;
                            pnlSceltaAdmin.Visible = false;
                            UCAvviso.Tipo = TipoAvviso.Warning;
                            UCAvviso.Visible = true;
                            UCAvviso.Messaggio = "Nessuna sede abilitata";
                        }
                    }
                    else
                    {
                        pnlSceltaOpe.Visible = true;
                        pnlSceltaOpe.Enabled = false;
                        pnlSceltaAdmin.Visible = false;
                        UCAvviso.Tipo = TipoAvviso.Warning;
                        UCAvviso.Visible = true;
                        UCAvviso.Messaggio = "Nessuna sede abilitata";
                    }
                }
            }
            else
            {
                string nextPage = "Default.aspx";
                if (Session["PreviousPage"] != null)
                {
                    nextPage = Session["PreviousPage"].ToString();
                }
                Session.Remove("PreviousPage");
                Response.Redirect(nextPage, true);
            }
        }

        private string GetSedeDefaultAdmin()
        {
            string sedeAdmin = string.Empty;

            try
            {
                var i = (from o in INPS.DNA.Context.OfficeList.Offices
                         where o.Value.AspnCode == ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode
                         select o).FirstOrDefault();
                sedeAdmin = string.Format("{0}-{1}", i.Value.AspnCode, (i.Value.ExtendedProperties != null ? i.Value.ExtendedProperties["SEDE"].Trim() : i.Value.Name.Trim()));
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }

            return sedeAdmin;
        }

        private ListItem[] loadSedi()
        {
            List<ListItem> listaSedi = new List<ListItem>();
            SediAbilitate = new List<string>();
            if (Session["Ruolo"] != null)
                SediAbilitate = INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(((Ruoli)Session["Ruolo"]).ToString()).ToList<string>();
            PresenterSedi presenter = new PresenterSedi();
            presenter.GetOfficeSediAbilitate(this);
            foreach (var item in DictionaryOfficeList)
            {
                listaSedi.Add(new ListItem(item.Key, item.Value));
            }
            return listaSedi.ToArray();
        }

        private ListItem[] loadSede()
        {
            List<ListItem> listaSedi = new List<ListItem>();
            SediAbilitate = new List<string>() { ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode };
            PresenterSedi presenter = new PresenterSedi();
            presenter.GetOfficeSediAbilitate(this);
            foreach (var item in DictionaryOfficeList)
            {
                listaSedi.Add(new ListItem(item.Key, item.Value));
            }
            return listaSedi.ToArray();
        }

        protected void btnSceltaSede_Click(object sender, EventArgs e)
        {
            PresenterSedi presenter = new PresenterSedi();
            if (CodeUtility.IsAmministratore(Session["Ruolo"]))
            {
                Sede = txtSceltaSede.Text.ToUpperInvariant().Substring(txtSceltaSede.Text.IndexOf('-') + 1).Trim();
            }
            else
            {
                Sede = ddlSedi.SelectedValue.ToUpperInvariant();
            }
            presenter.GetOffice(this);
            if (SelectedOffice == null)
            {
                UCAvviso.Tipo = TipoAvviso.Warning;
                UCAvviso.Visible = true;
                UCAvviso.Messaggio = "La sede selezionata non è valida";
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = SelectedOffice;
            checkCurrentOffice();
            //ricaricamento controllo UCChangeAzienda master page
            UCChangeSede uc = (UCChangeSede)((UCIntestazione)Master.FindControl("UCIntestazione")).FindControl("ucChangeSede");
            if (uc != null) uc.ReloadControl();
        }


    }
}
