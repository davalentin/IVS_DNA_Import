using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RisultatoRicerca
{
    public partial class UCRisultatoRicerca : CustomBaseUserControl, IRicercaPosizione
    {
        private String m_strSortExpDomanda;
        private SortDirection m_SortDirectionDomanda;
        private String m_strSortExpPensione;
        private SortDirection m_SortDirectionPensione;
        public event EventHandler ReloadUChangeSede;

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

        protected void Page_Load(object sender, EventArgs e)
        {
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            bool isPaginaConferma = false;

            if (!IsPostBack)
            {
                try
                {
                    if (Server.HtmlEncode(Request.QueryString["Conferma"]) != null)
                    {
                        if (Server.HtmlEncode(Request.QueryString["Conferma"]) == "true")
                            isPaginaConferma = true;
                    }

                    Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                    List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];
                    List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione>)Session["Pensioni"];
                    List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo> Sinonimi = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoSinonimo>)Session["Sinonimi"];
                    if (Session["Domanda"] != null)
                    { // gestione torna alle posizioni trovate per far visualizzare correttamente lo stato e il numero di certificato
                        Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaSelezionata = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                        foreach (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda in Domande)
                        {
                            if (domanda.NumeroDomanda == domandaSelezionata.NumeroDomanda && domanda.Stato == "DA ACQUISIRE" && !isPaginaConferma)
                            {
                                domanda.Stato = "IN ACQUISIZIONE";
                                domanda.Certificato = domandaSelezionata.Certificato;
                            }
                        }
                        Session["Domande"] = Domande;
                    }
                    ValorizzaEtichetteRisultatoRicerca();
                    GestionePulsanti();
                    m_strSortExpDomanda = String.Empty;
                    GvDomande_Load(Domande);
                    m_strSortExpPensione = String.Empty;
                    GvPensioni_Load(Pensioni);
                }
                catch (DnaExceptionBase)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo PageLoad" + ex);
                }
            }
            else
            {
                if (null != ViewState["sortExpDomanda"])
                {
                    m_strSortExpDomanda = ViewState["sortExpDomanda"] as String;
                }
                if (null != ViewState["sortDirectionDomanda"])
                {
                    m_SortDirectionDomanda = (SortDirection)ViewState["sortDirectionDomanda"];
                }


                if (null != ViewState["sortExpPensione"])
                {
                    m_strSortExpPensione = ViewState["sortExpPensione"] as String;
                }
                if (null != ViewState["sortDirectionPensione"])
                {
                    m_SortDirectionPensione = (SortDirection)ViewState["sortDirectionPensione"];
                }
            }
        }

        private void ValorizzaEtichetteRisultatoRicerca()
        {
            try
            {
                Utility.TipoRicerca TipoRicerca;
                TipoRicerca = (Utility.TipoRicerca)Session["TipoRicerca"];
                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione>)Session["Pensioni"];
                String Nome = (String)Session["Nome"];
                String Cognome = (String)Session["Cognome"];
                String CF = (String)Session["CF"];
                HdnSedeOperatore.Value = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0');
                if (TipoRicerca == Utility.TipoRicerca.NDomus)
                {
                    lblParametriRicerca.Text = Domande != null && Domande.Count > 0 ? Domande.First().NumeroDomanda : string.Empty;
                }
                else if (TipoRicerca == Utility.TipoRicerca.CodiceFiscale)
                {
                    if (CF != null)
                        lblParametriRicerca.Text = CF.ToUpperInvariant();//Anagrafica.CodiceFiscale;
                    else
                    {
                        lblParametriRicerca.Text = Cognome != null ? Cognome.ToUpperInvariant() : string.Empty;
                        lblParametriRicerca2.Text = Nome != null ? Nome.ToUpperInvariant() : string.Empty;
                    }
                }
                else if (TipoRicerca == Utility.TipoRicerca.Anagrafica)
                {
                    lblParametriRicerca.Text = Cognome != null ? Cognome.ToUpperInvariant() : string.Empty;
                    lblParametriRicerca2.Text = Nome != null ? Nome.ToUpperInvariant() : string.Empty;
                }
                if (Domande != null)
                {
                    int ndomande = Domande.Count;
                    lblNDomandeTrovate.Text = ndomande.ToString();
                }
                if (Pensioni != null)
                {
                    int npensioni = Pensioni.Count;
                    lblNPensioniTrovate.Text = npensioni.ToString();
                }
                else
                {
                    lblNPensioniTrovate.Text = "0";
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo ValorizzaEtichetteRisultatoRicerca" + ex);
            }
        }

        private void GestionePulsanti()
        {
            try
            {
                if (Session["Sinonimi"] != null)
                {
                    btnElencoSinonimi.Visible = true;
                    btnTornaARicerca.Visible = true;
                }
                else
                {
                    btnElencoSinonimi.Visible = false;
                    btnTornaARicerca.Visible = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo GestionePulsanti" + ex);
            }

        }


        protected void onClickSinonimi(object sender, EventArgs e)
        {
            Response.Redirect("RisultatoRicercaElaborazione.aspx");
        }


        protected void PensioniGridView_onRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione>)Session["Pensioni"];

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    if (((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione)(e.Row.DataItem)).Tipo == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione.TipoOperazione.Ricostituibile)
                    {
                        Button btn = (Button)e.Row.FindControl("btnRicostituzione");
                        btn.Enabled = true;

                    }

                    else
                    {
                        Button btn = (Button)e.Row.FindControl("btnRicostituzione");
                        btn.Enabled = false;
                    }
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

        protected void gvDomande_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvDomande.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];
                GvDomande_Load(Domande);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo gvDomande_onPageIndexChanging" + ex);
            }
        }

        private const string ASCENDING = " ASC";

        private const string DESCENDING = " DESC";

        protected void gvDomande_onSorting(Object sender, GridViewSortEventArgs e)
        {
            try
            {
                string sortExpression = e.SortExpression;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];


                if (GridViewSortDirectionDomande == SortDirection.Ascending)
                {
                    GridViewSortDirectionDomande = SortDirection.Descending;
                    SortGridViewDomande(sortExpression, DESCENDING);
                }
                else
                {
                    GridViewSortDirectionDomande = SortDirection.Ascending;
                    SortGridViewDomande(sortExpression, ASCENDING);
                }

                ViewState["sortDirectionDomanda"] = m_SortDirectionDomanda = GridViewSortDirectionDomande;
                ViewState["sortExpDomanda"] = m_strSortExpDomanda = e.SortExpression;
                GvDomande_Load(Domande);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo gvDomande_onSorting" + ex);
            }


        }

        private SortDirection GridViewSortDirectionDomande
        {
            get
            {
                if (ViewState["sortDirectionDomanda"] == null)
                    ViewState["sortDirectionDomanda"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirectionDomanda"];

            }
            set { ViewState["sortDirectionDomanda"] = value; }
        }

        private SortDirection GridViewSortDirectionPensioni
        {
            get
            {
                if (ViewState["sortDirectionPensione"] == null)
                    ViewState["sortDirectionPensione"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirectionPensione"];

            }
            set { ViewState["sortDirectionPensione"] = value; }
        }

        private void SortGridViewDomande(string sortExpression, string direction)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];

                if (sortExpression == "NumeroDomanda")
                {
                    if (direction == ASCENDING)
                        Domande.Sort(Delegati.ConfrontoNumeroDomandaAsc);
                    else
                        Domande.Sort(Delegati.ConfrontoNumeroDomandaDesc);
                }
                else if (sortExpression == "DescProdotto")
                {
                    if (direction == ASCENDING)
                        Domande = Domande.OrderBy(x => x.DescProdotto).ToList();
                    else
                        Domande = Domande.OrderByDescending(x => x.DescProdotto).ToList();
                }
                else if (sortExpression == "Categoria")
                {
                    if (direction == ASCENDING)
                        Domande.Sort(Delegati.ConfrontoCategoriaAsc);
                    else
                        Domande.Sort(Delegati.ConfrontoCategoriaDesc);
                }
                else if (sortExpression == "SedeCO")
                {
                    if (direction == ASCENDING)
                        Domande.Sort(Delegati.ConfrontoSedeAsc);
                    else
                        Domande.Sort(Delegati.ConfrontoSedeDesc);
                }
                else if (sortExpression == "Certificato")
                {
                    if (direction == ASCENDING)
                        Domande.Sort(Delegati.ConfrontoCertificatoAsc);
                    else
                        Domande.Sort(Delegati.ConfrontoCertificatoDesc);
                }
                else if (sortExpression == "Stato")
                {
                    if (direction == ASCENDING)
                        Domande.Sort(Delegati.ConfrontoStatoAsc);
                    else
                        Domande.Sort(Delegati.ConfrontoStatoDesc);
                }
                else if (sortExpression == "Tipo")
                {
                    if (direction == ASCENDING)
                        Domande.Sort(Delegati.ConfrontoTipoAsc);
                    else
                        Domande.Sort(Delegati.ConfrontoTipoDesc);
                }


                GvDomande_Load(Domande);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo SortGridView" + ex);
            }
        }

        private void SortGridViewPensioni(string sortExpression, string direction)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione>)Session["Pensioni"];

                if (sortExpression == "Certificato")
                {
                    if (direction == ASCENDING)
                        Pensioni.Sort(Delegati.ConfrontoCertificatoPensioneAsc);
                    else
                        Pensioni.Sort(Delegati.ConfrontoCertificatoPensioneDesc);
                }
                else if (sortExpression == "Categoria")
                {
                    if (direction == ASCENDING)
                        Pensioni.Sort(Delegati.ConfrontoCategoriaPensioneAsc);
                    else
                        Pensioni.Sort(Delegati.ConfrontoCategoriaPensioneDesc);
                }
                else if (sortExpression == "Sede")
                {
                    if (direction == ASCENDING)
                        Pensioni.Sort(Delegati.ConfrontoSedePensioneAsc);
                    else
                        Pensioni.Sort(Delegati.ConfrontoSedePensioneDesc);
                }
                else if (sortExpression == "DataCalcolo")
                {
                    if (direction == ASCENDING)
                        Pensioni.Sort(Delegati.ConfrontoDataCalcoloPensioneAsc);
                    else
                        Pensioni.Sort(Delegati.ConfrontoDataCalcoloPensioneDesc);
                }
                else if (sortExpression == "TipoComponente")
                {
                    if (direction == ASCENDING)
                        Pensioni.Sort(Delegati.ConfrontoTipoComponentePensioneAsc);
                    else
                        Pensioni.Sort(Delegati.ConfrontoTipoComponentePensioneDesc);
                }
                else if (sortExpression == "Eliminazione")
                {
                    if (direction == ASCENDING)
                        Pensioni.Sort(Delegati.ConfrontoEliminazionePensioneAsc);
                    else
                        Pensioni.Sort(Delegati.ConfrontoEliminazionePensioneDesc);
                }
                GvPensioni_Load(Pensioni);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo SortGridView" + ex);
            }
        }

        protected void gvPensioni_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvPensioni.PageIndex = e.NewPageIndex;
                gvPensioni.Columns[colonnegvPensioni.Eliminazione.GetHashCode()].Visible = false;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione>)Session["Pensioni"];
                GvPensioni_Load(Pensioni);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo gvPensioni_onPageIndexChanging" + ex);
            }

        }

        protected void gvPensioni_onSorting(Object sender, GridViewSortEventArgs e)
        {
            try
            {
                gvPensioni.Columns[colonnegvPensioni.Eliminazione.GetHashCode()].Visible = false;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione>)Session["Pensioni"];
                string sortExpression = e.SortExpression;
                if (GridViewSortDirectionPensioni == SortDirection.Ascending)
                {
                    GridViewSortDirectionPensioni = SortDirection.Descending;
                    SortGridViewPensioni(sortExpression, DESCENDING);
                }
                else
                {
                    GridViewSortDirectionPensioni = SortDirection.Ascending;
                    SortGridViewPensioni(sortExpression, ASCENDING);
                }
                ViewState["sortDirectionPensione"] = m_SortDirectionPensione = GridViewSortDirectionPensioni;
                ViewState["sortExpPensione"] = m_strSortExpPensione = e.SortExpression;
                GvPensioni_Load(Pensioni);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo gvPensioni_onSorting" + ex);
            }

        }

        protected void gvDomande_onRowCommand(Object sender, GridViewCommandEventArgs e)
        {

        }

        private void GvDomande_Load(List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande)
        {
            try
            {
                if (Session["Domande"] == null)
                    Session.Add("Domande", Domande);
                else
                    Session["Domande"] = Domande;
                gvDomande.DataSource = Domande;
                gvDomande.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo GvDomande_Load" + ex);
            }

        }

        private void GvPensioni_Load(List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione> Pensioni)
        {
            try
            {
                if (Session["Pensioni"] == null)
                    Session.Add("Pensioni", Pensioni);
                else
                    Session["Pensioni"] = Pensioni;
                gvPensioni.DataSource = Pensioni;
                gvPensioni.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo GvPensioni_Load" + ex);
            }

        }


        protected void gvDomande_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if (e.Row.RowType == DataControlRowType.Header)
                {
                    if (String.Empty != m_strSortExpDomanda)
                    {
                        AddSortImage(e.Row, "domanda");
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo gvDomande_RowCreated" + ex);
            }


        }

        protected void gvPensioni_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    if (String.Empty != m_strSortExpPensione)
                    {
                        AddSortImage(e.Row, "pensione");
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo gvPensioni_RowCreated" + ex);
            }
        }

        protected void btnConfermaPopUp_Click(object sender, EventArgs e)
        {
            string numeroDomanda = HdnNdomusSelezionato.Value;
            string progStorico = HdnProgStorico.Value;
            string sedeDomanda = HdnSedeDomanda.Value;
            string sedeOperatore = HdnSedeOperatore.Value;
            bool IsConsultazioneDomandaTRF = false;
            Boolean.TryParse(HdnIsConsultazioneDomandaTRF.Value, out IsConsultazioneDomandaTRF);
            string tipoDomanda = HdnTipoDomanda.Value;
            string messaggioVideo = string.Empty;

            if (sedeDomanda != sedeOperatore)
            {
                if (!CodeUtility.ChangeSede((Ruoli)Session["Ruolo"], sedeDomanda, false, out messaggioVideo))
                {
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Messaggio = messaggioVideo;
                    return;
                }

                RaiseReloadUChangeSede(this, null);
                HdnSedeOperatore.Value = INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode;
            }

            PresenterElaborazionePosizione presenterElaborazionePosizione = new PresenterElaborazionePosizione();

            this.RicercaPosizione = new RicercaPosizione();
            this.RicercaPosizione.Selezione = Utility.TipoRicerca.NDomus;
            this.RicercaPosizione.Domanda = numeroDomanda;
            this.RicercaPosizione.ProgStorico = (tipoDomanda != "PL" || !IsConsultazioneDomandaTRF) ? progStorico : null;                   
            this.IsConsultazione = IsConsultazioneDomandaTRF;
            this.IsDomandaDB = false;

            //Chiamata a RicercaDomanda
            presenterElaborazionePosizione.RicercaDomanda(this);

            if (HasError)
            {
                ucAvviso.Visible = true;
                if (this.Esito == AreaEsito.TipoEsito.KO)
                    ucAvviso.Tipo = TipoAvviso.Ko;
                else
                    ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = this.ErrorMessage;
                return;
            }
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda Domanda = this.ElencoDomande[0];

            if (String.Equals(Domanda.Categoria.Trim(), "VTELE".Trim()))
            {
                Domanda.Certificato = "0";
            }


            Session["Domanda"] = ((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)(Domanda));
            Session["Anagrafica"] = this.RiepilogoAnagrafica;
            Session["EsitoCalcolo"] = this.EsitoCalcolo;
            Session.Remove("Semaforo");
            Session.Remove("DatiPensione");
            Session.Remove("Lavorabile");
            Session.Remove("Sinonimi");

            if (((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)(Domanda)).Stato == "DA ACQUISIRE")
                Presenter.LogSicurezza.ScritturaLog(((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)(Domanda)).NumeroDomanda,
                ((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)(Domanda)).TipoAppartenenza,
                int.Parse(ConfigurationManager.AppSettings["IDEVENTO-ACQUISIZIONE"]), HttpContext.Current.Request.UserHostAddress, 0, string.Empty,
                this.RiepilogoAnagrafica.CodiceFiscale, string.Empty);
            else
                Presenter.LogSicurezza.ScritturaLog(((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)(Domanda)).NumeroDomanda,
                ((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)(Domanda)).TipoAppartenenza,
                int.Parse(ConfigurationManager.AppSettings["IDEVENTO-CONSULTAZIONE"]), HttpContext.Current.Request.UserHostAddress, 0, string.Empty,
                this.RiepilogoAnagrafica.CodiceFiscale, string.Empty);

            if (this.IsDomandaDB && !this.IsDomandaCalcolataProvvisoria && !IsConsultazioneDomandaTRF)
                Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
            else
                if (this.IsDomandaCalcolataProvvisoria || IsConsultazioneDomandaTRF)
                    Response.Redirect("ConfermaAcquisizione.aspx?Posizioni=true&Consulta=true", false); //con il query string gestisco la visualizzazione del pulsante "Torna alle posizioni trovate".
                else
                    Response.Redirect("ConfermaAcquisizione.aspx?Posizioni=true", false); //con il query string gestisco la visualizzazione del pulsante "Torna alle posizioni trovate".

        }

        private void AddSortImage(GridViewRow headerRow, String gvType)
        {
            try
            {
                string currentTheme = Page.Theme;
                Int32 iCol;
                if (gvType == "domanda")
                    iCol = GetSortColumnIndex(m_strSortExpDomanda, gvType);
                else
                    iCol = GetSortColumnIndex(m_strSortExpPensione, gvType);
                if (-1 == iCol)
                {
                    return;
                }
                // Create the sorting image based on the sort direction.   
                System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
                if (gvType == "domanda")
                {
                    if (SortDirection.Ascending == m_SortDirectionDomanda)
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
                }
                else
                {
                    if (SortDirection.Ascending == m_SortDirectionPensione)
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
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo AddSortImage" + ex);
            }

        }

        private int GetSortColumnIndex(String strCol, String gvType)
        {
            try
            {
                if (gvType == "domanda")
                {
                    foreach (DataControlField field in gvDomande.Columns)
                    {
                        if (field.SortExpression == strCol)
                        {
                            return gvDomande.Columns.IndexOf(field);
                        }
                    }
                }
                else if (gvType == "pensione")
                {
                    foreach (DataControlField field in gvPensioni.Columns)
                    {
                        if (field.SortExpression == strCol)
                        {
                            return gvPensioni.Columns.IndexOf(field);
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
                throw new INPS.DNA.DnaApplicationException("UCRisultatoRicerca, Errore nel metodo GetSortColumnIndex" + ex);
            }

        }

        protected object IsVisibleStampa(GridViewRow row)
        {
            AreaRispostaStatoPratica.DatiRiepilogoDomanda statoPratica = (AreaRispostaStatoPratica.DatiRiepilogoDomanda)row.DataItem;

            if (statoPratica != null)
            {
                CodeUtility.StatoPensione? stato = CodeUtility.GetValueFromDescription<CodeUtility.StatoPensione>(statoPratica.Stato);
                if (stato != null)
                {
                    switch (stato)
                    {
                        case CodeUtility.StatoPensione.Calcolata:
                        case CodeUtility.StatoPensione.CalcoloVerify:
                        case CodeUtility.StatoPensione.CalcolataNoWebDom:
                        case CodeUtility.StatoPensione.CalcolataNoFelpe:
                        case CodeUtility.StatoPensione.CalcolataNoOneri:
                        case CodeUtility.StatoPensione.CalcolataNoSai:
                        case CodeUtility.StatoPensione.CalcolataNoStazLavoro:
                        case CodeUtility.StatoPensione.CalcolataNoTotal:
                        case CodeUtility.StatoPensione.CalcolataNoSin:
                        case CodeUtility.StatoPensione.CalcolataNoNoteDebito:
                        case CodeUtility.StatoPensione.CalcolataNo6Scatti:
                        case CodeUtility.StatoPensione.CalcolataNoEquoInd:
                        case CodeUtility.StatoPensione.CalcolataNoIndennSpec:
                            break;
                        case CodeUtility.StatoPensione.ScartoDaCalcolo:
                        case CodeUtility.StatoPensione.ScartoVerify:
                            if (TipoAppRuolo == UtilityTipoAppartenenza.AGO)
                                break;
                            else
                                return false;
                        default:
                            return false;
                    }
                }
                else
                    return false;

                try
                {
                    if ((statoPratica.Sede + statoPratica.CentroOperativo) != INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            gvDomande.Columns[colonnegvPratiche.Stampa.GetHashCode()].Visible = true;
            return true;
        }

        protected object IsVisibleCodEliminazione(GridViewRow row)
        {
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione pensione = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoPensione)row.DataItem;

            if (pensione != null)
            {
                if (string.IsNullOrEmpty(pensione.Eliminazione))
                    return false;
            }

            gvPensioni.Columns[colonnegvPensioni.Eliminazione.GetHashCode()].Visible = true;
            return true;
        }

        protected void RaiseReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede(sender, e);
        }

        public string GetSedeForView(object dataItem)
        {
            string strSede = string.Empty;
            if (dataItem as AreaRispostaRiepilogo.DatiRiepilogoDomanda != null)
            {
                AreaRispostaRiepilogo.DatiRiepilogoDomanda datiRiepilogoDomanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)dataItem;
                if (TipoAppRuolo == UtilityTipoAppartenenza.AGO && !string.IsNullOrEmpty(datiRiepilogoDomanda.SedeDestinazione))
                    strSede = datiRiepilogoDomanda.SedeDestinazione + datiRiepilogoDomanda.CentroOperativoDestinazione;
                else
                    strSede = datiRiepilogoDomanda.SedeDaVisualizzare + datiRiepilogoDomanda.CentroOperativoDaVisualizzare;
            }
            return strSede;
        }

    }

    enum colonnegvPensioni
    {
        Eliminazione = 5
    }

    enum colonnegvPratiche
    {
        Stampa = 7
    }
}
