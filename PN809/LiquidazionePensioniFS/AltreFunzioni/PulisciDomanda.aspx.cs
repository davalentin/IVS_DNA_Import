using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class PulisciDomanda : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            UserControls.AltreFunzioni.PulisciDomanda.UCPulisciDomanda ucPulisciDomanda = (UserControls.AltreFunzioni.PulisciDomanda.UCPulisciDomanda)sender;
           
            if (ucPulisciDomanda.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucPulisciDomanda.ErrorMessage;
            }
            else
            {
                ucAvviso.Messaggio = "Pulizia eseguita correttamente";
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
            }
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }

        protected void event_ReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede();
        }
    }
}