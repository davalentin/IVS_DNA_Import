using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCDatiCalcoloStoricoGP_AGO : CustomBaseUserControl, IDatiContributiviAgo
    {
        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility.BloccaForm(this.domanda, pnlDomandeAUT);
            CodeUtility.BloccaForm(this.domanda, pnlImportoLordoDecorrenza);
            CodeUtility.BloccaForm(this.domanda, pnlContributoSolidarieta);
            CodeUtility.BloccaForm(this.domanda, pnlDatiCalcoloAPESociale);
        }

        internal void ValorizzaEtichette()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            VS_AreaDatiContributiviAgo = this.areaDatiContributiviAgo;

            LoadDecodificaData();

            if (Utility.IsDomandaCumulo(this.domanda.Categoria))
                BindDataForPanelsCumulo();
            else if (Utility.IsDomandaDAI(this.domanda.Categoria))
                BindDataForPanelsDAI();
            else
                BindDataForPanelsAGO();
        }

        #region private methods
        private void LoadDecodificaData()
        {
            VS_DecCodeGestioneCalcoloRetrib = this.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloRetributivo;
            VS_DecCodeGestioneCalcoloContrib = this.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloContributivo;
            if (this.areaDatiContributiviAgo.DatiExINPDAIStorico != null)
            {
                VS_CtrlDecorrenzaRetrExINPDAI = this.areaDatiContributiviAgo.DatiExINPDAIStorico.CtrlDecorrenzaRetrExINPDAI.ToList();
                VS_DecorrenzaCalcoloRetr = this.areaDatiContributiviAgo.DatiExINPDAIStorico.DecorrenzaCalcoloRetr;
            }
        }

        private void BindDataForPanelsAGO()
        {
            pnlAGO.Visible = true;

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (this.areaDatiContributiviAgo.IsPnlImportoLordoAllaDecVisible)
            {
                //VESO92 FILTRO L92
                pnlImportoLordoDecorrenza.Visible = true;
                pdivRetributivo.Visible = false;
                pdivContributivo.Visible = false;
                pnlDomandeAUT.Visible = false;
            }
            else if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
            {
                pnlDatiCalcoloAPESociale.Visible = true;
                pnlImportoLordoDecorrenza.Visible = false;
                pdivRetributivo.Visible = false;
                pdivContributivo.Visible = false;
                pnlDomandeAUT.Visible = false;
            }
            else if (this.areaDatiContributiviAgo.DatiCalcoloStorico.IsUnicarpe)
            {
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi != null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi != null) // misto
                {
                    pdivRetributivo.Visible = true;
                    pdivContributivo.Visible = true;
                    InitBindDataContributivi();
                    InitBindDataRetributivi();
                }
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi == null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi != null) // retributivo
                {
                    pdivRetributivo.Visible = true;
                    pdivContributivo.Visible = false;
                    InitBindDataRetributivi();
                }
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi != null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi == null) // contributivo         
                {
                    pdivRetributivo.Visible = false;
                    pdivContributivo.Visible = true;
                    InitBindDataContributivi();
                }
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi == null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi == null) // non valido
                {
                    pdivRetributivo.Visible = false;
                    pdivContributivo.Visible = false;
                }
            }
            else
            {
                switch (this.areaDatiContributiviAgo.DatiCalcoloStorico.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();
                        if (Utility.IsDomandaAUT(this.domanda.Categoria))
                            pnlDomandeAUT.Visible = true;
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        pdivRetributivo.Visible = true;
                        InitBindDataRetributivi();

                        if (this.areaDatiContributiviAgo.IsFineAssicurazionePost2012)
                        {
                            pdivContributivo.Visible = true;
                            InitBindDataContributivi();
                        }
                        else
                        {
                            //Riferimento mail: FW: Reeng Pensioni AGO - Modifiche applicative inabilità del 14/01/2014
                            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione && datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue &&
                                Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)))
                            {
                                pdivContributivo.Visible = true;
                                InitBindDataContributivi();
                            }
                            else
                                pdivContributivo.Visible = false;
                        }
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                        pdivRetributivo.Visible = true;
                        pdivContributivo.Visible = true;
                        InitBindDataRetributivi();
                        InitBindDataContributivi();
                        break;
                    case GestioneContribTipoCalcolo.NonValido:
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = false;
                        break;
                }
            }

            if (this.areaDatiContributiviAgo.DatiCalcoloStorico != null)
            {
                if (Utility.IsDomandaAUT(this.domanda.Categoria))
                    ddlFacoltaComputo.SelectedValue = this.areaDatiContributiviAgo.DatiCalcoloStorico.FacoltaComputo == true ? "SI" : "NO";

                if (this.areaDatiContributiviAgo.IsPnlImportoLordoAllaDecVisible)
                {
                    if (this.areaDatiContributiviAgo.DatiCalcoloStorico.ImportoLordoAllaDecorrenza.HasValue)
                        txtImportoLordoAllaDecorrenza.Text = this.areaDatiContributiviAgo.DatiCalcoloStorico.ImportoLordoAllaDecorrenza.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }
                if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
                {
                    if (this.areaDatiContributiviAgo.DatiCalcoloStorico.ImportoLordo.HasValue)
                        txtImportoLordo.Text = this.areaDatiContributiviAgo.DatiCalcoloStorico.ImportoLordo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }
            }
        }

        private void BindDataForPanelsCumulo()
        {
            pnlCumulo.Visible = true;

            VS_DecEnteGestioneFondo = this.areaDatiContributiviAgo.listaDecEnteGestioneFondo.ToList();
            VS_IsScaricoTrattenuteCumulo = areaDatiContributiviAgo.IsScaricoTrattenuteCumulo.GetValueOrDefault();
            SetViewStateIsRicostituzione();

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico != null && this.areaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico.LQuotePensione != null)
                InitGvQuotePensione(this.areaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico.LQuotePensione.ToList());


            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcoloQuoteMiglioramentiContrattualiStorico != null && this.areaDatiContributiviAgo.DatiCalcoloQuoteMiglioramentiContrattualiStorico.LQuoteMiglioramentiContrattuali != null)
                InitGvQuoteMiglioramentiContrattuali(this.areaDatiContributiviAgo.DatiCalcoloQuoteMiglioramentiContrattualiStorico.LQuoteMiglioramentiContrattuali.ToList());
        }

        private void BindDataForPanelsDAI()
        {
            pnlDAI.Visible = true;

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (this.areaDatiContributiviAgo.DatiCalcoloStorico.IsUnicarpe)
            {
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi != null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi != null) // misto
                {
                    pDivRetributivoDAI.Visible = true;
                    pDivContributivoDAI.Visible = true;
                    InitBindDataContributiviDAI();
                    InitBindDataRetributiviDAI();
                }
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi == null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi != null) // retributivo
                {
                    pDivRetributivoDAI.Visible = true;
                    pDivContributivoDAI.Visible = false;
                    InitBindDataRetributiviDAI();
                }
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi != null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi == null) // contributivo         
                {
                    pDivRetributivoDAI.Visible = false;
                    pDivContributivoDAI.Visible = true;
                    InitBindDataContributiviDAI();

                }
                if (this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi == null && this.areaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi == null) // non valido
                {
                    pDivRetributivoDAI.Visible = false;
                    pDivContributivoDAI.Visible = false;
                }
            }
            else
            {
                switch (this.areaDatiContributiviAgo.DatiCalcoloStorico.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        pDivRetributivoDAI.Visible = false;
                        pDivContributivoDAI.Visible = true;
                        pnlContributoSolidarieta.Visible = false;
                        InitBindDataContributiviDAI();
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        pDivRetributivoDAI.Visible = true;
                        InitBindDataRetributiviDAI();

                        if (this.areaDatiContributiviAgo.IsFineAssicurazionePost2012)
                        {
                            pDivContributivoDAI.Visible = true;
                            InitBindDataContributiviDAI();
                        }
                        else
                        {
                            //Rifeerimento mail: FW: Reeng Pensioni AGO - Modifiche applicative inabilità del 14/01/2014
                            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione && datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue &&
                                Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2011, 12, 31)))
                            {
                                pDivContributivoDAI.Visible = true;
                                InitBindDataContributiviDAI();
                            }
                            else
                                pDivContributivoDAI.Visible = false;
                        }
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                        pDivRetributivoDAI.Visible = true;
                        pDivContributivoDAI.Visible = true;
                        InitBindDataRetributiviDAI();
                        InitBindDataContributiviDAI();
                        break;
                    case GestioneContribTipoCalcolo.NonValido:
                        pDivRetributivoDAI.Visible = false;
                        pDivContributivoDAI.Visible = false;
                        break;
                }
            }

            if (this.areaDatiContributiviAgo.DatiExINPDAIStorico != null && !this.areaDatiContributiviAgo.DatiExINPDAIStorico.IsContribSolidarietaVisible)
                pnlContributoSolidarieta.Visible = false;

            if (this.areaDatiContributiviAgo.DatiExINPDAIStorico != null)
            {
                if (this.areaDatiContributiviAgo.DatiExINPDAIStorico.AnzAl95.HasValue)
                    txtAnzAl95.Text = this.areaDatiContributiviAgo.DatiExINPDAIStorico.AnzAl95.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                else if (this.areaDatiContributiviAgo.DatiCalcoloStorico.IsUnicarpe)
                    txtAnzAl95.Text = (0.9999M).ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (this.areaDatiContributiviAgo.DatiExINPDAIStorico.QuotaAl95.HasValue)
                    txtQuotaAl95.Text = this.areaDatiContributiviAgo.DatiExINPDAIStorico.QuotaAl95.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                else if (this.areaDatiContributiviAgo.DatiCalcoloStorico.IsUnicarpe)
                    txtQuotaAl95.Text = (0.9999M).ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private void InitBindDataRetributivi()
        {
            List<DatiRetributiviLocal> elencoDatiRetributivi = new List<DatiRetributiviLocal>();

            if (VS_AreaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi != null)
                elencoDatiRetributivi = MapDatiRetributiviForView(VS_AreaDatiContributiviAgo);

            gvDatiRetributivi.DataSource = elencoDatiRetributivi;
            gvDatiRetributivi.DataBind();
        }

        private void InitBindDataRetributiviDAI()
        {
            List<DatiRetributiviDAILocal> elencoDatiRetributivi = new List<DatiRetributiviDAILocal>();

            if (VS_AreaDatiContributiviAgo.DatiCalcoloStorico.lDatiRetributivi != null)
                elencoDatiRetributivi = MapDatiRetributiviDAIForView(VS_AreaDatiContributiviAgo);

            gvDatiRetributiviDAI.DataSource = elencoDatiRetributivi;
            gvDatiRetributiviDAI.DataBind();
        }

        private void InitBindDataContributivi()
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();

            if (VS_AreaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi != null)
                elencoDatiContributivi = MapDatiContributiviForView(VS_AreaDatiContributiviAgo);

            gvDatiContributivi.DataSource = elencoDatiContributivi;
            gvDatiContributivi.DataBind();
        }

        private void InitBindDataContributiviDAI()
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();

            if (VS_AreaDatiContributiviAgo.DatiCalcoloStorico.lDatiContributivi != null)
                elencoDatiContributivi = MapDatiContributiviForView(VS_AreaDatiContributiviAgo);

            gvDatiContributiviDAI.DataSource = elencoDatiContributivi;
            gvDatiContributiviDAI.DataBind();
        }

        private void InitGvQuotePensione(List<GestioneContribDatiQuotePensione> lstServer)
        {
            List<QuotePensioneLocal> lstQuotePensione = new List<QuotePensioneLocal>();

            if (lstServer != null && lstServer.Count() > 0)
                lstQuotePensione.AddRange(MapServiceToLocalObject(lstServer));

            gvQuotePensione.DataSource = lstQuotePensione;
            gvQuotePensione.DataBind();
        }

        private void InitGvQuoteMiglioramentiContrattuali(List<GestioneContribDatiQuoteMiglioramentiContrattuali> lstServer)
        {
            List<QuoteMiglioramentiContrattualiLocal> lstQuoteMiglioramentiContrattuali = new List<QuoteMiglioramentiContrattualiLocal>();

            if (lstServer != null && lstServer.Count() > 0)
                lstQuoteMiglioramentiContrattuali.AddRange(MapQuoteMiglioramentiToLocalObject(lstServer));

            divQuoteMiglioramentiContrattuali.Visible = true;
            gvQuoteMiglioramentiContrattuali.DataSource = lstQuoteMiglioramentiContrattuali;
            gvQuoteMiglioramentiContrattuali.DataBind();
        }

        private List<DatiContributiviLocal> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributiviLocal> elencoDatiContributivi = new List<DatiContributiviLocal>();
            foreach (GestioneAggiornamentoPECODatiContributivi contr in areaDatiContributivi.DatiCalcoloStorico.lDatiContributivi.ToList<GestioneAggiornamentoPECODatiContributivi>())
            {
                string settimana = string.Empty;
                string importo = string.Empty;
                string montante = string.Empty;
                string PL_Quotac = string.Empty;

                if (contr.Quota.HasValue)
                {
                    if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        settimana = contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivo.HasValue ? contr.ImportoContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteContributivo.HasValue ? contr.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        settimana = contr.SettimaneQuotaD.HasValue ? contr.SettimaneQuotaD.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivoQuotaD.HasValue ? contr.ImportoContributivoQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteContributivoQuotaD.HasValue ? contr.MontanteContributivoQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }

                PL_Quotac = contr.PL_Quotac.HasValue ? contr.PL_Quotac.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                elencoDatiContributivi.Add(new DatiContributiviLocal(contr.CodGestione.HasValue ? contr.CodGestione.Value.ToString() : string.Empty,
                    contr.Quota.HasValue ? contr.Quota.Value.ToString() : string.Empty,
                    settimana, importo, montante, PL_Quotac));
            }
            return elencoDatiContributivi;
        }

        private List<DatiRetributiviLocal> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviLocal> elencoDatiRetributivi = new List<DatiRetributiviLocal>();
            foreach (GestioneAggiornamentoPECODatiRetributivi retr in areaDatiContributivi.DatiCalcoloStorico.lDatiRetributivi.ToList<GestioneAggiornamentoPECODatiRetributivi>())
            {
                string settimana = string.Empty;
                string rmsQuota = string.Empty;
                string PL_Quotar = string.Empty;

                if (retr.Quota.HasValue)
                {
                    if (retr.Quota.HasValue && retr.Quota.Value.ToString().ToUpperInvariant() == "A")
                    {
                        settimana = retr.SettimaneA.HasValue ? retr.SettimaneA.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaA.HasValue ? retr.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (retr.Quota.HasValue && retr.Quota.Value.ToString().ToUpperInvariant() == "B")
                    {
                        settimana = retr.SettimaneB.HasValue ? retr.SettimaneB.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaB.HasValue ? retr.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }

                PL_Quotar = retr.PL_Quotar.HasValue ? retr.PL_Quotar.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                elencoDatiRetributivi.Add(new DatiRetributiviLocal(retr.CodGestione.HasValue ? retr.CodGestione.Value.ToString() : string.Empty,
                    retr.Quota.HasValue ? retr.Quota.Value.ToString() : string.Empty,
                    settimana, string.Empty, rmsQuota, retr.NSettimane707.HasValue ? retr.NSettimane707.Value.ToString() : string.Empty, PL_Quotar));
            }
            return elencoDatiRetributivi;
        }

        private List<DatiRetributiviDAILocal> MapDatiRetributiviDAIForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviDAILocal> elencoDatiRetributivi = new List<DatiRetributiviDAILocal>();
            foreach (GestioneAggiornamentoPECODatiRetributivi retr in areaDatiContributivi.DatiCalcoloStorico.lDatiRetributivi.ToList<GestioneAggiornamentoPECODatiRetributivi>())
            {
                string settimana = string.Empty;
                string rmsQuota = string.Empty;
                string codiceGestione = retr.CodGestione.HasValue ? retr.CodGestione.Value.ToString() : string.Empty;
                string quota = retr.Quota.HasValue ? retr.Quota.Value.ToString() : string.Empty;
                string tipoQuota = !string.IsNullOrEmpty(retr.CodiceTipoQuota) ? retr.CodiceTipoQuota : null;
                string decorrenza = GetDecorrenzaRetributiva(codiceGestione, quota, tipoQuota);
                if (!string.IsNullOrEmpty(quota))
                {
                    if (quota.ToUpperInvariant() == "A")
                    {
                        settimana = retr.SettimaneA.HasValue ? retr.SettimaneA.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaA.HasValue ? retr.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (quota.ToUpperInvariant() == "B")
                    {
                        settimana = retr.SettimaneB.HasValue ? retr.SettimaneB.Value.ToString() : string.Empty;
                        rmsQuota = retr.RMSQuotaB.HasValue ? retr.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                }
                string strSett707 = retr.NSettimane707.HasValue ? retr.NSettimane707.ToString() : string.Empty;
                elencoDatiRetributivi.Add(new DatiRetributiviDAILocal(codiceGestione, quota, tipoQuota, settimana, decorrenza, rmsQuota, strSett707));
            }
            return elencoDatiRetributivi;
        }

        public List<QuotePensioneLocal> MapServiceToLocalObject(List<GestioneContribDatiQuotePensione> lstService)
        {
            List<QuotePensioneLocal> lstLocal = new List<QuotePensioneLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (GestioneContribDatiQuotePensione elemS in lstService)
                {
                    QuotePensioneLocal elemL = new QuotePensioneLocal();
                    elemL.ImportoQuota = elemS.Importo.ToString();
                    elemL.Settimane = elemS.Settimane.ToString();
                    elemL.EnteGestioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Codice;
                    elemL.DescrizioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Ente;
                    elemL.IsTrattenute = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).IsTrattenuteAmmesse.GetValueOrDefault();
                    elemL.Decorrenza = string.Format("{0:dd/MM/yyyy}", elemS.Decorrenza);
                    if (elemS.ListaTrattenute != null && elemS.ListaTrattenute.Count() > 0)
                    {
                        elemL.ListaTrattenute = new List<QuotePensioneLocal.TrattenuteLocal>();
                        foreach (GestioneContribDatiQuotePensione.DatiTrattenute subElemS in elemS.ListaTrattenute)
                        {
                            QuotePensioneLocal.TrattenuteLocal subElemL = new QuotePensioneLocal.TrattenuteLocal();
                            subElemL.AnnoCompetenza = subElemS.AnnoCompetenza.ToString();
                            subElemL.CodiceTrattenute = subElemS.CodiceTrattenute;
                            subElemL.ImportoTrattenute = subElemS.ImportoTrattenute.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                            elemL.ListaTrattenute.Add(subElemL);
                        }
                    }
                    lstLocal.Add(elemL);
                }
            }
            return lstLocal;
        }

        public List<QuoteMiglioramentiContrattualiLocal> MapQuoteMiglioramentiToLocalObject(List<GestioneContribDatiQuoteMiglioramentiContrattuali> lstService)
        {
            List<QuoteMiglioramentiContrattualiLocal> lstLocal = new List<QuoteMiglioramentiContrattualiLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (GestioneContribDatiQuoteMiglioramentiContrattuali elemS in lstService)
                {
                    QuoteMiglioramentiContrattualiLocal elemL = new QuoteMiglioramentiContrattualiLocal();
                    elemL.Quota = elemS.Quota.ToString();
                    elemL.Codice = elemS.Codice;
                    elemL.DataDecorrenza = string.Format("{0:dd/MM/yyyy}", elemS.DataDecorrenza);

                    lstLocal.Add(elemL);
                }
            }
            return lstLocal;
        }

        private string GetValueFromIdRetr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = VS_DecCodeGestioneCalcoloRetrib;
                DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate(DecodificaGestioneCalcoloRetributivo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private string GetValueFromIdContr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = VS_DecCodeGestioneCalcoloContrib;
                DecodificaGestioneCalcoloContributivo app = listaCodeGestioneCalcoloContrib.ToList().Find(delegate(DecodificaGestioneCalcoloContributivo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private string GetDecorrenzaRetributiva(string codiceGestione, string quota, string tipoQuota)
        {
            string decorrenza = string.Empty;
            string codiceGestioneTraduzioneSuGP = string.Empty;

            if (string.IsNullOrEmpty(tipoQuota))
                tipoQuota = null;

            List<DecodificaGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetrib = VS_DecCodeGestioneCalcoloRetrib.ToList();
            DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.Find(delegate(DecodificaGestioneCalcoloRetributivo code) { return (code.Id.ToString() == codiceGestione); });
            if (app != null)
                codiceGestioneTraduzioneSuGP = app.TraduzioneSuGP.Trim();

            List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExINPDAI = VS_CtrlDecorrenzaRetrExINPDAI;
            if (listaCtrlDecorrenzaRetrExINPDAI != null && listaCtrlDecorrenzaRetrExINPDAI.Count > 0)
            {
                CtrlDecorrenzaRetrExINPDAI obj = listaCtrlDecorrenzaRetrExINPDAI.Find(x => x.Gestione.Trim() == codiceGestioneTraduzioneSuGP && x.Quota.ToString() == quota && x.TipoQuota == tipoQuota);
                if (obj != null)
                {
                    UtilityDifferenzaDateTime data = VS_DecorrenzaCalcoloRetr;

                    if (data != null)
                    {
                        if (obj.CodiceDecorrenza != 76)
                            decorrenza = obj.CodiceDecorrenza.ToString() + "/" + data.Year;
                        else
                            decorrenza = String.Format("{0:00}/{1:0000}", data.Month, data.Year);
                    }
                }
            }

            return decorrenza;
        }

        private void SetViewStateIsRicostituzione()
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                VS_IsRicostituzione = true;
            else
                VS_IsRicostituzione = false;
        }

        private bool IsVisualizzaTrattenute()
        {
            bool isVisible = false;
            if (VS_IsScaricoTrattenuteCumulo)
                isVisible = VS_AreaDatiContributiviAgo != null && VS_AreaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico != null && VS_AreaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico.LQuotePensione != null && VS_AreaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico.LQuotePensione.Count() > 0 && VS_AreaDatiContributiviAgo.DatiCalcoloQuotePensioneStorico.LQuotePensione.Any(x => x.ListaTrattenute != null && x.ListaTrattenute.Count() > 0);
            return isVisible;
        }
        #endregion private methods

        #region gvDatiContributivi

        protected void gvDatiContributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Quota;
                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdContr(((DatiContributiviLocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviLocal)(e.Row.DataItem)).Settimane;
                    ((Label)e.Row.FindControl("lblAmmontareContributivo")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).AmmontareContributivo, 4);
                    ((Label)e.Row.FindControl("lblMontanteContributivo")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).MontanteContributivo, 4);
                    if(((Label)e.Row.FindControl("lblQuotaContributiva")) != null) ((Label)e.Row.FindControl("lblQuotaContributiva")).Text = !string.IsNullOrEmpty(((DatiContributiviLocal)(e.Row.DataItem)).QuotaContributiva) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviLocal)(e.Row.DataItem)).QuotaContributiva, 4) : "";
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloStoricoGP_AGO, Errore nel metodo gvDatiContributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiContributivi_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaDatiContributivi areaDatiContributiviAgo = VS_AreaDatiContributiviAgo;

            if (this.domanda != null && areaDatiContributiviAgo != null)
            {
                if (areaDatiContributiviAgo.DatiCalcoloStorico != null && areaDatiContributiviAgo.DatiCalcoloStorico.IsUnicarpe && 
                    (Utility.IsDomandaVOPGI(this.domanda.Categoria) || Utility.IsDomandaIOPGI(this.domanda.Categoria)) &&
                    (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura))
                    gvDatiContributivi.Columns[(int)ColonneGvDatiContributivi.QuotaContr].Visible = true;
            }
        }

        #endregion gvDatiContributivi

        #region gvDatiRetributivi

        protected void gvDatiRetributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributiviLocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Quota;
                    ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Decorrenza;
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane;
                    ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).RetribuzioneMedia, 4);
                    ((Label)e.Row.FindControl("lblSettimane707")).Text = ((DatiRetributiviLocal)(e.Row.DataItem)).Settimane707;
                    ((Label)e.Row.FindControl("lblQuoteRetributivo")).Text = !string.IsNullOrEmpty(((DatiRetributiviLocal)(e.Row.DataItem)).QuoteRetributivo) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviLocal)(e.Row.DataItem)).QuoteRetributivo, 4) : "";
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloStoricoGP_AGO, Errore nel metodo gvDatiRetributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiRetributivi_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaDatiContributivi areaDatiContributiviAgo = VS_AreaDatiContributiviAgo;
            if (areaDatiContributiviAgo != null)
                gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Sett707].Visible = areaDatiContributiviAgo.IsSettimane707Visible;

            if (this.domanda != null && areaDatiContributiviAgo != null)
            {
                if (areaDatiContributiviAgo.DatiCalcoloStorico != null && areaDatiContributiviAgo.DatiCalcoloStorico.IsUnicarpe && 
                    (Utility.IsDomandaVOPGI(this.domanda.Categoria) || Utility.IsDomandaIOPGI(this.domanda.Categoria)) &&
                    (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura))
                    gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.QuoteRet].Visible = true;
            }
        }

        #endregion gvDatiRetributivi

        #region gvQuotePensione

        protected void gvQuotePensione_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            this.areaDatiContributiviAgo = VS_AreaDatiContributiviAgo;
            GridView gvQuotePensione = (GridView)sender;
            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.TipoCumulo.HasValue && !this.areaDatiContributiviAgo.TipoCumulo.Value)
                gvQuotePensione.Columns[ColonneGvQuotePensione.Decorrenza.GetHashCode()].Visible = true;
            gvQuotePensione.Columns[ColonneGvQuotePensione.VisualizzaTrattenute.GetHashCode()].Visible = IsVisualizzaTrattenute();
        }

        protected void gvQuotePensione_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                    QuotePensioneLocal row = ((QuotePensioneLocal)(e.Row.DataItem));
                    if (row != null)
                    {
                        ((Label)e.Row.FindControl("lblEnteGestioneFondo")).Text = row.EnteGestioneFondo;
                        ((Label)e.Row.FindControl("lblDescrizioneFondo")).Text = row.DescrizioneFondo;
                        if (!datiPensione.IsDomandaCumuloAutomatica || row.Decorrenza != string.Format("{0:dd/MM/yyyy}", new DateTime(9999, 1, 1)))
                            ((Label)e.Row.FindControl("lblDecorrenzaQuota")).Text = row.Decorrenza;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = row.Settimane;
                        ((Label)e.Row.FindControl("lblImportoQuota")).Text = row.ImportoQuota;
                        if (VS_IsScaricoTrattenuteCumulo)
                        {
                            List<QuotePensioneLocal.TrattenuteLocal> listaTrattenute = row.ListaTrattenute;
                            if (listaTrattenute != null && listaTrattenute.Count > 0)
                            {
                                ((Image)e.Row.FindControl("imgVisualizzaTrattenute")).Visible = true;
                                GridView gvTrattenute = ((GridView)e.Row.FindControl("gvTrattenute"));
                                gvTrattenute.DataSource = listaTrattenute;
                                gvTrattenute.DataBind();
                            }
                            else
                                ((Image)e.Row.FindControl("imgVisualizzaTrattenute")).Visible = false;
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloStoricoGP_AGO, Errore nel metodo gvQuotePensione_RowDataBound " + ex);
            }
        }

        #endregion gvQuotePensione

        #region gvQuoteMiglioramentiContrattuali

        protected void gvQuoteMiglioramentiContrattuali_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            this.areaDatiContributiviAgo = VS_AreaDatiContributiviAgo;
            GridView gvQuoteMiglioramentiContrattuali = (GridView)sender;
        }

        protected void gvQuoteMiglioramentiContrattuali_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                    QuoteMiglioramentiContrattualiLocal row = ((QuoteMiglioramentiContrattualiLocal)(e.Row.DataItem));
                    if (row != null)
                    {
                        ((Label)e.Row.FindControl("lblEnteGestioneFondoMiglioramenti")).Text = row.Codice;
                        ((Label)e.Row.FindControl("lblDecorrenzaQuotaMiglioramenti")).Text = row.DataDecorrenza;
                        ((Label)e.Row.FindControl("lblImportoQuotaMiglioramenti")).Text = row.Quota;

                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloStoricoGP_AGO, Errore nel metodo gvQuoteMiglioramentiContrattuali_RowDataBound " + ex);
            }
        }

        #endregion gvQuoteMiglioramentiContrattuali

        #region gvDatiRetributiviDAI

        protected void gvDatiRetributiviDAI_Load(object sender, EventArgs e)
        {
            AreaDatiContributivi areaDatiContributiviAgo = VS_AreaDatiContributiviAgo;
            if (areaDatiContributiviAgo != null)
                gvDatiRetributiviDAI.Columns[(int)ColonneGvDatiRetributiviDAI.Sett707].Visible = areaDatiContributiviAgo.IsSettimane707Visible;
        }

        protected void gvDatiRetributiviDAI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text = GetValueFromIdRetr(((DatiRetributiviDAILocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblCodiceGestione_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione_item")).Text);
                    ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiRetributiviDAILocal)(e.Row.DataItem)).Quota;
                    ((Label)e.Row.FindControl("lblTipoQuota_item")).Text = ((DatiRetributiviDAILocal)(e.Row.DataItem)).TipoQuota;
                    ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((DatiRetributiviDAILocal)(e.Row.DataItem)).Decorrenza;
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviDAILocal)(e.Row.DataItem)).Settimane;
                    ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviDAILocal)(e.Row.DataItem)).RetribuzioneMedia, 6);
                    ((Label)e.Row.FindControl("LblSettimane707")).Text = ((DatiRetributiviDAILocal)(e.Row.DataItem)).Settimane707;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloStoricoGP_AGO, Errore nel metodo gvDatiRetributiviDAI_RowDataBound " + ex);
            }
        }

        #endregion gvDatiRetributiviDAI

        #region enum
        private enum EnumViewState
        {
            AreaDatiContributiviAgo,
            ListaCodeGestioneCalcoloRetrib,
            ListaCodeGestioneCalcoloContrib,
            DecodificaEnteGestioneFondo,
            CtrlDecorrenzaRetrExINPDAI,
            DecorrenzaCalcoloRetr,
            IsScaricoTrattenuteCumulo,
            IsRicostituzione,
        }

        public enum ColonneGvDatiRetributivi { Sett707 = 5, QuoteRet = 6 };

        public enum ColonneGvDatiRetributiviDAI { Sett707 = 6 };

        public enum ColonneGvQuotePensione { Decorrenza = 2, VisualizzaTrattenute = 5 };

        public enum ColonneGvDatiContributivi { QuotaContr = 5 };
        #endregion enum

        #region ViewState Variables
        /// <summary>
        /// Area Dati calcolo
        /// </summary>
        private AreaDatiContributivi VS_AreaDatiContributiviAgo
        {
            get { return (AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]; }
            set { ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] = (AreaDatiContributivi)value; }

        }

        /// <summary>
        /// Decodifica CodeGestione Calcolo Retributivo
        /// </summary>
        private DecodificaGestioneCalcoloRetributivo[] VS_DecCodeGestioneCalcoloRetrib
        {
            get { return (DecodificaGestioneCalcoloRetributivo[])ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()]; }
            set { ViewState[EnumViewState.ListaCodeGestioneCalcoloRetrib.ToString()] = (DecodificaGestioneCalcoloRetributivo[])value; }

        }

        /// <summary>
        /// Decodifica CodeGestione Calcolo Contributivo
        /// </summary>
        private DecodificaGestioneCalcoloContributivo[] VS_DecCodeGestioneCalcoloContrib
        {
            get { return (DecodificaGestioneCalcoloContributivo[])ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()]; }
            set { ViewState[EnumViewState.ListaCodeGestioneCalcoloContrib.ToString()] = (DecodificaGestioneCalcoloContributivo[])value; }

        }

        /// <summary>
        /// Decodifica Quote
        /// </summary>
        private List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo> VS_DecEnteGestioneFondo
        {
            get { return (List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo>)ViewState[EnumViewState.DecodificaEnteGestioneFondo.ToString()]; }
            set { ViewState[EnumViewState.DecodificaEnteGestioneFondo.ToString()] = (List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo>)value; }

        }

        /// <summary>
        /// Decodifica CtrlDecorrenzaRetr per DAI
        /// </summary>
        private List<CtrlDecorrenzaRetrExINPDAI> VS_CtrlDecorrenzaRetrExINPDAI
        {
            get { return (List<CtrlDecorrenzaRetrExINPDAI>)ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()]; }
            set { ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()] = (List<CtrlDecorrenzaRetrExINPDAI>)value; }

        }

        /// <summary>
        /// Decorrenza Calcolo Retributivo
        /// </summary>
        private UtilityDifferenzaDateTime VS_DecorrenzaCalcoloRetr
        {
            get { return (UtilityDifferenzaDateTime)ViewState[EnumViewState.DecorrenzaCalcoloRetr.ToString()]; }
            set { ViewState[EnumViewState.DecorrenzaCalcoloRetr.ToString()] = (UtilityDifferenzaDateTime)value; }

        }

        private bool VS_IsScaricoTrattenuteCumulo
        {
            get { return (bool)ViewState[EnumViewState.IsScaricoTrattenuteCumulo.ToString()]; }
            set { ViewState[EnumViewState.IsScaricoTrattenuteCumulo.ToString()] = value; }
        }

        private bool VS_IsRicostituzione
        {
            get { return (bool)ViewState[EnumViewState.IsRicostituzione.ToString()]; }
            set { ViewState[EnumViewState.IsRicostituzione.ToString()] = value; }
        }

        #endregion ViewState Variables

        #region nested Class
        [Serializable]
        public class DatiContributiviLocal
        {
            public DatiContributiviLocal()
            { }
            public DatiContributiviLocal(string strGestione, string strQuota, string strSettimane, string strAmmontareContributivo, string strMontanteContributivo, string strQuotaContributiva)
            {
                this._strQuota = strQuota;
                this._strAmmontareContributivo = strAmmontareContributivo;
                this._strGestione = strGestione;
                this._strMontanteContributivo = strMontanteContributivo;
                this._strSettimane = strSettimane;
                this._strQuotaContributiva = strQuotaContributiva;
            }
            #region private properties
            private string _strQuota;
            private string _strGestione;
            private string _strSettimane;
            private string _strAmmontareContributivo;
            private string _strMontanteContributivo;
            private string _strQuotaContributiva;
            #endregion private properties

            #region public properties
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string AmmontareContributivo { get { return _strAmmontareContributivo; } set { _strAmmontareContributivo = value; } }
            public string MontanteContributivo { get { return _strMontanteContributivo; } set { _strMontanteContributivo = value; } }
            public string QuotaContributiva { get { return _strQuotaContributiva; } set { _strQuotaContributiva = value; } }
            #endregion public properties
        }

        [Serializable]
        public class DatiRetributiviLocal
        {
            public DatiRetributiviLocal()
            { }
            public DatiRetributiviLocal(string strGestione, string strQuota, string strSettimane, string strDecorrenza, string strRetribuzioneMedia, string strSettimane707, string strQuoteRetributivo)
            {
                this._strQuota = strQuota;
                this._strGestione = strGestione;
                this._strDecorrenza = strDecorrenza;
                this._strSettimane = strSettimane;
                this._strRetribuzioneMedia = strRetribuzioneMedia;
                this._settimane707 = strSettimane707;
                this._quoteRetributivo = strQuoteRetributivo;
            }

            #region private properties
            private string _strGestione;
            private string _strQuota;
            private string _strSettimane;
            private string _strDecorrenza;
            private string _strRetribuzioneMedia;
            private string _settimane707;
            private string _quoteRetributivo;

            #endregion private properties

            #region public properties
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string Decorrenza { get { return _strDecorrenza; } set { _strDecorrenza = value; } }
            public string RetribuzioneMedia { get { return _strRetribuzioneMedia; } set { _strRetribuzioneMedia = value; } }
            public string Settimane707 { get { return _settimane707; } set { _settimane707 = value; } }
            public string QuoteRetributivo { get { return _quoteRetributivo; } set { _quoteRetributivo = value; } }
            #endregion public properties

        }

        [Serializable]
        public class DatiRetributiviDAILocal
        {
            public DatiRetributiviDAILocal()
            { }
            public DatiRetributiviDAILocal(string strGestione, string strQuota, string strTipoQuota, string strSettimane, string strDecorrenza, string strRetribuzioneMedia, string strSettimane707)
            {
                this._strQuota = strQuota;
                this._strGestione = strGestione;
                this._strTipoQuota = strTipoQuota;
                this._strDecorrenza = strDecorrenza;
                this._strSettimane = strSettimane;
                this._strRetribuzioneMedia = strRetribuzioneMedia;
                this._settimane707 = strSettimane707;
            }

            #region private properties
            private string _strGestione;
            private string _strQuota;
            private string _strTipoQuota;
            private string _strSettimane;
            private string _strDecorrenza;
            private string _strRetribuzioneMedia;
            private string _settimane707;
            #endregion private properties

            #region public properties
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string TipoQuota { get { return _strTipoQuota; } set { _strTipoQuota = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string Decorrenza { get { return _strDecorrenza; } set { _strDecorrenza = value; } }
            public string RetribuzioneMedia { get { return _strRetribuzioneMedia; } set { _strRetribuzioneMedia = value; } }
            public string Settimane707 { get { return _settimane707; } set { _settimane707 = value; } }
            #endregion public properties
        }

        [Serializable]
        public class QuotePensioneLocal
        {
            public QuotePensioneLocal()
            { }

            public QuotePensioneLocal(string enteGestioneFondo, string settimane, string importoQuota, string decorrenza, string descrizione, bool isTrattenute)
            {
                this.ImportoQuota = importoQuota;
                this.Settimane = settimane;
                this.EnteGestioneFondo = enteGestioneFondo;
                this.DescrizioneFondo = descrizione;
            }

            public string Settimane { get; set; }
            public string ImportoQuota { get; set; }
            public string EnteGestioneFondo { get; set; }
            public string DescrizioneFondo { get; set; }
            public string Decorrenza { get; set; }
            public bool IsTrattenute { get; set; }
            public List<TrattenuteLocal> ListaTrattenute { get; set; }

            [Serializable]
            public class TrattenuteLocal
            {
                public string AnnoCompetenza { get; set; }
                public string CodiceTrattenute { get; set; }
                public string ImportoTrattenute { get; set; }

                public TrattenuteLocal() { }

                public TrattenuteLocal(string annoCompetenza, string codiceTrattenute, string importoTrattenute)
                {
                    this.AnnoCompetenza = annoCompetenza;
                    this.CodiceTrattenute = codiceTrattenute;
                    this.ImportoTrattenute = importoTrattenute;
                }
            }
        }

        [Serializable]
        public class QuoteMiglioramentiContrattualiLocal
        {
            public QuoteMiglioramentiContrattualiLocal() { }
            public long Id { get; set; }
            public long? IdPensione { get; set; }
            public string Codice { get; set; }
            public string DataDecorrenza { get; set; }
            public string Quota { get; set; }
            public bool IsStorico { get; set; }
        }

        #endregion nested Class
    }
}