using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.ProvvisoriePerCoefficienti
{
    public partial class UCProvvisoriePerCoefficienti : CustomBaseUserControl, IDecorrenzaProvvisoriaCoefficienti
    {
        #region IDecorrenzaProvvisoriaCoefficienti
        public DateTime? DataDecorrenzaProvvisoriaObbligatoria { get; set; }
        public UtilityTipoAppartenenza? TipoAppartenenza { get; set; }
        #endregion IDecorrenzaProvvisoriaCoefficienti

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            TipoAppartenenza = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            if (!IsPostBack)
            {
                PresenterProvvisorieCoefficienti presenter = new PresenterProvvisorieCoefficienti();
                presenter.GetDataDecorrenzaProvvisoriaObbligatoria(this);

                if (!this.HasError)
                {
                    ValorizzaEtichette();
                }
                else
                    RaiseShowAvviso(this, null);
            }
        }

        protected void btnApplica_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDecorrenzaProvvisoriaObbligatoria.Text))
                DataDecorrenzaProvvisoriaObbligatoria = Utility.GetDateFromString(txtDecorrenzaProvvisoriaObbligatoria.Text);
            else
                DataDecorrenzaProvvisoriaObbligatoria = null;

            PresenterProvvisorieCoefficienti presenter = new PresenterProvvisorieCoefficienti();
            presenter.SetDataDecorrenzaProvvisoriaObbligatoria(this);

            if (!this.HasError)
            {
                ErrorMessage = "Data modificata correttamente";
                ValorizzaEtichette();
            }
            RaiseShowAvviso(this, null);
        }

        private void ValorizzaEtichette()
        {
            if (DataDecorrenzaProvvisoriaObbligatoria.HasValue)
                txtDecorrenzaProvvisoriaObbligatoria.Text = string.Format("{0:MM/yyyy}",DataDecorrenzaProvvisoriaObbligatoria.Value);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;
    }
}