using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiFondo_PI : CustomBaseUserControl, IDatiContributivi, ITitolarePensione, IDatiPensioneFondoPI
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDatiPensioneFondoPI
        public long IdFondo { get; set; }
        public long? IdRecordFondo { get; set; }
        public string NumDomanda { get; set; }
        public short? ControCodiceRetribuzione { get; set; }
        public AreaDatiPensioneFondoPI areaDatiPensioneFondoPI { get; set; }
        #endregion

        [Serializable]
        private sealed class RigaElencoFondoPI
        {
            public long IdFondo { get; set; }
            public long IdRecordFondo { get; set; }
            public string DecorrenzaFondo { get; set; }
            public byte? Semaforo { get; set; }
        }

        #region Page Lifecycle
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = (Presenter.SvrLiquidazioneFs.AreaDatiContributivi)Session["AreaDatiContributivi"];

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                ManagerViewState();
                ShowElenco();
                GvLoad();
            }
        }

        public void ValorizzaEtichette()
        {
            LoadDdls();
        }

        private void ManagerViewState()
        {
            // reset hidden
            if (hfIdFondo != null) hfIdFondo.Value = string.Empty;
            if (hfIdRecordFondo != null) hfIdRecordFondo.Value = string.Empty;
        }

        private void LoadDdl()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

        }
        #endregion

        #region UI helpers
        private void ShowElenco()
        {
            pnlElencoDatiFondo.Visible = true;
            pnlDettaglioDatiFondo.Visible = false;
        }

        private void ShowDettaglio()
        {
            pnlElencoDatiFondo.Visible = false;
            pnlDettaglioDatiFondo.Visible = true;
        }
        #endregion

        #region GridView
        private void GvLoad()
        {
            gvElencoDatiFondo.DataSource = BuildElencoRowsFromArea();
            gvElencoDatiFondo.DataBind();
        }

        private List<RigaElencoFondoPI> BuildElencoRowsFromArea()
        {
            List<RigaElencoFondoPI> result = new List<RigaElencoFondoPI>();

            if (areaDatiContributivi == null ||
                areaDatiContributivi.ElencoDatiPensioneFondoPI == null)
                return result;

            foreach (var kv in areaDatiContributivi.ElencoDatiPensioneFondoPI)
            {
                result.Add(new RigaElencoFondoPI
                {
                    IdFondo = kv.IdFondo,
                    IdRecordFondo = kv.IdRecordFondo,
                    DecorrenzaFondo = kv.DecorrenzaFondo.HasValue ? kv.DecorrenzaFondo.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Semaforo = kv.SemaforoRecord
                });
            }
            return result;
        }
        #endregion

        #region GridView Eventi
        protected void gvElenco_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            RigaElencoFondoPI riga = (RigaElencoFondoPI)e.Row.DataItem;
            Image img = (Image)e.Row.FindControl("img");
            if (img == null) return;

            if (riga.Semaforo == 2)
            {
                img.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/verde_tab.png";
                img.ToolTip = "Salvato";
            }
            else if (riga.Semaforo == 1)
            {
                img.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/arancione_tab.png";
                img.ToolTip = "Non Salvato";
            }
            else if (riga.Semaforo == 0)
            {
                img.ImageUrl = "~/App_Themes/" + Page.Theme + "/Images/rosso_tab.png";
                img.ToolTip = "Non Salvato";
            }
            else
            {
                img.ImageUrl = "";
                img.ToolTip = "";
            }
        }

        protected void gvElenco_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Modifica") return;

            RaiseHidePopUp(this, EventArgs.Empty);

            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

            HiddenField hdnIdFondo = (HiddenField)row.FindControl("hdnIdFondo");
            if (hdnIdFondo == null || string.IsNullOrEmpty(hdnIdFondo.Value)) return;

            HiddenField hdnIdRecordFondo = (HiddenField)row.FindControl("hdnIdRecordFondo");
            if (hdnIdRecordFondo == null || string.IsNullOrEmpty(hdnIdRecordFondo.Value)) return;

            long idFondo;
            if (!long.TryParse(hdnIdFondo.Value, out idFondo)) return;

            long idRecordFondo;
            if (!long.TryParse(hdnIdRecordFondo.Value, out idRecordFondo)) return;

            this.IdFondo = idFondo;
            if (hfIdFondo != null) hfIdFondo.Value = IdFondo.ToString();

            this.IdRecordFondo = idRecordFondo;
            if (hfIdRecordFondo != null) hfIdRecordFondo.Value = IdRecordFondo.ToString();

            this.NumDomanda = (this.domanda != null) ? this.domanda.NumeroDomanda : null;

            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.GetDatiPensioneFondoPi(this);

            if (this.areaDatiPensioneFondoPI != null)
            {
                if (this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi != null)
                    Session["AreaDatiPensioneFondoPIDettaglio"] = this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi;
                if (this.areaDatiPensioneFondoPI.DatiRecordFondo != null)
                    Session["AreaDatiPensioneRecordFondo"] = this.areaDatiPensioneFondoPI.DatiRecordFondo;
            }

            ViewState[EnumViewState.IsInsertFondoPI.ToString()] = false;

            ValorizzaDettaglio();

            txtDecorrenzaFondo.Enabled = false;
            ddlCodiceNonCalcolo.Enabled = false;
            ShowDettaglio();
        }
        #endregion

        #region Dettaglio
        private void ValorizzaDettaglio()
        {
            if (this.areaDatiPensioneFondoPI == null)
                return;
            ViewState[EnumViewState.CategoriaFondoPI.ToString()] = this.areaDatiContributivi.CategoriaFondoPI;

            GestioneFondoDatiFondoPI dettaglio = this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi;
            GestioneRecordFondoDatiRecordFondo recordFondo = this.areaDatiPensioneFondoPI.DatiRecordFondo;
            RenderControls();
            LoadDdls();

            if (dettaglio != null)
            {
                chkCodNonVedente.Checked = dettaglio.NonVedente.HasValue ? dettaglio.NonVedente.Value : false;
                txtQualifica.Text = dettaglio.Qualifica;
                txtImportoIIS.Text = dettaglio.ImportoIIS.HasValue ? dettaglio.ImportoIIS.Value.ToString() : string.Empty;
                txtImportoPensione.Text = dettaglio.StipendioAnnuo.HasValue ? dettaglio.StipendioAnnuo.Value.ToString() : string.Empty;
                txtRiscattiAA.Text = dettaglio.RiscattiAA.HasValue ? dettaglio.RiscattiAA.Value.ToString() : string.Empty;
                txtRiscattiMM.Text = dettaglio.RiscattiMM.HasValue ? dettaglio.RiscattiMM.Value.ToString() : string.Empty;
                txtRiscattiGG.Text = dettaglio.RiscattiGG.HasValue ? dettaglio.RiscattiGG.Value.ToString() : string.Empty;
                txtPensioneFacoltativaMensile.Text = dettaglio.PensioneFacoltativaMensile.HasValue ? dettaglio.PensioneFacoltativaMensile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtControCodiceRetribuzione.Text = string.Empty;
                txtStipendioBase.Text = dettaglio.StipendioBase.HasValue ? dettaglio.StipendioBase.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtIncrementoDPR346.Text = dettaglio.IncrementoDPR346.HasValue ? dettaglio.IncrementoDPR346.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtScatti.Text = dettaglio.Scatti;
                txtNumeroMatricola.Text = dettaglio.NumeroMatricola;
                if (dettaglio.AttCon.HasValue)
                    ddlAttCon.SelectedValue = dettaglio.AttCon.Value.ToString();

                ViewState[EnumViewState.TipoCalcolo.ToString()] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;

                if (this.areaDatiContributivi.CategoriaFondoPI.HasValue)
                {
                    switch (this.areaDatiContributivi.CategoriaFondoPI.Value)
                    {
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.U:
                            ValorizzaEtichetteCatU(dettaglio);
                            ValorizzaEtichetteHidden(dettaglio);
                            break;
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.V:
                            ValorizzaEtichetteCatV(dettaglio);
                            break;
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.A:
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.B:
                            ValorizzaEtichetteCatA(dettaglio);
                            ValorizzaEtichetteHidden(dettaglio);
                            break;
                        default:
                            ValorizzaEtichetteHidden(dettaglio);
                            break;
                    }
                }
            }

            if (recordFondo != null)
            {
                txtDecorrenzaFondo.Text = recordFondo.DecorrenzaValiditaDati.HasValue ?
                    string.Format("{0:dd/MM/yyyy}", recordFondo.DecorrenzaValiditaDati.Value) : string.Empty;
                ddlCodiceNonCalcolo.SelectedValue = recordFondo.CodiceNonCalcolo.HasValue ?
                    recordFondo.CodiceNonCalcolo.Value.ToString() : " ";
            }
        }

        private void RenderControls()
        {
            if (this.areaDatiContributivi != null && this.areaDatiContributivi.CategoriaFondoPI.HasValue)
            {
                switch (this.areaDatiContributivi.CategoriaFondoPI.Value)
                {
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.U:
                        pnlCatU.Visible = true;
                        break;
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.V:
                        pnlCatV.Visible = true;
                        trLblPensioneFacoltativaMensile.Visible = true;
                        trTxtPensioneFacoltativaMensile.Visible = true;
                        break;
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.A:
                    case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.B:
                        if (this.areaDatiContributivi.IsPIAPIBAnte99.GetValueOrDefault())
                            pnlCatAB.Visible = true;
                        break;
                }
                //switch (this.areaDatiContributivi.CategoriaFondoPI.Value)
                //{
                //    case UtilityCategoriaFondoPI.A:
                //    case UtilityCategoriaFondoPI.Uno:
                //    case UtilityCategoriaFondoPI.Y:
                //        pnlControcodiceRetr.Visible = true;
                //        break;
                //}
            }
        }

        private void LoadDdls()
        {
            ddlAttCon.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlAttCon);

            if (this.areaDatiContributivi != null && this.areaDatiContributivi.ListaAttCon != null)
            {
                foreach (AttCon attCon in this.areaDatiContributivi.ListaAttCon)
                {
                    CodeUtility.SetValueDdl(
                        ddlAttCon,
                        string.Format("{0} - {1}", attCon.Id.ToString(), attCon.Descrizione),
                        attCon.Descrizione,
                        attCon.Id.ToString());
                }
            }
        }

        #region Cat U
        private void ValorizzaEtichetteCatU(GestioneFondoDatiFondoPI dettaglio)
        {
            //spostato visibile per tutti
            //if (dettaglio.AttCon.HasValue)
            //    ddlAttCon.SelectedValue = dettaglio.AttCon.Value.ToString();

            if (dettaglio.PercentualeCapitalizzazione.HasValue)
                txtPercentualeCapitalizzazione.Text = dettaglio.PercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

            if (dettaglio.CodiceMaggiorazione.HasValue)
                ddlCodiceMaggiorazione.SelectedValue = dettaglio.CodiceMaggiorazione.Value.ToString();

            if (dettaglio.PensComplRiv1_95.HasValue)
                txtPensComplRiv1_95.Text = dettaglio.PensComplRiv1_95.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

            lblNumeroMatricola.Text = "Onorari Legali:";
        }
        private void RecuperaCampiU(GestioneFondoDatiFondoPI dettaglio)
        {
            //if (!string.IsNullOrEmpty(ddlAttCon.SelectedValue))
            //    dettaglio.AttCon = CodeUtility.StringToNullableChar(ddlAttCon.SelectedValue);

            if (!string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text))
                dettaglio.PercentualeCapitalizzazione = CodeUtility.StringToNullableDecimal(txtPercentualeCapitalizzazione.Text);

            if (!string.IsNullOrEmpty(ddlCodiceMaggiorazione.SelectedValue))
                dettaglio.CodiceMaggiorazione = CodeUtility.StringToNullableChar(ddlCodiceMaggiorazione.SelectedValue);

            if (!string.IsNullOrEmpty(txtPensComplRiv1_95.Text))
                dettaglio.PensComplRiv1_95 = CodeUtility.StringToNullableDecimal(txtPensComplRiv1_95.Text);
        }
        #endregion Cat U

        #region Cat V
        private void ValorizzaEtichetteCatV(GestioneFondoDatiFondoPI dettaglio)
        {
            if (dettaglio.RMSQuotaA.HasValue)
                txtRMSQuotaAFondo.Text = dettaglio.RMSQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

            if (dettaglio.RMSQuotaB.HasValue)
                txtRMSQuotaBFondo.Text = dettaglio.RMSQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

            if (dettaglio.NSettimaneQuotaA.HasValue)
                txtNSettimaneQuotaA.Text = dettaglio.NSettimaneQuotaA.Value.ToString();

            if (dettaglio.NSettimaneQuotaB.HasValue)
                txtNSettimaneQuotaB.Text = dettaglio.NSettimaneQuotaB.Value.ToString();

            if (dettaglio.PensioneFacoltativaMensile.HasValue)
                txtPensioneFacoltativaMensile.Text = dettaglio.PensioneFacoltativaMensile.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
        }

        private void RecuperaCampiV(GestioneFondoDatiFondoPI dettaglio)
        {
            if (!string.IsNullOrEmpty(txtRMSQuotaAFondo.Text))
                dettaglio.RMSQuotaA = CodeUtility.StringToNullableDecimal(txtRMSQuotaAFondo.Text);

            if (!string.IsNullOrEmpty(txtRMSQuotaBFondo.Text))
                dettaglio.RMSQuotaB = CodeUtility.StringToNullableDecimal(txtRMSQuotaBFondo.Text);

            if (!string.IsNullOrEmpty(txtNSettimaneQuotaA.Text))
                dettaglio.NSettimaneQuotaA = CodeUtility.StringToNullableShort(txtNSettimaneQuotaA.Text);

            if (!string.IsNullOrEmpty(txtNSettimaneQuotaB.Text))
                dettaglio.NSettimaneQuotaB = CodeUtility.StringToNullableShort(txtNSettimaneQuotaB.Text);

            if (!string.IsNullOrEmpty(txtPensioneFacoltativaMensile.Text))
                dettaglio.PensioneFacoltativaMensile = CodeUtility.StringToNullableDecimal(txtPensioneFacoltativaMensile.Text);
        }
        #endregion Cat V

        #region Cat A
        private void ValorizzaEtichetteCatA(GestioneFondoDatiFondoPI dettaglio)
        {
            if (dettaglio.PercentualeCapitalizzazione.HasValue)
                txtPercentualeCapitalizzazione.Text = dettaglio.PercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
        }

        private void RecuperaCampiA(GestioneFondoDatiFondoPI dettaglio)
        {
            if (!string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text))
                dettaglio.PercentualeCapitalizzazione = CodeUtility.StringToNullableDecimal(txtPercentualeCapitalizzazione.Text);
        }
        #endregion

        #region altre Cat
        private void ValorizzaEtichetteHidden(GestioneFondoDatiFondoPI dettaglio)
        {
            if (this.areaDatiContributivi != null &&
                this.areaDatiContributivi.DatiCalcolo != null &&
                this.areaDatiContributivi.DatiCalcolo.fondoPI != null)
            {
                //visibile per tutti
                //if (this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.HasValue && hdnAttCon != null && string.IsNullOrEmpty(hdnAttCon.Value))
                //    hdnAttCon.Value = this.areaDatiContributivi.DatiCalcolo.fondoPI.AttCon.Value.ToString();

                if (dettaglio.PercentualeCapitalizzazione.HasValue && string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text))
                    txtPercentualeCapitalizzazione.Text = dettaglio.PercentualeCapitalizzazione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        private void RecuperaCampiHidden(GestioneFondoDatiFondoPI dettaglio)
        {
            //spostato visibile per tutti 
            //if (hdnAttCon != null && !string.IsNullOrEmpty(hdnAttCon.Value) && !dettaglio.AttCon.HasValue)
            //    dettaglio.AttCon = Convert.ToChar(hdnAttCon.Value);

            if (!string.IsNullOrEmpty(txtPercentualeCapitalizzazione.Text) && !dettaglio.PercentualeCapitalizzazione.HasValue)
                dettaglio.PercentualeCapitalizzazione = CodeUtility.StringToNullableDecimal(txtPercentualeCapitalizzazione.Text);
        }
        #endregion
        #endregion

        #region Pulsanti dettaglio

        protected void btnAggiungiFondo_Click(object sender, EventArgs e)
        {
            RaiseHidePopUp(this, EventArgs.Empty);

            this.IdRecordFondo = 0;

            if (this.areaDatiContributivi != null)
            {
                if (this.areaDatiContributivi.IdFondo != null)
                    this.IdFondo = (long)this.areaDatiContributivi.IdFondo;

                if (hfIdFondo != null)
                    hfIdFondo.Value = this.IdFondo.ToString();
            }

            if (this.areaDatiPensioneFondoPI == null)
                this.areaDatiPensioneFondoPI = new AreaDatiPensioneFondoPI();

            if (this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi == null)
                this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi =
                    new GestioneFondoDatiFondoPI();

            if (this.areaDatiPensioneFondoPI.DatiRecordFondo == null)
                this.areaDatiPensioneFondoPI.DatiRecordFondo =
                    new GestioneRecordFondoDatiRecordFondo();

            Session["AreaDatiPensioneFondoPIDettaglio"] =
                this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi;

            Session["AreaDatiPensioneRecordFondo"] =
                this.areaDatiPensioneFondoPI.DatiRecordFondo;

            ViewState[EnumViewState.IsInsertFondoPI.ToString()] = true;

            ResetCampiDettaglio();

            ShowDettaglio();
        }

        protected void btnSalvaDettaglioFondo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda =
                    (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)
                    Session["Domanda"];

            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi =
                    (AreaDatiContributivi)Session["AreaDatiContributivi"];

            BuildAreaFromUIFondo();

            if (!String.IsNullOrEmpty(txtControCodiceRetribuzione.Text))
                this.ControCodiceRetribuzione = short.Parse(txtControCodiceRetribuzione.Text);

            bool isInsert = true;

            if (ViewState[EnumViewState.IsInsertFondoPI.ToString()] != null)
                isInsert = (bool)ViewState[EnumViewState.IsInsertFondoPI.ToString()];

            long idFondo = 0;

            if (hfIdFondo != null && !string.IsNullOrEmpty(hfIdFondo.Value))
                long.TryParse(hfIdFondo.Value, out idFondo);

            if (!isInsert && idFondo == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Sessione scaduta o dati non validi. Riaprire il dettaglio.";

                Utility.CustomEventArgs ev =
                    new Utility.CustomEventArgs(null,
                    (this.domanda != null && this.domanda.Tipofondo.HasValue)
                    ? this.domanda.Tipofondo.Value : 0);

                RaiseShowAvviso(this, ev);
                return;
            }

            this.IdFondo = idFondo;

            long idRecordFondo = 0;

            if (hfIdRecordFondo != null && !string.IsNullOrEmpty(hfIdRecordFondo.Value))
                long.TryParse(hfIdRecordFondo.Value, out idRecordFondo);

            this.IdRecordFondo = idRecordFondo;

            this.NumDomanda = (this.domanda != null) ? this.domanda.NumeroDomanda : null;

            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.StoreDatiPensioneFondoPi(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, new Utility.CustomEventArgs(null,this.domanda != null ? this.domanda.Tipofondo : 0));
                return;
            }

            presenter.GetDatiContributivi(this);
            // presenter.GetDatiPensioneFondoPi(this);

            //if (this.areaDatiPensioneFondoPI != null)
            //{
            //    if (this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi != null)
            //        Session["AreaDatiPensioneFondoPIDettaglio"] = this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi;
            //    if (this.areaDatiPensioneFondoPI.DatiRecordFondo != null)
            //        Session["AreaDatiPensioneRecordFondo"] = this.areaDatiPensioneFondoPI.DatiRecordFondo;
            //}

            //ValorizzaDettaglio();
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, (this.domanda != null && this.domanda.Tipofondo.HasValue) ? this.domanda.Tipofondo.Value : 0);
            RaiseShowAvviso(this, Cevent);
            ShowDettaglio();
        }

        protected void btnEliminaDettaglio_Click(object sender, EventArgs e)
        {
            long idRecordFondo = 0;

            if (hfIdRecordFondo != null && !string.IsNullOrEmpty(hfIdRecordFondo.Value))
                long.TryParse(hfIdRecordFondo.Value, out idRecordFondo);

            this.IdRecordFondo = idRecordFondo;

            if (idRecordFondo == 0)
            {
                this.HasError = true;
                this.ErrorMessage = "Sessione scaduta o dati non validi. Riaprire il dettaglio.";

                Utility.CustomEventArgs ev =
                    new Utility.CustomEventArgs(null,
                    (this.domanda != null && this.domanda.Tipofondo.HasValue)
                    ? this.domanda.Tipofondo.Value : 0);

                RaiseShowAvviso(this, ev);
                return;
            }

            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.EliminaDatiPensioneFondoPIByIdRecordFondo(this);

            btnTornaElencoFondo_Click(sender, e);
        }

        protected void btnTornaElencoFondo_Click(object sender, EventArgs e)
        {
            RaiseHidePopUp(this, EventArgs.Empty);
            RaiseHideAvviso(this, EventArgs.Empty);

            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.GetDatiContributivi(this);
            Session["AreaDatiContributivi"] = this.areaDatiContributivi;
            GvLoad();
            ShowElenco();
        }

        private void BuildAreaFromUIFondo()
        {
            GestioneFondoDatiFondoPI dettaglio =
                Session["AreaDatiPensioneFondoPIDettaglio"]
                as GestioneFondoDatiFondoPI;

            GestioneRecordFondoDatiRecordFondo recordFondo = Session["AreaDatiPensioneRecordFondo"]
                as GestioneRecordFondoDatiRecordFondo;

            if (dettaglio == null)
            {
                this.HasError = true;
                this.ErrorMessage = "Aggiornare la pagina.";
                Utility.CustomEventArgs cevent = new Utility.CustomEventArgs(null, this.domanda != null && this.domanda.Tipofondo.HasValue ? this.domanda.Tipofondo.Value : 0);
                RaiseShowAvviso(this, cevent);
                return;
            }

            if (this.areaDatiPensioneFondoPI == null)
                this.areaDatiPensioneFondoPI = new AreaDatiPensioneFondoPI();

            if (this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi == null)
                this.areaDatiPensioneFondoPI.DettaglioDatiPensioneFondoPi = dettaglio;

            if (this.areaDatiPensioneFondoPI.DatiRecordFondo == null)
                this.areaDatiPensioneFondoPI.DatiRecordFondo = recordFondo;

            recordFondo.DecorrenzaValiditaDati = Utility.GetDateFromString(txtDecorrenzaFondo.Text);
            recordFondo.CodiceNonCalcolo = ddlCodiceNonCalcolo.SelectedValue[0];

            dettaglio.NonVedente = chkCodNonVedente.Checked;
            dettaglio.Qualifica = txtQualifica.Text;
            dettaglio.ImportoIIS = ParseNullableDecimalIt(txtImportoIIS.Text);
            dettaglio.StipendioAnnuo = ParseNullableDecimalIt(txtImportoPensione.Text);

            if (!String.IsNullOrEmpty(txtRiscattiAA.Text))
                dettaglio.RiscattiAA = short.Parse(txtRiscattiAA.Text);
            if (!String.IsNullOrEmpty(txtRiscattiMM.Text))
                dettaglio.RiscattiMM = short.Parse(txtRiscattiMM.Text);
            if (!String.IsNullOrEmpty(txtRiscattiGG.Text))
                dettaglio.RiscattiGG = short.Parse(txtRiscattiGG.Text);

            if (!String.IsNullOrEmpty(txtPensioneFacoltativaMensile.Text))
                dettaglio.PensioneFacoltativaMensile =
                ParseNullableDecimalIt(txtPensioneFacoltativaMensile.Text);

            dettaglio.StipendioBase = !string.IsNullOrEmpty(txtStipendioBase.Text) ? CodeUtility.StringToNullableDecimal(txtStipendioBase.Text) : dettaglio.StipendioBase;

            if (ViewState[EnumViewState.TipoCalcolo.ToString()] != null && this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcolo != null)
                this.areaDatiContributivi.DatiCalcolo.TipoCalcolo =
                    (GestioneContribTipoCalcolo)ViewState[EnumViewState.TipoCalcolo.ToString()];

            dettaglio.IncrementoDPR346 = ParseNullableDecimalIt(txtIncrementoDPR346.Text);
            dettaglio.Scatti = txtScatti.Text;
            dettaglio.NumeroMatricola = txtNumeroMatricola.Text;

            if (!string.IsNullOrEmpty(ddlAttCon.SelectedValue))
                dettaglio.AttCon = CodeUtility.StringToNullableChar(ddlAttCon.SelectedValue);

            if (ViewState[EnumViewState.CategoriaFondoPI.ToString()] != null)
            {
                UtilityCategoriaFondoPI? cat = (UtilityCategoriaFondoPI?)ViewState[EnumViewState.CategoriaFondoPI.ToString()];
                if (cat.HasValue)
                {
                    switch (cat.Value)
                    {
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.U:
                            RecuperaCampiU(dettaglio);
                            RecuperaCampiHidden(dettaglio);
                            break;
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.V:
                            RecuperaCampiV(dettaglio);
                            break;
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.A:
                        case Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI.B:
                            RecuperaCampiA(dettaglio);
                            RecuperaCampiHidden(dettaglio);
                            break;
                        default:
                            RecuperaCampiHidden(dettaglio);
                            break;
                    }
                }
            }
        }

        private void ResetCampiDettaglio()
        {
            if (txtDecorrenzaFondo != null) txtDecorrenzaFondo.Text = string.Empty;
            if (chkCodNonVedente != null) chkCodNonVedente.Checked = false;
            if (txtQualifica != null) txtQualifica.Text = string.Empty;
            if (txtImportoIIS != null) txtImportoIIS.Text = string.Empty;
            if (txtImportoPensione != null) txtImportoPensione.Text = string.Empty;

            if (txtRiscattiAA != null) txtRiscattiAA.Text = string.Empty;
            if (txtRiscattiMM != null) txtRiscattiMM.Text = string.Empty;
            if (txtRiscattiGG != null) txtRiscattiGG.Text = string.Empty;

            if (txtPensioneFacoltativaMensile != null)
                txtPensioneFacoltativaMensile.Text = string.Empty;

            if (txtStipendioBase != null) txtStipendioBase.Text = string.Empty;

            if (txtControCodiceRetribuzione != null)
                txtControCodiceRetribuzione.Text = string.Empty;

            if (txtRMSQuotaAFondo != null) txtRMSQuotaAFondo.Text = string.Empty;
            if (txtRMSQuotaBFondo != null) txtRMSQuotaBFondo.Text = string.Empty;
            if (txtNSettimaneQuotaA != null) txtNSettimaneQuotaA.Text = string.Empty;
            if (txtNSettimaneQuotaB != null) txtNSettimaneQuotaB.Text = string.Empty;

            if (txtPercentualeCapitalizzazione != null)
                txtPercentualeCapitalizzazione.Text = string.Empty;

            if (txtPensComplRiv1_95 != null)
                txtPensComplRiv1_95.Text = string.Empty;

            if (ddlAttCon != null && ddlAttCon.Items != null && ddlAttCon.Items.Count > 0)
                ddlAttCon.ClearSelection();

            if (ddlCodiceMaggiorazione != null && ddlCodiceMaggiorazione.Items != null && ddlCodiceMaggiorazione.Items.Count > 0)
                ddlCodiceMaggiorazione.ClearSelection();

            if (txtIncrementoDPR346 != null) txtIncrementoDPR346.Text = string.Empty;
            if (txtNumeroMatricola != null) txtNumeroMatricola.Text = string.Empty;
            if (txtScatti != null) txtScatti.Text = string.Empty;
            ddlCodiceNonCalcolo.ClearSelection();

            txtDecorrenzaFondo.Enabled = true;
            ddlCodiceNonCalcolo.Enabled = true;
        }

        #endregion

        #region Event
        public event EventHandler CaricaDatiCalcolo;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event EventHandler HideAvviso;
        public event EventHandler ShowPopUp;
        public event EventHandler HidePopUp;

        protected void RaiseCaricaDatiCalcolo(object sender, EventArgs e)
        {
            EventHandler handler = CaricaDatiCalcolo;
            if (handler != null) handler(sender, e);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            Utility.CustomEventHandler handler = ShowAvviso;
            if (handler != null) handler(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            Utility.CustomEventHandler handler = ShowAvvisoElimina;
            if (handler != null) handler(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseShowPopUp(object sender, EventArgs e)
        {
            EventHandler handler = ShowPopUp;
            if (handler != null) handler(sender, e);
        }

        protected void RaiseHidePopUp(object sender, EventArgs e)
        {
            EventHandler handler = HidePopUp;
            if (handler != null) handler(sender, e);
        }
        #endregion Event

        #region Enum
        public enum EnumViewState
        {
            CategoriaFondoPI,
            TipoCalcolo,
            IsInsertFondoPI
        }
        #endregion Enum

        private static decimal? ParseNullableDecimalIt(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            decimal value;

            // prima provo it-IT (virgola)
            if (decimal.TryParse(input.Trim(),
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("it-IT"),
                out value))
                return value;

            if (decimal.TryParse(input.Trim(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value))
                return value;

            return null;
        }

        //public class DettaglioDatiPensioneFondoPI
        //{
        //    public bool CodNonVedente { get; set; }
        //    public string Qualifica { get; set; }
        //    public string ImportoIIS { get; set; }
        //    public string ImportoPensione { get; set; }
        //    public string Riscatti { get; set; }
        //    public string PensioneFacoltativa { get; set; }
        //    public string ControcodRetr { get; set; }
        //    public string DecorrenzaFondo { get; set; }
        //}
    }
}