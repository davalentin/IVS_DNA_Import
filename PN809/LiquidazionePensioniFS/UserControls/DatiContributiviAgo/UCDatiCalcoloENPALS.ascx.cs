using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCDatiCalcoloENPALS : CustomBaseUserControl, IDatiContributiviAgo
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

        public void ValorizzaEtichetteDatiCalcoloENPALS(IDatiContributiviAgo Dati, bool abilitaPulsanteElimina)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] = Dati.areaDatiContributiviAgo;

            BindDataForPanels(Dati.areaDatiContributiviAgo);

            ManagePulsanti();

            btnEliminaDatiCalcolo.Enabled = abilitaPulsanteElimina;

            if ((this.domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S")) ||
                Utility.IsDomandaRipristino(datiPensione))
                btnEliminaDatiCalcolo.Enabled = false;
        }

        internal GestioneContribDatiCalcoloENPALS GetDatiCalcoloENPALS()
        {
            GestioneContribDatiCalcoloENPALS datiCalcoloENPALS = new GestioneContribDatiCalcoloENPALS();

            if ((List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] != null)
                datiCalcoloENPALS.LDatiRetributivi = RemoveItemBlank((List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()]).ToArray();

            if ((List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()] != null)
                datiCalcoloENPALS.LDatiContributivi = RemoveItemBlank((List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()]).ToArray();

            if (!string.IsNullOrEmpty(txtImportoProRataTemporis.Text))
                datiCalcoloENPALS.ImportoProRataTemporis = decimal.Parse(txtImportoProRataTemporis.Text);

            if (!string.IsNullOrEmpty(txtImportoQuotaRetributivaInMisto.Text))
                datiCalcoloENPALS.ImportoQuotaRetributivaInMisto = decimal.Parse(txtImportoQuotaRetributivaInMisto.Text);

            if (!string.IsNullOrEmpty(txtImportoPensione.Text))
                datiCalcoloENPALS.ImportoPensione = decimal.Parse(txtImportoPensione.Text);

            if (!string.IsNullOrEmpty(txtImportoPensione707.Text))
                datiCalcoloENPALS.ImportoPensione707 = decimal.Parse(txtImportoPensione707.Text);

            if (!string.IsNullOrEmpty(txtImportoIIS.Text))
                datiCalcoloENPALS.ImportoIIS = decimal.Parse(txtImportoIIS.Text);

            if (!string.IsNullOrEmpty(txtDecorrenzaImportoIIS.Text))
                datiCalcoloENPALS.DecorrenzaImportoIIS = Utility.GetDateFromString(txtDecorrenzaImportoIIS.Text);

            return datiCalcoloENPALS;
        }

        public void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiContributiviAgo == null)
                this.areaDatiContributiviAgo = new AreaDatiContributivi();

            this.areaDatiContributiviAgo.DatiCalcoloENPALS = GetDatiCalcoloENPALS();

            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            presenterDatiContributiviAgo.SalvaDatiCalcolo(this);

            if (!this.HasError)
                btnEliminaDatiCalcolo.Enabled = true;

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        public void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            presenterDatiContributiviAgo.EliminaDatiCalcolo(this);

            if (!this.HasError)
                btnEliminaDatiCalcolo.Enabled = false;

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo eliminati correttamente.";
                RaiseShowAvviso(this, null);
                CleanData();
                BindDataForPanels((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]);
            }
        }

        public void ManagePulsanti()
        {
            if (this.areaDatiContributiviAgo.DatiCalcolo.TipoCalcolo != GestioneContribTipoCalcolo.NonValido)
            {
                btnSalvaDatiCalcolo.Enabled = true;
                btnEliminaDatiCalcolo.Enabled = true;
            }
            else
            {
                btnSalvaDatiCalcolo.Enabled = false;
                btnEliminaDatiCalcolo.Enabled = false;
            }
        }

        internal void DisabilitaPulsanti()
        {
            btnSalvaDatiCalcolo.Enabled = false;
            btnEliminaDatiCalcolo.Enabled = false;
        }

        #region private methods
        private void BindDataForPanels(AreaDatiContributivi areaDatiContributivi)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
            {
                txtImportoProRataTemporis.Enabled = true;
                txtImportoQuotaRetributivaInMisto.Enabled = true;
                txtImportoPensione.Enabled = true;
                txtImportoPensione707.Enabled = true;
                txtImportoIIS.Enabled = true;
                txtDecorrenzaImportoIIS.Enabled = true;
            }

            if (areaDatiContributivi != null)
            {
                if (areaDatiContributivi.DatiCalcoloENPALS != null)
                {
                    if (areaDatiContributivi.DatiCalcoloENPALS.ImportoPensione.HasValue)
                        txtImportoPensione.Text = areaDatiContributivi.DatiCalcoloENPALS.ImportoPensione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (areaDatiContributivi.DatiCalcoloENPALS.ImportoPensione707.HasValue)
                        txtImportoPensione707.Text = areaDatiContributivi.DatiCalcoloENPALS.ImportoPensione707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    if (areaDatiContributivi.DatiCalcoloENPALS.ImportoIIS.HasValue)
                        txtImportoIIS.Text = areaDatiContributivi.DatiCalcoloENPALS.ImportoIIS.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                    lblDecorrenzaImportoPensione.Text = areaDatiContributivi.DatiCalcoloENPALS.DecorrenzaImportoPensione;
                    if (areaDatiContributivi.DatiCalcoloENPALS.DecorrenzaImportoIIS.HasValue)
                        txtDecorrenzaImportoIIS.Text = String.Format("{0:dd/MM/yyyy}", areaDatiContributivi.DatiCalcoloENPALS.DecorrenzaImportoIIS);
                }

                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                        pdivRetributivo.Visible = true;
                        InitBindDataRetributivi();
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                        pdivRetributivo.Visible = true;
                        pdivContributivo.Visible = true;
                        pdivMisto.Visible = true;
                        InitBindDataRetributivi();
                        InitBindDataContributivi();
                        if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.ImportoQuotaRetributivaInMisto.HasValue)
                            txtImportoQuotaRetributivaInMisto.Text = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.ImportoQuotaRetributivaInMisto.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        break;
                    case GestioneContribTipoCalcolo.MistoL214:
                        pdivRetributivo.Visible = true;
                        pdivContributivo.Visible = true;
                        pdivMisto.Visible = true;
                        InitBindDataRetributivi();
                        InitBindDataContributivi();
                        trImportoPensione707.Visible = true;
                        if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.ImportoQuotaRetributivaInMisto.HasValue)
                            txtImportoQuotaRetributivaInMisto.Text = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.ImportoQuotaRetributivaInMisto.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        break;
                    case GestioneContribTipoCalcolo.RetributivoComma707:
                        pdivContributivo.Visible = true;
                        InitBindDataContributivi();
                        pdivRetributivo.Visible = true;
                        InitBindDataRetributivi();
                        trImportoPensione707.Visible = true;
                        break;
                    case GestioneContribTipoCalcolo.NonValido:
                        pdivRetributivo.Visible = false;
                        pdivContributivo.Visible = false;
                        pdivMisto.Visible = false;
                        InitBindDataRetributivi();
                        InitBindDataContributivi();
                        break;
                }
            }
        }

        private void InitBindDataRetributivi()
        {
            List<GestioneContribDatiRetributiviENPALS> elencoDatiRetributivi = new List<GestioneContribDatiRetributiviENPALS>();

            if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.LDatiRetributivi != null)
                elencoDatiRetributivi = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.LDatiRetributivi.ToList();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
            {
                if (elencoDatiRetributivi.Count < 2)
                    elencoDatiRetributivi.Add(new GestioneContribDatiRetributiviENPALS());
                gvDatiRetributivi.AutoGenerateEditButton = true;
            }

            if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.ImportoProRataTemporis.HasValue)
                txtImportoProRataTemporis.Text = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.ImportoProRataTemporis.ToString();

            gvDatiRetributivi.DataSource = elencoDatiRetributivi;
            ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = elencoDatiRetributivi;
            gvDatiRetributivi.DataBind();
        }

        private void InitBindDataContributivi()
        {
            List<GestioneContribDatiContributiviENPALS> elencoDatiContributivi = new List<GestioneContribDatiContributiviENPALS>();

            if (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.LDatiContributivi != null)
                elencoDatiContributivi = ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcoloENPALS.LDatiContributivi.ToList();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
            {
                if (elencoDatiContributivi.Count < 1)
                    elencoDatiContributivi.Add(new GestioneContribDatiContributiviENPALS());
                gvDatiContributivi.AutoGenerateEditButton = true;
            }

            gvDatiContributivi.DataSource = elencoDatiContributivi;
            ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = elencoDatiContributivi;
            gvDatiContributivi.DataBind();
        }

        private bool IsListaEmpty(char tipo)
        {
            switch (tipo)
            {
                case 'R':
                    List<GestioneContribDatiRetributiviENPALS> listaDatiRetrApp = (List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                    if (listaDatiRetrApp == null || (listaDatiRetrApp.Count == 1 && string.IsNullOrEmpty(listaDatiRetrApp[0].Decorrenza) &&
                        !listaDatiRetrApp[0].Giorni707.HasValue && !listaDatiRetrApp[0].Importo.HasValue && !listaDatiRetrApp[0].Importo707.HasValue &&
                        !listaDatiRetrApp[0].NTotaleContributiCalcolo.HasValue && !listaDatiRetrApp[0].Periodi.HasValue && !listaDatiRetrApp[0].Quota.HasValue &&
                        !listaDatiRetrApp[0].RM.HasValue))
                        return true;
                    else
                        return false;
                case 'C':
                    List<GestioneContribDatiContributiviENPALS> listaDatiContrApp = (List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                    if (listaDatiContrApp == null || (listaDatiContrApp.Count == 1 && !listaDatiContrApp[0].CoefficienteTrasformazione.HasValue &&
                        string.IsNullOrEmpty(listaDatiContrApp[0].Decorrenza) && !listaDatiContrApp[0].ImportoContributivoTotale.HasValue && !listaDatiContrApp[0].Montante.HasValue &&
                        !listaDatiContrApp[0].NumeroContributiTotale.HasValue && !listaDatiContrApp[0].Quota.HasValue))
                        return true;
                    else
                        return false;
            }

            return false;
        }

        private bool IsEmptyEditableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0) ||
                (row.FindControl("txtNSettimane") != null && ((TextBox)row.FindControl("txtNSettimane")).Text != string.Empty) ||
                (row.FindControl("txtNTotaleContributiCalcolo") != null && ((TextBox)row.FindControl("txtNTotaleContributiCalcolo")).Text != string.Empty) ||
                (row.FindControl("txtRMS") != null && ((TextBox)row.FindControl("txtRMS")).Text != string.Empty) ||
                (row.FindControl("txtImporto") != null && ((TextBox)row.FindControl("txtImporto")).Text != string.Empty) ||
                (row.FindControl("txtNSettimane707") != null && ((TextBox)row.FindControl("txtNSettimane707")).Text != string.Empty) ||
                (row.FindControl("txtImporto707") != null && ((TextBox)row.FindControl("txtImporto707")).Text != string.Empty)
                )

                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowContrib(GridViewRow row)
        {
            if ((row.FindControl("ddlQuota") != null && ((DropDownList)row.FindControl("ddlQuota")).SelectedIndex != 0) ||
                (row.FindControl("txtNumeroContributiTotale") != null && ((TextBox)row.FindControl("txtNumeroContributiTotale")).Text != string.Empty) ||
                (row.FindControl("txtImportoContributivoTotale") != null && ((TextBox)row.FindControl("txtImportoContributivoTotale")).Text != string.Empty) ||
                (row.FindControl("txtMontante") != null && ((TextBox)row.FindControl("txtMontante")).Text != string.Empty) ||
                (row.FindControl("txtCoefficienteTrasformazione") != null && ((TextBox)row.FindControl("txtCoefficienteTrasformazione")).Text != string.Empty)
                )

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowRetrib(GridViewRow row)
        {
            if ((row.FindControl("lblQuota") != null && ((Label)row.FindControl("lblQuota")).Text != string.Empty) ||
                (row.FindControl("lblNSettimane") != null && ((Label)row.FindControl("lblNSettimane")).Text != string.Empty) ||
                (row.FindControl("lblNTotaleContributiCalcolo") != null && ((Label)row.FindControl("lblNTotaleContributiCalcolo")).Text != string.Empty) ||
                (row.FindControl("lblRMS") != null && ((Label)row.FindControl("lblRMS")).Text != string.Empty) ||
                (row.FindControl("lblImporto") != null && ((Label)row.FindControl("lblImporto")).Text != string.Empty) ||
                (row.FindControl("lblNSettimane707") != null && ((Label)row.FindControl("lblNSettimane707")).Text != string.Empty) ||
                (row.FindControl("lblImporto707") != null && ((Label)row.FindControl("lblImporto707")).Text != string.Empty)
                )

                return false;
            else
                return true;
        }

        private bool IsEmptyReadableRowContrib(GridViewRow row)
        {
            if ((row.FindControl("lblQuota") != null && ((Label)row.FindControl("lblQuota")).Text != string.Empty) ||
                (row.FindControl("lblNumeroContributiTotale") != null && ((Label)row.FindControl("lblNumeroContributiTotale")).Text != string.Empty) ||
                (row.FindControl("lblImportoContributivoTotale") != null && ((Label)row.FindControl("lblImportoContributivoTotale")).Text != string.Empty) ||
                (row.FindControl("lblMontante") != null && ((Label)row.FindControl("lblMontante")).Text != string.Empty) ||
                (row.FindControl("lblCoefficienteTrasformazione") != null && ((Label)row.FindControl("lblCoefficienteTrasformazione")).Text != string.Empty)
                )

                return false;
            else
                return true;
        }

        private void CleanData()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
            {
                txtImportoProRataTemporis.Text = string.Empty;
                txtImportoQuotaRetributivaInMisto.Text = string.Empty;
                txtImportoPensione.Text = string.Empty;
                txtImportoPensione707.Text = string.Empty;
                txtImportoIIS.Text = string.Empty;
                txtDecorrenzaImportoIIS.Text = string.Empty;

                AreaDatiContributivi areaDatiContributivi = (AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()];
                if (areaDatiContributivi != null)
                {
                    string decorrenzaImportoPensione = areaDatiContributivi.DatiCalcoloENPALS.DecorrenzaImportoPensione;
                    areaDatiContributivi.DatiCalcoloENPALS = new GestioneContribDatiCalcoloENPALS();
                    areaDatiContributivi.DatiCalcoloENPALS.DecorrenzaImportoPensione = decorrenzaImportoPensione;
                    areaDatiContributivi.DatiCalcoloENPALS.LDatiRetributivi = new GestioneContribDatiRetributiviENPALS[0];
                    areaDatiContributivi.DatiCalcoloENPALS.LDatiContributivi = new GestioneContribDatiContributiviENPALS[0];
                }
            }
        }

        private bool IsNull(GestioneContribDatiRetributiviENPALS obj)
        {
            if (!string.IsNullOrEmpty(obj.Decorrenza) || obj.Giorni707.HasValue || obj.Importo.HasValue || obj.Importo707.HasValue || obj.NTotaleContributiCalcolo.HasValue ||
                obj.Periodi.HasValue || obj.Quota.HasValue || obj.RM.HasValue)
                return false;

            return true;
        }

        public bool IsNull(GestioneContribDatiContributiviENPALS obj)
        {
            if (obj.CoefficienteTrasformazione.HasValue || !string.IsNullOrEmpty(obj.Decorrenza) || obj.ImportoContributivoTotale.HasValue ||
                obj.Montante.HasValue || obj.NumeroContributiTotale.HasValue || obj.Quota.HasValue)
                return false;

            return true;
        }

        public List<GestioneContribDatiRetributiviENPALS> RemoveItemBlank(List<GestioneContribDatiRetributiviENPALS> list)
        {
            List<GestioneContribDatiRetributiviENPALS> listToReturn = new List<GestioneContribDatiRetributiviENPALS>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                    if (!IsNull(item))
                        listToReturn.Add(item);
            }

            return listToReturn;
        }

        public List<GestioneContribDatiContributiviENPALS> RemoveItemBlank(List<GestioneContribDatiContributiviENPALS> list)
        {
            List<GestioneContribDatiContributiviENPALS> listToReturn = new List<GestioneContribDatiContributiviENPALS>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                    if (!IsNull(item))
                        listToReturn.Add(item);
            }

            return listToReturn;
        }
        #endregion private methods

        #region gvContributivi
        protected void gvDatiContributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                    if (!CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
                    {
                        ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                        ((Label)e.Row.FindControl("lblNumeroContributiTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).NumeroContributiTotale.ToString();
                        if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue)
                            ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.HasValue)
                            ((Label)e.Row.FindControl("lblMontante")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                        if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue)
                            ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                    }
                    //prima riga
                    else if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty('C') && gvDatiContributivi.EditIndex == -1)
                        {
                            gvDatiContributivi.EditIndex = 0;

                            gvDatiContributivi.DataSource = (List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                            gvDatiContributivi.DataBind();
                        }
                        else if (IsEmptyEditableRowContrib(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloENPALSContr", Page.Theme);
                                LinkButton delete = ((LinkButton)(e.Row.FindControl("btnDeleteContributivi")));
                                delete.Text = string.Empty;
                                ((DropDownList)e.Row.FindControl("ddlQuota")).SelectedValue = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                                ((Label)e.Row.FindControl("lblNumeroContributiTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).NumeroContributiTotale.ToString();
                                if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue)
                                    ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                                if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.HasValue)
                                    ((Label)e.Row.FindControl("lblMontante")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                                if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue)
                                    ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteContributivi");
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                ((DropDownList)e.Row.FindControl("ddlQuota")).SelectedValue = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloENPALSContr", Page.Theme);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                                ((Label)e.Row.FindControl("lblNumeroContributiTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).NumeroContributiTotale.ToString();
                                if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue)
                                    ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                                if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.HasValue)
                                    ((Label)e.Row.FindControl("lblMontante")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                                if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue)
                                    ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteContributivi");
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            ((DropDownList)e.Row.FindControl("ddlQuota")).SelectedValue = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloENPALSContr", Page.Theme);
                        }
                        // Per ENPALS viene gestito un unico record contributivo
                        //else if (IsNull((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)) &&
                        //    e.Row.DataItemIndex == ((List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()]).Count - 1)
                        //{
                        //    LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        //    add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                        //    add.ToolTip = "Aggiungi";
                        //}
                        else
                        {
                            ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                            ((Label)e.Row.FindControl("lblNumeroContributiTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).NumeroContributiTotale.ToString();
                            if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.HasValue)
                                ((Label)e.Row.FindControl("lblImportoContributivoTotale")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).ImportoContributivoTotale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                            if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.HasValue)
                                ((Label)e.Row.FindControl("lblMontante")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
                            if (((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.HasValue)
                                ((Label)e.Row.FindControl("lblCoefficienteTrasformazione")).Text = ((GestioneContribDatiContributiviENPALS)(e.Row.DataItem)).CoefficienteTrasformazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], Page.Theme, "btnDeleteContributivi");
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
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgoENPALS, Errore nel metodo gvDatiContributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiContributivi_DataBound(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (!CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
                gvDatiContributivi.Columns[(int)ColonneGvDatiContributivi.Elimina].Visible = false;
        }

        protected void gvDatiContributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<GestioneContribDatiContributiviENPALS> listaDatiContribApp = new List<GestioneContribDatiContributiviENPALS>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    if (!IsEmptyReadableRowContrib(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            listaDatiContribApp.Add(new GestioneContribDatiContributiviENPALS
                            {
                                Quota = CodeUtility.StringToNullableChar(((Label)rApp.FindControl("lblQuota")).Text),
                                NumeroContributiTotale = CodeUtility.StringToNullableInt(((Label)rApp.FindControl("lblNumeroContributiTotale")).Text),
                                ImportoContributivoTotale = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblImportoContributivoTotale")).Text),
                                Montante = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblMontante")).Text),
                                CoefficienteTrasformazione = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblCoefficienteTrasformazione")).Text)
                            });
                        }
                    }
                    else if (!IsEmptyEditableRowContrib(rApp))
                    {
                        List<GestioneContribDatiContributiviENPALS> datiContributivi = (List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                        if (datiContributivi != null && datiContributivi.Count - 1 > rApp.DataItemIndex)
                        {
                            if (rApp.DataItemIndex != r.DataItemIndex)
                            {
                                listaDatiContribApp.Add(new GestioneContribDatiContributiviENPALS
                                {
                                    Quota = datiContributivi[rApp.DataItemIndex].Quota,
                                    NumeroContributiTotale = datiContributivi[rApp.DataItemIndex].NumeroContributiTotale,
                                    ImportoContributivoTotale = datiContributivi[rApp.DataItemIndex].ImportoContributivoTotale,
                                    Montante = datiContributivi[rApp.DataItemIndex].Montante,
                                    CoefficienteTrasformazione = datiContributivi[rApp.DataItemIndex].CoefficienteTrasformazione
                                });
                            }
                        }
                    }
                }

                listaDatiContribApp.Add(new GestioneContribDatiContributiviENPALS());

                ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContribApp;
                gvDatiContributivi.EditIndex = -1;
                gvDatiContributivi.DataSource = listaDatiContribApp;
                gvDatiContributivi.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowContrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<GestioneContribDatiContributiviENPALS> listaDatiContrApp = new List<GestioneContribDatiContributiviENPALS>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        if (!IsEmptyEditableRowContrib(rApp))
                        {
                            listaDatiContrApp.Add(new GestioneContribDatiContributiviENPALS
                            {
                                Quota = CodeUtility.StringToNullableChar(((DropDownList)rApp.FindControl("ddlQuota")).SelectedValue),
                                NumeroContributiTotale = CodeUtility.StringToNullableInt(((TextBox)rApp.FindControl("txtNumeroContributiTotale")).Text),
                                ImportoContributivoTotale = CodeUtility.StringToNullableDecimal(((TextBox)rApp.FindControl("txtImportoContributivoTotale")).Text),
                                Montante = CodeUtility.StringToNullableDecimal(((TextBox)rApp.FindControl("txtMontante")).Text),
                                CoefficienteTrasformazione = CodeUtility.StringToNullableDecimal(((TextBox)rApp.FindControl("txtCoefficienteTrasformazione")).Text)
                            });
                        }
                        else if (!IsEmptyReadableRowContrib(rApp))
                        {
                            listaDatiContrApp.Add(new GestioneContribDatiContributiviENPALS
                            {
                                Quota = CodeUtility.StringToNullableChar(((Label)rApp.FindControl("lblQuota")).Text),
                                NumeroContributiTotale = CodeUtility.StringToNullableInt(((Label)rApp.FindControl("lblNumeroContributiTotale")).Text),
                                ImportoContributivoTotale = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblImportoContributivoTotale")).Text),
                                Montante = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblMontante")).Text),
                                CoefficienteTrasformazione = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblCoefficienteTrasformazione")).Text)
                            });
                        }
                    }
                    if (listaDatiContrApp.Count < 1)
                        listaDatiContrApp.Add(new GestioneContribDatiContributiviENPALS());
                    gvDatiContributivi.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiContributivi.ToString()] = listaDatiContrApp;
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<GestioneContribDatiContributiviENPALS> listaDatiContrApp = (List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                if (!IsListaEmpty('C'))
                {
                    gvDatiContributivi.EditIndex = -1;
                    gvDatiContributivi.DataSource = listaDatiContrApp;
                    gvDatiContributivi.DataBind();
                }
            }
        }

        protected void gvDatiContributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiContributivi.EditIndex = e.NewEditIndex;
                List<GestioneContribDatiContributiviENPALS> listaDatiContrApp = (List<GestioneContribDatiContributiviENPALS>)ViewState[EnumViewState.ElencoDatiContributivi.ToString()];
                gvDatiContributivi.DataSource = listaDatiContrApp;
                gvDatiContributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviENPALS, Errore nel metodo gvDatiContributivi_RowEditing " + ex);
            }
        }
        #endregion gvContributivi

        #region gvRetributivi
        protected void gvDatiRetributivi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                    if (!CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
                    {
                        ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                        ((Label)e.Row.FindControl("lblNSettimane")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Periodi.ToString();
                        ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString();
                        ((Label)e.Row.FindControl("lblRMS")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).RM.ToString();
                        ((Label)e.Row.FindControl("lblImporto")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo.ToString();
                        ((Label)e.Row.FindControl("lblNSettimane707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Giorni707.ToString();
                        ((Label)e.Row.FindControl("lblImporto707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo707.ToString();
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Decorrenza;
                    }
                    //prima riga
                    else if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty('R') && gvDatiRetributivi.EditIndex == -1)
                        {
                            gvDatiRetributivi.EditIndex = 0;

                            gvDatiRetributivi.DataSource = (List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                            gvDatiRetributivi.DataBind();
                        }
                        else if (IsEmptyEditableRowRetrib(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloENPALSRetr", Page.Theme);
                                LinkButton delete = ((LinkButton)(e.Row.FindControl("btnDeleteRetributivi")));
                                delete.Text = string.Empty;
                                ((DropDownList)e.Row.FindControl("ddlQuota")).SelectedValue = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                                ((Label)e.Row.FindControl("lblNSettimane")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Periodi.ToString();
                                ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString();
                                ((Label)e.Row.FindControl("lblRMS")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).RM.ToString();
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo.ToString();
                                ((Label)e.Row.FindControl("lblNSettimane707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Giorni707.ToString();
                                ((Label)e.Row.FindControl("lblImporto707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo707.ToString();
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Decorrenza;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[9], Page.Theme, "btnDeleteRetributivi");
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                ((DropDownList)e.Row.FindControl("ddlQuota")).SelectedValue = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                                CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloENPALSRetr", Page.Theme);
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                                ((Label)e.Row.FindControl("lblNSettimane")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Periodi.ToString();
                                ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString();
                                ((Label)e.Row.FindControl("lblRMS")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).RM.ToString();
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo.ToString();
                                ((Label)e.Row.FindControl("lblNSettimane707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Giorni707.ToString();
                                ((Label)e.Row.FindControl("lblImporto707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo707.ToString();
                                ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Decorrenza;
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[9], Page.Theme, "btnDeleteRetributivi");
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            ((DropDownList)e.Row.FindControl("ddlQuota")).SelectedValue = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCTabDatiCalcoloENPALSRetr", Page.Theme);
                        }
                        else if (IsNull((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)) &&
                            e.Row.DataItemIndex == ((List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()]).Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = "<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblQuota")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Quota.ToString();
                            ((Label)e.Row.FindControl("lblNSettimane")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Periodi.ToString();
                            ((Label)e.Row.FindControl("lblNTotaleContributiCalcolo")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).NTotaleContributiCalcolo.ToString();
                            ((Label)e.Row.FindControl("lblRMS")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).RM.ToString();
                            ((Label)e.Row.FindControl("lblImporto")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo.ToString();
                            ((Label)e.Row.FindControl("lblNSettimane707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Giorni707.ToString();
                            ((Label)e.Row.FindControl("lblImporto707")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Importo707.ToString();
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = ((GestioneContribDatiRetributiviENPALS)(e.Row.DataItem)).Decorrenza;

                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[9], Page.Theme, "btnDeleteRetributivi");
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
                throw new INPS.DNA.DnaApplicationException("UCDatiRetributiviENPALS, Errore nel metodo gvDatiRetributivi_RowDataBound " + ex);
            }
        }

        protected void gvDatiRetributivi_DataBound(object sender, EventArgs e)
        {
            if (!(ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()] != null && (((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.RetributivoComma707 ||
                ((AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributiviAgo.ToString()]).DatiCalcolo.TipoCalcolo == GestioneContribTipoCalcolo.MistoL214)))
            {
                gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Giorni707].Visible = false;
                gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Importo707].Visible = false;
            }

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
            if (!CodeUtility.IsEnpalsManualePL(true, CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura), datiPensione.IsDatiENPALSRecuperati))
                gvDatiRetributivi.Columns[(int)ColonneGvDatiRetributivi.Elimina].Visible = false;
        }

        protected void gvDatiRetributivi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<GestioneContribDatiRetributiviENPALS> listaDatiRetribApp = new List<GestioneContribDatiRetributiviENPALS>();
                foreach (GridViewRow rApp in ((GridView)sender).Rows)
                {
                    if (!IsEmptyReadableRowRetrib(rApp))
                    {
                        if (rApp.DataItemIndex != r.DataItemIndex)
                        {
                            listaDatiRetribApp.Add(new GestioneContribDatiRetributiviENPALS
                            {
                                Quota = CodeUtility.StringToNullableChar(((Label)rApp.FindControl("lblQuota")).Text),
                                Periodi = CodeUtility.StringToNullableShort(((Label)rApp.FindControl("lblNSettimane")).Text),
                                NTotaleContributiCalcolo = CodeUtility.StringToNullableInt(((Label)rApp.FindControl("lblNTotaleContributiCalcolo")).Text),
                                RM = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblRMS")).Text),
                                Importo = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblImporto")).Text),
                                Giorni707 = CodeUtility.StringToNullableShort(((Label)rApp.FindControl("lblNSettimane707")).Text),
                                Importo707 = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblImporto707")).Text)
                            });
                        }
                    }
                    else if (!IsEmptyEditableRowRetrib(rApp))
                    {
                        List<GestioneContribDatiRetributiviENPALS> datiRetributivi = (List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                        if (datiRetributivi != null && datiRetributivi.Count - 1 > rApp.DataItemIndex)
                        {
                            if (rApp.DataItemIndex != r.DataItemIndex)
                            {
                                listaDatiRetribApp.Add(new GestioneContribDatiRetributiviENPALS
                                {
                                    Quota = datiRetributivi[rApp.DataItemIndex].Quota,
                                    Periodi = datiRetributivi[rApp.DataItemIndex].Periodi,
                                    NTotaleContributiCalcolo = datiRetributivi[rApp.DataItemIndex].NTotaleContributiCalcolo,
                                    RM = datiRetributivi[rApp.DataItemIndex].RM,
                                    Importo = datiRetributivi[rApp.DataItemIndex].Importo,
                                    Giorni707 = datiRetributivi[rApp.DataItemIndex].Giorni707,
                                    Importo707 = datiRetributivi[rApp.DataItemIndex].Importo707
                                });
                            }
                        }
                    }
                }

                listaDatiRetribApp.Add(new GestioneContribDatiRetributiviENPALS());

                ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetribApp;
                gvDatiRetributivi.EditIndex = -1;
                gvDatiRetributivi.DataSource = listaDatiRetribApp;
                gvDatiRetributivi.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowRetrib((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<GestioneContribDatiRetributiviENPALS> listaDatiRetrApp = new List<GestioneContribDatiRetributiviENPALS>();
                    foreach (GridViewRow rApp in ((GridView)sender).Rows)
                    {
                        if (!IsEmptyEditableRowRetrib(rApp))
                        {
                            listaDatiRetrApp.Add(new GestioneContribDatiRetributiviENPALS
                            {
                                Quota = CodeUtility.StringToNullableChar(((DropDownList)rApp.FindControl("ddlQuota")).SelectedValue),
                                Periodi = CodeUtility.StringToNullableShort(((TextBox)rApp.FindControl("txtNSettimane")).Text),
                                NTotaleContributiCalcolo = CodeUtility.StringToNullableInt(((TextBox)rApp.FindControl("txtNTotaleContributiCalcolo")).Text),
                                RM = CodeUtility.StringToNullableDecimal(((TextBox)rApp.FindControl("txtRMS")).Text),
                                Importo = CodeUtility.StringToNullableDecimal(((TextBox)rApp.FindControl("txtImporto")).Text),
                                Giorni707 = CodeUtility.StringToNullableShort(((TextBox)rApp.FindControl("txtNSettimane707")).Text),
                                Importo707 = CodeUtility.StringToNullableDecimal(((TextBox)rApp.FindControl("txtImporto707")).Text),
                                Decorrenza = ((Label)rApp.FindControl("lblDecorrenza")).Text
                            });
                        }
                        else if (!IsEmptyReadableRowRetrib(rApp))
                        {
                            listaDatiRetrApp.Add(new GestioneContribDatiRetributiviENPALS
                            {
                                Quota = CodeUtility.StringToNullableChar(((Label)rApp.FindControl("lblQuota")).Text),
                                Periodi = CodeUtility.StringToNullableShort(((Label)rApp.FindControl("lblNSettimane")).Text),
                                NTotaleContributiCalcolo = CodeUtility.StringToNullableInt(((Label)rApp.FindControl("lblNTotaleContributiCalcolo")).Text),
                                RM = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblRMS")).Text),
                                Importo = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblImporto")).Text),
                                Giorni707 = CodeUtility.StringToNullableShort(((Label)rApp.FindControl("lblNSettimane707")).Text),
                                Importo707 = CodeUtility.StringToNullableDecimal(((Label)rApp.FindControl("lblImporto707")).Text),
                                Decorrenza = ((Label)rApp.FindControl("lblDecorrenza")).Text
                            });
                        }
                    }
                    if (listaDatiRetrApp.Count < 2)
                        listaDatiRetrApp.Add(new GestioneContribDatiRetributiviENPALS());
                    gvDatiRetributivi.EditIndex = -1;
                    ViewState[EnumViewState.ElencoDatiRetributivi.ToString()] = listaDatiRetrApp;
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<GestioneContribDatiRetributiviENPALS> listaDatiRetrApp = (List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                if (!IsListaEmpty('R'))
                {
                    gvDatiRetributivi.EditIndex = -1;
                    gvDatiRetributivi.DataSource = listaDatiRetrApp;
                    gvDatiRetributivi.DataBind();
                }
            }
        }

        protected void gvDatiRetributivi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiRetributivi.EditIndex = e.NewEditIndex;
                List<GestioneContribDatiRetributiviENPALS> listaDatiRetrApp = (List<GestioneContribDatiRetributiviENPALS>)ViewState[EnumViewState.ElencoDatiRetributivi.ToString()];
                gvDatiRetributivi.DataSource = listaDatiRetrApp;
                gvDatiRetributivi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiRetributiviENPALS, Errore nel metodo gvDatiRetributivi_RowEditing " + ex);
            }
        }
        #endregion gvRetributivi

        #region Events
        public event EventHandler ShowAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }
        #endregion Events

        #region Enum

        public enum ColonneGvDatiRetributivi
        {
            Giorni707 = 5,
            Importo707 = 6,
            Elimina = 7
        };

        public enum ColonneGvDatiContributivi
        {
            Elimina = 5
        }

        public enum EnumViewState
        {
            ElencoDatiRetributivi,
            ElencoDatiContributivi,
            AreaDatiContributiviAgo
        }

        #endregion Enum
    }
}
