using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici
{
    public partial class UCExCombattente : CustomBaseUserControl, IMaggiorazioneBenefici
    {
        public const string Legge140 = "Legge140";
        public const string Legge336 = "Legge336";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    if (IsPostBack)
                    {
                        if (HiddenSelectedLegge.Value == Legge140)
                            AbilitaPannelloLegge140();
                        if (HiddenSelectedLegge.Value == Legge336)
                            AbilitaPannelloLegge336();
                    }

                    AbilitaPannelli();
                    break;
                default:
                    if (this.domanda.IsDomandaINPDAP)
                    {
                        if (IsPostBack)
                        {
                            if (HiddenSelectedLegge.Value == Legge140)
                                AbilitaPannelloLegge140();
                            if (HiddenSelectedLegge.Value == Legge336)
                                AbilitaPannelloLegge336();
                        }

                        AbilitaPannelli();

                    }
                    break;
            }
        }

        protected void SalvaExCombattente_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaMaggiorazioneBenefici = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();
            areaMaggiorazioneBenefici.DatiExCombattente = GetValoriExCombattente();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaExCombattente(this);

            if (!this.HasError)
                ValorizzaEtichetteExCombattente(this);
            else
            {
                if (HiddenSelectedLegge.Value == Legge140)
                {
                    AbilitaPannelloLegge140();
                    resetLegge336();
                }
                if (HiddenSelectedLegge.Value == Legge336)
                {
                    AbilitaPannelloLegge336();
                    resetLegge140();
                }
            }
            RaiseShowAvviso(this, null);
        }

        protected void EliminaExCombattente_Click(Object sender, EventArgs e)
        {
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.EliminaExCombattente(this);

            if (!this.HasError)
            {
                ClearForm();
                ValorizzaEtichetteExCombattente(this);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        internal Presenter.SvrLiquidazioneFs.DatiExCombattente GetValoriExCombattente()
        {
            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiExCombattente = new Presenter.SvrLiquidazioneFs.DatiExCombattente();

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    this.areaMaggiorazioneBenefici.DatiExCombattente = GetValoriExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    this.areaMaggiorazioneBenefici.DatiExCombattente = GetValoriExCombattentePT();
                    break;
                default:
                    if (this.domanda.IsDomandaINPDAP)
                        this.areaMaggiorazioneBenefici.DatiExCombattente = GetValoriExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI();

                    break;
            }

            return this.areaMaggiorazioneBenefici.DatiExCombattente;
        }

        internal void ValorizzaEtichetteExCombattente(IMaggiorazioneBenefici maggiorazioneBenefici)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            this.areaMaggiorazioneBenefici = maggiorazioneBenefici.areaMaggiorazioneBenefici;

            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        LoadDdl(this.domanda.Tipofondo);
                        RenderControlsFromTipoFondo(datiPensione);
                        ValorizzaEtichetteExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI();
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        RenderControlsFromTipoFondo(datiPensione);
                        ValorizzaEtichetteExCombattentePT();
                        break;
                }
            }
            else if (this.domanda.IsDomandaINPDAP)
            {
                LoadDdl(this.domanda.Tipofondo);
                RenderControlsFromTipoFondo(datiPensione);
                ValorizzaEtichetteExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI();
            }

            GestioneEtichetteRic(datiPensione);
        }

        private Presenter.SvrLiquidazioneFs.DatiExCombattente GetValoriExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI()
        {
            Presenter.SvrLiquidazioneFs.DatiExCombattente exCombattente = new Presenter.SvrLiquidazioneFs.DatiExCombattente();
            if (HiddenSelectedLegge.Value == Legge140)
            {
                exCombattente.CodiceCieco = !string.IsNullOrEmpty(ddlExCombattente.SelectedValue) ? byte.Parse(ddlExCombattente.SelectedValue) : (byte?)null;
                exCombattente.DecorrenzaMaggiorazioneArt6 = !string.IsNullOrEmpty(txtDecorrenza.Text) && !txtDecorrenza.Text.Equals("MM/AAAA") ? Utility.GetDateFromString(txtDecorrenza.Text) : (DateTime?)null;
                exCombattente.ExCombattente = null;
                exCombattente.RMSSenzaLegge33670QA = null;
                exCombattente.RMSSenzaLegge33670QB = null;
                exCombattente.PercentualeMaggiorazioneSenzaLegge33670 = null;
            }
            if (HiddenSelectedLegge.Value == Legge336)
            {
                exCombattente.ExCombattente = ddlMaggExCombattente.SelectedIndex != 0 ? byte.Parse(ddlMaggExCombattente.SelectedValue) : (byte?)null;
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        GetValori336EL_ET_TT_VL_GAS_PM(ref exCombattente);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                        GetValori336DZ(ref exCombattente);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        GetValori336ES_PI(ref exCombattente);
                        break;

                }
                exCombattente.CodiceCieco = null;
                exCombattente.DecorrenzaMaggiorazioneArt6 = null;
            }
            return exCombattente;
        }

        private void GetValori336EL_ET_TT_VL_GAS_PM(ref Presenter.SvrLiquidazioneFs.DatiExCombattente exCombattente)
        {
            exCombattente.RMSSenzaLegge33670QA = !string.IsNullOrEmpty(txtRMSL33670QuotaA.Text) ? decimal.Parse(txtRMSL33670QuotaA.Text) : (decimal?)null;
            exCombattente.RMSSenzaLegge33670QB = !string.IsNullOrEmpty(txtRMSL33670QuotaB.Text) ? decimal.Parse(txtRMSL33670QuotaB.Text) : (decimal?)null;
            exCombattente.PercentualeMaggiorazioneSenzaLegge33670 = !string.IsNullOrEmpty(txtPercentualeMaggSL33670.Text) ? byte.Parse(txtPercentualeMaggSL33670.Text) : (byte?)null;
        }

        private void GetValori336DZ(ref Presenter.SvrLiquidazioneFs.DatiExCombattente exCombattente)
        {
            exCombattente.RMSSenzaLegge33670QA = !string.IsNullOrEmpty(txtRMSL33670QuotaA.Text) ? decimal.Parse(txtRMSL33670QuotaA.Text) : (decimal?)null;
            exCombattente.RMSSenzaLegge33670QB = !string.IsNullOrEmpty(txtRMSL33670QuotaB.Text) ? decimal.Parse(txtRMSL33670QuotaB.Text) : (decimal?)null;
        }

        private void GetValori336ES_PI(ref Presenter.SvrLiquidazioneFs.DatiExCombattente exCombattente)
        {
            exCombattente.RMSSenzaLegge33670QA = !string.IsNullOrEmpty(txtRMSL33670QuotaA.Text) ? decimal.Parse(txtRMSL33670QuotaA.Text) : (decimal?)null;
        }

        private Presenter.SvrLiquidazioneFs.DatiExCombattente GetValoriExCombattentePT()
        {
            Presenter.SvrLiquidazioneFs.DatiExCombattente exCombattente = new Presenter.SvrLiquidazioneFs.DatiExCombattente();

            if (string.IsNullOrEmpty(txtDirittoScatti.Text))
                exCombattente.DirittoScattiLegge336 = null;
            else
                exCombattente.DirittoScattiLegge336 = Int32.Parse(txtDirittoScatti.Text);

            return exCombattente;
        }

        private void LoadDdl(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo)
        {
            if (this.areaMaggiorazioneBenefici != null)
            {
                //Legge 140
                if (this.areaMaggiorazioneBenefici.ListaCodiceCieco != null)
                {
                    if (tipoFondo.HasValue && tipoFondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES)
                    {
                        ddlExCombattente.Items.Clear();
                        foreach (Presenter.SvrLiquidazioneFs.CodiceCieco codicecieco in this.areaMaggiorazioneBenefici.ListaCodiceCieco)
                            if (codicecieco.Id.Trim() == "0")
                                CodeUtility.SetValueDdl(ddlExCombattente, codicecieco.Descrizione, codicecieco.Descrizione, codicecieco.Id);
                    }
                    else
                    {
                        ddlExCombattente.Items.Clear();
                        CodeUtility.SetItemBlankDdl(ddlExCombattente);
                        foreach (Presenter.SvrLiquidazioneFs.CodiceCieco codicecieco in this.areaMaggiorazioneBenefici.ListaCodiceCieco)
                            if (codicecieco.Id.Trim() == "8" || codicecieco.Id.Trim() == "9")
                                CodeUtility.SetValueDdl(ddlExCombattente, codicecieco.Descrizione, codicecieco.Descrizione, codicecieco.Id);
                    }
                }

                if (tipoFondo.HasValue && tipoFondo.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS)
                {
                    //Legge 336
                    if (this.areaMaggiorazioneBenefici.ListaCodiceMaggiorazioneExCombattente != null)
                    {
                        ddlMaggExCombattente.Items.Clear();
                        CodeUtility.SetItemBlankDdl(ddlMaggExCombattente);
                        foreach (Presenter.SvrLiquidazioneFs.CodiceMaggiorazioneExCombattente codiceExCombattente in this.areaMaggiorazioneBenefici.ListaCodiceMaggiorazioneExCombattente)
                            CodeUtility.SetValueDdl(ddlMaggExCombattente, codiceExCombattente.Descrizione, codiceExCombattente.Descrizione, codiceExCombattente.Id.ToString());
                    }
                }
            }
        }

        private void RenderControlsFromTipoFondo(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        this.pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnl336EL_ET_GAS_DZ_ES_PM_PI.Visible = true;
                        if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL && CodeUtility.IsRicostituzione(datiPensione))
                            HdnFondoEL.Value = "SI";
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        this.pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pdivL336.Visible = false;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        this.pnlExCombattentePT.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                        this.pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnl336EL_ET_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnlPercentualeMaggSL33670.Visible = false;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                        this.pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnl336EL_ET_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnlPercentualeMaggSL33670.Visible = false;
                        this.pnlRMSL33670QuotaB.Visible = false;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        this.pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnl336EL_ET_GAS_DZ_ES_PM_PI.Visible = true;
                        this.pnlPercentualeMaggSL33670.Visible = false;
                        this.pnlRMSL33670QuotaB.Visible = false;
                        HiddenSelectedLegge.Value = Legge336;
                        if (this.areaMaggiorazioneBenefici.DatiExCombattente == null)
                            this.areaMaggiorazioneBenefici.DatiExCombattente = new Presenter.SvrLiquidazioneFs.DatiExCombattente();
                        pdivL140.Style.Add("display", "none");
                        break;
                }

                if (CodeUtility.IsRicostituzione(datiPensione) 
                    && !CodeUtility.IsRicostituzioneContributiva(datiPensione)
                    && this.domanda.Tipofondo.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT
                    && this.domanda.Tipofondo.Value != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS)
                    pdivL336.Visible = false;

                // ENG - Abilitato pannello legge 336 sul pannello benefici della linea fondi speciali
                if (datiPensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
                {
                    if (CodeUtility.IsRicostituzione(datiPensione)
                                && (
                                    this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI
                                    || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL
                                    )
                                )
                    {
                        pdivL336.Visible = true;
                    } 
                }
            }
            else if (this.domanda.IsDomandaINPDAP)
            {
                this.pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.Visible = true;
                this.pdivL336.Visible = false;
            }
        }

        private void ValorizzaEtichetteExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI()
        {
            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.DatiExCombattente != null)
            {
                AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();
                if (HiddenSelectedLegge.Value == Legge140 
                    || this.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco.HasValue 
                    || this.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue)
                {
                    AbilitaPannelloLegge140();
                    if (this.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco.HasValue)
                        ddlExCombattente.SelectedValue = this.areaMaggiorazioneBenefici.DatiExCombattente.CodiceCieco.Value.ToString();
                    else
                        ddlExCombattente.SelectedIndex = 0;
                    if (this.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6.HasValue)
                        txtDecorrenza.Text = String.Format("{0:MM/yyyy}", this.areaMaggiorazioneBenefici.DatiExCombattente.DecorrenzaMaggiorazioneArt6.Value);

                    //Legge 336
                    //ddlMaggExCombattente.SelectedIndex = 0;
                    //txtRMSL33670QuotaA.Text        = string.Empty;
                    //txtRMSL33670QuotaB.Text        = string.Empty;
                    //txtPercentualeMaggSL33670.Text = string.Empty;
                    //txtRetribuzioneSenzaBenefici336.Text = string.Empty;

                    //Eng - Ricostituzioni dei fondi != FS e PT se legge 140 selezionata, legge 336 disabilitata e se legge 336 selezionata, 140 disabilitata
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            if (CodeUtility.IsRicostituzione(datiPensione))
                            {
                                radioL336.Enabled = false;
                            }
                            break;
                    }
                    
                    resetLegge336();
                }

                if (HiddenSelectedLegge.Value == Legge336 
                    || this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.HasValue 
                    || (this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.HasValue 
                        && this.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS) 
                        || this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QB.HasValue 
                        || this.areaMaggiorazioneBenefici.DatiExCombattente.PercentualeMaggiorazioneSenzaLegge33670.HasValue
                    )
                {
                    AbilitaPannelloLegge336();

                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            ValorizzaEtichette336EL_ET_TT_VL_GAS_PM(datiPensione);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                            ValorizzaEtichette336DZ();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                            ValorizzaEtichette336ES_PI();
                            break;
                    }
                    //Legge 140
                    //ddlExCombattente.SelectedIndex = 0;
                    //txtDecorrenza.Text = string.Empty;

                    resetLegge140();
                }
            }
            else
            {
                HiddenSelectedLegge.Value = string.Empty;
                radioL140.Checked = false;
                radioL336.Checked = false;

                //Legge 140
                //ddlExCombattente.SelectedIndex = 0;
                //txtDecorrenza.Text = string.Empty;

                resetLegge140();

                //Legge 336
                //ddlMaggExCombattente.SelectedIndex = 0;
                //txtRMSL33670QuotaA.Text = string.Empty;
                //txtRMSL33670QuotaB.Text = string.Empty;
                //txtPercentualeMaggSL33670.Text = string.Empty;
                //txtRetribuzioneSenzaBenefici336.Text = string.Empty;

                resetLegge336();
            }
        }

        private void ValorizzaEtichetteExCombattentePT()
        {
            if (this.areaMaggiorazioneBenefici != null && this.areaMaggiorazioneBenefici.DatiExCombattente != null)
                this.txtDirittoScatti.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.DirittoScattiLegge336.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.DirittoScattiLegge336.Value.ToString() : string.Empty;
        }

        private void ValorizzaEtichette336EL_ET_TT_VL_GAS_PM(AreaTitolare.DatiPensione datiPensione)
        {
            if (this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.HasValue)
                ddlMaggExCombattente.SelectedValue = this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.Value.ToString();
            txtRMSL33670QuotaA.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtRMSL33670QuotaB.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QB.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtPercentualeMaggSL33670.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.PercentualeMaggiorazioneSenzaLegge33670.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.PercentualeMaggiorazioneSenzaLegge33670.Value.ToString() : string.Empty;

            //Eng - Ricostituzioni dei fondi != FS e PT che hanno i dati della Legge 336 sul tab Maggiorazioni valori a 0 di default 
            //se legge 140 selezionata, legge 336 disabilitata e se legge 336 selezionata 140 disabilitata
            if (CodeUtility.IsRicostituzione(datiPensione))
            {
                if (txtRMSL33670QuotaA.Text == "")
                    txtRMSL33670QuotaA.Text = "0";
                if (txtRMSL33670QuotaB.Text == "")
                    txtRMSL33670QuotaB.Text = "0";
                if (txtPercentualeMaggSL33670.Text == "")
                    txtPercentualeMaggSL33670.Text = "0";
                radioL140.Enabled = false;
            }
        }

        private void ValorizzaEtichette336DZ()
        {
            if (this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.HasValue)
                ddlMaggExCombattente.SelectedValue = this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.Value.ToString();
            txtRMSL33670QuotaA.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
            txtRMSL33670QuotaB.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QB.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void ValorizzaEtichette336ES_PI()
        {
            if (this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.HasValue)
                ddlMaggExCombattente.SelectedValue = this.areaMaggiorazioneBenefici.DatiExCombattente.ExCombattente.Value.ToString();
            txtRMSL33670QuotaA.Text = this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.HasValue ? this.areaMaggiorazioneBenefici.DatiExCombattente.RMSSenzaLegge33670QA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {

        }

        private void AbilitaPannelli()
        {
            radioL140.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioL140.InputAttributes.Add("EnableClass", "onClassLegge140");
            radioL336.Attributes.Add("onclick", "javascript:SetRadio(this)");
            radioL336.InputAttributes.Add("EnableClass", "onClassLegge336");
        }

        private void AbilitaPannelloLegge140()
        {
            HiddenSelectedLegge.Value = Legge140;
            radioL140.Checked = true;
            radioL336.Checked = false;
        }

        private void AbilitaPannelloLegge336()
        {
            HiddenSelectedLegge.Value = Legge336;
            radioL140.Checked = false;
            radioL336.Checked = true;
        }

        private void resetLegge140()
        {
            ddlExCombattente.SelectedIndex = 0;
            txtDecorrenza.Text = string.Empty;
        }

        private void resetLegge336()
        {
            ddlMaggExCombattente.SelectedIndex = 0;
            txtRMSL33670QuotaA.Text = string.Empty;
            txtRMSL33670QuotaB.Text = string.Empty;
            txtPercentualeMaggSL33670.Text = string.Empty;
        }

        private void GestioneEtichetteRic(AreaTitolare.DatiPensione datiPensione)
        {
            if (CodeUtility.IsRicostituzione(datiPensione) 
                && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) 
                && !(
                    this.domanda.IsDomandaINPDAP 
                    || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS 
                    || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT
                    )
                )
            {
                ddlExCombattente.Enabled = false;
                txtDecorrenza.Enabled = false;
                radioL336.Enabled = false;
                ddlMaggExCombattente.Enabled = false;
                txtRMSL33670QuotaA.Enabled = false;
                txtRMSL33670QuotaB.Enabled = false;
                txtPercentualeMaggSL33670.Enabled = false;
                btnEliminaExCombattente.Enabled = false;
            }

            // ENG - Abilitato pannello legge 336 sul pannello benefici della linea fondi speciali
            if (datiPensione.TipoAppartenenzaDomanda.Value == AreaTitolare.DatiPensione.TipoAppDomanda.FS)
            {
                if (CodeUtility.IsRicostituzione(datiPensione)
                        && (this.domanda.Tipofondo.HasValue &&
                            (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI
                            || this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL)
                            )
                        )
                {
                    radioL336.Enabled = true;
                } 
            }
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneFs.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IMaggiorazioneBenefici
    }
}