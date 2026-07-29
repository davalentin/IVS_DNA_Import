using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiFondoGAS_ES : CustomBaseUserControl, IDatiContributivi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalvaDatiFondo_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiFondoGAS(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiFondo_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiFondoGAS(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Fondo";
            else
            {
                ClearForm();
                ValorizzaEtichette();
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        public void ValorizzaEtichette()
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.SvrLiquidazioneFs.GestioneContribEntityDatiFondo DatiFondoGAS_ES = areaDatiContributivi.DatiFondo;

            RenderControls();
            switch (domanda.Tipofondo)
            {
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    if (areaDatiContributivi != null && areaDatiContributivi.DatiFondo != null)
                    {
                        if (DatiFondoGAS_ES.ServizioUtileAA.HasValue)
                            txtServizioUtileAA.Text = areaDatiContributivi.DatiFondo.ServizioUtileAA.ToString();

                        if (DatiFondoGAS_ES.ServizioUtileMM.HasValue)
                            txtServizioUtileMM.Text = areaDatiContributivi.DatiFondo.ServizioUtileMM.ToString();

                        if (DatiFondoGAS_ES.RetribuzionePensionabile.HasValue)
                            txtRetribuzionePensionabile.Text = areaDatiContributivi.DatiFondo.RetribuzionePensionabile.ToString();
                    }
                    break;
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:

                    if (areaDatiContributivi != null && DatiFondoGAS_ES.fondoES != null)
                    {
                        //decodifica Articolo58
                        ddlArticolo58.Items.Clear();
                        CodeUtility.SetValueDdl(ddlArticolo58, "", "", "");
                        foreach (var dec in DatiFondoGAS_ES.fondoES.DecArt58)
                        {
                            CodeUtility.SetValueDdl(ddlArticolo58, dec.Id.ToString(), dec.Descrizione, dec.Id.ToString());
                        }
                        //decodifica Promiscui
                        ddlPromiscui.Items.Clear();
                        CodeUtility.SetValueDdl(ddlPromiscui, "", "", "0");
                        foreach (var dec in DatiFondoGAS_ES.fondoES.DecPromiscui)
                        {
                            CodeUtility.SetValueDdl(ddlPromiscui, dec.Id.ToString(), dec.Descrizione, dec.Id.ToString());
                        }

                        if (DatiFondoGAS_ES.fondoES.Articolo58.HasValue)
                            ddlArticolo58.SelectedValue = DatiFondoGAS_ES.fondoES.Articolo58.ToString();
                        if (DatiFondoGAS_ES.fondoES.Articolo59.HasValue)
                            ddlArticolo59.SelectedValue = DatiFondoGAS_ES.fondoES.Articolo59.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.Optanti.HasValue)
                            ddlOptanti.SelectedValue = DatiFondoGAS_ES.fondoES.Optanti.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.Saltuari.HasValue)
                            ddlSaltuari.SelectedValue = DatiFondoGAS_ES.fondoES.Saltuari.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.Promiscui.HasValue)
                            ddlPromiscui.SelectedValue = DatiFondoGAS_ES.fondoES.Promiscui.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.CodiceDz.HasValue)
                            ddlCodiceEsDz.SelectedValue = DatiFondoGAS_ES.fondoES.CodiceDz.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.AnnoUtile.HasValue)
                            ddlAnnoUtile.SelectedValue = DatiFondoGAS_ES.fondoES.AnnoUtile.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.CodiciRetributivi.HasValue)
                            ddlCodiceRetribuzione.SelectedValue = DatiFondoGAS_ES.fondoES.CodiciRetributivi.ToString().ToLowerInvariant();
                        if (DatiFondoGAS_ES.fondoES.MaggiorazionePrivilegiata.HasValue)
                            ddlMaggiorazionePrivilegiata.SelectedValue = DatiFondoGAS_ES.fondoES.MaggiorazionePrivilegiata.ToString().ToLowerInvariant();
                        //txtbox
                        if (DatiFondoGAS_ES.fondoES.ClassePensioneAnte50.HasValue)
                            txtboxClasseAnte50.Text = DatiFondoGAS_ES.fondoES.ClassePensioneAnte50.ToString();
                        if (DatiFondoGAS_ES.fondoES.CodiceEsattoria != null)
                            txtboxCodiceEsattoria.Text = DatiFondoGAS_ES.fondoES.CodiceEsattoria;
                        if (DatiFondoGAS_ES.RetribuzionePensionabile.HasValue)
                            txtRetribuzionePensionabile.Text = DatiFondoGAS_ES.RetribuzionePensionabile.ToString();
                        if (DatiFondoGAS_ES.ServizioUtileAA.HasValue)
                            txtServizioUtileAA.Text = DatiFondoGAS_ES.ServizioUtileAA.ToString();
                        if (DatiFondoGAS_ES.ServizioUtileMM.HasValue)
                            txtServizioUtileMM.Text = DatiFondoGAS_ES.ServizioUtileMM.ToString();
                        //elementi calcolo
                        List<ElementiCalcolo> lstElemCalcolo = new List<ElementiCalcolo>();
                        MapToLocalObject(out lstElemCalcolo, DatiFondoGAS_ES.fondoES);

                        if (lstElemCalcolo.Count == 0)
                        {
                            gvElementiDiCalcolo.EditIndex = 0;
                        }

                        ViewState["elementiCalcolo"] = lstElemCalcolo;

                        //add empty element
                        inserisciElementoCalcolo();
                        GvElementiCalcolo_Load();
                    }
                    break;
            }
        }


        public void RecuperaCampi()
        {
            if (areaDatiContributivi == null)
                areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (domanda.Tipofondo)
            {
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    if (areaDatiContributivi.DatiFondo == null)
                        areaDatiContributivi.DatiFondo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribEntityDatiFondo();
                    if (!string.IsNullOrEmpty(txtServizioUtileAA.Text))
                        areaDatiContributivi.DatiFondo.ServizioUtileAA = short.Parse(txtServizioUtileAA.Text);
                    if (!string.IsNullOrEmpty(txtServizioUtileMM.Text))
                        areaDatiContributivi.DatiFondo.ServizioUtileMM = short.Parse(txtServizioUtileMM.Text);
                    if (!string.IsNullOrEmpty(txtRetribuzionePensionabile.Text))
                        areaDatiContributivi.DatiFondo.RetribuzionePensionabile = decimal.Parse(txtRetribuzionePensionabile.Text);
                    if (!string.IsNullOrEmpty(txtControcodice.Text))
                        areaDatiContributivi.DatiFondo.ControCodice = int.Parse(txtControcodice.Text);
                    break;
                case Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:

                    if (areaDatiContributivi.DatiFondo == null)
                        areaDatiContributivi.DatiFondo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribEntityDatiFondo();
                    if (!string.IsNullOrEmpty(txtServizioUtileAA.Text))
                        areaDatiContributivi.DatiFondo.ServizioUtileAA = short.Parse(txtServizioUtileAA.Text);
                    if (!string.IsNullOrEmpty(txtServizioUtileMM.Text))
                        areaDatiContributivi.DatiFondo.ServizioUtileMM = short.Parse(txtServizioUtileMM.Text);
                    if (!string.IsNullOrEmpty(txtRetribuzionePensionabile.Text))
                        areaDatiContributivi.DatiFondo.RetribuzionePensionabile = decimal.Parse(txtRetribuzionePensionabile.Text);
                    if (!string.IsNullOrEmpty(txtControcodice.Text))
                        areaDatiContributivi.DatiFondo.ControCodice = int.Parse(txtControcodice.Text);


                    if (areaDatiContributivi.DatiFondo.fondoES == null)
                        areaDatiContributivi.DatiFondo.fondoES = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribFondoES();

                    Presenter.SvrLiquidazioneFs.GestioneContribEntityDatiFondo DatiFondoES = areaDatiContributivi.DatiFondo;
                    if (ddlArticolo58.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.Articolo58 = (byte?)byte.Parse(ddlArticolo58.SelectedValue);
                    if (ddlArticolo59.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.Articolo59 = bool.Parse(ddlArticolo59.SelectedValue);
                    if (ddlArticolo59.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.Optanti = bool.Parse(ddlOptanti.SelectedValue);
                    if (ddlSaltuari.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.Saltuari = bool.Parse(ddlSaltuari.SelectedValue);
                    if (ddlPromiscui.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.Promiscui = byte.Parse(ddlPromiscui.SelectedValue);
                    if (ddlCodiceEsDz.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.CodiceDz = bool.Parse(ddlCodiceEsDz.SelectedValue);
                    if (ddlAnnoUtile.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.AnnoUtile = (bool?)bool.Parse(ddlAnnoUtile.SelectedValue);
                    if (ddlCodiceRetribuzione.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.CodiciRetributivi = (byte?)byte.Parse(ddlCodiceRetribuzione.SelectedValue);
                    if (ddlMaggiorazionePrivilegiata.SelectedValue != string.Empty)
                        DatiFondoES.fondoES.MaggiorazionePrivilegiata = bool.Parse(ddlMaggiorazionePrivilegiata.SelectedValue);
                    if (txtboxClasseAnte50.Text != string.Empty)
                        DatiFondoES.fondoES.ClassePensioneAnte50 = byte.Parse(txtboxClasseAnte50.Text);
                    if (txtboxCodiceEsattoria.Text != string.Empty)
                        DatiFondoES.fondoES.CodiceEsattoria = txtboxCodiceEsattoria.Text;
                    if (txtRetribuzionePensionabile.Text != string.Empty)
                        DatiFondoES.RetribuzionePensionabile = decimal.Parse(txtRetribuzionePensionabile.Text);
                    if (txtServizioUtileAA.Text != string.Empty)
                        DatiFondoES.ServizioUtileAA = short.Parse(txtServizioUtileAA.Text);
                    if (txtServizioUtileMM.Text != string.Empty)
                        DatiFondoES.ServizioUtileMM = short.Parse(txtServizioUtileMM.Text);
                    List<ElementiCalcolo> lstElemCalc = (List<ElementiCalcolo>)ViewState["elementiCalcolo"];
                    if (lstElemCalc != null && lstElemCalc.Count > 0)
                    {
                        List<ElementiCalcolo> lstElemToSave = lstElemCalc.Where((elem) => { return elem.MesiServizioUtile.HasValue && elem.Retribuzione.HasValue; }).ToList();
                        var datiFondo = DatiFondoES.fondoES;
                        MapFromLocalObject(ref datiFondo, lstElemToSave);
                    }
                    break;
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            // popolamento dei controls html con i valori di default (es.: txtPippo.Text = "mm/aaaa";)
        }

        private void MapFromLocalObject(ref Presenter.SvrLiquidazioneFs.GestioneContribFondoES DatiFondo, List<ElementiCalcolo> lstElemCalcolo)
        {
            if (lstElemCalcolo != null && lstElemCalcolo.Count > 0)
            {
                DatiFondo.Retribuzione = lstElemCalcolo.ElementAt(0).Retribuzione;
                DatiFondo.MMServizioUtile = lstElemCalcolo.ElementAt(0).MesiServizioUtile;
                if (lstElemCalcolo.Count > 1)
                {
                    DatiFondo.Retribuzione2 = lstElemCalcolo.ElementAt(1).Retribuzione;
                    DatiFondo.MMServizioUtile2 = lstElemCalcolo.ElementAt(1).MesiServizioUtile;
                }
                if (lstElemCalcolo.Count > 2)
                {
                    DatiFondo.Retribuzione3 = lstElemCalcolo.ElementAt(2).Retribuzione;
                    DatiFondo.MMServizioUtile3 = lstElemCalcolo.ElementAt(2).MesiServizioUtile;
                }
                if (lstElemCalcolo.Count > 3)
                {
                    DatiFondo.Retribuzione4 = lstElemCalcolo.ElementAt(3).Retribuzione;
                    DatiFondo.MMServizioUtile4 = lstElemCalcolo.ElementAt(3).MesiServizioUtile;
                }
            }
        }

        private void MapToLocalObject(out List<ElementiCalcolo> lstElemCalcolo, Presenter.SvrLiquidazioneFs.GestioneContribFondoES DatiFondoGAS_ES)
        {
            lstElemCalcolo = new List<ElementiCalcolo>();
            if (DatiFondoGAS_ES.Retribuzione.HasValue && DatiFondoGAS_ES.MMServizioUtile.HasValue)
            {
                lstElemCalcolo.Add(new ElementiCalcolo(DatiFondoGAS_ES.MMServizioUtile.Value, DatiFondoGAS_ES.Retribuzione.Value));
            }
            if (DatiFondoGAS_ES.Retribuzione2.HasValue && DatiFondoGAS_ES.MMServizioUtile2.HasValue)
            {
                lstElemCalcolo.Add(new ElementiCalcolo(DatiFondoGAS_ES.MMServizioUtile2.Value, DatiFondoGAS_ES.Retribuzione2.Value));
            }
            if (DatiFondoGAS_ES.Retribuzione3.HasValue && DatiFondoGAS_ES.MMServizioUtile3.HasValue)
            {
                lstElemCalcolo.Add(new ElementiCalcolo(DatiFondoGAS_ES.MMServizioUtile3.Value, DatiFondoGAS_ES.Retribuzione3.Value));
            }
            if (DatiFondoGAS_ES.Retribuzione4.HasValue && DatiFondoGAS_ES.MMServizioUtile4.HasValue)
            {
                lstElemCalcolo.Add(new ElementiCalcolo(DatiFondoGAS_ES.MMServizioUtile4.Value, DatiFondoGAS_ES.Retribuzione4.Value));
            }

        }

        #region Grid Elementi calcolo

        protected void gvElementiCalcolo_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            List<ElementiCalcolo> lstElemCalc = (List<ElementiCalcolo>)ViewState["elementiCalcolo"];
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                lstElemCalc.RemoveAt(r.DataItemIndex);
                if (lstElemCalc.Count == 1)
                {
                    //this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                    //AreaTitolare.DatiStatoCivileTitolare statoCivileCod = new AreaTitolare.DatiStatoCivileTitolare();
                    //statoCivileCod.Codice = this.TitolarePensione.Anagrafica.CodiceStatoCivile.ToString();
                    //AreaDecodifica.DatiStatoCivile seDec = ConvertSave2GvDataSource(statoCivileCod);

                    //RaiseGetDecorrenzaPensione(this, null);
                    //elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;
                    //elencoStatoCivile[0].SCivile = seDec.Descrizione;
                    //elencoStatoCivile[0].CodSCivile = seDec.Id;

                    //RaiseGetDecorrenzaPensione(this, null);
                    //elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;
                    //elencoStatoCivile[0].SCivile = "";
                    //elencoStatoCivile[0].CodSCivile = "";
                    gvElementiDiCalcolo.EditIndex = 0;
                }

                ViewState["elementiCalcolo"] = lstElemCalc;
                GvElementiCalcolo_Load();

                //if (btnSalva.Enabled == false)
                //    btnSalva.Enabled = true;

                //RaiseAbilitaTastoSalva(this, null);
            }
            else if (e.CommandName == "Edit")
            {
                //if (elencoStatoCivile[0].Decorrenza == "")
                //{
                //    RaiseGetDecorrenzaPensione(this, null);
                //    elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;

                //}
                //if (btnSalva.Enabled == true)
                //    btnSalva.Enabled = false;
                //RaiseDisabilitaTastoSalva(this, null);

            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                int idxRow = r.RowIndex;

                string sMeseUtile = ((TextBox)(r.Cells[1].Controls[1])).Text;
                string sRetrib = ((TextBox)(r.Cells[2].Controls[1])).Text;
                int meseUtile = int.Parse(sMeseUtile);
                decimal retrib = decimal.Parse(sRetrib);
                ElementiCalcolo elemCalcolo = new ElementiCalcolo(meseUtile, retrib);

                if (idxRow == lstElemCalc.Count - 1)
                {
                    //elem nuovo
                    lstElemCalc.Insert(lstElemCalc.Count - 1, elemCalcolo);
                }
                else
                {
                    //edit elemento esistente
                    lstElemCalc.RemoveAt(idxRow);
                    lstElemCalc.Insert(idxRow, elemCalcolo);
                }
                ViewState["elementiCalcolo"] = lstElemCalc;
                gvElementiDiCalcolo.EditIndex = -1;
                GvElementiCalcolo_Load();
            }
            else if (e.CommandName == "Cancel")
            {
                if (lstElemCalc.Count == 1)
                {
                    gvElementiDiCalcolo.EditIndex = 0;
                }
                else
                    gvElementiDiCalcolo.EditIndex = -1;
                GvElementiCalcolo_Load();
            }
        }

        protected void gvElementiCalcolo_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //try
            //{
            //    List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];

            //    //Reset the edit index.
            //    //btnAnnulla.Visible = false;
            //    //Bind data to the GridView control.
            //    GvStatoCivile_Load();

            //    if (elencoStatoCivile.Count == 1)
            //    {
            //        btnSalva.Enabled = false;
            //        RaiseDisabilitaTastoSalva(this, null);
            //    }
            //    else
            //    {
            //        btnSalva.Enabled = true;
            //        RaiseAbilitaTastoSalva(this, null);
            //    }

            //    //if (btnSalva.Enabled == false)
            //    //    btnSalva.Enabled = true;
            //    ////RaiseAnnullaStatoCivile(this, null);
            //    //RaiseAbilitaTastoSalva(this, null);

            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowCancelingEdit " + ex);
            //}

        }

        protected void gvElementiCalcolo_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvElementiDiCalcolo.EditIndex = e.NewEditIndex;
                GvElementiCalcolo_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowEditing " + ex);
            }
        }

        protected void gvElementiCalcolo_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //try
            //{
            //    List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
            //    GridViewRow row = gvStatoCivile.Rows[e.RowIndex];
            //    if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
            //    {
            //        int i = ((gvStatoCivile.PageIndex * 10) + e.RowIndex);

            //        if (elencoStatoCivile.Count != i + 1)
            //            elencoStatoCivile.RemoveAt(elencoStatoCivile.Count - 1);
            //        gvStatoCivile.EditIndex = -1;
            //        ViewState["elencoStatoCivile"] = elencoStatoCivile;
            //        GvStatoCivile_Load();
            //        //RaiseAnnullaStatoCivile(this, null);
            //    }
            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            //}
        }

        protected void gvElementiCalcolo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<ElementiCalcolo> elencoStatoCivile = (List<ElementiCalcolo>)ViewState["elementiCalcolo"];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCDatiFondoGAS_ES_GvElementiCalcolo";
                        save.CommandName = "Salva";

                        //DropDownList ddlSC = new DropDownList();
                        //ddlSC = (DropDownList)e.Row.FindControl("ddlStatoCivile");
                        //LoadDdl(ddlSC);
                        //TextBox txtDecorrenza = (TextBox)e.Row.FindControl("txtDecorrenzaStatoCivile");

                        //ddlSC.SelectedValue = ((StatoCivile)e.Row.DataItem).CodSCivile;

                        //if (btnSalvaDatiFondo.Enabled == true)
                        //    btnSalvaDatiFondo.Enabled = false;
                        // RaiseDisabilitaTastoSalva(this, null);

                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoStatoCivile.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            LinkButton delete = (LinkButton)e.Row.FindControl("btnDelete");
                            int index = e.Row.DataItemIndex;
                            if (index >= 0 && index <= elencoStatoCivile.Count - 2)
                            {
                                edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                edit.ToolTip = "Modifica";

                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
                            }
                        }
                    }
                }
                if (e.Row.RowIndex >= 4)
                {
                    e.Row.Visible = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowDataBound " + ex);
            }
        }

        protected void gvElementiCalcolo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<ElementiCalcolo> elencoStatoCivile = (List<ElementiCalcolo>)ViewState["elementiCalcolo"];
                if (elencoStatoCivile.Count < 1)
                    inserisciElementoCalcolo();
                GvElementiCalcolo_Load();
                //RaiseAnnullaStatoCivile(this, null);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowDeleting " + ex);
            }
        }

        protected void gvElementiCalcolo_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            //try
            //{
            //    gvStatoCivile.PageIndex = e.NewPageIndex;
            //    GvStatoCivile_Load();
            //}
            //catch (DnaExceptionBase)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_onPageIndexChanging" + ex);
            //}
        }

        private void GvElementiCalcolo_Load()
        {
            try
            {
                List<ElementiCalcolo> elencoElementiCalcolo = (List<ElementiCalcolo>)ViewState["elementiCalcolo"];
                gvElementiDiCalcolo.DataSource = elencoElementiCalcolo;
                //gvElementiDiCalcolo.DataKeyNames = new string[] { "CodSCivile" };
                gvElementiDiCalcolo.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo GvStatoCivile_Load " + ex);
            }
        }

        private void inserisciElementoCalcolo()
        {
            try
            {
                List<ElementiCalcolo> elencoStatoCivile = (List<ElementiCalcolo>)ViewState["elementiCalcolo"];
                AddItemBlank(ref elencoStatoCivile);
                ViewState["elementiCalcolo"] = (List<ElementiCalcolo>)elencoStatoCivile;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo InserisciStatoCivile " + ex);
            }
        }

        private void AddItemBlank(ref List<ElementiCalcolo> elencoStatoCivile)
        {
            int index = elencoStatoCivile.FindIndex(
                delegate (ElementiCalcolo code)
                {
                    return (!code.MesiServizioUtile.HasValue && !code.Retribuzione.HasValue);
                });

            if (index < 0)
                elencoStatoCivile.Add(new ElementiCalcolo(null, null));
        }

        private void RenderControls()
        {

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    pnlFondoES.Visible = true;
                    break;
            }
        }

        #endregion Grid Elementi calcolo


        #region Event

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion Event


        #region Nestled class
        [Serializable]
        public class ElementiCalcolo
        {
            public int? MesiServizioUtile { get; set; }
            public decimal? Retribuzione { get; set; }

            public ElementiCalcolo(int? mesiServizioUtile, decimal? retribuzione)
            {


                this.MesiServizioUtile = mesiServizioUtile;
                this.Retribuzione = retribuzione;
            }
        }

        #endregion Nestled class


    }
}