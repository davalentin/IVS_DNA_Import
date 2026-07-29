using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class GestioneAziendeESPA : CustomBasePage
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
            IViewUI tabAziendeESPA = (IViewUI)sender;
            ucAvviso.Tipo = tabAziendeESPA.HasError? TipoAvviso.Warning: TipoAvviso.Ok;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = tabAziendeESPA.ErrorMessage;
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }
    }
}