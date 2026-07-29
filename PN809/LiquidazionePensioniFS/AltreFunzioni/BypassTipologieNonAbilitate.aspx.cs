using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class BypassTipologieNonAbilitate : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            UserControls.AltreFunzioni.BypassTipologieNonAbilitate.UCBypassTipologieNonAbilitate tabTipologieNonAbilitate = (UserControls.AltreFunzioni.BypassTipologieNonAbilitate.UCBypassTipologieNonAbilitate)sender;

            if (tabTipologieNonAbilitate.HasError)
                ucAvviso.Tipo = TipoAvviso.Warning;
            else
                ucAvviso.Tipo = TipoAvviso.Ok;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = tabTipologieNonAbilitate.ErrorMessage;
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }
    }
}