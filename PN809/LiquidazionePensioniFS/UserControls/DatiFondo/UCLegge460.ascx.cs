using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo
{
    public partial class UCLegge460 : CustomBaseUserControl, IDatiFondo
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiFondo
        public AreaDatiFondo areaDatiFondo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiFondo

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichette(AreaDatiFondo areaDatiFondo)
        {
            ClearForm();

            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            HiddenFieldSedi.Value = CodeUtility.LoadSedi();

            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            LoadDdl(datiDecodifica, datiPensione);

            if (areaDatiFondo != null && areaDatiFondo.DatiLegge460 != null)
            {
                ViewState[EnumViewState.IdRecordFondo.ToString()] = areaDatiFondo.IdRecordFondo;

                if (areaDatiFondo.DatiLegge460.SiglaCategoria.HasValue)
                    ddlCodiceCategoria.SelectedValue = areaDatiFondo.DatiLegge460.SiglaCategoria.ToString().PadLeft(3, '0');

                if (areaDatiFondo.DatiLegge460.CodiceSede.HasValue)
                    txtSede.Text = CodeUtility.GetSede(areaDatiFondo.DatiLegge460.CodiceSede.ToString().PadLeft(4, '0'));

                if (!string.IsNullOrEmpty(areaDatiFondo.DatiLegge460.NCertificato))
                    txtCertificato.Text = areaDatiFondo.DatiLegge460.NCertificato.PadLeft(8, '0');

                if (areaDatiFondo.DatiLegge460.NMesiRiscattati.HasValue)
                    txtnumMesiRiscatti.Text = areaDatiFondo.DatiLegge460.NMesiRiscattati.Value.ToString();

                if (areaDatiFondo.DatiLegge460.NMesiTotali.HasValue)
                    txtNumMesiTotali.Text = areaDatiFondo.DatiLegge460.NMesiTotali.Value.ToString();
            }

            if ((this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)
               && (Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione)))
            {
                ddlCodiceCategoria.Enabled = false;
                txtCertificato.Enabled = false;
                txtnumMesiRiscatti.Enabled = false;
                txtNumMesiTotali.Enabled = false;
                txtSede.Enabled = false;
                btnEliminaDatiLegge460.Enabled = false;
            }
        }

        internal Presenter.SvrLiquidazioneFs.DatiLegge460 RecuperaCampi()
        {
            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.DatiLegge460 = new DatiLegge460();

            this.areaDatiFondo.DatiLegge460.SiglaCategoria = ddlCodiceCategoria.SelectedIndex != 0 ? Convert.ToInt32(ddlCodiceCategoria.SelectedValue) : (Int32?)null;
            this.areaDatiFondo.DatiLegge460.CodiceSede = !string.IsNullOrEmpty(txtSede.Text) ? CodeUtility.ControlSede(txtSede.Text) : (short?)null;
            this.areaDatiFondo.DatiLegge460.NCertificato = txtCertificato.Text;
            this.areaDatiFondo.DatiLegge460.NMesiRiscattati = (!string.IsNullOrEmpty(txtnumMesiRiscatti.Text)) ? Convert.ToInt32(txtnumMesiRiscatti.Text) : (Int32?)null;
            this.areaDatiFondo.DatiLegge460.NMesiTotali = (!string.IsNullOrEmpty(txtNumMesiTotali.Text)) ? Convert.ToInt32(txtNumMesiTotali.Text) : (Int32?)null;

            return this.areaDatiFondo.DatiLegge460;
        }

        protected void btnSalvaDatiLegge460_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RecuperaCampi();

            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.StoreDatiLegge460ByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Legge 4/60 salvati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiLegge460(this, null);
            }
        }

        protected void btnEliminaDatiLegge460_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.areaDatiFondo == null)
                this.areaDatiFondo = new AreaDatiFondo();
            this.areaDatiFondo.IdRecordFondo = (long)ViewState[EnumViewState.IdRecordFondo.ToString()];

            Presenter.PresenterDatiFondo presenter = new Presenter.PresenterDatiFondo();
            presenter.EliminaDatiLegge460ByIdRecordFondo(this);

            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati Legge 4/60 eliminati correttamente.";
                RaiseShowAvviso(this, null);
                RaiseUpdateSemaforoDatiLegge460(this, null);
                ValorizzaEtichette(this.areaDatiFondo);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseHidePulsanteSalva(this, null);
            RaiseTornaARegistrazioniFondo(this, null);
        }

        #region private methods
        private void LoadDdl(AreaDecodifica datiDecodifica, AreaTitolare.DatiPensione datiPensione)
        {
            List<AreaDecodifica.DatiCategoriaPensione> listaCategorie = datiDecodifica.ElencoCategoriePensione.ToList();
            List<string> listCategorieVisible = new List<string>();
            if (!String.IsNullOrEmpty(this.domanda.Categoria))
            {
                switch (this.domanda.Categoria.Substring(0, 1))
                {
                    case "V": //Se Vxx l’elenco di categorie visibili sarà: VO (001), VR (015), VOCOM (021), VOART (018)
                        listCategorieVisible.Add("0001");
                        listCategorieVisible.Add("0015");
                        listCategorieVisible.Add("0018");
                        listCategorieVisible.Add("0021");
                        break;
                    case "I": //Se Ixx l’elenco di categorie presente nella drop sarà: IO (002), IR (016), IOCOM (022), IOART  (019)
                        listCategorieVisible.Add("0001");
                        listCategorieVisible.Add("0002");
                        listCategorieVisible.Add("0016");
                        listCategorieVisible.Add("0019");
                        listCategorieVisible.Add("0022");
                        break;
                    case "S": //Se Sxx l’elenco di categorie presente nella drop sarà: SO (003), SR (017), SOCOM (023), SOART (020)
                        listCategorieVisible.Add("0001");
                        listCategorieVisible.Add("0003");
                        listCategorieVisible.Add("0017");
                        listCategorieVisible.Add("0020");
                        listCategorieVisible.Add("0023");
                        //ENG - Reversibilita 024
                        if (Utility.IsDomandaReversibilita(datiPensione) && (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                        {
                            listCategorieVisible.Add("0015");
                            listCategorieVisible.Add("0018");
                            listCategorieVisible.Add("0021");
                        }
                        break;
                }
            }

            listaCategorie.Sort((x, y) => String.CompareOrdinal(x.Codice, y.Codice));

            foreach (string categoriaVisible in listCategorieVisible)
            {
                foreach (AreaDecodifica.DatiCategoriaPensione categoria in listaCategorie)
                {
                    if (!String.IsNullOrEmpty(categoria.Codice))
                    {
                        if (categoria.Codice.Length == 4)
                        {
                            if (categoria.Codice.Length >= 1 && categoria.Codice.Substring(0, 1).Equals("0") && categoria.Codice.Equals(categoriaVisible) &&
                                !ddlCodiceCategoria.Items.Contains(new ListItem(categoria.Sigla, categoria.Codice.Substring(1))))
                            {
                                CodeUtility.SetValueDdl(ddlCodiceCategoria, categoria.Sigla, categoria.Codice.Substring(1));
                            }
                        }
                        else if (categoria.Codice.Length == 3)
                        {
                            if (categoria.Codice.Length >= 1 && categoria.Codice.Substring(0, 1).Equals("0") && categoria.Codice.Equals(categoriaVisible.Substring(categoriaVisible.Length - 3, 3)) &&
                                !ddlCodiceCategoria.Items.Contains(new ListItem(categoria.Sigla, categoria.Codice)))
                            {
                                CodeUtility.SetValueDdl(ddlCodiceCategoria, categoria.Sigla, categoria.Codice);
                            }
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
        }

        #endregion private methods

        #region Event Handlers
        public event EventHandler ShowAvviso;
        public event EventHandler UpdateSemaforoDatiLegge460;
        public event EventHandler HidePulsanteSalva;
        public event EventHandler TornaARegistrazioniFondo;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiLegge460(object sender, EventArgs e)
        {
            UpdateSemaforoDatiLegge460(sender, e);
        }

        protected void RaiseHidePulsanteSalva(object sender, EventArgs e)
        {
            HidePulsanteSalva(sender, e);
        }

        protected void RaiseTornaARegistrazioniFondo(object sender, EventArgs e)
        {
            TornaARegistrazioniFondo(sender, e);
        }
        #endregion Event Handlers

        enum EnumViewState
        {
            IdRecordFondo
        }
    }
}