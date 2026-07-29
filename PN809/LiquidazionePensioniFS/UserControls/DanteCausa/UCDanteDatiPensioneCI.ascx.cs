using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Web.UI;

using INPS.DNA.UI.Web;
using INPS.DNA.Logging;
using INPS.DNA.Exceptions;
using INPS.DNA;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;


namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa
{
    public partial class UCDanteDatiPensioneCI : CustomBaseUserControl, IDanteCausa
    {

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDanteCausa
        public long numDomanda { get; set; }
        public Presenter.SvrLiquidazione.AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaControlliPensioneCI(IDanteCausa danteCausa)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RenderControls();

            if (danteCausa.areaDanteCausa.DatiPensioneCI != null)
            {
                if (danteCausa.areaDanteCausa.DatiPensioneCI.Adeguata.HasValue)
                    this.txtPensioneAdeguata.Text = Convert.ToString(decimal.Round(danteCausa.areaDanteCausa.DatiPensioneCI.Adeguata.Value, 2));
                if (danteCausa.areaDanteCausa.DatiPensioneCI.Articolo6140.HasValue)
                    this.lblArt6.InnerText = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.Articolo6140.Value);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.DecorrenzaMaggiorazioneArt6.HasValue)
                    this.lblDecorrenzaArt6.InnerText = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.DatiPensioneCI.DecorrenzaMaggiorazioneArt6.Value);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.Articolo1Legge5991.HasValue)
                    this.ddlArt1L5991.Items.FindByValue(danteCausa.areaDanteCausa.DatiPensioneCI.Articolo1Legge5991.Value.ToString()).Selected = true;
                if (danteCausa.areaDanteCausa.DatiPensioneCI.CodiceTipoPerequazione.HasValue && this.ddlTipoPerequazione.Items.FindByValue(danteCausa.areaDanteCausa.DatiPensioneCI.CodiceTipoPerequazione.Value.ToString())!= null)
                    this.ddlTipoPerequazione.Items.FindByValue(danteCausa.areaDanteCausa.DatiPensioneCI.CodiceTipoPerequazione.Value.ToString()).Selected = true;
                if (danteCausa.areaDanteCausa.DatiPensioneCI.DecorrenzaOriginariaPrima.HasValue)
                    this.txtDatiPensioneDCal.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.DatiPensioneCI.DecorrenzaOriginariaPrima.Value);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.Aumento7290.HasValue)
                    this.txtAumentoSentenza7290.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.Aumento7290);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.Aumento7290DC.HasValue)
                    this.txtAumentoSentenza7290Art2.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.Aumento7290DC);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.AumentoMensileLegge161289Art2.HasValue)
                    this.txtAumentoTotaleArt2DPCM.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.AumentoMensileLegge161289Art2.Value);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.MensileLegge5991.HasValue)
                {
                    this.txtMensile.Text = Convert.ToString(decimal.Round(danteCausa.areaDanteCausa.DatiPensioneCI.MensileLegge5991.Value, 2));
                    this.htxtMensile.Value = this.txtMensile.Text;
                }
                if (danteCausa.areaDanteCausa.DatiPensioneCI.AumentoMensileLegge5991Comma2.HasValue)
                {
                    this.txtTotale90.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.AumentoMensileLegge5991Comma2.Value);
                    this.htxtTotale90.Value = this.txtTotale90.Text;
                }
                if (danteCausa.areaDanteCausa.DatiPensioneCI.AumentoMensileLegge5991Comma9.HasValue)
                {
                    this.txtTotale9294.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.AumentoMensileLegge5991Comma9.Value);
                    this.htxtTotale9294.Value = this.txtTotale9294.Text;
                }
                if (danteCausa.areaDanteCausa.DatiPensioneCI.TotaleArticolo345Legge140.HasValue)
                    this.txtTotale.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.TotaleArticolo345Legge140.Value);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.TotaleQuoteFisse.HasValue)
                    this.txtTotaleQuoteFisse.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneCI.TotaleQuoteFisse.Value);
                if (danteCausa.areaDanteCausa.DatiPensioneCI.VirtualeIntegrata.HasValue)
                    this.txtPensioneVirtualeIntegrata.Text = Convert.ToString(decimal.Round(danteCausa.areaDanteCausa.DatiPensioneCI.VirtualeIntegrata.Value, 4));
                if (danteCausa.areaDanteCausa.DatiPensioneCI.VirtualePura.HasValue)
                    this.txtPensioneVirtualePura.Text = Convert.ToString(decimal.Round(danteCausa.areaDanteCausa.DatiPensioneCI.VirtualePura.Value, 4));
                if (danteCausa.areaDanteCausa.DatiPensioneCI.lDatiPensioniEstereDc != null && danteCausa.areaDanteCausa.DatiPensioneCI.lDatiPensioniEstereDc.Count() > 0)
                {
                    foreach (DatiPensioneCI.DatiPensioniEstereDc datiPensioniEstereDc in danteCausa.areaDanteCausa.DatiPensioneCI.lDatiPensioniEstereDc)
                    {
                        if (datiPensioniEstereDc.CodiciVari.HasValue && (datiPensioniEstereDc.CodiciVari.Value == 3 || datiPensioniEstereDc.CodiciVari.Value == 4 || datiPensioniEstereDc.CodiciVari.Value == 5 || datiPensioniEstereDc.CodiciVari.Value == 8))
                        {
                            this.ddlArticolo.Items.FindByValue(datiPensioniEstereDc.CodiciVari.Value.ToString()).Selected = true;


                            if (datiPensioniEstereDc.Importo.HasValue)
                                this.txtImporto.Text = Convert.ToString(datiPensioniEstereDc.Importo.Value);
                        }
                        else
                        {
                            if (datiPensioniEstereDc.CodiciVari.HasValue && datiPensioniEstereDc.CodiciVari.Value == 6 && datiPensioniEstereDc.Importo.HasValue)
                                this.txtImportoDecorr.Text = Convert.ToString(datiPensioniEstereDc.Importo.Value);

                            if (datiPensioniEstereDc.CodiciVari.HasValue && datiPensioniEstereDc.CodiciVari.Value == 10 && datiPensioniEstereDc.Importo.HasValue)
                                this.txtTotaleSupplementi.Text = Convert.ToString(datiPensioniEstereDc.Importo.Value);
                        }
                    }
                }

                if (danteCausa.areaDanteCausa.DatiPensioneCI.Articolo1Legge5991.HasValue && danteCausa.areaDanteCausa.DatiPensioneCI.Articolo1Legge5991.Value)
                {
                    this.txtTotale90.CssClass = "tb8 txtUppercase";
                    this.txtTotale90.Enabled = true;
                    this.txtMensile.CssClass = "tb8 txtUppercase";
                    this.txtMensile.Enabled = true;
                    this.txtTotale9294.CssClass = "tb8 txtUppercase";
                    this.txtTotale9294.Enabled = true;
                }
                else
                {
                    this.txtTotale90.CssClass = "tboxdisable";
                    this.txtTotale90.Enabled = false;
                    this.txtMensile.CssClass = "tboxdisable";
                    this.txtMensile.Enabled = false;
                    this.txtTotale9294.CssClass = "tboxdisable";
                    this.txtTotale9294.Enabled = false;
                }

                if (danteCausa.areaDanteCausa.DatiPensioneCI.ImportoPagamentoDataMorte49593.HasValue && danteCausa.areaDanteCausa.DatiPensioneCI.ImportoPagamentoDataMorte49593 != null)
                {
                    this.txtImportoPagamentoDataMorte49593.Text = Convert.ToString(decimal.Round(danteCausa.areaDanteCausa.DatiPensioneCI.ImportoPagamentoDataMorte49593.Value, 2));
                }
            }
            else
                danteCausa.areaDanteCausa.DatiPensioneCI = new DatiPensioneCI();
            ViewState["AreaDC"] = danteCausa.areaDanteCausa;
        }

        internal DatiPensioneCI GetValoriPensioneCI()
        {
            AreaDanteCausa areaDC = (AreaDanteCausa)ViewState["AreaDC"];

            decimal? dNull = null;
            byte? bNull = null;
            DateTime? dateNull = null;
            bool? boolNull = null;
            if (areaDC != null && areaDC.DatiPensioneCI != null)
            {
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc = new DatiPensioneCI.DatiPensioniEstereDc[3];

                areaDC.DatiPensioneCI.Adeguata = String.IsNullOrEmpty(this.txtPensioneAdeguata.Text) ? dNull : Convert.ToDecimal(this.txtPensioneAdeguata.Text);
                areaDC.DatiPensioneCI.Articolo6140 = String.IsNullOrEmpty(this.lblArt6.InnerText) ? bNull : Convert.ToByte(this.lblArt6.InnerText);
                areaDC.DatiPensioneCI.DecorrenzaMaggiorazioneArt6 = String.IsNullOrEmpty(this.lblDecorrenzaArt6.InnerText) ? dateNull : Utility.GetDateFromString(this.lblDecorrenzaArt6.InnerText);
                areaDC.DatiPensioneCI.Articolo1Legge5991 = String.IsNullOrEmpty(this.ddlArt1L5991.SelectedValue) ? boolNull : Convert.ToBoolean(this.ddlArt1L5991.SelectedValue);
                areaDC.DatiPensioneCI.CodiceTipoPerequazione = String.IsNullOrEmpty(this.ddlTipoPerequazione.SelectedValue) ? bNull : Convert.ToByte(this.ddlTipoPerequazione.SelectedValue);

                if (!String.IsNullOrEmpty(this.txtDatiPensioneDCal.Text) && !this.txtDatiPensioneDCal.Text.ToUpperInvariant().Equals("MM/AAAA"))
                {
                    try
                    {
                        areaDC.DatiPensioneCI.DecorrenzaOriginariaPrima = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDatiPensioneDCal.Text)));
                    }
                    catch (Exception)
                    {
                        areaDC.DatiPensioneCI.DecorrenzaOriginariaPrima = null;
                    }
                }
                else
                    areaDC.DatiPensioneCI.DecorrenzaOriginariaPrima = null;

                areaDC.DatiPensioneCI.Aumento7290 = String.IsNullOrEmpty(this.txtAumentoSentenza7290.Text) ? dNull : Convert.ToDecimal(this.txtAumentoSentenza7290.Text);
                areaDC.DatiPensioneCI.Aumento7290DC = String.IsNullOrEmpty(this.txtAumentoSentenza7290Art2.Text) ? dNull : Convert.ToDecimal(this.txtAumentoSentenza7290Art2.Text);
                areaDC.DatiPensioneCI.AumentoMensileLegge161289Art2 = String.IsNullOrEmpty(this.txtAumentoTotaleArt2DPCM.Text) ? dNull : Convert.ToDecimal(this.txtAumentoTotaleArt2DPCM.Text);
                areaDC.DatiPensioneCI.MensileLegge5991 = String.IsNullOrEmpty(this.txtMensile.Text) ? dNull : Convert.ToDecimal(this.txtMensile.Text);
                areaDC.DatiPensioneCI.AumentoMensileLegge5991Comma2 = String.IsNullOrEmpty(this.txtTotale90.Text) ? dNull : Convert.ToDecimal(this.txtTotale90.Text);
                areaDC.DatiPensioneCI.AumentoMensileLegge5991Comma9 = String.IsNullOrEmpty(this.txtTotale9294.Text) ? dNull : Convert.ToDecimal(this.txtTotale9294.Text);
                areaDC.DatiPensioneCI.TotaleArticolo345Legge140 = String.IsNullOrEmpty(this.txtTotale.Text) ? dNull : Convert.ToDecimal(this.txtTotale.Text);
                areaDC.DatiPensioneCI.TotaleQuoteFisse = String.IsNullOrEmpty(this.txtTotaleQuoteFisse.Text) ? dNull : Convert.ToDecimal(this.txtTotaleQuoteFisse.Text);
                areaDC.DatiPensioneCI.VirtualeIntegrata = String.IsNullOrEmpty(this.txtPensioneVirtualeIntegrata.Text) ? dNull : Convert.ToDecimal(this.txtPensioneVirtualeIntegrata.Text);
                areaDC.DatiPensioneCI.VirtualePura = String.IsNullOrEmpty(this.txtPensioneVirtualePura.Text) ? dNull : Convert.ToDecimal(this.txtPensioneVirtualePura.Text);
                //PRIMO RECORD valorizzato con: codice = 10 e l'importo = il valore del campo Totale Supplementi
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[0] = new DatiPensioneCI.DatiPensioniEstereDc();
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[0].CodiciVari = Convert.ToByte("10");
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[0].Importo = String.IsNullOrEmpty(this.txtTotaleSupplementi.Text) ? dNull : Convert.ToDecimal(this.txtTotaleSupplementi.Text);
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[0].IdDanteCausa = areaDC.Id;
                //SECONDO RECORD valorizzato con: il codice = 6 e l'importo = il valore del campo Importo Art. 6
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[1] = new DatiPensioneCI.DatiPensioniEstereDc();
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[1].CodiciVari = Convert.ToByte("6");
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[1].Importo = String.IsNullOrEmpty(this.txtImportoDecorr.Text) ? dNull : Convert.ToDecimal(this.txtImportoDecorr.Text);
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[1].IdDanteCausa = areaDC.Id;
                //TERZO RECORD valorizzato con: il codice = il valore del campo Articolo e l'importo = il valore del campo Importo
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[2] = new DatiPensioneCI.DatiPensioniEstereDc();
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[2].CodiciVari = String.IsNullOrEmpty(this.ddlArticolo.SelectedValue) ? bNull : Convert.ToByte(this.ddlArticolo.SelectedValue);
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[2].Importo = String.IsNullOrEmpty(this.txtImporto.Text) ? dNull : Convert.ToDecimal(this.txtImporto.Text);
                areaDC.DatiPensioneCI.lDatiPensioniEstereDc[2].IdDanteCausa = areaDC.Id;
                areaDC.DatiPensioneCI.ImportoPagamentoDataMorte49593 = String.IsNullOrEmpty(this.txtImportoPagamentoDataMorte49593.Text) ? dNull : Convert.ToDecimal(this.txtImportoPagamentoDataMorte49593.Text);
            }
            else
                areaDC = new AreaDanteCausa();

            return areaDC.DatiPensioneCI;
        }

        private void RenderControls()
        {
            LoadDdlArt1L5991();
            LoadDdlPerequazione();
            LoadArt3458();
        }

        private void LoadDdlArt1L5991()
        {
            this.ddlArt1L5991.Items.Add(new ListItem(string.Empty, string.Empty));
            this.ddlArt1L5991.Items.Add(new ListItem("SI", "True"));
            this.ddlArt1L5991.Items.Add(new ListItem("NO", "False"));
        }

        private void LoadDdlPerequazione()
        {
            this.ddlTipoPerequazione.Items.Add(new ListItem(string.Empty, string.Empty));
            this.ddlTipoPerequazione.Items.Add(new ListItem("= T.M", "1"));
            this.ddlTipoPerequazione.Items.Add(new ListItem("< T.M", "5"));
            this.ddlTipoPerequazione.Items.Add(new ListItem("> T.M", "6"));
        }

        private void LoadArt3458()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            AreaDecodifica.DatiCodiciVari[] listaCodiciVari = valoriDecodificati.ElencoCodiciVari;

            this.ddlArticolo.Items.Add(new ListItem(string.Empty, string.Empty));
            foreach (AreaDecodifica.DatiCodiciVari datiCodiciVari in listaCodiciVari)
            {
                if (datiCodiciVari.Id == "3" || datiCodiciVari.Id == "4" || datiCodiciVari.Id == "5" || datiCodiciVari.Id == "8")
                    this.ddlArticolo.Items.Add(new ListItem(datiCodiciVari.Descrizione, datiCodiciVari.Id));
            }
        }

        protected void btSalvaPensioneCI_Click(object sender, EventArgs e)
        {
            areaDanteCausa = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaDanteCausa();
            areaDanteCausa.DatiPensioneCI = new DatiPensioneCI();
            areaDanteCausa.DatiPensioneCI = GetValoriPensioneCI();

            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.SalvaDatiPensioneCI(this);
            RaiseShowAvviso(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;
    }
}