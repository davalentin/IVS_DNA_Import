using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.CambioDataPrepensionamentoLetteraB
{
    public partial class UCCambioDataPrepensionamentoLetteraB : CustomBaseUserControl, IDataLimitePrepensionamentoLetteraB
    {
        public TipoAvviso tipoAvviso { get; set; }

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDataLimitePrepLetteraB
        public AreaStoricoDataLimitePrepensionementoLetteraB storicoDataLimitePrepensionamentoLetteraB { get; set; }
        public AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB dataLimitePrepensionamentoLetteraB { get; set; }
        public UtilityTipoAppartenenza tipoAppRuolo { get; set; }
        #endregion IDataLimitePrepLetteraB

        UtilityTipoAppartenenza tipoAppartenenza { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            if (!IsPostBack)
            {
                ValorizzaEtichette();
                int paginaDaVisualizzare = 1;
                ValorizzaGriglia(paginaDaVisualizzare);
            }
        }

        protected void btnApplica_Click(Object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDataPrepLetteraB.Text))
            {
                var DataPrepLetteraB = Utility.GetDateFromString(txtDataPrepLetteraB.Text);
                if (!DataPrepLetteraB.HasValue)
                {
                    this.dataLimitePrepensionamentoLetteraB.DataLimitePoligraficiLetteraB = DataPrepLetteraB.Value;
                    this.ErrorMessage = "Inserire una data valida";
                    this.tipoAvviso = TipoAvviso.Ko;
                }
                else
                {
                    //Inserimento di data anteriore all’ultima data limite sulle domande di PrepLetteraB e successiva alla data corrente;
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    string controlloDinamico = string.Empty;
                    presenter.GetControlloDinamicoByNomeControllo("DataCalcoloPoligraficiEBA", out controlloDinamico);
                    DateTime? DataDb = Utility.DataFromString(controlloDinamico, Utility.FormatoData.AAAAmmGG);

                    if (DataDb.HasValue && DataDb.Value > DataPrepLetteraB)
                    {
                        ErrorMessage = "Inserimento di data anteriore all’ultima data limite sulle domande di PrepLetteraB";
                        tipoAvviso = TipoAvviso.Info;
                    }
                    if (DataDb.HasValue && DataPrepLetteraB > DateTime.UtcNow)
                    {
                        ErrorMessage = "Inserimento di data successiva alla data corrente";
                        tipoAvviso = TipoAvviso.Info;
                    }

                    Presenter.PresenterCambioDataLimitePrepensionamentoLetteraB presenterDataPrepLetteraB = new PresenterCambioDataLimitePrepensionamentoLetteraB();

                    //AreaStoricoDataLimiteDomandePrepLetteraB.StoricoDataLimiteDomandePrepLetteraB nuovaDataLimiteDomandePrepLetteraB = new AreaStoricoDataLimiteDomandePrepLetteraB.StoricoDataLimiteDomandePrepLetteraB();
                    this.dataLimitePrepensionamentoLetteraB = new AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB();

                    this.dataLimitePrepensionamentoLetteraB.DataLimitePoligraficiLetteraB = Utility.DataFromString(txtDataPrepLetteraB.Text, Utility.FormatoData.GGmmAAAA).Value;
                    this.dataLimitePrepensionamentoLetteraB.Matricola = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;

                    AreaEsito esito = presenterDataPrepLetteraB.SetDataCalcoloPrepensionamentoLetteraB(this);

                    ValorizzaGriglia(1);
                    if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                    {
                        ErrorMessage = esito.Messaggio;
                        tipoAvviso = TipoAvviso.Ko;
                    }
                    else if (string.IsNullOrEmpty(ErrorMessage))
                    {
                        ErrorMessage = "Data modificata correttamente";
                        tipoAvviso = TipoAvviso.Ok;
                    }

                }
            }
            else
            {
                ErrorMessage = "Inserire una data";
                tipoAvviso = TipoAvviso.Ko;
            }

            ValorizzaEtichette();

            RaiseShowAvviso(this, null);
        }

        protected void ValorizzaEtichette()
        {
            string dataDb;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            presenter.GetControlloDinamicoByNomeControllo("DataCalcoloPoligraficiEBA", out dataDb);

            var i = Utility.DataFromString(dataDb, Utility.FormatoData.AAAAmmGG);

            txtDataPrepLetteraB.Text = string.Format("{0:dd/MM/yyyy}", i);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        public event EventHandler ShowAvviso;


        #region private member

        private void ValorizzaViewStateStoricoDataPrepLetteraB()
        {
            if (this.storicoDataLimitePrepensionamentoLetteraB != null)
            {
                ViewState["ElencoStoricoDataPrepLetteraB"] = this.storicoDataLimitePrepensionamentoLetteraB.ListStoricoDataLimiteDomandePrepensionementoLetteraB.ToList();
            }
        }

        private void gvStoricoDataPrepLetteraB_Load(int paginaDaVisualizzare)
        {
            try
            {
                gvStoricoDataPrepLetteraB.DataSource = (List<AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB>)ViewState["ElencoStoricoDataPrepLetteraB"];
                gvStoricoDataPrepLetteraB.PageIndex = paginaDaVisualizzare < 1 ? 0 : paginaDaVisualizzare - 1;//paginaDaVisualizzare - 1 perchè la prima pagina ha PageIndex = 0 
                gvStoricoDataPrepLetteraB.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCCambioDataPrepLetteraB, Errore nel metodo gvBypassControlli_Load " + ex);
            }
        }

        #endregion private member
        private void ValorizzaGriglia(int paginaDaVisualizzare)
        {
            Presenter.PresenterCambioDataLimitePrepensionamentoLetteraB presenter = new Presenter.PresenterCambioDataLimitePrepensionamentoLetteraB();
            AreaStoricoDataLimitePrepensionementoLetteraB storico = null;
            AreaEsito esito = presenter.GetDataCalcoloPrepensionamentoLetteraB(out storico);
            this.storicoDataLimitePrepensionamentoLetteraB = storico;

            if (this.HasError)
            {
                RaiseShowAvviso(this, null);
                return;
            }

            ValorizzaViewStateStoricoDataPrepLetteraB();
            gvStoricoDataPrepLetteraB_Load(paginaDaVisualizzare);
        }

        #region Grid
        protected void gvStoricoDataPrepLetteraB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                List<AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB> elencoStorico = (List<AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB>)ViewState["ElencoStoricoDataPrepLetteraB"];
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (elencoStorico != null && elencoStorico.Count > 0)
                    {
                        Label lbMatricola = (Label)e.Row.FindControl("lbMatricola");
                        lbMatricola.Text =
                            elencoStorico.Exists(x => x.Id == ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id) ?
                            elencoStorico.Find(x => x.Id == ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id).Matricola : string.Empty;

                        Label lbDataModifica = (Label)e.Row.FindControl("lbDataModifica");
                        lbDataModifica.Text =
                            elencoStorico.Exists(x => x.Id == ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id) ?
                            elencoStorico.Find(x => x.Id == ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id).DataModifica.ToString("dd/MM/yyyy") : string.Empty;

                        Label lblDataLimitePrepLetteraB = (Label)e.Row.FindControl("lblDataLimitePrepLetteraB");
                        lblDataLimitePrepLetteraB.Text =
                            elencoStorico.Exists(x => x.Id == ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id) ?
                            elencoStorico.Find(x => x.Id == ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id).DataLimitePoligraficiLetteraB.ToString("dd/MM/yyyy") : string.Empty;

                        Label id = (Label)e.Row.FindControl("lblId");
                        id.Text = ((AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)(e.Row.DataItem)).Id.ToString();

                        LinkButton edit = (LinkButton)e.Row.FindControl("btnEdit");
                        int index = e.Row.DataItemIndex;
                        if (index >= 0 && index <= elencoStorico.Count - 1)
                        {
                            edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                            edit.ToolTip = "Modifica";
                            edit.OnClientClick = "BlockUI();";
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
                throw new INPS.DNA.DnaApplicationException("UCBypassControlli, Errore nel metodo gvBypassControlli_RowDataBound " + ex);
            }
        }

        protected void gvStoricoDataPrepLetteraB_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ShowNota")
            {
                hdnTextDialog.Value = e.CommandArgument.ToString();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalDialog", "ShowNota();", true);
            }

            if (e.CommandName == "Modifica")
            {
                hdnTextDialogEdit.Value = e.CommandArgument.ToString();

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;

                List<AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB> elencoStorico = (List<AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB>)ViewState["ElencoStoricoDataPrepLetteraB"];
                Label id = (Label)r.FindControl("lblId");

                hdnIdDialogEdit.Value = id.Text;

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalDialogEdit", "ShowNotaEdit();", true);
            }

        }

        protected void gvStoricoDataPrepLetteraB_onPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvStoricoDataPrepLetteraB.EditIndex = -1;
                int paginaDaVisualizzare = e.NewPageIndex + 1;//e.NewPageIndex + 1  perchè la  prima pagina ha PageIndex = 0 
                hdnIndexGrid.Value = paginaDaVisualizzare.ToString();
                gvStoricoDataPrepLetteraB_Load(paginaDaVisualizzare);
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCBypassControlli, Errore nel metodo gvBypassControlli_onPageIndexChanging" + ex);
            }
        }

        protected string ValorizzaTesto(GridViewRow row)
        {
            AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB storico = (AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB)row.DataItem;

            if (!string.IsNullOrEmpty(storico.Note))
                return "Vedi nota";
            else
            {
                ((LinkButton)row.FindControl("lblNote")).Enabled = false;
                return "Nessuna nota";
            }
        }
        #endregion Grid

        protected void btnModifica_Click(object sender, EventArgs e)
        {
            Presenter.PresenterCambioDataLimitePrepensionamentoLetteraB presenter = new Presenter.PresenterCambioDataLimitePrepensionamentoLetteraB();

            AreaEsito esito = presenter.UpdateNote(int.Parse(hdnIdDialogEdit.Value), hdnTextDialogEdit.Value);

            int paginaDaVisualizzare = hdnIndexGrid.Value != "" ? int.Parse(hdnIndexGrid.Value) : 1;
            ValorizzaGriglia(paginaDaVisualizzare);
        }
    }
}