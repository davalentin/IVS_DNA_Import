using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract.AggiornaCalcoloNoInd;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.InterfacceViews;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class AggiornaCalcoloNoInd : CustomBasePage, IInfoPostCalcolo, IQuadriSemafori, IAggiornaCalcoloNoInd
    {
        PresenterAggiornaCalcoloNoInd Presenter;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IInfoPostCalcolo
        public AreaTitolare.DatiPensione datiPensione { get; set; }

        public AreaEsito areaEsito { get; set; }

        public string statoPensione { get; set; }
        public AreaQuadri areaQuadri { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        public List<CausaleDebito> CausaliDebito { get; set; }
        public List<CausaleDtoLite> LegendaCausaliAmmesse { get; set; }
        public RootIndebitoDto Indebito { get; set; }
        #endregion IInfoPostCalcolo

        #region Componenti Grafiche
        public string MessaggioCodaPannelloValutazioneEventualeScelta { get; set; }
        #endregion


        public bool? BloccoValidazioneCausali
        {
            get
            {
                // lettura da ViewState (può essere null)
                bool? value = ViewState["BloccoValidazioneCausali"] as bool?;

                if (value == null)
                {
                    // 1) calcolo lazy
                    value = Presenter.ControllaBloccoValidazioneCausali();

                    // 2) persisto per i postback successivi
                    ViewState["BloccoValidazioneCausali"] = value;
                }

                return value;
            }
            set
            {
                ViewState["BloccoValidazioneCausali"] = value;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Presenter = new PresenterAggiornaCalcoloNoInd(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Presenter.PageLoadNoPostBack();
            }
        }

        public void MostraValutazioneEventualeScelta()
        {
            CaricaDomandaDaSessione();

            this.lblEsito.Text = AreaEsito.TipoEsito.OK.ToString();
            this.lblDettaglio.Text = "Calcolo eseguito correttamente";
            this.lblCertificatoValore.Text = domanda.Certificato;

            this.LblMessaggioCodaPannelloValutazioneEventualeScelta.InnerHtml = MessaggioCodaPannelloValutazioneEventualeScelta;

            bool? BloccoValidazioneCausali = this.BloccoValidazioneCausali;
            if (BloccoValidazioneCausali != null && (bool)BloccoValidazioneCausali)
            {
                this.LblMessaggioCodaPannelloValutazioneEventualeScelta.Attributes.CssStyle.Add("color", "red");
                btnProseguiValidazione.Enabled = false;
            }

            pnlTabValutazioneEventualeScelta.Visible = true;
            valutazione_eventuale_scelta.Visible = true;

            pnlTabElencoCasualiDebito.Visible = false;
            elenco_casuali_debito.Visible = false;
        }

        private void BindCasualiDebito()
        {
            CaricaCausaliDabitoDaSessione();
            LeggiLegendaCausaliAmmesseSessione();
            casualiDebito.DataSource = CausaliDebito;
            gvLegendaCausali.DataSource = LegendaCausaliAmmesse;
            casualiDebito.DataBind();
            gvLegendaCausali.DataBind();
        }

        public void MostraElencoCasualiDebito()
        {
            CaricaDomandaDaSessione();

            Presenter.CaricaCasuali();
            ScriviIndebitoSessione(Indebito);

            LeggiIndebitoDaSessione();
            CaricaDomandaDaSessione();
            LeggiDatiPensioneDaSessione();

            //Il presenter torna True se il blocco deve essere applicato, da qui le assegnazioni seguenti
            bool BloccoValidazioneCausali = (bool)this.BloccoValidazioneCausali;
            this.btnValidaCasuali.Enabled = !BloccoValidazioneCausali;
            this.lblPulsanteValidaCausaliDisabilitato.Visible = BloccoValidazioneCausali;
            this.lblElencoCasualiMessaggioCoda.Visible = !BloccoValidazioneCausali;

            if (!Indebito.Success)
            {
                GestioneErroriRecuperoDatiIndebito();
                return;
            }

            List<CausaleDebito> lista = Presenter.EstraiCasualiDebito();
            ScriviCausaliDebitoSessione(lista);
            ScriviLegendaCausaliAmmesseSessione(LegendaCausaliAmmesse);
            BindCasualiDebito();

            this.txtDal.Text = Indebito.Data.DataInizioDebito.ToString("dd/MM/yyyy");
            this.txtAl.Text = Indebito.Data.DataFineDebito.ToString("dd/MM/yyyy");

            ValorizzaRisultatiCalcoloPannelloElencoCausali();

            pnlTabValutazioneEventualeScelta.Visible = false;
            valutazione_eventuale_scelta.Visible = false;
            pnlTabElencoCasualiDebito.Visible = true;
            elenco_casuali_debito.Visible = true;
        }

        private void GestioneErroriRecuperoDatiIndebito()
        {
            LeggiIndebitoDaSessione();
            CaricaDomandaDaSessione();

            if(Presenter.ControlloErroreCausatoBloccoIvs())
            {
                this.txtDal.Text = string.Empty;
                this.txtAl.Text = string.Empty;

                ValorizzaRisultatiCalcoloPannelloElencoCausali();

                this.lblResultCalcoloElencoCausaliDebito.Text = "Attenzione, per il debito da 'Ricostituzione Online' indicato non è previsto accodo del TE08IND e rispettiva selezione delle causali per motivi ostativi stabiliti dal calcolo";
                this.lblElencoCasualiMessaggioCoda.InnerText = "Per questa posizione è possibile solamente accogliere direttamente la domanda e consultare l’usuale TE08.";

                List<CausaleDebito> lista = new List<CausaleDebito>();
                ScriviCausaliDebitoSessione(lista);
                BindCasualiDebito();

                casualiDebito.AutoGenerateEditButton = false;
                casualiDebito.AutoGenerateDeleteButton = false;
                casualiDebito.AutoGenerateSelectButton = false;

                btnValidaCasuali.Visible = false;

                pnlTabValutazioneEventualeScelta.Visible = false;
                valutazione_eventuale_scelta.Visible = false;
                pnlTabElencoCasualiDebito.Visible = true;
                elenco_casuali_debito.Visible = true;
            } else
            {
                this.txtDal.Text = string.Empty;
                this.txtAl.Text = string.Empty;

                ValorizzaRisultatiCalcoloPannelloElencoCausali();

                pnlTabValutazioneEventualeScelta.Visible = false;
                valutazione_eventuale_scelta.Visible = false;
                pnlTabElencoCasualiDebito.Visible = true;
                elenco_casuali_debito.Visible = true;

                divResultElencoCasualiDebito.Visible = false;
                elencoCasualiMessaggioCoda.Visible = false;
                elencoCasualiPeiodoIndebito.Visible = false;
                tabellaCasualiDebito.Visible = false;
                divRisultatoElencoCasuali.Visible = false;
                btnValidaCasuali.Visible = false;
                btnAccogliDomandaElencoCasuali.Visible = false;
                ucAvviso.Messaggio = "Errore nel recupero delle informazioni inerenti l'indebito da servizio esterno";
                ucAvviso.Visible = true;
            }
        }

        //Click per il passaggio da CALCOLO NO INDEB WAIT a CALCOLO NO INDEB
        protected void btnProseguiValidazione_Click(object sender, EventArgs e)
        {
            Presenter.CambiaStatoDomanda();
        }

        //Click per accogliere la domanda (metodo NotificaTE08) nello stato CALCOLO NO INDEB WAIT
        protected void btnAccogliDomanda_Click(object sender, EventArgs e)
        {
            Presenter.AccogliDomanda();
        }

        //Metodi per gestire la visualizzazione dei risultati del evento "Click_AccogliDomanda
        //nello stato domanda CALCOLO NO INDEB WAIT
        public void AccogliDomandaMostraEsitoPositivo()
        {
            this.imgIcon.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/ok.png";
            this.lblMsg.Text = "La domanda è stata accolta. Non sono state validate " +
                "le causali. Il TE08 è consultabile in Stampeweb. Gestire l'indebito in procedura \"RI\"";
            this.divRisultatoValutazioneEventualeScelta.Visible = true;
            this.LblMessaggioCodaPannelloValutazioneEventualeScelta.Visible = false;

            btnProseguiValidazione.Visible = false;
            btnAccogliDomanda.Visible = false;
        }

        public void AccogliDomandaMostraEsitoNegativo()
        {
            this.imgIcon.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/alert.png";
            this.lblMsg.Text = "Si è generato un errore in fase di notifica, " +
                "si prega di riprovare";
            this.divRisultatoValutazioneEventualeScelta.Visible = true;
        }

        //Click per Accogliere la domanda (metodo NotificaTE08) nello stato CALCOLO NO INDEB
        protected void btnAccogliDomandaElencoCasuali_Click(object sender, EventArgs e)
        {
            Presenter.AccogliDomanda();
        }

        //Metodi per gestire la visualizzazione dei risultati del evento "Click_AccogliDomanda
        //nello stato domanda CALCOLO NO INDEB
        public void ElencoCausaliMetodoAccogliDomandaVisualizzazioneEsitoPositivo()
        {
            this.imgElencoCasuali.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/ok.png";
            this.lblRisultatoElencoCasuali.Text = "La domanda è stata accolta. Non sono state validate le " +
                "causali. Il TE08 è consultabile in StampeWeb. Gestire l'indebito in procedura \"RI\".";
            this.divRisultatoElencoCasuali.Visible = true;
            this.lblResultCalcoloElencoCausaliDebito.Visible = false;
            this.elencoCasualiMessaggioCoda.Visible = false;
            this.elencoCasualiPeiodoIndebito.Visible = false;
            this.tabellaCasualiDebito.Visible = false;
            this.lblPulsanteValidaCausaliDisabilitato.Visible = false;

            btnValidaCasuali.Visible = false;
            btnAccogliDomandaElencoCasuali.Visible = false;
        }

        public void ElencoCausaliMetodoAccogliDomandaVisualizzazioneEsitoNegativo()
        {
            this.imgElencoCasuali.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/alert.png";
            this.lblRisultatoElencoCasuali.Text = "Si è generato un errore in fase di notifica, " +
                "si prega di riprovare";
            this.divRisultatoElencoCasuali.Visible = true;
        }

        //Click per validare le casuali (metodo AggiornaCasuali) nello stato CALCOLO NO INDEB
        protected void btnValidaCasuali_Click(object sender, EventArgs e)
        {
            Presenter.ValidaCausali();
        }

        public void ValidaCausaliMostraEsitoPositivo()
        {
            this.imgElencoCasuali.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/ok.png";
            this.lblRisultatoElencoCasuali.Text = "La domanda è stata accolta. Le causali sono " +
                "state inserite nel TE08/Ind consultabile in Stampeweb. In automatico saranno " +
                "individuate le modalità di recupero e la pratica di indebito sarà caricata in RI";
            this.divRisultatoElencoCasuali.Visible = true;
            this.lblPulsanteValidaCausaliDisabilitato.Visible = false;

            Presenter.AggiornaSemafori();
            btnValidaCasuali.Visible = false;
            btnAccogliDomandaElencoCasuali.Visible = false;
        }

        public void ValidaCausaliMostraEsitoNegativo()
        {
            this.imgElencoCasuali.ImageUrl = "../App_Themes/" + Page.Theme + "/Images/alert.png";
            this.lblRisultatoElencoCasuali.Text = "Si è generato un errore in fase di notifica, " +
                "si prega di riprovare";
            this.divRisultatoElencoCasuali.Visible = true;
        }

        protected void gvCasualiIndebito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) 
                return; 
            var causaleDebito = e.Row.DataItem as CausaleDebito; 
            if (causaleDebito == null) 
                return; 
            bool isEdit = (e.Row.RowState & DataControlRowState.Edit) > 0; 
            // INDICE DELLA COLONNA "Causale Analitica"
            int colCausaleAnalitica = 2; 
            //Modalità EDIT
            if (isEdit) { 
                e.Row.Cells[colCausaleAnalitica].Attributes.Remove("title");
                DropDownList ddl = (DropDownList)e.Row.FindControl("ddlCausaleAnalitica"); 
                if (ddl != null) { 
                    CausaleDtoLite[] casualiAmmesse = causaleDebito.CasualiAmmesse; 
                    ddl.DataSource = casualiAmmesse.Select(c => c.Analitica).ToList(); 
                    ddl.DataBind(); 
                    ddl.Items.Insert(0, new ListItem("- Seleziona -", "")); 
                    string valore = causaleDebito.CausaleAnalitica.ToString(); 
                    if (ddl.Items.FindByValue(valore) != null) 
                        ddl.SelectedValue = valore; 
                    foreach (ListItem li in ddl.Items) { 
                        if (li.Value == null || li.Value.Trim().Length == 0) 
                            continue; 
                        int analitica = int.Parse(li.Value); 
                        CausaleDtoLite causale = casualiAmmesse.FirstOrDefault(c => c.Analitica == analitica); 
                        if (causale != null) 
                            li.Attributes["title"] = causale.Descrizione; 
                    } 
                } 
                // Pulsante CANCEL
                LinkButton cancel = (LinkButton)e.Row.Cells[0].Controls[2]; 
                if (cancel != null) { 
                    cancel.Text = string.Format( "<img width=16 height=16 border=0 src=../App_themes/{0}/Images/cancel24.png />", Theme ); 
                    cancel.ToolTip = "Annulla"; 
                    cancel.Attributes["style"] = "text-decoration:none;"; 
                } 
                // Pulsante SAVE
                LinkButton save = (LinkButton)e.Row.Cells[0].Controls[0]; 
                if (save != null) { 
                    save.Text = string.Format( "<img width=16 height=16 border=0 src=../App_themes/{0}/Images/save24.png />", Theme ); 
                    save.ToolTip = "Salva"; 
                    save.Attributes["style"] = "text-decoration:none;"; 
                } 
                return; 
            } 
            //Modalità VISUALIZZAZIONE
            string descrizione = causaleDebito.Descrizione; 
            if (descrizione != null && descrizione.Trim().Length > 0) 
                e.Row.Cells[colCausaleAnalitica].Attributes["title"] = descrizione;
            else
                e.Row.Cells[colCausaleAnalitica].Attributes["title"] = "! Descrizione Non Disponibile !";
            // Pulsante EDIT
            LinkButton editButton = (LinkButton)e.Row.Cells[0].Controls[0]; 
            if (editButton != null) { 
                editButton.Text = string.Format( "<img width=20 height=20 border=0 src=../App_themes/{0}/Images/pencil.png />", Page.Theme ); 
                editButton.ToolTip = "Modifica"; 
                editButton.Attributes["style"] = "text-decoration:none;";
                editButton.Visible = !(bool)BloccoValidazioneCausali;
            }
        }

        protected void gvCasualiIndebito_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (btnValidaCasuali.Visible == false)
            {
                e.Cancel = true;
                return;
            }

            btnAccogliDomandaElencoCasuali.Enabled = false;
            btnValidaCasuali.Enabled = false;

            casualiDebito.EditIndex = e.NewEditIndex;
            BindCasualiDebito(); // rebinda solo la griglia
        }

        protected void gvCasualiIndebito_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            btnAccogliDomandaElencoCasuali.Enabled = true;
            btnValidaCasuali.Enabled = (bool)!BloccoValidazioneCausali;

            casualiDebito.EditIndex = -1;
            BindCasualiDebito(); // rebinda solo la griglia
        }

        protected void gvCasualiIndebito_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int rowIndex = e.RowIndex;

            // Recupero l'ID della riga dal DataKey
            int id = (int)casualiDebito.DataKeys[rowIndex].Value;

            CaricaCausaliDabitoDaSessione();

            CausaleDebito item = CausaliDebito.FirstOrDefault(c => c.Id == id);

            // Recupero il DropDownList dalla riga
            GridViewRow row = casualiDebito.Rows[rowIndex];
            DropDownList ddl = row.FindControl("ddlCausaleAnalitica") as DropDownList;

            if (item != null)
            {
                if (ddl != null)
                {
                    string nuovaAnalitica = ddl.SelectedValue;

                    if (!string.IsNullOrEmpty(nuovaAnalitica))
                    {
                        int nuovoCodice = int.Parse(nuovaAnalitica);
                        item.CausaleAnalitica = nuovoCodice;
                        if (item.CasualiAmmesse != null)
                        {
                            var nuovaCausale = item.CasualiAmmesse.FirstOrDefault(c => c.Analitica == nuovoCodice);
                            if (nuovaCausale != null)
                                item.Descrizione = nuovaCausale.Descrizione;
                            else item.Descrizione = string.Empty;
                        }
                    }
                    else
                    {
                        item.CausaleAnalitica = 0;
                        item.Descrizione = string.Empty;
                    }

                    ScriviCausaliDebitoSessione(CausaliDebito);
                }
            }

            // Esco dalla modalità edit
            casualiDebito.EditIndex = -1;

            // Riabilito i pulsanti abilitazione
            btnAccogliDomandaElencoCasuali.Enabled = true;
            btnValidaCasuali.Enabled = (bool)!BloccoValidazioneCausali;

            // Ricarico la griglia aggiornata
            BindCasualiDebito();
        }

        public void LeggiInfoLiquidazione()
        {
            this.InfoLiquidazione = this.ValorizzaInfoLiquidazione(ucInfoLiquidazione);
        }

        public void CaricaDomandaDaSessione()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["domanda"];
        }

        public void CaricaCausaliDabitoDaSessione()
        {
            CausaliDebito = (List<CausaleDebito>)Session["CasualiDebito"];
        }

        public void LeggiIndebitoDaSessione()
        {
            if (Indebito == null)
                Indebito = (RootIndebitoDto)Session["Indebito"];
        }

        public void LeggiDatiPensioneDaSessione()
        {
            if (datiPensione == null)
                datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
        }

        public void LeggiLegendaCausaliAmmesseSessione()
        {
            if (LegendaCausaliAmmesse == null)
                LegendaCausaliAmmesse = (List<CausaleDtoLite>)Session["LegendaCausaliAmmesse"];
        }

        public void ScriviCausaliDebitoSessione(List<CausaleDebito> CausaliDebito)
        {
            Session["CasualiDebito"] = CausaliDebito;
        }

        public void ScriviIndebitoSessione(RootIndebitoDto Indebito)
        {
            Session["Indebito"] = Indebito;
        }

        public void ScriviDomandaSessione(AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda)
        {
            Session["domanda"] = domanda;
        }

        public void ScriviLegendaCausaliAmmesseSessione(List<CausaleDtoLite> legenda)
        {
            Session["LegendaCausaliAmmesse"] = legenda;
        }

        public void PreparaAreaInfoPratica()
        {
            this.areaInfoPratica = new AreaInfoPratica
            {
                ElencoTab = new AreaQuadri.Tab[] { }
            };
        }

        public void ApplicaSemaforiUI()
        {
            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        public void ValorizzaRisultatiCalcoloPannelloElencoCausali()
        {
            this.elencoCasualiDebito_EsitoCalcolo.Text = AreaEsito.TipoEsito.OK.ToString();
            this.elencoCasualiDebito_DettaglioEsito.Text = "Calcolo eseguito correttamente";
            this.elencoCasualiDebito_Certificato.Text = domanda.Certificato;
        }

        public void MostraAvviso(string Message)
        {
            ucAvviso.Messaggio = Message;
            ucAvviso.Visible = true;
        }

        public void NascondiAvviso()
        {
            ucAvviso.Visible = false;
        }
    }
}
