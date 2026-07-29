using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;


namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class Istruttoria : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public GestioneAnagraficaAccordiDecodAnagraficaAccordi decodAnagraficaAccordi { get; set; }
        #endregion ILiquidazionePensioneAgo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            HiddenFieldSiglaCategoria.Value = this.domanda.Categoria.Trim();
            HiddenFieldFiltro.Value = datiPensione.Filtro;

            if (Utility.IsDomandaVESO92(domanda.Categoria) || Utility.IsDomandaESOAMB(domanda.Categoria) || Utility.IsDomandaESPA(domanda.Categoria))
            {
                if (Request["__EVENTTARGET"] == "txtAzienda")
                {
                    txtAziendaOnChange();
                    RaiseHideAvviso(this, null);
                }
            }
        }

        protected void AggiornaCampiCodice_Click(Object sender, EventArgs e)
        {
            RaiseHideAvviso(this, null);
            bool? isPrepensionamentoEditoria = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoria.ToString()];
            bool? isPrepensionamentoEditoriaArt1c154L205_2017 = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaArt1c154L205_2017.ToString()];
            bool? IsPrepensionamentoEditoriaArt1c500L160_2019 = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaArt1c500L160_2019.ToString()];
            bool? isPrepensionamentoEditoriaLetteraB = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaLetteraB.ToString()];

            if (isPrepensionamentoEditoria.GetValueOrDefault())
            {
                List<DecAnagraficaAccordi> listaDecAnagraficaAccordi = (List<DecAnagraficaAccordi>)ViewState[EnumViewState.ListaDecAnagraficaAccordi.ToString()];
                List<DecAnagraficaAziende> listaDecAnagraficaAziende = (List<DecAnagraficaAziende>)ViewState[EnumViewState.ListaDecAnagraficaAziende.ToString()];
                DecAnagraficaAccordi anagraficaAccordi = null;
                short? codiceAccordi = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);

                GetCampiCodice(codiceAccordi, listaDecAnagraficaAccordi, out anagraficaAccordi);

                if ((anagraficaAccordi != null) && (anagraficaAccordi.Abilitata.Value == true))
                {
                    txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                    if (anagraficaAccordi.DataAccordi.HasValue)
                    {
                        txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        txtDataAccordi.Text = "";
                    }
                    txtDecreto.Text = anagraficaAccordi.Decreto;
                }
                else
                {
                    this.HasError = true;
                    if (this.HasError)
                    {
                        if (anagraficaAccordi == null)
                            this.ErrorMessage = "Attenzione: il Codice Accordo inserito non rientra attualmente tra quelli censiti per la liquidazione della prestazione di prepensionamento ex art. 37.";
                        else
                            if ((anagraficaAccordi != null) && (anagraficaAccordi.Abilitata.Value == false))
                                this.ErrorMessage = "Il campo 'Abilitata' del codice è impostato a false.";
                    }
                }
            }
            else if (isPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault())
            {
                List<DecAnagraficaAccordiPerTipo0171> listaDecAnagraficaAccordi = (List<DecAnagraficaAccordiPerTipo0171>)ViewState[EnumViewState.ListaDecAnagraficaAccordiPerTipo0171.ToString()];
                List<DecAnagraficaAziendePerTipo0171> listaDecAnagraficaAziende = (List<DecAnagraficaAziendePerTipo0171>)ViewState[EnumViewState.ListaDecAnagraficaAziendePerTipo0171.ToString()];
                DecAnagraficaAccordiPerTipo0171 anagraficaAccordi = null;
                short? codiceAccordi = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);

                GetCampiCodice(codiceAccordi, listaDecAnagraficaAccordi, out anagraficaAccordi);

                if ((anagraficaAccordi != null) && (anagraficaAccordi.Abilitata.Value == true))
                {
                    txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                    txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    this.HasError = true;
                    if (this.HasError)
                    {
                        if (anagraficaAccordi == null)
                            this.ErrorMessage = "Codice non presente.";
                        else
                            if ((anagraficaAccordi != null) && (anagraficaAccordi.Abilitata.Value == false))
                                this.ErrorMessage = "Il campo 'Abilitata' del codice è impostato a false.";
                    }
                }
            }
            else if (IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
            {
                List<DecAnagraficaAccordiPerTipo0179> listaDecAnagraficaAccordi = (List<DecAnagraficaAccordiPerTipo0179>)ViewState[EnumViewState.ListaDecAnagraficaAccordiPerTipo0179.ToString()];
                List<DecAnagraficaAziendePerTipo0179> listaDecAnagraficaAziende = (List<DecAnagraficaAziendePerTipo0179>)ViewState[EnumViewState.ListaDecAnagraficaAziendePerTipo0179.ToString()];
                DecAnagraficaAccordiPerTipo0179 anagraficaAccordi = null;
                short? codiceAccordi = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);

                GetCampiCodice(codiceAccordi, listaDecAnagraficaAccordi, out anagraficaAccordi);

                if ((anagraficaAccordi != null))
                {
                    txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                    txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    this.HasError = true;
                    if (this.HasError)
                    {
                        if (anagraficaAccordi == null)
                            this.ErrorMessage = "Codice non presente.";
                    }
                }
            }
            else if (isPrepensionamentoEditoriaLetteraB.GetValueOrDefault())
            {
                List<DecAnagraficaAccordiLetteraB> listaDecAnagraficaAccordi = (List<DecAnagraficaAccordiLetteraB>)ViewState[EnumViewState.ListaDecAnagraficaAccordiLetteraB.ToString()];
                List<DecAnagraficaAziendeLetteraB> listaDecAnagraficaAziende = (List<DecAnagraficaAziendeLetteraB>)ViewState[EnumViewState.ListaDecAnagraficaAziendeLetteraB.ToString()];
                DecAnagraficaAccordiLetteraB anagraficaAccordi = null;
                short? codiceAccordi = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);

                GetCampiCodice(codiceAccordi, listaDecAnagraficaAccordi, out anagraficaAccordi);

                if ((anagraficaAccordi != null) && (anagraficaAccordi.Abilitata.Value == true))
                {
                    txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                    if (anagraficaAccordi.DataAccordi.HasValue)
                    {
                        txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        txtDataAccordi.Text = "";
                    }
                    txtDecreto.Text = anagraficaAccordi.Decreto;
                }
                else
                {
                    this.HasError = true;
                    if (this.HasError)
                    {
                        if (anagraficaAccordi == null)
                            this.ErrorMessage = "Attenzione: il Codice Accordo inserito non rientra attualmente tra quelli censiti per la liquidazione della prestazione di prepensionamento ex art. 37 lett. b).";
                        else
                            if ((anagraficaAccordi != null) && (anagraficaAccordi.Abilitata.Value == false))
                                this.ErrorMessage = "Il campo 'Abilitata' del codice è impostato a false.";
                    }
                }
            }
            if (this.HasError)
            {
                txtDenominazioneAzienda.Text = string.Empty;
                txtDataAccordi.Text = string.Empty;
                txtDecreto.Text = string.Empty;
                RaiseShowAvviso(this, null);
            }
        }

        protected void SalvaIstruttoria_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiIstruttoria = GetDatiIstruttoria();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiIstruttoriaAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaIstruttoria_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiIstruttoriaAgo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Istruttoria";
            else
            {
                ClearForm();
                ValorizzaEtichetteIstruttoria(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void btnAggiornaAnnoBancaFideiussione_OnClick(object sender, EventArgs e)
        {
            txtAziendaOnChange();

            RaiseHideAvviso(this, null);
        }

        protected void ddlAnnoBancaFideiussione_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            short? annoBancaFideiussione = CodeUtility.StringToNullableShort(ddlAnnoBancaFideiussione.SelectedValue);
            List<DecBancaFideiussione> listaDecBancaFideiussioneFilterAnno = (List<DecBancaFideiussione>)ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterAnno.ToString()];
            List<DecBancaFideiussione> listaDecBancaFideiussioneFilterProgressivo = null;
            LoadDdlProgressivoBancaFideiussione(annoBancaFideiussione, listaDecBancaFideiussioneFilterAnno, out listaDecBancaFideiussioneFilterProgressivo);

            ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterProgressivo.ToString()] = listaDecBancaFideiussioneFilterProgressivo;

            if (listaDecBancaFideiussioneFilterProgressivo == null ||
                (!listaDecBancaFideiussioneFilterProgressivo.Exists(x => x.Progressivo.HasValue)))
                btnAggiornaProgressivoBancaFideiussione.Enabled = false;
            else
                btnAggiornaProgressivoBancaFideiussione.Enabled = true;

            ViewState[EnumViewState.DataScadenzaAssegno.ToString()] = Utility.GetDateFromString(txtScadenza.Text);
            SelezionaTipoCalendarioPerScadenzaAssegno();

            RaiseHideAvviso(this, null);
        }

        protected void ddlProgressivoBancaFideiussione_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ValorizzaBancaFideiussione();

            ViewState[EnumViewState.DataScadenzaAssegno.ToString()] = Utility.GetDateFromString(txtScadenza.Text);
            SelezionaTipoCalendarioPerScadenzaAssegno();

            RaiseHideAvviso(this, null);
        }

        private void ValorizzaBancaFideiussione()
        {
            if (Utility.IsDomandaVESO92(domanda.Categoria) || Utility.IsDomandaESPA(domanda.Categoria))
            {
                short? progressivoBancaFideiussione = CodeUtility.StringToNullableShort(ddlProgressivoBancaFideiussione.SelectedValue);

                if (progressivoBancaFideiussione.HasValue)
                {
                    List<DecBancaFideiussione> listaDecBancaFideiussioneFilterProgressivo = (List<DecBancaFideiussione>)ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterProgressivo.ToString()];
                    DecBancaFideiussione bancaFideiussioneSelected = listaDecBancaFideiussioneFilterProgressivo.SingleOrDefault(x => x.Progressivo == progressivoBancaFideiussione);
                    if (bancaFideiussioneSelected != null)
                    {
                        lblABIBancaFideiussione.Text = bancaFideiussioneSelected.ABI.GetValueOrDefault().ToString().PadLeft(5, '0');
                        lblCABBancaFideiussione.Text = bancaFideiussioneSelected.CAB.GetValueOrDefault().ToString().PadLeft(7, '0');
                        lblBancaFideiussione.Text = bancaFideiussioneSelected.BancaFideiussione;
                    }
                    else
                        CleanBancaFideiussione();
                }
                else
                    CleanBancaFideiussione();
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
        }

        internal void EnableDisableBtnSalva(bool enable)
        {
            this.btnSalvaIstruttoria.Enabled = enable;
            this.btnPopUp.Enabled = enable;
            this.btnSalvaIstruttoriaNoRiduzione.Enabled = enable;
            this.btnEliminaIstruttoria.Enabled = enable;
        }

        internal DatiIstruttoria GetDatiIstruttoria()
        {
            //ENG - RIC RIDUZIONE RETRIBUTIVA
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiIstruttoria = new DatiIstruttoria();
            areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS = new DatiIstruttoria.ENPALS();

            areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];
            areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoria.ToString()];
            areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017 = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaArt1c154L205_2017.ToString()];
            areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019 = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaArt1c500L160_2019.ToString()];
            areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaLetteraB.ToString()];

            if (!String.IsNullOrEmpty(ddlCodCD_CM_MR.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceCdCmMr = byte.Parse(ddlCodCD_CM_MR.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceCdCmMr = null;

            if (!String.IsNullOrEmpty(ddlCodReqRidotti.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.Legge44997 = byte.Parse(ddlCodReqRidotti.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiIstruttoria.Legge44997 = null;

            if (!String.IsNullOrEmpty(ddlSoggettoDerogato.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceParticolareSoggettoDerogato = long.Parse(ddlSoggettoDerogato.SelectedValue);
            else
                areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceParticolareSoggettoDerogato = null;

            string aliquotaTFR = string.Empty;
            if (!String.IsNullOrEmpty(txtAliquotaTFREsodatiInt.Text) && !String.IsNullOrEmpty(txtAliquotaTFREsodatiDec.Text))
            {
                aliquotaTFR = string.Concat(txtAliquotaTFREsodatiInt.Text, ",", txtAliquotaTFREsodatiDec.Text);
                areaLiquidazionePensioneAgo.DatiIstruttoria.AliquotaTFREsodati = Convert.ToDecimal(aliquotaTFR);
            }
            else
                areaLiquidazionePensioneAgo.DatiIstruttoria.AliquotaTFREsodati = null;

            //ENG - RIC RIDUZIONE RETRIBUTIVA
            if ((areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.HasValue && areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.Value) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                if (ddlRiduzioneRetributiva != null && string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneRetributiva = true;
                else if (ddlRiduzioneRetributiva != null && string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneRetributiva = false;
            }

            areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;

            if (HiddenFieldAziendaVisible.Value == "true")
            {
                if (!String.IsNullOrEmpty(txtAzienda.Text))
                {
                    List<DecodificaAzienda> listaAziendaEditoria = (List<DecodificaAzienda>)ViewState[EnumViewState.ListaAziendaEditoria.ToString()];
                    DecodificaAzienda codeAziendaEditoria = listaAziendaEditoria.Find(x => (x.TraduzioneSuGP + " - " + x.Descrizione.Trim()) == txtAzienda.Text);
                    if (codeAziendaEditoria != null)
                        areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati = ControlAzienda(txtAzienda.Text);
                }
            }

            if (Utility.IsDomandaBancari(this.domanda.Categoria))
            {
                if (!string.IsNullOrEmpty(ddlCodiceBanca.SelectedValue))
                {
                    areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati = CodeUtility.StringToNullableShort(ddlCodiceBanca.SelectedValue);
                }
            }

            if (Utility.IsDomandaPSO(this.domanda.Categoria))
            {
                if (!string.IsNullOrEmpty(ddlCodiceEnte.SelectedValue))
                {
                    areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceEnte = CodeUtility.StringToNullableShort(ddlCodiceEnte.SelectedValue);
                }
            }

            if (HiddenFieldAttivitaUsurantiVisible.Value == "true")
            {
                if (string.Equals(ddlAttivitaUsuranti.SelectedValue, "SI"))
                    areaLiquidazionePensioneAgo.DatiIstruttoria.Attivitausuranti = true;
                else if (string.Equals(ddlAttivitaUsuranti.SelectedValue, "NO"))
                    areaLiquidazionePensioneAgo.DatiIstruttoria.Attivitausuranti = false;
            }

            if (!string.IsNullOrEmpty(ddlCodiceDeroga1.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga1 = ddlCodiceDeroga1.SelectedValue;
            if (!string.IsNullOrEmpty(ddlCodiceDeroga2.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga2 = ddlCodiceDeroga2.SelectedValue;
            if (!string.IsNullOrEmpty(ddlCodiceDeroga3.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga3 = ddlCodiceDeroga3.SelectedValue;
            if (!string.IsNullOrEmpty(ddlCodiceDeroga4.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga4 = ddlCodiceDeroga4.SelectedValue;

            if (!string.IsNullOrEmpty(ddlAnnoBancaFideiussione.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.AnnoBancaFideiussoria = CodeUtility.StringToNullableShort(ddlAnnoBancaFideiussione.SelectedValue);
            if (!string.IsNullOrEmpty(ddlProgressivoBancaFideiussione.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.ProgressivoBancaFideiussoria = CodeUtility.StringToNullableByte(ddlProgressivoBancaFideiussione.SelectedValue);

            if (!string.IsNullOrEmpty(ddlRiduzioneAssegno.SelectedValue))
                areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneAssegno = decimal.Parse(ddlRiduzioneAssegno.SelectedValue);

            if (!string.IsNullOrEmpty(txtScadenza.Text))
                areaLiquidazionePensioneAgo.DatiIstruttoria.ScadenzaAssegno = Utility.GetDateFromString(txtScadenza.Text);

            if (!string.IsNullOrEmpty(txtCodicePrepensionamentoEditoria.Text))
            {
                if (areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault())
                    areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoria = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);
                else if (areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault())
                    areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);
                else if (areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                    areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);
                else if (areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
                    areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoriaLetteraB = CodeUtility.StringToNullableShort(txtCodicePrepensionamentoEditoria.Text);
            }

            return areaLiquidazionePensioneAgo.DatiIstruttoria;
        }

        internal void GetCampiCodice(short? codice, List<DecAnagraficaAccordi> listaDecAnagraficaAccordi, out DecAnagraficaAccordi anagraficaAccordi)
        {
            anagraficaAccordi = null;
            if (codice.HasValue)
                anagraficaAccordi = listaDecAnagraficaAccordi.Where(x => x.Codice.HasValue && x.Codice == codice).SingleOrDefault();
        }

        internal void GetCampiCodice(short? codice, List<DecAnagraficaAccordiPerTipo0171> listaDecAnagraficaAccordi, out DecAnagraficaAccordiPerTipo0171 anagraficaAccordi)
        {
            anagraficaAccordi = null;
            if (codice.HasValue)
                anagraficaAccordi = listaDecAnagraficaAccordi.Where(x => x.Codice.HasValue && x.Codice == codice).SingleOrDefault();
        }

        internal void GetCampiCodice(short? codice, List<DecAnagraficaAccordiPerTipo0179> listaDecAnagraficaAccordi, out DecAnagraficaAccordiPerTipo0179 anagraficaAccordi)
        {
            anagraficaAccordi = null;
            if (codice.HasValue)
                anagraficaAccordi = listaDecAnagraficaAccordi.Where(x => x.Codice.HasValue && x.Codice == codice).SingleOrDefault();
        }

        internal void GetCampiCodice(short? codice, List<DecAnagraficaAccordiLetteraB> listaDecAnagraficaAccordi, out DecAnagraficaAccordiLetteraB anagraficaAccordi)
        {
            anagraficaAccordi = null;
            if (codice.HasValue)
                anagraficaAccordi = listaDecAnagraficaAccordi.Where(x => x.Codice.HasValue && x.Codice == codice).SingleOrDefault();
        }

        internal void ValorizzaEtichetteIstruttoria(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

            ViewState[EnumViewState.ListaAziendaEditoria.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.listaAziendaEditoria.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaBancaFideiussione != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaBancaFideiussione.Count() > 0)
                ViewState[EnumViewState.ListaDecodificaBancaFideiussione.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaBancaFideiussione.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordi != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordi.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAccordi.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordi.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziende != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziende.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAziende.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziende.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0171 != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0171.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAccordiPerTipo0171.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0171.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0171 != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0171.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAziendePerTipo0171.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0171.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0179 != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0179.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAccordiPerTipo0179.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0179.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0179 != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0179.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAziendePerTipo0179.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0179.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiLetteraB != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiLetteraB.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAccordiLetteraB.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiLetteraB.ToList();
            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendeLetteraB != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendeLetteraB.Count() > 0)
                ViewState[EnumViewState.ListaDecAnagraficaAziendeLetteraB.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendeLetteraB.ToList();

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if ((liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.Value) ||
                (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80))
                ViewState["RiduzioneRetrib"] = true;

            ViewState[EnumViewState.DataScadenzaAssegno.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null ? liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.ScadenzaAssegno : null;
            ViewState[EnumViewState.IsDataScadenzaAssegnoStoricoValorizzata.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.IsScadenzaStoricoValorizzata.GetValueOrDefault();

            if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaAziendeScadenzaAssegnoGGMMAAAA != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaAziendeScadenzaAssegnoGGMMAAAA.Length > 0)
                ViewState[EnumViewState.ListaAziendeScadenzaAssegnoGGMMAAAA.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaAziendeScadenzaAssegnoGGMMAAAA.ToList();

            ViewState[EnumViewState.IsPrepensionamentoEditoria.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA;
            ViewState[EnumViewState.IsPrepensionamentoEditoriaArt1c154L205_2017.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017;
            ViewState[EnumViewState.IsPrepensionamentoEditoriaArt1c500L160_2019.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019;
            ViewState[EnumViewState.IsPrepensionamentoEditoriaLetteraB.ToString()] = liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA;

            HiddenFieldDecorrenzaOriginaria.Value = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
            HiddenFieldDataNascitaTitolare.Value = string.Format("{0:dd/MM/yyyy}", Anagrafica.DataNascita.Value);

            if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault() ||
                liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault() ||
                liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() ||
                liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
                HiddenFieldIsPrepensionamentoEditoria.Value = "SI";
            else
                HiddenFieldIsPrepensionamentoEditoria.Value = "NO";

            LoadDdl(liquidazioneAgo, datiPensione, this.domanda.Categoria.Trim(), domanda.IsDomandaRiapertura);
            Utility.Categoria? categoria = Utility.GetCategoria(this.domanda.Categoria.Trim());

            ValorizzaHdnAzienda(liquidazioneAgo.areaLiquidazionePensioneAgo.listaAziendaEditoria.ToList());

            RenderControls(liquidazioneAgo.areaLiquidazionePensioneAgo);

            if (Utility.IsDomandaRipristino(datiPensione))
                pnlCodiceRequisitoRidotto.Enabled = false;

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null)
            {
                switch (categoria)
                {
                    case Utility.Categoria.VR:
                    case Utility.Categoria.SR:
                    case Utility.Categoria.IR:
                        {
                            if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceCdCmMr.HasValue)
                                ddlCodCD_CM_MR.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceCdCmMr.Value.ToString();
                            else
                                ddlCodCD_CM_MR.SelectedIndex = 0;
                        }
                        break;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati.HasValue)
                {
                    txtAzienda.Text = GetAzienda(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati.Value);
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) || (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                        (Utility.IsDomandaVESO92(domanda.Categoria.Trim()) || Utility.IsDomandaESPA(domanda.Categoria.Trim()))) || datiPensione.IsDatiAggiuntiviFromJSON.GetValueOrDefault())
                        txtAzienda.Enabled = false;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.AnnoBancaFideiussoria.HasValue)
                {
                    if (ddlAnnoBancaFideiussione.Items.FindByText(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.AnnoBancaFideiussoria.ToString()) != null)
                    {
                        ddlAnnoBancaFideiussione.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.AnnoBancaFideiussoria.ToString();
                        if ((Utility.IsDomandaVESO92(domanda.Categoria.Trim()) || Utility.IsDomandaESPA(domanda.Categoria.Trim()) || Utility.IsDomandaESOPMI(domanda.Categoria.Trim())) && datiPensione.IsDatiAggiuntiviFromJSON.HasValue &&
                            datiPensione.IsDatiAggiuntiviFromJSON.Value)
                            ddlAnnoBancaFideiussione.Enabled = false;
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.ProgressivoBancaFideiussoria.HasValue)
                {
                    if (ddlProgressivoBancaFideiussione.Items.FindByText(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.ProgressivoBancaFideiussoria.ToString()) != null)
                    {
                        ddlProgressivoBancaFideiussione.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.ProgressivoBancaFideiussoria.ToString();
                        if ((Utility.IsDomandaVESO92(domanda.Categoria.Trim()) || Utility.IsDomandaESPA(domanda.Categoria.Trim()) || Utility.IsDomandaESOPMI(domanda.Categoria.Trim())) && datiPensione.IsDatiAggiuntiviFromJSON.HasValue &&
                            datiPensione.IsDatiAggiuntiviFromJSON.Value)
                            ddlProgressivoBancaFideiussione.Enabled = false;
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                    ddlSoggettoDerogato.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceParticolareSoggettoDerogato.Value.ToString();
                else
                    ddlSoggettoDerogato.SelectedIndex = -1;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.AliquotaTFREsodati.HasValue)
                {
                    string aliquotaEsodati = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.AliquotaTFREsodati.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    string[] aleso = aliquotaEsodati.Split(',');
                    txtAliquotaTFREsodatiInt.Text = aleso[0];
                    txtAliquotaTFREsodatiDec.Text = aleso[1];
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.Legge44997.HasValue)
                    ddlCodReqRidotti.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.Legge44997.Value.ToString();
                else
                    ddlCodReqRidotti.SelectedIndex = 0;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneRetributiva)
                    ddlRiduzioneRetributiva.SelectedValue = "SI";
                else ddlRiduzioneRetributiva.SelectedValue = "NO";
                txtRiduzioneRetributiva.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneRetributivaPercentuale.HasValue ? liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.Attivitausuranti.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.Attivitausuranti.Value)
                    ddlAttivitaUsuranti.SelectedValue = "SI";
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.Attivitausuranti.HasValue && !liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.Attivitausuranti.Value)
                    ddlAttivitaUsuranti.SelectedValue = "NO";
                else ddlAttivitaUsuranti.SelectedValue = string.Empty;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS != null)
                {
                    if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga1))
                        ddlCodiceDeroga1.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga1;
                    if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga2))
                        ddlCodiceDeroga2.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga2;
                    if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga3))
                        ddlCodiceDeroga3.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga3;
                    if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga4))
                        ddlCodiceDeroga4.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga4;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneAssegno.HasValue)
                    ddlRiduzioneAssegno.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.RiduzioneAssegno.Value.ToString("N0");

                if (datiPensione.IsDatiAggiuntiviFromJSON.GetValueOrDefault())
                {
                    ddlRiduzioneAssegno.Enabled = false;
                }

                if (Utility.IsDomandaCRED27(this.domanda.Categoria) && !Utility.IsDomandaVOCRED_CRED27_DAP(this.domanda.Categoria, datiPensione.Filtro))
                {
                    if (!liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaRicTrfCred27GestioneL.GetValueOrDefault())
                        ddlRiduzioneAssegno.Enabled = true;
                    else
                        ddlRiduzioneAssegno.Enabled = false;
                }

                if (Utility.IsDomandaVOCRED_CRED27_DAP(this.domanda.Categoria, datiPensione.Filtro) && Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    ddlRiduzioneAssegno.Enabled = false;
                }


                if (Utility.IsDomandaBancari(this.domanda.Categoria) && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati != null)
                {
                    if (ddlCodiceBanca.Items.FindByValue(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati.ToString()) != null)
                    {
                        ddlCodiceBanca.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati.ToString();
                        if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) || Utility.IsDomandaRipristino(datiPensione))
                            ddlCodiceBanca.Enabled = false;
                    }
                }

                if (Utility.IsDomandaPSO(this.domanda.Categoria))
                {
                    //Valorizzaizone codice ente
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceEnte != null && ddlCodiceEnte.Items.FindByValue(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceEnte.ToString()) != null)
                        ddlCodiceEnte.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceEnte.ToString();
                    else
                    {
                        var codiceEnte = Utility.GetCodiceEnteByCertificato(this.domanda.Certificato);
                        ddlCodiceEnte.SelectedValue = !string.IsNullOrEmpty(codiceEnte) ? codiceEnte : "";
                    }
                    ddlCodiceEnte.Enabled = false;
                }
            }

            if (Utility.IsDomandaRipristino(datiPensione) || Utility.IsDomandaRiliquidazione(datiPensione))
            {
                ddlAttivitaUsuranti.Enabled = false;
            }

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault())
                {
                    List<DecAnagraficaAccordi> listaDecAnagraficaAccordi = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordi.ToList();
                    List<DecAnagraficaAziende> listaDecAnagraficaAziende = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziende.ToList();
                    short? codiceAziendaEditoria = null;
                    DecAnagraficaAccordi anagraficaAccordi = new DecAnagraficaAccordi();

                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null)
                    {
                        codiceAziendaEditoria = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoria;

                        if (codiceAziendaEditoria.HasValue)
                        {
                            GetCampiCodice(codiceAziendaEditoria, listaDecAnagraficaAccordi, out anagraficaAccordi);
                            txtCodicePrepensionamentoEditoria.Text = codiceAziendaEditoria.Value.ToString();
                            if (anagraficaAccordi != null)
                            {
                                txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                                if (anagraficaAccordi.DataAccordi.HasValue)
                                    txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                                txtDecreto.Text = anagraficaAccordi.Decreto;
                            }
                        }
                        else
                        {
                            txtDenominazioneAzienda.Text = string.Empty;
                            txtDataAccordi.Text = string.Empty;
                            txtDecreto.Text = string.Empty;
                        }
                    }

                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura))
                    {
                        txtCodicePrepensionamentoEditoria.Visible = true;
                        txtCodicePrepensionamentoEditoria.Enabled = false;
                        btnAggiorna.Visible = true;
                        btnAggiorna.Enabled = false;
                        txtDenominazioneAzienda.Visible = true;
                        lblDenominazioneAzienda.Visible = true;
                        txtDataAccordi.Visible = true;
                        lblDataAccordi.Visible = true;
                        txtDecreto.Visible = true;
                        lblDecreto.Visible = true;
                    }

                    if (CodeUtility.IsRicostituzione(datiPensione) && domanda.Categoria.Trim() == "SO")
                        RFVCodicePrepensionamentoEditoria.Enabled = false;
                }
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault())
                {
                    List<DecAnagraficaAccordiPerTipo0171> listaDecAnagraficaAccordi = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0171.ToList();
                    List<DecAnagraficaAziendePerTipo0171> listaDecAnagraficaAziende = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0171.ToList();
                    short? codiceAziendaEditoria = null;
                    DecAnagraficaAccordiPerTipo0171 anagraficaAccordi = new DecAnagraficaAccordiPerTipo0171();

                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null)
                    {
                        codiceAziendaEditoria = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171;

                        if (codiceAziendaEditoria.HasValue)
                        {
                            GetCampiCodice(codiceAziendaEditoria, listaDecAnagraficaAccordi, out anagraficaAccordi);
                            txtCodicePrepensionamentoEditoria.Text = codiceAziendaEditoria.Value.ToString();
                            if (anagraficaAccordi != null)
                            {
                                txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                                txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                            }
                        }
                        else
                        {
                            txtDenominazioneAzienda.Text = string.Empty;
                            txtDataAccordi.Text = string.Empty;
                            txtDecreto.Text = string.Empty;
                        }
                    }

                    txtDecreto.Visible = false;
                    lblDecreto.Visible = false;

                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura))
                    {
                        txtCodicePrepensionamentoEditoria.Visible = true;
                        txtCodicePrepensionamentoEditoria.Enabled = false;
                        btnAggiorna.Visible = false;
                        txtDenominazioneAzienda.Visible = false;
                        lblDenominazioneAzienda.Visible = false;
                        txtDataAccordi.Visible = false;
                        lblDataAccordi.Visible = false;
                    }
                }
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                {
                    List<DecAnagraficaAccordiPerTipo0179> listaDecAnagraficaAccordi = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiPerTipo0179.ToList();
                    List<DecAnagraficaAziendePerTipo0179> listaDecAnagraficaAziende = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendePerTipo0179.ToList();
                    short? codiceAziendaEditoria = null;
                    DecAnagraficaAccordiPerTipo0179 anagraficaAccordi = new DecAnagraficaAccordiPerTipo0179();

                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null)
                    {
                        codiceAziendaEditoria = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179;

                        if (codiceAziendaEditoria.HasValue)
                        {
                            GetCampiCodice(codiceAziendaEditoria, listaDecAnagraficaAccordi, out anagraficaAccordi);
                            txtCodicePrepensionamentoEditoria.Text = codiceAziendaEditoria.Value.ToString();
                            if (anagraficaAccordi != null)
                            {
                                txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                                txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                            }
                        }
                        else
                        {
                            txtDenominazioneAzienda.Text = string.Empty;
                            txtDataAccordi.Text = string.Empty;
                            txtDecreto.Text = string.Empty;
                        }
                    }

                    txtDecreto.Visible = false;
                    lblDecreto.Visible = false;

                    if (CodeUtility.IsRicostituzione(datiPensione))
                    {
                        txtCodicePrepensionamentoEditoria.Enabled = false;
                        btnAggiorna.Visible = false;
                    }

                    if (domanda.IsDomandaRiapertura)
                    {
                        txtCodicePrepensionamentoEditoria.Visible = true;
                        txtCodicePrepensionamentoEditoria.Enabled = false;
                        btnAggiorna.Visible = false;
                        txtDenominazioneAzienda.Visible = false;
                        lblDenominazioneAzienda.Visible = false;
                        txtDataAccordi.Visible = false;
                        lblDataAccordi.Visible = false;
                    }
                }
                else if (liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
                {
                    List<DecAnagraficaAccordiLetteraB> listaDecAnagraficaAccordi = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAccordiLetteraB.ToList();
                    List<DecAnagraficaAziendeLetteraB> listaDecAnagraficaAziende = liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecAnagraficaAziendeLetteraB.ToList();
                    short? codiceAziendaEditoria = null;
                    DecAnagraficaAccordiLetteraB anagraficaAccordi = new DecAnagraficaAccordiLetteraB();

                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null)
                    {
                        codiceAziendaEditoria = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceAziendaEditoriaLetteraB;

                        if (codiceAziendaEditoria.HasValue)
                        {
                            GetCampiCodice(codiceAziendaEditoria, listaDecAnagraficaAccordi, out anagraficaAccordi);
                            txtCodicePrepensionamentoEditoria.Text = codiceAziendaEditoria.Value.ToString();
                            if (anagraficaAccordi != null)
                            {
                                txtDenominazioneAzienda.Text = listaDecAnagraficaAziende.Find(x => x.Id == anagraficaAccordi.DenominazioneAzienda.GetValueOrDefault()).DenominazioneAzienda;
                                if (anagraficaAccordi.DataAccordi.HasValue)
                                    txtDataAccordi.Text = anagraficaAccordi.DataAccordi.Value.ToString("dd/MM/yyyy");
                                txtDecreto.Text = anagraficaAccordi.Decreto;
                            }
                        }
                        else
                        {
                            txtDenominazioneAzienda.Text = string.Empty;
                            txtDataAccordi.Text = string.Empty;
                            txtDecreto.Text = string.Empty;
                        }
                    }

                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura))
                    {
                        txtCodicePrepensionamentoEditoria.Visible = true;
                        txtCodicePrepensionamentoEditoria.Enabled = false;
                        btnAggiorna.Visible = true;
                        btnAggiorna.Enabled = false;
                        txtDenominazioneAzienda.Visible = true;
                        lblDenominazioneAzienda.Visible = true;
                        txtDataAccordi.Visible = true;
                        lblDataAccordi.Visible = true;
                        txtDecreto.Visible = true;
                        lblDecreto.Visible = true;
                    }

                    if (CodeUtility.IsRicostituzione(datiPensione) && domanda.Categoria.Trim() == "SO")
                        RFVCodicePrepensionamentoEditoria.Enabled = false;
                }

                if ((this.domanda.Categoria.Trim() == "VO" || this.domanda.Categoria.Trim() == "VOPGI") && Utility.IsDomandaManualeInvaliditaOver80(datiPensione))
                {
                    ddlCodReqRidotti.SelectedValue = "6";
                    ddlCodReqRidotti.Enabled = false;
                }
            }

            if (Utility.IsDomandaVOCRED_CRED27_DAP(this.domanda.Categoria, datiPensione.Filtro) && Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                ddlRiduzioneAssegno.Enabled = false;
            }


            //Gestione blocco CodiceAzienda per domande provenienti da Patronato per VESO33 e VESO99
            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
                ManageAzienda(liquidazioneAgo.areaLiquidazionePensioneAgo.CodiceAziendaFromPatronato, this.domanda);

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
                ManageGestioneAPESociale(this.domanda, datiPensione);

            ManageRiduzioneRetributiva(liquidazioneAgo);
            MagageAliquotaTfrEsodati(liquidazioneAgo.areaLiquidazionePensioneAgo.IsAliquotaTfrEsodati.Value, datiPensione);
            if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value)
                GestioneEtichetteIsUnicarpe(datiPensione, liquidazioneAgo);
            if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (Utility.IsDomandaVOCRED(this.domanda.Categoria) || Utility.IsDomandaVOCOOP(this.domanda.Categoria) || (Utility.IsDomandaVOESO(this.domanda.Categoria) && (Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(datiPensione) || Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione)))))
                ddlSoggettoDerogato.Enabled = true;
            ManageCodiciDerogaENPALS(liquidazioneAgo);

            ValorizzaBancaFideiussione();

            if (IsPostBack)
                ViewState[EnumViewState.DataScadenzaAssegno.ToString()] = Utility.GetDateFromString(txtScadenza.Text);
            SelezionaTipoCalendarioPerScadenzaAssegno();

            if (CodeUtility.IsRicostituzione(datiPensione) && liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                ddlCodCD_CM_MR.Enabled = false;
                ddlCodReqRidotti.Enabled = false;
                ddlCodiceBanca.Enabled = false;
                ddlSoggettoDerogato.Enabled = false;
                txtAzienda.Enabled = false;
                txtAliquotaTFREsodatiInt.Enabled = false;
                ddlAttivitaUsuranti.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
                pnlPrepensionamentoEditoria.Enabled = false;
                pnlCodiciDerogaENPLAS.Enabled = false;
                ddlRiduzioneAssegno.Enabled = false;
                pnlBancaFideiussione.Enabled = false;
                pnlScadenzaAssegno.Enabled = false;
                ddlCodiceEnte.Enabled = false;
            }

            //ENG - CRED27 E COOP28
            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) && (Utility.IsDomandaCOOP28(this.domanda.Categoria) || Utility.IsDomandaCRED27(this.domanda.Categoria)))
            {
                txtAliquotaTFREsodatiInt.Enabled = false;
                txtAliquotaTFREsodatiDec.Enabled = false;
            }

            //ENG - RIC VOPGI filtro L80
            if (Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria) && datiPensione.IdTipoPLPerRIC.HasValue && datiPensione.IdTipoPLPerRIC == (byte)Utility.TipoPLPerRIC.RicVOPGIFiltroL80)
            {
                ddlCodReqRidotti.SelectedValue = "6";
                ddlCodReqRidotti.Enabled = false;
            }
        }


        private string GetAziendaFromTraduzioneSuGp(string traduzioneSuGp)
        {
            List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda> listaAziendaEditoria = (List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda>)ViewState[EnumViewState.ListaAziendaEditoria.ToString()];
            string sAziendaEditoria = string.Empty;
            if (!string.IsNullOrEmpty(traduzioneSuGp))
            {
                Presenter.SvrLiquidazioneAgo.DecodificaAzienda aziendaEditoria = listaAziendaEditoria.Find((code) => (code.TraduzioneSuGP == traduzioneSuGp));
                sAziendaEditoria = GetItemAutocompleteAzienda(aziendaEditoria);
            }
            return sAziendaEditoria;
        }



        internal bool ManageButtonRiduzioneRetributiva(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            AreaTitolare.DatiPensione datiPensione = this.GetDatiPensione(this);
            if (titolare != null)
            {
                if (titolare.DataNascita.HasValue)
                {
                    DateTime? dataRiferimento = datiPensione.DecorrenzaOriginaria;
                    if (Utility.IsDomandaVOCRED(this.domanda.Categoria) ||
                        (Utility.IsDomandaVOESO(this.domanda.Categoria) && ((!CodeUtility.IsRicostituzione(datiPensione) && Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione) && datiPensione.CodiceTipoRichiesta == "74") || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null && Utility.IsDomandaVOESOFerrovieDelloStatoRicConFiltro(this.domanda.Categoria, this.domanda.GP2BB05, liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP)))))
                    {
                        HiddenDataTitolareAdd62.Value = string.Format("{0:dd/MM/yyyy}", titolare.DataNascita.Value.AddYears(62));
                        if ((liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.Value))
                        {
                            //la decorrenza pensione deve cadere nell’intervallo [01/2012 - 12/2014]
                            if (!(dataRiferimento.HasValue && Utility.DataSuccessivaA(dataRiferimento.Value, new DateTime(2012, 1, 1)) && !Utility.DataStrettamenteSuccessivaA(dataRiferimento.Value, new DateTime(2014, 12, 31))))
                            {
                                return false;
                            }
                            //la data di perfezionamento dei requisiti deve essere strettamente inferiore a 01/2015
                            if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2015, 1, 1)))
                                return false;

                            //primo byte codice natura deve essere 1 (gestito lato javascript)
                            return true;
                        }
                    }
                    if (dataRiferimento.HasValue)
                    {
                        if (!(DateTime.Compare(titolare.DataNascita.Value.AddYears(62), dataRiferimento.Value) < 0) &&
                            (liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ManageRiduzioneRetributiva(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            bool? isPrepensionamentoEditoria = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoria.ToString()];
            bool? isPrepensionamentoEditoriaLetteraB = (bool?)ViewState[EnumViewState.IsPrepensionamentoEditoriaLetteraB.ToString()];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (((bool?)ViewState["RiduzioneRetrib"]).HasValue && ((bool?)ViewState["RiduzioneRetrib"]).Value)
            {
                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

                pnlRiduzioneRetributiva.Visible = true;

                bool IsRiduzionePresent = ManageButtonRiduzioneRetributiva(liquidazioneAgo);
                //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
                if (IsRiduzionePresent && liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null &&
                    ((liquidazioneAgo.areaLiquidazionePensioneAgo.IsUsuranti.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.IsUsuranti.Value) ||
                    (liquidazioneAgo.areaLiquidazionePensioneAgo.TipologiaSalvaguardia.HasValue) ||
                    (Utility.IsDomandaVOMIN(this.domanda.Categoria) && Utility.IsDomandaAnzianitaAnticipata(datiPensione)) ||
                    // tipo calcolo contributivo (gestito lato Javascript)
                    (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1))))
                    )
                    IsRiduzionePresent = false;

                btnSalvaIstruttoriaNoRiduzione.Visible = !IsRiduzionePresent;
                btnPopUp.Visible = IsRiduzionePresent;
                btnSalvaIstruttoria.Visible = IsRiduzionePresent;

                if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetributivaEnabled.HasValue &&
                    !liquidazioneAgo.areaLiquidazionePensioneAgo.IsRiduzioneRetributivaEnabled.Value &&
                    tipologiaTipoPensione != CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 &&
                    tipologiaTipoPensione != CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80)
                {
                    ddlRiduzioneRetributiva.Enabled = false;
                    txtRiduzioneRetributiva.Enabled = false;
                }

                if ((tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80))
                {
                    ddlRiduzioneRetributiva.Enabled = false;
                    ddlRiduzioneRetributiva.SelectedValue = "NO";
                    txtRiduzioneRetributiva.Enabled = false;
                }

                //VOCRED CRED27 VOCOOP COOP28
                if (Utility.IsDomandaCRED27(domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(domanda.Categoria) || Utility.IsDomandaSPED(domanda.Categoria) || Utility.IsDomandaPSO(domanda.Categoria) || Utility.IsDomandaPMO(domanda.Categoria))
                {
                    ddlRiduzioneRetributiva.Enabled = false;
                    ddlRiduzioneRetributiva.SelectedValue = "NO";
                    txtRiduzioneRetributiva.Enabled = false;
                    txtRiduzioneRetributiva.Text = string.Empty;
                }
                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 1, 1)) && !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2014, 12, 31))
                   && (Utility.IsDomandaVOCRED(this.domanda.Categoria) || (Utility.IsDomandaVOESO(this.domanda.Categoria) && Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione) && datiPensione.CodiceTipoRichiesta == "74")))
                {
                    ddlRiduzioneRetributiva.Enabled = true;
                    txtRiduzioneRetributiva.Enabled = true;
                }

                if (CodeUtility.IsRicostituzione(datiPensione) && liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                {
                    ddlRiduzioneRetributiva.Enabled = false;
                    txtRiduzioneRetributiva.Enabled = false;
                }
            }
            else
            {
                pnlRiduzioneRetributiva.Visible = false;
                btnSalvaIstruttoriaNoRiduzione.Visible = true;
                btnPopUp.Visible = false;
                btnSalvaIstruttoria.Visible = false;
            }

            if (isPrepensionamentoEditoria.GetValueOrDefault() || isPrepensionamentoEditoriaLetteraB.GetValueOrDefault())
            {
                pnlRiduzioneRetributiva.Visible = false;
            }
        }

        private void ManageCodiciDerogaENPALS(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaENPALS)
            {
                pnlCodiciDerogaENPLAS.Visible = true;
                if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null &&
                    liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS != null)
                {
                    ddlCodiceDeroga1.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga1;
                    ddlCodiceDeroga2.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga2;
                    ddlCodiceDeroga3.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga3;
                    ddlCodiceDeroga4.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.DatiENPALS.CodiceDeroga4;
                }
            }
        }

        private void ManageAzienda(string codiceAziendaFromPatronato, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
        {
            if (!Utility.IsDomandaVESO33(domanda.Categoria) && !Utility.IsDomandaVESO92(domanda.Categoria) &&
                !Utility.IsDomandaVOESO(domanda.Categoria) && !Utility.IsDomandaVOCOOP_COOP28(domanda.Categoria) && !Utility.IsDomandaVESO29(domanda.Categoria) &&
                !Utility.IsDomandaVOCRED_CRED27(domanda.Categoria) && !Utility.IsDomandaESOTEL(domanda.Categoria) && !Utility.IsDomandaESOAMB(domanda.Categoria) &&
                !Utility.IsDomandaESPA(domanda.Categoria))
                return;
            if (!string.IsNullOrEmpty(codiceAziendaFromPatronato) && string.IsNullOrEmpty(txtAzienda.Text))
            {
                string sAzienda = GetAziendaFromTraduzioneSuGp(codiceAziendaFromPatronato);
                if (!string.IsNullOrEmpty(sAzienda))
                {
                    //se l'ho trovato lo blocco replicando il comportamento del migrato
                    txtAzienda.Text = sAzienda;
                    txtAzienda.Enabled = false;
                }
            }
        }

        private void ManageGestioneAPESociale(AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaAPESociale(domanda.Categoria))
            {
                Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                bool? isDataScadenzaAssegnoStoricoValorizzata = (bool?)ViewState[EnumViewState.IsDataScadenzaAssegnoStoricoValorizzata.ToString()];

                if (tipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                {
                    lblIstruttoriaAPESociale.Visible = true;
                }
                if (CodeUtility.IsRicostituzione(datiPensione) && !isDataScadenzaAssegnoStoricoValorizzata.GetValueOrDefault())
                {
                    lblScadenzaIndennitaAPESociale.Visible = true;
                }

                pnlScadenzaAssegno.Visible = true;
                pnlCodiceRequisitoRidotto.Visible = false;

                lblTextScadenzaAssegno.Text = "Data Scadenza Indennità:";
            }
        }

        private void LoadDdl(ILiquidazionePensioneAgo liquidazioneAgo, AreaTitolare.DatiPensione datiPensione, string categoria, bool isRiaperturaDomanda)
        {
            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaCDCMMR != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaCDCMMR.Count() > 0)
                {
                    ddlCodCD_CM_MR.Items.Clear();
                    CodeUtility.SetValueDdl(ddlCodCD_CM_MR, string.Empty, string.Empty, string.Empty);
                    foreach (CDCMMR codeCDCMMR in liquidazioneAgo.areaLiquidazionePensioneAgo.listaCDCMMR)
                    {
                        CodeUtility.SetValueDdl(ddlCodCD_CM_MR, codeCDCMMR.Descrizione, codeCDCMMR.Descrizione, codeCDCMMR.Id.ToString());
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaLegge44997 != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaLegge44997.Count() > 0)
                {
                    ddlCodReqRidotti.Items.Clear();
                    CodeUtility.SetValueDdl(ddlCodReqRidotti, string.Empty, string.Empty, string.Empty);
                    foreach (DecodificaLegge44997 codeLegge44997 in liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaLegge44997)
                    {
                        CodeUtility.SetValueDdl(ddlCodReqRidotti, codeLegge44997.Descrizione, codeLegge44997.Id.ToString());
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare.Count() > 0 &&
                    ((liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                    || Utility.IsDomandaVOCRED(categoria) || Utility.IsDomandaVOCOOP(categoria) ||
                    (Utility.IsDomandaVOESO(categoria) && (Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(datiPensione) || Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione) || IsVOESORicErarialiOrFerrovie(liquidazioneAgo, datiPensione, isRiaperturaDomanda))))
                    )
                {
                    pnlSoggettoDerogato.Visible = true;
                    ddlSoggettoDerogato.Items.Clear();
                    CodeUtility.SetValueDdl(ddlSoggettoDerogato, string.Empty, string.Empty, string.Empty);
                    List<CodiceParticolare> listaCodiceParticolare = new List<CodiceParticolare>();
                    if ((Utility.IsDomandaVOCRED(categoria) || Utility.IsDomandaVOCOOP(categoria) || (Utility.IsDomandaVOESO(categoria) && (Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(datiPensione) || Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione)))) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    {
                        if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2007, 06, 01)))
                        {
                            listaCodiceParticolare = liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare.Where(x => x.TraduzioneSuGp == '5' || x.TraduzioneSuGp == '1').ToList();
                        }
                        else if (Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2007, 06, 01)) && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2008, 11, 01)))
                        {
                            listaCodiceParticolare = liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare.Where(x => x.TraduzioneSuGp == '5').ToList();
                        }
                        else if (Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2008, 11, 01)))
                        {
                            listaCodiceParticolare = liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare.Where(x => x.TraduzioneSuGp == '6').ToList();
                        }
                    }
                    else
                    {
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare != null)
                            listaCodiceParticolare = liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare.ToList();
                    }
                    foreach (CodiceParticolare codeParticolare in listaCodiceParticolare)
                    {
                        CodeUtility.SetValueDdl(ddlSoggettoDerogato, (codeParticolare.TraduzioneSuGp.HasValue ? codeParticolare.TraduzioneSuGp.Value.ToString() : string.Empty) +
                            " - " + codeParticolare.Descrizione, codeParticolare.Id.ToString());
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaDerogaENPALS != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaDerogaENPALS.Count() > 0)
                {
                    ddlCodiceDeroga1.Items.Clear();
                    ddlCodiceDeroga2.Items.Clear();
                    ddlCodiceDeroga3.Items.Clear();
                    ddlCodiceDeroga4.Items.Clear();
                    CodeUtility.SetValueDdl(ddlCodiceDeroga1, string.Empty, string.Empty, string.Empty);
                    CodeUtility.SetValueDdl(ddlCodiceDeroga2, string.Empty, string.Empty, string.Empty);
                    CodeUtility.SetValueDdl(ddlCodiceDeroga3, string.Empty, string.Empty, string.Empty);
                    CodeUtility.SetValueDdl(ddlCodiceDeroga4, string.Empty, string.Empty, string.Empty);
                    foreach (DecodificaDerogaENPALS codiceDeroga in liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaDerogaENPALS)
                    {
                        CodeUtility.SetValueDdl(ddlCodiceDeroga1, codiceDeroga.Codice, codiceDeroga.Descrizione, codiceDeroga.Codice);
                        CodeUtility.SetValueDdl(ddlCodiceDeroga2, codiceDeroga.Codice, codiceDeroga.Descrizione, codiceDeroga.Codice);
                        CodeUtility.SetValueDdl(ddlCodiceDeroga3, codiceDeroga.Codice, codiceDeroga.Descrizione, codiceDeroga.Codice);
                        CodeUtility.SetValueDdl(ddlCodiceDeroga4, codiceDeroga.Codice, codiceDeroga.Descrizione, codiceDeroga.Codice);
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaBancaFideiussione != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaBancaFideiussione.Count() > 0)
                {
                    short? codiceBancaEsodati = null;

                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null)
                        codiceBancaEsodati = GetAziendaTraduzioneSuGP(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodati);
                    if (!codiceBancaEsodati.HasValue && !string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.CodiceAziendaFromPatronato))
                        codiceBancaEsodati = CodeUtility.StringToNullableShort(liquidazioneAgo.areaLiquidazionePensioneAgo.CodiceAziendaFromPatronato);

                    List<DecBancaFideiussione> decBancaFideiussioneFilterAnno = null;
                    List<DecBancaFideiussione> listaDecBancaFideiussioneFilterProgressivo = null;
                    LoadDdlAnnoBancaFideiussione(codiceBancaEsodati, liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaBancaFideiussione.ToList(), out decBancaFideiussioneFilterAnno);

                    LoadDdlProgressivoBancaFideiussione(
                        liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null ? liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.AnnoBancaFideiussoria : null,
                        decBancaFideiussioneFilterAnno, out listaDecBancaFideiussioneFilterProgressivo);

                    ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterAnno.ToString()] = decBancaFideiussioneFilterAnno;
                    ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterProgressivo.ToString()] = listaDecBancaFideiussioneFilterProgressivo;
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecodificaBanchePerSede != null && liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecodificaBanchePerSede.Count() > 0)
                {
                    LoadDdlBancaPerSede(this.domanda.Sede, liquidazioneAgo.areaLiquidazionePensioneAgo.ListaDecodificaBanchePerSede.ToList());
                }

            }
        }

        private bool IsVOESORicErarialiOrFerrovie(ILiquidazionePensioneAgo liquidazioneAgo, AreaTitolare.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (Utility.IsDomandaVOESO(this.domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP != null &&
                ((Convert.ToInt32(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP) >= 900 && Convert.ToInt32(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP) <= 1000) || (Convert.ToInt32(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP) >= 601 && Convert.ToInt32(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP) <= 799)))
                return true;
            return false;
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneAgo liquidazioneAgo)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                ddlCodReqRidotti.Enabled = false;
                ddlSoggettoDerogato.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
            else if (tipoUnicarpe == Utility.TipoUnicarpe.Manuale && liquidazioneAgo != null &&
                liquidazioneAgo.areaLiquidazionePensioneAgo != null && ((liquidazioneAgo.areaLiquidazionePensioneAgo.TipologiaSalvaguardia.HasValue) ||
                (liquidazioneAgo.areaLiquidazionePensioneAgo.IsUsuranti.HasValue && liquidazioneAgo.areaLiquidazionePensioneAgo.IsUsuranti.Value)))
            {
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
        }

        private short? ControlAzienda(string aziendaInserita)
        {
            List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda> listaAziendaEditoria = (List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda>)ViewState[EnumViewState.ListaAziendaEditoria.ToString()];
            short? codAzienda = null;
            string sTradSuGpAzienda = aziendaInserita.IndexOf('-') >= 1 ? aziendaInserita.Substring(0, aziendaInserita.IndexOf('-') - 1).Trim() : string.Empty;
            Presenter.SvrLiquidazioneAgo.DecodificaAzienda codeAziendaEditoria = listaAziendaEditoria.Find(x => x.TraduzioneSuGP.Trim() == sTradSuGpAzienda.Trim());
            if (codeAziendaEditoria != null)
                codAzienda = codeAziendaEditoria.Id;
            return codAzienda;
        }

        private string GetAzienda(short? codAzienda)
        {
            List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda> listaAziendaEditoria = (List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda>)ViewState[EnumViewState.ListaAziendaEditoria.ToString()];
            string sAziendaEditoria = string.Empty;
            if (codAzienda.HasValue)
            {
                Presenter.SvrLiquidazioneAgo.DecodificaAzienda aziendaEditoria = listaAziendaEditoria.Find((code) => (code.Id == codAzienda.Value));
                sAziendaEditoria = GetItemAutocompleteAzienda(aziendaEditoria);
            }
            return sAziendaEditoria;
        }

        private short? GetAziendaTraduzioneSuGP(short? codAzienda)
        {
            List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda> listaAziendaEditoria = (List<Presenter.SvrLiquidazioneAgo.DecodificaAzienda>)ViewState[EnumViewState.ListaAziendaEditoria.ToString()];
            short? sAziendaEditoria = null;
            if (codAzienda.HasValue)
            {
                Presenter.SvrLiquidazioneAgo.DecodificaAzienda aziendaEditoria = listaAziendaEditoria.Find((code) => (code.Id == codAzienda.Value));
                if (aziendaEditoria != null)
                    sAziendaEditoria = CodeUtility.StringToNullableShort(aziendaEditoria.TraduzioneSuGP);
            }
            return sAziendaEditoria;
        }

        private short? GetBancaPerSedeTraduzioneSuGP(short? codAzienda, Presenter.SvrLiquidazioneAgo.DecodificaBanchePerSede[] ListaDecodificaBanchePerSede)
        {
            short? sAziendaEditoria = null;
            var listaDec = ListaDecodificaBanchePerSede.ToList();
            var aziendaEditoria = listaDec.Find((code) => (code.Id == codAzienda.Value));
            if (aziendaEditoria != null)
                sAziendaEditoria = CodeUtility.StringToNullableShort(aziendaEditoria.TraduzioneSuGP);

            return sAziendaEditoria;
        }

        private string GetItemAutocompleteAzienda(Presenter.SvrLiquidazioneAgo.DecodificaAzienda dec)
        {
            string ret = string.Empty;
            if (dec != null)
                ret = string.Format("{0} - {1}", dec.TraduzioneSuGP, dec.Descrizione.Trim());
            return ret;
        }

        private void ValorizzaHdnAzienda(List<DecodificaAzienda> lstDecAzienda)
        {
            string elencoAziende = string.Empty;
            HiddenFieldAziende.Value = string.Empty;

            foreach (DecodificaAzienda codeAziendaEditoria in lstDecAzienda)
                elencoAziende = string.Concat(elencoAziende, ";", GetItemAutocompleteAzienda(codeAziendaEditoria));

            HiddenFieldAziende.Value = elencoAziende;
        }

        private void MagageAliquotaTfrEsodati(bool isVisible, AreaTitolare.DatiPensione datiPensione)
        {
            pnlAliquotaTfrEsodati.Visible = isVisible;
            if ((CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) && (Utility.IsDomandaVOCOOP(this.domanda.Categoria) || Utility.IsDomandaVOCRED(this.domanda.Categoria)))
                || datiPensione.IsDatiAggiuntiviFromJSON.GetValueOrDefault())
            {
                txtAliquotaTFREsodatiInt.Enabled = false;
                txtAliquotaTFREsodatiDec.Enabled = false;
            }
        }

        private void RenderControls(AreaLiquidazionePensione areaLiquidazionePensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(this.TitolarePensione.Pensione.CodeGruppo, this.TitolarePensione.Pensione.CodeProdotto, this.TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            Utility.Categoria? categoria = Utility.GetCategoria(this.domanda.Categoria.Trim());

            switch (categoria)
            {
                case Utility.Categoria.VR:
                case Utility.Categoria.SR:
                case Utility.Categoria.IR:
                    lblCodCD_CM_MR.Visible = true;
                    ddlCodCD_CM_MR.Visible = true;
                    break;
                case Utility.Categoria.IOAUT:
                case Utility.Categoria.SOAUT:
                case Utility.Categoria.VOAUT:
                    ddlAttivitaUsuranti.Enabled = false;
                    break;
                case Utility.Categoria.VESO33:
                case Utility.Categoria.VOCOOP:
                case Utility.Categoria.COOP28:
                case Utility.Categoria.VESO29:
                case Utility.Categoria.VOESO:
                case Utility.Categoria.ESOTEL:
                case Utility.Categoria.ESOAMB:
                    pnlCodiceRequisitoRidotto.Visible = false;
                    pnlScadenzaAssegno.Visible = true;
                    break;
                case Utility.Categoria.VESO92:
                case Utility.Categoria.ESPA:
                    pnlCodiceRequisitoRidotto.Visible = false;
                    pnlBancaFideiussione.Visible = true;
                    if (CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura))
                        pnlBancaFideiussione.Enabled = false;
                    pnlScadenzaAssegno.Visible = true;
                    break;
                case Utility.Categoria.VOCRED:
                case Utility.Categoria.CRED27:
                    pnlCodiceRequisitoRidotto.Visible = false;
                    pnlRiduzioneAssegno.Visible = true;
                    pnlScadenzaAssegno.Visible = true;
                    break;
            }

            if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                areaLiquidazionePensione.IsSperimentaleDonna.GetValueOrDefault() || areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() ||
                this.domanda.IsDomandaINPDAP || areaLiquidazionePensione.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault() || areaLiquidazionePensione.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
            {
                pnlCodiceRequisitoRidotto.Visible = false;
            }

            CodeUtility.DisableEliminaForRicostituzioni(btnEliminaIstruttoria);
            if (Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)))
            {
                btnEliminaIstruttoria.Enabled = false;
            }

            //ENG - Integrazione Modifiche Accenture
            if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            {
                btnEliminaIstruttoria.Enabled = false;
                btnSalvaIstruttoria.Enabled = false;
                btnSalvaIstruttoriaNoRiduzione.Enabled = false;
            }

            if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            {
                btnEliminaIstruttoria.Enabled = false;
                btnSalvaIstruttoria.Enabled = false;
                btnSalvaIstruttoriaNoRiduzione.Enabled = false;
            }

            List<DecBancaFideiussione> listaDecodificaBancaFideiussioneFilterAnno = (List<DecBancaFideiussione>)ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterAnno.ToString()];
            if ((listaDecodificaBancaFideiussioneFilterAnno != null && !listaDecodificaBancaFideiussioneFilterAnno.Exists(x => x.Anno.HasValue)) ||
                ((Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)) && this.TitolarePensione.Pensione.IsDatiAggiuntiviFromJSON.HasValue &&
                this.TitolarePensione.Pensione.IsDatiAggiuntiviFromJSON.Value))
            {
                if (listaDecodificaBancaFideiussioneFilterAnno != null && !listaDecodificaBancaFideiussioneFilterAnno.Exists(x => x.Anno.HasValue))
                    lblNoPianoEsodo.Visible = true;
                btnAggiornaAnnoBancaFideiussione.Enabled = false;
                btnAggiornaProgressivoBancaFideiussione.Enabled = false;
            }
            else
            {
                lblNoPianoEsodo.Visible = false;
                btnAggiornaAnnoBancaFideiussione.Enabled = true;
            }

            List<DecBancaFideiussione> listaDecodificaBancaFideiussioneFilterProgressivo = (List<DecBancaFideiussione>)ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterProgressivo.ToString()];
            if (listaDecodificaBancaFideiussioneFilterProgressivo == null ||
                (listaDecodificaBancaFideiussioneFilterProgressivo != null && !listaDecodificaBancaFideiussioneFilterProgressivo.Exists(x => x.Progressivo.HasValue)) ||
                ((Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)) && this.TitolarePensione.Pensione.IsDatiAggiuntiviFromJSON.HasValue &&
                this.TitolarePensione.Pensione.IsDatiAggiuntiviFromJSON.Value))
                btnAggiornaProgressivoBancaFideiussione.Enabled = false;
            else
                btnAggiornaProgressivoBancaFideiussione.Enabled = true;

            if ((this.domanda.IsDomandaENPALS && (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Supplementare || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Invalidita_Supplementare ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Reversibilita_Supplementare || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Indiretta_Supplementare ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_InvaliditaAssegno_Ordinario)) ||
                areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault() || CodeUtility.IsDomandaAUT(this.domanda.Categoria) ||
                Utility.IsDomandaSPED(this.domanda.Categoria) || Utility.IsDomandaVOMIN_SOMIN(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria) || Utility.IsDomandaPescatori(this.domanda.Categoria))
            {
                ddlCodReqRidotti.Enabled = false;
                if (!Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                {
                    if (Utility.IsDomandaPescatoriFiltroL80(this.TitolarePensione.Pensione, this.domanda.Categoria))
                        ddlCodReqRidotti.SelectedIndex = 6;
                    else
                        ddlCodReqRidotti.SelectedIndex = 0;
                }
            }

            if (areaLiquidazionePensione.IsPrepensionamentoEditoriaFiltroEAA.GetValueOrDefault() ||
                areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault() ||
                areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() ||
                areaLiquidazionePensione.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
            {
                pnlPrepensionamentoEditoria.Visible = true;
                if (areaLiquidazionePensione.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                    lblCodice.InnerText = "Codice Accordo";
                if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                {
                    txtCodicePrepensionamentoEditoria.Enabled = false;
                    btnAggiorna.Enabled = false;
                }
                txtDenominazioneAzienda.Enabled = false;
                txtDataAccordi.Enabled = false;
                txtDecreto.Enabled = false;
            }

            if (this.domanda.IsDomandaENPALS && CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura), this.TitolarePensione.Pensione.IsDatiENPALSRecuperati))
            {
                ddlCodiceDeroga1.Enabled = true;
                ddlCodiceDeroga2.Enabled = true;
                ddlCodiceDeroga3.Enabled = true;
                ddlCodiceDeroga4.Enabled = true;
            }

            if (Utility.IsDomandaBancari(this.domanda.Categoria))
            {
                pnlCodiceRequisitoRidotto.Visible = false;
                pnlBancari.Visible = true;
            }

            if (Utility.IsDomandaPSO(this.domanda.Categoria))
            {
                pnlCodiceEntePSO.Visible = true;
                pnlCodiceRequisitoRidotto.Visible = false;
            }
        }

        private void LoadDdlAnnoBancaFideiussione(short? codiceBancaEsodati, List<DecBancaFideiussione> listaDecodificaBancaFideiussione, out List<DecBancaFideiussione> decBancaFideiussioneFilterAnno)
        {
            decBancaFideiussioneFilterAnno = null;
            ddlAnnoBancaFideiussione.Items.Clear();
            CodeUtility.SetValueDdl(ddlAnnoBancaFideiussione, string.Empty, string.Empty, string.Empty);
            CleanBancaFideiussione();

            if (codiceBancaEsodati.HasValue && listaDecodificaBancaFideiussione != null && listaDecodificaBancaFideiussione.Count > 0)
            {
                decBancaFideiussioneFilterAnno = listaDecodificaBancaFideiussione.
                    Where(x => x.CodiceAzienda == codiceBancaEsodati.ToString()).ToList();
                if (decBancaFideiussioneFilterAnno != null && decBancaFideiussioneFilterAnno.Count > 0)
                    foreach (DecBancaFideiussione decBancaFideiussione in decBancaFideiussioneFilterAnno)
                        if (decBancaFideiussione.Anno.HasValue && ddlAnnoBancaFideiussione.Items.FindByValue(decBancaFideiussione.Anno.Value.ToString()) == null)
                            CodeUtility.SetValueDdl(ddlAnnoBancaFideiussione, decBancaFideiussione.Anno.ToString(), decBancaFideiussione.Anno.ToString(), decBancaFideiussione.Anno.ToString());
            }
        }

        private void LoadDdlBancaPerSede(string sede, List<DecodificaBanchePerSede> listaDecodificaBancaPerSede)
        {
            ddlCodiceBanca.Items.Clear();
            CodeUtility.SetValueDdl(ddlCodiceBanca, string.Empty, string.Empty, string.Empty);

            if (listaDecodificaBancaPerSede != null && listaDecodificaBancaPerSede.Count > 0)
            {
                foreach (DecodificaBanchePerSede decBanca in listaDecodificaBancaPerSede.Where(x => x.CodiceSede == sede))
                    CodeUtility.SetValueDdl(ddlCodiceBanca, decBanca.TraduzioneSuGP.ToString() + " - " + decBanca.Descrizione, decBanca.Descrizione, decBanca.Id.ToString());

                if (listaDecodificaBancaPerSede.Count(x => x.CodiceSede == sede) == 1)
                {
                    ddlCodiceBanca.Enabled = false;
                    ddlCodiceBanca.SelectedIndex = 1;
                }
            }
        }

        private void LoadDdlProgressivoBancaFideiussione(short? annoBancaFideiussione, List<DecBancaFideiussione> decBancaFideiussioneFilterAnno,
            out List<DecBancaFideiussione> decBancaFideiussioneFilterProgressivo)
        {
            decBancaFideiussioneFilterProgressivo = null;

            ddlProgressivoBancaFideiussione.Items.Clear();
            CodeUtility.SetValueDdl(ddlProgressivoBancaFideiussione, string.Empty, string.Empty, string.Empty);
            CleanBancaFideiussione();

            if (annoBancaFideiussione.HasValue && decBancaFideiussioneFilterAnno != null && decBancaFideiussioneFilterAnno.Count > 0)
            {
                decBancaFideiussioneFilterProgressivo = decBancaFideiussioneFilterAnno.
                                       Where(x => x.Anno == annoBancaFideiussione).ToList();
                foreach (DecBancaFideiussione decBancaFideiussione in decBancaFideiussioneFilterProgressivo)
                    if (decBancaFideiussione.Progressivo.HasValue && ddlProgressivoBancaFideiussione.Items.FindByValue(decBancaFideiussione.Progressivo.Value.ToString()) == null)
                        CodeUtility.SetValueDdl(ddlProgressivoBancaFideiussione, decBancaFideiussione.Progressivo.ToString(), decBancaFideiussione.Progressivo.ToString(),
                            decBancaFideiussione.Progressivo.ToString());
            }
        }

        private void txtAziendaOnChange()
        {
            short? codiceBancaEsodati = txtAzienda.Text.IndexOf('-') >= 1 ? CodeUtility.StringToNullableShort(txtAzienda.Text.Substring(0, txtAzienda.Text.IndexOf('-') - 1).Trim()) : null;
            List<DecBancaFideiussione> listaDecBancaFideiussione = (List<DecBancaFideiussione>)ViewState[EnumViewState.ListaDecodificaBancaFideiussione.ToString()];
            List<DecBancaFideiussione> listaDecBancaFideiussioneFilterAnno = null;
            List<DecBancaFideiussione> listaDecBancaFideiussioneFilterProgressivo = null;
            LoadDdlAnnoBancaFideiussione(codiceBancaEsodati, listaDecBancaFideiussione, out listaDecBancaFideiussioneFilterAnno);
            LoadDdlProgressivoBancaFideiussione(null, listaDecBancaFideiussioneFilterAnno, out listaDecBancaFideiussioneFilterProgressivo);

            ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterAnno.ToString()] = listaDecBancaFideiussioneFilterAnno;
            ViewState[EnumViewState.ListaDecodificaBancaFideiussioneFilterProgressivo.ToString()] = listaDecBancaFideiussioneFilterProgressivo;

            if (listaDecBancaFideiussioneFilterAnno != null && !listaDecBancaFideiussioneFilterAnno.Exists(x => x.Anno.HasValue))
            {
                lblNoPianoEsodo.Visible = true;
                btnAggiornaAnnoBancaFideiussione.Enabled = false;
                btnAggiornaProgressivoBancaFideiussione.Enabled = false;
            }
            else
            {
                lblNoPianoEsodo.Visible = false;
                btnAggiornaAnnoBancaFideiussione.Enabled = true;
            }

            if (listaDecBancaFideiussioneFilterProgressivo == null ||
                (!listaDecBancaFideiussioneFilterProgressivo.Exists(x => x.Progressivo.HasValue)))
                btnAggiornaProgressivoBancaFideiussione.Enabled = false;
            else
                btnAggiornaProgressivoBancaFideiussione.Enabled = true;

            ViewState[EnumViewState.DataScadenzaAssegno.ToString()] = Utility.GetDateFromString(txtScadenza.Text);
            SelezionaTipoCalendarioPerScadenzaAssegno();
        }

        private void CleanBancaFideiussione()
        {
            lblABIBancaFideiussione.Text = string.Empty;
            lblCABBancaFideiussione.Text = string.Empty;
            lblBancaFideiussione.Text = string.Empty;
        }

        /// <summary>
        /// in base a categoria e filtro dell'azienda del titolare, se presente nella lista, seleziona il calendario nel formato GGmmAAAA oppure mmAAAA
        /// </summary>
        private void SelezionaTipoCalendarioPerScadenzaAssegno()
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            DateTime? dataScadenzaAssegno = (DateTime?)ViewState[EnumViewState.DataScadenzaAssegno.ToString()];
            bool? isDataScadenzaAssegnoStoricoValorizzata = (bool?)ViewState[EnumViewState.IsDataScadenzaAssegnoStoricoValorizzata.ToString()];

            List<DecAziendeScadenzaAssegnoGGmmAAAA> listaAziendeGGMMAAAA = (List<DecAziendeScadenzaAssegnoGGmmAAAA>)ViewState[EnumViewState.ListaAziendeScadenzaAssegnoGGMMAAAA.ToString()];

            txtScadenza.CssClass = txtScadenza.CssClass.Replace("dateGGmmAAAA", "");
            txtScadenza.CssClass = txtScadenza.CssClass.Replace("dateMMaaaa", "");
            txtScadenza.CssClass = txtScadenza.CssClass.Replace("date-picker-base", "");
            txtScadenza.CssClass = txtScadenza.CssClass.Replace("date-picker", "");
            txtScadenza.CssClass = txtScadenza.CssClass.Trim();

            string sTradSuGpAzienda = txtAzienda.Text.IndexOf('-') >= 1 ? txtAzienda.Text.Substring(0, txtAzienda.Text.IndexOf('-') - 1).Trim() : string.Empty;
            byte? sProgressivo = !string.IsNullOrEmpty(ddlProgressivoBancaFideiussione.SelectedValue) ? CodeUtility.StringToNullableByte(ddlProgressivoBancaFideiussione.SelectedValue) : null;
            if ((Utility.IsDomandaVESO29(this.domanda.Categoria) && datiPensione.CodiceTipoRichiesta != "74" && !CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione)) ||
                (Utility.IsDomandaVOESO(this.domanda.Categoria) && datiPensione.CodiceTipoRichiesta != "74" && datiPensione.CodiceTipoRichiesta != "71" && datiPensione.CodiceTipoRichiesta != "70" && !CodeUtility.IsRicostituzione(this.TitolarePensione.Pensione)) ||
                Utility.IsDomandaIsoPensioneRicWithScadenzaAssegnoGGMMAAAA(this.domanda.Categoria, this.domanda.CodGruppo, this.domanda.IsScadenzaAssegnoConGiorno) ||
                ((Utility.IsDomandaVESO92_L92(this.domanda.Categoria, datiPensione.Filtro) || Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaVESO92RicWithScadenzaAssegnoGGMMAAAA(this.domanda.Categoria, this.domanda.CodGruppo, this.domanda.IsScadenzaAssegnoConGiorno) ||
                Utility.IsDomandaESPA_L26(this.domanda.Categoria, datiPensione.Filtro) || Utility.IsDomandaESPARicWithScadenzaAssegnoGGMMAAAA(this.domanda.Categoria, this.domanda.CodGruppo, this.domanda.IsScadenzaAssegnoConGiorno)) &&
                 listaAziendeGGMMAAAA != null && listaAziendeGGMMAAAA.Exists(x => x.TraduzioneSuGP == sTradSuGpAzienda && (!x.ProgressivoRichiesto.HasValue || x.ProgressivoRichiesto == sProgressivo))) ||
                (dataScadenzaAssegno.HasValue && dataScadenzaAssegno.Value.Day != 1))
            {
                txtScadenza.CssClass += (" dateGGmmAAAA date-picker-base");
                if (dataScadenzaAssegno.HasValue)
                    txtScadenza.Text = string.Format("{0:dd/MM/yyyy}", dataScadenzaAssegno.Value);
                else
                    txtScadenza.Text = "GG/MM/AAAA";
                REVtxtScadenzaMMAAAA.Enabled = false;
                REVtxtScadenzaGGMMAAAA.Enabled = true;
            }
            else
            {
                txtScadenza.CssClass += (" dateMMaaaa date-picker");
                if (dataScadenzaAssegno.HasValue)
                    txtScadenza.Text = dataScadenzaAssegno.Value.ToString("MM/yyyy");
                else
                    txtScadenza.Text = "MM/AAAA";

                REVtxtScadenzaMMAAAA.Enabled = true;
                REVtxtScadenzaGGMMAAAA.Enabled = false;
            }

            if ((Utility.IsDomandaVESO92_L92(this.domanda.Categoria, datiPensione.Filtro) && (string.IsNullOrEmpty(sTradSuGpAzienda) ||
                // Se l'azienda inserita richiede il progressivo e questo non è stato inserito, allora lascio la textbox bloccata
                (listaAziendeGGMMAAAA != null && listaAziendeGGMMAAAA.Exists(x => x.TraduzioneSuGP == sTradSuGpAzienda && x.ProgressivoRichiesto.HasValue && !sProgressivo.HasValue)))) ||
                ((Utility.IsDomandaAPESociale(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica) ||
                (Utility.IsDomandaAPESociale(this.domanda.Categoria) && CodeUtility.IsRicostituzione(datiPensione) && !isDataScadenzaAssegnoStoricoValorizzata.GetValueOrDefault()) ||
                (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && Utility.IsDomandaESOTEL(domanda.Categoria)))
                txtScadenza.Enabled = false;
            else
                txtScadenza.Enabled = true;

            if (Utility.IsDomandaESOPMI(this.domanda.Categoria) && datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value.Month == 12 &&
                datiPensione.DecorrenzaOriginaria.Value.Year == 2022)
            {
                dataScadenzaAssegno = new DateTime(2023, 1, 1);
                txtScadenza.Text = dataScadenzaAssegno.Value.ToString("MM/yyyy");
                txtScadenza.Enabled = false;
            }

            if ((Utility.IsDomandaCRED27(this.domanda.Categoria) || Utility.IsDomandaCOOP28(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) ||
                ((Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)) && dataScadenzaAssegno.HasValue && Utility.DataSuccessivaA(dataScadenzaAssegno.Value, new DateTime(2027, 2, 1)))
                txtScadenza.Enabled = true;
        }

        #region events
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
        public event EventHandler HideAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }
        #endregion events

        #region enum
        public enum EnumViewState
        {
            ListaDecodificaBancaFideiussione,
            ListaDecodificaBancaFideiussioneFilterAnno,
            ListaDecodificaBancaFideiussioneFilterProgressivo,
            DataScadenzaAssegno,
            ListaAziendaEditoria,
            ListaAziendeScadenzaAssegnoGGMMAAAA,
            ListaDecAnagraficaAccordi,
            ListaDecAnagraficaAziende,
            ListaDecAnagraficaAccordiPerTipo0171,
            ListaDecAnagraficaAziendePerTipo0171,
            IsPrepensionamentoEditoria,
            IsPrepensionamentoEditoriaArt1c154L205_2017,
            IsPrepensionamentoEditoriaArt1c500L160_2019,
            IsPrepensionamentoEditoriaLetteraB,
            ListaDecAnagraficaAccordiPerTipo0179,
            ListaDecAnagraficaAziendePerTipo0179,
            IsDataScadenzaAssegnoStoricoValorizzata,
            ListaDecAnagraficaAccordiLetteraB,
            ListaDecAnagraficaAziendeLetteraB
        }
        #endregion enum
    }
}
