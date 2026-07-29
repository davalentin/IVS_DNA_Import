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
    public partial class UCDatiAssicurativiPI_GAS_CL : CustomBaseUserControl, ITitolarePensione, IRecordFondo, ILiquidazionePensione
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
        public event EventHandler ManageCodiceNoCalcoloPIU;
        public event Utility.EventHandlerMessage ManageExCombattente;

        protected void RaiseManageExCombattente(object sender, Utility.EventMessageArgs e)
        {
            if (ManageExCombattente != null)
                ManageExCombattente(sender, e);
        }

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

        protected void RaiseManageCodiceNoCalcoloPIU(object sender, EventArgs e)
        {
            if (ManageCodiceNoCalcoloPIU != null)
                ManageCodiceNoCalcoloPIU(sender, e);
        }

        #endregion EventHandler

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.Tipofondo.HasValue)
                tipoFondo.Value = this.domanda.Tipofondo.ToString();
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

            if (domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI
               || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                ViewState["BackupFondoPI"] = liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI : null;

            //ViewState["DecorrenzaPensione"] = datiPensione.DecorrenzaOriginaria;
            ManageDecorrenzaForReversibilita(datiPensione, liquidazione.areaLiquidazionePensioneFS.DecorrenzaPensioneDirettaDC);
            ViewState[EnumViewState.CategoriaFondoPI.ToString()] = liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI;
            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
                ViewState["IsUsuranti"] = "SI";
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



            LoadDdl(this.domanda.Tipofondo, liquidazione);
            RenderControlsFromTipoFondo(this.domanda.Tipofondo, liquidazione);
            ValorizzaEtichetteDatiAssicurativiCommon(liquidazione, datiPensione);
            switch (domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    ValorizzaEtichetteDatiAssicurativiPI(liquidazione, datiPensione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    ValorizzaEtichetteDatiAssicurativiGAS(liquidazione);
                    ValorizzaCodeRequisiti2SperDonnaGAS(IsDomandaSperDonna, liquidazione);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    ValorizzaEtichetteDatiAssicurativiCL(liquidazione);
                    ValorizzaCodeRequisiti2SperDonna(IsDomandaSperDonna, liquidazione);
                    break;
            }

            if (isDomandaInabilitaAmianto)
            {
                pnlAttEconomProfInd.Visible = true;
                txtAttivitaEconomica.Text = "01";
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Text = "250";
                txtProfessioneIndividuale.Enabled = false;
            }

            GestioneEtichetteRic(datiPensione);
        }

        internal Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativi(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, out List<RecordFondo> listaRecordFondo)
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = null;
            areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiCommon(out listaRecordFondo);
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiPI(areaLiquidazionePensioneFS);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiGAS(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    areaLiquidazionePensioneFS.DatiAssicurativi = GetDatiAssicurativiCL(areaLiquidazionePensioneFS.DatiAssicurativi);
                    break;
            }

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        internal List<RecordFondo> GetElencoRecordFondo()
        {
            if (ViewState[EnumViewState.ElencoRecordFondo.ToString()] != null)
                return (List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()];

            return null;
        }

        #region GridView gvRecordFondo

        protected void gvRecordFondo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            try
            {
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var edit = e.Row.Cells[0].Controls.OfType<LinkButton>().FirstOrDefault();
                    edit.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                    edit.ToolTip = "Modifica";

                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        // implementato per gestire la presenza di un solo record con la grid disabilitata
                        //if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI && ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                        //    (UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] != UtilityCategoriaFondoPI.U &&
                        //    (UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] != UtilityCategoriaFondoPI.V)
                        //    GestioneGridDisabled(false, e.Row, elencoRecordFondo);

                        if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL)
                        {
                            if (e.Row.Cells[1].FindControl("lblcodiceNatura3") != null)
                                e.Row.Cells[1].FindControl("lblcodiceNatura3").Visible = false;
                            if (e.Row.Cells[1].FindControl("ddlCodNatura3") != null)
                                e.Row.Cells[1].FindControl("ddlCodNatura3").Visible = false;
                        }

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
                                BindData(false);
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
                                //aggiunto record blank al primo codice natura in quanto per il fondo PI i codici natura non sono contemplati e i controlli passano.
                                CodeUtility.SetValueDdl(ddlCodNatura1, string.Empty, string.Empty, " ");
                                if (!(ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                                    ((UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U ||
                                    (UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)))
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

                            DropDownList ddlCodiceNoCalcolo = (DropDownList)e.Row.FindControl("ddlCodiceNonCalcolo");

                            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI && ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                                ((UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U ||
                                (UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)) || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                            {
                                ddlCodNatura1.Enabled = false;
                                if ((UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)
                                {
                                    ((extAreaRecordFondo)e.Row.DataItem)._CodiceNonCalcolo = 'S';
                                    ddlCodiceNoCalcolo.Enabled = false;
                                }
                            }

                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCRecordFondo", Page.Theme);
                            CodeUtility.SetCampiGridEdit(e.Row, true, ViewState["DecorrenzaPensione"], this.domanda.Tipofondo);
                            CodeUtility.ManageCampiGridEdit(e.Row, true, datiPensione, this.domanda.Tipofondo);
                            if (ViewState["IsUsuranti"] != null)
                                ManageCodNatura3(ddlCodNatura3);

                            if (!(ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                                ((UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U ||
                                 (UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)))
                            {
                                if (ViewState["IsDomandaSperDonna"] != null)
                                    ManageSperDonna(ddlCodNatura2);

                                if (ViewState["IsCodNatura2Enabled"] != null)
                                    ManageCodNatura2Bonus(ddlCodNatura2);
                            }

                            // implementato per gestire la presenza di un solo record con la grid disabilitata
                            //if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI && ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                            //(UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] != UtilityCategoriaFondoPI.U &&
                            //(UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] != UtilityCategoriaFondoPI.V)
                            //    GestioneGridDisabled(true, e.Row, elencoRecordFondo);

                            if (!(ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                                ((UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U ||
                                 (UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)))
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
                            {
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], null, Page.Theme, string.Empty);
                            }
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
                            CodeUtility.SetValueDdl(ddlCodNatura1, string.Empty, string.Empty, " ");
                            if (!(ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                                    ((UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U ||
                                    (UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)))
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

                            DropDownList ddlCodiceNoCalcolo = (DropDownList)e.Row.FindControl("ddlCodiceNonCalcolo");

                            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI && ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                                ((UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U ||
                                (UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)) || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                            {
                                ddlCodNatura1.Enabled = false;
                                if ((UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.V)
                                {
                                    ((extAreaRecordFondo)e.Row.DataItem)._CodiceNonCalcolo = 'S';
                                    ddlCodiceNoCalcolo.Enabled = false;
                                }
                            }

                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCRecordFondo", Page.Theme);
                            CodeUtility.SetCampiGridEdit(e.Row, false, ViewState["DecorrenzaPensione"], this.domanda.Tipofondo);
                            CodeUtility.ManageCampiGridEdit(e.Row, false, datiPensione, this.domanda.Tipofondo);
                        }
                        else
                        {
                            bool gridDisabled = ViewState["GridDisabled"] != null && (bool)ViewState["GridDisabled"];

                            if (!gridDisabled && e.Row.DataItemIndex == elencoRecordFondo.Count - 1)
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
                            else if (e.Row.DataItemIndex >= 0 && e.Row.DataItemIndex <= elencoRecordFondo.Count - 2) { 
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

        protected void gvRecordFondo_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRecordFondo.EditIndex = e.NewEditIndex;
                BindData(false);
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
                List<RecordFondo> elencoRecordFondo = (List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()];
                GridViewRow row = gvRecordFondo.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvRecordFondo.PageIndex * 10) + e.RowIndex);
                    if (elencoRecordFondo.Count != i + 1)
                        elencoRecordFondo.RemoveAt(elencoRecordFondo.Count - 1);
                    gvRecordFondo.EditIndex = -1;
                    ViewState[EnumViewState.ElencoRecordFondo.ToString()] = elencoRecordFondo;
                    BindData(false);
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
                BindData(false);
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
            List<RecordFondo> listaRecordFondo = BindData(false);
            RecordFondo[] elencoRecordFondo = listaRecordFondo.ToArray();

            if (e.CommandName == "Elimina")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (listaRecordFondo.Count == 0)
                {
                    char cSpace = ' ';
                    elencoRecordFondo = CodeUtility.AggiungiRecord(elencoRecordFondo.ToList(), cSpace, cSpace, cSpace, cSpace, new DateTime(), null).ToArray();
                    ViewState[EnumViewState.ElencoRecordFondo.ToString()] = elencoRecordFondo.ToList();
                }
                else
                {
                    if (row.DataItemIndex == 0)
                        modalitaEdit.Value = "false";

                    listaRecordFondo.RemoveAt(row.DataItemIndex);
                    ViewState[EnumViewState.ElencoRecordFondo.ToString()] = listaRecordFondo;
                }
                BindData(false);
                btnSalvaDatiAssicurativi.Enabled = true;
                btnEliminaDatiAssicurativi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
                RaiseManageCodiceNoCalcoloPIU(this, null);

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
                salvaCommand((GridViewRow)((Control)e.CommandSource).NamingContainer, listaRecordFondo, elencoRecordFondo);
                BindData(false);
                RaiseManageCodiceNoCalcoloPIU(this, null);

            }
            else if (e.CommandName == "Annulla")
            {
                listaRecordFondo = (List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()];
                if (listaRecordFondo.Count > 1)
                {
                    gvRecordFondo.EditIndex = -1;
                    btnSalvaDatiAssicurativi.Enabled = true;
                    btnEliminaDatiAssicurativi.Enabled = true;
                    RaiseAbilitaTastoSalva(this, null);
                }
                BindData(false);
            }
        }

        protected void gvRecordFondo_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRecordFondo.PageIndex = e.NewPageIndex;
                BindData(false);
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

        protected void gvRecordFondo_DataBound(object sender, EventArgs e)
        {
            ManageCheckExCombattenteForPIU(GetElencoRecordFondo());
        }

        #endregion GridView gvRecordFondo

        #region Private Methods Common

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
                ViewState[EnumViewState.ElencoRecordFondo.ToString()] = listaRecordFondo;
            }
            else   //modifica elemento
                elencoRecordFondo = CodeUtility.ModificaRecord(elencoRecordFondo.ToList(), row.DataItemIndex, valueCodNatura1, valueCodNatura2, valueCodNatura3, valueCodiceNonCalcolo, dateDecorrenza, dateCessazione).ToArray();

            gvRecordFondo.EditIndex = -1;

        }

        // da eliminare se non serve piu la gestione Grid con un solo record e disabled
        private void GestioneGridDisabled(bool IsForGrid, GridViewRow row, List<RecordFondo> elencoRecordFondo)
        {
            if (!IsForGrid)
            {
                LinkButton button = ((LinkButton)(row.Cells[0].Controls[0]));
                button.Enabled = IsForGrid;
                button.Text = "&nbsp;&nbsp;&nbsp;";
            }
            else
            {
                salvaCommand(row, elencoRecordFondo, elencoRecordFondo.ToArray());
                BindData(IsForGrid);
            }
        }

        // isGridDasabled: da eliminare se non serve piu la gestione Grid con un solo record e disabled
        private List<RecordFondo> BindData(bool isGridDasabled)
        {
            List<RecordFondo> elencoRecordFondo = GetData(isGridDasabled);
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

        // isGridDasabled: da eliminare se non serve piu la gestione Grid con un solo record e disabled
        private List<RecordFondo> GetData(bool isGridDasabled)
        {
            List<RecordFondo> elencoRecordFondo = new List<RecordFondo>();
            if ((List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()] == null)
            {
                elencoRecordFondo = CodeUtility.CreaRecord();
                ViewState[EnumViewState.ElencoRecordFondo.ToString()] = elencoRecordFondo;
            }
            else
            {
                elencoRecordFondo = (List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()];

                CodeUtility.EliminaRecordVuoti(elencoRecordFondo);
                if (!isGridDasabled)
                    elencoRecordFondo = CodeUtility.AggiungiRecord(elencoRecordFondo, null, null, null, ' ', new DateTime(), null);
                if (ViewState["IsUsuranti"] != null && elencoRecordFondo != null && elencoRecordFondo.Count > 1)
                    elencoRecordFondo[0]._CodiceNatura3 = 'Z';
                ViewState[EnumViewState.ElencoRecordFondo.ToString()] = (List<RecordFondo>)elencoRecordFondo;
            }
            return elencoRecordFondo;
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

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta != null)
                ddlAttivitaSvolta.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta;
            else
                ddlAttivitaSvolta.SelectedIndex = 0;

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico != null)
                ddlCodiceSpecifico.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico.ToString();
            else
                ddlCodiceSpecifico.SelectedIndex = 0;

            if (datiPensione.FlagUnicarpe.HasValue && datiPensione.TipoLetturaUnicarpe.HasValue)
            {
                if (liquidazione.areaLiquidazionePensioneFS != null)
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

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiCommon(out List<RecordFondo> listaRecordFondo)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativi = new Presenter.SvrLiquidazioneFs.DatiAssicurativi();

            listaRecordFondo = (List<RecordFondo>)ViewState[EnumViewState.ElencoRecordFondo.ToString()];
            CodeUtility.EliminaRecordVuoti(listaRecordFondo);
            areaLiquidazionePensioneFS.ListaRecordFondo = listaRecordFondo.ToArray();

            if (!(String.Equals(txtPrimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtPrimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtPrimoVersamento.Text);

            if (!(String.Equals(txtUltimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtUltimoVersamento.Text)))
                areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtUltimoVersamento.Text);

            if (!String.IsNullOrEmpty(ddlAttivitaSvolta.SelectedValue))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaSvolta = ddlAttivitaSvolta.SelectedValue;

            if (!(String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue)))
                areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico = !String.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue) ? Convert.ToByte(ddlCodiceSpecifico.SelectedValue) : (byte?)null;

            if (!String.IsNullOrEmpty(txtAttivitaEconomica.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.AttivitaEconomica = CodeUtility.StringToNullableShort(txtAttivitaEconomica.Text);

            if (!String.IsNullOrEmpty(txtProfessioneIndividuale.Text))
                areaLiquidazionePensioneFS.DatiAssicurativi.ProfessioneIndividuale = CodeUtility.StringToNullableShort(txtProfessioneIndividuale.Text);

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, -1);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            modalitaEdit.Value = "false";
            txtCodiceRequisiti2.Text = "0";

            //if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
            //    txtUltimoVersamento.Text = "30/09/1999";
        }

        private void LoadDdl(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            char? enteFondo = null;
            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    tipoFondo = AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI;
                    enteFondo = 'A';
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    LoadDdlGAS(liquidazione);
                    break;
            }


            if (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null)
                enteFondo = CodeUtility.GetCharCategoriaFondoPI(((UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()]).Value);

            List<CodiceSpecifico> listaCodiceSpecifico;

            //gestione PI cambiata
            //riscrittura per gestiore tutti i casi
            if (enteFondo != null)
            {
                listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico
                    .ToList()
                    .FindAll(delegate (CodiceSpecifico code)
                    {
                        return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString()
                            && code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant()
                            && code.EnteFondo == enteFondo);
                    });


                if (listaCodiceSpecifico == null || listaCodiceSpecifico.Count == 0)
                {
                    listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico
                        .ToList()
                        .FindAll(delegate (CodiceSpecifico code)
                        {
                            return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString()
                                && code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant()
                                && code.EnteFondo == null);
                        });
                }

                if (listaCodiceSpecifico != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico != null)
                {
                    var codice = listaCodiceSpecifico.Find(x=> x.Id == liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico);
                    if(codice == null)
                    {
                        listaCodiceSpecifico.Add(liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().Find(delegate (CodiceSpecifico code)
                        {
                            return (code.Id == liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceSpecifico);
                        }));
                    }

                }
            }
            else
            {
                listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico
                    .ToList()
                    .FindAll(delegate (CodiceSpecifico code)
                    {
                        return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString()
                            && code.Fondo == tipoFondo.Value.ToString().ToUpperInvariant()
                            && code.EnteFondo == null);
                    });
            }

            ViewState["CodiciNatura"] = liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura;

            ddlAttivitaSvolta.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttivitaSvolta);
            if (liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte != null)
                foreach (DatiAttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte)
                {
                    if (!string.IsNullOrEmpty(this.domanda.Categoria) && !string.IsNullOrEmpty(this.domanda.Categoria.Trim()) && this.domanda.Categoria.Trim().EndsWith("PIA"))
                    {
                        if (attivitaSvolta.TraduzioneSuGp.Trim() == "1")
                            CodeUtility.SetValueDdl(ddlAttivitaSvolta, attivitaSvolta.Descrizione, attivitaSvolta.Descrizione, attivitaSvolta.Id);
                    }
                    else
                        CodeUtility.SetValueDdl(ddlAttivitaSvolta, attivitaSvolta.Descrizione, attivitaSvolta.Descrizione, attivitaSvolta.Id);
                }

            ddlCodiceSpecifico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceSpecifico);
            foreach (CodiceSpecifico codSpec in listaCodiceSpecifico)
                CodeUtility.SetValueDdl(ddlCodiceSpecifico, (codSpec.TraduzioneGp.HasValue ? codSpec.TraduzioneGp + " - " : string.Empty) + codSpec.Descrizione, codSpec.Descrizione, codSpec.Id.Value.ToString());

            ddlCodRequisiti1.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodRequisiti1);
            foreach (CodiceRequisito1 codReq1 in liquidazione.areaLiquidazionePensioneFS.ListaCodiceRequisito1)
                CodeUtility.SetValueDdl(ddlCodRequisiti1, codReq1.Id + " - " + codReq1.Descrizione, codReq1.Descrizione, codReq1.Id);
        }

        private void RenderControlsFromTipoFondo(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, ILiquidazionePensione liquidazione)
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

            switch (tipoFondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    pnlPI.Visible = true;
                    RenderControlsPI();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    pnlGAS.Visible = true;
                    RenderControlsGAS(liquidazione);
                    pnlCodiceRequisiti.Visible = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL:
                    pnlCommonFooter.Visible = false;
                    pnlCL.Visible = true;
                    pnlCodiceRequisiti.Visible = true;
                    ddlCodRequisiti1.Enabled = false;
                    //ENG - CodicePensioneSenzaRequisiti visibile ed editabile per le PL e RIC categoria pensione VCL
                    if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura || this.domanda.Categoria.ToString().Trim().ToUpperInvariant() == "VCL")
                        pnlCodicePensioneSenzaRequisiti.Visible = true;
                    if (this.domanda.Categoria.ToString().Trim().ToUpperInvariant() == "VCL")
                        ddlCodicePensioneSenzaRequisiti.Enabled = true;
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

        private void ValorizzaCodeRequisiti2SperDonna(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione)
        {
            if (IsDomandaSperDonna && (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.HasValue))
                txtCodiceRequisiti2.Text = "9";
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
            {
                //COMMON
                gvRecordFondo.Enabled = false;
                txtPrimoVersamento.Enabled = false;
                txtUltimoVersamento.Enabled = false;
                txtAttivitaEconomica.Enabled = false;
                txtProfessioneIndividuale.Enabled = false;
                ddlAttivitaSvolta.Enabled = false;
                ddlAttivitaSvolta_RF.Enabled = false;
                btnEliminaDatiAssicurativi.Enabled = false;


                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL)
                    ddlCodRequisiti1.Enabled = false;

                if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                {
                    txtServizioUtileAA.Enabled = false;
                    RequiredFieldValidator2.Enabled = false;
                    txtServizioUtileMM.Enabled = false;
                    RequiredFieldValidator3.Enabled = false;
                    txtServizioUtileGG.Enabled = false;
                    RequiredFieldValidator4.Enabled = false;
                    ddlLivello.Enabled = false;
                    txtSettimaneMaggiorazione.Enabled = false;
                    txtSettimaneEsclusive.Enabled = false;
                    txtSettimaneINPDAI.Enabled = false;
                    txtServizioNonUtileAA.Enabled = false;
                    txtServizioNonUtileMM.Enabled = false;
                    txtServizioNonUtileGG.Enabled = false;
                    ddlCodiceSpecifico_RF.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS)
                {
                    ddlCodDirittoQuoteFisse.Enabled = false;
                    ddlConvenzione.Enabled = false;
                    txtMesiAnte46.Enabled = false;
                    txtAnzianitaUtileDal46.Enabled = false;
                    txtMesiUtiliIndennitaAggiuntiva.Enabled = false;
                    txtMesiNonUtiliIndennitaAggiuntiva.Enabled = false;
                    txtServizioUtileIndennitaAggiuntiva.Enabled = false;
                    txtRetribuzione.Enabled = false;
                    txtDitta.Enabled = false;
                    txtPercentualeRiduzione.Enabled = false;
                    ddlCodiceDimissioni.Enabled = false;
                    ddlCodicePensioneRidotta.Enabled = false;
                    txtConguaglio.Enabled = false;
                }
                else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.CL)
                {
                    txtServizioUtileAA_CL.Enabled = false;
                    RFVtxtServizioUtileAA_CL.Enabled = false;
                    txtServizioUtileMM_CL.Enabled = false;
                    RFVtxtServizioUtileMM_CL.Enabled = false;
                    txtImportoAltraPensione.Enabled = false;
                    txtAnniDifferimento.Enabled = false;
                    ddlCodicePensioneSenzaRequisiti.Enabled = false;
                    txtEtaPerfezionamentoRequisiti.Enabled = false;
                    txtDataPerfezionamentoRequisiti.Enabled = false;
                    RFVtxtDataPerfezionamentoRequisiti.Enabled = false;
                    ddlContrProvv.Enabled = false;
                    ddlCodiceSpecifico.Enabled = false;
                    ddlCodiceSpecifico_RF.Enabled = false;
                }
            }


            if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
            {
                //Per il momento sblocchiamo per tutte le PI
                txtPrimoVersamento.Enabled = true;
                txtUltimoVersamento.Enabled = true;
            }
        }
        #endregion Private Methods Common

        #region Fondo PI

        private void ValorizzaEtichetteDatiAssicurativiPI(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
            {
                List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                if (areaRecord.Count() == 0 ||
                    (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null))
                {
                    // Gestione standard delle griglie
                    ViewState[EnumViewState.ElencoRecordFondo.ToString()] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                    ViewState["GridDisabled"] = false;
                    BindData(false);
                }
                else
                {
                    // Gestione con griglia bloccata
                    ViewState[EnumViewState.ElencoRecordFondo.ToString()] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                    ViewState["GridDisabled"] = true;
                    BindData(true);
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.HasValue)
                {
                    ValorizzaEtichetteDatiAssicurativiPIDefault(liquidazione);
                    switch (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.Value)
                    {
                        case UtilityCategoriaFondoPI.A:
                        case UtilityCategoriaFondoPI.Uno:
                        case UtilityCategoriaFondoPI.Y:
                        case UtilityCategoriaFondoPI.B:
                            ValorizzaEtichetteDatiAssicurativiPICatA1YB(liquidazione);
                            break;
                        case UtilityCategoriaFondoPI.U:
                            ValorizzaEtichetteDatiAssicurativiPICatU(liquidazione);
                            break;
                        case UtilityCategoriaFondoPI.V:
                            ValorizzaEtichetteDatiAssicurativiPICatV(liquidazione);
                            break;

                    }
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.HasValue)
            {
                if (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.A)
                {
                    ddlAttivitaSvolta.SelectedIndex = 1;
                    ddlAttivitaSvolta.Enabled = false;
                    //if (!Utility.IsDomandaReversibilita(datiPensione) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    //    txtUltimoVersamento.Text = "30/09/1999";
                }

                if (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.Uno)
                {
                    DatiAttivitaSvolta attivitaSvolta = liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte.ToList().Find(x => x.TraduzioneSuGp.Trim() == "E");
                    if (attivitaSvolta != null)
                    {
                        ddlAttivitaSvolta.SelectedValue = attivitaSvolta.Id;
                        ddlAttivitaSvolta.Enabled = false;
                    }
                    //lblStipendioAnnuo.Text = "Importo Pensione:";
                    //RFVtxtStipendioAnnuo.ErrorMessage = "Importo Pensione: campo obbligatorio";
                    //if (!Utility.IsDomandaReversibilita(datiPensione) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    //    txtUltimoVersamento.Text = "30/09/1999";
                }

                if (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.Y)
                {
                    DatiAttivitaSvolta attivitaSvolta = liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte.ToList().Find(x => x.TraduzioneSuGp.Trim() == "P");
                    if (attivitaSvolta != null)
                    {
                        ddlAttivitaSvolta.SelectedValue = attivitaSvolta.Id;
                        ddlAttivitaSvolta.Enabled = false;
                    }
                    //lblStipendioAnnuo.Text = "Importo Pensione:";
                    //RFVtxtStipendioAnnuo.ErrorMessage = "Importo Pensione: campo obbligatorio";
                    //if (!Utility.IsDomandaReversibilita(datiPensione) && !CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    //    txtUltimoVersamento.Text = "30/09/1999";
                }

                if (liquidazione.areaLiquidazionePensioneFS.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.U)
                {
                    pnlPensioneAGO.Visible = true;
                    if (!Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
                    {
                        txtPensioneAGOCertificato.Enabled = false;
                        txtPensioneAGOCodiceCategoria.Enabled = false;
                        txtPensioneAGOSede.Enabled = false;
                    }
                }
            }

        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiPI(AreaLiquidazionePensione areaLiquidazionePensioneFS)
        {
            if (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null)
                areaLiquidazionePensioneFS.CategoriaFondoPI = (Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()];
            if (areaLiquidazionePensioneFS.CategoriaFondoPI.HasValue)
            {
                var fondoBackup = ViewState["BackupFondoPI"] as DatiAssicurativi.FondoPI;
                areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI = fondoBackup != null ?
                    fondoBackup : new Presenter.SvrLiquidazioneFs.DatiAssicurativi.FondoPI();

                GetDatiAssicurativiPIDefault(areaLiquidazionePensioneFS.DatiAssicurativi);
                switch (areaLiquidazionePensioneFS.CategoriaFondoPI.Value)
                {
                    case UtilityCategoriaFondoPI.A:
                    case UtilityCategoriaFondoPI.Uno:
                    case UtilityCategoriaFondoPI.Y:
                    case UtilityCategoriaFondoPI.B:
                        GetDatiAssicurativiPICatA1YB(areaLiquidazionePensioneFS.DatiAssicurativi);
                        break;
                    case UtilityCategoriaFondoPI.U:
                        GetDatiAssicurativiPICatU(areaLiquidazionePensioneFS.DatiAssicurativi);
                        break;
                    case UtilityCategoriaFondoPI.V:
                        GetDatiAssicurativiPICatV(areaLiquidazionePensioneFS.DatiAssicurativi);
                        break;
                }
            }

            return areaLiquidazionePensioneFS.DatiAssicurativi;
        }

        private void RenderControlsPI()
        {
            if (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null)
            {
                pnlPICommon.Visible = true;

                switch ((UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()])
                {
                    case UtilityCategoriaFondoPI.A:
                    case UtilityCategoriaFondoPI.Uno:
                    case UtilityCategoriaFondoPI.Y:
                    case UtilityCategoriaFondoPI.B:
                        break;
                    case UtilityCategoriaFondoPI.U:
                        pnlPICatU.Visible = true;
                        break;
                    case UtilityCategoriaFondoPI.V:
                        pnlAttivitaSvolta.Visible = false;
                        pnlPICatV.Visible = true;
                        break;
                }

                //Per il momento blocchiamo per tutte le PI
                //RFtxtMatricola.Enabled = false;
                //txtMatricola_CV.Enabled = false;
                //txtMatricola.Enabled = false;
                //txtDecorrenzaPensioneEliminata.Enabled = false;
            }
        }

        private void ManageCheckExCombattenteForPIU(List<RecordFondo> lstRecordFondo)
        {
            if (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null &&
                   ((UtilityCategoriaFondoPI)ViewState[EnumViewState.CategoriaFondoPI.ToString()] == UtilityCategoriaFondoPI.U))
            {
                if (lstRecordFondo != null && lstRecordFondo.Exists(x => x._CodiceNonCalcolo == 'N'))
                {
                    RaiseManageExCombattente(this, new Utility.EventMessageArgs("false"));
                }
                else
                    RaiseManageExCombattente(this, new Utility.EventMessageArgs("true"));
            }
        }

        #region Cat.A 1 Y 

        private void ValorizzaEtichetteDatiAssicurativiPICatA1YB(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile != null)
                {
                    txtServizioUtileAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.Value.ToString() : string.Empty;

                    txtServizioUtileMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.Value.ToString() : string.Empty;

                    txtServizioUtileGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.Value.ToString() : string.Empty;

                } 
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiPICatA1YB(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            
            datiAssicurativi.fondoPI.ServizioUtile = new DatiAssicurativi.DatiServizioUtile();

            if (!String.IsNullOrEmpty(txtServizioUtileAA.Text))
                datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA = short.Parse(txtServizioUtileAA.Text);

            if (!String.IsNullOrEmpty(txtServizioUtileMM.Text))
                datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM = short.Parse(txtServizioUtileMM.Text);

            if (!String.IsNullOrEmpty(txtServizioUtileGG.Text))
                datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG = short.Parse(txtServizioUtileGG.Text);

            return datiAssicurativi;
        }

        #endregion Cat.A 1 Y 

        #region Cat.U

        private void ValorizzaEtichetteDatiAssicurativiPICatU(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile != null)
                {
                    txtServizioUtileAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.Value.ToString() : string.Empty;

                    txtServizioUtileMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.Value.ToString() : string.Empty;

                    txtServizioUtileGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.Livello.HasValue)
                    ddlLivello.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.Livello.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.SettimaneMaggiorazione.HasValue)
                    txtSettimaneMaggiorazione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.SettimaneMaggiorazione.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.SettimaneEsclusive.HasValue)
                    txtSettimaneEsclusive.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.SettimaneEsclusive.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.SettimaneINPDAI.HasValue)
                    txtSettimaneINPDAI.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.SettimaneINPDAI.Value.ToString();

                if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.CodiceCategoria))
                    txtPensioneAGOCodiceCategoria.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.CodiceCategoria;

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.Sede.HasValue)
                    txtPensioneAGOSede.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.Sede.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.Certificato.HasValue)
                    txtPensioneAGOCertificato.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.Certificato.Value.ToString();


            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiPICatU(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoPI.ServizioUtile = new DatiAssicurativi.DatiServizioUtile();

            if (!string.IsNullOrEmpty(txtServizioUtileAA.Text) || !string.IsNullOrEmpty(txtServizioUtileMM.Text) || !string.IsNullOrEmpty(txtServizioUtileGG.Text))
            {
                datiAssicurativi.fondoPI.ServizioUtile = new DatiAssicurativi.DatiServizioUtile();
                if (!string.IsNullOrEmpty(txtServizioUtileAA.Text))
                    datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA = CodeUtility.StringToNullableShort(txtServizioUtileAA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileMM.Text))
                    datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM = CodeUtility.StringToNullableShort(txtServizioUtileMM.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileGG.Text))
                    datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG = CodeUtility.StringToNullableShort(txtServizioUtileGG.Text);
            }

            if (!string.IsNullOrEmpty(ddlLivello.SelectedValue))
                datiAssicurativi.fondoPI.Livello = CodeUtility.StringToNullableByte(ddlLivello.SelectedValue);

            if (!string.IsNullOrEmpty(txtSettimaneMaggiorazione.Text))
                datiAssicurativi.fondoPI.SettimaneMaggiorazione = CodeUtility.StringToNullableShort(txtSettimaneMaggiorazione.Text);

            if (!string.IsNullOrEmpty(txtSettimaneEsclusive.Text))
                datiAssicurativi.fondoPI.SettimaneEsclusive = CodeUtility.StringToNullableShort(txtSettimaneEsclusive.Text);

            if (!string.IsNullOrEmpty(txtSettimaneINPDAI.Text))
                datiAssicurativi.fondoPI.SettimaneINPDAI = CodeUtility.StringToNullableShort(txtSettimaneINPDAI.Text);

            return datiAssicurativi;
        }
        #endregion Cat.U

        #region Cat.V

        private void ValorizzaEtichetteDatiAssicurativiPICatV(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile != null)
                {
                    txtServizioUtileAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.Value.ToString() : string.Empty;

                    txtServizioUtileMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.Value.ToString() : string.Empty;

                    txtServizioUtileGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                }

                txtServizioNonUtileAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioNonUtileAA.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioNonUtileAA.Value.ToString() : string.Empty;

                txtServizioNonUtileMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioNonUtileMM.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioNonUtileMM.Value.ToString() : string.Empty;

                txtServizioNonUtileGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioNonUtileGG.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioNonUtileGG.Value.ToString() : string.Empty;
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiPICatV(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoPI.ServizioUtile = new DatiAssicurativi.DatiServizioUtile();

            if (!string.IsNullOrEmpty(txtServizioUtileAA.Text) || !string.IsNullOrEmpty(txtServizioUtileMM.Text) || !string.IsNullOrEmpty(txtServizioUtileGG.Text))
            {
                datiAssicurativi.fondoPI.ServizioUtile = new DatiAssicurativi.DatiServizioUtile();
                if (!string.IsNullOrEmpty(txtServizioUtileAA.Text))
                    datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA = CodeUtility.StringToNullableShort(txtServizioUtileAA.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileMM.Text))
                    datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM = CodeUtility.StringToNullableShort(txtServizioUtileMM.Text);
                if (!string.IsNullOrEmpty(txtServizioUtileGG.Text))
                    datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG = CodeUtility.StringToNullableShort(txtServizioUtileGG.Text);
            }

            if (!string.IsNullOrEmpty(txtServizioNonUtileAA.Text))
                datiAssicurativi.fondoPI.ServizioNonUtileAA = CodeUtility.StringToNullableShort(txtServizioNonUtileAA.Text);

            if (!string.IsNullOrEmpty(txtServizioNonUtileMM.Text))
                datiAssicurativi.fondoPI.ServizioNonUtileMM = CodeUtility.StringToNullableShort(txtServizioNonUtileGG.Text);

            if (!string.IsNullOrEmpty(txtServizioNonUtileGG.Text))
                datiAssicurativi.fondoPI.ServizioNonUtileGG = CodeUtility.StringToNullableShort(txtServizioNonUtileGG.Text);

            return datiAssicurativi;
        }
        #endregion Cat.V

        #region PI Default 
        private void ValorizzaEtichetteDatiAssicurativiPIDefault(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile != null)
                {
                    txtServizioUtileAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA.Value.ToString() : string.Empty;

                    txtServizioUtileMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM.Value.ToString() : string.Empty;

                    txtServizioUtileGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.HasValue ? liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG.Value.ToString() : string.Empty;

                }
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiPIDefault(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoPI.ServizioUtile = new DatiAssicurativi.DatiServizioUtile();

            if (!String.IsNullOrEmpty(txtServizioUtileAA.Text))
                datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileAA = short.Parse(txtServizioUtileAA.Text);

            if (!String.IsNullOrEmpty(txtServizioUtileMM.Text))
                datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileMM = short.Parse(txtServizioUtileMM.Text);

            if (!String.IsNullOrEmpty(txtServizioUtileGG.Text))
                datiAssicurativi.fondoPI.ServizioUtile.ServizioUtileGG = short.Parse(txtServizioUtileGG.Text);

            return datiAssicurativi;
        }


        #endregion PI Default 
        #endregion Fondo PI

        #region Fondo GAS
        private void ValorizzaEtichetteDatiAssicurativiGAS(ILiquidazionePensione liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
                {
                    List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                    if (areaRecord.Count() == 0)
                    {
                        ViewState[EnumViewState.ElencoRecordFondo.ToString()] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                        BindData(false);
                    }
                    else
                    {
                        ViewState[EnumViewState.ElencoRecordFondo.ToString()] = areaRecord;
                        BindData(false);
                    }
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null)
                {
                    txtUltimoVersamento.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione != null ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione) : string.Empty;

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 != null)
                        if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                            ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS != null)
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Convenzione != null)
                            ddlConvenzione.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Convenzione;

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse != null)
                            ddlCodDirittoQuoteFisse.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceDirittoQuoteFisse.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.MesiAnte46 != null)
                            txtMesiAnte46.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.MesiAnte46.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.AnzianitaUtileDal46 != null)
                            txtAnzianitaUtileDal46.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.AnzianitaUtileDal46.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.MesiUtiliIndennitaAggiuntiva != null)
                            txtMesiUtiliIndennitaAggiuntiva.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.MesiUtiliIndennitaAggiuntiva.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.MesiNonUtiliIndennitaAggiuntiva != null)
                            txtMesiNonUtiliIndennitaAggiuntiva.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.MesiNonUtiliIndennitaAggiuntiva.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.ServizioUtileIndennitaAggiuntiva != null)
                            txtServizioUtileIndennitaAggiuntiva.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.ServizioUtileIndennitaAggiuntiva.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Retribuzione != null)
                            txtRetribuzione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Retribuzione.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Ditta != null)
                            txtDitta.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Ditta.Trim();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.PercentualeRiduzione != null)
                            txtPercentualeRiduzione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.PercentualeRiduzione.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.CodiceDimissioni != null)
                        {
                            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.CodiceDimissioni.Value)
                                ddlCodiceDimissioni.SelectedValue = "SI";
                            else
                                ddlCodiceDimissioni.SelectedValue = "NO";
                        }

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.CodicePensioneRidotta != null)
                        {
                            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.CodicePensioneRidotta.Value)
                                ddlCodicePensioneRidotta.SelectedValue = "SI";
                            else
                                ddlCodicePensioneRidotta.SelectedValue = "NO";
                        }

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Conguaglio != null)
                            txtConguaglio.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoGAS.Conguaglio.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 != null)
                            ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();
                    }
                }
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiGAS(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoGAS = new DatiAssicurativi.FondoGAS();

            if (!string.IsNullOrEmpty(ddlConvenzione.SelectedValue))
                datiAssicurativi.fondoGAS.Convenzione = ddlConvenzione.SelectedValue;

            if (!string.IsNullOrEmpty(ddlCodDirittoQuoteFisse.SelectedValue))
                datiAssicurativi.CodiceDirittoQuoteFisse = byte.Parse(ddlCodDirittoQuoteFisse.SelectedValue);

            if (!string.IsNullOrEmpty(txtMesiAnte46.Text))
                datiAssicurativi.fondoGAS.MesiAnte46 = short.Parse(txtMesiAnte46.Text);

            if (!string.IsNullOrEmpty(txtAnzianitaUtileDal46.Text))
                datiAssicurativi.fondoGAS.AnzianitaUtileDal46 = short.Parse(txtAnzianitaUtileDal46.Text);

            if (!string.IsNullOrEmpty(txtMesiUtiliIndennitaAggiuntiva.Text))
                datiAssicurativi.fondoGAS.MesiUtiliIndennitaAggiuntiva = short.Parse(txtMesiUtiliIndennitaAggiuntiva.Text);

            if (!string.IsNullOrEmpty(txtMesiNonUtiliIndennitaAggiuntiva.Text))
                datiAssicurativi.fondoGAS.MesiNonUtiliIndennitaAggiuntiva = short.Parse(txtMesiNonUtiliIndennitaAggiuntiva.Text);

            if (!string.IsNullOrEmpty(txtServizioUtileIndennitaAggiuntiva.Text))
                datiAssicurativi.fondoGAS.ServizioUtileIndennitaAggiuntiva = short.Parse(txtServizioUtileIndennitaAggiuntiva.Text);

            if (!string.IsNullOrEmpty(txtRetribuzione.Text))
                datiAssicurativi.fondoGAS.Retribuzione = decimal.Parse(txtRetribuzione.Text);

            if (!string.IsNullOrEmpty(txtDitta.Text.Trim()))
                datiAssicurativi.fondoGAS.Ditta = txtDitta.Text.Trim().ToUpperInvariant();

            if (!string.IsNullOrEmpty(txtPercentualeRiduzione.Text))
                datiAssicurativi.fondoGAS.PercentualeRiduzione = short.Parse(txtPercentualeRiduzione.Text);

            if (String.Equals(ddlCodiceDimissioni.SelectedValue, "SI"))
                datiAssicurativi.fondoGAS.CodiceDimissioni = true;
            else if (String.Equals(ddlCodiceDimissioni.SelectedValue, "NO"))
                datiAssicurativi.fondoGAS.CodiceDimissioni = false;

            if (String.Equals(ddlCodicePensioneRidotta.SelectedValue, "SI"))
                datiAssicurativi.fondoGAS.CodicePensioneRidotta = true;
            else if (String.Equals(ddlCodicePensioneRidotta.SelectedValue, "NO"))
                datiAssicurativi.fondoGAS.CodicePensioneRidotta = false;

            if (!string.IsNullOrEmpty(txtConguaglio.Text))
                datiAssicurativi.fondoGAS.Conguaglio = decimal.Parse(txtConguaglio.Text);

            if (!string.IsNullOrEmpty(ddlCodRequisiti1.SelectedValue))
                datiAssicurativi.CodiceRequisiti1 = char.Parse(ddlCodRequisiti1.SelectedValue);

            return datiAssicurativi;
        }

        private void RenderControlsGAS(ILiquidazionePensione liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.IsCodDirittoQuoteFisseVisible.HasValue)
                    pnlCodDirittoQuoteFisse.Visible = liquidazione.areaLiquidazionePensioneFS.IsCodDirittoQuoteFisseVisible.Value;

                if (liquidazione.areaLiquidazionePensioneFS.IsIndennitaAggiuntivaVisible.HasValue)
                    pnlIndennitaAggiuntiva.Visible = liquidazione.areaLiquidazionePensioneFS.IsIndennitaAggiuntivaVisible.Value;
            }
        }

        private void LoadDdlGAS(ILiquidazionePensione liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceConvenzioneInternazionale != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceConvenzioneInternazionale.Count() > 0)
                {
                    ddlConvenzione.Items.Clear();
                    CodeUtility.SetItemBlankDdl(ddlConvenzione);
                    foreach (CodiceConvenzioneInternazionale codConvInternazionale in liquidazione.areaLiquidazionePensioneFS.ListaCodiceConvenzioneInternazionale)
                        CodeUtility.SetValueDdl(ddlConvenzione, codConvInternazionale.Descrizione, codConvInternazionale.Descrizione, codConvInternazionale.Id);
                }
            }
        }

        private void ValorizzaCodeRequisiti2SperDonnaGAS(bool IsDomandaSperDonna, ILiquidazionePensione liquidazione)
        {
            if (IsDomandaSperDonna && (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti2.HasValue))
                txtCodiceRequisiti2.Text = "9";
        }
        #endregion Fondo GAS

        #region Fondo CL
        private void ValorizzaEtichetteDatiAssicurativiCL(ILiquidazionePensione liquidazione)
        {
            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo != null)
                {
                    List<RecordFondo> areaRecord = liquidazione.areaLiquidazionePensioneFS.ListaRecordFondo.ToList();
                    if (areaRecord.Count() == 0)
                    {
                        ViewState[EnumViewState.ElencoRecordFondo.ToString()] = CodeUtility.AggiungiRecord(areaRecord, null, null, null, ' ', new DateTime(), null);
                        BindData(false);
                    }
                    else
                    {
                        ViewState[EnumViewState.ElencoRecordFondo.ToString()] = areaRecord;
                        BindData(false);
                    }
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi != null)
                {
                    txtUltimoVersamento.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione != null ? String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.FineAssicurazione) : string.Empty;

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1 != null)
                        if (!String.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString()))
                            ddlCodRequisiti1.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.CodiceRequisiti1.ToString();

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL != null)
                    {
                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.AnniDifferimento.HasValue)
                            txtAnniDifferimento.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.AnniDifferimento.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.CodicePensioneSenzaRequisiti.HasValue)
                            ddlCodicePensioneSenzaRequisiti.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.CodicePensioneSenzaRequisiti.Value ? "SI" : "NO";

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.DataPerfezionamentoRequisiti.HasValue)
                            txtDataPerfezionamentoRequisiti.Text = String.Format("{0:MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.DataPerfezionamentoRequisiti.Value);

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.EtaPerfezionamentoRequisiti.HasValue)
                            txtEtaPerfezionamentoRequisiti.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.EtaPerfezionamentoRequisiti.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ImportoAltraPensione.HasValue)
                            txtImportoAltraPensione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ImportoAltraPensione.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ServizioUtileAA.HasValue)
                            txtServizioUtileAA_CL.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ServizioUtileAA.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ServizioUtileMM.HasValue)
                            txtServizioUtileMM_CL.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ServizioUtileMM.ToString();

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ContrProvv.HasValue)
                        {
                            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ContrProvv.Value == 'S')
                                ddlContrProvv.SelectedValue = "SI";
                            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativi.fondoCL.ContrProvv.Value == 'N')
                                ddlContrProvv.SelectedValue = "NO";
                        }
                        else
                            ddlContrProvv.SelectedValue = "NO";
                    }
                }

                ddlCodRequisiti1.SelectedValue = "A";
            }
        }

        private Presenter.SvrLiquidazioneFs.DatiAssicurativi GetDatiAssicurativiCL(Presenter.SvrLiquidazioneFs.DatiAssicurativi datiAssicurativi)
        {
            datiAssicurativi.fondoCL = new DatiAssicurativi.FondoCL();

            if (!string.IsNullOrEmpty(txtAnniDifferimento.Text))
                datiAssicurativi.fondoCL.AnniDifferimento = short.Parse(txtAnniDifferimento.Text);

            if (ddlCodicePensioneSenzaRequisiti.SelectedValue == "SI")
                datiAssicurativi.fondoCL.CodicePensioneSenzaRequisiti = true;
            else
                datiAssicurativi.fondoCL.CodicePensioneSenzaRequisiti = false;

            if (!string.IsNullOrEmpty(txtDataPerfezionamentoRequisiti.Text))
                datiAssicurativi.fondoCL.DataPerfezionamentoRequisiti = Utility.GetDateFromString(txtDataPerfezionamentoRequisiti.Text);

            if (!string.IsNullOrEmpty(txtEtaPerfezionamentoRequisiti.Text))
                datiAssicurativi.fondoCL.EtaPerfezionamentoRequisiti = byte.Parse(txtEtaPerfezionamentoRequisiti.Text);

            if (!string.IsNullOrEmpty(txtImportoAltraPensione.Text))
                datiAssicurativi.fondoCL.ImportoAltraPensione = decimal.Parse(txtImportoAltraPensione.Text);

            if (!string.IsNullOrEmpty(txtServizioUtileAA_CL.Text))
                datiAssicurativi.fondoCL.ServizioUtileAA = short.Parse(txtServizioUtileAA_CL.Text);

            if (!string.IsNullOrEmpty(txtServizioUtileMM_CL.Text))
                datiAssicurativi.fondoCL.ServizioUtileMM = short.Parse(txtServizioUtileMM_CL.Text);

            if (ddlContrProvv.SelectedValue == "SI")
                datiAssicurativi.fondoCL.ContrProvv = 'S';
            else
                datiAssicurativi.fondoCL.ContrProvv = 'N';

            if (!string.IsNullOrEmpty(ddlCodRequisiti1.SelectedValue))
                datiAssicurativi.CodiceRequisiti1 = char.Parse(ddlCodRequisiti1.SelectedValue);

            if (!(String.IsNullOrEmpty(txtCodiceRequisiti2.Text)))
                datiAssicurativi.CodiceRequisiti2 = char.Parse(txtCodiceRequisiti2.Text);

            return datiAssicurativi;
        }

        #endregion Fondo CL

        #region Enum
        public enum EnumViewState
        {
            CategoriaFondoPI,
            ElencoRecordFondo,
        }
        #endregion Enum


    }
}
