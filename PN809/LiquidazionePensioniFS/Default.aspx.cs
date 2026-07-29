using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Any, CheckSequenceOnPostBack = false)]
    public partial class Default : BasePage, IHomePage
    {
        #region IAvvisiMessaggi
        public AreaHomepage AreaHomePage { get; set; }
        public UtilityTipoAppartenenza? TipoApp { get; set; }
        #endregion IAvvisiMessaggi

        #region IViewUI Members
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI Members

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();

                Session.Add("SessionAlive", true);

                if (Session["Ruolo"] == null)
                {
                    if (CodeUtility.IsMultiRuolo())
                    {
                        Response.Redirect("~/SceltaRuolo.aspx", true);
                    }
                    else
                    {
                        Dictionary<string, string> ruoliAbilitati = CodeUtility.GetRuoliAbilitati();
                        if (ruoliAbilitati != null && ruoliAbilitati.Count > 0)
                            Session["Ruolo"] = (Ruoli)Enum.Parse(typeof(Ruoli), ruoliAbilitati.FirstOrDefault().Key);
                    }
                }

                if (Session["Ruolo"] != null)
                {
                    if (CodeUtility.IsAmministratore(Session["Ruolo"]))
                        pnlTabAggiornamenti.Visible = true;
                    else
                        pnlTabAggiornamenti.Visible = false;
                }
                else
                {
                    Session.Add(CodeUtility.EnumSession.Courtesy_Type.ToString(), CodeUtility.CourtesyType.RuoloNonAbilitato);
                    Response.Redirect("~/Courtesy.aspx", true);
                }

                PresenterMenuLeft presenter = new PresenterMenuLeft();
                this.TipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);

                presenter.GetAreaAvvisiMessaggi(this);
                CodeUtility.SetAvvisi(this.AreaHomePage.AreaAvvisi);
                CodeUtility.SetMessaggiHermes(this.AreaHomePage.AreaMessaggiHermes);
                CodeUtility.SetAggiornamenti(this.AreaHomePage.AreaAggiornamenti);
            }

            if (ConfigurationManager.AppSettings["MostraSceltaSirio"] != null &&
            ConfigurationManager.AppSettings["MostraSceltaSirio"] == "SI")
            {
                pnlSceltaTema.Visible = true;
                string tema = this.Page.Theme;
                if (tema == "iFrame")
                {
                    lblTema.Text = "Torna alla precedente versione dell'applicativo";
                    hTema.Value = "BlueINPS1";
                }
                else
                {
                    lblTema.Text = "Prova la nuova versione di Liquidazione Pensioni";
                    hTema.Value = "iFrame";
                }
            }
        }

        [System.Web.Services.WebMethod(true)]
        [System.Web.Script.Services.ScriptMethod()]
        public static Versione[] LoadVersioni()
        {
            List<Versione> versioni = new List<Versione>();
            Dictionary<string, string> listaVersioni = CodeUtility.GetVersioni();

            if (listaVersioni != null)
            {
                versioni.Add(new Versione(listaVersioni["VersioneWA"]));
                versioni.Add(new Versione(listaVersioni["VersioneWCF"]));

                if (System.Web.HttpContext.Current.Session["Ruolo"] != null)
                {
                    switch (Utility.GetTipoAppartenenzaRuolo(System.Web.HttpContext.Current.Session["Ruolo"]))
                    {
                        case TipoAppartenenzaRuolo.AGO:
                            versioni.Add(new Versione(listaVersioni["VersioneWCFAGO"]));
                            break;
                        case TipoAppartenenzaRuolo.FS:

                            versioni.Add(new Versione(listaVersioni["VersioneWCFFS"]));
                            break;
                        case TipoAppartenenzaRuolo.CI:
                            versioni.Add(new Versione(listaVersioni["VersioneWCFCI"]));
                            break;
                    }
                }
            }

            return versioni.ToArray();
        }

        [System.Web.Services.WebMethod(true)]
        [System.Web.Script.Services.ScriptMethod()]
        public static AvvisiArray[] LoadAvvisi()
        {
            List<AvvisiArray> avvisi = new List<AvvisiArray>();
            List<Presenter.SvrLiquidazione.Avvisi> elencoAvvisi = CodeUtility.GetAvvisi();

            if (elencoAvvisi != null && elencoAvvisi.Count > 0)
            {
                foreach (Presenter.SvrLiquidazione.Avvisi a in elencoAvvisi)
                {
                    if (a.Attivo)
                        avvisi.Add(new AvvisiArray(a));
                }
            }

            return avvisi.ToArray();
        }

        [System.Web.Services.WebMethod(true)]
        [System.Web.Script.Services.ScriptMethod()]
        public static MessaggiHermesArray[] LoadMessaggiHermes()
        {
            List<MessaggiHermesArray> messaggiHermes = new List<MessaggiHermesArray>();
            List<Presenter.SvrLiquidazione.MessaggiHermes> elencoMessaggiHermes = CodeUtility.GetMessaggiHermes();

            if (elencoMessaggiHermes != null && elencoMessaggiHermes.Count > 0)
            {
                foreach (Presenter.SvrLiquidazione.MessaggiHermes m in elencoMessaggiHermes)
                {
                    if (m.Attivo)
                        messaggiHermes.Add(new MessaggiHermesArray(m));
                }
            }
            return messaggiHermes.ToArray();
        }

        [System.Web.Services.WebMethod(true)]
        [System.Web.Script.Services.ScriptMethod()]
        public static AggiornamentiArray[] LoadAggiornamenti()
        {
            List<AggiornamentiArray> aggiornamenti = new List<AggiornamentiArray>();
            List<Presenter.SvrLiquidazione.Aggiornamenti> elencoAggiornamenti = CodeUtility.GetAggiornamenti();

            if (elencoAggiornamenti != null && elencoAggiornamenti.Count > 0)
            {
                foreach (Presenter.SvrLiquidazione.Aggiornamenti a in elencoAggiornamenti)
                {
                    if (a.Attivo)
                        aggiornamenti.Add(new AggiornamentiArray(a));
                }
            }

            return aggiornamenti.ToArray();
        }

        protected void btnScegliTema_Click(object sender, EventArgs e)
        {
            string tema = this.Page.Theme;
            if (hTema.Value == "iFrame")
                Session["isSirio"] = true;
            else
                Session["isSirio"] = false;

            Response.Redirect(Request.RawUrl); // ricarica la pagina
        }
    }

    [Serializable]
    public class Versione
    {
        public string Titolo { get; set; }

        internal Versione(string titolo)
        {
            this.Titolo = titolo;
        }
    }

    [Serializable]
    public class AvvisiArray
    {
        public string Titolo { get; set; }
        public string Testo { get; set; }

        internal AvvisiArray(string titolo, string testo)
        {
            this.Titolo = titolo;
            this.Testo = testo;
        }

        internal AvvisiArray(Presenter.SvrLiquidazione.Avvisi avviso)
        {
            this.Titolo = avviso.TimeStamp.ToString("dd/MM/yyyy") + " - " + avviso.Titolo;
            this.Testo = avviso.Testo;
        }
    }

    [Serializable]
    public class MessaggiHermesArray
    {
        public string Titolo { get; set; }
        public string Testo { get; set; }
        public string Url { get; set; }
        public string Categoria { get; set; }

        internal MessaggiHermesArray(string titolo, string testo, string url, string categoria)
        {
            this.Titolo = titolo;
            this.Testo = testo;
            this.Url = url;
            this.Categoria = categoria;
        }

        internal MessaggiHermesArray(Presenter.SvrLiquidazione.MessaggiHermes messaggioHermes)
        {
            this.Titolo = messaggioHermes.TimeStamp.ToString("dd/MM/yyyy") + " - " + messaggioHermes.Titolo;
            this.Testo = messaggioHermes.Testo +
                //"<BR><span style='font-size: 12px'>Tipologia: " + messaggioHermes.Categoria + "</span>" +
                "<BR><a style='font-size: 12px' href=\"" + messaggioHermes.Url + "\" target=\"_blank\">Apri " + messaggioHermes.Categoria.ToLowerInvariant() + "</a>";
            this.Url = messaggioHermes.Url;
            this.Categoria = messaggioHermes.Categoria;
        }
    }

    [Serializable]
    public class AggiornamentiArray
    {
        public string Titolo { get; set; }
        public string Testo { get; set; }

        internal AggiornamentiArray(string titolo, string testo)
        {
            this.Titolo = titolo;
            this.Testo = testo;
        }

        internal AggiornamentiArray(Presenter.SvrLiquidazione.Aggiornamenti aggiornamento)
        {
            this.Titolo = aggiornamento.TimeStamp.ToString("dd/MM/yyyy") + " - " + aggiornamento.Titolo;
            this.Testo = aggiornamento.Testo;
        }
    }
}
