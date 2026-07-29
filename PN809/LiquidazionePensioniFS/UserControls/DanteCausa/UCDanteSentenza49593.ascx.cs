using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Text.RegularExpressions;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa
{
    public partial class UCDanteSentenza49593 : CustomBaseUserControl, IDanteCausa
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
            if (!Page.IsPostBack)
            {
                if (this.domanda == null)
                    domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            }
        }

        internal void ValorizzaEtichetteDatiSentenza49593(IDanteCausa danteCausa)
        {
            if (this.domanda == null)
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<DatiRedditiSentenza495_93.RedditoSentenza495_93> listaRedditi = new List<DatiRedditiSentenza495_93.RedditoSentenza495_93>();
            List<DatiRedditiSentenza495_93.RedditoSentenza495_93> listaSentenze = new List<DatiRedditiSentenza495_93.RedditoSentenza495_93>();

            if (danteCausa != null && danteCausa.areaDanteCausa != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93 != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93 != null)
            {
                listaSentenze = danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93.ToList().FindAll(x => x.MeseSentenza.HasValue || x.AnnoSentenza.HasValue || x.CodiceSentenza.HasValue);

                listaRedditi =
                    danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93
                        .Where(x => !listaSentenze.Contains(x))
                        .ToList();
            }

            //gestisci visibilità pannelli
            if (danteCausa.areaDanteCausa.DatiRedditiSentenza495_93 != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.IsDCSentenza495_93Ante2009)
                divRedditiAnte2009.Visible = true;

            if (danteCausa.areaDanteCausa.DatiRedditiSentenza495_93 != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.IsDCSentenza495_93Post2008)
                divRedditiPost2008.Visible = true;


            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda != null && this.domanda.TipoAppartenenza != null && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria))
            {
                if (Utility.IsRicostituzione(this.domanda.CodGruppo))
                    divSentenze.Visible = true;

                divImportoMensilePensioneEstera.Visible = true;
                if (danteCausa.areaDanteCausa.ImportoMensilePensioneEstera.HasValue)
                    txtImportoMensilePensioneEstera.Text = danteCausa.areaDanteCausa.ImportoMensilePensioneEstera.Value.ToString();
                else
                    txtImportoMensilePensioneEstera.Text = string.Empty;

                lblRedditiPost2008.InnerText = "Redditi del dante causa in applicazione sentenza 495/93 post 2008";
                //divRedditiAnte2009.Visible = false;
            }

            List<RedditoSent495_93> ElencoRedditiPre2009 = new List<RedditoSent495_93>();
            List<RedditoSent495_93> ElencoRedditiPost2008 = new List<RedditoSent495_93>();
            List<Sentenze> elencoSentenze = new List<Sentenze>();

            if (danteCausa != null && danteCausa.areaDanteCausa != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93 != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93 != null && danteCausa.areaDanteCausa.DatiRedditiSentenza495_93.LredditiSentenza495_93.Count() > 0)
            {
                List<DatiRedditiSentenza495_93.RedditoSentenza495_93> redditiPre2009 = listaRedditi.FindAll(x => x.AnnoReddito <= 2008);
                List<DatiRedditiSentenza495_93.RedditoSentenza495_93> redditiPost2008 = listaRedditi.FindAll(x => x.AnnoReddito > 2008);

                foreach (DatiRedditiSentenza495_93.RedditoSentenza495_93 redditoPre2009 in redditiPre2009)
                    AddItem(ref ElencoRedditiPre2009, redditoPre2009);
                foreach (DatiRedditiSentenza495_93.RedditoSentenza495_93 redditoPost2008 in redditiPost2008)
                    AddItem(ref ElencoRedditiPost2008, redditoPost2008);
                foreach (DatiRedditiSentenza495_93.RedditoSentenza495_93 sentenza in listaSentenze)
                    AddItemSentenze(ref elencoSentenze, new Sentenze(sentenza.CodiceSentenza.ToString(), sentenza.FlagSentenza, string.Format("{0:D2}/{1}", sentenza.MeseSentenza, sentenza.AnnoSentenza)));
            }
            AddItemBlank(ref ElencoRedditiPre2009);
            ViewState[EnumViewState.RedditiPre2009.ToString()] = ElencoRedditiPre2009;

            AddItemBlank(ref ElencoRedditiPost2008);
            ViewState[EnumViewState.RedditiPost2008.ToString()] = ElencoRedditiPost2008;

            AddItemBlank(ref elencoSentenze);
            ViewState[EnumViewState.Sentenze.ToString()] = elencoSentenze;

            gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009;
            gvSentenzaAnte2009.DataBind();

            gvSentenzaPost2008.DataSource = ElencoRedditiPost2008;
            gvSentenzaPost2008.DataBind();

            GridViewSentenze.DataSource = elencoSentenze;
            GridViewSentenze.DataBind();
        }

        internal List<DatiRedditiSentenza495_93.RedditoSentenza495_93> GetValoriRedditi()
        {
            List<DatiRedditiSentenza495_93.RedditoSentenza495_93> lRedditiSentenza495_93App = new List<DatiRedditiSentenza495_93.RedditoSentenza495_93>();

            List<RedditoSent495_93> listRedditiPre2009 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()] != null ? ((List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()]).ToList() : null;
            List<RedditoSent495_93> listRedditiPost2008 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()] != null ? ((List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()]).ToList() : null;

            removeItemBlankPre2009(ref listRedditiPre2009);
            removeItemBlankPost2008(ref listRedditiPost2008);

            if ((listRedditiPre2009 != null && listRedditiPre2009.Count() > 0) || (listRedditiPost2008 != null && listRedditiPost2008.Count() > 0))
            {
                if (listRedditiPre2009 != null && listRedditiPre2009.Count() > 0)
                {
                    foreach (RedditoSent495_93 redditoPre2009 in listRedditiPre2009)
                    {
                        DatiRedditiSentenza495_93.RedditoSentenza495_93 redditiSentenza495_93App = new DatiRedditiSentenza495_93.RedditoSentenza495_93();
                        if (!string.IsNullOrEmpty(redditoPre2009.AnnoReddito))
                            redditiSentenza495_93App.AnnoReddito = Convert.ToInt16(redditoPre2009.AnnoReddito);
                        if (!string.IsNullOrEmpty(redditoPre2009.RedditoTitolare))
                            redditiSentenza495_93App.RedditoTitolare = Convert.ToDecimal(redditoPre2009.RedditoTitolare);
                        if (!string.IsNullOrEmpty(redditoPre2009.RedditoConiuge))
                            redditiSentenza495_93App.RedditoConiuge = Convert.ToDecimal(redditoPre2009.RedditoConiuge);
                        redditiSentenza495_93App.IsPre2009 = true;
                        redditiSentenza495_93App.CodiceDiReddito = redditoPre2009.CodiceDiReddito;

                        lRedditiSentenza495_93App.Add(redditiSentenza495_93App);
                    }
                }

                if (listRedditiPost2008 != null && listRedditiPost2008.Count() > 0)
                {
                    foreach (RedditoSent495_93 redditoPost2008 in listRedditiPost2008)
                    {
                        DatiRedditiSentenza495_93.RedditoSentenza495_93 redditiSentenza495_93App = new DatiRedditiSentenza495_93.RedditoSentenza495_93();
                        if (!string.IsNullOrEmpty(redditoPost2008.AnnoReddito))
                            redditiSentenza495_93App.AnnoReddito = Convert.ToInt16(redditoPost2008.AnnoReddito);
                        if (!string.IsNullOrEmpty(redditoPost2008.RedditoDaPensioneDC))
                            redditiSentenza495_93App.RedditoDaPensioneDC = Convert.ToDecimal(redditoPost2008.RedditoDaPensioneDC);
                        if (!string.IsNullOrEmpty(redditoPost2008.RedditoTitolare))
                            redditiSentenza495_93App.RedditoTitolare = Convert.ToDecimal(redditoPost2008.RedditoTitolare);
                        if (!string.IsNullOrEmpty(redditoPost2008.RedditoDaPensioneConiuge))
                            redditiSentenza495_93App.RedditoDaPensioneConiuge = Convert.ToDecimal(redditoPost2008.RedditoDaPensioneConiuge);
                        if (!string.IsNullOrEmpty(redditoPost2008.RedditoConiuge))
                            redditiSentenza495_93App.RedditoConiuge = Convert.ToDecimal(redditoPost2008.RedditoConiuge);
                        redditiSentenza495_93App.IsPre2009 = false;
                        redditiSentenza495_93App.CodiceDiReddito = redditoPost2008.CodiceDiReddito;

                        lRedditiSentenza495_93App.Add(redditiSentenza495_93App);
                    }
                }
            }

            return lRedditiSentenza495_93App;
        }

        internal List<DatiRedditiSentenza495_93.RedditoSentenza495_93> GetValoriSetenze()
        {
            List<DatiRedditiSentenza495_93.RedditoSentenza495_93> datiSentenze = new List<DatiRedditiSentenza495_93.RedditoSentenza495_93>();

            List<Sentenze> sentenzeModel = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()] != null ? ((List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()]).ToList() : null;

            removeItemBlankSentenze(ref sentenzeModel);

            if (sentenzeModel != null && sentenzeModel.Count > 0)
            {
                foreach (Sentenze sentenza in sentenzeModel)
                {
                    DatiRedditiSentenza495_93.RedditoSentenza495_93 datiSentenza = new DatiRedditiSentenza495_93.RedditoSentenza495_93();

                    Regex regex = new Regex(@"^(?:(\d{2})?)\/?(?:(\d{4})?)$");
                    Match match = regex.Match(sentenza.DataDal.Trim());

                    if (match.Success)
                    {
                        short m;
                        short y;
                        datiSentenza.MeseSentenza = short.TryParse(match.Groups[1].Value, out m) ? m : (short?)null;
                        datiSentenza.AnnoSentenza = short.TryParse(match.Groups[2].Value, out y) ? y : (short?)null;
                    }
                    else
                    {
                        datiSentenza.MeseSentenza = null;
                        datiSentenza.AnnoSentenza = null;
                    }
                    short icisen2;
                    datiSentenza.CodiceSentenza = sentenza.Codice != null ? (short.TryParse(sentenza.Codice.Trim(), out icisen2) ? icisen2 : (short?)null) : null;
                    datiSentenza.FlagSentenza = sentenza.Sentenza.Equals(string.Empty) ? (bool?)null : (sentenza.Sentenza.Equals("SI") ? true : false);

                    datiSentenze.Add(datiSentenza);
                }
            }

            return datiSentenze;
        }

        public DatiRedditiSentenza495_93 GetDatiQuadroSentenze495()
        {
            DatiRedditiSentenza495_93 resultDati = new DatiRedditiSentenza495_93();
            List<DatiRedditiSentenza495_93.RedditoSentenza495_93> redditi = GetValoriRedditi();
            List<DatiRedditiSentenza495_93.RedditoSentenza495_93> sentenze = GetValoriSetenze();
            resultDati.LredditiSentenza495_93 = redditi.Concat(sentenze).ToArray();
            return resultDati;
        }

        protected void btnSalvaRedditi_Click(object sender, EventArgs e)
        {
            areaDanteCausa = new AreaDanteCausa();
            areaDanteCausa.DatiRedditiSentenza495_93 = GetDatiQuadroSentenze495();
            areaDanteCausa.ImportoMensilePensioneEstera = GetValorePensioneEstera();

            domanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.SalvaDatiRedditi(this);

            RaiseShowAvviso(this, null);
        }

        internal decimal? GetValorePensioneEstera()
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda != null && this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria))
            {
                if (!String.IsNullOrEmpty(txtImportoMensilePensioneEstera.Text) && !String.IsNullOrEmpty(txtImportoMensilePensioneEstera.Text.Trim()))
                {
                    decimal importMensilePensioneEstera = 0;
                    if (Decimal.TryParse(txtImportoMensilePensioneEstera.Text.Trim(), out importMensilePensioneEstera))
                        return importMensilePensioneEstera;
                }
            }

            return null;
        }

        protected void btnEliminaRedditi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            Presenter.PresenterDanteCausa presenterDanteCausa = new PresenterDanteCausa();
            presenterDanteCausa.EliminaRedditi(this);

            if (this.HasError)
                this.ErrorMessage = "Errore durante l'eliminazione dei Redditi";
            else
            {
                modalitaEditPre2009.Value = "false";
                modalitaEditPost2008.Value = "false";

                ValorizzaEtichetteDatiSentenza49593(this);
            }

            //Verificare se è necessario implementare un nuovo messaggio di avviso per la conferma eliminazione!!!
            RaiseShowAvvisoElimina(this, null);
        }

        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseShowAvvisoElimina(object sender, EventArgs e)
        {
            ShowAvvisoElimina(sender, e);
        }

        #region private method
        private void removeItemBlankSentenze(ref List<Sentenze> lista)
        {
            if (lista == null || lista.Count == 0)
                return;

            int index = lista.FindIndex(delegate (Sentenze code)
            {
                return (code.Sentenza == string.Empty && code.DataDal == string.Empty && code.Codice == string.Empty);
            });

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private void AddItemBlank(ref List<Sentenze> lista)
        {
            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            lista.Add(new Sentenze(string.Empty, null, string.Empty));
        }

        private void AddItemBlank(ref List<RedditoSent495_93> lista)
        {
            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (lista == null || lista.Count == 0 || this.domanda == null || this.domanda.TipoAppartenenza != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI || !CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria))
                lista.Add(new RedditoSent495_93(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
        }

        private void AddItemSentenze(ref List<Sentenze> lista, Sentenze item)
        {
            //ENG - Aggiornamento Modifica Sentenza 495 
            if (this.domanda == null)
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            lista.Add(new Sentenze()
            {
                Codice = item.Codice,
                DataDal = item.DataDal,
                Sentenza = item.Sentenza
            });
        }

        private void AddItem(ref List<RedditoSent495_93> lista, DatiRedditiSentenza495_93.RedditoSentenza495_93 item)
        {
            //ENG - Aggiornamento Modifica Sentenza 495 
            if (this.domanda == null)
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];


            if ((item.AnnoReddito.HasValue && (item.RedditoTitolare.HasValue || item.RedditoConiuge.HasValue || item.RedditoDaPensioneConiuge.HasValue || item.RedditoDaPensioneDC.HasValue))
                || (this.domanda != null && this.domanda.TipoAppartenenza != null && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && item.AnnoReddito.HasValue))
            {
                if (lista == null)
                    lista = new List<RedditoSent495_93>();
                RedditoSent495_93 reddito = new RedditoSent495_93();
                reddito.AnnoReddito = item.AnnoReddito.Value.ToString();
                reddito.RedditoTitolare = item.RedditoTitolare.HasValue ? item.RedditoTitolare.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                reddito.RedditoConiuge = item.RedditoConiuge.HasValue ? item.RedditoConiuge.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                reddito.RedditoDaPensioneConiuge = item.RedditoDaPensioneConiuge.HasValue ? item.RedditoDaPensioneConiuge.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                reddito.RedditoDaPensioneDC = item.RedditoDaPensioneDC.HasValue ? item.RedditoDaPensioneDC.Value.ToString(System.Globalization.CultureInfo.CurrentUICulture) : string.Empty;
                reddito.CodiceDiReddito = item.CodiceDiReddito;
                lista.Add(reddito);
            }
        }

        private bool IsListaEmpty(List<Sentenze> elencoSentenze)
        {
            if (elencoSentenze.Count == 0 || (elencoSentenze.Count == 1 && elencoSentenze[0] != null && String.IsNullOrEmpty(elencoSentenze[0].Sentenza) && String.IsNullOrEmpty(elencoSentenze[0].Codice) && String.IsNullOrEmpty(elencoSentenze[0].DataDal)))
                return true;
            else
                return false;
        }

        private bool IsListaEmpty(List<RedditoSent495_93> ElencoRedditi)
        {
            if (ElencoRedditi.Count == 0 || (ElencoRedditi.Count == 1 && String.IsNullOrEmpty(ElencoRedditi[0].AnnoReddito) && String.IsNullOrEmpty(ElencoRedditi[0].RedditoConiuge) && String.IsNullOrEmpty(ElencoRedditi[0].RedditoDaPensioneConiuge) &&
                String.IsNullOrEmpty(ElencoRedditi[0].RedditoDaPensioneDC) && String.IsNullOrEmpty(ElencoRedditi[0].RedditoTitolare)))
                return true;
            else
                return false;
        }

        private void EnableEditableModeSentenze(TableCell cell_CancelSave)
        {
            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;

            save.ValidationGroup = "UCDanteSentenze49593Sentenze";
        }

        private void EnableEditableMode(TableCell cell_CancelSave, bool IsAnte2009)
        {
            LinkButton cancel = ((LinkButton)(cell_CancelSave.Controls[2]));
            cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
            cancel.ToolTip = "Annulla";
            cancel.CommandName = "Annulla";

            LinkButton save = ((LinkButton)(cell_CancelSave.Controls[0]));
            save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
            save.ToolTip = "Salva";
            save.CommandName = "Salva";
            save.CausesValidation = true;
            if (IsAnte2009)
                save.ValidationGroup = "UCDanteSentenze49593";
            else
                save.ValidationGroup = "UCDanteSentenze49593Post";
        }

        private void EnableReadableModeSentenze(TableCell cell_Edit, TableCell cell_Delete)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteSentenze")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private void EnableReadableMode(TableCell cell_Edit, TableCell cell_Delete, bool IsAnte2009)
        {
            LinkButton edit = ((LinkButton)(cell_Edit.Controls[0]));
            edit.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
            edit.ToolTip = "Modifica";
            LinkButton delete;
            if (IsAnte2009)
                delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteSentenzaAnte2009")));
            else
                delete = ((LinkButton)(cell_Delete.FindControl("btnDeleteSentenzaPost2008")));
            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";
        }

        private void removeItemBlankPre2009(ref List<RedditoSent495_93> lista)
        {
            if (lista == null || lista.Count == 0)
                return;

            int index = lista.FindIndex(delegate (RedditoSent495_93 code)
            {
                return (code.AnnoReddito == string.Empty && code.RedditoTitolare == string.Empty && code.RedditoConiuge == string.Empty);
            });

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private void removeItemBlankPost2008(ref List<RedditoSent495_93> lista)
        {
            if (lista == null || lista.Count == 0)
                return;

            int index = lista.FindIndex(delegate (RedditoSent495_93 code)
            {
                return (code.AnnoReddito == string.Empty && code.RedditoTitolare == string.Empty && code.RedditoConiuge == string.Empty && code.RedditoDaPensioneConiuge == string.Empty && code.RedditoDaPensioneDC == string.Empty);
            });

            if (index >= 0)
            {
                lista.RemoveAt(index);
            }
        }

        private bool IsEmptyEditableRowPre2009(GridViewRow row)
        {
            if ((row.FindControl("txtAnno") != null && ((TextBox)row.FindControl("txtAnno")).Text != string.Empty) ||
                (row.FindControl("txtImporto") != null && ((TextBox)row.FindControl("txtImporto")).Text != string.Empty) ||
                (row.FindControl("txtRedditoConiuge") != null && ((TextBox)row.FindControl("txtRedditoConiuge")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private bool IsEmptyEditableRowPost2008(GridViewRow row)
        {
            if ((row.FindControl("txtAnno") != null && ((TextBox)row.FindControl("txtAnno")).Text != string.Empty) ||
                (row.FindControl("txtRedditoDC") != null && ((TextBox)row.FindControl("txtRedditoDC")).Text != string.Empty) ||
                (row.FindControl("txtRedditoPensioneNoDC") != null && ((TextBox)row.FindControl("txtRedditoPensioneNoDC")).Text != string.Empty) ||
                (row.FindControl("txtRedditoPensioneConiuge") != null && ((TextBox)row.FindControl("txtRedditoPensioneConiuge")).Text != string.Empty) ||
                (row.FindControl("txtRedditoNoPensioneConiuge") != null && ((TextBox)row.FindControl("txtRedditoNoPensioneConiuge")).Text != string.Empty))
                return false;
            else
                return true;
        }

        private void ManageBtnSalva()
        {
            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            List<RedditoSent495_93> elencoRedditiPre2009 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()];
            List<RedditoSent495_93> elencoRedditiPost2008 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()];
            List<Sentenze> elencoSentenze = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()];

            if (((!IsListaEmpty(elencoRedditiPre2009) && modalitaEditPre2009.Value == "true") ||
                (!IsListaEmpty(elencoRedditiPost2008) && modalitaEditPost2008.Value == "true")) ||
                (IsListaEmpty(elencoRedditiPre2009) && IsListaEmpty(elencoRedditiPost2008)))
                btnSalvaRedditi.Enabled = false;
            else
                btnSalvaRedditi.Enabled = true;
        }

        private void RimuoviDallaGriglia(ref List<Sentenze> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        private void RimuoviDallaGriglia(ref List<RedditoSent495_93> lista, int index)
        {
            if (lista != null && lista.Count > index)
            {
                lista.RemoveAt(index);
            }
        }

        #endregion private method

        #region gvSentenzaAnte2009

        protected void gvSentenzaAnte2009_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSentenzaAnte2009.EditIndex = e.NewEditIndex;
                List<RedditoSent495_93> ElencoRedditiPre2009 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()];
                gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009;
                gvSentenzaAnte2009.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaAnte2009_RowEditing " + ex);
            }
        }

        protected void gvSentenzaAnte2009_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvSentenzaAnte2009_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvSentenzaAnte2009.EditIndex = -1;

                List<RedditoSent495_93> ElencoRedditiPre2009 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()];
                gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009;
                gvSentenzaAnte2009.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaAnte2009_RowCancelingEdit " + ex);
            }
        }

        protected void gvSentenzaAnte2009_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSentenzaAnte2009.EditIndex = -1;
                gvSentenzaAnte2009.PageIndex = e.NewPageIndex;
                List<RedditoSent495_93> ElencoRedditiPre2009 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()];
                gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009;
                gvSentenzaAnte2009.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaAnte2009_PageIndexChanging" + ex);
            }
        }

        protected void gvSentenzaAnte2009_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<RedditoSent495_93> ElencoRedditiPre2009App = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                int index = ElencoRedditiPre2009App.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref ElencoRedditiPre2009App, index);

                this.modalitaEditPre2009.Value = "false";
                gvSentenzaAnte2009.EditIndex = -1;
                ViewState[EnumViewState.RedditiPre2009.ToString()] = ElencoRedditiPre2009App;
                gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009App;
                gvSentenzaAnte2009.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditPre2009.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowPre2009((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<RedditoSent495_93> ElencoRedditiPre2009App = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                    int index = ElencoRedditiPre2009App.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    ElencoRedditiPre2009App[index].AnnoReddito = ((TextBox)r.FindControl(Keys.DatiSentenzaPre2009_TxtAnno)).Text;
                    ElencoRedditiPre2009App[index].RedditoTitolare = ((TextBox)r.FindControl(Keys.DatiSentenzaPre2009_TxtImporto)).Text;
                    ElencoRedditiPre2009App[index].RedditoConiuge = ((TextBox)r.FindControl(Keys.DatiSentenzaPre2009_TxtRedditoConiuge)).Text;

                    // Sto inserendo un nuovo record
                    if (index == ElencoRedditiPre2009App.Count - 1)
                        ElencoRedditiPre2009App.Add(new RedditoSent495_93(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvSentenzaAnte2009.EditIndex = -1;
                    ViewState[EnumViewState.RedditiPre2009.ToString()] = ElencoRedditiPre2009App;
                    modalitaEditPre2009.Value = "false";
                    gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009App;
                    gvSentenzaAnte2009.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<RedditoSent495_93> ElencoRedditiPre2009 = ((List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()]);
                if (!IsListaEmpty(ElencoRedditiPre2009))
                {
                    modalitaEditPre2009.Value = "false";
                    gvSentenzaAnte2009.EditIndex = -1;
                    gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009;
                    gvSentenzaAnte2009.DataBind();
                }
            }
        }

        protected void gvSentenzaAnte2009_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            List<RedditoSent495_93> ElencoRedditiPre2009 = ((List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPre2009.ToString()]);

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty(ElencoRedditiPre2009) && !Convert.ToBoolean(modalitaEditPre2009.Value))
                        {
                            gvSentenzaAnte2009.EditIndex = 0;
                            modalitaEditPre2009.Value = "true";
                            gvSentenzaAnte2009.DataSource = ElencoRedditiPre2009;
                            gvSentenzaAnte2009.DataBind();
                        }
                        else if (IsEmptyEditableRowPre2009(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {

                                EnableEditableMode(e.Row.Cells[0], true);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[3].FindControl("btnDeleteSentenzaAnte2009")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnno")).Text = ((RedditoSent495_93)(e.Row.DataItem)).AnnoReddito;
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoTitolare;
                                ((Label)e.Row.FindControl("lblRedditoConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoConiuge;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3], true);
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableMode(e.Row.Cells[0], true);
                                //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                                //if (this.domanda != null && this.domanda.TipoAppartenenza != null && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI &&
                                //    CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura))
                                //    ((TextBox)e.Row.FindControl("txtAnno")).Enabled = false;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnno")).Text = ((RedditoSent495_93)(e.Row.DataItem)).AnnoReddito;
                                ((Label)e.Row.FindControl("lblImporto")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoTitolare;
                                ((Label)e.Row.FindControl("lblRedditoConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoConiuge;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3], true);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            EnableEditableMode(e.Row.Cells[0], true);
                            //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                            //    if (this.domanda != null && this.domanda.TipoAppartenenza != null && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI &&
                            //        CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura))
                            //        ((TextBox)e.Row.FindControl("txtAnno")).Enabled = false;
                        }
                        else if (e.Row.DataItemIndex == ElencoRedditiPre2009.Count - 1)
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";

                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblAnno")).Text = ((RedditoSent495_93)(e.Row.DataItem)).AnnoReddito;
                            ((Label)e.Row.FindControl("lblImporto")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoTitolare;
                            ((Label)e.Row.FindControl("lblRedditoConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoConiuge;
                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[3], true);
                        }
                    }
                }

                ManageBtnSalva();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaAnte2009_RowDataBound " + ex);
            }
        }

        #endregion gvSentenzaAnte2009

        #region gvSentenzaPost2008
        protected void gvSentenzaPost2008_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvSentenzaPost2008.EditIndex = e.NewEditIndex;
                List<RedditoSent495_93> ElencoRedditiPost2008 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()];
                gvSentenzaPost2008.DataSource = ElencoRedditiPost2008;
                gvSentenzaPost2008.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaPost2008_RowEditing " + ex);
            }
        }

        protected void gvSentenzaPost2008_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvSentenzaPost2008_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvSentenzaPost2008.EditIndex = -1;
                List<RedditoSent495_93> ElencoRedditiPost2008 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()];
                gvSentenzaPost2008.DataSource = ElencoRedditiPost2008;
                gvSentenzaPost2008.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaPost2008_RowCancelingEdit " + ex);
            }
        }

        protected void gvSentenzaPost2008_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvSentenzaPost2008.EditIndex = -1;
                gvSentenzaPost2008.PageIndex = e.NewPageIndex;
                List<RedditoSent495_93> ElencoRedditiPost2008 = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()];
                gvSentenzaPost2008.DataSource = ElencoRedditiPost2008;
                gvSentenzaPost2008.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaPost2008_PageIndexChanging" + ex);
            }
        }

        protected void gvSentenzaPost2008_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<RedditoSent495_93> ElencoRedditiPost2008App = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                int index = ElencoRedditiPost2008App.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref ElencoRedditiPost2008App, index);

                this.modalitaEditPost2008.Value = "false";
                gvSentenzaPost2008.EditIndex = -1;

                //ENG - Gestione Pensione Estera e redditi Sentenza 495
                if (this.domanda != null && this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria))
                    AddItemBlank(ref ElencoRedditiPost2008App);

                ViewState[EnumViewState.RedditiPost2008.ToString()] = ElencoRedditiPost2008App;
                gvSentenzaPost2008.DataSource = ElencoRedditiPost2008App;
                gvSentenzaPost2008.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditPost2008.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowPost2008((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<RedditoSent495_93> ElencoRedditiPost2008App = (List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.HdnGUID);
                    int index = ElencoRedditiPost2008App.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    ElencoRedditiPost2008App[index].AnnoReddito = ((TextBox)r.FindControl(Keys.DatiSentenzaPost2008_TxtAnno)).Text;
                    ElencoRedditiPost2008App[index].RedditoDaPensioneDC = ((TextBox)r.FindControl(Keys.DatiSentenzaPost2008_TxtRedditoDC)).Text;
                    ElencoRedditiPost2008App[index].RedditoTitolare = ((TextBox)r.FindControl(Keys.DatiSentenzaPost2008_TxtRedditoPensioneNoDC)).Text;
                    ElencoRedditiPost2008App[index].RedditoDaPensioneConiuge = ((TextBox)r.FindControl(Keys.DatiSentenzaPost2008_TxtRedditoPensioneConiuge)).Text;
                    ElencoRedditiPost2008App[index].RedditoConiuge = ((TextBox)r.FindControl(Keys.DatiSentenzaPost2008_TxtRedditoNoPensioneConiuge)).Text;

                    // Sto inserendo un nuovo record
                    if (index == ElencoRedditiPost2008App.Count - 1 && (this.domanda == null || this.domanda.TipoAppartenenza != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI || !CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)))
                        ElencoRedditiPost2008App.Add(new RedditoSent495_93(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                    gvSentenzaPost2008.EditIndex = -1;
                    ViewState[EnumViewState.RedditiPost2008.ToString()] = ElencoRedditiPost2008App;
                    modalitaEditPost2008.Value = "false";
                    gvSentenzaPost2008.DataSource = ElencoRedditiPost2008App;
                    gvSentenzaPost2008.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<RedditoSent495_93> ElencoRedditiPost2008 = ((List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()]);
                if (!IsListaEmpty(ElencoRedditiPost2008))
                {
                    modalitaEditPost2008.Value = "false";
                    gvSentenzaPost2008.EditIndex = -1;
                    gvSentenzaPost2008.DataSource = ElencoRedditiPost2008;
                    gvSentenzaPost2008.DataBind();
                }
            }
        }

        protected void gvSentenzaPost2008_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            List<RedditoSent495_93> ElencoRedditiPost2008 = ((List<RedditoSent495_93>)ViewState[EnumViewState.RedditiPost2008.ToString()]);

            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //prima riga
                    if (e.Row.DataItemIndex == 0)
                    {
                        //vuota
                        if (IsListaEmpty(ElencoRedditiPost2008) && !Convert.ToBoolean(modalitaEditPost2008.Value))
                        {
                            gvSentenzaPost2008.EditIndex = 0;
                            modalitaEditPost2008.Value = "true";
                            gvSentenzaPost2008.DataSource = ElencoRedditiPost2008;
                            gvSentenzaPost2008.DataBind();
                        }
                        else if (IsEmptyEditableRowPost2008(e.Row))
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {

                                EnableEditableMode(e.Row.Cells[0], false);
                                LinkButton delete = ((LinkButton)(e.Row.Cells[3].FindControl("btnDeleteSentenzaPost2008")));
                                delete.Text = string.Empty;
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnno")).Text = ((RedditoSent495_93)(e.Row.DataItem)).AnnoReddito;
                                ((Label)e.Row.FindControl("lblRedditoDC")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoDaPensioneDC;
                                ((Label)e.Row.FindControl("lblRedditoPensioneNoDC")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoTitolare;
                                ((Label)e.Row.FindControl("lblRedditoPensioneConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoDaPensioneConiuge;
                                ((Label)e.Row.FindControl("lblRedditoNoPensioneConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoConiuge;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], false);
                            }
                        }
                        else  //prima riga non vuota
                        {
                            if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                            {
                                EnableEditableMode(e.Row.Cells[0], false);
                                //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                                //if (this.domanda != null && this.domanda.TipoAppartenenza != null && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI &&
                                //    CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura))
                                //    ((TextBox)e.Row.FindControl("txtAnno")).Enabled = false;

                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblAnno")).Text = ((RedditoSent495_93)(e.Row.DataItem)).AnnoReddito;
                                ((Label)e.Row.FindControl("lblRedditoDC")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoDaPensioneDC;
                                ((Label)e.Row.FindControl("lblRedditoPensioneNoDC")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoTitolare;
                                ((Label)e.Row.FindControl("lblRedditoPensioneConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoDaPensioneConiuge;
                                ((Label)e.Row.FindControl("lblRedditoNoPensioneConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoConiuge;

                                EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], false);
                            }
                        }
                    }
                    else  // righe successive
                    {
                        if (e.Row.Cells[0].Controls.Count == 3)  // edit mode
                        {
                            EnableEditableMode(e.Row.Cells[0], false);
                            //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                            //if (this.domanda != null && this.domanda.TipoAppartenenza != null && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI &&
                            //    CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria) && (Utility.IsRicostituzione(this.domanda.CodGruppo) || this.domanda.IsDomandaRiapertura))
                            //    ((TextBox)e.Row.FindControl("txtAnno")).Enabled = false;
                        }
                        else if (e.Row.DataItemIndex == ElencoRedditiPost2008.Count - 1 &&
                            (this.domanda == null || this.domanda.TipoAppartenenza != AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI || !CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria)))
                        {
                            LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                            add.ToolTip = "Aggiungi";

                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblAnno")).Text = ((RedditoSent495_93)(e.Row.DataItem)).AnnoReddito;
                            ((Label)e.Row.FindControl("lblRedditoDC")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoDaPensioneDC;
                            ((Label)e.Row.FindControl("lblRedditoPensioneNoDC")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoTitolare;
                            ((Label)e.Row.FindControl("lblRedditoPensioneConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoDaPensioneConiuge;
                            ((Label)e.Row.FindControl("lblRedditoNoPensioneConiuge")).Text = ((RedditoSent495_93)(e.Row.DataItem)).RedditoConiuge;
                            EnableReadableMode(e.Row.Cells[0], e.Row.Cells[5], false);
                        }
                    }

                    //ENG - Aggiornamento Modifica Sentenza 495 
                    if (this.domanda != null && this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI && CodeUtility.IsDomandaSuperstitiOrRicostituzione(this.domanda.Categoria))
                    {
                        RequiredFieldValidator validatoreRedditoDC = ((RequiredFieldValidator)e.Row.FindControl("RequiredFieldTxtRedditoDC"));
                        if (validatoreRedditoDC != null)
                            validatoreRedditoDC.Enabled = false;
                    }

                }

                ManageBtnSalva();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaPost2008_RowDataBound " + ex);
            }
        }

        #endregion gvSentenzaPost2008

        #region GrindViewSentenze
        protected void GridViewSentenze_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow)
                    return;

                // Ottieni il dato associato alla riga
                Sentenze sentenza = (Sentenze)e.Row.DataItem;

                // Verifica se sei in modalità Edit globale tramite hidden field
                bool isEditGlobale = GetEditModeFromHiddenField();

                // Verifica se la riga è "vuota" in base ai dati
                bool isEmpty = IsSentenzaVuota(sentenza);

                // Verifica se la riga è in edit mode ASP.NET
                bool isRowInEdit = (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit;

                if (isEmpty)
                {
                    if (IsEmptyEditableRowSentenze(e.Row))
                    {
                        // La riga vuota è in edit (ad es. utente ha cliccato "modifica" sull’ultima riga)
                        if (isRowInEdit || isEditGlobale)
                        {
                            EnableEditableModeSentenze(e.Row.Cells[0]);
                            ClearDeleteButton(e.Row);
                        }
                        else
                        {
                            // Rende la riga vuota leggibile (non edit) e visibile il pulsante aggiungi
                            PopulateReadOnlyControls(e.Row, sentenza);
                            SetAddIcon(e.Row);
                            //EnableReadableModeSentenze(e.Row.Cells[0], e.Row.Cells[3]);
                        }
                    }
                    else
                    {
                        // Rarissimo, ma fallback se la riga vuota non è considerata editable
                        SetAddIcon(e.Row);
                    }
                }
                else
                {
                    // Righe normali con dati
                    if (isRowInEdit || isEditGlobale)
                    {
                        PopulateEditControls(e.Row, sentenza);
                        EnableEditableModeSentenze(e.Row.Cells[0]);
                    }
                    else
                    {
                        PopulateReadOnlyControls(e.Row, sentenza);
                        EnableReadableModeSentenze(e.Row.Cells[0], e.Row.Cells[3]);
                    }
                }

                // Pulsante "Salva" visibile se necessario
                ManageBtnSalva();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaAnte2009_RowDataBound " + ex);
            }
        }

        protected void GridViewSentenze_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridViewSentenze.EditIndex = e.NewEditIndex;
                List<Sentenze> sentenze = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()];
                GridViewSentenze.DataSource = sentenze;
                GridViewSentenze.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDanteSentenza49593, Errore nel metodo GridViewSentenze_RowEditing " + ex);
            }
        }

        protected void GridViewSentenze_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void GridViewSentenze_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridViewSentenze.EditIndex = -1;
                GridViewSentenze.PageIndex = e.NewPageIndex;
                List<Sentenze> sentenze = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()];
                GridViewSentenze.DataSource = sentenze;
                GridViewSentenze.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaPost2008_PageIndexChanging" + ex);
            }
        }

        protected void GridViewSentenze_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridViewSentenze.EditIndex = -1;

                List<Sentenze> elencoSentenze = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()];
                GridViewSentenze.DataSource = elencoSentenze;
                GridViewSentenze.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("UCDanteSentenza49593, Errore nel metodo gvSentenzaAnte2009_RowCancelingEdit " + ex);
            }
        }

        private bool GetEditModeFromHiddenField()
        {
            var hidden = Page.FindControl("modalitaEditSentenza") as HiddenField;
            bool val;
            return hidden != null && bool.TryParse(hidden.Value, out val) && val;
        }

        private bool IsSentenzaVuota(Sentenze s)
        {
            return string.IsNullOrEmpty(s.Sentenza)
                && string.IsNullOrEmpty(s.Codice)
                && string.IsNullOrEmpty(s.DataDal);
        }

        private void SetAddIcon(GridViewRow row)
        {
            var addButton = row.Cells[0].Controls[0] as LinkButton;
            if (addButton != null)
            {
                addButton.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                addButton.ToolTip = "Aggiungi";
            }
        }

        private void ClearDeleteButton(GridViewRow row)
        {
            var delete = row.Cells[3].FindControl("btnDeleteSentenze") as LinkButton;
            if (delete != null)
                delete.Text = string.Empty;
        }

        private void PopulateReadOnlyControls(GridViewRow row, Sentenze sentenza)
        {
            Label lblSentenza = row.FindControl("lblSentenza") as Label;
            if (lblSentenza != null)
                lblSentenza.Text = sentenza.Sentenza;

            var lblCodice = row.FindControl("lblCodice") as Label;
            if (lblCodice != null)
                lblCodice.Text = sentenza.Codice;

            var lblDataDal = row.FindControl("lblSentenzeDataDal") as Label;
            if (lblDataDal != null)
                lblDataDal.Text = sentenza.DataDal;
        }

        private void PopulateEditControls(GridViewRow row, Sentenze sentenza)
        {
            DropDownList ddlSentenza = row.FindControl("ddlSentenza") as DropDownList;
            if (ddlSentenza != null)
            {
                ddlSentenza.SelectedValue = sentenza.Sentenza;
            }
        }

        private bool IsEmptyEditableRowSentenze(GridViewRow row)
        {
            DropDownList ddlSentenza = row.FindControl("ddlSentenza") as DropDownList;
            TextBox txtCodice = row.FindControl("txtCodice") as TextBox;
            TextBox txtDataDal = row.FindControl("txtSentenzeDataDal") as TextBox;

            bool isSentenzaEmpty = ddlSentenza == null || string.IsNullOrEmpty(ddlSentenza.SelectedValue);
            bool isCodiceEmpty = txtCodice == null || string.IsNullOrEmpty(txtCodice.Text);
            bool isDataDalEmpty = txtDataDal == null || string.IsNullOrEmpty(txtDataDal.Text);

            return isSentenzaEmpty && isCodiceEmpty && isDataDalEmpty;
        }

        protected void GridViewSentenze_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Elimina")
            {
                #region Elimina

                GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                List<Sentenze> sentenze = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()];
                HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.SentenzaHdnGUID);
                int index = sentenze.FindIndex(x => x.Id.ToString() == hdnGUID.Value);
                RimuoviDallaGriglia(ref sentenze, index);

                this.modalitaEditSentenza.Value = "false";
                GridViewSentenze.EditIndex = -1;
                ViewState[EnumViewState.Sentenze.ToString()] = sentenze;
                GridViewSentenze.DataSource = sentenze;
                GridViewSentenze.DataBind();

                #endregion Elimina
            }
            else if (e.CommandName == "Edit")
            {
                modalitaEditPre2009.Value = "true";
            }
            else if (e.CommandName == "Salva")
            {
                #region Salva

                if (!IsEmptyEditableRowSentenze((GridViewRow)((Control)e.CommandSource).NamingContainer))
                {
                    GridViewRow r = (GridViewRow)((WebControl)e.CommandSource).NamingContainer;
                    List<Sentenze> sentenze = (List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()];
                    HiddenField hdnGUID = (HiddenField)r.FindControl(Keys.SentenzaHdnGUID);
                    int index = sentenze.FindIndex(x => x.Id.ToString() == hdnGUID.Value);

                    sentenze[index].Sentenza = ((DropDownList)r.FindControl("ddlSentenza")).SelectedValue;
                    sentenze[index].Codice = ((TextBox)r.FindControl("txtCodice")).Text;
                    sentenze[index].DataDal = ((TextBox)r.FindControl("txtSentenzeDataDal")).Text;

                    // Sto inserendo un nuovo record
                    if (index == sentenze.Count - 1)
                        sentenze.Add(new Sentenze(string.Empty, null, string.Empty));
                    GridViewSentenze.EditIndex = -1;
                    ViewState[EnumViewState.Sentenze.ToString()] = sentenze;
                    modalitaEditSentenza.Value = "false";
                    GridViewSentenze.DataSource = sentenze;
                    GridViewSentenze.DataBind();
                }
                #endregion Salva
            }
            else if (e.CommandName == "Annulla")
            {
                List<Sentenze> sentenze = ((List<Sentenze>)ViewState[EnumViewState.Sentenze.ToString()]);
                if (!IsListaEmpty(sentenze))
                {
                    modalitaEditSentenza.Value = "false";
                    GridViewSentenze.EditIndex = -1;
                    GridViewSentenze.DataSource = sentenze;
                    GridViewSentenze.DataBind();
                }
            }
        }
        #endregion

        public event EventHandler ShowAvviso;
        public event EventHandler ShowAvvisoElimina;

        [Serializable]
        public class RedditoSent495_93
        {
            public RedditoSent495_93()
            {
                this.Id = Guid.NewGuid();
            }
            public RedditoSent495_93(string annoReddito, string redditoTitolare, string redditoConiuge, string redditoDaPensioneConiuge, string redditoDaPensioneDC)
            {
                this.Id = Guid.NewGuid();
                _AnnoReddito = annoReddito;
                _RedditoTitolare = redditoTitolare;
                _RedditoConiuge = redditoConiuge;
                _RedditoDaPensioneConiuge = redditoDaPensioneConiuge;
                _RedditoDaPensioneDC = redditoDaPensioneDC;
                _CodiceDiReddito = CodiceDiReddito;
            }

            private string _AnnoReddito;
            private string _RedditoTitolare;
            private string _RedditoConiuge;
            private string _RedditoDaPensioneConiuge;
            private string _RedditoDaPensioneDC;
            private string _CodiceDiReddito;
            private short? _MeseReddito;
            private short? _ICISEN2;

            public Guid Id { get; set; }
            public string AnnoReddito { get { return _AnnoReddito; } set { _AnnoReddito = value; } }
            public string RedditoTitolare { get { return _RedditoTitolare; } set { _RedditoTitolare = value; } }
            public string RedditoConiuge { get { return _RedditoConiuge; } set { _RedditoConiuge = value; } }
            public string RedditoDaPensioneConiuge { get { return _RedditoDaPensioneConiuge; } set { _RedditoDaPensioneConiuge = value; } }
            public string RedditoDaPensioneDC { get { return _RedditoDaPensioneDC; } set { _RedditoDaPensioneDC = value; } }
            public string CodiceDiReddito { get { return _CodiceDiReddito; } set { _CodiceDiReddito = value; } }
            public short? MeseReddito { get { return _MeseReddito; } set { _MeseReddito = value; } }
            public short? ICISEN2 { get { return _ICISEN2; } set { _ICISEN2 = value; } }
        }

        [Serializable]
        public class Sentenze
        {
            public Sentenze()
            {
                Id = Guid.NewGuid();
            }

            public Sentenze(string Codice, bool? Sentenza, string DataDal)
            {
                Id = Guid.NewGuid();
                this.Codice = Codice;
                this.Sentenza = Sentenza == null ? string.Empty : (Sentenza == true ? "SI" : "NO");
                this.DataDal = DataDal;
            }

            public Guid Id { get; set; }
            public string Sentenza { get; set; }
            public string Codice { get; set; }
            public string DataDal { get; set; }
        }

        #region Enums
        public enum EnumViewState
        {
            RedditiPre2009,
            RedditiPost2008,
            Sentenze
        }
        #endregion Enums

        #region Keys
        public class Keys
        {
            public const string SentenzaHdnGUID = "SentenzaHdnGUID";
            public const string HdnGUID = "hdnGUID";
            public const string DatiSentenzaPre2009_TxtAnno = "txtAnno";
            public const string DatiSentenzaPre2009_TxtImporto = "txtImporto";
            public const string DatiSentenzaPre2009_TxtRedditoConiuge = "txtRedditoConiuge";
            public const string DatiSentenzaPost2008_TxtAnno = "txtAnno";
            public const string DatiSentenzaPost2008_TxtRedditoDC = "txtRedditoDC";
            public const string DatiSentenzaPost2008_TxtRedditoPensioneNoDC = "txtRedditoPensioneNoDC";
            public const string DatiSentenzaPost2008_TxtRedditoPensioneConiuge = "txtRedditoPensioneConiuge";
            public const string DatiSentenzaPost2008_TxtRedditoNoPensioneConiuge = "txtRedditoNoPensioneConiuge";
        }
        #endregion Keys
    }
}
