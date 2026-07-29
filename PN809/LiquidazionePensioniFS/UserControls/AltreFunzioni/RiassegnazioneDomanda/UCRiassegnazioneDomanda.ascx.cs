using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA.Context;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.RiassegnazioneDomanda
{
    public partial class UCRiassegnazioneDomanda : CustomBaseUserControl, IRiassegnazioneDomanda
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ValorizzaEtichette(this);
        }

        protected void btnRicerca_Click(object sender, EventArgs e)
        {
            GetDati(UtilityTipoOperazione.RICERCA);
            txtNuovaMatricola.Text = string.Empty;

            PresenterRiassegnazioneDomanda presenterRiassegnazioneDomanda = new PresenterRiassegnazioneDomanda();
            presenterRiassegnazioneDomanda.RiassegnaDomanda(this);

            bool showError = true;
            if (!string.IsNullOrEmpty(this.SedeDiversa))
            {
                HdnSede.Value = this.SedeDiversa;
                showError = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUpSede", "<script>ShowPopUpSede();</script>", false);
            }

            GestioneEsitoSvc(UtilityTipoOperazione.RICERCA, showError);
        }

        protected void btnRiassegna_Click(object sender, EventArgs e)
        {
            GetDati(UtilityTipoOperazione.UPDATE);

            PresenterRiassegnazioneDomanda presenterRiassegnazioneDomanda = new PresenterRiassegnazioneDomanda();
            presenterRiassegnazioneDomanda.RiassegnaDomanda(this);

            GestioneEsitoSvc(UtilityTipoOperazione.UPDATE, true);
        }

        private void ValorizzaEtichette(IRiassegnazioneDomanda riassegnazioneDomanda)
        {
            if (riassegnazioneDomanda != null)
            {
                lblNumeroDomanda.Text = riassegnazioneDomanda.NumeroDomanda.ToString();
                lblStatoPensione.Text = riassegnazioneDomanda.StatoPensione;
                lblMatricola.Text = riassegnazioneDomanda.VecchiaMatricola;
            }
        }

        private void GetDati(UtilityTipoOperazione tipoOperazione)
        {
            long numDomanda = 0;
            if (tipoOperazione == UtilityTipoOperazione.RICERCA)
                long.TryParse(txtNumeroDomanda.Text, out numDomanda);

            if (tipoOperazione == UtilityTipoOperazione.UPDATE)
            {
                long.TryParse(lblNumeroDomanda.Text, out numDomanda);
                this.VecchiaMatricola = lblMatricola.Text;
                this.NuovaMatricola = txtNuovaMatricola.Text;
                this.StatoPensione = lblStatoPensione.Text;
            }

            this.NumeroDomanda = numDomanda;
            this.TipoAppOperatore = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            this.TipoOperazione = tipoOperazione;
        }

        private void ClearField()
        {
            txtNuovaMatricola.Text = string.Empty;
        }

        private void ClearFieldLabel()
        {
            lblNumeroDomanda.Text = string.Empty;
            lblStatoPensione.Text = string.Empty;
            lblMatricola.Text = string.Empty;
        }

        private void GestioneEsitoSvc(UtilityTipoOperazione tipoOperazione, bool showError)
        {
            if (this.HasError)
            {
                if (tipoOperazione == UtilityTipoOperazione.RICERCA)
                {
                    ClearFieldLabel();
                    pnlInfoDomanda.Visible = false;
                }

                if (showError)
                    RaiseShowAvviso(this, null);
                return;
            }
            else
            {
                if (tipoOperazione == UtilityTipoOperazione.UPDATE)
                {
                    ClearField();
                    this.ErrorMessage = "Riassegnazione domanda effettuata correttamente";
                    RaiseShowAvviso(this, null);
                }
                else
                {
                    RaiseHideInfo(this, null);
                }

                pnlInfoDomanda.Visible = true;
                ValorizzaEtichette(this);
            }
        }


        public event EventHandler ShowAvviso;
        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler HideInfo;
        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }

        public event EventHandler CambioSede;
        protected void RaiseCambioSede(object sender, EventArgs args)
        {
            CambioSede(sender, args);
        }

        #region IRiassegnazioneDomanda
        public AreaEsito.TipoEsito Esito { get; set; }
        public long NumeroDomanda { get; set; }
        public string StatoPensione { get; set; }
        public string VecchiaMatricola { get; set; }
        public string NuovaMatricola { get; set; }
        public UtilityRuolo? Ruolo { get; set; }
        public UtilityTipoAppartenenza? TipoAppOperatore { get; set; }
        public UtilityTipoOperazione? TipoOperazione { get; set; }
        public string SedeDiversa { get; set; }
        #endregion IRiassegnazioneDomanda

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region Cambio Sede Domanda
        public void btnConfermaPopUp_Click(object sender, EventArgs args)
        {
            string errori = string.Empty;
            string nuovaSede = HdnSede.Value;

            if (!CodeUtility.ChangeSede((Ruoli)Session["Ruolo"], nuovaSede, false, out errori))
            {
                this.HasError = true;
                this.ErrorMessage = errori;
                RaiseShowAvviso(this, null);
                return;
            }
            this.RaiseCambioSede(this, null);
            this.RaiseHideInfo(this, null);
            pnlInfoDomanda.Visible = true;

            btnRicerca_Click(sender, args);
        }


        #endregion Cambio Sede Domanda
    }
}