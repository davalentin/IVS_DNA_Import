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
    public partial class UCQuotaFondoIntegrativo : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione
    {
        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        List<DatiQuotaFondoIntegrativoLocal> elencoQuotaFondoIntViewState = new List<DatiQuotaFondoIntegrativoLocal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        public void ValorizzaEtichetteQuotaFondoIntegrativo(IDatiContributiviAgo Dati)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);

            ViewState["areaDatiContributiviAgo"] = Dati.areaDatiContributiviAgo;

            List<DatiQuotaFondoIntegrativoLocal> elencoDatiQuotaFondoIntegrativoLocal = new List<DatiQuotaFondoIntegrativoLocal>();

            if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoIntegrativo != null &&
                ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo != null)
                elencoDatiQuotaFondoIntegrativoLocal = MapDatiQuotaFondoIntegrativoForView((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]);

            if (!((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe)
            {
                DatiQuotaFondoIntegrativoLocal Empty = elencoDatiQuotaFondoIntegrativoLocal.Find(delegate(DatiQuotaFondoIntegrativoLocal code)
                {
                    return (code.Ammontare == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty &&
                            code.Montante == string.Empty && code.Settimane == string.Empty);
                });

                if (Empty == null)
                    elencoDatiQuotaFondoIntegrativoLocal.Add(new DatiQuotaFondoIntegrativoLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            }

            ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()] = elencoDatiQuotaFondoIntegrativoLocal;

            if ((CodeUtility.IsRicostituzione(datiPensione) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) || Utility.IsDomandaRipristino(datiPensione))
                btnEliminaQuotaFondoIntegrativo.Enabled = false;

            LoadDecodificaData(Dati);
            GvQuotaFondoIntegrativo_Load();
        }

        private static List<DatiQuotaFondoIntegrativoLocal> MapDatiQuotaFondoIntegrativoForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiQuotaFondoIntegrativoLocal> elencoDatiQuotaFondoIntegrativo = new List<DatiQuotaFondoIntegrativoLocal>();
            foreach (GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo contr in areaDatiContributivi.DatiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo.ToList<GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo>())
            {
                string settimana = string.Empty;
                string importo = string.Empty;
                string montante = string.Empty;
                string PL_Quotac = string.Empty;
                if (contr.Quota.HasValue)
                {
                    if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        settimana = contr.NSettimane.HasValue ? contr.NSettimane.Value.ToString() : string.Empty;
                        importo = contr.ImportoContributivoTotale.HasValue ? contr.ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.Montante.HasValue ? contr.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    else if (contr.Quota.HasValue && contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        settimana = contr.NSettimaneQuotaD.HasValue ? contr.NSettimaneQuotaD.Value.ToString() : string.Empty;
                        importo = contr.ImportoContribTotaleQuotaD.HasValue ? contr.ImportoContribTotaleQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        montante = contr.MontanteQuotaD.HasValue ? contr.MontanteQuotaD.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    }
                    PL_Quotac = contr.PL_Quotac.HasValue ? contr.PL_Quotac.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                    elencoDatiQuotaFondoIntegrativo.Add(new DatiQuotaFondoIntegrativoLocal(contr.CodiceGestione.HasValue ? contr.CodiceGestione.Value.ToString() : string.Empty, contr.Quota.HasValue ? contr.Quota.Value.ToString() : string.Empty,
                        settimana, importo, montante, PL_Quotac));
                }
            }
            return elencoDatiQuotaFondoIntegrativo;
        }

        public void btnSalvaQuotaFondoIntegrativo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviAgo = new AreaDatiContributivi();

            RecuperaCampi(this.areaDatiContributiviAgo);

            if (this.areaDatiContributiviAgo.DatiQuotaFondoIntegrativo != null &&
                this.areaDatiContributiviAgo.DatiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo.Count() > 0)
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.SalvaDatiCalcoloQuotaFondoIntegrativo(this);

                btnEliminaQuotaFondoIntegrativo.Enabled = true;
                ViewState["areaDatiContributiviAgo"] = this.areaDatiContributiviAgo;

            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Quota Fondo Integrativo da salvare";
            }

            if (this.HasError)
            {
                RaiseHideAvviso(this, null);
                RaiseShowAvviso(this, null);
            }
            else
            {
                this.ErrorMessage = "Dati Quota Fondo Integrativo salvati correttamente.";
                RaiseHideAvviso(this, null);
                RaiseShowAvviso(this, null);
            }
        }

        public void btnEliminaQuotaFondoIntegrativo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiQuotaFondoIntegrativoLocal> listaDatiQuotaFondoIntApp = (List<DatiQuotaFondoIntegrativoLocal>)ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()];

            if (listaDatiQuotaFondoIntApp != null && listaDatiQuotaFondoIntApp.Count() > 0)
            {
                PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
                presenterDatiContributiviAgo.EliminaDatiCalcoloQuotaFondoIntegrativo(this);

                if (!this.HasError)
                {
                    if ((listaDatiQuotaFondoIntApp != null && listaDatiQuotaFondoIntApp.Count() > 0))
                        modalitaEdit.Value = "false";
                    InitializeData(this, null);
                }
            }
            else
            {
                this.HasError = true;
                this.ErrorMessage = "Non ci sono Dati Quota Fondo Integrativo da eliminare";
            }

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Quota Fondo Integrativo eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiQuotaFondoIntegrativoLocal> listaDatiQuotaApp = (List<DatiQuotaFondoIntegrativoLocal>)ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()];
            areaDatiContributiviAgo.DatiQuotaFondoIntegrativo = new GestioneContribDatiQuotaFondoIntegrativo();
            areaDatiContributiviAgo.DatiQuotaFondoIntegrativo.IdPensione = ((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiQuotaFondoIntegrativo.IdPensione;
            if (listaDatiQuotaApp != null && listaDatiQuotaApp.Count() > 0)
            {
                List<GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo> listQuotaFondo = GetDataQuotaFondoIntegrativoToSave(listaDatiQuotaApp);
                int nDatiQuotaFondo = listQuotaFondo.Count();
                areaDatiContributiviAgo.DatiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo = new GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo[nDatiQuotaFondo];
                areaDatiContributiviAgo.DatiQuotaFondoIntegrativo.lDatiQuotaFondoIntegrativo = listQuotaFondo.ToArray();
            }
        }

        private List<GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo> GetDataQuotaFondoIntegrativoToSave(List<DatiQuotaFondoIntegrativoLocal> lDatiQuotaFondoIntegrativoLocal)
        {
            List<GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo> lContr = new List<GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo>();

            foreach (DatiQuotaFondoIntegrativoLocal datiQuotaFondoIntegrativoLocal in lDatiQuotaFondoIntegrativoLocal)
            {
                if (datiQuotaFondoIntegrativoLocal.Ammontare == string.Empty && datiQuotaFondoIntegrativoLocal.Gestione == string.Empty && datiQuotaFondoIntegrativoLocal.Quota == string.Empty &&
                    datiQuotaFondoIntegrativoLocal.Montante == string.Empty && datiQuotaFondoIntegrativoLocal.Settimane == string.Empty)
                    continue;

                GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo Contr = new GestioneQuotaFondoIntegrativoDatiQuotaFondoIntegrativo();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiQuotaFondoIntegrativoLocal.Gestione.Trim() != string.Empty)
                    Contr.CodiceGestione = Convert.ToInt64(datiQuotaFondoIntegrativoLocal.Gestione.Trim());
                else
                    Contr.CodiceGestione = null;

                Contr.Quota = !String.IsNullOrEmpty(datiQuotaFondoIntegrativoLocal.Quota) ? Convert.ToChar(datiQuotaFondoIntegrativoLocal.Quota) : (char?)null;

                if (Contr.Quota.HasValue && Contr.Quota.Value.ToString().ToUpperInvariant() == "C")
                {
                    Contr.ImportoContributivoTotale = datiQuotaFondoIntegrativoLocal.Ammontare.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoIntegrativoLocal.Ammontare.Trim()) : (decimal?)null;
                    Contr.Montante = datiQuotaFondoIntegrativoLocal.Montante.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoIntegrativoLocal.Montante.Trim()) : (decimal?)null;
                    Contr.NSettimane = datiQuotaFondoIntegrativoLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiQuotaFondoIntegrativoLocal.Settimane.Trim()) : (int?)null;
                }
                else if (Contr.Quota.HasValue && Contr.Quota.Value.ToString().ToUpperInvariant() == "D")
                {
                    Contr.ImportoContribTotaleQuotaD = datiQuotaFondoIntegrativoLocal.Ammontare.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoIntegrativoLocal.Ammontare.Trim()) : (decimal?)null;
                    Contr.MontanteQuotaD = datiQuotaFondoIntegrativoLocal.Montante.Trim() != string.Empty ? Convert.ToDecimal(datiQuotaFondoIntegrativoLocal.Montante.Trim()) : (decimal?)null;
                    Contr.NSettimaneQuotaD = datiQuotaFondoIntegrativoLocal.Settimane.Trim() != string.Empty ? Convert.ToInt32(datiQuotaFondoIntegrativoLocal.Settimane.Trim()) : (int?)null;
                }
                Contr.PL_Quotac = datiQuotaFondoIntegrativoLocal.PL_Quotac != null && datiQuotaFondoIntegrativoLocal.PL_Quotac != string.Empty ? Convert.ToDecimal(datiQuotaFondoIntegrativoLocal.PL_Quotac.Trim()) : (decimal?)null;

                lContr.Add(Contr);
            }
            return lContr;
        }

        protected void gvQuotaFondoIntegrativo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<DatiQuotaFondoIntegrativoLocal> listaDatiQuotaFondoIntApp = (List<DatiQuotaFondoIntegrativoLocal>)ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()];
            this.areaDatiContributiviAgo = (AreaDatiContributivi)ViewState["areaDatiContributiviAgo"];

            if (e.CommandName == "Elimina")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                listaDatiQuotaFondoIntApp.RemoveAt(r.DataItemIndex);
                ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()] = listaDatiQuotaFondoIntApp;

                GvQuotaFondoIntegrativo_Load();

            }
            else if (e.CommandName == "Edit")
            {
                this.modalitaEdit.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                if (!IsEmptyEditableRow((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    int index = r.DataItemIndex;

                    listaDatiQuotaFondoIntApp[index].Quota = ((DropDownList)r.FindControl("ddlQuota")).SelectedValue;
                    listaDatiQuotaFondoIntApp[index].Settimane = ((TextBox)r.FindControl("txtSettimane")).Text;
                    listaDatiQuotaFondoIntApp[index].Ammontare = ((TextBox)r.FindControl("txtAmmontare")).Text;
                    listaDatiQuotaFondoIntApp[index].Montante = ((TextBox)r.FindControl("txtMontante")).Text;
                    listaDatiQuotaFondoIntApp[index].Gestione = ((DropDownList)r.FindControl("ddlCodiceGestioneQuotaFondo")).SelectedValue;

                    // Sto inserendo un nuovo record
                    if (index == listaDatiQuotaFondoIntApp.Count - 1)
                        listaDatiQuotaFondoIntApp.Add(new DatiQuotaFondoIntegrativoLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvQuotaFondoIntegrativo.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()] = listaDatiQuotaFondoIntApp;
                    this.modalitaEdit.Value = "false";
                    btnSalvaQuotaFondoIntegrativo.Enabled = true;
                    btnEliminaQuotaFondoIntegrativo.Enabled = true;
                    RaiseGestisciTastoSalva(this, null);
                    GvQuotaFondoIntegrativo_Load();
                }
            }
            else if (e.CommandName == "Annulla")
            {
                if (!IsListaEmpty())
                {
                    this.modalitaEdit.Value = "false";
                    gvQuotaFondoIntegrativo.EditIndex = -1;
                    RaiseGestisciTastoSalva(this, null);
                    btnSalvaQuotaFondoIntegrativo.Enabled = true;
                    btnEliminaQuotaFondoIntegrativo.Enabled = true;
                    GvQuotaFondoIntegrativo_Load();
                }
            }
        }

        protected void gvQuotaFondoIntegrativo_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvQuotaFondoIntegrativo.EditIndex = -1;
                GvQuotaFondoIntegrativo_Load();
                btnSalvaQuotaFondoIntegrativo.Enabled = true;
                btnEliminaQuotaFondoIntegrativo.Enabled = true;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCQuotaFondoIntegrativo, Errore nel metodo gvQuotaFondoIntegrativo_RowCancelingEdit " + ex);
            }
        }

        protected void gvQuotaFondoIntegrativo_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvQuotaFondoIntegrativo.EditIndex = e.NewEditIndex;
                GvQuotaFondoIntegrativo_Load();
                btnSalvaQuotaFondoIntegrativo.Enabled = false;
                btnEliminaQuotaFondoIntegrativo.Enabled = false;
                RaiseGestisciTastoSalva(this, null);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuotaFondoIntegrativo, Errore nel metodo gvQuotaFondoIntegrativo_RowEditing " + ex);
            }
        }

        protected void gvQuotaFondoIntegrativo_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvQuotaFondoIntegrativo_DataBound(object sender, EventArgs e)
        {

        }

        protected void gvQuotaFondoIntegrativo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = null;
            datiPensione = GetDatiPensione(this);
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((AreaDatiContributivi)ViewState["areaDatiContributiviAgo"]).DatiCalcolo.IsUnicarpe || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
                    {
                        gvQuotaFondoIntegrativo.EditIndex = -1;

                        ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                        ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Quota;

                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblAmmontare")).Text = !string.IsNullOrEmpty(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Ammontare) ? CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Ammontare, 4) : "";
                        ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Montante, 4) : "";

                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    else if (e.Row.DataItemIndex == 0)    //prima riga
                    {
                        //vuota
                        if (IsListaEmpty() && !Convert.ToBoolean(modalitaEdit.Value))
                        {
                            gvQuotaFondoIntegrativo.EditIndex = 0;
                            modalitaEdit.Value = "true";
                            btnSalvaQuotaFondoIntegrativo.Enabled = false;
                            btnEliminaQuotaFondoIntegrativo.Enabled = false;
                            RaiseGestisciTastoSalva(this, null);

                            GvQuotaFondoIntegrativo_Load();
                        }
                        else if (IsEmptyEditableRow(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdl(e.Row);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaFondoIntegrativoAgo", Page.Theme);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteQuotaFondoIntegrativo")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                                ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Quota;

                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblAmmontare")).Text = !string.IsNullOrEmpty(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Ammontare) ? CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Ammontare, 4) : "";
                                ((Label)e.Row.FindControl("lblMontante")).Text = !string.IsNullOrEmpty(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Montante) ? CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Montante, 4) : "";

                                //if (CodeUtility.IsRicostituzione(datiPensione))
                                //    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "");
                                //else
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaFondoIntegrativo");
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdl(e.Row);
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaFondoIntegrativoAgo", Page.Theme);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                                ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Quota;

                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblAmmontare")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Ammontare, 4);
                                ((Label)e.Row.FindControl("lblMontante")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Montante, 4);

                                //if (CodeUtility.IsRicostituzione(datiPensione))
                                //    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "");
                                //else
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaFondoIntegrativo");
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            GestioneDdl(e.Row);
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabQuotaFondoIntegrativoAgo", Page.Theme);
                        }
                        else if (e.Row.DataItemIndex == ((List<DatiQuotaFondoIntegrativoLocal>)ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text = GetValueFromId(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione);
                            ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestioneQuotaFondo_item")).Text);
                            ((Label)e.Row.FindControl("lblQuota_item")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Quota;

                            ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Gestione;
                            ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Settimane;
                            ((Label)e.Row.FindControl("lblAmmontare")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Ammontare, 4);
                            ((Label)e.Row.FindControl("lblMontante")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiQuotaFondoIntegrativoLocal)(e.Row.DataItem)).Montante, 4);

                            //if (CodeUtility.IsRicostituzione(datiPensione))
                            //    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "");
                            //else
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteQuotaFondoIntegrativo");
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
                throw new INPS.DNA.DnaApplicationException("UCQuotaFondoIntegrativo, Errore nel metodo gvQuotaFondoIntegrativo_RowDataBound " + ex);
            }
        }

        private void LoadDecodificaData(IDatiContributiviAgo areaDatiContributivi)
        {
            if (areaDatiContributivi.areaDatiContributiviAgo != null)
            {
                ViewState["listaCodeGestioneQuotaFondoIntegrativo"] = areaDatiContributivi.areaDatiContributiviAgo.listaDecodificaGestioneQuotaFondoIntegrativo;
            }
        }

        private void GestioneDdl(GridViewRow row)
        {
            DropDownList ddlGestione = new DropDownList();
            ddlGestione = (DropDownList)row.FindControl("ddlCodiceGestioneQuotaFondo");
            ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
            Label lblCodiceGestioneQuotaFondo_item = (Label)row.FindControl("lblCodiceGestioneQuotaFondo_item");

            DecodificaGestioneQuotaFondoIntegrativo[] listaCodeGestioneQuotaFondoIntegrativo = (DecodificaGestioneQuotaFondoIntegrativo[])ViewState["listaCodeGestioneQuotaFondoIntegrativo"];
            IEnumerable<DecodificaGestioneQuotaFondoIntegrativo> listaOrdinata = listaCodeGestioneQuotaFondoIntegrativo.OrderBy(x => x.TraduzioneSuGP);

            foreach (DecodificaGestioneQuotaFondoIntegrativo datiCodeGestioneQuotaFondoIntegrativo in listaOrdinata)
            {
                ListItem li = new ListItem();
                li.Attributes.Add("title", datiCodeGestioneQuotaFondoIntegrativo.Descrizione);
                li.Text = datiCodeGestioneQuotaFondoIntegrativo.TraduzioneSuGP + " - " + datiCodeGestioneQuotaFondoIntegrativo.Descrizione;
                li.Value = datiCodeGestioneQuotaFondoIntegrativo.Id.ToString();

                ddlGestione.Items.Add(li);
            }
            if (((DatiQuotaFondoIntegrativoLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                ddlGestione.SelectedIndex = 0;
            else
                if (ddlGestione.Items.FindByValue(((DatiQuotaFondoIntegrativoLocal)(row.DataItem)).Gestione.Trim()) != null)
                    ddlGestione.Items.FindByValue(((DatiQuotaFondoIntegrativoLocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                else
                    ddlGestione.SelectedIndex = 0;

            DropDownList ddlQuota = new DropDownList();
            ddlQuota = (DropDownList)row.FindControl("ddlQuota");
            if (((DatiQuotaFondoIntegrativoLocal)(row.DataItem)).Quota.Trim() == string.Empty)
                ddlQuota.SelectedIndex = 0;
            else
                ddlQuota.SelectedValue = ((DatiQuotaFondoIntegrativoLocal)(row.DataItem)).Quota;

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);
        }

        private void GvQuotaFondoIntegrativo_Load()
        {
            try
            {
                elencoQuotaFondoIntViewState = (List<DatiQuotaFondoIntegrativoLocal>)ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()];
                gvQuotaFondoIntegrativo.DataSource = elencoQuotaFondoIntViewState;
                gvQuotaFondoIntegrativo.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCQuotaFondoIntegrativo, Errore nel metodo GvQuotaFondoIntegrativo_Load " + ex);
            }
        }

        private bool IsListaEmpty()
        {
            List<DatiQuotaFondoIntegrativoLocal> listaDatiQuotaFondoIntApp = (List<DatiQuotaFondoIntegrativoLocal>)ViewState[EnumViewState.ElencoDatiQuotaFondoIntegrativo.ToString()];
            if (listaDatiQuotaFondoIntApp == null || (listaDatiQuotaFondoIntApp.Count == 1 && listaDatiQuotaFondoIntApp[0].Ammontare == string.Empty &&
                listaDatiQuotaFondoIntApp[0].Gestione == string.Empty && listaDatiQuotaFondoIntApp[0].Montante == string.Empty &&
                listaDatiQuotaFondoIntApp[0].Settimane == string.Empty))
                return true;
            else
                return false;

        }

        private string GetValueFromId(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneQuotaFondoIntegrativo[] listaCodeGestioneQuotaFondoIntegrativo = (DecodificaGestioneQuotaFondoIntegrativo[])ViewState["listaCodeGestioneQuotaFondoIntegrativo"];
                DecodificaGestioneQuotaFondoIntegrativo app = listaCodeGestioneQuotaFondoIntegrativo.ToList().Find(delegate(DecodificaGestioneQuotaFondoIntegrativo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private bool IsEmptyEditableRow(GridViewRow row)
        {
            if (row.FindControl("txtAmmontare") != null && ((TextBox)row.FindControl("txtAmmontare")).Text != string.Empty &&
                row.FindControl("txtMontante") != null && ((TextBox)row.FindControl("txtMontante")).Text != string.Empty &&
                row.FindControl("txtSettimane") != null && ((TextBox)row.FindControl("txtSettimane")).Text != string.Empty &&
                row.FindControl("ddlCodiceGestioneQuotaFondo") != null && ((DropDownList)row.FindControl("ddlCodiceGestioneQuotaFondo")).SelectedIndex != 0 &&
                row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        protected void gvQuotaFondoIntegrativo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        #region EventHandler

        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;
        public event EventHandler GestisciTastoSalva;
        public event EventHandler InitializeData;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseGestisciTastoSalva(object sender, EventArgs e)
        {
            if (GestisciTastoSalva != null)
                GestisciTastoSalva(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        #endregion EventHandler

        #region Enums
        public enum EnumViewState
        {
            ElencoDatiQuotaFondoIntegrativo
        }
        #endregion Enums
    }
    #region nested Class
    [Serializable]
    public class DatiQuotaFondoIntegrativoLocal
    {
        public DatiQuotaFondoIntegrativoLocal()
        {
            this.Id = Guid.NewGuid();
        }
        public DatiQuotaFondoIntegrativoLocal(string strGestione, string strQuota, string strSettimane, string strAmmontareContributivo, string strMontanteContributivo, string strPL_Quotac)
        {
            this.Id = Guid.NewGuid();
            this._strQuota = strQuota;
            this._strAmmontare = strAmmontareContributivo;
            this._strGestione = strGestione;
            this._strMontante = strMontanteContributivo;
            this._strSettimane = strSettimane;
            this._strPL_Quotac = strPL_Quotac;
        }
        #region private properties
        private string _strQuota;
        private string _strGestione;
        private string _strSettimane;
        private string _strAmmontare;
        private string _strMontante;
        private string _strPL_Quotac;
        #endregion private properties

        #region public properties
        public Guid Id { get; set; }
        public string Quota { get { return _strQuota; } set { _strQuota = value; } }
        public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
        public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
        public string Ammontare { get { return _strAmmontare; } set { _strAmmontare = value; } }
        public string Montante { get { return _strMontante; } set { _strMontante = value; } }
        public string PL_Quotac { get { return _strPL_Quotac; } set { _strPL_Quotac = value; } }
        #endregion public properties

    }
    #endregion nested Class
}