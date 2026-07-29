using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Presenter.Interface;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Periodi
{
    public partial class UCPeriodi : CustomBaseUserControl, IPeriodi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IPeriodi
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaPeriodi areaPeriodi { get; set; }
        #endregion IPeriodi

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalvaDatiPeriodi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaPeriodi = RecuperaCampi();
            PresenterPeriodi presenter = new PresenterPeriodi();
            presenter.SalvaDatiPeriodi(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
            else
            {
                this.ErrorMessage = "Dati Periodi salvati correttamente.";
                RaiseShowAvviso(this, null);
            }

            RaiseAggiornaSemaforo(this, null);
        }

        protected void btnEliminaDatiPeriodi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaPeriodi = RecuperaCampiPerDelete();
            PresenterPeriodi presenter = new PresenterPeriodi();
            presenter.EliminaDatiPeriodi(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
            else
            {
                this.ErrorMessage = "Dati Periodi eliminati correttamente.";
                RaiseShowAvviso(this, null);
            }

            RaiseAggiornaSemaforo(this, null);

            InitData(this.areaPeriodi);
        }
        #endregion protected methods

        #region internal methods
        internal void InitData(AreaPeriodi areaPeriodi)
        {
            SetViewState(areaPeriodi);
            ValorizzaEtichette(areaPeriodi);

            ManagePulsanti();
        }

        internal AreaPeriodi RecuperaCampi()
        {
            AreaPeriodi areaPeriodi = new AreaPeriodi();
            areaPeriodi.DatiPeriodi = new PeriodoAventiDiritto();
            areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto = (GestioneAnagraficaDatiAnagrafici)ViewState[EnumViewstate.Anagrafica.ToString()];
            areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto = (GestioneFamiliariFamiliare)ViewState[EnumViewstate.Familiare.ToString()];
            if (!string.IsNullOrEmpty(txtRevSan.Text))
                areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto.ScadenzaRevisioneSanitaria = Utility.GetDateFromString(txtRevSan.Text);
            areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto = (GestionePeriodiAventiDirittoPeriodoAventiDiritto[])ViewState[EnumViewstate.ListaPeriodi.ToString()];

            // Rimuovo l'elemento vuoto
            if (areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto.Count() > 0)
                areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto = areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto.Take(areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto.Count() - 1).ToArray();

            areaPeriodi.DatiPeriodi.IdAventeDiritto = (long)ViewState[EnumViewstate.IdAventeDiritto.ToString()];

            areaPeriodi.ElencoGradiParentela = (GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare[])ViewState[EnumViewstate.ListaGradoParentela.ToString()];

            return areaPeriodi;
        }

        internal AreaPeriodi RecuperaCampiPerDelete()
        {
            AreaPeriodi areaPeriodi = new AreaPeriodi();
            areaPeriodi.DatiPeriodi = new PeriodoAventiDiritto();
            areaPeriodi.DatiPeriodi.IdAventeDiritto = (long)ViewState[EnumViewstate.IdAventeDiritto.ToString()];
            areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto = (GestioneAnagraficaDatiAnagrafici)ViewState[EnumViewstate.Anagrafica.ToString()];
            areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto = (GestioneFamiliariFamiliare)ViewState[EnumViewstate.Familiare.ToString()];
            areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto.ScadenzaRevisioneSanitaria = null;
            areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto = ((GestionePeriodiAventiDirittoPeriodoAventiDiritto[])ViewState[EnumViewstate.ListaPeriodi.ToString()]);
            areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto = areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto.Where(x => x.IsFromWebDom).ToArray();

            return areaPeriodi;
        }
        #endregion internal methods

        #region private methods
        private void SetViewState(AreaPeriodi areaPeriodi)
        {
            if (areaPeriodi != null && areaPeriodi.DatiPeriodi != null)
            {
                ViewState[EnumViewstate.IdAventeDiritto.ToString()] = areaPeriodi.DatiPeriodi.IdAventeDiritto;
                ViewState[EnumViewstate.ListaPeriodi.ToString()] = areaPeriodi.DatiPeriodi.ListaPeriodiAventeDiritto ?? new GestionePeriodiAventiDirittoPeriodoAventiDiritto[1];
                ViewState[EnumViewstate.ListaGradoParentela.ToString()] = areaPeriodi.ElencoGradiParentela ?? new GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare[1];

                if (areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto != null)
                    ViewState[EnumViewstate.Anagrafica.ToString()] = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto;

                if (areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto != null)
                    ViewState[EnumViewstate.Familiare.ToString()] = areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto;
            }
        }

        private void ValorizzaEtichette(AreaPeriodi areaPeriodi)
        {
            if (areaPeriodi != null && areaPeriodi.DatiPeriodi != null)
            {
                if (areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto != null)
                {
                    lblCodiceFiscale.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.CodiceFiscale;
                    Lbcognome.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Cognome;
                    LbNome.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Nome;
                    lbCognAcquisito.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.CognomeAcquisito;
                    if (areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Sesso.HasValue)
                        LbSesso.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.Sesso.ToString();
                    if (areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.DataNascita.HasValue)
                        LbDataDiNascita.Text = string.Format("{0:dd/MM/yyyy}", areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.DataNascita.Value);
                    LbComunedinascita.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.ComuneNascita;
                    LbProvinciadinascita.Text = areaPeriodi.DatiPeriodi.DatiAnagraficiAventeDiritto.ProvinciaNascita;
                    if (areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto.DataMorte.HasValue)
                    {
                        lblDataMorteValue.Text = string.Format("{0:dd/MM/yyyy}", areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto.DataMorte.Value);
                        pnlDataMorte.Visible = true;
                    }
                }

                if (areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto != null)
                {
                    if (areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto.ScadenzaRevisioneSanitaria.HasValue)
                        txtRevSan.Text = areaPeriodi.DatiPeriodi.DatiFamiliareAventeDiritto.ScadenzaRevisioneSanitaria.Value.ToString("MM/yyyy");
                    else
                        txtRevSan.Text = string.Empty;
                }

                gvPeriodi_Load();
            }
        }

        private void ManagePulsanti()
        {
            if (CodeUtility.IsGridViewInEditPresent(this.Page))
            {
                btnSalvaDatiPeriodi.Enabled = false;
                RaiseDisabilitaPulsanti(this, null);
            }
            else
            {
                btnSalvaDatiPeriodi.Enabled = true;
                RaiseAbilitaPulsanti(this, null);
            }
        }
        #endregion private methods

        #region gvPeriodi
        #region protected methods
        protected void gvPeriodi_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            RaiseHideAvviso(this, null);
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

                GestionePeriodiAventiDirittoPeriodoAventiDiritto[] listaPeriodi = (GestionePeriodiAventiDirittoPeriodoAventiDiritto[])ViewState[EnumViewstate.ListaPeriodi.ToString()];
                var listaPeriodiApp = listaPeriodi.ToList();
                listaPeriodiApp.RemoveAt(r.RowIndex);
                listaPeriodi = listaPeriodiApp.ToArray();
                ViewState[EnumViewstate.ListaPeriodi.ToString()] = listaPeriodi;

                gvPeriodi.EditIndex = -1;
                gvPeriodi_Load();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

                GestionePeriodiAventiDirittoPeriodoAventiDiritto[] listaPeriodi = (GestionePeriodiAventiDirittoPeriodoAventiDiritto[])ViewState[EnumViewstate.ListaPeriodi.ToString()];
                GestionePeriodiAventiDirittoPeriodoAventiDiritto periodoInEdit = listaPeriodi.ElementAt(r.RowIndex);

                DropDownList gradoParentela = (DropDownList)r.FindControl(Keys.DdlGradoParentela_GrigliaPeriodi);
                TextBox decorrenzaPeriodo = (TextBox)r.FindControl(Keys.TxtDecorrenzaPeriodo_GrigliaPeriodi);
                TextBox cessazionePeriodo = (TextBox)r.FindControl(Keys.TxtCessazionePeriodo_GrigliaPeriodi);

                if (!string.IsNullOrEmpty(gradoParentela.SelectedValue))
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                    char? grado = CodeUtility.StringToNullableChar(gradoParentela.SelectedValue.Substring(0, 1));
                    if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                        !listaPeriodi.Any(x => x.GradoParentela == 'C') && grado == 'C')
                    {
                        this.HasError = true;
                        this.ErrorMessage = "Non è possibile inserire un periodo con relazione Coniuge/Unito Civilmente se non è già presente un periodo con la stessa relazione";
                        RaiseShowAvviso(this, null);
                        return;
                    }
                    periodoInEdit.GradoParentela = grado;
                    // Effettuo il PadRight perchè non tutte le tipologie di parentela hanno il secondo carattere
                    periodoInEdit.TipoUnione = gradoParentela.SelectedValue.PadRight(2, ' ').Substring(1, 1);

                    if (periodoInEdit.GradoParentela == 'R')
                    {
                        TextBox percGiudice = (TextBox)r.FindControl(Keys.TxtPercGiudice_GrigliaPeriodi);
                        if (percGiudice != null)
                            periodoInEdit.PercGiudice = CodeUtility.StringToNullableDecimal(percGiudice.Text);
                    }
                    else
                        periodoInEdit.PercGiudice = null;

                    if (r.RowIndex > 0) 
                    {
                        DateTime? DecorrenzaEditRow = Utility.GetDateFromString(decorrenzaPeriodo.Text);
                        DateTime? DecorrenzaPreviusRow = listaPeriodi.ElementAt(r.RowIndex - 1).DecorrenzaPeriodo;

                        if (DecorrenzaEditRow.HasValue && DecorrenzaPreviusRow.HasValue) 
                        {
                            //sopra ci deve essere quella meno recente
                            if (DecorrenzaPreviusRow > DecorrenzaEditRow) 
                            {
                                this.HasError = true;
                                this.ErrorMessage = "I periodi devono essere inseriti da quello con decorrenza minore a quello con decorrenza maggiore";
                                RaiseShowAvviso(this, null);
                                return;
                            }
                        }
                    }
                }
                periodoInEdit.DecorrenzaPeriodo = Utility.GetDateFromString(decorrenzaPeriodo.Text);
                periodoInEdit.CessazionePeriodo = Utility.GetDateFromString(cessazionePeriodo.Text);

                gvPeriodi.EditIndex = -1;
                gvPeriodi_Load();

                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvPeriodi.EditIndex = -1;
                gvPeriodi_Load();
            }

            ManagePulsanti();
        }

        protected void gvPeriodi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    GestionePeriodiAventiDirittoPeriodoAventiDiritto periodo = (GestionePeriodiAventiDirittoPeriodoAventiDiritto)e.Row.DataItem;

                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            GestioneDdls(e.Row);
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaPeriodi, Page.Theme);
                            LinkButton delete = ((LinkButton)(e.Row.FindControl(Keys.BtnDelete_GrigliaPeriodi)));
                            delete.Text = string.Empty;
                            if (periodo.IsFromWebDom || periodo.IsFromGP.GetValueOrDefault())
                            {
                                DropDownList ddlGradoParentela = (DropDownList)e.Row.FindControl(Keys.DdlGradoParentela_GrigliaPeriodi);
                                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                                if (periodo.IsFromWebDom)
                                {
                                    if (!Utility.IsDomandaReversibilita(datiPensione))
                                    {
                                        TextBox txtDecorrenzaPeriodo = (TextBox)e.Row.FindControl(Keys.TxtDecorrenzaPeriodo_GrigliaPeriodi);
                                        txtDecorrenzaPeriodo.Enabled = false;
                                    }
                                }
                                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                                    periodo.GradoParentela == 'C' && periodo.IsFromGP.GetValueOrDefault())
                                    ddlGradoParentela.Enabled = false;
                                if (periodo.IsFromWebDom && periodo.GradoParentela == 'O' && this.domanda.IsDomandaINPDAP)
                                {
                                    ddlGradoParentela.Enabled = false;
                                }
                            }
                        
                        }
                        else
                        {
                            if (periodo.IsFromWebDom)
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, null);
                            else
                                CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.BtnDelete_GrigliaPeriodi);

                            Label lblGradoParentela = (Label)e.Row.FindControl(Keys.LblGradoParentela_GrigliaPeriodi);
                            lblGradoParentela.Text = GetId_DescriptionGradoParentelaById(periodo.GradoParentela, periodo.TipoUnione.PadLeft(1, ' '));
                        }
                    }
                    else   //record successivi al primo
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            GestioneDdls(e.Row);
                            CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaPeriodi, Page.Theme);
                            LinkButton delete = ((LinkButton)(e.Row.FindControl(Keys.BtnDelete_GrigliaPeriodi)));
                            delete.Text = string.Empty;
                            if (periodo.IsFromWebDom || periodo.IsFromGP.GetValueOrDefault())
                            {
                                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                                DropDownList ddlGradoParentela = (DropDownList)e.Row.FindControl(Keys.DdlGradoParentela_GrigliaPeriodi);
                                if (periodo.IsFromWebDom)
                                {
                                    if (!Utility.IsDomandaReversibilita(datiPensione) && !Utility.IsDomandaIndiretta(datiPensione))
                                    {
                                        TextBox txtDecorrenzaPeriodo = (TextBox)e.Row.FindControl(Keys.TxtDecorrenzaPeriodo_GrigliaPeriodi);
                                        txtDecorrenzaPeriodo.Enabled = false;
                                    }
                                }
                                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                                    periodo.GradoParentela == 'C' && periodo.IsFromGP.GetValueOrDefault())
                                    ddlGradoParentela.Enabled = false;
                                if (periodo.IsFromWebDom && periodo.GradoParentela == 'O' && this.domanda.IsDomandaINPDAP)
                                {
                                    ddlGradoParentela.Enabled = false;
                                }
                            }
                           
                        }
                        else
                        {
                            if (e.Row.DataItemIndex == ((GestionePeriodiAventiDirittoPeriodoAventiDiritto[])ViewState[EnumViewstate.ListaPeriodi.ToString()]).Count() - 1)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";
                            }
                            else
                            {
                                if (periodo.IsFromWebDom)
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, null);
                                else
                                    CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.BtnDelete_GrigliaPeriodi);

                                Label lblGradoParentela = (Label)e.Row.FindControl(Keys.LblGradoParentela_GrigliaPeriodi);
                                lblGradoParentela.Text = GetId_DescriptionGradoParentelaById(periodo.GradoParentela, periodo.TipoUnione.PadLeft(1, ' '));
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
                throw new INPS.DNA.DnaApplicationException("UCPeriodi, Errore nel metodo gvPeriodi_RowDataBound " + ex);
            }
        }

        protected void gvPeriodi_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvPeriodi.EditIndex = e.NewEditIndex;
                gvPeriodi_Load();

                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPeriodi, Errore nel metodo gvPeriodi_RowEditing " + ex);
            }
        }

        protected void gvPeriodi_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvPeriodi.EditIndex = -1;
                gvPeriodi_Load();

                ManagePulsanti();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCPeriodi, Errore nel metodo gvPeriodi_RowCancelingEdit " + ex);
            }
        }

        #endregion protected methods

        #region private methods
        private void gvPeriodi_Load()
        {
            GestionePeriodiAventiDirittoPeriodoAventiDiritto[] listaPeriodi = (GestionePeriodiAventiDirittoPeriodoAventiDiritto[])ViewState[EnumViewstate.ListaPeriodi.ToString()];

            if (listaPeriodi == null || listaPeriodi.Length == 0 || !listaPeriodi.ToList().Exists(x => x != null))
                return;

            if (listaPeriodi.FirstOrDefault(x => x.GradoParentela.GetValueOrDefault() == 'C') != null)
                lblPercGiudicePerConiuge.Visible = true;
            else
                lblPercGiudicePerConiuge.Visible = false;

            if (listaPeriodi.FirstOrDefault(x => x.GradoParentela.GetValueOrDefault() == 'R') != null)
                lblPercGiudicePerExConiuge.Visible = true;
            else
                lblPercGiudicePerExConiuge.Visible = false;

            if (listaPeriodi.Count() == 1 && !listaPeriodi.First().GradoParentela.HasValue && !listaPeriodi.First().DecorrenzaPeriodo.HasValue && !listaPeriodi.First().CessazionePeriodo.HasValue)
                gvPeriodi.EditIndex = 0;
            else if (!listaPeriodi.ToList().Exists(x => !x.GradoParentela.HasValue && !x.DecorrenzaPeriodo.HasValue && !x.CessazionePeriodo.HasValue))
            {
                var listaPeriodiApp = listaPeriodi.ToList();
                listaPeriodiApp.Add(new GestionePeriodiAventiDirittoPeriodoAventiDiritto());
                listaPeriodi = listaPeriodiApp.ToArray();
                ViewState[EnumViewstate.ListaPeriodi.ToString()] = listaPeriodi;

                if (listaPeriodi.Count() == 1)
                    gvPeriodi.EditIndex = 0;
            }

            //DECORRENZA PERIODO
            if (!listaPeriodi[0].DecorrenzaPeriodo.HasValue)
            {
                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                listaPeriodi[0].DecorrenzaPeriodo = datiPensione.DecorrenzaOriginaria;
            }

            //CESSAZIONE PERIODO
            GestioneAnagraficaDatiAnagrafici aventiDiritto = (GestioneAnagraficaDatiAnagrafici)ViewState[EnumViewstate.Anagrafica.ToString()];
            DateTime? cessazionePeriodo = null;
            DateTime dataNascita = aventiDiritto.DataNascita.GetValueOrDefault();

            for (int i = 0; i < listaPeriodi.Length - 1; i++)
            {
                if (!listaPeriodi[i].CessazionePeriodo.HasValue)
                {
                    char gradoParentela = listaPeriodi[i].GradoParentela.GetValueOrDefault();

                    switch (gradoParentela)
                    {
                        case 'M': //MINORE
                            cessazionePeriodo = Utility.FirstDayOfMonth(dataNascita.AddYears(18).AddMonths(1));
                            break;
                        case 'S': //STUDENTE
                            cessazionePeriodo = Utility.FirstDayOfMonth(dataNascita.AddYears(21).AddMonths(1));
                            break;
                        case 'U': //UNIVERSITARIO
                            cessazionePeriodo = Utility.FirstDayOfMonth(dataNascita.AddYears(26).AddMonths(1));
                            break;
                    }

                    listaPeriodi[i].CessazionePeriodo = cessazionePeriodo;
                }
            }

            gvPeriodi.DataSource = listaPeriodi;
            gvPeriodi.DataBind();
        }

        private void GestioneDdls(GridViewRow row)
        {
            DropDownList ddlGradoParentela = new DropDownList();
            ddlGradoParentela = (DropDownList)row.FindControl(Keys.DdlGradoParentela_GrigliaPeriodi);

            ddlGradoParentela.Items.Add(new ListItem(string.Empty, string.Empty));

            List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> listaGradoParentela = ((GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare[])ViewState[EnumViewstate.ListaGradoParentela.ToString()]).ToList();

            // Unito Civilmente
            var idx = listaGradoParentela.FindIndex(x => x.Id == "C" && x.TipoUnione == "U");
            var item = listaGradoParentela[idx];
            listaGradoParentela.RemoveAt(idx);
            listaGradoParentela.Insert(0, item);

            // Coniuge
            idx = listaGradoParentela.FindIndex(x => x.Id == "C" && x.TipoUnione == "M");
            item = listaGradoParentela[idx];
            listaGradoParentela.RemoveAt(idx);
            listaGradoParentela.Insert(0, item);

            foreach (var gradoParentela in listaGradoParentela)
                CodeUtility.SetValueDdl(ddlGradoParentela, gradoParentela.Id + " - " + gradoParentela.Descrizione, gradoParentela.Id + " - " + gradoParentela.Descrizione, gradoParentela.Id + gradoParentela.TipoUnione.PadLeft(1, ' '));

            if (((GestionePeriodiAventiDirittoPeriodoAventiDiritto)(row.DataItem)).GradoParentela.HasValue)
                ddlGradoParentela.Items.FindByValue(((GestionePeriodiAventiDirittoPeriodoAventiDiritto)(row.DataItem)).GradoParentela.Value.ToString() + ((GestionePeriodiAventiDirittoPeriodoAventiDiritto)(row.DataItem)).TipoUnione.PadLeft(1, ' ')).Selected = true;
        }

        private string GetId_DescriptionGradoParentelaById(char? gradoParentela, string tipoUnione)
        {
            if (gradoParentela.HasValue)
            {
                GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare[] listaGradoParentela = (GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare[])ViewState[EnumViewstate.ListaGradoParentela.ToString()];
                GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare objGradoParentela = listaGradoParentela.FirstOrDefault(x => x.Id == gradoParentela.ToString() && x.TipoUnione.PadLeft(1, ' ') == tipoUnione);
                if (objGradoParentela != null)
                    return objGradoParentela.Id + " - " + objGradoParentela.Descrizione;
            }

            return string.Empty;
        }
        #endregion private methods
        #endregion gvPeriodi

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler AggiornaSemaforo;
        public event EventHandler AbilitaPulsanti;
        public event EventHandler DisabilitaPulsanti;
        public event EventHandler HideAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseAggiornaSemaforo(object sender, EventArgs e)
        {
            if (AggiornaSemaforo != null)
                AggiornaSemaforo(sender, e);
        }

        protected void RaiseAbilitaPulsanti(object sender, EventArgs e)
        {
            if (AbilitaPulsanti != null)
                AbilitaPulsanti(sender, e);
        }

        protected void RaiseDisabilitaPulsanti(object sender, EventArgs e)
        {
            if (DisabilitaPulsanti != null)
                DisabilitaPulsanti(sender, e);
        }
        #endregion Events

        #region enum
        public enum EnumViewstate
        {
            IdAventeDiritto,
            ListaPeriodi,
            Anagrafica,
            Familiare,
            ListaGradoParentela
        }

        #endregion enum

        #region nested classes
        public class Keys
        {
            public const string ValidationGroup_GrigliaPeriodi = "UCPeriodiGrid";
            public const string BtnDelete_GrigliaPeriodi = "btnDeletePeriodi";
            public const string LblGradoParentela_GrigliaPeriodi = "lblGradoParentela";
            public const string DdlGradoParentela_GrigliaPeriodi = "ddlGradoParentela";
            public const string TxtDecorrenzaPeriodo_GrigliaPeriodi = "txtDecorrenzaPeriodo";
            public const string TxtCessazionePeriodo_GrigliaPeriodi = "txtCessazionePeriodo";
            public const string TxtPercGiudice_GrigliaPeriodi = "txtPercGiudice";
        }
        #endregion nested classes
    }
}
