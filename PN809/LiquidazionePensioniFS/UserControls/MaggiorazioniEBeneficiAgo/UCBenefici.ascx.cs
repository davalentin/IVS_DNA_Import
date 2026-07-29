using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo
{
    public partial class UCBenefici : CustomBaseUserControl, IMaggiorazioneBeneficiAgo, ITitolarePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaBenefici_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiBenefici = GetValoriBenefici();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaBeneficiAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaBenefici_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaBeneficiAgo(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Benefici";
            else
            {
                ClearForm();
                ValorizzaEtichetteBenefici(this);
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


        private void RenderControls(IMaggiorazioneBeneficiAgo maggiorazioneBenefici, AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione != CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione && !this.domanda.IsDomandaRiapertura)
                pnlOneriTerrorismo.Visible = false;

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Inabilita_Invalidita)
                pnlSentenze.Visible = false;

            if (this.domanda.IsDomandaENPALS)
            {
                pnlENPALS.Visible = true;
                pnlSettimaneConIncremento.Visible = false;
            }

            if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioBloccato.HasValue && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioBloccato.Value)
                ddlTipoSettimaneBeneficio.Enabled = false;

            if (Utility.IsDomandaAUT(this.domanda.Categoria) || CodeUtility.IsRicostituzione(datiPensione) || Utility.IsDomandaSPED(this.domanda.Categoria))
            {
                txtSettimane1Percento.Enabled = false;
                txtSettimane05Percento.Enabled = false;
                if (Utility.IsDomandaSPED(this.domanda.Categoria))
                {
                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                }
            }

            if (Utility.IsDomandaIndiretta(datiPensione) && (Utility.IsDomandaSOTOT(this.domanda.Categoria) || Utility.IsDomandaSOCUM(this.domanda.Categoria)))
            {
                txtSentenza495240.Text = string.Empty;
                txtSentenza495240.Enabled = false;
                txtSettimane1Percento.Text = string.Empty;
                txtSettimane1Percento.Enabled = false;
                txtSettimane05Percento.Text = string.Empty;
                txtSettimane05Percento.Enabled = false;
            }

            if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                txtSettAnzContPost311295.Enabled = false;

            // pannelli terrorismo e sentenze non visibili (risulta visibile solo tipo beneficio e settimane beneficio)
            if (Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) ||
                Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) || Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria) ||
                Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria))
            {
                pnlOneriTerrorismo.Visible = false;
                pnlSentenze.Visible = false;
            }
            if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                pnlOneriTerrorismo.Visible = false;

            if (this.domanda.IsDomandaINPDAP)
            {
                lblNumeroSettimaneBeneficio.Text = "Numero giorni beneficio:";
                txtNumeroSettimaneBeneficio.MaxLength = 5;
            }

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                if (!CodeUtility.IsRicostituzione(datiPensione))
                {
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    txtSettAnzContPost311295.Enabled = false;
                }

                if (datiPensione.IsDomandaAPEPrecociOrRicostituzione || Utility.IsDomandaTotalizzazione(this.domanda.Categoria) || Utility.IsDomandaCumulo(this.domanda.Categoria))
                {
                    txtOneriTerrorismoUno.Enabled = false;
                    txtOneriTerrorismoDue.Enabled = false;
                    txtOneriTerrorismoTre.Enabled = false;
                }

                if ((areaMaggiorazioneBenefici != null && areaMaggiorazioneBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault()) ||
                    datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione ||
                    datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                    ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                    (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                    datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                    btnEliminaBenefici.Enabled = false;
            }


            string CodFase = string.Empty;
            string Domanda = domanda.NumeroDomanda;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Presenter.SvrLiquidazione.AreaEsito esito = objWS.GetCodFaseByNDomus(out CodFase, Domanda);
            string Gruppo = this.TitolarePensione.Pensione.CodeGruppo;

            if (Utility.IsDomandaVecchiaiaENAV(datiPensione, domanda.Categoria) || (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && Utility.IsRiaperturaRicTRF_Benefici16_17(datiPensione, maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio, Gruppo, CodFase))) //AbilitazioneRIC_TRFMemo16_2020
            {
                hdnIsDomandaVecchiaiaENAV.Value = "SI";
            }
            else
            {
                hdnIsDomandaVecchiaiaENAV.Value = "NO";
            }

            if (datiPensione.SceltaLavoratriciMadri.HasValue)
            {
                pnlBenefici.Enabled = false;
            }

            if (Utility.IsDomandaVecchiaiaENAV(datiPensione, domanda.Categoria))
            {
                txtSentenza495240.Enabled = false;
                txtSettimane1Percento.Enabled = false;
                txtSettimane05Percento.Enabled = false;
            }

            if (Utility.IsDomandaPescatori(domanda.Categoria))
            {
                txtSettimane1Percento.Text = string.Empty;
                txtSettimane1Percento.Enabled = false;
                txtSettimane05Percento.Text = string.Empty;
                txtSettimane05Percento.Enabled = false;
            }

            if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                txtSettimane1Percento.Text = string.Empty;
                txtSettimane1Percento.Enabled = false;
                txtSettimane05Percento.Text = string.Empty;
                txtSettimane05Percento.Enabled = false;
                if (this.domanda.Categoria != "IMP")
                {
                    txtSentenza495240.Enabled = false;
                }
            }

            if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                txtSettimane1Percento.Text = string.Empty;
                txtSettimane1Percento.Enabled = false;
                txtSettimane05Percento.Text = string.Empty;
                txtSettimane05Percento.Enabled = false;
                if (this.domanda.Categoria != "IMP")
                {
                    txtSentenza495240.Enabled = false;
                }
            }

            if (Utility.IsDomandaRipristino(datiPensione))
                btnEliminaBenefici.Enabled = false;
        }

        internal Presenter.SvrLiquidazioneAgo.DatiBenefici GetValoriBenefici()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneAgo.DatiBenefici();

            if (string.IsNullOrEmpty(ddlTipoSettimaneBeneficio.SelectedValue))
                this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio = ddlTipoSettimaneBeneficio.SelectedValue;

            if (string.IsNullOrEmpty(txtNumeroSettimaneBeneficio.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = int.Parse(txtNumeroSettimaneBeneficio.Text);

            #region Oneri Terrorismo
            List<Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo> listOneriTerrorismo = new List<Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo>();

            if (!string.IsNullOrEmpty(txtOneriTerrorismoUno.Text))
            {
                Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoUno.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 1;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoDue.Text))
            {
                Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoDue.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 2;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoTre.Text))
            {
                Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneAgo.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoTre.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 3;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo = listOneriTerrorismo.ToArray();
            #endregion Oneri Terrorismo

            if (string.IsNullOrEmpty(txtSettimane1Percento.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento1Percento = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento1Percento = int.Parse(txtSettimane1Percento.Text);

            if (string.IsNullOrEmpty(txtSettimane05Percento.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento05Percento = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento05Percento = int.Parse(txtSettimane05Percento.Text);

            if (string.IsNullOrEmpty(txtSentenza495240.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.Sentenza495240 = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.Sentenza495240 = byte.Parse(txtSentenza495240.Text);

            if (new List<string> { "01", "16", "17", "04" }.Contains(this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio) &&
                !string.IsNullOrEmpty(txtSettAnzContPost311295.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295 = CodeUtility.StringToNullableShort(txtSettAnzContPost311295.Text);

            if (string.IsNullOrEmpty(txtDataNonVedenteDal.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtDataNonVedenteDal.Text)));

            if (string.IsNullOrEmpty(txtSettIntegrazioneContrConcessa.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettIntegrazioneContributivaConcessa = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettIntegrazioneContributivaConcessa = int.Parse(txtSettIntegrazioneContrConcessa.Text);

            return this.areaMaggiorazioneBenefici.DatiBenefici;
        }

        internal void ValorizzaEtichetteBenefici(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

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

            if (CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) || Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
                hdnIsRicostituzione.Value = "SI";
            else
                hdnIsRicostituzione.Value = "NO";

            if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault())
                hdnIsPrepensionamento_2017.Value = "SI";
            else
                hdnIsPrepensionamento_2017.Value = "NO";

            if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                hdnIsPrepensionamento_2019.Value = "SI";
            else
                hdnIsPrepensionamento_2019.Value = "NO";

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                hdnIsMaggiorazioneAmiantoLegge208_2015.Value = "SI";
            else
                hdnIsMaggiorazioneAmiantoLegge208_2015.Value = "NO";

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente != null &&
                maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.NumeroContributiNLNonVedenti.HasValue)
                ViewState[EnumViewState.NumeroContributiNLNonVedenti.ToString()] = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiExCombattente.NumeroContributiNLNonVedenti.Value;

            LoadDdl(maggiorazioneBenefici);
            RenderControls(maggiorazioneBenefici, TitolarePensione.Pensione);

            ManageTipoBeneficio(TitolarePensione.Pensione, this.domanda, maggiorazioneBenefici);

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null)
            {
                if (!string.IsNullOrEmpty(maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio))
                    ddlTipoSettimaneBeneficio.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.HasValue)
                    txtNumeroSettimaneBeneficio.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.Value.ToString();
                else
                    txtNumeroSettimaneBeneficio.Text = string.Empty;

                #region Oneri Terrorismo
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo != null)
                {
                    for (int i = 0; i < maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo.Count(); i++)
                    {
                        if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 1)
                        {
                            txtOneriTerrorismoUno.Text = Math.Round((decimal)maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                        if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                             maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                             maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 2)
                        {
                            txtOneriTerrorismoDue.Text = Math.Round((decimal)maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                        if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                             maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                             maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 3)
                        {
                            txtOneriTerrorismoTre.Text = Math.Round((decimal)maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                    }
                }
                #endregion Oneri Terrorismo

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento1Percento.HasValue)
                    txtSettimane1Percento.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento1Percento.Value.ToString();
                else
                    txtSettimane1Percento.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento05Percento.HasValue)
                    txtSettimane05Percento.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneIncremento05Percento.Value.ToString();
                else
                    txtSettimane05Percento.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.Sentenza495240.HasValue)
                    txtSentenza495240.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.Sentenza495240.Value.ToString();
                else
                    txtSentenza495240.Text = string.Empty;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295.HasValue)
                    txtSettAnzContPost311295.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295.Value.ToString();

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal.HasValue)
                    txtDataNonVedenteDal.Text = String.Format("{0:dd/MM/yyyy}", maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal.Value);

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettIntegrazioneContributivaConcessa.HasValue)
                    txtSettIntegrazioneContrConcessa.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettIntegrazioneContributivaConcessa.Value.ToString();
                else
                    txtSettIntegrazioneContrConcessa.Text = string.Empty;
            }
            //else
            //{
            //    ddlTipoSettimaneBeneficio.SelectedIndex = 0;
            //    txtNumeroSettimaneBeneficio.Text = string.Empty;
            //    txtOneriTerrorismoUno.Text = string.Empty;
            //    txtOneriTerrorismoDue.Text = string.Empty;
            //    txtOneriTerrorismoTre.Text = string.Empty;
            //    txtSettimane1Percento.Text = string.Empty;
            //    txtSettimane05Percento.Text = string.Empty;
            //    txtSentenza495240.Text = string.Empty;
            //}

            ManageDatiENPALS(maggiorazioneBenefici);

            GestioneEtichetteIsUnicarpe(maggiorazioneBenefici);

            if (Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            {
                pnlBenefici.Enabled = false;
            }

            ManageDatiAssicurativi(maggiorazioneBenefici);

        }

        //Memorizzo in un hidden field il campo numero settimane salvato sul quadro LiquidazionePensione -> Assicurativi
        public void ManageDatiAssicurativi(IMaggiorazioneBeneficiAgo maggiorazioniBenefici)
        {
            if (maggiorazioniBenefici != null && maggiorazioniBenefici.areaMaggiorazioneBenefici != null && maggiorazioniBenefici.areaMaggiorazioneBenefici.Settimane.HasValue &&
                maggiorazioniBenefici.areaMaggiorazioneBenefici.Settimane.Value > 0)
            {
                hdnNumeroSettimaneDatiAssicurativi.Value = maggiorazioniBenefici.areaMaggiorazioneBenefici.Settimane.ToString();
            }
        }

        private void LoadDdl(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                ddlTipoSettimaneBeneficio.Items.Clear();
                CodeUtility.SetItemBlankDdl(ddlTipoSettimaneBeneficio);
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipoBenefici != null)
                    foreach (Presenter.SvrLiquidazioneAgo.TipoBenefici tipoBenefici in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipoBenefici)
                        CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, tipoBenefici.Descrizione, tipoBenefici.Descrizione, tipoBenefici.Id);
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

        private void ManageTipoBeneficio(AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda, IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
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

            if (datiPensione != null)
            {
                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

                if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art1_C250_Legge232)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "13";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }
                else if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "06"; //inabilità
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                string CodFase = string.Empty;
                string Domanda = domanda.NumeroDomanda;
                Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                Presenter.SvrLiquidazione.AreaEsito esito = objWS.GetCodFaseByNDomus(out CodFase, Domanda);
                string Gruppo = this.TitolarePensione.Pensione.CodeGruppo;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && Utility.IsRiaperturaRicTRF_Benefici16_17(datiPensione, maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio, Gruppo, CodFase)) //AbilitazioneRIC_TRFMemo16_2020
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio;
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    txtNumeroSettimaneBeneficio.Enabled = false;
                    txtSettAnzContPost311295.Enabled = false;
                }

                if (datiPensione.IsDomandaQuota100OrRicostituzione)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "14";
                    ddlTipoSettimaneBeneficio.Enabled = false;

                    txtNumeroSettimaneBeneficio.Enabled = false;

                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                    txtSettimane1Percento.Text = string.Empty;
                    txtSettimane1Percento.Enabled = false;
                    txtSettimane05Percento.Text = string.Empty;
                    txtSettimane05Percento.Enabled = false;
                }

                if (datiPensione.IsDomandaQuota102OrRicostituzione)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "18";
                    ddlTipoSettimaneBeneficio.Enabled = false;

                    txtNumeroSettimaneBeneficio.Enabled = false;

                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                    txtSettimane1Percento.Text = string.Empty;
                    txtSettimane1Percento.Enabled = false;
                    txtSettimane05Percento.Text = string.Empty;
                    txtSettimane05Percento.Enabled = false;
                }

                if (datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "19";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    txtNumeroSettimaneBeneficio.Enabled = false;
                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                    txtSettimane1Percento.Text = string.Empty;
                    txtSettimane1Percento.Enabled = false;
                    txtSettimane05Percento.Text = string.Empty;
                    txtSettimane05Percento.Enabled = false;
                }

                //ENG - Memo 123/2024
                if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                   (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                   (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                    datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "24";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                        txtNumeroSettimaneBeneficio.Enabled = false;
                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                    txtSettimane1Percento.Text = string.Empty;
                    txtSettimane1Percento.Enabled = false;
                    txtSettimane05Percento.Text = string.Empty;
                    txtSettimane05Percento.Enabled = false;
                }

                if (datiPensione.SceltaLavoratriciMadri.HasValue)
                {
                    switch (datiPensione.SceltaLavoratriciMadri.Value)
                    {
                        case 1:
                            ddlTipoSettimaneBeneficio.SelectedValue = "12";
                            break;
                        case 2:
                            ddlTipoSettimaneBeneficio.SelectedValue = "15";
                            break;
                    }
                }
            }

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                if (domanda != null && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) &&
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioInabilitaByPrimoCodiceNatura.GetValueOrDefault())
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "06"; //inabilità
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }
                if (domanda != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioNonVedenteByPrimoCodiceNatura.GetValueOrDefault())
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "01"; //non vedente
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioAmianto181.GetValueOrDefault() || maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "04";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                    {
                        txtSentenza495240.Text = string.Empty;
                        txtSentenza495240.Enabled = false;
                        txtSettimane1Percento.Text = string.Empty;
                        txtSettimane1Percento.Enabled = false;
                        txtSettimane05Percento.Text = string.Empty;
                        txtSettimane05Percento.Enabled = false;
                    }
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioExArt80.HasValue && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioExArt80.Value)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "08";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMinatori.HasValue && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMinatori.Value)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "07";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioUsuranti.HasValue && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioUsuranti.Value)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "03";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsDomandaInabilitaIndiretta.HasValue && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsDomandaInabilitaIndiretta.Value)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "06";//inabilità
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoria.GetValueOrDefault() ||
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault() ||
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault() ||
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "02";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaArt1c500L160_2019.GetValueOrDefault())
                    {
                        txtNumeroSettimaneBeneficio.Text = string.Empty;
                        txtSentenza495240.Enabled = false;
                        txtSettimane1Percento.Enabled = false;
                        txtSettimane05Percento.Enabled = false;

                        if (CodeUtility.IsRicostituzione(datiPensione))
                        {
                            txtOneriTerrorismoUno.Enabled = false;
                            txtOneriTerrorismoDue.Enabled = false;
                            txtOneriTerrorismoTre.Enabled = false;
                        }
                    }
                    else if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaArt1c154L205_2017.GetValueOrDefault())
                    {
                        txtSettimane1Percento.Enabled = false;
                        txtSettimane05Percento.Enabled = false;
                    }
                    else if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoria.GetValueOrDefault() || maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
                    {
                        txtNumeroSettimaneBeneficio.Enabled = false;
                        txtSentenza495240.Enabled = false;
                        txtSettimane1Percento.Enabled = false;
                        txtSettimane05Percento.Enabled = false;
                        if (CodeUtility.IsRicostituzione(datiPensione))
                        {
                            pnlOneriTerrorismo.Visible = false;
                        }

                        if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsPrepensionamentoEditoriaFiltroEBA.GetValueOrDefault())
                        {
                            pnlSettIntegrazioneContrConcess.Visible = true;
                            txtSettIntegrazioneContrConcessa.Enabled = false;
                        }
                    }
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault())
                    ddlTipoSettimaneBeneficio.Enabled = false;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioApePrecociFromFELPE.GetValueOrDefault())
                {
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    //FG - Evolutive su domande ai precoci al 09/01/2017
                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                    txtSettimane1Percento.Text = string.Empty;
                    txtSettimane1Percento.Enabled = false;
                    txtSettimane05Percento.Text = string.Empty;
                    txtSettimane05Percento.Enabled = false;
                }
            }
            if (domanda != null)
            {
                if (Utility.IsDomandaVESO33(domanda.Categoria) || Utility.IsDomandaVESO92(domanda.Categoria) || Utility.IsDomandaVOCRED_CRED27(domanda.Categoria) ||
                    Utility.IsDomandaVOCOOP_COOP28(domanda.Categoria) || Utility.IsDomandaVOESO(domanda.Categoria) || Utility.IsDomandaVESO29(domanda.Categoria) ||
                    Utility.IsDomandaESOTEL(domanda.Categoria) || Utility.IsDomandaESOAMB(domanda.Categoria) || Utility.IsDomandaESPA(domanda.Categoria))
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "05"; // ESODATI
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (Utility.IsDomandaVESO92(domanda.Categoria) ||
                        Utility.IsDomandaESPA(domanda.Categoria) || Utility.IsDomandaCRED27(domanda.Categoria) || Utility.IsDomandaVESO33(domanda.Categoria)))
                    {
                        btnEliminaBenefici.Enabled = false;
                    }
                }

                if (Utility.IsDomandaAUT(this.domanda.Categoria) && (ddlTipoSettimaneBeneficio.Enabled && pnlBenefici.Enabled)) //se non è abilitato, cade in uno degli altri casi
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "01"; // lavoratore non vedente
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (Utility.IsDomandaSPED(this.domanda.Categoria) && ddlTipoSettimaneBeneficio.SelectedValue != "06")
                {
                    ddlTipoSettimaneBeneficio.Items.Remove(ddlTipoSettimaneBeneficio.Items.FindByValue("06")); // inabilità
                }

                if (Utility.IsDomandaVOMIN(this.domanda.Categoria) && ddlTipoSettimaneBeneficio.SelectedValue == "")
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "01"; // lavoratore non vedente
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                {
                    ddlTipoSettimaneBeneficio.Enabled = false;

                    if (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim() == "IMP")
                        txtNumeroSettimaneBeneficio.Enabled = false;

                    if (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim() == "IMP" && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1984, 08, 01)))
                        ddlTipoSettimaneBeneficio.SelectedValue = "06"; // inabilità
                }

                if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
                {
                    ddlTipoSettimaneBeneficio.Enabled = false;

                    if (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim() == "IMP")
                        txtNumeroSettimaneBeneficio.Enabled = false;

                    if (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim() == "IMP" && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.GetValueOrDefault(), new DateTime(1984, 08, 01)))
                        ddlTipoSettimaneBeneficio.SelectedValue = "06"; // inabilità
                }

                //ENG - memo 28/2024 - 0001-0001-0017 e 0001-0001-0045 (filtro PAV)
                //con decorrenza pensione a partire dal 2024 e calcolo contributivo dobbiamo inviare al calcolo GP1AV61=23 (modifica sospesa)
                if (!String.IsNullOrEmpty(controlloDinamico28_2024) && controlloDinamico28_2024.Trim().ToUpperInvariant() == "SI")
                {
                    if (((datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0017") ||
                        (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0001" && datiPensione.CodeTipo == "0045" && datiPensione.CodiceTipoRichiesta == "AV")) &&
                        (datiPensione.TipoCalcolo == UtilityTipoCalcolo.Contributivo || (!String.IsNullOrEmpty(datiPensione.Tipologia) &&
                        (datiPensione.Tipologia.ToUpperInvariant() == "SISTEMA CALCOLO CONTRIBUTIVO" || datiPensione.Tipologia.ToUpperInvariant() == "COMPUTO - ART. 3 D.M. 282/1996"))) &&
                        datiPensione.DecorrenzaOriginaria.HasValue)
                    {
                        ddlTipoSettimaneBeneficio.Enabled = true;
                    }
                }

                if (Utility.IsDomandaVOCUM(domanda.Categoria) && domanda.IsDomandaRiapertura && datiPensione.IsDomandaAPEPrecociOrRicostituzione)
                    txtNumeroSettimaneBeneficio.Enabled = false;
            }
            if (Utility.IsPensioneInabilitaProficuoLavoroCumulo(this.domanda.Categoria, datiPensione))
            {
                hdnIsPensioneProficuoCumulo.Value = "SI";
                txtNumeroSettimaneBeneficio.Enabled = false;
            }
            else
            {
                hdnIsPensioneProficuoCumulo.Value = "NO";

            }
        }

        private void ManageDatiENPALS(IMaggiorazioneBeneficiAgo maggiorazioniBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaENPALS)
            {
                if (maggiorazioniBenefici != null && maggiorazioniBenefici.areaMaggiorazioneBenefici != null)
                {
                    if (maggiorazioniBenefici.areaMaggiorazioneBenefici.DatiBenefici != null)
                    {
                        if (maggiorazioniBenefici.areaMaggiorazioneBenefici.DatiBenefici.IndicatoreInvalidita80.HasValue)
                        {
                            if (maggiorazioniBenefici.areaMaggiorazioneBenefici.DatiBenefici.IndicatoreInvalidita80.Value == '0')
                                ddlIndicatoreInvalidita80.SelectedValue = "NO";
                            else if (maggiorazioniBenefici.areaMaggiorazioneBenefici.DatiBenefici.IndicatoreInvalidita80.Value == '1')
                                ddlIndicatoreInvalidita80.SelectedValue = "SI";
                        }
                        else
                            ddlIndicatoreInvalidita80.SelectedValue = "";
                    }

                    if (ViewState[EnumViewState.NumeroContributiNLNonVedenti.ToString()] != null)
                    {
                        ddlTipoSettimaneBeneficio.Enabled = false;
                        txtNumeroSettimaneBeneficio.Enabled = false;
                    }

                    if (maggiorazioniBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && maggiorazioniBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio == "01")
                        txtSettAnzContPost311295.Enabled = true;
                }
            }
        }

        private void GestioneEtichetteIsUnicarpe(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            if (TitolarePensione.Pensione != null && Utility.IsDomandaUnicarpe(TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
                {
                    txtNumeroSettimaneBeneficio.Enabled = maggiorazioneBenefici.areaMaggiorazioneBenefici.IsNumSettimaneBeneficioEnabled.GetValueOrDefault();

                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio == "01")
                    {
                        pnlDataNonVedenteDal.Visible = true;
                        txtNumeroSettimaneBeneficio.Enabled = false;
                        if (!this.domanda.IsDomandaENPALS)
                            txtSettAnzContPost311295.Enabled = false;
                        ddlTipoSettimaneBeneficio.Enabled = false;
                    }

                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                    {
                        txtSettAnzContPost311295.Enabled = false;
                    }
                }
            }
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IMaggiorazioneBenefici

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare




        public enum EnumViewState
        {
            NumeroContributiNLNonVedenti
        }




    }
}