using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi
{
    public partial class UCSupplementi : CustomBaseUserControl, ISupplementi
    {
        AreaSupplementi elencoSupplementiFromViewState = new AreaSupplementi();
        AreaSupplementi elencoSupplementiToViewState = new AreaSupplementi();

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ISup
        public long numDomanda { get; set; }
        public AreaSupplementi lstSupplementi { get; set; }
        public Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public DatiContribuzioneEnpals datiContribuzioneEnpals { get; set; }
        #endregion ISup

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
        }

        internal void ValorizzaEtichette(ISupplementi supplementi)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            if (supplementi.risposta.DecorrenzaPensioneDanteCausa.HasValue)
                hfDecorrenzaOriginaria.Value = supplementi.risposta.DecorrenzaPensioneDanteCausa.Value.ToString("dd/MM/yyyy");
            else if (datipensione.DecorrenzaOriginaria.HasValue)
                hfDecorrenzaOriginaria.Value = datipensione.DecorrenzaOriginaria.Value.ToString("dd/MM/yyyy");

            ViewState["elencoTipoSupplementi"] = supplementi.risposta.ListTipoSupplementi;
            List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>();

            if (supplementi.risposta != null)
                elencoSupplementi = new List<DatiSupplementi>(supplementi.risposta.ListDatiSupplementi);
            elencoSupplementi.Add(new DatiSupplementi());

            elencoSupplementiToViewState.ListDatiSupplementi = elencoSupplementi.ToArray();
            ViewState["elencoSupplementi"] = elencoSupplementiToViewState;

            bool IsTipoCalcoloModificato = supplementi.risposta.IsTipoCalcoloModificato.GetValueOrDefault();
            ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()] = IsTipoCalcoloModificato;

            GvSupplementi_Load();

            GestioneRic();
        }

        protected void btnSalvaTabSupplementi_Click(object sender, EventArgs e)
        {

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito();
            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();

            this.lstSupplementi = GetDatiUcSupplementi();
            presenterSupplementi.SalvaTabSupplementiByDomanda(this);

            if (!this.HasError)
            {
                GvSupplementi_Load();
            }

            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaEsito.TipoEsito.KO;
            }

            Utility.CustomEventArgs Cevnt = new Utility.CustomEventArgs(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS, null);
            RaiseSalvaSupplementi(this, Cevnt);
        }

        protected void btnEliminaTabSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();

            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();
            presenterSupplementi.EliminaTabSupplementiByDomanda(this);

            //ENG - MEMO 50/2023 
            string valoreControlloMemo50_2023 = string.Empty;
            if (ViewState["AbilitazioneMemo50_2023"] != null)
                valoreControlloMemo50_2023 = (string)ViewState["AbilitazioneMemo50_2023"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esitoControllo = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out valoreControlloMemo50_2023);
                if (esitoControllo != null && esitoControllo.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloMemo50_2023) && !String.IsNullOrEmpty(valoreControlloMemo50_2023.Trim()))
                    ViewState["AbilitazioneMemo50_2023"] = valoreControlloMemo50_2023.Trim();
            }

            if (this.HasError == true)
            {
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && Utility.IsRicostituzione_MotiviContributivi(datiPensione) &&
                    datiPensione.CodeTipo == "0001" && !this.domanda.IsDomandaINPDAP)
                {
                    esito.Messaggio = this.ErrorMessage;
                    esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO;
                }
                else
                    this.ErrorMessage = "Errore durante l'eliminazione dei Supplementi";
            }
            else
            {
                modalitaEdit.Value = "false";
                DatiSupplementi supp = new DatiSupplementi();
                List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>();
                elencoSupplementi.Add(supp);
                elencoSupplementiToViewState.ListDatiSupplementi = elencoSupplementi.ToArray();
                ViewState["elencoSupplementi"] = elencoSupplementiToViewState;
                GvSupplementi_Load();
            }
            Utility.CustomEventArgs Cevnt = new Utility.CustomEventArgs(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS, null);
            RaiseEliminaSupplementi(this, Cevnt);
        }

        internal AreaSupplementi GetDatiUcSupplementi()
        {
            try
            {
                elencoSupplementiFromViewState = ViewState["elencoSupplementi"] as AreaSupplementi;

                List<DatiSupplementi> elencoSupplementiToSave = new List<DatiSupplementi>();
                for (int i = 0; i < elencoSupplementiFromViewState.ListDatiSupplementi.Count() - 1; i++)
                {

                    elencoSupplementiToSave.Add(elencoSupplementiFromViewState.ListDatiSupplementi[i]);
                }
                lstSupplementi = new AreaSupplementi();
                lstSupplementi.ListDatiSupplementi = elencoSupplementiToSave.ToArray();
                return lstSupplementi;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo GetDatiUcStatoCivile " + ex);
            }
        }

        private void GestioneTastoSalva()
        {
            if (modalitaEdit.Value == "true")
            {
                btnSalvaTabSupplementi.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
            else
            {
                btnSalvaTabSupplementi.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);
            }
        }

        #region Grid Supplementi

        protected void gvSupplementi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                //ENG - MEMO 50/2023 
                string valoreControlloMemo50_2023 = string.Empty;
                if (ViewState["AbilitazioneMemo50_2023"] != null)
                    valoreControlloMemo50_2023 = (string)ViewState["AbilitazioneMemo50_2023"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out valoreControlloMemo50_2023);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && !String.IsNullOrEmpty(valoreControlloMemo50_2023) && !String.IsNullOrEmpty(valoreControlloMemo50_2023.Trim()))
                        ViewState["AbilitazioneMemo50_2023"] = valoreControlloMemo50_2023.Trim();
                }
                bool isRicSupplementoMemo50 = false;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    setDdl(e);
                    elencoSupplementiFromViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                    List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>(elencoSupplementiFromViewState.ListDatiSupplementi);
                    int num = elencoSupplementi.Count;

                    DatiSupplementi supplemento = elencoSupplementi.ElementAt(e.Row.DataItemIndex);

                    if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" &&
                        Utility.IsRicostituzione_Supplemento(datiPensione) && !this.domanda.IsDomandaINPDAP && elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        if (supplemento.IsFromPrelievo == true && !elencoSupplementi.Exists(x => x.IsFromPrelievo == false && !string.IsNullOrEmpty(x.CodGestioneSupplemento)))
                            isRicSupplementoMemo50 = true;
                    }

                    if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && !this.domanda.IsDomandaINPDAP &&
                        Utility.IsRicostituzione_MotiviDocumentali(datiPensione) && elencoSupplementi != null && elencoSupplementi.Count() > 0)  // sola lettura
                    {
                        gvSupplementi.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblRSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).TipoSupplemento.ToString();
                        ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                        ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                        ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString();
                        ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.00") : string.Empty;
                        ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString();
                        ((Label)e.Row.FindControl("txtMontanteSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString();
                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    else
                    {
                        //prima riga
                        if (e.Row.DataItemIndex == 0)
                        {
                            //vuota
                            if (IsListaEmpty() && !Convert.ToBoolean(modalitaEdit.Value))
                            {
                                gvSupplementi.EditIndex = 0;
                                modalitaEdit.Value = "true";
                                GestioneTastoSalva();

                                gvSupplementi.DataSource = elencoSupplementi;
                                gvSupplementi.DataBind();
                            }
                            else
                            {
                                if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                                {
                                    setCampiEdit(e, supplemento.IsFromPrelievo);
                                    EnableEditableMode(e.Row.Cells[0]);
                                }
                                else
                                {
                                    ((Label)e.Row.FindControl("lblRSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).TipoSupplemento.ToString();
                                    ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                    ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                    ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString();
                                    ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.00") : string.Empty;
                                    ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString();
                                    ((Label)e.Row.FindControl("txtMontanteSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString();

                                    if (isRicSupplementoMemo50)
                                    {
                                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                        button.Enabled = false;
                                        button.Text = "&nbsp;&nbsp;&nbsp;";
                                    }
                                    else
                                        EnableReadableMode(e.Row.Cells[0], e.Row.Cells[8]);
                                }
                            }
                        }
                        else // righe successive
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                setCampiEdit(e, supplemento.IsFromPrelievo);
                                EnableEditableMode(e.Row.Cells[0]);
                            }

                            else if (e.Row.DataItemIndex == num - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblRSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).TipoSupplemento.ToString();
                                ((Label)e.Row.FindControl("lblTipoSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).CodGestioneSupplemento;
                                ((Label)e.Row.FindControl("lblDecorrenzaSupplementi")).Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)(e.Row.DataItem)).DecorrenzaSupplemento);
                                ((Label)e.Row.FindControl("lblSettimaneSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).NSettimaneSupplemento.ToString();
                                ((Label)e.Row.FindControl("lblRMSSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.00") : string.Empty;
                                ((Label)e.Row.FindControl("lblQuotaSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).QuotaSupplemento.ToString();
                                ((Label)e.Row.FindControl("txtMontanteSupplementi")).Text = ((DatiSupplementi)(e.Row.DataItem)).MontanteSupplemento.ToString();

                                if (isRicSupplementoMemo50)
                                {
                                    LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                    button.Enabled = false;
                                    button.Text = "&nbsp;&nbsp;&nbsp;";
                                }
                                else
                                    EnableReadableMode(e.Row.Cells[0], e.Row.Cells[8]);
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
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo gvSupplementi_RowDataBound " + ex);
            }
        }

        protected void gvSupplementi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            elencoSupplementiFromViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
            List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>(elencoSupplementiFromViewState.ListDatiSupplementi);
            int num = elencoSupplementi.Count;

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            //ENG - MEMO 50/2023 
            string valoreControlloMemo50_2023 = string.Empty;
            if (ViewState["AbilitazioneMemo50_2023"] != null)
                valoreControlloMemo50_2023 = (string)ViewState["AbilitazioneMemo50_2023"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out valoreControlloMemo50_2023);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloMemo50_2023) && !String.IsNullOrEmpty(valoreControlloMemo50_2023.Trim()))
                    ViewState["AbilitazioneMemo50_2023"] = valoreControlloMemo50_2023.Trim();
            }

            if (e.CommandName == "Delete")
            {
                modalitaEdit.Value = "false";

                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                //ENG - MEMO 50/2023
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.CodeTipo == "0001" && !this.domanda.IsDomandaINPDAP && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }
                elencoSupplementi.RemoveAt(r.DataItemIndex);

                if (elencoSupplementi.Count > 1)
                    gvSupplementi.EditIndex = -1;

                elencoSupplementiToViewState.ListDatiSupplementi = elencoSupplementi.ToArray();

                ViewState["elencoSupplementi"] = elencoSupplementiToViewState;
                GvSupplementi_Load();
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEdit.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                modalitaEdit.Value = "false";
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                //ENG - MEMO 50/2023
                if (ViewState["AbilitazioneMemo50_2023"] != null && (string)ViewState["AbilitazioneMemo50_2023"] == "SI" && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && datiPensione.CodeTipo == "0001" && !this.domanda.IsDomandaINPDAP && !(bool)ViewState[EnumViewState.IsTipoCalcoloModificato.ToString()])
                {
                    if (elencoSupplementi != null && elencoSupplementi.Count() > 0)
                    {
                        DatiSupplementi supplemento = elencoSupplementi.ElementAt(r.DataItemIndex);
                        if (supplemento.IsFromPrelievo)
                        {
                            this.HasError = true;
                            this.ErrorMessage = "Attenzione, non è possibile variare i dati del supplemento con il prodotto Webdom di ricostituzione per motivi contributivi di tipo ordinario. Utilizzare il prodotto Webdom di ricostituzione per variazione dati supplemento";
                            RaiseShowAvviso(this, null);
                            return;
                        }
                    }
                }

                if ((r.DataItemIndex - 1) == (num - 2))    //aggiunta riga (non si tratta di una modifica)
                {
                    DatiSupplementi supp = new DatiSupplementi();
                    if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[1].Controls[1])).SelectedValue)))
                        supp.TipoSupplemento = (((DropDownList)(r.Cells[1].Controls[1])).SelectedValue)[0];
                    if (!String.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)))
                        supp.MontanteSupplemento = Decimal.Parse(((TextBox)(r.Cells[7].Controls[1])).Text);
                    supp.CodGestioneSupplemento = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue;
                    supp.DecorrenzaSupplemento = Convert.ToDateTime(((TextBox)(r.Cells[3].Controls[1])).Text);
                    supp.NSettimaneSupplemento = Convert.ToInt32(((TextBox)(r.Cells[4].Controls[1])).Text);
                    if (!String.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)))
                        supp.RMSSupplemento = Decimal.Parse(((TextBox)(r.Cells[5].Controls[1])).Text);
                    if (!String.IsNullOrEmpty((((DropDownList)(r.Cells[6].Controls[1])).SelectedValue)))
                        supp.QuotaSupplemento = (((DropDownList)(r.Cells[6].Controls[1])).SelectedValue)[0];

                    elencoSupplementi.RemoveAt(num - 1);
                    elencoSupplementi.Add(supp);
                    DatiSupplementi newsup = new DatiSupplementi();
                    elencoSupplementi.Add(newsup);
                }
                else
                    saveValueRow(elencoSupplementi, e, r);

                gvSupplementi.EditIndex = -1;

                elencoSupplementiToViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                elencoSupplementiToViewState.ListDatiSupplementi = elencoSupplementi.ToArray();
                ViewState["elencoSupplementi"] = elencoSupplementiToViewState;
                GvSupplementi_Load();
            }
            else if (e.CommandName == "Annulla")
            {
                elencoSupplementiToViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                elencoSupplementiToViewState.ListDatiSupplementi = elencoSupplementi.ToArray();
                if (!IsListaEmpty())
                {
                    modalitaEdit.Value = "false";
                    gvSupplementi.EditIndex = -1;
                    GvSupplementi_Load();
                }
            }

            GestioneTastoSalva();
        }

        protected void gvSupplementi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvSupplementi.EditIndex = -1;

                //Bind data to the GridView control.
                GvSupplementi_Load();
                RaiseAbilitaTastoSalva(this, null);
                btnSalvaTabSupplementi.Enabled = true;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo gvSupplementi_RowCancelingEdit " + ex);
            }
        }

        protected void gvSupplementi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSupplementi.EditIndex = e.NewEditIndex;
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo gvSupplementi_RowEditing " + ex);
            }
        }

        protected void gvSupplementi_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                AreaSupplementi elencoSupplementiViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>(elencoSupplementiViewState.ListDatiSupplementi);

                if (elencoSupplementi.Count < 1)
                    inserisciSupplementi();
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo gvSupplementi_RowDeleting " + ex);
            }
        }

        protected void gvSupplementi_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                AreaSupplementi elencoSupplementiViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>(elencoSupplementiViewState.ListDatiSupplementi);
                GridViewRow row = gvSupplementi.Rows[e.RowIndex];
                if (((DropDownList)(row.Cells[2].Controls[1])).SelectedIndex != 0)
                {
                    int i = ((gvSupplementi.PageIndex * 10) + e.RowIndex);

                    if (elencoSupplementi.Count != i + 1)
                        elencoSupplementi.RemoveAt(elencoSupplementi.Count - 1);
                    gvSupplementi.EditIndex = -1;
                    ViewState["elencoSupplementi"] = elencoSupplementi.ToArray();
                    GvSupplementi_Load();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            }
        }

        protected void gvSupplementi_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSupplementi.PageIndex = e.NewPageIndex;
                GvSupplementi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo gvSupplementi_onPageIndexChanging" + ex);
            }
        }

        #endregion Grid Supplementi

        #region Private Methods Grid Supplementi

        private void setDdl(GridViewRowEventArgs e)
        {
            DropDownList ddlRSupplementi = (DropDownList)e.Row.FindControl("ddlRSupplementi");
            if (ddlRSupplementi != null)
            {
                ddlRSupplementi.Items[1].Attributes.Add("title", "Contributivo");
                ddlRSupplementi.Items[2].Attributes.Add("title", "Retributivo");
                ddlRSupplementi.Items[3].Attributes.Add("title", "Quota D");
            }

            DropDownList ddlTipoSupplementi = (DropDownList)e.Row.FindControl("ddlTipoSupplementi");
            if (ddlTipoSupplementi != null)
            {
                TipoSupplementi[] tipoSupp = ViewState["elencoTipoSupplementi"] as TipoSupplementi[];

                for (int i = 0; i < tipoSupp.Count(); i++)
                {
                    ddlTipoSupplementi.Items.Add(new ListItem(tipoSupp[i].Id, tipoSupp[i].Id));
                    ddlTipoSupplementi.Items[i + 1].Attributes.Add("title", tipoSupp[i].Descrizione);
                }
            }

            DropDownList ddlQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlQuotaSupplementi");
            if (ddlQuotaSupplementi != null)
            {
                ddlQuotaSupplementi.Items[1].Attributes.Add("title", "A");
                ddlQuotaSupplementi.Items[2].Attributes.Add("title", "B");
            }
        }

        private void setCampiEdit(GridViewRowEventArgs e, bool IsFromPrelievo)
        {
            DropDownList ddlRSupplementi = new DropDownList();
            ddlRSupplementi = (DropDownList)e.Row.FindControl("ddlRSupplementi");
            DropDownList ddlQuotaSupplementi = new DropDownList();
            ddlQuotaSupplementi = (DropDownList)e.Row.FindControl("ddlQuotaSupplementi");
            DropDownList ddlTipoSupplementi = new DropDownList();
            ddlTipoSupplementi = (DropDownList)e.Row.FindControl("ddlTipoSupplementi");
            TextBox txtDecorrenzaSupplementi = (TextBox)e.Row.FindControl("txtDecorrenzaSupplementi");
            TextBox txtSettimaneSupplementi = (TextBox)e.Row.FindControl("txtSettimaneSupplementi");
            TextBox txtRMSSupplementi = (TextBox)e.Row.FindControl("txtRMSSupplementi");
            TextBox txtMontanteSupplementi = (TextBox)e.Row.FindControl("txtMontanteSupplementi");
            ddlRSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).TipoSupplemento.ToString();
            if (((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento != null)
                ddlTipoSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).CodGestioneSupplemento;
            ddlQuotaSupplementi.SelectedValue = ((DatiSupplementi)e.Row.DataItem).QuotaSupplemento.ToString();
            txtDecorrenzaSupplementi.Text = String.Format("{0:MM/yyyy}", ((DatiSupplementi)e.Row.DataItem).DecorrenzaSupplemento);
            txtSettimaneSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).NSettimaneSupplemento.ToString();
            txtRMSSupplementi.Text = ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.HasValue ? ((DatiSupplementi)(e.Row.DataItem)).RMSSupplemento.Value.ToString("0.00") : string.Empty;
            txtMontanteSupplementi.Text = ((DatiSupplementi)e.Row.DataItem).MontanteSupplemento.ToString();
            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            CustomValidator requiredddlTipoSupplementi = (CustomValidator)e.Row.FindControl("requiredddlTipoSupplementi");
            requiredddlTipoSupplementi.Enabled = false;
            if (this.domanda != null && this.domanda.Tipofondo.HasValue && this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET)
            {

                if (CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datipensione) || Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datipensione))
                {
                    CustomValidator requiredddlQuotaSupplementi = (CustomValidator)e.Row.FindControl("requiredddlQuotaSupplementi");
                    requiredddlQuotaSupplementi.Enabled = false;
                    if (IsFromPrelievo && Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datipensione))
                    {
                        ddlRSupplementi.Enabled = false;
                        ddlTipoSupplementi.Enabled = false;
                        ddlQuotaSupplementi.Enabled = false;
                        txtDecorrenzaSupplementi.Enabled = false;
                        txtSettimaneSupplementi.Enabled = false;
                        txtRMSSupplementi.Enabled = false;
                        txtMontanteSupplementi.Enabled = false;
                    }
                }
            }

            //ENG - MEMO 50/2023
            if ((this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT) &&
                Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datipensione))
            {
                ddlTipoSupplementi.SelectedValue = "1";
                ddlTipoSupplementi.Enabled = false;
            }

            if (Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datipensione) && !IsFromPrelievo)
            {
                ddlRSupplementi.SelectedValue = "R";
                ddlRSupplementi.Enabled = false;
                ddlTipoSupplementi.SelectedValue = "1";
                ddlTipoSupplementi.Enabled = false;
                ddlQuotaSupplementi.SelectedValue = "A";
                ddlQuotaSupplementi.Enabled = false;
            }

            if (this.domanda.Categoria != null && this.domanda.Categoria.Trim().EndsWith("PM"))
            {
                DateTime decorrenzaPensione = new DateTime();
                if (this.risposta != null && this.risposta.DecorrenzaPensioneDanteCausa.HasValue)
                {
                    decorrenzaPensione = this.risposta.DecorrenzaPensioneDanteCausa.Value;
                }
                else if (datipensione.DecorrenzaOriginaria.HasValue)
                {
                    decorrenzaPensione = datipensione.DecorrenzaOriginaria.Value;
                }
                if (decorrenzaPensione != null && Utility.DataSuccessivaA(decorrenzaPensione, new DateTime(1989, 10, 1)))
                {
                    ddlTipoSupplementi.SelectedValue = "M";
                    ddlTipoSupplementi.Enabled = false;
                    requiredddlTipoSupplementi.Enabled = false;

                }
                else if (decorrenzaPensione != null && Utility.DataSuccessivaA(new DateTime(1980, 01, 1), decorrenzaPensione))
                {
                    ddlTipoSupplementi.SelectedValue = "1";
                    ddlTipoSupplementi.Enabled = false;
                    requiredddlTipoSupplementi.Enabled = false;

                }
                else
                {
                    ddlTipoSupplementi.Items.Clear();
                    ddlTipoSupplementi.Items.Add(new ListItem("1", "1"));
                    ddlTipoSupplementi.Items.Add(new ListItem("M", "M"));
                    ddlTipoSupplementi.Enabled = true;
                    requiredddlTipoSupplementi.Enabled = false;
                }


            }

            ddlTipoSupplementi_SelectedIndexChanged(ddlTipoSupplementi, e);
        }

        private void saveValueRow(List<DatiSupplementi> elencoSupplementi, GridViewCommandEventArgs e, GridViewRow r)
        {
            elencoSupplementi[r.DataItemIndex].TipoSupplemento = !String.IsNullOrEmpty((((DropDownList)(r.Cells[1].Controls[1])).SelectedValue)) ? (((DropDownList)(r.Cells[1].Controls[1])).SelectedValue)[0] : (char?)null;
            elencoSupplementi[r.DataItemIndex].CodGestioneSupplemento = !String.IsNullOrEmpty((((DropDownList)(r.Cells[2].Controls[1])).SelectedValue)) ? ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue : string.Empty;
            elencoSupplementi[r.DataItemIndex].DecorrenzaSupplemento = !String.IsNullOrEmpty((((TextBox)(r.Cells[3].Controls[1])).Text)) ? Convert.ToDateTime(((TextBox)(r.Cells[3].Controls[1])).Text) : (DateTime?)null;
            elencoSupplementi[r.DataItemIndex].NSettimaneSupplemento = !String.IsNullOrEmpty((((TextBox)(r.Cells[4].Controls[1])).Text)) ? Convert.ToInt32(((TextBox)(r.Cells[4].Controls[1])).Text) : (int?)null;
            elencoSupplementi[r.DataItemIndex].RMSSupplemento = !String.IsNullOrEmpty((((TextBox)(r.Cells[5].Controls[1])).Text)) ? Decimal.Parse(((TextBox)(r.Cells[5].Controls[1])).Text) : (decimal?)null;
            elencoSupplementi[r.DataItemIndex].QuotaSupplemento = !String.IsNullOrEmpty((((DropDownList)(r.Cells[6].Controls[1])).SelectedValue)) ? (((DropDownList)(r.Cells[6].Controls[1])).SelectedValue)[0] : (char?)null;
            elencoSupplementi[r.DataItemIndex].MontanteSupplemento = !String.IsNullOrEmpty((((TextBox)(r.Cells[7].Controls[1])).Text)) ? Decimal.Parse(((TextBox)(r.Cells[7].Controls[1])).Text) : (decimal?)null;
        }

        private void inserisciSupplementi()
        {
            try
            {
                elencoSupplementiFromViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                DatiSupplementi supp = new DatiSupplementi();
                elencoSupplementiFromViewState.ListDatiSupplementi[elencoSupplementiFromViewState.ListDatiSupplementi.Count()] = supp;
                ViewState["elencoSupplementi"] = elencoSupplementiFromViewState;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo InserisciSupplementi " + ex);
            }
        }

        private bool IsListaEmpty()
        {
            elencoSupplementiFromViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
            List<DatiSupplementi> listaDatiSuppl = new List<DatiSupplementi>(elencoSupplementiFromViewState.ListDatiSupplementi);

            if (listaDatiSuppl.Count == 1 && string.IsNullOrEmpty(listaDatiSuppl[0].CodGestioneSupplemento) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].DecorrenzaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].MontanteSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].QuotaSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].RMSSupplemento.ToString()) &&
                string.IsNullOrEmpty(listaDatiSuppl[0].TipoSupplemento.ToString()) && string.IsNullOrEmpty(listaDatiSuppl[0].NSettimaneSupplemento.ToString()))
                return true;
            else
                return false;
        }

        private void EnableEditableMode(TableCell cell_CancelSave)
        {
            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            save.ValidationGroup = "UCTabSupplementi";

        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl("txtDecorrenzaSupplementi") != null && ((TextBox)row.FindControl("txtDecorrenzaSupplementi")).Text != string.Empty &&
                row.FindControl("txtSettimaneSupplementi") != null && ((TextBox)row.FindControl("txtSettimaneSupplementi")).Text != string.Empty &&
                row.FindControl("txtRMSSupplementi") != null && ((TextBox)row.FindControl("txtRMSSupplementi")).Text != string.Empty &&
                row.FindControl("txtMontanteSupplementi") != null && ((TextBox)row.FindControl("txtMontanteSupplementi")).Text != string.Empty &&
                row.FindControl("ddlRSupplementi") != null && ((DropDownList)row.FindControl("ddlRSupplementi")).SelectedIndex != 0 &&
                row.FindControl("ddlTipoSupplementi") != null && ((DropDownList)row.FindControl("ddlTipoSupplementi")).SelectedIndex != 0 &&
                row.FindControl("ddlQuotaSupplementi") != null && ((DropDownList)row.FindControl("ddlQuotaSupplementi")).SelectedIndex != 0)
                return false;
            else
                return true;

        }

        private bool IsEmptyReadableRow(GridViewRow row)
        {
            if (row.FindControl("lblRSupplementi") != null && ((TextBox)row.FindControl("lblRSupplementi")).Text != string.Empty &&
                row.FindControl("lblTipoSupplementi") != null && ((TextBox)row.FindControl("lblTipoSupplementi")).Text != string.Empty &&
                row.FindControl("lblDecorrenzaSupplementi") != null && ((TextBox)row.FindControl("lblDecorrenzaSupplementi")).Text != string.Empty &&
                row.FindControl("lblSettimaneSupplementi") != null && ((TextBox)row.FindControl("lblSettimaneSupplementi")).Text != string.Empty &&
                row.FindControl("lblRMSSupplementi") != null && ((DropDownList)row.FindControl("lblRMSSupplementi")).SelectedIndex != 0 &&
                row.FindControl("lblQuotaSupplementi") != null && ((DropDownList)row.FindControl("lblQuotaSupplementi")).SelectedIndex != 0 &&
                row.FindControl("txtMontanteSupplementi") != null && ((DropDownList)row.FindControl("txtMontanteSupplementi")).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDelete")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private void GvSupplementi_Load()
        {
            try
            {
                elencoSupplementiFromViewState = ViewState["elencoSupplementi"] as AreaSupplementi;
                List<DatiSupplementi> elencoSupplementi = new List<DatiSupplementi>(elencoSupplementiFromViewState.ListDatiSupplementi);
                gvSupplementi.DataSource = elencoSupplementi;
                gvSupplementi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo GvSupplementi_Load " + ex);
            }
        }

        private void GestioneRic()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datipensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            if (datipensione != null && (datipensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione || this.domanda.IsDomandaRiapertura))
            {
                if (datipensione.CodeProdotto != "0107" && datipensione.CodeProdotto != "0102" &&
                    datipensione.CodeProdotto != "0307" && datipensione.CodeProdotto != "0302" &&
                    datipensione.CodeProdotto != "0407" && datipensione.CodeProdotto != "0402" &&
                    !Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datipensione) &&
                    !this.domanda.IsDomandaRiapertura)
                    CodeUtility.BloccaForm((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], this);
            }
        }

        #endregion Private Methods Grid Supplementi

        public event Utility.CustomEventHandler SalvaSupplementi;
        public event Utility.CustomEventHandler EliminaSupplementi;
        public event EventHandler ErrorSalvaSupplementi;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler GetDecorrenzaPensione;
        public event EventHandler ShowAvviso;

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

        protected void RaiseSalvaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            if (SalvaSupplementi != null)
                SalvaSupplementi(sender, e);
        }

        protected void RaiseEliminaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            if (EliminaSupplementi != null)
                EliminaSupplementi(sender, e);
        }

        protected void RaiseErrorSalvaSupplementi(object sender, EventArgs e)
        {
            if (ErrorSalvaSupplementi != null)
                ErrorSalvaSupplementi(sender, e);
        }

        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void ddlTipoSupplementi_SelectedIndexChanged(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            if (Utility.IsRicostituzione_Supplemento(datiPensione) || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione))
            {
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.NamingContainer;

                string selectedValue = ddl.SelectedValue;

                // CustomValidator riga
                CustomValidator requiredddlQuotaSupplementi = (CustomValidator)row.FindControl("requiredddlQuotaSupplementi");

                if (requiredddlQuotaSupplementi != null)
                {
                    if (selectedValue == "2" || selectedValue == "3" || selectedValue == "4")
                        requiredddlQuotaSupplementi.Enabled = false;
                    else
                        requiredddlQuotaSupplementi.Enabled = true;
                }

            }
        }

        #region enum
        enum EnumViewState
        {
            IsTipoCalcoloModificato
        }
        #endregion enum
    }
}

