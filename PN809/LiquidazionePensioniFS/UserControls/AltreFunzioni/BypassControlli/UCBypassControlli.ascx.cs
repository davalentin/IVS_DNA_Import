using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.BypassControlli
{
    public partial class UCBypassControlli : CustomBaseUserControl, IBypassControlli
    {
        #region IBypassControlli
        public AreaBypassControllo BypassControllo { get; set; }
        public AreaBypassControllo.BypassControllo datiBypassControllo { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        #endregion IBypassControlli

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (!IsPostBack)
            {
                int paginaDaVisualizzare = 1;
                ValorizzaGriglia(paginaDaVisualizzare);
                ValorizzaEtichette();

                InizializzaSwitchInsertPerPensione_o_numeroDomanda();
            }
        }


        private void InizializzaSwitchInsertPerPensione_o_numeroDomanda()
        {
            AbilitaDisabilitaInserimentoPerPensione_SuPannelloInsert(false);
            AbilitaDisabilitaInserimentoPerNumeroDomanda_SuPannelloInsert(false);
        }


        #region abilitazione e disabilitazione inserimento per pensione pannello di inserimento by pass
        private void AbilitaDisabilitaInserimentoPerPensione_SuPannelloInsert(bool isAbilitato)
        {
            txtInsertCodCategoria.Enabled = isAbilitato;
            txtInsertCodCategoria.CssClass = (isAbilitato ? txtInsertCodCategoria.CssClass.Replace("tboxdisable", "") : txtInsertCodCategoria.CssClass + " tboxdisable");

            txtInsertCodiceSede.Enabled = isAbilitato;
            txtInsertCodiceSede.CssClass = (isAbilitato ? txtInsertCodiceSede.CssClass.Replace("tboxdisable", "") : txtInsertCodiceSede.CssClass + " tboxdisable");

            txtInsertNCertificato.Enabled = isAbilitato;
            txtInsertNCertificato.CssClass = (isAbilitato ? txtInsertNCertificato.CssClass.Replace("tboxdisable", "") : txtInsertNCertificato.CssClass + " tboxdisable");

            revInsertCategoriaNonValida.Enabled = isAbilitato;
            rfvInsertCategoriaRichiesta.Enabled = isAbilitato;
            revInsertLunghezzaCategoria.Enabled = isAbilitato;

            revInsertCodiceSedeNonValido.Enabled = isAbilitato;
            rfvInsertCodiceSedeRichiesto.Enabled = isAbilitato;
            revInsertLunghezzaCodiceSede.Enabled = isAbilitato;

            revInsertNCertificatNonValido.Enabled = isAbilitato;
            refInsertNCertificatoRichiesto.Enabled = isAbilitato;
            revInsertLunghezzaNCertificato.Enabled = isAbilitato;
        }

        private void AbilitaDisabilitaInserimentoPerNumeroDomanda_SuPannelloInsert(bool isAbilitato)
        {
            txtInsertNumeroDomanda.Enabled = isAbilitato;
            txtInsertNumeroDomanda.CssClass = (isAbilitato ? txtInsertNumeroDomanda.CssClass.Replace("tboxdisable", "") : txtInsertNumeroDomanda.CssClass + " tboxdisable");
            revInsertLunghezzaNumeroDomanda.Enabled = isAbilitato;
            rfvInsertNumeroDomandaRichiesto.Enabled = isAbilitato;
        }
        #endregion

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            int paginaDaVisualizzare = 1;
            string errore = string.Empty;

            ControlsApplicaFiltro(out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                this.HasError = true;
                this.ErrorMessage = errore;
                RaiseShowAvviso(this, null);
                return;
            }

            ValorizzaGriglia(paginaDaVisualizzare);
            gvBypassControlli_Load(paginaDaVisualizzare);
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            AbilitaFiltro();
            int paginaDaVisualizzare = 1;
            ValorizzaGriglia(paginaDaVisualizzare);
            RaiseHideInfo(this, null);
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            string errore = string.Empty;

            RecuperaCampi(out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                this.HasError = true;
                this.ErrorMessage = errore;
                RaiseShowAvviso(this, null);
                return;
            }

            PresenterBypassControlli presenter = new PresenterBypassControlli();
            presenter.SalvaBypassControlli(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
            else
            {
                this.ErrorMessage = "Controllo bypassato correttamente";
                RaiseShowAvviso(this, null);
            }
            AbilitaFiltro();
            PulisciPannelloInsert();

            int paginaDaVisualizzare = int.MaxValue;//per visualizzare l'ultima pagina
            ValorizzaGriglia(paginaDaVisualizzare);

            ScriptManager.RegisterStartupScript(this, GetType(), "CleanFields", "CleanFields();", true);

            radioInsertDomanda.Checked = false;
            radioInsertPensione.Checked = false;
            InizializzaSwitchInsertPerPensione_o_numeroDomanda();
        }

        protected void ddlBypassInsert_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            List<AreaBypassControllo.DecBypassControllo> elencoDecBypassControllo = (List<AreaBypassControllo.DecBypassControllo>)ViewState["ElencoDecBypassControlli"];
            if (ddlBypassInsert.SelectedIndex == 0)
                lblDescrizioneBypass.Text = "Nessun Bypass selezionato";
            else
            {
                if (elencoDecBypassControllo != null && elencoDecBypassControllo.Count > 0)
                {
                    AreaBypassControllo.DecBypassControllo bypass = elencoDecBypassControllo.Find(x => x.Id.ToString() == ddlBypassInsert.SelectedValue);
                    if (bypass != null)
                        lblDescrizioneBypass.Text = bypass.Descrizione;
                }
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "SetScroll", "SetScroll();", true);
        }

        #region private members
        private void ValorizzaEtichette()
        {
            lblMatricolaInsert.Text = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;

            List<AreaBypassControllo.DecBypassControllo> elencoDecBypassControlli = (List<AreaBypassControllo.DecBypassControllo>)ViewState["ElencoDecBypassControlli"];

            if (elencoDecBypassControlli != null && elencoDecBypassControlli.Count > 0)
            {
                ddlFiltroBypass.Items.Clear();
                ddlBypassInsert.Items.Clear();
                CodeUtility.SetItemBlankDdl(ddlFiltroBypass);
                CodeUtility.SetItemBlankDdl(ddlBypassInsert);

                var bypassAmmessiDirettoreFS = new List<string> { "CONFERMA_ESENZIONE_VITTIME" };

                foreach (AreaBypassControllo.DecBypassControllo decBypass in elencoDecBypassControlli)
                {
                    Ruoli ruolo = (Ruoli)Session["Ruolo"];
                    if (ruolo == Ruoli.P8974 && !bypassAmmessiDirettoreFS.Contains(decBypass.Nome.Trim()))
                    {
                        continue;
                    }
                    CodeUtility.SetValueDdl(ddlFiltroBypass, decBypass.Nome, decBypass.Id.ToString());
                    CodeUtility.SetValueDdl(ddlBypassInsert, decBypass.Nome, decBypass.Id.ToString());
                }
            }
        }

        private void ValorizzaGriglia(int paginaDaVisualizzare)
        {
            Presenter.PresenterBypassControlli presenter = new Presenter.PresenterBypassControlli();
            presenter.CaricaBypassControlli(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            ValorizzaViewStateBypassControlli();
            Filtra();
            gvBypassControlli_Load(paginaDaVisualizzare);
        }

        private void ValorizzaViewStateBypassControlli()
        {
            if (this.BypassControllo != null)
            {
                ViewState["ElencoBypassControlli"] = this.BypassControllo.ListBypassControllo.ToList();
                ViewState["ElencoDecBypassControlli"] = this.BypassControllo.ListDecBypassControllo.ToList();
            }
        }

        private void gvBypassControlli_Load(int paginaDaVisualizzare)
        {
            try
            {
                gvBypassControlli.DataSource = (List<AreaBypassControllo.BypassControllo>)ViewState["ElencoBypassControlli"];
                gvBypassControlli.PageIndex = paginaDaVisualizzare < 1 ? 0 : paginaDaVisualizzare - 1;//paginaDaVisualizzare - 1 perchè la prima pagina ha PageIndex = 0 
                gvBypassControlli.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCBypassControlli, Errore nel metodo gvBypassControlli_Load " + ex);
            }
        }

        private void ControlsApplicaFiltro(out string errore)
        {
            errore = string.Empty;

            if (String.IsNullOrEmpty(txtFiltroNumeroDomanda.Text.Trim()) && String.IsNullOrEmpty(txtFiltroCodCategoria.Text.Trim()) && String.IsNullOrEmpty(txtFiltroCodiceSede.Text.Trim()) &&
                String.IsNullOrEmpty(txtFiltroNCertificato.Text.Trim()) && String.IsNullOrEmpty(txtFiltroMatricola.Text.Trim()) && ddlFiltroBypass.SelectedIndex == 0 &&
                ddlLock.SelectedIndex == 0)
            {
                errore = "Inserire almeno un filtro di ricerca";
                return;

            }

            //Filtro Chiave Pensione: per utilizzare questo filtro bisogna compilare tutti e tre i campi che lo compongono
            if (!String.IsNullOrEmpty(txtFiltroCodCategoria.Text.Trim()) || !String.IsNullOrEmpty(txtFiltroCodiceSede.Text.Trim()) ||
                !String.IsNullOrEmpty(txtFiltroNCertificato.Text.Trim()))
            {
                if (String.IsNullOrEmpty(txtFiltroCodCategoria.Text.Trim()))
                {
                    errore = "Filtro di ricerca: Inserire una categoria pensione";
                    return;
                }

                if (String.IsNullOrEmpty(txtFiltroCodiceSede.Text.Trim()))
                {
                    errore = "Filtro di ricerca: Inserire un codice sede pensione";
                    return;
                }

                if (String.IsNullOrEmpty(txtFiltroNCertificato.Text.Trim()))
                {
                    errore = "Filtro di ricerca: Inserire un certificato di pensione";
                    return;
                }
            }

            if (!string.IsNullOrEmpty(txtFiltroNumeroDomanda.Text.Trim()))
            {
                if (txtFiltroNumeroDomanda.Text.Length != 13)
                {
                    errore = "Il Numero Domanda deve essere di 13 caratteri";
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtFiltroCodCategoria.Text.Trim()))
            {
                if (txtFiltroCodCategoria.Text.Length != 3)
                {
                    errore = "La Categoria della chiave pensione deve essere di 3 caratteri";
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtFiltroCodiceSede.Text.Trim()))
            {
                if (txtFiltroCodiceSede.Text.Length != 4)
                {
                    errore = "Il Codice Sede della chiave pensione deve essere di 4 caratteri";
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtFiltroNCertificato.Text.Trim()))
            {
                if (txtFiltroNCertificato.Text.Length != 8)
                {
                    errore = "Il Certificato della chiave pensione deve essere di 8 caratteri";
                    return;
                }
            }
        }

        private void Filtra()
        {           
            int count = 0;
            List<AreaBypassControllo.BypassControllo> elencoBypassControlli = (List<AreaBypassControllo.BypassControllo>)ViewState["ElencoBypassControlli"];
            if (elencoBypassControlli != null && elencoBypassControlli.Count > 0)
            {

                if (!string.IsNullOrEmpty(txtFiltroNumeroDomanda.Text.Trim()))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.NDomus.ToString().Trim().ToUpperInvariant() == txtFiltroNumeroDomanda.Text.Trim().ToUpperInvariant());
                }

                if (!string.IsNullOrEmpty(txtFiltroCodCategoria.Text.Trim()))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.CodCategoria != null && x.CodCategoria.ToString().PadLeft(3, '0').Trim() == txtFiltroCodCategoria.Text.Trim());
                }
                if (!string.IsNullOrEmpty(txtFiltroCodiceSede.Text.Trim()))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.CodiceSede.ToString().ToUpperInvariant().PadLeft(4, '0') == txtFiltroCodiceSede.Text.Trim().ToUpperInvariant());
                }
                if (!string.IsNullOrEmpty(txtFiltroNCertificato.Text.Trim()))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.NCertificato.ToString().Trim().ToUpperInvariant().PadLeft(8, '0') == txtFiltroNCertificato.Text.Trim().ToUpperInvariant());
                }

                if (!string.IsNullOrEmpty(ddlFiltroBypass.SelectedValue))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.IdDecBypassControllo.ToString() == ddlFiltroBypass.SelectedValue);
                }
                if (!string.IsNullOrEmpty(txtFiltroMatricola.Text.Trim()))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.Matricola.Trim().ToUpperInvariant() == txtFiltroMatricola.Text.Trim().ToUpperInvariant());
                }
                if (!string.IsNullOrEmpty(ddlLock.SelectedValue.Trim()))
                {
                    count++;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.Lock == (ddlLock.SelectedValue == "SI" ? true : false));
                }

                //direttore FS
                Ruoli ruolo = (Ruoli)Session["Ruolo"];
                if (ruolo == Ruoli.P8974)
                {
                    count++;
                    string matricola = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
                    elencoBypassControlli = elencoBypassControlli.FindAll(x => x.Matricola.Trim().ToUpperInvariant() == matricola.Trim().ToUpperInvariant());                   
                }
            }
            if (count > 0)
                ViewState["ElencoBypassControlli"] = elencoBypassControlli;
        }

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;

            txtFiltroMatricola.Enabled = false;

            ddlFiltroBypass.Enabled = false;
            txtFiltroMatricola.Enabled = false;
            ddlLock.Enabled = false;

            txtFiltroNumeroDomanda.Enabled = false;
            txtFiltroNumeroDomanda.CssClass = txtFiltroNumeroDomanda.CssClass.Replace("tboxdisable", "");

            txtFiltroCodCategoria.Enabled = false;
            txtFiltroCodCategoria.CssClass = txtFiltroCodCategoria.CssClass.Replace("tboxdisable", "");

            txtFiltroCodiceSede.Enabled = false;
            txtFiltroCodiceSede.CssClass = txtFiltroCodiceSede.CssClass.Replace("tboxdisable", "");

            txtFiltroNCertificato.Enabled = false;
            txtFiltroNCertificato.CssClass = txtFiltroNCertificato.CssClass.Replace("tboxdisable", "");

        }

        private void AbilitaFiltro()
        {
            PulisciPannelloFiltri();
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;

            ddlFiltroBypass.Enabled = true;
            txtFiltroMatricola.Enabled = true;
            ddlLock.Enabled = true;

            txtFiltroNumeroDomanda.Enabled = true;
            txtFiltroCodCategoria.Enabled = true;
            txtFiltroCodiceSede.Enabled = true;
            txtFiltroNCertificato.Enabled = true;
        }

        /// <summary>
        /// Ripulisce i campi del filtro di ricerca
        /// </summary>
        private void PulisciPannelloFiltri()
        {
            txtFiltroNumeroDomanda.Text = string.Empty;
            ddlFiltroBypass.ClearSelection();
            txtFiltroMatricola.Text = string.Empty;
            txtFiltroCodCategoria.Text = string.Empty;
            txtFiltroCodiceSede.Text = string.Empty;
            txtFiltroNCertificato.Text = string.Empty;
            txtFiltroMatricola.Text = string.Empty;

            ddlLock.ClearSelection();
        }

        /// <summary>
        /// Ripulisce i campi deL pannello insert
        /// </summary>
        private void PulisciPannelloInsert()
        {
            txtInsertNumeroDomanda.Text = "";

            txtInsertCodCategoria.Text = "";
            txtInsertCodiceSede.Text = "";
            txtInsertNCertificato.Text = "";

            ddlBypassInsert.ClearSelection();

            lblDescrizioneBypass.Text = "";
            txtNote.Text = "";
        }

        private void RecuperaCampi(out string errore)
        {
            errore = string.Empty;

            if (!radioInsertPensione.Checked && !radioInsertDomanda.Checked)
            {
                errore = "Inserimento: Selezionare il numero domanda o la chiave pensione ";
                return;
            }

            this.datiBypassControllo = new AreaBypassControllo.BypassControllo();

            this.datiBypassControllo.Matricola = lblMatricolaInsert.Text;

            if (!string.IsNullOrEmpty(ddlBypassInsert.SelectedValue))
                this.datiBypassControllo.IdDecBypassControllo = long.Parse(ddlBypassInsert.SelectedValue);

            if (!string.IsNullOrEmpty(txtNote.Text))
                this.datiBypassControllo.Note = txtNote.Text;

            if (radioInsertDomanda.Checked)
            {
                if (!string.IsNullOrEmpty(txtInsertNumeroDomanda.Text))
                    this.datiBypassControllo.NDomus = long.Parse(txtInsertNumeroDomanda.Text);
            }

            if (radioInsertPensione.Checked)
            {
                if (!string.IsNullOrEmpty(txtInsertCodCategoria.Text))
                    this.datiBypassControllo.CodCategoria = txtInsertCodCategoria.Text.Trim();

                if (!string.IsNullOrEmpty(txtInsertCodiceSede.Text))
                {
                    if (txtInsertCodiceSede.Text.Length == 4)
                        this.datiBypassControllo.CodiceSede = short.Parse(txtInsertCodiceSede.Text);
                    else
                    {
                        errore = "Il Codice Sede della chiave pensione deve essere di 4 caratteri";
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtInsertNCertificato.Text))
                {
                    if (txtInsertNCertificato.Text.Length == 8)
                        this.datiBypassControllo.NCertificato = int.Parse(txtInsertNCertificato.Text);
                    else
                    {
                        errore = "Il Certificato della chiave pensione deve essere di 8 caratteri";
                        return;
                    }

                }
            }

            this.datiBypassControllo.Lock = false;
        }
        #endregion private members

        #region Grid
        protected void gvBypassControlli_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                List<AreaBypassControllo.BypassControllo> elencoBypassControlli = (List<AreaBypassControllo.BypassControllo>)ViewState["ElencoBypassControlli"];
                List<AreaBypassControllo.DecBypassControllo> elencoDecBypassControlli = (List<AreaBypassControllo.DecBypassControllo>)ViewState["ElencoDecBypassControlli"];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (elencoDecBypassControlli != null && elencoDecBypassControlli.Count > 0)
                    {
                        Label lblBypass = (Label)e.Row.FindControl("lblBypass");
                        lblBypass.Text = elencoDecBypassControlli.Exists(x => x.Id == ((AreaBypassControllo.BypassControllo)(e.Row.DataItem)).IdDecBypassControllo) ?
                            elencoDecBypassControlli.Find(x => x.Id == ((AreaBypassControllo.BypassControllo)(e.Row.DataItem)).IdDecBypassControllo).Nome : string.Empty;
                    }

                    LinkButton delete = (LinkButton)e.Row.FindControl("btnDelete");
                    int index = e.Row.DataItemIndex;
                    if (index >= 0 && index <= elencoBypassControlli.Count - 1)
                    {
                        delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                        delete.ToolTip = "Elimina";
                        delete.OnClientClick = "BlockUI();";
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCBypassControlli, Errore nel metodo gvBypassControlli_RowDataBound " + ex);
            }
        }

        protected void gvBypassControlli_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                long id = 0;
                long.TryParse(e.CommandArgument.ToString(), out id);
                this.datiBypassControllo = new AreaBypassControllo.BypassControllo();
                this.datiBypassControllo.Id = id;

                PresenterBypassControlli presenter = new PresenterBypassControlli();
                presenter.EliminaBypassControlli(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Controllo riabilitato correttamente";
                    RaiseShowAvviso(this, null);
                }

                int paginaDaVisualizzare = 1;
                ValorizzaGriglia(paginaDaVisualizzare);
            }
            else if (e.CommandName == "ShowNota")
            {
                hdnTextDialog.Value = e.CommandArgument.ToString();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalDialog", "ShowNota();", true);
            }
        }

        protected void gvBypassControlli_onPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvBypassControlli.EditIndex = -1;
                int paginaDaVisualizzare = e.NewPageIndex + 1;//e.NewPageIndex + 1  perchè la  prima pagina ha PageIndex = 0 
                gvBypassControlli_Load(paginaDaVisualizzare);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCBypassControlli, Errore nel metodo gvBypassControlli_onPageIndexChanging" + ex);
            }
        }

        protected void gvBypassControlli_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected string ValorizzaTesto(GridViewRow row)
        {
            AreaBypassControllo.BypassControllo bypass = (AreaBypassControllo.BypassControllo)row.DataItem;

            if (!string.IsNullOrEmpty(bypass.Note))
                return "Vedi nota";
            else
            {
                ((LinkButton)row.FindControl("lblNote")).Enabled = false;
                return "Nessuna nota";
            }
        }
        #endregion Grid

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler HideInfo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }
        #endregion Event Handlers



        protected void radioInsert_CheckedChanged(object sender, EventArgs e)
        {
            if (radioInsertDomanda.Checked)
            {
                txtInsertCodCategoria.Text = string.Empty;
                txtInsertCodiceSede.Text = string.Empty;
                txtInsertNCertificato.Text = string.Empty;
                AbilitaDisabilitaInserimentoPerNumeroDomanda_SuPannelloInsert(true);
                AbilitaDisabilitaInserimentoPerPensione_SuPannelloInsert(false);
            }
            else
            {
                txtInsertNumeroDomanda.Text = string.Empty;
                AbilitaDisabilitaInserimentoPerPensione_SuPannelloInsert(true);
                AbilitaDisabilitaInserimentoPerNumeroDomanda_SuPannelloInsert(false);
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "SetScroll", "SetScroll();", true);

            RaiseHideInfo(this, null);
        }
    }
}