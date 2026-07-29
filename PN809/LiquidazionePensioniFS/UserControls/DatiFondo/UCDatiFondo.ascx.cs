using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCDatiFondo : CustomBaseUserControl, IDatiFondo, ITitolarePensione, IDanteCausa
    {

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        #endregion IDatiFondo

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = this.GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - RIC REVERSIBILITA 024            
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            ManageDecorrenzaForReversibilita(this.TitolarePensione.Pensione, areaDatiFondo.DecorrenzaPensioneDirettaDC, this.areaDanteCausa, areaDatiFondo.TipoReversibilita);

            if (this.TitolarePensione.Pensione != null && this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenzaPensione.Text = String.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value);

            this.areaDatiFondo = areaDatiFondo;

            RenderControls(this.TitolarePensione.Pensione);


            if ((this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                this.domanda.Categoria.Substring(0, 1) == "S") || this.domanda.IsDomandaINPDAP)
                trTipoPensione.Visible = false;

            btnSalvaDatiFondo.Text = "Salva Dati "; // + CodeUtility.GetLabelFondoCassa(this.domanda);
            btnEliminaDatiFondo.Text = "Elimina Dati "; // + CodeUtility.GetLabelFondoCassa(this.domanda);

            if (areaDatiFondo != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;
                ViewState[EnumViewState.IsPrimoRecord.ToString()] = areaDatiFondo.IsPrimoRecord;

                if (areaDatiFondo.IsPrimoRecord.HasValue && areaDatiFondo.IsPrimoRecord.Value)
                {
                    txtDecorrenzaRegistrazione.Visible = false;
                    lblDecorrenzaRegistrazione.Visible = true;
                    RFVtxtDecorrenzaRegistrazione.Enabled = false;

                    if ((domanda.IsDomandaINPDAP && CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && Utility.IsDomandaReversibilitaOrRicostituzione(this.TitolarePensione.Pensione, this.areaDanteCausa, this.domanda.Categoria, areaDatiFondo.TipoReversibilita, this.domanda.Tipofondo)) ||
                        (CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && this.domanda.Categoria.Substring(0, 1) == "S" && this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)))
                    {
                        //Per le ricostituzioni di REV, se già valorizzato DecorrenzaValidita non va sovrascritto con la decorrenza pensione
                        if (areaDatiFondo.DatiFondo.DecorrenzaValidita.HasValue)
                            lblDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaValidita.Value);
                        else
                        {
                            if (((DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()]).HasValue)
                                lblDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", (DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()]);
                        }
                    }
                    else
                    {
                        if (((DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()]).HasValue)
                            lblDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", (DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()]);
                    }
                }
                else
                {
                    txtDecorrenzaRegistrazione.Visible = true;
                    lblDecorrenzaRegistrazione.Visible = false;
                    RFVtxtDecorrenzaRegistrazione.Enabled = true;
                    lblDecorrenzaRegistrazione.Text = "";

                    if (areaDatiFondo.DatiFondo.DecorrenzaValidita.HasValue)
                        txtDecorrenzaRegistrazione.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaValidita.Value);
                }

                if (areaDatiFondo.DatiFondo != null)
                {
                    lblTipoPensione.Text = areaDatiFondo.DatiFondo.TipoPensione;

                    if (domanda.IsDomandaINPDAP && CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione) && Utility.IsDomandaReversibilitaOrRicostituzione(this.TitolarePensione.Pensione, this.areaDanteCausa, this.domanda.Categoria, areaDatiFondo.TipoReversibilita, this.domanda.Tipofondo))
                    {
                        //Per le ricostituzioni di REV, se la DecorrenzaCalcolo è diversa DecorrenzaRegistrazione, deve essere impostata uguale
                        if (areaDatiFondo.DatiFondo.DecorrenzaCalcolo.HasValue)
                        {
                            if (lblDecorrenzaRegistrazione.Text != String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaCalcolo.Value))
                                lblDecorrenzaCalcolo.Text = lblDecorrenzaRegistrazione.Text;
                            else
                                lblDecorrenzaCalcolo.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaCalcolo.Value);
                        }
                    }
                    else
                    {
                        if (areaDatiFondo.DatiFondo.DecorrenzaCalcolo.HasValue)
                            lblDecorrenzaCalcolo.Text = String.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DecorrenzaCalcolo.Value);
                    }

                    if (this.domanda.IsDomandaINPDAP)
                    {
                        if (!CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && !Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione))
                        {
                            ddlTredicesimaMens.SelectedValue = "SI";
                            ddlTredicesimaMens.Enabled = false;
                        }
                        else
                        {
                            if (areaDatiFondo.DatiFondo.TrediciMensilita.HasValue)
                            {
                                if (areaDatiFondo.DatiFondo.TrediciMensilita.Value)
                                {
                                    ddlTredicesimaMens.SelectedValue = "SI";
                                    ddlTredicesimaMens.Enabled = false;
                                }

                                else
                                    ddlTredicesimaMens.SelectedValue = "NO";
                            }
                        }
                    }
                    else
                    {
                        if (areaDatiFondo.DatiFondo.TrediciMensilita.HasValue)
                        {
                            if (areaDatiFondo.DatiFondo.TrediciMensilita.Value)
                                ddlTredicesimaMens.SelectedValue = "SI";
                            else
                                ddlTredicesimaMens.SelectedValue = "NO";
                        }
                        else
                            ddlTredicesimaMens.ClearSelection();
                    }

                    if (areaDatiFondo.DatiFondo.IncrementoContrattuale.HasValue)
                        lblIncrementoContrattuale.Text = areaDatiFondo.DatiFondo.IncrementoContrattuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (areaDatiFondo.DatiFondo.PagamentoIndennitaIntegrativaSpeciale.HasValue)
                    {
                        if (areaDatiFondo.DatiFondo.PagamentoIndennitaIntegrativaSpeciale.Value)
                            ddlPagIndennIntegrSpec.SelectedValue = "SI";
                        else
                            ddlPagIndennIntegrSpec.SelectedValue = "NO";
                    }
                    else
                        ddlPagIndennIntegrSpec.ClearSelection();

                    if (areaDatiFondo.DatiFondo.DirittoIndennitaIntegrativaSpeciale.HasValue)
                    {
                        if (areaDatiFondo.DatiFondo.DirittoIndennitaIntegrativaSpeciale.Value)
                            ddlDirittoIndennIntegrSpec.SelectedValue = "SI";
                        else
                            ddlDirittoIndennIntegrSpec.SelectedValue = "NO";
                    }
                    else
                        ddlDirittoIndennIntegrSpec.ClearSelection();

                    if (areaDatiFondo.DatiFondo.IntegrazioneMinimo.HasValue)
                    {
                        if (areaDatiFondo.DatiFondo.IntegrazioneMinimo.Value)
                            ddlIntegrazioneMinimo.SelectedValue = "SI";
                        else
                            ddlIntegrazioneMinimo.SelectedValue = "NO";
                    }
                    else
                        ddlIntegrazioneMinimo.ClearSelection();

                    if (this.domanda.IsDomandaINPDAP && !CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                    {
                        ddlIndennIntegrSpecConglobata.ClearSelection();
                        ddlIndennIntegrSpecConglobata.Enabled = false;
                        //var decorrenza = (DateTime?)ViewState[EnumViewState.DecorrenzaPensione.ToString()];
                        //if (decorrenza.HasValue && decorrenza.Value.Year > 2009)
                        //    ddlIndennIntegrSpecConglobata.SelectedValue = "NO";
                    }
                    else
                    {
                        if (areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata.HasValue)
                        {
                            if (areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata.Value)
                                ddlIndennIntegrSpecConglobata.SelectedValue = "SI";
                            else
                                ddlIndennIntegrSpecConglobata.SelectedValue = "NO";
                        }
                        else
                            ddlIndennIntegrSpecConglobata.ClearSelection();
                    }

                    if (areaDatiFondo.DatiFondo.TitolareAltraPensione.HasValue && !this.domanda.IsDomandaINPDAP)
                    {
                        if (areaDatiFondo.DatiFondo.TitolareAltraPensione.Value)
                            ddlTitAltraPensione.SelectedValue = "SI";
                        else
                            ddlTitAltraPensione.SelectedValue = "NO";
                    }
                    else
                    {
                        ddlTitAltraPensione.SelectedValue = "NO";
                        if (this.domanda.IsDomandaINPDAP)
                        {
                            tdNOTitAltraPensione.Visible = true;
                            tdLblTitAltraPensione.Visible = false;
                            tdDdlTitAltraPensione.Visible = false;
                        }
                    }

                    if (Utility.AbilitaFlussoSeiScatti() && Utility.IsDomandaCTPS(this.domanda.Categoria) && this.domanda.IsDomandaINPDAP)
                    {
                        txtNumeroRate.Text = areaDatiFondo.DatiFondo.NumeroRate.HasValue ? areaDatiFondo.DatiFondo.NumeroRate.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        txtImportoSingolaRata.Text = areaDatiFondo.DatiFondo.ImportoSingolaRata.HasValue ? areaDatiFondo.DatiFondo.ImportoSingolaRata.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        labelImportoSingolaRata.Visible = true;
                        txtImportoSingolaRata.Visible = true;
                        pnlNumeroRate.Visible = true;
                        Art4.Visible = true;
                        bool bloccaCampi6Scatti = (Utility.IsRicostituzione(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione)) || Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione);
                        if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) != Utility.TipoUnicarpe.Automatica && !bloccaCampi6Scatti)
                        {
                            txtNumeroRate.Enabled = true;
                            txtImportoSingolaRata.Enabled = true;
                        }
                        else
                        {
                            txtNumeroRate.Enabled = false;
                            txtImportoSingolaRata.Enabled = false;
                        }
                    }
                    else
                    {
                        labelImportoSingolaRata.Visible = false;
                        txtImportoSingolaRata.Visible = false;
                        pnlNumeroRate.Visible = false;
                    }

                    //Memo 145/2025
                    if (this.domanda.IsDomandaINPDAP && Utility.IsDomandaCTPS(this.domanda.Categoria))
                    {
                        pnlIndennitaSpeciale.Visible = true;
                        txtCodiceIndennizzo.Text = areaDatiFondo.DatiFondo.CodInd;
                        txtImportoIndenizzo.Text = areaDatiFondo.DatiFondo.ImpInd.HasValue ? areaDatiFondo.DatiFondo.ImpInd.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        txtImportoRataIniziale.Text = areaDatiFondo.DatiFondo.ImpRataIniz.HasValue ? areaDatiFondo.DatiFondo.ImpRataIniz.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        txtImportoRata.Text = areaDatiFondo.DatiFondo.ImpRataOrd.HasValue ? areaDatiFondo.DatiFondo.ImpRataOrd.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        txtImportoRataFinale.Text = areaDatiFondo.DatiFondo.ImpRataFin.HasValue ? areaDatiFondo.DatiFondo.ImpRataFin.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        txtNumRate.Text = areaDatiFondo.DatiFondo.NumRate.HasValue ? areaDatiFondo.DatiFondo.NumRate.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : String.Empty;
                        if (areaDatiFondo.DatiFondo.DataInizioInd.HasValue)
                            this.txtInizioIndennizzo.Text = string.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DataInizioInd.Value);
                        if (areaDatiFondo.DatiFondo.DataCessInd.HasValue)
                            this.txtFineIndennizzo.Text = string.Format("{0:dd/MM/yyyy}", areaDatiFondo.DatiFondo.DataCessInd.Value);

                        bool bloccaCampiMemo = (Utility.IsRicostituzione(this.TitolarePensione.Pensione) && !Utility.IsRicostituzione_MotiviContributivi(this.TitolarePensione.Pensione)) || Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione);
                        if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica || bloccaCampiMemo)
                        {
                            pnlIndennitaSpeciale.Enabled = false;
                        }
                    }

                    string categoriaPensione = domanda.Categoria != null ? this.domanda.Categoria.Trim().ToUpper() : null;
                    if (categoriaPensione != null && (categoriaPensione.Equals("SPT") || categoriaPensione.Equals("SFS")))
                    {
                        this.pnlIndennitàSpecialeLorda.Visible = true;
                        this.txtImportoIndennitaSpecialeLorda.Text = areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeLorda.ToString();
                    }
                }
            }

            //ENG - Reversibilita 024
            if (Utility.IsDomandaReversibilita(this.TitolarePensione.Pensione) && this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                RequiredFieldValidator1.Enabled = false;
                RequiredFieldValidator3.Enabled = false;
            }
        }

        internal Presenter.SvrLiquidazioneFs.DatiFondo RecuperaCampi()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiFondo = new Presenter.SvrLiquidazioneFs.DatiFondo();

            if (!string.IsNullOrEmpty(txtDecorrenzaRegistrazione.Text))
                this.areaDatiFondo.DatiFondo.DecorrenzaValidita = Presenter.Utility.GetDateFromString(txtDecorrenzaRegistrazione.Text);
            else
                if (!string.IsNullOrEmpty(lblDecorrenzaRegistrazione.Text))
                this.areaDatiFondo.DatiFondo.DecorrenzaValidita = Presenter.Utility.GetDateFromString(lblDecorrenzaRegistrazione.Text);

            if (!string.IsNullOrEmpty(lblDecorrenzaCalcolo.Text))
                this.areaDatiFondo.DatiFondo.DecorrenzaCalcolo = Presenter.Utility.GetDateFromString(lblDecorrenzaCalcolo.Text);

            if (String.Equals(ddlTredicesimaMens.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.TrediciMensilita = true;
            else if (String.Equals(ddlTredicesimaMens.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.TrediciMensilita = false;

            if (!string.IsNullOrEmpty(lblIncrementoContrattuale.Text))
                this.areaDatiFondo.DatiFondo.IncrementoContrattuale = decimal.Parse(lblIncrementoContrattuale.Text);

            if (String.Equals(ddlPagIndennIntegrSpec.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.PagamentoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlPagIndennIntegrSpec.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.PagamentoIndennitaIntegrativaSpeciale = false;

            if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.DirittoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.DirittoIndennitaIntegrativaSpeciale = false;

            if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.IntegrazioneMinimo = true;
            else if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.IntegrazioneMinimo = false;

            if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata = true;
            else if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeConglobata = false;

            if (String.Equals(ddlTitAltraPensione.SelectedValue, "SI"))
                this.areaDatiFondo.DatiFondo.TitolareAltraPensione = true;
            else if (String.Equals(ddlTitAltraPensione.SelectedValue, "NO"))
                this.areaDatiFondo.DatiFondo.TitolareAltraPensione = false;
            if (!string.IsNullOrEmpty(txtNumeroRate.Text))
                this.areaDatiFondo.DatiFondo.NumeroRate = Int32.Parse(txtNumeroRate.Text);
            if (!string.IsNullOrEmpty(txtImportoSingolaRata.Text))
                this.areaDatiFondo.DatiFondo.ImportoSingolaRata = decimal.Parse(txtImportoSingolaRata.Text);

            if (!string.IsNullOrEmpty(txtImportoIndenizzo.Text))
                this.areaDatiFondo.DatiFondo.ImpInd = decimal.Parse(txtImportoIndenizzo.Text);
            if (!string.IsNullOrEmpty(txtCodiceIndennizzo.Text))
                this.areaDatiFondo.DatiFondo.CodInd = txtCodiceIndennizzo.Text;
            if (!string.IsNullOrEmpty(txtInizioIndennizzo.Text))
                this.areaDatiFondo.DatiFondo.DataInizioInd = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtInizioIndennizzo.Text)));
            if (!string.IsNullOrEmpty(txtFineIndennizzo.Text))
                this.areaDatiFondo.DatiFondo.DataCessInd = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtFineIndennizzo.Text)));
            if (!string.IsNullOrEmpty(txtImportoRataIniziale.Text))
                this.areaDatiFondo.DatiFondo.ImpRataIniz = decimal.Parse(txtImportoRataIniziale.Text);
            if (!string.IsNullOrEmpty(txtImportoRata.Text))
                this.areaDatiFondo.DatiFondo.ImpRataOrd = decimal.Parse(txtImportoRata.Text);
            if (!string.IsNullOrEmpty(txtImportoRataFinale.Text))
                this.areaDatiFondo.DatiFondo.ImpRataFin = decimal.Parse(txtImportoRataFinale.Text);
            if (!string.IsNullOrEmpty(txtNumRate.Text))
                this.areaDatiFondo.DatiFondo.NumRate = Int32.Parse(txtNumRate.Text);

            string categoriaPensione = this.domanda.Categoria != null? this.domanda.Categoria.Trim().ToUpper() : null;
            if (categoriaPensione != null && (categoriaPensione.Equals("SPT") || categoriaPensione.Equals("SFS")))
            {
                areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeLorda = this.txtImportoIndennitaSpecialeLorda.Text != string.Empty ?  decimal.Parse(this.txtImportoIndennitaSpecialeLorda.Text) : (decimal?)null;
            }
            else
                areaDatiFondo.DatiFondo.IndennitaIntegrativaSpecialeLorda = null;

            return this.areaDatiFondo.DatiFondo;
        }

        protected void SalvaDatiFondo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.StoreDatiFondoByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati " + CodeUtility.GetLabelFondoCassa(this.domanda) + " salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiFondo(this, null);
            }
        }

        protected void btnEliminaDatiFondo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];
            this.areaDatiFondo.IsPrimoRecord = (bool)ViewState[EnumViewState.IsPrimoRecord.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.EliminaDatiFondoByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati " + CodeUtility.GetLabelFondoCassa(this.domanda) + " eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiFondo(this, null);

                ClearForm();

                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiFondo;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiFondo(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiFondo != null)
                UpdateSemaforoDatiFondo(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiFondo != null)
                HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            if (TornaARegistrazioniFondo != null)
                TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers

        #region private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        private void RenderControls(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaINPDAP)
            {
                //attività 16/08/2021 - rimossa visibilità campo
                trIndennIntegrSpecConglobata.Visible = true;


                if (this.areaDatiFondo != null && this.areaDatiFondo.DatiFondo != null)
                {
                    pnlIncrementoContrattuale.Visible = false;

                    if (this.areaDatiFondo.IsDecPensAnteAgosto95.HasValue)
                    {
                        pnlIntegrazioneMinimo.Visible = this.areaDatiFondo.IsDecPensAnteAgosto95.Value;
                        pnlDirittoIndennitaIntegrativaSpeciale.Visible = false;
                        pnlPagamentoIndennitaIntegrativaSpeciale.Visible = false;
                    }
                }

                if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
                    ddlIndennIntegrSpecConglobata.Enabled = false;

                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaReversibilita(datiPensione))
                    btnEliminaDatiFondo.Enabled = false;
            }
            else
            {
                if (this.areaDatiFondo != null && this.areaDatiFondo.DatiFondo != null)
                {
                    if (this.areaDatiFondo.DatiFondo.IncrementoContrattuale.HasValue)
                        pnlIncrementoContrattuale.Visible = true;
                    else
                    {
                        pnlIncrementoContrattuale.Visible = false;
                        lblIncrementoContrattuale.Text = string.Empty;
                    }

                    if (this.areaDatiFondo.IsDecPensAnteAgosto95.HasValue)
                    {
                        pnlIntegrazioneMinimo.Visible = this.areaDatiFondo.IsDecPensAnteAgosto95.Value;
                        pnlDirittoIndennitaIntegrativaSpeciale.Visible = this.areaDatiFondo.IsDecPensAnteAgosto95.Value;
                        pnlPagamentoIndennitaIntegrativaSpeciale.Visible = this.areaDatiFondo.IsDecPensAnteAgosto95.Value;
                    }

                }

                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                    && (Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione)))
                    btnEliminaDatiFondo.Enabled = false;
            }
        }

        private void ManageDecorrenzaForReversibilita(AreaTitolare.DatiPensione datiPensione, DateTime? decorrenzaPensioneDirettaDC, AreaDanteCausa danteCausa, char? tipoReversibilita)
        {
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            //ENG - RIC REVERSIBILITA 024 
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita
                || (Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, this.domanda.Categoria, tipoReversibilita, this.domanda.Tipofondo)
                && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.IsDomandaINPDAP)))
                ViewState[EnumViewState.DecorrenzaPensione.ToString()] = decorrenzaPensioneDirettaDC;
            else
                ViewState[EnumViewState.DecorrenzaPensione.ToString()] = datiPensione.DecorrenzaOriginaria;
        }
        #endregion private methods

        enum EnumViewState
        {
            IdRecordFondo,
            IsPrimoRecord,
            DecorrenzaPensione
        }
    }
}