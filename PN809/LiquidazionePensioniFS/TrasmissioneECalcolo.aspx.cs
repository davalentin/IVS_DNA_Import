using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class TrasmissioneECalcolo : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            radioInvioPosizione.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioInvioPosizione.InputAttributes.Add("EnableClass", "onClassInvioPosizione");
           

            radioRicercaPosizioneDaTrasmettere.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioRicercaPosizioneDaTrasmettere.InputAttributes.Add("EnableClass", "onClassRicercaPosizione");

            divTxtInvioPosizione.Attributes.Add("onclick", "javascript:SetRadio(this)");
            divTxtInvioPosizione.Attributes.Add("EnableClass", "onClassInvioPosizione");

            divTxtRicercaPosizione.Attributes.Add("onclick", "javascript:SetRadio(this)");
            divTxtRicercaPosizione.Attributes.Add("EnableClass", "onClassRicercaPosizione");

            btnInvioPensione.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arrow-right.gif";
            imgAggiungiCriterio.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/add24.png";
            btnRicerca.Text = "Ricerca";
        }

        protected void btnRicerca_Click(object sender, EventArgs e)
        {



        }
    }


}
