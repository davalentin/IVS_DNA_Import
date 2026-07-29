using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAbilitazioneServizi
{
    public partial class UCAbilitazioneUffPagatoriNew : CustomBaseUserControl, IAbilitazioneServizi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IAbilitazioneServizi
        public AreaAbilitazioneServizi areaAbilitazioneServizi { get; set; }
        #endregion IAbilitazioneServizi

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PresenterAbilitazioneServizi presenter = new PresenterAbilitazioneServizi();
                presenter.GetAbilitazioneServizi(this);

                ValorizzaEtichette(this);
            }
        }

        protected void btnApplica_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            ApplicaAbilitazione();
        }

        private void ValorizzaEtichette(IAbilitazioneServizi iAbilitazioneServizi)
        {
            if (iAbilitazioneServizi.areaAbilitazioneServizi != null && iAbilitazioneServizi.areaAbilitazioneServizi.IsServizioUffPagatoriNewAttivo)
                ddlAbilitazioneUffPagatori.SelectedValue = "SI";
            else
                ddlAbilitazioneUffPagatori.SelectedValue = "NO";
        }

        private void ApplicaAbilitazione()
        {
            PresenterAbilitazioneServizi presenter = new PresenterAbilitazioneServizi();
            presenter.SetAbilitazioneUffPagatori(this);
            if (!this.HasError)
                this.ErrorMessage = "Abilitazione del servizio eseguita correttamente";

            RaiseShowAvviso(this, null);
        }

        //preleva contenuto ddl e lo mette nell interfaccia (this)
        private void RecuperaCampi()
        {
            this.areaAbilitazioneServizi = new AreaAbilitazioneServizi();

            if (!string.IsNullOrEmpty(ddlAbilitazioneUffPagatori.SelectedItem.Value))
                if (ddlAbilitazioneUffPagatori.SelectedItem.Value == "SI")
                    this.areaAbilitazioneServizi.IsServizioUffPagatoriNewAttivo = true;
                else
                    this.areaAbilitazioneServizi.IsServizioUffPagatoriNewAttivo = false;
        }

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

        public event EventHandler ShowAvviso;

        public event EventHandler ShowInfo;

        public event EventHandler HideInfo;

        #endregion Events
    }
}