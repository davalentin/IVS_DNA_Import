using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr
{
    public partial class UCDatiCalcolo707 : CustomBaseUserControl, IDatiFondo, IDatiContributivi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        #region IDatiContributivi
        public AreaDatiContributivi areaDatiContributivi { get; set; }

        #endregion IDatiContributivi

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(DatiCalcolo707 datiCalcolo707, long? idRecordFondo, PaginaChiamante paginaChiamante)
        {
            ClearForm();

            if (datiCalcolo707 != null)
            {
                if (this.domanda == null)
                    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

                List<DatiCalcolo707.DatiServizioUtile707> listaDatiServizioUtile707 = datiCalcolo707.LDatiServizioUtile707 != null ? datiCalcolo707.LDatiServizioUtile707.ToList() : null;

                if (idRecordFondo != null)
                    ViewState[EnumViewState.IdRecordFondo.ToString()] = idRecordFondo;
                ViewState[EnumViewState.PaginaChiamante.ToString()] = paginaChiamante;

                RenderControls(datiCalcolo707);

                txtPensioneAnnuaLorda707.Text = datiCalcolo707.PensioneAnnuaLorda707.HasValue ? datiCalcolo707.PensioneAnnuaLorda707.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;

                if (listaDatiServizioUtile707 != null && listaDatiServizioUtile707.Count() > 0)
                {
                    foreach (DatiCalcolo707.DatiServizioUtile707 servUtile707 in listaDatiServizioUtile707)
                    {
                        switch (servUtile707.Quota)
                        {
                            case "A":
                                txtServizioUtileAAQtaA.Text = servUtile707.ServizioUtileAA.HasValue ? servUtile707.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaA.Text = servUtile707.ServizioUtileMM.HasValue ? servUtile707.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaA.Text = servUtile707.ServizioUtileGG.HasValue ? servUtile707.ServizioUtileGG.Value.ToString() : string.Empty;
                                txtPensioneRetribAnnua707QtaA.Text = servUtile707.QuotaPensioneRetributivaAnnua.HasValue ? servUtile707.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B1":
                                txtServizioUtileAAQtaB1.Text = servUtile707.ServizioUtileAA.HasValue ? servUtile707.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaB1.Text = servUtile707.ServizioUtileMM.HasValue ? servUtile707.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaB1.Text = servUtile707.ServizioUtileGG.HasValue ? servUtile707.ServizioUtileGG.Value.ToString() : string.Empty;
                                txtPensioneRetribAnnua707QtaB1.Text = servUtile707.QuotaPensioneRetributivaAnnua.HasValue ? servUtile707.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B2":
                                txtServizioUtileAAQtaB2.Text = servUtile707.ServizioUtileAA.HasValue ? servUtile707.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaB2.Text = servUtile707.ServizioUtileMM.HasValue ? servUtile707.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaB2.Text = servUtile707.ServizioUtileGG.HasValue ? servUtile707.ServizioUtileGG.Value.ToString() : string.Empty;
                                txtPensioneRetribAnnua707QtaB2.Text = servUtile707.QuotaPensioneRetributivaAnnua.HasValue ? servUtile707.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B3":
                                txtServizioUtileAAQtaB3.Text = servUtile707.ServizioUtileAA.HasValue ? servUtile707.ServizioUtileAA.Value.ToString() : string.Empty;
                                txtServizioUtileMMQtaB3.Text = servUtile707.ServizioUtileMM.HasValue ? servUtile707.ServizioUtileMM.Value.ToString() : string.Empty;
                                txtServizioUtileGGQtaB3.Text = servUtile707.ServizioUtileGG.HasValue ? servUtile707.ServizioUtileGG.Value.ToString() : string.Empty;
                                txtPensioneRetribAnnua707QtaB3.Text = servUtile707.QuotaPensioneRetributivaAnnua.HasValue ? servUtile707.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B4": // cessazione
                                txtServizioUtileCessazioneAA.Text = servUtile707.ServizioUtileCessazioneAA.HasValue ? servUtile707.ServizioUtileCessazioneAA.Value.ToString() : string.Empty;
                                txtServizioUtileCessazioneMM.Text = servUtile707.ServizioUtileCessazioneMM.HasValue ? servUtile707.ServizioUtileCessazioneMM.Value.ToString() : string.Empty;
                                txtServizioUtileCessazioneGG.Text = servUtile707.ServizioUtileCessazioneGG.HasValue ? servUtile707.ServizioUtileCessazioneGG.Value.ToString() : string.Empty;
                                txtPensioneRetribAnnua707QtaB4.Text = servUtile707.QuotaPensioneRetributivaAnnua.HasValue ? servUtile707.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                break;
                            case "B5":
                                if (this.domanda.IsDomandaINPDAP)
                                {
                                    txtServizioUtileAAQtaB5.Text = servUtile707.ServizioUtileAA.HasValue ? servUtile707.ServizioUtileAA.Value.ToString() : string.Empty;
                                    txtServizioUtileMMQtaB5.Text = servUtile707.ServizioUtileMM.HasValue ? servUtile707.ServizioUtileMM.Value.ToString() : string.Empty;
                                    txtServizioUtileGGQtaB5.Text = servUtile707.ServizioUtileGG.HasValue ? servUtile707.ServizioUtileGG.Value.ToString() : string.Empty;
                                    txtPensioneRetribAnnua707QtaB5.Text = servUtile707.QuotaPensioneRetributivaAnnua.HasValue ? servUtile707.QuotaPensioneRetributivaAnnua.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                                }
                                break;
                        }
                    }
                }

                CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
                CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
                CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
                CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
                if (tipologiaProdottoPensione == CodeUtility.TipologiaPensioneProdotto.pr_Reversibilita && this.domanda.IsDomandaINPDAP)
                {
                    CodeUtility.BloccaForm((Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"], pnlDatiCalcolo);
                    btnEliminaDatiCalcolo707.Enabled = false;
                }

                //ENG - PL Reversibilità 024
                if (Utility.IsDomandaReversibilita(datiPensione) && !this.domanda.IsDomandaRiapertura &&
                    (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                {
                    if (datiCalcolo707 != null)
                    {
                        ViewState["IsPensioneAnnuaLorda707DaPrelievo"] = datiCalcolo707.IsPensioneAnnuaLorda707DaPrelievo;
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                        {
                            if (datiCalcolo707.IsPensioneAnnuaLorda707DaPrelievo.GetValueOrDefault())
                                txtPensioneAnnuaLorda707.Enabled = false;
                            else
                                txtPensioneAnnuaLorda707.Enabled = true;
                        }
                    }
                }
            }
        }

        internal DatiCalcolo707 RecuperaCampi()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            DatiCalcolo707 datiCalcolo707 = new DatiCalcolo707();

            datiCalcolo707.PensioneAnnuaLorda707 = !string.IsNullOrEmpty(txtPensioneAnnuaLorda707.Text) ? Convert.ToDecimal(txtPensioneAnnuaLorda707.Text) : (decimal?)null;

            List<DatiCalcolo707.DatiServizioUtile707> lDatiServUtile = new List<DatiCalcolo707.DatiServizioUtile707>();
            DatiCalcolo707.DatiServizioUtile707 datiServUtile = null;

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaA.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaA.Text) ||
               !String.IsNullOrEmpty(txtPensioneRetribAnnua707QtaA.Text))
            {
                datiServUtile = new DatiCalcolo707.DatiServizioUtile707();
                datiServUtile.Quota = "A";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaA.Text) ? Convert.ToInt16(txtServizioUtileAAQtaA.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = CodeUtility.StringToNullableByte(txtServizioUtileMMQtaA.Text);
                datiServUtile.ServizioUtileGG = CodeUtility.StringToNullableByte(txtServizioUtileGGQtaA.Text);
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtPensioneRetribAnnua707QtaA.Text) ? Convert.ToDecimal(txtPensioneRetribAnnua707QtaA.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB1.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB1.Text) ||
                !String.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB1.Text))
            {
                datiServUtile = new DatiCalcolo707.DatiServizioUtile707();
                datiServUtile.Quota = "B1";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB1.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB1.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB1.Text);
                datiServUtile.ServizioUtileGG = CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB1.Text);
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB1.Text) ? Convert.ToDecimal(txtPensioneRetribAnnua707QtaB1.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB2.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB2.Text) ||
                !String.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB2.Text))
            {
                datiServUtile = new DatiCalcolo707.DatiServizioUtile707();
                datiServUtile.Quota = "B2";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB2.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB2.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB2.Text);
                datiServUtile.ServizioUtileGG = CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB2.Text);
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB2.Text) ? Convert.ToDecimal(txtPensioneRetribAnnua707QtaB2.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB3.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB3.Text) ||
                !String.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB3.Text))
            {
                datiServUtile = new DatiCalcolo707.DatiServizioUtile707();
                datiServUtile.Quota = "B3";
                datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB3.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB3.Text) : (short?)null;
                datiServUtile.ServizioUtileMM = CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB3.Text);
                datiServUtile.ServizioUtileGG = CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB3.Text);
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB3.Text) ? Convert.ToDecimal(txtPensioneRetribAnnua707QtaB3.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (!String.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneMM.Text) || !String.IsNullOrEmpty(txtServizioUtileCessazioneGG.Text) ||
                !String.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB4.Text))
            {
                datiServUtile = new DatiCalcolo707.DatiServizioUtile707();
                datiServUtile.Quota = "B4";
                datiServUtile.ServizioUtileCessazioneAA = !string.IsNullOrEmpty(txtServizioUtileCessazioneAA.Text) ? Convert.ToInt16(txtServizioUtileCessazioneAA.Text) : (short?)null;
                datiServUtile.ServizioUtileCessazioneMM = CodeUtility.StringToNullableByte(txtServizioUtileCessazioneMM.Text);
                datiServUtile.ServizioUtileCessazioneGG = CodeUtility.StringToNullableByte(txtServizioUtileCessazioneGG.Text);
                datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB4.Text) ? Convert.ToDecimal(txtPensioneRetribAnnua707QtaB4.Text) : (decimal?)null;
                lDatiServUtile.Add(datiServUtile);
            }

            if (this.domanda.IsDomandaINPDAP)
            {
                if (!String.IsNullOrEmpty(txtServizioUtileAAQtaB5.Text) || !String.IsNullOrEmpty(txtServizioUtileMMQtaB5.Text) || !String.IsNullOrEmpty(txtServizioUtileGGQtaB5.Text))
                {
                    datiServUtile = new DatiCalcolo707.DatiServizioUtile707();
                    datiServUtile.Quota = "B5";
                    datiServUtile.ServizioUtileAA = !string.IsNullOrEmpty(txtServizioUtileAAQtaB5.Text) ? Convert.ToInt16(txtServizioUtileAAQtaB5.Text) : (short?)null;
                    datiServUtile.ServizioUtileMM = !string.IsNullOrEmpty(txtServizioUtileMMQtaB5.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileMMQtaB5.Text) : (byte?)null;
                    datiServUtile.ServizioUtileGG = !string.IsNullOrEmpty(txtServizioUtileGGQtaB5.Text) ? CodeUtility.StringToNullableByte(txtServizioUtileGGQtaB5.Text) : (byte?)null;
                    datiServUtile.QuotaPensioneRetributivaAnnua = !string.IsNullOrEmpty(txtPensioneRetribAnnua707QtaB5.Text) ? Convert.ToDecimal(txtPensioneRetribAnnua707QtaB5.Text) : (decimal?)null;
                    lDatiServUtile.Add(datiServUtile);
                }
            }

            if (lDatiServUtile != null && lDatiServUtile.Count() > 0)
            {
                datiCalcolo707.LDatiServizioUtile707 = lDatiServUtile.ToArray();
            }

            //ENG - PL Reversibilita 024 
            if (Utility.IsDomandaReversibilita(datiPensione) && !this.domanda.IsDomandaRiapertura && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
            {
                if (ViewState["IsPensioneAnnuaLorda707DaPrelievo"] != null)
                {
                    datiCalcolo707.IsPensioneAnnuaLorda707DaPrelievo = (bool)ViewState["IsPensioneAnnuaLorda707DaPrelievo"];
                }
            }

            return datiCalcolo707;
        }

        protected void btnSalvaDatiCalcolo707_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PaginaChiamante paginaChiamante = (PaginaChiamante)ViewState[EnumViewState.PaginaChiamante.ToString()];

            switch (paginaChiamante)
            {
                case PaginaChiamante.DatiContributivi:
                    if (this.areaDatiContributivi == null)
                        this.areaDatiContributivi = new AreaDatiContributivi();
                    this.areaDatiContributivi.DatiCalcolo707 = RecuperaCampi();
                    PresenterDatiContributivi presenterContrib = new PresenterDatiContributivi();
                    presenterContrib.SalvaTabDatiCalcolo707(this);
                    break;
                case PaginaChiamante.DatiFondo:
                    if (this.areaDatiFondo == null)
                        this.areaDatiFondo = new AreaDatiFondo();
                    this.areaDatiFondo.DatiCalcolo707 = RecuperaCampi();
                    this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];
                    PresenterDatiFondo presenterFondo = new PresenterDatiFondo();
                    presenterFondo.StoreDatiCalcolo707ByIdRecordFondo(this);
                    break;
            }

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo 707 salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiCalcolo707(this, null);
            }
        }

        protected void btnEliminaDatiCalcolo707_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PaginaChiamante paginaChiamante = (PaginaChiamante)ViewState[EnumViewState.PaginaChiamante.ToString()];

            switch (paginaChiamante)
            {
                case PaginaChiamante.DatiContributivi:
                    if (this.areaDatiContributivi == null)
                        this.areaDatiContributivi = new AreaDatiContributivi();
                    Presenter.PresenterDatiContributivi presenterContrib = new Presenter.PresenterDatiContributivi();
                    presenterContrib.EliminaTabDatiCalcolo707(this);
                    break;
                case PaginaChiamante.DatiFondo:
                    if (this.areaDatiFondo == null)
                        this.areaDatiFondo = new AreaDatiFondo();
                    this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];
                    Presenter.PresenterDatiFondo presenterFondo = new Presenter.PresenterDatiFondo();
                    presenterFondo.EliminaDatiCalcolo707ByIdRecordFondo(this);
                    break;
            }

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Calcolo 707 eliminati correttamente.";

                switch (paginaChiamante)
                {
                    case PaginaChiamante.DatiContributivi:
                        RaiseShowAvvisoElimina(this, null);
                        ValorizzaEtichette(this.areaDatiContributivi.DatiCalcolo707, null, paginaChiamante);
                        break;
                    case PaginaChiamante.DatiFondo:
                        RaiseShowAvviso(this, null);
                        RaiseUpdateSemaforoDatiCalcolo707(this, null);
                        ValorizzaEtichette(this.areaDatiFondo.DatiCalcolo707, this.areaDatiFondo.IdRecordFondo, paginaChiamante);
                        break;
                }
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
        public event EventHandler UpdateSemaforoDatiCalcolo707;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiCalcolo707(object sender, EventArgs e)
        {
            if (UpdateSemaforoDatiCalcolo707 != null)
                UpdateSemaforoDatiCalcolo707(sender, e);
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

        private void RenderControls(DatiCalcolo707 datiCalcolo707)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda.IsDomandaINPDAP)
            {
                if (!Utility.IsDomandaCPDEL(this.domanda.Categoria))
                    pnlDatiPost97.Visible = true;

                if (Utility.IsRicostituzione_ProvenienteDaListePensioniDaVerificare(datiPensione) || Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) || Utility.IsRicostituzioneVariazioneDatiContitolari(datiPensione))
                    pnlDatiCalcolo.Enabled = false;
            }

            if (datiCalcolo707 != null)
            {
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ||
                    (this.domanda.IsDomandaINPDAP && (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))))
                {
                    txtPensioneAnnuaLorda707.Enabled = false;
                    txtPensioneRetribAnnua707QtaA.Enabled = false;
                    txtPensioneRetribAnnua707QtaB1.Enabled = false;
                    txtPensioneRetribAnnua707QtaB2.Enabled = false;
                    txtPensioneRetribAnnua707QtaB3.Enabled = false;
                    txtPensioneRetribAnnua707QtaB4.Enabled = false;
                    txtPensioneRetribAnnua707QtaB5.Enabled = false; //INPDAP
                    txtServizioUtileCessazioneAA.Enabled = false;
                    txtServizioUtileCessazioneGG.Enabled = false;
                    txtServizioUtileCessazioneMM.Enabled = false;
                    txtServizioUtileAAQtaA.Enabled = false;
                    txtServizioUtileAAQtaB1.Enabled = false;
                    txtServizioUtileAAQtaB2.Enabled = false;
                    txtServizioUtileAAQtaB3.Enabled = false;
                    txtServizioUtileAAQtaB5.Enabled = false; //INPDAP
                    txtServizioUtileMMQtaA.Enabled = false;
                    txtServizioUtileMMQtaB1.Enabled = false;
                    txtServizioUtileMMQtaB2.Enabled = false;
                    txtServizioUtileMMQtaB3.Enabled = false;
                    txtServizioUtileMMQtaB5.Enabled = false; //INPDAP
                    txtServizioUtileGGQtaA.Enabled = false;
                    txtServizioUtileGGQtaB1.Enabled = false;
                    txtServizioUtileGGQtaB2.Enabled = false;
                    txtServizioUtileGGQtaB3.Enabled = false;
                    txtServizioUtileGGQtaB5.Enabled = false; //INPDAP
                    //INPDAP
                    pnlQuotaPensioneRetributivaAnnua707B98.Visible = true;
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        #endregion private methods

        public enum EnumViewState
        {
            IdRecordFondo,
            PaginaChiamante
        }

        public enum PaginaChiamante
        {
            DatiContributivi,
            DatiFondo
        }
    }
}