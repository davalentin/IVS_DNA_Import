using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.DNA.Logging;
using INPS.DNA.Exceptions;
using INPS.DNA;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Configuration;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.DNA.Context;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class ElaborazionePosizione : CustomBasePage, IRicercaPosizione
    {

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

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            AbilitaPannelli();
            SvuotaSessioneRicerca();
        }

        private void AbilitaPannelli()
        {
            radioAnagrafica.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioAnagrafica.InputAttributes.Add("EnableClass", "onClassAnagrafica");
            radioCodiceFIscale.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioCodiceFIscale.InputAttributes.Add("EnableClass", "onClassCodiceFiscale");
            radioDomanda.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioDomanda.InputAttributes.Add("EnableClass", "onClassDomanda");
            divTxtNumeroDomanda.Attributes.Add("onclick", "javascript:SetRadio(this)");
            divTxtNumeroDomanda.Attributes.Add("EnableClass", "onClassDomanda");
            divTxtCodiceFiscale.Attributes.Add("onclick", "javascript:SetRadio(this)");
            divTxtCodiceFiscale.Attributes.Add("EnableClass", "onClassCodiceFiscale");
            divTxtCognome.Attributes.Add("onclick", "javascript:SetRadio(this)");
            divTxtCognome.Attributes.Add("EnableClass", "onClassAnagrafica");

        }

        protected void SvuotaSessioneRicerca()
        {
            Session.Remove("Pensioni");             //elenco pensioni trovate
            Session.Remove("Domande");              //elenco domande trovate
            Session.Remove("Anagrafica");           //anagrafica soggetto
            Session.Remove("TornaASinonimi");       //switch per UC da visualizzare
            Session.Remove("Nome");
            Session.Remove("Cognome");
            Session.Remove("CF");
            Session.Remove("TipoRicerca");
            Session.Remove("Domanda");              //riepilogo domanda
            Session.Remove("EsitoCalcolo");
            Session.Remove("Semaforo");             //semafori quadri
            Session.Remove("Sinonimi");             //elenco sinonimi
            Session.Remove("DatiPensione");         //dati pensione 
            Session.Remove("Criteri");              //eliminazione criterio in visualizzazione stato pratiche
            Session.Remove("Lavorabile");           //flag per determinare se la pensione è lavorabile
        }


        protected void btnRicerca_Click(object sender, EventArgs e)
        {
            PresenterElaborazionePosizione presenterElaborazionePosizione = new PresenterElaborazionePosizione();

            this.RicercaPosizione = new RicercaPosizione();


            if (!string.IsNullOrEmpty(HdnNDom.Value)) //Ricerca per Ndomus in seguito a cambio sede
            {
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.NDomus;
                this.RicercaPosizione.Domanda = HdnNDom.Value;
                this.IsPaginaConferma = false;
                HdnNDom.Value = string.Empty;
                HdnSede.Value = string.Empty;
            }
            else if (radioDomanda.Checked)                       //Ricerca per numero domanda
            {
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.NDomus;
                this.RicercaPosizione.Domanda = txtNumeroDomanda.Text;
                this.IsPaginaConferma = false;
            }
            else if (radioCodiceFIscale.Checked)            //Ricerca per Codice Fiscale
            {
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.CodiceFiscale;
                this.RicercaPosizione.CodiceFiscale = txtCodiceFiscale.Text.Trim();
            }
            else //if(radioAnagrafica.Checked)
            {                                          //Ricerca per anagrafica

                this.RicercaPosizione.Selezione = Utility.TipoRicerca.Anagrafica;
                this.RicercaPosizione.Cognome = txtCognome.Text;
                this.RicercaPosizione.Nome = txtNome.Text;
                this.RicercaPosizione.DataNascita = txtDataNascita.Text;
            }
            presenterElaborazionePosizione.RicercaDomanda(this);        //Chiamata a RicercaDomanda

            //cambio sede domanda 
            if (!string.IsNullOrEmpty(this.SedeDiversa) && this.RicercaPosizione.Selezione == Utility.TipoRicerca.NDomus)
            {
                if (CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
                {
                    HdnSede.Value = this.SedeDiversa;
                    HdnNDom.Value = this.RicercaPosizione.Domanda;
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpSede", "<script>ShowPopUpSede();</script>", false);
                    return;
                }
                else
                {
                    PresenterSedi presenter = new PresenterSedi();
                    List<string> sediAbilitate = presenter.GetOfficeAspnCodeAbilitati(INPS.DNA.Security.DnaPrincipal.Current.OfficeForCurrentApplication(((Ruoli)Session["Ruolo"]).ToString()).ToList<string>());
                    if (sediAbilitate.Contains(this.SedeDiversa))
                    {
                        HdnSede.Value = this.SedeDiversa;
                        HdnNDom.Value = this.RicercaPosizione.Domanda;
                        ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpSede", "<script>ShowPopUpSede();</script>", false);
                        return;
                    }
                }
            }
            if (HasError)
            {
                if (Esito == AreaEsito.TipoEsito.KO)
                {
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Ko;
                }
                else
                {
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Warning;
                }

                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ErrorMessage;
            }
            else                    //OK
            {
                ucAvviso.Visible = false;

                /*Inserimento dati in sessione*/
                Session["TipoRicerca"] = this.RicercaPosizione.Selezione;
                Session["Nome"] = this.RicercaPosizione.Nome;
                Session["Cognome"] = this.RicercaPosizione.Cognome;
                Session["CF"] = this.RicercaPosizione.CodiceFiscale;
                Session["Anagrafica"] = this.RiepilogoAnagrafica;
                Session["Domande"] = this.ElencoDomande;
                Session["Pensioni"] = this.ElencoPensioni;
                if (this.ElencoSinonimi != null)
                    Session["Sinonimi"] = this.ElencoSinonimi;
                else
                    Session["InfoErroreWebDom"] = this.ErrorMessage;

                if (this.RicercaPosizione.Selezione == Utility.TipoRicerca.NDomus)
                {
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
                        throw new INPS.DNA.DnaApplicationException("ElaborazionePosizione, Errore  in BtnRicerca_Click" + ex);
                    }

                    if (this.IsDomandaDB && !this.IsDomandaCalcolataProvvisoria)
                        Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
                    else
                        if (this.IsDomandaCalcolataProvvisoria)
                            Response.Redirect("ConfermaAcquisizione.aspx?Consulta=true", false); //con il query string gestisco la visualizzazione del pulsante "Consulta".
                        else
                            Response.Redirect("ConfermaAcquisizione.aspx", false);
                }
                else
                    Response.Redirect("RisultatoRicercaElaborazione.aspx", false);
            }
        }


        #region Cambio Sede Domanda
        public void btnConfermaPopUp_Click(object sender, EventArgs args)
        {
            string errori = string.Empty;
            string nuovaSede = HdnSede.Value;

            if (!CodeUtility.ChangeSede((Ruoli)Session["Ruolo"], nuovaSede, false, out errori))
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = errori;
                return;
            }
            //ricaricamento controllo UCChangeAzienda master page
            this.ReloadUChangeSede();
            UCChangeSede uc = (UCChangeSede)((UCIntestazione)Master.FindControl("UCIntestazione")).FindControl("ucChangeSede");
            if (uc != null) uc.ReloadControl();

            btnRicerca_Click(sender, args);
        }

        #endregion Cambio Sede Domanda
    }
}
