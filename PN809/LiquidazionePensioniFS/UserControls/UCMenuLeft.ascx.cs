using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCMenuLeft : CustomBaseUserControl, IQuadriSemafori, ITitolarePensione, IControlliDinamici, IDanteCausa
    {
        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IControlliDinamici
        public DateTime? DataSistema { get; set; }
        public DateTime? DataINDCOM { get; set; }
        #endregion IControlliDinamici

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {
            Image1.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";
            imgInviaAlCalcolo.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgStampa.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            Image3.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggCI05.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggWebDom.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggFelpe.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggOneri.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggSai.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggINPDAP.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggTotal.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggBooking.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggTot.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggNoteDebito.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgAggPianiDiPagamento.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            Image2.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            Image.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            imgPresaInCarico.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right2.png";
            //imgExit.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/exit.png";

            if ((bool)Session["isIframe"]) 
            {
                liHome.Visible = false;
                //liExit.Visible = false;
            }

            if (!IsPostBack)
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                string controlloDinamico = string.Empty;
                string controlloDinamicoAbilitazioneSpachettate024 = string.Empty;
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamico);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    controlloDinamicoAbilitazioneSpachettate024 = controlloDinamico;

                //ENG - Spacchettate SOPGI
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
                datiPensione = GetDatiPensione(this);

                if (this.domanda.Categoria.Trim() == "SOPGI")
                {
                    if (this.areaDanteCausa == null)
                    {
                        PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                        presenterDanteCausa.GetDatiDanteCausa(this);
                    }
                }

                if (((!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.StartsWith("S")) || Utility.IsDomandaSuperstiti(this.domanda.CodGruppo)) && !this.domanda.IsDomandaENPALS && !this.domanda.IsDomandaINPDAP
                    && !(!String.IsNullOrEmpty(controlloDinamicoAbilitazioneSpachettate024) && controlloDinamicoAbilitazioneSpachettate024.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione))
                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa)
                    && !Utility.IsDomandaSpacchettamentoSO(datiPensione, this.domanda) && !Utility.IsDomandaSpacchettamentoSOART(datiPensione, this.domanda) && !Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, this.domanda)
                    && !Utility.IsDomandaSpacchettamentoSR(datiPensione, this.domanda))
                    lblFamiliare.Text = "Contitolari";


                if (ConfigurationManager.AppSettings["CambioDataSistemaVisible"] != null &&
                     ConfigurationManager.AppSettings["CambioDataSistemaVisible"] == "SI")
                {
                    this.liDataSistema.Visible = true;
                    GetDataSistema((UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]), this);
                    lblDataSistema.Text = string.Format("Data Sistema: {0:dd/MM/yyyy}", DataSistema.Value);
                }


                if (this.domanda.TipoAutomazione != null)
                {
                    lblPresaInCarico.Text = "Presa in carico Domanda automatizzata";
                }

            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ChangeAttivato();
            ManageTitle();
        }

        internal void GetSemaforo()
        {
            if (Session["Semaforo"] == null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                PresenterMenuLeft presenterMenuLeft = new PresenterMenuLeft();
                this.areaInfoPratica = new AreaInfoPratica();
                CodeUtility.AggiornaSemafori(this, this, null);
            }

            this.areaQuadri = (AreaQuadri)Session["Semaforo"];

            GetInfoCalcoloAbilitato();
            GetInfoStampaAbilitata();
            GetAggiornaWebDomAbilitata();
            GetInfoAggCI05Abilitato();
            GetInfoAggWebDomAbilitato();
            GetInfoAggFelpeAbilitato();
            GetInfoAggOneriAbilitato();
            GetInfoAggSaiAbilitato();
            GetInfoAggINPDAPAbilitato();
            GetInfoAggTotalAbilitato();
            GetInfoAggBookingAbilitato();
            GetInfoAggTotAbilitato();
            GetInfoAggNoteDebitoAbilitato();
            GetInfoAggPianiDiPagamentoAbilitato();
            GetInfoAggEquoIndAbilitato();
            GetInfoAggIndennitaSpecialeAbilitato();
            GetInfoPresaInCarico();
            ValorizzaMenuSemafori();
        }

        private void GetInfoCalcoloAbilitato()
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.Stato == "CALCOLATA" || this.domanda.Stato == "CALCOLO NO WEBDOM" ||
                this.domanda.Stato == "CALCOLO NO FELPE" || this.domanda.Stato == "CALCOLO NO ONERI" ||
                this.domanda.Stato == "CALCOLO NO INDEB" || this.domanda.Stato == "CALCOLO NO INDEB WAIT" ||
                this.domanda.Stato == "CALCOLO NO SAI" || this.domanda.Stato == "CALCOLO NO SIN" || this.domanda.Stato == "CALCOLO NO TOTAL" || this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoStazLavoro)
                || this.domanda.Stato == "CALCOLO NO TOT" || this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoNoteDebito) || this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNo6Scatti) ||
                this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoEquoInd) || this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoIndennSpec) || this.domanda.Stato == "CALCOLO NO BOOKING")
            {
                DisabilitaPulsanteCalcolo();
            }
            else
            {
                if (this.domanda.IsCalcoloAbilitato)
                    AbilitaPulsanteCalcolo();
                else
                    DisabilitaPulsanteCalcolo();
            }
        }

        private void GetInfoStampaAbilitata()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null)
                {

                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO INDEB":
                            DisabilitaPulsanteStampa();
                            break;
                        case "CALCOLO NO INDEB WAIT":
                            DisabilitaPulsanteStampa();
                            break;
                        case "CALCOLATA":
                        case "CALCOLO NO WEBDOM":
                        case "CALCOLO NO FELPE":
                        case "CALCOLO NO ONERI":
                        case "CALCOLO NO SAI":
                        case "CALCOLO NO SIN":
                        case "CALCOLO NO STAZ. LAVORO":
                        case "CALCOLO NO TOTAL":
                        case "CALCOLO NO TOT":
                        case "CALCOLO VERIFY":
                        case "CALCOLO NO NOTE DEBITO":
                        case "CALCOLO NO SEI SCATTI":
                        case "CALCOLO NO EQUOIND":
                        case "CALCOLO NO INDENN SPEC":

                            AbilitaPulsanteStampa();
                            break;

                        case "SCARTO DA CALCOLO":
                        case "SCARTO VERIFY":
                            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                                AbilitaPulsanteStampa();
                            else
                                DisabilitaPulsanteStampa();
                            break;
                        default:
                            DisabilitaPulsanteStampa();
                            break;
                    }
                }
                else
                {
                    DisabilitaPulsanteStampa();
                }
            }
            else
            {
                DisabilitaPulsanteStampa();
            }
        }

        private void GetAggiornaWebDomAbilitata()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null)
                {

                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO INDEB":
                            AbilitaAggiornaCalcoloNoInd();
                            break;
                        case "CALCOLO NO INDEB WAIT":
                            AbilitaAggiornaCalcoloNoInd();
                            break;
                        default:
                            DisabilitaAggiornaCalcoloNoInd();
                            break;
                    }
                }
                else
                {
                    DisabilitaAggiornaCalcoloNoInd();
                }
            }
            else
            {
                DisabilitaAggiornaCalcoloNoInd();
            }
        }

        private void GetInfoAggCI05Abilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null)
                {

                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO STAZ. LAVORO":
                            AbilitaPulsanteAggCI05();
                            break;
                        default:
                            DisabilitaPulsanteAggCI05();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggCI05();
            }
            else
                DisabilitaPulsanteAggCI05();
        }

        private void GetInfoAggWebDomAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null)
                {

                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO WEBDOM":
                            AbilitaPulsanteAggWebDom();
                            break;
                        default:
                            DisabilitaPulsanteAggWebDom();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggWebDom();
            }
            else
                DisabilitaPulsanteAggWebDom();
        }

        private void GetInfoAggFelpeAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null)
                {

                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO FELPE":
                            AbilitaPulsanteAggFelpe();
                            break;
                        default:
                            DisabilitaPulsanteAggFelpe();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggFelpe();
            }
            else
                DisabilitaPulsanteAggFelpe();
        }

        private void GetInfoAggOneriAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO ONERI":
                            AbilitaPulsanteAggOneri();
                            break;
                        default:
                            DisabilitaPulsanteAggOneri();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggOneri();
            }
            else
                DisabilitaPulsanteAggOneri();
        }

        private void GetInfoAggSaiAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO SAI":
                            AbilitaPulsanteAggSai();
                            break;
                        default:
                            DisabilitaPulsanteAggSai();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggSai();
            }
            else
                DisabilitaPulsanteAggSai();
        }

        private void GetInfoAggINPDAPAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO SIN":
                            AbilitaPulsanteAggINPDAP();
                            break;
                        default:
                            DisabilitaPulsanteAggINPDAP();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggINPDAP();
            }
            else
                DisabilitaPulsanteAggINPDAP();
        }

        private void GetInfoAggTotalAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO TOTAL":
                            AbilitaPulsanteAggTotal();
                            break;
                        default:
                            DisabilitaPulsanteAggTotal();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggTotal();
            }
            else
                DisabilitaPulsanteAggTotal();
        }

        private void GetInfoAggNoteDebitoAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS || this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO))
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO NOTE DEBITO":
                            AbilitaPulsanteAggNoteDebito();
                            break;
                        default:
                            DisabilitaPulsanteAggNoteDebito();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggNoteDebito();
            }
            else
                DisabilitaPulsanteAggNoteDebito();
        }

        private void GetInfoAggPianiDiPagamentoAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO SEI SCATTI":
                            AbilitaPulsanteAggPianiDiPagamento();
                            break;
                        default:
                            DisabilitaPulsanteAggPianiDiPagamento();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggPianiDiPagamento();
            }
            else
                DisabilitaPulsanteAggPianiDiPagamento();
        }

        private void GetInfoAggEquoIndAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO EQUOIND":
                            AbilitaPulsanteAggEquoInd();
                            break;
                        default:
                            DisabilitaPulsanteAggEquoInd();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggEquoInd();
            }
            else
                DisabilitaPulsanteAggEquoInd();
        }

        private void GetInfoAggIndennitaSpecialeAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO INDENN SPEC":
                            AbilitaPulsanteAggIndennitaSpeciale();
                            break;
                        default:
                            DisabilitaPulsanteAggIndennitaSpeciale();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggIndennitaSpeciale();
            }
            else
                DisabilitaPulsanteAggIndennitaSpeciale();
        }
        private void GetInfoAggBookingAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null) //ENG - Booking: correzione per FS e CI
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO BOOKING":
                            AbilitaPulsanteAggBooking();
                            break;
                        default:
                            DisabilitaPulsanteAggBooking();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggBooking();
            }
            else
                DisabilitaPulsanteAggBooking();
        }

        private void GetInfoAggTotAbilitato()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                {
                    switch (this.domanda.Stato)
                    {
                        case "CALCOLO NO TOT":
                            AbilitaPulsanteAggTot();
                            break;
                        default:
                            DisabilitaPulsanteAggTot();
                            break;
                    }
                }
                else
                    DisabilitaPulsanteAggTot();
            }
            else
                DisabilitaPulsanteAggTot();
        }

        private void GetInfoPresaInCarico()
        {
            if (Session["Domanda"] != null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                if (this.domanda != null)
                {
                    if (CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.UTENTE || this.domanda.Stato == "CALCOLATA" || this.domanda.IsMatchMatricola)
                        DisabilitaPulsantePresaInCarico();
                    else
                        AbilitaPulsantePresaInCarico();
                }
                else
                    DisabilitaPulsantePresaInCarico();
            }
            else
                DisabilitaPulsantePresaInCarico();
        }

        private void ValorizzaMenuSemafori()
        {
            #region common
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            if (areaQuadri != null && areaQuadri.QuadroTitolare != null)
                ValorizzaSemafori(imgTitolare, this.areaQuadri.QuadroTitolare.Quadro, liTitolare);

            if (areaQuadri != null && areaQuadri.QuadroDetrazioni != null)
                ValorizzaSemafori(imgDetrazioni, this.areaQuadri.QuadroDetrazioni.Quadro, liDetrazioni);

            if (areaQuadri != null && areaQuadri.QuadroPagamento != null)
                ValorizzaSemafori(imgPagamento, this.areaQuadri.QuadroPagamento.Quadro, liModalitaPagamento);

            if (areaQuadri != null && areaQuadri.QuadroDelegatoTutore != null)
                ValorizzaSemafori(imgDelegatoTutore, this.areaQuadri.QuadroDelegatoTutore.Quadro, liDelegatoTutore);

            if (areaQuadri != null && areaQuadri.QuadroRedditi != null)
                ValorizzaSemafori(imgRedditi, this.areaQuadri.QuadroRedditi.Quadro, liRedditi);

            if (areaQuadri != null && areaQuadri.QuadroDanteCausa != null)
                ValorizzaSemafori(imgDanteCausa, this.areaQuadri.QuadroDanteCausa.Quadro, liDanteCausa);

            if (areaQuadri != null && areaQuadri.QuadroFamiliari != null)
                ValorizzaSemafori(imgFamiliare, this.areaQuadri.QuadroFamiliari.Quadro, liFamiliare);

            if (areaQuadri != null && areaQuadri.QuadroSupplementi != null)
            {
                ValorizzaSemafori(imgSupplementi, this.areaQuadri.QuadroSupplementi.Quadro, liSupplementi);
                if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione))
                {
                    lblSupplementi.Text = "Adeguamento Pro Quota Casse";
                }
            }

            if (areaQuadri != null && areaQuadri.QuadroOneri != null)
                ValorizzaSemafori(imgOneri, this.areaQuadri.QuadroOneri.Quadro, liOneri);

            if (areaQuadri != null && areaQuadri.QuadroDatiNoCalcolo != null)
                ValorizzaSemafori(imgDatiNoCalcolo, this.areaQuadri.QuadroDatiNoCalcolo.Quadro, liDatiNoCalcolo);

            if (areaQuadri != null && areaQuadri.QuadroAventiDiritto != null)
                ValorizzaSemafori(imgAventiDiritto, this.areaQuadri.QuadroAventiDiritto.Quadro, liAventiDiritto);

            if (areaQuadri != null && areaQuadri.QuadroPeriodi != null)
                ValorizzaSemafori(imgPeriodi, this.areaQuadri.QuadroPeriodi.Quadro, liPeriodi);

            if (areaQuadri != null && areaQuadri.QuadroAltreDomandeCollegate != null)
                ValorizzaSemafori(imgAltreDomandeCollegate, this.areaQuadri.QuadroAltreDomandeCollegate.Quadro, liAltreDomandeCollegate);

            if (areaQuadri != null && areaQuadri.QuadroRichiestaBonus != null)
            {
                ValorizzaSemafori(imgRichiestaBonus, this.areaQuadri.QuadroRichiestaBonus.Quadro, liRichiestaBonus);
                if (datiPensione != null && datiPensione.IsRichiestaBonus.HasValue && datiPensione.IsRichiestaBonus.Value && this.domanda != null && this.domanda.Stato == Utility.GetDescription(CodeUtility.StatoPensione.Calcolata))
                {
                    lblRichiestaBonus.Text = "Richiesta Bonus/Esito Prenotazione";
                }
            }


            #endregion common

            #region custom

            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (areaQuadri != null && areaQuadri.QuadroLiquidazionePensione != null)
            {
                if (this.domanda != null && this.domanda.TipoAppartenenza != null)
                {
                    switch (this.domanda.TipoAppartenenza)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                            ValorizzaSemafori(imgLiquidazionePensioneAgo, this.areaQuadri.QuadroLiquidazionePensione.Quadro, liLiquidazionePensioneAgo);
                            ValorizzaSemafori(imgLiquidazionePensione, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensione);
                            ValorizzaSemafori(imgLiquidazionePensioneCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneCi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS:
                            ValorizzaSemafori(imgLiquidazionePensione, this.areaQuadri.QuadroLiquidazionePensione.Quadro, liLiquidazionePensione);
                            ValorizzaSemafori(imgLiquidazionePensioneAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneAgo);
                            ValorizzaSemafori(imgLiquidazionePensioneCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneCi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                            ValorizzaSemafori(imgLiquidazionePensioneCi, this.areaQuadri.QuadroLiquidazionePensione.Quadro, liLiquidazionePensioneCi);
                            ValorizzaSemafori(imgLiquidazionePensioneAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneAgo);
                            ValorizzaSemafori(imgLiquidazionePensione, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensione);
                            break;
                        default:
                            ValorizzaSemafori(imgLiquidazionePensione, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensione);
                            ValorizzaSemafori(imgLiquidazionePensioneAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneAgo);
                            ValorizzaSemafori(imgLiquidazionePensioneCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneCi);
                            break;
                    }
                }
                else
                {
                    ValorizzaSemafori(imgLiquidazionePensione, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensione);
                    ValorizzaSemafori(imgLiquidazionePensioneAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneAgo);
                    ValorizzaSemafori(imgLiquidazionePensioneCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liLiquidazionePensioneCi);
                }
            }

            if (areaQuadri != null && areaQuadri.QuadroDatiContributivi != null)
            {
                if (this.domanda != null && this.domanda.TipoAppartenenza != null)
                {
                    switch (this.domanda.TipoAppartenenza)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                            ValorizzaSemafori(imgDatiContributiviAgo, this.areaQuadri.QuadroDatiContributivi.Quadro, liDatiContributiviAgo);
                            ValorizzaSemafori(imgDatiContributivi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributivi);
                            ValorizzaSemafori(imgDatiContributiviCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviCi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS:
                            ValorizzaSemafori(imgDatiContributivi, this.areaQuadri.QuadroDatiContributivi.Quadro, liDatiContributivi);
                            ValorizzaSemafori(imgDatiContributiviAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviAgo);
                            ValorizzaSemafori(imgDatiContributiviCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviCi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                            ValorizzaSemafori(imgDatiContributiviCi, this.areaQuadri.QuadroDatiContributivi.Quadro, liDatiContributiviCi);
                            ValorizzaSemafori(imgDatiContributivi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributivi);
                            ValorizzaSemafori(imgDatiContributiviAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviAgo);
                            break;
                        default:
                            ValorizzaSemafori(imgDatiContributivi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributivi);
                            ValorizzaSemafori(imgDatiContributiviAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviAgo);
                            ValorizzaSemafori(imgDatiContributiviCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviCi);
                            break;
                    }
                }
                else
                {
                    ValorizzaSemafori(imgDatiContributivi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributivi);
                    ValorizzaSemafori(imgDatiContributiviAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviAgo);
                    ValorizzaSemafori(imgDatiContributiviCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiContributiviCi);
                }
            }

            if (areaQuadri != null && areaQuadri.QuadroMaggiorazioniBenefici != null)
            {
                if (this.domanda != null && this.domanda.TipoAppartenenza != null)
                {
                    switch (this.domanda.TipoAppartenenza)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS:
                            ValorizzaSemafori(imgMaggiorazioniEBenefici, this.areaQuadri.QuadroMaggiorazioniBenefici.Quadro, liMaggiorazioniBenefici);
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiAgo);
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiCi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiAgo, this.areaQuadri.QuadroMaggiorazioniBenefici.Quadro, liMaggiorazioniBeneficiAgo);
                            ValorizzaSemafori(imgMaggiorazioniEBenefici, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBenefici);
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiCi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiCi, this.areaQuadri.QuadroMaggiorazioniBenefici.Quadro, liMaggiorazioniBeneficiCi);
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiAgo);
                            ValorizzaSemafori(imgMaggiorazioniEBenefici, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBenefici);
                            break;
                        default:
                            ValorizzaSemafori(imgMaggiorazioniEBenefici, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBenefici);
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiAgo);
                            ValorizzaSemafori(imgMaggiorazioniEBeneficiCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiCi);
                            break;
                    }
                }
                else
                {
                    ValorizzaSemafori(imgMaggiorazioniEBenefici, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBenefici);
                    ValorizzaSemafori(imgMaggiorazioniEBeneficiAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiAgo);
                    ValorizzaSemafori(imgMaggiorazioniEBeneficiCi, AreaQuadri.Semaforo.Rosso_NonAbilitato, liMaggiorazioniBeneficiCi);
                }
            }

            if (areaQuadri != null && areaQuadri.QuadroBititolarita != null)
            {
                if (this.domanda != null && this.domanda.TipoAppartenenza != null)
                {
                    switch (this.domanda.TipoAppartenenza)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                            ValorizzaSemafori(imgBititolarita, this.areaQuadri.QuadroBititolarita.Quadro, liBititolarita);
                            break;
                        default:
                            ValorizzaSemafori(imgBititolarita, AreaQuadri.Semaforo.Rosso_NonAbilitato, liBititolarita);
                            break;
                    }
                }
                else
                {
                    ValorizzaSemafori(imgBititolarita, AreaQuadri.Semaforo.Rosso_NonAbilitato, liBititolarita);
                }
            }
            //liEliminazione
            if (areaQuadri != null && areaQuadri.QuadroEliminazione != null)
            {
                if (this.domanda != null && this.domanda.TipoAppartenenza != null)
                {
                    switch (this.domanda.TipoAppartenenza)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                            ValorizzaSemafori(imgEliminazione, this.areaQuadri.QuadroEliminazione.Quadro, liEliminazione);
                            break;
                        default:
                            ValorizzaSemafori(imgEliminazione, AreaQuadri.Semaforo.Rosso_NonAbilitato, liEliminazione);
                            break;
                    }
                }
                else
                {
                    ValorizzaSemafori(imgEliminazione, AreaQuadri.Semaforo.Rosso_NonAbilitato, liEliminazione);
                }
            }

            if (areaQuadri != null && areaQuadri.QuadroDatiFondo != null)
            {
                if (this.domanda != null && this.domanda.TipoAppartenenza != null)
                {
                    switch (this.domanda.TipoAppartenenza)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                            ValorizzaSemafori(imgDatiFondoAgo, this.areaQuadri.QuadroDatiFondo.Quadro, liDatiFondoAgo);
                            ValorizzaSemafori(imgDatiFondo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiFondo);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS:
                            ValorizzaSemafori(imgDatiFondo, this.areaQuadri.QuadroDatiFondo.Quadro, liDatiFondo);
                            ValorizzaSemafori(imgDatiFondoAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiFondoAgo);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                            ValorizzaSemafori(imgDatiFondo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiFondo);
                            ValorizzaSemafori(imgDatiFondoAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiFondoAgo);
                            break;
                    }
                }
                else
                {
                    ValorizzaSemafori(imgDatiFondo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiFondo);
                    ValorizzaSemafori(imgDatiFondoAgo, AreaQuadri.Semaforo.Rosso_NonAbilitato, liDatiFondoAgo);
                }
            }

            #endregion custom

            if (!((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).IsMatchMatricola &&
                   ((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && datiPensione.TipoAutomazione != null)
            {
                //disabilitaVociMenu();
                //liTitolare.Visible = false;
                abilitaVociMenuNonAncoraGestite();
                liInviaCalcolo.Visible = false;
                liStampa.Visible = false;
                liPresaInCarico.Visible = true;
            }
            else
                //Eliminare da tale metodo i quadri di volta in volta gestiti in modo automatico
                abilitaVociMenuNonAncoraGestite();
        }

        private void ValorizzaSemafori(Image myImage, AreaQuadri.Semaforo tab, HtmlGenericControl voceMenu)
        {
            voceMenu.Visible = true;
            string currentTheme = Page.Theme;

            if (tab == AreaQuadri.Semaforo.Rosso_NonAbilitato)
            {
                voceMenu.Visible = false;
            }
            else if (tab == AreaQuadri.Semaforo.Rosso_Abilitato)
            {
                myImage.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                if (string.IsNullOrEmpty(voceMenu.Attributes["class"]))
                    voceMenu.Attributes["class"] = string.Empty;
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("optional", "");
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("mandatory", "");
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("saved", "");
                voceMenu.Attributes["class"] = string.Format("{0} {1}", voceMenu.Attributes["class"].Trim(), "mandatory");
            }

            else if (tab == AreaQuadri.Semaforo.Giallo)
            {
                myImage.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
                if (string.IsNullOrEmpty(voceMenu.Attributes["class"]))
                    voceMenu.Attributes["class"] = string.Empty;
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("optional", "");
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("mandatory", "");
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("saved", "");
                voceMenu.Attributes["class"] = string.Format("{0} {1}", voceMenu.Attributes["class"].Trim(), "optional");
            }
            else if (tab == AreaQuadri.Semaforo.Verde)
            {
                myImage.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                if (string.IsNullOrEmpty(voceMenu.Attributes["class"]))
                    voceMenu.Attributes["class"] = string.Empty;
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("optional", "");
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("mandatory", "");
                voceMenu.Attributes["class"] = voceMenu.Attributes["class"].Replace("saved", "");
                voceMenu.Attributes["class"] = string.Format("{0} {1}", voceMenu.Attributes["class"].Trim(), "saved");
            }
        }

        internal void ChangeAttivato()
        {
            switch (CodeUtility.GetCurrentPageName())
            {
                case "PosizioneSelezionata.aspx":
                    //liTitolare.Attributes.Add("class", "attivato");
                    if (this.areaQuadri == null || this.areaQuadri.QuadroTitolare == null ||
                        this.areaQuadri.QuadroTitolare.Quadro != AreaQuadri.Semaforo.Verde)
                        disabilitaVociMenu();
                    break;

                case "Titolare.aspx":
                    CodeUtility.AddClass(liTitolare, "attivato");
                    if (this.areaQuadri == null || this.areaQuadri.QuadroTitolare == null ||
                        this.areaQuadri.QuadroTitolare.Quadro != AreaQuadri.Semaforo.Verde)
                        disabilitaVociMenu();
                    break;
                case "DelegatoTutore.aspx":
                    CodeUtility.AddClass(liDelegatoTutore, "attivato");
                    break;
                case "Familiare.aspx":
                    CodeUtility.AddClass(liFamiliare, "attivato");
                    break;
                case "LiquidazionePensione.aspx":
                    CodeUtility.AddClass(liLiquidazionePensione, "attivato");
                    break;
                case "LiquidazionePensioneAgo.aspx":
                    CodeUtility.AddClass(liLiquidazionePensioneAgo, "attivato");
                    break;
                case "LiquidazionePensioneCi.aspx":
                    CodeUtility.AddClass(liLiquidazionePensioneCi, "attivato");
                    break;
                case "DatiContributivi.aspx":
                    CodeUtility.AddClass(liDatiContributivi, "attivato");
                    break;
                case "DatiContributiviAgo.aspx":
                    CodeUtility.AddClass(liDatiContributiviAgo, "attivato");
                    break;
                case "DatiContributiviCi.aspx":
                    CodeUtility.AddClass(liDatiContributiviCi, "attivato");
                    break;
                case "MaggiorazioniEBenefici.aspx":
                    CodeUtility.AddClass(liMaggiorazioniBenefici, "attivato");
                    break;
                case "MaggiorazioniEBeneficiAgo.aspx":
                    CodeUtility.AddClass(liMaggiorazioniBeneficiAgo, "attivato");
                    break;
                case "MaggiorazioniEBeneficiCi.aspx":
                    CodeUtility.AddClass(liMaggiorazioniBeneficiCi, "attivato");
                    break;
                case "Detrazioni.aspx":
                    liTitolare.Visible = true;
                    CodeUtility.AddClass(liDetrazioni, "attivato");
                    break;
                case "Redditi.aspx":
                    CodeUtility.AddClass(liRedditi, "attivato");
                    break;
                case "ModalitaPagamento.aspx":
                case "ResultPagamentoStatic.aspx":
                    CodeUtility.AddClass(liModalitaPagamento, "attivato");
                    break;
                case "SindacatoPatronato.aspx":
                    CodeUtility.AddClass(liSindacatoPatronato, "attivato");
                    break;
                case "Default.aspx":
                    CodeUtility.AddClass(liHome, "attivato");
                    break;
                case "DanteCausa.aspx":
                    CodeUtility.AddClass(liDanteCausa, "attivato");
                    break;
                case "Supplementi.aspx":
                    CodeUtility.AddClass(liSupplementi, "attivato");
                    break;
                case "Bititolarita.aspx":
                    CodeUtility.AddClass(liBititolarita, "attivato");
                    break;
                case "InvioCalcolo.aspx":
                    CodeUtility.AddClass(liInviaCalcolo, "attivato");
                    break;
                case "AggiornaWebDom.aspx":
                    CodeUtility.AddClass(liAggWebDom, "attivato");
                    break;
                case "PresaInCarico.aspx":
                    CodeUtility.AddClass(liPresaInCarico, "attivato");
                    if (this.areaQuadri == null || this.areaQuadri.QuadroTitolare == null ||
                        this.areaQuadri.QuadroTitolare.Quadro != AreaQuadri.Semaforo.Verde)
                        disabilitaVociMenu();
                    break;
                case "Eliminazione.aspx":
                    CodeUtility.AddClass(liEliminazione, "attivato");
                    break;
                case "Oneri.aspx":
                    CodeUtility.AddClass(liOneri, "attivato");
                    break;
                case "DatiFondo.aspx":
                    CodeUtility.AddClass(liDatiFondo, "attivato");
                    break;
                case "DatiFondoAgo.aspx":
                    CodeUtility.AddClass(liDatiFondoAgo, "attivato");
                    break;
                case "AggiornaCI05.aspx":
                    CodeUtility.AddClass(liAggCI05, "attivato");
                    break;
                case "DatiNoCalcolo.aspx":
                    CodeUtility.AddClass(liDatiNoCalcolo, "attivato");
                    break;
                case "AventiDiritto.aspx":
                    CodeUtility.AddClass(liAventiDiritto, "attivato");
                    break;
                case "Periodi.aspx":
                    CodeUtility.AddClass(liPeriodi, "attivato");
                    break;
                case "AltreDomandeCollegate.aspx":
                    CodeUtility.AddClass(liAltreDomandeCollegate, "attivato");
                    break;
                case "RichiestaBonus.aspx":
                    CodeUtility.AddClass(liRichiestaBonus, "attivato");
                    break;
                case "AggiornaCalcoloNoInd.aspx":
                    CodeUtility.AddClass(liRichiestaBonus, "attivato");
                    break;
                default:
                    break;
            }
        }

        internal void ManageTitle()
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                lblDatiFondo.Text = "Dati " + CodeUtility.GetLabelFondoCassa(this.domanda);
            }
            catch (Exception) { }
        }
        public void DisabilitaDomanda()
        {
            liTitolare.Visible = false;
            disabilitaVociMenu();
        }

        private void DisabilitaPulsanteCalcolo()
        {
            liInviaCalcolo.Visible = false;
        }

        private void AbilitaPulsanteCalcolo()
        {
            liInviaCalcolo.Visible = true;
        }

        private void DisabilitaPulsanteStampa()
        {
            liStampa.Visible = false;
        }

        private void AbilitaPulsanteStampa()
        {
            liStampa.Visible = true;
        }

        private void DisabilitaAggiornaCalcoloNoInd()
        {
            liAggiornaCalcoloNoInd.Visible = false;
        }

        private void AbilitaAggiornaCalcoloNoInd()
        {
            liAggiornaCalcoloNoInd.Visible = true;
        }

        private void AbilitaPulsantePresaInCarico()
        {
            liPresaInCarico.Visible = true;
        }

        private void DisabilitaPulsantePresaInCarico()
        {
            liPresaInCarico.Visible = false;
        }

        private void DisabilitaPulsanteAggCI05()
        {
            liAggCI05.Visible = false;
        }

        private void AbilitaPulsanteAggCI05()
        {
            liAggCI05.Visible = true;
        }

        private void DisabilitaPulsanteAggWebDom()
        {
            liAggWebDom.Visible = false;
        }

        private void AbilitaPulsanteAggWebDom()
        {
            liAggWebDom.Visible = true;
        }

        private void DisabilitaPulsanteAggFelpe()
        {
            liAggFelpe.Visible = false;
        }

        private void AbilitaPulsanteAggFelpe()
        {
            liAggFelpe.Visible = true;
        }

        private void DisabilitaPulsanteAggOneri()
        {
            liAggOneri.Visible = false;
        }

        private void AbilitaPulsanteAggOneri()
        {
            liAggOneri.Visible = true;
        }

        private void DisabilitaPulsanteAggSai()
        {
            liAggSai.Visible = false;
        }

        private void AbilitaPulsanteAggSai()
        {
            liAggSai.Visible = true;
        }

        private void DisabilitaPulsanteAggINPDAP()
        {
            liAggINPDAP.Visible = false;
        }

        private void AbilitaPulsanteAggINPDAP()
        {
            liAggINPDAP.Visible = true;
        }

        private void DisabilitaPulsanteAggTotal()
        {
            liAggTotal.Visible = false;
        }

        private void AbilitaPulsanteAggTotal()
        {
            liAggTotal.Visible = true;
        }

        private void DisabilitaPulsanteAggNoteDebito()
        {
            liAggNoteDebito.Visible = false;
        }

        private void AbilitaPulsanteAggNoteDebito()
        {
            liAggNoteDebito.Visible = true;
        }

        private void DisabilitaPulsanteAggPianiDiPagamento()
        {
            liAggiornaPianiDiPagamento.Visible = false;
        }

        private void DisabilitaPulsanteAggEquoInd()
        {
            liAggiornaEquoInd.Visible = false;
        }

        private void DisabilitaPulsanteAggIndennitaSpeciale()
        {
            liAggiornaIndennitaSpeciale.Visible = false;
        }

        private void AbilitaPulsanteAggPianiDiPagamento()
        {
            liAggiornaPianiDiPagamento.Visible = true;
        }

        private void AbilitaPulsanteAggEquoInd()
        {
            liAggiornaEquoInd.Visible = true;
        }

        private void AbilitaPulsanteAggIndennitaSpeciale()
        {
            liAggiornaIndennitaSpeciale.Visible = true;
        }

        private void DisabilitaPulsanteAggBooking()
        {
            liAggBooking.Visible = false;
        }

        private void AbilitaPulsanteAggBooking()
        {
            liAggBooking.Visible = true;
        }

        private void DisabilitaPulsanteAggTot()
        {
            liAggTot.Visible = false;
        }

        private void AbilitaPulsanteAggTot()
        {
            liAggTot.Visible = true;
        }

        private void disabilitaVociMenu()
        {
            liDelegatoTutore.Visible = false;
            liFamiliare.Visible = false;
            liLiquidazionePensione.Visible = false;
            liLiquidazionePensioneAgo.Visible = false;
            liLiquidazionePensioneCi.Visible = false;
            liDatiContributivi.Visible = false;
            liDatiContributiviAgo.Visible = false;
            liDatiContributiviCi.Visible = false;
            liDetrazioni.Visible = false;
            liRedditi.Visible = false;
            liModalitaPagamento.Visible = false;
            liSindacatoPatronato.Visible = false;
            liMaggiorazioniBenefici.Visible = false;
            liMaggiorazioniBeneficiCi.Visible = false;
            liMaggiorazioniBeneficiAgo.Visible = false;
            liDanteCausa.Visible = false;
            liSupplementi.Visible = false;
            liBititolarita.Visible = false;
            liEliminazione.Visible = false;
            liOneri.Visible = false;
            liDatiFondo.Visible = false;
            liDatiFondoAgo.Visible = false;
            liDatiNoCalcolo.Visible = false;
            liAventiDiritto.Visible = false;
            liPeriodi.Visible = false;
            liAltreDomandeCollegate.Visible = false;
            liRichiestaBonus.Visible = false;
        }

        private void abilitaVociMenuNonAncoraGestite()
        {
            liSindacatoPatronato.Visible = false;
            //liAggiornaWebDom.Visible = true;
            //liMaggiorazioniBeneficiAgo.Visible = false;
            //liMaggiorazioniBeneficiCi.Visible = false;
        }
    }
}
