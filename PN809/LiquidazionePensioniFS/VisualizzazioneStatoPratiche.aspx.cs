using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{

    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class VisualizzazioneStatoPratiche : CustomBasePage, IStatoPratiche
    {
        #region IStatoPratiche
        public StatoPratica StatoPratica { get; set; }
        public List<AreaRispostaStatoPratica.DatiStatoPratica> ElencoStatoPratiche { get; set; }
        public Presenter.SvrLiquidazione.AreaEsito.TipoEsito Esito { get; set; }
        public UtilityTipoAppartenenza TipoAppRuolo { get; set; }
        public UtilityRuolo Ruolo { get; set; }
        #endregion IStatoPratiche

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        protected void Page_Load(object sender, EventArgs e)
        {
            int search = -1;
            bool isQuerystring = false;
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);

            if (!Page.IsPostBack)
            {
                if (Server.HtmlEncode(Request.QueryString["R"]) != null)
                    isQuerystring = int.TryParse(Server.HtmlEncode(Request.QueryString["R"]), out search);

                if (!isQuerystring)
                    search = -1;

                if (search == 0)
                {
                    Session.Remove("FlagBack");
                    Session.Remove("CriteriComplete");
                    Session.Remove("NumeroCriteri");
                    Session.Remove("CriteriSelezionati");
                    Session.Remove("Criteri");
                }

                if (Session["FlagAnnulla"] != null && (bool)Session["FlagAnnulla"] == true)
                {
                    Session.Remove("FlagBack");
                    Session.Remove("CriteriComplete");
                    Session.Remove("NumeroCriteri");
                    Session.Remove("CriteriSelezionati");
                    Session.Remove("Criteri");

                    Session.Remove("FlagAnnulla");
                }

                if (Session["FlagBack"] == null)
                {
                    Session.Remove("Pratiche");
                    Session.Remove("NumeroCriteri");
                    Session.Remove("StatoPratica");
                    Session.Remove("VSPNumeroDomanda");
                    Session.Remove("VSPFondo");
                    Session.Remove("VSPCassa");
                    Session.Remove("VSPSede");
                    Session.Remove("VSPStatoPratica");
                    Session.Remove("VSPCategoriaPensione");
                    Session.Remove("VSPDataElaborazioneDal");
                    Session.Remove("VSPDataElaborazioneAl");
                    Session.Remove("VSPDataPresentazioneDal");
                    Session.Remove("VSPDataPresentazioneAl");
                    Session.Remove("VSPCodiceFiscale");
                    Session.Remove("VSPCognome");
                    Session.Remove("VSPNome");
                    Session.Remove("VSPMatricola");
                    Session.Remove("VSPTipoDomandaInLavorazione");
                    Session.Remove("VSPTipoDomandaLavorata");
                    Session.Remove("VSPGruppo");
                    Session.Remove("Pensioni");             //elenco pensioni trovate
                    Session.Remove("Domande");              //elenco domande trovate
                    Session.Remove("Anagrafica");           //anagrafica soggetto
                    Session.Remove("TornaASinonimi");       //switch per UC da visualizzare
                    Session.Remove("Nome");
                    Session.Remove("Cognome");
                    Session.Remove("CF");
                    Session.Remove("TipoRicerca");
                    Session.Remove("Domanda");              //riepilogo domanda
                    Session.Remove("EsitoCalcolo");
                    Session.Remove("Semaforo");             //semafori quadri
                    Session.Remove("Sinonimi");             //elenco sinonimi
                    Session.Remove("DatiPensione");         //dati pensione 
                    Session.Remove("Criteri");              //eliminazione criterio in visualizzazione stato pratiche
                    Session.Remove("Lavorabile");           //flag per determinare se la pensione è lavorabile
                }

                int numCriteri = -1;
                if (Session["NumeroCriteri"] == null)
                    numCriteri = 0;
                else numCriteri = (int)Session["NumeroCriteri"];

                if (Session["CriteriComplete"] != null)
                {
                    List<string> criteri = null;

                    for (int i = 0; i <= numCriteri - 1; i++)
                    {
                        switch (i.ToString())
                        {
                            case "0":
                                criteri = (List<string>)Session["CriteriComplete"];
                                ucStatoPratiche.LoadUserControl(criteri);
                                ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                ucStatoPratiche.LoadCriteriRicerca(criteri);
                                break;
                            case "1":
                                criteri = (List<string>)Session["CriteriComplete"];
                                ucStatoPratiche1.LoadUserControl(criteri);
                                ucStatoPratiche1.Visible = true;
                                ((DropDownList)ucStatoPratiche1.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                                ucStatoPratiche1.LoadCriteriRicerca(criteri);
                                break;
                            case "2":
                                criteri = (List<string>)Session["CriteriComplete"];
                                ucStatoPratiche2.LoadUserControl(criteri);
                                ucStatoPratiche2.Visible = true;
                                ((DropDownList)ucStatoPratiche2.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                ((DropDownList)ucStatoPratiche1.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                                ucStatoPratiche2.LoadCriteriRicerca(criteri);
                                break;
                            case "3":
                                criteri = (List<string>)Session["CriteriComplete"];
                                ucStatoPratiche3.LoadUserControl(criteri);
                                ucStatoPratiche3.Visible = true;
                                ucStatoPratiche3.FindControl("addButton").Visible = false;
                                ((DropDownList)ucStatoPratiche3.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                ((DropDownList)ucStatoPratiche2.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                                ucStatoPratiche3.LoadCriteriRicerca(criteri);
                                break;
                        }
                    }

                    switch (numCriteri.ToString())
                    {
                        case "0":
                            ucStatoPratiche.LoadUserControl(criteri);
                            ucStatoPratiche1.LoadUserControl(criteri);
                            ucStatoPratiche2.LoadUserControl(criteri);
                            ucStatoPratiche3.LoadUserControl(criteri);
                            break;
                        case "1":
                            ucStatoPratiche1.LoadUserControl(criteri);
                            ucStatoPratiche2.LoadUserControl(criteri);
                            ucStatoPratiche3.LoadUserControl(criteri);
                            break;
                        case "2":
                            ucStatoPratiche2.LoadUserControl(criteri);
                            ucStatoPratiche3.LoadUserControl(criteri);
                            break;
                        case "3":
                            ucStatoPratiche3.LoadUserControl(criteri);
                            break;
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ////criterio di default della ricerca "visualizza stato pratiche" impostato su Sede
            ////commentato a seguito della modifica richiesta dal documento "Funzionalità IVS versione 1.7 dell' 11/11/2016"


            //if (this.Ruolo == UtilityRuolo.AMMINISTRATORE)
            //{
            //    if (string.IsNullOrEmpty(((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).SelectedValue))
            //    {
            //        ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).SelectedValue = "Sede";
            //        ((TextBox)ucStatoPratiche.FindControl("txtSede")).Text = GetSedeCurrentOffice();
            //    }
            //}

            ////criterio di default della ricerca "visualizza stato pratiche" impostato su Numero Domanda
            if (string.IsNullOrEmpty(((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).SelectedValue))
                ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).SelectedValue = "NumeroDomanda";
        }

        protected void btnRicerca_Click(object sender, EventArgs e)
        {
            int numeroCriteri = -1;
            if (Session["FlagBack"] != null)
            {
                numeroCriteri = (int)Session["NumeroCriteri"];
            }
            else
            {
                numeroCriteri = Int32.Parse(hdnNCriteri.Value) + 1;
            }
            SetCriteri();

            //metodo per ripopolare la sessione CriteriComplete necessaria per il "Torna alla Ricerca"
            GetListaCriteriComplete();

            if (numeroCriteri == 4)
            {
                event_AggiungiParametro(sender, e);
            }
            PresenterVisualizzaStatoPratiche presenterVisualizzaStatoPratiche = new PresenterVisualizzaStatoPratiche();
            this.StatoPratica = new StatoPratica();
            SetRicerca();
            presenterVisualizzaStatoPratiche.RicercaPratiche(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                return;

            }
            Session["Pratiche"] = this.ElencoStatoPratiche;
            Session["NumeroCriteri"] = numeroCriteri;
            Session["StatoPratica"] = this.StatoPratica;
            Response.Redirect("RisultatoVisualizzaStatoPratiche.aspx", false);
        }


        protected void event_AggiungiParametro(object sender, EventArgs e)
        {
            int numeroCriteriSelezionati = -1;
            string numCriteri = "";

            if (Session["FlagBack"] != null)
            {
                numeroCriteriSelezionati = (int)Session["NumeroCriteri"] - 1;
                numCriteri = numeroCriteriSelezionati.ToString();
            }
            else
            {
                numCriteri = hdnNCriteri.Value;
            }

            switch (numCriteri)
            {
                case "0":
                    ucStatoPratiche1.Visible = true;
                    ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                    ((DropDownList)ucStatoPratiche1.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                    break;
                case "1":
                    ucStatoPratiche2.Visible = true;
                    ((DropDownList)ucStatoPratiche2.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                    ((DropDownList)ucStatoPratiche1.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                    break;
                case "2":
                    ucStatoPratiche3.Visible = true;
                    ((DropDownList)ucStatoPratiche3.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                    ((DropDownList)ucStatoPratiche2.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                    break;
                case "3":
                    ucStatoPratiche3.Visible = true;
                    ucStatoPratiche3.FindControl("addButton").Visible = false;
                    ((DropDownList)ucStatoPratiche3.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = false;
                    break;
            }

            SetCriterio();
            SetCriteri();

            int myNumeroCriteri;

            if (Session["FlagBack"] != null)
            {
                myNumeroCriteri = (int)Session["NumeroCriteri"] + 1;
                if (myNumeroCriteri > 4)
                    myNumeroCriteri = 4;
            }
            else
            {
                myNumeroCriteri = Int32.Parse(hdnNCriteri.Value);
                myNumeroCriteri = myNumeroCriteri + 1;
                hdnNCriteri.Value = myNumeroCriteri.ToString();
            }

            Session["NumeroCriteri"] = int.Parse(myNumeroCriteri.ToString());
        }
        private void SetCriteri()
        {
            string[] resCriteri;
            List<string> criteriSelezionati = new List<string>();

            if (ucStatoPratiche.Visible)
            {
                hdnCriterio1.Value = ucStatoPratiche.GetCriterio();
                resCriteri = ucStatoPratiche.GetValore(hdnCriterio1.Value);
                hdnValueCriterio1.Value = resCriteri[0];
                if (!(hdnCriterio1.Value == "Numero Domanda") || (hdnCriterio1.Value == "Codice Fiscale"))
                {
                    hdnValueCriterio1b.Value = resCriteri[1];
                }
                criteriSelezionati.Add(hdnCriterio1.Value);
                Session["CriteriSelezionati"] = criteriSelezionati;

            }

            if (ucStatoPratiche1.Visible)
            {

                hdnCriterio2.Value = ucStatoPratiche1.GetCriterio();
                resCriteri = ucStatoPratiche1.GetValore(hdnCriterio2.Value);
                hdnValueCriterio2.Value = resCriteri[0];
                if (!(hdnCriterio2.Value == "Numero Domanda") || (hdnCriterio2.Value == "Codice Fiscale"))
                {
                    hdnValueCriterio2b.Value = resCriteri[1];
                }
                criteriSelezionati.Add(hdnCriterio2.Value);
                Session["CriteriSelezionati"] = criteriSelezionati;
            }

            if (ucStatoPratiche2.Visible)
            {
                hdnCriterio3.Value = ucStatoPratiche2.GetCriterio();
                resCriteri = ucStatoPratiche2.GetValore(hdnCriterio3.Value);
                hdnValueCriterio3.Value = resCriteri[0];
                if (!(hdnCriterio3.Value == "Numero Domanda") || (hdnCriterio3.Value == "Codice Fiscale"))
                {
                    hdnValueCriterio3b.Value = resCriteri[1];
                }
                criteriSelezionati.Add(hdnCriterio3.Value);
                Session["CriteriSelezionati"] = criteriSelezionati;
            }

            if (ucStatoPratiche3.Visible)
            {
                hdnCriterio4.Value = ucStatoPratiche3.GetCriterio();
                resCriteri = ucStatoPratiche3.GetValore(hdnCriterio4.Value);
                hdnValueCriterio4.Value = resCriteri[0];
                if (!(hdnCriterio4.Value == "Numero Domanda") || (hdnCriterio4.Value == "Codice Fiscale"))
                {
                    hdnValueCriterio4b.Value = resCriteri[1];
                }
                criteriSelezionati.Add(hdnCriterio4.Value);
                Session["CriteriSelezionati"] = criteriSelezionati;
            }
        }

        private void SetCriterio()
        {
            List<string> criteri;
            string[] resCriteri;

            int numeroCriteriSelezionati = -1;
            string numCriteri = "";

            if (Session["FlagBack"] != null)
            {
                numeroCriteriSelezionati = (int)Session["NumeroCriteri"] - 1;
                numCriteri = numeroCriteriSelezionati.ToString();
            }
            else
            {
                numCriteri = hdnNCriteri.Value;
            }

            switch (numCriteri)
            {
                case "0":
                    ucStatoPratiche1.Visible = true;
                    hdnCriterio1.Value = ucStatoPratiche.GetCriterio();
                    resCriteri = ucStatoPratiche.GetValore(hdnCriterio1.Value);
                    hdnValueCriterio1.Value = resCriteri[0];
                    if (!(hdnCriterio1.Value == "Numero Domanda") || (hdnCriterio1.Value == "Codice Fiscale"))
                    {
                        hdnValueCriterio1b.Value = resCriteri[1];
                    }
                    criteri = RimuoviCriterio(hdnCriterio1.Value);
                    ucStatoPratiche1.LoadDdl(criteri);
                    break;

                case "1":
                    ucStatoPratiche2.Visible = true;
                    hdnCriterio2.Value = ucStatoPratiche1.GetCriterio();
                    resCriteri = ucStatoPratiche1.GetValore(hdnCriterio2.Value);
                    hdnValueCriterio2.Value = resCriteri[0];
                    if (!(hdnCriterio2.Value == "Numero Domanda") || (hdnCriterio2.Value == "Codice Fiscale"))
                    {
                        hdnValueCriterio2b.Value = resCriteri[1];
                    }
                    criteri = RimuoviCriterio(hdnCriterio2.Value);
                    ucStatoPratiche2.LoadDdl(criteri);
                    break;

                case "2":
                    ucStatoPratiche3.Visible = true;
                    hdnCriterio3.Value = ucStatoPratiche2.GetCriterio();
                    resCriteri = ucStatoPratiche2.GetValore(hdnCriterio3.Value);
                    hdnValueCriterio3.Value = resCriteri[0];
                    if (!(hdnCriterio3.Value == "Numero Domanda") || (hdnCriterio3.Value == "Codice Fiscale"))
                    {
                        hdnValueCriterio3b.Value = resCriteri[1];
                    }
                    criteri = RimuoviCriterio(hdnCriterio3.Value);
                    ucStatoPratiche3.LoadDdl(criteri);
                    break;

                case "3":
                    hdnCriterio4.Value = ucStatoPratiche3.GetCriterio();
                    resCriteri = ucStatoPratiche3.GetValore(hdnCriterio4.Value);
                    hdnValueCriterio4.Value = resCriteri[0];
                    if (!(hdnCriterio4.Value == "Numero Domanda") || (hdnCriterio4.Value == "Codice Fiscale"))
                    {
                        hdnValueCriterio4b.Value = resCriteri[1];
                    }
                    break;

            }
        }

        private List<string> RimuoviCriterio(string criterio)
        {
            List<string> criteri = (List<string>)Session["Criteri"];

            if (criterio == "") return criteri;
            else
            {
                if (criteri == null)
                {
                    criteri = GetListaCriteri();
                }
                else
                {
                    List<string> myCriteri;
                    myCriteri = criteri.ToList();
                    bool res = myCriteri.Remove(criterio);
                    criteri = myCriteri;
                    Session["Criteri"] = criteri;
                }
                return criteri;
            }
        }
        /*************************************************************************************************************/


        protected void event_RimuoviParametro(object sender, EventArgs e)
        {
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);

            ResetCriterio();
            SetCriteri();

            int numeroCriteriSelezionati = -1;
            string numCriteri = "";
            if (Session["FlagBack"] != null)
            {
                numeroCriteriSelezionati = (int)Session["NumeroCriteri"] - 1;
                numCriteri = numeroCriteriSelezionati.ToString();
            }
            else
            {
                int myNumeroCriteri;
                myNumeroCriteri = Int32.Parse(hdnNCriteri.Value);
                myNumeroCriteri = myNumeroCriteri - 1;
                numCriteri = myNumeroCriteri.ToString();
            }

            if (numCriteri == "0")
            {
                ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                List<string> criteri = CodeUtility.GetCriteriRicerca(TipoAppRuolo, Ruolo);
                Session["Criteri"] = criteri;
            }
            hdnNCriteri.Value = numCriteri;
            Session["NumeroCriteri"] = int.Parse(numCriteri);
        }
        public void DeleteCriterio(String Criterio)
        {

            switch (Criterio)
            {
                case "Numero Domanda":
                    Session.Remove("VSPNumeroDomanda");
                    break;
                case "Categoria Pensione":
                    Session.Remove("VSPCategoriaPensione");
                    break;
                case "Data Elaborazione":
                    Session.Remove("VSPDataElaborazioneDal");
                    Session.Remove("VSPDataElaborazioneAl");
                    break;
                case "Data Presentazione":
                    Session.Remove("VSPDataPresentazioneDal");
                    Session.Remove("VSPDataPresentazioneAl");
                    break;
                case "Fondo":
                    Session.Remove("VSPFondo");
                    break;
                case "Cassa":
                    Session.Remove("VSPCassa");
                    break;
                case "Sede":
                    Session.Remove("VSPSede");
                    break;
                case "Stato Pratica":
                    Session.Remove("VSPStatoPratica");
                    break;
                case "Codice Fiscale":
                    Session.Remove("VSPCodiceFiscale");
                    break;
                case "Anagrafica":
                    Session.Remove("VSPCognome");
                    Session.Remove("VSPNome");
                    break;
                case "Matricola":
                    Session.Remove("VSPMatricola");
                    break;
                case "PL/TRF e/o RIC in lavorazione":
                    Session.Remove("VSPTipoDomandaInLavorazione");
                    break;
                case "PL/TRF e/o RIC lavorate":
                    Session.Remove("VSPTipoDomandaLavorata");
                    break;
                case "Gruppo":
                    Session.Remove("VSPGruppo");
                    break;
                case "Prodotto":
                    Session.Remove("VSPProdotto");
                    break;
                case "Tipo":
                    Session.Remove("VSPTipo");
                    break;
            }
            return;
        }

        private void ResetCriterio()
        {
            List<string> criteri = (List<string>)Session["Criteri"];
            Session["CriteriComplete"] = criteri;

            int numeroCriteriSelezionati = -1;
            string numCriteri = "";
            List<string> criteriSelezionati = new List<string>();

            if (Session["FlagBack"] != null)
            {
                criteriSelezionati = (List<string>)Session["CriteriSelezionati"];
                numeroCriteriSelezionati = (int)Session["NumeroCriteri"] - 1;
                numCriteri = numeroCriteriSelezionati.ToString();
            }
            else
            {
                numCriteri = hdnNCriteri.Value;
            }

            switch (numCriteri)
            {
                case "0":
                    if (Session["FlagBack"] != null)
                    {
                        for (int i = criteriSelezionati.Count - 1; i >= 0; i--)
                        {
                            if (criteriSelezionati[i] != null)
                            {
                                hdnCriterio1.Value = criteriSelezionati[i];
                                criteriSelezionati[i] = null;
                                criteri = AggiungiCriterio(hdnCriterio1.Value);
                                ucStatoPratiche.Visible = false;
                                hdnCriterio1.Value = "";
                                hdnValueCriterio1.Value = "";
                                hdnValueCriterio1b.Value = "";
                                ucStatoPratiche.FindControl("btnAggiungi").Visible = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        hdnCriterio1.Value = ucStatoPratiche.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio1.Value);
                        ucStatoPratiche.Visible = false;
                        hdnCriterio1.Value = "";
                        hdnValueCriterio1.Value = "";
                        hdnValueCriterio1b.Value = "";
                        ucStatoPratiche.FindControl("btnAggiungi").Visible = true;
                        break;
                    }
                    break;
                case "1":
                    if (Session["FlagBack"] != null)
                    {
                        for (int i = criteriSelezionati.Count - 1; i >= 0; i--)
                        {
                            if (criteriSelezionati[i] != null)
                            {
                                hdnCriterio2.Value = criteriSelezionati[i];
                                criteriSelezionati[i] = null;
                                criteri = AggiungiCriterio(hdnCriterio2.Value);
                                if (i - 1 >= 0 && criteriSelezionati[i - 1] != null)
                                {
                                    hdnCriterio1.Value = criteriSelezionati[i - 1];
                                    criteri = AggiungiCriterio(hdnCriterio1.Value);
                                }
                                ucStatoPratiche1.Visible = false;
                                hdnCriterio2.Value = "";
                                hdnValueCriterio2.Value = "";
                                hdnValueCriterio2b.Value = "";
                                ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        hdnCriterio2.Value = ucStatoPratiche1.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio2.Value);
                        hdnCriterio1.Value = ucStatoPratiche.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio1.Value);
                        ucStatoPratiche1.Visible = false;
                        hdnCriterio2.Value = "";
                        hdnValueCriterio2.Value = "";
                        hdnValueCriterio2b.Value = "";
                        ((DropDownList)ucStatoPratiche.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                        break;
                    }
                    break;
                case "2":
                    if (Session["FlagBack"] != null)
                    {
                        for (int i = criteriSelezionati.Count - 1; i >= 0; i--)
                        {
                            if (criteriSelezionati[i] != null)
                            {
                                hdnCriterio3.Value = criteriSelezionati[i];
                                criteriSelezionati[i] = null;
                                criteri = AggiungiCriterio(hdnCriterio3.Value);
                                if (i - 1 >= 0 && criteriSelezionati[i - 1] != null)
                                {
                                    hdnCriterio2.Value = criteriSelezionati[i - 1];
                                    criteri = AggiungiCriterio(hdnCriterio2.Value);
                                }
                                ucStatoPratiche2.Visible = false;
                                hdnCriterio3.Value = "";
                                hdnValueCriterio3.Value = "";
                                hdnValueCriterio3b.Value = "";
                                ((DropDownList)ucStatoPratiche1.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        hdnCriterio3.Value = ucStatoPratiche2.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio3.Value);
                        hdnCriterio2.Value = ucStatoPratiche1.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio2.Value);
                        ucStatoPratiche2.Visible = false;
                        hdnCriterio3.Value = "";
                        hdnValueCriterio3.Value = "";
                        hdnValueCriterio3b.Value = "";
                        ((DropDownList)ucStatoPratiche1.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                        break;
                    }
                    break;

                case "3":
                    if (Session["FlagBack"] != null)
                    {
                        for (int i = criteriSelezionati.Count - 1; i >= 0; i--)
                        {
                            if (criteriSelezionati[i] != null)
                            {
                                hdnCriterio4.Value = criteriSelezionati[i];
                                criteriSelezionati[i] = null;
                                criteri = AggiungiCriterio(hdnCriterio4.Value);
                                if (i - 1 >= 0 && criteriSelezionati[i - 1] != null)
                                {
                                    hdnCriterio3.Value = criteriSelezionati[i - 1];
                                    criteri = AggiungiCriterio(hdnCriterio3.Value);
                                }
                                ucStatoPratiche3.Visible = false;
                                hdnCriterio4.Value = "";
                                hdnValueCriterio4.Value = "";
                                hdnValueCriterio4b.Value = "";
                                ((DropDownList)ucStatoPratiche2.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        hdnCriterio4.Value = ucStatoPratiche3.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio4.Value);
                        hdnCriterio3.Value = ucStatoPratiche2.GetCriterio();
                        criteri = AggiungiCriterio(hdnCriterio3.Value);
                        ucStatoPratiche3.Visible = false;
                        hdnCriterio4.Value = "";
                        hdnValueCriterio4.Value = "";
                        hdnValueCriterio4b.Value = "";
                        ((DropDownList)ucStatoPratiche2.FindControl("ddlVisualizzazioneStatoPratiche")).Enabled = true;
                        break;
                    }
                    break;
            }
        }

        private List<string> AggiungiCriterio(string criterio)
        {

            List<string> criteri = (List<string>)Session["Criteri"];
            if (criteri == null)
            {
                criteri = GetListaCriteri();
            }
            else
            {
                bool trovato = false;
                List<string> myCriteri;
                for (int i = 0; i < criteri.Count; i++)
                {
                    if (criteri[i] == criterio)
                    {
                        trovato = true;
                        break;
                    }
                    else
                    {
                        trovato = false;
                    }
                }
                if (!(trovato))
                {
                    myCriteri = criteri.ToList();
                    myCriteri.Add(criterio);
                    criteri = myCriteri;
                    Session["Criteri"] = criteri;

                }



            }
            return criteri;
        }




        private List<string> GetListaCriteri()
        {
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            List<string> criteri = CodeUtility.GetCriteriRicerca(TipoAppRuolo, Ruolo);
            Session["Criteri"] = criteri;
            return criteri;
        }

        private void GetListaCriteriComplete()
        {
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            List<string> criteri = CodeUtility.GetCriteriRicerca(TipoAppRuolo, Ruolo);
            Session["CriteriComplete"] = criteri;

        }

        private void SetRicerca()
        {
            Session.Remove("VSPNumeroDomanda");
            Session.Remove("VSPFondo");
            Session.Remove("VSPCassa");
            Session.Remove("VSPSede");
            Session.Remove("VSPStatoPratica");
            Session.Remove("VSPCategoriaPensione");
            Session.Remove("VSPDataElaborazioneDal");
            Session.Remove("VSPDataElaborazioneAl");
            Session.Remove("VSPDataPresentazioneDal");
            Session.Remove("VSPDataPresentazioneAl");
            Session.Remove("VSPCodiceFiscale");
            Session.Remove("VSPCognome");
            Session.Remove("VSPNome");
            Session.Remove("VSPMatricola");
            Session.Remove("VSPTipoDomandaInLavorazione");
            Session.Remove("VSPTipoDomandaLavorata");
            Session.Remove("VSPGruppo");
            Session.Remove("VSPProdotto");
            Session.Remove("VSPTipo");

            int numeroCriteriSelezionati = -1;

            if (Session["FlagBack"] != null)
            {
                numeroCriteriSelezionati = (int)Session["NumeroCriteri"];
            }
            else
            {
                numeroCriteriSelezionati = Int32.Parse(hdnNCriteri.Value);
            }


            for (int i = 0; i <= numeroCriteriSelezionati + 1; i++)
            {
                if (i == 0)
                {
                    SetValueRicerca(hdnCriterio1.Value, hdnValueCriterio1.Value, hdnValueCriterio1b.Value);
                }
                else if (i == 1)
                {
                    SetValueRicerca(hdnCriterio2.Value, hdnValueCriterio2.Value, hdnValueCriterio2b.Value);
                }
                else if (i == 2)
                {
                    SetValueRicerca(hdnCriterio3.Value, hdnValueCriterio3.Value, hdnValueCriterio3b.Value);
                }
                else
                {
                    SetValueRicerca(hdnCriterio4.Value, hdnValueCriterio4.Value, hdnValueCriterio4b.Value);
                }

            }
        }
        private void SetValueRicerca(String criterio, String valore, String valore2)
        {

            switch (criterio)
            {
                case "Numero Domanda":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.NumeroDomanda;
                    StatoPratica.NumeroDomanda = valore;
                    Session["VSPNumeroDomanda"] = valore;
                    break;
                case "Categoria Pensione":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.CategoriaPensione;
                    StatoPratica.CategoriaPensione = valore2;
                    Session["VSPCategoriaPensione"] = valore2;
                    break;
                case "Data Elaborazione":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.DataElaborazione;
                    StatoPratica.DataElaborazioneMin = valore;
                    Session["VSPDataElaborazioneDal"] = valore;
                    StatoPratica.DataElaborazioneMax = valore2;
                    Session["VSPDataElaborazioneAl"] = valore2;
                    break;
                case "Data Presentazione":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.DataPresentazione;
                    StatoPratica.DataPresentazioneMin = valore;
                    Session["VSPDataPresentazioneDal"] = valore;
                    StatoPratica.DataPresentazioneMax = valore2;
                    Session["VSPDataPresentazioneAl"] = valore2;
                    break;
                case "Fondo":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Fondo;
                    StatoPratica.Fondo = valore;
                    Session["VSPFondo"] = valore2;
                    break;
                case "Cassa":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Cassa;
                    StatoPratica.Cassa = valore;
                    Session["VSPCassa"] = valore2;
                    break;
                case "Sede":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Sede;
                    StatoPratica.Sede = valore;
                    Session["VSPSede"] = valore2;
                    break;
                case "Stato Pratica":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.StatoPratica;
                    StatoPratica.SPratica = Int16.Parse(valore);
                    Session["VSPStatoPratica"] = valore2;
                    break;
                case "Codice Fiscale":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.CodiceFiscale;
                    StatoPratica.CodiceFiscale = valore;
                    Session["VSPCodiceFiscale"] = valore;
                    break;
                case "Anagrafica":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Anagrafica;
                    StatoPratica.Cognome = valore;
                    Session["VSPCognome"] = valore;
                    StatoPratica.Nome = valore2;
                    Session["VSPNome"] = valore2;
                    break;
                case "Matricola":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Matricola;
                    StatoPratica.Matricola = valore;
                    Session["VSPMatricola"] = valore;
                    break;
                case "PL/TRF e/o RIC in lavorazione":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.TipoDomandaInLavorazione;
                    switch (valore)
                    {
                        case "ALL":
                            StatoPratica.TipoDomandaInLavorazione = GestioneStatoPraticaTipoDomanda.PL_RIC;
                            break;
                        case "PL":
                            StatoPratica.TipoDomandaInLavorazione = GestioneStatoPraticaTipoDomanda.PL;
                            break;
                        case "RIC":
                            StatoPratica.TipoDomandaInLavorazione = GestioneStatoPraticaTipoDomanda.RIC;
                            break;
                    }
                    Session["VSPTipoDomandaInLavorazione"] = valore;
                    break;
                case "PL/TRF e/o RIC lavorate":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.TipoDomandaLavorata;
                    switch (valore)
                    {
                        case "ALL":
                            StatoPratica.TipoDomandaLavorata = GestioneStatoPraticaTipoDomanda.PL_RIC;
                            break;
                        case "PL":
                            StatoPratica.TipoDomandaLavorata = GestioneStatoPraticaTipoDomanda.PL;
                            break;
                        case "RIC":
                            StatoPratica.TipoDomandaLavorata = GestioneStatoPraticaTipoDomanda.RIC;
                            break;
                    }
                    Session["VSPTipoDomandaLavorata"] = valore;
                    break;
                case "Gruppo":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Gruppo;
                    StatoPratica.Gruppo = valore;
                    Session["VSPGruppo"] = valore;
                    break;
                case "Prodotto":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Prodotto;
                    StatoPratica.Prodotto = valore;
                    Session["VSPProdotto"] = valore;
                    break;
                case "Tipo":
                    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.Tipo;
                    StatoPratica.Tipo = valore;
                    Session["VSPTipo"] = valore;
                    break;
                    //case "Periodo Giacenza":
                    //    StatoPratica.Criterio = Utility.CriterioRicercaStatoPratica.PeriodoGiacenza;
                    //    StatoPratica.PeriodoGiacenza = valore;
                    //    break;
            }
        }


        protected void btnAnnulla_Click(object sender, EventArgs e)
        {
            Session["FlagAnnulla"] = true;
            Response.Redirect("VisualizzazioneStatoPratiche.aspx");
        }


        private string GetSedeCurrentOffice()
        {
            string currentSede = string.Empty;

            try
            {
                var i = (from o in INPS.DNA.Context.OfficeList.Offices
                         where o.Value.AspnCode == INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.AspnCode
                         select o).FirstOrDefault();
                currentSede = string.Format("{0}-{1}", i.Value.AspnCode, (i.Value.ExtendedProperties != null ? i.Value.ExtendedProperties["SEDE"].Trim() : i.Value.Name.Trim()));
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }

            return currentSede;
        }
    }

}
