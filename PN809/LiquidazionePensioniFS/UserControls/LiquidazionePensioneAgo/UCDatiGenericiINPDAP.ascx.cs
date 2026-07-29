using System;
using System.Linq;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCDatiGenericiINPDAP : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            BindClick();
            AddInputClass();
        }

        protected void SalvaDatiGenerici_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP = GetDatiGenerici();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiGenericiAgo(this);

            if (!HasError)
            {
                ClearBonusSection();
                RaiseShowAvviso(this, null);
                //ricarica ddl codiceComunicazione3
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
                SetDdlCodiceComunicazione3(datiDecodifica, this);
            }
            else
            {
                RaiseShowAvviso(this, null);
            }
        }

        protected void EliminaDatiGenerici_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiGenericiAgo(this);

            if (!this.HasError)
            {
                ClearForm();
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                if (datiPensione.DecorrenzaOriginaria.HasValue)
                    lblDecorrenzaPensioneData.Text = datiPensione.DecorrenzaOriginaria.Value.ToShortDateString();
                bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);
                ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        internal void ValorizzaEtichetteDatiGenerici(ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            if (liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.IsProvvisoriaVisible.HasValue &&
                           liquidazione.areaLiquidazionePensioneAgo.IsProvvisoriaVisible.Value)
                ViewState["IsProvvisoriaVisible"] = liquidazione.areaLiquidazionePensioneAgo.IsProvvisoriaVisible.Value;

            LoadDdlCommon(liquidazione, datiDecodifica);

            RenderControlsCommon(IsDomandaSperDonna, liquidazione, datiPensione);

            ValorizzaEtichetteCommon(IsDomandaSperDonna, liquidazione, datiPensione);

            //Gestione ricostituzioni
            if (datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura)
                GestioneEtichetteRic(datiPensione);

            ManageSperDonna(IsDomandaSperDonna, liquidazione);
        }

        internal DatiGenericiINPDAP GetDatiGenerici()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP = new DatiGenericiINPDAP();
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP = GetDatiGenericiCommon();

            return areaLiquidazionePensioneAgo.DatiGenericiINPDAP;
        }

        internal void ClearBonusSection()
        {
            if (!String.Equals(ddlCodNatura2DG.SelectedValue, "Y"))
            {
                ddlAttribuzioneBonus.SelectedIndex = 0;
                txtDataInizioBonus.Text = "MM/AAAA";
                txtDataFineBonus.Text = "MM/AAAA";
            }
        }

        #endregion protected methods

        #region private methods

        internal void SetDdlCodiceComunicazione3(AreaDecodifica datiDecodifica, ILiquidazionePensioneAgo liquidazioneAgo)
        {
            ddlCodComunicazioni3.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampo3 comunicazioneCampo3 in datiDecodifica.ElencoComunicazioneCampo3)
            {
                switch (comunicazioneCampo3.Id)
                {
                    case "Q":
                        if (liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null
                            && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3 != null
                            && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3.Equals('Q'))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "":
                        CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "P":
                        if (ViewState["IsProvvisoriaVisible"] != null && (bool)ViewState["IsProvvisoriaVisible"])
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                }
            }
            if (liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null
                           && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3 != null)
                ddlCodComunicazioni3.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3.ToString();

        }

        private void BindClick()
        {
            chkBenefici.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkExCombattente.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            ddlCodNatura2DG.Attributes.Add("onChange", "javascript:getDDLCodNatura2Value()");
        }

        private void AddInputClass()
        {
            chkBenefici.InputAttributes.Add("EnableClass", "onClassBenefici");
            chkExCombattente.InputAttributes.Add("EnableClass", "onClassExCombattente");
        }

        private void ValorizzaEtichetteCommon(bool IsDomandaSperDonna, ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblDecorrenzaPensione.Text = "Decorrenza Giuridica:";
            lblAnzAnniSperDonnaSemestre.Text = "Anni servizio 247:";
            lblAnzAnniSperDonnaTrimestre.Text = "Anni servizio 247:";

            if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null)
            {
                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.NaturaPensione))
                {
                    try
                    {
                        ddlCodNatura1DG.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.NaturaPensione.Substring(0, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura2DG.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.NaturaPensione.Substring(1, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura3DG.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.NaturaPensione.Substring(2, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                }
                else
                {
                    ddlCodNatura1DG.SelectedIndex = 0;
                    ddlCodNatura2DG.SelectedIndex = 0;
                    ddlCodNatura3DG.SelectedIndex = 0;
                }

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null)
                {
                    if (pnlRequisitiAnte247.Visible)
                    {
                        if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.Value)
                            ddlReqAnte247.SelectedValue = "SI";
                        else if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.Value)
                            ddlReqAnte247.SelectedValue = "NO";

                        if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TrimesteRequisiti.HasValue)
                            ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TrimesteRequisiti.ToString();

                        txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnnoRequisiti.Value.ToString() : string.Empty;
                        txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnzianitaAnni.ToString() : string.Empty;
                    }
                    else
                    {
                        if (IsDomandaSperDonna)
                        {
                            if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.Value)
                                ddlSperimentaleDonna.SelectedValue = "SI";
                            else if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.RequisitiAnte247.Value)
                                ddlSperimentaleDonna.SelectedValue = "NO";

                            if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TrimesteRequisiti.HasValue)
                                ddlSemestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TrimesteRequisiti.ToString();

                            txtSemestreRequisiti.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnnoRequisiti.ToString() : string.Empty;
                            txtAnzAnniSperDonna.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AnzianitaAnni.ToString() : string.Empty;
                        }
                    }
                }
            }

            ManageCodNatura3(liquidazione);

            if (!datiPensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenzaPensioneData.Text = string.Empty;
            else
                lblDecorrenzaPensioneData.Text = datiPensione.DecorrenzaOriginaria.Value.ToShortDateString();
            //valorizza causa carico
            string causaCarico = (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null) ? (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CausaCarico.ToString()) : (string.Empty);
            bool causaCaricoEnabled;
            ddlCausaCarico.SelectedValue = CodeUtility.FS_SelectedValueDdlCausaCaricoByTipoDomanda(GetDatiPensione(this), causaCarico, out causaCaricoEnabled);
            ddlCausaCarico.Enabled = causaCaricoEnabled;

            if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null)
            {
                txtDecorrenzaArretrati.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DecorrenzaCalcoloArretrati.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DecorrenzaCalcoloArretrati) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceArretrati.HasValue)
                    ddlCodiciArretrati.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceArretrati.ToString();
                else ddlCodiciArretrati.SelectedIndex = 0;

                txtDataCompletezza.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataCompletezza.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataCompletezza) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TipoCalcolo.HasValue)
                {
                    try
                    {
                        ddlTipoCalcolo.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TipoCalcolo.Value.ToString();
                        if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TipoCalcolo.Value.ToString()) &&
                            liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TipoCalcolo.Value.ToString().Trim() != string.Empty &&
                            (string.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue) || ddlTipoCalcolo.SelectedValue.Trim() == string.Empty))
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
                        this.HasError = true;
                        this.ErrorMessage = "Tipo calcolo precedentemente salvato non compatibile con l'attuale valore della data perfezionamento requisiti";
                        RaiseShowAvviso(this, Cevent);
                    }
                }
                else ddlTipoCalcolo.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo1.HasValue)
                    ddlCodComunicazioni1.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo1.ToString();
                else ddlCodComunicazioni1.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo2.HasValue)
                    ddlCodComunicazioni2.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo2.ToString();
                else ddlCodComunicazioni2.SelectedIndex = 0;

                if (ddlCodComunicazioni1.SelectedValue == "1" || ddlCodComunicazioni1.SelectedValue == "2")
                    ddlCodComunicazioni2.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo2.ToString();

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3.HasValue)
                    ddlCodComunicazioni3.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3.ToString();

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo4.HasValue)
                    ddlCodComunicazioni4.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo4.ToString();
                else ddlCodComunicazioni4.SelectedIndex = 0;

                txtScadRevSanitaria.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.ScadenzaRevisioneSanitaria.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.ScadenzaRevisioneSanitaria) : string.Empty;
                chkBenefici.Checked = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.Benefici.HasValue && liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.Benefici.Value ? true : false;
                chkExCombattente.Checked = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.ExCombattente.HasValue && liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.ExCombattente.Value ? true : false;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus == true)
                        ddlAttribuzioneBonus.SelectedValue = "SI";
                    else if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus == false)
                        ddlAttribuzioneBonus.SelectedValue = "NO";
                }
                else ddlAttribuzioneBonus.SelectedIndex = 0;

                txtDataInizioBonus.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.InizioBonus.HasValue && !String.Equals(liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.InizioBonus.ToString().ToLowerInvariant(), "mm/aaaa") ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.InizioBonus.Value) : string.Empty;
                txtDataFineBonus.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.FineBonus.HasValue && !String.Equals(liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.FineBonus.ToString().ToLowerInvariant(), "mm/aaaa") ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.FineBonus.Value) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataInteressiLegali.HasValue)
                    txtInteressiLegali.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataInteressiLegali.Value);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AliquotaMediaINPDAP.HasValue)
                    txtAliquotaMedia.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AliquotaMediaINPDAP.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataRivalsaINPDAP.HasValue)
                    txtDataRivalsa.Text = liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataRivalsaINPDAP.Value.ToShortDateString();
            }

            if (IsDomandaSperDonna)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP == null || !liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TipoCalcolo.HasValue)
                {
                    ddlTipoCalcolo.ClearSelection();
                    if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                        ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                }
                ddlTipoCalcolo.Enabled = false;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP == null)
                {
                    ddlSperimentaleDonna.ClearSelection();
                    ddlSperimentaleDonna.SelectedValue = "SI";
                    txtAnzAnniSperDonna.Text = "35";
                }
                ddlSperimentaleDonna.Enabled = false;
            }
        }

        private DatiGenericiINPDAP GetDatiGenericiCommon()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP = new DatiGenericiINPDAP();

            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DecorrenzaCalcoloArretrati = (!String.IsNullOrEmpty(txtDecorrenzaArretrati.Text)) && (!String.Equals(txtDecorrenzaArretrati.Text.ToLowerInvariant(), "gg/mm/aaaa")) ? Utility.GetDateFromString(txtDecorrenzaArretrati.Text) : (DateTime?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceArretrati = !String.IsNullOrEmpty(ddlCodiciArretrati.SelectedValue) ? byte.Parse(ddlCodiciArretrati.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.ScadenzaRevisioneSanitaria = (!String.IsNullOrEmpty(txtScadRevSanitaria.Text)) && (!String.Equals(txtScadRevSanitaria.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtScadRevSanitaria.Text) : (DateTime?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataCompletezza = !String.Equals(txtDataCompletezza.Text.ToLowerInvariant(), "gg/mm/aaaa") ? Utility.GetDateFromString(txtDataCompletezza.Text) : (DateTime?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.TipoCalcolo = !String.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue) ? byte.Parse(ddlTipoCalcolo.SelectedValue) : (byte?)null;

            string naturaPensione = "";
            naturaPensione = String.Concat(ddlCodNatura1DG.SelectedValue, ddlCodNatura2DG.SelectedValue, ddlCodNatura3DG.SelectedValue);
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.NaturaPensione = naturaPensione;

            if (ddlAttribuzioneBonus.SelectedValue == "SI")
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus = true;
            else if (ddlAttribuzioneBonus.SelectedValue == "NO")
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus = false;
            else
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus = null;

            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.InizioBonus = (!String.IsNullOrEmpty(txtDataInizioBonus.Text)) && (!String.Equals(txtDataInizioBonus.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtDataInizioBonus.Text) : (DateTime?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.FineBonus = (!String.IsNullOrEmpty(txtDataFineBonus.Text)) && (!String.Equals(txtDataFineBonus.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtDataFineBonus.Text) : (DateTime?)null;

            if (!String.Equals(ddlCodNatura2DG.SelectedValue, "Y"))
            {
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AttribuzioneBonus = null;
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.InizioBonus = null;
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.FineBonus = null;
            }

            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CausaCarico = !String.IsNullOrEmpty(ddlCausaCarico.SelectedValue) ? byte.Parse(ddlCausaCarico.SelectedValue) : (byte?)null;

            if (!String.IsNullOrEmpty(ddlCodComunicazioni1.SelectedValue))
            {
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo1 = byte.Parse(ddlCodComunicazioni1.SelectedValue);
                if (String.Equals(ddlCodComunicazioni1.SelectedValue, "1"))
                {
                    if (!String.IsNullOrEmpty(ddlCodComunicazioni2.Text))
                    {
                        if (!String.Equals(ddlCodComunicazioni2.Text, " "))
                            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo2 = char.Parse(ddlCodComunicazioni2.SelectedValue);
                    }
                }
                else if (String.Equals(ddlCodComunicazioni1.SelectedValue, "2"))
                {
                    if (!String.IsNullOrEmpty(ddlCodComunicazioni2.Text))
                        if (!String.Equals(ddlCodComunicazioni2.Text, " "))
                            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo2 = char.Parse(ddlCodComunicazioni2.SelectedValue);
                }
            }


            if (!String.IsNullOrEmpty(ddlCodComunicazioni3.SelectedValue))
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3 = char.Parse(ddlCodComunicazioni3.SelectedValue);

            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo3 = !String.IsNullOrEmpty(ddlCodComunicazioni3.SelectedValue) ? char.Parse(ddlCodComunicazioni3.SelectedValue) : (char?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.CodiceComunicazioneCampo4 = !String.IsNullOrEmpty(ddlCodComunicazioni4.SelectedValue) ? byte.Parse(ddlCodComunicazioni4.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.Benefici = HiddenFieldChkBenefici.Value == "true" ? true : false;
            areaLiquidazionePensioneAgo.DatiGenericiINPDAP.ExCombattente = chkExCombattente.Checked ? true : false;

            if (!string.IsNullOrEmpty(HiddenIntLeg.Value) && !HiddenIntLeg.Value.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataInteressiLegali = Utility.GetDateFromString(HiddenIntLeg.Value);
            else
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataInteressiLegali = null;

            if (!string.IsNullOrEmpty(txtAliquotaMedia.Text))
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.AliquotaMediaINPDAP = CodeUtility.StringToNullableDecimal(txtAliquotaMedia.Text);

            if (!string.IsNullOrEmpty(txtDataRivalsa.Text))
                areaLiquidazionePensioneAgo.DatiGenericiINPDAP.DataRivalsaINPDAP = Utility.GetDateFromString(txtDataRivalsa.Text);

            return areaLiquidazionePensioneAgo.DatiGenericiINPDAP;
        }

        private void RenderControlsCommon(bool IsDomandaSperDonna, ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            pnlCommonRequisitiAnteSperDonna.Visible = true;
            rowCausaCarico.Visible = true;
            pnlDecorrenzaGiuridica.Visible = true;

            if (liquidazione.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.HasValue && !liquidazione.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.Value)
                ddlCodNatura2DG.Enabled = liquidazione.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.Value;
            pnlCommon.Visible = true;
            pnlCommonHeader.Visible = true;
            pnlCommonCheck.Visible = true;

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Substring(0, 1) == "I")  // pensione Invalidità
            {
                pnlRequisitiAnte247.Visible = false;
                pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                this.pnlSperimentaleDonna.Visible = false;
            }
            else
            {
                if (IsDomandaSperDonna)
                {
                    pnlRequisitiAnte247.Visible = false;
                    pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;

                    if (!liquidazione.areaLiquidazionePensioneAgo.IsRequisitiL247_L243Enable.HasValue || !liquidazione.areaLiquidazionePensioneAgo.IsRequisitiL247_L243Enable.Value)
                        this.pnlSperimentaleDonna.Visible = false;
                    else
                        this.pnlSperimentaleDonna.Visible = true;
                }
                else
                {
                    if (!liquidazione.areaLiquidazionePensioneAgo.IsRequisitiL247_L243Enable.HasValue || !liquidazione.areaLiquidazionePensioneAgo.IsRequisitiL247_L243Enable.Value)
                    {
                        pnlRequisitiAnte247.Visible = false;
                        pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                    }
                    else
                    {
                        pnlRequisitiAnte247.Visible = true;
                        pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = true;
                    }

                    this.pnlSperimentaleDonna.Visible = false;
                }
            }

            if (!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Substring(0, 1) == "S")  // pensione ai Superstiti
            {
                pnlRequisitiAnte247.Visible = false;
                pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
            }

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
            {
                rowCausaCarico.Visible = false;

                if (tipologiaProdottoPensione != CodeUtility.TipologiaPensioneProdotto.pr_VariazioneDecorrenza)
                {
                    pnlRequisitiAnte247.Visible = false;
                    pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                }
            }

            ManageProvvisoria(liquidazione);

            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2DG, liquidazione.areaLiquidazionePensioneAgo.IsSperimentaleDonna.Value);
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            switch (datiPensione.CodeProdotto)
            {
                case "0101":
                case "0102":
                case "0104":
                case "0108":
                case "0111":
                case "0112":
                case "0120":
                case "0301":
                case "0302":
                case "0304":
                case "0308":
                case "0311":
                case "0312":
                case "0320":
                case "0401":
                case "0402":
                case "0404":
                case "0408":
                case "0411":
                case "0412":
                case "0420":
                    TrScadenzaRevisioneSanitaria.Visible = false;
                    ddlTipoCalcolo.Enabled = false;
                    ddlCodNatura1DG.Enabled = false;
                    ddlCodNatura2DG.Enabled = false;
                    ddlCodNatura3DG.Enabled = false;
                    pnlRequisitiAnte247.Visible = false;
                    pnlTrimestreAnzianitaRequisitiNoInvalidita.Visible = false;
                    break;
                default:
                    break;
            }

            ddlCodiciArretrati.Enabled = false;
            ddlCodiciArretrati.SelectedValue = "8";
            ddlCausaCarico.Enabled = false;

            if (trBonus2432004.Visible && datiPensione.CodeProdotto != "0107" && datiPensione.CodeProdotto != "0102" &&
                datiPensione.CodeProdotto != "0307" && datiPensione.CodeProdotto != "0302" &&
                datiPensione.CodeProdotto != "0407" && datiPensione.CodeProdotto != "0402" && !this.domanda.IsDomandaRiapertura)
            {
                CodeUtility.BloccaForm((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], pnlBonus2432004);
            }

            btnEliminaDatiGenerici.Enabled = false;
        }

        private void LoadDdlCommon(ILiquidazionePensioneAgo liquidazione, Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica)
        {
            ddlTipoCalcolo.Items.Clear();
            ddlTipoCalcolo.Items.Add(new ListItem(string.Empty, " "));
            Presenter.SvrLiquidazione.AreaDecodifica.DatiTipoCalcolo[] listaTipoCalcolo = datiDecodifica.ElencoTipoCalcolo;// areaDecodifica.GetValuesDecodifica().ElencoTipoCalcolo;
            foreach (Presenter.SvrLiquidazione.AreaDecodifica.DatiTipoCalcolo tipoCalcolo in listaTipoCalcolo)
                if ((tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Inps" && (tipoCalcolo.TraduzioneSuGP == 1 || tipoCalcolo.TraduzioneSuGP == 2 || tipoCalcolo.TraduzioneSuGP == 9)) ||
                    (this.domanda.IsDomandaENPALS && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals" && (tipoCalcolo.TraduzioneSuGP == 9)) ||
                    (this.domanda.IsDomandaENPALS && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals" && (tipoCalcolo.TraduzioneSuGP == 2)))
                    CodeUtility.SetValueDdl(ddlTipoCalcolo, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);

            //load ddl causa carico
            Presenter.SvrLiquidazione.AreaDecodifica.DatiCausaCarico[] listaCausaCarico = CodeUtility.FS_GetDdlCausaCaricoByTipoDomanda(GetDatiPensione(this), datiDecodifica.ElencoCausaCarico).ToArray();
            ddlCausaCarico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCausaCarico);
            foreach (AreaDecodifica.DatiCausaCarico causaCarico in listaCausaCarico)
                CodeUtility.SetValueDdl(ddlCausaCarico, causaCarico.Descrizione, causaCarico.Descrizione, causaCarico.Id);

            AreaDecodifica.DatiComunicazioneCampi1_2[] listaComunicazioneC1_2 = datiDecodifica.ElencoComunicazioneCampi1_2;
            ddlCodComunicazioni2.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampi1_2 comunicazioneC1_2 in listaComunicazioneC1_2)
                CodeUtility.SetValueDdl(ddlCodComunicazioni2, comunicazioneC1_2.Campo2.ToString(), comunicazioneC1_2.Descrizione, comunicazioneC1_2.Campo2.ToString());

            SetDdlCodiceComunicazione3(datiDecodifica, liquidazione);

            AreaDecodifica.DatiComunicazioneCampo4[] listaComunicazioneC4 = datiDecodifica.ElencoComunicazioneCampo4;
            ddlCodComunicazioni4.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampo4 comunicazioneCampo4 in listaComunicazioneC4)
            {
                if (CodeUtility.LoadRecordEsenzioneFiscaleFS(comunicazioneCampo4.Id, liquidazione.areaLiquidazionePensioneAgo.IsEsenzioneFiscaleEstero))
                    CodeUtility.SetValueDdl(ddlCodComunicazioni4, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
            }

            if (liquidazione.areaLiquidazionePensioneAgo.listaCodiciNatura != null && liquidazione.areaLiquidazionePensioneAgo.listaCodiciNatura.Count() > 0)
            {
                ddlCodNatura1DG.Items.Clear();
                ddlCodNatura2DG.Items.Clear();
                ddlCodNatura3DG.Items.Clear();

                CodeUtility.SetValueDdl(ddlCodNatura1DG, string.Empty, string.Empty, " ");
                CodeUtility.SetValueDdl(ddlCodNatura2DG, string.Empty, string.Empty, " ");
                CodeUtility.SetValueDdl(ddlCodNatura3DG, string.Empty, string.Empty, " ");

                foreach (Presenter.SvrLiquidazioneAgo.CodiciNatura codiceNatura in liquidazione.areaLiquidazionePensioneAgo.listaCodiciNatura)
                    if (codiceNatura.Posizione == 1)
                        CodeUtility.SetValueDdl(ddlCodNatura1DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                    else if (codiceNatura.Posizione == 2)
                        CodeUtility.SetValueDdl(ddlCodNatura2DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                    else
                        CodeUtility.SetValueDdl(ddlCodNatura3DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            txtScadRevSanitaria.Text = "mm/aaaa";
            txtDataInizioBonus.Text = "mm/aaaa";
            txtDataFineBonus.Text = "mm/aaaa";
            txtTrimestreRequisiti.Text = "aaaa";
            txtAnzAnni.Text = "aa";
            txtSemestreRequisiti.Text = "aaaa";
            txtAnzAnniSperDonna.Text = "aa";
        }

        private void ManageProvvisoria(ILiquidazionePensioneAgo liquidazione)
        {
            //if (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP != null)
            //{
            //    ddlCodComunicazioni3.Enabled = !Utility.IsDomandaProvvisoria(liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.IsProvvisoria);
            //}

            if (liquidazione.areaLiquidazionePensioneAgo.IsCodiceComunicazione3Visible.HasValue)
            {
                ddlCodComunicazioni3.Visible = liquidazione.areaLiquidazionePensioneAgo.IsCodiceComunicazione3Visible.Value;
            }
        }

        private void ManageCodNatura3(ILiquidazionePensioneAgo liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneAgo.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsUsuranti.Value)
            {
                if (!ddlCodNatura3DG.Items.Contains(new ListItem("Z", "Z")))
                    ddlCodNatura3DG.Items.Add(new ListItem("Z", "Z"));
                ddlCodNatura3DG.SelectedValue = "Z";
                ddlCodNatura3DG.Enabled = false;
            }
        }

        private void ManageSperDonna(bool IsDomandaSperDonna, ILiquidazionePensioneAgo liquidazione)
        {
            if (IsDomandaSperDonna)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && (liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP == null || string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneAgo.DatiGenericiINPDAP.NaturaPensione)))
                {
                    ddlCodNatura2DG.ClearSelection();
                    if (ddlCodNatura2DG.Items.FindByValue("O") != null)
                        ddlCodNatura2DG.SelectedValue = "O";
                }
                ddlCodNatura2DG.Enabled = false;
            }
        }

        private DatiGenericiINPDAP GetDatiGenericiToPensioneFondoFS(DatiGenericiINPDAP datiGenerici)
        {
            if (pnlRequisitiAnte247.Visible)
            {
                if (String.Equals(ddlReqAnte247.SelectedValue, "SI"))
                    datiGenerici.RequisitiAnte247 = true;
                else if (String.Equals(ddlReqAnte247.SelectedValue, "NO"))
                    datiGenerici.RequisitiAnte247 = false;

                datiGenerici.TrimesteRequisiti = !String.IsNullOrEmpty(ddlTrimestreRequisiti.SelectedValue) ? byte.Parse(ddlTrimestreRequisiti.SelectedValue) : (byte?)null;
                datiGenerici.AnnoRequisiti = (!String.Equals(txtTrimestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtTrimestreRequisiti.Text))) ? Int16.Parse(txtTrimestreRequisiti.Text) : (short?)null;
                datiGenerici.AnzianitaAnni = (!String.Equals(txtAnzAnni.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnni.Text))) ? Int32.Parse(txtAnzAnni.Text) : (int?)null;
            }
            else
            {
                if (this.pnlSperimentaleDonna.Visible)
                {
                    if (String.Equals(ddlSperimentaleDonna.SelectedValue, "SI"))
                        datiGenerici.RequisitiAnte247 = true;
                    else if (String.Equals(ddlSperimentaleDonna.SelectedValue, "NO"))
                        datiGenerici.RequisitiAnte247 = false;

                    datiGenerici.TrimesteRequisiti = !String.IsNullOrEmpty(ddlSemestreRequisiti.SelectedValue) ? byte.Parse(ddlSemestreRequisiti.SelectedValue) : (byte?)null;
                    datiGenerici.AnnoRequisiti = (!String.Equals(txtSemestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtSemestreRequisiti.Text))) ? Int16.Parse(txtSemestreRequisiti.Text) : (short?)null;
                    datiGenerici.AnzianitaAnni = (!String.Equals(txtAnzAnniSperDonna.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnniSperDonna.Text))) ? Int32.Parse(txtAnzAnniSperDonna.Text) : (int?)null;
                }
            }

            return datiGenerici;
        }

        #endregion private methods

        #region Events

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion
    }
}