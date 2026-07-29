using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi
{
    public partial class UCProrataCi : CustomBaseUserControl, IDatiContributiviCi, ITitolarePensione
    {
        private enum EnumViewState { ElencoImportiEsteri, DatiContributiviCi }

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributiviCi
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaDatiContributivi areaDatiContributiviCi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviCi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

                if (this.areaDatiContributiviCi != null)
                {
                    ViewState[EnumViewState.DatiContributiviCi.ToString()] = this.areaDatiContributiviCi;
                    if (this.areaDatiContributiviCi.ProRata != null)
                    {
                        ValorizzaListaStatiEsteri(this);
                        //DataBind();
                    }
                }

                if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    divSalvaProrata.Visible = false;
            }
        }

        protected void ConfermaModifiche_Click(object sender, EventArgs e)
        {
            RaiseNascondiAvviso(this, null);

            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaDatiContributiviCi = ConfermaModificheProrata(Convert.ToInt16(hdnNRecordProrata.Value));

            if (this.HasError)
            {
                RaiseShowAvvisoDatiProRata(this, null);
            }
            else
            {
                RaiseAggiornaSemaforoTabIntegrazioneVirtuale(this, null);
                presenterDatiContributiviCi.SalvaTabProrata(this);
                ViewState[EnumViewState.DatiContributiviCi.ToString()] = this.areaDatiContributiviCi;
                if (!this.HasError)
                {
                    HideDatiStato();
                    RaiseAggiornaAnniTabIntegrazioneVirtuale(this, null);
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
            this.areaDatiContributiviCi = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()];

            List<GestioneDatiContributiviCiPensioniCiImportiEsteri> lista = this.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteri.ToList();
            lista = EliminaRecordImportiVuoti(lista);
            this.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteri = lista.ToArray();
            ViewState[EnumViewState.DatiContributiviCi.ToString()] = this.areaDatiContributiviCi;

            if (modalitaEditImporti.Value == "true")
            {
                gvImportiEsteri.EditIndex = -1;
                modalitaEditImporti.Value = "false";
            }

            //if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            //    divSalvaProrata.Visible = true;
            //else
            //    divSalvaProrata.Visible = false;
        }

        protected void btnCancelProRata_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            presenterDatiContributiviCi.EliminaDatiProRata(this);

            if (!this.HasError)
            {
                presenterDatiContributiviCi.GetDatiContributivi(this);
                if (this.areaDatiContributiviCi.ProRata != null)
                {
                    ViewState[EnumViewState.DatiContributiviCi.ToString()] = this.areaDatiContributiviCi;
                    ValorizzaListaStatiEsteri(this);
                    DataBind();
                }
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
            AreaDatiContributivi area = ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()]);

            GestioneContribPrestazioneEstera prestazioneEstera = GetDatiPrestazioneEstera();
            area.ProRata.ElencoStatiEsteri.ElementAt(indiceProrata).PrestazioneEstera = prestazioneEstera;
            List<GestioneDatiContributiviCiPensioniCiImportiEsteri> importiEsteri = GetDatiImportiEsteri();

            if (this.HasError)
                return null;

            importiEsteri = EliminaRecordImportiVuoti(importiEsteri);
            area.ProRata.ElencoStatiEsteri.ElementAt(indiceProrata).ElencoImportiEsteri = importiEsteri.ToArray();

            return area;
        }

        internal AreaDatiContributivi ConfermaModificheProrata()
        {
            return ConfermaModificheProrata(Convert.ToInt16(hdnNRecordProrata.Value));
        }

        private List<GestioneDatiContributiviCiPensioniCiImportiEsteri> GetDatiImportiEsteri()
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

            List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri = (List<GestioneDatiContributiviCiPensioniCiImportiEsteri>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];
            List<GestioneDatiContributiviCiPensioniCiImportiEsteri> importiEsteri = new List<GestioneDatiContributiviCiPensioniCiImportiEsteri>();
            foreach (GestioneDatiContributiviCiPensioniCiImportiEsteri importoEsteroVS in elencoImportiEsteri)
            {
                GestioneDatiContributiviCiPensioniCiImportiEsteri importoEstero = new GestioneDatiContributiviCiPensioniCiImportiEsteri();
                importoEstero.DecorrenzaPrestazioneEE = importoEsteroVS.DecorrenzaPrestazioneEE == DateTime.MinValue ? null : importoEsteroVS.DecorrenzaPrestazioneEE;
                importoEstero.CessazionePrestazioneEE = importoEsteroVS.CessazionePrestazioneEE == DateTime.MinValue ? null : importoEsteroVS.CessazionePrestazioneEE;
                importoEstero.ImportoPrestazioneEE = importoEsteroVS.ImportoPrestazioneEE;
                importiEsteri.Add(importoEstero);
            }

            return importiEsteri;
        }

        private GestioneContribPrestazioneEstera GetDatiPrestazioneEstera()
        {
            GestioneContribPrestazioneEstera prestazioneEstera = new GestioneContribPrestazioneEstera();
            if (!String.IsNullOrEmpty(lblIdPrestazioneEE.Text))
                prestazioneEstera._Id = long.Parse(lblIdPrestazioneEE.Text);
            if (!String.IsNullOrEmpty(lblNomeStato.Text))
                prestazioneEstera.NomeStato = lblNomeStato.Text;
            if (!String.IsNullOrEmpty(lblCodiceIstituzione.Text))
                prestazioneEstera._CodiceIstituzione = lblCodiceIstituzione.Text;
            if (!String.IsNullOrEmpty(lblCodiceStatoEE.Text))
                prestazioneEstera._CodiceStatoEE = lblCodiceStatoEE.Text;
            if (!String.IsNullOrEmpty(lblCitta.Text))
                prestazioneEstera.Citta = lblCitta.Text;
            if (!String.IsNullOrEmpty(lblSigla.Text))
                prestazioneEstera.Sigla = lblSigla.Text;
            if (!String.IsNullOrEmpty(lblCodiceConvenzione.Text))
                prestazioneEstera._CodiceConvenzione = Byte.Parse(lblCodiceConvenzione.Text);
            if (!String.IsNullOrEmpty(txtDataPrecedenteLiquidazione.Text))
                prestazioneEstera._DecorrenzaLiquidazioneStatoEE = Utility.GetDateFromString(txtDataPrecedenteLiquidazione.Text);
            if (!String.IsNullOrEmpty(txtDataRicalcolo.Text))
                prestazioneEstera._DecorrenzaRicalcolo = Utility.GetDateFromString(txtDataRicalcolo.Text);
            if (!String.IsNullOrEmpty(txtSettimaneMisuraDecorrenzaPensione.Text))
                prestazioneEstera._ContributiEEDecorrenzaOriginaria = Int32.Parse(txtSettimaneMisuraDecorrenzaPensione.Text);
            if (!String.IsNullOrEmpty(txtSettimaneARicalcolo.Text))
                prestazioneEstera._ContributiEERicalcolo = Int32.Parse(txtSettimaneARicalcolo.Text);
            if (!String.IsNullOrEmpty(txtSettimaneDiritto.Text))
                prestazioneEstera._ContributiEEDiritto = Int32.Parse(txtSettimaneDiritto.Text);
            if (!String.IsNullOrEmpty(lblMatricolaIstituzioneEE.Text))
                prestazioneEstera.MatricolaIstituzione = lblMatricolaIstituzioneEE.Text;
            if (!String.IsNullOrEmpty(lblCodicePi.Text))
                prestazioneEstera._CodicePi = lblCodicePi.Text[0];
            if (!string.IsNullOrEmpty(ddlSospensioneIntegrazioneTrattamentoMinimo.SelectedValue))
            {
                if (ddlSospensioneIntegrazioneTrattamentoMinimo.SelectedValue == "SI")
                    prestazioneEstera._SospensioneCautelativaIntegrazione = 'S';
                else if (ddlSospensioneIntegrazioneTrattamentoMinimo.SelectedValue == "NO")
                    prestazioneEstera._SospensioneCautelativaIntegrazione = 'N';
            }
            else
                prestazioneEstera._SospensioneCautelativaIntegrazione = null;
            if (!String.IsNullOrEmpty(txtEta.Text))
                prestazioneEstera._EtaSospensione = byte.Parse(txtEta.Text);
            if (!string.IsNullOrEmpty(hdnIsArt48Checked.Value))
            {
                if (hdnIsArt48Checked.Value == "true")
                    prestazioneEstera._CodiceArt48 = 'S';
                else if (hdnIsArt48Checked.Value == "false")
                    prestazioneEstera._CodiceArt48 = 'N';
            }
            if (!String.IsNullOrEmpty(txtDecorrenzaArt48.Text))
                prestazioneEstera._DecorrenzaArt48 = Utility.GetDateFromString(txtDecorrenzaArt48.Text);
            if (!String.IsNullOrEmpty(txtIntegrazioneExJugoslavia.Text))
                prestazioneEstera._QuotaIntegrazioneEEeArgentinaResidentiItalia = Decimal.Parse(txtIntegrazioneExJugoslavia.Text);
            if (!String.IsNullOrEmpty(txtDecorrenzaIntegrazione.Text))
                prestazioneEstera._DecorrenzaIntegrazione = Utility.GetDateFromString(txtDecorrenzaIntegrazione.Text);
            //elemento è stato modificato quindi imposto il flag confermato a false
            prestazioneEstera._Confermato = true;

            return prestazioneEstera;
        }


        internal GestioneContribProRata GetDatiProRata()
        {
            areaDatiContributiviCi = ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()]);

            if (this.editpan.Visible)
            {
                GestioneContribPrestazioneEstera prestazioneEstera = GetDatiPrestazioneEstera();
                this.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).PrestazioneEstera = prestazioneEstera;
                List<GestioneDatiContributiviCiPensioniCiImportiEsteri> importiEsteri = GetDatiImportiEsteri();
                if (this.HasError)
                    return null;
                importiEsteri = EliminaRecordImportiVuoti(importiEsteri);
                areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(hdnNRecordProrata.Value)).ElencoImportiEsteri = importiEsteri.ToArray();
            }

            if (areaDatiContributiviCi.ProRata != null && areaDatiContributiviCi.ProRata.ElencoStatiEsteri != null)
            {
                foreach (GestioneContribStatoEstero stato in this.areaDatiContributiviCi.ProRata.ElencoStatiEsteri)
                {
                    stato.PrestazioneEstera._Confermato = true;
                    int index = stato.ElencoImportiEsteri.ToList().FindIndex(delegate(GestioneDatiContributiviCiPensioniCiImportiEsteri ie) { return (ie.CessazionePrestazioneEE == DateTime.MinValue && ie.DecorrenzaPrestazioneEE == DateTime.MinValue && ie.ImportoPrestazioneEE == decimal.MinValue); });
                    if (index >= 0)
                    {
                        List<GestioneDatiContributiviCiPensioniCiImportiEsteri> importiEE = new List<GestioneDatiContributiviCiPensioniCiImportiEsteri>();
                        importiEE = stato.ElencoImportiEsteri.ToList();
                        importiEE.RemoveAt(index);
                        stato.ElencoImportiEsteri = importiEE.ToArray();
                    }
                }
            }
            return ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()]).ProRata;
        }

        internal GestioneContribProRata GetViewStateProRata()
        {
            return ((AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()]).ProRata;
        }

        internal void AggiornaAnniIntegrazioneVirtuale(object sender, EventArgs e)
        {
            RaiseAggiornaAnniTabIntegrazioneVirtuale(this, null);
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

            //if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            //    divSalvaProrata.Visible = true;
            //else
            //    divSalvaProrata.Visible = false;

            ValorizzaListaStatiEsteri(this);
        }


        private void ValorizzaEtichetteProRataEstera(GestioneContribPrestazioneEstera prestazioneEstera)
        {
            lblIdPrestazioneEE.Text = prestazioneEstera._Id.ToString();
            lblCodiceIstituzione.Text = prestazioneEstera._CodiceIstituzione;
            lblCodiceStatoEE.Text = prestazioneEstera._CodiceStatoEE;
            lblCitta.Text = prestazioneEstera.Citta;
            lblSigla.Text = prestazioneEstera.Sigla;
            lblNomeStato.Text = prestazioneEstera.NomeStato;
            lblMatricolaIstituzioneEE.Text = prestazioneEstera.MatricolaIstituzione;
            lblCodicePi.Text = prestazioneEstera._CodicePi.HasValue ? prestazioneEstera._CodicePi.ToString() : string.Empty;
            if (prestazioneEstera._CodiceConvenzione != null)
                lblCodiceConvenzione.Text = prestazioneEstera._CodiceConvenzione.ToString();
            if (prestazioneEstera._DecorrenzaLiquidazioneStatoEE != null)
                txtDataPrecedenteLiquidazione.Text = String.Format("{0:MM/yyyy}", prestazioneEstera._DecorrenzaLiquidazioneStatoEE);
            else
                txtDataPrecedenteLiquidazione.Text = "";
            if (prestazioneEstera._DecorrenzaRicalcolo != null)
                txtDataRicalcolo.Text = String.Format("{0:MM/yyyy}", prestazioneEstera._DecorrenzaRicalcolo);
            else
                txtDataRicalcolo.Text = "";
            if (!String.IsNullOrEmpty(prestazioneEstera._ContributiEEDecorrenzaOriginaria.ToString()))
                txtSettimaneMisuraDecorrenzaPensione.Text = prestazioneEstera._ContributiEEDecorrenzaOriginaria.ToString();
            else
                txtSettimaneMisuraDecorrenzaPensione.Text = "";
            if (!String.IsNullOrEmpty(prestazioneEstera._ContributiEERicalcolo.ToString()))
                txtSettimaneARicalcolo.Text = prestazioneEstera._ContributiEERicalcolo.ToString();
            else
                txtSettimaneARicalcolo.Text = "";
            if (!String.IsNullOrEmpty(prestazioneEstera._ContributiEEDiritto.ToString()))
                txtSettimaneDiritto.Text = prestazioneEstera._ContributiEEDiritto.ToString();
            else
                txtSettimaneDiritto.Text = "";
            if (prestazioneEstera._SospensioneCautelativaIntegrazione != null)
            {
                if (String.Equals(prestazioneEstera._SospensioneCautelativaIntegrazione.ToString(), "S"))
                    ddlSospensioneIntegrazioneTrattamentoMinimo.SelectedValue = "SI";
                else if (String.Equals(prestazioneEstera._SospensioneCautelativaIntegrazione.ToString(), "N"))
                    ddlSospensioneIntegrazioneTrattamentoMinimo.SelectedValue = "NO";
            }
            else
            {
                ddlSospensioneIntegrazioneTrattamentoMinimo.SelectedValue = "SI";
            }
            if (!String.IsNullOrEmpty(prestazioneEstera._EtaSospensione.ToString()))
                txtEta.Text = prestazioneEstera._EtaSospensione.ToString();
            else
                txtEta.Text = "";
            if (prestazioneEstera._CodiceArt48 != null)
            {
                if (String.Equals(prestazioneEstera._CodiceArt48.ToString(), "S"))
                    chkApplicazioneArt48.Checked = true;
                else if (String.Equals(prestazioneEstera._CodiceArt48.ToString(), "N"))
                    chkApplicazioneArt48.Checked = false;
            }
            else
                chkApplicazioneArt48.Checked = false;
            if (prestazioneEstera._DecorrenzaArt48 != null)
                txtDecorrenzaArt48.Text = String.Format("{0:MM/yyyy}", prestazioneEstera._DecorrenzaArt48);
            else
                txtDecorrenzaArt48.Text = "";


            if (prestazioneEstera._CodiceStatoEE != "13" && prestazioneEstera._CodiceStatoEE != "14")
            {
                lblIntegrazioneExJugoslavia.Visible = false;
                txtIntegrazioneExJugoslavia.Visible = false;

                lblDecorrenzaIntegrazione.Visible = false;
                txtDecorrenzaIntegrazione.Visible = false;
            }
            else
            {
                if (prestazioneEstera._CodiceStatoEE == "13")
                    lblIntegrazioneExJugoslavia.Text = "Integrazione a carico della ex Jugoslavia:";
                else if (prestazioneEstera._CodiceStatoEE == "14")
                    lblIntegrazioneExJugoslavia.Text = "Integrazione a carico dell'Argentina:";

                if (!String.IsNullOrEmpty(prestazioneEstera._QuotaIntegrazioneEEeArgentinaResidentiItalia.ToString()))
                    txtIntegrazioneExJugoslavia.Text = prestazioneEstera._QuotaIntegrazioneEEeArgentinaResidentiItalia.ToString();
                else
                    txtIntegrazioneExJugoslavia.Text = "";

                if (prestazioneEstera._DecorrenzaIntegrazione != null)
                    txtDecorrenzaIntegrazione.Text = String.Format("{0:MM/yyyy}", prestazioneEstera._DecorrenzaIntegrazione);
                else
                    txtDecorrenzaIntegrazione.Text = "";
            }
            List<GestioneContribStatoEstero> stati = this.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ToList();
            if (stati.First().PrestazioneEstera._CodiceConvenzione == 58)
            {
                txtSettimaneMisuraDecorrenzaPensione.Enabled = prestazioneEstera._CodiceStatoEE == "58";
                txtSettimaneDiritto.Enabled = prestazioneEstera._CodiceStatoEE == "58";
            }

            GestioneEtichetteIsUnicarpe();

            //ENG - Avviso per Stato Croazia
            if (prestazioneEstera != null && !String.IsNullOrEmpty(prestazioneEstera._CodiceStatoEE) &&
                !String.IsNullOrEmpty(prestazioneEstera._CodiceStatoEE.Trim()) && prestazioneEstera._CodiceStatoEE.Trim() == "39")
                lblAvvisoStatoCroazia.Visible = true;
            else
                lblAvvisoStatoCroazia.Visible = false;
        }

        #region private methods
        private void GestioneEtichetteIsUnicarpe()
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                txtSettimaneMisuraDecorrenzaPensione.Enabled = false;
                txtSettimaneDiritto.Enabled = false;
            }
        }
        #endregion private methods

        #region gvIstituzioniEstere

        protected void gvIstituzioniEstere_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string currentTheme = Page.Theme;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AreaDatiContributivi areaDatiContributiviCi = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()];
                if (areaDatiContributiviCi != null && areaDatiContributiviCi.ProRata != null && areaDatiContributiviCi.ProRata.ElencoStatiEsteri != null &&
                    areaDatiContributiviCi.ProRata.ElencoStatiEsteri.Count() > 0)
                {
                    GestioneContribStatoEstero[] elencoStatiEsteri = areaDatiContributiviCi.ProRata.ElencoStatiEsteri;

                    Image img = (Image)e.Row.FindControl("img");

                    if (elencoStatiEsteri[e.Row.RowIndex].PrestazioneEstera._Confermato == true)
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
                areaDatiContributiviCi = (AreaDatiContributivi)ViewState[EnumViewState.DatiContributiviCi.ToString()];

                GestioneContribStatoEstero statoEstero = areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ElementAt(Convert.ToInt16(e.CommandArgument));
                ValorizzaEtichetteProRataEstera(statoEstero.PrestazioneEstera);

                List<GestioneDatiContributiviCiPensioniCiImportiEsteri> lstCopy = new List<GestioneDatiContributiviCiPensioniCiImportiEsteri>();
                foreach (GestioneDatiContributiviCiPensioniCiImportiEsteri elem in statoEstero.ElencoImportiEsteri)
                    lstCopy = AggiungiRecordImporti(lstCopy, elem.ImportoPrestazioneEE, elem.CessazionePrestazioneEE, elem.DecorrenzaPrestazioneEE);
                ViewState[EnumViewState.ElencoImportiEsteri.ToString()] = lstCopy;

                divSalvaProrata.Visible = false;
                editpan.Visible = true;
                pnlTable.Visible = false;
                hdnIsInProrata.Value = "true";
                if (CodeUtility.IsRicostituzioneOrRiapertura(this.TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) && statoEstero.PrestazioneEsteraStorico != null &&
                    statoEstero.PrestazioneEsteraStorico._CodiceArt48.HasValue && statoEstero.PrestazioneEsteraStorico._CodiceArt48.Value == 'S' && !statoEstero.PrestazioneEsteraStorico._DecorrenzaArt48.HasValue)
                    hdnIsBloccoArt48.Value = "true";
                else
                    hdnIsBloccoArt48.Value = "false";
                BindDataImportiEsteri(hdnNRecordProrata.Value);
                List<GestioneContribStatoEstero> stati = this.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ToList();
                if (stati.First().PrestazioneEstera._CodiceConvenzione == 58)
                {
                    txtSettimaneMisuraDecorrenzaPensione.Enabled = statoEstero.PrestazioneEstera._CodiceStatoEE == "58";
                    txtSettimaneDiritto.Enabled = statoEstero.PrestazioneEstera._CodiceStatoEE == "58";
                }
            }
        }

        internal void ValorizzaListaStatiEsteri(IDatiContributiviCi datiContributivi)
        {
            if (datiContributivi.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.Count() > 0)
            {
                List<GestioneContribStatoEstero> listaStatiEsteri = datiContributivi.areaDatiContributiviCi.ProRata.ElencoStatiEsteri.ToList();
                datiContributivi.areaDatiContributiviCi.ProRata.ElencoStatiEsteri = listaStatiEsteri.ToArray();

                List<ElementiProrata> listaProrata = new List<ElementiProrata>();
                listaProrata = CreaDataSource(datiContributivi.areaDatiContributiviCi.ProRata.ElencoStatiEsteri);
                gvIstituzioniEstere.DataSource = listaProrata;
                gvIstituzioniEstere.DataBind();
            }
        }

        private List<ElementiProrata> CreaDataSource(GestioneContribStatoEstero[] elencoStatiEsteri)
        {
            ElementiProrata elementoProRata;
            List<ElementiProrata> listaProrata = new List<ElementiProrata>();
            int i = 0;
            foreach (GestioneContribStatoEstero statoEstero in elencoStatiEsteri)
            {
                elementoProRata = new ElementiProrata();
                elementoProRata.id = i++;
                elementoProRata.nomeStato = statoEstero.PrestazioneEstera.NomeStato;
                elementoProRata.codiceIstituzione = statoEstero.PrestazioneEstera._CodiceIstituzione;
                elementoProRata.codiceStato = statoEstero.PrestazioneEstera._CodiceStatoEE;
                elementoProRata.Confermato = statoEstero.PrestazioneEstera._Confermato;

                listaProrata.Add(elementoProRata);
            }
            return listaProrata;
        }

        #endregion gvIstituzioniEstere

        #region gvImportiEsteri

        protected List<GestioneDatiContributiviCiPensioniCiImportiEsteri> BindDataImportiEsteri(string indiceStatoEstero)
        {
            //List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri = new List<GestioneDatiContributiviCiPensioniCiImportiEsteri>();

            List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri = (List<GestioneDatiContributiviCiPensioniCiImportiEsteri>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];

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
            foreach (GestioneDatiContributiviCiPensioniCiImportiEsteri importoEstero in elencoImportiEsteri)
            {
                extAreaImportiEsteri myExt = new extAreaImportiEsteri(importoEstero);
                extListAreaImportiEsteri.Add(myExt);
            }
            gvImportiEsteri.DataSource = extListAreaImportiEsteri;
            gvImportiEsteri.DataKeyNames = new String[] { "strDecorrenzaPrestazione" };
            gvImportiEsteri.DataBind();
            return elencoImportiEsteri;
        }

        private List<GestioneDatiContributiviCiPensioniCiImportiEsteri> CreaRecord()
        {
            List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri = new List<GestioneDatiContributiviCiPensioniCiImportiEsteri>();
            elencoImportiEsteri = AggiungiRecordImporti(elencoImportiEsteri, null, null, null);
            return elencoImportiEsteri;
        }

        private List<GestioneDatiContributiviCiPensioniCiImportiEsteri> AggiungiRecordImporti(List<GestioneDatiContributiviCiPensioniCiImportiEsteri> listaRecord, Decimal? ImportoPrestazioneEE, DateTime? CessazionePrestazioneEE, DateTime? DecorrenzaPrestazioneEE)
        {
            GestioneDatiContributiviCiPensioniCiImportiEsteri record = new GestioneDatiContributiviCiPensioniCiImportiEsteri();
            record.ImportoPrestazioneEE = ImportoPrestazioneEE;
            record.CessazionePrestazioneEE = CessazionePrestazioneEE;
            record.DecorrenzaPrestazioneEE = DecorrenzaPrestazioneEE;
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
                List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri = (List<GestioneDatiContributiviCiPensioniCiImportiEsteri>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()];
                GridViewRow row = gvImportiEsteri.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvImportiEsteri.PageIndex * 10) + e.RowIndex);
                    if (elencoImportiEsteri.Count != i + 1)
                        elencoImportiEsteri.RemoveAt(elencoImportiEsteri.Count - 1);
                    gvImportiEsteri.EditIndex = -1;
                    //((AreaDatiContributivi)ViewState["DatiContributiviCi"]).ProRata.ElencoStatiEsteri[Convert.ToInt32(hdnNRecordProrata.Value)].ElencoImportiEsteri = elencoImportiEsteri.ToArray();

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
                List<GestioneDatiContributiviCiPensioniCiImportiEsteri> listaImportiEsteri = BindDataImportiEsteri(hdnNRecordProrata.Value);
                GestioneDatiContributiviCiPensioniCiImportiEsteri[] elencoImportiEsteri = listaImportiEsteri.ToArray();
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (listaImportiEsteri.Count == 0)
                {
                    elencoImportiEsteri[0].ImportoPrestazioneEE = null;
                    elencoImportiEsteri[0].DecorrenzaPrestazioneEE = null;
                    elencoImportiEsteri[0].CessazionePrestazioneEE = null;
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
                if (!IsEmptyEditableRowImpotiEE((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    List<GestioneDatiContributiviCiPensioniCiImportiEsteri> listaImportiEsteri = BindDataImportiEsteri(hdnNRecordProrata.Value);
                    GestioneDatiContributiviCiPensioniCiImportiEsteri[] elencoImportiEsteri = listaImportiEsteri.ToArray();

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
                        elencoImportiEsteri[row.DataItemIndex].DecorrenzaPrestazioneEE = dateDecorrenza;
                        elencoImportiEsteri[row.DataItemIndex].CessazionePrestazioneEE = dateCessazione;
                        elencoImportiEsteri[row.DataItemIndex].ImportoPrestazioneEE = valueImporto;
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
                List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri = ((List<GestioneDatiContributiviCiPensioniCiImportiEsteri>)ViewState[EnumViewState.ElencoImportiEsteri.ToString()]);
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.DataItemIndex == 0) //primo record
                    {
                        if ((elencoImportiEsteri.Count == 1) &&
                            (elencoImportiEsteri.First().DecorrenzaPrestazioneEE == null || elencoImportiEsteri.First().DecorrenzaPrestazioneEE == DateTime.MinValue) &&
                            (elencoImportiEsteri.First().CessazionePrestazioneEE == null || elencoImportiEsteri.First().CessazionePrestazioneEE == DateTime.MinValue) &&
                            (elencoImportiEsteri.First().ImportoPrestazioneEE == null || elencoImportiEsteri.First().ImportoPrestazioneEE == decimal.MinValue))
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
                            save.ValidationGroup = "UCTabProrataGrid";
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
                            save.ValidationGroup = "UCTabProrataGrid";
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
                row.FindControl("txtImportoPrestazioneEE") != null && ((TextBox)row.FindControl("txtImportoPrestazioneEE")).Text != string.Empty) //&&
                //row.FindControl("txtCessazionePrestazioneEE") != null && ((TextBox)row.FindControl("txtCessazionePrestazioneEE")).Text != string.Empty)
                return false;
            else
                return true;
        }

        private void setCampiEditImportiEsteri(GridViewRowEventArgs e, bool IsFirstRecord)
        {
            TextBox txtDecorrenzaPrestazioneEE = (TextBox)e.Row.FindControl("txtDecorrenzaPrestazioneEE");
            TextBox txtCessazionePrestazioneEE = (TextBox)e.Row.FindControl("txtCessazionePrestazioneEE");
            TextBox txtImportoPrestazioneEE = (TextBox)e.Row.FindControl("txtImportoPrestazioneEE");
            string myDec = String.Format("{0:MM/yyyy}", ((GestioneDatiContributiviCiPensioniCiImportiEsteri)e.Row.DataItem).DecorrenzaPrestazioneEE);
            if (String.Equals(myDec, "01/0001"))
                txtDecorrenzaPrestazioneEE.Text = string.Empty;

            string mySosp = String.Format("{0:MM/yyyy}", ((GestioneDatiContributiviCiPensioniCiImportiEsteri)e.Row.DataItem).CessazionePrestazioneEE);
            if (String.Equals(mySosp, "01/0001"))
                txtCessazionePrestazioneEE.Text = "";

            string myImporti = ((GestioneDatiContributiviCiPensioniCiImportiEsteri)e.Row.DataItem).ImportoPrestazioneEE.ToString();
            if (String.Equals(myImporti, decimal.MinValue.ToString("dd/MM/yyyy")))
                txtImportoPrestazioneEE.Text = "";
        }

        private List<GestioneDatiContributiviCiPensioniCiImportiEsteri> EliminaRecordImportiVuoti(List<GestioneDatiContributiviCiPensioniCiImportiEsteri> elencoImportiEsteri)
        {
            int i = 0; int j = 0;
            int[] elementiDaEliminare = new int[30];
            foreach (GestioneDatiContributiviCiPensioniCiImportiEsteri importoEstero in elencoImportiEsteri)
            {
                if (importoEstero.CessazionePrestazioneEE == null && importoEstero.DecorrenzaPrestazioneEE == null &&
                    importoEstero.ImportoPrestazioneEE == null)
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
            ViewState[EnumViewState.DatiContributiviCi.ToString()] = area;
        }

        #endregion gvImportiEsteri

        #region EventHandler


        public static Comparison<GestioneContribStatoEstero> sortStatiEsteri = delegate(GestioneContribStatoEstero d1, GestioneContribStatoEstero d2)
        {
            try
            {
                int retValue = int.MinValue;
                retValue = string.Compare(d1.PrestazioneEstera._CodiceIstituzione, d2.PrestazioneEstera._CodiceIstituzione, false, CultureInfo.InvariantCulture);
                if (retValue == 0)
                {
                    retValue = string.Compare(d1.PrestazioneEstera._CodiceStatoEE, d2.PrestazioneEstera._CodiceStatoEE, false, CultureInfo.InvariantCulture);
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
        public event EventHandler AggiornaAnniTabIntegrazioneVirtuale;
        public event EventHandler AggiornaSemaforoTabIntegrazioneVirtuale;

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

        protected void RaiseAggiornaAnniTabIntegrazioneVirtuale(object sender, EventArgs e)
        {
            AggiornaAnniTabIntegrazioneVirtuale(sender, e);
        }

        protected void RaiseAggiornaSemaforoTabIntegrazioneVirtuale(object sender, EventArgs e)
        {
            AggiornaSemaforoTabIntegrazioneVirtuale(sender, e);
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

    public class extAreaImportiEsteri : GestioneDatiContributiviCiPensioniCiImportiEsteri
    {
        public extAreaImportiEsteri(GestioneDatiContributiviCiPensioniCiImportiEsteri area)
        {
            this.ImportoPrestazioneEE = area.ImportoPrestazioneEE;
            this.DecorrenzaPrestazioneEE = area.DecorrenzaPrestazioneEE;
            this.CessazionePrestazioneEE = area.CessazionePrestazioneEE;
        }

        public String strDecorrenzaPrestazione
        {
            get
            {
                if (this.DecorrenzaPrestazioneEE == DateTime.MinValue)
                {
                    return "";
                }
                String dp = this.DecorrenzaPrestazioneEE.ToString();
                String dp2 = String.Format("{0:MM/yyyy}", this.DecorrenzaPrestazioneEE);
                return dp2;
            }
        }

        public String strCessazionePrestazione
        {
            get
            {
                if (this.CessazionePrestazioneEE == DateTime.MinValue)
                {
                    return "";
                }
                String ds = this.CessazionePrestazioneEE.ToString();
                String ds2 = String.Format("{0:MM/yyyy}", this.CessazionePrestazioneEE);
                return ds2;
            }
        }

        public String strImportoPrestazione
        {
            get
            {
                if (this.ImportoPrestazioneEE == decimal.MinValue)
                {
                    return "";
                }
                String ip = this.ImportoPrestazioneEE.ToString();
                return ip;
            }
        }

    }
}


