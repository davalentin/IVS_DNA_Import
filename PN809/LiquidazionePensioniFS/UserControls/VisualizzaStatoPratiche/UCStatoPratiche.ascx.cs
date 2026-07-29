using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.VisualizzaStatoPratiche
{
    public partial class UCStatoPratiche : CustomBaseUserControl, IStatoPratiche, ISedi
    {
        #region ISedi Members
        public string CommaSeparatedSedi { get; set; }
        public Dictionary<string, string> DictionaryOfficeList { get; set; }
        public string Sede { get; set; }
        public List<string> SediAbilitate { get; set; }
        public INPS.DNA.Office SelectedOffice { get; set; }
        #endregion ISedi Members

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
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            if (!Page.IsPostBack)
            {
                if (Session["FlagBack"] == null)
                {
                    List<string> criteri = (List<string>)Session["Criteri"];
                    LoadUserControl(criteri);
                }
            }

            ddlVisualizzazioneStatoPratiche.Attributes.Add("onChange", "javascript:ddlScelta('" + this.ClientID + "');");
        }

        public void LoadUserControl(List<string> criteri)
        {
            if (criteri == null)
            {
                criteri = ucGetListaCriteri();
                Session["Criteri"] = criteri;
                Session["CriteriComplete"] = criteri;
            }

            ddlCategoriaPensione.Items.Clear();
            ddlFondo.Items.Clear();
            ddlCassa.Items.Clear();
            ddlStatoPratica.Items.Clear();
            ddlGruppo.Items.Clear();
            ddlProdotto.Items.Clear();
            ddlTipo.Items.Clear();

            CodeUtility valuesDecodifica = new CodeUtility();
            AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();

            LoadCategoriePensione(valoriDecodificati);

            AreaDecodifica.DatiStatoPensione[] listaStatiPensione = valoriDecodificati.ElencoStatiPensione;

            //Rimuovo lo stato CALCOLATA NO ONERI, CALCOLATA NO SAI e CALCOLATA NO TOTAL per domande non AGO
            this.TipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            if (this.TipoAppRuolo != UtilityTipoAppartenenza.AGO)
            {
                List<AreaDecodifica.DatiStatoPensione> appList = listaStatiPensione.ToList();
                appList.RemoveAll(x => (x.CodiceStato == "10" || x.CodiceStato == "11" || x.CodiceStato == "13"));
                listaStatiPensione = appList.ToArray();
            }

            //Rimuovo lo stato CALCOLATA NO STAZ.LAVORO per domande non CI
            if (this.TipoAppRuolo != UtilityTipoAppartenenza.CI)
            {
                List<AreaDecodifica.DatiStatoPensione> appList = listaStatiPensione.ToList();
                appList.RemoveAll(x => (x.CodiceStato == "12"));
                listaStatiPensione = appList.ToArray();
            }

            foreach (AreaDecodifica.DatiStatoPensione statoPensione in listaStatiPensione)
            {
                CodeUtility.SetValueDdl(ddlStatoPratica, statoPensione.DecodificaStato, statoPensione.CodiceStato);
            }
            ddlStatoPratica.Items.Remove(ddlStatoPratica.Items.FindByValue("0"));

            AreaDecodifica.DatiFondoPensione[] listaFondiPensione = valoriDecodificati.ElencoFondiPensione;

            foreach (AreaDecodifica.DatiFondoPensione fondo in listaFondiPensione)
            {
                CodeUtility.SetValueDdl(ddlFondo, fondo.DescFondo, fondo.CodFondo);
            }

            AreaDecodifica.DatiFondoPensione[] listaCasseGDP = valoriDecodificati.ElencoCasseGDP;

            if (listaCasseGDP != null)
            {
                foreach (AreaDecodifica.DatiFondoPensione cassa in listaCasseGDP)
                {
                    CodeUtility.SetValueDdl(ddlCassa, cassa.DescFondo, cassa.CodFondo);
                }
            }

            AreaDecodifica.DatiRicercaGPT[] listaRicercaGPT = valoriDecodificati.ElencoRicercaGPT;

            if (listaRicercaGPT != null)
            {
                foreach (AreaDecodifica.DatiRicercaGPT ricerca in listaRicercaGPT)
                {
                    switch (ricerca.GPT)
                    {
                        case 'G':
                            CodeUtility.SetValueDdl(ddlGruppo, ricerca.Codice, ricerca.Codice);
                            break;
                        case 'P':
                            CodeUtility.SetValueDdl(ddlProdotto, ricerca.Codice, ricerca.Codice);
                            break;
                        case 'T':
                            CodeUtility.SetValueDdl(ddlTipo, ricerca.Codice, ricerca.Codice);
                            break;
                    }
                }
            }

            LoadMatricola();

            LoadDdl(criteri);
            HiddenFieldSedi.Value = CodeUtility.LoadSediECo();
        }

        private void LoadCategoriePensione(AreaDecodifica valoriDecodificati)
        {
            List<string> listaCatAmmesse = CodeUtility.GetCategoriePensione(valoriDecodificati, Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]));
            if (listaCatAmmesse != null && listaCatAmmesse.Count > 0)
            {
                listaCatAmmesse.Sort((x, y) => string.Compare(x, y, false, System.Globalization.CultureInfo.CurrentUICulture));

                foreach (string categoria in listaCatAmmesse)
                {
                    if (ddlCategoriaPensione.Items == null || !ddlCategoriaPensione.Items.Contains(new ListItem(categoria, categoria)))
                        CodeUtility.SetValueDdl(ddlCategoriaPensione, categoria, categoria);
                }
            }
        }


        public void LoadCriteriRicerca(List<string> criteri)
        {
            List<string> criteriSelezionati = (List<string>)Session["CriteriSelezionati"];

            if (criteriSelezionati != null)
            {
                for (int i = 0; i < criteriSelezionati.Count; i++)
                {
                    if (Session["VSPNumeroDomanda"] != null)
                    {
                        if (criteriSelezionati[i] == "Numero Domanda")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtNumeroDomanda.Visible = true;
                                    txtNumeroDomanda.Text = Session["VSPNumeroDomanda"].ToString();
                                    Session.Remove("VSPNumeroDomanda");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPFondo"] != null)
                    {
                        if (criteriSelezionati[i] == "Fondo")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlFondo.Visible = true;
                                    int currIndex = ddlFondo.Items.IndexOf((ListItem)ddlFondo.Items.FindByText((string)Session["VSPFondo"]));
                                    ddlFondo.SelectedIndex = currIndex;
                                    Session.Remove("VSPFondo");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPCassa"] != null)
                    {
                        if (criteriSelezionati[i] == "Cassa")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlCassa.Visible = true;
                                    int currIndex = ddlCassa.Items.IndexOf((ListItem)ddlCassa.Items.FindByText((string)Session["VSPCassa"]));
                                    ddlCassa.SelectedIndex = currIndex;
                                    Session.Remove("VSPCassa");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPSede"] != null)
                    {
                        if (criteriSelezionati[i] == "Sede")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtSede.Visible = true;
                                    txtSede.Text = Session["VSPSede"].ToString();
                                    Session.Remove("VSPSede");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPStatoPratica"] != null)
                    {
                        if (criteriSelezionati[i] == "Stato Pratica")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlStatoPratica.Visible = true;
                                    int currIndex = ddlStatoPratica.Items.IndexOf((ListItem)ddlStatoPratica.Items.FindByText((string)Session["VSPStatoPratica"]));
                                    ddlStatoPratica.SelectedIndex = currIndex;
                                    Session.Remove("VSPStatoPratica");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPCategoriaPensione"] != null)
                    {
                        if (criteriSelezionati[i] == "Categoria Pensione")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlCategoriaPensione.Visible = true;
                                    int currIndex = ddlCategoriaPensione.Items.IndexOf((ListItem)ddlCategoriaPensione.Items.FindByText((string)Session["VSPCategoriaPensione"]));
                                    ddlCategoriaPensione.SelectedIndex = currIndex;
                                    Session.Remove("VSPCategoriaPensione");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPDataElaborazioneDal"] != null && Session["VSPDataElaborazioneAl"] != null)
                    {
                        if (criteriSelezionati[i] == "Data Elaborazione")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtDataElaborazioneMin.Visible = true;
                                    txtDataElaborazioneMin.Text = Session["VSPDataElaborazioneDal"].ToString();
                                    Session.Remove("VSPDataElaborazioneDal");
                                    txtDataElaborazioneMax.Visible = true;
                                    txtDataElaborazioneMax.Text = Session["VSPDataElaborazioneAl"].ToString();
                                    Session.Remove("VSPDataElaborazioneAl");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPDataPresentazioneDal"] != null && Session["VSPDataPresentazioneAl"] != null)
                    {
                        if (criteriSelezionati[i] == "Data Presentazione")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtDataPresentazioneMin.Visible = true;
                                    txtDataPresentazioneMin.Text = Session["VSPDataPresentazioneDal"].ToString();
                                    Session.Remove("VSPDataPresentazioneDal");
                                    txtDataPresentazioneMax.Visible = true;
                                    txtDataPresentazioneMax.Text = Session["VSPDataPresentazioneAl"].ToString();
                                    Session.Remove("VSPDataPresentazioneAl");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPCodiceFiscale"] != null)
                    {
                        if (criteriSelezionati[i] == "Codice Fiscale")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtCodiceFiscale.Visible = true;
                                    txtCodiceFiscale.Text = Session["VSPCodiceFiscale"].ToString();
                                    Session.Remove("VSPCodiceFiscale");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPCognome"] != null && Session["VSPNome"] != null)
                    {
                        if (criteriSelezionati[i] == "Anagrafica")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtCognome.Visible = true;
                                    txtCognome.Text = Session["VSPCognome"].ToString();
                                    Session.Remove("VSPCognome");
                                    txtNome.Visible = true;
                                    txtNome.Text = Session["VSPNome"].ToString();
                                    Session.Remove("VSPNome");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPMatricola"] != null)
                    {
                        if (criteriSelezionati[i] == "Matricola")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    txtMatricola.Visible = true;
                                    txtMatricola.Text = Session["VSPMatricola"].ToString();
                                    Session.Remove("VSPMatricola");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPTipoDomandaInLavorazione"] != null)
                    {
                        if (criteriSelezionati[i] == "PL/TRF e/o RIC in lavorazione")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlTipoDomandaInLavorazione.Visible = true;
                                    int currIndex = ddlTipoDomandaInLavorazione.Items.IndexOf((ListItem)ddlTipoDomandaInLavorazione.Items.FindByValue((string)Session["VSPTipoDomandaInLavorazione"]));
                                    ddlTipoDomandaInLavorazione.SelectedIndex = currIndex;
                                    Session.Remove("VSPTipoDomandaInLavorazione");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPTipoDomandaLavorata"] != null)
                    {
                        if (criteriSelezionati[i] == "PL/TRF e/o RIC lavorate")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlTipoDomandaLavorata.Visible = true;
                                    int currIndex = ddlTipoDomandaLavorata.Items.IndexOf((ListItem)ddlTipoDomandaLavorata.Items.FindByValue((string)Session["VSPTipoDomandaLavorata"]));
                                    ddlTipoDomandaLavorata.SelectedIndex = currIndex;
                                    Session.Remove("VSPTipoDomandaLavorata");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPGruppo"] != null)
                    {
                        if (criteriSelezionati[i] == "Gruppo")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlGruppo.Visible = true;
                                    int currIndex = ddlGruppo.Items.IndexOf((ListItem)ddlGruppo.Items.FindByText((string)Session["VSPGruppo"]));
                                    ddlGruppo.SelectedIndex = currIndex;
                                    Session.Remove("VSPGruppo");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPProdotto"] != null)
                    {
                        if (criteriSelezionati[i] == "Prodotto")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlProdotto.Visible = true;
                                    int currIndex = ddlProdotto.Items.IndexOf((ListItem)ddlProdotto.Items.FindByText((string)Session["VSPProdotto"]));
                                    ddlProdotto.SelectedIndex = currIndex;
                                    Session.Remove("VSPProdotto");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }

                    if (Session["VSPTipo"] != null)
                    {
                        if (criteriSelezionati[i] == "Tipo")
                        {
                            for (int j = 0; j < criteri.Count; j++)
                            {
                                if (criteri[j] == criteriSelezionati[i])
                                {
                                    ddlVisualizzazioneStatoPratiche.SelectedIndex = j;
                                    ddlTipo.Visible = true;
                                    int currIndex = ddlTipo.Items.IndexOf((ListItem)ddlTipo.Items.FindByText((string)Session["VSPTipo"]));
                                    ddlTipo.SelectedIndex = currIndex;
                                    Session.Remove("VSPTipo");
                                    List<string> criteriList = new List<string>(criteri);
                                    criteriList.Remove(criteri[j]);
                                    criteri = criteriList;
                                    criteri.Sort();
                                    Session["CriteriComplete"] = criteri;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<string> ucGetListaCriteri()
        {
            UtilityTipoAppartenenza tipoAppRuolo = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
            this.Ruolo = CodeUtility.GetRuolo(Session["Ruolo"]);
            List<string> criteri = CodeUtility.GetCriteriRicerca(tipoAppRuolo, this.Ruolo);

            criteri.Sort();
            Session["Criteri"] = criteri;
            Session["CriteriComplete"] = criteri;
            return criteri;
        }

        public event EventHandler AggiungiParametro;

        public event EventHandler RimuoviParametro;

        protected void AddParametro(object sender, EventArgs e)
        {

            string criterio = ucHdnNCriteri.Value;
            AggiungiParametro(this, null);
            ucHdnNCriteri.Value = (Int32.Parse(criterio) + 1).ToString();
        }

        protected void RemoveParametro(object sender, EventArgs e)
        {
            RimuoviParametro(this, null);
        }


        internal string GetCriterio()
        {
            return ddlVisualizzazioneStatoPratiche.SelectedItem.Text;
        }

        internal string[] GetValore(string criterio)
        {
            string[] resultCriterio = { "", "" };
            switch (criterio)
            {
                case "Numero Domanda":
                    resultCriterio[0] = txtNumeroDomanda.Text;
                    break;
                case "Categoria Pensione":
                    resultCriterio[0] = ddlCategoriaPensione.SelectedItem.Value;
                    resultCriterio[1] = ddlCategoriaPensione.SelectedItem.Text;
                    break;
                case "Data Elaborazione":
                    resultCriterio[0] = txtDataElaborazioneMin.Text;
                    resultCriterio[1] = txtDataElaborazioneMax.Text;
                    break;
                case "Data Presentazione":
                    resultCriterio[0] = txtDataPresentazioneMin.Text;
                    resultCriterio[1] = txtDataPresentazioneMax.Text;
                    break;
                case "Fondo":
                    resultCriterio[0] = ddlFondo.SelectedItem.Value;
                    resultCriterio[1] = ddlFondo.SelectedItem.Text;
                    break;
                case "Cassa":
                    resultCriterio[0] = ddlCassa.SelectedItem.Value;
                    resultCriterio[1] = ddlCassa.SelectedItem.Text;
                    break;
                case "Sede":
                    string sedeToSplit = txtSede.Text;
                    string[] sede = sedeToSplit.Split('-');
                    resultCriterio[0] = sede[0];
                    resultCriterio[1] = txtSede.Text;
                    break;
                case "Stato Pratica":
                    resultCriterio[0] = ddlStatoPratica.SelectedItem.Value;
                    resultCriterio[1] = ddlStatoPratica.SelectedItem.Text;
                    break;
                case "Codice Fiscale":
                    resultCriterio[0] = txtCodiceFiscale.Text;
                    break;
                case "Anagrafica":
                    resultCriterio[0] = txtCognome.Text;
                    resultCriterio[1] = txtNome.Text;
                    break;
                case "Matricola":
                    resultCriterio[0] = txtMatricola.Text;
                    break;
                case "PL/TRF e/o RIC in lavorazione":
                    resultCriterio[0] = ddlTipoDomandaInLavorazione.SelectedItem.Value;
                    break;
                case "PL/TRF e/o RIC lavorate":
                    resultCriterio[0] = ddlTipoDomandaLavorata.SelectedItem.Value;
                    break;
                case "Gruppo":
                    resultCriterio[0] = ddlGruppo.SelectedItem.Value;
                    resultCriterio[1] = ddlGruppo.SelectedItem.Text;
                    break;
                case "Prodotto":
                    resultCriterio[0] = ddlProdotto.SelectedItem.Value;
                    resultCriterio[1] = ddlProdotto.SelectedItem.Text;
                    break;
                case "Tipo":
                    resultCriterio[0] = ddlTipo.SelectedItem.Value;
                    resultCriterio[1] = ddlTipo.SelectedItem.Text;
                    break;
            }
            return resultCriterio;
        }

        internal void LoadDdl(List<string> valori)
        {
            ddlVisualizzazioneStatoPratiche.Items.Clear();
            valori.Sort();
            foreach (string valore in valori)
            {
                ListItem li = new ListItem();
                li.Text = valore;
                li.Value = valore.Replace(" ", "");
                ddlVisualizzazioneStatoPratiche.Items.Add(li);

            }
        }

        internal void LoadMatricola()
        {
            HiddenFieldMatricolaValue.Value = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            if (CodeUtility.IsAmministratore(Session["Ruolo"]) || CodeUtility.IsDirettore_RdP(Session["Ruolo"]))
                HiddenFieldMatricolaEnabled.Value = "true";
            else
                HiddenFieldMatricolaEnabled.Value = "false";
        }
    }
}