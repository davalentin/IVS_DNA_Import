using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RichiestaBonus
{
    public partial class UCRichiestaBonus : System.Web.UI.UserControl, IRichiestaBonus
    {
        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        public AreaRichiestaBonus areaRichiestaBonus { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        public void ValorizzaRichiestaBonus()
        {
            ViewState[EnumViewState.AreaRichiestaBonus.ToString()] = this.areaRichiestaBonus;

            gvRichiestaBonus_Load();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void EnabledBtnEliminaAnniRichiestaBonus(bool enabled)
        {
            btnEliminaAnniRichiestaBonus.Enabled = enabled;
        }

        public void EnabledBtnSalvaAnniRichiestaBonus(bool enabled)
        {
            btnSalvaAnniRichiestaBonus.Enabled = enabled;
        }

        protected void gvRichiestaBonus_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRichiestaBonus.PageIndex = e.NewPageIndex;
                List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda> Domande = (List<Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda>)Session["Domande"];
                gvRichiestaBonus_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRichiestaBonus, Errore nel metodo gvRichiestaBonus_onPageIndexChanging" + ex);
            }
        }

        private void gvRichiestaBonus_Load()
        {
            try
            {
                InitBindDataRichiestaBonus();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRichiestaBonus, Errore nel metodo gvRichiestaBonus_Load" + ex);
            }
        }

        protected void gvRichiestaBonus_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (areaRichiestaBonus != null && areaRichiestaBonus.RichiestaBonus != null && !areaRichiestaBonus.RichiestaBonus.IsDataFromDB && ((DatiRichiestaBonusLocal)(e.Row.DataItem)).Prescrizione == "SI")
                        ((CheckBox)e.Row.FindControl("chkRichiediBonus")).Checked = false;
                    else
                        ((CheckBox)e.Row.FindControl("chkRichiediBonus")).Checked = ((DatiRichiestaBonusLocal)(e.Row.DataItem)).IsRichiestaBonus;

                    if (((DatiRichiestaBonusLocal)(e.Row.DataItem)).IsRichiestaBonus)
                    {
                        if (areaRichiestaBonus != null && areaRichiestaBonus.RichiestaBonus != null && (((DatiRichiestaBonusLocal)(e.Row.DataItem)).Anno < areaRichiestaBonus.RichiestaBonus.AnnoInizioBonus || ((DatiRichiestaBonusLocal)(e.Row.DataItem)).Prescrizione == "SI"))
                            ((CheckBox)e.Row.FindControl("chkRichiediBonus")).Enabled = true;
                        else
                            ((CheckBox)e.Row.FindControl("chkRichiediBonus")).Enabled = false;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCRichiestaBonus, Errore nel metodo gvRichiestaBonus_RowDataBound " + ex);
            }
        }

        private void InitBindDataRichiestaBonus()
        {
            List<DatiRichiestaBonusLocal> elencoDatiRichiestaBonus = new List<DatiRichiestaBonusLocal>();


            if (((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.DatiAnniRichiestaBonus != null)
            {
                foreach (GestioneAnniRichiestaBonusDatiAnniRichiestaBonus annoBonus in ((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.DatiAnniRichiestaBonus)
                {
                    elencoDatiRichiestaBonus.Add(new DatiRichiestaBonusLocal(annoBonus));
                }
            }
            if (elencoDatiRichiestaBonus.Count() > 0)
            {
                lblStatoPreCalcolo.Visible = true;
            }

            gvRichiestaBonus.DataSource = elencoDatiRichiestaBonus;
            ViewState[EnumViewState.ElencoDatiRichiestaBonus.ToString()] = elencoDatiRichiestaBonus;
            gvRichiestaBonus.DataBind();
        }

        public void btnSalvaAnniRichiestaBonus_Click(object sender, EventArgs e)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaRichiestaBonus = new AreaRichiestaBonus();

            RecuperaCampi(this.areaRichiestaBonus);

            RaiseSalvaRichiestaBonus(sender, e);

        }

        internal void RecuperaCampi(AreaRichiestaBonus areaRichiestaBonus)
        {
            this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            List<DatiRichiestaBonusLocal> listaDatiRichiestaBonusApp = (List<DatiRichiestaBonusLocal>)ViewState[EnumViewState.ElencoDatiRichiestaBonus.ToString()];

            areaRichiestaBonus.RichiestaBonus = new GestioneRichiestaBonusAreaRichiestaBonus();
            areaRichiestaBonus.RichiestaBonus.Categoria = ((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.Categoria;
            areaRichiestaBonus.RichiestaBonus.Certificato = ((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.Certificato;
            areaRichiestaBonus.RichiestaBonus.Sede = ((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.Sede;
            areaRichiestaBonus.RichiestaBonus.TipoBonus = ((AreaRichiestaBonus)ViewState["AreaRichiestaBonus"]).RichiestaBonus.TipoBonus;

            List<GestioneAnniRichiestaBonusDatiAnniRichiestaBonus> lDatiAnniRichiestaBonus = new List<GestioneAnniRichiestaBonusDatiAnniRichiestaBonus>();
            if (listaDatiRichiestaBonusApp != null && listaDatiRichiestaBonusApp.Count > 0)
            {
                int i = 0;
                foreach (DatiRichiestaBonusLocal annoLocal in listaDatiRichiestaBonusApp)
                {
                    GestioneAnniRichiestaBonusDatiAnniRichiestaBonus anno = new GestioneAnniRichiestaBonusDatiAnniRichiestaBonus();
                    anno.Anno = annoLocal.Anno;

                    anno.CodiceEsitoMessaggio = annoLocal.CodiceEsitoMessaggio;
                    anno.DescrizioneEsitoMessaggio = annoLocal.DescrizioneEsitoMessaggio;
                    anno.EsitoCalcoloBeneficio = annoLocal.EsitoCalcoloBeneficio;
                    anno.Prescrizione = annoLocal.Prescrizione == "SI" ? Convert.ToByte(1) : Convert.ToByte(0);
                    anno.IdPensione = annoLocal.IdPensione;
                    anno.Id = annoLocal.Id;

                    CheckBox chkBonus = gvRichiestaBonus.Rows[i].FindControl("chkRichiediBonus") as CheckBox;
                    if (chkBonus != null)
                        anno.IsRichiestaBonus = chkBonus.Checked;

                    lDatiAnniRichiestaBonus.Add(anno);
                    i++;
                }
            }
            areaRichiestaBonus.RichiestaBonus.DatiAnniRichiestaBonus = lDatiAnniRichiestaBonus.ToArray();
        }

        public void btnEliminaAnniRichiestaBonus_Click(object sender, EventArgs e)
        {
            RaiseEliminazioneRichiestaBonus(sender, e);
        }



        #region EventHandler

        public event EventHandler ShowAvviso;
        public event EventHandler HideAvviso;
        public event EventHandler EliminazioneRichiestaBonus;
        public event EventHandler SalvaRichiestaBonus;

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseHideAvviso(object sender, EventArgs e)
        {
            if (HideAvviso != null)
                HideAvviso(sender, e);
        }

        protected void RaiseEliminazioneRichiestaBonus(object sender, EventArgs e)
        {
            if (EliminazioneRichiestaBonus != null)
                EliminazioneRichiestaBonus(sender, e);
        }

        protected void RaiseSalvaRichiestaBonus(object sender, EventArgs e)
        {
            if (SalvaRichiestaBonus != null)
                SalvaRichiestaBonus(sender, e);
        }

        #endregion EventHandler

        #region Enums
        public enum EnumViewState
        {
            AreaRichiestaBonus,
            ElencoDatiRichiestaBonus
        }
        #endregion Enums
    }

    #region nested Class

    [Serializable]
    public class DatiRichiestaBonusLocal
    {
        public DatiRichiestaBonusLocal()
        {

        }

        public DatiRichiestaBonusLocal(GestioneAnniRichiestaBonusDatiAnniRichiestaBonus annoBonus)
        {
            this.Id = annoBonus.Id;
            this.IdPensione = annoBonus.IdPensione;
            this.Anno = annoBonus.Anno;
            this.Prescrizione = annoBonus.Prescrizione == 1 ? "SI" : "NO";
            this.CodiceEsitoMessaggio = annoBonus.CodiceEsitoMessaggio;
            this.DescrizioneEsitoMessaggio = annoBonus.DescrizioneEsitoMessaggio;
            this.EsitoCalcoloBeneficio = annoBonus.EsitoCalcoloBeneficio;
            this.IsRichiestaBonus = annoBonus.IsRichiestaBonus;
        }
        #region private properties
        private long _Id;
        private long _IdPensione;
        private int _Anno;
        private string _Prescrizione;
        private string _CodiceEsitoMessaggio;
        private string _DescrizioneEsitoMessaggio;
        private string _EsitoCalcoloBeneficio;
        private bool _IsRichiestaBonus;

        #endregion private properties

        #region public properties

        public long Id { get { return _Id; } set { _Id = value; } }
        public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public int Anno { get { return _Anno; } set { _Anno = value; } }
        public string Prescrizione { get { return _Prescrizione; } set { _Prescrizione = value; } }
        public string CodiceEsitoMessaggio { get { return _CodiceEsitoMessaggio; } set { _CodiceEsitoMessaggio = value; } }
        public string DescrizioneEsitoMessaggio { get { return _DescrizioneEsitoMessaggio; } set { _DescrizioneEsitoMessaggio = value; } }
        public string EsitoCalcoloBeneficio { get { return _EsitoCalcoloBeneficio; } set { _EsitoCalcoloBeneficio = value; } }
        public bool IsRichiestaBonus { get { return _IsRichiestaBonus; } set { _IsRichiestaBonus = value; } }

        #endregion public properties

    }

    #endregion nested Class
}