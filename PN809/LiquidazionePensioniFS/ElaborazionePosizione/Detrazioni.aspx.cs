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
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Detrazioni : CustomBasePage, IInfoLiquidazione, IDetrazioni, IQuadriSemafori, ITitolarePensione
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDetrazioni
        public AreaDetrazioni detrazioniPensione { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public string CodiceFiscale { get; set; }
        #endregion IDetrazioni

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                LoadPage();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.detrazioniPensione = (AreaDetrazioni)ViewState["Detrazioni"];
            GestioneDetrazioniSoggetto[] elencoSoggetti = ViewState[EnumViewState.ElencoSoggetti.ToString()] as GestioneDetrazioniSoggetto[];
            if (elencoSoggetti != null && elencoSoggetti.Count() > 0 && this.detrazioniPensione.DatiInput != null && !string.IsNullOrEmpty(this.detrazioniPensione.DatiInput.CodiceFiscale))
            {
                if (elencoSoggetti.ToList().Exists(x => x.CodiceFiscale == this.detrazioniPensione.DatiInput.CodiceFiscale && x.Confermato))
                    ValorizzaSemaforiTab(imgDetrazioni, AreaQuadri.Semaforo.Verde, pnlTabDetrazioni);
                else
                    ValorizzaSemaforiTab(imgDetrazioni, AreaQuadri.Semaforo.Rosso_Abilitato, pnlTabDetrazioni);
            }
            else if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgDetrazioni, this.areaQuadri.QuadroDetrazioni.TabDetrazioni, pnlTabDetrazioni);
            }
        }

        protected void SalvaDetrazioni(Object sender, EventArgs e)
        {
            PresenterDetrazioni presenterDetrazioni = new PresenterDetrazioni();
            this.detrazioniPensione = (AreaDetrazioni)ViewState["Detrazioni"];
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ucDetrazioni.detrazioniPensione = this.detrazioniPensione;
            presenterDetrazioni.SalvaDetrazioni(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                if (detrazioniPensione.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Informativa)
                {
                    ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Info;
                    ucAvviso.Messaggio = detrazioniPensione.Messaggio;
                }
                else if (detrazioniPensione.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore)
                {
                    string strError = detrazioniPensione.Messaggio;
                    string strError2 = strError.Replace(" ", "");
                    strError = strError2.Trim();
                    strError2 = strError.ToLowerInvariant();

                    if (strError2.Equals("ledetrazionisonostatevariate."))
                        ucAvviso.Tipo = TipoAvviso.Info;

                    else
                        ucAvviso.Tipo = TipoAvviso.Ko;
                    ucAvviso.Messaggio = detrazioniPensione.Messaggio;
                    ucDetrazioni.detrazioniPensione = this.detrazioniPensione;
                    ucDetrazioni.ValorizzaDetrazioni();
                }
                else
                {
                    ucAvviso.Tipo = TipoAvviso.Ok;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Dati Detrazioni salvati correttamente";
                }
                //                ViewState["Detrazioni"] = this.detrazioniPensione;
                //                return;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Detrazioni salvati correttamente";

                if (this.detrazioniPensione != null && this.detrazioniPensione.DatiInput != null && !string.IsNullOrEmpty(this.detrazioniPensione.DatiInput.CodiceFiscale))
                {
                    GestioneDetrazioniSoggetto[] elencoSoggetti = ViewState[EnumViewState.ElencoSoggetti.ToString()] as GestioneDetrazioniSoggetto[];
                    if (elencoSoggetti != null)
                    {
                        elencoSoggetti.ToList().FindAll(x => x.CodiceFiscale == this.detrazioniPensione.DatiInput.CodiceFiscale).ForEach(x => x.Confermato = true);
                        ViewState[EnumViewState.ElencoSoggetti.ToString()] = elencoSoggetti.ToArray();
                    }
                }
            }
            ViewState["Detrazioni"] = this.detrazioniPensione;
            GestioneAcquisizione();

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucAcquisizioneDetrazioni(object sender, EventArgs e)
        {
            this.detrazioniPensione = (AreaDetrazioni)ViewState["Detrazioni"];
            Response.Redirect(this.detrazioniPensione.Url);
        }

        protected void event_ucAggiornamentoDetrazioni(object sender, EventArgs e)
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDetrazioni presenterDetrazioni = new PresenterDetrazioni();
            if (sender as UserControls.Detrazioni.UCDetrazioni != null && ((UserControls.Detrazioni.UCDetrazioni)sender).detrazioniPensione != null &&
                ((UserControls.Detrazioni.UCDetrazioni)sender).detrazioniPensione.DatiInput != null && !string.IsNullOrEmpty(((UserControls.Detrazioni.UCDetrazioni)sender).detrazioniPensione.DatiInput.CodiceFiscale))
            {
                if (this.detrazioniPensione == null)
                    this.detrazioniPensione = new AreaDetrazioni();
                this.detrazioniPensione.DatiInput = ((UserControls.Detrazioni.UCDetrazioni)sender).detrazioniPensione.DatiInput;
            }
            presenterDetrazioni.GetDetrazioni(this);
            ucDetrazioni.detrazioniPensione = this.detrazioniPensione;
            ViewState["Detrazioni"] = this.detrazioniPensione;
            ucDetrazioni.ValorizzaDetrazioni();
            GestioneAcquisizione();
            this.btnSalvaDetrazioni.Enabled = true;
            if (HasError)
            {
                ucAvviso.Visible = true;
                if (detrazioniPensione.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore)
                {
                    if (String.Compare((detrazioniPensione.Messaggio).Trim(), ("Le detrazioni sono state variate").Trim(), true, CultureInfo.InvariantCulture) == 0)
                        ucAvviso.Tipo = TipoAvviso.Info;
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        this.btnSalvaDetrazioni.Enabled = false;
                    }
                    ucAvviso.Messaggio = detrazioniPensione.Messaggio;
                    return;
                }
            }
            else
            {
                Label lblReddito = (Label)(ucDetrazioni.FindControl("lblAgevolazReddLavAut"));
                if (lblReddito != null)
                {
                    if (!String.IsNullOrEmpty(lblReddito.Text))
                    {
                        ViewState["Detrazioni"] = this.detrazioniPensione;
                        ucAvviso.Tipo = TipoAvviso.Info;
                        ucAvviso.Messaggio = "Detrazioni acquisite correttamente. Si prega di salvarle.";

                    }
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        ucAvviso.Messaggio = "Non esistono detrazioni associate al soggetto. E' necessario acquisirle.";
                    }
                }
            }
            return;
        }

        protected void event_ucRicaricaSoggetti(object sender, EventArgs e)
        {
            this.detrazioniPensione = (AreaDetrazioni)ViewState["Detrazioni"];
            this.detrazioniPensione.DatiInput = null;
            ucAvviso.Visible = false;

            LoadPage();
        }

        protected void event_ucDisabilitaSalva(object sender, EventArgs e)
        {
            this.btnSalvaDetrazioni.Enabled = false;
        }

        protected void event_ucAbilitaSalva(object sender, EventArgs e)
        {
            this.btnSalvaDetrazioni.Enabled = true;
        }

        #region private methods

        private void LoadPage()
        {
            CaricaDetrazioni();
            ucDetrazioni.ValorizzaDetrazioni();
            GestioneAcquisizione();
        }

        private void CaricaDetrazioni()
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ManageRecuperoDati();
            ucDetrazioni.detrazioniPensione = this.detrazioniPensione;
            ViewState["Detrazioni"] = this.detrazioniPensione;
            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                && !this.domanda.IsDomandaINPDAP)
                this.btnSalvaDetrazioni.Enabled = false;
            else
                this.btnSalvaDetrazioni.Enabled = true;
            if (HasError)
            {
                ucAvviso.Visible = true;
                if (detrazioniPensione.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Informativa)
                {
                    ucAvviso.Tipo = TipoAvviso.Info;
                    ucAvviso.Messaggio = detrazioniPensione.Messaggio;
                }
                else if (detrazioniPensione.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore)
                {
                    if (String.Compare((detrazioniPensione.Messaggio).Trim(), ("Le detrazioni sono state variate").Trim(), true, CultureInfo.InvariantCulture) == 0)
                        ucAvviso.Tipo = TipoAvviso.Info;
                    else
                    {
                        ucAvviso.Tipo = TipoAvviso.Ko;
                        this.btnSalvaDetrazioni.Enabled = false;
                    }
                    ucAvviso.Messaggio = detrazioniPensione.Messaggio;
                    return;
                }
            }
        }

        private void GestioneAcquisizione()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.detrazioniPensione == null || string.IsNullOrEmpty(this.detrazioniPensione.Url))
                ucDetrazioni.GestioneAcquisizione(false);
            else if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                if (TitolarePensione == null)
                    TitolarePensione = new AreaTitolare();
                if (TitolarePensione.Pensione == null)
                    TitolarePensione.Pensione = GetDatiPensione(this);

                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

                if ((tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione && !this.detrazioniPensione.IsVariazioneDetrazioni) || 
                    this.domanda.IsDomandaRiapertura)
                    ucDetrazioni.GestioneAcquisizione(false);
                else
                    ucDetrazioni.GestioneAcquisizione(true);
            }
            else
                ucDetrazioni.GestioneAcquisizione(true);
        }

        private void ManageRecuperoDati()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            PresenterDetrazioni presenterDetrazioni = new PresenterDetrazioni();

            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria))
            {
                presenterDetrazioni.GetSoggettiDetrazioni(this);
                if (this.detrazioniPensione != null)
                {
                    if(this.domanda.IsDomandaINPDAP && this.detrazioniPensione.ElencoSoggetti != null && this.detrazioniPensione.ElencoSoggetti.Count() == 1)
                        presenterDetrazioni.GetDetrazioni(this);
                    else
                        ViewState[EnumViewState.ElencoSoggetti.ToString()] = this.detrazioniPensione.ElencoSoggetti;
                }           
            }
            else
                presenterDetrazioni.GetDetrazioni(this);
        }

        #endregion private methods

        #region enum
        private enum EnumViewState
        {
            ElencoSoggetti
        }
        #endregion enum
    }
}
