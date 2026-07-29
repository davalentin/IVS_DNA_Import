using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAggiornamento
    {
        #region Area Aggiornamento WebDom
        #region Public methods
        public static bool GetAreaAggiornamentoWebDom(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate, out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
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
                        domandeTotali = lstEsitoAggWebDom.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;
                        domandeDaElaborare = lstEsitoAggWebDom.Where(x => x.Esito == null).Count();
                        domandeElaborate = lstEsitoAggWebDom.Where(x => x.Esito == true).Count();
                        domandeElaborateConErrore = lstEsitoAggWebDom.Where(x => x.Esito == false).Count();

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, 8, null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue,
                                                                                   null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno,
                                                                                   null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, 8, null, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue,
                                                                               null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno,
                                                                               null, null, null, out lst);
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

        public static void ElaboraDomandeWebDom(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstEsitoAggWebDom;
            GestioneEsitoAggiornamentoWebDom.GetEsitoAggiornamentoWebDom(tipoApp, out lstEsitoAggWebDom);
            if (IsElaborazioneInCorso(lstEsitoAggWebDom))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoWebDom, null, DateTime.MinValue, DateTime.MaxValue,
                                                                       DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                                                                       GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstDomCalcNoWebDom = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoWebDom.EliminaEsitoAggiornamentoWebDomByTipoApp(tipoApp);

                lstDomCalcNoWebDom = lst.Select(x => new GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom { Ndomus = long.Parse(x.NumeroDomanda), TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoWebDom)
                    GestioneEsitoAggiornamentoWebDom.SalvaEsitoAggiornamentoWebDom(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo lo Sblocco e poi eseguiamo l'AggiornafaseAttivita
            foreach (var elem in lstDomCalcNoWebDom)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, null, out datiPensione);
                    byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;

                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        string statoPensione;
                        if (GestioneWebDom.AggiornaWebDom(datiPensione, datiDanteCausa, datiPensione.MatricolaUtenteAcquisizione, Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)), out statoPensione, out msgErrore))
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
                    //SCRIWO
                    GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, datiPensione.MatricolaUtenteAcquisizione, Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)));
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeWebDom -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lstEsitoAggWebDom)
        {
            //get intervallo per utility agg webdom
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggWebDom", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggWebDom != null && lstEsitoAggWebDom.Count > 0 && lstEsitoAggWebDom.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggWebDom.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods
        #endregion Area Aggiornamento WebDom

        #region Area Aggiornamento Felpe
        #region Public methods
        public static bool GetAreaAggiornamentoFelpe(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe> lstEsitoAggFelpe;
                GestioneEsitoAggiornamentoFelpe.GetEsitoAggiornamentoFelpe(tipoApp, out lstEsitoAggFelpe);

                if (lstEsitoAggFelpe != null && lstEsitoAggFelpe.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggFelpe.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggFelpe.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggFelpe.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggFelpe))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggFelpe.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoFelpe, null, DateTime.MinValue,
                                                                                   DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                                                                                   GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoFelpe, null, DateTime.MinValue,
                                                                               DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                                                                               GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeFelpe(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe> lstEsitoAggFelpe;
            GestioneEsitoAggiornamentoFelpe.GetEsitoAggiornamentoFelpe(tipoApp, out lstEsitoAggFelpe);
            if (IsElaborazioneInCorso(lstEsitoAggFelpe))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoFelpe, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe> lstDomCalcNoFelpe = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoFelpe.EliminaEsitoAggiornamentoFelpeByTipoApp(tipoApp);

                lstDomCalcNoFelpe = lst.Select(x => new GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoFelpe)
                    GestioneEsitoAggiornamentoFelpe.SalvaEsitoAggiornamentoFelpe(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'AggiornaFelpe
            foreach (var elem in lstDomCalcNoFelpe)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);
                    byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;

                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        string statoPensione;
                        if (GestioneAggiornamentoPECO.AggiornaFelpe(datiPensione, datiDanteCausa, datiPensione.MatricolaUtenteAcquisizione, Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)), out statoPensione, out msgErrore))
                            elem.Esito = true;
                        else
                        {
                            if (statoPensione == Utility.GetDescription(Utility.StatoPensione.CalcolataNoFelpe))
                            {
                                elem.Esito = false;
                                elem.Errore = msgErrore;
                            }
                            else
                                elem.Esito = true;
                        }
                        GestioneEsitoAggiornamentoFelpe.SalvaEsitoAggiornamentoFelpe(elem);
                        transactionScope.Complete();
                    }
                    //SCRIWO
                    GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeFelpe -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe> lstEsitoAggFelpe)
        {
            //get intervallo per utility agg webdom
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggFelpe", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggFelpe != null && lstEsitoAggFelpe.Count > 0 && lstEsitoAggFelpe.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggFelpe.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods
        #endregion Area Aggiornamento Felpe

        #region Area Aggiornamento Oneri
        #region Public methods
        public static bool GetAreaAggiornamentoOneri(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri> lstEsitoAggOneri;
                GestioneEsitoAggiornamentoOneri.GetEsitoAggiornamentoOneri(tipoApp, out lstEsitoAggOneri);

                if (lstEsitoAggOneri != null && lstEsitoAggOneri.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggOneri.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggOneri.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggOneri.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggOneri))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggOneri.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoOneri, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoOneri, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeOneri(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri> lstEsitoAggOneri;
            GestioneEsitoAggiornamentoOneri.GetEsitoAggiornamentoOneri(tipoApp, out lstEsitoAggOneri);
            if (IsElaborazioneInCorso(lstEsitoAggOneri))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoOneri, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri> lstDomCalcNoOneri = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoOneri.EliminaEsitoAggiornamentoOneriByTipoApp(tipoApp);

                lstDomCalcNoOneri = lst.Select(x => new GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoOneri)
                    GestioneEsitoAggiornamentoOneri.SalvaEsitoAggiornamentoOneri(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'AggiornaOneri
            foreach (var elem in lstDomCalcNoOneri)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);
                    byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;

                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    string statoPensione;
                    if (GestioneOneriPrepensionamento.AggiornaOneri(datiPensione, datiDanteCausa, out statoPensione, out msgErrore))
                        elem.Esito = true;
                    else
                    {
                        if (statoPensione == Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri))
                        {
                            elem.Esito = false;
                            elem.Errore = msgErrore;
                        }
                        else
                            elem.Esito = true;
                    }
                    GestioneEsitoAggiornamentoOneri.SalvaEsitoAggiornamentoOneri(elem);
                    //SCRIWO
                    GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, string.Empty, 0);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeOneri -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri> lstEsitoAggOneri)
        {
            //get intervallo per utility agg webdom
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggOneri", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggOneri != null && lstEsitoAggOneri.Count > 0 && lstEsitoAggOneri.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggOneri.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods
        #endregion Area Aggiornamento Oneri

        #region Area Aggiornamento Cumulo
        #region Public methods
        public static bool GetAreaAggiornamentoCumulo(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo> lstEsitoAggCumulo;
                GestioneEsitoAggiornamentoCumulo.GetEsitoAggiornamentoCumulo(tipoApp, out lstEsitoAggCumulo);

                if (lstEsitoAggCumulo != null && lstEsitoAggCumulo.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggCumulo.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggCumulo.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggCumulo.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggCumulo))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggCumulo.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoTotal, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                            GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoTotal, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                        GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeCumulo(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo> lstEsitoAggCumulo;
            GestioneEsitoAggiornamentoCumulo.GetEsitoAggiornamentoCumulo(tipoApp, out lstEsitoAggCumulo);
            if (IsElaborazioneInCorso(lstEsitoAggCumulo))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoTotal, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo> lstDomCalcNoCumulo = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoCumulo.EliminaEsitoAggiornamentoCumuloByTipoApp(tipoApp);

                lstDomCalcNoCumulo = lst.Select(x => new GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoCumulo)
                    GestioneEsitoAggiornamentoCumulo.SalvaEsitoAggiornamentoCumulo(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'AggiornaCumulo
            foreach (var elem in lstDomCalcNoCumulo)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);

                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    //string statoPensione;
                    if (GestioneTotalIvs.AggiornaCumulo(datiPensione, out msgErrore))
                        elem.Esito = true;
                    else
                    {
                        //if (statoPensione == Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal))
                        //{
                        elem.Esito = false;
                        elem.Errore = msgErrore;
                        //}
                        //else
                        //    elem.Esito = true;
                    }
                    GestioneEsitoAggiornamentoCumulo.SalvaEsitoAggiornamentoCumulo(elem);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeCumulo -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo> lstEsitoAggCumulo)
        {
            //get intervallo per utility agg webdom
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggCumulo", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggCumulo != null && lstEsitoAggCumulo.Count > 0 && lstEsitoAggCumulo.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggCumulo.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods
        #endregion Area Aggiornamento Cumulo

        #region Area Aggiornamento Tot
        #region Public methods
        public static bool GetAreaAggiornamentoTot(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot> lstEsitoAggTot;
                GestioneEsitoAggiornamentoTot.GetEsitoAggiornamentoTot(tipoApp, out lstEsitoAggTot);

                if (lstEsitoAggTot != null && lstEsitoAggTot.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggTot.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggTot.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggTot.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggTot))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggTot.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoTot, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                            GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoTot, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno,
                        GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeTot(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot> lstEsitoAggTot;
            GestioneEsitoAggiornamentoTot.GetEsitoAggiornamentoTot(tipoApp, out lstEsitoAggTot);
            if (IsElaborazioneInCorso(lstEsitoAggTot))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoTot, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot> lstDomCalcNoTot = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoTot.EliminaEsitoAggiornamentoTotByTipoApp(tipoApp);

                lstDomCalcNoTot = lst.Select(x => new GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoTot)
                    GestioneEsitoAggiornamentoTot.SalvaEsitoAggiornamentoTot(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'AggiornaTot
            foreach (var elem in lstDomCalcNoTot)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);

                    //string statoPensione;
                    if (GestioneTotalIvs.AggiornaTot(datiPensione, out msgErrore))
                        elem.Esito = true;
                    else
                    {
                        //if (statoPensione == Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal))
                        //{
                        elem.Esito = false;
                        elem.Errore = msgErrore;
                        //}
                        //else
                        //    elem.Esito = true;
                    }
                    GestioneEsitoAggiornamentoTot.SalvaEsitoAggiornamentoTot(elem);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeTot -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot> lstEsitoAggTot)
        {
            //get intervallo per utility agg webdom
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggTot", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggTot != null && lstEsitoAggTot.Count > 0 && lstEsitoAggTot.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggTot.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods
        #endregion Area Aggiornamento Tot

        #region Area Aggiornamento SAI
        #region Public methods
        public static bool GetAreaAggiornamentoSAI(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI> lstEsitoAggSAI;
                GestioneEsitoAggiornamentoSAI.GetEsitoAggiornamentoSAI(tipoApp, out lstEsitoAggSAI);

                if (lstEsitoAggSAI != null && lstEsitoAggSAI.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggSAI.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggSAI.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggSAI.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggSAI))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggSAI.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoSAI, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoSAI, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeSAI(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI> lstEsitoAggSAI;
            GestioneEsitoAggiornamentoSAI.GetEsitoAggiornamentoSAI(tipoApp, out lstEsitoAggSAI);
            if (IsElaborazioneInCorso(lstEsitoAggSAI))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoSAI, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI> lstDomCalcNoSAI = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoSAI.EliminaEsitoAggiornamentoSAIByTipoApp(tipoApp);

                lstDomCalcNoSAI = lst.Select(x => new GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoSAI)
                    GestioneEsitoAggiornamentoSAI.SalvaEsitoAggiornamentoSAI(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'Aggiorna SAI
            foreach (var elem in lstDomCalcNoSAI)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);

                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                    TipoRichiesta.PAG? tipoRic = GestioneSAI.GetTipoRichiestaPAG(datiPensione);

                    if (GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, tipoRic, out msgErrore))
                        elem.Esito = true;
                    else
                    {
                        elem.Esito = false;
                        elem.Errore = msgErrore;
                    }
                    GestioneEsitoAggiornamentoSAI.SalvaEsitoAggiornamentoSAI(elem);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeSAI -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI> lstEsitoAggSAI)
        {
            //get intervallo per utility agg SAI
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggSAI", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggSAI != null && lstEsitoAggSAI.Count > 0 && lstEsitoAggSAI.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggSAI.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods

        #endregion Area Aggiornamento SAI

        #region Area Aggiornamento INPDAP
        #region Public methods
        public static bool GetAreaAggiornamentoINPDAP(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP> lstEsitoAggINPDAP;
                GestioneEsitoAggiornamentoINPDAP.GetEsitoAggiornamentoINPDAP(tipoApp, out lstEsitoAggINPDAP);

                if (lstEsitoAggINPDAP != null && lstEsitoAggINPDAP.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggINPDAP.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggINPDAP.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggINPDAP.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggINPDAP))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggINPDAP.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoSIN, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoSIN, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeINPDAP(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP> lstEsitoAggINPDAP;
            GestioneEsitoAggiornamentoINPDAP.GetEsitoAggiornamentoINPDAP(tipoApp, out lstEsitoAggINPDAP);
            if (IsElaborazioneInCorso(lstEsitoAggINPDAP))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoSIN, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP> lstDomCalcNoINPDAP = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoINPDAP.EliminaEsitoAggiornamentoINPDAPByTipoApp(tipoApp);

                lstDomCalcNoINPDAP = lst.Select(x => new GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoINPDAP)
                    GestioneEsitoAggiornamentoINPDAP.SalvaEsitoAggiornamentoINPDAP(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'Aggiorna INPDAP
            foreach (var elem in lstDomCalcNoINPDAP)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);

                    if (GestioneINPDAP.AggiornaINPDAP(datiPensione, out msgErrore))
                        elem.Esito = true;
                    else
                    {
                        elem.Esito = false;
                        elem.Errore = msgErrore;
                    }
                    GestioneEsitoAggiornamentoINPDAP.SalvaEsitoAggiornamentoINPDAP(elem);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeINPDAP -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP> lstEsitoAggINPDAP)
        {
            //get intervallo per utility agg INPDAP
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggINPDAP", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggINPDAP != null && lstEsitoAggINPDAP.Count > 0 && lstEsitoAggINPDAP.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggINPDAP.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods

        #endregion Area Aggiornamento INPDAP

        #region Area Aggiornamento NOTE DEBITO
        #region Public methods
        public static bool GetAreaAggiornamentoNoteDiDebito(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
            out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito> lstEsitoAggNoteDiDebito;
                GestioneEsitoAggiornamentoNoteDiDebito.GetEsitoAggiornamentoNoteDiDebito(tipoApp, out lstEsitoAggNoteDiDebito);

                if (lstEsitoAggNoteDiDebito != null && lstEsitoAggNoteDiDebito.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggNoteDiDebito.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggNoteDiDebito.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggNoteDiDebito.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggNoteDiDebito))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggNoteDiDebito.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoNoteDebito, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoNoteDebito, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandeNoteDiDebito(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito> lstEsitoAggNoteDiDebito;
            GestioneEsitoAggiornamentoNoteDiDebito.GetEsitoAggiornamentoNoteDiDebito(tipoApp, out lstEsitoAggNoteDiDebito);
            if (IsElaborazioneInCorso(lstEsitoAggNoteDiDebito))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNoNoteDebito, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito> lstDomCalcNoNoteDiDebito = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoNoteDiDebito.EliminaEsitoAggiornamentoNoteDiDebitoByTipoApp(tipoApp);

                lstDomCalcNoNoteDiDebito = lst.Select(x => new GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoNoteDiDebito)
                    GestioneEsitoAggiornamentoNoteDiDebito.SalvaEsitoAggiornamentoNoteDiDebito(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'Aggiorna NoteDiDebito
            foreach (var elem in lstDomCalcNoNoteDiDebito)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);

                    if (GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out msgErrore))
                        elem.Esito = true;
                    else
                    {
                        elem.Esito = false;
                        elem.Errore = msgErrore;
                    }
                    GestioneEsitoAggiornamentoNoteDiDebito.SalvaEsitoAggiornamentoNoteDiDebito(elem);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandeNoteDiDebito -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
        #endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito> lstEsitoAggNoteDiDebito)
        {
            //get intervallo per utility agg NoteDiDebito
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggNoteDiDebito", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggNoteDiDebito != null && lstEsitoAggNoteDiDebito.Count > 0 && lstEsitoAggNoteDiDebito.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggNoteDiDebito.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods

        #endregion Area Aggiornamento NOTE DEBITO

        #region Area Aggiornamento PIANI DI PAGAMENTO
        #region Public methods
        public static bool GetAreaAggiornamentoPianiDiPagamento(Utility.TipoAppartenenza tipoApp, out bool isAggiornamentoInCorso, out int? domandeDaElaborare, out int? domandeElaborate,
           out int? domandeElaborateConErrore, out int? domandeTotali, out string messaggioVideo)
        {
            isAggiornamentoInCorso = false;
            domandeDaElaborare = null;
            domandeElaborate = null;
            domandeElaborateConErrore = null;
            domandeTotali = null;
            messaggioVideo = string.Empty;
            try
            {
                List<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento> lstEsitoAggPianiDiPagamento;
                GestioneEsitoAggiornamentoPianiDiPagamento.GetEsitoAggiornamentoPianiDiPagamento(tipoApp, out lstEsitoAggPianiDiPagamento);

                if (lstEsitoAggPianiDiPagamento != null && lstEsitoAggPianiDiPagamento.Count > 0)
                {
                    domandeDaElaborare = lstEsitoAggPianiDiPagamento.Where(x => x.Esito == null).Count();
                    domandeElaborate = lstEsitoAggPianiDiPagamento.Where(x => x.Esito == true).Count();
                    domandeElaborateConErrore = lstEsitoAggPianiDiPagamento.Where(x => x.Esito == false).Count();

                    if (IsElaborazioneInCorso(lstEsitoAggPianiDiPagamento))
                    {
                        //elab in corso
                        isAggiornamentoInCorso = true;

                        domandeTotali = lstEsitoAggPianiDiPagamento.Count;
                    }
                    else
                    {
                        //elab non in corso
                        isAggiornamentoInCorso = false;

                        List<GestioneStatoPratica.DatiDomandaDettagliata> lst = new List<GestioneStatoPratica.DatiDomandaDettagliata>();
                        GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNo6Scatti, null, DateTime.MinValue,
                            DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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
                    GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNo6Scatti, null, DateTime.MinValue,
                        DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
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

        public static void ElaboraDomandePianiDiPagamento(Utility.TipoAppartenenza tipoApp)
        {
            //Controllo che non ci sia un'attività in corso
            List<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento> lstEsitoAggPianiDiPagamento;
            GestioneEsitoAggiornamentoPianiDiPagamento.GetEsitoAggiornamentoPianiDiPagamento(tipoApp, out lstEsitoAggPianiDiPagamento);
            bool isCodiceEsito9 = false;
            if (IsElaborazioneInCorso(lstEsitoAggPianiDiPagamento))
                return;
            //Controllo se ci sono domande da elaborare
            List<GestioneStatoPratica.DatiDomandaDettagliata> lst = null;
            GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(null, null, null, null, null, null, null, (int)Utility.StatoPensione.CalcolataNo6Scatti, null, DateTime.MinValue, DateTime.MaxValue,
                DateTime.MinValue, DateTime.MaxValue, null, tipoApp, Utility.Ruolo.AMMINISTRATORE, GestioneStatoPratica.TipoDomanda.Nessuno, GestioneStatoPratica.TipoDomanda.Nessuno, null, null, null, out lst);
            if (lst == null || lst.Count == 0)
                return;
            //FASE 1 - si cancella la tabella tmp e si inseriscono le dom da elaborare        
            List<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento> lstDomCalcNoPianiDiPagamento = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneEsitoAggiornamentoPianiDiPagamento.EliminaEsitoAggiornamentoPianiDiPagamentoByTipoApp(tipoApp);

                lstDomCalcNoPianiDiPagamento = lst.Select(x => new GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento { Ndomus = long.Parse(x.NumeroDomanda), ProgStorico = x.ProgStorico, TipoApp = tipoApp.ToString() }).ToList();
                foreach (var elem in lstDomCalcNoPianiDiPagamento)
                    GestioneEsitoAggiornamentoPianiDiPagamento.SalvaEsitoAggiornamentoPianiDiPagamento(elem);

                transactionScope.Complete();
            }

            //FASE 2 - per ogni domanda da elaborare effettuiamo l'Aggiorna PianiDiPagamento
            foreach (var elem in lstDomCalcNoPianiDiPagamento)
            {
                string msgErrore = string.Empty;
                try
                {
                    GestionePensione.DatiPensione datiPensione;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(elem.Ndomus, elem.ProgStorico, out datiPensione);

                    if (GestioneINPDAP.AggiornaPianiDiPagamento(datiPensione, out msgErrore, out isCodiceEsito9))
                        elem.Esito = true;
                    else
                    {
                        elem.Esito = false;
                        elem.Errore = msgErrore;
                    }
                    GestioneEsitoAggiornamentoPianiDiPagamento.SalvaEsitoAggiornamentoPianiDiPagamento(elem);
                }
                catch (Exception ex)
                {
                    INPS.DNA.Logging.Logger.WriteError(string.Format("ElaboraDomandePianiDiPagamento -> Durante l'elaborazione della domanda {0} è stato rilevato il seguente errore '{1}'.", elem.Ndomus, ex.Message));
                }
            }
        }
#endregion Public methods

        #region Private methods
        private static bool IsElaborazioneInCorso(List<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento> lstEsitoAggPianiDiPagamento)
        {
            //get intervallo per utility agg PianiDiPagamento
            GestioneControlliDinamici.ControlloDinamico controlloDinamico;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("TimeoutElaborazioneAggPianiDiPagamento", out controlloDinamico);
            TimeSpan IntervalloBlocco = new TimeSpan(0, controlloDinamico != null ? int.Parse(controlloDinamico.ValoreControllo) : 5, 0);

            return lstEsitoAggPianiDiPagamento != null && lstEsitoAggPianiDiPagamento.Count > 0 && lstEsitoAggPianiDiPagamento.Exists(x => x.Esito == null) &&
                               (DateTime.Now - lstEsitoAggPianiDiPagamento.OrderByDescending(x => x.Timestamp).First().Timestamp) < IntervalloBlocco;
        }
        #endregion Private methods

#endregion Area Aggiornamento PIANI DI PAGAMENTO

        /// <summary>
        /// Recupera le informazioni dalle tabelle EsitoAggiornamento in base al tipo della lista in out
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tipoApp"></param>
        /// <param name="lstEsitoAggiornamento"></param>
        internal static void GetEsitoAggiornamentoGeneric<T>(Utility.TipoAppartenenza tipoApp, out List<EsitoAggiornamento> lstEsitoAggiornamento)
        {
            lstEsitoAggiornamento = null;

            Type listType = typeof(T);

            if (listType == typeof(GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom))
            {
                List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom> lst = new List<GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom>();
                GestioneEsitoAggiornamentoWebDom.GetEsitoAggiornamentoWebDom(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe))
            {
                List<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe> lst = new List<GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe>();
                GestioneEsitoAggiornamentoFelpe.GetEsitoAggiornamentoFelpe(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri))
            {
                List<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri> lst = new List<GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri>();
                GestioneEsitoAggiornamentoOneri.GetEsitoAggiornamentoOneri(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo))
            {
                List<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo> lst = new List<GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo>();
                GestioneEsitoAggiornamentoCumulo.GetEsitoAggiornamentoCumulo(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoCumulo.EsitoAggiornamentiCumulo elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot))
            {
                List<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot> lst = new List<GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot>();
                GestioneEsitoAggiornamentoTot.GetEsitoAggiornamentoTot(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoTot.EsitoAggiornamentiTot elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI))
            {
                List<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI> lst = new List<GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI>();
                GestioneEsitoAggiornamentoSAI.GetEsitoAggiornamentoSAI(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoSAI.EsitoAggiornamentiSAI elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP))
            {
                List<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP> lst = new List<GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP>();
                GestioneEsitoAggiornamentoINPDAP.GetEsitoAggiornamentoINPDAP(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoINPDAP.EsitoAggiornamentiINPDAP elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito))
            {
                List<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito> lst = new List<GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito>();
                GestioneEsitoAggiornamentoNoteDiDebito.GetEsitoAggiornamentoNoteDiDebito(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoNoteDiDebito.EsitoAggiornamentiNoteDiDebito elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
            else if (listType == typeof(GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento))
            {
                List<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento> lst = new List<GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento>();
                GestioneEsitoAggiornamentoPianiDiPagamento.GetEsitoAggiornamentoPianiDiPagamento(tipoApp, out lst);

                if (lst != null && lst.Count > 0)
                {
                    lstEsitoAggiornamento = new List<EsitoAggiornamento>();
                    foreach (GestioneEsitoAggiornamentoPianiDiPagamento.EsitoAggiornamentiPianiDiPagamento elem in lst)
                    {
                        EsitoAggiornamento obj = new EsitoAggiornamento();
                        Utility.ValorizzaOggetti(elem, obj);
                        lstEsitoAggiornamento.Add(obj);
                    }
                }
            }
        }

        #region nested class
        public class EsitoAggiornamento
        {
            public long Ndomus { get; set; }
            public string TipoApp { get; set; }
            public System.Nullable<bool> Esito { get; set; }
            public string Errore { get; set; }
            public System.Nullable<System.DateTime> Timestamp { get; set; }
        }
        #endregion nested class
    }
}
