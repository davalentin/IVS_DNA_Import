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

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.CambioDataSistema
{
    public partial class UCCambioDataSistema : CustomBaseUserControl, IControlliDinamici
    {
        #region IControlliDinamici
        public DateTime? DataSistema { get; set; }
        public DateTime? DataINDCOM { get; set; }
        #endregion IControlliDinamici

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        UtilityTipoAppartenenza tipoAppartenenza { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            tipoAppartenenza = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

            if (!IsPostBack)
            {
                GetDataSistema(tipoAppartenenza, this);

                ValorizzaEtichette();
            }
        }

        protected void btnApplica_Click(Object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDataSistema.Text))
                DataSistema = Utility.GetDateFromString(txtDataSistema.Text);
            else
                DataSistema = null;

            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            AreaEsito esito = presenter.SetDataSistema(tipoAppartenenza, DataSistema);
            SetDataSistema(tipoAppartenenza, this);

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                ErrorMessage = esito.Messaggio;
                HasError = true;
            }
            else
            {
                ErrorMessage = "Data modificata correttamente";
                HasError = false;
            }

            ValorizzaEtichette();

            RaiseShowAvviso(this, null);
        }

        protected void btnRipristina_Click(Object sender, EventArgs e)
        {
            DataSistema = null;

            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            AreaEsito esito = presenter.SetDataSistema(tipoAppartenenza, DataSistema);
            SetDataSistema(tipoAppartenenza, this);

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                ErrorMessage = esito.Messaggio;
                HasError = true;
            }
            else
            {
                ErrorMessage = "Data ripristinata correttamente";
                HasError = false;
            }

            ValorizzaEtichette();

            RaiseShowAvviso(this, null);
        }

        protected void ValorizzaEtichette()
        {
            if (this.DataSistema.HasValue)
                txtDataSistema.Text = string.Format("{0:dd/MM/yyyy}", this.DataSistema.Value);
            else
                txtDataSistema.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;
    }
}