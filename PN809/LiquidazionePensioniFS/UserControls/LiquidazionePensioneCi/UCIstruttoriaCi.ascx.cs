using System;
using System.Linq;
using System.Web.UI;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi
{
    public partial class UCIstruttoriaCi : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneCi
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

        #region internal methods
        internal void ValorizzaEtichetteIstruttoria(ILiquidazionePensioneCi liquidazioneCi)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ViewState["RiduzioneRetrib"] = liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetribVisible.HasValue ? liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetribVisible.Value : (bool?)null;

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            LoadDdl(liquidazioneCi);
            Utility.Categoria? categoria = Utility.GetCategoria(this.domanda.Categoria.Trim());

            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                    ddlSoggettoDerogato.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceParticolareSoggettoDerogato.Value.ToString();
                else
                    ddlSoggettoDerogato.SelectedIndex = 0;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.Legge44997.HasValue)
                    ddlCodReqRidotti.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.Legge44997.Value.ToString();
                else
                    ddlCodReqRidotti.SelectedIndex = 0;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceContrattoEquiparato.HasValue)
                    txtCodContrEqu.Text = liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceContrattoEquiparato.Value.ToString();
                else
                    txtCodContrEqu.Text = string.Empty;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceLivelloEquip.HasValue)
                    txtCodLivEqu.Text = liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceLivelloEquip.Value.ToString();
                else
                    txtCodLivEqu.Text = string.Empty;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.RiduzioneRetributiva)
                    ddlRiduzioneRetributiva.SelectedValue = "SI";
                else ddlRiduzioneRetributiva.SelectedValue = "NO";
                txtRiduzioneRetributiva.Text = liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.RiduzioneRetributivaPercentuale.HasValue ? 
                    liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            }

            ManageRiduzioneRetributiva(liquidazioneCi);
            if (datiPensione.FlagUnicarpe.HasValue)
                GestioneEtichetteIsUnicarpe(datiPensione, liquidazioneCi);

            if (CodeUtility.IsRicostituzione(datiPensione) && liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.GetValueOrDefault())
            {
                ddlCodReqRidotti.Enabled = false;
                ddlSoggettoDerogato.Enabled = false;
                txtCodContrEqu.Enabled = false;
                txtCodLivEqu.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;

            }
        }

        internal DatiIstruttoria GetDatiIstruttoria()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiIstruttoria = new DatiIstruttoria();

            areaLiquidazionePensioneCi.IsRiduzioneRetribVisible = (bool?)ViewState["RiduzioneRetrib"];

            if (!String.IsNullOrEmpty(ddlCodReqRidotti.SelectedValue))
                areaLiquidazionePensioneCi.DatiIstruttoria.Legge44997 = byte.Parse(ddlCodReqRidotti.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiIstruttoria.Legge44997 = null;

            if (!String.IsNullOrEmpty(ddlSoggettoDerogato.SelectedValue))
                areaLiquidazionePensioneCi.DatiIstruttoria.CodiceParticolareSoggettoDerogato = long.Parse(ddlSoggettoDerogato.SelectedValue);
            else
                areaLiquidazionePensioneCi.DatiIstruttoria.CodiceParticolareSoggettoDerogato = null;

            if (!String.IsNullOrEmpty(txtCodContrEqu.Text))
                areaLiquidazionePensioneCi.DatiIstruttoria.CodiceContrattoEquiparato = short.Parse(txtCodContrEqu.Text);
            else
                areaLiquidazionePensioneCi.DatiIstruttoria.CodiceContrattoEquiparato = null;

            if (!String.IsNullOrEmpty(txtCodLivEqu.Text))
                areaLiquidazionePensioneCi.DatiIstruttoria.CodiceLivelloEquip = short.Parse(txtCodLivEqu.Text);
            else
                areaLiquidazionePensioneCi.DatiIstruttoria.CodiceLivelloEquip = null;

            if (areaLiquidazionePensioneCi.IsRiduzioneRetribVisible.HasValue && areaLiquidazionePensioneCi.IsRiduzioneRetribVisible.Value)
            {
                if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "SI"))
                    areaLiquidazionePensioneCi.DatiIstruttoria.RiduzioneRetributiva = true;
                else if (string.Equals(ddlRiduzioneRetributiva.SelectedValue, "NO"))
                    areaLiquidazionePensioneCi.DatiIstruttoria.RiduzioneRetributiva = false;
            }
            areaLiquidazionePensioneCi.DatiIstruttoria.RiduzioneRetributivaPercentuale = !string.IsNullOrEmpty(txtRiduzioneRetributiva.Text) ? Convert.ToDecimal(txtRiduzioneRetributiva.Text) : (decimal?)null;

            return areaLiquidazionePensioneCi.DatiIstruttoria;
        }

        internal void EnableDisableBtnSalva(bool enable)
        {
            this.btnSalvaIstruttoria.Enabled = enable;
            this.btnPopUp.Enabled = enable;
            this.btnSalvaIstruttoriaNoRiduzione.Enabled = enable;
            this.btnEliminaIstruttoria.Enabled = enable;
        }

        internal bool ManageButtonRiduzioneRetributiva(ILiquidazionePensioneCi liquidazioneCi)
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            AreaTitolare.DatiPensione datiPensione = this.GetDatiPensione(this);
            if (titolare != null)
            {
                if (titolare.DataNascita.HasValue && datiPensione.DecorrenzaOriginaria.HasValue)
                {
                    if (!(DateTime.Compare(titolare.DataNascita.Value.AddYears(62), datiPensione.DecorrenzaOriginaria.Value) < 0) &&
                        (liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetribVisible.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetribVisible.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion internal methods

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
            BindClick();
            AddInputClass();
        }

        protected void SalvaIstruttoria_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiIstruttoria = GetDatiIstruttoria();

            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiIstruttoriaCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaIstruttoria_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiIstruttoriaCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Istruttoria";
            }
            else
            {
                ClearForm();
                ValorizzaEtichetteIstruttoria(this);
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
        #endregion protected methods

        #region private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        private void BindClick()
        {
            //chkExCombattente.Attributes.Add("onclick", "javascript:SetCheckBox(this)");
        }

        private void AddInputClass()
        {
            //chkExCombattente.InputAttributes.Add("EnableClass", "onClassExCombattente");
        }

        private void LoadDdl(ILiquidazionePensioneCi liquidazioneCi)
        {
            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.listaDecodificaLegge44997 != null && liquidazioneCi.areaLiquidazionePensioneCi.listaDecodificaLegge44997.Count() > 0)
                {
                    ddlCodReqRidotti.Items.Clear();
                    CodeUtility.SetValueDdl(ddlCodReqRidotti, string.Empty, string.Empty);
                    foreach (DecodificaLegge44997 codeLegge44997 in liquidazioneCi.areaLiquidazionePensioneCi.listaDecodificaLegge44997)
                    {
                        CodeUtility.SetValueDdl(ddlCodReqRidotti, codeLegge44997.Descrizione, codeLegge44997.Id.ToString());
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.listaCodiceParticolare != null && liquidazioneCi.areaLiquidazionePensioneCi.listaCodiceParticolare.Count() > 0 &&
                    liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiIstruttoria.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    pnlSoggettoDerogato.Visible = true;
                    ddlSoggettoDerogato.Items.Clear();
                    CodeUtility.SetValueDdl(ddlSoggettoDerogato, string.Empty, string.Empty, string.Empty);
                    foreach (CodiceParticolare codeParticolare in liquidazioneCi.areaLiquidazionePensioneCi.listaCodiceParticolare)
                    {
                        CodeUtility.SetValueDdl(ddlSoggettoDerogato, (codeParticolare.TraduzioneSuGp.HasValue ? codeParticolare.TraduzioneSuGp.Value.ToString() : string.Empty) +
                            " - " + codeParticolare.Descrizione, codeParticolare.Id.ToString());
                    }
                }

                // IMPLEMENTARE CARICAMENTO PER LE ALTRE DUE DDL
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione, ILiquidazionePensioneCi liquidazioneCi)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                ddlSoggettoDerogato.Enabled = false;
                ddlCodReqRidotti.Enabled = false;
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
            else if (tipoUnicarpe == Utility.TipoUnicarpe.Manuale && liquidazioneCi != null &&
                liquidazioneCi.areaLiquidazionePensioneCi != null && ((liquidazioneCi.areaLiquidazionePensioneCi.TipologiaSalvaguardia.HasValue) ||
                (liquidazioneCi.areaLiquidazionePensioneCi.IsUsuranti.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.IsUsuranti.Value)))
            {
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
        }

        private void ManageRiduzioneRetributiva(ILiquidazionePensioneCi liquidazioneCi)
        {
            if (((bool?)ViewState["RiduzioneRetrib"]).HasValue && ((bool?)ViewState["RiduzioneRetrib"]).Value)
            {
                pnlRiduzioneRetributiva.Visible = true;

                bool IsRiduzionePresent = ManageButtonRiduzioneRetributiva(liquidazioneCi);
                //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
                if (IsRiduzionePresent && liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null &&
                    ((liquidazioneCi.areaLiquidazionePensioneCi.IsUsuranti.HasValue && liquidazioneCi.areaLiquidazionePensioneCi.IsUsuranti.Value) ||
                    (liquidazioneCi.areaLiquidazionePensioneCi.TipologiaSalvaguardia.HasValue) ||
                    (liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetributivaEnabled.HasValue && !liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetributivaEnabled.Value)))
                    IsRiduzionePresent = false;
                btnSalvaIstruttoriaNoRiduzione.Visible = !IsRiduzionePresent;
                btnPopUp.Visible = IsRiduzionePresent;
                btnSalvaIstruttoria.Visible = IsRiduzionePresent;

                if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetributivaEnabled.HasValue && !liquidazioneCi.areaLiquidazionePensioneCi.IsRiduzioneRetributivaEnabled.Value)
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
        #endregion private methods

        #region events
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
        #endregion events
    }
}