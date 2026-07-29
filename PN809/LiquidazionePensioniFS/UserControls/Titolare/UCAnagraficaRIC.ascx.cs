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
    public partial class UCAnagraficaRIC : CustomBaseUserControl, ITitolarePensione, IDanteCausa
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

                if (this.domanda.IsDomandaINPDAP)
                    hdnIsINPDAP.Value = "SI";

                if (!Page.IsPostBack)
                {
                    ViewState["TitolarePensione"] = TitolarePensione;
                    LoadDdl(TitolarePensione);
                    GestionePnlDecorrenzaPensioneFromTipoFondo();
                    RenderControls();
                    ValorizzaEtichetteUCAnagrafica();
                }
                else
                    TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];
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
            try
            {
                string sedi = string.Empty;
                CodeUtility areaDecodifica = new CodeUtility();
                AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

                //CodeUtility.SetValueDdl(ddlSindacato, string.Empty, string.Empty, string.Empty);
                if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                    pnlSindacato.Visible = false;
                else
                {
                    pnlSindacato.Visible = true;
                    //29-01-2014 Non sarà più presente il record BLACK. Ma verrà sostituito dal record seguente. 
                    CodeUtility.SetValueDdl(ddlSindacato, "0 - NESSUN SINDACATO", "NESSUN SINDACATO", "0");
                    if (areaTitolare.ElencoSindacati != null && areaTitolare.ElencoSindacati.Count() > 0)
                    {
                        List<Sindacato> listaSindacati = areaTitolare.ElencoSindacati.OrderBy(x => x.Id).ToList();
                        foreach (Sindacato sindacato in listaSindacati)
                            CodeUtility.SetValueDdl(ddlSindacato, sindacato.Id.PadRight(2, ' ') + " - " + sindacato.Sigla, sindacato.Descrizione, sindacato.Id);
                    }
                }

                if ((GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS) ||
                    this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue)
                {

                    if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI &&
                        this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue && this.TitolarePensione.Pensione.CodiceSedeDestinazione.Value == 0)
                    {
                        pnlSedeDestinazioneRIC.Visible = false;
                    }
                    else
                    {
                        HiddenFieldSedi.Value = CodeUtility.LoadSedi();
                        if (HiddenFieldSedi.Value != string.Empty)
                        {
                            pnlSedeDestinazioneRIC.Visible = true;
                            //ddlSedeDestinazione.Visible = true;
                        }
                        else
                        {
                            pnlSedeDestinazioneRIC.Visible = false;
                            //ddlSedeDestinazione.Visible = false;
                        }
                    }
                }
                else
                {
                    pnlSedeDestinazioneRIC.Visible = false;
                    //ddlSedeDestinazione.Visible = false;
                }

                if ((GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue) ||
                    (this.TitolarePensione.Pensione.IsDomandaAPEPrecociOrRicostituzione && this.domanda.TipoAppartenenza != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS) ||
                    (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS))
                    txtSedeDestinazione.Enabled = false;

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
                throw new INPS.DNA.DnaApplicationException("UCAnagraficaRIC, Errore nel metodo LoadDdl " + ex);
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
                      this.TitolarePensione.Pensione.CodiceTipoRichiesta == "AV") || Utility.IsRicostituzione(this.TitolarePensione.Pensione))
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
                else if (this.domanda.IsDomandaINPDAP)
                    dateFormat = "{0:dd/MM/yyyy}";
            }
            else if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                bool IsEnteIstruttoreFondoExINPDAP = false;
                if (TitolarePensione != null)
                {
                    CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
                    IsEnteIstruttoreFondoExINPDAP = TitolarePensione.IsEnteIstruttoreFondoExINPDAP;
                    if (IsEnteIstruttoreFondoExINPDAP && Utility.IsDomandaIOCUM(this.domanda.Categoria))
                    {
                        dateFormat = "{0:dd/MM/yyyy}";
                    }
                    else
                        dateFormat = "{0:MM/yyyy}";
                }
                else
                    dateFormat = "{0:MM/yyyy}";
            }
            else
            {
                dateFormat = "{0:MM/yyyy}";
            }

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
                else if (this.domanda.IsDomandaINPDAP)
                {
                    pnlTxtDecorrenzaPensioneFSPT.Visible = true;
                    pnlTxtDecorrenzaPensione.Visible = false;
                }
            }

            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                pnlTxtDecorrenzaPensioneFSPT.Visible = false;
                pnlTxtDecorrenzaPensione.Visible = true;
            }
        }

        private bool IsVariazioneDecorrenza()
        {
            if (this.TitolarePensione != null && this.TitolarePensione.Pensione != null &&
                (this.TitolarePensione.Pensione.CodeProdotto == "0110" ||
                this.TitolarePensione.Pensione.CodeProdotto == "0310" ||
                this.TitolarePensione.Pensione.CodeProdotto == "0410"))
            {
                //da nuova indicazione la data perfezionamento requisiti non va mai visualizzata per le RIC
                //hiddenFieldPerf.Value = "SI";
                return true;
            }
            else
                return false;
        }

        private string GetSedeDomanda()
        {
            if (Session["Domanda"] != null)
                return ((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]).Sede;
            else
                return string.Empty;
        }

        private void RenderControls()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.TipoAppartenenza.HasValue)
            {
                switch (this.domanda.TipoAppartenenza.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                        ddlSindacato.Enabled = false;
                        hdnIsFromService.Value = this.TitolarePensione.Sindacato.IsFromService != null ? this.TitolarePensione.Sindacato.IsFromService.ToString() : string.Empty;
                        break;
                }
            }
        }

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
                else if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione)
                {
                    if (datiPensione.CodeProdotto.StartsWith("01"))
                        hiddenFieldPerf.Value = "SI";
                    else
                        hiddenFieldPerf.Value = "NO";
                }
                else
                    hiddenFieldPerf.Value = "SI";
            }
            else
            {
                if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Inabilita_Invalidita || tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Superstiti)
                {
                    hiddenFieldPerf.Value = "NO";
                }
                else if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione)
                {
                    if (datiPensione.CodeProdotto.StartsWith("01") && !Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) && !Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria) && !Utility.IsDomandaPSO(this.domanda.Categoria))
                        hiddenFieldPerf.Value = "SI";
                    else
                        hiddenFieldPerf.Value = "NO";
                }
                else
                    hiddenFieldPerf.Value = "SI";
            }

            //ENG - Gestione label perfezionamento requisiti RIC/TRF IOPGI NON MIGRATE
            if ((Utility.IsRicostituzione(datiPensione) || this.domanda.IsDomandaRiapertura) && Utility.IsDomandaIOPGI(this.domanda.Categoria) && String.IsNullOrEmpty(datiPensione.GP1AV91B)
                && datiPensione.DataPerfezionamentoRequisiti.HasValue)
            {
                hdnMostraLabelPerfezionamentoRequisiti.Value = "SI";
            }
        }

        private void GestioneEtichetteIsUnicarpe()
        {
            if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                ddlLavoratorePubblico.Enabled = false;
            }
        }

        internal AreaTitolare GetDatiUcAnagrafica()
        {
            try
            {
                AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

                //ENG - RIC/TRF Computo Senza Filtro PAV   
                string controlloDinamicoComputo = string.Empty;
                if (ViewState["AbilitazioneMemo123_2021"] != null)
                    controlloDinamicoComputo = (string)ViewState["AbilitazioneMemo123_2021"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out controlloDinamicoComputo);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo123_2021"] = controlloDinamicoComputo;
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


                //ENG - Memo 28/2024   
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

                if (TitolarePensione == null)
                    TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];
                this.TitolarePensione.Pensione.NDomus = Int64.Parse(Domanda.NumeroDomanda);
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

                this.TitolarePensione.Pensione.DataPresentazioneDomanda = Utility.GetDateFromString(lblDataPresentazioneDomanda.Text);

                if (!IsVariazioneDecorrenza())
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
                        else if (this.domanda.IsDomandaINPDAP)
                        {
                            if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                            else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                                this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                        }
                    }
                    else if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                    {
                        if (Utility.IsDomandaCumulo(this.domanda.Categoria))
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
                    else
                    {
                        if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(lblDecorrenzaPensione.Text);
                        else // unicarpe = true ma DecorrenzaOriginaria = null --> valore acquisito dall'inserimento manuale nella txtDecorrenzaPensione
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
                    }

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
                        catch (Exception)
                        {
                            this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti = null;
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
                        else if (this.domanda.IsDomandaINPDAP)
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                    }
                    else if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                    {
                        if (Utility.IsDomandaCumulo(this.domanda.Categoria))
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensioneFSPT.Text);
                        else
                            this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
                    }
                    else
                    {
                        this.TitolarePensione.Pensione.DecorrenzaOriginaria = Utility.GetDateFromString(txtDecorrenzaPensione.Text);
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
                //    ((AreaTitolare.DatiPensione)Session["DatiPensione"]).DecorrenzaOriginaria = this.TitolarePensione.Pensione.DecorrenzaOriginaria;
                this.TitolarePensione.Pensione.Fondo = TitolarePensione.Pensione.Fondo;

                if (pnlSindacato.Visible && !string.IsNullOrEmpty(ddlSindacato.SelectedItem.Text) && this.TitolarePensione.ElencoSindacati != null && this.TitolarePensione.ElencoSindacati.Count() > 0)
                {
                    this.TitolarePensione.Sindacato.CodiceSindacato = ddlSindacato.SelectedItem.Value;
                    string descrizione = ddlSindacato.SelectedItem.Text;
                    this.TitolarePensione.Sindacato.IsFromService = !string.IsNullOrEmpty(hdnIsFromService.Value) ? bool.Parse(hdnIsFromService.Value) : (bool?)null;
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
                    {
                        this.TitolarePensione.Sindacato.Stato = sindacato.Stato;
                    }
                    // this.TitolarePensione.Sindacato.Stato = this.TitolarePensione.ElencoSindacati.ToList().Find(x => x.Id == ddlSindacato.SelectedItem.Value).Stato;
                }
                else
                    this.TitolarePensione.Sindacato = new AreaTitolare.DatiSindacato();

                if ((GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS) || !string.IsNullOrEmpty(txtSedeDestinazione.Text))
                {
                    this.TitolarePensione.Pensione.CodiceSedeDestinazione = CodeUtility.ControlSede(txtSedeDestinazione.Text);
                    if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value != AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                        this.TitolarePensione.Pensione.CentroOperativoDestinazione = CodeUtility.ControlCentroOperativo(txtSedeDestinazione.Text);
                }

                if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI &&
                     this.TitolarePensione.Pensione.CodiceSedeDestinazione.HasValue && this.TitolarePensione.Pensione.CodiceSedeDestinazione.Value == 0)
                {
                    this.TitolarePensione.Pensione.CodiceSedeDestinazione = null;
                }

                //if aggiunto perchè su Produzione c'è un problema che su test non si presenta
                if (GetTipoAppartenenzaDomanda().Value.Equals(AreaTitolare.DatiPensione.TipoAppDomanda.CI) && !String.IsNullOrEmpty(this.TitolarePensione.Pensione.CodiceSedeDestinazione.ToString()))
                {
                    if (this.TitolarePensione.Pensione.CodiceSedeDestinazione.ToString().Equals("0"))
                        this.TitolarePensione.Pensione.CodiceSedeDestinazione = null;
                }

                if (this.TitolarePensione.IsSceltaLavoratriciMadriVisible)
                {
                    //ENG - Memo 28_2024: TRF/RIC Anticipate - slegare il numero figli dalla lavoratrice madre
                    bool eseguiControlloNumFigliLavoratriceMadre = true;

                    if (controlloDinamico28_2024 != null && controlloDinamico28_2024.Trim().ToUpperInvariant() == "SI"
                        && this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti.HasValue
                        && Utility.DataSuccessivaA(this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti.Value, new DateTime(2024, 1, 1)))
                    {
                        if (this.domanda.IsDomandaRiapertura)
                        {
                            if ((this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" && this.TitolarePensione.Pensione.CodeTipo == "0017")
                                || (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" && this.TitolarePensione.Pensione.CodeTipo == "0045" && this.TitolarePensione.Pensione.CodiceTipoRichiesta == "AV"))
                            {
                                eseguiControlloNumFigliLavoratriceMadre = false;
                            }
                        }
                        else if (Utility.IsRicostituzione(this.TitolarePensione.Pensione))
                        {
                            if ((GetTipoAppartenenzaDomanda() == AreaTitolare.DatiPensione.TipoAppDomanda.AGO || GetTipoAppartenenzaDomanda() == AreaTitolare.DatiPensione.TipoAppDomanda.CI))
                            {
                                if (this.TitolarePensione.Pensione.IdTipoPLPerRIC == 7 && !String.IsNullOrEmpty(this.TitolarePensione.Pensione.NaturaPensione)
                                   && (this.TitolarePensione.Pensione.NaturaPensione.Substring(0, 1) == "1" || this.TitolarePensione.Pensione.NaturaPensione.Substring(0, 1) == "2"))
                                {
                                    eseguiControlloNumFigliLavoratriceMadre = false;
                                }

                                if (GetTipoAppartenenzaDomanda() == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                                {
                                    if (this.TitolarePensione.Pensione.IdTipoPLPerRIC == 26)
                                        eseguiControlloNumFigliLavoratriceMadre = false;
                                }
                            }
                            else if (GetTipoAppartenenzaDomanda() == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                            {
                                if (this.TitolarePensione.Pensione.IdTipoPLPerRIC == 7)
                                {
                                    if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && this.TitolarePensione.Pensione.CodiceSpecifico == 47)
                                        || (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT && this.TitolarePensione.Pensione.CodiceSpecifico == 41)
                                        || (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT && this.TitolarePensione.Pensione.CodiceSpecifico == 14)
                                        || (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET && this.TitolarePensione.Pensione.CodiceSpecifico == 22))
                                    {
                                        eseguiControlloNumFigliLavoratriceMadre = false;
                                    }

                                    if (this.domanda.IsDomandaINPDAP && (this.TitolarePensione.Pensione.CodiceSpecifico == 181 || this.TitolarePensione.Pensione.CodiceSpecifico == 182))
                                    {
                                        eseguiControlloNumFigliLavoratriceMadre = false;
                                    }
                                }
                            }
                        }
                    }

                    //ACN segnalazione 42137 
                    if ((this.domanda.IsDomandaRiapertura || Utility.IsRicostituzione(this.TitolarePensione.Pensione)) && GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI && this.TitolarePensione.Pensione.IsDomandaTipoContributivo)
                    {
                        eseguiControlloNumFigliLavoratriceMadre = false;
                    }

                    byte numFigli = 0;
                    byte.TryParse(ddlFigli.SelectedItem.Value, out numFigli);
                    this.TitolarePensione.Pensione.NumeroFigli = numFigli == 0 ? (byte?)null : numFigli;
                    byte sceltaLM = 0;
                    byte.TryParse(ddlSeltaLM.SelectedItem.Value, out sceltaLM);
                    this.TitolarePensione.Pensione.SceltaLavoratriciMadri = sceltaLM == 0 ? (byte?)null : sceltaLM;
                    if (eseguiControlloNumFigliLavoratriceMadre)
                    {
                        if (this.TitolarePensione.Pensione.IsSceltaLavoratriciMadriEmpty)
                        {
                            if (numFigli > 0)
                            {
                                this.TitolarePensione.Pensione.SceltaLavoratriciMadri = 2;
                                ddlSeltaLM.SelectedValue = "2";
                            }
                            else
                            {
                                this.TitolarePensione.Pensione.SceltaLavoratriciMadri = (byte?)null;
                                ddlSeltaLM.SelectedValue = "0";
                            }
                        }
                    }
                }

                //ENG - RIC/TRF Anticipate Computo Senza Filtro PAV
                if (!String.IsNullOrEmpty(controlloDinamicoComputo) && controlloDinamicoComputo.Trim().ToUpperInvariant() == "SI"
                    && (Utility.IsDomandaAUTAnticipataInComputo(this.TitolarePensione.Pensione, this.domanda.Categoria, false) || this.TitolarePensione.Pensione.IdTipoPLPerRIC == 21)
                    && !(this.TitolarePensione.IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024 == true))
                {
                    this.TitolarePensione.Pensione.NumeroFigli = null;
                    this.TitolarePensione.Pensione.SceltaLavoratriciMadri = null;
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
                throw new INPS.DNA.DnaApplicationException("UCAnagraficaRIC, Errore nel metodo GetDatiUcAnagrafica" + ex);
            }
        }

        internal void ValorizzaEtichetteUCAnagrafica()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            try
            {
                string dateFormat = SetDateFormat();

                lblNumeroDomanda.Text = this.domanda.NumeroDomanda;
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

                //ENG - RIC/TRF Computo Senza Filtro PAV   
                string controlloDinamicoComputo = string.Empty;
                if (ViewState["AbilitazioneMemo123_2021"] != null)
                    controlloDinamicoComputo = (string)ViewState["AbilitazioneMemo123_2021"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out controlloDinamicoComputo);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo123_2021"] = controlloDinamicoComputo;
                }

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

                //ENG - Memo 123/2024 
                string controlloDinamicoMemo123_2024 = string.Empty;
                if (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null)
                    controlloDinamicoMemo123_2024 = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"];
                else
                {
                    Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out controlloDinamicoMemo123_2024);
                    if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneRIC_TRFMemo123_2024"] = controlloDinamicoMemo123_2024;
                }

                //ENG - Memo 123/2024 
                string controlloDinamicoMemo123_2024OpzioneContrib = string.Empty;
                if (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null)
                    controlloDinamicoMemo123_2024OpzioneContrib = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"];
                else
                {
                    Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out controlloDinamicoMemo123_2024OpzioneContrib);
                    if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] = controlloDinamicoMemo123_2024OpzioneContrib;
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
                {
                    ddlCittadinanza.SelectedValue = this.TitolarePensione.Anagrafica.Cittadinanza;
                }
                else
                {
                    ddlCittadinanza.SelectedIndex = 0;
                    if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                        btnCittadinanza.Enabled = true;
                    ddlCittadinanza.SelectedIndex = 0;
                }
                ddlCittadinanza.Enabled = true;

                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                {
                    btnCittadinanza.Visible = false;
                }

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

                if (CodeUtility.IsRicostituzioneOrRiaperturaAGOAutomaticaAbilitata(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura, this.domanda.Categoria) &&
                    this.domanda.Categoria.StartsWith("V"))
                {
                    if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiStoricoGP.HasValue)
                    {
                        trPerfRequisitiStorico.Visible = true;
                        lblPerfezionamentoReqStorico.Text = string.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiStoricoGP.Value);
                    }

                    if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione))
                        hdnPerfReqEditabile.Value = "SI";
                }
                else if (!this.TitolarePensione.Pensione.DataPerfezionamentoRequisitiStoricoGP.HasValue ||
                         (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione)))
                    hdnPerfReqEditabile.Value = "SI";

                if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, domanda.IsDomandaRiapertura) && (Utility.IsDomandaVESO92(domanda.Categoria) || Utility.IsDomandaVESO33(domanda.Categoria) || Utility.IsDomandaVOCOOP(domanda.Categoria) || Utility.IsDomandaVOCRED(domanda.Categoria) || Utility.IsDomandaVOESO(domanda.Categoria) || Utility.IsDomandaESPA(domanda.Categoria)))
                {
                    hdnPerfReqEditabile.Value = "SI";
                    if (Utility.IsDomandaVOCOOP(domanda.Categoria) || Utility.IsDomandaVOCRED(domanda.Categoria) || Utility.IsDomandaVOESO(domanda.Categoria))
                        hdnIsIsoPensione.Value = "SI";
                }

                if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, domanda.IsDomandaRiapertura) && (Utility.IsDomandaTotalizzazione(domanda.Categoria) || Utility.IsDomandaCumulo(domanda.Categoria) || Utility.IsDomandaBancari(domanda.Categoria)))
                {
                    hdnPerfReqEditabile.Value = "NO";
                }

                if (domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(TitolarePensione.Pensione, true) != Utility.TipoUnicarpe.Automatica &&
                    (Utility.IsDomandaVOAUT(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVOESO(this.domanda.Categoria) ||
                    Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)))
                {
                    hdnPerfReqEditabile.Value = "SI";
                }

                //ricostituzione con codici natura che individuano contributiva
                //if (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && (this.TitolarePensione.Pensione.NaturaPensione != null && (this.TitolarePensione.Pensione.NaturaPensione.StartsWith("1") || this.TitolarePensione.Pensione.NaturaPensione.StartsWith("2") || this.TitolarePensione.Pensione.NaturaPensione.StartsWith("6") || this.TitolarePensione.Pensione.NaturaPensione.StartsWith(" "))))
                //    hdnIsContributiva.Value = "SI";

                if (Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                    Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) ||
                    Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) ||
                    Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaESOPMI(this.domanda.Categoria))
                {
                    lblEtichettaDecorrenzaPensione.Text = "Decorrenza Assegno:";
                    validateDecorrenzaReq.ErrorMessage = "Inserire la data di decorrenza dell'assegno";
                    customCheckDecorrenzaPensione.ErrorMessage = "Decorrenza Assegno: data illogica";
                }

                #region Gestione DecorrenzaPensione e DataPerfezionamentoRequisiti in funzione della Variazione Decorrenza

                htxtDecorrenzaPensione.Value = String.Format(dateFormat, this.TitolarePensione.Pensione.DecorrenzaOriginaria);

                if (!IsVariazioneDecorrenza())
                {
                    lblDecorrenzaPensione.Text = String.Format(dateFormat, this.TitolarePensione.Pensione.DecorrenzaOriginaria);

                    //txtDecorrenzaPensione.Text = String.Format("{0:MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria); // utilizzato lato html per la visibilità del pannello perfezionamento requisiti

                    if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                        hVarDec.Value = "True";
                }
                //else
                //{
                txtDecorrenzaPensione.Text = String.Format("{0:MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria);
                txtDecorrenzaPensioneFSPT.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria);

                if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                {
                    if (IsVariazioneDecorrenza() || this.domanda.IsDomandaRiapertura ||
                        CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    {
                        if (this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti.HasValue)
                        {
                            txtPerfRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                            htxtPerfRequisiti.Value = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                            lbltxtPerfezRequisiti.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DataPerfezionamentoRequisiti);
                            if (CodeUtility.IsRicostituzione(TitolarePensione.Pensione) && Utility.IsDomandaUnicarpe(TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.IsDomandaENPALS)
                                txtPerfRequisiti.Enabled = false;
                        }
                    }
                }
                #endregion Gestione DecorrenzaPensione e DataPerfezionamentoRequisiti in funzione della Variazione Decorrenza

                if (!(String.IsNullOrEmpty(this.TitolarePensione.Patronato.Descrizione)))
                    lblPatronato.Text = this.TitolarePensione.Patronato.Descrizione.Trim();
                if (!(String.IsNullOrEmpty(this.TitolarePensione.Sindacato.CodiceSindacato)) && !Utility.IsDomandaAPESociale(this.domanda.Categoria))
                    ddlSindacato.SelectedValue = this.TitolarePensione.Sindacato.CodiceSindacato.Trim();
                else
                    if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
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

                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                hiddenFieldTipoFondo.Value = domanda.Tipofondo != null && !String.IsNullOrEmpty(domanda.Tipofondo.Value.ToString()) ? domanda.Tipofondo.Value.ToString() : string.Empty;

                GestionePerfRequisitiVisible(this.TitolarePensione.Pensione, this.domanda);

                if (this.TitolarePensione.Pensione.IsDomandaQuota100OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaQuota102OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                    (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024) && controlloDinamicoMemo123_2024.Trim().ToUpperInvariant() == "SI" && this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                    (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024OpzioneContrib) && controlloDinamicoMemo123_2024OpzioneContrib.Trim().ToUpperInvariant() == "SI" && this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione) ||
                    this.TitolarePensione.Pensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                {
                    tdLabelLavoratorePubblico.Visible = true;
                    tdFieldLavoratorePubblico.Visible = true;
                    if (this.TitolarePensione.Pensione.LavoratorePubblico.HasValue)
                        ddlLavoratorePubblico.SelectedValue = this.TitolarePensione.Pensione.LavoratorePubblico.Value ? "SI" : "NO";
                }
                //ENG - RIC/TRF Anticipate Computo Senza Filtro PAV
                if (this.TitolarePensione.IsSceltaLavoratriciMadriVisible &&
                    !(!string.IsNullOrEmpty(controlloDinamicoComputo) && controlloDinamicoComputo.Trim().ToUpperInvariant() == "SI" && (Utility.IsDomandaAUTAnticipataInComputo(this.TitolarePensione.Pensione, this.domanda.Categoria, false) || this.TitolarePensione.Pensione.IdTipoPLPerRIC == 21)))
                {
                    trSceltaLM.Visible = true;
                    trNumFigli.Visible = true;
                    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();// <= 3 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "3";
                    ddlSeltaLM.SelectedValue = this.TitolarePensione.Pensione.SceltaLavoratriciMadri.GetValueOrDefault().ToString();

                    bool isCampiSceltaLavMadriDisabled = ConfigurationManager.AppSettings["DisabilitaCampiSceltaLavMadri"] != null && ConfigurationManager.AppSettings["DisabilitaCampiSceltaLavMadri"] == "SI";

                    if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica ||
                    (CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura)
                    && !this.TitolarePensione.Pensione.IsSceltaLavoratriciMadriEmpty) || isCampiSceltaLavMadriDisabled)
                    {
                        ddlFigli.Enabled = false;
                        ddlSeltaLM.Enabled = false;
                    }
                    else if ((CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura)
                        && this.TitolarePensione.Pensione.IsSceltaLavoratriciMadriEmpty))
                    {
                        ddlFigli.Enabled = true;
                        ddlSeltaLM.Enabled = false;
                        hdnIsSceltaLavMadriEmpty.Value = "SI";
                    }
                }

                //ENG - memo 28/2024 DPR bloccata
                if ((Utility.IsRicostituzione(this.TitolarePensione.Pensione) && this.TitolarePensione.Pensione.IdTipoPLPerRIC.HasValue &&
                    (this.TitolarePensione.Pensione.IdTipoPLPerRIC == 26 || this.TitolarePensione.Pensione.IdTipoPLPerRIC == 29 || this.TitolarePensione.Pensione.IdTipoPLPerRIC == 7 ||
                    this.TitolarePensione.Pensione.IdTipoPLPerRIC == 28)) || (this.domanda.IsDomandaRiapertura && ((this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" &&
                    this.TitolarePensione.Pensione.CodeTipo == "0017") || (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0001" &&
                    this.TitolarePensione.Pensione.CodeTipo == "0045" && this.TitolarePensione.Pensione.CodiceTipoRichiesta == "AV") ||
                    (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0002" && this.TitolarePensione.Pensione.CodeTipo == "0001") ||
                    (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0002" && this.TitolarePensione.Pensione.CodeTipo == "0045") ||
                    (this.TitolarePensione.Pensione.CodeGruppo == "0001" && this.TitolarePensione.Pensione.CodeProdotto == "0002" && this.TitolarePensione.Pensione.CodeTipo == "0017"))))
                {
                    txtPerfRequisiti.Enabled = false;
                    ddlFigli.Enabled = false;
                }

                if (this.TitolarePensione.Pensione.IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA || this.TitolarePensione.Pensione.IsRicostituzioneOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB ||
                    this.TitolarePensione.Pensione.IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraA || this.TitolarePensione.Pensione.IsDomandaOpzioneDonna_Legge197_2022_Art1_Comma292_LetteraB)
                {
                    if (this.TitolarePensione.Pensione.IdTipoPLPerRIC != 16 && this.TitolarePensione.Pensione.IdTipoPLPerRIC != 18)
                    {
                        trNumFigli.Visible = true;
                        ddlFigli.Enabled = false;
                        ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();
                    }
                    else
                    {
                        trNumFigliOpzioneDonna.Visible = true;
                        ddlFigliOpzDonna.Enabled = false;
                        ddlFigliOpzDonna.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault() <= 1 ? this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString() : "2";
                    }
                }

                if (!(this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                     (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                     (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione) ||
                     this.TitolarePensione.Pensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
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
                if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI && this.TitolarePensione.Pensione.IsDomandaTipoContributivo)
                {
                    ddlFigli.Enabled = false;
                    ddlSeltaLM.Enabled = false;
                }

                //ENG - Memo 57_2023
                if (!String.IsNullOrEmpty(valoreControlloMemo57_2023) && valoreControlloMemo57_2023.Trim().ToUpperInvariant() == "SI")
                {
                    if (Utility.IsDomandaAPESociale(this.domanda.Categoria))
                    {
                        trNumFigli.Visible = true;
                        ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();
                        ddlFigli.Enabled = false;
                    }
                }
                if (GetTipoAppartenenzaDomanda().HasValue && GetTipoAppartenenzaDomanda().Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                    ddlSindacato.Enabled = false;

                GestioneEtichetteIsUnicarpe();

                //ENG - Memo 123/2024 aggiornato al 27/03/2025
                if ((Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione.CodeGruppo, this.TitolarePensione.Pensione.CodeProdotto) && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) != Utility.TipoUnicarpe.Automatica &&
                    (this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || this.TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                    this.TitolarePensione.Pensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione || this.TitolarePensione.Pensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione ||
                    this.TitolarePensione.Pensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)
                    hdnPerfReqEditabile.Value = "NO";

                if (this.TitolarePensione.IsRicVoautNoFiltroPavAssunzioneCaricoEntro042024 == true)
                {
                    trSceltaLM.Visible = true;
                    trNumFigli.Visible = true;
                    ddlFigli.SelectedValue = this.TitolarePensione.Pensione.NumeroFigli.GetValueOrDefault().ToString();
                    ddlSeltaLM.SelectedValue = this.TitolarePensione.Pensione.SceltaLavoratriciMadri.GetValueOrDefault().ToString();
                    ddlFigli.Enabled = false;
                    ddlSeltaLM.Enabled = false;
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
                throw new INPS.DNA.DnaApplicationException("UCAnagraficaRIC, Errore nel metodo ValorizzaEtichetteUCAnagrafica " + ex);
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
    }
}
