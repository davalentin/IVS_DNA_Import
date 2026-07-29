using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using System.Configuration;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Any, CheckSequenceOnPostBack = false)]
    public partial class Previsan : BasePage, ISedi, IRicercaPosizione
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

        #region IElaborazionePosizione
        public RicercaPosizione RicercaPosizione { get; set; }
        public RicercaPosizione RicercaDanteCausa { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ElencoDomande { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ElencoPensioni { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ElencoSinonimi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica RiepilogoAnagrafica { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiEsitoCalcolo EsitoCalcolo { get; set; }
        public Presenter.SvrLiquidazione.AreaEsito.TipoEsito Esito { get; set; }
        public UtilityTipoAppartenenza TipoAppRuolo { get; set; }
        public UtilityRuolo Ruolo { get; set; }
        public bool IsDomandaDB { get; set; }
        public bool IsPaginaConferma { get; set; }
        public bool IsDomandaCalcolataProvvisoria { get; set; }
        public bool IsConsultazione { get; set; }
        public string SedeDiversa { get; set; }
        public bool IsRicercaManualeDA { get; set; }
        public bool IsNuovoCertificatoGeneratoEnpals { get; set; }
        //ENG - Pensioni Ovunque: gestione nuovo pannello
        public bool MostraPanelloMessBloccantePensioniOvunque { get; set; }
        public string SedePensioneGP1ALZ6 { get; set; }
        public string CodCategoriaPensione { get; set; }
        public string CertificatoInseguimentoPensione { get; set; }
        //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
        public bool IsPaginaVisualizzazioneStatoPratiche { get; set; }
        //ENG - Gestione Popup Memo 239
        public bool MostraPopupMemo239 { get; set; }
        //ENG - Gestione Popup Memo 31/2023
        public bool MostraPopupMemo312023 { get; set; }
        #endregion IElaborazionePosizione

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Add("SessionAlive", true);
                Session.Add("Previsan", true);

                if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["Sede"]) || string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["CentroOperativo"]) ||
                    string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["NumDomus"]))
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Sede e/o Numero Domanda non presenti";
                    btnRicercaNDomus.Enabled = false;
                }
                else
                {
                    this.Sede = Crypt.Decrypt(HttpContext.Current.Request.QueryString["Sede"]) + Crypt.Decrypt(HttpContext.Current.Request.QueryString["CentroOperativo"]);
                    lblDomus.Text = Crypt.Decrypt(HttpContext.Current.Request.QueryString["NumDomus"]);

                    if (!string.IsNullOrEmpty(lblDomus.Text))
                        lblDomus.Text = lblDomus.Text.Trim();

                    if (Session["Ruolo"] == null)
                    {
                        if (CodeUtility.IsMultiRuolo())
                        {
                            Session.Add("UrlPrevisan", HttpContext.Current.Request.Url.PathAndQuery);
                            Response.Redirect("~/SceltaRuolo.aspx", true);
                        }
                        else
                        {
                            Dictionary<string, string> ruoliAbilitati = CodeUtility.GetRuoliAbilitati();
                            if (ruoliAbilitati != null && ruoliAbilitati.Count > 0)
                                Session["Ruolo"] = (Ruoli)Enum.Parse(typeof(Ruoli), ruoliAbilitati.FirstOrDefault().Key);
                        }
                    }

                    if (Session["Ruolo"] == null)
                    {
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Visible = true;
                        ucAvviso.Messaggio = "Ruolo utente non valido";
                        btnRicercaNDomus.Enabled = false;
                    }
                    else
                    {
                        if (!ControllaValiditaDati())
                        {
                            btnRicercaNDomus.Enabled = false;
                            ucAvviso.Tipo = TipoAvviso.Warning;
                            ucAvviso.Visible = true;
                            ucAvviso.Messaggio = ErrorMessage;
                            Session.Remove("Previsan");
                        }
                    }
                }

                if (SelectedOffice != null)
                    lblSede.Text = SelectedOffice.AspnCode + " - " + SelectedOffice.City;

                if (ConfigurationManager.AppSettings["PREVISAN-CLIENT"] != null &&
                    ConfigurationManager.AppSettings["PREVISAN-CLIENT"] == "1")
                {
                    pnlWelcome.Visible = false;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenModalDialog", "<script type='text/javascript'>document.getElementById('" + btnRicercaNDomus.ClientID + "').click() ;</script>", false);
                    btnRicercaNDomus.Style.Add("display", "none");
                    btnRicercaNDomus.Attributes.Add("onclick", "javascript:BlockUI()");
                }
                else if (ConfigurationManager.AppSettings["PREVISAN-CLIENT"] != null &&
                    ConfigurationManager.AppSettings["PREVISAN-CLIENT"] == "2")
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenModalDialog", "<script type='text/javascript'>document.getElementById('" + btnRicercaNDomus.ClientID + "').click() ;</script>", false);
                    btnRicercaNDomus.Style.Add("display", "none");
                    lblMsgCortesia.Text = "Attendere. Elaborazione in corso...";
                    Loading.Visible = true;
                }
                else
                {
                    btnRicercaNDomus.Attributes.Add("onclick", "javascript:BlockUI()");
                }

                if (!string.IsNullOrEmpty(ucAvviso.Messaggio))
                    lblMsgCortesia.Visible = false;
            }
        }

        private void RicercaNDomus(string numeroDomanda)
        {
            PresenterElaborazionePosizione presenterElaborazionePosizione = new PresenterElaborazionePosizione();
            this.RicercaPosizione = new RicercaPosizione();

            this.RicercaPosizione.Selezione = Utility.TipoRicerca.NDomus;
            this.RicercaPosizione.Domanda = numeroDomanda;
            this.IsPaginaConferma = false;
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            presenterElaborazionePosizione.RicercaDomanda(this);        //Chiamata a RicercaDomanda
            if (HasError)
            {
                if (Esito == AreaEsito.TipoEsito.KO)
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Ko;
                else
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Warning;

                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ErrorMessage;

                lblMsgCortesia.Visible = false;
            }
            else
            {
                ucAvviso.Visible = false;

                Session["TipoRicerca"] = this.RicercaPosizione.Selezione;
                Session["Nome"] = this.RicercaPosizione.Nome;
                Session["Cognome"] = this.RicercaPosizione.Cognome;
                Session["CF"] = this.RicercaPosizione.CodiceFiscale;
                Session["Anagrafica"] = this.RiepilogoAnagrafica;
                Session["Domande"] = this.ElencoDomande;
                Session["Pensioni"] = this.ElencoPensioni;
                if (this.ElencoSinonimi != null)
                    Session["Sinonimi"] = this.ElencoSinonimi;

                try
                {
                    Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda result = this.ElencoDomande.Find(
                        delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
                        {
                            return domanda.NumeroDomanda == this.RicercaPosizione.Domanda;
                        }
                        );
                    if (!Utility.ControlloSedi(short.Parse(result.Sede), short.Parse(result.CentroOperativo)))
                    {
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Visible = true;
                        ucAvviso.Messaggio = "La sede dell'operatore non coincide con la sede della domanda selezionata (" +
                            result.Sede.PadLeft(4, '0') + result.CentroOperativo.PadLeft(2, '0') + ").";
                        return;
                    }
                    else if (result.TipoAppartenenza != (AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)TipoAppRuolo)
                    {
                        ucAvviso.Visible = true;
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = "Ruolo Utente non abilitato alla lavorazione della domanda.";
                        return;
                    }
                    else
                    {
                        ucAvviso.Visible = false;
                        ucAvviso.Messaggio = "";
                    }
                    Session["Domanda"] = result;
                    Session["Anagrafica"] = this.RiepilogoAnagrafica;
                    Session["EsitoCalcolo"] = this.EsitoCalcolo;

                    if (result.Stato == "DA ACQUISIRE")
                        Presenter.LogSicurezza.ScritturaLog(result.NumeroDomanda, result.TipoAppartenenza,
                            int.Parse(ConfigurationManager.AppSettings["IDEVENTO-ACQUISIZIONE"]), HttpContext.Current.Request.UserHostAddress, 0, string.Empty,
                            this.RiepilogoAnagrafica.CodiceFiscale, string.Empty);
                    else
                        Presenter.LogSicurezza.ScritturaLog(result.NumeroDomanda, result.TipoAppartenenza,
                            int.Parse(ConfigurationManager.AppSettings["IDEVENTO-CONSULTAZIONE"]), HttpContext.Current.Request.UserHostAddress, 0, string.Empty,
                            this.RiepilogoAnagrafica.CodiceFiscale, string.Empty);
                }
                catch (DnaApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new INPS.DNA.DnaApplicationException("Previsan, Errore  in btnRicercaNDomus_Click" + ex);
                }

                if (this.IsDomandaDB && !this.IsDomandaCalcolataProvvisoria)
                    Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
                else
                    if (this.IsDomandaCalcolataProvvisoria)
                        Response.Redirect("ConfermaAcquisizione.aspx?Consulta=true", false); //con il query string gestisco la visualizzazione del pulsante "Consulta".
                    else
                        Response.Redirect("ConfermaAcquisizione.aspx", false);
            }

            Session.Remove("Previsan");
        }

        protected void btnRicercaNDomus_Click(object sender, EventArgs e)
        {
            RicercaNDomus(lblDomus.Text);
        }

        private bool ControllaValiditaDati()
        {
            if (!checkSedi())
                return false;

            if (string.IsNullOrEmpty(lblDomus.Text))
            {
                ErrorMessage = "Numero Domanda non presente";
                HasError = true;
                return false;
            }

            if (lblDomus.Text.Length != 13)
            {
                ErrorMessage = "Lunghezza per Numero Domanda non corretto (il campo deve essere lungo 13)";
                HasError = true;
                return false;
            }

            long nDomus = 0;
            long.TryParse(lblDomus.Text, out nDomus);
            if (nDomus == 0)
            {
                ErrorMessage = "Formato non corretto per Numero Domanda";
                HasError = true;
                return false;
            }

            return true;
        }

        private bool checkSedi()
        {
            if (CodeUtility.IsAmministratore(Session["Ruolo"]))
                SelectedOffice = INPS.DNA.Context.OfficeList.OfficeFullList.FirstOrDefault(x => x.AspnCode == Sede);
            else
            {
                PresenterSedi presenter = new PresenterSedi();
                if (Session["Ruolo"] != null)
                {
                    SediAbilitate = presenter.GetOfficeAspnCodeAbilitati(INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(((Ruoli)Session["Ruolo"]).ToString()).ToList<string>());
                }
                if (SediAbilitate != null && SediAbilitate.Count > 0)
                {
                    if (SediAbilitate.Contains(Sede))
                        SelectedOffice = INPS.DNA.Context.OfficeList.OfficeFullList.FirstOrDefault(x => x.AspnCode == Sede);
                    else
                        return SetErrorMessage("La sede selezionata (" + this.Sede + ") non è abilitata per il ruolo di " + Utility.GetDescription((Ruoli)Session["Ruolo"]));
                }
                else if (!string.IsNullOrEmpty(((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode))
                {
                    SediAbilitate = new List<string>() { ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode };
                    if (SediAbilitate != null && SediAbilitate.Count > 0)
                    {
                        if (SediAbilitate.Contains(Sede))
                            SelectedOffice = INPS.DNA.Context.OfficeList.OfficeFullList.FirstOrDefault(x => x.AspnCode == Sede);
                        else
                            return SetErrorMessage("La sede selezionata (" + this.Sede + ") non è abilitata per il ruolo di " + Utility.GetDescription((Ruoli)Session["Ruolo"]));
                    }
                }
                else
                    return SetErrorMessage("Nessuna sede abilitata");
            }

            if (SelectedOffice == null)
                return SetErrorMessage("La sede selezionata non è valida");

            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = SelectedOffice;

            return true;
        }

        private bool SetErrorMessage(string messaggio)
        {
            ErrorMessage = messaggio;
            HasError = true;
            btnRicercaNDomus.Enabled = false;
            return false;
        }
    }
}