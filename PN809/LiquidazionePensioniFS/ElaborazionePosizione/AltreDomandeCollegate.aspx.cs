using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class AltreDomandeCollegate : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, IAltreDomandeCollegate
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IAltreDomandeCollegate
        public AreaAltreDomandeCollegate AreaAltreDomandeCollegate { get; set; }
        public long NumeroDomandaAventeDiritto { get; set; }
        #endregion IAltreDomandeCollegate

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDati();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    //ValorizzaSemaforiTab(imgAltreDomandeCollegate, this.areaQuadri.QuadroAltreDomandeCollegate.TabAltreDomandeCollegate, pnlTabAltreDomandeCollegate);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("AltreDomandeCollegate, Errore nel metodo Page_PreRender " + ex);
            }
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            if (((IAltreDomandeCollegate)sender).HasError)
                ucAvviso.Tipo = TipoAvviso.Warning;
            else
                ucAvviso.Tipo = TipoAvviso.Ok;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = ((IAltreDomandeCollegate)sender).ErrorMessage;
        }
        #endregion protected methods

        #region private methods
        private void CaricaDati()
        {
            PresenterAltreDomandeCollegate presenter = new PresenterAltreDomandeCollegate();
            presenter.GetAltreDomandeCollegate(this);
            if (!this.HasError)
            {
                ucAltreDomandeCollegate.ValorizzaEtichette(this);
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlAltreDomandeCollegate.Enabled = false;
                return;
            }
        }

        #endregion private methods
    }
}