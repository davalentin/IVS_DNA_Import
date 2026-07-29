using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi
{
    public partial class UCBenefici : CustomBaseUserControl, IMaggiorazioneBeneficiCi, ITitolarePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SalvaBenefici_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiBenefici = GetValoriBenefici();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaBeneficiCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaBenefici_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaBeneficiCi(this);

            if (!this.HasError)
            {
                ValorizzaEtichetteBenefici(null);
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

        internal Presenter.SvrLiquidazioneCi.DatiBenefici GetValoriBenefici()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneCi.DatiBenefici();

            if (string.IsNullOrEmpty(ddlTipoSettimaneBeneficio.SelectedValue))
                this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio = ddlTipoSettimaneBeneficio.SelectedValue;

            if (string.IsNullOrEmpty(txtNumeroSettimaneBeneficio.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = int.Parse(txtNumeroSettimaneBeneficio.Text);

            List<Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo> listOneriTerrorismo = new List<Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo>();

            if (!string.IsNullOrEmpty(txtOneriTerrorismoUno.Text))
            {
                Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoUno.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 1;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoDue.Text))
            {
                Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoDue.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 2;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoTre.Text))
            {
                Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneCi.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoTre.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 3;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo = listOneriTerrorismo.ToArray();

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

            if ((this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio == "01" || this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio == "04") &&
                !string.IsNullOrEmpty(txtSettAnzContPost311295.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295 = CodeUtility.StringToNullableShort(txtSettAnzContPost311295.Text);

            if (string.IsNullOrEmpty(txtDataNonVedenteDal.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtDataNonVedenteDal.Text)));

            return this.areaMaggiorazioneBenefici.DatiBenefici;
        }

        internal void ValorizzaEtichetteBenefici(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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

            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);

            if (CodeUtility.IsRicostituzioneOrRiapertura(titolare.Pensione, this.domanda.IsDomandaRiapertura))
                hdnIsRicostituzione.Value = "SI";
            else
                hdnIsRicostituzione.Value = "NO";

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                hdnIsMaggiorazioneAmiantoLegge208_2015.Value = "SI";
            else
                hdnIsMaggiorazioneAmiantoLegge208_2015.Value = "NO";

            LoadDdl(maggiorazioneBenefici);
            RenderControls(maggiorazioneBenefici != null ? maggiorazioneBenefici.areaMaggiorazioneBenefici : null);

            if (titolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura)
                pnlOneriTerrorismo.Visible = true;

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null)
            {
                if (!string.IsNullOrEmpty(maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio))
                    ddlTipoSettimaneBeneficio.SelectedValue = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio;
                else
                    ddlTipoSettimaneBeneficio.SelectedIndex = 0;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.HasValue)
                    txtNumeroSettimaneBeneficio.Text = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.Value.ToString();
                else
                    txtNumeroSettimaneBeneficio.Text = string.Empty;

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

            ManageTipoBeneficio(maggiorazioneBenefici);
            ManageDatiAssicurativi(maggiorazioneBenefici);
            GestioneEtichetteIsUnicarpe(maggiorazioneBenefici);

        }

        private void LoadDdl(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipoBenefici != null)
                    foreach (Presenter.SvrLiquidazioneCi.TipoBenefici tipoBenefici in maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaTipoBenefici)
                        CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, tipoBenefici.Descrizione, tipoBenefici.Descrizione, tipoBenefici.Id);
            }
        }

        private void RenderControls(Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                ddlTipoSettimaneBeneficio.Enabled = false;
                if (areaMaggiorazioneBenefici == null || !areaMaggiorazioneBenefici.IsDomandaPensioneInabilita.GetValueOrDefault() ||
                    datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione)
                    txtNumeroSettimaneBeneficio.Enabled = false;
                txtSettAnzContPost311295.Enabled = false;

                if ((areaMaggiorazioneBenefici != null && areaMaggiorazioneBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault()) ||
                    datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione ||
                    datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                    ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                    (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))
                    btnEliminaBenefici.Enabled = false;
            }

            if (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
               (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
               (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                txtNumeroSettimaneBeneficio.Enabled = false;

            if (datiPensione.SceltaLavoratriciMadri.HasValue)
            {
                pnlBenefici.Enabled = false;
                pnlOneriTerrorismo.Enabled = false;
                pnlSentenze.Enabled = false;
            }
        }

        private void ManageTipoBeneficio(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione)
            {
                ddlTipoSettimaneBeneficio.SelectedValue = "13";
                ddlTipoSettimaneBeneficio.Enabled = false;
            }

            if (datiPensione.IsDomandaQuota100OrRicostituzione)
            {
                ddlTipoSettimaneBeneficio.SelectedValue = "14";
                ddlTipoSettimaneBeneficio.Enabled = false;
            }

            if (datiPensione.IsDomandaQuota102OrRicostituzione)
            {
                ddlTipoSettimaneBeneficio.SelectedValue = "18";
                ddlTipoSettimaneBeneficio.Enabled = false;
            }

            if (datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)
            {
                ddlTipoSettimaneBeneficio.SelectedValue = "19";
                ddlTipoSettimaneBeneficio.Enabled = false;
            }

            if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
               (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
               (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
            {
                ddlTipoSettimaneBeneficio.SelectedValue = "24";
                ddlTipoSettimaneBeneficio.Enabled = false;
            }

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioExArt80.HasValue && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioExArt80.Value)
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "08";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() ||
                    maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioApePrecociFromFELPE.GetValueOrDefault())
                    ddlTipoSettimaneBeneficio.Enabled = false;

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                {
                    ddlTipoSettimaneBeneficio.SelectedValue = "04";
                    ddlTipoSettimaneBeneficio.Enabled = false;
                    txtSentenza495240.Text = string.Empty;
                    txtSentenza495240.Enabled = false;
                    txtSettimane1Percento.Text = string.Empty;
                    txtSettimane1Percento.Enabled = false;
                    txtSettimane05Percento.Text = string.Empty;
                    txtSettimane05Percento.Enabled = false;
                }
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

        private void GestioneEtichetteIsUnicarpe(IMaggiorazioneBeneficiCi maggiorazioneBenefici)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null)
                {
                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio == "01")
                    {
                        pnlDataNonVedenteDal.Visible = true;
                        txtNumeroSettimaneBeneficio.Enabled = false;
                        txtSettAnzContPost311295.Enabled = false;
                        ddlTipoSettimaneBeneficio.Enabled = false;
                    }

                    if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.IsBeneficioMaggiorazioneAmiantoLegge208_2015.GetValueOrDefault())
                    {
                        txtSettAnzContPost311295.Enabled = false;
                        txtNumeroSettimaneBeneficio.Enabled = false;
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
        public Presenter.SvrLiquidazioneCi.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion IMaggiorazioneBenefici

        #region ITitolarePensione Members

        public AreaTitolare TitolarePensione { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion ITitolarePensione Members

        //Memorizzo in un hidden field il campo numero settimane salvato sul quadro LiquidazionePensione -> Assicurativi
        public void ManageDatiAssicurativi(IMaggiorazioneBeneficiCi maggiorazioniBenefici)
        {

            if (maggiorazioniBenefici != null && maggiorazioniBenefici.areaMaggiorazioneBenefici != null && maggiorazioniBenefici.areaMaggiorazioneBenefici.Settimane.HasValue &&
                maggiorazioniBenefici.areaMaggiorazioneBenefici.Settimane.Value > 0)
            {
                hdnNumeroSettimaneDatiAssicurativi.Value = maggiorazioniBenefici.areaMaggiorazioneBenefici.Settimane.ToString();
            }
        }


    }
}