using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAbilitazioneUniDetra
{
    public partial class UCAbilitazioneUniDetra : CustomBaseUserControl, IAbilitazioneServizi
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
            ApplicaUniDetra();
        }

        private void ValorizzaEtichette(IAbilitazioneServizi iUniDetra)
        {
            if (iUniDetra.areaAbilitazioneServizi != null && iUniDetra.areaAbilitazioneServizi.IsNewUniDetraAttivo)
                ddlAbilitazioneUniDetra.SelectedValue = "SI";
            else
                ddlAbilitazioneUniDetra.SelectedValue = "NO";
        }

        private void ApplicaUniDetra()
        {
            PresenterAbilitazioneServizi presenter = new PresenterAbilitazioneServizi();
            presenter.SetAbilitazioneUniDetra(this);
            if (!this.HasError)
                this.ErrorMessage = "Abilitazione del servizio eseguita correttamente";

            RaiseShowAvviso(this, null);
        }

        //preleva contenuto ddl e lo mette nell interfaccia (this)
        private void RecuperaCampi()
        {
            this.areaAbilitazioneServizi = new AreaAbilitazioneServizi();

            if (!string.IsNullOrEmpty(ddlAbilitazioneUniDetra.SelectedItem.Value))
                if (ddlAbilitazioneUniDetra.SelectedItem.Value == "SI")
                    this.areaAbilitazioneServizi.IsNewUniDetraAttivo = true;
                else
                    this.areaAbilitazioneServizi.IsNewUniDetraAttivo = false;
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