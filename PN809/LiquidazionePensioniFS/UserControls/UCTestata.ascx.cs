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
using System.Configuration;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCTestata : CustomBaseUserControl, ISegnalazione, IVersioni
    {
        #region ISegnalazione
        public AreaEsito areaEsito { get; set; }
        public AreaInvioSegnalazione InvioSegnalazione { get; set; }
        #endregion ISegnalazione

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        #region IVersioni Members
        public Dictionary<string, string> listaVersioni { get; set; }
        #endregion IVersioni Members

        public UtilityTipoAppartenenza TipoAppRuolo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            hCurrentTheme.Value = Page.Theme;

            var identity = (INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity;
            string firstName = identity.FirstName;
            string lastName = identity.LastName;
            lblUserInitial.Text = ( (firstName != null && firstName.Length > 0 ? firstName[0].ToString() : "") +
                (lastName != null && lastName.Length > 0 ? lastName[0].ToString() : "")
            ).ToUpper();


            lblUtente.Text = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).FirstName + " " + ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).LastName;
            lblMatricola.Text = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;

            if (!IsPostBack)
            {
                //lblUtente.Text = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).FirstName + " " + ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).LastName;

                if (Session["ListaVersioni"] == null)
                {
                    Presenter.PresenterMenuLeft presenter = new Presenter.PresenterMenuLeft();
                    presenter.GetListaVersioni(this);

                    if (Session["Ruolo"] != null)
                    {
                        this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

                        switch (TipoAppRuolo)
                        {
                            case UtilityTipoAppartenenza.FS:
                                presenter.GetListaVersioniFS(this);
                                break;
                            case UtilityTipoAppartenenza.AGO:
                                presenter.GetListaVersioniAGO(this);
                                break;
                            case UtilityTipoAppartenenza.CI:
                                presenter.GetListaVersioniCI(this);
                                break;
                        }
                    }

                    CodeUtility.SetVersioni(this.listaVersioni);
                }

                if (Session["Ruolo"] != null)
                {
                    this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

                    hTipoApp.Value = TipoAppRuolo.ToString();
                    if (ConfigurationManager.AppSettings[TipoAppRuolo.ToString() + "PulsantiIntestazioneVisible"] != null &&
                        ConfigurationManager.AppSettings[TipoAppRuolo.ToString() + "PulsantiIntestazioneVisible"] == "SI")
                        pnlPulsantiIntestazione.Visible = true;
                    else
                        pnlPulsantiIntestazione.Visible = false;

                    LoadDdl();
                }
                else
                    pnlPulsantiIntestazione.Visible = false;

                //if (ConfigurationManager.AppSettings[TipoAppRuolo.ToString() + "ProceduraDPIVisible"] != null &&
                //    ConfigurationManager.AppSettings[TipoAppRuolo.ToString() + "ProceduraDPIVisible"] == "SI")
                //{
                //    if (Session["URLDPI"] != null)
                //    {
                //        pnlProceduraDPI.Visible = true;
                //        hUrlDPI.Value = (string)Session["URLDPI"];
                //    }
                //}

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["QuestionarioValutazione"]))
                    hValutazione.Value = ConfigurationManager.AppSettings["QuestionarioValutazione"];

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LinkRemedy"]))
                    hRemedy.Value = ConfigurationManager.AppSettings["LinkRemedy"];
            }
        }

        protected void btnInviaSegnalazione_Click(object sender, EventArgs e)
        {
            ValorizzaSegnalazione();
            PresenterInvioSegnalazione presenter = new PresenterInvioSegnalazione();
            presenter.InvioSegnalazione(this);
            if (this.HasError)
            {
                hEsito.Value = this.ErrorMessage;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenModalDialog", "ShowSegnalazione();", true);
            }
            else
            {
                lblEsitoSegnalazione.Text = "Segnalazione inviata correttamente";
                hEsito.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenModalDialog", "ShowEsitoSegnalazione();", true);
            }
        }

        private void ValorizzaSegnalazione()
        {
            string procedura = string.Empty;
            if (Session["Ruolo"] != null)
                TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            switch (TipoAppRuolo)
            {
                case UtilityTipoAppartenenza.AGO:
                    procedura = "AGO";
                    break;
                case UtilityTipoAppartenenza.FS:
                    procedura = "FONDI";
                    break;
                case UtilityTipoAppartenenza.CI:
                    procedura = "CONVENZIONI";
                    break;
            }

            this.InvioSegnalazione = new AreaInvioSegnalazione();
            this.InvioSegnalazione.Segnalazione = new Segnalazione();
            this.InvioSegnalazione.Segnalazione.NDomus = hNumeroDomus.Value;
            this.InvioSegnalazione.Segnalazione.MatricolaOperatore = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            this.InvioSegnalazione.Segnalazione.Procedura = procedura;
            this.InvioSegnalazione.Segnalazione.Tipologia = hTipologia.Value;
            this.InvioSegnalazione.Segnalazione.Messaggio = hMessaggio.Value;
            this.InvioSegnalazione.Segnalazione.RecapitoMittente = hTelefono.Value;
            this.InvioSegnalazione.Segnalazione.Destinatari = new string[2];
            this.InvioSegnalazione.Segnalazione.Destinatari[0] = hDestinatario.Value;
            this.InvioSegnalazione.Segnalazione.Destinatari[1] = this.InvioSegnalazione.Segnalazione.MatricolaOperatore + "@inps.it";
            this.InvioSegnalazione.Segnalazione.NomeMittente = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).FirstName;
            this.InvioSegnalazione.Segnalazione.CognomeMittente = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).LastName;
            this.InvioSegnalazione.Segnalazione.CodiceFiscale = hCodiceFiscale.Value.ToUpperInvariant();
            this.InvioSegnalazione.Segnalazione.Categoria = hCategoria.Value;
            this.InvioSegnalazione.Segnalazione.Sede = hSede.Value;
            this.InvioSegnalazione.Segnalazione.Certificato = hCertificato.Value;
            this.InvioSegnalazione.Segnalazione.CodiceErrore = hCodiceErrore.Value.ToUpperInvariant();
            this.InvioSegnalazione.Segnalazione.tipoApp = TipoAppRuolo;
            this.InvioSegnalazione.Segnalazione.DecorrenzaPensione = !string.IsNullOrEmpty(hDecorrenzaPensione.Value) ? DateTime.Parse(hDecorrenzaPensione.Value, CultureInfo.GetCultureInfo("it-IT")) : (DateTime?)null;
        }

        private void LoadDdl()
        {
            switch (TipoAppRuolo)
            {
                case UtilityTipoAppartenenza.AGO:
                    CodeUtility.SetValueDdl(ddlDestinatario, "AGO", "Supporto.IVS@inps.it");
                    break;
                case UtilityTipoAppartenenza.FS:
                    CodeUtility.SetValueDdl(ddlDestinatario, "FONDI", "ReingFondiSpeciali@inps.it");
                    break;
                case UtilityTipoAppartenenza.CI:
                    CodeUtility.SetValueDdl(ddlDestinatario, "CONVENZIONI", "Supporto.IVS@inps.it");
                    break;
            }
        }

        public void ValorizzaHiddenField(string page)
        {
            switch (page)
            {
                case "ProcedureOperatore":
                    hPath.Value = string.Empty;
                    break;
                case "Liquidazione":
                    hPath.Value = "../";
                    break;
            }
        }
    }
}