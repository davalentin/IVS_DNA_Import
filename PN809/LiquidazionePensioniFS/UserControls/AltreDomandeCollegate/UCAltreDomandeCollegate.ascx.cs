using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreDomandeCollegate
{
    public partial class UCAltreDomandeCollegate : CustomBaseUserControl, IAltreDomandeCollegate
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IAltreDomandeCollegate
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaAltreDomandeCollegate AreaAltreDomandeCollegate { get; set; }
        public long NumeroDomandaAventeDiritto { get; set; }
        #endregion IAltreDomandeCollegate

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #endregion protected methods

        #region internal methods
        internal void ValorizzaEtichette(IAltreDomandeCollegate altreDomandeCollegate)
        {
            gvDatiAltreDomandeCollegate.DataSource = altreDomandeCollegate.AreaAltreDomandeCollegate.ElencoDomandeCollegate;
            gvDatiAltreDomandeCollegate.DataBind();
        }
        #endregion internal methods

        #region gvAltreDomandeCollegate
        protected void gvDatiAltreDomandeCollegate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DomandeCollegate domandaCollegata = (DomandeCollegate)e.Row.DataItem;

                    if (domandaCollegata.PensioneAventeDiritto != null)
                        ((Label)e.Row.FindControl("lblPensioneGenerataAventeDiritto")).Text = domandaCollegata.PensioneAventeDiritto.SiglaCategoria + " - " + 
                            domandaCollegata.PensioneAventeDiritto.Sede.PadLeft(4, '0') + " - " + domandaCollegata.PensioneAventeDiritto.Certificato.PadLeft(8, '0');

                    if (domandaCollegata.PensioneRiferimentoDC != null)
                        ((Label)e.Row.FindControl("lblPensioneRiferimentoDanteCausa")).Text = domandaCollegata.PensioneRiferimentoDC.SiglaCategoria + " - " +
                            domandaCollegata.PensioneRiferimentoDC.Sede.PadLeft(4, '0') + " - " + domandaCollegata.PensioneRiferimentoDC.Certificato.PadLeft(8, '0');
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAltreDomandeCollegate, Errore nel metodo gvDatiAltreDomandeCollegate_RowDataBound " + ex);
            }
        }

        protected void gvDatiAltreDomandeCollegate_onRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Selezione")
            {
                if (this.domanda == null)
                    this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                long numeroDomanda = 0;
                long.TryParse((string)e.CommandArgument, out numeroDomanda);

                this.NumeroDomandaAventeDiritto = numeroDomanda;

                PresenterAltreDomandeCollegate presenter = new PresenterAltreDomandeCollegate();
                presenter.GetAventiDiritto(this);

                if (this.HasError)
                    RaiseShowAvviso(this, null);
                else
                    ValorizzaAventiDiritto();
            }
        }
        #endregion gvAltreDomandeCollegate

        #region gvAventiDiritto
        protected void gvAventiDiritto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var listaAnagrafiche = GetVSListaAnagrafiche();
                    var listaDecodifiche = GetVSListaDecodiche();
                    GestioneAventiDirittoAventiDiritto aventeDiritto = (GestioneAventiDirittoAventiDiritto)e.Row.DataItem;
                    var anagrafica = listaAnagrafiche.Find(x => x.Id == aventeDiritto.IdAnagrafica);

                    ((Label)e.Row.FindControl("lblNome")).Text = anagrafica.Nome;
                    ((Label)e.Row.FindControl("lblCognome")).Text = anagrafica.Cognome;
                    ((Label)e.Row.FindControl("lblCodiceFiscale")).Text = anagrafica.CodiceFiscale;
                    ((Label)e.Row.FindControl("lblDataNascita")).Text = String.Format("{0:dd/MM/yyyy}", anagrafica.DataNascita);
                    ((Label)e.Row.FindControl("lblParentelaDC")).Text = String.Format("{0} - {1}", aventeDiritto.DecParentelaDA, listaDecodifiche.Exists(x => x.Id ==
                        aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString()) ? listaDecodifiche.First(x => x.Id ==
                        aventeDiritto.DecParentelaDA.GetValueOrDefault().ToString()).Descrizione : string.Empty);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCAltreDomandeCollegate, Errore nel metodo gvAventiDiritto_RowDataBound " + ex);
            }
        }
        #endregion gvAventiDiritto

        #region private methods
        private List<GestioneAnagraficaDatiAnagrafici> GetVSListaAnagrafiche()
        {
            return ViewState[EnumViewState.ListaAnagrafiche.ToString()] != null ? (List<GestioneAnagraficaDatiAnagrafici>)ViewState[EnumViewState.ListaAnagrafiche.ToString()] : null;
        }

        private void SetVSListaAnagrafiche(List<GestioneAnagraficaDatiAnagrafici> listaAnagrafiche)
        {
            ViewState[EnumViewState.ListaAnagrafiche.ToString()] = listaAnagrafiche;
        }

        private List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> GetVSListaDecodiche()
        {
            return ViewState[EnumViewState.ListaDecodifiche.ToString()] != null ? (List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare>)ViewState[EnumViewState.ListaDecodifiche.ToString()] : null;
        }

        private void SetVSListaDecodiche(List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare> listaAnagrafiche)
        {
            ViewState[EnumViewState.ListaDecodifiche.ToString()] = listaAnagrafiche;
        }

        private List<GestioneAventiDirittoAventiDiritto> GetVSListaAventiDiritto()
        {
            return ViewState[EnumViewState.ListaAventiDiritto.ToString()] != null ? (List<GestioneAventiDirittoAventiDiritto>)ViewState[EnumViewState.ListaAventiDiritto.ToString()] : null;
        }

        private void SetVSListaAventiDiritto(List<GestioneAventiDirittoAventiDiritto> listaAventiDiritto)
        {
            ViewState[EnumViewState.ListaAventiDiritto.ToString()] = listaAventiDiritto;
        }

        private void ValorizzaAventiDiritto()
        {
            pnlAventiDiritto.Visible = true;

            if (this.AreaAltreDomandeCollegate != null && this.AreaAltreDomandeCollegate.AreaAventiDiritto != null)
            {
                var listaAventiDiritto = this.AreaAltreDomandeCollegate.AreaAventiDiritto.ListaAventiDiritto != null ?
                    this.AreaAltreDomandeCollegate.AreaAventiDiritto.ListaAventiDiritto.ToList() : new List<GestioneAventiDirittoAventiDiritto>();
                var listaAnagrafiche = this.AreaAltreDomandeCollegate.AreaAventiDiritto.ListaAnagrafiche != null ?
                    this.AreaAltreDomandeCollegate.AreaAventiDiritto.ListaAnagrafiche.ToList() : new List<GestioneAnagraficaDatiAnagrafici>();
                var listaDecodifiche = this.AreaAltreDomandeCollegate.ElencoGradiParentela != null ?
                    this.AreaAltreDomandeCollegate.ElencoGradiParentela.ToList() : new List<GestioneAreaFamiliariAreaDecFam.DatiSiglaFamiliare>();
                SetVSListaAventiDiritto(listaAventiDiritto);
                SetVSListaAnagrafiche(listaAnagrafiche);
                SetVSListaDecodiche(listaDecodifiche);
                gvAventiDiritto_Load(listaAventiDiritto);   
            }
        }

        private void gvAventiDiritto_Load(List<GestioneAventiDirittoAventiDiritto> listaAventiDiritto)
        {
            gvAventiDiritto.DataSource = listaAventiDiritto;
            gvAventiDiritto.DataBind();
        }
        #endregion private methods

        #region enum
        public enum EnumViewState
        {
            ListaAnagrafiche,
            ListaDecodifiche,
            ListaAventiDiritto
        }
        #endregion enum

        #region Events
        public event EventHandler ShowAvviso;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            if (ShowAvviso != null)
                ShowAvviso(sender, e);
        }
        #endregion Events
    }
}