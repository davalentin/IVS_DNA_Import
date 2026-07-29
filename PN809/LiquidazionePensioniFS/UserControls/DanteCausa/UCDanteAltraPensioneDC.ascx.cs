using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Web.UI;

using INPS.DNA.UI.Web;
using INPS.DNA.Logging;
using INPS.DNA.Exceptions;
using INPS.DNA;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa
{
    public partial class UCDanteAltraPensioneDC : CustomBaseUserControl, IDanteCausa
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDanteCausa
        public long numDomanda { get; set; }
        public Presenter.SvrLiquidazione.AreaDanteCausa areaDanteCausa { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDanteCausa


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaControlliAltraPensione(IDanteCausa danteCausa)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            RenderPanel();
            RenderControlsDdl(this.domanda.TipoAppartenenza, danteCausa);
            if (danteCausa.areaDanteCausa.AltraPensioneDC != null)
            {
                if (danteCausa.areaDanteCausa.AltraPensioneDC.CessazioneAltraPensione.HasValue)
                    this.txtCessazione.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.AltraPensioneDC.CessazioneAltraPensione.Value);
                if (danteCausa.areaDanteCausa.AltraPensioneDC.DecorrenzaAltraPensione.HasValue)
                    this.txtDecorrenza.Text = String.Format("{0:MM/yyyy}", danteCausa.areaDanteCausa.AltraPensioneDC.DecorrenzaAltraPensione.Value);
                if (danteCausa.areaDanteCausa.AltraPensioneDC.EnteAltraPensione.HasValue)
                    this.txtEnte.Text = danteCausa.areaDanteCausa.AltraPensioneDC.EnteAltraPensione.Value.ToString();
                if (danteCausa.areaDanteCausa.AltraPensioneDC.CodiceImportoAltraPensione.HasValue)
                    this.ddlCodiceImporto.Items.FindByValue(danteCausa.areaDanteCausa.AltraPensioneDC.CodiceImportoAltraPensione.Value.ToString()).Selected = true;
                if (!string.IsNullOrEmpty(danteCausa.areaDanteCausa.AltraPensioneDC.CategoriaAltraPensione))
                {
                    short resShort = 0;
                    short.TryParse(danteCausa.areaDanteCausa.AltraPensioneDC.CategoriaAltraPensione, out resShort);
                    string value = resShort != 0 ? resShort.ToString().PadLeft(3, '0') : danteCausa.areaDanteCausa.AltraPensioneDC.CategoriaAltraPensione.Trim();
                    if(this.ddlCategoriaPensione.Items.FindByValue(value) != null) this.ddlCategoriaPensione.Items.FindByValue(value).Selected = true ;
                }
                if (danteCausa.areaDanteCausa.AltraPensioneDC.CodiceUCAltraPensione.HasValue)
                    this.ddlCodiceUC.Items.FindByValue(danteCausa.areaDanteCausa.AltraPensioneDC.CodiceUCAltraPensione.Value.ToString()).Selected = true;

                if (!String.IsNullOrEmpty(danteCausa.areaDanteCausa.AltraPensioneDC.NaturaPensioneAltraPensione))
                {
                    this.ddlCodNatura1.Items.FindByValue(danteCausa.areaDanteCausa.AltraPensioneDC.NaturaPensioneAltraPensione.Substring(0, 1)).Selected = true;
                    this.ddlCodNatura2.Items.FindByValue(danteCausa.areaDanteCausa.AltraPensioneDC.NaturaPensioneAltraPensione.Substring(1, 1)).Selected = true;
                    this.ddlCodNatura3.Items.FindByValue(danteCausa.areaDanteCausa.AltraPensioneDC.NaturaPensioneAltraPensione.Substring(2, 1)).Selected = true;
                }
            }
            else
                danteCausa.areaDanteCausa.AltraPensioneDC = new AltraPensioneDC();
            ViewState["AreaDC"] = danteCausa.areaDanteCausa;
        }

        internal AltraPensioneDC GetValoriAltraPensioneDC()
        {
            AreaDanteCausa areaDC = (AreaDanteCausa)ViewState["AreaDC"];
            if (areaDC != null && areaDC.AltraPensioneDC != null)
            {
                if (!String.IsNullOrEmpty(this.txtCessazione.Text))
                {
                    try
                    {
                        areaDC.AltraPensioneDC.CessazioneAltraPensione = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtCessazione.Text)));
                    }
                    catch (Exception)
                    {
                        areaDC.AltraPensioneDC.CessazioneAltraPensione = null;
                    }
                }
                else
                    areaDC.AltraPensioneDC.CessazioneAltraPensione = null;

                if (!String.IsNullOrEmpty(this.txtDecorrenza.Text))
                {
                    try
                    {
                        areaDC.AltraPensioneDC.DecorrenzaAltraPensione = Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(this.txtDecorrenza.Text)));
                    }
                    catch (Exception)
                    {
                        areaDC.AltraPensioneDC.DecorrenzaAltraPensione = null;
                    }
                }
                else
                    areaDC.AltraPensioneDC.DecorrenzaAltraPensione = null;

                if (!String.IsNullOrEmpty(this.txtEnte.Text))
                    areaDC.AltraPensioneDC.EnteAltraPensione = short.Parse(txtEnte.Text);
                else
                    areaDC.AltraPensioneDC.EnteAltraPensione = null;

                if (!String.IsNullOrEmpty(this.ddlCodiceImporto.SelectedValue))
                    areaDC.AltraPensioneDC.CodiceImportoAltraPensione = Convert.ToChar(this.ddlCodiceImporto.SelectedValue);
                else
                    areaDC.AltraPensioneDC.CodiceImportoAltraPensione = null;

                if (!String.IsNullOrEmpty(this.ddlCategoriaPensione.SelectedValue))
                {
                    short resShort = 0;
                    short.TryParse(this.ddlCategoriaPensione.SelectedValue, out resShort);
                    areaDC.AltraPensioneDC.CategoriaAltraPensione = resShort != 0 ? resShort.ToString() : this.ddlCategoriaPensione.SelectedValue.Trim();
                }
                else
                    areaDC.AltraPensioneDC.CategoriaAltraPensione = null;

                if (!String.IsNullOrEmpty(this.ddlCodiceUC.SelectedValue))
                    areaDC.AltraPensioneDC.CodiceUCAltraPensione = Convert.ToChar(this.ddlCodiceUC.SelectedValue);
                else
                    areaDC.AltraPensioneDC.CodiceUCAltraPensione = null;

                areaDC.AltraPensioneDC.NaturaPensioneAltraPensione = String.Concat(new string[] { this.ddlCodNatura1.SelectedValue, this.ddlCodNatura2.SelectedValue, this.ddlCodNatura3.SelectedValue });
            }
            else
                areaDC = new AreaDanteCausa();


            return areaDC.AltraPensioneDC;
        }

        private void RenderControlsDdl(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? tipoAppartenenza, IDanteCausa danteCausa)
        {
            try
            {
                LoadDdlCodiciImportoAltraPensione();
                LoadDdlCategoriaPensione(tipoAppartenenza);
                LoadDdlU_C();
                LoadDdlCodeNaturaAGO(danteCausa);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteAltraPensione, Errore nel metodo RenderControlsDdl: " + ex);
            }
        }

        private void RenderPanel()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO)
            {
                pnlNaturaPensione.Visible = true;
            }
        }

        private void LoadDdlU_C()
        {
            this.ddlCodiceUC.Items.Add(new ListItem(string.Empty, string.Empty));
            this.ddlCodiceUC.Items.Add(new ListItem("U", "U"));
            this.ddlCodiceUC.Items.Add(new ListItem("C", "C"));
        }

        private void LoadDdlCategoriaPensione(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? tipoAppartenenza)
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            List<AreaDecodifica.DatiCategoriaPensione> listaCategoriePensioni = valoriDecodificati.ElencoCategoriePensione.ToList();
            List<AreaDecodifica.DatiCategoriaAltraPensione> listaCategorieAltraPensione = valoriDecodificati.ElencoCategorieAltraPensione.ToList();
            GestioneCategoriePensioni(tipoAppartenenza, listaCategoriePensioni, listaCategorieAltraPensione);
        }

        private void LoadDdlCodiciImportoAltraPensione()
        {
            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
            AreaDecodifica.DatiCodiciImportoAltraPensione[] listaCodiciImportoAltraPensione = valoriDecodificati.ElencoCodiciImportoAltraPensione;

            foreach (AreaDecodifica.DatiCodiciImportoAltraPensione importoAltraPensione in listaCodiciImportoAltraPensione)
                this.ddlCodiceImporto.Items.Add(new ListItem(importoAltraPensione.Descrizione, importoAltraPensione.Id));
        }

        private void LoadDdlCodeNaturaAGO(IDanteCausa danteCausa)
        {
            this.ddlCodNatura1.Items.Clear();
            this.ddlCodNatura2.Items.Clear();
            this.ddlCodNatura3.Items.Clear();
            List<CodiciNatura> listCodiciNatura = danteCausa.areaDanteCausa.ElencoCodiciNatura.ToList();
            CodeUtility.SetValueDdl(ddlCodNatura2, string.Empty, string.Empty, " ");
            CodeUtility.SetValueDdl(ddlCodNatura3, string.Empty, string.Empty, " ");
            foreach (CodiciNatura codiceNatura in listCodiciNatura)
            {
                switch (codiceNatura.Posizione)
                {
                    case 1:
                        this.ddlCodNatura1.Items.Add(new ListItem(codiceNatura.TraduzioneSuGP.Value.ToString(), codiceNatura.TraduzioneSuGP.Value.ToString()));
                        this.ddlCodNatura1.Items[this.ddlCodNatura1.Items.Count - 1].Attributes.Add("title", codiceNatura.Descrizione);
                        break;
                    case 2:
                        this.ddlCodNatura2.Items.Add(new ListItem(codiceNatura.TraduzioneSuGP.Value.ToString(), codiceNatura.TraduzioneSuGP.Value.ToString()));
                        this.ddlCodNatura2.Items[this.ddlCodNatura2.Items.Count - 1].Attributes.Add("title", codiceNatura.Descrizione);
                        break;
                    case 3:
                        this.ddlCodNatura3.Items.Add(new ListItem(codiceNatura.TraduzioneSuGP.Value.ToString(), codiceNatura.TraduzioneSuGP.Value.ToString()));
                        this.ddlCodNatura3.Items[this.ddlCodNatura3.Items.Count - 1].Attributes.Add("title", codiceNatura.Descrizione);
                        break;
                    default:
                        break;
                }
            }
        }

        private void GestioneCategoriePensioni(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? tipoAppartenenza, List<AreaDecodifica.DatiCategoriaPensione> listaCategoriePensione, List<AreaDecodifica.DatiCategoriaAltraPensione> listaCategorieAltraPensione)
        {
            List<KeyValuePair<string, string>> listaCategorieApp = null;
            PresenterDanteCausa presenterDC = new PresenterDanteCausa();
            ddlCategoriaPensione.Items.Add(new ListItem(string.Empty, string.Empty));

            if (tipoAppartenenza.HasValue)
            {
                if (tipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                    listaCategorieApp = presenterDC.GestioneCategoriePensioniAltraPensione(listaCategoriePensione);
                else
                    listaCategorieApp = presenterDC.GestioneCategoriePensioniAltraPensione(listaCategorieAltraPensione.FindAll(x => x.Appartenenza == tipoAppartenenza.ToString()));
            }

            if (listaCategorieApp != null && listaCategorieApp.Count > 0)
            {
                foreach (KeyValuePair<string, string> app in listaCategorieApp)
                {
                    ListItem li = new ListItem();
                    li.Attributes.Add("title", app.Value);
                    li.Text = app.Value;
                    li.Value = app.Key;
                    ddlCategoriaPensione.Items.Add(li);
                }
            }
        }

        protected void btSalvaAltraPensioneDC_Click(object sender, EventArgs e)
        {
            areaDanteCausa = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaDanteCausa();
            areaDanteCausa.AltraPensioneDC = new AltraPensioneDC();
            areaDanteCausa.AltraPensioneDC = GetValoriAltraPensioneDC();

            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.SalvaDatiAltraPensione(this);
            RaiseShowAvviso(this, null);
        }


        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;
    }
}
