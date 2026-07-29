using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCPrecedentePensione : CustomBaseUserControl, ILiquidazionePensione, ISedi
    {
        #region ILiquidazionePensione
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ISedi Members
        public string CommaSeparatedSedi { get; set; }
        public Dictionary<string, string> DictionaryOfficeList { get; set; }
        public string Sede { get; set; }
        public List<string> SediAbilitate { get; set; }
        public INPS.DNA.Office SelectedOffice { get; set; }
        #endregion ISedi Members

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

            }
        }


        internal void ValorizzaEtichettePrecedentePensione(ILiquidazionePensione liquidazione, AreaTitolare.DatiPensione datiPensione)
        {
            LoadDdl();
            HiddenFieldSedi.Value = CodeUtility.LoadSedi();

            if (liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione.CodiceP18PrecedentePensione != null)
                {
                    string codiceP18 = liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione.CodiceP18PrecedentePensione.ToString().PadLeft(3, '0');

                    if (ddlCodiceP18.Items.FindByValue(codiceP18) != null)
                        ddlCodiceP18.SelectedValue = codiceP18;
                    else
                    {
                        CodeUtility.SetValueDdl(ddlCodiceP18, codiceP18, codiceP18);
                        ddlCodiceP18.SelectedValue = codiceP18;
                    }
                }
                if (liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione.SedePrecedentePensione != null)
                {
                    txtSede.Text = CodeUtility.GetSede(liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione.SedePrecedentePensione.ToString().PadLeft(4, '0'));
                }
                if (liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione.CertificatoPrecedentePensione != null)
                {
                    txtCertificato.Text = liquidazione.areaLiquidazionePensioneFS.DatiPrecedentePensione.CertificatoPrecedentePensione.ToString().PadLeft(8, '0');
                }

                if (liquidazione.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL || liquidazione.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT ||
                    liquidazione.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET || liquidazione.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL ||
                    liquidazione.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS ||
                    (!liquidazione.domanda.IsDomandaINPDAP && liquidazione.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS && liquidazione.domanda.Tipofondo != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT && liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.GetValueOrDefault() && liquidazione.areaLiquidazionePensioneFS.IsReversibilitaOrRicostituzione.GetValueOrDefault()))
                {
                    if ((liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.HasValue && liquidazione.areaLiquidazionePensioneFS.IsDomandaTrasformazioneAOI.Value) ||
                        (liquidazione.areaLiquidazionePensioneFS.DatiGenerici != null && liquidazione.areaLiquidazionePensioneFS.DatiGenerici.TrasformazioneAOI.GetValueOrDefault() && liquidazione.areaLiquidazionePensioneFS.IsReversibilitaOrRicostituzione.GetValueOrDefault()))
                    {
                        ddlCodiceP18.Enabled = false;
                        txtSede.Enabled = false;
                        txtCertificato.Enabled = false;
                    }
                }
            }

            if (liquidazione.areaLiquidazionePensioneFS.IsRicostituzioneForMemo72.GetValueOrDefault())
            {
                pnlPrecedentePensioneComune.Enabled = false;
            }

            if (Utility.IsDomandaRipristino(datiPensione))
            {
                pnlPrecedentePensioneComune.Enabled = false;
                btnEliminaDatiPrecedPensione.Enabled = false;
            }
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
            areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiPrecedentePensione = GetPrecedentePensione();


            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaPrecedentePensione(this);
            RaiseShowAvviso(this, null);

        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }
        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;


        public INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiPrecedentePensione GetPrecedentePensione()
        {
            INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione areaLiquidazionePensioneFS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiPrecedentePensione = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiPrecedentePensione();

            if (!String.IsNullOrEmpty(ddlCodiceP18.SelectedValue))
                areaLiquidazionePensioneFS.DatiPrecedentePensione.CodiceP18PrecedentePensione = Int16.Parse(ddlCodiceP18.SelectedValue);

            if (!String.IsNullOrEmpty(txtSede.Text))
                areaLiquidazionePensioneFS.DatiPrecedentePensione.SedePrecedentePensione = CodeUtility.ControlSede(txtSede.Text);

            if (!String.IsNullOrEmpty(txtCertificato.Text))
                areaLiquidazionePensioneFS.DatiPrecedentePensione.CertificatoPrecedentePensione = Int32.Parse(txtCertificato.Text);

            return areaLiquidazionePensioneFS.DatiPrecedentePensione;

        }

        internal void ResettaEtichettePrecedentePensione()
        {
            if (ddlCodiceP18.Items != null && ddlCodiceP18.Items.Count > 0)
                ddlCodiceP18.ClearSelection();
            txtSede.Text = "";
            txtCertificato.Text = "";
        }

        protected void btnEliminaDatiPrecedPensione_Click(object sender, EventArgs e)
        {
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaPrecedentePensione(this);

            if (this.HasError)
            {
                this.ErrorMessage = "Errore durante l'eliminazione dei Dati Precedente Pensione";
            }
            else
            {
                ValorizzaEtichetteDatiPrecPensioneEmpty();
            }

            RaiseShowAvvisoElimina(this, null);
        }

        private void ValorizzaEtichetteDatiPrecPensioneEmpty()
        {
            ddlCodiceP18.SelectedIndex = 0;
            txtSede.Text = string.Empty;
            txtCertificato.Text = string.Empty;
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }
    }
}