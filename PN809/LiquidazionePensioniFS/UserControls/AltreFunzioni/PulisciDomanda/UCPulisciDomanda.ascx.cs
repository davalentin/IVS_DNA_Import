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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.PulisciDomanda
{
    public partial class UCPulisciDomanda : CustomBaseUserControl, IPuliziaDomanda
    {
        #region IView
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IView

        #region IPuliziaDomanda
        public long numeroDomanda { get; set; }
        public AreaPuliziaDomanda areaPuliziaDomanda { get; set; }
        public UtilityTipoAppartenenza TipoAppOperatore { get; set; }
        public UtilityRuolo RuoloOperatore { get; set; }
        public int Sede { get; set; }
        #endregion IPuliziaDomanda

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ValorizzaEtichette()
        {
            if (areaPuliziaDomanda != null)
            {
                if (areaPuliziaDomanda.EntityPuliziaDomanda != null)
                {
                    pnlInfoDomanda.Visible = true;
                    lblStatoWebDom.Text = !string.IsNullOrEmpty(areaPuliziaDomanda.EntityPuliziaDomanda.Attivita) ? areaPuliziaDomanda.EntityPuliziaDomanda.Attivita : string.Empty;
                    lblDataInizio.Text = areaPuliziaDomanda.EntityPuliziaDomanda.DataInizio.HasValue ? areaPuliziaDomanda.EntityPuliziaDomanda.DataInizio.Value.ToString("dd/MM/yyyy") : string.Empty;
                    lblDataFine.Text = areaPuliziaDomanda.EntityPuliziaDomanda.DataFine.HasValue ? areaPuliziaDomanda.EntityPuliziaDomanda.DataFine.Value.ToString("dd/MM/yyyy") : string.Empty;

                    lblNumeroDomanda.Text = this.numeroDomanda.ToString();

                    if (!string.IsNullOrEmpty(areaPuliziaDomanda.SedeDiversa))
                        pnlInfoDomanda.Visible = false;
                }
                else
                    pnlInfoDomanda.Visible = false;

                btnChiudiAttivita.Visible = areaPuliziaDomanda.IsPuliziaDisponibile;
            }
        }

        #region Event Methods
        public void btnChiudiAttivita_Click(object sender, EventArgs args)
        {
            //recupera campi 
            if (!string.IsNullOrEmpty(lblNumeroDomanda.Text))
                this.numeroDomanda = long.Parse(lblNumeroDomanda.Text);
            this.TipoAppOperatore = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.RuoloOperatore = CodeUtility.GetRuolo(Session["Ruolo"]);

            Presenter.PresenterPuliziaDomanda presenter = new Presenter.PresenterPuliziaDomanda();
            presenter.PulisciDomanda(this);
            ValorizzaEtichette();
            RaiseShowAvviso(this, new EventArgs());
        }

        public void btnRicerca_Click(object sender, EventArgs args)
        {
            //recupera campi 
            if (!string.IsNullOrEmpty(txtNumeroDomanda.Text))
                this.numeroDomanda = long.Parse(txtNumeroDomanda.Text);
            this.TipoAppOperatore = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.RuoloOperatore = CodeUtility.GetRuolo(Session["Ruolo"]);

            Presenter.PresenterPuliziaDomanda presenter = new Presenter.PresenterPuliziaDomanda();
            presenter.GetDomandaWebDom(this);
            ValorizzaEtichette();
            //mostro messaggio solo in caso di errore
            if (this.HasError)
            {
                if (!string.IsNullOrEmpty(this.areaPuliziaDomanda.SedeDiversa) && CodeUtility.GetRuolo(Session["Ruolo"]) == UtilityRuolo.AMMINISTRATORE)
                {
                    HdnSedeOperatore.Value = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0');
                    HdnSedeDomanda.Value = this.areaPuliziaDomanda.SedeDiversa;
                    ScriptManager.RegisterStartupScript(this, GetType(), "CallShowPopUp", "<script>ShowPopUp();</script>", false);
                    return;
                }
                RaiseShowAvviso(this, new EventArgs());
            }
            else
                RaiseHideAvviso(this, new EventArgs());
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
        #endregion Event Methods

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            HideAvviso(sender, e);
        }

        public event EventHandler ReloadUChangeSede;
        protected void RaiseReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede(sender, e);
        }

        #endregion Events
    }
}