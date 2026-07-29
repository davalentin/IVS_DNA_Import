using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi
{
    public partial class UCMaternitaAcnaCi : CustomBaseUserControl, IDatiContributiviCi
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                if (this.areaDatiContributiviCi != null)
                {
                    ViewState["DatiContributiviCiM"] = this.areaDatiContributiviCi;

                    if (this.areaDatiContributiviCi.LMaternitaAcna != null)
                        ValorizzaEtichette(this.areaDatiContributiviCi.LMaternitaAcna.ToList());
                    else
                        ValorizzaEtichetteEmpty();
                }
            }
        }

        protected void btnSalvaMaternitaAcna_Click(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito();
            PresenterDatiContributiviCI presenterDatiContributiviCi = new PresenterDatiContributiviCI();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            GetDatiMaternitaAcnaTab();
            if (this.areaDatiContributiviCi.DatiCalcolo == null)
                this.areaDatiContributiviCi.DatiCalcolo = new GestioneContribDatiCalcolo();
            
            presenterDatiContributiviCi.SalvaTabMaternitaAcnaCi(this);

            if (!this.HasError)
                ValorizzaEtichette(areaDatiContributiviCi.LMaternitaAcna.ToList());
            else
            {
                esito.Messaggio = this.ErrorMessage;
                esito.RisultatoOperazione = INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaEsito.TipoEsito.KO;
            }

            RaiseShowAvvisoMaternitaAcna(this, null);
        }

        protected void btnEliminaMaternitaAcna_Click(object sender, EventArgs e)
        {            
            this.domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDatiContributiviCI presenterDatiContributivi = new PresenterDatiContributiviCI();
            presenterDatiContributivi.EliminaTabMaternitaAcnaCi(this);

            if (this.HasError == true)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei dati Maternità / Acna";
            }
            else
            {
                ViewState["DatiContributiviCiM"] = null;
                ValorizzaEtichette(null);
            }

            RaiseShowAvvisoEliminaMaternitaAcna(this, null);
        }

        internal GestioneContribMaternitaAcna[] GetDatiMaternitaAcna()
        {
            return GetDatiMaternitaAcnaTab();
        }

        #region Private Methods

        private void ValorizzaEtichette(List<GestioneContribMaternitaAcna> maternitaAcna)
        {
            if (maternitaAcna == null)
                ValorizzaEtichetteEmpty();
            else
            {
                foreach (GestioneContribMaternitaAcna dati in maternitaAcna)
                {
                    if (dati.Tipo == 'M')
                    {
                        //Maternità
                        if (!string.IsNullOrEmpty(dati.ImportoIVS.ToString()))
                            txtImportoIVSMaternita.Text = dati.ImportoIVS.ToString();
                        else
                            txtImportoIVSMaternita.Text = string.Empty;

                        if (!string.IsNullOrEmpty(dati.SettimaneAl1292.ToString()))
                            txtSettimane31dic92Maternita.Text = dati.SettimaneAl1292.ToString();
                        else
                            txtSettimane31dic92Maternita.Text = string.Empty;

                        if (!string.IsNullOrEmpty(dati.SettimaneDL50392.ToString()))
                            txtSettimaneDL50392Maternita.Text = dati.SettimaneDL50392.ToString();
                        else
                            txtSettimaneDL50392Maternita.Text = string.Empty;
                    }
                    if(dati.Tipo == 'A')
                    {
                        //Cengio
                        if (!string.IsNullOrEmpty(dati.ImportoIVS.ToString()))
                            txtImportoIVSCengio.Text = dati.ImportoIVS.ToString();
                        else
                            txtImportoIVSCengio.Text = string.Empty;

                        if (!string.IsNullOrEmpty(dati.SettimaneAl1292.ToString()))
                            txtSettimane31dic92Cengio.Text = dati.SettimaneAl1292.ToString();
                        else
                            txtSettimane31dic92Cengio.Text = string.Empty;

                        if (!string.IsNullOrEmpty(dati.SettimaneDL50392.ToString()))
                            txtSettimaneDL50392Cengio.Text = dati.SettimaneDL50392.ToString();
                        else
                            txtSettimaneDL50392Cengio.Text = string.Empty;
                    }
                }
                this.areaDatiContributiviCi.LMaternitaAcna = maternitaAcna.ToArray();
                ViewState["DatiContributiviCiM"] = this.areaDatiContributiviCi; 
            }
        }

        private GestioneContribMaternitaAcna[] GetDatiMaternitaAcnaTab()
        {
            List<GestioneContribMaternitaAcna> LMaternitaCengio = new List<GestioneContribMaternitaAcna>();
            GestioneContribMaternitaAcna MaternitaCengio = null;
            if (!string.IsNullOrEmpty(txtImportoIVSMaternita.Text) || !string.IsNullOrEmpty(txtSettimane31dic92Maternita.Text)
                || !string.IsNullOrEmpty(txtSettimaneDL50392Maternita.Text))
            {
                MaternitaCengio = new GestioneContribMaternitaAcna();
                MaternitaCengio.Tipo = 'M';               

                if (string.IsNullOrEmpty(txtImportoIVSMaternita.Text))
                    MaternitaCengio.ImportoIVS = null;
                else
                    MaternitaCengio.ImportoIVS = Convert.ToDecimal(txtImportoIVSMaternita.Text);

                if (string.IsNullOrEmpty(txtSettimane31dic92Maternita.Text))
                    MaternitaCengio.SettimaneAl1292 = null;
                else
                    MaternitaCengio.SettimaneAl1292 = Convert.ToInt32(txtSettimane31dic92Maternita.Text);

                if (string.IsNullOrEmpty(txtSettimaneDL50392Maternita.Text))
                    MaternitaCengio.SettimaneDL50392 = null;
                else
                    MaternitaCengio.SettimaneDL50392 = Convert.ToInt32(txtSettimaneDL50392Maternita.Text);
                
                LMaternitaCengio.Add(MaternitaCengio);
            }
            
            if (!string.IsNullOrEmpty(txtImportoIVSCengio.Text) || !string.IsNullOrEmpty(txtSettimane31dic92Cengio.Text)
                || !string.IsNullOrEmpty(txtSettimaneDL50392Cengio.Text))
            {
                MaternitaCengio = new GestioneContribMaternitaAcna();
                MaternitaCengio.Tipo = 'A';

                if (string.IsNullOrEmpty(txtImportoIVSCengio.Text))
                    MaternitaCengio.ImportoIVS = null;
                else
                    MaternitaCengio.ImportoIVS = Convert.ToDecimal(txtImportoIVSCengio.Text);

                if (string.IsNullOrEmpty(txtSettimane31dic92Cengio.Text))
                    MaternitaCengio.SettimaneAl1292 = null;
                else
                    MaternitaCengio.SettimaneAl1292 = Convert.ToInt32(txtSettimane31dic92Cengio.Text);

                if (string.IsNullOrEmpty(txtSettimaneDL50392Cengio.Text))
                    MaternitaCengio.SettimaneDL50392 = null;
                else
                    MaternitaCengio.SettimaneDL50392 = Convert.ToInt32(txtSettimaneDL50392Cengio.Text);

                LMaternitaCengio.Add(MaternitaCengio);
            }

            if (ViewState["DatiContributiviCiM"] == null)
            {
                this.areaDatiContributiviCi = new AreaDatiContributivi();
                if (LMaternitaCengio != null && LMaternitaCengio.Count > 0)
                {
                    this.areaDatiContributiviCi.LMaternitaAcna = new GestioneContribMaternitaAcna[LMaternitaCengio.Count];
                    this.areaDatiContributiviCi.LMaternitaAcna = LMaternitaCengio.ToArray();
                }
                else
                    this.areaDatiContributiviCi.LMaternitaAcna = null;
                
            }
            else
            {
                this.areaDatiContributiviCi = (AreaDatiContributivi)ViewState["DatiContributiviCiM"];
                if (LMaternitaCengio != null && LMaternitaCengio.Count > 0)
                {
                    this.areaDatiContributiviCi.LMaternitaAcna = new GestioneContribMaternitaAcna[LMaternitaCengio.Count];
                    this.areaDatiContributiviCi.LMaternitaAcna = LMaternitaCengio.ToArray();
                }
                else
                    this.areaDatiContributiviCi.LMaternitaAcna = null;
            }
            
            return this.areaDatiContributiviCi.LMaternitaAcna;
        }

        private void ValorizzaEtichetteEmpty()
        {
            txtImportoIVSMaternita.Text = string.Empty;
            txtSettimane31dic92Maternita.Text = string.Empty;
            txtSettimaneDL50392Maternita.Text = string.Empty;

            txtImportoIVSCengio.Text = string.Empty;
            txtSettimane31dic92Cengio.Text = string.Empty;
            txtSettimaneDL50392Cengio.Text = string.Empty;            
        }

        #endregion Private Methods

        #region EventHandler

        
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler ShowAvvisoMaternitaAcna;
        public event EventHandler ShowAvvisoEliminaMaternitaAcna;

        protected void RaiseShowAvvisoMaternitaAcna(object sender, EventArgs e)
        {
            ShowAvvisoMaternitaAcna(sender, e);
        }

        protected void RaiseShowAvvisoEliminaMaternitaAcna(object sender, EventArgs e)
        {
            ShowAvvisoEliminaMaternitaAcna(sender, e);
        }

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

        #endregion EventHandler

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributiviCi
        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaDatiContributivi areaDatiContributiviCi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributiviCi
    }
}