using INPS.DNA.Presenter.Interface;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class GestioneAziendeESOAMB : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            IViewUI tabAzESOAMB = (IViewUI)sender;

            if (tabAzESOAMB.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
            }

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = tabAzESOAMB.ErrorMessage;
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }
    }
}