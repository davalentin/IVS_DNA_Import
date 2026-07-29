using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.DNA;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo
{
    public partial class UCDatiInteressiLegali : CustomBaseUserControl, ITitolarePensione, ILiquidazionePensioneAgo
    {
        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            RaiseHideAvviso(this, null);
        }

        protected void btnSalvaInteressiLegali_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiInteressiLegali = GetDatiInteressiLegali();

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.SalvaDatiInteressiLegali(this);

            RaiseShowAvviso(this, null);
        }

        protected void btnEliminaInteressiLegali_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.EliminaDatiInteressiLegali(this);
            if (!this.HasError)
            {
                ValorizzaEtichetteIntLegali(this);
            }
            RaiseShowAvvisoElimina(this, null);
        }

        #region metodi protected gvInteressiLegali

        protected void gvInteressiLegali_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    List<GestioneInteressiLegaliDatiInteressiLegali> elencoTipiInteressiLegali = (List<GestioneInteressiLegaliDatiInteressiLegali>)ViewState[EnumViewState.InteressiLegali.ToString()];
                    if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        CodeUtility.EnableEditableMode(e.Row.Cells[0], Keys.ValidationGroup_GrigliaInteressiLegali, Page.Theme);
                        DropDownList ddlTipointLegale = (DropDownList)e.Row.FindControl("ddlTipoIntILegale");
                        LoadDdlTipointeressiLegali(ddlTipointLegale);
                        ddlTipointLegale.SelectedValue = ((GestioneInteressiLegaliDatiInteressiLegali)e.Row.DataItem).TipoInteresseLegale.GetValueOrDefault().ToString();
                    }

                    else
                    {
                        if (e.Row.DataItemIndex == elencoTipiInteressiLegali.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";
                            add.OnClientClick = "BlockUI();";
                        }
                        else
                        {
                            CodeUtility.EnableReadableMode(e.Row.Cells[0], e.Row.Cells[4], Page.Theme, Keys.BtnDelete);
                        }
                        Label lblTipoIntILegale = (Label)e.Row.FindControl("lblTipoIntILegale");
                        lblTipoIntILegale.Text = GetTipiInteressiLegaliPerLabel(((GestioneInteressiLegaliDatiInteressiLegali)e.Row.DataItem).TipoInteresseLegale);
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiInteressiLegali, Errore nel metodo gvInteressiLegali_RowDataBound " + ex);
            }
        }

        protected void gvInteressiLegali_onRowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvInteressiLegali_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvInteressiLegali.EditIndex = -1;
                gvInteressiLegali.PageIndex = e.NewPageIndex;
                gvInteressiLegali_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiInteressiLegali, Errore nel metodo gvInteressiLegali_onPageIndexChanging" + ex);
            }
        }

        private void gvInteressiLegali_Load()
        {
            try
            {
                List<GestioneInteressiLegaliDatiInteressiLegali> listaInteresseLegale = (List<GestioneInteressiLegaliDatiInteressiLegali>)ViewState[EnumViewState.InteressiLegali.ToString()];
                if (listaInteresseLegale.Count == 1 && !listaInteresseLegale.First().TipoInteresseLegale.HasValue && !listaInteresseLegale.First().DataInizio.HasValue && !listaInteresseLegale.First().DataFine.HasValue && !listaInteresseLegale.First().Importo.HasValue)
                    gvInteressiLegali.EditIndex = 0;
                else if (!listaInteresseLegale.ToList().Exists(x => !x.TipoInteresseLegale.HasValue && !x.DataInizio.HasValue && !x.DataFine.HasValue && !x.Importo.HasValue))
                {
                    var listaI = listaInteresseLegale.ToList();
                    listaI.Add(new GestioneInteressiLegaliDatiInteressiLegali());
                    listaInteresseLegale = listaI.ToList();
                    ViewState[EnumViewState.InteressiLegali.ToString()] = listaInteresseLegale;

                    if (listaInteresseLegale.Count() == 1)
                        gvInteressiLegali.EditIndex = 0;
                }
                gvInteressiLegali.DataSource = listaInteresseLegale;
                gvInteressiLegali.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiInteressiLegali, Errore nel metodo gvInteressiLegali_Load " + ex);
            }
        }

        protected void gvInteressiLegali_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                if (((List<GestioneInteressiLegaliDatiInteressiLegali>)ViewState[EnumViewState.InteressiLegali.ToString()]).Count() < 2)
                    gvInteressiLegali.EditIndex = 0;
                else
                    gvInteressiLegali.EditIndex = -1;
                //Bind data to the GridView control.
                gvInteressiLegali_Load();

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiInteressiLEgali, Errore nel metodo gvInteressiLegali_RowCancelingEdit " + ex);
            }
        }

        protected void gvInteressiLegali_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvInteressiLegali.EditIndex = e.NewEditIndex;
                gvInteressiLegali_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiInteressiLegali, Errore nel metodo gvInteressiLegali_RowEditing " + ex);
            }
        }

        protected void gvInteressiLegali_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

                List<GestioneInteressiLegaliDatiInteressiLegali> listaintLeg = (List<GestioneInteressiLegaliDatiInteressiLegali>)ViewState[EnumViewState.InteressiLegali.ToString()];

                var listaI = listaintLeg.ToList();
                listaI.RemoveAt(r.RowIndex);
                listaintLeg = listaI.ToList();
                ViewState[EnumViewState.InteressiLegali.ToString()] = listaintLeg;

                gvInteressiLegali.EditIndex = -1;
                gvInteressiLegali_Load();

                #endregion Elimina
            }

            if (e.CommandName == "Edit")
            {

            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

                List<GestioneInteressiLegaliDatiInteressiLegali> listaInteressiLegali = (List<GestioneInteressiLegaliDatiInteressiLegali>)ViewState[EnumViewState.InteressiLegali.ToString()];
                GestioneInteressiLegaliDatiInteressiLegali interesseLegaleToSave = listaInteressiLegali.ElementAt(r.RowIndex);

                DropDownList tipoInteresseLegale = (DropDownList)r.FindControl(Keys.ddlTipoInteresseLegale);
                TextBox dataInizioIntLegale = (TextBox)r.FindControl(Keys.txtDataInizioIntLegale);
                TextBox dataFineIntLegale = (TextBox)r.FindControl(Keys.txtDataFineIntLegale);
                TextBox importoIntLegale = (TextBox)r.FindControl(Keys.txtImportoIntLegale);

                interesseLegaleToSave.TipoInteresseLegale = CodeUtility.StringToNullableInt64(tipoInteresseLegale.SelectedValue);
                interesseLegaleToSave.DataFine = Utility.GetDateFromString(dataFineIntLegale.Text);
                interesseLegaleToSave.DataInizio = Utility.GetDateFromString(dataInizioIntLegale.Text);
                interesseLegaleToSave.Importo = CodeUtility.StringToNullableDecimal(importoIntLegale.Text);

                gvInteressiLegali.EditIndex = -1;
                gvInteressiLegali_Load();

                #endregion
            }
            else if (e.CommandName == "Annulla")
            {
                gvInteressiLegali.EditIndex = -1;
                gvInteressiLegali_Load();
            }
        }

        #endregion metodi protected gvInteressiLegali

        internal DatiInteressiLegali GetDatiInteressiLegali()
        {
            AreaLiquidazionePensione areaLiquidazionePensioneAgo = new AreaLiquidazionePensione();
            areaLiquidazionePensioneAgo.DatiInteressiLegali = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.DatiInteressiLegali();

            areaLiquidazionePensioneAgo.DatiInteressiLegali.ListaInteressiLegali = GetInteressiLegali();

            areaLiquidazionePensioneAgo.DatiInteressiLegali.ListaInteressiLegali = areaLiquidazionePensioneAgo.DatiInteressiLegali.ListaInteressiLegali.Where(x => x.TipoInteresseLegale.HasValue || x.DataInizio.HasValue || x.DataFine.HasValue || x.Importo.HasValue).ToArray();

            return areaLiquidazionePensioneAgo.DatiInteressiLegali;
        }

        private string GetTipiInteressiLegaliPerLabel(long? p)
        {
            if (p.HasValue)
            {
                List<TipoInteresse> listaDecodfica = (List<TipoInteresse>)ViewState[EnumViewState.TipoInteressiLegali.ToString()];
                TipoInteresse dec = listaDecodfica.Find(x => x.Id == p);
                if (dec != null)
                    return dec.Descrizione;
            }
            return string.Empty;
        }

        internal void ValorizzaEtichetteIntLegali(ILiquidazionePensioneAgo liquidazione)
        {
            List<GestioneInteressiLegaliDatiInteressiLegali> listaInteressiLeg = new List<GestioneInteressiLegaliDatiInteressiLegali>();

            if (liquidazione.areaLiquidazionePensioneAgo != null && liquidazione.areaLiquidazionePensioneAgo.DatiInteressiLegali != null)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.DatiInteressiLegali.ListaInteressiLegali != null && liquidazione.areaLiquidazionePensioneAgo.DatiInteressiLegali.ListaInteressiLegali.Count() > 0)
                    listaInteressiLeg = liquidazione.areaLiquidazionePensioneAgo.DatiInteressiLegali.ListaInteressiLegali.ToList();
            }
            if (liquidazione.areaLiquidazionePensioneAgo != null)
            {
                if (liquidazione.areaLiquidazionePensioneAgo.ListaTipoInteresseLegale != null)
                    ViewState[EnumViewState.TipoInteressiLegali.ToString()] = liquidazione.areaLiquidazionePensioneAgo.ListaTipoInteresseLegale.ToList();
            }
            // Aggiungo un elemento vuoto
            listaInteressiLeg.Add(new GestioneInteressiLegaliDatiInteressiLegali());

            ViewState[EnumViewState.InteressiLegali.ToString()] = listaInteressiLeg;
            gvInteressiLegali_Load();
        }

        internal GestioneInteressiLegaliDatiInteressiLegali[] GetInteressiLegali()
        {
            return ((List<GestioneInteressiLegaliDatiInteressiLegali>)ViewState[EnumViewState.InteressiLegali.ToString()]).ToArray();
        }

        private void LoadDdlTipointeressiLegali(DropDownList ddlTipoIntILegale)
        {
            List<TipoInteresse> listaTipointeresse = (List<TipoInteresse>)ViewState[EnumViewState.TipoInteressiLegali.ToString()];
            ddlTipoIntILegale.Items.Clear();
            CodeUtility.SetItemBlankDdl(ddlTipoIntILegale);
            if (listaTipointeresse != null)
            {
                foreach (TipoInteresse tipoInt in listaTipointeresse)
                    CodeUtility.SetValueDdl(ddlTipoIntILegale, tipoInt.Descrizione, tipoInt.Descrizione, tipoInt.Id.ToString());
            }
        }

        #region EventHandler

        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event Utility.CustomEventHandler ShowAvviso;
        public event Utility.CustomEventHandler ShowAvvisoElimina;
        public event Utility.CustomEventHandler HideAvviso;

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

        protected void RaiseShowAvviso(object sender, Utility.CustomEventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, Utility.CustomEventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvvisoElimina(sender, e);
        }

        protected void RaiseHideAvviso(object sender, Utility.CustomEventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        #endregion EventHandler

        #region enum
        public enum EnumViewState
        {
            InteressiLegali,
            TipoInteressiLegali
        }
        #endregion enum

        public class Keys
        {
            public const string ValidationGroup_GrigliaInteressiLegali = "GrigliaInteressiLegali";
            public const string BtnDelete = "Btndelete";
            public const string ddlTipoInteresseLegale = "ddlTipoIntILegale";
            public const string txtDataInizioIntLegale = "txtDataInizio";
            public const string txtDataFineIntLegale = "txtDataFine";
            public const string txtImportoIntLegale = "txtImporto";
        }
    }
}
