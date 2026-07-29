using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCDatiAssicurativiINPDAP : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {
        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            RaiseHideAvviso(this, null);
        }

        protected void SalvaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP = GetDatiAssicurativi();
            areaLiquidazionePensioneAgo.ListaRipartizioneINPDAP = GetRipartizioniINPDAP();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiAssicurativiAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiAssicurativiAgo(this);
            if (!this.HasError)
            {
                ClearForm();
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

                ValorizzaEtichetteDatiAssicurativiCommon(this, datiPensione);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        #region metodi protected gvRipartizioni

        protected void gvRipartizioni_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_Grigliaripartizioni, Page.Theme);
                    }
                    else
                    {
                        CodeUtility.EnableReadableMode(e.Row.Cells[0], null, Page.Theme, null);
                    }
                    Label lblEnte = (Label)e.Row.FindControl("lblEnte");
                    lblEnte.Text = GetDescizioneRipartizioneINPDAP(((RipartizioneINPDAP)e.Row.DataItem).CodiceEnte);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiINPDAP, Errore nel metodo gvRipartizioni_RowDataBound " + ex);
            }
        }

        protected void gvRipartizioni_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRipartizioni.EditIndex = -1;
                gvRipartizioni.PageIndex = e.NewPageIndex;
                gvRipartizioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiINPDAP, Errore nel metodo gvRipartizioni_onPageIndexChanging" + ex);
            }
        }

        private void gvRipartizioni_Load()
        {
            try
            {
                gvRipartizioni.DataSource = (List<RipartizioneINPDAP>)ViewState[EnumViewState.RipartizioniINPDAP.ToString()];
                gvRipartizioni.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiINPDAP, Errore nel metodo gvRipartizioni_Load " + ex);
            }
        }

        protected void gvRipartizioni_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<RipartizioneINPDAP>)ViewState[EnumViewState.RipartizioniINPDAP.ToString()]).Count() < 2)
                    gvRipartizioni.EditIndex = 0;
                else
                    gvRipartizioni.EditIndex = -1;
                //Bind data to the GridView control.
                gvRipartizioni_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiINPDAP, Errore nel metodo gvRipartizioni_RowCancelingEdit " + ex);
            }

        }

        protected void gvRipartizioni_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvRipartizioni.EditIndex = e.NewEditIndex;
                gvRipartizioni_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiAssicurativiINPDAP, Errore nel metodo gvRipartizioni_RowEditing " + ex);
            }
        }

        protected void gvRipartizioni_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

                List<RipartizioneINPDAP> listaRipartizioni = (List<RipartizioneINPDAP>)ViewState[EnumViewState.RipartizioniINPDAP.ToString()];
                RipartizioneINPDAP ripartizionetoSave = listaRipartizioni.ElementAt(r.RowIndex);

                TextBox percentuale = (TextBox)r.FindControl(Keys.TxtPercentuale_GrigliaRipartizioni);

                ripartizionetoSave.Importo = CodeUtility.StringToNullableDecimal(percentuale.Text);

                gvRipartizioni.EditIndex = -1;
                gvRipartizioni_Load();

                #endregion
            }
            else if (e.CommandName == "Annulla")
            {
                gvRipartizioni.EditIndex = -1;
                gvRipartizioni_Load();
            }
        }

        #endregion metodi protected gvRipartizioni

        #region Common

        private void ValorizzaEtichetteDatiAssicurativiCommon(ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (liquidazione.areaLiquidazionePensioneAgo.ListaRipartizioneINPDAP != null)
                ViewState[EnumViewState.RipartizioniINPDAP.ToString()] = liquidazione.areaLiquidazionePensioneAgo.ListaRipartizioneINPDAP.ToList();

            if (liquidazione.areaLiquidazionePensioneAgo.TipoPensione != null)
            {
                lblTipoPensione.Text = liquidazione.areaLiquidazionePensioneAgo.TipoPensione.First().Key;
                hdnTipoPensione.Value = liquidazione.areaLiquidazionePensioneAgo.TipoPensione.First().Value.ToString();
            }

            if (datiPensione.DecorrenzaOriginaria == null)
                lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            else
            {
                lblDecorrenzaPensioneDatiAssicurativi.Text = String.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
            }

            REQFddlAttivitaSvoltaINPDAP.ErrorMessage = "Qualifica Professionale: Si prega di inserire la Qualifica Professionale";
            REQFtxtAttivitaSvoltaINPDAP.ErrorMessage = "Qualifica Professionale: Si prega di inserire la Qualifica Professionale";

            if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP != null)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.InizioAssicurazione != null)
                    txtPrimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.InizioAssicurazione);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.FineAssicurazione != null)
                    txtUltimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.FineAssicurazione);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.AttivitaSvolta != null)
                {
                    if (((int?)ViewState["CountListaAttivitaSvolta"]).GetValueOrDefault() <= 10)
                        ddlAttivitaSvoltaINPDAP.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.AttivitaSvolta;
                    else
                        txtAttivitaSvoltaINPDAP.Text = GetAttivitaSvoltaAgo(liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.AttivitaSvolta, liquidazione);
                }
                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.CausaCessazione.HasValue)
                    txtCausaCessazione.Text = GetCausaCessazione(liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.CausaCessazione.Value);

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.CodiceSpecifico.HasValue)
                    ddlCodiceSpecifico.SelectedValue = liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.CodiceSpecifico.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.TitolareAltraPensione.HasValue)
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.TitolareAltraPensione.Value)
                        ddlTitAltraPensione.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.TitolareAltraPensione.Value)
                        ddlTitAltraPensione.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.DirittoIndennitaIntegrativaSpeciale.HasValue)
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.IntegrazioneMinimo.HasValue)
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.IntegrazioneMinimo.Value)
                        ddlIntegrazioneMinimo.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.IntegrazioneMinimo.Value)
                        ddlIntegrazioneMinimo.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.RiduzioneL537.HasValue)
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.IISAbbattimentoAnni.HasValue)
                    if (liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "NO";
            }

            gvRipartizioni_Load();
        }

        private DatiAssicurativiINPDAP GetDatiAssicurativiCommon()
        {
            DatiAssicurativiINPDAP datiAssicurativi = new DatiAssicurativiINPDAP();

            if (!(String.Equals(txtPrimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtPrimoVersamento.Text)))
                datiAssicurativi.InizioAssicurazione = Utility.GetDateFromString(txtPrimoVersamento.Text);

            if (!(String.Equals(txtUltimoVersamento.Text, "gg/mm/aaaa")) && (!String.IsNullOrEmpty(txtUltimoVersamento.Text)))
                datiAssicurativi.FineAssicurazione = Utility.GetDateFromString(txtUltimoVersamento.Text);

            if (!string.IsNullOrEmpty(ddlCodiceSpecifico.SelectedValue))
                datiAssicurativi.CodiceSpecifico = byte.Parse(ddlCodiceSpecifico.SelectedValue);

            if (!String.IsNullOrEmpty(txtCausaCessazione.Text))
                datiAssicurativi.CausaCessazione = ControlCausaCessazione(txtCausaCessazione.Text);

            if (((int?)ViewState["CountListaAttivitaSvolta"]).GetValueOrDefault() <= 10)
                datiAssicurativi.AttivitaSvolta = ddlAttivitaSvoltaINPDAP.SelectedValue;
            else
                datiAssicurativi.AttivitaSvolta = ControlAttivitaSvolta(ddlAttivitaSvoltaINPDAP.SelectedValue);

            if (String.Equals(ddlTitAltraPensione.SelectedValue, "SI"))
                datiAssicurativi.TitolareAltraPensione = true;
            else if (String.Equals(ddlTitAltraPensione.SelectedValue, "NO"))
                datiAssicurativi.TitolareAltraPensione = false;

            if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "SI"))
                datiAssicurativi.DirittoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "NO"))
            {
                datiAssicurativi.DirittoIndennitaIntegrativaSpeciale = false;
                ddlRiduzioneL537.SelectedValue = "NO";
                ddlIISAbbattimentoAnni.SelectedValue = "NO";
            }

            if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "SI"))
                datiAssicurativi.IntegrazioneMinimo = true;
            else if (String.Equals(ddlIntegrazioneMinimo.SelectedValue, "NO"))
                datiAssicurativi.IntegrazioneMinimo = false;

            if (String.Equals(ddlRiduzioneL537.SelectedValue, "SI"))
                datiAssicurativi.RiduzioneL537 = true;
            else if (String.Equals(ddlRiduzioneL537.SelectedValue, "NO"))
                datiAssicurativi.RiduzioneL537 = false;

            if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "SI"))
                datiAssicurativi.IISAbbattimentoAnni = true;
            else if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "NO"))
                datiAssicurativi.IISAbbattimentoAnni = false;

            return datiAssicurativi;
        }

        #endregion

        #region Private Methods

        private string GetDescizioneRipartizioneINPDAP(long codiceEnte)
        {
            List<DecodificaEnteRipartizioneINPDAP> listaDecodfica = (List<DecodificaEnteRipartizioneINPDAP>)ViewState[EnumViewState.DecEnteRipartizioneINPDAP.ToString()];
            DecodificaEnteRipartizioneINPDAP dec = listaDecodfica.Find(x => x.Id == codiceEnte);
            if (dec != null)
                return dec.Descrizione;
            return string.Empty;
        }

        private void LoadDdl(ILiquidazionePensioneAgo liquidazione, AreaDecodifica datiDecodifica)
        {
            List<CodiceSpecifico> listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneAgo.ListaCodiceSpecifico.ToList().FindAll(delegate (CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneAgo.TipoPensione.First().Value.ToString() &&
                  code.Fondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS.ToString().ToUpperInvariant());
            });

            ViewState["CodiciNatura"] = liquidazione.areaLiquidazionePensioneAgo.listaCodiciNatura;

            ddlCodiceSpecifico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceSpecifico);
            foreach (CodiceSpecifico codSpec in listaCodiceSpecifico)
                CodeUtility.SetValueDdl(ddlCodiceSpecifico, codSpec.Descrizione, codSpec.Descrizione, codSpec.Id.Value.ToString());

            string elencoCausaCessazione = string.Empty;
            foreach (CausaCessazione causaCessazione in liquidazione.areaLiquidazionePensioneAgo.ListaCausaCessazione.ToList())
            {
                elencoCausaCessazione = string.Concat(causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione, ";");
                HiddenFieldCausaCessazione.Value = string.Concat(HiddenFieldCausaCessazione.Value, elencoCausaCessazione);
            }

            ViewState["CountListaAttivitaSvolta"] = liquidazione.areaLiquidazionePensioneAgo.ListaAttivitaSvolte.Count();

            if (((int?)ViewState["CountListaAttivitaSvolta"]).GetValueOrDefault() <= 10)
            {
                pnlDDLAttivitaSvoltaINPDAP.Visible = true;
                ddlAttivitaSvoltaINPDAP.Items.Clear();
                CodeUtility.SetItemBlankDdl(ddlAttivitaSvoltaINPDAP);
                foreach (AttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneAgo.ListaAttivitaSvolte.ToList())
                    CodeUtility.SetValueDdl(ddlAttivitaSvoltaINPDAP, attivitaSvolta.Descrizione, attivitaSvolta.Descrizione, attivitaSvolta.Id);
            }
            else
            {
                pnlTXTAttivitaSvoltaINPDAP.Visible = true;
                string elencoAttivitaSvolta = string.Empty;
                foreach (AttivitaSvolta attivitaSvolta in liquidazione.areaLiquidazionePensioneAgo.ListaAttivitaSvolte)
                {
                    elencoAttivitaSvolta = string.Concat(attivitaSvolta.TraduzioneSuGp + " - " + attivitaSvolta.Descrizione + " (" + attivitaSvolta.LimiteEta + " - " + attivitaSvolta.LimiteServizio + ")", ";");
                    hiddenAttivitaSvolte.Value = string.Concat(hiddenAttivitaSvolte.Value, elencoAttivitaSvolta);
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, -1);
        }

        private void RenderControlsCommon(ILiquidazionePensioneAgo liquidazione)
        {
            pnlCommonHeader.Visible = true;
            pnlCustomINPDAP.Visible = true;
            pnlCustomINPDAP.Visible = true;
            pnlDecAnteAgosto95.Visible = liquidazione.areaLiquidazionePensioneAgo.IsDecPensAnteAgosto95.Value;
        }

        private void ManageDecorrenzaForReversibilita(AreaTitolare.DatiPensione datiPensione, DateTime? decorrenzaPensioneDirettaDC)
        {
            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
                ViewState["DecorrenzaPensione"] = decorrenzaPensioneDirettaDC;
            else
                ViewState["DecorrenzaPensione"] = datiPensione.DecorrenzaOriginaria;
        }

        private string ControlAttivitaSvolta(string attivitaSvoltaInserita)
        {
            string attivitaSvolta = string.Empty;
            List<AttivitaSvolta> listaAttivitaSvolte = (List<AttivitaSvolta>)ViewState["ListaAttivitaSvolte"];

            if (!string.IsNullOrEmpty(attivitaSvoltaInserita))
            {
                char[] separatori = { '-', '(', ')' };
                string traduzioneSuGP = attivitaSvoltaInserita.Split(separatori).ElementAt(0).Trim();
                string descrizione = attivitaSvoltaInserita.Split(separatori).ElementAt(1).Trim();
                string limiteEta = attivitaSvoltaInserita.Split(separatori).ElementAt(2).Trim();
                string limiteServizio = attivitaSvoltaInserita.Split(separatori).ElementAt(3).Trim();

                foreach (AttivitaSvolta attivitaSvoltaDB in listaAttivitaSvolte)
                {
                    if (attivitaSvoltaDB.TraduzioneSuGp.Trim() == traduzioneSuGP && attivitaSvoltaDB.Descrizione.Trim().ToUpperInvariant() == descrizione.ToUpperInvariant() &&
                        attivitaSvoltaDB.LimiteEta == byte.Parse(limiteEta) && attivitaSvoltaDB.LimiteServizio == byte.Parse(limiteServizio))
                    {
                        attivitaSvolta = attivitaSvoltaDB.Id;
                        break;
                    }
                }
            }

            return attivitaSvolta;
        }

        #endregion

        #region internal methods

        internal void ValorizzaEtichetteDatiAssicurativi(ILiquidazionePensioneAgo liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ManageDecorrenzaForReversibilita(datiPensione, liquidazione.areaLiquidazionePensioneAgo.DecorrenzaPensioneDirettaDC);

            ViewState["ListaAttivitaSvolte"] = liquidazione.areaLiquidazionePensioneAgo.ListaAttivitaSvolte.ToList();
            if (liquidazione.areaLiquidazionePensioneAgo.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneAgo.IsUsuranti.Value)
                ViewState["IsUsuranti"] = "SI";

            if (IsDomandaSperDonna)
                ViewState["IsDomandaSperDonna"] = "SI";

            if (liquidazione.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.HasValue)
                ViewState["IsCodNatura2Enabled"] = liquidazione.areaLiquidazionePensioneAgo.IsCodiceNatura2Enabled.Value;

            if (liquidazione.areaLiquidazionePensioneAgo.IsSperimentaleDonna.HasValue)
                ViewState["IsCodiceNatura2DisabledPerSperDonna"] = liquidazione.areaLiquidazionePensioneAgo.IsSperimentaleDonna.Value;

            if (liquidazione.areaLiquidazionePensioneAgo.ListaDecEnteRipartizioneINPDAP != null)
                ViewState[EnumViewState.DecEnteRipartizioneINPDAP.ToString()] = liquidazione.areaLiquidazionePensioneAgo.ListaDecEnteRipartizioneINPDAP.ToList();

            if (liquidazione.areaLiquidazionePensioneAgo.ListaCausaCessazione != null)
                ViewState["elencoCausaCessazione"] = liquidazione.areaLiquidazionePensioneAgo.ListaCausaCessazione.ToList();

            RenderControlsCommon(liquidazione);

            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            LoadDdl(liquidazione, datiDecodifica);
            ValorizzaEtichetteDatiAssicurativiCommon(liquidazione, datiPensione);
        }

        internal DatiAssicurativiINPDAP GetDatiAssicurativi()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.DatiAssicurativiINPDAP();

            areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP = GetDatiAssicurativiCommon();

            return areaLiquidazionePensioneAgo.DatiAssicurativiINPDAP;
        }

        internal RipartizioneINPDAP[] GetRipartizioniINPDAP()
        {
            return ((List<RipartizioneINPDAP>)ViewState[EnumViewState.RipartizioniINPDAP.ToString()]).ToArray();
        }

        #endregion internal methods

        #region INPDAP Ago

        private string GetAttivitaSvoltaAgo(string idAttivitaSvolta, ILiquidazionePensioneAgo liquidazione)
        {
            if (!string.IsNullOrEmpty(idAttivitaSvolta))
            {
                AttivitaSvolta attivitaSvolta = Array.Find(liquidazione.areaLiquidazionePensioneAgo.ListaAttivitaSvolte, x => x.Id == idAttivitaSvolta);
                return attivitaSvolta.TraduzioneSuGp + " - " + attivitaSvolta.Descrizione + " (" + attivitaSvolta.LimiteEta + " - " + attivitaSvolta.LimiteServizio + ")";
            }
            else return string.Empty;
        }

        private long? ControlCausaCessazione(string causaCessazioneInserita)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            long? codCausaCessazione = null;
            string[] cessazione = causaCessazioneInserita.Split('-');

            if (elencoCausaCessazione != null)
            {
                foreach (CausaCessazione causaCessazione in elencoCausaCessazione)
                {
                    if (causaCessazione.TraduzioneSuGP.ToUpperInvariant().Trim() == cessazione[0].ToUpperInvariant().Trim() && causaCessazione.Descrizione.ToUpperInvariant().Trim() == cessazione[1].Trim().ToUpperInvariant())
                        codCausaCessazione = causaCessazione.Id;
                }
            }

            return codCausaCessazione;
        }

        private string GetCausaCessazione(long? codCausaCessazione)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            if (codCausaCessazione.HasValue && elencoCausaCessazione != null)
            {
                CausaCessazione causaCessazione = elencoCausaCessazione.Find(delegate (CausaCessazione code)
                { return (code.Id == codCausaCessazione.Value); });
                return causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione;
            }
            else return string.Empty;
        }

        #endregion INPDAP Ago

        #region EventHandler

        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler GetDecorrenzaPensione;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event Utility.CustomEventHandler HideAvviso;

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        protected void RaiseDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (DisabilitaTastoSalva != null)
                DisabilitaTastoSalva(sender, e);
        }

        protected void RaiseGetDecorrenzaPensione(object sender, EventArgs e)
        {
            if (GetDecorrenzaPensione != null)
                GetDecorrenzaPensione(sender, e);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseHideAvviso(object sender, Utility.CustomEventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }


        #endregion EventHandler

        #region Keys
        private class Keys
        {
            public const string ValidationGroup_Grigliaripartizioni = "GrigliaRipartizioni";
            public const string TxtPercentuale_GrigliaRipartizioni = "txtPercentuale";
        }
        #endregion Keys

        public enum EnumViewState
        {
            RipartizioniINPDAP,
            DecEnteRipartizioneINPDAP
        }
    }
}
