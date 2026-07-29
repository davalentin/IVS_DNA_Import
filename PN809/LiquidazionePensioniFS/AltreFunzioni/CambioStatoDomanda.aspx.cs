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
    public partial class CambioStatoDomanda : CustomBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            UserControls.AltreFunzioni.CambioStatoDomanda.UCCambioStatoDomanda ucCambioStatoDomanda = (UserControls.AltreFunzioni.CambioStatoDomanda.UCCambioStatoDomanda)sender;

            if (ucCambioStatoDomanda.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucCambioStatoDomanda.ErrorMessage;
            }
            else
            {
                ucAvviso.Visible = false;
                ucAvviso.Tipo = TipoAvviso.Ok;
            }
        }

        protected void event_ucShowAvvisoStatoCambiato(object sender, EventArgs args)
        {
            UserControls.AltreFunzioni.CambioStatoDomanda.UCCambioStatoDomanda ucCambioStatoDomanda = (UserControls.AltreFunzioni.CambioStatoDomanda.UCCambioStatoDomanda)sender;

            if (ucCambioStatoDomanda.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucCambioStatoDomanda.ErrorMessage;
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = "I dati della domanda sono stati modificati correttamente";
            }
        }

        protected void event_ReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede();
        }

        protected void event_HideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Tipo = TipoAvviso.Ok;
        }
    }
}