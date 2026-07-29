using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using System;
using System.Linq;
using System.Web.UI;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi
{
    public partial class UCDatiGenerici : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneCi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneCi
        public AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion ILiquidazionePensioneCi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        internal void ValorizzaEtichetteDatiGenerici(ILiquidazionePensioneCi liquidazioneCi)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);

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

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(titolare.Pensione.CodeGruppo, titolare.Pensione.CodeProdotto, titolare.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            //ENG - Memo 28/2024
            string ctrlAbilitazioneMemo28 = string.Empty;
            if (ViewState["AbilitazioneMemo28_2024"] != null)
                ctrlAbilitazioneMemo28 = (string)ViewState["AbilitazioneMemo28_2024"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo28_2024", out ctrlAbilitazioneMemo28);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneMemo28_2024"] = ctrlAbilitazioneMemo28;
            }


            ManageTrasformazioneAOI(titolare.Pensione);
            ManageCheckBenefici(titolare.Pensione, liquidazioneCi);
            ManageConfermeInvalidita(titolare.Pensione, liquidazioneCi);
            ManageModalitaLiquidazione(titolare.Pensione);
            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
                CodeUtility.ManagePanelEsenzioneFiscaleAGO_CI(ref pnlEsenzioneFiscale, liquidazioneCi.areaLiquidazionePensioneCi.IsEsenzioneFiscaleEstero.Value, titolare.Pensione.CodeGruppo, this.domanda.IsDomandaRiapertura);

            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
                ddlCodNatura1DG.Enabled = liquidazioneCi.areaLiquidazionePensioneCi.IsVecchiaiaInvaliditaSupplementare.HasValue ? !liquidazioneCi.areaLiquidazionePensioneCi.IsVecchiaiaInvaliditaSupplementare.Value : true;

            if (CodeUtility.IsTipoContributivoConOpzione(titolare.Pensione, liquidazioneCi.areaLiquidazionePensioneCi.IsPensioneTipoContributivoConOpzione)
                || titolare.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)
                ddlCodNatura2DG.Enabled = false;

            if (titolare.Pensione.DecorrenzaOriginaria.HasValue)
            {
                string inputDecorrenza = titolare.Pensione.DecorrenzaOriginaria.ToString();
                lblDecorrenzaPensioneDatiGenerici.Text = String.Format("{0:MM/yyyy}", titolare.Pensione.DecorrenzaOriginaria.Value);
            }

            LoadDdl(liquidazioneCi, titolare.Pensione);

            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.IsRichiestaBonusBookingAbilitata.GetValueOrDefault() || liquidazioneCi.areaLiquidazionePensioneCi.IsRichiestaBonus154Abilitata.GetValueOrDefault())
                {
                    ManageRichiestaBonus(titolare.Pensione, liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici);
                }
                ddlCodNatura1DG.ClearSelection();
                if (!string.IsNullOrEmpty(liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione))
                {
                    if (ddlCodNatura1DG.Items.FindByValue(liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(0, 1)) != null)
                        ddlCodNatura1DG.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(0, 1);
                    if (ddlCodNatura2DG.Items.FindByValue(liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(1, 1)) != null)
                    {
                        ddlCodNatura2DG.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(1, 1);

                        if (liquidazioneCi.areaLiquidazionePensioneCi.IsSperimentaleDonna.GetValueOrDefault())
                            ddlCodNatura2DG.Enabled = false;
                        //ENG - per le pensioni della nuova opzione donna (tipo 0190) il secondo byte del codice natura "O" deve essere sempre selezionato e bloccato
                        if (liquidazioneCi.areaLiquidazionePensioneCi.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.HasValue)
                        {
                            CodeUtility.DisableCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292(ddlCodNatura2DG, liquidazioneCi.areaLiquidazionePensioneCi.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.Value);
                        }
                    }
                    if (ddlCodNatura3DG.Items.FindByValue(liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(2, 1)) != null)
                    {
                        ddlCodNatura3DG.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(2, 1);

                        if (liquidazioneCi.areaLiquidazionePensioneCi.IsUsuranti.HasValue)
                            ddlCodNatura3DG.Enabled = !liquidazioneCi.areaLiquidazionePensioneCi.IsUsuranti.Value;

                        if (liquidazioneCi.areaLiquidazionePensioneCi.IsTrasformazioneInvalidita.HasValue)
                            ddlCodNatura3DG.Enabled = !liquidazioneCi.areaLiquidazionePensioneCi.IsTrasformazioneInvalidita.Value;
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceArretrati.HasValue)
                    ddlCodiciArretrati.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceArretrati.Value.ToString();
                else
                    ddlCodiciArretrati.SelectedIndex = 0;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaCalcoloArretrati.HasValue)
                    txtDecorrenzaArretrati.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaCalcoloArretrati.Value);

                if (tipologiaGruppoPensione != CodeUtility.TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia)
                {
                    //ENG - Il campo Data Revisione Sanitaria non deve essere valorizzato per le pensioni con primo byte siglacategoria = 'V' o 'S'
                    if (this.domanda.Categoria != null && (this.domanda.Categoria.ToUpperInvariant().StartsWith("V") || this.domanda.Categoria.ToUpperInvariant().StartsWith("S")))
                    {
                        pnlScadRevSanitaria.Visible = false;
                    }
                    else
                    {
                        pnlScadRevSanitaria.Visible = true;
                    }

                    //ENG - Il campo Data Revisione Sanitaria non deve essere valorizzato
                    if (!(Utility.IsRicostituzione(titolare.Pensione) && (titolare.Pensione.IsDomandaAnticipataFlessibileOrRicostituzione || titolare.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                         (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                         || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                    {
                        if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ScadenzaRevisioneSanitaria.HasValue)
                            txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ScadenzaRevisioneSanitaria.Value);
                    }
                }

                //ENG - Memo 28_2024
                if (!String.IsNullOrEmpty(ctrlAbilitazioneMemo28) && ctrlAbilitazioneMemo28.Trim().ToUpperInvariant() == "SI")
                {
                    if (Utility.IsRicostituzione(titolare.Pensione) && titolare.Pensione.DecorrenzaOriginaria.HasValue
                        && Utility.DataStrettamenteSuccessivaA(titolare.Pensione.DecorrenzaOriginaria.Value, new DateTime(2024, 1, 1)))
                    {
                        //RIC DI PL 0001/0001/0017
                        if (titolare.Pensione.IdTipoPLPerRIC == 7)
                        {
                            if (!String.IsNullOrEmpty(liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione)
                                && (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(0, 1) == "1" || liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione.Substring(0, 1) == "2"))
                            {
                                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ScadenzaRevisioneSanitaria.HasValue)
                                    txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ScadenzaRevisioneSanitaria.Value);
                            }
                        }
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataCompletezza.HasValue)
                    txtDataCompletezza.Text = String.Format("{0:dd/MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataCompletezza.Value);

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataInteressiLegali.HasValue)
                    txtInteressiLegali.Text = String.Format("{0:dd/MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataInteressiLegali.Value);

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CausaCarico.HasValue)
                    ddlCausaCarico.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CausaCarico.Value.ToString();
                else
                    ddlCausaCarico.SelectedIndex = 0;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataInizioCalcolo.HasValue)
                    txtDataCalcolo.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataInizioCalcolo.Value);

                if (Utility.IsDomandaUnicarpe(titolare.Pensione, true) != Utility.TipoUnicarpe.Automatica &&
                   (liquidazioneCi.areaLiquidazionePensioneCi.IsSperimentaleDonna.GetValueOrDefault() || titolare.Pensione.IsDomandaQuota100OrRicostituzione || titolare.Pensione.IsDomandaQuota102OrRicostituzione))
                {
                    ddlCodMobilita.SelectedIndex = 0;
                    ddlCodMobilita.Enabled = false;
                }
                else
                {
                    if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceMobilita.HasValue)
                        ddlCodMobilita.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceMobilita.Value.ToString();
                    else
                        ddlCodMobilita.SelectedIndex = 0;
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceDomandaRicorso.HasValue)
                    ddlCodDomandaRicorso.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceDomandaRicorso.Value.ToString();
                else
                    ddlCodDomandaRicorso.SelectedIndex = 0;

                if (!chkBenefici.Checked)
                {
                    if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.Benefici.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.Benefici.Value)
                    {
                        chkBenefici.Checked = true;
                        HiddenBenefici.Value = "true";
                    }
                    else
                    {
                        chkBenefici.Checked = false;
                        HiddenBenefici.Value = "false";
                    }
                }

                chkExCombattente.Checked = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ExCombattente.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ExCombattente.Value ? true : false;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.TrattenutaInpdap.HasValue)
                {
                    if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.TrattenutaInpdap.Value)
                        ddlTrattINPDAP.SelectedValue = "SI";
                    else if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.TrattenutaInpdap.Value == false)
                        ddlTrattINPDAP.SelectedValue = "NO";
                }
                else
                    ddlTrattINPDAP.SelectedIndex = 0;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataRinunciaTrattenutaInpdap.HasValue)
                    txtDecTrattINPDAP.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataRinunciaTrattenutaInpdap.Value);

                if (CodeUtility.IsRicostituzione(titolare.Pensione) && !Utility.IsDomandaEccezioneMemo86(this.domanda.Categoria, titolare.Pensione.NaturaPensione, titolare.Pensione) && titolare.Pensione.DataPresentazioneDomanda != null &&
                    Utility.DataStrettamenteSuccessivaA(titolare.Pensione.DataPresentazioneDomanda.Value, new DateTime(2022, 02, 20)))
                {
                    HiddenFieldIsRICPost20022022.Value = "SI";

                    //ENG - Aggiornamento Memo86
                    string controlloDinamicoAggiornamentoMemo86 = string.Empty;
                    Presenter.PresenterControlliDinamici presenterAggiornamentoMemo86 = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esitoCaricamentoControlloDinamicoAggiornamentoMemo86 = presenterAggiornamentoMemo86.GetControlloDinamicoByNomeControllo("DataAttivazioneMemo86Del12_06_2023", out controlloDinamicoAggiornamentoMemo86);

                    if (esitoCaricamentoControlloDinamicoAggiornamentoMemo86 != null
                        && esitoCaricamentoControlloDinamicoAggiornamentoMemo86.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && !String.IsNullOrEmpty(controlloDinamicoAggiornamentoMemo86) && !String.IsNullOrEmpty(controlloDinamicoAggiornamentoMemo86.Trim())
                        && liquidazioneCi.areaLiquidazionePensioneCi.DataPrelievoDomanda.HasValue
                        && Utility.DataSuccessivaA(liquidazioneCi.areaLiquidazionePensioneCi.DataPrelievoDomanda.Value, Utility.DataFromString(controlloDinamicoAggiornamentoMemo86.Trim(), Utility.FormatoData.AAAAmmGG).Value))
                    {
                        VerificaAdesioneFondoCreditoAggiornamentoMemo86(liquidazioneCi.areaLiquidazionePensioneCi.IsPresenteTrattenutaFondoCreditoDaPrelievo, liquidazioneCi.areaLiquidazionePensioneCi.IsDataRinunciaTrattenutaInpdapStorico);
                    }
                    else
                    {
                        if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault())
                        {
                            ddlTrattINPDAP.Enabled = false;
                            txtDecTrattINPDAP.Enabled = false;
                        }
                        else
                            VerificaAdesioneFondoCredito();
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataRicezionePrenotazioneCentrale.HasValue)
                    txtDataPrenotazione.Text = String.Format("{0:dd/MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DataRicezionePrenotazioneCentrale.Value);

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaBonus.HasValue)
                    txtDecorrenzaBonus.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaBonus.Value);

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceComunicazioneCampo4.HasValue)
                {
                    ddlEsenzioneFiscale.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.CodiceComunicazioneCampo4.ToString();

                    if (CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura) && liquidazioneCi.areaLiquidazionePensioneCi.IsEsenzioneFiscaleVittima.GetValueOrDefault())
                    {
                        if (ddlEsenzioneFiscale.SelectedValue == "1")
                        {
                            ddlEsenzioneFiscale.Enabled = false;
                        }
                    }
                }
                else
                {
                    if (CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura))
                    {
                        //tutte le domande di trasformazione e ricostituzione
                        if (liquidazioneCi.areaLiquidazionePensioneCi.IsEsenzioneFiscaleEsteroFromDetrazioni.GetValueOrDefault())
                        {
                            if (ddlEsenzioneFiscale.Items.FindByValue("2") != null)
                                ddlEsenzioneFiscale.SelectedValue = ddlEsenzioneFiscale.Items.FindByValue("2").Value;
                        }

                        if (liquidazioneCi.areaLiquidazionePensioneCi.IsEsenzioneFiscaleVittima.GetValueOrDefault())
                        {
                            if (ddlEsenzioneFiscale.Items.FindByValue("1") != null)
                            {
                                ddlEsenzioneFiscale.SelectedValue = ddlEsenzioneFiscale.Items.FindByValue("1").Value;
                                ddlEsenzioneFiscale.Enabled = false;
                            }
                        }
                    }
                    else
                        ddlEsenzioneFiscale.SelectedIndex = 0;
                }

                //chkDetrazioniEstero.Checked = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.EsenzioneFiscaleEE.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.EsenzioneFiscaleEE.Value ? true : false;
                //chkVittimeTerrorismo.Checked = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.EsenzioneFiscale.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.EsenzioneFiscale.Value ? true : false;

                if (CodeUtility.IsTipoContributivoConOpzione(titolare.Pensione, liquidazioneCi.areaLiquidazionePensioneCi.IsPensioneTipoContributivoConOpzione)
                    || titolare.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura) && (titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                    || (CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                    || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                {
                    chkContributiva.Checked = true;
                    chkContributiva.Enabled = false;
                }
                else if (!(CodeUtility.IsContributivaPura(titolare.Pensione) || liquidazioneCi.areaLiquidazionePensioneCi.IsPensioneTipoContributivoAnzianitàVecchiaia.GetValueOrDefault()))
                    chkContributiva.Checked = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.FlagContributiva.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.FlagContributiva.Value ? true : false;
                if (CodeUtility.IsContributivaPura(titolare.Pensione) || liquidazioneCi.areaLiquidazionePensioneCi.IsPensioneTipoContributivoAnzianitàVecchiaia.GetValueOrDefault())
                    chkContributiva.Enabled = false;
                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.Maggiorazioni.HasValue)
                {
                    chkMaggiorazioni.Checked = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.Maggiorazioni.Value;
                    if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.TrattamentoDisagi.GetValueOrDefault())
                        chkMaggiorazioni.ToolTip = "È presente una richiesta di maggiorazione sociale in domanda";
                }
                else if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.TrattamentoDisagi.GetValueOrDefault())
                {
                    chkMaggiorazioni.Checked = true;
                    chkMaggiorazioni.ToolTip = "È presente una richiesta di maggiorazione sociale in domanda";
                }
                bool? trattamentoDisagi = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.TrattamentoDisagi;
                HiddenTrattamentoDisagi.Value = !trattamentoDisagi.HasValue ? "" : (trattamentoDisagi.Value == true ? "true" : "false");

                if (Utility.IsRicostituzione_Reddituale(titolare.Pensione) && titolare.Pensione.CodeTipo == "0101")
                    chkMaggiorazioni.Checked = true;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NRiconoscimentiInvalidita.HasValue)
                    ddlConfermeInvalidita.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NRiconoscimentiInvalidita.Value.ToString();
                else ddlConfermeInvalidita.SelectedIndex = 0;
                if (!chkRichiestaBonus.Checked)
                    chkRichiestaBonus.Checked = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.IsRichiestaBonus.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.IsRichiestaBonus.Value ? true : false;

                if (CodeUtility.IsRicostituzione(titolare.Pensione) && liquidazioneCi.areaLiquidazionePensioneCi.IsDataRinunciaTrattenutaInpdapStorico.GetValueOrDefault())
                {
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = false;
                }
            }

            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.IsRipristino.GetValueOrDefault())
            {
                ddlCausaCarico.SelectedValue = "9";
                ddlCausaCarico.Enabled = false;
                ddlCodDomandaRicorso.SelectedValue = "9";
                ddlCodDomandaRicorso.Enabled = false;
                txtDataCompletezza.Visible = false;
                validateDataCompletezza.Enabled = false;
                RFDataCompletezza.Enabled = false;
                txtInteressiLegali.Visible = false;
                lblInteressiLegali.Visible = false;
                lblDataCompletezza.Visible = false;
            }

            if (liquidazioneCi != null && !String.IsNullOrEmpty(liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ModalitaLiquidazione))
                ddlModalitaLiquidazione.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.ModalitaLiquidazione;
            else
                ddlModalitaLiquidazione.SelectedIndex = 0;

            string CodFase = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Presenter.SvrLiquidazione.AreaEsito esitoCodFase = objWS.GetCodFaseByNDomus(out CodFase, domanda.NumeroDomanda);
            string Gruppo = titolare.Pensione.CodeGruppo;
            string Prodotto = titolare.Pensione.CodeProdotto;
            string Caratterizzazione = titolare.Pensione.Caratterizzazione;
            string TipoLetturaUnicarpe = titolare.Pensione.TipoLetturaUnicarpe.ToString();

            if (Utility.checkMemo74_88(CodFase, Gruppo, Prodotto, Caratterizzazione, TipoLetturaUnicarpe))
            {
                chkProvvisoria.Checked = true;
                chkProvvisoria.Enabled = false;
            }
            else
                chkProvvisoria.Checked = liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.FlagProvvisoria.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.FlagProvvisoria.Value ? true : false;

            if (titolare.Pensione.FlagUnicarpe.HasValue)
                GestioneEtichetteIsUnicarpe(titolare.Pensione);

            if (titolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura)
            {
                pnlDataPrenotazione.Visible = true;
                txtDataPrenotazione.Enabled = false;
            }

            if (CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura))
            {
                ddlCausaCarico.Enabled = false;
                ddlCodiciArretrati.Enabled = false;
            }

            ManageForPensioniVecchiaiaCalcoloContrib(liquidazioneCi, titolare.Pensione);

            if (CodeUtility.IsRicostituzione(titolare.Pensione) && liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null &&
                liquidazioneCi.areaLiquidazionePensioneCi.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                //ENG - sulle ricostituzioni della nuova opzione donna rendere non editabili tutti i campi del pannello liquidazione pensione – generici ad eccezione della “data completezza”, “decorrenza arretrati” e primo codice natura
                ddlCodNatura1DG.Enabled = true;
                ddlCodNatura3DG.Enabled = false;
                txtScadRevSanitaria.Enabled = false;
                ddlEsenzioneFiscale.Enabled = false;
                chkExCombattente.Enabled = false;
                chkBenefici.Enabled = false;
                HiddenOpzioneDonnaLegge_197_2022.Value = "true";
                chkMaggiorazioni.Enabled = false;
                txtDecorrenzaBonus.Enabled = false;
                chkContributiva.Enabled = false;
            }

            //ENG - TFR della nuova opzione donna rendere non editabile il terzo byte del codice natura
            if (this.domanda.IsDomandaRiapertura && liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null &&
                liquidazioneCi.areaLiquidazionePensioneCi.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                ddlCodNatura3DG.Enabled = false;
            }

            //ENG - Memo 123/2024
            if ((!CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura) && titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura) && titolare.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
            {
                ddlCodNatura2DG.ClearSelection();
                if (ddlCodNatura2DG.Items.FindByValue("J") != null)
                    ddlCodNatura2DG.SelectedValue = "J";
                ddlCodNatura2DG.Enabled = false;
            }
            if (Utility.IsDomandaRipristino(titolare.Pensione))
            {
                txtDataCalcolo.Visible = false;
                lblDecorrenzaRipristino.Visible = false;
            }
        }

        internal DatiGenerici GetDatiGenerici()
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaLiquidazionePensione areaLiquidazionePensioneCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiGenerici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.DatiGenerici();

            string naturaPensione = "";
            naturaPensione = String.Concat(ddlCodNatura1DG.SelectedValue, ddlCodNatura2DG.SelectedValue, ddlCodNatura3DG.SelectedValue);
            areaLiquidazionePensioneCi.DatiGenerici.NaturaPensione = naturaPensione;

            if (!String.IsNullOrEmpty(ddlCodiciArretrati.SelectedValue))
                areaLiquidazionePensioneCi.DatiGenerici.CodiceArretrati = byte.Parse(ddlCodiciArretrati.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiGenerici.CodiceArretrati = null;

            if (!string.IsNullOrEmpty(txtDecorrenzaArretrati.Text) && !txtDecorrenzaArretrati.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaCalcoloArretrati = Utility.GetDateFromString(txtDecorrenzaArretrati.Text);
            else
                areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaCalcoloArretrati = null;

            if (!string.IsNullOrEmpty(txtScadRevSanitaria.Text) && !txtScadRevSanitaria.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiGenerici.ScadenzaRevisioneSanitaria = Utility.GetDateFromString(txtScadRevSanitaria.Text);
            else
                areaLiquidazionePensioneCi.DatiGenerici.ScadenzaRevisioneSanitaria = null;

            if (!string.IsNullOrEmpty(txtDataCompletezza.Text) && !txtDataCompletezza.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneCi.DatiGenerici.DataCompletezza = Utility.GetDateFromString(txtDataCompletezza.Text);
            else
                areaLiquidazionePensioneCi.DatiGenerici.DataCompletezza = null;

            if (areaLiquidazionePensioneCi.DatiGenerici.DataCompletezza.HasValue)
            {
                DateTime DataInteressiLegaliApp = areaLiquidazionePensioneCi.DatiGenerici.DataCompletezza.Value;
                areaLiquidazionePensioneCi.DatiGenerici.DataInteressiLegali = DataInteressiLegaliApp.AddDays(121);
            }
            else
                areaLiquidazionePensioneCi.DatiGenerici.DataInteressiLegali = null;

            if (!String.IsNullOrEmpty(ddlCausaCarico.SelectedValue))
                areaLiquidazionePensioneCi.DatiGenerici.CausaCarico = byte.Parse(ddlCausaCarico.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiGenerici.CausaCarico = null;

            if (!String.IsNullOrEmpty(ddlCausaCarico.SelectedValue) && ddlCausaCarico.SelectedValue == "9")
            {
                if (!string.IsNullOrEmpty(txtDataCalcolo.Text) && !txtDataCalcolo.Text.ToUpperInvariant().Equals("MM/AAAA"))
                    areaLiquidazionePensioneCi.DatiGenerici.DataInizioCalcolo = Utility.GetDateFromString(txtDataCalcolo.Text);
                else
                    areaLiquidazionePensioneCi.DatiGenerici.DataInizioCalcolo = null;
            }

            if (!String.IsNullOrEmpty(ddlCodMobilita.SelectedValue))
                areaLiquidazionePensioneCi.DatiGenerici.CodiceMobilita = byte.Parse(ddlCodMobilita.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiGenerici.CodiceMobilita = null;

            if (!String.IsNullOrEmpty(ddlCodDomandaRicorso.SelectedValue))
                areaLiquidazionePensioneCi.DatiGenerici.CodiceDomandaRicorso = byte.Parse(ddlCodDomandaRicorso.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiGenerici.CodiceDomandaRicorso = null;

            if (!String.IsNullOrEmpty(ddlEsenzioneFiscale.SelectedValue))
                areaLiquidazionePensioneCi.DatiGenerici.CodiceComunicazioneCampo4 = byte.Parse(ddlEsenzioneFiscale.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiGenerici.CodiceComunicazioneCampo4 = null;

            areaLiquidazionePensioneCi.DatiGenerici.Benefici = HiddenBenefici.Value == "true" ? true : false;
            areaLiquidazionePensioneCi.DatiGenerici.ExCombattente = chkExCombattente.Checked == true ? chkExCombattente.Checked : false;
            areaLiquidazionePensioneCi.DatiGenerici.TrasformazioneAOI = chkTrasfAOI.Checked == true ? chkTrasfAOI.Checked : false;
            if (pnlRichiestaBonus.Visible)
            {
                areaLiquidazionePensioneCi.DatiGenerici.IsRichiestaBonus = chkRichiestaBonus.Checked == true ? chkRichiestaBonus.Checked : false;
                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                datiPensione.IsRichiestaBonus = areaLiquidazionePensioneCi.DatiGenerici.IsRichiestaBonus;
                if (datiPensione.CodeTipo != "0167" && chkRichiestaBonus.Checked == true)
                    areaLiquidazionePensioneCi.DatiGenerici.AnnoDecorrenzaBonus = !String.IsNullOrEmpty(txtAnnoBonus.Text) && !txtAnnoBonus.Text.ToUpperInvariant().Equals("AAAA") ? txtAnnoBonus.Text : string.Empty;
                else
                    areaLiquidazionePensioneCi.DatiGenerici.AnnoDecorrenzaBonus = !String.IsNullOrEmpty(hdnAnnoRichiestaBonus14.Value) ? hdnAnnoRichiestaBonus14.Value : string.Empty;
                Session["DatiPensione"] = datiPensione;
            }


            if (String.Equals(ddlTrattINPDAP.SelectedValue, "SI"))
                areaLiquidazionePensioneCi.DatiGenerici.TrattenutaInpdap = true;
            else if (String.Equals(ddlTrattINPDAP.SelectedValue, "NO"))
                areaLiquidazionePensioneCi.DatiGenerici.TrattenutaInpdap = false;
            else if (String.Equals(ddlTrattINPDAP.SelectedValue, ""))
                areaLiquidazionePensioneCi.DatiGenerici.TrattenutaInpdap = null;

            if (!string.IsNullOrEmpty(txtDecTrattINPDAP.Text) && !txtDecTrattINPDAP.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiGenerici.DataRinunciaTrattenutaInpdap = Utility.GetDateFromString(txtDecTrattINPDAP.Text);
            else
                areaLiquidazionePensioneCi.DatiGenerici.DataRinunciaTrattenutaInpdap = null;

            if (!string.IsNullOrEmpty(txtDataPrenotazione.Text) && !txtDataPrenotazione.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneCi.DatiGenerici.DataRicezionePrenotazioneCentrale = Utility.GetDateFromString(txtDataPrenotazione.Text);
            else
                areaLiquidazionePensioneCi.DatiGenerici.DataRicezionePrenotazioneCentrale = null;

            if (!string.IsNullOrEmpty(txtDecorrenzaBonus.Text) && !txtDecorrenzaBonus.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaBonus = Utility.GetDateFromString(txtDecorrenzaBonus.Text);
            else
                areaLiquidazionePensioneCi.DatiGenerici.DecorrenzaBonus = null;

            //areaLiquidazionePensioneCi.DatiGenerici.EsenzioneFiscaleEE = chkDetrazioniEstero.Checked == true ? chkDetrazioniEstero.Checked : false;
            //areaLiquidazionePensioneCi.DatiGenerici.EsenzioneFiscale = chkVittimeTerrorismo.Checked == true ? chkVittimeTerrorismo.Checked : false;
            areaLiquidazionePensioneCi.DatiGenerici.FlagContributiva = chkContributiva.Checked == true ? chkContributiva.Checked : false;
            areaLiquidazionePensioneCi.DatiGenerici.Maggiorazioni = chkMaggiorazioni.Checked == true ? chkMaggiorazioni.Checked : false;

            areaLiquidazionePensioneCi.DatiGenerici.NRiconoscimentiInvalidita = !String.IsNullOrEmpty(ddlConfermeInvalidita.SelectedValue) ? byte.Parse(ddlConfermeInvalidita.SelectedValue) : (byte?)null;

            if (!String.IsNullOrEmpty(ddlModalitaLiquidazione.SelectedValue))
                areaLiquidazionePensioneCi.DatiGenerici.ModalitaLiquidazione = ddlModalitaLiquidazione.SelectedValue;
            else
                areaLiquidazionePensioneCi.DatiGenerici.ModalitaLiquidazione = null;
            areaLiquidazionePensioneCi.DatiGenerici.FlagProvvisoria = chkProvvisoria.Checked == true ? chkProvvisoria.Checked : false;
            areaLiquidazionePensioneCi.DatiGenerici.TrattamentoDisagi = HiddenTrattamentoDisagi.Value == "true" ? true : (HiddenTrattamentoDisagi.Value == "false" ? false : (bool?)null);

            return areaLiquidazionePensioneCi.DatiGenerici;
        }

        internal void SetHiddenPrecedentePensioneValue(string value)
        {
            this.HiddenPrecedentePensione.Value = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
            BindClick();
            AddInputClass();
        }

        protected void SalvaDatiGenerici_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiGenerici = GetDatiGenerici();

            areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            areaRiepilogoDomanda.NumeroDomanda = datiPensione.NDomus.ToString();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiGenericiCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaDatiGenerici_Click(Object sender, EventArgs e)
        {
            areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            areaRiepilogoDomanda.NumeroDomanda = datiPensione.NDomus.ToString();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiGenericiCi(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Generici";
            else
            {
                ClearForm();
                ValorizzaEtichetteDatiGenerici(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        //ENG - Aggiornamento Memo86
        protected void RaiseShowAvvisoTrattenutaFondoCredito(object sender, EventArgs e)
        {
            ShowAvvisoTrattenutaFondoCredito(sender, e);
        }

        private void BindClick()
        {
            chkBenefici.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkExCombattente.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkTrasfAOI.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            chkMaggiorazioni.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
            //sospeso in attesa di indicazioni circa i valori del secondo campo per valore 1° dropdownlist = 1: Sede
            //ddlCodComunicazioni1.Attributes.Add("onChange", "javascript:getDDLCodComunicazioni1Value()");
            ddlCodNatura2DG.Attributes.Add("onChange", "javascript:getDDLCodNatura2Value()");
            //txtInteressiLegali.Attributes.Add("onmouseout", "setDataInteressiLegali()");
            txtDataCompletezza.Attributes.Add("onblur", "setDataInteressiLegali()");
            ddlCausaCarico.Attributes.Add("onChange", "javascript:onChangeCausaCarico()");
        }

        private void AddInputClass()
        {
            chkBenefici.InputAttributes.Add("EnableClass", "onClassBenefici");
            chkExCombattente.InputAttributes.Add("EnableClass", "onClassExCombattente");
            chkTrasfAOI.InputAttributes.Add("EnableClass", "onClassTrasfAOI");
            chkMaggiorazioni.InputAttributes.Add("EnableClass", "onClassMaggiorazioni");
        }

        private void LoadDdl(ILiquidazionePensioneCi liquidazioneCi, AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.listaCodiciNatura != null && liquidazioneCi.areaLiquidazionePensioneCi.listaCodiciNatura.Count() > 0)
                {
                    if (ddlCodNatura1DG.Items.Count == 0 && ddlCodNatura2DG.Items.Count == 0 && ddlCodNatura3DG.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlCodNatura2DG, string.Empty, string.Empty, " ");
                        CodeUtility.SetValueDdl(ddlCodNatura3DG, string.Empty, string.Empty, " ");
                        foreach (Presenter.SvrLiquidazioneCi.CodiciNatura codeNatura in liquidazioneCi.areaLiquidazionePensioneCi.listaCodiciNatura)
                        {
                            if (codeNatura.Posizione == 1)
                                CodeUtility.SetValueDdl(ddlCodNatura1DG, codeNatura.TraduzioneSuGP.ToString(), codeNatura.Descrizione, codeNatura.TraduzioneSuGP.ToString());
                            else if (codeNatura.Posizione == 2)
                                CodeUtility.SetValueDdl(ddlCodNatura2DG, codeNatura.TraduzioneSuGP.ToString(), codeNatura.Descrizione, codeNatura.TraduzioneSuGP.ToString());
                            else
                                CodeUtility.SetValueDdl(ddlCodNatura3DG, codeNatura.TraduzioneSuGP.ToString(), codeNatura.Descrizione, codeNatura.TraduzioneSuGP.ToString());
                        }
                    }
                }
                if (liquidazioneCi.areaLiquidazionePensioneCi.listaDomandaRicorso != null && liquidazioneCi.areaLiquidazionePensioneCi.listaDomandaRicorso.Count() > 0)
                {
                    if (ddlCodDomandaRicorso.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlCodDomandaRicorso, string.Empty, string.Empty, string.Empty);
                        foreach (DomandaRicorso codiceDomandaRicorso in liquidazioneCi.areaLiquidazionePensioneCi.listaDomandaRicorso)
                            CodeUtility.SetValueDdl(ddlCodDomandaRicorso, codiceDomandaRicorso.Descrizione, codiceDomandaRicorso.Descrizione, codiceDomandaRicorso.Id.ToString());
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.listaRiconoscimentiInvalidita != null && liquidazioneCi.areaLiquidazionePensioneCi.listaRiconoscimentiInvalidita.Count() > 0)
                {
                    if (ddlConfermeInvalidita.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlConfermeInvalidita, string.Empty, string.Empty, string.Empty);
                        foreach (DecodificaRiconoscimentiInvalidita codeRiconoscimentiInvalidita in liquidazioneCi.areaLiquidazionePensioneCi.listaRiconoscimentiInvalidita)
                            if (codeRiconoscimentiInvalidita.Id != 0)
                                CodeUtility.SetValueDdl(ddlConfermeInvalidita, string.Concat(codeRiconoscimentiInvalidita.Id.ToString(), " - ", codeRiconoscimentiInvalidita.Descrizione), codeRiconoscimentiInvalidita.Descrizione, codeRiconoscimentiInvalidita.Id.ToString());
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.listaModalitaLiquidazione != null && liquidazioneCi.areaLiquidazionePensioneCi.listaModalitaLiquidazione.Count() > 0)
                {
                    if (ddlModalitaLiquidazione.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlModalitaLiquidazione, string.Empty, string.Empty, string.Empty);
                        foreach (DecModalitaLiquidazione codeModLiquidazione in liquidazioneCi.areaLiquidazionePensioneCi.listaModalitaLiquidazione)
                            CodeUtility.SetValueDdl(ddlModalitaLiquidazione, codeModLiquidazione.Descrizione, codeModLiquidazione.TraduzioneGp.ToString(), codeModLiquidazione.ValoreAggPeco);
                    }
                }

            }

            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            Presenter.SvrLiquidazione.AreaDecodifica.DatiCausaCarico[] listaCausaCarico = datiDecodifica.ElencoCausaCarico;// areaDecodifica.GetValuesDecodifica().ElencoCausaCarico;

            if (ddlCausaCarico.Items.Count == 0)
            {
                CodeUtility.SetValueDdl(ddlCausaCarico, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiCausaCarico causaCarico in listaCausaCarico)
                    // Il codice 2 è ammesso solo per le ricostituzioni e trf
                    if (causaCarico.Id == "1" || causaCarico.Id == "9" || (causaCarico.Id == "2" && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, true)))
                        CodeUtility.SetValueDdl(ddlCausaCarico, causaCarico.Id + " - " + causaCarico.Descrizione, causaCarico.Descrizione, causaCarico.Id);
            }

            AreaDecodifica.DatiCodeMobilita[] listaCodeMobilita = datiDecodifica.ElencoCodeMobilita;

            if (ddlCodMobilita.Items.Count == 0)
            {
                CodeUtility.SetValueDdl(ddlCodMobilita, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiCodeMobilita codeMobilita in listaCodeMobilita)
                    CodeUtility.SetValueDdl(ddlCodMobilita, codeMobilita.Descrizione, codeMobilita.Descrizione, codeMobilita.Id);
            }

            AreaDecodifica.DatiComunicazioneCampo4[] listaComunicazioneC4 = datiDecodifica.ElencoComunicazioneCampo4;

            if (ddlEsenzioneFiscale.Items.Count == 0)
            {
                bool? isEsenzioneFiscaleEstero = null;
                if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
                    isEsenzioneFiscaleEstero = liquidazioneCi.areaLiquidazionePensioneCi.IsEsenzioneFiscaleEstero;
                CodeUtility.SetValueDdl(ddlEsenzioneFiscale, "NESSUNA ESENZIONE", "NESSUNA ESENZIONE", string.Empty);
                foreach (AreaDecodifica.DatiComunicazioneCampo4 comunicazioneCampo4 in listaComunicazioneC4)
                {
                    if (CodeUtility.LoadRecordEsenzioneFiscaleAGO_CI(comunicazioneCampo4.Id, datiPensione.CodeGruppo, this.domanda.IsDomandaRiapertura, isEsenzioneFiscaleEstero, true))
                        if (!this.domanda.Categoria.StartsWith("S") && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)
                            && comunicazioneCampo4.Id == "1")
                            CodeUtility.SetValueDdl(ddlEsenzioneFiscale, "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", "ESENZIONE FISCALE VITTIME TERRORISMO/DOVERE", comunicazioneCampo4.Id);
                        else
                            CodeUtility.SetValueDdl(ddlEsenzioneFiscale, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Descrizione, comunicazioneCampo4.Id);
                }
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                ddlCodMobilita.Enabled = false;
                txtDecorrenzaBonus.Enabled = false;
                ddlModalitaLiquidazione.Enabled = false;
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);

            txtDecorrenzaArretrati.Text = "MM/AAAA";
            txtScadRevSanitaria.Text = "MM/AAAA";
            txtDataCompletezza.Text = "GG/MM/AAAA";
            txtDecTrattINPDAP.Text = "MM/AAAA";
            txtDataPrenotazione.Text = "GG/MM/AAAA";
            txtDecorrenzaBonus.Text = "MM/AAAA";
            txtDataCalcolo.Text = String.Format("{0:MM/yyyy}", titolare.Pensione.DecorrenzaOriginaria.Value);
        }

        private void ManageTrasformazioneAOI(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI)
            {
                pnlTrasformazioneAOI.Visible = true;
                chkTrasfAOI.Checked = true;
                chkTrasfAOI.Enabled = false;
            }
        }

        private void ManageRichiestaBonus(AreaTitolare.DatiPensione datiPensione, DatiGenerici datiGenerici)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, false) && (datiPensione.CodeProdotto == "0101" || datiPensione.CodeProdotto == "0301" || datiPensione.CodeProdotto == "0401"))
            {
                pnlRichiestaBonus.Visible = true;
                if (datiPensione.CodeTipo == "0167")
                {
                    lblRichiestaBonus.Text = "Bonus 14°:";
                    chkRichiestaBonus.Checked = true;
                    chkRichiestaBonus.Enabled = false;
                    hdnAnnoRichiestaBonus14.Value = datiGenerici.AnnoDecorrenzaBonus;
                }
                else
                {
                    lblRichiestaBonus.Text = "Bonus 154:";
                }
                HiddenAnnoBonusBooking.Value = "SI";

                if (datiPensione.IsRichiestaBonus.HasValue && datiPensione.IsRichiestaBonus.Value)
                {
                    chkRichiestaBonus.Checked = true;
                    if (datiPensione.CodeTipo != "0167" && !string.IsNullOrEmpty(datiGenerici.AnnoDecorrenzaBonus))
                    {
                        txtAnnoBonus.Text = datiGenerici.AnnoDecorrenzaBonus;
                    }
                }
            }
        }

        //private void ManageEsenzioneFiscale(bool isVisible, string gruppo)
        //{
        //    if (gruppo.Equals("0031"))
        //        pnlEsenzioneFiscale.Visible = true;
        //    else
        //        pnlEsenzioneFiscale.Visible = isVisible;
        //}

        private void ManageCheckBenefici(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneCi liquidazione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione ||
                datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (liquidazione.areaLiquidazionePensioneCi != null && (liquidazione.areaLiquidazionePensioneCi.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() ||
                liquidazione.areaLiquidazionePensioneCi.IsBeneficioApePrecociFromFELPE.GetValueOrDefault())) ||
                datiPensione.SceltaLavoratriciMadri.HasValue || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                chkBenefici.Enabled = false;

            if (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.SceltaLavoratriciMadri.HasValue || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
            {
                chkBenefici.Checked = true;
                HiddenBenefici.Value = "true";
            }
            if (datiPensione.IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto)
            {
                chkBenefici.Checked = true;
                HiddenBenefici.Value = "true";
                chkBenefici.Enabled = false;
            }
            if (liquidazione.areaLiquidazionePensioneCi != null && ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                liquidazione.areaLiquidazionePensioneCi.IsBeneficioNonVedente.GetValueOrDefault()) ||
                liquidazione.areaLiquidazionePensioneCi.IsBeneficioNonVedenteFromStorico.GetValueOrDefault()))
            {
                chkBenefici.Checked = true;
                chkBenefici.Enabled = false;
            }
        }

        private void ManageConfermeInvalidita(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneCi liquidazioneCi)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InvaliditaAssegno || Utility.IsDomandaRliquidazioneAssegnoInvalidita(datiPensione))
                pnlConfermeInvalidita.Visible = true;

            //ENG - RIC INVALIDITA
            string categoria = (!String.IsNullOrEmpty(this.domanda.Categoria)) ? this.domanda.Categoria.Trim().ToUpperInvariant() : "";
            if (Utility.IsRicostituzione(datiPensione)
                && (categoria == "IOS" || categoria == "IOCOMS" || categoria == "IOARTS" || categoria == "IRS")
                && liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici != null
                && liquidazioneCi.areaLiquidazionePensioneCi.DatiGenerici.NRiconoscimentiInvalidita.HasValue)
            {
                pnlConfermeInvalidita.Visible = true;
                ddlConfermeInvalidita.Enabled = false;
            }
        }

        private void ManageForPensioniVecchiaiaCalcoloContrib(ILiquidazionePensioneCi liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            //FG - Controlli tipo contributivo
            if (liquidazione.areaLiquidazionePensioneCi.IsPensioneTipoContributivo.GetValueOrDefault()
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione
                || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo
                || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))) //ENG - MEMO 166/2023
            {
                var itemCodiceMobilita = ddlCodMobilita.Items.FindByText("");
                if (itemCodiceMobilita != null)
                {
                    ddlCodMobilita.SelectedValue = itemCodiceMobilita.Value;
                    ddlCodMobilita.Enabled = false;
                }
            }
        }

        private void ManageModalitaLiquidazione(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                pnlModalitaLiquidazione.Visible = false;
                pnlProvvisoria.Visible = false;
            }
            else
            {
                Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                {
                    pnlModalitaLiquidazione.Visible = true;
                    pnlProvvisoria.Visible = false;
                }
            }
        }

        protected void VerificaAdesioneFondoCredito()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.VerificaAdesioneFondoCredito(titolare.CodiceFiscale, this);

            if (this.HasError)
            {
                ddlTrattINPDAP.SelectedValue = "NO";
                txtDecTrattINPDAP.Text = "MM/AAAA";
                txtDecTrattINPDAP.Enabled = false;
                ddlTrattINPDAP.Enabled = false;
            }
            else
            {
                ddlTrattINPDAP.SelectedValue = "SI";
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = true;
            }
        }

        //ENG - Aggiornamento Memo86
        protected void VerificaAdesioneFondoCreditoAggiornamentoMemo86(bool? isPresenteTrattenutaFondoCreditoDaPrelievo, bool? IsDataRinunciaTrattenutaInpdapStorico)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.VerificaAdesioneFondoCredito(titolare.CodiceFiscale, this);
            bool isPresenteAdesioneFondoCredito = (this.HasError) ? false : true;

            //casistica blank, casistica discordante, casistica concordante NO
            if (!isPresenteTrattenutaFondoCreditoDaPrelievo.HasValue ||
                isPresenteTrattenutaFondoCreditoDaPrelievo.Value != isPresenteAdesioneFondoCredito ||
                (!isPresenteTrattenutaFondoCreditoDaPrelievo.Value && !isPresenteAdesioneFondoCredito))
            {
                if (!isPresenteAdesioneFondoCredito)
                {
                    ddlTrattINPDAP.SelectedValue = "NO";
                    txtDecTrattINPDAP.Text = "MM/AAAA";
                    txtDecTrattINPDAP.Enabled = false;
                    ddlTrattINPDAP.Enabled = false;
                }
                else
                {
                    ddlTrattINPDAP.SelectedValue = "SI";
                    ddlTrattINPDAP.Enabled = false;
                    txtDecTrattINPDAP.Enabled = true;
                }

                if (isPresenteTrattenutaFondoCreditoDaPrelievo.HasValue && isPresenteTrattenutaFondoCreditoDaPrelievo.Value != isPresenteAdesioneFondoCredito)
                    RaiseShowAvvisoTrattenutaFondoCredito(this, null);
            }
            else if (isPresenteTrattenutaFondoCreditoDaPrelievo.Value && isPresenteAdesioneFondoCredito) //casistica concordante SI
            {
                ddlTrattINPDAP.Enabled = false;
                txtDecTrattINPDAP.Enabled = false;
            }
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
        //ENG - Aggiornamento Memo86
        public event EventHandler ShowAvvisoTrattenutaFondoCredito;

    }
}
