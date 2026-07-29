using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCOpzione : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {        
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaOpzione_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiOpzione = GetDatiOpzione();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiOpzioneAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaOpzione_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiOpzioneAgo(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Opzione";
            }
            else
            {
                ValorizzaEtichetteOpzione(null);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        internal void ValorizzaEtichetteOpzione(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiOpzione != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiOpzione.DataDomandaOpzione.HasValue)
                    txtDataDomandaOpzione.Text = string.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiOpzione.DataDomandaOpzione.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiOpzione.DecorrenzaOpzione.HasValue)
                    txtDecorrenzaOpzione.Text = String.Format("{0:MM/yyyy}",liquidazioneAgo.areaLiquidazionePensioneAgo.DatiOpzione.DecorrenzaOpzione.Value);  
            }
            else
            {
                txtDataDomandaOpzione.Text = "GG/MM/AAAA";
                txtDecorrenzaOpzione.Text = "MM/AAAA";
            }


            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (datiPensione.FlagUnicarpe.HasValue)
                GestioneEtichetteIsUnicarpe(datiPensione);
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                this.txtDecorrenzaOpzione.Enabled = false;
            }
        }

        internal DatiOpzione GetDatiOpzione()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiOpzione = new DatiOpzione();

            if (string.IsNullOrEmpty(txtDataDomandaOpzione.Text) || txtDataDomandaOpzione.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiOpzione.DataDomandaOpzione = null;
            else
                areaLiquidazionePensioneAgo.DatiOpzione.DataDomandaOpzione = Utility.GetDateFromString(txtDataDomandaOpzione.Text);

            if (string.IsNullOrEmpty(txtDecorrenzaOpzione.Text) || txtDecorrenzaOpzione.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiOpzione.DecorrenzaOpzione = null;
            else
                areaLiquidazionePensioneAgo.DatiOpzione.DecorrenzaOpzione = Utility.GetDateFromString(txtDecorrenzaOpzione.Text);

            return areaLiquidazionePensioneAgo.DatiOpzione;   
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
    }
}