using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class AggiornaSai : CustomBasePage, IInfoPostCalcolo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IInfoPostCalcolo
        public AreaTitolare.DatiPensione datiPensione { get; set; }
        public Presenter.SvrLiquidazione.AreaEsito areaEsito { get; set; }
        public string statoPensione { get; set; }
        #endregion IInfoPostCalcolo

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                this.divResult.Style.Add("display", "none");
            }
        }

        protected void btnAggSai_Click(object sender, EventArgs e)
        {
            datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            PresenterInvioCalcolo InvioCalcolo = new PresenterInvioCalcolo();
            InvioCalcolo.AggiornaSai(this);

            setReturnData();

            this.divIntro.Style.Add("display", "none");
            this.divResult.Style.Add("display", "block");
        }

        private void setReturnData()
        {
            if (this.areaEsito != null)
            {
                this.lblDettaglio.Text = this.areaEsito.Messaggio != null ? this.areaEsito.Messaggio.ToUpperInvariant() : "";
                this.lblEsito.Text = this.areaEsito.RisultatoOperazione.ToString().ToUpperInvariant();
            }

            this.lblStato.Text = this.statoPensione != null ? this.statoPensione.ToUpperInvariant() : "";

            if (this.areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK && this.statoPensione.ToUpperInvariant() == "CALCOLATA")
            {
                btnAggSai.Enabled = false;

                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                Domanda.Stato = this.statoPensione.ToUpperInvariant();
                Session["Domanda"] = Domanda;
            }
        }
    }
}
