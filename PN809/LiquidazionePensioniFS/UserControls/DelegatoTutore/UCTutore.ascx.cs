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
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore
{
    public partial class UCTutore : CustomBaseUserControl, IDelegatoTutore, IRicercaPosizione
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
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo tutore { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo delegato { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDelegatoTutore

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
                m_strSortExp = String.Empty;
                if (ddlCodiceTutore.Items.Count == 0)
                    Load_ddlCodiceTutore();

                AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.domanda = Domanda;
                PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
                presenterDelegatoTutore.IsNotTutorePresent(this);

                if (!this.HasError == true)
                {
                    btnEliminaTabTutore.Enabled = false;
                }
            }
            AbilitaPannelli();

            btnRicerca1Tutore.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/search24.png";
            btnRicerca2Tutore.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/search24.png";
        }



        private void AbilitaPannelli()
        {
            radioAnagraficaTutore.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
            radioAnagraficaTutore.InputAttributes.Add("EnableClass", "onClassAnagraficaTutore");
            radioCodiceFiscaleTutore.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
            radioCodiceFiscaleTutore.InputAttributes.Add("EnableClass", "onClassCodiceFiscaleTutore");
            divTxtCodiceFiscaleTutore.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
            divTxtCodiceFiscaleTutore.Attributes.Add("EnableClass", "onClassCodiceFiscaleTutore");
            divTxtCognomeTutore.Attributes.Add("onclick", "javascript:SetRadio_" + this.ClientID + "(this)");
            divTxtCognomeTutore.Attributes.Add("EnableClass", "onClassAnagraficaTutore");

        }


        internal void ValorizzaEtichetteTutore(IDelegatoTutore DelegatoTutore)
        {
            ViewState["Tutore"] = DelegatoTutore.tutore.AnagraficaTitolare;
            try
            {
                if (ddlCodiceTutore.Items.Count == 0)
                    Load_ddlCodiceTutore();

                lblCFTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.CodiceFiscale;
                hdnCodiceFiscaleTutore.Value = DelegatoTutore.tutore.AnagraficaTitolare.CodiceFiscale;
                lblCognomeTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.Cognome;
                if (DelegatoTutore.tutore.AnagraficaTitolare.CodiceTutore.HasValue)
                    ddlCodiceTutore.SelectedValue = DelegatoTutore.tutore.AnagraficaTitolare.CodiceTutore.ToString();
                else
                    ddlCodiceTutore.SelectedIndex = 0;
                if (DelegatoTutore.tutore.AnagraficaTitolare.CessValAmmSost.HasValue)
                    txtCessValAmmSost.Text = String.Format("{0:MM/yyyy}", DelegatoTutore.tutore.AnagraficaTitolare.CessValAmmSost.Value);
                else
                    txtCessValAmmSost.Text = "MM/AAAA";
                lblNomeTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.Nome;
                if (DelegatoTutore.tutore.AnagraficaTitolare.Sesso != null)
                    lblSessoTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.Sesso.ToString();
                else
                    lblSessoTutore.Text = string.Empty;
                if (DelegatoTutore.tutore.AnagraficaTitolare.DataNascita.HasValue)
                    lblDataNascitaTutore.Text = String.Format("{0:dd/MM/yyyy}", DelegatoTutore.tutore.AnagraficaTitolare.DataNascita);
                else
                    lblDataNascitaTutore.Text = string.Empty;
                lblComuneNascitaTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.ComuneNascita;
                lblProvinciaNascitaTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.ProvinciaNascita;
                lblIndirizzoTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.Indirizzo;
                lblNCivicoTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.NumeroCivico;
                lblCapTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.Cap;
                lblComuneResidenzaTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.ComuneResidenza;
                lblProvinciaTutore.Text = DelegatoTutore.tutore.AnagraficaTitolare.ProvinciaResidenza;
                if (DelegatoTutore.tutore.AnagraficaTitolare.DataMorte.HasValue)
                {
                    pnlDataMorte.Visible = true;
                    lblDataMorte.Text = string.Format("{0:dd/MM/yyyy}", DelegatoTutore.tutore.AnagraficaTitolare.DataMorte.Value);
                }
                else
                    pnlDataMorte.Visible = false;

                txtTelTutore.Text = this.tutore.AnagraficaTitolare.Tel;
                txtCellTutore.Text = this.tutore.AnagraficaTitolare.Cell;
                txtEmailTutore.Text = this.tutore.AnagraficaTitolare.EMail;
                if (!String.IsNullOrEmpty(DelegatoTutore.tutore.AnagraficaTitolare.CodiceFiscale))
                {
                    ddlCodiceTutore.Enabled = true;
                    txtTelTutore.Enabled = true;
                    txtCellTutore.Enabled = true;
                    txtEmailTutore.Enabled = true;
                }
                else
                {
                    ddlCodiceTutore.Enabled = false;
                    txtTelTutore.Enabled = false;
                    txtCellTutore.Enabled = false;
                    txtEmailTutore.Enabled = false;
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

        internal AreaRispostaRiepilogo.DatiRiepilogoAnagrafica GetDatiUcTutore()
        {
            try
            {
                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.tutore = new AreaRispostaRiepilogo();
                this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                this.domanda = Domanda;

                if (ViewState["Tutore"] != null)
                {
                    this.tutore.AnagraficaTitolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)ViewState["Tutore"];
                }
                this.tutore.AnagraficaTitolare.Cell = txtCellTutore.Text;
                this.tutore.AnagraficaTitolare.Tel = txtTelTutore.Text;
                this.tutore.AnagraficaTitolare.EMail = txtEmailTutore.Text;
                if (!(String.IsNullOrEmpty(ddlCodiceTutore.SelectedItem.Value)))
                    this.tutore.AnagraficaTitolare.CodiceTutore = ddlCodiceTutore.SelectedItem.Value[0];
                if (!string.IsNullOrEmpty(txtCessValAmmSost.Text) && !txtCessValAmmSost.Text.ToUpperInvariant().Equals("MM/AAAA"))
                    this.tutore.AnagraficaTitolare.CessValAmmSost = Utility.GetDateFromString(txtCessValAmmSost.Text);
                else
                    this.tutore.AnagraficaTitolare.CessValAmmSost = null;
                return this.tutore.AnagraficaTitolare;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo GetDatiUcTutore" + ex);
            }
        }

        protected void RicercaTutore_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();

            this.RicercaPosizione = new RicercaPosizione();
            this.RicercaPosizione.Domanda = this.domanda.NumeroDomanda;

            if (radioCodiceFiscaleTutore.Checked)            //Ricerca per Codice Fiscale
            {
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.CodiceFiscale;
                this.RicercaPosizione.CodiceFiscale = hdnCodiceFiscaleTutore.Value.Trim();
            }
            else if (radioAnagraficaTutore.Checked)
            {                                          //Ricerca per anagrafica
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.Anagrafica;
                this.RicercaPosizione.Cognome = txtCognomeTutore.Text;
                this.RicercaPosizione.Nome = txtNomeTutore.Text;
                this.RicercaPosizione.DataNascita = txtDataNascitaTutore.Text;

            }
            this.tutore = new AreaRispostaRiepilogo();
            this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            presenterDelegatoTutore.RicercaTutore(this);        //Chiamata a RicercaDomanda

            if (HasError)
            {
                ViewState.Remove("Tutore");

                datiOmonimiTutore.Visible = false;
                divDatiTutore.Visible = true;
                this.tutore = new AreaRispostaRiepilogo();
                this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                //this.tutore.AnagraficaTitolare = this.RiepilogoAnagrafica;
                ViewState.Add("Tutore", this.tutore.AnagraficaTitolare);
                ValorizzaEtichetteTutore(this);
                RaiseErrorTutore(this, null);

            }
            else                    //OK
            {
                RaiseNotErrorTutore(this, null);
                if (this.ElencoSinonimi != null)
                {
                    ViewState["SinonimiTutore"] = this.ElencoSinonimi;
                    gvSinonimiTutore_Load(this.ElencoSinonimi);
                    datiOmonimiTutore.Visible = true;
                    divDatiTutore.Visible = false;
                    ViewState.Remove("Tutore");
                }
                else
                {
                    datiOmonimiTutore.Visible = false;
                    divDatiTutore.Visible = true;
                    this.tutore = new AreaRispostaRiepilogo();
                    this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                    this.tutore.AnagraficaTitolare = this.RiepilogoAnagrafica;
                    if (ViewState["Tutore"] == null)
                        ViewState.Add("Tutore", this.tutore.AnagraficaTitolare);
                    else
                        ViewState["Tutore"] = this.tutore.AnagraficaTitolare;
                    ValorizzaEtichetteTutore(this);
                }

                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                presenterDelegatoTutore.IsNotTutorePresent(this);
                if (HasError)
                {
                    object senderTutorePresent = new object();
                    EventArgs eTutorePresent = new EventArgs();
                    senderTutorePresent = this.ErrorMessage;
                    RaiseErrorTutore(senderTutorePresent, eTutorePresent);
                }
            }

        }

        public event EventHandler ErrorTutore;
        public event EventHandler NotErrorTutore;

        protected void RaiseErrorTutore(object sender, EventArgs e)
        {
            if (ErrorTutore != null)
                ErrorTutore(sender, e);
        }

        protected void RaiseNotErrorTutore(object sender, EventArgs e)
        {
            if (NotErrorTutore != null)
                NotErrorTutore(sender, e);
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
                //                PresenterElaborazionePosizione presenterElaborazionePosizione = new PresenterElaborazionePosizione();
                this.RicercaPosizione = new RicercaPosizione();
                this.RicercaPosizione.Domanda = this.domanda.NumeroDomanda;
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.CodiceFiscale;
                this.RicercaPosizione.CodiceFiscale = r.Cells[0].Text;
                presenterDelegatoTutore.RicercaTutore(this);

                //                presenterElaborazionePosizione.RicercaDomanda(this);
                if (HasError)
                {
                    ViewState.Remove("Tutore");
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
                    datiOmonimiTutore.Visible = false;
                    divDatiTutore.Visible = true;
                    this.tutore = new AreaRispostaRiepilogo();
                    this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                    this.tutore.AnagraficaTitolare = this.RiepilogoAnagrafica;
                    if (ViewState["Tutore"] == null)
                        ViewState.Add("Tutore", this.tutore.AnagraficaTitolare);
                    else
                        ViewState["Tutore"] = this.tutore.AnagraficaTitolare;
                    ValorizzaEtichetteTutore(this);

                }
            }
        }

        protected void gvSinonimiTutore_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSinonimiTutore.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiTutore"];
                gvSinonimiTutore_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo gvSinonimiTutore_onPageIndexChanging" + ex);
            }
        }

        private const string ASCENDING = " ASC";

        private const string DESCENDING = " DESC";

        protected void gvSinonimiTutore_onSorting(Object sender, GridViewSortEventArgs e)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiTutore"];
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
                gvSinonimiTutore_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo gvSinonimiTutore_onSorting" + ex);
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
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)ViewState["SinonimiTutore"];
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
                gvSinonimiTutore_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo SortGridViewSinonimi" + ex);
            }
        }


        private void gvSinonimiTutore_Load(List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi)
        {
            try
            {
                if (ViewState["SinonimiTutore"] == null)
                    ViewState.Add("SinonimiTutore", Sinonimi);
                else
                    ViewState["SinonimiTutore"] = Sinonimi;
                gvSinonimiTutore.DataSource = Sinonimi;
                gvSinonimiTutore.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo gvSinonimiTutore_Load" + ex);
            }
        }



        protected void gvSinonimiTutore_RowCreated(object sender, GridViewRowEventArgs e)
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
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo gvSinonimiTutore_RowCreated" + ex);
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
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo AddSortImage" + ex);
            }

        }

        private int GetSortColumnIndex(String strCol)
        {
            try
            {
                {
                    foreach (DataControlField field in gvSinonimiTutore.Columns)
                    {
                        if (field.SortExpression == strCol)
                        {
                            return gvSinonimiTutore.Columns.IndexOf(field);
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
                throw new INPS.DNA.DnaApplicationException("UCTutore, Errore nel metodo GetSortColumnIndex" + ex);
            }
        }



        private void Load_ddlCodiceTutore()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            ListItem li = new ListItem();
            ddlCodiceTutore.Items.Add(li);
            foreach (AreaDecodifica.DatiTutore code in valoriDecodificati.ElencoTutore)
            {
                li = new ListItem(code.Descrizione, code.Id);
                ddlCodiceTutore.Items.Add(li);
            }
        }

        protected void btnSalvaTabTutore_Click(object sender, EventArgs e)
        {
            this.tutore = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo();
            this.tutore.AnagraficaTitolare = GetDatiUcTutore();
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
            presenterDelegatoTutore.SalvaDatiTutore(this);

            if (!this.HasError)
            {
                btnEliminaTabTutore.Enabled = true;
                RaiseGestisciDisabilitazioneTabDelegato(this, null);
            }

            RaiseShowAvviso(this, null);


        }

        protected void btnEliminaTabTutore_Click(object sender, EventArgs e)
        {
            this.tutore = new AreaRispostaRiepilogo();
            this.tutore.AnagraficaTitolare = GetDatiUcTutore();
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDelegatoTutore presenterDelegatoTutore = new PresenterDelegatoTutore();
            presenterDelegatoTutore.EliminaTutore(this);

            if (this.HasError == true)
            {
                this.ErrorMessage = "Errore durante l'eliminazione del Tutore";
            }
            else
            {
                ViewState.Remove("Tutore");
                this.tutore = new AreaRispostaRiepilogo();
                this.tutore.AnagraficaTitolare = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                ValorizzaEtichetteTutore(this);
                // disabilita btn
                btnEliminaTabTutore.Enabled = false;
                RaiseGestisciDisabilitazioneTabDelegato(this, null);
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


        public void AbilitaPulsanteEliminaTutore(bool abilita)
        {
            btnEliminaTabTutore.Enabled = abilita;
        }

        //ENG - Reversibilita 024
        public event EventHandler GestisciDisabilitazioneTabDelegato;
        protected void RaiseGestisciDisabilitazioneTabDelegato(object sender, EventArgs e)
        {
            GestisciDisabilitazioneTabDelegato(sender, e);
        }
    }
}
