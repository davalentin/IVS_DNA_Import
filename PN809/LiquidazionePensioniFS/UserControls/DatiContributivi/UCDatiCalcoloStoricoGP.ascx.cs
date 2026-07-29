using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiCalcoloStoricoGP : CustomBaseUserControl, IDatiContributivi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility.BloccaForm(this.domanda, this);
        }

        internal void ValorizzaEtichette()
        {
            if (this.areaDatiContributivi != null)
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                RenderControlsFromTipoCalcolo_TipoFondo();

                ValorizzaEtichetteCommon();

                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        ValorizzaEtichetteEL_TT();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        ValorizzaEtichetteET();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        ValorizzaEtichetteEL_TT();
                        ValorizzaEtichetteTT();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        ValorizzaEtichetteDatiCalcoloVL();
                        break;
                }
            }
        }

        #region private methods
        private void ValorizzaEtichetteCommon()
        {
            txtImportoContributivoTotale.Text = areaDatiContributivi.DatiCalcoloStorico.ImportoContributivoTotale.HasValue ? areaDatiContributivi.DatiCalcoloStorico.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontante.Text = areaDatiContributivi.DatiCalcoloStorico.Montante.HasValue ? areaDatiContributivi.DatiCalcoloStorico.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimane.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimane.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimane.Value.ToString() : string.Empty;

            txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcoloStorico.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcoloStorico.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcoloStorico.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcoloStorico.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaDL214.Value.ToString() : string.Empty;

            txtRMSA.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaA.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaA.Value.ToString("0.0000") : string.Empty;
            txtRMSB.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaB.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaB.Value.ToString("0.0000") : string.Empty;
            txtSettimaneA.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaA.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaA.Value.ToString() : string.Empty;
            txtSettimaneB.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaB.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaB.Value.ToString() : string.Empty;
            txtRetribuzioneAgoAnnua.Text = areaDatiContributivi.DatiCalcoloStorico.RetribuzionePonderataAnnua.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RetribuzionePonderataAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            if (areaDatiContributivi.DatiCalcoloStorico.RiduzioneRetributiva)
                ddlRiduzioneRetributiva.SelectedValue = "SI";
            else
                ddlRiduzioneRetributiva.SelectedValue = "NO";
            txtRiduzioneRetributiva.Text = areaDatiContributivi.DatiCalcoloStorico.RiduzioneRetributivaPercentuale.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void ValorizzaEtichetteEL_TT()
        {
            lblTitoloDatiRetributivi.Text = "Decreto Legislativo 562";
            txtRMSD.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaD.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimaneC.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaC.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaC.Value.ToString() : string.Empty;
            txtSettimaneD.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaD.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaD.Value.ToString() : string.Empty;
            txtRMSD.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaD.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimaneD.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaD.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaD.Value.ToString() : string.Empty;

            txtQuotaAComma707.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaA707.Value.ToString() : string.Empty;
            txtQuotaBComma707.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaB707.Value.ToString() : string.Empty;
            txtQuotaCComma707.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaC707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaC707.Value.ToString() : string.Empty;
            txtQuotaDComma707.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaD707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaD707.Value.ToString() : string.Empty;

            txtRetribuzionePonderataComma707.Text = areaDatiContributivi.DatiCalcoloStorico.RetribuzionePonderataAGO707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RetribuzionePonderataAGO707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void ValorizzaEtichetteET()
        {
            if (areaDatiContributivi != null && areaDatiContributivi.DatiCalcoloStorico != null)
                switch (areaDatiContributivi.DatiCalcoloStorico.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:

                        lblTitoloDatiRetributivi.Text = "Decreto Legislativo 414";
                        List<GestioneContribDatiServizioUtile> lDatiServizioUtile = null;
                        if (areaDatiContributivi.DatiCalcoloStorico.fondoET == null || areaDatiContributivi.DatiCalcoloStorico.fondoET.lDatiServizioUtile == null || areaDatiContributivi.DatiCalcoloStorico.fondoET.lDatiServizioUtile.Count() == 0)
                        {
                            lDatiServizioUtile = new List<GestioneContribDatiServizioUtile>();
                            GestioneContribDatiServizioUtile DatiServizioUtile = new GestioneContribDatiServizioUtile();
                            DatiServizioUtile.Quota = "A";
                            lDatiServizioUtile.Add(DatiServizioUtile);
                            DatiServizioUtile = new GestioneContribDatiServizioUtile();
                            DatiServizioUtile.Quota = "B";
                            lDatiServizioUtile.Add(DatiServizioUtile);
                            DatiServizioUtile = new GestioneContribDatiServizioUtile();
                            DatiServizioUtile.Quota = "C";
                            lDatiServizioUtile.Add(DatiServizioUtile);
                        }
                        else
                            lDatiServizioUtile = areaDatiContributivi.DatiCalcoloStorico.fondoET.lDatiServizioUtile.ToList();

                        foreach (GestioneContribDatiServizioUtile servUtile in lDatiServizioUtile)
                        {
                            switch (servUtile.Quota)
                            {
                                case "A":
                                    txtServizioUtileAAQtaA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaA.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaA.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRetribPensionabileQtaA.Text = servUtile.RetribuzionePensionabile.HasValue ? servUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B":
                                    txtServizioUtileAAQtaB.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRetribPensionabileQtaB.Text = servUtile.RetribuzionePensionabile.HasValue ? servUtile.RetribuzionePensionabile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "C":
                                    txtServizioUtileAAQtaC.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaC.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaC.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    break;
                            }
                        }
                        break;
                }
        }

        private void ValorizzaEtichetteTT()
        {
            lblTitoloDatiRetributivi.Text = "Decreto Legislativo 658";
            txtRetribUltimoAnnoRetrib.Text = (areaDatiContributivi.DatiCalcoloStorico.fondoTT != null && areaDatiContributivi.DatiCalcoloStorico.fondoTT.RetribuzioneUltimoAnnoQuotaA.HasValue) ? areaDatiContributivi.DatiCalcoloStorico.fondoTT.RetribuzioneUltimoAnnoQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtRetribuzioneBiennio.Text = (areaDatiContributivi.DatiCalcoloStorico.fondoTT != null && areaDatiContributivi.DatiCalcoloStorico.fondoTT.RetribuzioneBiennio.HasValue) ? areaDatiContributivi.DatiCalcoloStorico.fondoTT.RetribuzioneBiennio.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void ValorizzaEtichetteDatiCalcoloVL()
        {
            txtRetribuzioneMediaSettADatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaA.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaA.Value.ToString("0.0000") : string.Empty;
            txtSettimaneA1DatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaA.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaA.Value.ToString() : string.Empty;
            txtSettimaneA2DatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaA2.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaA2.Value.ToString() : string.Empty;
            txtRetribuzioneMediaSettBDatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaB.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaB.Value.ToString("0.0000") : string.Empty;
            txtSettimaneBDatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaB.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaB.Value.ToString() : string.Empty;
            txtSettimaneC1DatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaC.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaC.Value.ToString() : string.Empty;
            txtSettimaneC2DatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaC2.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaC2.Value.ToString() : string.Empty;
            txtRetribuzioneMediaSettDDatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.RMSQuotaD.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RMSQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtSettimaneDDatiRetrib.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaD.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaD.Value.ToString() : string.Empty;

            txtImportTotale335_VL.Text = areaDatiContributivi.DatiCalcoloStorico.ImportoContributivoTotale.HasValue ? areaDatiContributivi.DatiCalcoloStorico.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

            txtMontante_VL.Text = areaDatiContributivi.DatiCalcoloStorico.Montante.HasValue ? areaDatiContributivi.DatiCalcoloStorico.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontanteDa0196a0697_VL.Text = areaDatiContributivi.DatiCalcoloStorico.MontanteAnte0697.HasValue ? areaDatiContributivi.DatiCalcoloStorico.MontanteAnte0697.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtA96_VL.Text = areaDatiContributivi.DatiCalcoloStorico.AnzianitaAnte0697AA.HasValue ? areaDatiContributivi.DatiCalcoloStorico.AnzianitaAnte0697AA.Value.ToString() : string.Empty;
            txtM96_VL.Text = areaDatiContributivi.DatiCalcoloStorico.AnzianitaAnte0697MM.HasValue ? areaDatiContributivi.DatiCalcoloStorico.AnzianitaAnte0697MM.Value.ToString() : string.Empty;
            txtG96_VL.Text = areaDatiContributivi.DatiCalcoloStorico.AnzianitaAnte0697GG.HasValue ? areaDatiContributivi.DatiCalcoloStorico.AnzianitaAnte0697GG.Value.ToString() : string.Empty;
            txtMontanteDa0697_VL.Text = areaDatiContributivi.DatiCalcoloStorico.Montante.HasValue ? areaDatiContributivi.DatiCalcoloStorico.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtA97_VL.Text = areaDatiContributivi.DatiCalcoloStorico.AnzianitaPost0697AA.HasValue ? areaDatiContributivi.DatiCalcoloStorico.AnzianitaPost0697AA.Value.ToString() : string.Empty;
            txtM97_VL.Text = areaDatiContributivi.DatiCalcoloStorico.AnzianitaPost0697MM.HasValue ? areaDatiContributivi.DatiCalcoloStorico.AnzianitaPost0697MM.Value.ToString() : string.Empty;
            txtG97_VL.Text = areaDatiContributivi.DatiCalcoloStorico.AnzianitaPost0697GG.HasValue ? areaDatiContributivi.DatiCalcoloStorico.AnzianitaPost0697GG.Value.ToString() : string.Empty;

            if (areaDatiContributivi.DatiCalcoloStorico.RiduzioneRetributiva)
                ddlRiduzioneRetributiva.SelectedValue = "SI";
            else
                ddlRiduzioneRetributiva.SelectedValue = "NO";
            txtRiduzioneRetributiva.Text = areaDatiContributivi.DatiCalcoloStorico.RiduzioneRetributivaPercentuale.HasValue ? areaDatiContributivi.DatiCalcoloStorico.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

            txtImportoContribTotaleQuotaDL214.Text = areaDatiContributivi.DatiCalcoloStorico.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcoloStorico.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtMontanteQuotaDL214.Text = areaDatiContributivi.DatiCalcoloStorico.MontanteQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcoloStorico.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtNSettimaneQuotaDL214.Text = areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaDL214.HasValue ? areaDatiContributivi.DatiCalcoloStorico.NSettimaneQuotaDL214.Value.ToString() : string.Empty;

            //Comma 707
            #region Comma 707
            txtQuotaA1Comma707VL.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaA707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaA707.Value.ToString() : string.Empty;
            txtQuotaA2Comma707VL.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaA2707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaA2707.Value.ToString() : string.Empty;
            txtQuotaBComma707VL.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaB707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaB707.Value.ToString() : string.Empty;
            txtQuotaC1Comma707VL.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaC707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaC707.Value.ToString() : string.Empty;
            txtQuotaC2Comma707VL.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaC2707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaC2707.Value.ToString() : string.Empty;
            txtQuotaDComma707VL.Text = areaDatiContributivi.DatiCalcoloStorico.QuotaD707.HasValue ? areaDatiContributivi.DatiCalcoloStorico.QuotaD707.Value.ToString() : string.Empty;
            #endregion Comma 707
        }

        private void RenderControlsFromTipoCalcolo_TipoFondo()
        {
            switch (areaDatiContributivi.DatiCalcoloStorico.TipoCalcolo)
            {
                #region Contributivo
                case GestioneContribTipoCalcolo.Contributivo:
                    pdivContributivo.Visible = true;
                    switch (this.domanda.Tipofondo)
                    {
                        case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            pnlDatiCalcoloContributiviLegge335_EL_TT_ET.Visible = true;
                            break;
                        case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            pnlDatiCalcoloContributiviLegge335_VL.Visible = true;
                            pnlDatiContributiviVLFelpe.Visible = true;
                            break;
                    }
                    pdivContributivoL214_Common.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                #endregion Contributivo
                #region Retributivo
                case GestioneContribTipoCalcolo.Retributivo:
                    pdivRetributivo.Visible = true;
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                            pnlDatiCalcoloRetributiviTT.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributiviET.Visible = true;
                            RenderControlsQuotaA();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            pnlDatiRetributiviVL.Visible = true;
                            pnlDatiRetributiviCustomVL.Visible = true;
                            break;
                    }
                    ManageRiduzioneRetributiva();
                    pdivContributivoL214_Common.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                #endregion Retributivo
                #region Misto
                case GestioneContribTipoCalcolo.Misto:
                    pdivRetributivo.Visible = true;
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                            pnlDatiCalcoloRetributiviTT.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributiviET.Visible = true;
                            RenderControlsQuotaA();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            pnlDatiRetributiviVL.Visible = true;
                            break;
                    }
                    rigaD.Visible = false;
                    ManageRiduzioneRetributiva();

                    pdivContributivo.Visible = true;
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            pnlDatiCalcoloContributiviLegge335_EL_TT_ET.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            pnlDatiCalcoloContributiviLegge335_VL.Visible = true;
                            break;
                    }

                    pdivContributivoL214_Common.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                #endregion Misto
                #region RetributivoMonti
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    pdivRetributivo.Visible = true;
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributivi_EL_TT.Visible = true;
                            pnlDatiCalcoloRetributiviTT.Visible = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            pnlDatiCalcoloRetributivi_EL_TT_ET.Visible = true;
                            pnlDatiCalcoloRetributiviET.Visible = true;
                            RenderControlsQuotaA();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            pnlDatiRetributiviVL.Visible = true;
                            pnlDatiRetributiviCustomVL.Visible = true;
                            break;
                    }
                    rigaD.Visible = true;

                    ManageRiduzioneRetributiva();

                    pdivContributivoL214_Common.Visible = this.areaDatiContributivi.IsContribL214Visible.HasValue ? this.areaDatiContributivi.IsContribL214Visible.Value : false;
                    break;
                #endregion RetributivoMonti
                case GestioneContribTipoCalcolo.NonValido:
                    break;
            }

            // Render Controls ex comma 707
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    if (areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
                    {
                        pdivComma707.Visible = true;
                        pnlComma707_EL_TT_ET.Visible = true;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    if (areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
                    {
                        pdivComma707.Visible = true;
                        pnlComma707_VL.Visible = true;
                    }
                    break;
            }
        }

        private void RenderControlsQuotaA()
        {
            if ((this.areaDatiContributivi.IsAnzianita.HasValue && this.areaDatiContributivi.IsAnzianita.Value) ||
                (this.areaDatiContributivi.IsInvaliditaSpecifica.HasValue && this.areaDatiContributivi.IsInvaliditaSpecifica.Value) ||
                (this.areaDatiContributivi.IsVecchiaiaSpecifica.HasValue && this.areaDatiContributivi.IsVecchiaiaSpecifica.Value))
                pnlRigaA.Visible = false;
        }

        private void ManageRiduzioneRetributiva()
        {
            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
                pnlRiduzioneRetributiva.Visible = true;
            else
                pnlRiduzioneRetributiva.Visible = false;
        }
        #endregion private methods
    }
}