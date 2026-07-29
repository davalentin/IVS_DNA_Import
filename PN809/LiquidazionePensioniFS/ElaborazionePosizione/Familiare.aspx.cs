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
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Familiari;


namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Familiare : CustomBasePage, IInfoLiquidazione, IQuadriSemafori
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ucFamiliari.TitolarePensione = new AreaTitolare();
            ucFamiliari.TitolarePensione.Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgRiepilogo, this.areaQuadri.QuadroFamiliari.TabFamiliari, pnlRiepilogoFamiliari);
            }
        }

        protected void FamiliariSalvati(object sender, EventArgs e)
        {
            ucAvviso.Visible = true;
            UCFamiliari ucFamiliari = (UCFamiliari)sender;
            if (ucFamiliari == null || ucFamiliari.areaEsito == null || string.IsNullOrEmpty(ucFamiliari.areaEsito.Messaggio))
            {
                ucAvviso.Messaggio = "Dati Familiari salvati correttamente";
                ucAvviso.Tipo = TipoAvviso.Ok;
            }
            else
            {
                ucAvviso.Messaggio = string.Format("Dati del Familiare {0}salvati correttamente. <br /><br />{1}", string.IsNullOrEmpty(ucFamiliari.codiceFiscale) ? string.Empty : ucFamiliari.codiceFiscale + ' ', ucFamiliari.areaEsito.Messaggio);
                ucAvviso.Tipo = TipoAvviso.Warning;
            }
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ValorizzaConsultazioneANF(ucFamiliari);

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        private void ValorizzaConsultazioneANF(UCFamiliari ucFamiliari)
        {
            lblConsultazioneANF.Text = string.Empty;
            if (ucFamiliari != null && ucFamiliari.consultazioneANF != null)
            {
                GestioneFamiliariConsultazioneUnificataANF consultazione = ucFamiliari.consultazioneANF;
                if (consultazione.listaDatiDomandaAnf != null && consultazione.listaDatiDomandaAnf.Count() > 0)
                {
                    string esitoConsultazione = string.Format("Per il soggetto {0}, con la consultazione effettuata il {1},", consultazione.codiceFiscaleRichiedente, consultazione.dataRichiestaRichiedente);
                    esitoConsultazione += " risultano domande:<br/><br/>";
                    Dictionary<string, List<GestioneFamiliariDomandaAnf>> listaFonti = consultazione.listaDatiDomandaAnf.GroupBy(x => x.codiceFonte).ToDictionary(y => y.Key, y => y.ToList());
                    if (listaFonti != null && listaFonti.Count() > 0)
                    {
                        foreach (string codiceFonte in listaFonti.Keys)
                        {
                            List<GestioneFamiliariDomandaAnf> datiDomanda = null;
                            listaFonti.TryGetValue(codiceFonte, out datiDomanda);
                            if (datiDomanda != null && datiDomanda.Count() > 0)
                            {
                                esitoConsultazione += string.Format("&nbsp;&nbsp;-&nbsp;&nbsp;Sulla prestazione <b>'{0}'</b>:<ul>", datiDomanda.FirstOrDefault().descrizioneFonte);
                                List<GestioneFamiliariDomandaAnf> listaDomandeFiltrata = new List<GestioneFamiliariDomandaAnf>();
                                foreach (GestioneFamiliariDomandaAnf domanda in datiDomanda)
                                {
                                    if (!listaDomandeFiltrata.Exists(x => (String.IsNullOrEmpty(x.periodoDataDa) && String.IsNullOrEmpty(x.periodoDataA) && String.IsNullOrEmpty(domanda.periodoDataDa) && String.IsNullOrEmpty(domanda.periodoDataA) || (!String.IsNullOrEmpty(x.periodoDataDa) && !String.IsNullOrEmpty(x.periodoDataA) && !String.IsNullOrEmpty(domanda.periodoDataDa) && !String.IsNullOrEmpty(domanda.periodoDataA) && x.periodoDataDa == domanda.periodoDataDa && x.periodoDataA == domanda.periodoDataA)) &&
                                                                        x.codicePratica1 == domanda.codicePratica1 && x.numeroProtocolloDomanda == domanda.numeroProtocolloDomanda &&
                                                                        x.statoDomanda == domanda.statoDomanda))
                                        listaDomandeFiltrata.Add(domanda);
                                }

                                if (listaDomandeFiltrata != null && listaDomandeFiltrata.Count() > 0)
                                {
                                    foreach (GestioneFamiliariDomandaAnf domanda in listaDomandeFiltrata)
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
                            }
                        }
                        lblConsultazioneANF.Text = esitoConsultazione;
                        ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpConsultazioneANF", "<script>ShowPopUpConsultazioneANF();</script>", false);
                    }
                }
            }
        }

        protected void FamiliariEliminati(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            UserControls.Familiari.UCFamiliari tabFamiliari = (UserControls.Familiari.UCFamiliari)sender;
            if (tabFamiliari.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabFamiliari.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Familiari eliminati correttamente";
            }
        }

        protected void FamiliariNonSalvati(object sender, EventArgs e)
        {
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = ((AreaEsito)(sender)).Messaggio;
            ucAvviso.Tipo = TipoAvviso.Warning;
        }

        protected void event_ucAddModFamiliare(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Info;
            ucAvviso.Messaggio = "Per rendere effettive le modifiche, cliccare su Salva Informazioni";
            ucAvviso.Visible = true;
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = ((UCFamiliari)sender).ErrorMessage;
        }
    }
}

