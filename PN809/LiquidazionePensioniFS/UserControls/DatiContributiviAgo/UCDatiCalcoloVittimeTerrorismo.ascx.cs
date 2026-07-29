using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCDatiCalcoloVittimeTerrorismo : CustomBaseUserControl, IDatiContributiviAgo
    {
        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ValorizzaEtichetteDatiCalcoloVittimeTerrorismo(IDatiContributiviAgo Dati)
        {
            ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] = Dati.areaDatiContributiviAgo;
            ViewState[EnumViewState.ElencoCodeGestioneCalcoloRetrib.ToString()] = Dati.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloRetributivo;
            ViewState[EnumViewState.ElencoCodeGestioneCalcoloContrib.ToString()] = Dati.areaDatiContributiviAgo.listaDecodificaGestioneCalcoloContributivo;
            ViewState[EnumViewState.IsBeneficioImportoPensioneX.ToString()] = Dati.areaDatiContributiviAgo.IsBeneficioImportoPensioneX;
            ViewState[EnumViewState.IsBeneficioVittimeTerrorismo.ToString()] = Dati.areaDatiContributiviAgo.IsBeneficioVittimeTerrorismo;
            if (Dati.areaDatiContributiviAgo.DatiExINPDAI != null)
            {
                ViewState[EnumViewState.DecodificaTipoQuota.ToString()] = Dati.areaDatiContributiviAgo.DatiExINPDAI.DecodificaTipoQuota.ToList();
                ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()] = Dati.areaDatiContributiviAgo.DatiExINPDAI.CtrlDecorrenzaRetrExINPDAI.ToList();
            }
            hdnSoggettoBeneficiario.Value = Dati.areaDatiContributiviAgo.SoggettoBeneficiario.GetValueOrDefault().ToString();
            hdnTipologiaPrestazione.Value = Dati.areaDatiContributiviAgo.TipologiaPrestazione.GetValueOrDefault().ToString();
            hdnTipologiaBeneficio.Value = Dati.areaDatiContributiviAgo.TipologiaBeneficio.GetValueOrDefault().ToString();

            if (Dati.areaDatiContributiviAgo.IsDatiRetributiviVittimeVisible)
                InitBindDataRetributivi();
            else
                pdivRetributivo.Visible = false;

            if (Dati.areaDatiContributiviAgo.IsDatiContributiviVittimeVisible)
                InitBindDataContributivi();
            else
                pdivContributivo.Visible = false;

            if (Dati.areaDatiContributiviAgo.IsDatiImportoPensioneVittimeVisible)
                InitBindDataImportoPensione();
            else
                pdivImportoPensione.Visible = false;

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if ((Dati.areaDatiContributiviAgo != null && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                Dati.areaDatiContributiviAgo.IsBeneficioVittimeTerrorismo.GetValueOrDefault()) || Utility.IsDomandaRipristino(datiPensione))
            {
                gvDatiRetributiviVittime.Enabled = false;
                gvDatiContributiviVittime.Enabled = false;
                gvDatiImportoPensioneVittime.Enabled = false;
                btnEliminaDatiCalcoloVittime.Enabled = false;
            }
        }

        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo = new GestioneContribDatiCalcoloVittimeTerrorismo();

            List<DatiRetributiviVittimeLocal> listaDatiRetributiviVittime = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
            List<DatiContributiviVittimeLocal> listaDatiContributiviVittime = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
            List<DatiImportoPensioneVittimeLocal> listaDatiImportoPensioneVittime = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];

            if ((listaDatiRetributiviVittime != null && listaDatiRetributiviVittime.Count() > 0))
            {
                List<GestioneContribDatiRetributiviVittimeTerrorismo> listRetr = GetDataRetributiviToSave(listaDatiRetributiviVittime);
                int nDatiRetributivi = listRetr.Count();
                areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo = new GestioneContribDatiRetributiviVittimeTerrorismo[nDatiRetributivi];
                areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo = listRetr.ToArray();
            }

            if ((listaDatiContributiviVittime != null && listaDatiContributiviVittime.Count() > 0))
            {
                List<GestioneContribDatiContributiviVittimeTerrorismo> listContr = GetDataContributiviToSave(listaDatiContributiviVittime);
                int nDatiContributivi = listContr.Count();
                areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo = new GestioneContribDatiContributiviVittimeTerrorismo[nDatiContributivi];
                areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo = listContr.ToArray();
            }

            if ((listaDatiImportoPensioneVittime != null && listaDatiImportoPensioneVittime.Count() > 0))
            {
                List<GestioneContribDatiImportoPensioneVittimeTerrorismo> listImp = GetDataImportoPensioneToSave(listaDatiImportoPensioneVittime);
                int nDatiImportoPensione = listImp.Count();
                areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo = new GestioneContribDatiImportoPensioneVittimeTerrorismo[nDatiImportoPensione];
                areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo = listImp.ToArray();
            }
        }

        public void btnSalvaDatiCalcoloVittime_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviAgo = new AreaDatiContributivi();
            RecuperaCampi(this.areaDatiContributiviAgo);

            PresenterDatiContributiviAGO presenter = new PresenterDatiContributiviAGO();
            presenter.SalvaDatiCalcoloVittime(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo Terrorismo salvati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        public void btnEliminaDatiCalcoloVittime_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviAGO presenter = new PresenterDatiContributiviAGO();
            presenter.EliminaDatiCalcoloVittime(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo Terrorismo eliminati correttamente.";
                RaiseShowAvviso(this, null);
                InitializeData(this, null);
            }
        }

        protected void gvVittime_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        internal void DisabilitaPulsanti()
        {
            btnSalvaDatiCalcoloVittime.Enabled = false;
            btnEliminaDatiCalcoloVittime.Enabled = false;
        }

        internal void UpdateGridRetributivi(List<GestioneAggiornamentoPECODatiRetributivi> lstDatiRetributiviPage)
        {
            gvDatiRetributiviVittime.EditIndex = -1;
            List<DatiRetributiviVittimeLocal> lstRetributivi = ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()] as List<DatiRetributiviVittimeLocal>;
            if (lstRetributivi == null)
                lstRetributivi = new List<DatiRetributiviVittimeLocal>();
            else
                lstRetributivi = lstRetributivi.FindAll(x => !x.IsFromDatiCalcolo);

            //costruisco i record con cui aggiornare
            if (lstDatiRetributiviPage != null && lstDatiRetributiviPage.Count > 0)
            {
                lstRetributivi.InsertRange(0, lstDatiRetributiviPage.Select(x => new DatiRetributiviVittimeLocal()
                {
                    Gestione = x.CodGestione.ToString(),
                    IsFromDatiCalcolo = true,
                    Quota = x.Quota.ToString(),
                    TipoQuota = x.CodiceTipoQuota,
                    Settimane = (x.SettimaneA.GetValueOrDefault() + x.SettimaneB.GetValueOrDefault()).ToString(),
                    RMS = (x.RMSQuotaA.GetValueOrDefault() + x.RMSQuotaB.GetValueOrDefault()).ToString(System.Globalization.CultureInfo.CurrentUICulture)
                }).ToList());
            }
            gvDatiRetributiviVittime.DataSource = lstRetributivi;
            ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()] = lstRetributivi;
            gvDatiRetributiviVittime.DataBind();
        }

        internal void UpdateGridContributivi(List<GestioneAggiornamentoPECODatiContributivi> lstDatiContributiviPage)
        {
            gvDatiContributiviVittime.EditIndex = -1;
            List<DatiContributiviVittimeLocal> lstContributivi = ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()] as List<DatiContributiviVittimeLocal>;
            if (lstContributivi == null)
                lstContributivi = new List<DatiContributiviVittimeLocal>();
            else
                lstContributivi = lstContributivi.FindAll(x => !x.IsFromDatiCalcolo);

            //costruisco i record con cui aggiornare
            if (lstDatiContributiviPage != null && lstDatiContributiviPage.Count > 0)
            {
                lstContributivi.InsertRange(0, lstDatiContributiviPage.Select(x => new DatiContributiviVittimeLocal()
                {
                    Gestione = x.CodGestione.ToString(),
                    IsFromDatiCalcolo = true,
                    Quota = x.Quota.ToString(),
                    Settimane = (x.Settimane.GetValueOrDefault() + x.SettimaneQuotaD.GetValueOrDefault()).ToString(),
                    Ammontare = (x.ImportoContributivo.GetValueOrDefault() + x.ImportoContributivoQuotaD.GetValueOrDefault()).ToString(System.Globalization.CultureInfo.CurrentUICulture),
                    Montante = (x.MontanteContributivo.GetValueOrDefault() + x.MontanteContributivoQuotaD.GetValueOrDefault()).ToString(System.Globalization.CultureInfo.CurrentUICulture)
                }).ToList());
            }
            gvDatiContributiviVittime.DataSource = lstContributivi;
            ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()] = lstContributivi;
            gvDatiContributiviVittime.DataBind();
        }

        #region gvDatiRetributiviVittime

        protected void gvDatiRetributiviVittime_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiRetributiviVittime.EditIndex = e.NewEditIndex;

                List<DatiRetributiviVittimeLocal> listaDatiRetrVittimeApp = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
                gvDatiRetributiviVittime.DataSource = listaDatiRetrVittimeApp;
                gvDatiRetributiviVittime.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiRetributiviVittime_RowEditing " + ex);
            }
        }

        protected void gvDatiRetributiviVittime_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiRetributiviVittime.EditIndex = -1;

                List<DatiRetributiviVittimeLocal> listaDatiRetrVittimeApp = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
                gvDatiRetributiviVittime.DataSource = listaDatiRetrVittimeApp;
                gvDatiRetributiviVittime.DataBind();


            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiRetributiviVittime_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiRetributiviVittime_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            List<DatiRetributiviVittimeLocal> datiRetributivi = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiRetributiviVittimeLocal> listaDatiRetribVittimeApp = new List<DatiRetributiviVittimeLocal>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string DecorrenzaBeneficioApp = string.Empty;
                    string GestioneApp = string.Empty;
                    string QuotaApp = string.Empty;
                    string TipoQuotaApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string RMSApp = string.Empty;
                    string BeneficioApp = string.Empty;
                    bool IsFromDatiCalcolo = false;
                    if (!IsEmptyReadableRowRetrib(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            DecorrenzaBeneficioApp = ((Label)rApp.FindControl("lblDecorrenzaBeneficio")).Text;
                            GestioneApp = ((Label)rApp.FindControl("lblIdCodeGestione")).Text;
                            QuotaApp = ((Label)rApp.FindControl("lblQuota")).Text;
                            TipoQuotaApp = ((Label)rApp.FindControl("lblTipoQuota_item")).Text;
                            SettimaneApp = ((Label)rApp.FindControl("lblSettimane")).Text;
                            RMSApp = ((Label)rApp.FindControl("lblRetribuzioneMedia")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;
                            IsFromDatiCalcolo = datiRetributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                            listaDatiRetribVittimeApp.Add(new DatiRetributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, TipoQuotaApp, SettimaneApp, RMSApp, BeneficioApp, IsFromDatiCalcolo));
                        }
                    }
                    else if (!IsEmptyEditableRowRetrib(rApp))
                    {

                        if (datiRetributivi != null && datiRetributivi.Count - 1 > rApp.DataItemIndex)
                        {
                            if (rApp.DataItemIndex != r.DataItemIndex)
                            {
                                DecorrenzaBeneficioApp = datiRetributivi[rApp.DataItemIndex].DecorrenzaBeneficio;
                                GestioneApp = datiRetributivi[rApp.DataItemIndex].Gestione;
                                QuotaApp = datiRetributivi[rApp.DataItemIndex].Quota;
                                TipoQuotaApp = datiRetributivi[rApp.DataItemIndex].TipoQuota;
                                SettimaneApp = datiRetributivi[rApp.DataItemIndex].Settimane;
                                RMSApp = datiRetributivi[rApp.DataItemIndex].RMS;
                                BeneficioApp = datiRetributivi[rApp.DataItemIndex].Beneficio;
                                IsFromDatiCalcolo = datiRetributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                                listaDatiRetribVittimeApp.Add(new DatiRetributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, TipoQuotaApp, SettimaneApp, RMSApp, BeneficioApp, IsFromDatiCalcolo));
                            }
                        }
                    }
                }

                listaDatiRetribVittimeApp.Add(new DatiRetributiviVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false));

                ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()] = listaDatiRetribVittimeApp;
                gvDatiRetributiviVittime.EditIndex = -1;
                gvDatiRetributiviVittime.DataSource = listaDatiRetribVittimeApp;
                gvDatiRetributiviVittime.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRetrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiRetributiviVittimeLocal> listaDatiRetrVittimeApp = new List<DatiRetributiviVittimeLocal>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string DecorrenzaBeneficioApp = string.Empty;
                        string GestioneApp = string.Empty;
                        string QuotaApp = string.Empty;
                        string TipoQuotaApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string RMSApp = string.Empty;
                        string BeneficioApp = string.Empty;
                        bool IsFromDatiCalcolo = false;
                        if (!IsEmptyEditableRowRetrib(rApp))
                        {
                            DecorrenzaBeneficioApp = ((TextBox)rApp.FindControl("txtDecorrenzaBeneficio")).Text;
                            GestioneApp = ((DropDownList)rApp.FindControl("ddlCodiceGestione")).SelectedValue;
                            QuotaApp = ((DropDownList)rApp.FindControl("ddlQuota")).SelectedValue;
                            if (Utility.IsDomandaDAI(this.domanda.Categoria))
                            {
                                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloRetrib.ToString()];
                                DecodificaGestioneCalcoloRetributivo gestioneA = listaCodeGestioneCalcoloRetrib.ToList().Find(x => x.TraduzioneSuGP.Trim() == "A");
                                //logica selezione gestione per quote di tipo A e per quote di altri tipi
                                if (gestioneA != null && GestioneApp == gestioneA.Id.ToString())
                                {
                                    TipoQuotaApp = ((DropDownList)rApp.FindControl("ddlTipoQuotaGestioneA")).SelectedValue;
                                }
                                else
                                {
                                    TipoQuotaApp = ((DropDownList)rApp.FindControl("ddlTipoQuotaGestioneAltre")).SelectedValue;
                                }
                                if (!ControlCodGestioneQuotaTipoQuota(GestioneApp, QuotaApp, TipoQuotaApp))
                                {
                                    string strGestione = ((DropDownList)rApp.FindControl("ddlCodiceGestione")).SelectedItem.Text.Split('-')[0].Trim();
                                    this.HasError = true;
                                    this.ErrorMessage = "La terna Gestione '" + strGestione + "', Quota '" + QuotaApp + "' e Tipo Quota '" + TipoQuotaApp + "' non è valida.";
                                    RaiseShowAvviso(this, null);
                                    return;
                                }
                            }
                            SettimaneApp = ((TextBox)rApp.FindControl("txtSettimaneRetributive")).Text;
                            RMSApp = ((TextBox)rApp.FindControl("txtRetribuzioneMedia")).Text;
                            BeneficioApp = ((DropDownList)rApp.FindControl("ddlBeneficio")).SelectedValue;
                            IsFromDatiCalcolo = datiRetributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                            listaDatiRetrVittimeApp.Add(new DatiRetributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, TipoQuotaApp, SettimaneApp, RMSApp, BeneficioApp, IsFromDatiCalcolo));
                        }
                        else if (!IsEmptyReadableRowRetrib(rApp))
                        {
                            DecorrenzaBeneficioApp = ((Label)rApp.FindControl("lblDecorrenzaBeneficio")).Text;
                            GestioneApp = ((Label)rApp.FindControl("lblIdCodeGestione")).Text;
                            QuotaApp = ((Label)rApp.FindControl("lblQuota")).Text;
                            TipoQuotaApp = ((Label)rApp.FindControl("lblTipoQuota_item")).Text;
                            SettimaneApp = ((Label)rApp.FindControl("lblSettimane")).Text;
                            RMSApp = ((Label)rApp.FindControl("lblRetribuzioneMedia")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;
                            IsFromDatiCalcolo = datiRetributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                            listaDatiRetrVittimeApp.Add(new DatiRetributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, TipoQuotaApp, SettimaneApp, RMSApp, BeneficioApp, IsFromDatiCalcolo));
                        }
                    }
                    listaDatiRetrVittimeApp.Add(new DatiRetributiviVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false));
                    gvDatiRetributiviVittime.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()] = listaDatiRetrVittimeApp;
                    gvDatiRetributiviVittime.DataSource = listaDatiRetrVittimeApp;
                    gvDatiRetributiviVittime.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiRetributiviVittimeLocal> listaDatiRetrVittimeApp = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
                if (!IsListaEmpty('R'))
                {
                    gvDatiRetributiviVittime.EditIndex = -1;
                    gvDatiRetributiviVittime.DataSource = listaDatiRetrVittimeApp;
                    gvDatiRetributiviVittime.DataBind();
                }
            }

            RaiseHideAvviso(this, null);

        }

        protected void gvDatiRetributiviVittime_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //record proveniente da DatiCalcolo
                    if (((DatiRetributiviVittimeLocal)(e.Row.DataItem)).IsFromDatiCalcolo)
                    {


                        ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                        ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl("lblTipoQuota_item")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).TipoQuota;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).RMS, 4);
                        ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Beneficio;

                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    //prima riga
                    else if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty('R') && gvDatiRetributiviVittime.EditIndex == -1)
                        {
                            gvDatiRetributiviVittime.EditIndex = 0;

                            gvDatiRetributiviVittime.DataSource = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
                            gvDatiRetributiviVittime.DataBind();
                        }
                        else if (IsEmptyEditableRowRetrib(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, 'R');
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeRetr", Page.Theme);
                                LinkButton delete = ((LinkButton)(e.Row.FindControl("btnDeleteRetributivi")));
                                delete.Text = string.Empty;
                                LinkButton save = (LinkButton)(e.Row.Cells[0].Controls[0]);
                                save.OnClientClick = "riabilitaCampi();";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblTipoQuota_item")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).TipoQuota;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).RMS, 4);
                                ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Beneficio;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteRetributivi");
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, 'R');
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeRetr", Page.Theme);
                                LinkButton save = (LinkButton)(e.Row.Cells[0].Controls[0]);
                                save.OnClientClick = "riabilitaCampi();";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblTipoQuota_item")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).TipoQuota;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).RMS, 4);
                                ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Beneficio;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDeleteRetributivi");
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            GestioneDdls(e.Row, 'R');
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeRetr", Page.Theme);
                            LinkButton save = (LinkButton)(e.Row.Cells[0].Controls[0]);
                            save.OnClientClick = "riabilitaCampi();";
                        }

                        else if (e.Row.DataItemIndex == ((List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                            ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                            ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                            ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                            ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Quota;
                            ((Label)e.Row.FindControl("lblTipoQuota_item")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).TipoQuota;
                            ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                            ((Label)e.Row.FindControl("lblRetribuzioneMedia")).Text = CodeUtility.ConvertDecimalFixedPoint(((DatiRetributiviVittimeLocal)(e.Row.DataItem)).RMS, 4);
                            ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiRetributiviVittimeLocal)(e.Row.DataItem)).Beneficio;

                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, "btnDeleteRetributivi");
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
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiRetributiviVittime_RowDataBound " + ex);
            }
        }

        protected void gvDatiRetributiviVittime_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }

        #endregion gvDatiRetributiviVittime

        #region gvDatiContributiviVittime

        protected void gvDatiContributiviVittime_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiContributiviVittime.EditIndex = e.NewEditIndex;

                List<DatiContributiviVittimeLocal> listaDatiContrVittimeApp = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                gvDatiContributiviVittime.DataSource = listaDatiContrVittimeApp;
                gvDatiContributiviVittime.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiContributiviVittime_RowEditing " + ex);
            }
        }

        protected void gvDatiContributiviVittime_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiContributiviVittime.EditIndex = -1;

                List<DatiContributiviVittimeLocal> listaDatiContrVittimeApp = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                gvDatiContributiviVittime.DataSource = listaDatiContrVittimeApp;
                gvDatiContributiviVittime.DataBind();


            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiContributiviVittime_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiContributiviVittime_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiContributiviVittimeLocal> listaDatiContribVittimeApp = new List<DatiContributiviVittimeLocal>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string DecorrenzaBeneficioApp = string.Empty;
                    string GestioneApp = string.Empty;
                    string QuotaApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string BeneficioApp = string.Empty;
                    string AmmontareApp = string.Empty;
                    string MontanteApp = string.Empty;
                    bool IsFromDatiCalcolo = false;

                    List<DatiContributiviVittimeLocal> datiContributivi = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                    if (!IsEmptyReadableRowContrib(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            DecorrenzaBeneficioApp = ((Label)rApp.FindControl("lblDecorrenzaBeneficio")).Text;
                            GestioneApp = ((Label)rApp.FindControl("lblIdCodeGestione")).Text;
                            QuotaApp = ((Label)rApp.FindControl("lblQuota")).Text;
                            SettimaneApp = ((Label)rApp.FindControl("lblSettimane")).Text;
                            AmmontareApp = ((Label)rApp.FindControl("lblAmmontare")).Text;
                            MontanteApp = ((Label)rApp.FindControl("lblMontante")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;
                            IsFromDatiCalcolo = datiContributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                            listaDatiContribVittimeApp.Add(new DatiContributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, SettimaneApp, BeneficioApp, AmmontareApp, MontanteApp, IsFromDatiCalcolo));
                        }
                    }
                    else if (!IsEmptyEditableRowContrib(rApp))
                    {

                        if (datiContributivi != null && datiContributivi.Count - 1 > rApp.DataItemIndex)
                        {
                            if (rApp.DataItemIndex != r.DataItemIndex)
                            {
                                DecorrenzaBeneficioApp = datiContributivi[rApp.DataItemIndex].DecorrenzaBeneficio;
                                GestioneApp = datiContributivi[rApp.DataItemIndex].Gestione;
                                QuotaApp = datiContributivi[rApp.DataItemIndex].Quota;
                                SettimaneApp = datiContributivi[rApp.DataItemIndex].Settimane;
                                AmmontareApp = datiContributivi[rApp.DataItemIndex].Ammontare;
                                MontanteApp = datiContributivi[rApp.DataItemIndex].Montante;
                                BeneficioApp = datiContributivi[rApp.DataItemIndex].Beneficio;
                                IsFromDatiCalcolo = datiContributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                                listaDatiContribVittimeApp.Add(new DatiContributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, SettimaneApp, BeneficioApp, AmmontareApp, MontanteApp, IsFromDatiCalcolo));
                            }
                        }
                    }
                }

                listaDatiContribVittimeApp.Add(new DatiContributiviVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false));

                ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()] = listaDatiContribVittimeApp;
                gvDatiContributiviVittime.EditIndex = -1;
                gvDatiContributiviVittime.DataSource = listaDatiContribVittimeApp;
                gvDatiContributiviVittime.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowContrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiContributiviVittimeLocal> datiContributivi = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                    List<DatiContributiviVittimeLocal> listaDatiContrVittimeApp = new List<DatiContributiviVittimeLocal>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string DecorrenzaBeneficioApp = string.Empty;
                        string GestioneApp = string.Empty;
                        string QuotaApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string BeneficioApp = string.Empty;
                        string AmmontareApp = string.Empty;
                        string MontanteApp = string.Empty;
                        bool IsFromDatiCalcolo = false;

                        if (!IsEmptyEditableRowContrib(rApp))
                        {
                            DecorrenzaBeneficioApp = ((TextBox)rApp.FindControl("txtDecorrenzaBeneficio")).Text;
                            GestioneApp = ((DropDownList)rApp.FindControl("ddlCodiceGestione")).SelectedValue;
                            QuotaApp = ((DropDownList)rApp.FindControl("ddlQuota")).SelectedValue;
                            SettimaneApp = ((TextBox)rApp.FindControl("txtSettimaneRetributive")).Text;
                            AmmontareApp = ((TextBox)rApp.FindControl("txtAmmontare")).Text;
                            MontanteApp = ((TextBox)rApp.FindControl("txtMontante")).Text;
                            BeneficioApp = ((DropDownList)rApp.FindControl("ddlBeneficio")).SelectedValue;
                            IsFromDatiCalcolo = datiContributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                            listaDatiContrVittimeApp.Add(new DatiContributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, SettimaneApp, BeneficioApp, AmmontareApp, MontanteApp, IsFromDatiCalcolo));
                        }
                        else if (!IsEmptyReadableRowContrib(rApp))
                        {
                            DecorrenzaBeneficioApp = ((Label)rApp.FindControl("lblDecorrenzaBeneficio")).Text;
                            GestioneApp = ((Label)rApp.FindControl("lblIdCodeGestione")).Text;
                            QuotaApp = ((Label)rApp.FindControl("lblQuota")).Text;
                            SettimaneApp = ((Label)rApp.FindControl("lblSettimane")).Text;
                            AmmontareApp = ((Label)rApp.FindControl("lblAmmontare")).Text;
                            MontanteApp = ((Label)rApp.FindControl("lblMontante")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;
                            IsFromDatiCalcolo = datiContributivi[rApp.DataItemIndex].IsFromDatiCalcolo;
                            listaDatiContrVittimeApp.Add(new DatiContributiviVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, QuotaApp, SettimaneApp, BeneficioApp, AmmontareApp, MontanteApp, IsFromDatiCalcolo));
                        }
                    }
                    listaDatiContrVittimeApp.Add(new DatiContributiviVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false));
                    gvDatiContributiviVittime.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()] = listaDatiContrVittimeApp;
                    gvDatiContributiviVittime.DataSource = listaDatiContrVittimeApp;
                    gvDatiContributiviVittime.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiContributiviVittimeLocal> listaDatiContrVittimeApp = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                if (!IsListaEmpty('C'))
                {
                    gvDatiContributiviVittime.EditIndex = -1;
                    gvDatiContributiviVittime.DataSource = listaDatiContrVittimeApp;
                    gvDatiContributiviVittime.DataBind();
                }
            }

            RaiseHideAvviso(this, null);

        }

        protected void gvDatiContributiviVittime_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //record proveniente da DatiCalcolo
                    if (((DatiContributiviVittimeLocal)(e.Row.DataItem)).IsFromDatiCalcolo)
                    {
                        ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                        ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdContr(((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                        ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                        ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                        ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Quota;
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                        ((Label)e.Row.FindControl("lblAmmontare")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Ammontare;
                        ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Montante;
                        ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Beneficio;

                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                    }
                    //prima riga
                    else if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty('C') && gvDatiContributiviVittime.EditIndex == -1)
                        {
                            gvDatiContributiviVittime.EditIndex = 0;

                            gvDatiContributiviVittime.DataSource = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                            gvDatiContributiviVittime.DataBind();
                        }
                        else if (IsEmptyEditableRowContrib(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, 'C');
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeContr", Page.Theme);
                                LinkButton delete = ((LinkButton)(e.Row.FindControl("btnDeleteContributivi")));
                                delete.Text = string.Empty;
                                LinkButton save = (LinkButton)(e.Row.Cells[0].Controls[0]);
                                save.OnClientClick = "riabilitaCampi();";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdContr(((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblAmmontare")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Ammontare;
                                ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Montante;
                                ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Beneficio;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[8], Page.Theme, "btnDeleteContributivi");
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, 'C');
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeContr", Page.Theme);
                                LinkButton save = (LinkButton)(e.Row.Cells[0].Controls[0]);
                                save.OnClientClick = "riabilitaCampi();";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdContr(((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Quota;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblAmmontare")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Ammontare;
                                ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Montante;
                                ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Beneficio;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[8], Page.Theme, "btnDeleteContributivi");
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            GestioneDdls(e.Row, 'C');
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeContr", Page.Theme);
                            LinkButton save = (LinkButton)(e.Row.Cells[0].Controls[0]);
                            save.OnClientClick = "riabilitaCampi();";
                        }

                        else if (e.Row.DataItemIndex == ((List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                            ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdContr(((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione);
                            ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                            ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Gestione;
                            ((Label)e.Row.FindControl("lblQuota")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Quota;
                            ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Settimane;
                            ((Label)e.Row.FindControl("lblAmmontare")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Ammontare;
                            ((Label)e.Row.FindControl("lblMontante")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Montante;
                            ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiContributiviVittimeLocal)(e.Row.DataItem)).Beneficio;
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[8], Page.Theme, "btnDeleteContributivi");
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
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiContributiviVittime_RowDataBound " + ex);
            }
        }

        protected void gvDatiContributiviVittime_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }

        #endregion gvDatiContributiviVittime

        #region gvDatiImportoPensioneVittime

        protected void gvDatiImportoPensioneVittime_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiImportoPensioneVittime.EditIndex = e.NewEditIndex;

                List<DatiImportoPensioneVittimeLocal> listaDatiImpVittimeApp = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];
                gvDatiImportoPensioneVittime.DataSource = listaDatiImpVittimeApp;
                gvDatiImportoPensioneVittime.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiImportoPensioneVittime_RowEditing " + ex);
            }
        }

        protected void gvDatiImportoPensioneVittime_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiImportoPensioneVittime.EditIndex = -1;

                List<DatiImportoPensioneVittimeLocal> listaDatiImpVittimeApp = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];
                gvDatiImportoPensioneVittime.DataSource = listaDatiImpVittimeApp;
                gvDatiImportoPensioneVittime.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiImportoPensioneVittime_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiImportoPensioneVittime_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<DatiImportoPensioneVittimeLocal> listaDatiImpVittimeApp = new List<DatiImportoPensioneVittimeLocal>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    string DecorrenzaBeneficioApp = string.Empty;
                    string GestioneApp = string.Empty;
                    string SettimaneApp = string.Empty;
                    string BeneficioApp = string.Empty;
                    string ImportoPensioneApp = string.Empty;

                    if (!IsEmptyReadableRowImportoPensione(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            DecorrenzaBeneficioApp = ((Label)rApp.FindControl("lblDecorrenzaBeneficio")).Text;
                            GestioneApp = ((Label)rApp.FindControl("lblIdCodeGestione")).Text;
                            SettimaneApp = ((Label)rApp.FindControl("lblSettimane")).Text;
                            ImportoPensioneApp = ((Label)rApp.FindControl("lblImportoPensione")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;

                            listaDatiImpVittimeApp.Add(new DatiImportoPensioneVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, SettimaneApp, BeneficioApp, ImportoPensioneApp));
                        }
                    }
                    else if (!IsEmptyEditableRowImportoPensione(rApp))
                    {
                        List<DatiImportoPensioneVittimeLocal> datiImportoPensione = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];
                        if (datiImportoPensione != null && datiImportoPensione.Count - 1 > rApp.DataItemIndex)
                        {
                            if (rApp.DataItemIndex != r.DataItemIndex)
                            {
                                DecorrenzaBeneficioApp = datiImportoPensione[rApp.DataItemIndex].DecorrenzaBeneficio;
                                GestioneApp = datiImportoPensione[rApp.DataItemIndex].Gestione;
                                SettimaneApp = datiImportoPensione[rApp.DataItemIndex].Settimane;
                                ImportoPensioneApp = datiImportoPensione[rApp.DataItemIndex].ImportoPensione;
                                BeneficioApp = datiImportoPensione[rApp.DataItemIndex].Beneficio;
                                listaDatiImpVittimeApp.Add(new DatiImportoPensioneVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, SettimaneApp, BeneficioApp, ImportoPensioneApp));
                            }
                        }
                    }
                }

                listaDatiImpVittimeApp.Add(new DatiImportoPensioneVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

                ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()] = listaDatiImpVittimeApp;
                gvDatiImportoPensioneVittime.EditIndex = -1;
                gvDatiImportoPensioneVittime.DataSource = listaDatiImpVittimeApp;
                gvDatiImportoPensioneVittime.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowImportoPensione((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<DatiImportoPensioneVittimeLocal> listaDatiImpVittimeApp = new List<DatiImportoPensioneVittimeLocal>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        string DecorrenzaBeneficioApp = string.Empty;
                        string GestioneApp = string.Empty;
                        string SettimaneApp = string.Empty;
                        string BeneficioApp = string.Empty;
                        string ImportoPensioneApp = string.Empty;

                        if (!IsEmptyEditableRowImportoPensione(rApp))
                        {
                            DecorrenzaBeneficioApp = ((TextBox)rApp.FindControl("txtDecorrenzaBeneficio")).Text;
                            GestioneApp = ((DropDownList)rApp.FindControl("ddlCodiceGestione")).SelectedValue;
                            SettimaneApp = ((TextBox)rApp.FindControl("txtSettimaneRetributive")).Text;
                            ImportoPensioneApp = ((TextBox)rApp.FindControl("txtImportoPensione")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;

                            listaDatiImpVittimeApp.Add(new DatiImportoPensioneVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, SettimaneApp, BeneficioApp, ImportoPensioneApp));
                        }
                        else if (!IsEmptyReadableRowImportoPensione(rApp))
                        {
                            DecorrenzaBeneficioApp = ((Label)rApp.FindControl("lblDecorrenzaBeneficio")).Text;
                            GestioneApp = ((Label)rApp.FindControl("lblIdCodeGestione")).Text;
                            SettimaneApp = ((Label)rApp.FindControl("lblSettimane")).Text;
                            ImportoPensioneApp = ((Label)rApp.FindControl("lblImportoPensione")).Text;
                            BeneficioApp = ((Label)rApp.FindControl("lblBeneficio")).Text;

                            listaDatiImpVittimeApp.Add(new DatiImportoPensioneVittimeLocal(DecorrenzaBeneficioApp, GestioneApp, SettimaneApp, BeneficioApp, ImportoPensioneApp));
                        }
                    }
                    listaDatiImpVittimeApp.Add(new DatiImportoPensioneVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvDatiImportoPensioneVittime.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()] = listaDatiImpVittimeApp;
                    gvDatiImportoPensioneVittime.DataSource = listaDatiImpVittimeApp;
                    gvDatiImportoPensioneVittime.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<DatiImportoPensioneVittimeLocal> listaDatiImpVittimeApp = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];
                if (!IsListaEmpty('I'))
                {
                    gvDatiImportoPensioneVittime.EditIndex = -1;
                    gvDatiImportoPensioneVittime.DataSource = listaDatiImpVittimeApp;
                    gvDatiImportoPensioneVittime.DataBind();
                }
            }

            RaiseHideAvviso(this, null);

        }

        protected void gvDatiImportoPensioneVittime_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Presenter.SvrLiquidazioneAgo.AreaDatiContributivi area = ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] as Presenter.SvrLiquidazioneAgo.AreaDatiContributivi;

                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty('I') && gvDatiImportoPensioneVittime.EditIndex == -1)
                        {
                            gvDatiImportoPensioneVittime.EditIndex = 0;
                            gvDatiImportoPensioneVittime.DataSource = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];
                            gvDatiImportoPensioneVittime.DataBind();
                        }
                        else if (IsEmptyEditableRowImportoPensione(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, 'I');
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeImp", Page.Theme);
                                LinkButton delete = ((LinkButton)(e.Row.FindControl("btnDeleteImportoPensione")));
                                delete.Text = string.Empty;

                                TextBox txtSettimaneRetributive = (TextBox)e.Row.FindControl("txtSettimaneRetributive");
                                if (area != null)
                                {
                                    if (area.IsSettimaneImportoPensioneLocked)
                                    {
                                        txtSettimaneRetributive.Enabled = false;
                                        RequiredFieldValidator requiredFieldtxtSettimaneRetributive = (RequiredFieldValidator)e.Row.FindControl("RequiredFieldtxtSettimaneRetributive");
                                        requiredFieldtxtSettimaneRetributive.Enabled = false;
                                    }
                                    else
                                        txtSettimaneRetributive.Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Settimane;
                                }
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = area != null && area.IsSettimaneImportoPensioneLocked ? string.Empty : ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblImportoPensione")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).ImportoPensione;
                                ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Beneficio;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteImportoPensione");
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                GestioneDdls(e.Row, 'I');
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeImp", Page.Theme);

                                TextBox txtSettimaneRetributive = (TextBox)e.Row.FindControl("txtSettimaneRetributive");
                                if (area != null)
                                {
                                    if (area.IsSettimaneImportoPensioneLocked)
                                    {
                                        txtSettimaneRetributive.Enabled = false;
                                        RequiredFieldValidator requiredFieldtxtSettimaneRetributive = (RequiredFieldValidator)e.Row.FindControl("RequiredFieldtxtSettimaneRetributive");
                                        requiredFieldtxtSettimaneRetributive.Enabled = false;
                                    }
                                    else
                                        txtSettimaneRetributive.Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Settimane;
                                }
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Gestione);
                                ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                                ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Gestione;
                                ((Label)e.Row.FindControl("lblSettimane")).Text = area != null && area.IsSettimaneImportoPensioneLocked ? string.Empty : ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Settimane;
                                ((Label)e.Row.FindControl("lblImportoPensione")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).ImportoPensione;
                                ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Beneficio;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteImportoPensione");
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            GestioneDdls(e.Row, 'I');
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloVittimeImp", Page.Theme);

                            TextBox txtSettimaneRetributive = (TextBox)e.Row.FindControl("txtSettimaneRetributive");
                            if (area != null)
                            {
                                if (area.IsSettimaneImportoPensioneLocked)
                                {
                                    txtSettimaneRetributive.Enabled = false;
                                    RequiredFieldValidator requiredFieldtxtSettimaneRetributive = (RequiredFieldValidator)e.Row.FindControl("RequiredFieldtxtSettimaneRetributive");
                                    requiredFieldtxtSettimaneRetributive.Enabled = false;
                                }
                                else
                                    txtSettimaneRetributive.Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Settimane;
                            }
                        }

                        else if (e.Row.DataItemIndex == ((List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenzaBeneficio")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).DecorrenzaBeneficio;
                            ((Label)e.Row.FindControl("lblCodiceGestione")).Text = GetValueFromIdRetr(((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Gestione);
                            ((Label)e.Row.FindControl("lblCodiceGestione")).Attributes.Add("title", ((Label)e.Row.FindControl("lblCodiceGestione")).Text);
                            ((Label)e.Row.FindControl("lblIdCodeGestione")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Gestione;
                            ((Label)e.Row.FindControl("lblSettimane")).Text = area != null && area.IsSettimaneImportoPensioneLocked ? string.Empty : ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Settimane;
                            ((Label)e.Row.FindControl("lblImportoPensione")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).ImportoPensione;
                            ((Label)e.Row.FindControl("lblBeneficio")).Text = ((DatiImportoPensioneVittimeLocal)(e.Row.DataItem)).Beneficio;
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteImportoPensione");
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
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloVittimeTerrorismo, Errore nel metodo gvDatiImportoPensioneVittime_RowDataBound " + ex);
            }
        }

        protected void gvDatiImportoPensioneVittime_DataBound(object sender, EventArgs e)
        {
            ManagePulsanti();
        }

        protected void gvDatiRetributiviVittime_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            gvDatiRetributiviVittime.Columns[(int)ColonneGvDatiRetributiviVittime.TipoQuota].Visible = Utility.IsDomandaDAI(this.domanda.Categoria);
        }

        #endregion gvDatiImportoPensioneVittime

        #region private methods

        private bool IsListaEmpty(char tipo)
        {
            switch (tipo)
            {
                case 'R':
                    List<DatiRetributiviVittimeLocal> listaDatiRetrVittimeApp = (List<DatiRetributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()];
                    if (listaDatiRetrVittimeApp == null || (listaDatiRetrVittimeApp.Count == 1 && listaDatiRetrVittimeApp[0].DecorrenzaBeneficio == string.Empty &&
                        listaDatiRetrVittimeApp[0].Gestione == string.Empty && listaDatiRetrVittimeApp[0].Quota == string.Empty && listaDatiRetrVittimeApp[0].Settimane == string.Empty &&
                        listaDatiRetrVittimeApp[0].RMS == string.Empty && listaDatiRetrVittimeApp[0].Beneficio == string.Empty))
                        return true;
                    else
                        return false;
                case 'C':
                    List<DatiContributiviVittimeLocal> listaDatiContrVittimeApp = (List<DatiContributiviVittimeLocal>)ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()];
                    if (listaDatiContrVittimeApp == null || (listaDatiContrVittimeApp.Count == 1 && listaDatiContrVittimeApp[0].DecorrenzaBeneficio == string.Empty &&
                        listaDatiContrVittimeApp[0].Gestione == string.Empty && listaDatiContrVittimeApp[0].Quota == string.Empty && listaDatiContrVittimeApp[0].Settimane == string.Empty &&
                        listaDatiContrVittimeApp[0].Beneficio == string.Empty && listaDatiContrVittimeApp[0].Ammontare == string.Empty && listaDatiContrVittimeApp[0].Montante == string.Empty
                        ))
                        return true;
                    else
                        return false;
                case 'I':
                    List<DatiImportoPensioneVittimeLocal> listaDatiImportoPensioneVittimeApp = (List<DatiImportoPensioneVittimeLocal>)ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()];
                    if (listaDatiImportoPensioneVittimeApp == null || (listaDatiImportoPensioneVittimeApp.Count == 1 && listaDatiImportoPensioneVittimeApp[0].DecorrenzaBeneficio == string.Empty &&
                        listaDatiImportoPensioneVittimeApp[0].Gestione == string.Empty && listaDatiImportoPensioneVittimeApp[0].Settimane == string.Empty &&
                        listaDatiImportoPensioneVittimeApp[0].Beneficio == string.Empty && listaDatiImportoPensioneVittimeApp[0].ImportoPensione == string.Empty
                        ))
                        return true;
                    else
                        return false;
            }

            return false;
        }

        private void ManagePulsanti()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            bool btnSalvaEnable = true;
            Presenter.SvrLiquidazioneAgo.AreaDatiContributivi area = ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] as Presenter.SvrLiquidazioneAgo.AreaDatiContributivi;

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (gvDatiRetributiviVittime.EditIndex == -1 && gvDatiContributiviVittime.EditIndex == -1 && gvDatiImportoPensioneVittime.EditIndex == -1)
            {
                if (area != null)
                {
                    RaiseGestisciTastoSalva(this, new Utility.EventMessageArgs() { Message = "SI" });
                    btnSalvaDatiCalcoloVittime.Enabled = true;
                    btnEliminaDatiCalcoloVittime.Enabled = true;
                }
                return;
            }
            else
            {
                btnSalvaEnable = false;

                if (gvDatiImportoPensioneVittime.EditIndex == -1 && area.IsDatiImportoPensioneVittimeVisible)
                {
                    List<DatiImportoPensioneVittimeLocal> lstImportoPensione = ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()] as List<DatiImportoPensioneVittimeLocal>;
                    if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_gt_80)
                    {
                        if (lstImportoPensione != null &&
                            lstImportoPensione.Exists(x => !string.IsNullOrEmpty(x.DecorrenzaBeneficio) && Utility.DataSuccessivaA(Utility.GetDateFromString(x.DecorrenzaBeneficio).GetValueOrDefault(), new DateTime(2008, 1, 1))))
                            btnSalvaEnable = true;
                    }
                    else if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Anticipata_Benefici_L206_2004_Vittime_Invalidità_lt_80 ||
                        tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_Benefici_L206_2004_Vittime_Invalidità_lt_80)
                    {
                        if (lstImportoPensione != null &&
                            lstImportoPensione.Exists(x => !string.IsNullOrEmpty(x.DecorrenzaBeneficio) && Utility.DataSuccessivaA(Utility.GetDateFromString(x.DecorrenzaBeneficio).GetValueOrDefault(), new DateTime(2007, 1, 1))))
                            btnSalvaEnable = true;
                    }
                }

                if (btnSalvaEnable)
                {
                    RaiseGestisciTastoSalva(this, new Utility.EventMessageArgs() { Message = "SI" });
                    btnSalvaDatiCalcoloVittime.Enabled = true;
                    btnEliminaDatiCalcoloVittime.Enabled = true;
                }
                else
                {
                    RaiseGestisciTastoSalva(this, new Utility.EventMessageArgs() { Message = "NO" });
                    btnSalvaDatiCalcoloVittime.Enabled = false;
                    btnEliminaDatiCalcoloVittime.Enabled = false;
                }
            }

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                ViewState[EnumViewState.IsBeneficioVittimeTerrorismo.ToString()] != null && (bool)ViewState[EnumViewState.IsBeneficioVittimeTerrorismo.ToString()])
            {
                gvDatiRetributiviVittime.Enabled = false;
                gvDatiContributiviVittime.Enabled = false;
                gvDatiImportoPensioneVittime.Enabled = false;
                btnEliminaDatiCalcoloVittime.Enabled = false;
            }

            if (Utility.IsDomandaRipristino(datiPensione))
            {
                btnEliminaDatiCalcoloVittime.Enabled = false;
            }
        }

        private bool IsEmptyReadableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("lblDecorrenzaBeneficio") != null && ((Label)row.FindControl("lblDecorrenzaBeneficio")).Text != string.Empty) ||
                (row.FindControl("lblCodiceGestione") != null && ((Label)row.FindControl("lblCodiceGestione")).Text != string.Empty) ||
                (row.FindControl("lblQuota") != null && ((Label)row.FindControl("lblQuota")).Text != string.Empty) ||
                (row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty) ||
                (row.FindControl("lblRetribuzioneMedia") != null && ((Label)row.FindControl("lblRetribuzioneMedia")).Text != string.Empty) ||
                (row.FindControl("lblBeneficio") != null && ((Label)row.FindControl("lblBeneficio")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowContrib(GridViewRow row)
        {
            if ((row.FindControl("lblDecorrenzaBeneficio") != null && ((Label)row.FindControl("lblDecorrenzaBeneficio")).Text != string.Empty) ||
                (row.FindControl("lblCodiceGestione") != null && ((Label)row.FindControl("lblCodiceGestione")).Text != string.Empty) ||
                (row.FindControl("lblQuota") != null && ((Label)row.FindControl("lblQuota")).Text != string.Empty) ||
                (row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty) ||
                (row.FindControl("lblAmmontare") != null && ((Label)row.FindControl("lblAmmontare")).Text != string.Empty) ||
                (row.FindControl("lblMontante") != null && ((Label)row.FindControl("lblMontante")).Text != string.Empty) ||
                (row.FindControl("lblBeneficio") != null && ((Label)row.FindControl("lblBeneficio")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowImportoPensione(GridViewRow row)
        {
            if ((row.FindControl("lblDecorrenzaBeneficio") != null && ((Label)row.FindControl("lblDecorrenzaBeneficio")).Text != string.Empty) ||
                (row.FindControl("lblCodiceGestione") != null && ((Label)row.FindControl("lblCodiceGestione")).Text != string.Empty) ||
                (row.FindControl("lblSettimane") != null && ((Label)row.FindControl("lblSettimane")).Text != string.Empty) ||
                (row.FindControl("lblImportoPensione") != null && ((Label)row.FindControl("lblImportoPensione")).Text != string.Empty)
                )
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("txtDecorrenzaBeneficio") != null && ((TextBox)row.FindControl("txtDecorrenzaBeneficio")).Text != string.Empty) ||
                (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0) ||
                (row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0) ||
                (row.FindControl("txtSettimaneRetributive") != null && ((TextBox)row.FindControl("txtSettimaneRetributive")).Text != string.Empty) ||
                (row.FindControl("txtRetribuzioneMedia") != null && ((TextBox)row.FindControl("txtRetribuzioneMedia")).Text != string.Empty) ||
                (row.FindControl("ddlBeneficio") != null && ((DropDownList)row.FindControl("ddlBeneficio")).SelectedIndex != 0)
                )

                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowContrib(GridViewRow row)
        {
            if ((row.FindControl("txtDecorrenzaBeneficio") != null && ((TextBox)row.FindControl("txtDecorrenzaBeneficio")).Text != string.Empty) ||
                (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0) ||
                (row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0) ||
                (row.FindControl("txtSettimaneRetributive") != null && ((TextBox)row.FindControl("txtSettimaneRetributive")).Text != string.Empty) ||
                (row.FindControl("txtAmmontare") != null && ((TextBox)row.FindControl("txtAmmontare")).Text != string.Empty) ||
                (row.FindControl("txtMontante") != null && ((TextBox)row.FindControl("txtMontante")).Text != string.Empty) ||
                (row.FindControl("ddlBeneficio") != null && ((DropDownList)row.FindControl("ddlBeneficio")).SelectedIndex != 0)
                )

                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowImportoPensione(GridViewRow row)
        {
            if ((row.FindControl("txtDecorrenzaBeneficio") != null && ((TextBox)row.FindControl("txtDecorrenzaBeneficio")).Text != string.Empty) ||
                (row.FindControl("ddlCodiceGestione") != null && ((DropDownList)row.FindControl("ddlCodiceGestione")).SelectedIndex != 0) ||
                (row.FindControl("txtSettimaneRetributive") != null && ((TextBox)row.FindControl("txtSettimaneRetributive")).Text != string.Empty) ||
                (row.FindControl("txtImportoPensione") != null && ((TextBox)row.FindControl("txtImportoPensione")).Text != string.Empty)
                )

                return false;
            else
                return true;
        }

        private void GestioneDdls(GridViewRow row, char tipo)
        {
            DropDownList ddlGestione = new DropDownList();
            ddlGestione = (DropDownList)row.FindControl("ddlCodiceGestione");
            switch (tipo)
            {
                case 'R':
                    ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                    DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloRetrib.ToString()];
                    IEnumerable<DecodificaGestioneCalcoloRetributivo> listaOrdinataRetr = listaCodeGestioneCalcoloRetrib.OrderBy(x => x.TraduzioneSuGP);
                    foreach (DecodificaGestioneCalcoloRetributivo datiCodeGestioneCalcoloRetrib in listaOrdinataRetr)
                    {
                        ListItem li = new ListItem();
                        li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                        li.Text = datiCodeGestioneCalcoloRetrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloRetrib.Descrizione;
                        li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                        ddlGestione.Items.Add(li);
                    }
                    if (((DatiRetributiviVittimeLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                        ddlGestione.SelectedIndex = 0;
                    else
                        ddlGestione.Items.FindByValue(((DatiRetributiviVittimeLocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    DropDownList ddlTipoQuotaGestioneAltre = new DropDownList();
                    DropDownList ddlTipoQuotaGestioneA = new DropDownList();

                    ddlTipoQuotaGestioneAltre = (DropDownList)row.FindControl("ddlTipoQuotaGestioneAltre");
                    ddlTipoQuotaGestioneA = (DropDownList)row.FindControl("ddlTipoQuotaGestioneA");

                    ddlTipoQuotaGestioneAltre.Items.Add(new ListItem(string.Empty, string.Empty));
                    ddlTipoQuotaGestioneA.Items.Add(new ListItem("   - quota 17", string.Empty));

                    List<DecodificaTipoQuota> listaDecodificaTipoQuota = (List<DecodificaTipoQuota>)ViewState[EnumViewState.DecodificaTipoQuota.ToString()];
                    List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExINPDAI = (List<CtrlDecorrenzaRetrExINPDAI>)ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()];

                    if (listaDecodificaTipoQuota != null && listaDecodificaTipoQuota.Count() > 0)
                    {
                        foreach (DecodificaTipoQuota decTipoQuota in listaDecodificaTipoQuota)
                        {
                            ListItem li = new ListItem();
                            li.Attributes.Add("title", decTipoQuota.Decodifica);
                            li.Text = decTipoQuota.Codice;
                            li.Value = decTipoQuota.Codice;
                            ddlTipoQuotaGestioneAltre.Items.Add(li);

                            if (listaCtrlDecorrenzaRetrExINPDAI != null && listaCtrlDecorrenzaRetrExINPDAI.Count() > 0)
                            {
                                CtrlDecorrenzaRetrExINPDAI ctrl = listaCtrlDecorrenzaRetrExINPDAI.Find(x => x.TipoQuota == decTipoQuota.Codice);
                                ListItem liGestioneA = new ListItem();
                                liGestioneA.Attributes.Add("title", decTipoQuota.Decodifica);
                                liGestioneA.Text = decTipoQuota.Codice + (ctrl != null ? " - quota " + ctrl.CodiceDecorrenza : string.Empty);
                                liGestioneA.Value = decTipoQuota.Codice;
                                ddlTipoQuotaGestioneA.Items.Add(liGestioneA);
                            }
                        }
                    }
                    ddlTipoQuotaGestioneAltre.SelectedValue = ((DatiRetributiviVittimeLocal)(row.DataItem)).TipoQuota;
                    ddlTipoQuotaGestioneA.SelectedValue = ((DatiRetributiviVittimeLocal)(row.DataItem)).TipoQuota;
                    break;
                case 'I':
                    ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                    DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloImp = (DecodificaGestioneCalcoloRetributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloRetrib.ToString()];
                    IEnumerable<DecodificaGestioneCalcoloRetributivo> listaOrdinataImp = listaCodeGestioneCalcoloImp.OrderBy(x => x.TraduzioneSuGP);
                    foreach (DecodificaGestioneCalcoloRetributivo datiCodeGestioneCalcoloRetrib in listaOrdinataImp)
                    {
                        ListItem li = new ListItem();
                        li.Attributes.Add("title", datiCodeGestioneCalcoloRetrib.Descrizione);
                        li.Text = datiCodeGestioneCalcoloRetrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloRetrib.Descrizione;
                        li.Value = datiCodeGestioneCalcoloRetrib.Id.ToString();
                        ddlGestione.Items.Add(li);
                    }
                    if (((DatiImportoPensioneVittimeLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                        ddlGestione.SelectedIndex = 0;
                    else
                        ddlGestione.Items.FindByValue(((DatiImportoPensioneVittimeLocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    break;
                case 'C':
                    ddlGestione.Items.Add(new ListItem(string.Empty, string.Empty));
                    DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloContrib.ToString()];
                    IEnumerable<DecodificaGestioneCalcoloContributivo> listaOrdinataContr = listaCodeGestioneCalcoloContrib.OrderBy(x => x.TraduzioneSuGP);
                    foreach (DecodificaGestioneCalcoloContributivo datiCodeGestioneCalcoloContrib in listaOrdinataContr)
                    {
                        ListItem li = new ListItem();
                        li.Attributes.Add("title", datiCodeGestioneCalcoloContrib.Descrizione);
                        li.Text = datiCodeGestioneCalcoloContrib.TraduzioneSuGP + " - " + datiCodeGestioneCalcoloContrib.Descrizione;
                        li.Value = datiCodeGestioneCalcoloContrib.Id.ToString();
                        ddlGestione.Items.Add(li);
                    }
                    if (((DatiContributiviVittimeLocal)(row.DataItem)).Gestione.Trim() == string.Empty)
                        ddlGestione.SelectedIndex = 0;
                    else
                        ddlGestione.Items.FindByValue(((DatiContributiviVittimeLocal)(row.DataItem)).Gestione.Trim()).Selected = true;
                    break;
            }

            DropDownList ddlQuota;
            switch (tipo)
            {
                case 'R':
                    ddlQuota = (DropDownList)row.FindControl("ddlQuota");
                    if (ddlQuota != null)
                        ddlQuota.SelectedValue = ((DatiRetributiviVittimeLocal)(row.DataItem)).Quota;
                    break;
                case 'C':
                    ddlQuota = (DropDownList)row.FindControl("ddlQuota");
                    if (ddlQuota != null)
                    {
                        ddlQuota.SelectedValue = ((DatiContributiviVittimeLocal)(row.DataItem)).Quota;

                        //Riferimento mail: LIQPENS - Segnalazioni AGO del 30/06/2014
                        //Dati contributivi: se il sistema di calcolo è retributivo, il codice quota C  non deve essere presente nel menu di scelta
                        if ((((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.TipoCalcolo) == GestioneContribTipoCalcolo.Retributivo)
                            ddlQuota.Items.Remove(ddlQuota.Items.FindByValue("C"));
                    }
                    break;
            }

            DropDownList ddlBeneficio;
            switch (tipo)
            {
                case 'R':
                    ddlBeneficio = (DropDownList)row.FindControl("ddlBeneficio");
                    ddlBeneficio.SelectedValue = ((DatiRetributiviVittimeLocal)(row.DataItem)).Beneficio;
                    break;
                case 'C':
                    ddlBeneficio = (DropDownList)row.FindControl("ddlBeneficio");
                    ddlBeneficio.SelectedValue = ((DatiContributiviVittimeLocal)(row.DataItem)).Beneficio;
                    break;
                case 'I':
                    if ((bool)ViewState[EnumViewState.IsBeneficioImportoPensioneX.ToString()])
                    {
                        Label lblBeneficio = (Label)row.FindControl("lblBeneficio");
                        lblBeneficio.Text = "X";
                    }
                    break;
            }
        }

        private string GetValueFromIdRetr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloRetrib.ToString()];
                DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate(DecodificaGestioneCalcoloRetributivo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private string GetValueFromIdContr(string id)
        {
            string ret = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                long index = Convert.ToInt64(id);
                DecodificaGestioneCalcoloContributivo[] listaCodeGestioneCalcoloContrib = (DecodificaGestioneCalcoloContributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloContrib.ToString()];
                DecodificaGestioneCalcoloContributivo app = listaCodeGestioneCalcoloContrib.ToList().Find(delegate(DecodificaGestioneCalcoloContributivo code) { return (code.Id == index); });
                if (app != null)
                    ret = app.TraduzioneSuGP + " - " + app.Descrizione;
            }
            return ret;
        }

        private void InitBindDataRetributivi()
        {
            List<DatiRetributiviVittimeLocal> elencoDatiRetributivi = new List<DatiRetributiviVittimeLocal>();
            this.areaDatiContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()];

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo != null &&
                this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo != null &&
                this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo.Count() > 0)
                elencoDatiRetributivi = MapDatiRetributiviForView(this.areaDatiContributiviAgo);

            DatiRetributiviVittimeLocal Empty = elencoDatiRetributivi.Find(delegate(DatiRetributiviVittimeLocal code)
            {
                return (code.DecorrenzaBeneficio == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty && code.Settimane == string.Empty &&
                        code.RMS == string.Empty && code.Beneficio == string.Empty);
            });

            if (Empty == null)
                elencoDatiRetributivi.Add(new DatiRetributiviVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false));

            gvDatiRetributiviVittime.DataSource = elencoDatiRetributivi;
            ViewState[EnumViewState.ElencoDatiRetributiviVittime.ToString()] = elencoDatiRetributivi;
            gvDatiRetributiviVittime.DataBind();
        }

        private void InitBindDataContributivi()
        {
            List<DatiContributiviVittimeLocal> elencoDatiContributivi = new List<DatiContributiviVittimeLocal>();

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo != null &&
                this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo != null &&
                this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo.Count() > 0)
                elencoDatiContributivi = MapDatiContributiviForView((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]);

            DatiContributiviVittimeLocal Empty = elencoDatiContributivi.Find(delegate(DatiContributiviVittimeLocal code)
            {
                return (code.DecorrenzaBeneficio == string.Empty && code.Gestione == string.Empty && code.Quota == string.Empty && code.Settimane == string.Empty &&
                        code.Ammontare == string.Empty && code.Montante == string.Empty && code.Beneficio == string.Empty);
            });

            if (Empty == null)
                elencoDatiContributivi.Add(new DatiContributiviVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false));

            gvDatiContributiviVittime.DataSource = elencoDatiContributivi;
            ViewState[EnumViewState.ElencoDatiContributiviVittime.ToString()] = elencoDatiContributivi;
            gvDatiContributiviVittime.DataBind();
        }

        private void InitBindDataImportoPensione()
        {
            List<DatiImportoPensioneVittimeLocal> elencoDatiImportoPensione = new List<DatiImportoPensioneVittimeLocal>();

            if (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo != null &&
                this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo != null &&
                this.areaDatiContributiviAgo.DatiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo.Count() > 0)
                elencoDatiImportoPensione = MapDatiImportoPensioneForView(this.areaDatiContributiviAgo);

            DatiImportoPensioneVittimeLocal Empty = elencoDatiImportoPensione.Find(delegate(DatiImportoPensioneVittimeLocal code)
            {
                return (code.DecorrenzaBeneficio == string.Empty && code.Gestione == string.Empty && code.Settimane == string.Empty &&
                         code.ImportoPensione == string.Empty && code.Beneficio == string.Empty);
            });

            if (Empty == null)
                elencoDatiImportoPensione.Add(new DatiImportoPensioneVittimeLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            gvDatiImportoPensioneVittime.DataSource = elencoDatiImportoPensione;
            ViewState[EnumViewState.ElencoDatiImportoPensioneVittime.ToString()] = elencoDatiImportoPensione;
            gvDatiImportoPensioneVittime.DataBind();
        }

        private static List<DatiRetributiviVittimeLocal> MapDatiRetributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiRetributiviVittimeLocal> elencoDatiRetributivi = new List<DatiRetributiviVittimeLocal>();
            foreach (GestioneContribDatiRetributiviVittimeTerrorismo retr in areaDatiContributivi.DatiCalcoloVittimeTerrorismo.ListaDatiRetributiviVittimeTerrorismo)
            {
                elencoDatiRetributivi.Add(new DatiRetributiviVittimeLocal(retr.DecorrenzaBeneficio.HasValue ? retr.DecorrenzaBeneficio.Value.ToString("MM/yyyy") : string.Empty,
                    retr.CodiceGestioneRetr.HasValue ? retr.CodiceGestioneRetr.Value.ToString() : string.Empty, retr.Quota.HasValue ? retr.Quota.Value.ToString() : string.Empty, !String.IsNullOrEmpty(retr.CodiceTipoQuota) ? retr.CodiceTipoQuota : string.Empty,
                    retr.Settimane.HasValue ? retr.Settimane.Value.ToString() : string.Empty, retr.RMS.HasValue ? retr.RMS.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty,
                    retr.Beneficio.HasValue ? retr.Beneficio.Value.ToString() : string.Empty,
                    retr.IsFromDatiCalcolo.GetValueOrDefault()));
            }
            return elencoDatiRetributivi;
        }

        private static List<DatiContributiviVittimeLocal> MapDatiContributiviForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiContributiviVittimeLocal> elencoDatiContributivi = new List<DatiContributiviVittimeLocal>();
            foreach (GestioneContribDatiContributiviVittimeTerrorismo retr in areaDatiContributivi.DatiCalcoloVittimeTerrorismo.ListaDatiContributiviVittimeTerrorismo)
            {
                elencoDatiContributivi.Add(new DatiContributiviVittimeLocal(retr.DecorrenzaBeneficio.HasValue ? retr.DecorrenzaBeneficio.Value.ToString("MM/yyyy") : string.Empty,
                    retr.CodiceGestioneContr.HasValue ? retr.CodiceGestioneContr.Value.ToString() : string.Empty, retr.Quota.HasValue ? retr.Quota.Value.ToString() : string.Empty,
                    retr.Settimane.HasValue ? retr.Settimane.Value.ToString() : string.Empty, retr.Beneficio.HasValue ? retr.Beneficio.Value.ToString() : string.Empty,
                    retr.Ammontare.HasValue ? retr.Ammontare.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty,
                    retr.Montante.HasValue ? retr.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty
                    , retr.IsFromDatiCalcolo.GetValueOrDefault()
                    ));
            }
            return elencoDatiContributivi;
        }

        private static List<DatiImportoPensioneVittimeLocal> MapDatiImportoPensioneForView(AreaDatiContributivi areaDatiContributivi)
        {
            List<DatiImportoPensioneVittimeLocal> elencoDatiImportoPensione = new List<DatiImportoPensioneVittimeLocal>();
            foreach (GestioneContribDatiImportoPensioneVittimeTerrorismo retr in areaDatiContributivi.DatiCalcoloVittimeTerrorismo.ListaDatiImportoPensioneVittimeTerrorismo)
            {
                elencoDatiImportoPensione.Add(new DatiImportoPensioneVittimeLocal(retr.DecorrenzaBeneficio.HasValue ? retr.DecorrenzaBeneficio.Value.ToString("MM/yyyy") : string.Empty,
                    retr.CodiceGestioneRetr.HasValue ? retr.CodiceGestioneRetr.Value.ToString() : string.Empty, retr.Settimane.HasValue ? retr.Settimane.Value.ToString() : string.Empty,
                    retr.Beneficio.HasValue ? retr.Beneficio.Value.ToString() : string.Empty, retr.ImportoPensione.HasValue ? retr.ImportoPensione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty));
            }
            return elencoDatiImportoPensione;
        }

        private List<GestioneContribDatiRetributiviVittimeTerrorismo> GetDataRetributiviToSave(List<DatiRetributiviVittimeLocal> lDatiRetributiviVittimeLocal)
        {
            List<GestioneContribDatiRetributiviVittimeTerrorismo> lRetr = new List<GestioneContribDatiRetributiviVittimeTerrorismo>();

            foreach (DatiRetributiviVittimeLocal datiRetributiviVittimeLocal in lDatiRetributiviVittimeLocal)
            {
                if ((datiRetributiviVittimeLocal.Beneficio == string.Empty && datiRetributiviVittimeLocal.DecorrenzaBeneficio == string.Empty && datiRetributiviVittimeLocal.Gestione == string.Empty &&
                    datiRetributiviVittimeLocal.Quota == string.Empty && datiRetributiviVittimeLocal.TipoQuota == string.Empty && datiRetributiviVittimeLocal.RMS == string.Empty && datiRetributiviVittimeLocal.Settimane == string.Empty)
                    || (datiRetributiviVittimeLocal.IsFromDatiCalcolo))
                    continue;

                GestioneContribDatiRetributiviVittimeTerrorismo Retr = new GestioneContribDatiRetributiviVittimeTerrorismo();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiRetributiviVittimeLocal.Gestione.Trim() != string.Empty)
                    Retr.CodiceGestioneRetr = Convert.ToInt64(datiRetributiviVittimeLocal.Gestione.Trim());
                else
                    Retr.CodiceGestioneRetr = null;

                Retr.Quota = !string.IsNullOrEmpty(datiRetributiviVittimeLocal.Quota) ? Convert.ToChar(datiRetributiviVittimeLocal.Quota) : (char?)null;
                Retr.CodiceTipoQuota = !string.IsNullOrEmpty(datiRetributiviVittimeLocal.TipoQuota) ? datiRetributiviVittimeLocal.TipoQuota : null;
                Retr.RMS = !string.IsNullOrEmpty(datiRetributiviVittimeLocal.RMS) ? Convert.ToDecimal(datiRetributiviVittimeLocal.RMS.Trim()) : (decimal?)null;
                Retr.Beneficio = !string.IsNullOrEmpty(datiRetributiviVittimeLocal.Beneficio) ? Convert.ToChar(datiRetributiviVittimeLocal.Beneficio.Trim()) : (char?)null;
                Retr.DecorrenzaBeneficio = !string.IsNullOrEmpty(datiRetributiviVittimeLocal.DecorrenzaBeneficio) ? Utility.GetDateFromString(datiRetributiviVittimeLocal.DecorrenzaBeneficio.Trim()) : null;
                Retr.Settimane = !string.IsNullOrEmpty(datiRetributiviVittimeLocal.Settimane) ? Convert.ToInt32(datiRetributiviVittimeLocal.Settimane.Trim()) : (int?)null;

                lRetr.Add(Retr);
            }
            return lRetr;
        }

        private List<GestioneContribDatiContributiviVittimeTerrorismo> GetDataContributiviToSave(List<DatiContributiviVittimeLocal> lDatiContributiviVittimeLocal)
        {
            List<GestioneContribDatiContributiviVittimeTerrorismo> lContr = new List<GestioneContribDatiContributiviVittimeTerrorismo>();

            foreach (DatiContributiviVittimeLocal datiContributiviVittimeLocal in lDatiContributiviVittimeLocal)
            {
                if ((datiContributiviVittimeLocal.Ammontare == string.Empty && datiContributiviVittimeLocal.Beneficio == string.Empty && datiContributiviVittimeLocal.DecorrenzaBeneficio == string.Empty &&
                    datiContributiviVittimeLocal.Gestione == string.Empty && datiContributiviVittimeLocal.Montante == string.Empty && datiContributiviVittimeLocal.Quota == string.Empty &&
                    datiContributiviVittimeLocal.Settimane == string.Empty) ||
                    (datiContributiviVittimeLocal.IsFromDatiCalcolo))
                    continue;

                GestioneContribDatiContributiviVittimeTerrorismo Contr = new GestioneContribDatiContributiviVittimeTerrorismo();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiContributiviVittimeLocal.Gestione.Trim() != string.Empty)
                    Contr.CodiceGestioneContr = Convert.ToInt64(datiContributiviVittimeLocal.Gestione.Trim());
                else
                    Contr.CodiceGestioneContr = null;

                Contr.Quota = !string.IsNullOrEmpty(datiContributiviVittimeLocal.Quota) ? Convert.ToChar(datiContributiviVittimeLocal.Quota) : (char?)null;
                Contr.Ammontare = !string.IsNullOrEmpty(datiContributiviVittimeLocal.Ammontare) ? Convert.ToDecimal(datiContributiviVittimeLocal.Ammontare.Trim()) : (decimal?)null;
                Contr.Beneficio = !string.IsNullOrEmpty(datiContributiviVittimeLocal.Beneficio) ? Convert.ToChar(datiContributiviVittimeLocal.Beneficio.Trim()) : (char?)null;
                Contr.DecorrenzaBeneficio = !string.IsNullOrEmpty(datiContributiviVittimeLocal.DecorrenzaBeneficio) ? Utility.GetDateFromString(datiContributiviVittimeLocal.DecorrenzaBeneficio.Trim()) : null;
                Contr.Montante = !string.IsNullOrEmpty(datiContributiviVittimeLocal.Montante) ? Convert.ToDecimal(datiContributiviVittimeLocal.Montante.Trim()) : (decimal?)null;
                Contr.Settimane = !string.IsNullOrEmpty(datiContributiviVittimeLocal.Settimane) ? Convert.ToInt32(datiContributiviVittimeLocal.Settimane.Trim()) : (int?)null;

                lContr.Add(Contr);
            }
            return lContr;
        }

        private List<GestioneContribDatiImportoPensioneVittimeTerrorismo> GetDataImportoPensioneToSave(List<DatiImportoPensioneVittimeLocal> lDatiImportoPensioneVittimeLocal)
        {
            List<GestioneContribDatiImportoPensioneVittimeTerrorismo> lImp = new List<GestioneContribDatiImportoPensioneVittimeTerrorismo>();

            foreach (DatiImportoPensioneVittimeLocal datiImportoPensioneVittimeLocal in lDatiImportoPensioneVittimeLocal)
            {
                if (datiImportoPensioneVittimeLocal.Beneficio == string.Empty && datiImportoPensioneVittimeLocal.DecorrenzaBeneficio == string.Empty && datiImportoPensioneVittimeLocal.Gestione == string.Empty &&
                    datiImportoPensioneVittimeLocal.ImportoPensione == string.Empty && datiImportoPensioneVittimeLocal.Settimane == string.Empty)
                    continue;

                GestioneContribDatiImportoPensioneVittimeTerrorismo Imp = new GestioneContribDatiImportoPensioneVittimeTerrorismo();

                // reperire l'id del campo ddl Cod Gestione selezionato
                if (datiImportoPensioneVittimeLocal.Gestione.Trim() != string.Empty)
                    Imp.CodiceGestioneRetr = Convert.ToInt64(datiImportoPensioneVittimeLocal.Gestione.Trim());
                else
                    Imp.CodiceGestioneRetr = null;

                Imp.ImportoPensione = !string.IsNullOrEmpty(datiImportoPensioneVittimeLocal.ImportoPensione) ? Convert.ToDecimal(datiImportoPensioneVittimeLocal.ImportoPensione.Trim()) : (decimal?)null;
                Imp.Beneficio = !string.IsNullOrEmpty(datiImportoPensioneVittimeLocal.Beneficio) ? Convert.ToChar(datiImportoPensioneVittimeLocal.Beneficio.Trim()) : (char?)null;
                Imp.DecorrenzaBeneficio = !string.IsNullOrEmpty(datiImportoPensioneVittimeLocal.DecorrenzaBeneficio) ? Utility.GetDateFromString(datiImportoPensioneVittimeLocal.DecorrenzaBeneficio.Trim()) : null;
                Imp.Settimane = !string.IsNullOrEmpty(datiImportoPensioneVittimeLocal.Settimane) ? Convert.ToInt32(datiImportoPensioneVittimeLocal.Settimane.Trim()) : (int?)null;

                lImp.Add(Imp);
            }
            return lImp;
        }

        private bool ControlCodGestioneQuotaTipoQuota(string codiceGestione, string quota, string tipoQuota)
        {
            string codiceGestioneTraduzioneSuGP = string.Empty;

            if (string.IsNullOrEmpty(tipoQuota))
                tipoQuota = null;

            DecodificaGestioneCalcoloRetributivo[] listaCodeGestioneCalcoloRetrib = (DecodificaGestioneCalcoloRetributivo[])ViewState[EnumViewState.ElencoCodeGestioneCalcoloRetrib.ToString()];
            DecodificaGestioneCalcoloRetributivo app = listaCodeGestioneCalcoloRetrib.ToList().Find(delegate(DecodificaGestioneCalcoloRetributivo code) { return (code.Id.ToString() == codiceGestione); });
            if (app != null)
                codiceGestioneTraduzioneSuGP = app.TraduzioneSuGP.Trim();

            List<CtrlDecorrenzaRetrExINPDAI> listaCtrlDecorrenzaRetrExINPDAI = (List<CtrlDecorrenzaRetrExINPDAI>)ViewState[EnumViewState.CtrlDecorrenzaRetrExINPDAI.ToString()];
            if (listaCtrlDecorrenzaRetrExINPDAI != null && listaCtrlDecorrenzaRetrExINPDAI.Count > 0)
            {
                CtrlDecorrenzaRetrExINPDAI obj = listaCtrlDecorrenzaRetrExINPDAI.Find(x => x.Gestione.Trim() == codiceGestioneTraduzioneSuGP && x.Quota.ToString() == quota && x.TipoQuota == tipoQuota);
                if (obj != null)
                    return true;
            }
            return false;
        }
        #endregion private methods

        #region Events

        public event Utility.EventHandlerMessage GestisciTastoSalva;
        public event EventHandler ShowAvviso;
        public event EventHandler InitializeData;
        public event EventHandler HideAvviso;

        protected void RaiseGestisciTastoSalva(object sender, Utility.EventMessageArgs e)
        {
            if (GestisciTastoSalva != null)
                GestisciTastoSalva(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            HideAvviso(sender, e);
        }

        #endregion Events

        #region nested Class
        [Serializable]
        public class DatiRetributiviVittimeLocal
        {
            public DatiRetributiviVittimeLocal()
            { }

            public DatiRetributiviVittimeLocal(string strDecorrenzaBeneficio, string strGestione, string strQuota, string strTipoQuota, string strSettimane, string strRMS, string strBeneficio, bool isFromDatiCalcolo)
            {
                this._strDecorrenzaBeneficio = strDecorrenzaBeneficio;
                this._strGestione = strGestione;
                this._strQuota = strQuota;
                this._strTipoQuota = strTipoQuota;
                this._strSettimane = strSettimane;
                this._strRMS = strRMS;
                this._strBeneficio = strBeneficio;
                this._isFromDatiCalcolo = isFromDatiCalcolo;
            }

            #region private properties

            private string _strDecorrenzaBeneficio;
            private string _strGestione;
            private string _strQuota;
            private string _strTipoQuota;
            private string _strSettimane;
            private string _strRMS;
            private string _strBeneficio;
            private bool _isFromDatiCalcolo;

            #endregion private properties

            #region public properties

            public string DecorrenzaBeneficio { get { return _strDecorrenzaBeneficio; } set { _strDecorrenzaBeneficio = value; } }
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string TipoQuota { get { return _strTipoQuota; } set { _strTipoQuota = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string RMS { get { return _strRMS; } set { _strRMS = value; } }
            public string Beneficio { get { return _strBeneficio; } set { _strBeneficio = value; } }
            public bool IsFromDatiCalcolo { get { return _isFromDatiCalcolo; } set { _isFromDatiCalcolo = value; } }

            #endregion public properties

            #region Public methods
            public bool isEmpty()
            {
                return string.IsNullOrEmpty(this._strDecorrenzaBeneficio) &&
                        string.IsNullOrEmpty(this._strGestione) &&
                        string.IsNullOrEmpty(this._strQuota) &&
                        string.IsNullOrEmpty(this._strTipoQuota) &&
                        string.IsNullOrEmpty(this._strSettimane) &&
                        string.IsNullOrEmpty(this._strRMS) &&
                        string.IsNullOrEmpty(this._strBeneficio);
            }
            #endregion Public methods

        }

        [Serializable]
        public class DatiContributiviVittimeLocal
        {
            public DatiContributiviVittimeLocal()
            { }

            public DatiContributiviVittimeLocal(string strDecorrenzaBeneficio, string strGestione, string strQuota, string strSettimane, string strBeneficio, string strAmmontare, string strMontante, bool isFromDatiCalcolo, string strPL_Quotac = null)
            {
                this._strDecorrenzaBeneficio = strDecorrenzaBeneficio;
                this._strGestione = strGestione;
                this._strQuota = strQuota;
                this._strSettimane = strSettimane;
                this._strBeneficio = strBeneficio;
                this._strAmmontare = strAmmontare;
                this._strMontante = strMontante;
                this._isFromDatiCalcolo = isFromDatiCalcolo;
                this._strPL_Quotac = strPL_Quotac;
            }

            #region private properties

            private string _strDecorrenzaBeneficio;
            private string _strGestione;
            private string _strQuota;
            private string _strSettimane;
            private string _strBeneficio;
            private string _strAmmontare;
            private string _strMontante;
            private bool _isFromDatiCalcolo;
            private string _strPL_Quotac;

            #endregion private properties

            #region public properties

            public string DecorrenzaBeneficio { get { return _strDecorrenzaBeneficio; } set { _strDecorrenzaBeneficio = value; } }
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Quota { get { return _strQuota; } set { _strQuota = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string Beneficio { get { return _strBeneficio; } set { _strBeneficio = value; } }
            public string Ammontare { get { return _strAmmontare; } set { _strAmmontare = value; } }
            public string Montante { get { return _strMontante; } set { _strMontante = value; } }
            public bool IsFromDatiCalcolo { get { return _isFromDatiCalcolo; } set { _isFromDatiCalcolo = value; } }
            public string PL_Quotac { get { return _strPL_Quotac; } set { _strPL_Quotac = value; } }

            #endregion public properties

            #region Public methods
            public bool isEmpty()
            {
                return string.IsNullOrEmpty(_strDecorrenzaBeneficio) &&
                        string.IsNullOrEmpty(_strGestione) &&
                        string.IsNullOrEmpty(_strQuota) &&
                        string.IsNullOrEmpty(_strSettimane) &&
                        string.IsNullOrEmpty(_strBeneficio) &&
                        string.IsNullOrEmpty(_strAmmontare) &&
                        string.IsNullOrEmpty(_strMontante);
            }
            #endregion Public methods

        }

        [Serializable]
        public class DatiImportoPensioneVittimeLocal
        {
            public DatiImportoPensioneVittimeLocal()
            { }

            public DatiImportoPensioneVittimeLocal(string strDecorrenzaBeneficio, string strGestione, string strSettimane, string strBeneficio, string strImportoPensione, string strPL_Quotar = null, string strPL_Quotar707 = null)
            {
                this._strDecorrenzaBeneficio = strDecorrenzaBeneficio;
                this._strGestione = strGestione;
                this._strSettimane = strSettimane;
                this._strBeneficio = strBeneficio;
                this._strImportoPensione = strImportoPensione;
                this._strPL_Quotar = strPL_Quotar;
                this._strPL_Quotar707 = strPL_Quotar707;
            }

            #region private properties

            private string _strDecorrenzaBeneficio;
            private string _strGestione;
            private string _strSettimane;
            private string _strBeneficio;
            private string _strImportoPensione;
            private string _strPL_Quotar;
            private string _strPL_Quotar707;
            #endregion private properties

            #region public properties

            public string DecorrenzaBeneficio { get { return _strDecorrenzaBeneficio; } set { _strDecorrenzaBeneficio = value; } }
            public string Gestione { get { return _strGestione; } set { _strGestione = value; } }
            public string Settimane { get { return _strSettimane; } set { _strSettimane = value; } }
            public string Beneficio { get { return _strBeneficio; } set { _strBeneficio = value; } }
            public string ImportoPensione { get { return _strImportoPensione; } set { _strImportoPensione = value; } }
            public string PL_Quotar { get { return _strPL_Quotar; } set { _strPL_Quotar = value; } }
            public string PL_Quotar707 { get { return _strPL_Quotar707; } set { _strPL_Quotar707 = value; } }
            #endregion public properties

        }

        #endregion nested Class

        #region Enum

        public enum EnumViewState
        {
            ElencoDatiRetributiviVittime,
            ElencoDatiContributiviVittime,
            ElencoDatiImportoPensioneVittime,
            ElencoCodeGestioneCalcoloRetrib,
            ElencoCodeGestioneCalcoloContrib,
            AreaDatiContributiviAgo,
            IsBeneficioImportoPensioneX,
            IsBeneficioVittimeTerrorismo,
            DecodificaTipoQuota,
            CtrlDecorrenzaRetrExINPDAI
        }

        public enum ColonneGvDatiRetributiviVittime
        {
            TipoQuota = 3,
        }

        #endregion Enum



    }
}