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
using INPS.DNA.Context;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.CambioStatoDomanda
{
    public partial class UCCambioStatoDomanda : CustomBaseUserControl, ICambioStatoDomanda
    {

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ICambioStatoDomanda
        public AreaCambioStatoDomanda areaCambioStatoDomanda { get; set; }
        #endregion ICambioStatoDomanda

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadDdl();
        }

        private void LoadDdl()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            foreach (var elem in valoriDecodificati.ElencoStatiPensione)
            {
                if (elem.DecodificaStato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoWebDom) || elem.DecodificaStato == Utility.GetDescription(CodeUtility.StatoPensione.DaCalcolare))
                    CodeUtility.SetValueDdl(ddlStatoPensione, elem.DecodificaStato, elem.DecodificaStato, elem.CodiceStato);
            }
        }

        protected void btnRicerca_Click(object sender, EventArgs e)
        {
            this.txtNuovoNCertificato.Text = null;
            this.txtNuovaDataElaborazioneWebdom.Text = null;
            RecuperaCampi();

            PresenterCambiaStatoDomanda presenterCambiaStatoDomanda = new PresenterCambiaStatoDomanda();
            this.areaCambioStatoDomanda.IsUpdateOperation = false;
            presenterCambiaStatoDomanda.CambioStatoDomanda(this);

            ValorizzaEtichette();
            if (!this.HasError)
            {
                pnlInfoDomanda.Visible = true;
            }
            else
            {
                pnlInfoDomanda.Visible = false;
                if (!string.IsNullOrEmpty(this.areaCambioStatoDomanda.SedeDiversa) && CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
                {
                    HdnSedeOperatore.Value = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0');
                    HdnSedeDomanda.Value = this.areaCambioStatoDomanda.SedeDiversa;
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUp", "<script>ShowPopUp();</script>", false);
                    return;
                }
            }
            RaiseShowAvviso(this, new EventArgs());
        }

        protected void btnCambiaStato_Click(object sender, EventArgs args)
        {
            RecuperaCampi();
            this.areaCambioStatoDomanda.IsUpdateOperation = true;
            PresenterCambiaStatoDomanda presenterCambiaStatoDomanda = new PresenterCambiaStatoDomanda();
            presenterCambiaStatoDomanda.CambioStatoDomanda(this);
            ValorizzaEtichette();
            ShowAvvisoStatoCambiato(this, new EventArgs());
        }

        protected void btnConfermaPopUp_Click(object sender, EventArgs e)
        {
            string sedeDomanda = HdnSedeDomanda.Value;
            string sedeOperatore = HdnSedeOperatore.Value;
            string messaggioVideo = string.Empty;

            if (sedeDomanda != sedeOperatore)
            {
                if (!CodeUtility.ChangeSede((Ruoli)Session["Ruolo"], sedeDomanda, false, out messaggioVideo))
                {
                    this.HasError = true;
                    this.ErrorMessage = messaggioVideo;
                    RaiseShowAvviso(this, null);
                    return;
                }

                RaiseReloadUChangeSede(this, null);
            }

            HdnSedeDomanda.Value = string.Empty;
            HdnSedeOperatore.Value = string.Empty;

            pnlInfoDomanda.Visible = true;

            RaiseHideAvviso(this, null);
        }

        private void RecuperaCampi()
        {
            this.areaCambioStatoDomanda = new AreaCambioStatoDomanda();
            //info di sessione
            this.areaCambioStatoDomanda.TipoAppOperatore = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.areaCambioStatoDomanda.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            this.areaCambioStatoDomanda.Sede = int.Parse(INPS.Pensioni.LiquidazionePensione.Presenter.Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + INPS.Pensioni.LiquidazionePensione.Presenter.Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0'));

            if (!string.IsNullOrEmpty(txtNumeroDomanda.Text))
                this.areaCambioStatoDomanda.NumeroDomanda = long.Parse(txtNumeroDomanda.Text);
            if (!string.IsNullOrEmpty(lblNumeroDomanda.Text))
                this.areaCambioStatoDomanda.NumeroDomandaUpdate = long.Parse(lblNumeroDomanda.Text);
            if (!string.IsNullOrEmpty(this.ddlStatoPensione.SelectedItem.Text))
                areaCambioStatoDomanda.NuovoStatoPensione = this.ddlStatoPensione.SelectedItem.Text;
            if (!string.IsNullOrEmpty(this.txtNuovoNCertificato.Text))
                areaCambioStatoDomanda.NuovoNCertificato = this.txtNuovoNCertificato.Text;
            if (!string.IsNullOrEmpty(this.txtNuovaDataElaborazioneWebdom.Text))
                areaCambioStatoDomanda.NuovaDataElaborazioneWebdom = Utility.GetDateFromString(this.txtNuovaDataElaborazioneWebdom.Text);
        }

        public void ValorizzaEtichette()
        {
            if (areaCambioStatoDomanda.NumeroDomandaUpdate != 0)
                this.lblNumeroDomanda.Text = areaCambioStatoDomanda.NumeroDomandaUpdate.ToString();

            if (!string.IsNullOrEmpty(areaCambioStatoDomanda.StatoPensione))
                this.lblStatoPensioneAttuale.Text = areaCambioStatoDomanda.StatoPensione;

            if (!string.IsNullOrEmpty(areaCambioStatoDomanda.NCertificato))
                this.lblNumeroCertificatoAttuale.Text = areaCambioStatoDomanda.NCertificato;

                this.lblDataElabWebdomAttuale.Text = areaCambioStatoDomanda.DataElaborazioneWebdom != null ? areaCambioStatoDomanda.DataElaborazioneWebdom.Value.ToString("dd/MM/yyyy") : null;
        }

        #region Events

        public event EventHandler ShowAvviso;
        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvvisoStatoCambiato;
        protected void RaiseShowError(object sender, EventArgs args)
        {
            ShowAvvisoStatoCambiato(sender, args);
        }

        public event EventHandler HideAvviso;
        protected void RaiseHideAvviso(object sender, EventArgs args)
        {
            HideAvviso(sender, args);
        }

        public event EventHandler ReloadUChangeSede;
        protected void RaiseReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede(sender, e);
        }
        #endregion Events
    }
}
