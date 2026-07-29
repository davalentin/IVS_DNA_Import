using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AventiDiritto
{
    public partial class UCAventiDiritto : CustomBaseUserControl, IAventiDiritto, IDanteCausa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IAventiDiritto
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaAventiDiritto AreaAventiDiritto { get; set; }
        #endregion IAventiDiritto

        #region IDanteCausa
        public AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domandaDante { get; set; }
        public long numDomanda { get; set; }
        #endregion IDanteCausa

        /// <summary>
        /// Questo campo serve a capire se la prima colonna del repeater (quella relativa ai radio button) è visibile
        /// </summary>
        bool columnSelectionVisible = false;

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void SalvaAventiDiritto_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.AreaAventiDiritto = RecuperaCampi();
            PresenterAventiDiritto presenter = new PresenterAventiDiritto();
            presenter.SalvaDatiAventiDiritto(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
            else
            {
                ValorizzaEtichette(this);

                this.ErrorMessage = "Dati Aventi Diritto salvati correttamente.";
                RaiseShowAvviso(this, null);
            }

            RaiseAggiornaSemaforo(this, null);
        }

        protected void AggiornaDaWebDom_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterAventiDiritto presenter = new PresenterAventiDiritto();
            presenter.AggiornaAventiDirittoFromWebDom(this);

            RaiseAggiornaSemaforo(this, null);

            if (!this.HasError)
            {
                ValorizzaEtichette(this);

                this.ErrorMessage = "Aggiornamento degli Aventi Diritto eseguito correttamente.";
                RaiseShowAvviso(this, null);
            }
            else
                RaiseShowAvviso(this, null);
        }

        protected void AggiornaDaArchivioPensione_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterAventiDiritto presenter = new PresenterAventiDiritto();
            presenter.AggiornaAventiDirittoFromArchivioPensione(this);

            RaiseAggiornaSemaforo(this, null);

            if (!this.HasError)
            {
                ValorizzaEtichette(this);

                this.ErrorMessage = "Aggiornamento degli Aventi Diritto eseguito correttamente.";
                RaiseShowAvviso(this, null);
            }
            else
                RaiseShowAvviso(this, null);
        }
        #endregion protected methods

        #region internal methods
        internal void ValorizzaEtichette(IAventiDiritto iAventiDiritto)
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Spacchettate SOPGI
            if (this.areaDanteCausa == null)
            {
                PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
                presenterDanteCausa.GetDatiDanteCausa(this);
            }

            string controlloDinamico = string.Empty;
            string controlloDinamicoAbilitazioneSpacchettate024 = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamico);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                controlloDinamicoAbilitazioneSpacchettate024 = controlloDinamico;

            if (iAventiDiritto.AreaAventiDiritto != null && iAventiDiritto.AreaAventiDiritto.DatiAventiDiritto != null)
            {
                var listaAventiDiritto = iAventiDiritto.AreaAventiDiritto.DatiAventiDiritto.ListaAventiDiritto != null ?
                    iAventiDiritto.AreaAventiDiritto.DatiAventiDiritto.ListaAventiDiritto.ToList() : null;
                var listaAnagrafiche = iAventiDiritto.AreaAventiDiritto.DatiAventiDiritto.ListaAnagrafiche != null ?
                    iAventiDiritto.AreaAventiDiritto.DatiAventiDiritto.ListaAnagrafiche.ToList() : null;
                var listaDecodifiche = iAventiDiritto.AreaAventiDiritto.ElencoGradiParentela != null ?
                    iAventiDiritto.AreaAventiDiritto.ElencoGradiParentela.ToList() : null;

                // Se il numero di idAnagrafiche è minore del numero di record, significa che sono presenti dei doppioni
                if (listaAventiDiritto != null && listaAventiDiritto.GroupBy(x => x.IdAnagrafica).Select(x => x.First()).Count() < listaAventiDiritto.Count)
                    lblMsgSelezione.Visible = true;
                else
                    lblMsgSelezione.Visible = false;

                SetVSListaAventiDiritto(listaAventiDiritto);
                SetVSListaAnagrafiche(listaAnagrafiche);
                SetVSListaDecodiche(listaDecodifiche);
                gvDatiAventiDiritto_Load(listaAventiDiritto);
            }

            if (iAventiDiritto.AreaAventiDiritto != null && iAventiDiritto.AreaAventiDiritto.IsFascicoloGenerato == true)
                btnAggiornaDaArchivioPensione.Enabled = false;

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                btnAggiornaDaWebDom.Enabled = false;
            //se ricostituzione o riapertura disabilita

            if ((this.domanda.IsDomandaENPALS || this.domanda.IsDomandaINPDAP || (!string.IsNullOrEmpty(controlloDinamicoAbilitazioneSpacchettate024) && controlloDinamicoAbilitazioneSpacchettate024.ToUpperInvariant() == "SI"
                && Utility.IsDomandaSpacchettamento024(this.domanda.Tipofondo, this.domanda.Categoria, this.domanda.DataAcquisizione)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(this.domanda.Categoria, datiPensione, this.areaDanteCausa) ||
                Utility.IsDomandaSpacchettamentoSO(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, this.domanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, this.domanda) ||
                Utility.IsDomandaSpacchettamentoSR(datiPensione, this.domanda)) && Utility.IsDomandaSuperstiti(datiPensione))
                btnAggiornaDaWebDom.Visible = false;
        }

        internal AreaAventiDiritto RecuperaCampi()
        {
            AreaAventiDiritto areaAventiDiritto = new AreaAventiDiritto();
            areaAventiDiritto.DatiAventiDiritto = new Presenter.SvrLiquidazione.AventiDiritto();

            var listaAventiDiritto = GetVSListaAventiDiritto();
            var listaAnagrafiche = GetVSListaAnagrafiche();

            if (listaAventiDiritto != null)
            {
                foreach (RepeaterItem row in repAventiDiritto.Items)
                {
                    long idAventeDiritto = long.Parse(((HiddenField)row.FindControl(Keys.LblId)).Value);
                    GridView gvDatiPeriodi = (GridView)row.FindControl(Keys.GvDatiPeriodi);
                    foreach (GridViewRow rowDatiPeriodi in gvDatiPeriodi.Rows)
                    {
                        string cessazione = ((TextBox)rowDatiPeriodi.FindControl(Keys.TxtCessazionePeriodo_GrigliaPeriodi)).Text;
                        long idPeriodo = long.Parse(((Label)rowDatiPeriodi.FindControl(Keys.LblIdPeriodo_GrigliaPeriodi)).Text);
                        var listaPeriodiPerAventeDiritto = listaAventiDiritto.Find(x => x.Id == idAventeDiritto).ListaPeriodi;
                        if (listaPeriodiPerAventeDiritto != null)
                        {
                            var periodo = listaPeriodiPerAventeDiritto.First(x => x.Id == idPeriodo);
                            periodo.CessazionePeriodo = Utility.GetDateFromString(cessazione);
                        }
                    }

                    long id = long.Parse(((HiddenField)row.FindControl(Keys.LblId)).Value);
                    var aventeDiritto = listaAventiDiritto.Find(x => x.Id == id);
                    if (aventeDiritto != null)
                    {
                        CheckBox rdSelect = ((CheckBox)row.FindControl(Keys.RadioSelectionAventeDiritto));
                        if (rdSelect.Checked)
                            aventeDiritto.IsSelezionato = true;
                        else
                            aventeDiritto.IsSelezionato = false;

                        DropDownList ddlNucleo = (DropDownList)row.FindControl(Keys.DdlNucleo);
                        if (!string.IsNullOrEmpty(ddlNucleo.SelectedValue))
                        {
                            aventeDiritto.CodiceNucleo = ddlNucleo.SelectedValue;
                        }
                        else
                        {
                            aventeDiritto.CodiceNucleo = null;
                        }
                    }
                }
            }

            areaAventiDiritto.DatiAventiDiritto.ListaAventiDiritto = listaAventiDiritto != null ? listaAventiDiritto.ToArray() : null;
            areaAventiDiritto.DatiAventiDiritto.ListaAnagrafiche = listaAnagrafiche != null ? listaAnagrafiche.ToArray() : null;

            return areaAventiDiritto;
        }
        #endregion internal methods

        #region private methods
        private void gvDatiAventiDiritto_Load(List<GestioneAventiDirittoAventiDiritto> listaAventiDiritto)
        {
            if (listaAventiDiritto == null || listaAventiDiritto.Count == 0)
            {
                repAventiDiritto.Visible = false;
                lblNoAventiDiritto.Visible = true;
            }
            else
            {
                repAventiDiritto.Visible = true;
                lblNoAventiDiritto.Visible = false;
                repAventiDiritto.DataSource = listaAventiDiritto;
                repAventiDiritto.DataBind();
            }
        }

        private List<GestioneAventiDirittoAventiDiritto> GetVSListaAventiDiritto()
        {
            return ViewState[EnumViewState.ListaAventiDiritto.ToString()] != null ? (List<GestioneAventiDirittoAventiDiritto>)ViewState[EnumViewState.ListaAventiDiritto.ToString()] : null;
        }

        private void SetVSListaAventiDiritto(List<GestioneAventiDirittoAventiDiritto> listaAventiDiritto)
        {
            ViewState[EnumViewState.ListaAventiDiritto.ToString()] = listaAventiDiritto;
        }

        private List<GestioneAnagraficaDatiAnagrafici> GetVSListaAnagrafiche()
        {
            return ViewState[EnumViewState.ListaAnagrafiche.ToString()] != null ? (List<GestioneAnagraficaDatiAnagrafici>)ViewState[EnumViewState.ListaAnagrafiche.ToString()] : null;
        }

        private void SetVSListaAnagrafiche(List<GestioneAnagraficaDatiAnagrafici> listaAnagrafiche)
        {
            ViewState[EnumViewState.ListaAnagrafiche.ToString()] = listaAnagrafiche;
        }

        private List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> GetVSListaDecodiche()
        {
            return ViewState[EnumViewState.ListaDecodifiche.ToString()] != null ? (List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare>)ViewState[EnumViewState.ListaDecodifiche.ToString()] : null;
        }

        private void SetVSListaDecodiche(List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> listaAnagrafiche)
        {
            ViewState[EnumViewState.ListaDecodifiche.ToString()] = listaAnagrafiche;
        }

        private void ShowSelectionColumnRepAventiDiritto(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.ID == "itemRadioButton" || ctrl.ID == "headerRadioButton")
                    ctrl.Visible = true;
                else
                    ShowSelectionColumnRepAventiDiritto(ctrl);
            }
        }

        private void LoadDdlNucleo(DropDownList ddlNucleo, byte numRighe)
        {
            try
            {
                ListItem li = new ListItem();
                li.Text = "";
                li.Value = "";
                ddlNucleo.Items.Add(li);
                for (int i = 1; i <= numRighe; i++)
                {
                    li = new ListItem();
                    li.Text = "N" + i;
                    li.Value = "N" + i;
                    ddlNucleo.Items.Add(li);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAventiDiritto, Errore nel metodo LoadDdlNucleo " + ex);
            }
        }

        #endregion private methods

        #region repAventiDiritto
        protected void repAventiDiritto_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var listaAnagrafiche = GetVSListaAnagrafiche();
                    var listaDecodifiche = GetVSListaDecodiche();
                    var listaAventiDiritto = GetVSListaAventiDiritto();
                    GestioneAventiDirittoAventiDiritto aventeDiritto = (GestioneAventiDirittoAventiDiritto)e.Item.DataItem;
                    DropDownList ddlNucleo = ((DropDownList)e.Item.FindControl(Keys.DdlNucleo));
                    LoadDdlNucleo(ddlNucleo, (byte)listaAventiDiritto.Count);
                    ((Label)e.Item.FindControl(Keys.LblNucleoDaWebDom)).Text = CodeUtility.GetSI_NO(aventeDiritto.NucleoTitolare);
                    ((Label)e.Item.FindControl(Keys.LblNucleoDaArchivioPensione)).Text = aventeDiritto.CodiceNucleoFromGP;
                    if (aventeDiritto.CodiceNucleo != null)
                    {
                        ddlNucleo.SelectedValue = aventeDiritto.CodiceNucleo;
                    }
                    else if (aventeDiritto.CodiceNucleoFromGP != null)
                    {
                        ddlNucleo.SelectedValue = aventeDiritto.CodiceNucleoFromGP;
                    }
                    if (!String.IsNullOrEmpty(aventeDiritto.CategoriaPensione) && aventeDiritto.SedePensione.HasValue && aventeDiritto.CertificatoPensione.HasValue)
                    {
                        ((Label)e.Item.FindControl(Keys.LblPensione)).Text = aventeDiritto.CategoriaPensione + " - " + aventeDiritto.SedePensione.ToString().PadLeft(4, '0') + " - " + aventeDiritto.CertificatoPensione.ToString().PadLeft(8, '0');
                    }
                    long idAnagrafica = long.Parse(((HiddenField)e.Item.FindControl(Keys.LblIdAnagrafica)).Value);
                    var anagrafica = listaAnagrafiche.Find(x => x.Id == idAnagrafica);

                    ((Label)e.Item.FindControl(Keys.LblNome)).Text = anagrafica.Nome;
                    ((Label)e.Item.FindControl(Keys.LblCognome)).Text = anagrafica.Cognome;
                    ((Label)e.Item.FindControl(Keys.LblCodiceFiscale)).Text = anagrafica.CodiceFiscale;
                    ((Label)e.Item.FindControl(Keys.LblRelazioneDA)).Text = String.Format("{0} - {1}", aventeDiritto.DecParentelaDA, listaDecodifiche.Exists(x => x.Id ==
                        aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString() && x.TipoUnione == aventeDiritto.TipoUnione) ? listaDecodifiche.First(x => x.Id ==
                        aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString() && x.TipoUnione == aventeDiritto.TipoUnione).Descrizione : string.Empty);

                    if (listaAventiDiritto.Count(x => x.IdAnagrafica == idAnagrafica) > 1)
                    {
                        CheckBox radio = (CheckBox)e.Item.FindControl(Keys.RadioSelectionAventeDiritto);
                        radio.Visible = true;
                        radio.CssClass += anagrafica.CodiceFiscale;
                        radio.Attributes.Add("onclick", "SetUniqueRadioButton('" + anagrafica.CodiceFiscale + "', this);");
                        columnSelectionVisible = true;

                        if (listaAventiDiritto.Exists(x => x.IdAnagrafica == idAnagrafica && (!string.IsNullOrEmpty(x.CategoriaPensione) || x.SedePensione.HasValue || x.CertificatoPensione.HasValue)) &&
                            aventeDiritto.PresenzaWebDom)
                            radio.Enabled = false;
                    }

                    GridView gvDatiPeriodi = (GridView)e.Item.FindControl(Keys.GvDatiPeriodi);
                    gvDatiPeriodi.DataSource = aventeDiritto.ListaPeriodi;
                    gvDatiPeriodi.DataBind();
                }
                else if (e.Item.ItemType == ListItemType.Footer)
                {
                    if (columnSelectionVisible)
                    {
                        ShowSelectionColumnRepAventiDiritto(repAventiDiritto);
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAventiDiritto, Errore nel metodo repAventiDiritto_ItemDataBound " + ex);
            }
        }
        #endregion repAventiDiritto

        #region gvDatiPeriodi
        protected void gvDatiPeriodi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    AreaRispostaRiepilogo.DatiRiepilogoAnagrafica Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

                    var listaDecodifiche = GetVSListaDecodiche();
                    var listaAventiDiritto = GetVSListaAventiDiritto();
                    var listaAnagrafiche = GetVSListaAnagrafiche();

                    GestionePeriodiAventiDirittoPeriodoAventiDiritto periodo = (GestionePeriodiAventiDirittoPeriodoAventiDiritto)e.Row.DataItem;
                    ((Label)e.Row.FindControl("lblGradoParentela")).Text = String.Format("{0} - {1}", periodo.GradoParentela, listaDecodifiche.Exists(x => x.Id ==
                        periodo.GradoParentela.GetValueOrDefault().ToString()) ? listaDecodifiche.First(x => x.Id ==
                        periodo.GradoParentela.GetValueOrDefault().ToString()).Descrizione : string.Empty);

                    GestioneAventiDirittoAventiDiritto aventeDiritto = listaAventiDiritto.Find(x => x.Id == periodo.IdAventeDiritto);
                    GestioneAnagraficaDatiAnagrafici anagraficaAventeDiritto = listaAnagrafiche.Find(x => x.Id == aventeDiritto.IdAnagrafica);

                    if (!string.IsNullOrEmpty(aventeDiritto.CategoriaPensione) || aventeDiritto.SedePensione.HasValue || aventeDiritto.CertificatoPensione.HasValue || anagraficaAventeDiritto.CodiceFiscale == Anagrafica.CodiceFiscale)
                        ((TextBox)e.Row.FindControl(Keys.TxtCessazionePeriodo_GrigliaPeriodi)).Enabled = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAventiDiritto, Errore nel metodo gvDatiPeriodi_RowDataBound " + ex);
            }
        }
        #endregion gvDatiPeriodi

        #region enum
        public enum EnumViewState
        {
            ListaAventiDiritto,
            ListaAnagrafiche,
            ListaDecodifiche
        }
        #endregion enum

        #region nested classes
        public class Keys
        {
            public const string LblIdAnagrafica = "hdnIdAnagrafica";
            public const string LblId = "hdnId";
            public const string LblIdPeriodo_GrigliaPeriodi = "lblIdPeriodo";
            public const string RadioSelectionAventeDiritto = "chkSelect";
            public const string GvDatiPeriodi = "gvDatiPeriodi";
            public const string TxtCessazionePeriodo_GrigliaPeriodi = "txtCessazionePeriodo";
            public const string DdlNucleo = "ddlNucleo";
            public const string LblNucleoDaWebDom = "lblNucleoDaWebDom";
            public const string LblNucleoDaArchivioPensione = "lblNucleoDaArchivioPensione";
            public const string LblPensione = "lblPensione";
            public const string LblNome = "lblNome";
            public const string LblCognome = "lblCognome";
            public const string LblCodiceFiscale = "lblCodiceFiscale";
            public const string LblRelazioneDA = "lblRelazioneDA";
        }
        #endregion nested classes

        #region Events
        public event EventHandler ShowAvviso;
        public event EventHandler AggiornaSemaforo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseAggiornaSemaforo(object sender, EventArgs e)
        {
            if (AggiornaSemaforo != null)
                AggiornaSemaforo(sender, e);
        }
        #endregion Events
    }
}
