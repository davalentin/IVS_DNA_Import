using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using INPS.DNA.Context;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneCi
{
    public class GestionePrelievo
    {
        #region public members
        public static void PrelevaDomanda(RichiestaPrelievo richiesta, out RispostaPrelievo risposta, out string messaggioVideo)
        {
            risposta = null;
            messaggioVideo = "";

            DateTime dataSistema = Utility.DataSistemaCi;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoData = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioNuovoTracciato", out controlloDinamicoData);
            DateTime dataInizioNuovoTracciato = Utility.DataFromString(controlloDinamicoData.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            bool isNuovoTracciato = false;
            // Se è una Ric o TRF e la data sistema è compresa tra i due controlli dinamici(DataInizioInterregno e DataFineInterregno)
            // oppure se la data sistema è maggiore uguale al 01/12/2023 viene eseguito il nuovo tracciato
            if ((richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                   && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno)) ||
                Utility.DataSuccessivaA(dataSistema, dataInizioNuovoTracciato))
                isNuovoTracciato = true;

            Data.GACI AreaPrelievo = null;
            Data.GACINew AreaPrelievoNew = null;

            if (!isNuovoTracciato)
            {
                ValorizzaAreaPrelievo(richiesta, out AreaPrelievo, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Request, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.GACI, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);
                EseguiPrelievo(AreaPrelievo);

                ControllaEsitoPrelievo(AreaPrelievo, richiesta, out messaggioVideo);

                if (AreaPrelievo.HasError)
                    GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.GACI, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievo.FinalResponse, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.GACI, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

                if (!string.IsNullOrEmpty(AreaPrelievo.MessaggioDaLoggare))
                {
                    long numeroDomanda = 0;
                    long.TryParse(richiesta.NumDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievo.MessaggioDaLoggare, null, null);
                }
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
            }
            else
            {
                ValorizzaAreaPrelievoNew(richiesta, out AreaPrelievoNew, out messaggioVideo);
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.Request, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.GACI, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);
                EseguiPrelievoNew(AreaPrelievoNew);

                ControllaEsitoPrelievoNew(AreaPrelievoNew, richiesta, out messaggioVideo);

                if (AreaPrelievoNew.HasError)
                    GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.GACI, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
                else
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.FinalResponse, Utility.Servizio.SrvLiquidazioneCi, Utility.MetodoServizio.GACI, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

                if (!string.IsNullOrEmpty(AreaPrelievoNew.MessaggioDaLoggare))
                {
                    long numeroDomanda = 0;
                    long.TryParse(richiesta.NumDomanda, out numeroDomanda);
                    GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievoNew.MessaggioDaLoggare, null, null);
                }
                if (!String.IsNullOrEmpty(messaggioVideo))
                    return;
            }

            if (!isNuovoTracciato)
                NormalizzaAreaToDB(AreaPrelievo, richiesta, out risposta);
            else
                NormalizzaAreaToDBNew(AreaPrelievoNew, richiesta, out risposta);
        }
        #endregion public members

        #region private members
        private static void ValorizzaAreaPrelievo(RichiestaPrelievo richiesta, out Data.GACI AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            if (richiesta.Categoria == 0 || richiesta.SedeOperatore == 0 || richiesta.Sede == 0 ||
                richiesta.Certificato == 0)
            {
                messaggioVideo = "Area richiesta non valorizzata correttamente";
                return;
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(richiesta.SedeOperatore.ToString().PadLeft(4, '0') + richiesta.CentroOperativoOperatore.ToString().PadLeft(2, '0'));

            //RINNOVO
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            DateTime dataSistema = Utility.DataSistemaCi;
            int annoCompetenza = 0;
            int annoComp = 0;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoCI", out ctrl);

            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.CI, out annoComp);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo l'anno a 0
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                    annoCompetenza = dataSistema.Year + 1;
                else
                    annoCompetenza = annoComp;
            }

            AreaPrelievo = new INPS.Pensioni.LiquidazioneCi.Data.GACI(richiesta.Sede.ToString().PadLeft(4, '0'), richiesta.Categoria.ToString().PadLeft(3, '0'),
                richiesta.Certificato.ToString().PadLeft(8, '0'), richiesta.CodiceAf, richiesta.CodiceAs, richiesta.AltriDati, annoCompetenza);

            AreaPrelievo.IsRic = richiesta.TipoDomanda == TipoDomanda.Ricostituzione ? true : false;
        }

        private static void EseguiPrelievo(Data.GACI AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void ControllaEsitoPrelievo(Data.GACI AreaPrelievo, RichiestaPrelievo richiesta, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (AreaPrelievo.HasError)
            {
                if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                    messaggioVideo = AreaPrelievo.Messaggio;
                else
                {
                    if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null && AreaPrelievo.FinalResponse.Gruppo1.AreaTP11 != null && AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CO == 0)
                    {
                        GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
                        GestioneControlliDinamici.ControlloDinamico ctrl = null;
                        DateTime dataSistema = Utility.DataSistemaCi;

                        //RINNOVO
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
                        DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
                        DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoCI", out ctrl);

                        if (ctrl != null && ctrl.ValoreControllo == "SI" && richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                            && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                            messaggioVideo = "RIC-RIN: OPERAZIONE NON CONSENTITA PER MANCANZA DATI RINNOVATI";
                        else
                            messaggioVideo = "Nessuna pensione presente";

                    }
                }
            }
        }

        private static void NormalizzaAreaToDB(Data.GACI AreaPrelievo, RichiestaPrelievo richiesta, out RispostaPrelievo risposta)
        {
            risposta = new RispostaPrelievo();
            risposta.CodiceFiscale = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12.TP1COFI;
            TipoDomanda tipoDomanda = richiesta.TipoDomanda;
            bool isRiaperturaDomanda = richiesta.IsRiaperturaDomanda;
            List<GestioneDecodifica.StatoEstero> listaDecStatiEsteri = null;

            if (AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEOSEC != 0 &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEORM != 0)
                risposta.DataDecorrenza = new DateTime(int.Parse(AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEOSEC.ToString().PadLeft(2, '0') +
                    AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEOAA.ToString().PadLeft(2, '0')),
                    (int)AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEORM, 1);

            if (AreaPrelievo.FinalResponse.Gruppo1.AreaTP11 != null)
            {
                if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CITT1))
                {
                    if (listaDecStatiEsteri == null)
                        GestioneDecodifica.GetStatiEsteri(out listaDecStatiEsteri);

                    if (listaDecStatiEsteri != null && listaDecStatiEsteri.Count > 0)
                    {
                        string app = AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CITT1 == "I" ? "ITA" : AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CITT1;
                        GestioneDecodifica.StatoEstero statoEstero = listaDecStatiEsteri.Find(x => x.Sigla == app);
                        if (statoEstero != null)
                        {
                            risposta.Cittadinanza = !string.IsNullOrEmpty(statoEstero.CodCatastale) ? statoEstero.CodCatastale.Trim() : string.Empty;
                        }
                    }
                }
            }

            #region datiPensione
            GestionePensione.DatiPensione datiPensione = null;
            MappingDaHost.ValorizzaDatiPensione(AreaPrelievo, tipoDomanda, isRiaperturaDomanda, richiesta.Categoria, out datiPensione);
            risposta.DatiPensione = datiPensione;
            #endregion datiPensione

            #region datiSindacato
            GestionePensione.DatiSindacato datiSindacato = null;
            MappingDaHost.ValorizzaDatiSindacato(AreaPrelievo, out datiSindacato);
            risposta.DatiSindacato = datiSindacato;
            #endregion datiSindacato

            #region datiDetrazioni
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            MappingDaHost.ValorizzaDatiDetrazioni(AreaPrelievo, out datiDetrazioni);
            risposta.DatiDetrazioni = datiDetrazioni;
            #endregion datiDetrazioni

            #region datiPagamento
            GestionePagamento.DatiPagamento datiPagamento = null;
            MappingDaHost.ValorizzaDatiPagamento(AreaPrelievo, out datiPagamento);
            risposta.DatiPagamento = datiPagamento;
            #endregion datiPagamento

            #region listaFamiliari
            List<Entity.DatiFamiliari> listaFamiliari = null;
            MappingDaHost.ValorizzaDatiFamiliare(AreaPrelievo, tipoDomanda, out listaFamiliari);
            risposta.ListaFamiliari = listaFamiliari;
            #endregion listaFamiliari

            #region listaCalcoloContributivo
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo = null;
            MappingDaHost.ValorizzaDatiCalcoloContributivo(AreaPrelievo, out listaCalcoloContributivo);
            risposta.ListaCalcoloContributivo = listaCalcoloContributivo;
            #endregion listaCalcoloContributivo

            #region listaCalcoloRetributivo
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo = null;
            MappingDaHost.ValorizzaDatiCalcoloRetributivo(AreaPrelievo, out listaCalcoloRetributivo);
            risposta.ListaCalcoloRetributivo = listaCalcoloRetributivo;
            #endregion listaCalcoloRetributivo

            #region listaSupplementi
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            MappingDaHost.ValorizzaDatiSupplementi(AreaPrelievo, out listaSupplementi);
            risposta.ListaSupplementi = listaSupplementi;
            #endregion listaSupplementi

            #region listaStatiCivili
            List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili = null;
            MappingDaHost.ValorizzaDatiStatiCivili(AreaPrelievo, out listaStatiCivili);
            risposta.ListaStatiCivili = listaStatiCivili;
            #endregion listaStatiCivili

            #region datiDanteCausa
            MappingDaHost.DatiAnagDanteCausa datiAnagDanteCausa = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            MappingDaHost.ValorizzaDatiDanteCausa(AreaPrelievo, out datiAnagDanteCausa, out datiDanteCausa, tipoDomanda, richiesta.Categoria);
            risposta.DatiAnagDanteCausa = datiAnagDanteCausa;
            risposta.DatiDanteCausa = datiDanteCausa;
            #endregion datiDanteCausa

            #region datiPensioniEstereDc
            GestioneDanteCausa.PensioniEstereDcBL pensioniEstereDc = null;
            MappingDaHost.ValorizzaDatiPensioniEstereDc(AreaPrelievo, tipoDomanda, richiesta.Categoria, out pensioniEstereDc);
            risposta.DatiPensioniEstereDc = pensioniEstereDc;

            GestioneDanteCausa.PensioniEstereDcBL importoTotSupplementi = null;
            MappingDaHost.ValorizzaDatiPensioniEstereDcImportoTotaleSupplementi(AreaPrelievo, tipoDomanda, richiesta.Categoria, out importoTotSupplementi);
            risposta.ImportoTotaleSupplementi = importoTotSupplementi;

            GestioneDanteCausa.PensioniEstereDcBL importoArt6 = null;
            MappingDaHost.ValorizzaDatiPensioniEstereDcImportoArt6(AreaPrelievo, tipoDomanda, richiesta.Categoria, out importoArt6);
            risposta.ImportoArt6 = importoArt6;

            #endregion datiPensioniEstereDc

            #region listaResidenzeEstere
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            MappingDaHost.ValorizzaDatiResidenzeEstere(AreaPrelievo, out listaResidenzeEstere);
            risposta.ListaResidenzeEstere = listaResidenzeEstere;
            #endregion listaResidenzeEstere

            #region listaDatiSentenza495_93
            List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiSentenza495_93 = null;
            MappingDaHost.ValorizzaDatiSentenza495_93(AreaPrelievo, tipoDomanda, richiesta.Categoria, out listaDatiSentenza495_93);
            risposta.ListaDatiSentenza495_93 = listaDatiSentenza495_93;
            #endregion listaDatiSentenza495_93

            #region datiIstruttoria
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            MappingDaHost.ValorizzaDatiIstruttoria(AreaPrelievo, tipoDomanda, richiesta.Categoria, out datiIstruttoria);
            risposta.DatiIstruttoria = datiIstruttoria;
            #endregion datiIstruttoria

            #region datiVittimeTerrorismo
            GestioneVittimeTerrorismo.DatiVittimeTerrorismo datiVittimeTerrorismo = null;
            MappingDaHost.ValorizzaDatiVittimeTerrorismo(AreaPrelievo, out datiVittimeTerrorismo);
            risposta.DatiVittimeTerrorismo = datiVittimeTerrorismo;
            #endregion datiVittimeTerrorismo

            #region DatiPensioniCiDatiGenerici
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniCiDatiGenerici = null;
            MappingDaHost.ValorizzaDatiPensioniCiDatiGenerici(AreaPrelievo, tipoDomanda, richiesta.Categoria, out datiPensioniCiDatiGenerici);
            risposta.DatiPensioniCiDatiGenerici = datiPensioniCiDatiGenerici;
            #endregion DatiPensioniCiDatiGenerici

            #region listaPensioniCiImportiValuta
            List<GestioneDatiContributiviCi.PensioniCiImportiValuta> listaPensioniCiImportiValuta = null;
            MappingDaHost.ValorizzaDatiPensioniCIImportiValuta(AreaPrelievo, out listaPensioniCiImportiValuta);
            risposta.ListaPensioniCiImportiValuta = listaPensioniCiImportiValuta;
            #endregion listaPensioniCiImportiValuta

            #region datiIntegrazioneArt11
            GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11 = null;
            MappingDaHost.ValorizzaDatiIntegrazioneArt11(AreaPrelievo, out datiIntegrazioneArt11);
            risposta.DatiIntegrazioneArt11 = datiIntegrazioneArt11;
            #endregion datiIntegrazioneArt11

            #region listaDatiCalcoloContributivoEstero
            List<GestioneCalcolo.DatiCalcoloContributivoEstero> listaDatiCalcoloContributivoEstero = null;
            MappingDaHost.ValorizzaDatiCalcoloContributivoEstero(AreaPrelievo, out listaDatiCalcoloContributivoEstero);
            risposta.ListaCalcoloContributivoEstero = listaDatiCalcoloContributivoEstero;
            #endregion listaDatiCalcoloContributivoEstero

            #region listaPensioniCiMaternitaAcna
            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listaPensioniCiMaternitaAcna = null;
            MappingDaHost.ValorizzaDatiPensioniCiMaternitaAcna(AreaPrelievo, out listaPensioniCiMaternitaAcna);
            risposta.ListaPensioniCiMaternitaAcna = listaPensioniCiMaternitaAcna;
            #endregion listaPensioniCiMaternitaAcna

            #region listaStatiEsteri
            List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
            MappingDaHost.ValorizzaDatiStatiEsteri(AreaPrelievo, out listaStatiEsteri);
            risposta.ListaStatiEsteri = listaStatiEsteri;
            #endregion listaStatiEsteri

            #region datiTutore
            MappingDaHost.DatiTutore datiTutore = null;
            MappingDaHost.ValorizzaDatiTutore(AreaPrelievo, out datiTutore);
            risposta.DatiTutore = datiTutore;
            #endregion datiTutore

            #region datiDelegato
            MappingDaHost.DatiDelegato datiDelegato = null;
            MappingDaHost.ValorizzaDatiDelegato(AreaPrelievo, out datiDelegato);
            risposta.DatiDelegato = datiDelegato;
            #endregion datiDelegato

            #region datiNuoveLiquidate
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            MappingDaHost.ValorizzaDatiNuoveLiquidate(AreaPrelievo, out datiNuoveLiquidate);
            risposta.DatiNuoveLiquidate = datiNuoveLiquidate;
            #endregion datiNuoveLiquidate

            #region datiEliminazione
            GestionePensione.DatiEliminazione datiEliminazione = null;
            MappingDaHost.ValorizzaDatiEliminazione(AreaPrelievo, out datiEliminazione);
            risposta.DatiEliminazione = datiEliminazione;
            #endregion datiEliminazione

            #region datiMaggiorazioniBenefici
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            MappingDaHost.ValorizzaDatiMaggiorazioni(AreaPrelievo, ref datiPensione, tipoDomanda, richiesta.Categoria, out datiMaggiorazioniBenefici);
            risposta.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            #endregion datiMaggiorazioniBenefici

            #region datiOneri
            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            MappingDaHost.ValorizzaDatiOneri(AreaPrelievo, ref datiPensione, out listaDatiOneri);
            risposta.ListaDatiOneri = listaDatiOneri;
            #endregion datiOneri

            #region datiBeneficiParticolari
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = null;
            MappingDaHost.ValorizzaDatiBeneficiParticolari(AreaPrelievo, out listaDatiBeneficiParticolari);
            risposta.ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
            #endregion datiBeneficiParticolari

            #region datiBititolarità
            List<GestioneAltrePensioni.AltraPensione> listaBititolarita = null;
            MappingDaHost.ValorizzaDatiBititolarita(AreaPrelievo, out listaBititolarita);
            risposta.ListaBititolarita = listaBititolarita;
            #endregion datiBititolarità

            #region datiPostDecOriginaria
            List<GestioneContrib.DatiPostDecOriginaria> listaDatiPostDecOriginaria = null;
            MappingDaHost.ValorizzaDatiPostDecOriginaria(AreaPrelievo, out listaDatiPostDecOriginaria);
            risposta.ListaDatiPostDecOriginaria = listaDatiPostDecOriginaria;
            #endregion datiPostDecOriginaria

            #region DatiInail
            //ENG - Reversibilità: campi Inail
            List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaDatiInail = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();
            MappingDaHost.ValorizzaDatiINAIL(AreaPrelievo, out listaDatiInail);
            risposta.ListaDatiInail = listaDatiInail;

            #endregion

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
               AreaPrelievo.FinalResponse.Gruppo1.AreaW2 != null)
            {
                risposta.IABTIPEN = AreaPrelievo.FinalResponse.Gruppo1.AreaW2.IABTIPEN;
            }
        }

        private static void ValorizzaAreaPrelievoNew(RichiestaPrelievo richiesta, out Data.GACINew AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            if (richiesta.Categoria == 0 || richiesta.SedeOperatore == 0 || richiesta.Sede == 0 ||
                richiesta.Certificato == 0)
            {
                messaggioVideo = "Area richiesta non valorizzata correttamente";
                return;
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(richiesta.SedeOperatore.ToString().PadLeft(4, '0') + richiesta.CentroOperativoOperatore.ToString().PadLeft(2, '0'));

            //RINNOVO
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            DateTime dataSistema = Utility.DataSistemaCi;
            int annoCompetenza = 0;
            int annoComp = 0;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoCI", out ctrl);

            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.CI, out annoComp);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo l'anno a 0
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                    annoCompetenza = dataSistema.Year + 1;
                else
                    annoCompetenza = annoComp;
            }

            AreaPrelievo = new INPS.Pensioni.LiquidazioneCi.Data.GACINew(richiesta.Sede.ToString().PadLeft(4, '0'), richiesta.Categoria.ToString().PadLeft(3, '0'),
                richiesta.Certificato.ToString().PadLeft(8, '0'), richiesta.CodiceAf, richiesta.CodiceAs, richiesta.AltriDati, annoCompetenza);

            AreaPrelievo.IsRic = richiesta.TipoDomanda == TipoDomanda.Ricostituzione ? true : false;
        }

        private static void EseguiPrelievoNew(Data.GACINew AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void ControllaEsitoPrelievoNew(Data.GACINew AreaPrelievo, RichiestaPrelievo richiesta, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (AreaPrelievo.HasError)
            {
                if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                    messaggioVideo = AreaPrelievo.Messaggio;
                else
                {
                    if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null && AreaPrelievo.FinalResponse.Gruppo1.AreaTP11 != null && AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CO == 0)
                    {
                        GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
                        GestioneControlliDinamici.ControlloDinamico ctrl = null;
                        DateTime dataSistema = Utility.DataSistemaCi;

                        //RINNOVO
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
                        DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
                        DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievoCI", out ctrl);

                        if (ctrl != null && ctrl.ValoreControllo == "SI" && richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                            && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                            messaggioVideo = "RIC-RIN: OPERAZIONE NON CONSENTITA PER MANCANZA DATI RINNOVATI";
                        else
                            messaggioVideo = "Nessuna pensione presente";

                    }
                }
            }
        }

        private static void NormalizzaAreaToDBNew(Data.GACINew AreaPrelievo, RichiestaPrelievo richiesta, out RispostaPrelievo risposta)
        {
            risposta = new RispostaPrelievo();
            risposta.CodiceFiscale = AreaPrelievo.FinalResponse.Gruppo1.AreaTP12.TP1COFI;
            TipoDomanda tipoDomanda = richiesta.TipoDomanda;
            bool isRiaperturaDomanda = richiesta.IsRiaperturaDomanda;
            List<GestioneDecodifica.StatoEstero> listaDecStatiEsteri = null;

            if (AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEOSEC != 0 &&
                AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEORM != 0)
                risposta.DataDecorrenza = new DateTime(int.Parse(AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEOSEC.ToString().PadLeft(2, '0') +
                    AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEOAA.ToString().PadLeft(2, '0')),
                    (int)AreaPrelievo.FinalResponse.Gruppo1.AreaW1L.IW1DEORM, 1);

            if (AreaPrelievo.FinalResponse.Gruppo1.AreaTP11 != null)
            {
                if (!string.IsNullOrEmpty(AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CITT1))
                {
                    if (listaDecStatiEsteri == null)
                        GestioneDecodifica.GetStatiEsteri(out listaDecStatiEsteri);

                    if (listaDecStatiEsteri != null && listaDecStatiEsteri.Count > 0)
                    {
                        string app = AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CITT1 == "I" ? "ITA" : AreaPrelievo.FinalResponse.Gruppo1.AreaTP11.TP1CITT1;
                        GestioneDecodifica.StatoEstero statoEstero = listaDecStatiEsteri.Find(x => x.Sigla == app);
                        if (statoEstero != null)
                        {
                            risposta.Cittadinanza = !string.IsNullOrEmpty(statoEstero.CodCatastale) ? statoEstero.CodCatastale.Trim() : string.Empty;
                        }
                    }
                }
            }

            #region datiPensione
            GestionePensione.DatiPensione datiPensione = null;
            MappingDaHostNew.ValorizzaDatiPensione(AreaPrelievo, tipoDomanda, isRiaperturaDomanda, richiesta.Categoria, out datiPensione);
            risposta.DatiPensione = datiPensione;
            #endregion datiPensione

            #region datiSindacato
            GestionePensione.DatiSindacato datiSindacato = null;
            MappingDaHostNew.ValorizzaDatiSindacato(AreaPrelievo, out datiSindacato);
            risposta.DatiSindacato = datiSindacato;
            #endregion datiSindacato

            #region datiDetrazioni
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            MappingDaHostNew.ValorizzaDatiDetrazioni(AreaPrelievo, out datiDetrazioni);
            risposta.DatiDetrazioni = datiDetrazioni;
            #endregion datiDetrazioni

            #region datiPagamento
            GestionePagamento.DatiPagamento datiPagamento = null;
            MappingDaHostNew.ValorizzaDatiPagamento(AreaPrelievo, out datiPagamento);
            risposta.DatiPagamento = datiPagamento;
            #endregion datiPagamento

            #region listaFamiliari
            List<Entity.DatiFamiliari> listaFamiliari = null;
            MappingDaHostNew.ValorizzaDatiFamiliare(AreaPrelievo, tipoDomanda, out listaFamiliari);
            risposta.ListaFamiliari = listaFamiliari;
            #endregion listaFamiliari

            #region listaCalcoloContributivo
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo = null;
            MappingDaHostNew.ValorizzaDatiCalcoloContributivo(AreaPrelievo, out listaCalcoloContributivo);
            risposta.ListaCalcoloContributivo = listaCalcoloContributivo;
            #endregion listaCalcoloContributivo

            #region listaCalcoloRetributivo
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo = null;
            MappingDaHostNew.ValorizzaDatiCalcoloRetributivo(AreaPrelievo, out listaCalcoloRetributivo);
            risposta.ListaCalcoloRetributivo = listaCalcoloRetributivo;
            #endregion listaCalcoloRetributivo

            #region listaSupplementi
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            MappingDaHostNew.ValorizzaDatiSupplementi(AreaPrelievo, out listaSupplementi);
            risposta.ListaSupplementi = listaSupplementi;
            #endregion listaSupplementi

            #region listaStatiCivili
            List<GestioneAnagrafica.DatiStatoCivile> listaStatiCivili = null;
            MappingDaHostNew.ValorizzaDatiStatiCivili(AreaPrelievo, out listaStatiCivili);
            risposta.ListaStatiCivili = listaStatiCivili;
            #endregion listaStatiCivili

            #region datiDanteCausa
            MappingDaHostNew.DatiAnagDanteCausa datiAnagDanteCausaNew = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            MappingDaHostNew.ValorizzaDatiDanteCausa(AreaPrelievo, out datiAnagDanteCausaNew, out datiDanteCausa, tipoDomanda, richiesta.Categoria);
            MappingDaHost.DatiAnagDanteCausa datiAnagDanteCausa = new MappingDaHost.DatiAnagDanteCausa();
            Utility.ValorizzaOggetti(datiAnagDanteCausaNew, datiAnagDanteCausa);
            risposta.DatiDanteCausa = datiDanteCausa;
            risposta.DatiAnagDanteCausa = datiAnagDanteCausa;
            #endregion datiDanteCausa

            #region datiPensioniEstereDc
            GestioneDanteCausa.PensioniEstereDcBL pensioniEstereDc = null;
            MappingDaHostNew.ValorizzaDatiPensioniEstereDc(AreaPrelievo, tipoDomanda, richiesta.Categoria, out pensioniEstereDc);
            risposta.DatiPensioniEstereDc = pensioniEstereDc;

            GestioneDanteCausa.PensioniEstereDcBL importoTotSupplementi = null;
            MappingDaHostNew.ValorizzaDatiPensioniEstereDcImportoTotaleSupplementi(AreaPrelievo, tipoDomanda, richiesta.Categoria, out importoTotSupplementi);
            risposta.ImportoTotaleSupplementi = importoTotSupplementi;

            GestioneDanteCausa.PensioniEstereDcBL importoArt6 = null;
            MappingDaHostNew.ValorizzaDatiPensioniEstereDcImportoArt6(AreaPrelievo, tipoDomanda, richiesta.Categoria, out importoArt6);
            risposta.ImportoArt6 = importoArt6;

            #endregion datiPensioniEstereDc

            #region listaResidenzeEstere
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            MappingDaHostNew.ValorizzaDatiResidenzeEstere(AreaPrelievo, out listaResidenzeEstere);
            risposta.ListaResidenzeEstere = listaResidenzeEstere;
            #endregion listaResidenzeEstere

            #region listaDatiSentenza495_93
            List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiSentenza495_93 = null;
            MappingDaHostNew.ValorizzaDatiSentenza495_93(AreaPrelievo, tipoDomanda, richiesta.Categoria, out listaDatiSentenza495_93);
            risposta.ListaDatiSentenza495_93 = listaDatiSentenza495_93;
            #endregion listaDatiSentenza495_93

            #region datiIstruttoria
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            MappingDaHostNew.ValorizzaDatiIstruttoria(AreaPrelievo, tipoDomanda, richiesta.Categoria, out datiIstruttoria);
            risposta.DatiIstruttoria = datiIstruttoria;
            #endregion datiIstruttoria

            #region datiVittimeTerrorismo
            GestioneVittimeTerrorismo.DatiVittimeTerrorismo datiVittimeTerrorismo = null;
            MappingDaHostNew.ValorizzaDatiVittimeTerrorismo(AreaPrelievo, out datiVittimeTerrorismo);
            risposta.DatiVittimeTerrorismo = datiVittimeTerrorismo;
            #endregion datiVittimeTerrorismo

            #region DatiPensioniCiDatiGenerici
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniCiDatiGenerici = null;
            MappingDaHostNew.ValorizzaDatiPensioniCiDatiGenerici(AreaPrelievo, tipoDomanda, richiesta.Categoria, out datiPensioniCiDatiGenerici);
            risposta.DatiPensioniCiDatiGenerici = datiPensioniCiDatiGenerici;
            #endregion DatiPensioniCiDatiGenerici

            #region listaPensioniCiImportiValuta
            List<GestioneDatiContributiviCi.PensioniCiImportiValuta> listaPensioniCiImportiValuta = null;
            MappingDaHostNew.ValorizzaDatiPensioniCIImportiValuta(AreaPrelievo, out listaPensioniCiImportiValuta);
            risposta.ListaPensioniCiImportiValuta = listaPensioniCiImportiValuta;
            #endregion listaPensioniCiImportiValuta

            #region datiIntegrazioneArt11
            GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11 = null;
            MappingDaHostNew.ValorizzaDatiIntegrazioneArt11(AreaPrelievo, out datiIntegrazioneArt11);
            risposta.DatiIntegrazioneArt11 = datiIntegrazioneArt11;
            #endregion datiIntegrazioneArt11

            #region listaDatiCalcoloContributivoEstero
            List<GestioneCalcolo.DatiCalcoloContributivoEstero> listaDatiCalcoloContributivoEstero = null;
            MappingDaHostNew.ValorizzaDatiCalcoloContributivoEstero(AreaPrelievo, out listaDatiCalcoloContributivoEstero);
            risposta.ListaCalcoloContributivoEstero = listaDatiCalcoloContributivoEstero;
            #endregion listaDatiCalcoloContributivoEstero

            #region listaPensioniCiMaternitaAcna
            List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> listaPensioniCiMaternitaAcna = null;
            MappingDaHostNew.ValorizzaDatiPensioniCiMaternitaAcna(AreaPrelievo, out listaPensioniCiMaternitaAcna);
            risposta.ListaPensioniCiMaternitaAcna = listaPensioniCiMaternitaAcna;
            #endregion listaPensioniCiMaternitaAcna

            #region listaStatiEsteri
            List<GestioneContrib.StatoEstero> listaStatiEsteri = null;
            MappingDaHostNew.ValorizzaDatiStatiEsteri(AreaPrelievo, out listaStatiEsteri);
            risposta.ListaStatiEsteri = listaStatiEsteri;
            #endregion listaStatiEsteri

            #region datiTutore
            MappingDaHostNew.DatiTutore datiTutoreNew = null;
            MappingDaHostNew.ValorizzaDatiTutore(AreaPrelievo, out datiTutoreNew);
            MappingDaHost.DatiTutore datiTutore = new MappingDaHost.DatiTutore();
            Utility.ValorizzaOggetti(datiTutoreNew, datiTutore);
            risposta.DatiTutore = datiTutore;
            #endregion datiTutore

            #region datiDelegato
            MappingDaHostNew.DatiDelegato datiDelegatoNew = null;
            MappingDaHostNew.ValorizzaDatiDelegato(AreaPrelievo, out datiDelegatoNew);
            MappingDaHost.DatiDelegato datiDelegato = new MappingDaHost.DatiDelegato();
            Utility.ValorizzaOggetti(datiDelegatoNew, datiDelegato);
            risposta.DatiDelegato = datiDelegato;
            #endregion datiDelegato

            #region datiNuoveLiquidate
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
            MappingDaHostNew.ValorizzaDatiNuoveLiquidate(AreaPrelievo, out datiNuoveLiquidate);
            risposta.DatiNuoveLiquidate = datiNuoveLiquidate;
            #endregion datiNuoveLiquidate

            #region datiEliminazione
            GestionePensione.DatiEliminazione datiEliminazione = null;
            MappingDaHostNew.ValorizzaDatiEliminazione(AreaPrelievo, out datiEliminazione);
            risposta.DatiEliminazione = datiEliminazione;
            #endregion datiEliminazione

            #region datiMaggiorazioniBenefici
            Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            MappingDaHostNew.ValorizzaDatiMaggiorazioni(AreaPrelievo, ref datiPensione, tipoDomanda, richiesta.Categoria, out datiMaggiorazioniBenefici);
            risposta.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            #endregion datiMaggiorazioniBenefici

            #region datiOneri
            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            MappingDaHostNew.ValorizzaDatiOneri(AreaPrelievo, ref datiPensione, out listaDatiOneri);
            risposta.ListaDatiOneri = listaDatiOneri;
            #endregion datiOneri

            #region datiBeneficiParticolari
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = null;
            MappingDaHostNew.ValorizzaDatiBeneficiParticolari(AreaPrelievo, out listaDatiBeneficiParticolari);
            risposta.ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
            #endregion datiBeneficiParticolari

            #region datiBititolarità
            List<GestioneAltrePensioni.AltraPensione> listaBititolarita = null;
            MappingDaHostNew.ValorizzaDatiBititolarita(AreaPrelievo, out listaBititolarita);
            risposta.ListaBititolarita = listaBititolarita;
            #endregion datiBititolarità

            #region datiPostDecOriginaria
            List<GestioneContrib.DatiPostDecOriginaria> listaDatiPostDecOriginaria = null;
            MappingDaHostNew.ValorizzaDatiPostDecOriginaria(AreaPrelievo, out listaDatiPostDecOriginaria);
            risposta.ListaDatiPostDecOriginaria = listaDatiPostDecOriginaria;
            #endregion datiPostDecOriginaria

            #region DatiInail
            //ENG - Reversibilità: campi Inail
            List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaDatiInail = new List<GestionePensioneInailInabilita.DatiPensioniINAIL>();
            MappingDaHostNew.ValorizzaDatiINAIL(AreaPrelievo, out listaDatiInail);
            risposta.ListaDatiInail = listaDatiInail;

            #endregion

            if (AreaPrelievo.FinalResponse != null && AreaPrelievo.FinalResponse.Gruppo1 != null &&
               AreaPrelievo.FinalResponse.Gruppo1.AreaW2 != null)
            {
                risposta.IABTIPEN = AreaPrelievo.FinalResponse.Gruppo1.AreaW2.IABTIPEN;
            }
        }
        #endregion private members

        #region nested class
        [Serializable]
        public class RichiestaPrelievo
        {
            public RichiestaPrelievo(short sede, short categoria, int certificato, string codice_as, string codice_af, string altriDati,
                 short sedeOperatore, short centroOperativoOperatore, TipoDomanda tipoDomanda, bool isRiaperturaDomanda, string numDomanda)
            {
                this._Sede = sede;
                this._Categoria = categoria;
                this._Certificato = certificato;
                this._CodiceAs = codice_as;
                this._CodiceAf = codice_af;
                this._AltriDati = altriDati;
                this._SedeOperatore = sedeOperatore;
                this._CentroOperativoOperatore = centroOperativoOperatore;
                this._TipoDomanda = tipoDomanda;
                this._IsRiaperturaDomanda = isRiaperturaDomanda;
                this._NumDomanda = numDomanda;
            }

            #region public properties
            public short Sede { get { return _Sede; } set { _Sede = value; } }
            public short Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public int Certificato { get { return _Certificato; } set { _Certificato = value; } }
            public string CodiceAs { get { return _CodiceAs; } set { _CodiceAs = value; } }
            public string CodiceAf { get { return _CodiceAf; } set { _CodiceAf = value; } }
            public string AltriDati { get { return _AltriDati; } set { _AltriDati = value; } }
            public short SedeOperatore { get { return _SedeOperatore; } set { _SedeOperatore = value; } }
            public short CentroOperativoOperatore { get { return _CentroOperativoOperatore; } set { _CentroOperativoOperatore = value; } }
            public TipoDomanda TipoDomanda { get { return _TipoDomanda; } set { _TipoDomanda = value; } }
            public bool IsRiaperturaDomanda { get { return _IsRiaperturaDomanda; } set { _IsRiaperturaDomanda = value; } }
            public string NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }
            #endregion public properties

            #region private properties
            private short _Sede;
            private short _Categoria;
            private int _Certificato;
            private string _CodiceAs;
            private string _CodiceAf;
            private string _AltriDati;
            private short _SedeOperatore;
            private short _CentroOperativoOperatore;
            private TipoDomanda _TipoDomanda;
            private bool _IsRiaperturaDomanda;
            private string _NumDomanda;
            #endregion private properties
        }

        public class RispostaPrelievo
        {
            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public System.Nullable<DateTime> DataDecorrenza { get { return _DataDecorrenza; } set { _DataDecorrenza = value; } }
            public GestionePensione.DatiPensione DatiPensione { get { return _DatiPensione; } set { _DatiPensione = value; } }
            public GestionePensione.DatiSindacato DatiSindacato { get { return _DatiSindacato; } set { _DatiSindacato = value; } }
            public GestioneDetrazioniImposta.DatiDetrazioni DatiDetrazioni { get { return _DatiDetrazioni; } set { _DatiDetrazioni = value; } }
            public GestionePagamento.DatiPagamento DatiPagamento { get { return _DatiPagamento; } set { _DatiPagamento = value; } }
            public List<Entity.DatiFamiliari> ListaFamiliari { get { return _ListaFamiliari; } set { _ListaFamiliari = value; } }
            public List<GestioneCalcolo.DatiCalcoloContributivo> ListaCalcoloContributivo { get { return _ListaCalcoloContributivo; } set { _ListaCalcoloContributivo = value; } }
            public List<GestioneCalcolo.DatiCalcoloRetributivo> ListaCalcoloRetributivo { get { return _ListaCalcoloRetributivo; } set { _ListaCalcoloRetributivo = value; } }
            public List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> ListaSupplementi { get { return _ListaSupplementi; } set { _ListaSupplementi = value; } }
            public MappingDaHost.DatiAnagDanteCausa DatiAnagDanteCausa { get { return _DatiAnagDanteCausa; } set { _DatiAnagDanteCausa = value; } }
            public GestioneDanteCausa.DatiDanteCausa DatiDanteCausa { get { return _DatiDanteCausa; } set { _DatiDanteCausa = value; } }
            public GestioneDanteCausa.PensioniEstereDcBL DatiPensioniEstereDc { get { return _DatiPensioniEstereDc; } set { _DatiPensioniEstereDc = value; } }
            public GestioneDanteCausa.PensioniEstereDcBL ImportoTotaleSupplementi { get { return _ImportoTotaleSupplementi; } set { _ImportoTotaleSupplementi = value; } }
            public GestioneDanteCausa.PensioniEstereDcBL ImportoArt6 { get { return _ImportoArt6; } set { _ImportoArt6 = value; } }
            public List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstere { get { return _ListaResidenzeEstere; } set { _ListaResidenzeEstere = value; } }
            public List<GestioneDanteCausa.DatiRedditoSentenza495_93> ListaDatiSentenza495_93 { get { return _ListaDatiSentenza495_93; } set { _ListaDatiSentenza495_93 = value; } }
            public GestioneIstruttoria.DatiIstruttoria DatiIstruttoria { get { return _DatiIstruttoria; } set { _DatiIstruttoria = value; } }
            public MappingDaHost.DatiDelegato DatiDelegato { get { return _DatiDelegato; } set { _DatiDelegato = value; } }
            public MappingDaHost.DatiTutore DatiTutore { get { return _DatiTutore; } set { _DatiTutore = value; } }
            public GestioneVittimeTerrorismo.DatiVittimeTerrorismo DatiVittimeTerrorismo { get { return _DatiVittimeTerrorismo; } set { _DatiVittimeTerrorismo = value; } }
            public List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCivili { get { return _ListaStatiCivili; } set { _ListaStatiCivili = value; } }
            public GestioneDatiGenericiAgoCi.PensioniDatiGenerici DatiPensioniCiDatiGenerici { get { return _DatiPensioniCiDatiGenerici; } set { _DatiPensioniCiDatiGenerici = value; } }
            public List<GestioneDatiContributiviCi.PensioniCiImportiValuta> ListaPensioniCiImportiValuta { get { return _ListaPensioniCiImportiValuta; } set { _ListaPensioniCiImportiValuta = value; } }
            public GestioneIntegrazioneArt11.IntegrazioneArt11 DatiIntegrazioneArt11 { get { return _DatiIntegrazioneArt11; } set { _DatiIntegrazioneArt11 = value; } }
            public List<GestioneCalcolo.DatiCalcoloContributivoEstero> ListaCalcoloContributivoEstero { get { return _ListaCalcoloContributivoEstero; } set { _ListaCalcoloContributivoEstero = value; } }
            public List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> ListaPensioniCiMaternitaAcna { get { return _ListaPensioniCiMaternitaAcna; } set { _ListaPensioniCiMaternitaAcna = value; } }
            public List<GestioneContrib.StatoEstero> ListaStatiEsteri { get { return _ListaStatiEsteri; } set { _ListaStatiEsteri = value; } }
            public GestioneNuoveLiquidate.NuoveLiquidate DatiNuoveLiquidate { get { return _DatiNuoveLiquidate; } set { _DatiNuoveLiquidate = value; } }
            public GestionePensione.DatiEliminazione DatiEliminazione { get { return _DatiEliminazione; } set { _DatiEliminazione = value; } }
            public Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici DatiMaggiorazioniBenefici { get { return _DatiMaggiorazioniBenefici; } set { _DatiMaggiorazioniBenefici = value; } }
            public List<GestioneOneri.DatiOneri> ListaDatiOneri { get { return _ListaDatiOneri; } set { _ListaDatiOneri = value; } }
            public List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari { get { return _ListaDatiBeneficiParticolari; } set { _ListaDatiBeneficiParticolari = value; } }
            public List<GestioneAltrePensioni.AltraPensione> ListaBititolarita { get { return _ListaBititolarita; } set { _ListaBititolarita = value; } }
            public List<GestioneContrib.DatiPostDecOriginaria> ListaDatiPostDecOriginaria { get { return _ListaDatiPostDecOriginaria; } set { _ListaDatiPostDecOriginaria = value; } }
            public string Cittadinanza { get { return _Cittadinanza; } set { _Cittadinanza = value; } }
            //ENG - Reversibilità: campi Inail
            public List<GestionePensioneInailInabilita.DatiPensioniINAIL> ListaDatiInail { get { return _ListaDatiInail; } set { _ListaDatiInail = value; } }

            public string IABTIPEN { get { return _IABTIPEN; } set { _IABTIPEN = value; } }
            #endregion public properties

            #region private properties
            private string _CodiceFiscale;
            private System.Nullable<DateTime> _DataDecorrenza;
            private GestionePensione.DatiPensione _DatiPensione;
            private GestionePensione.DatiSindacato _DatiSindacato;
            private GestioneDetrazioniImposta.DatiDetrazioni _DatiDetrazioni;
            private GestionePagamento.DatiPagamento _DatiPagamento;
            private List<Entity.DatiFamiliari> _ListaFamiliari;
            private List<GestioneCalcolo.DatiCalcoloContributivo> _ListaCalcoloContributivo;
            private List<GestioneCalcolo.DatiCalcoloRetributivo> _ListaCalcoloRetributivo;
            private List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> _ListaSupplementi;
            private MappingDaHost.DatiAnagDanteCausa _DatiAnagDanteCausa;
            private GestioneDanteCausa.DatiDanteCausa _DatiDanteCausa;
            private GestioneDanteCausa.PensioniEstereDcBL _DatiPensioniEstereDc;
            private GestioneDanteCausa.PensioniEstereDcBL _ImportoTotaleSupplementi;
            private GestioneDanteCausa.PensioniEstereDcBL _ImportoArt6;
            private List<GestioneAnagrafica.DatiResidenzaEstero> _ListaResidenzeEstere;
            private List<GestioneDanteCausa.DatiRedditoSentenza495_93> _ListaDatiSentenza495_93;
            private GestioneIstruttoria.DatiIstruttoria _DatiIstruttoria;
            private MappingDaHost.DatiDelegato _DatiDelegato;
            private MappingDaHost.DatiTutore _DatiTutore;
            private GestioneVittimeTerrorismo.DatiVittimeTerrorismo _DatiVittimeTerrorismo;
            private List<GestioneAnagrafica.DatiStatoCivile> _ListaStatiCivili;
            private GestioneDatiGenericiAgoCi.PensioniDatiGenerici _DatiPensioniCiDatiGenerici;
            private List<GestioneDatiContributiviCi.PensioniCiImportiValuta> _ListaPensioniCiImportiValuta;
            private GestioneIntegrazioneArt11.IntegrazioneArt11 _DatiIntegrazioneArt11;
            private List<GestioneCalcolo.DatiCalcoloContributivoEstero> _ListaCalcoloContributivoEstero;
            private List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> _ListaPensioniCiMaternitaAcna;
            private List<GestioneContrib.StatoEstero> _ListaStatiEsteri;
            private GestioneNuoveLiquidate.NuoveLiquidate _DatiNuoveLiquidate;
            private GestionePensione.DatiEliminazione _DatiEliminazione;
            private Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici _DatiMaggiorazioniBenefici;
            private List<GestioneOneri.DatiOneri> _ListaDatiOneri;
            private List<GestioneBeneficiParticolari.DatiBeneficiParticolari> _ListaDatiBeneficiParticolari;
            private List<GestioneAltrePensioni.AltraPensione> _ListaBititolarita;
            private List<GestioneContrib.DatiPostDecOriginaria> _ListaDatiPostDecOriginaria;
            private string _Cittadinanza;
            private string _IABTIPEN;
            //ENG - Reversibilità: campi Inail
            private List<GestionePensioneInailInabilita.DatiPensioniINAIL> _ListaDatiInail;
            #endregion private properties
        }

        [Serializable]
        public enum TipoDomanda
        {
            Reversibilità,
            Ricostituzione
        };
        #endregion nested class
    }
}





