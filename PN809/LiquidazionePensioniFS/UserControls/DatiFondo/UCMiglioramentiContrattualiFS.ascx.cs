using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using System.ComponentModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCmiglioramenticontrattualiFS : CustomBaseUserControl, IDatiFondo, ITitolarePensione
    {
        public string ValidationGroupname { get; set; }

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare
    
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        #region View State Variables

        private bool VS_IsRicostituzione
        {
            get { return (bool)ViewState["IsRicostituzione"]; }
            set { ViewState["IsRicostituzione"] = value; }
        }

        private List<MiglioramenticontrattualiFSLocal> VS_ElencoMiglioramentiContrattualiFS
        {
            get { return (List<MiglioramenticontrattualiFSLocal>)ViewState["elencoMiglioramentiContrattualiFS"]; }
            set { ViewState["elencoMiglioramentiContrattualiFS"] = value; }

        }

        private bool VS_IsModalitaEdit
        {
            get { return (bool)ViewState["IsModalitaEdit"]; }
            set { ViewState["IsModalitaEdit"] = value; }
        }

        private AreaDatiFondo VS_AreaDatiFondo
        {
            get { return (AreaDatiFondo)ViewState["AreaDatiFondo"]; }
            set { ViewState["AreaDatiFondo"] = value; }
        }

        private List<MiglioramenticontrattualiFSLocal> VS_ElencomiglioramenticontrattualiFSInizio
        {
            get { return (List<MiglioramenticontrattualiFSLocal>)ViewState["elencoMiglioramentiContrattualiIFSnizio"]; }
            set { ViewState["elencoMiglioramentiContrattualiFSInizio"] = value; }
        }


        #endregion View State Variables

            #region Web Form Events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void btnSalvamiglioramenticontrattualiFS_Click(object sender, EventArgs args)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            // serve per il salvataggio completo (rivisitabile)
            try
            {
                ((Web.DatiFondo)sender).HasError = this.HasError;
                ((Web.DatiFondo)sender).ErrorMessage = this.ErrorMessage;
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
            //if (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica)
            //    btnEliminamiglioramenticontrattualiFS.Enabled = true;
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Fondo salvati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        public void btnEliminamiglioramenticontrattualiFS_Click(object sender, EventArgs args)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];


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
                this.ErrorMessage = "Dati Fondo eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }
        }

        internal void DisabilitaPulsanti()
        {
            btnSalvamiglioramenticontrattualiFS.Enabled = false;
            btnEliminamiglioramenticontrattualiFS.Enabled = false;
        }
        #endregion Web Form Events

        #region Grid View Quote Pensione
        protected void gvmiglioramenticontrattualiFS_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gvmiglioramenticontrattualiFS = (GridView)sender;
                gvmiglioramenticontrattualiFS.EditIndex = e.NewEditIndex;
                VS_IsModalitaEdit = true;
                gvmiglioramenticontrattualiFS.DataSource = VS_ElencoMiglioramentiContrattualiFS;
                gvmiglioramenticontrattualiFS.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCmiglioramenticontrattualiFS, Errore nel metodo gvmiglioramenticontrattualiFS_RowEditing " + ex);
            }
        }

        protected void gvmiglioramenticontrattualiFS_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvmiglioramenticontrattualiFS = (GridView)sender;
                GridViewRow row = gvmiglioramenticontrattualiFS.Rows[e.RowIndex];
                MiglioramenticontrattualiFSLocal quotaPensione = VS_ElencoMiglioramentiContrattualiFS[row.DataItemIndex];

                quotaPensione.Codice = ((DropDownList)row.FindControl(Utility.GetDescription(EnumControlli.ddlEnteGestioneFondo))).SelectedItem.Text;

                decimal d = 0;
                decimal.TryParse(((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtImportoQuota))).Text, out d);
                quotaPensione.Quota = string.Format("{0:F7}", d);

                if (gvmiglioramenticontrattualiFS.Columns[GVmiglioramenticontrattualiFSColumn.Decorrenza.GetHashCode()].Visible)
                    quotaPensione.DataDecorrenza = ((TextBox)row.FindControl(Utility.GetDescription(EnumControlli.txtDecorrenzaQuota))).Text;


                VS_ElencoMiglioramentiContrattualiFS[e.RowIndex] = quotaPensione;
                gvmiglioramenticontrattualiFS.EditIndex = -1;
                VS_IsModalitaEdit = false;
                gvmiglioramenticontrattualiFS.DataSource = VS_ElencoMiglioramentiContrattualiFS;
                gvmiglioramenticontrattualiFS.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiFondo, Errore nel metodo gvmiglioramenticontrattualiFS_RowUpdating " + ex);
            }
        }

        protected void gvmiglioramenticontrattualiFS_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridView gvmiglioramenticontrattualiFS = (GridView)sender;
                VS_ElencoMiglioramentiContrattualiFS.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoMiglioramentiContrattualiFS.Count == 0)
                {
                    VS_ElencoMiglioramentiContrattualiFS.Add(MiglioramenticontrattualiFSLocal.GetEmptyQuotaPensione());
                    gvmiglioramenticontrattualiFS.EditIndex = 0;
                }
                else
                {
                    VS_IsModalitaEdit = false;
                    gvmiglioramenticontrattualiFS.EditIndex = -1;
                }
                gvmiglioramenticontrattualiFS.DataSource = VS_ElencoMiglioramentiContrattualiFS;
                gvmiglioramenticontrattualiFS.DataBind();
                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCmiglioramenticontrattualiFS, Errore nel metodo gvmiglioramenticontrattualiFS_RowCancelingEdit/ " + ex);
            }
        }

        protected void gvmiglioramenticontrattualiFS_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                RaiseHideAvviso(this, null);
                if (e.CommandName == "Aggiungi")
                {
                    GridView gvmiglioramenticontrattualiFS = (GridView)sender;
                    VS_ElencoMiglioramentiContrattualiFS.Add(MiglioramenticontrattualiFSLocal.GetEmptyQuotaPensione());
                    VS_IsModalitaEdit = true;
                    gvmiglioramenticontrattualiFS.EditIndex = VS_ElencoMiglioramentiContrattualiFS.Count - 1;
                    gvmiglioramenticontrattualiFS.DataSource = VS_ElencoMiglioramentiContrattualiFS;
                    gvmiglioramenticontrattualiFS.DataBind();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiFondo, Errore nel metodo gvmiglioramenticontrattualiFS_RowCommand " + ex);
            }
        }

        protected void gvmiglioramenticontrattualiFS_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                GridView gvmiglioramenticontrattualiFS = (GridView)sender;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowIndex == gvmiglioramenticontrattualiFS.EditIndex)
                    {

                        CodeUtility.EnableEditableMode(e.Row.Cells[0], "UCGvmiglioramenticontrattualiFS", Page.Theme, false);
                        ((Image)e.Row.FindControl(Utility.GetDescription(EnumControlli.imgVisualizzaTrattenute))).Visible = false;
                    }
                    else
                    {
                        MiglioramenticontrattualiFSLocal row = VS_ElencoMiglioramentiContrattualiFS[e.Row.RowIndex];
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
                    e.Row.Visible = !VS_IsRicostituzione && gvmiglioramenticontrattualiFS.EditIndex == -1;
                    if (!VS_IsRicostituzione && e.Row.Visible)
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
                throw new INPS.DNA.DnaApplicationException("UCDatiFondo, Errore nel metodo gvmiglioramenticontrattualiFS_RowDataBound " + ex);
            }
        }

        protected void gvmiglioramenticontrattualiFS_DataBinding(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GridView gvmiglioramenticontrattualiFS = (GridView)sender;
            gvmiglioramenticontrattualiFS.ShowFooter =!VS_IsRicostituzione && gvmiglioramenticontrattualiFS.EditIndex == -1;
        }

        protected void gvmiglioramenticontrattualiFS_DataBound(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            this.areaDatiFondo = VS_AreaDatiFondo;
            GridView gvmiglioramenticontrattualiFS = (GridView)sender;

            gvmiglioramenticontrattualiFS.Columns[GVmiglioramenticontrattualiFSColumn.ModificaQuota.GetHashCode()].Visible = (!Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione) && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica);
            //gvmiglioramenticontrattualiFS.Columns[GVmiglioramenticontrattualiFSColumn.EliminaQuota.GetHashCode()].Visible = !VS_IsRicostituzione && !datiPensione.IsDomandaCumuloAutomatica && !datiPensione.IsDomandaTotAutomatica;
            gvmiglioramenticontrattualiFS.Columns[GVmiglioramenticontrattualiFSColumn.ModificaQuota.GetHashCode()].Visible = false;
            ManagePulsanti();
        }

        protected void gvmiglioramenticontrattualiFS_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gvmiglioramenticontrattualiFS = (GridView)sender;
                VS_ElencoMiglioramentiContrattualiFS.RemoveAt(e.RowIndex);
                //rimozione di eventuali record vuoti
                VS_ElencoMiglioramentiContrattualiFS.RemoveAll(x => x.IsEmpty());
                if (VS_ElencoMiglioramentiContrattualiFS.Count == 0)
                {
                    VS_ElencoMiglioramentiContrattualiFS.Add(MiglioramenticontrattualiFSLocal.GetEmptyQuotaPensione());
                    gvmiglioramenticontrattualiFS.EditIndex = 0;
                    VS_IsModalitaEdit = true;
                }
                else
                    gvmiglioramenticontrattualiFS.EditIndex = -1;
                gvmiglioramenticontrattualiFS.DataSource = VS_ElencoMiglioramentiContrattualiFS;
                gvmiglioramenticontrattualiFS.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiFondo, Errore nel metodo gvTrattenute_RowDeleting " + ex);
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

        private void ManagePulsanti()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (VS_ElencoMiglioramentiContrattualiFS != null && VS_ElencoMiglioramentiContrattualiFS.Where(x => !x.IsEmpty()).Count() > 0 && !VS_IsModalitaEdit)
            {
                btnSalvamiglioramenticontrattualiFS.Enabled = false;
                //TODO per il momento tutto bloccato
                //btnSalvamiglioramenticontrattualiFS.Enabled = true;
                RaiseGestisciTastoSalva(this, null);
            }
            else
            {
                btnSalvamiglioramenticontrattualiFS.Enabled = false;
                RaiseGestisciTastoSalva(this, null);
            }


            //if (VS_ElencomiglioramenticontrattualiFS != null && VS_ElencomiglioramenticontrattualiFS.Where(x => !x.IsEmpty()).Count() > 0 &&
            //    VS_IsRicostituzione == false && !VS_IsModalitaEdit)
            //    btnEliminamiglioramenticontrattualiFS.Enabled = true;
            //else
            //    btnEliminamiglioramenticontrattualiFS.Enabled = false;

            //if (this.domanda.IsDomandaRiapertura && Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica && !this.domanda.Categoria.StartsWith("S"))
            //    btnEliminamiglioramenticontrattualiFS.Enabled = false;

            //if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(this.TitolarePensione.Pensione) || this.TitolarePensione.Pensione.IsDomandaCumuloAutomatica == true ||
            //    this.TitolarePensione.Pensione.IsDomandaTotAutomatica == true || Utility.IsDomandaRipristino(this.TitolarePensione.Pensione))
            //    btnEliminamiglioramenticontrattualiFS.Enabled = false;
        }

        public void ValorizzaEtichette(IDatiFondo idDatiFondo)
        {
            AreaDatiFondo  areaDatiFondo = idDatiFondo.areaDatiFondo;
            VS_AreaDatiFondo = areaDatiFondo;

            SetViewStateIsRicostituzione();
            if (idDatiFondo.areaDatiFondo != null && idDatiFondo.areaDatiFondo.QuoteMiglioramentiContrattuali != null)
            {
                VS_IsModalitaEdit = false;
                InitGvmiglioramenticontrattualiFS(idDatiFondo.areaDatiFondo.QuoteMiglioramentiContrattuali.LDatiQuoteMiglioramentiContrattuali.ToList());
            }
            else // dati non presenti
            {
                VS_IsModalitaEdit = true;
                InitGvmiglioramenticontrattualiFS(null);
            }
            ManagePulsanti();


        }

        private void InitGvmiglioramenticontrattualiFS(List<GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali> lstServer)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            List<MiglioramenticontrattualiFSLocal> lstmiglioramenticontrattualiFS = new List<MiglioramenticontrattualiFSLocal>();

            if (lstServer != null && lstServer.Count() > 0)
            {
                lstmiglioramenticontrattualiFS.AddRange(MapServiceToLocalObject(lstServer));
            }
            if (lstmiglioramenticontrattualiFS.Count == 0 && !VS_IsRicostituzione)
                lstmiglioramenticontrattualiFS.Add(MiglioramenticontrattualiFSLocal.GetEmptyQuotaPensione());
            VS_ElencoMiglioramentiContrattualiFS = lstmiglioramenticontrattualiFS;
            //View state dove salvo la lista quote al primo caricamento del quadro (non verrà modificata)
            VS_ElencomiglioramenticontrattualiFSInizio = lstmiglioramenticontrattualiFS;
            if (lstmiglioramenticontrattualiFS.Count == 1 && lstmiglioramenticontrattualiFS[0].IsEmpty())
                gvmiglioramenticontrattualiFS.EditIndex = 0;
            gvmiglioramenticontrattualiFS.DataSource = lstmiglioramenticontrattualiFS;
            gvmiglioramenticontrattualiFS.DataBind();
        }


        public List<GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali> MapLocalToServiceObject(List<MiglioramenticontrattualiFSLocal> lstLocal)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali> lstService = new List<GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali>();
            if (lstLocal != null && lstLocal.Count() > 0)
            {
                foreach (MiglioramenticontrattualiFSLocal elem in lstLocal)
                {
                    if (!elem.IsEmpty())
                    {
                        GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali elemService = new GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali();
                        elemService.Quota = elem.Quota;
                        elemService.Codice = elem.Codice;
                        elemService.DataDecorrenza = elem.DataDecorrenza;

                        lstService.Add(elemService);
                    }
                }
            }
            return lstService;
        }

        public List<MiglioramenticontrattualiFSLocal> MapServiceToLocalObject(List<GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali> lstService)
        {
            List<MiglioramenticontrattualiFSLocal> lstLocal = new List<MiglioramenticontrattualiFSLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (GestioneMiglioramentiContrattualiDatiQuoteMiglioramentiContrattuali elemS in lstService)
                {
                    MiglioramenticontrattualiFSLocal elemL = new MiglioramenticontrattualiFSLocal();
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

        public enum GVmiglioramenticontrattualiFSColumn
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
        public class MiglioramenticontrattualiFSLocal
        {
            public MiglioramenticontrattualiFSLocal()
            {
                this.Id = Guid.NewGuid();
                this.IsEditable = true;
            }
            public MiglioramenticontrattualiFSLocal(string codice, string dataDecorrenza, string quota)
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

            public static MiglioramenticontrattualiFSLocal GetEmptyQuotaPensione()
            {
                return new MiglioramenticontrattualiFSLocal(string.Empty, string.Empty, string.Empty);
            }


        }
        #endregion Nestled Class
    }
}
