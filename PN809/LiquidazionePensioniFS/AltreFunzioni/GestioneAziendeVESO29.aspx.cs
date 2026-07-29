using INPS.DNA.Presenter.Interface;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class GestioneAziendeVESO29 : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            IViewUI tabAzVESO29 = (IViewUI)sender;

            if (tabAzVESO29.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
            }

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = tabAzVESO29.ErrorMessage;
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }
    }
}