using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCIntestazione : CustomBaseUserControl//, IIntestazione
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ucChangeSede.ReloadControl();
            this.ucChangeRuolo.ReloadControl();
        }

    }
}