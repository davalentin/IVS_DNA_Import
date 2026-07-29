using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using System.Configuration;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.VisualizzaStatoPratiche
{
    public partial class UCVisualizzaStatoPratiche : CustomBaseUserControl, IRicercaPosizione, IStatoPratiche
    {

        #region IStatoPratiche
        public StatoPratica StatoPratica { get; set; }
        public List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> ElencoStatoPratiche { get; set; }
        #endregion IStatoPratiche

        #region IRicercaPosizione
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
        #endregion IRicercaPosizione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        private String m_strSortExpPratica;
        private SortDirection m_SortDirectionPratica;
        private const string ASCENDING = " ASC";
        private const string DESCENDING = " DESC";

        #region Events
        public event EventHandler EliminaPraticaEvent;
        public event EventHandler ReloadUChangeSede;

        protected void RaiseEliminaPratica(object sender, EventArgs e)
        {
            EliminaPraticaEvent(this, null);
        }

        protected void RaiseReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede(sender, e);
        }
        #endregion Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);

            if (!Page.IsPostBack)
            {
                try
                {
                    List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche = (List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica>)Session["Pratiche"];
                    GvPratiche_Load(Pratiche);
                }
                catch (DnaExceptionBase)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo PageLoad" + ex);
                }
                ValorizzaEtichetteRisultatoRicerca();

            }
            else
            {
                if (null != ViewState["sortExpPratica"])
                {
                    m_strSortExpPratica = ViewState["sortExpPratica"] as String;
                }
                if (null != ViewState["sortDirectionPratica"])
                {
                    m_SortDirectionPratica = (SortDirection)ViewState["sortDirectionPratica"];
                }
            }
        }

        private void ValorizzaEtichetteRisultatoRicerca()
        {
            try
            {
                Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
                List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche = (List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica>)Session["Pratiche"];

                HdnSedeOperatore.Value = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0');

                if (Session["NumeroCriteri"] != null)
                {
                    lblNCriteriSelezionati.Text = ((int)Session["NumeroCriteri"]).ToString();
                }
                if (Session["VSPNumeroDomanda"] != null)
                {
                    lblParametriNumeroDomanda.Text = (string)Session["VSPNumeroDomanda"];
                    divParametriNumeroDomanda.Visible = true;
                }
                if (Session["VSPCategoriaPensione"] != null)
                {
                    lblParametriCategoriaPensione.Text = (string)Session["VSPCategoriaPensione"];
                    divParametriCategoriaPensione.Visible = true;
                }
                if (Session["VSPStatoPratica"] != null)
                {
                    lblParametriStatoPratica.Text = (string)Session["VSPStatoPratica"];
                    divParametriStatoPratica.Visible = true;
                }
                if (Session["VSPSede"] != null)
                {
                    lblParametriSede.Text = (string)Session["VSPSede"];
                    divParametriSede.Visible = true;
                }
                if (Session["VSPFondo"] != null)
                {
                    lblParametriFondo.Text = (string)Session["VSPFondo"];
                    divParametriFondo.Visible = true;
                }
                if (Session["VSPCassa"] != null)
                {
                    lblParametriCassa.Text = (string)Session["VSPCassa"];
                    divParametriCassa.Visible = true;
                }
                if (Session["VSPNome"] != null)
                {
                    lblParametriNome.Text = (string)Session["VSPNome"];
                    divParametriAnagrafica.Visible = true;
                }
                if (Session["VSPCognome"] != null)
                {
                    lblParametriCognome.Text = (string)Session["VSPCognome"];
                }
                if (Session["VSPCodiceFiscale"] != null)
                {
                    lblParametriCodiceFiscale.Text = (string)Session["VSPCodiceFiscale"];
                    divParametriCodiceFiscale.Visible = true;
                }
                if (Session["VSPDataPresentazioneDal"] != null)
                {
                    lblParametriDataPresentazioneDal.Text = (string)Session["VSPDataPresentazioneDal"];
                    divParametriDataPresentazione.Visible = true;
                }
                if (Session["VSPDataPresentazioneAl"] != null)
                {
                    lblParametriDataPresentazioneAl.Text = (string)Session["VSPDataPresentazioneAl"];

                }
                if (Session["VSPDataElaborazioneDal"] != null)
                {
                    lblParametriDataElaborazioneDal.Text = (string)Session["VSPDataElaborazioneDal"];
                    divParametriDataElaborazione.Visible = true;
                }
                if (Session["VSPDataElaborazioneAl"] != null)
                {
                    lblParametriDataElaborazioneAl.Text = (string)Session["VSPDataElaborazioneAl"];
                }
                if (Session["VSPMatricola"] != null)
                {
                    lblParametriMatricola.Text = (string)Session["VSPMatricola"];
                    divParametriMatricola.Visible = true;
                }
                if (Session["VSPTipoDomandaInLavorazione"] != null)
                {
                    switch (Session["VSPTipoDomandaInLavorazione"].ToString())
                    {
                        case "ALL":
                            lblParametriTipoDomandaInLavorazione.Text = "PL e RIC";
                            divParametriTipoDomandaInLavorazione.Visible = true;
                            break;
                        default:
                            lblParametriTipoDomandaInLavorazione.Text = Session["VSPTipoDomandaInLavorazione"].ToString();
                            divParametriTipoDomandaInLavorazione.Visible = true;
                            break;
                    }
                }
                if (Session["VSPTipoDomandaLavorata"] != null)
                {
                    switch (Session["VSPTipoDomandaLavorata"].ToString())
                    {
                        case "ALL":
                            lblParametriTipoDomandaLavorata.Text = "PL e RIC";
                            divParametriTipoDomandaLavorata.Visible = true;
                            break;
                        default:
                            lblParametriTipoDomandaLavorata.Text = Session["VSPTipoDomandaLavorata"].ToString();
                            divParametriTipoDomandaLavorata.Visible = true;
                            break;
                    }
                }
                if (Session["VSPGruppo"] != null)
                {
                    lblParametriGruppo.Text = Session["VSPGruppo"].ToString();
                    divParametriGruppo.Visible = true;
                }
                if (Session["VSPProdotto"] != null)
                {
                    lblParametriProdotto.Text = Session["VSPProdotto"].ToString();
                    divParametriProdotto.Visible = true;
                }
                if (Session["VSPTipo"] != null)
                {
                    lblParametriTipo.Text = Session["VSPTipo"].ToString();
                    divParametriTipo.Visible = true;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo ValorizzaEtichetteRisultatoRicerca" + ex);
            }
        }

        private void GvPratiche_Load(List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche)
        {
            try
            {
                if (Session["Pratiche"] == null)
                    Session.Add("Pratiche", Pratiche);
                else
                    Session["Pratiche"] = Pratiche;

                lblNPraticheTrovate.Text = Pratiche != null ? Pratiche.Count.ToString() : "0";
                gvPratiche.DataSource = Pratiche;
                gvPratiche.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo GvPratiche_Load" + ex);
            }

        }

        protected void GvPratiche_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvPratiche.Columns[colonnegvPratiche.Delete.GetHashCode()].Visible = false;
                gvPratiche.Columns[colonnegvPratiche.Stampa.GetHashCode()].Visible = false;
                gvPratiche.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche = (List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica>)Session["Pratiche"];
                GvPratiche_Load(Pratiche);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo gvPratiche_onPageIndexChanging" + ex);
            }
        }

        protected void GvPratiche_onSorting(Object sender, GridViewSortEventArgs e)
        {
            try
            {
                gvPratiche.Columns[colonnegvPratiche.Delete.GetHashCode()].Visible = false;
                gvPratiche.Columns[colonnegvPratiche.Stampa.GetHashCode()].Visible = false;
                string sortExpression = e.SortExpression;
                List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche = (List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica>)Session["Pratiche"];


                if (GridViewSortDirectionPratiche == SortDirection.Ascending)
                {
                    GridViewSortDirectionPratiche = SortDirection.Descending;
                    SortGridViewPratiche(sortExpression, DESCENDING);
                }
                else
                {
                    GridViewSortDirectionPratiche = SortDirection.Ascending;
                    SortGridViewPratiche(sortExpression, ASCENDING);
                }

                ViewState["sortDirectionPratica"] = m_SortDirectionPratica = GridViewSortDirectionPratiche;
                ViewState["sortExpDomanda"] = m_strSortExpPratica = e.SortExpression;
                GvPratiche_Load(Pratiche);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo GvPratiche_onSorting" + ex);
            }
        }

        private SortDirection GridViewSortDirectionPratiche
        {
            get
            {
                if (ViewState["sortDirectionPratica"] == null)
                    ViewState["sortDirectionPratica"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirectionPratica"];

            }
            set { ViewState["sortDirectionPratica"] = value; }
        }


        private void SortGridViewPratiche(string sortExpression, string direction)
        {
            try
            {
                List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche = (List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica>)Session["Pratiche"];

                if (sortExpression == "NumeroDomanda")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.NumeroDomanda, y.NumeroDomanda, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.NumeroDomanda, y.NumeroDomanda, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "DescProdotto")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.DescProdotto, y.DescProdotto, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.DescProdotto, y.DescProdotto, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "Categoria")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Categoria, y.Categoria, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Categoria, y.Categoria, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "Cognome")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Cognome, y.Cognome, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Cognome, y.Cognome, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "Nome")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Nome, y.Nome, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Nome, y.Nome, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }

                else if (sortExpression == "SedeCO")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Sede + x.CentroOperativo, y.Sede + y.CentroOperativo, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Sede + x.CentroOperativo, y.Sede + y.CentroOperativo, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "Certificato")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Certificato, y.Certificato, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Certificato, y.Certificato, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "Stato")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Stato, y.Stato, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Stato, y.Stato, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }
                else if (sortExpression == "Tipo")
                {
                    if (direction == ASCENDING)
                        Pratiche.Sort((x, y) => string.Compare(x.Tipo, y.Tipo, true, System.Globalization.CultureInfo.InvariantCulture));
                    else
                    {
                        Pratiche.Sort((x, y) => string.Compare(x.Tipo, y.Tipo, true, System.Globalization.CultureInfo.InvariantCulture));
                        Pratiche.Reverse();
                    }
                }

                GvPratiche_Load(Pratiche);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo SortGridView" + ex);
            }
        }

        protected void GvPratiche_onRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica> Pratiche = (List<Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica>)Session["Pratiche"];
            if (e.CommandName == "EliminaPratica")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterVisualizzaStatoPratiche presenterVisualizzaStatoPratiche = new PresenterVisualizzaStatoPratiche();
                Presenter.SvrLiquidazione.AreaRispostaStatoPratica.DatiStatoPratica pratica = Pratiche.ElementAt(r.DataItemIndex);

                // controllo sul tipo di appartenenza dell'operatore
                if (pratica.TipoAppartenenza != (AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)TipoAppRuolo)
                {
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Messaggio = "Ruolo Utente non abilitato alla lavorazione della domanda.";
                    return;
                }
                else
                {
                    ucAvviso.Visible = false;
                    ucAvviso.Messaggio = "";
                }

                string sedeDomanda = pratica.Sede + pratica.CentroOperativo;
                string sedeOperatore = HdnSedeOperatore.Value;
                string messaggioVideo = string.Empty;

                if (sedeDomanda != sedeOperatore)
                {
                    CodeUtility areaDecodifica = new CodeUtility();
                    AreaDecodifica valoriDecodificati = areaDecodifica.GetValuesDecodifica();
                    AreaDecodifica.DatiDecSede[] elencoDecSede = valoriDecodificati.ElencoDecSede;

                    //31/01/2022: verifico se la sede della domanda è chiusa e si trova nella stessa provincia della sede di appartenenza dell'operatore
                    int sedeAppartenenzaOperatore = Utility.GetSedeDiAppartenenzaOperatore();
                    AreaDecodifica.DatiDecSede decSedeChiusa = null;
                    bool isSedeChiusaStessaProvinciaOperatore = false;
                    if (elencoDecSede != null && elencoDecSede.Count() > 0)
                    {
                        decSedeChiusa = elencoDecSede.ToList().FindAll(x => !String.IsNullOrEmpty(pratica.Sede) && !String.IsNullOrEmpty(x.CodProvincia) && pratica.Sede.PadLeft(4, '0').Substring(0, 2) == x.CodProvincia.PadLeft(3, '0').Substring(1, 2)
                             && !String.IsNullOrEmpty(x.CodZona) && pratica.Sede.PadLeft(4, '0').Substring(2, 2) == x.CodZona.PadLeft(3, '0').Substring(1, 2)
                             && !String.IsNullOrEmpty(pratica.CentroOperativo) && !String.IsNullOrEmpty(x.CodCentroOperativo) && pratica.CentroOperativo.PadLeft(2, '0').Substring(0, 2) == x.CodCentroOperativo.PadLeft(3, '0').Substring(1, 2)
                             && x.CodAttivitaSede.GetValueOrDefault() == '0').FirstOrDefault();
                        isSedeChiusaStessaProvinciaOperatore = (decSedeChiusa != null && !String.IsNullOrEmpty(decSedeChiusa.CodProvincia)) ? decSedeChiusa.CodProvincia.PadLeft(3, '0').Substring(1, 2) == sedeAppartenenzaOperatore.ToString().PadLeft(6, '0').Substring(0, 2) : false;
                    }

                    if (!CodeUtility.ChangeSede((Ruoli)Session["Ruolo"], sedeDomanda, isSedeChiusaStessaProvinciaOperatore, out messaggioVideo))
                    {
                        ucAvviso.Visible = true;
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = messaggioVideo;
                        return;
                    }

                    RaiseReloadUChangeSede(this, null);
                    HdnSedeOperatore.Value = INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode;
                }

                this.StatoPratica = new StatoPratica();
                this.StatoPratica.NumeroDomanda = pratica.NumeroDomanda;
                presenterVisualizzaStatoPratiche.EliminaPratica(this);

                int retCode = 0;
                string descErrore = string.Empty;
                if (HasError)
                {
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Messaggio = ErrorMessage;
                    retCode = 1;
                    descErrore = ErrorMessage;
                    Presenter.LogSicurezza.ScritturaLog(pratica.NumeroDomanda, pratica.TipoAppartenenza, int.Parse(ConfigurationManager.AppSettings["IDEVENTO-ELIMINAZIONE"]),
                        HttpContext.Current.Request.UserHostAddress, retCode, descErrore, pratica.CodiceFiscale, string.Empty);
                    return;
                }
                else
                {
                    ucAvviso.Visible = false;
                    ucAvviso.Messaggio = "";
                    Presenter.LogSicurezza.ScritturaLog(pratica.NumeroDomanda, pratica.TipoAppartenenza, int.Parse(ConfigurationManager.AppSettings["IDEVENTO-ELIMINAZIONE"]),
                        HttpContext.Current.Request.UserHostAddress, retCode, descErrore, pratica.CodiceFiscale, string.Empty);
                }

                Pratiche.Remove(pratica);
                Session["Pratiche"] = Pratiche;
                GvPratiche_Load(Pratiche);
                RaiseEliminaPratica(this, null);
            }
        }

        protected string GetButtonImage(string iconName)
        {
            return "~/App_Themes/" + Page.Theme + "/Images/"+ iconName;
        }


        protected void GvPratiche_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if (e.Row.RowType == DataControlRowType.Header)
                {
                    if (String.Empty != m_strSortExpPratica)
                    {
                        AddSortImage(e.Row, "pratica");
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo GvPratiche_RowCreated" + ex);
            }


        }

        protected void btnConfermaPopUp_Click(object sender, EventArgs e)
        {
            string numeroDomanda = HdnNdomusSelezionato.Value;
            string progStorico = HdnProgStorico.Value;
            string sedeDomanda = HdnSedeDomanda.Value;
            string sedeOperatore = HdnSedeOperatore.Value;
            string tipoAppartenenzaPratica = HdnTipoAppartenenza.Value;
            string messaggioVideo = string.Empty;
            string stato = HdnStatoSelezionato.Value;

            if(stato == "NON LAVORABILE")
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = "Stato domanda 'NON LAVORABILE'. E' necessario eliminare e riacquisire.";
                return;
            }

            if (tipoAppartenenzaPratica != ((AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp)TipoAppRuolo).ToString())
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = "Ruolo Utente non abilitato alla lavorazione della domanda.";
                return;
            }
            else
            {
                ucAvviso.Visible = false;
                ucAvviso.Messaggio = "";
            }

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
            this.RicercaPosizione.ProgStorico = progStorico;
            this.IsConsultazione = !string.IsNullOrEmpty(progStorico); // Se è presente il progStorico allora si vuole consultare una domanda calcolata
            //ENG - Bypass "ELIMINAZIONE_CONTROLLO_SEDE"
            this.IsPaginaVisualizzazioneStatoPratiche = true;
           
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

            Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda result = this.ElencoDomande.Find(
                        delegate(Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda)
                        {
                            return domanda.NumeroDomanda == this.RicercaPosizione.Domanda;
                        }
                        );

            Session["Domanda"] = result;
            Session["Anagrafica"] = this.RiepilogoAnagrafica;
            Session["EsitoCalcolo"] = this.EsitoCalcolo;
            Session.Remove("Semaforo");
            Session.Remove("DatiPensione");
            Session.Remove("Lavorabile");

            if (result.Stato == "DA ACQUISIRE")
                Presenter.LogSicurezza.ScritturaLog(result.NumeroDomanda, result.TipoAppartenenza,
                int.Parse(ConfigurationManager.AppSettings["IDEVENTO-ACQUISIZIONE"]), HttpContext.Current.Request.UserHostAddress, 0, string.Empty,
                this.RiepilogoAnagrafica.CodiceFiscale, string.Empty);
            else
                Presenter.LogSicurezza.ScritturaLog(result.NumeroDomanda, result.TipoAppartenenza,
                int.Parse(ConfigurationManager.AppSettings["IDEVENTO-CONSULTAZIONE"]), HttpContext.Current.Request.UserHostAddress, 0, string.Empty,
                this.RiepilogoAnagrafica.CodiceFiscale, string.Empty);

            Response.Redirect("ElaborazionePosizione/PosizioneSelezionata.aspx", false);
        }

        private void AddSortImage(GridViewRow headerRow, String gvType)
        {
            try
            {
                Int32 iCol;
                //                if (gvType == "pratica")
                iCol = GetSortColumnIndex(m_strSortExpPratica, gvType);
                if (-1 == iCol)
                {
                    return;
                }
                // Create the sorting image based on the sort direction.   
                System.Web.UI.WebControls.Image sortImage = new System.Web.UI.WebControls.Image();
                if (gvType == "pratica")
                {
                    string currentTheme = Page.Theme;
                    if (SortDirection.Ascending == m_SortDirectionPratica)
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
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo AddSortImage" + ex);
            }

        }

        private int GetSortColumnIndex(String strCol, String gvType)
        {
            try
            {
                if (gvType == "pratica")
                {
                    foreach (DataControlField field in gvPratiche.Columns)
                    {
                        if (field.SortExpression == strCol)
                        {
                            return gvPratiche.Columns.IndexOf(field);
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
                throw new INPS.DNA.DnaApplicationException("UCVisualizzaStatoPratiche, Errore nel metodo GetSortColumnIndex" + ex);
            }

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Session["FlagBack"] = true;
            Response.Redirect("~/VisualizzazioneStatoPratiche.aspx");
        }

        protected object IsVisibleDelete(GridViewRow row, object trueResult, object falseResult)
        {
            AreaRispostaStatoPratica.DatiStatoPratica statoPratica = (AreaRispostaStatoPratica.DatiStatoPratica)row.DataItem;
            if (statoPratica.Stato == "CALCOLATA" || statoPratica.Stato == "CALCOLO NO WEBDOM" ||
                statoPratica.Stato == "CALCOLO NO FELPE" || statoPratica.Stato == "CALCOLO NO ONERI" ||
                statoPratica.Stato == "CALCOLO NO SAI" || statoPratica.Stato == "CALCOLO NO SIN" || statoPratica.Stato == "CALCOLO NO TOTAL" ||
                statoPratica.Stato == "CALCOLO NO TOT" || statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoStazLavoro) || statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoNoteDebito) || statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNo6Scatti) || statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoEquoInd) || statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcolataNoIndennSpec) ||
                statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcoloNoInd) ||
                statoPratica.Stato == Utility.GetDescription(CodeUtility.StatoPensione.CalcoloNoIndWait))
                return false;

            gvPratiche.Columns[colonnegvPratiche.Delete.GetHashCode()].Visible = true;
            return true;
        }

        protected object IsVisibleStampa(GridViewRow row)
        {
            AreaRispostaStatoPratica.DatiStatoPratica statoPratica = (AreaRispostaStatoPratica.DatiStatoPratica)row.DataItem;

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
            }

            gvPratiche.Columns[colonnegvPratiche.Stampa.GetHashCode()].Visible = true;
            return true;
        }

        public string GetSedeForView(object dataItem)
        {
            string strSede = string.Empty;
            if (dataItem as AreaRispostaStatoPratica.DatiStatoPratica != null)
            {
                AreaRispostaStatoPratica.DatiStatoPratica statoPratica = (AreaRispostaStatoPratica.DatiStatoPratica)dataItem;
                if (TipoAppRuolo == UtilityTipoAppartenenza.AGO && !string.IsNullOrEmpty(statoPratica.SedeDestinazione))
                    strSede = statoPratica.SedeDestinazione + statoPratica.CentroOperativoDestinazione;
                else
                    strSede = statoPratica.Sede + statoPratica.CentroOperativo;
            }
            return strSede;
        }
    }

    enum colonnegvPratiche
    {
        Stampa = 9,
        Delete = 10
    }
}
