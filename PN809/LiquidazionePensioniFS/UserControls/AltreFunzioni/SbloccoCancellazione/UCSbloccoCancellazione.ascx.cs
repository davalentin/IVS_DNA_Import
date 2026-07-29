using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.SbloccoCancellazione
{
    public partial class UCSbloccoCancellazione : CustomBaseUserControl, ISbloccoCancellazione, ISedi
    {
        #region ISbloccoCancellazione
        public AreaSbloccoCancellazione areaSbloccoCancellazione { get; set; }
        public AreaEsito.TipoEsito Esito { get; set; }
        #endregion ISbloccoCancellazione

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
            if (!IsPostBack)
            {
                PresenterSedi presenter = new PresenterSedi();
                presenter.GetCommaSeparatedSedi(this);

                CodeUtility valuesDecodifica = new CodeUtility();
                AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
                HiddenFieldSedi.Value = CommaSeparatedSedi;
                LoadCategoriePensione(valoriDecodificati);
                LoadTipoOperazione();
            }
        }

        protected void btnSbloccoCancellazione_Click(object sender, EventArgs e)
        {
            string sedeSelezionata = string.Empty;
            if (!ControlSedeSelezionata(out sedeSelezionata))
            {
                RaiseShowAvviso(this, null);
                return;
            }

            this.areaSbloccoCancellazione = GetDatiSbloccoCancellazione(sedeSelezionata);
            UtilityTipoOperazione? tipoOperatione = this.areaSbloccoCancellazione.TipoOperazione;

            PresenterSbloccoCancellazione presenterSbloccoCancellazione = new PresenterSbloccoCancellazione();
            presenterSbloccoCancellazione.SbloccoCancellazione(this);

            GestioneEsitoSvc(tipoOperatione);
        }

        private void GestioneEsitoSvc(UtilityTipoOperazione? tipoOperazione)
        {
            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }
            else
            {
                if (tipoOperazione == UtilityTipoOperazione.INSERIMENTO)
                    this.ErrorMessage = "Inserimento domanda da sbloccare effettuato correttamente";

                if (tipoOperazione == UtilityTipoOperazione.CANCELLAZIONE)
                    this.ErrorMessage = "Cancellazione domanda da sbloccare effettuata correttamente";

                ClearField();
                RaiseShowAvviso(this, null);
            }
        }

        private void ClearField()
        {
            txtNumeroDomanda.Text = string.Empty;
            txtSede.Text = string.Empty;
            ddlCategoriaPensione.SelectedIndex = 0;
            ddlTipoOperazione.SelectedIndex = 0;
        }

        private bool ControlSedeSelezionata(out string sedeSelezionata)
        {
            sedeSelezionata = string.Empty;
            this.Sede = txtSede.Text.ToUpperInvariant().Substring(txtSede.Text.IndexOf('-') + 1).Trim();

            PresenterSedi presenter = new PresenterSedi();
            presenter.GetOffice(this);
            if (SelectedOffice == null)
            {
                this.HasError = true;
                this.ErrorMessage = "La sede selezionata non è valida";
                return false;
            }
            sedeSelezionata = SelectedOffice.AspnCode;
            return true;
        }

        private AreaSbloccoCancellazione GetDatiSbloccoCancellazione(string sedeSelezionata)
        {
            this.areaSbloccoCancellazione = new AreaSbloccoCancellazione();

            string[] sedeToSplit = sedeSelezionata.Split('-');
            long nDomus = 0;
            long.TryParse(txtNumeroDomanda.Text, out nDomus);

            areaSbloccoCancellazione.NumeroDomanda = nDomus;
            areaSbloccoCancellazione.CodiceSede = short.Parse(sedeToSplit[0].PadLeft(4, '0').Substring(0, 4));
            areaSbloccoCancellazione.CentroOperativo = byte.Parse(sedeToSplit[0].PadRight(2, '0').Substring(4, 2));
            areaSbloccoCancellazione.SiglaCategoria = ddlCategoriaPensione.SelectedValue;

            switch (ddlTipoOperazione.SelectedValue)
            {
                case "INSERIMENTO":
                    areaSbloccoCancellazione.TipoOperazione = UtilityTipoOperazione.INSERIMENTO;
                    break;
                case "CANCELLAZIONE":
                    areaSbloccoCancellazione.TipoOperazione = UtilityTipoOperazione.CANCELLAZIONE;
                    break;
            }

            return areaSbloccoCancellazione;
        }

        private void LoadTipoOperazione()
        {
            CodeUtility.SetValueDdl(ddlTipoOperazione, UtilityTipoOperazione.INSERIMENTO.ToString(), UtilityTipoOperazione.INSERIMENTO.ToString());
            CodeUtility.SetValueDdl(ddlTipoOperazione, UtilityTipoOperazione.CANCELLAZIONE.ToString(), UtilityTipoOperazione.CANCELLAZIONE.ToString());
        }

        private void LoadCategoriePensione(AreaDecodifica valoriDecodificati)
        {
            AreaDecodifica.DatiCategoriaPensione[] listaCategoriePensioni = valoriDecodificati.ElencoCategoriePensione;

            List<string> listaCatAmmesse = new List<string>();
            foreach (AreaDecodifica.DatiCategoriaPensione categoria in listaCategoriePensioni)
            {
                if (!String.IsNullOrEmpty(categoria.Sigla))
                    if (categoria.Appartenenza != Utility.GetDescription(Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"])))
                        continue;
                string codiceCategoria = categoria.Codice;
                int codice;
                categoria.Sigla = categoria.Sigla.Trim();
                Int32.TryParse(codiceCategoria.Trim(), out codice);
                if (codice < 99 || (codice > 200 && codice < 207) || (codice > 212 && codice < 243) || codice >= 170 && codice <= 172 || codice == 198 || codice == 199 || codice == 127 || codice == 128 ||
                    codice == 143 || codice == 243 || codice == 244 || codice == 245)
                {
                    switch (categoria.Sigla)
                    {
                        case "EL":
                        case "VL":
                        case "ES":
                        case "GAS":
                        case "FS":
                        case "ET":
                        case "TT":
                        case "DZ":
                        case "CL":
                        case "PM":
                        case "PL":
                            listaCatAmmesse.Add("V" + categoria.Sigla);
                            listaCatAmmesse.Add("I" + categoria.Sigla);
                            listaCatAmmesse.Add("S" + categoria.Sigla);
                            break;
                        case "PI":
                            listaCatAmmesse.Add("V" + categoria.Sigla + "A");
                            listaCatAmmesse.Add("I" + categoria.Sigla + "A");
                            listaCatAmmesse.Add("S" + categoria.Sigla + "A");
                            break;
                        case "PMS":
                        case "PMO":
                            break;
                        default:
                            listaCatAmmesse.Add(categoria.Sigla);
                            break;
                    }
                }
            }
            if (listaCatAmmesse.Count > 0)
            {
                listaCatAmmesse.Sort((x, y) => string.Compare(x, y, false, CultureInfo.InvariantCulture));

                foreach (string categoria in listaCatAmmesse)
                {
                    if (ddlCategoriaPensione.Items == null || !ddlCategoriaPensione.Items.Contains(new ListItem(categoria, categoria)))
                        CodeUtility.SetValueDdl(ddlCategoriaPensione, categoria, categoria);
                }
            }
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;
    }
}