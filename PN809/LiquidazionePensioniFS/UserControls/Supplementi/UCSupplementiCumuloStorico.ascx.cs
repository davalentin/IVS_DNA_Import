using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi
{
    public partial class UCSupplementiCumuloStorico : CustomBaseUserControl, ISupplementi, ITitolarePensione
    {

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        #region ISupplementi
        public long numDomanda { get; set; }
        public AreaSupplementi lstSupplementi { get; set; }
        public Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ISupplementi

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        private List<Presenter.SvrLiquidazione.DecEnteGestioneFondo> VS_DecEnteGestioneFondo
        {
            get { return (List<Presenter.SvrLiquidazione.DecEnteGestioneFondo>)ViewState["decodificaEnteGestioneFondo"]; }
            set { ViewState["decodificaEnteGestioneFondo"] = value; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ValorizzaEtichette(ISupplementi iSupplementi)
        {
            if (iSupplementi != null && iSupplementi.risposta != null && iSupplementi.risposta.ListaDatiSupplementiCumuloStorico != null && iSupplementi.risposta.ListaDatiSupplementiCumuloStorico.Count() > 0)
            {
                AreaSupplementi areaDatiSupplementi = iSupplementi.risposta;
                VS_DecEnteGestioneFondo = areaDatiSupplementi.ListaDecEnteGestioneFondo.ToList();

                InitGVSupplementiStorico(iSupplementi.risposta.ListaDatiSupplementiCumuloStorico.ToList());

            }
        }

        private void InitGVSupplementiStorico(List<DatiSupplementiCumulo> listaSupplementiCumuloStorico)
        {
            if (listaSupplementiCumuloStorico != null && listaSupplementiCumuloStorico.Count() > 0)
            {
                List<QuoteSupplementiLocal> listaQuoteSupplementiStorico = new List<QuoteSupplementiLocal>();
                listaQuoteSupplementiStorico.AddRange(MapServiceToLocalObject(listaSupplementiCumuloStorico));

                gvSupplementiStorico.DataSource = listaQuoteSupplementiStorico;
                gvSupplementiStorico.DataBind();
            }
        }


        protected void gvSupplementiStorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblEnteGestioneFondoStorico_item")).Text = ((QuoteSupplementiLocal)(e.Row.DataItem)).EnteGestioneFondo;
                    ((Label)e.Row.FindControl("lblDescrizioneFondoStorico_item")).Text = ((QuoteSupplementiLocal)(e.Row.DataItem)).DescrizioneFondo;
                    ((Label)e.Row.FindControl("lblDecorrenzaQuotaStorico_item")).Text = ((QuoteSupplementiLocal)(e.Row.DataItem)).Decorrenza;
                    ((Label)e.Row.FindControl("lblImportoQuotaStorico_Item")).Text = ((QuoteSupplementiLocal)(e.Row.DataItem)).ImportoQuota;
                    ((Label)e.Row.FindControl("lblSettimane_Item")).Text = ((QuoteSupplementiLocal)(e.Row.DataItem)).Settimane;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDatiCalcoloStoricoGP_AGO, Errore nel metodo gvDatiContributivi_RowDataBound " + ex);
            }
        }

        #region Nestled Class
        [Serializable]
        public class QuoteSupplementiLocal
        {
            public QuoteSupplementiLocal()
            {
                this.Id = Guid.NewGuid();
            }
            public QuoteSupplementiLocal(string enteGestioneFondo, string importoQuota, string id, string descrizioneFondo, string decorrenza, string settimane)
            {
                this.Id = Guid.NewGuid();
                this.IdEnteGestioneFondo = id;
                this.ImportoQuota = importoQuota;
                this.EnteGestioneFondo = enteGestioneFondo;
                this.DescrizioneFondo = descrizioneFondo;
                this.Decorrenza = decorrenza;
                this.Settimane = settimane;
            }

            public bool IsEmpty()
            {
                bool ret = false;
                if (string.IsNullOrEmpty(this.IdEnteGestioneFondo) && string.IsNullOrEmpty(this.ImportoQuota) && string.IsNullOrEmpty(this.EnteGestioneFondo) &&
                    string.IsNullOrEmpty(this.Decorrenza) && string.IsNullOrEmpty(this.Settimane))
                    ret = true;
                return ret;
            }

            public Guid Id { get; private set; }
            public string IdEnteGestioneFondo { get; set; }
            public string ImportoQuota { get; set; }
            public string EnteGestioneFondo { get; set; }
            public string DescrizioneFondo { get; set; }
            public string Decorrenza { get; set; }
            public string Settimane { get; set; }


            public static QuoteSupplementiLocal GetEmptyQuotaSupplementi()
            {
                return new QuoteSupplementiLocal(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }

        public List<QuoteSupplementiLocal> MapServiceToLocalObject(List<DatiSupplementiCumulo> lstService)
        {
            List<QuoteSupplementiLocal> lstLocal = new List<QuoteSupplementiLocal>();
            if (lstService != null && lstService.Count() > 0)
            {
                foreach (DatiSupplementiCumulo elemS in lstService)
                {
                    QuoteSupplementiLocal elemL = new QuoteSupplementiLocal();
                    elemL.ImportoQuota = elemS.Importo.ToString();
                    elemL.EnteGestioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Codice;
                    elemL.DescrizioneFondo = VS_DecEnteGestioneFondo.Find(x => x.Id == elemS.EnteGestioneFondo).Ente;
                    elemL.IdEnteGestioneFondo = elemS.EnteGestioneFondo.ToString();
                    elemL.Decorrenza = string.Format("{0:MM/yyyy}", elemS.Decorrenza);
                    elemL.Settimane = (elemS.Settimane.HasValue) ? elemS.Settimane.Value.ToString() : string.Empty;
                    lstLocal.Add(elemL);
                }
            }
            return lstLocal;
        }

        #endregion Nestled Class
    }
}