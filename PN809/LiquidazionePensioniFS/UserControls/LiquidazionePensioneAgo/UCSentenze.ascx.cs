using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCSentenze : CustomBaseUserControl, ILiquidazionePensioneAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaLiquidazionePensione areaLiquidazionePensioneAgo)
        {
            if (areaLiquidazionePensioneAgo != null && areaLiquidazionePensioneAgo.DatiSentenze != null)
            {
                ViewState[EnumViewState.DatiSentenze.ToString()] = areaLiquidazionePensioneAgo.DatiSentenze;

                if (areaLiquidazionePensioneAgo.DatiSentenze.lDatiSentenze != null && areaLiquidazionePensioneAgo.DatiSentenze.lDatiSentenze.Count() > 0)
                {
                    gvSentenze.DataSource = areaLiquidazionePensioneAgo.DatiSentenze.lDatiSentenze;
                    gvSentenze.DataBind();
                }

                lblSentenza49593Value.Text = areaLiquidazionePensioneAgo.DatiSentenze.IsSentenza49593.HasValue ? areaLiquidazionePensioneAgo.DatiSentenze.IsSentenza49593.Value ? "SI" : "NO" : string.Empty;
                lblSentenza2401994Value.Text = areaLiquidazionePensioneAgo.DatiSentenze.IsSentenza2401994.HasValue ? areaLiquidazionePensioneAgo.DatiSentenze.IsSentenza2401994.Value ? "SI" : "NO" : string.Empty;
                lblSentenze49593_2401994Value.Text = areaLiquidazionePensioneAgo.DatiSentenze.IsSentenze49593_2401994.HasValue ? areaLiquidazionePensioneAgo.DatiSentenze.IsSentenze49593_2401994.Value ? "SI" : "NO" : string.Empty;
            }
        }

        internal DatiSentenze RecuperaCampi()
        {
            DatiSentenze datiSentenze = (DatiSentenze)ViewState[EnumViewState.DatiSentenze.ToString()];

            return datiSentenze;
        }

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        protected void btnSalvaSentenze_Click(object sender, EventArgs e)
        {
            PresenterLiquidazionePensione presenterLiquidazionePensione = new PresenterLiquidazionePensione();
            this.areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaLiquidazionePensioneAgo.DatiSentenze = RecuperaCampi();
            presenterLiquidazionePensione.SalvaDatiTabSentenze(this);
            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaSentenze_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaLiquidazionePensioneAgo == null)
                this.areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();

            PresenterLiquidazionePensione presenter = new PresenterLiquidazionePensione();
            presenter.EliminaDatiTabSentenze(this);

            if (this.HasError)
                RaiseShowAvvisoElimina(this, null);
            else
            {
                RaiseShowAvvisoElimina(this, null);
                ValorizzaEtichette(this.areaLiquidazionePensioneAgo);
            }
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            if (ShowAvvisoElimina != null)
                ShowAvvisoElimina(sender, e);
        }
        #endregion Events

        #region enum
        private enum EnumViewState
        {
            DatiSentenze
        }
        #endregion enum 
    }
}