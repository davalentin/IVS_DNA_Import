using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Redditi
{
    public partial class UCRedditi : System.Web.UI.UserControl
    {
        public AreaRedditi areaRedditi { get; set; }

        public event EventHandler AcquisizioneRedditi;
        public event EventHandler AggiornamentoRedditi;
        public event EventHandler EliminazioneRedditi;

        public void ValorizzaRedditi()
        {
            ViewState[EnumViewState.AreaRedditi.ToString()] = this.areaRedditi;

            if (areaRedditi.Redditi.UltimaModifica != DateTime.MinValue)
                lblUltimaVariazione.Text = "Ultima variazione dei redditi presenti in archivio risale al " + this.areaRedditi.Redditi.UltimaModifica.ToString("dd/MM/yyyy");
            else
                lblUltimaVariazione.Text = "";
            gvRedditi_Load();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                AreaQuadri areaQuadri = (AreaQuadri)Session["Semaforo"];
                if (areaQuadri.QuadroRedditi.TabRedditi == AreaQuadri.Semaforo.Verde)
                {
                    btnElimina.Visible = true;
                    btnAcquisisci.Visible = false;
                }
                else
                {
                    btnElimina.Visible = false;
                    btnAcquisisci.Visible = true;
                }
            }
        }

        protected void gvRedditi_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRedditi, Errore nel metodo gvRedditi_RowCreated" + ex);
            }


        }

        protected void gvRedditi_onRowCommand(Object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gvRedditi_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRedditi.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];
                gvRedditi_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRedditi, Errore nel metodo gvRedditi_onPageIndexChanging" + ex);
            }
        }

        private void gvRedditi_Load()
        {
            try
            {
                gvRedditi.DataSource = FormattaRedditiVideo();
                gvRedditi.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRedditi, Errore nel metodo gvRedditi_Load" + ex);
            }

        }

        private DataTable FormattaRedditiVideo()
        {
            DataTable listaRedditi = new DataTable();
            listaRedditi.Columns.Add("AnnoReddito");
            listaRedditi.Columns.Add("Rilevanze");

            if (areaRedditi == null)
                areaRedditi = (AreaRedditi)ViewState[EnumViewState.AreaRedditi.ToString()];

            if (areaRedditi.Redditi.ListaRedditi == null)
                return listaRedditi;

            for (int i = 0; i < areaRedditi.Redditi.ListaRedditi.Length; i++)
            {
                if (i == 0)
                    listaRedditi.Rows.Add(areaRedditi.Redditi.ListaRedditi[i].AnnoReddito, areaRedditi.Redditi.ListaRedditi[0].Rilevanza);
                else
                {
                    if (areaRedditi.Redditi.ListaRedditi[i].AnnoReddito == areaRedditi.Redditi.ListaRedditi[i - 1].AnnoReddito)
                        listaRedditi.Rows[listaRedditi.Rows.Count - 1]["Rilevanze"] =
                            listaRedditi.Rows[listaRedditi.Rows.Count - 1]["Rilevanze"] + " " + areaRedditi.Redditi.ListaRedditi[i].Rilevanza;
                    else
                        listaRedditi.Rows.Add(areaRedditi.Redditi.ListaRedditi[i].AnnoReddito, areaRedditi.Redditi.ListaRedditi[i].Rilevanza);
                }
            }

            return listaRedditi;
        }

        public void AggiornaRedditi(Object sender, EventArgs e)
        {
            RaiseAggiornamentoRedditi(this, null);
        }

        protected void RaiseAggiornamentoRedditi(object sender, EventArgs e)
        {
            if (AggiornamentoRedditi != null)
                AggiornamentoRedditi(sender, e);
        }

        public void AcquisisciRedditi(Object sender, EventArgs e)
        {
            RaiseAcquisizioneRedditi(this, null);
        }

        public void EliminaRedditi(Object sender, EventArgs e)
        {
            RaiseEliminaRedditi(this, null);
        }

        protected void RaiseAcquisizioneRedditi(object sender, EventArgs e)
        {
            if (AcquisizioneRedditi != null)
                AcquisizioneRedditi(sender, e);
        }

        protected void RaiseEliminaRedditi(object sender, EventArgs e)
        {
            if (EliminazioneRedditi != null)
                EliminazioneRedditi(sender, e);
        }

        #region Enums
        public enum EnumViewState
        {
            AreaRedditi
        }
        #endregion Enums
    }
}