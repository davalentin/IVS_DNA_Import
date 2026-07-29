using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiAssicurativiINPDAP : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensione
    {
        #region ILiquidazionePensione
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);
            RaiseHideAvviso(this, Cevent);
        }

        protected void SalvaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativiINPDAP = GetDatiAssicurativi();
            areaLiquidazionePensioneFS.ListaRipartizioneINPDAP = GetRipartizioniINPDAP();

            PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiAssicurativiFS(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiAssicurativi_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiAssicurativiFS(this);
            if (!this.HasError)
            {
                ClearForm();
                AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

                ValorizzaEtichetteDatiAssicurativiCommon(this, datiPensione);
            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);
            RaiseShowAvvisoElimina(this, Cevent);
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

                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                gvRipartizioni.EditIndex = -1;
                gvRipartizioni_Load();
            }
        }

        #endregion metodi protected gvRipartizioni

        #region Common

        private void ValorizzaEtichetteDatiAssicurativiCommon(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione.SceltaLavoratriciMadri.HasValue)
                hdnSKIP_ManageEnableBeneficiJS.Value = "TRUE";

            if (liquidazione.areaLiquidazionePensioneFS.ListaRipartizioneINPDAP != null)
                ViewState[EnumViewState.RipartizioniINPDAP.ToString()] = liquidazione.areaLiquidazionePensioneFS.ListaRipartizioneINPDAP.ToList();

            if (liquidazione.areaLiquidazionePensioneFS.TipoPensione != null)
            {
                lblTipoPensione.Text = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Key;
                hdnTipoPensione.Value = liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString();
            }

            if (datiPensione.DecorrenzaOriginaria == null)
                lblDecorrenzaPensioneDatiAssicurativi.Text = "";
            else
            {
                lblDecorrenzaPensioneDatiAssicurativi.Text = String.Format("{0:dd/MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
            }

            REQFddlMicroqualificaINPDAP.ErrorMessage = "Microqualifica: Si prega di inserire la Microqualifica";
            REQFtxtMicroqualificaINPDAP.ErrorMessage = "Microqualifica: Si prega di inserire la Microqualifica";
            Utility.TipoUnicarpe TipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura) && TipoUnicarpe != Utility.TipoUnicarpe.Automatica)
                RequiredCausaCessazione.Enabled = true;
            
            if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.InizioAssicurazione != null)
                    txtPrimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.InizioAssicurazione);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.FineAssicurazione != null)
                    txtUltimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.FineAssicurazione);

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Microqualifica.HasValue)
                {
                    if (((int?)ViewState["CountListaMicroqualifica"]).GetValueOrDefault() <= 10)
                    {
                        if (ddlMicroqualificaINPDAP.Items.FindByValue(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Microqualifica.Value.ToString()) != null)
                            ddlMicroqualificaINPDAP.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Microqualifica.Value.ToString();
                    }
                    else
                        txtMicroqualificaINPDAP.Text = GetMicroqualifica(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Microqualifica, liquidazione);
                }
               

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CausaCessazione.HasValue)
                    ddlCausaCessazione.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CausaCessazione.Value.ToString();
               

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CodiceSpecifico.HasValue)
                    ddlCodiceSpecifico.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CodiceSpecifico.Value.ToString();
                else if (ddlCodiceSpecifico.Items.Count == 2)
                {
                    ddlCodiceSpecifico.SelectedIndex = 1;
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.DirittoIndennitaIntegrativaSpeciale.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.DirittoIndennitaIntegrativaSpeciale.Value)
                        ddlDirittoIndennIntegrSpec.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.RiduzioneL537.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.RiduzioneL537.Value)
                        ddlRiduzioneL537.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.IISAbbattimentoAnni.HasValue)
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "SI";
                    else if (!liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.IISAbbattimentoAnni.Value)
                        ddlIISAbbattimentoAnni.SelectedValue = "NO";

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliDirittoAA.HasValue)
                    txtVVUtiliDirittoAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliDirittoAA.ToString();
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliDirittoMM.HasValue)
                    txtVVUtiliDirittoMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliDirittoMM.ToString();
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliDirittoGG.HasValue)
                    txtVVUtiliDirittoGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliDirittoGG.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliMisuraAA.HasValue)
                    txtVVUtiliMisuraAA.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliMisuraAA.ToString();
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliMisuraMM.HasValue)
                    txtVVUtiliMisuraMM.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliMisuraMM.ToString();
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliMisuraGG.HasValue)
                    txtVVUtiliMisuraGG.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.VVUtiliMisuraGG.ToString();

                //**Revisione Campi INPDAP**
                //if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.AttivitaEconomica.HasValue)
                //    txtAttivitaEconomica.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.AttivitaEconomica.ToString().PadLeft(2, '0');

                //if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.ProfessioneIndividuale.HasValue)
                //    txtProfessioneIndividuale.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.ProfessioneIndividuale.ToString().PadLeft(3, '0');

                //if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.AnniMax.HasValue)
                //    txtAnniMax.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.AnniMax.ToString();

                //if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.AnniUtili.HasValue)
                //    txtAnniUtili.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.AnniUtili.ToString();
                //***


                //I tre campi Comparto, Settore e Ruolo sono collegati in maniera gerarchica
                //Nel value di settore e ruolo sono presenti i relativi "padri" separati da ";"
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Comparto.HasValue)
                {
                    if (ddlComparto.Items.FindByValue(liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Comparto.Value.ToString()) != null)
                        ddlComparto.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Comparto.Value.ToString();

                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Settore.HasValue)
                    {
                        var valueSettore = "$" + liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Comparto.Value.ToString() + ";"
                            + liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Settore.Value.ToString();
                        if (ddlSettore.Items.FindByValue(valueSettore) != null)
                            ddlSettore.SelectedValue = valueSettore;

                        if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Ruolo.HasValue)
                        {
                            var valueRuolo = "$" + liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Comparto.Value.ToString() + ";"
                            + liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Settore.Value.ToString() + ";"
                            + liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.Ruolo.Value.ToString();
                            if (ddlRuolo.Items.FindByValue(valueRuolo) != null)
                                ddlRuolo.SelectedValue = valueRuolo;
                        }
                    }
                }
            }

            if (Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura) && datiPensione.CodeGruppo == "0001" && datiPensione.CodeProdotto == "0002" && datiPensione.CodeTipo != "0172")
            {
                ddlCodiceSpecifico.SelectedIndex = 1;
                ddlCodiceSpecifico.Enabled = false;
            }

            if (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
            {
                if (Utility.IsDomandaInabilitaLegge335(datiPensione))
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CodiceSpecifico.HasValue)
                    {
                        var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, 'F');
                        ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != 0 ? idCodiceSpecifico.ToString() : "";
                    }
                    ddlCodiceSpecifico.Enabled = false;
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CausaCessazione.HasValue)
                        ddlCausaCessazione.SelectedValue = GetIdCausaCessazioneByTraduzionesuGP("0006");
                    ddlCausaCessazione.Enabled = Utility.IsDomandaOrganizzazioniInternazionali(datiPensione); 
                }

                if (Utility.IsDomandaInabilitaProficuoLavoro(datiPensione))
                {
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CodiceSpecifico.HasValue)
                    {
                        var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, 'E');
                        ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                    }
                    ddlCodiceSpecifico.Enabled = false;
                    if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP == null || !liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CausaCessazione.HasValue)
                        ddlCausaCessazione.SelectedValue = GetIdCausaCessazioneByTraduzionesuGP("0005");

                    ddlCausaCessazione.Enabled = false; 
                }

                if (datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione && Utility.IsDomandaPL(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico != null && liquidazione.areaLiquidazionePensioneFS.TipoPensione != null)
                    {
                        var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, 'J');
                        ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                        ddlCodiceSpecifico.Enabled = false;
                    }
                }
            }
            else
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP != null && liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CodiceSpecifico.HasValue)
                    ddlCodiceSpecifico.Enabled = false;
            }
            if (Utility.IsDomandaInabilitaPrivilegioGestionePubblica(datiPensione))
            {
                if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico != null && liquidazione.areaLiquidazionePensioneFS.TipoPensione != null)
                {
                    var idCodiceSpecifico = GetIdCodiceSpecificoByTraduzioneSuGP(liquidazione, 'G');
                    ddlCodiceSpecifico.SelectedValue = idCodiceSpecifico != null ? idCodiceSpecifico.ToString() : "";
                    ddlCodiceSpecifico.Enabled = false;
                }
            }

            Utility.TipoUnicarpe tipoUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true);
            if (tipoUnicarpe == Utility.TipoUnicarpe.Automatica)
            {
                if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN)
                {
                    ddlMicroqualificaINPDAP.Enabled = false;
                    txtMicroqualificaINPDAP.Enabled = false;
                    //**Revisione Campi INPDAP**
                    //txtAnniMax.Enabled = false;
                    //txtAnniUtili.Enabled = false;
                }
                //**Revisione Campi INPDAP**
                //txtCausaCessazione.Enabled = false;
                txtPrimoVersamento.Enabled = false;
                txtUltimoVersamento.Enabled = false;

                if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
                {
                    ddlCausaCessazione.Enabled = false;
                    txtProgAmministrazione.Enabled = false;
                    txtCfAmministrazione.Enabled = false;
                }
            }

            //**Revisione Campi INPDAP**
            if (Utility.IsDomandaCTPS(this.domanda.Categoria))
                gvRipartizioni_Load();
            else
                pnlRipartizioni.Visible = false;

            bool isAbilitaFlussoNoteDiDebito = ConfigurationManager.AppSettings["AbilitaFlussoNoteDiDebito"] != null && ConfigurationManager.AppSettings["AbilitaFlussoNoteDiDebito"] == "SI";
            if (isAbilitaFlussoNoteDiDebito)
            {
                pnlAmministrazione.Visible = true;
                if (liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP != null)
                {
                    txtProgAmministrazione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.ProgAmministrazione;
                    txtCfAmministrazione.Text = liquidazione.areaLiquidazionePensioneFS.DatiAssicurativiINPDAP.CfAmministrazione;
                }
            }
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

            datiAssicurativi.CausaCessazione = ddlCausaCessazione.SelectedIndex != 0 ? Convert.ToInt64(ddlCausaCessazione.SelectedValue) : (Int64?)null;


            if (((int?)ViewState["CountListaMicroqualifica"]).GetValueOrDefault() <= 10)
            {
                if (!string.IsNullOrEmpty(ddlMicroqualificaINPDAP.SelectedValue))
                    datiAssicurativi.Microqualifica = CodeUtility.StringToNullableLong(ddlMicroqualificaINPDAP.SelectedValue);
            }
            else
            {
                if (!string.IsNullOrEmpty(txtMicroqualificaINPDAP.Text))
                    datiAssicurativi.Microqualifica = ControlMicroqualifica(txtMicroqualificaINPDAP.Text);
            }

            if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "SI"))
                datiAssicurativi.DirittoIndennitaIntegrativaSpeciale = true;
            else if (String.Equals(ddlDirittoIndennIntegrSpec.SelectedValue, "NO"))
            {
                datiAssicurativi.DirittoIndennitaIntegrativaSpeciale = false;
                ddlRiduzioneL537.SelectedValue = "NO";
                ddlIISAbbattimentoAnni.SelectedValue = "NO";
            }

            if (String.Equals(ddlRiduzioneL537.SelectedValue, "SI"))
                datiAssicurativi.RiduzioneL537 = true;
            else if (String.Equals(ddlRiduzioneL537.SelectedValue, "NO"))
                datiAssicurativi.RiduzioneL537 = false;

            if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "SI"))
                datiAssicurativi.IISAbbattimentoAnni = true;
            else if (String.Equals(ddlIISAbbattimentoAnni.SelectedValue, "NO"))
                datiAssicurativi.IISAbbattimentoAnni = false;

            if (!String.IsNullOrEmpty(txtVVUtiliDirittoAA.Text))
                datiAssicurativi.VVUtiliDirittoAA = CodeUtility.StringToNullableShort(txtVVUtiliDirittoAA.Text);
            if (!String.IsNullOrEmpty(txtVVUtiliDirittoMM.Text))
                datiAssicurativi.VVUtiliDirittoMM = CodeUtility.StringToNullableByte(txtVVUtiliDirittoMM.Text);
            if (!String.IsNullOrEmpty(txtVVUtiliDirittoGG.Text))
                datiAssicurativi.VVUtiliDirittoGG = CodeUtility.StringToNullableByte(txtVVUtiliDirittoGG.Text);

            if (!String.IsNullOrEmpty(txtVVUtiliMisuraAA.Text))
                datiAssicurativi.VVUtiliMisuraAA = CodeUtility.StringToNullableShort(txtVVUtiliMisuraAA.Text);
            if (!String.IsNullOrEmpty(txtVVUtiliMisuraMM.Text))
                datiAssicurativi.VVUtiliMisuraMM = CodeUtility.StringToNullableByte(txtVVUtiliMisuraMM.Text);
            if (!String.IsNullOrEmpty(txtVVUtiliMisuraGG.Text))
                datiAssicurativi.VVUtiliMisuraGG = CodeUtility.StringToNullableByte(txtVVUtiliMisuraGG.Text);

            //**Revisione Campi INPDAP**
            //if (!String.IsNullOrEmpty(txtAttivitaEconomica.Text))
            //    datiAssicurativi.AttivitaEconomica = CodeUtility.StringToNullableShort(txtAttivitaEconomica.Text);

            //if (!String.IsNullOrEmpty(txtProfessioneIndividuale.Text))
            //    datiAssicurativi.ProfessioneIndividuale = CodeUtility.StringToNullableShort(txtProfessioneIndividuale.Text);

            //if (!string.IsNullOrEmpty(txtAnniMax.Text))
            //    datiAssicurativi.AnniMax = CodeUtility.StringToNullableByte(txtAnniMax.Text);

            //if (!string.IsNullOrEmpty(txtAnniUtili.Text))
            //    datiAssicurativi.AnniUtili = CodeUtility.StringToNullableByte(txtAnniUtili.Text);
            //***


            //I tre campi Comparto, Settore e Ruolo sono collegati in maniera gerarchica
            //Nel value di settore e ruolo sono presenti i relativi "padri" separati da ";"
            if (!string.IsNullOrEmpty(ddlComparto.SelectedValue))
                datiAssicurativi.Comparto = CodeUtility.StringToNullableInt(ddlComparto.SelectedValue);

            if (!string.IsNullOrEmpty(ddlSettore.SelectedValue))
            {
                var settoreValue = ddlSettore.SelectedValue.Substring(ddlSettore.SelectedValue.LastIndexOf(';') + 1);
                datiAssicurativi.Settore = CodeUtility.StringToNullableInt(settoreValue);
            }

            if (!string.IsNullOrEmpty(ddlRuolo.SelectedValue))
            {
                var ruoloValue = ddlRuolo.SelectedValue.Substring(ddlRuolo.SelectedValue.LastIndexOf(';') + 1);
                datiAssicurativi.Ruolo = CodeUtility.StringToNullableInt(ruoloValue);
            }

            bool isAbilitaFlussoNoteDiDebito = ConfigurationManager.AppSettings["AbilitaFlussoNoteDiDebito"] != null && ConfigurationManager.AppSettings["AbilitaFlussoNoteDiDebito"] == "SI";
            if (isAbilitaFlussoNoteDiDebito)
            {
                datiAssicurativi.ProgAmministrazione = txtProgAmministrazione.Text;
                datiAssicurativi.CfAmministrazione = txtCfAmministrazione.Text;
            }
            return datiAssicurativi;
        }

        #endregion Common

        #region Private Methods

        private string GetDescizioneRipartizioneINPDAP(long codiceEnte)
        {
            List<DecodificaEnteRipartizioneINPDAP> listaDecodfica = (List<DecodificaEnteRipartizioneINPDAP>)ViewState[EnumViewState.DecEnteRipartizioneINPDAP.ToString()];
            DecodificaEnteRipartizioneINPDAP dec = listaDecodfica.Find(x => x.Id == codiceEnte);
            if (dec != null)
                return dec.Descrizione;
            return string.Empty;
        }

        private void LoadDdl(ILiquidazionePensione liquidazione, AreaDecodifica datiDecodifica)
        {
            List<CodiceSpecifico> listaCodiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().FindAll(delegate (CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.FirstOrDefault().Value.ToString() &&
                  code.Fondo == "DAP");
            });

            ddlCausaCessazione.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCausaCessazione);
            foreach (CausaCessazione causaCessazione in liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList())
                CodeUtility.SetValueDdl(ddlCausaCessazione, causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione, causaCessazione.Descrizione, causaCessazione.Id.ToString());


            ViewState["CodiciNatura"] = liquidazione.areaLiquidazionePensioneFS.ListaCodiciNatura;

            ddlCodiceSpecifico.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlCodiceSpecifico);
            foreach (CodiceSpecifico codSpec in listaCodiceSpecifico)
                CodeUtility.SetValueDdl(ddlCodiceSpecifico, codSpec.Descrizione, codSpec.Descrizione, codSpec.Id.Value.ToString());

            if (listaCodiceSpecifico != null && listaCodiceSpecifico.Count == 1)
            {
                ddlCodiceSpecifico.SelectedIndex = 1;
                ddlCodiceSpecifico.Enabled = false;
            }

            string elencoCausaCessazione = string.Empty;
            if (liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione != null)
            {
                foreach (CausaCessazione causaCessazione in liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList())
                {
                    elencoCausaCessazione = string.Concat(causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione, ";");
                    HiddenFieldCausaCessazione.Value = string.Concat(HiddenFieldCausaCessazione.Value, elencoCausaCessazione);
                }
            }
            ViewState["CountListaMicroqualifica"] = liquidazione.areaLiquidazionePensioneFS.ListaAttivitaSvolte.Count();

            if (((int?)ViewState["CountListaMicroqualifica"]).GetValueOrDefault() <= 10)
            {
                pnlDDLMicroqualificaINPDAP.Visible = true;
                ddlMicroqualificaINPDAP.Items.Clear();
                CodeUtility.SetItemBlankDdl(ddlMicroqualificaINPDAP);
                foreach (MicroqualificaINPDAP microqualifica in liquidazione.areaLiquidazionePensioneFS.ListaMicroqualificaINPDAP.ToList())
                    CodeUtility.SetValueDdl(ddlMicroqualificaINPDAP, microqualifica.Descrizione, microqualifica.Descrizione, microqualifica.Id.ToString());
            }
            else
            {
                pnlTXTMicroqualificaINPDAP.Visible = true;
                string elencoMicroqualifica = string.Empty;
                foreach (MicroqualificaINPDAP microqualifica in liquidazione.areaLiquidazionePensioneFS.ListaMicroqualificaINPDAP)
                {
                    elencoMicroqualifica = string.Concat(microqualifica.TraduzioneSuGP + " - " + microqualifica.Descrizione, ";");
                    hiddenMicroqualifica.Value = string.Concat(hiddenMicroqualifica.Value, elencoMicroqualifica);
                }
            }

            pnlComparto.Visible = true;
            ddlComparto.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlComparto);
            if (liquidazione.areaLiquidazionePensioneFS.ListaCtrlCompartoSettoreRuolo != null)
            {
                var listaComparti = liquidazione.areaLiquidazionePensioneFS.ListaCtrlCompartoSettoreRuolo.ToList().GroupBy(x => x.CodiceComparto).Select(x => x);
                foreach (var comparto in listaComparti)
                {
                    var descrizione = datiDecodifica.ElencoDecComparto.Where(x => x.Codice == comparto.Key).Select(x => x.Descrizione).FirstOrDefault();
                    CodeUtility.SetValueDdl(ddlComparto, comparto.Key.ToString() + " - " + descrizione, comparto.Key.ToString(), comparto.Key.ToString());
                }
            }

            ddlSettore.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlSettore);
            if (liquidazione.areaLiquidazionePensioneFS.ListaCtrlCompartoSettoreRuolo != null)
            {
                var listaComparti = liquidazione.areaLiquidazionePensioneFS.ListaCtrlCompartoSettoreRuolo.ToList().GroupBy(x => new { x.CodiceComparto, x.CodiceSettore }).Select(x => new { CodiceComparto = x.Key.CodiceComparto, CodiceSettore = x.Key.CodiceSettore });
                foreach (var comparto in listaComparti)
                {
                    var descrizione = datiDecodifica.ElencoDecSettore.Where(x => x.Codice == comparto.CodiceSettore).Select(x => x.Descrizione).FirstOrDefault();
                    CodeUtility.SetValueDdl(ddlSettore, comparto.CodiceSettore.ToString() + " - " + descrizione, comparto.CodiceSettore.ToString(), "$" + comparto.CodiceComparto.ToString() + ";" + comparto.CodiceSettore.ToString());
                }
            }

            ddlRuolo.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlRuolo);
            if (liquidazione.areaLiquidazionePensioneFS.ListaCtrlCompartoSettoreRuolo != null)
            {
                foreach (var comparto in liquidazione.areaLiquidazionePensioneFS.ListaCtrlCompartoSettoreRuolo)
                {
                    var descrizione = datiDecodifica.ElencoDecRuolo.Where(x => x.Codice == comparto.CodiceRuolo).Select(x => x.Descrizione).FirstOrDefault();
                    CodeUtility.SetValueDdl(ddlRuolo, comparto.CodiceRuolo.ToString() + " - " + descrizione, comparto.CodiceRuolo.ToString(), "$" + comparto.CodiceComparto.ToString() + ";" + comparto.CodiceSettore.ToString() + ";" + comparto.CodiceRuolo.ToString());
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, -1);
        }

        private void RenderControlsCommon(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            pnlCommonHeader.Visible = true;
            pnlCustomINPDAP.Visible = true;
            pnlCustomINPDAP.Visible = true;
            pnlDecAnteAgosto95.Visible = liquidazione.areaLiquidazionePensioneFS.IsDecPensAnteAgosto95.Value;
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                pnlVVUtiliDiritto.Visible = true;
                pnlVVUtiliMisura.Visible = true;
                //**Revisione Campi INPDAP**
                //pnlAttivitaEconomica.Visible = true;
                //pnlProfessioneIndividuale.Visible = true;
            }
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

        private long? ControlMicroqualifica(string microqualificaInserita)
        {
            long? microqualifica = null;
            List<MicroqualificaINPDAP> listaMicroqualifica = (List<MicroqualificaINPDAP>)ViewState["ListaMicroqualifica"];

            if (!string.IsNullOrEmpty(microqualificaInserita))
            {
                char[] separatori = { '-' };
                string traduzioneSuGP = microqualificaInserita.Split(separatori).ElementAt(0).Trim();
                string descrizione = microqualificaInserita.Split(separatori).ElementAt(1).Trim();

                foreach (MicroqualificaINPDAP microqualificaDB in listaMicroqualifica)
                {
                    if (microqualificaDB.TraduzioneSuGP.Trim() == traduzioneSuGP && microqualificaDB.Descrizione.Trim().ToUpperInvariant() == descrizione.ToUpperInvariant())
                    {
                        microqualifica = microqualificaDB.Id;
                        break;
                    }
                }
            }

            return microqualifica;
        }

        #endregion Private Methods

        #region internal methods

        internal void ValorizzaEtichetteDatiAssicurativi(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione, bool IsDomandaSperDonna, bool isDomandaInabilitaAmianto)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            ManageDecorrenzaForReversibilita(datiPensione, liquidazione.areaLiquidazionePensioneFS.DecorrenzaPensioneDirettaDC);

            ViewState["ListaMicroqualifica"] = liquidazione.areaLiquidazionePensioneFS.ListaMicroqualificaINPDAP.ToList();
            if (liquidazione.areaLiquidazionePensioneFS.IsUsuranti.HasValue && liquidazione.areaLiquidazionePensioneFS.IsUsuranti.Value)
                ViewState["IsUsuranti"] = "SI";

            if (IsDomandaSperDonna)
                ViewState["IsDomandaSperDonna"] = "SI";

            if (liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.HasValue)
                ViewState["IsCodNatura2Enabled"] = liquidazione.areaLiquidazionePensioneFS.IsCodiceNatura2Enabled.Value;

            if (liquidazione.areaLiquidazionePensioneFS.IsSperimentaleDonna.HasValue)
                ViewState["IsCodiceNatura2DisabledPerSperDonna"] = liquidazione.areaLiquidazionePensioneFS.IsSperimentaleDonna.Value;

            if (liquidazione.areaLiquidazionePensioneFS.ListaDecEnteRipartizioneINPDAP != null)
                ViewState[EnumViewState.DecEnteRipartizioneINPDAP.ToString()] = liquidazione.areaLiquidazionePensioneFS.ListaDecEnteRipartizioneINPDAP.ToList();

            if (liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione != null)
                ViewState["elencoCausaCessazione"] = liquidazione.areaLiquidazionePensioneFS.ListaCausaCessazione.ToList();

            RenderControlsCommon(liquidazione, datiPensione);

            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            LoadDdl(liquidazione, datiDecodifica);
            ValorizzaEtichetteDatiAssicurativiCommon(liquidazione, datiPensione);
            if (isDomandaInabilitaAmianto)
            {
                //**Revisione Campi INPDAP**
                //pnlAttivitaEconomica.Visible = true;
                //pnlProfessioneIndividuale.Visible = true;
                //txtAttivitaEconomica.Text = "01";
                //txtAttivitaEconomica.Enabled = false;
                //txtProfessioneIndividuale.Text = "250";
                //txtProfessioneIndividuale.Enabled = false;
            }

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
            if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita)
            {
                CodeUtility.BloccaForm((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], pnlTotale);
                btnEliminaDatiAssicurativi.Enabled = false;
            }

            if (datiPensione.Gestione == "019" && datiPensione.TipoFelpe != null)
            {
                btnEliminaDatiAssicurativi.Enabled = false;
            }

            if (Utility.isDomandaGiornalistiDipendentiConSistemaPrivato(datiPensione))
            {
                trDateAssicurazione.Visible = false;
                pnlCodiceSpecifico.Visible = false;
            }
        }

        internal DatiAssicurativiINPDAP GetDatiAssicurativi()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiAssicurativiINPDAP = new Presenter.SvrLiquidazioneFs.DatiAssicurativiINPDAP();

            areaLiquidazionePensioneFS.DatiAssicurativiINPDAP = GetDatiAssicurativiCommon();

            return areaLiquidazionePensioneFS.DatiAssicurativiINPDAP;
        }

        internal RipartizioneINPDAP[] GetRipartizioniINPDAP()
        {
            return ((List<RipartizioneINPDAP>)ViewState[EnumViewState.RipartizioniINPDAP.ToString()]).ToArray();
        }

        #endregion internal methods

        #region INPDAP

        private string GetMicroqualifica(long? idMicroqualifica, ILiquidazionePensione liquidazione)
        {
            if (!idMicroqualifica.HasValue)
            {
                MicroqualificaINPDAP microqualifica = Array.Find(liquidazione.areaLiquidazionePensioneFS.ListaMicroqualificaINPDAP, x => x.Id == idMicroqualifica.GetValueOrDefault());
                return microqualifica.TraduzioneSuGP + " - " + microqualifica.Descrizione;
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
                return causaCessazione != null ? causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione : string.Empty;
            }
            else return string.Empty;
        }

        private string GetCausaCessazioneByTraduzioneSuGP(string traduzioneSuGP)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            if (traduzioneSuGP != null && elencoCausaCessazione != null)
            {
                CausaCessazione causaCessazione = elencoCausaCessazione.Find(delegate (CausaCessazione code)
                { return (code.TraduzioneSuGP.Trim() == traduzioneSuGP); });
                return causaCessazione != null ? causaCessazione.TraduzioneSuGP + " - " + causaCessazione.Descrizione : string.Empty;
            }
            else return string.Empty;
        }

        private string GetIdCausaCessazioneByTraduzionesuGP(string traduzioneSuGP)
        {
            List<CausaCessazione> elencoCausaCessazione = null;
            if (ViewState["elencoCausaCessazione"] != null)
                elencoCausaCessazione = (List<CausaCessazione>)ViewState["elencoCausaCessazione"];

            if (traduzioneSuGP != null && elencoCausaCessazione != null)
            {
                CausaCessazione causaCessazione = elencoCausaCessazione.Find(delegate (CausaCessazione code)
                { return (code.TraduzioneSuGP.Trim() == traduzioneSuGP); });
                return causaCessazione != null ? causaCessazione.Id.ToString() : string.Empty;
            }
            else return string.Empty;
        }

        private byte? GetIdCodiceSpecificoByTraduzioneSuGP(ILiquidazionePensione liquidazione, char traduzioneSuGP)
        {
            CodiceSpecifico codiceSpecifico = liquidazione.areaLiquidazionePensioneFS.ListaCodiceSpecifico.ToList().Find(delegate (CodiceSpecifico code)
            {
                return (code.TipoSelezionabile.ToString() == liquidazione.areaLiquidazionePensioneFS.TipoPensione.First().Value.ToString() &&
                  code.Fondo == "DAP" && code.TraduzioneGp == traduzioneSuGP);
            });
            return codiceSpecifico != null ? codiceSpecifico.Id : null;
        }

        #endregion INPDAP

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
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            if (ShowAvvisoElimina != null)
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