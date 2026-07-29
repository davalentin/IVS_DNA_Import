using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RichiestaBonus
{
    public partial class UCEsitoPrenotazione : System.Web.UI.UserControl, IRichiestaBonus
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        public AreaRichiestaBonus areaRichiestaBonus { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        public void ValorizzaEsitoPrenotazione()
        {
            ViewState[EnumViewState.AreaRichiestaBonus.ToString()] = this.areaRichiestaBonus;

            gvEsitoPrenotazione_Load();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void gvEsitoPrenotazione_Load()
        {
            try
            {
                gvEsitoPrenotazione.DataSource = ((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.DatiPrenotazioneElaborazioni;
                gvEsitoPrenotazione.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRichiestaBonus, Errore nel metodo gvRichiestaBonus_Load" + ex);
            }
        }

        #region EventHandler

        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        #endregion EventHandler

        #region Enums
        public enum EnumViewState
        {
            AreaRichiestaBonus
        }
        #endregion Enums
    }
}