using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;
using System.Data;
using System.Diagnostics;

using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;



namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class RisultatoVisualizzaStatoPratiche : CustomBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
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
                Session.Remove("Lavorabile");           //flag per determinare se la pensione è lavorabile
            }
        }

        protected void event_ucEliminaPratica(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Messaggio = "Pratica eliminata correttamente";
            ucAvviso.Visible = true;
            
        }

        protected void event_ReloadUChangeSede(object sender, EventArgs e)
        {
            ReloadUChangeSede();
        }
    }
}
