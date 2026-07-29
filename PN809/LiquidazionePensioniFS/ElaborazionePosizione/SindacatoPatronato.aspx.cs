using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class SindacatoPatronato : CustomBasePage, IInfoLiquidazione
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            imgSindacato.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";
            imgPatronato.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Info;
                ucAvviso.Messaggio = "Dati dimostrativi non riguardanti la domanda in lavorazione";
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        //protected void Page_PreRender(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if ((AreaQuadri)Session["Semaforo"] != null)
        //        {
        //            this.areaQuadri = (AreaQuadri)Session["Semaforo"];
        //            ValorizzaSemaforiTab(imgSindacato, this.areaQuadri., pnlTabSindacato);
        //            ValorizzaSemaforiTab(imgPatronato, this.areaQuadri., pnlTabPatronato);
        //        }
        //    }
        //    catch (DnaExceptionBase)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new INPS.DNA.DnaApplicationException("SindacatoPatronato, Errore nel metodo Page_PreRender " + ex);
        //    }
        //}



        protected void SalvaDati_Click(object sender, EventArgs e)
        {
        }

    }
}
