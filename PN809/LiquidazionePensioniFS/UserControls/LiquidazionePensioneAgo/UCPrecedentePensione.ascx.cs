using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCPrecedentePensione : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {
        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        internal void ValorizzaEtichettePrecedentePensione(ILiquidazionePensioneAgo liquidazioneAgo)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //visible = false per FS, CI,AGO per gruppo != 0031 
            AreaTitolare titolare = new AreaTitolare();
            titolare.Pensione = GetDatiPensione(this);
            if (titolare.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione && !this.domanda.IsDomandaRiapertura
                && titolare.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Riliquidazione && titolare.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.RiliquidazioneSuperstiti
                && titolare.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.Ripristino && titolare.Pensione.Tipo != AreaTitolare.DatiPensione.TipoDomanda.RipristinoSuperstiti)
            {
                TrDecOrig.Visible = false;
                TrDecCarico.Visible = false;
            }
            else
            {
                TrDecOrig.Visible = true;
                TrDecCarico.Visible = true;
            }
            LoadDdl();
            HiddenFieldSedi.Value = CodeUtility.LoadSedi();

            if (liquidazioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo != null && liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza != null)
            {
                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.CodiceP18PrecedentePensione.HasValue)
                {
                    string codiceP18 = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.CodiceP18PrecedentePensione.Value.ToString().PadLeft(3, '0');

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

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.SedePrecedentePensione.HasValue)
                {
                    txtSede.Text = CodeUtility.GetSede(liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.SedePrecedentePensione.Value.ToString().PadLeft(4, '0'));
                    if (string.IsNullOrEmpty(txtSede.Text) && liquidazioneAgo.areaLiquidazionePensioneAgo.DataAssunzioneCarico.HasValue &&
                        Utility.IsDomandaDAIAnte2003(liquidazioneAgo.areaLiquidazionePensioneAgo.DataAssunzioneCarico.Value, this.domanda.Categoria))
                        txtSede.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.SedePrecedentePensione.Value.ToString().PadLeft(4, '0');
                }
                else
                    txtSede.Text = string.Empty;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.CertificatoPrecedentePensione.HasValue)
                    txtCertificato.Text = liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.CertificatoPrecedentePensione.Value.ToString().PadLeft(8, '0');
                else
                    txtCertificato.Text = string.Empty;

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaOriginariaAltraPensione.HasValue)
                    txtDecOriginaria.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaOriginariaAltraPensione.Value);

                if (liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaCaricoPrecedentePensione.HasValue)
                    txtDecCarico.Text = String.Format("{0:MM/yyyy}", liquidazioneAgo.areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaCaricoPrecedentePensione.Value);
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
            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiProvenienza = new DatiProvenienza();

            if (!String.IsNullOrEmpty(ddlCodiceP18.SelectedValue))
                areaLiquidazionePensioneAgo.DatiProvenienza.CodiceP18PrecedentePensione = short.Parse(ddlCodiceP18.SelectedValue);

            if (!string.IsNullOrEmpty(txtSede.Text))
                areaLiquidazionePensioneAgo.DatiProvenienza.SedePrecedentePensione = CodeUtility.ControlSede(txtSede.Text);

            if (!string.IsNullOrEmpty(txtCertificato.Text))
                areaLiquidazionePensioneAgo.DatiProvenienza.CertificatoPrecedentePensione = Int32.Parse(txtCertificato.Text);

            if (!string.IsNullOrEmpty(txtDecOriginaria.Text) && !txtDecOriginaria.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaOriginariaAltraPensione = Utility.GetDateFromString(txtDecOriginaria.Text);

            if (!string.IsNullOrEmpty(txtDecCarico.Text) && !txtDecCarico.Text.ToUpperInvariant().Equals("MM/AAAA"))
                areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaCaricoPrecedentePensione = Utility.GetDateFromString(txtDecCarico.Text);

            return areaLiquidazionePensioneAgo.DatiProvenienza;
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

        protected void SalvaPrecedentePensione_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiProvenienza = GetDatiProvenienza();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiPrecedentePensioneAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void EliminaPrecedentePensione_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiPrecedentePensioneAgo(this);

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

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;
    }
}