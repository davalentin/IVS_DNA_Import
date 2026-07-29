using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiNoCalcolo
{
    public partial class UCDatiNoCalcolo : CustomBaseUserControl, IDatiNoCalcolo
    {
        #region IDatiNoCalcolo
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public long IdRecordNoCalcolo { get; set; }
        public AreaNoCalcolo AreaNoCalcolo { get; set; }
        #endregion IDatiNoCalcolo

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        protected void SalvaDatiNoCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.AreaNoCalcolo = new AreaNoCalcolo();
            this.AreaNoCalcolo.DatiNoCalcolo = new Presenter.SvrLiquidazioneFs.DatiNoCalcolo();
            this.IdRecordNoCalcolo = (long)ViewState[EnumViewState.IdRecordNoCalcolo.ToString()];
            this.AreaNoCalcolo.DatiNoCalcolo = RecuperaCampi();
            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();
            presenter.StoreDatiNoCalcoloByIdRecord(this);

            //((AreaNoCalcolo)ViewState[VS_DatiNoCalcolo.AreaDatiNoCalcolo]).DatiNoCalcolo.TabNoCalcolo = areaNoCalcolo.DatiNoCalcolo.TabNoCalcolo;
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati No Calcolo salvati correttamente.";
                RaiseUpdateSemaforoDatiNoCalcolo(this, null);
                RaiseShowAvviso(this, null);
            }
        }

        protected void btnEliminaDatiNoCalcolo_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.AreaNoCalcolo = new AreaNoCalcolo();
            this.AreaNoCalcolo.DatiNoCalcolo = new Presenter.SvrLiquidazioneFs.DatiNoCalcolo();
            this.IdRecordNoCalcolo = (long)ViewState[EnumViewState.IdRecordNoCalcolo.ToString()];
            Presenter.PresenterNoCalcolo presenter = new Presenter.PresenterNoCalcolo();
            presenter.DeleteDatiNoCalcoloByIdRecord(this);
            ValorizzaEtichette(this);
            
            //((AreaNoCalcolo)ViewState[VS_DatiNoCalcolo.AreaDatiNoCalcolo]).DatiNoCalcolo.TabNoCalcolo = areaNoCalcolo.DatiNoCalcolo.TabNoCalcolo;
            if (this.HasError)
                RaiseShowAvviso(this, null);
            else
            {
                this.ErrorMessage = "Dati No Calcolo eliminati correttamente.";
                RaiseUpdateSemaforoDatiNoCalcolo(this, null);
                RaiseShowAvviso(this, null);
            }
        }

        protected void TornaElencoRegistrazioni_Click(object sender, EventArgs e)
        {
            RaiseTornaElencoRegistrazioni(this, null);
        }

        internal void ValorizzaEtichette(IDatiNoCalcolo iDatiNoCalcolo)
        {
            ViewState[EnumViewState.IdRecordNoCalcolo.ToString()] = iDatiNoCalcolo.IdRecordNoCalcolo;
            if (iDatiNoCalcolo.AreaNoCalcolo != null && iDatiNoCalcolo.AreaNoCalcolo.DatiNoCalcolo != null)
            {
                Presenter.SvrLiquidazioneFs.DatiNoCalcolo entity = iDatiNoCalcolo.AreaNoCalcolo.DatiNoCalcolo;
                
                txtDecorrenzaRegistrazione.Text = entity.Decorrenza;
                txtAdeguataFondo.Text = entity.AdeguataFondo.ToString();
                txtAggFamigliaFondo.Text = entity.AggFamigliaFondo.ToString();
                txtArt21.Text = entity.Art21.ToString();
                txtAssegniFamiliari.Text = entity.AssegniFamiliari.ToString();
                txtEccedenzaAgo.Text = entity.EccedenzaAgo.ToString();
                txtFacArt14.Text = entity.FacArt14.ToString();
                txtImportoMensile.Text = entity.ImportoMensile.ToString();
                txtIndIntSpeciale.Text = entity.IndIntSpeciale.ToString();
                txtOnereCaricoAmm.Text = entity.OnereCaricoAmm.ToString();
                txtQuotaAgoEsclusiva.Text = entity.QuotaAgoEsclusiva.ToString();
                txtTredicesima.Text = entity.Tredicesima.ToString();

                ViewState[EnumViewState.ListaComponentiFamiliari.ToString()] = entity.ListaComponentiFamiliari != null ? entity.ListaComponentiFamiliari.ToList() : null;

                dataListComponentiFamiliari.RepeatColumns = entity.ListaComponentiFamiliari != null ? entity.ListaComponentiFamiliari.Count() > 2 ? 3 : entity.ListaComponentiFamiliari.Count() : 0;
                dataListComponentiFamiliari.DataSource = entity.ListaComponentiFamiliari != null ? entity.ListaComponentiFamiliari.ToList() : new List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari>();
                dataListComponentiFamiliari.DataBind();
            }
            ManagePrevalTredicesima(iDatiNoCalcolo.AreaNoCalcolo.CategoriaPI);
        }

        private void ManagePrevalTredicesima(UtilityCategoriaFondoPI? categoriaPI)
        {
            if (categoriaPI == UtilityCategoriaFondoPI.V)
                HdnPrevalTredicesima.Value = "0,5165";
        }

        public Presenter.SvrLiquidazioneFs.DatiNoCalcolo RecuperaCampi()
        {
            Presenter.SvrLiquidazioneFs.DatiNoCalcolo entity = new Presenter.SvrLiquidazioneFs.DatiNoCalcolo();

            if (!string.IsNullOrEmpty(txtDecorrenzaRegistrazione.Text))
                entity.Decorrenza = txtDecorrenzaRegistrazione.Text;
            //txtAdeguataAgo = entity.AdeguataAgo.ToString();
            if(!string.IsNullOrEmpty(txtAdeguataFondo.Text))
                entity.AdeguataFondo = decimal.Parse(txtAdeguataFondo.Text);

            if (!string.IsNullOrEmpty(txtAggFamigliaFondo.Text))
                entity.AggFamigliaFondo = decimal.Parse(txtAggFamigliaFondo.Text);

            if (!string.IsNullOrEmpty(txtArt21.Text))
                entity.Art21 = decimal.Parse(txtArt21.Text);

            if (!string.IsNullOrEmpty( txtAssegniFamiliari.Text ))
                entity.AssegniFamiliari = decimal.Parse(txtAssegniFamiliari.Text);

            if (!string.IsNullOrEmpty(txtEccedenzaAgo.Text))
                entity.EccedenzaAgo = decimal.Parse(txtEccedenzaAgo.Text);

            if (!string.IsNullOrEmpty(txtFacArt14.Text))
                entity.FacArt14 = decimal.Parse(txtFacArt14.Text);

            if (!string.IsNullOrEmpty(txtImportoMensile.Text))
                entity.ImportoMensile = decimal.Parse(txtImportoMensile.Text);

            if (!string.IsNullOrEmpty(txtIndIntSpeciale.Text))
                entity.IndIntSpeciale = decimal.Parse(txtIndIntSpeciale.Text);

            if (!string.IsNullOrEmpty(txtOnereCaricoAmm.Text))
                entity.OnereCaricoAmm = decimal.Parse(txtOnereCaricoAmm.Text);

            if (!string.IsNullOrEmpty(txtQuotaAgoEsclusiva.Text))
                entity.QuotaAgoEsclusiva = decimal.Parse(txtQuotaAgoEsclusiva.Text);
            //txtTipoVar.Text = entity.TipoVar.ToString();

            if (!string.IsNullOrEmpty(txtTredicesima.Text))
                entity.Tredicesima = decimal.Parse(txtTredicesima.Text);

            if (ViewState[EnumViewState.ListaComponentiFamiliari.ToString()] != null)
                entity.ListaComponentiFamiliari = ((List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari>)ViewState[EnumViewState.ListaComponentiFamiliari.ToString()]).FindAll(x => x.IsSelected).ToArray();

            return entity;
        }

        public long GetIdRecordNoCalcolo()
        {
            return (long)ViewState[EnumViewState.IdRecordNoCalcolo.ToString()];
        }

        #region DataList

        protected void dataListComponentiFamiliari_DataBound(Object sender, DataListItemEventArgs e)
        {
            string currentTheme = Page.Theme;
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //LinkButton lnk = ((LinkButton)e.Item.FindControl("lnkComponenteFamiliare"));
                //lnk.ForeColor = ((INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari)e.Item.DataItem).IsSelected ? Color.FromArgb(0x04, 0x89, 0xB1) : Color.Navy;
                //lnk.Style.Add("text-decoration", !((INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari)e.Item.DataItem).IsSelected ? "line-through" : "none");

                Image img = ((Image)e.Item.FindControl("imgComponenteFamiliare"));
                if (((INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari)e.Item.DataItem).IsSelected)
                    img.ImageUrl = "../../App_Themes/" + currentTheme + "/Images/check16.png";
                else
                    img.ImageUrl = "../../App_Themes/" + currentTheme + "/Images/xrosso16.png";
            }
        }

        protected void dataListComponentiFamiliari_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "SelectCFComponenteFamiliare")
            {
                // Recupero i dati dal ViewState
                if (ViewState[EnumViewState.ListaComponentiFamiliari.ToString()] != null)
                {
                    // Imposto IsSelected sul familiare
                    LinkButton lnk = (LinkButton)e.CommandSource;
                    string codiceFiscale = lnk.Text;

                    List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari> listaComponentiFamiliari = (List<INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari>)ViewState[EnumViewState.ListaComponentiFamiliari.ToString()];
                    INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari componente = listaComponentiFamiliari.Find(x => x.CodiceFiscale == codiceFiscale);
                    componente.IsSelected = !componente.IsSelected;
                    ViewState[EnumViewState.ListaComponentiFamiliari.ToString()] = listaComponentiFamiliari;

                    // Rifaccio il DataBind
                    dataListComponentiFamiliari.DataSource = listaComponentiFamiliari;
                    dataListComponentiFamiliari.DataBind();
                }
            }
        }

        #endregion DataList

        #region Event
        public event EventHandler TornaElencoRegistrazioni;
        public event EventHandler ShowAvviso;
        public event EventHandler ShowPulsanteSalva;
        public event EventHandler UpdateSemaforoDatiNoCalcolo;


        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowPulsanteSalva(object sender, EventArgs e)
        {
            ShowPulsanteSalva(sender, e);
        }

        protected void RaiseTornaElencoRegistrazioni(object sender, EventArgs e)
        {
            TornaElencoRegistrazioni(sender, e);
        }

        protected void RaiseUpdateSemaforoDatiNoCalcolo(object sender, EventArgs e)
        {
            UpdateSemaforoDatiNoCalcolo(sender, e);
        }

        #endregion Event

        #region Enum
        public enum EnumViewState
        {
            ListaComponentiFamiliari,
            IdRecordNoCalcolo
        }
        #endregion Enum
    }
   
}
