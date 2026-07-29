using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public partial class Bititolarita : CustomBasePage, IQuadriSemafori, IBititolarita
    {
        #region IBititolarita
        public Presenter.SvrLiquidazioneAgo.AreaDatiBititolarita areaDatiBititolaritaAgo { get; set; }
        public Presenter.SvrLiquidazioneCi.AreaDatiBititolarita areaDatiBititolaritaCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IBititolarita

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDatiBititolarita();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        private void CaricaDatiBititolarita()
        {
            PresenterBititolarita presenterBititolarita = new PresenterBititolarita();
            switch(this.domanda.TipoAppartenenza)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                    presenterBititolarita.GetBititolaritaAgo(this);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                    presenterBititolarita.GetBititolaritaCi(this);
                    break;
            }

            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlBititolarita.Enabled = false;
                return;
            }
            switch(this.domanda.TipoAppartenenza)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                    pnlTabAltrePensioniAGO.Visible = true;
                    ucAltrePensioniAGO.ValorizzaEtichetteAltrePensioni(this);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                    pnlTabAltrePensioniCI.Visible = true;
                    ucAltrePensioniCI.ValorizzaEtichetteAltrePensioni(this);
                    break;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                switch (this.domanda.TipoAppartenenza)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                        hdnSelected.Value = "#altre_pensioni_AGO";
                        ValorizzaSemaforiTab(imgAltrePensioniAGO, this.areaQuadri.QuadroBititolarita.TabAltrePensioni, pnlTabAltrePensioniAGO);
                        ValorizzaSemaforiTab(imgAltrePensioniCI, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabAltrePensioniCI);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                        hdnSelected.Value = "#altre_pensioni_CI";
                        ValorizzaSemaforiTab(imgAltrePensioniCI, this.areaQuadri.QuadroBititolarita.TabAltrePensioni, pnlTabAltrePensioniCI);
                        ValorizzaSemaforiTab(imgAltrePensioniAGO, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabAltrePensioniAGO);
                        break;
                }
            }
        }

        protected void SalvaBititolarita_Click(object sender, EventArgs e)
        {
            try
            {
                PresenterBititolarita presenterBititolarita = new PresenterBititolarita();
                switch(this.domanda.TipoAppartenenza)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                        this.areaDatiBititolaritaAgo = ucAltrePensioniAGO.GetDatiAltrePensioni();
                        presenterBititolarita.SalvaBititolaritaAgo(this);
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                        this.areaDatiBititolaritaCi = ucAltrePensioniCI.GetDatiAltrePensioni();
                        presenterBititolarita.SalvaBititolaritaCi(this);
                        break;
                }
                if (this.HasError)
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = this.ErrorMessage;
                }
                else
                {
                    ucAvviso.Tipo = TipoAvviso.Ok;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = "Dati Bititolarità salvati correttamente";
                }
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Bititolarita, Errore nel metodo SalvaBititolarita" + ex);
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this,ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoAltrePensioni(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEvents(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Altre Pensioni salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this,ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaAltrePensioni(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEvents(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Altre Pensioni eliminati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this,ucInfoLiquidazione);
        }

        protected void event_ucShowErrorAltrePensioni(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEvents(sender, e, out hasError, out errorMsg);

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
        }

        protected void event_ucAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (!btnSalva.Enabled)
                btnSalva.Enabled = true;
        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalva.Enabled)
                btnSalva.Enabled = false;
        }

        private void GetDataEvents(object sender, Utility.CustomEventArgs e, out bool hasError, out string errorMsg)
        {
            hasError = false;
            errorMsg = string.Empty;
            switch(this.domanda.TipoAppartenenza)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                    UserControls.Bititolarita.UCAltrePensioniAgo tabAltrePensioniAgo = (UserControls.Bititolarita.UCAltrePensioniAgo)sender;
                    hasError = tabAltrePensioniAgo.HasError;
                    errorMsg = tabAltrePensioniAgo.ErrorMessage;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                    UserControls.Bititolarita.UCAltrePensioniCi tabAltrePensioniCi = (UserControls.Bititolarita.UCAltrePensioniCi)sender;
                    hasError = tabAltrePensioniCi.HasError;
                    errorMsg = tabAltrePensioniCi.ErrorMessage;
                    break;
            }
        }
    }
}
