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
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiAssicurativiEL_TT_ET : CustomBaseUserControl, ITitolarePensione, IRecordFondo, ILiquidazionePensione
    {
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
                btnSalvaDatiAssicurativi.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
                ValorizzaEtichetteDatiAssicurativi(this, datiPensione, IsDomandaSperDonna, datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione);
            }
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        internal void ValorizzaEtichetteDatiAssicurativi(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna, bool isDomandaInabilitaAmianto)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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

            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
                ViewState["IsUsuranti"] = "SI";

            if (liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.Value)
                ViewState["IsDomandaTrasformazioneAOI"] = "SI";

            if (IsDomandaSperDonna)
                ViewState["IsDomandaSperDonna"] = "SI";

            //Eng - OpzioneDonna_Legge197 secondo byte del codice requisiti prepopolato con il valore 9
            if (liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.HasValue)
            {
                ViewState["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"] = liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.Value;
                if ((liquidazione.areaLiquidazionePensioneFS.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione.Value) && (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.HasValue))
                {
                    txtCodiceRequisiti2.Text = "9";
                }
            }


            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue)
                ViewState["IsCodNatura2Enabled"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.HasValue)
                ViewState["IsCodiceNatura2DisabledPerSperDonna"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.Value;

            if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivoConOpzione.GetValueOrDefault() || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((!String.IsNullOrEmpty(controlloDinamicoMemo123_2024) && controlloDinamicoMemo123_2024.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024OpzioneContrib) && controlloDinamicoMemo123_2024OpzioneContrib.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                ViewState["IsPensioneTipoContributivoConOpzione"] = "SI";

            if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
            {
                // DPR Armonizzazione
                if (liquidazione.areaLiquidazionePensioneFS.ListaPersonaleViaggiante != null)
                    ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()] = liquidazione.areaLiquidazionePensioneFS.ListaPersonaleViaggiante.ToList();
                ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante.ToString()] = liquidazione.areaLiquidazionePensioneFS.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante;
                ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante.ToString()] = liquidazione.areaLiquidazionePensioneFS.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante;
                // --------------------
            }

            LoadDdl(this.domanda.Tipofondo, liquidazione);
            ValorizzaEtichetteDatiAssicurativiCommon(liquidazione, datiPensione);
            ValorizzaCodeRequisiti2SperDonna(IsDomandaSperDonna, liquidazione);
            ValorizzaEtichetteByTipoFondo(liquidazione, datiPensione);

            if (isDomandaInabilitaAmianto)
            {
                pnlAttEconomProfInd.Visible = true;
                txtAttivitaEconomica.Text = "01";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "250";
                txtProfessioneIndividuale.Enabled = false;
            }

            if (Utility.IsDomandaReversibilita(datiPensione))
                ddlCodiceSpecifico_RF.Enabled = false;

            GestioneEtichetteRic(datiPensione);
        }

        private void ValorizzaEtichetteByTipoFondo(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            switch (domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    pnlEL.Visible = true;
                    pnlCommonEL_TT.Visible = true;
                    VisualizzaPannelliEL(datiPensione, liquidazione.areaLiquidazionePensioneFS);
                    ValorizzaEtichetteDatiAssicurativiEL(liquidazione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    pnlET.Visible = true;
                    pnlCommonTT_ET.Visible = true;
                    VisualizzaPannelliET(datiPensione, liquidazione.areaLiquidazionePensioneFS);
                    ValorizzaEtichetteDatiAssicurativiET(liquidazione, datiPensione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    pnlTT.Visible = true;
                    pnlCommonEL_TT.Visible = true;
                    pnlCommonTT_ET.Visible = true;
                    VisualizzaPannelliTT(datiPensione, liquidazione.areaLiquidazionePensioneFS);
                    ValorizzaEtichetteDatiAssicurativiTT(liquidazione, datiPensione);
                    break;
            }
        }

        internal INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, out List<RecordFondo> listaRecordFondo)
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = null;
            areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiCommon(out listaRecordFondo);
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiEL(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiET(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiTT(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
            }

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        #region GridView gvRecordFondo

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
            List<RecordFondo> listaRecordFondo = BindData();
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

                                ddlCodNatura1.Items.Clear();
                                ddlCodNatura2.Items.Clear();
                                ddlCodNatura3.Items.Clear();
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

                            if (ViewState["IsDomandaTrasformazioneAOI"] != null)
                                ManageCodNatura3AOI(ddlCodNatura3);

                            if (ViewState["IsDomandaSperDonna"] != null)
                                ManageSperDonna(ddlCodNatura2);

                            if (ViewState["IsCodNatura2Enabled"] != null)
                                ManageCodNatura2Bonus(ddlCodNatura2);

                            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2, (bool)ViewState["IsCodiceNatura2DisabledPerSperDonna"]);

                            //Eng - OpzioneDonna_Legge197 nel pannello del Record Fondo, il secondo byte del codice natura “O” non editabile
                            if (ViewState["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"] != null)
                                CodeUtility.DisableCodNatura2PerOpzioneDonna_Legge197_2022_Art1_Comma292(ddlCodNatura2, (bool)(ViewState["IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione"]));

                            if (CodeUtility.IsTipoContributivoConOpzione(datiPensione, ViewState["IsPensioneTipoContributivoConOpzione"] != null ? true : false))
                            {
                                ddlCodNatura2.ClearSelection();
                                if (ddlCodNatura2.Items.FindByValue("J") != null)
                                    ddlCodNatura2.SelectedValue = "J";

                                ddlCodNatura2.Enabled = false;
                            }

                            if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                            {
                                //DPR Armonizzazione
                                ManageCodNatura2PerPersonaleViaggiante(ddlCodNatura2);
                                //----------------------
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
                            ddlCodNatura1.Items.Clear();
                            ddlCodNatura2.Items.Clear();
                            ddlCodNatura3.Items.Clear();
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

        #endregion GridView gvRecordFondo

        #region Methods private & comuni a tutti i fondi

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            this.txtRiscattiAA.Text = "AA";
            this.txtRiscattiMM.Text = "MM";
            this.txtAnzianitaPregressaAA.Text = "AA";
            this.txtAnzianitaPregressaMM.Text = "MM";
            this.txtServizioMilitareAA.Text = "AA";
            this.txtServizioMilitareMM.Text = "MM";
            this.txtArt3AA.Text = "AA";
            this.txtArt3MM.Text = "MM";
            txtCodiceRequisiti2.Text = "0";
        }

        private void ValorizzaCodeRequisiti2SperDonna(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione)
        {
            if (IsDomandaSperDonna && (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.HasValue))
                txtCodiceRequisiti2.Text = "9";
        }

        private void LoadDdl(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    LoadDdlEL(datiDecodifica);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    LoadDdlET(liquidazione, datiDecodifica);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    LoadDdlTT(datiDecodifica);
                    break;
            }

            List<CodiceSpecifico> listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().FindAll(delegate(CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                  code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant());
            });

            ViewState["CodiciNatura"] = liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura;

            ddlCodRequisiti1.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodRequisiti1);
            foreach (CodiceRequisito1 codReq1 in liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito1)
                CodeUtility.SetValueDdl(ddlCodRequisiti1, codReq1.Id + " - " + codReq1.Descrizione, codReq1.Descrizione, codReq1.Id);

            ddlAttivitaSvolta.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttivitaSvolta);
            if (liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte != null)
                foreach (DatiAttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte)
                    CodeUtility.SetValueDdl(ddlAttivitaSvolta, attivitaSvolta.Descrizione, attivitaSvolta.Descrizione, attivitaSvolta.Id);

            ddlCodiceConvenzione.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceConvenzione);
            foreach (CodiceConvenzioneInternazionale codConvInternazionale in liquidazione.areaLiquidazionePensioneFS.ListaCodiceConvenzioneInternazionale)
                CodeUtility.SetValueDdl(ddlCodiceConvenzione, codConvInternazionale.Descrizione, codConvInternazionale.Descrizione, codConvInternazionale.Id);

            ddlCodiceSpecifico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceSpecifico);
            foreach (CodiceSpecifico codSpec in listaCodiceSpecifico)
                CodeUtility.SetValueDdl(ddlCodiceSpecifico, codSpec.Descrizione, codSpec.Descrizione, codSpec.Id.Value.ToString());

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito1 != null)
                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                    ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();
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

        private void ManageCodNatura2PerPersonaleViaggiante(DropDownList ddlCodNatura2)
        {
            if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
            {
                ddlCodNatura2.SelectedValue = "W";
                ddlCodNatura2.Enabled = false;
            }
            else if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
            {
                ddlCodNatura2.SelectedValue = "K";
                ddlCodNatura2.Enabled = false;
            }
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            //Eng - Il tab Assicurativi è visibile per le Ricostituzioni non FS/PT/INPDAP con Prodotto "0109" e Tipo "0130"
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.CodeProdotto != "0109" && datiPensione.CodeTipo != "0130")
            {
                //COMMON
                gvRecordFondo.Enabled = false;
                txtPrimoVersamento.Enabled = false;
                txtUltimoVersamento.Enabled = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                ddlCodiceSpecifico.Enabled = false;
                ddlCodiceSpecifico_RF.Enabled = false;
                ddlCodRequisiti1.Enabled = false;
                txtDirittoQuoteFisse.Enabled = false;
                txtCodiceRequisiti2.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET)
                {
                    txtDecorrenzaTeorica.Enabled = false;
                    txtRenditaInail.Enabled = false;
                    txtRetribEffettivaInail.Enabled = false;
                }

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL)
                {
                    ddlCodiceConvenzione.Enabled = false;
                    ddlAttivitaSvolta.Enabled = false;
                    ddlAttivitaSvolta_RF.Enabled = false;
                }

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL)
                {
                    txtRiscattiAA.Enabled = false;
                    txtRiscattiMM.Enabled = false;
                    txtAnzianitaPregressaAA.Enabled = false;
                    txtAnzianitaPregressaMM.Enabled = false;
                    txtServizioMilitareAA.Enabled = false;
                    txtServizioMilitareMM.Enabled = false;
                    txtArt3AA.Enabled = false;
                    txtArt3MM.Enabled = false;
                    ddlCodDirittoQuoteFisse_EL.Enabled = false;
                    ddlGradoInvalidita.Enabled = false;
                    txtPercentualeMaggiorazione.Enabled = false;
                    ddlProrataEnel.Enabled = false;
                    ddlCodiceAziendaEL.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT)
                {
                    chkDimissioniAnte97.Enabled = false;
                    txtContributiFissiAnno.Enabled = false;
                    txtContributiFissiMese.Enabled = false;
                    txtContributiFissiGiorno.Enabled = false;
                    txtRiscattiRiservaAnno.Enabled = false;
                    txtRiscattiRiservaMese.Enabled = false;
                    txtRiscattiRiservaGiorno.Enabled = false;
                    txtPeriodiFigurativiAnno.Enabled = false;
                    txtPeriodiFigurativiMese.Enabled = false;
                    txtPeriodiFigurativiGiorno.Enabled = false;
                    txtSupplementoOBG.Enabled = false;
                    ddlDitta.Enabled = false;
                    txtPensioneGenitori.Enabled = false;
                    chkArt5Legge58.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET)
                {
                    ddlPartTime.Enabled = false;
                    txtCessazioneIscrizione.Enabled = false;
                    txtInterrPTVAnno.Enabled = false;
                    txtInterrPTVMese.Enabled = false;
                    txtInterrPTVGiorno.Enabled = false;
                    ddlServizioMilitare.Enabled = false;
                    txtNSettimaneLeva.Enabled = false;
                    txtNSettimanaRichiamato.Enabled = false;
                    txtContribAgoL402.Enabled = false;
                    txtContribAgoL140.Enabled = false;
                    txtAziendaET.Enabled = false;
                    txtAziendaET_RF.Enabled = false;
                    txtStipendio.Enabled = false;
                    RFVtxtStipendio.Enabled = false;
                    txtTredicesima.Enabled = false;
                    RFVtxtTredicesima.Enabled = false;
                    txtQuattordicesima.Enabled = false;
                    txtElementiAccessori.Enabled = false;
                    txtCompetenze.Enabled = false;
                    ddlEsodo.Enabled = false;
                    txtRetribuzioneEsodo.Enabled = false;
                    txtGradoInvalidita.Enabled = false;
                    ddlPersonaleViaggiante.Enabled = false;
                }
            }

            if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione))
            {
                txtStipendio.Enabled = false;
                txtTredicesima.Enabled = false;
                txtQuattordicesima.Enabled = false;
                txtElementiAccessori.Enabled = false;
                txtCompetenze.Enabled = false;
                txtRetribuzioneEsodo.Enabled = false;
                txtRetribEffettivaInail.Enabled = false;
            }
        }
        #endregion Methods private & comuni a tutti i fondi

        #region Fondi Comuni //Elettrici - Telefonici

        private void ValorizzaEtichetteDatiAssicurativiCommon(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
            {
                List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                ViewState["elencoRecordFondo"] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                BindData();
            }

            if (liquidazione.areaLiquidazionePensioneFS.TipoPensione != null)
            {
                lblTipoPensione.Text = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Key;
                hdnTipoPensione.Value = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString();
            }

            if (datiPensione.DecorrenzaOriginaria == null)
                lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            else
            {
                String inputDecorrenza = datiPensione.DecorrenzaOriginaria.ToString();
                lblDecorrenzaPensioneDatiAssicurativi.Text = inputDecorrenza.Substring(3, 7);
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione != null)
                txtPrimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione);

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione != null)
                txtUltimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione);

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2 != null)
                txtCodiceRequisiti2.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.ToString();

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico != null)
                ddlCodiceSpecifico.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.ToString();

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 != null)
                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                    ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.HasValue)
                txtDirittoQuoteFisse.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.Value.ToString();

            if (datiPensione.FlagUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.HasValue)
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
                    GestioneEtichetteIsUnicarpe(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi, datiPensione);
            }
        }

        private void GestioneEtichetteIsUnicarpe(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi, AreaTitolare.DatiPensione datiPensione)
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
                }
            }
        }

        private INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiCommon(out List<RecordFondo> listaRecordFondo)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
            CodeUtility.EliminaRecordVuoti(listaRecordFondo);
            areaLiquidazionePensioneFS.ListaRecordFondo = listaRecordFondo.ToArray();

            if (!(String.Equals(txtPrimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtPrimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtPrimoVersamento.Text);

            if (!(String.Equals(txtUltimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtUltimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtUltimoVersamento.Text);

            if (!String.IsNullOrEmpty(ddlAttivitaSvolta.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta.SelectedValue;

            if (!(String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue)))
            {
                byte? bNull = null;
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico = !String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue) ? Convert.ToByte(ddlCodiceSpecifico.SelectedValue) : bNull;
            }

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

            if (!String.IsNullOrEmpty(txtDirittoQuoteFisse.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse = CodeUtility.StringToNullableByte(txtDirittoQuoteFisse.Text);

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        #endregion Fondi Comuni //Elettrici - Telefonici

        #region Fondo EL

        private void ValorizzaEtichetteDatiAssicurativiEL(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoRiscatti.HasValue)
                    txtRiscattiAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoRiscatti.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseRiscatti.HasValue)
                    txtRiscattiMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseRiscatti.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoAnzianitaPregressa.HasValue)
                    txtAnzianitaPregressaAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoAnzianitaPregressa.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseAnzianitaPregressa.HasValue)
                    txtAnzianitaPregressaMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseAnzianitaPregressa.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoServizioMilitare.HasValue)
                    txtServizioMilitareAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoServizioMilitare.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseServizioMilitare.HasValue)
                    txtServizioMilitareMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseServizioMilitare.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoArt3Legge107971.HasValue)
                    txtArt3AA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.AnnoArt3Legge107971.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseArt3Legge107971.HasValue)
                    txtArt3MM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.MeseArt3Legge107971.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.ProRataEnel.HasValue)
                    ddlProrataEnel.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.ProRataEnel.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.CodiceAzienda.HasValue)
                    ddlCodiceAziendaEL.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.CodiceAzienda.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.DecorrenzaTeorica.HasValue)
                    txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.DecorrenzaTeorica);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.ConvenzioneInternazionale.HasValue)
                    ddlCodiceConvenzione.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.ConvenzioneInternazionale.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse != null)
                    ddlCodDirittoQuoteFisse_EL.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.ToString();

                if (pnlInvalidita_MaggiorazioneAnte97.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.GradoInvalidita.HasValue)
                        ddlGradoInvalidita.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.GradoInvalidita.ToString();
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.PercentualeMaggiorazione.HasValue)
                        txtPercentualeMaggiorazione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoEL.PercentualeMaggiorazione.ToString();
                }
            }
        }

        private INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiEL(INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoEL = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi.FondoEL();

            if (!(String.IsNullOrEmpty(txtRiscattiAA.Text)) && (!String.Equals(txtRiscattiAA.Text, "AA")))
                datiAssicurativi.fondoEL.AnnoRiscatti = byte.Parse(txtRiscattiAA.Text);

            if (!(String.IsNullOrEmpty(txtRiscattiMM.Text)) && (!String.Equals(txtRiscattiMM.Text, "MM")))
                datiAssicurativi.fondoEL.MeseRiscatti = byte.Parse(txtRiscattiMM.Text);

            if (!(String.IsNullOrEmpty(txtAnzianitaPregressaAA.Text)) && (!String.Equals(txtAnzianitaPregressaAA.Text, "AA")))
                datiAssicurativi.fondoEL.AnnoAnzianitaPregressa = byte.Parse(txtAnzianitaPregressaAA.Text);

            if (!(String.IsNullOrEmpty(txtAnzianitaPregressaMM.Text)) && (!String.Equals(txtAnzianitaPregressaMM.Text, "MM")))
                datiAssicurativi.fondoEL.MeseAnzianitaPregressa = byte.Parse(txtAnzianitaPregressaMM.Text);

            if (!(String.IsNullOrEmpty(txtServizioMilitareAA.Text)) && (!String.Equals(txtServizioMilitareAA.Text, "AA")))
                datiAssicurativi.fondoEL.AnnoServizioMilitare = byte.Parse(txtServizioMilitareAA.Text);

            if (!(String.IsNullOrEmpty(txtServizioMilitareMM.Text)) && (!String.Equals(txtServizioMilitareMM.Text, "MM")))
                datiAssicurativi.fondoEL.MeseServizioMilitare = byte.Parse(txtServizioMilitareMM.Text);

            if (!(String.IsNullOrEmpty(txtArt3AA.Text)) && (!String.Equals(txtArt3AA.Text, "AA")))
                datiAssicurativi.fondoEL.AnnoArt3Legge107971 = byte.Parse(txtArt3AA.Text);
            if (!(String.IsNullOrEmpty(txtArt3MM.Text)) && (!String.Equals(txtArt3MM.Text, "MM")))
                datiAssicurativi.fondoEL.MeseArt3Legge107971 = byte.Parse(txtArt3MM.Text);

            if (!String.IsNullOrEmpty(ddlProrataEnel.SelectedValue))
                datiAssicurativi.fondoEL.ProRataEnel = byte.Parse(ddlProrataEnel.SelectedValue);

            if (!String.IsNullOrEmpty(ddlCodiceAziendaEL.SelectedValue))
                datiAssicurativi.fondoEL.CodiceAzienda = Convert.ToInt64(ddlCodiceAziendaEL.SelectedValue);

            if (!(String.Equals(txtDecorrenzaTeorica.Text, "mm/aaaa")) && (!String.IsNullOrEmpty(txtDecorrenzaTeorica.Text)))
                datiAssicurativi.fondoEL.DecorrenzaTeorica = Utility.GetDateFromString(txtDecorrenzaTeorica.Text);

            if (!String.IsNullOrEmpty(ddlCodiceConvenzione.SelectedValue))
                datiAssicurativi.fondoEL.ConvenzioneInternazionale = char.Parse(ddlCodiceConvenzione.SelectedValue);

            if (!string.IsNullOrEmpty(ddlCodDirittoQuoteFisse_EL.SelectedValue))
                datiAssicurativi.CodiceDirittoQuoteFisse = byte.Parse(ddlCodDirittoQuoteFisse_EL.SelectedValue);

            if (pnlInvalidita_MaggiorazioneAnte97.Visible)
            {
                if (!String.IsNullOrEmpty(ddlGradoInvalidita.SelectedValue))
                    datiAssicurativi.fondoEL.GradoInvalidita = CodeUtility.StringToNullableByte(ddlGradoInvalidita.SelectedValue);
                if (!String.IsNullOrEmpty(txtPercentualeMaggiorazione.Text))
                    datiAssicurativi.fondoEL.PercentualeMaggiorazione = CodeUtility.StringToNullableByte(txtPercentualeMaggiorazione.Text);
            }

            return datiAssicurativi;
        }

        private void LoadDdlEL(AreaDecodifica datiDecodifica)
        {
            AreaDecodifica.DatiGradoInvalidita[] listaGradoInvalidita = datiDecodifica.ElencoGradoInvalidita;

            ddlGradoInvalidita.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlGradoInvalidita);
            foreach (AreaDecodifica.DatiGradoInvalidita gradoInvalidita in listaGradoInvalidita)
                CodeUtility.SetValueDdl(ddlGradoInvalidita, gradoInvalidita.Descrizione, gradoInvalidita.Descrizione, gradoInvalidita.Id);

            AreaDecodifica.DatiProrataEnel[] listaProrataEnel = datiDecodifica.ElencoProrataEnel;
            ddlProrataEnel.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlProrataEnel);
            foreach (AreaDecodifica.DatiProrataEnel prorataEnel in listaProrataEnel)
                CodeUtility.SetValueDdl(ddlProrataEnel, prorataEnel.Descrizione, prorataEnel.Descrizione, prorataEnel.Id);

            List<AreaDecodifica.DatiCodiceAzienda> listaCodiceAzienda = datiDecodifica.ElencoCodiceAzienda.ToList().FindAll(x => x.Fondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL.ToString());
            ddlCodiceAziendaEL.Items.Clear();
            CodeUtility.SetValueDdl(ddlCodiceAziendaEL, string.Empty, string.Empty, string.Empty);
            foreach (AreaDecodifica.DatiCodiceAzienda codiceAzienda in listaCodiceAzienda)
                CodeUtility.SetValueDdl(ddlCodiceAziendaEL, codiceAzienda.Descrizione, codiceAzienda.Descrizione, codiceAzienda.Id.ToString());
        }

        private void VisualizzaPannelliEL(AreaTitolare.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            if (datiPensione.DecorrenzaOriginaria <= Utility.GetDateFromString("12/1983"))
                pnlCodDirittoQuoteFisse_EL.Visible = true;

            if (areaLiquidazionePensione != null && areaLiquidazionePensione.IsDomandaAnteArmonizzazione.GetValueOrDefault())
            {
                pnlInvalidita_MaggiorazioneAnte97.Visible = true;
                if (areaLiquidazionePensione.IsReversibilitaOrRicostituzione.GetValueOrDefault())
                    trDirittoQuoteFisse.Visible = true;
            }
        }

        #endregion Fondo EL

        #region Fondo TT

        private void ValorizzaEtichetteDatiAssicurativiTT(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.ConvenzioneInternazionale.HasValue && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.ConvenzioneInternazionale.Value.ToString() != "\0")
                    this.ddlCodiceConvenzione.Items.FindByValue(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.ConvenzioneInternazionale.Value.ToString()).Selected = true;
                else
                    this.ddlCodiceConvenzione.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiContributiFissiAnni.HasValue)
                    this.txtContributiFissiAnno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiContributiFissiAnni.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiContributiFissiMesi.HasValue)
                    this.txtContributiFissiMese.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiContributiFissiMesi.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiContributiFissiGiorni.HasValue)
                    this.txtContributiFissiGiorno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiContributiFissiGiorni.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiRiservaMatematicaAnni.HasValue)
                    this.txtRiscattiRiservaAnno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiRiservaMatematicaAnni.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiRiservaMatematicaMesi.HasValue)
                    this.txtRiscattiRiservaMese.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiRiservaMatematicaMesi.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiRiservaMatematicaGiorni.HasValue)
                    this.txtRiscattiRiservaGiorno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RiscattiRiservaMatematicaGiorni.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PeriodiFigurativiAnni.HasValue)
                    this.txtPeriodiFigurativiAnno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PeriodiFigurativiAnni.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PeriodiFigurativiMesi.HasValue)
                    this.txtPeriodiFigurativiMese.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PeriodiFigurativiMesi.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PeriodiFigurativiGiorni.HasValue)
                    this.txtPeriodiFigurativiGiorno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PeriodiFigurativiGiorni.Value.ToString();

                if (pnlDecorrenzaTeorica.Visible)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.DecorrenzaTeorica.HasValue)
                        this.txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.DecorrenzaTeorica.Value);

                if (pnlSupplementoOBG.Visible)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.SupplementoLegge58367.HasValue)
                        this.txtSupplementoOBG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.SupplementoLegge58367.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.Ditta.HasValue)
                    this.ddlDitta.Items.FindByValue(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.Ditta.Value.ToString()).Selected = true;
                else
                    this.ddlDitta.SelectedIndex = 0;

                //Visualizzo il pannello Inail solo per pensioni di reversibilità (gruppo 003 , prodotto 021), invalidità (gruppo 002, prodotto 011) e inabilita (gruppo 002, prodotto 012)
                if (pnlInailTT_ET.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RenditaInailAnnua.HasValue)
                        this.txtRenditaInail.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RenditaInailAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RetribuzioneMensileInail.HasValue)
                        this.txtRetribEffettivaInail.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.RetribuzioneMensileInail.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }

                if (pnlPensioneGenitori.Visible)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PensioneDirettaGenitori.HasValue)
                        this.txtPensioneGenitori.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.PensioneDirettaGenitori.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.DimissioniAnte97.HasValue)
                    this.chkDimissioniAnte97.Checked = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.DimissioniAnte97.Value;
                else
                    this.chkDimissioniAnte97.Checked = false;

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.CodiceArt5L58.HasValue)
                    this.chkArt5Legge58.Checked = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoTT.CodiceArt5L58.Value;
                else
                    this.chkArt5Legge58.Checked = false;
            }
        }

        private INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiTT(INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoTT = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi.FondoTT();

            int? inull = null;
            long? lNull = null;
            DateTime? dtNull = null;
            char? cNull = null;
            datiAssicurativi.fondoTT.CodiceArt5L58 = chkArt5Legge58.Checked == true ? chkArt5Legge58.Checked : false;

            datiAssicurativi.fondoTT.ConvenzioneInternazionale = ddlCodiceConvenzione.SelectedIndex != 0 ? Convert.ToChar(ddlCodiceConvenzione.SelectedValue) : cNull;

            datiAssicurativi.fondoTT.DecorrenzaTeorica = !string.IsNullOrEmpty(this.txtDecorrenzaTeorica.Text) ? Convert.ToDateTime(this.txtDecorrenzaTeorica.Text) : dtNull;
            datiAssicurativi.fondoTT.DimissioniAnte97 = chkDimissioniAnte97.Checked == true ? chkDimissioniAnte97.Checked : false;
            datiAssicurativi.fondoTT.Ditta = ddlDitta.SelectedIndex != 0 ? Convert.ToInt64(ddlDitta.SelectedValue) : lNull;
            datiAssicurativi.fondoTT.PensioneDirettaGenitori = !string.IsNullOrEmpty(txtPensioneGenitori.Text) ? Convert.ToDecimal(txtPensioneGenitori.Text) : 0M;
            datiAssicurativi.fondoTT.PeriodiFigurativiAnni = !string.IsNullOrEmpty(txtPeriodiFigurativiAnno.Text) ? Convert.ToInt32(txtPeriodiFigurativiAnno.Text) : inull;
            datiAssicurativi.fondoTT.PeriodiFigurativiMesi = !string.IsNullOrEmpty(txtPeriodiFigurativiMese.Text) ? Convert.ToInt32(txtPeriodiFigurativiMese.Text) : inull;
            datiAssicurativi.fondoTT.PeriodiFigurativiGiorni = !string.IsNullOrEmpty(txtPeriodiFigurativiGiorno.Text) ? Convert.ToInt32(txtPeriodiFigurativiGiorno.Text) : inull;
            datiAssicurativi.fondoTT.RenditaInailAnnua = !string.IsNullOrEmpty(txtRenditaInail.Text) ? Convert.ToDecimal(txtRenditaInail.Text) : 0M;
            datiAssicurativi.fondoTT.RetribuzioneMensileInail = !string.IsNullOrEmpty(txtRetribEffettivaInail.Text) ? Convert.ToDecimal(txtRetribEffettivaInail.Text) : 0M;
            datiAssicurativi.fondoTT.RiscattiContributiFissiAnni = !string.IsNullOrEmpty(txtContributiFissiAnno.Text) ? Convert.ToInt32(txtContributiFissiAnno.Text) : inull;
            datiAssicurativi.fondoTT.RiscattiContributiFissiMesi = !string.IsNullOrEmpty(txtContributiFissiMese.Text) ? Convert.ToInt32(txtContributiFissiMese.Text) : inull;
            datiAssicurativi.fondoTT.RiscattiContributiFissiGiorni = !string.IsNullOrEmpty(txtContributiFissiGiorno.Text) ? Convert.ToInt32(txtContributiFissiGiorno.Text) : inull;
            datiAssicurativi.fondoTT.RiscattiRiservaMatematicaAnni = !string.IsNullOrEmpty(txtRiscattiRiservaAnno.Text) ? Convert.ToInt32(txtRiscattiRiservaAnno.Text) : inull;
            datiAssicurativi.fondoTT.RiscattiRiservaMatematicaMesi = !string.IsNullOrEmpty(txtRiscattiRiservaMese.Text) ? Convert.ToInt32(txtRiscattiRiservaMese.Text) : inull;
            datiAssicurativi.fondoTT.RiscattiRiservaMatematicaGiorni = !string.IsNullOrEmpty(txtRiscattiRiservaGiorno.Text) ? Convert.ToInt32(txtRiscattiRiservaGiorno.Text) : inull;
            datiAssicurativi.fondoTT.SupplementoLegge58367 = !string.IsNullOrEmpty(txtSupplementoOBG.Text) ? Convert.ToDecimal(txtSupplementoOBG.Text) : 0M;

            return datiAssicurativi;
        }

        private void LoadDdlTT(AreaDecodifica datiDecodifica)
        {
            List<AreaDecodifica.DatiCodiceAzienda> listaCodiceAzienda = datiDecodifica.ElencoCodiceAzienda.ToList().FindAll(x => x.Fondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT.ToString());
            ddlDitta.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlDitta);
            foreach (AreaDecodifica.DatiCodiceAzienda codiceAzienda in listaCodiceAzienda)
                CodeUtility.SetValueDdl(ddlDitta, codiceAzienda.Descrizione, codiceAzienda.Descrizione, codiceAzienda.Id.ToString());
        }

        private void VisualizzaPannelliTT(AreaTitolare.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            DateTime dataVisualizDecTeorica = new DateTime(1968, 1, 1);
            DateTime dataVisualizSupplOBGPensGen = new DateTime(1997, 7, 1);
            DateTime? decorrenza = ViewState["DecorrenzaPensione"] as DateTime?;

            if (decorrenza.HasValue && decorrenza.Value.CompareTo(dataVisualizDecTeorica) < 0)
                pnlDecorrenzaTeorica.Visible = true;

            if (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value.CompareTo(dataVisualizSupplOBGPensGen) < 0)
                pnlSupplementoOBG.Visible = true;

            //Visualizzo il pannello Inail solo per pensioni di reversibilità (gruppo 003), invalidità (gruppo 002, prodotto 011) e inabilita (gruppo 002, prodotto 012)
            //if ((datiPensione.CodeGruppo == "0003") || (datiPensione.CodeGruppo == "0002" && (datiPensione.CodeProdotto == "0011" || datiPensione.CodeProdotto == "0012")))

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            //if (tipologiaPensione.HasValue && ((tipologiaPensione == CodeUtility.TipologiaPensione.gr_Reversibilita) || 
            //    (tipologiaPensione == CodeUtility.TipologiaPensione.gr_Inabilita_Invalidita && (tipologiaPensione == CodeUtility.TipologiaPensione.pr_InvaliditaAssegno || tipologiaPensione == CodeUtility.TipologiaPensione.pr_InabilitaPensione))))

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Superstiti ||
                    (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InvaliditaAssegno || tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_InabilitaPensione))
                pnlInailTT_ET.Visible = true;

            if (datiPensione.DecorrenzaOriginaria.HasValue && datiPensione.DecorrenzaOriginaria.Value.CompareTo(dataVisualizSupplOBGPensGen) < 0)
                pnlPensioneGenitori.Visible = true;

            validateTxtRetribEffettivaInailTT.Enabled = true;
            txtRetribEffettivaInail.MaxLength = 9;

            if (areaLiquidazionePensione != null && areaLiquidazionePensione.IsReversibilitaOrRicostituzione.GetValueOrDefault() && areaLiquidazionePensione.IsDomandaAnteArmonizzazione.GetValueOrDefault())
                trDirittoQuoteFisse.Visible = true;
        }

        #endregion Fondo TT

        #region Fondo ET

        private void ValorizzaEtichetteDatiAssicurativiET(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.PartTime.HasValue)
                    ddlPartTime.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.PartTime.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.DataEsonero.HasValue)
                    txtCessazioneIscrizione.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.DataEsonero.Value);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.AAInterruzione.HasValue)
                    txtInterrPTVAnno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.AAInterruzione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.MMInterruzione.HasValue)
                    txtInterrPTVMese.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.MMInterruzione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.GGInterruzione.HasValue)
                    txtInterrPTVGiorno.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.GGInterruzione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.NSettimaneLeva.HasValue)
                    txtNSettimaneLeva.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.NSettimaneLeva.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.NSettimaneRichiamato.HasValue)
                    txtNSettimanaRichiamato.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.NSettimaneRichiamato.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ContributiAgoLegge40245.HasValue)
                    txtContribAgoL402.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ContributiAgoLegge40245.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ContributiAgoLegge140830.HasValue)
                    txtContribAgoL140.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ContributiAgoLegge140830.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.CodAzienda.HasValue)
                    txtAziendaET.Text = GetAziendaET(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.CodAzienda.Value);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Stipendio.HasValue)
                    txtStipendio.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Stipendio.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Importo13ma.HasValue)
                    txtTredicesima.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Importo13ma.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Importo14ma.HasValue)
                    txtQuattordicesima.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Importo14ma.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ElementiAccessori.HasValue)
                    txtElementiAccessori.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ElementiAccessori.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Competenze40Percento.HasValue)
                    txtCompetenze.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.Competenze40Percento.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.CodiceServizioMilitare.HasValue)
                    ddlServizioMilitare.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.CodiceServizioMilitare.Value ? "True" : "False";

                if ((bool)liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.CodiceEsodo.HasValue)
                    ddlEsodo.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.CodiceEsodo.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.RetribuzioneEsodo.HasValue)
                    txtRetribuzioneEsodo.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.RetribuzioneEsodo.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (pnlInvaliditaTxt.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.GradoInvalidita.HasValue)
                        txtGradoInvalidita.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.GradoInvalidita.Value.ToString();
                }

                if (pnlInailTT_ET.Visible)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ImportoRenditaInail.HasValue)
                        txtRenditaInail.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.ImportoRenditaInail.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.RetribuzioneEffettiva.HasValue)
                        txtRetribEffettivaInail.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.RetribuzioneEffettiva.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                }

                if (pnlDecorrenzaTeorica.Visible)
                {
                    //obbligatorietà prevista al momento solo per ET
                    txtDecorrenzaTeorica_RF.Enabled = true;
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.DecorrenzaTeorica != null)
                        txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.DecorrenzaTeorica);
                }


                if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                {
                    // DPR Armonizzazione
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.PersonaleViaggiante.HasValue)
                        ddlPersonaleViaggiante.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoET.PersonaleViaggiante.Value.ToString();
                    else
                    {
                        List<PersonaleViaggiante> listaPersonaleViaggiante = (List<PersonaleViaggiante>)ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()];

                        if (listaPersonaleViaggiante != null)
                        {
                            PersonaleViaggiante personaleViaggiante = null;
                            if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
                            {
                                personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 1);
                                if (personaleViaggiante != null)
                                    ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                            }
                            else if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
                            {
                                personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 2);
                                if (personaleViaggiante != null)
                                    ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                            }
                        }
                    }
                    //--------------------------
                }
            }
            else
            {
                ddlPartTime.SelectedIndex = 0;
                txtCessazioneIscrizione.Text = string.Empty;
                txtInterrPTVAnno.Text = string.Empty;
                txtInterrPTVMese.Text = string.Empty;
                txtInterrPTVGiorno.Text = string.Empty;
                txtNSettimaneLeva.Text = string.Empty;
                txtNSettimanaRichiamato.Text = string.Empty;
                txtContribAgoL402.Text = string.Empty;
                txtContribAgoL140.Text = string.Empty;
                txtAziendaET.Text = string.Empty;
                txtStipendio.Text = string.Empty;
                txtTredicesima.Text = string.Empty;
                txtQuattordicesima.Text = string.Empty;
                txtElementiAccessori.Text = string.Empty;
                txtCompetenze.Text = string.Empty;
                ddlServizioMilitare.SelectedIndex = 0;
                ddlEsodo.SelectedIndex = 0;
                txtRetribuzioneEsodo.Text = string.Empty;
                txtGradoInvalidita.Text = string.Empty;
                txtRenditaInail.Text = string.Empty;
                txtRetribEffettivaInail.Text = string.Empty;
                txtDecorrenzaTeorica.Text = string.Empty;

                if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
                {
                    // DPR armonizzazione
                    List<PersonaleViaggiante> listaPersonaleViaggiante = (List<PersonaleViaggiante>)ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()];
                    if (listaPersonaleViaggiante != null)
                    {
                        PersonaleViaggiante personaleViaggiante = null;
                        if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
                        {
                            personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 1);
                            if (personaleViaggiante != null)
                                ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                        }
                        else if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
                        {
                            personaleViaggiante = listaPersonaleViaggiante.Find(x => x.TraduzioneSuGP == 2);
                            if (personaleViaggiante != null)
                                ddlPersonaleViaggiante.SelectedValue = personaleViaggiante.Id.ToString();
                        }
                    }
                    //-------------------------
                }
            }

            //prevalorizzazione Decorrenza Teorica
            if (pnlDecorrenzaTeorica.Visible && ViewState["DecorrenzaPensione"] != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivo.GetValueOrDefault()
                    || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione
                    || datiPensione.IsDomandaVecchiaiaAOICalcoloContributivo
                    || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) 
                    || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))) //ENG - MEMO 166/2023
                {
                    txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", (DateTime)ViewState["DecorrenzaPensione"]);
                    txtDecorrenzaTeorica.Enabled = false;
                }
                else if (string.IsNullOrEmpty(txtDecorrenzaTeorica.Text))
                {
                    txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", (DateTime)ViewState["DecorrenzaPensione"]);
                }
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiET(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            int? inull = null;
            DateTime? dtNull = null;
            bool? bNull = null;
            short? sNull = null;
            decimal? dNull = null;

            datiAssicurativi.fondoET = new Presenter.SvrLiquidazioneFs.DatiAssicurativi.FondoET();

            if (ddlPartTime.SelectedIndex != 0)
                datiAssicurativi.fondoET.PartTime = ddlPartTime.SelectedIndex != 0 ? Convert.ToBoolean(ddlPartTime.SelectedValue) : bNull;

            datiAssicurativi.fondoET.DataEsonero = (!string.IsNullOrEmpty(txtCessazioneIscrizione.Text) && txtCessazioneIscrizione.Text.ToUpperInvariant() != "GG/MM/AAAA") ? Convert.ToDateTime(txtCessazioneIscrizione.Text) : dtNull;
            datiAssicurativi.fondoET.AAInterruzione = !string.IsNullOrEmpty(txtInterrPTVAnno.Text) ? Convert.ToInt32(txtInterrPTVAnno.Text) : inull;
            datiAssicurativi.fondoET.MMInterruzione = !string.IsNullOrEmpty(txtInterrPTVMese.Text) ? Convert.ToInt32(txtInterrPTVMese.Text) : inull;
            datiAssicurativi.fondoET.GGInterruzione = !string.IsNullOrEmpty(txtInterrPTVGiorno.Text) ? Convert.ToInt32(txtInterrPTVGiorno.Text) : inull;
            datiAssicurativi.fondoET.NSettimaneLeva = !string.IsNullOrEmpty(txtNSettimaneLeva.Text) ? Convert.ToInt16(txtNSettimaneLeva.Text) : sNull;
            datiAssicurativi.fondoET.NSettimaneRichiamato = !string.IsNullOrEmpty(txtNSettimanaRichiamato.Text) ? Convert.ToInt16(txtNSettimanaRichiamato.Text) : sNull;
            datiAssicurativi.fondoET.ContributiAgoLegge40245 = !string.IsNullOrEmpty(txtContribAgoL402.Text) ? Convert.ToDecimal(txtContribAgoL402.Text) : dNull;
            datiAssicurativi.fondoET.ContributiAgoLegge140830 = !string.IsNullOrEmpty(txtContribAgoL140.Text) ? Convert.ToDecimal(txtContribAgoL140.Text) : dNull;

            if (!String.IsNullOrEmpty(txtAziendaET.Text))
                datiAssicurativi.fondoET.CodAzienda = ControlAziendaET(txtAziendaET.Text);

            datiAssicurativi.fondoET.Stipendio = !string.IsNullOrEmpty(txtStipendio.Text) ? Convert.ToDecimal(txtStipendio.Text) : dNull;
            datiAssicurativi.fondoET.Importo13ma = !string.IsNullOrEmpty(txtTredicesima.Text) ? Convert.ToDecimal(txtTredicesima.Text) : dNull;
            datiAssicurativi.fondoET.Importo14ma = !string.IsNullOrEmpty(txtQuattordicesima.Text) ? Convert.ToDecimal(txtQuattordicesima.Text) : dNull;
            datiAssicurativi.fondoET.ElementiAccessori = !string.IsNullOrEmpty(txtElementiAccessori.Text) ? Convert.ToDecimal(txtElementiAccessori.Text) : dNull;
            datiAssicurativi.fondoET.Competenze40Percento = !string.IsNullOrEmpty(txtCompetenze.Text) ? Convert.ToDecimal(txtCompetenze.Text) : dNull;
            datiAssicurativi.fondoET.CodiceServizioMilitare = ddlServizioMilitare.SelectedIndex != 0 ? Convert.ToBoolean(ddlServizioMilitare.SelectedValue) : bNull;
            datiAssicurativi.fondoET.CodiceEsodo = ddlEsodo.SelectedIndex != 0 ? Convert.ToBoolean(ddlEsodo.SelectedValue) : bNull;
            datiAssicurativi.fondoET.RetribuzioneEsodo = !string.IsNullOrEmpty(txtRetribuzioneEsodo.Text) ? Convert.ToDecimal(txtRetribuzioneEsodo.Text) : dNull;
            datiAssicurativi.fondoET.GradoInvalidita = CodeUtility.StringToNullableByte(txtGradoInvalidita.Text);
            datiAssicurativi.fondoET.ImportoRenditaInail = !string.IsNullOrEmpty(txtRenditaInail.Text) ? Convert.ToDecimal(txtRenditaInail.Text) : dNull;
            datiAssicurativi.fondoET.RetribuzioneEffettiva = !string.IsNullOrEmpty(txtRetribEffettivaInail.Text) ? Convert.ToDecimal(txtRetribEffettivaInail.Text) : dNull;
            if (!(String.Equals(txtDecorrenzaTeorica.Text, "mm/aaaa")) && (!String.IsNullOrEmpty(txtDecorrenzaTeorica.Text)))
                datiAssicurativi.fondoET.DecorrenzaTeorica = Utility.GetDateFromString(txtDecorrenzaTeorica.Text);

            if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
            {
                // DPR Armonizzazione
                long resLong = 0;
                long.TryParse(ddlPersonaleViaggiante.SelectedValue, out resLong);
                datiAssicurativi.fondoET.PersonaleViaggiante = resLong != 0 ? resLong : (long?)null;
                //------------------------
            }

            return datiAssicurativi;
        }

        private void VisualizzaPannelliET(AreaTitolare.DatiPensione datiPensione, AreaLiquidazionePensione areaLiquidazionePensione)
        {
            DateTime? decorrenzaPensione = (areaLiquidazionePensione != null && areaLiquidazionePensione.DecorrenzaPensioneDirettaDC.HasValue) ? areaLiquidazionePensione.DecorrenzaPensioneDirettaDC : datiPensione.DecorrenzaOriginaria.Value;

            if (decorrenzaPensione.HasValue && decorrenzaPensione.Value.CompareTo(new DateTime(1996, 09, 01)) < 0)
                pnlET_ServizioMilitare.Visible = true;

            //Visualizzo il pannello Inail solo per pensioni di reversibilità (gruppo 003), invalidità (gruppo 002, prodotto 011) e inabilita (gruppo 002, prodotto 012)
            //if ((datiPensione.CodeGruppo == "0003") || (datiPensione.CodeGruppo == "0002" && (datiPensione.CodeProdotto == "0011" || datiPensione.CodeProdotto == "0012")))
            pnlInailTT_ET.Visible = true;

            pnlInvaliditaTxt.Visible = true;

            //DecorrenzaTeorica sempre visibile
            pnlDecorrenzaTeorica.Visible = true;

            validateTxtRetribEffettivaInailET.Enabled = true;

            if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
            {
                if (((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante.ToString()]).GetValueOrDefault() ||
                    ((bool?)ViewState[EnumViewState.IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante.ToString()]).GetValueOrDefault())
                    pnlPersonaleViaggiante.Visible = true;
            }

            if (areaLiquidazionePensione != null && areaLiquidazionePensione.IsReversibilitaOrRicostituzione.GetValueOrDefault() && areaLiquidazionePensione.IsDomandaAnteArmonizzazione.GetValueOrDefault())
                trDirittoQuoteFisse.Visible = true;
        }

        private void LoadDdlET(ILiquidazionePensione liquidazione, AreaDecodifica datiDecodifica)
        {
            List<CodiceEsodo> listaCodiceEsodo = liquidazione.areaLiquidazionePensioneFS.ListaCodiceEsodo.ToList();

            ddlEsodo.Items.Clear();
            CodeUtility.SetValueDdl(ddlEsodo, string.Empty, string.Empty, string.Empty);
            foreach (CodiceEsodo codiceEsodo in listaCodiceEsodo)
                CodeUtility.SetValueDdl(ddlEsodo, codiceEsodo.Descrizione, codiceEsodo.Descrizione, codiceEsodo.Codice.ToString());

            List<CodicePartTime> listaCodicePartTime = liquidazione.areaLiquidazionePensioneFS.ListaCodicePartTime.ToList();
            ddlPartTime.Items.Clear();
            CodeUtility.SetValueDdl(ddlPartTime, string.Empty, string.Empty, string.Empty);
            foreach (CodicePartTime codicePartTime in listaCodicePartTime)
                CodeUtility.SetValueDdl(ddlPartTime, codicePartTime.Descrizione, codicePartTime.Descrizione, codicePartTime.Codice.ToString());

            string elencoAziende = string.Empty;
            List<AreaDecodifica.DatiCodiceAzienda> listaAzienda = datiDecodifica.ElencoCodiceAzienda.ToList().FindAll(x => x.Fondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET.ToString());
            foreach (AreaDecodifica.DatiCodiceAzienda codeAzienda in listaAzienda)
            {
                elencoAziende = string.Concat(codeAzienda.TraduzioneGp + " - " + codeAzienda.Descrizione, ";");
                HiddenFieldAziende.Value = string.Concat(HiddenFieldAziende.Value, elencoAziende);
            }

            if (ConfigurationManager.AppSettings["DPRArmonizzazione"] != null && ConfigurationManager.AppSettings["DPRArmonizzazione"] == "SI")
            {
                // DPR Armonizzazione
                ddlPersonaleViaggiante.Items.Clear();
                CodeUtility.SetValueDdl(ddlPersonaleViaggiante, string.Empty, string.Empty, string.Empty);
                foreach (PersonaleViaggiante personaleViaggiante in (List<PersonaleViaggiante>)ViewState[EnumViewState.ListaPersonaleViaggiante.ToString()])
                    CodeUtility.SetValueDdl(ddlPersonaleViaggiante, personaleViaggiante.TraduzioneSuGP + " - " + personaleViaggiante.Descrizione, personaleViaggiante.Descrizione, personaleViaggiante.Id.ToString());
                //--------------------
            }
        }

        private long? ControlAziendaET(string aziendaInserita)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            List<AreaDecodifica.DatiCodiceAzienda> listaAzienda = datiDecodifica.ElencoCodiceAzienda.ToList().FindAll(x => x.Fondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET.ToString());
            long? codAzienda = null;

            foreach (AreaDecodifica.DatiCodiceAzienda codeAzienda in listaAzienda)
            {
                if (aziendaInserita.Length >= 6 && codeAzienda.TraduzioneGp.ToUpperInvariant() == aziendaInserita.ToUpperInvariant().Trim().Substring(0, 6))
                    codAzienda = codeAzienda.Id;
            }

            return codAzienda;
        }

        private string GetAziendaET(long? codAzienda)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            List<AreaDecodifica.DatiCodiceAzienda> listaAzienda = datiDecodifica.ElencoCodiceAzienda.ToList().FindAll(x => x.Fondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET.ToString());

            if (codAzienda.HasValue)
            {
                AreaDecodifica.DatiCodiceAzienda azienda = listaAzienda.Find(delegate(AreaDecodifica.DatiCodiceAzienda code)
                { return (code.Id == codAzienda.Value); });
                return azienda.TraduzioneGp + " - " + azienda.Descrizione;
            }
            else return string.Empty;
        }

        #endregion Fondo ET

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

        #endregion EventHandler

        #region enum

        public enum EnumViewState
        {
            ListaPersonaleViaggiante,
            IsDomandaPersonaleViaggianteSenzaPerditaTitoloAbilitante,
            IsDomandaPersonaleViaggianteConPerditaTitoloAbilitante
        }

        #endregion enum
    }
}
