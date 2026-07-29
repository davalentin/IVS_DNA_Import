using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneCi.Data;
using INPS.Pensioni.LiquidazioneCi.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;



namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestioneContrib
    {
        #region ProRata

        public static void GetStatiEsteri(GestionePensione.DatiPensione datiPensione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere,
            GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out List<StatoEstero> listaStatiEsteri,
            out string cittadinanzaTitolare, out bool IsDataFromDB, out string messaggioVideo)
        {
            IsDataFromDB = false;
            listaStatiEsteri = null;
            cittadinanzaTitolare = string.Empty;
            messaggioVideo = "";

            // get data from DB
            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count > 0)
            {
                IsDataFromDB = true;
                GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out listaStatiEsteri);
                return;
            }

            if (datiPensione == null)
            {
                messaggioVideo = "Dati pensione non disponibili";
                return;
            }

            GetAndStoreStatiEsteri(datiPensione, datiAggPec, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out listaStatiEsteri, out cittadinanzaTitolare, out messaggioVideo);
        }

        public static void GetStatiEsteriFromService(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore,
            short sedeOperatore, short centroOperativoOperatore, out List<StatoEstero> listaStatiEsteri, out string cittadinanzaTitolare, out string messaggioVideo)
        {
            listaStatiEsteri = null;
            messaggioVideo = "";

            // get data from Trans
            List<PrestazioneEstera> listaPrestazioniEstere = null;
            cittadinanzaTitolare = string.Empty;
            bool isNuovaProcedura = false;

            try
            {
                if (!GestioneNACI.VerificaProcedura(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out isNuovaProcedura, out messaggioVideo))
                    return;

                if (isNuovaProcedura)
                    GestioneNACI.GetListaStatiIstituzione(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out listaPrestazioniEstere, out cittadinanzaTitolare, out messaggioVideo);
                else
                    GestioneAllegatiConvenzioni.GetPrestazioneEstereByNumeroDomanda(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out listaPrestazioniEstere, out cittadinanzaTitolare, out messaggioVideo);
            }
            catch (Exception)
            {
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                return;
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                return;
            }

            if (listaPrestazioniEstere == null || listaPrestazioniEstere.Count == 0)
            {
                messaggioVideo = "Prestazioni Estere mancanti";
                return;
            }

            listaStatiEsteri = new List<StatoEstero>();
            foreach (PrestazioneEstera prestazioneEstera in listaPrestazioniEstere)
            {
                StatoEstero statoEstero = new StatoEstero();
                statoEstero.PrestazioneEstera = prestazioneEstera;
                Data.aciistit descPrestazioneEstera = null;
                Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEstera.CodiceStatoEE + statoEstero.PrestazioneEstera.CodiceIstituzione, out descPrestazioneEstera);
                if (descPrestazioneEstera != null)
                {
                    statoEstero.PrestazioneEstera.Sigla = descPrestazioneEstera.SIGLISTI;
                    statoEstero.PrestazioneEstera.Citta = descPrestazioneEstera.CITTAIST;
                    statoEstero.PrestazioneEstera.NomeStato = descPrestazioneEstera.NOMESTAT;
                    statoEstero.PrestazioneEstera.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                    statoEstero.PrestazioneEstera.CodiceConvenzione = Utility.StringToNullableByte(descPrestazioneEstera.CODICONV);
                    statoEstero.PrestazioneEstera.Confermato = false;
                }
                statoEstero.ElencoImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
                listaStatiEsteri.Add(statoEstero);

                if (listaPrestazioniEstere.FindAll(x => (x.CodiceStatoEE == prestazioneEstera.CodiceStatoEE && x.CodiceIstituzione == prestazioneEstera.CodiceIstituzione)).Count > 1)
                    messaggioVideo = "Istituzioni uguali: variare CI81";
            }
        }

        //ENG - RIC/TRF: aggiunta la gestione per il recupero degli stati(se presenti e diversi da quelli provenienti da prelievo) dal servizio Naci o AllegatiConvenzioni
        public static void GetStatiEsteriFromServiceRicTrf(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore,
            short sedeOperatore, short centroOperativoOperatore, out List<StatoEstero> listaStatiEsteri, out string cittadinanzaTitolare, out string messaggioVideo)
        {
            listaStatiEsteri = null;
            messaggioVideo = "";

            // get data from Trans
            List<PrestazioneEstera> listaPrestazioniEstere = null;
            cittadinanzaTitolare = string.Empty;
            bool isNuovaProcedura = false;

            try
            {
                if (!GestioneNACI.VerificaProcedura(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out isNuovaProcedura, out messaggioVideo))
                    return;

                if (isNuovaProcedura)
                    GestioneNACI.GetListaStatiIstituzione(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out listaPrestazioniEstere, out cittadinanzaTitolare, out messaggioVideo);
                else
                    GestioneAllegatiConvenzioni.GetPrestazioneEstereByNumeroDomanda(numeroDomanda, matricolaOperatore, codiceSede, centroOperativo, out listaPrestazioniEstere, out cittadinanzaTitolare, out messaggioVideo);
            }
            catch (Exception)
            {
                INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                return;
            }

            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                return;
            }

            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0)
            {
                listaStatiEsteri = new List<StatoEstero>();
                foreach (PrestazioneEstera prestazioneEstera in listaPrestazioniEstere)
                {
                    StatoEstero statoEstero = new StatoEstero();
                    statoEstero.PrestazioneEstera = prestazioneEstera;
                    Data.aciistit descPrestazioneEstera = null;
                    Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEstera.CodiceStatoEE + statoEstero.PrestazioneEstera.CodiceIstituzione, out descPrestazioneEstera);
                    if (descPrestazioneEstera != null)
                    {
                        statoEstero.PrestazioneEstera.Sigla = descPrestazioneEstera.SIGLISTI;
                        statoEstero.PrestazioneEstera.Citta = descPrestazioneEstera.CITTAIST;
                        statoEstero.PrestazioneEstera.NomeStato = descPrestazioneEstera.NOMESTAT;
                        statoEstero.PrestazioneEstera.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                        statoEstero.PrestazioneEstera.CodiceConvenzione = Utility.StringToNullableByte(descPrestazioneEstera.CODICONV);
                        statoEstero.PrestazioneEstera.Confermato = false;
                    }
                    statoEstero.ElencoImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
                    listaStatiEsteri.Add(statoEstero);

                    if (listaPrestazioniEstere.FindAll(x => (x.CodiceStatoEE == prestazioneEstera.CodiceStatoEE && x.CodiceIstituzione == prestazioneEstera.CodiceIstituzione)).Count > 1)
                        messaggioVideo = "Istituzioni uguali: variare CI81";
                }
            }
        }

        internal static void GetStatiEEfromDBByIdPensione(long idPensione, List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere, out List<StatoEstero> listaStatiEsteri)
        {
            listaStatiEsteri = new List<StatoEstero>();
            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
            GestioneDatiContributiviCi.GetImportiEsteriByIdPensione(idPensione, out listaImportiEsteri);
            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEEStorico = null;
            GestioneDatiContributiviCi.GetPrestazioniEEStoricoByIdPensione(idPensione, out listaPrestazioniEEStorico);

            foreach (GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneEE in listaPrestazioniEstere)
            {
                StatoEstero statoEstero = new StatoEstero();
                statoEstero.PrestazioneEstera = new PrestazioneEstera();
                Utility.ValorizzaOggetti(prestazioneEE, statoEstero.PrestazioneEstera);
                Data.aciistit descPrestazioneEstera = null;
                Data.DAPrestazioniEstere.GetPrestazioneEstera(statoEstero.PrestazioneEstera.CodiceStatoEE + statoEstero.PrestazioneEstera.CodiceIstituzione, out descPrestazioneEstera);
                if (descPrestazioneEstera != null)
                {
                    statoEstero.PrestazioneEstera.Sigla = descPrestazioneEstera.SIGLISTI;
                    statoEstero.PrestazioneEstera.Citta = descPrestazioneEstera.CITTAIST;
                    statoEstero.PrestazioneEstera.NomeStato = descPrestazioneEstera.NOMESTAT;
                    statoEstero.PrestazioneEstera.SiglaStato = descPrestazioneEstera.SIGLASTAT;
                    statoEstero.PrestazioneEstera.MatricolaIstituzione = statoEstero.PrestazioneEstera.MatricolaIstituzioneEE;
                }
                if (listaPrestazioniEEStorico != null && listaPrestazioniEEStorico.Count > 0 &&
                    listaPrestazioniEEStorico.Any(x => x.CodiceStatoEE == statoEstero.PrestazioneEstera.CodiceStatoEE && x.CodiceIstituzione == statoEstero.PrestazioneEstera.CodiceIstituzione))
                {
                    GestioneDatiContributiviCi.PensioniCiPrestazioniEE prestazioneStoricoDB = listaPrestazioniEEStorico.FirstOrDefault(x => x.CodiceStatoEE == statoEstero.PrestazioneEstera.CodiceStatoEE && x.CodiceIstituzione == statoEstero.PrestazioneEstera.CodiceIstituzione);
                    PrestazioneEstera prestazioneStorico = new PrestazioneEstera();
                    Utility.ValorizzaOggetti(prestazioneStoricoDB, prestazioneStorico);
                    statoEstero.PrestazioneEsteraStorico = prestazioneStorico;
                }
                statoEstero.ElencoImportiEsteri = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestazioneEE.Id);
                listaStatiEsteri.Add(statoEstero);
            }
        }

        public static void StoreStatiEsteri(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, List<StatoEstero> listaStatiEsteri,
            DatiCalcolo datiCalcolo, bool singleTab, List<RedditiPerIntegrazioneVirtuale> listaReddIntegrazVirtuale, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            List<GestioneContrib.RedditiPerIntegrazioneVirtuale> listaReddIntegrazVirtualeDB = null;
            GestioneContrib.GetDatiRedditiPerIntegrazioneVirtualeByIdPensione(datiPensione, out listaReddIntegrazVirtualeDB, out messaggioVideo);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEE);

            string codiceStato = string.Empty;
            codiceStato = listaPrestazioniEE[0].CodiceStatoEE;

            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePerStato(codiceStato, datiPensione.DecorrenzaOriginaria, out listaCodiciConvenzione);

            if (ControlsStatiEsteri(datiPensione, datiIstruttoriaCommon, listaStatiEsteri, datiCalcolo, singleTab, out messaggioVideo))
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                    {
                        foreach (StatoEstero statoEstero in listaStatiEsteri)
                        {
                            statoEstero.PrestazioneEstera.IdPensione = datiPensione.Id;
                            if (String.IsNullOrEmpty(statoEstero.PrestazioneEstera.MatricolaIstituzioneEE))
                                statoEstero.PrestazioneEstera.MatricolaIstituzioneEE = statoEstero.PrestazioneEstera.MatricolaIstituzione;
                            GestioneDatiContributiviCi.SalvaPrestazioneEstera(statoEstero.PrestazioneEstera);
                            GestioneDatiContributiviCi.EliminaImportiEsteriPerPrestazione(statoEstero.PrestazioneEstera.Id);
                            if (statoEstero.ElencoImportiEsteri != null && statoEstero.ElencoImportiEsteri.Count > 0)
                            {
                                foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importoEstero in statoEstero.ElencoImportiEsteri)
                                {
                                    importoEstero.IDPrestazioneEE = statoEstero.PrestazioneEstera.Id;
                                    GestioneDatiContributiviCi.SalvaImportoEstero(importoEstero);
                                }
                            }
                        }
                        quadroDatiContributivi.TabProRata = 0;
                        if (!listaStatiEsteri.Any(x => x.PrestazioneEstera == null) && !listaStatiEsteri.Any(x => !x.PrestazioneEstera.Confermato.GetValueOrDefault()))
                            quadroDatiContributivi.TabProRata = 2;

                        if (listaReddIntegrazVirtuale.Count() != listaReddIntegrazVirtualeDB.Count() && listaCodiciConvenzione[0].CodiceConvenzione != 13)
                            quadroDatiContributivi.TabIntegrazioneVirtuale = 0;

                        GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
                    }
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaStatiEsteri(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore)
        {
            string messaggioVideo = string.Empty;

            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiContributiviCi.EliminaAllPrestazioniEE(datiPensione.Id);
                datiQuadroDatiContributivi.TabProRata = 0;
                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        internal static void GetAndStoreStatiEsteri(GestionePensione.DatiPensione datiPensione, GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec, string matricolaOperatore, short sedeOperatore,
            short centroOperativoOperatore, out List<StatoEstero> listaStatiEsteri, out string cittadinanzaTitolare, out string errori)
        {
            listaStatiEsteri = null;
            cittadinanzaTitolare = string.Empty;
            errori = string.Empty;
            try
            {
                GetStatiEsteriFromService(datiPensione.NDomus, Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)), datiPensione.CentroOperativo.HasValue ? datiPensione.CentroOperativo.Value : (byte)0,
                    matricolaOperatore, sedeOperatore, centroOperativoOperatore, out listaStatiEsteri, out cittadinanzaTitolare, out errori);
            }
            catch (INPS.DNA.DnaApplicationException)
            {
                throw new INPS.DNA.DnaApplicationException(errori);
            }
            if (!string.IsNullOrEmpty(errori))
                return;

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                foreach (StatoEstero statoEstero in listaStatiEsteri)
                {
                    if (datiAggPec != null)
                    {
                        GestioneAggiornamentoPECO.DatiIstituzioniEstere istituzionePECO = null;
                        if (datiAggPec.lIstituzioniEstere != null && datiAggPec.lIstituzioniEstere.Count > 0)
                            istituzionePECO = datiAggPec.lIstituzioniEstere.Find(x => x.CodiceIstituzione == statoEstero.PrestazioneEstera.CodiceIstituzione && x.CodiceStatoEE == statoEstero.PrestazioneEstera.CodiceStatoEE);
                        if (istituzionePECO != null)
                        {
                            if (istituzionePECO.ContributiEEDecorrenzaOriginaria != 0)
                                statoEstero.PrestazioneEstera.ContributiEEDecorrenzaOriginaria = istituzionePECO.ContributiEEDecorrenzaOriginaria;

                            if (istituzionePECO.ContributiEEDiritto != 0)
                                statoEstero.PrestazioneEstera.ContributiEEDiritto = istituzionePECO.ContributiEEDiritto;
                        }
                    }
                    if (String.IsNullOrEmpty(statoEstero.PrestazioneEstera.MatricolaIstituzioneEE))
                        statoEstero.PrestazioneEstera.MatricolaIstituzioneEE = statoEstero.PrestazioneEstera.MatricolaIstituzione;
                    listaPrestazioniEE.Add(statoEstero.PrestazioneEstera);
                }
                GestioneDatiContributiviCi.SalvaListaPrestazioniEstere(datiPensione.Id, listaPrestazioniEE);
            }
        }

        public static void GetListaCodiceConvenzione(out List<CodiceConvenzione> listaCodiceConvenzione)
        {
            listaCodiceConvenzione = new List<CodiceConvenzione>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzione> listaCodiceConvenzioneDB = null;
            GestioneDecodifica.GetCodiceConvenzione(out listaCodiceConvenzioneDB);
            if (listaCodiceConvenzioneDB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzione codiceConvenzioneDB in listaCodiceConvenzioneDB)
                {
                    CodiceConvenzione codiceConvenzione = new CodiceConvenzione();
                    codiceConvenzione.Id = codiceConvenzioneDB.Id;
                    codiceConvenzione.Descrizione = codiceConvenzioneDB.Descrizione;
                    listaCodiceConvenzione.Add(codiceConvenzione);
                }
            }
        }

        public static void GetListaCodiceVirtuale(out List<CodiceVirtuale> listaCodiceVirtuale)
        {
            listaCodiceVirtuale = new List<CodiceVirtuale>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceVirtuale> listaCodiceVirtualeDB = null;
            GestioneDecodifica.GetCodiceVirtuale(out listaCodiceVirtualeDB);
            if (listaCodiceVirtualeDB != null)
            {
                foreach (INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceVirtuale codiceVirtualeDB in listaCodiceVirtualeDB)
                {
                    CodiceVirtuale codiceVirtuale = new CodiceVirtuale();
                    codiceVirtuale.Id = codiceVirtualeDB.Id;
                    codiceVirtuale.Descrizione = codiceVirtualeDB.Descrizione;
                    listaCodiceVirtuale.Add(codiceVirtuale);
                }
            }
        }

        public static void GetListaRegimeLiquidazione(out List<RegimeLiquidazione> listaRegimeLiquidazione)
        {
            listaRegimeLiquidazione = new List<RegimeLiquidazione>();
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.RegimeLiquidazione> listaRegimeLiquidazioneDB = null;
            GestioneDecodifica.GetRegimeLiquidazione(out listaRegimeLiquidazioneDB);
            if (listaRegimeLiquidazioneDB != null)
            {
                foreach (GestioneDecodifica.RegimeLiquidazione regimeLiquidazioneDB in listaRegimeLiquidazioneDB)
                {
                    RegimeLiquidazione regimeLiquidazione = new RegimeLiquidazione();
                    regimeLiquidazione.Id = regimeLiquidazioneDB.Id;
                    regimeLiquidazione.Descrizione = regimeLiquidazioneDB.Descrizione;
                    listaRegimeLiquidazione.Add(regimeLiquidazione);
                }
            }
        }

        private static bool ControlsStatiEsteri(GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, List<StatoEstero> listaStatiEsteri, DatiCalcolo datiCalcolo, bool singleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
            {
                GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiCIGenerici);

                GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

                GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa = null;
                if (danteCausa != null)
                    GestioneAnagrafica.GetAnagraficaByIdAnagrafica(danteCausa.IdAnagrafica, out datiAnagraficiDanteCausa);

                GestionePensione.DatiEliminazione datiEliminazione = null;
                GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);

                List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
                GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out listaResidenzeEstere);

                List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
                GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCtrlCodiceConvenzionePrestazioniEE(out listaCodiciConvenzione);

                if (datiPensione == null)
                {
                    messaggioVideo = "Dati Pensione obbligatori.";
                    return false;
                }

                if (datiCIGenerici == null)
                    datiCIGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                if (singleTab)
                {
                    List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
                    GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiContributivi);

                    List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
                    GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiRetributivi);

                    GestioneContrib.GetDatiCalcoloByDatiPensione(datiPensione, ldatiContributivi, ldatiRetributivi, null, out datiCalcolo, out messaggioVideo);
                }

                int? settimaneRetributiveQuotaA = null;
                int? settimaneRetributiveQuotaB = null;
                int? settimaneContributive = null;
                int? settimaneContributiveDL214 = null;
                if (datiCalcolo != null)
                {
                    if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
                    {
                        foreach (GestioneAggiornamentoPECO.DatiRetributivi retr in datiCalcolo.LDatiRetributivi)
                        {
                            if (retr.CodiceGestione == 1)
                            {
                                if (retr.QuotePrimeLiquidate == 'A')
                                    settimaneRetributiveQuotaA = retr.NSettimaneQuotaA;
                                if (retr.QuotePrimeLiquidate == 'B')
                                    settimaneRetributiveQuotaB = retr.NSettimaneQuotaB;
                            }
                        }
                    }
                    if (datiCalcolo.LDatiContributivi != null && datiCalcolo.LDatiContributivi.Count > 0)
                    {
                        foreach (GestioneAggiornamentoPECO.DatiContributivi contr in datiCalcolo.LDatiContributivi)
                        {
                            if (contr.CodiceGestione == 1)
                            {
                                if (contr.Quota == 'C')
                                    settimaneContributive = contr.Nsettimane;
                                if (contr.Quota == 'D')
                                    settimaneContributiveDL214 = contr.SettimaneQuotaD;
                            }
                        }
                    }
                }

                int? sommaSettimaneItaliane = settimaneRetributiveQuotaA.GetValueOrDefault() + settimaneRetributiveQuotaB.GetValueOrDefault() + (datiCIGenerici != null ? datiCIGenerici.VVMisuraAl1292.GetValueOrDefault() : 0) + (datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392.GetValueOrDefault() : 0) + (datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento.GetValueOrDefault() : 0) + settimaneContributive.GetValueOrDefault() + settimaneContributiveDL214.GetValueOrDefault();
                int? sommaSettimaneEstere = null;
                int? sommaSettimaneDirittoEstere = null;

                foreach (StatoEstero stato in listaStatiEsteri)
                {
                    sommaSettimaneEstere = sommaSettimaneEstere.GetValueOrDefault() + stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria.GetValueOrDefault();
                    sommaSettimaneDirittoEstere = sommaSettimaneDirittoEstere.GetValueOrDefault() + stato.PrestazioneEstera.ContributiEEDiritto.GetValueOrDefault();
                }

                //////////////////////////////// settiamo il numero di settimane in base alla categoria////////////////////
                string categoriaNumerica = datiPensione.GetCodCategoria();
                int categoria = 0;
                int.TryParse(categoriaNumerica, out categoria);
                int? settimane = GestioneControlli.NumeroSettimane(datiCIGenerici != null ? datiCIGenerici.SettimaneItalianeDiritto : null, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null, datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null);
                if (categoria > 0 && categoria < 7)
                {
                    settimane = settimane.GetValueOrDefault() + (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0);
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////

                byte? codiceConvenzione = listaStatiEsteri != null && listaStatiEsteri.Count > 0 ? listaStatiEsteri[0].PrestazioneEstera.CodiceConvenzione : null;
                int codicePrimoStato = 0;
                if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                    int.TryParse(listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE, out codicePrimoStato);

                DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, danteCausa != null ? danteCausa.DecorrenzaPensione : null);

                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

                DateTime? ultimaDecorrenzaResidenzaItaliana = GestioneControlli.GetUltimaDecorrenzaResidenzaItaliana(datiAnagrafici.CodiceComuneResidenza, listaResidenzeEstere, codiceConvenzione);

                int? primoCodiceStatoEE = listaStatiEsteri != null && listaStatiEsteri.Count > 0 && !string.IsNullOrEmpty(listaStatiEsteri.First().PrestazioneEstera.CodiceStatoEE) ? int.Parse(listaStatiEsteri.First().PrestazioneEstera.CodiceStatoEE) : 0;

                if (!GestioneControlli.VerificaSettimaneEffettiveCodiceStatoEE(datiCIGenerici.NContributiItalia, listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE))
                {
                    messaggioVideo = "Settimane Effettive mancanti.";
                    return false;
                }

                #region Controlli OBG Misura 503 o Contributi 335

                //Entity.DatiGenerici datiGenerici = null;
                //GestioneLiquidazionePensione.GetDatiGenerici(numeroDomanda, out datiGenerici);

                //Entity.DatiAssicurativi datiAssicurativi = null;
                //GestioneLiquidazionePensione.GetDatiAssicurativi(numeroDomanda, out datiAssicurativi);

                //if (datiGenerici == null)
                //    datiGenerici = new INPS.Pensioni.LiquidazioneCi.Entity.DatiGenerici();

                //if (datiAssicurativi == null)
                //    datiAssicurativi = new INPS.Pensioni.LiquidazioneCi.Entity.DatiAssicurativi();

                //int? nSettimane = null;
                //if (datiCalcolo.LDatiContributivi != null && datiCalcolo.LDatiContributivi.Count > 0)
                //    nSettimane = datiCalcolo.LDatiContributivi[0].Nsettimane;

                //if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
                //    foreach (GestioneAggiornamentoPECO.DatiRetributivi datiRetributivi in datiCalcolo.LDatiRetributivi)
                //        if (!GestioneControlli.VerificaOBGMisura335Contributi335(datiAssicurativi.FineAssicurazione, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione,
                //            datiRetributivi.NSettimaneQuotaB, nSettimane, datiAssicurativi.CodiceConvenzione, datiCalcolo.NContributiVolontari))
                //        {
                //            messaggioVideo = "OBG Misura 503/92 o Contributi 335/95 mancanti.";
                //            return false;
                //        }

                #endregion Controlli OBG Misura 503 o Contributi 335
                //ENG - Bypassato controllo per stato Francia (01) e istituzione 0509, 0510, 0511
                if (listaStatiEsteri[0].PrestazioneEstera.Confermato.GetValueOrDefault() && !listaStatiEsteri[0].PrestazioneEstera.ContributiEEDecorrenzaOriginaria.HasValue)
                {
                    if (!(!String.IsNullOrEmpty(listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE) && listaStatiEsteri[0].PrestazioneEstera.CodiceStatoEE == "01"
                        && !String.IsNullOrEmpty(listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione)
                        && (listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione == "0509" || listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione == "0510" || listaStatiEsteri[0].PrestazioneEstera.CodiceIstituzione == "0511")))
                    {
                        messaggioVideo = "Settimane estere (primo Stato) mancanti.";
                        return false;
                    }
                }

                #region Categorie minori o uguali a 6
                if (categoria > 0 && categoria <= 6)
                {
                    if (listaStatiEsteri.Count<StatoEstero>(x => x.PrestazioneEstera.Confermato.GetValueOrDefault()) == listaStatiEsteri.Count)
                    {
                        if (!GestioneControlli.ControlsSettimaneWithDecPensioneAndCodRequisitiParticolari(datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, datiPensione.SiglaCategoria, settimane,
                            datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null,
                            sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.ControlsSettimaneWithCodiceSedeAndCertificato(datiPensione, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, settimane,
                            datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, sommaSettimaneDirittoEstere,
                            datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, out messaggioVideo))
                            return false;
                    }
                }
                #endregion Categorie minori o uguali a 6

                int index = 0;
                List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();
                foreach (StatoEstero stato in listaStatiEsteri)
                {
                    if (stato.PrestazioneEstera.Confermato.GetValueOrDefault())
                    {
                        stato.ElencoImportiEsteri.Sort(delegate
                            (GestioneDatiContributiviCi.PensioniCiImportiEsteri c1, GestioneDatiContributiviCi.PensioniCiImportiEsteri c2)
                        { return c1.DecorrenzaPrestazioneEE.Value.CompareTo(c2.DecorrenzaPrestazioneEE.Value); });

                        bool isDecorrenzaResidenzaItalianaOK = GestioneControlli.IsDecorrenzaResidenzaItalianaOK(ultimaDecorrenzaResidenzaItaliana, stato.ElencoImportiEsteri);

                        bool dec_Opz = false;
                        bool dec2000 = false;

                        if (listaStatiEsteri.FindAll(x => (x.PrestazioneEstera.CodiceStatoEE == stato.PrestazioneEstera.CodiceStatoEE && x.PrestazioneEstera.CodiceIstituzione == stato.PrestazioneEstera.CodiceIstituzione)).Count > 1)
                        {
                            messaggioVideo = "Istituzioni uguali: variare CI81";
                            return false;
                        }

                        if (!GestioneControlli.VerificaStatoEsteroInConvenzione(int.Parse(stato.PrestazioneEstera.CodiceStatoEE)))
                        {
                            messaggioVideo = "Stato non in convenzione (" + (index + 1) + "° Stato)";
                            return false;
                        }

                        if (!GestioneControlli.VerificaIstituzioneLussemburgo(datiPensione.CausaCarico, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), int.Parse(stato.PrestazioneEstera.CodiceIstituzione)))
                        {
                            messaggioVideo = "Istituzione Lussemburgo errata: diversa da 0001, 0002, 0003, 0004, 0005, 0501, 0502, 0503";
                            return false;
                        }

                        if (!GestioneControlli.ControlliTurchia(listaCodiciConvenzione, codiceConvenzione, datiPensione.DecorrenzaOriginaria, datiAnagrafici.Cittadinanza, index, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), out messaggioVideo))
                            return false;

                        if (index == 0 && !GestioneControlli.VerificaSloveniaWithDecPensione(int.Parse(stato.PrestazioneEstera.CodiceStatoEE), datiPensione.DecorrenzaOriginaria, codiceConvenzione))
                        {
                            messaggioVideo = "Convenzione " + codiceConvenzione + " incompatibile con Stato SLOVENIA";
                            return false;
                        }

                        if (!GestioneControlli.VerificaSloveniaWithCittadinanza(codiceConvenzione, datiAnagrafici.Cittadinanza, danteCausa != null ? danteCausa.DecorrenzaPensione : null))
                        {
                            messaggioVideo = "Convenzione Slovenia incompatibile con la cittadinanza";
                            return false;
                        }

                        if (!GestioneControlli.VerificaCroaziaWithCittadinanza(codiceConvenzione, datiAnagrafici.Cittadinanza, danteCausa != null ? danteCausa.DecorrenzaPensione : null))
                        {
                            messaggioVideo = "Convenzione Croata incompatibile con la cittadinanza";
                            return false;
                        }

                        if (index == 0)
                        {
                            DateTime? data = GetDecorrenzaRiferimentoWithConvenzione(codiceConvenzione, datiPensione.DecorrenzaOriginaria, datiPensione.Gruppo, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), int.Parse(stato.PrestazioneEstera.CodiceIstituzione));
                            if (data != null)
                            {
                                if (codiceConvenzione == 4)
                                    messaggioVideo = "Decorrenza convenzione '04' non compresa tra 04/1953 e 03/1973";
                                else
                                    messaggioVideo = "Decorrenza anteriore alla convenzione (" + codiceConvenzione + "--> " + String.Format("{0:MM/yyyy}", data) + ")";
                                return false;
                            }

                            if (!GestioneControlli.VerificaCodiceConvenzioneWithStatoEstero(datiPensione, stato.PrestazioneEstera.CodiceStatoEE, stato.PrestazioneEstera.CodiceConvenzione, datiPensione.Gruppo))
                            {
                                messaggioVideo = "Codice Convenzione errato o incompatibile con Stato " + stato.PrestazioneEstera.NomeStato;
                                return false;
                            }

                            if (danteCausa != null)
                            {
                                if (!GestioneControlli.VerificaConvenzioneWithDecorrenzaDiretta(stato.PrestazioneEstera.CodiceConvenzione, decorrenza, int.Parse(stato.PrestazioneEstera.CodiceStatoEE)))
                                {
                                    messaggioVideo = "Codice Convenzione incompatibile con Decorrenza Pensione";
                                    return false;
                                }
                            }

                            if (!GestioneControlli.ControlliSvizzera(settimane, datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null,
                                !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0,
                                listaStatiEsteri.Count > 1 ? int.Parse(listaStatiEsteri[1].PrestazioneEstera.CodiceStatoEE) : 0, datiPensione.DecorrenzaOriginaria, categoria, datiPensione.Gruppo,
                                datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null,
                                stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, datiAnagrafici.Sesso,
                                datiAnagrafici.DataNascita, listaResidenzeEstere, datiEliminazione != null ? datiEliminazione.DecorrenzaEliminazione : null, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaSettimaneSvizzere(codiceConvenzione, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, datiAnagrafici.Cittadinanza, out messaggioVideo))
                                return false;
                        }

                        if (!GestioneControlli.VerificaSettimaneRicalcolo2080(stato.PrestazioneEstera.ContributiEERicalcolo))
                        {
                            messaggioVideo = "Settimane estere a ricalcolo maggiori di 2080";
                            return false;
                        }

                        if (!GestioneControlli.VerificaSettimaneRicalcoloCodeConv(stato.PrestazioneEstera.ContributiEERicalcolo, stato.PrestazioneEstera.CodiceConvenzione, stato.PrestazioneEstera.CodiceStatoEE))
                        {
                            messaggioVideo = "Settimane estere a ricalcolo incompatibile con Stato / Convenzione (17)";
                            return false;
                        }

                        if (stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0)
                        {
                            GestioneDatiContributiviCi.PensioniCiImportiEsteri appImportoEstero = null;
                            int indexImportiEsteri = 0;
                            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importiEsteri in stato.ElencoImportiEsteri)
                            {
                                importiEsteri.IDPrestazioneEE = stato.PrestazioneEstera.Id;
                                if (!GestioneControlli.VerificaSettimaneRicalcoloDecorrEstero(stato.PrestazioneEstera.ContributiEERicalcolo, importiEsteri.DecorrenzaPrestazioneEE))
                                {
                                    messaggioVideo = "Settimane estere a ricalcolo incompatibili con decorrenza estera";
                                    return false;
                                }


                                if (!GestioneControlli.VerificaSettimaneRicalcoloDecEsteroDecOrig(stato.PrestazioneEstera.ContributiEERicalcolo, stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE, datiPensione.DecorrenzaOriginaria))
                                {
                                    messaggioVideo = "Settimane estere a ricalcolo incompatibili con decorrenza estera";
                                    return false;
                                }

                                if (!GestioneControlli.VerificaSettEstereCodeNot17(index, stato.PrestazioneEstera.CodiceStatoEE, stato.PrestazioneEstera.ContributiEERicalcolo, importiEsteri.DecorrenzaPrestazioneEE, codiceConvenzione, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null))
                                {
                                    messaggioVideo = "Settimane estere mancanti o errate";
                                    return false;
                                }

                                if (!GestioneControlli.VerificaPresenzaMatricola(datiPensione.CausaCarico, datiEliminazione != null ? datiEliminazione.CodiceMotivo : null, !string.IsNullOrEmpty(stato.PrestazioneEstera.MatricolaIstituzione) ? stato.PrestazioneEstera.MatricolaIstituzione : !string.IsNullOrEmpty(stato.PrestazioneEstera.MatricolaIstituzioneEE) ? stato.PrestazioneEstera.MatricolaIstituzioneEE : null, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceIstituzione) ? int.Parse(stato.PrestazioneEstera.CodiceIstituzione) : 0, importiEsteri.DecorrenzaPrestazioneEE, out messaggioVideo))
                                    return false;

                                if (appImportoEstero != null)
                                {
                                    if (!Utility.DataStrettamenteSuccessivaA(importiEsteri.DecorrenzaPrestazioneEE.Value, appImportoEstero.DecorrenzaPrestazioneEE.Value))
                                    {
                                        messaggioVideo = "Decorrenza Prestazione Estera non in sequenza";
                                        return false;
                                    }

                                    if (appImportoEstero.CessazionePrestazioneEE.HasValue && !Utility.DataSuccessivaA(importiEsteri.DecorrenzaPrestazioneEE.Value, appImportoEstero.CessazionePrestazioneEE.Value))
                                    {
                                        messaggioVideo = "Decorrenza Prestazione Estera non posteriore a Cessazione precedente";
                                        return false;
                                    }
                                }

                                if (!GestioneControlli.VerificaDecorrenzaImportiEsteriPosterioreADataOdierna(importiEsteri.DecorrenzaPrestazioneEE, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, out messaggioVideo))
                                    return false;

                                if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithDecorrenzaOriginaria(importiEsteri.DecorrenzaPrestazioneEE, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                                    return false;

                                if (indexImportiEsteri > 0)
                                {
                                    if (!GestioneControlli.VerificaMeseDecorrenzaImportiEsteriPerLussemburgo(importiEsteri.DecorrenzaPrestazioneEE, tipoDomanda, appImportoEstero.CessazionePrestazioneEE, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE, out messaggioVideo))
                                        return false;
                                }

                                if (!GestioneControlli.VerificaCompatibilitaImportoWithDecorrenza(importiEsteri.ImportoPrestazioneEE, importiEsteri.DecorrenzaPrestazioneEE, out messaggioVideo))
                                    return false;

                                if (!GestioneControlli.VerificaCoerenzaDecorrenzaCessazione(importiEsteri.DecorrenzaPrestazioneEE, importiEsteri.CessazionePrestazioneEE, out messaggioVideo))
                                    return false;

                                GestioneControlli.GetDecOpz(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, stato.ElencoImportiEsteri, indexImportiEsteri, ref dec_Opz);
                                dec2000 = GestioneControlli.GetDec2000(stato.ElencoImportiEsteri);

                                appImportoEstero = importiEsteri;
                                indexImportiEsteri++;
                            }

                            if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithCodiceVirtuale(stato.ElencoImportiEsteri.Last().DecorrenzaPrestazioneEE, stato.ElencoImportiEsteri.Last().CessazionePrestazioneEE, tipoDomanda, datiCIGenerici != null ? datiCIGenerici.CodiceVirtuale : null, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithCodiceOpzione(datiIstruttoria != null ? datiIstruttoria.CodiceOpzioneRiliquidazione : null, stato.ElencoImportiEsteri.First().DecorrenzaPrestazioneEE, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                                return false;
                        }

                        if (!GestioneControlli.VerificaContribRicalcoloContribDecOrig(stato.PrestazioneEstera.ContributiEERicalcolo, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria))
                        {
                            messaggioVideo = "Settimane estere a ricalcolo incompatibili con decorrenza estera";
                            return false;
                        }

                        if (!GestioneControlli.VerificaContribDirittoNullContribDecOrig(stato.PrestazioneEstera.ContributiEEDiritto, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), int.Parse(stato.PrestazioneEstera.CodiceIstituzione)))
                        {
                            messaggioVideo = "Settimane diritto mancanti o errate";
                            return false;
                        }

                        if (danteCausa != null)
                        {
                            if (!GestioneControlli.VerificaStatiEsteriWithDanteCausa(decorrenza, codiceConvenzione, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].ImportoPrestazioneEE : null))
                            {
                                messaggioVideo = "Settimane esteri mancanti (stato CEE)";
                                return false;
                            }
                        }

                        if (!GestioneControlli.VerificaDataPrecedenteLiquidazioneWithCausaCarico(stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE, datiPensione.CausaCarico, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDataPrecedenteLiquidazione(stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDataPrecedenteLiquidazioneWithDecImportiEsteri(stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDataRicalcolo(stato.PrestazioneEstera.DecorrenzaRicalcolo, datiPensione.Gruppo, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, stato.PrestazioneEstera.ContributiEERicalcolo, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSettimaneARicalcolo(stato.PrestazioneEstera.ContributiEERicalcolo, codiceConvenzione, codicePrimoStato, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, datiPensione.DecorrenzaOriginaria, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaContributiTurchia(listaCodiciConvenzione, codiceConvenzione, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), stato.PrestazioneEstera.ContributiEEDiritto, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaContributiDanimarca(codiceConvenzione, datiPensione.DecorrenzaOriginaria, stato.PrestazioneEstera.CodiceStatoEE, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.PrestazioneEstera.ContributiEEDiritto, datiAnagrafici.Cittadinanza))
                        {
                            messaggioVideo = "Contributi o quota Danesi incompatibili con cittadinanza extraUE.";
                            return false;
                        }

                        if (!GestioneControlli.VerificaContributiDanimarcaDanteCausa(codiceConvenzione, datiPensione.DecorrenzaOriginaria, stato.PrestazioneEstera.CodiceStatoEE,
                            stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.PrestazioneEstera.ContributiEEDiritto,
                            datiAnagraficiDanteCausa != null ? datiAnagraficiDanteCausa.Cittadinanza : string.Empty, datiPensione.Gruppo, datiPensione.Prodotto))
                        {
                            messaggioVideo = "Ctr. o Quota DANESI incompatibili con cittad.extraUE dante causa.";
                            return false;
                        }

                        if (!GestioneControlli.VerificaSospensioneEstero(stato.PrestazioneEstera.SospensioneCautelativaIntegrazione, tipoDomanda, stato.PrestazioneEstera.CodiceArt48, stato.PrestazioneEstera.EtaSospensione, datiAnagrafici.Sesso, datiPensione.CausaCarico, datiAnagrafici.DataNascita, codiceConvenzione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaArticolo48(datiPensione, stato.PrestazioneEstera.CodiceArt48, codiceConvenzione, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null,
                            stato.PrestazioneEstera.DecorrenzaArt48, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria,
                            !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, stato.PrestazioneEstera.ContributiEEDiritto,
                            listaStatiEsteri.Exists(x => x.PrestazioneEstera.CodiceStatoEE.Trim() == "11"), listaStatiEsteri.Exists(x => x.PrestazioneEstera.CodiceStatoEE.Trim() == "20"),
                            listaStatiEsteri.Exists(x => x.PrestazioneEstera.CodiceStatoEE.Trim() == "17"), datiAnagrafici.Cittadinanza,
                            datiIstruttoria != null ? datiIstruttoria.DataDomandaOpzione : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSettimaneEstere(tipoDomanda, codiceConvenzione, datiPensione.DecorrenzaOriginaria, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].ImportoPrestazioneEE : null, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, index, stato.PrestazioneEstera.ContributiEEDiritto, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceIstituzione) ? int.Parse(stato.PrestazioneEstera.CodiceIstituzione) : 0, out messaggioVideo))
                            return false;

                        if (GestioneControlli.GetCodiceConvenzioneByCodiceStatoEE(!string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, datiPensione.DecorrenzaOriginaria) == 0)
                        {
                            messaggioVideo = "Stato/Convenzione errato o mancante";
                            return false;
                        }

                        if (!GestioneControlli.VerificaConvenzioneVaticano(codiceConvenzione, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, datiAnagrafici.Cittadinanza, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaNuovaCaledonia(!string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceIstituzione) ? int.Parse(stato.PrestazioneEstera.CodiceIstituzione) : 0, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, stato.PrestazioneEstera.ContributiEERicalcolo, stato.PrestazioneEstera.ContributiEEDiritto, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, stato.PrestazioneEstera.SospensioneCautelativaIntegrazione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDataRicalcolo(stato.PrestazioneEstera.DecorrenzaRicalcolo, stato.ElencoImportiEsteri, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaObbligatorietaDecorrenzaImportiEsteri(
                            stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, codiceConvenzione,
                            !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0,
                            settimane.GetValueOrDefault() - (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0), stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE,
                            stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri.Last().CessazionePrestazioneEE : null, datiPensione.CausaCarico,
                            datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, ultimaDecorrenzaResidenzaItaliana, isDecorrenzaResidenzaItalianaOK, primoCodiceStatoEE, dec_Opz,
                            datiAnagrafici.CodiceComuneResidenza, dec2000, datiPensione.DecorrenzaOriginaria, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.ControlliVenezuela(tipoDomanda, !string.IsNullOrEmpty(stato.PrestazioneEstera.CodiceStatoEE) ? int.Parse(stato.PrestazioneEstera.CodiceStatoEE) : 0, stato.PrestazioneEstera.SospensioneCautelativaIntegrazione, stato.PrestazioneEstera.ContributiEEDiritto, stato.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, stato.ElencoImportiEsteri, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.ControlliYugoslavia(datiPensione.CausaCarico, codiceConvenzione, datiAnagrafici.CodiceComuneResidenza, int.Parse(stato.PrestazioneEstera.CodiceStatoEE), stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, datiPensione.DecorrenzaOriginaria, stato.PrestazioneEstera.DecorrenzaIntegrazione, stato.PrestazioneEstera.QuotaIntegrazioneEEeArgentinaResidentiItalia, stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE, stato.ElencoImportiEsteri != null && stato.ElencoImportiEsteri.Count > 0 ? stato.ElencoImportiEsteri[0].ImportoPrestazioneEE : null, index, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSospensioneCautelativaIntegrazioneObbligatoria(stato.PrestazioneEstera.SospensioneCautelativaIntegrazione, out messaggioVideo))
                            return false;

                        index++;

                        listaPrestazioniEstere.Add(stato.PrestazioneEstera);
                        listaImportiEsteri.AddRange(stato.ElencoImportiEsteri);
                    }
                }

                if (!GestioneControlli.VerificaCompatibilitaTraStati(codiceConvenzione, listaPrestazioniEstere, datiPensione.DecorrenzaOriginaria, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaPresenzaDecorrenza01_XXper335(listaPrestazioniEstere, listaImportiEsteri, codiceConvenzione, datiAnagrafici.CodiceComuneResidenza, listaResidenzeEstere, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportiEsteriWithCodNatura(listaPrestazioniEstere, listaImportiEsteri, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsSettimaneOBGSettimaneDiritto(datiPensione, tipoDomanda, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, sommaSettimaneDirittoEstere, datiCIGenerici != null ? datiCIGenerici.SettimaneItalianeDiritto : null, datiCIGenerici != null ? datiCIGenerici.SettimaneItalianeMisura : null, out messaggioVideo))
                    return false;

                #region PCIPL39 Categoria >= 7
                if (categoria >= 7)
                {
                    if (listaStatiEsteri.Count<StatoEstero>(x => x.PrestazioneEstera.Confermato.GetValueOrDefault()) == listaStatiEsteri.Count)
                    {
                        if (!GestioneControlli.ControlsSettimaneWithCodReqParticolari(datiPensione.Gruppo, sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, settimane, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.ControlsSettimaneWithCodReqParticolariAndTipoDomanda(tipoDomanda, sommaSettimaneDirittoEstere, datiIstruttoria != null ? datiIstruttoria.NSettGodimentoAssegno : null, settimane, datiIstruttoria != null ? datiIstruttoria.CodiceRequisitiParticolari : null, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiPensione.NaturaPensione, out messaggioVideo))
                            return false;
                    }
                }
                #endregion PCIPL39 Categoria >= 7

                DatiAssicurativi datiAssicurativi;
                GestioneLiquidazionePensione.GetDatiAssicurativi(datiPensione, datiIstruttoria, Utility.IsRiaperturaDomanda(datiPensione.Id), out datiAssicurativi);
                GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

                if (!GestioneControlli.VerificaDataPerfezionamentoPerPensioneTipoContributivo(datiPensione, datiAssicurativi.NSettimaneOBG, datiAssicurativi.NContributiVolontari, datiAssicurativi.SettimaneItalianeDiritto, listaPrestazioniEstere,
                   datiAnagraficiTitolare, Utility.DataSistemaCi, out messaggioVideo))
                {
                    return false;
                }
            }

            return true;
        }

        public static DateTime? GetDecorrenzaRiferimentoWithConvenzione(byte? codiceConvenzione, DateTime? decPensione, string gruppo, int codiceStato, int istituzione)
        {
            DateTime dataCompare;

            if (!codiceConvenzione.HasValue)
                return null;

            dataCompare = new DateTime(1988, 09, 01);
            if (codiceConvenzione.Value == 33 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1977, 09, 01);
            if (codiceConvenzione.Value == 21 && gruppo == "0001" && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1965, 03, 01);
            if (codiceConvenzione.Value == 21 && (gruppo == "0002" || gruppo == "0003") && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1979, 01, 01);
            if ((codiceConvenzione.Value == 24 || codiceConvenzione.Value == 25) && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1983, 11, 01);
            if (codiceConvenzione.Value == 30 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1980, 03, 01);
            if (codiceConvenzione.Value == 27 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1975, 11, 01);
            if (codiceConvenzione.Value == 22 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            if (codiceConvenzione.Value == 4)
            {
                dataCompare = new DateTime(1953, 04, 01);
                if (codiceStato == 4 && istituzione != 6 && (decPensione.Value.CompareTo(dataCompare) < 0 || decPensione.Value.CompareTo(new DateTime(1973, 03, 01)) > 0))
                    return dataCompare;

                if (codiceStato == 4 && istituzione == 6 && decPensione.Value.CompareTo(dataCompare) < 0)
                    return dataCompare;
            }

            dataCompare = new DateTime(1958, 05, 01);
            if (codiceConvenzione.Value == 26 && istituzione == 1 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1967, 01, 01);
            if (codiceConvenzione.Value == 26 && istituzione == 2 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1962, 01, 01);
            if (codiceConvenzione.Value == 17 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1987, 06, 01);
            if (codiceConvenzione.Value == 34 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1987, 06, 01);
            if (codiceConvenzione.Value == 34 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1978, 11, 01);
            if (codiceConvenzione.Value == 23 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1985, 06, 01);
            if (codiceConvenzione.Value == 31 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1991, 11, 01);
            if (codiceConvenzione.Value == 37 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1961, 01, 01);
            if (codiceConvenzione.Value == 13 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(2002, 08, 01);
            if (codiceConvenzione.Value == 38 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(2003, 11, 01);
            if (codiceConvenzione.Value == 39 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(1986, 01, 01);
            if (codiceConvenzione.Value == 11 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(2004, 05, 01);
            if (codiceConvenzione.Value == 12 && (codiceStato == 44 || codiceStato == 45 || codiceStato == 46 || codiceStato == 47 || codiceStato == 48 || codiceStato == 49 ||
                codiceStato == 50 || codiceStato == 51 || codiceStato == 52) && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(2004, 01, 01);
            if (codiceConvenzione.Value == 53 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(2007, 01, 01);
            if (codiceConvenzione.Value == 12 && (codiceStato == 54 || codiceStato == 55) && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            dataCompare = new DateTime(2020, 01, 01);
            if (codiceConvenzione.Value == 60 && decPensione.Value.CompareTo(dataCompare) < 0)
                return dataCompare;

            return null;
        }

        #region internal members
        internal static void GetPrestazioniEstereFromCI05(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            out List<PrestazioneEstera> listaPrestazioniEstere, out string cittadinanzaTitolare, out string messaggioVideo)
        {
            listaPrestazioniEstere = null;
            cittadinanzaTitolare = string.Empty;
            messaggioVideo = "";
            Data.CI05Lettura AreaPrelievo = null;
            Guid guid = Guid.NewGuid();

            ValorizzaAreaPrelievo(numeroDomanda, codiceSede, centroOperativo, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out AreaPrelievo, out messaggioVideo);

            GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Request, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI05, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);

            if (!String.IsNullOrEmpty(messaggioVideo))
                return;
            EseguiPrelievo(AreaPrelievo);

            GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Response, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.CI05, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);

            ControllaEsitoPrelievo(AreaPrelievo, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
                return;
            NormalizzaArea(AreaPrelievo, out listaPrestazioniEstere, out cittadinanzaTitolare);
        }
        #endregion internal members

        #region private members

        private static void ValorizzaAreaPrelievo(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out Data.CI05Lettura AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            if (numeroDomanda == 0 || String.IsNullOrEmpty(matricolaOperatore) || matricolaOperatore.Trim() == "" || sedeOperatore == 0)
            {
                messaggioVideo = "Richiesta non valorizzata correttamente";
                return;
            }

            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(sedeOperatore.ToString().PadLeft(4, '0') + centroOperativoOperatore.ToString().PadLeft(2, '0'));

            string sede = String.Format("{0}{1}", codiceSede.ToString().PadLeft(4, '0'), centroOperativo.ToString().PadLeft(2, '0'));
            AreaPrelievo = new CI05Lettura("PCICS10", matricolaOperatore, sede, "0", numeroDomanda);
        }

        private static void EseguiPrelievo(Data.CI05Lettura AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void ControllaEsitoPrelievo(Data.CI05Lettura AreaPrelievo, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                messaggioVideo = AreaPrelievo.Messaggio;
        }

        private static void NormalizzaArea(Data.CI05Lettura AreaPrelievo, out List<PrestazioneEstera> listaPrestazioniEstere, out string cittadinanzaTitolare)
        {
            listaPrestazioniEstere = null;
            cittadinanzaTitolare = string.Empty;
            if (AreaPrelievo != null && AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Richiedente != null && AreaPrelievo.FinalResponse.Richiedente.StatiEsteri != null)
            {
                foreach (Data.HostResponse.CI05AreaRichiedente.Area_StatiEsteri prestazioneEstera in AreaPrelievo.FinalResponse.Richiedente.StatiEsteri)
                {
                    Data.aciistit descPrestazioneEstera = null;
                    Data.DAPrestazioniEstere.GetPrestazioneEstera(prestazioneEstera.A_STIS.ToString().PadLeft(6, '0'), out descPrestazioneEstera);
                    if (descPrestazioneEstera != null)
                    {
                        if (listaPrestazioniEstere == null)
                            listaPrestazioniEstere = new List<PrestazioneEstera>();
                        listaPrestazioniEstere.Add(new PrestazioneEstera(descPrestazioneEstera.CDSTAIST, descPrestazioneEstera.SIGLISTI,
                            descPrestazioneEstera.CITTAIST, descPrestazioneEstera.NOMESTAT, descPrestazioneEstera.SIGLASTAT, descPrestazioneEstera.CODICONV,
                            !String.IsNullOrEmpty(prestazioneEstera.A_MATRE) ? prestazioneEstera.A_MATRE.Trim() : "", string.IsNullOrEmpty(prestazioneEstera.A_PI) ? (char?)null : prestazioneEstera.A_PI[0], false));
                    }
                }
            }
            if (AreaPrelievo != null && AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Richiedente != null && AreaPrelievo.FinalResponse.Richiedente.CI2005 != null)
            {
                string siglaNazione = AreaPrelievo.FinalResponse.Richiedente.CI2005.A_CITT1;
                if (!string.IsNullOrEmpty(siglaNazione) && siglaNazione.Trim() != string.Empty)
                {
                    List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
                    GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);
                    if (listaStatiEsteri != null)
                    {
                        // Viene effettuata questa modifica perchè dal CI05 nel caso di Italia ci arriva "I", mentre nella listaStatiEsteri abbiamo "ITA"
                        if (siglaNazione.Trim() == "I")
                            siglaNazione = "ITA";

                        List<GestioneDecodifica.StatoEstero> statiEsteri = listaStatiEsteri.FindAll(x => x.Sigla.Trim().ToUpperInvariant() == siglaNazione.Trim().ToUpperInvariant());
                        if (statiEsteri != null && statiEsteri.Count == 1 && statiEsteri[0] != null)
                        {
                            cittadinanzaTitolare = statiEsteri[0].CodCatastale;
                        }
                    }
                }
            }
        }

        #endregion private members

        #endregion ProRata

        #region DatiCalcolo

        public static void GetDatiCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi,
            List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi, GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec, out DatiCalcolo datiCalcolo, out string messaggioVideo)
        {
            datiCalcolo = null;
            messaggioVideo = string.Empty;
            bool IsDataFromDb = false;

            if (datiPensione != null)
            {
                List<GestioneCalcolo.DatiCalcoloContributivoEstero> ldatiContributivoEE = null;
                GestioneCalcolo.GetCalcoloContributivoEsteroCIbyIdPensione(datiPensione.Id, out ldatiContributivoEE);

                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);

                if (!(Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && ldatiContributivi == null && ldatiRetributivi == null) || (Utility.IsRiaperturaDomanda(datiPensione.Id)) || (Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault()))
                {
                    IsDataFromDb = true;
                    if (datiAggPec == null)
                        datiAggPec = new GestioneAggiornamentoPECO.DatiTotaliAggPec();

                    if (ldatiContributivi != null)
                    {
                        datiAggPec.lContribuzione = new List<GestioneAggiornamentoPECO.DatiContributivi>();
                        foreach (GestioneCalcolo.DatiCalcoloContributivo calContr in ldatiContributivi)
                        {
                            GestioneAggiornamentoPECO.DatiContributivi datiContr = new GestioneAggiornamentoPECO.DatiContributivi();
                            datiContr.CodiceGestione = calContr.CodiceGestione;
                            if (calContr.ImportoContributivoTotale.HasValue || calContr.Montante.HasValue || calContr.NSettimane.HasValue)
                            {
                                datiContr.Quota = 'C';
                                datiContr.ImportoContributivoTotale = calContr.ImportoContributivoTotale;
                                datiContr.MontanteContributivo = calContr.Montante;
                                datiContr.Nsettimane = calContr.NSettimane;
                            }
                            else if (calContr.ImportoContribTotaleQuotaDL214.HasValue || datiContr.MontanteContributivoQuotaD.HasValue || calContr.NSettimaneQuotaDL214.HasValue)
                            {
                                datiContr.Quota = 'D';
                                datiContr.ImportoContributivoQuotaD = calContr.ImportoContribTotaleQuotaDL214;
                                datiContr.MontanteContributivoQuotaD = calContr.MontanteQuotaDL214;
                                datiContr.SettimaneQuotaD = calContr.NSettimaneQuotaDL214;
                            }

                            datiAggPec.lContribuzione.Add(datiContr);
                        }
                    }
                    if (ldatiRetributivi != null)
                    {
                        datiAggPec.lRetribuzione = new List<GestioneAggiornamentoPECO.DatiRetributivi>();
                        foreach (GestioneCalcolo.DatiCalcoloRetributivo calcRetr in ldatiRetributivi)
                        {
                            GestioneAggiornamentoPECO.DatiRetributivi datiRetr = new GestioneAggiornamentoPECO.DatiRetributivi();
                            if (calcRetr.QuotePrimeLiquidate.HasValue)
                            {
                                datiRetr.QuotePrimeLiquidate = calcRetr.QuotePrimeLiquidate;
                                if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                                {
                                    datiRetr.NSettimaneQuotaA = calcRetr.NSettimaneQuotaA;
                                    datiRetr.RMSQuotaA = calcRetr.RMSQuotaA;
                                }
                                else if (calcRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                                {
                                    datiRetr.NSettimaneQuotaB = calcRetr.NSettimaneQuotaB;
                                    datiRetr.RMSQuotaB = calcRetr.RMSQuotaB;
                                }
                            }
                            datiRetr.CodiceGestione = calcRetr.CodiceGestione;
                            datiRetr.DecorrenzaOriginariaPensione = calcRetr.DecorrenzaOriginariaPensione;
                            datiRetr.Nsettimane707 = calcRetr.NSettimane707;

                            datiAggPec.lRetribuzione.Add(datiRetr);
                        }
                    }
                    if (ldatiContributivoEE != null)
                    {
                        datiAggPec.lContribuzioneEE = new List<GestioneAggiornamentoPECO.DatiContributiEsteri>();
                        foreach (GestioneCalcolo.DatiCalcoloContributivoEstero calcContrEE in ldatiContributivoEE)
                        {
                            GestioneAggiornamentoPECO.DatiContributiEsteri datiContrEE = new GestioneAggiornamentoPECO.DatiContributiEsteri();
                            datiContrEE.CodiceGestione = calcContrEE.CodiceGestione;
                            datiContrEE.Decorrenza = calcContrEE.Decorrenza;
                            datiContrEE.Settimane = calcContrEE.Settimane;

                            datiAggPec.lContribuzioneEE.Add(datiContrEE);
                        }
                    }
                }

                if (datiAggPec != null && !datiAggPec.IsNull())
                {
                    GestioneAggiornamentoPECO.ImpostaDatiControllo(datiAggPec, out messaggioVideo);
                    datiCalcolo = new DatiCalcolo(datiAggPec);
                }
                else
                {
                    datiCalcolo = new DatiCalcolo();
                    datiCalcolo.LDatiContributivi = null;
                    datiCalcolo.LDatiRetributivi = null;
                    datiCalcolo.LDatiContributiEsteri = null;
                }
                if (datiPensione != null)
                {
                    datiCalcolo.IsUnicarpe = Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica;
                    datiCalcolo.IdPensione = datiPensione.Id;
                    datiCalcolo.FineAssicurazione = datiPensione.FineAssicurazione;
                    datiCalcolo.InizioAssicurazione = datiPensione.InizioAssicurazione;
                }

                if (datiGenerici != null)
                {
                    if (!datiCalcolo.ContributiItalianiEdEsteriAl1295.HasValue) // Il dato può arrivare da Felpe
                        datiCalcolo.ContributiItalianiEdEsteriAl1295 = datiGenerici.ContributiItalianiEdEsteriAl1295;
                    datiCalcolo.CTRMaternitaAcna = datiGenerici.MaternitaAcna;
                    datiCalcolo.MontanteInvalidita = datiGenerici.CMSM;
                }

                datiCalcolo.IsDataFromDB = IsDataFromDb;
            }
        }

        public static void StoreDatiCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate,
            ref Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, DatiCalcolo datiCalcolo, List<MaternitaAcna> LmaternitaAcna, GestioneContrib.ProRata proRata,
            bool singleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            long? longNull = null;
            List<GestioneCalcolo.DatiCalcoloContributivo> lContribuzione = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> lRetribuzione = null;
            List<GestioneCalcolo.DatiCalcoloContributivoEstero> lContributivoEstero = null;

            if (ControlsDatiCalcolo(datiCalcolo, datiPensione, datiIstruttoriaCommon, datiNuoveLiquidate, datiMaggiorazioniBeneficiCommon, LmaternitaAcna, proRata, singleTab, out messaggioVideo))
            {
                if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
                {
                    lRetribuzione = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                    foreach (GestioneAggiornamentoPECO.DatiRetributivi calRetr in datiCalcolo.LDatiRetributivi)
                    {
                        GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                        datiRetr.IdPensione = datiPensione.Id;
                        datiRetr.DecorrenzaOriginariaPensione = calRetr.DecorrenzaOriginariaPensione;
                        datiRetr.CodiceGestione = calRetr.CodiceGestione;
                        datiRetr.QuotePrimeLiquidate = calRetr.QuotePrimeLiquidate;
                        if (datiRetr.QuotePrimeLiquidate.HasValue && datiRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                        {
                            datiRetr.RMSQuotaA = calRetr.RMSQuotaA;
                            datiRetr.NSettimaneQuotaA = calRetr.NSettimaneQuotaA;
                        }
                        else if (datiRetr.QuotePrimeLiquidate.HasValue && datiRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                        {
                            datiRetr.RMSQuotaB = calRetr.RMSQuotaB;
                            datiRetr.NSettimaneQuotaB = calRetr.NSettimaneQuotaB;
                        }
                        datiRetr.NSettimane707 = calRetr.Nsettimane707;
                        lRetribuzione.Add(datiRetr);
                        //if (ControlsDatiRetributivi(datiRetr, out messaggioVideo))
                        //    lRetribuzione.Add(datiRetr);
                        //else
                        //    return;
                    }
                }

                if (datiCalcolo.LDatiContributivi != null && datiCalcolo.LDatiContributivi.Count > 0)
                {
                    lContribuzione = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                    foreach (GestioneAggiornamentoPECO.DatiContributivi calContr in datiCalcolo.LDatiContributivi)
                    {
                        GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();

                        datiContr.IdPensione = datiPensione.Id;
                        datiContr.CodiceGestione = calContr.CodiceGestione;

                        if (calContr.Quota.HasValue && calContr.Quota.Value.ToString().ToUpperInvariant() == "C")
                        {
                            datiContr.ImportoContributivoTotale = calContr.ImportoContributivoTotale;
                            datiContr.Montante = calContr.MontanteContributivo;
                            datiContr.NSettimane = calContr.Nsettimane;
                        }
                        else if (calContr.Quota.HasValue && calContr.Quota.Value.ToString().ToUpperInvariant() == "D")
                        {
                            datiContr.ImportoContribTotaleQuotaDL214 = calContr.ImportoContributivoQuotaD;
                            datiContr.MontanteQuotaDL214 = calContr.MontanteContributivoQuotaD;
                            datiContr.NSettimaneQuotaDL214 = calContr.SettimaneQuotaD;
                        }

                        lContribuzione.Add(datiContr);
                        //if (ControlsDatiContributivi(datiContr, out messaggioVideo))
                        //    lContribuzione.Add(datiContr);
                        //else
                        //    return;
                    }
                }
                if (datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
                {
                    lContributivoEstero = new List<GestioneCalcolo.DatiCalcoloContributivoEstero>();
                    foreach (GestioneAggiornamentoPECO.DatiContributiEsteri calContrEE in datiCalcolo.LDatiContributiEsteri)
                    {
                        GestioneCalcolo.DatiCalcoloContributivoEstero datiContrEE = new GestioneCalcolo.DatiCalcoloContributivoEstero();

                        datiContrEE.CodiceGestione = calContrEE.CodiceGestione.HasValue ? Convert.ToInt64(calContrEE.CodiceGestione) : longNull;
                        datiContrEE.Decorrenza = calContrEE.Decorrenza;
                        datiContrEE.Settimane = calContrEE.Settimane;
                        datiContrEE.IdPensione = datiPensione.Id;

                        if (ControlsDatiContributiviEE(datiContrEE, out messaggioVideo))
                            lContributivoEstero.Add(datiContrEE);
                        else
                            return;
                    }
                }

                GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);

                if (datiGenerici != null)
                {
                    // i dati provenienti da felpe sono non modificabili e non cancellabili
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica)
                    {
                        datiCalcolo.ContributiItalianiEdEsteriAl1295 = datiGenerici.ContributiItalianiEdEsteriAl1295;
                        datiCalcolo.MontanteInvalidita = datiGenerici.CMSM;
                    }
                }

                if (datiCalcolo.ContributiItalianiEdEsteriAl1295.HasValue || datiCalcolo.CTRMaternitaAcna.HasValue)
                {
                    if (datiGenerici == null)
                        datiGenerici = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();
                    datiGenerici.ContributiItalianiEdEsteriAl1295 = datiCalcolo.ContributiItalianiEdEsteriAl1295;
                    datiGenerici.MaternitaAcna = datiCalcolo.CTRMaternitaAcna;
                    datiGenerici.CMSM = datiCalcolo.MontanteInvalidita;
                }

                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

                #region Get dati per gestione visibilità Tab Maternità / Acna

                List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listPensioniCiMaternitaAcna = null;
                GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out listPensioniCiMaternitaAcna);

                #endregion Get dati per gestione visibilità Tab Maternità / Acna

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (datiGenerici != null)
                        GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);

                    GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
                    if (lContribuzione != null && lContribuzione.Count > 0)
                        GestioneCalcolo.SalvaListCalcoloContributivoCI_AGO(lContribuzione);

                    GestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(datiPensione.Id, false);
                    if (lRetribuzione != null && lRetribuzione.Count > 0)
                        GestioneCalcolo.SalvaListaCalcoloRetributivoCI_AGO(lRetribuzione);

                    GestioneCalcolo.EliminaCalcoloContributivoEsteroCIByIdPensione(datiPensione.Id);
                    if (lContributivoEstero != null && lContributivoEstero.Count > 0)
                        GestioneCalcolo.SalvaListCalcoloContributivoEsteroCI(lContributivoEstero);

                    datiQuadroDatiContributivi.TabDatiCalcolo = 2;

                    #region Gestione visibilità Tab Maternità / Acna

                    if (datiCalcolo.CTRMaternitaAcna.HasValue && datiCalcolo.CTRMaternitaAcna.Value)
                    {
                        if (datiQuadroDatiContributivi.TabMaternAcna == 1)
                            datiQuadroDatiContributivi.TabMaternAcna = 0;
                    }
                    else
                    {
                        if (listPensioniCiMaternitaAcna == null || listPensioniCiMaternitaAcna.Count == 0)
                            datiQuadroDatiContributivi.TabMaternAcna = 1;
                    }

                    ////////if (datiCalcolo.CTRMaternitaAcna.HasValue && datiCalcolo.CTRMaternitaAcna.Value)
                    ////////    if (listPensioniCiMaternitaAcna == null || listPensioniCiMaternitaAcna.Count == 0)
                    ////////        datiQuadroDatiContributivi.TabMaternAcna = 0;
                    ////////    else
                    ////////        datiQuadroDatiContributivi.TabMaternAcna = 2;
                    ////////else
                    ////////    datiQuadroDatiContributivi.TabMaternAcna = null;

                    #endregion Gestione visibilità Tab Maternità / Acna

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }
            }
        }

        //public static bool ControlsDatiRetributivi(GestioneCalcolo.DatiCalcoloRetributivo datiRetr, out string errore)
        //{
        //    errore = string.Empty;
        //    return true;
        //}

        //public static bool ControlsDatiContributivi(GestioneCalcolo.DatiCalcoloContributivo datiContr, out string errore)
        //{
        //    errore = string.Empty;
        //    return true;
        //}

        public static bool ControlsDatiContributiviEE(GestioneCalcolo.DatiCalcoloContributivoEstero datiContrEE, out string errore)
        {
            errore = string.Empty;
            return true;
        }

        public static void DeleteDatiCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
            try
            {
                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);

                if (datiGenerici != null)
                {
                    datiGenerici.CMSM = null;
                    datiGenerici.ContributiItalianiEdEsteriAl1295 = null;
                    datiGenerici.RMS9090 = null;
                    datiGenerici.RMS8888 = null;
                    datiGenerici.SettimanePerCalcoloContributivo = null;
                    datiGenerici.MaternitaAcna = null;
                }

                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

                #region Get dati per gestione visibilità Tab Maternità / Acna

                List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listPensioniCiMaternitaAcna = null;
                GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out listPensioniCiMaternitaAcna);

                #endregion Get dati per gestione visibilità Tab Maternità / Acna

                if (listPensioniCiMaternitaAcna != null && listPensioniCiMaternitaAcna.Count > 0)
                {
                    messaggioVideo = "Cancellare i dati della tab Maternità / Acna prima di procedere con la cancellazione dei Dati Calcolo";
                    return;
                }

                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (datiGenerici != null)
                    {
                        if (GestioneDatiGenericiAgoCi.IsDatiGenericiNull(datiGenerici))
                            GestioneDatiGenericiAgoCi.EliminaDatiGenericiByIdPensione(datiPensione.Id);
                        else
                            GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);
                    }

                    GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensione.Id, datiGenerici);
                    GestioneCalcolo.EliminaCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, false);
                    GestioneCalcolo.EliminaCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, false);
                    GestioneCalcolo.EliminaCalcoloContributivoEsteroCIByIdPensione(datiPensione.Id);

                    if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    {
                        datiQuadroDatiContributivi.Tipo = 2;
                        datiQuadroDatiContributivi.TabDatiCalcolo = 0;
                    }
                    else
                        datiQuadroDatiContributivi.TabDatiCalcolo = 1;

                    datiQuadroDatiContributivi.TabMaternAcna = 1;

                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }

        private static bool ControlsDatiCalcolo(DatiCalcolo datiCalcolo, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaCommon, GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate,
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiCommon, List<MaternitaAcna> LmaternitaAcna, GestioneContrib.ProRata proRata, bool singleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (singleTab)
                GestioneContrib.GetDatiMaternitaAcnaByIdPensione(datiPensione.Id, out LmaternitaAcna);

            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaCodeGestioneCalcoloRetributivo = null;
            GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out listaCodeGestioneCalcoloRetributivo);
            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaCodeGestioneCalcoloContributivo = null;
            GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out listaCodeGestioneCalcoloContributivo);
            List<GestioneDecodifica.CodeGestione> listaCodeGestione = null;
            GestioneDecodifica.GetCodiceGestione(out listaCodeGestione);

            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);

            Utility.TipoAppartenenza? tipo = INPS.Pensioni.Liquidazione.BLCommon.Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipo != Utility.TipoAppartenenza.CI)
                return false;

            if (datiCalcolo == null || ((datiCalcolo.LDatiContributiEsteri == null || datiCalcolo.LDatiContributiEsteri.Count == 0) &&
                                        (datiCalcolo.LDatiContributivi == null || datiCalcolo.LDatiContributivi.Count == 0) &&
                                        (datiCalcolo.LDatiRetributivi == null || datiCalcolo.LDatiRetributivi.Count == 0)))
            {
                messaggioVideo = "Inserire i dati obbligatori nella tab 'Dati Calcolo' prima di salvare.";
                return false;
            }

            if (!datiPensione.FineAssicurazione.HasValue)
            {
                messaggioVideo = "Data 'Fine Assicurazione' assente; verificare nella sezione 'Liquidazione Pensione'.";
                return false;
            }

            bool IsDomandaStandard = false;
            if (!string.IsNullOrEmpty(datiPensione.SiglaCategoria) && (datiPensione.SiglaCategoria.ToUpperInvariant().Trim() == "VOS" || datiPensione.SiglaCategoria.ToUpperInvariant().Trim() == "IOS" || datiPensione.SiglaCategoria.ToUpperInvariant().Trim() == "SOS"))
                IsDomandaStandard = true;

            if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
            {
                if (IsDomandaStandard)
                {
                    if (datiCalcolo.LDatiRetributivi.Count > 2)
                    {
                        messaggioVideo = "Dati Retributivi: è possibile acquisire al più due record, per una domanda di categoria 'VOS, IOS o SOS'.";
                        return false;
                    }
                    foreach (GestioneAggiornamentoPECO.DatiRetributivi Retrb in datiCalcolo.LDatiRetributivi)
                    {
                        if (!Retrb.CodiceGestione.HasValue || Retrb.CodiceGestione.Value == 0)
                        {
                            messaggioVideo = "Dati Retributivi: Codice gestione obbligatorio.";
                            return false;
                        }
                        if (Retrb.CodiceGestione.HasValue && Retrb.CodiceGestione.Value != 1)
                        {
                            messaggioVideo = "Dati Retributivi: Codice gestione deve essere obligatoriamente: 'gestione OBG'.";
                            return false;
                        }
                    }

                    //List<GestioneAggiornamentoPECO.DatiRetributivi> listApp = datiCalcolo.LDatiRetributivi.FindAll(delegate(GestioneAggiornamentoPECO.DatiRetributivi retr1)
                    //{
                    //    return datiCalcolo.LDatiRetributivi.FindAll(delegate(GestioneAggiornamentoPECO.DatiRetributivi retr2)
                    //    {
                    //        return (retr1.CodiceGestione == retr2.CodiceGestione && retr1.QuotePrimeLiquidate == retr2.QuotePrimeLiquidate);
                    //    }).Count() > 1;
                    //}).ToList();

                    //if (listApp.Count > 1)
                    //{
                    //    messaggioVideo = "Dati Retributivi: non può essere presente più di una occorrenza con lo stesso codice gestione e la stessa quota.";
                    //    return false;
                    //}

                }
                else
                {
                    foreach (GestioneAggiornamentoPECO.DatiRetributivi Retrb in datiCalcolo.LDatiRetributivi)
                    {
                        if (Retrb.QuotePrimeLiquidate.HasValue && Retrb.QuotePrimeLiquidate.Value == 'A')
                        {
                            if (!Retrb.CodiceGestione.HasValue || Retrb.CodiceGestione.Value == 0)
                            {
                                messaggioVideo = "Dati Retributivi: Codice gestione obbligatorio.";
                                return false;
                            }
                            if (!Retrb.RMSQuotaA.HasValue || Retrb.RMSQuotaA.Value == 0)
                            {
                                messaggioVideo = "Dati Retributivi: RMS Quota A obbligatorio.";
                                return false;
                            }
                            if (!Retrb.NSettimaneQuotaA.HasValue || Retrb.NSettimaneQuotaA.Value == 0)
                            {
                                messaggioVideo = "Dati Retributivi: Settimane Quota A obbligatorio.";
                                return false;
                            }
                        }

                        if (Retrb.QuotePrimeLiquidate.HasValue && Retrb.QuotePrimeLiquidate.Value == 'B')
                        {
                            if (!Retrb.RMSQuotaB.HasValue || Retrb.RMSQuotaB.Value == 0)
                            {
                                messaggioVideo = "Dati Retributivi: RMS Quota B obbligatorio.";
                                return false;
                            }
                        }
                    }
                }

                List<GestioneAggiornamentoPECO.DatiRetributivi> listApp = datiCalcolo.LDatiRetributivi.FindAll(delegate (GestioneAggiornamentoPECO.DatiRetributivi retr1)
                {
                    return datiCalcolo.LDatiRetributivi.FindAll(delegate
                        (GestioneAggiornamentoPECO.DatiRetributivi retr2)
                    {
                        return (retr1.CodiceGestione == retr2.CodiceGestione && retr1.QuotePrimeLiquidate == retr2.QuotePrimeLiquidate);
                    }).Count > 1;
                }).ToList();

                if (listApp.Count > 1)
                {
                    messaggioVideo = "Dati Retributivi: non può essere presente più di una occorrenza con lo stesso codice gestione e la stessa quota.";
                    return false;
                }
            }

            if (datiCalcolo.LDatiContributivi != null && datiCalcolo.LDatiContributivi.Count > 0)
            {
                if (IsDomandaStandard)
                {
                    if (datiCalcolo.LDatiContributivi.Count > 2)
                    {
                        messaggioVideo = "Dati Calcolo: è possibile acquisire al più due record di tipo dati calcolo, per una domanda di categoria 'VOS, IOS o SOS'.";
                        return false;
                    }

                    foreach (GestioneAggiornamentoPECO.DatiContributivi Contr in datiCalcolo.LDatiContributivi)
                    {
                        if (!Contr.CodiceGestione.HasValue || Contr.CodiceGestione.Value == 0)
                        {
                            messaggioVideo = "Dati Calcolo: Codice gestione obbligatorio.";
                            return false;
                        }

                        if (Contr.CodiceGestione.HasValue && Contr.CodiceGestione.Value != 1)
                        {
                            messaggioVideo = "Dati Calcolo: Codice gestione deve essere obligatoriamente: 'gestione FPLD'.";
                            return false;
                        }

                        if (Contr.Quota == 'C' && Contr.Nsettimane.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Tipo Calcolo Contributivo: Settimane mancanti";
                            return false;
                        }

                        if (Contr.Quota == 'C' && Contr.ImportoContributivoTotale.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Tipo Calcolo Contributivo: Importo Contributi mancante";
                            return false;
                        }

                        if (Contr.Quota == 'C' && Contr.MontanteContributivo.GetValueOrDefault() == 0)
                        {
                            messaggioVideo = "Tipo Calcolo Contributivo: Montante Contributi mancante";
                            return false;
                        }
                    }
                }

                List<GestioneAggiornamentoPECO.DatiContributivi> listApp = datiCalcolo.LDatiContributivi.FindAll(delegate (GestioneAggiornamentoPECO.DatiContributivi contr1)
                {
                    return datiCalcolo.LDatiContributivi.FindAll(delegate
                        (GestioneAggiornamentoPECO.DatiContributivi contr2)
                    {
                        return (contr1.CodiceGestione == contr2.CodiceGestione && contr1.Quota == contr2.Quota);
                    }).Count > 1;
                }).ToList();

                if (listApp.Count > 1)
                {
                    messaggioVideo = "Dati Calcolo: non può essere presente più di una occorrenza con lo stesso codice gestione e la stessa quota.";
                    return false;
                }
            }

            if (datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
            {
                datiCalcolo.LDatiContributiEsteri.Sort(delegate
                    (GestioneAggiornamentoPECO.DatiContributiEsteri c1, GestioneAggiornamentoPECO.DatiContributiEsteri c2)
                { return c1.Decorrenza.Value.CompareTo(c2.Decorrenza); });

                int index = 0;
                GestioneAggiornamentoPECO.DatiContributiEsteri contrEsteroApp = null;
                foreach (GestioneAggiornamentoPECO.DatiContributiEsteri contrEstero in datiCalcolo.LDatiContributiEsteri)
                {
                    if (index == 0)
                        contrEsteroApp = contrEstero;
                    else
                    {
                        if (!Utility.DataSuccessivaA(contrEstero.Decorrenza.Value, contrEsteroApp.Decorrenza.Value))
                        {
                            messaggioVideo = "Decorrenze Contributi Esteri non in sequenza";
                            return false;
                        }
                    }

                    if ((contrEstero.CodiceGestione.HasValue || contrEstero.Decorrenza.HasValue) && (!contrEstero.CodiceGestione.HasValue || !contrEstero.Decorrenza.HasValue))
                    {
                        messaggioVideo = "Registrazioni Contributi Esteri incomplete";
                        return false;
                    }

                    //Check presenza codice gestione in lista dati contributivi e lista dati retributivi
                    var codeGestione = listaCodeGestione.Find(x => x.Id == contrEstero.CodiceGestione.Value);
                    if (codeGestione != null)
                    {
                        switch (codeGestione.Legge)
                        {
                            case "335":
                                // Se non esiste un elemento nei dati contributivi che abbia traduzione su GP X o XH, dove X corrisponde a 1, 2, 3 o 4
                                if (!datiCalcolo.LDatiContributivi.Exists(x =>
                                    listaCodeGestioneCalcoloContributivo.Exists(y => y.TraduzioneSuGP == x.CodiceGestione.GetValueOrDefault().ToString() ||
                                        y.TraduzioneSuGP == x.CodiceGestione.GetValueOrDefault().ToString() + "H")))
                                {
                                    messaggioVideo = "Codice Gestione Contributi Esteri non presente nella sezione Dati Contributivi";
                                    return false;
                                }
                                break;
                            case "503":
                                // Se non esiste un elemento nei dati retributivi che abbia quota B e traduzione su GP X o XH, dove X corrisponde a 1, 2, 3 o 4
                                if (!datiCalcolo.LDatiRetributivi.Exists(x => x.QuotePrimeLiquidate.GetValueOrDefault() == 'B' &&
                                    listaCodeGestioneCalcoloRetributivo.Exists(y => y.Id == x.CodiceGestione &&
                                    (y.TraduzioneSuGP == (codeGestione.TraduzioneSuGP.GetValueOrDefault() - 60).ToString() || y.TraduzioneSuGP == (codeGestione.TraduzioneSuGP.GetValueOrDefault() - 60).ToString() + "H"))))
                                {
                                    messaggioVideo = "Codice Gestione Contributi Esteri non presente nella sezione Dati Retributivi";
                                    return false;
                                }
                                break;
                            case "233":
                                // Se non esiste un elemento nei dati retributivi che abbia quota B e traduzione su GP X o XH, dove X corrisponde a 1, 2, 3 o 4
                                if (!datiCalcolo.LDatiRetributivi.Exists(x => x.QuotePrimeLiquidate.GetValueOrDefault() == 'A' &&
                                    listaCodeGestioneCalcoloRetributivo.Exists(y => y.Id == x.CodiceGestione &&
                                    (y.TraduzioneSuGP == (codeGestione.TraduzioneSuGP.GetValueOrDefault() - 70).ToString() || y.TraduzioneSuGP == (codeGestione.TraduzioneSuGP.GetValueOrDefault() - 70).ToString() + "H"))))
                                {
                                    messaggioVideo = "Codice Gestione Contributi Esteri non presente nella sezione Dati Retributivi";
                                    return false;
                                }
                                break;
                        }
                    }

                    int indexDupl = 0;
                    foreach (GestioneAggiornamentoPECO.DatiContributiEsteri contrEsteroDupl in datiCalcolo.LDatiContributiEsteri)
                    {
                        if (index < indexDupl)
                        {
                            if (contrEstero.CodiceGestione == contrEsteroDupl.CodiceGestione && contrEstero.Decorrenza == contrEsteroDupl.Decorrenza)
                            {
                                messaggioVideo = "Stesso Codice Gestione e Decorrenza ripetuto in piu' registrazioni";
                                return false;
                            }
                        }

                        indexDupl++;
                    }

                    if (contrEstero.CodiceGestione.GetValueOrDefault() > 0 && (!contrEstero.Decorrenza.HasValue || contrEstero.Settimane.GetValueOrDefault() == 0))
                    {
                        messaggioVideo = "Riga incompleta (manca Decorrenza / Settimane)";
                        return false;
                    }

                    //if (!Utility.VerificaData(contrEstero.Decorrenza, Utility.TipoAppartenenza.CI, out messaggioVideo))
                    //{
                    //    messaggioVideo = "Decorrenza Contributi Italiani ed Esteri: " + messaggioVideo;
                    //    return false;
                    //}

                    index++;
                }
            }

            #region PCIPL39 Categoria >= 7
            if (categoria >= 7)
            {
                //controllo sul numero settimane per la prevqalorizzazione delle settimane italiane misura in Dati Assicurativi (obbligatorio e prevalorizzato con la somma delle settimane)
                int settimaneItalianeMisura = GestioneContrib.GetNumeroSettimaneItalianeMisura(datiCalcolo.LDatiContributivi, datiCalcolo.LDatiRetributivi);
                if (!GestioneControlli.VerificaSettimaneDatiCalcolo(settimaneItalianeMisura, datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                    return false;
            }
            #endregion PCIPL39 Categoria >= 7

            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> LmaternitaAcnaDB = null;
            GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(datiPensione.Id, out LmaternitaAcnaDB);

            if (LmaternitaAcnaDB != null && (!datiCalcolo.CTRMaternitaAcna.HasValue || (datiCalcolo.CTRMaternitaAcna.HasValue && !datiCalcolo.CTRMaternitaAcna.Value)))
            {
                messaggioVideo = "Cancellare i dati della tab 'Maternità/Acna' prima di procedere con il salvataggio dei dati della tab 'Dati Calcolo'";
                return false;
            }

            //chiamata ai metodi di controllo dei dati Calcolo per le CI
            if (!ControlsCrossDatiCalcolo(datiCalcolo, datiPensione, datiIstruttoriaCommon, datiMaggiorazioniBeneficiCommon, LmaternitaAcna, proRata, singleTab, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsSettimane707(datiPensione, datiCalcolo.LDatiRetributivi, datiCalcolo.LDatiContributivi, listaCodeGestioneCalcoloRetributivo, listaCodeGestioneCalcoloContributivo,
                datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null, datiCalcolo.ContributiItalianiEdEsteriAl1295, out messaggioVideo))
                return false;

            return true;
        }

        private static bool ControlsCrossDatiCalcolo(DatiCalcolo datiCalcolo, GestionePensione.DatiPensione datiPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, List<MaternitaAcna> LdatiMaternitaAcna, GestioneContrib.ProRata proRata, bool singleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiCIGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiCIGenerici);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausa != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausa.IdAnagrafica, out datiAnagraficiDC);

            GestioneIntegrazioneArt11.IntegrazioneArt11 integrazioneArt11 = null;
            GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(datiPensione.Id, out integrazioneArt11);

            List<GestioneDecodifica.CodeGestione> listaCodiciGestione = null;
            GestioneDecodifica.GetCodiceGestione(out listaCodiciGestione);

            if (singleTab)
            {
                proRata = new ProRata();
                List<StatoEstero> elencoStatiEsteri = null;

                List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
                GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

                GestioneContrib.GetStatiEEfromDBByIdPensione(datiPensione.Id, listaPrestazioniEstere, out elencoStatiEsteri);

                proRata.ElencoStatiEsteri = elencoStatiEsteri;
            }

            int? nSettimane = null;
            if (datiCalcolo.LDatiContributivi != null && datiCalcolo.LDatiContributivi.Count > 0)
                nSettimane = datiCalcolo.LDatiContributivi[0].Nsettimane;

            int? settimaneRetributiveQuotaACodGestione1 = null;
            int? settimaneRetributiveQuotaACodGestione2 = null;
            int? settimaneRetributiveQuotaACodGestione3 = null;
            int? settimaneRetributiveQuotaACodGestione4 = null;
            decimal? rmsQuotaACodGestione1 = null;
            decimal? rmsQuotaACodGestione2 = null;
            decimal? rmsQuotaACodGestione3 = null;
            decimal? rmsQuotaACodGestione4 = null;
            int? settimaneRetributiveQuotaBCodGestione1 = null;
            int? settimaneRetributiveQuotaBCodGestione2 = null;
            int? settimaneRetributiveQuotaBCodGestione3 = null;
            int? settimaneRetributiveQuotaBCodGestione4 = null;
            decimal? rmsQuotaBCodGestione1 = null;
            decimal? rmsQuotaBCodGestione2 = null;
            decimal? rmsQuotaBCodGestione3 = null;
            decimal? rmsQuotaBCodGestione4 = null;
            int? settimane707QuotaBCodGestione1 = null;
            int? settimane707QuotaBCodGestione2 = null;
            int? settimane707QuotaBCodGestione3 = null;
            int? settimane707QuotaBCodGestione4 = null;
            int? settimaneContributiveCodGestione1 = null;
            int? settimaneContributiveCodGestione2 = null;
            int? settimaneContributiveCodGestione3 = null;
            int? settimaneContributiveCodGestione4 = null;
            decimal? montanteCodGestione1 = null;
            decimal? montanteCodGestione2 = null;
            decimal? montanteCodGestione3 = null;
            decimal? montanteCodGestione4 = null;
            decimal? importoContributivoTotaleCodGestione1 = null;
            decimal? importoContributivoTotaleCodGestione2 = null;
            decimal? importoContributivoTotaleCodGestione3 = null;
            decimal? importoContributivoTotaleCodGestione4 = null;
            int? settimaneContributiveDL214CodGestione1 = null;
            int? settimaneContributiveDL214CodGestione2 = null;
            int? settimaneContributiveDL214CodGestione3 = null;
            int? settimaneContributiveDL214CodGestione4 = null;
            decimal? montanteContributivoQuotaDCodGestione1 = null;
            decimal? montanteContributivoQuotaDCodGestione2 = null;
            decimal? montanteContributivoQuotaDCodGestione3 = null;
            decimal? montanteContributivoQuotaDCodGestione4 = null;
            decimal? importoContributivoQuotaDCodGestione1 = null;
            decimal? importoContributivoQuotaDCodGestione2 = null;
            decimal? importoContributivoQuotaDCodGestione3 = null;
            decimal? importoContributivoQuotaDCodGestione4 = null;

            int? sommaSettimaneContributi = 0;

            if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
            {
                foreach (GestioneAggiornamentoPECO.DatiRetributivi retr in datiCalcolo.LDatiRetributivi)
                {
                    if (retr.CodiceGestione == 1)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            settimaneRetributiveQuotaACodGestione1 = retr.NSettimaneQuotaA;
                            rmsQuotaACodGestione1 = retr.RMSQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione1.GetValueOrDefault();
                        }
                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            settimaneRetributiveQuotaBCodGestione1 = retr.NSettimaneQuotaB;
                            settimane707QuotaBCodGestione1 = retr.Nsettimane707;
                            rmsQuotaBCodGestione1 = retr.RMSQuotaB;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault();
                        }
                    }

                    if (retr.CodiceGestione == 2)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione2 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione2 = retr.NSettimaneQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione2.GetValueOrDefault();
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione2 = retr.RMSQuotaB;
                            settimane707QuotaBCodGestione2 = retr.Nsettimane707;
                            settimaneRetributiveQuotaBCodGestione2 = retr.NSettimaneQuotaB;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault();
                        }
                    }

                    if (retr.CodiceGestione == 3)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione3 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione3 = retr.NSettimaneQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione3.GetValueOrDefault();
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione3 = retr.RMSQuotaB;
                            settimane707QuotaBCodGestione3 = retr.Nsettimane707;
                            settimaneRetributiveQuotaBCodGestione3 = retr.NSettimaneQuotaB;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault();
                        }
                    }

                    if (retr.CodiceGestione == 4)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            rmsQuotaACodGestione4 = retr.RMSQuotaA;
                            settimaneRetributiveQuotaACodGestione4 = retr.NSettimaneQuotaA;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione4.GetValueOrDefault();
                        }

                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            rmsQuotaBCodGestione4 = retr.RMSQuotaB;
                            settimane707QuotaBCodGestione4 = retr.Nsettimane707;
                            settimaneRetributiveQuotaBCodGestione4 = retr.NSettimaneQuotaB;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();
                        }
                    }
                }
            }
            if (datiCalcolo.LDatiContributivi != null && datiCalcolo.LDatiContributivi.Count > 0)
            {
                foreach (GestioneAggiornamentoPECO.DatiContributivi contr in datiCalcolo.LDatiContributivi)
                {
                    if (contr.CodiceGestione == 1)
                    {
                        if (contr.Quota == 'C')
                        {
                            settimaneContributiveCodGestione1 = contr.Nsettimane;
                            importoContributivoTotaleCodGestione1 = contr.ImportoContributivoTotale;
                            montanteCodGestione1 = contr.MontanteContributivo;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione1.GetValueOrDefault();
                        }
                        if (contr.Quota == 'D')
                        {
                            settimaneContributiveDL214CodGestione1 = contr.SettimaneQuotaD;
                            importoContributivoQuotaDCodGestione1 = contr.ImportoContributivoQuotaD;
                            montanteContributivoQuotaDCodGestione1 = contr.MontanteContributivoQuotaD;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione1.GetValueOrDefault();
                        }
                    }

                    if (contr.CodiceGestione == 2)
                    {
                        if (contr.Quota == 'C')
                        {
                            settimaneContributiveCodGestione2 = contr.Nsettimane;
                            montanteCodGestione2 = contr.MontanteContributivo;
                            importoContributivoTotaleCodGestione2 = contr.ImportoContributivoTotale;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione2.GetValueOrDefault();
                        }
                        if (contr.Quota == 'D')
                        {
                            settimaneContributiveDL214CodGestione2 = contr.SettimaneQuotaD;
                            importoContributivoQuotaDCodGestione2 = contr.ImportoContributivoQuotaD;
                            montanteContributivoQuotaDCodGestione2 = contr.MontanteContributivoQuotaD;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione2.GetValueOrDefault();
                        }

                    }

                    if (contr.CodiceGestione == 3)
                    {
                        if (contr.Quota == 'C')
                        {
                            settimaneContributiveCodGestione3 = contr.Nsettimane;
                            montanteCodGestione3 = contr.MontanteContributivo;
                            importoContributivoTotaleCodGestione3 = contr.ImportoContributivoTotale;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione3.GetValueOrDefault();
                        }
                        if (contr.Quota == 'D')
                        {
                            settimaneContributiveDL214CodGestione3 = contr.SettimaneQuotaD;
                            importoContributivoQuotaDCodGestione3 = contr.ImportoContributivoQuotaD;
                            montanteContributivoQuotaDCodGestione3 = contr.MontanteContributivoQuotaD;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione3.GetValueOrDefault();
                        }

                    }

                    if (contr.CodiceGestione == 4)
                    {
                        if (contr.Quota == 'C')
                        {
                            settimaneContributiveCodGestione4 = contr.Nsettimane;
                            montanteCodGestione4 = contr.MontanteContributivo;
                            importoContributivoTotaleCodGestione4 = contr.ImportoContributivoTotale;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();
                        }
                        if (contr.Quota == 'D')
                        {
                            settimaneContributiveDL214CodGestione4 = contr.SettimaneQuotaD;
                            importoContributivoQuotaDCodGestione4 = contr.ImportoContributivoQuotaD;
                            montanteContributivoQuotaDCodGestione4 = contr.MontanteContributivoQuotaD;
                            sommaSettimaneContributi = sommaSettimaneContributi.GetValueOrDefault() + settimaneContributiveDL214CodGestione4.GetValueOrDefault();
                        }

                    }
                }
            }

            decimal rmsQuotaATotale = rmsQuotaACodGestione1.GetValueOrDefault() + rmsQuotaACodGestione2.GetValueOrDefault() + rmsQuotaACodGestione3.GetValueOrDefault() + rmsQuotaACodGestione4.GetValueOrDefault();
            decimal rmsQuotaBTotale = rmsQuotaBCodGestione1.GetValueOrDefault() + rmsQuotaBCodGestione2.GetValueOrDefault() + rmsQuotaBCodGestione3.GetValueOrDefault() + rmsQuotaBCodGestione4.GetValueOrDefault();

            int settimaneQuotaATotale = settimaneRetributiveQuotaACodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione2.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaACodGestione4.GetValueOrDefault();

            int settimaneQuotaBTotale = settimaneRetributiveQuotaBCodGestione1.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione2.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione3.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault();

            int settimaneQuotaCTotale = settimaneContributiveCodGestione1.GetValueOrDefault() + settimaneContributiveCodGestione2.GetValueOrDefault() + settimaneContributiveCodGestione3.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();

            int settimaneQuotaDTotale = settimaneContributiveDL214CodGestione1.GetValueOrDefault() + settimaneContributiveDL214CodGestione2.GetValueOrDefault() + settimaneContributiveDL214CodGestione3.GetValueOrDefault() + settimaneContributiveDL214CodGestione4.GetValueOrDefault();

            int? settimane707QuotaBTotali = settimane707QuotaBCodGestione1.GetValueOrDefault() + settimane707QuotaBCodGestione2.GetValueOrDefault() + settimane707QuotaBCodGestione3.GetValueOrDefault() + settimane707QuotaBCodGestione4.GetValueOrDefault();

            int? sommaSettimaneDirittoEstere = null;
            if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
                foreach (GestioneContrib.StatoEstero prestEE in proRata.ElencoStatiEsteri)
                {
                    sommaSettimaneDirittoEstere = sommaSettimaneDirittoEstere.GetValueOrDefault() + prestEE.PrestazioneEstera.ContributiEEDiritto.GetValueOrDefault();
                }

            byte? codiceConvenzione = null;
            if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
                codiceConvenzione = proRata.ElencoStatiEsteri[0].PrestazioneEstera.CodiceConvenzione;

            long? codiceGestioneContributiEsteri = null;
            if (datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
                codiceGestioneContributiEsteri = datiCalcolo.LDatiContributiEsteri[0].CodiceGestione;
            short? primoCodiceGestioneTraduzioneSuGP = 0;
            if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
            {
                GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == codiceGestioneContributiEsteri);
                if (codeGestione != null)
                    primoCodiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
            }

            //////////////////////////////// settiamo il numero di settimane in base alla categoria////////////////////
            string categoriaNumerica = datiPensione.GetCodCategoria();
            int categoria = 0;
            int.TryParse(categoriaNumerica, out categoria);
            int? settimane = GestioneControlli.NumeroSettimane(datiCIGenerici != null ? datiCIGenerici.SettimaneItalianeDiritto : null, datiIstruttoria != null ? datiIstruttoria.NSettimaneOBG : null,
                datiIstruttoria != null ? datiIstruttoria.NContributiUtiliLavoratoriAutonomi : null);
            if (categoria > 0 && categoria < 7)
            {
                settimane = settimane.GetValueOrDefault() + (datiIstruttoria != null ? datiIstruttoria.NContributiVolontari.GetValueOrDefault() : 0);
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            DateTime? decorrenza = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);

            int? settimaneRicalcoloMisura = 0;
            DateTime?[] primaDecorrenzaImportiEsteri = new DateTime?[6];
            int? settimaneEstereWithCodiceArt48 = 0;
            bool set_Rical = false;
            char? codiceArt48PrimoStato = null;
            if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
            {
                int index = 0;
                foreach (GestioneContrib.StatoEstero prestEE in proRata.ElencoStatiEsteri)
                {
                    List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> LImportiEsteri = null;
                    if (prestEE.ElencoImportiEsteri != null && prestEE.ElencoImportiEsteri.Count > 0)
                        LImportiEsteri = prestEE.ElencoImportiEsteri.FindAll(x => x.IDPrestazioneEE == prestEE.PrestazioneEstera.Id);
                    settimaneRicalcoloMisura = GestioneControlli.GetNumeroSettimaneRicalcoloMisura(settimaneRicalcoloMisura, prestEE.PrestazioneEstera.CodiceArt48, LImportiEsteri != null && LImportiEsteri.Count > 0 ? LImportiEsteri[0].DecorrenzaPrestazioneEE : null, prestEE.PrestazioneEstera.ContributiEERicalcolo, prestEE.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, ref set_Rical);
                    primaDecorrenzaImportiEsteri[index] = LImportiEsteri != null && LImportiEsteri.Count > 0 ? LImportiEsteri[0].DecorrenzaPrestazioneEE : null;
                    settimaneEstereWithCodiceArt48 = GestioneControlli.GetNumeroSettimaneEstereWithCodiceArt48(settimaneEstereWithCodiceArt48, prestEE.PrestazioneEstera.CodiceArt48, prestEE.PrestazioneEstera.ContributiEERicalcolo, prestEE.PrestazioneEstera.ContributiEEDecorrenzaOriginaria);
                    if (index == 0)
                        codiceArt48PrimoStato = prestEE.PrestazioneEstera.CodiceArt48;
                    index++;
                }
            }

            int?[] numeroSettimaneEstere = null;
            int? sommaSettimaneContributiItalianiEdEsteri = 0;
            bool isCodiceGestione0XPresenteContributiItalianiEdEsteri = false;
            bool isCodiceGestione6XPresenteContributiItalianiEdEsteri = false;
            int?[] sommaGEST_EST_61 = null;
            int sommaSettimaneDecUgualePrimaDec = 0;
            int sommaSettimaneCodGestione1_61CTRItalianiEdEsteri = 0;

            int? sommaSettimaneCodiceGestioneX4 = settimaneRetributiveQuotaACodGestione4.GetValueOrDefault() + settimaneRetributiveQuotaBCodGestione4.GetValueOrDefault() + settimaneContributiveCodGestione4.GetValueOrDefault();
            if (categoria == 92 || categoria == 93)
                sommaSettimaneCodiceGestioneX4 = sommaSettimaneCodiceGestioneX4.GetValueOrDefault() + (datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : 0);
            bool isDecorrenzaContributiItalianiEdEsteriDuplicata = false;
            if (datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
            {
                numeroSettimaneEstere = new int?[datiCalcolo.LDatiContributiEsteri.Count];
                sommaGEST_EST_61 = new int?[datiCalcolo.LDatiContributiEsteri.Count];
                int indexCalcoloContributivoEstero = 0;
                foreach (GestioneAggiornamentoPECO.DatiContributiEsteri datiContributiEsteri in datiCalcolo.LDatiContributiEsteri)
                {
                    if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
                    {
                        int index = 0;
                        foreach (GestioneContrib.StatoEstero prestEE in proRata.ElencoStatiEsteri)
                        {
                            numeroSettimaneEstere[indexCalcoloContributivoEstero] = GestioneControlli.GetNumeroSettimaneEstereWithDecorrenzaContributiItalianiEdEsteri(numeroSettimaneEstere[indexCalcoloContributivoEstero], datiContributiEsteri.Decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, prestEE.PrestazioneEstera.CodiceArt48, prestEE.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, primaDecorrenzaImportiEsteri[index], prestEE.PrestazioneEstera.ContributiEERicalcolo);
                            sommaGEST_EST_61[indexCalcoloContributivoEstero] = GestioneControlli.GEST_EST_61(sommaGEST_EST_61[indexCalcoloContributivoEstero], datiContributiEsteri.Decorrenza,
                                datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiCIGenerici != null ? datiCIGenerici.DecorrenzaBonus : null,
                                prestEE.PrestazioneEstera.CodiceArt48, primaDecorrenzaImportiEsteri, prestEE.PrestazioneEstera.ContributiEEDecorrenzaOriginaria, prestEE.PrestazioneEstera.ContributiEERicalcolo, index);

                            index++;
                        }
                    }

                    short codiceGestioneTraduzioneSuGP = 0;
                    if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                    {
                        GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContributiEsteri.CodiceGestione.Value);
                        if (codeGestione != null)
                            codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                    }
                    sommaSettimaneCodiceGestioneX4 = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteriCodGestioneX4(sommaSettimaneCodiceGestioneX4, codiceGestioneTraduzioneSuGP, datiContributiEsteri.Settimane);

                    sommaSettimaneContributiItalianiEdEsteri = sommaSettimaneContributiItalianiEdEsteri.GetValueOrDefault() + datiContributiEsteri.Settimane.GetValueOrDefault();

                    if (datiCalcolo.LDatiContributiEsteri.FindAll(x => x.Decorrenza == datiContributiEsteri.Decorrenza).Count > 1)
                        isDecorrenzaContributiItalianiEdEsteriDuplicata = true;

                    if (codiceGestioneTraduzioneSuGP / 10 == 0)
                        isCodiceGestione0XPresenteContributiItalianiEdEsteri = true;
                    if (codiceGestioneTraduzioneSuGP / 10 == 6)
                        isCodiceGestione6XPresenteContributiItalianiEdEsteri = true;

                    if (codiceGestioneTraduzioneSuGP == 1 || codiceGestioneTraduzioneSuGP == 61)
                        sommaSettimaneCodGestione1_61CTRItalianiEdEsteri += datiContributiEsteri.Settimane.GetValueOrDefault();

                    indexCalcoloContributivoEstero++;
                }
            }

            #endregion GetData

            #region Controlli OBG Misura 503 o Contributi 335

            //if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
            //    foreach (GestioneAggiornamentoPECO.DatiRetributivi datiRetributivi in datiCalcolo.LDatiRetributivi)
            //        if (!GestioneControlli.VerificaOBGMisura335Contributi335(datiAssicurativi.FineAssicurazione, datiGenerici.FlagContributiva, datiGenerici.NaturaPensione,
            //            datiRetributivi.NSettimaneQuotaB, nSettimane, datiAssicurativi.CodiceConvenzione, datiCalcolo.NContributiVolontari))
            //        {
            //            messaggioVideo = "OBG Misura 503/92 o Contributi 335/95 mancanti.";
            //            return false;
            //        }

            #endregion Controlli OBG Misura 503 o Contributi 335

            #region Controlli VV Misura Al 192

            if (!GestioneControlli.VerificaSettVVMisuraWithDecOrigWithDecOpzioneWithNContribVolWithNsett(datiCIGenerici != null ? datiCIGenerici.VVMisuraAl1292 : null, datiPensione.DecorrenzaOriginaria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null,
                datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiCIGenerici != null ? datiCIGenerici.ImportoIVS : null, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, nSettimane))
            {
                messaggioVideo = "Settimane VV per Misura mancanti o incompatibili con VV diritto.";
                return false;
            }

            #endregion Controlli VV Misura Al 192

            #region Controlli R.M.S.

            if (datiCalcolo.LDatiRetributivi != null && datiCalcolo.LDatiRetributivi.Count > 0)
                foreach (GestioneAggiornamentoPECO.DatiRetributivi datiRetributivi in datiCalcolo.LDatiRetributivi)
                {
                    if (!GestioneControlli.VerificaRMSWithDecOriginaria(datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiRetributivi.RMSQuotaA))
                    {
                        messaggioVideo = "R.M.S. errata per decorrenza ante 05/1968.";
                        return false;
                    }

                    if (!GestioneControlli.VerificaRMSDanteCausa(datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiRetributivi.RMSQuotaA,
                        datiPensione.InizioAssicurazione, datiPensione.SiglaCategoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null,
                        datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null, datiPensione.NaturaPensione,
                        datiPensione.Gruppo, datiPensione.Prodotto))
                    {
                        messaggioVideo = "R.M.S. mancante.";
                        return false;
                    }

                    #region Categorie minori o uguali a 6
                    if (categoria > 0 && categoria <= 6)
                    {
                        if (!GestioneControlli.VerificaRMSQuotaAWithDecorrenze(decorrenza, datiRetributivi.RMSQuotaA, out messaggioVideo))
                            return false;
                    }
                    #endregion Categorie minori o uguali a 6
                }

            #endregion Controlli R.M.S.

            #region PCIPL39 categoria >= 7
            if (categoria >= 7 || ((datiCalcolo.LDatiRetributivi == null || datiCalcolo.LDatiRetributivi.Count == 0 || !datiCalcolo.LDatiRetributivi.Exists(x => x.QuotePrimeLiquidate == 'A')) && !Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) && !Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione)))
            {
                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione1, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione2, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione3, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSQuotaAWithDecorrenzaAndInizioAssicurazione(datiPensione, categoria, rmsQuotaACodGestione4, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(categoria, datiPensione.NaturaPensione, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, rmsQuotaBCodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(categoria, datiPensione.NaturaPensione, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, rmsQuotaBCodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSQuotaBWithDecorrenzaAndFineAssicurazione(categoria, datiPensione.NaturaPensione, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, rmsQuotaBCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 1, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 2, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione3, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 3, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneItaliane1993(categoria, rmsQuotaBCodGestione4, settimaneRetributiveQuotaBCodGestione4, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, decorrenza, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null,
                    datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, 4, datiPensione.InizioAssicurazione, datiPensione.Gruppo, out messaggioVideo))
                    return false;

                //84: categoria minima per la comparazione all'interno del metodo. 87: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, 84, 87, out messaggioVideo))
                    return false;

                //87: categoria minima per la comparazione all'interno del metodo. 91: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, 87, 91, out messaggioVideo))
                    return false;

                //90: categoria minima per la comparazione all'interno del metodo. 94: categoria massima per la comparazione nel metodo
                if (!GestioneControlli.ControlsQuotaBWithcategoriaAndSettPrepensionamento(categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, 90, 94, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContributiviWithDecorrenza(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione1, settimaneContributiveCodGestione1, importoContributivoTotaleCodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContributiviWithDecorrenza(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione2, settimaneContributiveCodGestione2, importoContributivoTotaleCodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione1, importoContributivoTotaleCodGestione1, settimaneContributiveCodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione2, importoContributivoTotaleCodGestione2, settimaneContributiveCodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione3, importoContributivoTotaleCodGestione3, settimaneContributiveCodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaImportiWithContributi(montanteCodGestione4, importoContributivoTotaleCodGestione4, settimaneContributiveCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsQuotaAWithSettimaneQuotaDAndCategoria(categoria, settimaneRetributiveQuotaACodGestione2, settimaneRetributiveQuotaBCodGestione2, settimaneContributiveCodGestione2, settimaneContributiveDL214CodGestione2, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, 2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsQuotaAWithSettimaneQuotaDAndCategoria(categoria, settimaneRetributiveQuotaACodGestione3, settimaneRetributiveQuotaBCodGestione3, settimaneContributiveCodGestione3, settimaneContributiveDL214CodGestione3, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, 3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsQuotaAWithSettimaneQuotaDAndCategoria(categoria, settimaneRetributiveQuotaACodGestione4, settimaneRetributiveQuotaBCodGestione4, settimaneContributiveCodGestione4, settimaneContributiveDL214CodGestione4, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, 4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsContributiviWithDecorrenzaWithSettQuotaD(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione3, settimaneContributiveCodGestione3, importoContributivoTotaleCodGestione3, settimaneContributiveDL214CodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsContributiviWithDecorrenzaWithSettQuotaD(categoria, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione,
                    datiPensione.NaturaPensione, montanteCodGestione4, settimaneContributiveCodGestione4, importoContributivoTotaleCodGestione4, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaBWithRsmQuotaB(decorrenza, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione,
                    settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, categoria, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, settimaneRetributiveQuotaBCodGestione1, null, null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaFineAssicurazioneWithSettimaneQuotaD(datiPensione.InizioAssicurazione, settimaneContributiveDL214CodGestione1, settimaneContributiveDL214CodGestione2, settimaneContributiveDL214CodGestione3, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaBAndSettimaneWithFineAssicurazione(datiPensione, datiPensione.FineAssicurazione, rmsQuotaBCodGestione1, rmsQuotaBCodGestione2, rmsQuotaBCodGestione3, rmsQuotaBCodGestione4,
                    settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, settimaneContributiveDL214CodGestione1, settimaneContributiveDL214CodGestione2,
                    settimaneContributiveDL214CodGestione3, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaInizioAndFineAssicurazioneWithSettimaneTotaliQuotaB(decorrenza, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, settimaneQuotaBTotale, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneAndImportoAndMontanteQuotaD(settimaneContributiveDL214CodGestione2, importoContributivoQuotaDCodGestione2, montanteContributivoQuotaDCodGestione2, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneAndImportoAndMontanteQuotaD(settimaneContributiveDL214CodGestione3, importoContributivoQuotaDCodGestione3, montanteContributivoQuotaDCodGestione3, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneAndImportoAndMontanteQuotaD(settimaneContributiveDL214CodGestione4, importoContributivoQuotaDCodGestione4, montanteContributivoQuotaDCodGestione4, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCmsmAndRMS(decorrenza, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiCalcolo.MontanteInvalidita, categoria, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione2, 2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCmsmAndRMS(decorrenza, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiCalcolo.MontanteInvalidita, categoria, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione3, 3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneFittizieWithCmsmAndRMS(decorrenza, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiCalcolo.MontanteInvalidita, categoria, rmsQuotaBCodGestione4, settimaneRetributiveQuotaBCodGestione4, 4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCmsmWithSettimaneFittizie(datiCalcolo.MontanteInvalidita, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, decorrenza, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneRetributiveQuotaBCodGestione2, out messaggioVideo))
                {
                    messaggioVideo = "Contributi CD/CM dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneRetributiveQuotaBCodGestione3, out messaggioVideo))
                {
                    messaggioVideo = "Contributi ART dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithPeriodoAss(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneRetributiveQuotaBCodGestione4, out messaggioVideo))
                {
                    messaggioVideo = "Contributi COM dal 1993 incompatibili con periodo assicurativo";
                    return false;
                }

                if (!GestioneControlli.VerificaSettimaneQuotaBWithPeriodoAssicurativo(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneRetributiveQuotaBCodGestione1, datiPensione.DataInizioCalcolo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCmsmWithDecorrenza(decorrenza, datiCalcolo.MontanteInvalidita, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneContributive(categoria, 2, datiPensione.NaturaPensione, settimaneContributiveCodGestione2, settimaneContributiveDL214CodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneContributive(categoria, 3, datiPensione.NaturaPensione, settimaneContributiveCodGestione3, settimaneContributiveDL214CodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneContributive(categoria, 4, datiPensione.NaturaPensione, settimaneContributiveCodGestione4, settimaneContributiveDL214CodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(1, datiPensione.NaturaPensione, rmsQuotaACodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(2, datiPensione.NaturaPensione, rmsQuotaACodGestione2, rmsQuotaBCodGestione2, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(3, datiPensione.NaturaPensione, rmsQuotaACodGestione3, rmsQuotaBCodGestione3, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRmsQuotaAandQuotaB(4, datiPensione.NaturaPensione, rmsQuotaACodGestione4, rmsQuotaBCodGestione4, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(1, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione1, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(2, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione2, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(3, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione3, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaCWithCapienzaSett(4, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, settimaneContributiveCodGestione4, datiPensione.NaturaPensione, datiPensione, out messaggioVideo))
                    return false;

                if (datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
                {
                    int index = 0;
                    foreach (GestioneAggiornamentoPECO.DatiContributiEsteri datiContributiEsteri in datiCalcolo.LDatiContributiEsteri)
                    {
                        int? settimaneToCompare = 0;
                        short? codiceGestioneTraduzioneSuGP = 0;
                        if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                        {
                            GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContributiEsteri.CodiceGestione.Value);
                            if (codeGestione != null)
                                codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                        }

                        if (!GestioneControlli.VerificaContributiItalianiEdEsteri(codiceGestioneTraduzioneSuGP, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, settimaneRicalcoloMisura, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaRMSWithContributiItalianiEdEsteri(codiceGestioneTraduzioneSuGP, montanteCodGestione1, montanteContributivoQuotaDCodGestione1, montanteCodGestione2, montanteContributivoQuotaDCodGestione2, montanteCodGestione3, montanteContributivoQuotaDCodGestione3, montanteCodGestione4, montanteContributivoQuotaDCodGestione4, rmsQuotaBCodGestione1, rmsQuotaBCodGestione2, rmsQuotaBCodGestione3, rmsQuotaBCodGestione4, rmsQuotaACodGestione1, rmsQuotaACodGestione2, rmsQuotaACodGestione3, rmsQuotaACodGestione4, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaDecorrenzaContributiItalianiEdEsteri(datiContributiEsteri.Decorrenza, decorrenza, datiCIGenerici != null ? datiCIGenerici.DecorrenzaBonus : null, codiceGestioneTraduzioneSuGP, primaDecorrenzaImportiEsteri, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSettimaneEstereWithContributiItalianiEdEsteri(datiContributiEsteri.Decorrenza, numeroSettimaneEstere[index], sommaSettimaneContributiItalianiEdEsteri, decorrenza, datiPensione.InizioAssicurazione, sommaSettimaneContributi, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaCapienzaSettimaneContributiItalianiEdEsteri(codiceGestioneTraduzioneSuGP, decorrenza, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, datiContributiEsteri.Decorrenza, settimaneRetributiveQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione4, datiContributiEsteri.Settimane, categoria, datiPensione, out messaggioVideo))
                            return false;

                        if (!GestioneControlli.VerificaSettimaneWithContributiItalianiEdEsteri(sommaSettimaneContributiItalianiEdEsteri, settimaneEstereWithCodiceArt48, sommaSettimaneContributi, out messaggioVideo))
                            return false;

                        settimaneToCompare = settimaneRetributiveQuotaBCodGestione2;
                        foreach (GestioneAggiornamentoPECO.DatiContributiEsteri appDatiContributiEsteri in datiCalcolo.LDatiContributiEsteri)
                            settimaneToCompare = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteri9395(settimaneToCompare, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, appDatiContributiEsteri.Settimane, 62);

                        if (!GestioneControlli.VerificaSettimaneNelPeriodo9395(datiPensione.FineAssicurazione, datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, decorrenza, settimaneToCompare, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, "CD-CM", out messaggioVideo))
                            return false;

                        settimaneToCompare = settimaneRetributiveQuotaBCodGestione3;
                        foreach (GestioneAggiornamentoPECO.DatiContributiEsteri appDatiContributiEsteri in datiCalcolo.LDatiContributiEsteri)
                            settimaneToCompare = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteri9395(settimaneToCompare, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, appDatiContributiEsteri.Settimane, 63);

                        if (!GestioneControlli.VerificaSettimaneNelPeriodo9395(datiPensione.FineAssicurazione, datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, decorrenza, settimaneToCompare, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, "ART", out messaggioVideo))
                            return false;

                        settimaneToCompare = settimaneRetributiveQuotaBCodGestione4;
                        foreach (GestioneAggiornamentoPECO.DatiContributiEsteri appDatiContributiEsteri in datiCalcolo.LDatiContributiEsteri)
                            settimaneToCompare = GestioneControlli.GetNumeroSettimaneContributiItalianiEdEsteri9395(settimaneToCompare, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, appDatiContributiEsteri.Settimane, 64);

                        if (!GestioneControlli.VerificaSettimaneNelPeriodo9395(datiPensione.FineAssicurazione, datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneContributiveCodGestione2, settimaneContributiveCodGestione3, settimaneContributiveCodGestione4, decorrenza, settimaneToCompare, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, "COM", out messaggioVideo))
                            return false;

                        index++;
                    }

                    if (categoria >= 7)
                    {
                        if (!GestioneControlli.VerificaContributiItalianiEdEsteriWithSettimaneProRata(sommaSettimaneContributiItalianiEdEsteri, settimaneRicalcoloMisura, set_Rical, isDecorrenzaContributiItalianiEdEsteriDuplicata,
                            datiPensione.DecorrenzaOriginaria, datiCalcolo.LDatiContributiEsteri[0].Decorrenza, sommaSettimaneCodiceGestioneX4.GetValueOrDefault() == 2080, out messaggioVideo))
                            return false;
                    }
                }
                else
                {
                    if (!GestioneControlli.VerificaObbligatorietaContributiItalianiEdEsteri(null, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, settimaneRicalcoloMisura, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.VerificaContribItalianiEsteri1295WithLegge335(datiPensione, datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneContributiveCodGestione1, montanteCodGestione1, importoContributivoTotaleCodGestione1, settimaneContributiveCodGestione2, montanteCodGestione2, importoContributivoTotaleCodGestione2, settimaneContributiveCodGestione3, montanteCodGestione3, importoContributivoTotaleCodGestione3, settimaneContributiveCodGestione4, montanteCodGestione4, importoContributivoTotaleCodGestione4, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione2, rmsQuotaBCodGestione2, settimaneRetributiveQuotaBCodGestione3, rmsQuotaBCodGestione3, settimaneRetributiveQuotaBCodGestione4, rmsQuotaBCodGestione4, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, isCodiceGestione0XPresenteContributiItalianiEdEsteri, isCodiceGestione6XPresenteContributiItalianiEdEsteri, primoCodiceGestioneTraduzioneSuGP, codiceArt48PrimoStato, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMSQuotaBWithDecorrenzaAndUltimoContributo(rmsQuotaBCodGestione1, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, datiPensione.DataInizioCalcolo, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimaneQuotaBWithDecorrenzaAndUltimoContributo(settimaneRetributiveQuotaBCodGestione1, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, categoria, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, datiPensione.DataInizioCalcolo, datiPensione, out messaggioVideo))
                    return false;
            }
            #endregion PCIPL39 categoria >= 7

            if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295WithQuotaC(datiPensione, datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneQuotaCTotale, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione1, importoContributivoTotaleCodGestione1, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione2, importoContributivoTotaleCodGestione2, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione3, importoContributivoTotaleCodGestione3, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaImportoContributivoTotWithMontante(datiPensione.DecorrenzaOriginaria, montanteCodGestione4, importoContributivoTotaleCodGestione4, out messaggioVideo))
                return false;

            if (!GestioneControlli.VerificaCompletezzaDatiContributiviQuotaD(settimaneContributiveDL214CodGestione1, importoContributivoQuotaDCodGestione1, montanteContributivoQuotaDCodGestione1,
                datiPensione.DecorrenzaOriginaria, out messaggioVideo))
                return false;

            #region Categorie minori o uguali a 6
            if ((categoria > 0 && categoria <= 6) || categoria == 88 || categoria == 91 || categoria == 85)
            {
                if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295(datiPensione, datiPensione.DecorrenzaOriginaria, datiAnagrafici.DataNascita, datiAnagraficiDC != null ? datiAnagraficiDC.DataMorte : null, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiPensione.DataPerfezionamentoRequisiti, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null, datiCalcolo != null ? datiCalcolo.ContributiItalianiEdEsteriAl1295 : null, settimane707QuotaBTotali, rmsQuotaBTotale, rmsQuotaATotale, settimaneQuotaATotale, settimaneQuotaBTotale, settimaneQuotaCTotale, settimaneQuotaDTotale, datiDanteCausa != null ? datiDanteCausa.Certificato : null, categoria, datiPensione.NaturaPensione, datiPensione.Gruppo, out messaggioVideo))
                    return false;
            }

            if (categoria > 0 && categoria <= 6)
            {
                if (!GestioneControlli.VerificaRMS8888WithRMSQuotaA(datiCIGenerici != null ? datiCIGenerici.RMS8888 : null, rmsQuotaACodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMS9090WithRMSQuotaA(datiCIGenerici != null ? datiCIGenerici.RMS9090 : null, rmsQuotaACodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimanePost1993WithNSettimaneIncrementoPercentuale(datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento1Percento : null, datiMaggiorazioniBenefici != null ? datiMaggiorazioniBenefici.NSettimaneIncremento05Percento : null, settimaneRetributiveQuotaBCodGestione1, settimaneContributiveCodGestione1, settimaneContributiveDL214CodGestione1, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaNSettimaneQuotaAWithInizioAssicurazione(settimaneRetributiveQuotaACodGestione1, datiPensione.InizioAssicurazione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMSQuotaAWithOpzioneAndDanteCausa(categoria, datiIstruttoria != null ? datiIstruttoria.DecorrenzaOpzione : null, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, rmsQuotaACodGestione1, datiPensione.InizioAssicurazione, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, datiPensione.NaturaPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaObbligatorietaRMSQuotaAWithDecorrenze(categoria, rmsQuotaACodGestione1, decorrenza, datiDanteCausa != null ? datiDanteCausa.Certificato : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRMSQuotaBWithSettimane(datiCalcolo != null ? datiCalcolo.ContributiItalianiEdEsteriAl1295 : null, settimaneQuotaCTotale, settimaneQuotaDTotale, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiCIGenerici != null ? datiCIGenerici.CMSM : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsRMSWithDecorrenzaAndAssicurazioneAndCodNatura(decorrenza, datiPensione.FineAssicurazione, datiPensione.NaturaPensione, rmsQuotaBCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, rmsQuotaACodGestione1, settimaneRetributiveQuotaACodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraAl1292 : null, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiPensione.Gruppo, datiAnagraficiDC != null ? datiAnagraficiDC.DataNascita : null, datiAnagraficiDC != null ? datiAnagraficiDC.Sesso : null, datiAnagrafici.DataNascita, datiAnagrafici.Sesso, datiPensione,out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContributiWithFineAssicurazione(datiPensione.FineAssicurazione, datiPensione.NaturaPensione, settimaneRetributiveQuotaBCodGestione1, settimaneContributiveCodGestione1, settimaneContributiveDL214CodGestione1, codiceConvenzione, datiIstruttoria != null ? datiIstruttoria.NContributiVolontari : null, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, rmsQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiPensione, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaContributiWithDecorrenza(decorrenza, datiPensione.DecorrenzaOriginaria, settimaneContributiveCodGestione1, importoContributivoTotaleCodGestione1, montanteCodGestione1, settimaneContributiveDL214CodGestione1, importoContributivoQuotaDCodGestione1, montanteContributivoQuotaDCodGestione1, datiPensione.NaturaPensione, datiPensione.FineAssicurazione, rmsQuotaBCodGestione1, datiCalcolo.MontanteInvalidita, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, datiDanteCausa != null ? datiDanteCausa.Certificato : null, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaRegistrazioneADecorrenza(datiPensione.InizioAssicurazione, primoCodiceGestioneTraduzioneSuGP, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaCMSM(decorrenza, datiCalcolo.MontanteInvalidita, datiCIGenerici != null ? datiCIGenerici.NSettFittiziePrepensionamento : null, out messaggioVideo))
                    return false;

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                // ENG - Bypass MONTANTE_335_INCOMPATIBILE con data ultimo contributo
                if (!Utility.IsDomandaTipoContributivo(datiPensione, null, true) && !Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) &&
                    !((!Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRiaperturaDomanda(datiPensione.Id) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))) ||
                       ((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && ((ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))))
                {
                    if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.LiquidazionePensione_Assicurativi_CI.MONTANTE_335_INCOMPATIBILE))
                    {
                        if (!GestioneControlli.VerificaMontante335(decorrenza, datiPensione.FineAssicurazione, montanteCodGestione1, datiPensione.NaturaPensione, datiCalcolo.MontanteInvalidita, out messaggioVideo))
                            return false;
                    }
                }


                //if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295(datiPensione.InizioAssicurazione, datiCalcolo.ContributiItalianiEdEsteriAl1295, datiPensione.NaturaPensione, settimaneContributiveCodGestione1, importoContributivoTotaleCodGestione1, montanteCodGestione1, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, rmsQuotaBCodGestione1, datiPensione.Gruppo, isCodiceGestione0XPresenteContributiItalianiEdEsteri, primoCodiceGestioneTraduzioneSuGP, isCodiceGestione6XPresenteContributiItalianiEdEsteri, codiceArt48PrimoStato, out messaggioVideo))
                //    return false;

                int sommmaSettimaneContrEE = 0;
                int sommmaSettimaneContrEE1993_1995 = 0;
                if (datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
                {
                    int index = 0;
                    foreach (GestioneAggiornamentoPECO.DatiContributiEsteri datiContribEsteri in datiCalcolo.LDatiContributiEsteri)
                    {
                        short? codiceGestioneTraduzioneSuGP = 0;
                        if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                        {
                            GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContribEsteri.CodiceGestione.Value);
                            if (codeGestione != null)
                                codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                        }

                        sommmaSettimaneContrEE = GestioneControlli.CalcolaSettimaneContrEE(sommmaSettimaneContrEE, datiPensione.InizioAssicurazione, codiceGestioneTraduzioneSuGP, datiContribEsteri.Decorrenza, datiContribEsteri.Settimane, datiPensione.DecorrenzaOriginaria);

                        sommmaSettimaneContrEE1993_1995 = GestioneControlli.CalcolaSettimane(datiPensione.InizioAssicurazione, settimaneContributiveCodGestione1, settimaneRetributiveQuotaBCodGestione1,
                            codiceGestioneTraduzioneSuGP, datiContribEsteri.Settimane, sommmaSettimaneContrEE1993_1995);

                        if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1993, 01, 01)) && datiPensione.InizioAssicurazione.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.InizioAssicurazione.Value, new DateTime(1993, 01, 01)))
                        {
                            if (index == 0)
                            {
                                sommaSettimaneDecUgualePrimaDec = datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0 ? datiCalcolo.LDatiContributiEsteri[0].Settimane.GetValueOrDefault() : 0;
                            }
                            else
                            {
                                if (datiCalcolo.LDatiContributiEsteri[0].Decorrenza.Equals(datiContribEsteri.Decorrenza))
                                {
                                    sommaSettimaneDecUgualePrimaDec += datiContribEsteri.Settimane.GetValueOrDefault();
                                }
                            }
                        }
                    }
                }

                if (!GestioneControlli.VerificaSettimane1993_1995(datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.DecorrenzaOriginaria, sommmaSettimaneContrEE1993_1995, out messaggioVideo))
                    return false;

                if (!GestioneControlli.VerificaSettimane23390_50392_33595(sommaSettimaneCodGestione1_61CTRItalianiEdEsteri, settimaneRicalcoloMisura, out messaggioVideo))
                    return false;

            }
            #endregion Categorie minori o uguali a 6

            if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
            {
                int index = 0;
                foreach (StatoEstero statoEE in proRata.ElencoStatiEsteri)
                {
                    if (datiCalcolo != null && datiCalcolo.LDatiContributiEsteri != null && datiCalcolo.LDatiContributiEsteri.Count > 0)
                    {
                        int indexContributi = 0;
                        foreach (GestioneAggiornamentoPECO.DatiContributiEsteri datiContribEsteri in datiCalcolo.LDatiContributiEsteri)
                        {
                            short? codiceGestioneTraduzioneSuGP = 0;
                            if (listaCodiciGestione != null && listaCodiciGestione.Count > 0)
                            {
                                GestioneDecodifica.CodeGestione codeGestione = listaCodiciGestione.Find(x => x.Id == datiContribEsteri.CodiceGestione.Value);
                                if (codeGestione != null)
                                    codiceGestioneTraduzioneSuGP = codeGestione.TraduzioneSuGP.HasValue ? codeGestione.TraduzioneSuGP.Value : (short)0;
                            }

                            if (index > 0)
                            {
                                if (!GestioneControlli.ControlsSettimaneEstereWithDecorrenzaRicalcolo(datiPensione.InizioAssicurazione, datiContribEsteri.Decorrenza, statoEE.PrestazioneEstera != null ? statoEE.PrestazioneEstera.ContributiEERicalcolo : null,
                                    statoEE.ElencoImportiEsteri != null && statoEE.ElencoImportiEsteri.Count > 0 ? statoEE.ElencoImportiEsteri[0].DecorrenzaPrestazioneEE : null, out messaggioVideo))
                                    return false;
                            }

                            if (categoria > 0 && categoria <= 6)
                            {
                                if (!GestioneControlli.ControlsContributiEsteri(indexContributi, codiceGestioneTraduzioneSuGP, datiContribEsteri.Decorrenza, datiContribEsteri.Settimane, datiPensione.InizioAssicurazione,
                                    rmsQuotaBCodGestione1, importoContributivoTotaleCodGestione1, importoContributivoQuotaDCodGestione1, datiPensione.FineAssicurazione, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiPensione.DecorrenzaOriginaria,
                                    datiPensione.NaturaPensione, montanteCodGestione1, montanteContributivoQuotaDCodGestione1, datiCIGenerici != null ? datiCIGenerici.DecorrenzaBonus : null, primaDecorrenzaImportiEsteri,
                                    datiCalcolo != null ? datiCalcolo.ContributiItalianiEdEsteriAl1295 : null, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraAl1292 : null,
                                    sommaGEST_EST_61[indexContributi], sommaSettimaneContributiItalianiEdEsteri, sommaSettimaneDecUgualePrimaDec, datiPensione, out messaggioVideo))
                                    return false;
                            }
                            indexContributi++;
                        }
                    }
                    index++;
                }
            }

            if (!GestioneControlli.VerificaOpzioneContributiva(datiPensione, datiNuoveLiquidate != null ? datiNuoveLiquidate.FlagContributiva : null,
                datiCalcolo.ContributiItalianiEdEsteriAl1295, settimaneQuotaATotale, settimaneQuotaBTotale, out messaggioVideo))
                return false;

            if (LdatiMaternitaAcna != null && LdatiMaternitaAcna.Count > 0)
            {
                foreach (GestioneContrib.MaternitaAcna maternitaAcna in LdatiMaternitaAcna)
                {
                    if (maternitaAcna.Tipo == 'M') // Maternità
                    {
                        if (maternitaAcna.SettimaneAl1292.GetValueOrDefault() > 0 || maternitaAcna.SettimaneDL50392.GetValueOrDefault() > 0 || maternitaAcna.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.VerificaMaternitaWithDatiCalcolo(maternitaAcna.SettimaneAl1292, settimaneRetributiveQuotaACodGestione1, rmsQuotaACodGestione1, maternitaAcna.SettimaneDL50392, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                                return false;
                        }
                    }

                    if (maternitaAcna.Tipo == 'A') // Acna
                    {
                        if (maternitaAcna.SettimaneAl1292.GetValueOrDefault() > 0 || maternitaAcna.SettimaneDL50392.GetValueOrDefault() > 0 || maternitaAcna.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.VerificaAcnaWithDatiCalcolo(maternitaAcna.SettimaneAl1292, maternitaAcna.SettimaneDL50392, settimaneRetributiveQuotaACodGestione1, rmsQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                                return false;
                        }

                    }
                }
            }

            if (!GestioneControlli.VerificaCapienzaSettimaneDL50392WithAssicurazione(datiPensione, datiPensione.InizioAssicurazione, datiPensione.FineAssicurazione, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione, datiPensione.DecorrenzaOriginaria, decorrenza, settimaneRetributiveQuotaBCodGestione1, datiCIGenerici != null ? datiCIGenerici.VVMisuraDL50392 : null, datiPensione.AttivitaEconomica, out messaggioVideo))
                return false;

            if (!GestioneControlli.ControlsContributiItalianiEsteriAl1295PerAPEPrecoci(datiPensione, datiCalcolo != null ? datiCalcolo.ContributiItalianiEdEsteriAl1295 : null, out messaggioVideo))
                return false;

            return true;
        }

        public static bool IsFineAssicurazionePost2012(DateTime? fineAssicurazione)
        {
            DateTime DuemilaDodici = new DateTime(2012, 01, 01);
            if (fineAssicurazione.HasValue)
                return Liquidazione.BLCommon.Utility.DataSuccessivaA(fineAssicurazione.Value, DuemilaDodici);

            return false;
        }

        public static bool IsInizioAssicurazionePost1995(DateTime? inizioAssicurazione)
        {
            DateTime dataCompare = new DateTime(1995, 01, 01);
            if (inizioAssicurazione.HasValue)
                return Liquidazione.BLCommon.Utility.DataSuccessivaA(inizioAssicurazione.Value, dataCompare);

            return false;
        }

        private static int GetNumeroSettimaneItalianeMisura(List<GestioneAggiornamentoPECO.DatiContributivi> listaDatiCalcoloContributivo, List<GestioneAggiornamentoPECO.DatiRetributivi> listaDatiCalcoloRetributivo)
        {
            int settimaneItalianeMisura = 0;

            if (listaDatiCalcoloContributivo != null && listaDatiCalcoloContributivo.Count > 0)
            {
                foreach (GestioneAggiornamentoPECO.DatiContributivi calcoloContrib in listaDatiCalcoloContributivo)
                {
                    if (calcoloContrib.Nsettimane.HasValue)
                        settimaneItalianeMisura = settimaneItalianeMisura + (int)calcoloContrib.Nsettimane;
                    if (calcoloContrib.SettimaneQuotaD.HasValue)
                        settimaneItalianeMisura = settimaneItalianeMisura + (int)calcoloContrib.SettimaneQuotaD;
                }
            }

            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneAggiornamentoPECO.DatiRetributivi calcoloRetrib in listaDatiCalcoloRetributivo)
                {
                    if (calcoloRetrib.QuotePrimeLiquidate.ToString() == "A")
                        settimaneItalianeMisura = settimaneItalianeMisura + calcoloRetrib.NSettimaneQuotaA.GetValueOrDefault();

                    if (calcoloRetrib.QuotePrimeLiquidate.ToString() == "B")
                        settimaneItalianeMisura = settimaneItalianeMisura + calcoloRetrib.NSettimaneQuotaB.GetValueOrDefault();
                }
            }

            return settimaneItalianeMisura;
        }

        internal static List<GestioneCalcolo.DatiCalcoloRetributivo> MappingDatiRetributiviFromViewToBL(List<GestioneAggiornamentoPECO.DatiRetributivi> ldatiRetributivi)
        {
            List<GestioneCalcolo.DatiCalcoloRetributivo> datiRetributivi = null;
            if (ldatiRetributivi != null && ldatiRetributivi.Count > 0)
            {
                datiRetributivi = new List<GestioneCalcolo.DatiCalcoloRetributivo>();
                foreach (GestioneAggiornamentoPECO.DatiRetributivi calRetr in ldatiRetributivi)
                {
                    GestioneCalcolo.DatiCalcoloRetributivo datiRetr = new GestioneCalcolo.DatiCalcoloRetributivo();

                    datiRetr.DecorrenzaOriginariaPensione = calRetr.DecorrenzaOriginariaPensione;
                    datiRetr.CodiceGestione = calRetr.CodiceGestione;
                    datiRetr.QuotePrimeLiquidate = calRetr.QuotePrimeLiquidate;
                    if (datiRetr.QuotePrimeLiquidate.HasValue && datiRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "A")
                    {
                        datiRetr.RMSQuotaA = calRetr.RMSQuotaA;
                        datiRetr.NSettimaneQuotaA = calRetr.NSettimaneQuotaA;
                    }
                    else if (datiRetr.QuotePrimeLiquidate.HasValue && datiRetr.QuotePrimeLiquidate.Value.ToString().ToUpperInvariant() == "B")
                    {
                        datiRetr.RMSQuotaB = calRetr.RMSQuotaB;
                        datiRetr.NSettimaneQuotaB = calRetr.NSettimaneQuotaB;
                    }
                    datiRetr.NSettimane707 = calRetr.Nsettimane707;

                    datiRetributivi.Add(datiRetr);
                }
            }
            return datiRetributivi;
        }

        internal static List<GestioneCalcolo.DatiCalcoloContributivo> MappingDatiContributiviFromViewToBL(List<GestioneAggiornamentoPECO.DatiContributivi> ldatiContributivi)
        {
            List<GestioneCalcolo.DatiCalcoloContributivo> datiContributivi = null;
            if (ldatiContributivi != null && ldatiContributivi.Count > 0)
            {
                datiContributivi = new List<GestioneCalcolo.DatiCalcoloContributivo>();
                foreach (GestioneAggiornamentoPECO.DatiContributivi calContr in ldatiContributivi)
                {
                    GestioneCalcolo.DatiCalcoloContributivo datiContr = new GestioneCalcolo.DatiCalcoloContributivo();

                    datiContr.CodiceGestione = calContr.CodiceGestione;

                    if (calContr.Quota.HasValue && calContr.Quota.Value.ToString().ToUpperInvariant() == "C")
                    {
                        datiContr.ImportoContributivoTotale = calContr.ImportoContributivoTotale;
                        datiContr.Montante = calContr.MontanteContributivo;
                        datiContr.NSettimane = calContr.Nsettimane;
                    }
                    else if (calContr.Quota.HasValue && calContr.Quota.Value.ToString().ToUpperInvariant() == "D")
                    {
                        datiContr.ImportoContribTotaleQuotaDL214 = calContr.ImportoContributivoQuotaD;
                        datiContr.MontanteQuotaDL214 = calContr.MontanteContributivoQuotaD;
                        datiContr.NSettimaneQuotaDL214 = calContr.SettimaneQuotaD;
                    }

                    datiContributivi.Add(datiContr);
                }
            }
            return datiContributivi;
        }

        #endregion DatiCalcolo

        #region ImportiEsteri

        public static void GetDatiImportiEsteriByIdPensione(long idPensione, out List<PensioniCiImportiValuta> LdatiImportiValutaEsteri, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            LdatiImportiValutaEsteri = null;
            List<GestioneDatiContributiviCi.PensioniCiImportiValuta> LdatiImportiValutaEsteriCommon = null;
            GestioneDatiContributiviCi.GetImportiEsteriValutaByIdPensione(idPensione, out LdatiImportiValutaEsteriCommon);
            if (LdatiImportiValutaEsteriCommon != null)
            {
                LdatiImportiValutaEsteri = new List<PensioniCiImportiValuta>();
                foreach (GestioneDatiContributiviCi.PensioniCiImportiValuta ieapp in LdatiImportiValutaEsteriCommon)
                {
                    PensioniCiImportiValuta ie = new PensioniCiImportiValuta();
                    Utility.ValorizzaOggetti(ieapp, ie);
                    LdatiImportiValutaEsteri.Add(ie);
                }

            }
        }

        public static void StoreDatiImportiEsteri(GestionePensione.DatiPensione datiPensione, List<PensioniCiImportiValuta> LdatiImportiEsteri, GestioneContrib.ProRata proRata, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (ControlsDatiImportiEsteriByIdPensione(datiPensione.Id, LdatiImportiEsteri, proRata, datiPensione.CausaCarico, IsSingleTabSaved, out messaggioVideo))
            {
                GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    GestioneDatiContributiviCi.EliminaAllImportiEsteriValutaByIdPensione(datiPensione.Id);
                    foreach (PensioniCiImportiValuta ImportoEsteroValuta in LdatiImportiEsteri)
                    {
                        GestioneDatiContributiviCi.PensioniCiImportiValuta pensioniCiImportiValuta = new GestioneDatiContributiviCi.PensioniCiImportiValuta();
                        Utility.ValorizzaOggetti(ImportoEsteroValuta, pensioniCiImportiValuta);
                        pensioniCiImportiValuta.IdPensione = datiPensione.Id;
                        GestioneDatiContributiviCi.SalvaImportiEsteriValuta(pensioniCiImportiValuta);
                    }
                    quadroDatiContributivi.TabContrEsteri = 2;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                    transactionScope.Complete();
                }
            }
        }

        public static void CancelDatiImportiEsteri(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiContributiviCi.EliminaAllImportiEsteriValutaByIdPensione(datiPensione.Id);
                quadroDatiContributivi.TabContrEsteri = 1;
                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        private static bool ControlsDatiImportiEsteriByIdPensione(long idPensione, List<PensioniCiImportiValuta> LdatiImportiEsteri, GestioneContrib.ProRata proRata, byte? causaCarico, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region GetData

            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
            GestioneDatiContributiviCi.GetImportiEsteriByIdPensione(idPensione, out listaImportiEsteri);

            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            GestioneAnagrafica.GetResidenzeEstereByIdPensione(idPensione, out listaResidenzeEstere);

            if (IsSingleTabSaved)
            {
                List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
                GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(idPensione, out listaPrestazioniEstere);

                List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
                GestioneContrib.GetStatiEEfromDBByIdPensione(idPensione, listaPrestazioniEstere, out listaStatiEsteri);

                proRata = new ProRata();
                proRata.ElencoStatiEsteri = listaStatiEsteri;
                proRata.IsDataFromDB = true;
            }
            else
            {
                if (proRata == null || proRata.ElencoStatiEsteri == null || proRata.ElencoStatiEsteri.Count == 0)
                    proRata = null;
            }
            #endregion GetData

            if (LdatiImportiEsteri != null && LdatiImportiEsteri.Count > 0)
            {
                if (!GestioneControlli.VerificaImportiEsteriWithCausaCarico(causaCarico, LdatiImportiEsteri.First().ImportoPrestazioneEE, LdatiImportiEsteri.First().DecorrenzaPrestazioneEE))
                {
                    messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                    return false;
                }

                if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
                {
                    bool flag = false;
                    foreach (StatoEstero stato in proRata.ElencoStatiEsteri)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                        if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                            listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.PrestazioneEstera.Id);

                        flag = GestioneControlli.VerificaImportiEsteriWithPrestazioniEE((listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0) ? listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE : null, LdatiImportiEsteri.First().ImportoPrestazioneEE, LdatiImportiEsteri.First().DecorrenzaPrestazioneEE);

                        if (flag)
                            break;
                    }
                    if (!flag)
                    {
                        messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                        return false;
                    }

                    flag = false;
                    foreach (StatoEstero stato in proRata.ElencoStatiEsteri)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                        if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                            listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.PrestazioneEstera.Id);

                        flag = GestioneControlli.VerificaImportiEsteriWithDecPrecLiquidata(causaCarico, stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE, LdatiImportiEsteri.First().ImportoPrestazioneEE, LdatiImportiEsteri.First().DecorrenzaPrestazioneEE);

                        if (flag)
                            break;
                    }
                    if (!flag)
                    {
                        messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                        return false;
                    }

                    if (!GestioneControlli.VerificaImportiEsteriWithConvenzione(causaCarico, proRata.ElencoStatiEsteri.First().PrestazioneEstera.CodiceConvenzione, listaResidenzeEstere, LdatiImportiEsteri.First().ImportoPrestazioneEE, LdatiImportiEsteri.First().DecorrenzaPrestazioneEE))
                    {
                        messaggioVideo = "Gli Importi Esteri non devono essere acquisiti";
                        return false;
                    }
                }
            }

            if (LdatiImportiEsteri == null || LdatiImportiEsteri.Count == 0)
            {
                if (IsSingleTabSaved)
                    messaggioVideo = "Non sono presenti dati della tab 'Importi Esteri' da salvare.";
                return false;
            }

            if (LdatiImportiEsteri != null && LdatiImportiEsteri.Count > 0)
            {
                LdatiImportiEsteri.Sort(delegate
                    (PensioniCiImportiValuta c1, PensioniCiImportiValuta c2)
                { return c1.DecorrenzaPrestazioneEE.Value.CompareTo(c2.DecorrenzaPrestazioneEE.Value); });

                int index = 0;
                foreach (PensioniCiImportiValuta importoEstero in LdatiImportiEsteri)
                {
                    if (!GestioneControlli.VerificaObbligatorietaImportiEsteri(importoEstero.ImportoPrestazioneEE, importoEstero.DecorrenzaPrestazioneEE))
                    {
                        messaggioVideo = "Decorrenza e/o Importo mancanti";
                        return false;
                    }

                    if (!GestioneControlli.VerificaDataDecorrenzaImportiEsteri(importoEstero.DecorrenzaPrestazioneEE))
                    {
                        messaggioVideo = "Decorrenza posteriore al 12/1992";
                        return false;
                    }

                    if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
                    {
                        if (index == 0)
                        {
                            bool flag = false;
                            foreach (StatoEstero stato in proRata.ElencoStatiEsteri)
                            {
                                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                                    listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.PrestazioneEstera.Id);

                                flag = GestioneControlli.VerificaDecorrenzaImportiEsteriWithDecorrenzaPrestazioniEE(importoEstero.DecorrenzaPrestazioneEE, (listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0) ? listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE : null);

                                if (flag)
                                    break;
                            }
                            if (!flag)
                            {
                                messaggioVideo = "Decorrenza diversa da Decorrenza Estero";
                                return false;
                            }

                            foreach (StatoEstero stato in proRata.ElencoStatiEsteri)
                            {
                                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                                    listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.PrestazioneEstera.Id);

                                if (!GestioneControlli.VerificaDecorrenzaImportiEsteriWithPrestazioniEE(importoEstero.DecorrenzaPrestazioneEE, (listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0) ? listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE : null, stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE))
                                {
                                    messaggioVideo = "Decorrenza maggiore di Decorrenza Stato " + stato.PrestazioneEstera.NomeStato + " / " + stato.PrestazioneEstera.CodiceIstituzione;
                                    return false;
                                }
                            }
                        }
                    }

                    index++;
                }

                if (proRata != null && proRata.ElencoStatiEsteri != null && proRata.ElencoStatiEsteri.Count > 0)
                {
                    bool decIsGreaterThan91 = false;
                    bool decIsGreaterThan90 = false;
                    index = 0;
                    DateTime? dataMin = new DateTime(9999, 01, 01);

                    foreach (StatoEstero stato in proRata.ElencoStatiEsteri)
                    {
                        List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                        if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                            listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.PrestazioneEstera.Id);

                        if (listaImportiEsteriPrestazione != null && listaImportiEsteriPrestazione.Count > 0)
                        {
                            if (!stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE.HasValue)
                                if (!Utility.DataSuccessivaA(listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE.Value, dataMin.Value))
                                    dataMin = listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE;

                            if (index != 0)
                            {
                                if (!decIsGreaterThan91)
                                    decIsGreaterThan91 = Utility.DataSuccessivaA(listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE.Value, new DateTime(1991, 01, 01));
                                if (!decIsGreaterThan90)
                                    decIsGreaterThan90 = Utility.DataSuccessivaA(listaImportiEsteriPrestazione.First().DecorrenzaPrestazioneEE.Value, new DateTime(1990, 01, 01));
                            }

                            index++;
                        }
                    }

                    index = 0;

                    if (causaCarico.GetValueOrDefault() == 5 || causaCarico.GetValueOrDefault() == 9 || causaCarico.GetValueOrDefault() == 2)
                    {
                        foreach (StatoEstero stato in proRata.ElencoStatiEsteri)
                        {
                            if (stato.PrestazioneEstera.DecorrenzaLiquidazioneStatoEE.HasValue)
                            {
                                List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteriPrestazione = null;
                                if (listaImportiEsteri != null && listaImportiEsteri.Count > 0)
                                    listaImportiEsteriPrestazione = listaImportiEsteri.FindAll(x => x.IDPrestazioneEE == stato.PrestazioneEstera.Id);

                                if (!GestioneControlli.ControlsDecorrenzaImportiEsteri(LdatiImportiEsteri, listaImportiEsteriPrestazione, dataMin, decIsGreaterThan90, decIsGreaterThan91, proRata.ElencoStatiEsteri.Count, index, out messaggioVideo))
                                    return false;
                            }

                            index++;
                        }
                    }
                }
            }
            return true;
        }

        #endregion ImportiEsteri

        #region MaternitaAcna

        public static void GetDatiMaternitaAcnaByIdPensione(long idPensione, out List<MaternitaAcna> LdatiMaternitaAcna)
        {
            LdatiMaternitaAcna = null;

            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> LdatiMaternitaAcnaCommon = null;
            GestioneDatiContributiviCi.GetMaternitaAcnaByIdPensione(idPensione, out LdatiMaternitaAcnaCommon);
            if (LdatiMaternitaAcnaCommon != null)
            {
                LdatiMaternitaAcna = new List<MaternitaAcna>();
                foreach (GestioneDatiContributiviCi.PensioniCiMaternitaAcna ma in LdatiMaternitaAcnaCommon)
                {
                    MaternitaAcna maApp = new MaternitaAcna();
                    Utility.ValorizzaOggetti(ma, maApp);
                    LdatiMaternitaAcna.Add(maApp);
                }
            }
        }

        public static void StoreDatiMaternitaAcna(GestionePensione.DatiPensione datiPensione, List<MaternitaAcna> LdatiMaternitaAcna, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (ControlsDatiMaternitaAcna(datiPensione, LdatiMaternitaAcna, IsSingleTabSaved, out messaggioVideo))
            {
                GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (MaternitaAcna maternitaAcna in LdatiMaternitaAcna)
                    {
                        GestioneDatiContributiviCi.PensioniCiMaternitaAcna pensioniCiMaternitaAcna = new GestioneDatiContributiviCi.PensioniCiMaternitaAcna();
                        Utility.ValorizzaOggetti(maternitaAcna, pensioniCiMaternitaAcna);
                        pensioniCiMaternitaAcna.IdPensione = datiPensione.Id;
                        GestioneDatiContributiviCi.SalvaMaternitaAcna(pensioniCiMaternitaAcna);
                    }
                    quadroDatiContributivi.TabMaternAcna = 2;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                    transactionScope.Complete();
                }
            }
        }

        public static void CancelDatiMaternitaAcna(GestionePensione.DatiPensione datiPensione)
        {
            string messaggioVideo = string.Empty;

            List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributivi = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiContributivi);

            List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributivi = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out ldatiRetributivi);

            DatiCalcolo datiCalcolo = null;
            GetDatiCalcoloByDatiPensione(datiPensione, ldatiContributivi, ldatiRetributivi, null, out datiCalcolo, out messaggioVideo);

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiContributiviCi.EliminaAllMaternitaAcna(datiPensione.Id);

                if (datiCalcolo != null && datiCalcolo.CTRMaternitaAcna.HasValue && datiCalcolo.CTRMaternitaAcna.Value)
                    quadroDatiContributivi.TabMaternAcna = 0;
                else
                    quadroDatiContributivi.TabMaternAcna = 1;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        private static bool ControlsDatiMaternitaAcna(GestionePensione.DatiPensione datiPensione, List<MaternitaAcna> LdatiMaternitaAcna, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool isMAternitaAcnaNull = true;
            char? sesso = null;
            DateTime? decorrenza = null;

            #region GetData
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiGenerici);

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            GestioneAnagrafica.DatiAnagrafici datiAnagraficiDC = null;
            if (datiDanteCausa != null)
                GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausa.IdAnagrafica, out datiAnagraficiDC);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(datiPensione.Id, out listaDatiCalcoloRetributivo);

            if (datiAnagraficiDC != null && datiAnagraficiDC.Sesso.HasValue)
                sesso = datiAnagraficiDC.Sesso;
            else
                sesso = datiAnagrafici.Sesso;

            if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                decorrenza = datiDanteCausa.DecorrenzaPensione;
            else
                decorrenza = datiPensione.DecorrenzaOriginaria;

            int? settimaneRetributiveQuotaACodGestione1 = null;
            int? settimaneRetributiveQuotaBCodGestione1 = null;
            decimal? rmsQuotaACodGestione1 = null;
            decimal? rmsQuotaBCodGestione1 = null;
            if (listaDatiCalcoloRetributivo != null && listaDatiCalcoloRetributivo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo retr in listaDatiCalcoloRetributivo)
                {
                    if (retr.CodiceGestione == 1)
                    {
                        if (retr.QuotePrimeLiquidate == 'A')
                        {
                            settimaneRetributiveQuotaACodGestione1 = retr.NSettimaneQuotaA;
                            rmsQuotaACodGestione1 = retr.RMSQuotaA;
                        }
                        if (retr.QuotePrimeLiquidate == 'B')
                        {
                            settimaneRetributiveQuotaBCodGestione1 = retr.NSettimaneQuotaB;
                            rmsQuotaBCodGestione1 = retr.RMSQuotaB;
                        }
                    }
                }
            }

            #endregion GetData


            if (LdatiMaternitaAcna != null && LdatiMaternitaAcna.Count > 0)
            {
                foreach (MaternitaAcna ma in LdatiMaternitaAcna)
                {
                    if (ma.ImportoIVS.HasValue || ma.SettimaneAl1292.HasValue || ma.SettimaneDL50392.HasValue)
                    {
                        isMAternitaAcnaNull = false;
                        break;
                    }
                }

                foreach (MaternitaAcna ma in LdatiMaternitaAcna)
                {
                    if (ma.Tipo == 'M')
                    {
                        if (ma.SettimaneAl1292.GetValueOrDefault() > 0 || ma.SettimaneDL50392.GetValueOrDefault() > 0 || ma.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.ControlsMaternita(ma.SettimaneAl1292, ma.SettimaneDL50392, sesso, decorrenza, ma.ImportoIVS, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaMaternitaWithDatiCalcolo(ma.SettimaneAl1292, settimaneRetributiveQuotaACodGestione1, rmsQuotaACodGestione1, ma.SettimaneDL50392, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                                return false;
                        }
                    }

                    if (ma.Tipo == 'A')
                    {
                        if (ma.SettimaneAl1292.GetValueOrDefault() > 0 || ma.SettimaneDL50392.GetValueOrDefault() > 0 || ma.ImportoIVS.GetValueOrDefault() > 0)
                        {
                            if (!GestioneControlli.ControlsAcna(ma.SettimaneAl1292, ma.SettimaneDL50392, decorrenza, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null, datiDanteCausa != null ? datiDanteCausa.DataMorte : null, ma.ImportoIVS, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaAcnaWithDatiAssicurativi(ma.SettimaneAl1292, ma.SettimaneDL50392, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, out messaggioVideo))
                                return false;

                            if (!GestioneControlli.VerificaAcnaWithDatiCalcolo(ma.SettimaneAl1292, ma.SettimaneDL50392, settimaneRetributiveQuotaACodGestione1, rmsQuotaACodGestione1, settimaneRetributiveQuotaBCodGestione1, rmsQuotaBCodGestione1, out messaggioVideo))
                                return false;
                        }
                    }
                }

                if (!isMAternitaAcnaNull && ((datiGenerici != null && !datiGenerici.MaternitaAcna.HasValue) || (datiGenerici != null && datiGenerici.MaternitaAcna.HasValue && !datiGenerici.MaternitaAcna.Value)))
                {
                    messaggioVideo = "Salvare i dati della tab 'Dati Calcolo' prima di procedere con il salvataggio dei dati della tab 'Maternità/Acna'";
                    return false;
                }
            }
            else
            {
                if (IsSingleTabSaved)
                    messaggioVideo = "Non sono presenti dati della tab 'Maternità/Acna' da salvare.";
                return false;
            }
            return true;
        }

        #endregion MaternitaAcna

        #region DatiPostDecOriginaria
        public static void GetDatiPostDecOriginariaByIdPensione(long idPensione, out List<DatiPostDecOriginaria> LDatiPostDecOriginaria)
        {
            LDatiPostDecOriginaria = null;
            List<GestioneDatiContributiviCi.DatiPostDecOriginaria> LDatiPostDecOriginariaCommon = null;
            GestioneDatiContributiviCi.GetDatiPostDecOriginariaByIdPensione(idPensione, out LDatiPostDecOriginariaCommon);
            if (LDatiPostDecOriginariaCommon != null && LDatiPostDecOriginariaCommon.Count > 0)
            {
                LDatiPostDecOriginaria = new List<DatiPostDecOriginaria>();
                foreach (GestioneDatiContributiviCi.DatiPostDecOriginaria datiPostDecOriginariaCommon in LDatiPostDecOriginariaCommon)
                {
                    DatiPostDecOriginaria datiPostDecOriginaria = new DatiPostDecOriginaria();
                    Utility.ValorizzaOggetti(datiPostDecOriginariaCommon, datiPostDecOriginaria);
                    LDatiPostDecOriginaria.Add(datiPostDecOriginaria);
                }
            }
        }

        public static void StoreDatiPostDecOriginaria(GestionePensione.DatiPensione datiPensione, List<DatiPostDecOriginaria> LDatiPostDecOriginaria, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (ControlsDatiPostDecOriginaria(LDatiPostDecOriginaria, IsSingleTabSaved, out messaggioVideo))
            {
                GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    List<GestioneDatiContributiviCi.DatiPostDecOriginaria> LDatiPostDecOriginariaCommon = new List<GestioneDatiContributiviCi.DatiPostDecOriginaria>();
                    foreach (DatiPostDecOriginaria datiPostDecOriginaria in LDatiPostDecOriginaria)
                    {
                        GestioneDatiContributiviCi.DatiPostDecOriginaria datiPostDecOriginariaCommon = new GestioneDatiContributiviCi.DatiPostDecOriginaria();
                        Utility.ValorizzaOggetti(datiPostDecOriginaria, datiPostDecOriginariaCommon);
                        LDatiPostDecOriginariaCommon.Add(datiPostDecOriginariaCommon);
                    }
                    GestioneDatiContributiviCi.EliminaAllDatiPostDecOriginaria(datiPensione.Id);
                    GestioneDatiContributiviCi.SalvaDatiPostDecOriginaria(datiPensione.Id, LDatiPostDecOriginariaCommon);
                    datiQuadroDatiContributivi.TabDatiPostDecOriginaria = 2;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                    transactionScope.Complete();
                }
            }
        }

        public static void CancelDatiPostDecOriginaria(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out datiQuadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiContributiviCi.EliminaAllDatiPostDecOriginaria(datiPensione.Id);
                datiQuadroDatiContributivi.TabDatiPostDecOriginaria = 1;
                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, datiQuadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiPostDecOriginaria(List<DatiPostDecOriginaria> LDatiPostDecOriginaria, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (LDatiPostDecOriginaria != null && LDatiPostDecOriginaria.Count > 0)
            {

            }
            else
            {
                if (IsSingleTabSaved)
                    messaggioVideo = "Inserire almeno un record per i Dati Post Decorrenza Originaria";
                return false;
            }

            return true;
        }
        #endregion DatiPostDecOriginaria

        #region LavoratoriAutonomi

        public static void GetDatiLavoratoriAutonomiByIdPensione(long idPensione, GestioneIstruttoria.DatiIstruttoria datiIstruttoria, out LavoratoriAutonomi datiLavoratoriAutonomi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            datiLavoratoriAutonomi = null;

            if (datiIstruttoria != null && (datiIstruttoria.NContributiUtiliLavoratoriAutonomi != null || datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi != null || datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi != null))
            {
                datiLavoratoriAutonomi = new LavoratoriAutonomi();
                datiLavoratoriAutonomi.IdPensione = idPensione;
                datiLavoratoriAutonomi.NContributiUtiliLavoratoriAutonomi = datiIstruttoria.NContributiUtiliLavoratoriAutonomi;
                datiLavoratoriAutonomi.NSettimaneVVDirittoLavoratoriAutonomi = datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi;
                datiLavoratoriAutonomi.NSettimaneVVMisuraLavoratoriAutonomi = datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi;
            }
        }

        public static void StoreDatiLavoratoriAutonomi(GestionePensione.DatiPensione datiPensione, LavoratoriAutonomi datiLavoratoriAutonomi, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (ControlsDatiLavoratoriAutonomi(datiLavoratoriAutonomi, IsSingleTabSaved, out messaggioVideo))
            {
                if (datiLavoratoriAutonomi.NContributiUtiliLavoratoriAutonomi.HasValue || datiLavoratoriAutonomi.NSettimaneVVDirittoLavoratoriAutonomi.HasValue ||
                    datiLavoratoriAutonomi.NSettimaneVVMisuraLavoratoriAutonomi.HasValue)
                {
                    if (datiIstruttoria == null)
                        datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria();

                    datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi = datiLavoratoriAutonomi.NSettimaneVVMisuraLavoratoriAutonomi;
                    datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi = datiLavoratoriAutonomi.NSettimaneVVDirittoLavoratoriAutonomi;
                    datiIstruttoria.NContributiUtiliLavoratoriAutonomi = datiLavoratoriAutonomi.NContributiUtiliLavoratoriAutonomi;
                }

                GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
                GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (datiIstruttoria != null)
                        GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);

                    quadroDatiContributivi.TabLavAutonomi = 2;
                    GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                    transactionScope.Complete();
                }
            }
        }

        public static void CancelDatiLavoratoriAutonomi(GestionePensione.DatiPensione datiPensione, ref GestioneIstruttoria.DatiIstruttoria datiIstruttoria)
        {
            if (datiIstruttoria != null)
            {
                datiIstruttoria.NContributiUtiliLavoratoriAutonomi = null;
                datiIstruttoria.NSettimaneVVDirittoLavoratoriAutonomi = null;
                datiIstruttoria.NSettimaneVVMisuraLavoratoriAutonomi = null;
            }

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiIstruttoria != null && GestioneIstruttoria.IsIstruttoriaNull(datiIstruttoria))
                    GestioneIstruttoria.EliminaIstruttoriaByIdPensione(datiPensione.Id);
                else
                    GestioneIstruttoria.SalvaIstruttoria(datiPensione.Id, datiIstruttoria);
                quadroDatiContributivi.TabLavAutonomi = 1;
                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        private static bool ControlsDatiLavoratoriAutonomi(LavoratoriAutonomi datiLavoratoriAutonomi, bool IsSingleTabSaved, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiLavoratoriAutonomi == null)
            {
                if (IsSingleTabSaved)
                    messaggioVideo = "Non sono presenti dati della tab 'Lavoratori Autonomi' da salvare.";
                return false;
            }

            return true;
        }

        #endregion LavoratoriAutonomi

        #region RedditiPerIntegrazioneVirtuale

        public static void GetDatiRedditiPerIntegrazioneVirtualeByIdPensione(GestionePensione.DatiPensione datiPensione, out List<RedditiPerIntegrazioneVirtuale> LredditiPerIntegrazVirt, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            LredditiPerIntegrazVirt = null;

            List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale> LredditiPerIntegrazVirtDB = null;
            GestioneDatiContributiviCi.GetRedditiPerIntegrazioneVirtuale(datiPensione.Id, out LredditiPerIntegrazVirtDB);

            List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> listaImportiEsteri = null;
            GestioneDatiContributiviCi.GetImportiEsteriByIdPensione(datiPensione.Id, out listaImportiEsteri);

            if (LredditiPerIntegrazVirtDB == null)
            {
                LredditiPerIntegrazVirt = new List<RedditiPerIntegrazioneVirtuale>();

                RedditiPerIntegrazioneVirtuale reddConiuge = new RedditiPerIntegrazioneVirtuale();
                reddConiuge.Anno = datiPensione.DecorrenzaOriginaria.Value.Year;
                LredditiPerIntegrazVirt.Add(reddConiuge);

                RedditiPerIntegrazioneVirtuale reddTitolare = new RedditiPerIntegrazioneVirtuale();
                reddTitolare.Anno = datiPensione.DecorrenzaOriginaria.Value.Year;
                reddTitolare.IsTitolare = true;
                LredditiPerIntegrazVirt.Add(reddTitolare);

                foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri anniRedd in listaImportiEsteri)
                {
                    RedditiPerIntegrazioneVirtuale anniReddConiuge = new RedditiPerIntegrazioneVirtuale();
                    RedditiPerIntegrazioneVirtuale anniReddTitolare = new RedditiPerIntegrazioneVirtuale();

                    if (anniRedd.DecorrenzaPrestazioneEE.Value.Year != datiPensione.DecorrenzaOriginaria.Value.Year)
                    {
                        anniReddConiuge.Anno = anniRedd.DecorrenzaPrestazioneEE.Value.Year;
                        if (!LredditiPerIntegrazVirt.Exists(x => x.Anno == anniReddConiuge.Anno))
                            LredditiPerIntegrazVirt.Add(anniReddConiuge);

                        anniReddTitolare.Anno = anniRedd.DecorrenzaPrestazioneEE.Value.Year;
                        anniReddTitolare.IsTitolare = true;
                        if (!LredditiPerIntegrazVirt.Exists(x => x.Anno == anniReddTitolare.Anno && x.IsTitolare))
                            LredditiPerIntegrazVirt.Add(anniReddTitolare);
                    }
                }
            }
            else
            {
                LredditiPerIntegrazVirt = new List<RedditiPerIntegrazioneVirtuale>();
                foreach (GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale redditiIV in LredditiPerIntegrazVirtDB)
                {
                    RedditiPerIntegrazioneVirtuale redd = new RedditiPerIntegrazioneVirtuale();
                    Utility.ValorizzaOggetti(redditiIV, redd);
                    LredditiPerIntegrazVirt.Add(redd);
                }
            }
        }

        public static void StoreDatiRedditiPerIntegrazioneVirtuale(GestionePensione.DatiPensione datiPensione, List<RedditiPerIntegrazioneVirtuale> LredditiPerIntegrazVirt, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale> LDatiReddIntegrazVirt = new List<GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale>();
                foreach (RedditiPerIntegrazioneVirtuale reddIntegraVirt in LredditiPerIntegrazVirt)
                {
                    GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale pensioniCiReddIntegrazVirt = new GestioneDatiContributiviCi.PensioniCiRedditiPerIntegrazioneVirtuale();
                    Utility.ValorizzaOggetti(reddIntegraVirt, pensioniCiReddIntegrazVirt);
                    LDatiReddIntegrazVirt.Add(pensioniCiReddIntegrazVirt);

                }
                GestioneDatiContributiviCi.EliminaAllRedditiPerIntegrazioneVirtuale(datiPensione.Id);
                GestioneDatiContributiviCi.SalvaRedditiPerIntegrazioneVirtuale(datiPensione.Id, LDatiReddIntegrazVirt);
                quadroDatiContributivi.TabIntegrazioneVirtuale = 2;
                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        public static void CancelDatiRedditiPerIntegrazioneVirtuale(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = null;
            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEE);

            string codiceStato = string.Empty;
            codiceStato = listaPrestazioniEE[0].CodiceStatoEE;

            List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
            GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePerStato(codiceStato, datiPensione.DecorrenzaOriginaria, out listaCodiciConvenzione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiContributiviCi.EliminaAllRedditiPerIntegrazioneVirtuale(datiPensione.Id);

                if (listaCodiciConvenzione[0].CodiceConvenzione != 13)
                    quadroDatiContributivi.TabIntegrazioneVirtuale = 0;
                else
                    quadroDatiContributivi.TabIntegrazioneVirtuale = 1;

                GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

                transactionScope.Complete();
            }
        }

        #endregion RedditiPerIntegrazioneVirtuale

        #region nested classes

        public class StatoEstero
        {
            #region private properties
            private PrestazioneEstera _PrestazioneEstera;
            private PrestazioneEstera _PrestazioneEsteraStorico;
            private List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> _ElencoImportiEsteri;
            #endregion private properties

            #region public properties
            public PrestazioneEstera PrestazioneEstera { get { return _PrestazioneEstera; } set { _PrestazioneEstera = value; } }
            public PrestazioneEstera PrestazioneEsteraStorico { get { return _PrestazioneEsteraStorico; } set { _PrestazioneEsteraStorico = value; } }
            public List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> ElencoImportiEsteri { get { return _ElencoImportiEsteri; } set { _ElencoImportiEsteri = value; } }

            #endregion public properties

        }

        public class PrestazioneEstera : GestioneDatiContributiviCi.PensioniCiPrestazioniEE
        {
            public PrestazioneEstera()
            { }

            public PrestazioneEstera(string codiceStatoIstituzione, string sigla, string citta,
                string nomeStato, string siglaStato, string codiceConvenzione, string matricolaIstituzione, char? codicePi, bool confermato)
            {
                this.CodiceStatoEE = codiceStatoIstituzione.Length == 6 ? codiceStatoIstituzione.Substring(0, 2) : "";
                this.CodiceIstituzione = codiceStatoIstituzione.Length == 6 ? codiceStatoIstituzione.Substring(2, 4) : "";
                this.CodicePi = codicePi;
                this._Sigla = sigla;
                this._Citta = citta;
                this._NomeStato = nomeStato;
                this._SiglaStato = siglaStato;
                this.CodiceConvenzione = Utility.StringToNullableByte(codiceConvenzione);
                this._MatricolaIstituzione = matricolaIstituzione;
                this.Confermato = confermato;
            }
            #region private properties
            private string _Sigla;
            private string _Citta;
            private string _NomeStato;
            private string _SiglaStato;
            private string _MatricolaIstituzione;
            private bool _IsStorico;

            #endregion private properties

            #region public properties
            public string Sigla { get { return _Sigla; } set { _Sigla = value; } }
            public string Citta { get { return _Citta; } set { _Citta = value; } }
            public string NomeStato { get { return _NomeStato; } set { _NomeStato = value; } }
            public string SiglaStato { get { return _SiglaStato; } set { _SiglaStato = value; } }
            public string MatricolaIstituzione { get { return _MatricolaIstituzione; } set { _MatricolaIstituzione = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

            #endregion public properties
        }

        public class CodiceConvenzione
        {
            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;

            #endregion private properties
        }

        public class CodiceVirtuale
        {
            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class RegimeLiquidazione
        {
            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        #region ProRata

        public class PensioniCiPrestazioniEE
        {

            #region private properties
            private long? _Id;

            private long _IdPensione;

            private string _CodiceStatoEE;

            private string _CodiceIstituzione;

            private string _MatricolaIstituzioneEE;

            private int? _ContributiEEDecorrenzaOriginaria;

            private int? _ContributiEERicalcolo;

            private DateTime? _DecorrenzaLiquidazioneStatoEE;

            private int? _ContributiEEDiritto;

            private char? _SospensioneCautelativaIntegrazione;

            private byte? _EtaSospensione;

            private char _CodiceArt48;

            private DateTime? _DecorrenzaArt48;

            private decimal? _QuotaIntegrazioneEEeArgentinaResidentiItalia;

            private DateTime? _DecorrenzaIntegrazione;

            private DateTime? _DecorrenzaRicalcolo;

            private byte? _CodiceConvenzione;
            #endregion private properties

            #region public properties
            public long? Id { get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public string CodiceStatoEE { get { return _CodiceStatoEE; } set { _CodiceStatoEE = value; } }

            public string CodiceIstituzione { get { return _CodiceIstituzione; } set { _CodiceIstituzione = value; } }

            public string MatricolaIstituzioneEE { get { return _MatricolaIstituzioneEE; } set { _MatricolaIstituzioneEE = value; } }

            public int? ContributiEEDecorrenzaOriginaria { get { return _ContributiEEDecorrenzaOriginaria; } set { _ContributiEEDecorrenzaOriginaria = value; } }

            public int? ContributiEERicalcolo { get { return _ContributiEERicalcolo; } set { _ContributiEERicalcolo = value; } }

            public DateTime? DecorrenzaLiquidazioneStatoEE { get { return _DecorrenzaLiquidazioneStatoEE; } set { _DecorrenzaLiquidazioneStatoEE = value; } }

            public int? ContributiEEDiritto { get { return _ContributiEEDiritto; } set { _ContributiEEDiritto = value; } }

            public char? SospensioneCautelativaIntegrazione { get { return _SospensioneCautelativaIntegrazione; } set { _SospensioneCautelativaIntegrazione = value; } }

            public byte? EtaSospensione { get { return _EtaSospensione; } set { _EtaSospensione = value; } }

            public char CodiceArt48 { get { return _CodiceArt48; } set { _CodiceArt48 = value; } }

            public DateTime? DecorrenzaArt48 { get { return _DecorrenzaArt48; } set { _DecorrenzaArt48 = value; } }

            public decimal? QuotaIntegrazioneEEeArgentinaResidentiItalia { get { return _QuotaIntegrazioneEEeArgentinaResidentiItalia; } set { _QuotaIntegrazioneEEeArgentinaResidentiItalia = value; } }

            public DateTime? DecorrenzaIntegrazione { get { return _DecorrenzaIntegrazione; } set { _DecorrenzaIntegrazione = value; } }

            public DateTime? DecorrenzaRicalcolo { get { return _DecorrenzaRicalcolo; } set { _DecorrenzaRicalcolo = value; } }

            public byte? CodiceConvenzione { get { return _CodiceConvenzione; } set { _CodiceConvenzione = value; } }
            #endregion public properties
        }

        public class PensioniCiImportiEsteri
        {
            #region private properties

            private long? _Id;

            private DateTime? _DecorrenzaPrestazioneEE;

            private DateTime? _CessazionePrestazioneEE;

            private decimal? _ImportoPrestazioneEE;

            private long _IDPrestazioneEE;

            #endregion private properties

            #region public properties

            public long? Id { get { return _Id; } set { _Id = value; } }

            public DateTime? DecorrenzaPrestazioneEE { get { return _DecorrenzaPrestazioneEE; } set { _DecorrenzaPrestazioneEE = value; } }

            public DateTime? CessazionePrestazioneEE { get { return _CessazionePrestazioneEE; } set { _CessazionePrestazioneEE = value; } }

            public decimal? ImportoPrestazioneEE { get { return _ImportoPrestazioneEE; } set { _ImportoPrestazioneEE = value; } }

            public long IDPrestazioneEE { get { return _IDPrestazioneEE; } set { _IDPrestazioneEE = value; } }

            #endregion public properties
        }

        public class ProRata
        {
            #region private properties

            private List<StatoEstero> _ElencoStatiEsteri;
            private bool? _IsDataFromDB;
            #endregion private properties

            #region public properties

            public List<StatoEstero> ElencoStatiEsteri { get { return _ElencoStatiEsteri; } set { _ElencoStatiEsteri = value; } }
            public bool? IsDataFromDB { get { return _IsDataFromDB; } set { _IsDataFromDB = value; } }
            #endregion public properties
        }

        #endregion ProRata

        #region DatiCalcolo

        public class DatiCalcolo
        {
            public DatiCalcolo()
            {
            }

            public DatiCalcolo(GestioneAggiornamentoPECO.DatiTotaliAggPec datiAggPec)
            {
                if (datiAggPec == null || datiAggPec.IsNull())
                    return;

                if (datiAggPec.lRetribuzione != null)
                    this._LDatiRetributivi = datiAggPec.lRetribuzione;

                if (datiAggPec.lContribuzione != null)
                    this._LDatiContributivi = datiAggPec.lContribuzione;

                if (datiAggPec.lContribuzioneEE != null)
                    this._LDatiContributiEsteri = datiAggPec.lContribuzioneEE;

                if (datiAggPec.DatiControllo != null)
                {
                    this._IsCalcoloValido = datiAggPec.DatiControllo.IsCalcoloValido;
                }

                if (datiAggPec.ContributiItalianiEdEsteriAl1295 != null)
                    this._ContributiItalianiEdEsteriAl1295 = datiAggPec.ContributiItalianiEdEsteriAl1295;
            }

            #region private properties
            private List<GestioneAggiornamentoPECO.DatiContributivi> _LDatiContributivi;
            private List<GestioneAggiornamentoPECO.DatiRetributivi> _LDatiRetributivi;
            private List<GestioneAggiornamentoPECO.DatiContributiEsteri> _LDatiContributiEsteri;

            private long _IdPensione;
            private bool _IsCalcoloValido;
            private bool _IsUnicarpe;
            private bool? _IsDataFromDB;

            private DateTime? _InizioAssicurazione;
            private DateTime? _FineAssicurazione;
            private bool? _CTRMaternitaAcna;
            private decimal? _MontanteInvalidita;
            private int? _ContributiItalianiEdEsteriAl1295;
            private bool _IsSettimane707Visible;

            #endregion private properties

            #region public properties

            public List<GestioneAggiornamentoPECO.DatiContributivi> LDatiContributivi { get { return _LDatiContributivi; } set { _LDatiContributivi = value; } }
            public List<GestioneAggiornamentoPECO.DatiRetributivi> LDatiRetributivi { get { return _LDatiRetributivi; } set { _LDatiRetributivi = value; } }
            public List<GestioneAggiornamentoPECO.DatiContributiEsteri> LDatiContributiEsteri { get { return _LDatiContributiEsteri; } set { _LDatiContributiEsteri = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public bool IsCalcoloValido { get { return _IsCalcoloValido; } set { _IsCalcoloValido = value; } }
            public bool IsUnicarpe { get { return _IsUnicarpe; } set { _IsUnicarpe = value; } }
            public bool? IsDataFromDB { get { return _IsDataFromDB; } set { _IsDataFromDB = value; } }

            public DateTime? InizioAssicurazione { get { return _InizioAssicurazione; } set { _InizioAssicurazione = value; } }
            public DateTime? FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }
            public bool? CTRMaternitaAcna { get { return _CTRMaternitaAcna; } set { _CTRMaternitaAcna = value; } }
            public decimal? MontanteInvalidita { get { return _MontanteInvalidita; } set { _MontanteInvalidita = value; } }
            public int? ContributiItalianiEdEsteriAl1295 { get { return _ContributiItalianiEdEsteriAl1295; } set { _ContributiItalianiEdEsteriAl1295 = value; } }
            public bool IsSettimane707Visible { get { return _IsSettimane707Visible; } set { _IsSettimane707Visible = value; } }

            #endregion public properties
        }

        #endregion DatiCalcolo

        #region PensioniCiImportiValuta

        public class PensioniCiImportiValuta
        {
            #region public properties
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? DecorrenzaPrestazioneEE { get { return _DecorrenzaPrestazioneEE; } set { _DecorrenzaPrestazioneEE = value; } }
            public decimal? ImportoPrestazioneEE { get { return _ImportoPrestazioneEE; } set { _ImportoPrestazioneEE = value; } }
            #endregion public properties

            #region private properties
            private long? _IdPensione;
            private DateTime? _DecorrenzaPrestazioneEE;
            private decimal? _ImportoPrestazioneEE;
            #endregion private properties
        }

        //public class ImportiEsteri
        //{
        //    #region public properties

        //    public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        //    public List<PensioniCiImportiValuta> LpensioniCiImportiValuta { get { return _LpensioniCiImportiValuta; } set { _LpensioniCiImportiValuta = value; } }
        //    #endregion public properties

        //    #region private properties

        //    private long _IdPensione;
        //    public List<PensioniCiImportiValuta> _LpensioniCiImportiValuta;

        //    #endregion private properties

        //}

        #endregion PensioniCiImportiValuta

        public class MaternitaAcna
        {
            #region private properties
            private long? _Id;
            private long _IdPensione;
            private decimal? _ImportoIVS;
            private int? _SettimaneAl1292;
            private int? _SettimaneDL50392;
            private char? _Tipo;
            #endregion private properties

            #region public properties
            public long? Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
            public int? SettimaneAl1292 { get { return _SettimaneAl1292; } set { _SettimaneAl1292 = value; } }
            public int? SettimaneDL50392 { get { return _SettimaneDL50392; } set { _SettimaneDL50392 = value; } }
            public char? Tipo { get { return _Tipo; } set { _Tipo = value; } }
            #endregion public properties
        }

        public class DatiPostDecOriginaria
        {
            #region private properties
            private long _Id;
            private long _IdPensione;
            private DateTime? _Decorrenza;
            private int? _CTR;
            private decimal? _IVS;
            private int? _SettimaneRetributive;
            private int? _SettimaneVV;
            private decimal? _RMS;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public int? CTR { get { return _CTR; } set { _CTR = value; } }
            public decimal? IVS { get { return _IVS; } set { _IVS = value; } }
            public int? SettimaneRetributive { get { return _SettimaneRetributive; } set { _SettimaneRetributive = value; } }
            public int? SettimaneVV { get { return _SettimaneVV; } set { _SettimaneVV = value; } }
            public decimal? RMS { get { return _RMS; } set { _RMS = value; } }
            #endregion public properties

            #region public methods

            public bool IsNull()
            {
                if (this._Decorrenza.HasValue ||
                    this._CTR.HasValue ||
                    this._IVS.HasValue ||
                    this._SettimaneRetributive.HasValue ||
                    this._SettimaneVV.HasValue ||
                    this._RMS.HasValue)
                    return false;

                return true;
            }

            #endregion public methods
        }

        public class LavoratoriAutonomi
        {
            #region private properties
            private long? _Id;
            private long _IdPensione;
            private int? _NContributiUtiliLavoratoriAutonomi;
            private int? _NSettimaneVVDirittoLavoratoriAutonomi;
            private int? _NSettimaneVVMisuraLavoratoriAutonomi;
            #endregion private properties

            #region public properties
            public long? Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int? NContributiUtiliLavoratoriAutonomi { get { return _NContributiUtiliLavoratoriAutonomi; } set { _NContributiUtiliLavoratoriAutonomi = value; } }
            public int? NSettimaneVVDirittoLavoratoriAutonomi { get { return _NSettimaneVVDirittoLavoratoriAutonomi; } set { _NSettimaneVVDirittoLavoratoriAutonomi = value; } }
            public int? NSettimaneVVMisuraLavoratoriAutonomi { get { return _NSettimaneVVMisuraLavoratoriAutonomi; } set { _NSettimaneVVMisuraLavoratoriAutonomi = value; } }
            #endregion public properties
        }

        public class RedditiPerIntegrazioneVirtuale
        {
            #region private properties
            private long _Id;
            private long _IdPensione;
            private int _Anno;
            private decimal? _Reddito;
            private bool _IsTitolare;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int Anno { get { return _Anno; } set { _Anno = value; } }
            public decimal? Reddito { get { return _Reddito; } set { _Reddito = value; } }
            public bool IsTitolare { get { return _IsTitolare; } set { _IsTitolare = value; } }
            #endregion public properties
        }

        #endregion nested classes

        public enum TipoCalcolo
        {
            NonValido,
            Contributivo = 22,
            Retributivo = 23,
            Misto = 24
        };

        public static void GetListaDecodificaGestioneCalcoloRetributivo(GestionePensione.DatiPensione datiPensione, out List<Entity.DecodificaGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivo)
        {
            listaDecodificaGestioneCalcoloRetributivo = null;
            List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetrCommon = null;
            GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetrCommon);

            if (elencoCodeGestioneCalcoloRetrCommon != null)
            {
                GetCodeGestioneCalcoloRetributivoCustom(datiPensione, ref elencoCodeGestioneCalcoloRetrCommon);
                listaDecodificaGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetrCommon.Select(x => { var y = new Entity.DecodificaGestioneCalcoloRetributivo(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
            }
        }

        public static void GetCodeGestioneCalcoloRetributivoCustom(GestionePensione.DatiPensione datiPensione, ref List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivoCustom)
        {
            if (datiPensione != null)
            {
                if (listaDecodificaGestioneCalcoloRetributivoCustom != null && listaDecodificaGestioneCalcoloRetributivoCustom.Count > 0)
                {
                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> listaDecodificaGestioneCalcoloRetributivoApp = listaDecodificaGestioneCalcoloRetributivoCustom.ToList();
                    string codCat = datiPensione.GetCodCategoria();
                    string filtro = datiPensione.GetFiltro();
                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                    {
                        listaDecodificaGestioneCalcoloRetributivoCustom = listaDecodificaGestioneCalcoloRetributivoApp.Where(e => e.TraduzioneSuGP.Trim() == "1" || e.TraduzioneSuGP.Trim() == "2" || e.TraduzioneSuGP.Trim() == "3" || e.TraduzioneSuGP.Trim() == "4").ToList();
                    }
                    else
                    {

                        foreach (GestioneDecodifica.CodeGestioneCalcoloRetributivo codeGestione in listaDecodificaGestioneCalcoloRetributivoApp)
                        {
                            switch (codeGestione.TraduzioneSuGP.Trim())
                            {
                                case "2":
                                case "3":
                                case "4":
                                    if (codCat == "0004" || codCat == "0005")
                                        listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                    break;
                                case "A":
                                case "Q":
                                case "P":
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                    break;
                                case "S":
                                    if (filtro != "BNS" && filtro != "BNX")
                                        listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                    break;
                                case "1H":
                                    if (filtro != "RAL" && filtro != "R44" && filtro != "R45")
                                        listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                    break;
                                case "2H":
                                case "3H":
                                case "4H":
                                    if (codCat == "0004" || codCat == "0005" || (filtro != "RAL" && filtro != "R44" && filtro != "R45"))
                                        listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                    break;
                                case "I":
                                    listaDecodificaGestioneCalcoloRetributivoCustom.Remove(codeGestione);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static void GetListaDecodificaGestioneCalcoloContributivo(GestionePensione.DatiPensione datiPensione, out List<DecodificaGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivo)
        {
            listaDecodificaGestioneCalcoloContributivo = null;
            List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContrCommon = null;
            GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContrCommon);

            if (elencoCodeGestioneCalcoloContrCommon != null)
            {
                GetCodeGestioneCalcoloContributivoCustom(datiPensione, ref elencoCodeGestioneCalcoloContrCommon);
                listaDecodificaGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContrCommon.Select(x => { var y = new Entity.DecodificaGestioneCalcoloContributivo(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();
            }
        }

        public static void GetCodeGestioneCalcoloContributivoCustom(GestionePensione.DatiPensione datiPensione, ref List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivoCustom)
        {
            if (datiPensione != null)
            {
                if (listaDecodificaGestioneCalcoloContributivoCustom != null && listaDecodificaGestioneCalcoloContributivoCustom.Count > 0)
                {
                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> listaDecodificaGestioneCalcoloContributivoApp = listaDecodificaGestioneCalcoloContributivoCustom.ToList();
                    string codCat = datiPensione.GetCodCategoria();
                    string filtro = datiPensione.GetFiltro();

                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                    {
                        listaDecodificaGestioneCalcoloContributivoCustom = listaDecodificaGestioneCalcoloContributivoApp.Where(e => e.TraduzioneSuGP.Trim() == "1" || e.TraduzioneSuGP.Trim() == "2" || e.TraduzioneSuGP.Trim() == "3" || e.TraduzioneSuGP.Trim() == "4").ToList();
                    }
                    else
                    {
                        foreach (GestioneDecodifica.CodeGestioneCalcoloContributivo codeGestione in listaDecodificaGestioneCalcoloContributivoApp)
                        {
                            switch (codeGestione.TraduzioneSuGP.Trim())
                            {
                                case "2":
                                case "3":
                                case "4":
                                    if (codCat == "0004" || codCat == "0005")
                                        listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                    break;
                                case "S":
                                    if (filtro != "BNS" && filtro != "BNX")
                                        listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                    break;
                                case "1H":
                                    if (filtro != "RAL" && filtro != "R44" && filtro != "R45")
                                        listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                    break;
                                case "2H":
                                case "3H":
                                case "4H":
                                    if (codCat == "0004" || codCat == "0005" || (filtro != "RAL" && filtro != "R44" && filtro != "R45"))
                                        listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                    break;
                                case "A":
                                case "Q":
                                //COD GEST AUT
                                case "G":
                                case "C1":
                                case "C2":
                                case "C3":
                                case "C4":
                                case "C5":
                                case "E1":
                                case "E2":
                                case "F0":
                                case "F1":
                                //Cod Banc
                                case "O":
                                case "P":
                                    listaDecodificaGestioneCalcoloContributivoCustom.Remove(codeGestione);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static void GetListaDecodificaCodeGestione(GestionePensione.DatiPensione datiPensione, out List<DecodificaCodeGestione> listaDecodificaCodeGestione)
        {
            listaDecodificaCodeGestione = null;
            List<GestioneDecodifica.CodeGestione> elencoCodeGestioneBL = null;
            GestioneDecodifica.GetCodiceGestione(out elencoCodeGestioneBL);
            if (elencoCodeGestioneBL != null)
                listaDecodificaCodeGestione = elencoCodeGestioneBL.Select(x => { var y = new Entity.DecodificaCodeGestione(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();


        }

        public static bool IsSettimane707Visible(GestionePensione.DatiPensione datiPensione, List<GestioneAggiornamentoPECO.DatiRetributivi> lDatiRetributivi,
            List<GestioneAggiornamentoPECO.DatiContributivi> lDatiContributivi, bool? flagContributiva)
        {
            List<GestioneCalcolo.DatiCalcoloRetributivo> datiRetributivi = MappingDatiRetributiviFromViewToBL(lDatiRetributivi);
            List<GestioneCalcolo.DatiCalcoloContributivo> datiContributivi = MappingDatiContributiviFromViewToBL(lDatiContributivi);

            return GestioneContrib.IsSettimane707Visible(datiPensione, datiRetributivi, datiContributivi, flagContributiva);
        }

        public static bool IsSettimane707Visible(GestionePensione.DatiPensione datiPensione, List<GestioneCalcolo.DatiCalcoloRetributivo> lDatiRetributivi,
            List<GestioneCalcolo.DatiCalcoloContributivo> lDatiContributivi, bool? flagContributiva)
        {
            DateTime dataCompare = new DateTime(2012, 1, 2);

            if (Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione) ||
                Utility.IsDomandaTipoContributivo(datiPensione, null, false) || Utility.isDomandaVecchiaiaTrasformazioneAOICalcoloContributivo(datiPensione)) //ENG - Memo 166/2023
                return false;

            if (lDatiContributivi != null && lDatiContributivi.Exists(x => x.IsQuotaL335Presente()))
                return false;

            // si applica il doppio calcolo legge 707 se il calcolo è retributivo, la pensione ha decorrenza successiva a 02/01/2012, il primo bit della natura pensione è uguale a 3 o 4
            if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare) &&
                (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (datiPensione.NaturaPensione.Substring(0, 1) != "3" && datiPensione.NaturaPensione.Substring(0, 1) != "4")))
            {
                // Per le domande provenienti da Felpe, se non è presente la quota D, allora non si applica il comma 707
                if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && lDatiRetributivi != null && lDatiRetributivi.Count > 0 &&
                    (lDatiContributivi == null || lDatiContributivi.Count(x => x.IsQuotaDL214Presente()) == 0))
                    return false;

                if (flagContributiva == null || flagContributiva == true)
                    return false;

                return true;
            }

            return false;
        }

    }
}
