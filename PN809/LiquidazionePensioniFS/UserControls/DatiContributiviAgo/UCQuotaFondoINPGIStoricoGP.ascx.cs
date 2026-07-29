using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCQuotaFondoINPGIStoricoGP : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione
    {
        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviAgo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region Enum
        public enum ColonneGvDatiRetributiviINPGI { Sett707 = 5, Importo707 = 6 };
        #endregion Enum

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        public void ValorizzaEtichetteQuotaFondoINPGI(IDatiContributiviAgo Dati)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            ViewState["areaDatiContributiviAgo"] = Dati.areaDatiContributiviAgo;

            //ENG - Aggiornamento Memo INPGI            
            string ctrlAbilitazioneMemoINPGI_20240307 = string.Empty;
            if (ViewState["AbilitazioneModificheMemoINPGI_20240307"] != null)
                ctrlAbilitazioneMemoINPGI_20240307 = (string)ViewState["AbilitazioneModificheMemoINPGI_20240307"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneModificheMemoINPGI_20240307", out ctrlAbilitazioneMemoINPGI_20240307);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneModificheMemoINPGI_20240307"] = ctrlAbilitazioneMemoINPGI_20240307;
            }

            LoadDecodificaData(Dati);
            BindDataForPanels(Dati.areaDatiContributiviAgo);

            //ENG - RIC VOPGI NON CONTRIBUTIVE
            if ((Utility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaVOPGI(this.domanda.Categoria)) || Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ||
                (((Utility.IsRicostituzione(datiPensione) && Utility.IsDomandaINPGI(this.domanda.Categoria)) ||
                (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && datiPensione.GP1AV91B == "2"))
                lblRicNonContrib.Visible = true;

            //ENG - Aggiornamento Memo INPGI
            if (!String.IsNullOrEmpty(ctrlAbilitazioneMemoINPGI_20240307) && ctrlAbilitazioneMemoINPGI_20240307.Trim().ToUpperInvariant() == "SI")
            {
                if (Utility.IsDomandaVOPGI_AGI(this.domanda.Categoria, datiPensione.Filtro, datiPensione.DirittoAutonomo, datiPensione.GP1AJ11) &&
                    datiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
                {
                    divContributiviINPGI.Visible = false;
                }
            }

            //ENG - INPGI migrate
            if ((((Utility.IsRicostituzione(datiPensione) || Utility.IsDomandaRipristino(datiPensione)) && Utility.IsDomandaINPGI(this.domanda.Categoria)) ||
                (!this.domanda.IsDomandaRiapertura && this.domanda.Categoria.Trim().ToUpperInvariant() == "SOPGI" && Utility.IsDomandaReversibilita(datiPensione))) && datiPensione.GP1AV91B == "2")
            {
                divContributiviINPGI.Visible = true;
                divRetributiviINPGI.Visible = false;
                gvContributiviINPGI.Columns[1].Visible = false;
                lblDatiContributivi.Text = "Pensione Mensile alla decorrenza:";
                lblRicNonContrib.Visible = false;
                gvContributiviINPGI.Columns[3].HeaderText = "Pensione mensile";
                gvContributiviINPGI.DataBind();
            }
        }

        private void BindDataForPanels(AreaDatiContributivi areaDatiContributivi)
        {
            if (areaDatiContributivi != null)
            {
                divRetributiviINPGI.Visible = true;
                divContributiviINPGI.Visible = true;
                InitBindDataRetributivi();
                InitBindDataContributivi();
                ValorizzaPeriodiPerDecodificaGestioneRetrib();
                ValorizzaPeriodiPerDecodificaGestioneContrib();
            }
        }

        private void InitBindDataContributivi()
        {
            List<DatiContributiviQuotaFondoINPGIStoricoGPLocal> elencoDatiContributivi = new List<DatiContributiviQuotaFondoINPGIStoricoGPLocal>();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoINPGIStorico.lDatiContributiviQuotaFondoINPGI != null)
                elencoDatiContributivi = MapDatiContributiviForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            gvContributiviINPGI.DataSource = elencoDatiContributivi;
            ViewState[EnumViewState.ElencoDatiContributiviQuotaFondoINPGI.ToString()] = elencoDatiContributivi;


            gvContributiviINPGI.DataBind();

        }

        private static List<DatiContributiviQuotaFondoINPGIStoricoGPLocal> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributiviQuotaFondoINPGIStoricoGPLocal> elencoContributiviQuotaFondoINPGI = new List<DatiContributiviQuotaFondoINPGIStoricoGPLocal>();
            foreach (GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI contr in areaDatiContributivi.DatiQuotaFondoINPGIStorico.lDatiContributiviQuotaFondoINPGI.ToList<GestioneQuotaFondoINPGIDatiCalcoloContributivoINPGI>())
            {
                string quota = string.Empty;
                string montante = string.Empty;

                quota = contr.Quota.HasValue ? contr.Quota.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                montante = contr.Montante.HasValue ? contr.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                elencoContributiviQuotaFondoINPGI.Add(new DatiContributiviQuotaFondoINPGIStoricoGPLocal(contr.CodiceGestione.HasValue ? contr.CodiceGestione.Value.ToString() : string.Empty, montante,
                    quota, contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty));
            }
            return elencoContributiviQuotaFondoINPGI;
        }

        private void InitBindDataRetributivi()
        {
            List<DatiRetributiviQuotaFondoINPGIStoricoGPLocal> elencoDatiRetributivi = new List<DatiRetributiviQuotaFondoINPGIStoricoGPLocal>();

            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoINPGIStorico.lDatiRetributiviQuotaFondoINPGI != null)
                elencoDatiRetributivi = MapDatiRetributiviForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            gvRetributiviINPGI.DataSource = elencoDatiRetributivi;
            ViewState[EnumViewState.ElencoDatiRetributiviQuotaFondoINPGI.ToString()] = elencoDatiRetributivi;


            gvRetributiviINPGI.DataBind();
        }

        private static List<DatiRetributiviQuotaFondoINPGIStoricoGPLocal> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviQuotaFondoINPGIStoricoGPLocal> elencoRetributiviQuotaFondoINPGI = new List<DatiRetributiviQuotaFondoINPGIStoricoGPLocal>();
            foreach (GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI contr in areaDatiContributivi.DatiQuotaFondoINPGIStorico.lDatiRetributiviQuotaFondoINPGI.ToList<GestioneQuotaFondoINPGIDatiCalcoloRetributivoINPGI>())
            {
                string quota = string.Empty;
                string importoCalcolato = string.Empty;
                string importoComma707 = string.Empty;
                string retribuzioneMediaSettimanale = string.Empty;

                importoCalcolato = contr.ImportoCalcolato.HasValue ? contr.ImportoCalcolato.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                importoComma707 = contr.ImportoComma707.HasValue ? contr.ImportoComma707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                retribuzioneMediaSettimanale = contr.RetribuzioneMediaSettimanale.HasValue ? contr.RetribuzioneMediaSettimanale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                elencoRetributiviQuotaFondoINPGI.Add(new DatiRetributiviQuotaFondoINPGIStoricoGPLocal(contr.CodiceGestione.HasValue ? contr.CodiceGestione.Value.ToString() : string.Empty, contr.Settimane.HasValue ? contr.Settimane.Value.ToString() : string.Empty,
                    quota, importoCalcolato, importoComma707, contr.SettimaneComma707.HasValue ? contr.SettimaneComma707.Value.ToString() : string.Empty, retribuzioneMediaSettimanale));
            }
            return elencoRetributiviQuotaFondoINPGI;
        }

        protected void gvContributiviINPGI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    gvContributiviINPGI.EditIndex = -1;
                    ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                    ((Label)e.Row.FindControl("lblQuota")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Quota) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Quota, 4) : "";
                    ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Montante, 4) : "";
                    ((Label)e.Row.FindControl("lblPeriodoContr")).Text = GetValueFromIdGestione(((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblSettimaneContr")).Text = ((DatiContributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Settimane;

                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvContributiviINPGI_RowDataBound " + ex);
            }
        }

        protected void gvRetributiviINPGI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    gvRetributiviINPGI.EditIndex = -1;
                    ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text = GetValueFromId(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneRetribQuotaFondo_item")).Text);
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Settimane;
                    ((Label)e.Row.FindControl("lblPeriodoRetr")).Text = GetValueFromIdGestione(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).Gestione);
                    ((Label)e.Row.FindControl("lblImportoCalcolato")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).ImportoCalcolato) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).ImportoCalcolato, 4) : "";
                    ((Label)e.Row.FindControl("lblImportoComma707")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).ImportoComma707) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).ImportoComma707, 4) : "";
                    ((Label)e.Row.FindControl("lblSettimaneComma707")).Text = ((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).SettimaneComma707;
                    ((Label)e.Row.FindControl("lblRetribuzioneMediaSettimanale")).Text = !string.IsNullOrEmpty(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale) ? CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviQuotaFondoINPGIStoricoGPLocal)(e.Row.DataItem)).RetribuzioneMediaSettimanale, 4) : "";

                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvRetributiviINPGI_RowDataBound " + ex);
            }
        }

        private void LoadDecodificaData(IDatiContributiviAgo areaDatiContributivi)
        {
            if (areaDatiContributivi.areaDatiContributiviAgo != null)
            {
                ViewState["listaCodeGestioneQuotaFondoINPGI"] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneQuotaFondoINPGI;
            }
        }

        private string GetValueFromId(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneQuotaFondoINPGI = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
                DecodificaGestioneQuotaFondoINPGI app = listaCodeGestioneQuotaFondoINPGI.ToList().Find(delegate(DecodificaGestioneQuotaFondoINPGI code) { return (code.Id == index); });
                if (app != null)
                    ret = app.Descrizione;
            }
            return ret;
        }

        private string GetValueFromIdGestione(string idGestione)
        {
            string periodo = string.Empty;
            DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcolo = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
            if (!string.IsNullOrEmpty(idGestione))
            {
                DecodificaGestioneQuotaFondoINPGI gestione = listaCodeGestioneCalcolo.Where(x => x.Id == long.Parse(idGestione)).Select(x => x).FirstOrDefault();
                if (gestione != null)
                {
                    if (!gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                        periodo = "Fino a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                    else if (gestione.PeriodoDal.HasValue && !gestione.PeriodoAl.HasValue)
                        periodo = "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " in poi";
                    else if (gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                        periodo = "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                }
            }
            return periodo;
        }

        private void ValorizzaPeriodiPerDecodificaGestioneRetrib()
        {
            DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcolo = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
            IEnumerable<DecodificaGestioneQuotaFondoINPGI> listaGestioneRetrib = listaCodeGestioneCalcolo.Select(x => x).Where(x => x.TipoQuota == "R").ToList();

            //ENG - Aggiornamento Memo INPGI - Necessario per risolvere la problematica relativa all'inserimento di una nuova quota sulla tabella di decodifica. I periodi devono seguire lo stesso
            //ordinamento delle quote
            if (ViewState["AbilitazioneModificheMemoINPGI_20240307"] != null && ViewState["AbilitazioneModificheMemoINPGI_20240307"].ToString().Trim().ToUpperInvariant() == "SI")
            {
                listaGestioneRetrib = listaGestioneRetrib.OrderBy(x => x.Descrizione);
            }

            string periodo = string.Empty;
            foreach (DecodificaGestioneQuotaFondoINPGI gestione in listaGestioneRetrib)
            {
                if (!gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Fino a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                else if (gestione.PeriodoDal.HasValue && !gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " in poi";
                else
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                periodo = periodo + ";";
            }
            hdnPeriodiRetrib.Value = periodo;
        }

        private void ValorizzaPeriodiPerDecodificaGestioneContrib()
        {
            DecodificaGestioneQuotaFondoINPGI[] listaCodeGestioneCalcolo = (DecodificaGestioneQuotaFondoINPGI[])ViewState["listaCodeGestioneQuotaFondoINPGI"];
            IEnumerable<DecodificaGestioneQuotaFondoINPGI> listaGestioneContrib = listaCodeGestioneCalcolo.Select(x => x).Where(x => x.TipoQuota == "C").ToList();
            string periodo = string.Empty;
            foreach (DecodificaGestioneQuotaFondoINPGI gestione in listaGestioneContrib)
            {
                if (!gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Fino a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                else if (gestione.PeriodoDal.HasValue && !gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " in poi";
                else if (gestione.PeriodoDal.HasValue && gestione.PeriodoAl.HasValue)
                    periodo = periodo + "Da " + gestione.PeriodoDal.Value.ToString("MM/yyyy") + " a " + gestione.PeriodoAl.Value.ToString("MM/yyyy");
                periodo = periodo + ";";
            }
            hdnPeriodiContrib.Value = periodo;
        }

        protected void gvRetributiviINPGI_Load(object sender, EventArgs e)
        {
            AreaDatiContributivi areaDatiContributiviAgo = ViewState["areaDatiContributiviAgo"] as AreaDatiContributivi;
            if (areaDatiContributiviAgo != null)
            {
                gvRetributiviINPGI.Columns[(int)ColonneGvDatiRetributiviINPGI.Sett707].Visible = areaDatiContributiviAgo.IsSettimane707INPGIVisible;
                gvRetributiviINPGI.Columns[(int)ColonneGvDatiRetributiviINPGI.Importo707].Visible = areaDatiContributiviAgo.IsSettimane707INPGIVisible;
            }
        }

        #region Enums
        public enum EnumViewState
        {
            ElencoDatiContributiviQuotaFondoINPGI,
            ElencoDatiRetributiviQuotaFondoINPGI
        }
        #endregion Enums
    }

    #region nested Class
    [Serializable]
    public class DatiContributiviQuotaFondoINPGIStoricoGPLocal
    {
        public DatiContributiviQuotaFondoINPGIStoricoGPLocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiContributiviQuotaFondoINPGIStoricoGPLocal(string strGestione, string strMontante, string strQuota, string strSettimane)
        {
            this.Id = Guid.NewGuid();
            this._strMontante = strMontante;
            this._strGestione = strGestione;
            this._strQuota = strQuota;
            this._strSettimane = strSettimane;
        }
        #region private properties
        private string _strMontante;
        private string _strGestione;
        private string _strQuota;
        private string _strSettimane;
        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Montante { get { return _strMontante; } set { _strMontante = value; } }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        #endregion public properties

    }

    [Serializable]
    public class DatiRetributiviQuotaFondoINPGIStoricoGPLocal
    {
        public DatiRetributiviQuotaFondoINPGIStoricoGPLocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiRetributiviQuotaFondoINPGIStoricoGPLocal(string strGestione, string strSettimane, string strQuota, string strImportoCalcolato, string strImportoComma707, string strSettimaneComma707, string strRetribuzioneMediaSettimanale)
        {
            this.Id = Guid.NewGuid();
            this._strSettimane = strSettimane;
            this._strGestione = strGestione;
            this._strQuota = strQuota;
            this._strImportoCalcolato = strImportoCalcolato;
            this._strImportoComma707 = strImportoComma707;
            this._strSettimaneComma707 = strSettimaneComma707;
            this._strRetribuzioneMediaSettimanale = strRetribuzioneMediaSettimanale;
        }
        #region private properties
        private string _strSettimane;
        private string _strGestione;
        private string _strQuota;
        private string _strImportoCalcolato;
        private string _strImportoComma707;
        private string _strSettimaneComma707;
        private string _strRetribuzioneMediaSettimanale;
        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string ImportoCalcolato { get { return _strImportoCalcolato; } set { _strImportoCalcolato = value; } }
        public string ImportoComma707 { get { return _strImportoComma707; } set { _strImportoComma707 = value; } }
        public string SettimaneComma707 { get { return _strSettimaneComma707; } set { _strSettimaneComma707 = value; } }
        public string RetribuzioneMediaSettimanale { get { return _strRetribuzioneMediaSettimanale; } set { _strRetribuzioneMediaSettimanale = value; } }
        #endregion public properties
    }
    #endregion nested Class
}