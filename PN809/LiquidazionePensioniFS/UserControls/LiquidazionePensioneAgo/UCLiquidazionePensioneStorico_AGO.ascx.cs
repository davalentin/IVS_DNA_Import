using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCLiquidazionePensioneStorico_AGO : CustomBaseUserControl, ILiquidazionePensioneAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        internal void ValorizzaEtichette(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RenderPanels();

            RenderControls(datiPensione, liquidazioneAgo.areaLiquidazionePensioneAgo);
            LoadDdl(liquidazioneAgo, datiPensione);

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico != null)
            {
                #region Dati Generici
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.DecorrenzaOriginaria.HasValue)
                {
                    string inputDecorrenza = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.DecorrenzaOriginaria.ToString();
                    lblDecorrenzaPensioneDatiGenerici.Text = inputDecorrenza.Substring(3, 7);
                }

                //Pensioni in regime di cumulo
                if (Utility.IsDomandaCumulo(this.domanda.Categoria))
                {
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.Contributivo.HasValue)
                        ddlContributivoCum.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.Contributivo.ToString();
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo.HasValue)
                    {
                        ddlTipoCumulo.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo.Value.ToString();
                        if (!liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.TipoCumulo.Value)
                        {
                            pnlCumuloEsterno.Visible = true;
                            if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CumuloEsterno.HasValue)
                                ddlCumuloEsterno.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiGenerici.CumuloEsterno.Value.ToString();
                        }
                    }
                }
                //pensioni normali
                else
                {
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.TipoCalcolo.HasValue)
                        ddlTipoCalcolo.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.TipoCalcolo.Value.ToString();
                    else
                        //per VESO92 e filtro L92 il calcolo deve essere bloccato a 'Contributivo'
                        if ((Utility.IsDomandaVESO92(this.domanda.Categoria) && liquidazioneAgo.areaLiquidazionePensioneAgo.IsDomandaVESO92WithFiltroL92.GetValueOrDefault())
                        || (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsDomandaVESO92WithGP2BB05(this.domanda.Categoria, this.domanda.GP2BB05)))
                        {
                            if (ddlTipoCalcolo.Items.FindByText("Contributivo") != null)
                                ddlTipoCalcolo.SelectedValue = ddlTipoCalcolo.Items.FindByText("Contributivo").Value;
                        }
                        else
                            ddlTipoCalcolo.SelectedIndex = 0;
                }

                if (!this.domanda.Categoria.StartsWith("V"))
                {
                    pnlScadRevSanitaria.Visible = true;
                    if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ScadenzaRevisioneSanitaria.HasValue)
                        txtScadRevSanitaria.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ScadenzaRevisioneSanitaria.Value);
                }

                if (!string.IsNullOrEmpty(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ModalitaLiquidazione))
                    ddlModalitaLiquidazione.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ModalitaLiquidazione;
                else
                    ddlModalitaLiquidazione.SelectedIndex = 0;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.CodiceMobilita.HasValue)
                    ddlCodMobilita.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.CodiceMobilita.Value.ToString();
                else
                    ddlCodMobilita.SelectedIndex = 0;

                #endregion Dati Generici

                #region Dati Assicurativi

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.InizioAssicurazione.HasValue)
                    txtInizioAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.InizioAssicurazione.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.FineAssicurazione.HasValue)
                    txtFineAssicurazione.Text = string.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.FineAssicurazione.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.AttivitaEconomica.HasValue)
                    txtAttivitaEconomica.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.AttivitaEconomica.Value.ToString().PadLeft(2, '0');

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ProfessioneIndividuale.HasValue)
                    txtProfessioneIndividuale.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.ProfessioneIndividuale.Value.ToString().PadLeft(3, '0');

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.InizioUltimoLavoro.HasValue)
                    txtInizioUltLav.Text = string.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.InizioUltimoLavoro.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.FineUltimoLavoro.HasValue)
                    txtFineUltLav.Text = string.Format("{0:dd/MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.FineUltimoLavoro.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NSettimaneOBG.HasValue)
                    txtNumeroSettimaneOBG.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NSettimaneOBG.Value.ToString();

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NContributiVolontari.HasValue)
                    txtNumContrVolontari.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NContributiVolontari.Value.ToString();

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NContributiVVAnzianita.HasValue)
                    txtNumContrVolontariAnz.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.NContributiVVAnzianita.Value.ToString();

                #endregion Dati Assicurativi

                #region Dati Istruttoria

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.CodiceParticolareSoggettoDerogato.HasValue)
                    ddlSoggettoDerogato.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.CodiceParticolareSoggettoDerogato.Value.ToString();
                else
                    ddlSoggettoDerogato.SelectedIndex = -1;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.Legge44997.HasValue)
                    ddlCodReqRidotti.SelectedValue = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.Legge44997.Value.ToString();
                else
                    ddlCodReqRidotti.SelectedIndex = 0;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.RiduzioneRetributiva)
                    ddlRiduzioneRetributiva.SelectedValue = "SI";
                else
                    ddlRiduzioneRetributiva.SelectedValue = "NO";

                txtRiduzioneRetributiva.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.RiduzioneRetributivaPercentuale.HasValue ?
                    liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.RiduzioneRetributivaPercentuale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                #endregion Dati Istruttoria
            }

            if (!CodeUtility.IsContentVisible(pnlDatiIstruttoria))
                pnlDatiIstruttoria.Visible = false;
        }

        #region private methods
        private void RenderPanels()
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                AreaQuadri areaQuadri = (AreaQuadri)Session["Semaforo"];
                if (areaQuadri.QuadroLiquidazionePensione.TabDatiGenerici != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    pnlDatiGenerici.Visible = true;

                if (areaQuadri.QuadroLiquidazionePensione.TabDatiAssicurativi != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    pnlDatiAssicurativi.Visible = true;

                if (areaQuadri.QuadroLiquidazionePensione.TabIstruttoria != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    pnlDatiIstruttoria.Visible = true;
            }
        }

        private void RenderControls(AreaTitolare.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            ManageModalitaLiquidazione(datiPensione);
            ManageCodiceMobilita(datiPensione, this.domanda.IsDomandaENPALS);
            ManageTipoCalcolo(this.domanda.Categoria, datiPensione);
            ManageRiduzioneRetributiva(datiPensione, areaLiquidazionePensione);
            GestioneEtichetteENPALS();

            Utility.Categoria? categoria = Utility.GetCategoria(this.domanda.Categoria.Trim());

            switch (categoria)
            {
                case Utility.Categoria.VOCRED:
                case Utility.Categoria.CRED27:
                case Utility.Categoria.VOCOOP:
                case Utility.Categoria.VOESO:
                case Utility.Categoria.COOP28:
                case Utility.Categoria.ESOTEL:
                case Utility.Categoria.ESOAMB:
                    pnlInizioFineUltimoLavoro.Visible = true;
                    break;
                case Utility.Categoria.VESO33:
                    pnlInizioFineUltimoLavoro.Visible = true;
                    pnlCodiceRequisitoRidotto.Visible = false;
                    break;
                case Utility.Categoria.VESO92:
                case Utility.Categoria.ESPA:
                    pnlCodiceRequisitoRidotto.Visible = false;
                    break;
            }

            if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                areaLiquidazionePensione.IsSperimentaleDonna.GetValueOrDefault())
                pnlCodiceRequisitoRidotto.Visible = false;

            if (this.domanda.IsDomandaENPALS)
            {
                pnlNSettimane_NContributiVolontariDiritto.Visible = false;
                pnlAttEconomProfInd.Visible = false;
                lblNumContrVolontariAnz.Text = "Numero Contributi Volontari:";
            }
            else if (Utility.IsDomandaDAI(this.domanda.Categoria))
            {
                pnlInizioFineUltimoLavoro.Visible = true;
                lblInizioUltLAv.Text = "Data Inizio Contribuzione Utile alla Misura:";
                lblFineUltLav.Text = "Data Fine &nbsp Contribuzione Utile alla Misura:";
            }
            else if (Utility.IsDomandaCumulo(this.domanda.Categoria))
            {
                pnlTipoCumulo.Visible = true;
            }
        }

        private void ManageRiduzioneRetributiva(AreaTitolare.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensioneAgo)
        {
            bool riduzioneRetrib = false;

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if ((areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.HasValue && areaLiquidazionePensioneAgo.IsRiduzioneRetribVisible.Value) ||
                (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80))
                riduzioneRetrib = true;

            if (riduzioneRetrib)
                pnlRiduzioneRetributiva.Visible = true;
            else
                pnlRiduzioneRetributiva.Visible = false;
        }
        
        private void LoadDdl(ILiquidazionePensioneAgo liquidazioneAgo, AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaModalitaLiquidazione != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaModalitaLiquidazione.Count() > 0)
                {
                    if (ddlModalitaLiquidazione.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlModalitaLiquidazione, string.Empty, string.Empty, string.Empty);
                        foreach (DecModalitaLiquidazione codeModLiquidazione in liquidazioneAgo.areaLiquidazionePensioneAgo.listaModalitaLiquidazione)
                            CodeUtility.SetValueDdl(ddlModalitaLiquidazione, codeModLiquidazione.Descrizione, codeModLiquidazione.TraduzioneGp.ToString(), codeModLiquidazione.ValoreAggPeco);
                    }
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaLegge44997 != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaLegge44997.Count() > 0)
                {
                    ddlCodReqRidotti.Items.Clear();
                    CodeUtility.SetValueDdl(ddlCodReqRidotti, string.Empty, string.Empty, string.Empty);
                    foreach (DecodificaLegge44997 codeLegge44997 in liquidazioneAgo.areaLiquidazionePensioneAgo.listaDecodificaLegge44997)
                        CodeUtility.SetValueDdl(ddlCodReqRidotti, codeLegge44997.Descrizione, codeLegge44997.Id.ToString());
                }

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare != null && liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare.Count() > 0 &&
                    liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico != null &&
                    liquidazioneAgo.areaLiquidazionePensioneAgo.DatiLiquidazionePensioneStorico.CodiceParticolareSoggettoDerogato.HasValue)
                {
                    pnlSoggettoDerogato.Visible = true;
                    ddlSoggettoDerogato.Items.Clear();
                    CodeUtility.SetValueDdl(ddlSoggettoDerogato, string.Empty, string.Empty, string.Empty);
                    foreach (CodiceParticolare codeParticolare in liquidazioneAgo.areaLiquidazionePensioneAgo.listaCodiceParticolare)
                        CodeUtility.SetValueDdl(ddlSoggettoDerogato, (codeParticolare.TraduzioneSuGp.HasValue ? codeParticolare.TraduzioneSuGp.Value.ToString() : string.Empty) +
                            " - " + codeParticolare.Descrizione, codeParticolare.Id.ToString());
                }
            }

            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            Presenter.SvrLiquidazione.AreaDecodifica.DatiTipoCalcolo[] listaTipoCalcolo = datiDecodifica.ElencoTipoCalcolo;// areaDecodifica.GetValuesDecodifica().ElencoTipoCalcolo;

            if (ddlTipoCalcolo.Items.Count == 0)
            {
                CodeUtility.SetValueDdl(ddlTipoCalcolo, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiTipoCalcolo tipoCalcolo in listaTipoCalcolo)
                    if ((tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Inps" && (tipoCalcolo.TraduzioneSuGP == 1 || tipoCalcolo.TraduzioneSuGP == 2 || tipoCalcolo.TraduzioneSuGP == 9)) ||
                        (this.domanda.IsDomandaENPALS && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals" && (tipoCalcolo.TraduzioneSuGP == 9)) ||
                        (this.domanda.IsDomandaENPALS && tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals" && (tipoCalcolo.TraduzioneSuGP == 2)))
                        CodeUtility.SetValueDdl(ddlTipoCalcolo, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);
            }

            AreaDecodifica.DatiCodeMobilita[] listaCodeMobilita = datiDecodifica.ElencoCodeMobilita;

            if (ddlCodMobilita.Items.Count == 0)
            {
                CodeUtility.SetValueDdl(ddlCodMobilita, string.Empty, string.Empty, string.Empty);
                foreach (AreaDecodifica.DatiCodeMobilita codeMobilita in listaCodeMobilita)
                    CodeUtility.SetValueDdl(ddlCodMobilita, codeMobilita.Descrizione, codeMobilita.Descrizione, codeMobilita.Id);
            }
        }

        private void ManageModalitaLiquidazione(AreaTitolare.DatiPensione datiPensione)
        {
            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                pnlModalitaLiquidazione.Visible = true;
        }

        private void ManageCodiceMobilita(AreaTitolare.DatiPensione datiPensione, bool isDomandaENPALS)
        {
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Anzianita_Vecchiaia && !isDomandaENPALS)
                pnlCodiceMobilita.Visible = true;
        }

        private void ManageTipoCalcolo(string siglaCategoria, AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaCumulo(siglaCategoria))
            {
                pnlTipoCalcolo.Visible = false;
                pnlTipoCalcoloCum.Visible = true;
            }
            else if (Utility.IsDomandaVESO92_L92(siglaCategoria, datiPensione.Filtro) || 
                     (Utility.IsDomandaVESO29(siglaCategoria) && !string.IsNullOrEmpty(datiPensione.Filtro) && datiPensione.Filtro.Trim() == "FS") ||
                     Utility.IsDomandaVOCRED_CRED27_DAP(this.domanda.Categoria, datiPensione.Filtro) || Utility.IsDomandaAPESociale(siglaCategoria) || 
                     Utility.IsDomandaESOTEL(siglaCategoria) || Utility.IsDomandaESOAMB(siglaCategoria)
                     || (CodeUtility.IsRicostituzione(datiPensione) && Utility.IsIsoPensioneWithGP2BB05(siglaCategoria, this.domanda.GP2BB05)))
            {
                pnlTipoCalcolo.Visible = false;
            }
        }

        private void GestioneEtichetteENPALS()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.IsDomandaENPALS)
                pnlModalitaLiquidazione.Visible = true;
        }

        #endregion private methods
    }
}
