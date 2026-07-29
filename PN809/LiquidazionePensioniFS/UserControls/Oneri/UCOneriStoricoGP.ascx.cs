using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri
{
    public partial class UCOneriStoricoGP : CustomBaseUserControl, IOneri, ITitolarePensione
    {

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IAreaOneri
        public Presenter.SvrLiquidazione.AreaOneri areaOneri { get; set; }
        #endregion IAreaOneri

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        List<DatiOneriBenefParticolari.DatiOneri> elencoOneriViewState = new List<DatiOneriBenefParticolari.DatiOneri>();
        List<DatiOneriBenefParticolari.DatiBeneficiParticolari> elencoBeneficiViewState = new List<DatiOneriBenefParticolari.DatiBeneficiParticolari>();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichetteOneri(IOneri iOneri)
        {
            ViewState["oneri"] = iOneri.areaOneri;

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (iOneri.areaOneri != null)
            {
                if (iOneri.areaOneri.DatiOneriBenefParticolariStorico != null)
                {
                    if (iOneri.areaOneri.DatiOneriBenefParticolariStorico.ListaDatiOneri != null)
                    {
                        ViewState["ElencoOneri"] = iOneri.areaOneri.DatiOneriBenefParticolariStorico.ListaDatiOneri.ToList();
                        ViewState["ElencoGruppo"] = iOneri.areaOneri.ListaGruppoOneri.ToList();
                        ViewState["ElencoSottoGruppo"] = iOneri.areaOneri.ListaSottoGruppoOneri.ToList();
                        gvOneriStoricoGP.DataSource = ViewState["ElencoOneri"];
                    }
                    else
                    {
                        gvOneriStoricoGP.DataSource = null;
                    }

                    if (iOneri.areaOneri.DatiOneriBenefParticolariStorico.ListaDatiBeneficiParticolari != null)
                    {
                        ViewState["ElencoBenefici"] = iOneri.areaOneri.DatiOneriBenefParticolariStorico.ListaDatiBeneficiParticolari.ToList();
                        gvBeneficiStoricoGP.DataSource = ViewState["ElencoBenefici"];
                    }
                    else
                    {
                        gvBeneficiStoricoGP.DataSource = null;
                    }
                }
            }

            gvOneriStoricoGP.DataBind();
            gvOneriStoricoGP.Visible = true;

            gvBeneficiStoricoGP.DataBind();
            gvBeneficiStoricoGP.Visible = true;
        }

        #region GridView OneriStoricoGP

        protected void gvOneriStoricoGP_Load(object sender, EventArgs e)
        {
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            List<DatiOneriBenefParticolari.DatiOneri> listaOneri = (List<DatiOneriBenefParticolari.DatiOneri>)ViewState["ElencoOneri"];

            // Se la domanda è Vittime Terrorismo o se non sono presenti oneri diversi da gruppo 4700 sottogruppo 4701
            // allora non mostro le colonne Settimane e Onere
            if (CodeUtility.IsDomandaVittimeTerrorismo(datiPensione) || (listaOneri != null && listaOneri.Count(x => x.IdCodeGruppo != GetIdGruppoFromValue("4700 ") && x.IdCodeSottoGruppo != GetIdSottoGruppoFromValue("4701 ") && x.IdCodeSottoGruppo != GetIdSottoGruppoFromValue("4702 ")) == 0))
            {
                gvOneriStoricoGP.Columns[(int)gvOneriStoricoGP_Colonne.Settimane].Visible = false;
                gvOneriStoricoGP.Columns[(int)gvOneriStoricoGP_Colonne.Onere].Visible = false;
            }
            //Precoci - ScadenzaBeneficio
            if (datiPensione.IsDomandaAPEPrecociOrRicostituzione)
                gvOneriStoricoGP.Columns[(int)gvOneriStoricoGP_Colonne.CessBenIncumul].Visible = true;
            else
                gvOneriStoricoGP.Columns[(int)gvOneriStoricoGP_Colonne.CessBenIncumul].Visible = false;
        }

        protected void gvOneriStoricoGP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                    AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

                    ((Label)e.Row.FindControl("lblGruppo")).Text = GetValueGruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());
                    ((Label)e.Row.FindControl("lblSottoGruppo")).Text = GetValueSottogruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                    if (this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                        this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT))
                    {
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                        ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                    }
                    else
                    {
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                        ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                    }
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                    ((Label)e.Row.FindControl("lblOnere")).Text = ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Onere.ToString();
                    ((Label)e.Row.FindControl("lblCessBenIncumul")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneriStoricoGP_RowDataBound " + ex);
            }
        }

        protected void gvOneriStoricoGP_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvOneriStoricoGP.PageIndex = e.NewPageIndex;
                GvOneriStoricoGP_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_onPageIndexChanging" + ex);
            }
        }

        #endregion GridView OneriStoricoGP

        #region GridView BeneficiParticolariStoricoGP

        protected void gvBeneficiStoricoGP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblCodiceBeneficiStoricoGP")).Text = ((DatiOneriBenefParticolari.DatiBeneficiParticolari)(e.Row.DataItem)).CodiceBenefici;
                    ((Label)e.Row.FindControl("lblSettimaneStoricoGP")).Text = ((DatiOneriBenefParticolari.DatiBeneficiParticolari)(e.Row.DataItem)).Settimane.ToString();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvBeneficiStoricoGP_RowDataBound " + ex);
            }
        }

        protected void gvBeneficiStoricoGP_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvBeneficiStoricoGP.PageIndex = e.NewPageIndex;
                GVBeneficiStoricoGP_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneriStoricoGP, Errore nel metodo gvBeneficiStoricoGP_onPageIndexChanging" + ex);
            }
        }

        #endregion GridView BeneficiParticolariStoricoGP

        #region Private Methods Oneri

        private void GvOneriStoricoGP_Load()
        {
            try
            {
                elencoOneriViewState = ViewState["ElencoOneri"] as List<DatiOneriBenefParticolari.DatiOneri>;
                gvOneriStoricoGP.DataSource = elencoOneriViewState;
                gvOneriStoricoGP.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo GvOneriStoricoGP_Load " + ex);
            }
        }

        private string GetValueGruppoFromId(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                int index = Convert.ToInt32(id);

                List<CodiciOneriGruppoOneri> listaGruppoOneri = (List<CodiciOneriGruppoOneri>)ViewState["ElencoGruppo"];

                CodiciOneriGruppoOneri app = listaGruppoOneri.Find(delegate (CodiciOneriGruppoOneri code)
                { return (code.Id == index); });
                return app.Code + " - " + app.Descrizione;
            }
            else
                return string.Empty;
        }

        private string GetValueSottogruppoFromId(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                int index = Convert.ToInt32(id);

                List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];

                CodiciOneriSottoGruppoOneri app = listaSottoGruppoOneri.Find(delegate (CodiciOneriSottoGruppoOneri code)
                { return (code.Id == index); });
                return app.Code + " - " + app.Descrizione;
            }
            else
                return string.Empty;
        }

        private long? GetIdGruppoFromValue(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                List<CodiciOneriGruppoOneri> listaGruppoOneri = (List<CodiciOneriGruppoOneri>)ViewState["ElencoGruppo"];

                CodiciOneriGruppoOneri app = listaGruppoOneri.Find(delegate (CodiciOneriGruppoOneri code)
                { return (code.Code == value.Substring(0, value.IndexOf(' '))); });
                return app.Id;
            }
            else
                return (long?)null;
        }

        private long? GetIdSottoGruppoFromValue(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];

                CodiciOneriSottoGruppoOneri app = listaSottoGruppoOneri.Find(delegate (CodiciOneriSottoGruppoOneri code)
                { return (code.Code == value.Substring(0, value.IndexOf(' '))); });
                if (app != null)
                    return app.Id;
                else
                    return (long?)null;
            }
            else
                return (long?)null;
        }

        #endregion Private Methods Oneri

        #region Private Methods Benefici Particolari

        private void GVBeneficiStoricoGP_Load()
        {
            try
            {
                elencoBeneficiViewState = ViewState["ElencoBenefici"] as List<DatiOneriBenefParticolari.DatiBeneficiParticolari>;
                gvBeneficiStoricoGP.DataSource = elencoBeneficiViewState;
                gvBeneficiStoricoGP.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo GVBeneficiStoricoGP_Load " + ex);
            }
        }

        #endregion Private Methods Benefici Particolari

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;

        #region enum
        enum gvOneriStoricoGP_Colonne
        {
            Settimane = 4,
            Onere = 5,
            CessBenIncumul = 6
        }
        #endregion enum

    }
}