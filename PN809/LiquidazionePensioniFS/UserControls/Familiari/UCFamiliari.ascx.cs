using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Familiari
{
    public partial class UCFamiliari : CustomBaseUserControl, IFamiliari, ITitolarePensione, IViewUI, IDanteCausa
    {
        #region IFamiliari

        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoAnagrafica areaRiepilogoAnagrafica { get; set; }
        public List<PresenterFamiliari.FamiliareFull> elencoFamiliari { get; set; }
        public List<GestioneAreaFamiliariAreaFamiliare> areaFamiliare { get; set; }
        public List<Anagrafica> anagrafica { get; set; }
        public GestioneFamiliariConsultazioneUnificataANF consultazioneANF { get; set; }
        public List<string> familiariToDelete { get; set; }
        public string codiceFiscale { get; set; }
        public GestioneAreaFamiliariAreaDecFam areaDecodifica { get; set; }
        public AreaEsito areaEsito { get; set; }

        #endregion IFamiliari

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        public event EventHandler SalvaFamiliari;
        public event EventHandler FamiliariNonSalvati;
        public event EventHandler AddModFamiliareEvent;
        public event EventHandler EliminaFamiliari;
        public event EventHandler ShowAvviso;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Spacchettate SOPGI
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            if (!IsPostBack)
            {
                AreaTitolare.DatiPensione datipensione = this.GetDatiPensione(this);
                hfDecorrenzaOriginaria.Value = datipensione.DecorrenzaOriginaria.ToString();

                string controlloDinamico = string.Empty;
                string controlloDinamicoAbilitazioneSpachettate024 = string.Empty;
                PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamico);
                if (esito != null && esito.RisultatoOperazione == AreaEsito.TipoEsito.OK)
                    controlloDinamicoAbilitazioneSpachettate024 = controlloDinamico;

                GetFamiliariFromService();

                ManagerViewState();

                pnlSearch.Visible = false;
                btnSalva.Visible = false;
                btnEliminaFamiliari.Visible = true;
                if (Utility.IsDomandaEsodo(this.domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datipensione, this.domanda.IsDomandaRiapertura))
                    btnAddFamiliare.Enabled = true;
                else if (this.domanda.IsDomandaRiapertura || Utility.IsDomandaSpacchettamentoINPDAP(this.domanda.IsDomandaINPDAP, this.domanda.Categoria)
                    || (!String.IsNullOrEmpty(controlloDinamicoAbilitazioneSpachettate024) && controlloDinamicoAbilitazioneSpachettate024.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione)))
                    btnAddFamiliare.Enabled = false;

                DataBind();

                GestioneVariazioneDatiContitolari();

                //Cambio Nomi Colonne SPT/SFS.
                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                        && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS
                        && this.domanda.CodGruppo.Equals("0031"))
                {
                    gvCodiciMaggiorazione.Columns[(int)GvCodMaggiorazioneColumns.LblQuotaAf].HeaderText = "Pagamento IIS";
                    gvCodiciMaggiorazione.Columns[(int)GvCodMaggiorazioneColumns.DdlQuotaAf].HeaderText = "Pagamento IIS";
                    gvCodiciMaggiorazione.Columns[(int)GvCodMaggiorazioneColumns.LblContitolarietaFondo].HeaderText = "Trattamento Minimo";
                    gvCodiciMaggiorazione.Columns[(int)GvCodMaggiorazioneColumns.DdlContitolarietaFondo].HeaderText = "Trattamento Minimo";
                }
            }
            GestioneInfoAgoCi();

            imgCercaAltriFamiliari.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/search24.png";
        }

        private void ManageParentelaAndCodMaggiorazioneForAGOAndCI()
        {
            trParentela.Visible = false;
            ExtraInfo.Visible = true;
            ViewState["ChangeIndexDDL"] = false;
            managerBtnSalva();
            GvCodiciMaggiorazione_Load();
        }

        #region private methods

        private void GetFamiliariFromService()
        {
            PresenterFamiliari Familiari = new PresenterFamiliari();
            Familiari.GetFamiliareByNumDomanda(this, this);
        }

        private void ManagerViewState()
        {
            ViewState["ListFamiliari"] = this.elencoFamiliari;

            List<string> CodFiscali = new List<string>();
            ViewState["ToBeDeleted"] = CodFiscali;

            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = new List<CodiciMaggiorazione>();
            ViewState["elencoCodiciMaggiorazione"] = elencoCodiciMaggiorazione;

            ViewState["DatiDecodifica"] = this.areaDecodifica;

            string controlloDinamico = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamico);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                ViewState["AbilitazioneSpacchettamento024"] = controlloDinamico;
        }

        private void managerBtnSalva()
        {
            if (ViewState["elencoCodiciMaggiorazione"] == null || ((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"]).Count == 0 ||
                (((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"]).Count == 1 &&
                    String.IsNullOrEmpty(((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"])[0].DescParentela) &&
                    String.IsNullOrEmpty(((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"])[0].Acquisizione) &&
                    String.IsNullOrEmpty(((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"])[0].Cessazione) &&
                    String.IsNullOrEmpty(((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"])[0].CodMaggiorazione) &&
                    String.IsNullOrEmpty(((List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"])[0].Maggiorazione)) ||
                (int?)ViewState["RowIndexEdit"] != null ||
                (bool)ViewState["ChangeIndexDDL"] ||
                (bool)ViewState["InsertNewRecord"])
                this.btnSalva.Enabled = false;
            else
                this.btnSalva.Enabled = true;
        }

        private void CaricaDropParentela(DropDownList ddlParentela, bool bypassConiugeAndUnitoCivilmente)
        {
            if (this.areaDecodifica == null)
                this.areaDecodifica = ViewState["DatiDecodifica"] as GestioneAreaFamiliariAreaDecFam;
            List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> lGradoFamiliare = this.areaDecodifica.ElencoSiglaFamiliare.ToList();
            ddlParentela.Items.Clear();
            // Unito Civilmente
            var idx = lGradoFamiliare.FindIndex(x => x.Id == "C" && x.TipoUnione == "U");
            var item = lGradoFamiliare[idx];
            lGradoFamiliare.RemoveAt(idx);
            lGradoFamiliare.Insert(0, item);

            // Coniuge
            idx = lGradoFamiliare.FindIndex(x => x.Id == "C" && x.TipoUnione == "M");
            item = lGradoFamiliare[idx];
            lGradoFamiliare.RemoveAt(idx);
            lGradoFamiliare.Insert(0, item);

            CodeUtility.SetValueDdl(ddlParentela, string.Empty, string.Empty, string.Empty);
            foreach (GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare gradoFamiliare in lGradoFamiliare)
            {
                if (!(bypassConiugeAndUnitoCivilmente && gradoFamiliare.Id == "C"))
                    CodeUtility.SetValueDdl(ddlParentela, gradoFamiliare.Id + " - " + gradoFamiliare.Descrizione, gradoFamiliare.Descrizione, gradoFamiliare.Id + gradoFamiliare.TipoUnione);
            }
        }

        private void Clearfields()
        {
            txtCFAltriFamiliari.Text = "";
            Lbcognome.Text = "";
            LbNome.Text = "";
            lbCognAcquisito.Text = "";
            LbComunedinascita.Text = "";
            LbDataDiNascita.Text = "";
            LbProvinciadinascita.Text = "";
            LbSesso.Text = "";
            txtRevSan.Text = "MM/AAAA";
            //txtDataMorte.Text = "GG/MM/AAAA";
            lblDataMorteValue.Text = string.Empty;

            DropParentela.ClearSelection();
        }

        private void GestioneInfoAgoCi()
        {
            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datipensione != null)
            {

                if (datipensione.TipoAppartenenzaDomanda.HasValue && datipensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                {
                    rowAgoCi.Visible = false;
                    trInformativaErroriInLoop.Visible = false;
                }
                else
                {
                    trInformativaErroriInLoop.Visible = datipensione.TipoAppartenenzaDomanda.HasValue && datipensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.AGO &&
                                                        CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && !this.domanda.IsDomandaENPALS
                                                        && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datipensione, this.areaDanteCausa)
                                                        && !Utility.IsDomandaSpacchettamentoSO(datipensione, this.domanda) && !Utility.IsDomandaSpacchettamentoSOART(datipensione, this.domanda)
                                                        && !Utility.IsDomandaSpacchettamentoSOCOM(datipensione, this.domanda) && !Utility.IsDomandaSpacchettamentoSR(datipensione, this.domanda);
                    rowAgoCi.Visible = true;
                    if (Utility.IsDomandaINDCOM(this.domanda.Categoria) || Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                    {
                        txtRevSan.Text = string.Empty;
                        txtRevSan.Enabled = false;
                    }
                }


            }
        }

        private void GestioneInfoAgoCi_Edit()
        {
            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            if (datipensione != null)
            {
                if (datipensione.TipoAppartenenzaDomanda.HasValue && datipensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                    rowAgoCi.Visible = false;
                else
                    rowAgoCi.Visible = true;
            }
        }

        private void GetNewFamiliare()
        {
            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = (List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"];
            RemoveItemBlank(ref elencoCodiciMaggiorazione);

            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];

            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica newuser = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)ViewState["newfamiliare"];
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (newuser != null)
            {
                GestioneAreaFamiliariAreaFamiliare familiare = new GestioneAreaFamiliariAreaFamiliare();
                familiare.Familiare = new GestioneFamiliariFamiliare();
                familiare.ElencoCodMaggFamiliari = new GestioneFamiliariCodMaggFamiliari[elencoCodiciMaggiorazione.Count];
                for (int i = 0; i < elencoCodiciMaggiorazione.Count; i++)
                {
                    familiare.ElencoCodMaggFamiliari[i] = new GestioneFamiliariCodMaggFamiliari();
                    if (!string.IsNullOrEmpty(elencoCodiciMaggiorazione[i].CodMaggiorazione))
                        familiare.ElencoCodMaggFamiliari[i].CodiceMaggiorazione = byte.Parse(elencoCodiciMaggiorazione[i].CodMaggiorazione);

                    familiare.ElencoCodMaggFamiliari[i].Decorrenza = Utility.GetDateFromString(elencoCodiciMaggiorazione[i].Acquisizione);

                    string codParentela = string.Empty;
                    if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                    {
                        codParentela = elencoCodiciMaggiorazione[i].CodParentela;
                        familiare.ElencoCodMaggFamiliari[i].SiglaFamiliare = Char.Parse(codParentela.Substring(0, 1));
                        // Effettuo il PadRight perchè non tutte le tipologie di parentela hanno il secondo carattere
                        familiare.ElencoCodMaggFamiliari[i].TipoUnione = codParentela.PadRight(2, ' ').Substring(1, 1);
                    }
                    else
                        codParentela = DropParentela.SelectedValue;

                    if (txtCFAltriFamiliari.Text != "" && codParentela != "CM" && codParentela != "CU" && codParentela != "I" && codParentela != "F" && codParentela != "G")
                    {
                        familiare.ElencoCodMaggFamiliari[i].Cessazione = !string.IsNullOrEmpty(elencoCodiciMaggiorazione[i].Cessazione) ? Utility.GetDateFromString(elencoCodiciMaggiorazione[i].Cessazione) : null;
                    }
                    else
                    {
                        if (!elencoCodiciMaggiorazione[i].Cessazione.Equals("MM/AAAA") && elencoCodiciMaggiorazione[i].Cessazione != string.Empty)
                            familiare.ElencoCodMaggFamiliari[i].Cessazione = Utility.GetDateFromString(elencoCodiciMaggiorazione[i].Cessazione);
                        else familiare.ElencoCodMaggFamiliari[i].Cessazione = null;

                        familiare.Familiare.CodiceDetrazioni = null;
                    }

                    if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) 
                        && this.domanda.CodGruppo.Equals("0031")
                        && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                    {
                        familiare.ElencoCodMaggFamiliari[i].QuotaAF = elencoCodiciMaggiorazione[i].QuotaAf;
                        familiare.ElencoCodMaggFamiliari[i].ContitolaritaFondo = elencoCodiciMaggiorazione[i].ContitolaritaFondo;
                    }

                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                    {
                        familiare.ElencoCodMaggFamiliari[i].DirittoAF = elencoCodiciMaggiorazione[i].DirittoAf;
                        familiare.ElencoCodMaggFamiliari[i].QuotaAF = elencoCodiciMaggiorazione[i].QuotaAf;
                        familiare.ElencoCodMaggFamiliari[i].ContitolaritaFondo = elencoCodiciMaggiorazione[i].ContitolaritaFondo;
                        familiare.ElencoCodMaggFamiliari[i].ContitolaritaAgo = elencoCodiciMaggiorazione[i].ContitolaritaAgo;
                    }
                }
                if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                {
                    if (familiare.ElencoCodMaggFamiliari != null && familiare.ElencoCodMaggFamiliari.Count() > 0)
                    {
                        GestioneFamiliariCodMaggFamiliari lastCodMaggiorazione = familiare.ElencoCodMaggFamiliari.OrderByDescending(x => x.Decorrenza).FirstOrDefault();
                        if (lastCodMaggiorazione != null && lastCodMaggiorazione.SiglaFamiliare.HasValue && !string.IsNullOrEmpty(lastCodMaggiorazione.TipoUnione))
                        {
                            familiare.Familiare.SiglaFamiliare = lastCodMaggiorazione.SiglaFamiliare;
                            familiare.Familiare.TipoUnione = lastCodMaggiorazione.TipoUnione;
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(DropParentela.SelectedValue))
                    {
                        familiare.Familiare.SiglaFamiliare = Char.Parse(DropParentela.SelectedValue.Substring(0, 1));
                        // Effettuo il PadRight perchè non tutte le tipologie di parentela hanno il secondo carattere
                        familiare.Familiare.TipoUnione = DropParentela.SelectedValue.PadRight(2, ' ').Substring(1, 1);
                    }
                }
                if (!txtRevSan.Text.Equals("MM/AAAA") && txtRevSan.Text != string.Empty)
                    familiare.Familiare.ScadenzaRevisioneSanitaria = Utility.GetDateFromString(txtRevSan.Text);
                else
                    familiare.Familiare.ScadenzaRevisioneSanitaria = null;
                if (lblDataMorteValue.Visible && lblDataMorteValue.Text != string.Empty)
                    familiare.Familiare.DataMorte = Utility.GetDateFromString(lblDataMorteValue.Text);
                else
                    familiare.Familiare.DataMorte = null;
                familiare.Familiare.numerodomanda = domanda.NumeroDomanda;
                familiare.Familiare.CodiceFiscale = newuser.CodiceFiscale;
                familiare.Familiare.Confermato = true;
                if (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI)
                {
                    if (this.elencoFamiliari != null && this.elencoFamiliari.Count > 0)
                        familiare.Familiare.Progressivo = (char)((byte)this.elencoFamiliari.Max(x => x.areaFamiliare.Familiare.Progressivo.GetValueOrDefault()) + 1);
                    else
                        familiare.Familiare.Progressivo = (char)0;
                }

                Anagrafica newanagrafica = new Anagrafica();
                newanagrafica.Nome = newuser.Nome;
                newanagrafica.Cognome = newuser.Cognome;
                newanagrafica.DataNascita = newuser.DataNascita;
                newanagrafica.CodiceFiscale = newuser.CodiceFiscale;
                newanagrafica.ComuneNascita = newuser.ComuneNascita;
                newanagrafica.Sesso = newuser.Sesso;
                newanagrafica.ProvinciaNascita = newuser.ProvinciaNascita;
                newanagrafica.CognomeAcquisito = newuser.CognomeAcquisito;
                newanagrafica.CodiceComuneNascita = newuser.CodiceComuneNascita;
                newanagrafica.Cittadinanza = newuser.Cittadinanza;
                newanagrafica.ComuneResidenza = newuser.ComuneResidenza;
                newanagrafica.CodiceComuneResidenza = newuser.CodiceComuneResidenza;
                newanagrafica.Indirizzo = newuser.Indirizzo;
                newanagrafica.NCivico = newuser.NumeroCivico;
                newanagrafica.CAP = newuser.Cap;
                newanagrafica.DomicilioEstero = newuser.DomicilioEstero;
                newanagrafica.ResidenzaEstero = newuser.ResidenzaEstero;
                newanagrafica.Codice1Arca = newuser.Codice1Arca;
                newanagrafica.Codice2Arca = newuser.Codice2Arca;
                newanagrafica.Tel = newuser.Tel;
                newanagrafica.Cell = newuser.Cell;
                newanagrafica.EMail = newuser.EMail;
                newanagrafica.ProvinciaResidenza = newuser.ProvinciaResidenza;
                newanagrafica.FrazioneResidenza = newuser.FrazioneResidenza;
                newanagrafica.CodiceStatoCivile = newuser.CodiceStatoCivile;
                newanagrafica.DecorrenzaStatoCivile = newuser.DecorrenzaStatoCivile;
                newanagrafica.DataMorte = newuser.DataMorte;

                List<PresenterFamiliari.FamiliareFull> ListFamiliariFullDistinct = new List<PresenterFamiliari.FamiliareFull>();
                foreach (PresenterFamiliari.FamiliareFull famFull in this.elencoFamiliari)
                    ListFamiliariFullDistinct.Add(famFull);

                int index = this.elencoFamiliari.FindIndex(x => x.anagrafica.CodiceFiscale == hidenCF.Text);

                PresenterFamiliari.FamiliareFull familiareFull = new PresenterFamiliari.FamiliareFull();
                familiareFull.anagrafica = newanagrafica;
                familiareFull.areaFamiliare = familiare;

                if (index < 0) // non è un doppione
                    ListFamiliariFullDistinct.Add(familiareFull);
                this.elencoFamiliari.Add(familiareFull);

                ViewState["ListFamiliari"] = ListFamiliariFullDistinct;
                ViewState["ListFamiliariDuplicate"] = this.elencoFamiliari;
                this.codiceFiscale = txtCFAltriFamiliari.Text;
            }
        }

        private void GetFamiliareModify()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = (List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"];
            RemoveItemBlank(ref elencoCodiciMaggiorazione);

            string Idanagrafica = (string)ViewState["Idanagraficamod"];
            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
            int indexFamiliaremodificato = int.Parse(hfRowIndex.Value);
            PresenterFamiliari.FamiliareFull ModifyFamiliare = this.elencoFamiliari[indexFamiliaremodificato];

            ModifyFamiliare.areaFamiliare.ElencoCodMaggFamiliari = new GestioneFamiliariCodMaggFamiliari[elencoCodiciMaggiorazione.Count];

            List<GestioneFamiliariCodMaggFamiliari> listCodiciMaggiorazione = new List<GestioneFamiliariCodMaggFamiliari>();
            foreach (CodiciMaggiorazione codMagg in elencoCodiciMaggiorazione)
            {
                GestioneFamiliariCodMaggFamiliari c = new GestioneFamiliariCodMaggFamiliari();
                if (codMagg.Acquisizione == string.Empty)
                    c.Decorrenza = null;
                else
                    c.Decorrenza = Utility.GetDateFromString(codMagg.Acquisizione);
                if (codMagg.Cessazione == string.Empty)
                    c.Cessazione = null;
                else
                    c.Cessazione = Utility.GetDateFromString(codMagg.Cessazione);
                if (!String.IsNullOrEmpty(codMagg.CodMaggiorazione))
                    c.CodiceMaggiorazione = byte.Parse(codMagg.CodMaggiorazione);

                if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                {
                    c.SiglaFamiliare = Char.Parse(codMagg.CodParentela.Substring(0, 1));
                    // Effettuo il PadRight perchè non tutte le tipologie di parentela hanno il secondo carattere
                    c.TipoUnione = codMagg.CodParentela.PadRight(2, ' ').Substring(1, 1);
                }

                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                    && this.domanda.CodGruppo.Equals("0031")
                    && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                {
                    c.QuotaAF = codMagg.QuotaAf;
                    c.ContitolaritaFondo = codMagg.ContitolaritaFondo;
                }

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                {
                    c.DirittoAF = codMagg.DirittoAf;
                    c.QuotaAF = codMagg.QuotaAf;
                    c.ContitolaritaFondo = codMagg.ContitolaritaFondo;
                    c.ContitolaritaAgo = codMagg.ContitolaritaAgo;
                }
                listCodiciMaggiorazione.Add(c);
            }

            ModifyFamiliare.areaFamiliare.ElencoCodMaggFamiliari = listCodiciMaggiorazione.ToArray();

            if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
            {
                if (ModifyFamiliare.areaFamiliare.ElencoCodMaggFamiliari != null && ModifyFamiliare.areaFamiliare.ElencoCodMaggFamiliari.Count() > 0)
                {
                    GestioneFamiliariCodMaggFamiliari lastCodMaggiorazione = ModifyFamiliare.areaFamiliare.ElencoCodMaggFamiliari.OrderByDescending(x => x.Decorrenza).FirstOrDefault();
                    if (lastCodMaggiorazione != null)
                    {
                        ModifyFamiliare.areaFamiliare.Familiare.SiglaFamiliare = lastCodMaggiorazione.SiglaFamiliare;
                        ModifyFamiliare.areaFamiliare.Familiare.TipoUnione = lastCodMaggiorazione.TipoUnione;
                    }
                }
            }
            else
            {
                ModifyFamiliare.areaFamiliare.Familiare.SiglaFamiliare = Char.Parse(DropParentela.SelectedValue.Substring(0, 1));
                // Effettuo il PadRight perchè non tutte le tipologie di parentela hanno il secondo carattere
                ModifyFamiliare.areaFamiliare.Familiare.TipoUnione = DropParentela.SelectedValue.PadRight(2, ' ').Substring(1, 1);
            }

            if (!txtRevSan.Text.Equals("MM/AAAA") && txtRevSan.Text != string.Empty)
                ModifyFamiliare.areaFamiliare.Familiare.ScadenzaRevisioneSanitaria = Utility.GetDateFromString(txtRevSan.Text);
            else
                ModifyFamiliare.areaFamiliare.Familiare.ScadenzaRevisioneSanitaria = null;

            if (lblDataMorteValue.Visible && lblDataMorteValue.Text != string.Empty)
                ModifyFamiliare.areaFamiliare.Familiare.DataMorte = Utility.GetDateFromString(lblDataMorteValue.Text);
            else
                ModifyFamiliare.areaFamiliare.Familiare.DataMorte = null;

            ModifyFamiliare.areaFamiliare.Familiare.Confermato = true;

            if ((this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI) && !ModifyFamiliare.areaFamiliare.Familiare.Progressivo.HasValue)
            {
                if (this.elencoFamiliari != null && this.elencoFamiliari.Count > 0)
                    ModifyFamiliare.areaFamiliare.Familiare.Progressivo = (char)((byte)this.elencoFamiliari.Max(x => x.areaFamiliare.Familiare.Progressivo.GetValueOrDefault()) + 1);
                else
                    ModifyFamiliare.areaFamiliare.Familiare.Progressivo = (char)0;
            }

            this.elencoFamiliari[indexFamiliaremodificato] = ModifyFamiliare;

            if (ViewState["UpdateArca"] != null)
            {
                this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];

                if (this.elencoFamiliari != null && this.elencoFamiliari.Count > 0)
                {
                    for (int i = 0; i < this.elencoFamiliari.Count; i++)
                    {
                        if (this.elencoFamiliari[i].anagrafica.CodiceFiscale.Trim().ToUpperInvariant() == ((Presenter.SvrLiquidazione.Anagrafica)ViewState["UpdateArca"]).CodiceFiscale.Trim().ToUpperInvariant())
                            this.elencoFamiliari[i].anagrafica = (Presenter.SvrLiquidazione.Anagrafica)ViewState["UpdateArca"];
                    }
                }
                ViewState.Remove("UpdateArca");
            }
            this.codiceFiscale = txtCFAltriFamiliari.Text;
        }

        private void RenderVisibleControls(GridViewRow row, bool btnSave, bool btnEdit, bool btnInsert, bool btnDelete)
        {
            ((LinkButton)(row.FindControl("btnSave"))).Visible = btnSave;
            ((LinkButton)(row.FindControl("btnEdit"))).Visible = btnEdit;
            ((LinkButton)(row.FindControl("btnAnnulla"))).Visible = btnSave;
            ((LinkButton)(row.FindControl("btnInsert"))).Visible = btnInsert;
            ((LinkButton)(row.FindControl("btnDelete"))).Visible = btnDelete;

            row.Cells[(int)GvCodMaggiorazioneColumns.LblParentela].Visible = btnDelete;
            row.Cells[(int)GvCodMaggiorazioneColumns.DdlParentela].Visible = btnSave;
            row.Cells[(int)GvCodMaggiorazioneColumns.LblMaggiorazione].Visible = btnDelete;
            row.Cells[(int)GvCodMaggiorazioneColumns.DdlCodMaggiorazione].Visible = btnSave;
            row.Cells[(int)GvCodMaggiorazioneColumns.LblDecorrenzaCarico].Visible = btnDelete;
            row.Cells[(int)GvCodMaggiorazioneColumns.TxtDecorrenzaCarico].Visible = btnSave;
            row.Cells[(int)GvCodMaggiorazioneColumns.LblFineCarico].Visible = btnDelete;
            row.Cells[(int)GvCodMaggiorazioneColumns.TxtFineCarico].Visible = btnSave;

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo = this.domanda.Tipofondo;
            if(tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || 
                tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL ||
                (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT && this.domanda.CodGruppo.Equals("0031")) ||
                (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && this.domanda.CodGruppo.Equals("0031")))
            {
                row.Cells[(int)GvCodMaggiorazioneColumns.LblQuotaAf].Visible = btnDelete;
                row.Cells[(int)GvCodMaggiorazioneColumns.DdlQuotaAf].Visible = btnSave;
                row.Cells[(int)GvCodMaggiorazioneColumns.LblContitolarietaFondo].Visible = btnDelete;
                row.Cells[(int)GvCodMaggiorazioneColumns.DdlContitolarietaFondo].Visible = btnSave;
            }

            row.Cells[(int)GvCodMaggiorazioneColumns.LblDirittoAf].Visible = (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL) && btnDelete;
            row.Cells[(int)GvCodMaggiorazioneColumns.DdlDirittoAf].Visible = (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL) && btnSave;
            row.Cells[(int)GvCodMaggiorazioneColumns.LblContitolarietaAgo].Visible = (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL) && btnDelete;
            row.Cells[(int)GvCodMaggiorazioneColumns.DdlContitolarietaAgo].Visible = (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL) && btnSave;
        }

        private void LoadDdl(DropDownList ddlSC, DropDownList ddlParentela)
        {
            try
            {
                ddlSC.Items.Clear();
                AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();

                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (Session["DatiPensione"] != null)
                    datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

                GestioneAreaFamiliariAreaDecFam datiDecodifica = (GestioneAreaFamiliariAreaDecFam)ViewState["DatiDecodifica"];

                string abilitazioneMemo33 = string.Empty;
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo33" + this.domanda.TipoAppartenenza, out abilitazioneMemo33);

                if (datiDecodifica != null && datiDecodifica.ElencoCodMaggFamiliari != null)
                {
                    foreach (GestioneAreaFamiliariAreaDecFam.DatiCodMaggFamiliari areadec in datiDecodifica.ElencoCodMaggFamiliari)
                    {
                        if (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                        {
                            if (Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                                Utility.IsDomandaAPESociale(this.domanda.Categoria) || Utility.IsDomandaAUT(this.domanda.Categoria) || Utility.IsDomandaSPED(this.domanda.Categoria) ||
                                (Utility.IsDomandaPescatori(this.domanda.Categoria) && !(CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))) ||
                                Utility.IsDomandaBancari(this.domanda.Categoria) || Utility.IsDomandaPSO(this.domanda.Categoria) || Utility.IsDomandaPMO(this.domanda.Categoria))
                            {
                                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(abilitazioneMemo33) && abilitazioneMemo33.Trim().ToUpperInvariant() == "SI"
                                    && (ddlParentela.SelectedValue == "N" || ddlParentela.SelectedValue == "J"))
                                    CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                                else if (areadec.Id == "0" && !Utility.IsDomandaSOAUT(this.domanda.Categoria))
                                    CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                                else if (areadec.Id == "1" && Utility.IsDomandaSOAUT(this.domanda.Categoria))
                                    CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                            }
                            else if (!String.IsNullOrEmpty(this.domanda.Categoria) && !this.domanda.Categoria.StartsWith("S"))
                            {
                                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK && !String.IsNullOrEmpty(abilitazioneMemo33) && abilitazioneMemo33.Trim().ToUpperInvariant() == "SI"
                                     && (ddlParentela.SelectedValue == "N" || ddlParentela.SelectedValue == "J"))
                                    CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                                else if ((ddlParentela.SelectedValue == "CM" || ddlParentela.SelectedValue == "CU") &&
                                    !Utility.IsDomandaINDCOM(this.domanda.Categoria) && !Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria) && !Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria))
                                {
                                    if (areadec.Id != "0")
                                        CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                                }
                                else
                                {
                                    if (areadec.Id == "0")
                                        CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                                }
                            }
                            else
                                CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                        }
                        else
                            CodeUtility.SetValueDdl(ddlSC, areadec.CampoVideo, areadec.Descrizione, areadec.Id);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GvCodiciMaggiorazione_Load()
        {
            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = (List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"];
            CodiciMaggiorazione Empty = elencoCodiciMaggiorazione.Find(delegate(CodiciMaggiorazione code)
            { return (string.IsNullOrEmpty(code.Acquisizione) && string.IsNullOrEmpty(code.Cessazione) && string.IsNullOrEmpty(code.CodMaggiorazione) && string.IsNullOrEmpty(code.Maggiorazione)) && string.IsNullOrEmpty(code.DescParentela) && string.IsNullOrEmpty(code.CodParentela); });

            if (Empty == null)
                elencoCodiciMaggiorazione.Add(new CodiciMaggiorazione(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            if ((bool)ViewState["ChangeIndexDDL"] && elencoCodiciMaggiorazione.Count != 1)
                RemoveItemBlank(ref elencoCodiciMaggiorazione);

            gvCodiciMaggiorazione.DataSource = elencoCodiciMaggiorazione;
            gvCodiciMaggiorazione.DataKeyNames = new string[] { "CodMaggiorazione" };
            gvCodiciMaggiorazione.DataBind();
        }

        private void RemoveItemBlank(ref List<CodiciMaggiorazione> lista)
        {
            int index = lista.FindIndex(delegate(CodiciMaggiorazione code)
            { return (string.IsNullOrEmpty(code.Acquisizione) && string.IsNullOrEmpty(code.Cessazione) && string.IsNullOrEmpty(code.CodMaggiorazione) && string.IsNullOrEmpty(code.Maggiorazione)); });

            if (index >= 0)
                lista.RemoveAt(index);
        }

        private void AddItemBlank(ref List<CodiciMaggiorazione> lista)
        {
            if (lista != null)
            {
                int index = lista.FindIndex(delegate(CodiciMaggiorazione code)
                {
                    return (string.IsNullOrEmpty(code.Acquisizione) && string.IsNullOrEmpty(code.Cessazione) && string.IsNullOrEmpty(code.CodMaggiorazione) && string.IsNullOrEmpty(code.Maggiorazione));
                }
                    );

                if (index < 0)
                {
                    lista.Add(new CodiciMaggiorazione());
                }
            }
        }

        private void SalvaNewFamiliare()
        {
            AreaEsito esito = FamiliariManager(true);
            if (!this.HasError)
            {
                ViewState["elencoCodiciMaggiorazione"] = new List<CodiciMaggiorazione>();
                ViewState["ToBeDeleted"] = new List<string>();

                pnlFamiliari.Visible = true;
                pnlSearch.Visible = false;
                Clearfields();
                btnAddFamiliare.Text = "Aggiungi Familiare";
                btnAddFamiliare.CssClass = AddCssClass(btnAddFamiliare.CssClass, "primary");
                if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault())
                    btnAddFamiliare.Width = Unit.Pixel(190);
                else
                    btnAddFamiliare.Width = Unit.Pixel(150);
                btnSalva.Visible = false;
                btnEliminaFamiliari.Visible = true;
                this.areaEsito = esito;
                RaiseSalvafamiliari(this, null);
                DataBind();
            }
            else
            {
                pnlFamiliari.Visible = false;
                pnlSearch.Visible = true;
                RaiseFamiliariNonSalvati(esito, null);
            }
        }

        private void UpdateFamiliare()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaEsito esito = FamiliariManager(false);

            if (!this.HasError)
            {
                btnAddFamiliare.Text = "Aggiungi Familiare";
                btnAddFamiliare.CssClass = AddCssClass(btnAddFamiliare.CssClass, "primary");
                if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault())
                    btnAddFamiliare.Width = Unit.Pixel(190);
                else
                    btnAddFamiliare.Width = Unit.Pixel(150);
                btnAddFamiliare.Visible = true;
                btnSalva.Visible = false;
                btnEliminaFamiliari.Visible = true;
                txtCFAltriFamiliari.Enabled = true;
                btnUpdateArca.Visible = false;
                txtRevSan.Text = "MM/AAAA";
                //txtDataMorte.Text = "GG/MM/AAAA";
                pnlTopFamiliari.Visible = true;
                pnlFamiliari.Visible = true;
                pnlSearch.Visible = false;
                ExtraInfo.Visible = false;
                DropParentela.ClearSelection();
                this.areaEsito = esito;
                RaiseSalvafamiliari(this, null);
                ViewState["ToBeDeleted"] = new List<string>();

                DataBind();
            }
            else
            {
                this.btnAddFamiliare.Text = "Annulla Modifica Familiare";
                btnAddFamiliare.CssClass = RemoveCssClass(btnAddFamiliare.CssClass, "primary");
                if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault())
                    btnAddFamiliare.Width = Unit.Pixel(208);
                else
                    btnAddFamiliare.Width = Unit.Pixel(168);
                pnlFamiliari.Visible = false;
                pnlSearch.Visible = true;
                if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                {
                    ExtraInfo.Visible = true;
                }
                btnSalva.Visible = true;
                btnEliminaFamiliari.Visible = false;
                txtCFAltriFamiliari.Enabled = false;
                imgCercaAltriFamiliari.Visible = false;
                btnUpdateArca.Visible = true;
                RaiseFamiliariNonSalvati(esito, null);
            }

            GestioneVariazioneDatiContitolari();
        }

        private void DeleteFamiliare()
        {
            AreaEsito esito = FamiliariManager(false);
            if (!this.HasError)
            {
                this.areaEsito = esito;
                RaiseSalvafamiliari(this, null);
            }
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                RaiseFamiliariNonSalvati(esito, null);
            }

            ViewState["ToBeDeleted"] = new List<string>();
            DataBind();
        }

        private AreaEsito FamiliariManager(bool IsNewFamiliare)
        {
            this.familiariToDelete = (List<string>)ViewState["ToBeDeleted"];
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsNewFamiliare)
                this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
            else
                this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliariDuplicate"];

            PresenterFamiliari Familiari = new PresenterFamiliari();
            AreaEsito esito = Familiari.SalvaFamiliari(this, this);
            ViewState["ListFamiliari"] = this.elencoFamiliari;
            return esito;
        }

        private void InizializzaDataCarico(TextBox txtDecCarico, TextBox txtFineCarico, DropDownList ddlParentela)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (Session["DatiPensione"] != null)
            {
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                if ((datiPensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione) && !this.domanda.IsDomandaRiapertura)
                {
                    txtDecCarico.Text = datiPensione.DecorrenzaOriginaria.HasValue ? String.Format("{0:MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value) : string.Empty;

                    //Figlio Minore, precompilare la 'Data Fine Carico' con la data di compimento del suo 18° compleanno
                    if (ddlParentela.SelectedValue == "M")
                    {
                        DateTime? dataMaggiorenne = null;
                        // Se sono in modifica il ViewState["isModify"] è true, mentre se sono in aggiunta il ViewState["isModify"] è null
                        if (((bool?)ViewState["isModify"]).GetValueOrDefault())
                        {
                            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
                            string idAnagrafica = (string)ViewState["Idanagraficamod"];
                            if (!string.IsNullOrEmpty(idAnagrafica))
                            {
                                long id;
                                long.TryParse(idAnagrafica, out id);
                                PresenterFamiliari.FamiliareFull familiare = this.elencoFamiliari.Find(x => x.anagrafica.Id == id);
                                dataMaggiorenne = familiare != null ? new DateTime(familiare.anagrafica.DataNascita.Value.AddYears(18).AddMonths(1).Year, familiare.anagrafica.DataNascita.Value.AddYears(18).AddMonths(1).Month, 1) : (DateTime?)null;
                                txtFineCarico.Text = dataMaggiorenne.HasValue ? String.Format("{0:MM/yyyy}", dataMaggiorenne.Value) : string.Empty;
                            }
                        }
                        else
                        {
                            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica familiare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)ViewState["newfamiliare"];
                            dataMaggiorenne = familiare != null ? new DateTime(familiare.DataNascita.Value.AddYears(18).AddMonths(1).Year, familiare.DataNascita.Value.AddYears(18).AddMonths(1).Month, 1) : (DateTime?)null;
                            txtFineCarico.Text = dataMaggiorenne.HasValue ? String.Format("{0:MM/yyyy}", dataMaggiorenne.Value) : string.Empty;
                        }

                        if (datiPensione.DecorrenzaOriginaria.HasValue && dataMaggiorenne.HasValue && dataMaggiorenne.Value < datiPensione.DecorrenzaOriginaria.Value)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione: Alla decorrenza pensione il grado di parentela del familiare acquisito non è congruo.";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }
            }
        }

        private void InizializzaCodiciMaggiorazioni(DropDownList ddlCodMagg, DropDownList ddlParentela)
        {
            if (Session["DatiPensione"] != null)
            {
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

                if (datiPensione.TipoAppartenenzaDomanda.HasValue && datiPensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                {
                    if (ddlParentela.SelectedValue == "CM" || ddlParentela.SelectedValue == "CU")
                        ddlCodMagg.SelectedIndex = 0; //NO
                    //else
                    //    ddlCodMagg.SelectedIndex = 1; //SI
                }
            }
        }

        private void CFsearch(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = (List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"];
            elencoCodiciMaggiorazione.Clear();

            PresenterFamiliari PresenterFamiliari = new PresenterFamiliari();

            try
            {
                this.codiceFiscale = txtCFAltriFamiliari.Text;
                PresenterFamiliari.RicercaAnagraficaByCodiceFiscale(this);
                if (this.areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    HasError = true;
                    ErrorMessage = this.areaEsito.Messaggio;
                    AreaEsito esito = new AreaEsito();
                    esito.Messaggio = ErrorMessage;
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    RaiseFamiliariNonSalvati(esito, e);
                    Clearfields();
                    ViewState["newfamiliare"] = "";
                    DropParentela.Enabled = false;
                }
                else if (this.areaRiepilogoAnagrafica != null)
                {
                    hidenCF.Text = this.areaRiepilogoAnagrafica.CodiceFiscale;
                    Lbcognome.Text = this.areaRiepilogoAnagrafica.Cognome;
                    LbNome.Text = this.areaRiepilogoAnagrafica.Nome;
                    lbCognAcquisito.Text = this.areaRiepilogoAnagrafica.CognomeAcquisito;
                    LbSesso.Text = this.areaRiepilogoAnagrafica.Sesso.ToString();
                    String data = this.areaRiepilogoAnagrafica.DataNascita.ToString();
                    LbDataDiNascita.Text = data.Substring(0, data.LastIndexOf(@"/", StringComparison.CurrentCulture) + 5);
                    LbComunedinascita.Text = this.areaRiepilogoAnagrafica.ComuneNascita;
                    LbProvinciadinascita.Text = this.areaRiepilogoAnagrafica.ProvinciaNascita;
                    if (this.areaRiepilogoAnagrafica.DataMorte.HasValue)
                    {
                        pnlDataMorte.Visible = true;
                        lblDataMorteValue.Text = String.Format("{0:dd/MM/yyyy}", this.areaRiepilogoAnagrafica.DataMorte.Value);
                    }
                    else
                        pnlDataMorte.Visible = false;

                    if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                    {
                        ManageParentelaAndCodMaggiorazioneForAGOAndCI();
                    }
                    else
                    {
                        CaricaDropParentela(DropParentela, false);
                        DropParentela.SelectedValue = string.Empty;
                        DropParentela.Enabled = true;
                    }

                    if (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) ||
                        Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                        Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaINDCOM(this.domanda.Categoria) ||
                        Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) ||
                        Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                    {
                        txtRevSan.Text = string.Empty;
                        txtRevSan.Enabled = false;
                    }

                    ViewState["newfamiliare"] = this.areaRiepilogoAnagrafica;
                }
                else
                {
                    AreaEsito esito = new AreaEsito();
                    esito.Messaggio = "Nessuna Corrispondenza trovata";
                    esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                    RaiseFamiliariNonSalvati(esito, e);
                    Clearfields();
                    ViewState["newfamiliare"] = string.Empty;
                    DropParentela.Enabled = false;
                }
            }
            catch (DNA.DnaApplicationException)
            {
                Clearfields();
                ViewState["newfamiliare"] = string.Empty;
                throw;
            }
        }

        private static Comparison<GestioneFamiliariCodMaggFamiliari> sortCodMaggDec = delegate(GestioneFamiliariCodMaggFamiliari d1, GestioneFamiliariCodMaggFamiliari d2)
        {
            try
            {
                int retValue = int.MinValue;
                retValue = d1.Decorrenza.Value.CompareTo(d2.Decorrenza.Value);

                return retValue;
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Errore nel metodo sortCodMagg" + ex);
            }
        };

        private static Comparison<GestioneFamiliariCodMaggFamiliari> sortCodMaggCes = delegate(GestioneFamiliariCodMaggFamiliari d1, GestioneFamiliariCodMaggFamiliari d2)
        {
            try
            {
                int retValue = int.MinValue;
                if (d1.Cessazione.HasValue && d2.Cessazione.HasValue)
                    retValue = d2.Cessazione.Value.CompareTo(d1.Cessazione.Value);
                else
                {
                    if (d1.Cessazione.HasValue && !d2.Cessazione.HasValue)
                        retValue = -1;
                    else if (!d1.Cessazione.HasValue && d2.Cessazione.HasValue)
                        retValue = 1;
                    else
                        retValue = 0;
                }
                return retValue;
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Errore nel metodo sortCodMagg" + ex);
            }
        };


        #endregion private methods

        #region Grid Familiari

        protected void ViewFamiliari_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
            hfRowIndex.Value = Convert.ToInt16(e.CommandArgument).ToString();

            if (e.CommandName == "delfam")
            {
                string Idanagrafica = ViewFamiliari.DataKeys[Convert.ToInt16(e.CommandArgument)].Value.ToString();

                List<string> listCF = (List<string>)ViewState["ToBeDeleted"];
                if (this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.CodiceFiscale != string.Empty)
                    listCF.Add(this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.CodiceFiscale);
                else
                    listCF.Add(this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.Id.ToString());

                PresenterFamiliari.FamiliareFull famDaEliminare = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)];
                this.elencoFamiliari.RemoveAt(Convert.ToInt16(e.CommandArgument));
                ViewState["ListFamiliari"] = this.elencoFamiliari;
                DeleteFamiliare();

                if (ViewState["ToBeDeleted"] != null && ((List<string>)ViewState["ToBeDeleted"]).Count > 0)
                {
                    if (this.elencoFamiliari == null)
                        this.elencoFamiliari = new List<PresenterFamiliari.FamiliareFull>();
                    ViewState["ListFamiliari"] = this.elencoFamiliari;
                }
                DataBind();
            }
            if (e.CommandName == "modfam")
            {
                string Idanagrafica = ViewFamiliari.DataKeys[Convert.ToInt16(e.CommandArgument)].Value.ToString();
                ViewState["Idanagraficamod"] = Idanagrafica;

                txtCFAltriFamiliari.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.CodiceFiscale;
                Lbcognome.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.Cognome;
                LbNome.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.Nome;
                lbCognAcquisito.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.CognomeAcquisito;
                LbSesso.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.Sesso.ToString();
                LbComunedinascita.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.ComuneNascita;
                LbDataDiNascita.Text = String.Format("{0:dd/MM/yyyy}", this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.DataNascita);
                LbProvinciadinascita.Text = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].anagrafica.ProvinciaNascita;
                if (this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.DataMorte.HasValue)
                {
                    lblDataMorteValue.Text = String.Format("{0:dd/MM/yyyy}", this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.DataMorte);
                    pnlDataMorte.Visible = true;
                }
                else
                {
                    pnlDataMorte.Visible = false;
                }
                if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                {
                    trParentela.Visible = false;
                    ExtraInfo.Visible = true;
                }
                else
                {
                    CaricaDropParentela(DropParentela, false);
                    if (DropParentela.Items.FindByValue(this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.SiglaFamiliare.ToString() +
                                                    this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.TipoUnione.Trim()) != null)
                        DropParentela.SelectedValue = this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.SiglaFamiliare.ToString() +
                                                    this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.TipoUnione.Trim();
                    ManageDdlParentelaByDatiFamiliare(DropParentela, this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare);
                    if (DropParentela.SelectedValue == "")
                        ExtraInfo.Visible = false;
                    else ExtraInfo.Visible = true;
                }
                List<CodiciMaggiorazione> elencoCodiciMaggiorazione = new List<CodiciMaggiorazione>();
                if (this.areaDecodifica == null)
                    this.areaDecodifica = (GestioneAreaFamiliariAreaDecFam)ViewState["DatiDecodifica"];

                if (this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.ElencoCodMaggFamiliari != null && this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.ElencoCodMaggFamiliari.Count() > 0)
                {
                    foreach (GestioneFamiliariCodMaggFamiliari CodMaggFamiliari in this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.ElencoCodMaggFamiliari)
                    {
                        CodiciMaggiorazione c = new CodiciMaggiorazione();
                        c.Acquisizione = String.Format("{0:MM/yyyy}", CodMaggFamiliari.Decorrenza);
                        c.Cessazione = String.Format("{0:MM/yyyy}", CodMaggFamiliari.Cessazione);

                        c.CodMaggiorazione = CodMaggFamiliari.CodiceMaggiorazione.ToString();
                        GestioneAreaFamiliariAreaDecFam.DatiCodMaggFamiliari codMagg = null;

                        if (this.areaDecodifica != null && this.areaDecodifica.ElencoCodMaggFamiliari != null && this.areaDecodifica.ElencoCodMaggFamiliari.Count() > 0)
                            codMagg = this.areaDecodifica.ElencoCodMaggFamiliari.ToList().Find(x => x.Id == CodMaggFamiliari.CodiceMaggiorazione.ToString());

                        if (codMagg != null)
                            c.Maggiorazione = codMagg.CampoVideo;
                        if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                        {
                            GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare siglaFamiliare = this.areaDecodifica.ElencoSiglaFamiliare.ToList().FirstOrDefault(x => x.Id == CodMaggFamiliari.SiglaFamiliare.GetValueOrDefault().ToString() && x.TipoUnione == CodMaggFamiliari.TipoUnione.Trim());
                            if (siglaFamiliare != null)
                            {
                                c.DescParentela = siglaFamiliare.Id + " - " + siglaFamiliare.Descrizione;
                                c.CodParentela = siglaFamiliare.Id + siglaFamiliare.TipoUnione;
                            }
                        }

                        AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoaDomanda = this.domanda.Tipofondo;
                        if (tipoaDomanda.HasValue && (tipoaDomanda == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL || tipoaDomanda == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI))
                        {
                            c.DirittoAf = CodMaggFamiliari.DirittoAF;
                            c.QuotaAf = CodMaggFamiliari.QuotaAF;
                            c.ContitolaritaFondo = CodMaggFamiliari.ContitolaritaFondo;
                            c.ContitolaritaAgo = CodMaggFamiliari.ContitolaritaAgo;
                            string acquisizione = c.Acquisizione;
                            if(acquisizione == null || acquisizione.Length == 0 || acquisizione.Trim().Length == 0)
                                c.Acquisizione = this.areaDanteCausa.AnagraficaDC.DataMorte.HasValue ? String.Format("{0:MM/yyyy}", this.areaDanteCausa.AnagraficaDC.DataMorte.Value.AddMonths(1)) : null;
                        }

                        if((tipoaDomanda == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || tipoaDomanda == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                            && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS
                            && this.domanda.CodGruppo.Equals("0031"))
                        {
                            c.QuotaAf = CodMaggFamiliari.QuotaAF;
                            c.ContitolaritaFondo = CodMaggFamiliari.ContitolaritaFondo;
                        }

                        elencoCodiciMaggiorazione.Add(c);
                    }
                }
                this.btnAddFamiliare.Text = "Annulla Modifica Familiare";
                btnAddFamiliare.CssClass = RemoveCssClass(btnAddFamiliare.CssClass, "primary");
                if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault())
                    btnAddFamiliare.Width = Unit.Pixel(208);
                else
                    btnAddFamiliare.Width = Unit.Pixel(168);
                pnlFamiliari.Visible = false;
                pnlSearch.Visible = true;
                btnSalva.Visible = true;
                btnEliminaFamiliari.Visible = false;
                txtCFAltriFamiliari.Enabled = false;
                imgCercaAltriFamiliari.Visible = false;
                btnUpdateArca.Visible = true;

                ViewState["isModify"] = true;
                ViewState["ChangeIndexDDL"] = false;
                ViewState["InsertNewRecord"] = false;
                ViewState["elencoCodiciMaggiorazione"] = elencoCodiciMaggiorazione;

                GvCodiciMaggiorazione_Load();

                txtRevSan.Text = "MM/AAAA";
                GestioneInfoAgoCi_Edit();

                if (rowAgoCi.Visible)
                {
                    if (this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.ScadenzaRevisioneSanitaria != null)
                        txtRevSan.Text = String.Format("{0:MM/yyyy}", this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.ScadenzaRevisioneSanitaria);
                    if (this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.DataMorte != null)
                    {
                        pnlDataMorte.Visible = true;
                        lblDataMorteValue.Text = String.Format("{0:dd/MM/yyyy}", this.elencoFamiliari[Convert.ToInt16(e.CommandArgument)].areaFamiliare.Familiare.DataMorte);
                    }
                    else
                    {
                        pnlDataMorte.Visible = false;
                        lblDataMorteValue.Text = string.Empty;
                    }
                }

                if (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) ||
                    Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                    Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaINDCOM(this.domanda.Categoria) ||
                    Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) ||
                    Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                {
                    txtRevSan.Text = string.Empty;
                    txtRevSan.Enabled = false;
                }

            }
            managerBtnSalva();
        }

        private void ManageDdlParentelaByDatiFamiliare(DropDownList ddlParentela, GestioneFamiliariFamiliare familiare)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagraficaTitolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            // Per le pensioni ai superstiti (unico caso in cui è ammesso il titolare tra i familiari) il grado di parentela deve essere bloccato con quello che ci arriva da Webdom
            if (familiare != null && ((anagraficaTitolare != null && anagraficaTitolare.CodiceFiscale == familiare.CodiceFiscale && familiare.Provenienza == 'W' && !string.IsNullOrEmpty(ddlParentela.SelectedValue)) ||
                (familiare.Provenienza == 'P' && familiare.SiglaFamiliare == 'C') || (familiare.Provenienza == 'W' && familiare.SiglaFamiliare == 'O')))
                ddlParentela.Enabled = false;
            else
                ddlParentela.Enabled = true;
        }

        protected void ViewFamiliari_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagraficaTitolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];

            if (e.Row.RowIndex >= 0 && this.elencoFamiliari != null && this.elencoFamiliari[e.Row.RowIndex] != null)
            {
                string siglaFamiliare = this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.SiglaFamiliare.ToString();
                string tipoUnione = this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.TipoUnione.Trim();
                if (this.areaDecodifica == null)
                    this.areaDecodifica = (GestioneAreaFamiliariAreaDecFam)ViewState["DatiDecodifica"];

                List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> listaGradoFamiliare = this.areaDecodifica.ElencoSiglaFamiliare.ToList();
                if (listaGradoFamiliare != null && listaGradoFamiliare.Count > 0 && listaGradoFamiliare.Exists(x => x.Id == siglaFamiliare && x.TipoUnione == tipoUnione))
                {
                    GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare parentela = listaGradoFamiliare.FirstOrDefault(x => x.Id == siglaFamiliare && x.TipoUnione == tipoUnione);
                    if (parentela != null)
                    {
                        string selectedValueGradoParentela = parentela.Id + " - " + parentela.Descrizione;
                        if (e.Row.Cells[4] != null)
                            e.Row.Cells[4].Controls.Add(new LiteralControl(selectedValueGradoParentela));
                    }
                }
                string provenienza = "OPERATORE";
                bool isEnabled = true;
                if (this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.Provenienza.HasValue && !Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione))
                {
                    switch (this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.Provenienza.Value)
                    {
                        case 'W':
                            provenienza = "WEBDOM";
                            if (anagraficaTitolare != null && anagraficaTitolare.CodiceFiscale == this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.CodiceFiscale)
                                isEnabled = false;
                            break;
                        case 'P':
                            provenienza = "PRELIEVO";
                            isEnabled = false;
                            btnEliminaFamiliari.Enabled = isEnabled;
                            break;
                        default:
                            break;
                    }
                }
                if (anagraficaTitolare != null && anagraficaTitolare.CodiceFiscale == this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.CodiceFiscale &&
                    (Utility.IsDomandaSpacchettamentoENPALS(this.domanda.IsDomandaENPALS, this.domanda.Categoria) || Utility.IsDomandaSpacchettamentoINPDAP(this.domanda.IsDomandaINPDAP, this.domanda.Categoria)
                    || (ViewState["AbilitazioneSpacchettamento024"] != null && ViewState["AbilitazioneSpacchettamento024"].ToString().ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione))
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa)
                    || Utility.IsDomandaSpacchettamentoSO(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, this.domanda)
                    || Utility.IsDomandaSpacchettamentoSR(datiPensione, this.domanda)))
                    isEnabled = false;

                Image img = (Image)e.Row.FindControl("img");
                string currentTheme = Page.Theme;

                if (this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.Confermato)
                {
                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                    img.ToolTip = "Salvato";
                }
                else if (!this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.Confermato)
                {
                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                    img.ToolTip = "Non Salvato";
                }

                if (e.Row.Cells[5] != null)
                    e.Row.Cells[5].Controls.Add(new LiteralControl(provenienza));

                string dec = string.Empty;
                string cess = string.Empty;

                if (this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.ElencoCodMaggFamiliari != null && this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.ElencoCodMaggFamiliari.Count() > 0)
                {
                    List<GestioneFamiliariCodMaggFamiliari> list = this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.ElencoCodMaggFamiliari.ToList();

                    if (list != null && list.Count > 0)
                    {
                        int index = 0;
                        if (this.domanda.TipoAppartenenza != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                            index = list.Count - 1;
                        list.Sort(sortCodMaggDec);
                        dec = String.Format("{0:MM/yyyy}", list[index].Decorrenza);

                        if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                            list.Sort(sortCodMaggCes);
                        cess = String.Format("{0:MM/yyyy}", list[index].Cessazione);
                    }
                }
                if (e.Row.Cells[6] != null)
                    e.Row.Cells[6].Controls.Add(new LiteralControl(dec));
                if (e.Row.Cells[7] != null)
                    e.Row.Cells[7].Controls.Add(new LiteralControl(cess));

                if (((Button)(e.Row.FindControl("btnelimina"))) != null)
                    ((Button)(e.Row.FindControl("btnelimina"))).Enabled = isEnabled;

                if (this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.TipoComponente == 'T' && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                    (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura))
                    if (((Button)(e.Row.FindControl("btnmodifica"))) != null)
                        ((Button)(e.Row.FindControl("btnmodifica"))).Enabled = false;

                if (anagraficaTitolare.CodiceFiscale == this.elencoFamiliari[e.Row.RowIndex].areaFamiliare.Familiare.CodiceFiscale &&
                    (Utility.IsDomandaSpacchettamentoENPALS(this.domanda.IsDomandaENPALS, this.domanda.Categoria) || Utility.IsDomandaSpacchettamentoINPDAP(this.domanda.IsDomandaINPDAP, this.domanda.Categoria) ||
                    Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa) ||
                    (ViewState["AbilitazioneSpacchettamento024"] != null && ViewState["AbilitazioneSpacchettamento024"].ToString().ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione))
                    || Utility.IsDomandaSpacchettamentoSO(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, this.domanda)
                    || Utility.IsDomandaSpacchettamentoSR(datiPensione, this.domanda)) &&
                    !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    e.Row.Visible = false;
            }
        }

        protected void ViewFamiliari_DataBinding(object sender, EventArgs e)
        {
            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.elencoFamiliari != null)
            {
                List<Presenter.SvrLiquidazione.Anagrafica> ListAnagrafica = new List<Anagrafica>();
                foreach (PresenterFamiliari.FamiliareFull fam in this.elencoFamiliari)
                {
                    //if (!(Utility.IsDomandaSpacchettamentoENPALS(this.domanda.IsDomandaENPALS, this.domanda.Categoria) && fam.areaFamiliare.Familiare.TipoComponente == 'T'))
                    ListAnagrafica.Add(fam.anagrafica);
                }


                ViewFamiliari.DataSource = ListAnagrafica;
            }
        }

        #endregion Grid Familiari

        #region Grid CodiciMaggiorazione

        protected void gvCodiciMaggiorazione_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                this.areaDecodifica = ViewState["DatiDecodifica"] as GestioneAreaFamiliariAreaDecFam;
                if (e.Row.DataItemIndex == ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"])).Count - 1)
                {
                    DropDownList ddlParentelaApp = null;
                    if (((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"])).Count == 1 &&
                        ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"]))[0].DescParentela == string.Empty &&
                        ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"]))[0].Acquisizione == string.Empty &&
                        ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"]))[0].Cessazione == string.Empty &&
                        ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"]))[0].CodMaggiorazione == string.Empty &&
                        ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"]))[0].Maggiorazione == string.Empty)
                    {
                        RenderVisibleControls(e.Row, true, false, false, false);
                        if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                        {
                            ddlParentelaApp = (DropDownList)e.Row.FindControl("ddlParentela");
                            CaricaDropParentela(ddlParentelaApp, false);
                            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
                            string idAnagrafica = (string)ViewState["Idanagraficamod"];
                            PresenterFamiliari.FamiliareFull familiare = null;
                            if (!string.IsNullOrEmpty(idAnagrafica))
                            {
                                long id;
                                long.TryParse(idAnagrafica, out id);
                                familiare = this.elencoFamiliari.Find(x => x.anagrafica.Id == id);
                            }
                            if (familiare != null && familiare.areaFamiliare != null && familiare.areaFamiliare.Familiare != null && familiare.areaFamiliare.Familiare.SiglaFamiliare.HasValue)
                            {
                                ddlParentelaApp.Items.FindByValue(familiare.areaFamiliare.Familiare.SiglaFamiliare.ToString() + familiare.areaFamiliare.Familiare.TipoUnione.Trim()).Selected = true;
                                if (familiare.areaFamiliare.Familiare.Provenienza.HasValue && familiare.areaFamiliare.Familiare.Provenienza == 'W' && familiare.areaFamiliare.Familiare.SiglaFamiliare == 'O')
                                {
                                    ddlParentelaApp.Enabled = false;
                                }
                                else
                                {
                                    ddlParentelaApp.Enabled = familiare.areaFamiliare.Familiare.SiglaFamiliare != 'C';
                                   
                                }
                                LoadDdl((DropDownList)e.Row.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                                InizializzaDataCarico((TextBox)e.Row.FindControl("txtDecorrenzaCarico"), (TextBox)e.Row.FindControl("txtFineCarico"), ddlParentelaApp);
                                InizializzaCodiciMaggiorazioni((DropDownList)e.Row.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                            }
                            else
                            {
                                ((DropDownList)e.Row.FindControl("ddlCodMaggiorazione")).Enabled = false;
                                ((TextBox)e.Row.FindControl("txtDecorrenzaCarico")).Enabled = false;
                                ((TextBox)e.Row.FindControl("txtFineCarico")).Enabled = false;
                            }
                        }
                        else
                        {
                            ddlParentelaApp = DropParentela;
                            LoadDdl((DropDownList)e.Row.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                            InizializzaDataCarico((TextBox)e.Row.FindControl("txtDecorrenzaCarico"), (TextBox)e.Row.FindControl("txtFineCarico"), ddlParentelaApp);
                            InizializzaCodiciMaggiorazioni((DropDownList)e.Row.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                        }
                    }
                    else
                    {
                        if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS &&
                            (bool)ViewState["ChangeIndexDDL"])
                        {
                            ddlParentelaApp = DropParentela;
                            LoadDdl((DropDownList)e.Row.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                            //if (((Label)(e.Row.FindControl("lblCodMaggiorazione"))).Text != string.Empty)
                            //    ((DropDownList)e.Row.FindControl("ddlCodMaggiorazione")).Items.FindByValue(((Label)(e.Row.FindControl("lblCodMaggiorazione"))).Text).Selected = true;
                            InizializzaDataCarico((TextBox)e.Row.FindControl("txtDecorrenzaCarico"), (TextBox)e.Row.FindControl("txtFineCarico"), ddlParentelaApp);
                            InizializzaCodiciMaggiorazioni((DropDownList)e.Row.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                            RenderVisibleControls(e.Row, true, false, false, false);
                        }
                        else
                            RenderVisibleControls(e.Row, false, false, true, true);

                        ((LinkButton)(e.Row.FindControl("btnDelete"))).Text = string.Empty;
                    }
                }
                else
                    RenderVisibleControls(e.Row, false, true, false, true);


                //ENG - RIC VARIAZIONE DATI CONTITOLARI
                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && Utility.IsRicostituzioneVariazioneDatiContitolari(datipensione))
                {
                    if (this.TitolarePensione != null && this.TitolarePensione.Anagrafica != null && !String.IsNullOrEmpty(this.TitolarePensione.Anagrafica.CodiceFiscale))
                    {
                        if (ViewState["ListFamiliari"] != null && ((List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"]).Exists(x => x.anagrafica != null && x.anagrafica.CodiceFiscale == this.TitolarePensione.Anagrafica.CodiceFiscale))
                        {
                            GestioneAreaFamiliariAreaFamiliare soggettoPresentazioneDomanda = ((List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"]).FindAll(x => x.anagrafica != null && x.anagrafica.CodiceFiscale == this.TitolarePensione.Anagrafica.CodiceFiscale).First().areaFamiliare;
                            if (soggettoPresentazioneDomanda != null && soggettoPresentazioneDomanda.Familiare != null)
                            {
                                long idAnagraficaSoggettoPresentazioneDomanda = soggettoPresentazioneDomanda.Familiare.IdAnagrafica;
                                bool isTitolarePensione = soggettoPresentazioneDomanda.Familiare.FlagTitolare.GetValueOrDefault();
                                if (!isTitolarePensione) //Se il soggetto che ha presentato la domanda non è il titolare della pensione allora gli altri familiari non possono essere modificati
                                {
                                    if (ViewState["Idanagraficamod"] != null && ViewState["Idanagraficamod"].ToString() != idAnagraficaSoggettoPresentazioneDomanda.ToString())
                                    {
                                        txtRevSan.Enabled = false;
                                        ((LinkButton)(e.Row.FindControl("btnSave"))).Visible = false;
                                        ((LinkButton)(e.Row.FindControl("btnEdit"))).Visible = false;
                                        ((LinkButton)(e.Row.FindControl("btnAnnulla"))).Visible = false;
                                        ((LinkButton)(e.Row.FindControl("btnInsert"))).Visible = false;
                                        ((LinkButton)(e.Row.FindControl("btnDelete"))).Visible = false;

                                    }
                                    else
                                    {
                                        txtRevSan.Enabled = true;
                                    }

                                    if (btnAddFamiliare.Text.Equals("Annulla Modifica Familiare"))
                                        btnAddFamiliare.Enabled = true;
                                }
                            }
                        }

                    }
                }

            }
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.DdlParentela].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.DdlCodMaggiorazione].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.TxtDecorrenzaCarico].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.TxtFineCarico].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.DdlDirittoAf].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.DdlQuotaAf].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.DdlContitolarietaAgo].Visible = false;
                e.Row.Cells[(int)GvCodMaggiorazioneColumns.DdlContitolarietaFondo].Visible = false;
            }
        }

        protected void gvCodiciMaggiorazione_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

            CustomValidator validateTxtDecorrenzaCarico = new CustomValidator();
            CustomValidator validateTxtFineCarico = (r.FindControl("CustomValidator5")) as CustomValidator;
            CustomValidator txtdecorrenzaValidorigi = new CustomValidator();

            ViewState["ChangeIndexDDL"] = false;
            ViewState["InsertNewRecord"] = false;

            //Rimuovo, se presente, il record vuoto e successivamente lo aggiungo. Implementata tale gestione per risolvere il problema nel caso in cui, al momento del savataggio, viene restituito un messaggio
            //di errore dal servizio e successivamente si prova a prendere in edit un record del grid. Vedi mail del 19-07-2013 ReEng - comportamento anomalo griglia familiari
            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"]));
            RemoveItemBlank(ref elencoCodiciMaggiorazione);
            AddItemBlank(ref elencoCodiciMaggiorazione);

            if (e.CommandName == "Save")
            {
                int indexRow = r.RowIndex;
                GridViewRow rApp = ((GridView)sender).Rows[indexRow];
                AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

                DropDownList ddlParentelaApp = this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS ? (DropDownList)r.FindControl("ddlParentela") : DropParentela;
                if (ddlParentelaApp != null && ddlParentelaApp.SelectedValue != "CM" && ddlParentelaApp.SelectedValue != "CU" && ddlParentelaApp.SelectedValue != "I" && ddlParentelaApp.SelectedValue != "G" && ddlParentelaApp.SelectedValue != "F")
                {
                    if((this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && !(ddlParentelaApp.SelectedValue == "A" && this.domanda.Categoria.StartsWith("S"))) && !(ddlParentelaApp.SelectedValue == "O" && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS || this.domanda.IsDomandaINPDAP)))
                    {
                        if (((TextBox)(r.FindControl("txtFineCarico"))).Text == string.Empty)
                        {
                            validateTxtFineCarico.IsValid = false;
                            return;
                        }
                    }
                    if (((TextBox)(r.FindControl("txtDecorrenzaCarico"))).Text != string.Empty)
                    {
                        string acquisizioneApp = string.Empty;
                        string cessazioneApp = string.Empty;
                        string codeApp = string.Empty;
                        string maggiorazioneApp = string.Empty;
                        string descParentela = string.Empty;
                        string codParentela = string.Empty;
                        string dirittoAf = string.Empty;
                        string quotaAf = string.Empty;
                        string contitolaritaFondo = string.Empty;
                        string contabilitàAgo = string.Empty;

                        if (rApp.Cells[(int)GvCodMaggiorazioneColumns.TxtDecorrenzaCarico].Visible)
                            acquisizioneApp = ((TextBox)(rApp.FindControl("txtDecorrenzaCarico"))).Text;
                        else
                            acquisizioneApp = ((Label)(rApp.FindControl("lblAcqusizione"))).Text;

                        if (rApp.Cells[(int)GvCodMaggiorazioneColumns.TxtFineCarico].Visible)
                            cessazioneApp = ((TextBox)(rApp.FindControl("txtFineCarico"))).Text;
                        else
                            cessazioneApp = ((Label)(rApp.FindControl("lblCessazione"))).Text;

                        dirittoAf = rApp.Cells[(int)GvCodMaggiorazioneColumns.DdlDirittoAf].Visible ? ((DropDownList)rApp.FindControl("ddlDirittoAf")).SelectedValue : ((Label)(rApp.FindControl("lblDirittoAf"))).Text;

                        //Qui di devono leggere i valori (QuotaAf e ContitolaritaFondo) per le ricostituzioni SFS/SPT della linea FS
                        if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                            && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS
                            && this.domanda.CodGruppo.Equals("0031"))
                        {
                            quotaAf = ((DropDownList)rApp.FindControl("ddlQuotaAf")).SelectedValue.Equals("SI") ? "S" : "N";
                            contitolaritaFondo = ((DropDownList)rApp.FindControl("ddlContitolarietaFondo")).SelectedValue.Equals("SI") ? "S" : "N";
                        }
                        else
                        {
                            quotaAf = rApp.Cells[(int)GvCodMaggiorazioneColumns.DdlQuotaAf].Visible ? ((DropDownList)rApp.FindControl("ddlQuotaAf")).SelectedValue : ((Label)(rApp.FindControl("lblQuotaAf"))).Text;

                            contitolaritaFondo = rApp.Cells[(int)GvCodMaggiorazioneColumns.DdlContitolarietaFondo].Visible ? ((DropDownList)rApp.FindControl("ddlContitolarietaFondo")).SelectedValue : ((Label)(rApp.FindControl("lblContitolarietaFondo"))).Text;

                        }
                        contabilitàAgo = rApp.Cells[(int)GvCodMaggiorazioneColumns.DdlContitolarietaAgo].Visible ? ((DropDownList)rApp.FindControl("ddlContitolarietaAgo")).SelectedValue : ((Label)(rApp.FindControl("lblContitolarietaAgo"))).Text;

                        if (rApp.Cells[(int)GvCodMaggiorazioneColumns.DdlCodMaggiorazione].Visible && ((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem != null)
                        {
                            maggiorazioneApp = ((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem.Text;
                            codeApp = ((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem.Value;
                            ((Label)(rApp.FindControl("lblCodMaggiorazione"))).Text = codeApp;
                        }
                        else
                        {
                            codeApp = ((Label)(rApp.FindControl("lblCodMaggiorazione"))).Text;
                            maggiorazioneApp = ((Label)(rApp.FindControl("lblMaggiorazione"))).Text;
                        }

                        if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                        {
                            if (rApp.Cells[(int)GvCodMaggiorazioneColumns.DdlParentela].Visible && ddlParentelaApp.SelectedItem != null)
                            {
                                descParentela = ddlParentelaApp.SelectedItem.Text;
                                codParentela = ddlParentelaApp.SelectedValue;
                                ((Label)(rApp.FindControl("lblCodParentela"))).Text = codParentela;
                            }
                            else
                            {
                                descParentela = ((Label)(rApp.FindControl("lblParentela"))).Text;
                                codParentela = ((Label)(rApp.FindControl("lblCodParentela"))).Text;
                            }
                        }

                        if (!string.IsNullOrEmpty(acquisizioneApp) && !string.IsNullOrEmpty(cessazioneApp))
                        {
                            string[] dec = acquisizioneApp.Split('/');
                            string[] fine = cessazioneApp.Split('/');

                            int acquisizioneMese = int.Parse(dec[0]);
                            int acquisizioneAnno = int.Parse(dec[1]);

                            int fineMese = int.Parse(fine[0]);
                            int fineAnno = int.Parse(fine[1]);

                            if (fineAnno < acquisizioneAnno)
                            {
                                txtdecorrenzaValidorigi.IsValid = false;
                                return;
                            }
                            if (fineMese < acquisizioneMese && fineAnno == acquisizioneAnno)
                            {
                                txtdecorrenzaValidorigi.IsValid = false;
                                return;
                            }
                        }

                        if (elencoCodiciMaggiorazione.Count < indexRow)
                        {
                            CodiciMaggiorazione re = new CodiciMaggiorazione();
                            re.CodMaggiorazione = codeApp;
                            re.Maggiorazione = maggiorazioneApp;
                            re.Acquisizione = acquisizioneApp;
                            re.Cessazione = cessazioneApp;
                            re.DescParentela = descParentela;
                            re.CodParentela = codParentela;
                            re.QuotaAf = quotaAf;
                            re.DirittoAf = dirittoAf;
                            re.ContitolaritaAgo = contabilitàAgo;
                            re.ContitolaritaFondo = contitolaritaFondo;
                            elencoCodiciMaggiorazione.Add(re);
                        }
                        else
                        {
                            elencoCodiciMaggiorazione[indexRow].CodMaggiorazione = codeApp;
                            elencoCodiciMaggiorazione[indexRow].Maggiorazione = maggiorazioneApp;
                            elencoCodiciMaggiorazione[indexRow].Acquisizione = acquisizioneApp;
                            elencoCodiciMaggiorazione[indexRow].Cessazione = cessazioneApp;
                            elencoCodiciMaggiorazione[indexRow].DescParentela = descParentela;
                            elencoCodiciMaggiorazione[indexRow].CodParentela = codParentela;
                            elencoCodiciMaggiorazione[indexRow].QuotaAf = quotaAf;
                            elencoCodiciMaggiorazione[indexRow].DirittoAf = dirittoAf;
                            elencoCodiciMaggiorazione[indexRow].ContitolaritaFondo = contitolaritaFondo;
                            elencoCodiciMaggiorazione[indexRow].ContitolaritaAgo = contabilitàAgo;
                        }
                    }
                    else
                    {
                        validateTxtDecorrenzaCarico.IsValid = false;
                        return;
                    }
                }
                else
                {
                    string acquisizioneApp = string.Empty;
                    string cessazioneApp = string.Empty;
                    string codeApp = string.Empty;
                    string maggiorazioneApp = string.Empty;
                    string descParentela = string.Empty;
                    string codParentela = string.Empty;
                    string dirittoAf = string.Empty;
                    string quotaAf = string.Empty;
                    string contitolaritaFondo = string.Empty;
                    string contitolaritàAgo = string.Empty;

                    if (((TextBox)(rApp.FindControl("txtDecorrenzaCarico"))).Text != string.Empty && !(((TextBox)(rApp.FindControl("txtDecorrenzaCarico"))).Text).Equals("MM/AAAA"))
                        acquisizioneApp = ((TextBox)(rApp.FindControl("txtDecorrenzaCarico"))).Text;
                    else
                    {
                        validateTxtDecorrenzaCarico.IsValid = false;
                        return;
                    }
                    if (((TextBox)(rApp.FindControl("txtFineCarico"))).Text != string.Empty && !(((TextBox)(rApp.FindControl("txtFineCarico"))).Text).Equals("MM/AAAA"))
                        cessazioneApp = ((TextBox)(rApp.FindControl("txtFineCarico"))).Text;

                    if (datipensione.TipoAppartenenzaDomanda.HasValue)
                    {
                        if (datipensione.TipoAppartenenzaDomanda.Value != AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                        {
                            if (((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem != null && ((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem.Value != string.Empty)
                            {
                                maggiorazioneApp = ((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem.Text;
                                codeApp = ((DropDownList)rApp.FindControl("ddlCodMaggiorazione")).SelectedItem.Value;
                            }
                        }
                        else
                            codeApp = "0";

                        if (datipensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.AGO || datipensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                        {
                            if (((DropDownList)rApp.FindControl("ddlParentela")).SelectedItem != null && ((DropDownList)rApp.FindControl("ddlParentela")).SelectedItem.Value != string.Empty)
                            {
                                descParentela = ((DropDownList)rApp.FindControl("ddlParentela")).SelectedItem.Text;
                                codParentela = ((DropDownList)rApp.FindControl("ddlParentela")).SelectedValue;
                                ((Label)(rApp.FindControl("lblCodParentela"))).Text = codParentela;
                            }
                            else
                            {
                                descParentela = ((Label)(rApp.FindControl("lblParentela"))).Text;
                                codParentela = ((Label)(rApp.FindControl("lblCodParentela"))).Text;
                            }
                        }
                    }

                    //Qui si devono leggere i valori per le PI/PL
                    if(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI
                        || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                    {
                        dirittoAf = ((DropDownList)rApp.FindControl("ddlDirittoAf")).SelectedValue;
                        quotaAf = ((DropDownList)rApp.FindControl("ddlQuotaAf")).SelectedValue;
                        contitolaritaFondo = ((DropDownList)rApp.FindControl("ddlContitolarietaFondo")).SelectedValue;
                        contitolaritàAgo = ((DropDownList)rApp.FindControl("ddlContitolarietaAgo")).SelectedValue;
                    }

                    //Qui di devono leggere i valori (QuotaAf e ContitolaritaFondo) per le ricostituzioni SFS/SPT della linea FS
                    if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                        && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS
                        && this.domanda.CodGruppo.Equals("0031"))
                    {
                        quotaAf = ((DropDownList)rApp.FindControl("ddlQuotaAf")).SelectedValue.Equals("SI") ? "S" : "N";
                        contitolaritaFondo = ((DropDownList)rApp.FindControl("ddlContitolarietaFondo")).SelectedValue.Equals("SI")? "S" : "N";
                    }

                    if (elencoCodiciMaggiorazione.Count < indexRow)
                    {
                        CodiciMaggiorazione re = new CodiciMaggiorazione();
                        re.CodMaggiorazione = codeApp;
                        re.Maggiorazione = maggiorazioneApp;
                        re.Acquisizione = acquisizioneApp;
                        re.Cessazione = cessazioneApp;
                        re.DescParentela = descParentela;
                        re.CodParentela = codParentela;
                        re.DirittoAf = dirittoAf;
                        re.QuotaAf = quotaAf;
                        re.ContitolaritaFondo = contitolaritaFondo;
                        re.ContitolaritaAgo = contitolaritàAgo;
                        elencoCodiciMaggiorazione.Add(re);
                    }
                    else
                    {
                        elencoCodiciMaggiorazione[indexRow].CodMaggiorazione = codeApp;
                        elencoCodiciMaggiorazione[indexRow].Maggiorazione = maggiorazioneApp;
                        elencoCodiciMaggiorazione[indexRow].Acquisizione = acquisizioneApp;
                        elencoCodiciMaggiorazione[indexRow].Cessazione = cessazioneApp;
                        elencoCodiciMaggiorazione[indexRow].DescParentela = descParentela;
                        elencoCodiciMaggiorazione[indexRow].CodParentela = codParentela;
                        elencoCodiciMaggiorazione[indexRow].DirittoAf = dirittoAf;
                        elencoCodiciMaggiorazione[indexRow].QuotaAf = quotaAf;
                        elencoCodiciMaggiorazione[indexRow].ContitolaritaFondo = contitolaritaFondo;
                        elencoCodiciMaggiorazione[indexRow].ContitolaritaAgo = contitolaritàAgo;
                    }
                }
                ViewState["elencoCodiciMaggiorazione"] = elencoCodiciMaggiorazione;
                GvCodiciMaggiorazione_Load();
                gvCodiciMaggiorazione_DataBound(sender, null);
            }
            else if (e.CommandName == "Edit")
            {
                ViewState["RowIndexEdit"] = r.RowIndex;
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    if (rApp.DataItemIndex == elencoCodiciMaggiorazione.Count - 1)
                        RenderVisibleControls(rApp, false, false, true, true);
                    else
                        RenderVisibleControls(rApp, false, true, false, true);
                }
                DropDownList ddlParentelaApp = null;
                if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                {
                    bool bypassConiugeAndUnitoCivilmente = false;
                    if (r.RowIndex != 0 && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    {
                        if (elencoCodiciMaggiorazione != null && elencoCodiciMaggiorazione.Count() > 0 &&
                                elencoCodiciMaggiorazione.Exists(x => !string.IsNullOrEmpty(x.CodParentela) && !x.CodParentela.StartsWith("C")))
                            bypassConiugeAndUnitoCivilmente = true;
                    }

                    ddlParentelaApp = (DropDownList)r.FindControl("ddlParentela");
                    CaricaDropParentela(ddlParentelaApp, bypassConiugeAndUnitoCivilmente);
                    if (((Label)(r.FindControl("lblParentela"))).Text != string.Empty)
                    {
                        try
                        {
                            ddlParentelaApp.Items.FindByValue(((Label)(r.FindControl("lblCodParentela"))).Text).Selected = true;
                        }
                        catch (Exception)
                        {
                            ddlParentelaApp.SelectedIndex = 0;
                        }
                    }
                    if (ddlParentelaApp.SelectedIndex > 0)
                    {
                        if (((bool?)ViewState["isModify"]).GetValueOrDefault())
                        {
                            this.elencoFamiliari = (List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"];
                            string idAnagrafica = (string)ViewState["Idanagraficamod"];
                            if (!string.IsNullOrEmpty(idAnagrafica))
                            {
                                long id;
                                long.TryParse(idAnagrafica, out id);
                                PresenterFamiliari.FamiliareFull familiare = this.elencoFamiliari.Find(x => x.anagrafica.Id == id);
                                if (familiare != null)
                                    ManageDdlParentelaByDatiFamiliare(ddlParentelaApp, familiare.areaFamiliare.Familiare);
                            }
                        }
                        ((TextBox)(r.FindControl("txtDecorrenzaCarico"))).Text = ((Label)(r.FindControl("lblAcqusizione"))).Text;
                        ((TextBox)(r.FindControl("txtFineCarico"))).Text = ((Label)(r.FindControl("lblCessazione"))).Text;
                        LoadDdl((DropDownList)r.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                        if (((Label)(r.FindControl("lblCodMaggiorazione"))).Text != string.Empty)
                        {
                            try
                            {
                                ((DropDownList)r.FindControl("ddlCodMaggiorazione")).Items.FindByValue(((Label)(r.FindControl("lblCodMaggiorazione"))).Text).Selected = true;
                            }
                            catch (Exception)
                            {
                                ((DropDownList)r.FindControl("ddlCodMaggiorazione")).SelectedIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        ((DropDownList)r.FindControl("ddlCodMaggiorazione")).Enabled = false;
                        ((DropDownList)r.FindControl("ddlCodMaggiorazione")).ClearSelection();
                        ((TextBox)r.FindControl("txtDecorrenzaCarico")).Enabled = false;
                        ((TextBox)r.FindControl("txtDecorrenzaCarico")).Text = string.Empty;
                        ((TextBox)r.FindControl("txtFineCarico")).Enabled = false;
                        ((TextBox)r.FindControl("txtFineCarico")).Text = string.Empty;
                    }
                }
                else
                {
                    ddlParentelaApp = DropParentela;
                    ((TextBox)(r.FindControl("txtDecorrenzaCarico"))).Text = ((Label)(r.FindControl("lblAcqusizione"))).Text;
                    ((TextBox)(r.FindControl("txtFineCarico"))).Text = ((Label)(r.FindControl("lblCessazione"))).Text;
                    LoadDdl((DropDownList)r.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);

                    #region Valorizzazione DropDownLists Campi Aggiuntivi Codici Maggiornazione
                    AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo = this.domanda.Tipofondo;
                    if ((new List<AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo?>() {AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL,
                        AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS}).Contains(tipoFondo))
                    {
                        if (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI ||
                            tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                        {
                            ((DropDownList)r.FindControl("ddlDirittoAf")).SelectedValue = ((Label)r.FindControl("lblDirittoAf")).Text != string.Empty ? ((Label)r.FindControl("lblDirittoAf")).Text : " ";
                            ((DropDownList)r.FindControl("ddlContitolarietaAgo")).SelectedValue = ((Label)r.FindControl("lblContitolarietaAgo")).Text != string.Empty ? ((Label)r.FindControl("lblContitolarietaAgo")).Text : "NO";
                            ((DropDownList)r.FindControl("ddlQuotaAf")).SelectedValue = ((Label)r.FindControl("lblQuotaAf")).Text != string.Empty ? ((Label)r.FindControl("lblQuotaAf")).Text : "NO";
                            ((DropDownList)r.FindControl("ddlContitolarietaFondo")).SelectedValue = ((Label)r.FindControl("lblContitolarietaFondo")).Text != string.Empty ? ((Label)r.FindControl("lblContitolarietaFondo")).Text : "NO";
                        }
                        else
                        {
                            ((DropDownList)r.FindControl("ddlQuotaAf")).SelectedValue = ((Label)r.FindControl("lblQuotaAf")).Text != string.Empty ? (((Label)r.FindControl("lblQuotaAf")).Text.Equals("S")? "SI" : "NO") : "NO";
                            ((DropDownList)r.FindControl("ddlContitolarietaFondo")).SelectedValue = ((Label)r.FindControl("lblContitolarietaFondo")).Text != string.Empty ? (((Label)r.FindControl("lblContitolarietaFondo")).Text.Equals("S")? "SI" : "NO") : "NO";
                        }
                    }
                    #endregion

                    if (((Label)(r.FindControl("lblCodMaggiorazione"))).Text != string.Empty)
                    {
                        try
                        {
                            ((DropDownList)r.FindControl("ddlCodMaggiorazione")).Items.FindByValue(((Label)(r.FindControl("lblCodMaggiorazione"))).Text).Selected = true;
                        }
                        catch (Exception)
                        {
                            ((DropDownList)r.FindControl("ddlCodMaggiorazione")).SelectedIndex = 0;
                        }
                    }
                }

                RenderVisibleControls(r, true, false, false, false);
                gvCodiciMaggiorazione_DataBound(sender, null);
            }
            else if (e.CommandName == "Annulla")
            {
                RemoveItemBlank(ref elencoCodiciMaggiorazione);
                GvCodiciMaggiorazione_Load();
            }
            else if (e.CommandName == "Insert")
            {
                Label fineCarico = null;
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {

                    if (rApp.DataItemIndex == elencoCodiciMaggiorazione.Count - 2)
                    {
                        fineCarico = (Label)rApp.FindControl("lblCessazione");
                        if (string.IsNullOrEmpty(fineCarico.Text))
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Inserire la fine carico prima di aggiungere un nuovo periodo di carico";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }

                    //if (rApp.DataItemIndex == ((List<CodiciMaggiorazione>)(ViewState["elencoCodiciMaggiorazione"])).Count - 1)
                    if (rApp.DataItemIndex == elencoCodiciMaggiorazione.Count - 1)
                    {
                        DropDownList ddlParentelaApp = null;
                        if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                        {
                            bool bypassConiugeAndUnitoCivilmente = false;
                            if (r.RowIndex != 0 && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                            {
                                if (elencoCodiciMaggiorazione != null && elencoCodiciMaggiorazione.Count() > 0 &&
                                        elencoCodiciMaggiorazione.Exists(x => !string.IsNullOrEmpty(x.CodParentela) && !x.CodParentela.StartsWith("C")))
                                    bypassConiugeAndUnitoCivilmente = true;
                            }

                            ddlParentelaApp = (DropDownList)rApp.FindControl("ddlParentela");
                            CaricaDropParentela(ddlParentelaApp, bypassConiugeAndUnitoCivilmente);
                            if (elencoCodiciMaggiorazione.Exists(x => x.CodParentela == "CU" || x.CodParentela == "CM"))
                            {
                                ddlParentelaApp.Items.FindByValue(elencoCodiciMaggiorazione.FirstOrDefault(x => x.CodParentela == "CU" || x.CodParentela == "CM").CodParentela).Selected = true;
                                ddlParentelaApp.Enabled = false;
                            }
                        }
                        else
                            ddlParentelaApp = DropParentela;
                        try
                        {
                            if (elencoCodiciMaggiorazione.Count > 1 && !(this.domanda.TipoAppartenenza.HasValue &&
                                this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS &&
                                string.IsNullOrEmpty(elencoCodiciMaggiorazione[elencoCodiciMaggiorazione.Count - 2].CodParentela)))

                                ddlParentelaApp.Items.FindByValue(elencoCodiciMaggiorazione[elencoCodiciMaggiorazione.Count - 2].CodParentela).Selected = true;
                        }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        LoadDdl((DropDownList)rApp.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);
                        InizializzaDataCarico((TextBox)rApp.FindControl("txtDecorrenzaCarico"), (TextBox)rApp.FindControl("txtFineCarico"), ddlParentelaApp);
                        InizializzaCodiciMaggiorazioni((DropDownList)rApp.FindControl("ddlCodMaggiorazione"), ddlParentelaApp);

                        if (fineCarico != null)
                        {
                            TextBox decorrenzaCarico = (TextBox)rApp.FindControl("txtDecorrenzaCarico");
                            decorrenzaCarico.Text = fineCarico.Text;
                        }

                        RenderVisibleControls(rApp, true, false, false, false);
                    }
                    else
                        RenderVisibleControls(rApp, false, true, false, true);
                }

                ViewState["InsertNewRecord"] = true;
                gvCodiciMaggiorazione_DataBound(sender, null);
            }
            else if (e.CommandName == "Delete")
            {
                elencoCodiciMaggiorazione = new List<CodiciMaggiorazione>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    if (rApp.DataItemIndex != r.DataItemIndex)
                    {
                        CodiciMaggiorazione re = new CodiciMaggiorazione();
                        re.CodMaggiorazione = ((Label)(rApp.FindControl("lblCodMaggiorazione"))).Text;
                        re.Maggiorazione = ((Label)(rApp.FindControl("lblMaggiorazione"))).Text;
                        re.Acquisizione = ((Label)(rApp.FindControl("lblAcqusizione"))).Text;
                        re.Cessazione = ((Label)(rApp.FindControl("lblCessazione"))).Text;
                        re.DescParentela = ((Label)(rApp.FindControl("lblParentela"))).Text;
                        re.CodParentela = ((Label)(rApp.FindControl("lblCodParentela"))).Text;
                        elencoCodiciMaggiorazione.Add(re);
                    }
                }
                RemoveItemBlank(ref elencoCodiciMaggiorazione);
                ViewState["elencoCodiciMaggiorazione"] = elencoCodiciMaggiorazione;
                GvCodiciMaggiorazione_Load();
            }
            managerBtnSalva();
            ViewState.Remove("RowIndexEdit");
            ViewState["InsertNewRecord"] = false;
        }

        protected void gvCodiciMaggiorazione_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            GridView app = (GridView)sender;
            if (datipensione.TipoAppartenenzaDomanda.HasValue)
            {
                if (domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                {
                    if (app.Columns[(int)GvCodMaggiorazioneColumns.LblMaggiorazione] != null)
                        app.Columns[(int)GvCodMaggiorazioneColumns.LblMaggiorazione].Visible = false;
                    if (app.Columns[(int)GvCodMaggiorazioneColumns.DdlCodMaggiorazione] != null)
                        app.Columns[(int)GvCodMaggiorazioneColumns.DdlCodMaggiorazione].Visible = false;
                    if (app.Columns[(int)GvCodMaggiorazioneColumns.LblParentela] != null)
                        app.Columns[(int)GvCodMaggiorazioneColumns.LblParentela].Visible = false;
                    if (app.Columns[(int)GvCodMaggiorazioneColumns.DdlParentela] != null)
                        app.Columns[(int)GvCodMaggiorazioneColumns.DdlParentela].Visible = false;
                }
            }
            
            if (!(domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI ||
                domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL) &&
                !(this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && this.domanda.CodGruppo.Equals("0031")
                    || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && this.domanda.CodGruppo.Equals("0031"))) 
            {
                if (app.Columns[(int)GvCodMaggiorazioneColumns.LblQuotaAf] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.LblQuotaAf].Visible = false;
                if (app.Columns[(int)GvCodMaggiorazioneColumns.DdlQuotaAf] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.DdlQuotaAf].Visible = false;

                if (app.Columns[(int)GvCodMaggiorazioneColumns.LblContitolarietaFondo] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.LblContitolarietaFondo].Visible = false;
                if (app.Columns[(int)GvCodMaggiorazioneColumns.DdlContitolarietaFondo] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.DdlContitolarietaFondo].Visible = false;
            }

            if (!(domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI ||
                domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL))
            {
                if (app.Columns[(int)GvCodMaggiorazioneColumns.LblDirittoAf] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.LblDirittoAf].Visible = false;
                if (app.Columns[(int)GvCodMaggiorazioneColumns.DdlDirittoAf] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.DdlDirittoAf].Visible = false;

                if (app.Columns[(int)GvCodMaggiorazioneColumns.LblContitolarietaAgo] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.LblContitolarietaAgo].Visible = false;
                if (app.Columns[(int)GvCodMaggiorazioneColumns.DdlContitolarietaAgo] != null)
                    app.Columns[(int)GvCodMaggiorazioneColumns.DdlContitolarietaAgo].Visible = false;
            }
        }

        protected void ShowGridCodiciMaggiorazione_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropParentela.SelectedValue == "")
                ExtraInfo.Visible = false;
            else
                ExtraInfo.Visible = true;

            ViewState["ChangeIndexDDL"] = true;
            managerBtnSalva();
            GvCodiciMaggiorazione_Load();
            ViewState["GradoParentela"] = DropParentela.SelectedValue;
        }

        protected void gvCodiciMaggiorazione_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void gvCodiciMaggiorazione_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void ddlParentela_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlParentela = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlParentela.NamingContainer;
            DropDownList ddlCodMaggiorazione = ((DropDownList)row.FindControl("ddlCodMaggiorazione"));
            TextBox txtDecorrenzaCarico = ((TextBox)row.FindControl("txtDecorrenzaCarico"));
            TextBox txtFineCarico = ((TextBox)row.FindControl("txtFineCarico"));
            ddlCodMaggiorazione.ClearSelection();
            string decorrenzaCarico = txtDecorrenzaCarico.Text;
            txtDecorrenzaCarico.Text = string.Empty;
            txtFineCarico.Text = string.Empty;
            if (ddlParentela.SelectedIndex > 0)
            {
                ddlCodMaggiorazione.Enabled = true;
                txtDecorrenzaCarico.Enabled = true;
                txtFineCarico.Enabled = true;
                LoadDdl(ddlCodMaggiorazione, ddlParentela);
                InizializzaDataCarico(txtDecorrenzaCarico, txtFineCarico, ddlParentela);
                InizializzaCodiciMaggiorazioni(ddlCodMaggiorazione, ddlParentela);
                if (!string.IsNullOrEmpty(decorrenzaCarico))
                    txtDecorrenzaCarico.Text = decorrenzaCarico;
            }
            else
            {
                ddlCodMaggiorazione.Enabled = false;
                txtDecorrenzaCarico.Enabled = false;
                txtFineCarico.Enabled = false;
            }

        }

        protected void ddlDirittoAf_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void ddlQuotaAf_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void ddlContitolarietaFondo_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void ddlContitolarietaAgo_SelectedIndexChanged(object sender, EventArgs e) { }
        #endregion Grid CodiciMaggiorazione

        #region protected methods

        protected void RaiseSalvafamiliari(object sender, EventArgs e)
        {
            if (SalvaFamiliari != null)
                SalvaFamiliari(sender, e);
        }

        protected void RaiseFamiliariNonSalvati(object sender, EventArgs e)
        {
            if (FamiliariNonSalvati != null)
                FamiliariNonSalvati(sender, e);
        }

        protected void RaiseAddModFamiliare(object sender, EventArgs e)
        {
            AddModFamiliareEvent(this, null);
        }

        protected void RaiseEliminaFamiliari(object sender, EventArgs e)
        {
            if (EliminaFamiliari != null)
                EliminaFamiliari(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected bool CheckCoerencyCodiciMaggiorazione()
        {
            List<CodiciMaggiorazione> codiciMaggiorazione = (List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"];
            RemoveItemBlank(ref codiciMaggiorazione);
            foreach (CodiciMaggiorazione codice in codiciMaggiorazione)
            {
                if(domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                {
                    if(codice.DirittoAf == null)
                    {
                        ucAvviso.Messaggio = string.Format("Impossibile Assegnare Valore NULL a Diritto A.F. per domande con tipo fondo {0}", domanda.Tipofondo);
                        ucAvviso.Visible = true;
                        return false;
                    }
                    if((codice.Cessazione == string.Empty || DateTime.Parse(codice.Cessazione) > DateTime.Parse(codice.Acquisizione)) && (codice.ContitolaritaAgo != "SI" && codice.ContitolaritaFondo != "SI"))
                    {
                        ucAvviso.Messaggio = string.Format("Almeno uno fra Contitolarità AGO e contitolarità Fondo deve essere valorizzato a 'SI'");
                        ucAvviso.Visible = true;
                        return false;
                    }
                }
            }
            ucAvviso.Visible = false;
            return true;
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            if (!CheckCoerencyCodiciMaggiorazione())
                return;
            if (ViewState["isModify"] == null)
            {
                GetNewFamiliare();
                SalvaNewFamiliare();
            }
            else
            {
                GetFamiliareModify();
                UpdateFamiliare();
            }
        }

        protected void btnUpdateArca_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterFamiliari PresenterFamiliari = new PresenterFamiliari();

            this.codiceFiscale = txtCFAltriFamiliari.Text;
            PresenterFamiliari.AggiornaAnagraficaByArca(this);
            if (this.areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                HasError = true;
                ErrorMessage = this.areaEsito.Messaggio;
                AreaEsito esito = new AreaEsito();
                esito.Messaggio = ErrorMessage;
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                RaiseFamiliariNonSalvati(esito, e);
            }
            else if (this.areaRiepilogoAnagrafica != null)
            {
                hidenCF.Text = this.areaRiepilogoAnagrafica.CodiceFiscale;
                Lbcognome.Text = this.areaRiepilogoAnagrafica.Cognome;
                LbNome.Text = this.areaRiepilogoAnagrafica.Nome;
                lbCognAcquisito.Text = this.areaRiepilogoAnagrafica.CognomeAcquisito;
                LbSesso.Text = this.areaRiepilogoAnagrafica.Sesso.ToString();
                String data = this.areaRiepilogoAnagrafica.DataNascita.ToString();
                LbDataDiNascita.Text = data.Substring(0, data.LastIndexOf(@"/", StringComparison.CurrentCulture) + 5);
                LbComunedinascita.Text = this.areaRiepilogoAnagrafica.ComuneNascita;
                LbProvinciadinascita.Text = this.areaRiepilogoAnagrafica.ProvinciaNascita;
                if (this.areaRiepilogoAnagrafica.DataMorte.HasValue)
                {
                    pnlDataMorte.Visible = true;
                    lblDataMorteValue.Text = String.Format("{0:dd/MM/yyyy}", this.areaRiepilogoAnagrafica.DataMorte.Value);
                }
                else
                {
                    pnlDataMorte.Visible = false;
                    lblDataMorteValue.Text = string.Empty;
                }

                Anagrafica modanagrafica = new Anagrafica();
                modanagrafica.Nome = this.areaRiepilogoAnagrafica.Nome;
                modanagrafica.Cognome = this.areaRiepilogoAnagrafica.Cognome;
                modanagrafica.DataNascita = this.areaRiepilogoAnagrafica.DataNascita;
                modanagrafica.CodiceFiscale = this.areaRiepilogoAnagrafica.CodiceFiscale;
                modanagrafica.ComuneNascita = this.areaRiepilogoAnagrafica.ComuneNascita;
                modanagrafica.Sesso = this.areaRiepilogoAnagrafica.Sesso;
                modanagrafica.ProvinciaNascita = this.areaRiepilogoAnagrafica.ProvinciaNascita;
                modanagrafica.CognomeAcquisito = this.areaRiepilogoAnagrafica.CognomeAcquisito;
                modanagrafica.CodiceComuneNascita = this.areaRiepilogoAnagrafica.CodiceComuneNascita;
                modanagrafica.Cittadinanza = this.areaRiepilogoAnagrafica.Cittadinanza;
                modanagrafica.ComuneResidenza = this.areaRiepilogoAnagrafica.ComuneResidenza;
                modanagrafica.CodiceComuneResidenza = this.areaRiepilogoAnagrafica.CodiceComuneResidenza;
                modanagrafica.Indirizzo = this.areaRiepilogoAnagrafica.Indirizzo;
                modanagrafica.NCivico = this.areaRiepilogoAnagrafica.NumeroCivico;
                modanagrafica.CAP = this.areaRiepilogoAnagrafica.Cap;
                modanagrafica.DomicilioEstero = this.areaRiepilogoAnagrafica.DomicilioEstero;
                modanagrafica.ResidenzaEstero = this.areaRiepilogoAnagrafica.ResidenzaEstero;
                modanagrafica.Codice1Arca = this.areaRiepilogoAnagrafica.Codice1Arca;
                modanagrafica.Codice2Arca = this.areaRiepilogoAnagrafica.Codice2Arca;
                modanagrafica.Tel = this.areaRiepilogoAnagrafica.Tel;
                modanagrafica.Cell = this.areaRiepilogoAnagrafica.Cell;
                modanagrafica.EMail = this.areaRiepilogoAnagrafica.EMail;
                modanagrafica.ProvinciaResidenza = this.areaRiepilogoAnagrafica.ProvinciaResidenza;
                modanagrafica.FrazioneResidenza = this.areaRiepilogoAnagrafica.FrazioneResidenza;
                modanagrafica.CodiceStatoCivile = this.areaRiepilogoAnagrafica.CodiceStatoCivile;
                modanagrafica.DecorrenzaStatoCivile = this.areaRiepilogoAnagrafica.DecorrenzaStatoCivile;
                modanagrafica.CodiceFiscale = this.areaRiepilogoAnagrafica.CodiceFiscale;
                modanagrafica.DataMorte = this.areaRiepilogoAnagrafica.DataMorte;
                ViewState["UpdateArca"] = modanagrafica;
            }
            else
            {
                AreaEsito esito = new AreaEsito();
                esito.Messaggio = "Errore durante l'aggiornamento da ARCA";
                esito.RisultatoOperazione = AreaEsito.TipoEsito.KO;
                RaiseFamiliariNonSalvati(esito, e);
            }
        }

        protected void btnAggiungiFamiliare_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            List<CodiciMaggiorazione> elencoCodiciMaggiorazione = (List<CodiciMaggiorazione>)ViewState["elencoCodiciMaggiorazione"];
            elencoCodiciMaggiorazione.Clear();


            ViewState["elencoCodiciMaggiorazione"] = elencoCodiciMaggiorazione;
            ViewState["isModify"] = null;
            ViewState["Idanagraficamod"] = null;

            if (btnAddFamiliare.Text.Equals("Aggiungi Familiare"))
            {
                btnAddFamiliare.Text = "Annulla Aggiungi Familiare";
                btnAddFamiliare.CssClass = RemoveCssClass(btnAddFamiliare.CssClass, "primary");
                if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault())
                    btnAddFamiliare.Width = Unit.Pixel(208);
                else
                    btnAddFamiliare.Width = Unit.Pixel(168);
                pnlFamiliari.Visible = false;
                btnSalva.Visible = true;
                btnEliminaFamiliari.Visible = false;
                pnlSearch.Visible = true;
                if (this.domanda.TipoAppartenenza.HasValue && (this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO || this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI))
                    trParentela.Visible = false;
                else
                    DropParentela.Enabled = false;
                txtCFAltriFamiliari.Enabled = true;
                imgCercaAltriFamiliari.Visible = true;
                btnUpdateArca.Visible = false;
                Clearfields();
            }
            else
            {
                btnAddFamiliare.Text = "Aggiungi Familiare";
                btnAddFamiliare.CssClass = AddCssClass(btnAddFamiliare.CssClass, "primary"); ;
                if ((Session["IsSistemaUnico"] as bool?).GetValueOrDefault())
                    btnAddFamiliare.Width = Unit.Pixel(190);
                else
                    btnAddFamiliare.Width = Unit.Pixel(150);
                pnlFamiliari.Visible = true;
                ExtraInfo.Visible = false;
                btnSalva.Visible = false;
                btnEliminaFamiliari.Visible = true;
                pnlSearch.Visible = false;
                ViewFamiliari_DataBinding(null, null);
                ViewFamiliari.DataBind();
                ViewState.Remove("UpdateArca");

                GestioneVariazioneDatiContitolari();
            }
            managerBtnSalva();
        }

        private void GestioneVariazioneDatiContitolari()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            //ENG - RIC VARIAZIONE DATI CONTITOLARI
            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && Utility.IsRicostituzioneVariazioneDatiContitolari(datipensione))
            {
                if (this.TitolarePensione != null && this.TitolarePensione.Anagrafica != null && !String.IsNullOrEmpty(this.TitolarePensione.Anagrafica.CodiceFiscale))
                {
                    if (ViewState["ListFamiliari"] != null && ((List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"]).Exists(x => x.anagrafica != null && x.anagrafica.CodiceFiscale == this.TitolarePensione.Anagrafica.CodiceFiscale))
                    {
                        GestioneAreaFamiliariAreaFamiliare soggettoPresentazioneDomanda = ((List<PresenterFamiliari.FamiliareFull>)ViewState["ListFamiliari"]).FindAll(x => x.anagrafica != null && x.anagrafica.CodiceFiscale == this.TitolarePensione.Anagrafica.CodiceFiscale).First().areaFamiliare;
                        if (soggettoPresentazioneDomanda != null && soggettoPresentazioneDomanda.Familiare != null)
                        {
                            bool isTitolarePensione = soggettoPresentazioneDomanda.Familiare.FlagTitolare.GetValueOrDefault();
                            if (!isTitolarePensione)
                            {
                                if (btnAddFamiliare.Text.Equals("Aggiungi Familiare"))
                                    btnAddFamiliare.Enabled = false;
                            }
                        }
                    }
                }
            }

        }

        protected void btnEliminaFamiliari_Click(object sender, EventArgs e)
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterFamiliari presenterFamiliari = new PresenterFamiliari();
            presenterFamiliari.CancellaFamiliari(this, this);

            ViewState["ListFamiliari"] = this.elencoFamiliari;
            DataBind();
            RaiseEliminaFamiliari(this, null);
        }

        protected void btnElimina_Click(object sender, EventArgs e)
        {

        }

        protected void CercaAltriFamiliari(object sender, ImageClickEventArgs e)
        {
            try
            {
                CFsearch(sender, e);
                DataBind();

                if (ViewState["GradoParentela"] != null)
                {
                    if ((Convert.ToString(ViewState["GradoParentela"])) == string.Empty)
                        ExtraInfo.Visible = false;
                    else
                        ExtraInfo.Visible = true;
                }
            }
            catch { return; }
        }

        protected void DropParentela_PreRender(object sender, EventArgs e)
        {
            foreach (ListItem _listItem in DropParentela.Items)
            {
                _listItem.Attributes.Add("title", _listItem.Text);
            }
            DropParentela.Attributes.Add("onmouseover", "this.title=this.options[this.selectedIndex].title");
        }


        public static string RemoveCssClass(string currentClasses, string classToRemove)
        {
            if (currentClasses == null || currentClasses.Trim().Length == 0)
                return currentClasses ?? string.Empty;

            if (classToRemove == null || classToRemove.Trim().Length == 0)
                return currentClasses;

            var filtered = currentClasses
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(c => !string.Equals(c, classToRemove, StringComparison.Ordinal))
                .ToArray();

            return string.Join(" ", filtered);
        }


        public static string AddCssClass(string currentClasses, string classToAdd)
        {
            if (classToAdd == null || classToAdd.Trim().Length == 0)
                return currentClasses ?? string.Empty;

            classToAdd = classToAdd.Trim();

            if (currentClasses == null || currentClasses.Trim().Length == 0)
                return classToAdd;

            var classes = currentClasses
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (!classes.Contains(classToAdd))
                classes.Add(classToAdd);

            return string.Join(" ", classes.ToArray());
        }



        #endregion protected methods

        #region nested class

        [Serializable]
        public class CodiciMaggiorazione
        {
            public CodiciMaggiorazione()
            { }

            public CodiciMaggiorazione(string acquisizione, string cessazione, string maggiorazione, string codMaggiorazione, string parentela, string descParentela, string codParentela)
            {
                this.Acquisizione = acquisizione;
                this.Cessazione = cessazione;
                this.Maggiorazione = maggiorazione;
                this.CodMaggiorazione = codMaggiorazione;
                this.DescParentela = descParentela;
            }

            public string Acquisizione { get; set; }
            public string Cessazione { get; set; }
            public string Maggiorazione { get; set; }
            public string CodMaggiorazione { get; set; }
            public string DescParentela { get; set; }
            public string CodParentela { get; set; }
            public string DirittoAf { get; set; }
            public string QuotaAf { get; set; }
            public string ContitolaritaFondo { get; set; }
            public string ContitolaritaAgo { get; set; }
        }


        public enum GvCodMaggiorazioneColumns
        {
            BtnFunzione = 0,
            LblParentela = 1,
            DdlParentela = 2,
            LblMaggiorazione = 3,
            DdlCodMaggiorazione = 4,
            LblDecorrenzaCarico = 5,
            TxtDecorrenzaCarico = 6,
            LblFineCarico = 7,
            TxtFineCarico = 8,
            LblDirittoAf = 9,
            DdlDirittoAf = 10,
            LblQuotaAf = 11,
            DdlQuotaAf = 12,
            LblContitolarietaFondo = 13,
            DdlContitolarietaFondo = 14,
            LblContitolarietaAgo = 15,
            DdlContitolarietaAgo = 16,
            BtnElimina = 17
        }
        #endregion nested class

    }
}
