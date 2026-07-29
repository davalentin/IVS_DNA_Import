using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;

using INPS.DNA;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class DatiAssicurativi : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo, IDanteCausa
    {
        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        internal void ValorizzaEtichetteDatiAssicurativi(ILiquidazionePensioneAgo liquidazione, bool isDomandaInabilitaAmianto, bool isDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            Utility.Categoria? categoria = Utility.GetCategoria(this.domanda.Categoria.Trim());

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024 = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null)
                controlloDinamicoMemo123_2024 = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out controlloDinamicoMemo123_2024);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024"] = controlloDinamicoMemo123_2024;
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024OpzioneContrib = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null)
                controlloDinamicoMemo123_2024OpzioneContrib = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out controlloDinamicoMemo123_2024OpzioneContrib);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] = controlloDinamicoMemo123_2024OpzioneContrib;
            }
            
            RenderControls(datiPensione, liquidazione.areaLiquidazionePensioneAgo);
            ValorizzaEtichetteCommon(liquidazione, datiPensione);

            if (this.domanda.IsDomandaENPALS)
                ValorizzaEtichetteDatiAssicurativiENPALS(liquidazione);
            else
                ValorizzaEtichetteDatiAssicurativiAGO(liquidazione, datiPensione);

            if (liquidazione.areaLiquidazionePensioneAgo != null)
                GestioneUsuranti(liquidazione);

            ValorizzaEtichetteByCategoria(datiPensione, categoria, liquidazione);

            if (isDomandaInabilitaAmianto)
            {
                pnlAttEconomProfInd.Visible = true;
                txtAttivitaEconomica.Text = "01";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "250";
                txtProfessioneIndividuale.Enabled = false;
                hiddenFieldAttivitaEconomica.Value = "01";
                hiddenFieldProfessioneIndividuale.Value = "250";
            }
            else if (isDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto)
            {
                pnlAttEconomProfInd.Visible = true;
                txtAttivitaEconomica.Text = "15";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "208";
                txtProfessioneIndividuale.Enabled = false;
                hiddenFieldAttivitaEconomica.Value = "15";
                hiddenFieldProfessioneIndividuale.Value = "208";
            }
        }

        private void ValorizzaEtichetteByCategoria(AreaTitolare.DatiPensione datiPensione, Utility.Categoria? categoria, ILiquidazionePensioneAgo liquidazione)
        {
            switch (categoria)
            {
                case Utility.Categoria.VOCRED:
                case Utility.Categoria.CRED27:
                    {
                        pnlInizioFineUltimoLavoro.Visible = true;

                        txtAttivitaEconomica.Text = "65";
                        txtAttivitaEconomica.Enabled = false;

                        txtProfessioneIndividuale.Text = "195";
                        txtProfessioneIndividuale.Enabled = false;

                        hiddenFieldAttivitaEconomica.Value = "65";
                        hiddenFieldProfessioneIndividuale.Value = "195";
                    }
                    break;
                case Utility.Categoria.VOART:
                case Utility.Categoria.IOART:
                case Utility.Categoria.SOART:
                    {
                        if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                        {
                            txtAttivitaEconomica.Text = "57";
                            hiddenFieldAttivitaEconomica.Value = "57";
                        }
                        if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                        {
                            txtProfessioneIndividuale.Text = "175";
                            hiddenFieldProfessioneIndividuale.Value = "175";
                        }
                    }
                    break;
                case Utility.Categoria.VOAUT:
                case Utility.Categoria.SOAUT:
                case Utility.Categoria.IOAUT:
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "71";
                        hiddenFieldAttivitaEconomica.Value = "71";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "174";
                        hiddenFieldProfessioneIndividuale.Value = "174";
                    }
                    break;
                case Utility.Categoria.VESO33:
                    pnlInizioFineUltimoLavoro.Visible = true;
                    trInizioFineUltimoLavoro.Visible = false;
                    RFVtxtFineUltLav.Enabled = false;
                    RFVtxtInizioUltLav.Enabled = false;
                    txtInizioUltLav.Text = string.Empty;
                    txtFineUltLav.Text = string.Empty;
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "51";
                        hiddenFieldAttivitaEconomica.Value = "51";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "161";
                        hiddenFieldProfessioneIndividuale.Value = "161";
                    }
                    break;
                case Utility.Categoria.VOESO:
                    RFVtxtFineUltLav.Enabled = true;
                    RFVtxtInizioUltLav.Enabled = true;
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "51";
                        hiddenFieldAttivitaEconomica.Value = "51";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "161";
                        hiddenFieldProfessioneIndividuale.Value = "161";
                    }
                    break;
                case Utility.Categoria.VESO29:
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "51";
                        hiddenFieldAttivitaEconomica.Value = "51";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "161";
                        hiddenFieldProfessioneIndividuale.Value = "161";
                    }
                    break;
                case Utility.Categoria.VOCOOP:
                case Utility.Categoria.COOP28:
                    pnlInizioFineUltimoLavoro.Visible = true;
                    RFVtxtFineUltLav.Enabled = true;
                    RFVtxtInizioUltLav.Enabled = true;
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;

                    txtAttivitaEconomica.Text = "65";
                    hiddenFieldAttivitaEconomica.Value = "65";

                    txtProfessioneIndividuale.Text = "195";
                    hiddenFieldProfessioneIndividuale.Value = "195";

                    break;
                case Utility.Categoria.VESO92:
                    RFVtxtFineUltLav.Enabled = false;
                    RFVtxtInizioUltLav.Enabled = false;
                    pnlImportoUltimaRetribuzione.Visible = false;
                    RFVtxtImportaUltimaRetribuzione.Enabled = false;
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "92";
                        hiddenFieldAttivitaEconomica.Value = "92";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "012";
                        hiddenFieldProfessioneIndividuale.Value = "012";
                    }
                    break;
                case Utility.Categoria.ESOTEL:
                    txtImportaUltimaRetribuzione.Enabled = false;
                    txtImportaUltimaRetribuzione.Visible = false;
                    pnlImportoUltimaRetribuzione.Visible = false;
                    txtAttivitaEconomica.Text = "64";
                    hiddenFieldAttivitaEconomica.Value = "64";
                    txtProfessioneIndividuale.Text = "002";
                    hiddenFieldProfessioneIndividuale.Value = "002";
                    break;
                case Utility.Categoria.ESOAMB:
                    txtAttivitaEconomica.Text = "70";
                    hiddenFieldAttivitaEconomica.Value = "70";
                    txtProfessioneIndividuale.Text = "190";
                    hiddenFieldProfessioneIndividuale.Value = "190";
                    break;
                case Utility.Categoria.VDAI:
                case Utility.Categoria.IDAI:
                case Utility.Categoria.SDAI:
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "71";
                        hiddenFieldAttivitaEconomica.Value = "71";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "194";
                        hiddenFieldProfessioneIndividuale.Value = "194";
                    }
                    break;
                case Utility.Categoria.VOSPETT:
                    if (datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo == "0008")
                    {
                        if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                        {
                            txtAttivitaEconomica.Text = "01";
                            hiddenFieldAttivitaEconomica.Value = "01";
                        }
                        if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                        {
                            txtProfessioneIndividuale.Text = "383";
                            hiddenFieldProfessioneIndividuale.Value = "383";
                        }

                        txtAttivitaEconomica.Enabled = false;
                        txtProfessioneIndividuale.Enabled = false;
                        pnlAttEconomProfInd.Visible = true;

                    }
                    break;
                case Utility.Categoria.INDCOM:
                    txtAttivitaEconomica.Text = "61";
                    txtAttivitaEconomica.Enabled = false;

                    txtProfessioneIndividuale.Text = "172";
                    txtProfessioneIndividuale.Enabled = false;
                    break;
                case Utility.Categoria.SOCUM:
                case Utility.Categoria.IOCUM:
                case Utility.Categoria.VOTOT:
                case Utility.Categoria.SOTOT:
                case Utility.Categoria.IOTOT:
                    //if ((liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica.HasValue) &&
                    //    (liquidazione.areaLiquidazionePensioneAgo.DatiGenerici == null || liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa != 1))
                    //{
                    //    txtAttivitaEconomica.Text = "71";
                    //    txtAttivitaEconomica.Enabled = false;
                    //    txtProfessioneIndividuale.Enabled = false;
                    //}

                    CodeUtility areaDecodifica = new CodeUtility();
                    Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
                    Presenter.SvrLiquidazione.AreaDecodifica.DatiCtrlEnteCassaCodiceGestione[] listaCtrlEnteCassaCodiceGestione = datiDecodifica.ElencoCtrlEnteCassaCodiceGestione;
                    if (listaCtrlEnteCassaCodiceGestione != null && listaCtrlEnteCassaCodiceGestione.Length > 0)
                    {
                        ddlCtrlEnteCassaCodiceGestione.Visible = true;
                        foreach (var item in listaCtrlEnteCassaCodiceGestione.Where(x => x.CodiceCategoria == categoria.ToString()))
                            CodeUtility.SetValueDdl(ddlCtrlEnteCassaCodiceGestione, item.Professione, string.Empty, item.TraduzioneSuGP);

                        if ((liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ProfessioneIndividuale.HasValue)
                        && liquidazione.areaLiquidazionePensioneAgo.DatiGenerici != null && listaCtrlEnteCassaCodiceGestione != null && listaCtrlEnteCassaCodiceGestione.Length > 0)
                        {
                            var enteCassa = liquidazione.areaLiquidazionePensioneAgo.listaDecodificaEnteCassaProfessionale.Where(x => x.Id == liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa).Select(x => x.TraduzioneSuGP).FirstOrDefault();
                            enteCassa = !string.IsNullOrEmpty(enteCassa) ? enteCassa.ToString().PadLeft(4, '0') : enteCassa;
                            txtProfessioneIndividuale.Text = listaCtrlEnteCassaCodiceGestione.Where(x => x.CodiceCategoria == categoria.ToString() && x.TraduzioneSuGP == enteCassa).Select(x => x.Professione).FirstOrDefault();
                            if (Utility.IsDomandaTotalizzazione(categoria.ToString()) && !string.IsNullOrEmpty(txtProfessioneIndividuale.Text) &&
                                (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica.HasValue) &&
                                (liquidazione.areaLiquidazionePensioneAgo.DatiGenerici == null || liquidazione.areaLiquidazionePensioneAgo.DatiGenerici.EnteCassa != 1))
                            {
                                txtAttivitaEconomica.Text = "71";
                            }
                        }
                    }
                    break;
                case Utility.Categoria.ESPA:
                    RFVtxtFineUltLav.Enabled = false;
                    RFVtxtInizioUltLav.Enabled = false;
                    pnlImportoUltimaRetribuzione.Visible = false;
                    RFVtxtImportaUltimaRetribuzione.Enabled = false;
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    {
                        txtAttivitaEconomica.Text = "41";
                        hiddenFieldAttivitaEconomica.Value = "41";
                    }
                    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    {
                        txtProfessioneIndividuale.Text = "148";
                        hiddenFieldProfessioneIndividuale.Value = "148";
                    }
                    break;
                case Utility.Categoria.VMP:
                case Utility.Categoria.IMP:
                    pnlAttEconomProfInd.Visible = true;
                    txtAttivitaEconomica.Text = "25";
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Text = "191";
                    txtProfessioneIndividuale.Enabled = false;
                    hiddenFieldAttivitaEconomica.Value = "25";
                    hiddenFieldProfessioneIndividuale.Value = "191";
                    break;
            }
        }

        internal INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.DatiAssicurativi GetDatiAssicurativi()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.DatiAssicurativi();

            if (string.IsNullOrEmpty(txtInizioAssicurazione.Text) || txtInizioAssicurazione.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.InizioAssicurazione = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtInizioAssicurazione.Text);

            if (string.IsNullOrEmpty(txtFineAssicurazione.Text) || txtFineAssicurazione.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.FineAssicurazione = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtFineAssicurazione.Text);

            if (string.IsNullOrEmpty(txtInizioUltLav.Text) || txtInizioUltLav.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.InizioUltimoLavoro = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.InizioUltimoLavoro = Utility.GetDateFromString(txtInizioUltLav.Text);

            if (string.IsNullOrEmpty(txtFineUltLav.Text) || txtFineUltLav.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.FineUltimoLavoro = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.FineUltimoLavoro = Utility.GetDateFromString(txtFineUltLav.Text);

            if (!string.IsNullOrEmpty(txtImportaUltimaRetribuzione.Text))
                areaLiquidazionePensioneAgo.DatiAssicurativi.ImportoUltimaRetribuzione = decimal.Parse(txtImportaUltimaRetribuzione.Text);
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.ImportoUltimaRetribuzione = null;

            if (string.IsNullOrEmpty(hiddenFieldAttivitaEconomica.Value))
                areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica = int.Parse(hiddenFieldAttivitaEconomica.Value);

            if (string.IsNullOrEmpty(hiddenFieldProfessioneIndividuale.Value))
                areaLiquidazionePensioneAgo.DatiAssicurativi.ProfessioneIndividuale = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.ProfessioneIndividuale = int.Parse(hiddenFieldProfessioneIndividuale.Value);

            if (string.IsNullOrEmpty(txtNumeroSettimaneOBG.Text))
                areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOBG = null;
            else
            {
                areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOBG = int.Parse(txtNumeroSettimaneOBG.Text);
            }
            if (string.IsNullOrEmpty(txtNumeroSettimaneOI.Text))
                areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOI = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOI = int.Parse(txtNumeroSettimaneOI.Text);

            if (string.IsNullOrEmpty(txtNumContrVolontari.Text))
                areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVolontari = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVolontari = int.Parse(txtNumContrVolontari.Text);

            if (string.IsNullOrEmpty(txtNumContrVolontariAnz.Text))
                areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVVAnzianita = null;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVVAnzianita = int.Parse(txtNumContrVolontariAnz.Text);

            if (String.Equals(ddlReqVecch1294.SelectedValue, "SI"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiVecchiaiaAl1294 = true;
            else if (String.Equals(ddlReqVecch1294.SelectedValue, "NO"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiVecchiaiaAl1294 = false;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiVecchiaiaAl1294 = null;

            if (String.Equals(ddlReqAnz996.SelectedValue, "SI"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl996 = true;
            else if (String.Equals(ddlReqAnz996.SelectedValue, "NO"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl996 = false;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl996 = null;

            if (String.Equals(ddlReqAnz1294.SelectedValue, "SI"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl1294 = true;
            else if (String.Equals(ddlReqAnz1294.SelectedValue, "NO"))
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl1294 = false;
            else
                areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl1294 = null;

            if (ddlReqArt2Dl503.SelectedIndex > 0)
                areaLiquidazionePensioneAgo.DatiAssicurativi.ReqArt2DL503 = CodeUtility.StringToNullableShort(ddlReqArt2Dl503.SelectedValue);

            if (this.domanda.IsDomandaENPALS)
            {
                areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS = new Presenter.SvrLiquidazioneAgo.DatiAssicurativi.ENPALS();

                if (!string.IsNullOrEmpty(txtAADiritto.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.AADiritto = CodeUtility.StringToNullableShort(txtAADiritto.Text);

                if (!string.IsNullOrEmpty(txtMMDiritto.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.MMDiritto = CodeUtility.StringToNullableShort(txtMMDiritto.Text);

                if (!string.IsNullOrEmpty(txtEtaDirittoAA.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaDirittoAA = CodeUtility.StringToNullableShort(txtEtaDirittoAA.Text);

                if (!string.IsNullOrEmpty(txtEtaDirittoMM.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaDirittoMM = CodeUtility.StringToNullableShort(txtEtaDirittoMM.Text);

                if (!string.IsNullOrEmpty(txtNTotDiritto.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotDiritto = CodeUtility.StringToNullableInt(txtNTotDiritto.Text);

                if (!string.IsNullOrEmpty(txtAnzianitaContributiva.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.AnzianitaContributiva = CodeUtility.StringToNullableShort(txtAnzianitaContributiva.Text);

                if (!string.IsNullOrEmpty(txtGruppoPrevalente.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.GruppoPrevalente = CodeUtility.StringToNullableChar(txtGruppoPrevalente.Text);

                if (!string.IsNullOrEmpty(txtGruppoDiritto.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.GruppoDiritto = CodeUtility.StringToNullableChar(txtGruppoDiritto.Text);

                if (!string.IsNullOrEmpty(txtRaggruppamentoPrevalente.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.RaggruppamentoPrevalente = CodeUtility.StringToNullableChar(txtRaggruppamentoPrevalente.Text);

                if (!string.IsNullOrEmpty(txtQualifica.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.Qualifica = txtQualifica.Text;

                if (!string.IsNullOrEmpty(txtNTotQualifica.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotQualifica = CodeUtility.StringToNullableInt(txtNTotQualifica.Text);

                if (!string.IsNullOrEmpty(txtNTotContributi.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotContributi = CodeUtility.StringToNullableInt(txtNTotContributi.Text);

                if (!string.IsNullOrEmpty(txtNTotContributiEnpals.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotContributiEnpals = CodeUtility.StringToNullableInt(txtNTotContributiEnpals.Text);

                if (!string.IsNullOrEmpty(txtEtaMisuraAA.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaMisuraAA = CodeUtility.StringToNullableShort(txtEtaMisuraAA.Text);

                if (!string.IsNullOrEmpty(txtEtaMisuraMM.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaMisuraMM = CodeUtility.StringToNullableShort(txtEtaMisuraMM.Text);

                if (!string.IsNullOrEmpty(txtNContributiMisura.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiMisura = CodeUtility.StringToNullableShort(txtNContributiMisura.Text);

                if (!string.IsNullOrEmpty(txtNContributiTriennio.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiTriennio = CodeUtility.StringToNullableShort(txtNContributiTriennio.Text);

                if (!string.IsNullOrEmpty(txtNContributiQuinquennio.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiQuinquennio = CodeUtility.StringToNullableShort(txtNContributiQuinquennio.Text);

                if (!string.IsNullOrEmpty(txtNContributiNL155.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiNL155 = CodeUtility.StringToNullableShort(txtNContributiNL155.Text);

                if (!string.IsNullOrEmpty(txtNContributiNL222.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiNL222 = CodeUtility.StringToNullableShort(txtNContributiNL222.Text);

                if (!string.IsNullOrEmpty(txtInizioBonus.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.InizioBonus = Utility.GetDateFromString(txtInizioBonus.Text);
                if (!string.IsNullOrEmpty(txtFineBonus.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.FineBonus = Utility.GetDateFromString(txtFineBonus.Text);
            }
            //ENG - MEMO 74_2023
            //ENG - Memo 116/2025
            if (pnlCodiceConvenzione.Visible == true)
            {
                if (string.IsNullOrEmpty(txtCodiceConvenzione.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.CodiceConvenzioneAgo = null;
                else
                    areaLiquidazionePensioneAgo.DatiAssicurativi.CodiceConvenzioneAgo = byte.Parse(txtCodiceConvenzione.Text);
            }

            if (pnlTotSettEstereUtiliPerDirittoEContrEsteraTotale.Visible == true)
            {
                if (string.IsNullOrEmpty(txtCTotSettEstereUtiliPerDiritto.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.TotaleSettimaneEstereUtiliPerDiritto = null;
                else
                    areaLiquidazionePensioneAgo.DatiAssicurativi.TotaleSettimaneEstereUtiliPerDiritto = short.Parse(txtCTotSettEstereUtiliPerDiritto.Text);

                if (string.IsNullOrEmpty(txtContribuzioneEsteraTotale.Text))
                    areaLiquidazionePensioneAgo.DatiAssicurativi.ContribuzioneEsteraTotale = null;
                else
                    areaLiquidazionePensioneAgo.DatiAssicurativi.ContribuzioneEsteraTotale = int.Parse(txtContribuzioneEsteraTotale.Text);
            }
            //

            return areaLiquidazionePensioneAgo.DatiAssicurativi;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            HiddenFieldSiglaCategoria.Value = this.domanda.Categoria.Trim();
            SetSettimaneTotali();
        }

        protected void SalvaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            SetSettimaneTotali();

            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiAssicurativi = GetDatiAssicurativi();
            
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiAssicurativiAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiAssicurativiAgo(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Assicurativi: " + this.ErrorMessage;
            }
            else
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                ValorizzaEtichetteDatiAssicurativi(this, datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione, datiPensione.IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void nSettimaneOI_TextChanged(object sender, EventArgs e)
        {
            SetSettimaneTotali();
        }
        
        protected void nSettimaneOBG_TextChanged(object sender, EventArgs e)
        {
            SetSettimaneTotali();
        }
        
        private void SetSettimaneTotali()
        {
            //ENG - Memo 79
            if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
            {
                int settimaneOI = string.IsNullOrEmpty(txtNumeroSettimaneOI.Text) ? 0 : int.Parse(txtNumeroSettimaneOI.Text);
                int settimaneOBG = string.IsNullOrEmpty(txtNumeroSettimaneOBG.Text) ? 0 : int.Parse(txtNumeroSettimaneOBG.Text);
                txtNumeroSettimaneTot.Text = (settimaneOI + settimaneOBG).ToString();
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione, Presenter.SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (areaLiquidazione != null && areaLiquidazione.DatiAssicurativi != null)
            {
                Presenter.SvrLiquidazioneAgo.DatiAssicurativi datiAssicurativi = areaLiquidazione.DatiAssicurativi;
                Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                {
                    if (datiAssicurativi.InizioAssicurazione.HasValue || datiAssicurativi.FineAssicurazione.HasValue)
                        pnlInizioFineAssicurazione.Enabled = false;

                    txtNumeroSettimaneOBG.Enabled = false;
                    txtNumContrVolontari.Enabled = false;
                    txtNumContrVolontariAnz.Enabled = false;

                    if (datiAssicurativi.AttivitaEconomicaFELPE.HasValue || datiAssicurativi.ProfessioneIndividualeFELPE.HasValue)
                    {
                        txtAttivitaEconomica.Enabled = false;
                        txtProfessioneIndividuale.Enabled = false;
                    }

                    if (Utility.IsDomandaDAI(this.domanda.Categoria))
                    {
                        //per le DAI devono essere bloccati i campi Data Inizio Contribuzione Utile alla Misura e 
                        // Data Fine Contribuzione Utile alla Misura
                        txtInizioUltLav.Enabled = false;
                        txtFineUltLav.Enabled = false;
                    }
                }
                if (areaLiquidazione.IsDomandaAmianto181FromUnicarpe == true)
                {
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                }
            }
        }

        private void GestioneUsuranti(ILiquidazionePensioneAgo liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null &&
                liquidazione.areaLiquidazionePensioneAgo.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsUsuranti.Value)
            {
                if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    txtAttivitaEconomica.Text = "67";
                if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    txtProfessioneIndividuale.Text = "011";

                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
            }
        }

        private void ManageAnzianitaVecchiaia94_96(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            DateTime dataCompare = new DateTime(2000, 12, 01);

            if (this.domanda.Categoria.StartsWith("V") && (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value <= dataCompare))
            {
                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    pnlAnzVecch.Visible = true;
                pnlContenitoreReqArt2Dl503.Visible = true;
            }
        }

        private void ManageBonus(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.Filtro == "BNS")
                pnlBonus.Visible = true;
        }

        private void RenderControls(AreaTitolare.DatiPensione datiPensione, Presenter.SvrLiquidazioneAgo.AreaLiquidazionePensione areaLiquidazionePensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.SvrLiquidazioneAgo.DatiIstruttoria datiIstruttoria = areaLiquidazionePensione.DatiIstruttoria;

            ManageAnzianitaVecchiaia94_96(datiPensione);
            ManageBonus(datiPensione);
            ManageDomandaAUT(datiPensione);
            ManageDomandaINDCOM(this.domanda.Categoria);
            ManageDomandaTOT(this.domanda.Categoria, datiPensione);
            ManageDomandaPSO(this.domanda.Categoria);

            if (this.domanda.IsDomandaENPALS)
            {
                bool isEnpalsManuale = CodeUtility.IsEnpalsManualePL(this.domanda.IsDomandaENPALS, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati);

                trMessaggioStaticoENPALS.Visible = true;
                txtInizioBonus.Enabled = true;
                txtFineBonus.Enabled = true;

                if (Utility.IsDomandaReversibilita(datiPensione))
                {
                    if (areaLiquidazionePensione.DatiLiquidazionePensioneStorico != null && areaLiquidazionePensione.DatiLiquidazionePensioneStorico.InizioAssicurazione.HasValue)
                        txtInizioAssicurazione.Enabled = false;
                    if (areaLiquidazionePensione.DatiLiquidazionePensioneStorico != null && areaLiquidazionePensione.DatiLiquidazionePensioneStorico.FineAssicurazione.HasValue)
                        txtFineAssicurazione.Enabled = false;

                    txtAADiritto.Enabled = true;
                    txtMMDiritto.Enabled = true;
                    txtEtaDirittoAA.Enabled = true;
                    txtEtaDirittoMM.Enabled = true;
                    txtNTotDiritto.Enabled = true;
                    txtGruppoPrevalente.Enabled = true;
                    txtGruppoDiritto.Enabled = true;
                    txtRaggruppamentoPrevalente.Enabled = true;
                    txtQualifica.Enabled = true;
                    txtNTotQualifica.Enabled = true;
                    txtNTotContributi.Enabled = true;
                    txtNTotContributiEnpals.Enabled = true;
                    txtEtaMisuraAA.Enabled = true;
                    txtEtaMisuraMM.Enabled = true;
                    txtNContributiMisura.Enabled = true;
                    txtNContributiTriennio.Enabled = true;
                    txtNContributiQuinquennio.Enabled = true;
                    txtNContributiNL155.Enabled = true;
                    txtNContributiNL222.Enabled = true;
                    if (areaLiquidazionePensione.DecorrenzaPensioneDirettaDC.HasValue && !Utility.DataSuccessivaA(areaLiquidazionePensione.DecorrenzaPensioneDirettaDC.Value, new DateTime(1995, 1, 1)))
                    {
                        requiredInizioAssicurazione.Visible = false;
                        RFFineAssicurazione.Visible = false;
                    }
                }
                else
                {
                    // Caso ricostituzione non contributiva + indiretta
                    if (CodeUtility.IsRicostituzioneNonContributiva(datiPensione) || CodeUtility.IsRicostituzioneContributivaPerEsecuzioneSentenza(datiPensione) || Utility.IsDomandaIndiretta(datiPensione))
                    {
                        if (areaLiquidazionePensione.DatiLiquidazionePensioneStorico != null && areaLiquidazionePensione.DatiLiquidazionePensioneStorico.InizioAssicurazione.HasValue)
                            txtInizioAssicurazione.Enabled = false;
                        if (areaLiquidazionePensione.DatiLiquidazionePensioneStorico != null && areaLiquidazionePensione.DatiLiquidazionePensioneStorico.FineAssicurazione.HasValue)
                            txtFineAssicurazione.Enabled = false;
                    }
                    else if (!isEnpalsManuale)
                    {
                        txtInizioAssicurazione.Enabled = false;
                        txtFineAssicurazione.Enabled = false;
                    }

                    if (isEnpalsManuale)
                    {
                        txtAADiritto.Enabled = true;
                        txtMMDiritto.Enabled = true;
                        txtEtaDirittoAA.Enabled = true;
                        txtEtaDirittoMM.Enabled = true;
                        txtNTotDiritto.Enabled = true;
                        txtAnzianitaContributiva.Enabled = true;
                        txtGruppoPrevalente.Enabled = true;
                        txtGruppoDiritto.Enabled = true;
                        txtRaggruppamentoPrevalente.Enabled = true;
                        txtQualifica.Enabled = true;
                        txtNTotQualifica.Enabled = true;
                        txtNTotContributi.Enabled = true;
                        txtNTotContributiEnpals.Enabled = true;
                        txtEtaMisuraAA.Enabled = true;
                        txtEtaMisuraMM.Enabled = true;
                        txtNContributiMisura.Enabled = true;
                        txtNContributiTriennio.Enabled = true;
                        txtNContributiQuinquennio.Enabled = true;
                        txtNContributiNL155.Enabled = true;
                        txtNContributiNL222.Enabled = true;
                    }
                    else
                        txtNumContrVolontariAnz.Enabled = false;
                }
                pnlNSettimane_NContributiVolontariDiritto.Visible = false;
                pnlDatiAssicurativiENPALS.Visible = true;
                pnlAttEconomProfInd.Visible = false;
                if (!(CodeUtility.IsRicostituzione(datiPensione) && this.domanda.SiglaCategoriaPensione.ToUpperInvariant().StartsWith("S")))
                    pnlDecorrenzaPensioneENPALS.Visible = true;
                lblNumContrVolontariAnz.Text = "Numero Contributi Volontari:";
            }
            else if (Utility.IsDomandaDAI(this.domanda.Categoria))
            {
                pnlInizioFineUltimoLavoro.Visible = true;
                pnlImportoUltimaRetribuzione.Visible = false;
                lblInizioUltLAv.Text = "Data Inizio Contribuzione Utile alla Misura:";
                lblFineUltLav.Text = "Data Fine &nbsp Contribuzione Utile alla Misura:";
            }
            //VESO92 FILTRO L92
            if ((areaLiquidazionePensione != null && areaLiquidazionePensione.IsDomandaVESO92WithFiltroL92.GetValueOrDefault())
                || (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaVESO92WithGP2BB05(this.domanda.Categoria, this.domanda.GP2BB05)))
            {
                txtImportaUltimaRetribuzione.Text = string.Empty;
                txtImportaUltimaRetribuzione.Enabled = false;
                if (!areaLiquidazionePensione.IsDomandaVESO92WithFiltroL92.GetValueOrDefault())
                    txtNumeroSettimaneOBG.Enabled = false;
                //Permesso inserimento ContributiVolontariAnzianita e ContributiVolontariDiritto per tutte le domande di ESODO (JIRA L2_IST_LIQ-1835)
                //txtNumContrVolontari.Enabled = false;
                //txtNumContrVolontariAnz.Enabled = false;
                RFVtxtImportaUltimaRetribuzione.Enabled = false;
            }
            if (Utility.IsDomandaCumulo(this.domanda.Categoria))
            {
                if (datiPensione.IsDomandaCumuloAutomatica)
                {
                    txtInizioAssicurazione.Enabled = false;
                    txtFineAssicurazione.Enabled = false;
                    txtNumeroSettimaneOBG.Enabled = false;
                    txtNumContrVolontari.Enabled = false;
                    txtNumContrVolontariAnz.Enabled = false;
                }
                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    txtNumContrVolontari.Enabled = false;
                    txtNumContrVolontariAnz.Enabled = false;
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                }
            }
            if (Utility.IsDomandaVESO29(this.domanda.Categoria))
            {
                //Permesso inserimento ContributiVolontariAnzianita e ContributiVolontariDiritto per tutte le domande di ESODO (JIRA L2_IST_LIQ-1835)
                //pnlNSettimane_NContributiVolontariDiritto.Visible = false;
                //pnlNumContrVolontariAnz.Visible = false;
                pnlInizioFineUltimoLavoro.Visible = true;
                trInizioFineUltimoLavoro.Visible = false;
            }
            if (Utility.IsDomandaAPESociale(this.domanda.Categoria) && CodeUtility.IsRicostituzione(datiPensione))
            {
                txtInizioAssicurazione.Enabled = false;
                txtFineAssicurazione.Enabled = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                txtNumeroSettimaneOBG.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
            }
            if (Utility.IsDomandaVOESO(this.domanda.Categoria))
            {
                if ((!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) && ((Utility.IsAssegnoStraordinarioRiscossioneTributiErariali(datiPensione)) || (Utility.IsAssegnoStraordinarioFerrovieDelloStato(datiPensione) && this.domanda.CodiceTipoRichiesta == "74")))
                    || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) && datiIstruttoria != null && datiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP != null && ((Convert.ToInt32(datiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP) >= 900 && Convert.ToInt32(datiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP) <= 1000) || (datiIstruttoria != null && Utility.IsDomandaVOESOFerrovieDelloStatoRicConFiltro(this.domanda.Categoria, this.domanda.GP2BB05, datiIstruttoria.CodiceBancaEsodatiTraduzioneSuGP)))))
                {
                    pnlNSettimane_NContributiVolontariDiritto.Visible = true;
                    pnlNumContrVolontariAnz.Visible = true;
                    RFNumeroSettimane.Enabled = true;
                }
                //Permesso inserimento ContributiVolontariAnzianita e ContributiVolontariDiritto per tutte le domande di ESODO (JIRA L2_IST_LIQ-1835)
                //else
                //{
                //    pnlNSettimane_NContributiVolontariDiritto.Visible = false;
                //    pnlNumContrVolontariAnz.Visible = false;
                //}              
                pnlInizioFineUltimoLavoro.Visible = true;
                trInizioFineUltimoLavoro.Visible = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
            }
            if (Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria))
            {
                pnlInizioFineUltimoLavoro.Visible = true;
                trInizioFineUltimoLavoro.Visible = false;
                RFVtxtFineUltLav.Enabled = true;
                RFVtxtInizioUltLav.Enabled = true;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                //Permesso inserimento ContributiVolontariAnzianita e ContributiVolontariDiritto per tutte le domande di ESODO (JIRA L2_IST_LIQ-1835)
                //pnlNumContrVolontariAnz.Visible = false;
            }

            if (Utility.IsDomandaSPED(this.domanda.Categoria))
            {
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                RFAttivitaEconimica.Enabled = false;
                RFProfessioneIndividuale.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    txtNumeroSettimaneOBG.Enabled = false;
                }
            }

            if (Utility.IsDomandaRenditaCasalinghe(this.domanda.Categoria) || Utility.IsDomandaRenditaFacoltativa(this.domanda.Categoria))
            {
                txtNumeroSettimaneOBG.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
                ddlReqVecch1294.Enabled = false;
                ddlReqAnz1294.Enabled = false;
                ddlReqAnz996.Enabled = false;
            }

            if (Utility.IsDomandaVOST(this.domanda.Categoria))
            {
                txtAttivitaEconomica.Enabled = false;
                RFAttivitaEconimica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                RFProfessioneIndividuale.Enabled = false;
                txtNumeroSettimaneOBG.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
                ddlReqVecch1294.Enabled = false;
                ddlReqAnz1294.Enabled = false;
                ddlReqAnz996.Enabled = false;
            }
            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                areaLiquidazionePensione != null && areaLiquidazionePensione.DatiAssicurativi != null && areaLiquidazionePensione.DatiAssicurativi.ReqArt2DL503.HasValue)
                pnlReqArt2Dl503.Visible = true;
            else
                pnlReqArt2Dl503.Visible = false;

            if (Utility.IsDomandaRipristino(datiPensione) && !this.domanda.IsDomandaRiapertura)
            {
                txtInizioAssicurazione.Enabled = false;
                txtFineAssicurazione.Enabled = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                txtNumeroSettimaneOBG.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
                if (Utility.IsDomandaDAI(this.domanda.Categoria))
                {
                    //per le DAI devono essere bloccati i campi Data Inizio Contribuzione Utile alla Misura e 
                    // Data Fine Contribuzione Utile alla Misura
                    txtInizioUltLav.Enabled = false;
                    txtFineUltLav.Enabled = false;
                }
            }

            //Sbloccare i campi per riaperture di Ripristini e Riliquidazioni
            if (Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione) && this.domanda.IsDomandaRiapertura)
            {
                txtInizioAssicurazione.Enabled = true;
                txtFineAssicurazione.Enabled = true;
                txtAttivitaEconomica.Enabled = true;
                txtProfessioneIndividuale.Enabled = true;
                txtNumeroSettimaneOBG.Enabled = true;
                txtNumContrVolontari.Enabled = true;
                txtNumContrVolontariAnz.Enabled = true;
                txtInizioUltLav.Enabled = true;
                txtFineUltLav.Enabled = true;
            }

            if (datiPensione.IsDatiAggiuntiviFromJSON.GetValueOrDefault())
            {
                txtFineAssicurazione.Enabled = false;
                txtInizioUltLav.Enabled = false;
                txtFineUltLav.Enabled = false;
            }

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (Utility.IsDomandaVOCRED(this.domanda.Categoria) || Utility.IsDomandaVOCOOP(this.domanda.Categoria) || Utility.IsDomandaVOESO(this.domanda.Categoria)))
            {
                pnlCodiceAttivitaLavorativa.Visible = true;
                if (this.domanda.GP1AXE3 != null)
                {
                    switch (this.domanda.GP1AXE3)
                    {
                        case 0:
                            txtCodiceAttivitaLavorativa.Text = "";
                            pnlCodiceAttivitaLavorativa.Visible = false;
                            break;
                        case 1:
                            txtCodiceAttivitaLavorativa.Text = "1 - Concorrenziale azienda Esodante";
                            break;
                        case 2:
                            txtCodiceAttivitaLavorativa.Text = "2 - Non concorrenziale azienda Esodante";
                            break;
                        default:
                            txtCodiceAttivitaLavorativa.Text = this.domanda.GP1AXE3 + " -";
                            break;
                    }
                }
            }
            CodeUtility.DisableEliminaForRicostituzioni(btnEliminaDatiAssicurativi);
            CodeUtility.DisableEliminaForRipristini(btnEliminaDatiAssicurativi);
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria)))
            {
                btnEliminaDatiAssicurativi.Enabled = false;
            }

            //ENG - MEMO 74_2023
            //ENG - Memo 116/2025
            if ((areaLiquidazionePensione != null && areaLiquidazionePensione.DatiAssicurativi != null && areaLiquidazionePensione.IsMemo74_2023Abilitato.GetValueOrDefault()) ||
                datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione ||
                datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione)
            {
                pnlCodiceConvenzione.Visible = true;
                if (!(datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione ||
                    datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione))
                    pnlTotSettEstereUtiliPerDirittoEContrEsteraTotale.Visible = true;
            }

            //ENG - Memo 79
            pnlNSettimane_OrganizzazioniInternazionali.Visible = Utility.IsDomandaOrganizzazioniInternazionali(datiPensione);
            if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
                lblNumeroSettimane.InnerText = "Numero Settimane Italiane";
        }


        private void ManageDomandaAUT(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (Utility.IsDomandaAUT(this.domanda.Categoria))
            {
                if (Utility.IsDomandaVecchiaiaInComputo(datiPensione) || Utility.IsDomandaAUTAnticipataInComputo(datiPensione, this.domanda.Categoria, true))
                {
                    txtNumContrVolontari.Enabled = true;
                    txtNumContrVolontariAnz.Enabled = true;
                }
                else
                {
                    if (Utility.IsDomandaAUTAnticipataInComputo(datiPensione, this.domanda.Categoria, false))
                    {
                        txtNumContrVolontari.Enabled = true;
                        txtNumContrVolontariAnz.Enabled = true;
                    }
                    else if (!(!Utility.IsRicostituzione(datiPensione) && !this.domanda.IsDomandaRiapertura && datiPensione.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSEOrRicostituzione))
                    {
                        txtNumContrVolontari.Enabled = false;
                        txtNumContrVolontariAnz.Enabled = false;
                    }
                }
            }
        }

        private void ManageDomandaINDCOM(string categoria)
        {
            if (Utility.IsDomandaINDCOM(categoria))
            {
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
                txtNumeroSettimaneOBG.Enabled = false;
            }
        }

        private void ManageDomandaTOT(string categoria, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaTotalizzazione(categoria))
            {
                if (Utility.IsDomandaVOTOT(categoria))
                {
                    txtNumContrVolontari.Enabled = false;
                    txtNumContrVolontariAnz.Enabled = false;
                }

                if (datiPensione.IsDomandaTotAutomatica)
                {
                    txtInizioAssicurazione.Enabled = false;
                    txtFineAssicurazione.Enabled = false;
                    txtNumeroSettimaneOBG.Enabled = false;
                    txtNumContrVolontari.Enabled = false;
                    txtNumContrVolontariAnz.Enabled = false;
                }

                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura))
                {
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                }
            }
        }

        private void ManageDomandaPSO(string categoria)
        {
            if (Utility.IsDomandaPSO(categoria))
            {
                txtNumeroSettimaneOBG.Enabled = false;
                txtNumContrVolontari.Enabled = false;
                txtNumContrVolontariAnz.Enabled = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                txtNumeroSettimaneOBG.Text = string.Empty;
                txtNumContrVolontari.Text = string.Empty;
                txtNumContrVolontariAnz.Text = string.Empty;
                txtAttivitaEconomica.Text = string.Empty;
                txtProfessioneIndividuale.Text = string.Empty;
                hiddenFieldAttivitaEconomica.Value = string.Empty;
                hiddenFieldProfessioneIndividuale.Value = string.Empty;
                RFAttivitaEconimica.Enabled = false;
                RFProfessioneIndividuale.Enabled = false;
                if (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura)
                {
                    txtInizioAssicurazione.Enabled = false;
                    txtFineAssicurazione.Enabled = false;
                }

            }
        }

        private void ValorizzaEtichetteDatiAssicurativiENPALS(ILiquidazionePensioneAgo liquidazione)
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (this.TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue)
                txtDecorrenzaPensione.Text = string.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value);

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi != null)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioAssicurazione.HasValue)
                    txtInizioAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioAssicurazione.Value);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS != null)
                {
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.AADiritto.HasValue)
                        txtAADiritto.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.AADiritto.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.MMDiritto.HasValue)
                        txtMMDiritto.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.MMDiritto.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.RaggruppamentoPrevalente.HasValue)
                        txtRaggruppamentoPrevalente.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.RaggruppamentoPrevalente.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.GruppoPrevalente.HasValue)
                        txtGruppoPrevalente.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.GruppoPrevalente.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.GruppoDiritto.HasValue)
                        txtGruppoDiritto.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.GruppoDiritto.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotContributi.HasValue)
                        txtNTotContributi.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotContributi.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotContributiEnpals.HasValue)
                        txtNTotContributiEnpals.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotContributiEnpals.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaDirittoAA.HasValue)
                        txtEtaDirittoAA.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaDirittoAA.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaDirittoMM.HasValue)
                        txtEtaDirittoMM.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaDirittoMM.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaMisuraAA.HasValue)
                        txtEtaMisuraAA.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaMisuraAA.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaMisuraMM.HasValue)
                        txtEtaMisuraMM.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.EtaMisuraMM.Value.ToString();

                    if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.Qualifica))
                        txtQualifica.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.Qualifica;

                    //if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.DataFinestra.HasValue)
                    //    txtDataFinestra.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.DataFinestra.Value);

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiMisura.HasValue)
                        txtNContributiMisura.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiMisura.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotDiritto.HasValue)
                        txtNTotDiritto.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotDiritto.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.AnzianitaContributiva.HasValue)
                        txtAnzianitaContributiva.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.AnzianitaContributiva.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotQualifica.HasValue)
                        txtNTotQualifica.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NTotQualifica.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiQuinquennio.HasValue)
                        txtNContributiQuinquennio.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiQuinquennio.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiTriennio.HasValue)
                        txtNContributiTriennio.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiTriennio.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiNL222.HasValue)
                        txtNContributiNL222.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiNL222.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiNL155.HasValue)
                        txtNContributiNL155.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.DatiENPALS.NContributiNL155.Value.ToString();
                }

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ReqArt2DL503.HasValue)
                    ddlReqArt2Dl503.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ReqArt2DL503.Value.ToString();
            }

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico != null)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.AttivitaEconomica.HasValue ||
                    liquidazione.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ProfessioneIndividuale.HasValue)
                {
                    txtAttivitaEconomica.Enabled = false;
                    txtProfessioneIndividuale.Enabled = false;
                    pnlAttEconomProfInd.Visible = true;
                }
            }
        }

        private void ValorizzaEtichetteDatiAssicurativiAGO(ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi != null)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioAssicurazione.HasValue)
                    txtInizioAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioAssicurazione.Value);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioUltimoLavoro.HasValue)
                    txtInizioUltLav.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioUltimoLavoro.Value);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.FineUltimoLavoro.HasValue)
                    txtFineUltLav.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.FineUltimoLavoro.Value);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ImportoUltimaRetribuzione.HasValue)
                    txtImportaUltimaRetribuzione.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ImportoUltimaRetribuzione.ToString();
                else
                    txtImportaUltimaRetribuzione.Text = string.Empty;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOBG.HasValue)
                {
                    txtNumeroSettimaneOBG.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOBG.Value.ToString();
                }
                else
                    txtNumeroSettimaneOBG.Text = string.Empty;

                if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione) && liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOI.HasValue)
                {
                    int settimaneOI = 0;
                    int settimaneOBG;
                    int.TryParse(liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOI.Value.ToString(), out settimaneOI);
                    int.TryParse(liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOBG.Value.ToString(), out settimaneOBG);
                    txtNumeroSettimaneOI.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NSettimaneOI.Value.ToString();
                    txtNumeroSettimaneTot.Text = (settimaneOI + settimaneOBG).ToString();
                }
                else
                    txtNumeroSettimaneOI.Text = string.Empty;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVolontari.HasValue)
                    txtNumContrVolontari.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVolontari.Value.ToString();
                else
                    txtNumContrVolontari.Text = string.Empty;

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ReqArt2DL503.HasValue)
                    ddlReqArt2Dl503.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ReqArt2DL503.Value.ToString();

                GestioneEtichetteIsUnicarpe(datiPensione, liquidazione.areaLiquidazionePensioneAgo);
            }
            else
            {
                if (!datiPensione.FlagUnicarpe.HasValue || (datiPensione.FlagUnicarpe.HasValue && !datiPensione.FlagUnicarpe.Value))
                {
                    txtInizioAssicurazione.Text = "GG/MM/AAAA";
                    txtInizioUltLav.Text = "GG/MM/AAAA";
                    txtFineUltLav.Text = "GG/MM/AAAA";
                    txtNumeroSettimaneOBG.Text = string.Empty;
                    txtNumContrVolontari.Text = string.Empty;
                    txtImportaUltimaRetribuzione.Text = string.Empty;
                }
            }

            if ((!datiPensione.FlagUnicarpe.HasValue || (datiPensione.FlagUnicarpe.HasValue && !datiPensione.FlagUnicarpe.Value)) &&
                (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione))
            {
                txtInizioUltLav.Enabled = false;
                txtFineUltLav.Enabled = false;
            }

            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }
            if (this.domanda == null)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            }


            if ((Utility.IsDomandaBancari(this.domanda.SiglaCategoriaPensione) && Utility.IsRicostituzione(this.domanda.CodGruppo)) ||
                (Utility.IsDomandaRicOrTrf_PSO_PMO_DAIAnte2003(this.domanda.Categoria, datiPensione.CodeGruppo, liquidazione.areaLiquidazionePensioneAgo.DataAssunzioneCarico, domanda.IsDomandaRiapertura))
                || (liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.IsAnte96 != null))
            {
                RegularExpressionValidator2.Visible = false;
                requiredInizioAssicurazione.Visible = false;
                customInizioAssicurazione.Visible = false;
                customCheckDataInizioAssicurazione.Visible = false;
                validatetxtFineAssicurazione.Visible = false;
                customFineAssicurazione.Visible = false;
                RFFineAssicurazione.Visible = false;
                customCheckDataFineAssicurazione.Visible = false;
                RegularExpressionValidator4.Visible = false;
                RFAttivitaEconimica.Visible = false;
                checkLenghtAttEconomica.Visible = false;
                RegularExpressionValidator3.Visible = false;
                RFProfessioneIndividuale.Visible = false;
                checkLenghtProfIndividuale.Visible = false;
            }

            //ENG - Spacchettamento SOPGI
            if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa) &&
                !((Utility.IsRicostituzione(datiPensione.CodeGruppo) || (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && !string.IsNullOrEmpty(datiPensione.GP1AV91B) && datiPensione.GP1AV91B == "2"))
            {
                if (Utility.IsDomandaReversibilita(datiPensione) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    txtInizioAssicurazione.Enabled = false;
                    txtFineAssicurazione.Enabled = false;
                }
            }

            //ENG - SOPGI pensioni di reversibilità il campo GP1AV08 prelevato dalla GAIN deve essere visibile sul pannello dati assicurativi (campo “Numero Settimane”) non editabile
            if (this.domanda.Categoria != null && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))
            {
                txtNumeroSettimaneOBG.Enabled = false;
            }

            //ENG - RIC/TRF (NO ENPALS): rendere non obbligatori i campi "Attivita Economica" e "Professione Individuale" se dal prelievo arrivano vuoti
            if (Utility.IsRicostituzione(datiPensione) || this.domanda.IsDomandaRiapertura)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null)
                {
                    if (!liquidazione.areaLiquidazionePensioneAgo.IsAttivitaEconomicaDaPrelievo.HasValue || !liquidazione.areaLiquidazionePensioneAgo.IsAttivitaEconomicaDaPrelievo.Value)
                        RFAttivitaEconimica.Enabled = false;

                    if (!liquidazione.areaLiquidazionePensioneAgo.IsProfessioneIndividualeDaPrelievo.HasValue || !liquidazione.areaLiquidazionePensioneAgo.IsProfessioneIndividualeDaPrelievo.Value)
                        RFProfessioneIndividuale.Enabled = false;

                }
            }
            //ENG - MEMO 74_2023
            //ENG - Memo 116/2025
            if (pnlCodiceConvenzione.Visible == true)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi != null &&
                    liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.CodiceConvenzioneAgo.HasValue)
                    txtCodiceConvenzione.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.CodiceConvenzioneAgo.Value.ToString();
                else
                    txtCodiceConvenzione.Text = string.Empty;
            }

            if (pnlTotSettEstereUtiliPerDirittoEContrEsteraTotale.Visible == true)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi != null &&
                    liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.TotaleSettimaneEstereUtiliPerDiritto.HasValue)
                    txtCTotSettEstereUtiliPerDiritto.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.TotaleSettimaneEstereUtiliPerDiritto.Value.ToString();
                else
                    txtCTotSettEstereUtiliPerDiritto.Text = string.Empty;

                if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi != null &&
                    liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ContribuzioneEsteraTotale.HasValue)
                    txtContribuzioneEsteraTotale.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ContribuzioneEsteraTotale.Value.ToString();
                else
                    txtContribuzioneEsteraTotale.Text = string.Empty;
            }
            //
        }

        private void ValorizzaEtichetteCommon(ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione ||
                (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.IsPensioneInvaliditaInabilitaENPALSOrCasellario.HasValue &&
                liquidazione.areaLiquidazionePensioneAgo.IsPensioneInvaliditaInabilitaENPALSOrCasellario.Value) ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Inabilita_Art1_C250_Legge232 ||
                datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaAPEPrecociOrRicostituzione ||
                datiPensione.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione || datiPensione.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione ||
                datiPensione.SceltaLavoratriciMadri.HasValue || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                hdnNOTUncheckBenefici.Value = "TRUE";
            if (datiPensione.SceltaLavoratriciMadri.HasValue || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) || Utility.IsDomandaRipristino(datiPensione) || Utility.IsDomandaUsuranti(datiPensione) || (Utility.IsDomandaSPED(domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura))
                || (Utility.IsDomandaCumulo(domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura)))
            {
                if (!(datiPensione.SceltaLavoratriciMadri.HasValue && datiPensione.SceltaLavoratriciMadri.Value == 1 && Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaPensioneReversibilitaOrRicostituzione(domanda.Categoria, datiPensione, areaDanteCausa) && !Utility.IsDomandaCumulo(domanda.Categoria)
                    && liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.TipoSettimaneBeneficio == "12"))
                {
                    hdnSKIP_ManageEnableBeneficiJS.Value = "TRUE";
                }
            }

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneAgo != null)
            {
                hdnIsDatiBeneficiSalvati.Value = liquidazione.areaLiquidazionePensioneAgo.IsDatiBeneficiSalvati.GetValueOrDefault().ToString().ToUpperInvariant();

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi != null)
                {
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.FineAssicurazione.HasValue)
                        txtFineAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.FineAssicurazione.Value);
                    else
                        txtFineAssicurazione.Text = "GG/MM/AAAA";

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica.HasValue)
                    {
                        txtAttivitaEconomica.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica.Value.ToString().PadLeft(2, '0');
                        hiddenFieldAttivitaEconomica.Value = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.AttivitaEconomica.Value.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        txtAttivitaEconomica.Text = string.Empty;
                        hiddenFieldAttivitaEconomica.Value = string.Empty;
                    }

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ProfessioneIndividuale.HasValue)
                    {
                        txtProfessioneIndividuale.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ProfessioneIndividuale.Value.ToString().PadLeft(3, '0');
                        hiddenFieldProfessioneIndividuale.Value = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.ProfessioneIndividuale.Value.ToString().PadLeft(3, '0');
                    }
                    else
                    {
                        txtProfessioneIndividuale.Text = string.Empty;
                        hiddenFieldProfessioneIndividuale.Value = string.Empty;
                    }

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVVAnzianita.HasValue)
                        txtNumContrVolontariAnz.Text = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.NContributiVVAnzianita.Value.ToString();
                    else
                        txtNumContrVolontariAnz.Text = string.Empty;

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiVecchiaiaAl1294 != null)
                    {
                        if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiVecchiaiaAl1294)
                            ddlReqVecch1294.SelectedValue = "SI";
                        else if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiVecchiaiaAl1294 == false)
                            ddlReqVecch1294.SelectedValue = "NO";
                    }
                    else
                        ddlReqVecch1294.SelectedIndex = 0;

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl996 != null)
                    {
                        if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl996)
                            ddlReqAnz996.SelectedValue = "SI";
                        else if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl996 == false)
                            ddlReqAnz996.SelectedValue = "NO";
                    }
                    else
                        ddlReqAnz996.SelectedIndex = 0;

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl1294 != null)
                    {
                        if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl1294)
                            ddlReqAnz1294.SelectedValue = "SI";
                        else if ((bool)liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.RequisitiAl1294 == false)
                            ddlReqAnz1294.SelectedValue = "NO";
                    }
                    else
                        ddlReqAnz1294.SelectedIndex = 0;

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioBonus.HasValue)
                        txtInizioBonus.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.InizioBonus.Value);
                    else
                        txtInizioBonus.Text = "GG/MM/AAAA";

                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.FineBonus.HasValue)
                        txtFineBonus.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativi.FineBonus.Value);
                    else
                        txtFineBonus.Text = "GG/MM/AAAA";

                    //if (datiPensione.TipoAutomazione.HasValue)
                    //{
                    //    if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                    //        txtAttivitaEconomica.Text = "11";
                    //    if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                    //        txtProfessioneIndividuale.Text = "111";
                    //}
                }
                else
                {
                    if (!datiPensione.FlagUnicarpe.HasValue || (datiPensione.FlagUnicarpe.HasValue && !datiPensione.FlagUnicarpe.Value))
                    {
                        txtFineAssicurazione.Text = "GG/MM/AAAA";
                        txtNumContrVolontariAnz.Text = string.Empty;
                    }
                    txtAttivitaEconomica.Text = string.Empty;
                    txtProfessioneIndividuale.Text = string.Empty;

                    if (datiPensione.TipoAutomazione.HasValue)
                    {
                        if (string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                            txtAttivitaEconomica.Text = "11";
                        if (string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                            txtProfessioneIndividuale.Text = "111";
                    }
                }
            }
            SetSettimaneTotali();
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
    }
}
