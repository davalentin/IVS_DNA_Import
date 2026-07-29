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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Detrazioni
{
    public partial class UCDetrazioni : CustomBaseUserControl, IDetrazioni, IDanteCausa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDetrazioni
        public AreaDetrazioni detrazioniPensione
        {
            get { return ViewState[EnumViewState.AreaDetrazioni.ToString()] as AreaDetrazioni; }
            set { ViewState[EnumViewState.AreaDetrazioni.ToString()] = value; }
        }

        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDetrazioni

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        public event EventHandler AcquisizioneDetrazioni;
        public event EventHandler AggiornamentoDetrazioni;
        public event EventHandler RicaricaSoggetti;

        protected void Page_Load(object sender, EventArgs e)
        {
            //            ViewState["Detrazioni"] = this.detrazioniPensione;
        }

        public void ValorizzaDetrazioni()
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            //ENG - REVERSIBILITA FS (NO INPDAP/024)            
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            if (domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, this.areaDanteCausa, domanda.Categoria)
                && !domanda.IsDomandaINPDAP && domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
            {
                string valoreControlloAnnoCompetenza = string.Empty;
                if (ViewState["AnnoCompetenza"] != null)
                    valoreControlloAnnoCompetenza = (string)ViewState["AnnoCompetenza"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetAnnoCompetenza(UtilityTipoAppartenenza.FS, out valoreControlloAnnoCompetenza);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && !String.IsNullOrEmpty(valoreControlloAnnoCompetenza) && !String.IsNullOrEmpty(valoreControlloAnnoCompetenza.Trim()))
                        ViewState["AnnoCompetenza"] = valoreControlloAnnoCompetenza.Trim();
                }

                string valoreControlloDisabilitaDetrazioniObbligatorieContitolari = string.Empty;
                if (ViewState["DisabilitaDetrazioniObbligatorieContitolariFS"] != null)
                    valoreControlloDisabilitaDetrazioniObbligatorieContitolari = (string)ViewState["DisabilitaDetrazioniObbligatorieContitolariFS"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("DisabilitaDetrazioniObbligatorieContitolariFS", out valoreControlloDisabilitaDetrazioniObbligatorieContitolari);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && !String.IsNullOrEmpty(valoreControlloDisabilitaDetrazioniObbligatorieContitolari) && !String.IsNullOrEmpty(valoreControlloDisabilitaDetrazioniObbligatorieContitolari.Trim()))
                        ViewState["DisabilitaDetrazioniObbligatorieContitolariFS"] = valoreControlloDisabilitaDetrazioniObbligatorieContitolari.Trim();
                }

            }

            if (this.detrazioniPensione != null)
            {
                if (this.detrazioniPensione.ElencoSoggetti != null && this.detrazioniPensione.ElencoSoggetti.Count() > 0)
                {
                    pnlDetrazioni.Visible = false;
                    pnlSoggetti.Visible = true;

                    gv_Soggetti.DataSource = this.detrazioniPensione.ElencoSoggetti;
                    gv_Soggetti.DataBind();
                }
                else if (this.detrazioniPensione.Detrazioni != null)
                {
                    pnlDetrazioni.Visible = true;
                    pnlSoggetti.Visible = false;

                    lblAgevolazReddLavAut.Text = this.detrazioniPensione.Detrazioni.DetrazioniReddito.ToString();
                    lblAgevolazioniPensionati.Text = this.detrazioniPensione.Detrazioni.AgevolazionePensionati.ToString();
                    lblConiugeoPrimoFiglio.Text = this.detrazioniPensione.Detrazioni.ConiugeOFiglio.ToString();
                    lblNFigliMin3NoHandicap100.Text = this.detrazioniPensione.Detrazioni.FigliMinori3AnniNoHandicap100.ToString();
                    lblNFigliMin3NoHandicap50.Text = this.detrazioniPensione.Detrazioni.FigliMinori3AnniNoHandicap50.ToString();
                    lblNFigliMin3Handicap100.Text = this.detrazioniPensione.Detrazioni.FigliMinori3AnniHandicap100.ToString();
                    lblNFigliMin3Handicap50.Text = this.detrazioniPensione.Detrazioni.FigliMinori3AnniHandicap50.ToString();
                    lblNFigliMagg3Handicap100.Text = this.detrazioniPensione.Detrazioni.FigliMaggiori3AnniHandicap100.ToString();
                    lblNFigliMagg3Handicap50.Text = this.detrazioniPensione.Detrazioni.FigliMaggiori3AnniHandicap50.ToString();
                    lblNFigliMagg3NoHandicap100.Text = this.detrazioniPensione.Detrazioni.FigliMaggiori3AnniNoHandicap100.ToString();
                    lblNFigliMagg3NoHandicap50.Text = this.detrazioniPensione.Detrazioni.FigliMaggiori3AnniNoHandicap50.ToString();
                    lblNAltriFamiliari100.Text = this.detrazioniPensione.Detrazioni.AltriFamiliari100.ToString();
                    lblNAltriFamiliari50.Text = this.detrazioniPensione.Detrazioni.AltriFamiliari50.ToString();
                    lblAddizionaleLombardiaVeneto.Text = this.detrazioniPensione.Detrazioni.AddizionaleLombardiaVeneto.ToString();

                    lblDecorrenzaImposte.Text = String.Format("{0:MM/yyyy}", this.detrazioniPensione.Detrazioni.DecorrenzaDetrazioneImposte);
                    // nuove detrazioni servizio unidetra
                    lblNonResidenteSchumacker.Text = this.detrazioniPensione.Detrazioni.NonResidenteSchumacker.ToString();
                    lblConvDoppieImposizioni.Text = this.detrazioniPensione.Detrazioni.ConvDoppieImposizioni.ToString();


                    if (domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && CodeUtility.IsDomandaSuperstitiOrRicostituzione(domanda.Categoria)
                        && !domanda.IsDomandaINPDAP) //per le superstiti inpdap il soggetto è sempre uno solo
                        btnTornaAiSoggetti.Visible = true;
                }
                else
                {
                    lblAgevolazReddLavAut.Text = string.Empty;
                    lblAgevolazioniPensionati.Text = string.Empty;
                    lblConiugeoPrimoFiglio.Text = string.Empty;
                    lblNFigliMin3NoHandicap100.Text = string.Empty;
                    lblNFigliMin3NoHandicap50.Text = string.Empty;
                    lblNFigliMin3Handicap100.Text = string.Empty;
                    lblNFigliMin3Handicap50.Text = string.Empty;
                    lblNFigliMagg3Handicap100.Text = string.Empty;
                    lblNFigliMagg3Handicap50.Text = string.Empty;
                    lblNFigliMagg3NoHandicap100.Text = string.Empty;
                    lblNFigliMagg3NoHandicap50.Text = string.Empty;
                    lblNAltriFamiliari100.Text = string.Empty;
                    lblNAltriFamiliari50.Text = string.Empty;
                    lblAddizionaleLombardiaVeneto.Text = string.Empty;

                    lblDecorrenzaImposte.Text = string.Empty;
                    // nuove detrazioni servizio unidetra
                    lblNonResidenteSchumacker.Text = string.Empty;
                    lblConvDoppieImposizioni.Text = string.Empty;
                }
            }

        }

        public void AggiornaDetrazioni(Object sender, EventArgs e)
        {
            RaiseAggiornamentoDetrazioni(this, null);
        }

        protected void RaiseAggiornamentoDetrazioni(object sender, EventArgs e)
        {
            if (AggiornamentoDetrazioni != null)
                AggiornamentoDetrazioni(sender, e);
        }

        public void AcquisisciDetrazioni(Object sender, EventArgs e)
        {
            RaiseAcquisizioneDetrazioni(this, null);
        }

        protected void RaiseAcquisizioneDetrazioni(object sender, EventArgs e)
        {
            if (AcquisizioneDetrazioni != null)
                AcquisizioneDetrazioni(sender, e);
        }

        public void TornaAiSoggetti(Object sender, EventArgs e)
        {
            this.detrazioniPensione.DatiInput = null;
            lblCodiceFiscale.Text = string.Empty;
            RaiseTornaAiSoggetti(this, null);
        }

        protected void RaiseTornaAiSoggetti(object sender, EventArgs e)
        {
            if (RicaricaSoggetti != null)
                RicaricaSoggetti(sender, e);
        }

        public void GestioneAcquisizione(bool isEnabled)
        {
            btnAcquisisci.Enabled = isEnabled;
        }

        #region gv_Soggetti

        protected void gv_Soggetti_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string currentTheme = Page.Theme;
            if (e.Row.RowType == DataControlRowType.DataRow && this.detrazioniPensione != null && this.detrazioniPensione.ElencoSoggetti != null)
            {
                Image img = (Image)e.Row.FindControl("img");

                //ENG - REVERSIBILITA FS (NO INPDAP/024)   
                AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                if (this.detrazioniPensione.ElencoSoggetti.FirstOrDefault(x => x.IdAnagrafica == ((GestioneDetrazioniSoggetto)e.Row.DataItem).IdAnagrafica).Confermato)
                {
                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                    img.ToolTip = "Salvato";
                }
                else
                {
                    //ENG - REVERSIBILITA FS (NO INPDAP/024)
                    if (ViewState["DisabilitaDetrazioniObbligatorieContitolariFS"] != null && ViewState["DisabilitaDetrazioniObbligatorieContitolariFS"].ToString().ToUpperInvariant() == "SI")
                    {
                        if (domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, this.areaDanteCausa, domanda.Categoria)
                            && !domanda.IsDomandaINPDAP && domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT
                            && ViewState["AnnoCompetenza"] != null)
                        {
                            int annoCompetenza = 0;
                            Int32.TryParse(ViewState["AnnoCompetenza"].ToString(), out annoCompetenza);

                            GestioneDetrazioniSoggetto soggettoCorrente = this.detrazioniPensione.ElencoSoggetti.FirstOrDefault(x => x.IdAnagrafica == ((GestioneDetrazioniSoggetto)e.Row.DataItem).IdAnagrafica);
                            if (soggettoCorrente != null && soggettoCorrente.IsContitolare && soggettoCorrente.DataCessazione.HasValue && soggettoCorrente.DataCessazione.Value.Year < annoCompetenza)
                            {
                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
                                img.ToolTip = "Non Obbligatorio";
                            }
                            else
                            {
                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                img.ToolTip = "Non Salvato";
                            }
                        }
                        else
                        {
                            img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                            img.ToolTip = "Non Salvato";
                        }
                    }
                    else
                    {

                        img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                        img.ToolTip = "Non Salvato";
                    }
                }
            }
        }

        protected void gv_Soggetti_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Dettaglio")
            {
                long? idAnagrafica = CodeUtility.StringToNullableLong(e.CommandArgument as string);
                if (this.detrazioniPensione.ElencoSoggetti != null && this.detrazioniPensione.ElencoSoggetti.FirstOrDefault(x => x.IdAnagrafica == idAnagrafica) != null)
                {
                    this.detrazioniPensione.DatiInput = new AreaDetrazioni.AreaInput();
                    this.detrazioniPensione.DatiInput.CodiceFiscale = this.detrazioniPensione.ElencoSoggetti.FirstOrDefault(x => x.IdAnagrafica == idAnagrafica).CodiceFiscale;
                    lblCodiceFiscale.Text = string.Format("({0})", this.detrazioniPensione.DatiInput.CodiceFiscale);
                }
                RaiseAggiornamentoDetrazioni(this, null);
            }
        }

        #endregion gv_Soggetti

        #region Enums

        private enum EnumViewState
        {
            AreaDetrazioni
        }

        #endregion Enums
    }
}