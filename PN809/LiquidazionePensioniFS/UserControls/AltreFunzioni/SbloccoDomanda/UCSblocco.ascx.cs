using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA.Context;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.SbloccoDomanda
{
    public partial class UCSblocco : CustomBaseUserControl, ISbloccoDomanda
    {
        #region ISbloccoDomanda
        public long numDomanda { get; set; }
        public AreaEsito areaEsito { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        public string sedeDiversa { get; set; }
        #endregion ISbloccoDomanda

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (!IsPostBack)
            {
            }
        }

        protected void btnSblocco_Click(object sender, EventArgs e)
        {
            SbloccaDomanda();
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

            SbloccaDomanda();
        }

        #region private methods
        private void SbloccaDomanda()
        {
            if (string.IsNullOrEmpty(txtNumeroDomanda.Text))
            {
                this.HasError = true;
                this.ErrorMessage = "Errore nell'esecuzione dello sblocco domanda: numero domanda mancante";
                RaiseShowAvviso(this, null);
                return;
            }

            long numDom = 0;
            long.TryParse(txtNumeroDomanda.Text, out numDom);
            if (numDom == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Errore nell'esecuzione dello sblocco domanda: numero domanda non corretto";
                RaiseShowAvviso(this, null);
                return;
            }

            this.numDomanda = numDom;

            PresenterSbloccoDomanda presenterSbloccoDomanda = new PresenterSbloccoDomanda();
            presenterSbloccoDomanda.SbloccoDomanda(this);

            if (this.HasError)
            {
                if (!string.IsNullOrEmpty(this.sedeDiversa) && CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
                {
                    HdnSedeOperatore.Value = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0');
                    HdnSedeDomanda.Value = this.sedeDiversa;
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUp", "<script>ShowPopUp();</script>", false);
                    return;
                }
                RaiseShowAvviso(this, null);
                return;
            }

            RaiseShowInfo(this, null);
        }
        #endregion private methods

        #region Events
        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowInfo(object sender, EventArgs e)
        {
            ShowInfo(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }

        protected void RaiseReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede(sender, e);
        }

        public event EventHandler ShowAvviso;

        public event EventHandler ShowInfo;

        public event EventHandler HideInfo;

        public event EventHandler ReloadUChangeSede;
        #endregion Events
    }
}