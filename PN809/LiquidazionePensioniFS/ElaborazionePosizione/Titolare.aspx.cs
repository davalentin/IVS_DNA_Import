using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;


namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Titolare : CustomBasePage, IInfoLiquidazione, ITitolarePensione, IQuadriSemafori, IDanteCausa
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare Tit = this.GetDatiTitolare(this);
                if (Tit.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = Tit.Esito.Messaggio;
                }

                ViewState["AreaTitolare"] = Tit;
                SwitchUserControls();
                ucStatoCivile.TitolarePensione = Tit;
                ucStatoCivile.ValorizzaEtichette();

                ucResidenzeEstere.TitolarePensione = Tit;
                ucResidenzeEstere.ValorizzaEtichette();

                Session["DatiPensione"] = Tit.Pensione;
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    ValorizzaSemaforiTab(imgAnagrafica, this.areaQuadri.QuadroTitolare.TabAnagrafica, pnlTabAnagrafica);
                    ValorizzaSemaforiTab(imgStatoCivile, this.areaQuadri.QuadroTitolare.TabStatiCivili, pnlTabStatoCivile);
                    ValorizzaSemaforiTab(imgResidenzeEstere, this.areaQuadri.QuadroTitolare.TabResidenzeEstero, pnlTabResidenzeEstere);
                }

                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Titolare, Errore nel metodo Page_PreRender " + ex);
            }
        }

        protected void event_ucSalvaStatoCivile(object sender, EventArgs e)
        {
            if (ucAvviso.Visible == true)
                ucAvviso.Visible = false;
        }

        protected void event_ucErrorSalvaStatoCivile(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Inserire la Decorrenza e lo Stato Civile ";
        }

        protected void event_ucAnnullaSalvaStatoCivile(object sender, EventArgs e)
        {
            ucAvviso.Messaggio = "";
            ucAvviso.Visible = false;
        }

        protected void event_ucSalvaResidenzeEstere(object sender, EventArgs e)
        {
            if (ucAvviso.Visible == true)
                ucAvviso.Visible = false;
        }

        protected void event_ucErrorSalvaResidenzeEstere(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Inserire la Decorrenza e lo Stato di residenza estera";
        }

        protected void event_ucAnnullaResidenzeEstere(object sender, EventArgs e)
        {
            ucAvviso.Messaggio = "";
            ucAvviso.Visible = false;
        }

        protected void event_ucAbilitaTastoSalva(object sender, EventArgs e)
        {
            //try
            //{
            //    if ((AreaQuadri)Session["Semaforo"] != null)
            //    {
            //        this.areaQuadri = (AreaQuadri)Session["Semaforo"];
            //        if (!(this.areaQuadri.QuadroTitolare.TabStatiCivili != AreaQuadri.Semaforo.Giallo && this.areaQuadri.QuadroTitolare.TabStatiCivili != AreaQuadri.Semaforo.Rosso_NonAbilitato && CodeUtility.IsGridViewInEditPresent(ucStatoCivile)) &&
            //            !(this.areaQuadri.QuadroTitolare.TabResidenzeEstero != AreaQuadri.Semaforo.Giallo && this.areaQuadri.QuadroTitolare.TabResidenzeEstero != AreaQuadri.Semaforo.Rosso_NonAbilitato && CodeUtility.IsGridViewInEditPresent(ucResidenzeEstere)))
            //            btnSalva.Enabled = true;
            //    }
            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("Titolare, Errore nel metodo event_ucAbilitaTastoSalva " + ex);
            //}
        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            //btnSalva.Enabled = false;
        }

        protected void event_ucGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            TextBox dp = null;
            if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
            {
                dp = (TextBox)ucAnagraficaRIC.FindControl("txtDecorrenzaPensione");
            }
            else
            {
                dp = (TextBox)ucAnagrafica.FindControl("txtDecorrenzaPensione");
            }
            HiddenField hidResidenzeEstere = (HiddenField)ucResidenzeEstere.FindControl("hdn_txtDecorrenzaPensione");
            hidResidenzeEstere.Value = dp.Text;
            HiddenField hidStatoCivile = (HiddenField)ucStatoCivile.FindControl("hdn_txtDecorrenzaPensioneSC");
            hidStatoCivile.Value = dp.Text;
        }

        protected void event_ucGetResidenzaEstera(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            string codResidenzaEstera = string.Empty;
            if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
            {
                codResidenzaEstera = ucAnagraficaRIC.TitolarePensione.Anagrafica.CodiceComuneResidenza;
            }
            else
            {
                codResidenzaEstera = ucAnagrafica.TitolarePensione.Anagrafica.CodiceComuneResidenza;
            }
            HiddenField hidComuneStatoResidenza = (HiddenField)ucResidenzeEstere.FindControl("hdn_lblCodiceComuneResidenza");
            hidComuneStatoResidenza.Value = codResidenzaEstera;
        }

        protected void event_ucShowAvvisoStatoCivile(object sender, EventArgs e)
        {
            UserControls.Titolare.UCStatoCivile tabStatoCivile = (UserControls.Titolare.UCStatoCivile)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabStatoCivile.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabStatoCivile.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Stato Civile salvati correttamente";

            }
        }

        protected void event_ucShowAvvisoAnagrafica(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            bool err = false;
            string mess = string.Empty;
            if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
            {
                UserControls.Titolare.UCAnagraficaRIC tabAnagraficaRIC = (UserControls.Titolare.UCAnagraficaRIC)sender;
                err = tabAnagraficaRIC.HasError;
                mess = tabAnagraficaRIC.ErrorMessage;
            }
            else
            {
                UserControls.Titolare.UCAnagrafica tabAnagrafica = (UserControls.Titolare.UCAnagrafica)sender;
                err = tabAnagrafica.HasError;
                mess = tabAnagrafica.ErrorMessage;
            }
            if (err)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = mess;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Anagrafica salvati correttamente";
                if (!(IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura))
                {
                    UserControls.Titolare.UCAnagrafica TabCertificato = (UserControls.Titolare.UCAnagrafica)sender;
                    TabCertificato.SetTabCertificatoAfterSave();
                }
            }
        }

        protected void event_ucShowAvvisoResidenzeEstere(object sender, EventArgs e)
        {
            UserControls.Titolare.UCResidenzeEstere tabResidenzeEstere = (UserControls.Titolare.UCResidenzeEstere)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabResidenzeEstere.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabResidenzeEstere.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Residenze Estere salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoDeleteResidenzeEstere(object sender, EventArgs e)
        {
            UserControls.Titolare.UCResidenzeEstere tabResidenzeEstere = (UserControls.Titolare.UCResidenzeEstere)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabResidenzeEstere.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabResidenzeEstere.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Residenze Estere eliminati correttamente";
            }
        }

        public string GetAbsoluteUri(string relativeUri)
        {
            var uri = new Uri(Request.Url, ResolveUrl(relativeUri));
            return uri.AbsoluteUri;
        }

        protected void btnSalvaTitolare_Click(Object sender, EventArgs e)
        {
            try
            {
                bool isTabAnagraficaSaved = false;
                bool isWarning = false;
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                PresenterTitolare presenterTitolare = new PresenterTitolare();
                if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
                {
                    ucAnagraficaRIC.TitolarePensione = (AreaTitolare)ViewState["AreaTitolare"];
                    this.TitolarePensione = ucAnagraficaRIC.GetDatiUcAnagrafica();
                }
                else
                {
                    ucAnagrafica.TitolarePensione = (AreaTitolare)ViewState["AreaTitolare"];
                    this.TitolarePensione = ucAnagrafica.GetDatiUcAnagrafica();
                }

                this.TitolarePensione.ElencoStatiCiviliTitolare = ucStatoCivile.GetDatiUcStatoCivile().ElencoStatiCiviliTitolare;
                this.TitolarePensione.ElencoResidenzeEstereTitolare = ucResidenzeEstere.GetDatiUcResidenzeEstere().ElencoResidenzeEstereTitolare;

                presenterTitolare.SalvaDatiTitolare(this, out isTabAnagraficaSaved, out isWarning);

                if (isTabAnagraficaSaved || isWarning)
                    Session["DatiPensione"] = TitolarePensione.Pensione;

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.Titolare);
                elencoTab.Add(AreaQuadri.Tab.Familiare);
                elencoTab.Add(AreaQuadri.Tab.Redditi);
                elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
                elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
                elencoTab.Add(AreaQuadri.Tab.Eliminazione);
                elencoTab.Add(AreaQuadri.Tab.Oneri);
                elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

                if (HasError)
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = ErrorMessage;
                    return;
                }
                else
                {
                    if (this.TitolarePensione.Pensione != null && ((AreaTitolare.DatiPensione)Session["DatiPensione"]) != null)
                    {
                        if (this.TitolarePensione.Pensione.TipoLetturaUnicarpe == null || this.TitolarePensione.Pensione.TipoLetturaUnicarpe.Value.ToString().Trim() == "C")
                        {
                            string CodFase = string.Empty;
                            string Domanda = domanda.NumeroDomanda;
                            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                            Presenter.SvrLiquidazione.AreaEsito esito = objWS.GetCodFaseByNDomus(out CodFase, Domanda);
                            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                            {
                                string SiglaCategoria = this.domanda.Categoria;
                                string Gruppo = this.TitolarePensione.Pensione.CodeGruppo;
                                string Prodotto = this.TitolarePensione.Pensione.CodeProdotto;
                                string Tipo = this.TitolarePensione.Pensione.CodeTipo;
                                DateTime DecorrenzaOriginaria = this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value;
                                string codFisc = this.TitolarePensione.Anagrafica.CodiceFiscale;
                                if (Utility.CheckDatiPeco_FunzioneC(CodFase, SiglaCategoria, Gruppo, Prodotto, Tipo))
                                {
                                    bool Decorrenza = DecorrenzaOriginaria >= new DateTime(2023, 4, 1);
                                    string errore = string.Empty;
                                    string Caratterizzazione = string.Empty;
                                    if (Decorrenza)
                                    {
                                        string warning = string.Empty;
                                        AreaTitolare.DatiPensione.TipoAppDomanda AppDomanda = this.TitolarePensione.Pensione.TipoAppartenenzaDomanda.Value;

                                        if (SiglaCategoria.StartsWith("S")) //Superstiti
                                        {
                                            PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                                            presenterDanteCausa.GetDatiDanteCausa(this);
                                            codFisc = this.areaDanteCausa.AnagraficaDC.CodiceFiscale;
                                        }

                                        esito = objWS.GetDatiPECO_FunzioneC(Domanda, codFisc, AppDomanda.ToString(), this.TitolarePensione.Pensione.CodeGestione, this.TitolarePensione.Pensione.CodeFondo, ref Caratterizzazione, out errore);
                                        if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                                        {
                                            ((AreaTitolare.DatiPensione)Session["DatiPensione"]).Caratterizzazione = Caratterizzazione;

                                            hiddInfoMessage.Value = string.Empty;
                                            if (!string.IsNullOrEmpty(errore))
                                            {
                                                hiddInfoMessage.Value = errore;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        esito = objWS.CleanTipoSpecECaratterizzazione(Domanda, ref Caratterizzazione, out errore);
                                        ((AreaTitolare.DatiPensione)Session["DatiPensione"]).Caratterizzazione = Caratterizzazione;
                                    }
                                }
                            }
                        }
                    }

                    ucAvviso.Tipo = TipoAvviso.Ok;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Dati Titolare salvati correttamente";
                }
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Titolare, Errore nel metodo SalvaDati" + ex);
            }
        }

        protected void btnAggiornaARCA_Click(Object sender, EventArgs e)
        {
            try
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                PresenterTitolare presenterTitolare = new PresenterTitolare();
                if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
                {
                    ucAnagraficaRIC.TitolarePensione = (AreaTitolare)ViewState["AreaTitolare"];
                    this.TitolarePensione = ucAnagraficaRIC.GetDatiUcAnagrafica();
                }
                else
                {
                    ucAnagrafica.TitolarePensione = (AreaTitolare)ViewState["AreaTitolare"];
                    this.TitolarePensione = ucAnagrafica.GetDatiUcAnagrafica();
                }

                presenterTitolare.AggiornaDaARCA(this);

                if (HasError)
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = ErrorMessage;
                    return;
                }
                else
                {
                    ucAvviso.Tipo = TipoAvviso.Ok;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Dati ARCA aggiornati";

                    ResetTabFocusDopoAggiornaARCA();
                    ViewState["AreaTitolare"] = this.TitolarePensione;

                    ucStatoCivile.UpdateViewState(this);
                    ucResidenzeEstere.UpdateViewState(this);

                    Session["Anagrafica"] = this.TitolarePensione.Anagrafica;
                    if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
                    {
                        ucAnagraficaRIC.UpdateViewState(this);
                        ucAnagraficaRIC.ValorizzaEtichetteUCAnagrafica();
                    }
                    else
                    {
                        ucAnagrafica.UpdateViewState(this);
                        ucAnagrafica.ValorizzaEtichetteUCAnagrafica();
                    }

                    ucStatoCivile.TitolarePensione = TitolarePensione;
                    ucStatoCivile.ValorizzaEtichette();
                    ucResidenzeEstere.TitolarePensione = TitolarePensione;
                    ucResidenzeEstere.ValorizzaEtichette();
                }

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.Titolare);
                elencoTab.Add(AreaQuadri.Tab.Redditi);
                elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
                elencoTab.Add(AreaQuadri.Tab.Detrazioni);
                elencoTab.Add(AreaQuadri.Tab.Eliminazione);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Titolare, Errore nel metodo Aggiorna da ARCA: " + ex);
            }
        }

        private void SwitchUserControls()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (IsRic((AreaTitolare)ViewState["AreaTitolare"]) || this.domanda.IsDomandaRiapertura)
            {
                this.ucAnagrafica.Visible = false;
                this.ucAnagraficaRIC.Visible = true;
                ucAnagraficaRIC.TitolarePensione = (AreaTitolare)ViewState["AreaTitolare"];

            }
            else
            {
                this.ucAnagrafica.Visible = true;
                this.ucAnagraficaRIC.Visible = false;
                ucAnagrafica.TitolarePensione = (AreaTitolare)ViewState["AreaTitolare"];
            }
        }

        private bool IsRic(AreaTitolare Tit)
        {
            if (Tit != null && Tit.Pensione != null &&
                Tit.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione)
                return true;
            else
                return false;
        }

        private void ResetTabFocusDopoAggiornaARCA()
        {
            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && !this.TitolarePensione.Anagrafica.CodiceComuneResidenza.StartsWith("Z"))
                hdnSelected.Value = "#anagrafica";
        }

        private void ManagePulsanti()
        {
            try
            {
                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    if (!(this.areaQuadri.QuadroTitolare.TabStatiCivili != AreaQuadri.Semaforo.Giallo && this.areaQuadri.QuadroTitolare.TabStatiCivili != AreaQuadri.Semaforo.Rosso_NonAbilitato && CodeUtility.IsGridViewInEditPresent(ucStatoCivile)) &&
                        !(this.areaQuadri.QuadroTitolare.TabResidenzeEstero != AreaQuadri.Semaforo.Giallo && this.areaQuadri.QuadroTitolare.TabResidenzeEstero != AreaQuadri.Semaforo.Rosso_NonAbilitato && CodeUtility.IsGridViewInEditPresent(ucResidenzeEstere)))
                        btnSalva.Enabled = true;
                    else
                        btnSalva.Enabled = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Titolare, Errore nel metodo ManagePulsanti " + ex);
            }
        }
    }
}
