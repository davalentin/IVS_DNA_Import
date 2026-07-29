using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System.Text.RegularExpressions;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class PosizioneSelezionata : CustomBasePage, IInfoLiquidazione, ITitolarePensione
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValorizzaCampiPosizioneSelezionata();
                ValorizzaInfoLiquidazione(ucInfoLiquidazione);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Session["MsgNonBloccante"] != null)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = Session["MsgNonBloccante"].ToString();
                Session["MsgNonBloccante"] = null;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            CheckIsDecorrenzaValida();

            string nameReport;

            if (Server.HtmlEncode(Request.QueryString["PresaInCarico"]) != null)
            {
                nameReport = this.Request.QueryString["PresaInCarico"];
                if (nameReport == "S")
                {
                    ucAvviso.Tipo = TipoAvviso.Info;
                    ucAvviso.Visible = true;

                    Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    ucAvviso.Messaggio = "Domanda presa in carico dalla matricola " + Domanda.MatricolaUtenteAcquisizione;
                }
            }
        }


        private void DomandaNonLavorabile()
        {
            Session["NonLavorabile"] = true;
        }

        public event EventHandler PosizioneNonLavorabile;

        protected void RaisePosizioneNonLavorabile(object sender, EventArgs e)
        {
            if (PosizioneNonLavorabile != null)
                PosizioneNonLavorabile(sender, e);
        }


        private void ValorizzaCampiPosizioneSelezionata()
        {
            try
            {
                AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                AreaRispostaRiepilogo.DatiEsitoCalcolo EsitoCalcolo = (AreaRispostaRiepilogo.DatiEsitoCalcolo)Session["EsitoCalcolo"];

                this.domanda = Domanda;
                this.GetDatiPensione(this);

                //AreaTitolare titolare = new AreaTitolare();
                //PresenterTitolare presenterTitolare = new PresenterTitolare();
                //titolare = presenterTitolare.CaricaTitolare(this);
                //Session["DatiPensione"] = (AreaTitolare.DatiPensione)titolare.Pensione;
                if (Session["URLDPI"] == null && !string.IsNullOrEmpty(Domanda.UrlDPI))
                    Session.Add("URLDPI", Domanda.UrlDPI);

                ////////////////////////////////////////////////////////////////////////
                string msg = "Disponibile in visualizzazione.";
                if (!((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).IsMatchMatricola &&
                    ((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
                    msg = "E' comunque possibile lavorare la domanda.";

                //Gestione della visibilità del pannelo di alert relativo alla presa in carico della domanda
                if (this.domanda.Stato.Trim().ToUpperInvariant() != "CALCOLATA")
                {
                    string msg2 = " <BR/>Per prenderla in carico fare click sulla voce di menù 'Presa In Carico'";
                    pnlPresaInCarico.Visible = !this.domanda.IsMatchMatricola;
                    if (this.domanda.TipoAutomazione == null)
                        lblMsg.Text = "Domanda in carico alla matricola " + this.domanda.MatricolaUtenteAcquisizione + ". " + msg + msg2;
                    else
                        lblMsg.Text = "Domanda in carico al processo automatizzato." + msg + msg2;
                }
                ////////////////////////////////////////////////////////////////////////

                //ENG - Implementazione Meta Processo
                string controlloDinamicoSbloccaMetaProcesso = string.Empty;
                if (ViewState["SbloccaMetaProcesso"] != null)
                    controlloDinamicoSbloccaMetaProcesso = (string)ViewState["SbloccaMetaProcesso"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out controlloDinamicoSbloccaMetaProcesso);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["SbloccaMetaProcesso"] = controlloDinamicoSbloccaMetaProcesso;
                }

                lblNumeroDomanda.Text = Domanda.NumeroDomanda;
                lblCodiceFiscale.Text = Anagrafica.CodiceFiscale;
                lblCategoria.Text = Domanda.Categoria;
                if (ViewState["SbloccaMetaProcesso"] != null && ViewState["SbloccaMetaProcesso"].ToString() == "SI" && Domanda.CodiceSedeLavorazione.HasValue &&
                    Domanda.CodiceSedeLavorazione.Value > 0 && (Domanda.Tipo == "RIC" || Domanda.Tipo == "TRF"))
                {
                    lblSede.Text = Domanda.CodiceSedeLavorazione.ToString().PadLeft(4, '0') + "-" + (!String.IsNullOrEmpty(Domanda.CentroOperativo) ? Domanda.CentroOperativo.PadLeft(2, '0') : "00");
                    lblSedeText.InnerText = "Sede Lavorazione:";
                }
                else
                    lblSede.Text = Domanda.Sede;
                lblCertificato.Text = Domanda.Certificato;
                lblCognome.Text = Anagrafica.Cognome;
                lblNome.Text = Anagrafica.Nome;
                if (Anagrafica.Sesso != null)
                    lblSesso.Text = Anagrafica.Sesso.ToString();
                lblDataNascita.Text = String.Format("{0:dd/MM/yyyy}", Anagrafica.DataNascita);
                lblComuneNascita.Text = Anagrafica.ComuneNascita;
                lblProvinciaStatoNascita.Text = Anagrafica.ProvinciaNascita;
                lblIndirizzo.Text = Anagrafica.Indirizzo;
                lblNCivico.Text = Anagrafica.NumeroCivico;
                lblCAP.Text = Anagrafica.Cap;
                lblComuneStatoResidenza.Text = Anagrafica.ComuneResidenza;
                lblProvinciaResidenza.Text = Anagrafica.ProvinciaResidenza;
                lblFrazione.Text = Anagrafica.FrazioneResidenza;
                lblTipoDomanda.Text = Domanda.Tipo;

                if (Anagrafica.ResidenzaEstero.HasValue && Anagrafica.ResidenzaEstero.Value)
                {
                    lblResidenteEstero.Text = "SI";
                    pnlFrazioneEstero.Visible = true;
                }
                else if (Anagrafica.ResidenzaEstero.HasValue && !Anagrafica.ResidenzaEstero.Value)
                    lblResidenteEstero.Text = "NO";
                else
                    lblResidenteEstero.Text = "NON PRESENTE";

                if (Anagrafica.DataMorte.HasValue)
                {
                    pnlDataMorte.Visible = true;
                    lblDataMorte.Text = String.Format("{0:dd/MM/yyyy}", Anagrafica.DataMorte);
                }

                if (Anagrafica.IsNatoInItalia == true)
                    etichettaProvinciaStatoNascita.Text = "Provincia di Nascita:";
                else
                    etichettaProvinciaStatoNascita.Text = "Stato di Nascita:";
                if (Anagrafica.IsResidenteInItalia == true)
                    etichettaComuneStatoResidenza.Text = "Comune di Residenza:";
                else
                    etichettaComuneStatoResidenza.Text = "Stato di Residenza:";
                CheckDomandaLavorabile(Domanda.Categoria);

                if (Session["Criteri"] == null)
                {
                    divPulsantiRicerca.Visible = true;
                    divPulsantiStatoPratica.Visible = false;
                }
                else
                {
                    divPulsantiStatoPratica.Visible = true;
                    divPulsantiRicerca.Visible = false;
                }

                if (EsitoCalcolo != null && !String.IsNullOrEmpty(Domanda.Stato) &&
                    Domanda.Stato.Trim().ToUpperInvariant() != "IN ACQUISIZIONE" && Domanda.Stato.Trim().ToUpperInvariant() != "DA CALCOLARE")
                {
                    lblEsitoCalcolo.Text = EsitoCalcolo.Esito;
                    lblDettaglioEsitoCalcolo.Text = EsitoCalcolo.DettaglioEsito;

                    if (!String.IsNullOrEmpty(Domanda.Stato) &&
                        (Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLATA" || Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO WEBDOM" ||
                        Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO FELPE" || Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO ONERI" ||
                        Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO SAI" || Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO SIN" || Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO TOTAL" ||
                        Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO NO TOT" || Domanda.Stato.Trim().ToUpperInvariant() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoStazLavoro) || Domanda.Stato.Trim().ToUpperInvariant() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoNoteDebito) || Domanda.Stato.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNo6Scatti) || Domanda.Stato.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoEquoInd) || Domanda.Stato.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoIndennSpec)))
                    {
                        lblMessCalcoloDefinitivo.Text = msg = "La stampa contenente il libretto di pensione è consultabile dalla procedura StampeWeb";

                        lblCertificatoEsitoTitolo.Visible = true;
                        lblCertificatoEsitoValore.Visible = true;
                        lblCertificatoEsitoValore.Text = Domanda.Certificato != null ? Domanda.Certificato.PadLeft(8, '0') : string.Empty;
                    }
                    else
                    {
                        lblCertificatoEsitoTitolo.Visible = false;
                        lblCertificatoEsitoValore.Text = "";
                        lblCertificatoEsitoValore.Visible = false;
                    }
                    pnlEsitoCalcolo.Visible = true;
                }
                else
                {
                    lblEsitoCalcolo.Text = "";
                    lblDettaglioEsitoCalcolo.Text = "";
                    lblCertificatoEsitoTitolo.Visible = false;
                    lblCertificatoEsitoValore.Text = "";
                    lblCertificatoEsitoValore.Visible = false;
                    pnlEsitoCalcolo.Visible = false;
                }

                if (Domanda.GP1ALB1 == 2)
                    pnlInformativaSupplementiRic.Visible = true;

                if (ConfigurationManager.AppSettings["GestioneSCRIWO"] == "SI" && (Session["Scriwo"] as bool?).GetValueOrDefault())
                {
                    if (this.domanda.IsMatchMatricola && string.IsNullOrEmpty(ucAvviso.Messaggio))
                    {
                        if (!String.IsNullOrEmpty(Domanda.Stato) &&
                            (Domanda.Stato.Trim().ToUpperInvariant() == "DA CALCOLARE" || Domanda.Stato.Trim().ToUpperInvariant() == "SCARTO DA CALCOLO" || Domanda.Stato.Trim().ToUpperInvariant() == "CALCOLO VERIFY" ||
                            Domanda.Stato.Trim().ToUpperInvariant() == "SCARTO VERIFY"))
                        {
                            Response.Redirect("InvioCalcolo.aspx", false);
                        }
                        else
                        {
                            Response.Redirect("Titolare.aspx", false);
                        }
                    }
                    Session.Remove("Scriwo");
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PosizioneSelezionata, Errore nel metodo ValorizzaCampiPosizioneSelezionata " + ex);
            }
        }

        private void CheckIsDecorrenzaValida()
        {
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];


            this.domanda = Domanda;
            this.GetDatiPensione(this);
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            //PresenterTitolare presenterTitolare = new PresenterTitolare();
            //AreaTitolare Tit = new AreaTitolare();
            //Tit = presenterTitolare.CaricaTitolare(this);
            //Session["DatiPensione"] = Tit.Pensione;


            UCMenuLeft menu = (UCMenuLeft)this.Master.FindControl("UCMenu");
            menu.TitolarePensione = new AreaTitolare();
            menu.TitolarePensione.Pensione = datiPensione;

            //Usato per test disabilitazione domanda
            //Tit.Pensione.IsDecorrenzaValida = false;
            if (datiPensione.IsDecorrenzaValida == false)
            {

                ucAvviso.Tipo = TipoAvviso.Info;
                ucAvviso.Messaggio = "Decorrenza della pensione non ancora gestita dalla procedura reingegnerizzata";
                ucAvviso.Visible = true;
                Session["Lavorabile"] = false;
            }
            else if (datiPensione.IsDecorrenzaValida == true)
            {
                ucAvviso.Tipo = TipoAvviso.Info;
                ucAvviso.Messaggio = "";
                ucAvviso.Visible = false;
                Session["Lavorabile"] = true;
            }
        }


        private void CheckDomandaLavorabile(string categoria)
        {
            try
            {
                UCMenuLeft menu = (UCMenuLeft)this.Master.FindControl("UCMenu");
                if (categoria.Trim() == "VTELE")
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Messaggio = "Posizione non lavorabile";
                    ucAvviso.Visible = true;
                    Session["Lavorabile"] = false;
                }
                else
                {
                    ucAvviso.Messaggio = "";
                    ucAvviso.Visible = false;
                    Session["Lavorabile"] = true;
                }
            }

            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PosizioneSelezionata, Errore nel metodo CheckDomandaLavorabile " + ex);
            }
        }

        protected void PresaInCarico_Click(object sender, EventArgs e)
        {
        }
    }
}