using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class StampaAggiornamento : BasePage, IAggiornamento
    {
        #region IAggiornamento Members
        public Presenter.SvrLiquidazione.UtilityTipoAppartenenza? TipoApp { get; set; }
        public AreaAggiornamento areaAggiornamento { get; set; }
        #endregion IAggiornamento Members

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        AreaAggiornamento.TipoAggiornamento TipoAggiornamento { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TipoApp = null;
                if (Server.HtmlEncode(Request.QueryString["TipoApp"]) != null)
                {
                    switch (Server.HtmlEncode(Request.QueryString["TipoApp"]))
                    {
                        case "FS":
                            TipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.FS;
                            break;
                        case "AGO":
                            TipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.AGO;
                            break;
                        case "CI":
                            TipoApp = Presenter.SvrLiquidazione.UtilityTipoAppartenenza.CI;
                            break;
                    }

                    if (Server.HtmlEncode(Request.QueryString["Tipo"]) != null)
                    {
                        switch (Server.HtmlEncode(Request.QueryString["Tipo"]))
                        {
                            case "W":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.WebDom;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.WebDom;
                                break;
                            case "F":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Felpe;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.Felpe;
                                break;
                            case "O":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Oneri;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.Oneri;
                                break;
                            case "C":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Cumulo;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.Cumulo;
                                break;
                            case "S":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.SAI;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.SAI;
                                break;
                            case "I":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.INPDAP;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.INPDAP;
                                break;
                            case "T":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.Tot;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.Tot;
                                break;
                            case "N":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.NoteDiDebito;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.NoteDiDebito;
                                break;
                            case "P":
                                this.areaAggiornamento = new AreaAggiornamento();
                                this.areaAggiornamento.TipoAggiornamentoInCorso = AreaAggiornamento.TipoAggiornamento.PianiDiPagamento;
                                this.TipoAggiornamento = AreaAggiornamento.TipoAggiornamento.PianiDiPagamento;
                                break;
                        }
                    }
                }
                else
                {
                    lblErrore.Visible = true;
                    lblErrore.Text = "Tipo appartenenza non disponibile";
                }

                if (TipoApp != null)
                {
                    PresenterAggiornamento presenter = new PresenterAggiornamento();
                    presenter.CaricaPdfAggiornamento(this);
                    
                    if (this.areaAggiornamento != null)
                    {
                        string filename = string.Empty; 
                        byte[] bytes = {};

                        if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.WebDom && this.areaAggiornamento.AreaAggiornamentoWebDom != null && 
                            this.areaAggiornamento.AreaAggiornamentoWebDom.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoWebDom.PdfDoc.ToArray();
                            filename = "AggiornamentoWebDom_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.Felpe && this.areaAggiornamento.AreaAggiornamentoFelpe != null && 
                            this.areaAggiornamento.AreaAggiornamentoFelpe.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoFelpe.PdfDoc.ToArray();
                            filename = "AggiornamentoFelpe_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.Oneri && this.areaAggiornamento.AreaAggiornamentoOneri != null && 
                            this.areaAggiornamento.AreaAggiornamentoOneri.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoOneri.PdfDoc.ToArray();
                            filename = "AggiornamentoOneri_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.Cumulo && this.areaAggiornamento.AreaAggiornamentoCumulo != null &&
                            this.areaAggiornamento.AreaAggiornamentoCumulo.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoCumulo.PdfDoc.ToArray();
                            filename = "AggiornamentoCumulo_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.Tot && this.areaAggiornamento.AreaAggiornamentoTot != null &&
                            this.areaAggiornamento.AreaAggiornamentoTot.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoTot.PdfDoc.ToArray();
                            filename = "AggiornamentoTot_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.SAI && this.areaAggiornamento.AreaAggiornamentoSAI != null &&
                            this.areaAggiornamento.AreaAggiornamentoSAI.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoSAI.PdfDoc.ToArray();
                            filename = "AggiornamentoSAI_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.INPDAP && this.areaAggiornamento.AreaAggiornamentoINPDAP != null &&
                            this.areaAggiornamento.AreaAggiornamentoINPDAP.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoINPDAP.PdfDoc.ToArray();
                            filename = "AggiornamentoINPDAP_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.NoteDiDebito && this.areaAggiornamento.AreaAggiornamentoNoteDiDebito != null &&
                            this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoNoteDiDebito.PdfDoc.ToArray();
                            filename = "AggiornamentoNoteDiDebito_" + TipoApp.ToString() + ".pdf";
                        }
                        else if (this.TipoAggiornamento == AreaAggiornamento.TipoAggiornamento.PianiDiPagamento && this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento != null &&
                         this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.PdfDoc != null)
                        {
                            bytes = this.areaAggiornamento.AreaAggiornamentoPianiDiPagamento.PdfDoc.ToArray();
                            filename = "AggiornamentoPianiDiPagamento_" + TipoApp.ToString() + ".pdf";
                        }
                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
                        Response.AppendHeader("Content-Length", bytes.Length.ToString());
                        Response.AppendHeader("Accept-Ranges", "bytes");
                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        lblErrore.Visible = true;
                        lblErrore.Text = "Nessuna domanda con esito negativo.";
                    }
                }
            }
        }
    }
}
