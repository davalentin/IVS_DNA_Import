using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.Aggiornamento
{

    public class TipoAggiornamentoToastArgs : EventArgs
    {
        public AreaAggiornamento.TipoAggiornamento TipoAggiornamentoInCorso { get; set; }

        public TipoAggiornamentoToastArgs(AreaAggiornamento.TipoAggiornamento tipo)
        {
            TipoAggiornamentoInCorso = tipo;
        }
    }

    public partial class UCAggiornamento : CustomBaseUserControl, IAggiornamento
    {
        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }


        public string Titolo
        {
            get
            {
                object value = ViewState["Titolo"];
                if (value == null)
                    return string.Empty;

                return (string)value;
            }
            set
            {
                ViewState["Titolo"] = value;
            }
        }

        #endregion IViewUI Members

        #region IAggiornamento
        public UtilityTipoAppartenenza? TipoApp { get; set; }
        public AreaAggiornamento areaAggiornamento { get; set; }
        #endregion IAggiornamento

        protected void Page_Load(object sender, EventArgs e)
        {
            lblTitolo.Text = Titolo;
        }

        internal void ValorizzaEtichette(AreaAggiornamento.AreaAggiornamentoGeneric areaAggiornamento)
        {
            pnlElabora.Visible = true;
            if (areaAggiornamento.DomandeDomandeTotali.GetValueOrDefault() > 0)
                btnElabora.Enabled = true;
            else
                btnElabora.Enabled = false;
            if (areaAggiornamento.DomandeElaborateConErrore.GetValueOrDefault() > 0)
                btnPDF.Enabled = true;
            else
                btnPDF.Enabled = false;

            lblDomandeTotali.Text = areaAggiornamento.DomandeDomandeTotali.GetValueOrDefault().ToString();
        }

        protected void btnElabora_Click(object sender, EventArgs e)
        {
            RaiseHideAvviso(this, null);
            this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.areaAggiornamento = new AreaAggiornamento();
            this.areaAggiornamento.TipoAggiornamentoInCorso = GetTipoAggiornamento();

            PresenterAggiornamento presenter = new PresenterAggiornamento();
            presenter.ElaboraAggiornamento(this);

            if (HasError)
                RaiseShowElaborazioneInCorso(this, null);
            else
                RaiseRecuperaInformazioni();
        }

        protected void btnGeneraPDF_Click(object sender, EventArgs e)
        {
            this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            AreaAggiornamento.TipoAggiornamento tipoAggiornamento = GetTipoAggiornamento();
            string tipoAgg = string.Empty;
            switch (tipoAggiornamento)
            {
                case AreaAggiornamento.TipoAggiornamento.WebDom:
                    tipoAgg = "W";
                    break;
                case AreaAggiornamento.TipoAggiornamento.Felpe:
                    tipoAgg = "F";
                    break;
                case AreaAggiornamento.TipoAggiornamento.Oneri:
                    tipoAgg = "O";
                    break;
                case AreaAggiornamento.TipoAggiornamento.Cumulo:
                    tipoAgg = "C";
                    break;
                case AreaAggiornamento.TipoAggiornamento.SAI:
                    tipoAgg = "S";
                    break;
                case AreaAggiornamento.TipoAggiornamento.INPDAP:
                    tipoAgg = "I";
                    break;
                case AreaAggiornamento.TipoAggiornamento.NoteDiDebito:
                    tipoAgg = "N";
                    break;
                case AreaAggiornamento.TipoAggiornamento.Tot:
                    tipoAgg = "T";
                    break;
                case AreaAggiornamento.TipoAggiornamento.PianiDiPagamento:
                    tipoAgg = "P";
                    break;
            }
            string path = "StampaAggiornamento.aspx?TipoApp=" + this.TipoApp.ToString() + "&Tipo=" + tipoAgg;
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "newWindow", String.Format("<script>window.open('{0}', '', 'toolbar=no,resizable=yes,scrollbars=yes');</script>", path));
        }

        internal void SetTipoAggiornamento(AreaAggiornamento.TipoAggiornamento tipoAggiornamento)
        {
            ViewState[EnumViewState.TipoAggiornamento.ToString()] = tipoAggiornamento;
        }

        internal AreaAggiornamento.TipoAggiornamento GetTipoAggiornamento()
        {
            return (AreaAggiornamento.TipoAggiornamento)ViewState[EnumViewState.TipoAggiornamento.ToString()];
        }

        #region EventHandler

        public event EventHandler ShowElaborazioneInCorso;
        public event EventHandler<TipoAggiornamentoToastArgs> RecuperaInformazioni;
        public event EventHandler HideAvviso;

        public void RaiseShowElaborazioneInCorso(object sender, EventArgs args)
        {
            if (ShowElaborazioneInCorso != null)
                ShowElaborazioneInCorso(sender, args);
        }

        public void RaiseRecuperaInformazioni()
        {
            var args = new TipoAggiornamentoToastArgs(this.areaAggiornamento.TipoAggiornamentoInCorso);
            if (RecuperaInformazioni != null)
                RecuperaInformazioni(this, args);
        }

        public void RaiseHideAvviso(object sender, EventArgs args)
        {
            if (HideAvviso != null)
                HideAvviso(sender, args);
        }

        #endregion EventHandler

        #region enum
        public enum EnumViewState
        {
            TipoAggiornamento,
        }
        #endregion enum
    }
}
