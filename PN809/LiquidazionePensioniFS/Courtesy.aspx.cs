using System;
using INPS.DNA.UI.Web;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Courtesy : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session[CodeUtility.EnumSession.Courtesy_Type.ToString()] != null)
                {
                    switch ((CodeUtility.CourtesyType)Session[CodeUtility.EnumSession.Courtesy_Type.ToString()])
                    {
                        case CodeUtility.CourtesyType.SessionExpired:
                            divSessionExpired.Visible = true;
                            break;
                        case CodeUtility.CourtesyType.RuoloNonAbilitato:
                            divRuoloNonAbilitato.Visible = true;
                            break;
                    }
                    Session.Remove(CodeUtility.EnumSession.Courtesy_Type.ToString());
                }
            }
        }
    }
}
