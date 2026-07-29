using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo
{
    public partial class UCDatiEsteri : CustomBaseUserControl, IDatiContributiviAgo, ITitolarePensione
    {
        private enum EnumViewState { ElencoImportiEsteri, ElencoDatiEsteri, DatiContributiviAgo }

        #region IDatiContributiviAgo
        public Presenter.SvrLiquidazioneAgo.AreaDatiContributivi areaDatiContributiviAgo { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviAgo

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        private const int indiceStati = 6;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

                if (this.areaDatiContributiviAgo != null)
                {
                    ViewState[EnumViewState.DatiContributiviAgo.ToString()] = this.areaDatiContributiviAgo;

                    if (this.areaDatiContributiviAgo.ProRata == null)
                    {
                        pnlDatiEsteriEditMode.Visible = true;
                        BindDataDatiEsteri();
                    }
                    else
                    {
                        ValorizzaListaStatiEsteri(this);
                    }
                }
            }
        }

        protected void ConfermaModifiche_Click(object sender, EventArgs e)
        {
            RaiseNascondiAvviso(this, null);
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviAgo = ConfermaModificheProrata(Convert.ToInt16(hdnNRecordProrata.Value));

            if (!((this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.IsDatiEsteriFromServices.GetValueOrDefault()) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)) && !this.HasError)
            {
                List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = ((List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()]);
                if (this.areaDatiContributiviAgo != null)
                    this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri = elencoDatiEsteri.ToArray();
            }

            if (this.HasError)
            {
                RaiseShowAvvisoDatiProRata(this, null);
            }
            else
            {
                presenterDatiContributiviAgo.SalvaTabDatiEsteri(this);
                ViewState[EnumViewState.DatiContributiviAgo.ToString()] = this.areaDatiContributiviAgo;
                if (!this.HasError)
                {
                    HideDatiStato();
                }
                RaiseShowAvvisoDatiProRata(this, null);
            }
        }

        protected void AnnullaModifiche_Click(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RaiseNascondiAvviso(this, null);
            editpan.Visible = false;
            pnlTable.Visible = true;
            hdnIsInProrata.Value = "false";
            this.areaDatiContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()];
            List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = ((List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()]);
            List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];

            if ((this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.IsDatiEsteriFromServices.GetValueOrDefault()) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) ||
                 elencoDatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).PrestazioneEsteraCumulo._Confermato == true)
            {
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> lista = this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteriCumulo.ToList();
                lista = EliminaRecordImportiVuoti(lista);
                this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteriCumulo = lista.ToArray();
                ViewState[EnumViewState.DatiContributiviAgo.ToString()] = this.areaDatiContributiviAgo;

            }

            if (modalitaEditImporti.Value == "true")
            {
                gvImportiEsteri.EditIndex = -1;
                modalitaEditImporti.Value = "false";
            }

            if (!((this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.IsDatiEsteriFromServices.GetValueOrDefault()) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)))
                divEliminaProrata.Visible = true;
        }

        protected void btnCancelProRata_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            presenterDatiContributiviAgo.EliminaDatiEsteri(this);

            if (!this.HasError)
            {
                presenterDatiContributiviAgo.GetDatiContributivi(this);
                ValorizzaGridStatiEsteriVuota();///
                divEliminaProrata.Visible = false;
                RaiseShowAvvisoEliminaDatiProRata(this, null);
            }
            else
            {
                this.ErrorMessage = ErrorMessage;
                if (string.IsNullOrEmpty(this.ErrorMessage))
                    this.ErrorMessage = "Non ci sono Dati Pro Rata da eliminare";
                RaiseShowAvvisoEliminaDatiProRata(this, null);
            }

        }

        internal AreaDatiContributivi ConfermaModificheProrata(int indiceProrata)
        {
            AreaDatiContributivi area = ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()]);

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            GestioneContribPrestazioneEsteraCumulo prestazioneEstera = GetDatiPrestazioneEstera();
            List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> importiEsteri = GetDatiImportiEsteri();
            if (this.HasError)
                return null;

            if ((area != null && area.IsDatiEsteriFromServices.GetValueOrDefault()) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                area.ProRata.ElencoStatiEsteri.ElementAt(indiceProrata).PrestazioneEsteraCumulo = prestazioneEstera;
                importiEsteri = EliminaRecordImportiVuoti(importiEsteri);
                area.ProRata.ElencoStatiEsteri.ElementAt(indiceProrata).ElencoImportiEsteriCumulo = importiEsteri.ToArray();
            }
            else
            {
                List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = ((List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()]);
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];

                elencoDatiEsteri = EliminaRecordStatiVuoti(elencoDatiEsteri);
                elencoDatiEsteri.ElementAt(indiceProrata).PrestazioneEsteraCumulo = prestazioneEstera;

                elencoImportiEsteri = EliminaRecordImportiVuoti(importiEsteri);
                elencoDatiEsteri.ElementAt(indiceProrata).ElencoImportiEsteriCumulo = elencoImportiEsteri.ToArray();
            }

            return area;
        }

        internal AreaDatiContributivi ConfermaModificheProrata()
        {
            return ConfermaModificheProrata(Convert.ToInt16(hdnNRecordProrata.Value));
        }

        private List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> GetDatiImportiEsteri()
        {
            if (gvImportiEsteri.EditIndex != -1)
            {
                TextBox txtDecorrenzaImportiEE = (TextBox)gvImportiEsteri.Rows[gvImportiEsteri.EditIndex].FindControl("txtDecorrenzaPrestazioneEE");
                TextBox txtImportoPrestazioneEE = (TextBox)gvImportiEsteri.Rows[gvImportiEsteri.EditIndex].FindControl("txtImportoPrestazioneEE");
                TextBox txtCessazione = (TextBox)gvImportiEsteri.Rows[gvImportiEsteri.EditIndex].FindControl("txtCessazionePrestazioneEE");

                if (!String.IsNullOrEmpty(txtDecorrenzaImportiEE.Text) || !String.IsNullOrEmpty(txtImportoPrestazioneEE.Text) || !String.IsNullOrEmpty(txtCessazione.Text))
                {
                    this.HasError = true;
                    this.ErrorMessage = "Per proseguire è necessario confermare l'inserimento dei dati relativi al prorata";
                    return null;
                }
            }

            List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];
            List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> importiEsteri = new List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>();
            foreach (GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo importoEsteroVS in elencoImportiEsteri)
            {
                GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo importoEstero = new GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo();
                importoEstero.DecorrenzaPrestazione = importoEsteroVS.DecorrenzaPrestazione == DateTime.MinValue ? null : importoEsteroVS.DecorrenzaPrestazione;
                importoEstero.CessazionePrestazione = importoEsteroVS.CessazionePrestazione == DateTime.MinValue ? null : importoEsteroVS.CessazionePrestazione;
                importoEstero.ImportoPrestazione = importoEsteroVS.ImportoPrestazione;
                importiEsteri.Add(importoEstero);
            }

            return importiEsteri;
        }

        private GestioneContribPrestazioneEsteraCumulo GetDatiPrestazioneEstera()
        {
            GestioneContribPrestazioneEsteraCumulo prestazioneEstera = new GestioneContribPrestazioneEsteraCumulo();
            if (!String.IsNullOrEmpty(lblIdPrestazioneEE.Text))
                prestazioneEstera._Id = long.Parse(lblIdPrestazioneEE.Text);
            if (!String.IsNullOrEmpty(lblNomeStato.Text))
                prestazioneEstera.NomeStato = lblNomeStato.Text;
            if (!String.IsNullOrEmpty(lblCodiceStatoEE.Text))
                prestazioneEstera._CodiceStato = lblCodiceStatoEE.Text;
            if (!String.IsNullOrEmpty(lblCodiceIstituzione.Text))
                prestazioneEstera._CodiceIstituzione = lblCodiceIstituzione.Text;
            if (!String.IsNullOrEmpty(lblCitta.Text))
                prestazioneEstera.Citta = lblCitta.Text;
            if (!String.IsNullOrEmpty(lblSigla.Text))
                prestazioneEstera.Sigla = lblSigla.Text;
            if (!String.IsNullOrEmpty(txtSettimaneMisuraDecorrenzaPensione.Text))
                prestazioneEstera._SettimaneMisura = Int32.Parse(txtSettimaneMisuraDecorrenzaPensione.Text);
            if (!String.IsNullOrEmpty(txtSettimaneDiritto.Text))
                prestazioneEstera._ContributiDiritto = Int32.Parse(txtSettimaneDiritto.Text);
            //elemento è stato modificato quindi imposto il flag confermato a false
            prestazioneEstera._Confermato = true;

            return prestazioneEstera;
        }

        private GestioneContribPrestazioneEsteraCumulo GetDatiPrestazioneEsteraNull()
        {
            GestioneContribPrestazioneEsteraCumulo prestazioneEstera = new GestioneContribPrestazioneEsteraCumulo();

            if (!String.IsNullOrEmpty(txtSettimaneMisuraDecorrenzaPensione.Text))
                prestazioneEstera._SettimaneMisura = null;
            if (!String.IsNullOrEmpty(txtSettimaneDiritto.Text))
                prestazioneEstera._ContributiDiritto = null;

            return prestazioneEstera;
        }


        internal GestioneContribProRata GetDatiProRata()
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            areaDatiContributiviAgo = ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()]);

            if (this.editpan.Visible)
            {
                GestioneContribPrestazioneEsteraCumulo prestazioneEstera = GetDatiPrestazioneEstera();
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> importiEsteri = GetDatiImportiEsteri();
                if (this.HasError)
                    return null;

                if ((this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.IsDatiEsteriFromServices.GetValueOrDefault()) || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).PrestazioneEsteraCumulo = prestazioneEstera;
                    importiEsteri = EliminaRecordImportiVuoti(importiEsteri);
                    areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteriCumulo = importiEsteri.ToArray();
                }
                else
                {
                    List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = ((List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()]);
                    List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];

                    elencoDatiEsteri = EliminaRecordStatiVuoti(elencoDatiEsteri);
                    elencoDatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).PrestazioneEsteraCumulo = prestazioneEstera;

                    elencoImportiEsteri = EliminaRecordImportiVuoti(importiEsteri);
                    elencoDatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteriCumulo = elencoImportiEsteri.ToArray();

                    this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri = elencoDatiEsteri.ToArray();
                }
            }

            if (areaDatiContributiviAgo.ProRata != null && areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null)
            {
                foreach (GestioneContribStatoEsteroCumulo stato in this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri)
                {
                    stato.PrestazioneEsteraCumulo._Confermato = true;
                    int index = stato.ElencoImportiEsteriCumulo.ToList().FindIndex(delegate(GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo ie) { return (ie.CessazionePrestazione == DateTime.MinValue && ie.DecorrenzaPrestazione == DateTime.MinValue && ie.ImportoPrestazione == decimal.MinValue); });
                    if (index >= 0)
                    {
                        List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> importiEE = new List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>();
                        importiEE = stato.ElencoImportiEsteriCumulo.ToList();
                        importiEE.RemoveAt(index);
                        stato.ElencoImportiEsteriCumulo = importiEE.ToArray();
                    }
                }
            }
            return ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()]).ProRata;
        }

        internal GestioneContribProRata GetViewStateProRata()
        {
            return ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()]).ProRata;
        }

        internal void HideDatiStato()
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ViewState.Remove(EnumViewState.ElencoImportiEsteri.ToString());
            editpan.Visible = false;
            pnlTable.Visible = true;
            hdnIsInProrata.Value = "false";
            if (modalitaEditImporti.Value == "true")
            {
                gvImportiEsteri.EditIndex = -1;
                modalitaEditImporti.Value = "false";
            }
            ValorizzaListaStatiEsteri(this);
        }

        private void ValorizzaEtichetteProRataEstera(GestioneContribPrestazioneEsteraCumulo prestazioneEstera)
        {
            lblIdPrestazioneEE.Text = prestazioneEstera._Id.ToString();
            lblCodiceIstituzione.Text = prestazioneEstera._CodiceIstituzione;
            lblCodiceStatoEE.Text = prestazioneEstera._CodiceStato;
            lblNomeStato.Text = prestazioneEstera.NomeStato;
            lblCitta.Text = prestazioneEstera.Citta;
            lblSigla.Text = prestazioneEstera.Sigla;
            if (!String.IsNullOrEmpty(prestazioneEstera._SettimaneMisura.ToString()))
                txtSettimaneMisuraDecorrenzaPensione.Text = prestazioneEstera._SettimaneMisura.ToString();
            else
                txtSettimaneMisuraDecorrenzaPensione.Text = "";
            if (!String.IsNullOrEmpty(prestazioneEstera._ContributiDiritto.ToString()))
                txtSettimaneDiritto.Text = prestazioneEstera._ContributiDiritto.ToString();
            else
                txtSettimaneDiritto.Text = "";
        }

        #region gvDatiEsteri

        protected List<GestioneContribStatoEsteroCumulo> BindDataDatiEsteri()
        {
            List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = (List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()];

            if (elencoDatiEsteri == null ||
                elencoDatiEsteri.Count() == 0)
                elencoDatiEsteri = CreaRecordStatiEsteri();
            else
            {
                EliminaRecordStatiVuoti(elencoDatiEsteri);
                elencoDatiEsteri = AggiungiRecordDatiEsteri(elencoDatiEsteri, null, null);
            }
            ViewState[EnumViewState.ElencoDatiEsteri.ToString()] = elencoDatiEsteri;
            divEliminaProrata.Visible = true;

            List<extAreaDatiEsteri> extListAreaDatiEsteri = new List<extAreaDatiEsteri>();
            foreach (GestioneContribStatoEsteroCumulo statoEstero in elencoDatiEsteri)
            {
                extAreaDatiEsteri myExt = new extAreaDatiEsteri(statoEstero);
                extListAreaDatiEsteri.Add(myExt);
            }
            gvDatiEsteri.DataSource = extListAreaDatiEsteri;
            gvDatiEsteri.DataKeyNames = new String[] { "strCodiceStato" };
            gvDatiEsteri.DataBind();
            return elencoDatiEsteri;
        }

        private List<GestioneContribStatoEsteroCumulo> CreaRecordStatiEsteri()
        {
            List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = new List<GestioneContribStatoEsteroCumulo>();
            elencoDatiEsteri = AggiungiRecordDatiEsteri(elencoDatiEsteri, null, null);
            return elencoDatiEsteri;
        }

        private List<GestioneContribStatoEsteroCumulo> AggiungiRecordDatiEsteri(List<GestioneContribStatoEsteroCumulo> listaRecord, string codiceStato, string CodiceIstituzione)
        {
            GestioneContribStatoEsteroCumulo record = new GestioneContribStatoEsteroCumulo();
            record.PrestazioneEsteraCumulo = new GestioneContribPrestazioneEsteraCumulo();
            record.PrestazioneEsteraCumulo._CodiceStato = codiceStato;
            record.PrestazioneEsteraCumulo._CodiceIstituzione = CodiceIstituzione;
            if (listaRecord.Count(x => x.PrestazioneEsteraCumulo != null && !string.IsNullOrEmpty(x.PrestazioneEsteraCumulo._CodiceStato)) < indiceStati)
                listaRecord.Add(record);
            return listaRecord;
        }

        protected void gvDatiEsteri_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvDatiEsteri.EditIndex = e.NewEditIndex;
                BindDataDatiEsteri();
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowEditing " + ex);
            }
        }

        protected void gvDatiEsteri_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvDatiEsteri.PageIndex = e.NewPageIndex;
                BindDataDatiEsteri();
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_onPageIndexChanging" + ex);
            }
        }

        protected void gvDatiEsteri_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<GestioneContribStatoEsteroCumulo> elencoStatiEsteri = (List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()];
                GridViewRow row = gvDatiEsteri.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvDatiEsteri.PageIndex * 10) + e.RowIndex);
                    if (elencoStatiEsteri.Count != i + 1)
                        elencoStatiEsteri.RemoveAt(elencoStatiEsteri.Count - 1);
                    gvDatiEsteri.EditIndex = -1;

                    BindDataDatiEsteri();
                }
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowUpdating " + ex);
            }
        }

        protected void gvDatiEsteri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvDatiEsteri.EditIndex = -1;
                BindDataDatiEsteri();
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvDatiEsteri_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ricerca")
            {
                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                hdnNRecordProrata.Value = Convert.ToInt16(e.CommandArgument).ToString();
                RaiseNascondiAvviso(this, null);
                areaDatiContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()];
                string descStato = string.Empty;
                string descIstituzione = string.Empty;
                string descCittà = string.Empty;
                List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = ((List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()]);
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];
                if (elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0)
                {
                    Presenter.PresenterDatiContributiviAGO presenter = new PresenterDatiContributiviAGO();
                    if (elencoDatiEsteri[Convert.ToInt16(e.CommandArgument)].PrestazioneEsteraCumulo._Confermato == true)
                    {
                        GestioneContribStatoEsteroCumulo statoEstero = elencoDatiEsteri[Convert.ToInt16(e.CommandArgument)];
                        ValorizzaEtichetteProRataEstera(statoEstero.PrestazioneEsteraCumulo);

                        Presenter.SvrLiquidazioneAgo.AreaEsito esito = presenter.ControlsCompatibilitàCodiceConvenzioneWithStatoEstero(this, statoEstero);
                        if (esito != null && !string.IsNullOrEmpty(esito.Messaggio))
                        {
                            ValorizzaEtichetteStatoIstituzione(Convert.ToInt32(hdnNRecordProrata.Value), out descStato, out descIstituzione, out descCittà);
                            this.HasError = true;
                            this.ErrorMessage = esito.Messaggio + descStato;
                            RaiseShowAvvisoDatiProRata(this, null);
                            return;
                        }

                        List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> lstCopy = new List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>();
                        foreach (GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo elem in statoEstero.ElencoImportiEsteriCumulo)
                            lstCopy = AggiungiRecordImporti(lstCopy, elem.ImportoPrestazione, elem.CessazionePrestazione, elem.DecorrenzaPrestazione);
                        ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = lstCopy;
                    }
                    else
                    {
                        ValorizzaEtichetteStatoIstituzione(Convert.ToInt32(hdnNRecordProrata.Value), out descStato, out descIstituzione, out descCittà);
                        if (string.IsNullOrEmpty(descStato))
                        {
                            this.HasError = true;
                            this.ErrorMessage = "STATO O/E ISTITUZIONE NON ESITENTI";
                            RaiseShowAvvisoDatiProRata(this, null);
                            return;
                        }
                        else
                        {
                            if (elencoImportiEsteri != null)
                            {
                                GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo[] imp = elencoImportiEsteri.ToArray();
                                elencoDatiEsteri[Convert.ToInt16(e.CommandArgument)].ElencoImportiEsteriCumulo = null;
                                imp = elencoDatiEsteri[Convert.ToInt16(e.CommandArgument)].ElencoImportiEsteriCumulo;
                                this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri = elencoDatiEsteri.ToArray();
                                ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = imp;
                            }
                            ViewState[EnumViewState.DatiContributiviAgo.ToString()] = areaDatiContributiviAgo;
                        }

                        Presenter.SvrLiquidazioneAgo.AreaEsito esito = presenter.ControlsCompatibilitàCodiceConvenzioneWithStatoEstero(this, elencoDatiEsteri[Convert.ToInt16(e.CommandArgument)]);
                        if (esito != null && !string.IsNullOrEmpty(esito.Messaggio))
                        {
                            this.HasError = true;
                            this.ErrorMessage = esito.Messaggio + descStato;
                            RaiseShowAvvisoDatiProRata(this, null);
                            return;
                        }
                    }
                }
                lblNomeStato.Visible = true;
                lblCodiceIstituzione.Visible = true;
                divEliminaProrata.Visible = false;
                editpan.Visible = true;
                pnlTable.Visible = false;
                hdnIsInProrata.Value = "true";
                BindDataImportiEsteri(hdnNRecordProrata.Value);
            }
            else if (e.CommandName == "Elimina")
            {
                RaiseNascondiAvviso(this, null);
                List<GestioneContribStatoEsteroCumulo> listaDatiEsteri = BindDataDatiEsteri();
                GestioneContribStatoEsteroCumulo[] elencoDatiEsteri = listaDatiEsteri.ToArray();
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (listaDatiEsteri.Count == 0)
                {
                    elencoDatiEsteri[0].PrestazioneEsteraCumulo._CodiceStato = null;
                    elencoDatiEsteri[0].PrestazioneEsteraCumulo._CodiceIstituzione = null;
                }
                else
                {
                    if (row.DataItemIndex == 0)
                        modalitaEditStatiEsteri.Value = "false";
                    EliminaDatiEsteri(elencoDatiEsteri[Convert.ToInt16(e.CommandArgument)].PrestazioneEsteraCumulo._Id);
                    listaDatiEsteri.RemoveAt(row.DataItemIndex);
                }
                ViewState[EnumViewState.ElencoDatiEsteri.ToString()] = listaDatiEsteri;
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                if (listaDatiEsteri.Count() == 1)
                    this.areaQuadri.QuadroDatiContributivi.TabDatiEsteri = AreaQuadri.Semaforo.Rosso_Abilitato;
                BindDataDatiEsteri();
            }
            else if (e.CommandName == "Edit")
            {
            }
            else if (e.CommandName == "Salva")
            {
                RaiseNascondiAvviso(this, null);
                if (!IsEmptyEditableRowStatiEE((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<GestioneContribStatoEsteroCumulo> listaDatiEsteri = BindDataDatiEsteri();
                    GestioneContribStatoEsteroCumulo[] elencoDatiiEsteri = listaDatiEsteri.ToArray();

                    string valueStato = string.Empty;
                    string valueIstituzione = string.Empty;
                    int index = int.Parse(((string)(e.CommandArgument)));

                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

                    TextBox txtStato = (TextBox)row.FindControl("txtCodiceStato");
                    if (!String.IsNullOrEmpty(txtStato.Text))
                        valueStato = txtStato.Text;

                    TextBox txtIstituzione = (TextBox)row.FindControl("txtCodiceIstituzione");
                    if (!String.IsNullOrEmpty(txtIstituzione.Text))
                        valueIstituzione = txtIstituzione.Text;

                    if (!String.IsNullOrEmpty(valueIstituzione) && valueIstituzione.Length < 4)
                    {
                        this.HasError = true;
                        this.ErrorMessage = "Il campo Istituzione deve contenere 4 cifre (compreso eventuale 0 iniziale)";
                        RaiseShowAvvisoDatiProRata(this, null);
                        return;
                    }

                    if ((row.DataItemIndex - 1) == (elencoDatiiEsteri.Length - 2) && elencoDatiiEsteri.Count(x => x.PrestazioneEsteraCumulo != null && !string.IsNullOrEmpty(x.PrestazioneEsteraCumulo._CodiceStato)) < indiceStati)    //aggiunta riga (non si tratta di una modifica)
                    {
                        listaDatiEsteri = AggiungiRecordDatiEsteri(listaDatiEsteri, valueStato, valueIstituzione);
                        gvDatiEsteri.EditIndex = -1;
                    }
                    else   //modifica elemento
                    {
                        elencoDatiiEsteri[row.DataItemIndex].PrestazioneEsteraCumulo._CodiceStato = valueStato;
                        elencoDatiiEsteri[row.DataItemIndex].PrestazioneEsteraCumulo._CodiceIstituzione = valueIstituzione;
                        gvDatiEsteri.EditIndex = -1;
                    }

                    ViewState[EnumViewState.ElencoDatiEsteri.ToString()] = listaDatiEsteri;
                    BindDataDatiEsteri();
                }
            }
            else if (e.CommandName == "Cancel")
            {
                modalitaEditStatiEsteri.Value = "false";
                gvDatiEsteri.EditIndex = -1;
            }
        }

        protected void gvDatiEsteri_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string currentTheme = Page.Theme;
                List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = ((List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()]);
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        if (elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0 && elencoDatiEsteri.Count == 1 &&
                            string.IsNullOrEmpty(elencoDatiEsteri.First().PrestazioneEsteraCumulo._CodiceStato) && string.IsNullOrEmpty(elencoDatiEsteri.First().PrestazioneEsteraCumulo._CodiceIstituzione))
                        {   //unica riga vuota, partenza in modalità edit

                            if (modalitaEditStatiEsteri.Value == "false")
                            {
                                gvDatiEsteri.EditIndex = 0;
                                modalitaEditStatiEsteri.Value = "true";
                                BindDataDatiEsteri();
                                Image img = (Image)e.Row.FindControl("img");
                                if ((elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0 && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true) ||
                                    (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.ProRata != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0 &&
                                     this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true))
                                {
                                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                    img.ToolTip = "Salvato";
                                }
                                else
                                {

                                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                    img.ToolTip = "Non Salvato";
                                }
                            }
                        }

                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            TextBox txtCodiceStato = (TextBox)e.Row.FindControl("txtCodiceStato");
                            TextBox txtCodiceIstituzione = (TextBox)e.Row.FindControl("txtCodiceIstituzione");

                            LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));

                            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                            cancel.ToolTip = "Annulla";
                            LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                            save.ToolTip = "Salva";
                            save.CausesValidation = true;
                            save.ValidationGroup = "UCTabDatiEsteriGrid2";
                            save.CommandName = "Salva";
                            setCampiEditStatiEsteri(e, true);

                            Image img = (Image)e.Row.FindControl("img");
                            if ((elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0 && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true) ||
                                (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.ProRata != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0 &&
                                 this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true))
                            {
                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                img.ToolTip = "Salvato";
                            }
                            else
                            {

                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                img.ToolTip = "Non Salvato";
                            }
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteStati")));
                            int index = e.Row.DataItemIndex;
                            if (index >= 0 && index <= elencoDatiEsteri.Count - 2)
                            {
                                edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                edit.ToolTip = "Modifica";

                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
                            }

                            ((Button)e.Row.FindControl("btnRicerca")).Enabled = true;

                            Image img = (Image)e.Row.FindControl("img");
                            if ((elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0 && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true) ||
                               (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.ProRata != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0 &&
                               this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true))
                            {
                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                img.ToolTip = "Salvato";
                            }
                            else
                            {

                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                img.ToolTip = "Non Salvato";
                            }
                        }
                    }
                    else   //record successivi al primo
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)    //modalità edit
                        {
                            TextBox txtCodiceStato = (TextBox)e.Row.FindControl("txtCodiceStato");
                            TextBox txtCodiceIstituzione = (TextBox)e.Row.FindControl("txtCodiceIstituzione");
                            LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                            cancel.ToolTip = "Annulla";
                            LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                            save.ToolTip = "Salva";
                            save.CausesValidation = true;
                            save.ValidationGroup = "UCTabDatiEsteriGrid2";
                            save.CommandName = "Salva";
                            setCampiEditStatiEsteri(e, false);

                            Image img = (Image)e.Row.FindControl("img");
                            if ((elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0 && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true) ||
                                     (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.ProRata != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0 &&
                                      this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true))
                            {
                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                img.ToolTip = "Salvato";
                            }
                            else
                            {

                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                img.ToolTip = "Non Salvato";
                            }
                        }
                        else
                        {
                            if (e.Row.DataItemIndex == elencoDatiEsteri.Count - 1 && elencoDatiEsteri.Count(x => x.PrestazioneEsteraCumulo != null && !string.IsNullOrEmpty(x.PrestazioneEsteraCumulo._CodiceStato)) < indiceStati)
                            {
                                LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                                add.ToolTip = "Aggiungi";

                                Image img = (Image)e.Row.FindControl("img");
                                img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                img.ToolTip = "Non Salvato";
                            }
                            else
                            {
                                LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                                LinkButton delete = ((LinkButton)(e.Row.Cells[5].FindControl("btnDeleteStati")));
                                int index = e.Row.DataItemIndex;
                                if (index >= 0 &&
                                    (index <= elencoDatiEsteri.Count - 2 || elencoDatiEsteri.Count(x => x.PrestazioneEsteraCumulo != null && !string.IsNullOrEmpty(x.PrestazioneEsteraCumulo._CodiceStato)) <= indiceStati))
                                {
                                    edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                    edit.ToolTip = "Modifica";

                                    delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                    delete.ToolTip = "Elimina";
                                }
                                ((Button)e.Row.FindControl("btnRicerca")).Enabled = true;

                                Image img = (Image)e.Row.FindControl("img");
                                if ((elencoDatiEsteri != null && elencoDatiEsteri.Count() > 0 && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && elencoDatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true) ||
                                     (this.areaDatiContributiviAgo != null && this.areaDatiContributiviAgo.ProRata != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0 &&
                                      this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.HasValue && this.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato.Value == true))
                                {
                                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                                    img.ToolTip = "Salvato";
                                }
                                else
                                {

                                    img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                                    img.ToolTip = "Non Salvato";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvImportiEsteri_RowDataBound " + ex);
            }
        }

        private bool IsEmptyEditableRowStatiEE(GridViewRow row)
        {
            if (row.FindControl("txtCodiceStato") != null && ((TextBox)row.FindControl("txtCodiceStato")).Text != string.Empty &&
                row.FindControl("txtCodiceIstituzione") != null && ((TextBox)row.FindControl("txtCodiceIstituzione")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void setCampiEditStatiEsteri(GridViewRowEventArgs e, bool IsFirstRecord)
        {
            TextBox txtStato = (TextBox)e.Row.FindControl("txtCodiceStato");
            TextBox txtIstituzione = (TextBox)e.Row.FindControl("txtCodiceIstituzione");
        }

        private List<GestioneContribStatoEsteroCumulo> EliminaRecordStatiVuoti(List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri)
        {
            int i = 0; int j = 0;
            int[] elementiDaEliminare = new int[30];
            foreach (GestioneContribStatoEsteroCumulo statoEstero in elencoDatiEsteri)
            {
                if (string.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo._CodiceStato) && string.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo._CodiceIstituzione))
                {
                    elementiDaEliminare[j] = i;
                    j++;
                }
                i++;
            }

            for (int z = 0; z < j; z++)
            {
                if (elementiDaEliminare[z] <= elencoDatiEsteri.Count - 1)
                    elencoDatiEsteri.RemoveAt(elementiDaEliminare[z]);
            }
            return elencoDatiEsteri;
        }

        private List<GestioneContribStatoEsteroCumulo> EliminaRecordStati(List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri)
        {
            int i = 0; int j = 0;
            int[] elementiDaEliminare = new int[30];
            foreach (GestioneContribStatoEsteroCumulo statoEstero in elencoDatiEsteri)
            {
                if (!string.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo._CodiceStato) && !string.IsNullOrEmpty(statoEstero.PrestazioneEsteraCumulo._CodiceIstituzione))
                {
                    elementiDaEliminare[j] = i;
                    j++;
                }
                i++;
            }

            for (int z = 0; z < j; z++)
            {
                if (elementiDaEliminare[z] <= elencoDatiEsteri.Count - 1)
                    elencoDatiEsteri.RemoveAt(elementiDaEliminare[z]);
            }
            return elencoDatiEsteri;
        }

        private void ValorizzaEtichetteStatoIstituzione(int indiceProrata, out string descStato, out string descIstituzione, out string descCittà)
        {
            descStato = string.Empty;
            descIstituzione = string.Empty;
            descCittà = string.Empty;

            Label txtCodiceStatoEE = (Label)gvDatiEsteri.Rows[indiceProrata].FindControl("lblCodiceStato");
            Label txtCodiceIstituzioneEE = (Label)gvDatiEsteri.Rows[indiceProrata].FindControl("lblCodiceIstituzione");

            if (!String.IsNullOrEmpty(txtCodiceStatoEE.Text) && !String.IsNullOrEmpty(txtCodiceIstituzioneEE.Text))
            {
                Presenter.PresenterDatiContributiviAGO presenter = new PresenterDatiContributiviAGO();
                Presenter.SvrLiquidazioneAgo.AreaEsito esito = presenter.RecuperaStatiEsteri(txtCodiceStatoEE.Text, txtCodiceIstituzioneEE.Text, out descStato, out descIstituzione, out descCittà, this);
                if (!string.IsNullOrEmpty(descStato))
                {
                    lblCodiceIstituzione.Text = txtCodiceIstituzioneEE.Text;
                    lblCodiceStatoEE.Text = txtCodiceStatoEE.Text;
                    lblNomeStato.Text = descStato;
                    lblSigla.Text = descIstituzione;
                    lblCitta.Text = descCittà;
                    txtSettimaneDiritto.Text = string.Empty;
                    txtSettimaneMisuraDecorrenzaPensione.Text = string.Empty;
                }
            }
        }

        private void EliminaDatiEsteri(long id)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviAGO presenterDatiContributiviAgo = new PresenterDatiContributiviAGO();
            presenterDatiContributiviAgo.EliminaStatoEstero(id, this);
        }
        #endregion gvDatiEsteri

        #region gvIstituzioniEstere

        protected void gvIstituzioniEstere_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AreaDatiContributivi areaDatiContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()];
                if (areaDatiContributiviAgo != null && areaDatiContributiviAgo.ProRata != null && areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null &&
                    areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0)
                {
                    GestioneContribStatoEsteroCumulo[] elencoStatiEsteri = areaDatiContributiviAgo.ProRata.ElencoStatiEsteri;

                    Image img = (Image)e.Row.FindControl("img");
                    
                    string currentTheme = Page.Theme;

                    if (elencoStatiEsteri[e.Row.RowIndex].PrestazioneEsteraCumulo._Confermato == true)
                    {
                        img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/verde_tab.png";
                        img.ToolTip = "Salvato";
                    }
                    else if (Utility.IsRicostituzione(datiPensione.CodeGruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione.CodeGruppo, datiPensione.CodeProdotto))
                    {
                        img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/arancione_tab.png";
                        img.ToolTip = "Non Obbligatorio";
                    }
                    else
                    {
                        img.ImageUrl = "~/App_Themes/" + currentTheme + "/Images/rosso_tab.png";
                        img.ToolTip = "Non Salvato";
                    }
                }
            }

        }

        protected void gvIstituzioniEstere_DataBinding(object sender, EventArgs e)
        { }

        protected void gvIstituzioniEstere_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "modifica")
            {
                if (this.TitolarePensione == null)
                    this.TitolarePensione = new AreaTitolare();
                if (this.TitolarePensione.Pensione == null)
                    this.TitolarePensione.Pensione = GetDatiPensione(this);
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                hdnNRecordProrata.Value = Convert.ToInt16(e.CommandArgument).ToString();
                RaiseNascondiAvviso(this, null);
                areaDatiContributiviAgo = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviAgo.ToString()];

                GestioneContribStatoEsteroCumulo statoEstero = areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(e.CommandArgument));
                ValorizzaEtichetteProRataEstera(statoEstero.PrestazioneEsteraCumulo);

                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> lstCopy = new List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>();
                foreach (GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo elem in statoEstero.ElencoImportiEsteriCumulo)
                    lstCopy = AggiungiRecordImporti(lstCopy, elem.ImportoPrestazione, elem.CessazionePrestazione, elem.DecorrenzaPrestazione);
                ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = lstCopy;

                editpan.Visible = true;
                txtSettimaneDiritto.Enabled = false;
                txtSettimaneMisuraDecorrenzaPensione.Enabled = false;
                divEliminaProrata.Visible = false;
                pnlTable.Visible = false;
                hdnIsInProrata.Value = "true";
                BindDataImportiEsteri(hdnNRecordProrata.Value);
            }
        }

        internal void ValorizzaListaStatiEsteri(IDatiContributiviAgo datiContributivi)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            if (datiContributivi != null && datiContributivi.areaDatiContributiviAgo != null && datiContributivi.areaDatiContributiviAgo.ProRata != null &&
                datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri != null && datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.Count() > 0)
            {
                if (datiContributivi.areaDatiContributiviAgo.IsDatiEsteriFromServices.GetValueOrDefault() || CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    divEliminaProrata.Visible = false;
                    List<GestioneContribStatoEsteroCumulo> listaStatiEsteri = datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ToList();
                    datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri = listaStatiEsteri.ToArray();

                    List<ElementiProrata> listaProrata = new List<ElementiProrata>();
                    listaProrata = CreaDataSource(datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri);
                    gvIstituzioniEstere.DataSource = listaProrata;
                    gvIstituzioniEstere.DataBind();
                }
                else
                {
                    pnlDatiEsteriEditMode.Visible = true;
                    divEliminaProrata.Visible = true;
                    List<GestioneContribStatoEsteroCumulo> listaStatiEsteri = datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri.ToList();
                    datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri = listaStatiEsteri.ToArray();
                    ViewState[EnumViewState.ElencoDatiEsteri.ToString()] = listaStatiEsteri;

                    List<ElementiProrataNew> listaProrata = new List<ElementiProrataNew>();
                    listaProrata = CreaDataSourceEditMode(datiContributivi.areaDatiContributiviAgo.ProRata.ElencoStatiEsteri);
                    gvDatiEsteri.DataSource = listaProrata;
                    gvDatiEsteri.DataKeyNames = new String[] { "strCodiceStato" };
                    gvDatiEsteri.DataBind();
                    BindDataDatiEsteri();
                }
            }
        }

        internal void ValorizzaGridStatiEsteriVuota()
        {
            List<GestioneContribStatoEsteroCumulo> elencoDatiEsteri = (List<GestioneContribStatoEsteroCumulo>)ViewState[EnumViewState.ElencoDatiEsteri.ToString()];

            EliminaRecordStati(elencoDatiEsteri);
            elencoDatiEsteri = CreaRecordStatiEsteri();
            modalitaEditStatiEsteri.Value = "false";

            ViewState[EnumViewState.ElencoDatiEsteri.ToString()] = elencoDatiEsteri;
            divEliminaProrata.Visible = true;

            List<extAreaDatiEsteri> extListAreaDatiEsteri = new List<extAreaDatiEsteri>();
            foreach (GestioneContribStatoEsteroCumulo statoEstero in elencoDatiEsteri)
            {
                extAreaDatiEsteri myExt = new extAreaDatiEsteri(statoEstero);
                extListAreaDatiEsteri.Add(myExt);
            }
            gvDatiEsteri.DataSource = extListAreaDatiEsteri;
            gvDatiEsteri.DataKeyNames = new String[] { "strCodiceStato" };
            gvDatiEsteri.DataBind();
        }

        private List<ElementiProrata> CreaDataSource(GestioneContribStatoEsteroCumulo[] elencoStatiEsteri)
        {
            ElementiProrata elementoProRata;
            List<ElementiProrata> listaProrata = new List<ElementiProrata>();
            int i = 0;
            foreach (GestioneContribStatoEsteroCumulo statoEstero in elencoStatiEsteri)
            {
                elementoProRata = new ElementiProrata();
                elementoProRata.id = i++;
                elementoProRata.nomeStato = statoEstero.PrestazioneEsteraCumulo.NomeStato;
                elementoProRata.codiceIstituzione = statoEstero.PrestazioneEsteraCumulo._CodiceIstituzione;
                elementoProRata.codiceStato = statoEstero.PrestazioneEsteraCumulo._CodiceStato;
                elementoProRata.Confermato = statoEstero.PrestazioneEsteraCumulo._Confermato;

                listaProrata.Add(elementoProRata);
            }
            return listaProrata;
        }

        private List<ElementiProrataNew> CreaDataSourceEditMode(GestioneContribStatoEsteroCumulo[] elencoStatiEsteri)
        {
            ElementiProrataNew elementoProRata;
            List<ElementiProrataNew> listaProrata = new List<ElementiProrataNew>();
            int i = 0;
            foreach (GestioneContribStatoEsteroCumulo statoEstero in elencoStatiEsteri)
            {
                elementoProRata = new ElementiProrataNew();
                elementoProRata.id = i++;
                elementoProRata.codiceIstituzione = statoEstero.PrestazioneEsteraCumulo._CodiceIstituzione;
                elementoProRata.codiceStato = statoEstero.PrestazioneEsteraCumulo._CodiceStato;
                elementoProRata.Confermato = statoEstero.PrestazioneEsteraCumulo._Confermato;

                listaProrata.Add(elementoProRata);
            }
            return listaProrata;
        }
        #endregion gvIstituzioniEstere

        #region gvImportiEsteri

        protected List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> BindDataImportiEsteri(string indiceStatoEstero)
        {
            List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];

            if (elencoImportiEsteri == null ||
                elencoImportiEsteri.Count() == 0)
                elencoImportiEsteri = CreaRecord();
            else
            {

                EliminaRecordImportiVuoti(elencoImportiEsteri);
                elencoImportiEsteri = AggiungiRecordImporti(elencoImportiEsteri, null, null, null);
            }
            ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = elencoImportiEsteri;

            List<extAreaImportiEsteri> extListAreaImportiEsteri = new List<extAreaImportiEsteri>();
            foreach (GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo importoEstero in elencoImportiEsteri)
            {
                extAreaImportiEsteri myExt = new extAreaImportiEsteri(importoEstero);
                extListAreaImportiEsteri.Add(myExt);
            }
            gvImportiEsteri.DataSource = extListAreaImportiEsteri;
            gvImportiEsteri.DataKeyNames = new String[] { "strDecorrenzaPrestazione" };
            gvImportiEsteri.DataBind();
            return elencoImportiEsteri;
        }

        private List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> CreaRecord()
        {
            List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = new List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>();
            elencoImportiEsteri = AggiungiRecordImporti(elencoImportiEsteri, null, null, null);
            return elencoImportiEsteri;
        }

        private List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> AggiungiRecordImporti(List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> listaRecord, Decimal? ImportoPrestazioneEE, DateTime? CessazionePrestazioneEE, DateTime? DecorrenzaPrestazioneEE)
        {
            GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo record = new GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo();
            record.ImportoPrestazione = ImportoPrestazioneEE;
            record.CessazionePrestazione = CessazionePrestazioneEE;
            record.DecorrenzaPrestazione = DecorrenzaPrestazioneEE;
            listaRecord.Add(record);
            return listaRecord;
        }

        protected void gvImportiEsteri_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvImportiEsteri.EditIndex = e.NewEditIndex;
                BindDataImportiEsteri(hdnNRecordProrata.Value);
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowEditing " + ex);
            }
        }

        protected void gvImportiEsteri_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvImportiEsteri.PageIndex = e.NewPageIndex;
                BindDataImportiEsteri(hdnNRecordProrata.Value);
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiFS, Errore nel metodo gvRecordFondo_onPageIndexChanging" + ex);
            }
        }

        protected void gvImportiEsteri_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = (List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];
                GridViewRow row = gvImportiEsteri.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvImportiEsteri.PageIndex * 10) + e.RowIndex);
                    if (elencoImportiEsteri.Count != i + 1)
                        elencoImportiEsteri.RemoveAt(elencoImportiEsteri.Count - 1);
                    gvImportiEsteri.EditIndex = -1;

                    BindDataImportiEsteri(hdnNRecordProrata.Value);
                }
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowUpdating " + ex);
            }
        }

        protected void gvImportiEsteri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvImportiEsteri.EditIndex = -1;
                BindDataImportiEsteri(hdnNRecordProrata.Value);
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvDatiContributivi_RowCancelingEdit " + ex);
            }
        }

        protected void gvImportiEsteri_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> listaImportiEsteri = BindDataImportiEsteri(hdnNRecordProrata.Value);
                GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo[] elencoImportiEsteri = listaImportiEsteri.ToArray();
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (listaImportiEsteri.Count == 0)
                {
                    elencoImportiEsteri[0].ImportoPrestazione = null;
                    elencoImportiEsteri[0].DecorrenzaPrestazione = null;
                    elencoImportiEsteri[0].CessazionePrestazione = null;
                }
                else
                {
                    if (row.DataItemIndex == 0)
                        modalitaEditImporti.Value = "false";
                    listaImportiEsteri.RemoveAt(row.DataItemIndex);
                }
                ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = listaImportiEsteri;
                BindDataImportiEsteri(hdnNRecordProrata.Value);
            }
            else if (e.CommandName == "Edit")
            {
            }
            else if (e.CommandName == "Salva")
            {
                RaiseNascondiAvviso(this, null);
                if (!IsEmptyEditableRowImpotiEE((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> listaImportiEsteri = BindDataImportiEsteri(hdnNRecordProrata.Value);
                    GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo[] elencoImportiEsteri = listaImportiEsteri.ToArray();

                    string valueDecorrenza = string.Empty;
                    string valueCessazione = string.Empty;
                    decimal valueImporto = decimal.MinValue;
                    int index = int.Parse(((string)(e.CommandArgument)));

                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

                    TextBox txtDecorrenzaImportiEE = (TextBox)row.FindControl("txtDecorrenzaPrestazioneEE");
                    if (!String.IsNullOrEmpty(txtDecorrenzaImportiEE.Text))
                        valueDecorrenza = txtDecorrenzaImportiEE.Text;
                    DateTime dateDecorrenza;
                    DateTime.TryParse(valueDecorrenza, out dateDecorrenza);

                    TextBox txtImportoPrestazioneEE = (TextBox)row.FindControl("txtImportoPrestazioneEE");
                    if (!String.IsNullOrEmpty(txtImportoPrestazioneEE.Text))
                        valueImporto = Decimal.Parse(txtImportoPrestazioneEE.Text);

                    TextBox txtCessazione = (TextBox)row.FindControl("txtCessazionePrestazioneEE");
                    if (!String.IsNullOrEmpty(txtCessazione.Text))
                        valueCessazione = txtCessazione.Text;

                    DateTime dateCessazione;
                    DateTime.TryParse(valueCessazione, out dateCessazione);

                    if ((row.DataItemIndex - 1) == (elencoImportiEsteri.Length - 2))    //aggiunta riga (non si tratta di una modifica)
                    {
                        listaImportiEsteri = AggiungiRecordImporti(listaImportiEsteri, valueImporto, dateCessazione, dateDecorrenza);
                        gvImportiEsteri.EditIndex = -1;
                    }
                    else   //modifica elemento
                    {
                        elencoImportiEsteri[row.DataItemIndex].DecorrenzaPrestazione = dateDecorrenza;
                        elencoImportiEsteri[row.DataItemIndex].CessazionePrestazione = dateCessazione;
                        elencoImportiEsteri[row.DataItemIndex].ImportoPrestazione = valueImporto;
                        gvImportiEsteri.EditIndex = -1;
                    }

                    ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = listaImportiEsteri;
                    BindDataImportiEsteri(hdnNRecordProrata.Value);
                }
            }
            else if (e.CommandName == "Cancel")
            {
                modalitaEditImporti.Value = "false";
                gvImportiEsteri.EditIndex = -1;
            }
        }

        protected void gvImportiEsteri_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri = ((List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()]);
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        if ((elencoImportiEsteri.Count == 1) &&
                            (elencoImportiEsteri.First().DecorrenzaPrestazione == null || elencoImportiEsteri.First().DecorrenzaPrestazione == DateTime.MinValue) &&
                            (elencoImportiEsteri.First().CessazionePrestazione == null || elencoImportiEsteri.First().CessazionePrestazione == DateTime.MinValue) &&
                            (elencoImportiEsteri.First().ImportoPrestazione == null || elencoImportiEsteri.First().ImportoPrestazione == decimal.MinValue))
                        {   //unica riga vuota, partenza in modalità edit

                            if (modalitaEditImporti.Value == "false")
                            {
                                gvImportiEsteri.EditIndex = 0;
                                modalitaEditImporti.Value = "true";
                                BindDataImportiEsteri(hdnNRecordProrata.Value);
                            }
                        }
                        else
                            this.btnConfermaModifiche.Enabled = true;

                        if (e.Row.Cells[0].Controls.Count == 3)
                        {
                            TextBox txtDecorrenzaPrestazioneEE = (TextBox)e.Row.FindControl("txtDecorrenzaPrestazioneEE");
                            TextBox txtCessazionePrestazioneEE = (TextBox)e.Row.FindControl("txtDecorrenzaCessazioneEE");

                            LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));

                            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                            cancel.ToolTip = "Annulla";
                            LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                            save.ToolTip = "Salva";
                            save.CausesValidation = true;
                            save.ValidationGroup = "UCTabDatiEsteriGrid";
                            save.CommandName = "Salva";
                            setCampiEditImportiEsteri(e, true);
                        }
                        else
                        {
                            LinkButton delete = (LinkButton)e.Row.FindControl("btnDelete");
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            int index = e.Row.DataItemIndex;
                            if (index >= 0 && index <= elencoImportiEsteri.Count - 2)
                            {
                                edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                                edit.ToolTip = "Modifica";

                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
                            }
                        }
                    }
                    else   //record successivi al primo
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)    //modalità edit
                        {
                            TextBox Decorrenza = (TextBox)e.Row.FindControl("txtDecorrenzaPrestazioneEE");
                            LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                            cancel.ToolTip = "Annulla";
                            LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                            save.ToolTip = "Salva";
                            save.CausesValidation = true;
                            save.ValidationGroup = "UCTabDatiEsteriGrid";
                            save.CommandName = "Salva";
                            setCampiEditImportiEsteri(e, false);
                        }
                        else
                        {
                            if (e.Row.DataItemIndex == elencoImportiEsteri.Count - 1)
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
                                if (index >= 0 && index <= elencoImportiEsteri.Count - 2)
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
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloCi, Errore nel metodo gvImportiEsteri_RowDataBound " + ex);
            }
        }

        private bool IsEmptyEditableRowImpotiEE(GridViewRow row)
        {
            if (row.FindControl("txtDecorrenzaPrestazioneEE") != null && ((TextBox)row.FindControl("txtDecorrenzaPrestazioneEE")).Text != string.Empty &&
                row.FindControl("txtImportoPrestazioneEE") != null && ((TextBox)row.FindControl("txtImportoPrestazioneEE")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void setCampiEditImportiEsteri(GridViewRowEventArgs e, bool IsFirstRecord)
        {
            TextBox txtDecorrenzaPrestazioneEE = (TextBox)e.Row.FindControl("txtDecorrenzaPrestazioneEE");
            TextBox txtCessazionePrestazioneEE = (TextBox)e.Row.FindControl("txtCessazionePrestazioneEE");
            TextBox txtImportoPrestazioneEE = (TextBox)e.Row.FindControl("txtImportoPrestazioneEE");
            string myDec = String.Format("{0:MM/yyyy}", ((GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo)e.Row.DataItem).DecorrenzaPrestazione);
            if (String.Equals(myDec, "01/0001"))
                txtDecorrenzaPrestazioneEE.Text = string.Empty;

            string mySosp = String.Format("{0:MM/yyyy}", ((GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo)e.Row.DataItem).CessazionePrestazione);
            if (String.Equals(mySosp, "01/0001"))
                txtCessazionePrestazioneEE.Text = "";

            string myImporti = ((GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo)e.Row.DataItem).ImportoPrestazione.ToString();
            if (String.Equals(myImporti, decimal.MinValue.ToString("dd/MM/yyyy")))
                txtImportoPrestazioneEE.Text = "";
        }

        private List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> EliminaRecordImportiVuoti(List<GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo> elencoImportiEsteri)
        {
            int i = 0; int j = 0;
            int[] elementiDaEliminare = new int[30];
            foreach (GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo importoEstero in elencoImportiEsteri)
            {
                if (importoEstero.CessazionePrestazione == null && importoEstero.DecorrenzaPrestazione == null &&
                    importoEstero.ImportoPrestazione == null)
                {
                    elementiDaEliminare[j] = i;
                    j++;
                }
                i++;
            }

            for (int z = 0; z < j; z++)
            {
                if (elementiDaEliminare[z] <= elencoImportiEsteri.Count - 1)
                    elencoImportiEsteri.RemoveAt(elementiDaEliminare[z]);
            }
            return elencoImportiEsteri;
        }

        internal void SetViewStateArea(AreaDatiContributivi area)
        {
            ViewState[EnumViewState.DatiContributiviAgo.ToString()] = area;
        }

        #endregion gvImportiEsteri

        #region EventHandler
        public static Comparison<GestioneContribStatoEsteroCumulo> sortStatiEsteri = delegate(GestioneContribStatoEsteroCumulo d1, GestioneContribStatoEsteroCumulo d2)
        {
            try
            {
                int retValue = int.MinValue;
                retValue = string.Compare(d1.PrestazioneEsteraCumulo._CodiceIstituzione, d2.PrestazioneEsteraCumulo._CodiceIstituzione, false, CultureInfo.InvariantCulture);
                if (retValue == 0)
                {
                    retValue = string.Compare(d1.PrestazioneEsteraCumulo._CodiceStato, d2.PrestazioneEsteraCumulo._CodiceStato, false, CultureInfo.InvariantCulture);
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Errore nel metodo SortStatiEsteri" + ex);
            }
        };

        public static Comparison<ElementiProrata> sortProrataEstera = delegate(ElementiProrata d1, ElementiProrata d2)
        {
            try
            {
                int retValue = int.MinValue;
                retValue = string.Compare(d1.codiceIstituzione, d2.codiceIstituzione, false, CultureInfo.InvariantCulture);
                if (retValue == 0)
                {
                    retValue = string.Compare(d1.codiceStato, d2.codiceStato, false, CultureInfo.InvariantCulture);
                }
                return retValue;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Errore nel metodo SortStatiEsteri" + ex);
            }
        };

        public event EventHandler ShowAvvisoDatiProRata;
        public event EventHandler ShowAvvisoEliminaDatiProRata;
        public event EventHandler NascondiAvviso;

        protected void RaiseShowAvvisoDatiProRata(object sender, EventArgs e)
        {
            ShowAvvisoDatiProRata(sender, e);
        }

        protected void RaiseShowAvvisoEliminaDatiProRata(object sender, EventArgs e)
        {
            ShowAvvisoEliminaDatiProRata(sender, e);
        }

        protected void RaiseNascondiAvviso(object sender, EventArgs e)
        {
            NascondiAvviso(sender, e);
        }
        #endregion EventHandler
    }

    public class ElementiProrata
    {
        private int _intId;
        private string _strNomeStato;
        private string _strCodiceIstituzione;
        private string _strCodiceStato;

        public int id { get { return _intId; } set { _intId = value; } }
        public string nomeStato { get { return _strNomeStato; } set { _strNomeStato = value; } }
        public string codiceIstituzione { get { return _strCodiceIstituzione; } set { _strCodiceIstituzione = value; } }
        public string codiceStato { get { return _strCodiceStato; } set { _strCodiceStato = value; } }
        public bool? Confermato { get; set; }
    }

    public class ElementiProrataNew
    {
        private int _intId;
        private string _CodiceIstituzione;
        private string _CodiceStato;

        public int id { get { return _intId; } set { _intId = value; } }
        public string codiceIstituzione { get { return _CodiceIstituzione; } set { _CodiceIstituzione = value; } }
        public string codiceStato { get { return _CodiceStato; } set { _CodiceStato = value; } }
        public bool? Confermato { get; set; }

        public String strCodiceStato
        {
            get
            {
                if (this._CodiceStato == string.Empty)
                {
                    return "";
                }
                return this._CodiceStato;
            }
        }

        public String strCodiceIstituzione
        {
            get
            {
                if (this._CodiceIstituzione == string.Empty)
                {
                    return "";
                }
                return this._CodiceIstituzione;
            }
        }
    }

    public class extAreaImportiEsteri : GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo
    {
        public extAreaImportiEsteri(GestioneDatiEsteriCumuloPensioneImportiEsteriCumulo area)
        {
            this.ImportoPrestazione = area.ImportoPrestazione;
            this.DecorrenzaPrestazione = area.DecorrenzaPrestazione;
            this.CessazionePrestazione = area.CessazionePrestazione;
        }

        public String strDecorrenzaPrestazione
        {
            get
            {
                if (this.DecorrenzaPrestazione == DateTime.MinValue)
                {
                    return "";
                }
                String dp = this.DecorrenzaPrestazione.ToString();
                String dp2 = String.Format("{0:MM/yyyy}", this.DecorrenzaPrestazione);
                return dp2;
            }
        }

        public String strCessazionePrestazione
        {
            get
            {
                if (this.CessazionePrestazione == DateTime.MinValue)
                {
                    return "";
                }
                String ds = this.CessazionePrestazione.ToString();
                String ds2 = String.Format("{0:MM/yyyy}", this.CessazionePrestazione);
                return ds2;
            }
        }

        public String strImportoPrestazione
        {
            get
            {
                if (this.ImportoPrestazione == decimal.MinValue)
                {
                    return "";
                }
                String ip = this.ImportoPrestazione.ToString();
                return ip;
            }
        }
    }

    public class extAreaDatiEsteri : GestioneContribStatoEsteroCumulo
    {
        public extAreaDatiEsteri(GestioneContribStatoEsteroCumulo area)
        {
            if (area == null)
                area = new GestioneContribStatoEsteroCumulo();
            if (area.PrestazioneEsteraCumulo == null)
                area.PrestazioneEsteraCumulo = new GestioneContribPrestazioneEsteraCumulo();
            if (this.PrestazioneEsteraCumulo == null)
                this.PrestazioneEsteraCumulo = new GestioneContribPrestazioneEsteraCumulo();
            this.PrestazioneEsteraCumulo._CodiceStato = area.PrestazioneEsteraCumulo._CodiceStato;
            this.PrestazioneEsteraCumulo._CodiceIstituzione = area.PrestazioneEsteraCumulo._CodiceIstituzione;
        }

        public String strCodiceStato
        {
            get
            {
                if (this.PrestazioneEsteraCumulo._CodiceStato == string.Empty)
                {
                    return "";
                }
                return this.PrestazioneEsteraCumulo._CodiceStato;
            }
        }

        public String strCodiceIstituzione
        {
            get
            {
                if (this.PrestazioneEsteraCumulo._CodiceIstituzione == string.Empty)
                {
                    return "";
                }
                return this.PrestazioneEsteraCumulo._CodiceIstituzione;
            }
        }
    }
}