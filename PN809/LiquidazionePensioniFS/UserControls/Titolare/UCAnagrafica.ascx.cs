using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Titolare
{
    public partial class UCAnagrafica : CustomBaseUserControl, ITitolarePensione, IDanteCausa
    {
        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                    Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                    Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) ||
                    Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) ||
                    Utility.IsDomandaESOPMI(this.domanda.Categoria))
                    hdnIsEsodati.Value = "SI";

                if (this.domanda.IsDomandaINPDAP)
                    hdnIsINPDAP.Value = "SI";

                if (!Page.IsPostBack)
                {
                    ViewState["TitolarePensione"] = TitolarePensione;
                    LoadDdl(TitolarePensione);
                    GestionePnlDecorrenzaPensioneFromTipoFondo();
                    ValorizzaEtichetteUCAnagrafica();
                }
                else
                    TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                bool IsEnteIstruttoreFondoExINPDAP = false;
                if (TitolarePensione != null)
                {
                    CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
                    IsEnteIstruttoreFondoExINPDAP = TitolarePensione.IsEnteIstruttoreFondoExINPDAP;
                    if ((Utility.IsDomandaIOCUM(this.domanda.Categoria) && tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione &&
                        (IsEnteIstruttoreFondoExINPDAP || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art2_C12_Legge335)) || Utility.IsPensioneInabilitaProficuoLavoroCumulo(this.domanda.Categoria, this.TitolarePensione.Pensione))
                    {
                        hdnIsLikeFSPT.Value = "SI";
                    }
                }
                SetTabCertificato();
            }
        }

        private AreaTitolare.DatiPensione.TipoAppDomanda? GetTipoAppartenenzaDomanda()
        {
            if (Session["DatiPensione"] != null)
                return ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;
            else
                return (AreaTitolare.DatiPensione.TipoAppDomanda?)null;
        }

        private void LoadDdl(AreaTitolare areaTitolare)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            try
            {
                if (this.TitolarePensione == null)
                    this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                string sedi = string.Empty;
                CodeUtility areaDecodifica = new CodeUtility();
                AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

                //29-01-2014 Non sarà più presente il record BLACK. Ma verrà sostituito dal record seguente. 
                CodeUtility.SetValueDdl(ddlSindacato, "0 - NESSUN SINDACATO", "NESSUN SINDACATO", "0");

                if (areaTitolare.ElencoSindacati != null && areaTitolare.ElencoSindacati.Count() > 0)
                {
                    List<Sindacato> listaSindacati = areaTitolare.ElencoSindacati.OrderBy(x => x.Id).ToList();
                    foreach (Sindacato sindacato in listaSindacati)
                        CodeUtility.SetValueDdl(ddlSindacato, sindacato.Id.PadRight(2, ' ') + " - " + sindacato.Sigla, sindacato.Descrizione, sindacato.Id);
                }

                if ((GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS) ||
                    this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue)
                {
                    if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI && Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione)
                        && this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue && this.TitolarePensione.Pensione.CodiceSedeDestinazione.Value == 0)
                    {
                        pnlSedeDestinazione.Visible = false;
                    }
                    else
                    {
                        HiddenFieldSedi.Value = CodeUtility.LoadSedi();
                        if (HiddenFieldSedi.Value != string.Empty)
                        {
                            pnlSedeDestinazione.Visible = true;
                            //ddlSedeDestinazione.Visible = true;
                        }
                        else
                        {
                            pnlSedeDestinazione.Visible = false;
                            //ddlSedeDestinazione.Visible = false;
                        }
                    }

                    bool isBloccoSedeINPDAP = ConfigurationManager.AppSettings["CheckSedeDestinazioneINPDAP"] != null && ConfigurationManager.AppSettings["CheckSedeDestinazioneINPDAP"] == "SI";

                    if ((GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue) ||
                        (this.TitolarePensione.Pensione.IsDomandaAPEPrecociOrRicostituzione && this.domanda.TipoAppartenenza != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS))
                        txtSedeDestinazione.Enabled = false;
                    else if (isBloccoSedeINPDAP && this.domanda.IsDomandaINPDAP)
                        txtSedeDestinazione.Enabled = false;
                }
                else
                {
                    pnlSedeDestinazione.Visible = false;
                    //ddlSedeDestinazione.Visible = false;
                }

                Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoEstero[] listStatiEsteri = datiDecodifica.ElencoStatiEsteri;
                foreach (AreaDecodifica.DatiStatoEstero statoEstero in listStatiEsteri)
                {
                    CodeUtility.SetValueDdl(ddlCittadinanza, statoEstero.Descrizione, statoEstero.CodCatastale, statoEstero.CodCatastale);
                }

                LoadDdlFigli();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAnagrafica, Errore nel metodo LoadDdl " + ex);
            }

        }

        private bool checkMemo228_2025(AreaTitolare.DatiPensione Pensione)
        {
            bool retVal = false;
            if ((Pensione.CodeGruppo == "0001" && Pensione.CodeProdotto == "0001" && Pensione.CodeTipo == "0017") ||
                (Pensione.CodeGruppo == "0001" && Pensione.CodeProdotto == "0002" && Pensione.CodeTipo == "0017") ||
                (Pensione.CodeGruppo == "0001" && Pensione.CodeProdotto == "0002" && Pensione.CodeTipo == "0030") ||
                (Pensione.CodeGruppo == "0001" && Pensione.CodeProdotto == "0002" && Pensione.CodeTipo == "0045") ||
                (Pensione.CodeGruppo == "0001" && Pensione.CodeProdotto == "0001" && Pensione.CodeTipo == "0045" &&
                      this.TitolarePensione.Pensione.CodiceTipoRichiesta == "AV"))
            {
                retVal = true;
            }

            return retVal;
        }

        private void LoadDdlFigli()
        {
            string controlloDinamicoMemo228_2025 = string.Empty;
            DateTime DataLimite = new DateTime(1900, 1, 1);
            bool blnMemo228_2025 = false;

            if (checkMemo228_2025(this.TitolarePensione.Pensione))
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo228_2025", out controlloDinamicoMemo228_2025);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                {
                    ViewState["AbilitazioneMemo228_2025"] = controlloDinamicoMemo228_2025;
                    if (!string.IsNullOrEmpty(controlloDinamicoMemo228_2025))
                    {
                        if (DateTime.TryParse(controlloDinamicoMemo228_2025, out DataLimite))
                        {
                            if (DateTime.Now.Date >= DataLimite)
                                blnMemo228_2025 = true;
                        }
                        else
                        {
                            throw new INPS.DNA.DnaApplicationException("Errore nel Parse della Data del Controllo Dinamico controlloDinamicoMemo228_2025");
                        }
                    }
                }
            }

            ddlFigli.Items.Clear();
            ddlFigli.Items.Add(new ListItem("", "0"));
            ddlFigli.Items.Add(new ListItem("1", "1"));
            ddlFigli.Items.Add(new ListItem("2", "2"));
            if (blnMemo228_2025)
            {
                ddlFigli.Items.Add(new ListItem("3", "3"));
                ddlFigli.Items.Add(new ListItem("più di 3", "4"));
            }
            else
            {
                ddlFigli.Items.Add(new ListItem("più di 2", "3"));
            }
        }

        private string SetDateFormat()
        {
            string dateFormat = string.Empty;

            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo.Value)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            dateFormat = "{0:dd/MM/yyyy}";
                            break;
                        default:
                            dateFormat = "{0:MM/yyyy}";
                            break;
                    }
                }

                if (this.domanda.IsDomandaINPDAP)
                    dateFormat = "{0:dd/MM/yyyy}";
            }
            else
                dateFormat = "{0:MM/yyyy}";

            return dateFormat;
        }

        private void GestionePnlDecorrenzaPensioneFromTipoFondo()
        {
            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo.Value)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            pnlTxtDecorrenzaPensioneFSPT.Visible = true;
                            pnlTxtDecorrenzaPensione.Visible = false;
                            break;
                    }
                }

                if (this.domanda.IsDomandaINPDAP)
                {
                    pnlTxtDecorrenzaPensioneFSPT.Visible = true;
                    pnlTxtDecorrenzaPensione.Visible = false;
                }
            }

            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];
                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                bool IsEnteIstruttoreFondoExINPDAP = false;
                if (TitolarePensione != null)
                {
                    CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
                    IsEnteIstruttoreFondoExINPDAP = TitolarePensione.IsEnteIstruttoreFondoExINPDAP;
                }

                if ((Utility.IsDomandaIOCUM(this.domanda.Categoria) && tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione &&
                       (IsEnteIstruttoreFondoExINPDAP || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art2_C12_Legge335))
                    || Utility.IsPensioneInabilitaProficuoLavoroCumulo(this.domanda.Categoria, this.TitolarePensione.Pensione))
                {
                    pnlTxtDecorrenzaPensioneFSPT.Visible = true;
                    pnlTxtDecorrenzaPensione.Visible = false;
                }
                else
                {
                    pnlTxtDecorrenzaPensioneFSPT.Visible = false;
                    pnlTxtDecorrenzaPensione.Visible = true;
                }
            }
        }

        //private void GestionePerfRequisitiVisible(string gruppo, string prodotto)
        private void GestionePerfRequisitiVisible(AreaTitolare.DatiPensione datiPensione, Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            //if (gruppo == "0002" && (prodotto == "0011" || prodotto == "0012"))
            //if (gruppo == "0002" )  //sostituito dalla riga sotto. la riga precedente era già commentata
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            var tipoFondo = domanda.Tipofondo;
            if (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS)
            {
                if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
                {
                    hiddenFieldPerf.Value = "NO";
                }
                else
                    hiddenFieldPerf.Value = "SI";
            }
            else
            {
                if ((Utility.IsDomandaRipristino(datiPensione) && (this.domanda.Categoria.Substring(0, 1) == "S" || this.domanda.Categoria.Substring(0, 1) == "I"))
                    || (Utility.IsDomandaRiliquidazione(datiPensione) && (this.domanda.Categoria.Substring(0, 1) == "S" || this.domanda.Categoria.Substring(0, 1) == "I")))
                {
                    hiddenFieldPerf.Value = "NO";
                }
                else
                {
                    if ((tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Inabilita_Invalidita && !(Utility.IsDomandaIOPGI(this.domanda.Categoria) && !Utility.IsDomandaIOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro))) || tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Superstiti ||
                        Utility.IsDomandaINDCOM(this.domanda.Categoria) || Utility.IsDomandaPSO(this.domanda.Categoria) || Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                    {
                        hiddenFieldPerf.Value = "NO";
                    }
                    else
                        hiddenFieldPerf.Value = "SI";
                }
            }
        }

        private void GestioneEtichetteIsUnicarpe()
        {
            if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica ||
                this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica)
            {
                ddlLavoratorePubblico.Enabled = false;
            }
        }

        private string GetSedeDomanda()
        {
            if (Session["Domanda"] != null)
                return ((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).Sede;
            else
                return string.Empty;
        }

        internal AreaTitolare GetDatiUcAnagrafica()
        {

            try
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

                //ENG - memo 28/2024   
                string controlloDinamico28_2024 = string.Empty;
                if (ViewState["AbilitazioneMemo28_2024"] != null)
                    controlloDinamico28_2024 = (string)ViewState["AbilitazioneMemo28_2024"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out controlloDinamico28_2024);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo28_2024"] = controlloDinamico28_2024;
                }

                //ENG - Memo 57_2023                  
                string controlloDinamico57_2023 = string.Empty;
                if (ViewState["AbilitazioneMemo57_2023"] != null)
                    controlloDinamico57_2023 = (string)ViewState["AbilitazioneMemo57_2023"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo57_2023", out controlloDinamico57_2023);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo57_2023"] = controlloDinamico57_2023;
                }

                if (TitolarePensione == null)
                    TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];
                this.TitolarePensione.Pensione.NDomus = Int64.Parse(domanda.NumeroDomanda);
                this.TitolarePensione.Anagrafica.CodiceFiscale = Anagrafica.CodiceFiscale;
                this.TitolarePensione.Anagrafica.Cognome = Anagrafica.Cognome;
                this.TitolarePensione.Anagrafica.Nome = Anagrafica.Nome;
                this.TitolarePensione.Anagrafica.Sesso = Anagrafica.Sesso;
                this.TitolarePensione.Anagrafica.CodiceStatoCivile = Anagrafica.CodiceStatoCivile;
                this.TitolarePensione.Anagrafica.DataNascita = Anagrafica.DataNascita;
                this.TitolarePensione.Anagrafica.ComuneNascita = Anagrafica.ComuneNascita;
                this.TitolarePensione.Anagrafica.ProvinciaNascita = Anagrafica.ProvinciaNascita;
                this.TitolarePensione.Anagrafica.Indirizzo = Anagrafica.Indirizzo;
                this.TitolarePensione.Anagrafica.NumeroCivico = Anagrafica.NumeroCivico;
                this.TitolarePensione.Anagrafica.Cap = Anagrafica.Cap;
                this.TitolarePensione.Anagrafica.ComuneResidenza = Anagrafica.ComuneResidenza;
                this.TitolarePensione.Anagrafica.ProvinciaResidenza = Anagrafica.ProvinciaResidenza;
                this.TitolarePensione.Anagrafica.ResidenzaEstero = Anagrafica.ResidenzaEstero;
                this.TitolarePensione.Anagrafica.FrazioneResidenza = Anagrafica.FrazioneResidenza;

                if (!string.IsNullOrEmpty(ddlCittadinanza.SelectedItem.Text))
                    this.TitolarePensione.Anagrafica.Cittadinanza = ddlCittadinanza.SelectedItem.Value;
                else
                    this.TitolarePensione.Anagrafica.Cittadinanza = string.Empty;
                this.TitolarePensione.Anagrafica.Tel = txtTel.Text;
                this.TitolarePensione.Anagrafica.Cell = txtCell.Text;
                this.TitolarePensione.Anagrafica.EMail = txtEmail.Text;

                if (textBoxCert.Text != null && textBoxCert.Text != "")
                {
                    this.TitolarePensione.Pensione.NCertificato = int.Parse(textBoxCert.Text);
                }

                this.TitolarePensione.Pensione.DataPresentazioneDomanda = Utility.GetDateFromString(lblDataPresentazioneDomanda.Text);

                //ENG - memo 13 - opzionedonna2023 ddlFigli valorizzabile da view
                if (this.TitolarePensione.Pensione.Filtro == "KWA" || this.TitolarePensione.Pensione.Filtro == "KYA")
                {
                    if (!string.IsNullOrEmpty(ddlFigli.SelectedItem.Text))
                        this.TitolarePensione.Pensione.NumeroFigli = byte.Parse(ddlFigli.SelectedItem.Value);
                    else
                        this.TitolarePensione.Pensione.NumeroFigli = 0;
                }
                else if (this.TitolarePensione.Pensione.Filtro == "KXM" || this.TitolarePensione.Pensione.Filtro == "KZM")
                {
                    if (!string.IsNullOrEmpty(ddlFigliOpzDonna.SelectedItem.Text))
                        this.TitolarePensione.Pensione.NumeroFigli = byte.Parse(ddlFigliOpzDonna.SelectedItem.Value);
                    else
                        this.TitolarePensione.Pensione.NumeroFigli = 0;
                }

                if ((this.TitolarePensione.Pensione.FlagUnicarpe.HasValue && this.TitolarePensione.Pensione.FlagUnicarpe.Value &&
                    this.TitolarePensione.Pensione.TipoLetturaUnicarpe.HasValue && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica) ||// condizione di acquisizione da Felpe
                    (this.domanda.IsDomandaENPALS && !CodeUtility.IsEnpalsManualePL(this.domanda.IsDomandaENPALS, CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura), this.TitolarePensione.Pensione.IsDatiENPALSRecuperati)))
                {
                    if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                    {
                        if (this.domanda.Tipofondo.HasValue)
                        {
                            switch (this.domanda.Tipofondo.Value)
                            {
                                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                                    if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                                        this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                                    else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                                        this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                                    break;
                                default:
                                    if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                                        this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                                    else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                                        this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
                                    break;
                            }
                        }

                        if (this.domanda.IsDomandaINPDAP)
                        {
                            if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                            else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                        }
                    }
                    else
                    {
                        if (pnlTxtDecorrenzaPensioneFSPT.Visible == true)
                        {
                            if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                            else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                        }
                        else
                        {
                            if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                            else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
                        }
                    }

                    if (this.domanda.IsDomandaENPALS)
                    {
                        try  // caso raro per cui DataPerfezionamentoRequisitiUnicarpe.HasValue ma la DecorrenzaOriginaria non permette la visualizzazione della Requisiti
                        {
                            this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(lbltxtPerfezRequisiti.Text)));
                        }
                        catch (Exception)
                        {
                            try
                            {
                                this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtPerfRequisiti.Text)));
                            }
                            catch (Exception)
                            {
                                this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = null;
                            }
                        }
                    }
                    //la data perfezionamento requisiti in questo caso non deve mai essere valorizzata , se ci arriva common restituirà un errore 
                    else if (!(this.domanda.IsDomandaINPDAP && this.TitolarePensione.Pensione.TipoLetturaUnicarpe.HasValue && this.TitolarePensione.Pensione.TipoLetturaUnicarpe.Value == 'L' && (this.domanda.Categoria.Substring(0, 2).ToUpperInvariant() == "SO" || this.domanda.Categoria.Substring(0, 2).ToUpperInvariant() == "IO")))
                    {

                        if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiUnicarpe.HasValue)
                        {
                            try  // caso raro per cui DataPerfezionamentoRequisitiUnicarpe.HasValue ma la DecorrenzaOriginaria non permette la visualizzazione della Requisiti
                            {
                                this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(lbltxtPerfezRequisiti.Text)));
                            }
                            catch (Exception)
                            {
                                this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = null;
                            }
                        }

                        else
                        {
                            try
                            {
                                this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtPerfRequisiti.Text)));
                            }
                            catch (Exception ex)
                            {
                                this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = null;
                            }
                        }
                    }
                }
                else
                {
                    if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                    {
                        if (this.domanda.Tipofondo.HasValue)
                        {
                            switch (this.domanda.Tipofondo.Value)
                            {
                                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                                    this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                                    break;
                                default:
                                    this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
                                    break;
                            }
                        }

                        if (this.domanda.IsDomandaINPDAP)
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                    }
                    else
                    {
                        if (pnlTxtDecorrenzaPensioneFSPT.Visible == true)
                        {
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                        }
                        else
                        {
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
                        }
                    }


                    try
                    {
                        this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtPerfRequisiti.Text)));
                    }
                    catch (Exception)
                    {
                        this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = null;
                    }
                }

                //if (((AreaTitolare.DatiPensione)Session["DatiPensione"]) != null)
                //((AreaTitolare.DatiPensione)Session["DatiPensione"]).DecorrenzaOriginaria = this.TitolarePensione.Pensione.DecorrenzaOriginaria;
                this.TitolarePensione.Pensione.Fondo = TitolarePensione.Pensione.Fondo;
                if (pnlSindacato.Visible && !string.IsNullOrEmpty(ddlSindacato.SelectedItem.Text) && this.TitolarePensione.ElencoSindacati != null && this.TitolarePensione.ElencoSindacati.Count() > 0)
                {
                    this.TitolarePensione.Sindacato = new AreaTitolare.DatiSindacato();
                    this.TitolarePensione.Sindacato.CodiceSindacato = ddlSindacato.SelectedItem.Value;
                    this.TitolarePensione.Sindacato.IsFromService = !string.IsNullOrEmpty(hdnIsFromService.Value) ? bool.Parse(hdnIsFromService.Value) : (bool?)null;
                    string descrizione = ddlSindacato.SelectedItem.Text;
                    try
                    {
                        descrizione = descrizione.Substring(descrizione.IndexOf(" - ", StringComparison.CurrentCulture) + 3);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    this.TitolarePensione.Sindacato.DescrizioneSindacato = descrizione;

                    Sindacato sindacato = this.TitolarePensione.ElencoSindacati.ToList().Find(x => x.Id == ddlSindacato.SelectedItem.Value);
                    if (sindacato != null)
                        this.TitolarePensione.Sindacato.Stato = sindacato.Stato;
                }
                else
                    this.TitolarePensione.Sindacato = new AreaTitolare.DatiSindacato();

                if ((GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS) || !string.IsNullOrEmpty(txtSedeDestinazione.Text))
                {
                    this.TitolarePensione.Pensione.CodiceSedeDestinazione = CodeUtility.ControlSede(txtSedeDestinazione.Text);
                    if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value != AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                        this.TitolarePensione.Pensione.CentroOperativoDestinazione = CodeUtility.ControlCentroOperativo(txtSedeDestinazione.Text);
                }

                if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI && Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione)
                         && this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue && this.TitolarePensione.Pensione.CodiceSedeDestinazione.Value == 0)
                {
                    this.TitolarePensione.Pensione.CodiceSedeDestinazione = null;
                }

                //if aggiunto perchè su Produzione c'è un problema che su test non si presenta
                if (GetTipoAppartenenzaDomanda().Value.Equals(AreaTitolare.DatiPensione.TipoAppDomanda.CI) && Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione)
                          && !String.IsNullOrEmpty(this.TitolarePensione.Pensione.CodiceSedeDestinazione.ToString()))
                {
                    if (this.TitolarePensione.Pensione.CodiceSedeDestinazione.ToString().Equals("0"))
                        this.TitolarePensione.Pensione.CodiceSedeDestinazione = null;
                }

                if (this.TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione
                    || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione
                    || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                {
                    string lavoratorePubblico = ddlLavoratorePubblico.SelectedItem.Value;
                    if (!string.IsNullOrEmpty(lavoratorePubblico))
                        this.TitolarePensione.Pensione.LavoratorePubblico = lavoratorePubblico == "SI" ? true : (lavoratorePubblico == "NO" ? false : (bool?)null);
                }

                if (this.TitolarePensione.IsSceltaLavoratriciMadriVisible)
                {
                    byte numFigli = 0;
                    byte.TryParse(ddlFigli.SelectedItem.Value, out numFigli);
                    this.TitolarePensione.Pensione.NumeroFigli = numFigli == 0 ? (byte?)null : numFigli;
                    byte sceltaLM = 0;
                    byte.TryParse(ddlSeltaLM.SelectedItem.Value, out sceltaLM);
                    this.TitolarePensione.Pensione.SceltaLavoratriciMadri = sceltaLM == 0 ? (byte?)null : sceltaLM;
                    //ENG - memo 28/2024
                    if (!String.IsNullOrEmpty(controlloDinamico28_2024) && controlloDinamico28_2024.Trim().ToUpperInvariant() == "SI" &&
                        (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" && this.TitolarePensione.Pensione.CodeTipo == "0017") ||
                        (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" && this.TitolarePensione.Pensione.CodeTipo == "0045" &&
                        this.TitolarePensione.Pensione.CodiceTipoRichiesta == "AV"))
                    {
                        this.TitolarePensione.Pensione.NumeroFigli = numFigli;
                    }
                }

                if (pnlDataCondizioniPerComputo.Visible)
                {
                    if (string.IsNullOrEmpty(txtDataCondizioniPerComputo.Text))
                        this.TitolarePensione.Pensione.DataCondizioniPerComputo = null;
                    else
                        this.TitolarePensione.Pensione.DataCondizioniPerComputo = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtDataCondizioniPerComputo.Text)));
                }

                //ENG - Memo 57_2023
                if (!String.IsNullOrEmpty(controlloDinamico57_2023) && controlloDinamico57_2023.Trim().ToUpperInvariant() == "SI")
                {
                    if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
                    {
                        byte numFigli = 0;
                        byte.TryParse(ddlFigli.SelectedItem.Value, out numFigli);
                        this.TitolarePensione.Pensione.NumeroFigli = numFigli == 0 ? (byte?)null : numFigli;
                    }
                }

                return this.TitolarePensione;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAnagrafica, Errore nel metodo GetDatiUcAnagrafica" + ex);
            }
        }

        internal void ValorizzaEtichetteUCAnagrafica()
        {
            try
            {
                string dateFormat = SetDateFormat();

                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                //ENG - memo 28/2024   
                string controlloDinamico28_2024 = string.Empty;
                if (ViewState["AbilitazioneMemo28_2024"] != null)
                    controlloDinamico28_2024 = (string)ViewState["AbilitazioneMemo28_2024"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out controlloDinamico28_2024);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo28_2024"] = controlloDinamico28_2024;
                }

                string valoreControllo = string.Empty;
                if (ViewState["AbilitazioneMemo123_2021"] != null)
                    valoreControllo = (string)ViewState["AbilitazioneMemo123_2021"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out valoreControllo);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo123_2021"] = valoreControllo;
                }

                //ENG - Memo 57_2023
                string valoreControlloMemo57_2023 = string.Empty;
                if (ViewState["AbilitazioneMemo57_2023"] != null)
                    valoreControlloMemo57_2023 = (string)ViewState["AbilitazioneMemo57_2023"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo57_2023", out valoreControlloMemo57_2023);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo57_2023"] = valoreControlloMemo57_2023;
                }


                //ENG - Memo 228_2025
                string controlloDinamicoMemo228_2025 = string.Empty;
                if (ViewState["AbilitazioneMemo228_2025"] != null)
                    controlloDinamicoMemo228_2025 = (string)ViewState["AbilitazioneMemo228_2025"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo228_2025", out controlloDinamicoMemo228_2025);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo228_2025"] = controlloDinamicoMemo228_2025;
                }

                lblNumeroDomanda.Text = domanda.NumeroDomanda;
                lblCodiceFiscale.Text = this.TitolarePensione.Anagrafica.CodiceFiscale;
                lblCognome.Text = this.TitolarePensione.Anagrafica.Cognome;
                lblNome.Text = this.TitolarePensione.Anagrafica.Nome;
                if (this.TitolarePensione.Anagrafica.Sesso != null)
                    lblSesso.Text = this.TitolarePensione.Anagrafica.Sesso.ToString();
                lblDataDiNascita.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Anagrafica.DataNascita);
                lblComuneNascita.Text = this.TitolarePensione.Anagrafica.ComuneNascita;
                lblProvinciaStatoNascita.Text = this.TitolarePensione.Anagrafica.ProvinciaNascita;
                lblIndirizzo.Text = this.TitolarePensione.Anagrafica.Indirizzo;
                lblNCivico.Text = this.TitolarePensione.Anagrafica.NumeroCivico;
                lblCAP.Text = this.TitolarePensione.Anagrafica.Cap;
                lblComuneStatoResidenza.Text = this.TitolarePensione.Anagrafica.ComuneResidenza;
                lblProvincia.Text = this.TitolarePensione.Anagrafica.ProvinciaResidenza;
                lblFrazione.Text = this.TitolarePensione.Anagrafica.FrazioneResidenza;
                if (this.TitolarePensione.Anagrafica.ResidenzaEstero.HasValue && this.TitolarePensione.Anagrafica.ResidenzaEstero.Value)
                {
                    lblResidenteEstero.Text = "SI";
                    pnlFrazioneEstero.Visible = true;
                }
                else if (this.TitolarePensione.Anagrafica.ResidenzaEstero.HasValue && !this.TitolarePensione.Anagrafica.ResidenzaEstero.Value)
                    lblResidenteEstero.Text = "NO";
                else
                    lblResidenteEstero.Text = "NON PRESENTE";

                if (this.TitolarePensione.Anagrafica.DataMorte.HasValue)
                {
                    pnlDataMorte.Visible = true;
                    lblDataMorte.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Anagrafica.DataMorte);
                }

                if (!(String.IsNullOrEmpty(this.TitolarePensione.Anagrafica.Cittadinanza)))
                    ddlCittadinanza.SelectedValue = this.TitolarePensione.Anagrafica.Cittadinanza;
                else
                    ddlCittadinanza.SelectedIndex = 0;

                if (this.TitolarePensione.Anagrafica.IsNatoInItalia == true)
                    etichettaProvinciaStatoNascita.Text = "Provincia di Nascita:";
                else
                    etichettaProvinciaStatoNascita.Text = "Stato di Nascita:";
                if (this.TitolarePensione.Anagrafica.IsResidenteInItalia == true)
                    etichettaComuneStatoResidenza.Text = "Comune di Residenza:";
                else
                    etichettaComuneStatoResidenza.Text = "Stato di Residenza:";

                txtTel.Text = this.TitolarePensione.Anagrafica.Tel;
                txtCell.Text = this.TitolarePensione.Anagrafica.Cell;
                txtEmail.Text = this.TitolarePensione.Anagrafica.EMail;
                lblDataPresentazioneDomanda.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPresentazioneDomanda);

                if (hdnIsEsodati.Value == "SI")
                {
                    lblEtichettaDecorrenzaPensione.Text = "Decorrenza Assegno:";
                    validateDecorrenzaReq.ErrorMessage = "Inserire la data di decorrenza dell'assegno";
                    customCheckDecorrenzaPensione.ErrorMessage = "Decorrenza Assegno: data illogica";
                }

                if (CodeUtility.IsContributiva(this.TitolarePensione.Pensione) || Utility.IsDomandaVecchiaiaInComputo(this.TitolarePensione.Pensione))
                    hdnIsDPRVisibleBefore2011.Value = "SI";

                #region Gestione DecorrenzaPensione e DataPerfezionamentoRequisiti in funzione del FlagUnicarpe

                htxtDecorrenzaPensione.Value = String.Format(dateFormat, this.TitolarePensione.Pensione.DecorrenzaOriginaria);

                if ((this.TitolarePensione.Pensione.FlagUnicarpe.HasValue && this.TitolarePensione.Pensione.FlagUnicarpe.Value &&
                    this.TitolarePensione.Pensione.TipoLetturaUnicarpe.HasValue && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica) ||// condizione di acquisizione da Felpe
                    (this.domanda.IsDomandaENPALS && !CodeUtility.IsEnpalsManualePL(this.domanda.IsDomandaENPALS, CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura), this.TitolarePensione.Pensione.IsDatiENPALSRecuperati)) ||
                    this.TitolarePensione.IsDecorrenzaDisabledPerSuperstiti || this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica || this.TitolarePensione.Pensione.IsDomandaTotAutomatica)
                {
                    lblDecorrenzaPensione.Text = String.Format(dateFormat, this.TitolarePensione.Pensione.DecorrenzaOriginaria);

                    //txtDecorrenzaPensione.Text = String.Format("{0:MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria); // utilizzato lato html per la visibilità del pannello perfezionamento requisiti

                    //if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                    if ((this.TitolarePensione.Pensione.FlagUnicarpe.HasValue && this.TitolarePensione.Pensione.FlagUnicarpe.Value &&
                    this.TitolarePensione.Pensione.TipoLetturaUnicarpe.HasValue && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica) ||// condizione di acquisizione da Felpe
                    (this.domanda.IsDomandaENPALS && !CodeUtility.IsEnpalsManualePL(this.domanda.IsDomandaENPALS, CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura), this.TitolarePensione.Pensione.IsDatiENPALSRecuperati)) ||
                    this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica || this.TitolarePensione.Pensione.IsDomandaTotAutomatica)
                        hFlagUnicarpe.Value = "True";

                    validateDecorrenzaReqFSPT.Enabled = false;
                    validateDecorrenzaReq.Enabled = false;
                }
                //else
                //{

                    txtDecorrenzaPensione.Text = String.Format("{0:MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria);
                                
                if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                {
                    if (!(this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && (Utility.IsDomandaRipristinoSuperstiti(this.TitolarePensione.Pensione) && (this.TitolarePensione.Pensione.CodeTipo == "0026" || this.TitolarePensione.Pensione.CodeTipo == "0027"))))
                    {
                        txtDecorrenzaPensione.Enabled = false;
                    }
                    else
                    {
                        txtDecorrenzaPensione.Enabled = true;
                    }
                    
                    if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiUnicarpe.HasValue)
                    {
                        txtPerfRequisiti.Enabled = false;
                        txtDecorrenzaPensioneFSPT.Enabled = false;
                    }
                }

                if (Utility.IsDomandaVOTOT(domanda.Categoria) && this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                {
                    txtDecorrenzaPensione.Enabled = false;
                }

                //}
                txtDecorrenzaPensioneFSPT.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria);

                if (this.TitolarePensione.Pensione.IsDatiAggiuntiviFromJSON.GetValueOrDefault())
                {
                    txtDecorrenzaPensione.Enabled = false;
                }
                //GestionePerfRequisitiVisible(this.TitolarePensione.Pensione.CodeGruppo, this.TitolarePensione.Pensione.CodeProdotto);
                GestionePerfRequisitiVisible(this.TitolarePensione.Pensione, this.domanda);
                if (hiddenFieldPerf.Value == "SI")
                {
                    if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue &&
                        (this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value.CompareTo(new DateTime(2010, 12, 31)) > 0 || CodeUtility.IsContributiva(this.TitolarePensione.Pensione)))
                    {
                        if (this.TitolarePensione.Pensione.FlagUnicarpe.HasValue && this.TitolarePensione.Pensione.FlagUnicarpe.Value &&
                            this.TitolarePensione.Pensione.TipoLetturaUnicarpe.HasValue && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica) // condizione di acquisizione da Felpe
                        {
                            if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiUnicarpe.HasValue)
                            {
                                lbltxtPerfezRequisiti.Text = txtPerfRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiUnicarpe); // txtPerfRequisiti.Text serve solo in fase di salvataggio 
                                htxtPerfRequisiti.Value = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiUnicarpe);
                            }
                            else
                            {
                                txtPerfRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                                htxtPerfRequisiti.Value = string.Empty;
                            }
                        }
                        else
                        {
                            if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti.HasValue)
                            {
                                if (this.domanda.IsDomandaENPALS || this.domanda.IsDomandaINPDAP || this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica || this.TitolarePensione.Pensione.IsDomandaTotAutomatica)
                                    lbltxtPerfezRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);

                                txtPerfRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                                htxtPerfRequisiti.Value = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                            }
                        }
                    }
                    else if (Utility.IsDomandaVESO33(this.domanda.Categoria) ||
                       Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                       Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                       Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                       Utility.IsDomandaESOTEL(this.domanda.Categoria) ||
                       Utility.IsDomandaESOAMB(this.domanda.Categoria) ||
                       Utility.IsDomandaVESO29(this.domanda.Categoria))
                    {
                        if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti.HasValue)
                        {
                            txtPerfRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                            htxtPerfRequisiti.Value = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                        }
                    }

                    if (this.domanda.IsDomandaINPDAP || Utility.IsDomandaAnzianitaInComputo(this.TitolarePensione.Pensione) || Utility.IsDomandaVecchiaiaInComputo(this.TitolarePensione.Pensione))
                    {
                        if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti.HasValue)
                        {
                            lbltxtPerfezRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);

                            txtPerfRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                            htxtPerfRequisiti.Value = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                        }
                    }
                }
                #endregion Gestione DecorrenzaPensione e DataPerfezionamentoRequisiti in funzione del FlagUnicarpe

                if (!(String.IsNullOrEmpty(this.TitolarePensione.Patronato.Descrizione)))
                    lblPatronato.Text = this.TitolarePensione.Patronato.CodiceUfficio + " " + this.TitolarePensione.Patronato.Descrizione.Trim();

                if (!Utility.IsDomandaAPESociale(this.domanda.Categoria))
                {
                    if (!String.IsNullOrEmpty(this.TitolarePensione.Sindacato.CodiceSindacato))
                        ddlSindacato.SelectedValue = this.TitolarePensione.Sindacato.CodiceSindacato.Trim();
                    if (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                    {
                        if (this.TitolarePensione.Sindacato.IsFromService == true)
                            ddlSindacato.Enabled = false;
                        hdnIsFromService.Value = this.TitolarePensione.Sindacato.IsFromService != null ? this.TitolarePensione.Sindacato.IsFromService.ToString() : string.Empty;
                    }
                }
                else
                {
                    ddlSindacato.SelectedIndex = 0;
                    ddlSindacato.Enabled = false;
                }

                //Al momento la sede di destinazione non deve essere precompilata per i fondi speciali
                if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                {
                    txtSedeDestinazione.Text = this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue ?
                        CodeUtility.GetSede(this.TitolarePensione.Pensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0')) : string.Empty; //CodeUtility.GetSede(GetSedeDomanda());
                }
                else if (this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue)
                {
                    txtSedeDestinazione.Text = CodeUtility.GetSedeDa6(this.TitolarePensione.Pensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0') + (this.TitolarePensione.Pensione.CentroOperativoDestinazione.HasValue ?
                        this.TitolarePensione.Pensione.CentroOperativoDestinazione.ToString().PadLeft(2, '0') : "00"));
                }

                hiddenFieldTipoFondo.Value = domanda.Tipofondo != null && !String.IsNullOrEmpty(domanda.Tipofondo.Value.ToString()) ? domanda.Tipofondo.Value.ToString() : string.Empty;

                hiddenFieldDecDisabledSuperstiti.Value = this.TitolarePensione.IsDecorrenzaDisabledPerSuperstiti ? "SI" : "NO";

                if (this.TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione
                    || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione
                    || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                {
                    tdLabelLavoratorePubblico.Visible = true;
                    tdFieldLavoratorePubblico.Visible = true;
                    if (this.TitolarePensione.Pensione.LavoratorePubblico.HasValue)
                        ddlLavoratorePubblico.SelectedValue = this.TitolarePensione.Pensione.LavoratorePubblico.Value ? "SI" : "NO";
                    if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                        ddlLavoratorePubblico.Enabled = false;
                }

                if (this.TitolarePensione.IsSceltaLavoratriciMadriVisible &&
                    !(!string.IsNullOrEmpty(valoreControllo) && valoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(this.TitolarePensione.Pensione, this.domanda.Categoria, false)))
                {
                    trSceltaLM.Visible = true;
                    trNumFigli.Visible = true;
                    //if (checkMemo228_2025(this.TitolarePensione.Pensione) && blnMemo228_2025)
                    //    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 4 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "4";
                    //else
                    //    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 3 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "3";
                    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();

                    ddlSeltaLM.SelectedValue = this.TitolarePensione.Pensione.SceltaLavoratriciMadri.GetValueOrDefault().ToString();
                    if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica ||
                        (CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura)
                        && !this.TitolarePensione.Pensione.IsSceltaLavoratriciMadriEmpty) || this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica)
                    {
                        ddlFigli.Enabled = false;
                        ddlSeltaLM.Enabled = false;
                    }
                    else if ((CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura)
                        && this.TitolarePensione.Pensione.IsSceltaLavoratriciMadriEmpty))
                    {
                        ddlFigli.Enabled = true;
                        ddlSeltaLM.Enabled = false;
                    }
                    else if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione) && !this.TitolarePensione.Pensione.IsSceltaLavoratriciMadriEmpty)
                    {
                        ddlFigli.Enabled = false;
                        ddlSeltaLM.Enabled = false;
                    }

                }

                if (this.TitolarePensione.Pensione.IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA || this.TitolarePensione.Pensione.IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB)
                {
                    //ENG - memo_13_opzionedonna2023 ddlFigli bloccato solo per le opzionedonna automatiche(KWA-KYA)
                    if (this.TitolarePensione.Pensione.Filtro != "KXM" && this.TitolarePensione.Pensione.Filtro != "KZM")
                    {
                        trNumFigli.Visible = true;
                        ddlFigli.Enabled = false;
                        //if (blnMemo228_2025)
                        //    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 4 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "4";
                        //else
                        //    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 3 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "3";
                        ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();
                    }
                    else
                    {
                        trNumFigliOpzioneDonna.Visible = true;
                        ddlFigliOpzDonna.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 1 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "2";
                    }
                }

                if (!(this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                     && ((this.TitolarePensione.Pensione.DataOpzione.HasValue && this.TitolarePensione.Pensione.DataOpzione.Value != DateTime.MinValue) ||
                     (this.TitolarePensione.Pensione.DataRaggiungimentoOpzione.HasValue && this.TitolarePensione.Pensione.DataRaggiungimentoOpzione.Value != DateTime.MinValue)))
                {
                    trOpzione.Visible = true;
                    if (this.TitolarePensione.Pensione.DataOpzione.HasValue && this.TitolarePensione.Pensione.DataOpzione.Value != DateTime.MinValue)
                    {
                        tdOpz.Visible = true;
                        lblDataOpzione.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataOpzione.Value);
                    }
                    if (this.TitolarePensione.Pensione.DataRaggiungimentoOpzione.HasValue && this.TitolarePensione.Pensione.DataRaggiungimentoOpzione.Value != DateTime.MinValue)
                    {
                        tdRaggOpz.Visible = true;
                        lblDataRaggOpzione.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataRaggiungimentoOpzione.Value);
                    }

                }

                GestioneEtichetteIsUnicarpe();
                //ENG - Aggiornamento Memo 90/2016
                if ((!string.IsNullOrEmpty(valoreControllo) && valoreControllo == "SI" &&
                    (Utility.IsDomandaVecchiaiaInComputo(this.TitolarePensione.Pensione) || Utility.IsDomandaAnzianitaInComputo(this.TitolarePensione.Pensione))) ||
                    (this.domanda.TipoAppartenenza.GetValueOrDefault() == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                        ((this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0002" && this.TitolarePensione.Pensione.CodeTipo == "0192") ||
                        (this.TitolarePensione.Pensione.CodeGruppo == "0002" && this.TitolarePensione.Pensione.CodeProdotto == "0011" && this.TitolarePensione.Pensione.CodeTipo == "0045") ||
                        (this.TitolarePensione.Pensione.CodeGruppo == "0002" && this.TitolarePensione.Pensione.CodeProdotto == "0012" && this.TitolarePensione.Pensione.CodeTipo == "0045") ||
                        (this.TitolarePensione.Pensione.CodeGruppo == "0003" && this.TitolarePensione.Pensione.CodeProdotto == "0022" && this.TitolarePensione.Pensione.CodeTipo == "0045"))))
                {
                    pnlDataCondizioniPerComputo.Visible = true;
                    lblDataCondizioniPerComputo.Visible = true;

                    if (this.TitolarePensione.Pensione.DataCondizioniPerComputo.HasValue)
                        txtDataCondizioniPerComputo.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataCondizioniPerComputo.Value);
                    else
                        txtDataCondizioniPerComputo.Text = string.Empty;
                }

                //ENG - memo 28_2024 
                if (!String.IsNullOrEmpty(controlloDinamico28_2024) && controlloDinamico28_2024.Trim().ToUpperInvariant() == "SI")
                {
                    if ((this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" && this.TitolarePensione.Pensione.CodeTipo == "0017") ||
                        (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" && this.TitolarePensione.Pensione.CodeTipo == "0045" &&
                        this.TitolarePensione.Pensione.CodiceTipoRichiesta == "AV"))
                    {
                        hdnAbilitazioneMemo28_2024.Value = "SI";
                        ddlFigli.Items[0].Text = "0";
                        ddlFigli.Items.Insert(0, "");
                        if (!this.TitolarePensione.Pensione.NumeroFigli.HasValue && !(Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica ||
                            this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica))
                            ddlFigli.SelectedValue = "";
                        RequiredFieldValidatorDdlFigli.Enabled = true;
                        if (ddlFigli.SelectedValue == "0")
                        {
                            ddlSeltaLM.SelectedValue = "0";
                            ddlSeltaLM.Enabled = false;
                        }
                        else if (!(Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica ||
                            this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica))
                        {
                            ddlSeltaLM.Enabled = true;
                        }
                    }
                }

                //ENG - Memo 57_2023
                if (!String.IsNullOrEmpty(valoreControlloMemo57_2023) && valoreControlloMemo57_2023.Trim().ToUpperInvariant() == "SI")
                {
                    if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
                    {
                        trNumFigli.Visible = true;
                        //if(blnMemo228_2025)
                        //    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 4 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "4";
                        //else
                        //    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 3 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "3";
                        ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();
                        ddlFigli.Enabled = false;
                    }
                }

                //ENG - Memo 116/2025
                if (this.TitolarePensione.Pensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione || this.TitolarePensione.Pensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                {
                    trNumFigli.Visible = false;
                    trSceltaLM.Visible = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAnagrafica, Errore nel metodo ValorizzaEtichetteUCAnagrafica " + ex);
            }
        }

        internal void UpdateViewState(ITitolarePensione titolare)
        {
            ViewState["TitolarePensione"] = titolare.TitolarePensione;
        }

        public string GetAbsoluteUri(string relativeUri)
        {
            var uri = new Uri(Request.Url, ResolveUrl(relativeUri));
            return uri.AbsoluteUri;
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            PresenterTitolare presenterTitolare = new PresenterTitolare();
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();

            this.TitolarePensione.Anagrafica = this.GetDatiUcAnagrafica().Anagrafica;
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            bool isWarning = false;
            presenterTitolare.SalvaDatiTabAnagrafica(this, out isWarning);

            if (!this.HasError || isWarning)
            {
                if (this.TitolarePensione.Pensione != null && ((AreaTitolare.DatiPensione)Session["DatiPensione"]) != null)
                {
                    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).DecorrenzaOriginaria = this.TitolarePensione.Pensione.DecorrenzaOriginaria;
                    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).DataPerfezionamentoRequisiti = this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti;
                    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).CodiceSedeDestinazione = this.TitolarePensione.Pensione.CodiceSedeDestinazione;
                    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).CentroOperativoDestinazione = this.TitolarePensione.Pensione.CentroOperativoDestinazione;
                    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).NumeroFigli = this.TitolarePensione.Pensione.NumeroFigli;
                    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).SceltaLavoratriciMadri = this.TitolarePensione.Pensione.SceltaLavoratriciMadri;

                    AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
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
                                    AreaTitolare.DatiPensione.TipoAppDomanda AppDomanda = GetTipoAppartenenzaDomanda().Value;

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
            }

            ViewState["TitolarePensione"] = TitolarePensione;
            RaiseShowAvvisoAnagrafica(this, null);
        }

        protected void RaiseShowAvvisoAnagrafica(object sender, EventArgs e)
        {
            ShowAvvisoAnagrafica(sender, e);
        }

        public event EventHandler ShowAvvisoAnagrafica;

        public void SetTabCertificato()
        {
            if (Utility.IsDomandaPSO(this.domanda.Categoria) || Utility.IsDomandaPMO(this.domanda.Categoria))
            {
                certOpz.Visible = true;

                if (this.domanda.Certificato != null && this.domanda.Certificato != "00000000")
                {
                    textBoxCert.Text = this.domanda.Certificato;
                    textBoxCert.Text.PadLeft(8, '0');
                    textBoxCert.Enabled = false;
                }
                else
                {
                    textBoxCert.Enabled = true;
                }
            }
            else
            {
                certOpz.Visible = false;
            }
        }

        public void SetTabCertificatoAfterSave()
        {
            if (Utility.IsDomandaPSO(this.domanda.Categoria) || Utility.IsDomandaPMO(this.domanda.Categoria))
            {
                if (!string.IsNullOrEmpty(textBoxCert.Text))
                {
                    textBoxCert.Enabled = false;
                    textBoxCert.Text = textBoxCert.Text.PadLeft(8, '0');
                    if (this.domanda == null)
                        this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    this.domanda.Certificato = textBoxCert.Text;
                }

            }
        }

    }
}