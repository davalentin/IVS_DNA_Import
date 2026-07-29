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
    public partial class GestioneAbilitazioneTrasformazioni : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            UserControls.AltreFunzioni.GestioneAbilitazioneTrasformazioni.UCGestioneAbilTrasf tabTrasformazioni = (UserControls.AltreFunzioni.GestioneAbilitazioneTrasformazioni.UCGestioneAbilTrasf)sender;

            if (tabTrasformazioni.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabTrasformazioni.ErrorMessage;
            }
        }

        protected void event_ucShowInfo(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Operazione eseguita correttamente";
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }
    }
}