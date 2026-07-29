using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiAssicurativiVL_FS_PT : CustomBaseUserControl, ITitolarePensione, IRecordFondo, ILiquidazionePensione, IDanteCausa
    {
        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        }

        protected void SalvaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            List<RecordFondo> listaRecordFondo = null;
            areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativi(domanda.Tipofondo, out listaRecordFondo);
            if (!(bool)ViewState["IsDomandaConNuovaGestioneDatiFondoFSPT"])
                areaLiquidazionePensioneFS.ListaRecordFondo = listaRecordFondo.ToArray();


            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiAssicurativiFS(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiAssicurativiFS(this);
            if (!this.HasError)
            {
                ClearForm();
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);
                if (!(bool)ViewState["IsDomandaConNuovaGestioneDatiFondoFSPT"])
                {
                    btnSalvaDatiAssicurativi.Enabled = false;
                    btnEliminaDatiAssicurativi.Enabled = false;
                    RaiseDisabilitaTastoSalva(this, null);
                }
                ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione);
            }
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteDatiAssicurativi(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna, bool isDomandaInabilitaAmianto)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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

            //ViewState["DecorrenzaPensione"] = datiPensione.DecorrenzaOriginaria;
            ManageDecorrenzaForReversibilita(datiPensione, liquidazione.areaLiquidazionePensioneFS.DecorrenzaPensioneDirettaDC);
            ViewState["IsCodiceSpecificoVisible"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoVisible;
            ViewState["ListaAttivitaSvolte"] = liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte.ToList();
            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
                ViewState["IsUsuranti"] = "SI";

            if (liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.Value &&
                this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
                ViewState["IsDomandaTrasformazioneAOI_VL"] = "SI";

            if (IsDomandaSperDonna)
                ViewState["IsDomandaSperDonna"] = "SI";

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue)
                ViewState["IsCodNatura2Enabled"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.HasValue)
                ViewState["IsCodiceNatura2DisabledPerSperDonna"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.Value;

            if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivoConOpzione.GetValueOrDefault() || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((!String.IsNullOrEmpty(controlloDinamicoMemo123_2024) && controlloDinamicoMemo123_2024.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024OpzioneContrib) && controlloDinamicoMemo123_2024OpzioneContrib.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                ViewState["IsPensioneTipoContributivoConOpzione"] = "SI";

            ViewState["IsDomandaConNuovaGestioneDatiFondoFSPT"] = liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();

            RenderControlsCommon();
            RenderControlsFromTipoFondo(liquidazione, datiPensione);

            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            LoadDdl(this.domanda.Tipofondo, liquidazione, datiDecodifica, datiPensione);
            ValorizzaEtichetteDatiAssicurativiCommon(liquidazione, datiPensione);
            ValorizzaCodeRequisiti2SperDonna(IsDomandaSperDonna, liquidazione);

            switch (domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    LoadDdlVL_PT(liquidazione);
                    ValorizzaEtichetteDatiAssicurativiVL(liquidazione, datiPensione);
                    ManageValidatorVL(datiPensione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    LoadDdlFS(liquidazione);
                    ValorizzaEtichetteDatiAssicurativiFS(liquidazione, datiPensione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    LoadDdlVL_PT(liquidazione);
                    LoadDdlPT(liquidazione);
                    ValorizzaEtichetteDatiAssicurativiPT(liquidazione, datiPensione);
                    break;
            }

            if (isDomandaInabilitaAmianto)
            {
                pnlAttivitaEconomica.Visible = true;
                pnlProfessioneIndividuale.Visible = true;
                txtAttivitaEconomica.Text = "01";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "250";
                txtProfessioneIndividuale.Enabled = false;
            }

            GestioneEtichetteRic(datiPensione);

            //ENG - Reversibilita 024
            //ENG - RIC REVERSIBILITA 024
            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, areaDanteCausa, this.domanda.Categoria, liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null ? liquidazione.areaLiquidazionePensioneFS.TipoReversibilita : null, this.domanda.Tipofondo)
                && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                if (Utility.IsDomandaReversibilita(datiPensione))
                {
                    ddlOnereMEF.Enabled = false;
                    ddlDirittoIndennIntegrSpec.Enabled = false;
                    ddlIndennIntegrSpecConglobata.Enabled = false;
                    hdnReversibilità024.Value = "SI";
                }

                //ENG - PL Reversibilita 024
                if (!this.domanda.IsDomandaRiapertura)
                {
                    requiredPrimoVersamento.Enabled = false;
                    RFUltimoVersamento.Enabled = false;
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
                    {
                        ddlAttivitaSvolta_RF.Enabled = false;
                    }
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS)
                    {
                        ddlAttivitaSvoltaFS_RF.Enabled = false;
                        txtAttivitaSvoltaFS_RF.Enabled = false;
                    }
                }
            }

        }

        internal DatiAssicurativi GetDatiAssicurativi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, out List<RecordFondo> listaRecordFondo)
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = null;
            areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiCommon(out listaRecordFondo);
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiVL(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiFS(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiPT(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
            }

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        private List<RecordFondo> GetData()
        {
            List<RecordFondo> elencoRecordFondo = new List<RecordFondo>();
            if ((List<RecordFondo>)ViewState["elencoRecordFondo"] == null)
            {
                elencoRecordFondo = CodeUtility.CreaRecord();
                ViewState["elencoRecordFondo"] = elencoRecordFondo;
            }
            else
            {
                elencoRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                CodeUtility.EliminaRecordVuoti(elencoRecordFondo);
                elencoRecordFondo = CodeUtility.AggiungiRecord(elencoRecordFondo, null, null, null, ' ', new DateTime(), null);
                if (ViewState["IsUsuranti"] != null && elencoRecordFondo != null && elencoRecordFondo.Count > 1)
                    elencoRecordFondo[0]._CodiceNatura3 = 'Z';
                ViewState["elencoRecordFondo"] = (List<RecordFondo>)elencoRecordFondo;
            }
            return elencoRecordFondo;

        }

        #region GridView gvRecordFondo VL

        private List<RecordFondo> BindData()
        {
            List<RecordFondo> elencoRecordFondo = GetData();

            List<extAreaRecordFondo> extListAreaRecordFondo = new List<extAreaRecordFondo>();
            foreach (RecordFondo recordFondo in elencoRecordFondo)
            {
                extAreaRecordFondo myExt = new extAreaRecordFondo(recordFondo);
                extListAreaRecordFondo.Add(myExt);
            }

            gvRecordFondo.DataSource = extListAreaRecordFondo;
            gvRecordFondo.DataKeyNames = new string[] { "strDecorrenzaValidita" };
            gvRecordFondo.DataBind();

            return elencoRecordFondo;
        }

        protected void gvRecordFondo_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRecordFondo.EditIndex = e.NewEditIndex;
                BindData();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowEditing " + ex);
            }
        }

        protected void gvRecordFondo_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                GridViewRow row = gvRecordFondo.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvRecordFondo.PageIndex * 10) + e.RowIndex);
                    if (elencoRecordFondo.Count != i + 1)
                        elencoRecordFondo.RemoveAt(elencoRecordFondo.Count - 1);
                    gvRecordFondo.EditIndex = -1;
                    ViewState["elencoRecordFondo"] = elencoRecordFondo;
                    BindData();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowUpdating " + ex);
            }
        }

        protected void gvRecordFondo_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvRecordFondo.EditIndex = -1;
                //Bind data to the GridView control.
                BindData();
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowCancelingEdit " + ex);
            }
        }

        protected void gvRecordFondo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<RecordFondo> listaRecordFondo = GetData();
            RecordFondo[] elencoRecordFondo = listaRecordFondo.ToArray();

            if (e.CommandName == "Elimina")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (listaRecordFondo.Count == 0)
                {
                    char cSpace = ' ';
                    elencoRecordFondo = CodeUtility.AggiungiRecord(elencoRecordFondo.ToList(), cSpace, cSpace, cSpace, cSpace, new DateTime(), null).ToArray();
                    ViewState["elencoRecordFondo"] = elencoRecordFondo.ToList();
                }
                else
                {
                    if (row.DataItemIndex == 0)
                        modalitaEdit.Value = "false";

                    listaRecordFondo.RemoveAt(row.DataItemIndex);
                    ViewState["elencoRecordFondo"] = listaRecordFondo;
                }
                BindData();
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }

            else if (e.CommandName == "Edit")
            {
                RaiseGetDecorrenzaPensione(this, null);
                btnSalvaDatiAssicurativi.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                DropDownList ddlCodNatura1 = (DropDownList)row.FindControl("ddlCodNatura1");
                char? valueCodNatura1 = ddlCodNatura1.SelectedValue[0];

                DropDownList ddlCodNatura2 = (DropDownList)row.FindControl("ddlCodNatura2");
                char? valueCodNatura2 = ddlCodNatura2.SelectedValue[0];

                DropDownList ddlCodNatura3 = (DropDownList)row.FindControl("ddlCodNatura3");
                char? valueCodNatura3 = ddlCodNatura3.SelectedValue[0];

                DropDownList ddlCodiceNonCalcolo = (DropDownList)row.Cells[2].Controls[1];
                char valueCodiceNonCalcolo = ddlCodiceNonCalcolo.SelectedValue[0];

                string decorrenza = ((TextBox)(row.Cells[3].Controls[1])).Text;
                DateTime dateDecorrenza = !String.IsNullOrEmpty(decorrenza) ? Convert.ToDateTime(decorrenza) : DateTime.MinValue;

                string cessazione = ((TextBox)(row.Cells[4].Controls[1])).Text;
                DateTime? dateCessazione = !String.IsNullOrEmpty(cessazione) ? Convert.ToDateTime(cessazione) : (DateTime?)null;

                RaiseAbilitaTastoSalva(this, null);
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;

                if ((row.DataItemIndex - 1) == (elencoRecordFondo.Length - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    listaRecordFondo = CodeUtility.AggiungiRecord(listaRecordFondo, valueCodNatura1, valueCodNatura2, valueCodNatura3, valueCodiceNonCalcolo, dateDecorrenza, dateCessazione);
                    ViewState["elencoRecordFondo"] = listaRecordFondo;
                }
                else   //modifica elemento
                    elencoRecordFondo = CodeUtility.ModificaRecord(elencoRecordFondo.ToList(), row.DataItemIndex, valueCodNatura1, valueCodNatura2, valueCodNatura3, valueCodiceNonCalcolo, dateDecorrenza, dateCessazione).ToArray();

                gvRecordFondo.EditIndex = -1;
                BindData();
            }

            else if (e.CommandName == "Annulla")
            {
                listaRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                if (listaRecordFondo.Count > 1)
                {
                    gvRecordFondo.EditIndex = -1;
                    btnSalvaDatiAssicurativi.Enabled = true;
                    btnEliminaDatiAssicurativi.Enabled = true;
                    RaiseAbilitaTastoSalva(this, null);
                }
                BindData();
            }
        }

        protected void gvRecordFondo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            try
            {
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        if ((elencoRecordFondo.Count == 1) && (!elencoRecordFondo.First()._CodiceNatura1.HasValue) &&
                            (!elencoRecordFondo.First()._CodiceNatura2.HasValue) && (!elencoRecordFondo.First()._CodiceNatura3.HasValue)
                            )
                        {
                            //unica riga vuota, partenza in modalità edit
                            if (modalitaEdit.Value == "false")
                            {
                                RaiseDisabilitaTastoSalva(this, null);
                                btnSalvaDatiAssicurativi.Enabled = false;
                                btnEliminaDatiAssicurativi.Enabled = false;
                                gvRecordFondo.EditIndex = 0;
                                modalitaEdit.Value = "true";
                                BindData();
                            }
                        }
                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            DropDownList ddlCodNatura1 = (DropDownList)e.Row.FindControl("ddlCodNatura1");
                            DropDownList ddlCodNatura2 = (DropDownList)e.Row.FindControl("ddlCodNatura2");
                            DropDownList ddlCodNatura3 = (DropDownList)e.Row.FindControl("ddlCodNatura3");

                            if (ViewState["CodiciNatura"] != null)
                            {
                                Presenter.SvrLiquidazioneFs.CodiciNatura[] listaCodiceNatura = (Presenter.SvrLiquidazioneFs.CodiciNatura[])ViewState["CodiciNatura"];
                                CodeUtility.SetValueDdl(ddlCodNatura2, string.Empty, string.Empty, " ");
                                CodeUtility.SetValueDdl(ddlCodNatura3, string.Empty, string.Empty, " ");
                                foreach (Presenter.SvrLiquidazioneFs.CodiciNatura codiceNatura in listaCodiceNatura)
                                {
                                    if (codiceNatura.Posizione != null)
                                    {
                                        if (codiceNatura.Posizione == 1)
                                            CodeUtility.SetValueDdl(ddlCodNatura1, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                        else if (codiceNatura.Posizione == 2)
                                            CodeUtility.SetValueDdl(ddlCodNatura2, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                        else
                                            CodeUtility.SetValueDdl(ddlCodNatura3, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    }
                                }
                            }
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCRecordFondo", Page.Theme);
                            CodeUtility.SetCampiGridEdit(e.Row, true, ViewState["DecorrenzaPensione"], this.domanda.Tipofondo);
                            CodeUtility.ManageCampiGridEdit(e.Row, true, datiPensione, this.domanda.Tipofondo);
                            if (ViewState["IsUsuranti"] != null)
                                ManageCodNatura3(ddlCodNatura3);

                            if (ViewState["IsDomandaTrasformazioneAOI_VL"] != null)
                                ManageCodNatura3AOI(ddlCodNatura3);

                            if (ViewState["IsDomandaSperDonna"] != null)
                                ManageSperDonna(ddlCodNatura2);

                            if (ViewState["IsCodNatura2Enabled"] != null)
                                ManageCodNatura2Bonus(ddlCodNatura2);

                            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2, (bool)ViewState["IsCodiceNatura2DisabledPerSperDonna"]);

                            if (CodeUtility.IsTipoContributivoConOpzione(datiPensione, ViewState["IsPensioneTipoContributivoConOpzione"] != null ? true : false))
                            {
                                ddlCodNatura2.ClearSelection();
                                if (ddlCodNatura2.Items.FindByValue("J") != null)
                                    ddlCodNatura2.SelectedValue = "J";

                                ddlCodNatura2.Enabled = false;
                            }
                        }
                        else
                        {
                            if (e.Row.DataItemIndex >= 0 && e.Row.DataItemIndex <= elencoRecordFondo.Count - 2)
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], null, Page.Theme, string.Empty);

                        }
                    }
                    else   //record successivi al primo
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            DropDownList ddlCodNatura1 = (DropDownList)e.Row.FindControl("ddlCodNatura1");
                            DropDownList ddlCodNatura2 = (DropDownList)e.Row.FindControl("ddlCodNatura2");
                            DropDownList ddlCodNatura3 = (DropDownList)e.Row.FindControl("ddlCodNatura3");

                            Presenter.SvrLiquidazioneFs.CodiciNatura[] listaCodiceNatura = (Presenter.SvrLiquidazioneFs.CodiciNatura[])ViewState["CodiciNatura"];
                            CodeUtility.SetValueDdl(ddlCodNatura2, string.Empty, string.Empty, " ");
                            CodeUtility.SetValueDdl(ddlCodNatura3, string.Empty, string.Empty, " ");
                            foreach (Presenter.SvrLiquidazioneFs.CodiciNatura codiceNatura in listaCodiceNatura)
                            {
                                if (codiceNatura.Posizione != null)
                                {
                                    if (codiceNatura.Posizione == 1)
                                        CodeUtility.SetValueDdl(ddlCodNatura1, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    else if (codiceNatura.Posizione == 2)
                                        CodeUtility.SetValueDdl(ddlCodNatura2, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    else
                                        CodeUtility.SetValueDdl(ddlCodNatura3, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                }
                            }

                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCRecordFondo", Page.Theme);
                            CodeUtility.SetCampiGridEdit(e.Row, false, ViewState["DecorrenzaPensione"], this.domanda.Tipofondo);
                            CodeUtility.ManageCampiGridEdit(e.Row, false, datiPensione, this.domanda.Tipofondo);
                        }
                        else
                        {
                            if (e.Row.DataItemIndex == elencoRecordFondo.Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                                if (e.Row.Cells[1].FindControl("lblcodiceNatura1") != null)
                                    e.Row.Cells[1].FindControl("lblcodiceNatura1").Visible = false;
                                if (e.Row.Cells[1].FindControl("lblcodiceNatura2") != null)
                                    e.Row.Cells[1].FindControl("lblcodiceNatura2").Visible = false;
                                if (e.Row.Cells[1].FindControl("lblcodiceNatura3") != null)
                                    e.Row.Cells[1].FindControl("lblcodiceNatura3").Visible = false;
                            }
                            else
                            {
                                if (e.Row.DataItemIndex >= 0 && e.Row.DataItemIndex <= elencoRecordFondo.Count - 2)
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDelete");
                            }
                        }
                    }
                }
            }

            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowDataBound " + ex);
            }
        }

        protected void gvRecordFondo_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRecordFondo.PageIndex = e.NewPageIndex;
                BindData();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_onPageIndexChanging" + ex);
            }
        }

        #endregion GridView gvRecordFondo VL

        #region GridView gvRecordFondo FS - PT

        private List<RecordFondo> BindDataFS_PT()
        {
            List<RecordFondo> elencoRecordFondo = GetData();

            List<extAreaRecordFondoFS_PT> extListAreaRecordFondo = new List<extAreaRecordFondoFS_PT>();
            foreach (RecordFondo recordFondo in elencoRecordFondo)
            {
                extAreaRecordFondoFS_PT myExt = new extAreaRecordFondoFS_PT(recordFondo);
                extListAreaRecordFondo.Add(myExt);
            }

            gvRecordFondoFS_PT.DataSource = extListAreaRecordFondo;
            gvRecordFondoFS_PT.DataKeyNames = new string[] { "strDecorrenzaValidita" };
            gvRecordFondoFS_PT.DataBind();

            return elencoRecordFondo;
        }

        protected void gvRecordFondoFS_PT_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRecordFondoFS_PT.EditIndex = e.NewEditIndex;
                BindDataFS_PT();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowEditing " + ex);
            }
        }

        protected void gvRecordFondoFS_PT_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                GridViewRow row = gvRecordFondoFS_PT.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvRecordFondoFS_PT.PageIndex * 10) + e.RowIndex);
                    if (elencoRecordFondo.Count != i + 1)
                        elencoRecordFondo.RemoveAt(elencoRecordFondo.Count - 1);
                    gvRecordFondoFS_PT.EditIndex = -1;
                    ViewState["elencoRecordFondo"] = elencoRecordFondo;
                    BindDataFS_PT();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowUpdating " + ex);
            }
        }

        protected void gvRecordFondoFS_PT_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvRecordFondoFS_PT.EditIndex = -1;
                //Bind data to the GridView control.
                BindData();
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowCancelingEdit " + ex);
            }
        }

        protected void gvRecordFondoFS_PT_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<RecordFondo> listaRecordFondo = GetData();
            RecordFondo[] elencoRecordFondo = listaRecordFondo.ToArray();

            if (e.CommandName == "Elimina")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (listaRecordFondo.Count == 0)
                {
                    char cSpace = ' ';
                    elencoRecordFondo = CodeUtility.AggiungiRecord(elencoRecordFondo.ToList(), cSpace, cSpace, cSpace, cSpace, new DateTime(), null).ToArray();
                    ViewState["elencoRecordFondo"] = elencoRecordFondo.ToList();
                }
                else
                {
                    if (row.DataItemIndex == 0)
                        modalitaEdit.Value = "false";

                    listaRecordFondo.RemoveAt(row.DataItemIndex);
                    ViewState["elencoRecordFondo"] = listaRecordFondo;
                }
                BindDataFS_PT();
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }

            else if (e.CommandName == "Edit")
            {
                RaiseGetDecorrenzaPensione(this, null);
                btnSalvaDatiAssicurativi.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                DropDownList ddlCodNatura1 = (DropDownList)row.FindControl("ddlCodNatura1");
                char? valueCodNatura1 = ddlCodNatura1.SelectedValue[0];

                DropDownList ddlCodNatura2 = (DropDownList)row.FindControl("ddlCodNatura2");
                char? valueCodNatura2 = ddlCodNatura2.SelectedValue[0];

                DropDownList ddlCodNatura3 = (DropDownList)row.FindControl("ddlCodNatura3");
                char? valueCodNatura3 = ddlCodNatura3.SelectedValue[0];

                DropDownList ddlCodiceNonCalcolo = (DropDownList)row.Cells[2].Controls[1];
                char valueCodiceNonCalcolo = ddlCodiceNonCalcolo.SelectedValue[0];

                string decorrenza = ((TextBox)(row.Cells[3].Controls[1])).Text;
                DateTime dateDecorrenza = !String.IsNullOrEmpty(decorrenza) ? Convert.ToDateTime(decorrenza) : DateTime.MinValue;

                string cessazione = ((TextBox)(row.Cells[4].Controls[1])).Text;
                DateTime? dateCessazione = !String.IsNullOrEmpty(cessazione) ? Convert.ToDateTime(cessazione) : (DateTime?)null;

                RaiseAbilitaTastoSalva(this, null);
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;

                if ((row.DataItemIndex - 1) == (elencoRecordFondo.Length - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    listaRecordFondo = CodeUtility.AggiungiRecord(listaRecordFondo, valueCodNatura1, valueCodNatura2, valueCodNatura3, valueCodiceNonCalcolo, dateDecorrenza, dateCessazione);
                    ViewState["elencoRecordFondo"] = listaRecordFondo;
                }
                else   //modifica elemento
                    elencoRecordFondo = CodeUtility.ModificaRecord(elencoRecordFondo.ToList(), row.DataItemIndex, valueCodNatura1, valueCodNatura2, valueCodNatura3, valueCodiceNonCalcolo, dateDecorrenza, dateCessazione).ToArray();

                gvRecordFondoFS_PT.EditIndex = -1;
                BindDataFS_PT();
            }

            else if (e.CommandName == "Annulla")
            {
                listaRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                if (listaRecordFondo.Count > 1)
                {
                    gvRecordFondoFS_PT.EditIndex = -1;
                    btnSalvaDatiAssicurativi.Enabled = true;
                    btnEliminaDatiAssicurativi.Enabled = true;
                    RaiseAbilitaTastoSalva(this, null);
                }
                BindDataFS_PT();
            }
        }

        protected void gvRecordFondoFS_PT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            try
            {
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        if ((elencoRecordFondo.Count == 1) && (!elencoRecordFondo.First()._CodiceNatura1.HasValue) &&
                            (!elencoRecordFondo.First()._CodiceNatura2.HasValue) && (!elencoRecordFondo.First()._CodiceNatura3.HasValue)
                            )
                        {
                            //unica riga vuota, partenza in modalità edit
                            if (modalitaEdit.Value == "false")
                            {
                                RaiseDisabilitaTastoSalva(this, null);
                                btnSalvaDatiAssicurativi.Enabled = false;
                                btnEliminaDatiAssicurativi.Enabled = false;
                                gvRecordFondoFS_PT.EditIndex = 0;
                                modalitaEdit.Value = "true";
                                BindDataFS_PT();
                            }
                        }
                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            DropDownList ddlCodNatura1 = (DropDownList)e.Row.FindControl("ddlCodNatura1");
                            DropDownList ddlCodNatura2 = (DropDownList)e.Row.FindControl("ddlCodNatura2");
                            DropDownList ddlCodNatura3 = (DropDownList)e.Row.FindControl("ddlCodNatura3");

                            if (ViewState["CodiciNatura"] != null)
                            {
                                Presenter.SvrLiquidazioneFs.CodiciNatura[] listaCodiceNatura = (Presenter.SvrLiquidazioneFs.CodiciNatura[])ViewState["CodiciNatura"];
                                CodeUtility.SetValueDdl(ddlCodNatura2, string.Empty, string.Empty, " ");
                                foreach (Presenter.SvrLiquidazioneFs.CodiciNatura codiceNatura in listaCodiceNatura)
                                {
                                    if (codiceNatura.Posizione != null)
                                    {
                                        if (codiceNatura.Posizione == 1)
                                            CodeUtility.SetValueDdl(ddlCodNatura1, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                        else if (codiceNatura.Posizione == 2)
                                            CodeUtility.SetValueDdl(ddlCodNatura2, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                        else
                                            CodeUtility.SetValueDdl(ddlCodNatura3, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    }
                                }
                            }
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCRecordFondo", Page.Theme);
                            CodeUtility.SetCampiGridEdit(e.Row, true, ViewState["DecorrenzaPensione"], this.domanda.Tipofondo);
                            if (ViewState["IsUsuranti"] != null)
                                ManageCodNatura3(ddlCodNatura3);

                            if (ViewState["IsDomandaSperDonna"] != null)
                                ManageSperDonna(ddlCodNatura2);

                            if (ViewState["IsCodNatura2Enabled"] != null)
                                ManageCodNatura2Bonus(ddlCodNatura2);

                            if (CodeUtility.IsTipoContributivoConOpzione(datiPensione, ViewState["IsPensioneTipoContributivoConOpzione"] != null ? true : false))
                            {
                                ddlCodNatura2.ClearSelection();
                                if (ddlCodNatura2.Items.FindByValue("J") != null)
                                    ddlCodNatura2.SelectedValue = "J";

                                ddlCodNatura2.Enabled = false;
                            }
                        }
                        else
                        {
                            if (e.Row.DataItemIndex >= 0 && e.Row.DataItemIndex <= elencoRecordFondo.Count - 2)
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], null, Page.Theme, string.Empty);

                        }
                    }
                    else   //record successivi al primo
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            DropDownList ddlCodNatura1 = (DropDownList)e.Row.FindControl("ddlCodNatura1");
                            DropDownList ddlCodNatura2 = (DropDownList)e.Row.FindControl("ddlCodNatura2");
                            DropDownList ddlCodNatura3 = (DropDownList)e.Row.FindControl("ddlCodNatura3");

                            Presenter.SvrLiquidazioneFs.CodiciNatura[] listaCodiceNatura = (Presenter.SvrLiquidazioneFs.CodiciNatura[])ViewState["CodiciNatura"];
                            CodeUtility.SetValueDdl(ddlCodNatura2, string.Empty, string.Empty, " ");
                            foreach (Presenter.SvrLiquidazioneFs.CodiciNatura codiceNatura in listaCodiceNatura)
                            {
                                if (codiceNatura.Posizione != null)
                                {
                                    if (codiceNatura.Posizione == 1)
                                        CodeUtility.SetValueDdl(ddlCodNatura1, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    else if (codiceNatura.Posizione == 2)
                                        CodeUtility.SetValueDdl(ddlCodNatura2, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    else
                                        CodeUtility.SetValueDdl(ddlCodNatura3, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                }
                            }

                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCRecordFondo", Page.Theme);
                            CodeUtility.SetCampiGridEdit(e.Row, false, ViewState["DecorrenzaPensione"], this.domanda.Tipofondo);
                        }
                        else
                        {
                            if (e.Row.DataItemIndex == elencoRecordFondo.Count - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                                if (e.Row.Cells[1].FindControl("lblcodiceNatura1") != null)
                                    e.Row.Cells[1].FindControl("lblcodiceNatura1").Visible = false;
                                if (e.Row.Cells[1].FindControl("lblcodiceNatura2") != null)
                                    e.Row.Cells[1].FindControl("lblcodiceNatura2").Visible = false;
                                if (e.Row.Cells[1].FindControl("lblcodiceNatura3") != null)
                                    e.Row.Cells[1].FindControl("lblcodiceNatura3").Visible = false;
                            }
                            else
                            {
                                if (e.Row.DataItemIndex >= 0 && e.Row.DataItemIndex <= elencoRecordFondo.Count - 2)
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDelete");
                            }
                        }
                    }
                }
            }

            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_RowDataBound " + ex);
            }
        }

        protected void gvRecordFondoFS_PT_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRecordFondoFS_PT.PageIndex = e.NewPageIndex;
                BindDataFS_PT();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_onPageIndexChanging" + ex);
            }
        }

        #endregion GridView gvRecordFondo FS - PT

        #region Common

        private void ValorizzaEtichetteDatiAssicurativiCommon(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (!liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault())
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
                {
                    List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                    ViewState["elencoRecordFondo"] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);

                    if (this.domanda.Tipofondo.HasValue)
                    {
                        switch (this.domanda.Tipofondo.Value)
                        {
                            case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                                BindData();
                                break;
                            case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                            case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                                BindDataFS_PT();
                                break;
                        }
                    }
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.TipoPensione != null)
            {
                lblTipoPensione.Text = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Key;
                hdnTipoPensione.Value = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString();
            }

            //if (datiPensione.DecorrenzaOriginaria == null)
            //    lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            //else
            //{
            //    String inputDecorrenza = datiPensione.DecorrenzaOriginaria.ToString();
            //    lblDecorrenzaPensioneDatiAssicurativi.Text = inputDecorrenza.Substring(3, 7);
            //}

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione != null)
                txtPrimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione);

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione != null)
                txtUltimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione);

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2 != null)
                txtCodiceRequisiti2.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.ToString();

            if ((!liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoVisible.HasValue || liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoVisible.Value) &&
                liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico != null)
                ddlCodiceSpecifico.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.ToString();

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 != null)
                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                    ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();

            if (datiPensione.FlagUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.HasValue)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
                    GestioneEtichetteIsUnicarpe(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi, datiPensione, this.domanda.Tipofondo);
            }

            if (this.domanda.IsDomandaRiapertura || datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione)
            {
                pnlCapitalizzazione.Enabled = false;
            }
        }

        private DatiAssicurativi GetDatiAssicurativiCommon(out List<RecordFondo> listaRecordFondo)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi();
            listaRecordFondo = null;

            if (!(bool)ViewState["IsDomandaConNuovaGestioneDatiFondoFSPT"])
            {
                listaRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                CodeUtility.EliminaRecordVuoti(listaRecordFondo);
                areaLiquidazionePensioneFS.ListaRecordFondo = listaRecordFondo.ToArray();
            }

            if (!(String.Equals(txtPrimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtPrimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtPrimoVersamento.Text);

            if (!(String.Equals(txtUltimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtUltimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtUltimoVersamento.Text);

            if (!((bool?)ViewState["IsCodiceSpecificoVisible"]).HasValue || ((bool?)ViewState["IsCodiceSpecificoVisible"]).Value)
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico = !String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue) ? Convert.ToByte(ddlCodiceSpecifico.SelectedValue) : (byte?)null;

            if (!(String.IsNullOrEmpty(ddlCodRequisiti1.SelectedValue)))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 = char.Parse(ddlCodRequisiti1.SelectedValue);

            if (!(String.IsNullOrEmpty(txtCodiceRequisiti2.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2 = char.Parse(txtCodiceRequisiti2.Text);

            if (!String.IsNullOrEmpty(ddlAttivitaSvolta.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta.SelectedValue;

            if (!String.IsNullOrEmpty(txtAttivitaEconomica.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica = CodeUtility.StringToNullableInt(txtAttivitaEconomica.Text);

            if (!String.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale = CodeUtility.StringToNullableInt(txtProfessioneIndividuale.Text);

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        #endregion Common

        #region Private Methods

        private void GestioneEtichetteIsUnicarpe(DatiAssicurativi datiAssicurativi, AreaTitolare.DatiPensione datiPensione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            pnlTxtPrimoVersamento.Enabled = true;
            pnlTxtUltimoVersamento.Enabled = true;

            if (datiAssicurativi != null)
            {
                Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
                if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
                {
                    if (datiAssicurativi.InizioAssicurazione.HasValue)
                        pnlTxtPrimoVersamento.Enabled = false;

                    if (datiAssicurativi.FineAssicurazione.HasValue)
                        pnlTxtUltimoVersamento.Enabled = false;

                    if (tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
                    {
                        txtRetrAgoQuotaA.Enabled = false;
                        txtRetrAgoQuotaB.Enabled = false;
                    }
                }
            }
        }

        private void ValorizzaCodeRequisiti2SperDonna(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione)
        {
            if (IsDomandaSperDonna && (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.HasValue))
                txtCodiceRequisiti2.Text = "9";
        }

        private void LoadDdl(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione, AreaDecodifica datiDecodifica, AreaTitolare.DatiPensione datiPensione)
        {
            List<CodiceSpecifico> listaCodiceSpecifico;
            if (Utility.IsRicostituzione(datiPensione.CodeGruppo) && ((liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico == 188 && tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) || (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico == 189 && tipoFondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS)))
            {
                listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().FindAll(cs => cs.TipoSelezionabile.ToString() == "3" && cs.Fondo == tipoFondo.Value.ToString().ToUpperInvariant());
            }
            else
            {
                //Il problema è qui. Dovrei filtrare i codici specifici con TipoSelezionabile 3, invece sto filtrando quelli con 1
                listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().FindAll(delegate (CodiceSpecifico code)
                {
                    Dictionary<string, char?> tipiPensioneSelezionabili = liquidazione.areaLiquidazionePensioneFS.TipoPensione;
                    return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                      code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant());
                });
            }


            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
                CodeUtility.IsRicostituzione(datiPensione) && 
                Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, areaDanteCausa, this.domanda.Categoria, liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null ? liquidazione.areaLiquidazionePensioneFS.TipoReversibilita : null, this.domanda.Tipofondo))
            {
                List<int> listaCodiceSpecificoX = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico
                                                                                            .Where(x => x.TraduzioneGp == 'X')
                                                                                            .Where(x => x.Fondo == this.domanda.Tipofondo.ToString())
                                                                                            .Select(x => Convert.ToInt32(x.Id.Value))
                                                                                            .ToList();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.HasValue)
                {
                    if (!listaCodiceSpecifico.Where(x => x.Id == liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.Value).Any())
                    {
                        listaCodiceSpecifico.Add(liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().FindAll(x => x.Id == liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.Value).FirstOrDefault());
                    }
                    if(listaCodiceSpecifico.Where(x => listaCodiceSpecificoX.Contains(Convert.ToInt32(x.Id))).Any())
                    {
                        liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled = false;
                    }
                }
            }

            List<CodiceArt22> listaCodiceArt22 = liquidazione.areaLiquidazionePensioneFS.ListaCodiceArt22.ToList().FindAll(delegate(CodiceArt22 code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                  code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant());
            });

            ViewState["CodiciNatura"] = liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura;

            ddlCodRequisiti1.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodRequisiti1);
            foreach (CodiceRequisito1 codReq1 in liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito1)
                CodeUtility.SetValueDdl(ddlCodRequisiti1, codReq1.Id + " - " + codReq1.Descrizione, codReq1.Descrizione, codReq1.Id);

            ddlCodiceConvenzione.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceConvenzione);
            foreach (CodiceConvenzioneInternazionale codConvInternazionale in liquidazione.areaLiquidazionePensioneFS.ListaCodiceConvenzioneInternazionale)
                CodeUtility.SetValueDdl(ddlCodiceConvenzione, codConvInternazionale.Descrizione, codConvInternazionale.Descrizione, codConvInternazionale.Id);

            ddlCodiceSpecifico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceSpecifico);
            foreach (CodiceSpecifico codSpec in listaCodiceSpecifico)
                CodeUtility.SetValueDdl(ddlCodiceSpecifico, codSpec.Descrizione, codSpec.Descrizione, codSpec.Id.Value.ToString());

            ddlCodArt22.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodArt22);
            if (listaCodiceArt22 != null)
                foreach (CodiceArt22 codiceArt22 in listaCodiceArt22)
                    CodeUtility.SetValueDdl(ddlCodArt22, codiceArt22.Descrizione, codiceArt22.Descrizione, codiceArt22.Id.ToString());

            ddlCodCapitalizzazione.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodCapitalizzazione);
            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceCapitalizzazione != null)
                foreach (CodiceCapitalizzazione codiceCapitalizzazione in liquidazione.areaLiquidazionePensioneFS.ListaCodiceCapitalizzazione)
                    CodeUtility.SetValueDdl(ddlCodCapitalizzazione, codiceCapitalizzazione.Descrizione, codiceCapitalizzazione.Descrizione, codiceCapitalizzazione.Codice.ToString());

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito1 != null)
                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                    ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            txtCodiceRequisiti2.Text = "0";
        }

        private void RenderControlsCommon()
        {
            pnlCommonHeader.Visible = true;
        }

        private void RenderControlsFromTipoFondo(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    pnlCustomVL.Visible = true;
                    pnlCustomVL_PT.Visible = true;
                    pnlCustomFooterVL.Visible = true;
                    if (!String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Substring(0, 1) == "I")  // pensione Invalidità
                        rowDataInvalidita.Visible = true;
                    if (((bool?)ViewState["IsCodiceSpecificoVisible"]).HasValue && !((bool?)ViewState["IsCodiceSpecificoVisible"]).Value)
                        pnlCodiceSpecifico.Visible = false;
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.Value)
                        ddlCodiceSpecifico.Enabled = false;
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceArt22Enabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceArt22Enabled.Value)
                        ddlCodArt22.Enabled = false;
                    pnlGridViewVL.Visible = true;
                    RFVddlTredicesimaMens.Enabled = false;
                    pnlCapitalizzazione.Visible = liquidazione.areaLiquidazionePensioneFS.IsCapitalizzazioneVisible.GetValueOrDefault();

                    if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsReversibilitaOrRicostituzione.GetValueOrDefault() &&
                        liquidazione.areaLiquidazionePensioneFS.IsDomandaAnteArmonizzazione.GetValueOrDefault())
                        trDirittoQuoteFisse.Visible = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        pnlVVUtiliDiritto.Visible = true;
                        pnlVVUtiliMisura.Visible = true;
                        pnlAttivitaEconomica.Visible = true;
                        pnlProfessioneIndividuale.Visible = true;
                    }
                    pnlCustomFS_PT.Visible = true;
                    pnlCustomFS.Visible = true;
                    pnlDecAnteAgosto95.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
                    tdPag1.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
                    tdPag2.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
                    pnlGridViewFS_PT.Visible = true;
                    pnlIISConglobata.Visible = (this.domanda.IsDomandaRiapertura || datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione) ? true : (Utility.IsDomandaReversibilita(datiPensione) &&
                        liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata.HasValue) ? true : false;
                    RFVddlTredicesimaMens.Enabled = true;
                    pnlRecordFondo.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlDecorrenzaCalcoloNuovaGestioneDatiFondoFSPT.Visible = liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlVecchiaGestioneDatiFondoFSPT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlIntegrazioneMinimo.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.Value)
                        ddlCodiceSpecifico.Enabled = false;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        pnlVVUtiliDiritto.Visible = true;
                        pnlVVUtiliMisura.Visible = true;
                        pnlAttivitaEconomica.Visible = true;
                        pnlProfessioneIndividuale.Visible = true;
                    }
                    pnlCustomPT.Visible = true;
                    pnlCustomFS_PT.Visible = true;
                    pnlCustomVL_PT.Visible = true;
                    pnlDecAnteAgosto95.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
                    tdPag1.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
                    tdPag2.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
                    pnlGridViewFS_PT.Visible = true;
                    RFVddlTredicesimaMens.Enabled = true;
                    pnlRecordFondo.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlDecorrenzaCalcoloNuovaGestioneDatiFondoFSPT.Visible = liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlVecchiaGestioneDatiFondoFSPT.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    pnlIntegrazioneMinimo.Visible = !liquidazione.areaLiquidazionePensioneFS.IsDomandaConNuovaGestioneDatiFondoFSPT.GetValueOrDefault();
                    ddlOnereMEF.Enabled = !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura);
                    if (liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.HasValue && !liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.Value)
                        ddlCodiceSpecifico.Enabled = false;
                    break;
            }
        }

        private void ManageCodNatura3(DropDownList ddlCodNatura3)
        {
            if (!ddlCodNatura3.Items.Contains(new ListItem("Z", "Z")))
                ddlCodNatura3.Items.Add(new ListItem("Z", "Z"));
            ddlCodNatura3.SelectedValue = "Z";
            ddlCodNatura3.Enabled = false;
        }

        private void ManageCodNatura3AOI(DropDownList ddlCodNatura3)
        {
            ddlCodNatura3.SelectedValue = "H";
            ddlCodNatura3.Enabled = false;
        }

        private void ManageDecorrenzaForReversibilita(AreaTitolare.DatiPensione datiPensione, DateTime? decorrenzaPensioneDirettaDC)
        {
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
                ViewState["DecorrenzaPensione"] = decorrenzaPensioneDirettaDC;
            else
                ViewState["DecorrenzaPensione"] = datiPensione.DecorrenzaOriginaria;
        }

        private void ManageSperDonna(DropDownList ddlCodNatura2)
        {
            ddlCodNatura2.SelectedValue = "O";
            ddlCodNatura2.Enabled = false;
        }

        private void ManageCodNatura2Bonus(DropDownList ddlCodNatura2)
        {
            if (!(bool)ViewState["IsCodNatura2Enabled"])
            {
                ddlCodNatura2.SelectedValue = "Y";
                ddlCodNatura2.Enabled = false;
            }
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL)
            {
                gvRecordFondo.Enabled = false;
                txtPrimoVersamento.Enabled = false;
                txtUltimoVersamento.Enabled = false;
                ddlCodiceSpecifico.Enabled = false;
                ddlCodiceSpecifico_RF.Enabled = false;
                ddlCodiceConvenzione.Enabled = false;
                ddlCodArt22.Enabled = false;
                RequiredFieldValidator1.Enabled = false;
                txtInvalidita.Enabled = false;
                RequiredFieldValidator4.Enabled = false;
                txtVersamentiVolontariAA.Enabled = false;
                txtVersamentiVolontariMM.Enabled = false;
                txtVersamentiVolontariGG.Enabled = false;
                txtRiscattiRicongiunzioniAA.Enabled = false;
                txtRiscattiRicongiunzioniMM.Enabled = false;
                txtRiscattiRicongiunzioniGG.Enabled = false;
                ddlCodCapitalizzazione.Enabled = false;
                txtAliquotaIRPEF.Enabled = false;
                txtImportoPercentualeCapitalizzazione.Enabled = false;
                txtRetrAgoQuotaA.Enabled = false;
                txtRetrAgoQuotaB.Enabled = false;
                txtDirittoQuoteFisse.Enabled = false;
                ddlAttivitaSvolta.Enabled = false;
                ddlCodRequisiti1.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;
            }
        }
        #endregion Private Methods

        #region Fondo VL

        private void ValorizzaEtichetteDatiAssicurativiVL(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblAttivita.Text = "Attività Svolta:";
            ddlAttivitaSvolta_RF.ErrorMessage = "Attività svolta: Si prega di inserire l'Attività Svolta";

            if (datiPensione.DecorrenzaOriginaria == null)
                lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            else
            {
                String inputDecorrenza = datiPensione.DecorrenzaOriginaria.ToString();
                lblDecorrenzaPensioneDatiAssicurativi.Text = inputDecorrenza.Substring(3, 7);
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.HasValue)
                txtDirittoQuoteFisse.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.Value.ToString();

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ConvenzioneInternazionale.HasValue)
                    ddlCodiceConvenzione.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ConvenzioneInternazionale.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.CodiceArt22.HasValue)
                    ddlCodArt22.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.CodiceArt22.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.DataInvalidita.HasValue)
                    txtInvalidita.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.DataInvalidita.Value);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ProsecuzioneVolontariaAA.HasValue)
                    txtVersamentiVolontariAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ProsecuzioneVolontariaAA.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ProsecuzioneVolontariaMM.HasValue)
                    txtVersamentiVolontariMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ProsecuzioneVolontariaMM.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ProsecuzioneVolontariaGG.HasValue)
                    txtVersamentiVolontariGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ProsecuzioneVolontariaGG.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RiscattiRicongiunzioniAA.HasValue)
                    txtRiscattiRicongiunzioniAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RiscattiRicongiunzioniAA.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RiscattiRicongiunzioniMM.HasValue)
                    txtRiscattiRicongiunzioniMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RiscattiRicongiunzioniMM.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RiscattiRicongiunzioniGG.HasValue)
                    txtRiscattiRicongiunzioniGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RiscattiRicongiunzioniGG.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.CodiceCapitalizzazione.HasValue)
                    ddlCodCapitalizzazione.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.CodiceCapitalizzazione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.AliquotaIrpef.HasValue)
                    txtAliquotaIRPEF.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.AliquotaIrpef.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ImportoPercentualeCapitalizzazione.HasValue)
                    txtImportoPercentualeCapitalizzazione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.ImportoPercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaA.HasValue)
                    txtRetrAgoQuotaA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaB.HasValue)
                    txtRetrAgoQuotaB.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private DatiAssicurativi GetDatiAssicurativiVL(DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoVL = new Presenter.SvrLiquidazioneFs.DatiAssicurativi.FondoVL();

            datiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta.SelectedIndex >= 0 && !string.IsNullOrEmpty(ddlAttivitaSvolta.SelectedValue) ? ddlAttivitaSvolta.SelectedValue : null;
            datiAssicurativi.CodiceDirittoQuoteFisse = !string.IsNullOrEmpty(txtDirittoQuoteFisse.Text) ? CodeUtility.StringToNullableByte(txtDirittoQuoteFisse.Text) : null;

            datiAssicurativi.fondoVL.ConvenzioneInternazionale = ddlCodiceConvenzione.SelectedIndex != 0 ? Convert.ToChar(ddlCodiceConvenzione.SelectedValue) : (char?)null;
            datiAssicurativi.fondoVL.CodiceArt22 = ddlCodArt22.SelectedIndex != 0 ? Convert.ToByte(ddlCodArt22.SelectedValue) : (byte?)null;
            datiAssicurativi.fondoVL.DataInvalidita = (!string.IsNullOrEmpty(txtInvalidita.Text) && txtInvalidita.Text.ToUpperInvariant() != "GG/MM/AAAA") ? Convert.ToDateTime(txtInvalidita.Text) : (DateTime?)null;
            datiAssicurativi.fondoVL.ProsecuzioneVolontariaAA = !string.IsNullOrEmpty(txtVersamentiVolontariAA.Text) ? Convert.ToInt32(txtVersamentiVolontariAA.Text) : (int?)null;
            datiAssicurativi.fondoVL.ProsecuzioneVolontariaMM = !string.IsNullOrEmpty(txtVersamentiVolontariMM.Text) ? Convert.ToInt32(txtVersamentiVolontariMM.Text) : (int?)null;
            datiAssicurativi.fondoVL.ProsecuzioneVolontariaGG = !string.IsNullOrEmpty(txtVersamentiVolontariGG.Text) ? Convert.ToInt32(txtVersamentiVolontariGG.Text) : (int?)null;
            datiAssicurativi.fondoVL.RiscattiRicongiunzioniAA = !string.IsNullOrEmpty(txtRiscattiRicongiunzioniAA.Text) ? Convert.ToInt32(txtRiscattiRicongiunzioniAA.Text) : (int?)null;
            datiAssicurativi.fondoVL.RiscattiRicongiunzioniMM = !string.IsNullOrEmpty(txtRiscattiRicongiunzioniMM.Text) ? Convert.ToInt32(txtRiscattiRicongiunzioniMM.Text) : (int?)null;
            datiAssicurativi.fondoVL.RiscattiRicongiunzioniGG = !string.IsNullOrEmpty(txtRiscattiRicongiunzioniGG.Text) ? Convert.ToInt32(txtRiscattiRicongiunzioniGG.Text) : (int?)null;
            datiAssicurativi.fondoVL.CodiceCapitalizzazione = ddlCodCapitalizzazione.SelectedIndex != 0 ? Convert.ToByte(ddlCodCapitalizzazione.SelectedValue) : (byte?)null;
            datiAssicurativi.fondoVL.AliquotaIrpef = !string.IsNullOrEmpty(txtAliquotaIRPEF.Text) ? Convert.ToDecimal(txtAliquotaIRPEF.Text) : (decimal?)null;
            datiAssicurativi.fondoVL.ImportoPercentualeCapitalizzazione = !string.IsNullOrEmpty(txtImportoPercentualeCapitalizzazione.Text) ? Convert.ToDecimal(txtImportoPercentualeCapitalizzazione.Text) : (decimal?)null;
            datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaA = !string.IsNullOrEmpty(txtRetrAgoQuotaA.Text) ? Convert.ToDecimal(txtRetrAgoQuotaA.Text) : (decimal?)null;
            datiAssicurativi.fondoVL.RetribuzioneSettimanaleAgoQuotaB = !string.IsNullOrEmpty(txtRetrAgoQuotaB.Text) ? Convert.ToDecimal(txtRetrAgoQuotaB.Text) : (decimal?)null;

            return datiAssicurativi;
        }

        private void ManageValidatorVL(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Anzianita || tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Invalidita_Ordinaria)
                ddlCodiceSpecifico_RF.Enabled = true;
            else
                ddlCodiceSpecifico_RF.Enabled = false;
        }
        #endregion Fondo VL

        #region Fondo FS

        private void ValorizzaEtichetteDatiAssicurativiFS(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            ViewState["elencoCausaCessazione"] = liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList();

            ddlAttivitaSvoltaFS_RF.ErrorMessage = "Qualifica Professionale: Si prega di inserire la Qualifica Professionale";
            txtAttivitaSvoltaFS_RF.ErrorMessage = "Qualifica Professionale: Si prega di inserire la Qualifica Professionale";

            if (datiPensione.DecorrenzaOriginaria == null)
                lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            else
            {
                lblDecorrenzaPensioneDatiAssicurativi.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                txtDecorrenzaCalcolo.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                lblDecorrenzaCalcolo.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                txtDecorrenzaCalcolo.Enabled = false;
                txtDecorrenzaCalcolo.CssClass = txtDecorrenzaCalcolo.CssClass.Replace("date-picker-base", "");
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
            {
                if (((int?)ViewState["CountListaAttivitaSvolta"]).GetValueOrDefault() <= 10)
                    ddlAttivitaSvoltaFS.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;
                else
                    txtAttivitaSvoltaFS.Text = GetAttivitaSvoltaFS(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta, liquidazione);
            }

            if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaInabilitaLegge335(datiPensione))
            {
                if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.HasValue)
                {
                    var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, this.domanda.Tipofondo, 'F');
                    ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                }
                ddlCodiceSpecifico.Enabled = false;
            }

            if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione && Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico != null && liquidazione.areaLiquidazionePensioneFS.TipoPensione != null && this.domanda != null && this.domanda.Tipofondo != null)
                {
                    var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, this.domanda.Tipofondo, 'J');
                    ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                    ddlCodiceSpecifico.Enabled = false;
                }
            }

            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT) &&
              CodeUtility.IsRicostituzione(datiPensione) && 
              Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, areaDanteCausa, this.domanda.Categoria, liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null ? liquidazione.areaLiquidazionePensioneFS.TipoReversibilita : null, this.domanda.Tipofondo))
            {
                if (liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled.HasValue && liquidazione.areaLiquidazionePensioneFS.IsCodiceSpecificoEnabled == false)
                    ddlCodiceSpecifico.Enabled = false;
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.CausaCessazione.HasValue)
                    ddlCausaCessazioneFS.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.CausaCessazione.Value.ToString();

                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaInabilitaLegge335(datiPensione))
                {
                    if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.CausaCessazione.HasValue) ddlCausaCessazioneFS.SelectedValue = GetIdCausaCessazioneByTraduzionesuGP("515");
                    ddlCausaCessazioneFS.Enabled = false;
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.PagamentoIndennitaIntegrativaSpeciale.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.PagamentoIndennitaIntegrativaSpeciale.Value)
                        ddlPagIndennIntegrSpec.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.PagamentoIndennitaIntegrativaSpeciale.Value)
                        ddlPagIndennIntegrSpec.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata.Value)
                        ddlIndennIntegrSpecConglobata.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata.Value)
                        ddlIndennIntegrSpecConglobata.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.TrediciMensilita.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.TrediciMensilita.Value)
                        ddlTredicesimaMens.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.TrediciMensilita.Value)
                        ddlTredicesimaMens.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.TitolareAltraPensione.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.TitolareAltraPensione.Value)
                        ddlTitAltraPensione.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.TitolareAltraPensione.Value)
                        ddlTitAltraPensione.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.DirittoIndennitaIntegrativaSpeciale.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IntegrazioneMinimo.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IntegrazioneMinimo.Value)
                        ddlIntegrazioneMinimo.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IntegrazioneMinimo.Value)
                        ddlIntegrazioneMinimo.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.RiduzioneL537.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IISAbbattimentoAnni.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.VVUtiliDiritto.HasValue)
                    txtVVUtiliDiritto.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.VVUtiliDiritto.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.VVUtiliMisura.HasValue)
                    txtVVUtiliMisura.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoFST.VVUtiliMisura.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica.HasValue)
                    txtAttivitaEconomica.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica.ToString().PadLeft(2, '0');

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale.HasValue)
                    txtProfessioneIndividuale.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale.ToString().PadLeft(3, '0');
            }

            if (liquidazione.areaLiquidazionePensioneFS.IsPrimoVersamentoNonObbligatorio.GetValueOrDefault())
                requiredPrimoVersamento.Enabled = false;
        }

        private DatiAssicurativi GetDatiAssicurativiFS(DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoFST = new DatiAssicurativi.FondoFST();

            //if (!String.IsNullOrEmpty(txtCausaCessazione.Text))
            //    datiAssicurativi.fondoFST.CausaCessazione = ControlCausaCessazione(txtCausaCessazione.Text);
            datiAssicurativi.fondoFST.CausaCessazione = ddlCausaCessazioneFS.SelectedIndex != 0 ? Convert.ToInt64(ddlCausaCessazioneFS.SelectedValue) : (Int64?)null;

            if (((int?)ViewState["CountListaAttivitaSvolta"]).GetValueOrDefault() <= 10)
                datiAssicurativi.AttivitaSvolta = ddlAttivitaSvoltaFS.SelectedIndex >= 0 && !string.IsNullOrEmpty(ddlAttivitaSvoltaFS.SelectedValue) ? ddlAttivitaSvoltaFS.SelectedValue : null;
            else
                datiAssicurativi.AttivitaSvolta = ControlAttivitaSvoltaFS(ddlAttivitaSvoltaFS.SelectedValue);

            if (String.Equals(ddlPagIndennIntegrSpec.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.PagamentoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlPagIndennIntegrSpec.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.PagamentoIndennitaIntegrativaSpeciale = false;

            if (String.Equals(ddlTredicesimaMens.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.TrediciMensilita = true;
            else if (String.Equals(ddlTredicesimaMens.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.TrediciMensilita = false;

            if (!string.IsNullOrEmpty(hdnDecorrenzaCalcolo.Value))
                datiAssicurativi.fondoFST.DecorrenzaCalcolo = Convert.ToDateTime(hdnDecorrenzaCalcolo.Value);
            else
                datiAssicurativi.fondoFST.DecorrenzaCalcolo = (!string.IsNullOrEmpty(txtDecorrenzaCalcolo.Text) && txtDecorrenzaCalcolo.Text.ToUpperInvariant() != "GG/MM/AAAA") ? Convert.ToDateTime(txtDecorrenzaCalcolo.Text) : (DateTime?)null;

            if (String.Equals(ddlTitAltraPensione.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.TitolareAltraPensione = true;
            else if (String.Equals(ddlTitAltraPensione.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.TitolareAltraPensione = false;

            if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.DirittoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "NO"))
            {
                datiAssicurativi.fondoFST.DirittoIndennitaIntegrativaSpeciale = false;
                ddlRiduzioneL537.SelectedValue = "NO";
                ddlIISAbbattimentoAnni.SelectedValue = "NO";
                ddlIndennIntegrSpecConglobata.SelectedValue = "NO";
            }

            if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.IntegrazioneMinimo = true;
            else if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.IntegrazioneMinimo = false;

            if (String.Equals(ddlRiduzioneL537.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.RiduzioneL537 = true;
            else if (String.Equals(ddlRiduzioneL537.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.RiduzioneL537 = false;

            if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.IISAbbattimentoAnni = true;
            else if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.IISAbbattimentoAnni = false;

            if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "SI"))
                datiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata = true;
            else if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "NO"))
                datiAssicurativi.fondoFST.IndennitaIntegrativaSpecialeConglobata = false;

            if (!String.IsNullOrEmpty(txtVVUtiliDiritto.Text))
                datiAssicurativi.fondoFST.VVUtiliDiritto = CodeUtility.StringToNullableShort(txtVVUtiliDiritto.Text);

            if (!String.IsNullOrEmpty(txtVVUtiliMisura.Text))
                datiAssicurativi.fondoFST.VVUtiliMisura = CodeUtility.StringToNullableShort(txtVVUtiliMisura.Text);

            return datiAssicurativi;
        }

        private string GetAttivitaSvoltaFS(string idAttivitaSvolta, ILiquidazionePensione liquidazione)
        {
            if (!string.IsNullOrEmpty(idAttivitaSvolta))
            {
                DatiAttivitaSvolta attivitaSvolta = Array.Find(liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte, x => x.Id == idAttivitaSvolta);
                return attivitaSvolta.TraduzioneSuGp + " - " + attivitaSvolta.Descrizione + " (" + attivitaSvolta.LimiteEta + " - " + attivitaSvolta.LimiteServizio + ")";
            }
            else return string.Empty;
        }

        private string ControlAttivitaSvoltaFS(string attivitaSvoltaInserita)
        {
            string attivitaSvolta = null;
            List<DatiAttivitaSvolta> listaAttivitaSvolte = (List<DatiAttivitaSvolta>)ViewState["ListaAttivitaSvolte"];

            if (!string.IsNullOrEmpty(attivitaSvoltaInserita))
            {
                char[] separatori = { '-', '(', ')' };
                string traduzioneSuGP = attivitaSvoltaInserita.Split(separatori).ElementAt(0).Trim();
                string descrizione = attivitaSvoltaInserita.Split(separatori).ElementAt(1).Trim();
                string limiteEta = attivitaSvoltaInserita.Split(separatori).ElementAt(2).Trim();
                string limiteServizio = attivitaSvoltaInserita.Split(separatori).ElementAt(3).Trim();

                foreach (DatiAttivitaSvolta attivitaSvoltaDB in listaAttivitaSvolte)
                {
                    if (attivitaSvoltaDB.TraduzioneSuGp.Trim() == traduzioneSuGP && attivitaSvoltaDB.Descrizione.Trim().ToUpperInvariant() == descrizione.ToUpperInvariant() &&
                        attivitaSvoltaDB.LimiteEta == byte.Parse(limiteEta) && attivitaSvoltaDB.LimiteServizio == byte.Parse(limiteServizio))
                    {
                        attivitaSvolta = attivitaSvoltaDB.Id;
                        break;
                    }
                }
            }

            return attivitaSvolta;
        }

        private void LoadDdlFS(ILiquidazionePensione liquidazione)
        {
            //string elencoCausaCessazione = string.Empty;
            //foreach (CausaCessazione causaCessazione in liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList())
            //{
            //    elencoCausaCessazione = string.Concat(causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione, ";");
            //    HiddenFieldCausaCessazione.Value = string.Concat(HiddenFieldCausaCessazione.Value, elencoCausaCessazione);
            //}
            ddlCausaCessazioneFS.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCausaCessazioneFS);
            foreach (CausaCessazione causaCessazione in liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList())
                CodeUtility.SetValueDdl(ddlCausaCessazioneFS, causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione, causaCessazione.Descrizione, causaCessazione.Id.ToString());


            ViewState["CountListaAttivitaSvolta"] = liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte.Count();

            if (((int?)ViewState["CountListaAttivitaSvolta"]).GetValueOrDefault() <= 10)
            {
                pnlDDLAttivitaSvoltaFS.Visible = true;
                ddlAttivitaSvoltaFS.Items.Clear();
                CodeUtility.SetItemBlankDdl(ddlAttivitaSvoltaFS);
                foreach (DatiAttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte.ToList())
                    CodeUtility.SetValueDdl(ddlAttivitaSvoltaFS, attivitaSvolta.Descrizione, attivitaSvolta.Descrizione, attivitaSvolta.Id);
            }
            else
            {
                pnlTXTAttivitaSvoltaFS.Visible = true;
                string elencoAttivitaSvolta = string.Empty;
                foreach (DatiAttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte)
                {
                    elencoAttivitaSvolta = string.Concat(attivitaSvolta.TraduzioneSuGp + " - " + attivitaSvolta.Descrizione + " (" + attivitaSvolta.LimiteEta + " - " + attivitaSvolta.LimiteServizio + ")", ";");
                    hiddenAttivitaSvolte.Value = string.Concat(hiddenAttivitaSvolte.Value, elencoAttivitaSvolta);
                }
            }
        }

        private long? ControlCausaCessazione(string causaCessazioneInserita)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            long? codCausaCessazione = null;
            string[] cessazione = causaCessazioneInserita.Split('-');

            if (elencoCausaCessazione != null)
            {
                foreach (CausaCessazione causaCessazione in elencoCausaCessazione)
                {
                    if (causaCessazione.TraduzioneSuGP.ToUpperInvariant().Trim() == cessazione[0].ToUpperInvariant().Trim() && causaCessazione.Descrizione.ToUpperInvariant().Trim() == cessazione[1].Trim().ToUpperInvariant())
                        codCausaCessazione = causaCessazione.Id;
                }
            }

            return codCausaCessazione;
        }

        private string GetCausaCessazione(long? codCausaCessazione)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            if (codCausaCessazione.HasValue && elencoCausaCessazione != null)
            {
                CausaCessazione causaCessazione = elencoCausaCessazione.Find(delegate(CausaCessazione code)
                { return (code.Id == codCausaCessazione.Value); });
                return causaCessazione != null ? causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione : string.Empty;
            }
            else return string.Empty;
        }

        private string GetIdCausaCessazioneByTraduzionesuGP(string traduzioneSuGP)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            if (traduzioneSuGP != null && elencoCausaCessazione != null)
            {
                CausaCessazione causaCessazione = elencoCausaCessazione.Find(delegate(CausaCessazione code)
                { return (code.TraduzioneSuGP.Trim() == traduzioneSuGP); });
                return causaCessazione != null ? causaCessazione.Id.ToString() : string.Empty;
            }
            else return string.Empty;
        }


        private byte? GetIdCodiceSpecificoByTraduzioneSuGP(ILiquidazionePensione liquidazione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, char traduzioneSuGP)
        {
            CodiceSpecifico codiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().Find(delegate(CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                  code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant() && code.TraduzioneGp == traduzioneSuGP);
            });
            return codiceSpecifico != null ? codiceSpecifico.Id : null;
        }

        #endregion Fondo FS

        #region Fondo PT

        private void ValorizzaEtichetteDatiAssicurativiPT(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            lblAttivita.Text = "Qualifica Professionale:";
            ddlAttivitaSvolta_RF.ErrorMessage = "Qualifica Professionale: Si prega di inserire la Qualifica Professionale";

            ViewState["elencoCausaCessazione"] = liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList();

            if (datiPensione.DecorrenzaOriginaria == null)
                lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            else
            {
                lblDecorrenzaPensioneDatiAssicurativi.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                lblDecorrenzaCalcolo.Text = string.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaInabilitaLegge335(datiPensione))
            {
                if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.HasValue)
                {
                    var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, this.domanda.Tipofondo, 'F');
                    ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                }
                ddlCodiceSpecifico.Enabled = false;
            }

            if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione && Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, this.domanda.Tipofondo, 'J');
                ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                ddlCodiceSpecifico.Enabled = false;
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.CausaCessazione.HasValue)
                    ddlCausaCessazionePT.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.CausaCessazione.Value.ToString();
                //txtCausaCessazionePT.Text = GetCausaCessazione(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.CausaCessazione.Value);

                if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && Utility.IsDomandaInabilitaLegge335(datiPensione))
                {
                    if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.CausaCessazione.HasValue) ddlCausaCessazionePT.SelectedValue = GetIdCausaCessazioneByTraduzionesuGP("515");
                    ddlCausaCessazionePT.Enabled = false;
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.PagamentoIndennitaIntegrativaSpeciale.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.PagamentoIndennitaIntegrativaSpeciale.Value)
                        ddlPagIndennIntegrSpec.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.PagamentoIndennitaIntegrativaSpeciale.Value)
                        ddlPagIndennIntegrSpec.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IndennitaIntegrativaSpecialeConglobata.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IndennitaIntegrativaSpecialeConglobata.Value)
                        ddlIndennIntegrSpecConglobata.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IndennitaIntegrativaSpecialeConglobata.Value)
                        ddlIndennIntegrSpecConglobata.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.TrediciMensilita.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.TrediciMensilita.Value)
                        ddlTredicesimaMens.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.TrediciMensilita.Value)
                        ddlTredicesimaMens.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.DecorrenzaCalcolo.HasValue)
                    txtDecorrenzaCalcolo.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.DecorrenzaCalcolo.Value);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.DirittoIndennitaIntegrativaSpeciale.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IntegrazioneMinimo.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IntegrazioneMinimo.Value)
                        ddlIntegrazioneMinimo.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IntegrazioneMinimo.Value)
                        ddlIntegrazioneMinimo.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.RiduzioneL537.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IISAbbattimentoAnni.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.OnereMEF.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.OnereMEF.Value)
                        ddlOnereMEF.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.OnereMEF.Value)
                        ddlOnereMEF.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.RipartizioneInpdap.HasValue)
                    txtRipartizioneInpdap.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.RipartizioneInpdap.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.VVUtiliDiritto.HasValue)
                    txtVVUtiliDiritto.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.VVUtiliDiritto.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.VVUtiliMisura.HasValue)
                    txtVVUtiliMisura.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPT.VVUtiliMisura.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica.HasValue)
                    txtAttivitaEconomica.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica.ToString().PadLeft(2, '0');

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale.HasValue)
                    txtProfessioneIndividuale.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale.ToString().PadLeft(3, '0');
            }
        }

        private DatiAssicurativi GetDatiAssicurativiPT(DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoPT = new DatiAssicurativi.FondoPT();

            datiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta.SelectedIndex >= 0 && !string.IsNullOrEmpty(ddlAttivitaSvolta.SelectedValue) ? ddlAttivitaSvolta.SelectedValue : null;

            datiAssicurativi.fondoPT.CausaCessazione = ddlCausaCessazionePT.SelectedIndex != 0 ? Convert.ToInt64(ddlCausaCessazionePT.SelectedValue) : (Int64?)null;

            if (String.Equals(ddlPagIndennIntegrSpec.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.PagamentoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlPagIndennIntegrSpec.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.PagamentoIndennitaIntegrativaSpeciale = false;

            if (String.Equals(ddlTredicesimaMens.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.TrediciMensilita = true;
            else if (String.Equals(ddlTredicesimaMens.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.TrediciMensilita = false;

            datiAssicurativi.fondoPT.DecorrenzaCalcolo = (!string.IsNullOrEmpty(txtDecorrenzaCalcolo.Text) && txtDecorrenzaCalcolo.Text.ToUpperInvariant() != "GG/MM/AAAA") ? Convert.ToDateTime(txtDecorrenzaCalcolo.Text) : (DateTime?)null;

            if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.DirittoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "NO"))
            {
                datiAssicurativi.fondoPT.DirittoIndennitaIntegrativaSpeciale = false;
                ddlRiduzioneL537.SelectedValue = "NO";
                ddlIISAbbattimentoAnni.SelectedValue = "NO";
                ddlIndennIntegrSpecConglobata.SelectedValue = "NO";
            }

            if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.IntegrazioneMinimo = true;
            else if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.IntegrazioneMinimo = false;

            if (String.Equals(ddlRiduzioneL537.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.RiduzioneL537 = true;
            else if (String.Equals(ddlRiduzioneL537.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.RiduzioneL537 = false;

            if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.IISAbbattimentoAnni = true;
            else if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.IISAbbattimentoAnni = false;

            if (String.Equals(ddlOnereMEF.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.OnereMEF = true;
            else if (String.Equals(ddlOnereMEF.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.OnereMEF = false;

            if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "SI"))
                datiAssicurativi.fondoPT.IndennitaIntegrativaSpecialeConglobata = true;
            else if (String.Equals(ddlIndennIntegrSpecConglobata.SelectedValue, "NO"))
                datiAssicurativi.fondoPT.IndennitaIntegrativaSpecialeConglobata = false;

            datiAssicurativi.fondoPT.RipartizioneInpdap = !string.IsNullOrEmpty(txtRipartizioneInpdap.Text) ? Convert.ToDecimal(txtRipartizioneInpdap.Text) : (decimal?)null;

            if (!string.IsNullOrEmpty(txtVVUtiliDiritto.Text))
                datiAssicurativi.fondoPT.VVUtiliDiritto = CodeUtility.StringToNullableShort(txtVVUtiliDiritto.Text);

            if (!string.IsNullOrEmpty(txtVVUtiliMisura.Text))
                datiAssicurativi.fondoPT.VVUtiliMisura = CodeUtility.StringToNullableShort(txtVVUtiliMisura.Text);

            return datiAssicurativi;
        }

        private void LoadDdlPT(ILiquidazionePensione liquidazione)
        {
            ddlCausaCessazionePT.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCausaCessazionePT);
            foreach (CausaCessazione causaCessazione in liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList())
                CodeUtility.SetValueDdl(ddlCausaCessazionePT, causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione, causaCessazione.Descrizione, causaCessazione.Id.ToString());
        }

        #endregion Fondo PT

        #region Common VL - PT

        private void LoadDdlVL_PT(ILiquidazionePensione liquidazione)
        {
            ddlAttivitaSvolta.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttivitaSvolta);
            if (liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte != null)
                foreach (DatiAttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte)
                    CodeUtility.SetValueDdl(ddlAttivitaSvolta, attivitaSvolta.Descrizione, attivitaSvolta.Descrizione, attivitaSvolta.Id);
        }

        #endregion Common VL - PT

        #region ILiquidazionePensione
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        #endregion ILiquidazionePensione

        #region IRecordFondo
        public RecordFondo[] areaArrayRecordFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IRecordFondo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region EventHandler

        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler GetDecorrenzaPensione;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        protected void RaiseDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (DisabilitaTastoSalva != null)
                DisabilitaTastoSalva(sender, e);
        }

        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion EventHandler
    }
}
