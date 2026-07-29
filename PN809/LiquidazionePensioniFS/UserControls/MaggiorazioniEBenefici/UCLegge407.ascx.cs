using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici
{
    public partial class UCLegge407 : CustomBaseUserControl, IMaggiorazioneBenefici
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SalvaLegge407_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaMaggiorazioneBenefici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
            areaMaggiorazioneBenefici.DatiDL407 = GetValoriDL407();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaTabDatiDL407(this);

            //if (HasError) { 
            RaiseShowAvviso(this, null);
            //}
        }

        protected void EliminaLegge407_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaTabDatiDL407(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati DL407";
            }
            else
            {
                ValorizzaEtichetteDL407(null);
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

        internal Presenter.SvrLiquidazioneFs.DatiDL407 GetValoriDL407()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiDL407 = new Presenter.SvrLiquidazioneFs.DatiDL407();

            if (string.IsNullOrEmpty(txtSettimaneA.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaA = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaA = Convert.ToInt32(txtSettimaneA.Text);

            if (string.IsNullOrEmpty(txtSettimaneB.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaB = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaB = Convert.ToInt32(txtSettimaneB.Text);

            if (string.IsNullOrEmpty(txtSettimaneC.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaC = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaC = Convert.ToInt32(txtSettimaneC.Text);

            if (string.IsNullOrEmpty(txtSettimaneD.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaD = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaD = Convert.ToInt32(txtSettimaneD.Text);

            if (string.IsNullOrEmpty(txtRMSA.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaA = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaA = Convert.ToDecimal(txtRMSA.Text);

            if (string.IsNullOrEmpty(txtRMSB.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaB = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaB = Convert.ToDecimal(txtRMSB.Text);

            if (string.IsNullOrEmpty(txtRMSD.Text))
                this.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaD = null;
            else
                this.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaD = Convert.ToDecimal(txtRMSD.Text);

            RecuperaDatiAnteArmonizzazione();
            return this.areaMaggiorazioneBenefici.DatiDL407;
        }

        internal void ValorizzaEtichetteDL407(IMaggiorazioneBenefici maggBen)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (maggBen != null && maggBen.areaMaggiorazioneBenefici != null && maggBen.areaMaggiorazioneBenefici.DatiDL407 != null)
            {
                txtSettimaneA.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaA.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaA.Value.ToString() : "";
                txtSettimaneB.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaB.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaB.Value.ToString() : "";
                txtSettimaneC.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaC.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaC.Value.ToString() : "";
                txtSettimaneD.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaD.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.NSettimaneQuotaD.Value.ToString() : "";
                txtRMSA.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaA.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : "";
                txtRMSB.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaB.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : "";
                txtRMSD.Text = maggBen.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaD.HasValue ? maggBen.areaMaggiorazioneBenefici.DatiDL407.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : "";
                ValorizzaEtichetteAnteArmonizzazione(maggBen.areaMaggiorazioneBenefici);
            }
            else
            {
                txtSettimaneA.Text = string.Empty;
                txtSettimaneB.Text = string.Empty;
                txtSettimaneC.Text = string.Empty;
                txtSettimaneD.Text = string.Empty;
                txtRMSA.Text = string.Empty;
                txtRMSB.Text = string.Empty;
                txtRMSD.Text = string.Empty;
                txtELAnteArmQtaA_AA.Text = string.Empty;
                txtELAnteArmQtaA_RetrPens.Text = string.Empty;
                txtELAnteArmQtaA_RetrPensSL336.Text = string.Empty;
                txtELAnteArmQtaA_CC.Text = string.Empty;
                txtELAnteArmQtaB_AA.Text = string.Empty;
                txtELAnteArmQtaB_RetrPens.Text = string.Empty;
                txtELAnteArmQtaB_RetrPensSL336.Text = string.Empty;
                txtELAnteArmQtaB_CC.Text = string.Empty;
                txtELAnteArmQtaC_AA.Text = string.Empty;

            }
            GestioneEtichetteRic(datiPensione);
        }

        public void RenderControls(IMaggiorazioneBenefici maggBen)
        {
            if (maggBen.areaMaggiorazioneBenefici != null && maggBen.areaMaggiorazioneBenefici.IsNuovaGestioneDL407ForAnteArm.GetValueOrDefault())
            {
                pnlDL407.Visible = false;
                pnlELAnteArmonizzazione.Visible = true;
            }
        }

        #region Ante Armonizzazione
        private void ValorizzaEtichetteAnteArmonizzazione(AreaMaggiorazioniBenefici areaMaggiorazioneBenefici)
        {
            if (areaMaggiorazioneBenefici != null && areaMaggiorazioneBenefici.DatiDL407 != null && areaMaggiorazioneBenefici.DatiDL407.LstServizioUtileAnteArm != null)
            {
                List<GestioneMaggiorazioniBeneficiDatiServizioUtileDL407> lDatiServizioUtile = areaMaggiorazioneBenefici.DatiDL407.LstServizioUtileAnteArm != null ? areaMaggiorazioneBenefici.DatiDL407.LstServizioUtileAnteArm.ToList() : null;

                if (lDatiServizioUtile != null)
                {
                    foreach (GestioneMaggiorazioniBeneficiDatiServizioUtileDL407 servUtile in lDatiServizioUtile)
                    {
                        switch (servUtile.Quota)
                        {
                            case "A":
                                txtELAnteArmQtaA_AA.Text = servUtile.ServizioUtileAA.ToString();
                                txtELAnteArmQtaA_RetrPens.Text = servUtile.RetribuzionePensionabile.ToString();
                                txtELAnteArmQtaA_RetrPensSL336.Text = servUtile.RetribPensSL336.ToString();
                                break;
                            case "B":
                                txtELAnteArmQtaB_AA.Text = servUtile.ServizioUtileAA.ToString();
                                txtELAnteArmQtaB_RetrPens.Text = servUtile.RetribuzionePensionabile.ToString();
                                txtELAnteArmQtaB_RetrPensSL336.Text = servUtile.RetribPensSL336.ToString();
                                break;
                            case "C":
                                txtELAnteArmQtaC_AA.Text = servUtile.ServizioUtileAA.ToString();
                                break;
                        }
                    }
                }
            }
        }

        private void RecuperaDatiAnteArmonizzazione()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new AreaMaggiorazioniBenefici();

            List<GestioneMaggiorazioniBeneficiDatiServizioUtileDL407> lDatiServUtile = new List<GestioneMaggiorazioniBeneficiDatiServizioUtileDL407>();
            GestioneMaggiorazioniBeneficiDatiServizioUtileDL407 datiServUtile = null;

            if (!string.IsNullOrEmpty(txtELAnteArmQtaA_AA.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaA_RetrPens.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaA_CC.Text)
                || !string.IsNullOrEmpty(txtELAnteArmQtaA_RetrPensSL336.Text))
            {
                datiServUtile = new GestioneMaggiorazioniBeneficiDatiServizioUtileDL407();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtELAnteArmQtaA_AA.Text) ? Convert.ToInt16(txtELAnteArmQtaA_AA.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtELAnteArmQtaA_RetrPens.Text) ? Convert.ToDecimal(txtELAnteArmQtaA_RetrPens.Text) : (decimal?)null;
                datiServUtile.RetribPensSL336 = !string.IsNullOrEmpty(txtELAnteArmQtaA_RetrPensSL336.Text) ? Convert.ToDecimal(txtELAnteArmQtaA_RetrPensSL336.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtELAnteArmQtaA_CC.Text) ? Convert.ToInt16(txtELAnteArmQtaA_CC.Text) : (short?)null;
                datiServUtile.Quota = "A";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtELAnteArmQtaB_AA.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaB_RetrPens.Text) || !string.IsNullOrEmpty(txtELAnteArmQtaB_CC.Text)
                || !string.IsNullOrEmpty(txtELAnteArmQtaB_RetrPensSL336.Text))
            {
                datiServUtile = new GestioneMaggiorazioniBeneficiDatiServizioUtileDL407();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtELAnteArmQtaB_AA.Text) ? Convert.ToInt16(txtELAnteArmQtaB_AA.Text) : (short?)null;
                datiServUtile.RetribuzionePensionabile = !string.IsNullOrEmpty(txtELAnteArmQtaB_RetrPens.Text) ? Convert.ToDecimal(txtELAnteArmQtaB_RetrPens.Text) : (decimal?)null;
                datiServUtile.RetribPensSL336 = !string.IsNullOrEmpty(txtELAnteArmQtaB_RetrPensSL336.Text) ? Convert.ToDecimal(txtELAnteArmQtaB_RetrPensSL336.Text) : (decimal?)null;
                datiServUtile.ControCodiceRetributivo = !string.IsNullOrEmpty(txtELAnteArmQtaB_CC.Text) ? Convert.ToInt16(txtELAnteArmQtaB_CC.Text) : (short?)null;
                datiServUtile.Quota = "B";
                lDatiServUtile.Add(datiServUtile);
            }
            if (!string.IsNullOrEmpty(txtELAnteArmQtaC_AA.Text))
            {
                datiServUtile = new GestioneMaggiorazioniBeneficiDatiServizioUtileDL407();
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtELAnteArmQtaC_AA.Text) ? Convert.ToInt16(txtELAnteArmQtaC_AA.Text) : (short?)null;
                datiServUtile.Quota = "C";
                lDatiServUtile.Add(datiServUtile);
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
                this.areaMaggiorazioneBenefici.DatiDL407.LstServizioUtileAnteArm = lDatiServUtile.ToArray();
        }

        #endregion Ante Armonizzazione

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                txtRMSA.Enabled = false;
                txtSettimaneA.Enabled = false;
                txtRMSB.Enabled = false;
                txtSettimaneB.Enabled = false;
                txtSettimaneC.Enabled = false;
                txtRMSD.Enabled = false;
                txtSettimaneD.Enabled = false;
                txtELAnteArmQtaA_AA.Enabled = false;
                txtELAnteArmQtaA_CC.Enabled = false;
                txtELAnteArmQtaA_RetrPens.Enabled = false;
                txtELAnteArmQtaA_RetrPensSL336.Enabled = false;
                txtELAnteArmQtaB_AA.Enabled = false;
                txtELAnteArmQtaB_CC.Enabled = false;
                txtELAnteArmQtaB_RetrPens.Enabled = false;
                txtELAnteArmQtaB_RetrPensSL336.Enabled = false;
                txtELAnteArmQtaC_AA.Enabled = false;
                btnEliminaLegge407.Enabled = false;
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