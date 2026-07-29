using System;
using System.IO;
using System.Web.UI;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class Stampa : Page, IStampa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IStampa
        public AreaTitolare.DatiPensione datiPensione { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaEsito areaEsito { get; set; }
        public MemoryStream msPDF { get; set; }
        #endregion IStampa


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                datiPensione = new AreaTitolare.DatiPensione();

                if (Server.HtmlEncode(Request.QueryString["NumDomanda"]) != null)
                {
                    datiPensione.NDomus = long.Parse(Server.HtmlEncode(Request.QueryString["NumDomanda"]));
                    if (!string.IsNullOrEmpty(Server.HtmlEncode(Request.QueryString["ProgStorico"])))
                        datiPensione.ProgStorico = byte.Parse(Server.HtmlEncode(Request.QueryString["ProgStorico"]));
                }
                else
                    datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

                if (datiPensione != null)
                {
                    PresenterStampa stampa = new PresenterStampa();
                    stampa.GetStampaDomanda(this);
                }
                else
                {
                    domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    PresenterStampa stampa = new PresenterStampa();
                    stampa.GetStampaDomandaByChiavePensione(this);
                }

                if (this.areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    this.lblErrore.Visible = true;
                    this.lblErrore.Text = this.areaEsito.Messaggio != null ? this.areaEsito.Messaggio.ToUpperInvariant() : string.Empty;
                    return;
                }

                if (this.msPDF == null)
                {
                    this.lblErrore.Visible = true;
                    this.lblErrore.Text = "La stampa del TE08 o del TE08/Ind è in corso di elaborazione. Si prega di riprovare tra qualche minuto.";
                    return;
                }
                try
                {
                    this.lblErrore.Visible = false;
                    this.lblErrore.Text = string.Empty;

                    string nDomus = datiPensione != null ? datiPensione.NDomus.ToString() : domanda != null ? domanda.NumeroDomanda : string.Empty;

                    string filename = "Stampa_" + nDomus + ".pdf";

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
                    Response.AppendHeader("Content-Length", this.msPDF.Length.ToString());
                    Response.AppendHeader("Accept-Ranges", "bytes");
                    Response.BinaryWrite(this.msPDF.ToArray());
                    Response.Flush();
                    Response.End();
                }
                catch (Exception)
                {
                    this.lblErrore.Visible = true;
                    this.lblErrore.Text = "Errore nella visualizzazione della stampa. Riprovare più tardi";
                }
            }
        }
    }
}
