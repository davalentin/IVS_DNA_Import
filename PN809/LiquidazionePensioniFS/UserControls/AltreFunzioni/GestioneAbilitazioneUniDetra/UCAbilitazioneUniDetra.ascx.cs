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
    public partial class UCAbilitazioneUniDetra : CustomBaseUserControl, IUniDetra
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IUniDetra
        public AreaUniDetra areaUniDetra { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PresenterAbilitazioneUniDetra presenter = new PresenterAbilitazioneUniDetra();
                presenter.GetAbilitazioneUniDetra(this);

                ValorizzaEtichette(this);
            }
        }

        protected void btnApplica_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            ApplicaUniDetra();
        }

        private void ValorizzaEtichette(IUniDetra iUniDetra)
        {
            if (iUniDetra.areaUniDetra != null && iUniDetra.areaUniDetra.IsNewUniDetraAttivo)
                ddlAbilitazioneUniDetra.SelectedValue = "SI";
            else
                ddlAbilitazioneUniDetra.SelectedValue = "NO";
        }

        private void ApplicaUniDetra()
        {
            PresenterAbilitazioneUniDetra presenter = new PresenterAbilitazioneUniDetra();
            presenter.SetAbilitazioneUniDetra(this);
            if (!this.HasError)
                this.ErrorMessage = "Abilitazione del servizio eseguita correttamente";

            RaiseShowAvviso(this, null);
        }

        //preleva contenuto ddl e lo mette nell interfaccia (this)
        private void RecuperaCampi()
        {
            this.areaUniDetra = new AreaUniDetra();

            if (!string.IsNullOrEmpty(ddlAbilitazioneUniDetra.SelectedItem.Value))
                if (ddlAbilitazioneUniDetra.SelectedItem.Value == "SI")
                    this.areaUniDetra.IsNewUniDetraAttivo = true;
                else
                    this.areaUniDetra.IsNewUniDetraAttivo = false;
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