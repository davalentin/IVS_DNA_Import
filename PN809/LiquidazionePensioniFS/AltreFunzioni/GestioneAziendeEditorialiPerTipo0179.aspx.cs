using INPS.DNA.Presenter.Interface;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class GestioneAziendeEditorialiPerTipo0179 : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// metodi degli eventi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            IViewUI iViewUI = (IViewUI)sender;

            if (iViewUI.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
            }

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = iViewUI.ErrorMessage;
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }
    }
}