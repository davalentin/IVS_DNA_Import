using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiGenericiINPDAP : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneFS
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneFS

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            BindClick();
            AddInputClass();
        }

        protected void SalvaDatiGenerici_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiGenericiINPDAP = GetDatiGenerici();

            if (ViewState["DatiStoricoGP"] != null)
                areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico = (DatiLiquidazionePensioneStorico)ViewState["DatiStoricoGP"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new Presenter.PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiGenerici(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);

            if (!HasError)
            {
                ClearBonusSection();
                RaiseShowAvviso(this, Cevent);
                //ricarica ddl codiceComunicazione3
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
                SetDdlCodiceComunicazione3(datiDecodifica, this);
            }
            else
            {
                RaiseShowAvviso(this, Cevent);
                GestioneAOI();
            }
        }

        internal void GestioneAOI()
        {
            if (!this.chkTrasfAOI.Checked)
            {
                Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                presenterLiquidazione.GetLiquidazionePensione(this);
                if (HasError)
                    return;
                if (this.areaLiquidazionePensioneFS.DatiPrecedentePensione != null && this.trTrasfAOI.Visible == true)
                    this.chkTrasfAOI.Checked = true;
            }
        }

        protected void EliminaDatiGenerici_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new Presenter.PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiGenerici(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);

            if (!this.HasError)
            {
                ClearForm();
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                if (datiPensione.DecorrenzaOriginaria.HasValue)
                    lblDecorrenzaPensioneData.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);
                ValorizzaEtichetteDatiGenerici(this, datiPensione, IsDomandaSperDonna);
            }

            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteDatiGenerici(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            var anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

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

            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.HasValue &&
                           liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.Value)
                ViewState["IsProvvisoriaVisible"] = liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.Value;

            ViewState["DatiStoricoGP"] = (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null) ? (DatiLiquidazionePensioneStorico)liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico : null;
            LoadDdlCommon(liquidazione, datiDecodifica, datiPensione, this.domanda.Tipofondo);

            RenderControlsCommon(IsDomandaSperDonna, liquidazione, datiPensione);

            ValorizzaEtichetteCommon(IsDomandaSperDonna, liquidazione, datiPensione);

            //Gestione ricostituzioni
            if (datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura)
            {
                GestioneEtichetteRic(datiPensione);
                HiddenFieldIsRicostituzione.Value = "SI";
            }
            else
                HiddenFieldIsRicostituzione.Value = "NO";

            //Gestione ripristini
            if (datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ripristino)
                GestioneEtichetteRipristini(datiPensione);

            if (liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.Value)
            {
                ddlCodNatura3DG.SelectedValue = "H";
                ddlCodNatura3DG.Enabled = false;
                chkTrasfAOI.Checked = true;
                chkTrasfAOI.Enabled = false;
            }
            else
            {
                trTrasfAOI.Visible = false;
            }

            //NEW-Trattenuta
            if (this.domanda.Categoria.StartsWith("V") || (CodeUtility.IsRicostituzione(datiPensione) && !datiPensione.IsPLInvalidita.GetValueOrDefault()) || (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto != "0011"))
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap.Value)
                        ddlTrattINPDAP.SelectedValue = "SI";
                    else if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap.Value == false)
                        ddlTrattINPDAP.SelectedValue = "NO";
                }
                else
                    ddlTrattINPDAP.SelectedIndex = 0;

                if (pnlINPDAP.Visible && CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsDomandaEccezioneMemo86(this.domanda.Categoria, datiPensione.NaturaPensione, datiPensione) && datiPensione.DataPresentazioneDomanda != null &&
                    Utility.DataStrettamenteSuccessivaA(datiPensione.DataPresentazioneDomanda.Value, new DateTime(2022, 02, 20)))
                {
                    HiddenFieldIsRICPost20022022.Value = "SI";
                    if (!String.Equals(ddlTrattINPDAP.SelectedValue, "SI"))
                        ddlTrattINPDAP.SelectedValue = "NO";
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRinunciaTrattenutaInpdap.HasValue)
                    txtDecTrattINPDAP.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRinunciaTrattenutaInpdap.Value);
                else
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap.HasValue)
                    {
                        if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap.Value)
                            txtDecTrattINPDAP.Text = String.Format("{0:MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                    }
                }
            }

            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2DG, liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.Value);
            ManageSperDonna(IsDomandaSperDonna, liquidazione);
            ManageForPensioniVecchiaiaCalcoloContrib(liquidazione, datiPensione);

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && CodeUtility.IsRicostituzione(datiPensione) &&
                liquidazione.areaLiquidazionePensioneFS.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault())
            {
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }

            if (anagrafica != null && anagrafica.DataMorte.HasValue && anagrafica.DataMorte.Value != DateTime.MinValue)
            {
                ddlElContCodice.SelectedValue = "10";
                ddlElContCodice.Enabled = false;
                txtElContDataEvento.Text = String.Format("{0:dd/MM/yyyy}", anagrafica.DataMorte.Value);
                txtElContDataEvento.Enabled = false;
                HiddenFieldIsRicostituzione.Value = "SI";
            }

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                    ddlTipoCalcolo.Enabled = false;

                //ENG - sulle ricostituzioni della nuova opzione donna rendere non editabili tutti i campi del pannello liquidazione pensione – generici ad eccezione della “data completezza”, “decorrenza arretrati” e primo codice natura
                if (CodeUtility.IsRicostituzione(datiPensione) && liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                {
                    ddlCodNatura1DG.Enabled = true;
                    ddlCodNatura3DG.Enabled = false;
                    txtInteressiLegali.Enabled = false;
                    txtScadRevSanitaria.Enabled = false;
                    ddlCodiciArretrati.Enabled = false;
                    txtDataRivalsa.Enabled = false;
                    ddlTipoCalcolo.Enabled = false;
                    ddlCausaCarico.Enabled = false;
                    if (!liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                        ddlCodComunicazioni4.Enabled = false;
                    chkTrasfAOI.Enabled = false;
                    chkExCombattente.Enabled = false;
                    chkBenefici.Enabled = false;
                }

                string CodFase = GetCodFase(domanda.NumeroDomanda);
                if (Utility.IsRicostituzione(datiPensione) || Utility.IsRiaperturaDomanda(CodFase))
                {
                    ddlElContCodice.Enabled = false;
                    txtElContDecorrenza.Enabled = false;
                    txtElContDataEvento.Enabled = false;
                }

                //ENG - TFR della nuova opzione donna rendere non editabile il terzo byte del codice natura
                if (this.domanda.IsDomandaRiapertura && liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
                {
                    ddlCodNatura3DG.Enabled = false;
                }

                //ENG - PL o TRF GDP ddlCodComunicazioni4 disabled se il titolare non è residente all'estero
                if (Utility.IsDomandaPL(datiPensione, false) && !liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                {
                    ddlCodComunicazioni4.Enabled = false;
                }

                //ENG - RIC GDP ddlCodComunicazioni3 disabled, ddlCodComunicazioni4 disabled se il titolare non è residente all'estero
                //ENG - RIC/TRF INPDAP: il campo deve essere sempre sbloccato
                if (CodeUtility.IsRicostituzione(datiPensione) || domanda.IsDomandaRiapertura)
                {
                    //ddlCodComunicazioni3.Enabled = false;                 
                    ddlCodComunicazioni4.Enabled = true;
                }

                if ((CodeUtility.IsRicostituzione(datiPensione) || domanda.IsDomandaRiapertura) && ddlCodNatura3DG.SelectedValue == "Z")
                {
                    ddlCodNatura3DG.Enabled = false;
                }

                if (Utility.IsDomandaPL(datiPensione, domanda.IsDomandaRiapertura) && datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0140" && ddlCodNatura3DG.SelectedValue == "Z")
                {
                    ddlCodNatura3DG.Enabled = false;
                }
            }

            //ENG - GDP - RIC CONCESSIONE ALTRA PENSIONE
            if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione))
                ddlTipoCalcolo.Enabled = false;
            if (Utility.isDomandaGiornalistiDipendentiConSistemaPrivato(datiPensione))
            {
                ddlTipoCalcolo.Visible = false;
                lblTipoCalcolo.Visible = false;
            }

            if(Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
                hdnIsRicEsenzioneFiscaleVittimeDelDovere.Value = "SI";

            CodeUtility.ManageRecordEsenzioneFiscale(ref ddlCodComunicazioni4, Utility.IsRicostituzione(datiPensione), Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione));

        }

        private void ManageCodNatura2PerPersonaleViaggiante(DropDownList ddlCodNatura2)
        {
            if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault())
            {
                ddlCodNatura2.SelectedValue = "W";
                ddlCodNatura2.Enabled = false;
            }
            else if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault())
            {
                ddlCodNatura2.SelectedValue = "K";
                ddlCodNatura2.Enabled = false;
            }
        }

        internal DatiGenericiINPDAP GetDatiGenerici()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiGenericiINPDAP = new DatiGenericiINPDAP();
            areaLiquidazionePensioneFS.DatiGenericiINPDAP = GetDatiGenericiCommon();

            return areaLiquidazionePensioneFS.DatiGenericiINPDAP;
        }

        internal void ClearBonusSection()
        {
            if (!String.Equals(ddlCodNatura2DG.SelectedValue, "Y"))
            {
                ddlAttribuzioneBonus.SelectedIndex = 0;
                txtDataInizioBonus.Text = "GG/MM/AAAA";
                txtDataFineBonus.Text = "GG/MM/AAAA";
            }
        }

        #endregion protected methods

        #region private methods

        internal void SetDdlCodiceComunicazione3(AreaDecodifica datiDecodifica, ILiquidazionePensione liquidazione)
        {
            ddlCodComunicazioni3.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampo3 comunicazioneCampo3 in datiDecodifica.ElencoComunicazioneCampo3)
            {
                switch (comunicazioneCampo3.Id)
                {
                    case "Y":
                        if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.Equals('Y'))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "":
                        CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "P":
                        if (ViewState["IsProvvisoriaVisible"] != null && (bool)ViewState["IsProvvisoriaVisible"] &&
                              ((liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3 != null
                              && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals("P"))
                              ||
                             ((liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.HasValue) &&
                             (liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico == null || !liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.HasValue
                             || liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals("P")))))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    default:
                        if (ViewState["IsProvvisoriaVisible"] != null && (bool)ViewState["IsProvvisoriaVisible"] &&
                            (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals(comunicazioneCampo3.Id.Trim().ToUpperInvariant()))
                            ||
                            ((liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.HasValue) &&
                            (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.HasValue
                            && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals(comunicazioneCampo3.Id.Trim().ToUpperInvariant()))))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                }
            }
            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null
                           && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3 != null)
                ddlCodComunicazioni3.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.ToString();

            if (checkMemo74_88())
            {
                ddlCodComunicazioni3.SelectedValue = "P";
                ddlCodComunicazioni3.Enabled = false;
            }
        }

        private string GetCodFase(string NumeroDomanda)
        {
            string CodFase = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Presenter.SvrLiquidazione.AreaEsito esito = objWS.GetCodFaseByNDomus(out CodFase, NumeroDomanda);
            return CodFase;
        }

        private bool checkMemo74_88()
        {
            bool retVal = false;
            string CodFase = GetCodFase(domanda.NumeroDomanda);
            string Gruppo = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).CodeGruppo;
            string Prodotto = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).CodeProdotto;
            string Caratterizzazione = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).Caratterizzazione;
            string TipoLetturaUnicarpe = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoLetturaUnicarpe.ToString();

            if (Utility.checkMemo74_88(CodFase, Gruppo, Prodotto, Caratterizzazione, TipoLetturaUnicarpe))
            {
                retVal = true;
            }
            return retVal;
        }

        private void BindClick()
        {
            chkBenefici.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkExCombattente.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            ddlCodNatura2DG.Attributes.Add("onChange", "javascript:getDDLCodNatura2Value()");
            chkTrasfAOI.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            txtDataCompletezza.Attributes.Add("onblur", "setDataInteressiLegali()");
        }

        private void AddInputClass()
        {
            chkBenefici.InputAttributes.Add("EnableClass", "onClassBenefici");
            chkExCombattente.InputAttributes.Add("EnableClass", "onClassExCombattente");
            chkTrasfAOI.InputAttributes.Add("EnableClass", "onClassTrasfAOI");
        }

        private void ValorizzaEtichetteCommon(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblDecorrenzaPensione.Text = "Decorrenza Giuridica:";
            lblAnzAnniSperDonnaSemestre.Text = "Anni servizio 247:";
            lblAnzAnniSperDonnaTrimestre.Text = "Anni servizio 247:";

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null)
            {
                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.NaturaPensione))
                {
                    try
                    {
                        ddlCodNatura1DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.NaturaPensione.Substring(0, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura2DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.NaturaPensione.Substring(1, 1);
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    try
                    {
                        ddlCodNatura3DG.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.NaturaPensione.Substring(2, 1);
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

                if (pnlRequisitiAnte247.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                        ddlReqAnte247.SelectedValue = "SI";
                    else if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                        ddlReqAnte247.SelectedValue = "NO";

                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.HasValue)
                        ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.ToString();

                    txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.Value.ToString() : string.Empty;
                    txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.ToString() : string.Empty;
                }
                else
                {
                    if (IsDomandaSperDonna)
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                            ddlSperimentaleDonna.SelectedValue = "SI";
                        else if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                            ddlSperimentaleDonna.SelectedValue = "NO";

                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.HasValue)
                            ddlSemestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.ToString();

                        txtSemestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.ToString() : string.Empty;
                        txtAnzAnniSperDonna.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.ToString() : string.Empty;
                    }
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceMotivo.HasValue)
                    ddlElContCodice.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceMotivo.ToString();
                else ddlElContCodice.SelectedIndex = 0;

                txtElContDecorrenza.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaEliminazione.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaEliminazione) : string.Empty;
                HiddenFieldDecorrenza.Value = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaEliminazione.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaEliminazione) : string.Empty;
                txtElContDataEvento.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataEvento.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataEvento) : string.Empty;
            }

            txtElContDecorrenza.Enabled = false;

            ManageCodNatura3(liquidazione);

            if (!datiPensione.DecorrenzaOriginaria.HasValue)
                lblDecorrenzaPensioneData.Text = string.Empty;
            else
                lblDecorrenzaPensioneData.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
            //valorizza causa carico
            string causaCarico = (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null) ? (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CausaCarico.ToString()) : (string.Empty);
            bool causaCaricoEnabled;
            ddlCausaCarico.SelectedValue = CodeUtility.FS_SelectedValueDdlCausaCaricoByTipoDomanda(GetDatiPensione(this), causaCarico, out causaCaricoEnabled);
            ddlCausaCarico.Enabled = causaCaricoEnabled;

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null)
            {
                txtDecorrenzaArretrati.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaCalcoloArretrati.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaCalcoloArretrati) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceArretrati.HasValue)
                    ddlCodiciArretrati.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceArretrati.ToString();
                else ddlCodiciArretrati.SelectedIndex = 0;

                txtDataCompletezza.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataCompletezza.HasValue ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataCompletezza) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo.HasValue)
                {
                    try
                    {
                        ddlTipoCalcolo.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo.Value.ToString();
                        if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo.Value.ToString()) &&
                            liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo.Value.ToString().Trim() != string.Empty &&
                            (string.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue) || ddlTipoCalcolo.SelectedValue.Trim() == string.Empty))
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);
                        this.HasError = true;
                        this.ErrorMessage = "Tipo calcolo precedentemente salvato non compatibile con l'attuale valore della data perfezionamento requisiti";
                        RaiseShowAvviso(this, Cevent);
                    }
                }
                else ddlTipoCalcolo.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo1.HasValue)
                    ddlCodComunicazioni1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo1.ToString();
                else ddlCodComunicazioni1.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo2.HasValue)
                    ddlCodComunicazioni2.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo2.ToString();
                else ddlCodComunicazioni2.SelectedIndex = 0;

                if (ddlCodComunicazioni1.SelectedValue == "1" || ddlCodComunicazioni1.SelectedValue == "2")
                    ddlCodComunicazioni2.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo2.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.HasValue)
                    ddlCodComunicazioni3.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo4.HasValue)
                {
                    ddlCodComunicazioni4.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo4.ToString();

                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleVittima.GetValueOrDefault())
                    {
                        if (ddlCodComunicazioni4.SelectedValue == "1")
                        {
                            ddlCodComunicazioni4.Enabled = false;
                        }
                    }

                    CodeUtility.ManageRecordEsenzioneFiscale(ref ddlCodComunicazioni4, Utility.IsRicostituzione(datiPensione), Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione));
                }
                else
                {
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    {
                        // codice commentato in base al Memo 349/2025
                        ////tutte le domande di trasformazione e ricostituzione
                        //if (liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleEsteroFromDetrazioni.GetValueOrDefault())
                        //{
                        //    if (ddlCodComunicazioni4.Items.FindByValue("2") != null)
                        //        ddlCodComunicazioni4.SelectedValue = ddlCodComunicazioni4.Items.FindByValue("2").Value;
                        //}

                        if (liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleVittima.GetValueOrDefault())
                        {
                            if (ddlCodComunicazioni4.Items.FindByValue("1") != null)
                            {
                                ddlCodComunicazioni4.SelectedValue = ddlCodComunicazioni4.Items.FindByValue("1").Value;
                                ddlCodComunicazioni4.Enabled = false;
                            }
                        }
                    }
                    else
                        ddlCodComunicazioni4.SelectedIndex = 0;
                }

                txtScadRevSanitaria.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.ScadenzaRevisioneSanitaria.HasValue ? String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.ScadenzaRevisioneSanitaria) : string.Empty;

                chkBenefici.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.Benefici.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.Benefici.Value ? true : false;

                chkExCombattente.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.ExCombattente.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.ExCombattente.Value ? true : false;
                chkTrasfAOI.Checked = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrasformazioneAOI.Value ? true : false;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus == true)
                        ddlAttribuzioneBonus.SelectedValue = "SI";
                    else if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus == false)
                        ddlAttribuzioneBonus.SelectedValue = "NO";
                }
                else ddlAttribuzioneBonus.SelectedIndex = 0;

                txtDataInizioBonus.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.InizioBonus.HasValue && !String.Equals(liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.InizioBonus.ToString().ToLowerInvariant(), "gg/mm/aaaa") ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.InizioBonus.Value) : string.Empty;
                txtDataFineBonus.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.FineBonus.HasValue && !String.Equals(liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.FineBonus.ToString().ToLowerInvariant(), "gg/mm/aaaa") ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.FineBonus.Value) : string.Empty;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataInteressiLegali.HasValue)
                    txtInteressiLegali.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataInteressiLegali.Value);

                //**Revisione Campi INPDAP**
                //if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AliquotaMediaINPDAP.HasValue)
                //    txtAliquotaMedia.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AliquotaMediaINPDAP.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRivalsaINPDAP.HasValue)
                    txtDataRivalsa.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRivalsaINPDAP.Value);

                if (pnlRequisitiAnte247.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                        ddlReqAnte247.SelectedValue = "SI";
                    else if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                        ddlReqAnte247.SelectedValue = "NO";

                    if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.HasValue)
                        ddlTrimestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.ToString();

                    txtTrimestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.Value.ToString() : string.Empty;
                    txtAnzAnni.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.ToString() : string.Empty;
                }
                else
                {
                    if (IsDomandaSperDonna)
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                            ddlSperimentaleDonna.SelectedValue = "SI";
                        else if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.HasValue && !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247.Value)
                            ddlSperimentaleDonna.SelectedValue = "NO";

                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.HasValue)
                            ddlSemestreRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti.ToString();

                        txtSemestreRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti.ToString() : string.Empty;
                        txtAnzAnniSperDonna.Text = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni.ToString() : string.Empty;
                    }
                }
            }

            if (IsDomandaSperDonna)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo.HasValue)
                {
                    ddlTipoCalcolo.ClearSelection();
                    if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                        ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                }
                ddlTipoCalcolo.Enabled = false;

                if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP == null)
                {
                    ddlSperimentaleDonna.ClearSelection();
                    ddlSperimentaleDonna.SelectedValue = "SI";
                    txtAnzAnniSperDonna.Text = "35";
                }
                ddlSperimentaleDonna.Enabled = false;
            }

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                ddlTipoCalcolo.Enabled = false;
                ddlCodNatura1DG.Enabled = true;
                ddlCodNatura2DG.Enabled = true;
                ddlCodNatura3DG.Enabled = true;
                ddlCodComunicazioni3.Enabled = false;
                if (!liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                    ddlCodComunicazioni4.Enabled = false;
                if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsBeneficioNonVedente.GetValueOrDefault())
                    chkBenefici.Checked = true;
            }

            if ((Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura) ||
               CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)) && ddlCodNatura3DG.SelectedValue == "V")
                ddlCodNatura3DG.Enabled = false;

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) || Utility.IsDomandaReversibilita(datiPensione) || Utility.IsCTPSPrivilegio(datiPensione, this.domanda.Categoria))
            {
                ManageCodNatura1();
            }

            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsBeneficioNonVedenteFromStorico.GetValueOrDefault())
            {
                chkBenefici.Checked = true;
                chkBenefici.Enabled = false;
            }

            if (CodeUtility.IsTipoContributivoConOpzione(datiPensione, liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivoConOpzione) || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
            {
                ddlCodNatura2DG.ClearSelection();
                if (ddlCodNatura2DG.Items.FindByValue("J") != null)
                    ddlCodNatura2DG.SelectedValue = "J";
                ddlCodNatura2DG.Enabled = false;
            }

            if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione || (datiPensione.SceltaLavoratriciMadri.HasValue && datiPensione.SceltaLavoratriciMadri != 0) || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                chkBenefici.Checked = true;

            if ((this.domanda.Categoria.StartsWith("V") || (datiPensione.CodeGruppo == "0002" && datiPensione.CodeProdotto != "0011") ||
                (CodeUtility.IsRicostituzione(datiPensione) && !datiPensione.IsPLInvalidita.GetValueOrDefault())) && !Utility.IsDomandaEccezioneMemo86(this.domanda.Categoria, datiPensione.NaturaPensione, datiPensione))
                pnlINPDAP.Visible = true;

            if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaReversibilita(datiPensione))
            {
                ddlTipoCalcolo.Enabled = false;
                btnEliminaDatiGenerici.Enabled = false;
            }

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
            {
                ddlCodComunicazioni3.ClearSelection();
                ddlCodComunicazioni3.Enabled = false;
            }

            if (Utility.IsRicostituzione_ProvenienteDaListePensioniDaVerificare(datiPensione) || Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) || Utility.IsRicostituzioneVariazioneDatiContitolari(datiPensione))
            {
                ddlCodComunicazioni3.Enabled = false;
                if (!liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                    ddlCodComunicazioni4.Enabled = false;
                pnlINPDAP.Enabled = false;
            }

            if (ConfigurationManager.AppSettings["DPRArmonizzazioneVOCPDEL"] != null && ConfigurationManager.AppSettings["DPRArmonizzazioneVOCPDEL"] == "SI")
            {
                // DPR Armonizzazione
                if (liquidazione.areaLiquidazionePensioneFS.ListaPersonaleViaggiante != null)
                    ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()] = liquidazione.areaLiquidazionePensioneFS.ListaPersonaleViaggiante.ToList();

                ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL.ToString()] = liquidazione.areaLiquidazionePensioneFS.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL;
                ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL.ToString()] = liquidazione.areaLiquidazionePensioneFS.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL;

                ManageCodNatura2PerPersonaleViaggiante(ddlCodNatura2DG);
                //----------------------
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null)
            {
                if (ConfigurationManager.AppSettings["DPRArmonizzazioneVOCPDEL"] != null && ConfigurationManager.AppSettings["DPRArmonizzazioneVOCPDEL"] == "SI")
                {
                    if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault() || (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault()))
                    {
                        pnlPersonaleViaggiante.Visible = true;
                        ddlPersonaleViaggiante.Visible = true;


                        // DPR Armonizzazione
                        ddlPersonaleViaggiante.Items.Clear();
                        CodeUtility.SetValueDdl(ddlPersonaleViaggiante, string.Empty, string.Empty, string.Empty);
                        foreach (PersonaleViaggiante personaleViaggiante in (List<PersonaleViaggiante>)ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()])
                            CodeUtility.SetValueDdl(ddlPersonaleViaggiante, personaleViaggiante.TraduzioneSuGP + " - " + personaleViaggiante.Descrizione, personaleViaggiante.Descrizione, personaleViaggiante.Id.ToString());


                        if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.PersonaleViaggiante.HasValue)
                            ddlPersonaleViaggiante.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.PersonaleViaggiante.Value.ToString();
                        else
                        {
                            List<PersonaleViaggiante> listaPersonaleViaggiante = (List<PersonaleViaggiante>)ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()];

                            if (listaPersonaleViaggiante != null)
                            {
                                PersonaleViaggiante personaleViaggiante = null;
                                if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault())
                                {
                                    personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 1);
                                    if (personaleViaggiante != null)
                                        ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                                }
                                else if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault())
                                {
                                    personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 2);
                                    if (personaleViaggiante != null)
                                        ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                if (ConfigurationManager.AppSettings["DPRArmonizzazioneVOCPDEL"] != null && ConfigurationManager.AppSettings["DPRArmonizzazioneVOCPDEL"] == "SI")
                {
                    // DPR armonizzazione
                    List<PersonaleViaggiante> listaPersonaleViaggiante = (List<PersonaleViaggiante>)ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()];
                    if (listaPersonaleViaggiante != null)
                    {
                        PersonaleViaggiante personaleViaggiante = null;
                        if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault())
                        {
                            personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 1);
                            if (personaleViaggiante != null)
                                ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                        }
                        else if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL.ToString()]).GetValueOrDefault())
                        {
                            personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 2);
                            if (personaleViaggiante != null)
                                ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                        }
                    }
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.IsMiglioramentiContrattualiAutomatici.GetValueOrDefault())
            {
               txtDecorrenzaArretrati.Text =  string.Format("{0:MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
               txtDecorrenzaArretrati.Enabled = false;
            }
        }

        private DatiGenericiINPDAP GetDatiGenericiCommon()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiGenericiINPDAP = new DatiGenericiINPDAP();
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaCalcoloArretrati = (!String.IsNullOrEmpty(txtDecorrenzaArretrati.Text)) && (!String.Equals(txtDecorrenzaArretrati.Text.ToLowerInvariant(), "mm/aaaa")) ? Utility.GetDateFromString(txtDecorrenzaArretrati.Text) : (DateTime?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceArretrati = !String.IsNullOrEmpty(ddlCodiciArretrati.SelectedValue) ? byte.Parse(ddlCodiciArretrati.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.ScadenzaRevisioneSanitaria = (!String.IsNullOrEmpty(txtScadRevSanitaria.Text)) && (!String.Equals(txtScadRevSanitaria.Text.ToLowerInvariant(), "gg/mm/aaaa")) ? Utility.GetDateFromString(txtScadRevSanitaria.Text) : (DateTime?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataCompletezza = !String.Equals(txtDataCompletezza.Text.ToLowerInvariant(), "gg/mm/aaaa") ? Utility.GetDateFromString(txtDataCompletezza.Text) : (DateTime?)null;
            if (Utility.isDomandaGiornalistiDipendentiConSistemaPrivato(datiPensione))
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo = (byte?)18;
            else
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.TipoCalcolo = !String.IsNullOrEmpty(ddlTipoCalcolo.SelectedValue) ? byte.Parse(ddlTipoCalcolo.SelectedValue) : (byte?)null;

            string naturaPensione = "";
            naturaPensione = String.Concat(ddlCodNatura1DG.SelectedValue, ddlCodNatura2DG.SelectedValue, ddlCodNatura3DG.SelectedValue);
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.NaturaPensione = naturaPensione;

            if (ddlAttribuzioneBonus.SelectedValue == "SI")
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus = true;
            else if (ddlAttribuzioneBonus.SelectedValue == "NO")
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus = false;
            else
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus = null;

            areaLiquidazionePensioneFS.DatiGenericiINPDAP.InizioBonus = (!String.IsNullOrEmpty(txtDataInizioBonus.Text)) && (!String.Equals(txtDataInizioBonus.Text.ToLowerInvariant(), "gg/mm/aaaa")) ? Utility.GetDateFromString(txtDataInizioBonus.Text) : (DateTime?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.FineBonus = (!String.IsNullOrEmpty(txtDataFineBonus.Text)) && (!String.Equals(txtDataFineBonus.Text.ToLowerInvariant(), "gg/mm/aaaa")) ? Utility.GetDateFromString(txtDataFineBonus.Text) : (DateTime?)null;

            if (!String.Equals(ddlCodNatura2DG.SelectedValue, "Y"))
            {
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.AttribuzioneBonus = null;
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.InizioBonus = null;
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.FineBonus = null;
            }

            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CausaCarico = !String.IsNullOrEmpty(ddlCausaCarico.SelectedValue) ? byte.Parse(ddlCausaCarico.SelectedValue) : (byte?)null;

            if (!String.IsNullOrEmpty(ddlCodComunicazioni1.SelectedValue))
            {
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo1 = byte.Parse(ddlCodComunicazioni1.SelectedValue);
                if (String.Equals(ddlCodComunicazioni1.SelectedValue, "1"))
                {
                    if (!String.IsNullOrEmpty(ddlCodComunicazioni2.Text))
                    {
                        if (!String.Equals(ddlCodComunicazioni2.Text, " "))
                            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo2 = char.Parse(ddlCodComunicazioni2.SelectedValue);
                    }
                }
                else if (String.Equals(ddlCodComunicazioni1.SelectedValue, "2"))
                {
                    if (!String.IsNullOrEmpty(ddlCodComunicazioni2.Text))
                        if (!String.Equals(ddlCodComunicazioni2.Text, " "))
                            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo2 = char.Parse(ddlCodComunicazioni2.SelectedValue);
                }
            }


            if (!String.IsNullOrEmpty(ddlCodComunicazioni3.SelectedValue))
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3 = char.Parse(ddlCodComunicazioni3.SelectedValue);

            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo3 = !String.IsNullOrEmpty(ddlCodComunicazioni3.SelectedValue) ? char.Parse(ddlCodComunicazioni3.SelectedValue) : (char?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo4 = !String.IsNullOrEmpty(ddlCodComunicazioni4.SelectedValue) ? byte.Parse(ddlCodComunicazioni4.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.Benefici = chkBenefici.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.ExCombattente = chkExCombattente.Checked ? true : false;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrasformazioneAOI = chkTrasfAOI.Checked ? true : false;

            if (!string.IsNullOrEmpty(HiddenIntLeg.Value) && !HiddenIntLeg.Value.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataInteressiLegali = Utility.GetDateFromString(HiddenIntLeg.Value);
            else
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataInteressiLegali = null;

            //**Revisione Campi INPDAP**
            //if (!string.IsNullOrEmpty(txtAliquotaMedia.Text))
            //    areaLiquidazionePensioneFS.DatiGenericiINPDAP.AliquotaMediaINPDAP = CodeUtility.StringToNullableDecimal(txtAliquotaMedia.Text);

            if (!string.IsNullOrEmpty(txtDataRivalsa.Text))
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRivalsaINPDAP = Utility.GetDateFromString(txtDataRivalsa.Text);

            areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceMotivo = !String.IsNullOrEmpty(ddlElContCodice.SelectedValue) ? byte.Parse(ddlElContCodice.SelectedValue) : (byte?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.DecorrenzaEliminazione = (!String.Equals(HiddenFieldDecorrenza.Value.ToLowerInvariant(), "mm/aaaa")) && (!(String.IsNullOrEmpty(HiddenFieldDecorrenza.Value))) ? Utility.GetDateFromString(HiddenFieldDecorrenza.Value) : (DateTime?)null;
            areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataEvento = (!String.Equals(txtElContDataEvento.Text.ToLowerInvariant(), "gg/mm/aaaa")) && (!(String.IsNullOrEmpty(txtElContDataEvento.Text))) ? Utility.GetDateFromString(txtElContDataEvento.Text) : (DateTime?)null;

            if (pnlRequisitiAnte247.Visible)
            {
                if (String.Equals(ddlReqAnte247.SelectedValue, "SI"))
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247 = true;
                else if (String.Equals(ddlReqAnte247.SelectedValue, "NO"))
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247 = false;

                areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti = !String.IsNullOrEmpty(ddlTrimestreRequisiti.SelectedValue) ? byte.Parse(ddlTrimestreRequisiti.SelectedValue) : (!string.IsNullOrEmpty(hdnTrimesteRequisiti.Value) ? byte.Parse(hdnTrimesteRequisiti.Value) : (byte?)null);
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti = (!String.Equals(txtTrimestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtTrimestreRequisiti.Text))) ? Int16.Parse(txtTrimestreRequisiti.Text) : (!string.IsNullOrEmpty(hdnTrimesteRequisitiAnno.Value) ? Int16.Parse(hdnTrimesteRequisitiAnno.Value) : (short?)null);
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni = (!String.Equals(txtAnzAnni.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnni.Text))) ? Int32.Parse(txtAnzAnni.Text) : (int?)null;
            }
            else
            {
                if (this.pnlSperimentaleDonna.Visible)
                {
                    if (String.Equals(ddlSperimentaleDonna.SelectedValue, "SI"))
                        areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247 = true;
                    else if (String.Equals(ddlSperimentaleDonna.SelectedValue, "NO"))
                        areaLiquidazionePensioneFS.DatiGenericiINPDAP.RequisitiAnte247 = false;

                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrimesteRequisiti = !String.IsNullOrEmpty(ddlSemestreRequisiti.SelectedValue) ? byte.Parse(ddlSemestreRequisiti.SelectedValue) : (byte?)null;
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnnoRequisiti = (!String.Equals(txtSemestreRequisiti.Text.ToLowerInvariant(), "aaaa")) && (!(string.IsNullOrEmpty(txtSemestreRequisiti.Text))) ? Int16.Parse(txtSemestreRequisiti.Text) : (short?)null;
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.AnzianitaAnni = (!String.Equals(txtAnzAnniSperDonna.Text.ToLowerInvariant(), "aa")) && (!(string.IsNullOrEmpty(txtAnzAnniSperDonna.Text))) ? Int32.Parse(txtAnzAnniSperDonna.Text) : (int?)null;
                }
            }

            //NEW-Trattenuta
            if (pnlINPDAP.Visible)
            {
                if (String.Equals(ddlTrattINPDAP.SelectedValue, "SI"))
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap = true;
                else if (String.Equals(ddlTrattINPDAP.SelectedValue, "NO"))
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap = false;
                else if (String.Equals(ddlTrattINPDAP.SelectedValue, ""))
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.TrattenutaInpdap = null;

                if (!string.IsNullOrEmpty(txtDecTrattINPDAP.Text) && !txtDecTrattINPDAP.Text.ToUpperInvariant().Equals("MM/AAAA"))
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRinunciaTrattenutaInpdap = Utility.GetDateFromString(txtDecTrattINPDAP.Text);
                else
                    areaLiquidazionePensioneFS.DatiGenericiINPDAP.DataRinunciaTrattenutaInpdap = null;
            }

            if (pnlPersonaleViaggiante.Visible)
            {
                long resLong = 0;
                long.TryParse(ddlPersonaleViaggiante.SelectedValue, out resLong);
                areaLiquidazionePensioneFS.DatiGenericiINPDAP.PersonaleViaggiante = resLong != 0 ? resLong : (long?)null;
            }

            return areaLiquidazionePensioneFS.DatiGenericiINPDAP;
        }

        private void RenderControlsCommon(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            pnlCommonRequisitiAnteSperDonna.Visible = true;
            rowCausaCarico.Visible = true;
            pnlDecorrenzaGiuridica.Visible = true;

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value)
                ddlCodNatura2DG.Enabled = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;
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

                    if (!liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.HasValue || !liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.Value)
                        this.pnlSperimentaleDonna.Visible = false;
                    else
                        this.pnlSperimentaleDonna.Visible = true;
                }
                else
                {
                    if (!liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.HasValue || !liquidazione.areaLiquidazionePensioneFS.IsRequisitiL247_L243Enable.Value)
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

            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2DG, liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.Value);

            //ENG - per le pensioni della nuova opzione donna (tipo 0190) il secondo byte del codice natura "O" deve essere sempre selezionato e bloccato
            if (liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.HasValue)
            {
                CodeUtility.DisableCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292(ddlCodNatura2DG, liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.Value);
            }

            if (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || (datiPensione.SceltaLavoratriciMadri.HasValue && datiPensione.SceltaLavoratriciMadri != 0) || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                chkBenefici.Enabled = false;
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

            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                ddlCodComunicazioni3.Enabled = false;
        }

        private void GestioneEtichetteRipristini(AreaTitolare.DatiPensione datiPensione)
        {
            ddlTipoCalcolo.Enabled = false;
            chkTrasfAOI.Enabled = false;
        }

        private void LoadDdlCommon(ILiquidazionePensione liquidazione, AreaDecodifica datiDecodifica, AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            ddlTipoCalcolo.Items.Clear();
            ddlTipoCalcolo.Items.Add(new ListItem(string.Empty, " "));
            foreach (TipoCalcolo tipoCalcolo in liquidazione.areaLiquidazionePensioneFS.ListaTipoCalcolo)
                CodeUtility.SetValueDdl(ddlTipoCalcolo, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);

            //load ddl causa carico
            AreaDecodifica.DatiCausaCarico[] listaCausaCarico = CodeUtility.FS_GetDdlCausaCaricoByTipoDomanda(GetDatiPensione(this), datiDecodifica.ElencoCausaCarico).ToArray();
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
                if (comunicazioneCampo4.Id == string.Empty)
                    comunicazioneCampo4.Descrizione = "NESSUNA ESENZIONE";

                if (CodeUtility.LoadRecordEsenzioneFiscaleFS(comunicazioneCampo4.Id, liquidazione.areaLiquidazionePensioneFS.IsEsenzioneFiscaleEstero, liquidazione.areaLiquidazionePensioneFS.IsCodComunicazioniEsenzioneFiscaleVittimaVisibile, tipoFondo, this.domanda.IsDomandaINPDAP, this.domanda.IsDomandaRiapertura, datiPensione))
                    if (!this.domanda.Categoria.StartsWith("S") && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)
                            && comunicazioneCampo4.Id == "1")
                        CodeUtility.SetValueDdl(ddlCodComunicazioni4, "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", comunicazioneCampo4.Id);
                    else
                        if ((Utility.IsRicostituzione(datiPensione) || this.domanda.IsDomandaRiapertura)
                           && comunicazioneCampo4.Id == "2")
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.GetValueOrDefault())
                            CodeUtility.SetValueDdl(ddlCodComunicazioni4, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
                    }
                    else
                        CodeUtility.SetValueDdl(ddlCodComunicazioni4, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
            }

            //ENG - INPDAP -  Esezione Fiscale Vittima
            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) || Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null
                    && liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.HasValue && liquidazione.areaLiquidazionePensioneFS.IsResidenteEstero.Value &&
                    (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo4.HasValue || liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.CodiceComunicazioneCampo4.Value != 1))
                {
                    if (ddlCodComunicazioni4.Items.FindByValue("1") != null)
                    {
                        ddlCodComunicazioni4.Items.Remove(ddlCodComunicazioni4.Items.FindByValue("1"));
                    }
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura.Count() > 0)
            {
                ddlCodNatura1DG.Items.Clear();
                ddlCodNatura2DG.Items.Clear();
                ddlCodNatura3DG.Items.Clear();

                CodeUtility.SetValueDdl(ddlCodNatura2DG, string.Empty, string.Empty, " ");

                foreach (Presenter.SvrLiquidazioneFs.CodiciNatura codiceNatura in liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura)
                    if (codiceNatura.Posizione == 1)
                        CodeUtility.SetValueDdl(ddlCodNatura1DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                    else if (codiceNatura.Posizione == 2)
                        CodeUtility.SetValueDdl(ddlCodNatura2DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                    else
                        CodeUtility.SetValueDdl(ddlCodNatura3DG, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
            }

            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceEliminazione != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceEliminazione.Count() > 0)
            {
                if (ddlElContCodice.Items.Count == 0)
                {
                    CodeUtility.SetValueDdl(ddlElContCodice, string.Empty, string.Empty, string.Empty);
                    foreach (Presenter.SvrLiquidazioneFs.CodiceEliminazione codeEliminazione in liquidazione.areaLiquidazionePensioneFS.ListaCodiceEliminazione)
                        CodeUtility.SetValueDdl(ddlElContCodice, codeEliminazione.TestoVideo, codeEliminazione.Descrizione, codeEliminazione.Id);
                }
            }


        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        internal void SetHiddenPrecedentePensioneValue(string value)
        {
            this.HiddenPrecedentePensione.Value = value;
        }

        private void SetDefaultValue()
        {
            txtScadRevSanitaria.Text = "gg/mm/aaaa";
            txtDataInizioBonus.Text = "gg/mm/aaaa";
            txtDataFineBonus.Text = "gg/mm/aaaa";
            txtTrimestreRequisiti.Text = "aaaa";
            txtAnzAnni.Text = "aa";
            txtSemestreRequisiti.Text = "aaaa";
            txtAnzAnniSperDonna.Text = "aa";
        }

        private void ManageProvvisoria(ILiquidazionePensione liquidazione)
        {
            //if (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP != null)
            //{
            //    ddlCodComunicazioni3.Enabled = !Utility.IsDomandaProvvisoria(liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.IsProvvisoria);
            //}

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceComunicazione3Visible.HasValue)
            {
                ddlCodComunicazioni3.Visible = liquidazione.areaLiquidazionePensioneFS.IsCodiceComunicazione3Visible.Value;
            }
        }

        private void ManageCodNatura3(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
            {
                if (!ddlCodNatura3DG.Items.Contains(new ListItem("Z", "Z")))
                    ddlCodNatura3DG.Items.Add(new ListItem("Z", "Z"));
                ddlCodNatura3DG.SelectedValue = "Z";
                ddlCodNatura3DG.Enabled = false;
            }

        }

        private void ManageCodNatura1()
        {
            if (ddlCodNatura1DG.Items.Contains(new ListItem("7", "7")))
            {
                ddlCodNatura1DG.SelectedValue = "7";
                ddlCodNatura1DG.Enabled = false;
            }
        }

        private void ManageSperDonna(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione)
        {
            if (IsDomandaSperDonna)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && (liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP == null ||
                    string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiGenericiINPDAP.NaturaPensione)))
                {
                    ddlCodNatura2DG.ClearSelection();
                    if (ddlCodNatura2DG.Items.FindByValue("O") != null)
                        ddlCodNatura2DG.SelectedValue = "O";
                }
                ddlCodNatura2DG.Enabled = false;
            }
        }

        private void ManageForPensioniVecchiaiaCalcoloContrib(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            //FG - Controlli tipo contributivo 
            if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivo.GetValueOrDefault() || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione
                || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))) //ENG - MEMO 166/2023
            {
                var itemTipoCalcolo = ddlTipoCalcolo.Items.FindByText("Contributivo");
                if (itemTipoCalcolo != null)
                {
                    ddlTipoCalcolo.SelectedValue = itemTipoCalcolo.Value;
                    ddlTipoCalcolo.Enabled = false;
                }
            }
        }
        #endregion private methods

        #region Events

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event Utility.CustomEventHandler ShowError;

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            if (ShowAvvisoElimina != null)
                ShowAvvisoElimina(sender, e);
        }

        protected void RaiseShowError(object sender, Utility.CustomEventArgs e)
        {
            if (ShowError != null)
                ShowError(sender, e);
        }

        #endregion Events

        public enum EnumViewState
        {
            ListaPersonaleViaggiante,
            IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitanteVOCPDEL,
            IsDomandaPersonaleViaggianteConPerditaTitoloAbilitanteVOCPDEL

        }
    }
}