using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDoppioCalcolo_ES_DZ_GAS_PM : CustomBaseUserControl
    {
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        pnlFondi_ES_DZ_PM.Visible = false;
                        pnlFondi_GAS.Visible = true;
                        break;
                    default:
                        pnlFondi_ES_DZ_PM.Visible = true;
                        pnlFondi_GAS.Visible = false;
                        break;
                }
            }
        }

        public void RecuperaCampiComma707(AreaDatiContributivi areaContributivi)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    if (!string.IsNullOrEmpty(txtQuotaAComma707_GAS.Text))
                        areaContributivi.DatiCalcolo.QuotaA707 = short.Parse(txtQuotaAComma707_GAS.Text);
                    if (!string.IsNullOrEmpty(txtQuotaBComma707_GAS.Text))
                        areaContributivi.DatiCalcolo.QuotaB707 = short.Parse(txtQuotaBComma707_GAS.Text);
                    if (!string.IsNullOrEmpty(txtQuotaAComma707Esclusive_GAS.Text))
                        areaContributivi.DatiCalcolo.QuotaAES707 = short.Parse(txtQuotaAComma707Esclusive_GAS.Text);
                    if (!string.IsNullOrEmpty(txtQuotaBComma707Esclusive_GAS.Text))
                        areaContributivi.DatiCalcolo.QuotaBES707 = short.Parse(txtQuotaBComma707Esclusive_GAS.Text);
                    break;
                default:
                    if (!string.IsNullOrEmpty(txtQuotaAComma707.Text))
                        areaContributivi.DatiCalcolo.QuotaA707 = short.Parse(txtQuotaAComma707.Text);
                    if (!string.IsNullOrEmpty(txtQuotaBComma707.Text))
                        areaContributivi.DatiCalcolo.QuotaB707 = short.Parse(txtQuotaBComma707.Text);
                    if (!string.IsNullOrEmpty(txtRetribuzionePonderataComma707.Text))
                        areaContributivi.DatiCalcolo.RetribuzionePonderataAGO707 = decimal.Parse(txtRetribuzionePonderataComma707.Text);
                    break;
            }
        }

        public void ValorizzaEtichetteComma707(AreaDatiContributivi areaDatiContributivi)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //Comma 707
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    txtQuotaAComma707_GAS.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707.Value.ToString() : string.Empty;
                    txtQuotaBComma707_GAS.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707.Value.ToString() : string.Empty;
                    txtQuotaAComma707Esclusive_GAS.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaAES707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaAES707.Value.ToString() : string.Empty;
                    txtQuotaBComma707Esclusive_GAS.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaBES707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaBES707.Value.ToString() : string.Empty;
                    break;
                default:
                    txtQuotaAComma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaA707.Value.ToString() : string.Empty;
                    txtQuotaBComma707.Text = areaDatiContributivi.DatiCalcolo != null && areaDatiContributivi.DatiCalcolo.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcolo.QuotaB707.Value.ToString() : string.Empty;
                    txtRetribuzionePonderataComma707.Text = areaDatiContributivi.DatiCalcolo != null ? areaDatiContributivi.DatiCalcolo.RetribuzionePonderataAGO707.ToString() : string.Empty;
                    break;
            }
            
            // Render Controls ex comma 707
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    trRetribuzionePonderata707.Visible = false;
                    break;
            }

            // TODO: Rimuovere nel momento in cui verranno portati in produzione i fondi con il comma 707
            // Disabilitazione Required Field Validator per comma 707
            switch (this.domanda.Tipofondo)
            {
                default:
                    RFVtxtRetribuzionePonderataComma707.Enabled = false;
                    break;
            }
        }

        public void SetValidationGroup(string validationGroup)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    REV_txtQuotaAComma707_GAS.ValidationGroup = validationGroup;
                    REV_txtQuotaBComma707_GAS.ValidationGroup = validationGroup;
                    REV_txtQuotaAComma707Esclusive_GAS.ValidationGroup = validationGroup;
                    REV_txtQuotaBComma707Esclusive_GAS.ValidationGroup = validationGroup;
                    break;
                default:
                    REV_txtQuotaAComma707.ValidationGroup = validationGroup;
                    REV_txtQuotaBComma707.ValidationGroup = validationGroup;
                    REV_txtRetribuzionePonderataComma707.ValidationGroup = validationGroup;
                    RFVtxtRetribuzionePonderataComma707.ValidationGroup = validationGroup;
                    break;
            }
        }
    }
}
