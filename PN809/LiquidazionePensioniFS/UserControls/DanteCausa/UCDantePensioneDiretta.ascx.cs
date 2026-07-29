using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa
{
    public partial class UCDantePensioneDiretta : CustomBaseUserControl, IDanteCausa, ITitolarePensione
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

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void RenderControlsDiv()
        {
            if (ViewState["TipoAppartenenzaDomanda"] == null)
                ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;
            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
            {
                this.divAgo.Visible = false;
                this.divAgoCI.Visible = true;
                this.pnlNaturaPensione.Visible = true;
            }
            else
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
            {
                this.divAgo.Visible = true;
                this.divAgoCI.Visible = true;
            }
            else
            {
                this.divAgo.Visible = false;
                this.divAgoCI.Visible = false;
                this.pnlEliminazione.Visible = true;
            }
        }

        private void RenderControlsDdl(IDanteCausa danteCausa)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDanteCausa.DatiMaggiorazione780[] lDatiMaggiorazione780 = danteCausa.areaDanteCausa.ElencoMaggiorazione780;
            try
            {
                LoadDdlCodeEliminazione(danteCausa);
                //LoadDdlCodeEliminazione(areaDecodifica);
                LoadDdlCodeTipoPensione(areaDecodifica);
                LoadDdlCodeMaggiorazione781(lDatiMaggiorazione780);
                LoadDdlCategoriaPensione();
                if (ViewState["TipoAppartenenzaDomanda"] == null)
                    ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;

                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI ||
                    ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                    LoadDdlCodeNaturaCI(danteCausa, areaDecodifica);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteAnagrafica, Errore nel metodo RenderControls: " + ex);
            }
        }

        public void RenderRicBanc()
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = CodeUtility.GetDatiPensioneFromSession();

            if (Utility.IsDomandaBancRicAnte1991(this.domanda.Categoria, this.TitolarePensione.Pensione, this.areaDanteCausa))
            {
                pnlPensioneDiretta.Enabled = false;
            }

        }
        //private void LoadDdlCodeEliminazione(CodeUtility areaDecodifica)
        //{
        //    this.ddlCodiceEliminazione.Items.Clear();
        //    AreaDecodifica.DatiCodiceEliminazione[] listCodiceEliminazione = areaDecodifica.GetValuesDecodifica().ElencoCodiceEliminazione;
        //    foreach (AreaDecodifica.DatiCodiceEliminazione codiceEliminazione in listCodiceEliminazione)
        //    {
        //        this.ddlCodiceEliminazione.Items.Add(new ListItem(codiceEliminazione.Descrizione, codiceEliminazione.Id));
        //        this.ddlCodiceEliminazione.Items[this.ddlCodiceEliminazione.Items.Count - 1].Attributes.Add("title", codiceEliminazione.Descrizione);
        //    }
        //}

        private void LoadDdlCodeEliminazione(IDanteCausa danteCausa)
        {
            //this.ddlCodiceEliminazione.Items.Clear();
            //AreaDecodifica.DatiCodiceEliminazione[] listCodiceEliminazione = areaDecodifica.GetValuesDecodifica().ElencoCodiceEliminazione;
            //foreach (AreaDecodifica.DatiCodiceEliminazione codiceEliminazione in listCodiceEliminazione)
            //{
            //    this.ddlCodiceEliminazione.Items.Add(new ListItem(codiceEliminazione.Descrizione, codiceEliminazione.Id));
            //    this.ddlCodiceEliminazione.Items[this.ddlCodiceEliminazione.Items.Count - 1].Attributes.Add("title", codiceEliminazione.Descrizione);
            //}

            if (danteCausa != null && danteCausa.areaDanteCausa != null)
            {
                if (danteCausa.areaDanteCausa.ElencoCodiceEliminazione != null && danteCausa.areaDanteCausa.ElencoCodiceEliminazione.Count() > 0)
                {
                    if (ddlCodiceEliminazione.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlCodiceEliminazione, string.Empty, string.Empty, string.Empty);
                        foreach (CodiceEliminazione codeEliminazione in danteCausa.areaDanteCausa.ElencoCodiceEliminazione)
                            CodeUtility.SetValueDdl(ddlCodiceEliminazione, codeEliminazione.Descrizione, codeEliminazione.Descrizione, codeEliminazione.Id);
                    }
                }
            }
        }

        //Attività del 23/07/2020:
        //richiesto cambio puntamento foreign key da DecodificaTipoPensione a DecodificaTipoCalcolo.
        //La ddl non è visibile quindi non sono state apportate altre modifiche.
        private void LoadDdlCodeTipoPensione(CodeUtility areaDecodifica)
        {
            this.ddlTipoPensione.Items.Clear();
            //Rivista Gestione del ddlTipoPensione - 21/09/2020
            List<AreaDecodifica.DatiTipoCalcolo> listaTipoCalcolo = areaDecodifica.GetValuesDecodifica().ElencoTipoCalcolo.ToList();
            CodeUtility.SetValueDdl(this.ddlTipoPensione, string.Empty, string.Empty, string.Empty);
            foreach (AreaDecodifica.DatiTipoCalcolo tipoCalcolo in listaTipoCalcolo)
            {
                switch (this.domanda.TipoAppartenenza)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                        if (!this.domanda.IsDomandaENPALS)
                        {
                            if (tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Inps")
                                CodeUtility.SetValueDdl(ddlTipoPensione, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);
                        }
                        else
                        {
                            if (tipoCalcolo.Tipologia == "AGO" && tipoCalcolo.Tipo.Trim() == "Enpals")
                                CodeUtility.SetValueDdl(ddlTipoPensione, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);
                        }
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS:
                        if (tipoCalcolo.Tipologia == "FS")
                            CodeUtility.SetValueDdl(ddlTipoPensione, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                        if (tipoCalcolo.Tipologia == "CI")
                            CodeUtility.SetValueDdl(ddlTipoPensione, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);
                        break;
                }
                //Rivista Gestione del ddlTipoPensione - 21/09/2020
            }
        }

        private void LoadDdlCodeMaggiorazione781(AreaDanteCausa.DatiMaggiorazione780[] lDatiMaggiorazione781)
        {
            this.ddlCodiceMaggiora.Items.Clear();
            this.ddlCodiceMaggiora.Items.Add(new ListItem(string.Empty, string.Empty));
            if (lDatiMaggiorazione781 != null)
            {
                foreach (AreaDanteCausa.DatiMaggiorazione780 codeDatiMaggiorazione781 in lDatiMaggiorazione781)
                {
                    this.ddlCodiceMaggiora.Items.Add(new ListItem(codeDatiMaggiorazione781.Descrizione, codeDatiMaggiorazione781.Id));
                    this.ddlCodiceMaggiora.Items[this.ddlCodiceMaggiora.Items.Count - 1].Attributes.Add("title", codeDatiMaggiorazione781.Descrizione);
                }
            }
        }

        private void LoadDdlCodeNaturaCI(IDanteCausa danteCausa, CodeUtility areaDecodifica)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            this.ddlCodNatura1.Items.Clear();
            this.ddlCodNatura2.Items.Clear();
            this.ddlCodNatura3.Items.Clear();
            //List<CodiciNatura> listCodiciNatura = danteCausa.areaDanteCausa.ElencoCodiciNatura.ToList();
            List<CodiciNatura> listCodiciNatura = danteCausa.areaDanteCausa.ElencoCodiciNatura.ToList();
            //Eng - per SOS, SRS, SOARTS, SOCOMS e CodiceProdotto 0021 secondo e terzo byte del codice natura aggiunto blank
            //ENG - aggiunto blank anche per le TRF/RIC SOS, SRS, SOARTS, SOCOMS
            if (danteCausa.domanda != null && danteCausa.domanda.Categoria != null)
            {
                if (((danteCausa.domanda.Categoria.Trim() == "SOS" || danteCausa.domanda.Categoria.Trim() == "SRS" || danteCausa.domanda.Categoria.Trim() == "SOARTS" || danteCausa.domanda.Categoria.Trim() == "SOCOMS") && danteCausa.domanda.CodProdotto == "0021") ||
                    ((danteCausa.domanda.Categoria.Trim() == "SOS" || danteCausa.domanda.Categoria.Trim() == "SRS" || danteCausa.domanda.Categoria.Trim() == "SOARTS" || danteCausa.domanda.Categoria.Trim() == "SOCOMS") && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)))
                {
                    CodeUtility.SetValueDdl(ddlCodNatura2, string.Empty, string.Empty, " ");
                    CodeUtility.SetValueDdl(ddlCodNatura3, string.Empty, string.Empty, " ");
                }
            }

            foreach (CodiciNatura codiceNatura in listCodiciNatura)
            {
                switch (codiceNatura.Posizione)
                {
                    case 1:
                        this.ddlCodNatura1.Items.Add(new ListItem(codiceNatura.TraduzioneSuGP.Value.ToString(), codiceNatura.TraduzioneSuGP.Value.ToString()));
                        this.ddlCodNatura1.Items[this.ddlCodNatura1.Items.Count - 1].Attributes.Add("title", codiceNatura.Descrizione);
                        break;
                    case 2:
                        this.ddlCodNatura2.Items.Add(new ListItem(codiceNatura.TraduzioneSuGP.Value.ToString(), codiceNatura.TraduzioneSuGP.Value.ToString()));
                        this.ddlCodNatura2.Items[this.ddlCodNatura2.Items.Count - 1].Attributes.Add("title", codiceNatura.Descrizione);
                        break;
                    case 3:
                        this.ddlCodNatura3.Items.Add(new ListItem(codiceNatura.TraduzioneSuGP.Value.ToString(), codiceNatura.TraduzioneSuGP.Value.ToString()));
                        this.ddlCodNatura3.Items[this.ddlCodNatura3.Items.Count - 1].Attributes.Add("title", codiceNatura.Descrizione);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadDdlCategoriaPensione()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            AreaDecodifica.DatiCategoriaPensione[] listaCategoriePensioni = valoriDecodificati.ElencoCategoriePensione;
            GestioneCategoriePensioni(listaCategoriePensioni);
        }

        private void GestioneCategoriePensioni(AreaDecodifica.DatiCategoriaPensione[] lista)
        {
            PresenterDanteCausa presenterDC = new PresenterDanteCausa();
            ddlCategoriaPensione.Items.Add(new ListItem(string.Empty, string.Empty));
            foreach (KeyValuePair<string, string> app in presenterDC.GestioneCategoriePensioni(lista))
            {
                ListItem li = new ListItem();
                li.Attributes.Add("title", app.Key);
                li.Text = app.Key;
                li.Value = app.Value;
                ddlCategoriaPensione.Items.Add(li);
            }
        }

        internal void ValorizzaControlliPensioneDiretta(IDanteCausa danteCausa)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.areaDanteCausa == null)
                this.areaDanteCausa = danteCausa.areaDanteCausa;

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            this.TitolarePensione.Pensione = datiPensione;

            if (ViewState["TipoAppartenenzaDomanda"] == null)
                ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;

            string controlloDinamico = string.Empty;
            string controlloDinamicoAbilitazioneSpacchettate024 = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamico);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                controlloDinamicoAbilitazioneSpacchettate024 = controlloDinamico;

            RenderControlsDiv();
            RenderControlsDdl(danteCausa);
            RenderRicBanc();

            if (danteCausa.areaDanteCausa.DatiPensioneDiretta != null && !String.IsNullOrEmpty(danteCausa.areaDanteCausa.DatiPensioneDiretta.SiglaCategoria))
                if (this.ddlCategoriaPensione.Items.FindByText(danteCausa.areaDanteCausa.DatiPensioneDiretta.SiglaCategoria.Trim()) != null)
                    this.ddlCategoriaPensione.Items.FindByText(danteCausa.areaDanteCausa.DatiPensioneDiretta.SiglaCategoria.Trim()).Selected = true;

            if (danteCausa.areaDanteCausa.DatiPensioneDiretta != null && danteCausa.areaDanteCausa.DatiPensioneDiretta.Certificato != null)
                this.txtCertificato.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.Certificato).PadLeft(8, '0');

            DateTime dataAssunzioneCarico = danteCausa.areaDanteCausa.DataAssunzioneCarico ?? DateTime.MaxValue;

            if (danteCausa.areaDanteCausa.DatiPensioneDiretta != null && !String.IsNullOrEmpty(danteCausa.areaDanteCausa.DatiPensioneDiretta.Sede))
            {
                if (Utility.IsDomandaDAIAnte2003(dataAssunzioneCarico, domanda.Categoria))
                {
                    this.txtSede.Text = danteCausa.areaDanteCausa.DatiPensioneDiretta.Sede;
                }
                else
                {
                    this.txtSede.Text = CodeUtility.GetSede(danteCausa.areaDanteCausa.DatiPensioneDiretta.Sede.PadLeft(4, '0'));
                }

            }


            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
            {
                this.ddlCodiceEliminazione.Items.FindByValue(danteCausa.areaDanteCausa.ElencoCodiceEliminazione.First(x => x.TraduzioneSuGP == '1').Id).Selected = true;
                this.ddlCodiceEliminazione.Enabled = false;

                if (CodeUtility.IsRicostituzione(datiPensione) && this.domanda.Categoria.StartsWith("S") && !this.domanda.IsDomandaINPDAP &&
                    !(domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    txtDecorrenzaEliminazioneCont.Enabled = false;
                    txtDecorrenzaEliminazione.Enabled = false;
                }
            }

            if (this.domanda.IsDomandaINPDAP ||
                (!string.IsNullOrEmpty(controlloDinamicoAbilitazioneSpacchettate024) && controlloDinamicoAbilitazioneSpacchettate024.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione)))
            {
                txtDecorrenzaEliminazione.Enabled = false;
                txtDecorrenzaEliminazioneCont.Enabled = true;
                RequiredFieldValidator1.Enabled = false;
            }

            //ENG - Reversibilita 024        
            if (Utility.IsDomandaReversibilita(datiPensione) && (domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                txtDecorrenzaEliminazione.Enabled = false;

            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa.areaDanteCausa, this.domanda.Categoria) &&
                this.domanda != null && this.domanda.Categoria != null && this.domanda.Categoria.Trim() == "SDZ")
            {
                txtDecorrenzaEliminazione.Enabled = false;
            }
            if (danteCausa.areaDanteCausa.DatiPensioneDiretta != null && danteCausa.areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.HasValue)
                this.txtDecorrenza.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.DatiPensioneDiretta.DecorrenzaPensione.Value);

            if (danteCausa.areaDanteCausa.DatiPensioneDiretta != null && danteCausa.areaDanteCausa.DatiPensioneDiretta.DecorrenzaEliminazione.HasValue)
                this.txtDecorrenzaEliminazione.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.DatiPensioneDiretta.DecorrenzaEliminazione.Value);
            else if (danteCausa.areaDanteCausa.AnagraficaDC != null && danteCausa.areaDanteCausa.AnagraficaDC.DataMorte.HasValue && ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                this.txtDecorrenzaEliminazione.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.AnagraficaDC.DataMorte.Value.AddMonths(1));

            if (danteCausa.areaDanteCausa.DatiPensioneDiretta != null && danteCausa.areaDanteCausa.DatiPensioneDiretta.DecorrenzaEliminazioneContabile.HasValue)
                this.txtDecorrenzaEliminazioneCont.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.DatiPensioneDiretta.DecorrenzaEliminazioneContabile.Value);
            else if (Utility.IsDomandaReversibilita(datiPensione) && (domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                && ddlCodiceEliminazione.SelectedValue == danteCausa.areaDanteCausa.ElencoCodiceEliminazione.First(x => x.TraduzioneSuGP == '1').Id) //ENG - Reversibilita 024
                this.txtDecorrenzaEliminazioneCont.Text = txtDecorrenzaEliminazione.Text;
            else if (this.domanda.IsDomandaINPDAP)
                this.txtDecorrenzaEliminazioneCont.Text = string.Empty;

            if (ViewState["TipoAppartenenzaDomanda"] == null)
                ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;
            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI ||
                ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
            {
                if (danteCausa.areaDanteCausa.DatiPensioneDiretta.Maggiorazione781Contributi != null)
                    this.ddlCodiceMaggiora.Items.FindByValue(Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.Maggiorazione781Contributi)).Selected = true;
            }

            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
            {
                if (!String.IsNullOrEmpty(danteCausa.areaDanteCausa.DatiPensioneDiretta.NaturaPensione))
                {
                    this.ddlCodNatura1.Items.FindByValue(danteCausa.areaDanteCausa.DatiPensioneDiretta.NaturaPensione.Substring(0, 1)).Selected = true;
                    this.ddlCodNatura2.Items.FindByValue(danteCausa.areaDanteCausa.DatiPensioneDiretta.NaturaPensione.Substring(1, 1)).Selected = true;
                    this.ddlCodNatura3.Items.FindByValue(danteCausa.areaDanteCausa.DatiPensioneDiretta.NaturaPensione.Substring(2, 1)).Selected = true;
                }
            }

            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
            {
                if (danteCausa.areaDanteCausa.DatiPensioneDiretta.CodiceBeneficiLegge != null)
                    this.txtCodiceBeneficiLegge.Text = Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.CodiceBeneficiLegge);
                //Rivista Gestione del ddlTipoPensione - 21/09/2020
                if (danteCausa.areaDanteCausa.DatiPensioneDiretta.CodiceTipoPensione != null && this.ddlTipoPensione.Items.FindByValue(Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.CodiceTipoPensione)) != null)
                    this.ddlTipoPensione.Items.FindByValue(Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.CodiceTipoPensione)).Selected = true;
                //Rivista Gestione del ddlTipoPensione - 21/09/2020
                this.txtImportopensione84.Text = (danteCausa.areaDanteCausa.DatiPensioneDiretta.ImportoPensione311284 == null) ? String.Empty : Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.ImportoPensione311284);
                this.txtImportopensione85.Text = (danteCausa.areaDanteCausa.DatiPensioneDiretta.ImportoPensione1185 == null) ? String.Empty : Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.ImportoPensione1185);
                this.txtImportopensione90.Text = (danteCausa.areaDanteCausa.DatiPensioneDiretta.ImportoPensione1190 == null) ? String.Empty : Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.ImportoPensione1190);
                this.txtNumeroContributiDiretta.Text = (danteCausa.areaDanteCausa.DatiPensioneDiretta.NContributiDiretta == null) ? String.Empty : Convert.ToString(danteCausa.areaDanteCausa.DatiPensioneDiretta.NContributiDiretta);
                pnlTipoPensione.Visible = false;
            }

            //ENG - Ric Superstiti 024: in presenza del bypass NESSUN_DANTE_CAUSA allora il tab anagrafica obbligatorio e il tab diretta opzionale
            if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
            {
                if (danteCausa.areaDanteCausa.IsPresenteBypassNessunDanteCausa && Utility.IsRicostituzione(datiPensione) && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                    && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {

                    ddlCategoriaPensione.Enabled = true;
                    txtSede.Enabled = true;
                    txtCertificato.Enabled = true;
                    txtDecorrenza.Enabled = true;
                    ddlCodiceEliminazione.Enabled = true;

                }
            }

            ViewState["AreaDC"] = danteCausa.areaDanteCausa;
        }

        internal DatiPensioneDiretta GetValoriPensioneDiretta()
        {
            AreaDanteCausa areaDC = (AreaDanteCausa)ViewState["AreaDC"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];


            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS && areaDC != null && areaDC.IsPresenteBypassNessunDanteCausa && Utility.IsRicostituzione(datiPensione) && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                   && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                if (areaDC.DatiPensioneDiretta == null)
                    areaDC.DatiPensioneDiretta = new DatiPensioneDiretta();
            }


            if (areaDC != null && areaDC.DatiPensioneDiretta != null)
            {
                if (!String.IsNullOrEmpty(this.ddlCategoriaPensione.SelectedItem.Text))
                    //areaDC.DatiPensioneDiretta.SiglaCategoria = this.lblCategoria.Text;
                    areaDC.DatiPensioneDiretta.SiglaCategoria = this.ddlCategoriaPensione.SelectedItem.Text;
                if (!String.IsNullOrEmpty(this.txtCertificato.Text))
                    areaDC.DatiPensioneDiretta.Certificato = Convert.ToInt32(this.txtCertificato.Text);
                if (!String.IsNullOrEmpty(this.txtSede.Text))
                    areaDC.DatiPensioneDiretta.Sede = Convert.ToString(CodeUtility.ControlSede(this.txtSede.Text));
                if (!String.IsNullOrEmpty(this.ddlCodiceEliminazione.SelectedValue))
                    areaDC.DatiPensioneDiretta.CodiceEliminazione = Convert.ToByte(this.ddlCodiceEliminazione.SelectedValue);
                if (!String.IsNullOrEmpty(this.txtDecorrenzaEliminazione.Text))
                {
                    try
                    {
                        areaDC.DatiPensioneDiretta.DecorrenzaEliminazione = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDecorrenzaEliminazione.Text)));
                    }
                    catch (Exception)
                    {
                        areaDC.DatiPensioneDiretta.DecorrenzaEliminazione = null;
                    }
                }
                if (!String.IsNullOrEmpty(this.txtDecorrenzaEliminazioneCont.Text))
                {
                    try
                    {
                        areaDC.DatiPensioneDiretta.DecorrenzaEliminazioneContabile = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDecorrenzaEliminazioneCont.Text)));
                    }
                    catch (Exception)
                    {
                        areaDC.DatiPensioneDiretta.DecorrenzaEliminazioneContabile = null;
                    }
                }

                if (ViewState["TipoAppartenenzaDomanda"] == null)
                    ViewState["TipoAppartenenzaDomanda"] = ((AreaTitolare.DatiPensione)Session["DatiPensione"]).TipoAppartenenzaDomanda;
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI ||
                    ((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                {
                    if (!String.IsNullOrEmpty(this.ddlCodiceMaggiora.SelectedValue))
                        areaDC.DatiPensioneDiretta.Maggiorazione781Contributi = Convert.ToByte(this.ddlCodiceMaggiora.SelectedValue);
                }

                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.CI)
                    areaDC.DatiPensioneDiretta.NaturaPensione = String.Concat(new string[] { this.ddlCodNatura1.SelectedValue, this.ddlCodNatura2.SelectedValue, this.ddlCodNatura3.SelectedValue });

                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.AGO)
                {
                    if (!String.IsNullOrEmpty(this.txtCodiceBeneficiLegge.Text))
                        areaDC.DatiPensioneDiretta.CodiceBeneficiLegge = Convert.ToByte(this.txtCodiceBeneficiLegge.Text);
                    if (!String.IsNullOrEmpty(this.ddlTipoPensione.SelectedValue) && !String.IsNullOrEmpty(this.ddlTipoPensione.SelectedValue.Trim()))
                        areaDC.DatiPensioneDiretta.CodiceTipoPensione = Convert.ToByte(this.ddlTipoPensione.SelectedValue);
                    else
                        areaDC.DatiPensioneDiretta.CodiceTipoPensione = null;
                    if (!String.IsNullOrEmpty(this.txtImportopensione84.Text))
                        areaDC.DatiPensioneDiretta.ImportoPensione311284 = Convert.ToDecimal(this.txtImportopensione84.Text);
                    if (!String.IsNullOrEmpty(this.txtImportopensione85.Text))
                        areaDC.DatiPensioneDiretta.ImportoPensione1185 = Convert.ToDecimal(this.txtImportopensione85.Text);
                    if (!String.IsNullOrEmpty(this.txtImportopensione90.Text))
                        areaDC.DatiPensioneDiretta.ImportoPensione1190 = Convert.ToDecimal(this.txtImportopensione90.Text);
                    if (!String.IsNullOrEmpty(this.txtNumeroContributiDiretta.Text))
                        areaDC.DatiPensioneDiretta.NContributiDiretta = Convert.ToInt32(this.txtNumeroContributiDiretta.Text);
                }

                //ENG - Per queste tipologie di domande non deve essere presente nessun vincolo al salvataggio
                if (((AreaTitolare.DatiPensione.TipoAppDomanda)ViewState["TipoAppartenenzaDomanda"]) == AreaTitolare.DatiPensione.TipoAppDomanda.FS && areaDC != null && areaDC.IsPresenteBypassNessunDanteCausa && Utility.IsRicostituzione(datiPensione) && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)
                    && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    if (!String.IsNullOrEmpty(txtDecorrenza.Text))
                    {
                        try
                        {
                            areaDC.DatiPensioneDiretta.DecorrenzaPensione = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDecorrenza.Text)));
                        }
                        catch (Exception)
                        {
                            areaDC.DatiPensioneDiretta.DecorrenzaPensione = null;
                        }
                    }
                    else
                        areaDC.DatiPensioneDiretta.DecorrenzaPensione = null;
                }

            }
            else
                areaDC = new AreaDanteCausa();

            return areaDC.DatiPensioneDiretta;
        }

        protected void btnSalvaPensioneDiretta_Click(object sender, EventArgs e)
        {
            areaDanteCausa = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaDanteCausa();
            areaDanteCausa.DatiPensioneDiretta = new DatiPensioneDiretta();
            areaDanteCausa.DatiPensioneDiretta = GetValoriPensioneDiretta();

            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.SalvaDatiPensioneDiretta(this);
            RaiseShowAvviso(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;

    }
}
