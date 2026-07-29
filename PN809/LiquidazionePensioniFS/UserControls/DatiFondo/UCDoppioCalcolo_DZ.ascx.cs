using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCDoppioCalcolo_DZ: CustomBaseUserControl
    {
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                pnlFondi_ES_DZ_PM.Visible = true;
                pnlFondi_GAS.Visible = false;
            }
        }

        public void RecuperaCampiComma707(AreaDatiFondo areaDatiFondo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!string.IsNullOrEmpty(txtQuotaAComma707.Text))
                areaDatiFondo.DatiCalcoloDZ.QuotaA707 = short.Parse(txtQuotaAComma707.Text);
            if (!string.IsNullOrEmpty(txtQuotaBComma707.Text))
                areaDatiFondo.DatiCalcoloDZ.QuotaB707 = short.Parse(txtQuotaBComma707.Text);
            if (!string.IsNullOrEmpty(txtRetribuzionePonderataComma707.Text))
                areaDatiFondo.DatiCalcoloDZ.RetribuzionePonderataAGO707 = decimal.Parse(txtRetribuzionePonderataComma707.Text);
            
        }

        public void ValorizzaEtichetteComma707(AreaDatiFondo areaDatiFondo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            txtQuotaAComma707.Text = areaDatiFondo.DatiCalcoloDZ != null && areaDatiFondo.DatiCalcoloDZ.QuotaA707.HasValue ? areaDatiFondo.DatiCalcoloDZ.QuotaA707.Value.ToString() : string.Empty;
            txtQuotaBComma707.Text = areaDatiFondo.DatiCalcoloDZ != null && areaDatiFondo.DatiCalcoloDZ.QuotaB707.HasValue ? areaDatiFondo.DatiCalcoloDZ.QuotaB707.Value.ToString() : string.Empty;
            txtRetribuzionePonderataComma707.Text = areaDatiFondo.DatiCalcoloDZ != null ? areaDatiFondo.DatiCalcoloDZ.RetribuzionePonderataAGO707.ToString() : string.Empty;
            
            // Render Controls ex comma 707
            switch (this.domanda.Tipofondo)
            {
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

            REV_txtQuotaAComma707.ValidationGroup = validationGroup;
            REV_txtQuotaBComma707.ValidationGroup = validationGroup;
            REV_txtRetribuzionePonderataComma707.ValidationGroup = validationGroup;
            RFVtxtRetribuzionePonderataComma707.ValidationGroup = validationGroup;
        }
    }
}
