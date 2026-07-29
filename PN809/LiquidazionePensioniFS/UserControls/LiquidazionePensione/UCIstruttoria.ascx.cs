using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Linq;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class Istruttoria : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensione
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensione
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaIstruttoria_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP = GetDatiIstruttoria();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new Presenter.PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiIstruttoriaFS(this);

            if (!this.HasError)
                this.ErrorMessage = "Dati Istruttoria salvati correttamente";

            RaiseShowAvviso(this, null);
        }

        protected void EliminaIstruttoria_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new Presenter.PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiIstruttoriaFS(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Istruttoria";
            else
            {
                this.ErrorMessage = "Dati Istruttoria eliminati correttamente";
                ClearForm();
                ValorizzaEtichetteIstruttoria(this);
            }

            RaiseShowAvviso(this, null);
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

        internal DatiIstruttoriaINPDAP GetDatiIstruttoria()
        {
            DatiIstruttoriaINPDAP datiIstruttoriaINPDAP = new DatiIstruttoriaINPDAP();

            bool? isRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];

            if (!String.IsNullOrEmpty(ddlSoggettoDerogato.SelectedValue))
                datiIstruttoriaINPDAP.CodiceParticolareSoggettoDerogato = long.Parse(ddlSoggettoDerogato.SelectedValue);
            else
                datiIstruttoriaINPDAP.CodiceParticolareSoggettoDerogato = null;

            if (isRiduzioneRetribVisible.HasValue && isRiduzioneRetribVisible.Value)
            {
                if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    datiIstruttoriaINPDAP.RiduzioneRetributiva = true;
                else if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    datiIstruttoriaINPDAP.RiduzioneRetributiva = false;
            }
            datiIstruttoriaINPDAP.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;

            return datiIstruttoriaINPDAP;
        }

        internal void ValorizzaEtichetteIstruttoria(ILiquidazionePensione liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetribVisible.GetValueOrDefault())
                ViewState["RiduzioneRetrib"] = true;

            LoadDdl(liquidazione);

            RenderControls(liquidazione);

            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP.CodiceParticolareSoggettoDerogato.HasValue)
                    ddlSoggettoDerogato.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP.CodiceParticolareSoggettoDerogato.Value.ToString();
                else
                    ddlSoggettoDerogato.SelectedIndex = -1;

                if (liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP.RiduzioneRetributiva)
                    ddlRiduzioneRetributiva.SelectedValue = "SI";
                else
                    ddlRiduzioneRetributiva.SelectedValue = "NO";
                txtRiduzioneRetributiva.Text = liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP.RiduzioneRetributivaPercentuale.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            }

            ManageRiduzioneRetributiva(liquidazione);
            if (datiPensione.FlagUnicarpe.HasValue && datiPensione.FlagUnicarpe.Value)
                GestioneEtichetteIsUnicarpe(datiPensione, liquidazione);

            if (CodeUtility.IsRicostituzione(datiPensione) && liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                ddlSoggettoDerogato.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
        }

        internal bool ManageButtonRiduzioneRetributiva(ILiquidazionePensione liquidazione)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            AreaTitolare.DatiPensione datiPensione = this.GetDatiPensione(this);
            if (titolare != null)
            {
                if (titolare.DataNascita.HasValue && datiPensione.DecorrenzaOriginaria.HasValue)
                {
                    if (!(DateTime.Compare(titolare.DataNascita.Value.AddYears(62), datiPensione.DecorrenzaOriginaria.Value) < 0) &&
                        (liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetribVisible.HasValue && liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetribVisible.Value))
                        return true;
                }
            }

            return false;
        }

        private void ManageRiduzioneRetributiva(ILiquidazionePensione liquidazione)
        {
            if (((bool?)ViewState["RiduzioneRetrib"]).GetValueOrDefault())
            {
                pnlRiduzioneRetributiva.Visible = true;

                bool IsRiduzionePresent = ManageButtonRiduzioneRetributiva(liquidazione);
                //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
                // in caso di VOCRED 3 CRED27 non vamostrato il popup su 62 anni
                if (IsRiduzionePresent && liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null &&
                    liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetributivaEnabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetributivaEnabled.Value)
                    IsRiduzionePresent = false;

                btnSalvaIstruttoriaNoRiduzione.Visible = !IsRiduzionePresent;
                btnPopUp.Visible = IsRiduzionePresent;
                btnSalvaIstruttoria.Visible = IsRiduzionePresent;

                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetributivaEnabled.HasValue &&
                    !liquidazione.areaLiquidazionePensioneFS.IsRiduzioneRetributivaEnabled.Value)
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
        }

        private void LoadDdl(ILiquidazionePensione liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare.Count() > 0 &&
                    liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP != null && liquidazione.areaLiquidazionePensioneFS.DatiIstruttoriaINPDAP.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    pnlSoggettoDerogato.Visible = true;
                    ddlSoggettoDerogato.Items.Clear();
                    CodeUtility.SetValueDdl(ddlSoggettoDerogato, string.Empty, string.Empty, string.Empty);
                    foreach (CodiceParticolare codeParticolare in liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare)
                    {
                        CodeUtility.SetValueDdl(ddlSoggettoDerogato, (codeParticolare.TraduzioneSuGp.HasValue ? codeParticolare.TraduzioneSuGp.Value.ToString() : string.Empty) +
                            " - " + codeParticolare.Descrizione, codeParticolare.Id.ToString());
                    }
                }
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensione liquidazione)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                ddlSoggettoDerogato.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
        }

        private void RenderControls(ILiquidazionePensione liquidazione)
        {
            CodeUtility.DisableEliminaForRicostituzioni(btnEliminaIstruttoria);
        }

        #region events
        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }
        #endregion events
    }
}