using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo
{
    public partial class UCDatiCalcolo : CustomBaseUserControl, IDatiFondoAgo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            if (areaDatiFondo != null && areaDatiFondo.DatiCalcolo != null)
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                List<GestioneDatiServizioUtileINPDAPServizioUtile> listaDatiServizioUtile = areaDatiFondo.DatiCalcolo.lDatiServizioUtile != null ? areaDatiFondo.DatiCalcolo.lDatiServizioUtile.ToList() : null;

                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;

                RenderControls(areaDatiFondo);

                txtPensioneAnnuaLorda.Text = areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda.HasValue ? areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                txtAnniServUtiliDiritto.Text = areaDatiFondo.DatiCalcolo.ServizioUtileDiritto.HasValue ? areaDatiFondo.DatiCalcolo.ServizioUtileDiritto.Value.ToString() : string.Empty;

                if ((areaDatiFondo.DatiCalcolo.TipoCalcolo != GestioneContribTipoCalcolo.Contributivo) && (listaDatiServizioUtile != null && listaDatiServizioUtile.Count() > 0))
                {
                    foreach (GestioneDatiServizioUtileINPDAPServizioUtile servUtile in listaDatiServizioUtile)
                    {
                        switch (servUtile.Quota)
                        {
                            case "A":
                                txtServizioUtileAAQtaA.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaA.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaA.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                txtRetribuzioneQtaA.Text = servUtile.Retribuzione.HasValue ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                txtImpIndenIntegrSpecQtaA.Text = servUtile.ImportoIndennitaIntegrativaSpeciale.HasValue ? servUtile.ImportoIndennitaIntegrativaSpeciale.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B1":
                                txtServizioUtileAAQtaB1.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaB1.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaB1.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                txtRMSQtaB1.Text = servUtile.Retribuzione.HasValue ? servUtile.Retribuzione.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B2":
                                txtServizioUtileAAQtaB2.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaB2.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaB2.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                break;
                            case "B3":
                                txtServizioUtileAAQtaB3.Text = servUtile.ServizioUtileAA.HasValue ? servUtile.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaB3.Text = servUtile.ServizioUtileMM.HasValue ? servUtile.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaB3.Text = servUtile.ServizioUtileGG.HasValue ? servUtile.ServizioUtileGG.Value.ToString() : string.Empty;
                                break;
                            case "B4": // cessazione
                                txtServizioUtileCessazioneAA.Text = servUtile.ServizioUtileCessazioneAA.HasValue ? servUtile.ServizioUtileCessazioneAA.Value.ToString() : string.Empty;
                                txtServizioUtileCessazioneMM.Text = servUtile.ServizioUtileCessazioneMM.HasValue ? servUtile.ServizioUtileCessazioneMM.Value.ToString() : string.Empty;
                                txtServizioUtileCessazioneGG.Text = servUtile.ServizioUtileCessazioneGG.HasValue ? servUtile.ServizioUtileCessazioneGG.Value.ToString() : string.Empty;
                                break;
                        }
                    }
                }

                switch (areaDatiFondo.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.Misto:
                        txtMontante.Text = areaDatiFondo.DatiCalcolo.Montante.HasValue ? areaDatiFondo.DatiCalcolo.Montante.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        break;
                }

                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        txtRetribuzioneSenzaBenefici336.Text = areaDatiFondo.DatiCalcolo.RMSSenzaLegge33670QA.HasValue ? areaDatiFondo.DatiCalcolo.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                        break;
                }
            }
        }

        internal DatiCalcolo RecuperaCampi()
        {
            bool isContributivo = false;
            bool isRetributivo = false;

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiCalcolo = new DatiCalcolo();

            this.areaDatiFondo.DatiCalcolo.Montante = !string.IsNullOrEmpty(txtMontante.Text) ? Convert.ToDecimal(txtMontante.Text) : (decimal?)null;

            if (this.areaDatiFondo.DatiCalcolo.Montante.HasValue)
                isContributivo = true;

            this.areaDatiFondo.DatiCalcolo.PensioneAnnuaLorda = !string.IsNullOrEmpty(txtPensioneAnnuaLorda.Text) ? Convert.ToDecimal(txtPensioneAnnuaLorda.Text) : (decimal?)null;
            this.areaDatiFondo.DatiCalcolo.ServizioUtileDiritto = !string.IsNullOrEmpty(txtAnniServUtiliDiritto.Text) ? Convert.ToInt16(txtAnniServUtiliDiritto.Text) : (short?)null;
            this.areaDatiFondo.DatiCalcolo.RMSSenzaLegge33670QA = !string.IsNullOrEmpty(txtRetribuzioneSenzaBenefici336.Text) ? Convert.ToDecimal(txtRetribuzioneSenzaBenefici336.Text) : (decimal?)null;

            List<GestioneDatiServizioUtileINPDAPServizioUtile> lDatiServUtile = new List<GestioneDatiServizioUtileINPDAPServizioUtile>();
            GestioneDatiServizioUtileINPDAPServizioUtile datiServUtile = null;

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ||
               !String.IsNullOrEmpty(txtRetribuzioneQtaA.Text) || !String.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text))
            {
                datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                datiServUtile.Quota = "A";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) ? Convert.ToInt16(txtServizioUtileAAQtaA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) ? Convert.ToInt16(txtServizioUtileMMQtaA.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ? Convert.ToInt16(txtServizioUtileGGQtaA.Text) : (short?)null;
                datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRetribuzioneQtaA.Text) ? Convert.ToDecimal(txtRetribuzioneQtaA.Text) : (decimal?)null;
                datiServUtile.ImportoIndennitaIntegrativaSpeciale = !string.IsNullOrEmpty(txtImpIndenIntegrSpecQtaA.Text) ? Convert.ToDecimal(txtImpIndenIntegrSpecQtaA.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ||
                !String.IsNullOrEmpty(txtRMSQtaB1.Text))
            {
                datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                datiServUtile.Quota = "B1";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB1.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB1.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB1.Text) : (short?)null;
                datiServUtile.Retribuzione = !string.IsNullOrEmpty(txtRMSQtaB1.Text) ? Convert.ToDecimal(txtRMSQtaB1.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text))
            {
                datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                datiServUtile.Quota = "B2";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB2.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB2.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB2.Text) : (short?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text))
            {
                datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                datiServUtile.Quota = "B3";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB3.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) ? Convert.ToInt16(txtServizioUtileMMQtaB3.Text) : (short?)null;
                datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text) ? Convert.ToInt16(txtServizioUtileGGQtaB3.Text) : (short?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text))
            {
                datiServUtile = new GestioneDatiServizioUtileINPDAPServizioUtile();
                datiServUtile.Quota = "B4";
                datiServUtile.ServizioUtileCessazioneAA = !string.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) ? Convert.ToInt16(txtServizioUtileCessazioneAA.Text) : (short?)null;
                datiServUtile.ServizioUtileCessazioneMM = !string.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) ? Convert.ToInt16(txtServizioUtileCessazioneMM.Text) : (short?)null;
                datiServUtile.ServizioUtileCessazioneGG = !string.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text) ? Convert.ToInt16(txtServizioUtileCessazioneGG.Text) : (short?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
            {
                this.areaDatiFondo.DatiCalcolo.lDatiServizioUtile = lDatiServUtile.ToArray();
                isRetributivo = true;
            }

            if (isContributivo && isRetributivo)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Misto;
            else if (isContributivo)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Contributivo;
            else if (isRetributivo)
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.Retributivo;
            else
                this.areaDatiFondo.DatiCalcolo.TipoCalcolo = GestioneContribTipoCalcolo.NonValido;

            return this.areaDatiFondo.DatiCalcolo;
        }

        protected void btnSalvaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.StoreDatiCalcoloByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiCalcolo(this, null);
            }
        }

        protected void btnEliminaDatiCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondoAgo presenter = new Presenter.PresenterDatiFondoAgo();
            presenter.EliminaDatiCalcoloByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiCalcolo(this, null);
                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiCalcolo;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiCalcolo(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiCalcolo != null)
                UpdateSemaforoDatiCalcolo(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            if (HidePulsanteSalva != null)
                HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            if (TornaARegistrazioniFondo != null)
                TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers

        #region private methods
        private void RenderControls(AreaDatiFondo areaDatiFondo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (areaDatiFondo != null && areaDatiFondo.DatiCalcolo != null)
            {
                switch (areaDatiFondo.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                        pnlDatiRetributivi.Visible = true;
                        pnlDatiContributivi.Visible = false;
                        break;
                    case GestioneContribTipoCalcolo.Contributivo:
                        pnlDatiRetributivi.Visible = false;
                        pnlDatiContributivi.Visible = true;
                        break;
                    case GestioneContribTipoCalcolo.Misto:
                        pnlDatiRetributivi.Visible = true;
                        pnlDatiContributivi.Visible = true;
                        break;
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        #endregion private methods

        enum EnumViewState
        {
            IdRecordFondo
        }
    }
}