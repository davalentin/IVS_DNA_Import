using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiAssicurativiDZ_ES_PM : CustomBaseUserControl, ITitolarePensione, IRecordFondo, ILiquidazionePensione
    {
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

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        #endregion EventHandler

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

            if (liquidazione.areaLiquidazionePensioneFS != null)
                ViewState["areaLiquidazionePensioneFS"] = liquidazione.areaLiquidazionePensioneFS;

            ManageDecorrenzaForReversibilita(datiPensione, liquidazione.areaLiquidazionePensioneFS.DecorrenzaPensioneDirettaDC);
            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
                ViewState["IsUsuranti"] = "SI";

            if (IsDomandaSperDonna)
                ViewState["IsDomandaSperDonna"] = "SI";

            if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceLegge413 != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceLegge413.Count() > 0)
                ViewState["ListaCodiceLegge413"] = liquidazione.areaLiquidazionePensioneFS.ListaCodiceLegge413.ToList();

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.HasValue)
                ViewState["IsCodiceNatura2DisabledPerSperDonna"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2DisabledPerSperDonna.Value;
            
            if (liquidazione.areaLiquidazionePensioneFS.IsPensioneTipoContributivoConOpzione.GetValueOrDefault() || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((!String.IsNullOrEmpty(controlloDinamicoMemo123_2024) && controlloDinamicoMemo123_2024.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024OpzioneContrib) && controlloDinamicoMemo123_2024OpzioneContrib.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))))
                ViewState["IsPensioneTipoContributivoConOpzione"] = "SI";

            LoadDdl(this.domanda.Tipofondo, liquidazione);
            RenderControlsFromTipoFondo(this.domanda.Tipofondo, liquidazione);
            ValorizzaEtichetteDatiAssicurativiCommon(liquidazione, datiPensione);
            switch (domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    ValorizzaEtichetteDatiAssicurativiDZ(liquidazione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    ValorizzaEtichetteDatiAssicurativiES(liquidazione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    ValorizzaEtichetteDatiAssicurativiPM(liquidazione);
                    break;
            }

            if (datiPensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura)
                GestioneEtichetteRic(datiPensione);

            if (isDomandaInabilitaAmianto)
            {
                pnlAttEconomProfInd.Visible = true;
                txtAttivitaEconomica.Text = "01";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "250";
                txtProfessioneIndividuale.Enabled = false;
            }
        }

        internal Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, out List<RecordFondo> listaRecordFondo)
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = null;
            areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiCommon(out listaRecordFondo);
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiDZ(areaLiquidazionePensioneFS);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiES(areaLiquidazionePensioneFS);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiPM(areaLiquidazionePensioneFS);
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

            if(this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            if (TitolarePensione.Pensione == null)
                TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            try
            {
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //per il fondo PM ci sta una gestione particolare dei codici natura
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM)
                    {
                        ManageDgvRowForPM(e.Row, datiPensione);
                    }

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
                                    if (codiceNatura.Posizione == 1)
                                        CodeUtility.SetValueDdl(ddlCodNatura1, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    else if (codiceNatura.Posizione == 2)
                                        CodeUtility.SetValueDdl(ddlCodNatura2, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                                    else
                                        CodeUtility.SetValueDdl(ddlCodNatura3, codiceNatura.TraduzioneSuGP.ToString(), codiceNatura.Descrizione, codiceNatura.TraduzioneSuGP.ToString());
                            }
                            
                            //Blindatura del codice natura per categoria VPM
                            if (areaLiquidazionePensioneFS == null)
                                areaLiquidazionePensioneFS = (AreaLiquidazionePensione)ViewState["areaLiquidazionePensioneFS"];

                            if (areaLiquidazionePensioneFS.IsDecorrenzaSuccSett1989.HasValue && areaLiquidazionePensioneFS.IsDecorrenzaSuccSett1989.Value &&
                                !String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim() == "VPM")
                            {
                                ddlCodNatura1.SelectedValue = "1";
                                ddlCodNatura1.Enabled = false;
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

                            CodeUtility.DisableCodNatura2PerSperDonna(ddlCodNatura2, (bool)ViewState["IsCodiceNatura2DisabledPerSperDonna"]);

                            if (CodeUtility.IsTipoContributivoConOpzione(datiPensione, areaLiquidazionePensioneFS.IsPensioneTipoContributivoConOpzione) 
                                || TitolarePensione.Pensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione
                                || (!CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                                || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && TitolarePensione.Pensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))
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

                            //Blindatura del codice natura per categoria VPM
                            if (areaLiquidazionePensioneFS == null)
                                areaLiquidazionePensioneFS = (AreaLiquidazionePensione)ViewState["areaLiquidazionePensioneFS"];

                            if (areaLiquidazionePensioneFS.IsDecorrenzaSuccSett1989.HasValue && areaLiquidazionePensioneFS.IsDecorrenzaSuccSett1989.Value &&
                                !String.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.Trim() == "VPM")
                            {
                                ddlCodNatura1.SelectedValue = "1";
                                ddlCodNatura1.Enabled = false;
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
                                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ)
                                    e.Row.Cells[0].Controls[0].Visible = false;
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
                                {

                                    string idButton = "btnDelete";
                                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ)
                                        idButton = string.Empty;
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, idButton);
                                }
                                    
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

        private void ManageDgvRowForPM(GridViewRow gridViewRow,AreaTitolare.DatiPensione datiPensione)
        {
            if(this.domanda==null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            
            AreaTitolare.DatiPensione.TipoDomanda tipoDomanda = datiPensione.Tipo;
            
            //Per le riapeture si avrà un comportamento uguale alle ricostituzioni
            if (this.domanda.IsDomandaRiapertura)
                tipoDomanda = AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione;

            switch (tipoDomanda)
            {
                case AreaTitolare.DatiPensione.TipoDomanda.Normale:
                case AreaTitolare.DatiPensione.TipoDomanda.Ripristino:
                    if (gridViewRow.Cells[1].FindControl("lblCodiceNatura3") != null)
                        gridViewRow.Cells[1].FindControl("lblCodiceNatura3").Visible = false;
                    if (gridViewRow.Cells[1].FindControl("ddlCodNatura3") != null)
                        gridViewRow.Cells[1].FindControl("ddlCodNatura3").Visible = false;
                    if (gridViewRow.Cells[1].FindControl("lblCodiceNatura2") != null)
                        gridViewRow.Cells[1].FindControl("lblCodiceNatura2").Visible = false;
                    if (gridViewRow.Cells[1].FindControl("ddlCodNatura2") != null)
                        gridViewRow.Cells[1].FindControl("ddlCodNatura2").Visible = false;
                    break;
                case AreaTitolare.DatiPensione.TipoDomanda.Superstiti:
                case AreaTitolare.DatiPensione.TipoDomanda.RipristinoSuperstiti:
                    if (gridViewRow.Cells[1].FindControl("lblCodiceNatura3") != null)
                        gridViewRow.Cells[1].FindControl("lblCodiceNatura3").Visible = false;
                    if (gridViewRow.Cells[1].FindControl("ddlCodNatura3") != null)
                        gridViewRow.Cells[1].FindControl("ddlCodNatura3").Visible = false;
                    break;
                case AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione:
                    if (gridViewRow.Cells[1].FindControl("lblCodiceNatura3") != null)
                        ((TextBox)gridViewRow.Cells[1].FindControl("lblCodiceNatura3")).Enabled = false;
                    if (gridViewRow.Cells[1].FindControl("ddlCodNatura3") != null)
                        ((DropDownList)gridViewRow.Cells[1].FindControl("ddlCodNatura3")).Enabled = false;
                    if (gridViewRow.Cells[1].FindControl("lblCodiceNatura2") != null)
                        ((TextBox)gridViewRow.Cells[1].FindControl("lblCodiceNatura2")).Enabled = false;
                    if (gridViewRow.Cells[1].FindControl("ddlCodNatura2") != null)
                        ((DropDownList)gridViewRow.Cells[1].FindControl("ddlCodNatura2")).Enabled = false;
                    if (gridViewRow.Cells[1].FindControl("lblcodiceNatura1") != null)
                        ((TextBox)gridViewRow.Cells[1].FindControl("lblcodiceNatura1")).Enabled = false;
                    if (gridViewRow.Cells[1].FindControl("ddlCodNatura1") != null)
                        ((DropDownList)gridViewRow.Cells[1].FindControl("ddlCodNatura1")).Enabled = false;
                    break;
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

        #region private methods

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, -1);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            modalitaEdit.Value = "false";
            //txtCodiceRequisiti2.Text = "0";
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

        private void LoadDdl(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    LoadDdlES(liquidazione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    LoadDdlPM(liquidazione);
                    break;
            }

            List<CodiceSpecifico> listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().FindAll(delegate(CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                  code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant());
            });

            ViewState["CodiciNatura"] = liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura;

            ddlCodiceSpecifico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceSpecifico);
            foreach (CodiceSpecifico codSpec in listaCodiceSpecifico)
                CodeUtility.SetValueDdl(ddlCodiceSpecifico, codSpec.TraduzioneGp + " - " + codSpec.Descrizione, codSpec.Descrizione, codSpec.Id.Value.ToString());

            ddlCodRequisiti1.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodRequisiti1);
            foreach (CodiceRequisito1 codReq1 in liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito1)
                CodeUtility.SetValueDdl(ddlCodRequisiti1, codReq1.Id + " - " + codReq1.Descrizione, codReq1.Descrizione, codReq1.Id);

            ddlCodRequisiti2.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodRequisiti2);
            foreach (CodiceRequisito2 codReq2 in liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito2)
                CodeUtility.SetValueDdl(ddlCodRequisiti2, codReq2.Id.ToString() + " - " + codReq2.Descrizione, codReq2.Descrizione, codReq2.Id.ToString());
        }

        private void RenderControlsFromTipoFondo(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    pnlDZ.Visible = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    pnlES.Visible = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    pnlPM.Visible = true;
                    if (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.StartsWith("I"))
                        pnlAnnoUtileUltimoDecennio.Visible = true;
                    break;
            }
        }

        private void ValorizzaEtichetteDatiAssicurativiCommon(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.TipoPensione != null)
            {
                lblTipoPensione.Text = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Key;
                hdnTipoPensione.Value = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString();
            }

            lblDecorrenzaPensioneDatiAssicurativi.Text = datiPensione.DecorrenzaOriginaria != null ? datiPensione.DecorrenzaOriginaria.ToString().Substring(3, 7) : string.Empty;

            txtPrimoVersamento.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione != null ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione) : string.Empty;
            txtUltimoVersamento.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione != null ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione) : string.Empty;

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico != null)
                ddlCodiceSpecifico.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.ToString();
            else
                ddlCodiceSpecifico.SelectedIndex = 0;

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 != null)
                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                    ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2 != null)
                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.ToString()))
                    ddlCodRequisiti2.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.ToString();

            //if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2 != null)
            //    txtCodiceRequisiti2.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.ToString();

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

        private void salvaCommand(GridViewRow row, List<RecordFondo> listaRecordFondo, RecordFondo[] elencoRecordFondo)
        {
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

        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiCommon(out List<RecordFondo> listaRecordFondo)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = (List<RecordFondo>)ViewState["elencoRecordFondo"];
            CodeUtility.EliminaRecordVuoti(listaRecordFondo);
            areaLiquidazionePensioneFS.ListaRecordFondo = listaRecordFondo.ToArray();

            if (!(String.Equals(txtPrimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtPrimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtPrimoVersamento.Text);

            if (!(String.Equals(txtUltimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtUltimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtUltimoVersamento.Text);

            if (!(String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue)))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico = !String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue) ? Convert.ToByte(ddlCodiceSpecifico.SelectedValue) : (byte?)null;

            if (!string.IsNullOrEmpty(ddlCodRequisiti1.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 = char.Parse(ddlCodRequisiti1.SelectedValue);

            if (!string.IsNullOrEmpty(ddlCodRequisiti2.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2 = char.Parse(ddlCodRequisiti2.SelectedValue);

            if (!String.IsNullOrEmpty(txtAttivitaEconomica.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica = CodeUtility.StringToNullableShort(txtAttivitaEconomica.Text);

            if (!String.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale = CodeUtility.StringToNullableShort(txtProfessioneIndividuale.Text);

            return areaLiquidazionePensioneFS.DatiAssicurativi;
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

        private void ManageSperDonna(DropDownList ddlCodNatura2)
        {
            ddlCodNatura2.SelectedValue = "O";
            ddlCodNatura2.Enabled = false;
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    ddlCodRequisiti1.Enabled = false;
                    break;
            }

            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                //COMMON
                gvRecordFondo.Enabled = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                txtPrimoVersamento.Enabled = false;
                txtUltimoVersamento.Enabled = false;
                ddlCodiceSpecifico.Enabled = false;
                ddlCodiceSpecifico_RF.Enabled = false;
                ddlCodRequisiti1.Enabled = false;
                ddlCodRequisiti2.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ)
                {
                    txtDataCessazioneServizio.Enabled = false;
                    txtRiscattiAA_DZ.Enabled = false;
                    txtRiscattiMM_DZ.Enabled = false;
                    txtMaggiorazionePensionePrivilegiata_AA.Enabled = false;
                    txtMaggiorazionePensionePrivilegiata_MM.Enabled = false;
                    ddlCodiceBenefici.Enabled = false;
                    ddlCodDirittoQuoteFisse_DZ.Enabled = false;
                    ddlCodiceDz.Enabled = false;
                    txtClasseAnte50.Enabled = false;
                    txtDitta.Enabled = false;
                    ddlRaggiuntoRequisiti311297.Enabled = false;
                    ddlCodiceEsodo.Enabled = false;
                    txtMaggiorazioneAnzianitaEsodo_AA.Enabled = false;
                    txtMaggiorazioneAnzianitaEsodo_MM.Enabled = false;
                    txtRetribuzioneAlNettoBeneficiEsodo.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                {
                    ddlAttivitaSvolta.Enabled = false;
                    ddlAttivitaSvolta_RF.Enabled = false;
                    txtConvenzioneInternazionale.Enabled = false;
                    ddlCodDirittoQuoteFisse_ES.Enabled = false;
                    txtRiscattiAA_ES.Enabled = false;
                    txtRiscattiMM_ES.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM)
                {
                    ddlAttivitaSvolta_1PM.Enabled = false;
                    ddlAttivitaSvolta_2PM.Enabled = false;
                    ddlTipoLiquidazionePM.Enabled = false;
                    ddlAnnoUtileUltimoDecennio.Enabled = false;
                }
            }
        }

        #endregion private methods

        #region Fondo DZ

        private void ValorizzaEtichetteDatiAssicurativiDZ(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
            {
                List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                ViewState["elencoRecordFondo"] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                BindData();
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.DataCessazioneServizio.HasValue)
                    txtDataCessazioneServizio.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.DataCessazioneServizio.Value);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RiscattiAA.HasValue)
                    txtRiscattiAA_DZ.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RiscattiAA.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RiscattiMM.HasValue)
                    txtRiscattiMM_DZ.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RiscattiMM.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataAA.HasValue)
                    txtMaggiorazionePensionePrivilegiata_AA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataAA.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataMM.HasValue)
                    txtMaggiorazionePensionePrivilegiata_MM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataMM.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceBenefici.HasValue)
                    ddlCodiceBenefici.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceBenefici.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.HasValue)
                    ddlCodDirittoQuoteFisse_DZ.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceCaroPane.HasValue)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceCaroPane.Value)
                        ddlCaroPane.SelectedValue = "1";
                    else
                        ddlCaroPane.SelectedValue = "2";
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceDZ.HasValue)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceDZ.Value)
                        ddlCodiceDz.SelectedValue = "SI";
                    else
                        ddlCodiceDz.SelectedValue = "NO";
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceEsodo.HasValue)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceEsodo.Value)
                        ddlCodiceEsodo.SelectedValue = "1";
                    else
                        ddlCodiceEsodo.SelectedValue = "0";
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.ClasseAnte50.HasValue)
                    txtClasseAnte50.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.ClasseAnte50.ToString();

                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.Ditta))
                    txtDitta.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.Ditta;

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazioneAnzianitaEsodoAA.HasValue)
                    txtMaggiorazioneAnzianitaEsodo_AA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazioneAnzianitaEsodoAA.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazioneAnzianitaEsodoMM.HasValue)
                    txtMaggiorazioneAnzianitaEsodo_MM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazioneAnzianitaEsodoMM.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RetribuzioneAlNettoBeneficiEsodo.HasValue)
                    txtRetribuzioneAlNettoBeneficiEsodo.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RetribuzioneAlNettoBeneficiEsodo.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.PercentualeLiquidazionePensione.HasValue)
                    lblPercentualeLiquidazionePensione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.PercentualeLiquidazionePensione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RaggiuntoRequisiti311297.HasValue)
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RaggiuntoRequisiti311297.Value)
                        ddlRaggiuntoRequisiti311297.SelectedValue = "SI";
                    else
                        ddlRaggiuntoRequisiti311297.SelectedValue = "NO";
                }
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiDZ(AreaLiquidazionePensione areaLiquidazionePensioneFS)
        {
            areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ = new DatiAssicurativi.FondoDZ();

            if (!(String.Equals(txtDataCessazioneServizio.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtDataCessazioneServizio.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.DataCessazioneServizio = Utility.GetDateFromString(txtDataCessazioneServizio.Text);

            if (!string.IsNullOrEmpty(txtRiscattiAA_DZ.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RiscattiAA = short.Parse(txtRiscattiAA_DZ.Text);

            if (!string.IsNullOrEmpty(txtRiscattiMM_DZ.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RiscattiMM = short.Parse(txtRiscattiMM_DZ.Text);

            if (!string.IsNullOrEmpty(txtMaggiorazionePensionePrivilegiata_AA.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataAA = short.Parse(txtMaggiorazionePensionePrivilegiata_AA.Text);

            if (!string.IsNullOrEmpty(txtMaggiorazionePensionePrivilegiata_MM.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazionePensionePrivilegiataMM = short.Parse(txtMaggiorazionePensionePrivilegiata_MM.Text);

            if (!string.IsNullOrEmpty(ddlCodiceBenefici.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceBenefici = short.Parse(ddlCodiceBenefici.SelectedValue);

            if (!string.IsNullOrEmpty(ddlCodDirittoQuoteFisse_DZ.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse = byte.Parse(ddlCodDirittoQuoteFisse_DZ.SelectedValue);

            if (!string.IsNullOrEmpty(ddlCaroPane.SelectedValue))
            {
                if (ddlCaroPane.SelectedValue.Equals("1"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceCaroPane = true;
                else if (ddlCaroPane.SelectedValue.Equals("2"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceCaroPane = false;
            }

            if (!string.IsNullOrEmpty(ddlCodiceDz.SelectedValue))
            {
                if (ddlCodiceDz.SelectedValue.Equals("SI"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceDZ = true;
                else if (ddlCodiceDz.SelectedValue.Equals("NO"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceDZ = false;
            }

            if (!string.IsNullOrEmpty(txtClasseAnte50.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.ClasseAnte50 = short.Parse(txtClasseAnte50.Text);

            if (!string.IsNullOrEmpty(txtDitta.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.Ditta = txtDitta.Text;

            if (!string.IsNullOrEmpty(ddlCodiceEsodo.SelectedValue))
            {
                if(ddlCodiceEsodo.SelectedValue.Equals("1"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceEsodo = true;
                else if(ddlCodiceEsodo.SelectedValue.Equals("0"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.CodiceEsodo = false;
            }

            if (!string.IsNullOrEmpty(txtMaggiorazioneAnzianitaEsodo_AA.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazioneAnzianitaEsodoAA = short.Parse(txtMaggiorazioneAnzianitaEsodo_AA.Text);

            if (!string.IsNullOrEmpty(txtMaggiorazioneAnzianitaEsodo_MM.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.MaggiorazioneAnzianitaEsodoMM = short.Parse(txtMaggiorazioneAnzianitaEsodo_MM.Text);

            if (!string.IsNullOrEmpty(txtRetribuzioneAlNettoBeneficiEsodo.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RetribuzioneAlNettoBeneficiEsodo = decimal.Parse(txtRetribuzioneAlNettoBeneficiEsodo.Text);

            areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.PercentualeLiquidazionePensione = int.Parse(lblPercentualeLiquidazionePensione.Text);

            if (!string.IsNullOrEmpty(ddlRaggiuntoRequisiti311297.SelectedValue))
            {
                if (ddlRaggiuntoRequisiti311297.SelectedValue.Equals("SI"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RaggiuntoRequisiti311297 = true;
                else if (ddlRaggiuntoRequisiti311297.SelectedValue.Equals("NO"))
                    areaLiquidazionePensioneFS.DatiAssicurativi.fondoDZ.RaggiuntoRequisiti311297 = false;
            }

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        #endregion Fondo DZ

        #region Fondo ES
        private void ValorizzaEtichetteDatiAssicurativiES(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
            {
                List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                ViewState["elencoRecordFondo"] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                BindData();
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.ConvenzioneInternazionale.HasValue)
                    txtConvenzioneInternazionale.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.ConvenzioneInternazionale.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.AnniRiscatti.HasValue)
                    txtRiscattiAA_ES.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.AnniRiscatti.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.MesiRiscatti.HasValue)
                    txtRiscattiMM_ES.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.MesiRiscatti.Value.ToString();
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.HasValue)
                ddlCodDirittoQuoteFisse_ES.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.Value.ToString();
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiES(AreaLiquidazionePensione areaLiquidazionePensioneFS)
        {
            areaLiquidazionePensioneFS.DatiAssicurativi.fondoES = new DatiAssicurativi.FondoES();

            if (!string.IsNullOrEmpty(ddlAttivitaSvolta.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta.SelectedValue;

            if (!string.IsNullOrEmpty(txtConvenzioneInternazionale.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.ConvenzioneInternazionale = char.Parse(txtConvenzioneInternazionale.Text);

            if (!string.IsNullOrEmpty(ddlCodDirittoQuoteFisse_ES.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse = byte.Parse(ddlCodDirittoQuoteFisse_ES.SelectedValue);

            if (!string.IsNullOrEmpty(txtRiscattiAA_ES.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.AnniRiscatti = int.Parse(txtRiscattiAA_ES.Text);

            if (!string.IsNullOrEmpty(txtRiscattiMM_ES.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoES.MesiRiscatti = int.Parse(txtRiscattiMM_ES.Text);

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        private void LoadDdlES(ILiquidazionePensione liquidazione)
        {
            ddlAttivitaSvolta.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttivitaSvolta);
            foreach (DatiAttivitaSvolta attSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte)
                CodeUtility.SetValueDdl(ddlAttivitaSvolta, attSvolta.Descrizione, attSvolta.Descrizione, attSvolta.Id);
        }
        #endregion Fondo ES

        #region Fondo PM
        private void ValorizzaEtichetteDatiAssicurativiPM(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
            {
                List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                ViewState["elencoRecordFondo"] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                BindData();
            }

            if (string.IsNullOrEmpty(lblCL413.Text))
            {
                // Il campo viene prevalorizzato con il codice 'M' fino a nuova analisi
                CodiceLegge413 app = liquidazione.areaLiquidazionePensioneFS.ListaCodiceLegge413.Single(x => x.Id == 'M');
                if (app != null)
                    lblCL413.Text = app.Descrizione;
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.AttivitaSvolta2.HasValue)
                    ddlAttivitaSvolta_2PM.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.AttivitaSvolta2.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.TipoLiquidazione.HasValue)
                    ddlTipoLiquidazionePM.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.TipoLiquidazione.ToString();

                if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.AnnoUtileUltimoDecennio.GetValueOrDefault())
                    ddlAnnoUtileUltimoDecennio.SelectedValue = "NO";
                else
                    ddlAnnoUtileUltimoDecennio.SelectedValue = "SI";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.CL413.HasValue)
                {
                    CodiceLegge413 app = liquidazione.areaLiquidazionePensioneFS.ListaCodiceLegge413.Single(x => x.Id == liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.CL413.Value);
                    if (app != null)
                        lblCL413.Text = app.Descrizione;
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta_1PM.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;

            //TODO rivedere?
            if (liquidazione.areaLiquidazionePensioneFS.IsDecorrenzaSuccSett1989.GetValueOrDefault())
            {
                ddlTipoLiquidazionePM.SelectedValue = "9";
                ddlTipoLiquidazionePM.Enabled = false;
            }

            //TODO: messo textbox perchè non conosco i possibili valori 
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.HasValue)
                txtCodiceDirittoQuoteFisse.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.ToString();

        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiPM(AreaLiquidazionePensione areaLiquidazionePensioneFS)
        {
            areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM = new DatiAssicurativi.FondoPM();

            if (!string.IsNullOrEmpty(ddlAttivitaSvolta_1PM.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta_1PM.SelectedValue;

            if (!string.IsNullOrEmpty(ddlAttivitaSvolta_2PM.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.AttivitaSvolta2 = char.Parse(ddlAttivitaSvolta_2PM.SelectedValue);

            if (!string.IsNullOrEmpty(ddlTipoLiquidazionePM.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.TipoLiquidazione = byte.Parse(ddlTipoLiquidazionePM.SelectedValue);

            if (!string.IsNullOrEmpty(ddlAnnoUtileUltimoDecennio.SelectedValue) && ddlAnnoUtileUltimoDecennio.SelectedValue == "SI")
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.AnnoUtileUltimoDecennio = true;
            else
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.AnnoUtileUltimoDecennio = false;

            if (!string.IsNullOrEmpty(lblCL413.Text))
            {
                List<CodiceLegge413> listaCodiceLegge413 = (List<CodiceLegge413>)ViewState["ListaCodiceLegge413"];
                if (listaCodiceLegge413 != null && listaCodiceLegge413.Count > 0)
                {
                    CodiceLegge413 app = listaCodiceLegge413.Single(x => x.Descrizione == lblCL413.Text);
                    if (app != null)
                        areaLiquidazionePensioneFS.DatiAssicurativi.fondoPM.CL413 = app.Id;
                }
            }

            if (!string.IsNullOrEmpty(txtCodiceDirittoQuoteFisse.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse = byte.Parse(txtCodiceDirittoQuoteFisse.Text);

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        private void LoadDdlPM(ILiquidazionePensione liquidazione)
        {
            ddlAttivitaSvolta_1PM.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttivitaSvolta_1PM);
            foreach (DatiAttivitaSvolta attSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte)
                CodeUtility.SetValueDdl(ddlAttivitaSvolta_1PM, attSvolta.Id + " - " + attSvolta.Descrizione, attSvolta.Descrizione, attSvolta.Id);

            ddlTipoLiquidazionePM.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlTipoLiquidazionePM);
            foreach (TipoLiquidazionePM tipoLiquidazionePM in liquidazione.areaLiquidazionePensioneFS.ListaTipoLiquidazionePM)
                CodeUtility.SetValueDdl(ddlTipoLiquidazionePM, tipoLiquidazionePM.Id.ToString() + " - " + tipoLiquidazionePM.Descrizione, tipoLiquidazionePM.Descrizione, tipoLiquidazionePM.Id.ToString());

            ddlAttivitaSvolta_2PM.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttivitaSvolta_2PM);
            foreach (AttivitaSvolta2 attivitaSvolta2 in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolta2)
                CodeUtility.SetValueDdl(ddlAttivitaSvolta_2PM, attivitaSvolta2.Id + " - " + attivitaSvolta2.Descrizione, attivitaSvolta2.Descrizione, attivitaSvolta2.Id.ToString());
        }
        #endregion Fondo PM
    }
}
