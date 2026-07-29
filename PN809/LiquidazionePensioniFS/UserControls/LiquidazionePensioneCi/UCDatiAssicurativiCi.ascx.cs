using System;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using System.Collections.Generic;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi
{
    public partial class UCDatiAssicurativiCi : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneCi
    {
        #region ILiquidazionePensioneCi
        public AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion ILiquidazionePensioneCi

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        internal void ValorizzaEtichetteDatiAssicurativi(ILiquidazionePensioneCi liquidazione, bool isDomandaInabilitaAmianto, bool isDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto)
        {
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            DateTime? dataDecorrenza = (DateTime?)((AreaTitolare.DatiPensione)Session["DatiPensione"]).DecorrenzaOriginaria;
            LoadDdl(liquidazione);
            ManageAnzianitaVecchiaia94_96(datiPensione, liquidazione.areaLiquidazionePensioneCi);
            ViewState["TipoGestione"] = liquidazione.areaLiquidazionePensioneCi.IsGestioneNormale;
            bool isNormale = (bool)ViewState["TipoGestione"];
            VisualizzaPannelli(datiPensione);

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneCi != null && liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi != null)
            {
                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.InizioAssicurazione.HasValue)
                    txtInizioAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.InizioAssicurazione.Value);

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.FineAssicurazione.HasValue)
                    txtFineAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.FineAssicurazione.Value);

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.AttivitaEconomica.HasValue)
                    txtAttivitaEconomica.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.AttivitaEconomica.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ProfessioneIndividuale.HasValue)
                    txtProfessioneIndividuale.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ProfessioneIndividuale.Value.ToString();

                if (datiPensione.CodeGruppo == "0001")
                {
                    pnlDelibera12688.Visible = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto.HasValue && liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto.Value > 1040;
                }

                if (isNormale)
                {
                    #region Gestione Normale

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RMS8888.HasValue)
                    {
                        decimal RMS8888 = Math.Truncate(10000 * (decimal)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RMS8888 / 10000);
                        txtRMS8888.Text = RMS8888.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    }

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RMS9090.HasValue)
                    {
                        decimal RMS9090 = Math.Truncate(10000 * (decimal)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RMS9090 / 10000);
                        txtRMS9090.Text = RMS9090.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    }

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.VVMisuraAl1292.HasValue)
                        txtVVMisura1292.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.VVMisuraAl1292.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.VVMisuraDL50392.HasValue)
                        txtVVMisuraDl50392.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.VVMisuraDL50392.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimanePerCalcoloContributivo.HasValue)
                        txtSettCalcoloContrib.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimanePerCalcoloContributivo.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS_Art11.HasValue)
                        txtIVSArt11488.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS_Art11.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NSettimaneOBG.HasValue)
                        txtSettOBGDiritto.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NSettimaneOBG.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NContributiVolontari.HasValue)
                        txtSettVVDiritto.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NContributiVolontari.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.IsImportoIVSVisible.HasValue && liquidazione.areaLiquidazionePensioneCi.IsImportoIVSVisible.Value)
                    {
                        if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS.HasValue)
                            txtImportoIVS.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                        lblImportoIVS.Visible = true;
                        txtImportoIVS.Visible = true;
                    }

                    #endregion Gestione Normale
                }
                else
                {
                    #region Gestione Speciale

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto.HasValue)
                        txtSettItalianeDiritto.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeMisura.HasValue)
                        txtSettItalianeMisura.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeMisura.ToString();

                    if (liquidazione.areaLiquidazionePensioneCi.IsAnte96.GetValueOrDefault())
                    {
                        if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS.HasValue)
                            txtImportoIVS.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS.ToString();

                        lblImportoIVS.Visible = true;
                        txtImportoIVS.Visible = true;
                    }

                    #endregion Gestione Speciale
                }

                #region Gestione Comune 2

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NSettFittiziePrepensionamento.HasValue)
                    txtSettFittizie.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NSettFittiziePrepensionamento.ToString();

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NContributiItalia.HasValue)
                    txtSettEffettive.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NContributiItalia.ToString();

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NSettGodimentoAssegno.HasValue)
                    txtSettGodimentoAssegno.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.NSettGodimentoAssegno.ToString();

                #endregion Gestione Comune 2

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiVecchiaiaAl1294.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiVecchiaiaAl1294.Value)
                        ddlReqVecch1294.SelectedValue = "SI";
                    else if (!(bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiVecchiaiaAl1294.Value)
                        ddlReqVecch1294.SelectedValue = "NO";
                }

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl996.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl996.Value)
                        ddlReqVecch996.SelectedValue = "SI";
                    else if (!(bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl996.Value)
                        ddlReqVecch996.SelectedValue = "NO";
                }

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl1294.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl1294.Value)
                        ddlReqAnz1294.SelectedValue = "SI";
                    else if (!(bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl1294.Value)
                        ddlReqAnz1294.SelectedValue = "NO";
                }

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceConvenzione.HasValue)
                    txtCodiceConvenzione.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceConvenzione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.AnniDifferimento.HasValue)
                    txtAnniDifferimento.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.AnniDifferimento.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceVirtuale.HasValue)
                    ddlCodiceVirtuale.SelectedValue = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceVirtuale.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.DecorrenzaCodiceVirtuale.HasValue)
                    txtDecCodVirtuale.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.DecorrenzaCodiceVirtuale.Value);
                else
                    txtDecCodVirtuale.Text = "MM/AAAA";

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.DeliberaCee126.HasValue && liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.DeliberaCee126.Value)
                    chkDelibera12688.Checked = true;
                else
                    chkDelibera12688.Checked = false;

                ////////////// in attesa di maggiori specifiche sul tipo di dato (numero di cifre massime editabili, lunghezza parte decimale, ...) /////////////////////////
                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoCristallizzazione3481.HasValue)
                    txtImportoCristallizzazione.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.ImportoCristallizzazione3481.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceBloccoArretratiEE.HasValue)
                {
                    if ((bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceBloccoArretratiEE.Value)
                        ddlCodice.SelectedValue = "SI";
                    else if (!(bool)liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceBloccoArretratiEE.Value)
                        ddlCodice.SelectedValue = "NO";
                }

                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.UfficioPagatoreArretratiEsteri))
                    txtCodUffPagatoreIstEstera.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.UfficioPagatoreArretratiEsteri;

                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiciMotivazioniCi281))
                    ddlCodiceMotivazioniCI28.SelectedValue = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiciMotivazioniCi281;

                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiciCi21.HasValue)
                    ddlCodiceCI21.SelectedValue = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiciCi21.Value.ToString();

                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneOBGMisura12_92.HasValue)
                {
                    txtSettimaneOBGMisura12_92.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneOBGMisura12_92.Value.ToString();
                }

                bool checkTipoAppartenenza = domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI;
                bool checkTipoDomanda = domanda.Tipo.Trim().Equals("RIC");
                List<string> categorieDaEscudere = new List<string>(){ "VOARTS" , "IOARTS", "SOARTS", "VOCOMS", "IOCOMS", "SOCOMS", "VRS", "IRS", "SRS"};
                bool checkCategoriaDomanda = categorieDaEscudere.Contains<string>(domanda.Categoria.Trim());
                if (checkTipoAppartenenza && checkTipoDomanda && checkCategoriaDomanda)
                    txtSettimaneOBGMisura12_92.Enabled = false;


                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneOBGMisuraDL503_92.HasValue)
                {
                    txtSettimaneOBGMisuraDL503_92.Text = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneOBGMisuraDL503_92.Value.ToString();
                }

                if (checkTipoAppartenenza && checkTipoDomanda && checkCategoriaDomanda)
                    txtSettimaneOBGMisuraDL503_92.Enabled = false;

                GestioneUsuranti(liquidazione);

                //ENG - Gestione Nuovo Codice CI28
                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceCI28.HasValue)
                    ddlCodiceCI28.SelectedValue = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceCI28.Value.ToString();

                //ENG - Sistemazione per Codice Requisiti Particolari
                if (liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceRequisitiParticolari.HasValue)
                    ddlRequisitiParticolariDiritto.SelectedValue = liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi.CodiceRequisitiParticolari.Value.ToString();
            }

            if (datiPensione.FlagUnicarpe.HasValue)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneCi != null)
                    GestioneEtichetteIsUnicarpe(liquidazione.areaLiquidazionePensioneCi.DatiAssicurativi, datiPensione);
            }

            if (isDomandaInabilitaAmianto)
            {
                txtAttivitaEconomica.Text = "01";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "250";
                txtProfessioneIndividuale.Enabled = false;
            }
            else if (isDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto)
            {
                txtAttivitaEconomica.Text = "15";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "208";
                txtProfessioneIndividuale.Enabled = false;
            }
        }

        internal DatiAssicurativi GetDatiAssicurativi()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiAssicurativi = new DatiAssicurativi();

            if (!string.IsNullOrEmpty(txtInizioAssicurazione.Text) && !(txtInizioAssicurazione.Text.ToUpperInvariant().Equals("GG/MM/AAAA")))
                areaLiquidazionePensioneCi.DatiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtInizioAssicurazione.Text);

            if (!string.IsNullOrEmpty(txtFineAssicurazione.Text) && !(txtFineAssicurazione.Text.ToUpperInvariant().Equals("GG/MM/AAAA")))
                areaLiquidazionePensioneCi.DatiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtFineAssicurazione.Text);

            if (!string.IsNullOrEmpty(txtAttivitaEconomica.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.AttivitaEconomica = int.Parse(txtAttivitaEconomica.Text);

            if (!string.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.ProfessioneIndividuale = int.Parse(txtProfessioneIndividuale.Text);

            if (!string.IsNullOrEmpty(txtRMS8888.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.RMS8888 = decimal.Parse(txtRMS8888.Text);

            if (!string.IsNullOrEmpty(txtRMS9090.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.RMS9090 = decimal.Parse(txtRMS9090.Text);

            if (!string.IsNullOrEmpty(txtVVMisura1292.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.VVMisuraAl1292 = int.Parse(txtVVMisura1292.Text);

            if (!string.IsNullOrEmpty(txtVVMisuraDl50392.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.VVMisuraDL50392 = int.Parse(txtVVMisuraDl50392.Text);

            if (!string.IsNullOrEmpty(txtSettCalcoloContrib.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.SettimanePerCalcoloContributivo = int.Parse(txtSettCalcoloContrib.Text);

            if (!string.IsNullOrEmpty(txtIVSArt11488.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS_Art11 = decimal.Parse(txtIVSArt11488.Text);

            if (!string.IsNullOrEmpty(txtSettOBGDiritto.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.NSettimaneOBG = int.Parse(txtSettOBGDiritto.Text);

            if (!string.IsNullOrEmpty(txtSettVVDiritto.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.NContributiVolontari = int.Parse(txtSettVVDiritto.Text);

            if (!string.IsNullOrEmpty(txtSettItalianeDiritto.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto = int.Parse(txtSettItalianeDiritto.Text);

            if (!string.IsNullOrEmpty(txtImportoIVS.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.ImportoIVS = decimal.Parse(txtImportoIVS.Text);

            if (!string.IsNullOrEmpty(txtSettFittizie.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.NSettFittiziePrepensionamento = int.Parse(txtSettFittizie.Text);

            if (!string.IsNullOrEmpty(txtSettEffettive.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.NContributiItalia = int.Parse(txtSettEffettive.Text);

            if (!string.IsNullOrEmpty(txtSettGodimentoAssegno.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.NSettGodimentoAssegno = int.Parse(txtSettGodimentoAssegno.Text);

            if (String.Equals(ddlReqVecch1294.SelectedValue, "SI"))
                areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiVecchiaiaAl1294 = true;
            else if (String.Equals(ddlReqVecch1294.SelectedValue, "NO"))
                areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiVecchiaiaAl1294 = false;

            if (String.Equals(ddlReqVecch996.SelectedValue, "SI"))
                areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl996 = true;
            else if (String.Equals(ddlReqVecch996.SelectedValue, "NO"))
                areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl996 = false;

            if (String.Equals(ddlReqAnz1294.SelectedValue, "SI"))
                areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl1294 = true;
            else if (String.Equals(ddlReqAnz1294.SelectedValue, "NO"))
                areaLiquidazionePensioneCi.DatiAssicurativi.RequisitiAl1294 = false;

            if (!string.IsNullOrEmpty(txtCodiceConvenzione.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiceConvenzione = byte.Parse(txtCodiceConvenzione.Text);

            if (!string.IsNullOrEmpty(txtAnniDifferimento.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.AnniDifferimento = int.Parse(txtAnniDifferimento.Text);

            if (!string.IsNullOrEmpty(ddlCodiceVirtuale.SelectedValue))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiceVirtuale = char.Parse(ddlCodiceVirtuale.SelectedValue);

            if (!string.IsNullOrEmpty(txtDecCodVirtuale.Text) && !(txtDecCodVirtuale.Text.ToUpperInvariant().Equals("MM/AAAA")))
                areaLiquidazionePensioneCi.DatiAssicurativi.DecorrenzaCodiceVirtuale = Utility.GetDateFromString(txtDecCodVirtuale.Text);

            areaLiquidazionePensioneCi.DatiAssicurativi.DeliberaCee126 = chkDelibera12688.Checked == true ? chkDelibera12688.Checked : false;

            //////////// in attesa di maggiori specifiche sul tipo di dato (numero di cifre massime editabili, lunghezza parte decimale, ...) /////////////////////////
            if (!string.IsNullOrEmpty(txtImportoCristallizzazione.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.ImportoCristallizzazione3481 = Convert.ToDecimal(txtImportoCristallizzazione.Text);

            if (String.Equals(ddlCodice.SelectedValue, "SI"))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiceBloccoArretratiEE = true;
            else if (String.Equals(ddlCodice.SelectedValue, "NO"))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiceBloccoArretratiEE = false;

            if (!string.IsNullOrEmpty(txtCodUffPagatoreIstEstera.Text))
                areaLiquidazionePensioneCi.DatiAssicurativi.UfficioPagatoreArretratiEsteri = txtCodUffPagatoreIstEstera.Text;

            if (!string.IsNullOrEmpty(ddlCodiceMotivazioniCI28.SelectedValue))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiciMotivazioniCi281 = ddlCodiceMotivazioniCI28.SelectedValue;

            if (!string.IsNullOrEmpty(ddlCodiceCI21.SelectedValue))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiciCi21 = char.Parse(ddlCodiceCI21.SelectedValue);

            if (!string.IsNullOrEmpty(ddlRequisitiParticolariDiritto.SelectedValue))
                areaLiquidazionePensioneCi.DatiAssicurativi.CodiceRequisitiParticolari = byte.Parse(ddlRequisitiParticolariDiritto.SelectedValue);

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (datiPensione.CodeGruppo == "0001")
            {
                pnlDelibera12688.Visible = areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto.HasValue && areaLiquidazionePensioneCi.DatiAssicurativi.SettimaneItalianeDiritto.Value > 1040;
            }

            //ENG - Gestione Nuovo Codice CI28   
            if (!String.IsNullOrEmpty(ddlCodiceCI28.SelectedValue))
            {
                char codiceCI28 = ' ';
                if (char.TryParse(ddlCodiceCI28.SelectedValue, out codiceCI28))
                    areaLiquidazionePensioneCi.DatiAssicurativi.CodiceCI28 = CodeUtility.StringToNullableChar(codiceCI28.ToString());
            }

            return areaLiquidazionePensioneCi.DatiAssicurativi;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiAssicurativi = GetDatiAssicurativi();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiAssicurativiCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiAssicurativiCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Assicurativi";
            }
            else
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                ClearForm();
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

        #region Private Methods

        private void GestioneEtichetteIsUnicarpe(Presenter.SvrLiquidazioneCi.DatiAssicurativi datiAssicurativi, AreaTitolare.DatiPensione datiPensione)
        {
            if (datiAssicurativi != null)
            {
                Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                {
                    if (datiAssicurativi.InizioAssicurazione.HasValue || datiAssicurativi.FineAssicurazione.HasValue)
                        pnlInizioFineAssicurazione.Enabled = false;

                    //gestione normale
                    txtVVMisura1292.Enabled = false;
                    txtVVMisuraDl50392.Enabled = false;
                    txtSettOBGDiritto.Enabled = false;
                    txtSettVVDiritto.Enabled = false;
                    //gestion speciale
                    txtSettItalianeDiritto.Enabled = false;
                    //comune ad entrambe le gestioni
                    txtImportoIVS.Enabled = false;
                    txtSettFittizie.Enabled = false;
                    txtSettEffettive.Enabled = false;
                    txtSettGodimentoAssegno.Enabled = false;
                }
            }
        }

        private void GestioneUsuranti(ILiquidazionePensioneCi liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneCi != null &&
                liquidazione.areaLiquidazionePensioneCi.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneCi.IsUsuranti.Value)
            {
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
            }
        }

        private void GestioneRicostituzioni(ILiquidazionePensioneCi liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                pnlCodiceMotivazioniCI28.Visible = true;
        }

        private void LoadDdl(ILiquidazionePensioneCi liquidazioneCi)
        {
            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.lCodiceVirtuale != null && liquidazioneCi.areaLiquidazionePensioneCi.lCodiceVirtuale.Count() > 0)
                {
                    CodeUtility.SetValueDdl(ddlCodiceVirtuale, string.Empty, string.Empty);
                    foreach (CodiceVirtuale codeVirtuale in liquidazioneCi.areaLiquidazionePensioneCi.lCodiceVirtuale)
                    {
                        string descrizione = codeVirtuale.Id + " - " + codeVirtuale.Descrizione;
                        CodeUtility.SetValueDdl(ddlCodiceVirtuale, descrizione, descrizione, codeVirtuale.Id);
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi21 != null && liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi21.Count() > 0)
                {
                    CodeUtility.SetValueDdl(ddlCodiceCI21, string.Empty, string.Empty);
                    foreach (CodiceCi21 codeCi21 in liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi21)
                    {
                        CodeUtility.SetValueDdl(ddlCodiceCI21, codeCi21.Descrizione, codeCi21.Codice.ToString());
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi28 != null && liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi28.Count() > 0)
                {
                    CodeUtility.SetValueDdl(ddlCodiceMotivazioniCI28, string.Empty, string.Empty);
                    foreach (CodiceCi28 codeCi28 in liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi28)
                    {
                        CodeUtility.SetValueDdl(ddlCodiceMotivazioniCI28, codeCi28.Descrizione, codeCi28.Codice);
                    }
                }

                //ENG - Gestione Nuovo Codice CI28
                if (liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi28 != null && liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi28.Count() > 0)
                {
                    CodeUtility.SetValueDdl(ddlCodiceCI28, string.Empty, string.Empty);
                    foreach (CodiceCi28 codiceCI28 in liquidazioneCi.areaLiquidazionePensioneCi.lCodiceCi28)
                    {
                        if (codiceCI28.Codice == "A" || codiceCI28.Codice == "B" || codiceCI28.Codice == "C")
                            CodeUtility.SetValueDdl(ddlCodiceCI28, codiceCI28.Descrizione, codiceCI28.Codice);
                    }
                }

                if (liquidazioneCi.areaLiquidazionePensioneCi.listaCodiceRequisitiParticolari != null && liquidazioneCi.areaLiquidazionePensioneCi.listaCodiceRequisitiParticolari.Count() > 0)
                {
                    CodeUtility.SetValueDdl(ddlRequisitiParticolariDiritto, string.Empty, string.Empty);
                    foreach (CodiceRequisitiParticolari codiceRequisitiParticolari in liquidazioneCi.areaLiquidazionePensioneCi.listaCodiceRequisitiParticolari)
                    {
                        CodeUtility.SetValueDdl(ddlRequisitiParticolariDiritto, codiceRequisitiParticolari.Id + " - " + codiceRequisitiParticolari.Descrizione, codiceRequisitiParticolari.Descrizione, codiceRequisitiParticolari.Id);
                    }
                }
            }
        }

        private void ManageAnzianitaVecchiaia94_96(AreaTitolare.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            DateTime dataCompare = new DateTime(2009, 01, 01);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (((tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Anzianita || tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Vecchiaia) &&
                (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value < dataCompare)) ||
                (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione &&
                (areaLiquidazionePensione.IsPensioneAnzianitaOrRicostituzione.GetValueOrDefault() || areaLiquidazionePensione.IsPensioneVecchiaiaOrRicostituzione.GetValueOrDefault())))
                pnlAnzVecch.Visible = true;
        }

        private void VisualizzaPannelli(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                pnlSettimaneOBGMisura.Visible = true;
            }

            bool isNormale = (bool)ViewState["TipoGestione"];
            if (isNormale)
            {
                pnlGestioneNormale.Visible = true;
                pnlGestioneSpeciale.Visible = false;
            }
            else
            {
                pnlGestioneNormale.Visible = false;
                pnlGestioneSpeciale.Visible = true;
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();

        }

        private void SetDefaultValue()
        {
            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);

            txtInizioAssicurazione.Text = "GG/MM/AAAA";
            txtFineAssicurazione.Text = "GG/MM/AAAA";
            txtDecCodVirtuale.Text = "MM/AAAA";
        }

        #endregion Private Methods

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
    }
}