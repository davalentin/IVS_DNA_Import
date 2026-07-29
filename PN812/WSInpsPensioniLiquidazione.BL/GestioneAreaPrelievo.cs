using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaPrelievo
    {
        #region public members
        #endregion public members

        #region private members
        internal static void SalvaAreaPrelievo(GestionePensione.DatiPensione datiPensioneMaster, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaMaster,
            GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidateMaster, GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiMaster,
            GestioneFondo.DatiFondo datiPensioneFondoDatiGenericiMaster, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenericiMaster, List<GestioneOneri.DatiOneri> lOneriMaster,
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> lDatiBeneficiParticolariMaster, object AreaPrelievo, Utility.TipoAppartenenza tipoAppartenenza, Utility.TipoDomanda tipoDomanda,
            Entity.ParametriARCA parametriArca, GestioneAnagrafica.DatiAnagrafici anagraficaTitolare, GestioneDatiControlloFelpe.ControlloFelpe controlloFelpeMaster,
            List<GestioneFamiliari.FamiliareRecuperato> CFfamiliari, GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivoEnpals,
            GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivoEnpals, GestioneEnpals.DatiEnpals datiENPALS, bool isRiapertura,
            List<GestionePrepensionamento.DatiPrepensionamento> listaDatiPrepensionamento, List<BLCommon.Entity.DatiSupplementiENPALS> listaDatiSupplementiENPALS,
            BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneENPALSSupp, BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneENPALSLiq,
            GestionePagamento.DatiPagamento datiPagamentoWebDom, List<BLCommon.Entity.DatiSuppRecordENPALS> listaDatiSuppRecordENPALS, GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP,
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivoStorico, List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivoStorico,
            List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtileStorico, List<GestioneCalcolo.QuotePensione> listaDatiQuotePensioneStorico,
            List<GestioneCalcolo.TrattenuteQuotePensione> listaDatiTrattenuteQuotePensioneStorico, List<GestioneOneri.DatiOneri> listaDatiOneriStorico,
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolariStorico, bool isDomandaConNuovaGestioneDatiFondoFSPT,
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDirittoWebDom, List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDirittoGP,
            GestionePensione.DatiSindacato datiSindacatoMaster, List<GestioneAnagrafica.DatiStatoCivile> listaDatiStatoCivileMaster, List<GestioneCalcolo.QuotePensione> listaDatiQuotePensioneTotalIVS,
            List<GestioneCalcolo.TrattenuteQuotePensione> listaDatiTrattenuteQuotePensioneTotalIVS, List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditoSentenza495_93,
            GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAPMaster, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAPMaster,
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioniImposta, GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivoEnpalsStorico,
            GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivoEnpalsStorico, List<BLCommon.Entity.DatiSupplementiCumulo> listaDatiSupplementiCumuloStorico,
            List<ServiceReferences.LiquidazioneCi.GestioneContribStatoEstero> listaStatiEsteriCiStorico, Entity.DatiFondoSpecificoFELPE datiFondoSpecificoFELPE,
            List<BLCommon.Entity.DatiSupplementiCumulo> listaDatiSupplementiCumuloTotalIVS, List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoGP,
            List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP, short sedeChiavePensione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, Utility.TipoAutomazione? tipoAutomazione, out List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo, out bool isFamiliariVerde, out long idFondo, List<ServiceReferences.AggPec.CI_ISTITUZIONI> istituzioniEsterePECO, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa,
            List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivoSOPGI, List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivoSOPGI, List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> datiCalcoloContributivoQuotaFondoSOPGI, List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> datiCalcoloRetributivoQuotaFondoSOPGI,
            List<BLCommon.Entity.DatiSupplementi> datiSupplementiSOPGI, decimal? coefficienteTrasformazione, DateTime? maxDecDatiCalcoloAnte96, List<BLCommon.Entity.DatiSupplementi> listaDatiSupplementiStorico, GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023, bool isTabInailVisibleForCI, DateTime? scadenzaBeneficioUnicarpe,
            GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023, List<ServiceReferences.LiquidazioneAgo.GestioneContribStatoEsteroCumulo> listaStatiEsteriCumuloStorico, List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaDatiQuotaFondoIntegrativoStorico, ServiceReferences.WebDom.DatiDomanda datiDomanda,
            List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattualiMaster, List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattualiStorico, GestioneMiglioramentiContrattuali.DatiMiglioramentiContrattuali miglioramentiContrattualiMaster,
            List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiCalcoloRetributivoINPGIStorico, List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiCalcoloContributivoINPGIStorico, List<GestioneCalcolo.DatiCalcoloContributivo> datiCalcoloContributivoSpacchettateAGO, List<GestioneCalcolo.DatiCalcoloRetributivo> datiCalcoloRetributivoSpacchettateAGO, List<BLCommon.Entity.DatiSupplementi> datiSupplementiSpacchettateAGO, out string errori)
        {
            errori = string.Empty;
            bool familiariDaPrelievo = false;
            isFamiliariVerde = false;
            idFondo = 0;
            Dictionary<long, long> dictionaryIdRecordFondo = null;
            listaDatiRecordFondo = null;

            if (Utility.IsDomandaENPALS(datiPensioneMaster.Gestione))
            {
                long idPensione = 0;
                Dictionary<DateTime, long> idRecordENPALSMaster = null;
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.AGO:
                        if (AreaPrelievo != null)
                        {
                            ServiceReferences.LiquidazioneAgo.AreaPrelievo prelievoAgo = (ServiceReferences.LiquidazioneAgo.AreaPrelievo)AreaPrelievo;
                            if (prelievoAgo != null && prelievoAgo.Risposta != null)
                            {
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                                {
                                    // Vengono impostati a null perchè per le ricostituzioni ci arrivano dei dati errati dal prelievo
                                    // Rif. mail con oggetto "RE: Analisi RIC AGO" del 07/08/2014
                                    prelievoAgo.Risposta.DatiPensione.RequisitiAl1294 = null;
                                    prelievoAgo.Risposta.DatiPensione.RequisitiAl996 = null;
                                    prelievoAgo.Risposta.DatiPensione.RequisitiVecchiaiaAl1294 = null;
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////

                                    // Pulizia dei dati sporchi che sono stati recuperati dal prelievo (in caso di RIC per motivi contributivi, con importo pro rata temporis valorizzato, non bisogna effettuare la pulizia
                                    //del campo Natura Pensione per non perdere il valore impostato in fase di chiamata al SAI)
                                    if (!Utility.IsRicostituzione_MotiviContributivi(datiPensioneMaster) || (datiCalcoloRetributivoEnpals == null || datiCalcoloRetributivoEnpals.ImportoProRataTemporis.GetValueOrDefault() == 0))
                                        datiPensioneMaster.NaturaPensione = prelievoAgo.Risposta.DatiPensione.NaturaPensione;
                                    if (!Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoria = null;
                                    if (!Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 = null;
                                    if (!Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 = null;
                                    if (!Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoriaLetteraB = null;
                                    if (!Utility.IsDomandaAPEPrecoci(datiPensioneMaster) && !Utility.IsDomandaAPESociale(datiPensioneMaster.SiglaCategoria) &&
                                        !Utility.IsPoloPALS(datiPensioneMaster))
                                    {
                                        prelievoAgo.Risposta.DatiPensione.CodiceSedeDestinazione = null;
                                        prelievoAgo.Risposta.DatiPensione.CentroOperativoDestinazione = null;
                                        if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura))
                                        {
                                            datiPensioneMaster.CodiceSede = sedeChiavePensione;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////

                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoAgo.Risposta.DatiPensione, null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, prelievoAgo.Risposta.DatiIstruttoria, idPensione);
                                    //ENG - RIC REVERSIBILITA ENPALS
                                    if (!(tipoDomanda == Utility.TipoDomanda.Ricostituzione && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensioneMaster, danteCausa)
                                        && datiStoricoGP != null && (datiStoricoGP.TipoSettimaneBeneficio == "14" || datiStoricoGP.TipoSettimaneBeneficio == "18" || datiStoricoGP.TipoSettimaneBeneficio == "19" || datiStoricoGP.TipoSettimaneBeneficio == "12" || datiStoricoGP.TipoSettimaneBeneficio == "24")))
                                    {
                                        ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, prelievoAgo.Risposta.DatiMaggiorazioniBenefici, idPensione);
                                    }
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    ArchiviaDatiPagamento(prelievoAgo.Risposta.DatiPagamento, datiPagamentoWebDom, idPensione);
                                    List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari = null;
                                    ArchiviaDatiFamiliare(parametriArca, prelievoAgo.Risposta.ListaFamiliari, anagraficaTitolare, tipoAppartenenza, tipoDomanda, datiPensioneMaster, isRiapertura, out familiariDaPrelievo,
                                        out isFamiliariVerde, out listaFamiliari, danteCausa, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        return;
                                    if (prelievoAgo.Risposta.ListaResidenzeEstere != null)
                                        ArchiviaDatiResidenzeEstere(prelievoAgo.Risposta.ListaResidenzeEstere.ToList(), idPensione, anagraficaTitolare.Id);
                                    if (prelievoAgo.Risposta.ListaStatiCivili != null)
                                        ArchiviaDatiStatiCivili(prelievoAgo.Risposta.ListaStatiCivili.ToList(), idPensione, anagraficaTitolare.Id);
                                    if (prelievoAgo.Risposta.DatiSindacato != null)
                                        prelievoAgo.Risposta.DatiSindacato.IsFromService = true;
                                    //ArchiviaDatiDelegato(parametriArca, prelievoAgo.Risposta.DatiDelegato, tipoAppartenenza, datiPensioneMaster);
                                    ArchiviaDatiTutore(parametriArca, prelievoAgo.Risposta.DatiTutore, tipoAppartenenza, idPensione, datiPensioneMaster.NDomus.ToString());
                                    ArchiviaDatiDetrazioni(prelievoAgo.Risposta.DatiDetrazioni, idPensione);
                                    ArchiviaDatiSindacato(datiSindacatoMaster, prelievoAgo.Risposta.DatiSindacato, idPensione);
                                    ArchiviaDatiSupplementiBase(prelievoAgo.Risposta.DatiSupplementiBase, idPensione);
                                    ArchiviaDatiEliminazione(prelievoAgo.Risposta.DatiEliminazione, idPensione);
                                    ArchiviaDatiPensioniDatiGenerici(prelievoAgo.Risposta.DatiPensioniDatiGenerici, idPensione);
                                    if (prelievoAgo.Risposta.ListaBititolarita != null)
                                        ArchiviaDatiBititolarita(prelievoAgo.Risposta.ListaBititolarita.ToList(), idPensione);
                                    if (prelievoAgo.Risposta.ListaInail != null)
                                        ArchiviaDatiInail(prelievoAgo.Risposta.ListaInail.ToList(), idPensione);
                                    ArchiviaDatiInabilita(prelievoAgo.Risposta.DatiInabilita, idPensione);

                                    if (prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93 != null)
                                        ArchiviaDatiRedditiSentenza495_93(prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93.ToList(), idPensione, datiPensioneMaster);
                                    ArchiviaDatiCalcoloContributivoEnpals(datiCalcoloContributivoEnpals, prelievoAgo.Risposta.CalcoloContributivoENPALS, idPensione);
                                    ArchiviaDatiCalcoloRetributivoEnpals(datiCalcoloRetributivoEnpals, prelievoAgo.Risposta.CalcoloRetributivoENPALS, idPensione);

                                    if (lOneriMaster != null && lOneriMaster.Count > 0)
                                    {
                                        ArchiviaDatiOneri(lOneriMaster, idPensione);

                                        if (listaDatiOneriStorico != null && listaDatiOneriStorico.Count > 0)
                                            ArchiviaDatiOneri(listaDatiOneriStorico, idPensione);
                                    }
                                    else if (prelievoAgo.Risposta.ListaDatiOneri != null)
                                        ArchiviaDatiOneri(prelievoAgo.Risposta.ListaDatiOneri.ToList(), idPensione);
                                    ArchiviaDatiENPALS(datiENPALS, prelievoAgo.Risposta.DatiENPALS, idPensione);

                                    if (prelievoAgo.Risposta.ListaSuppRecordENPALS != null && prelievoAgo.Risposta.ListaSuppRecordENPALS.Count() > 0)
                                        ArchiviaDatiRecordSuppENPALS(listaDatiSuppRecordENPALS, prelievoAgo.Risposta.ListaSuppRecordENPALS.ToList(), idPensione, out idRecordENPALSMaster);
                                    else
                                        ArchiviaDatiRecordSuppENPALS(listaDatiSuppRecordENPALS, null, idPensione, out idRecordENPALSMaster);
                                    ArchiviaDatiIntegrazioneArt11(prelievoAgo.Risposta.DatiIntegrazioneArt11, idPensione);
                                    if (prelievoAgo.Risposta.ListaSupplementiENPALS != null && prelievoAgo.Risposta.ListaSupplementiENPALS.Count() > 0)
                                        ArchiviaDatiSupplementiENPALS(listaDatiSupplementiENPALS, prelievoAgo.Risposta.ListaSupplementiENPALS.ToList(), idPensione, idRecordENPALSMaster);
                                    else
                                        ArchiviaDatiSupplementiENPALS(listaDatiSupplementiENPALS, null, idPensione, idRecordENPALSMaster);

                                    ArchiviaDatiContribuzioneENPALS(datiContribuzioneENPALSSupp, datiPensioneMaster);
                                    ArchiviaDatiContribuzioneENPALS(datiContribuzioneENPALSLiq, datiPensioneMaster);

                                    ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        return;

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);
                                    if (datiCalcoloRetributivoEnpalsStorico != null)
                                        ArchiviaDatiCalcoloRetributivoEnpals(datiCalcoloRetributivoEnpalsStorico, null, idPensione);
                                    if (datiCalcoloContributivoEnpalsStorico != null)
                                        ArchiviaDatiCalcoloContributivoEnpals(datiCalcoloContributivoEnpalsStorico, null, idPensione);

                                    if (prelievoAgo.Risposta.ListaDatiSentenzaArt4 != null && prelievoAgo.Risposta.ListaDatiSentenzaArt4.Count() > 0)
                                        ArchiviaDatiSentenzaArt4(prelievoAgo.Risposta.ListaDatiSentenzaArt4.ToList(), idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiSentenze != null && prelievoAgo.Risposta.ListaDatiSentenze.Count() > 0)
                                        ArchiviaDatiSentenze(prelievoAgo.Risposta.ListaDatiSentenze.ToList(), idPensione);
                                }
                                else
                                {
                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoAgo.Risposta.DatiPensione, null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, null, idPensione);
                                    ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, null, idPensione);
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93 != null)
                                        ArchiviaDatiRedditiSentenza495_93(prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93.ToList(), idPensione, datiPensioneMaster);

                                    ArchiviaDatiCalcoloContributivoEnpals(datiCalcoloContributivoEnpals, prelievoAgo.Risposta.CalcoloContributivoENPALS, idPensione);
                                    ArchiviaDatiCalcoloRetributivoEnpals(datiCalcoloRetributivoEnpals, prelievoAgo.Risposta.CalcoloRetributivoENPALS, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiOneri != null)
                                        ArchiviaDatiOneri(prelievoAgo.Risposta.ListaDatiOneri.ToList(), idPensione);
                                    if (prelievoAgo.Risposta.ListaSuppRecordENPALS != null && prelievoAgo.Risposta.ListaSuppRecordENPALS.Count() > 0)
                                        ArchiviaDatiRecordSuppENPALS(listaDatiSuppRecordENPALS, prelievoAgo.Risposta.ListaSuppRecordENPALS.ToList(), idPensione, out idRecordENPALSMaster);
                                    else
                                        ArchiviaDatiRecordSuppENPALS(listaDatiSuppRecordENPALS, null, idPensione, out idRecordENPALSMaster);
                                    if (prelievoAgo.Risposta.ListaSupplementiENPALS != null && prelievoAgo.Risposta.ListaSupplementiENPALS.Count() > 0)
                                        ArchiviaDatiSupplementiENPALS(listaDatiSupplementiENPALS, prelievoAgo.Risposta.ListaSupplementiENPALS.ToList(), idPensione, idRecordENPALSMaster);
                                    else
                                        ArchiviaDatiSupplementiENPALS(listaDatiSupplementiENPALS, null, idPensione, idRecordENPALSMaster);

                                    ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        return;

                                    ArchiviaDatiSindacato(datiSindacatoMaster, null, idPensione);

                                    ArchiviaDatiENPALS(datiENPALS, prelievoAgo.Risposta.DatiENPALS, idPensione);

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                                    if (datiDetrazioniImposta != null)
                                        ArchiviaDatiDetrazioni(datiDetrazioniImposta, idPensione);

                                    if (CFfamiliari != null && CFfamiliari.Count > 0)
                                    {
                                        ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    if (prelievoAgo.Risposta.ListaDatiSentenzaArt4 != null && prelievoAgo.Risposta.ListaDatiSentenzaArt4.Count() > 0)
                                        ArchiviaDatiSentenzaArt4(prelievoAgo.Risposta.ListaDatiSentenzaArt4.ToList(), idPensione);

                                    if (datiPagamentoWebDom != null)
                                        GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);
                                }
                                return;
                            }
                        }
                        break;
                }

                ArchiviaDatiPensione(datiPensioneMaster, null, null, out idPensione);
                ArchiviaDatiRedditiSentenza495_93(listaDatiRedditoSentenza495_93, idPensione, datiPensioneMaster);
                ArchiviaDatiCalcoloContributivoEnpals(datiCalcoloContributivoEnpals, null, idPensione);
                ArchiviaDatiCalcoloRetributivoEnpals(datiCalcoloRetributivoEnpals, null, idPensione);
                if (lOneriMaster != null)
                    ArchiviaDatiOneri(lOneriMaster, idPensione);
                ArchiviaDatiIstruttoria(datiIstruttoriaMaster, null, idPensione);
                ArchiviaDatiENPALS(datiENPALS, null, idPensione);

                ArchiviaDatiRecordSuppENPALS(listaDatiSuppRecordENPALS, null, idPensione, out idRecordENPALSMaster);
                ArchiviaDatiSupplementiENPALS(listaDatiSupplementiENPALS, null, idPensione, idRecordENPALSMaster);

                ArchiviaDatiContribuzioneENPALS(datiContribuzioneENPALSSupp, datiPensioneMaster);
                ArchiviaDatiContribuzioneENPALS(datiContribuzioneENPALSLiq, datiPensioneMaster);
                ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, null, idPensione);

                ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return;

                ArchiviaDatiSindacato(datiSindacatoMaster, null, idPensione);

                if (CFfamiliari != null && CFfamiliari.Count > 0)
                {
                    ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                    if (!string.IsNullOrEmpty(errori))
                        return;
                }

                if (datiPagamentoWebDom != null)
                    GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);

                ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                return;
            }
            else
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        if (AreaPrelievo != null)
                        {
                            ServiceReferences.LiquidazioneFs.AreaPrelievo prelievoFs = (ServiceReferences.LiquidazioneFs.AreaPrelievo)AreaPrelievo;
                            if (prelievoFs != null && prelievoFs.Risposta != null)
                            {
                                long idPensione = 0;
                                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensioneMaster.SiglaCategoria);
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti || isRiapertura)
                                {
                                    if (prelievoFs.Risposta.DatiPensione != null &&
                                        (!prelievoFs.Risposta.DatiPensione.CodiceArretrati.HasValue || prelievoFs.Risposta.DatiPensione.CodiceArretrati.Value == 0))
                                        prelievoFs.Risposta.DatiPensione.CodiceArretrati = 8;

                                    if (tipoDomanda == Utility.TipoDomanda.Ripristino)
                                    {
                                        prelievoFs.Risposta.DatiPensione.CodiceSedeDestinazione = null;
                                        prelievoFs.Risposta.DatiPensione.CentroOperativoDestinazione = null;
                                    }

                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoFs.Risposta.DatiPensione, null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, prelievoFs.Risposta.DatiIstruttoria, idPensione);
                                    ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, prelievoFs.Risposta.DatiMaggiorazioniBenefici, idPensione);
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    ArchiviaDatiEliminazione(prelievoFs.Risposta.DatiEliminazione, idPensione);
                                    ArchiviaDatiSindacato(datiSindacatoMaster, prelievoFs.Risposta.DatiSindacato, idPensione);
                                    ArchiviaDatiDetrazioni(prelievoFs.Risposta.DatiDetrazioni, idPensione);
                                    if (tipoDomanda != Utility.TipoDomanda.Ripristino && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti)
                                        ArchiviaDatiPagamento(prelievoFs.Risposta.DatiPagamento, datiPagamentoWebDom, idPensione);
                                    List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari = null;
                                    ArchiviaDatiFamiliare(parametriArca, prelievoFs.Risposta.ListaFamiliari, anagraficaTitolare, tipoAppartenenza, tipoDomanda, datiPensioneMaster, isRiapertura, out familiariDaPrelievo,
                                        out isFamiliariVerde, out listaFamiliari, danteCausa, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        return;
                                    if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensioneMaster) && prelievoFs.Risposta.ListaDetrazioniContitolare != null)
                                        ArchiviaDatiDetrazioniContitolare(prelievoFs.Risposta.ListaDetrazioniContitolare.ToList(), listaFamiliari, idPensione);
                                    if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione))
                                    {
                                        ArchiviaDatiFondoINPDAP(prelievoFs.Risposta.DatiPensioneFondoDatiGenerici,
                                            prelievoFs.Risposta.ListaRecordFondo != null ? prelievoFs.Risposta.ListaRecordFondo.ToList() : null,
                                            prelievoFs.Risposta.ListaDatiPensioneINPDAP != null ? prelievoFs.Risposta.ListaDatiPensioneINPDAP.ToList() : null,
                                            prelievoFs.Risposta.ListaRecordDatiFondoINPDAP != null ? prelievoFs.Risposta.ListaRecordDatiFondoINPDAP.ToList() : null,
                                            datiPensioneINPDAPMaster, recordDatiFondoINPDAPMaster, listaRecordDatiFondoGP,
                                            idPensione, out idFondo, out dictionaryIdRecordFondo);
                                        ArchiviaDatiDelegato(parametriArca, prelievoFs.Risposta.DatiDelegato, tipoAppartenenza, datiPensioneMaster);
                                        ArchiviaDatiTutore(parametriArca, prelievoFs.Risposta.DatiTutore, tipoAppartenenza, idPensione, datiPensioneMaster.NDomus.ToString());

                                    }
                                    else
                                    {

                                        ArchiviaDatiFondo(tipoFondo, prelievoFs.Risposta.DatiPensioneFondoDatiGenerici, prelievoFs.Risposta.DatiFondoSpecifico, datiFondoSpecificoFELPE,
                                        prelievoFs.Risposta.ListaRecordFondo != null ? prelievoFs.Risposta.ListaRecordFondo.ToList() : null, idPensione, isDomandaConNuovaGestioneDatiFondoFSPT,
                                        prelievoFs.Risposta.DatiDanteCausa, prelievoFs.Risposta.DatiLavorazione, datiPensioneMaster, out idFondo, out dictionaryIdRecordFondo);
                                    }
                                    if (prelievoFs.Risposta.ListaSupplementi != null)
                                        ArchiviaDatiSupplementi(prelievoFs.Risposta.ListaSupplementi.ToList(), idPensione);
                                    if (prelievoFs.Risposta.ListaResidenzeEstere != null)
                                        ArchiviaDatiResidenzeEstere(prelievoFs.Risposta.ListaResidenzeEstere.ToList(), idPensione, anagraficaTitolare.Id);
                                    if (prelievoFs.Risposta.ListaStatiCivili != null)
                                        ArchiviaDatiStatiCivili(prelievoFs.Risposta.ListaStatiCivili.ToList(), idPensione, anagraficaTitolare.Id);
                                    ArchiviaDatiDL407(prelievoFs.Risposta.DatiDL407, idPensione);
                                    if (prelievoFs.Risposta.ListaOneriTerrorismo != null)
                                        ArchiviaDatiOneriTerrorismo(prelievoFs.Risposta.ListaOneriTerrorismo.ToList(), tipoAppartenenza, idPensione);
                                    if (prelievoFs.Risposta.ListaDatiNoCalcolo != null && prelievoFs.Risposta.ListaDatiNoCalcolo.Count() > 0)
                                        ArchiviaDatiNoCalcolo(prelievoFs.Risposta.ListaDatiNoCalcolo.ToList(), datiPensioneMaster);
                                    if (prelievoFs.Risposta.ListaInail != null)
                                        ArchiviaDatiInail(prelievoFs.Risposta.ListaInail.ToList(), idPensione);
                                    if (Utility.IsDomandaUnicarpe(datiPensioneMaster, true) != Utility.TipoUnicarpe.Automatica)
                                    {
                                        if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione))
                                        {
                                            if (prelievoFs.Risposta.ListaDatiServizioUtileINPDAP != null)
                                                ArchiviaDatiServizioUtileINPDAP(prelievoFs.Risposta.ListaDatiServizioUtileINPDAP.ToList(), dictionaryIdRecordFondo, idPensione);
                                        }
                                        if (prelievoFs.Risposta.ListaDatiServizioUtile != null)
                                            ArchiviaDatiServizioUtile(prelievoFs.Risposta.ListaDatiServizioUtile.ToList(), idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione,
                                                tipoFondo, isDomandaConNuovaGestioneDatiFondoFSPT);
                                        if (prelievoFs.Risposta.ListaDatiCalcoloContributivo != null)
                                            ArchiviaDatiCalcoloContributivoFS(prelievoFs.Risposta.ListaDatiCalcoloContributivo.ToList(), idPensione, dictionaryIdRecordFondo, tipoFondo,
                                                isDomandaConNuovaGestioneDatiFondoFSPT, Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione), datiPensioneMaster);
                                        ArchiviaDatiCalcoloRetributivo(prelievoFs.Risposta.DatiCalcoloRetributivo, idPensione);
                                        if (prelievoFs.Risposta.ListaDatiBeneficiParticolari != null)
                                            ArchiviaDatiBeneficiParticolari(prelievoFs.Risposta.ListaDatiBeneficiParticolari.ToList(), idPensione);
                                        if (new List<Utility.TipoFondo> { Utility.TipoFondo.FS, Utility.TipoFondo.PT }.Contains(tipoFondo.GetValueOrDefault()) &&
                                            prelievoFs.Risposta.ListaDatiServizioUtile707 != null)
                                        {
                                            ArchiviaDatiServizioUtile707(prelievoFs.Risposta.ListaDatiServizioUtile707.ToList(), idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione,
                                               isDomandaConNuovaGestioneDatiFondoFSPT);
                                        }
                                        if (prelievoFs.Risposta.ListaDatiServizioUtileINPDAP707 != null)
                                        {
                                            ArchiviaDatiServizioUtileINPDAP707(prelievoFs.Risposta.ListaDatiServizioUtileINPDAP707.ToList(), idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione);
                                        }
                                    }
                                    if (tipoFondo == Utility.TipoFondo.DZ)
                                    {
                                        if (prelievoFs.Risposta.DatiCalcoloRetributivoDZ.Any())
                                            ArchiviaDatiCalcoloRetributivoDZ(prelievoFs.Risposta.DatiCalcoloRetributivoDZ.ToList(), dictionaryIdRecordFondo, idPensione);
                                        //if (prelievoFs.Risposta.ListaDatiCalcoloContributivo.Any())
                                        //    ArchiviaDatiCalcoloContributivoDZ(prelievoFs.Risposta.ListaDatiCalcoloContributivo.ToList(), dictionaryIdRecordFondo, idPensione);
                                    }

                                    if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione)
                                        || (controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensioneMaster, isRiapertura)))
                                    {
                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);
                                    if (listaDatiCalcoloRetributivoStorico != null && listaDatiCalcoloRetributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloRetributivo(listaDatiCalcoloRetributivoStorico.First(), idPensione);
                                    if (listaDatiCalcoloContributivoStorico != null && listaDatiCalcoloContributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloContributivoFS(listaDatiCalcoloContributivoStorico, idPensione, dictionaryIdRecordFondo, tipoFondo, isDomandaConNuovaGestioneDatiFondoFSPT, Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione), datiPensioneMaster);
                                    if (listaDatiServizioUtileStorico != null && listaDatiServizioUtileStorico.Count > 0)
                                        ArchiviaDatiServizioUtile(listaDatiServizioUtileStorico, idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione,
                                             tipoFondo, isDomandaConNuovaGestioneDatiFondoFSPT);
                                    if (listaDatiOneriStorico != null && listaDatiOneriStorico.Count > 0)
                                        ArchiviaDatiOneri(listaDatiOneriStorico, idPensione);
                                    if (listaDatiBeneficiParticolariStorico != null && listaDatiBeneficiParticolariStorico.Count > 0)
                                        ArchiviaDatiBeneficiParticolari(listaDatiBeneficiParticolariStorico, idPensione);
                                    if (lOneriMaster != null && lOneriMaster.Count > 0)
                                        ArchiviaDatiOneri(lOneriMaster, idPensione);
                                    else if (prelievoFs.Risposta.ListaDatiOneri != null)
                                    {
                                        if (scadenzaBeneficioUnicarpe.HasValue)
                                        {
                                            foreach (GestioneOneri.DatiOneri onerePrelievo in prelievoFs.Risposta.ListaDatiOneri)
                                                onerePrelievo.ScadenzaBeneficio = scadenzaBeneficioUnicarpe;
                                        }

                                        ArchiviaDatiOneri(prelievoFs.Risposta.ListaDatiOneri.ToList(), idPensione);
                                    }
                                    if (lDatiBeneficiParticolariMaster != null && lDatiBeneficiParticolariMaster.Count > 0)
                                        ArchiviaDatiBeneficiParticolari(lDatiBeneficiParticolariMaster, idPensione);

                                    if (controlloFelpeMaster != null)
                                        GestioneDatiControlloFelpe.SalvaDatiControlloFelpe(datiPensioneMaster.Id, controlloFelpeMaster);

                                    if (listaQuoteMiglioramentiContrattualiMaster != null || prelievoFs.Risposta.DatiQuoteMiglioramentiContrattuali != null)
                                        ArchiviaDatiQuoteMiglioramentiContrattuali(listaQuoteMiglioramentiContrattualiMaster, prelievoFs.Risposta.DatiQuoteMiglioramentiContrattuali != null ? prelievoFs.Risposta.DatiQuoteMiglioramentiContrattuali.ToList() : null, idPensione);

                                    if (tipoFondo == Utility.TipoFondo.PL)
                                        ArchiviaDatiInabilita(prelievoFs.Risposta.DatiInabilita, idPensione);
                                }
                                else
                                {
                                    prelievoFs.Risposta.DatiPensione.CodiceSedeDestinazione = null;
                                    prelievoFs.Risposta.DatiPensione.CentroOperativoDestinazione = null;

                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoFs.Risposta.DatiPensione, null, out idPensione);
                                    if (!Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione) && tipoFondo.HasValue && tipoFondo.Value != Utility.TipoFondo.FS &&
                                        tipoFondo.Value != Utility.TipoFondo.PT && datiPensioneMaster.TrasformazioneAOI.GetValueOrDefault() && Utility.IsDomandaReversibilita(datiPensioneMaster))
                                        ArchiviaDatiIstruttoria(datiIstruttoriaMaster, prelievoFs.Risposta.DatiIstruttoria, idPensione);
                                    else
                                        ArchiviaDatiIstruttoria(datiIstruttoriaMaster, null, idPensione);
                                    ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, prelievoFs.Risposta.DatiMaggiorazioniBenefici, idPensione);
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    if (prelievoFs.Risposta.ListaSupplementi != null)
                                        ArchiviaDatiSupplementi(prelievoFs.Risposta.ListaSupplementi.ToList(), idPensione);

                                    if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione))
                                    {
                                        if (Utility.IsDomandaReversibilita(datiPensioneMaster) && Utility.IsDomandaPL(datiPensioneMaster) && prelievoFs.Risposta.ListaRecordDatiFondoINPDAP != null)
                                        {
                                            foreach (GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoINPDAP_Prelievo in prelievoFs.Risposta.ListaRecordDatiFondoINPDAP.ToList())
                                            {
                                                datiRecordFondoINPDAP_Prelievo.ScadenzaBenefici = null;
                                                datiRecordFondoINPDAP_Prelievo.ScadenzaIllimitata = null;

                                            }
                                        }
                                        ArchiviaDatiFondoINPDAP(prelievoFs.Risposta.DatiPensioneFondoDatiGenerici,
                                    prelievoFs.Risposta.ListaRecordFondo != null ? prelievoFs.Risposta.ListaRecordFondo.ToList() : null,
                                    prelievoFs.Risposta.ListaDatiPensioneINPDAP != null ? prelievoFs.Risposta.ListaDatiPensioneINPDAP.ToList() : null,
                                    prelievoFs.Risposta.ListaRecordDatiFondoINPDAP != null ? prelievoFs.Risposta.ListaRecordDatiFondoINPDAP.ToList() : null,
                                    datiPensioneINPDAPMaster, recordDatiFondoINPDAPMaster, listaRecordDatiFondoGP,
                                    idPensione, out idFondo, out dictionaryIdRecordFondo);
                                    }
                                    else
                                    {
                                        ArchiviaDatiFondo(tipoFondo, prelievoFs.Risposta.DatiPensioneFondoDatiGenerici, prelievoFs.Risposta.DatiFondoSpecifico, datiFondoSpecificoFELPE,
                                        prelievoFs.Risposta.ListaRecordFondo != null ? prelievoFs.Risposta.ListaRecordFondo.ToList() : null, idPensione, isDomandaConNuovaGestioneDatiFondoFSPT,
                                         prelievoFs.Risposta.DatiDanteCausa, prelievoFs.Risposta.DatiLavorazione, datiPensioneMaster, out idFondo, out dictionaryIdRecordFondo);
                                    }

                                    if (prelievoFs.Risposta.ListaDatiCalcoloContributivo != null)
                                        ArchiviaDatiCalcoloContributivoFS(prelievoFs.Risposta.ListaDatiCalcoloContributivo.ToList(), idPensione, dictionaryIdRecordFondo, tipoFondo,
                                            isDomandaConNuovaGestioneDatiFondoFSPT, Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione), datiPensioneMaster);
                                    if (tipoFondo == Utility.TipoFondo.DZ)
                                    {
                                        ArchiviaDatiCalcoloRetributivoDZ(prelievoFs.Risposta.DatiCalcoloRetributivoDZ.ToList(), dictionaryIdRecordFondo, idPensione);
                                    }
                                    ArchiviaDatiCalcoloRetributivo(prelievoFs.Risposta.DatiCalcoloRetributivo, idPensione);
                                    if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione))
                                    {
                                        if (listaDatiServizioUtileINPDAP != null && listaDatiServizioUtileINPDAP.Count > 0)
                                            ArchiviaDatiServizioUtileINPDAP(listaDatiServizioUtileINPDAP, dictionaryIdRecordFondo, idPensione);
                                        else if (prelievoFs.Risposta.ListaDatiServizioUtileINPDAP != null)
                                            ArchiviaDatiServizioUtileINPDAP(prelievoFs.Risposta.ListaDatiServizioUtileINPDAP.ToList(), dictionaryIdRecordFondo, idPensione);
                                        if (prelievoFs.Risposta.ListaDatiServizioUtileINPDAP707 != null)
                                        {
                                            ArchiviaDatiServizioUtileINPDAP707(prelievoFs.Risposta.ListaDatiServizioUtileINPDAP707.ToList(), idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione);
                                        }
                                    }
                                    else
                                    {
                                        if (prelievoFs.Risposta.ListaDatiServizioUtile != null)
                                            ArchiviaDatiServizioUtile(prelievoFs.Risposta.ListaDatiServizioUtile.ToList(), idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione,
                                                tipoFondo, isDomandaConNuovaGestioneDatiFondoFSPT);

                                        if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)
                                           && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensioneMaster, prelievoFs.Risposta.DatiDanteCausa, prelievoFs.Risposta.DatiLavorazione))
                                        {
                                            if (prelievoFs.Risposta.ListaDatiServizioUtile707 != null && prelievoFs.Risposta.ListaDatiServizioUtile707.Count() > 0)
                                            {
                                                ArchiviaDatiServizioUtile707(prelievoFs.Risposta.ListaDatiServizioUtile707.ToList(), idFondo, dictionaryIdRecordFondo, tipoAppartenenza, idPensione,
                                                isDomandaConNuovaGestioneDatiFondoFSPT);
                                            }
                                        }
                                    }
                                    if (CFfamiliari != null && CFfamiliari.Count > 0)
                                    {
                                        //rimosso salvataggio da prelievo che avveniva per le REV PI
                                        //Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensioneMaster.SiglaCategoria);
                                        //if (prelievoFs != null && prelievoFs.Risposta != null && prelievoFs.Risposta.ListaFamiliari != null && categoriaFondoPI != null)
                                        //     ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, prelievoFs.Risposta.ListaFamiliari, out errori);
                                        //else
                                        
                                        ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione)
                                        || (controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensioneMaster, isRiapertura)))
                                    {
                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    ArchiviaDatiSindacato(datiSindacatoMaster, null, idPensione);

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                                    ArchiviaDatiDL407(prelievoFs.Risposta.DatiDL407, idPensione);

                                    if (datiPagamentoWebDom != null)
                                        GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);
                                }

                                return;
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        if (AreaPrelievo != null)
                        {
                            ServiceReferences.LiquidazioneCi.AreaPrelievo prelievoCi = (ServiceReferences.LiquidazioneCi.AreaPrelievo)AreaPrelievo;
                            if (prelievoCi != null && prelievoCi.Risposta != null)
                            {
                                long idPensione = 0;
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura || tipoDomanda == Utility.TipoDomanda.Ripristino)
                                {
                                    //ENG - Le Ape Precoci non sono più polarizzate
                                    prelievoCi.Risposta.DatiPensione.CodiceSedeDestinazione = null;
                                    prelievoCi.Risposta.DatiPensione.CentroOperativoDestinazione = null;
                                    if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.CI) && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura))
                                    {
                                        datiPensioneMaster.CodiceSede = sedeChiavePensione;
                                    }

                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoCi.Risposta.DatiPensione, prelievoCi.Risposta.DatiPensioniCiDatiGenerici != null ? prelievoCi.Risposta.DatiPensioniCiDatiGenerici.ContributiItalianiEdEsteriAl1295 : null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, prelievoCi.Risposta.DatiIstruttoria, idPensione);
                                    ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, prelievoCi.Risposta.DatiMaggiorazioniBenefici, idPensione);
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, prelievoCi.Risposta.DatiNuoveLiquidate, idPensione);
                                    ArchiviaDatiPensioniCiDatiGenerici(datiPensioniDatiGenericiMaster, prelievoCi.Risposta.DatiPensioniCiDatiGenerici, idPensione);
                                    ArchiviaDatiDetrazioni(prelievoCi.Risposta.DatiDetrazioni, idPensione);
                                    ArchiviaDatiPagamento(prelievoCi.Risposta.DatiPagamento, datiPagamentoWebDom, idPensione);
                                    if (prelievoCi.Risposta.ListaBititolarita != null)
                                        ArchiviaDatiBititolarita(prelievoCi.Risposta.ListaBititolarita.ToList(), idPensione);
                                    List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari = null;
                                    ArchiviaDatiFamiliare(parametriArca, prelievoCi.Risposta.ListaFamiliari, anagraficaTitolare, tipoAppartenenza, tipoDomanda, datiPensioneMaster, isRiapertura, out familiariDaPrelievo,
                                        out isFamiliariVerde, out listaFamiliari, danteCausa, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        return;
                                    if (Utility.IsDomandaUnicarpe(datiPensioneMaster, true) != Utility.TipoUnicarpe.Automatica)
                                    {
                                        if (prelievoCi.Risposta.ListaCalcoloContributivo != null)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoCi.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        if (prelievoCi.Risposta.ListaCalcoloRetributivo != null)
                                            ArchiviaDatiCalcoloRetributivo(prelievoCi.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                    }
                                    if (listaDatiCalcoloRetributivoStorico != null && listaDatiCalcoloRetributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloRetributivo(listaDatiCalcoloRetributivoStorico, idPensione);
                                    if (listaDatiCalcoloContributivoStorico != null && listaDatiCalcoloContributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloContributivoAGO_CI(listaDatiCalcoloContributivoStorico, idPensione);
                                    if (prelievoCi.Risposta.ListaSupplementi != null)
                                        ArchiviaDatiSupplementi(prelievoCi.Risposta.ListaSupplementi.ToList(), idPensione);
                                    if (prelievoCi.Risposta.ListaResidenzeEstere != null)
                                        ArchiviaDatiResidenzeEstere(prelievoCi.Risposta.ListaResidenzeEstere.ToList(), idPensione, anagraficaTitolare.Id);
                                    if (prelievoCi.Risposta.ListaStatiCivili != null)
                                    {
                                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                                            ArchiviaDatiStatiCiviliRicTrfCI(prelievoCi.Risposta.ListaStatiCivili.ToList(), idPensione, datiDomanda, anagraficaTitolare.Id);
                                        else
                                            ArchiviaDatiStatiCivili(prelievoCi.Risposta.ListaStatiCivili.ToList(), idPensione, anagraficaTitolare.Id);
                                    }
                                    //ArchiviaDatiDelegato(parametriArca, prelievoCi.Risposta.DatiDelegato, tipoAppartenenza, datiPensioneMaster);
                                    ArchiviaDatiTutore(parametriArca, prelievoCi.Risposta.DatiTutore, tipoAppartenenza, idPensione, datiPensioneMaster.NDomus.ToString());
                                    ArchiviaDatiVittimeTerrorismo(prelievoCi.Risposta.DatiVittimeTerrorismo, idPensione);
                                    if (prelievoCi.Risposta.ListaPensioniCiImportiValuta != null)
                                        ArchiviaDatiPensioniCIImportiValuta(prelievoCi.Risposta.ListaPensioniCiImportiValuta.ToList(), idPensione);
                                    ArchiviaDatiIntegrazioneArt11(prelievoCi.Risposta.DatiIntegrazioneArt11, idPensione);
                                    if (prelievoCi.Risposta.ListaCalcoloContributivoEstero != null)
                                        ArchiviaDatiCalcoloContributivoEstero(prelievoCi.Risposta.ListaCalcoloContributivoEstero.ToList(), idPensione);
                                    if (prelievoCi.Risposta.ListaPensioniCiMaternitaAcna != null)
                                        ArchiviaDatiPensioniCiMaternitaAcna(prelievoCi.Risposta.ListaPensioniCiMaternitaAcna.ToList(), idPensione);
                                    if (listaStatiEsteriCiStorico != null)
                                        ArchiviaDatiStatiEsteri(listaStatiEsteriCiStorico, idPensione);
                                    // ENG - Per le TRF automatiche i campi 'settimane misura a decorrenza pensione' e 'settimane diritto' 
                                    //       devono essere valorizzati con quello che arriva da Unicarpe nei campi CI_Misest e CI_Direst
                                    if (isRiapertura && Utility.IsDomandaUnicarpe(datiPensioneMaster, true) == Utility.TipoUnicarpe.Automatica)
                                    {
                                        if (prelievoCi.Risposta.ListaStatiEsteri != null && istituzioniEsterePECO != null)
                                            ArchiviaDatiStatiEsteriPerTrfAutomatiche(prelievoCi.Risposta.ListaStatiEsteri.ToList(), idPensione, istituzioniEsterePECO);
                                    }
                                    else
                                    {
                                        if (prelievoCi.Risposta.ListaStatiEsteri != null)
                                            ArchiviaDatiStatiEsteri(prelievoCi.Risposta.ListaStatiEsteri.ToList(), idPensione);
                                    }

                                    ArchiviaDatiEliminazione(prelievoCi.Risposta.DatiEliminazione, idPensione);
                                    if (prelievoCi.Risposta.ListaDatiOneri != null)
                                    {
                                        if (scadenzaBeneficioUnicarpe.HasValue)
                                        {
                                            foreach (GestioneOneri.DatiOneri onerePrelievo in prelievoCi.Risposta.ListaDatiOneri)
                                                onerePrelievo.ScadenzaBeneficio = scadenzaBeneficioUnicarpe;
                                        }

                                        ArchiviaDatiOneri(prelievoCi.Risposta.ListaDatiOneri.ToList(), idPensione);
                                    }
                                    if (prelievoCi.Risposta.ListaDatiBeneficiParticolari != null)
                                        ArchiviaDatiBeneficiParticolari(prelievoCi.Risposta.ListaDatiBeneficiParticolari.ToList(), idPensione);

                                    //ENG - RIC/TRF ArchiviaDatiSindacato prelevati
                                    ArchiviaDatiSindacato(datiSindacatoMaster, prelievoCi.Risposta.DatiSindacato != null ? prelievoCi.Risposta.DatiSindacato : null, idPensione);
                                    if (controlloFelpeMaster != null)
                                        GestioneDatiControlloFelpe.SalvaDatiControlloFelpe(datiPensioneMaster.Id, controlloFelpeMaster);

                                    //ENG - RIC/TRF
                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                                    //ENG - Reversibilità: campi inail (RIC/TRF),
                                    //ENG - Inabilità e Assegni ordinari: gestione tab Inail (RIC/TRF)
                                    if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensioneMaster, prelievoCi.Risposta.DatiDanteCausa) || Utility.IsDomandaPensioneInabilita(datiPensioneMaster)
                                        || Utility.IsAssegnoInvalidita(datiPensioneMaster) || isTabInailVisibleForCI)
                                    {
                                        if (prelievoCi.Risposta.ListaDatiInail != null && prelievoCi.Risposta.ListaDatiInail.Count() > 0)
                                            ArchiviaDatiInail(prelievoCi.Risposta.ListaDatiInail.ToList(), idPensione);
                                    }
                                    //ENG - Superstiti RIC/TRF: prelevare i valori dei campi: ICISEN2, ICISEN3A e ICISEN3M e poi rimandarli al calcolo. Il campo ICISEN3A(Anno reddito) non deve essere editabile
                                    if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensioneMaster))
                                    {
                                        if (prelievoCi.Risposta.ListaDatiSentenza495_93 != null)
                                            ArchiviaDatiRedditiSentenza495_93(prelievoCi.Risposta.ListaDatiSentenza495_93.ToList(), idPensione, datiPensioneMaster);
                                    }
                                }
                                else
                                {
                                    //ENG - Implementata la gestione mancante per le Reversibilità
                                    if (Utility.IsDomandaReversibilita(datiPensioneMaster))
                                    {

                                        ArchiviaDatiPensione(datiPensioneMaster, prelievoCi.Risposta.DatiPensione, prelievoCi.Risposta.DatiPensioniCiDatiGenerici != null ? prelievoCi.Risposta.DatiPensioniCiDatiGenerici.ContributiItalianiEdEsteriAl1295 : null, out idPensione);

                                        ArchiviaDatiIstruttoria(datiIstruttoriaMaster, prelievoCi.Risposta.DatiIstruttoria, idPensione);

                                        if (prelievoCi.Risposta.ListaCalcoloContributivo != null)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoCi.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        if (prelievoCi.Risposta.ListaCalcoloRetributivo != null)
                                            ArchiviaDatiCalcoloRetributivo(prelievoCi.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                        if (prelievoCi.Risposta.DatiMaggiorazioniBenefici != null)
                                            ArchiviaDatiMaggiorazioneBenefici(prelievoCi.Risposta.DatiMaggiorazioniBenefici, null, idPensione);
                                        //ENG - Reversibilita CI
                                        if (prelievoCi.Risposta.ListaCalcoloContributivoEstero != null)
                                            ArchiviaDatiCalcoloContributivoEstero(prelievoCi.Risposta.ListaCalcoloContributivoEstero.ToList(), idPensione);

                                        //ENG - Reversibilità: campi Inail (PL)
                                        if (prelievoCi.Risposta.ListaDatiInail != null && prelievoCi.Risposta.ListaDatiInail.Count() > 0)
                                            ArchiviaDatiInail(prelievoCi.Risposta.ListaDatiInail.ToList(), idPensione);
                                        if (prelievoCi.Risposta.ListaSupplementi != null && Utility.IsDomandaPL(datiPensioneMaster))
                                            ArchiviaDatiSupplementi(prelievoCi.Risposta.ListaSupplementi.ToList(), idPensione);
                                    }
                                    else
                                    {
                                        if (Utility.IsDomandaRipristinoOrRiliquidazioneSuperstiti(datiPensioneMaster) && (datiPensioneMaster.Tipo == "0026" || datiPensioneMaster.Tipo == "0027"))
                                        {
                                            datiPensioneMaster.DecorrenzaOriginaria = null;
                                            datiPensioneMaster.DecorrenzaOriginariaPrima = null;
                                        }
                                        ArchiviaDatiPensione(datiPensioneMaster, null, prelievoCi.Risposta.DatiPensioniCiDatiGenerici != null ? prelievoCi.Risposta.DatiPensioniCiDatiGenerici.ContributiItalianiEdEsteriAl1295 : null, out idPensione);
                                        ArchiviaDatiIstruttoria(datiIstruttoriaMaster, null, idPensione);
                                        ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, null, idPensione);
                                    }

                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);

                                    if (Utility.IsDomandaReversibilita(datiPensioneMaster))
                                    {
                                        if (prelievoCi.Risposta.DatiPensioniCiDatiGenerici != null)
                                            ArchiviaDatiPensioniCiDatiGenerici(datiPensioniDatiGenericiMaster, prelievoCi.Risposta.DatiPensioniCiDatiGenerici, idPensione);
                                    }
                                    else
                                        ArchiviaDatiPensioniCiDatiGenerici(datiPensioniDatiGenericiMaster, null, idPensione);

                                    if (CFfamiliari != null)
                                    {
                                        ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    ArchiviaDatiSindacato(datiSindacatoMaster, null, idPensione);

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                                    if (datiPagamentoWebDom != null)
                                        GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);
                                }
                                return;
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        if (AreaPrelievo != null)
                        {
                            ServiceReferences.LiquidazioneAgo.AreaPrelievo prelievoAgo = (ServiceReferences.LiquidazioneAgo.AreaPrelievo)AreaPrelievo;
                            if (prelievoAgo != null && prelievoAgo.Risposta != null)
                            {
                                long idPensione = 0;
                                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti ||
                                    isRiapertura)
                                {
                                    // Vengono impostati a null perchè per le ricostituzioni ci arrivano dei dati errati dal prelievo
                                    // Rif. mail con oggetto "RE: Analisi RIC AGO" del 07/08/2014
                                    prelievoAgo.Risposta.DatiPensione.RequisitiAl1294 = null;
                                    prelievoAgo.Risposta.DatiPensione.RequisitiAl996 = null;
                                    prelievoAgo.Risposta.DatiPensione.RequisitiVecchiaiaAl1294 = null;
                                    ///////////////////////////////////////////////////////////////////////////////////////////////////

                                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                                    // Pulizia dei dati sporchi che sono stati recuperati dal prelievo 
                                    datiPensioneMaster.NaturaPensione = prelievoAgo.Risposta.DatiPensione.NaturaPensione;
                                    if (!Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoria = null;
                                    if (!Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0171 = null;
                                    if (!Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoriaPerTipo0179 = null;
                                    if (!Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiIstruttoria.CodiceAziendaEditoriaLetteraB = null;
                                    if (Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensioneMaster) || Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensioneMaster) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensioneMaster))
                                        prelievoAgo.Risposta.DatiPensione.CodiceBancaEsodati = null;
                                    if (!Utility.IsDomandaCumulo(datiPensioneMaster.SiglaCategoria) && !Utility.IsDomandaTotalizzazione(datiPensioneMaster.SiglaCategoria))
                                        prelievoAgo.Risposta.DatiPensioniDatiGenerici.EnteCassa = null;
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////

                                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                                    // Per tutte le pensioni differenti da INPDAI viene impostato codice Gestione a Null poichè 
                                    // al prelievo non ci è possibile determinare se è un ex-Inpdai oppure no.
                                    if (!Utility.IsDomandaINPDAI(datiPensioneMaster.SiglaCategoria))
                                    {
                                        if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Length > 0)
                                        {
                                            foreach (var elem in prelievoAgo.Risposta.ListaCalcoloRetributivo)
                                                elem.CodiceTipoQuota = null;
                                        }

                                        if (listaDatiCalcoloRetributivoStorico != null && listaDatiCalcoloRetributivoStorico.Count > 0)
                                        {
                                            foreach (var elem in listaDatiCalcoloRetributivoStorico)
                                                elem.CodiceTipoQuota = null;
                                        }
                                    }

                                    if (!Utility.IsDomandaAPEPrecoci(datiPensioneMaster) && !Utility.IsDomandaAPESociale(datiPensioneMaster.SiglaCategoria))
                                    {
                                        prelievoAgo.Risposta.DatiPensione.CodiceSedeDestinazione = null;
                                        prelievoAgo.Risposta.DatiPensione.CentroOperativoDestinazione = null;
                                        if (Utility.IsPensioniOvunqueAttiva(Utility.TipoAppartenenza.AGO) && (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura))
                                        {
                                            datiPensioneMaster.CodiceSede = sedeChiavePensione;
                                        }
                                    }
                                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                                    if (maxDecDatiCalcoloAnte96.HasValue)
                                    {
                                        var tipoAnte96 = Utility.IsDomandaAnte96(datiPensioneMaster, datiPensioneMaster, danteCausa, isRiapertura, maxDecDatiCalcoloAnte96);
                                        datiPensioneMaster.Ante96ByDatiCalcolo = (byte?)tipoAnte96;
                                        datiPensioneMaster.MaxDecDatiCalcoloAnte96 = maxDecDatiCalcoloAnte96;
                                    }
                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoAgo.Risposta.DatiPensione, null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, prelievoAgo.Risposta.DatiIstruttoria, idPensione);
                                    //ENG - RIC REVERSIBILITA
                                    if (!(tipoDomanda == Utility.TipoDomanda.Ricostituzione && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensioneMaster, danteCausa) && !Utility.IsDomandaCumulo(datiPensioneMaster.SiglaCategoria)
                                        && datiStoricoGP != null && (datiStoricoGP.TipoSettimaneBeneficio == "14" || datiStoricoGP.TipoSettimaneBeneficio == "18" || datiStoricoGP.TipoSettimaneBeneficio == "19" || datiStoricoGP.TipoSettimaneBeneficio == "12" || datiStoricoGP.TipoSettimaneBeneficio == "24")))
                                    {
                                        ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, prelievoAgo.Risposta.DatiMaggiorazioniBenefici, idPensione);
                                    }
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    if (tipoDomanda != Utility.TipoDomanda.Ripristino && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti)
                                        ArchiviaDatiPagamento(prelievoAgo.Risposta.DatiPagamento, datiPagamentoWebDom, idPensione);
                                    List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari = null;
                                    ArchiviaDatiFamiliare(parametriArca, prelievoAgo.Risposta.ListaFamiliari, anagraficaTitolare, tipoAppartenenza, tipoDomanda, datiPensioneMaster, isRiapertura, out familiariDaPrelievo,
                                        out isFamiliariVerde, out listaFamiliari, danteCausa, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        return;
                                    if (prelievoAgo.Risposta.ListaResidenzeEstere != null)
                                        ArchiviaDatiResidenzeEstere(prelievoAgo.Risposta.ListaResidenzeEstere.ToList(), idPensione, anagraficaTitolare.Id);
                                    if (prelievoAgo.Risposta.ListaStatiCivili != null)
                                        ArchiviaDatiStatiCivili(prelievoAgo.Risposta.ListaStatiCivili.ToList(), idPensione, anagraficaTitolare.Id);

                                    //ENG - Spacchettate SOPGI
                                    if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
                                    {
                                        if (prelievoAgo.Risposta.ListaSupplementi != null && prelievoAgo.Risposta.ListaSupplementi.Count() > 0)
                                            ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        else if (datiSupplementiSOPGI != null && datiSupplementiSOPGI.Count > 0)
                                            ArchiviaDatiSupplementi(datiSupplementiSOPGI, idPensione);
                                    }
                                    else if (Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura)
                                        || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
                                    {
                                        if (prelievoAgo.Risposta.ListaSupplementi != null && prelievoAgo.Risposta.ListaSupplementi.Count() > 0)
                                            ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        else if (datiSupplementiSpacchettateAGO != null && datiSupplementiSpacchettateAGO.Count > 0)
                                            ArchiviaDatiSupplementi(datiSupplementiSpacchettateAGO, idPensione);
                                    }
                                    else
                                    {
                                        //ENG - MEMO 50/2023
                                        if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" &&
                                             Utility.IsRicostituzione_MotiviContributivi(datiPensioneMaster) && datiPensioneMaster.Tipo == "0001" && !Utility.IsDomandaENPALS(datiPensioneMaster.SiglaCategoria) &&
                                             !Utility.IsDomandaCumulo(datiPensioneMaster.SiglaCategoria) && !(Utility.IsDomandaAnte96(datiPensioneMaster, datiPensioneMaster, danteCausa, isRiapertura) != null))
                                        {
                                            if (listaDatiSupplementiStorico != null && listaDatiSupplementiStorico.Count > 0)
                                                ArchiviaDatiSupplementiStorico(listaDatiSupplementiStorico.ToList(), prelievoAgo.Risposta.ListaSupplementi != null ? prelievoAgo.Risposta.ListaSupplementi.ToList() : null, idPensione);
                                        }
                                        else
                                        {
                                            if (prelievoAgo.Risposta.ListaSupplementi != null)
                                                ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        }

                                    }

                                    //ArchiviaDatiDelegato(parametriArca, prelievoAgo.Risposta.DatiDelegato, tipoAppartenenza, datiPensioneMaster);
                                    ArchiviaDatiTutore(parametriArca, prelievoAgo.Risposta.DatiTutore, tipoAppartenenza, idPensione, datiPensioneMaster.NDomus.ToString());

                                    if (Utility.IsDomandaUnicarpe(datiPensioneMaster, true) != Utility.TipoUnicarpe.Automatica)
                                    {
                                        //ENG - Spacchettate SOPGI
                                        if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
                                        {
                                            if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Count() > 0)
                                                ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                            else if (datiCalcoloRetributivoSOPGI != null && datiCalcoloRetributivoSOPGI.Count > 0)
                                                ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSOPGI, idPensione);
                                        }
                                        else if (Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura)
                                                 || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
                                        {
                                            if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Count() > 0)
                                                ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                            else if (datiCalcoloRetributivoSpacchettateAGO != null && datiCalcoloRetributivoSpacchettateAGO.Count > 0)
                                                ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSpacchettateAGO, idPensione);
                                        }
                                        else
                                        {
                                            if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null)
                                                ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                        }

                                        //ENG  - Spacchettate SOPGI
                                        if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
                                        {
                                            if (prelievoAgo.Risposta.ListaDatiRetributiviINPGI != null && prelievoAgo.Risposta.ListaDatiRetributiviINPGI.Count() > 0)
                                                ArchiviaDatiCalcoloRetributivoINPGI(prelievoAgo.Risposta.ListaDatiRetributiviINPGI.ToList(), idPensione);
                                            else if (datiCalcoloRetributivoQuotaFondoSOPGI != null && datiCalcoloRetributivoQuotaFondoSOPGI.Count > 0)
                                                ArchiviaDatiCalcoloRetributivoINPGI(datiCalcoloRetributivoQuotaFondoSOPGI, idPensione);
                                        }
                                        else
                                        {
                                            if (prelievoAgo.Risposta.ListaDatiRetributiviINPGI != null)
                                                ArchiviaDatiCalcoloRetributivoINPGI(prelievoAgo.Risposta.ListaDatiRetributiviINPGI.ToList(), idPensione);
                                        }

                                        //ENG  - Spacchettate SOPGI
                                        if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
                                        {
                                            if (prelievoAgo.Risposta.ListaDatiContributiviINPGI != null && prelievoAgo.Risposta.ListaDatiContributiviINPGI.Count() > 0)
                                                ArchiviaDatiCalcoloContributivoINPGI(prelievoAgo.Risposta.ListaDatiContributiviINPGI.ToList(), idPensione);
                                            else if (datiCalcoloContributivoQuotaFondoSOPGI != null && datiCalcoloContributivoQuotaFondoSOPGI.Count > 0)
                                                ArchiviaDatiCalcoloContributivoINPGI(datiCalcoloContributivoQuotaFondoSOPGI, idPensione);
                                        }
                                        else
                                        {
                                            if (prelievoAgo.Risposta.ListaDatiContributiviINPGI != null)
                                                ArchiviaDatiCalcoloContributivoINPGI(prelievoAgo.Risposta.ListaDatiContributiviINPGI.ToList(), idPensione);
                                        }

                                        if (Utility.IsDomandaCumulo(datiPensioneMaster.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensioneMaster.SiglaCategoria))
                                        {
                                            ArchiviaDatiQuotePensione(listaDatiQuotePensioneTotalIVS, prelievoAgo.Risposta.ListaQuotePensione != null ? prelievoAgo.Risposta.ListaQuotePensione.ToList() : null, idPensione);
                                            ArchiviaDatiTrattenuteQuotePensione(listaDatiTrattenuteQuotePensioneTotalIVS, prelievoAgo.Risposta.ListaTrattenuteQuotePensione != null ? prelievoAgo.Risposta.ListaTrattenuteQuotePensione.ToList() : null, idPensione);
                                            if (Utility.IsDomandaMiglioramentiContrattuali(datiPensioneMaster))
                                            {
                                                ArchiviaDatiMiglioramentiContrattuali(miglioramentiContrattualiMaster, idPensione);
                                                ArchiviaDatiQuoteMiglioramentiContrattuali(listaQuoteMiglioramentiContrattualiMaster, prelievoAgo.Risposta.ListaQuoteMiglioramentiContrattuali != null ? prelievoAgo.Risposta.ListaQuoteMiglioramentiContrattuali.ToList() : null, idPensione);
                                            }
                                        }
                                        else if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa)) //ENG - Spacchettate SOPGI
                                        {
                                            if (prelievoAgo.Risposta.ListaCalcoloContributivo != null && prelievoAgo.Risposta.ListaCalcoloContributivo.Count() > 0)
                                                ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                            else if (datiCalcoloContributivoSOPGI != null && datiCalcoloContributivoSOPGI.Count > 0)
                                                ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSOPGI, idPensione);
                                        }
                                        else if (Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura)
                                            || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
                                        {
                                            if (prelievoAgo.Risposta.ListaCalcoloContributivo != null && prelievoAgo.Risposta.ListaCalcoloContributivo.Count() > 0)
                                                ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                            else if (datiCalcoloContributivoSpacchettateAGO != null && datiCalcoloContributivoSpacchettateAGO.Count > 0)
                                                ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSpacchettateAGO, idPensione);
                                        }
                                        else
                                        {
                                            if (prelievoAgo.Risposta.ListaCalcoloContributivo != null)
                                                ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        }

                                        if (prelievoAgo.Risposta.ListaDatiBeneficiParticolari != null)
                                            ArchiviaDatiBeneficiParticolari(prelievoAgo.Risposta.ListaDatiBeneficiParticolari.ToList(), idPensione);
                                    }

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);
                                    if (listaDatiCalcoloRetributivoStorico != null && listaDatiCalcoloRetributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloRetributivo(listaDatiCalcoloRetributivoStorico, idPensione);
                                    if (Utility.IsDomandaCumulo(datiPensioneMaster.SiglaCategoria))
                                    {
                                        ArchiviaDatiQuotePensione(listaDatiQuotePensioneStorico, null, idPensione);
                                        ArchiviaDatiTrattenuteQuotePensione(listaDatiTrattenuteQuotePensioneStorico, null, idPensione);
                                        //mettere if su base prestazione
                                        ArchiviaDatiQuoteMiglioramentiContrattuali(listaQuoteMiglioramentiContrattualiStorico, null, idPensione);
                                        if (Utility.IsRicostituzione(datiPensioneMaster.Gruppo) || isRiapertura)
                                        {
                                            ArchiviaDatiSupplementiCumulo(listaDatiSupplementiCumuloStorico, null, idPensione);
                                            if (datiPensioneMaster.IsCumuloAutomatica.GetValueOrDefault() && Utility.IsDomandaSupplementoCumulo(datiPensioneMaster))
                                            {
                                                ArchiviaDatiSupplementiCumuloAutomatiche(listaDatiSupplementiCumuloTotalIVS, prelievoAgo.Risposta.ListaSupplementiCumulo != null ? prelievoAgo.Risposta.ListaSupplementiCumulo.ToList() : null, idPensione);
                                            }
                                            else if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(datiPensioneMaster)) //ENG - Memo 32_a/2018
                                                ArchiviaDatiSupplementiCumulo(listaDatiSupplementiCumuloTotalIVS, null, idPensione);
                                            else
                                                ArchiviaDatiSupplementiCumulo(listaDatiSupplementiCumuloTotalIVS, prelievoAgo.Risposta.ListaSupplementiCumulo != null ? prelievoAgo.Risposta.ListaSupplementiCumulo.ToList() : null, idPensione);
                                        }
                                        //ENG - MEMO 74_2023
                                        if (ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI" && Utility.IsDomandaVOCUM(datiPensioneMaster.SiglaCategoria) &&
                                            !string.IsNullOrEmpty(datiPensioneMaster.NaturaPensione) && datiPensioneMaster.NaturaPensione.Substring(2, 1) == "V")
                                        {
                                            if (listaStatiEsteriCumuloStorico != null)
                                                ArchiviaDatiEsteriCumulo(listaStatiEsteriCumuloStorico, idPensione);

                                            if (Utility.IsRicostituzione(datiPensioneMaster.Gruppo) || isRiapertura)
                                                ArchiviaDatiEsteriCumulo(prelievoAgo.Risposta.ListaStatiEsteri != null ? prelievoAgo.Risposta.ListaStatiEsteri.ToList() : null, idPensione);
                                        }
                                    }
                                    if (Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensioneMaster) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensioneMaster) ||
                                        Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(datiPensioneMaster))
                                    {
                                        //ENG - Memo 116/2025       
                                        if (listaStatiEsteriCumuloStorico != null)
                                            ArchiviaDatiEsteriCumulo(listaStatiEsteriCumuloStorico, idPensione);

                                        if (Utility.IsRicostituzione(datiPensioneMaster.Gruppo) || isRiapertura)
                                            ArchiviaDatiEsteriCumulo(prelievoAgo.Risposta.ListaStatiEsteri != null ? prelievoAgo.Risposta.ListaStatiEsteri.ToList() : null, idPensione);
                                    }
                                    if (Utility.IsDomandaTotalizzazione(datiPensioneMaster.SiglaCategoria) && (Utility.IsRicostituzione(datiPensioneMaster.Gruppo) || isRiapertura))
                                    {
                                        ArchiviaDatiSupplementiCumulo(listaDatiSupplementiCumuloStorico, null, idPensione);
                                        ArchiviaDatiSupplementiCumulo(listaDatiSupplementiCumuloTotalIVS, prelievoAgo.Risposta.ListaSupplementiCumulo != null ? prelievoAgo.Risposta.ListaSupplementiCumulo.ToList() : null, idPensione);
                                    }
                                    else if (listaDatiCalcoloContributivoStorico != null && listaDatiCalcoloContributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloContributivoAGO_CI(listaDatiCalcoloContributivoStorico, idPensione);
                                    if (listaDatiOneriStorico != null && listaDatiOneriStorico.Count > 0)
                                        ArchiviaDatiOneri(listaDatiOneriStorico, idPensione);
                                    if (listaDatiBeneficiParticolariStorico != null && listaDatiBeneficiParticolariStorico.Count > 0)
                                        ArchiviaDatiBeneficiParticolari(listaDatiBeneficiParticolariStorico, idPensione);
                                    if (lOneriMaster != null && lOneriMaster.Count > 0)
                                        ArchiviaDatiOneri(lOneriMaster, idPensione);
                                    else if (prelievoAgo.Risposta.ListaDatiOneri != null)
                                    {
                                        if (scadenzaBeneficioUnicarpe.HasValue)
                                        {
                                            foreach (GestioneOneri.DatiOneri onerePrelievo in prelievoAgo.Risposta.ListaDatiOneri)
                                                onerePrelievo.ScadenzaBeneficio = scadenzaBeneficioUnicarpe;
                                        }

                                        ArchiviaDatiOneri(prelievoAgo.Risposta.ListaDatiOneri.ToList(), idPensione);
                                    }
                                    if (lDatiBeneficiParticolariMaster != null && lDatiBeneficiParticolariMaster.Count > 0)
                                        ArchiviaDatiBeneficiParticolari(lDatiBeneficiParticolariMaster, idPensione);
                                    if (prelievoAgo.Risposta.DatiSindacato != null)
                                        prelievoAgo.Risposta.DatiSindacato.IsFromService = true;
                                    ArchiviaDatiDetrazioni(prelievoAgo.Risposta.DatiDetrazioni, idPensione);
                                    ArchiviaDatiSindacato(datiSindacatoMaster, prelievoAgo.Risposta.DatiSindacato, idPensione);
                                    ArchiviaDatiIntegrazioneArt11(prelievoAgo.Risposta.DatiIntegrazioneArt11, idPensione);
                                    ArchiviaDatiSupplementiBase(prelievoAgo.Risposta.DatiSupplementiBase, idPensione);
                                    if (!Utility.IsDomandaRipristinoOrRiliquidazione(datiPensioneMaster))
                                        ArchiviaDatiEliminazione(prelievoAgo.Risposta.DatiEliminazione, idPensione);
                                    if (prelievoAgo.Risposta.DatiPensioniDatiGenerici != null)
                                    {
                                        if (Utility.IsDomandaAutomatica(datiPensioneMaster))
                                        {
                                            prelievoAgo.Risposta.DatiPensioniDatiGenerici.AnzAl95 = null;
                                            prelievoAgo.Risposta.DatiPensioniDatiGenerici.QuotaAl95 = null;
                                        }

                                        if (datiPensioniDatiGenericiMaster != null)
                                        {
                                            prelievoAgo.Risposta.DatiPensioniDatiGenerici.TipoCertificazioneFelpe = datiPensioniDatiGenericiMaster.TipoCertificazioneFelpe;
                                            prelievoAgo.Risposta.DatiPensioniDatiGenerici.TipologiaCumulo = datiPensioniDatiGenericiMaster.TipologiaCumulo;
                                            //ENG - TRF AUTOMATICHE VESO92/ESPA 
                                            if (isRiapertura && Utility.IsDomandaAutomatica(datiPensioneMaster) && (Utility.IsDomandaESPA(datiPensioneMaster.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensioneMaster.SiglaCategoria)))
                                            {
                                                prelievoAgo.Risposta.DatiPensioniDatiGenerici.ImportoLordoAllaDecorrenza = datiPensioniDatiGenericiMaster.ImportoLordoAllaDecorrenza;
                                                //ENG - TRF AUTOMATICHE VESO92/ESPA Scadenza Assegno da Unicarpe
                                                prelievoAgo.Risposta.DatiPensioniDatiGenerici.ScadenzaAssegno = datiPensioniDatiGenericiMaster.ScadenzaAssegno;
                                            }
                                        }
                                    }
                                    ArchiviaDatiPensioniDatiGenerici(prelievoAgo.Risposta.DatiPensioniDatiGenerici, idPensione);
                                    if (prelievoAgo.Risposta.ListaBititolarita != null)
                                        ArchiviaDatiBititolarita(prelievoAgo.Risposta.ListaBititolarita.ToList(), idPensione);
                                    if (prelievoAgo.Risposta.ListaInail != null)
                                        ArchiviaDatiInail(prelievoAgo.Risposta.ListaInail.ToList(), idPensione);
                                    ArchiviaDatiInabilita(prelievoAgo.Risposta.DatiInabilita, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93 != null)
                                        ArchiviaDatiRedditiSentenza495_93(prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93.ToList(), idPensione, datiPensioneMaster);
                                    if (listaDatiPrepensionamento != null && listaDatiPrepensionamento.Count() > 0)
                                        ArchiviaDatiPrepensionamento(listaDatiPrepensionamento, idPensione);
                                    ArchiviaDatiBeneficioVittimeTerrorismo(datiPensioneMaster, prelievoAgo.Risposta.DatiBeneficioVittimeTerrorismo, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiCalcoloVittimeTerrorismo != null)
                                        ArchiviaDatiCalcoloVittimeTerrorismo(prelievoAgo.Risposta.ListaDatiCalcoloVittimeTerrorismo.ToList(), idPensione);
                                    if (controlloFelpeMaster != null)
                                        GestioneDatiControlloFelpe.SalvaDatiControlloFelpe(datiPensioneMaster.Id, controlloFelpeMaster);
                                    if (datiDetrazioniImposta != null)
                                        ArchiviaDatiDetrazioni(datiDetrazioniImposta, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiSentenze != null && prelievoAgo.Risposta.ListaDatiSentenze.Count() > 0)
                                        ArchiviaDatiSentenze(prelievoAgo.Risposta.ListaDatiSentenze.ToList(), idPensione);

                                    //ENG - Spacchettate SOPGI - Spacchettate AGO
                                    if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa) || Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura)
                                        || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
                                    {
                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                                    if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                                    {
                                        if (listaDatiQuotaFondoIntegrativoStorico != null && listaDatiQuotaFondoIntegrativoStorico.Count() > 0)
                                            ArchiviaDatiQuotaFondoIntegrativo(listaDatiQuotaFondoIntegrativoStorico.ToList(), idPensione);
                                    }

                                    if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && Utility.IsDomandaINPGI(datiPensioneMaster) && Utility.IsDomandaUnicarpe(datiPensioneMaster, true) == Utility.TipoUnicarpe.Automatica)
                                    {
                                        if (listaDatiCalcoloRetributivoINPGIStorico != null && listaDatiCalcoloRetributivoINPGIStorico.Count > 0)
                                            ArchiviaDatiCalcoloRetributivoINPGI(listaDatiCalcoloRetributivoINPGIStorico, idPensione);

                                        if (listaDatiCalcoloContributivoINPGIStorico != null && listaDatiCalcoloContributivoINPGIStorico.Count > 0)
                                            ArchiviaDatiCalcoloContributivoINPGI(listaDatiCalcoloContributivoINPGIStorico, idPensione);
                                    }
                                }
                                else if (tipoDomanda == Utility.TipoDomanda.Superstiti && Utility.IsDomandaReversibilita(datiPensioneMaster))
                                {
                                    ArchiviaDatiPensione(datiPensioneMaster, prelievoAgo.Risposta.DatiPensione, null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, null, idPensione);
                                    ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, null, idPensione);
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93 != null)
                                        ArchiviaDatiRedditiSentenza495_93(prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93.ToList(), idPensione, datiPensioneMaster);
                                    if (CFfamiliari != null)
                                    {
                                        ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }
                                    ArchiviaDatiSindacato(datiSindacatoMaster, null, idPensione);

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                                    if (listaDatiCalcoloRetributivoStorico != null && listaDatiCalcoloRetributivoStorico.Count > 0)
                                        ArchiviaDatiCalcoloRetributivo(listaDatiCalcoloRetributivoStorico, idPensione);

                                    //ENG - Spacchettate SOPGI
                                    if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
                                    {
                                        if (prelievoAgo.Risposta.ListaCalcoloContributivo != null && prelievoAgo.Risposta.ListaCalcoloContributivo.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        else if (datiCalcoloContributivoSOPGI != null && datiCalcoloContributivoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Count() > 0)
                                            ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                        else if (datiCalcoloRetributivoSOPGI != null && datiCalcoloRetributivoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaSupplementi != null && prelievoAgo.Risposta.ListaSupplementi.Count() > 0)
                                            ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        else if (datiSupplementiSOPGI != null && datiSupplementiSOPGI.Count() > 0)
                                            ArchiviaDatiSupplementi(datiSupplementiSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaDatiContributiviINPGI != null && prelievoAgo.Risposta.ListaDatiContributiviINPGI.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoINPGI(prelievoAgo.Risposta.ListaDatiContributiviINPGI.ToList(), idPensione);
                                        else if (datiCalcoloContributivoQuotaFondoSOPGI != null && datiCalcoloContributivoQuotaFondoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloContributivoINPGI(datiCalcoloContributivoQuotaFondoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaDatiRetributiviINPGI != null && prelievoAgo.Risposta.ListaDatiRetributiviINPGI.Count() > 0)
                                            ArchiviaDatiCalcoloRetributivoINPGI(prelievoAgo.Risposta.ListaDatiRetributiviINPGI.ToList(), idPensione);
                                        else if (datiCalcoloRetributivoQuotaFondoSOPGI != null && datiCalcoloRetributivoQuotaFondoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloRetributivoINPGI(datiCalcoloRetributivoQuotaFondoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.DatiPensioniDatiGenerici != null)
                                            ArchiviaDatiPensioniDatiGenerici(prelievoAgo.Risposta.DatiPensioniDatiGenerici, idPensione);

                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }
                                    else if (Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura)
                                        || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
                                    {
                                        if (prelievoAgo.Risposta.ListaCalcoloContributivo != null && prelievoAgo.Risposta.ListaCalcoloContributivo.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        else if (datiCalcoloContributivoSpacchettateAGO != null && datiCalcoloContributivoSpacchettateAGO.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSpacchettateAGO, idPensione);

                                        if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Count() > 0)
                                            ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                        else if (datiCalcoloRetributivoSpacchettateAGO != null && datiCalcoloRetributivoSpacchettateAGO.Count > 0)
                                            ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSpacchettateAGO, idPensione);

                                        if (prelievoAgo.Risposta.ListaSupplementi != null && prelievoAgo.Risposta.ListaSupplementi.Count() > 0)
                                            ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        else if (datiSupplementiSpacchettateAGO != null && datiSupplementiSpacchettateAGO.Count() > 0)
                                            ArchiviaDatiSupplementi(datiSupplementiSpacchettateAGO, idPensione);

                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }
                                    else
                                    {
                                        if (prelievoAgo.Risposta.ListaCalcoloContributivo != null)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                    }

                                    if (datiPagamentoWebDom != null)
                                        GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);

                                }
                                else
                                {
                                    ArchiviaDatiPensione(datiPensioneMaster, null, null, out idPensione);
                                    ArchiviaDatiIstruttoria(datiIstruttoriaMaster, null, idPensione);
                                    ArchiviaDatiMaggiorazioneBenefici(datiMaggiorazioniBeneficiMaster, null, idPensione);
                                    ArchiviaDatiNuoveLiquidate(datiNuoveLiquidateMaster, null, idPensione);
                                    if (prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93 != null)
                                        ArchiviaDatiRedditiSentenza495_93(prelievoAgo.Risposta.ListaDatiRedditiSentenza495_93.ToList(), idPensione, datiPensioneMaster);
                                    if (CFfamiliari != null)
                                    {
                                        ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }
                                    ArchiviaDatiSindacato(datiSindacatoMaster, null, idPensione);

                                    ArchiviaDatiStoricoGP(datiStoricoGP, idPensione);

                                    if (Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensioneMaster))
                                    {
                                        if (datiPensioneMaster != null && lOneriMaster != null && lOneriMaster.Count > 0)
                                        {
                                            //GestioneOneri.EliminaOneri(datiPensioneMaster.Id);
                                            foreach (GestioneOneri.DatiOneri datiOneriMaster in lOneriMaster)
                                            {
                                                datiOneriMaster.IdPensione = datiPensioneMaster.Id;
                                                GestioneOneri.SalvaOneriOnere(datiOneriMaster);
                                            }
                                        }
                                        else if (prelievoAgo.Risposta.ListaDatiOneri != null)
                                            ArchiviaDatiOneri(prelievoAgo.Risposta.ListaDatiOneri.ToList(), idPensione);
                                    }

                                    if (datiPagamentoWebDom != null)
                                        GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);

                                    //ENG - Spacchettate SOPGI
                                    if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
                                    {
                                        if (prelievoAgo.Risposta.ListaCalcoloContributivo != null && prelievoAgo.Risposta.ListaCalcoloContributivo.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        else if (datiCalcoloContributivoSOPGI != null && datiCalcoloContributivoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Count() > 0)
                                            ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                        else if (datiCalcoloRetributivoSOPGI != null && datiCalcoloRetributivoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaSupplementi != null && prelievoAgo.Risposta.ListaSupplementi.Count() > 0)
                                            ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        else if (datiSupplementiSOPGI != null && datiSupplementiSOPGI.Count() > 0)
                                            ArchiviaDatiSupplementi(datiSupplementiSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaDatiContributiviINPGI != null && prelievoAgo.Risposta.ListaDatiContributiviINPGI.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoINPGI(prelievoAgo.Risposta.ListaDatiContributiviINPGI.ToList(), idPensione);
                                        else if (datiCalcoloContributivoQuotaFondoSOPGI != null && datiCalcoloContributivoQuotaFondoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloContributivoINPGI(datiCalcoloContributivoQuotaFondoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.ListaDatiRetributiviINPGI != null && prelievoAgo.Risposta.ListaDatiRetributiviINPGI.Count() > 0)
                                            ArchiviaDatiCalcoloRetributivoINPGI(prelievoAgo.Risposta.ListaDatiRetributiviINPGI.ToList(), idPensione);
                                        else if (datiCalcoloRetributivoQuotaFondoSOPGI != null && datiCalcoloRetributivoQuotaFondoSOPGI.Count > 0)
                                            ArchiviaDatiCalcoloRetributivoINPGI(datiCalcoloRetributivoQuotaFondoSOPGI, idPensione);

                                        if (prelievoAgo.Risposta.DatiPensioniDatiGenerici != null)
                                            ArchiviaDatiPensioniDatiGenerici(prelievoAgo.Risposta.DatiPensioniDatiGenerici, idPensione);

                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }

                                    //ENG - Spacchettamento AGO
                                    if (Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura)
                                         || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
                                    {
                                        if (prelievoAgo.Risposta.ListaCalcoloContributivo != null && prelievoAgo.Risposta.ListaCalcoloContributivo.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(prelievoAgo.Risposta.ListaCalcoloContributivo.ToList(), idPensione);
                                        else if (datiCalcoloContributivoSpacchettateAGO != null && datiCalcoloContributivoSpacchettateAGO.Count() > 0)
                                            ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSpacchettateAGO, idPensione);

                                        if (prelievoAgo.Risposta.ListaCalcoloRetributivo != null && prelievoAgo.Risposta.ListaCalcoloRetributivo.Count() > 0)
                                            ArchiviaDatiCalcoloRetributivo(prelievoAgo.Risposta.ListaCalcoloRetributivo.ToList(), idPensione);
                                        else if (datiCalcoloRetributivoSpacchettateAGO != null && datiCalcoloRetributivoSpacchettateAGO.Count > 0)
                                            ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSpacchettateAGO, idPensione);

                                        if (prelievoAgo.Risposta.ListaSupplementi != null && prelievoAgo.Risposta.ListaSupplementi.Count() > 0)
                                            ArchiviaDatiSupplementi(prelievoAgo.Risposta.ListaSupplementi.ToList(), idPensione);
                                        else if (datiSupplementiSpacchettateAGO != null && datiSupplementiSpacchettateAGO.Count() > 0)
                                            ArchiviaDatiSupplementi(datiSupplementiSpacchettateAGO, idPensione);

                                        ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                                        if (!string.IsNullOrEmpty(errori))
                                            return;
                                    }
                                }

                                return;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (tipoAppartenenza == Utility.TipoAppartenenza.CI && Utility.IsDomandaAPEPrecoci(datiPensioneMaster))
            {
                datiPensioneMaster.CodiceSedeDestinazione = null;
                datiPensioneMaster.CentroOperativoDestinazione = null;
            }

            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa) && coefficienteTrasformazione.HasValue)
            {
                if (datiPensioniDatiGenericiMaster == null)
                    datiPensioniDatiGenericiMaster = new GestioneDatiGenericiAgoCi.PensioniDatiGenerici();

                datiPensioniDatiGenericiMaster.PL_Coeftrasf = coefficienteTrasformazione;
            }


            if (datiPensioneMaster != null)
                GestionePensione.SalvaPensione(datiPensioneMaster);

            if (datiPensioneMaster != null && datiIstruttoriaMaster != null)
                GestioneIstruttoria.SalvaIstruttoria(datiPensioneMaster.Id, datiIstruttoriaMaster);

            if (datiPensioneMaster != null && datiMaggiorazioniBeneficiMaster != null)
            {
                datiMaggiorazioniBeneficiMaster.IdPensione = datiPensioneMaster.Id;
                GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiMaster);
            }

            if (datiPensioneMaster != null && datiNuoveLiquidateMaster != null)
            {
                datiNuoveLiquidateMaster.IdPensione = datiPensioneMaster.Id;
                GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidateMaster);
            }

            if (datiPensioneMaster != null && datiPensioniDatiGenericiMaster != null)
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(datiPensioneMaster.Id, datiPensioniDatiGenericiMaster);

            if (datiPensioneMaster != null && datiPensioneFondoDatiGenericiMaster != null)
                GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiPensioneFondoDatiGenericiMaster);

            if (datiPensioneMaster != null && datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoTT != null)
            {
                if (datiPensioneFondoDatiGenericiMaster != null)
                    idFondo = datiPensioneFondoDatiGenericiMaster.Id;
                else
                {
                    GestioneFondo.DatiFondo datiFondo = null;
                    GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensioneMaster.Id, out datiFondo);
                    if (datiFondo == null)
                    {
                        datiFondo = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiFondo);
                    }
                    idFondo = datiFondo.Id;
                }
                datiFondoSpecificoFELPE.DatiFondoTT.IdFondo = idFondo;
                GestioneFondo.SalvaFondoTT(idFondo, datiFondoSpecificoFELPE.DatiFondoTT);
            }

            if (datiPensioneMaster != null && datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoVL != null)
            {
                if (datiPensioneFondoDatiGenericiMaster != null)
                    idFondo = datiPensioneFondoDatiGenericiMaster.Id;
                else
                {
                    GestioneFondo.DatiFondo datiFondo = null;
                    GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensioneMaster.Id, out datiFondo);
                    if (datiFondo == null)
                    {
                        datiFondo = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiFondo);
                    }
                    idFondo = datiFondo.Id;
                }
                datiFondoSpecificoFELPE.DatiFondoVL.IdFondo = idFondo;
                GestioneFondo.SalvaFondoVL(idFondo, datiFondoSpecificoFELPE.DatiFondoVL);
            }

            if (datiPensioneMaster != null && datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoET != null)
            {
                if (datiPensioneFondoDatiGenericiMaster != null)
                    idFondo = datiPensioneFondoDatiGenericiMaster.Id;
                else
                {
                    GestioneFondo.DatiFondo datiFondo = null;
                    GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensioneMaster.Id, out datiFondo);
                    if (datiFondo == null)
                    {
                        datiFondo = new GestioneFondo.DatiFondo();
                        GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiFondo);
                    }
                    idFondo = datiFondo.Id;
                }
                datiFondoSpecificoFELPE.DatiFondoET.IdFondo = idFondo;
                GestioneFondo.SalvaFondoET(idFondo, datiFondoSpecificoFELPE.DatiFondoET);
            }

            if (datiPensioneMaster != null && lOneriMaster != null && lOneriMaster.Count > 0)
            {
                //GestioneOneri.EliminaOneri(datiPensioneMaster.Id);
                foreach (GestioneOneri.DatiOneri datiOneriMaster in lOneriMaster)
                {
                    datiOneriMaster.IdPensione = datiPensioneMaster.Id;
                    GestioneOneri.SalvaOneriOnere(datiOneriMaster);
                }
            }

            if (datiPensioneMaster != null && lDatiBeneficiParticolariMaster != null && lDatiBeneficiParticolariMaster.Count > 0)
            {
                //GestioneBeneficiParticolari.DeleteDatiBeneficiParticolari(datiPensioneMaster.Id);
                foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari datiBeneficiParticolari in lDatiBeneficiParticolariMaster)
                {
                    datiBeneficiParticolari.IdPensione = datiPensioneMaster.Id;
                    GestioneBeneficiParticolari.SalvaDatiBeneficiParticolari(datiBeneficiParticolari);
                }
            }

            if (controlloFelpeMaster != null)
            {
                GestioneDatiControlloFelpe.SalvaDatiControlloFelpe(datiPensioneMaster.Id, controlloFelpeMaster);
            }

            if (CFfamiliari != null && !familiariDaPrelievo)
            {
                ArchiviaFamiliari(parametriArca, CFfamiliari, datiPensioneMaster, null, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return;
            }

            if (datiPagamentoWebDom != null)
            {
                GestionePagamento.SalvaPagamento(datiPensioneMaster.Id, datiPagamentoWebDom);
            }

            if (datiSindacatoMaster != null)
            {
                GestionePensione.SalvaSindacato(datiPensioneMaster.Id, datiSindacatoMaster);
            }

            if (datiPensioneMaster.IsCumuloAutomatica.GetValueOrDefault() ||
                datiPensioneMaster.IsTotAutomatica.GetValueOrDefault())
            {
                if (listaDatiStatoCivileMaster != null && listaDatiStatoCivileMaster.Count > 0)
                    ArchiviaDatiStatiCivili(listaDatiStatoCivileMaster, datiPensioneMaster.Id, anagraficaTitolare.Id);

                if (listaDatiQuotePensioneTotalIVS != null && listaDatiQuotePensioneTotalIVS.Count > 0)
                    ArchiviaDatiQuotePensione(listaDatiQuotePensioneTotalIVS, null, datiPensioneMaster.Id);

                if (listaDatiTrattenuteQuotePensioneTotalIVS != null && listaDatiTrattenuteQuotePensioneTotalIVS.Count > 0)
                    ArchiviaDatiTrattenuteQuotePensione(listaDatiTrattenuteQuotePensioneTotalIVS, null, datiPensioneMaster.Id);
            }

            Utility.TipoFondo? tipoFondoCompare = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensioneMaster.SiglaCategoria);
            if (!(tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) &&
                !Utility.IsDomandaReversibilita(datiPensioneMaster) &&
                (isDomandaConNuovaGestioneDatiFondoFSPT || tipoFondoCompare == Utility.TipoFondo.DZ || tipoFondoCompare == Utility.TipoFondo.PI || tipoFondoCompare == Utility.TipoFondo.PL))
            {

                listaDatiRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();

                //per le pi e pl inseriamo in fase di prelievo solo la riga in pensioneFondoDatiGenerici
                if (!(tipoFondoCompare == Utility.TipoFondo.PI || tipoFondoCompare == Utility.TipoFondo.PL))
                {
                    if (tipoFondoCompare == Utility.TipoFondo.DZ && recordFondo.CodiceNatura1 == null && recordFondo.CodiceNatura2 == null && recordFondo.CodiceNatura3 == null && recordFondo.CodiceNonCalcolo == null)
                    {
                        recordFondo.CodiceNatura1 = '0';
                        recordFondo.CodiceNatura2 = ' ';
                        recordFondo.CodiceNatura3 = ' ';
                        recordFondo.CodiceNonCalcolo = ' ';
                    }
                    GestioneRecordFondo.SalvaSingoloRecordFondo(datiPensioneMaster.Id, recordFondo);
                    listaDatiRecordFondo.Add(recordFondo);
                }

                if (idFondo == 0)
                {
                    GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo();
                    GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiFondo);
                    idFondo = datiFondo.Id;
                }

                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensioneMaster.SiglaCategoria);
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.PT:
                            if (datiFondoSpecificoFELPE == null)
                                datiFondoSpecificoFELPE = new Entity.DatiFondoSpecificoFELPE();
                            if (datiFondoSpecificoFELPE.DatiFondoPT == null)
                                datiFondoSpecificoFELPE.DatiFondoPT = new GestioneFondo.DatiFondoPT();
                            GestioneFondo.SalvaFondoPTRecordFondo(idFondo, recordFondo.Id, datiFondoSpecificoFELPE.DatiFondoPT);
                            break;
                        case Utility.TipoFondo.DZ:
                            if (datiFondoSpecificoFELPE == null)
                                datiFondoSpecificoFELPE = new Entity.DatiFondoSpecificoFELPE();
                            if (datiFondoSpecificoFELPE.DatiFondoDZ == null)
                                datiFondoSpecificoFELPE.DatiFondoDZ = new GestioneFondo.DatiFondoDZ();
                            GestioneFondo.SalvaFondoDZRecordFondo(idFondo, recordFondo.Id, datiFondoSpecificoFELPE.DatiFondoDZ);
                            break;
                        case Utility.TipoFondo.FS:
                            if (datiFondoSpecificoFELPE == null)
                                datiFondoSpecificoFELPE = new Entity.DatiFondoSpecificoFELPE();
                            if (datiFondoSpecificoFELPE.DatiFondoFST == null)
                                datiFondoSpecificoFELPE.DatiFondoFST = new GestioneFondo.DatiFondoFST();
                            GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, recordFondo.Id, datiFondoSpecificoFELPE.DatiFondoFST);
                            break;
                    }
                }
            }
            else
            {
                if (datiPensioneMaster != null && datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoFST != null)
                {
                    if (datiPensioneFondoDatiGenericiMaster != null)
                        idFondo = datiPensioneFondoDatiGenericiMaster.Id;
                    else
                    {
                        GestioneFondo.DatiFondo datiFondo = null;
                        GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensioneMaster.Id, out datiFondo);
                        if (datiFondo == null)
                        {
                            datiFondo = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiFondo);
                        }
                        idFondo = datiFondo.Id;
                    }
                    datiFondoSpecificoFELPE.DatiFondoFST.IdFondo = idFondo;
                    GestioneFondo.SalvaFondoFST(idFondo, datiFondoSpecificoFELPE.DatiFondoFST);
                }

                if (datiPensioneMaster != null && datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoPT != null)
                {
                    if (datiPensioneFondoDatiGenericiMaster != null)
                        idFondo = datiPensioneFondoDatiGenericiMaster.Id;
                    else
                    {
                        GestioneFondo.DatiFondo datiFondo = null;
                        GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensioneMaster.Id, out datiFondo);
                        if (datiFondo == null)
                        {
                            datiFondo = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(datiPensioneMaster.Id, datiFondo);
                        }
                        idFondo = datiFondo.Id;
                    }
                    datiFondoSpecificoFELPE.DatiFondoPT.IdFondo = idFondo;
                    GestioneFondo.SalvaFondoPT(idFondo, datiFondoSpecificoFELPE.DatiFondoPT);
                }
            }

            if (Utility.IsDomandaINPDAP(datiPensioneMaster.Gestione))
            {
                listaDatiRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
                GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                GestioneRecordFondo.SalvaSingoloRecordFondo(datiPensioneMaster.Id, recordFondo);
                listaDatiRecordFondo.Add(recordFondo);

                if (recordDatiFondoINPDAPMaster == null)
                    recordDatiFondoINPDAPMaster = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensioneMaster.Id, recordFondo.Id, recordDatiFondoINPDAPMaster);

                if (datiPensioneINPDAPMaster == null)
                    datiPensioneINPDAPMaster = new GestionePensioneINPDAP.DatiPensioneINPDAP();
                GestionePensioneINPDAP.SalvaPensioneINPDAPRecordFondo(datiPensioneMaster.Id, recordFondo.Id, datiPensioneINPDAPMaster);

                ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return;
            }

            //ENG - Spacchettate 024
            if (controlloDinamicoSpacchettate024 != null && !String.IsNullOrEmpty(controlloDinamicoSpacchettate024.ValoreControllo) && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensioneMaster, isRiapertura))
            {
                ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return;
            }

            //ENG - Spacchettate SOPGI
            if (Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensioneMaster, danteCausa))
            {
                if (datiCalcoloContributivoSOPGI != null && datiCalcoloContributivoSOPGI.Count > 0)
                    ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSOPGI, datiPensioneMaster.Id);

                if (datiCalcoloRetributivoSOPGI != null && datiCalcoloRetributivoSOPGI.Count > 0)
                    ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSOPGI, datiPensioneMaster.Id);

                if (datiSupplementiSOPGI != null && datiSupplementiSOPGI.Count() > 0)
                    ArchiviaDatiSupplementi(datiSupplementiSOPGI, datiPensioneMaster.Id);

                if (datiCalcoloContributivoQuotaFondoSOPGI != null && datiCalcoloContributivoQuotaFondoSOPGI.Count > 0)
                    ArchiviaDatiCalcoloContributivoINPGI(datiCalcoloContributivoQuotaFondoSOPGI, datiPensioneMaster.Id);

                if (datiCalcoloRetributivoQuotaFondoSOPGI != null && datiCalcoloRetributivoQuotaFondoSOPGI.Count > 0)
                    ArchiviaDatiCalcoloRetributivoINPGI(datiCalcoloRetributivoQuotaFondoSOPGI, datiPensioneMaster.Id);

                ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return;
            }

            //ENG - Spacchettamento AGO
            if (Utility.IsDomandaSpacchettamentoSO(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOART(datiPensioneMaster, isRiapertura) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensioneMaster, isRiapertura)
                || Utility.IsDomandaSpacchettamentoSR(datiPensioneMaster, isRiapertura))
            {
                if (datiCalcoloContributivoSpacchettateAGO != null && datiCalcoloContributivoSpacchettateAGO.Count > 0)
                    ArchiviaDatiCalcoloContributivoAGO_CI(datiCalcoloContributivoSpacchettateAGO, datiPensioneMaster.Id);

                if (datiCalcoloRetributivoSpacchettateAGO != null && datiCalcoloRetributivoSpacchettateAGO.Count > 0)
                    ArchiviaDatiCalcoloRetributivo(datiCalcoloRetributivoSpacchettateAGO, datiPensioneMaster.Id);

                if (datiSupplementiSpacchettateAGO != null && datiSupplementiSpacchettateAGO.Count() > 0)
                    ArchiviaDatiSupplementi(datiSupplementiSpacchettateAGO, datiPensioneMaster.Id);

                ArchiviaDatiAventiDiritto(parametriArca, datiPensioneMaster, listaDatiAventiDirittoWebDom, listaDatiAventiDirittoGP, out errori);
                if (!string.IsNullOrEmpty(errori))
                    return;
            }
        }

        private static void ArchiviaDatiSupplementiCumuloAutomatiche(List<BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiMaster, List<BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiDaPrelievo, long idPensione)
        {
            if (listaSupplementiDaPrelievo != null && listaSupplementiDaPrelievo.Count > 0)
            {
                listaSupplementiDaPrelievo.ForEach(x => x.IdPensione = idPensione);
                GestioneSupplementi.SalvaDatiSupplementiCumulo(listaSupplementiDaPrelievo);
            }

            if (listaSupplementiMaster != null && listaSupplementiMaster.Count > 0)
            {
                listaSupplementiMaster.ForEach(x => x.IdPensione = idPensione);
                GestioneSupplementi.SalvaDatiSupplementiCumulo(listaSupplementiMaster);
            }
        }

        #region ArchiviazioneConDatiMaster
        private static void ArchiviaDatiPensione(GestionePensione.DatiPensione datiPensioneMaster, GestionePensione.DatiPensione datiPensioneDaPrelievo, int? ContributiItalianiEdEsteriAl1295, out long idPensione)
        {
            idPensione = 0;
            if (datiPensioneMaster != null)
            {
                if (datiPensioneDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiPensioneDaPrelievo, datiPensioneMaster);
                if (Utility.GetEnumTipoPLPerRICbyId(datiPensioneMaster.IdTipoPLPerRIC) == Utility.TipoPLPerRIC.Nessuno)
                {
                    datiPensioneMaster.IdTipoPLPerRIC = (byte?)Utility.IsDomandaTipoContributivoFromPrelievo(datiPensioneMaster, ContributiItalianiEdEsteriAl1295);
                    if (datiPensioneMaster.IdTipoPLPerRIC == 0)
                        datiPensioneMaster.IdTipoPLPerRIC = null;
                }
                GestionePensione.SalvaPensione(datiPensioneMaster);
                idPensione = datiPensioneMaster.Id;
            }
            else if (datiPensioneDaPrelievo != null)
            {
                GestionePensione.SalvaPensione(datiPensioneDaPrelievo);
                idPensione = datiPensioneDaPrelievo.Id;
            }
        }

        private static void ArchiviaDatiIstruttoria(GestioneIstruttoria.DatiIstruttoria datiIstruttoriaMaster, GestioneIstruttoria.DatiIstruttoria datiIstruttoriaDaPrelievo, long idPensione)
        {
            if (datiIstruttoriaMaster != null)
            {
                if (datiIstruttoriaDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiIstruttoriaDaPrelievo, datiIstruttoriaMaster);
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoriaMaster);
            }
            else if (datiIstruttoriaDaPrelievo != null && !Utility.ConfrontaOggetti(datiIstruttoriaDaPrelievo, new GestioneIstruttoria.DatiIstruttoria()))
                GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoriaDaPrelievo);
        }

        private static void ArchiviaDatiMaggiorazioneBenefici(BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiMaster,
            BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBeneficiDaPrelievo, long idPensione)
        {
            if (datiMaggiorazioniBeneficiMaster != null)
            {
                if (datiMaggiorazioniBeneficiDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiMaggiorazioniBeneficiDaPrelievo, datiMaggiorazioniBeneficiMaster);
                datiMaggiorazioniBeneficiMaster.IdPensione = idPensione;
                BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiMaster);
            }
            else if (datiMaggiorazioniBeneficiDaPrelievo != null && !Utility.ConfrontaOggetti(datiMaggiorazioniBeneficiDaPrelievo, new BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici()))
            {
                datiMaggiorazioniBeneficiDaPrelievo.IdPensione = idPensione;
                BLCommon.GestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(datiMaggiorazioniBeneficiDaPrelievo);
            }
        }

        private static void ArchiviaDatiNuoveLiquidate(GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidateMaster, GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidateDaPrelievo, long idPensione)
        {
            if (datiNuoveLiquidateMaster != null)
            {
                if (datiNuoveLiquidateDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiNuoveLiquidateDaPrelievo, datiNuoveLiquidateMaster);
                datiNuoveLiquidateMaster.IdPensione = idPensione;
                GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidateMaster);
            }
            else if (datiNuoveLiquidateDaPrelievo != null && !Utility.ConfrontaOggetti(datiNuoveLiquidateDaPrelievo, new GestioneNuoveLiquidate.NuoveLiquidate()))
            {
                datiNuoveLiquidateDaPrelievo.IdPensione = idPensione;
                GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidateDaPrelievo);
            }
        }

        private static void ArchiviaDatiPensioniCiDatiGenerici(GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniCiDatiGenericiMaster, GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniCiDatiGenericiDaPrelievo, long idPensione)
        {
            if (datiPensioniCiDatiGenericiMaster != null)
            {
                if (datiPensioniCiDatiGenericiDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiPensioniCiDatiGenericiDaPrelievo, datiPensioniCiDatiGenericiMaster);
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniCiDatiGenericiMaster);
            }
            else if (datiPensioniCiDatiGenericiDaPrelievo != null && !Utility.ConfrontaOggetti(datiPensioniCiDatiGenericiDaPrelievo, new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniCiDatiGenericiDaPrelievo);
        }

        private static void ArchiviaDatiCalcoloContributivoEnpals(GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivoMaster,
            GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivoDaPrelievo, long idPensione)
        {
            if (datiCalcoloContributivoMaster != null)
            {
                if (datiCalcoloContributivoDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiCalcoloContributivoDaPrelievo, datiCalcoloContributivoMaster);
                datiCalcoloContributivoMaster.IdPensione = idPensione;
                GestioneCalcolo.SalvaCalcoloContributivoEnpals(datiCalcoloContributivoMaster);
            }
            else if (datiCalcoloContributivoDaPrelievo != null && !datiCalcoloContributivoDaPrelievo.IsDatiCalcoloContributivoEnpalsNull())
            {
                datiCalcoloContributivoDaPrelievo.IdPensione = idPensione;
                GestioneCalcolo.SalvaCalcoloContributivoEnpals(datiCalcoloContributivoDaPrelievo);
            }
        }

        private static void ArchiviaDatiCalcoloRetributivoEnpals(GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivoMaster,
            GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivoDaPrelievo, long idPensione)
        {
            if (datiCalcoloRetributivoMaster != null)
            {
                if (datiCalcoloRetributivoDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiCalcoloRetributivoDaPrelievo, datiCalcoloRetributivoMaster);
                datiCalcoloRetributivoMaster.IdPensione = idPensione;
                GestioneCalcolo.SalvaCalcoloRetributivoEnpals(datiCalcoloRetributivoMaster);
            }
            else if (datiCalcoloRetributivoDaPrelievo != null && !datiCalcoloRetributivoDaPrelievo.IsDatiCalcoloRetributivoEnpalsNull())
            {
                datiCalcoloRetributivoDaPrelievo.IdPensione = idPensione;
                GestioneCalcolo.SalvaCalcoloRetributivoEnpals(datiCalcoloRetributivoDaPrelievo);
            }
        }

        private static void ArchiviaDatiSupplementiENPALS(List<BLCommon.Entity.DatiSupplementiENPALS> ListaSupplementiMaster, List<BLCommon.Entity.DatiSupplementiENPALS> ListaSupplementiDaPrelievo,
            long idPensione, Dictionary<DateTime, long> idRecordENPALS)
        {
            List<BLCommon.Entity.DatiSupplementiENPALS> listaSupplementiToSave = new List<BLCommon.Entity.DatiSupplementiENPALS>();

            if (ListaSupplementiDaPrelievo != null && ListaSupplementiDaPrelievo.Count > 0)
            {
                if (ListaSupplementiMaster != null && ListaSupplementiMaster.Count > 0)
                {
                    listaSupplementiToSave.AddRange(ListaSupplementiMaster.FindAll(x => ListaSupplementiDaPrelievo.Exists(y => y.Decorrenza == x.Decorrenza)));
                    listaSupplementiToSave.AddRange(ListaSupplementiMaster.FindAll(x => !ListaSupplementiDaPrelievo.Exists(y => y.Decorrenza == x.Decorrenza)));
                    listaSupplementiToSave.AddRange(ListaSupplementiDaPrelievo.FindAll(x => !ListaSupplementiMaster.Exists(y => y.Decorrenza == x.Decorrenza)));
                }
                else
                    listaSupplementiToSave.AddRange(ListaSupplementiDaPrelievo);
            }
            else
                if (ListaSupplementiMaster != null && ListaSupplementiMaster.Count > 0)
                listaSupplementiToSave.AddRange(ListaSupplementiMaster);

            if (listaSupplementiToSave.Count > 0)
            {
                listaSupplementiToSave = listaSupplementiToSave.OrderBy(x => x.Decorrenza).ToList();

                for (int i = 0; i < listaSupplementiToSave.Count; i++)
                {
                    if (Utility.ConfrontaOggetti(listaSupplementiToSave[i], new BLCommon.Entity.DatiSupplementiENPALS()))
                    {
                        listaSupplementiToSave.RemoveAt(i);
                        i--;
                    }
                }
                if (listaSupplementiToSave.Count > 0)
                    foreach (BLCommon.Entity.DatiSupplementiENPALS supplementi in listaSupplementiToSave)
                    {
                        long idRecord = 0;
                        idRecordENPALS.TryGetValue(supplementi.Decorrenza.GetValueOrDefault(), out idRecord);
                        if (idRecord != 0)
                            GestioneSupplementi.SalvaDatiSupplementiEnpalsByIdSuppRecordENPALS(idPensione, idRecord, supplementi);
                    }
            }
        }

        private static void ArchiviaDatiPagamento(GestionePagamento.DatiPagamento datiPagamentoDaPrelievo, GestionePagamento.DatiPagamento datiPagamentoWebDom, long idPensione)
        {
            GestionePagamento.DatiPagamento datiPagamentoToSave = datiPagamentoDaPrelievo;
            Utility.ValorizzaOggettiMaster(datiPagamentoWebDom, datiPagamentoToSave);

            if (datiPagamentoToSave != null && !Utility.ConfrontaOggetti(datiPagamentoToSave, new GestionePagamento.DatiPagamento()))
            {
                List<GestioneUfficiPagatori.AreaUfficioPagatore> listaUfficioPagatore = null;
                string errore = string.Empty;
                if (datiPagamentoToSave.ABI.HasValue && datiPagamentoToSave.CAB.HasValue)
                {
                    if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(datiPagamentoToSave.ABI.Value, datiPagamentoToSave.CAB.Value, out listaUfficioPagatore, out errore))
                        INPS.DNA.Logging.Logger.WriteError(errore);
                }
                else if (!string.IsNullOrEmpty(datiPagamentoToSave.IBAN))
                {
                    int abi = 0;
                    int.TryParse(datiPagamentoToSave.IBAN.Substring(5, 5), out abi);
                    datiPagamentoToSave.ABI = abi;
                    int cab = 0;
                    int.TryParse(datiPagamentoToSave.IBAN.Substring(10, 5), out cab);
                    datiPagamentoToSave.CAB = cab;
                    if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(abi, cab, out listaUfficioPagatore, out errore))
                        INPS.DNA.Logging.Logger.WriteError(errore);
                }
                if (listaUfficioPagatore != null && listaUfficioPagatore.Count > 0)
                {
                    datiPagamentoToSave = new GestionePagamento.DatiPagamento(datiPagamentoToSave.IBAN, null, datiPagamentoToSave.ModalitaPagamento, listaUfficioPagatore.First().CodiceMeccanizzazione,
                        datiPagamentoToSave.ABI, datiPagamentoToSave.CAB, datiPagamentoToSave.Frazionario, datiPagamentoToSave.BIC, datiPagamentoToSave.Libretto, null, null, null, null, null, datiPagamentoToSave.TrattenutaInpdap,
                        datiPagamentoToSave.TipoPagamento, datiPagamentoToSave.StatoEstero, datiPagamentoToSave.DataRinunciaTrattenutaInpdap, listaUfficioPagatore.First().Nome, listaUfficioPagatore.First().Agenzia,
                            listaUfficioPagatore.First().Cap, listaUfficioPagatore.First().Citta, listaUfficioPagatore.First().Indirizzo, datiPagamentoToSave.CodCatastaleEstero,
                            datiPagamentoToSave.IsFromWebDom);
                }
                if (datiPagamentoToSave.ABI.HasValue && datiPagamentoToSave.ABI.Value == 07601)
                {
                    datiPagamentoToSave.TipoPagamento = 'P';
                    datiPagamentoToSave.Frazionario = datiPagamentoToSave.CAB.HasValue ? datiPagamentoToSave.CAB.Value : (int?)null;
                    datiPagamentoToSave.CAB = null;
                }
                else if (datiPagamentoToSave.ABI.HasValue && datiPagamentoToSave.CAB.HasValue && datiPagamentoToSave.ABI == 36081 && datiPagamentoToSave.CAB == 05138)
                {
                    datiPagamentoToSave.TipoPagamento = 'P';
                    datiPagamentoToSave.Frazionario = null;
                }
                else if (datiPagamentoToSave.ABI.HasValue && datiPagamentoToSave.ABI.Value == 99999)
                    datiPagamentoToSave.TipoPagamento = 'C';
                else if ((datiPagamentoToSave.CAB.GetValueOrDefault().ToString().StartsWith("44") || datiPagamentoToSave.CAB.GetValueOrDefault().ToString().StartsWith("77")) &&
                    datiPagamentoToSave.CAB.GetValueOrDefault().ToString().Length >= 7)
                    datiPagamentoToSave.TipoPagamento = 'E';
                else
                    datiPagamentoToSave.TipoPagamento = 'B';
                GestionePagamento.SalvaPagamento(idPensione, datiPagamentoToSave);
            }
        }

        private static void ArchiviaDatiENPALS(GestioneEnpals.DatiEnpals datiENPALSMaster, GestioneEnpals.DatiEnpals datiENPALSDaPrelievo, long idPensione)
        {
            if (datiENPALSMaster != null)
            {
                if (datiENPALSDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiENPALSDaPrelievo, datiENPALSMaster);
                datiENPALSMaster.IdPensione = idPensione;
                GestioneEnpals.SalvaDatiEnpalsEnpals(datiENPALSMaster);
            }
            else if (datiENPALSDaPrelievo != null && !Utility.ConfrontaOggetti(datiENPALSDaPrelievo, new GestioneEnpals.DatiEnpals()))
            {
                datiENPALSDaPrelievo.IdPensione = idPensione;
                GestioneEnpals.SalvaDatiEnpalsEnpals(datiENPALSDaPrelievo);
            }
        }

        private static void ArchiviaDatiRecordSuppENPALS(List<BLCommon.Entity.DatiSuppRecordENPALS> datiSuppRecordENPALSMaster, List<BLCommon.Entity.DatiSuppRecordENPALS> datiSuppRecordENPALSDaPrelievo,
            long idPensione, out Dictionary<DateTime, long> idRecordENPALSMaster)
        {
            idRecordENPALSMaster = null;

            List<BLCommon.Entity.DatiSuppRecordENPALS> listaSupplementiToSave = new List<BLCommon.Entity.DatiSuppRecordENPALS>();

            if (datiSuppRecordENPALSDaPrelievo != null && datiSuppRecordENPALSDaPrelievo.Count > 0)
            {
                if (datiSuppRecordENPALSMaster != null && datiSuppRecordENPALSMaster.Count > 0)
                {
                    listaSupplementiToSave.AddRange(datiSuppRecordENPALSMaster);
                    listaSupplementiToSave.AddRange(datiSuppRecordENPALSDaPrelievo.FindAll(x => !datiSuppRecordENPALSMaster.Exists(y => y.Decorrenza == x.Decorrenza)));
                }
                else
                    listaSupplementiToSave.AddRange(datiSuppRecordENPALSDaPrelievo);
            }
            else
                if (datiSuppRecordENPALSMaster != null)
                listaSupplementiToSave.AddRange(datiSuppRecordENPALSMaster);

            if (listaSupplementiToSave.Count > 0)
            {
                listaSupplementiToSave = listaSupplementiToSave.OrderBy(x => x.Decorrenza).ToList();

                for (int i = 0; i < listaSupplementiToSave.Count; i++)
                {
                    if (Utility.ConfrontaOggetti(listaSupplementiToSave[i], new BLCommon.Entity.DatiSuppRecordENPALS()))
                    {
                        listaSupplementiToSave.RemoveAt(i);
                        i--;
                    }
                }
                if (listaSupplementiToSave.Count > 0)
                    foreach (BLCommon.Entity.DatiSuppRecordENPALS supplementi in listaSupplementiToSave)
                    {
                        long? idRecord = null;
                        GestioneSupplementi.SalvaDatiSuppRecordEnpals(idPensione, supplementi, out idRecord);

                        if (idRecordENPALSMaster == null)
                            idRecordENPALSMaster = new Dictionary<DateTime, long>();
                        idRecordENPALSMaster.Add(supplementi.Decorrenza.GetValueOrDefault(), idRecord.GetValueOrDefault());
                    }
            }
        }

        private static void ArchiviaDatiAventiDiritto(Entity.ParametriARCA parametriArca, GestionePensione.DatiPensione datiPensione,
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoMaster, List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoDaPrelievo, out string errori)
        {
            bool isListaPeriodiTitolareChanged = false;
            GestioneAreaAventiDiritto.MergeAndSaveAventiDiritto(datiPensione, null, listaAventiDirittoMaster, listaAventiDirittoDaPrelievo, GestioneAreaAventiDiritto.EnumTipoMerge.WebDom_GP, parametriArca,
                out isListaPeriodiTitolareChanged, out errori);
        }

        private static void ArchiviaDatiFondo(Utility.TipoFondo? tipoFondo, GestioneFondo.DatiFondo datiPensioneFondoDatiGenericiDaPrelievo,
            ServiceReferences.LiquidazioneFs.GestionePrelievoDatiFondoSpecifico DatiFondoSpecificoDaPrelievo, Entity.DatiFondoSpecificoFELPE datiFondoSpecificoFELPE,
            List<GestioneRecordFondo.DatiRecordFondo> ListaRecordFondoDaPrelievo, long idPensione, bool isDomandaConNuovaGestioneDatiFondoFSPT,
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, BLCommon.GestioneLavorazione.DatiLavorazione datiLavorazione, GestionePensione.DatiPensione datiPensioneMaster, out long idFondo,
            out Dictionary<long, long> dictionaryIdRecordFondo)
        {
            idFondo = 0;
            dictionaryIdRecordFondo = null;

            if (datiPensioneFondoDatiGenericiDaPrelievo == null)
                datiPensioneFondoDatiGenericiDaPrelievo = new GestioneFondo.DatiFondo();

            if (isDomandaConNuovaGestioneDatiFondoFSPT)
                datiPensioneFondoDatiGenericiDaPrelievo.Privilegiate = null;

            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiPensioneFondoDatiGenericiDaPrelievo);

            if (DatiFondoSpecificoDaPrelievo != null)
            {
                idFondo = datiPensioneFondoDatiGenericiDaPrelievo.Id;

                if (isDomandaConNuovaGestioneDatiFondoFSPT || tipoFondo == Utility.TipoFondo.DZ || tipoFondo == Utility.TipoFondo.PI || tipoFondo == Utility.TipoFondo.PL || tipoFondo == Utility.TipoFondo.PM)
                {
                    if (tipoFondo.HasValue)
                    {
                        if (ListaRecordFondoDaPrelievo != null && ListaRecordFondoDaPrelievo.Count > 0)
                        {
                            foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in ListaRecordFondoDaPrelievo)
                            {
                                long progressivo = recordFondo.Id;
                                GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);

                                if (dictionaryIdRecordFondo == null)
                                    dictionaryIdRecordFondo = new Dictionary<long, long>();

                                dictionaryIdRecordFondo.Add(progressivo, recordFondo.Id);
                            }
                        }

                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.PT:
                                List<GestioneFondo.DatiFondoPT> listaDatiPensioneFondoDatiPT = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPT.ToList();
                                if (listaDatiPensioneFondoDatiPT != null && listaDatiPensioneFondoDatiPT.Count > 0 &&
                                    !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiPT.FirstOrDefault(), new GestioneFondo.DatiFondoPT()))
                                {
                                    foreach (GestioneFondo.DatiFondoPT datiFondoPTPrelievo in listaDatiPensioneFondoDatiPT)
                                    {
                                        long idRecordFondo = dictionaryIdRecordFondo[datiFondoPTPrelievo.IdRecordFondo.GetValueOrDefault()];
                                        GestioneFondo.DatiFondoPT datiFondoPT = null;
                                        if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoPT != null)
                                        {
                                            datiFondoPT = datiFondoSpecificoFELPE.DatiFondoPT;
                                            Utility.ValorizzaOggettiMaster(datiFondoPTPrelievo, datiFondoPT);
                                        }
                                        else
                                            datiFondoPT = datiFondoPTPrelievo;
                                        datiFondoPT.IdRecordFondo = idRecordFondo;
                                        datiFondoPT.DecorrenzaEconomica = null;
                                        datiFondoPT.DecorrenzaSecondaria = null;
                                        //ENG - Reversibilità 024 (PL, TRF, RIC)
                                        if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensioneMaster, datiDanteCausa, datiLavorazione))
                                        {
                                            if (datiFondoPTPrelievo.CausaCessazione.HasValue && datiFondoPTPrelievo.CausaCessazione.Value == 0)
                                                datiFondoPTPrelievo.CausaCessazione = null;
                                        }

                                        GestioneFondo.SalvaFondoPTRecordFondo(idFondo, idRecordFondo, datiFondoPT);
                                    }
                                }
                                break;
                            case Utility.TipoFondo.FS:
                                List<GestioneFondo.DatiFondoFST> listaDatiPensioneFondoDatiFS = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiFS.ToList();
                                if (listaDatiPensioneFondoDatiFS != null && listaDatiPensioneFondoDatiFS.Count > 0 &&
                                    !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiFS.FirstOrDefault(), new GestioneFondo.DatiFondoFST()))
                                {
                                    foreach (GestioneFondo.DatiFondoFST datiFondoFSPrelievo in listaDatiPensioneFondoDatiFS)
                                    {
                                        long idRecordFondo = dictionaryIdRecordFondo[datiFondoFSPrelievo.IdRecordFondo.GetValueOrDefault()];
                                        GestioneFondo.DatiFondoFST datiFondoFST = null;
                                        if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoFST != null)
                                        {
                                            datiFondoFST = datiFondoSpecificoFELPE.DatiFondoFST;
                                            Utility.ValorizzaOggettiMaster(datiFondoFSPrelievo, datiFondoFST);
                                        }
                                        else
                                            datiFondoFST = datiFondoFSPrelievo;
                                        datiFondoFST.IdRecordFondo = idRecordFondo;
                                        //ENG - Reversibilità 024 (PL, TRF, RIC)
                                        if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensioneMaster, datiDanteCausa, datiLavorazione))
                                        {
                                            if (datiFondoFSPrelievo.CausaCessazione.HasValue && datiFondoFSPrelievo.CausaCessazione.Value == 0)
                                                datiFondoFSPrelievo.CausaCessazione = null;
                                        }

                                        GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, idRecordFondo, datiFondoFST);
                                    }
                                }
                                break;
                            case Utility.TipoFondo.DZ:
                                List<GestioneFondo.DatiFondoDZ> listaDatiPensioneFondoDatiDZ = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiDZ.ToList();

                                if (listaDatiPensioneFondoDatiDZ != null && listaDatiPensioneFondoDatiDZ.Count > 0 &&
                                            !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiDZ.FirstOrDefault(), new GestioneFondo.DatiFondoDZ()))
                                {
                                    foreach (GestioneFondo.DatiFondoDZ datiFondoDZPrelievo in listaDatiPensioneFondoDatiDZ)
                                    {
                                        long idRecordFondo = dictionaryIdRecordFondo[datiFondoDZPrelievo.IdRecordFondo.GetValueOrDefault()];
                                        GestioneFondo.DatiFondoDZ datiFondoDZ = null;
                                        datiFondoDZ = datiFondoDZPrelievo;
                                        datiFondoDZ.IdRecordFondo = idRecordFondo;
                                        datiFondoDZ.DataCessazioneServizio = datiFondoDZPrelievo.DataCessazioneServizio;
                                        datiFondoDZ.DecorrenzaSecondaria = datiFondoDZPrelievo.DecorrenzaSecondaria;
                                        GestioneFondo.SalvaFondoDZRecordFondo(idFondo, idRecordFondo, datiFondoDZ);
                                    }
                                }
                                break;
                            case Utility.TipoFondo.PM:
                                List<GestioneFondo.DatiFondoPM> listaDatiPensioneFondoDatiPM = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPM.ToList();
                                if (listaDatiPensioneFondoDatiPM != null && listaDatiPensioneFondoDatiPM.Count > 0 &&
                                           !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiPM.FirstOrDefault(), new GestioneFondo.DatiFondoPI()))
                                {
                                    foreach (GestioneFondo.DatiFondoPM datiPensioneFondoDatiPM in listaDatiPensioneFondoDatiPM)
                                    {
                                        if (datiPensioneFondoDatiPM != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiPM, new GestioneFondo.DatiFondoPM()))
                                        {
                                            long idRecordFondo = dictionaryIdRecordFondo[datiPensioneFondoDatiPM.IdRecordFondo.GetValueOrDefault()];
                                            datiPensioneFondoDatiPM.IdRecordFondo = idRecordFondo;
                                            GestioneFondo.SalvaFondoPMRecordFondo(idFondo, idRecordFondo, datiPensioneFondoDatiPM);
                                        }
                                    }
                                }
                                //Dati AGO PM
                                if (DatiFondoSpecificoDaPrelievo != null && DatiFondoSpecificoDaPrelievo.DatiPensioneAgoPM != null)
                                {
                                    foreach (GestioneFondo.DatiAgoPM datiAgoPM in DatiFondoSpecificoDaPrelievo.DatiPensioneAgoPM)
                                    {
                                        if (datiAgoPM != null && !Utility.ConfrontaOggetti(datiAgoPM, new GestioneFondo.DatiAgoPM()))
                                        {
                                            GestioneFondo.SalvaDatiAgoPMRecordFondo(idFondo, datiAgoPM);
                                        }
                                    }
                                }
                                break;
                            
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                //SPOSTATO TUTTO QUI PER PI e PL
                                List<GestioneFondo.DatiFondoPI> listaDatiPensioneFondoDatiPI = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPI != null ? DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPI.ToList() : null;
                                if (listaDatiPensioneFondoDatiPI != null && listaDatiPensioneFondoDatiPI.Count > 0 &&
                                           !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiPI.FirstOrDefault(), new GestioneFondo.DatiFondoPI()))
                                {
                                    foreach (GestioneFondo.DatiFondoPI datiPensioneFondoDatiPI in listaDatiPensioneFondoDatiPI)
                                    {
                                        if (datiPensioneFondoDatiPI != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiPI, new GestioneFondo.DatiFondoPI()))
                                        {
                                            long idRecordFondo = dictionaryIdRecordFondo[datiPensioneFondoDatiPI.IdRecordFondo.GetValueOrDefault()];
                                            datiPensioneFondoDatiPI.IdRecordFondo = idRecordFondo;
                                            //stiamo mettendo il semaforo del singolo record giallo in fase di acquisizione
                                            datiPensioneFondoDatiPI.SemaforoRecord = 1;
                                            GestioneFondo.SalvaFondoPIRecordFondo(idFondo, idRecordFondo, datiPensioneFondoDatiPI);
                                        }
                                    }
                                }

                                //Dati AGO PI
                                if (DatiFondoSpecificoDaPrelievo != null && DatiFondoSpecificoDaPrelievo.DatiPensioneAgoPI != null)
                                {
                                    foreach (GestioneFondo.DatiAgoPI datiAgoPI in DatiFondoSpecificoDaPrelievo.DatiPensioneAgoPI)
                                    {
                                        if (datiAgoPI != null && !Utility.ConfrontaOggetti(datiAgoPI, new GestioneFondo.DatiAgoPI()))
                                        {
                                            //stiamo mettendo il semaforo del singolo record giallo in fase di acquisizione
                                            datiAgoPI.SemaforoRecord = 1;
                                            GestioneFondo.SalvaDatiAgoPIRecordFondo(idFondo, datiAgoPI);
                                        }
                                    }
                                }

                                // Dati AGO TEORICO PI
                                if (DatiFondoSpecificoDaPrelievo != null && DatiFondoSpecificoDaPrelievo.DatiPensioneAgoTeoricoPI != null)
                                {
                                    foreach (GestioneFondo.DatiAgoTeoricoPI datiAgoTeoricoPI in DatiFondoSpecificoDaPrelievo.DatiPensioneAgoTeoricoPI)
                                    {
                                        if (datiAgoTeoricoPI != null && !Utility.ConfrontaOggetti(datiAgoTeoricoPI, new GestioneFondo.DatiAgoTeoricoPI()))
                                        {
                                            GestioneFondo.SalvaDatiAgoTeoricoPIRecordFondo(idFondo, datiAgoTeoricoPI);
                                        }
                                    }
                                }

                                break;
                        }
                    }
                }
                else
                {
                    if (ListaRecordFondoDaPrelievo != null && ListaRecordFondoDaPrelievo.Count > 0)
                    {
                        ListaRecordFondoDaPrelievo.ForEach(x => x.Id = 0);
                        GestioneRecordFondo.SalvaRecordFondo(idPensione, ListaRecordFondoDaPrelievo);
                    }

                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.EL:
                                GestioneFondo.DatiFondoEL datiPensioneFondoDatiEL = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiEL;
                                if (datiPensioneFondoDatiEL != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiEL, new GestioneFondo.DatiFondoEL()))
                                    GestioneFondo.SalvaFondoEL(idFondo, datiPensioneFondoDatiEL);
                                break;
                            case Utility.TipoFondo.TT:
                                GestioneFondo.DatiFondoTT datiPensioneFondoDatiTT = null;
                                if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoTT != null)
                                {
                                    datiPensioneFondoDatiTT = datiFondoSpecificoFELPE.DatiFondoTT;
                                    if (DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiTT != null)
                                        Utility.ValorizzaOggettiMaster(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiTT, datiPensioneFondoDatiTT);
                                }
                                else
                                    datiPensioneFondoDatiTT = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiTT;
                                if (datiPensioneFondoDatiTT != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiTT, new GestioneFondo.DatiFondoTT()))
                                    GestioneFondo.SalvaFondoTT(idFondo, datiPensioneFondoDatiTT);
                                break;
                            case Utility.TipoFondo.ET:
                                GestioneFondo.DatiFondoET datiPensioneFondoDatiET = null;
                                if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoET != null)
                                {
                                    datiPensioneFondoDatiET = datiFondoSpecificoFELPE.DatiFondoET;
                                    if (DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiET != null)
                                        Utility.ValorizzaOggettiMaster(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiET, datiPensioneFondoDatiET);
                                }
                                else
                                    datiPensioneFondoDatiET = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiET;
                                if (datiPensioneFondoDatiET != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiET, new GestioneFondo.DatiFondoET()))
                                    GestioneFondo.SalvaFondoET(idFondo, datiPensioneFondoDatiET);
                                break;
                            case Utility.TipoFondo.VL:
                                GestioneFondo.DatiFondoVL datiPensioneFondoDatiVL = null;
                                if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoVL != null)
                                {
                                    datiPensioneFondoDatiVL = datiFondoSpecificoFELPE.DatiFondoVL;
                                    if (DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiVL != null)
                                        Utility.ValorizzaOggettiMaster(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiVL, datiPensioneFondoDatiVL);
                                }
                                else
                                    datiPensioneFondoDatiVL = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiVL;
                                if (datiPensioneFondoDatiVL != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiVL, new GestioneFondo.DatiFondoVL()))
                                    GestioneFondo.SalvaFondoVL(idFondo, datiPensioneFondoDatiVL);
                                break;
                            case Utility.TipoFondo.PT:
                                GestioneFondo.DatiFondoPT datiPensioneFondoDatiPT = null;
                                if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoPT != null)
                                {
                                    datiPensioneFondoDatiPT = datiFondoSpecificoFELPE.DatiFondoPT;
                                    if (DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPT != null && DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPT.Count() > 0 &&
                                        !Utility.ConfrontaOggetti(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPT.FirstOrDefault(), new GestioneFondo.DatiFondoPT()))
                                    {
                                        Utility.ValorizzaOggettiMaster(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPT.FirstOrDefault(), datiPensioneFondoDatiPT);
                                        datiPensioneFondoDatiPT.IdRecordFondo = null;
                                        datiPensioneFondoDatiPT.ScadenzaBenefici = null;
                                    }
                                }
                                else
                                    datiPensioneFondoDatiPT = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPT.FirstOrDefault();
                                if (datiPensioneFondoDatiPT != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiPT, new GestioneFondo.DatiFondoPT()))
                                    GestioneFondo.SalvaFondoPT(idFondo, datiPensioneFondoDatiPT);
                                break;
                            case Utility.TipoFondo.FS:
                                GestioneFondo.DatiFondoFST datiPensioneFondoDatiFST = null;
                                if (datiFondoSpecificoFELPE != null && datiFondoSpecificoFELPE.DatiFondoFST != null)
                                {
                                    datiPensioneFondoDatiFST = datiFondoSpecificoFELPE.DatiFondoFST;
                                    if (DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiFS != null && DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiFS.Count() > 0 &&
                                        !Utility.ConfrontaOggetti(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiFS.FirstOrDefault(), new GestioneFondo.DatiFondoFST()))
                                    {
                                        Utility.ValorizzaOggettiMaster(DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiFS.FirstOrDefault(), datiPensioneFondoDatiFST);
                                        datiPensioneFondoDatiFST.IdRecordFondo = null;
                                        //datiPensioneFondoDatiFST.RMSSenzaLegge33670QA = null;
                                    }
                                }
                                else
                                    datiPensioneFondoDatiFST = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiFS.FirstOrDefault();
                                if (datiPensioneFondoDatiFST != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiFST, new GestioneFondo.DatiFondoFST()))
                                    GestioneFondo.SalvaFondoFST(idFondo, datiPensioneFondoDatiFST);
                                break;
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                                //SPOSTATO TUTTO SOPRA PER PI E PL
                                List<GestioneFondo.DatiFondoPI> listaDatiPensioneFondoDatiPI = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPI != null ? DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPI.ToList() : null;
                                if (listaDatiPensioneFondoDatiPI != null && listaDatiPensioneFondoDatiPI.Count > 0 &&
                                           !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiPI.FirstOrDefault(), new GestioneFondo.DatiFondoPI()))
                                {
                                    foreach (GestioneFondo.DatiFondoPI datiPensioneFondoDatiPI in listaDatiPensioneFondoDatiPI)
                                    {
                                        if (datiPensioneFondoDatiPI != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiPI, new GestioneFondo.DatiFondoPI()))
                                        {
                                            long idRecordFondo = dictionaryIdRecordFondo[datiPensioneFondoDatiPI.IdRecordFondo.GetValueOrDefault()];
                                            datiPensioneFondoDatiPI.IdRecordFondo = idRecordFondo;

                                            GestioneFondo.SalvaFondoPIRecordFondo(idFondo, idRecordFondo, datiPensioneFondoDatiPI);
                                        }
                                    }
                                }
                                break;
                                //List<GestioneFondo.DatiFondoPI> listaDatiPensioneFondoDatiPI = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPI != null? DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPI.ToList() : null;
                                //if (listaDatiPensioneFondoDatiPI != null && listaDatiPensioneFondoDatiPI.Count > 0 &&
                                //           !Utility.ConfrontaOggetti(listaDatiPensioneFondoDatiPI.FirstOrDefault(), new GestioneFondo.DatiFondoPI()))
                                //{
                                //    foreach (GestioneFondo.DatiFondoPI datiPensioneFondoDatiPI in listaDatiPensioneFondoDatiPI)
                                //    {
                                //        if (datiPensioneFondoDatiPI != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiPI, new GestioneFondo.DatiFondoPI()))
                                //            GestioneFondo.SalvaFondoPI(idFondo, datiPensioneFondoDatiPI);
                                //    }
                                //}

                                ////Dati AGO PI
                                //if (DatiFondoSpecificoDaPrelievo != null && DatiFondoSpecificoDaPrelievo.DatiPensioneAgoPI != null)
                                //{
                                //    foreach (GestioneFondo.DatiAgoPI datiAgoPI in DatiFondoSpecificoDaPrelievo.DatiPensioneAgoPI)
                                //    {
                                //        if (datiAgoPI != null && !Utility.ConfrontaOggetti(datiAgoPI, new GestioneFondo.DatiAgoPI()))
                                //        {
                                //            GestioneFondo.SalvaDatiAgoPI(datiPensioneFondoDatiPI.Id, datiAgoPI);
                                //        }
                                //    }
                                //}

                                //// Dati AGO TEORICO PI
                                //if (DatiFondoSpecificoDaPrelievo != null && DatiFondoSpecificoDaPrelievo.DatiPensioneAgoTeoricoPI != null)
                                //{
                                //    foreach (GestioneFondo.DatiAgoTeoricoPI datiAgoTeoricoPI in DatiFondoSpecificoDaPrelievo.DatiPensioneAgoTeoricoPI)
                                //    {
                                //        if (datiAgoTeoricoPI != null &&!Utility.ConfrontaOggetti( datiAgoTeoricoPI,new GestioneFondo.DatiAgoTeoricoPI()))
                                //        {
                                //            GestioneFondo.SalvaDatiAgoTeoricoPI( datiPensioneFondoDatiPI.Id, datiAgoTeoricoPI);
                                //        }
                                //    }
                                //}

                                break;
                            case Utility.TipoFondo.GAS:
                                GestioneFondo.DatiFondoGAS datiPensioneFondoDatiGAS = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiGAS;
                                if (datiPensioneFondoDatiGAS != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiGAS, new GestioneFondo.DatiFondoGAS()))
                                    GestioneFondo.SalvaFondoGAS(idFondo, datiPensioneFondoDatiGAS);
                                break;
                            case Utility.TipoFondo.CL:
                                GestioneFondo.DatiFondoCL datiPensioneFondoDatiCL = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiCL;
                                if (datiPensioneFondoDatiCL != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiCL, new GestioneFondo.DatiFondoCL()))
                                    GestioneFondo.SalvaFondoCL(idFondo, datiPensioneFondoDatiCL);
                                break;
                            case Utility.TipoFondo.ES:
                                GestioneFondo.DatiFondoES datiPensioneFondoDatiES = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiES;
                                if (datiPensioneFondoDatiES != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiES, new GestioneFondo.DatiFondoES()))
                                    GestioneFondo.SalvaFondoES(idFondo, datiPensioneFondoDatiES);
                                break;

                            case Utility.TipoFondo.PM:
                                //SPOSTATO SOPRA CME LISTA
                                GestioneFondo.DatiFondoPM datiPensioneFondoDatiPM = DatiFondoSpecificoDaPrelievo.DatiPensioneFondoDatiPM.ToList().FirstOrDefault();
                                if (datiPensioneFondoDatiPM != null && !Utility.ConfrontaOggetti(datiPensioneFondoDatiPM, new GestioneFondo.DatiFondoPM()))
                                    GestioneFondo.SalvaFondoPM(idFondo, datiPensioneFondoDatiPM);
                                break;
                        }
                    }
                }
            }
        }

        private static void ArchiviaDatiFondoINPDAP(GestioneFondo.DatiFondo datiPensioneFondoDatiGenericiDaPrelievo,
            List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo_Prelievo,
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordFondoINPDAP_Prelievo,
            GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP_FELPE,
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoINPDAP_FELPE,
            List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP_GP,
            long idPensione, out long idFondo,
            out Dictionary<long, long> dictionaryIdRecordFondo)
        {
            idFondo = 0;
            dictionaryIdRecordFondo = null;

            if (datiPensioneFondoDatiGenericiDaPrelievo == null)
                datiPensioneFondoDatiGenericiDaPrelievo = new GestioneFondo.DatiFondo();

            datiPensioneFondoDatiGenericiDaPrelievo.Privilegiate = null;

            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiPensioneFondoDatiGenericiDaPrelievo);

            idFondo = datiPensioneFondoDatiGenericiDaPrelievo.Id;

            if (listaRecordFondo_Prelievo != null && listaRecordFondo_Prelievo.Count > 0)
            {
                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaRecordFondo_Prelievo)
                {
                    long progressivo = recordFondo.Id;
                    GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);

                    if (dictionaryIdRecordFondo == null)
                        dictionaryIdRecordFondo = new Dictionary<long, long>();

                    dictionaryIdRecordFondo.Add(progressivo, recordFondo.Id);
                }

                if (listaDatiPensioneINPDAP != null && listaDatiPensioneINPDAP.Count > 0 &&
                    !Utility.ConfrontaOggetti(listaDatiPensioneINPDAP.FirstOrDefault(), new GestionePensioneINPDAP.DatiPensioneINPDAP()))
                {
                    foreach (GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAP_Prelievo in listaDatiPensioneINPDAP)
                    {
                        long idRecordFondo = dictionaryIdRecordFondo[datiPensioneINPDAP_Prelievo.IdRecordFondo.GetValueOrDefault()];
                        ArchiviaDatiPensioneINPDAP(datiPensioneINPDAP_FELPE, datiPensioneINPDAP_Prelievo, idPensione, idRecordFondo);
                    }
                }

                if (listaRecordDatiFondoINPDAP_GP != null && listaRecordDatiFondoINPDAP_GP.Count > 0)
                {
                    foreach (GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoINPDAP_GP in listaRecordDatiFondoINPDAP_GP)
                    {
                        long idRecordFondo = dictionaryIdRecordFondo[datiRecordFondoINPDAP_GP.IdRecordFondo];
                        ArchiviaRecordFondoINPDAP(datiRecordFondoINPDAP_FELPE, datiRecordFondoINPDAP_GP, idPensione, idRecordFondo);
                    }
                }
                else if (listaRecordFondoINPDAP_Prelievo != null && listaRecordFondoINPDAP_Prelievo.Count > 0 &&
                    !Utility.ConfrontaOggetti(listaRecordFondoINPDAP_Prelievo.FirstOrDefault(), new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP()))
                {
                    foreach (GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoINPDAP_Prelievo in listaRecordFondoINPDAP_Prelievo)
                    {
                        long idRecordFondo = dictionaryIdRecordFondo[datiRecordFondoINPDAP_Prelievo.IdRecordFondo];
                        ArchiviaRecordFondoINPDAP(datiRecordFondoINPDAP_FELPE, datiRecordFondoINPDAP_Prelievo, idPensione, idRecordFondo);
                    }
                }
            }
        }

        private static void ArchiviaDatiPensioneINPDAP(GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAPMaster, GestionePensioneINPDAP.DatiPensioneINPDAP datiPensioneINPDAPDaPrelievo,
            long idPensione, long idRecordFondo)
        {
            if (datiPensioneINPDAPMaster != null)
            {
                if (datiPensioneINPDAPDaPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiPensioneINPDAPDaPrelievo, datiPensioneINPDAPMaster);
                GestionePensioneINPDAP.SalvaPensioneINPDAPRecordFondo(idPensione, idRecordFondo, datiPensioneINPDAPMaster);
            }
            else if (datiPensioneINPDAPDaPrelievo != null)
                GestionePensioneINPDAP.SalvaPensioneINPDAPRecordFondo(idPensione, idRecordFondo, datiPensioneINPDAPDaPrelievo);
        }

        private static void ArchiviaRecordFondoINPDAP(GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoMaster, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP datiRecordFondoPrelievo,
            long idPensione, long idRecordFondo)
        {
            if (datiRecordFondoMaster != null)
            {
                if (datiRecordFondoPrelievo != null)
                    Utility.ValorizzaOggettiMaster(datiRecordFondoPrelievo, datiRecordFondoMaster);
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(idPensione, idRecordFondo, datiRecordFondoMaster);
            }
            else if (datiRecordFondoPrelievo != null)
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(idPensione, idRecordFondo, datiRecordFondoPrelievo);
        }
        #endregion ArchiviazioneConDatiMaster

        #region ArchiviazioneSenzaDatiMaster
        private static void ArchiviaDatiEliminazione(GestionePensione.DatiEliminazione datiEliminazioneDaPrelievo, long idPensione)
        {
            if (datiEliminazioneDaPrelievo != null && !Utility.ConfrontaOggetti(datiEliminazioneDaPrelievo, new GestionePensione.DatiEliminazione()))
                GestionePensione.SalvaEliminazione(idPensione, datiEliminazioneDaPrelievo);
        }

        private static void ArchiviaDatiSindacato(GestionePensione.DatiSindacato datiSindacatoMaster, GestionePensione.DatiSindacato datiSindacatoDaPrelievo, long idPensione)
        {
            // Ha priorità il sindacato proveniente da WebDom rispetto a quello del prelievo

            if (datiSindacatoMaster != null && !Utility.ConfrontaOggetti(datiSindacatoMaster, new GestionePensione.DatiSindacato()))
                GestionePensione.SalvaSindacato(idPensione, datiSindacatoMaster);
            else if (datiSindacatoDaPrelievo != null && !Utility.ConfrontaOggetti(datiSindacatoDaPrelievo, new GestionePensione.DatiSindacato()))
                GestionePensione.SalvaSindacato(idPensione, datiSindacatoDaPrelievo);
        }

        private static void ArchiviaDatiDetrazioni(GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioniDaPrelievo, long idPensione)
        {
            if (datiDetrazioniDaPrelievo != null && !Utility.ConfrontaOggetti(datiDetrazioniDaPrelievo, new GestioneDetrazioniImposta.DatiDetrazioni()))
                GestioneDetrazioniImposta.SalvaDetrazioni(idPensione, datiDetrazioniDaPrelievo);
        }

        private static void ArchiviaDatiDetrazioniContitolare(List<GestioneDetrazioniContitolare.DatiDetrazioniContitolareRecuperato> listaDetrazioniDaPrelievo,
            List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari, long idPensione)
        {
            if (listaDetrazioniDaPrelievo != null && listaDetrazioniDaPrelievo.Count > 0 && listaFamiliari != null && listaFamiliari.Count > 0)
            {
                foreach (var detrazioni in listaDetrazioniDaPrelievo)
                {
                    GestioneAreaFamiliari.AreaFamiliare familiare = listaFamiliari.FirstOrDefault(x => x.Familiare.CodiceFiscale == detrazioni.CodiceFiscale);
                    if (familiare != null)
                        GestioneDetrazioniContitolare.SalvaDetrazioni(idPensione, familiare.Familiare.IdAnagrafica, detrazioni);
                }
            }
        }

        private static void ArchiviaDatiFamiliare(Entity.ParametriARCA parametriArca, object ListaFamiliariDaPrelievo, GestioneAnagrafica.DatiAnagrafici anagraficaTitolare,
            Utility.TipoAppartenenza tipoAppartenenza, Utility.TipoDomanda tipoDomanda, GestionePensione.DatiPensione datiPensione, bool isRiapertura, out bool familiariDaPrelievo, out bool isFamiliariVerde,
             out List<GestioneAreaFamiliari.AreaFamiliare> listaFamiliari, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa, out string errori)
        {
            errori = string.Empty;
            familiariDaPrelievo = false;
            isFamiliariVerde = false;
            listaFamiliari = null;
            List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> listaRichiesteRicercaDomandeANF = null;
            byte? codiceMaggiorazione = null;
            DateTime? dataSistema = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (ListaFamiliariDaPrelievo != null)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        List<ServiceReferences.LiquidazioneFs.DatiFamiliari> datiFamiliariFs = ((ServiceReferences.LiquidazioneFs.DatiFamiliari[])ListaFamiliariDaPrelievo).ToList();
                        if (datiFamiliariFs != null && datiFamiliariFs.Count > 0)
                        {
                            foreach (ServiceReferences.LiquidazioneFs.DatiFamiliari famPrel in datiFamiliariFs)
                            {
                                GestioneAreaFamiliari.AreaFamiliare fam = new GestioneAreaFamiliari.AreaFamiliare();
                                fam.Familiare = famPrel.Familiare;
                                if (fam.Familiare != null)
                                    fam.Familiare.Provenienza = 'P';

                                if (fam.Familiare.CodiceFiscale != anagraficaTitolare.CodiceFiscale || !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                                    || !(controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiapertura)))
                                    fam.ElencoCodMaggFamiliari = famPrel.ElencoCodMaggFamiliari != null ? famPrel.ElencoCodMaggFamiliari.ToList() : null;
                                if (listaFamiliari == null)
                                    listaFamiliari = new List<GestioneAreaFamiliari.AreaFamiliare>();
                                listaFamiliari.Add(fam);
                            }
                            dataSistema = Utility.DataSistemaFs;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        List<ServiceReferences.LiquidazioneCi.DatiFamiliari> datiFamiliariCi = ((ServiceReferences.LiquidazioneCi.DatiFamiliari[])ListaFamiliariDaPrelievo).ToList();
                        if (datiFamiliariCi != null && datiFamiliariCi.Count > 0)
                        {
                            foreach (ServiceReferences.LiquidazioneCi.DatiFamiliari famPrel in datiFamiliariCi)
                            {
                                GestioneAreaFamiliari.AreaFamiliare fam = new GestioneAreaFamiliari.AreaFamiliare();
                                fam.Familiare = famPrel.Familiare;
                                if (fam.Familiare != null)
                                    fam.Familiare.Provenienza = 'P';
                                fam.ElencoCodMaggFamiliari = famPrel.ElencoCodMaggFamiliari != null ? famPrel.ElencoCodMaggFamiliari.ToList() : null;
                                if (listaFamiliari == null)
                                    listaFamiliari = new List<GestioneAreaFamiliari.AreaFamiliare>();
                                listaFamiliari.Add(fam);
                            }
                            codiceMaggiorazione = 1;
                            dataSistema = Utility.DataSistemaCi;
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        List<ServiceReferences.LiquidazioneAgo.DatiFamiliari> datiFamiliariAgo = ((ServiceReferences.LiquidazioneAgo.DatiFamiliari[])ListaFamiliariDaPrelievo).ToList();
                        if (datiFamiliariAgo != null && datiFamiliariAgo.Count > 0)
                        {
                            foreach (ServiceReferences.LiquidazioneAgo.DatiFamiliari famPrel in datiFamiliariAgo)
                            {
                                GestioneAreaFamiliari.AreaFamiliare fam = new GestioneAreaFamiliari.AreaFamiliare();
                                fam.Familiare = famPrel.Familiare;
                                if (fam.Familiare != null)
                                    fam.Familiare.Provenienza = 'P';
                                // Settiamo il semaforo del singolo familiare per ricostituzioni e reversibilità, poichè ci arrivano dal prelievo
                                fam.Familiare.Confermato = famPrel.Familiare.Confermato;

                                //ENG - Spacchettate SOPGI
                                if (fam.Familiare.CodiceFiscale != anagraficaTitolare.CodiceFiscale || (!Utility.IsDomandaSpacchettamentoENPALS(datiPensione) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) && !Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiapertura)
                                    && !Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiapertura) && !Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiapertura) && !Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiapertura)))
                                    fam.ElencoCodMaggFamiliari = famPrel.ElencoCodMaggFamiliari != null ? famPrel.ElencoCodMaggFamiliari.ToList() : null;
                                if (listaFamiliari == null)
                                    listaFamiliari = new List<GestioneAreaFamiliari.AreaFamiliare>();
                                listaFamiliari.Add(fam);
                            }
                            codiceMaggiorazione = 2;
                            dataSistema = Utility.DataSistemaAgo;
                        }
                        break;
                }

                if (listaFamiliari != null && listaFamiliari.Count > 0)
                {
                    List<Entity.Anagrafica> ListaAnagraficheFamiliari = null;
                    listaRichiesteRicercaDomandeANF = new List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF>();
                    GestioneControlliDinamici.ControlloDinamico ctrl = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ConsultazioneANFAttiva" + tipoAppartenenza.ToString(), out ctrl);
                    for (int i = 0; i < listaFamiliari.Count; i++)
                    {
                        GestioneAreaFamiliari.AreaFamiliare familiare = listaFamiliari[i];
                        if (familiare.Familiare != null)
                        {
                            GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                            richiestaArca.Applicazione = parametriArca.Applicazione;
                            richiestaArca.Matricola = parametriArca.Matricola;
                            richiestaArca.Provenienza = parametriArca.Provenienza;
                            richiestaArca.Ruolo = parametriArca.Ruolo;

                            richiestaArca.CodiceFiscaleRichiedente = familiare.Familiare.CodiceFiscale;
                            richiestaArca.CodiceFiscale = familiare.Familiare.CodiceFiscale;

                            Entity.Anagrafica anagraficaDA = null;
                            if (!string.IsNullOrEmpty(richiestaArca.CodiceFiscale))
                                if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, datiPensione.NDomus.ToString(), out anagraficaDA, out errori))
                                    return;

                            if (anagraficaDA != null)
                            {
                                if (ListaAnagraficheFamiliari == null)
                                    ListaAnagraficheFamiliari = new List<INPS.Pensioni.Liquidazione.Entity.Anagrafica>();
                                ListaAnagraficheFamiliari.Add(anagraficaDA);
                                if (!familiare.Familiare.DataMorte.HasValue && anagraficaDA.DataMorte.HasValue)
                                    familiare.Familiare.DataMorte = anagraficaDA.DataMorte;

                                if (ctrl != null && ctrl.ValoreControllo == "SI" && familiare != null && familiare.ElencoCodMaggFamiliari != null &&
                                    familiare.ElencoCodMaggFamiliari.Count > 0 && familiare.ElencoCodMaggFamiliari.Any(x => x.CodiceMaggiorazione == codiceMaggiorazione))
                                {
                                    string guidANF;
                                    GestioneANF.RicercaDomandeANFByCodiceFiscale(datiPensione.NDomus.ToString(), familiare.Familiare.CodiceFiscale, parametriArca.Matricola, out guidANF, out errori);
                                    if (!string.IsNullOrEmpty(errori))
                                        errori = string.Empty;
                                    else if (!string.IsNullOrEmpty(guidANF))
                                    {
                                        GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiesta = new GestioneFamiliari.DatiRichiestaRicercaDomandeANF();
                                        richiesta.DataRichiesta = dataSistema.GetValueOrDefault();
                                        richiesta.CodiceFiscale = familiare.Familiare.CodiceFiscale;
                                        richiesta.Guid = guidANF;
                                        listaRichiesteRicercaDomandeANF.Add(richiesta);
                                    }
                                }
                            }
                            else
                            {
                                listaFamiliari.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    List<GestioneAnagrafica.DatiAnagrafici> elencoAnagraficheDB = null;
                    if (ListaAnagraficheFamiliari != null)
                    {
                        elencoAnagraficheDB = new List<GestioneAnagrafica.DatiAnagrafici>();
                        foreach (Entity.Anagrafica anagrafica in ListaAnagraficheFamiliari)
                        {
                            GestioneAnagrafica.DatiAnagrafici anagraficaDB = new GestioneAnagrafica.DatiAnagrafici();
                            Utility.ValorizzaOggetti(anagrafica, anagraficaDB);
                            elencoAnagraficheDB.Add(anagraficaDB);
                        }
                    }

                    if (listaFamiliari != null && listaFamiliari.Count > 0 && ListaAnagraficheFamiliari != null && elencoAnagraficheDB.Count > 0 &&
                        listaFamiliari.Count == elencoAnagraficheDB.Count)
                    {
                        if ((tipoAppartenenza == Utility.TipoAppartenenza.AGO || tipoAppartenenza == Utility.TipoAppartenenza.FS || tipoAppartenenza == Utility.TipoAppartenenza.CI) &&
                            (tipoDomanda == Utility.TipoDomanda.Ricostituzione || tipoDomanda == Utility.TipoDomanda.Ripristino || isRiapertura)
                            &&
                            (listaFamiliari.Count == 1 && listaFamiliari.First().Familiare.TipoComponente == 'T')
                            ||
                            (!listaFamiliari.Exists(x => x.Familiare != null && !x.Familiare.Confermato))
                            )
                            isFamiliariVerde = true;

                        for (int i = 0; i < listaFamiliari.Count; i++)
                        {
                            if (listaFamiliari[i].Familiare.TipoComponente == 'T' && tipoAppartenenza == Utility.TipoAppartenenza.AGO &&
                                (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && anagraficaTitolare != null && !string.IsNullOrEmpty(anagraficaTitolare.Cittadinanza))
                                elencoAnagraficheDB[i].Cittadinanza = anagraficaTitolare.Cittadinanza;

                            string cfFamiliare = listaFamiliari[i].Familiare.CodiceFiscale;
                            GestioneFamiliari.DatiRichiestaRicercaDomandeANF richiesta = listaRichiesteRicercaDomandeANF != null ? listaRichiesteRicercaDomandeANF.FirstOrDefault(x => x.CodiceFiscale == cfFamiliare) : null;
                            GestioneFamiliari.SalvaFamiliare(listaFamiliari[i].Familiare, listaFamiliari[i].ElencoCodMaggFamiliari, elencoAnagraficheDB[i], richiesta, datiPensione.Id, datiPensione.SiglaCategoria);
                        }
                        familiariDaPrelievo = true;
                    }
                }
            }
        }

        private static void ArchiviaFamiliari(Entity.ParametriARCA parametriArca, List<GestioneFamiliari.FamiliareRecuperato> CFfamiliari, GestionePensione.DatiPensione datiPensione, object ListaFamiliariDaPrelievo, out string errori)
        {
            errori = string.Empty;
            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (CFfamiliari != null && CFfamiliari.Count > 0)
            {
                List<GestioneAreaFamiliari.AreaFamiliare> listaFam = new List<GestioneAreaFamiliari.AreaFamiliare>();
                if (categoriaFondoPI != null)
                {
                    List<ServiceReferences.LiquidazioneFs.DatiFamiliari> datiFamiliariFs = new List<ServiceReferences.LiquidazioneFs.DatiFamiliari>();
                    if (ListaFamiliariDaPrelievo != null)
                    {
                        datiFamiliariFs = ((ServiceReferences.LiquidazioneFs.DatiFamiliari[])ListaFamiliariDaPrelievo).ToList();
                    }

                    if (datiFamiliariFs != null && datiFamiliariFs.Count > 0)
                    {

                        foreach (ServiceReferences.LiquidazioneFs.DatiFamiliari famPrel in datiFamiliariFs)
                        {
                            GestioneAreaFamiliari.AreaFamiliare familiare = new GestioneAreaFamiliari.AreaFamiliare();
                            familiare.Familiare = famPrel.Familiare;

                            familiare.ElencoCodMaggFamiliari = famPrel.ElencoCodMaggFamiliari != null ? famPrel.ElencoCodMaggFamiliari.ToList() : null;

                            listaFam.Add(familiare);
                        }
                    }
                }
                List<Entity.Anagrafica> ListaAnagraficheFamiliari = null;
                List<GestioneFamiliari.Familiare> ListaFamiliari = null;
                foreach (GestioneFamiliari.FamiliareRecuperato de in CFfamiliari)
                {
                    GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                    richiestaArca.Applicazione = parametriArca.Applicazione;
                    richiestaArca.Matricola = parametriArca.Matricola;
                    richiestaArca.Provenienza = parametriArca.Provenienza;
                    richiestaArca.Ruolo = parametriArca.Ruolo;

                    richiestaArca.CodiceFiscaleRichiedente = de.CodiceFiscale;
                    richiestaArca.CodiceFiscale = de.CodiceFiscale;

                    Entity.Anagrafica anagraficaDA = null;
                    if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, datiPensione.NDomus.ToString(), out anagraficaDA, out errori))
                        return;

                    if (anagraficaDA != null)
                    {
                        if (ListaAnagraficheFamiliari == null)
                            ListaAnagraficheFamiliari = new List<INPS.Pensioni.Liquidazione.Entity.Anagrafica>();
                        if (ListaFamiliari == null)
                            ListaFamiliari = new List<GestioneFamiliari.Familiare>();
                        ListaAnagraficheFamiliari.Add(anagraficaDA);
                        GestioneFamiliari.Familiare fam = new GestioneFamiliari.Familiare();
                        fam.CodiceFiscale = de.CodiceFiscale;
                        fam.IdPensione = datiPensione.Id;
                        fam.SiglaFamiliare = de.SiglaFamiliare;
                        fam.TipoComponente = de.TipoComponente;
                        fam.TipoUnione = de.TipoUnione;
                        fam.DataMorte = anagraficaDA.DataMorte;
                        fam.Provenienza = 'W';

                        ListaFamiliari.Add(fam);
                    }
                }

                List<GestioneAnagrafica.DatiAnagrafici> elencoAnagraficheDB = null;
                if (ListaAnagraficheFamiliari != null)
                {
                    elencoAnagraficheDB = new List<GestioneAnagrafica.DatiAnagrafici>();
                    foreach (Entity.Anagrafica anagrafica in ListaAnagraficheFamiliari)
                    {
                        GestioneAnagrafica.DatiAnagrafici anagraficaDB = new GestioneAnagrafica.DatiAnagrafici();
                        Utility.ValorizzaOggetti(anagrafica, anagraficaDB);
                        elencoAnagraficheDB.Add(anagraficaDB);
                    }
                }

                if (ListaFamiliari != null && ListaFamiliari.Count > 0 && elencoAnagraficheDB.Count > 0 &&
                    ListaFamiliari.Count == elencoAnagraficheDB.Count)
                {
                    if (categoriaFondoPI == null)
                    {
                        for (int i = 0; i < ListaFamiliari.Count; i++)
                            GestioneFamiliari.SalvaFamiliare(ListaFamiliari[i], null, elencoAnagraficheDB[i], null, datiPensione.Id, datiPensione.SiglaCategoria);

                    }
                    else
                    {
                        for (int i = 0; i < ListaFamiliari.Count; i++)
                        {
                            GestioneAreaFamiliari.AreaFamiliare familiare = listaFam != null ? listaFam.ElementAtOrDefault<GestioneAreaFamiliari.AreaFamiliare>(i) : null;
                            GestioneFamiliari.SalvaFamiliare(ListaFamiliari[i], familiare != null ? familiare.ElencoCodMaggFamiliari : null, elencoAnagraficheDB[i], null, datiPensione.Id, datiPensione.SiglaCategoria);
                        }
                    }

                }
            }
        }

        private static void ArchiviaDatiCalcoloContributivoFS(List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiCalcoloContributivoDaPrelievo, long idPensione, Dictionary<long, long> dictionaryIdRecordFondo,
            Utility.TipoFondo? tipoFondo, bool isDomandaConNuovaGestioneDatiFondoFSPT, bool isDomandaINPDAP, GestionePensione.DatiPensione DatiPensioneMaster)
        {
            if (listaDatiCalcoloContributivoDaPrelievo != null && listaDatiCalcoloContributivoDaPrelievo.Count > 0)
            {
                if ((tipoFondo.HasValue && (tipoFondo.Value == Utility.TipoFondo.FS || tipoFondo.Value == Utility.TipoFondo.PT) && isDomandaConNuovaGestioneDatiFondoFSPT) ||
                    isDomandaINPDAP)
                {
                    foreach (GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivoDaPrelievo in listaDatiCalcoloContributivoDaPrelievo)
                    {
                        if (!Utility.ConfrontaOggetti(datiCalcoloContributivoDaPrelievo, new GestioneCalcolo.DatiCalcoloContributivo()))
                        {
                            try
                            {
                                if (dictionaryIdRecordFondo != null)
                                {
                                    long idRecordFondo = dictionaryIdRecordFondo[datiCalcoloContributivoDaPrelievo.IdRecordFondo];
                                    datiCalcoloContributivoDaPrelievo.IdRecordFondo = idRecordFondo;
                                }
                            }
                            catch (Exception)
                            {
                                // Eccezione ignorata
                            }

                            datiCalcoloContributivoDaPrelievo.IdPensione = idPensione;
                            GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(datiCalcoloContributivoDaPrelievo);
                        }
                    }
                }
                else
                {
                    if (!Utility.ConfrontaOggetti(listaDatiCalcoloContributivoDaPrelievo.FirstOrDefault(), new GestioneCalcolo.DatiCalcoloContributivo()))
                    {
                        if (tipoFondo == Utility.TipoFondo.DZ)
                        {
                            if (listaDatiCalcoloContributivoDaPrelievo.Any())
                            {
                                listaDatiCalcoloContributivoDaPrelievo.ForEach(x =>
                                {
                                    if (checkContributivoDazio(x))
                                    {
                                        var a = x.Id;
                                        if (dictionaryIdRecordFondo != null)
                                        {
                                            long idRecordFondo = dictionaryIdRecordFondo[x.Id];
                                            x.IdRecordFondo = idRecordFondo;
                                            x.IdPensione = idPensione;
                                        }

                                        GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(x);
                                    }
                                });
                            }
                        }
                        else
                        {
                            GestioneCalcolo.DatiCalcoloContributivo datiCalcoloContributivoDaPrelievo = listaDatiCalcoloContributivoDaPrelievo.FirstOrDefault();
                            datiCalcoloContributivoDaPrelievo.IdPensione = idPensione;
                            GestioneCalcolo.SalvaCalcoloContributivo(datiCalcoloContributivoDaPrelievo);
                        }
                    }
                }
            }
        }

        private static bool checkContributivoDazio(GestioneCalcolo.DatiCalcoloContributivo DC)
        {
            bool retVal = false;
            if (DC.MontanteQuotaDL214 != null || DC.NSettimaneQuotaDL214 != null || DC.ImportoContribTotaleQuotaDL214 != null)
            {
                retVal = true;
            }

            return retVal;
        }

        private static void ArchiviaDatiCalcoloContributivoAGO_CI(List<GestioneCalcolo.DatiCalcoloContributivo> ListaCalcoloContributivoDaPrelievo, long idPensione)
        {
            if (ListaCalcoloContributivoDaPrelievo != null && ListaCalcoloContributivoDaPrelievo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivo calcContr in ListaCalcoloContributivoDaPrelievo)
                {
                    calcContr.IdPensione = idPensione;
                }
                GestioneCalcolo.SalvaListCalcoloContributivoCI_AGO(ListaCalcoloContributivoDaPrelievo);
            }
        }

        private static void ArchiviaDatiSupplementiCumulo(List<BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiMaster, List<BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiDaPrelievo, long idPensione)
        {
            if (listaSupplementiMaster != null && listaSupplementiMaster.Count > 0)
            {
                listaSupplementiMaster.ForEach(x => x.IdPensione = idPensione);
                GestioneSupplementi.SalvaDatiSupplementiCumulo(listaSupplementiMaster);
            }
            if (listaSupplementiDaPrelievo != null && listaSupplementiDaPrelievo.Count > 0)
            {
                listaSupplementiDaPrelievo.ForEach(x => x.IdPensione = idPensione);
                GestioneSupplementi.SalvaDatiSupplementiCumulo(listaSupplementiDaPrelievo);
            }
        }

        private static void ArchiviaDatiSupplementiStorico(List<BLCommon.Entity.DatiSupplementi> listaSupplementiMaster, List<BLCommon.Entity.DatiSupplementi> listaSupplementiDaPrelievo, long idPensione)
        {
            if (listaSupplementiMaster != null && listaSupplementiMaster.Count > 0)
            {
                listaSupplementiMaster.ForEach(x => x.IdPensione = idPensione);
                GestioneSupplementi.SalvaDatiSupplementiStorico(idPensione, listaSupplementiMaster);
            }
            if (listaSupplementiDaPrelievo != null && listaSupplementiDaPrelievo.Count > 0)
            {
                listaSupplementiDaPrelievo.ForEach(x => x.IdPensione = idPensione);
                GestioneSupplementi.SalvaDatiSupplementiStorico(idPensione, listaSupplementiDaPrelievo);
            }
        }

        private static void ArchiviaDatiQuotePensione(List<GestioneCalcolo.QuotePensione> ListaQuotePensioneMaster, List<GestioneCalcolo.QuotePensione> ListaQuotePensioneDaPrelievo, long idPensione)
        {
            if (ListaQuotePensioneMaster != null && ListaQuotePensioneMaster.Count > 0)
            {
                ListaQuotePensioneMaster.ForEach(x => x.IdPensione = idPensione);
                GestioneCalcolo.SalvaListaQuotePensione(ListaQuotePensioneMaster);
            }
            else if (ListaQuotePensioneDaPrelievo != null && ListaQuotePensioneDaPrelievo.Count > 0)
            {
                ListaQuotePensioneDaPrelievo.ForEach(x => x.IdPensione = idPensione);
                GestioneCalcolo.SalvaListaQuotePensione(ListaQuotePensioneDaPrelievo);
            }
        }

        private static void ArchiviaDatiQuoteMiglioramentiContrattuali(List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> ListaQuoteMiglioramentiContrattualiMaster, List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> ListaMiglioramentiContrattualiDaPrelievo, long idPensione)
        {
            if (ListaQuoteMiglioramentiContrattualiMaster != null && ListaQuoteMiglioramentiContrattualiMaster.Count > 0)
            {
                ListaQuoteMiglioramentiContrattualiMaster.ForEach(x => x.IdPensione = idPensione);
                GestioneMiglioramentiContrattuali.SalvaListaQuotaMiglioramentiContrattuali(ListaQuoteMiglioramentiContrattualiMaster);
            }
            else if (ListaMiglioramentiContrattualiDaPrelievo != null && ListaMiglioramentiContrattualiDaPrelievo.Count > 0)
            {
                ListaMiglioramentiContrattualiDaPrelievo.ForEach(x => x.IdPensione = idPensione);
                GestioneMiglioramentiContrattuali.SalvaListaQuotaMiglioramentiContrattuali(ListaMiglioramentiContrattualiDaPrelievo);
            }
        }

        private static void ArchiviaDatiMiglioramentiContrattuali(GestioneMiglioramentiContrattuali.DatiMiglioramentiContrattuali miglioramentiContrattualiMaster, long idPensione)
        {
            //Non c'è prelievo?
            if (miglioramentiContrattualiMaster != null)
            {
                miglioramentiContrattualiMaster.IdPensione = idPensione;
                GestioneMiglioramentiContrattuali.SalvaMiglioramentiContrattuali(miglioramentiContrattualiMaster);
            }
        }

        private static void ArchiviaDatiTrattenuteQuotePensione(List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenuteQuotePensioneMaster, List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenuteQuotePensioneDaPrelievo, long idPensione)
        {
            if (listaTrattenuteQuotePensioneMaster != null && listaTrattenuteQuotePensioneMaster.Count > 0)
            {
                listaTrattenuteQuotePensioneMaster.ForEach(x => x.IdPensione = idPensione);
                GestioneCalcolo.SalvaListaTrattenuteQuotePensione(listaTrattenuteQuotePensioneMaster);
            }
            else if (listaTrattenuteQuotePensioneDaPrelievo != null && listaTrattenuteQuotePensioneDaPrelievo.Count > 0)
            {
                listaTrattenuteQuotePensioneDaPrelievo.ForEach(x => x.IdPensione = idPensione);
                GestioneCalcolo.SalvaListaTrattenuteQuotePensione(listaTrattenuteQuotePensioneDaPrelievo);
            }
        }

        private static void ArchiviaDatiCalcoloRetributivoDZ(List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiCalcoloRetributivo, Dictionary<long, long> dictionaryIdRecordFondo, long idPensione)
        {
            if (listaDatiCalcoloRetributivo.Any())
            {
                listaDatiCalcoloRetributivo.ForEach(x =>
                {
                    if (x.RMSQuotaA != null || x.NSettimaneQuotaA != null || x.RMSQuotaB != null || x.NSettimaneQuotaB != null)
                    {
                        var a = x.Id;
                        if (dictionaryIdRecordFondo != null)
                        {
                            long idRecordFondo = dictionaryIdRecordFondo[x.Id];
                            x.IdRecordFondo = idRecordFondo;
                            x.IdPensione = idPensione;
                        }

                        GestioneCalcolo.SalvaCalcoloRetributivoRecordFondo(x);
                    }
                });
            }
        }

        private static void ArchiviaDatiCalcoloRetributivo(GestioneCalcolo.DatiCalcoloRetributivo datiCalcoloRetributivoDaPrelievo, long idPensione)
        {
            if (datiCalcoloRetributivoDaPrelievo != null && !Utility.ConfrontaOggetti(datiCalcoloRetributivoDaPrelievo, new GestioneCalcolo.DatiCalcoloRetributivo()))
            {
                datiCalcoloRetributivoDaPrelievo.IdPensione = idPensione;
                GestioneCalcolo.SalvaCalcoloRetributivo(datiCalcoloRetributivoDaPrelievo);
            }
        }

        private static void ArchiviaDatiCalcoloRetributivo(List<GestioneCalcolo.DatiCalcoloRetributivo> ListaCalcoloRetributivoDaPrelievo, long idPensione)
        {
            if (ListaCalcoloRetributivoDaPrelievo != null && ListaCalcoloRetributivoDaPrelievo.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloRetributivo calcRetr in ListaCalcoloRetributivoDaPrelievo)
                {
                    calcRetr.IdPensione = idPensione;
                }
                GestioneCalcolo.SalvaListaCalcoloRetributivoCI_AGO(ListaCalcoloRetributivoDaPrelievo);
            }
        }

        private static void ArchiviaDatiCalcoloRetributivoINPGI(List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> ListaCalcoloRetributivoDaPrelievoINPGI, long idPensione)
        {
            if (ListaCalcoloRetributivoDaPrelievoINPGI != null && ListaCalcoloRetributivoDaPrelievoINPGI.Count > 0)
            {
                foreach (GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI calcRetr in ListaCalcoloRetributivoDaPrelievoINPGI)
                {
                    calcRetr.IdPensione = idPensione;
                }
                GestioneQuotaFondoINPGI.SalvaListaCalcoloRetributivoINPGI(ListaCalcoloRetributivoDaPrelievoINPGI);
            }
        }

        private static void ArchiviaDatiCalcoloContributivoINPGI(List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> ListaCalcoloContributivoDaPrelievoINPGI, long idPensione)
        {
            if (ListaCalcoloContributivoDaPrelievoINPGI != null && ListaCalcoloContributivoDaPrelievoINPGI.Count > 0)
            {
                foreach (GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI calcContr in ListaCalcoloContributivoDaPrelievoINPGI)
                {
                    calcContr.IdPensione = idPensione;
                }
                GestioneQuotaFondoINPGI.SalvaListaCalcoloContributivoINPGI(ListaCalcoloContributivoDaPrelievoINPGI);
            }
        }

        //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
        private static void ArchiviaDatiQuotaFondoIntegrativo(List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaQuotaFondoIntegrativoDaPrelievo, long idPensione)
        {
            if (ListaQuotaFondoIntegrativoDaPrelievo != null && ListaQuotaFondoIntegrativoDaPrelievo.Count > 0)
            {
                foreach (GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo quotaFondoIntegrativo in ListaQuotaFondoIntegrativoDaPrelievo)
                {
                    quotaFondoIntegrativo.IdPensione = idPensione;
                }
                GestioneQuotaFondoIntegrativo.SalvaListaQuotaFondoIntegrativo(idPensione, ListaQuotaFondoIntegrativoDaPrelievo);
            }
        }

        private static void ArchiviaDatiSupplementi(List<BLCommon.Entity.DatiSupplementi> ListaSupplementiDaPrelievo, long idPensione)
        {
            if (ListaSupplementiDaPrelievo != null && ListaSupplementiDaPrelievo.Count > 0)
            {
                for (int i = 0; i < ListaSupplementiDaPrelievo.Count; i++)
                {
                    if (Utility.ConfrontaOggetti(ListaSupplementiDaPrelievo[i], new BLCommon.Entity.DatiSupplementi()))
                    {
                        ListaSupplementiDaPrelievo.RemoveAt(i);
                        i--;
                    }
                }
                if (ListaSupplementiDaPrelievo.Count > 0)
                    GestioneSupplementi.SalvaDatiSupplementi(idPensione, ListaSupplementiDaPrelievo);
            }
        }

        private static void ArchiviaDatiSupplementiBase(BLCommon.Entity.SupplementiBase datiSupplementiBaseDaPrelievo, long idPensione)
        {
            if (datiSupplementiBaseDaPrelievo != null && !Utility.ConfrontaOggetti(datiSupplementiBaseDaPrelievo, new BLCommon.Entity.SupplementiBase()))
                GestioneSupplementi.SalvaDatiSupplementiBase(idPensione, datiSupplementiBaseDaPrelievo);
        }

        private static void ArchiviaDatiResidenzeEstere(List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstereDaPrelievo, long idPensione, long idAnagrafica)
        {
            if (ListaResidenzeEstereDaPrelievo != null && ListaResidenzeEstereDaPrelievo.Count > 0)
            {
                foreach (GestioneAnagrafica.DatiResidenzaEstero resEst in ListaResidenzeEstereDaPrelievo)
                {
                    GestioneAnagrafica.SalvaResidenzaEstero(idAnagrafica, idPensione, resEst);
                }
            }
        }

        private static void ArchiviaDatiStatiCivili(List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCiviliDaPrelievo, long idPensione, long idAnagrafica)
        {
            if (ListaStatiCiviliDaPrelievo != null && ListaStatiCiviliDaPrelievo.Count > 0)
            {
                foreach (GestioneAnagrafica.DatiStatoCivile stCiv in ListaStatiCiviliDaPrelievo)
                {
                    GestioneAnagrafica.SalvaStatoCivile(idAnagrafica, idPensione, stCiv);
                }
            }
        }

        //ENG - Linea CI RIC/TRF: se arriva dal prelievo CODSCIV = 9, come stato civile, bisogna salvare quello che arriva da Webdom.
        private static void ArchiviaDatiStatiCiviliRicTrfCI(List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCiviliDaPrelievo, long idPensione, ServiceReferences.WebDom.DatiDomanda datiDomanda, long idAnagrafica)
        {
            if (ListaStatiCiviliDaPrelievo != null && ListaStatiCiviliDaPrelievo.Count > 0)
            {
                if (ListaStatiCiviliDaPrelievo.Exists(x => x.Codice == '9'))
                {
                    if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Soggetto != null && datiDomanda.Dati.Soggetto.Rows.Count > 0)
                    {
                        foreach (ServiceReferences.WebDom.DataSetDomanda.SoggettoRow riga in datiDomanda.Dati.Soggetto.Rows)
                        {
                            foreach (GestioneAnagrafica.DatiStatoCivile stCiv in ListaStatiCiviliDaPrelievo)
                            {
                                if (stCiv.Codice == '9')
                                    stCiv.Codice = Convert.ToChar(riga.StatoCivile);
                                GestioneAnagrafica.SalvaStatoCivile(idAnagrafica, idPensione, stCiv);
                            }
                        }
                    }
                }
                else
                {
                    foreach (GestioneAnagrafica.DatiStatoCivile stCiv in ListaStatiCiviliDaPrelievo)
                    {
                        GestioneAnagrafica.SalvaStatoCivile(idAnagrafica, idPensione, stCiv);
                    }
                }
            }
        }

        private static void ArchiviaDatiDelegato(Entity.ParametriARCA parametriArca, object datiDelegato, Utility.TipoAppartenenza tipoAppartenenza, GestionePensione.DatiPensione datiPensione)
        {
            if (datiDelegato != null)
            {
                GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        //Solo INPDAP
                        ServiceReferences.LiquidazioneFs.MappingDaHostDatiDelegato delegatoFs = datiDelegato as ServiceReferences.LiquidazioneFs.MappingDaHostDatiDelegato;
                        if (delegatoFs != null && !Utility.ConfrontaOggetti(delegatoFs, new ServiceReferences.LiquidazioneFs.MappingDaHostDatiDelegato()))
                        {
                            anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                            anagrafica.CodiceFiscale = delegatoFs.CodiceFiscale;
                            anagrafica.CodiceDelegato = Utility.StringToNullableChar(delegatoFs.CodiceDelegato);
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        ServiceReferences.LiquidazioneCi.MappingDaHostDatiDelegato delegatoCi = datiDelegato as ServiceReferences.LiquidazioneCi.MappingDaHostDatiDelegato;
                        if (delegatoCi != null && !Utility.ConfrontaOggetti(delegatoCi, new ServiceReferences.LiquidazioneCi.MappingDaHostDatiDelegato()))
                        {
                            anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                            anagrafica.CodiceFiscale = delegatoCi.CodiceFiscale;
                            anagrafica.CodiceDelegato = Utility.StringToNullableChar(delegatoCi.CodiceDelegato);
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        ServiceReferences.LiquidazioneAgo.MappingDaHostDatiDelegato delegatoAgo = datiDelegato as ServiceReferences.LiquidazioneAgo.MappingDaHostDatiDelegato;
                        if (delegatoAgo != null && !Utility.ConfrontaOggetti(delegatoAgo, new ServiceReferences.LiquidazioneAgo.MappingDaHostDatiDelegato()))
                        {
                            anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                            anagrafica.CodiceFiscale = delegatoAgo.CodiceFiscale;
                            anagrafica.CodiceDelegato = Utility.StringToNullableChar(delegatoAgo.CodiceDelegato);
                        }
                        break;
                }

                if (anagrafica != null && !Utility.ConfrontaOggetti(anagrafica, new GestioneAnagrafica.DatiAnagrafici()))
                {
                    GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                    richiestaArca.Applicazione = parametriArca.Applicazione;
                    richiestaArca.Matricola = parametriArca.Matricola;
                    richiestaArca.Provenienza = parametriArca.Provenienza;
                    richiestaArca.Ruolo = parametriArca.Ruolo;

                    richiestaArca.CodiceFiscaleRichiedente = anagrafica.CodiceFiscale;
                    richiestaArca.CodiceFiscale = anagrafica.CodiceFiscale;

                    Entity.Anagrafica anagraficaDA = null;
                    string errori = string.Empty;
                    if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, datiPensione.NDomus.ToString(), out anagraficaDA, out errori) || !string.IsNullOrEmpty(errori))
                        return;

                    Utility.ValorizzaOggettiMaster(anagraficaDA, anagrafica);

                    GestioneDelegatoTutore.SalvaDelegatoDaPrelievo(datiPensione.Id, anagrafica);
                }
            }
        }

        private static void ArchiviaDatiTutore(Entity.ParametriARCA parametriArca, object datiTutore, Utility.TipoAppartenenza tipoAppartenenza, long idPensione, string numDomanda)
        {
            if (datiTutore != null)
            {
                GestioneAnagrafica.DatiAnagrafici anagrafica = null;
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        //Solo INPDAP
                        ServiceReferences.LiquidazioneFs.MappingDaHostDatiTutore tutoreFs = datiTutore as ServiceReferences.LiquidazioneFs.MappingDaHostDatiTutore;
                        if (tutoreFs != null && !Utility.ConfrontaOggetti(tutoreFs, new ServiceReferences.LiquidazioneFs.MappingDaHostDatiTutore()))
                        {
                            anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                            anagrafica.CodiceFiscale = tutoreFs.CodiceFiscale;
                            anagrafica.CodiceTutore = Utility.StringToNullableChar(tutoreFs.CodiceTutore);
                            anagrafica.CessValAmmSost = tutoreFs.CessValAmmSost;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        ServiceReferences.LiquidazioneCi.MappingDaHostDatiTutore tutoreCi = datiTutore as ServiceReferences.LiquidazioneCi.MappingDaHostDatiTutore;
                        if (tutoreCi != null && !Utility.ConfrontaOggetti(tutoreCi, new ServiceReferences.LiquidazioneCi.MappingDaHostDatiTutore()))
                        {
                            anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                            anagrafica.CodiceFiscale = tutoreCi.CodiceFiscale;
                            anagrafica.CodiceTutore = Utility.StringToNullableChar(tutoreCi.CodiceTutore);
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        ServiceReferences.LiquidazioneAgo.MappingDaHostDatiTutore tutoreAgo = datiTutore as ServiceReferences.LiquidazioneAgo.MappingDaHostDatiTutore;
                        if (tutoreAgo != null && !Utility.ConfrontaOggetti(tutoreAgo, new ServiceReferences.LiquidazioneAgo.MappingDaHostDatiTutore()))
                        {
                            anagrafica = new GestioneAnagrafica.DatiAnagrafici();
                            anagrafica.CodiceFiscale = tutoreAgo.CodiceFiscale;
                            anagrafica.CodiceTutore = Utility.StringToNullableChar(tutoreAgo.CodiceTutore);
                            anagrafica.CessValAmmSost = tutoreAgo.CessValAmmSost;
                        }
                        break;
                }

                if (anagrafica != null && !Utility.ConfrontaOggetti(anagrafica, new GestioneAnagrafica.DatiAnagrafici()) && !string.IsNullOrEmpty(anagrafica.CodiceFiscale))
                {
                    GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                    richiestaArca.Applicazione = parametriArca.Applicazione;
                    richiestaArca.Matricola = parametriArca.Matricola;
                    richiestaArca.Provenienza = parametriArca.Provenienza;
                    richiestaArca.Ruolo = parametriArca.Ruolo;

                    richiestaArca.CodiceFiscaleRichiedente = anagrafica.CodiceFiscale;
                    richiestaArca.CodiceFiscale = anagrafica.CodiceFiscale;

                    Entity.Anagrafica anagraficaDA = null;
                    string errori = string.Empty;
                    if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, numDomanda, out anagraficaDA, out errori) || !string.IsNullOrEmpty(errori))
                        return;

                    Utility.ValorizzaOggettiMaster(anagraficaDA, anagrafica);

                    GestioneDelegatoTutore.SalvaTutoreDaPrelievo(idPensione, anagrafica);
                }
            }
        }

        private static void ArchiviaDatiVittimeTerrorismo(GestioneVittimeTerrorismo.DatiVittimeTerrorismo datiVittimeTerrorismoDaPrelievo, long idPensione)
        {
            if (datiVittimeTerrorismoDaPrelievo != null && !Utility.ConfrontaOggetti(datiVittimeTerrorismoDaPrelievo, new GestioneVittimeTerrorismo.DatiVittimeTerrorismo()))
                GestioneVittimeTerrorismo.SalvaVittimeTerrorismo(idPensione, datiVittimeTerrorismoDaPrelievo);
        }

        private static void ArchiviaDatiPensioniCIImportiValuta(List<GestioneDatiContributiviCi.PensioniCiImportiValuta> ListaImportiValuta, long idPensione)
        {
            if (ListaImportiValuta != null && ListaImportiValuta.Count > 0)
                foreach (GestioneDatiContributiviCi.PensioniCiImportiValuta impVal in ListaImportiValuta)
                {
                    impVal.IdPensione = idPensione;
                    GestioneDatiContributiviCi.SalvaImportiEsteriValuta(impVal);
                }
        }

        private static void ArchiviaDatiIntegrazioneArt11(GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11DaPrelievo, long idPensione)
        {
            //TODO: Gestire l'idRecordENPALS per l'IntegrazioneArt11

            if (datiIntegrazioneArt11DaPrelievo != null && !Utility.ConfrontaOggetti(datiIntegrazioneArt11DaPrelievo, new GestioneIntegrazioneArt11.IntegrazioneArt11()))
                GestioneIntegrazioneArt11.SalvaIntegrazioneArt11(idPensione, datiIntegrazioneArt11DaPrelievo);
        }

        private static void ArchiviaDatiCalcoloContributivoEstero(List<GestioneCalcolo.DatiCalcoloContributivoEstero> ListaCalcoloContributivoEstero, long idPensione)
        {
            if (ListaCalcoloContributivoEstero != null && ListaCalcoloContributivoEstero.Count > 0)
            {
                foreach (GestioneCalcolo.DatiCalcoloContributivoEstero calcContrEst in ListaCalcoloContributivoEstero)
                    calcContrEst.IdPensione = idPensione;
                GestioneCalcolo.SalvaListCalcoloContributivoEsteroCI(ListaCalcoloContributivoEstero);
            }
        }

        private static void ArchiviaDatiPensioniCiMaternitaAcna(List<GestioneDatiContributiviCi.PensioniCiMaternitaAcna> ListaPensioniCiMaternitaAcna, long idPensione)
        {
            if (ListaPensioniCiMaternitaAcna != null && ListaPensioniCiMaternitaAcna.Count > 0)
            {
                foreach (GestioneDatiContributiviCi.PensioniCiMaternitaAcna matAcna in ListaPensioniCiMaternitaAcna)
                {
                    matAcna.IdPensione = idPensione;
                    GestioneDatiContributiviCi.SalvaMaternitaAcna(matAcna);
                }
            }
        }

        private static void ArchiviaDatiStatiEsteri(List<ServiceReferences.LiquidazioneCi.GestioneContribStatoEstero> ListaStatiEsteriCi, long idPensione)
        {
            if (ListaStatiEsteriCi != null && ListaStatiEsteriCi.Count > 0)
            {
                List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> ListaPrestazioniEE = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
                foreach (ServiceReferences.LiquidazioneCi.GestioneContribStatoEstero statoEstero in ListaStatiEsteriCi)
                {
                    if (statoEstero != null && statoEstero.PrestazioneEstera != null)
                    {
                        statoEstero.PrestazioneEstera.IdPensione = idPensione;
                        GestioneDatiContributiviCi.SalvaPrestazioneEstera(statoEstero.PrestazioneEstera);

                        if (statoEstero.ElencoImportiEsteri != null && statoEstero.ElencoImportiEsteri.Count() > 0)
                        {
                            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importo in statoEstero.ElencoImportiEsteri)
                            {
                                importo.IDPrestazioneEE = statoEstero.PrestazioneEstera.Id;
                                GestioneDatiContributiviCi.SalvaImportoEstero(importo);
                            }
                        }
                    }
                }
                //GestioneDatiContributiviCi.SalvaPrestazioniEEOld(idPensione, ListaPrestazioniEE);

                //List<GestioneDatiContributiviCi.PensioniCiImportiEsteri> ListaImportiEsteri = new List<GestioneDatiContributiviCi.PensioniCiImportiEsteri>();

                //foreach (var statoEstero in ListaStatiEsteriCi.Select((value, index) => new { value, index }))
                //{
                //    if (statoEstero.value != null && statoEstero.value.ElencoImportiEsteri != null && statoEstero.value.ElencoImportiEsteri.Count() > 0)
                //    {
                //        foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importo in statoEstero.value.ElencoImportiEsteri)
                //        {
                //            if (importo != null)
                //            {
                //                importo.IDPrestazioneEE = ListaPrestazioniEE[statoEstero.index].Id;
                //                ListaImportiEsteri.Add(importo);
                //            }
                //        }
                //    }
                //}
                //if (ListaImportiEsteri != null && ListaImportiEsteri.Count > 0)
                //    GestioneDatiContributiviCi.SalvaImportiEsteriOld(idPensione, ListaImportiEsteri);
            }
        }

        private static void ArchiviaDatiStatiEsteriPerTrfAutomatiche(List<ServiceReferences.LiquidazioneCi.GestioneContribStatoEstero> ListaStatiEsteriCi, long idPensione, List<ServiceReferences.AggPec.CI_ISTITUZIONI> istituzioniEsterePECO)
        {
            if (ListaStatiEsteriCi != null && ListaStatiEsteriCi.Count > 0)
            {
                List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> ListaPrestazioniEE = new List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE>();
                foreach (ServiceReferences.LiquidazioneCi.GestioneContribStatoEstero statoEstero in ListaStatiEsteriCi)
                {
                    if (statoEstero != null && statoEstero.PrestazioneEstera != null)
                    {
                        statoEstero.PrestazioneEstera.IdPensione = idPensione;

                        if (istituzioniEsterePECO != null)
                        {
                            double ContributiEEDecorrenzaOriginariaPECO = istituzioniEsterePECO.Find(x => x.CI_Stato == statoEstero.PrestazioneEstera.CodiceStatoEE && x.CI_Istit == statoEstero.PrestazioneEstera.CodiceIstituzione).CI_Misest;
                            if (!Utility.IsDoubleEquals(ContributiEEDecorrenzaOriginariaPECO, 0.0))
                                statoEstero.PrestazioneEstera.ContributiEEDecorrenzaOriginaria = (int)ContributiEEDecorrenzaOriginariaPECO;

                            double ContributiEEDirittoPECO = istituzioniEsterePECO.Find(x => x.CI_Stato == statoEstero.PrestazioneEstera.CodiceStatoEE && x.CI_Istit == statoEstero.PrestazioneEstera.CodiceIstituzione).CI_Direst;
                            if (!Utility.IsDoubleEquals(ContributiEEDirittoPECO, 0.0))
                                statoEstero.PrestazioneEstera.ContributiEEDiritto = (int)ContributiEEDirittoPECO;
                        }
                        GestioneDatiContributiviCi.SalvaPrestazioneEstera(statoEstero.PrestazioneEstera);

                        if (statoEstero.ElencoImportiEsteri != null && statoEstero.ElencoImportiEsteri.Count() > 0)
                        {
                            foreach (GestioneDatiContributiviCi.PensioniCiImportiEsteri importo in statoEstero.ElencoImportiEsteri)
                            {
                                importo.IDPrestazioneEE = statoEstero.PrestazioneEstera.Id;
                                GestioneDatiContributiviCi.SalvaImportoEstero(importo);
                            }
                        }
                    }
                }
            }
        }

        private static void ArchiviaDatiDL407(GestioneDL407.DatiDL407 datiDL407, long idPensione)
        {
            if (datiDL407 != null && !Utility.ConfrontaOggetti(datiDL407, new GestioneDL407.DatiDL407()))
                GestioneDL407.SalvaDL407(idPensione, datiDL407);
        }

        private static void ArchiviaDatiOneriTerrorismo(object ListaOneriTerrorismo, Utility.TipoAppartenenza tipoAppartenenza, long idPensione)
        {
            if (ListaOneriTerrorismo != null)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        List<ServiceReferences.LiquidazioneFs.DatiBenefici.OneriTerrorismo> ListaOneriTerrorismoFs = ListaOneriTerrorismo as List<ServiceReferences.LiquidazioneFs.DatiBenefici.OneriTerrorismo>;
                        if (ListaOneriTerrorismoFs != null && ListaOneriTerrorismoFs.Count > 0)
                        {
                            foreach (ServiceReferences.LiquidazioneFs.DatiBenefici.OneriTerrorismo onTerr in ListaOneriTerrorismoFs)
                            {
                                if (onTerr != null && onTerr.Importo.HasValue)
                                {
                                    onTerr.IdPensione = idPensione;
                                    GestioneRipartizioneFondi.DatiRipartizioneFondi rF = new GestioneRipartizioneFondi.DatiRipartizioneFondi();
                                    Utility.ValorizzaOggetti(onTerr, rF);
                                    GestioneRipartizioneFondi.SalvaRipartizioneFondi(rF);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private static void ArchiviaDatiOneri(List<GestioneOneri.DatiOneri> ListaOneri, long idPensione)
        {
            if (ListaOneri != null && ListaOneri.Count > 0)
            {
                foreach (GestioneOneri.DatiOneri on in ListaOneri)
                {
                    if (on != null && on.IdCodeGruppo.HasValue && on.IdCodeGruppo.Value != 0)
                    {
                        on.IdPensione = idPensione;
                        GestioneOneri.DatiOneri oneri = new GestioneOneri.DatiOneri();
                        Utility.ValorizzaOggetti(on, oneri);
                        GestioneOneri.SalvaOneriOnere(oneri);
                    }
                }
            }
        }

        private static void ArchiviaDatiBeneficiParticolari(List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaBeneficiParticolari, long idPensione)
        {
            if (ListaBeneficiParticolari != null && ListaBeneficiParticolari.Count > 0)
            {
                foreach (GestioneBeneficiParticolari.DatiBeneficiParticolari bP in ListaBeneficiParticolari)
                {
                    if (bP != null && !string.IsNullOrEmpty(bP.CodiceBenefici))
                    {
                        bP.IdPensione = idPensione;
                        GestioneBeneficiParticolari.DatiBeneficiParticolari benPart = new GestioneBeneficiParticolari.DatiBeneficiParticolari();
                        Utility.ValorizzaOggetti(bP, benPart);
                        GestioneBeneficiParticolari.SalvaDatiBeneficiParticolari(benPart);
                    }
                }
            }
        }

        private static void ArchiviaDatiServizioUtile(List<GestioneDatiServizioUtile.ServizioUtile> ListaDatiServizioUtile, long idFondo, Dictionary<long, long> dictionaryIdRecordFondo,
            Utility.TipoAppartenenza tipoAppartenenza, long idPensione, Utility.TipoFondo? tipoFondo, bool isDomandaConNuovaGestioneDatiFondoFSPT)
        {
            switch (tipoAppartenenza)
            {
                case Utility.TipoAppartenenza.FS:
                    if (ListaDatiServizioUtile != null && ListaDatiServizioUtile.Count > 0)
                    {
                        if (idFondo == 0)
                        {   //Non esiste record nella tab PensioneFondoDatiGenerici
                            GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
                            idFondo = datiFondo.Id;
                        }
                        foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in ListaDatiServizioUtile)
                        {
                            if (servizioUtile != null && !string.IsNullOrEmpty(servizioUtile.Quota))
                            {
                                if (isDomandaConNuovaGestioneDatiFondoFSPT || tipoFondo == Utility.TipoFondo.DZ)
                                {
                                    if (dictionaryIdRecordFondo != null)
                                    {
                                        long idRecordFondo = dictionaryIdRecordFondo[servizioUtile.IdRecordFondo.GetValueOrDefault()];
                                        servizioUtile.IdRecordFondo = idRecordFondo;

                                        GestioneDatiServizioUtile.ServizioUtile sU = new GestioneDatiServizioUtile.ServizioUtile();
                                        Utility.ValorizzaOggetti(servizioUtile, sU);
                                        GestioneDatiServizioUtile.SalvaDatiServizioUtileRecordFondo(idFondo, idRecordFondo, sU);
                                    }
                                }
                                else
                                {
                                    if (servizioUtile.IdRecordFondo == 1)
                                    {
                                        GestioneDatiServizioUtile.ServizioUtile sU = new GestioneDatiServizioUtile.ServizioUtile();
                                        Utility.ValorizzaOggetti(servizioUtile, sU);
                                        GestioneDatiServizioUtile.SalvaDatiServizioUtile(idFondo, sU);
                                    }
                                }
                            }

                            if (tipoFondo == Utility.TipoFondo.GAS || tipoFondo == Utility.TipoFondo.PI || tipoFondo == Utility.TipoFondo.CL || tipoFondo == Utility.TipoFondo.ES || tipoFondo == Utility.TipoFondo.PL ||  tipoFondo == Utility.TipoFondo.PM)
                            {
                                if (servizioUtile != null)
                                {
                                    GestioneDatiServizioUtile.ServizioUtile sU = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(servizioUtile, sU);
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtile(idFondo, sU);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private static void ArchiviaDatiServizioUtile707(List<GestioneCalcolo.ServizioUtile707> ListaDatiServizioUtile707, long idFondo, Dictionary<long, long> dictionaryIdRecordFondo,
            Utility.TipoAppartenenza tipoAppartenenza, long idPensione, bool isDomandaConNuovaGestioneDatiFondoFSPT)
        {
            switch (tipoAppartenenza)
            {
                case Utility.TipoAppartenenza.FS:
                    if (ListaDatiServizioUtile707 != null && ListaDatiServizioUtile707.Count > 0)
                    {
                        if (idFondo == 0)
                        {   //Non esiste record nella tab PensioneFondoDatiGenerici
                            GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
                            idFondo = datiFondo.Id;
                        }
                        foreach (GestioneCalcolo.ServizioUtile707 servizioUtile707 in ListaDatiServizioUtile707)
                        {
                            if (servizioUtile707 != null && !string.IsNullOrEmpty(servizioUtile707.Quota))
                            {
                                if (isDomandaConNuovaGestioneDatiFondoFSPT)
                                {
                                    if (dictionaryIdRecordFondo != null)
                                    {
                                        long idRecordFondo = dictionaryIdRecordFondo[servizioUtile707.IdRecordFondo.GetValueOrDefault()];
                                        servizioUtile707.IdRecordFondo = idRecordFondo;

                                        GestioneCalcolo.ServizioUtile707 sU = new GestioneCalcolo.ServizioUtile707();
                                        Utility.ValorizzaOggetti(servizioUtile707, sU);
                                        GestioneCalcolo.SalvaDatiServizioUtile707RecordFondo(idFondo, idRecordFondo, sU);
                                    }
                                }
                                else
                                {
                                    if (servizioUtile707.IdRecordFondo == 1)
                                    {
                                        GestioneCalcolo.ServizioUtile707 sU = new GestioneCalcolo.ServizioUtile707();
                                        Utility.ValorizzaOggetti(servizioUtile707, sU);
                                        GestioneCalcolo.SalvaDatiServizioUtile707(idFondo, sU);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private static void ArchiviaDatiServizioUtileINPDAP(List<GestioneDatiServizioUtileINPDAP.ServizioUtile> ListaDatiServizioUtile, Dictionary<long, long> dictionaryIdRecordFondo, long idPensione)
        {
            if (ListaDatiServizioUtile != null && ListaDatiServizioUtile.Count > 0)
            {
                foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile servizioUtile in ListaDatiServizioUtile)
                {
                    if (servizioUtile != null && !string.IsNullOrEmpty(servizioUtile.Quota))
                    {
                        if (dictionaryIdRecordFondo != null)
                        {
                            long idRecordFondo = dictionaryIdRecordFondo[servizioUtile.IdRecordFondo.GetValueOrDefault()];
                            servizioUtile.IdRecordFondo = idRecordFondo;

                            GestioneDatiServizioUtileINPDAP.ServizioUtile sU = new GestioneDatiServizioUtileINPDAP.ServizioUtile();
                            Utility.ValorizzaOggetti(servizioUtile, sU);
                            GestioneDatiServizioUtileINPDAP.SalvaDatiServizioUtileRecordFondo(idPensione, idRecordFondo, sU);
                        }
                    }
                }
            }
        }

        private static void ArchiviaDatiServizioUtileINPDAP707(List<GestioneCalcolo.ServizioUtileINPDAP707> ListaDatiServizioUtileINPDAP707, long idFondo, Dictionary<long, long> dictionaryIdRecordFondo,
            Utility.TipoAppartenenza tipoAppartenenza, long idPensione)
        {
            switch (tipoAppartenenza)
            {
                case Utility.TipoAppartenenza.FS:
                    if (ListaDatiServizioUtileINPDAP707 != null && ListaDatiServizioUtileINPDAP707.Count > 0)
                    {
                        if (idFondo == 0)
                        {   //Non esiste record nella tab PensioneFondoDatiGenerici
                            GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo();
                            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
                            idFondo = datiFondo.Id;
                        }
                        foreach (GestioneCalcolo.ServizioUtileINPDAP707 servizioUtileINPDAP707 in ListaDatiServizioUtileINPDAP707)
                        {
                            if (servizioUtileINPDAP707 != null && !string.IsNullOrEmpty(servizioUtileINPDAP707.Quota))
                            {
                                if (dictionaryIdRecordFondo != null)
                                {
                                    long idRecordFondo = dictionaryIdRecordFondo[servizioUtileINPDAP707.IdRecordFondo.GetValueOrDefault()];
                                    servizioUtileINPDAP707.IdRecordFondo = idRecordFondo;

                                    GestioneCalcolo.ServizioUtileINPDAP707 sU = new GestioneCalcolo.ServizioUtileINPDAP707();
                                    Utility.ValorizzaOggetti(servizioUtileINPDAP707, sU);
                                    GestioneCalcolo.SalvaDatiServizioUtileINPDAP707(idPensione, idRecordFondo, sU);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private static void ArchiviaDatiPensioniDatiGenerici(GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici, long idPensione)
        {
            if (datiPensioniDatiGenerici != null && !Utility.ConfrontaOggetti(datiPensioniDatiGenerici, new GestioneDatiGenericiAgoCi.PensioniDatiGenerici()))
                GestioneDatiGenericiAgoCi.SalvaDatiGenerici(idPensione, datiPensioniDatiGenerici);
        }

        private static void ArchiviaDatiBititolarita(List<GestioneAltrePensioni.AltraPensione> ListaBititolaritaDaPrelievo, long idPensione)
        {
            if (ListaBititolaritaDaPrelievo != null && ListaBititolaritaDaPrelievo.Count > 0)
            {
                for (int i = 0; i < ListaBititolaritaDaPrelievo.Count; i++)
                {
                    if (!Utility.ConfrontaOggetti(ListaBititolaritaDaPrelievo[i], new GestioneAltrePensioni.AltraPensione()))
                    {
                        ListaBititolaritaDaPrelievo[i].IdPensione = idPensione;
                        GestioneAltrePensioni.SalvaAltraPensione(ListaBititolaritaDaPrelievo[i]);
                    }
                }
            }
        }

        private static void ArchiviaDatiInail(List<GestionePensioneInailInabilita.DatiPensioniINAIL> ListaInailDaPrelievo, long idPensione)
        {
            if (ListaInailDaPrelievo != null && ListaInailDaPrelievo.Count > 0)
            {
                for (int i = 0; i < ListaInailDaPrelievo.Count; i++)
                {
                    if (!Utility.ConfrontaOggetti(ListaInailDaPrelievo[i], new GestionePensioneInailInabilita.DatiPensioniINAIL()))
                    {
                        ListaInailDaPrelievo[i].IdPensione = idPensione;
                        GestionePensioneInailInabilita.SalvaPensioniINAIL(ListaInailDaPrelievo[i]);
                    }
                }
            }
        }

        private static void ArchiviaDatiInabilita(GestionePensioneInailInabilita.DatiInabilita datiInabilita, long idPensione)
        {
            if (datiInabilita != null && !Utility.ConfrontaOggetti(datiInabilita, new GestionePensioneInailInabilita.DatiInabilita()))
            {
                datiInabilita.IdPensione = idPensione;
                GestionePensioneInailInabilita.SalvaInabilita(datiInabilita);
            }
        }

        private static void ArchiviaDatiRedditiSentenza495_93(List<BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditiSentenza495_93, long idPensione, GestionePensione.DatiPensione datiPensione)
        {
            bool IsDomandaRiliquidazioneIndiretta = datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0422" && datiPensione.Tipo == "0026";
            if (listaDatiRedditiSentenza495_93 != null && listaDatiRedditiSentenza495_93.Count > 0 && !IsDomandaRiliquidazioneIndiretta)
            {
                foreach (BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93 redditiSentenza495_93 in listaDatiRedditiSentenza495_93)
                {
                    if (!Utility.ConfrontaOggetti(redditiSentenza495_93, new BLCommon.GestioneDanteCausa.DatiRedditoSentenza495_93()))
                    {
                        redditiSentenza495_93.IdPensione = idPensione;
                        BLCommon.GestioneDanteCausa.SalvaRedditiSentenza495_93(redditiSentenza495_93, datiPensione);
                    }
                }
            }
        }

        private static void ArchiviaDatiPrepensionamento(List<BLCommon.GestionePrepensionamento.DatiPrepensionamento> listaDatiPrepensionamento, long idPensione)
        {
            if (listaDatiPrepensionamento != null && listaDatiPrepensionamento.Count > 0)
            {
                foreach (BLCommon.GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento in listaDatiPrepensionamento)
                {
                    if (!Utility.ConfrontaOggetti(datiPrepensionamento, new BLCommon.GestionePrepensionamento.DatiPrepensionamento()))
                    {
                        datiPrepensionamento.IdPensione = idPensione;
                        BLCommon.GestionePrepensionamento.SalvaDatiPrepensionamento(datiPrepensionamento);
                    }
                }
            }
        }

        private static void ArchiviaDatiContribuzioneENPALS(BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneENPALS, GestionePensione.DatiPensione datiPensione)
        {
            if (datiContribuzioneENPALS != null && !Utility.ConfrontaOggetti(datiContribuzioneENPALS, new BLCommon.Entity.DatiContribuzioneEnpals()))
                GestioneContribuzioneEnpals.SalvaDatiContributizioneEnpals(datiPensione, datiContribuzioneENPALS);
        }

        private static void ArchiviaDatiNoCalcolo(List<ServiceReferences.LiquidazioneFs.DatiNoCalcolo> listaDatiNoCalcolo, GestionePensione.DatiPensione datiPensione)
        {
            if (listaDatiNoCalcolo != null && listaDatiNoCalcolo.Count > 0)
            {
                foreach (ServiceReferences.LiquidazioneFs.DatiNoCalcolo datiNoCalcolo in listaDatiNoCalcolo)
                {
                    GestioneDatiNoCalcolo.RecordDatiNoCalcolo datiNoCalcoloBl = new GestioneDatiNoCalcolo.RecordDatiNoCalcolo();
                    long? idRecordNoCalcolo = 0;
                    Utility.ValorizzaOggetti(datiNoCalcolo, datiNoCalcoloBl);
                    datiNoCalcoloBl.IdPensione = datiPensione.Id;
                    GestioneDatiNoCalcolo.SalvaRecordNoCalcolo(datiPensione.Id, datiNoCalcoloBl, out idRecordNoCalcolo);
                    datiNoCalcoloBl.Id = idRecordNoCalcolo.GetValueOrDefault();

                    #region Componenti Familiari
                    if (datiNoCalcolo.ListaComponentiFamiliari != null && datiNoCalcolo.ListaComponentiFamiliari.Count() > 0)
                        foreach (ServiceReferences.LiquidazioneFs.DatiNoCalcolo.ComponentiFamiliari componente in datiNoCalcolo.ListaComponentiFamiliari)
                        {
                            GestioneComponenteFamiliare.ComponenteFamiliare componenteDB = new GestioneComponenteFamiliare.ComponenteFamiliare();
                            componenteDB.IdPensione = datiPensione.Id;
                            componenteDB.IdRecordDatiNoCalcolo = datiNoCalcoloBl.Id;
                            componenteDB.CodiceFiscale = componente.CodiceFiscale;
                            GestioneComponenteFamiliare.SalvaComponenteFamiliare(componenteDB);
                        }
                    #endregion Componenti Familiari

                    #region semafori
                    GestioneQuadri.DatiQuadroRecordNoCalcolo datiQuadroRecordNoCalcolo = new GestioneQuadri.DatiQuadroRecordNoCalcolo();
                    datiQuadroRecordNoCalcolo.TabNoCalcolo = 0;

                    GestioneQuadri.SalvaQuadroDatiRecordNoCalcolo(datiPensione.Id, idRecordNoCalcolo.GetValueOrDefault(), datiQuadroRecordNoCalcolo);
                    #endregion semafori
                }
            }
        }

        private static void ArchiviaDatiStoricoGP(GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP, long idPensione)
        {
            if (datiStoricoGP != null && !Utility.PropertiesAreAllNull(datiStoricoGP))
                GestioneDatiStoricoGP.SalvaDatiStoricoGP(idPensione, datiStoricoGP);
        }

        private static void ArchiviaDatiBeneficioVittimeTerrorismo(GestionePensione.DatiPensione datiPensione, GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo, long idPensione)
        {
            if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
            {
                datiBeneficioVittimeTerrorismo = new GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo();
                datiBeneficioVittimeTerrorismo.TipologiaPrestazione = 4;
            }

            if (datiBeneficioVittimeTerrorismo != null && !Utility.PropertiesAreAllNull(datiBeneficioVittimeTerrorismo))
                GestioneBeneficioVittimeTerrorismo.SalvaBeneficioVittimeTerrorismo(idPensione, datiBeneficioVittimeTerrorismo);
        }

        private static void ArchiviaDatiCalcoloVittimeTerrorismo(List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaCalcoloVittimeTerrorismoDaPrelievo, long idPensione)
        {
            if (listaCalcoloVittimeTerrorismoDaPrelievo != null && listaCalcoloVittimeTerrorismoDaPrelievo.Count > 0)
            {
                foreach (var calcVittime in listaCalcoloVittimeTerrorismoDaPrelievo.FindAll(x => !x.Equals(new GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo())))
                    GestioneCalcoloVittimeTerrorismo.SalvaCalcoloVittimeTerrorismo(idPensione, calcVittime);
            }
        }

        private static void ArchiviaDatiSentenzaArt4(List<GestioneSentenzaArt4.DatiSentenzaArt4> listaSentenzaArt4, long idPensione)
        {
            if (listaSentenzaArt4 != null && listaSentenzaArt4.Count > 0)
            {
                foreach (GestioneSentenzaArt4.DatiSentenzaArt4 sentArt4 in listaSentenzaArt4)
                    GestioneSentenzaArt4.SalvaSentenzaArt4(idPensione, sentArt4);
            }
        }

        private static void ArchiviaDatiSentenze(List<GestioneSentenze.DatiSentenze> listaSentenze, long idPensione)
        {
            if (listaSentenze != null && listaSentenze.Count > 0)
            {
                foreach (GestioneSentenze.DatiSentenze sent in listaSentenze)
                    GestioneSentenze.SalvaSentenze(idPensione, sent);
            }
        }

        internal static void ArchiviaDatiPensioniEstereDc(BLCommon.GestioneDanteCausa.PensioniEstereDcBL datiPensioniEstereDc, long idDanteCausa)
        {
            if (datiPensioniEstereDc != null && !Utility.ConfrontaOggetti(datiPensioniEstereDc, new BLCommon.GestioneDanteCausa.PensioniEstereDcBL()))
                datiPensioniEstereDc.IdDanteCausa = idDanteCausa;
            BLCommon.GestioneDanteCausa.SalvaPensioniEstereDC(datiPensioniEstereDc);
        }

        internal static void ArchiviaDatiPensioniEstereDcImportoTotaleSupplementi(BLCommon.GestioneDanteCausa.PensioniEstereDcBL importoTotSupplementi, long idDanteCausa)
        {
            importoTotSupplementi.CodiciVari = 10;
            if (importoTotSupplementi != null && !Utility.ConfrontaOggetti(importoTotSupplementi, new BLCommon.GestioneDanteCausa.PensioniEstereDcBL()))
                importoTotSupplementi.IdDanteCausa = idDanteCausa;
            BLCommon.GestioneDanteCausa.SalvaPensioniEstereDC(importoTotSupplementi);
        }

        internal static void ArchiviaDatiPensioniEstereDcImportoArt6(BLCommon.GestioneDanteCausa.PensioniEstereDcBL importoArt6, long idDanteCausa)
        {
            importoArt6.CodiciVari = 6;
            if (importoArt6 != null && !Utility.ConfrontaOggetti(importoArt6, new BLCommon.GestioneDanteCausa.PensioniEstereDcBL()))
                importoArt6.IdDanteCausa = idDanteCausa;
            BLCommon.GestioneDanteCausa.SalvaPensioniEstereDC(importoArt6);
        }

        //ENG - MEMO 74_2023
        private static void ArchiviaDatiEsteriCumulo(List<ServiceReferences.LiquidazioneAgo.GestioneContribStatoEsteroCumulo> listaStatiEsteriCumulo, long idPensione)
        {
            if (listaStatiEsteriCumulo != null && listaStatiEsteriCumulo.Count > 0)
            {
                List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> ListaPrestazioniEE = new List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo>();
                foreach (ServiceReferences.LiquidazioneAgo.GestioneContribStatoEsteroCumulo statoEstero in listaStatiEsteriCumulo)
                {
                    if (statoEstero != null && statoEstero.PrestazioneEsteraCumulo != null)
                    {
                        statoEstero.PrestazioneEsteraCumulo.IdPensione = idPensione;
                        GestioneDatiEsteriCumulo.SalvaPrestazioneEsteraCumulo(statoEstero.PrestazioneEsteraCumulo);

                        if (statoEstero.ElencoImportiEsteriCumulo != null && statoEstero.ElencoImportiEsteriCumulo.Count() > 0)
                        {
                            foreach (GestioneDatiEsteriCumulo.PensioneImportiEsteriCumulo importo in statoEstero.ElencoImportiEsteriCumulo)
                            {
                                importo.IdPensioneEsteraCumulo = statoEstero.PrestazioneEsteraCumulo.Id;
                                GestioneDatiEsteriCumulo.SalvaImportoEsteroCumulo(importo);
                            }
                        }
                    }
                }
            }
        }
        #endregion ArchiviazioneSenzaDatiMaster

        #endregion private members
    }
}
