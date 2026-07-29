using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA;
using System.Collections.Generic;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.ModalitaPagamento
{
    public partial class UCPagamento : CustomBaseUserControl, IPagamento, ITitolarePensione
    {
        #region IPagamento
        public AreaPagamento pagamentoPensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public RichiestaUfficiPagatori richiestaUfficiPagatori { get; set; }
        public UfficioPagatore[] ufficioPagatore { get; set; }
        #endregion IPagamento

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        string _service_error = "#_SERVICE_ERROR_#";

        public bool IsBancaItaliaFromWebDom
        {
            get { return (bool)ViewState[EnumViewState.IsBancaItaliaFromWebDom.ToString()]; }
            set { ViewState[EnumViewState.IsBancaItaliaFromWebDom.ToString()] = value; }
        }

        public bool IsPolarizzazionePerGestioneENPALSAttiva
        {
            get { return (bool)ViewState[EnumViewState.IsPolarizzazionePerGestioneENPALSAttiva.ToString()]; }
            set { ViewState[EnumViewState.IsPolarizzazionePerGestioneENPALSAttiva.ToString()] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!(Page.IsPostBack))
            {
                RenderControlsWithFondo();
                string nove = string.Empty;
                SetConstantValue(out nove);

                int abiCassaSede = 0;
                int.TryParse(nove, out abiCassaSede);
                RicercaDatiPagamento(abiCassaSede);

                LoadDdl();
                //SetConstantValue();
            }
            BindClick();
            AddInputClass();
        }

        internal void SalvaDatiPagamento(object sender, EventArgs e)
        {
            try
            {
                GetDatiUcPagamento();
                PresenterPagamento presenterPagamento = new PresenterPagamento();
                showPopUp.Value = "";

                if (this.tipoPagamento.Value != null)
                {
                    switch (tipoPagamento.Value)
                    {
                        case "B":
                            if (String.IsNullOrEmpty(modPagamentoB.Value) || String.IsNullOrEmpty(txtBanca.Text))
                            {
                                RaiseParametriNonValidi(this, null);
                                return;
                            }
                            break;
                        case "P":
                            if (String.IsNullOrEmpty(modPagamentoP.Value) || String.IsNullOrEmpty(txtUffPost.Text))
                            {
                                RaiseParametriNonValidi(this, null);
                                return;
                            }
                            break;
                        case "E":
                            if (String.IsNullOrEmpty(modPagamentoE.Value) || String.IsNullOrEmpty(txtNomeUfficioEstero.Text))
                            {
                                RaiseParametriNonValidi(this, null);
                                return;
                            }
                            break;
                        case "C":
                            if (String.IsNullOrEmpty(modPagamentoC.Value) || String.IsNullOrEmpty(txtDescrizioneSede.Text))
                            {
                                RaiseParametriNonValidi(this, null);
                                return;
                            }
                            break;
                        default:
                            break;
                    }

                    presenterPagamento.SalvaDatiPagamento(this);
                    if (HasError)
                    {
                        RaiseServiceErrorAvviso(this, null);
                        return;
                    }
                    else
                    {
                        ClearForm();
                        ValorizzaPagamento();

                    }
                    RaiseSalvaPagamento(this, null);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo SalvaDatiPagamento " + ex);
            }
        }

        internal void EliminaPagamento(object sender, EventArgs e)
        {
            try
            {
                PresenterPagamento presenterPagamento = new PresenterPagamento();
                //this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                presenterPagamento.EliminaDatiPagamento(this);
                if (HasError)
                {
                    RaiseServiceErrorAvviso(this, null);
                    return;
                }
                RaiseEliminaPagamento(this, null);
                ValorizzaUfficioPagatore(string.Empty, string.Empty);
                showPopUp.Value = string.Empty;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo EliminaPagamento" + ex);
            }
        }

        #region Private Methods

        private void RenderControlsWithFondo()
        {
            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        pnlCircolarita.Visible = true;
                        break;
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            paramRicerca1.Value = string.Empty;
            paramRicerca2.Value = string.Empty;
            paramRicerca3.Value = string.Empty;

            string nove = string.Empty;
            SetConstantValue(out nove);

            switch (pagamentoPensione.Pagamento.TipoPagamento.Value)
            {
                case 'B':
                    modPagamentoP.Value = string.Empty;
                    modPagamentoE.Value = string.Empty;
                    modPagamentoC.Value = string.Empty;
                    break;
                case 'P':
                    modPagamentoB.Value = string.Empty;
                    modPagamentoE.Value = string.Empty;
                    modPagamentoC.Value = string.Empty;
                    break;
                case 'E':
                    modPagamentoB.Value = string.Empty;
                    modPagamentoP.Value = string.Empty;
                    modPagamentoC.Value = string.Empty;
                    break;
                case 'C':
                    modPagamentoB.Value = string.Empty;
                    modPagamentoP.Value = string.Empty;
                    modPagamentoE.Value = string.Empty;
                    break;
                default:
                    modPagamentoB.Value = string.Empty;
                    modPagamentoC.Value = string.Empty;
                    modPagamentoP.Value = string.Empty;
                    modPagamentoE.Value = string.Empty;
                    break;
            }
        }

        private void LoadDdl()
        {
            LoadDdlStatoEstero();
            LoadddlCassaSedeSede();
        }

        private void SetConstantValue(out string nove)
        {
            nove = "99999";
            string sette = "07601";
            txtAbiSportelloCassaSede.Text = nove;
            txtAbiSportelloPosta.Text = txtAbiCircPosta.Text = sette;
            txtFrazionarioCircPosta.Text = nove.PadLeft(7, '0');
        }

        private void LoadDdlStatoEstero()
        {
            try
            {
                ddlStatoEstero.Items.Add(new ListItem(string.Empty, string.Empty));
                ddlStatoEsteroCCEstero.Items.Add(new ListItem(string.Empty, string.Empty));
                if (pagamentoPensione != null && pagamentoPensione.ListStatiEsteri != null && pagamentoPensione.ListStatiEsteri.Length > 0)
                {
                    ViewState[EnumViewState.ListaStatiEsteri.ToString()] = pagamentoPensione.ListStatiEsteri.ToList();

                    foreach (GestioneAreaPagamentoDatiStatoEstero statoEstero in pagamentoPensione.ListStatiEsteri.ToList())
                    {
                        ListItem li1 = new ListItem();
                        li1.Attributes.Add("title", statoEstero.NomeStato);
                        li1.Text = statoEstero.NomeStato;
                        li1.Value = statoEstero.NomeStato;
                        ddlStatoEsteroCCEstero.Items.Add(li1);
                        ddlStatoEstero.Items.Add(li1);
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo LoadDdlStatoEstero" + ex);
            }
        }

        private void LoadddlCassaSedeSede()
        {
            try
            {
                //Domande di esodo (categoria 27, 28, 29, 127, 128, 198 e 199)
                //Il controllo su DomandaEsodo e al tempo stesso non ESOTEL in quanto con il Memo216 si è aggiunto
                //il requisito per cui Cassa Sede deve poter essere utilizzato per le domande ESOTEL
                if (Utility.IsDomandaEsodo(this.domanda.Categoria) && !Utility.IsDomandaESOTEL(this.domanda.Categoria))
                {
                    ddlCassaSede.Enabled = false;
                }
                else
                {
                    CodeUtility.SetValueDdl(ddlCassaSede, string.Empty, string.Empty, string.Empty);
                }

                if (pagamentoPensione != null && pagamentoPensione.ListCassaSede != null && pagamentoPensione.ListCassaSede.Length > 0)
                {
                    ViewState["ListaCassaSede"] = pagamentoPensione.ListCassaSede;
                    foreach (GestioneAreaPagamentoDatiCassaSede cassaSede in pagamentoPensione.ListCassaSede.ToList())
                    {
                        if ((!Utility.IsDomandaEsodo(this.domanda.Categoria) || cassaSede.Cab.ToString() == "3300004"))
                            CodeUtility.SetValueDdl(ddlCassaSede, cassaSede.Agenzia, cassaSede.Agenzia, cassaSede.Cab.ToString());
                    }

                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo LoadDdlCassaSede" + ex);
            }
        }

        private void RicercaDatiPagamento(int abiCassaSede)
        {
            try
            {
                PresenterPagamento presenterPagamento = new PresenterPagamento();
                this.pagamentoPensione = new AreaPagamento();
                this.pagamentoPensione.Pagamento = new GestioneAreaPagamentoDatiPagamento();
                this.pagamentoPensione.Pagamento.ABI = abiCassaSede;
                presenterPagamento.RicercaDatiPagamento(this);
                RaiseManageBtnPopup(this, null);
                if (!HasError)
                {
                    RaiseNascondiPannelloAvviso(this, null);
                    if (!String.IsNullOrEmpty(pagamentoPensione.Pagamento.NomeUfficioPagatore))
                        RaiseVisualizzaTastoSalva(this, null);
                }
                else
                    RaiseServiceErrorAvviso(this, null);

                IsBancaItaliaFromWebDom = pagamentoPensione.IsBancaItaliaFromWebDom;
                IsPolarizzazionePerGestioneENPALSAttiva = pagamentoPensione.IsPolarizzazionePerGestioneENPALSAttiva;

                SetTipoPagamento();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo RicercaDatipagamento " + ex);
            }
        }

        private void SetTipoPagamento()
        {
            try
            {
                if (!pagamentoPensione.Pagamento.TipoPagamento.HasValue)
                    return;

                switch (pagamentoPensione.Pagamento.TipoPagamento.Value)
                {
                    case 'B':
                        tipoPagamento.Value = "B";
                        break;
                    case 'P':
                        tipoPagamento.Value = "P";
                        break;
                    case 'E':
                        tipoPagamento.Value = "E";
                        break;
                    case 'C':
                        tipoPagamento.Value = "C";
                        break;
                }
                ValorizzaPagamento();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo SetTipoPagamento " + ex);
            }
        }

        private void ValorizzaPagamento()
        {
            try
            {
                switch (pagamentoPensione.Pagamento.TipoPagamento.Value)
                {
                    case 'B':
                        rdbBanca.Checked = true;
                        txtIban.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.IBAN) ? pagamentoPensione.Pagamento.IBAN : string.Empty;
                        txtCodiceAbi.Text = Utility.WriteAbiCab(pagamentoPensione.Pagamento.ABI);
                        txtCodiceCab.Text = Utility.WriteAbiCab(pagamentoPensione.Pagamento.CAB);
                        txtBicBanca.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.BIC) ? pagamentoPensione.Pagamento.BIC : string.Empty;
                        txtBanca.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.NomeUfficioPagatore) ? pagamentoPensione.Pagamento.NomeUfficioPagatore : string.Empty;
                        txtAgenzia.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.AgenziaUfficioPagatore) ? pagamentoPensione.Pagamento.AgenziaUfficioPagatore : string.Empty;
                        txtIndirizzo.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.IndirizzoUfficioPagatore) ? pagamentoPensione.Pagamento.IndirizzoUfficioPagatore : string.Empty;
                        txtCittaBanca.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.CittaUfficioPagatore) ? pagamentoPensione.Pagamento.CittaUfficioPagatore : string.Empty;
                        txtCapBanca.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.CapUfficioPagatore) ? pagamentoPensione.Pagamento.CapUfficioPagatore : string.Empty;
                        CodMeccanizzazioneB.Value = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.UfficioPagatore) ? pagamentoPensione.Pagamento.UfficioPagatore : string.Empty;
                        if (IsBancaItaliaFromWebDom)
                            CodeUtility.BloccaForm(this.domanda, panMain);
                        break;
                    case 'P':
                        rdbPosta.Checked = true;
                        txtIbanPoste.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.IBAN) ? pagamentoPensione.Pagamento.IBAN : string.Empty;
                        txtBicPoste.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.BIC) ? pagamentoPensione.Pagamento.BIC : string.Empty;
                        txtUffPost.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.NomeUfficioPagatore) ? pagamentoPensione.Pagamento.NomeUfficioPagatore : string.Empty;
                        txtNumUffPost.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.AgenziaUfficioPagatore) ? pagamentoPensione.Pagamento.AgenziaUfficioPagatore : string.Empty;
                        txtIndirizzoUffPost.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.IndirizzoUfficioPagatore) ? pagamentoPensione.Pagamento.IndirizzoUfficioPagatore : string.Empty;
                        txtCapUffPost.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.CapUfficioPagatore) ? pagamentoPensione.Pagamento.CapUfficioPagatore : string.Empty;
                        txtCittaUffPost.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.CittaUfficioPagatore) ? pagamentoPensione.Pagamento.CittaUfficioPagatore : string.Empty;
                        txtCodAbiUffPost.Text = Utility.WriteAbiCab(pagamentoPensione.Pagamento.ABI);
                        if (pagamentoPensione.Pagamento.ABI == 07601)
                            txtCabFrazionario.Text = pagamentoPensione.Pagamento.Frazionario.HasValue ? pagamentoPensione.Pagamento.Frazionario.ToString() : string.Empty;
                        else
                            txtCabFrazionario.Text = pagamentoPensione.Pagamento.CAB.HasValue ? pagamentoPensione.Pagamento.CAB.ToString() : string.Empty;
                        txtLibretto.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.Libretto) ? pagamentoPensione.Pagamento.Libretto : string.Empty;
                        break;
                    case 'E':
                        rdbEstero.Checked = true;
                        txtNomeUfficioEstero.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.NomeUfficioPagatore) ? pagamentoPensione.Pagamento.NomeUfficioPagatore : string.Empty;
                        txtAbiUfficioEstero.Text = Utility.WriteAbiCab(pagamentoPensione.Pagamento.ABI);
                        txtCabUfficioEstero.Text = pagamentoPensione.Pagamento.CAB.HasValue ? pagamentoPensione.Pagamento.CAB.Value.ToString() : string.Empty;
                        txtAgenziaUfficioEstero.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.AgenziaUfficioPagatore) ? pagamentoPensione.Pagamento.AgenziaUfficioPagatore : string.Empty;
                        txtCittaUfficioEstero.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.CittaUfficioPagatore) ? pagamentoPensione.Pagamento.CittaUfficioPagatore : string.Empty;
                        txtIbanUfficioEstero.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.IBAN) ? pagamentoPensione.Pagamento.IBAN : string.Empty;
                        txtBicUfficioEstero.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.BIC) ? pagamentoPensione.Pagamento.BIC : string.Empty;
                        CodMeccanizzazioneE.Value = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.UfficioPagatore) ? pagamentoPensione.Pagamento.UfficioPagatore : string.Empty;
                        break;
                    case 'C':
                        rdbCassaSede.Checked = true;
                        txtDescrizioneSede.Text = !String.IsNullOrEmpty(pagamentoPensione.Pagamento.AgenziaUfficioPagatore) ? pagamentoPensione.Pagamento.AgenziaUfficioPagatore : string.Empty;
                        txtAbiCassa.Text = Utility.WriteAbiCab(pagamentoPensione.Pagamento.ABI);
                        txtCabCassa.Text = Utility.WriteAbiCab(pagamentoPensione.Pagamento.CAB);
                        break;
                }
                RaiseVisualizzaTastoSalva(this, null);
                if (!IsBancaItaliaFromWebDom)
                    RaiseVisualizzaEliminaPagamento(this, null);
                SetModalitaPagamento();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo ValorizzaPagamento " + ex);
            }
        }

        private void SetModalitaPagamento()
        {
            try
            {
                if (pagamentoPensione.Pagamento.ModalitaPagamento.HasValue)
                {
                    switch (tipoPagamento.Value)
                    {
                        case "B":
                            modPagamentoB.Value = pagamentoPensione.Pagamento.ModalitaPagamento.Value.ToString();
                            switch (modPagamentoB.Value)
                            {
                                case "S":
                                    rdbPagamSportelloBanca.Checked = true;
                                    break;
                                case "C":
                                    rdbContoCorrenteBanca.Checked = true;
                                    break;
                                case "L":
                                    rdbLibrettoRisparmioBanca.Checked = true;
                                    break;
                                case "K":
                                    rdbPrepagataBanca.Checked = true;
                                    break;
                            }
                            break;
                        case "P":
                            modPagamentoP.Value = pagamentoPensione.Pagamento.ModalitaPagamento.Value.ToString();
                            switch (modPagamentoB.Value)
                            {
                                case "X":
                                    rdbPagPostCircolarita.Checked = true;
                                    break;
                                case "S":
                                    rdbPagPostSportello.Checked = true;
                                    break;
                                case "L":
                                    rdbPagPostLibretto.Checked = true;
                                    break;
                                case "C":
                                    rdbPagPostContoCorr.Checked = true;
                                    break;
                                case "K":
                                    rdbPagPostPrepagata.Checked = true;
                                    break;
                            }
                            break;
                        case "E":
                            modPagamentoE.Value = pagamentoPensione.Pagamento.ModalitaPagamento.Value.ToString();
                            switch (modPagamentoB.Value)
                            {
                                case "S":
                                    rdbSportelloE.Checked = true;
                                    break;
                                case "A":
                                    rdbAssegnoE.Checked = true;
                                    break;
                                case "C":
                                    rdbContoCorrenteE.Checked = true;
                                    break;
                            }
                            break;
                        case "C":
                            modPagamentoC.Value = pagamentoPensione.Pagamento.ModalitaPagamento.Value.ToString();
                            switch (modPagamentoB.Value)
                            {
                                case "P":
                                    rdbPagamSportelloCassaSede.Checked = true;
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo SetModalitaPagamento " + ex);
            }
        }

        private void BindClick()
        {
            rdbBanca.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPosta.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbEstero.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbCassaSede.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagamSportelloBanca.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbLibrettoRisparmioBanca.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbContoCorrenteBanca.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPrepagataBanca.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagPostContoCorr.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagPostLibretto.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagPostSportello.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagPostCircolarita.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagPostPrepagata.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbSportelloE.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbContoCorrenteE.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbAssegnoE.Attributes.Add("onclick", "javascript:SetRadio(this)");
            rdbPagamSportelloCassaSede.Attributes.Add("onclick", "javascript:SetRadio(this)");
        }

        private void AddInputClass()
        {
            rdbBanca.InputAttributes.Add("EnableClass", "onClassBanca");
            rdbPosta.InputAttributes.Add("EnableClass", "onClassPosta");
            rdbEstero.InputAttributes.Add("EnableClass", "onClassEstero");
            rdbCassaSede.InputAttributes.Add("EnableClass", "onClassCassaSede");

            rdbPagamSportelloBanca.InputAttributes.Add("EnableClass", "onClassSportelloBanca");
            rdbLibrettoRisparmioBanca.InputAttributes.Add("EnableClass", "onClassLibrettoRisparmioBanca");
            rdbContoCorrenteBanca.InputAttributes.Add("EnableClass", "onClassContoCorrenteBanca");
            rdbPrepagataBanca.InputAttributes.Add("EnableClass", "onClassPrepagataBanca");

            rdbPagPostContoCorr.InputAttributes.Add("EnableClass", "onClassCCPosta");
            rdbPagPostLibretto.InputAttributes.Add("EnableClass", "onClassLibrettoPosta");
            rdbPagPostSportello.InputAttributes.Add("EnableClass", "onClassSportelloPosta");
            rdbPagPostCircolarita.InputAttributes.Add("EnableClass", "onClassCircPosta");
            rdbPagPostPrepagata.InputAttributes.Add("EnableClass", "onClassPrepagataPosta");

            rdbSportelloE.InputAttributes.Add("EnableClass", "onClassSportelloE");
            rdbContoCorrenteE.InputAttributes.Add("EnableClass", "onClassContoCorrenteE");
            rdbAssegnoE.InputAttributes.Add("EnableClass", "onClassAssegnoE");
            rdbPagamSportelloCassaSede.InputAttributes.Add("EnableClass", "onClassSportelloCassaSede");
        }

        private void ValorizzaRadioButton(string tipo, string modalita)
        {
            try
            {
                switch (tipo)
                {
                    case "B":
                        rdbBanca.Checked = true;
                        switch (modalita)
                        {
                            case "S":
                                rdbPagamSportelloBanca.Checked = true;
                                break;
                            case "C":
                                rdbContoCorrenteBanca.Checked = true;
                                break;
                            case "L":
                                rdbLibrettoRisparmioBanca.Checked = true;
                                break;
                            case "K":
                                rdbPrepagataBanca.Checked = true;
                                break;
                        }
                        break;
                    case "P":
                        rdbPosta.Checked = true;
                        switch (modalita)
                        {
                            case "S":
                                rdbPagPostSportello.Checked = true;
                                break;
                            case "C":
                                rdbPagPostContoCorr.Checked = true;
                                break;
                            case "L":
                                rdbPagPostLibretto.Checked = true;
                                break;
                            case "X":
                                rdbPagPostCircolarita.Checked = true;
                                break;
                            case "K":
                                rdbPagPostPrepagata.Checked = true;
                                break;
                        }
                        break;
                    case "E":
                        rdbEstero.Checked = true;
                        switch (modalita)
                        {
                            case "S":
                                rdbSportelloE.Checked = true;
                                break;
                            case "C":
                                rdbContoCorrenteE.Checked = true;
                                break;
                            case "A":
                                rdbAssegnoE.Checked = true;
                                break;
                        }
                        break;
                    case "C":
                        rdbCassaSede.Checked = true;
                        switch (modalita)
                        {
                            case "P":
                                rdbPagamSportelloCassaSede.Checked = true;
                                break;
                        }
                        break;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo ValorizzaRadioButton " + ex);
            }
        }

        private void ValorizzaUfficioPagatore(string tipo, string modalita)
        {
            try
            {
                switch (tipo)
                {
                    case "B":
                        txtIban.Text = string.Empty;
                        txtBicBanca.Text = string.Empty;
                        txtCodiceAbi.Text = Utility.WriteAbiCab(this.ufficioPagatore[0].Abi);
                        txtCodiceCab.Text = Utility.WriteAbiCab(this.ufficioPagatore[0].Cab);
                        txtBanca.Text = this.ufficioPagatore[0].Nome;
                        txtAgenzia.Text = this.ufficioPagatore[0].Agenzia;
                        txtIndirizzo.Text = this.ufficioPagatore[0].Indirizzo;
                        txtCittaBanca.Text = this.ufficioPagatore[0].Citta;
                        txtCapBanca.Text = this.ufficioPagatore[0].Cap;
                        CodMeccanizzazioneB.Value = this.ufficioPagatore[0].CodiceMeccanizzazione;
                        modPagamentoB.Value = modalita;
                        RaiseVisualizzaTastoSalva(this, null);
                        break;
                    case "P":
                        txtIbanPoste.Text = string.Empty;
                        txtUffPost.Text = this.ufficioPagatore[0].Nome;
                        txtNumUffPost.Text = this.ufficioPagatore[0].Agenzia;
                        txtIndirizzoUffPost.Text = this.ufficioPagatore[0].Indirizzo;
                        txtCapUffPost.Text = this.ufficioPagatore[0].Cap;
                        txtCittaUffPost.Text = this.ufficioPagatore[0].Citta;
                        txtCodAbiUffPost.Text = Utility.WriteAbiCab(this.ufficioPagatore[0].Abi);
                        if (this.ufficioPagatore[0].Abi == 07601)
                        {
                            lblCabFrazionario.Text = "Frazionario";
                            txtCabFrazionario.Text = this.ufficioPagatore[0].Frazionario != 0 ? this.ufficioPagatore[0].Frazionario.ToString().PadLeft(7, '0') : string.Empty;
                        }
                        else
                        {
                            lblCabFrazionario.Text = "CAB";
                            txtCabFrazionario.Text = Utility.WriteAbiCab(this.ufficioPagatore[0].Cab);
                        }
                        CodMeccanizzazioneP.Value = this.ufficioPagatore[0].CodiceMeccanizzazione;

                        RaiseVisualizzaTastoSalva(this, null);
                        modPagamentoP.Value = modalita;
                        break;
                    case "E":
                        if (modPagamentoE.Value != "C")
                        {
                            txtIbanUfficioEstero.Text = string.Empty;
                            txtBicUfficioEstero.Text = string.Empty;
                        }
                        txtNomeUfficioEstero.Text = this.ufficioPagatore[0].Nome;
                        txtAbiUfficioEstero.Text = Utility.WriteAbiCab(this.ufficioPagatore[0].Abi);
                        txtAgenziaUfficioEstero.Text = this.ufficioPagatore[0].Agenzia;
                        txtCittaUfficioEstero.Text = this.ufficioPagatore[0].Citta;
                        txtCabUfficioEstero.Text = this.ufficioPagatore[0].Cab.ToString();
                        modPagamentoE.Value = modalita;
                        CodMeccanizzazioneE.Value = this.ufficioPagatore[0].CodiceMeccanizzazione;
                        break;
                    case "C":

                        AreaPagamento ap = new AreaPagamento();
                        ap.ListCassaSede = (GestioneAreaPagamentoDatiCassaSede[])ViewState["ListaCassaSede"];
                        txtDescrizioneSede.Text = ap.ListCassaSede.ToList().Find(x => x.Cab == this.richiestaUfficiPagatori.Cab).Agenzia;
                        txtAbiCassa.Text = this.richiestaUfficiPagatori.Abi.ToString();
                        txtCabCassa.Text = this.richiestaUfficiPagatori.Cab.ToString();
                        modPagamentoC.Value = modalita;
                        RaiseVisualizzaTastoSalva(this, null);
                        break;
                    default:
                        ResetData();
                        rdbBanca.Checked = false;
                        rdbPosta.Checked = false;
                        rdbEstero.Checked = false;
                        rdbCassaSede.Checked = false;
                        rdbContoCorrenteBanca.Checked = false;
                        rdbLibrettoRisparmioBanca.Checked = false;
                        rdbPagamSportelloBanca.Checked = false;
                        rdbPrepagataBanca.Checked = false;
                        rdbPagPostContoCorr.Checked = false;
                        rdbPagPostLibretto.Checked = false;
                        rdbPagPostSportello.Checked = false;
                        rdbPagPostCircolarita.Checked = false;
                        rdbPagPostPrepagata.Checked = false;
                        rdbAssegnoE.Checked = false;
                        rdbContoCorrenteE.Checked = false;
                        rdbSportelloE.Checked = false;
                        rdbPagamSportelloCassaSede.Checked = false;
                        modPagamentoB.Value = string.Empty;
                        modPagamentoP.Value = string.Empty;
                        modPagamentoE.Value = string.Empty;
                        modPagamentoC.Value = string.Empty;
                        break;
                }
                tipoPagamento.Value = tipo;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo ValorizzaUfficioPagatore " + ex);
            }
        }

        private AreaPagamento GetDatiUcPagamento()
        {
            try
            {
                ValorizzaPagamentoPensione('\0');
                if (rdbBanca.Checked)
                    ValorizzaPagamentoPensione('B');

                else if (rdbPosta.Checked)
                    ValorizzaPagamentoPensione('P');

                else if (rdbEstero.Checked)
                    ValorizzaPagamentoPensione('E');

                else if (rdbCassaSede.Checked)
                    ValorizzaPagamentoPensione('C');

                pagamentoPensione.Pagamento.ModalitaPagamento = GetModalitaPagamento();

                return pagamentoPensione;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo GetDatiUcPagamento " + ex);
            }
        }

        private char GetModalitaPagamento()
        {
            try
            {
                if (rdbPagPostCircolarita.Checked)
                    return 'X';
                if (rdbPagamSportelloBanca.Checked || rdbPagPostSportello.Checked || rdbSportelloE.Checked)
                    return 'S';
                else if (rdbContoCorrenteBanca.Checked || rdbPagPostContoCorr.Checked || rdbContoCorrenteE.Checked)
                    return 'C';
                else if (rdbLibrettoRisparmioBanca.Checked || rdbPagPostLibretto.Checked)
                    return 'L';
                else if (rdbAssegnoE.Checked)
                    return 'A';
                else if (rdbPagamSportelloCassaSede.Checked)
                    return 'P';
                else if (rdbPrepagataBanca.Checked || rdbPagPostPrepagata.Checked)
                    return 'K';
                else
                    return ' ';
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo GetModalitaPagamento" + ex);
            }

        }

        private void ValorizzaPagamentoPensione(char tipoPagamento)
        {
            try
            {
                pagamentoPensione.Pagamento = new GestioneAreaPagamentoDatiPagamento();
                switch (tipoPagamento)
                {
                    case 'B':
                        pagamentoPensione.Pagamento.TipoPagamento = tipoPagamento;
                        pagamentoPensione.Pagamento.IBAN = !String.IsNullOrEmpty(this.txtIban.Text.Trim()) ? this.txtIban.Text : null;
                        pagamentoPensione.Pagamento.BIC = !String.IsNullOrEmpty(this.txtBicBanca.Text.Trim()) ? this.txtBicBanca.Text : null;
                        pagamentoPensione.Pagamento.NomeUfficioPagatore = !String.IsNullOrEmpty(this.txtBanca.Text.Trim()) ? this.txtBanca.Text : null;
                        pagamentoPensione.Pagamento.ABI = !String.IsNullOrEmpty(this.txtCodiceAbi.Text.Trim()) ? Int32.Parse(this.txtCodiceAbi.Text) : (int?)null;
                        pagamentoPensione.Pagamento.CAB = !String.IsNullOrEmpty(this.txtCodiceCab.Text.Trim()) ? Int32.Parse(this.txtCodiceCab.Text) : (int?)null;
                        pagamentoPensione.Pagamento.IndirizzoUfficioPagatore = !String.IsNullOrEmpty(this.txtIndirizzo.Text.Trim()) ? this.txtIndirizzo.Text : null;
                        pagamentoPensione.Pagamento.AgenziaUfficioPagatore = !String.IsNullOrEmpty(this.txtAgenzia.Text.Trim()) ? this.txtAgenzia.Text : null;
                        pagamentoPensione.Pagamento.CittaUfficioPagatore = !String.IsNullOrEmpty(this.txtCittaBanca.Text.Trim()) ? this.txtCittaBanca.Text : null;
                        pagamentoPensione.Pagamento.CapUfficioPagatore = !String.IsNullOrEmpty(this.txtCapBanca.Text.Trim()) ? this.txtCapBanca.Text : null;
                        pagamentoPensione.Pagamento.UfficioPagatore = !String.IsNullOrEmpty(CodMeccanizzazioneB.Value.Trim()) ? this.CodMeccanizzazioneB.Value : null;
                        break;
                    case 'P':
                        pagamentoPensione.Pagamento.TipoPagamento = tipoPagamento;
                        pagamentoPensione.Pagamento.IBAN = !String.IsNullOrEmpty(this.txtIbanPoste.Text.Trim()) ? this.txtIbanPoste.Text : null;
                        pagamentoPensione.Pagamento.NomeUfficioPagatore = !String.IsNullOrEmpty(this.txtUffPost.Text.Trim()) ? this.txtUffPost.Text : null;
                        pagamentoPensione.Pagamento.CittaUfficioPagatore = !String.IsNullOrEmpty(this.txtCittaUffPost.Text.Trim()) ? this.txtCittaUffPost.Text : null;
                        pagamentoPensione.Pagamento.CapUfficioPagatore = !String.IsNullOrEmpty(this.txtCapUffPost.Text.Trim()) ? this.txtCapUffPost.Text : null;
                        pagamentoPensione.Pagamento.IndirizzoUfficioPagatore = !String.IsNullOrEmpty(this.txtIndirizzoUffPost.Text.Trim()) ? this.txtIndirizzoUffPost.Text : null;
                        pagamentoPensione.Pagamento.AgenziaUfficioPagatore = !String.IsNullOrEmpty(this.txtNumUffPost.Text.Trim()) ? this.txtNumUffPost.Text : null;
                        pagamentoPensione.Pagamento.ABI = !String.IsNullOrEmpty(this.txtCodAbiUffPost.Text.Trim()) ? Int32.Parse(this.txtCodAbiUffPost.Text) : (int?)null;
                        if (pagamentoPensione.Pagamento.ABI == 07601)
                            pagamentoPensione.Pagamento.Frazionario = !String.IsNullOrEmpty(this.txtCabFrazionario.Text.Trim()) ? Int32.Parse(this.txtCabFrazionario.Text) : (int?)null;
                        else
                            pagamentoPensione.Pagamento.CAB = !String.IsNullOrEmpty(this.txtCabFrazionario.Text.Trim()) ? Int32.Parse(this.txtCabFrazionario.Text) : (int?)null;
                        pagamentoPensione.Pagamento.Libretto = !String.IsNullOrEmpty(this.txtLibretto.Text.Trim()) ? this.txtLibretto.Text : null;
                        pagamentoPensione.Pagamento.UfficioPagatore = !String.IsNullOrEmpty(CodMeccanizzazioneP.Value.Trim()) ? CodMeccanizzazioneP.Value : null;
                        break;
                    case 'E':
                        pagamentoPensione.Pagamento.TipoPagamento = tipoPagamento;
                        pagamentoPensione.Pagamento.NomeUfficioPagatore = !String.IsNullOrEmpty(this.txtNomeUfficioEstero.Text.Trim()) ? this.txtNomeUfficioEstero.Text : null;
                        pagamentoPensione.Pagamento.CittaUfficioPagatore = !String.IsNullOrEmpty(this.txtCittaUfficioEstero.Text.Trim()) ? this.txtCittaUfficioEstero.Text : null;
                        pagamentoPensione.Pagamento.AgenziaUfficioPagatore = !String.IsNullOrEmpty(this.txtAgenziaUfficioEstero.Text.Trim()) ? this.txtAgenziaUfficioEstero.Text : null;
                        pagamentoPensione.Pagamento.CAB = !String.IsNullOrEmpty(this.txtCabUfficioEstero.Text.Trim()) ? Int32.Parse(this.txtCabUfficioEstero.Text) : (int?)null;
                        pagamentoPensione.Pagamento.ABI = !String.IsNullOrEmpty(this.txtAbiUfficioEstero.Text.Trim()) ? Int32.Parse(this.txtAbiUfficioEstero.Text) : (int?)null;
                        pagamentoPensione.Pagamento.Libretto = !String.IsNullOrEmpty(this.txtLibretto.Text.Trim()) ? this.txtLibretto.Text : null;
                        pagamentoPensione.Pagamento.UfficioPagatore = !String.IsNullOrEmpty(CodMeccanizzazioneE.Value.Trim()) ? CodMeccanizzazioneE.Value : null;
                        if (modPagamentoE.Value == "C")
                        {
                            pagamentoPensione.Pagamento.IBAN = !String.IsNullOrEmpty(this.txtIbanUfficioEstero.Text.Trim()) ? this.txtIbanUfficioEstero.Text : null;
                            pagamentoPensione.Pagamento.BIC = !String.IsNullOrEmpty(this.txtBicUfficioEstero.Text.Trim()) ? this.txtBicUfficioEstero.Text : null;
                        }

                        List<GestioneAreaPagamentoDatiStatoEstero> listaStatiEsteri = ViewState[EnumViewState.ListaStatiEsteri.ToString()] as List<GestioneAreaPagamentoDatiStatoEstero>;
                        if (listaStatiEsteri != null)
                        {
                            GestioneAreaPagamentoDatiStatoEstero statoEstero = listaStatiEsteri.Find(x => x.NomeStato.Trim() == paramRicerca3.Value.Trim());
                            if (statoEstero != null)
                                pagamentoPensione.Pagamento.CodCatastaleEstero = statoEstero.CodCatastale;
                        }
                        break;
                    case 'C':
                        pagamentoPensione.Pagamento.TipoPagamento = tipoPagamento;
                        pagamentoPensione.Pagamento.ABI = !String.IsNullOrEmpty(this.txtAbiCassa.Text.Trim()) ? Int32.Parse(this.txtAbiCassa.Text) : (int?)null;
                        pagamentoPensione.Pagamento.CAB = !String.IsNullOrEmpty(this.txtCabCassa.Text.Trim()) ? Int32.Parse(this.txtCabCassa.Text) : (int?)null;
                        pagamentoPensione.Pagamento.AgenziaUfficioPagatore = !String.IsNullOrEmpty(txtDescrizioneSede.Text.Trim()) ? txtDescrizioneSede.Text : null;
                        break;
                    default:
                        break;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPagamento, Errore nel metodo ValorizzaPagamentoPensione " + ex);
            }
        }

        private void ResetData()
        {
            txtUffPost.Text = string.Empty;
            txtNumUffPost.Text = string.Empty;
            txtIndirizzoUffPost.Text = string.Empty;
            txtCapUffPost.Text = string.Empty;
            txtCittaUffPost.Text = string.Empty;
            txtCodAbiUffPost.Text = string.Empty;
            txtCodiceAbi.Text = string.Empty;
            txtCodiceCab.Text = string.Empty;
            txtBanca.Text = string.Empty;
            txtAgenzia.Text = string.Empty;
            txtIndirizzo.Text = string.Empty;
            txtCapBanca.Text = string.Empty;
            txtCittaBanca.Text = string.Empty;
            txtIban.Text = string.Empty;
            txtIbanPoste.Text = string.Empty;
            txtCabFrazionario.Text = string.Empty;
            txtBicBanca.Text = string.Empty;
            txtLibretto.Text = string.Empty;
            txtNomeUfficioEstero.Text = string.Empty;
            txtCittaUfficioEstero.Text = string.Empty;
            txtAgenziaUfficioEstero.Text = string.Empty;
            txtCabUfficioEstero.Text = string.Empty;
            txtAbiUfficioEstero.Text = string.Empty;
            txtIbanUfficioEstero.Text = string.Empty;
            txtBicUfficioEstero.Text = string.Empty;
            txtDescrizioneSede.Text = string.Empty;
            txtAbiCassa.Text = string.Empty;
            txtCabCassa.Text = string.Empty;
        }

        #endregion Private Methods

        #region Buttons Events

        protected void btnRicercaSportelloBanca_Click(object sender, EventArgs e)
        {
            string sErrore;
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            if (Utility.CheckAbiCabFrazionario(paramRicerca1.Value, out sErrore))
            {
                this.richiestaUfficiPagatori.Abi = Int32.Parse(paramRicerca1.Value);
                if (this.richiestaUfficiPagatori.Abi == 07601)
                {
                    RaiseParametriNonValidi(this, null);
                    return;
                }
            }
            else
            {
                RaiseParametriNonValidi(this, null);
                return;
            }
            if (Utility.CheckAbiCabFrazionario(paramRicerca2.Value, out sErrore))
                this.richiestaUfficiPagatori.Cab = Int32.Parse(paramRicerca2.Value);
            else
            {
                modPagamentoB.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            if (this.richiestaUfficiPagatori.Abi == 36081 && this.richiestaUfficiPagatori.Cab == 05138)
            {
                modPagamentoB.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Cab;
            this.richiestaUfficiPagatori.ModalitaPagamento = "S";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "BS";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoB.Value);
                }
                modPagamentoB.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("B", "S");
            showPopUp.Value = string.Empty;
        }

        protected void btnRicercaCCBanca_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            this.richiestaUfficiPagatori.Bic = paramRicerca2.Value;
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Banca;
            this.richiestaUfficiPagatori.ModalitaPagamento = "C";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {

                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "BC";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoB.Value);
                }
                modPagamentoB.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("B", "C");
            showPopUp.Value = string.Empty;
            txtIban.Text = paramRicerca1.Value;
            txtBicBanca.Text = paramRicerca2.Value;
        }

        protected void btnRicercaLibrettoBanca_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Banca;
            this.richiestaUfficiPagatori.ModalitaPagamento = "L";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "BL";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoB.Value);
                }
                modPagamentoB.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            showPopUp.Value = string.Empty;
            ValorizzaUfficioPagatore("B", "L");
            txtIban.Text = paramRicerca1.Value;
        }

        protected void btnRicercaPrepagataBanca_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            this.richiestaUfficiPagatori.Bic = paramRicerca2.Value;
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Banca;
            this.richiestaUfficiPagatori.ModalitaPagamento = "K";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {

                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "BK";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoB.Value);
                }
                modPagamentoB.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("B", "K");
            showPopUp.Value = string.Empty;
            txtIban.Text = paramRicerca1.Value;
            txtBicBanca.Text = paramRicerca2.Value;
        }

        protected void btnRicercaSportelloPosta_Click(object sender, EventArgs e)
        {
            string sErrore;
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            if (Utility.CheckAbiCabFrazionario(paramRicerca1.Value, out sErrore))
                this.richiestaUfficiPagatori.Abi = Int32.Parse(paramRicerca1.Value);
            else
            {
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            if (Utility.CheckAbiCabFrazionario(paramRicerca2.Value, out sErrore))
                this.richiestaUfficiPagatori.Frazionario = Int32.Parse(paramRicerca2.Value);
            else
            {
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Frazionario;
            this.richiestaUfficiPagatori.ModalitaPagamento = "S";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "PS";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoP.Value);
                }
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("P", "S");
            int resInt = 0;
            int.TryParse(paramRicerca1.Value, out resInt);
            txtCodAbiUffPost.Text = Utility.WriteAbiCab(resInt);
            txtCabFrazionario.Text = paramRicerca2.Value;
        }

        protected void btnRicercaLibrettoPosta_Click(object sender, EventArgs e)
        {
            string sErrore;
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            if (Utility.CheckAbiCabFrazionario(paramRicerca2.Value, out sErrore))
                this.richiestaUfficiPagatori.Frazionario = Int32.Parse(paramRicerca2.Value);
            else
            {
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Posta;
            this.richiestaUfficiPagatori.ModalitaPagamento = "L";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    validSummarySportelloBanca.Visible = true;
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "PL";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoP.Value);
                }
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("P", "L");
            txtCabFrazionario.Text = paramRicerca2.Value;
            txtIbanPoste.Text = paramRicerca1.Value.ToUpperInvariant();
        }

        protected void btnRicercaCCPosta_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            this.richiestaUfficiPagatori.Frazionario = Int32.Parse(paramRicerca2.Value);
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Posta;
            this.richiestaUfficiPagatori.ModalitaPagamento = "C";
            string saveTipo = tipoPagamento.Value;
            string saveMod = modPagamentoP.Value;
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "PC";
                    ValorizzaRadioButton(saveTipo, saveMod);
                }
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("P", "C");
            txtIbanPoste.Text = paramRicerca1.Value;
            txtCabFrazionario.Text = paramRicerca2.Value;
        }

        protected void btnRicercaCircPosta_Click(object sender, EventArgs e)
        {
            string sErrore;
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            if (Utility.CheckAbiCabFrazionario(paramRicerca1.Value, out sErrore))
            {
                this.richiestaUfficiPagatori.Abi = Int32.Parse(paramRicerca1.Value);
                if (this.richiestaUfficiPagatori.Abi != 07601)
                {
                    RaiseParametriNonValidi(this, null);
                    return;
                }
            }
            else
            {
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            if (Utility.CheckCassaSede(paramRicerca2.Value, out sErrore))
            {
                this.richiestaUfficiPagatori.Cab = Int32.Parse(paramRicerca2.Value);
                if (this.richiestaUfficiPagatori.Cab != 0099999)
                {
                    RaiseParametriNonValidi(this, null);
                    return;
                }
            }
            else
            {
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Cab;
            this.richiestaUfficiPagatori.ModalitaPagamento = "S";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "PX";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoP.Value);
                }
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("P", "X");
            showPopUp.Value = string.Empty;
        }

        protected void btnRicercaPrepagataPosta_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            this.richiestaUfficiPagatori.Frazionario = CodeUtility.StringToNullableInt(paramRicerca2.Value).GetValueOrDefault();
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Posta;
            this.richiestaUfficiPagatori.ModalitaPagamento = "K";
            string saveTipo = tipoPagamento.Value;
            string saveMod = modPagamentoP.Value;
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "PC";
                    ValorizzaRadioButton(saveTipo, saveMod);
                }
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            ValorizzaUfficioPagatore("P", "K");
            txtIbanPoste.Text = paramRicerca1.Value;
        }

        protected void btnRicercaCCEstero_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.Iban = paramRicerca1.Value;
            //nel caso estero, nel cap passo il bic/swift/Etc.
            this.richiestaUfficiPagatori.Bic = paramRicerca2.Value;
            this.richiestaUfficiPagatori.StatoEstero = paramRicerca3.Value;
            List<GestioneAreaPagamentoDatiStatoEstero> listaStatiEsteri = ViewState[EnumViewState.ListaStatiEsteri.ToString()] as List<GestioneAreaPagamentoDatiStatoEstero>;
            if (listaStatiEsteri != null)
            {
                GestioneAreaPagamentoDatiStatoEstero statoEstero = listaStatiEsteri.Find(x => x.NomeStato.Trim() == paramRicerca3.Value.Trim());
                if (statoEstero != null)
                {
                    this.richiestaUfficiPagatori.Abi = CodeUtility.StringToNullableInt(statoEstero.ABI).GetValueOrDefault();
                    this.richiestaUfficiPagatori.Cab = CodeUtility.StringToNullableInt(statoEstero.CAB).GetValueOrDefault();
                    this.richiestaUfficiPagatori.CodCatastaleEstero = statoEstero.CodCatastale;
                }
            }
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Estero;
            this.richiestaUfficiPagatori.ModalitaPagamento = "C";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //showPopUp.Value = "EC";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoE.Value);
                }
                modPagamentoE.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            txtIbanUfficioEstero.Text = paramRicerca1.Value;
            txtBicUfficioEstero.Text = paramRicerca2.Value;
            ValorizzaUfficioPagatore("E", "C");
        }

        protected void btnRicercaStatoEstero_Click(object sender, EventArgs e)
        {
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            this.richiestaUfficiPagatori.StatoEstero = paramRicerca1.Value;
            List<GestioneAreaPagamentoDatiStatoEstero> listaStatiEsteri = ViewState[EnumViewState.ListaStatiEsteri.ToString()] as List<GestioneAreaPagamentoDatiStatoEstero>;
            if (listaStatiEsteri != null)
            {
                GestioneAreaPagamentoDatiStatoEstero statoEstero = listaStatiEsteri.Find(x => x.NomeStato.Trim() == paramRicerca1.Value.Trim());
                if (statoEstero != null)
                {
                    this.richiestaUfficiPagatori.Abi = CodeUtility.StringToNullableInt(statoEstero.ABI).GetValueOrDefault();
                    this.richiestaUfficiPagatori.Cab = CodeUtility.StringToNullableInt(statoEstero.CAB).GetValueOrDefault();
                }
            }
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Estero;
            if (modPagamentoE.Value == "S")
                this.richiestaUfficiPagatori.ModalitaPagamento = "S";
            else if (modPagamentoE.Value == "A")
                this.richiestaUfficiPagatori.ModalitaPagamento = "A";
            PresenterPagamento presenterPagamento = new PresenterPagamento();
            presenterPagamento.RicercaUfficioPagatore(this);
            if (HasError)
            {
                if (this.ErrorMessage.Contains(_service_error))
                {
                    this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
                    RaiseServiceErrorAvviso(sender, e);

                }
                else
                {
                    RaiseNessunaPosizioneTrovata(this, null);
                    //if (modPagamentoE.Value == "S")
                    //    showPopUp.Value = "ES";
                    //else if (modPagamentoE.Value == "A")
                    //    showPopUp.Value = "EA";
                    ValorizzaRadioButton(tipoPagamento.Value, modPagamentoE.Value);
                }
                modPagamentoE.Value = modPagamentoPrecedente.Value;
                return;
            }
            else
            {
                showPopUp.Value = string.Empty;
                ResetData();
                RaiseVisualizzaTastoSalva(this, null);
            }
            if (modPagamentoE.Value == "S")
                ValorizzaUfficioPagatore("E", "S");
            else if (modPagamentoE.Value == "A")
                ValorizzaUfficioPagatore("E", "A");
        }

        protected void btnRicercaSportelloCassaSede_Click(object sender, EventArgs e)
        {
            string sErrore;
            this.richiestaUfficiPagatori = new RichiestaUfficiPagatori();
            if (Utility.CheckAbiCabFrazionario(paramRicerca1.Value, out sErrore))
            {
                this.richiestaUfficiPagatori.Abi = Int32.Parse(paramRicerca1.Value);
                if (this.richiestaUfficiPagatori.Abi != 99999)
                {
                    RaiseParametriNonValidi(this, null);
                    return;
                }
            }
            else
            {
                RaiseParametriNonValidi(this, null);
                return;
            }
            if (Utility.CheckCassaSede(paramRicerca2.Value, out sErrore))
                this.richiestaUfficiPagatori.Cab = Int32.Parse(paramRicerca2.Value);
            else
            {
                modPagamentoP.Value = modPagamentoPrecedente.Value;
                RaiseParametriNonValidi(this, null);
                return;
            }
            this.richiestaUfficiPagatori.Tipo = RichiestaUfficiPagatori.TipoRicerca.Cassa;

            //PresenterPagamento presenterPagamento = new PresenterPagamento();
            //presenterPagamento.RicercaUfficioPagatore(this);
            //if (HasError)
            //{
            //    if (this.ErrorMessage.Contains(_service_error))
            //    {
            //        this.ErrorMessage = this.ErrorMessage.Substring(_service_error.Length);
            //        RaiseServiceErrorAvviso(sender, e);
            //        modPagamentoC.Value = modPagamentoPrecedente.Value;
            //    }
            //    else
            //    {
            //        RaiseNessunaPosizioneTrovata(this, null);
            //        showPopUp.Value = "CS";
            //        ValorizzaRadioButton(tipoPagamento.Value, modPagamentoC.Value);
            //    }
            //    return;
            //}
            //else
            //{
            //    showPopUp.Value = string.Empty;
            //    ResetData();
            //    RaiseVisualizzaTastoSalva(this, null);
            //}

            ///////Blocco nuovo a seguito della modifica per il reperimento della CassaSede///////


            //this.ufficioPagatore[0].Abi = this.richiestaUfficiPagatori.Abi;
            //this.ufficioPagatore[0].Cab = this.richiestaUfficiPagatori.Abi;
            //this.ufficioPagatore[0].Agenzia = this.pagamentoPensione.ListCassaSede.ToList().Find(x => x.Cab == this.richiestaUfficiPagatori.Abi).Agenzia;

            showPopUp.Value = string.Empty;
            ResetData();
            RaiseVisualizzaTastoSalva(this, null);
            /////////////////////////////////////////////////////////////////////////////////////

            ValorizzaUfficioPagatore("C", "P");
            showPopUp.Value = string.Empty;
        }

        #endregion Buttons Events

        #region Events

        public event EventHandler EliminaPagamentoEvent;
        public event EventHandler VisualizzaEliminaPagamento;
        public event EventHandler SalvaPagamentoEvent;
        public event EventHandler NessunaPosizioneTrovata;
        public event EventHandler NascondiPannelloAvviso;
        public event EventHandler VisualizzaTastoSalva;
        public event EventHandler ParametriNonValidi;
        public event EventHandler ServiceErrorAvviso;
        public event EventHandler BloccaEliminaPagamento;
        public event EventHandler ManageBtnPopup;

        protected void RaiseServiceErrorAvviso(object sender, EventArgs e)
        {
            ServiceErrorAvviso(this, null);
        }

        protected void RaiseNessunaPosizioneTrovata(object sender, EventArgs e)
        {
            NessunaPosizioneTrovata(this, null);
        }

        protected void RaiseParametriNonValidi(object sender, EventArgs e)
        {
            HasError = true;
            ErrorMessage = "Parametri inseriti non validi";
            ParametriNonValidi(this, null);
        }

        protected void RaiseNascondiPannelloAvviso(object sender, EventArgs e)
        {
            NascondiPannelloAvviso(this, null);
        }

        protected void RaiseEliminaPagamento(object sender, EventArgs e)
        {
            ValorizzaUfficioPagatore("", "");
            EliminaPagamentoEvent(this, null);
        }

        protected void RaiseVisualizzaEliminaPagamento(object sender, EventArgs e)
        {
            VisualizzaEliminaPagamento(this, null);
        }

        protected void RaiseVisualizzaTastoSalva(object sender, EventArgs e)
        {
            VisualizzaTastoSalva(this, null);
        }

        protected void RaiseSalvaPagamento(object sender, EventArgs e)
        {
            SalvaPagamentoEvent(this, null);
        }

        protected void RaiseBloccaEliminaPagamento(object sender, EventArgs e)
        {
            if (BloccaEliminaPagamento != null)
                BloccaEliminaPagamento(sender, e);
        }

        protected void RaiseManageBtnPopup(object sender, EventArgs e)
        {
            if (ManageBtnPopup != null)
                ManageBtnPopup(sender, e);
        }
        #endregion Events

        #region Enum
        public enum EnumViewState
        {
            ListaStatiEsteri,
            IsBancaItaliaFromWebDom,
            IsPolarizzazionePerGestioneENPALSAttiva
        }
        #endregion Enum
    }
}
