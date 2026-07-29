using System;
using System.Web;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa
{
    public partial class UCDanteAnagrafica : CustomBaseUserControl, IDanteCausa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDanteCausa
        public long numDomanda { get; set; }
        public Presenter.SvrLiquidazione.AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDanteCausa


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;

            if (ViewState["TipoAppartenenzaDomanda"] == null)
                ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;
            if ((((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO ||
                 ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI))
            {
                this.divCI_AGO.Visible = true;
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                    this.trParentela.Visible = true;
                else
                    txtDataMorte.Enabled = false;
            }
        }

        private void LoadDdl()
        {
            try
            {
                LoadDdlStatoEstero_Cittadinanza();
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                    LoadDdlParentelaDC();

                CodeUtility areaDecodifica = new CodeUtility();
                LoadDdlCodeProvenienza(areaDecodifica);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteAnagrafica, Errore nel metodo RenderControls: " + ex);
            }
        }

        private void LoadDdlStatoEstero_Cittadinanza()
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica.DatiStatoEstero[] listStatiEsteri = areaDecodifica.GetValuesDecodifica().ElencoStatiEsteri;
            this.ddlCittadinanza.Items.Clear();
            foreach (AreaDecodifica.DatiStatoEstero statoEstero in listStatiEsteri)
            {
                this.ddlCittadinanza.Items.Add(new ListItem(statoEstero.Descrizione, statoEstero.CodCatastale));
                this.ddlresidenzaestero.Items.Add(new ListItem(statoEstero.Descrizione, statoEstero.CodCatastale));
            }
            this.ddlresidenzaestero.Items.Remove(new ListItem("ITALIA", "Z000"));
        }

        private void LoadDdlParentelaDC()
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica.DatiParentelaDC[] listParentelaDC = areaDecodifica.GetValuesDecodifica().ElencoParentelaDC;
            this.ddlRelazioneParentela.Items.Clear();
            foreach (AreaDecodifica.DatiParentelaDC parentelaDC in listParentelaDC)
            {
                this.ddlRelazioneParentela.Items.Add(new ListItem(parentelaDC.Descrizione, parentelaDC.Id));
                this.ddlRelazioneParentela.Items[this.ddlRelazioneParentela.Items.Count - 1].Attributes.Add("title", parentelaDC.Descrizione);

            }
        }

        private void LoadDdlCodeProvenienza(CodeUtility areaDecodifica)
        {
            this.ddlCodiceProvenienza.Items.Clear();
            AreaDecodifica.DatiCodiciProvenienza[] listCodiceProvenienza = areaDecodifica.GetValuesDecodifica().ElencoCodiciProvenienza;
            CodeUtility.SetItemBlankDdl(ddlCodiceProvenienza);
            foreach (AreaDecodifica.DatiCodiciProvenienza codeProvenienza in listCodiceProvenienza)
                CodeUtility.SetValueDdl(ddlCodiceProvenienza, codeProvenienza.Descrizione, codeProvenienza.Descrizione, codeProvenienza.Id);
        }

        internal void ValorizzaControlliAnagraficaDC(IDanteCausa danteCausa)
        {
            if (ViewState["TipoAppartenenzaDomanda"] == null)
                ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;

            //ENG - Reversibilita 024
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (danteCausa.areaDanteCausa != null && danteCausa.areaDanteCausa.AnagraficaDC != null && danteCausa.areaDanteCausa.AnagraficaDC.DataNascitaContitolareConiuge.HasValue)
                hdnDataNascitaContitolareConiuge.Value = string.Format("{0:dd/MM/yyyy}", danteCausa.areaDanteCausa.AnagraficaDC.DataNascitaContitolareConiuge.Value);

            string controlloDinamico = string.Empty;
            string controlloDinamicoAbilitazioneSpachettate024 = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamico);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                controlloDinamicoAbilitazioneSpachettate024 = controlloDinamico;

            RenderControls(danteCausa.areaDanteCausa);

            this.lblCFAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.CodiceFiscale;
            this.lblCognomeAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.Cognome;
            this.lblComuneNascitaAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.ComuneNascita;

            if (danteCausa.areaDanteCausa.AnagraficaDC.DataMatrimonio.HasValue)
                this.txtDataMatrimonio.Text = string.Format("{0:dd/MM/yyyy}", danteCausa.areaDanteCausa.AnagraficaDC.DataMatrimonio.Value);

            if (danteCausa.areaDanteCausa.AnagraficaDC.DataMorte.HasValue)
            {
                this.txtDataMorte.Text = string.Format("{0:dd/MM/yyyy}", danteCausa.areaDanteCausa.AnagraficaDC.DataMorte.Value);
                if (Utility.IsDomandaSpacchettamentoENPALS(this.domanda.IsDomandaENPALS, this.domanda.Categoria) || Utility.IsDomandaSpacchettamentoINPDAP(this.domanda.IsDomandaINPDAP, this.domanda.Categoria)
                    || (!String.IsNullOrEmpty(controlloDinamico) && controlloDinamico.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione))
                    || (Utility.IsDomandaReversibilita(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)) //ENG - Reversibilita 024
                    || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, danteCausa.areaDanteCausa)
                    || Utility.IsDomandaSpacchettamentoSO(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, this.domanda)
                    || Utility.IsDomandaSpacchettamentoSR(datiPensione, this.domanda)) //ENG - Spacchettate SOPGI
                    txtDataMorte.Enabled = false;
            }
            else
            {
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                {
                    this.btnSalvaAnagrafica.Enabled = false;
                    this.HasError = true;
                    this.ErrorMessage = "Data morte non valorizzata su Arca; impossibile proseguire l'acquisizione";
                    RaiseShowAvviso(this, null);
                }
            }

            if (danteCausa.areaDanteCausa.AnagraficaDC.DataNascita.HasValue)
                this.lblDataNascitaAnagrafica.Text = string.Format("{0:dd/MM/yyyy}", danteCausa.areaDanteCausa.AnagraficaDC.DataNascita.Value);
            this.lblNomeAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.Nome;
            this.lblProvinciaNascitaAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.ProvinciaNascita;
            if (danteCausa.areaDanteCausa.AnagraficaDC.Sesso.HasValue)
                this.lblSessoAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.Sesso.Value.ToString();

            if ((((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO ||
                 ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI))
            {
                LoadDdl();

                if (!string.IsNullOrEmpty(danteCausa.areaDanteCausa.AnagraficaDC.StatoEEResidenza))
                    this.ddlresidenzaestero.Items.FindByValue(danteCausa.areaDanteCausa.AnagraficaDC.StatoEEResidenza).Selected = true;
                if (danteCausa.areaDanteCausa.AnagraficaDC.DecorrenzaResidenza.HasValue)
                    this.txtResidenzaEE_Dal.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.AnagraficaDC.DecorrenzaResidenza.Value);

                if (danteCausa.areaDanteCausa.AnagraficaDC.Cittadinanza != null && danteCausa.areaDanteCausa.AnagraficaDC.Cittadinanza.Trim() != string.Empty)
                    this.ddlCittadinanza.Items.FindByValue(danteCausa.areaDanteCausa.AnagraficaDC.Cittadinanza).Selected = true;

                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                {
                    if (danteCausa.areaDanteCausa.AnagraficaDC.ParentelaDC.HasValue)
                    {
                        this.ddlRelazioneParentela.Items.FindByValue(danteCausa.areaDanteCausa.AnagraficaDC.ParentelaDC.Value.ToString()).Selected = true;
                        if (danteCausa.areaDanteCausa.AnagraficaDC.IsContitolareConiuge.HasValue && danteCausa.areaDanteCausa.AnagraficaDC.IsContitolareConiuge.Value)
                            this.ddlRelazioneParentela.Enabled = false;
                    }
                }

                ManageCodiceProvenienza(danteCausa.areaDanteCausa);
            }

            if (danteCausa.areaDanteCausa.AnagraficaDC.CategoriaFascicolo.HasValue && danteCausa.areaDanteCausa.AnagraficaDC.SedeFascicolo.HasValue && danteCausa.areaDanteCausa.AnagraficaDC.NumeroFascicolo.HasValue)
            {
                trCodiceFascicolo.Visible = true;
                lblCodiceFascicolo.Text = danteCausa.areaDanteCausa.AnagraficaDC.CategoriaFascicolo.ToString().PadLeft(3, '0') + " - " + danteCausa.areaDanteCausa.AnagraficaDC.SedeFascicolo.ToString().PadLeft(4, '0') + " - " + danteCausa.areaDanteCausa.AnagraficaDC.NumeroFascicolo.ToString().PadLeft(8, '0');
            }

            if (this.domanda != null && this.domanda.Categoria != null && this.domanda.Categoria.Trim() == "SDZ")
            {
                txtDataMorte.Enabled = false;
            }

            ViewState["AreaDC"] = danteCausa.areaDanteCausa;

            //ENG - Ric Superstiti 024: in presenza del bypass NESSUN_DANTE_CAUSA allora il tab anagrafica obbligatorio e il tab diretta opzionale
            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
            {
                if (danteCausa.areaDanteCausa.IsPresenteBypassNessunDanteCausa && Utility.IsRicostituzione(datiPensione) && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                    && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    txtCodiceFiscaleAnagrafica.Visible = true;
                    txtCognomeAnagrafica.Visible = true;
                    txtNomeAnagrafica.Visible = true;
                    txtSessoAnagrafica.Visible = true;
                    txtDataNascitaAnagrafica.Visible = true;
                    txtComuneNascitaAnagrafica.Visible = true;
                    txtProvinciaNascitaAnagrafica.Visible = true;
                    rfvCodiceFiscale.Enabled = true;

                    if (danteCausa.areaDanteCausa.AnagraficaDC != null)
                    {
                        txtCodiceFiscaleAnagrafica.Text = !String.IsNullOrEmpty(danteCausa.areaDanteCausa.AnagraficaDC.CodiceFiscale) ? danteCausa.areaDanteCausa.AnagraficaDC.CodiceFiscale.ToUpperInvariant() : "";
                        txtCognomeAnagrafica.Text = !String.IsNullOrEmpty(danteCausa.areaDanteCausa.AnagraficaDC.Cognome) ? danteCausa.areaDanteCausa.AnagraficaDC.Cognome.ToUpperInvariant() : "";
                        txtNomeAnagrafica.Text = !String.IsNullOrEmpty(danteCausa.areaDanteCausa.AnagraficaDC.Nome) ? danteCausa.areaDanteCausa.AnagraficaDC.Nome.ToUpperInvariant() : "";
                        txtSessoAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.Sesso.HasValue ? danteCausa.areaDanteCausa.AnagraficaDC.Sesso.ToString().ToUpperInvariant() : "";
                        txtDataNascitaAnagrafica.Text = danteCausa.areaDanteCausa.AnagraficaDC.DataNascita.HasValue ? danteCausa.areaDanteCausa.AnagraficaDC.DataNascita.Value.ToString("dd/MM/yyyy") : "";
                        txtComuneNascitaAnagrafica.Text = !String.IsNullOrEmpty(danteCausa.areaDanteCausa.AnagraficaDC.ComuneNascita) ? danteCausa.areaDanteCausa.AnagraficaDC.ComuneNascita.ToUpperInvariant() : "";
                        txtProvinciaNascitaAnagrafica.Text = !String.IsNullOrEmpty(danteCausa.areaDanteCausa.AnagraficaDC.ProvinciaNascita) ? danteCausa.areaDanteCausa.AnagraficaDC.ProvinciaNascita.ToUpperInvariant() : "";
                    }

                    lblCFAnagrafica.Visible = false;
                    lblCognomeAnagrafica.Visible = false;
                    lblNomeAnagrafica.Visible = false;
                    lblSessoAnagrafica.Visible = false;
                    lblDataNascitaAnagrafica.Visible = false;
                    lblComuneNascitaAnagrafica.Visible = false;
                    lblProvinciaNascitaAnagrafica.Visible = false;
                }
            }

            if (danteCausa.areaDanteCausa.AnagraficaDC != null && danteCausa.areaDanteCausa.AnagraficaDC.CodiceFiscale != null && danteCausa.areaDanteCausa.AnagraficaDC.CodiceFiscale.StartsWith("DANTEC_"))
                pnlCodiceFiscale.Visible = false;
        }

        internal AnagraficaDC GetValoriAnagraficaDC()
        {
            AreaDanteCausa areaDC = (AreaDanteCausa)ViewState["AreaDC"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (areaDC != null && areaDC.AnagraficaDC != null)
            {
                try
                {
                    areaDC.AnagraficaDC.DataMorte = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDataMorte.Text)));
                }
                catch (Exception)
                {
                    areaDC.AnagraficaDC.DataMorte = null;
                }

                try
                {
                    areaDC.AnagraficaDC.DataMatrimonio = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDataMatrimonio.Text)));
                }
                catch (Exception)
                {
                    areaDC.AnagraficaDC.DataMatrimonio = null;
                }

                if (ViewState["TipoAppartenenzaDomanda"] == null)
                    ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;
                if ((((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO ||
                     ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI))
                {
                    areaDC.AnagraficaDC.StatoEEResidenza = this.ddlresidenzaestero.SelectedValue;
                    try
                    {
                        areaDC.AnagraficaDC.DecorrenzaResidenza = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtResidenzaEE_Dal.Text)));
                    }
                    catch (Exception)
                    {
                        areaDC.AnagraficaDC.DecorrenzaResidenza = null;
                    }
                    areaDC.AnagraficaDC.Cittadinanza = this.ddlCittadinanza.SelectedValue;
                    if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                    {
                        if (this.ddlRelazioneParentela.SelectedItem.Value != string.Empty)
                            areaDC.AnagraficaDC.ParentelaDC = Convert.ToByte(this.ddlRelazioneParentela.SelectedItem.Value);
                        else
                            areaDC.AnagraficaDC.ParentelaDC = null;
                    }

                    if (!String.IsNullOrEmpty(this.ddlCodiceProvenienza.SelectedValue))
                        areaDC.AnagraficaDC.ProvenienzaPensione = Convert.ToByte(ddlCodiceProvenienza.SelectedValue);
                }

                //ENG - Ric Superstiti 024: in presenza del bypass NESSUN_DANTE_CAUSA allora il tab anagrafica obbligatorio e il tab diretta opzionale
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                {
                    if (areaDC != null && areaDC.IsPresenteBypassNessunDanteCausa && Utility.IsRicostituzione(datiPensione) && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                        && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                    {
                        areaDC.AnagraficaDC.CodiceFiscale = !String.IsNullOrEmpty(txtCodiceFiscaleAnagrafica.Text) ? txtCodiceFiscaleAnagrafica.Text.Trim().ToUpperInvariant() : "";
                        areaDC.AnagraficaDC.Nome = !String.IsNullOrEmpty(txtNomeAnagrafica.Text) ? txtNomeAnagrafica.Text.Trim().ToUpperInvariant() : "";
                        areaDC.AnagraficaDC.Cognome = !String.IsNullOrEmpty(txtCognomeAnagrafica.Text) ? txtCognomeAnagrafica.Text.Trim().ToUpperInvariant() : "";
                        try
                        {
                            if (!String.IsNullOrEmpty(txtSessoAnagrafica.Text))
                                areaDC.AnagraficaDC.Sesso = Convert.ToChar(txtSessoAnagrafica.Text.Trim());
                        }
                        catch (Exception)
                        {
                            areaDC.AnagraficaDC.Sesso = null;
                        }

                        try
                        {
                            if (!String.IsNullOrEmpty(txtDataNascitaAnagrafica.Text))
                                areaDC.AnagraficaDC.DataNascita = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtDataNascitaAnagrafica.Text)));
                        }
                        catch (Exception)
                        {
                            areaDC.AnagraficaDC.DataNascita = null;
                        }
                        areaDC.AnagraficaDC.ComuneNascita = !String.IsNullOrEmpty(txtComuneNascitaAnagrafica.Text) ? txtComuneNascitaAnagrafica.Text : null;
                        areaDC.AnagraficaDC.ProvinciaNascita = !String.IsNullOrEmpty(txtProvinciaNascitaAnagrafica.Text) ? txtProvinciaNascitaAnagrafica.Text : null;
                    }
                }
            }
            else
                areaDC = new AreaDanteCausa();
            return areaDC.AnagraficaDC;
        }

        protected void btnSalvaAnagrafica_Click(object sender, EventArgs e)
        {
            areaDanteCausa = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaDanteCausa();
            areaDanteCausa.AnagraficaDC = new AnagraficaDC();
            areaDanteCausa.AnagraficaDC = GetValoriAnagraficaDC();

            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.SalvaDatiAnagraficaDC(this);
            RaiseShowAvviso(this, null);
        }

        private void RenderControls(AreaDanteCausa areaDanteCausa)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"] == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
            {
                if ((CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && areaDanteCausa.AnagraficaDC.DataMatrimonioByPrelievo.GetValueOrDefault()) ||
                    Utility.IsTitolareExConiugeOrScioltoDallUnione(areaDanteCausa.AnagraficaDC.SiglaFamiliare))
                    txtDataMatrimonio.Enabled = false;
            }
            else
            {
                if ((CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                    (!this.domanda.IsDomandaENPALS || areaDanteCausa.AnagraficaDC.DataMatrimonioByPrelievo.GetValueOrDefault())) ||
                    Utility.IsTitolareExConiugeOrScioltoDallUnione(areaDanteCausa.AnagraficaDC.SiglaFamiliare))
                    txtDataMatrimonio.Enabled = false;
            }

            if ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"] == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
            {
                DateTime dataEstremoSuperiore = new DateTime(1995, 12, 31);
                if (Utility.DataSuccessivaA(dataEstremoSuperiore, datiPensione.DecorrenzaOriginaria.GetValueOrDefault()) ||
                    (areaDanteCausa != null && areaDanteCausa.DatiPensioneCI != null && Utility.DataSuccessivaA(dataEstremoSuperiore, areaDanteCausa.DatiPensioneCI.DecorrenzaOriginariaPrima.GetValueOrDefault())) ||
                    this.domanda.Categoria.ToUpperInvariant().StartsWith("S"))
                    txtDataMatrimonio.Enabled = true;
            }


            if (Utility.IsTitolareExConiugeOrScioltoDallUnione(areaDanteCausa.AnagraficaDC.SiglaFamiliare))
            {
                txtDataMatrimonio.Visible = false;
                lblDataMatrimonioUnioneCivile.Visible = false;
            }

            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"] == AreaTitolare.DatiPensione.TipoAppDomanda.AGO ||
                 (AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"] == AreaTitolare.DatiPensione.TipoAppDomanda.CI))
            {
                this.divCI_AGO.Visible = true;

                if ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"] == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                {
                    btnPopUp.Visible = true;
                    btnSalvaAnagrafica.Style.Add("display", "none");
                }

                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                    this.trParentela.Visible = true;

                if (areaDanteCausa.AnagraficaDC.StatoEEResidenzaByArca.HasValue)
                    ddlresidenzaestero.Enabled = !areaDanteCausa.AnagraficaDC.StatoEEResidenzaByArca.Value;
                if (areaDanteCausa.AnagraficaDC.CittadinanzaByArca.HasValue)
                {
                    ddlCittadinanza.Enabled = btnCittadinanza.Enabled = !areaDanteCausa.AnagraficaDC.CittadinanzaByArca.Value;
                }
                if (areaDanteCausa.AnagraficaDC.IsResidenzaEE_DalEnabled.HasValue)
                {
                    txtResidenzaEE_Dal.Enabled = areaDanteCausa.AnagraficaDC.IsResidenzaEE_DalEnabled.Value;
                    hdnIsResidenzaEE_DalEnabled.Value = areaDanteCausa.AnagraficaDC.IsResidenzaEE_DalEnabled.Value.ToString();
                }
            }
            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaSOPED(this.domanda.Categoria))
                RFVCittadinanza.Enabled = false;
            if (Utility.IsRicostituzione(datiPensione) && areaDanteCausa != null &&
                 (areaDanteCausa.IsAnte96 != null))
            {
                btnCittadinanza.Enabled = false;
                btnCittadinanza.Visible = false;

            }

        }

        private void ManageCodiceProvenienza(AreaDanteCausa areaDanteCausa)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (Utility.IsDomandaIndiretta(datiPensione))
            {
                if (ddlCodiceProvenienza.Items.FindByValue("0") != null)
                {
                    ddlCodiceProvenienza.SelectedValue = "0";
                    ddlCodiceProvenienza.Enabled = false;
                }
            }
            else if (Utility.IsDomandaReversibilita(datiPensione))
            {
                if (areaDanteCausa != null && areaDanteCausa.DatiPensioneDiretta != null)
                {
                    if (areaDanteCausa.DatiPensioneDiretta.SiglaCategoria.StartsWith("V") ||
                        (areaDanteCausa.DatiPensioneDiretta.SiglaCategoria.Trim() == "DIR" && areaDanteCausa.DatiPensioneDiretta.Sede == "9933"))
                    {
                        ddlCodiceProvenienza.SelectedValue = "1";
                        ddlCodiceProvenienza.Enabled = false;
                    }
                    if (areaDanteCausa.DatiPensioneDiretta.SiglaCategoria.StartsWith("I") ||
                        (areaDanteCausa.DatiPensioneDiretta.SiglaCategoria.Trim() == "INV" && areaDanteCausa.DatiPensioneDiretta.Sede == "9933"))
                    {
                        ddlCodiceProvenienza.SelectedValue = "2";
                        ddlCodiceProvenienza.Enabled = false;
                    }
                }
            }
            else if (areaDanteCausa.AnagraficaDC.ProvenienzaPensione != null)
            {
                ddlCodiceProvenienza.Items.FindByValue(Convert.ToString(areaDanteCausa.AnagraficaDC.ProvenienzaPensione)).Selected = true;
                if (Utility.IsDomandaRipristinoOrRiliquidazioneSuperstiti(datiPensione))
                {
                    ddlCodiceProvenienza.Enabled = false;
                }
            }


            if (CodeUtility.IsRicostituzione(datiPensione))
                ddlCodiceProvenienza.Enabled = false;

            if (Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
            {
                ddlCodiceProvenienza.SelectedValue = "0";
                ddlCodiceProvenienza.Visible = false;
                lblCodiceProvenienza.Visible = false;
            }
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;
    }
}
