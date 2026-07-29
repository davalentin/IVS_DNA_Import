using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using System.Globalization;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore
{
    public partial class UCDelegato : CustomBaseUserControl, IDelegatoTutore, IRicercaPosizione, ITitolarePensione
    {

        #region IElaborazionePosizione
        public RicercaPosizione RicercaPosizione { get; set; }
        public RicercaPosizione RicercaDanteCausa { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> ElencoDomande { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> ElencoPensioni { get; set; }
        public List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> ElencoSinonimi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica RiepilogoAnagrafica { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiEsitoCalcolo EsitoCalcolo { get; set; }
        public Presenter.SvrLiquidazione.AreaEsito.TipoEsito Esito { get; set; }
        public UtilityTipoAppartenenza TipoAppRuolo { get; set; }
        public UtilityRuolo Ruolo { get; set; }
        public bool IsDomandaDB { get; set; }
        public bool IsPaginaConferma { get; set; }
        public bool IsDomandaCalcolataProvvisoria { get; set; }
        public bool IsConsultazione { get; set; }
        public string SedeDiversa { get; set; }
        public bool IsRicercaManualeDA { get; set; }
        public bool IsNuovoCertificatoGeneratoEnpals { get; set; }
        //ENG - Pensioni Ovunque: gestione nuovo pannello
        public bool MostraPanelloMessBloccantePensioniOvunque { get; set; }
        public string SedePensioneGP1ALZ6 { get; set; }
        public string CodCategoriaPensione { get; set; }
        public string CertificatoInseguimentoPensione { get; set; }
        //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
        public bool IsPaginaVisualizzazioneStatoPratiche { get; set; }
        //ENG - Gestione Popup Memo 239
        public bool MostraPopupMemo239 { get; set; }
        //ENG - Gestione Popup Memo 31/2023
        public bool MostraPopupMemo312023 { get; set; }
        #endregion IElaborazionePosizione

        #region IDelegatoTutore
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo delegato { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo tutore { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDelegatoTutore

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        private String m_strSortExp;
        private SortDirection m_SortDirection;


        protected void Page_Load(object sender, EventArgs e)
        {
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            if (!Page.IsPostBack)
            {
                try
                {
                    List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiDelegato"];
                    m_strSortExp = String.Empty;
                    gvSinonimiDelegato_Load(Sinonimi);
                    if (ddlCodiceDelegato.Items.Count == 0)
                        Load_ddlCodiceDelegato();

                    AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    this.domanda = Domanda;
                    PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
                    presenterDelegatoTutore.IsNotDelegatoPresent(this);

                    if (!this.HasError == true)
                    {
                        btnEliminaTabDelegato.Enabled = false;
                    }
                }
                catch (DnaExceptionBase)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo PensioniGridView_onRowDataBound" + ex);
                }

            }
            AbilitaPannelli();
            RaiseGestisciDisabilitazioneTabDelegato(this, null);

            btnRicerca1Delegato.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/search24.png";
            btnRicerca2Delegato.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/search24.png";
        }

        private void Load_ddlCodiceDelegato()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            ListItem li = new ListItem();
            ddlCodiceDelegato.Items.Add(li);
            foreach (AreaDecodifica.DatiDelegato code in valoriDecodificati.ElencoDelegato)
            {
                li = new ListItem(code.Descrizione, code.Id);
                ddlCodiceDelegato.Items.Add(li);
            }
        }

        public void AbilitaPannelli()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(titolare.Pensione.CodeGruppo, titolare.Pensione.CodeProdotto, titolare.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (this.TipoAppRuolo == UtilityTipoAppartenenza.AGO && (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura))
            {
                pnlDelegato.Enabled = false;
            }
            else
            {
                radioAnagraficaDelegato.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
                radioAnagraficaDelegato.InputAttributes.Add("EnableClass", "onClassAnagraficaDelegato");
                radioCodiceFiscaleDelegato.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
                radioCodiceFiscaleDelegato.InputAttributes.Add("EnableClass", "onClassCodiceFiscaleDelegato");
                divTxtCodiceFiscaleDelegato.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
                divTxtCodiceFiscaleDelegato.Attributes.Add("EnableClass", "onClassCodiceFiscaleDelegato");
                divTxtCognomeDelegato.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
                divTxtCognomeDelegato.Attributes.Add("EnableClass", "onClassAnagraficaDelegato");
            }
        }

        //ENG - Reversibilita 024
        public void DisabilitaPannelli()
        {
            radioAnagraficaDelegato.Attributes.Remove("onclick");
            radioAnagraficaDelegato.InputAttributes.Remove("EnableClass");
            radioCodiceFiscaleDelegato.Attributes.Remove("onclick");
            radioCodiceFiscaleDelegato.InputAttributes.Remove("Ena/bleClass");
            divTxtCodiceFiscaleDelegato.Attributes.Remove("onclick");
            divTxtCodiceFiscaleDelegato.Attributes.Remove("EnableClass");
            divTxtCognomeDelegato.Attributes.Remove("onclick");
            divTxtCognomeDelegato.Attributes.Remove("EnableClass");
        }


        internal void ValorizzaEtichetteDelegato(IDelegatoTutore DelegatoTutore)
        {
            ViewState["Delegato"] = DelegatoTutore.delegato.AnagraficaTitolare;
            try
            {
                if (ddlCodiceDelegato.Items.Count == 0)
                    Load_ddlCodiceDelegato();

                lblCFDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.CodiceFiscale;
                hdnCodiceFiscaleDelegato.Value = DelegatoTutore.delegato.AnagraficaTitolare.CodiceFiscale;
                lblCognomeDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.Cognome;
                ddlCodiceDelegato.SelectedValue = DelegatoTutore.delegato.AnagraficaTitolare.CodiceDelegato.ToString();
                lblNomeDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.Nome;
                if (DelegatoTutore.delegato.AnagraficaTitolare.Sesso != null)
                    lblSessoDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.Sesso.ToString();
                else
                    lblSessoDelegato.Text = string.Empty;

                if (DelegatoTutore.delegato.AnagraficaTitolare.DataNascita != null)
                    lblDataNascitaDelegato.Text = String.Format("{0:dd/MM/yyyy}", DelegatoTutore.delegato.AnagraficaTitolare.DataNascita);
                else
                    lblDataNascitaDelegato.Text = string.Empty;
                lblComuneNascitaDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.ComuneNascita;
                lblProvinciaNascitaDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.ProvinciaNascita;
                lblIndirizzoDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.Indirizzo;
                lblNCivicoDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.NumeroCivico;
                lblCapDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.Cap;
                lblComuneResidenzaDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.ComuneResidenza;
                lblProvinciaDelegato.Text = DelegatoTutore.delegato.AnagraficaTitolare.ProvinciaResidenza;
                if (DelegatoTutore.delegato.AnagraficaTitolare.DataMorte.HasValue)
                {
                    pnlDataMorte.Visible = true;
                    lblDataMorte.Text = string.Format("{0:dd/MM/yyyy}", DelegatoTutore.delegato.AnagraficaTitolare.DataMorte.Value);
                }
                else
                    pnlDataMorte.Visible = false;

                txtTelDelegato.Text = this.delegato.AnagraficaTitolare.Tel;
                txtCellDelegato.Text = this.delegato.AnagraficaTitolare.Cell;
                txtEmailDelegato.Text = this.delegato.AnagraficaTitolare.EMail;
                if (!String.IsNullOrEmpty(DelegatoTutore.delegato.AnagraficaTitolare.CodiceFiscale))
                {
                    ddlCodiceDelegato.Enabled = true;
                    txtCellDelegato.Enabled = true;
                    txtTelDelegato.Enabled = true;
                    txtEmailDelegato.Enabled = true;
                }
                else
                {
                    ddlCodiceDelegato.Enabled = false;
                    txtCellDelegato.Enabled = false;
                    txtTelDelegato.Enabled = false;
                    txtEmailDelegato.Enabled = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo ValorizzaEtichetteUCDelegato " + ex);
            }

        }

        internal AreaRispostaRiepilogo.DatiRiepilogoAnagrafica GetDatiUcDelegato()
        {
            try
            {
                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.delegato = new AreaRispostaRiepilogo();
                this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                this.domanda = Domanda;
                if (ViewState["Delegato"] != null)
                {
                    this.delegato.AnagraficaTitolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)ViewState["Delegato"];
                }

                this.delegato.AnagraficaTitolare.Cell = txtCellDelegato.Text;
                this.delegato.AnagraficaTitolare.Tel = txtTelDelegato.Text;
                this.delegato.AnagraficaTitolare.EMail = txtEmailDelegato.Text;
                if (!(String.IsNullOrEmpty(ddlCodiceDelegato.SelectedItem.Value)))
                    this.delegato.AnagraficaTitolare.CodiceDelegato = ddlCodiceDelegato.SelectedItem.Value[0];
                return this.delegato.AnagraficaTitolare;
            }

            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo GetDatiUcDelegato" + ex);
            }

        }
        public event EventHandler ErrorDelegato;
        public event EventHandler NotErrorDelegato;

        protected void RaiseErrorDelegato(object sender, EventArgs e)
        {
            if (ErrorDelegato != null)
                ErrorDelegato(sender, e);
        }

        protected void RaiseNotErrorDelegato(object sender, EventArgs e)
        {
            if (NotErrorDelegato != null)
                NotErrorDelegato(sender, e);
        }



        protected void RicercaDelegato_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();

            this.RicercaPosizione = new RicercaPosizione();
            this.RicercaPosizione.Domanda = this.domanda.NumeroDomanda;

            if (radioCodiceFiscaleDelegato.Checked)            //Ricerca per Codice Fiscale
            {
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.CodiceFiscale;
                this.RicercaPosizione.CodiceFiscale = hdnCodiceFiscaleDelegato.Value.Trim();
            }
            else if (radioAnagraficaDelegato.Checked)
            {                                          //Ricerca per anagrafica
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.Anagrafica;
                this.RicercaPosizione.Cognome = txtCognomeDelegato.Text;
                this.RicercaPosizione.Nome = txtNomeDelegato.Text;
                this.RicercaPosizione.DataNascita = txtDataNascitaDelegato.Text;

            }
            this.delegato = new AreaRispostaRiepilogo();
            this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            presenterDelegatoTutore.RicercaDelegato(this);        //Chiamata a RicercaDomanda
            if (HasError)
            {
                ViewState.Remove("Delegato");
                this.delegato = new AreaRispostaRiepilogo();
                datiOmonimi.Visible = false;
                divDatiDelegato.Visible = true;
                this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                //this.delegato.AnagraficaTitolare = this.RiepilogoAnagrafica;
                ViewState.Add("Delegato", this.delegato.AnagraficaTitolare);
                ValorizzaEtichetteDelegato(this);
                RaiseErrorDelegato(this, null);
            }
            else                    //OK
            {
                RaiseNotErrorDelegato(this, null);
                if (this.ElencoSinonimi != null)
                {
                    ViewState["SinonimiDelegato"] = this.ElencoSinonimi;
                    gvSinonimiDelegato_Load(this.ElencoSinonimi);
                    datiOmonimi.Visible = true;
                    divDatiDelegato.Visible = false;
                    ViewState.Remove("Delegato");
                }
                else
                {
                    this.delegato = new AreaRispostaRiepilogo();
                    datiOmonimi.Visible = false;
                    divDatiDelegato.Visible = true;
                    this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                    this.delegato.AnagraficaTitolare = this.RiepilogoAnagrafica;
                    if (ViewState["Delegato"] == null)
                        ViewState.Add("Delegato", this.delegato.AnagraficaTitolare);
                    else
                        ViewState["Delegato"] = this.delegato.AnagraficaTitolare;

                    ValorizzaEtichetteDelegato(this);
                }

                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                presenterDelegatoTutore.IsNotDelegatoPresent(this);
                if (HasError)
                {
                    object senderDelegatoPresent = new object();
                    EventArgs eDelegatoPresent = new EventArgs();
                    senderDelegatoPresent = this.ErrorMessage;
                    RaiseErrorDelegato(senderDelegatoPresent, eDelegatoPresent);
                }
            }

        }


        protected void ScegliSinonimo_onRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (e.CommandName == "CercaPosizioni")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
                this.RicercaPosizione = new RicercaPosizione();
                this.RicercaPosizione.Domanda = this.domanda.NumeroDomanda;
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.CodiceFiscale;
                this.RicercaPosizione.CodiceFiscale = r.Cells[0].Text;
                presenterDelegatoTutore.RicercaDelegato(this);

                //                presenterElaborazionePosizione.RicercaDomanda(this);
                if (HasError)
                {
                    ViewState.Remove("Delegato");
                    if (this.Esito == AreaEsito.TipoEsito.KO)
                    {
                        ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Ko;
                    }
                    else
                    {
                        ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Warning;
                    }

                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = ErrorMessage;
                }
                else                    //OK
                {
                    ucAvviso.Visible = false;

                    /* Chiamata a pagina RisultatoRicercaElaborazione */
                    datiOmonimi.Visible = false;
                    divDatiDelegato.Visible = true;
                    this.delegato = new AreaRispostaRiepilogo();
                    this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                    this.delegato.AnagraficaTitolare = this.RiepilogoAnagrafica;
                    if (ViewState["Delegato"] == null)
                        ViewState.Add("Delegato", this.delegato.AnagraficaTitolare);
                    else
                        ViewState["Delegato"] = this.delegato.AnagraficaTitolare;

                    ValorizzaEtichetteDelegato(this);

                }
            }
        }

        protected void gvSinonimiDelegato_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSinonimiDelegato.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiDelegato"];
                gvSinonimiDelegato_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo gvSinonimiDelegato_onPageIndexChanging" + ex);
            }
        }

        private const string ASCENDING = " ASC";

        private const string DESCENDING = " DESC";

        protected void gvSinonimiDelegato_onSorting(Object sender, GridViewSortEventArgs e)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiDelegato"];
                string sortExpression = e.SortExpression;
                if (GridViewSortDirection == SortDirection.Ascending)
                {
                    GridViewSortDirection = SortDirection.Descending;
                    SortGridViewSinonimi(sortExpression, DESCENDING);
                }
                else
                {
                    GridViewSortDirection = SortDirection.Ascending;
                    SortGridViewSinonimi(sortExpression, ASCENDING);
                }
                ViewState["sortDirection"] = m_SortDirection = GridViewSortDirection;
                ViewState["sortExp"] = m_strSortExp = e.SortExpression;
                gvSinonimiDelegato_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo gvSinonimiDelegato_onSorting" + ex);
            }
        }

        private SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        private void SortGridViewSinonimi(string sortExpression, string direction)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiDelegato"];
                if (sortExpression == "CodiceFiscale")
                {
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoCodiceFiscaleAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoCodiceFiscaleDisc);
                }
                else if (sortExpression == "Cognome")
                {
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoCognomeAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoCognomeDisc);
                }
                else if (sortExpression == "Nome")
                {
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoNomeAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoNomeDisc);
                }
                else if (sortExpression == "DataNascita")
                {
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoDataNascitaAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoDataNascitaDisc);
                }
                gvSinonimiDelegato_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo SortGridViewSinonimi" + ex);
            }
        }


        private void gvSinonimiDelegato_Load(List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi)
        {
            try
            {
                //if (ViewState["SinonimiDelegato"] == null)
                //    ViewState.Add("SinonimiDelegato", Sinonimi);
                //else
                ViewState["SinonimiDelegato"] = Sinonimi;
                gvSinonimiDelegato.DataSource = Sinonimi;
                gvSinonimiDelegato.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo gvSinonimiDelegato_Load" + ex);
            }
        }



        protected void gvSinonimiDelegato_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    if (String.Empty != m_strSortExp)
                    {
                        AddSortImage(e.Row);
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo gvSinonimiDelegato_RowCreated" + ex);
            }

        }
        private void AddSortImage(GridViewRow headerRow)
        {
            try
            {
                string currentTheme = Page.Theme;
                Int32 iCol = GetSortColumnIndex(m_strSortExp);

                if (-1 == iCol)
                {
                    return;
                }
                // Create the sorting image based on the sort direction.   
                System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
                if (SortDirection.Ascending == m_SortDirection)
                {
                    sortImage.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/down.png";
                    sortImage.AlternateText = "Descending Order";
                    sortImage.Height = 12;
                    sortImage.Width = 12;


                }
                else
                {
                    sortImage.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/up.png";
                    sortImage.AlternateText = "Ascending Order";
                    sortImage.Height = 12;
                    sortImage.Width = 12;

                }


                // Add the image to the appropriate header cell.   
                headerRow.Cells[iCol].Controls.Add(sortImage);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo AddSortImage" + ex);
            }

        }

        private int GetSortColumnIndex(String strCol)
        {
            try
            {
                {
                    foreach (DataControlField field in gvSinonimiDelegato.Columns)
                    {
                        if (field.SortExpression == strCol)
                        {
                            return gvSinonimiDelegato.Columns.IndexOf(field);
                        }
                    }
                }

                return -1;
            }

            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo GetSortColumnIndex" + ex);
            }
        }

        protected void btnSalvaTabDelegato_Click(object sender, EventArgs e)
        {
            this.delegato = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo();
            this.delegato.AnagraficaTitolare = GetDatiUcDelegato();
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
            presenterDelegatoTutore.SalvaDatiDelegato(this);

            if (!this.HasError)
                btnEliminaTabDelegato.Enabled = true;

            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaTabDelegato_Click(object sender, EventArgs e)
        {
            this.delegato = new AreaRispostaRiepilogo();
            this.delegato.AnagraficaTitolare = GetDatiUcDelegato();
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
            presenterDelegatoTutore.EliminaDelegato(this);

            if (this.HasError == true)
            {
                this.ErrorMessage = "Errore durante l'eliminazione del Delegato";
            }
            else
            {
                ViewState.Remove("Delegato");
                this.delegato = new AreaRispostaRiepilogo();
                this.delegato.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                ValorizzaEtichetteDelegato(this);
                // disabilita btn
                btnEliminaTabDelegato.Enabled = false;
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }
        public event EventHandler ShowAvviso;

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }
        public event EventHandler ShowAvvisoElimina;

        public void AbilitaPulsanteEliminaDelegato(bool abilita)
        {
            btnEliminaTabDelegato.Enabled = abilita;
        }

        //ENG - Reversibilita 024
        public void AbilitaTabDelegato(bool disabilita)
        {
            try
            {
                pnlDelegato.Enabled = disabilita;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDelegato, Errore nel metodo DisabilitaTabDelegato " + ex);
            }
        }

        //ENG - Reversibilita 024
        public event EventHandler GestisciDisabilitazioneTabDelegato;
        protected void RaiseGestisciDisabilitazioneTabDelegato(object sender, EventArgs e)
        {
            GestisciDisabilitazioneTabDelegato(sender, e);
        }
    }
}
