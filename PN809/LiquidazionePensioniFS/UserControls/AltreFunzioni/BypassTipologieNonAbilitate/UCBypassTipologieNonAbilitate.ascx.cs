using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.BypassTipologieNonAbilitate
{
    public partial class UCBypassTipologieNonAbilitate : CustomBaseUserControl, IBypassTipologieNonAbilitate
    {
        #region ITipologieNonAbilitate
        public AreaCtrlBypassTipologieNonAbilitate AreaBypassTipologieNonAbilitate { get; set; }
        public AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate datiBypassTipologiaNonAbilitata { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        #endregion ITipologieNonAbilitate

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (ViewState["Filtro"] == null)
                ViewState["Filtro"] = false;
            if (!IsPostBack)
            {
                ValorizzaGriglia();
                AbilitaFiltro();
                txtFiltroTipoAppartenenza.Text = tipoAppRuolo.ToString();
                if (tipoAppRuolo == UtilityTipoAppartenenza.FS)
                {
                    pnlFondo.Visible = true;
                    gvTipologieNonAbilitate.Columns[1].Visible = true; // colonna corrispondente al Fondo
                }
                ValorizzaAutoComplete();
            }
        }

        protected void btnApplicaFiltro_Click(object sender, EventArgs e)
        {
            ViewState["Filtro"] = true;
            gvTipologieNonAbilitate.EditIndex = -1;
            ValorizzaGriglia();
            Filtra();
            gvTipologieNonAbilitate_Load();
            DisabilitaFiltro();
            RaiseHideInfo(this, null);
        }

        protected void btnAnnullaFiltro_Click(object sender, EventArgs e)
        {
            ViewState["Filtro"] = false;
            gvTipologieNonAbilitate.EditIndex = -1;
            ValorizzaGriglia();
            AbilitaFiltro();
            RaiseHideInfo(this, null);
        }

        private void Filtra()
        {
            int count = 0;
            List<BypassTipologieNonAbilitate> elencoTipologieNonAbilitate = (List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"];
            removeItemBlank(ref elencoTipologieNonAbilitate);
            if (!string.IsNullOrEmpty(txtFiltroTipoAppartenenza.Text.Trim()))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.TipoAppartenenza.Trim().ToUpperInvariant() == txtFiltroTipoAppartenenza.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtFiltroFondo.Text.Trim()))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Fondo.Trim().ToUpperInvariant() == txtFiltroFondo.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtFiltroGruppo.Text.Trim()))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Gruppo.Trim().ToUpperInvariant() == txtFiltroGruppo.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtFiltroProdotto.Text.Trim()))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Prodotto.Trim().ToUpperInvariant() == txtFiltroProdotto.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtFiltroTipo.Text.Trim()))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Tipo.Trim().ToUpperInvariant() == txtFiltroTipo.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtFiltroFiltro.Text))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Filtro.Trim().ToUpperInvariant() == txtFiltroFiltro.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtSede.Text))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Sede.Trim().ToUpperInvariant() == txtSede.Text.Trim().ToUpperInvariant());
            }
            if (!string.IsNullOrEmpty(txtCategoria.Text))
            {
                count++;
                elencoTipologieNonAbilitate = elencoTipologieNonAbilitate.FindAll(x => x.Categoria.Trim().ToUpperInvariant() == txtCategoria.Text.Trim().ToUpperInvariant());
            }
            if (count > 0)
            {
                ViewState["BypassTipologieNonAbilitate"] = elencoTipologieNonAbilitate;
                elencoTipologieNonAbilitate.Add(new BypassTipologieNonAbilitate());
                if (elencoTipologieNonAbilitate.Count() == 1)
                    gvTipologieNonAbilitate.EditIndex = 0;
            }
        }

        private void DisabilitaFiltro()
        {
            btnApplicaFiltro.Enabled = false;
            btnAnnullaFiltro.Enabled = true;
            txtFiltroFondo.Enabled = false;
            txtFiltroGruppo.Enabled = false;
            txtFiltroProdotto.Enabled = false;
            txtFiltroTipo.Enabled = false;
            txtFiltroFiltro.Enabled = false;
            txtCategoria.Enabled = false;
            txtSede.Enabled = false;
        }

        private void AbilitaFiltro()
        {
            PulisciFiltro();
            btnApplicaFiltro.Enabled = true;
            btnAnnullaFiltro.Enabled = false;
            txtFiltroFondo.Enabled = true;
            txtFiltroGruppo.Enabled = true;
            txtFiltroProdotto.Enabled = true;
            txtFiltroTipo.Enabled = true;
            txtFiltroFiltro.Enabled = true;
            txtCategoria.Enabled = true;
            txtSede.Enabled = true;
        }

        /// <summary>
        /// Ripulisce i campi del filtro di ricerca
        /// </summary>
        private void PulisciFiltro()
        {
            txtFiltroFondo.Text = string.Empty;
            txtFiltroGruppo.Text = string.Empty;
            txtFiltroProdotto.Text = string.Empty;
            txtFiltroTipo.Text = string.Empty;
            txtFiltroFiltro.Text = string.Empty;
            txtCategoria.Text = string.Empty;
            txtSede.Text = string.Empty;
        }

        private void removeItemBlank(ref List<BypassTipologieNonAbilitate> lista)
        {

            int index = lista.FindIndex(delegate (BypassTipologieNonAbilitate code)
            {
                return (string.IsNullOrEmpty(code.Filtro) && string.IsNullOrEmpty(code.Fondo) && string.IsNullOrEmpty(code.Gruppo) && string.IsNullOrEmpty(code.Prodotto) && string.IsNullOrEmpty(code.Tipo) &&
                    string.IsNullOrEmpty(code.TipoAppartenenza));
            }
                );

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private void ValorizzaAutoComplete()
        {
            if (tipoAppRuolo == UtilityTipoAppartenenza.FS)
            {
                string CommaSeparatedFondo;
                GetCommaSeparatedFondo(out CommaSeparatedFondo);
                HiddenFieldFondo.Value = CommaSeparatedFondo;
            }

            string CommaSeparatedGruppo;
            string CommaSeparatedDescGruppo;
            GetCommaSeparatedGruppo(out CommaSeparatedGruppo, out CommaSeparatedDescGruppo);
            HiddenFieldGruppo.Value = CommaSeparatedGruppo;
            HiddenFieldDescGruppo.Value = CommaSeparatedDescGruppo;

            string CommaSeparatedProdotto;
            string CommaSeparatedDescProdotto;
            GetCommaSeparatedProdotto(out CommaSeparatedProdotto, out CommaSeparatedDescProdotto);
            HiddenFieldProdotto.Value = CommaSeparatedProdotto;
            HiddenFieldDescProdotto.Value = CommaSeparatedDescProdotto;

            string CommaSeparatedTipo;
            string CommaSeparatedDescTipo;
            GetCommaSeparatedTipo(out CommaSeparatedTipo, out CommaSeparatedDescTipo);
            HiddenFieldTipo.Value = CommaSeparatedTipo;
            HiddenFieldDescTipo.Value = CommaSeparatedDescTipo;

            string CommaSeparatedFiltro;
            string CommaSeparatedDescFiltro;
            GetCommaSeparatedFiltro(out CommaSeparatedFiltro, out CommaSeparatedDescFiltro);
            HiddenFieldFiltro.Value = CommaSeparatedFiltro;
            HiddenFieldDescFiltro.Value = CommaSeparatedDescFiltro;

            string CommaSeparatedSede;
            string CommaSeparatedDescSede;
            GetCommaSeparatedSede(out CommaSeparatedSede, out CommaSeparatedDescSede);
            HiddenFieldSede.Value = CommaSeparatedSede;
            HiddenFieldDescSede.Value = CommaSeparatedDescSede;

            string CommaSeparatedCategoria;
            GetCommaSeparatedCategoria(out CommaSeparatedCategoria, tipoAppRuolo);
            HiddenFieldCategoria.Value = CommaSeparatedCategoria;
        }

        private void GetCommaSeparatedCategoria(out string CommaSeparatedCategoria, UtilityTipoAppartenenza tipologiAppartenza)
        {
            CommaSeparatedCategoria = string.Empty;

            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            List<string> listaCategoriePensioni = CodeUtility.GetCategoriePensione(valoriDecodificati, Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]));

            if (listaCategoriePensioni.Count > 0)
            {
                listaCategoriePensioni.Insert(0, "ALL");
                CommaSeparatedCategoria = listaCategoriePensioni.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Aggregate((s1, s2) => string.Concat(s1, ";", s2));
            }

        }

        private void GetCommaSeparatedSede(out string CommaSeparatedSede, out string CommaSeparatedDescSede)
        {
            CommaSeparatedSede = string.Empty;
            CommaSeparatedDescSede = string.Empty;
            List<Office> lstSedi = CodeUtility.GetListaSediProvinciali();
            if (lstSedi.Count > 0)
            {
                CommaSeparatedSede = lstSedi.Select(x => x.AspnCode.PadLeft(4, '0').Substring(0, 4)).Aggregate((s1, s2) => (string.Concat(s1, ";", s2)));
                CommaSeparatedDescSede = lstSedi.Select(x => x.ExtendedProperties != null ? x.ExtendedProperties["SEDE"].Trim() : x.Name.Trim()).Aggregate((s1, s2) => (string.Concat(s1, ";", s2)));
            }
        }

        private void GetCommaSeparatedFondo(out string CommaSeparatedFondo)
        {
            StringBuilder catBuilder = new StringBuilder();

            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            AreaDecodifica.DatiCategoriaPensione[] listaCategoriePensioni = valoriDecodificati.ElencoCategoriePensione;

            catBuilder.Append("ALL");
            foreach (AreaDecodifica.DatiCategoriaPensione categoria in listaCategoriePensioni)
            {
                if (categoria.Appartenenza == UtilityTipoAppartenenza.FS.ToString())
                {
                    if (!String.IsNullOrEmpty(categoria.Codice))
                    {
                        short codice = 0;
                        short.TryParse(categoria.Codice, out codice);
                        if (codice > 200)
                            continue;
                    }

                    if (!String.IsNullOrEmpty(categoria.Sigla))
                    {
                        if (categoria.Sigla.Substring(categoria.Sigla.Trim().Length - 2, 2) == "PT")
                            continue;
                        string nomeFiltro = categoria.Sigla.Trim();
                        catBuilder.Append(";");
                        catBuilder.Append(nomeFiltro);
                    }
                }
            }
            catBuilder.Append(";");
            catBuilder.Append("PT");
            catBuilder.Append(";");
            catBuilder.Append("INPDAP");
            CommaSeparatedFondo = catBuilder.ToString();
        }

        private void GetCommaSeparatedGruppo(out string CommaSeparatedGruppo, out string CommaSeparatedDescGruppo)
        {
            StringBuilder catBuilder = new StringBuilder();
            StringBuilder catBuilderDesc = new StringBuilder();
            CommaSeparatedGruppo = string.Empty;
            CommaSeparatedDescGruppo = string.Empty;

            if (AreaBypassTipologieNonAbilitate.ElencoGruppo != null)
            {
                foreach (GestioneDecodificaGruppo i in AreaBypassTipologieNonAbilitate.ElencoGruppo)
                {
                    // Codice e descrizione insieme sono troppo grandi per la textbox
                    string nomeGruppo = i.CodGruppo.Trim(); // string.Format("{0}-{1}", i.CodGruppo, i.DescGruppo);
                    string descGruppo = i.DescGruppo.Trim();
                    catBuilder.Append(nomeGruppo);
                    catBuilder.Append(";");
                    catBuilderDesc.Append(descGruppo);
                    catBuilderDesc.Append(";");
                }
                catBuilder.Remove(catBuilder.Length - 1, 1);
                catBuilderDesc.Remove(catBuilderDesc.Length - 1, 1);
                CommaSeparatedGruppo = catBuilder.ToString();
                CommaSeparatedDescGruppo = catBuilderDesc.ToString();
            }
        }

        private void GetCommaSeparatedProdotto(out string CommaSeparatedProdotto, out string CommaSeparatedDescProdotto)
        {
            StringBuilder catBuilder = new StringBuilder();
            StringBuilder catBuilderDesc = new StringBuilder();
            CommaSeparatedProdotto = string.Empty;
            CommaSeparatedDescProdotto = string.Empty;

            catBuilder.Append("ALL");
            catBuilderDesc.Append("ALL abilita tutti i Prodotti");

            if (AreaBypassTipologieNonAbilitate.ElencoProdotto != null)
            {
                foreach (GestioneDecodificaProdotto i in AreaBypassTipologieNonAbilitate.ElencoProdotto)
                {
                    // Codice e descrizione insieme sono troppo grandi per la textbox
                    string nomeProdotto = i.CodProdotto.Trim(); // string.Format("{0}-{1}", i.CodProdotto, i.DescProdotto);
                    string descProdotto = i.DescProdotto.Trim();
                    catBuilder.Append(";");
                    catBuilder.Append(nomeProdotto);
                    catBuilderDesc.Append(";");
                    catBuilderDesc.Append(descProdotto);
                }

                CommaSeparatedProdotto = catBuilder.ToString();
                CommaSeparatedDescProdotto = catBuilderDesc.ToString();
            }
        }

        private void GetCommaSeparatedTipo(out string CommaSeparatedTipo, out string CommaSeparatedDescTipo)
        {
            StringBuilder catBuilder = new StringBuilder();
            StringBuilder catBuilderDesc = new StringBuilder();
            CommaSeparatedTipo = string.Empty;
            CommaSeparatedDescTipo = string.Empty;

            catBuilder.Append("ALL");
            catBuilderDesc.Append("ALL abilita tutti i Tipi");

            if (AreaBypassTipologieNonAbilitate.ElencoTipo != null)
            {
                foreach (GestioneDecodificaTipo i in AreaBypassTipologieNonAbilitate.ElencoTipo)
                {
                    // Codice e descrizione insieme sono troppo grandi per la textbox
                    string nomeTipo = i.CodTipo.Trim(); // string.Format("{0}-{1}", i.CodTipo, i.DescTipo);
                    string descTipo = i.DescTipo.Trim();
                    catBuilder.Append(";");
                    catBuilder.Append(nomeTipo);
                    catBuilderDesc.Append(";");
                    catBuilderDesc.Append(descTipo);
                }

                CommaSeparatedTipo = catBuilder.ToString();
                CommaSeparatedDescTipo = catBuilderDesc.ToString();
            }
        }

        private void GetCommaSeparatedFiltro(out string CommaSeparatedFiltro, out string CommaSeparatedDescFiltro)
        {
            StringBuilder catBuilder = new StringBuilder();
            StringBuilder catBuilderDesc = new StringBuilder();
            CommaSeparatedFiltro = string.Empty;
            CommaSeparatedDescFiltro = string.Empty;

            catBuilder.Append("ALL");
            catBuilderDesc.Append("ALL abilita tutti i Filtri");

            if (AreaBypassTipologieNonAbilitate.ElencoFiltro != null)
            {
                foreach (GestioneDecodificaFiltro i in AreaBypassTipologieNonAbilitate.ElencoFiltro)
                {
                    if (i.Codice != null && i.Codice.Trim() != string.Empty)
                    {
                        // Codice e descrizione insieme sono troppo grandi per la textbox
                        string nomeFiltro = !string.IsNullOrEmpty(i.Codice) ? i.Codice : string.Empty; // string.Format("{0}-{1}", i.Codice, i.Descrizione);
                        string descFiltro = i.Descrizione.Trim();
                        catBuilder.Append(";");
                        catBuilder.Append(nomeFiltro);
                        catBuilderDesc.Append(";");
                        catBuilderDesc.Append(descFiltro);
                    }
                }

                CommaSeparatedFiltro = catBuilder.ToString();
                CommaSeparatedDescFiltro = catBuilderDesc.ToString();
            }
        }

        private void ValorizzaGriglia()
        {
            PresenterBypassTipologieNonAbilitate presenterTipologieNonAbilitate = new PresenterBypassTipologieNonAbilitate();
            presenterTipologieNonAbilitate.CaricaTipologieNonAbilitate(this);

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            FormattaElencoTipologieNonAbilitate();
            gvTipologieNonAbilitate_Load();
        }

        private void ValorizzaTipologiaNonAbilitataPerDelete(int index)
        {
            List<BypassTipologieNonAbilitate> elencoTipologieNonAbilitate = (List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"];

            ValorizzaTipologieNonAbilitate(elencoTipologieNonAbilitate, index);
        }

        private void ValorizzaBypassTipologiaNonAbilitataPerSave(GridViewRow row)
        {
            List<BypassTipologieNonAbilitate> elencoTipologieNonAbilitate = (List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"];

            elencoTipologieNonAbilitate[row.DataItemIndex].TipoAppartenenza = this.tipoAppRuolo.ToString();

            if (this.tipoAppRuolo != UtilityTipoAppartenenza.FS)
                elencoTipologieNonAbilitate[row.DataItemIndex].Fondo = null;
            else
                elencoTipologieNonAbilitate[row.DataItemIndex].Fondo = ((TextBox)row.Cells[2].Controls[1]).Text.ToUpperInvariant();

            elencoTipologieNonAbilitate[row.DataItemIndex].Gruppo = ((TextBox)row.Cells[3].Controls[1]).Text.ToUpperInvariant();
            elencoTipologieNonAbilitate[row.DataItemIndex].Prodotto = ((TextBox)row.Cells[4].Controls[1]).Text.ToUpperInvariant();
            elencoTipologieNonAbilitate[row.DataItemIndex].Tipo = ((TextBox)row.Cells[5].Controls[1]).Text.ToUpperInvariant();
            elencoTipologieNonAbilitate[row.DataItemIndex].Filtro = ((TextBox)row.Cells[6].Controls[1]).Text.ToUpperInvariant().PadRight(3, ' ');

            elencoTipologieNonAbilitate[row.DataItemIndex].Sede = ((TextBox)row.FindControl("txtSede")).Text.ToUpperInvariant().PadLeft(4, '0');
            elencoTipologieNonAbilitate[row.DataItemIndex].Categoria = ((TextBox)row.FindControl("txtCategoria")).Text.ToUpperInvariant();

            ValorizzaTipologieNonAbilitate(elencoTipologieNonAbilitate, row.DataItemIndex);

        }

        private void ValorizzaTipologieNonAbilitate(List<BypassTipologieNonAbilitate> elencoTipologieNonAbilitate, int index)
        {
            this.datiBypassTipologiaNonAbilitata = new AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate();

            datiBypassTipologiaNonAbilitata.Tipologia = this.tipoAppRuolo.ToString();
            datiBypassTipologiaNonAbilitata.Fondo = elencoTipologieNonAbilitate[index].Fondo;
            datiBypassTipologiaNonAbilitata.Gruppo = elencoTipologieNonAbilitate[index].Gruppo;
            datiBypassTipologiaNonAbilitata.Prodotto = elencoTipologieNonAbilitate[index].Prodotto;
            datiBypassTipologiaNonAbilitata.Tipo = elencoTipologieNonAbilitate[index].Tipo;
            datiBypassTipologiaNonAbilitata.Filtro = elencoTipologieNonAbilitate[index].Filtro;
            datiBypassTipologiaNonAbilitata.Categoria = elencoTipologieNonAbilitate[index].Categoria;
            datiBypassTipologiaNonAbilitata.Sede = short.Parse(elencoTipologieNonAbilitate[index].Sede);
        }

        private void FormattaElencoTipologieNonAbilitate()
        {
            List<BypassTipologieNonAbilitate> elencoTipologieNonAbilitate = new List<BypassTipologieNonAbilitate>();
            if (this.AreaBypassTipologieNonAbilitate.ElencoCtrlBypassTipologieNonAbilitate == null)
            {
                elencoTipologieNonAbilitate.Add(new BypassTipologieNonAbilitate());
            }
            else
            {
                foreach (var la in this.AreaBypassTipologieNonAbilitate.ElencoCtrlBypassTipologieNonAbilitate)
                {
                    BypassTipologieNonAbilitate l = new BypassTipologieNonAbilitate();
                    l.TipoAppartenenza = la.Tipologia;
                    l.Fondo = !string.IsNullOrEmpty(la.Fondo) ? la.Fondo.Trim() : null;
                    l.Gruppo = la.Gruppo;
                    l.Prodotto = la.Prodotto;
                    l.Tipo = la.Tipo;
                    l.Filtro = la.Filtro;
                    l.Categoria = la.Categoria;
                    l.Sede = la.Sede.ToString().PadLeft(4, '0');
                    elencoTipologieNonAbilitate.Add(l);
                }
                elencoTipologieNonAbilitate.Add(new BypassTipologieNonAbilitate());
            }

            if (elencoTipologieNonAbilitate.Count() < 2)
                gvTipologieNonAbilitate.EditIndex = 0;

            ViewState["BypassTipologieNonAbilitate"] = elencoTipologieNonAbilitate;
        }

        private void gvTipologieNonAbilitate_Load()
        {
            try
            {
                gvTipologieNonAbilitate.DataSource = (List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"];
                gvTipologieNonAbilitate.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTipologieNonAbilitate, Errore nel metodo gvTipologieNonAbilitate_Load " + ex);
            }
        }

        private void InitItemBlank(GridViewRowEventArgs e)
        {
            TextBox txtTipoAppartenenza = new TextBox();
            txtTipoAppartenenza = (TextBox)e.Row.FindControl("txtTipoAppartenenza");
            txtTipoAppartenenza.Text = tipoAppRuolo.ToString();// ((TipNonAbilitate)e.Row.DataItem).TipoAppartenenza;

            //Abilito il validatore per il fondo solamente se sono un tipoApp FS
            RequiredFieldValidator rfv = (RequiredFieldValidator)e.Row.FindControl("RequiredFieldValidatorTxtFondo");
            if (tipoAppRuolo == UtilityTipoAppartenenza.FS)
                rfv.Enabled = true;


            if (!string.IsNullOrEmpty(txtTipoAppartenenza.Text))
                txtTipoAppartenenza.Enabled = false;

            TextBox txtFondo = new TextBox();
            txtFondo = (TextBox)e.Row.FindControl("txtFondo");
            txtFondo.Text = ((BypassTipologieNonAbilitate)e.Row.DataItem).Fondo;

            TextBox txtGruppo = new TextBox();
            txtGruppo = (TextBox)e.Row.FindControl("txtGruppo");
            txtGruppo.Text = ((BypassTipologieNonAbilitate)e.Row.DataItem).Gruppo;

            TextBox txtProdotto = new TextBox();
            txtProdotto = (TextBox)e.Row.FindControl("txtProdotto");
            txtProdotto.Text = ((BypassTipologieNonAbilitate)e.Row.DataItem).Prodotto;

            TextBox txtTipo = new TextBox();
            txtTipo = (TextBox)e.Row.FindControl("txtTipo");
            txtTipo.Text = ((BypassTipologieNonAbilitate)e.Row.DataItem).Tipo;

            TextBox txtFiltro = new TextBox();
            txtFiltro = (TextBox)e.Row.FindControl("txtFiltro");
            txtFiltro.Text = ((BypassTipologieNonAbilitate)e.Row.DataItem).Filtro;
        }

        protected void gvTipologieNonAbilitate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterBypassTipologieNonAbilitate presenterTipologieNonAbilitate = new PresenterBypassTipologieNonAbilitate();
                ValorizzaTipologiaNonAbilitataPerDelete(r.DataItemIndex);
                presenterTipologieNonAbilitate.EliminaTipologiaNonAbilitata(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Tipologia Domanda riabilitata correttamente";
                    RaiseShowAvviso(this, null);
                }

                ValorizzaGriglia();

                if ((bool)ViewState["Filtro"])
                {
                    Filtra();
                    gvTipologieNonAbilitate_Load();
                    DisabilitaFiltro();
                }
                else
                {
                    AbilitaFiltro();
                }
            }
            else if (e.CommandName == "Edit")
            {
                RaiseHideInfo(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;

                PresenterBypassTipologieNonAbilitate presenterTipologieNonAbilitate = new PresenterBypassTipologieNonAbilitate();
                ValorizzaBypassTipologiaNonAbilitataPerSave(r);
                if (this.HasError)
                    return;

                presenterTipologieNonAbilitate.SalvaTipologiaNonAbilitata(this);

                if (this.HasError)
                {
                    RaiseShowAvviso(this, null);
                    return;
                }
                else
                {
                    this.ErrorMessage = "Bypass per Tipologia Non Abilitata creato correttamente";
                    RaiseShowAvviso(this, null);
                }

                gvTipologieNonAbilitate.EditIndex = -1;

                ValorizzaGriglia();
                AbilitaFiltro();
                ViewState["Filtro"] = false;
            }
            else if (e.CommandName == "Cancel")
            {
                GridView r = (GridView)e.CommandSource;
                int index = r.EditIndex + (r.PageIndex * r.PageSize);
                if (index == ((List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"]).Count - 1)
                    ((List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"])[index] = new BypassTipologieNonAbilitate();

                RaiseHideInfo(this, null);
            }
        }

        protected void gvTipologieNonAbilitate_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                // Reset the edit index.
                if (((List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"]).Count() < 2)
                    gvTipologieNonAbilitate.EditIndex = 0;
                else
                    gvTipologieNonAbilitate.EditIndex = -1;
                // Bind data to the GridView control.
                gvTipologieNonAbilitate_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTipologieNonAbilitate, Errore nel metodo gvTipologieNonAbilitate_RowCancelingEdit " + ex);
            }
        }

        protected void gvTipologieNonAbilitate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<BypassTipologieNonAbilitate> elencoTipologieNonAbilitate = (List<BypassTipologieNonAbilitate>)ViewState["BypassTipologieNonAbilitate"];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";
                        cancel.OnClientClick = "BlockUI();";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCTipologieNonAbilitate";
                        save.CommandName = "Salva";
                        save.OnClientClick = "if(validatePageGrid()){aspnetForm.target ='_self'; BlockUI();}";

                        InitItemBlank(e);
                    }
                    else
                    {
                        if (e.Row.DataItemIndex == elencoTipologieNonAbilitate.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            edit.Visible = false;
                            LinkButton delete = (LinkButton)e.Row.FindControl("btnDelete");
                            int index = e.Row.DataItemIndex;
                            if (index >= 0 && index <= elencoTipologieNonAbilitate.Count - 2)
                            {
                                delete.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
                                delete.ToolTip = "Elimina";
                                delete.OnClientClick = "BlockUI();";
                            }
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTipologieNonAbilitate, Errore nel metodo gvTipologieNonAbilitate_RowDataBound " + ex);
            }
        }

        protected void gvTipologieNonAbilitate_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvTipologieNonAbilitate.EditIndex = e.NewEditIndex;
                gvTipologieNonAbilitate_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTipologieNonAbilitate, Errore nel metodo gvTipologieNonAbilitate_RowEditing " + ex);
            }
        }

        protected void gvTipologieNonAbilitate_onPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvTipologieNonAbilitate.EditIndex = -1;
                gvTipologieNonAbilitate.PageIndex = e.NewPageIndex;
                gvTipologieNonAbilitate_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCTipologieNonAbilitate, Errore nel metodo gvTipologieNonAbilitate_onPageIndexChanging" + ex);
            }
        }

        protected void gvTipologieNonAbilitate_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        #region Event Handlers
        public event EventHandler HideInfo;
        public event EventHandler ShowAvviso;
        //public event EventHandler ShowInfo;

        protected void RaiseHideInfo(object sender, EventArgs e)
        {
            HideInfo(sender, e);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        //protected void RaiseShowInfo(object sender, EventArgs e)
        //{
        //    ShowInfo(sender, e);
        //}

        #endregion Event Handlers

        [Serializable()]
        public class BypassTipologieNonAbilitate
        {
            internal BypassTipologieNonAbilitate() { }

            #region private properties
            private string _TipoAppartenenza;
            private string _Fondo;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            private string _Filtro;
            private string _Categoria;
            private string _Sede;
            #endregion private properties

            #region public properties
            public string TipoAppartenenza { get { return _TipoAppartenenza; } set { _TipoAppartenenza = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public string Filtro { get { return _Filtro; } set { _Filtro = value; } }
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            #endregion public properties
        }
    }
}
