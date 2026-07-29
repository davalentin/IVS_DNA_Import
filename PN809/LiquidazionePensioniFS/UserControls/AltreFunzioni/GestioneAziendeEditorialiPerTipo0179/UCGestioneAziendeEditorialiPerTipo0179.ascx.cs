using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeEditorialiPerTipo0179
{
    public partial class UCGestioneAziendeEditorialiPerTipo0179 : CustomBaseUserControl, IAziendeEditorialiPerTipo0179
    {
        /// <summary>
        /// proprietà ereditate da IViewUI
        /// </summary>
        #region IViewUI

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        #endregion IViewUI

        /// <summary>
        /// proprietà ereditate da IAziendeEditoriali
        /// </summary>
        #region IAziendeEditoriali

        public AreaAziendeEditorialiPerTipo0179 AreaAziendeEditorialiPerTipo0179 { get; set; }
        public string CommaSeparatedDenominazioneAzienda { get; set; }
        public string CommaSeparatedCodice { get; set; }

        #endregion IAziendeEditoriali

        #region protected methods

        /// <summary>
        /// page load, richiama metodi ValorizzaGriglia, Abilita filtro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState[EnumViewState.Filtro.ToString()] == null)
                ViewState[EnumViewState.Filtro.ToString()] = false;

            if (!IsPostBack)
            {
                CaricaDati();
                ValorizzaGrigliaGvAnagraficaAziende();
                ValorizzaGrigliaGvAnagraficaAccordi();
                AbilitaFiltro();

                ValorizzaAutoComplete();
            }

            RaiseHideInfo(this, null);
        }

        #region metodi protected filtro di ricerca

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAnagraficaAccordi.EditIndex = -1;
            gvAnagraficaAziende.EditIndex = -1;

            ViewState[EnumViewState.Filtro.ToString()] = true;
            FiltraGvAnagraficaAccordi();
            FiltraGvAnagraficaAziende();
            gvAnagraficaAccordi_Load();
            gvAnagraficaAziende_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            CaricaDati();
            gvAnagraficaAccordi.EditIndex = -1;
            gvAnagraficaAziende.EditIndex = -1;
            ViewState[EnumViewState.Filtro.ToString()] = false;
            ValorizzaGrigliaGvAnagraficaAziende();
            ValorizzaGrigliaGvAnagraficaAccordi();

            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        #endregion metodi protected filtro di ricerca


        #region metodi protected GridView Anagrafica Accordi

        protected void gvAnagraficaAccordi_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAnagraficaAccordi.EditIndex = -1;
                gvAnagraficaAccordi.PageIndex = e.NewPageIndex;
                gvAnagraficaAccordi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAccordi_onPageIndexChanging" + ex);
            }
        }

        protected void gvAnagraficaAccordi_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvAnagraficaAccordi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()]).Count() < 2)
                    gvAnagraficaAccordi.EditIndex = 0;
                else
                    gvAnagraficaAccordi.EditIndex = -1;
                //Bind data to the GridView control.
                gvAnagraficaAccordi_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAccordi_RowCancelingEdit " + ex);
            }

        }

        protected void gvAnagraficaAccordi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAnagraficaAccordi.EditIndex = e.NewEditIndex;
                gvAnagraficaAccordi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAccordi_RowEditing " + ex);
            }
        }

        protected void gvAnagraficaAccordi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                #region Delete
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeEditorialiPerTipo0179 presenterAziendeEditoriali = new PresenterAziendeEditorialiPerTipo0179();
                ValorizzaAnagraficaAccordiPerDelete(r.DataItemIndex);
                presenterAziendeEditoriali.EliminaAnagraficaAccordi(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else if (this.ErrorMessage != null && this.ErrorMessage != string.Empty)
                {
                    //FG - Se ho un messaggio lo mostro a video anche in caso di OK
                    this.HasError = true;
                    RaiseShowAvviso(this, null);
                }
                else
                {
                    this.ErrorMessage = "Anagrafica Accordi eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvAnagraficaAziende();
                ValorizzaGrigliaGvAnagraficaAccordi();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                ValorizzaAutoComplete();
                #endregion Delete
            }
            else if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                #region Salva
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeEditorialiPerTipo0179 presenterAziendeEditoriali = new PresenterAziendeEditorialiPerTipo0179();
                ValorizzaAnagraficaAccordiPerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeEditoriali.InserisciAnagraficaAccordi(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);

                    string domandeLiquidabili = r.Cells[5].Text;

                    if (domandeLiquidabili != string.Empty && (((TextBox)r.FindControl(Keys.txtDomandeLiquidabili)).Text == null || ((TextBox)r.FindControl(Keys.txtDomandeLiquidabili)).Text == string.Empty))
                        r.Cells[5].Text = domandeLiquidabili;
                    return;
                }
                else
                {
                    this.ErrorMessage = "Anagrafica Accordi inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAnagraficaAccordi.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvAnagraficaAziende();
                ValorizzaGrigliaGvAnagraficaAccordi();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                ValorizzaAutoComplete();
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvAnagraficaAccordi.EditIndex = -1;
                gvAnagraficaAccordi_Load();
            }
        }

        protected void gvAnagraficaAccordi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordi = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        //FG - Valorizzo la dropDownList Abilitata con SI o NO
                        DropDownList ddlAbilitata = new DropDownList();
                        ddlAbilitata = (DropDownList)e.Row.FindControl("ddlAbilitata");
                        if (!string.IsNullOrEmpty(((AnagraficaAccordoPerTipo0179)e.Row.DataItem).AbilitataTxt))
                            ddlAbilitata.SelectedValue = ((AnagraficaAccordoPerTipo0179)e.Row.DataItem).AbilitataTxt;
                        else
                            ddlAbilitata.SelectedValue = "NO";

                        //FG - Controllo che sia label che textbox siano vuote, in questo caso è un ADD, altrimenti un EDIT
                        string label = string.Empty;
                        string textbox = string.Empty;

                        if (((Label)e.Row.FindControl("lblDenominazioneAziendaCode")) != null)
                            label = ((Label)e.Row.FindControl("lblDenominazioneAziendaCode")).Text;

                        if (((TextBox)e.Row.FindControl("txtDenominazioneAziendaCode")) != null)
                            textbox = ((TextBox)e.Row.FindControl("txtDenominazioneAziendaCode")).Text;

                        //FG - Disabilito la colonna di "Domande Liquidabili" in caso di EDIT
                        if (label != string.Empty || textbox != string.Empty)
                        {
                            e.Row.Cells[5].Text = ((TextBox)e.Row.FindControl(Keys.txtDomandeLiquidabili)).Text;

                            //FG - Valorizzo la denominazioneAzienda con il valore testuale e non con l'ID
                            string idDenominazioneAziendaOld = string.Empty;
                            string idDenominazioneAzienda = string.Empty;

                            if (((TextBox)e.Row.FindControl("txtDenominazioneAziendaCode")) != null)
                            {
                                idDenominazioneAziendaOld = ((TextBox)e.Row.FindControl("txtDenominazioneAziendaCode")).Text;
                                idDenominazioneAzienda = ConvertIdToDenominazioneAzienda(idDenominazioneAziendaOld);
                                ((TextBox)e.Row.FindControl("txtDenominazioneAziendaCode")).Text = idDenominazioneAzienda;
                            }
                        }

                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAccordi, Page.Theme);
                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.OnClientClick = "BlockUI()";
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.OnClientClick = "BlockUI()";
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAnagraficaAccordi.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[7], Page.Theme, Keys.BtnDelete_GrigliaAccordi);
                            //FG - Valorizzo la denominazioneAzienda con il valore testuale e non con l'ID
                            string idDenominazioneAziendaOld = string.Empty;
                            string idDenominazioneAzienda = string.Empty;

                            if (((Label)e.Row.FindControl("lblDenominazioneAziendaCode")) != null)
                            {
                                idDenominazioneAziendaOld = ((Label)e.Row.FindControl("lblDenominazioneAziendaCode")).Text;
                                idDenominazioneAzienda = ConvertIdToDenominazioneAzienda(idDenominazioneAziendaOld);
                                ((Label)e.Row.FindControl("lblDenominazioneAziendaCode")).Text = idDenominazioneAzienda;
                            }
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAccordi_RowDataBound " + ex);
            }
        }

        #endregion metodi protected GridView Anagrafica Accordi

        #region metodi protected GridView Aziende

        protected void gvAnagraficaAziende_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAnagraficaAziende.EditIndex = -1;
                gvAnagraficaAziende.PageIndex = e.NewPageIndex;
                gvAnagraficaAziende_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAziende_onPageIndexChanging" + ex);
            }
        }

        protected void gvAnagraficaAziende_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvAnagraficaAziende.EditIndex = e.NewEditIndex;
                gvAnagraficaAziende_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAziende_RowEditing " + ex);
            }
        }

        protected void gvAnagraficaAziende_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()]).Count() < 2)
                    gvAnagraficaAziende.EditIndex = 0;
                else
                    gvAnagraficaAziende.EditIndex = -1;
                //Bind data to the GridView control.
                gvAnagraficaAziende_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAziende_RowCancelingEdit " + ex);
            }

        }

        protected void gvAnagraficaAziende_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvAnagraficaAziende_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Delete
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeEditorialiPerTipo0179 presenterAziendeEditoriali = new PresenterAziendeEditorialiPerTipo0179();
                ValorizzaAnagraficaAziendePerDelete(r.DataItemIndex);
                presenterAziendeEditoriali.EliminaAnagraficaAziende(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Anagrafica Aziende eliminata correttamente";
                    RaiseShowAvviso(this, null);
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvAnagraficaAziende();
                ValorizzaGrigliaGvAnagraficaAccordi();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                ValorizzaAutoComplete();
                #endregion Delete

            }
            else if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterAziendeEditorialiPerTipo0179 presenterAziendeEditoriali = new PresenterAziendeEditorialiPerTipo0179();
                ValorizzaAnagraficaAziendePerSave(r);
                if (this.HasError)
                    return;

                presenterAziendeEditoriali.InserisciAnagraficaAziende(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Anagrafica Aziende inserita correttamente";
                    RaiseShowAvviso(this, null);
                    gvAnagraficaAziende.EditIndex = -1;
                }

                AbilitaFiltro();
                ValorizzaGrigliaGvAnagraficaAziende();
                ValorizzaGrigliaGvAnagraficaAccordi();
                ViewState[EnumViewState.Filtro.ToString()] = false;
                ValorizzaAutoComplete();
            }
            else if (e.CommandName == "Annulla")
            {
                gvAnagraficaAziende.EditIndex = -1;
                gvAnagraficaAziende_Load();
            }
        }

        protected void gvAnagraficaAziende_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];

                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaAziende, Page.Theme);
                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.OnClientClick = "BlockUI()";
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.OnClientClick = "BlockUI()";
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoAnagraficaAziende.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[2], Page.Theme, Keys.BtnElimina_GrigliaAziende);
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            edit.Text = "";
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
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAziende_RowDataBound " + ex);
            }
        }

        #endregion metodi protected dellla GridView Aziende

        #endregion protected methods

        #region private methods

        private void CaricaDati()
        {
            PresenterAziendeEditorialiPerTipo0179 presenterAziendeEditoriali = new PresenterAziendeEditorialiPerTipo0179();
            presenterAziendeEditoriali.CaricaAreaAziendeEditoriali(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
        }

        /// <summary>
        /// metodi privati del Filtro di ricerca
        /// </summary>
        #region metodi privati filtro di ricerca

        private void FiltraGvAnagraficaAccordi()
        {
            int count = 0;
            List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordiWS = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroCodice.Text.Trim()))
            {
                count++;
                elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => (x.Codice.HasValue ? x.Codice.ToString() : null) == txtFiltroCodice.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(ddlFiltroAbilitata.SelectedValue.Trim()))
            {
                count++;
                if (ddlFiltroAbilitata.SelectedValue.ToUpperInvariant() == "SI")
                    elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => x.Abilitata == true);
                else if (ddlFiltroAbilitata.SelectedValue.ToUpperInvariant() == "NO")
                    elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => x.Abilitata == false);
            }

            if (!string.IsNullOrEmpty(txtFiltroDenominazioneAzienda.Text.Trim()))
            {
                count++;
                elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => ConvertIdToDenominazioneAzienda(x.DenominazioneAzienda.GetValueOrDefault().ToString()) == txtFiltroDenominazioneAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroDataAccordiDa.Text) && Utility.GetDateFromString(txtFiltroDataAccordiDa.Text).HasValue)
            {
                count++;
                elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => x.DataAccordi.HasValue && Utility.DataSuccessivaA(x.DataAccordi.Value, Utility.GetDateFromString(txtFiltroDataAccordiDa.Text).Value));
            }

            if (!string.IsNullOrEmpty(txtFiltroDataAccordiA.Text) && Utility.GetDateFromString(txtFiltroDataAccordiA.Text).HasValue)
            {
                count++;
                elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => x.DataAccordi.HasValue && !Utility.DataStrettamenteSuccessivaA(x.DataAccordi.Value, Utility.GetDateFromString(txtFiltroDataAccordiA.Text).Value));
            }

            if (count > 0)
            {
                ViewState[EnumViewState.AnagraficaAccordi.ToString()] = elencoAnagraficaAccordiWS;
                elencoAnagraficaAccordiWS.Add(new AnagraficaAccordoPerTipo0179());

                DisabilitaFiltro();
            }
        }

        private void FiltraGvAnagraficaAziende()
        {
            int count = 0;
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziendeWS = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];

            if (!string.IsNullOrEmpty(txtFiltroDenominazioneAzienda.Text.Trim()))
            {
                count++;
                elencoAnagraficaAziendeWS = elencoAnagraficaAziendeWS.FindAll(x => (x.DenominazioneAzienda != null ? x.DenominazioneAzienda.Trim().ToUpperInvariant() : null) == txtFiltroDenominazioneAzienda.Text.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(txtFiltroSottoGruppoOneri.Text.Trim()))
            {
                count++;
                elencoAnagraficaAziendeWS = elencoAnagraficaAziendeWS.FindAll(x => x.SottogruppoPrimoOnere == txtFiltroSottoGruppoOneri.Text || x.SottogruppoSecondoOnere == txtFiltroSottoGruppoOneri.Text);
            }

            if (count > 0)
            {
                List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordiWS = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];
                elencoAnagraficaAccordiWS = elencoAnagraficaAccordiWS.FindAll(x => elencoAnagraficaAziendeWS.Exists(y => y.Id == x.DenominazioneAzienda));

                ViewState[EnumViewState.AnagraficaAziende.ToString()] = elencoAnagraficaAziendeWS;
                elencoAnagraficaAziendeWS.Add(new AnagraficaAziendaPerTipo0179());

                ViewState[EnumViewState.AnagraficaAccordi.ToString()] = elencoAnagraficaAccordiWS;
                elencoAnagraficaAccordiWS.Add(new AnagraficaAccordoPerTipo0179());

                DisabilitaFiltro();
            }
        }

        private void AbilitaFiltro()
        {
            PulisciFiltro();
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;
            txtFiltroCodice.Enabled = true;
            ddlFiltroAbilitata.Enabled = true;
            txtFiltroDenominazioneAzienda.Enabled = true;
            txtFiltroSottoGruppoOneri.Enabled = true;
            txtFiltroDataAccordiDa.Enabled = true;
            txtFiltroDataAccordiA.Enabled = true;
            //gvAnagraficaAccordi.EditIndex = -1;
            //gvAnagraficaAziende.EditIndex = -1;
        }

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;
            txtFiltroCodice.Enabled = false;
            ddlFiltroAbilitata.Enabled = false;
            txtFiltroDenominazioneAzienda.Enabled = false;
            txtFiltroSottoGruppoOneri.Enabled = false;
            txtFiltroDataAccordiDa.Enabled = false;
            txtFiltroDataAccordiA.Enabled = false;
        }

        private void PulisciFiltro()
        {
            txtFiltroCodice.Text = string.Empty;
            ddlFiltroAbilitata.Text = string.Empty;
            txtFiltroDenominazioneAzienda.Text = string.Empty;
            txtFiltroSottoGruppoOneri.Text = string.Empty;
            txtFiltroDataAccordiDa.Text = string.Empty;
            txtFiltroDataAccordiA.Text = string.Empty;
        }

        private void ValorizzaAutoComplete()
        {
            GetCommaSeparatedCodice();
            HiddenFieldCodice.Value = CommaSeparatedCodice;

            GetCommaSeparatedDenominazioneAzienda();
            HiddenFieldDenominazioneAzienda.Value = CommaSeparatedDenominazioneAzienda;
        }

        private void GetCommaSeparatedCodice()
        {
            List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordi = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];
            if (elencoAnagraficaAccordi != null && elencoAnagraficaAccordi.Count > 0)
                CommaSeparatedCodice = string.Join(";", elencoAnagraficaAccordi.Where(x => x.Codice.HasValue).Select(x => x.Codice.Value.ToString()).ToArray());
        }

        private void GetCommaSeparatedDenominazioneAzienda()
        {
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];
            if (elencoAnagraficaAziende != null && elencoAnagraficaAziende.Count > 0)
                CommaSeparatedDenominazioneAzienda = string.Join(";", elencoAnagraficaAziende.Where(x => !string.IsNullOrEmpty(x.DenominazioneAzienda)).Select(x => x.DenominazioneAzienda.Trim().ToUpperInvariant()).ToArray());
        }

        #endregion metodi privati del filtro di ricerca

        #region metodi privati GvAnagraficaAccordi

        /// <summary>
        /// metodo valorizza griglia 
        /// richiama metodo presenter carica anagrafiche accordi
        /// richiama metodo private formatta
        /// richiama metodo load
        /// </summary>
        private void ValorizzaGrigliaGvAnagraficaAccordi()
        {
            FormattaElencoAnagraficheAccordi();

            //// Va all'ultima pagina
            if ((List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()] != null)
                gvAnagraficaAccordi.PageIndex = ((List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()]).Count / gvAnagraficaAccordi.PageSize;

            gvAnagraficaAccordi_Load();
        }

        private void FormattaElencoAnagraficheAccordi()
        {
            List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordiWS = new List<AnagraficaAccordoPerTipo0179>();
            if (this.AreaAziendeEditorialiPerTipo0179.ElencoAnagraficheAccordi == null || this.AreaAziendeEditorialiPerTipo0179.ElencoAnagraficheAccordi.Count() == 0)
            {
                elencoAnagraficaAccordiWS.Add(new AnagraficaAccordoPerTipo0179());
            }
            else
            {
                elencoAnagraficaAccordiWS = this.AreaAziendeEditorialiPerTipo0179.ElencoAnagraficheAccordi.ToList();
                elencoAnagraficaAccordiWS.Add(new AnagraficaAccordoPerTipo0179());
            }

            if (elencoAnagraficaAccordiWS.Count() < 2)
                gvAnagraficaAccordi.EditIndex = 0;

            ViewState[EnumViewState.AnagraficaAccordi.ToString()] = elencoAnagraficaAccordiWS;
        }

        private void gvAnagraficaAccordi_Load()
        {
            try
            {
                gvAnagraficaAccordi.DataSource = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];
                gvAnagraficaAccordi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAccordi_Load " + ex);
            }
        }

        private void ValorizzaAnagraficaAccordiPerSave(GridViewRow row)
        {
            List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordi = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];

            AnagraficaAccordoPerTipo0179 accordiApp = new AnagraficaAccordoPerTipo0179();

            accordiApp.Id = elencoAnagraficaAccordi[row.DataItemIndex].Id;

            if ((((DropDownList)row.FindControl(Keys.ddlAbilitata)).SelectedValue) == "SI")
                accordiApp.Abilitata = true;
            else if ((((DropDownList)row.FindControl(Keys.ddlAbilitata)).SelectedValue) == "NO")
                accordiApp.Abilitata = false;

            accordiApp.Codice = CodeUtility.StringToNullableShort(((TextBox)row.FindControl(Keys.txtCodice)).Text);
            accordiApp.DataAccordi = Utility.GetDateFromString(((TextBox)row.FindControl(Keys.txtDataAccordi)).Text);

            string denominazioneAzienda = ((TextBox)row.FindControl(Keys.txtDenominazioneAziendaCode)).Text;
            accordiApp.DenominazioneAzienda = ConvertDenominazioneAziendaToId(denominazioneAzienda);

            if (!string.IsNullOrEmpty(((TextBox)row.FindControl(Keys.txtDomandeLiquidabili)).Text))
                accordiApp.DomandeLiquidabili = CodeUtility.StringToNullableInt(((TextBox)row.FindControl(Keys.txtDomandeLiquidabili)).Text);
            else
            {
                int domandeLiquidabili = 0;
                if (int.TryParse(row.Cells[5].Text, out domandeLiquidabili))
                    accordiApp.DomandeLiquidabili = domandeLiquidabili;
                else
                    accordiApp.DomandeLiquidabili = null;
            }

            accordiApp.DomandeLiquidate = elencoAnagraficaAccordi[row.DataItemIndex].DomandeLiquidate;
            if (!accordiApp.DomandeLiquidate.HasValue)
                accordiApp.DomandeLiquidate = 0;

            ValorizzaAnagraficaAccordi(accordiApp);
        }

        private long? ConvertDenominazioneAziendaToId(string denominazioneAzienda)
        {
            long idDenominazioneAzienda = 0;
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];

            if (elencoAnagraficaAziende != null)
            {
                foreach (var azienda in elencoAnagraficaAziende)
                {
                    if (azienda.DenominazioneAzienda != null && azienda.DenominazioneAzienda.ToUpperInvariant().Trim() == denominazioneAzienda.ToUpperInvariant().Trim())
                    {
                        idDenominazioneAzienda = azienda.Id;
                        break;
                    }
                }
            }

            return idDenominazioneAzienda;
        }

        private string ConvertIdToDenominazioneAzienda(long? idDenominazioneAzienda)
        {
            string denominazioneAzienda = string.Empty;
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];

            if (elencoAnagraficaAziende != null)
            {
                foreach (var azienda in elencoAnagraficaAziende)
                {
                    if (azienda.Id == idDenominazioneAzienda)
                    {
                        denominazioneAzienda = azienda.DenominazioneAzienda;
                        break;
                    }
                }
            }

            if (denominazioneAzienda != null)
                denominazioneAzienda = denominazioneAzienda.ToUpperInvariant();

            return denominazioneAzienda;
        }

        private string ConvertIdToDenominazioneAzienda(string denominazioneAzienda)
        {
            long? idDenominazioneAzienda = CodeUtility.StringToNullableLong(denominazioneAzienda);

            return ConvertIdToDenominazioneAzienda(idDenominazioneAzienda);
        }

        private void ValorizzaAnagraficaAccordiPerDelete(int index)
        {
            List<AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordi = (List<AnagraficaAccordoPerTipo0179>)ViewState[EnumViewState.AnagraficaAccordi.ToString()];

            AnagraficaAccordoPerTipo0179 accordiApp = elencoAnagraficaAccordi[index];
            ValorizzaAnagraficaAccordi(accordiApp);
        }

        private void ValorizzaAnagraficaAccordi(AnagraficaAccordoPerTipo0179 anagraficaAccordi)
        {
            if (this.AreaAziendeEditorialiPerTipo0179 == null)

                this.AreaAziendeEditorialiPerTipo0179 = new AreaAziendeEditorialiPerTipo0179();
            this.AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi = new AnagraficaAccordoPerTipo0179();

            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.Id = anagraficaAccordi.Id;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.Abilitata = anagraficaAccordi.Abilitata;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.AbilitataTxt = anagraficaAccordi.AbilitataTxt;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.Codice = anagraficaAccordi.Codice;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.DataAccordi = anagraficaAccordi.DataAccordi;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.DenominazioneAzienda = anagraficaAccordi.DenominazioneAzienda;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.DomandeLiquidabili = anagraficaAccordi.DomandeLiquidabili;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAccordi.DomandeLiquidate = anagraficaAccordi.DomandeLiquidate;
        }

        #endregion metodi privati gvAnagraficaAccordi

        #region metodi privati gvAnagraficaAziende

        private void ValorizzaGrigliaGvAnagraficaAziende()
        {
            FormattaElencoAziende();

            //// Va all'ultima pagina
            if ((List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()] != null)
                gvAnagraficaAziende.PageIndex = ((List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()]).Count / gvAnagraficaAziende.PageSize;

            gvAnagraficaAziende_Load();
        }

        private void FormattaElencoAziende()
        {
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziendeWS = new List<AnagraficaAziendaPerTipo0179>();
            if (this.AreaAziendeEditorialiPerTipo0179.ElencoAnagraficheAziende == null || this.AreaAziendeEditorialiPerTipo0179.ElencoAnagraficheAziende.Count() == 0)
            {
                elencoAnagraficaAziendeWS.Add(new AnagraficaAziendaPerTipo0179());
            }
            else
            {
                elencoAnagraficaAziendeWS = this.AreaAziendeEditorialiPerTipo0179.ElencoAnagraficheAziende.ToList();
                elencoAnagraficaAziendeWS.Add(new AnagraficaAziendaPerTipo0179());
            }

            if (elencoAnagraficaAziendeWS.Count() < 2)
                gvAnagraficaAziende.EditIndex = 0;

            ViewState[EnumViewState.AnagraficaAziende.ToString()] = elencoAnagraficaAziendeWS;
        }

        private void gvAnagraficaAziende_Load()
        {
            try
            {
                gvAnagraficaAziende.DataSource = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];
                gvAnagraficaAziende.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCGestioneAziendeEditorialiPerTipo0179, Errore nel metodo gvAnagraficaAziende_Load " + ex);
            }
        }

        private void ValorizzaAnagraficaAziendePerSave(GridViewRow r)
        {
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];

            AnagraficaAziendaPerTipo0179 azApp = new AnagraficaAziendaPerTipo0179();

            azApp.Id = elencoAnagraficaAziende[r.DataItemIndex].Id;

            azApp.DenominazioneAzienda = (((TextBox)r.FindControl(Keys.txtDenominazioneAzienda)).Text);
            azApp.SottogruppoPrimoOnere = (((TextBox)r.FindControl(Keys.txtSottogruppoPrimoOnere)).Text);
            if (!string.IsNullOrEmpty(((TextBox)r.FindControl(Keys.txtSottogruppoSecondoOnere)).Text.Trim()))
                azApp.SottogruppoSecondoOnere = (((TextBox)r.FindControl(Keys.txtSottogruppoSecondoOnere)).Text);

            ValorizzaAnagraficaAziende(azApp);
        }

        private void ValorizzaAnagraficaAziendePerDelete(int index)
        {
            List<AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende = (List<AnagraficaAziendaPerTipo0179>)ViewState[EnumViewState.AnagraficaAziende.ToString()];

            AnagraficaAziendaPerTipo0179 aziendeApp = elencoAnagraficaAziende[index];
            ValorizzaAnagraficaAziende(aziendeApp);
        }

        private void ValorizzaAnagraficaAziende(AnagraficaAziendaPerTipo0179 aziende)
        {
            if (this.AreaAziendeEditorialiPerTipo0179 == null)
                this.AreaAziendeEditorialiPerTipo0179 = new AreaAziendeEditorialiPerTipo0179();

            this.AreaAziendeEditorialiPerTipo0179.AnagraficheAziende = new AnagraficaAziendaPerTipo0179();

            AreaAziendeEditorialiPerTipo0179.AnagraficheAziende.Id = aziende.Id;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAziende.DenominazioneAzienda = aziende.DenominazioneAzienda;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAziende.SottogruppoPrimoOnere = aziende.SottogruppoPrimoOnere;
            AreaAziendeEditorialiPerTipo0179.AnagraficheAziende.SottogruppoSecondoOnere = aziende.SottogruppoSecondoOnere;
        }

        #endregion metodi privati gvAnagraficaAziende

        #endregion private methods

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler HideInfo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            if (HideInfo != null)
                HideInfo(sender, e);
        }
        #endregion Events

        #region Enum
        public enum EnumViewState
        {
            AnagraficaAccordi,
            AnagraficaAziende,
            Filtro
        }

        #endregion Enum

        #region private classes
        private class Keys
        {
            public const string ValidationGroup_GrigliaAccordi = "GrigliaAccordi";
            public const string ValidationGroup_GrigliaAziende = "GrigliaAziende";
            public const string BtnDelete_GrigliaAccordi = "btnDelete";
            public const string BtnElimina_GrigliaAziende = "btnElimina";
            public const string ddlAbilitata = "ddlAbilitata";
            public const string txtCodice = "txtCodice";
            public const string txtDataAccordi = "txtDataAccordi";
            public const string txtDecreto = "txtDecreto";
            public const string txtDenominazioneAziendaCode = "txtDenominazioneAziendaCode";
            public const string txtDomandeLiquidabili = "txtDomandeLiquidabili";
            public const string txtDomandeLiquidate = "txtDomandeLiquidate";
            public const string txtDenominazioneAzienda = "txtDenominazioneAzienda";
            public const string txtSottogruppoPrimoOnere = "txtSottogruppoPrimoOnere";
            public const string txtSottogruppoSecondoOnere = "txtSottogruppoSecondoOnere";
        }
        #endregion private classes
    }
}