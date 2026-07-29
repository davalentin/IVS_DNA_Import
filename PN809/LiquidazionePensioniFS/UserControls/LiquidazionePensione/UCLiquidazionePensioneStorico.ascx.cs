using System;
using System.Linq;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCDatiStorico : CustomBaseUserControl, ILiquidazionePensione
    {
        #region ILiquidazionePensione
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        internal void ValorizzaEtichette(ILiquidazionePensione liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.HasValue &&
                           liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.Value)
                ViewState[EnumViewState.IsProvvisoriaVisible.ToString()] = liquidazione.areaLiquidazionePensioneFS.IsProvvisoriaVisible.Value;


            LoadDdlCommon(liquidazione, this.domanda.Tipofondo, datiDecodifica);
            RenderPanels();
            RenderControlFromTipoFondo();

            ManageDeroga(liquidazione);

            if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null)
            {
                if (!liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.DecorrenzaOriginaria.HasValue)
                {
                    lblDecorrenzaPensioneDatiGenerici.Text = string.Empty;
                    lblDecorrenzaPensioneDatiAssicurativi.Text = string.Empty;
                }
                else
                {
                    String inputDecorrenza = liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.DecorrenzaOriginaria.ToString();
                    lblDecorrenzaPensioneDatiGenerici.Text = inputDecorrenza.Substring(3, 7);
                    lblDecorrenzaPensioneDatiAssicurativi.Text = inputDecorrenza.Substring(3, 7);
                }

                if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.TipoCalcolo.HasValue)
                    ddlTipoCalcolo.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.TipoCalcolo.Value.ToString();
                else
                    ddlTipoCalcolo.SelectedIndex = 0;

                if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.HasValue)
                    ddlCodComunicazioni3.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.InizioAssicurazione.HasValue)
                    txtPrimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.InizioAssicurazione);

                if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.FineAssicurazione.HasValue)
                    txtUltimoVersamento.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.FineAssicurazione);

                if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.RetribuzioneSettimanaleAgoQuotaA.HasValue)
                    txtRetrAgoQuotaA.Text = liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.RetribuzioneSettimanaleAgoQuotaA.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);

                if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.RetribuzioneSettimanaleAgoQuotaB.HasValue)
                    txtRetrAgoQuotaB.Text = liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.RetribuzioneSettimanaleAgoQuotaB.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
            }
        }

        #region private methods
        private void LoadDdlCommon(ILiquidazionePensione liquidazione, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica)
        {
            ddlTipoCalcolo.Items.Clear();
            ddlTipoCalcolo.Items.Add(new ListItem(string.Empty, " "));
            foreach (INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.TipoCalcolo tipoCalcolo in liquidazione.areaLiquidazionePensioneFS.ListaTipoCalcolo)
                CodeUtility.SetValueDdl(ddlTipoCalcolo, tipoCalcolo.Descrizione, tipoCalcolo.Descrizione, tipoCalcolo.Id);

            SetDdlCodiceComunicazione3(datiDecodifica, liquidazione);

            if (liquidazione.areaLiquidazionePensioneFS != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare != null && liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare.Count() > 0)
                {
                    if (ddlDeroga.Items.Count == 0)
                    {
                        CodeUtility.SetValueDdl(ddlDeroga, string.Empty, string.Empty, string.Empty);
                        foreach (Presenter.SvrLiquidazioneFs.CodiceParticolare codeParticolare in liquidazione.areaLiquidazionePensioneFS.ListaCodiceParticolare)
                            CodeUtility.SetValueDdl(ddlDeroga, codeParticolare.TraduzioneSuGp.GetValueOrDefault().ToString() + " - " + codeParticolare.Descrizione, codeParticolare.Descrizione, codeParticolare.Id.ToString());
                    }
                }
            }
        }

        private void SetDdlCodiceComunicazione3(AreaDecodifica datiDecodifica, ILiquidazionePensione liquidazione)
        {
            ddlCodComunicazioni3.Items.Clear();
            foreach (AreaDecodifica.DatiComunicazioneCampo3 comunicazioneCampo3 in datiDecodifica.ElencoComunicazioneCampo3)
            {
                switch (comunicazioneCampo3.Id)
                {
                    case "Q":
                        if (liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.Equals('Q'))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "":
                        CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    case "P":
                        if (ViewState[EnumViewState.IsProvvisoriaVisible.ToString()] != null && (bool)ViewState[EnumViewState.IsProvvisoriaVisible.ToString()] &&
                         (liquidazione.areaLiquidazionePensioneFS == null || liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico == null || !liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.HasValue
                         || liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.Equals('P')))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                    default:
                        if (ViewState[EnumViewState.IsProvvisoriaVisible.ToString()] != null && (bool)ViewState[EnumViewState.IsProvvisoriaVisible.ToString()] 
                            && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null 
                            && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3 != null
                            && liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceComunicazioneCampo3.ToString().ToUpperInvariant().Equals(comunicazioneCampo3.Id.Trim().ToUpperInvariant()))
                            CodeUtility.SetValueDdl(ddlCodComunicazioni3, comunicazioneCampo3.Id + " - " + comunicazioneCampo3.Descrizione, comunicazioneCampo3.Descrizione, comunicazioneCampo3.Id);
                        break;
                }
            }
        }

        private void ManageDeroga(ILiquidazionePensione liquidazione)
        {
            if (liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico != null &&
                liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceParticolareSoggettoDerogato.HasValue)
            {
                trDeroga.Visible = true;
                ddlDeroga.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiLiquidazionePensioneStorico.CodiceParticolareSoggettoDerogato.Value.ToString();
            }
        }

        private void RenderControlFromTipoFondo()
        {
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    pnlDatiAssicurativiVL.Visible = true;
                    break;
            }
        }

        private void RenderPanels()
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                AreaQuadri areaQuadri = (AreaQuadri)Session["Semaforo"];
                if (areaQuadri.QuadroLiquidazionePensione.TabDatiGenerici != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    pnlDatiGenerici.Visible = true;

                if (areaQuadri.QuadroLiquidazionePensione.TabDatiAssicurativi != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    pnlDatiAssicurativi.Visible = true;
            }
        }
        #endregion private methods

        #region enum
        enum EnumViewState
        {
            IsProvvisoriaVisible,
            IsDomandaProvvisoria
        }
        #endregion enum
    }
}