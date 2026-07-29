using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici
{
    public partial class UCBenefici : CustomBaseUserControl, IMaggiorazioneBenefici
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaBenefici_Click(Object sender, EventArgs e)
        {
            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
            this.areaMaggiorazioneBenefici.DatiBenefici = GetValoriBenefici();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaBenefici(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaBenefici_Click(Object sender, EventArgs e)
        {

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaBenefici(this);

            if (!this.HasError)
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

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {

        }

        internal Presenter.SvrLiquidazioneFs.DatiBenefici GetValoriBenefici()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiBenefici == null)
                this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneFs.DatiBenefici();

            GetDatiBeneficiCommon();
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    GetDatiBeneficiEL_ET_TT_VL_GAS_DZ_ES_PM();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    GetDatiBeneficiFS_PT_INPDAP();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    GetDatiBeneficiCL();
                    break;

            }
            if (this.domanda.IsDomandaINPDAP)
            {
                GetDatiBeneficiFS_PT_INPDAP();
            }
            return areaMaggiorazioneBenefici.DatiBenefici;
        }

        private void GetDatiBeneficiCommon()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiBenefici == null)
                this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneFs.DatiBenefici();

            if (string.IsNullOrEmpty(ddlTipoSettimaneBeneficio.SelectedValue))
                this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio = ddlTipoSettimaneBeneficio.SelectedValue;

            if (this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio == "01" && !string.IsNullOrEmpty(txtSettAnzContPost311295.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295 = CodeUtility.StringToNullableShort(txtSettAnzContPost311295.Text);

            if (string.IsNullOrEmpty(txtDataNonVedenteDal.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(txtDataNonVedenteDal.Text)));
        }

        private void GetDatiBeneficiEL_ET_TT_VL_GAS_DZ_ES_PM()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiBenefici == null)
                this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneFs.DatiBenefici();

            if (string.IsNullOrEmpty(txtNumeroSettimaneBeneficio.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = int.Parse(txtNumeroSettimaneBeneficio.Text);

            if (string.IsNullOrEmpty(txtDecorrenza.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale = Utility.GetDateFromString(txtDecorrenza.Text);

            if (string.IsNullOrEmpty(txtCessazione.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale = Utility.GetDateFromString(txtCessazione.Text);

            List<Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo> listOneriTerrorismo = new List<Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo>();

            if (!string.IsNullOrEmpty(txtOneriTerrorismoUno.Text))
            {
                Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoUno.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 1;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoDue.Text))
            {
                Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoDue.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 2;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoTre.Text))
            {
                Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoTre.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 3;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo = listOneriTerrorismo.ToArray();
        }

        private void GetDatiBeneficiFS_PT_INPDAP()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiBenefici == null)
                this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneFs.DatiBenefici();

            if (string.IsNullOrEmpty(txtNumeroSettimaneBeneficio.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio = int.Parse(txtNumeroSettimaneBeneficio.Text);

            this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioAA = !String.IsNullOrEmpty(txtAABeneficioTemporale.Text) ? Convert.ToInt16(txtAABeneficioTemporale.Text) : (Int16?)null;
            this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioMM = !String.IsNullOrEmpty(txtMMBeneficioTemporale.Text) ? Convert.ToInt16(txtMMBeneficioTemporale.Text) : (Int16?)null;
            this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioGG = !String.IsNullOrEmpty(txtGGBeneficioTemporale.Text) ? Convert.ToInt16(txtGGBeneficioTemporale.Text) : (Int16?)null;
            if (string.IsNullOrEmpty(txtDecorrenza.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale = Utility.GetDateFromString(txtDecorrenza.Text);

            if (string.IsNullOrEmpty(txtCessazione.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale = Utility.GetDateFromString(txtCessazione.Text);
        }

        private void GetDatiBeneficiCL()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiBenefici == null)
                this.areaMaggiorazioneBenefici.DatiBenefici = new Presenter.SvrLiquidazioneFs.DatiBenefici();

            if (string.IsNullOrEmpty(txtDecorrenza.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale = Utility.GetDateFromString(txtDecorrenza.Text);

            if (string.IsNullOrEmpty(txtCessazione.Text))
                this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale = null;
            else
                this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale = Utility.GetDateFromString(txtCessazione.Text);

            List<Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo> listOneriTerrorismo = new List<Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo>();

            if (!string.IsNullOrEmpty(txtOneriTerrorismoUno.Text))
            {
                Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoUno.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 1;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoDue.Text))
            {
                Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoDue.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 2;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            if (!string.IsNullOrEmpty(txtOneriTerrorismoTre.Text))
            {
                Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo oneriTerrorismo = new Presenter.SvrLiquidazioneFs.DatiBenefici.OneriTerrorismo();
                oneriTerrorismo.Importo = decimal.Parse(txtOneriTerrorismoTre.Text);
                oneriTerrorismo.CodiceAltroFondo = 548;
                oneriTerrorismo.Progressivo = 3;
                listOneriTerrorismo.Add(oneriTerrorismo);
            }

            areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo = listOneriTerrorismo.ToArray();
        }

        private void RenderControls(Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if ((areaMaggiorazioneBenefici != null &&
                (areaMaggiorazioneBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault() || areaMaggiorazioneBenefici.IsBeneficioApePrecociFromFELPE.GetValueOrDefault())) ||
                datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione)
                ddlTipoSettimaneBeneficio.Enabled = false;

            // TODO TIPO BENEFICIO BLOCCATO: Rimuovere nel caso in cui venga definita la logica di blocco per le ricostituzioni
            //if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            //{
            //    ddlTipoSettimaneBeneficio.Enabled = false;
            //    if (areaMaggiorazioneBenefici == null || 
            //        !areaMaggiorazioneBenefici.IsDomandaPensioneInabilita.GetValueOrDefault())
            //        txtNumeroSettimaneBeneficio.Enabled = false;
            //    txtSettAnzContPost311295.Enabled = false;
            //}

            if (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione ||
                datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || datiPensione.IsDomandaAPEPrecociOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
            {
                txtNumeroSettimaneBeneficio.Enabled = false;
                ddlTipoSettimaneBeneficio.Enabled = false;
            }

            //ENG - Memo 123/2024
            if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
                ddlTipoSettimaneBeneficio.Enabled = false;

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (
                (areaMaggiorazioneBenefici != null && areaMaggiorazioneBenefici.IsBeneficioArt24Comma15BisFromFELPE.GetValueOrDefault()) ||
                datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || 
                ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                btnEliminaBenefici.Enabled = false;

            if (datiPensione.SceltaLavoratriciMadri.HasValue)
            {
                pnlBeneficiCommon.Enabled = false;
                pnlMaggiorazioneSociale.Enabled = false;
                pnlOneriTerrorismo.Enabled = false;
            }

        }

        private void RenderControlsFromTipoFondo()
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        this.pnlMaggiorazioneSociale.Visible = true;
                        this.pnlOneriTerrorismo.Visible = true;
                        this.pnlSettBeneficio.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        this.pnlBenefTemporale.Visible = true;
                        txtAABeneficioTemporale.Enabled = false;
                        txtMMBeneficioTemporale.Enabled = false;
                        txtGGBeneficioTemporale.Enabled = false;
                        this.pnlSettBeneficio.Visible = true;
                        if ((this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.IsMaggiorazioniForMemo72.GetValueOrDefault()) ||
                            (CodeUtility.IsRicostituzioneContributiva(datiPensione) || (CodeUtility.IsRicostituzione(datiPensione) && datiPensione.CodeProdotto == "0101" && datiPensione.CodeTipo == "0101")))
                            this.pnlMaggiorazioneSociale.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                        this.pnlOneriTerrorismo.Visible = true;
                        this.pnlMaggiorazioneSociale.Visible = true;
                        break;
                }
            }
            else if (this.domanda.IsDomandaINPDAP)
            {
                this.pnlBenefTemporale.Visible = true;
                txtAABeneficioTemporale.Enabled = false;
                txtMMBeneficioTemporale.Enabled = false;
                txtGGBeneficioTemporale.Enabled = false;
                this.pnlSettBeneficio.Visible = true;

                if (CodeUtility.IsRicostituzioneContributiva(datiPensione) || (CodeUtility.IsRicostituzione(datiPensione) && datiPensione.CodeProdotto == "0101" && datiPensione.CodeTipo == "0101"))
                    this.pnlMaggiorazioneSociale.Visible = true;
            }
        }

        internal void ValorizzaEtichetteBenefici(IMaggiorazioneBenefici maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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

            this.areaMaggiorazioneBenefici = maggiorazioneBenefici.areaMaggiorazioneBenefici;
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                hdnIsRicostituzione.Value = "SI";
            else
                hdnIsRicostituzione.Value = "NO";

            LoadDdl();
            LoadDdlFromTipoFondo();
            RenderControls(this.areaMaggiorazioneBenefici);
            RenderControlsFromTipoFondo();
            ValorizzaEtichetteCommon();

            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        ValorizzaEtichetteEL_ET_TT_VL_GAS_DZ_ES_PM();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        ValorizzaEtichetteFS_PT_INPDAP();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                        ValorizzaEtichetteCL();
                        break;
                }
            }

            if (this.domanda.IsDomandaINPDAP)
            {
                ValorizzaEtichetteFS_PT_INPDAP();
            }

            GestioneEtichetteIsUnicarpe(maggiorazioneBenefici);
            GestioneEtichetteRic(datiPensione);
        }

        private void LoadDdl()
        {

        }

        private void LoadDdlFromTipoFondo()
        {
            ddlTipoSettimaneBeneficio.Items.Clear();
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, string.Empty, string.Empty, "");
                    if (this.areaMaggiorazioneBenefici != null)
                    {
                        if (this.areaMaggiorazioneBenefici.ListaTipoBenefici != null)
                            foreach (Presenter.SvrLiquidazioneFs.TipoBenefici tipoBenefici in this.areaMaggiorazioneBenefici.ListaTipoBenefici)
                            {
                                if (tipoBenefici.Id == "01" || tipoBenefici.Id == "19" || tipoBenefici.Id == "24")
                                    CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, tipoBenefici.Descrizione, tipoBenefici.Descrizione, tipoBenefici.Id);
                            }
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    ddlTipoSettimaneBeneficio.Items.Clear();
                    CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, string.Empty, string.Empty, "");
                    if (this.areaMaggiorazioneBenefici != null)
                    {
                        if (this.areaMaggiorazioneBenefici.ListaTipoBenefici != null)
                            foreach (Presenter.SvrLiquidazioneFs.TipoBenefici tipoBenefici in this.areaMaggiorazioneBenefici.ListaTipoBenefici)
                            {
                                //Applicata tale gestione solo per il fondo TT e per il tipoBeneficio Amianto, in quanto ha lo stesso codice degli Ex Iritel (02)
                                if (tipoBenefici.Id == "02")
                                    tipoBenefici.Descrizione = string.Format("{0} / Ex Iritel", tipoBenefici.Descrizione);

                                CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, tipoBenefici.Descrizione, tipoBenefici.Descrizione, tipoBenefici.Id);
                            }
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    ddlTipoSettimaneBeneficio.Items.Clear();
                    CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, string.Empty, string.Empty, "");
                    if (this.areaMaggiorazioneBenefici != null)
                    {
                        if (this.areaMaggiorazioneBenefici.ListaTipoBenefici != null)
                            foreach (Presenter.SvrLiquidazioneFs.TipoBenefici tipoBenefici in this.areaMaggiorazioneBenefici.ListaTipoBenefici)
                            {
                                if (tipoBenefici.Id != "05") // BENEFICI PREVISTI PER EX ART 24 COMMA 15 BIS
                                    CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, tipoBenefici.Descrizione, tipoBenefici.Descrizione, tipoBenefici.Id);
                            }
                    }
                    break;
                default:
                    ddlTipoSettimaneBeneficio.Items.Clear();
                    CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, string.Empty, string.Empty, "");
                    if (this.areaMaggiorazioneBenefici != null)
                    {
                        if (this.areaMaggiorazioneBenefici.ListaTipoBenefici != null)
                            foreach (Presenter.SvrLiquidazioneFs.TipoBenefici tipoBenefici in this.areaMaggiorazioneBenefici.ListaTipoBenefici)
                                CodeUtility.SetValueDdl(ddlTipoSettimaneBeneficio, tipoBenefici.Descrizione, tipoBenefici.Descrizione, tipoBenefici.Id);
                    }
                    break;
            }
        }

        private void ValorizzaEtichetteCommon()
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.DatiBenefici != null)
            {
                if (!string.IsNullOrEmpty(this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio))
                    ddlTipoSettimaneBeneficio.SelectedValue = this.areaMaggiorazioneBenefici.DatiBenefici.TipoSettimaneBeneficio;
                else
                    ddlTipoSettimaneBeneficio.SelectedIndex = 0;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295.HasValue)
                    txtSettAnzContPost311295.Text = this.areaMaggiorazioneBenefici.DatiBenefici.SettAnzContribPost311295.Value.ToString();

                if (this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal.HasValue)
                    txtDataNonVedenteDal.Text = String.Format("{0:dd/MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.DataNonVedenteDal.Value);
            }
            else
                ddlTipoSettimaneBeneficio.SelectedIndex = 0;

            if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione)
                ddlTipoSettimaneBeneficio.SelectedValue = "13";

            if (datiPensione.IsDomandaQuota100OrRicostituzione)
                ddlTipoSettimaneBeneficio.SelectedValue = "14";

            if (datiPensione.IsDomandaQuota102OrRicostituzione)
                ddlTipoSettimaneBeneficio.SelectedValue = "18";

            if (datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)
                ddlTipoSettimaneBeneficio.SelectedValue = "19";

            if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
               (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
               (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                ddlTipoSettimaneBeneficio.SelectedValue = "24";

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

            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.Settimane.HasValue &&
                this.areaMaggiorazioneBenefici.Settimane.Value > 0)
            {
                hdnNumeroSettimaneUtiliDiritto.Value = areaMaggiorazioneBenefici.Settimane.ToString();
            }

            if (Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
                hdnDecorrenzaPost012017.Value = "SI";
        }

        private void ValorizzaEtichetteEL_ET_TT_VL_GAS_DZ_ES_PM()
        {
            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.DatiBenefici != null)
            {
                if (this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.HasValue)
                    txtNumeroSettimaneBeneficio.Text = this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.Value.ToString();
                else
                    txtNumeroSettimaneBeneficio.Text = string.Empty;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
                    txtDecorrenza.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale.Value);
                else
                    txtDecorrenza.Text = string.Empty;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale.HasValue)
                    txtCessazione.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale.Value);
                else
                    txtCessazione.Text = string.Empty;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo != null)
                {
                    for (int i = 0; i < this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo.Count(); i++)
                    {
                        if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 1)
                        {
                            txtOneriTerrorismoUno.Text = Math.Round((decimal)this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                        if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 2)
                        {
                            txtOneriTerrorismoDue.Text = Math.Round((decimal)this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                        if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 3)
                        {
                            txtOneriTerrorismoTre.Text = Math.Round((decimal)this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                    }
                }
            }
            else
            {
                txtNumeroSettimaneBeneficio.Text = string.Empty;
                txtDecorrenza.Text = string.Empty;
                txtCessazione.Text = string.Empty;
                txtOneriTerrorismoUno.Text = string.Empty;
                txtOneriTerrorismoDue.Text = string.Empty;
                txtOneriTerrorismoTre.Text = string.Empty;
            }
        }

        private void ValorizzaEtichetteFS_PT_INPDAP()
        {
            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.DatiBenefici != null)
            {
                if (this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.HasValue)
                    txtNumeroSettimaneBeneficio.Text = this.areaMaggiorazioneBenefici.DatiBenefici.NSettimaneBeneficio.Value.ToString();
                else
                    txtNumeroSettimaneBeneficio.Text = string.Empty;

                txtAABeneficioTemporale.Text = this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioAA.HasValue ? this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioAA.Value.ToString() : String.Empty;
                txtMMBeneficioTemporale.Text = this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioMM.HasValue ? this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioMM.Value.ToString() : String.Empty;
                txtGGBeneficioTemporale.Text = this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioGG.HasValue ? this.areaMaggiorazioneBenefici.DatiBenefici.SettimaneBeneficioGG.Value.ToString() : String.Empty;
                if (this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
                    txtDecorrenza.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale.Value);
                else
                    txtDecorrenza.Text = string.Empty;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale.HasValue)
                    txtCessazione.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale.Value);
                else
                    txtCessazione.Text = string.Empty;
            }
            else
            {
                txtAABeneficioTemporale.Text = string.Empty;
                txtMMBeneficioTemporale.Text = string.Empty;
                txtGGBeneficioTemporale.Text = string.Empty;
                txtDecorrenza.Text = string.Empty;
                txtCessazione.Text = string.Empty;
            }

        }

        private void ValorizzaEtichetteCL()
        {
            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.DatiBenefici != null)
            {
                if (this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale.HasValue)
                    txtDecorrenza.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.DecorrenzaMaggiorazioneSociale.Value);
                else
                    txtDecorrenza.Text = string.Empty;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale.HasValue)
                    txtCessazione.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiBenefici.CessazioneMaggiorazioneSociale.Value);
                else
                    txtCessazione.Text = string.Empty;

                if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo != null)
                {
                    for (int i = 0; i < this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo.Count(); i++)
                    {
                        if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 1)
                        {
                            txtOneriTerrorismoUno.Text = Math.Round((decimal)this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                        if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 2)
                        {
                            txtOneriTerrorismoDue.Text = Math.Round((decimal)this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                        if (this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo != null &&
                            this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Progressivo == 3)
                        {
                            txtOneriTerrorismoTre.Text = Math.Round((decimal)this.areaMaggiorazioneBenefici.DatiBenefici.ListOneriTerrorismo[i].Importo, 2).ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        }

                    }
                }
            }
            else
            {
                txtDecorrenza.Text = string.Empty;
                txtCessazione.Text = string.Empty;
                txtOneriTerrorismoUno.Text = string.Empty;
                txtOneriTerrorismoDue.Text = string.Empty;
                txtOneriTerrorismoTre.Text = string.Empty;
            }
        }

        private void GestioneEtichetteIsUnicarpe(IMaggiorazioneBenefici maggiorazioneBenefici)
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
                }
            }
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                !this.domanda.IsDomandaINPDAP && this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
            {
                ddlTipoSettimaneBeneficio.Enabled = false;
                txtDataNonVedenteDal.Enabled = false;
                txtNumeroSettimaneBeneficio.Enabled = false;
                txtSettAnzContPost311295.Enabled = false;
                //Eng - per maggiorazioni sociali (gruppo= 0031, prodotto= 0101 / 0301 /0401, tipo= 0101)ad eccezione di FS,PT,INPDAP
                //bisogna rendere editabili i campi Decorrenza e Cessazione del pannello “Maggiorazione sociale”
                if (((datiPensione.CodeProdotto != "0101" && datiPensione.CodeProdotto != "0301" && datiPensione.CodeProdotto != "0401") || datiPensione.CodeTipo != "0101"))
                {
                    txtDecorrenza.Enabled = false;
                    txtCessazione.Enabled = false;
                }
                txtOneriTerrorismoUno.Enabled = false;
                txtOneriTerrorismoDue.Enabled = false;
                txtOneriTerrorismoTre.Enabled = false;
                btnEliminaBenefici.Enabled = false;
            }

            if (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && !this.domanda.IsDomandaINPDAP &&
                this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS &&
                this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
            {
                txtNumeroSettimaneBeneficio.Enabled = true;
            }
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IMaggiorazioneBenefici
    }
}