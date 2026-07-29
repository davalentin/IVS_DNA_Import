using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCDatiCalcolo : CustomBaseUserControl, IDatiFondo, IDanteCausa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiFondo

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            if (areaDatiFondo != null && areaDatiFondo.DatiCalcolo != null)
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                //ENG - RIC REVERSIBILITA 024            
                if (this.areaDanteCausa == null)
                {
                    PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                    presenterDanteCausa.GetDatiDanteCausa(this);
                }

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

                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

                List<GestioneDatiServizioUtileServizioUtile> listaDatiServizioUtile = areaDatiFondo.DatiCalcolo.lDatiServizioUtile != null ? areaDatiFondo.DatiCalcolo.lDatiServizioUtile.ToList() : null;
                List<GestioneDatiServizioUtileINPDAPServizioUtile> listaDatiServizioUtileINPDAP = areaDatiFondo.DatiCalcolo.lDatiServizioUtileINPDAP != null ? areaDatiFondo.DatiCalcolo.lDatiServizioUtileINPDAP.ToList() : null;

                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;
                ViewState[EnumViewState.ContribDL214.ToString()] = areaDatiFondo.IsContribL214Visible;

                RenderControls(areaDatiFondo);

                /* Memorizzo l'informazione della PensioneAnnuaLorda e PensioneAnnuaLorda214 nel ViewState perchè, avendo cambiato la gestione della text PensioneAnnuaLorda per le 024 RIC con prima liquidata automatica,
                 * le informazione di questi campi andrebbero perse in fase di salvataggio  */
                ViewState[EnumViewState.PensioneAnnuaLorda.ToString()] = areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda.HasValue ? areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : null;
                ViewState[EnumViewState.PensioneAnnuaLorda214.ToString()] = areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda214.HasValue ? areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : null;

                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                    CodeUtility.IsRicostituzione(datiPensione) && datiPensione.IsPLUnicarpe.GetValueOrDefault() && !(CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                    txtPensioneAnnuaLorda.Text = areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda214.HasValue ? areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                else
                    txtPensioneAnnuaLorda.Text = areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda.HasValue ? areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                txtAnniServUtiliDirittoAA.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoAA.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoAA.Value.ToString() : string.Empty;
                txtAnniServUtiliDirittoMM.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoMM.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoMM.Value.ToString() : string.Empty;
                
                if (this.domanda.IsDomandaINPDAP && areaDatiFondo.DatiCalcolo.ServizioUtileDirittoAA.HasValue && areaDatiFondo.DatiCalcolo.ServizioUtileDirittoMM.HasValue)
                    txtAnniServUtiliDirittoGG.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoGG.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoGG.Value.ToString() : "0";
                else
                    txtAnniServUtiliDirittoGG.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoGG.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoGG.Value.ToString() : string.Empty;

                if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
                {
                    txtAnniServUtiliDirittoOIAA.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIAA.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIAA.Value.ToString() : string.Empty;
                    txtAnniServUtiliDirittoOIMM.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIMM.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIMM.Value.ToString() : string.Empty;
                    txtAnniServUtiliDirittoOIGG.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIGG.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIGG.Value.ToString() : string.Empty;
                }

                if ((areaDatiFondo.DatiCalcolo.TipoCalcolo != GestioneContribTipoCalcolo.Contributivo))
                {
                    if (listaDatiServizioUtile != null && listaDatiServizioUtile.Count() > 0)
                    {
                        foreach (GestioneDatiServizioUtileServizioUtile servUtile in listaDatiServizioUtile)
                        {
                            switch (servUtile.Quota)
                            {
                                case "A":
                                    txtServizioUtileAAQtaA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaA.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaA.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRetribuzioneQtaA.Text = servUtile.Retribuzione.HasValue ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtImpIndenIntegrSpecQtaA.Text = servUtile.ImportoIndennitaIntegrativaSpeciale.HasValue ? servUtile.ImportoIndennitaIntegrativaSpeciale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtQuotaRetributivaAnnua.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B1":
                                    txtServizioUtileAAQtaB1.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB1.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB1.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB94.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B2":
                                    txtServizioUtileAAQtaB2.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB2.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB2.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB95.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B3":
                                    txtServizioUtileAAQtaB3.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB3.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB3.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB97.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B4": // cessazione
                                    txtServizioUtileCessazioneAA.Text = servUtile.ServizioUtileCessazioneAA.HasValue ? servUtile.ServizioUtileCessazioneAA.Value.ToString() : string.Empty;
                                    txtServizioUtileCessazioneMM.Text = servUtile.ServizioUtileCessazioneMM.HasValue ? servUtile.ServizioUtileCessazioneMM.Value.ToString() : string.Empty;
                                    txtServizioUtileCessazioneGG.Text = servUtile.ServizioUtileCessazioneGG.HasValue ? servUtile.ServizioUtileCessazioneGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaCessazione.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                            }
                        }
                    }

                    if (listaDatiServizioUtileINPDAP != null && listaDatiServizioUtileINPDAP.Count() > 0)
                    {
                        foreach (GestioneDatiServizioUtileINPDAPServizioUtile servUtile in listaDatiServizioUtileINPDAP)
                        {
                            switch (servUtile.Quota)
                            {
                                case "A":
                                    txtServizioUtileAAQtaA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaA.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaA.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtRetribuzioneQtaA.Text = servUtile.Retribuzione.HasValue && servUtile.Retribuzione.Value != 0 ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtImpIndenIntegrSpecQtaA.Text = servUtile.ImportoIndennitaIntegrativaSpeciale.HasValue ? servUtile.ImportoIndennitaIntegrativaSpeciale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtQuotaRetributivaAnnua.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B1":
                                    txtServizioUtileAAQtaB1.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB1.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB1.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(txtRMSQtaB1.Text))
                                        txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue && servUtile.Retribuzione.Value != 0 ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB94.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B2":
                                    txtServizioUtileAAQtaB2.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB2.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB2.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB95.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    if (string.IsNullOrEmpty(txtRMSQtaB1.Text))
                                        txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue && servUtile.Retribuzione.Value != 0 ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B3":
                                    txtServizioUtileAAQtaB3.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB3.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB3.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB97.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    if (string.IsNullOrEmpty(txtRMSQtaB1.Text))
                                        txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue && servUtile.Retribuzione.Value != 0 ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B4": // cessazione
                                    txtServizioUtileCessazioneAA.Text = servUtile.ServizioUtileCessazioneAA.HasValue ? servUtile.ServizioUtileCessazioneAA.Value.ToString() : string.Empty;
                                    txtServizioUtileCessazioneMM.Text = servUtile.ServizioUtileCessazioneMM.HasValue ? servUtile.ServizioUtileCessazioneMM.Value.ToString() : string.Empty;
                                    txtServizioUtileCessazioneGG.Text = servUtile.ServizioUtileCessazioneGG.HasValue ? servUtile.ServizioUtileCessazioneGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaCessazione.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    if (string.IsNullOrEmpty(txtRMSQtaB1.Text))
                                        txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue && servUtile.Retribuzione.Value != 0 ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                                case "B5":
                                    txtServizioUtileAAQtaB5.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB5.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB5.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtQuotaPensioneRetributivaAnnuaB98.Text = servUtile.QuotaPensioneRetributivaAnnua.HasValue ? servUtile.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    if (string.IsNullOrEmpty(txtRMSQtaB1.Text))
                                        txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue && servUtile.Retribuzione.Value != 0 ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                    break;
                            }
                        }
                        if (string.IsNullOrEmpty(txtRMSQtaB1.Text))
                            txtRMSQtaB1.Text = "0";
                    }
                }

                switch (areaDatiFondo.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.Misto:
                        if (areaDatiFondo.IsContribL214Visible.GetValueOrDefault())
                        {
                            txtImportoContribTotaleQuotaDL214.Text = areaDatiFondo.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiFondo.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : (this.domanda.IsDomandaINPDAP ? "0" : string.Empty);
                            txtMontanteQuotaDL214.Text = areaDatiFondo.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiFondo.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : (this.domanda.IsDomandaINPDAP ? "0" : string.Empty);
                            txtNSettimaneQuotaDL214.Text = areaDatiFondo.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiFondo.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : (this.domanda.IsDomandaINPDAP ? "0" : string.Empty);
                            txtQuotaPensioneContributivaAnnuaDL214.Text = areaDatiFondo.DatiCalcolo.QuotaContributivaAnnua.HasValue ? areaDatiFondo.DatiCalcolo.QuotaContributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        }
                        txtImportoContributivoTotaleFS_PT.Text = areaDatiFondo.DatiCalcolo.ImportoContributivoTotale.HasValue ? areaDatiFondo.DatiCalcolo.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        txtSettimaneFS_PT.Text = areaDatiFondo.DatiCalcolo.NSettimane.HasValue ? areaDatiFondo.DatiCalcolo.NSettimane.Value.ToString() : string.Empty;
                        txtMontanteFS_PT.Text = areaDatiFondo.DatiCalcolo.Montante.HasValue ? areaDatiFondo.DatiCalcolo.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        txtImportoQuotaCFS_PT.Text = areaDatiFondo.DatiCalcolo.MontanteContributivo.HasValue ? areaDatiFondo.DatiCalcolo.MontanteContributivo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        //valorizza 335
                        break;
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        txtImportoContribTotaleQuotaDL214.Text = areaDatiFondo.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue ? areaDatiFondo.DatiCalcolo.ImportoContribTotaleQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : (this.domanda.IsDomandaINPDAP ? "0" : string.Empty);
                        txtMontanteQuotaDL214.Text = areaDatiFondo.DatiCalcolo.MontanteQuotaDL214.HasValue ? areaDatiFondo.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : (this.domanda.IsDomandaINPDAP ? "0" : string.Empty);
                        txtNSettimaneQuotaDL214.Text = areaDatiFondo.DatiCalcolo.NSettimaneQuotaDL214.HasValue ? areaDatiFondo.DatiCalcolo.NSettimaneQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : (this.domanda.IsDomandaINPDAP ? "0" : string.Empty);
                        txtQuotaPensioneContributivaAnnuaDL214.Text = areaDatiFondo.DatiCalcolo.QuotaContributivaAnnua.HasValue ? areaDatiFondo.DatiCalcolo.QuotaContributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        break;
                }

                if (Utility.IsDomandaReversibilita(datiPensione) && this.domanda.IsDomandaINPDAP && txtImportoContributivoTotaleFS_PT.Text == "")
                {
                    txtImportoContributivoTotaleFS_PT.Text = "0";
                }

                if (Utility.IsDomandaReversibilita(datiPensione) && this.domanda.IsDomandaINPDAP && txtImportoContribTotaleQuotaDL214.Text == "")
                {
                    txtImportoContribTotaleQuotaDL214.Text = "0";
                }

                if (Utility.IsDomandaReversibilita(datiPensione) && Utility.IsDomandaCTPS(this.domanda.Categoria) && txtSettimaneFS_PT.Text == "")
                {
                    txtSettimaneFS_PT.Text = "0";
                }

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.IsDomandaINPDAP)
                    txtRetribuzioneSenzaBenefici336.Text = areaDatiFondo.DatiCalcolo.RMSSenzaLegge33670QA.HasValue ? areaDatiFondo.DatiCalcolo.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                //bloccato per tutte le automatiche
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    txtRetribuzioneSenzaBenefici336.Enabled = false;

                if (this.domanda.IsDomandaINPDAP)
                {
                    LoadDdlCommon(areaDatiFondo, datiPensione);
                    if ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG) || Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                    {
                        if (Utility.IsDomandaCTPS(this.domanda.Categoria))
                            txtDivisore.Text = "12";
                        else
                            txtDivisore.Text = "13";
                    }
                    else
                    {
                        txtDivisore.Text = areaDatiFondo.DatiCalcolo.Divisore.HasValue ? areaDatiFondo.DatiCalcolo.Divisore.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    if (Utility.IsDomandaCPDEL(this.domanda.Categoria))
                        txtDivisore.Enabled = false;

                    if (!string.IsNullOrEmpty(areaDatiFondo.DatiCalcolo.Capitolo) && ddlCapitolo.Items.FindByValue(areaDatiFondo.DatiCalcolo.Capitolo) != null)
                        ddlCapitolo.SelectedValue = areaDatiFondo.DatiCalcolo.Capitolo;
                    //prepopoliamo solo per le PL CPDEL (no PL di REV) il campo “Capitolo” con il valore 092
                    else if (Utility.IsDomandaCPDEL(this.domanda.Categoria) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && !Utility.IsDomandaReversibilita(datiPensione))
                    {
                        ListItem selectedListItem = ddlCapitolo.Items.FindByValue("092");
                        if (selectedListItem != null) selectedListItem.Selected = true;
                    }
                    else
                        ddlCapitolo.SelectedIndex = 0;
                    //txtCapitolo.Text = string.IsNullOrEmpty(areaDatiFondo.DatiCalcolo.Capitolo) ? string.Empty : areaDatiFondo.DatiCalcolo.Capitolo;
                }
                if (this.domanda.IsDomandaINPDAP || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                    txtCoefficienteTrasformazione.Text = areaDatiFondo.DatiCalcolo.CoefficienteTrasformazione.HasValue ? areaDatiFondo.DatiCalcolo.CoefficienteTrasformazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                    Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && !this.domanda.IsDomandaINPDAP)
                {
                    RfvTxtRetribuzioneQtaA.Enabled = true;
                    RfvTxtRMSQtaB1.Enabled = true;
                    RfvTxtServizioUtileAAQtaA.Enabled = true;
                    RfvTxtServizioUtileAAQtaB1.Enabled = true;
                    RfvTxtServizioUtileGGQtaA.Enabled = true;
                    RfvTxtServizioUtileGGQtaB1.Enabled = true;
                    RfvTxtServizioUtileMMQtaA.Enabled = true;
                    RfvTxtServizioUtileMMQtaB1.Enabled = true;
                    RfvTxtServizioUtileGGQtaB2.Enabled = true;
                    RfvTxtServizioUtileMMQtaB2.Enabled = true;
                    RfVTxtServizioUtileAAQtaB2.Enabled = true;
                    RfvTxtServizioUtileGGQtaB3.Enabled = true;
                    RfvTxtServizioUtileMMQtaB3.Enabled = true;
                    RfvTxtServizioUtileAAQtaB3.Enabled = true;
                    RfvTxtServizioUtileCessazioneAA.Enabled = true;
                    RfvTxtServizioUtileCessazioneGG.Enabled = true;
                    RfvTxtServizioUtileCessazioneMM.Enabled = true;
                    RfvTxtMontanteFS_PT.Enabled = true;
                    RfvTxtImportoQuotaCFS_PT.Enabled = true;
                    RfvTxtQuotaPensioneContributivaAnnuaDL214.Enabled = true;
                    RfvtxtImpIndenIntegrSpecQtaA.Enabled = true;
                }

                if (this.domanda.IsDomandaINPDAP && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && tipologiaProdottoPensione != CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
                {
                    RfvTxtMontanteFS_PT.Enabled = true;
                    RfvTxtImportoQuotaCFS_PT.Enabled = true;
                    RfvTxtQuotaPensioneContributivaAnnuaDL214.Enabled = true;
                }

                if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                    (CodeUtility.IsRicostituzione(datiPensione) && !datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                    (areaDatiFondo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Contributivo && this.domanda.Categoria.StartsWith("I")) ||
                    ((areaDatiFondo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Contributivo || areaDatiFondo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Misto) &&
                     areaDatiFondo.IsDomandaSperimentaleDonna.GetValueOrDefault()) ||
                    (areaDatiFondo.IsPensioneTipoContributivo.GetValueOrDefault() || areaDatiFondo.IsPensioneTipoContributivoConOpzione.GetValueOrDefault() || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                    datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo) || (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024) && controlloDinamicoMemo123_2024.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                    (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024OpzioneContrib) && controlloDinamicoMemo123_2024OpzioneContrib.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                    (CodeUtility.IsRicostituzione(datiPensione) && !datiPensione.IsPLUnicarpe.GetValueOrDefault() && areaDatiFondo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Misto))
                {
                    RfvTxtImportoContributivoTotaleFS_PT.Enabled = false;
                    RfvTxtImportoContribTotaleQuotaDL214.Enabled = false;
                    RfvTxtSettimaneFS_PT.Enabled = false;
                    RfvTxtNSettimaneQuotaDL214.Enabled = false;
                    RfvTxtMontanteQuotaDL214.Enabled = false;
                }

                //rimosso con revisione 2.1
                //if (Utility.IsDomandaRipristino(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.IsDomandaINPDAP))
                //{
                //    RfvTxtRMSQtaB1.Enabled = false;
                //    RfvTxtServizioUtileAAQtaA.Enabled = false;
                //    RfvTxtServizioUtileGGQtaA.Enabled = false;
                //    RfvTxtServizioUtileMMQtaA.Enabled = false;
                //    RfvTxtServizioUtileGGQtaB1.Enabled = false;
                //    RfvTxtServizioUtileAAQtaB1.Enabled = false;
                //    RfvTxtServizioUtileMMQtaB1.Enabled = false;
                //    RfvTxtServizioUtileGGQtaB2.Enabled = false;
                //    RfvTxtServizioUtileMMQtaB2.Enabled = false;
                //    RfVTxtServizioUtileAAQtaB2.Enabled = false;
                //    RfvTxtServizioUtileGGQtaB3.Enabled = false;
                //    RfvTxtServizioUtileMMQtaB3.Enabled = false;
                //    RfvTxtServizioUtileAAQtaB3.Enabled = false;
                //    RfvTxtServizioUtileCessazioneAA.Enabled = false;
                //    RfvTxtServizioUtileCessazioneGG.Enabled = false;
                //    RfvTxtServizioUtileCessazioneMM.Enabled = false;
                //}

                if (Utility.IsDomandaSalvaguardia178_2020(datiPensione))
                {
                    RfvTxtServizioUtileGGQtaB3.Enabled = true;
                    RfvTxtServizioUtileMMQtaB3.Enabled = true;
                    RfvTxtServizioUtileAAQtaB3.Enabled = true;
                    RfvTxtServizioUtileCessazioneAA.Enabled = true;
                    RfvTxtServizioUtileCessazioneGG.Enabled = true;
                    RfvTxtServizioUtileCessazioneMM.Enabled = true;
                }

                if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita && this.domanda.IsDomandaINPDAP)
                {
                    CodeUtility.BloccaForm((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], pnlUCDatiCalcolo);
                    btnEliminaDatiCalcolo.Enabled = false;
                }

                //ENG - Reversibilita Manuali 024
                //ENG - Indirette Manuali 024
                if ((Utility.IsDomandaReversibilita(datiPensione) || Utility.IsDomandaIndiretta(datiPensione))
                   && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                    && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                {
                    RfvTxtRMSQtaB1.Enabled = false;

                    RfvTxtServizioUtileGGQtaB1.Enabled = false;
                    RfvTxtServizioUtileMMQtaB1.Enabled = false;
                    RfvTxtServizioUtileAAQtaB1.Enabled = false;

                    RfvTxtServizioUtileGGQtaB2.Enabled = false;
                    RfvTxtServizioUtileMMQtaB2.Enabled = false;
                    RfVTxtServizioUtileAAQtaB2.Enabled = false;

                    RfvTxtServizioUtileGGQtaB3.Enabled = false;
                    RfvTxtServizioUtileMMQtaB3.Enabled = false;
                    RfvTxtServizioUtileAAQtaB3.Enabled = false;

                    RfvTxtServizioUtileCessazioneAA.Enabled = false;
                    RfvTxtServizioUtileCessazioneGG.Enabled = false;
                    RfvTxtServizioUtileCessazioneMM.Enabled = false;
                }

                //ENG - 024/GDP - RIC CONCESSIONE ALTRA PENSIONE
                if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && (this.domanda.IsDomandaINPDAP || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    pnlUCDatiCalcolo.Enabled = false;
                    btnEliminaDatiCalcolo.Enabled = false;
                }

                //ENG - PL Reversibilita 024
                //ENG - RIC REVERSIBILITA 024
                if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, areaDanteCausa, this.domanda.Categoria, areaDatiFondo != null ? areaDatiFondo.TipoReversibilita : null, this.domanda.Tipofondo)
                    && !this.domanda.IsDomandaRiapertura && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    if (Utility.IsDomandaReversibilita(datiPensione))
                    {
                        RfvTxtRetribuzioneQtaA.Enabled = false;
                        RfvtxtImpIndenIntegrSpecQtaA.Enabled = false;
                        RfvTxtRMSQtaB1.Enabled = false;
                        RfvTxtServizioUtileAAQtaA.Enabled = false;
                        RfvTxtServizioUtileMMQtaA.Enabled = false;
                        RfvTxtServizioUtileGGQtaA.Enabled = false;

                        if (areaDatiFondo != null && areaDatiFondo.DatiCalcolo != null)
                        {
                            ViewState["IsPensioneAnnuaLordaDaPrelievo"] = areaDatiFondo.DatiCalcolo.IsPensioneAnnuaLordaDaPrelievo;

                            if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                            {
                                if (areaDatiFondo.DatiCalcolo.IsPensioneAnnuaLordaDaPrelievo.GetValueOrDefault())
                                    txtPensioneAnnuaLorda.Enabled = false;
                                else
                                    txtPensioneAnnuaLorda.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzioneVariazioneDatiContitolari(datiPensione))
                            txtPensioneAnnuaLorda.Enabled = false;
                        else
                            txtPensioneAnnuaLorda.Enabled = true;
                    }

                    RfvTxtImportoContributivoTotaleFS_PT.Enabled = false;
                    RfvTxtSettimaneFS_PT.Enabled = false;
                    RfvTxtMontanteFS_PT.Enabled = false;
                    RfvTxtImportoQuotaCFS_PT.Enabled = false;

                    RfvTxtImportoContribTotaleQuotaDL214.Enabled = false;
                    RfvTxtNSettimaneQuotaDL214.Enabled = false;
                    RfvTxtMontanteQuotaDL214.Enabled = false;
                    RfvTxtQuotaPensioneContributivaAnnuaDL214.Enabled = false;

                }

                //ENG - PL VPT/VFS Manuali: resi non obbligatori i campi della quota B e cessazione
                if (!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim().ToUpperInvariant().StartsWith("V") &&
                   (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                    !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                {
                    if (areaDatiFondo.DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.Retributivo && areaDatiFondo.FineAssicurazione.HasValue &&
                        !Utility.DataStrettamenteSuccessivaA(areaDatiFondo.FineAssicurazione.Value, new DateTime(1997, 12, 31)))
                    {
                        //Quota B1
                        RfvTxtServizioUtileAAQtaB1.Enabled = false;
                        RfvTxtServizioUtileMMQtaB1.Enabled = false;
                        RfvTxtServizioUtileGGQtaB1.Enabled = false;
                        RfvTxtRMSQtaB1.Enabled = false;

                        //Quota B2
                        RfVTxtServizioUtileAAQtaB2.Enabled = false;
                        RfvTxtServizioUtileMMQtaB2.Enabled = false;
                        RfvTxtServizioUtileGGQtaB2.Enabled = false;

                        //Quota B3
                        RfvTxtServizioUtileAAQtaB3.Enabled = false;
                        RfvTxtServizioUtileMMQtaB3.Enabled = false;
                        RfvTxtServizioUtileGGQtaB3.Enabled = false;

                        //Quota Cessazione (B4)
                        RfvTxtServizioUtileCessazioneAA.Enabled = false;
                        RfvTxtServizioUtileCessazioneMM.Enabled = false;
                        RfvTxtServizioUtileCessazioneGG.Enabled = false;
                    }
                }
                //ENG - Memo 79
                pnlNSettimane_OrganizzazioniInternazionali.Visible = Utility.IsDomandaOrganizzazioniInternazionali(datiPensione);
                if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
                    SetSettimaneTotali();
            }
        }

        private void LoadDdlCommon(AreaDatiFondo areaDatiFondo, AreaTitolare.DatiPensione datiPensione)
        {
            ddlCapitolo.Items.Clear();
            ddlCapitolo.Items.Add(new ListItem(string.Empty, " "));
            foreach (DecCapitolo decCapitolo in areaDatiFondo.DatiCalcolo.lDecCapitolo)
                CodeUtility.SetValueDdl(ddlCapitolo, decCapitolo.Capitolo + " - " + decCapitolo.DescrizioneCapitolo, decCapitolo.DescrizioneCapitolo, decCapitolo.Capitolo);
        }

        internal DatiCalcolo RecuperaCampi()
        {
            bool isContributivoL214 = false;
            bool isContributivoL335 = false;
            bool isRetributivo = false;

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiCalcolo = new DatiCalcolo();

            this.areaDatiFondo.IsContribL214Visible = (bool?)ViewState[EnumViewState.ContribDL214.ToString()];
            this.areaDatiFondo.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontanteFS_PT.Text) ? Convert.ToDecimal(txtMontanteFS_PT.Text) : (decimal?)null;

            // Per le domande di RIC o Riapertura, il campo non è obbligatorio e può essere lasciato vuoto anche nel caso di tipo calcolo contributivo, per questo motivo viene considerata solo la visibilità
            if (this.areaDatiFondo.DatiCalcolo.Montante.HasValue || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && txtMontanteFS_PT.Visible))
                isContributivoL335 = true;

            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                && CodeUtility.IsRicostituzione(datiPensione) && datiPensione.IsPLUnicarpe.GetValueOrDefault() && !(CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
            {
                this.areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda214 = !string.IsNullOrEmpty(txtPensioneAnnuaLorda.Text) ? Convert.ToDecimal(txtPensioneAnnuaLorda.Text) : (decimal?)null;
                this.areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda = ViewState[EnumViewState.PensioneAnnuaLorda.ToString()] != null && ViewState[EnumViewState.PensioneAnnuaLorda.ToString()].ToString().Trim() != String.Empty ? CodeUtility.StringToNullableDecimal(ViewState[EnumViewState.PensioneAnnuaLorda.ToString()].ToString()) : (decimal?)null;
            }
            else
            {
                this.areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda = !string.IsNullOrEmpty(txtPensioneAnnuaLorda.Text) ? Convert.ToDecimal(txtPensioneAnnuaLorda.Text) : (decimal?)null;
                this.areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda214 = ViewState[EnumViewState.PensioneAnnuaLorda214.ToString()] != null && ViewState[EnumViewState.PensioneAnnuaLorda214.ToString()].ToString().Trim() != String.Empty ? CodeUtility.StringToNullableDecimal(ViewState[EnumViewState.PensioneAnnuaLorda214.ToString()].ToString()) : (decimal?)null;
            }
            this.areaDatiFondo.DatiCalcolo.ServizioUtileDirittoAA = !string.IsNullOrEmpty(txtAnniServUtiliDirittoAA.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoAA.Text) : (short?)null;
            this.areaDatiFondo.DatiCalcolo.ServizioUtileDirittoMM = !string.IsNullOrEmpty(txtAnniServUtiliDirittoMM.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoMM.Text) : (short?)null;
            this.areaDatiFondo.DatiCalcolo.ServizioUtileDirittoGG = !string.IsNullOrEmpty(txtAnniServUtiliDirittoGG.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoGG.Text) : (short?)null;
            if (Utility.IsDomandaOrganizzazioniInternazionali(datiPensione))
            {
                this.areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIAA = !string.IsNullOrEmpty(txtAnniServUtiliDirittoOIAA.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoOIAA.Text) : (short?)null;
                this.areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIMM = !string.IsNullOrEmpty(txtAnniServUtiliDirittoOIMM.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoOIMM.Text) : (short?)null;
                this.areaDatiFondo.DatiCalcolo.ServizioUtileDirittoOIGG = !string.IsNullOrEmpty(txtAnniServUtiliDirittoOIGG.Text) ? Convert.ToInt16(txtAnniServUtiliDirittoOIGG.Text) : (short?)null;
            }

            this.areaDatiFondo.DatiCalcolo.RMSSenzaLegge33670QA = !string.IsNullOrEmpty(txtRetribuzioneSenzaBenefici336.Text) ? Convert.ToDecimal(txtRetribuzioneSenzaBenefici336.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.CoefficienteTrasformazione = !string.IsNullOrEmpty(txtCoefficienteTrasformazione.Text) ? Convert.ToDecimal(txtCoefficienteTrasformazione.Text) : (decimal?)null;
            if (this.domanda.IsDomandaINPDAP)
            {
                List<GestioneDatiServizioUtileINPDAPServizioUtile> lDatiServUtile = new List<GestioneDatiServizioUtileINPDAPServizioUtile>();
                GestioneDatiServizioUtileINPDAPServizioUtile datiServUtile = null;

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ||
                   !String.IsNullOrEmpty(txtRetribuzioneQtaA.Text) || !String.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                    datiServUtile.Quota = "A";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) ? Convert.ToInt16(txtServizioUtileAAQtaA.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileMMQtaA.Text) : (byte?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileGGQtaA.Text) : (byte?)null;
                    datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRetribuzioneQtaA.Text) ? Convert.ToDecimal(txtRetribuzioneQtaA.Text) : (decimal?)null;
                    datiServUtile.ImportoIndennitaIntegrativaSpeciale = !string.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text) ? Convert.ToDecimal(txtImpIndenIntegrSpecQtaA.Text) : (decimal?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaRetributivaAnnua.Text) ? Convert.ToDecimal(txtQuotaRetributivaAnnua.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ||
                    !String.IsNullOrEmpty(txtRMSQtaB1.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                    datiServUtile.Quota = "B1";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB1.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB1.Text) : (byte?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB1.Text) : (byte?)null;
                    datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRMSQtaB1.Text) ? Convert.ToDecimal(txtRMSQtaB1.Text) : (decimal?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB94.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB94.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                    datiServUtile.Quota = "B2";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB2.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB2.Text) : (byte?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB2.Text) : (byte?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB95.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB95.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                    datiServUtile.Quota = "B3";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB3.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB3.Text) : (byte?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB3.Text) : (byte?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB97.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB97.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                    datiServUtile.Quota = "B4";
                    datiServUtile.ServizioUtileCessazioneAA = !string.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) ? Convert.ToInt16(txtServizioUtileCessazioneAA.Text) : (short?)null;
                    datiServUtile.ServizioUtileCessazioneMM = !string.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileCessazioneMM.Text) : (byte?)null;
                    datiServUtile.ServizioUtileCessazioneGG = !string.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileCessazioneGG.Text) : (byte?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaCessazione.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaCessazione.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB5.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB5.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB5.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                    datiServUtile.Quota = "B5";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB5.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB5.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB5.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB5.Text) : (byte?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB5.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB5.Text) : (byte?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB98.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB98.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
                {
                    this.areaDatiFondo.DatiCalcolo.lDatiServizioUtileINPDAP = lDatiServUtile.ToArray();
                    isRetributivo = true;
                }

                if (!string.IsNullOrEmpty(txtDivisore.Text))
                    this.areaDatiFondo.DatiCalcolo.Divisore = CodeUtility.StringToNullableByte(txtDivisore.Text);
                //if (!string.IsNullOrEmpty(txtCapitolo.Text))
                //    this.areaDatiFondo.DatiCalcolo.Capitolo = txtCapitolo.Text;
                this.areaDatiFondo.DatiCalcolo.Capitolo = !String.IsNullOrEmpty(ddlCapitolo.SelectedValue) ? ddlCapitolo.SelectedValue : null;
            }
            else
            {
                List<GestioneDatiServizioUtileServizioUtile> lDatiServUtile = new List<GestioneDatiServizioUtileServizioUtile>();
                GestioneDatiServizioUtileServizioUtile datiServUtile = null;

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ||
                   !String.IsNullOrEmpty(txtRetribuzioneQtaA.Text) || !String.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileServizioUtile();
                    datiServUtile.Quota = "A";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) ? Convert.ToInt16(txtServizioUtileAAQtaA.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) ? Convert.ToInt16(txtServizioUtileMMQtaA.Text) : (short?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ? Convert.ToInt16(txtServizioUtileGGQtaA.Text) : (short?)null;
                    datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRetribuzioneQtaA.Text) ? Convert.ToDecimal(txtRetribuzioneQtaA.Text) : (decimal?)null;
                    datiServUtile.ImportoIndennitaIntegrativaSpeciale = !string.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text) ? Convert.ToDecimal(txtImpIndenIntegrSpecQtaA.Text) : (decimal?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaRetributivaAnnua.Text) ? Convert.ToDecimal(txtQuotaRetributivaAnnua.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ||
                    !String.IsNullOrEmpty(txtRMSQtaB1.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileServizioUtile();
                    datiServUtile.Quota = "B1";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB1.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB1.Text) : (short?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB1.Text) : (short?)null;
                    datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRMSQtaB1.Text) ? Convert.ToDecimal(txtRMSQtaB1.Text) : (decimal?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB94.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB94.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileServizioUtile();
                    datiServUtile.Quota = "B2";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB2.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB2.Text) : (short?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB2.Text) : (short?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB95.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB95.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileServizioUtile();
                    datiServUtile.Quota = "B3";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB3.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB3.Text) : (short?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB3.Text) : (short?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaB97.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaB97.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (!String.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text))
                {
                    datiServUtile = new GestioneDatiServizioUtileServizioUtile();
                    datiServUtile.Quota = "B4";
                    datiServUtile.ServizioUtileCessazioneAA = !string.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) ? Convert.ToInt16(txtServizioUtileCessazioneAA.Text) : (short?)null;
                    datiServUtile.ServizioUtileCessazioneMM = !string.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) ? Convert.ToInt16(txtServizioUtileCessazioneMM.Text) : (short?)null;
                    datiServUtile.ServizioUtileCessazioneGG = !string.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text) ? Convert.ToInt16(txtServizioUtileCessazioneGG.Text) : (short?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneRetributivaAnnuaCessazione.Text) ? Convert.ToDecimal(txtQuotaPensioneRetributivaAnnuaCessazione.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }

                if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
                {
                    this.areaDatiFondo.DatiCalcolo.lDatiServizioUtile = lDatiServUtile.ToArray();
                    isRetributivo = true;
                }
            }

            RecuperaCampiContributivoLegge335();
            if (this.areaDatiFondo.IsContribL214Visible.HasValue && this.areaDatiFondo.IsContribL214Visible.Value)
                RecuperaCampiContributivoLegge214();

            if (this.areaDatiFondo.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue || this.areaDatiFondo.DatiCalcolo.MontanteQuotaDL214.HasValue || this.areaDatiFondo.DatiCalcolo.NSettimaneQuotaDL214.HasValue ||
                this.areaDatiFondo.DatiCalcolo.QuotaContributivaAnnua.HasValue)
                isContributivoL214 = true;

            if (this.areaDatiFondo.DatiCalcolo.ImportoContributivoTotale.HasValue || this.areaDatiFondo.DatiCalcolo.Montante.HasValue || this.areaDatiFondo.DatiCalcolo.MontanteContributivo.HasValue ||
                this.areaDatiFondo.DatiCalcolo.NSettimane.HasValue)
                isContributivoL335 = true;

            if (isContributivoL335 && isRetributivo)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Misto;
            else if (!isContributivoL335 && isContributivoL214 && isRetributivo)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.RetributivoMonti;
            else if (isContributivoL335 || isContributivoL214)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Contributivo;
            else if (isRetributivo)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Retributivo;
            else
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.NonValido;

            //ENG - PL Reversibilita 024 
            if (Utility.IsDomandaReversibilita(datiPensione) && !this.domanda.IsDomandaRiapertura && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                if (ViewState["IsPensioneAnnuaLordaDaPrelievo"] != null)
                {
                    this.areaDatiFondo.DatiCalcolo.IsPensioneAnnuaLordaDaPrelievo = (bool)ViewState["IsPensioneAnnuaLordaDaPrelievo"];
                }
            }


            return this.areaDatiFondo.DatiCalcolo;
        }

        private void RecuperaCampiContributivoLegge214()
        {
            this.areaDatiFondo.DatiCalcolo.ImportoContribTotaleQuotaDL214 = !string.IsNullOrEmpty(txtImportoContribTotaleQuotaDL214.Text) ? Convert.ToDecimal(txtImportoContribTotaleQuotaDL214.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.MontanteQuotaDL214 = !string.IsNullOrEmpty(txtMontanteQuotaDL214.Text) ? Convert.ToDecimal(txtMontanteQuotaDL214.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.NSettimaneQuotaDL214 = !string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text) ? Convert.ToInt32(txtNSettimaneQuotaDL214.Text) : (int?)null;
            this.areaDatiFondo.DatiCalcolo.QuotaContributivaAnnua = !string.IsNullOrEmpty(txtQuotaPensioneContributivaAnnuaDL214.Text) ? Convert.ToDecimal(txtQuotaPensioneContributivaAnnuaDL214.Text) : (decimal?)null;
        }

        private void RecuperaCampiContributivoLegge335()
        {
            this.areaDatiFondo.DatiCalcolo.ImportoContributivoTotale = !string.IsNullOrEmpty(txtImportoContributivoTotaleFS_PT.Text) ? Convert.ToDecimal(txtImportoContributivoTotaleFS_PT.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontanteFS_PT.Text) ? Convert.ToDecimal(txtMontanteFS_PT.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.MontanteContributivo = !string.IsNullOrEmpty(txtImportoQuotaCFS_PT.Text) ? Convert.ToDecimal(txtImportoQuotaCFS_PT.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.NSettimane = !string.IsNullOrEmpty(txtSettimaneFS_PT.Text) ? Convert.ToInt32(txtSettimaneFS_PT.Text) : (int?)null;
        }


        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();
            SetSettimaneTotali();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.StoreDatiCalcoloByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiCalcolo(this, null);
            }
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.EliminaDatiCalcoloByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiCalcolo(this, null);
                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiCalcolo;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiCalcolo(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiCalcolo != null)
                UpdateSemaforoDatiCalcolo(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            if (HidePulsanteSalva != null)
                HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            if (TornaARegistrazioniFondo != null)
                TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers

        #region private methods
        private void RenderControls(AreaDatiFondo areaDatiFondo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda.IsDomandaINPDAP)
            {
                if (!Utility.IsDomandaCPDEL(this.domanda.Categoria))
                    pnlDatiPost97.Visible = true;
                trDivisore.Visible = true;
                trCapitolo.Visible = true;
                lblCessazione.Text = "Dati al 31/12/2011";

                if (Utility.IsRicostituzione_ProvenienteDaListePensioniDaVerificare(datiPensione) || Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) || Utility.IsRicostituzioneVariazioneDatiContitolari(datiPensione))
                    pnlUCDatiCalcolo.Enabled = false;
            }

            if (areaDatiFondo != null)
            {
                if (areaDatiFondo.DatiCalcolo != null)
                {
                    switch (areaDatiFondo.DatiCalcolo.TipoCalcolo)
                    {
                        case GestioneContribTipoCalcolo.Retributivo:
                            pnlDatiRetributivi.Visible = true;
                            pnlDatiContributiviFS_PT.Visible = false;
                            break;
                        case GestioneContribTipoCalcolo.Contributivo:
                            pnlDatiRetributivi.Visible = false;
                            pnlDatiContributiviFS_PT.Visible = true;
                            break;
                        case GestioneContribTipoCalcolo.Misto:
                            pnlDatiRetributivi.Visible = true;
                            pnlDatiContributiviFS_PT.Visible = true;
                            break;
                        case GestioneContribTipoCalcolo.RetributivoMonti:
                            pnlDatiRetributivi.Visible = true;
                            break;
                    }

                    //Gestione da FELPE DL214,DL335
                    //AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        txtPensioneAnnuaLorda.Enabled = false;
                        txtAnniServUtiliDirittoAA.Enabled = false;
                        txtAnniServUtiliDirittoMM.Enabled = false;
                        txtAnniServUtiliDirittoGG.Enabled = false;
                        if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN)
                        {
                            txtDivisore.Enabled = false;
                            ddlCapitolo.Enabled = false;
                        }
                        else if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
                        {
                            txtDivisore.Enabled = false;
                            ddlCapitolo.Enabled = true;
                        }

                        tdLblCoefficienteTrasformazione.Visible = true;
                        tdTxtCoefficienteTrasformazione.Visible = true;
                        tdNoCoefficienteTrasformazione.Visible = false;
                        txtCoefficienteTrasformazione.Enabled = false;

                        //Quota A
                        txtServizioUtileAAQtaA.Enabled = false;
                        txtServizioUtileMMQtaA.Enabled = false;
                        txtServizioUtileGGQtaA.Enabled = false;
                        txtRetribuzioneQtaA.Enabled = false;
                        txtQuotaRetributivaAnnua.Enabled = false;
                        pnlQuotaRetributivaAnnua.Visible = true;
                        if (!this.domanda.IsDomandaINPDAP)
                            txtImpIndenIntegrSpecQtaA.Enabled = false;
                        //Quota B1
                        txtServizioUtileAAQtaB1.Enabled = false;
                        txtServizioUtileMMQtaB1.Enabled = false;
                        txtServizioUtileGGQtaB1.Enabled = false;
                        txtRMSQtaB1.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB94.Enabled = false;
                        pnlQuotaPensioneRetributivaAnnuaB94.Visible = true;
                        //Quota B2
                        txtServizioUtileAAQtaB2.Enabled = false;
                        txtServizioUtileMMQtaB2.Enabled = false;
                        txtServizioUtileGGQtaB2.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB95.Enabled = false;
                        pnlQuotaPensioneRetributivaAnnuaB95.Visible = true;
                        if (!Utility.IsDomandaSalvaguardia178_2020(datiPensione))
                        {
                            //Quota B3
                            txtServizioUtileAAQtaB3.Enabled = false;
                            txtServizioUtileMMQtaB3.Enabled = false;
                            txtServizioUtileGGQtaB3.Enabled = false;
                            txtQuotaPensioneRetributivaAnnuaB97.Enabled = false;
                            pnlQuotaPensioneRetributivaAnnuaB97.Visible = true;
                            //Quota B4 - Cessazione
                            txtServizioUtileCessazioneAA.Enabled = false;
                            txtServizioUtileCessazioneMM.Enabled = false;
                            txtServizioUtileCessazioneGG.Enabled = false;
                            txtQuotaPensioneRetributivaAnnuaCessazione.Enabled = false;
                            pnlQuotaPensioneRetributivaAnnuaCessazione.Visible = true;
                        }
                        //Quota B5
                        txtServizioUtileAAQtaB5.Enabled = false;
                        txtServizioUtileMMQtaB5.Enabled = false;
                        txtServizioUtileGGQtaB5.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB98.Enabled = false;
                        pnlQuotaPensioneRetributivaAnnuaB98.Visible = true;
                        //Quota C - DL335
                        txtImportoContributivoTotaleFS_PT.Enabled = false;
                        txtSettimaneFS_PT.Enabled = false;
                        txtMontanteFS_PT.Enabled = false;
                        txtImportoQuotaCFS_PT.Enabled = false;
                        RfvTxtImportoContributivoTotaleFS_PT.Enabled = false;
                        RfvTxtSettimaneFS_PT.Enabled = false;
                        RfvTxtMontanteFS_PT.Enabled = false;
                        RfvTxtImportoQuotaCFS_PT.Enabled = false;
                        //Quota D - DL214
                        txtImportoContribTotaleQuotaDL214.Enabled = false;
                        txtNSettimaneQuotaDL214.Enabled = false;
                        txtMontanteQuotaDL214.Enabled = false;
                        txtQuotaPensioneContributivaAnnuaDL214.Enabled = false;
                        RfvTxtImportoContribTotaleQuotaDL214.Enabled = false;
                        RfvTxtNSettimaneQuotaDL214.Enabled = false;
                        RfvTxtMontanteQuotaDL214.Enabled = false;
                        RfvTxtQuotaPensioneContributivaAnnuaDL214.Enabled = false;
                    }

                    if (datiPensione.IsPLUnicarpe.GetValueOrDefault() ||
                        this.domanda.IsDomandaINPDAP)
                    {
                        tdLblCoefficienteTrasformazione.Visible = true;
                        tdTxtCoefficienteTrasformazione.Visible = true;
                        tdNoCoefficienteTrasformazione.Visible = false;
                        pnlQuotaRetributivaAnnua.Visible = true;
                        pnlQuotaPensioneRetributivaAnnuaB94.Visible = true;
                        pnlQuotaPensioneRetributivaAnnuaB95.Visible = true;
                        pnlQuotaPensioneRetributivaAnnuaB97.Visible = true;
                        pnlQuotaPensioneRetributivaAnnuaCessazione.Visible = true;
                        pnlQuotaPensioneRetributivaAnnuaB98.Visible = true;
                    }

                    if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.IsDomandaINPDAP) &&
                        (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione)
                        || (CodeUtility.IsRicostituzioneVariazioneDatiContitolari(datiPensione) && !this.domanda.IsDomandaINPDAP)))
                    {
                        txtPensioneAnnuaLorda.Enabled = false;
                        txtAnniServUtiliDirittoAA.Enabled = false;
                        txtAnniServUtiliDirittoMM.Enabled = false;
                        txtAnniServUtiliDirittoGG.Enabled = false;
                        txtCoefficienteTrasformazione.Enabled = false;
                        //Quota A
                        txtServizioUtileAAQtaA.Enabled = false;
                        txtServizioUtileMMQtaA.Enabled = false;
                        txtServizioUtileGGQtaA.Enabled = false;
                        txtRetribuzioneQtaA.Enabled = false;
                        txtQuotaRetributivaAnnua.Enabled = false;
                        txtImpIndenIntegrSpecQtaA.Enabled = false;
                        if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.IsDomandaINPDAP)
                            txtRetribuzioneSenzaBenefici336.Enabled = false;
                        //Quota B1
                        txtServizioUtileAAQtaB1.Enabled = false;
                        txtServizioUtileMMQtaB1.Enabled = false;
                        txtServizioUtileGGQtaB1.Enabled = false;
                        txtRMSQtaB1.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB94.Enabled = false;
                        //Quota B2
                        txtServizioUtileAAQtaB2.Enabled = false;
                        txtServizioUtileMMQtaB2.Enabled = false;
                        txtServizioUtileGGQtaB2.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB95.Enabled = false;
                        //Quota B3
                        txtServizioUtileAAQtaB3.Enabled = false;
                        txtServizioUtileMMQtaB3.Enabled = false;
                        txtServizioUtileGGQtaB3.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB97.Enabled = false;
                        //Quota B4 - Cessazione
                        txtServizioUtileCessazioneAA.Enabled = false;
                        txtServizioUtileCessazioneMM.Enabled = false;
                        txtServizioUtileCessazioneGG.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaCessazione.Enabled = false;
                        //Quota B5
                        txtServizioUtileAAQtaB5.Enabled = false;
                        txtServizioUtileMMQtaB5.Enabled = false;
                        txtServizioUtileGGQtaB5.Enabled = false;
                        txtQuotaPensioneRetributivaAnnuaB98.Enabled = false;
                        //Quota C - DL335
                        txtImportoContributivoTotaleFS_PT.Enabled = false;
                        txtSettimaneFS_PT.Enabled = false;
                        txtMontanteFS_PT.Enabled = false;
                        txtImportoQuotaCFS_PT.Enabled = false;
                        //Quota D - DL214
                        txtImportoContribTotaleQuotaDL214.Enabled = false;
                        txtNSettimaneQuotaDL214.Enabled = false;
                        txtMontanteQuotaDL214.Enabled = false;
                        txtQuotaPensioneContributivaAnnuaDL214.Enabled = false;

                        if (this.domanda.IsDomandaINPDAP)
                        {
                            txtDivisore.Enabled = false;
                            ddlCapitolo.Enabled = false;
                        }
                    }
                }
                pnlDatiCalcoloContributiviLegge214_VL_FS_PT.Visible = areaDatiFondo.IsContribL214Visible.HasValue ? areaDatiFondo.IsContribL214Visible.Value : false;
            }


            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                 (Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione)
                 || CodeUtility.IsRicostituzioneVariazioneDatiContitolari(datiPensione)))
                btnEliminaDatiCalcolo.Enabled = false;

         
        }

        protected void nSettimaneOI_TextChanged(object sender, EventArgs e)
        {
            SetSettimaneTotali();
        }

        private void SetSettimaneTotali()
        {
            // ENG - Memo 79
            if (pnlNSettimane_OrganizzazioniInternazionali.Visible)
            {
                int AnniServUtiliDirittoTotAA =
                    (string.IsNullOrEmpty(txtAnniServUtiliDirittoAA.Text) ? 0 : int.Parse(txtAnniServUtiliDirittoAA.Text)) +
                    (string.IsNullOrEmpty(txtAnniServUtiliDirittoOIAA.Text) ? 0 : int.Parse(txtAnniServUtiliDirittoOIAA.Text));

                int AnniServUtiliDirittoTotMM =
                    (string.IsNullOrEmpty(txtAnniServUtiliDirittoMM.Text) ? 0 : int.Parse(txtAnniServUtiliDirittoMM.Text)) +
                    (string.IsNullOrEmpty(txtAnniServUtiliDirittoOIMM.Text) ? 0 : int.Parse(txtAnniServUtiliDirittoOIMM.Text));

                int AnniServUtiliDirittoTotGG =
                    (string.IsNullOrEmpty(txtAnniServUtiliDirittoGG.Text) ? 0 : int.Parse(txtAnniServUtiliDirittoGG.Text)) +
                    (string.IsNullOrEmpty(txtAnniServUtiliDirittoOIGG.Text) ? 0 : int.Parse(txtAnniServUtiliDirittoOIGG.Text));

                // Normalizzazione giorni: ogni 30 giorni aggiungi 1 mese
                if (AnniServUtiliDirittoTotGG > 29)
                {
                    AnniServUtiliDirittoTotMM += AnniServUtiliDirittoTotGG / 30;
                    AnniServUtiliDirittoTotGG = AnniServUtiliDirittoTotGG % 30;
                }

                // Normalizzazione mesi: ogni 12 mesi aggiungi 1 anno
                if (AnniServUtiliDirittoTotMM > 11)
                {
                    AnniServUtiliDirittoTotAA += AnniServUtiliDirittoTotMM / 12;
                    AnniServUtiliDirittoTotMM = AnniServUtiliDirittoTotMM % 12;
                }

                txtAnniServUtiliDirittoTotAA.Text = AnniServUtiliDirittoTotAA.ToString();
                txtAnniServUtiliDirittoTotMM.Text = AnniServUtiliDirittoTotMM.ToString();
                txtAnniServUtiliDirittoTotGG.Text = AnniServUtiliDirittoTotGG.ToString();
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        #endregion private methods

        enum EnumViewState
        {
            IdRecordFondo,
            ContribDL214,
            PensioneAnnuaLorda,
            PensioneAnnuaLorda214
        }
    }
}