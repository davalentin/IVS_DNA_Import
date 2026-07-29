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
    public partial class DelegatoTutore : CustomBasePage, IInfoLiquidazione, IDelegatoTutore, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IDelegatoTutore
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo delegato { get; set; }

        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo tutore { get; set; }

        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDelegatoTutore

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDelegato();
                CaricaTutore();
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
                    ValorizzaSemaforiTab(imgDelegato, this.areaQuadri.QuadroDelegatoTutore.TabDelegato, pnlTabDelegato);
                    ValorizzaSemaforiTab(imgTutore, this.areaQuadri.QuadroDelegatoTutore.TabTutore, pnlTabTutore);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("DelegatoTutore, Errore nel metodo Page_PreRender " + ex);
            }
        }

        private void CaricaDelegato()
        {
            PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.delegato = new AreaRispostaRiepilogo();
            this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            presenterDelegatoTutore.CaricaDelegato(this);
            ucDelegato.delegato = this.delegato;
            ucDelegato.ValorizzaEtichetteDelegato(this);
            return;
        }

        private void CaricaTutore()
        {
            PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.tutore = new AreaRispostaRiepilogo();
            this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            presenterDelegatoTutore.CaricaTutore(this);
            ucTutore.tutore = this.tutore;
            ucTutore.ValorizzaEtichetteTutore(this);
            return;
        }

        protected void SalvaDati_Click(object sender, EventArgs e)
        {
            try
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                String lblCFDel = ((Label)ucDelegato.FindControl("lblCFDelegato")).Text;
                String lblCFTut = ((Label)ucTutore.FindControl("lblCFTutore")).Text;
                if (!(String.IsNullOrEmpty(lblCFDel) && String.IsNullOrEmpty(lblCFTut)))
                {
                    PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
                    delegato = new AreaRispostaRiepilogo();
                    tutore = new AreaRispostaRiepilogo();
                    if (!String.IsNullOrEmpty(lblCFDel))
                    {
                        delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                        delegato.AnagraficaTitolare = ucDelegato.GetDatiUcDelegato();

                    }
                    if (!String.IsNullOrEmpty(lblCFTut))
                    {
                        tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                        tutore.AnagraficaTitolare = ucTutore.GetDatiUcTutore();

                    }
                    presenterDelegatoTutore.SalvaDelegatoTutore(this);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Impossibile salvare prima di aver effettuato la ricerca.";
                }
                if (HasError)
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = ErrorMessage;
                    return;
                }
                else
                {
                    if (tutore.AnagraficaTitolare != null)
                        ucTutore.AbilitaPulsanteEliminaTutore(true);

                    if (delegato.AnagraficaTitolare != null)
                        ucDelegato.AbilitaPulsanteEliminaDelegato(true);

                    //ENG - Reversibilita 024
                    DisabilitaTabDelegato();
                    ucAvviso.Tipo = TipoAvviso.Ok;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Dati Delegato / Tutore salvati correttamente";
                }

                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("DelegatoTutore, Errore nel metodo SalvaDati_Click" + ex);
            }
        }

        protected void event_ErrorUcDelegato(Object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            if (e == null)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = ucDelegato.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Info;
                ucAvviso.Messaggio = sender.ToString();
            }
        }

        protected void event_NotErrorUcDelegato(Object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = "";
        }


        protected void event_ErrorUcTutore(Object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            if (e == null)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = ucTutore.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Info;
                ucAvviso.Messaggio = sender.ToString();
            }
        }

        protected void event_NotErrorUcTutore(Object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = "";
        }

        protected void event_ucShowAvvisoDelegato(object sender, EventArgs e)
        {

            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCDelegato tabDelegato = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCDelegato)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDelegato.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDelegato.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Delegato salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoTutore(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCTutore tabTutore = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCTutore)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabTutore.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabTutore.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Tutore salvati correttamente";
            }
        }

        //Elimina

        protected void event_ucShowAvvisoEliminaDelegato(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCDelegato tabDelegato = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCDelegato)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDelegato.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDelegato.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Delegato eliminati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaTutore(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCTutore tabTutore = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCTutore)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabTutore.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabTutore.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Tutore eliminati correttamente";
            }
        }

        //

        public string CodiceFiscale
        {
            get
            {
                if (Session["Anagrafica"] != null)
                {
                    AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = Session["Anagrafica"] as AreaRispostaRiepilogo.DatiRiepilogoAnagrafica;
                    if (titolare != null)
                        return titolare.CodiceFiscale;
                }

                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Informazioni sul titolare non disponibili";

                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                CodeUtility.BloccaForm(this.domanda, this.Page);

                return string.Empty;
            }
        }

        //ENG - Reversibilita 024
        protected void event_GestisciDisabilitazioneTabDelegato(object sender, EventArgs e)
        {
            DisabilitaTabDelegato();
        }

        private void DisabilitaTabDelegato()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (Utility.IsDomandaReversibilita(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                this.areaInfoPratica = new AreaInfoPratica();
                List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
                elencoTab.Add(AreaQuadri.Tab.DelegatoTutore);
                this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

                CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];

                    if (this.areaQuadri != null && this.areaQuadri.QuadroDelegatoTutore != null && this.areaQuadri.QuadroDelegatoTutore.TabTutore == AreaQuadri.Semaforo.Verde)
                    {
                        ucDelegato.AbilitaTabDelegato(false);
                        ucDelegato.DisabilitaPannelli();
                    }
                    else
                    {
                        ucDelegato.AbilitaTabDelegato(true);
                        ucDelegato.AbilitaPannelli();
                    }
                }
            }
        }
    }
}
