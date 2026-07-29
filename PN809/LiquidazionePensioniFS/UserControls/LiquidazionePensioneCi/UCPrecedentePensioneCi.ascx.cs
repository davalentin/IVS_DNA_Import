using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi
{
    public partial class UCPrecedentePensioneCi : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneCi
    {
        #region ILiquidazionePensioneCi
        public AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion ILiquidazionePensioneCi

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        internal void ValorizzaEtichettePrecedentePensione(ILiquidazionePensioneCi liquidazioneCi)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);
            if (titolare.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione && !this.domanda.IsDomandaRiapertura)
            {
                TrDecOrig.Visible = false;
                TrDecCarico.Visible = false;
            }
            else
            {
                TrDecOrig.Visible = true;
                TrDecCarico.Visible = true;
            }

            ManageTrasformazioneAOI(titolare.Pensione);

            LoadDdl();
            HiddenFieldSedi.Value = CodeUtility.LoadSedi();
            UtilityTipoAppartenenza? TipoApp;
            TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (CodeUtility.IsDomandaRiliquidazioneAOI(titolare.Pensione) && TipoApp != null && TipoApp == UtilityTipoAppartenenza.CI)
            {
                pnlPrecedentePensione.Enabled = false;
            }
            if (titolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ripristino || titolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.RipristinoSuperstiti)
            {
                btnEliminaPrecedentePensione.Enabled = false;
            }
            if (liquidazioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi != null && liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza != null)
            {
                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.CodiceP18PrecedentePensione.HasValue)
                {
                    string codiceP18 = liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.CodiceP18PrecedentePensione.Value.ToString().PadLeft(3, '0');

                    if (ddlCodiceP18.Items.FindByValue(codiceP18) != null)
                        ddlCodiceP18.SelectedValue = codiceP18;
                    else
                    {
                        CodeUtility.SetValueDdl(ddlCodiceP18, codiceP18, codiceP18);
                        ddlCodiceP18.SelectedValue = codiceP18;
                    }
                }
                else
                    ddlCodiceP18.SelectedIndex = 0;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.SedePrecedentePensione.HasValue)
                    txtSede.Text = CodeUtility.GetSede(liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.SedePrecedentePensione.Value.ToString().PadLeft(4, '0'));
                else
                    txtSede.Text = string.Empty;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.CertificatoPrecedentePensione.HasValue)
                    txtCertificato.Text = liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.CertificatoPrecedentePensione.Value.ToString().PadLeft(8, '0');
                else
                    txtCertificato.Text = string.Empty;

                if (liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.DecorrenzaOriginariaAltraPensione.HasValue)
                    txtDecOriginaria.Text = String.Format("{0:MM/yyyy}", liquidazioneCi.areaLiquidazionePensioneCi.DatiProvenienza.DecorrenzaOriginariaAltraPensione.Value);

                if (titolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ripristino || titolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.RipristinoSuperstiti)
                {
                    txtCertificato.Enabled = false;
                    txtSede.Enabled = false;
                    ddlCodiceP18.Enabled = false;
                }
            }
            else
            {
                ddlCodiceP18.SelectedIndex = 0;
                txtSede.Text = string.Empty;
                txtCertificato.Text = string.Empty;
                txtDecOriginaria.Text = "MM/AAAA";
                txtDecCarico.Text = "MM/AAAA";
            }
        }

        internal DatiProvenienza GetDatiProvenienza()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiProvenienza = new DatiProvenienza();

            if (!String.IsNullOrEmpty(ddlCodiceP18.SelectedValue))
                areaLiquidazionePensioneCi.DatiProvenienza.CodiceP18PrecedentePensione = byte.Parse(ddlCodiceP18.SelectedValue);

            if (!string.IsNullOrEmpty(txtSede.Text))
                areaLiquidazionePensioneCi.DatiProvenienza.SedePrecedentePensione = CodeUtility.ControlSede(txtSede.Text);

            if (!string.IsNullOrEmpty(txtCertificato.Text))
                areaLiquidazionePensioneCi.DatiProvenienza.CertificatoPrecedentePensione = Int32.Parse(txtCertificato.Text);

            if (!string.IsNullOrEmpty(txtDecOriginaria.Text) && !txtDecOriginaria.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiProvenienza.DecorrenzaOriginariaAltraPensione = Utility.GetDateFromString(txtDecOriginaria.Text);

            if (!string.IsNullOrEmpty(txtDecCarico.Text) && !txtDecCarico.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneCi.DatiProvenienza.DecorrenzaCaricoPrecedentePensione = Utility.GetDateFromString(txtDecCarico.Text);

            return areaLiquidazionePensioneCi.DatiProvenienza;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

            }
        }

        protected void SalvaPrecedentePensione_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaLiquidazionePensioneCi = new AreaLiquidazionePensione();
            areaLiquidazionePensioneCi.DatiProvenienza = GetDatiProvenienza();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiPrecedentePensioneCi(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaPrecedentePensione_Click(Object sender, EventArgs e)
        {
            this.areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiPrecedentePensioneCi(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Pensione di Provenienza";
            }
            else
            {
                ValorizzaEtichettePrecedentePensione(null);
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

        private void LoadDdl()
        {
            CodeUtility areaDecodifica = new CodeUtility();
            Presenter.SvrLiquidazione.AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();
            Presenter.SvrLiquidazione.AreaDecodifica.DatiCategoriaPensione[] listaCategorie = datiDecodifica.ElencoCategoriePensione;
            List<AreaDecodifica.DatiCategoriaPensione> listaCat2 = new List<AreaDecodifica.DatiCategoriaPensione>();
            listaCat2 = listaCategorie.ToList();
            listaCat2.Sort((x, y) => String.CompareOrdinal(x.Codice, y.Codice));

            foreach (AreaDecodifica.DatiCategoriaPensione categoria in listaCat2)
                if (!String.IsNullOrEmpty(categoria.Codice))
                {
                    if (categoria.Codice.Length >= 1 && categoria.Codice.Substring(0, 1).Equals("0") &&
                            !ddlCodiceP18.Items.Contains(new ListItem(categoria.Codice.Substring(1), categoria.Codice.Substring(1))))
                    {
                        CodeUtility.SetValueDdl(ddlCodiceP18, categoria.Codice.Substring(1), categoria.Codice.Substring(1));
                    }
                }
        }

        private void ManageTrasformazioneAOI(AreaTitolare.DatiPensione datiPensione)
        {
            if (datiPensione == null)
                datiPensione = GetDatiPensione(this);

            CodeUtility.TipologiaPensioneGruppo tipologiaGruppoPensione = CodeUtility.TipologiaPensioneGruppo.gr_NessunValore;
            CodeUtility.TipologiaPensioneProdotto tipologiaProdottoPensione = CodeUtility.TipologiaPensioneProdotto.pr_NessunValore;
            CodeUtility.TipologiaPensioneTipo tipologiaTipoPensione = CodeUtility.TipologiaPensioneTipo.tp_NessunValore;
            CodeUtility.GetTipologiaPensione(datiPensione.CodeGruppo, datiPensione.CodeProdotto, datiPensione.CodeTipo, out tipologiaGruppoPensione, out tipologiaProdottoPensione, out tipologiaTipoPensione);

            if (tipologiaTipoPensione == CodeUtility.TipologiaPensioneTipo.tp_Vecchiaia_TrasfAOI)
            {
                ddlCodiceP18.Enabled = false;
                txtSede.Enabled = false;
                txtCertificato.Enabled = false;
                TdBtnEliminaPrecedentePensione.Visible = false;
                TdBtnSalvaPrecedentePensione.Style.Clear();
                TdBtnSalvaPrecedentePensione.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
            }
        }

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
    }
}