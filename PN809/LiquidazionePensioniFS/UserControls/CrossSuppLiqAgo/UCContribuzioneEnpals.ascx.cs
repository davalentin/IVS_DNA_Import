using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossSuppLiqAgo
{
    public partial class UCContribuzioneEnpals : CustomBaseUserControl, ICrossContribuzioneEnpals
    {

        #region IContribuzioneEnpals
        public DatiContribuzioneEnpals DatiContribuzioneEnpals { get; set; }
        public TipologiaContribuzioneEnpals Tipologia { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public bool IsContribuzioneEnpalsRetributivaVisible { get; set; }
        public bool IsContribuzioneEnpalsContributivaVisible { get; set; }
        #endregion IContribuzioneEnpals

        #region IView
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IView


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ValorizzaEtichette(ICrossContribuzioneEnpals contribEnpals)
        {
            RenderControls(contribEnpals.DatiContribuzioneEnpals, contribEnpals.IsContribuzioneEnpalsRetributivaVisible, contribEnpals.IsContribuzioneEnpalsContributivaVisible);

            if (contribEnpals.DatiContribuzioneEnpals != null)
            {
                if (contribEnpals.DatiContribuzioneEnpals.QuotaA != null)
                {
                    txtQuotaAEnpals.Text = contribEnpals.DatiContribuzioneEnpals.QuotaA.Enpals.ToString();
                    txtQuotaAEstera.Text = contribEnpals.DatiContribuzioneEnpals.QuotaA.Estera.ToString();
                    txtQuotaAFigurativa.Text = contribEnpals.DatiContribuzioneEnpals.QuotaA.Figurativa.ToString();
                    txtQuotaAInps.Text = contribEnpals.DatiContribuzioneEnpals.QuotaA.Inps.ToString();
                    txtQuotaAUfficio.Text = contribEnpals.DatiContribuzioneEnpals.QuotaA.Ufficio.ToString();
                    txtQuotaAVolontaria.Text = contribEnpals.DatiContribuzioneEnpals.QuotaA.Volontaria.ToString();
                }

                if (contribEnpals.DatiContribuzioneEnpals.QuotaB != null)
                {
                    txtQuotaBEnpals.Text = contribEnpals.DatiContribuzioneEnpals.QuotaB.Enpals.ToString();
                    txtQuotaBEstera.Text = contribEnpals.DatiContribuzioneEnpals.QuotaB.Estera.ToString();
                    txtQuotaBFigurativa.Text = contribEnpals.DatiContribuzioneEnpals.QuotaB.Figurativa.ToString();
                    txtQuotaBInps.Text = contribEnpals.DatiContribuzioneEnpals.QuotaB.Inps.ToString();
                    txtQuotaBUfficio.Text = contribEnpals.DatiContribuzioneEnpals.QuotaB.Ufficio.ToString();
                    txtQuotaBVolontaria.Text = contribEnpals.DatiContribuzioneEnpals.QuotaB.Volontaria.ToString();
                }

                if (contribEnpals.DatiContribuzioneEnpals.QuotaC != null)
                {
                    txtQuotaCEnpals.Text = contribEnpals.DatiContribuzioneEnpals.QuotaC.Enpals.ToString();
                    txtQuotaCEstera.Text = contribEnpals.DatiContribuzioneEnpals.QuotaC.Estera.ToString();
                    txtQuotaCFigurativa.Text = contribEnpals.DatiContribuzioneEnpals.QuotaC.Figurativa.ToString();
                    txtQuotaCInps.Text = contribEnpals.DatiContribuzioneEnpals.QuotaC.Inps.ToString();
                    txtQuotaCUfficio.Text = contribEnpals.DatiContribuzioneEnpals.QuotaC.Ufficio.ToString();
                    txtQuotaCVolontaria.Text = contribEnpals.DatiContribuzioneEnpals.QuotaC.Volontaria.ToString();
                }
            }
        }

        public void RecuperaCampi()
        {
            if (this.domanda == null)
                this.domanda = ((AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"]);

            //Quota A
            if (txtQuotaAEnpals.Text != string.Empty || txtQuotaAEstera.Text != string.Empty || txtQuotaAFigurativa.Text != string.Empty || txtQuotaAInps.Text != string.Empty
                || txtQuotaAUfficio.Text != string.Empty || txtQuotaAVolontaria.Text != string.Empty)
            {
                if (this.DatiContribuzioneEnpals == null)
                    this.DatiContribuzioneEnpals = new DatiContribuzioneEnpals();
                this.DatiContribuzioneEnpals.QuotaA = new DatiContribuzioneEnpals.Quota();

                if (txtQuotaAEnpals.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaA.Enpals = int.Parse(txtQuotaAEnpals.Text);
                if (txtQuotaAEstera.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaA.Estera = int.Parse(txtQuotaAEstera.Text);
                if (txtQuotaAFigurativa.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaA.Figurativa = int.Parse(txtQuotaAFigurativa.Text);
                if (txtQuotaAInps.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaA.Inps = int.Parse(txtQuotaAInps.Text);
                if (txtQuotaAUfficio.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaA.Ufficio = int.Parse(txtQuotaAUfficio.Text);
                if (txtQuotaAVolontaria.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaA.Volontaria = int.Parse(txtQuotaAVolontaria.Text);
            }
            //Quota B
            if (txtQuotaBEnpals.Text != string.Empty || txtQuotaBEstera.Text != string.Empty || txtQuotaBFigurativa.Text != string.Empty || txtQuotaBInps.Text != string.Empty
                || txtQuotaBUfficio.Text != string.Empty || txtQuotaBVolontaria.Text != string.Empty)
            {
                if (this.DatiContribuzioneEnpals == null)
                    this.DatiContribuzioneEnpals = new DatiContribuzioneEnpals();
                this.DatiContribuzioneEnpals.QuotaB = new DatiContribuzioneEnpals.Quota();

                if (txtQuotaBEnpals.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaB.Enpals = int.Parse(txtQuotaBEnpals.Text);
                if (txtQuotaBEstera.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaB.Estera = int.Parse(txtQuotaBEstera.Text);
                if (txtQuotaBFigurativa.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaB.Figurativa = int.Parse(txtQuotaBFigurativa.Text);
                if (txtQuotaBInps.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaB.Inps = int.Parse(txtQuotaBInps.Text);
                if (txtQuotaBUfficio.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaB.Ufficio = int.Parse(txtQuotaBUfficio.Text);
                if (txtQuotaBVolontaria.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaB.Volontaria = int.Parse(txtQuotaBVolontaria.Text);
            }
            //Quota C
            if (txtQuotaCEnpals.Text != string.Empty || txtQuotaCEstera.Text != string.Empty || txtQuotaCFigurativa.Text != string.Empty || txtQuotaCInps.Text != string.Empty
                || txtQuotaCUfficio.Text != string.Empty || txtQuotaCVolontaria.Text != string.Empty)
            {
                if (this.DatiContribuzioneEnpals == null)
                    this.DatiContribuzioneEnpals = new DatiContribuzioneEnpals();
                this.DatiContribuzioneEnpals.QuotaC = new DatiContribuzioneEnpals.Quota();

                if (txtQuotaCEnpals.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaC.Enpals = int.Parse(txtQuotaCEnpals.Text);
                if (txtQuotaCEstera.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaC.Estera = int.Parse(txtQuotaCEstera.Text);
                if (txtQuotaCFigurativa.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaC.Figurativa = int.Parse(txtQuotaCFigurativa.Text);
                if (txtQuotaCInps.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaC.Inps = int.Parse(txtQuotaCInps.Text);
                if (txtQuotaCUfficio.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaC.Ufficio = int.Parse(txtQuotaCUfficio.Text);
                if (txtQuotaCVolontaria.Text != string.Empty)
                    this.DatiContribuzioneEnpals.QuotaC.Volontaria = int.Parse(txtQuotaCVolontaria.Text);
            }
            if (DatiContribuzioneEnpals != null)
            {
                if (HdnTipologia.Value == "SAS")
                    this.DatiContribuzioneEnpals.Tipologia = TipologiaContribuzioneEnpals.SAS;
                else
                    this.DatiContribuzioneEnpals.Tipologia = TipologiaContribuzioneEnpals.SAI;
            }
        }

        public DatiContribuzioneEnpals GetDatiContribuzioneEnpals()
        {
            RecuperaCampi();
            return this.DatiContribuzioneEnpals;
        }

        public void btnSalva_Click(object source, EventArgs args)
        {
            RecuperaCampi();
            Presenter.PresenterContribuzioneEnpals presenter = new Presenter.PresenterContribuzioneEnpals();
            presenter.SalvaContribuzioneEnpalsByDomanda(this);
            RaiseSalvaContributiEnpals(this, null);

        }

        internal void SetHiddenField(TipologiaContribuzioneEnpals tipologia)
        {
            switch (tipologia)
            {
                case TipologiaContribuzioneEnpals.SAI:
                    HdnTipologia.Value = "SAI";
                    break;
                case TipologiaContribuzioneEnpals.SAS:
                    HdnTipologia.Value = "SAS";
                    break;
            }
        }

        private void RenderControls(DatiContribuzioneEnpals datiContribuzioneEnpals, bool isContribuzioneEnpalsRetributivaPresent, bool isContribuzioneEnpalsContributivaPresent)
        {
            // Se i dati retributivi non sono presenti allora non mostro le quote A e B, a meno che non siano presenti dei dati
            if (!isContribuzioneEnpalsRetributivaPresent)
            {
                divQuotaA.Visible = false;
                divQuotaB.Visible = false;

                if (datiContribuzioneEnpals != null && datiContribuzioneEnpals.QuotaA != null && !datiContribuzioneEnpals.QuotaA.Equals(new DatiContribuzioneEnpals.Quota()))
                    divQuotaA.Visible = true;

                if (datiContribuzioneEnpals != null && datiContribuzioneEnpals.QuotaB != null && !datiContribuzioneEnpals.QuotaB.Equals(new DatiContribuzioneEnpals.Quota()))
                    divQuotaB.Visible = true;
            }

            // Se i dati contributivi non sono presenti allora non mostro la quota C, a meno che non siano presenti dei dati
            if (!isContribuzioneEnpalsContributivaPresent)
            {
                divQuotaC.Visible = false;

                if (datiContribuzioneEnpals != null && datiContribuzioneEnpals.QuotaC != null && !datiContribuzioneEnpals.QuotaC.Equals(new DatiContribuzioneEnpals.Quota()))
                    divQuotaC.Visible = true;
            }
        }

        #region Events
        public event EventHandler SalvaContribuzioneEnpals;
        public void RaiseSalvaContributiEnpals(object sender, EventArgs args)
        {
            SalvaContribuzioneEnpals(sender, args);
        }
        #endregion Events
    }
}