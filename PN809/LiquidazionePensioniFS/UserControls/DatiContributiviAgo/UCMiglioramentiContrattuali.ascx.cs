using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCMiglioramentiContrattuali : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione
    {
        public string ValidationGroupname { get; set; }

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion IDatiContributiviAgo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region View State Variables

        /// <summary>
        /// Lista quote pensione contenuta nel ViewState
        /// </summary>
        private List<MiglioramentiContrattualiLocal> VS_ElencoMiglioramentiContrattuali
        {
            get { return (List<MiglioramentiContrattualiLocal>)ViewState["elencoMiglioramentiContrattuali"]; }
            set { ViewState["elencoMiglioramentiContrattuali"] = value; }

        }

        private List<MiglioramentiContrattualiLocal> VS_ElencoMiglioramentiContrattualiInizio
        {
            get { return (List<MiglioramentiContrattualiLocal>)ViewState["elencoMiglioramentiContrattualiInizio"]; }
            set { ViewState["elencoMiglioramentiContrattualiInizio"] = value; }

        }
        /// <summary>
        /// Decodifica Quote
        /// </summary>
        private List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo> VS_DecEnteGestoneFondo
        {
            get { return (List<Presenter.SvrLiquidazioneAgo.DecEnteGestioneFondo>)ViewState["decodificaEnteGestioneFondo"]; }
            set { ViewState["decodificaEnteGestioneFondo"] = value; }

        }

        /// <summary>
        /// Decodifica Trattenute
        /// </summary>
        private List<DecCodiceTrattenute> VS_DecCodiceTrattenute
        {
            get { return (List<DecCodiceTrattenute>)ViewState["decodificaCodiceTrattenute"]; }
            set { ViewState["decodificaCodiceTrattenute"] = value; }
        }


        private bool VS_IsRicostituzione
        {
            get { return (bool)ViewState["IsRicostituzione"]; }
            set { ViewState["IsRicostituzione"] = value; }
        }

        private bool VS_IsModalitaEdit
        {
            get { return (bool)ViewState["IsModalitaEdit"]; }
            set { ViewState["IsModalitaEdit"] = value; }
        }

        private AreaDatiContributivi VS_AreaDatiContributivi
        {
            get { return (AreaDatiContributivi)ViewState["AreaDatiContributivi"]; }
            set { ViewState["AreaDatiContributivi"] = value; }
        }

        private bool VS_IsScaricoTrattenuteCumulo
        {
            get { return (bool)ViewState["IsScaricoTrattenuteCumulo"]; }
            set { ViewState["IsScaricoTrattenuteCumulo"] = value; }
        }

        #endregion View State Variables

        #region Web Form Events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void btnSalvaMiglioramentiContrattuali_Click(object sender, EventArgs args)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            this.areaDatiContributiviAgo = new AreaDatiContributivi();
            RecuperaCampi(this.areaDatiContributiviAgo);
            PresenterDatiContributiviAGO presenter = new PresenterDatiContributiviAGO();
            presenter.SalvaDatiMiglioramenti(this);

            // serve per il salvataggio completo (rivisitabile)
            try
            {
                ((Web.DatiContributiviAgo)sender).HasError = this.HasError;
                ((Web.DatiContributiviAgo)sender).ErrorMessage = this.ErrorMessage;
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
            //if (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica)
            //    btnEliminaMiglioramentiContrattuali.Enabled = true;
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        public void btnEliminaMiglioramentiContrattuali_Click(object sender, EventArgs args)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            presenterDatiContributiviAgo.EliminaDatiCalcolo(this);

            if (!this.HasError)
            {
                RaiseInitializeData(sender, args);
            }
            else
                this.ErrorMessage = "Errore durante la la cancellazione delle Quote Pensione";
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        internal void DisabilitaPulsanti()
        {
            btnSalvaMiglioramentiContrattuali.Enabled = false;
            btnEliminaMiglioramentiContrattuali.Enabled = false;
        }
        #endregion Web Form Events

        #region Grid View Quote Pensione
        protected void gvMiglioramentiContrattuali_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gvMiglioramentiContrattuali = (GridView)sender;
                gvMiglioramentiContrattuali.EditIndex = e.NewEditIndex;
                VS_ElencoMiglioramentiContrattuali.RemoveAll(x => x.IsEmpty());
                VS_IsModalitaEdit = true;
                gvMiglioramentiContrattuali.DataSource = VS_ElencoMiglioramentiContrattuali;
                gvMiglioramentiContrattuali.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCMiglioramentiContrattuali, Errore nel metodo gvMiglioramentiContrattuali_RowEditing " + ex);
            }
        }

        protected void gvMiglioramentiContrattuali_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvMiglioramentiContrattuali = (GridView)sender;
                GridViewRow row = gvMiglioramentiContrattuali.Rows[e.RowIndex];
                MiglioramentiContrattualiLocal quotaPensione = VS_ElencoMiglioramentiContrattuali[row.DataItemIndex];

                quotaPensione.Codice = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedItem.Text;

                decimal d = 0;
                decimal.TryParse(((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text, out d);
                quotaPensione.Quota = string.Format("{0:F7}", d);

                if (gvMiglioramentiContrattuali.Columns[GVMiglioramentiContrattualiColumn.Decorrenza.GetHashCode()].Visible)
                    quotaPensione.DataDecorrenza = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtDecorrenzaQuota))).Text;


                VS_ElencoMiglioramentiContrattuali[e.RowIndex] = quotaPensione;
                gvMiglioramentiContrattuali.EditIndex = -1;
                VS_IsModalitaEdit = false;
                gvMiglioramentiContrattuali.DataSource = VS_ElencoMiglioramentiContrattuali;
                gvMiglioramentiContrattuali.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvMiglioramentiContrattuali_RowUpdating " + ex);
            }
        }

        protected void gvMiglioramentiContrattuali_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridView gvMiglioramentiContrattuali = (GridView)sender;
                VS_ElencoMiglioramentiContrattuali.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoMiglioramentiContrattuali.Count == 0)
                {
                    VS_ElencoMiglioramentiContrattuali.Add(MiglioramentiContrattualiLocal.GetEmptyQuotaPensione());
                    gvMiglioramentiContrattuali.EditIndex = 0;
                }
                else
                {
                    VS_IsModalitaEdit = false;
                    gvMiglioramentiContrattuali.EditIndex = -1;
                }
                gvMiglioramentiContrattuali.DataSource = VS_ElencoMiglioramentiContrattuali;
                gvMiglioramentiContrattuali.DataBind();
                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCMiglioramentiContrattuali, Errore nel metodo gvMiglioramentiContrattuali_RowCancelingEdit/ " + ex);
            }
        }

        protected void gvMiglioramentiContrattuali_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                RaiseHideAvviso(this, null);
                if (e.CommandName == "Aggiungi")
                {
                    GridView gvMiglioramentiContrattuali = (GridView)sender;
                    VS_ElencoMiglioramentiContrattuali.Add(MiglioramentiContrattualiLocal.GetEmptyQuotaPensione());
                    VS_IsModalitaEdit = true;
                    gvMiglioramentiContrattuali.EditIndex = VS_ElencoMiglioramentiContrattuali.Count - 1;
                    gvMiglioramentiContrattuali.DataSource = VS_ElencoMiglioramentiContrattuali;
                    gvMiglioramentiContrattuali.DataBind();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvMiglioramentiContrattuali_RowCommand " + ex);
            }
        }

        protected void gvMiglioramentiContrattuali_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvMiglioramentiContrattuali = (GridView)sender;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowIndex == gvMiglioramentiContrattuali.EditIndex)
                    {

                        CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCGvMiglioramentiContrattuali", Page.Theme, false);
                        ((Image)e.Row.FindControl(Utility.GetDescription(EnumControlli.imgVisualizzaTrattenute))).Visible = false;
                    }
                    else
                    {
                        MiglioramentiContrattualiLocal row = VS_ElencoMiglioramentiContrattuali[e.Row.RowIndex];
                        CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[e.Row.Cells.Count - 2], Page.Theme, row.IsEditable, !VS_IsRicostituzione);
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo))).Text = row.Codice;
                        if (!datiPensione.IsDomandaCumuloAutomatica || row.DataDecorrenza != string.Format("{0:dd/MM/yyyy}", new DateTime(9999, 1, 1)))
                            ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblDecorrenzaQuota))).Text = row.DataDecorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblValueDecorrenzaQuota))).Text = row.DataDecorrenza;
                        ((Label)e.Row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota))).Text = row.Quota;

                    }
                }
                else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Visible = !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && gvMiglioramentiContrattuali.EditIndex == -1;
                    if (!datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && e.Row.Visible)
                    {
                        foreach (TableCell cell in e.Row.Cells)
                        {
                            switch (e.Row.Cells.GetCellIndex(cell))
                            {
                                case 1:
                                    LinkButton add = (LinkButton)cell.FindControl("btnAggiungiQuote");
                                    add.Text = "<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                    add.ToolTip = "Aggiungi";
                                    break;
                                case 2:
                                    cell.ColumnSpan = e.Row.Cells.Count - 2;
                                    break;
                                default:
                                    cell.Visible = false;
                                    break;
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
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvMiglioramentiContrattuali_RowDataBound " + ex);
            }
        }

        protected void gvMiglioramentiContrattuali_DataBinding(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GridView gvMiglioramentiContrattuali = (GridView)sender;
            gvMiglioramentiContrattuali.ShowFooter = !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica && !VS_IsRicostituzione && gvMiglioramentiContrattuali.EditIndex == -1;
        }

        protected void gvMiglioramentiContrattuali_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            this.areaDatiContributiviAgo = VS_AreaDatiContributivi;
            GridView gvMiglioramentiContrattuali = (GridView)sender;
            
            gvMiglioramentiContrattuali.Columns[GVMiglioramentiContrattualiColumn.ModificaQuota.GetHashCode()].Visible = (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica);
            //gvMiglioramentiContrattuali.Columns[GVMiglioramentiContrattualiColumn.EliminaQuota.GetHashCode()].Visible = !VS_IsRicostituzione && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica;
            gvMiglioramentiContrattuali.Columns[GVMiglioramentiContrattualiColumn.ModificaQuota.GetHashCode()].Visible = false;
            ManagePulsanti();
        }

        protected void gvMiglioramentiContrattuali_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gvMiglioramentiContrattuali = (GridView)sender;
                VS_ElencoMiglioramentiContrattuali.RemoveAt(e.RowIndex);
                //rimozione di eventuali record vuoti
                VS_ElencoMiglioramentiContrattuali.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoMiglioramentiContrattuali.Count == 0)
                {
                    VS_ElencoMiglioramentiContrattuali.Add(MiglioramentiContrattualiLocal.GetEmptyQuotaPensione());
                    gvMiglioramentiContrattuali.EditIndex = 0;
                    VS_IsModalitaEdit = true;
                }
                else
                    gvMiglioramentiContrattuali.EditIndex = -1;
                gvMiglioramentiContrattuali.DataSource = VS_ElencoMiglioramentiContrattuali;
                gvMiglioramentiContrattuali.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiContributiviAgo, Errore nel metodo gvTrattenute_RowDeleting " + ex);
            }
        }



        private bool IsEmptyEditableRowQuotaPensione(GridViewRow row)
        {
            if (row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.txtDecorrenza)) != null && ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo)) != null && ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedIndex != 0)
                return false;
            else
                return true;
        }

        private bool IsEmptyRedableRowQuotaPensione(GridViewRow row)
        {
            if (row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.lblEnteGestioneFondo))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.lblImportoQuota))).Text != string.Empty &&
                row.FindControl(Utility.GetDescription(EnumControlli.lblDecorrenza)) != null && ((Label)row.FindControl(Utility.GetDescription(EnumControlli.lblDecorrenza))).Text != string.Empty
               )
                return false;
            else
                return true;
        }

        #endregion Grid View Quote Pensione

        #region Private Methods

        internal void RecuperaCampi(AreaDatiContributivi areaDatiContributiviAgo)
        {
            if (areaDatiContributiviAgo.DatiCalcoloQuoteMiglioramentiContrattuali == null)
                areaDatiContributiviAgo.DatiCalcoloQuoteMiglioramentiContrattuali = new GestioneContribDatiCalcoloQuoteMiglioramentiContrattuali();
            areaDatiContributiviAgo.DatiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali = MapLocalToServiceObject(VS_ElencoMiglioramentiContrattuali).ToArray();

        }

        private void ManagePulsanti()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (VS_ElencoMiglioramentiContrattuali != null && VS_ElencoMiglioramentiContrattuali.Where(x => !x.IsEmpty()).Count() > 0 && !VS_IsModalitaEdit)
            {
                btnSalvaMiglioramentiContrattuali.Enabled = true;
                RaiseGestisciTastoSalva(this, null);
            }
            else
            {
                btnSalvaMiglioramentiContrattuali.Enabled = false;
                RaiseGestisciTastoSalva(this, null);
            }


            //if (VS_ElencoMiglioramentiContrattuali != null && VS_ElencoMiglioramentiContrattuali.Where(x => !x.IsEmpty()).Count() > 0 &&
            //    VS_IsRicostituzione == false && !VS_IsModalitaEdit)
            //    btnEliminaMiglioramentiContrattuali.Enabled = true;
            //else
            //    btnEliminaMiglioramentiContrattuali.Enabled = false;

            //if (this.domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S"))
            //    btnEliminaMiglioramentiContrattuali.Enabled = false;

            //if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(this.TitolarePensione.Pensione) || this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica == true ||
            //    this.TitolarePensione.Pensione.IsDomandaTotAutomatica == true || Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            //    btnEliminaMiglioramentiContrattuali.Enabled = false;
        }

        public void ValorizzaEtichette(IDatiContributiviAgo idatiContributivi)
        {
            AreaDatiContributivi areaDatiContrib = idatiContributivi.areaDatiContributiviAgo;
            VS_AreaDatiContributivi = areaDatiContrib;
            VS_DecEnteGestoneFondo = areaDatiContrib.listaDecEnteGestioneFondo != null ? areaDatiContrib.listaDecEnteGestioneFondo.ToList() : null;
            VS_DecCodiceTrattenute = areaDatiContrib.ListaDecCodiceTrattenute != null ? areaDatiContrib.ListaDecCodiceTrattenute.ToList() : null;
            VS_IsScaricoTrattenuteCumulo = areaDatiContributiviAgo.IsScaricoTrattenuteCumulo.GetValueOrDefault();
            SetViewStateIsRicostituzione();
            if (areaDatiContrib.DatiCalcoloQuoteMiglioramentiContrattuali != null && areaDatiContrib.DatiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali != null)
            {
                VS_IsModalitaEdit = false;
                InitGvMiglioramentiContrattuali(areaDatiContrib.DatiCalcoloQuoteMiglioramentiContrattuali.LQuoteMiglioramentiContrattuali.ToList());
            }
            else // dati non presenti
            {
                VS_IsModalitaEdit = true;
                InitGvMiglioramentiContrattuali(null);
            }
            ManagePulsanti();


        }

        private void InitGvMiglioramentiContrattuali(List<GestioneContribDatiQuoteMiglioramentiContrattuali> lstServer)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            List<MiglioramentiContrattualiLocal> lstMiglioramentiContrattuali = new List<MiglioramentiContrattualiLocal>();

            if (lstServer != null && lstServer.Count() > 0)
            {
                lstMiglioramentiContrattuali.AddRange(MapServiceToLocalObject(lstServer));
            }
            if (lstMiglioramentiContrattuali.Count == 0 && !VS_IsRicostituzione && !(Utility.IsDomandaCumulo(this.domanda.Categoria) && datiPensione.IsDomandaCumuloAutomatica) && !(Utility.IsDomandaTotalizzazione(this.domanda.Categoria) && datiPensione.IsDomandaTotAutomatica))
                lstMiglioramentiContrattuali.Add(MiglioramentiContrattualiLocal.GetEmptyQuotaPensione());
            VS_ElencoMiglioramentiContrattuali = lstMiglioramentiContrattuali;
            //View state dove salvo la lista quote al primo caricamento del quadro (non verrà modificata)
            VS_ElencoMiglioramentiContrattualiInizio = lstMiglioramentiContrattuali;
            if (lstMiglioramentiContrattuali.Count == 1 && lstMiglioramentiContrattuali[0].IsEmpty())
                gvMiglioramentiContrattuali.EditIndex = 0;
            gvMiglioramentiContrattuali.DataSource = lstMiglioramentiContrattuali;
            gvMiglioramentiContrattuali.DataBind();
        }


        public List<GestioneContribDatiQuoteMiglioramentiContrattuali> MapLocalToServiceObject(List<MiglioramentiContrattualiLocal> lstLocal)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<GestioneContribDatiQuoteMiglioramentiContrattuali> lstService = new List<GestioneContribDatiQuoteMiglioramentiContrattuali>();
            if (lstLocal != null && lstLocal.Count() > 0)
            {
                foreach (MiglioramentiContrattualiLocal elem in lstLocal)
                {
                    if (!elem.IsEmpty())
                    {
                        GestioneContribDatiQuoteMiglioramentiContrattuali elemService = new GestioneContribDatiQuoteMiglioramentiContrattuali();
                        elemService.Quota = elem.Quota;
                        elemService.Codice = elem.Codice;
                        elemService.DataDecorrenza = elem.DataDecorrenza;

                        lstService.Add(elemService);
                    }
                }
            }
            return lstService;
        }

        public List<MiglioramentiContrattualiLocal> MapServiceToLocalObject(List<GestioneContribDatiQuoteMiglioramentiContrattuali> lstService)
        {
            List<MiglioramentiContrattualiLocal> lstLocal = new List<MiglioramentiContrattualiLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (GestioneContribDatiQuoteMiglioramentiContrattuali elemS in lstService)
                {
                    MiglioramentiContrattualiLocal elemL = new MiglioramentiContrattualiLocal();
                    elemL.Quota = elemS.Quota.ToString();
                    elemL.Codice = elemS.Codice.ToString();
                    elemL.DataDecorrenza = string.Format("{0:dd/MM/yyyy}", elemS.DataDecorrenza);
                    //elemL.IsEditable = IsButtonEditVisible(elemS.IsQuotaProgressiva);

                    lstLocal.Add(elemL);
                }
            }
            return lstLocal;
        }

        internal void SetViewStateIsRicostituzione()
        {

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                VS_IsRicostituzione = true;
            else
                VS_IsRicostituzione = false;
        }

        public bool IsButtonEditVisible(bool isQuotaProgressiva)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (isQuotaProgressiva)
            {
                if (Utility.IsDomandaVOCUM(this.domanda.Categoria) && CodeUtility.IsRicostituzioneCumuloProgressiva(datiPensione, this.domanda.Categoria))
                    return true;
                else
                    return false;
            }
            else
            {
                if (Utility.IsDomandaVOCUM(this.domanda.Categoria) && CodeUtility.IsRicostituzioneCumuloProgressiva(datiPensione, this.domanda.Categoria))
                    return false;
                else
                    return true;
            }
        }

        #endregion Private Methods

        #region Enum

        public enum EnumControlli
        {
            [Description("lblEnteGestioneFondo_item")]
            lblEnteGestioneFondo,
            [Description("ddlEnteGestioneFondo")]
            ddlEnteGestioneFondo,
            [Description("lblDecorrenza_item")]
            lblDecorrenza,
            [Description("txtDecorrenza")]
            txtDecorrenza,
            [Description("lblImportoQuota")]
            lblImportoQuota,
            [Description("txtImportoQuota")]
            txtImportoQuota,
            [Description("lblIdCodeGestione")]
            lblIdCodeGestione,
            [Description("btnEliminaQuotePensioni")]
            btnEliminaQuotePensioni,
            [Description("lblDescrizioneFondo_item")]
            lblDescrizioneFondo,
            [Description("lblDecorrenzaQuota_item")]
            lblDecorrenzaQuota,
            [Description("txtDecorrenzaQuota")]
            txtDecorrenzaQuota,
            [Description("lblValueDecorrenzaQuota")]
            lblValueDecorrenzaQuota,
            [Description("gvTrattenute")]
            gvTrattenute,
            [Description("imgVisualizzaTrattenute")]
            imgVisualizzaTrattenute,
            [Description("lblIdQuota")]
            lblIdQuota,
            [Description("txtAnnoCompetenza")]
            txtAnnoCompetenza,
            [Description("txtImportoTrattenute")]
            txtImportoTrattenute,
            [Description("REVtxtAnnoCompetenza")]
            REVtxtAnnoCompetenza,
            [Description("RFVtxtAnnoCompetenza")]
            RFVtxtAnnoCompetenza,
            [Description("REVtxtImportoTrattenute")]
            REVtxtImportoTrattenute,
            [Description("RFVtxtImportoTrattenute")]
            RFVtxtImportoTrattenute,
            [Description("RFVddlCodiceTrattenute")]
            RFVddlCodiceTrattenute,
            [Description("ddlCodiceTrattenute")]
            ddlCodiceTrattenute,
        }

        public enum GVMiglioramentiContrattualiColumn
        {
            ModificaQuota = 0,
            Decorrenza = 3,
            EliminaQuota = 6,
            VisualizzaTrattenute
        }

        public enum GVTrattenuteColumn
        {
            ModificaTrattenute = 0,
            EliminaTrattenute = 5,
        }

        #endregion Enum

        #region User Control Events
        public event EventHandler ShowAvviso;
        public event EventHandler InitializeData;
        public event EventHandler GestisciTastoSalva;
        public event EventHandler HideAvviso;
        public event EventHandler AddValidationGroupname;

        protected void RaiseGestisciTastoSalva(object sender, EventArgs e)
        {
            if (GestisciTastoSalva != null)
                GestisciTastoSalva(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseInitializeData(object sender, EventArgs e)
        {
            InitializeData(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseAddValidationGroupname(object sender, EventArgs e)
        {
            if (AddValidationGroupname != null)
                AddValidationGroupname(sender, e);
        }

        #endregion User Control Events

        #region Nestled Class
        [Serializable]
        public class MiglioramentiContrattualiLocal
        {
            public MiglioramentiContrattualiLocal()
            {
                this.Id = Guid.NewGuid();
                this.IsEditable = true;
            }
            public MiglioramentiContrattualiLocal(string codice, string dataDecorrenza, string quota)
            {

                this.Codice = codice;
                this.DataDecorrenza = dataDecorrenza;
                this.Quota = quota;

            }
            public bool IsEmpty()
            {
                bool ret = false;
                if (string.IsNullOrEmpty(this.Codice) && string.IsNullOrEmpty(this.DataDecorrenza) && string.IsNullOrEmpty(this.Quota))
                    ret = true;
                return ret;
            }

            public Guid Id { get; private set; }
            public long? IdPensione { get; set; }
            public string Codice { get; set; }
            public string DataDecorrenza { get; set; }
            public string Quota { get; set; }
            public bool IsStorico { get; set; }
            public bool IsEditable { get; set; }

            public static MiglioramentiContrattualiLocal GetEmptyQuotaPensione()
            {
                return new MiglioramentiContrattualiLocal(string.Empty, string.Empty, string.Empty);
            }


        }
        #endregion Nestled Class
    }
}
