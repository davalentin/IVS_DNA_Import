using System;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi
{
    public partial class UCDatiAgoGAS_ES : CustomBaseUserControl, IDatiContributivi, ITitolarePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                if (this.domanda.Tipofondo == Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                {
                    //Quota L214
                    txtMontanteQuotaDL214.MaxLength = 10;
                    REVtxtMontanteQuotaDL214_ES.Enabled = true;
                    REVtxtMontanteQuotaDL214.Enabled = false;
                    txtNSettimaneQuotaDL214.MaxLength = 3;
                }
            }
        }

        protected void btnSalvaDatiAgo_Click(object sender, EventArgs e)
        {
            RecuperaCampi();
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.SalvaTabDatiAgoGAS(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiAgo_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.EliminaTabDatiAgoGAS(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Ago";
            else
            {
                ClearForm();
                RaiseCaricaDatiCalcolo(this.areaDatiContributivi, null);

            }

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        public void ValorizzaEtichette(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            ViewState[EnumViewState.IsRiduzioneRetribVisible.ToString()] = this.areaDatiContributivi.IsRiduzioneRetribVisible;

            RenderControlsFromTipoCalcoloAndTipoFondo();
            LoadDdl();

            if (this.areaDatiContributivi.TipoPensione != null)
            {
                lblTipoPensione.Text = this.areaDatiContributivi.TipoPensione.First().Key;
                hdnTipoPensione.Value = this.areaDatiContributivi.TipoPensione.First().Value.ToString();
            }


            //Riduzione retributiva

            if (this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva)
                ddlRiduzioneRetributiva.SelectedValue = "SI";
            else ddlRiduzioneRetributiva.SelectedValue = "NO";
            if (this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.HasValue)
                txtRiduzioneRetributiva.Text = this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale.ToString();

            if (datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue)
                txtDecorrenzaDatiAgo.Text = String.Format("{0:MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:

                    //DATI GENERICI
                    if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcolo != null)
                    {
                        ViewState["TipoCalcolo"] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                        //Dati AGO

                        //Dati Retributivi
                        if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue)
                            txtRMSQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.Value.ToString("0.0000");

                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue)
                            txtNSettimaneQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaA.HasValue)
                            txtNSettimaneEsclusiveQuotaA.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaA.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue)
                            txtRMSQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.Value.ToString("0.0000");

                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue)
                            txtNSettimaneQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaB.HasValue)
                            txtNSettimaneEsclusiveQuotaB.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaB.ToString();

                        //Dati contributivi
                        if (this.areaDatiContributivi.DatiCalcolo.Montante.HasValue)
                            txtMontante.Text = this.areaDatiContributivi.DatiCalcolo.Montante.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivo.HasValue)
                            txtMontanteEsclusivo.Text = this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivo.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.NSettimane.HasValue)
                            txtNSettimane.Text = this.areaDatiContributivi.DatiCalcolo.NSettimane.ToString();

                        //Dati contrib L214
                        if (this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue)
                            txtMontanteQuotaDL214.Text = this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                            txtNSettimaneQuotaDL214.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivoQuotaDL214.HasValue)
                            txtMontanteEsclusivoQuotaDL214.Text = this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivoQuotaDL214.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);


                    }
                    //DATI SPECIFICI DEL FONDO
                    if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoGAS != null)
                    {
                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.CodiceTipoLiquidazione.HasValue)
                            ddlTipoLiquidazioneGAS.SelectedValue = this.areaDatiContributivi.DatiCalcolo.fondoGAS.CodiceTipoLiquidazione.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.SospensioneAGO.HasValue)
                            txtSospensioneAGO.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiCalcolo.fondoGAS.SospensioneAGO);

                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.SettimaneAnzianitaEsclusiva.HasValue)
                            txtSettimaneAnzianitaEsclusiva.Text = this.areaDatiContributivi.DatiCalcolo.fondoGAS.SettimaneAnzianitaEsclusiva.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.AnniDifferimento.HasValue)
                            txtAnniDifferimento.Text = this.areaDatiContributivi.DatiCalcolo.fondoGAS.AnniDifferimento.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.EtaMaturazioneRequisiti.HasValue)
                            txtEtaMaturazioneRequisiti.Text = this.areaDatiContributivi.DatiCalcolo.fondoGAS.EtaMaturazioneRequisiti.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.CodiceSpecificoAgo.HasValue)
                            txtCodiceSpecifico.Text = this.areaDatiContributivi.DatiCalcolo.fondoGAS.CodiceSpecificoAgo.ToString();

                        if (this.areaDatiContributivi.DatiCalcolo.fondoGAS.DecorrenzaTeorica.HasValue)
                            txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiCalcolo.fondoGAS.DecorrenzaTeorica.Value);
                    }

                    if (string.IsNullOrEmpty(txtDecorrenzaTeorica.Text) && datiPensione != null && datiPensione.DecorrenzaOriginaria.HasValue)
                        txtDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", datiPensione.DecorrenzaOriginaria.Value);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    //DATI GENERICI
                    if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcolo != null)
                    {
                        ViewState["TipoCalcolo"] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;

                        //DATI AGO
                        if (this.areaDatiContributivi.DatiCalcolo.NSettAnzianitaVV.HasValue)
                            txtboxVersamentiVolontari.Text = this.areaDatiContributivi.DatiCalcolo.NSettAnzianitaVV.Value.ToString();
                        //DATI RETRIBUTIVI
                        if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.HasValue)
                            txtboxQuotaA_RMS.Text = this.areaDatiContributivi.DatiCalcolo.RMSQuotaA.Value.ToString("0.0000");
                        if (this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.HasValue)
                            txtboxQuotaB_RMS.Text = this.areaDatiContributivi.DatiCalcolo.RMSQuotaB.Value.ToString("0.0000");
                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.HasValue)
                            txtboxQuotaA_SettAnzTot.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA.Value.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.HasValue)
                            txtboxQuotaB_SettAnzTot.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB.Value.ToString();
                        //DATI CONTRIBUTIVI
                        if (this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.HasValue)
                            txtboxContributiTotali.Text = this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.Montante.HasValue)
                            txtMontante.Text = this.areaDatiContributivi.DatiCalcolo.Montante.Value.ToString("0.0000");
                        if (this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivo.HasValue)
                            txtMontanteEsclusivo.Text = this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivo.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.NSettimane.HasValue)
                            txtNSettimane.Text = this.areaDatiContributivi.DatiCalcolo.NSettimane.ToString();
                        //Dati Contritivi L214
                        if (this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.HasValue)
                            txtMontanteQuotaDL214.Text = this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214.Value.ToString("#######.##");
                        if (this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                            txtNSettimaneQuotaDL214.Text = this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue)
                            txtboxL214_ImportoContributivo.Text = (this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214).ToString();
                    }
                    //DATI SPECIFICI DEL FONDO
                    if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoES != null)
                    {
                        //DatiAgo
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.CodiceTipoLiquidazione.HasValue)
                            txtTipoLiquidazioneES.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.CodiceTipoLiquidazione.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.Decorrenza.HasValue)
                            txtDecorrenzaDatiAgo.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiCalcolo.fondoES.Decorrenza);
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.DecorrenzaTeorica.HasValue)
                            txtboxDecorrenzaTeorica.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiCalcolo.fondoES.DecorrenzaTeorica);
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.Sospensione.HasValue)
                            txtSospensioneAGO.Text = String.Format("{0:MM/yyyy}", this.areaDatiContributivi.DatiCalcolo.fondoES.Sospensione);
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.ContributiDifferimentoQuota.HasValue)
                            txtboxContributiDiffQuota.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.ContributiDifferimentoQuota.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.AnniDifferimento.HasValue)
                            txtAnniDifferimento.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.AnniDifferimento.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.EtaMaturazioneRequisiti.HasValue)
                            txtEtaMaturazioneRequisiti.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.EtaMaturazioneRequisiti.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.CodiceSpecificoAgo.HasValue)
                            txtCodiceSpecifico.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.CodiceSpecificoAgo.ToString();
                        //Dati Retributivi
                        //quota A
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.SettimaneArt24QA.HasValue)
                            txtboxSettArt24.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.SettimaneArt24QA.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57.HasValue)
                            txtboxSettArt57.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11.HasValue)
                            txtboxIntegrArt11.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11.ToString();
                        //quaota B
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.SettimaneArt24QB.HasValue)
                            txtboxQuotaB_SettArt24.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.SettimaneArt24QB.ToString();
                        //if (this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57.HasValue)
                        //    txtboxSettArt57.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57.ToString();
                        //if (this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11.HasValue)
                        //    txtboxIntegrArt11.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11.ToString();
                        //DATI CONTRIBUTIVI
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge37758Art24.HasValue)
                            txtboxContributiArt24.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge37758Art24.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge37758Art57.HasValue)
                            txtboxContributiArt57.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge37758Art57.ToString();
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge143271Art14.HasValue)
                            txtboxSupplementoArt14.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge143271Art14.ToString();
                        //altra pensione
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.BaseAltraPensione.HasValue)
                        {
                            txtAltraPensione.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.BaseAltraPensione.ToString();
                        }
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES.CategoriaAltraPensione.HasValue)
                        {
                            ddlCategoriaPensione.SelectedValue = this.areaDatiContributivi.DatiCalcolo.fondoES.CategoriaAltraPensione.Value.ToString().PadLeft(4, '0');

                        }
                    }
                    break;


            }

            GestioneEtichetteIsUnicarpe(datiPensione);

            ManageButtons();

            ucDoppioCalcolo.ValorizzaEtichetteComma707(this.areaDatiContributivi);
        }

        public void RecuperaCampi()
        {
            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.GestioneContribDatiCalcolo();

            this.areaDatiContributivi.DatiCalcolo.TipoCalcolo = (GestioneContribTipoCalcolo)ViewState["TipoCalcolo"];

            if (ViewState[EnumViewState.IsRiduzioneRetribVisible.ToString()] != null && ((bool?)ViewState[EnumViewState.IsRiduzioneRetribVisible.ToString()]).GetValueOrDefault())
            {
                if (ddlRiduzioneRetributiva.SelectedValue.Equals("SI"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = true;
                else if (ddlRiduzioneRetributiva.SelectedValue.Equals("NO"))
                    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = false;
            }

            if (!string.IsNullOrEmpty(txtRiduzioneRetributiva.Text))
                this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale = decimal.Parse(txtRiduzioneRetributiva.Text);

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:

                    if (this.areaDatiContributivi.DatiCalcolo.fondoGAS == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS = new GestioneContribFondoGAS();

                    if (!string.IsNullOrEmpty(ddlTipoLiquidazioneGAS.SelectedValue))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.CodiceTipoLiquidazione = byte.Parse(ddlTipoLiquidazioneGAS.SelectedValue);

                    if (!string.IsNullOrEmpty(txtDecorrenzaDatiAgo.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.DecorrenzaDatiAgo = DateTime.Parse(txtDecorrenzaDatiAgo.Text);

                    if (!string.IsNullOrEmpty(txtSospensioneAGO.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.SospensioneAGO = DateTime.Parse(txtSospensioneAGO.Text);

                    if (!string.IsNullOrEmpty(txtSettimaneAnzianitaEsclusiva.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.SettimaneAnzianitaEsclusiva = short.Parse(txtSettimaneAnzianitaEsclusiva.Text);

                    if (!string.IsNullOrEmpty(txtAnniDifferimento.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.AnniDifferimento = int.Parse(txtAnniDifferimento.Text);

                    if (!string.IsNullOrEmpty(txtEtaMaturazioneRequisiti.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.EtaMaturazioneRequisiti = byte.Parse(txtEtaMaturazioneRequisiti.Text);

                    if (!string.IsNullOrEmpty(txtCodiceSpecifico.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.CodiceSpecificoAgo = char.Parse(txtCodiceSpecifico.Text.ToUpperInvariant());

                    if (!string.IsNullOrEmpty(txtDecorrenzaTeorica.Text))
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS.DecorrenzaTeorica = DateTime.Parse(txtDecorrenzaTeorica.Text);


                    if (!string.IsNullOrEmpty(txtRMSQuotaA.Text))
                        this.areaDatiContributivi.DatiCalcolo.RMSQuotaA = decimal.Parse(txtRMSQuotaA.Text);

                    if (!string.IsNullOrEmpty(txtNSettimaneQuotaA.Text))
                        this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA = int.Parse(txtNSettimaneQuotaA.Text);

                    if (!string.IsNullOrEmpty(txtNSettimaneEsclusiveQuotaA.Text))
                        this.areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaA = int.Parse(txtNSettimaneEsclusiveQuotaA.Text);

                    if (!string.IsNullOrEmpty(txtRMSQuotaB.Text))
                        this.areaDatiContributivi.DatiCalcolo.RMSQuotaB = decimal.Parse(txtRMSQuotaB.Text);

                    if (!string.IsNullOrEmpty(txtNSettimaneQuotaB.Text))
                        this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB = int.Parse(txtNSettimaneQuotaB.Text);

                    if (!string.IsNullOrEmpty(txtNSettimaneEsclusiveQuotaB.Text))
                        this.areaDatiContributivi.DatiCalcolo.NSettimaneEsclusiveQuotaB = int.Parse(txtNSettimaneEsclusiveQuotaB.Text);


                    if (!string.IsNullOrEmpty(txtMontante.Text))
                        this.areaDatiContributivi.DatiCalcolo.Montante = decimal.Parse(txtMontante.Text);

                    if (!string.IsNullOrEmpty(txtMontanteEsclusivo.Text))
                        this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivo = decimal.Parse(txtMontanteEsclusivo.Text);

                    if (!string.IsNullOrEmpty(txtNSettimane.Text))
                        this.areaDatiContributivi.DatiCalcolo.NSettimane = int.Parse(txtNSettimane.Text);

                    if (!string.IsNullOrEmpty(txtMontanteQuotaDL214.Text))
                        this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = decimal.Parse(txtMontanteQuotaDL214.Text);

                    if (!string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text))
                        this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = int.Parse(txtNSettimaneQuotaDL214.Text);

                    if (!string.IsNullOrEmpty(txtMontanteEsclusivoQuotaDL214.Text))
                        this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivoQuotaDL214 = decimal.Parse(txtMontanteEsclusivoQuotaDL214.Text);

                    break;


                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    {
                        //  ViewState["TipoCalcolo"] = this.areaDatiContributivi.DatiCalcolo.TipoCalcolo;
                        if (this.areaDatiContributivi.DatiCalcolo.fondoES == null)
                            this.areaDatiContributivi.DatiCalcolo.fondoES = new GestioneContribFondoES_AGO();

                        //DATI AGO
                        if (!string.IsNullOrEmpty(txtboxVersamentiVolontari.Text))
                            this.areaDatiContributivi.DatiCalcolo.NSettAnzianitaVV = int.Parse(txtboxVersamentiVolontari.Text);

                        //DATI RETRIBUTIVI
                        if (!string.IsNullOrEmpty(txtboxQuotaA_RMS.Text))
                            this.areaDatiContributivi.DatiCalcolo.RMSQuotaA = decimal.Parse(txtboxQuotaA_RMS.Text);
                        if (!string.IsNullOrEmpty(txtboxQuotaB_RMS.Text))
                            this.areaDatiContributivi.DatiCalcolo.RMSQuotaB = decimal.Parse(txtboxQuotaB_RMS.Text);
                        if (!string.IsNullOrEmpty(txtboxQuotaA_SettAnzTot.Text))
                            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaA = int.Parse(txtboxQuotaA_SettAnzTot.Text);
                        if (!string.IsNullOrEmpty(txtboxQuotaB_SettAnzTot.Text))
                            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaB = int.Parse(txtboxQuotaB_SettAnzTot.Text);
                        ////riduzione retributiva
                        //if (ddlRiduzioneRetributivaES.SelectedValue.Equals("SI"))
                        //    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = true;
                        //else if (ddlRiduzioneRetributivaES.SelectedValue.Equals("NO"))
                        //    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributiva = false;
                        //if (!string.IsNullOrEmpty(txtPercentualeRiduzioneES.Text))
                        //    this.areaDatiContributivi.DatiCalcolo.RiduzioneRetributivaPercentuale = decimal.Parse(txtPercentualeRiduzioneES.Text);
                        //DATI CONTRIBUTIVI
                        if (!string.IsNullOrEmpty(txtboxContributiTotali.Text))
                            this.areaDatiContributivi.DatiCalcolo.ImportoContributivoTotale = decimal.Parse(txtboxContributiTotali.Text);
                        if (!string.IsNullOrEmpty(txtMontante.Text))
                            this.areaDatiContributivi.DatiCalcolo.Montante = decimal.Parse(txtMontante.Text);
                        if (!string.IsNullOrEmpty(txtMontanteEsclusivo.Text))
                            this.areaDatiContributivi.DatiCalcolo.MontanteEsclusivo = decimal.Parse(txtMontanteEsclusivo.Text);
                        if (!string.IsNullOrEmpty(txtNSettimane.Text))
                            this.areaDatiContributivi.DatiCalcolo.NSettimane = int.Parse(txtNSettimane.Text);
                        //Dati Contritivi L214
                        if (!string.IsNullOrEmpty(txtMontanteQuotaDL214.Text))
                            this.areaDatiContributivi.DatiCalcolo.MontanteQuotaDL214 = decimal.Parse(txtMontanteQuotaDL214.Text);

                        if (!string.IsNullOrEmpty(txtNSettimaneQuotaDL214.Text))
                            this.areaDatiContributivi.DatiCalcolo.NSettimaneQuotaDL214 = int.Parse(txtNSettimaneQuotaDL214.Text);

                        if (!string.IsNullOrEmpty(txtboxL214_ImportoContributivo.Text))
                            this.areaDatiContributivi.DatiCalcolo.ImportoContribTotaleQuotaDL214 = decimal.Parse(txtboxL214_ImportoContributivo.Text);
                    }

                    //DATI SPECIFICI DEL FONDO
                    if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcolo != null && this.areaDatiContributivi.DatiCalcolo.fondoES != null)
                    {
                        //DatiAgo
                        if (!string.IsNullOrEmpty(txtTipoLiquidazioneES.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.CodiceTipoLiquidazione = byte.Parse(txtTipoLiquidazioneES.Text);

                        if (!string.IsNullOrEmpty(txtDecorrenzaDatiAgo.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.Decorrenza = DateTime.Parse(txtDecorrenzaDatiAgo.Text);

                        if (!string.IsNullOrEmpty(txtboxDecorrenzaTeorica.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.DecorrenzaTeorica = DateTime.Parse(txtboxDecorrenzaTeorica.Text);

                        if (!string.IsNullOrEmpty(txtSospensioneAGO.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.Sospensione = DateTime.Parse(txtSospensioneAGO.Text);

                        if (!string.IsNullOrEmpty(txtboxContributiDiffQuota.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.ContributiDifferimentoQuota = decimal.Parse(txtboxContributiDiffQuota.Text);

                        if (!string.IsNullOrEmpty(txtAnniDifferimento.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.AnniDifferimento = int.Parse(txtAnniDifferimento.Text);

                        if (!string.IsNullOrEmpty(txtEtaMaturazioneRequisiti.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.EtaMaturazioneRequisiti = byte.Parse(txtEtaMaturazioneRequisiti.Text);

                        if (!string.IsNullOrEmpty(txtCodiceSpecifico.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.CodiceSpecificoAgo = char.Parse(txtCodiceSpecifico.Text);
                        //Dati Retributivi
                        //quota A
                        if (!string.IsNullOrEmpty(txtboxSettArt24.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.SettimaneArt24QA = int.Parse(txtboxSettArt24.Text);

                        if (!string.IsNullOrEmpty(txtboxSettArt57.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57 = int.Parse(txtboxSettArt57.Text);

                        if (!string.IsNullOrEmpty(txtboxIntegrArt11.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11 = decimal.Parse(txtboxIntegrArt11.Text);
                        //quaota B
                        if (!string.IsNullOrEmpty(txtboxQuotaB_SettArt24.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.SettimaneArt24QB = int.Parse(txtboxQuotaB_SettArt24.Text);
                        //if (this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57.HasValue)
                        //    txtboxSettArt57.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.NSettimaneLegge37758Art57.ToString();
                        //if (this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11.HasValue)
                        //    txtboxIntegrArt11.Text = this.areaDatiContributivi.DatiCalcolo.fondoES.IntegrazioneArticolo11.ToString();
                        //DATI CONTRIBUTIVI
                        if (!string.IsNullOrEmpty(txtboxContributiArt24.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge37758Art24 = decimal.Parse(txtboxContributiArt24.Text);

                        if (!string.IsNullOrEmpty(txtboxContributiArt57.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge37758Art57 = decimal.Parse(txtboxContributiArt57.Text);

                        if (!string.IsNullOrEmpty(txtboxSupplementoArt14.Text))
                            this.areaDatiContributivi.DatiCalcolo.fondoES.ImportoContributiLegge143271Art14 = decimal.Parse(txtboxSupplementoArt14.Text);
                        //altra pensione
                        if (!string.IsNullOrEmpty(ddlCategoriaPensione.SelectedValue))
                        {
                            this.areaDatiContributivi.DatiCalcolo.fondoES.CategoriaAltraPensione = short.Parse(ddlCategoriaPensione.SelectedValue);
                        }
                        if (!string.IsNullOrEmpty(txtAltraPensione.Text))
                        {
                            this.areaDatiContributivi.DatiCalcolo.fondoES.BaseAltraPensione = decimal.Parse(txtAltraPensione.Text);
                        }
                    }
                    break;
            }
            ucDoppioCalcolo.RecuperaCampiComma707(this.areaDatiContributivi);

        }

        internal void EnableDisableBtnSalva(bool enable)
        {
            this.btnSalvaDatiAgo.Enabled = enable;
            this.btnPopUp.Enabled = enable;
            this.btnEliminaDatiAgo.Enabled = enable;
            this.btnSalvaDatiAgoNoRiduzione.Enabled = enable;
        }

        internal bool ManageButtonRiduzioneRetributiva()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica titolare = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
            AreaTitolare.DatiPensione DatiPensione = this.GetDatiPensione(this);

            if (titolare != null && DatiPensione != null)
            {
                if (titolare.DataNascita.HasValue && DatiPensione.DecorrenzaOriginaria.HasValue)
                {
                    if (!(DateTime.Compare(titolare.DataNascita.Value.AddYears(62), DatiPensione.DecorrenzaOriginaria.Value) < 0) &&
                        (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region private methods
        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            // popolamento dei controls html con i valori di default (es.: txtPippo.Text = "mm/aaaa";)
        }

        private void RenderControlsFromTipoCalcoloAndTipoFondo()
        {


            switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS)
                    {
                        divDatiAgo.Visible = true;
                        divDatiContributivi.Visible = true;
                        pnlDatiContributivi.Visible = true;
                        pnlTipoLiquidazioneGAS.Visible = true;
                        pnlDecorrenzaTeoricaGAS.Visible = true;
                        if (areaDatiContributivi.IsContribL214Visible.HasValue && areaDatiContributivi.IsContribL214Visible.Value)
                        {
                            pnlDatiContributiviL214.Visible = true;
                            pnlMontanteEscusivoGAS.Visible = true;
                        }
                    }
                    else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                    {
                        //dati ago
                        divDatiAgo.Visible = true;
                        pnlSettimaneVV.Visible = false;
                        pnlDatiAgoES.Visible = true;
                        pnlTipoLiquidazioneES.Visible = true;
                        //dati contributivi
                        divDatiContributivi.Visible = true;
                        pnlDatiContributivi.Visible = true;
                        pnlDatiContributiviES.Visible = true;
                        //dati contributivi L214    
                        if (areaDatiContributivi.IsContribL214Visible.HasValue && areaDatiContributivi.IsContribL214Visible.Value)
                        {
                            pnlDatiContributiviL214.Visible = true;
                            pnlImportoContributivo.Visible = true;
                        }
                        //altra pensione
                        if (areaDatiContributivi.IsAltraPensioneVisible.HasValue && areaDatiContributivi.IsAltraPensioneVisible.Value)
                        {
                            divAltraPensione.Visible = true;
                            pnlAltraPensione.Visible = true;
                            LoadDdlCategoriaPensione();
                        }
                    }
                    break;
                case GestioneContribTipoCalcolo.Retributivo:
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS)
                    {
                        divDatiAgo.Visible = true;
                        divDatiRetributivi.Visible = true;
                        pnlDatiRetributivi.Visible = true;
                        pnlTipoLiquidazioneGAS.Visible = true;
                        pnlDecorrenzaTeoricaGAS.Visible = true;
                        ManageRiduzioneRetributiva();
                    }
                    else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                    {
                        //dati ago
                        divDatiAgo.Visible = true;
                        pnlSettimaneVV.Visible = false;
                        pnlDatiAgoES.Visible = true;
                        pnlTipoLiquidazioneES.Visible = true;
                        //dati retributivi
                        divDatiRetributivi.Visible = true;
                        pnlDatiRetributivi.Visible = false;
                        pnlDatiRetributiviES.Visible = true;
                        ManageRiduzioneRetributiva();
                        //altra pensione
                        if (areaDatiContributivi.IsAltraPensioneVisible.HasValue && areaDatiContributivi.IsAltraPensioneVisible.Value)
                        {
                            divAltraPensione.Visible = true;
                            pnlAltraPensione.Visible = true;
                            LoadDdlCategoriaPensione();
                        }
                    }
                    break;
                case GestioneContribTipoCalcolo.Misto:
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS)
                    {
                        divDatiAgo.Visible = true;
                        divDatiRetributivi.Visible = true;
                        divDatiContributivi.Visible = true;
                        pnlDatiRetributivi.Visible = true;
                        pnlDatiContributivi.Visible = true;
                        pnlTipoLiquidazioneGAS.Visible = true;
                        pnlDecorrenzaTeoricaGAS.Visible = true;
                        if (areaDatiContributivi.IsContribL214Visible.HasValue && areaDatiContributivi.IsContribL214Visible.Value)
                        {
                            pnlDatiContributiviL214.Visible = pnlMontanteEscusivoGAS.Visible = true;
                        }

                        ManageRiduzioneRetributiva();

                    }
                    else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                    {
                        //dati ago
                        divDatiAgo.Visible = true;
                        pnlSettimaneVV.Visible = false;
                        pnlDatiAgoES.Visible = true;
                        pnlTipoLiquidazioneES.Visible = true;
                        //dati retributivi
                        divDatiRetributivi.Visible = true;
                        pnlDatiRetributivi.Visible = false;
                        pnlDatiRetributiviES.Visible = true;
                        //dati contributivi
                        divDatiContributivi.Visible = true;
                        pnlDatiContributivi.Visible = true;
                        pnlDatiContributiviES.Visible = true;
                        //dati contributivi L214    
                        if (areaDatiContributivi.IsContribL214Visible.HasValue && areaDatiContributivi.IsContribL214Visible.Value)
                        {
                            pnlDatiContributiviL214.Visible = true;
                            pnlImportoContributivo.Visible = true;

                        }
                        ManageRiduzioneRetributiva();
                        //altra pensione
                        if (areaDatiContributivi.IsAltraPensioneVisible.HasValue && areaDatiContributivi.IsAltraPensioneVisible.Value)
                        {
                            divAltraPensione.Visible = true;
                            pnlAltraPensione.Visible = true;
                            LoadDdlCategoriaPensione();
                        }

                    }
                    break;
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS)
                    {
                        divDatiAgo.Visible = true;
                        divDatiRetributivi.Visible = true;
                        pnlDatiRetributivi.Visible = true;
                        divDatiContributivi.Visible = true;
                        pnlTipoLiquidazioneGAS.Visible = true;
                        pnlDecorrenzaTeoricaGAS.Visible = true;
                        if (areaDatiContributivi.IsContribL214Visible.HasValue && areaDatiContributivi.IsContribL214Visible.Value)
                        {
                            pnlDatiContributiviL214.Visible = true;
                            pnlMontanteEscusivoGAS.Visible = true;

                        }
                        ManageRiduzioneRetributiva();
                    }
                    else if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                    {

                        //dati ago
                        divDatiAgo.Visible = true;
                        pnlSettimaneVV.Visible = false;
                        pnlDatiAgoES.Visible = true;
                        pnlTipoLiquidazioneES.Visible = true;
                        //dati retributivi
                        divDatiRetributivi.Visible = true;
                        pnlDatiRetributivi.Visible = false;
                        pnlDatiRetributiviES.Visible = true;
                        //dati contributivi
                        divDatiContributivi.Visible = true;
                        //dati contributivi L214        
                        pnlDatiContributiviL214.Visible = true;
                        pnlImportoContributivo.Visible = true;
                        ManageRiduzioneRetributiva();
                        if (areaDatiContributivi.IsAltraPensioneVisible.HasValue && areaDatiContributivi.IsAltraPensioneVisible.Value)
                        {
                            divAltraPensione.Visible = true;
                            pnlAltraPensione.Visible = true;
                            LoadDdlCategoriaPensione();
                        }
                    }
                    break;
            }
            if (areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault())
            {
                ucDoppioCalcolo.Visible = true;
                ucDoppioCalcolo.SetValidationGroup("UCTabDatiAgoGAS");
            }
        }


        private void LoadDdlCategoriaPensione()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            AreaDecodifica.DatiCategoriaPensione[] listaCategoriePensioni = valoriDecodificati.ElencoCategoriePensione;
            foreach (AreaDecodifica.DatiCategoriaPensione elem in listaCategoriePensioni)
            {
                CodeUtility.SetValueDdl(ddlCategoriaPensione, elem.Sigla, elem.Codice);
            }

        }

        private void LoadDdl()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        LoadDdlGAS();
                        break;
                }
            }
        }

        private void ManageRiduzioneRetributiva()
        {
            if (this.areaDatiContributivi.IsRiduzioneRetribVisible.HasValue && this.areaDatiContributivi.IsRiduzioneRetribVisible.Value)
            {
                pnlRiduzioneRetributiva.Visible = true;
            }
            else
            {
                pnlRiduzioneRetributiva.Visible = false;
            }

            bool IsRiduzionePresent = ManageButtonRiduzioneRetributiva();
            //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
            if (IsRiduzionePresent && this.areaDatiContributivi != null &&
                ((this.areaDatiContributivi.IsUsuranti.HasValue && this.areaDatiContributivi.IsUsuranti.Value) ||
                (this.areaDatiContributivi.TipologiaSalvaguardia.HasValue) ||
                (this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.Value)))
                IsRiduzionePresent = false;
            btnSalvaDatiAgoNoRiduzione.Visible = !IsRiduzionePresent;
            btnPopUp.Visible = IsRiduzionePresent;
            btnSalvaDatiAgo.Visible = IsRiduzionePresent;

            if (this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.Value)
            {
                ddlRiduzioneRetributiva.Enabled = false;
                txtRiduzioneRetributiva.Enabled = false;
            }
        }

        private void GestioneEtichetteIsUnicarpe(AreaTitolare.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.NonValido:
                        break;
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        if (areaDatiContributivi.TipologiaSalvaguardia.HasValue || (areaDatiContributivi.IsUsuranti.HasValue && areaDatiContributivi.IsUsuranti.Value))
                        {
                            ddlRiduzioneRetributiva.Enabled = false;
                            txtRiduzioneRetributiva.Enabled = false;
                        }
                        else
                        {
                            ddlRiduzioneRetributiva.Enabled = true;
                            txtRiduzioneRetributiva.Enabled = true;
                        }
                        break;
                }
            }
            else if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && areaDatiContributivi != null &&
                ((areaDatiContributivi.TipologiaSalvaguardia.HasValue) ||
                (areaDatiContributivi.IsUsuranti.HasValue && areaDatiContributivi.IsUsuranti.Value)))
            {
                switch (areaDatiContributivi.DatiCalcolo.TipoCalcolo)
                {
                    case GestioneContribTipoCalcolo.Retributivo:
                    case GestioneContribTipoCalcolo.Misto:
                    case GestioneContribTipoCalcolo.RetributivoMonti:
                        ddlRiduzioneRetributiva.Enabled = false;
                        txtRiduzioneRetributiva.Enabled = false;
                        break;
                    case GestioneContribTipoCalcolo.Contributivo:
                    case GestioneContribTipoCalcolo.NonValido:
                        break;
                }
            }

        }

        private void LoadDdlGAS()
        {
            ddlTipoLiquidazioneGAS.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlTipoLiquidazioneGAS);
            foreach (TipoLiquidazioneGAS tipoLiquidazioneGAS in this.areaDatiContributivi.ListaTipoLiquidazioneGAS)
                CodeUtility.SetValueDdl(ddlTipoLiquidazioneGAS, tipoLiquidazioneGAS.Id.ToString(), tipoLiquidazioneGAS.Descrizione, tipoLiquidazioneGAS.Id.ToString());
        }

        private void ManageButtons()
        {
            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Pensione == null)
                this.TitolarePensione.Pensione = GetDatiPensione(this);

            if (this.TitolarePensione.Pensione.TipoLetturaUnicarpe != 'L' && Utility.DataSuccessivaA(this.TitolarePensione.Pensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
            {
                btnPopUpContributivi.Style.Remove("display");
                btnPopUp.Style.Remove("display");
                btnSalvaDatiAgo.Style.Remove("display");
                btnSalvaDatiAgoNoRiduzione.Style.Remove("display");

                btnPopUp.Style.Add("display", "none");
                btnSalvaDatiAgo.Style.Add("display", "none");
                btnSalvaDatiAgoNoRiduzione.Style.Add("display", "none");

                RaiseShowPopUp(this, null);
                return;
            }

            btnPopUpContributivi.Style.Remove("display");
            btnPopUp.Style.Remove("display");
            btnSalvaDatiAgo.Style.Remove("display");
            btnSalvaDatiAgoNoRiduzione.Style.Remove("display");

            btnPopUpContributivi.Style.Add("display", "none");
            btnSalvaDatiAgo.Style.Add("display", "none");

            RaiseHidePopUp(this, null);
        }

        #endregion private methods

        #region Event

        public event EventHandler CaricaDatiCalcolo;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event EventHandler ShowPopUp;
        public event EventHandler HidePopUp;

        protected void RaiseCaricaDatiCalcolo(object sender, EventArgs e)
        {
            CaricaDatiCalcolo(sender, e);
        }

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        protected void RaiseShowPopUp(object sender, EventArgs e)
        {
            if (ShowPopUp != null)
                ShowPopUp(sender, e);
        }

        protected void RaiseHidePopUp(object sender, EventArgs e)
        {
            if (HidePopUp != null)
                HidePopUp(sender, e);
        }

        #endregion Event

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public Presenter.SvrLiquidazioneFs.AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region Enum
        public enum EnumViewState
        {
            IsRiduzioneRetribVisible,
        }
        #endregion Enum
    }
}