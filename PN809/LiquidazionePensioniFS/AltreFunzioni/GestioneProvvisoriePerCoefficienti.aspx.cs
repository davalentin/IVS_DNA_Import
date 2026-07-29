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

    public partial class GestioneProvvisoriePerCoefficienti : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// evento generico per la visualizzazione dei messaggi di avviso (interfaccia IViewUI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            IViewUI interfaccia = (IViewUI)sender;

            if (interfaccia.HasError)
                ucAvviso.Tipo = TipoAvviso.Warning;
            else
                ucAvviso.Tipo = TipoAvviso.Ok;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = interfaccia.ErrorMessage;
        }
    }
}