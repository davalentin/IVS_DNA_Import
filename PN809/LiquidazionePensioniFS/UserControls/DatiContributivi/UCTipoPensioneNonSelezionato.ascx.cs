using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCTipoPensioneNonSelezionato : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            imgInfoTipoPensioneNonSelezionata.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/info.png";

            if (!Page.IsPostBack) { 
            
            
            
            }
        }


        internal void ValorizzaEtichetteTipoPensioneNonSelezionato()
        {
            try
            {
                //lblSettimane=
                //lblMontante=
                //lblImportoContributivoTotale=
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributivi, Errore nel metodo ValorizzaEtichetteDatiContributivi " + ex);
            }

            return;
        }



    }
}