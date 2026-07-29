using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAggiornamentoWebDom
    {
        
        public static bool GetAreaAggiornamentoWebDom(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate, out int? domandeElaborateConErrore, out int? domandeTotali,out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstEsitoAggWebDom;
                GestioneEsitoAggiornamentoWebDom.GetEsitoAggiornamentoWebDom(tipoApp, out lstEsitoAggWebDom);

                if (lstEsitoAggWebDom != null && lstEsitoAggWebDom.Count > 0)
                {
                    if (IsElaborazioneInCorso(lstEsitoAggWebDom))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;
                        domandeDaElaborare = lstEsitoAggWebDom.Where(x => x.Esito == null).Count();
                        domandeElaborate = lstEsitoAggWebDom.Where(x => x.Esito == true).Count();
                        domandeElaborateConErrore = lstEsitoAggWebDom.Where(x => x.Esito == false).Count();
                        domandeTotali = lstEsitoAggWebDom.Count();
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;
                        domandeDaElaborare = lstEsitoAggWebDom.Where(x => x.Esito == null).Count();
                        domandeElaborate = lstEsitoAggWebDom.Where(x => x.Esito == true).Count();
                        domandeElaborateConErrore = lstEsitoAggWebDom.Where(x => x.Esito == false).Count();

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, 8, null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, out lst);
                        if (lst != null && lst.Count > 0)
                            domandeTotali = lst.Count;
                        else
                            domandeTotali = 0;

                    }
                }
                else
                {
                    //elab non in corso
                    isAggiornamentoInCorso = false;
                    List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, 8, null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, out lst);
                    if (lst != null && lst.Count > 0)
                        domandeTotali = lst.Count;
                    else
                        domandeTotali = 0;
                }
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
                messaggioVideo = ex.Message;
                return false;
            }

            return true;
        }



        #region Public methods
        public static void ElaboraDomandeWebDom(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstEsitoAggWebDom;
            GestioneEsitoAggiornamentoWebDom.GetEsitoAggiornamentoWebDom(tipoApp, out lstEsitoAggWebDom);
            if (IsElaborazioneInCorso(lstEsitoAggWebDom))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoWebDom , null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, out lst);
            if(lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstDomCalcNoWebDom = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoWebDom.EliminaEsitoAggiornamentoWebDomByTipoApp(tipoApp);

                lstDomCalcNoWebDom = lst.Select(x => new GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom() { Ndomus = long.Parse(x.NumeroDomanda), TipoApp = tipoApp.ToString()}).ToList(); 
                foreach(var elem in lstDomCalcNoWebDom)
                    GestioneEsitoAggiornamentoWebDom.SalvaEsitoAggiornamentoWebDom(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo lo Sblocco e poi eseguiamo l'AggiornafaseAttivita
            foreach(var elem in lstDomCalcNoWebDom)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomanda(elem.Ndomus, out datiPensione);

                     using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                     new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                     {
                         string statoPensione;
                         if (GestioneWebDom.AggiornaWebDom(datiPensione, datiPensione.MatricolaUtenteAcquisizione, datiPensione.CodiceSede, out statoPensione, out msgErrore))
                         {
                             elem.Esito = true;
                         }
                         else
                         {
                             if (statoPensione == Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom))
                             {
                                 elem.Esito = false;
                                 elem.Errore = msgErrore;
                             }
                             else
                                 elem.Esito = true;
                         }
                         GestioneEsitoAggiornamentoWebDom.SalvaEsitoAggiornamentoWebDom(elem);
                         transactionScope.Complete();
                     }
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeWebDom -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }   
        }
        #endregion

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstEsitoAggWebDom)
        {
            //get intervallo per utility agg webdom
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggWebDom", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico!=null ? int.Parse(controlloDinamico.ValoreControllo):5, 0);

            return lstEsitoAggWebDom != null && lstEsitoAggWebDom.Count > 0 && lstEsitoAggWebDom.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggWebDom.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion
    }
}
