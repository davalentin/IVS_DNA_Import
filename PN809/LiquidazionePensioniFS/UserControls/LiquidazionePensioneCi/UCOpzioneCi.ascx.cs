using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi
{
    public partial class UCOpzioneCi : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneCi
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ILiquidazionePensioneCi
        public AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion ILiquidazionePensioneCi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        internal void ValorizzaEtichetteOpzione(ILiquidazionePensioneCi liquidazioneCi)
        {
            LoadDdl(liquidazioneCi);

            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.DataDomandaOpzione.HasValue)
                    txtDataDomandaOpzione.Text = liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.DataDomandaOpzione.Value.ToShortDateString();

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaOpzione.HasValue)
                    txtDecorrenzaOpzione.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaOpzione.Value);

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.CodiceOpzioneRiliquidazione.HasValue)
                    ddlCodiceOpzioneRiliquidazione.SelectedValue = liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.CodiceOpzioneRiliquidazione.Value.ToString();

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaArt2Dpcm.HasValue)
                    txtDecorrenzaDCPM.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaArt2Dpcm.Value);
            }
            else
            {
                txtDataDomandaOpzione.Text = "GG/MM/AAAA";
                txtDecorrenzaOpzione.Text = "MM/AAAA";
                ddlCodiceOpzioneRiliquidazione.SelectedIndex = 0;
                txtDecorrenzaDCPM.Text = "MM/AAAA";
            }
        }

        internal DatiOpzione GetDatiOpzione()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiOpzione = new DatiOpzione();

            if (string.IsNullOrEmpty(txtDataDomandaOpzione.Text) || txtDataDomandaOpzione.Text.ToUpperInvariant().Equals("GG/MM/AAAA"))
                areaLiquidazionePensioneCi.DatiOpzione.DataDomandaOpzione = null;
            else
                areaLiquidazionePensioneCi.DatiOpzione.DataDomandaOpzione = Utility.GetDateFromString(txtDataDomandaOpzione.Text);

            if (string.IsNullOrEmpty(txtDecorrenzaOpzione.Text) || txtDecorrenzaOpzione.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaOpzione = null;
            else
                areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaOpzione = Utility.GetDateFromString(txtDecorrenzaOpzione.Text);

            if (string.IsNullOrEmpty(ddlCodiceOpzioneRiliquidazione.SelectedValue))
                areaLiquidazionePensioneCi.DatiOpzione.CodiceOpzioneRiliquidazione = null;
            else
                areaLiquidazionePensioneCi.DatiOpzione.CodiceOpzioneRiliquidazione = byte.Parse(ddlCodiceOpzioneRiliquidazione.SelectedValue);

            if (string.IsNullOrEmpty(txtDecorrenzaDCPM.Text) || txtDecorrenzaDCPM.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaArt2Dpcm = null;
            else
                areaLiquidazionePensioneCi.DatiOpzione.DecorrenzaArt2Dpcm = Utility.GetDateFromString(txtDecorrenzaDCPM.Text);

            return areaLiquidazionePensioneCi.DatiOpzione;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        protected void SalvaOpzione_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiOpzione = GetDatiOpzione();

            PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiOpzioneCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaOpzione_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiOpzioneCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Opzione";
            }
            else
            {
                ValorizzaEtichetteOpzione(null);
            }

            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        private void LoadDdl(ILiquidazionePensioneCi liquidazioneCi)
        {
            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.lOpzioneRiliquidazione != null && liquidazioneCi.areaLiquidazionePensioneCi.lOpzioneRiliquidazione.Count() > 0)
                {
                    CodeUtility.SetValueDdl(ddlCodiceOpzioneRiliquidazione, string.Empty, string.Empty);
                    foreach (OpzioneRiliquidazione opzioneRil in liquidazioneCi.areaLiquidazionePensioneCi.lOpzioneRiliquidazione)
                    {
                        string testo = string.Format("{0} - {1}", opzioneRil.Id, opzioneRil.Descrizione);
                        CodeUtility.SetValueDdl(ddlCodiceOpzioneRiliquidazione, testo, opzioneRil.Id.ToString());
                    }
                }
            }
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
    }
}