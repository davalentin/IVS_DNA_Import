using INPS.DNA.Presenter.Interface;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class GestioneAziendeVOESO : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            IViewUI tabAzVOESO = (IViewUI)sender;

            if (tabAzVOESO.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
            }

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = tabAzVOESO.ErrorMessage;
        }

        protected void event_ucHideInfo(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }

        protected void event_ucChangeTipo(object sender, EventArgs e)
        {
            string tipo = ucAzVOESO.GetTipoVOESO();
            switch(tipo)
            {
                case "0033":
                    lblTitle.Text = Keys.Title_ExMonopoli;
                    break;
                case "0034":
                    lblTitle.Text = Keys.Title_RiscossioneTributiErariali;
                    break;
                case "0036":
                    lblTitle.Text = Keys.Title_FS;
                    break;
                case "0053":
                    lblTitle.Text = Keys.Title_FS_Solidaristico;
                    break;
                default:
                    lblTitle.Text = Keys.Title_Default;
                    break;
            }
        }

        public class Keys
        {
            public const string Title_Default = "Gestione Aziende VOESO";
            public const string Title_ExMonopoli = "Gestione Aziende VOESO - Dipendenti ex Monopoli";
            public const string Title_RiscossioneTributiErariali = "Gestione Aziende VOESO - Riscossione Tributi Erariali";
            public const string Title_FS = "Gestione Aziende VOESO - Ferrovie dello Stato";
            public const string Title_FS_Solidaristico = "Gestione Aziende VOESO - Ferrovie dello Stato (solidaristico)";
        }
    }
}