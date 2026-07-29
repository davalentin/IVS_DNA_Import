using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class AggiornamentoWebDom : CustomBasePage, IAggiornamentoWebDom
    {
        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IAggiornamentoWebDom
        public UtilityTipoAppartenenza? TipoApp { get; set; }
        public AreaAggiornamentoWebDom areaAggiornamentoWebDom { get; set; }
        #endregion IAggiornamentoWebDom

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RecuperaInformazioni();
            }
        }

        private bool RecuperaInformazioni()
        {
            this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            PresenterAggiornamentoWebDom presenter = new PresenterAggiornamentoWebDom();
            presenter.GetAggiornamentoWebDom(this);

            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Ko;
                return false;
            }

            if (this.areaAggiornamentoWebDom != null)
            {
                if (this.areaAggiornamentoWebDom.IsAggiornamentoInCorso)
                {
                    pnlElabora.Visible = false;
                    pnlElaborazioneInCorso.Visible = true;
                    if (this.areaAggiornamentoWebDom.DomandeElaborate.HasValue || this.areaAggiornamentoWebDom.DomandeElaborateConErrore.HasValue || this.areaAggiornamentoWebDom.DomandeDaElaborare.HasValue)
                        pnlRiepilogo.Visible = true;
                    else
                        pnlRiepilogo.Visible = false;
                    lblDomandeElaborate.Text = (this.areaAggiornamentoWebDom.DomandeElaborate.GetValueOrDefault() + this.areaAggiornamentoWebDom.DomandeElaborateConErrore.GetValueOrDefault()).ToString();
                    lblDomandeNonElaborate.Text = this.areaAggiornamentoWebDom.DomandeDaElaborare.GetValueOrDefault().ToString();
                    return false;
                }
                else
                {
                    pnlElaborazioneInCorso.Visible = false;
                    pnlElabora.Visible = true;
                    if (this.areaAggiornamentoWebDom.DomandeDomandeTotali.GetValueOrDefault() > 0)
                        btnElabora.Visible = true;
                    else
                        btnElabora.Visible = false;
                    if (this.areaAggiornamentoWebDom.DomandeElaborateConErrore.GetValueOrDefault() > 0)
                        btnPDF.Visible = true;
                    else
                        btnPDF.Visible = false;

                    lblDomandeTotali.Text = this.areaAggiornamentoWebDom.DomandeDomandeTotali.GetValueOrDefault().ToString();
                }
            }

            return true;
        }

        protected void btnAggiorna_Click(object sender, EventArgs e)
        {
            RecuperaInformazioni();
        }

        protected void btnElabora_Click(object sender, EventArgs e)
        {
            this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            PresenterAggiornamentoWebDom presenter = new PresenterAggiornamentoWebDom();
            presenter.ElaboraAggiornamentoWebDom(this);

            if (HasError)
            {
                pnlElabora.Visible = false;
                pnlElaborazioneInCorso.Visible = true;
                pnlRiepilogo.Visible = false;
            }
            else
            {
                if (RecuperaInformazioni())
                {
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Elaborazione completata.";
                    ucAvviso.Tipo = TipoAvviso.Ok;
                }
            }
        }

        protected void btnGeneraPDF_Click(object sender, EventArgs e)
        {
            this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            string path = "StampaAggWebDom.aspx?TipoApp=" + this.TipoApp.ToString();
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "newWindow", String.Format("<script>window.open('{0}', '', 'toolbar=no,resizable=yes,scrollbars=yes');</script>", path));
        }
    }
}