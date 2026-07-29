using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class PresaInCarico : CustomBasePage, IRiassegnazioneDomanda
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione
        #region IRiassegnazioneDomanda
        public AreaEsito.TipoEsito Esito { get; set; }
        public long NumeroDomanda { get; set; }
        public string StatoPensione { get; set; }
        public string VecchiaMatricola { get; set; }
        public string NuovaMatricola { get; set; }
        public UtilityRuolo? Ruolo { get; set; }
        public UtilityTipoAppartenenza? TipoAppOperatore { get; set; }
        public UtilityTipoOperazione? TipoOperazione { get; set; }
        public string SedeDiversa { get; set; }
        #endregion IRiassegnazioneDomanda

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
            Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            lblMatricolaOperatore.Text = Domanda.MatricolaUtenteAcquisizione;
            if (!IsPostBack)
            {
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }

            if (this.Domanda.TipoAutomazione != null)
            {
                lblMatricolaOperatore.Visible = false;
                lblPresaInCarico.Visible = false;
                lblAutomazione.Visible = true;
            }
        }

        protected void btnPresaInCarico_Click(object sender, EventArgs e)
        {
            GetDati();

            PresenterRiassegnazioneDomanda presenterRiassegnazioneDomanda = new PresenterRiassegnazioneDomanda();
            presenterRiassegnazioneDomanda.RiassegnaDomanda(this);

            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                return;
            }
            else
            {
                Domanda.MatricolaUtenteAcquisizione = this.VecchiaMatricola;
                Domanda.IsMatchMatricola = true;
                Session["Domanda"] = Domanda;

                Response.Redirect("PosizioneSelezionata.aspx?PresaInCarico=S", false);
            }
        }

        private void GetDati()
        {
            long numDomanda = 0;

            long.TryParse(Domanda.NumeroDomanda, out numDomanda);
            this.VecchiaMatricola = lblMatricolaOperatore.Text;
            this.NuovaMatricola = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            this.StatoPensione = Domanda.Stato;

            this.NumeroDomanda = numDomanda;
            this.TipoAppOperatore = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            this.TipoOperazione = UtilityTipoOperazione.UPDATE;


        }
    }
}
