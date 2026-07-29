using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo
{
    public partial class UCOneri : CustomBaseUserControl, IMaggiorazioneBeneficiAgo, ITitolarePensione
    {
        List<DatiOneri> elencoOneriViewState = new List<DatiOneri>();
        List<DatiBeneficiParticolari> elencoBeneficiViewState = new List<DatiBeneficiParticolari>();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichetteOneri(IMaggiorazioneBeneficiAgo maggiorazioneBenefici)
        {
            ViewState["maggiorazioneBenefici"] = maggiorazioneBenefici.areaMaggiorazioneBenefici;

            if (maggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici != null && maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari != null)
            {
                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri != null)
                {
                    ViewState["ElencoOneri"] = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri.ToList();
                    ViewState["ElencoGruppo"] = maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaGruppoOneri.ToList();
                    ViewState["ElencoSottoGruppo"] = maggiorazioneBenefici.areaMaggiorazioneBenefici.ListaSottoGruppoOneri.ToList();
                    gvOneri.DataSource = ViewState["ElencoOneri"];
                    AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Lettura_L &&
                        maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri.Length > 0)
                    {
                        if (!maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeSottoGruppo.HasValue ||
                            maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeSottoGruppo.Value == 0 ||
                            !maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri[0].Scadenza.HasValue)
                            gvOneri.EditIndex = 0;
                    }
                }
                else
                {
                    gvOneri.DataSource = null;
                }

                if (maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari != null)
                {
                    ViewState["ElencoBenefici"] = maggiorazioneBenefici.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari.ToList();
                    gvBenefici.DataSource = ViewState["ElencoBenefici"];
                }
                else
                {
                    gvBenefici.DataSource = null;
                }
            }

            gvOneri.DataBind();
            gvOneri.Visible = true;

            gvBenefici.DataBind();
            gvBenefici.Visible = true;
        }

        internal DatiOneriBenefParticolari GetValoriOneri()
        {
            this.areaMaggiorazioneBenefici = (AreaMaggiorazioniBenefici)ViewState["maggiorazioneBenefici"];
            List<DatiOneri> listDatiOneri = (List<DatiOneri>)ViewState["ElencoOneri"];
            List<DatiBeneficiParticolari> listDatiBenefici = (List<DatiBeneficiParticolari>)ViewState["ElencoBenefici"];

            removeItemBlankOneri(ref listDatiOneri);
            removeItemBlankBenefici(ref listDatiBenefici);

            if (this.areaMaggiorazioneBenefici == null)
                this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();

            if (this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari == null)
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari = new DatiOneriBenefParticolari();
            
            if (listDatiOneri != null && listDatiOneri.Count() > 0)
            {
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri = new DatiOneri[listDatiOneri.Count()];
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri = listDatiOneri.ToArray();
            }
            else
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiOneri = null;

            if (listDatiBenefici != null && listDatiBenefici.Count() > 0)
            {
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = new DatiBeneficiParticolari[listDatiBenefici.Count()];
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = listDatiBenefici.ToArray();
            }
            else
                this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = null;


            return this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari;
        }

        protected void btnSalvaDatiOneri_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaMaggiorazioneBenefici = new Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici();

            this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari = new DatiOneriBenefParticolari();
            this.areaMaggiorazioneBenefici.DatiOneriBenefParticolari = GetValoriOneri();

            PresenterMaggiorazioneBenefici presenterMaggiorazioneBenefici = new PresenterMaggiorazioneBenefici();
            presenterMaggiorazioneBenefici.SalvaOneriBeneficiParticolariAgo(this);

            RaiseShowAvviso(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

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


        protected void RaiseSalvaOnere(object sender, EventArgs e)
        {
            if (SalvaOnere != null)
                SalvaOnere(sender, e);
        }

        protected void RaiseAnnullaOnere(object sender, EventArgs e)
        {
            if (AnnullaOnere != null)
                AnnullaOnere(sender, e);
        }

        #region GridView Oneri

        protected void gvOneri_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Lettura_L)
                    {
                        gvOneri.EditIndex = -1;
                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                        ((Label)e.Row.FindControl("lblGruppo")).Text = GetValueGruppoFromId(((DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());
                        ((Label)e.Row.FindControl("lblSottoGruppo")).Text = GetValueSottogruppoFromId(((DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((DatiOneri)(e.Row.DataItem)).Decorrenza);
                        ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneri)(e.Row.DataItem)).Scadenza);
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                        ((Label)e.Row.FindControl("lblOnere")).Text = ((DatiOneri)(e.Row.DataItem)).Onere.ToString();
                    }
                    else if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCTabOneri";
                        save.CommandName = "Salva";

                        DropDownList ddlSG = new DropDownList();
                        ddlSG = (DropDownList)e.Row.FindControl("ddlSottoGruppo");
                        LoadDdl(ddlSG, ((DatiOneri)e.Row.DataItem).IdCodeGruppo);

                        ddlSG.SelectedValue = ((DatiOneri)e.Row.DataItem).IdCodeSottoGruppo.HasValue ? ((DatiOneri)e.Row.DataItem).IdCodeSottoGruppo.Value.ToString() : string.Empty;

                        ((Label)e.Row.FindControl("lblGruppo_Edit")).Text = GetValueGruppoFromId(((DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());
                        ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Text = String.Format("{0:MM/yyyy}", ((DatiOneri)(e.Row.DataItem)).Decorrenza);
                        ((TextBox)e.Row.FindControl("txtCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneri)(e.Row.DataItem)).Scadenza);
                        // Modifica inserita a seguito della mail del 17/07/2014 inviata da Nunzio con oggetto: RE: ReEng Pensioni - Oneri Salvaguardia
                        //((TextBox)e.Row.FindControl("txtSettimane")).Text = ((DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                        //((TextBox)e.Row.FindControl("txtOnere")).Text = ((DatiOneri)(e.Row.DataItem)).Onere.ToString();

                        if (btnSalvaDatiOneri.Enabled == true)
                            btnSalvaDatiOneri.Enabled = false;
                        RaiseDisabilitaTastoSalva(this, null);

                    }
                    else
                    {
                        LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        int index = e.Row.DataItemIndex;
                        edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                        edit.ToolTip = "Modifica";
                        ((Label)e.Row.FindControl("lblGruppo")).Text = GetValueGruppoFromId(((DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());
                        ((Label)e.Row.FindControl("lblSottoGruppo")).Text = GetValueSottogruppoFromId(((DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                        ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((DatiOneri)(e.Row.DataItem)).Decorrenza);
                        ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneri)(e.Row.DataItem)).Scadenza);
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                        ((Label)e.Row.FindControl("lblOnere")).Text = ((DatiOneri)(e.Row.DataItem)).Onere.ToString();
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowDataBound " + ex);
            }
        }

        private void LoadDdl(DropDownList ddlSG, long? idCodeGruppo)
        {
            try
            {
                List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];
                if (listaSottoGruppoOneri != null)
                {
                    listaSottoGruppoOneri = listaSottoGruppoOneri.FindAll(x => x.IdOnere == (idCodeGruppo.HasValue ? idCodeGruppo.Value : 0));
                    if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                    {
                        foreach (CodiciOneriSottoGruppoOneri sG in listaSottoGruppoOneri)
                        {
                            ListItem li = new ListItem();
                            li.Attributes.Add("title", sG.Descrizione);
                            li.Text = sG.Code;
                            li.Value = sG.Id.ToString();
                            ddlSG.Items.Add(li);
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
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo LoadDdl " + ex);
            }

        }

        protected void gvOneri_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvOneri.PageIndex = e.NewPageIndex;
                GvOneri_Load();
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

        protected void gvOneri_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvOneri.EditIndex = e.NewEditIndex;
                GvOneri_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowEditing " + ex);
            }
        }

        protected void gvOneri_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<DatiOneri> elencoStatoCivile = (List<DatiOneri>)ViewState["ElencoOneri"];
                GridViewRow row = gvOneri.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvOneri.PageIndex * 10) + e.RowIndex);

                    if (elencoStatoCivile.Count != i + 1)
                        elencoStatoCivile.RemoveAt(elencoStatoCivile.Count - 1);
                    gvOneri.EditIndex = -1;
                    ViewState["elencoStatoCivile"] = elencoStatoCivile;
                    GvOneri_Load();
                    RaiseAnnullaOnere(this, null);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            }
        }

        protected void gvOneri_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            List<DatiOneri> elencoOneri = (List<DatiOneri>)ViewState["ElencoOneri"];
            if (e.CommandName == "Edit")
            {
                if (btnSalvaDatiOneri.Enabled == true)
                    btnSalvaDatiOneri.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);
            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;


                if (btnSalvaDatiOneri.Enabled == false)
                    btnSalvaDatiOneri.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);

                elencoOneri[r.DataItemIndex].IdCodeGruppo = GetIdGruppoFromValue(((Label)(r.Cells[1].Controls[1])).Text);
                elencoOneri[r.DataItemIndex].IdCodeSottoGruppo = long.Parse(((DropDownList)(r.Cells[2].Controls[1])).SelectedItem.Value);
                elencoOneri[r.DataItemIndex].Decorrenza = Utility.ConvertString2Data_MMAAAA(((Label)(r.Cells[3].Controls[1])).Text);
                elencoOneri[r.DataItemIndex].Scadenza = Utility.ConvertString2Data_MMAAAA(((TextBox)(r.Cells[4].Controls[1])).Text);
                short resShort = 0;
                short.TryParse(((Label)(r.Cells[5].Controls[1])).Text, out resShort);
                elencoOneri[r.DataItemIndex].Settimane = resShort != 0 ? resShort : (short?)null;
                decimal resDec = 0;
                decimal.TryParse(((Label)(r.Cells[6].Controls[1])).Text, out resDec);
                elencoOneri[r.DataItemIndex].Onere = resDec != 0 ? resDec : (decimal?)null;
                gvOneri.EditIndex = -1;
                RaiseSalvaOnere(this, null);

                ViewState["ElencoOneri"] = elencoOneri;
                GvOneri_Load();
            }
        }

        protected void gvOneri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvOneri.EditIndex = -1;
                //Bind data to the GridView control.
                GvOneri_Load();
                if (btnSalvaDatiOneri.Enabled == false)
                    btnSalvaDatiOneri.Enabled = true;
                RaiseAnnullaOnere(this, null);
                RaiseAbilitaTastoSalva(this, null);

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowCancelingEdit " + ex);
            }

        }

        #endregion

        #region GridView Benefici Particolari

        protected void gvBenefici_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblCodiceBenefici")).Text = ((DatiBeneficiParticolari)(e.Row.DataItem)).CodiceBenefici;
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiBeneficiParticolari)(e.Row.DataItem)).Settimane.ToString();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvBeneficiParticolari_RowDataBound " + ex);
            }
        }

        protected void gvBenefici_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvBenefici.PageIndex = e.NewPageIndex;
                GvBenefici_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCSupplementi, Errore nel metodo gvSupplementi_onPageIndexChanging" + ex);
            }
        }

        #endregion

        #region Private Methods Oneri

        private void GvOneri_Load()
        {
            try
            {
                elencoOneriViewState = ViewState["ElencoOneri"] as List<DatiOneri>;
                gvOneri.DataSource = elencoOneriViewState;
                gvOneri.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo GvOneri_Load " + ex);
            }
        }

        private void removeItemBlankOneri(ref List<DatiOneri> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate(DatiOneri code)
                {
                    return (string.IsNullOrEmpty(code.IdCodeGruppo.ToString()) && string.IsNullOrEmpty(code.IdCodeSottoGruppo.ToString()) &&
                        string.IsNullOrEmpty(code.Decorrenza.ToString()) && string.IsNullOrEmpty(code.Scadenza.ToString()) &&
                        string.IsNullOrEmpty(code.Settimane.ToString()) && string.IsNullOrEmpty(code.Onere.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        private string GetValueGruppoFromId(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                int index = Convert.ToInt32(id);

                List<CodiciOneriGruppoOneri> listaGruppoOneri = (List<CodiciOneriGruppoOneri>)ViewState["ElencoGruppo"];

                CodiciOneriGruppoOneri app = listaGruppoOneri.Find(delegate(CodiciOneriGruppoOneri code)
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

                CodiciOneriSottoGruppoOneri app = listaSottoGruppoOneri.Find(delegate(CodiciOneriSottoGruppoOneri code)
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

                CodiciOneriGruppoOneri app = listaGruppoOneri.Find(delegate(CodiciOneriGruppoOneri code)
                { return (code.Code == value.Substring(0, value.IndexOf(' '))); });
                return app.Id;
            }
            else
                return (long?)null;
        }

        #endregion

        #region Private Methods Benefici Particolari

        private void GvBenefici_Load()
        {
            try
            {
                elencoBeneficiViewState = ViewState["ElencoBenefici"] as List<DatiBeneficiParticolari>;
                gvBenefici.DataSource = elencoBeneficiViewState;
                gvBenefici.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo GvBenefici_Load " + ex);
            }
        }

        private void removeItemBlankBenefici(ref List<DatiBeneficiParticolari> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate(DatiBeneficiParticolari code)
                {
                    return (string.IsNullOrEmpty(code.CodiceBenefici) && string.IsNullOrEmpty(code.Settimane.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        #endregion

        public event EventHandler ShowAvviso;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler SalvaOnere;
        public event EventHandler AnnullaOnere;

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion

        #region IMaggiorazioneBenefici
        public Presenter.SvrLiquidazioneAgo.AreaMaggiorazioniBenefici areaMaggiorazioneBenefici { get; set; }
      
        #endregion

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion
    }
}