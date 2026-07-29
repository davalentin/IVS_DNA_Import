using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneCalcoloDomanda
    {
        #region public members
        public static bool CalcolaDomandaByDatiPensione(Entity.ParametriARCA parametriARCA, GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore,
            short centroOperativoOperatore, bool isVerify, bool isConsultazioniANFVerificate, bool isReingegnerizzato, out string statoPensione, out int certificato, out string chiavePensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni, out string transactionId, out string flagIndebito, out string messaggioVideo, out bool isCodiceEsito9)
        {
            isCodiceEsito9 = false;
            statoPensione = string.Empty;
            listaConsultazioniANF = null;
            listaPrenotazioneElaborazioni = null;
            certificato = 0;
            chiavePensione = string.Empty;
            messaggioVideo = string.Empty;
            string errore = string.Empty;
            bool effettuaCalcolo = true;
            transactionId = null;
            flagIndebito = null;

            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamico);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamico.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamico);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamico.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            string nomeControlloDinamico = "BloccoCalcolo" + tipoAppartenenza.GetValueOrDefault().ToString();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControlloDinamico, out controlloDinamico);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(tipoAppartenenza);

            //Per le Ric e Trf rinnovate non effettuo il blocco
            if (!((Utility.IsRicostituzione(datiPensione.Gruppo) || isRiaperturaDomanda) && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                 && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno)))
            {
                if (controlloDinamico != null && controlloDinamico.ValoreControllo == "SI")
                {
                    messaggioVideo = "Calcolo al momento non disponibile";
                    return false;
                }
            }

            if (datiPensione == null)
            {
                messaggioVideo = "Domanda non presente";
                return false;
            }

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if ((tipoAppartenenza == Utility.TipoAppartenenza.AGO || Utility.IsDomandaINPDAP(datiPensione.Gestione)) && datiPensione.FlagVerify.HasValue && !datiPensione.FlagVerify.Value)
            {
                if (!GestioneDatiPensioni.IsDomandaConPensioneLiquidata(datiPensione, isRiaperturaDomanda, out messaggioVideo))
                {
                    if (!string.IsNullOrEmpty(messaggioVideo))
                        return false;
                }
                else
                {
                    if (!GestioneStampeWeb.IsDomandaConStampaGenerata(datiPensione, out messaggioVideo))
                    {
                        if (!string.IsNullOrEmpty(messaggioVideo))
                            return false;
                        else
                        {
                            messaggioVideo = "Errore in fase di invio al calcolo. Riprovare più tardi";
                            return false;
                        }
                    }
                    effettuaCalcolo = false;

                    if (isVerify)
                    {
                        messaggioVideo = "La domanda risulta già liquidata. Effettuare il calcolo DEFINITIVO.";
                        return false;
                    }
                }
            }

            //aggiornamento FlagVerify
            if (!datiPensione.FlagVerify.HasValue || datiPensione.FlagVerify.Value != isVerify || !isVerify)
            {
                datiPensione.FlagVerify = isVerify;
                if (!isVerify && effettuaCalcolo)
                    datiPensione.DataTentativoCalcoloDefinitivo = DateTime.Now.Date;
                GestionePensione.SalvaPensione(datiPensione);
            }
            string codCat = datiPensione.GetCodCategoria();

            if (!AggiornaAttivitaWebDomPrimaDelCalcolo(datiPensione, matricolaOperatore, sedeOperatore, ref messaggioVideo))
                return false;

            if (!isVerify && tipoDomanda != Utility.TipoDomanda.Ricostituzione && !Utility.IsRiaperturaDomanda(datiPensione.Id))
            {
                GestionePagamento.DatiPagamento datiPagamento = null;
                GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamento);
                if (!GestioneUfficiPagatori.ValidaUfficioPagatore(datiPagamento, out messaggioVideo))
                    return false;
            }

            if (tipoAppartenenza == Utility.TipoAppartenenza.AGO && datiPensione.CodiceSedeDestinazione.HasValue &&
                (datiPensione.CodiceSede != datiPensione.CodiceSedeDestinazione || datiPensione.CentroOperativo != datiPensione.CentroOperativoDestinazione))
            {
                GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

                if (datiNuoveLiquidate == null || !datiNuoveLiquidate.CodiceProcessoDestinazione.HasValue)
                {
                    byte? codUnitaProcesso = null;
                    if (!GestioneWebDom.GetCodUnitaProcesso(datiPensione.CodiceSedeDestinazione, datiPensione.CentroOperativoDestinazione, datiPensione.Gestione, out codUnitaProcesso, out messaggioVideo))
                        return false;

                    if (datiNuoveLiquidate == null)
                    {
                        datiNuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();
                        datiNuoveLiquidate.IdPensione = datiPensione.Id;
                    }

                    datiNuoveLiquidate.CodiceProcessoDestinazione = codUnitaProcesso;
                    GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidate);
                }
            }

            if (Utility.IsPensioniOvunqueAttiva(tipoAppartenenza) && Utility.isRicostituzioneOrRiaperturaPolarizzata(datiPensione, isRiaperturaDomanda))
            {
                GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidate);

                if (datiNuoveLiquidate == null || !datiNuoveLiquidate.CodiceProcessoGP1ALZ6.HasValue)
                {
                    byte? codUnitaProcessoGP1ALZ6 = null;
                    if (!GestioneWebDom.GetCodUnitaProcessoGP1ALZ6(datiPensione.CodiceSedeGP1ALZ6, datiPensione.CentroOperativoGP1ALZ6, datiPensione.Gestione, out codUnitaProcessoGP1ALZ6, out messaggioVideo))
                        return false;

                    if (datiNuoveLiquidate == null)
                    {
                        datiNuoveLiquidate = new GestioneNuoveLiquidate.NuoveLiquidate();
                        datiNuoveLiquidate.IdPensione = datiPensione.Id;
                    }

                    datiNuoveLiquidate.CodiceProcessoGP1ALZ6 = codUnitaProcessoGP1ALZ6;
                    GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidate);
                }
            }

            // Per le domande che hanno generato il fascicolo verifico se su ARCA è stato già scritto un altro fascicolo
            //ENG - Spacchettate SOPGI
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            if ((Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa) ||
                Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda)) && tipoDomanda != Utility.TipoDomanda.Ricostituzione && !isRiaperturaDomanda)
            {

                if (datiDanteCausa != null && datiDanteCausa.IsFascicoloGenerato.GetValueOrDefault())
                {
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiDA = null;
                    GestioneAnagrafica.GetAnagraficaByIdAnagrafica(datiDanteCausa.IdAnagrafica, out datiAnagraficiDA);

                    GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                    richiestaArca.Applicazione = parametriARCA.Applicazione;
                    richiestaArca.Matricola = parametriARCA.Matricola;
                    richiestaArca.Provenienza = parametriARCA.Provenienza;
                    richiestaArca.Ruolo = parametriARCA.Ruolo;
                    richiestaArca.CodiceFiscaleRichiedente = datiAnagraficiDA.CodiceFiscale;
                    richiestaArca.CodiceFiscale = datiAnagraficiDA.CodiceFiscale;

                    Entity.Anagrafica anagraficaDA = null;
                    List<Entity.Pensione> elencoPensioniDA = null;
                    if (!GestioneARCA.GetAreaArcaByCodiceFiscale(richiestaArca, datiPensione.NDomus.ToString(), out anagraficaDA, out elencoPensioniDA, out messaggioVideo) ||
                        !string.IsNullOrEmpty(messaggioVideo))
                        return false;

                    Entity.Pensione pensioneFascicolo = null;
                    if (elencoPensioniDA != null && elencoPensioniDA.Count > 0)
                        pensioneFascicolo = elencoPensioniDA.Find(x => x.Categoria.Trim() == datiPensione.SiglaCategoria.Trim() && x.TipoComponente.GetValueOrDefault() == 'P');

                    if (pensioneFascicolo != null)
                    {
                        messaggioVideo = "IL CODICE FASCICOLO ASSOCIATO ALLA DOMANDA NON È CORRETTO. CANCELLARE E RIACQUISIRE NUOVAMENTE LA DOMANDA IN MODO CHE VENGA ASSOCIATO QUELLO ESATTO.";
                        return false;
                    }
                }
            }

            //ENG - Memo 121_2023            
            GestioneControlliDinamici.ControlloDinamico ctrlMemo121_2023 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("Abilitazione_Memo_121_2023", out ctrlMemo121_2023);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

            //ENG - Memo 123/2024
            GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

            if ((ctrlMemo121_2023 != null && !String.IsNullOrEmpty(ctrlMemo121_2023.ValoreControllo) && ctrlMemo121_2023.ValoreControllo.Trim().ToUpperInvariant() == "SI"
                && (Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione))
                && (!Utility.IsDomandaAutomatica(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione))) ||
                (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && ((ctrlMemo123_2024 != null && !String.IsNullOrEmpty(ctrlMemo123_2024.ValoreControllo) && ctrlMemo123_2024.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) ||
                (ctrlMemo123_2024OpzioneContrib != null && !String.IsNullOrEmpty(ctrlMemo123_2024OpzioneContrib.ValoreControllo) && ctrlMemo123_2024OpzioneContrib.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                 Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))
                && (!Utility.IsDomandaAutomatica(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione))) ||
                (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && (Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))
                && (!Utility.IsDomandaAutomatica(datiPensione) || Utility.IsDomandaENPALS(datiPensione.Gestione))))
            {
                List<GestioneOneri.DatiOneri> listaDatiOneri = null;
                GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out listaDatiOneri);

                GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);

                if (listaDatiOneri != null && listaDatiOneri.Count() > 0)
                {
                    DateTime? cessazioneIncumulabilita = Utility.CalcolaCessazioneIncumulabilita(datiPensione, datiAnagraficiTitolare, datiPensione.DataPerfezionamentoRequisiti);
                    if (cessazioneIncumulabilita.HasValue)
                    {
                        foreach (GestioneOneri.DatiOneri onere in listaDatiOneri)
                        {
                            if (!onere.ScadenzaBeneficio.HasValue || onere.ScadenzaBeneficio != cessazioneIncumulabilita)
                            {
                                GestioneOneri.EliminaOneriByIdPensione(datiPensione.Id);
                                onere.ScadenzaBeneficio = cessazioneIncumulabilita;
                                GestioneOneri.SalvaOneriOnere(onere);
                            }
                        }


                    }
                }

            }

            //per le automatizzate di supplemento se l'invio al calcolo definitivo avviene più di 7 giorni dopo l'acquisizione bloccare 
            if (!isVerify && datiPensione.TipoAutomazione == 1 &&
               (datiPensione.DataAcquisizioneIVS.HasValue && DateTime.UtcNow.Subtract(datiPensione.DataAcquisizioneIVS.Value).Days > 7))
            {
                messaggioVideo = "Domanda di supplemento automatizzata per la quale la data di prelievo dei dati pensione è superiore ai 7 giorni. La domanda dovrà essere cancellata e lavorata in modo manuale dalla sede";
                return false;
            }

            bool isPrimoInvioAlCalcolo = !datiPensione.DataElaborazione.HasValue;
            if (tipoAppartenenza.HasValue)
            {
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneNuovoFlusso = null;
                bool isNuovoCalcolo = false;
                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.FS:
                        ServiceReferences.LiquidazioneFs.AreaEsito AreaEsitoFs = null;
                        //Visto che gestiamo solo GDP al momento, mentto il controllo solo per GDP
                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                        {
                            GestioneNuovoCalcolo.FlowConf confDomanda;
                            isNuovoCalcolo = Utility.IsNuovoCalcolo(datiPensione, isVerify, out confDomanda);                         
                        }
                        datiPensione.IsNuovoCalcolo = isNuovoCalcolo;

                        CalcolaDomandaFs(datiPensione.NDomus, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isConsultazioniANFVerificate, isReingegnerizzato, isNuovoCalcolo, out statoPensione, out listaConsultazioniANF, out certificato, out AreaEsitoFs);
                        ///// Questa Get viene eseguita per aggiornare i dati pensione dopo le modifiche apportate durante il calcolo
                        GestionePensione.GetPensioneByNumeroDomandaAndProg(datiPensione.NDomus, null, out datiPensione);
                        BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                        messaggioVideo = AreaEsitoFs.Messaggio;

                        string nomeControllo = Utility.IsDomandaINPDAP(datiPensione.Gestione) ? "FlussoNuovoIndebitiGdp" : "FlussoNuovoIndebitiFs";
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo(nomeControllo, out ctrlAbilitazioneNuovoFlusso);

                        if (AreaEsitoFs.RisultatoOperazione == INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.OK && 
                            (ctrlAbilitazioneNuovoFlusso != null && !String.IsNullOrEmpty(ctrlAbilitazioneNuovoFlusso.ValoreControllo) && !String.IsNullOrEmpty(ctrlAbilitazioneNuovoFlusso.ValoreControllo.Trim()) &&
                                ctrlAbilitazioneNuovoFlusso.ValoreControllo == "SI") && 
                            !isVerify)
                        {
                            if (Utility.IsDomandaINPDAP(datiPensione.Gestione) && !String.IsNullOrEmpty(AreaEsitoFs.Messaggio)
                                && AreaEsitoFs.Messaggio.Length > 0)
                            {
                                var ultimoCarattere = AreaEsitoFs.Messaggio.Substring(AreaEsitoFs.Messaggio.Length - 1);
                                if (char.IsLetter(ultimoCarattere[0]))
                                    datiPensione.FlagIndebito = ultimoCarattere;
                            }
                            GestioneStatoDomanda_Indebiti(datiPensione.FlagIndebito, ref statoPensione);
                        }

                        if (AreaEsitoFs.RisultatoOperazione == INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO)
                        {
                            AggiornaAttivitaWebDomDopoIlCalcoloByDatiPensione(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, ref statoPensione, ref messaggioVideo, out listaPrenotazioneElaborazioni, out isCodiceEsito9);
                            GeneraStampaDopoCalcoloByDatiPensione(datiPensione, statoPensione, tipoAppartenenza, ref messaggioVideo);
                            return false;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        ServiceReferences.LiquidazioneCi.AreaEsito AreaEsitoCi = null;
                        certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                        CalcolaDomandaCi(datiPensione.NDomus, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isConsultazioniANFVerificate, out statoPensione, out listaConsultazioniANF, out AreaEsitoCi);
                        ///// Questa Get viene eseguita per aggiornare i dati pensione dopo le modifiche apportate durante il calcolo
                        GestionePensione.GetPensioneByNumeroDomandaAndProg(datiPensione.NDomus, null, out datiPensione);
                        BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                        messaggioVideo = AreaEsitoCi.Messaggio;

                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("FlussoNuovoIndebitiCi", out ctrlAbilitazioneNuovoFlusso);

                        if (AreaEsitoCi.RisultatoOperazione == INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.OK && 
                            (ctrlAbilitazioneNuovoFlusso != null && !String.IsNullOrEmpty(ctrlAbilitazioneNuovoFlusso.ValoreControllo) && !String.IsNullOrEmpty(ctrlAbilitazioneNuovoFlusso.ValoreControllo.Trim()) &&
                                ctrlAbilitazioneNuovoFlusso.ValoreControllo == "SI")
                            && !isVerify)
                            GestioneStatoDomanda_Indebiti(datiPensione.FlagIndebito, ref statoPensione);

                        if (AreaEsitoCi.RisultatoOperazione == INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO)
                        {
                            AggiornaAttivitaWebDomDopoIlCalcoloByDatiPensione(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, ref statoPensione, ref messaggioVideo, out listaPrenotazioneElaborazioni, out isCodiceEsito9);
                            GeneraStampaDopoCalcoloByDatiPensione(datiPensione, statoPensione, tipoAppartenenza, ref messaggioVideo);
                            return false;
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        ServiceReferences.LiquidazioneAgo.AreaEsito AreaEsitoAgo = null;
                        certificato = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value : 0;
                        if (effettuaCalcolo)
                        {
                            GestioneNuovoCalcolo.FlowConf confDomanda;
                            isNuovoCalcolo = Utility.IsNuovoCalcolo(datiPensione, isVerify, out confDomanda);
                            datiPensione.IsNuovoCalcolo = isNuovoCalcolo;
                            GestionePensione.SalvaPensione(datiPensione);
                            CalcolaDomandaAgo(datiPensione.NDomus, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isConsultazioniANFVerificate, isNuovoCalcolo, out statoPensione, out listaConsultazioniANF, out AreaEsitoAgo, out transactionId);
                            ///// Questa Get viene eseguita per aggiornare i dati pensione dopo le modifiche apportate durante il calcolo
                            GestionePensione.GetPensioneByNumeroDomandaAndProg(datiPensione.NDomus, null, out datiPensione);
                            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                            messaggioVideo = AreaEsitoAgo.Messaggio;

                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("FlussoNuovoIndebitiAgo", out ctrlAbilitazioneNuovoFlusso);

                            if (AreaEsitoAgo.RisultatoOperazione == INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.OK && 
                                (ctrlAbilitazioneNuovoFlusso != null && !String.IsNullOrEmpty(ctrlAbilitazioneNuovoFlusso.ValoreControllo) && !String.IsNullOrEmpty(ctrlAbilitazioneNuovoFlusso.ValoreControllo.Trim()) &&
                                    ctrlAbilitazioneNuovoFlusso.ValoreControllo == "SI")
                                && !isVerify)
                                GestioneStatoDomanda_Indebiti(datiPensione.FlagIndebito, ref statoPensione);

                            if (AreaEsitoAgo.RisultatoOperazione == INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO)
                            {
                                AggiornaAttivitaWebDomDopoIlCalcoloByDatiPensione(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, ref statoPensione, ref messaggioVideo, out listaPrenotazioneElaborazioni, out isCodiceEsito9);
                                GeneraStampaDopoCalcoloByDatiPensione(datiPensione, statoPensione, tipoAppartenenza, ref messaggioVideo);
                                return false;
                            }
                            if (listaConsultazioniANF != null && listaConsultazioniANF.Count > 0)
                                return true;
                        }
                        else
                        {
                            messaggioVideo = "Calcolo eseguito correttamente";
                            datiPensione.StatoPensione = (byte)Utility.StatoPensione.CalcolataNoWebDom;
                            GestionePensione.SalvaPensione(datiPensione);
                            GestioneDecodifica.GetStatoPensioneById(datiPensione.StatoPensione.Value, out statoPensione);
                        }
                        break;
                    default:
                        messaggioVideo = "Tipo appartenenza domanda non gestito";
                        return false;
                }
            }
            else
            {
                messaggioVideo = "Tipo appartenenza della domanda non individuato";
                return false;
            }

            Utility.StatoPensione? stato = Utility.GetStatoPensioneByDescrizione(statoPensione);
            if (stato.HasValue)
            {
                if (stato.Value == Utility.StatoPensione.CalcolataNoWebDom || stato.Value == Utility.StatoPensione.CalcolataNoStazLavoro)
                {
                    chiavePensione = (codCat.Length > 3 ? codCat.Substring(codCat.Length - 3) : codCat.PadLeft(3, '0')) + "-" +
                        (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + "-" +
                        certificato.ToString().PadLeft(8, '0');

                    if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    {
                        if (Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione))
                        {
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria != null && datiIstruttoria.CodiceAziendaEditoria.HasValue)
                                GestioneAnagraficaAccordi.UpdateCountLiquidate_AnagraficaAccordi(datiIstruttoria.CodiceAziendaEditoria);
                        }
                        else if (Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione))
                        {
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria != null && datiIstruttoria.CodiceAziendaEditoriaLetteraB.HasValue)
                                GestioneAnagraficaAccordiLetteraB.UpdateCountLiquidate_AnagraficaAccordi(datiIstruttoria.CodiceAziendaEditoriaLetteraB);
                        }
                        else if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione))
                        {
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria.CodiceAziendaEditoriaPerTipo0171.HasValue)
                                GestioneAnagraficaAccordiPerTipo0171.UpdateCountLiquidate_AnagraficaAccordi(datiIstruttoria.CodiceAziendaEditoriaPerTipo0171);
                        }
                    }
                }

                if (stato.Value == Utility.StatoPensione.CalcoloVerify && Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) &&
                    !Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda) && isPrimoInvioAlCalcolo)
                {
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                    if (datiIstruttoria.CodiceAziendaEditoriaPerTipo0179.HasValue)
                        GestioneAnagraficaAccordiPerTipo0179.UpdateCountLiquidate_AnagraficaAccordi(datiIstruttoria.CodiceAziendaEditoriaPerTipo0179, true);
                }

                if (stato.Value == Utility.StatoPensione.CalcolataNoStazLavoro && tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI)
                {
                    //Chiudo l'attività 00121 (in Liquidazione) e apro l'attività 00110 (Attesa Calcolo)
                    GestioneWebDom.AggiornamentoFaseAttivita(datiPensione, matricolaOperatore, sedeOperatore, out errore);
                    if (!string.IsNullOrEmpty(errore))
                        return false;
                    if (!GestioneAllegatiConvenzioni.AggiornaCI05ByNumeroDomanda(datiPensione, matricolaOperatore, sedeOperatore, centroOperativoOperatore, out errore))
                    {
                        datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoStazLavoro;
                        GestionePensione.SalvaPensione(datiPensione);
                        statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoStazLavoro);
                        messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;

                        //if (!GestioneAreaStampa.CancelStampaByIdPensione(datiPensione.Id, out errore))
                        //    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;

                        return false;
                    }
                    else
                    {
                        datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                        GestionePensione.SalvaPensione(datiPensione);
                        statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom);
                    }
                }
            }

            if (!(stato.HasValue && (stato.Value == Utility.StatoPensione.CalcoloNoIndeb || stato.Value == Utility.StatoPensione.CalcoloNoIndebWait)))
            {
                if (!AggiornaAttivitaWebDomDopoIlCalcoloByDatiPensione(datiPensione, datiDanteCausa, matricolaOperatore, sedeOperatore, ref statoPensione, ref messaggioVideo, out listaPrenotazioneElaborazioni, out isCodiceEsito9))
                    return false;
                //if (!GestioneAreaStampa.CancelStampaByIdPensione(datiPensione.Id, out errore))
                //{
                //    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                //    return false;
                //}
            }

            //nel caso di domanda calcolata in definitiva con aggiornamento WebDom OK aggiorno lo statoPensione a Calcolata
            if (datiPensione.StatoPensione == (int)Utility.StatoPensione.CalcolataNoWebDom)
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);
            }

            //indebiti
            if (stato.HasValue && !Utility.IsNullOrWhiteSpace(datiPensione.FlagIndebito))
            {
                flagIndebito = datiPensione.FlagIndebito.Trim();

                if (stato.Value == Utility.StatoPensione.CalcoloNoIndeb || stato.Value == Utility.StatoPensione.CalcoloNoIndebWait)
                {
                    statoPensione = Utility.GetDescription(stato);

                    datiPensione.StatoPensione = (byte)stato.Value;
                    GestionePensione.SalvaPensione(datiPensione);
                }
            }

            if (!GeneraStampaDopoCalcoloByDatiPensione(datiPensione, statoPensione, tipoAppartenenza, ref messaggioVideo))
                return false;

            return true;
        }

        public static bool AggiornaFelpeDopoWebDom(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneAggiornamentoPECO.AggiornaPECO(datiPensione, matricolaOperatore, sedeOperatore, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaOneriDopoFelpe(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneOneriPrepensionamento.AggiornaOneri(datiPensione, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaSaiDopoOneri(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, GestioneSAI.GetTipoRichiestaPAG(datiPensione), out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaINPDAPDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneINPDAP.AggiornaINPDAP(datiPensione, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaNoteDiDebitoDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaPianiDiPagamentoDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo, out bool isCodiceEsito9)
        {
            string errore = string.Empty;
            isCodiceEsito9 = false;
            GestioneINPDAP.AggiornaPianiDiPagamento(datiPensione, out errore, out isCodiceEsito9);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaEquoIndDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneINPDAP.AggiornaEquoIndennizzo(datiPensione, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaIndennitaSpecialeDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo, out bool isCodiceEsito9)
        {
            string errore = string.Empty;
            GestioneINPDAP.AggiornaIndennitaSpeciale(datiPensione, out errore, out isCodiceEsito9);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaPensCumuloDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneTotalIvs.AggiornaCumulo(datiPensione, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaPensTotDopoOneri(GestionePensione.DatiPensione datiPensione, ref string messaggioVideo)
        {
            string errore = string.Empty;
            GestioneTotalIvs.AggiornaTot(datiPensione, out errore);
            if (!string.IsNullOrEmpty(errore))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                return false;
            }

            return true;
        }

        public static bool AggiornaPrenotazioneElaborazioniDopoOneri(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, ref string messaggioVideo, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni)
        {
            List<GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus> datiAnniRichiestaBonus = null;
            GestioneRichiestaBonus.AreaRichiestaBonus richiestaBonus = new GestioneRichiestaBonus.AreaRichiestaBonus();
            listaPrenotazioneElaborazioni = new List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni>();

            //ENG - Booking FS-AGO
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            GestioneAnniRichiestaBonus.GetAnniRichiestaBonus(datiPensione.Id, out datiAnniRichiestaBonus);
            richiestaBonus.Certificato = datiPensione.NCertificato.Value.ToString().PadLeft(8, '0');
            richiestaBonus.Categoria = datiPensione.GetCodCategoria().Substring(1, 3);
            if ((tipoAppartenenza == Utility.TipoAppartenenza.FS || tipoAppartenenza == Utility.TipoAppartenenza.AGO) && Utility.IsRicostituzione_Reddituale(datiPensione) && datiPensione.CodiceSedeDestinazione.HasValue)
                richiestaBonus.Sede = datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0');
            else
                richiestaBonus.Sede = datiPensione.CodiceSede.ToString().PadLeft(4, '0');
            int[] listAnniRichiestaBonus = datiAnniRichiestaBonus.Where(x => x.IsRichiestaBonus == true).Select(x => x.Anno).ToArray();
            string anniRichiestaBonus = string.Empty;
            for (int i = 0; i < listAnniRichiestaBonus.Length; i++)
            {
                anniRichiestaBonus += listAnniRichiestaBonus[i];
                if (i < listAnniRichiestaBonus.Length - 1)
                    anniRichiestaBonus += '|';
            }
            richiestaBonus.Anni = anniRichiestaBonus;
            if (datiPensione.Tipo == "0167")
            {
                richiestaBonus.TipoBonus = "B14_I";
            }
            else
            {
                richiestaBonus.TipoBonus = "B154_I";
            }
            richiestaBonus.NumDomanda = datiPensione.NDomus.ToString();
            if (!GestioneRichiestaBonus.GetPrenotazioneElaborazioni(ref richiestaBonus, matricolaOperatore, sedeOperatore.ToString(), datiPensione.Id))
            {
                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? richiestaBonus.MessaggioVideo : messaggioVideo + " - " + richiestaBonus.MessaggioVideo;
                return false;
            }
            GestioneAnniRichiestaBonus.SalvaPrenotazioneElaborazioni(datiPensione.Id, richiestaBonus.DatiPrenotazioneElaborazioni);
            listaPrenotazioneElaborazioni = richiestaBonus.DatiPrenotazioneElaborazioni;
            GestioneQuadri.DatiQuadroRichiestaBonus quadroRichiestaBonus = null;
            GestioneQuadri.GetQuadroRichiestaBonusByDatiPensione(datiPensione, out quadroRichiestaBonus);
            quadroRichiestaBonus.TabEsitoPrenotazione = 0;
            GestioneQuadri.SalvaQuadroRichiestaBonus(datiPensione.Id, quadroRichiestaBonus);

            return true;
        }

        public static bool IsCalcoloVerify(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null)
            {
                if (datiPensione.FlagVerify.HasValue && datiPensione.FlagVerify.Value)
                    return true;
            }
            return false;
        }
        #endregion public members

        #region private members
        private static void CalcolaDomandaFs(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, bool isReingegnerizzato, bool? isNuovoCalcolo,
            out string statoPensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out int certificato,
            out ServiceReferences.LiquidazioneFs.AreaEsito Esito)
        {
            bool erroreTecnico = false;
            statoPensione = string.Empty;
            certificato = 0;
            Esito = null;
            listaConsultazioniANF = null;
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                ServiceReferences.LiquidazioneFs.ServizioLiquidazioneFsClient proxy = new ServiceReferences.LiquidazioneFs.ServizioLiquidazioneFsClient();
                try
                {
                    GestioneFamiliari.ConsultazioneUnificataANF[] listaConsultazioni = null;
                    Esito = proxy.CalcolaDomanda(out listaConsultazioni, out statoPensione, out certificato, numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isConsultazioniANFVerificate, isReingegnerizzato, isNuovoCalcolo);
                    if (listaConsultazioni != null && listaConsultazioni.Count() > 0)
                        listaConsultazioniANF = listaConsultazioni.ToList();
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(Esito.Messaggio);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Puntamento errato al servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Errore di comunicazione con il servizio LiquidazioneFs | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Errore nel consumo del servizio LiquidazioneFs: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(Esito.Messaggio);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (Esito != null && Esito.RisultatoOperazione == ServiceReferences.LiquidazioneFs.AreaEsito.TipoEsito.KO && erroreTecnico)
                    {
                        string messaggio = Esito.Messaggio;
                        Esito.Messaggio = "Errore tecnico durante le operazioni di calcolo della domanda";
                        string parametri = string.Format("Numero domanda: {0}; Matricola operatore: {1}; Sede operatore: {2}; Centro operativo operatore: {3}",
                            numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void CalcolaDomandaCi(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate,
            out string statoPensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out ServiceReferences.LiquidazioneCi.AreaEsito Esito)
        {
            bool erroreTecnico = false;
            statoPensione = string.Empty;
            Esito = null;
            listaConsultazioniANF = null;
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                ServiceReferences.LiquidazioneCi.ServizioLiquidazioneCiClient proxy = new ServiceReferences.LiquidazioneCi.ServizioLiquidazioneCiClient();
                try
                {
                    GestioneFamiliari.ConsultazioneUnificataANF[] listaConsultazioni = null;
                    Esito = proxy.CalcolaDomanda(out listaConsultazioni, out statoPensione, numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isConsultazioniANFVerificate);
                    if (listaConsultazioni != null && listaConsultazioni.Count() > 0)
                        listaConsultazioniANF = listaConsultazioni.ToList();
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(Esito.Messaggio);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Puntamento errato al servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Errore di comunicazione con il servizio LiquidazioneCi | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return;
                }
                catch (Exception Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Errore nel consumo del servizio LiquidazioneCi: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(Esito.Messaggio);
                    erroreTecnico = true;
                    return;
                }
                finally
                {
                    if (Esito != null && Esito.RisultatoOperazione == ServiceReferences.LiquidazioneCi.AreaEsito.TipoEsito.KO && erroreTecnico)
                    {
                        string messaggio = Esito.Messaggio;
                        Esito.Messaggio = "Errore tecnico durante le operazioni di calcolo della domanda";
                        string parametri = string.Format("Numero domanda: {0}; Matricola operatore: {1}; Sede operatore: {2}; Centro operativo operatore: {3}",
                            numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static void CalcolaDomandaAgo(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, bool isNuovoCalcolo,
            out string statoPensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out ServiceReferences.LiquidazioneAgo.AreaEsito Esito, out string transactionId)
        {
            bool erroreTecnico = false;
            listaConsultazioniANF = null;
            statoPensione = string.Empty;
            Esito = null;
            string stackTrace = null;
            transactionId = null;

            using (new MethodExecutionTracer())
            {
                ServiceReferences.LiquidazioneAgo.ServizioLiquidazioneAgoClient proxy = new ServiceReferences.LiquidazioneAgo.ServizioLiquidazioneAgoClient();
                try
                {

                    GestioneFamiliari.ConsultazioneUnificataANF[] listaConsultazioni = null;
                    Esito = proxy.CalcolaDomanda(out statoPensione, out listaConsultazioni, out transactionId, numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, isConsultazioniANFVerificate, isNuovoCalcolo);
                    if (listaConsultazioni != null && listaConsultazioni.Count() > 0)
                        listaConsultazioniANF = listaConsultazioni.ToList();
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    Esito = new ServiceReferences.LiquidazioneAgo.AreaEsito();
                    Esito.RisultatoOperazione = ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    Esito = new ServiceReferences.LiquidazioneAgo.AreaEsito
                    {
                        RisultatoOperazione = ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO,
                        Messaggio = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex))
                    };
                    stackTrace = Ex.StackTrace;
                    Logger.WriteError(Esito.Messaggio);
                    erroreTecnico = true;
                    return;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Puntamento errato al servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Errore di comunicazione con il servizio LiquidazioneAgo | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return;
                }
                catch (Exception Ex)
                {
                    Esito = new INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito();
                    Esito.RisultatoOperazione = INPS.Pensioni.Liquidazione.ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO;
                    Esito.Messaggio = string.Format("Errore nel consumo del servizio LiquidazioneAgo: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    erroreTecnico = true;
                    INPS.DNA.Logging.Logger.WriteError(Esito.Messaggio);
                    return;
                }
                finally
                {
                    if (Esito != null && Esito.RisultatoOperazione == ServiceReferences.LiquidazioneAgo.AreaEsito.TipoEsito.KO && erroreTecnico)
                    {
                        string messaggio = Esito.Messaggio;
                        Esito.Messaggio = "Errore tecnico durante le operazioni di calcolo della domanda";
                        string parametri = string.Format("Numero domanda: {0}; Matricola operatore: {1}; Sede operatore: {2}; Centro operativo operatore: {3}",
                            numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
        }

        private static bool AggiornaAttivitaWebDomPrimaDelCalcolo(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, ref string messaggioVideo)
        {
            //gestione attività WebDom
            bool IsGestioneAttivita = true;
            if (ConfigurationManager.AppSettings["BypassAttivitaWebDom"] != null &&
                    ConfigurationManager.AppSettings["BypassAttivitaWebDom"] == "SI")
            {
                IsGestioneAttivita = false;
            }

            if (IsGestioneAttivita)
            {
                string errore = string.Empty;
                GestioneWebDom.ChiusuraAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.CalcoloErrato, out errore);
                if (!string.IsNullOrEmpty(errore))
                {
                    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                    return false;
                }
            }

            return true;
        }

        private static bool AggiornaAttivitaWebDomDopoIlCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string matricolaOperatore, short sedeOperatore,
            ref string statoPensione, ref string messaggioVideo, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni, out bool isCodiceEsito9)
        {
            //gestione attività WebDom
            isCodiceEsito9 = false;
            bool IsGestioneAttivita = true;
            listaPrenotazioneElaborazioni = null;
            if (ConfigurationManager.AppSettings["BypassAttivitaWebDom"] != null &&
                    ConfigurationManager.AppSettings["BypassAttivitaWebDom"] == "SI")
            {
                IsGestioneAttivita = false;
            }

            if (IsGestioneAttivita)
            {
                string errore = string.Empty;

                if (datiPensione == null)
                {
                    messaggioVideo = "Domanda non presente";
                    return false;
                }

                if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                    datiPensione.StatoPensione.Value != (int)Utility.StatoPensione.DaCalcolare)
                {
                    if (datiPensione != null && datiPensione.StatoPensione.HasValue)
                    {
                        switch (datiPensione.StatoPensione.Value)
                        {
                            //in caso di calcolata esatta si utilizza il metodo WebDom AggiornaFaseAttività
                            //che si occupa in automatico di chiudere l'attività attesaCalcolo, aprire e chiudere l'attività calcoloEsatto e di aggiornare la situazione a 22
                            //per le AGO l'aggiornamento WebDom in caso di calcolo definitivo è a carico dello stesso calcolo
                            case (int)Utility.StatoPensione.CalcolataNoWebDom:
                                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                ////////SE IL CONTROLLO E' IMPOSTATO A 'SI' ALLORA L'AGGIORNAMENTO WEBDOM VIENE ESEGUITO DAL CALCOLO//////////////////
                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AggiornaWebDomAGOCalcoloCentrale", out ctrl);
                                if (ctrl != null && ctrl.ValoreControllo == "SI")
                                {
                                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
                                    {
                                        //ENG - ENPALS: TRF MANUALI PRECOCI
                                        if (!(Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && Utility.IsDomandaENPALS(datiPensione.Gestione) && Utility.IsDomandaAPEPrecoci(datiPensione)))
                                        {
                                            if (!AggiornaFelpeDopoWebDom(datiPensione, matricolaOperatore, sedeOperatore, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoFelpe;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoFelpe);
                                                return false;
                                            }
                                        }
                                        if (!AggiornaOneriDopoFelpe(datiPensione, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoOneri;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri);
                                            return false;
                                        }
                                        if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
                                        {
                                            if (!AggiornaSaiDopoOneri(datiPensione, datiDanteCausa, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSAI;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                                                return false;
                                            }
                                        }
                                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        {
                                            if (!AggiornaINPDAPDopoOneri(datiPensione, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSIN;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSIN);
                                                return false;
                                            }
                                            if (!AggiornaNoteDiDebitoDopoOneri(datiPensione, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                                                return false;
                                            }
                                            if (!AggiornaPianiDiPagamentoDopoOneri(datiPensione, ref messaggioVideo, out isCodiceEsito9))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNo6Scatti;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNo6Scatti);
                                                return false;
                                            }
                                            if (!AggiornaEquoIndDopoOneri(datiPensione, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                                                return false;
                                            }
                                            if (!AggiornaIndennitaSpecialeDopoOneri(datiPensione, ref messaggioVideo, out isCodiceEsito9))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                                                return false;
                                            }
                                        }
                                        if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault())
                                        {
                                            if (!AggiornaPensCumuloDopoOneri(datiPensione, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTotal;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal);
                                                return false;
                                            }
                                        }

                                        if (Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && datiPensione.IsTotAutomatica.GetValueOrDefault())
                                        {
                                            if (!AggiornaPensTotDopoOneri(datiPensione, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTot;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTot);
                                                return false;
                                            }
                                        }
                                        if (Utility.IsDomandaMiglioramentiContrattuali(datiPensione))
                                        {
                                            if (!AggiornaNoteDiDebitoDopoOneri(datiPensione, ref messaggioVideo))
                                            {
                                                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                                                GestionePensione.SalvaPensione(datiPensione);
                                                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                                                return false;
                                            }
                                        }

                                        datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
                                        GestionePensione.SalvaPensione(datiPensione);
                                        statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);
                                        break;
                                    }
                                }
                                GestioneWebDom.AggiornamentoFaseAttivita(datiPensione, matricolaOperatore, sedeOperatore, out errore);
                                if (!string.IsNullOrEmpty(errore))
                                {
                                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoWebDom;
                                    GestionePensione.SalvaPensione(datiPensione);
                                    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom);
                                    return false;
                                }
                                else
                                {
                                    //ENG - ENPALS: TRF MANUALI PRECOCI
                                    if (!(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && Utility.IsDomandaENPALS(datiPensione.Gestione) && Utility.IsDomandaAPEPrecoci(datiPensione)))
                                    {
                                        if (!AggiornaFelpeDopoWebDom(datiPensione, matricolaOperatore, sedeOperatore, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoFelpe;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoFelpe);
                                            return false;
                                        }
                                    }
                                    if (!AggiornaOneriDopoFelpe(datiPensione, ref messaggioVideo))
                                    {
                                        datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoOneri;
                                        GestionePensione.SalvaPensione(datiPensione);
                                        statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri);
                                        return false;
                                    }
                                    if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
                                    {
                                        if (!AggiornaSaiDopoOneri(datiPensione, datiDanteCausa, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSAI;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                                            return false;
                                        }
                                    }
                                    if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                    {
                                        if (!AggiornaINPDAPDopoOneri(datiPensione, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSIN;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSIN);
                                            return false;
                                        }
                                        if (!AggiornaNoteDiDebitoDopoOneri(datiPensione, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                                            return false;
                                        }
                                        if (!AggiornaPianiDiPagamentoDopoOneri(datiPensione, ref messaggioVideo, out isCodiceEsito9))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNo6Scatti;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNo6Scatti);
                                            return false;
                                        }
                                        if (!AggiornaEquoIndDopoOneri(datiPensione, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                                            return false;
                                        }
                                        if (!AggiornaIndennitaSpecialeDopoOneri(datiPensione, ref messaggioVideo, out isCodiceEsito9))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                                            return false;
                                        }
                                    }
                                    if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault())
                                    {
                                        if (!AggiornaPensCumuloDopoOneri(datiPensione, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTotal;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal);
                                            return false;
                                        }
                                    }
                                    if (Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && datiPensione.IsTotAutomatica.GetValueOrDefault())
                                    {
                                        if (!AggiornaPensTotDopoOneri(datiPensione, ref messaggioVideo))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTot;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTot);
                                            return false;
                                        }
                                    }
                                    if (!AggiornaNoteDiDebitoDopoOneri(datiPensione, ref messaggioVideo))
                                    {
                                        datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                                        GestionePensione.SalvaPensione(datiPensione);
                                        statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                                        return false;
                                    }

                                    if (datiPensione.IsRichiestaBonus.GetValueOrDefault())
                                    {
                                        if (!AggiornaPrenotazioneElaborazioniDopoOneri(datiPensione, matricolaOperatore, sedeOperatore, ref messaggioVideo, out listaPrenotazioneElaborazioni))
                                        {
                                            datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoBooking;
                                            GestionePensione.SalvaPensione(datiPensione);
                                            statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoBooking);
                                            return false;
                                        }
                                    }
                                }
                                break;
                            case (int)Utility.StatoPensione.ScartoDaCalcolo:
                                GestioneWebDom.ChiusuraAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.AttesaCalcolo, out errore);
                                if (!string.IsNullOrEmpty(errore))
                                {
                                    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                                    return false;
                                }
                                GestioneWebDom.AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.CalcoloErrato, out errore);
                                if (!string.IsNullOrEmpty(errore))
                                {
                                    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                                    return false;
                                }
                                break;
                            case (int)Utility.StatoPensione.CalcoloVerify:
                            case (int)Utility.StatoPensione.ScartoVerify:
                                break;
                        }
                    }
                }
            }
            return true;
        }

        private static bool GeneraStampaDopoCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, string statoPensione, Utility.TipoAppartenenza? tipoAppartenenza, ref string messaggioVideo)
        {
            MemoryStream msPDF = null;
            string errore = string.Empty;

            Utility.StatoPensione? stato = Utility.GetStatoPensioneByDescrizione(statoPensione);
            if (stato.HasValue)
            {
                switch (stato)
                {
                    case Utility.StatoPensione.Calcolata:
                    case Utility.StatoPensione.CalcoloVerify:
                    case Utility.StatoPensione.CalcolataNoWebDom:
                    case Utility.StatoPensione.CalcolataNoFelpe:
                    case Utility.StatoPensione.CalcolataNoOneri:
                    case Utility.StatoPensione.CalcolataNoSAI:
                    case Utility.StatoPensione.CalcolataNoStazLavoro:
                    case Utility.StatoPensione.CalcolataNoTotal:
                    case Utility.StatoPensione.CalcolataNoTot:
                    case Utility.StatoPensione.CalcolataNoSIN:
                    case Utility.StatoPensione.CalcolataNoBooking:
                    case Utility.StatoPensione.CalcolataNoNoteDebito:
                    case Utility.StatoPensione.CalcolataNo6Scatti:
                        if (!GestioneAreaStampa.CancelStampaByIdPensione(datiPensione.Id, out errore))
                        {
                            messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                            return false;
                        }
                        //if (!GestioneAreaStampa.GetStampaByDatiPensione(datiPensione, out msPDF, out errore))
                        //{
                        //    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                        //    return false;
                        //}
                        break;

                    case Utility.StatoPensione.ScartoDaCalcolo:
                    case Utility.StatoPensione.ScartoVerify:
                        if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
                        {
                            //if (!GestioneAreaStampa.CancelStampaByIdPensione(datiPensione.Id, out errore))
                            //{
                            //    messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                            //    return false;
                            //}
                            if (!GestioneAreaStampa.GetStampaByDatiPensione(datiPensione, out msPDF, out errore))
                            {
                                messaggioVideo = string.IsNullOrEmpty(messaggioVideo) ? errore : messaggioVideo + " - " + errore;
                                return false;
                            }
                        }
                        break;
                }
            }
            return true;
        }

        //Metodo per la gestione del cambio stato domanda basato sul Flag Indebito
        //Caso 0 non ho indebito ---> Passo direttamente allo stato calcolata
        //Casi R, E, S, X, Z: MOTIVO OSTEATIVO -----> Si è generato un indebito ma c'è un motivo osteativo alla produzione del TE08/Ind
        //Casi A,C,M: NO MOTIVO OSTATIVO ---------> Si è verificato un indebito NON vi è un motivo ostativo alla produzione del TE08/Ind (Stato CALCOLO NO INDB WAIT)
        //Caso I. Si è generato un Indebito ma non si cade in nessuna delle opzioni precedenti (Stato CALCOLO NO INDEB)
        private static void GestioneStatoDomanda_Indebiti(string flagIndebiti, ref string statoDomanda)
        {
            if (!String.IsNullOrEmpty(flagIndebiti))
            {
                switch (flagIndebiti.Trim())
                {
                    case "0":
                        break;
                    case "I":
                        statoDomanda = "CALCOLO NO INDEB";
                        break;
                    case "A":
                    case "C":
                    case "M":
                        statoDomanda = "CALCOLO NO INDEB WAIT";
                        break;
                    case "R":
                    case "E":
                    case "S":
                    case "X":
                    case "Z":
                        break;
                }
            }

        }

        #endregion private members
    }
}
