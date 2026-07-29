using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Eliminazione
{
    public partial class UCEliminazione : CustomBaseUserControl, IEliminazione, ITitolarePensione
    {
        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IEliminazione
        public AreaEliminazione areaEliminazione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IEliminazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region Event

        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #endregion Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Anagrafica == null)
                this.TitolarePensione.Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();
            PresenterEliminazione presenterElimin = new PresenterEliminazione();
            presenterElimin.SalvaDatiEliminazione(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, null);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnElimina_Click(object sender, EventArgs e)
        {
            Presenter.PresenterEliminazione presenterLiquidazione = new PresenterEliminazione();
            presenterLiquidazione.EliminaDatiEliminazione(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Generici";
            else
            {
                ClearForm();
                //ValorizzaEtichette();
            }
            RaiseShowAvvisoElimina(this, null);
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            txtElContDecorrenza.Text = "MM/AAAA";
            txtElContDataEvento.Text = "GG/MM/AAAA";
            HiddenFieldDecorrenza.Value = "MM/AAAA";
        }

        internal void LoadDdl(AreaEliminazione areaElim)
        {
            if (areaElim.ListaCodiceEliminazione != null && areaElim.ListaCodiceEliminazione.Count() > 0)
            {
                if (ddlElContCodice.Items.Count == 0)
                {
                    CodeUtility.SetValueDdl(ddlElContCodice, string.Empty, string.Empty, string.Empty);
                    foreach (GestioneAreaEliminazioneCodiceEliminazione codeEliminazione in areaElim.ListaCodiceEliminazione)
                        CodeUtility.SetValueDdl(ddlElContCodice, codeEliminazione.TestoVideo, codeEliminazione.Descrizione, codeEliminazione.Id);
                }
            }
        }

        internal void SetHiddenFieldIsRicostituzione()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                HiddenFieldIsRicostituzione.Value = "SI";
            else
                HiddenFieldIsRicostituzione.Value = "NO";

            if ((tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura) &&
                 (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                  Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria) || Utility.IsDomandaVESO29(this.domanda.Categoria)))
                HiddenFieldIsRic_TrFAssegniStraordinari.Value = "SI";
        }

        internal void ValorizzaEtichette()
        {
            if (domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.TitolarePensione == null)
                this.TitolarePensione = new AreaTitolare();
            if (this.TitolarePensione.Anagrafica == null)
                this.TitolarePensione.Anagrafica = (AreaRispostaRiepilogo.DatiRiepilogoAnagrafica)Session["Anagrafica"];

            LoadDdl(this.areaEliminazione);

            if (HiddenFieldIsRicostituzione.Value == "SI")
            {
                CodeUtility.BloccaForm(this.domanda, pnlEliminazione);
                RFVtxtDataFineCalcoloArretrati.Enabled = false;
                RFV2txtDataFineCalcoloArretrati.Enabled = false;

                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
                {
                    trFineCalcoloArretrati.Visible = true;
                    if (!this.domanda.IsDomandaENPALS)
                        trCampiRic.Visible = true;
                }

                //ENG - RIC/TRF
                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && IsRicostituzioneOrRiapertura())
                {
                    trFineCalcoloArretrati.Visible = true;
                    lblDataFineCalcoloArretratiGP1AP2A.Visible = false;
                    lblDataFineCalcoloArretratiStorico.Visible = false;
                    txtDataFineCalcoloArretrati.Enabled = true;
                    REVtxtDataFineCalcoloArretrati.Enabled = true;
                    RFVtxtDataFineCalcoloArretrati.Enabled = true;
                    RFV2txtDataFineCalcoloArretrati.Enabled = true;
                    CVtxtDataFineCalcoloArretrati.Enabled = true;
                }


            }

            if (this.areaEliminazione.DatiEliminazione != null)
            {
                if (this.areaEliminazione.DatiEliminazione.CodiceMotivo.HasValue)
                    ddlElContCodice.SelectedValue = this.areaEliminazione.DatiEliminazione.CodiceMotivo.ToString();
                else ddlElContCodice.SelectedIndex = 0;

                if (this.areaEliminazione.DatiEliminazione.DecorrenzaEliminazione.HasValue)
                {
                    //txtElContDecorrenza.Text = String.Format("{0:MM/yyyy}", this.areaEliminazione.DatiEliminazione.DecorrenzaEliminazione.Value);
                    HiddenFieldDecorrenza.Value = String.Format("{0:MM/yyyy}", this.areaEliminazione.DatiEliminazione.DecorrenzaEliminazione.Value);
                }
                else
                {
                    HiddenFieldDecorrenza.Value = "MM/AAAA";
                }

                if (this.areaEliminazione.DatiEliminazione.DataEvento.HasValue)
                    txtElContDataEvento.Text = String.Format("{0:dd/MM/yyyy}", this.areaEliminazione.DatiEliminazione.DataEvento.Value);

                //Eng - Per le TRF e le RIC AGO di pensioni eliminate rendere il campo data evento non editabile dall’operatore
                if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && IsRicostituzioneOrRiapertura())
                {
                    HiddenFieldLockDataEvento.Value = "SI";
                }

                if (this.areaEliminazione.DatiEliminazione.DataCessazioneDiritto.HasValue)
                    txtDataCessazioneDiritto.Text = String.Format("{0:dd/MM/yyyy}", this.areaEliminazione.DatiEliminazione.DataCessazioneDiritto.Value);

                if (this.areaEliminazione.DatiEliminazione.DataComunicazioneEliminazione.HasValue)
                    txtDataComunicazioneEliminazione.Text = String.Format("{0:MM/yyyy}", this.areaEliminazione.DatiEliminazione.DataComunicazioneEliminazione.Value);
            }

            if (this.areaEliminazione.DataFineCalcoloArretratiCalcolata.HasValue)
                txtDataFineCalcoloArretrati.Text = String.Format("{0:MM/yyyy}", this.areaEliminazione.DataFineCalcoloArretratiCalcolata.Value);

            if (this.areaEliminazione.DataFineCalcoloArretratiStorico.HasValue)
                lblDataFineCalcoloArretratiStorico.Text = String.Format("{0:MM/yyyy}", this.areaEliminazione.DataFineCalcoloArretratiStorico.Value);


            if (this.TitolarePensione != null && this.TitolarePensione.Anagrafica != null && this.TitolarePensione.Anagrafica.DataMorte.HasValue)
                HiddenFieldDataMorte.Value = string.Format("{0:dd/MM/yyyy}", this.TitolarePensione.Anagrafica.DataMorte.Value);

            if (Utility.IsDomandaIndennitaUnaTantum_AGO(TitolarePensione.Pensione))
            {
                //ENG - Indennità Una Tantum 
                string abilitaCtrlIndennitàUnaTantumMinorenniAnte300 = string.Empty;
                if (ViewState["AbilitaUnaTantumMinorenniAnte300"] != null)
                    abilitaCtrlIndennitàUnaTantumMinorenniAnte300 = (string)ViewState["AbilitaUnaTantumMinorenniAnte300"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitaUnaTantumMinorenniAnte300", out abilitaCtrlIndennitàUnaTantumMinorenniAnte300);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                        && !String.IsNullOrEmpty(abilitaCtrlIndennitàUnaTantumMinorenniAnte300) && !String.IsNullOrEmpty(abilitaCtrlIndennitàUnaTantumMinorenniAnte300.Trim()))
                        ViewState["AbilitaUnaTantumMinorenniAnte300"] = abilitaCtrlIndennitàUnaTantumMinorenniAnte300.Trim();
                }

                //ddlElContCodice.SelectedIndex = 1;
                if (ddlElContCodice.Items.FindByValue("8") != null)
                {
                    ddlElContCodice.SelectedIndex = ddlElContCodice.Items.IndexOf(ddlElContCodice.Items.FindByValue("8"));
                }
                ddlElContCodice.Enabled = false;
                if (string.IsNullOrEmpty(txtElContDataEvento.Text) || txtElContDataEvento.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                {
                    //ENG -  Se il titolare è nato entro 300 giorni dopo la morte del dante causa, allora la data evento deve essere uguale alla decorrenza originaria
                    if (!String.IsNullOrEmpty(abilitaCtrlIndennitàUnaTantumMinorenniAnte300) && abilitaCtrlIndennitàUnaTantumMinorenniAnte300.Trim().ToUpperInvariant() == "SI" &&
                        TitolarePensione != null && TitolarePensione.Anagrafica != null && TitolarePensione.Anagrafica.DataNascita.HasValue && Session["DataMorteDanteCausa"] != null && TitolarePensione.Pensione.DecorrenzaOriginaria.HasValue
                        && Utility.DataStrettamenteSuccessivaA(TitolarePensione.Anagrafica.DataNascita.Value, Convert.ToDateTime(Session["DataMorteDanteCausa"]))
                        && !Utility.DataStrettamenteSuccessivaA(TitolarePensione.Anagrafica.DataNascita.Value, Convert.ToDateTime(Session["DataMorteDanteCausa"]).AddDays(300)))
                    {
                        txtElContDataEvento.Text = "01/" + TitolarePensione.Pensione.DecorrenzaOriginaria.Value.ToString("MM/yyyy");

                    }
                    else if (Session["DataMorteDanteCausa"] != null)
                        txtElContDataEvento.Text = "01/" + (Convert.ToDateTime(Session["DataMorteDanteCausa"])).AddMonths(1).ToString("MM/yyyy");

                }
                //ENG - Per domande di Indennità Una Tantum il campo Data Evento deve essere bloccato
                HiddenFieldDataEvento.Value = txtElContDataEvento.Text;
                HiddenFieldLockDataEvento.Value = "SI";
                HiddenFieldIsDomandaIndennitaUnaTantum_AGO.Value = "SI";

                //ENG - Per domande di indennità una tantum rinominare la decorrenza e la data evento 
                if (!String.IsNullOrEmpty(abilitaCtrlIndennitàUnaTantumMinorenniAnte300) && abilitaCtrlIndennitàUnaTantumMinorenniAnte300.Trim().ToUpperInvariant() == "SI")
                {
                    lblDecorrenza.InnerText = "Decorrenza Eliminazione:";
                    lblDataEvento.InnerText = "Decorrenza Indennità Una Tantum:";
                }
            }

            if (this.areaEliminazione.IsMemo102Abilitato.GetValueOrDefault() && !CodeUtility.IsRicostituzioneOrRiapertura(TitolarePensione.Pensione, this.domanda.IsDomandaRiapertura) &&
                (Utility.IsDomandaVOESO(this.domanda.Categoria) || Utility.IsDomandaVESO33(this.domanda.Categoria) || Utility.IsDomandaVESO92(this.domanda.Categoria) || Utility.IsDomandaVOCOOP_COOP28(this.domanda.Categoria) ||
                Utility.IsDomandaVOCRED_CRED27(this.domanda.Categoria) || Utility.IsDomandaESPA(this.domanda.Categoria) || Utility.IsDomandaESOTEL(this.domanda.Categoria) || Utility.IsDomandaESOAMB(this.domanda.Categoria)))
            {
                HiddenFieldIsPLAssegniStraordinari.Value = "SI";
                HiddenFieldDecorrenzaPensione.Value = TitolarePensione.Pensione.DecorrenzaOriginaria.GetValueOrDefault().ToString("dd/MM/yyyy");
            }

            HiddenFieldIsMemo102Abilitato.Value = this.areaEliminazione.IsMemo102Abilitato.GetValueOrDefault() ? "SI" : "NO";


            DisabilitaEliminaPerRicostituzioni();
        }

        internal void RecuperaCampi()
        {
            if (areaEliminazione == null)
                this.areaEliminazione = new AreaEliminazione();

            if (this.areaEliminazione.DatiEliminazione == null)
                this.areaEliminazione.DatiEliminazione = new GestionePensioneDatiEliminazione();

            if (!string.IsNullOrEmpty(ddlElContCodice.SelectedValue))
                areaEliminazione.DatiEliminazione.CodiceMotivo = byte.Parse(ddlElContCodice.SelectedValue);

            if (!string.IsNullOrEmpty(HiddenFieldDecorrenza.Value) && !HiddenFieldDecorrenza.Value.ToUpperInvariant().Equals("MM/AAAA"))
                areaEliminazione.DatiEliminazione.DecorrenzaEliminazione = Utility.GetDateFromString(HiddenFieldDecorrenza.Value);

            if (!string.IsNullOrEmpty(txtElContDataEvento.Text) && !txtElContDataEvento.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaEliminazione.DatiEliminazione.DataEvento = Utility.GetDateFromString(txtElContDataEvento.Text);

            if (!areaEliminazione.DatiEliminazione.DataEvento.HasValue && !string.IsNullOrEmpty(HiddenFieldDataEvento.Value))
            {
                areaEliminazione.DatiEliminazione.DataEvento = Utility.GetDateFromString(HiddenFieldDataEvento.Value);
                if (HiddenFieldIsDomandaIndennitaUnaTantum_AGO.Value == "SI")
                    txtElContDataEvento.Text = HiddenFieldDataEvento.Value;
            }

            if (!string.IsNullOrEmpty(txtDataFineCalcoloArretrati.Text) && !txtDataFineCalcoloArretrati.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaEliminazione.DatiEliminazione.DataFineCalcoloArretrati = Utility.GetDateFromString(txtDataFineCalcoloArretrati.Text);

            if (!string.IsNullOrEmpty(txtDataCessazioneDiritto.Text) && !txtDataCessazioneDiritto.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaEliminazione.DatiEliminazione.DataCessazioneDiritto = Utility.GetDateFromString(txtDataCessazioneDiritto.Text);

            if (!string.IsNullOrEmpty(txtDataComunicazioneEliminazione.Text) && !txtDataComunicazioneEliminazione.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaEliminazione.DatiEliminazione.DataCessazioneDiritto = Utility.GetDateFromString(txtDataCessazioneDiritto.Text);
        }

        private void DisabilitaEliminaPerRicostituzioni()
        {
            btnElimina.Enabled = !IsRicostituzioneOrRiapertura();
        }

        private bool IsRicostituzioneOrRiapertura()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (TitolarePensione == null)
                TitolarePensione = new AreaTitolare();
            TitolarePensione.Pensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(TitolarePensione.Pensione.CodeGruppo, TitolarePensione.Pensione.CodeProdotto, TitolarePensione.Pensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);
            if (tipologiaGruppoPensione == CodeUtility.TipologiaPensioneGruppo.gr_Ricostituzione || this.domanda.IsDomandaRiapertura)
                return true;
            return false;
        }
    }
}