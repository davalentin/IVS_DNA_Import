using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Titolare
{
    public partial class UCStatoCivile : CustomBaseUserControl, ITitolarePensione
    {

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //if (ViewState["TitolarePensione"] == null)
                //    ViewState["TitolarePensione"] = this.TitolarePensione;
                //if (ViewState["elencoStatoCivile"] == null)
                //{
                //    List<AreaTitolare.DatiStatoCivileTitolare> elencoStatoCivileCod = this.TitolarePensione.ElencoStatiCiviliTitolare.ToList();
                //    List<StatoCivile> elencoStatoCivile = new List<StatoCivile>();
                //    if (elencoStatoCivileCod.Count == 0) // caso di dati da WEBDOM con stato civile pari a 'non definito' o 'non specificato'
                //    {
                //        AreaTitolare.DatiStatoCivileTitolare statoCivileCod = new AreaTitolare.DatiStatoCivileTitolare();
                //        statoCivileCod.Codice = string.Empty;
                //        statoCivileCod.Decorrenza = TitolarePensione.Pensione.DecorrenzaOriginaria;

                //        StatoCivile re = new StatoCivile();
                //        re.CodSCivile = string.Empty;
                //        re.SCivile = string.Empty;
                //        if (!string.IsNullOrEmpty(statoCivileCod.Decorrenza.ToString()))
                //            re.Decorrenza = String.Format("{0:MM/yyyy}", statoCivileCod.Decorrenza);
                //        else
                //            re.Decorrenza = string.Empty;
                //        elencoStatoCivile.Add(re);
                //    }
                //    else
                //    {
                //        foreach (AreaTitolare.DatiStatoCivileTitolare statoCivileCod in elencoStatoCivileCod)
                //        {
                //            AreaDecodifica.DatiStatoCivile seDec = ConvertSave2GvDataSource(statoCivileCod);
                //            StatoCivile re = new StatoCivile();

                //            re.CodSCivile = seDec.Id;
                //            re.SCivile = seDec.Descrizione;
                //            if (!string.IsNullOrEmpty(statoCivileCod.Decorrenza.ToString()))
                //                re.Decorrenza = String.Format("{0:MM/yyyy}", statoCivileCod.Decorrenza);
                //            else
                //                re.Decorrenza = string.Empty;
                //            elencoStatoCivile.Add(re);
                //        }
                //    }

                //    ViewState["elencoStatoCivile"] = elencoStatoCivile;
                //    if (elencoStatoCivileCod.Count == 0) 
                //        gvStatoCivile.EditIndex = 0;
                //    else
                //        inserisciStatoCivile();
                //}

                //if ((AreaQuadri)Session["Semaforo"] != null && ((AreaQuadri)Session["Semaforo"]).QuadroTitolare.TabStatiCivili == AreaQuadri.Semaforo.Rosso_Abilitato &&
                //    ((List<StatoCivile>)ViewState["elencoStatoCivile"]) != null && ((List<StatoCivile>)ViewState["elencoStatoCivile"]).Count == 2 &&
                //    this.TitolarePensione.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione)
                //    gvStatoCivile.EditIndex = 0;

                //GvStatoCivile_Load();
            }
        }

        internal void ValorizzaEtichette()
        {
            ValorizzaViewStateFromService();

            List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];

            if (elencoStatoCivile.Count == 0)
            {
                AreaDecodifica.DatiStatoCivile seDec = null;
                if (this.TitolarePensione.Anagrafica.CodiceStatoCivile.HasValue)
                {
                    AreaTitolare.DatiStatoCivileTitolare statoCivileCod = new AreaTitolare.DatiStatoCivileTitolare();
                    statoCivileCod.Codice = this.TitolarePensione.Anagrafica.CodiceStatoCivile.Value;
                    seDec = ConvertSave2GvDataSource(statoCivileCod);
                }

                StatoCivile statoCivile = new StatoCivile();
                statoCivile.CodSCivile = seDec != null ? seDec.Id.ToString() : string.Empty;
                statoCivile.SCivile = seDec != null ? seDec.Descrizione : string.Empty;
                statoCivile.Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;

                gvStatoCivile.EditIndex = 0;

                elencoStatoCivile.Add(statoCivile);

                ViewState["elencoStatoCivile"] = elencoStatoCivile;
            }
            else
            {
                RemoveItemBlank(ref elencoStatoCivile);

                if (elencoStatoCivile.Count == 1 && elencoStatoCivile[0].Decorrenza == string.Empty)
                    gvStatoCivile.EditIndex = 0;
                else
                {
                    inserisciStatoCivile();
                }
            }

            GvStatoCivile_Load();
        }

        internal AreaTitolare GetDatiUcStatoCivile()
        {
            try
            {
                List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
                List<AreaTitolare.DatiStatoCivileTitolare> elencoStatoCivileCod = new List<AreaTitolare.DatiStatoCivileTitolare>();
                foreach (StatoCivile statoCivile in elencoStatoCivile)
                {
                    if (!string.IsNullOrEmpty(statoCivile.CodSCivile))
                    {
                        AreaDecodifica.DatiStatoCivile reDecod = ConvertGv2SaveDataSource(statoCivile);
                        AreaTitolare.DatiStatoCivileTitolare reCod = new AreaTitolare.DatiStatoCivileTitolare();
                        reCod.Codice = reDecod.Id;
                        reCod.Decorrenza = Utility.GetDateFromString(statoCivile.Decorrenza);
                        elencoStatoCivileCod.Add(reCod);
                    }
                }

                elencoStatoCivileCod.Sort(delegate (AreaTitolare.DatiStatoCivileTitolare c1, AreaTitolare.DatiStatoCivileTitolare c2)
                {
                    if (c1.Decorrenza.HasValue && c2.Decorrenza.HasValue)
                        return c1.Decorrenza.Value.CompareTo(c2.Decorrenza);
                    else
                    {
                        if (c1.Decorrenza.HasValue && !c2.Decorrenza.HasValue)
                            return 1;
                        else if (!c1.Decorrenza.HasValue && c2.Decorrenza.HasValue)
                            return -1;
                        else
                            return 0;
                    }

                });

                if (this.TitolarePensione == null)
                    this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                this.TitolarePensione.ElencoStatiCiviliTitolare = elencoStatoCivileCod.ToArray();
                this.TitolarePensione.Pensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

                return this.TitolarePensione;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo GetDatiUcStatoCivile " + ex);
            }
        }

        internal void UpdateViewState(ITitolarePensione titolare)
        {
            ViewState["TitolarePensione"] = titolare.TitolarePensione;
        }

        #region Grid Stato Civile

        protected void gvStatoCivile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Boolean okDecorrenza;
            Boolean okStatoCivile;
            List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                elencoStatoCivile.RemoveAt(r.DataItemIndex);
                if (elencoStatoCivile.Count == 1)
                {
                    this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                    AreaTitolare.DatiStatoCivileTitolare statoCivileCod = new AreaTitolare.DatiStatoCivileTitolare();
                    statoCivileCod.Codice = this.TitolarePensione.Anagrafica.CodiceStatoCivile.HasValue ? this.TitolarePensione.Anagrafica.CodiceStatoCivile.Value : ' ';
                    AreaDecodifica.DatiStatoCivile seDec = ConvertSave2GvDataSource(statoCivileCod);

                    RaiseGetDecorrenzaPensione(this, null);
                    elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;
                    if (seDec != null)
                    {
                        elencoStatoCivile[0].SCivile = seDec.Descrizione;
                        elencoStatoCivile[0].CodSCivile = seDec.Id.ToString();
                    }

                    //RaiseGetDecorrenzaPensione(this, null);
                    //elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;
                    //elencoStatoCivile[0].SCivile = "";
                    //elencoStatoCivile[0].CodSCivile = "";
                    gvStatoCivile.EditIndex = 0;
                }

                ViewState["elencoStatoCivile"] = elencoStatoCivile;
                GvStatoCivile_Load();

                if (btnSalva.Enabled == false)
                    btnSalva.Enabled = true;
            }
            else if (e.CommandName == "Edit")
            {
                if (elencoStatoCivile[0].Decorrenza == "" || elencoStatoCivile[0].Decorrenza == null)
                {
                    RaiseGetDecorrenzaPensione(this, null);
                    elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;

                }
                if (btnSalva.Enabled == true)
                    btnSalva.Enabled = false;

            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                if ((String.IsNullOrEmpty(((TextBox)(r.Cells[1].Controls[1])).Text)) || ((((TextBox)(r.Cells[1].Controls[1])).Text) == "MM/AAAA"))
                    okDecorrenza = false;
                else
                    okDecorrenza = true;

                if ((String.IsNullOrEmpty(((DropDownList)(r.Cells[2].Controls[1])).Text)))
                    okStatoCivile = false;
                else
                    okStatoCivile = true;

                if (okDecorrenza && okStatoCivile)
                {
                    if (btnSalva.Enabled == false)
                        btnSalva.Enabled = true;

                    if ((r.DataItemIndex - 1) == (elencoStatoCivile.Count - 2))    //aggiunta riga (non si tratta di una modifica)
                    {
                        StatoCivile sc = new StatoCivile();
                        sc.Decorrenza = "";
                        sc.SCivile = "";
                        sc.CodSCivile = "";
                        elencoStatoCivile.Add(sc);
                    }

                    elencoStatoCivile[r.DataItemIndex].Decorrenza = ((TextBox)(r.Cells[1].Controls[1])).Text;
                    elencoStatoCivile[r.DataItemIndex].SCivile = ((DropDownList)(r.Cells[2].Controls[1])).SelectedItem.Text;
                    elencoStatoCivile[r.DataItemIndex].CodSCivile = ((DropDownList)(r.Cells[2].Controls[1])).SelectedValue + string.Empty;
                    gvStatoCivile.EditIndex = -1;

                    RaiseSalvaStatoCivile(this, null);

                }
                else
                {
                    RaiseErrorSalvaStatoCivile(this, null);
                }
                ViewState["elencoStatoCivile"] = elencoStatoCivile;
                GvStatoCivile_Load();
            }
            else if (e.CommandName == "Cancel")
            {
                if (elencoStatoCivile.Count == 1)
                {
                    RaiseGetDecorrenzaPensione(this, null);
                    elencoStatoCivile[0].Decorrenza = hdn_txtDecorrenzaPensioneSC.Value;
                    elencoStatoCivile[0].SCivile = "";
                    elencoStatoCivile[0].CodSCivile = "";
                    gvStatoCivile.EditIndex = 0;
                }
                else
                    gvStatoCivile.EditIndex = -1;
            }
        }

        protected void gvStatoCivile_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];

                //Reset the edit index.
                //btnAnnulla.Visible = false;
                //Bind data to the GridView control.
                GvStatoCivile_Load();

                if (elencoStatoCivile.Count == 1)
                    btnSalva.Enabled = false;
                else
                    btnSalva.Enabled = true;

                //if (btnSalva.Enabled == false)
                //    btnSalva.Enabled = true;
                ////RaiseAnnullaStatoCivile(this, null);
                //RaiseAbilitaTastoSalva(this, null);

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowCancelingEdit " + ex);
            }

        }

        protected void gvStatoCivile_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvStatoCivile.EditIndex = e.NewEditIndex;
                GvStatoCivile_Load();
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

        protected void gvStatoCivile_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
                GridViewRow row = gvStatoCivile.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvStatoCivile.PageIndex * 10) + e.RowIndex);

                    if (elencoStatoCivile.Count != i + 1)
                        elencoStatoCivile.RemoveAt(elencoStatoCivile.Count - 1);
                    gvStatoCivile.EditIndex = -1;
                    ViewState["elencoStatoCivile"] = elencoStatoCivile;
                    GvStatoCivile_Load();
                    //RaiseAnnullaStatoCivile(this, null);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            }
        }

        protected void gvStatoCivile_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        if (this.domanda == null)
                            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                        if (this.TitolarePensione == null)
                            this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];

                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCTabStatoCivile";
                        save.CommandName = "Salva";

                        DropDownList ddlSC = new DropDownList();
                        ddlSC = (DropDownList)e.Row.FindControl("ddlStatoCivile");
                        LoadDdl(ddlSC);
                        TextBox txtDecorrenza = (TextBox)e.Row.FindControl("txtDecorrenzaStatoCivile");

                        ddlSC.SelectedValue = ((StatoCivile)e.Row.DataItem).CodSCivile;

                        //if (!string.IsNullOrEmpty(this.domanda.Categoria) && this.domanda.Categoria.StartsWith("S") && this.TitolarePensione != null && this.TitolarePensione.IsContitolareConiuge)
                        //{
                        //    ddlSC.SelectedValue = "3";
                        //    ddlSC.Enabled = false;
                        //}

                        if (btnSalva.Enabled == true)
                            btnSalva.Enabled = false;
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

        protected void gvStatoCivile_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
                if (elencoStatoCivile.Count < 1)
                    inserisciStatoCivile();
                GvStatoCivile_Load();
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

        protected void gvStatoCivile_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvStatoCivile.PageIndex = e.NewPageIndex;
                GvStatoCivile_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_onPageIndexChanging" + ex);
            }
        }

        #endregion Grid Stato Civile

        #region Events

        public event EventHandler SalvaStatoCivile;
        public event EventHandler ErrorSalvaStatoCivile;
        public event EventHandler GetDecorrenzaPensione;
        public event EventHandler ShowAvvisoStatoCivile;

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            PresenterTitolare presenterTitolare = new PresenterTitolare();

            this.TitolarePensione = this.GetDatiUcStatoCivile();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            presenterTitolare.SalvaDatiTabStatoCivile(this);

            RaiseShowAvvisoStatoCivile(this, null);
        }

        protected void RaiseSalvaStatoCivile(object sender, EventArgs e)
        {
            if (SalvaStatoCivile != null)
                SalvaStatoCivile(sender, e);
        }

        protected void RaiseErrorSalvaStatoCivile(object sender, EventArgs e)
        {
            if (ErrorSalvaStatoCivile != null)
                ErrorSalvaStatoCivile(sender, e);
        }

        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        protected void RaiseShowAvvisoStatoCivile(object sender, EventArgs e)
        {
            ShowAvvisoStatoCivile(sender, e);
        }

        #endregion Events

        #region Privete Methods

        private void ValorizzaViewStateFromService()
        {
            if (ViewState["TitolarePensione"] == null)
                ViewState["TitolarePensione"] = this.TitolarePensione;

            if (ViewState["elencoStatoCivile"] == null)
            {
                List<AreaTitolare.DatiStatoCivileTitolare> elencoStatoCivileCod = this.TitolarePensione.ElencoStatiCiviliTitolare.ToList();
                List<StatoCivile> elencoStatoCivile = new List<StatoCivile>();

                foreach (AreaTitolare.DatiStatoCivileTitolare statoCivileService in elencoStatoCivileCod)
                {
                    elencoStatoCivile.Add(ValorizzaStatoCivile(statoCivileService));
                }

                ViewState["elencoStatoCivile"] = elencoStatoCivile;
            }
        }

        private StatoCivile ValorizzaStatoCivile(AreaTitolare.DatiStatoCivileTitolare statoCivileService)
        {
            AreaTitolare.DatiStatoCivileTitolare statoCivileCod = new AreaTitolare.DatiStatoCivileTitolare();
            statoCivileCod.Codice = statoCivileService.Codice;
            AreaDecodifica.DatiStatoCivile seDec = ConvertSave2GvDataSource(statoCivileCod);

            StatoCivile statoCivile = new StatoCivile();
            statoCivile.CodSCivile = seDec.Id.ToString();
            statoCivile.SCivile = seDec.Descrizione;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (!(this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && (Utility.IsDomandaRipristinoOrRiliquidazioneSuperstiti(this.TitolarePensione.Pensione) && (this.TitolarePensione.Pensione.CodeTipo == "0026" || this.TitolarePensione.Pensione.CodeTipo == "0027"))))
            {
                statoCivile.Decorrenza = string.Format("{0:MM/yyyy}", statoCivileService.Decorrenza);
            }
                

            return statoCivile;
        }

        private void inserisciStatoCivile()
        {
            try
            {
                List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
                AddItemBlank(ref elencoStatoCivile);
                ViewState["elencoStatoCivile"] = (List<StatoCivile>)elencoStatoCivile;
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

        private AreaDecodifica.DatiStatoCivile ConvertSave2GvDataSource(AreaTitolare.DatiStatoCivileTitolare statoCivileCod)
        {
            try
            {
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoCivile[] listStatoCivile = areaDecodifica.GetValuesDecodifica().ElencoStatiCivili;
                AreaDecodifica.DatiStatoCivile seDec = listStatoCivile.ToList().Find(
                    delegate (AreaDecodifica.DatiStatoCivile statoCivile)
                    {
                        return statoCivile.Id == statoCivileCod.Codice;
                    }
                    );
                return seDec;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo ConvertSave2GvDataSource " + ex);
            }
        }

        private void GvStatoCivile_Load()
        {
            try
            {
                List<StatoCivile> elencoStatoCivile = (List<StatoCivile>)ViewState["elencoStatoCivile"];
                gvStatoCivile.DataSource = elencoStatoCivile;
                gvStatoCivile.DataKeyNames = new string[] { "CodSCivile" };
                gvStatoCivile.DataBind();
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

        private void LoadDdl(DropDownList ddlSC)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.TitolarePensione == null)
                    this.TitolarePensione = (AreaTitolare)ViewState["TitolarePensione"];
                this.TitolarePensione.Pensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                ListItem li = new ListItem();
                li.Text = "";
                li.Value = "";
                ddlSC.Items.Add(li);
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoCivile[] listStatiCivile = areaDecodifica.GetValuesDecodifica().ElencoStatiCivili;
                string certificato = string.Empty;
                certificato = this.domanda.CertificatoPensione != null ? this.domanda.CertificatoPensione.ToString().PadLeft(8, '0') : "00000000";

                foreach (AreaDecodifica.DatiStatoCivile statoCivile in listStatiCivile)
                {
                    if ((!string.IsNullOrEmpty(this.domanda.Categoria) && Utility.IsDomandaPSO(this.domanda.Categoria)) && ((Utility.IsDomandaPL(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && (this.TitolarePensione.Pensione.CodeGruppo == "0005" && this.TitolarePensione.Pensione.CodeProdotto == "0043" && (this.TitolarePensione.Pensione.CodeTipo == "0014" || this.TitolarePensione.Pensione.CodeTipo == "0015"))) || (Utility.IsRicostituzione(this.TitolarePensione.Pensione.CodeGruppo) && (certificato.Substring(2, 1) == "3" || certificato.Substring(2, 1) == "6"))))

                    {

                        if (statoCivile.Id == '1' || statoCivile.Id == '3' || statoCivile.Id == 'C')
                        {
                            li = new ListItem();
                            li.Attributes.Add("title", statoCivile.Descrizione);
                            li.Text = statoCivile.Descrizione;
                            li.Value = statoCivile.Id.ToString();
                            ddlSC.Items.Add(li);
                        }
                    }
                    else
                    {
                        //dirette
                        if (!string.IsNullOrEmpty(this.domanda.Categoria) && !this.domanda.Categoria.StartsWith("S"))
                        {
                            li = new ListItem();
                            li.Attributes.Add("title", statoCivile.Descrizione);
                            li.Text = statoCivile.Descrizione;
                            li.Value = statoCivile.Id.ToString();
                            ddlSC.Items.Add(li);
                        }
                        else //superstiti
                        {
                            //caso coniuge del dante causa, può essere solo vedovo o vedovo dall'unione civile
                            if (this.TitolarePensione != null && this.TitolarePensione.IsContitolareConiuge)
                            {
                                if (statoCivile.Id == '3' || statoCivile.Id == 'C')
                                {
                                    li = new ListItem();
                                    li.Attributes.Add("title", statoCivile.Descrizione);
                                    li.Text = statoCivile.Descrizione;
                                    li.Value = statoCivile.Id.ToString();
                                    ddlSC.Items.Add(li);
                                }
                            }

                            else if (this.TitolarePensione != null && this.TitolarePensione.IsContitolareExConiuge)
                            {
                                if (statoCivile.Id == '5' || statoCivile.Id == '8')
                                {
                                    li = new ListItem();
                                    li.Attributes.Add("title", statoCivile.Descrizione);
                                    li.Text = statoCivile.Descrizione;
                                    li.Value = statoCivile.Id.ToString();
                                    ddlSC.Items.Add(li);
                                }
                            }

                            //caso ascendente(genitore) del dante causa, può essere celibe/nubile, vedovo, vedovo dall'unione civile, coniugato, unito civilmente (segnalazione 25/01/2017)
                            else if (this.TitolarePensione != null && this.TitolarePensione.IsContitolareAscendente)
                            {
                                if (statoCivile.Id == '1' || statoCivile.Id == '3' || statoCivile.Id == 'C' || statoCivile.Id == '2' || statoCivile.Id == '7')
                                {
                                    li = new ListItem();
                                    li.Attributes.Add("title", statoCivile.Descrizione);
                                    li.Text = statoCivile.Descrizione;
                                    li.Value = statoCivile.Id.ToString();
                                    ddlSC.Items.Add(li);
                                }
                            }

                            else
                            {
                                //familiari altri casi, può essere celibe/nubile, vedovo, vedovo dall'unione civile
                                if (statoCivile.Id == '1' || statoCivile.Id == '3' || statoCivile.Id == 'C')
                                {
                                    li = new ListItem();
                                    li.Attributes.Add("title", statoCivile.Descrizione);
                                    li.Text = statoCivile.Descrizione;
                                    li.Value = statoCivile.Id.ToString();
                                    ddlSC.Items.Add(li);
                                }
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
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo LoadDdl " + ex);
            }

        }

        private AreaDecodifica.DatiStatoCivile ConvertGv2SaveDataSource(StatoCivile re)
        {
            try
            {
                CodeUtility areaDecodifica = new CodeUtility();
                Presenter.SvrLiquidazione.AreaDecodifica.DatiStatoCivile[] listStatoCivile = areaDecodifica.GetValuesDecodifica().ElencoStatiCivili;
                AreaDecodifica.DatiStatoCivile reDec = listStatoCivile.ToList().Find(
                    delegate (AreaDecodifica.DatiStatoCivile statoCivile)
                    {
                        return statoCivile.Id == re.CodSCivile[0];
                    }
                    );
                return reDec;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo ConvertGv2SaveDataSource " + ex);
            }
        }

        private void AddItemBlank(ref List<StatoCivile> elencoStatoCivile)
        {
            int index = elencoStatoCivile.FindIndex(delegate (StatoCivile code)
            { return (string.IsNullOrEmpty(code.CodSCivile) && string.IsNullOrEmpty(code.SCivile) && string.IsNullOrEmpty(code.Decorrenza)); });

            if (index < 0)
                elencoStatoCivile.Add(new StatoCivile());
        }

        private void RemoveItemBlank(ref List<StatoCivile> elencoStatoCivile)
        {
            if (elencoStatoCivile != null && elencoStatoCivile.Count() > 0)
            {
                int index = elencoStatoCivile.FindIndex(delegate (StatoCivile code)
                { return (string.IsNullOrEmpty(code.CodSCivile) && string.IsNullOrEmpty(code.SCivile) && string.IsNullOrEmpty(code.Decorrenza)); });

                if (index >= 0)
                    elencoStatoCivile.RemoveAt(index);
            }
        }

        #endregion Privete Methods
    }

    [Serializable]
    public class StatoCivile
    {
        public string Decorrenza { get; set; }
        public string SCivile { get; set; }
        public string CodSCivile { get; set; }
    }
}

