using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione
{
    public partial class UCLegge460 : CustomBaseUserControl, ILiquidazionePensione
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
        }

        protected void btnSalvaDatiLegge460_Click(Object sender, EventArgs e)
        {
            areaLiquidazionePensioneFS = new AreaLiquidazionePensione();
            areaLiquidazionePensioneFS.DatiLegge460 = GetDatiLegge460();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiLegge460(this);

            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvviso(this, Cevent);
        }

        protected void btnEliminaDatiLegge460_Click(Object sender, EventArgs e)
        {
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiLegge460(this);
            if (!this.HasError)
            {
                ClearForm();
                ValorizzaEtichetteDatiLegge460(this);
            }
            Utility.CustomEventArgs Cevent = new Utility.CustomEventArgs(null, this.domanda.Tipofondo.Value);
            RaiseShowAvvisoElimina(this, Cevent);
        }

        internal void ValorizzaEtichetteDatiLegge460(ILiquidazionePensione liquidazione)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            HiddenFieldSedi.Value = CodeUtility.LoadSedi();

            CodeUtility areaDecodifica = new CodeUtility();
            AreaDecodifica datiDecodifica = areaDecodifica.GetValuesDecodifica();

            LoadDdl(datiDecodifica);

            if (liquidazione != null && liquidazione.areaLiquidazionePensioneFS != null && liquidazione.areaLiquidazionePensioneFS.DatiLegge460 != null)
            {
                if (liquidazione.areaLiquidazionePensioneFS.DatiLegge460.SiglaCategoria.HasValue)
                    ddlCodiceCategoria.SelectedValue = liquidazione.areaLiquidazionePensioneFS.DatiLegge460.SiglaCategoria.ToString().PadLeft(3, '0');

                if (liquidazione.areaLiquidazionePensioneFS.DatiLegge460.CodiceSede.HasValue)
                    txtSede.Text = CodeUtility.GetSede(liquidazione.areaLiquidazionePensioneFS.DatiLegge460.CodiceSede.ToString().PadLeft(4, '0'));

                if (!string.IsNullOrEmpty(liquidazione.areaLiquidazionePensioneFS.DatiLegge460.NCertificato))
                    txtCertificato.Text = liquidazione.areaLiquidazionePensioneFS.DatiLegge460.NCertificato.PadLeft(8, '0');

                if (liquidazione.areaLiquidazionePensioneFS.DatiLegge460.NMesiRiscattati.HasValue)
                    txtnumMesiRiscatti.Text = liquidazione.areaLiquidazionePensioneFS.DatiLegge460.NMesiRiscattati.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiLegge460.NMesiTotali.HasValue)
                    txtNumMesiTotali.Text = liquidazione.areaLiquidazionePensioneFS.DatiLegge460.NMesiTotali.Value.ToString();

                if (liquidazione.areaLiquidazionePensioneFS.DatiLegge460.DecorrenzaSecondaria.HasValue)
                    txtDecorrenzaSecondaria.Text = String.Format("{0:dd/MM/yyyy}", liquidazione.areaLiquidazionePensioneFS.DatiLegge460.DecorrenzaSecondaria.Value);
            }
        }

        internal DatiLegge460 GetDatiLegge460()
        {
            DatiLegge460 datiLegge460 = new DatiLegge460();

            datiLegge460.SiglaCategoria = ddlCodiceCategoria.SelectedIndex != 0 ? Convert.ToInt32(ddlCodiceCategoria.SelectedValue) : (Int32?)null;
            datiLegge460.CodiceSede = !string.IsNullOrEmpty(txtSede.Text) ? CodeUtility.ControlSede(txtSede.Text) : (short?)null;
            datiLegge460.NCertificato = txtCertificato.Text;
            datiLegge460.NMesiRiscattati = (!string.IsNullOrEmpty(txtnumMesiRiscatti.Text)) ? Convert.ToInt32(txtnumMesiRiscatti.Text) : (Int32?)null;
            datiLegge460.NMesiTotali = (!string.IsNullOrEmpty(txtNumMesiTotali.Text)) ? Convert.ToInt32(txtNumMesiTotali.Text) : (Int32?)null;
            datiLegge460.DecorrenzaSecondaria = (!string.IsNullOrEmpty(txtDecorrenzaSecondaria.Text) && txtDecorrenzaSecondaria.Text.ToUpperInvariant() != "GG/MM/AAAA") ? Convert.ToDateTime(txtDecorrenzaSecondaria.Text) : (DateTime?)null;

            return datiLegge460;
        }

        private void LoadDdl(AreaDecodifica datiDecodifica)
        {
            List<AreaDecodifica.DatiCategoriaPensione> listaCategorie = datiDecodifica.ElencoCategoriePensione.ToList();
            List<string> listCategorieVisible = new List<string>();
            if (!String.IsNullOrEmpty(this.domanda.Categoria))
            {
                switch (this.domanda.Categoria.Substring(0, 3))
                {
                    case "VPT": //Se VPT l’elenco di categorie visibili sarà: VO (001), VR (015), VOCOM (021), VOART (018)
                        listCategorieVisible.Add("0001");
                        listCategorieVisible.Add("0015");
                        listCategorieVisible.Add("0018");
                        listCategorieVisible.Add("0021");
                        break;
                    case "IPT": //Se IPT l’elenco di categorie presente nella drop sarà: IO (002), IR (016), IOCOM (022), IOART  (019)
                        listCategorieVisible.Add("0002");
                        listCategorieVisible.Add("0016");
                        listCategorieVisible.Add("0019");
                        listCategorieVisible.Add("0022");
                        break;
                    case "SPT": //Se SPT l’elenco di categorie presente nella drop sarà: SO (003), SR (017), SOCOM (023), SOART (020)
                        listCategorieVisible.Add("0003");
                        listCategorieVisible.Add("0017");
                        listCategorieVisible.Add("0020");
                        listCategorieVisible.Add("0023");
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
                        if (categoria.Codice.Length >= 1 && categoria.Codice.Substring(0, 1).Equals("0") && categoria.Codice.Equals(categoriaVisible) &&
                            !ddlCodiceCategoria.Items.Contains(new ListItem(categoria.Sigla, categoria.Codice.Substring(1))))
                        {
                            CodeUtility.SetValueDdl(ddlCodiceCategoria, categoria.Sigla, categoria.Codice.Substring(1));
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            CodeUtility.ClearForm(this, 0);
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {

        }

        #region ILiquidazionePensione Members
        public AreaLiquidazionePensione areaLiquidazionePensioneFS { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensione Members

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        #region EventHandlers
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

        #endregion EventHandlers
    }
}