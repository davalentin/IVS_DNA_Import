using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using System.Collections;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RisultatoRicerca
{
    public partial class UCSinonimi : CustomBaseUserControl, IRicercaPosizione
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

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        private String m_strSortExp;
        private SortDirection m_SortDirection;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);

            if (!IsPostBack)
            {
                Session["Sinonimo"] = 1;
                try
                {
                    List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
                    ValorizzaEtichetteUCSinonimi();
                    m_strSortExp = String.Empty;
                    GvSinonimi_Load(Sinonimi);

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

            else
            {
                if (null != ViewState["sortExp"])
                {
                    m_strSortExp = ViewState["sortExp"] as String;
                }
                if (null != ViewState["sortDirection"])
                {
                    m_SortDirection = (SortDirection)ViewState["sortDirection"];
                }
            }  
        }

        private void ValorizzaEtichetteUCSinonimi() {
            try
            {

                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
                if (Sinonimi != null)
                {
                    int nOmonimi = Sinonimi.Count;
                    lblNOmonimi.Text = nOmonimi.ToString();
                    if (Session["Cognome"] != null)
                        lblParametriRicerca.Text = (String)Session["Cognome"].ToString().ToUpperInvariant();
                    if (Session["Nome"] != null)
                        lblParametriRicerca2.Text = (String)Session["Nome"].ToString().ToUpperInvariant();
                }
                else
                {
                    
                    lblNOmonimi.Text = "0";
                    if (Session["Cognome"] != null)
                        lblParametriRicerca.Text = (String)Session["Cognome"].ToString().ToUpperInvariant();
                    if (Session["Nome"] != null)
                        lblParametriRicerca2.Text = (String)Session["Nome"].ToString().ToUpperInvariant();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex) {
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, errore nel metodo ValorizzaEtichetteUCSinonimi" + ex);
            }
        }

        protected void ScegliSinonimo_onRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CercaPosizioni")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                PresenterElaborazionePosizione presenterElaborazionePosizione = new PresenterElaborazionePosizione();
                this.RicercaPosizione = new RicercaPosizione();
                this.RicercaPosizione.Selezione = Utility.TipoRicerca.CodiceFiscale;
                this.RicercaPosizione.CodiceFiscale = r.Cells[0].Text;

                presenterElaborazionePosizione.RicercaDomanda(this);
                if (HasError)
                {
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

                    /*Inserimento dati in sessione*/
                    Session["TipoRicerca"] = this.RicercaPosizione.Selezione;
                    Session["Anagrafica"] = this.RiepilogoAnagrafica;
                    Session["Domande"] = this.ElencoDomande;
                    Session["Pensioni"] = this.ElencoPensioni;
                    Session["InfoErroreWebDom"] = this.ErrorMessage;

                    /* Chiamata a pagina RisultatoRicercaElaborazione */
                    Response.Redirect("RisultatoRicercaElaborazione.aspx");
                }
            }
        }

        protected void gvSinonimi_onPageIndexChanging(Object sender, GridViewPageEventArgs e) {
            try
            {
                gvSinonimi.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
                GvSinonimi_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo gvSinonimi_onPageIndexChanging" + ex);
            }
        }

        private const string ASCENDING = " ASC";

        private const string DESCENDING = " DESC";

        protected void gvSinonimi_onSorting(Object sender, GridViewSortEventArgs e) {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
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
                GvSinonimi_Load(Sinonimi);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo gvSinonimi_onSorting" + ex);
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
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
                if(sortExpression=="CodiceFiscale"){
                    if(direction==ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoCodiceFiscaleAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoCodiceFiscaleDisc);
                }
                else if (sortExpression == "Cognome") {
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoCognomeAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoCognomeDisc);
                }
                else if(sortExpression=="Nome"){
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoNomeAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoNomeDisc);
                }
                else if(sortExpression=="DataNascita"){
                    if (direction == ASCENDING)
                        Sinonimi.Sort(Delegati.ConfrontoDataNascitaAsc);
                    else
                        Sinonimi.Sort(Delegati.ConfrontoDataNascitaDisc);
                }
                GvSinonimi_Load(Sinonimi); 
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo SortGridViewSinonimi" + ex);
            }
        }


        private void GvSinonimi_Load(List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi) {
            try
            {
                if (Session["Sinonimi"] == null)
                    Session.Add("Sinonimi", Sinonimi);
                else
                    Session["Sinonimi"] = Sinonimi;
                gvSinonimi.DataSource = Sinonimi;
                gvSinonimi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo gvSinonimi_Load" + ex);
            }
        }



        protected void gvSinonimi_RowCreated(object sender, GridViewRowEventArgs e)
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
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo gvSinonimi_RowCreated" + ex);
            }

        }
        private void AddSortImage(GridViewRow headerRow)
        {
            try
            {
                Int32 iCol = GetSortColumnIndex(m_strSortExp);
                string currentTheme = Page.Theme;

                if (-1 == iCol)
                {
                    return;
                }
                // Create the sorting image based on the sort direction.   
                System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
                if (SortDirection.Ascending == m_SortDirection)
                {
                    sortImage.ImageUrl = "~/App_Themes/" + currentTheme + "/ Images/down.png";
                    sortImage.AlternateText = "Descending Order";
                    sortImage.Height = 12;
                    sortImage.Width = 12;


                }
                else
                {
                    sortImage.ImageUrl = "~/App_Themes/" + currentTheme + "/ Images/up.png";
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
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo AddSortImage" + ex);
            }

        }

        private int GetSortColumnIndex(String strCol)
        {
            try
            {
                {
                    foreach (DataControlField field in gvSinonimi.Columns)
                    {
                        if (field.SortExpression == strCol)
                        {
                            return gvSinonimi.Columns.IndexOf(field);
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
                throw new INPS.DNA.DnaApplicationException("UCSinonimi, Errore nel metodo GetSortColumnIndex" + ex);
            }
        }
    }
}
