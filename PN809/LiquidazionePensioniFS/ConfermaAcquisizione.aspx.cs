using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System.Configuration;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class ConfermaAcquisizione : CustomBasePage, IRicercaPosizione
    {
        #region IRicercaPosizione
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

        #endregion IRicercaPosizione


        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            InizializzaPnlRicercaDanteCausa();

            if (!IsPostBack)
            {
                ValorizzaEtichette();

                //con il query string gestisco la visualizzazione del pulsante "Torna alle posizioni trovate". Questo pulsante è visibile se effettuo una ricerca per C.F. o per dati anagrafici
                if (Server.HtmlEncode(Request.QueryString["Posizioni"]) != null)
                {
                    if (Server.HtmlEncode(Request.QueryString["Posizioni"]) == "true")
                        btnTornaPosizioni.Visible = true;
                }

                //con il query string gestisco la visualizzazione del pulsante "Consulta". Questo pulsante è visibile se la domanda ricercata è una riapertura ed è presente a DB la prima liquidata calcolata
                if (Server.HtmlEncode(Request.QueryString["Consulta"]) != null)
                {
                    if (Server.HtmlEncode(Request.QueryString["Consulta"]) == "true")
                    {
                        btnConsultazione.Visible = true;
                        btnConsultazione.Text = "Consulta la PL";
                        btnpopup.Text = "Continua con la TRF";
                    }
                }
            }
        }

        private void ValorizzaEtichette()
        {
            try
            {

                AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

                lblNumeroDomanda.Text = lblNumeroDomandaNew.Text = Domanda.NumeroDomanda;
                lblCategoria.Text = lblCategoriaNew.Text = Domanda.Categoria;
                lblCognome.Text = lblCognomeNew.Text = Anagrafica.Cognome;
                lblNome.Text = lblNomeNew.Text = Anagrafica.Nome;
                lblCodiceFiscale.Text = lblCodiceFiscaleNew.Text = Anagrafica.CodiceFiscale;
                lblGruppo.Text = lblGruppoNew.Text = Domanda.CodGruppo;
                lblProdotto.Text = lblProdottoNew.Text = Domanda.CodProdotto;
                lblTipo.Text = lblTipoNew.Text = Domanda.CodTipo;
                lblDescrizione.Text = lblDescrizioneNew.Text = Domanda.DescrizioneIstanza;
                HDecorrenzaFinestra.Value = Domanda.DecorrenzaFinestra;


                if (Domanda.CodGruppo == "0031" && Domanda.IsDomandaENPALS)
                    trInformativaRicEnpals.Visible = true;

                if (Domanda.Categoria != null && Domanda.Categoria.Trim() == "INDCOM" && Domanda.CodGruppo != "0031" && !Domanda.IsDomandaRiapertura)
                {
                    if (Utility.IsDomandaINDCOM175(Domanda.Categoria, Domanda.CodTipo))
                    {
                        Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                        string controlloDinamico = string.Empty;
                        presenter.GetControlloDinamicoByNomeControllo("DataCalcoloDefinitivoINDCOM", out controlloDinamico);
                        DateTime? dataCalcoloDefinitivoINDCOM = Utility.DataFromString(controlloDinamico, Utility.FormatoData.AAAAmmGG);


                        if (Domanda.DataPresentazionePreAcquisizione.HasValue && dataCalcoloDefinitivoINDCOM.HasValue && Utility.DataStrettamenteSuccessivaA(Domanda.DataPresentazionePreAcquisizione.Value, dataCalcoloDefinitivoINDCOM.Value))
                        {
                            lblInformativaIndcom.Text = string.Format("Le INDCOM con data domanda successiva al {0:dd/MM/yyyy} sono sospese. Non è possibile procedere al calcolo definitivo della prestazione in attesa del rifinanziamento del fondo per la razionalizzazione della rete commerciale", dataCalcoloDefinitivoINDCOM.Value);
                            trINDCOM.Visible = true;
                        }
                    }
                }


                if (Domanda.SiglaCategoriaPensione == null && Domanda.SedePensione == null && Domanda.CertificatoPensione == null)
                    trChiavePensione.Visible = false;
                else
                {
                    trChiavePensione.Visible = true;
                    lblChiavePensione.Text = string.Concat(Domanda.SiglaCategoriaPensione, " - ", Domanda.SedePensione, " - ", Domanda.CertificatoPensione).ToUpperInvariant();
                    if (Domanda.CodGruppo == "0031")
                        aVisualizzaPensione.Visible = true;
                }

                if (new List<string> { "XB", "XD" }.Contains(Domanda.CodiceTipoRichiesta))
                {
                    ucAvviso.Tipo = TipoAvviso.Info;
                    ucAvviso.Visible = true;
                    switch (Domanda.CodiceTipoRichiesta)
                    {
                        case "XB":
                            ucAvviso.Messaggio = "Per la seguente domanda è possibile procedere con la trattazione solo dopo che sia stata accertata la verifica dell’arco temporale attività gravosa";
                            break;
                        case "XD":
                            ucAvviso.Messaggio = "Per la seguente domanda è possibile procedere con la trattazione solo dopo che sia stata accertata la verifica dell’arco temporale attività usurante";
                            break;
                    }
                }

                if (Domanda.CodTipo == "0190" && (Domanda.CodiceTipoRichiesta == "KX" || Domanda.CodiceTipoRichiesta == "KZ" || Domanda.CodiceTipoRichiesta == "KV"))
                    lblOpzDonna2023.Visible = true;

                if (Session["URLDPI"] == null && !string.IsNullOrEmpty(Domanda.UrlDPI))
                    Session.Add("URLDPI", Domanda.UrlDPI);
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
        }

        private void InizializzaPnlRicercaDanteCausa()
        {
            if (pnlRicercaDanteCausa.Visible)
            {
                string nomeTema = "~/App_Themes/" + Page.Theme;
                imgIcon.ImageUrl = nomeTema + "/Images/alert.png";
                imgIcon.AlternateText = "Attenzione";
                radioAnagrafica.Attributes.Add("onclick", "javascript:SetRadio(this)");
                radioAnagrafica.InputAttributes.Add("EnableClass", "onClassAnagrafica");
                radioCodiceFiscale.Attributes.Add("onclick", "javascript:SetRadio(this)");
                radioCodiceFiscale.InputAttributes.Add("EnableClass", "onClassCodiceFiscale");
                divTxtCodiceFiscale.Attributes.Add("onclick", "javascript:SetRadio(this)");
                divTxtCodiceFiscale.Attributes.Add("EnableClass", "onClassCodiceFiscale");
                divTxtCognome.Attributes.Add("onclick", "javascript:SetRadio(this)");
                divTxtCognome.Attributes.Add("EnableClass", "onClassAnagrafica");
            }
        }

        protected void btnContinua_Click(object sender, CommandEventArgs e)
        {
            PresenterElaborazionePosizione presenterElaborazionePosizione = new PresenterElaborazionePosizione();
            AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            string errore = string.Empty;

            this.RicercaPosizione = new RicercaPosizione();

            this.RicercaPosizione.Selezione = Utility.TipoRicerca.NDomus;   //trovandomi ella pagina di conferma, so per certo che effettuerò una ricerca per NDomuns
            this.RicercaPosizione.Domanda = Domanda.NumeroDomanda;
            this.RicercaPosizione.Nome = Anagrafica.Nome;
            this.RicercaPosizione.Cognome = Anagrafica.Cognome;
            this.RicercaPosizione.CodiceFiscale = Anagrafica.CodiceFiscale;
            this.RicercaPosizione.DataNascita = Anagrafica.DataNascita.ToString();
            this.IsPaginaConferma = true;
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            bool isDomandaEnpals = false;
            string codiceCategoria = string.Empty;

            if (e.CommandArgument.ToString() == "Consulta")
                this.IsConsultazione = true;

            if (!RicercaManualeDanteCausa(out errore))
            {
                lblMsg.Text = errore;
                return;
            }

            Presenter.PresenterControlliDinamici presenterControlloDinamiciEnpals = new PresenterControlliDinamici();
            string controlloDinamicoGenerazioneCertificatoEnpals = string.Empty;
            Presenter.SvrLiquidazione.AreaEsito esitoCaricamentoControlloDinamicoEnpals = presenterControlloDinamiciEnpals.GetControlloDinamicoByNomeControllo("AbilitazioneGeneraCertificatoFascicoloENPALS", out controlloDinamicoGenerazioneCertificatoEnpals);



            presenterElaborazionePosizione.RicercaDomanda(this);        //Chiamata a RicercaDomanda
            //cambio sede domanda 
            if (this.IsConsultazione && !string.IsNullOrEmpty(this.SedeDiversa) && this.RicercaPosizione.Selezione == Utility.TipoRicerca.NDomus)
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
                if (IsRicercaManualeDA)
                {
                    pnlRicercaDanteCausa.Visible = true;
                    InizializzaPnlRicercaDanteCausa();
                    lblMsg.Text = ErrorMessage;
                }
                else
                {
                    if (Esito == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                        ucAvviso.Tipo = TipoAvviso.Ko;
                    else
                        ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = ErrorMessage;
                    if (ErrorMessage == "La domanda non risulta lavorata in Unicarpe, è necessario richiedere autorizzazione lavorazione manuale utilizzando il tasto “Richiesta Lavorazione Manuale”")
                    {
                        btnRichiestaLavorazioneManuale.Visible = true;
                    }
                }

                //ENG - Pensioni Ovunque: gestione nuovo pannello
                if (this.MostraPanelloMessBloccantePensioniOvunque)
                {
                    pnlPensioniOvunque.Visible = true;
                    trChiavePensione.Visible = false;
                    ValorizzaPannelloPensioniOvunque();
                }
                else
                {
                    pnlPensioniOvunque.Visible = false;
                    if (Domanda != null && Domanda.SiglaCategoriaPensione == null && Domanda.SedePensione == null && Domanda.CertificatoPensione == null)
                    {
                        trChiavePensione.Visible = false;
                    }
                    else
                    {
                        trChiavePensione.Visible = true;
                    }

                }
            }
            else
            {
                try
                {
                    AreaRispostaRiepilogo.DatiRiepilogoDomanda result = this.ElencoDomande.Find(
                        delegate(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
                        {
                            return domanda.NumeroDomanda == this.RicercaPosizione.Domanda;
                        }
                        );
                    Session["Domanda"] = result;
                    Session["Anagrafica"] = this.RiepilogoAnagrafica;
                    Session["EsitoCalcolo"] = this.EsitoCalcolo;
                    Session["MsgNonBloccante"] = ErrorMessage;
                    isDomandaEnpals = result.IsDomandaENPALS;
                    codiceCategoria = result.Categoria;

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
                    throw new INPS.DNA.DnaApplicationException("ConfermaAcquisizione, Errore in BtnContinua_Click" + ex);
                }

                if (isDomandaEnpals && esitoCaricamentoControlloDinamicoEnpals != null && esitoCaricamentoControlloDinamicoEnpals.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(controlloDinamicoGenerazioneCertificatoEnpals) && controlloDinamicoGenerazioneCertificatoEnpals.ToUpperInvariant() == "SI" && this.IsNuovoCertificatoGeneratoEnpals)
                {
                    HdnCodiceCategoria.Value = codiceCategoria;
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpGenerazioneCertificato", "<script>ShowPopUpGenerazioneCertificato();</script>", false);
                }
                else if(this.MostraPopupMemo239)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpMemo239", "<script>ShowPopUpMemo239();</script>", false);                    
                }
                else if (this.MostraPopupMemo312023)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpMemo312023", "<script>ShowPopUpMemo312023();</script>", false);
                }
                else
                    Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
            }
        }

        //ENG - Pensioni Ovunque: gestione nuovo pannello
        private void ValorizzaPannelloPensioniOvunque()
        {
            AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (domanda != null)
            {
                lblChiaveDomandaPensioniOvunque.Text = this.CodCategoriaPensione + " - " + this.SedePensioneGP1ALZ6 + " - " + (!String.IsNullOrEmpty(this.CertificatoInseguimentoPensione) ? this.CertificatoInseguimentoPensione : domanda.CertificatoPensione);
                lblSiglaCategoriaPensioniOvunque.Text = (!String.IsNullOrEmpty(domanda.Categoria)) ? domanda.Categoria.ToUpperInvariant() : "";
                lblSedeGestionePensioniOvunque.Text = this.SedePensioneGP1ALZ6;
            }
        }

        private bool RicercaManualeDanteCausa(out string errore)
        {
            errore = string.Empty;
            if (pnlRicercaDanteCausa.Visible)
            {
                if (!radioCodiceFiscale.Checked && !radioAnagrafica.Checked)
                {
                    errore = "Selezionare una tipologia di ricerca del Dante Causa";
                    return false;
                }

                this.RicercaDanteCausa = new RicercaPosizione();
                if (radioAnagrafica.Checked)                       //Ricerca per Anagrafica
                {
                    if (string.IsNullOrEmpty(txtNome.Text) || string.IsNullOrEmpty(txtCognome.Text) || string.IsNullOrEmpty(txtDataNascita.Text))
                    {
                        errore = "E' necessario inserire Nome, Cognome e Data di Nascita";
                        return false;
                    }
                    this.RicercaDanteCausa.Selezione = Utility.TipoRicerca.NDomusConRicercaDatiParzialiDA;
                    this.RicercaDanteCausa.Cognome = txtCognome.Text;
                    this.RicercaDanteCausa.Nome = txtNome.Text;
                    this.RicercaDanteCausa.DataNascita = txtDataNascita.Text;
                }
                else if (radioCodiceFiscale.Checked)            //Ricerca per Codice Fiscale
                {
                    if (string.IsNullOrEmpty(txtCodiceFiscale.Text))
                    {
                        errore = "E' necessario inserire il Codice Fiscale";
                        return false;
                    }
                    this.RicercaDanteCausa.Selezione = Utility.TipoRicerca.NDomusConRicercaCodiceFiscaleDA;
                    this.RicercaDanteCausa.CodiceFiscale = txtCodiceFiscale.Text.Trim();
                }
            }

            return true;
        }

        #region Cambio Sede Domanda
        public void btnConfermaPopUp_Click(object sender, CommandEventArgs args)
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

            btnContinua_Click(sender, args);
        }
        #endregion Cambio Sede Domanda

        public void btnConfermaMessaggioGenerazioneCertificato_Click(object sender, CommandEventArgs args)
        {
            Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
        }

        protected void btnRichiestaLavorazioneManuale_Click(object sender, EventArgs e)
        {
            AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            string messaggio = String.Empty;
            Int16 sede;
            Int16.TryParse(Domanda.Sede, out sede);
            PresenterLavorazioneManualeAutomatiche presenterLavorazioneManualeAutomatiche = new PresenterLavorazioneManualeAutomatiche();

            AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche lavorazioneManualeAutomatiche = new AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche();
            lavorazioneManualeAutomatiche.NDomus = Convert.ToInt64(Domanda.NumeroDomanda);
            lavorazioneManualeAutomatiche.SiglaCategoria = Domanda.Categoria;
            lavorazioneManualeAutomatiche.CodiceSede = sede;
            lavorazioneManualeAutomatiche.Gruppo = Domanda.CodGruppo;
            lavorazioneManualeAutomatiche.Prodotto = Domanda.CodProdotto;
            lavorazioneManualeAutomatiche.Tipo = Domanda.CodTipo;
            lavorazioneManualeAutomatiche.AutorizzazioneManuale = null;
            lavorazioneManualeAutomatiche.TipoApp = Domanda.TipoAppartenenza.ToString();
            lavorazioneManualeAutomatiche.DecorrenzaOriginaria = Domanda.DataAcquisizione;
            lavorazioneManualeAutomatiche.MatricolaUtente = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;

            Presenter.SvrLiquidazione.AreaEsito esito = presenterLavorazioneManualeAutomatiche.SalvaLavorazioneManualeAutomatiche(lavorazioneManualeAutomatiche, out messaggio);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
            {
                ucAvviso.Messaggio = "Richiesta di autorizzazione di lavorazione manuale";
                ucAvviso.Tipo = TipoAvviso.Ok;
                btnRichiestaLavorazioneManuale.Visible = false;
            }
            else
            {
                ucAvviso.Messaggio = messaggio;
                ucAvviso.Tipo = TipoAvviso.Ko;
                btnRichiestaLavorazioneManuale.Visible = true;
            }
        }

        public void btnConfermaPopupMemo239_Click(object sender, CommandEventArgs args)
        {
            Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
        }
        public void btnConfermaPopupMemo312023_Click(object sender, CommandEventArgs args)
        {
            Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
        }
    }
}
