using INPS.Pensioni.LiquidazionePensione.Presenter.Contract.AggiornaCalcoloNoInd;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.InterfacceViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAggiornaCalcoloNoInd
    {
        IAggiornaCalcoloNoInd view;
        const string MessaggioBloccoIvs = "Attenzione, per il debito da 'Ricostituzione Online' indicato non è previsto accodo del TE08IND e rispettiva selezione delle causali per motivi ostativi stabiliti dal calcolo:";

        public PresenterAggiornaCalcoloNoInd(IAggiornaCalcoloNoInd view)
        {
            this.view = view;
        }

        //Sceglie il pannello da mostrare fra ElencoCausaliDebito e ValutazioneEventualeScelta
        //in base allo stato della domanda (CALCOLO NO INDEB o CALCOLO NO INDEB WAIT)
        public void SceltaPannello()
        {
            switch (view.domanda.Stato)
            {
                case "CALCOLO NO INDEB":
                    view.MostraElencoCasualiDebito();
                    break;
                case "CALCOLO NO INDEB WAIT":
                    view.MessaggioCodaPannelloValutazioneEventualeScelta = GetMessaggioCodaPannelloEventualeScelta();
                    view.MostraValutazioneEventualeScelta();
                    break;
            }
        }

        public void CaricaCasuali()
        {
            RootIndebitoDto tempIndebito;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            objWS.GetAnteprimaDebito(out tempIndebito, long.Parse(view.domanda.NumeroDomanda), view.domanda.MatricolaUtenteAcquisizione);

            view.Indebito = tempIndebito;
        }

        public bool ControllaBloccoValidazioneCausaliByControlloDinamico()
        {
            view.LeggiDatiPensioneDaSessione();

            // Null-check espliciti
            if (view == null || view.datiPensione == null) 
                return false;

            AreaTitolare.DatiPensione.TipoAppDomanda? tipo = view.datiPensione.TipoAppartenenzaDomanda;
            if (!tipo.HasValue)
                return false;

            // Mappa il tipo al nome del controllo con uno switch, evitando duplicazione logica
            string nomeControllo = GetNomeControlloByTipo(tipo.Value);
            if (string.IsNullOrEmpty(nomeControllo))
                return false;

            // Prepara DTO per la chiamata
            AreaControlliDinamici controlli = new AreaControlliDinamici();
            controlli.NomeControllo = nomeControllo;

            // WCF client
            ServizioLiquidazioneClient client = new ServizioLiquidazioneClient();
            try
            {
                client.GetControlloDinamicoByNomeControllo(ref controlli);

                // Confronto case-insensitive (supportato in .NET 3.5)
                return string.Equals(controlli.ValoreControllo, "SI", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // In caso di errore remoto, fallback conservativo
                return false;
            }
            finally
            {
                // Chiusura sicura del client WCF
                try
                {
                    if (client != null)
                        client.Close();
                }
                catch
                {
                    try { client.Abort(); } catch {}
                }
            }
        }

        public bool ControllaBloccoValidazioneCausaliByCensimentoSedi()
        {
            view.CaricaDomandaDaSessione();

            // Null-check espliciti
            if (view == null || view.datiPensione == null)
                return true;

            string CodiceSede = Convert.ToString(view.domanda.CodiceSedeLavorazione);
            if (CodiceSede == null || CodiceSede.Trim().Length == 0)
                return true;

            // Prepara DTO per la chiamata
            AreaControlliDinamici controlli = new AreaControlliDinamici();
            controlli.NomeControllo = "ListaSediAbilitantiTe08Ind";

            // WCF client
            ServizioLiquidazioneClient client = new ServizioLiquidazioneClient();
            try
            {
                client.GetControlloDinamicoByNomeControllo(ref controlli);

                if (controlli.ValoreControllo == null || controlli.ValoreControllo.Trim().Length == 0)
                    return true;
              
                if (controlli.ValoreControllo.Equals("Tutte", StringComparison.OrdinalIgnoreCase))
                    return false;

                return !controlli.ValoreControllo.Contains(CodiceSede);
            }
            catch
            {
                // In caso di errore remoto, fallback conservativo
                return false;
            }
            finally
            {
                // Chiusura sicura del client WCF
                try
                {
                    if (client != null)
                        client.Close();
                }
                catch
                {
                    try { client.Abort(); } catch { }
                }
            }
        }

        public bool ControllaBloccoValidazioneCausali()
        {
            return (ControllaBloccoValidazioneCausaliByControlloDinamico() || ControllaBloccoValidazioneCausaliByCensimentoSedi());
        }

        public bool ControlloErroreCausatoBloccoIvs()
        {
            RootIndebitoDto indebito = view.Indebito;
            if (indebito == null || indebito.Message == null)
                return false;
            return indebito.Message.Contains(MessaggioBloccoIvs);
        }

        public List<CausaleDebito> EstraiCasualiDebito()
        {
            ContoRicDto[] contiRic = view.Indebito.Data.ContiRic;
            List<CausaleDebito> casualiDebito = new List<CausaleDebito>();
            List<CausaleDtoLite> LegendaCausaliAmmesse = new List<CausaleDtoLite>();

            foreach (ContoRicDto contoRic in contiRic)
            {
                CausaleDebito tempCausale = new CausaleDebito()
                {
                    Id = Array.IndexOf(contiRic, contoRic),
                    CausaleAnalitica = contoRic.ContoRecupero.Causale.Analitica,
                    CausaleSintetica = contoRic.ContoRecupero.Causale.Sintetica,
                    Descrizione = contoRic.ContoRecupero.Causale.Descrizione,
                    ContoRecupero = string.Format("{0} - {1}", contoRic.ContoRecupero.Codice, contoRic.ContoRecupero.Nome),
                    Importo = contoRic.ContoRecupero.Importo,
                    CasualiAmmesse = contoRic.CausaliAmmesse.Select(c => new CausaleDtoLite()
                    {
                        Analitica = c.Analitica,
                        Descrizione = c.Descrizione,
                        Sintetica = c.Sintetica
                    }).ToArray()
                };

                casualiDebito.Add(tempCausale);
                LegendaCausaliAmmesse.AddRange(tempCausale.CasualiAmmesse);
            }

            LegendaCausaliAmmesse = LegendaCausaliAmmesse.Distinct().ToList();
            view.LegendaCausaliAmmesse = LegendaCausaliAmmesse;

            return casualiDebito;
        }

        public void AggiornaSemafori()
        {
            view.LeggiInfoLiquidazione();
            view.CaricaDomandaDaSessione();
            view.PreparaAreaInfoPratica();
            view.ApplicaSemaforiUI();
        }

        public void PageLoadNoPostBack()
        {
            view.LeggiInfoLiquidazione();
            view.areaEsito = new AreaEsito()
            {
                RisultatoOperazione = AreaEsito.TipoEsito.OK,
                Messaggio = "Calcolo eseguito correttamente"
            };
            view.CaricaDomandaDaSessione();
            SceltaPannello();
        }

        public void AccogliDomanda()
        {
            try
            {
                view.CaricaDomandaDaSessione();
                view.LeggiDatiPensioneDaSessione();

                string StatoDomandaPreNotifica = view.domanda.Stato;

                ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                bool flagCi = view.datiPensione.TipoAppartenenzaDomanda.Equals(AreaTitolare.DatiPensione.TipoAppDomanda.CI);
                AreaEsito esito = objWS.NotificaTE08(long.Parse(view.domanda.NumeroDomanda), view.domanda.MatricolaUtenteAcquisizione, flagCi, short.Parse(view.domanda.Sede), short.Parse(view.domanda.CentroOperativo));

                
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.OK)
                {
                    AggiornaSemafori();
                    if (StatoDomandaPreNotifica == "CALCOLO NO INDEB WAIT")
                    {
                        view.AccogliDomandaMostraEsitoPositivo();
                        return;
                    }
                    view.ElencoCausaliMetodoAccogliDomandaVisualizzazioneEsitoPositivo();
                    return;
                }
                if (StatoDomandaPreNotifica == "CALCOLO NO INDEB WAIT")
                {
                    view.AccogliDomandaMostraEsitoNegativo();
                    return;
                }
                view.ElencoCausaliMetodoAccogliDomandaVisualizzazioneEsitoNegativo();
            }
            catch (Exception ex)
            {
                throw new DNA.DnaApplicationException(ex.Message);
            }
        }

        //Valida le causali selezionate e ritorna eventuale messaggio di errore
        public string ValidaCausaliSelezionate()
        {
            RootIndebitoDto Indebito = view.Indebito;
            List<CausaleDebito> CausaliDebito = view.CausaliDebito;

            for (int i = 0; i < Indebito.Data.ContiRic.Length; i++)
            {
                CausaleDto causaleSelezionata = new CausaleDto();
                causaleSelezionata.Analitica = CausaliDebito[i].CausaleAnalitica;
                causaleSelezionata.Sintetica = CausaliDebito[i].CausaleSintetica;
                if (CausaliDebito[i].CasualiAmmesse == null || CausaliDebito[i].CasualiAmmesse.Length == 0)
                    causaleSelezionata.Descrizione = CausaliDebito[i].Descrizione;
                else
                {
                    CausaleDtoLite nuovacausaleSelezionata = CausaliDebito[i].CasualiAmmesse.FirstOrDefault(c => c.Analitica.Equals(CausaliDebito[i].CausaleAnalitica) &&
                        c.Sintetica.Equals(CausaliDebito[i].CausaleSintetica));
                    if (nuovacausaleSelezionata == null)
                    {
                        return string.Format("Casusale selezionata (Analitica: {0}, Sintetica: {1}) non valida", causaleSelezionata.Analitica, causaleSelezionata.Sintetica); ;
                    }
                    causaleSelezionata.Descrizione = nuovacausaleSelezionata.Descrizione;
                }
                Indebito.Data.ContiRic[i].CausaleSelezionata = causaleSelezionata;
            }
            return string.Empty;
        }

        //Gestisce le logiche dietro al click del pulsante "Valida Causali"
        public void ValidaCausali()
        {
            try
            {
                view.CaricaDomandaDaSessione();
                view.CaricaCausaliDabitoDaSessione();
                view.LeggiIndebitoDaSessione();
                view.LeggiDatiPensioneDaSessione();

                string ErroreValidazioneCausali = ValidaCausaliSelezionate();
                //Se c'è un errore di validazione lo mostro in UcAvviso e interrempo la validazione causali
                if (!string.IsNullOrEmpty(ErroreValidazioneCausali)) {
                    view.MostraAvviso(ErroreValidazioneCausali);
                    return;
                }
                bool doppioControlloBloccoValidazioneCausali = ControllaBloccoValidazioneCausali(); 
                if(doppioControlloBloccoValidazioneCausali)
                {
                    view.MostraAvviso("Impossibile Procedere Con Validazione Causali. Risulta Attivo un Blocco alla Validazione Causali");
                    return;
                }
                view.NascondiAvviso();

                ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                bool flagCi = view.datiPensione.TipoAppartenenzaDomanda.Equals(AreaTitolare.DatiPensione.TipoAppDomanda.CI);
                AreaEsito esito = objWS.AggiornaCasuali(long.Parse(view.domanda.NumeroDomanda), view.domanda.MatricolaUtenteAcquisizione, view.Indebito.Data, flagCi, short.Parse(view.domanda.Sede), short.Parse(view.domanda.CentroOperativo));

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.OK)
                {
                    AggiornaSemafori();
                    view.ValidaCausaliMostraEsitoPositivo();
                    return;
                }
                view.ValidaCausaliMostraEsitoNegativo();
            }
            catch (Exception ex)
            {
                throw new DNA.DnaApplicationException(ex.Message);
            }
        }

        //Questo metodo serve per gestire il click del pulsante "Valida Causali" e cambiare lo stato della domanda da
        //AGGIORNA CALCOLO NO IND WEIT a AGGIORNA CALCOLO NO IND
        public void CambiaStatoDomanda()
        {
            try
            {
                view.CaricaDomandaDaSessione();

                ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                AreaCambioStatoDomanda inputCambioStato = new AreaCambioStatoDomanda()
                {
                    IsUpdateOperation = true,
                    NumeroDomandaUpdate = long.Parse(view.domanda.NumeroDomanda),
                    NuovoStatoPensione = "CALCOLO NO INDEB"
                };

                AreaEsito areaEsito = objWS.CambioStatoDomanda(ref inputCambioStato);

                if (areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK)
                {
                    view.domanda.Stato = "CALCOLO NO INDEB";
                    view.ScriviDomandaSessione(view.domanda);
                    SceltaPannello();
                }
                else
                {
                    throw new DNA.DnaApplicationException("Errore cambio stato domanda");
                }
            }
            catch (Exception ex)
            {
                throw new DNA.DnaApplicationException(ex.Message);
            }
        }

        private static string GetNomeControlloByTipo(AreaTitolare.DatiPensione.TipoAppDomanda tipo)
        {
            switch (tipo)
            {
                case AreaTitolare.DatiPensione.TipoAppDomanda.FS:
                    return "BloccaValidazioneCausaliFS";
                case AreaTitolare.DatiPensione.TipoAppDomanda.AGO:
                    return "BloccaValidazioneCausaliAGO";
                case AreaTitolare.DatiPensione.TipoAppDomanda.CI:
                    return "BloccaValidazioneCausaliCI";
                default:
                    return null;
            }
        }

        private string GetMessaggioCodaPannelloEventualeScelta()
        {
            string MessaggioCoda;
            bool? BloccoValidazioneCausali = view.BloccoValidazioneCausali;

            if (BloccoValidazioneCausali != null && (bool)BloccoValidazioneCausali)
            {
                MessaggioCoda = "In questa fase la \"Validazione delle causali\" non è attiva. Il debito deve essere gestito con<b> le consuete modalità</b> in procedura RI.";
                MessaggioCoda += "Per proseguire seleziona \"Accogli Domanda\".";
                //MessaggioCoda += "Per maggiori informazioni sulla nuova gestione del TE08 Ind <a href=\"../Manuali/ManualeNuovoFlussoIndebiti.pdf\" target=\"_blank\" rel=\"noopener\" style=\"color: red\">Consultare il pdf informativo</a>";
            }
            else
            {
                ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                string FlagIndebito;
                view.LeggiDatiPensioneDaSessione();
                objWS.GetFlagIndebitoByDomusAndProgressivoStorico(out FlagIndebito, view.datiPensione.NDomus, view.datiPensione.ProgStorico);
                switch (FlagIndebito)
                {
                    case "C":
                        MessaggioCoda = "È presente almeno un conguaglio ARTE da gestire.\nValutare se procedere con il TE08/Ind (Tasto \"Prosegui validazione causali di debito\") o con il TE08 (Tasto \"Accogli domanda\").";
                        break;
                    case "M":
                        MessaggioCoda = "Assegno ordinario di invalidità e presenza di almeno un conguaglio ARTE da gestire.\nValutare se procedere con il TE08/Ind (Tasto \"Prosegui validazione causali di debito\") o con il TE08 (Tasto \"Accogli domanda\") per effettuare la compensazione e per verificare l’indebito tenendo conto della \"Trattenuta per occupazione presso terzi\".";
                        break;
                    case "A":
                        MessaggioCoda = "Assegno ordinario di invalidità.\nValutare se procedere con il TE08/Ind (Tasto \"Prosegui validazione causali di debito\") o con il TE08 (Tasto \"Accogli domanda\") per verificare l’indebito tenendo conto della \"trattenuta per occupazione presso terzi\".";
                        break;
                    default:
                        MessaggioCoda = "Verificale la presenza di conguaglio ARTE da gestire e valutare se procedere con TE08 / Ind(tasto \"Prosegui validazione causali di debito\") o TE08(tasto \"Accogli la domanda\")";
                        break;
                }
            }
            return MessaggioCoda;
        }
    }
}
