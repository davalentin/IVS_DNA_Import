using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Collections.Generic;
// using System.Linq; // RIMOSSO per compatibilità e per evitare dipendenze LINQ
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiAGO_PI : CustomBaseUserControl, IDatiAgoFondoPI, IDatiContributivi, ITitolarePensione
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

        #region IDatiAgoFondoPI
        public long? IdDatiAgoFondoPI { get; set; }
        public AreaDatiAgoFondoPI areaDatiAgoFondoPI { get; set; }
        #endregion

        [Serializable]
        private sealed class RigaElencoAgoPI
        {
            public long Id { get; set; }
            public DateTime? DecorrenzaAgo { get; set; }
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
            LoadDdl();
        }

        private void ManagerViewState()
        {
            // reset hidden
            if (hfRowIndex != null) hfRowIndex.Value = string.Empty;
            if (hfIdDatiAgo != null) hfIdDatiAgo.Value = string.Empty;
        }

        private void LoadDdl()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda != null && this.domanda.Tipofondo.HasValue)
            {
                ddlTipoLiquidazione.Items.Clear();
                CodeUtility.SetItemBlankDdl(ddlTipoLiquidazione);

                if (this.areaDatiContributivi != null && this.areaDatiContributivi.ListaTipoLiquidazionePI != null)
                {
                    foreach (TipoLiquidazionePI tipoLiquidazionePI in this.areaDatiContributivi.ListaTipoLiquidazionePI)
                    {
                        CodeUtility.SetValueDdl(
                            ddlTipoLiquidazione,
                            tipoLiquidazionePI.Id.ToString(),
                            tipoLiquidazionePI.Descrizione,
                            tipoLiquidazionePI.Id.ToString());
                    }
                }
            }
        }
        #endregion

        #region UI helpers
        private void ShowElenco()
        {
            pnlElenco.Visible = true;
            pnlDettaglio.Visible = false;
        }

        private void ShowDettaglio()
        {
            pnlElenco.Visible = false;
            pnlDettaglio.Visible = true;
        }
        #endregion

        #region GridView
        private void GvLoad()
        {
            gvElenco.DataSource = BuildElencoRowsFromArea();
            gvElenco.DataBind();
        }

        private List<RigaElencoAgoPI> BuildElencoRowsFromArea()
        {
            List<RigaElencoAgoPI> result = new List<RigaElencoAgoPI>();

            if (this.areaDatiContributivi == null ||
                this.areaDatiContributivi.ElencoDatiAgo == null)
                return result;

            foreach (var kv in this.areaDatiContributivi.ElencoDatiAgo)
            {

                result.Add(new RigaElencoAgoPI
                {
                    Id = kv.Id,
                    DecorrenzaAgo = kv.DecorrenzaDatiAgo,
                    Semaforo = kv.SemaforoRecord
                });
            }

            result.Sort(delegate (RigaElencoAgoPI a, RigaElencoAgoPI b)
            {
                DateTime da = a.DecorrenzaAgo.HasValue ? a.DecorrenzaAgo.Value : DateTime.MinValue;
                DateTime db = b.DecorrenzaAgo.HasValue ? b.DecorrenzaAgo.Value : DateTime.MinValue;
                return DateTime.Compare(da, db);
            });

            return result;
        }
        #endregion

        #region GridView Eventi
        protected void gvElenco_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            RigaElencoAgoPI riga = (RigaElencoAgoPI)e.Row.DataItem;
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
                img.ImageUrl = ""; // placeholder
                img.ToolTip = "";
            }
        }

        protected void gvElenco_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Modifica") return;

            RaiseHidePopUp(this, EventArgs.Empty);

            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

            HiddenField hdn = (HiddenField)row.FindControl("hdnIdDatiAgoFondoPI");
            if (hdn == null || string.IsNullOrEmpty(hdn.Value)) return;

            long id;
            if (!long.TryParse(hdn.Value, out id)) return;

            this.IdDatiAgoFondoPI = id;
            if (hfIdDatiAgo != null) hfIdDatiAgo.Value = id.ToString();

            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.GetDatiAgoFondoPi(this);

            Session["AreaDatiAgoFondoPI"] = this.areaDatiAgoFondoPI;

            ValorizzaDettaglio();
            ShowDettaglio();
        }
        #endregion

        #region Dettaglio
        private void ValorizzaDettaglio()
        {
            if (this.areaDatiAgoFondoPI == null)
                return;

            Presenter.SvrLiquidazioneFs.GestioneFondoDatiAgoPI dettaglio = this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi;

            if (dettaglio != null)
            {
                txtDecorrenzaAgo.Text = dettaglio.DecorrenzaDatiAgo.HasValue
                    ? dettaglio.DecorrenzaDatiAgo.Value.ToString("MM/yyyy")
                    : string.Empty;

                ddlTipoLiquidazione.SelectedValue = dettaglio.TipoLiquidazione.HasValue
                    ? dettaglio.TipoLiquidazione.Value.ToString()
                    : string.Empty;

                txtCodiceSpecifico.Text = !string.IsNullOrEmpty(dettaglio.CodiceSpecificoAgo)
                    ? dettaglio.CodiceSpecificoAgo
                    : string.Empty;

                txtSospensione.Text = dettaglio.SospensioneAgo.HasValue
                    ? dettaglio.SospensioneAgo.Value.ToString("MM/yyyy")
                    : string.Empty;

                if (!string.IsNullOrEmpty(dettaglio.CodiceNatura))
                {
                    if (dettaglio.CodiceNatura.Length >= 1 && dettaglio.CodiceNatura[0] != ' ')
                        txtCodiceNatura1.Text = dettaglio.CodiceNatura[0].ToString();
                    else
                        txtCodiceNatura1.Text = string.Empty;

                    if (dettaglio.CodiceNatura.Length >= 2 && dettaglio.CodiceNatura[1] != ' ')
                        txtCodiceNatura2.Text = dettaglio.CodiceNatura[1].ToString();
                    else
                        txtCodiceNatura2.Text = string.Empty;

                    if (dettaglio.CodiceNatura.Length >= 3 && dettaglio.CodiceNatura[2] != ' ')
                        txtCodiceNatura3.Text = dettaglio.CodiceNatura[2].ToString();
                    else
                        txtCodiceNatura3.Text = string.Empty;
                }
                else
                {
                    txtCodiceNatura1.Text = string.Empty;
                    txtCodiceNatura2.Text = string.Empty;
                    txtCodiceNatura3.Text = string.Empty;
                }

                txtSettimaneVV.Text = dettaglio.SettimaneVV.HasValue
                    ? dettaglio.SettimaneVV.Value.ToString()
                    : string.Empty;

                txtCausaCarico.Text = !string.IsNullOrEmpty(dettaglio.CausaCarico)
                    ? dettaglio.CausaCarico
                    : string.Empty;

                txtCtres.Text = dettaglio.Ctres.HasValue
                    ? dettaglio.Ctres.Value.ToString()
                    : string.Empty;

                txtDirittoQuoteFisse.Text = dettaglio.DirittoQuoteFisse.HasValue
                   ? dettaglio.DirittoQuoteFisse.Value.ToString()
                   : string.Empty;

                txtSettExComb.Text = dettaglio.NSettimaneExCombattente.HasValue
                   ? dettaglio.NSettimaneExCombattente.Value.ToString()
                   : string.Empty;

                txtRMSRetr.Text = dettaglio.RMSRetributiva.HasValue
                    ? dettaglio.RMSRetributiva.Value.ToString()
                    : string.Empty;

                // Dati Retributivi - Quota A
                txtRMSQuotaA.Text = dettaglio.RMSQuotaA.HasValue
                    ? dettaglio.RMSQuotaA.Value.ToString()
                    : string.Empty;
                txtRMSOmogeneaQuotaA.Text = dettaglio.RMSQuotaAOmogenea.HasValue
                    ? dettaglio.RMSQuotaAOmogenea.Value.ToString()
                    : string.Empty;
                txtSettimaneTotQuotaA.Text = dettaglio.NSettimaneQuotaA.HasValue
                    ? dettaglio.NSettimaneQuotaA.Value.ToString()
                    : string.Empty;
                txtSettimaneEscQuotaA.Text = dettaglio.NSettimaneEsclusiveQuotaA.HasValue
                    ? dettaglio.NSettimaneEsclusiveQuotaA.Value.ToString()
                    : string.Empty;

                // Dati Retributivi - Quota B
                txtRMSQuotaB.Text = dettaglio.RMSQuotaB.HasValue
                    ? dettaglio.RMSQuotaB.Value.ToString()
                    : string.Empty;
                txtRMSOmogeneaQuotaB.Text = dettaglio.RMSQuotaBOmogenea.HasValue
                    ? dettaglio.RMSQuotaBOmogenea.Value.ToString()
                    : string.Empty;
                txtSettimaneTotQuotaB.Text = dettaglio.NSettimaneQuotaB.HasValue
                    ? dettaglio.NSettimaneQuotaB.Value.ToString()
                    : string.Empty;
                txtSettimaneEscQuotaB.Text = dettaglio.NSettimaneEsclusiveQuotaB.HasValue
                    ? dettaglio.NSettimaneEsclusiveQuotaB.Value.ToString()
                    : string.Empty;

                // Dati Contributivi
                txtMontanteTotale.Text = dettaglio.Montante.HasValue
                    ? dettaglio.Montante.Value.ToString()
                    : string.Empty;
            }
        }
        #endregion

        #region Pulsanti dettaglio

        protected void btnAggiungiDatiAgo_Click(object sender, EventArgs e)
        {
            RaiseHidePopUp(this, EventArgs.Empty);

            if (this.areaDatiAgoFondoPI == null)
                this.areaDatiAgoFondoPI = new AreaDatiAgoFondoPI();

            if (this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi == null)
                this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi =
                    new GestioneFondoDatiAgoPI();

            if (this.areaDatiContributivi != null)
            {
                if (this.areaDatiContributivi.IdFondo != null)
                    this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi.IdFondo = (long)this.areaDatiContributivi.IdFondo;
            }

            Session["AreaDatiAgoFondoPI"] = this.areaDatiAgoFondoPI;

            ResetCampiDettaglio();

            ShowDettaglio();
        }


        protected void btnTornaElenco_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RaiseHidePopUp(this, EventArgs.Empty);
            RaiseHideAvviso(this, EventArgs.Empty);

            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.GetDatiContributivi(this);
            Session["AreaDatiContributivi"] = this.areaDatiContributivi;
            GvLoad();
            ShowElenco();
        }

        protected void btnSalvaDettaglio_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)
                    Session["Domanda"];

            if (this.areaDatiContributivi == null || !this.areaDatiContributivi.IdFondo.HasValue)
            {
                this.HasError = true;
                this.ErrorMessage = "Id fondo non valorizzato.";

                AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tf = null;
                if (this.domanda != null && this.domanda.Tipofondo.HasValue)
                    tf = this.domanda.Tipofondo.Value;

                RaiseShowAvviso(this, new Utility.CustomEventArgs(null, tf));
                return;
            }

            long idSelezionato = 0;
            bool isNuovo = true;

            if (hfIdDatiAgo != null && !string.IsNullOrEmpty(hfIdDatiAgo.Value))
            {
                if (long.TryParse(hfIdDatiAgo.Value, out idSelezionato))
                    isNuovo = false;
            }

            if (Session["AreaDatiAgoFondoPI"] != null)
                this.areaDatiAgoFondoPI =
                    (Presenter.SvrLiquidazioneFs.AreaDatiAgoFondoPI)Session["AreaDatiAgoFondoPI"];

            BuildAreaFromUI();

            if (!isNuovo)
                this.IdDatiAgoFondoPI = idSelezionato;


            PresenterDatiContributivi presenter = new PresenterDatiContributivi();
            presenter.StoreDatiAgoFondoPi(this);

            if (this.HasError)
            {
                AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tf = null;
                if (this.domanda != null && this.domanda.Tipofondo.HasValue)
                    tf = this.domanda.Tipofondo.Value;

                RaiseShowAvviso(this, new Utility.CustomEventArgs(null, tf));
                return;
            }

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tfinal = null;
            if (this.domanda != null && this.domanda.Tipofondo.HasValue)
                tfinal = this.domanda.Tipofondo.Value;

            RaiseShowAvviso(this, new Utility.CustomEventArgs(null, tfinal));
            ShowDettaglio();
        }
        protected void btnEliminaDettaglioDatiAgo_Click(object sender, EventArgs e)
        {
            long idSelezionato = 0;

            if (long.TryParse(hfIdDatiAgo.Value, out idSelezionato))
                long.TryParse(hfIdDatiAgo.Value, out idSelezionato);

            this.IdDatiAgoFondoPI = idSelezionato;

            if (idSelezionato == 0)
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
            presenter.EliminaDatiAgoFondoPIById(this);

            btnTornaElenco_Click(sender, e);
        }

        private void BuildAreaFromUI()
        {
            if (this.areaDatiAgoFondoPI == null)
                this.areaDatiAgoFondoPI = new Presenter.SvrLiquidazioneFs.AreaDatiAgoFondoPI();

            if (this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi == null)
                this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi = new Presenter.SvrLiquidazioneFs.GestioneFondoDatiAgoPI();

            Presenter.SvrLiquidazioneFs.GestioneFondoDatiAgoPI dettaglio = this.areaDatiAgoFondoPI.DettaglioDatiAgoFondoPi;

            dettaglio.DecorrenzaDatiAgo =
                !string.IsNullOrEmpty(txtDecorrenzaAgo.Text)
                ? (DateTime?)DateTime.ParseExact(txtDecorrenzaAgo.Text.Trim(), "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                : null;

            dettaglio.TipoLiquidazione =
                !string.IsNullOrEmpty(ddlTipoLiquidazione.SelectedValue)
                ? (short?)short.Parse(ddlTipoLiquidazione.SelectedValue.Trim())
                : null;

            dettaglio.CodiceSpecificoAgo =
                !string.IsNullOrEmpty(txtCodiceSpecifico.Text)
                ? txtCodiceSpecifico.Text.Trim()
                : null;

            dettaglio.SospensioneAgo =
                !string.IsNullOrEmpty(txtSospensione.Text)
                ? (DateTime?)DateTime.ParseExact(txtSospensione.Text.Trim(), "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                : null;

            // Codice Natura (char(3))
            string c1 = !string.IsNullOrEmpty(txtCodiceNatura1.Text) ? txtCodiceNatura1.Text.Substring(0, 1) : " ";
            string c2 = !string.IsNullOrEmpty(txtCodiceNatura2.Text) ? txtCodiceNatura2.Text.Substring(0, 1) : " ";
            string c3 = !string.IsNullOrEmpty(txtCodiceNatura3.Text) ? txtCodiceNatura3.Text.Substring(0, 1) : " ";
            dettaglio.CodiceNatura = c1 + c2 + c3;

            dettaglio.SettimaneVV = !string.IsNullOrEmpty(txtSettimaneVV.Text) ? (short?)short.Parse(txtSettimaneVV.Text.Trim()) : null;

            dettaglio.CausaCarico =
                !string.IsNullOrEmpty(txtCausaCarico.Text)
                ? txtCausaCarico.Text.Trim()
                : null;

            dettaglio.DirittoQuoteFisse = !string.IsNullOrEmpty(txtDirittoQuoteFisse.Text) ? (short?)short.Parse(txtDirittoQuoteFisse.Text.Trim()) : null;

            dettaglio.Ctres =
               !string.IsNullOrEmpty(txtCtres.Text)
               ? (decimal?)decimal.Parse(txtCtres.Text.Trim())
               : null;

            dettaglio.NSettimaneExCombattente =
                !string.IsNullOrEmpty(txtSettExComb.Text)
                ? (int?)int.Parse(txtSettExComb.Text.Trim())
                : null;

            dettaglio.RMSRetributiva =
               !string.IsNullOrEmpty(txtRMSRetr.Text)
               ? (decimal?)decimal.Parse(txtRMSRetr.Text.Trim())
               : null;


            // --- DATI RETRIBUTIVI – QUOTA A --------------------------------
            dettaglio.RMSQuotaA =
                !string.IsNullOrEmpty(txtRMSQuotaA.Text)
                ? (decimal?)decimal.Parse(txtRMSQuotaA.Text.Trim())
                : null;

            dettaglio.RMSQuotaAOmogenea =
                !string.IsNullOrEmpty(txtRMSOmogeneaQuotaA.Text)
                ? (decimal?)decimal.Parse(txtRMSOmogeneaQuotaA.Text.Trim())
                : null;

            dettaglio.NSettimaneQuotaA =
                !string.IsNullOrEmpty(txtSettimaneTotQuotaA.Text)
                ? (int?)int.Parse(txtSettimaneTotQuotaA.Text.Trim())
                : null;

            dettaglio.NSettimaneEsclusiveQuotaA =
                !string.IsNullOrEmpty(txtSettimaneEscQuotaA.Text)
                ? (int?)int.Parse(txtSettimaneEscQuotaA.Text.Trim())
                : null;

            // --- DATI RETRIBUTIVI – QUOTA B --------------------------------
            dettaglio.RMSQuotaB =
                !string.IsNullOrEmpty(txtRMSQuotaB.Text)
                ? (decimal?)decimal.Parse(txtRMSQuotaB.Text.Trim())
                : null;

            dettaglio.RMSQuotaBOmogenea =
                !string.IsNullOrEmpty(txtRMSOmogeneaQuotaB.Text)
                ? (decimal?)decimal.Parse(txtRMSOmogeneaQuotaB.Text.Trim())
                : null;

            dettaglio.NSettimaneQuotaB =
                !string.IsNullOrEmpty(txtSettimaneTotQuotaB.Text)
                ? (int?)int.Parse(txtSettimaneTotQuotaB.Text.Trim())
                : null;

            dettaglio.NSettimaneEsclusiveQuotaB =
                !string.IsNullOrEmpty(txtSettimaneEscQuotaB.Text)
                ? (int?)int.Parse(txtSettimaneEscQuotaB.Text.Trim())
                : null;

            // --- DATI CONTRIBUTIVI ----------------------------------------
            dettaglio.Montante =
                !string.IsNullOrEmpty(txtMontanteTotale.Text)
                ? (decimal?)decimal.Parse(txtMontanteTotale.Text.Trim())
                : null;
            // (altri campi opzionali lasciati come nel sorgente originale)
        }

        private void ResetCampiDettaglio()
        {
            // --- hidden ---
            if (hfIdDatiAgo != null) hfIdDatiAgo.Value = string.Empty;

            // --- dati generali ---
            txtDecorrenzaAgo.Text = string.Empty;
            txtCodiceSpecifico.Text = string.Empty;
            txtSospensione.Text = string.Empty;

            txtCodiceNatura1.Text = string.Empty;
            txtCodiceNatura2.Text = string.Empty;
            txtCodiceNatura3.Text = string.Empty;

            txtSettimaneVV.Text = string.Empty;
            txtCausaCarico.Text = string.Empty;
            txtDirittoQuoteFisse.Text = string.Empty;
            txtCtres.Text = string.Empty;
            txtSettExComb.Text = string.Empty;
            txtRMSRetr.Text = string.Empty;

            if (ddlTipoLiquidazione != null)
                ddlTipoLiquidazione.SelectedIndex = 0;

            // --- QUOTA A ---
            txtRMSQuotaA.Text = string.Empty;
            txtRMSOmogeneaQuotaA.Text = string.Empty;
            txtSettimaneTotQuotaA.Text = string.Empty;
            txtSettimaneEscQuotaA.Text = string.Empty;

            // --- QUOTA B ---
            txtRMSQuotaB.Text = string.Empty;
            txtRMSOmogeneaQuotaB.Text = string.Empty;
            txtSettimaneTotQuotaB.Text = string.Empty;
            txtSettimaneEscQuotaB.Text = string.Empty;

            // --- contributivi ---
            txtMontanteTotale.Text = string.Empty;
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
        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        #endregion Event
    }
}
