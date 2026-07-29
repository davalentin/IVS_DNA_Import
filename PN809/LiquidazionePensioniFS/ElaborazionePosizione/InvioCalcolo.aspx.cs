using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using Polly;
using System.IO;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class InvioCalcolo : CustomBasePage, IInfoCalcolo, IQuadriSemafori, IStampa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IInfoCalcolo
        public AreaTitolare.DatiPensione datiPensione { get; set; }
        public Presenter.SvrLiquidazione.AreaEsito areaEsito { get; set; }
        public string statoPensione { get; set; }
        public int certificato { get; set; }
        public string chiavePensione { get; set; }
        public bool IsVerify { get; set; }
        public bool IsConsultazioniANFVerificate { get; set; }
        public bool IsNuovoCalcolo { get; set; }
        public string FlagIndennizzo { get; set; }
        public bool BloccaInvio { get; set; }
        public List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariConsultazioneUnificataANF> ListaConsultazioniANF { get; set; }
        public bool IsReingegnerizzato { get; set; }
        public List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneAnniRichiestaBonusDatiPrenotazioneElaborazioni> ListaPrenotazioneElaborazioni { get; set; }
        #endregion IInfoCalcolo

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region Stampa
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public MemoryStream msPDF { get; set; }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                setInitialDesign();
                setTipoCalcolo();
                SetTipoCalcoloForAmministratoreAGO();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void btnInvioCalcolo_Click(object sender, EventArgs e)
        {
            //    Handle<CustomException>()
            //.Retry(retryCount: 3,
            //onRetry: (exception, attemptNumber) =>
            //{
            //    //Change something to try to fix the problem
            //    speed = speed - 5;
            //    airIntake = airIntake - 5;
            //});

            if ((AreaQuadri)Session["Semaforo"] != null)
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];

            this.datiPensione = new AreaTitolare.DatiPensione();
            this.datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            this.IsVerify = ddlTipoCalcolo.SelectedValue == "D" ? false : true;
            this.IsConsultazioniANFVerificate = HdnConsultazioniANFVerificate.Value == "SI" ? true : false;
            this.IsReingegnerizzato = chkUtilizzaCalcoloReing.Checked;

            PresenterStampa presenterStampa = new PresenterStampa();
            presenterStampa.CancellaStampa(this);
            if (this.areaEsito == null || this.areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                this.lblEsito.Text = "KO";
                this.lblDettaglio.Text = this.areaEsito != null ? this.areaEsito.Messaggio : "Errore Generico Durante la Cancellazione della Stampa";
                return;
            }

            PresenterInvioCalcolo InvioCalcolo = new PresenterInvioCalcolo();

            InvioCalcolo.GetIsNuovoCalcolo(this);
            string transactionId = null;
            if (this.IsNuovoCalcolo)
            {
                var valori = new[] { 15, 20, 25, 15, 15 };
                var retry = Policy.HandleResult<int>(result => result != 0)
                    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(valori[attempt - 1]));

                InvioCalcolo.CalcolaDomanda(this, this, out transactionId);
                if (this.areaEsito != null && this.areaEsito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    retry.Execute(() =>
                    {
                        InvioCalcolo.GetEsitoNuovoCalcolo(this, transactionId);
                        int retCode = 0;
                        if (this.areaEsito != null && this.areaEsito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                            retCode = 1;
                        return retCode;
                    });
                }
            }
            else
            {
                InvioCalcolo.CalcolaDomanda(this, this, out transactionId);
            }

            setReturnData();

            if(domanda.Stato.Equals("CALCOLO NO INDEB") || domanda.Stato.Equals("CALCOLO NO INDEB WAIT"))
                Response.Redirect("AggiornaCalcoloNoInd.aspx");
        }

        // Funzione per accedere alla sequenza di Fibonacci come indice inverso e restituire l'elemento corrispondente
        static TimeSpan FibonacciInverso(int passo, int dimensione, int indice)
        {
            List<int> fibonacci = new List<int>();
            int a = 3;
            int b = passo;

            fibonacci.Add(a);
            fibonacci.Add(b);

            for (int i = 2; i < dimensione; i++)
            {
                int currentFibonacci = a + b;
                fibonacci.Add(currentFibonacci);
                a = b;
                b = currentFibonacci;
            }

            return TimeSpan.FromSeconds(fibonacci[fibonacci.Count - indice]);
        }

        private void setReturnData()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaEsito != null)
            {
                this.lblDettaglio.Text = this.areaEsito.Messaggio != null ? this.areaEsito.Messaggio.ToUpperInvariant() : "";
                this.lblEsito.Text = this.areaEsito.RisultatoOperazione.ToString().ToUpperInvariant();
            }

            if (!String.IsNullOrEmpty(this.statoPensione) &&
                (this.statoPensione.Trim().ToUpperInvariant() == "CALCOLATA" || this.statoPensione.Trim() == "CALCOLO NO WEBDOM" ||
                this.statoPensione.Trim() == "CALCOLO NO FELPE" || this.statoPensione.Trim() == "CALCOLO NO ONERI" ||
                this.statoPensione.Trim() == "CALCOLO NO SAI" || this.statoPensione.Trim() == "CALCOLO NO SIN" || this.statoPensione.Trim() == "CALCOLO NO TOTAL" || this.statoPensione.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoStazLavoro) ||
                this.statoPensione.Trim() == "CALCOLO NO BOOKING" || this.statoPensione.Trim() == "CALCOLO NO TOT" || this.statoPensione.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoNoteDebito) || this.statoPensione.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNo6Scatti) || this.statoPensione.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoEquoInd) || this.statoPensione.Trim() == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoIndennSpec)))
            {
                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    this.lblMessCalcoloDefinitivo.Text = "La stampa contenente il libretto di pensione è consultabile dalla procedura StampeWeb";

                switch(this.FlagIndennizzo)
                {
                    case "R":
                        this.msgResultIndennizzo.Text = "Soggetto residente all’estero: non è possibile predisporre e inviare il TE08/IND né verificare la recuperabilità su pensione.\nVerrà predisposto il TE08 e il debito dovrà essere gestito in procedura RI";
                        break;
                    case "E":
                        this.msgResultIndennizzo.Text = "Pensione in cumulo/totalizzazione con Ente/Cassa esterna: non è possibile predisporre e inviare il TE08/IND né verificare la recuperabilità su pensione.\nVerrà predisposto il TE08 e il debito dovrà essere gestito in procedura RI.";
                        break;
                    case "S":
                        this.msgResultIndennizzo.Text = "Ricostituzione a seguito di sentenza: non è possibile predisporre e inviare il TE08/IND né verificare la recuperabilità su pensione.\nVerrà predisposto il TE08 e il debito dovrà essere gestito in procedura RI.";
                        break;
                    case "X":
                        this.msgResultIndennizzo.Text = "Pensione eliminata per decesso: non è possibile predisporre e inviare il TE08/IND né verificare la recuperabilità su pensione.\nVerrà predisposto il TE08 e il debito dovrà essere gestito in procedura RI.";
                        break;
                    case "Z":
                        this.msgResultIndennizzo.Text = "Pensione eliminata: non è possibile predisporre e inviare il TE08/IND né verificare la recuperabilità su pensione.\nVerrà predisposto il TE08 e il debito dovrà essere gestito in procedura RI.";
                        break;
                }

                this.lblCertificatoTitolo.Visible = true;
                this.lblCertificatoValore.Visible = true;
                this.lblCertificatoValore.Text = this.certificato.ToString().PadLeft(8, '0');
                if (Session["Domanda"] != null)
                {
                    AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    Domanda.Certificato = this.certificato.ToString();
                    Session["Domanda"] = Domanda;
                }
                setDefinitiveDesign();
            }
            else
            {
                this.lblCertificatoTitolo.Visible = false;
                this.lblCertificatoValore.Text = "";
                this.lblCertificatoValore.Visible = false;
                setVerifyDesign();
            }
            this.lblStato.Text = this.statoPensione != null ? this.statoPensione.ToUpperInvariant() : "";

            if (!String.IsNullOrEmpty(this.statoPensione) && Session["Domanda"] != null)
            {
                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                Domanda.Stato = this.statoPensione.ToUpperInvariant();
                Session["Domanda"] = Domanda;

                int retCode = 0;
                string descErrore = string.Empty;
                if (this.areaEsito != null && this.areaEsito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    retCode = 1;
                    descErrore = this.areaEsito.Messaggio;
                }
                Presenter.LogSicurezza.ScritturaLog(Domanda.NumeroDomanda, Domanda.TipoAppartenenza, int.Parse(ConfigurationManager.AppSettings["IDEVENTO-CALCOLO"]),
                    HttpContext.Current.Request.UserHostAddress, retCode, descErrore,
                    Session["Anagrafica"] != null ? ((AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"]).CodiceFiscale : string.Empty,
                    this.chiavePensione);
            }

            this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);

            ValorizzaConsultazioneANF();
            if (this.statoPensione.Trim().ToUpperInvariant() == "CALCOLATA")
                ValorizzaEsitoPrenotazione();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            //ENG - Reversibilita 024
            if (Utility.IsDomandaReversibilita(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
            }

            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            if (this.IsNuovoCalcolo) btnInvioCalcolo.Enabled = false;
            if (this.BloccaInvio) btnInvioCalcolo.Enabled = false;

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        private void setTipoCalcolo()
        {
            datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            PresenterInvioCalcolo InvioCalcolo = new PresenterInvioCalcolo();
            InvioCalcolo.GetIsDomandaVerify(this);
            if (this.IsVerify)
                ddlTipoCalcolo.SelectedValue = "V";
            else
                ddlTipoCalcolo.SelectedValue = "D";

            InvioCalcolo.GetIsNuovoCalcolo(this);

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            string controlloDinamico = string.Empty;
            presenter.GetControlloDinamicoByNomeControllo("DataCalcoloDefinitivoINDCOM", out controlloDinamico);
            DateTime? dataCalcoloDefinitivoINDCOM = Utility.DataFromString(controlloDinamico, Utility.FormatoData.AAAAmmGG);

            if (datiPensione != null && Utility.IsDomandaINDCOM175(datiPensione, domanda.Categoria) && dataCalcoloDefinitivoINDCOM.HasValue &&
                Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda.Value, dataCalcoloDefinitivoINDCOM.Value))
            {
                ddlTipoCalcolo.SelectedValue = "V";
                ddlTipoCalcolo.Enabled = false;
            }

            string sedeLavorazione = Utility.GetSedeOperatore().ToString().PadLeft(4, '0');

            string controlloDinamicoSpacchettate = string.Empty;
            string controlloDinamicoAbilitazioneSpacchettate024 = string.Empty;
            Presenter.PresenterControlliDinamici presenterSpacchettate = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenterSpacchettate.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                controlloDinamicoAbilitazioneSpacchettate024 = controlloDinamicoSpacchettate;

            string controlloDinamicoAbilitazioneCalcoloReingSediPT = string.Empty;
            Presenter.SvrLiquidazione.AreaEsito esitoCalcoloReingSediPT = presenterSpacchettate.GetControlloDinamicoByNomeControllo("AbilitazioneCalcoloReingSediPT", out controlloDinamicoAbilitazioneCalcoloReingSediPT);

            string controlloDinamicoCalcoloReing = string.Empty;
            Presenter.SvrLiquidazione.AreaEsito esitoAbilitazioneCalcoloReing = presenterSpacchettate.GetControlloDinamicoByNomeControllo("UsaCalcoloReingegnerizzato", out controlloDinamicoCalcoloReing);

            if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT && esitoAbilitazioneCalcoloReing != null
                && esitoAbilitazioneCalcoloReing.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(controlloDinamicoCalcoloReing) && controlloDinamicoCalcoloReing.ToUpperInvariant() == "SI")
            {
                if (!String.IsNullOrEmpty(controlloDinamicoAbilitazioneSpacchettate024) && controlloDinamicoAbilitazioneSpacchettate024.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione))
                {
                    lblUtilizzaCalcoloReing.Visible = true;
                    chkUtilizzaCalcoloReing.Visible = true;
                    chkUtilizzaCalcoloReing.Checked = true;
                    chkUtilizzaCalcoloReing.Enabled = false;
                }
                else
                {
                    if (!CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && esitoCalcoloReingSediPT != null && esitoCalcoloReingSediPT.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && (String.IsNullOrEmpty(controlloDinamicoAbilitazioneCalcoloReingSediPT) || controlloDinamicoAbilitazioneCalcoloReingSediPT.Split(';').ToList().Exists(x => x.PadLeft(4, '0') == sedeLavorazione)))
                    {
                        lblUtilizzaCalcoloReing.Visible = true;
                        chkUtilizzaCalcoloReing.Visible = true;
                    }
                }
            }

            if (this.BloccaInvio)
            {
                btnInvioCalcolo.Enabled = false;
                lblAvvisoNuovoCalcolo.Text = "L'operazione di calcolo è in corso. Non appena disponibile l'esito, sarà visibile accedendo nuovamente alla posizione. ";
                divAvvisoNuovoCalcolo.Visible = true;
            }
        }

        private void SetTipoCalcoloForAmministratoreAGO()
        {
            if (!((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).IsMatchMatricola &&
                ((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
            {
                ddlTipoCalcolo.SelectedValue = "V";
                ddlTipoCalcolo.Enabled = false;
            }
        }

        private void setInitialDesign()
        {
            this.divResult.Style.Add("display", "none");
            rowMargin.Style.Add("height", "120px");
            divSeparator.Style.Add("height", "70px");
        }

        private void setVerifyDesign()
        {
            rowMargin.Style.Remove("height");
            rowMargin.Style.Add("height", "50px");
            this.divResult.Style.Add("display", "block");
            divSeparator.Style.Remove("height");
            divSeparator.Style.Add("height", "20px");
        }

        private void setDefinitiveDesign()
        {
            this.divIntro.Style.Add("display", "none");
            rowMargin.Style.Remove("height");
            rowMargin.Style.Add("height", "50px");
            this.divResult.Style.Add("display", "block");
            divSeparator.Style.Remove("height");
        }

        private void ValorizzaConsultazioneANF()
        {
            lblConsultazioneANF.Text = string.Empty;
            string esitoConsultazione = string.Empty;
            if (this.ListaConsultazioniANF != null && ListaConsultazioniANF.Count > 0)
            {
                foreach (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariConsultazioneUnificataANF consultazione in this.ListaConsultazioniANF)
                {
                    if (consultazione.listaDatiDomandaAnf != null && consultazione.listaDatiDomandaAnf.Count() > 0)
                    {
                        esitoConsultazione += string.Format("Per il soggetto {0}, con la consultazione effettuata il {1},", consultazione.codiceFiscaleRichiedente, consultazione.dataRichiestaRichiedente);
                        esitoConsultazione += " risultano domande:<br/><br/>";
                        Dictionary<string, List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf>> listaFonti = consultazione.listaDatiDomandaAnf.GroupBy(x => x.codiceFonte).ToDictionary(y => y.Key, y => y.ToList());
                        if (listaFonti != null && listaFonti.Count() > 0)
                        {
                            foreach (string codiceFonte in listaFonti.Keys)
                            {
                                List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf> datiDomanda = null;
                                listaFonti.TryGetValue(codiceFonte, out datiDomanda);
                                if (datiDomanda != null && datiDomanda.Count() > 0)
                                {
                                    esitoConsultazione += string.Format(" -  Sulla prestazione <b>'{0}'</b>:<ul>", datiDomanda.FirstOrDefault().descrizioneFonte);
                                    List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf> listaDomandeFiltrata = new List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf>();
                                    foreach (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf domanda in datiDomanda)
                                    {
                                        if (!listaDomandeFiltrata.Exists(x => (String.IsNullOrEmpty(x.periodoDataDa) && String.IsNullOrEmpty(x.periodoDataA) && String.IsNullOrEmpty(domanda.periodoDataDa) && String.IsNullOrEmpty(domanda.periodoDataA) || (!String.IsNullOrEmpty(x.periodoDataDa) && !String.IsNullOrEmpty(x.periodoDataA) && !String.IsNullOrEmpty(domanda.periodoDataDa) && !String.IsNullOrEmpty(domanda.periodoDataA) && x.periodoDataDa == domanda.periodoDataDa && x.periodoDataA == domanda.periodoDataA)) &&
                                                                             x.codicePratica1 == domanda.codicePratica1 && x.numeroProtocolloDomanda == domanda.numeroProtocolloDomanda &&
                                                                             x.statoDomanda == domanda.statoDomanda))
                                            listaDomandeFiltrata.Add(domanda);
                                    }

                                    if (listaDomandeFiltrata != null && listaDomandeFiltrata.Count() > 0)
                                    {
                                        foreach (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneFamiliariDomandaAnf domanda in listaDomandeFiltrata)
                                        {
                                            bool isDettaglioPratica = listaDomandeFiltrata.Where(x => x.codicePratica1 == domanda.codicePratica1).Count() > 1;
                                            esitoConsultazione += "<li>";
                                            if (!String.IsNullOrEmpty(domanda.periodoDataDa) && !String.IsNullOrEmpty(domanda.periodoDataA))
                                                esitoConsultazione += string.Format("Periodo <b>{0}</b>, ", domanda.periodoDataDa + " - " + domanda.periodoDataA);
                                            esitoConsultazione += string.Format("Numero <b>{0}</b>, Protocollo <b>'{1}'</b> e stato <b>'{2}'</b>.</li><br/>", domanda.codicePratica1,
                                                domanda.numeroProtocolloDomanda, CodeUtility.GetStatoDomandaANF(consultazione.codiceFiscaleRichiedente, domanda, isDettaglioPratica));
                                        }
                                    }
                                    esitoConsultazione += "</ul>";

                                    esitoConsultazione += "<br/><br/>";
                                    lblConsultazioneANF.Text = esitoConsultazione;
                                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpConsultazioniANF", "<script>ShowPopUpConsultazioniANF();</script>", false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ValorizzaEsitoPrenotazione()
        {
            if (ListaPrenotazioneElaborazioni != null && ListaPrenotazioneElaborazioni.Count() > 0)
            {
                gvEsitoPrenotazione.DataSource = ListaPrenotazioneElaborazioni;
                gvEsitoPrenotazione.DataBind();
                ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpEsitoPrenotazione", "<script>ShowPopUpEsitoPrenotazione();</script>", false);
            }
        }
    }

}

