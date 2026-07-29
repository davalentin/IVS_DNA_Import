using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class CambioDataINDCOM : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            UserControls.AltreFunzioni.CambioDataINDCOM.UCCambioDataINDCOM tabCambioDataSistema = (UserControls.AltreFunzioni.CambioDataINDCOM.UCCambioDataINDCOM)sender;

            ucAvviso.Tipo = tabCambioDataSistema.tipoAvviso;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = tabCambioDataSistema.ErrorMessage;
        }
    }
}