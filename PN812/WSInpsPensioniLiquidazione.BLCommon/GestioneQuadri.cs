using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneQuadri
    {
        #region Quadri

        public static void InizializzaQuadriByIdPensione(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoDomanda tipoDomanda,
            Utility.TipoAppartenenza? tipoAppartenenza, GestioneCtrlRic.ControlTabRic controlTabRic, bool isResidenteEstero, bool isRicRev, bool isDiretta,
            bool? isTabOneriVisible, bool isResEsteroPerRic, bool isSupplementiPerRic, bool isBancRicTrf,
            bool? isTabBeneficiVisible, bool? isTabExCombattenteVisible, bool? isTabPrivilegiateVisible, DateTime? decorrenzaPensione, bool? isTabIstruttoriaRequired,
            bool? isTabInailVisibleForAGO, bool? isTabPrecedentePensioneAGO, bool isDatiPostDecOriginariaVisibleCI, bool isFamiliariVerde, bool isSupplementiPerENPALS, bool? isTabMaggiorazioniVisible,
            bool isTabSentenza495_93AGORequired, DateTime? dataMorte, bool isRiapertura, bool? isTabPrepensionamentoVisible, bool isContribuzioneENPALSPerLiq, bool isContribuzioneENPALSPerSupp,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo, List<GestioneFondo.DatiFondoPT> datiFondoPensioneDatiPT, bool? isTabArticolo2Required,
            bool? isTabPrivilegiataRequired, bool isEliminazioneRequired, bool isDatiNoCalcoloVisible, bool? isTabDl407Visibile, bool isStoricoVisible, bool isSpacchettamentoPerRicostituzione,
            bool isBeneficioVittimeTerrorismo, bool? isTabInailForAGORequired, bool? isTabBititolaritaVisible, bool isDatiCalcoloPerRicRequired, bool? isTabSentenzeVisible, bool isTabRedditiOpzionale,
            bool isSupplementiPerRev, bool isSupplementiTotalIVS, bool? isDatiFondoOpzionale, bool? isDecorrenzaMaggiorazioneFromPrelievo, bool isConvenzione13, bool? isQuotaFondoIntegrativoVisible,
            bool datiFondoPensioneDatiPTdaPrelievo, bool? isTabArticolo2NotVisible, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, bool isOneriRicPrepensionamentoTipo0162NotVisible, bool tabEliminazioneGialloAutomazione, bool noDanteCausaAnte96,
            bool isTabInailVisibleForCI, bool isQuoteMiglioramentiContrattualiVisible, bool isDetrazioniObbligatorio)
        {
            InizializzaQuadroTitolare(datiPensione.Id, isResidenteEstero, tipoAppartenenza, tipoDomanda, controlTabRic, isResEsteroPerRic, isRiapertura);

            InizializzaQuadroDetrazioni(datiPensione, tipoAppartenenza, tipoDomanda, isRiapertura, isDetrazioniObbligatorio);

            InizializzaQuadroPagamento(datiPensione, tipoAppartenenza, tipoDomanda, isRiapertura);

            InizializzaQuadroLiquidazionePensione(datiPensione, tipoDomanda, tipoAppartenenza, datiPensione.SiglaCategoria, datiPensione.Gestione, controlTabRic, decorrenzaPensione, isTabIstruttoriaRequired,
                isTabInailVisibleForAGO, isTabPrecedentePensioneAGO, isRiapertura, isContribuzioneENPALSPerLiq, isDomandaConNuovaGestioneDatiFondoFSPT, isStoricoVisible, isTabInailForAGORequired, isTabSentenzeVisible, datiDanteCausa, isTabInailVisibleForCI);

            InizializzaQuadroDelegatoTutore(tipoAppartenenza, datiPensione, tipoDomanda, isRiapertura);

            InizializzaQuadroDatiContributivi(datiPensione, datiDanteCausa, tipoAppartenenza, tipoDomanda, controlTabRic, isDatiPostDecOriginariaVisibleCI, decorrenzaPensione, isRiapertura,
                isDomandaConNuovaGestioneDatiFondoFSPT, isStoricoVisible, isBeneficioVittimeTerrorismo, isDatiCalcoloPerRicRequired, isConvenzione13, isQuotaFondoIntegrativoVisible, isQuoteMiglioramentiContrattualiVisible);

            InizializzaQuadroRedditi(datiPensione, isRiapertura, tipoAppartenenza, isTabRedditiOpzionale);

            InizializzaQuadroDanteCausa(datiPensione, tipoDomanda, tipoAppartenenza, isRicRev, isDiretta, isTabSentenza495_93AGORequired, isRiapertura, noDanteCausaAnte96, datiDanteCausa);

            InizializzaQuadroFamiliari(datiPensione, tipoAppartenenza, tipoDomanda, isRiapertura, isSpacchettamentoPerRicostituzione, isFamiliariVerde, controlloDinamicoSpacchettate024, datiDanteCausa);

            InizializzaQuadroMaggiorazioniBenefici(datiPensione, tipoAppartenenza, isTabBeneficiVisible, isTabExCombattenteVisible, isTabPrivilegiateVisible,
                isTabMaggiorazioniVisible, isTabDl407Visibile, isRiapertura, isBeneficioVittimeTerrorismo, isDecorrenzaMaggiorazioneFromPrelievo, datiDanteCausa);

            InizializzaQuadroSupplementi(datiPensione, tipoAppartenenza, tipoDomanda, isSupplementiPerRic, isBancRicTrf, isSupplementiPerENPALS, isSupplementiPerRev, isSupplementiTotalIVS, controlTabRic,
                isRiapertura, isContribuzioneENPALSPerSupp, datiDanteCausa);

            InizializzaQuadroBititolarita(datiPensione, tipoAppartenenza, isRiapertura, isTabBititolaritaVisible, datiDanteCausa);

            InizializzaQuadroEliminazione(datiPensione, tipoAppartenenza, dataMorte, isRiapertura, isEliminazioneRequired, tabEliminazioneGialloAutomazione);

            InizializzaQuadroOneri(datiPensione, tipoAppartenenza, tipoDomanda, isRiapertura, isTabOneriVisible, isTabPrepensionamentoVisible, isStoricoVisible, isOneriRicPrepensionamentoTipo0162NotVisible);

            InizializzaQuadroDatiFondo(datiPensione, tipoAppartenenza, isDomandaConNuovaGestioneDatiFondoFSPT, isDatiFondoOpzionale);

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
            if (isDomandaConNuovaGestioneDatiFondoFSPT || Utility.IsDomandaINPDAP(datiPensione.Gestione) || tipoFondo == Utility.TipoFondo.DZ)
                InizializzaQuadroDatiRecordFondo(datiPensione, listaDatiRecordFondo, tipoAppartenenza, tipoDomanda, isDomandaConNuovaGestioneDatiFondoFSPT, datiFondoPensioneDatiPT, isTabPrivilegiataRequired,
                    isTabArticolo2Required, isDatiFondoOpzionale, datiFondoPensioneDatiPTdaPrelievo, isTabArticolo2NotVisible);

            InizializzaQuadroDatiNoCalcolo(datiPensione, isDatiNoCalcoloVisible);

            InizializzaQuadroPeriodi(datiPensione, tipoAppartenenza, isRiapertura, isSpacchettamentoPerRicostituzione, controlloDinamicoSpacchettate024, datiDanteCausa);

            InizializzaQuadroAventiDiritto(datiPensione, tipoAppartenenza, isRiapertura, isSpacchettamentoPerRicostituzione, controlloDinamicoSpacchettate024, datiDanteCausa);

            InizializzaQuadroAltreDomandeCollegate(datiPensione, tipoAppartenenza, isRiapertura, isSpacchettamentoPerRicostituzione, controlloDinamicoSpacchettate024, datiDanteCausa);

            InizializzaQuadroRichiestaBonus(datiPensione);
        }
        #endregion Quadri

        #region QuadroTitolare
        public static void GetQuadroTitolareByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroTitolare datiQuadroTitolare)
        {
            QuadroTitolare quadroTitolare = null;
            datiQuadroTitolare = null;
            DAGestioneQuadri.GetQuadroTitolareByIdPensione(datiPensione.Id, out quadroTitolare);
            if (quadroTitolare == null)
            {
                GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

                bool isResEsteroRic = false;
                List<GestioneAnagrafica.DatiResidenzaEstero> elencoResidenzeEstere = null;
                GestioneAnagrafica.GetResidenzeEstereByIdPensione(datiPensione.Id, out elencoResidenzeEstere);
                if (elencoResidenzeEstere != null && elencoResidenzeEstere.Count > 0)
                    isResEsteroRic = true;

                Utility.TipoAppartenenza? tipoAppartenenza = BLCommon.Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                GestioneCtrlRic.ControlTabRic controlTabRic = null;
                GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, tipoAppartenenza, out controlTabRic);

                InizializzaQuadroTitolare(datiPensione.Id, datiAnagrafici != null ? Utility.IsResidenteEstero(datiAnagrafici.CodiceComuneResidenza) : false,
                    tipoAppartenenza, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), controlTabRic, isResEsteroRic, Utility.IsRiaperturaDomanda(datiPensione.Id));

                DAGestioneQuadri.GetQuadroTitolareByIdPensione(datiPensione.Id, out quadroTitolare);
                if (quadroTitolare == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro titolare");
            }
            datiQuadroTitolare = new DatiQuadroTitolare();
            Utility.ValorizzaOggetti(quadroTitolare, datiQuadroTitolare);
        }

        public static void SalvaQuadroTitolare(long idPensione, DatiQuadroTitolare datiQuadroTitolare)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroTitolare quadroTitolare = new QuadroTitolare();
                Utility.ValorizzaOggetti(datiQuadroTitolare, quadroTitolare);
                quadroTitolare.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroTitolare(quadroTitolare);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroTitolare(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroTitolare(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroTitolare(long idPensione, bool isResidenteEstero, Utility.TipoAppartenenza? tipoAppartenenza,
            Utility.TipoDomanda tipoDomanda, GestioneCtrlRic.ControlTabRic controlTabRic, bool isResEsteroPerRic, bool isRiapertura)
        {
            DatiQuadroTitolare quadroTitolare = new DatiQuadroTitolare(idPensione, isResidenteEstero, tipoAppartenenza, tipoDomanda, controlTabRic, isResEsteroPerRic, isRiapertura);
            SalvaQuadroTitolare(idPensione, quadroTitolare);
        }

        public static void GestioneSemaforoQuadroTitolare(GestionePensione.DatiPensione datiPensione, bool isSaveStatoCivile, bool isDeleteStatoCivile, bool isSaveResidenzeEstere, bool isDeleteResidenzeEstere,
            bool isSaveAnagrafica, BLCommon.Entity.AreaTitolare areaTitolareBL, long idAnagrafica, Utility.TipoAppartenenza? tipoAppartenenza, ref DatiQuadroTitolare datiQuadroTitolare)
        {
            if (isSaveStatoCivile)
                datiQuadroTitolare.TabStatiCivili = 2;
            else if (isDeleteStatoCivile)
                datiQuadroTitolare.TabStatiCivili = 0;

            if (isSaveAnagrafica)
                datiQuadroTitolare.TabAnagrafica = 2;

            if (isSaveResidenzeEstere)
            {
                if (areaTitolareBL.ElencoResidenzeEstere != null && areaTitolareBL.ElencoResidenzeEstere.Count > 0)
                {
                    datiQuadroTitolare.TabResidenzeEstero = 2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(areaTitolareBL.Anagrafica.CodiceComuneResidenza) && areaTitolareBL.Anagrafica.CodiceComuneResidenza.StartsWith("Z") &&
                        tipoAppartenenza.HasValue && (tipoAppartenenza.Value == Utility.TipoAppartenenza.FS || tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO))
                        datiQuadroTitolare.TabResidenzeEstero = 0;
                    else
                        datiQuadroTitolare.TabResidenzeEstero = 1;
                }
            }
            else if (isDeleteResidenzeEstere)
            {
                if (Utility.IsResidenteEstero(areaTitolareBL.Anagrafica.CodiceComuneResidenza))
                    datiQuadroTitolare.TabResidenzeEstero = 0;
                else
                    datiQuadroTitolare.TabResidenzeEstero = 1;
            }

            SalvaQuadroTitolare(datiPensione.Id, datiQuadroTitolare);
        }

        #endregion QuadroTitolare

        #region QuadroDetrazioni
        public static void GetQuadroDetrazioniByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroDetrazioni datiQuadroDetrazioni)
        {
            QuadroDetrazioni quadroDetrazioni = null;
            datiQuadroDetrazioni = null;
            DAGestioneQuadri.GetQuadroDetrazioniByIdPensione(datiPensione.Id, out quadroDetrazioni);
            if (quadroDetrazioni == null)
            {
                InizializzaQuadroDetrazioni(datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione),
                    Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), Utility.IsRiaperturaDomanda(datiPensione.Id), null);
                DAGestioneQuadri.GetQuadroDetrazioniByIdPensione(datiPensione.Id, out quadroDetrazioni);
                if (quadroDetrazioni == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro detrazioni");
            }
            datiQuadroDetrazioni = new DatiQuadroDetrazioni();
            Utility.ValorizzaOggetti(quadroDetrazioni, datiQuadroDetrazioni);
        }

        public static void SalvaQuadroDetrazioni(long idPensione, DatiQuadroDetrazioni datiQuadroDetrazioni)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDetrazioni quadroDetrazioni = new QuadroDetrazioni();
                Utility.ValorizzaOggetti(datiQuadroDetrazioni, quadroDetrazioni);
                quadroDetrazioni.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroDetrazioni(quadroDetrazioni);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDetrazioni(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDetrazioni(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroDetrazioni(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isRiapertura, bool? isDetrazioniObbligatorio)
        {
            DatiQuadroDetrazioni quadroDetrazioni = new DatiQuadroDetrazioni(datiPensione, tipoAppartenenza, tipoDomanda, isRiapertura, isDetrazioniObbligatorio);
            SalvaQuadroDetrazioni(datiPensione.Id, quadroDetrazioni);
        }
        #endregion QuadroDetrazioni

        #region QuadroPagamento
        public static void GetQuadroPagamentoByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroPagamento datiQuadroPagamento)
        {
            QuadroPagamento quadroPagamento = null;
            datiQuadroPagamento = null;
            DAGestioneQuadri.GetQuadroPagamentoByIdPensione(datiPensione.Id, out quadroPagamento);
            if (quadroPagamento == null)
            {
                InizializzaQuadroPagamento(datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione),
                    Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), Utility.IsRiaperturaDomanda(datiPensione.Id));
                DAGestioneQuadri.GetQuadroPagamentoByIdPensione(datiPensione.Id, out quadroPagamento);
                if (quadroPagamento == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro pagamento");
            }
            datiQuadroPagamento = new DatiQuadroPagamento();
            Utility.ValorizzaOggetti(quadroPagamento, datiQuadroPagamento);
        }

        public static void SalvaQuadroPagamento(long idPensione, DatiQuadroPagamento datiQuadroPagamento)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroPagamento quadroPagamento = new QuadroPagamento();
                Utility.ValorizzaOggetti(datiQuadroPagamento, quadroPagamento);
                quadroPagamento.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroPagamento(quadroPagamento);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroPagamento(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroPagamento(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroPagamento(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isRiapertura)
        {
            DatiQuadroPagamento quadroPagamento = new DatiQuadroPagamento(tipoAppartenenza, tipoDomanda, isRiapertura, datiPensione);
            SalvaQuadroPagamento(datiPensione.Id, quadroPagamento);
        }

        public static void GestioneSemaforoQuadroPagamento(GestionePensione.DatiPensione datiPensione, GestionePagamento.DatiPagamento datiPagamentoDB, ref DatiQuadroPagamento datiQuadroPagamento)
        {
            if (!datiPagamentoDB.Equals(new GestionePagamento.DatiPagamento()))
            {
                GestionePagamento.SalvaPagamento(datiPensione.Id, datiPagamentoDB);
                datiQuadroPagamento.Tipo = 2;
                datiQuadroPagamento.TabPagamento = 2;
            }
            else
            {
                GestionePagamento.EliminaPagamentoByIdPensione(datiPensione.Id);
                datiQuadroPagamento.Tipo = 0;
                datiQuadroPagamento.TabPagamento = 0;

            }

            GestioneQuadri.SalvaQuadroPagamento(datiPensione.Id, datiQuadroPagamento);
        }

        #endregion QuadroPagamento

        #region QuadroLiquidazionePensione
        public static void GetQuadroLiquidazionePensioneByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione)
        {
            QuadroLiquidazionePensione quadroLiquidazionePensione = null;
            datiQuadroLiquidazionePensione = null;
            DAGestioneQuadri.GetQuadroLiquidazionePensioneByIdPensione(datiPensione.Id, out quadroLiquidazionePensione);
            if (quadroLiquidazionePensione == null)
            {
                if (datiPensione != null)
                {
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    GestioneCtrlRic.ControlTabRic controlTabRic = null;
                    GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, tipoAppartenenza, out controlTabRic);
                    bool? isTabIstruttoriaRequired = null;
                    bool? isTabInailVisibleForAGO = null;
                    bool? isTabInailForAGORequired = null;
                    bool? isTabPrecedentePensioneAGO_FS = null;
                    bool? isTabSentenzeVisible = null;
                    //ENG - Tab Inail
                    bool isTabInailVisibleForCI = false;
                    if (tipoAppartenenza.HasValue && tipoAppartenenza == Utility.TipoAppartenenza.AGO)
                    {
                        if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione)
                            || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione)
                            || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione)
                            || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione)
                            || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione)
                            || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione))
                            isTabIstruttoriaRequired = true;
                        else
                        {
                            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria != null && datiIstruttoria.Legge44997.HasValue && datiIstruttoria.Legge44997.Value != 0)
                                isTabIstruttoriaRequired = true;
                            else
                            {
                                GestioneDatiGenericiAgoCi.PensioniDatiGenerici pensioniDatiGenerici = null;
                                GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out pensioniDatiGenerici);
                                if (pensioniDatiGenerici != null && pensioniDatiGenerici.RiduzioneRetributiva)
                                    isTabIstruttoriaRequired = true;
                                else
                                    isTabIstruttoriaRequired = false;
                            }
                        }

                        if (!string.IsNullOrEmpty(datiPensione.Gruppo) && (datiPensione.Gruppo == "0002" || datiPensione.Gruppo == "0003" ||
                            (datiPensione.Gruppo == "0031" && datiPensione.Prodotto != null && datiPensione.Prodotto.StartsWith("03"))) && !Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) && !Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria))
                            isTabInailVisibleForAGO = true;
                        else
                            isTabInailVisibleForAGO = false;

                        isTabPrecedentePensioneAGO_FS = Utility.IsDomandaTrasformazioneAOI(datiPensione).GetValueOrDefault() || Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione);

                        List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaDatiPensioniInail = null;
                        GestionePensioneInailInabilita.GetPensioniINAILByIdPensione(datiPensione.Id, out listaDatiPensioniInail);
                        GestionePensioneInailInabilita.DatiInabilita datiInabilita = null;
                        GestionePensioneInailInabilita.GetInabilitaByIdPensione(datiPensione.Id, out datiInabilita);

                        if ((listaDatiPensioniInail != null && listaDatiPensioniInail.Count(x => !x.IsNull()) > 0) || (datiInabilita != null && !datiInabilita.IsNull()))
                            isTabInailForAGORequired = true;

                        if (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                        {
                            GestioneDatiGenericiAgoCi.PensioniDatiGenerici pensioniDatiGenerici = null;
                            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out pensioniDatiGenerici);
                            List<GestioneSentenze.DatiSentenze> listaDatiSentenze = null;
                            GestioneSentenze.GetDatiSentenze(datiPensione.Id, out listaDatiSentenze);

                            if ((pensioniDatiGenerici != null && pensioniDatiGenerici.CodRicalcoloSentenza.HasValue) ||
                                (listaDatiSentenze != null && listaDatiSentenze.Count > 0))
                                isTabSentenzeVisible = true;
                        }
                    }
                    else if (tipoAppartenenza.HasValue && tipoAppartenenza == Utility.TipoAppartenenza.CI)
                    {
                        if (Utility.IsDomandaUsuranti(datiPensione) || Utility.IsDomandaSalvaguardia122(datiPensione)
                            || Utility.IsDomandaSalvaguardia214(datiPensione) || Utility.IsDomandaSalvaguardia135(datiPensione)
                            || Utility.IsDomandaSalvaguardia228(datiPensione) || Utility.IsDomandaSalvaguardia124(datiPensione)
                            || Utility.IsDomandaSalvaguardia124Art11Bis(datiPensione) || Utility.IsDomandaSalvaguardia147(datiPensione)
                            || Utility.IsDomandaEsuberiPA(datiPensione) || Utility.IsDomandaSalvaguardia147_2014(datiPensione) || Utility.IsDomandaSalvaguardia208_2015(datiPensione)
                            || Utility.IsDomandaSalvaguardia178_2020(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione))
                            isTabIstruttoriaRequired = true;
                        else
                        {
                            GestioneDatiGenericiAgoCi.PensioniDatiGenerici pensioniDatiGenerici = null;
                            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out pensioniDatiGenerici);
                            if (pensioniDatiGenerici != null && pensioniDatiGenerici.RiduzioneRetributiva)
                                isTabIstruttoriaRequired = true;
                            else
                                isTabIstruttoriaRequired = false;
                        }

                        //ENG - Tab Inail
                        List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaDatiPensioniInail = null;
                        GestionePensioneInailInabilita.GetPensioniINAILByIdPensione(datiPensione.Id, out listaDatiPensioniInail);
                        if ((listaDatiPensioniInail != null && listaDatiPensioniInail.Count(x => !x.IsNull()) > 0))
                        {
                            isTabInailVisibleForCI = true;

                        }
                    }
                    else if (tipoAppartenenza.HasValue && tipoAppartenenza == Utility.TipoAppartenenza.FS)
                    {
                        isTabPrecedentePensioneAGO_FS = Utility.IsDomandaTrasformazioneAOI(datiPensione);
                    }

                    Entity.DatiContribuzioneEnpals datiContribuzioneEnpals = null;
                    GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(datiPensione.Id, TipologiaContribuzioneEnpals.SAI, out datiContribuzioneEnpals);
                    bool tabContribEnpalsVisible = datiContribuzioneEnpals != null ? !datiContribuzioneEnpals.IsNull() : false;

                    GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    InizializzaQuadroLiquidazionePensione(datiPensione, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), tipoAppartenenza,
                        datiPensione.SiglaCategoria, datiPensione.Gestione, controlTabRic, datiPensione.DecorrenzaOriginaria, isTabIstruttoriaRequired, isTabInailVisibleForAGO, isTabPrecedentePensioneAGO_FS,
                        Utility.IsRiaperturaDomanda(datiPensione.Id), tabContribEnpalsVisible, false, false, isTabInailForAGORequired, isTabSentenzeVisible, datiDanteCausa, isTabInailVisibleForCI);
                }
                DAGestioneQuadri.GetQuadroLiquidazionePensioneByIdPensione(datiPensione.Id, out quadroLiquidazionePensione);
                if (quadroLiquidazionePensione == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro liquidazione pensione");
            }
            datiQuadroLiquidazionePensione = new DatiQuadroLiquidazionePensione();
            Utility.ValorizzaOggetti(quadroLiquidazionePensione, datiQuadroLiquidazionePensione);
        }

        public static void SalvaQuadroLiquidazionePensione(long idPensione, DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroLiquidazionePensione quadroLiquidazionePensione = new QuadroLiquidazionePensione();
                Utility.ValorizzaOggetti(datiQuadroLiquidazionePensione, quadroLiquidazionePensione);
                quadroLiquidazionePensione.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroLiquidazionePensione(quadroLiquidazionePensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroLiquidazionePensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroLiquidazionePensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroLiquidazionePensione(GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda, Utility.TipoAppartenenza? tipoAppartenenza,
            string siglaCategoria, string gestione, GestioneCtrlRic.ControlTabRic controlTabRic, DateTime? decorrenzaPensione, bool? isTabIstruttoriaRequired, bool? isTabInailVisibleForAGO, bool? isTabPrecedentePensioneAGO,
            bool isRiapertura, bool? isContribuzioneENPALSPerLiq, bool isDomandaConNuovaGestioneDatiFondoFSPT, bool isStoricoVisible, bool? isTabInailForAGORequired, bool? isTabSentenzeVisible, GestioneDanteCausa.DatiDanteCausa datiDanteCausa,
            bool isTabInailVisibleForCI)
        {
            DatiQuadroLiquidazionePensione quadroLiquidazionePensione = new DatiQuadroLiquidazionePensione(datiPensione, tipoAppartenenza, tipoDomanda, controlTabRic, decorrenzaPensione, isTabIstruttoriaRequired,
                isTabInailVisibleForAGO, isTabPrecedentePensioneAGO, isRiapertura, isContribuzioneENPALSPerLiq, isDomandaConNuovaGestioneDatiFondoFSPT, isStoricoVisible, isTabInailForAGORequired,
                isTabSentenzeVisible, datiDanteCausa, isTabInailVisibleForCI);
            SalvaQuadroLiquidazionePensione(datiPensione.Id, quadroLiquidazionePensione);
        }

        #endregion QuadroLiquidazionePensione

        #region QuadroDelegatoTutore
        public static void GetQuadroDelegatoTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroDelegatoTutore datiQuadroDelegatoTutore)
        {
            datiQuadroDelegatoTutore = null;
            QuadroDelegatoTutore quadroDelegatoTutore = null;
            DAGestioneQuadri.GetQuadroDelegatoTutoreByIdPensione(datiPensione.Id, out quadroDelegatoTutore);
            if (quadroDelegatoTutore == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
                bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                InizializzaQuadroDelegatoTutore(tipoAppartenenza, datiPensione, tipoDomanda, isRiapertura);

                DAGestioneQuadri.GetQuadroDelegatoTutoreByIdPensione(datiPensione.Id, out quadroDelegatoTutore);
                if (quadroDelegatoTutore == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro delegato/tutore");
            }
            datiQuadroDelegatoTutore = new DatiQuadroDelegatoTutore();
            Utility.ValorizzaOggetti(quadroDelegatoTutore, datiQuadroDelegatoTutore);
        }

        public static void SalvaQuadroDelegatoTutore(long idPensione, DatiQuadroDelegatoTutore datiQuadroDelegatoTutore)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDelegatoTutore quadroDelegatoTutore = new QuadroDelegatoTutore();
                Utility.ValorizzaOggetti(datiQuadroDelegatoTutore, quadroDelegatoTutore);
                quadroDelegatoTutore.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroDelegatoTutore(quadroDelegatoTutore);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDelegatoTutore(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDelegatoTutore(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroDelegatoTutore(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda, bool isRiapertura)
        {
            DatiQuadroDelegatoTutore quadroDelegatoTutore = new DatiQuadroDelegatoTutore(tipoAppartenenza, datiPensione, tipoDomanda, isRiapertura);
            SalvaQuadroDelegatoTutore(datiPensione.Id, quadroDelegatoTutore);
        }
        #endregion QuadroDelegatoTutore

        #region QuadroDatiContributivi
        public static void GetQuadroDatiContributiviByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroDatiContributivi datiQuadroDatiContributivi)
        {
            datiQuadroDatiContributivi = null;
            QuadroDatiContributivi quadroDatiContributivi = null;
            DAGestioneQuadri.GetQuadroDatiContributiviByIdPensione(datiPensione.Id, out quadroDatiContributivi);
            if (quadroDatiContributivi == null)
            {
                GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                GestioneCtrlRic.ControlTabRic controlTabRic = null;
                GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, tipoAppartenenza, out controlTabRic);
                bool isTabDatiPostDecOriginariaVisibleCI = false;
                bool? isQuotaFondoIntegrativoVisible = false;
                if (datiDanteCausa != null && datiDanteCausa.DecorrenzaPensione.HasValue)
                {
                    if (!Utility.DataSuccessivaA(datiDanteCausa.DecorrenzaPensione.Value, new DateTime(1972, 07, 01)))
                        isTabDatiPostDecOriginariaVisibleCI = true;
                }
                else
                {
                    if (datiPensione.DecorrenzaOriginaria.HasValue)
                        if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(1972, 07, 01)))
                            isTabDatiPostDecOriginariaVisibleCI = true;
                }

                bool isConvenzione13 = false;

                List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEE = null;
                GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEE);

                if (listaPrestazioniEE != null && listaPrestazioniEE.Count > 0)
                {
                    string codiceStato = string.Empty;
                    codiceStato = listaPrestazioniEE[0].CodiceStatoEE;

                    List<GestioneCtrlCodiceConvenzionePrestazioniEE.DatiCtrlCodiceConvenzionePrestazioniEE> listaCodiciConvenzione = null;
                    GestioneCtrlCodiceConvenzionePrestazioniEE.GetListaCodiceConvenzionePerStato(codiceStato, datiPensione.DecorrenzaOriginaria, out listaCodiciConvenzione);

                    if (listaCodiciConvenzione != null && listaCodiciConvenzione.Count > 0 && listaCodiciConvenzione[0].CodiceConvenzione == 13)
                        isConvenzione13 = true;
                }

                if (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                {
                    List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaQuotaFondoIntegrativo = null;
                    GestioneQuotaFondoIntegrativo.GetQuotaFondoIntegrativoByIdPensione(datiPensione.Id, out listaQuotaFondoIntegrativo);

                    if (listaQuotaFondoIntegrativo != null && listaQuotaFondoIntegrativo.Count > 0)
                        isQuotaFondoIntegrativoVisible = true;
                }

                //Todo miglioramenti contrattuali
                InizializzaQuadroDatiContributivi(datiPensione, datiDanteCausa, tipoAppartenenza, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), controlTabRic,
                    isTabDatiPostDecOriginariaVisibleCI, datiPensione.DecorrenzaOriginaria, Utility.IsRiaperturaDomanda(datiPensione.Id), false, false, false, false, isConvenzione13, isQuotaFondoIntegrativoVisible, false);

                DAGestioneQuadri.GetQuadroDatiContributiviByIdPensione(datiPensione.Id, out quadroDatiContributivi);
                if (quadroDatiContributivi == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro dati calcolo");
            }
            datiQuadroDatiContributivi = new DatiQuadroDatiContributivi();
            Utility.ValorizzaOggetti(quadroDatiContributivi, datiQuadroDatiContributivi);
        }

        public static void SalvaQuadroDatiContributivi(long idPensione, DatiQuadroDatiContributivi datiQuadroDatiContributivi)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDatiContributivi quadroDatiContributivi = new QuadroDatiContributivi();
                Utility.ValorizzaOggetti(datiQuadroDatiContributivi, quadroDatiContributivi);
                quadroDatiContributivi.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroDatiContributivi(quadroDatiContributivi);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiContributivi(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDatiContributivi(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroDatiContributivi(GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda,
            GestioneCtrlRic.ControlTabRic controlTabRic, bool isTabDatiPostDecOriginariaVisibleCI, DateTime? decorrenzaPensione, bool isRiapertura,
            bool isDomandaConNuovaGestioneDatiFondoFSPT, bool isStoricoVisible, bool isBeneficioVittimeTerrorismo, bool isDatiCalcoloPerRicRequired, bool isConvenzione13, bool? isQuotaFondoIntegrativoVisible, bool isQuoteMiglioramentiContrattualiVisible)
        {
            DatiQuadroDatiContributivi quadroDatiContributivi = new DatiQuadroDatiContributivi(tipoAppartenenza, datiPensione, datiDanteCausa, tipoDomanda, controlTabRic, isTabDatiPostDecOriginariaVisibleCI,
                decorrenzaPensione, isRiapertura, isDomandaConNuovaGestioneDatiFondoFSPT, isStoricoVisible, isBeneficioVittimeTerrorismo, isDatiCalcoloPerRicRequired, isConvenzione13, isQuotaFondoIntegrativoVisible, isQuoteMiglioramentiContrattualiVisible);

            //if (datiPensione.Gruppo.Equals("0003") && datiPensione.SiglaCategoria.StartsWith("SPI"))
            //{
            //    quadroDatiContributivi.Tipo = 2; //Controllare da dove arrivava 1
            //    quadroDatiContributivi.TabDatiCalcolo = 0;
            //}
            SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);
        }
        #endregion QuadroDatiContributivi

        #region QuadroRedditi
        public static void GetQuadroRedditiByIdPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroRedditi datiQuadroRedditi)
        {
            QuadroRedditi quadroRedditi = null;
            datiQuadroRedditi = null;
            DAGestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione.Id, out quadroRedditi);
            if (quadroRedditi == null)
            {
                if (datiPensione.Id != 0)
                {
                    bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                    Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                    InizializzaQuadroRedditi(datiPensione, isRiapertura, tipoAppartenenza, false);
                }
                DAGestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione.Id, out quadroRedditi);
                if (quadroRedditi == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro redditi");
            }
            datiQuadroRedditi = new DatiQuadroRedditi();
            Utility.ValorizzaOggetti(quadroRedditi, datiQuadroRedditi);
        }

        public static void SalvaQuadroRedditi(long idPensione, DatiQuadroRedditi datiQuadroRedditi)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroRedditi quadroRedditi = new QuadroRedditi();
                Utility.ValorizzaOggetti(datiQuadroRedditi, quadroRedditi);
                quadroRedditi.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroRedditi(quadroRedditi);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroRedditi(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroRedditi(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroRedditi(GestionePensione.DatiPensione datiPensione, bool isRiapertura, Utility.TipoAppartenenza? tipoAppartenenza, bool isTabRedditiOpzionale)
        {
            DatiQuadroRedditi quadroRedditi = new DatiQuadroRedditi(datiPensione, isRiapertura, tipoAppartenenza, isTabRedditiOpzionale);
            SalvaQuadroRedditi(datiPensione.Id, quadroRedditi);
        }
        #endregion QuadroRedditi

        #region QuadroFamiliari
        public static void GetQuadroFamiliariByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroFamiliari datiquadrofamiliari)
        {
            datiquadrofamiliari = null;
            QuadroFamiliari quadroFamiliari = null;
            DAGestioneQuadri.GetQuadroFamiliariByIdPensione(datiPensione.Id, out quadroFamiliari);

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            //ENG  - Spacchettate SOPGI
            BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            if (quadroFamiliari == null)
            {
                if (datiPensione.Id != 0)
                    InizializzaQuadroFamiliari(datiPensione, tipoAppartenenza, tipoDomanda, isRiaperturaDomanda, false, false, controlloDinamicoSpacchettate024, danteCausa);
                DAGestioneQuadri.GetQuadroFamiliariByIdPensione(datiPensione.Id, out quadroFamiliari);
                if (quadroFamiliari == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro familiari");
            }
            datiquadrofamiliari = new DatiQuadroFamiliari(tipoAppartenenza, datiPensione, tipoDomanda, isRiaperturaDomanda, false, false, controlloDinamicoSpacchettate024, danteCausa);
            Utility.ValorizzaOggetti(quadroFamiliari, datiquadrofamiliari);
        }

        public static void SalvaQuadroFamiliari(long idPensione, DatiQuadroFamiliari datiQuadroFamiliari)
        {
            using (new MethodExecutionTracer())
            {
                QuadroFamiliari quadroFamiliari = new QuadroFamiliari();
                Utility.ValorizzaOggetti(datiQuadroFamiliari, quadroFamiliari);
                quadroFamiliari.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroFamiliari(quadroFamiliari);

            }
        }

        public static void EliminaQuadroFamiliari(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroFamiliari(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroFamiliari(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isRiaperturaDomanda,
            bool isSpacchettamentoPerRicostituzione, bool isFamiliariVerde, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DatiQuadroFamiliari quadroFamiliari = new DatiQuadroFamiliari(tipoAppartenenza, datiPensione, tipoDomanda, isRiaperturaDomanda, isSpacchettamentoPerRicostituzione, isFamiliariVerde, controlloDinamicoSpacchettate024, datiDanteCausa);
            SalvaQuadroFamiliari(datiPensione.Id, quadroFamiliari);
        }
        #endregion QuadroFamiliari

        #region QuadroDanteCausa
        public static void GetQuadroDanteCausaByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroDanteCausa datiQuadroDanteCausa)
        {
            QuadroDanteCausa quadroDanteCausa = null;
            datiQuadroDanteCausa = null;
            DAGestioneQuadri.GetQuadroDanteCausaByIdPensione(datiPensione.Id, out quadroDanteCausa);
            if (quadroDanteCausa == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
                InizializzaQuadroDanteCausa(datiPensione, tipoDomanda, tipoAppartenenza, false, false, false, Utility.IsRiaperturaDomanda(datiPensione.Id), false, null);

                DAGestioneQuadri.GetQuadroDanteCausaByIdPensione(datiPensione.Id, out quadroDanteCausa);
                if (quadroDanteCausa == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro dante causa");
            }
            datiQuadroDanteCausa = new DatiQuadroDanteCausa();
            Utility.ValorizzaOggetti(quadroDanteCausa, datiQuadroDanteCausa);
        }

        public static void SalvaQuadroDanteCausa(long idPensione, DatiQuadroDanteCausa datiQuadroDanteCausa, GestionePensione.DatiPensione datiPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDanteCausa quadroDanteCausa = new QuadroDanteCausa();
                Utility.ValorizzaOggetti(datiQuadroDanteCausa, quadroDanteCausa);
                quadroDanteCausa.IdPensione = idPensione;
                bool IsDomandaRiliquidazioneIndiretta = datiPensione.Gruppo == "0051" && datiPensione.Prodotto == "0422" && datiPensione.Tipo == "0026";
                if (IsDomandaRiliquidazioneIndiretta)
                {
                    quadroDanteCausa.TabPensioneDiretta = null;
                }

                DAGestioneQuadri.SalvaQuadroDanteCausa(quadroDanteCausa);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDanteCausa(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDanteCausa(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroDanteCausa(GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda, Utility.TipoAppartenenza? tipoAppartenenza, bool isRicRev,
            bool isDiretta, bool isTabSentenza495_93AGORequired, bool isRiapertura, bool noDanteCausaAnte96, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DatiQuadroDanteCausa quadroDanteCausa = new DatiQuadroDanteCausa();
            if (tipoDomanda != Utility.TipoDomanda.Superstiti && tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti && tipoDomanda != Utility.TipoDomanda.RiliquidazioneSuperstiti && !isRiapertura)
            {
                quadroDanteCausa.Tipo = 0;
                quadroDanteCausa.TabAnagrafica = null;
                quadroDanteCausa.TabPensioneDiretta = null;
                quadroDanteCausa.TabAltraPensione = null;
                quadroDanteCausa.TabDatiPensioneCI = null;
                quadroDanteCausa.TabSentenza49593 = null;
            }
            else
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:

                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
                        switch (tipoFondo)
                        {
                            //rimossa gestione specifica per PI
                            //case Utility.TipoFondo.PI:
                            //    if (tipoDomanda != Utility.TipoDomanda.Superstiti)
                            //    {
                            //        quadroDanteCausa.Tipo = 0;
                            //        quadroDanteCausa.TabAnagrafica = null;
                            //        quadroDanteCausa.TabPensioneDiretta = null;
                            //        quadroDanteCausa.TabAltraPensione = null;
                            //        quadroDanteCausa.TabDatiPensioneCI = null;
                            //        quadroDanteCausa.TabSentenza49593 = null;
                            //    }
                            //    else
                            //    {
                            //        quadroDanteCausa.Tipo = 2;
                            //        quadroDanteCausa.TabAnagrafica = 0;
                            //        quadroDanteCausa.TabPensioneDiretta = 1;
                            //    }
                            //    break;
                            default:
                                if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                                {
                                    quadroDanteCausa.Tipo = 2;
                                    quadroDanteCausa.TabAnagrafica = 0;
                                    if (isDiretta)
                                        quadroDanteCausa.TabPensioneDiretta = 0;
                                    else
                                        quadroDanteCausa.TabPensioneDiretta = null;
                                    quadroDanteCausa.TabAltraPensione = null;
                                    quadroDanteCausa.TabDatiPensioneCI = null;
                                    quadroDanteCausa.TabSentenza49593 = null;
                                }
                                else if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                                {
                                    if (isRicRev)
                                    {
                                        if (!isRiapertura && !(tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                                        {
                                            //ENG - RICOSTITUZIONI (NO Inpdap) quadroDanteCausa non deve essere visibile
                                            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || !GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO) || Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                            {
                                                quadroDanteCausa.Tipo = 2;
                                                quadroDanteCausa.TabAnagrafica = 2;
                                            }
                                            else
                                            {
                                                quadroDanteCausa.Tipo = 0;
                                                quadroDanteCausa.TabAnagrafica = null;
                                            }
                                        }
                                        else
                                        {
                                            if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO) &&
                                                tipoDomanda == Utility.TipoDomanda.Ricostituzione && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                                            {
                                                quadroDanteCausa.Tipo = 0;
                                                quadroDanteCausa.TabAnagrafica = null;
                                            }
                                            else
                                            {
                                                quadroDanteCausa.Tipo = 2;
                                                quadroDanteCausa.TabAnagrafica = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        quadroDanteCausa.Tipo = 0;
                                        quadroDanteCausa.TabAnagrafica = null;
                                    }
                                    if (isDiretta)
                                    {
                                        if (!isRiapertura && !(tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT || Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                                            quadroDanteCausa.TabPensioneDiretta = 2;
                                        else
                                            quadroDanteCausa.TabPensioneDiretta = 0;
                                    }
                                    else
                                        quadroDanteCausa.TabPensioneDiretta = null;
                                    quadroDanteCausa.TabAltraPensione = null;
                                    quadroDanteCausa.TabDatiPensioneCI = null;
                                    quadroDanteCausa.TabSentenza49593 = null;

                                    //ENG - Ric Superstiti 024: in presenza del bypass NESSUN_DANTE_CAUSA allora il tab anagrafica obbligatorio e il tab diretta opzionale
                                    if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA) || GestioneBypassControllo.CheckBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.Dante_Causa_FS.NESSUN_DANTE_CAUSA_DINAMICO)
                                        && tipoDomanda == Utility.TipoDomanda.Ricostituzione && Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione)
                                        && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                                    {
                                        quadroDanteCausa.Tipo = 2;
                                        quadroDanteCausa.TabAnagrafica = 0;
                                        quadroDanteCausa.TabPensioneDiretta = 1;
                                    }
                                }
                                break;
                        }

                        break;
                    case Utility.TipoAppartenenza.AGO:

                        if (tipoDomanda == Utility.TipoDomanda.Superstiti || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti || tipoDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti)
                        {
                            quadroDanteCausa.Tipo = 2;
                            quadroDanteCausa.TabAnagrafica = 0;
                            if (!Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
                                quadroDanteCausa.TabSentenza49593 = 1;

                            if (Utility.IsDomandaSOAUT_Supplementare(datiPensione, isRiapertura))
                                quadroDanteCausa.TabSentenza49593 = null;

                            if (isDiretta)
                            {
                                quadroDanteCausa.TabPensioneDiretta = 0;
                                quadroDanteCausa.TabAltraPensione = 1;
                                quadroDanteCausa.TabDatiPensioneCI = null;

                            }
                            else
                            {
                                quadroDanteCausa.TabPensioneDiretta = null;
                                quadroDanteCausa.TabAltraPensione = null;
                                quadroDanteCausa.TabDatiPensioneCI = null;
                            }
                            if ((tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti || tipoDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti) && (datiPensione.Contributivo == '8' || datiPensione.Contributivo == '5'))
                                quadroDanteCausa.TabSentenza49593 = null;
                        }
                        else if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione && !(Utility.IsDomandaENPALS(datiPensione.Gestione) && !datiPensione.SiglaCategoria.StartsWith("S"))) || isRiapertura)
                        {
                            if (isRicRev)
                            {
                                quadroDanteCausa.Tipo = 2;
                                quadroDanteCausa.TabAnagrafica = 0;
                                if (isTabSentenza495_93AGORequired)
                                    quadroDanteCausa.TabSentenza49593 = 0;
                                else
                                    quadroDanteCausa.TabSentenza49593 = 1;
                            }
                            else
                            {
                                quadroDanteCausa.Tipo = 0;
                                quadroDanteCausa.TabAnagrafica = null;
                            }

                            if (Utility.IsDomandaSOAUT_Supplementare(datiPensione, isRiapertura))
                                quadroDanteCausa.TabSentenza49593 = null;

                            if (isDiretta)
                                quadroDanteCausa.TabPensioneDiretta = 0;
                            else
                                quadroDanteCausa.TabPensioneDiretta = null;
                            quadroDanteCausa.TabAltraPensione = 1;
                            quadroDanteCausa.TabDatiPensioneCI = null;

                            if (noDanteCausaAnte96)
                            {
                                quadroDanteCausa.Tipo = 0;
                                quadroDanteCausa.TabAnagrafica = null;
                                quadroDanteCausa.TabSentenza49593 = null;
                                quadroDanteCausa.TabPensioneDiretta = 0;
                                quadroDanteCausa.TabAltraPensione = 1;
                                quadroDanteCausa.TabDatiPensioneCI = null;
                            }

                            if (Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiapertura) != null && quadroDanteCausa.TabSentenza49593 != null)
                            {
                                quadroDanteCausa.TabSentenza49593 = 1;
                            }

                        }


                        break;
                    case Utility.TipoAppartenenza.CI:

                        if (tipoDomanda == Utility.TipoDomanda.Superstiti)
                        {
                            quadroDanteCausa.Tipo = 2;
                            quadroDanteCausa.TabAnagrafica = 0;
                            //ENG - Aggiornamento Modifica Sentenza 495
                            quadroDanteCausa.TabSentenza49593 = 0;

                            if (isDiretta)
                            {
                                quadroDanteCausa.TabPensioneDiretta = 0;
                                quadroDanteCausa.TabAltraPensione = 1;
                                quadroDanteCausa.TabDatiPensioneCI = 1;

                                //ENG - PL SOS, SRS, SOARTS, SOCOMS prodotto "0021" tab TabDatiPensioneCI obbligatorio
                                if ((datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SOS" || datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SRS" ||
                                    datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SOARTS" || datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SOCOMS") &&
                                    datiPensione.Prodotto == "0021")
                                    quadroDanteCausa.TabDatiPensioneCI = 0;
                            }
                            else
                            {
                                quadroDanteCausa.TabPensioneDiretta = null;
                                quadroDanteCausa.TabAltraPensione = null;
                                quadroDanteCausa.TabDatiPensioneCI = null;
                            }
                        }
                        else if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                        {
                            if (isRicRev)
                            {
                                quadroDanteCausa.Tipo = 2;
                                quadroDanteCausa.TabAnagrafica = 0;
                                quadroDanteCausa.TabSentenza49593 = 1;
                            }
                            else
                            {
                                quadroDanteCausa.Tipo = 0;
                                quadroDanteCausa.TabAnagrafica = null;
                            }
                            if (isDiretta)
                                quadroDanteCausa.TabPensioneDiretta = 0;
                            else
                                quadroDanteCausa.TabPensioneDiretta = null;
                            quadroDanteCausa.TabAltraPensione = 1;

                            //ENG - TRF/RIC SOS, SRS, SOARTS, SOCOMS tab TabDatiPensioneCI obbligatorio
                            if (!(Utility.IsRicostituzione(datiPensione.Gruppo)) && (datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SOS" || datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SRS" ||
                                datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SOARTS" || datiPensione.SiglaCategoria.ToString().Trim().ToUpperInvariant() == "SOCOMS"))
                                quadroDanteCausa.TabDatiPensioneCI = 0;
                            else
                                quadroDanteCausa.TabDatiPensioneCI = 1;

                            //ENG - Aggiornamento Modifica Sentenza 495 
                            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                            {
                                quadroDanteCausa.Tipo = 2;
                                quadroDanteCausa.TabSentenza49593 = 0;
                            }

                            if (noDanteCausaAnte96)
                            {
                                quadroDanteCausa.TabDatiPensioneCI = 1;
                                quadroDanteCausa.TabSentenza49593 = 1;

                            }

                        }

                        if ((tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti || tipoDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti) && (datiPensione.Tipo == "0026" || datiPensione.Tipo == "0027"))
                        {
                            quadroDanteCausa.Tipo = 1;
                            quadroDanteCausa.TabAnagrafica = 1;
                            quadroDanteCausa.TabPensioneDiretta = null;
                            quadroDanteCausa.TabDatiPensioneCI = 1;
                            quadroDanteCausa.TabSentenza49593 = 1;
                            quadroDanteCausa.TabAltraPensione = 1;

                        }
                        break;
                }

                if (datiPensione.Gruppo.Equals("0003") && datiPensione.Prodotto.Equals("0021"))
                    quadroDanteCausa.TabAltraPensione = 1;
            }

            SalvaQuadroDanteCausa(datiPensione.Id, quadroDanteCausa, datiPensione);
        }

        #endregion QuadroDanteCausa

        #region QuadroMaggiorazioniBenefici

        public static void GetQuadroMaggiorazioniBeneficiByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici)
        {
            QuadroMaggiorazioniBenefici quadroMaggiorazioniBenefici = null;
            datiQuadroMaggiorazioniBenefici = null;
            DAGestioneQuadri.GetQuadroMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out quadroMaggiorazioniBenefici);
            if (quadroMaggiorazioniBenefici == null)
            {
                bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                GestioneFondo.DatiFondo datiFondo = null;
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondo);
                GestioneDatiStoricoGP.DatiStoricoGP datiStorico = null;
                GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStorico);
                GestioneDanteCausa.DatiDanteCausa datiDA = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDA);
                InizializzaQuadroMaggiorazioniBenefici(datiPensione, tipoAppartenenza, datiPensione.Benefici, datiPensione.ExCombattente,
                    datiFondo != null ? datiFondo.Privilegiate : null, datiPensione.Maggiorazioni, datiFondo != null ? datiFondo.ChkDL407 : null,
                    isRiapertura, false, datiStorico != null && datiStorico.DecorrenzaMaggiorazioneSociale.HasValue, datiDA);

                DAGestioneQuadri.GetQuadroMaggiorazioniBeneficiByIdPensione(datiPensione.Id, out quadroMaggiorazioniBenefici);
                if (quadroMaggiorazioniBenefici == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro maggiorazione/benefici");
            }
            datiQuadroMaggiorazioniBenefici = new DatiQuadroMaggiorazioniBenefici();
            Utility.ValorizzaOggetti(quadroMaggiorazioniBenefici, datiQuadroMaggiorazioniBenefici);
        }

        public static void SalvaQuadroMaggiorazioniBenefici(long idPensione, DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroMaggiorazioniBenefici quadroMaggiorazioniBenefici = new QuadroMaggiorazioniBenefici();
                Utility.ValorizzaOggetti(datiQuadroMaggiorazioniBenefici, quadroMaggiorazioniBenefici);
                quadroMaggiorazioniBenefici.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroMaggiorazioniBenefici(quadroMaggiorazioniBenefici);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroMaggiorazioniBenefici(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroMaggiorazioniBenefici(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroMaggiorazioniBenefici(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool? isTabBeneficiVisible,
            bool? isTabExCombattenteVisible, bool? isTabPrivilegiateVisible, bool? isTabMaggiorazioniVisible, bool? isTabDl407Visibile, bool isRiapertura, bool isBeneficioVittimeTerrorismo,
            bool? isDecorrenzaMaggiorazioneFromPrelievo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DatiQuadroMaggiorazioniBenefici quadroMaggiorazioniBenefici = new DatiQuadroMaggiorazioniBenefici(tipoAppartenenza, isTabBeneficiVisible, isTabExCombattenteVisible,
                isTabPrivilegiateVisible, isTabMaggiorazioniVisible, datiPensione, isTabDl407Visibile, isRiapertura, isBeneficioVittimeTerrorismo, isDecorrenzaMaggiorazioneFromPrelievo, datiDanteCausa);

            SalvaQuadroMaggiorazioniBenefici(datiPensione.Id, quadroMaggiorazioniBenefici);
        }

        #endregion QuadroMaggiorazioniBenefici

        #region QuadroSupplementi

        public static void GetQuadroSupplementiByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroSupplementi datiQuadroSupplementi)
        {
            QuadroSupplementi quadroSupplementi = null;
            datiQuadroSupplementi = null;
            DAGestioneQuadri.GetQuadroSupplementiByIdPensione(datiPensione.Id, out quadroSupplementi);
            if (quadroSupplementi == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                GestioneCtrlRic.ControlTabRic controlTabRic = null;
                GestioneCtrlRic.GetCtrlTabRic(datiPensione.Prodotto, tipoAppartenenza, out controlTabRic);
                Entity.DatiContribuzioneEnpals datiContribuzioneEnpals = null;
                GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(datiPensione.Id, TipologiaContribuzioneEnpals.SAS, out datiContribuzioneEnpals);
                bool tabContribEnpalsVisible = datiContribuzioneEnpals != null ? !datiContribuzioneEnpals.IsNull() : false;

                GestioneDanteCausa.DatiDanteCausa datiDA = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDA);
                InizializzaQuadroSupplementi(datiPensione, tipoAppartenenza, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto),
                    false, false, false, false, false, controlTabRic, Utility.IsRiaperturaDomanda(datiPensione.Id), tabContribEnpalsVisible, datiDA);

                DAGestioneQuadri.GetQuadroSupplementiByIdPensione(datiPensione.Id, out quadroSupplementi);
                if (quadroSupplementi == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro supplementi");
            }
            datiQuadroSupplementi = new DatiQuadroSupplementi();
            Utility.ValorizzaOggetti(quadroSupplementi, datiQuadroSupplementi);
        }

        public static void SalvaQuadroSupplementi(long idPensione, DatiQuadroSupplementi datiQuadroSupplementi)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroSupplementi quadroSupplementi = new QuadroSupplementi();
                Utility.ValorizzaOggetti(datiQuadroSupplementi, quadroSupplementi);
                quadroSupplementi.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroSupplementi(quadroSupplementi);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroSupplementi(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroSupplementi(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroSupplementi(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza,
             Utility.TipoDomanda tipoDomanda,
            bool isSupplementiPerRic, bool isBancRicTrf, bool isSupplementiPerENPALS, bool isSupplementiPerRev, bool isSupplementiTotalIVS, GestioneCtrlRic.ControlTabRic controlTabRic, bool isRiapertura, bool isContribuzioneENPALSPerSupp, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DatiQuadroSupplementi quadroSupplementi = null;

            quadroSupplementi = new DatiQuadroSupplementi(tipoAppartenenza, tipoDomanda, isSupplementiPerRic, isBancRicTrf, isSupplementiPerENPALS, isSupplementiPerRev, isSupplementiTotalIVS, controlTabRic, isRiapertura, isContribuzioneENPALSPerSupp, datiPensione, datiDanteCausa);
            SalvaQuadroSupplementi(datiPensione.Id, quadroSupplementi);
        }

        public static void InizializzaQuadroSupplementi(long idPensione)
        {
            DatiQuadroSupplementi quadroSupplementi = new DatiQuadroSupplementi();
            SalvaQuadroSupplementi(idPensione, quadroSupplementi);
        }

        #endregion QuadroSupplementi

        #region QuadroBititolarità

        public static void GetQuadroBititolaritaByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroBititolarita datiQuadroBititolarita)
        {
            datiQuadroBititolarita = null;
            QuadroBititolarita quadroBititolarita = null;
            DAGestioneQuadri.GetQuadroBititolaritaByIdPensione(datiPensione.Id, out quadroBititolarita);
            bool? isBititolaritaVisible = null;

            if (quadroBititolarita == null)
            {
                bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                GestioneDanteCausa.DatiDanteCausa datiDA = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDA);
                InizializzaQuadroBititolarita(datiPensione, tipoAppartenenza, isRiapertura, isBititolaritaVisible, datiDA);

                DAGestioneQuadri.GetQuadroBititolaritaByIdPensione(datiPensione.Id, out quadroBititolarita);
                if (quadroBititolarita == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro bititolarità");
            }
            datiQuadroBititolarita = new DatiQuadroBititolarita();
            Utility.ValorizzaOggetti(quadroBititolarita, datiQuadroBititolarita);
        }

        public static void SalvaQuadroBititolarita(long idPensione, DatiQuadroBititolarita DatiQuadroBititolarita)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroBititolarita quadroBititolarita = new QuadroBititolarita();
                Utility.ValorizzaOggetti(DatiQuadroBititolarita, quadroBititolarita);
                quadroBititolarita.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroBititolarita(quadroBititolarita);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroBititolarita(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroBititolarita(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroBititolarita(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isRiapertura, bool? isBititolaritaVisible, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
        {
            DatiQuadroBititolarita quadroBititolarita = new DatiQuadroBititolarita(datiPensione, tipoAppartenenza, isRiapertura, isBititolaritaVisible, datiDanteCausa);
            SalvaQuadroBititolarita(datiPensione.Id, quadroBititolarita);
        }

        #endregion QuadroBititolarità

        #region QuadroEliminazione

        public static void GetQuadroEliminazioneByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroEliminazione datiQuadroEliminazione)
        {
            datiQuadroEliminazione = null;
            QuadroEliminazione quadroEliminazione = null;
            DAGestioneQuadri.GetQuadroEliminazioneByIdPensione(datiPensione.Id, out quadroEliminazione);

            if (quadroEliminazione == null)
            {
                Titolare titolare = null;
                DAGestionePensione.GetTitolareByIdPensione(datiPensione.Id, out titolare);

                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                GestionePensione.DatiEliminazione datiEliminazione = null;
                GestionePensione.GetEliminazioneByIdPensione(datiPensione.Id, out datiEliminazione);
                bool isEliminazioneRequired = datiEliminazione != null && !datiEliminazione.Equals(new GestionePensione.DatiEliminazione());

                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

                InizializzaQuadroEliminazione(datiPensione, tipoAppartenenza, titolare.DataMorte, isRiaperturaDomanda, isEliminazioneRequired, false);

                DAGestioneQuadri.GetQuadroEliminazioneByIdPensione(datiPensione.Id, out quadroEliminazione);
                if (quadroEliminazione == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro eliminazione");
            }
            datiQuadroEliminazione = new DatiQuadroEliminazione();
            Utility.ValorizzaOggetti(quadroEliminazione, datiQuadroEliminazione);
        }

        public static void SalvaQuadroEliminazione(long idPensione, DatiQuadroEliminazione DatiQuadroEliminazione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroEliminazione quadroEliminazione = new QuadroEliminazione();
                Utility.ValorizzaOggetti(DatiQuadroEliminazione, quadroEliminazione);
                quadroEliminazione.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroEliminazione(quadroEliminazione);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroEliminazione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroEliminazione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroEliminazione(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, DateTime? dataMorte, bool isRiaperturaDomanda,
            bool isEliminazioneRequired, bool tabEliminazioneGialloAutomazione)
        {
            DatiQuadroEliminazione quadroEliminazione = new DatiQuadroEliminazione(tipoAppartenenza, datiPensione, dataMorte,
                Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), isRiaperturaDomanda, isEliminazioneRequired, tabEliminazioneGialloAutomazione);
            SalvaQuadroEliminazione(datiPensione.Id, quadroEliminazione);
        }

        #endregion QuadroEliminazione

        #region QuadroOneri
        public static void GetQuadroOneriByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroOneri datiQuadroOneri)
        {
            datiQuadroOneri = null;
            QuadroOneri quadroOneri = null;
            DAGestioneQuadri.GetQuadroOneriByIdPensione(datiPensione.Id, out quadroOneri);

            if (quadroOneri == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                bool? isPrepVisib = Utility.IsTabPrepensionamentoVisible(datiPensione, datiPensione.AttivitaEconomica, datiPensione.ProfessioneIndividuale, datiPensione.NaturaPensione);
                List<GestioneOneri.DatiOneri> lstOneri;
                GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
                GestioneOneri.GetOneriByIdPensione(datiPensione.Id, out lstOneri);
                GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(datiPensione.Id, out datiBeneficioVittimeTerrorismo);
                bool isOneriVisible = false;
                if ((lstOneri != null && lstOneri.Count > 0) || Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, datiBeneficioVittimeTerrorismo) ||
                    Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, datiBeneficioVittimeTerrorismo) || Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione) ||
                    Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione) || Utility.IsPrepensionamentoEditoriaArt1c500L160_2019(datiPensione) ||
                    Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaAPEPrecoci(datiPensione))
                    isOneriVisible = true;

                bool isOneriRicPrepensionamentoTipo0162NotVisible = false;
                if (Utility.IsRicostituzione(datiPensione.Gruppo) && (Utility.IsPrepensionamentoEditoriaFiltroEAA(datiPensione) || Utility.IsPrepensionamentoEditoriaFiltroEBA(datiPensione)) && (lstOneri == null || lstOneri.Count == 0) &&
                    !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "O" && datiPensione.DecorrenzaOriginaria.HasValue &&
                    !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2017, 01, 01)))
                    isOneriRicPrepensionamentoTipo0162NotVisible = true;

                bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
                InizializzaQuadroOneri(datiPensione, tipoAppartenenza, tipoDomanda, isRiapertura, isOneriVisible, isPrepVisib, false, isOneriRicPrepensionamentoTipo0162NotVisible);

                DAGestioneQuadri.GetQuadroOneriByIdPensione(datiPensione.Id, out quadroOneri);
                if (quadroOneri == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro oneri");
            }
            datiQuadroOneri = new DatiQuadroOneri();
            Utility.ValorizzaOggetti(quadroOneri, datiQuadroOneri);
        }

        public static void SalvaQuadroOneri(long idPensione, DatiQuadroOneri DatiQuadroOneri)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroOneri quadroOneri = new QuadroOneri();
                Utility.ValorizzaOggetti(DatiQuadroOneri, quadroOneri);
                quadroOneri.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroOneri(quadroOneri);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroOneri(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroOneri(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroOneri(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isRiapertura, bool? IsOneriVisible,
            bool? IsPrepVisible, bool isStoricoVisible, bool isOneriRicPrepensionamentoTipo0162NotVisible)
        {
            DatiQuadroOneri quadroOneri = new DatiQuadroOneri(tipoAppartenenza, IsOneriVisible, IsPrepVisible, tipoDomanda, isRiapertura, isStoricoVisible, datiPensione, isOneriRicPrepensionamentoTipo0162NotVisible);
            SalvaQuadroOneri(datiPensione.Id, quadroOneri);
        }

        #endregion QuadroOneri

        #region QuadroDatiFondo
        public static void GetQuadroDatiFondoByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            datiQuadroDatiFondo = null;
            QuadroDatiFondo quadroDatiFondo = null;
            DAGestioneQuadri.GetQuadroDatiFondoByIdPensione(datiPensione.Id, out quadroDatiFondo);

            if (quadroDatiFondo == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                bool isDatiFondoOpzionale = Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione);
                InizializzaQuadroDatiFondo(datiPensione, tipoAppartenenza, false, isDatiFondoOpzionale);
            }
            datiQuadroDatiFondo = new DatiQuadroDatiFondo();
            Utility.ValorizzaOggetti(quadroDatiFondo, datiQuadroDatiFondo);
        }

        public static void SalvaQuadroDatiFondo(long idPensione, DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDatiFondo quadroDatiFondo = new QuadroDatiFondo();
                Utility.ValorizzaOggetti(datiQuadroDatiFondo, quadroDatiFondo);
                quadroDatiFondo.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroDatiFondo(quadroDatiFondo);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiFondo(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDatiFondo(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroDatiFondo(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isNuovaGestioneDatiFondoFSPT, bool? isDatiFondoOpzionale)
        {
            DatiQuadroDatiFondo quadroDatiFondo = new DatiQuadroDatiFondo(datiPensione, tipoAppartenenza, isNuovaGestioneDatiFondoFSPT, isDatiFondoOpzionale);
            SalvaQuadroDatiFondo(datiPensione.Id, quadroDatiFondo);
        }

        #endregion QuadroDatiFondo

        #region QuadroDatiRecordFondo
        public static void GetQuadroDatiRecordFondoByDatiPensione(GestionePensione.DatiPensione datiPensione, out List<DatiQuadroDatiRecordFondo> listaDatiQuadroDatiRecordFondo)
        {
            listaDatiQuadroDatiRecordFondo = null;
            List<QuadroDatiRecordFondo> listaQuadroDatiRecordFondo = null;
            DAGestioneQuadri.GetQuadroDatiRecordFondoByIdPensione(datiPensione.Id, out listaQuadroDatiRecordFondo);

            if (listaQuadroDatiRecordFondo != null && listaQuadroDatiRecordFondo.Count > 0)
            {
                listaDatiQuadroDatiRecordFondo = new List<DatiQuadroDatiRecordFondo>();
                foreach (QuadroDatiRecordFondo quadroDatiRecordFondo in listaQuadroDatiRecordFondo)
                {
                    DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = new DatiQuadroDatiRecordFondo();
                    Utility.ValorizzaOggetti(quadroDatiRecordFondo, datiQuadroDatiRecordFondo);
                    listaDatiQuadroDatiRecordFondo.Add(datiQuadroDatiRecordFondo);
                }
            }
        }

        public static void GetQuadroDatiRecordFondoByIdRecordFondo(long idRecordFondo, out DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo)
        {
            datiQuadroDatiRecordFondo = null;
            QuadroDatiRecordFondo quadroDatiRecordFondo = null;
            DAGestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out quadroDatiRecordFondo);

            if (quadroDatiRecordFondo != null)
            {
                datiQuadroDatiRecordFondo = new DatiQuadroDatiRecordFondo();
                Utility.ValorizzaOggetti(quadroDatiRecordFondo, datiQuadroDatiRecordFondo);
            }
        }

        public static void SalvaQuadroDatiRecordFondo(long idPensione, long idRecordFondo, DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDatiRecordFondo quadroDatiRecordFondo = new QuadroDatiRecordFondo();
                Utility.ValorizzaOggetti(datiQuadroDatiRecordFondo, quadroDatiRecordFondo);
                quadroDatiRecordFondo.IdPensione = idPensione;
                quadroDatiRecordFondo.IdRecordFondo = idRecordFondo;
                DAGestioneQuadri.SalvaQuadroDatiRecordFondo(quadroDatiRecordFondo);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiRecordFondoByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaAllQuadroDatiRecordFondo(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiRecordFondoByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        public static List<DatiQuadroDatiRecordFondo> InizializzaQuadroDatiRecordFondo(GestionePensione.DatiPensione datiPensione, List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo,
            Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isNuovaGestioneDatiFondoFSPT, List<GestioneFondo.DatiFondoPT> datiFondoPensioneDatiPT, bool? isTabPrivilegiataRequired, bool? isTabArticolo2Required, bool? isDatiFondoOpzionale, bool datiFondoPensioneDatiPTdaPrelievo, bool? isTabArticolo2NotVisible)
        {
            List<DatiQuadroDatiRecordFondo> listaQuadroDatiRecordFondo = null;
            if (listaDatiRecordFondo != null && listaDatiRecordFondo.Count > 0)
            {
                listaQuadroDatiRecordFondo = new List<DatiQuadroDatiRecordFondo>();

                foreach (GestioneRecordFondo.DatiRecordFondo recordFondo in listaDatiRecordFondo)
                {
                    /*28/10/2021 - Nuova gestione semaforica del tab legge 4/60 per le PT (Le FS non prevedono questo tab).
                     * Ogni record fondo ha la sua gestione per il semaforo del tab legge 4/60*/
                    bool? isTabLegge460Required = null;
                    if (datiFondoPensioneDatiPT != null && datiFondoPensioneDatiPT.Count() > 0)
                    {
                        if (datiFondoPensioneDatiPT.Exists(x => x.IdRecordFondo == recordFondo.Id && !x.IsLegge460Null()))
                            isTabLegge460Required = true;
                    }

                    DatiQuadroDatiRecordFondo quadroDatiRecordFondo = new DatiQuadroDatiRecordFondo(recordFondo.Id, tipoAppartenenza, tipoDomanda, datiPensione, isNuovaGestioneDatiFondoFSPT,
                        isTabLegge460Required, isTabPrivilegiataRequired, isTabArticolo2Required, isDatiFondoOpzionale, datiFondoPensioneDatiPTdaPrelievo, isTabArticolo2NotVisible);
                    SalvaQuadroDatiRecordFondo(datiPensione.Id, recordFondo.Id, quadroDatiRecordFondo);
                    listaQuadroDatiRecordFondo.Add(quadroDatiRecordFondo);
                }
            }

            return listaQuadroDatiRecordFondo;
        }

        #endregion QuadroDatiRecordFondo

        #region QuadroDatiNoCalcolo

        public static void GetQuadroDatiNoCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroDatiNoCalcolo datiQuadroNoCalcolo)
        {
            datiQuadroNoCalcolo = null;
            QuadroDatiNoCalcolo quadroDatiNoCalcolo = null;
            DAGestioneQuadri.GetQuadroNoCalcoloByIdPensione(datiPensione.Id, out quadroDatiNoCalcolo);
            if (quadroDatiNoCalcolo == null)
            {
                bool isDatiNoCalcoloVisible = false;
                Utility.CategoriaFondoPI? categoriaPI = Utility.GetCategoriaFondoPI(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione), datiPensione.SiglaCategoria);
                if (categoriaPI == Utility.CategoriaFondoPI.U || categoriaPI == Utility.CategoriaFondoPI.V || categoriaPI == Utility.CategoriaFondoPI.A)
                {
                    List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo = null;
                    GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstRecordFondo);
                    if (lstRecordFondo != null && lstRecordFondo.Count > 0 && lstRecordFondo.Exists(x => x.CodiceNonCalcolo == 'S'))
                        isDatiNoCalcoloVisible = true;
                }

                InizializzaQuadroDatiNoCalcolo(datiPensione, isDatiNoCalcoloVisible);
                DAGestioneQuadri.GetQuadroNoCalcoloByIdPensione(datiPensione.Id, out quadroDatiNoCalcolo);
            }
            datiQuadroNoCalcolo = new DatiQuadroDatiNoCalcolo();
            Utility.ValorizzaOggetti(quadroDatiNoCalcolo, datiQuadroNoCalcolo);
        }

        public static void SalvaQuadroDatiNoCalcolo(long idPensione, DatiQuadroDatiNoCalcolo datiQuadroNoCalcolo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.QuadroDatiNoCalcolo quadroDatiNoCalcoloDb = new DataCommon.QuadroDatiNoCalcolo();
                Utility.ValorizzaOggetti(datiQuadroNoCalcolo, quadroDatiNoCalcoloDb);
                quadroDatiNoCalcoloDb.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroDatiNoCalcolo(quadroDatiNoCalcoloDb);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiNoCalcolo(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroDatiNoCalcolo(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroDatiNoCalcolo(GestionePensione.DatiPensione datiPensione, bool isVisible)
        {
            DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo = new DatiQuadroDatiNoCalcolo(datiPensione, isVisible);
            SalvaQuadroDatiNoCalcolo(datiPensione.Id, quadroDatiNoCalcolo);
        }

        #endregion QuadroDatiNoCalcolo

        #region QuadroDatiRecordNoCalcolo

        public static void GetQuadroDatiRecordNoCalcoloByDatiPensione(GestionePensione.DatiPensione datiPensione, out List<DatiQuadroRecordNoCalcolo> listaDatiQuadroDatiRecordNoCalcolo)
        {
            listaDatiQuadroDatiRecordNoCalcolo = null;
            List<QuadroDatiRecordNoCalcolo> listaQuadroDatiRecordNoCalcolo = null;
            DAGestioneQuadri.GetQuadroDatiRecordNoCalcoloByIdPensione(datiPensione.Id, out listaQuadroDatiRecordNoCalcolo);

            if (listaQuadroDatiRecordNoCalcolo != null && listaQuadroDatiRecordNoCalcolo.Count > 0)
            {
                listaDatiQuadroDatiRecordNoCalcolo = new List<DatiQuadroRecordNoCalcolo>();
                foreach (QuadroDatiRecordNoCalcolo record in listaQuadroDatiRecordNoCalcolo)
                {
                    DatiQuadroRecordNoCalcolo datiQuadroDatiRecordFondo = new DatiQuadroRecordNoCalcolo();
                    Utility.ValorizzaOggetti(record, datiQuadroDatiRecordFondo);
                    listaDatiQuadroDatiRecordNoCalcolo.Add(datiQuadroDatiRecordFondo);
                }
            }
        }

        public static void GetQuadroDatiRecordNoCalcoloByIdRecord(long idRecord, out DatiQuadroRecordNoCalcolo datiQuadroDatiRecordNoCalcolo)
        {
            datiQuadroDatiRecordNoCalcolo = null;
            QuadroDatiRecordNoCalcolo quadroDatiRecordFondo = null;
            DAGestioneQuadri.GetQuadroDatiRecordNoCalcoloByIdRecord(idRecord, out quadroDatiRecordFondo);

            if (quadroDatiRecordFondo != null)
            {
                datiQuadroDatiRecordNoCalcolo = new DatiQuadroRecordNoCalcolo();
                Utility.ValorizzaOggetti(quadroDatiRecordFondo, datiQuadroDatiRecordNoCalcolo);
            }
        }

        public static void SalvaQuadroDatiRecordNoCalcolo(long idPensione, long idRecordFondo, DatiQuadroRecordNoCalcolo datiQuadroRecordNoCalcolo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroDatiRecordNoCalcolo quadroDatiRecordNoCalcolo = new QuadroDatiRecordNoCalcolo();
                Utility.ValorizzaOggetti(datiQuadroRecordNoCalcolo, quadroDatiRecordNoCalcolo);
                quadroDatiRecordNoCalcolo.IdPensione = idPensione;
                quadroDatiRecordNoCalcolo.IdRecordDatiNoCalcolo = idRecordFondo;
                DAGestioneQuadri.SalvaQuadroRecordNoCalcolo(quadroDatiRecordNoCalcolo);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiRecordNoCalcoloByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaAllQuadroRecordNoCalcolo(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroDatiRecordNoCalcoloByIdRecord(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroRecordNoCalcoloByIdRecord(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #endregion QuadroDatiRecordNoCalcolo

        #region QuadroPeriodi

        public static void GetQuadroPeriodiByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroPeriodi datiQuadroPeriodi)
        {
            datiQuadroPeriodi = null;
            QuadroPeriodi quadroPeriodi = null;
            DAGestioneQuadri.GetQuadroPeriodiByIdPensione(datiPensione.Id, out quadroPeriodi);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (quadroPeriodi == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

                //ENG - Spacchettate SOPGI
                BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

                InizializzaQuadroPeriodi(datiPensione, tipoAppartenenza, isRiaperturaDomanda, false, controlloDinamicoSpacchettate024, danteCausa);

                DAGestioneQuadri.GetQuadroPeriodiByIdPensione(datiPensione.Id, out quadroPeriodi);
                if (quadroPeriodi == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro periodi");
            }
            datiQuadroPeriodi = new DatiQuadroPeriodi();
            Utility.ValorizzaOggetti(quadroPeriodi, datiQuadroPeriodi);
        }

        public static void SalvaQuadroPeriodi(long idPensione, DatiQuadroPeriodi datiQuadroPeriodi)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroPeriodi quadroPeriodi = new QuadroPeriodi();
                Utility.ValorizzaOggetti(datiQuadroPeriodi, quadroPeriodi);
                quadroPeriodi.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroPeriodi(quadroPeriodi);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroPeriodi(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroPeriodi(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroPeriodi(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            DatiQuadroPeriodi quadroPeriodi = new DatiQuadroPeriodi(tipoAppartenenza, datiPensione, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), isRiaperturaDomanda, isSpacchettamentoPerRicostituzione, controlloDinamicoSpacchettate024, danteCausa);
            SalvaQuadroPeriodi(datiPensione.Id, quadroPeriodi);
        }

        #endregion QuadroPeriodi

        #region QuadroAventiDiritto

        public static void GetQuadroAventiDirittoByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroAventiDiritto datiQuadroAventiDiritto)
        {
            datiQuadroAventiDiritto = null;
            QuadroAventiDiritto quadroAventiDiritto = null;
            DAGestioneQuadri.GetQuadroAventiDirittoByIdPensione(datiPensione.Id, out quadroAventiDiritto);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            if (quadroAventiDiritto == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

                //ENG - Spacchettate SOPGI
                BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa = null;
                BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

                InizializzaQuadroAventiDiritto(datiPensione, tipoAppartenenza, isRiaperturaDomanda, false, controlloDinamicoSpacchettate024, danteCausa);

                DAGestioneQuadri.GetQuadroAventiDirittoByIdPensione(datiPensione.Id, out quadroAventiDiritto);
                if (quadroAventiDiritto == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro aventi diritto");
            }
            datiQuadroAventiDiritto = new DatiQuadroAventiDiritto();
            Utility.ValorizzaOggetti(quadroAventiDiritto, datiQuadroAventiDiritto);
        }

        public static void SalvaQuadroAventiDiritto(long idPensione, DatiQuadroAventiDiritto datiQuadroAventiDiritto)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroAventiDiritto quadroAventiDiritto = new QuadroAventiDiritto();
                Utility.ValorizzaOggetti(datiQuadroAventiDiritto, quadroAventiDiritto);
                quadroAventiDiritto.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroAventiDiritto(quadroAventiDiritto);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroAventiDiritto(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroAventiDiritto(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroAventiDiritto(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            DatiQuadroAventiDiritto quadroAventiDiritto = new DatiQuadroAventiDiritto(tipoAppartenenza, datiPensione, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), isRiaperturaDomanda, isSpacchettamentoPerRicostituzione, controlloDinamicoSpacchettate024, danteCausa);
            SalvaQuadroAventiDiritto(datiPensione.Id, quadroAventiDiritto);
        }

        #endregion QuadroAventiDiritto

        #region QuadroAltreDomandeCollegate

        public static void GetQuadroAltreDomandeCollegateByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroAltreDomandeCollegate datiQuadroAltreDomandeCollegate)
        {
            datiQuadroAltreDomandeCollegate = null;
            QuadroAltreDomandeCollegate quadroAltreDomandeCollegate = null;
            DAGestioneQuadri.GetQuadroAltreDomandeCollegateByIdPensione(datiPensione.Id, out quadroAltreDomandeCollegate);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024 = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneSpacchettate024", out controlloDinamicoSpacchettate024);

            //ENG - Spacchettate SOPGI
            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            if (quadroAltreDomandeCollegate == null)
            {
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

                InizializzaQuadroAltreDomandeCollegate(datiPensione, tipoAppartenenza, isRiaperturaDomanda, false, controlloDinamicoSpacchettate024, danteCausa);

                DAGestioneQuadri.GetQuadroAltreDomandeCollegateByIdPensione(datiPensione.Id, out quadroAltreDomandeCollegate);
                if (quadroAltreDomandeCollegate == null)
                    throw new INPS.DNA.DnaApplicationException("Errore nel recupero del quadro altre domande collegate");
            }
            datiQuadroAltreDomandeCollegate = new DatiQuadroAltreDomandeCollegate();
            Utility.ValorizzaOggetti(quadroAltreDomandeCollegate, datiQuadroAltreDomandeCollegate);
        }

        public static void SalvaQuadroAltreDomandeCollegate(long idPensione, DatiQuadroAltreDomandeCollegate datiQuadroAltreDomandeCollegate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroAltreDomandeCollegate quadroAltreDomandeCollegate = new QuadroAltreDomandeCollegate();
                Utility.ValorizzaOggetti(datiQuadroAltreDomandeCollegate, quadroAltreDomandeCollegate);
                quadroAltreDomandeCollegate.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroAltreDomandeCollegate(quadroAltreDomandeCollegate);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroAltreDomandeCollegate(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroAltreDomandeCollegate(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroAltreDomandeCollegate(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, GestioneDanteCausa.DatiDanteCausa danteCausa)
        {
            DatiQuadroAltreDomandeCollegate quadroAltreDomandeCollegate = new DatiQuadroAltreDomandeCollegate(tipoAppartenenza, datiPensione, Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), isRiaperturaDomanda,
                isSpacchettamentoPerRicostituzione, controlloDinamicoSpacchettate024, danteCausa);
            SalvaQuadroAltreDomandeCollegate(datiPensione.Id, quadroAltreDomandeCollegate);
        }

        #endregion QuadroAltreDomandeCollegate

        #region QuadroRichiestaBonus

        public static void GetQuadroRichiestaBonusByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiQuadroRichiestaBonus datiQuadroRichiestaBonus)
        {
            datiQuadroRichiestaBonus = null;
            QuadroRichiestaBonus quadroRichiestaBonus = null;
            DAGestioneQuadri.GetQuadroRichiestaBonusByIdPensione(datiPensione.Id, out quadroRichiestaBonus);

            datiQuadroRichiestaBonus = new DatiQuadroRichiestaBonus();
            Utility.ValorizzaOggetti(quadroRichiestaBonus, datiQuadroRichiestaBonus);
        }

        public static void SalvaQuadroRichiestaBonus(long idPensione, DatiQuadroRichiestaBonus DatiQuadroRichiestaBonus)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuadroRichiestaBonus quadroRichiestaBonus = new QuadroRichiestaBonus();
                Utility.ValorizzaOggetti(DatiQuadroRichiestaBonus, quadroRichiestaBonus);
                quadroRichiestaBonus.IdPensione = idPensione;
                DAGestioneQuadri.SalvaQuadroRichiestaBonus(quadroRichiestaBonus);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuadroRichiestaBonus(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneQuadri.EliminaQuadroRichiestaBonus(idPensione);
                transactionScope.Complete();
            }
        }

        public static void InizializzaQuadroRichiestaBonus(GestionePensione.DatiPensione datiPensione)
        {
            DatiQuadroRichiestaBonus quadroRichiestaBonus = new DatiQuadroRichiestaBonus(datiPensione);

            SalvaQuadroRichiestaBonus(datiPensione.Id, quadroRichiestaBonus);
        }

        #endregion QuadroRichiestaBonus

        #region nested classes
        public class DatiQuadroTitolare
        {
            public DatiQuadroTitolare()
            {
                //valori di default
                this._Tipo = 2;
                this._TabAnagrafica = 0;
                this._TabStatiCivili = 0;
                this._TabResidenzeEstero = 1;
            }

            public DatiQuadroTitolare(long idPensione, bool isResidenteEstero, Utility.TipoAppartenenza? tipoAppartenenza,
                Utility.TipoDomanda tipoDomanda, GestioneCtrlRic.ControlTabRic controlTabRic, bool isResEsteroPerRic, bool isRiapertura)
            {
                //valori di default
                this._Tipo = 2;
                this._TabAnagrafica = 0;
                this._TabStatiCivili = 0;

                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
                {
                    if (isResidenteEstero ||
                        ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && controlTabRic != null && controlTabRic.TabResEstero) ||
                        ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && isResEsteroPerRic))
                        this._TabResidenzeEstero = 0;
                    else
                        this._TabResidenzeEstero = null;
                }
                else if (tipoAppartenenza.HasValue && (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI))
                {
                    if (isResidenteEstero)
                        this._TabResidenzeEstero = 0;
                    else
                        this._TabResidenzeEstero = 1;
                }
                else
                    this._TabResidenzeEstero = 1;
            }
            public DatiQuadroTitolare(System.Nullable<byte> tipo, System.Nullable<byte> tabAnagrafica, System.Nullable<byte> tabStatiCivili, System.Nullable<byte> tabResidenzeEstero)
            {
                this._Tipo = tipo;
                this._TabAnagrafica = tabAnagrafica;
                this._TabStatiCivili = tabStatiCivili;
                this._TabResidenzeEstero = tabResidenzeEstero;

            }
            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabAnagrafica;

            private System.Nullable<byte> _TabStatiCivili;

            private System.Nullable<byte> _TabResidenzeEstero;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabAnagrafica { get { return _TabAnagrafica; } set { _TabAnagrafica = value; } }

            public System.Nullable<byte> TabStatiCivili { get { return _TabStatiCivili; } set { _TabStatiCivili = value; } }

            public System.Nullable<byte> TabResidenzeEstero { get { return _TabResidenzeEstero; } set { _TabResidenzeEstero = value; } }
            #endregion public properties
        }

        public class DatiQuadroDetrazioni
        {
            public DatiQuadroDetrazioni()
            {
                //valori di default
                this._Tipo = 2;
                this._TabDetrazioni = 0;
            }

            public DatiQuadroDetrazioni(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isRiapertura, bool? isDetrazioniObbligatorio)
            {
                //se non è una CI ed è una ricostituzione o una ripertura
                //oppure, se è una AGO ed è una beneficio terrorismo over o under 80

                bool detrazioniNonObbligatorie = (isDetrazioniObbligatorio != true); // null o false
                bool ricostOrRiap = (tipoDomanda == Utility.TipoDomanda.Ricostituzione) || isRiapertura;

                bool agoTerrorismo =
                    tipoAppartenenza.HasValue &&
                    tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO &&
                    (
                        Utility.IsDomandaBeneficioTerrorismoOver80(datiPensione, null) ||
                        Utility.IsDomandaBeneficioTerrorismoUnder80(datiPensione, null)
                    );

                bool vocredOrCoop =
                    Utility.IsDomandaVOCRED_CRED27(datiPensione.SiglaCategoria) ||
                    Utility.IsDomandaVOCOOP_COOP28(datiPensione.SiglaCategoria);

                if (detrazioniNonObbligatorie && (ricostOrRiap || agoTerrorismo || vocredOrCoop))
                {
                    this.Tipo = 0; // quadro non visibile
                    this._TabDetrazioni = null;
                }
                else
                {
                    // valori di default
                    this._Tipo = 2;
                    this._TabDetrazioni = 0;
                }
            }

                public DatiQuadroDetrazioni(System.Nullable<byte> tipo, System.Nullable<byte> tabDetrazioni)
            {
                this._Tipo = tipo;
                this._TabDetrazioni = tabDetrazioni;

            }
            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabDetrazioni;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabDetrazioni { get { return _TabDetrazioni; } set { _TabDetrazioni = value; } }
            #endregion public properties
        }

        public class DatiQuadroPagamento
        {
            public DatiQuadroPagamento()
            {
                //valori di default
                this._Tipo = 2;
                this._TabPagamento = 0;
            }

            public DatiQuadroPagamento(Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isRiapertura, GestionePensione.DatiPensione datiPensione)
            {
                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                {
                    this.Tipo = 0;
                    this._TabPagamento = null;
                }
                else
                {
                    //valori di default
                    this._Tipo = 2;
                    this._TabPagamento = 0;
                }

                if (datiPensione.TipoAutomazione == (byte)Utility.TipoAutomazione.Vecchiaia)
                {
                    this._Tipo = 1;
                    this._TabPagamento = 1;
                }
            }
            public DatiQuadroPagamento(System.Nullable<byte> tipo, System.Nullable<byte> tabPagamento)
            {
                this._Tipo = tipo;
                this._TabPagamento = tabPagamento;

            }
            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabPagamento;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabPagamento { get { return _TabPagamento; } set { _TabPagamento = value; } }
            #endregion public properties
        }

        public class DatiQuadroLiquidazionePensione
        {
            public DatiQuadroLiquidazionePensione()
            {
                //valori di default FS
                this._Tipo = 2;
                this._TabDatiGenerici = 0;
                this._TabOpzione = null;
                this._TabPrecedentePensione = 1;
                this._TabIstruttoria = 1;
                this._TabDatiAssicurativi = 0;
                this._TabInail = 1;
                this._TabDatiLegge460 = null;
                this._TabContribuzioneEnpals = null;
            }

            public DatiQuadroLiquidazionePensione(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda,
                GestioneCtrlRic.ControlTabRic controlTabRic, DateTime? decorrenzaPensione, bool? isTabIstruttoriaRequired, bool? isTabInailVisibleForAGO,
                bool? isTabPrecedentePensioneAGO_FS, bool isRiapertura, bool? isContribuzioneENPALSPerLiq, bool isDomandaConNuovaGestioneDatiFondoFSPT, bool isStoricoVisible, bool? isTabInailForAGORequired,
                bool? isTabSentenzeVisible, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, bool isTabInailVisibleForCI)
            {
                DateTime dataCompare = new DateTime(1980, 01, 01);
                if (tipoAppartenenza != null)
                {
                    switch (tipoAppartenenza)
                    {
                        case Utility.TipoAppartenenza.AGO:
                            //valori di default AGO
                            this._Tipo = 2;
                            this._TabDatiGenerici = 0;
                            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                                this._TabOpzione = null;
                            else
                            {
                                if (!decorrenzaPensione.HasValue || decorrenzaPensione.Value >= dataCompare)
                                    this._TabOpzione = null;
                                else
                                    this._TabOpzione = 1;
                            }
                            if (isTabPrecedentePensioneAGO_FS.HasValue && isTabPrecedentePensioneAGO_FS.Value)
                                this._TabPrecedentePensione = 0;
                            else
                                this._TabPrecedentePensione = 1;
                            if (isTabIstruttoriaRequired.HasValue && isTabIstruttoriaRequired.Value && !((bool)Utility.IsDomandaRipristino(datiPensione)) &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaSO(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "O"))
                            {
                                this._TabIstruttoria = 0;
                            }
                            else
                                this._TabIstruttoria = 1;

                            if (((Utility.IsDomandaSPED(datiPensione) || Utility.IsDomandaVOST(datiPensione.SiglaCategoria) || Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsRenditaFacoltativa(datiPensione)) &&
                                (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)) ||
                                Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                                this._TabDatiAssicurativi = 1;
                            else
                                this._TabDatiAssicurativi = 0;
                            // Se sono presenti dei valori relativi al TabInail lo mostro a rosso in ogni caso
                            if (isTabInailForAGORequired.HasValue && isTabInailForAGORequired.Value)
                                this._TabInail = 0;
                            else if (isTabInailVisibleForAGO.HasValue && isTabInailVisibleForAGO.Value)
                                this._TabInail = 1;
                            else
                                this._TabInail = null;
                            this._TabDatiLegge460 = null;

                            if (isContribuzioneENPALSPerLiq.Value)
                            {
                                this._TabContribuzioneEnpals = 0;
                                this._Tipo = 2;
                            }
                            else
                                this._TabContribuzioneEnpals = null;

                            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) ||
                                Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) ||
                                Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) ||
                                Utility.IsDomandaMIN(datiPensione.SiglaCategoria) ||
                                Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) ||
                                Utility.IsRenditaCasalinghe(datiPensione) ||
                                Utility.IsRenditaFacoltativa(datiPensione) ||
                                Utility.IsDomandaVOST(datiPensione.SiglaCategoria))
                                this._TabIstruttoria = null;

                            if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria))
                                this._TabIstruttoria = 0;

                            if (isStoricoVisible)
                                this._TabStorico = 0;

                            if (Utility.IsRicostituzioneContributivaPerEsecuzioneSentenza(datiPensione))
                                this._TabSentenzaArt4 = 0;

                            if (isTabSentenzeVisible.HasValue && isTabSentenzeVisible.Value)
                                this._TabSentenze = 1;
                            if (Utility.IsDomandaPSO(datiPensione.SiglaCategoria))
                                this.TabDatiAssicurativi = null;
                            break;
                        case Utility.TipoAppartenenza.FS:
                            //valori di default FS

                            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.PI:
                                case Utility.TipoFondo.PL:
                                    this._Tipo = 2;
                                    this._TabDatiGenerici = 0;
                                    this._TabDatiAssicurativi = 0;
                                    this._TabOpzione = null;
                                    this._TabPrecedentePensione = null;
                                    this._TabIstruttoria = null;
                                    this._TabInail = null;
                                    this._TabDatiLegge460 = null;
                                    break;

                                case Utility.TipoFondo.PT:
                                    this._Tipo = 2;
                                    this._TabDatiGenerici = 0;
                                    this._TabOpzione = null;
                                    this._TabPrecedentePensione = 1;
                                    this._TabIstruttoria = null;
                                    this._TabDatiAssicurativi = 0;
                                    this._TabInail = 1;
                                    if (!isDomandaConNuovaGestioneDatiFondoFSPT)
                                        this._TabDatiLegge460 = 1;
                                    else
                                        this._TabDatiLegge460 = null;
                                    break;

                                case Utility.TipoFondo.EL:
                                case Utility.TipoFondo.TT:
                                case Utility.TipoFondo.ET:
                                case Utility.TipoFondo.VL:
                                    this._Tipo = 2;
                                    this._TabDatiGenerici = 0;
                                    this._TabOpzione = null;
                                    if (isTabPrecedentePensioneAGO_FS.HasValue && isTabPrecedentePensioneAGO_FS.Value)
                                        this._TabPrecedentePensione = 0;
                                    else
                                        this._TabPrecedentePensione = 1;
                                    this._TabIstruttoria = null;
                                    this._TabDatiAssicurativi = 0;
                                    this._TabInail = 1;
                                    this._TabDatiLegge460 = null;

                                    if (controlTabRic != null && (controlTabRic.TabGenerici || controlTabRic.TabAssicurativi) && isStoricoVisible)
                                        this._TabStorico = 0;

                                    break;

                                case Utility.TipoFondo.GAS:
                                case Utility.TipoFondo.ES:
                                    this._Tipo = 2;
                                    this._TabDatiGenerici = 0;
                                    this._TabOpzione = null;
                                    if (isTabPrecedentePensioneAGO_FS.HasValue && isTabPrecedentePensioneAGO_FS.Value)
                                        this._TabPrecedentePensione = 0;
                                    else
                                        this._TabPrecedentePensione = 1;
                                    this._TabIstruttoria = null;
                                    this._TabDatiAssicurativi = 0;
                                    this._TabInail = 1;
                                    this._TabDatiLegge460 = null;
                                    break;

                                default:
                                    this._Tipo = 2;
                                    this._TabDatiGenerici = 0;
                                    this._TabOpzione = null;
                                    this._TabPrecedentePensione = 1;
                                    this._TabIstruttoria = null;
                                    this._TabDatiAssicurativi = 0;
                                    this._TabInail = 1;
                                    this._TabDatiLegge460 = null;
                                    break;
                            }

                            if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura))
                            {
                                if (controlTabRic != null)
                                {
                                    //Il tab Assicurativi non è visibile per le RIC non contributive ad esclusione di FS/PT/INPDAP
                                    if (controlTabRic.TabAssicurativi && (isRiapertura || (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || tipoFondo == Utility.TipoFondo.FS ||
                                         tipoFondo == Utility.TipoFondo.PT || Utility.IsDomandaINPDAP(datiPensione.Gestione))))
                                        this._TabDatiAssicurativi = 0;
                                    //Eng - Il tab Assicurativi è visibile per le Ricostituzioni non FS/PT/INPDAP con Prodotto "0109" e Tipo "0130"
                                    else if (tipoDomanda == Utility.TipoDomanda.Ricostituzione && datiPensione.Prodotto == "0109" && datiPensione.Tipo == "0130" && tipoFondo != Utility.TipoFondo.FS &&
                                         tipoFondo != Utility.TipoFondo.PT && !Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        this._TabDatiAssicurativi = 0;
                                    else
                                        this._TabDatiAssicurativi = null;

                                    if (controlTabRic.TabGenerici)
                                        this._TabDatiGenerici = 0;
                                    else
                                        this._TabDatiGenerici = null;
                                }
                                else if (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && tipoFondo != Utility.TipoFondo.FS &&
                                    tipoFondo != Utility.TipoFondo.PT && !Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                    this._TabDatiAssicurativi = null;

                                if ((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && Utility.IsRicostituzione_VariazioneDatiContitolari(datiPensione))
                                    this._TabDatiAssicurativi = null;
                            }

                            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                this._TabIstruttoria = null;

                            if (Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura)
                                this._TabDatiAssicurativi = 1;

                            if (Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                                this._TabPrecedentePensione = 1;

                            //ENG - 024/GDP - RIC CONCESSIONE DATI ALTRA PENSIONE
                            if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && (Utility.IsDomandaINPDAP(datiPensione.Gestione) || tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))
                                this._TabDatiAssicurativi = null;

                            break;
                        case Utility.TipoAppartenenza.CI:
                            //valori di default CI
                            this._Tipo = 2;
                            this._TabDatiGenerici = 0;
                            if (/*tipoDomanda == Utility.TipoDomanda.Ricostituzione ||*/ isRiapertura)
                                this._TabOpzione = null;
                            else
                            {
                                if ((!decorrenzaPensione.HasValue || (decorrenzaPensione.Value >= dataCompare && tipoDomanda != Utility.TipoDomanda.Ricostituzione)) && !(Utility.IsDomandaReversibilita(datiPensione)))
                                    this._TabOpzione = null;
                                else
                                    this._TabOpzione = 1;
                            }
                            this._TabPrecedentePensione = 1;
                            if (isTabIstruttoriaRequired.HasValue && isTabIstruttoriaRequired.Value)
                                this._TabIstruttoria = 0;
                            else
                                this._TabIstruttoria = 1;
                            this._TabDatiAssicurativi = 0;

                            //ENG - Reversibilità: gestire il tab Inail
                            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa) || Utility.IsDomandaPensioneInabilita(datiPensione) ||
                                Utility.IsAssegnoInvalidita(datiPensione) || isTabInailVisibleForCI)
                                this._TabInail = 1;
                            else
                                this._TabInail = null;

                            this._TabDatiLegge460 = null;
                            break;
                    }
                }
                else
                {
                    //valori di default FS
                    this._Tipo = 2;
                    this._TabDatiGenerici = 0;
                    this._TabOpzione = null;
                    this._TabPrecedentePensione = 1;
                    this._TabIstruttoria = 1;
                    this._TabDatiAssicurativi = 0;
                    this._TabInail = 1;
                    this._TabDatiLegge460 = null;
                }

                if (datiPensione.TipoAutomazione == (byte)Utility.TipoAutomazione.Vecchiaia && !Utility.IsDomandaPSO(datiPensione.SiglaCategoria))
                    this._TabDatiAssicurativi = 1;
            }

            public DatiQuadroLiquidazionePensione(byte? tipo, byte? tabDatiGenerici,
                byte? tabOpzione, byte? tabPrecedentePensione, byte? tabIstruttoria,
                byte? tabDatiAssicurativi, byte? tabInail, byte? tabLegge460, byte? tabStorico, byte? tabInteressiLegali, byte? tabSentenzaArt4, byte? tabSentenze)
            {
                this._Tipo = tipo;
                this._TabDatiGenerici = tabDatiGenerici;
                this._TabOpzione = tabOpzione;
                this._TabPrecedentePensione = tabPrecedentePensione;
                this._TabIstruttoria = tabIstruttoria;
                this._TabDatiAssicurativi = tabDatiAssicurativi;
                this._TabInail = tabInail;
                this._TabDatiLegge460 = tabLegge460;
                this._TabStorico = tabStorico;
                this._TabInteressiLegali = tabInteressiLegali;
                this._TabSentenzaArt4 = tabSentenzaArt4;
                this._TabSentenze = tabSentenze;
            }

            #region private properties

            private byte? _Tipo;

            private byte? _TabDatiGenerici;

            private byte? _TabOpzione;

            private byte? _TabPrecedentePensione;

            private byte? _TabIstruttoria;

            private byte? _TabDatiAssicurativi;

            private byte? _TabInail;

            private byte? _TabDatiLegge460;

            private byte? _TabContribuzioneEnpals;

            private byte? _TabStorico;

            private byte? _TabInteressiLegali;

            private byte? _TabSentenzaArt4;

            private byte? _TabSentenze;
            #endregion private properties

            #region public properties

            public byte? Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public byte? TabDatiGenerici { get { return _TabDatiGenerici; } set { _TabDatiGenerici = value; } }

            public byte? TabOpzione { get { return _TabOpzione; } set { _TabOpzione = value; } }

            public byte? TabPrecedentePensione { get { return _TabPrecedentePensione; } set { _TabPrecedentePensione = value; } }

            public byte? TabIstruttoria { get { return _TabIstruttoria; } set { _TabIstruttoria = value; } }

            public byte? TabDatiAssicurativi { get { return _TabDatiAssicurativi; } set { _TabDatiAssicurativi = value; } }

            public byte? TabInail { get { return _TabInail; } set { _TabInail = value; } }

            public byte? TabDatiLegge460 { get { return _TabDatiLegge460; } set { _TabDatiLegge460 = value; } }

            public byte? TabContribuzioneEnpals { get { return _TabContribuzioneEnpals; } set { _TabContribuzioneEnpals = value; } }

            public byte? TabStorico { get { return _TabStorico; } set { _TabStorico = value; } }

            public byte? TabInteressiLegali { get { return _TabInteressiLegali; } set { _TabInteressiLegali = value; } }

            public byte? TabSentenzaArt4 { get { return _TabSentenzaArt4; } set { _TabSentenzaArt4 = value; } }

            public byte? TabSentenze { get { return _TabSentenze; } set { _TabSentenze = value; } }
            #endregion public properties
        }

        public class DatiQuadroDelegatoTutore
        {
            public DatiQuadroDelegatoTutore()
            {
                //valori di default
                this._Tipo = 1;
                this._TabDelegato = 1;
                this._TabTutore = 1;
            }

            public DatiQuadroDelegatoTutore(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda, bool isRiapertura)
            {
                if (tipoAppartenenza.HasValue)
                {
                    switch (tipoAppartenenza.Value)
                    {
                        case Utility.TipoAppartenenza.FS:
                            GestioneControlliDinamici.ControlloDinamico controlloDinamicoReversibilita024 = null;
                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccoDeleghe024Reversibilita", out controlloDinamicoReversibilita024);
                            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                            if ((Utility.IsDomandaINPDAP(datiPensione.Gestione) && !(tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && Utility.AbilitaQuadroDelegheTuteleINPDAP())
                                || (controlloDinamicoReversibilita024 != null && !String.IsNullOrEmpty(controlloDinamicoReversibilita024.ValoreControllo) &&
                                controlloDinamicoReversibilita024.ValoreControllo.Trim().ToUpperInvariant() == "SI" && Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                            {
                                this._Tipo = 1;
                                this._TabDelegato = 1;
                                this._TabTutore = 1;
                            }
                            else
                            {
                                this._Tipo = 0;
                                this._TabDelegato = null;
                                this._TabTutore = null;
                            }
                            break;
                        case Utility.TipoAppartenenza.AGO:
                            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                            {
                                this._Tipo = 0;
                                this._TabDelegato = null;
                                this._TabTutore = null;
                            }
                            else
                            {
                                //valori di default
                                this._Tipo = 1;
                                this._TabDelegato = 1;
                                this._TabTutore = 1;
                            }
                            break;
                        default:
                            //valori di default
                            this._Tipo = 1;
                            this._TabDelegato = 1;
                            this._TabTutore = 1;
                            break;
                    }
                }
                else
                {
                    //valori di default
                    this._Tipo = 1;
                    this._TabDelegato = 1;
                    this._TabTutore = 1;
                }
            }

            public DatiQuadroDelegatoTutore(System.Nullable<byte> tipo, System.Nullable<byte> tabDelegato,
                System.Nullable<byte> tabTutore)
            {
                this._Tipo = tipo;
                this._TabDelegato = tabDelegato;
                this._TabTutore = tabTutore;
            }

            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabDelegato;

            private System.Nullable<byte> _TabTutore;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabDelegato { get { return _TabDelegato; } set { _TabDelegato = value; } }

            public System.Nullable<byte> TabTutore { get { return _TabTutore; } set { _TabTutore = value; } }
            #endregion public properties
        }

        public class DatiQuadroDatiContributivi
        {
            public DatiQuadroDatiContributivi()
            {
                //valori di default FS
                this._Tipo = 2;
                this._TabDatiCalcolo = 0;
            }

            public DatiQuadroDatiContributivi(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, Utility.TipoDomanda tipoDomanda,
                GestioneCtrlRic.ControlTabRic controlTabRic, bool isTabDatiPostDecOriginariaVisibleCI, DateTime? decorrenzaPensione, bool isRiapertura, bool isDomandaConNuovaGestioneDatiFondoFSPT,
                bool isStoricoVisible, bool isBeneficioVittimeTerrorismo, bool isDatiCalcoloPerRicRequired, bool isConvenzione13, bool? isQuotaFondoIntegrativoVisible, bool isQuoteMiglioramentiContrattualiVisible)
            {
                DateTime? decorrenzaAntePostArmonizzazione = Utility.GetDecorrenzaPensioneOrDecorrenzaDantecausa(datiPensione.DecorrenzaOriginaria, datiDanteCausa != null ? datiDanteCausa.DecorrenzaPensione : null);
                //ENG - MEMO 50/2023
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
                //ENG - MEMO 74_2023 
                GestioneControlliDinamici.ControlloDinamico ctrlMemo74_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo74_2023", out ctrlMemo74_2023);

                if (tipoAppartenenza != null)
                {
                    switch (tipoAppartenenza)
                    {
                        #region FS
                        case Utility.TipoAppartenenza.FS:

                            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                            Utility.CategoriaFondoPI? categoriaFondoPI = Utility.GetCategoriaFondoPI(tipoAppartenenza, datiPensione.SiglaCategoria);
                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.PI:
                                case Utility.TipoFondo.PL:
                                    if (categoriaFondoPI.HasValue && (categoriaFondoPI.Value == Utility.CategoriaFondoPI.U || categoriaFondoPI.Value == Utility.CategoriaFondoPI.V))
                                    {
                                        //valori di default FS
                                        if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                            !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._TabDatiCalcolo = 0;
                                        else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._TabDatiCalcolo = null;
                                        else
                                        {
                                            this._TabDatiCalcolo = 1;
                                        }

                                        this._TabProRata = null;
                                        this._TabContrEsteri = null;
                                        this._TabMaternAcna = null;
                                        this._TabLavAutonomi = null;
                                        this._TabDatiPostDecOriginaria = null;
                                        this._TabDatiCalcoloENPALS = null;
                                        this._TabAnte67 = null;
                                        this._TabSL33670 = null;
                                        this._TabDatiCalcoloINPDAI = null;
                                        this._TabDatiAgo = null;
                                        this._TabDatiFondo = null;

                                        if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) ||
                                            (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                            this._Tipo = 0;
                                        else if (IsTipoFacoltativo())
                                            this._Tipo = 1;
                                        else
                                            this._Tipo = 2;

                                    }
                                    else
                                    {
                                        //valori di default FS
                                        this._Tipo = 0;
                                        this._TabDatiCalcolo = null;
                                        this._TabProRata = null;
                                        this._TabContrEsteri = null;
                                        this._TabMaternAcna = null;
                                        this._TabLavAutonomi = null;
                                        this._TabDatiPostDecOriginaria = null;
                                        this._TabDatiCalcoloENPALS = null;
                                        this._TabAnte67 = null;
                                        this._TabSL33670 = null;
                                        this._TabDatiCalcoloINPDAI = null;
                                        this._TabDatiAgo = null;
                                        this._TabDatiFondo = null;
                                    }

                                    break;
                                case Utility.TipoFondo.DZ:  //Nuova Gestione Dazi Daniele
                                case Utility.TipoFondo.CL:
                                    //valori di default FS
                                    this._Tipo = 0;
                                    this._TabDatiCalcolo = null;
                                    this._TabProRata = null;
                                    this._TabContrEsteri = null;
                                    this._TabMaternAcna = null;
                                    this._TabLavAutonomi = null;
                                    this._TabDatiPostDecOriginaria = null;
                                    this._TabDatiCalcoloENPALS = null;
                                    this._TabAnte67 = null;
                                    this._TabSL33670 = null;
                                    this._TabDatiCalcoloINPDAI = null;

                                    break;
                                case Utility.TipoFondo.GAS:
                                    this._TabDatiFondo = 0;
                                    if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                        !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                    {
                                        if (decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, new DateTime(1998, 02, 01)))
                                            this._TabDatiAgo = 0;
                                        else
                                            this._TabDatiAgo = 1;
                                    }
                                    else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiAgo = null;
                                    else
                                        this._TabDatiAgo = 1;

                                    if (!Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabArt11e14 = 1;

                                    this._TabDatiCalcolo = null;
                                    this._TabProRata = null;
                                    this._TabContrEsteri = null;
                                    this._TabMaternAcna = null;
                                    this._TabLavAutonomi = null;
                                    this._TabDatiPostDecOriginaria = null;
                                    this._TabDatiCalcoloENPALS = null;
                                    this._TabAnte67 = null;
                                    this._TabSL33670 = null;
                                    this._TabDatiCalcoloINPDAI = null;

                                    if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) ||
                                        (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                        this._Tipo = 0;
                                    else if (IsTipoFacoltativo())
                                        this._Tipo = 1;
                                    else
                                        this._Tipo = 2;
                                    break;
                                case Utility.TipoFondo.ES:
                                    this._TabDatiFondo = 0;
                                    if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                        !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                    {
                                        if (decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, new DateTime(1998, 02, 01)))
                                            this._TabDatiAgo = 0;
                                        else
                                            this._TabDatiAgo = 1;
                                    }
                                    else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiAgo = null;
                                    else
                                        this._TabDatiAgo = 1;

                                    if (!Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabArt11e14 = 1;

                                    this._TabDatiCalcolo = null;
                                    this._TabProRata = null;
                                    this._TabContrEsteri = null;
                                    this._TabMaternAcna = null;
                                    this._TabLavAutonomi = null;
                                    this._TabDatiPostDecOriginaria = null;
                                    this._TabDatiCalcoloENPALS = null;
                                    this._TabAnte67 = 1;
                                    this._TabSL33670 = 1;
                                    this._TabDatiCalcoloINPDAI = null;

                                    if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) ||
                                        (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                        this._Tipo = 0;
                                    else if (IsTipoFacoltativo())
                                        this._Tipo = 1;
                                    else
                                        this._Tipo = 2;
                                    break;

                                case Utility.TipoFondo.PT:
                                case Utility.TipoFondo.FS:
                                    if (isDomandaConNuovaGestioneDatiFondoFSPT)
                                    {
                                        this._Tipo = 0;
                                        this._TabDatiCalcolo = null;
                                        this._TabProRata = null;
                                        this._TabContrEsteri = null;
                                        this._TabMaternAcna = null;
                                        this._TabLavAutonomi = null;
                                        this._TabDatiPostDecOriginaria = null;
                                        this._TabDatiCalcoloENPALS = null;
                                        this._TabAnte67 = null;
                                        this._TabSL33670 = null;
                                        this._TabDatiCalcoloINPDAI = null;
                                    }
                                    else
                                    {
                                        if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                            !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._TabDatiCalcolo = 0;
                                        else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._TabDatiCalcolo = null;
                                        else
                                            this._TabDatiCalcolo = 1;

                                        this._TabProRata = null;
                                        this._TabContrEsteri = null;
                                        this._TabMaternAcna = null;
                                        this._TabLavAutonomi = null;
                                        this._TabDatiPostDecOriginaria = null;
                                        this._TabDatiCalcoloENPALS = null;
                                        this._TabAnte67 = null;
                                        this._TabSL33670 = null;
                                        this._TabDatiCalcoloINPDAI = null;

                                        if (IsTipoFacoltativo() && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._Tipo = 1;
                                        else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this.Tipo = 0;
                                        else
                                            this._Tipo = 2;
                                    }
                                    break;
                                case Utility.TipoFondo.ET:
                                    //valori di default FS
                                    if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                        !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiCalcolo = 0;
                                    else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiCalcolo = null;
                                    else
                                        this._TabDatiCalcolo = 1;

                                    this._TabProRata = null;
                                    this._TabContrEsteri = null;
                                    this._TabMaternAcna = null;
                                    this._TabLavAutonomi = null;
                                    this._TabDatiPostDecOriginaria = null;
                                    this._TabDatiCalcoloENPALS = null;
                                    this._TabAnte67 = null;
                                    this._TabSL33670 = null;
                                    this._TabDatiCalcoloINPDAI = null;
                                    //Condizione visibilita tab DatiCalcolo\DatiAltraPensione - Dati Ago per ante-armonizzazione
                                    if (Utility.IsDomandaAnteArmonizzazione(datiPensione, tipoFondo, decorrenzaAntePostArmonizzazione) &&
                                        Utility.IsVisibleTabAltraPensioneDatiAgo(datiPensione, datiDanteCausa, datiPensione.DecorrenzaOriginaria, datiPensione.NaturaPensione) &&
                                        !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                    {
                                        if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                            this._TabDatiAgo = 0;
                                        else
                                            this._TabDatiAgo = 1;
                                    }
                                    else
                                        this._TabDatiAgo = null;

                                    if (controlTabRic != null && controlTabRic.TabCalcolo == true && isStoricoVisible && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabStorico = 0;

                                    if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) ||
                                        (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                        this._Tipo = 0;
                                    else if (IsTipoFacoltativo())
                                        this._Tipo = 1;
                                    else
                                        this._Tipo = 2;
                                    break;
                                case Utility.TipoFondo.EL:
                                case Utility.TipoFondo.TT:
                                case Utility.TipoFondo.VL:
                                    //valori di default FS
                                    if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                        !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiCalcolo = 0;
                                    else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiCalcolo = null;
                                    else
                                        this._TabDatiCalcolo = 1;

                                    this._TabProRata = null;
                                    this._TabContrEsteri = null;
                                    this._TabMaternAcna = null;
                                    this._TabLavAutonomi = null;
                                    this._TabDatiPostDecOriginaria = null;
                                    this._TabDatiCalcoloENPALS = null;
                                    this._TabAnte67 = null;
                                    this._TabSL33670 = null;
                                    this._TabDatiCalcoloINPDAI = null;

                                    if (controlTabRic != null && controlTabRic.TabCalcolo == true && isStoricoVisible && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabStorico = 0;

                                    if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) ||
                                        (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                        this._Tipo = 0;
                                    else if (IsTipoFacoltativo())
                                        this._Tipo = 1;
                                    else
                                        this._Tipo = 2;
                                    break;
                                default:

                                    //valori di default FS
                                    if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                        !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiCalcolo = 0;
                                    else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        this._TabDatiCalcolo = null;
                                    else
                                        this._TabDatiCalcolo = 1;

                                    this._TabProRata = null;
                                    this._TabContrEsteri = null;
                                    this._TabMaternAcna = null;
                                    this._TabLavAutonomi = null;
                                    this._TabDatiPostDecOriginaria = null;
                                    this._TabDatiCalcoloENPALS = null;
                                    this._TabAnte67 = null;
                                    this._TabSL33670 = null;
                                    this._TabDatiCalcoloINPDAI = null;

                                    if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && !Utility.IsDomandaINPDAP(datiPensione.Gestione)) ||
                                        (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                        this._Tipo = 0;
                                    else if (IsTipoFacoltativo())
                                        this._Tipo = 1;
                                    else
                                        this._Tipo = 2;
                                    break;
                            }
                            if (tipoFondo == Utility.TipoFondo.GAS)
                            {
                                if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && controlTabRic != null)
                                {
                                    if (controlTabRic.TabCalcolo)
                                    {
                                        this._TabDatiFondo = 0;
                                        if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                            !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                        {
                                            if (decorrenzaPensione.HasValue && Utility.DataStrettamenteSuccessivaA(decorrenzaPensione.Value, new DateTime(1998, 02, 01)))
                                                this._TabDatiAgo = 0;
                                            else
                                                this._TabDatiAgo = 1;
                                        }
                                        else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._TabDatiAgo = null;
                                        else
                                            this._TabDatiAgo = 1;

                                        if (!Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                            this._TabArt11e14 = 1;

                                        if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && !Utility.IsDomandaINPDAP(datiPensione.Gestione)) ||
                                            (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                            this._Tipo = 0;
                                        else if (IsTipoFacoltativo())
                                            this._Tipo = 1;
                                        else
                                            this._Tipo = 2;
                                    }
                                    else
                                    {
                                        this._Tipo = 0;
                                        this._TabDatiFondo = null;
                                        this._TabDatiAgo = null;
                                        this._TabArt11e14 = null;
                                    }
                                }
                            }
                            else
                            {
                                if ((tipoFondo != Utility.TipoFondo.PI || (categoriaFondoPI.HasValue &&
                                    (categoriaFondoPI.Value == Utility.CategoriaFondoPI.U || categoriaFondoPI.Value == Utility.CategoriaFondoPI.V))) && tipoFondo != Utility.TipoFondo.CL &&
                                    tipoFondo != Utility.TipoFondo.GAS && tipoFondo != Utility.TipoFondo.ES && (!isDomandaConNuovaGestioneDatiFondoFSPT || (tipoFondo != Utility.TipoFondo.PT && tipoFondo != Utility.TipoFondo.FS)))
                                {
                                    if ((tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura) && controlTabRic != null && !isDomandaConNuovaGestioneDatiFondoFSPT && tipoFondo != Utility.TipoFondo.DZ)
                                    {
                                        if (controlTabRic.TabCalcolo)
                                        {
                                            if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                                !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                                this._TabDatiCalcolo = 0;
                                            else if (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                                this._TabDatiCalcolo = null;
                                            else
                                                this._TabDatiCalcolo = 1;

                                            if ((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione)) ||
                                                (Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) || (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))))
                                                this._Tipo = 0;
                                            else if (IsTipoFacoltativo())
                                                this._Tipo = 1;
                                            else
                                                this._Tipo = 2;
                                        }
                                        else
                                        {
                                            this._Tipo = 0;
                                            this._TabDatiCalcolo = null;
                                        }
                                    }
                                }
                            }

                            if (Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura && !isDomandaConNuovaGestioneDatiFondoFSPT && tipoFondo != Utility.TipoFondo.CL && tipoFondo != Utility.TipoFondo.DZ && tipoFondo != Utility.TipoFondo.ES && //Nuova Gestione Dazi Daniele
                                !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                            {
                                this._Tipo = 1;
                                if (this._TabDatiCalcolo == 0)
                                    this._TabDatiCalcolo = 1;
                                if (this._TabDatiAgo == 0)
                                    this._TabDatiAgo = 1;
                            }

                            //Questa condizione controlla se siamo in presenza di una PrimaLiquidata, esisterebbe la funziona in Utility ma in questo punto genera problemi di lettura da Database.
                            //Per risolvere è sono stati ripetuti i controlli effettuati da IsPL usando il flag in input isRiapertura invece di invocare il Database.
                            bool flagDomandaPl = !(Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) || Utility.IsDomandaRipristinoOrRiliquidazione(datiPensione));

                            if (tipoFondo == Utility.TipoFondo.PI && flagDomandaPl && datiPensione.Gruppo == "0003" && datiPensione.SiglaCategoria.StartsWith("SPI"))
                            {
                                this.Tipo = 2;
                                this._TabDatiCalcolo = 0;
                            }

                            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                            {
                                this._Tipo = 0;
                                this._TabDatiCalcolo = null;
                                this._TabProRata = null;
                                this._TabContrEsteri = null;
                                this._TabMaternAcna = null;
                                this._TabLavAutonomi = null;
                                this._TabDatiPostDecOriginaria = null;
                                this._TabDatiCalcoloENPALS = null;
                                this._TabAnte67 = null;
                                this._TabSL33670 = null;
                                this._TabDatiCalcoloINPDAI = null;
                            }

                            //per le ric pi/pl mostro il quadro solo per per le ric contributive
                            if (tipoFondo.HasValue && 
                                (tipoFondo.Value == Utility.TipoFondo.PI || tipoFondo.Value == Utility.TipoFondo.PL)
                                && (!Utility.IsRicostituzione(datiPensione.Gruppo) ||
                                (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsRicostituzione_MotiviContributivi(datiPensione))))
                            {

                                this._TabDatiFondo = 1;

                                if (!(categoriaFondoPI.Value == Utility.CategoriaFondoPI.V || categoriaFondoPI.Value == Utility.CategoriaFondoPI.U))
                                    this._TabDatiAgo = 1;

                                this._Tipo = 1;
                                this._TabDatiCalcolo = null;
                            }

                            this._TabQuotePensione = null;
                            this._TabVittime = null;
                            break;
                        #endregion FS
                        #region CI
                        case Utility.TipoAppartenenza.CI:
                            //valori di default Ci
                            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                this._TabDatiCalcolo = 0;
                            else
                                this._TabDatiCalcolo = 1;

                            if (datiPensione.Gruppo.Equals("0003") && datiPensione.SiglaCategoria.StartsWith("SPI"))
                                this._TabDatiCalcolo = 2;

                            this._TabProRata = 0;
                            this._TabMaternAcna = 1;
                            this._TabLavAutonomi = null;
                            this._TabDatiCalcoloINPDAI = null;
                            this._TabVittime = null;

                            if (isTabDatiPostDecOriginariaVisibleCI)
                                this._TabDatiPostDecOriginaria = 1;
                            else
                                this._TabDatiPostDecOriginaria = null;

                            this._TabDatiCalcoloENPALS = null;

                            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti || isRiapertura)
                                this._TabContrEsteri = 1;

                            this._TabQuotePensione = null;

                            if (isConvenzione13)
                                this._TabIntegrazioneVirtuale = 1;
                            else
                                this._TabIntegrazioneVirtuale = 0;

                            if (IsTipoFacoltativo())
                                this._Tipo = 1;
                            else
                                this._Tipo = 2;

                            // ENG - Rendere opzionale pannello DatiCalcolo per ricostituzioni per supplemento della linea CI
                            if (Utility.IsRicostituzione_Supplemento(datiPensione))
                            {
                                this._Tipo = 1;
                                this._TabProRata = 1;
                                this._TabMaternAcna = 1;
                                this._TabContrEsteri = 1;
                                this._TabDatiCalcolo = 1;
                                this._TabIntegrazioneVirtuale = 1;
                            }
                            if (Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                                this._TabIntegrazioneVirtuale = 1;
                            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                                this._TabIntegrazioneVirtuale = 1;
                            break;
                        #endregion CI
                        #region AGO
                        case Utility.TipoAppartenenza.AGO:
                            //valori di default Ago
                            this._TabProRata = null;
                            this._TabContrEsteri = null;
                            this._TabMaternAcna = null;
                            this._TabLavAutonomi = 1;
                            this._TabDatiPostDecOriginaria = null;
                            this._TabDatiCalcolo = null;
                            this._TabDatiCalcoloENPALS = null;
                            this._TabDatiCalcoloINPDAI = null;
                            this._TabQuotePensione = null;
                            this._TabQuotaFondoIntegrativo = null;
                            this._TabQuotaFondoINPGI = null;
                            this._TabDatiEsteri = null;
                            this._TabMiglioramentiContrattuali = null;

                            if (isBeneficioVittimeTerrorismo)
                            {
                                if (Utility.IsRicostituzione(datiPensione.Gruppo))
                                    this._TabVittime = 1;
                                else
                                    this._TabVittime = 0;
                            }
                            else
                                this._TabVittime = null;
                            if (Utility.IsDomandaENPALS(datiPensione.Gestione))
                            {
                                if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)) &&
                                    !Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                                    this._TabDatiCalcoloENPALS = 0;
                                else
                                    this._TabDatiCalcoloENPALS = 1;
                            }
                            else if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria))
                            {
                                if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione) ||
                                    (isDatiCalcoloPerRicRequired && tipoDomanda == Utility.TipoDomanda.Ricostituzione)) &&
                                    !Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                                    this._TabDatiCalcoloINPDAI = 0;
                                else
                                    this._TabDatiCalcoloINPDAI = 1;
                            }
                            else if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) || Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria))
                            {
                                if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione) ||
                                    (isDatiCalcoloPerRicRequired && tipoDomanda == Utility.TipoDomanda.Ricostituzione)) &&
                                    !Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                                    this._TabQuotePensione = 0;
                                else
                                    this._TabQuotePensione = 1;
                            }
                            else
                            {
                                bool isRipristino = Utility.IsDomandaRipristino(datiPensione) != null ? (bool)Utility.IsDomandaRipristino(datiPensione) : false;
                                bool isRiliquidazione = Utility.IsDomandaRiliquidazione(datiPensione) != null ? (bool)Utility.IsDomandaRiliquidazione(datiPensione) : false;

                                if (isRipristino && !Utility.IsDomandaAutomatica(datiPensione))
                                    this._TabDatiCalcolo = 1;
                                else if ((tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione) || isRiliquidazione ||
                                    (isDatiCalcoloPerRicRequired && tipoDomanda == Utility.TipoDomanda.Ricostituzione) || (isRipristino && Utility.IsDomandaAutomatica(datiPensione)))
                                     && !Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) && !(Utility.IsDomandaReversibilita(datiPensione) && Utility.IsDomandaPMO(datiPensione.SiglaCategoria)) && !(Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione))
                                    && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa))
                                    this._TabDatiCalcolo = 0;
                                else
                                    this._TabDatiCalcolo = 1;
                            }

                            if (isStoricoVisible && !Utility.IsDomandaVESO92_L92(datiPensione))
                                this._TabStorico = 0;

                            if (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) || (Utility.IsDomandaIOPGI(datiPensione.SiglaCategoria) && !Utility.IsDomandaIOPGI_AGI(datiPensione))
                                || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa))
                            {
                                this._TabQuotaFondoINPGI = 0;

                                //ENG - RIC VOPGI CONTRIBUTIVE Tab Dati Calcolo obbligatorio 
                                if (Utility.IsDomandaVOPGI(datiPensione.SiglaCategoria) && Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                    this._TabDatiCalcolo = 0;

                            }

                            if (IsTipoFacoltativo())
                                this._Tipo = 1;
                            else
                                this._Tipo = 2;

                            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                            {
                                this._Tipo = 0;
                                SetAllNull();
                            }
                            if (Utility.IsDomandaSPED(datiPensione) || Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) || Utility.IsDomandaVOST(datiPensione.SiglaCategoria) || Utility.IsDomandaPSO(datiPensione.SiglaCategoria))
                            {
                                this._Tipo = 0;
                                SetAllNull();
                            }
                            //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                            if (Utility.IsDomandaAnticipataEsattoriali(datiPensione) || (isQuotaFondoIntegrativoVisible.GetValueOrDefault() && Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
                                this._TabQuotaFondoIntegrativo = 0;
                            //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
                            if (isQuotaFondoIntegrativoVisible.GetValueOrDefault() && !Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                                this._TabQuotaFondoIntegrativo = 1;

                            if (tipoDomanda == Utility.TipoDomanda.Ricostituzione && Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiapertura) != null)
                            {
                                this._Tipo = 1;
                                this._TabDatiCalcolo = 1;
                            }

                            //ENG - INPGI migrate
                            if ((((Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsDomandaRipristino(datiPensione).Value) && Utility.IsDomandaINPGI(datiPensione.SiglaCategoria)) ||
                                (Utility.IsDomandaSOPGI(datiPensione.SiglaCategoria) && Utility.IsDomandaReversibilita(datiPensione) && !isRiapertura)) && datiPensione.GP1AV91B == "2")
                                this._TabDatiCalcolo = null;

                            //ENG - MEMO 74_2023 
                            //ENG - Memo 116/2025
                            List<GestioneDatiEsteriCumulo.PensioneEsteraCumulo> listaPrestazioniEstere = null;
                            GestioneDatiEsteriCumulo.GetPrestazioniEstereCumuloByIdPensione(datiPensione.Id, out listaPrestazioniEstere);
                            if ((Utility.IsDomandaVOCUM(datiPensione.SiglaCategoria) && ctrlMemo74_2023 != null && ctrlMemo74_2023.ValoreControllo == "SI") ||
                                Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) ||
                                Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(datiPensione))
                            {
                                if (Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsRicostituzione_MotiviContributivi(datiPensione) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0)
                                    this._TabDatiEsteri = 1;
                                else if ((!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "V") ||
                                        (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0) ||
                                        Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione) || Utility.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSE(datiPensione) ||
                                        Utility.IsDomandaVOAUTVecchiaiaTipoContributivoFiltroGSE(datiPensione))
                                    this._TabDatiEsteri = 0;
                            }

                            if (isQuoteMiglioramentiContrattualiVisible)
                                this._TabMiglioramentiContrattuali = 0;

                            if (isStoricoVisible && Utility.IsDomandaINPGI(datiPensione))
                                this._TabQuotaFondoINPGIStorico = 0;
                            break;
                            #endregion AGO
                    }
                }
                else
                {
                    //valori di default FS
                    this._Tipo = 2;
                    this._TabDatiCalcolo = 0;
                }
            }

            public DatiQuadroDatiContributivi(System.Nullable<byte> tipo, System.Nullable<byte> tabDatiCalcolo, System.Nullable<byte> tabProRata,
                System.Nullable<byte> tabContrEsteri, System.Nullable<byte> tabMaternAcna, System.Nullable<byte> tabLavAutonomi, System.Nullable<byte> tabDatiPostDecOriginaria,
                System.Nullable<byte> tabDatiFondo, System.Nullable<byte> tabDatiAgo, System.Nullable<byte> tabArt11e14, System.Nullable<byte> tabDatiCalcoloENPALS, byte? tabAnte67, byte? tabSL33670,
                System.Nullable<byte> tabDatiCalcoloINPDAI, byte? tabQuotePensione, byte? tabVittime, byte? tabDatiCalcolo707, byte? tabStorico, byte? tabIntegrazioneVirtuale, byte? tabQuotaFondoIntegrativo,
                byte? tabQuotaFondoINPGI, byte? tabDatiEsteri, byte? tabQuotaFondoINPGIStorico)
            {
                this._Tipo = tipo;
                this._TabDatiCalcolo = tabDatiCalcolo;
                this._TabProRata = tabProRata;
                this._TabContrEsteri = tabContrEsteri;
                this._TabMaternAcna = tabMaternAcna;
                this._TabLavAutonomi = tabLavAutonomi;
                this._TabDatiPostDecOriginaria = tabDatiPostDecOriginaria;
                this._TabDatiFondo = tabDatiFondo;
                this._TabDatiAgo = tabDatiAgo;
                this._TabArt11e14 = tabArt11e14;
                this._TabDatiCalcoloENPALS = tabDatiCalcoloENPALS;
                this._TabAnte67 = tabAnte67;
                this._TabSL33670 = tabSL33670;
                this._TabDatiCalcoloINPDAI = tabDatiCalcoloINPDAI;
                this._TabQuotePensione = tabQuotePensione;
                this._TabVittime = tabVittime;
                this._TabDatiCalcolo707 = tabDatiCalcolo707;
                this._TabStorico = tabStorico;
                this._TabIntegrazioneVirtuale = TabIntegrazioneVirtuale;
                this._TabQuotaFondoIntegrativo = tabQuotaFondoIntegrativo;
                this._TabQuotaFondoINPGI = tabQuotaFondoINPGI;
                this._TabDatiEsteri = tabDatiEsteri;
                this._TabQuotaFondoINPGIStorico = tabQuotaFondoINPGIStorico;
            }

            // Se viene aggiunta una nuova tab, deve essere inserita la voce sul metodo SetAllNull
            #region private properties
            private byte? _Tipo;

            private byte? _TabDatiCalcolo;

            private byte? _TabProRata;

            private byte? _TabContrEsteri;

            private byte? _TabMaternAcna;

            private byte? _TabLavAutonomi;

            private byte? _TabDatiPostDecOriginaria;

            private byte? _TabDatiFondo;

            private byte? _TabDatiAgo;

            private byte? _TabArt11e14;

            private byte? _TabDatiCalcoloENPALS;

            private byte? _TabAnte67;

            private byte? _TabSL33670;

            private byte? _TabDatiCalcoloINPDAI;

            private byte? _TabQuotePensione;

            private byte? _TabVittime;

            private byte? _TabDatiCalcolo707;

            private byte? _TabStorico;

            private byte? _TabIntegrazioneVirtuale;

            private byte? _TabQuotaFondoIntegrativo;

            private byte? _TabQuotaFondoINPGI;

            private byte? _TabDatiEsteri;

            private byte? _TabMiglioramentiContrattuali;

            private byte? _TabQuotaFondoINPGIStorico;

            #endregion private properties

            #region public properties

            public byte? Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public byte? TabDatiCalcolo { get { return _TabDatiCalcolo; } set { _TabDatiCalcolo = value; } }

            public byte? TabProRata { get { return _TabProRata; } set { _TabProRata = value; } }

            public byte? TabContrEsteri { get { return _TabContrEsteri; } set { _TabContrEsteri = value; } }

            public byte? TabMaternAcna { get { return _TabMaternAcna; } set { _TabMaternAcna = value; } }

            public byte? TabLavAutonomi { get { return _TabLavAutonomi; } set { _TabLavAutonomi = value; } }

            public byte? TabDatiPostDecOriginaria { get { return _TabDatiPostDecOriginaria; } set { _TabDatiPostDecOriginaria = value; } }

            public byte? TabDatiFondo { get { return _TabDatiFondo; } set { _TabDatiFondo = value; } }

            public byte? TabDatiAgo { get { return _TabDatiAgo; } set { _TabDatiAgo = value; } }

            public byte? TabArt11e14 { get { return _TabArt11e14; } set { _TabArt11e14 = value; } }

            public byte? TabDatiCalcoloENPALS { get { return _TabDatiCalcoloENPALS; } set { _TabDatiCalcoloENPALS = value; } }

            public byte? TabAnte67 { get { return _TabAnte67; } set { _TabAnte67 = value; } }

            public byte? TabSL33670 { get { return _TabSL33670; } set { _TabSL33670 = value; } }

            public byte? TabDatiCalcoloINPDAI { get { return _TabDatiCalcoloINPDAI; } set { _TabDatiCalcoloINPDAI = value; } }

            public byte? TabQuotePensione { get { return _TabQuotePensione; } set { _TabQuotePensione = value; } }

            public byte? TabVittime { get { return _TabVittime; } set { _TabVittime = value; } }

            public byte? TabDatiCalcolo707 { get { return _TabDatiCalcolo707; } set { _TabDatiCalcolo707 = value; } }

            public byte? TabStorico { get { return _TabStorico; } set { _TabStorico = value; } }

            public byte? TabIntegrazioneVirtuale { get { return _TabIntegrazioneVirtuale; } set { _TabIntegrazioneVirtuale = value; } }

            public byte? TabQuotaFondoIntegrativo { get { return _TabQuotaFondoIntegrativo; } set { _TabQuotaFondoIntegrativo = value; } }

            public byte? TabQuotaFondoINPGI { get { return _TabQuotaFondoINPGI; } set { _TabQuotaFondoINPGI = value; } }

            public byte? TabDatiEsteri { get { return _TabDatiEsteri; } set { _TabDatiEsteri = value; } }

            public byte? TabMiglioramentiContrattuali { get { return _TabMiglioramentiContrattuali; } set { _TabMiglioramentiContrattuali = value; } }

            public byte? TabQuotaFondoINPGIStorico { get { return _TabQuotaFondoINPGIStorico; } set { _TabQuotaFondoINPGIStorico = value; } }
            #endregion public properties

            #region private methods
            private void SetAllNull()
            {
                this._TabAnte67 = null;
                this._TabArt11e14 = null;
                this._TabContrEsteri = null;
                this._TabDatiAgo = null;
                this._TabDatiCalcolo = null;
                this._TabDatiCalcoloENPALS = null;
                this._TabDatiCalcoloINPDAI = null;
                this._TabDatiFondo = null;
                this._TabDatiPostDecOriginaria = null;
                this._TabLavAutonomi = null;
                this._TabMaternAcna = null;
                this._TabProRata = null;
                this._TabQuotePensione = null;
                this._TabSL33670 = null;
                this._TabStorico = null;
                this._TabVittime = null;
                this._TabDatiCalcolo707 = null;
                this._TabQuotaFondoIntegrativo = null;
                this._TabQuotaFondoINPGI = null;
                this._TabDatiEsteri = null;
            }

            private bool IsTipoFacoltativo()
            {
                if ((!this._TabAnte67.HasValue || this._TabAnte67.Value == 1) &&
                    (!this._TabArt11e14.HasValue || this._TabArt11e14.Value == 1) &&
                    (!this._TabContrEsteri.HasValue || this._TabContrEsteri.Value == 1) &&
                    (!this._TabDatiAgo.HasValue || this._TabDatiAgo.Value == 1) &&
                    (!this._TabDatiCalcolo.HasValue || this._TabDatiCalcolo.Value == 1) &&
                    (!this._TabDatiCalcoloENPALS.HasValue || this._TabDatiCalcoloENPALS.Value == 1) &&
                    (!this._TabDatiCalcoloINPDAI.HasValue || this._TabDatiCalcoloINPDAI.Value == 1) &&
                    (!this._TabDatiFondo.HasValue || this._TabDatiFondo.Value == 1) &&
                    (!this._TabDatiPostDecOriginaria.HasValue || this._TabDatiPostDecOriginaria.Value == 1) &&
                    (!this._TabLavAutonomi.HasValue || this._TabLavAutonomi.Value == 1) &&
                    (!this._TabMaternAcna.HasValue || this._TabMaternAcna.Value == 1) &&
                    (!this._TabProRata.HasValue || this._TabProRata.Value == 1) &&
                    (!this._TabQuotePensione.HasValue || this._TabQuotePensione.Value == 1) &&
                    (!this._TabSL33670.HasValue || this._TabSL33670.Value == 1) &&
                    (!this._TabVittime.HasValue || this._TabVittime.Value == 1) &&
                    (!this._TabDatiCalcolo707.HasValue || this._TabDatiCalcolo707.Value == 1) &&
                    (!this._TabIntegrazioneVirtuale.HasValue || this.TabIntegrazioneVirtuale.Value == 1) &&
                    (!this._TabQuotaFondoIntegrativo.HasValue || this._TabQuotaFondoIntegrativo.Value == 1) &&
                    (!this._TabQuotaFondoINPGI.HasValue || this._TabQuotaFondoINPGI.Value == 1) &&
                    (!this._TabDatiEsteri.HasValue || this._TabDatiEsteri.Value == 1) &&
                    (!this._TabMiglioramentiContrattuali.HasValue || this._TabMiglioramentiContrattuali.Value == 1)
                    )
                    return true;

                return false;
            }
            #endregion private methods
        }

        public class DatiQuadroRedditi
        {
            public DatiQuadroRedditi()
            {
                //valori di default
                this._Tipo = 2;
                this._TabRedditi = 0;
            }
            public DatiQuadroRedditi(GestionePensione.DatiPensione datiPensione, bool isRiapertura, Utility.TipoAppartenenza? tipoAppartenenza, bool isTabRedditiOpzionale)
            {
                if (isTabRedditiOpzionale)
                {
                    this._Tipo = 1;
                    this._TabRedditi = 1;
                }
                else if (Utility.IsDomandaVESO33(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria) ||
                         Utility.IsDomandaESOTEL(datiPensione.SiglaCategoria) || Utility.IsDomandaESOAMB(datiPensione.SiglaCategoria) ||
                         (Utility.IsDomandaAUT(datiPensione) && datiPensione.GetFiltro().ToUpperInvariant().Equals("ERI")) || Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) ||
                         Utility.IsDomandaESPA(datiPensione.SiglaCategoria) || Utility.IsDomandaAGOTipoContributivoFiltroERI(datiPensione) ||
                         Utility.IsRenditaFacoltativa(datiPensione) || Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsDomandaESOPMI(datiPensione.SiglaCategoria) ||
                         Utility.IsDomandaCOOP28_DAP(datiPensione))
                {
                    this._Tipo = 0;
                    this._TabRedditi = null;
                }
                else if (Utility.IsRicostituzione_Reddituale(datiPensione) || ((tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || Utility.IsDomandaINPDAP(datiPensione.Gestione)) && Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione)))
                {
                    this._Tipo = 2;
                    this._TabRedditi = 0;
                }
                else if ((Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO)
                    || (Utility.IsDomandaINPDAP(datiPensione.Gestione) && Utility.IsRicostituzione(datiPensione.Gruppo) && !(Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione)))
                    || (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI && Utility.IsRicostituzione_Supplemento(datiPensione))) //ENG - Modifica Supplementi CI Memo 177/2012
                {
                    this._Tipo = 1;
                    this._TabRedditi = 1;
                }
                else
                {
                    this._Tipo = 2;
                    this._TabRedditi = 0;
                }

                if (datiPensione.TipoAutomazione == (byte)Utility.TipoAutomazione.Vecchiaia)
                {
                    this._Tipo = 1;
                    this._TabRedditi = 1;
                }

                //ENG - GDP - RIC Concessione Altra Pensione
                if (Utility.IsRicostituzioneConcessioneAltraPensione(datiPensione) && Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    this._Tipo = 1;
                    this._TabRedditi = 1;
                }
            }

            public DatiQuadroRedditi(System.Nullable<byte> tipo, System.Nullable<byte> tabRedditi)
            {
                this._Tipo = tipo;
                this._TabRedditi = tabRedditi;

            }
            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabRedditi;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabRedditi { get { return _TabRedditi; } set { _TabRedditi = value; } }
            #endregion public properties
        }

        public class DatiQuadroFamiliari
        {
            public DatiQuadroFamiliari()
            {
                //valori di default
                this._Tipo = 1;
                this._TabFamiliari = 1;
            }
            public DatiQuadroFamiliari(System.Nullable<byte> tipo, System.Nullable<byte> tabFamiliari)
            {
                this._Tipo = tipo;
                this._TabFamiliari = tabFamiliari;

            }

            public DatiQuadroFamiliari(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda,
                bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, bool isFamiliariVerde, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);

                if (isFamiliariVerde)
                {
                    this._Tipo = 1;
                    this._TabFamiliari = 2;
                }
                else if ((datiPensione.Gruppo.Equals("0003") && !Utility.IsDomandaSpacchettamentoENPALS(datiPensione) && !Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) && !Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, datiDanteCausa)
                    && !(controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)) && !Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                    && !Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) && !Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda)))
                {
                    this._Tipo = 2;
                    this._TabFamiliari = 0;
                }
                else
                {
                    this._Tipo = 1;
                    this._TabFamiliari = 1;
                }
            }

            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabFamiliari;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabFamiliari { get { return _TabFamiliari; } set { _TabFamiliari = value; } }
            #endregion public properties
        }

        public class DatiQuadroDanteCausa
        {
            public DatiQuadroDanteCausa()
            {
                //valori di default
                this._Tipo = 0;
                this._TabAnagrafica = null;
                this._TabPensioneDiretta = null;
                this._TabAltraPensione = null;
                this._TabDatiPensioneCI = null;
                this._TabSentenza49593 = null;
            }
            public DatiQuadroDanteCausa(System.Nullable<byte> tipo, System.Nullable<byte> tabAnagrafica, System.Nullable<byte> tabPensioneDiretta, System.Nullable<byte> tabAltraPensione, System.Nullable<byte> tabDatiPensioneCI, byte? tabSentenza49593)
            {
                this._Tipo = tipo;
                this._TabAnagrafica = tabAnagrafica;
                this._TabPensioneDiretta = tabPensioneDiretta;
                this._TabAltraPensione = tabAltraPensione;
                this._TabDatiPensioneCI = tabDatiPensioneCI;
                this._TabSentenza49593 = tabSentenza49593;

            }
            #region private properties
            private System.Nullable<byte> _Tipo;

            private System.Nullable<byte> _TabAnagrafica;

            private System.Nullable<byte> _TabPensioneDiretta;

            private System.Nullable<byte> _TabAltraPensione;

            private System.Nullable<byte> _TabDatiPensioneCI;

            private System.Nullable<byte> _TabSentenza49593;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public System.Nullable<byte> TabAnagrafica { get { return _TabAnagrafica; } set { _TabAnagrafica = value; } }

            public System.Nullable<byte> TabPensioneDiretta { get { return _TabPensioneDiretta; } set { _TabPensioneDiretta = value; } }

            public System.Nullable<byte> TabAltraPensione { get { return _TabAltraPensione; } set { _TabAltraPensione = value; } }

            public System.Nullable<byte> TabDatiPensioneCI { get { return _TabDatiPensioneCI; } set { _TabDatiPensioneCI = value; } }

            public System.Nullable<byte> TabSentenza49593 { get { return _TabSentenza49593; } set { _TabSentenza49593 = value; } }

            #endregion public properties
        }

        public class DatiQuadroMaggiorazioniBenefici
        {
            public DatiQuadroMaggiorazioniBenefici()
            {
                this._Tipo = 0;
                this._TabExCombattente = null;
                this._TabBenefici = null;
                this._TabLegge407 = null;
                this._TabPrivilegiate = null;
                this._TabArticolo2 = null;
                this._TabMaggiorazioni = null;
                this._TabBeneficioVittimeTerrorismo = null;

            }

            public DatiQuadroMaggiorazioniBenefici(Utility.TipoAppartenenza? tipoAppartenenza, bool? isTabBeneficiVisible, bool? isTabExCombattenteVisible,
                bool? isTabPrivilegiateVisible, bool? isTabMaggiorazioniVisible, GestionePensione.DatiPensione datiPensione, bool? isTabDl407Visibile,
                bool isRiapertura, bool isBeneficioVittimeTerrorismo, bool? isDecorrenzaMaggiorazioneFromPrelievo, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
            {
                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                        switch (tipoFondo)
                        {
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:

                                this._Tipo = 0;

                                this._TabExCombattente = null;
                                this._TabBenefici = null;
                                this._TabLegge407 = null;
                                this._TabMaggiorazioni = null;
                                this._TabBeneficioVittimeTerrorismo = null;

                                break;
                            case Utility.TipoFondo.FS:
                            case Utility.TipoFondo.PT:
                                if ((isTabPrivilegiateVisible.HasValue && isTabPrivilegiateVisible.Value) ||
                                    (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value))
                                    this._Tipo = 1;
                                else
                                    this._Tipo = 0;

                                if (isTabPrivilegiateVisible.HasValue && isTabPrivilegiateVisible.Value)
                                    this._TabPrivilegiate = 0;
                                else
                                    this._TabPrivilegiate = null;

                                if (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value)
                                    this._TabExCombattente = 0;
                                else
                                    this._TabExCombattente = null;

                                this._TabBeneficioVittimeTerrorismo = null;

                                break;
                            default:
                                if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaQuota100(datiPensione) ||
                                    Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                                    (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                                    this._Tipo = 2;
                                else if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) ||
                                    (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value) ||
                                    isTabDl407Visibile.GetValueOrDefault())
                                    this._Tipo = 1;
                                else
                                    this._Tipo = 0;

                                if (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value)
                                    this._TabExCombattente = 0;
                                else
                                    this._TabExCombattente = null;

                                if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaQuota100(datiPensione) ||
                                    Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                                    (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                                    this._TabBenefici = 2;
                                else if (isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value)
                                {
                                    if (isDecorrenzaMaggiorazioneFromPrelievo.GetValueOrDefault())
                                        this._TabBenefici = 1;
                                    else
                                        this._TabBenefici = 0;
                                }
                                else
                                    this._TabBenefici = null;

                                if (isTabDl407Visibile.GetValueOrDefault())
                                    this._TabLegge407 = 0;
                                else
                                    this._TabLegge407 = null;

                                this._TabMaggiorazioni = null;
                                this._TabBeneficioVittimeTerrorismo = null;
                                break;
                        }
                        break;

                    case Utility.TipoAppartenenza.AGO:
                        if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaQuota100(datiPensione) ||
                            Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                            (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                            Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)))
                            this._Tipo = 2;
                        else if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) ||
                            (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value) || (isTabMaggiorazioniVisible.HasValue && isTabMaggiorazioniVisible.Value) ||
                            isBeneficioVittimeTerrorismo || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaCOOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaCRED27(datiPensione.SiglaCategoria))))
                            this._Tipo = 1;
                        else
                            this._Tipo = 0;


                        if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaQuota100(datiPensione) ||
                            Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                            (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) ||
                            Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))))
                            this._TabBenefici = 2;
                        else if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) && ((Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaCOOP28(datiPensione.SiglaCategoria) || Utility.IsDomandaCRED27(datiPensione.SiglaCategoria) || Utility.IsDomandaVESO92(datiPensione.SiglaCategoria))) || Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault()))
                            this._TabBenefici = 1;
                        else if (isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value)
                            this._TabBenefici = 0;
                        else
                            this._TabBenefici = null;

                        if (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value)
                            this._TabExCombattente = 0;
                        else
                            this._TabExCombattente = null;

                        if (isTabMaggiorazioniVisible.HasValue && isTabMaggiorazioniVisible.Value)
                            this._TabMaggiorazioni = 0;
                        else
                            this._TabMaggiorazioni = null;

                        if (isBeneficioVittimeTerrorismo || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
                            this._TabBeneficioVittimeTerrorismo = 0;

                        this._TabLegge407 = null;

                        if (Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiapertura) != null && this._Tipo != 0)
                        {
                            this._Tipo = 1;
                            if (this._TabBenefici == 0) this._TabBenefici = 1;
                            if (this._TabMaggiorazioni == 0) this._TabMaggiorazioni = 1;
                            if (this._TabExCombattente == 0) this._TabExCombattente = 1;
                        }

                        break;
                    case Utility.TipoAppartenenza.CI:
                        if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) &&
                            (Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                            (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                            this._Tipo = 2;
                        else if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) ||
                            (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value) || (isTabMaggiorazioniVisible.HasValue && isTabMaggiorazioniVisible.Value))
                            this._Tipo = 1;
                        else
                            this._Tipo = 0;



                        if ((isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) && (Utility.IsDomandaQuota100(datiPensione) ||
                            Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                            (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)))))
                            this._TabBenefici = 2;
                        else if (isTabBeneficiVisible.HasValue && isTabBeneficiVisible.Value)
                            this._TabBenefici = 0;
                        else
                            this._TabBenefici = null;

                        if (isTabExCombattenteVisible.HasValue && isTabExCombattenteVisible.Value)
                            this._TabExCombattente = 0;
                        else
                            this._TabExCombattente = null;

                        if (isTabMaggiorazioniVisible.HasValue && isTabMaggiorazioniVisible.Value)
                            this._TabMaggiorazioni = 0;
                        else
                            this._TabMaggiorazioni = null;

                        this._TabLegge407 = null;
                        this._TabBeneficioVittimeTerrorismo = null;
                        break;
                    default:


                        this._TabExCombattente = null;
                        this._TabBenefici = null;
                        this._TabLegge407 = null;
                        this._TabMaggiorazioni = null;
                        this._TabBeneficioVittimeTerrorismo = null;

                        break;
                }

                if (Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione))
                    this._TabBeneficioVittimeTerrorismo = 2;
            }

            public DatiQuadroMaggiorazioniBenefici(byte? tipo, byte? tabExCombattente, byte? tabBenefici, byte? tabDL407, byte? tabOneri, byte? tabPrivilegiate, byte? tabArticolo2,
                byte? tabMaggiorazioni, byte? tabPrepensionamento, byte? tabBeneficioVittimeTerrorismo)
            {
                this._Tipo = tipo;
                this._TabExCombattente = tabExCombattente;
                this._TabBenefici = tabBenefici;
                this._TabLegge407 = tabDL407;
                this._TabPrivilegiate = tabPrivilegiate;
                this._TabArticolo2 = tabArticolo2;
                this._TabMaggiorazioni = tabMaggiorazioni;
                this._TabBeneficioVittimeTerrorismo = tabBeneficioVittimeTerrorismo;
            }

            #region private properties

            private byte? _Tipo;
            private byte? _TabExCombattente;
            private byte? _TabBenefici;
            private byte? _TabLegge407;
            private byte? _TabPrivilegiate;
            private byte? _TabArticolo2;
            private byte? _TabMaggiorazioni;
            private byte? _TabBeneficioVittimeTerrorismo;

            #endregion private properties

            #region public properties

            public byte? Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public byte? TabExCombattente { get { return _TabExCombattente; } set { _TabExCombattente = value; } }

            public byte? TabBenefici { get { return _TabBenefici; } set { _TabBenefici = value; } }

            public byte? TabLegge407 { get { return _TabLegge407; } set { _TabLegge407 = value; } }

            public byte? TabPrivilegiate { get { return _TabPrivilegiate; } set { _TabPrivilegiate = value; } }

            public byte? TabArticolo2 { get { return _TabArticolo2; } set { _TabArticolo2 = value; } }

            public byte? TabMaggiorazioni { get { return _TabMaggiorazioni; } set { _TabMaggiorazioni = value; } }

            public byte? TabBeneficioVittimeTerrorismo { get { return _TabBeneficioVittimeTerrorismo; } set { _TabBeneficioVittimeTerrorismo = value; } }

            #endregion public properties
        }

        public class DatiQuadroSupplementi
        {
            public DatiQuadroSupplementi()
            {
                //valori di default
                this._Tipo = 1;
                this._TabSupplementi = 1;
                this._TabIntegrazioneArt11 = null;
                //ENG - Memo 32_a/2018
                this._TabStoricoSupplementi = null;
            }

            public DatiQuadroSupplementi(Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, bool isSupplementiPerRic, bool isBancRicTrf, bool isSupplementiPerENPALS,
                bool isSupplementiPerRev, bool isSupplementiPerTotalIVS, GestioneCtrlRic.ControlTabRic controlTabRic, bool isRiapertura, bool isContribuzioneENPALSPerSupp,
                GestionePensione.DatiPensione datiPensione, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
            {
                Utility.TipoQuadro? tipoQuadro = Utility.GetVisibilitaQuadroSupplementi(datiPensione, datiPensione.NaturaPensione, isRiapertura, datiDanteCausa);
                //ENG - MEMO 50/2023
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:

                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                        switch (tipoFondo)
                        {
                            case Utility.TipoFondo.PI:
                            case Utility.TipoFondo.PL:
                            case Utility.TipoFondo.CL:
                                this._Tipo = 0;
                                this._TabSupplementi = null;
                                this._TabIntegrazioneArt11 = null;
                                break;
                            default:
                                //valori di default
                                if (tipoQuadro != Utility.TipoQuadro.NonVisibile)
                                {
                                    this._Tipo = 1;
                                    this._TabSupplementi = 1;
                                    this._TabIntegrazioneArt11 = null;
                                }
                                else
                                {
                                    this._Tipo = 0;
                                    this._TabSupplementi = null;
                                    this._TabIntegrazioneArt11 = null;
                                }
                                break;
                        }
                        //ENG - MEMO 50/2023
                        if (((ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione))
                            || Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione)) || Utility.isDomandaRicperRiliquidazioneEtaPensionabile(datiPensione)
                            && !Utility.IsDomandaINPDAP(datiPensione.Gestione))
                        {
                            this._Tipo = 2;
                            this.TabSupplementi = 0;
                        }
                        else if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiapertura)
                        {
                            if (isSupplementiPerRic || (controlTabRic != null && controlTabRic.TabSupplementi))
                            {
                                this._Tipo = 1;
                                this.TabSupplementi = 1;
                            }
                            else
                            {
                                this._Tipo = 0;
                                this._TabSupplementi = null;
                            }
                        }
                        else if (Utility.IsDomandaReversibilita(datiPensione))
                        {
                            if (isSupplementiPerRev)
                            {
                                this._Tipo = 1;
                                this.TabSupplementi = 1;
                            }
                            else
                            {
                                this._Tipo = 0;
                                this._TabSupplementi = null;
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        //ENG - Memo 32_a/2018
                        this.TabStoricoSupplementi = null;
                        //Per domande APE Sociale non dovrà essere visibile né gestito il pannello “Supplementi”
                        if (Utility.IsDomandaAPESociale(datiPensione.SiglaCategoria) || Utility.IsDomandaSPED(datiPensione) ||
                        Utility.IsDomandaINDCOM(datiPensione.SiglaCategoria) || Utility.IsDomandaESPA(datiPensione.SiglaCategoria) || Utility.IsDomandaIobancInabilita(datiPensione))
                        {
                            this._Tipo = 0;
                            this._TabSupplementi = null;
                            this._TabIntegrazioneArt11 = null;
                        }
                        else
                        {
                            //valori di default
                            if (tipoQuadro == Utility.TipoQuadro.Obbligatorio)
                            {
                                this._Tipo = 2;
                                this._TabSupplementi = 0;
                                this._TabIntegrazioneArt11 = null;
                            }
                            else if (tipoQuadro == Utility.TipoQuadro.NonVisibile)
                            {
                                this._Tipo = 0;
                                this._TabSupplementi = null;
                                this._TabIntegrazioneArt11 = null;
                            }
                            else
                            {
                                this._Tipo = 1;
                                this._TabSupplementi = 1;
                                this._TabIntegrazioneArt11 = null;
                            }

                            if ((Utility.IsRicostituzione_Reddituale(datiPensione) && isSupplementiPerRic) || Utility.IsPannelloSupplementiAnte96(datiPensione, datiPensione, datiDanteCausa, isRiapertura))
                            {
                                this._Tipo = 1;
                                this._TabSupplementi = 1;
                            }
                            // se sono presenti i dati per enpals oppure i dati per le ricostituzioni dobbiamo rendere rosso il quadro
                            else if ((isSupplementiPerENPALS || isSupplementiPerRic || isSupplementiPerTotalIVS) && !isBancRicTrf)
                            {
                                this._Tipo = 2;
                                this._TabSupplementi = 0;
                            }

                            if (isContribuzioneENPALSPerSupp)
                            {
                                this._Tipo = 2;
                                this.TabContribuzioneEnpals = 0;
                            }
                        }
                        //ENG - MEMO 50/2023
                        if (ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione) && !Utility.IsDomandaENPALS(datiPensione.Gestione) && !Utility.IsDomandaCumulo(datiPensione.SiglaCategoria))
                        {
                            this._Tipo = 2;
                            this.TabSupplementi = 0;
                        }

                        //ENG Memo 32_a/2018
                        if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(datiPensione))
                        {
                            this._Tipo = 2;
                            this.TabSupplementi = 0;
                            this.TabStoricoSupplementi = 0;
                        }

                        if (Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiapertura) != null && this._Tipo == 2)
                        {
                            this._Tipo = 1;
                            this._TabSupplementi = 1;
                        }
                        break;

                    case Utility.TipoAppartenenza.CI:

                        //valori di default
                        if (tipoQuadro == Utility.TipoQuadro.Facoltativo)
                        {
                            this._Tipo = 1;
                            this._TabSupplementi = 1;
                            this._TabIntegrazioneArt11 = null;
                        }
                        else
                        {
                            this._Tipo = 0;
                            this._TabSupplementi = null;
                            this._TabIntegrazioneArt11 = null;
                        }
                        break;
                }
            }

            public DatiQuadroSupplementi(byte? tipo, byte? tabSupplementi, byte? tabIntegrazioneArt11)
            {
                this._Tipo = tipo;
                this._TabSupplementi = tabSupplementi;
                this._TabIntegrazioneArt11 = tabIntegrazioneArt11;
            }

            #region private properties

            private byte? _Tipo;
            private byte? _TabSupplementi;
            private byte? _TabIntegrazioneArt11;
            private byte? _TabContribuzioneEnpals;
            private byte? _TabStoricoSupplementi;

            #endregion private properties

            #region public properties

            public byte? Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public byte? TabSupplementi { get { return _TabSupplementi; } set { _TabSupplementi = value; } }

            public byte? TabIntegrazioneArt11 { get { return _TabIntegrazioneArt11; } set { _TabIntegrazioneArt11 = value; } }

            public byte? TabContribuzioneEnpals { get { return _TabContribuzioneEnpals; } set { _TabContribuzioneEnpals = value; } }

            //ENG - Memo 32_a/2018
            public byte? TabStoricoSupplementi { get { return _TabStoricoSupplementi; } set { _TabStoricoSupplementi = value; } }

            #endregion public properties
        }

        public class DatiQuadroBititolarita
        {
            public DatiQuadroBititolarita()
            {
            }

            public DatiQuadroBititolarita(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isRiapertura, bool? isBititolaritaVisible, GestioneDanteCausa.DatiDanteCausa datiDanteCausa)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.AGO:
                        if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.NaturaPensione != null && char.Parse(datiPensione.NaturaPensione.PadRight(3, ' ').Substring(0, 1).ToUpperInvariant()) == '5' && (Utility.IsRenditaCasalinghe(datiPensione) || Utility.IsRenditaFacoltativa(datiPensione)))
                        {
                            this._Tipo = 1;
                            this._TabAltrePensioni = 1;
                        }
                        else if ((Utility.IsRicostituzione_Reddituale(datiPensione) && isBititolaritaVisible.GetValueOrDefault()))
                        {
                            this._Tipo = 1;
                            this._TabAltrePensioni = 1;
                        }
                        else if ((datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && (datiPensione.Tipo == "0009" || datiPensione.Tipo == "0192")) ||
                            (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0013" && datiPensione.Tipo == "0009") ||
                            (datiPensione.Gruppo == "0003" && datiPensione.Prodotto == "0022" && datiPensione.Tipo == "0009") || // pensione supplementare
                            (isBititolaritaVisible.GetValueOrDefault() && (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiapertura)))
                        {
                            this._Tipo = 2;
                            this._TabAltrePensioni = 0;
                        }
                        else
                        {
                            this._Tipo = 0;
                            this._TabAltrePensioni = null;
                        }

                        if (Utility.IsDomandaAnte96(datiPensione, datiPensione, datiDanteCausa, isRiapertura) != null && this._Tipo == 2)
                        {
                            this._Tipo = 1;
                            this._TabAltrePensioni = 1;
                        }
                        break;

                    case Utility.TipoAppartenenza.CI:
                        if (isBititolaritaVisible == true && (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiapertura))
                        {
                            this._Tipo = 2;
                            this._TabAltrePensioni = 0;
                        }
                        else
                        {
                            this._Tipo = 0;
                            this._TabAltrePensioni = null;
                        }
                        break;
                    default:
                        this._Tipo = 0;
                        this._TabAltrePensioni = null;
                        break;
                }
            }

            #region private properties
            private byte? _Tipo;
            private byte? _TabAltrePensioni;
            #endregion private properties

            #region public properties
            public byte? Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public byte? TabAltrePensioni { get { return _TabAltrePensioni; } set { _TabAltrePensioni = value; } }

            #endregion public properties
        }

        public class DatiQuadroEliminazione
        {
            #region public properties

            public System.Nullable<byte> Tipo { get; set; }

            public System.Nullable<byte> TabEliminazione { get; set; }

            #endregion public properties

            public DatiQuadroEliminazione()
            { }

            public DatiQuadroEliminazione(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, DateTime? dataMorte, Utility.TipoDomanda tipoDomanda,
                bool isRiaperturaDomanda, bool isEliminazioneRequired, bool tabEliminazioneGialloAutomazione)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        this.Tipo = 0;
                        this.TabEliminazione = null;
                        break;
                    case Utility.TipoAppartenenza.CI:
                    case Utility.TipoAppartenenza.AGO:
                        if ((dataMorte.HasValue && dataMorte.Value >= datiPensione.DecorrenzaOriginaria) || Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione))
                        {
                            if (tabEliminazioneGialloAutomazione)
                            {
                                this.Tipo = 1;
                                this.TabEliminazione = 1;
                            }
                            else
                            {
                                this.Tipo = 2;
                                this.TabEliminazione = 0;
                            }
                        }
                        else
                        {
                            this.Tipo = 1;
                            this.TabEliminazione = 1;
                        }

                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isEliminazioneRequired)
                            {
                                this.Tipo = 2;
                                this.TabEliminazione = 0;
                            }
                            else
                            {
                                this.Tipo = 0;
                                this.TabEliminazione = null;
                            }
                        }
                        break;
                }

            }
        }

        public class DatiQuadroOneri
        {
            #region public properties
            public System.Nullable<byte> Tipo { get; set; }
            public System.Nullable<byte> TabOneri { get; set; }
            public System.Nullable<byte> TabPrepensionamento { get; set; }
            public byte? TabStorico { get; set; }
            #endregion public properties

            public DatiQuadroOneri()
            { }

            public DatiQuadroOneri(Utility.TipoAppartenenza? tipoAppartenenza, bool? IsVisibleForOneri, bool? isPrepVisible, Utility.TipoDomanda tipoDomanda, bool isRiapertura, bool isStoricoVisible,
                GestionePensione.DatiPensione datiPensione, bool isOneriRicPrepensionamentoTipo0162NotVisible)
            {
                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out ctrlMemo123_2024);

                //ENG - Memo 123/2024
                GestioneControlliDinamici.ControlloDinamico ctrlMemo123_2024OpzioneContrib = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out ctrlMemo123_2024OpzioneContrib);

                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        if (IsVisibleForOneri.HasValue && IsVisibleForOneri.Value)
                        {
                            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) || Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault())
                            {
                                this.Tipo = 1;
                                this.TabOneri = 1;
                            }
                            else if (datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002"))
                            {
                                //set quadro
                                this.Tipo = 2;
                                //set tab
                                this.TabOneri = 0;
                            }
                            else
                            {
                                this.Tipo = 0;
                                this.TabOneri = null;
                            }

                            if (Utility.IsDomandaInabilitaAmianto(datiPensione) && !(Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) || Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault()))
                            {
                                this.Tipo = 2;
                                this.TabOneri = 0;
                            }
                        }
                        else
                        {
                            this.Tipo = 0;
                            this.TabOneri = null;
                        }

                        if (this.Tipo != 0 && isStoricoVisible)
                            this.TabStorico = 0;
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        //set quadro
                        if ((tipoDomanda == Utility.TipoDomanda.Ripristino && IsVisibleForOneri.HasValue && IsVisibleForOneri.Value) || (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) &&
                            (Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || (Utility.IsDomandaBancari(datiPensione.SiglaCategoria) && (IsVisibleForOneri.HasValue && IsVisibleForOneri.Value)) ||
                            (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione)) ||
                            (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaSO(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "O")))
                            this.Tipo = 1;
                        else if ((IsVisibleForOneri.HasValue && IsVisibleForOneri.Value && !isOneriRicPrepensionamentoTipo0162NotVisible) ||
                                 (isPrepVisible.HasValue && isPrepVisible.Value &&
                                  tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti &&
                                  tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiapertura))
                            this.Tipo = 2;
                        else
                            this.Tipo = 0;
                        //set tab
                        if ((tipoDomanda == Utility.TipoDomanda.Ripristino && IsVisibleForOneri.HasValue && IsVisibleForOneri.Value) || ((IsVisibleForOneri.HasValue && IsVisibleForOneri.Value) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) &&
                            (Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) || Utility.IsDomandaBancari(datiPensione.SiglaCategoria) ||
                            (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione)) || Utility.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSE(datiPensione))) ||
                            (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaSO(datiPensione.SiglaCategoria) && !string.IsNullOrEmpty(datiPensione.NaturaPensione) && datiPensione.NaturaPensione.Substring(2, 1) == "O")))
                            this.TabOneri = 1;
                        else if (IsVisibleForOneri.HasValue && IsVisibleForOneri.Value && !isOneriRicPrepensionamentoTipo0162NotVisible)
                            this.TabOneri = 0;
                        else
                            this.TabOneri = null;

                        if (isPrepVisible.HasValue && isPrepVisible.Value &&
                            tipoDomanda != Utility.TipoDomanda.Ricostituzione && tipoDomanda != Utility.TipoDomanda.RipristinoSuperstiti &&
                            tipoDomanda != Utility.TipoDomanda.Ripristino && !isRiapertura)
                            this.TabPrepensionamento = 0;
                        else
                            this.TabPrepensionamento = null;

                        if (this.Tipo != 0 && isStoricoVisible && !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault()))
                            this.TabStorico = 0;
                        break;
                    case Utility.TipoAppartenenza.CI:
                        if (IsVisibleForOneri.HasValue && IsVisibleForOneri.Value)
                        {
                            if (Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura) &&
                                (Utility.IsDomandaAPEPrecoci(datiPensione) || Utility.IsDomandaQuota100(datiPensione) || Utility.IsDomandaInabilitaAmianto(datiPensione) || Utility.IsDomandaQuota102(datiPensione) || Utility.IsDomandaAnticipataFlessibile(datiPensione) || Utility.IsDomandaAnticipataFlessibileOpzioneContributivo(datiPensione) ||
                                (ctrlMemo123_2024 != null && ctrlMemo123_2024.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileLeggeBilancio2024(datiPensione)) || (ctrlMemo123_2024OpzioneContrib != null && ctrlMemo123_2024OpzioneContrib.ValoreControllo == "SI" && Utility.IsDomandaAnticipataFlessibileOpzioneContributivoLeggeBilancio2024(datiPensione))))
                            {
                                this.Tipo = 1;
                                this.TabOneri = 1;
                            }
                            else
                            {
                                this.Tipo = 2;
                                this.TabOneri = 0;
                            }
                        }
                        else
                        {
                            this.Tipo = 0;
                            this.TabOneri = null;
                        }
                        break;
                    default:
                        if (IsVisibleForOneri.HasValue && IsVisibleForOneri.Value)
                        {
                            this.Tipo = 2;
                            this.TabOneri = 0;
                        }
                        else
                        {
                            this.Tipo = 0;
                            this.TabOneri = null;
                        }
                        this.TabPrepensionamento = null;
                        break;
                }
            }
        }

        public class DatiQuadroDatiFondo
        {
            #region public properties
            public System.Nullable<byte> Tipo { get; set; }
            public System.Nullable<byte> TabRegistrazioniFondo { get; set; }
            #endregion public properties

            public DatiQuadroDatiFondo() { }

            public DatiQuadroDatiFondo(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppartenenza, bool isNuovaGestioneDatiFondoFSPT, bool? isDatiFondoOpzionale)
            {
                //ENG - MEMO 50/2023
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                        if (tipoFondo.HasValue)
                        {
                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.PT:
                                case Utility.TipoFondo.FS:
                                    if (isNuovaGestioneDatiFondoFSPT && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                    {
                                        if (isDatiFondoOpzionale.GetValueOrDefault())
                                        {
                                            this.Tipo = 1;
                                            this.TabRegistrazioniFondo = 1;
                                        }
                                        else
                                        {
                                            this.Tipo = 2;
                                            this.TabRegistrazioniFondo = 0;
                                        }
                                    }
                                    else
                                    {
                                        this.Tipo = 0;
                                        this.TabRegistrazioniFondo = null;
                                    }
                                    break;
                                case Utility.TipoFondo.DZ: //Nuova Gestione Dazi Daniele
                                    this.Tipo = 2;
                                    this.TabRegistrazioniFondo = 0;
                                    break;
                                default:
                                    this.Tipo = 0;
                                    this.TabRegistrazioniFondo = null;
                                    break;
                            }
                        }

                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                        {
                            this.Tipo = 2;
                            this.TabRegistrazioniFondo = 0;
                        }
                        break;
                    default:
                        this.Tipo = 0;
                        this.TabRegistrazioniFondo = null;
                        break;
                }
            }
        }

        public class DatiQuadroDatiRecordFondo
        {
            #region public properties
            public long IdRecordFondo { get; set; }
            public System.Nullable<byte> TabDatiCalcoloDZ { get; set; }
            public System.Nullable<byte> TabDatiFondo { get; set; }
            public System.Nullable<byte> TabDatiCalcolo { get; set; }
            public System.Nullable<byte> TabLegge460 { get; set; }
            public System.Nullable<byte> TabPrivilegiate { get; set; }
            public System.Nullable<byte> TabArticolo2 { get; set; }
            public System.Nullable<byte> TabDatiCalcolo707 { get; set; }
            public System.Nullable<byte> TabMiglioramentiContrattualiFS { get; set; }
            #endregion public properties

            public DatiQuadroDatiRecordFondo() { }

            public DatiQuadroDatiRecordFondo(long idRecordFondo, Utility.TipoAppartenenza? tipoAppartenenza, Utility.TipoDomanda tipoDomanda, GestionePensione.DatiPensione datiPensione,
                bool isNuovaGestioneDatiFondoFSPT, bool? isTabLegge460Required, bool? isTabPrivilegiataRequired, bool? isTabArticolo2Required, bool? isDatiFondoOpzionale, bool datiFondoPensioneDatiPTdaPrelievo, bool? isTabArticolo2NotVisible)
            {
                //ENG - MEMO 50/2023
                GestioneControlliDinamici.ControlloDinamico ctrlAbilitazioneMemo50_2023 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMemo50_2023", out ctrlAbilitazioneMemo50_2023);

                switch (tipoAppartenenza)
                {
                    #region FS
                    case Utility.TipoAppartenenza.FS:
                        Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(datiPensione.IndConvInt, datiPensione.Gestione, datiPensione.SiglaCategoria);
                        if (tipoFondo.HasValue)
                        {
                            this.TabMiglioramentiContrattualiFS = null;
                            switch (tipoFondo)
                            {
                                case Utility.TipoFondo.PT:
                                    this.TabDatiCalcoloDZ = null;
                                    this.TabMiglioramentiContrattualiFS = null;
                                    if (isNuovaGestioneDatiFondoFSPT && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                    {
                                        this.IdRecordFondo = idRecordFondo;
                                        if (!isDatiFondoOpzionale.GetValueOrDefault())
                                            this.TabDatiFondo = 0;
                                        else
                                            this.TabDatiFondo = 1;
                                        if (!isDatiFondoOpzionale.GetValueOrDefault() &&
                                            (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
                                            this.TabDatiCalcolo = 0;
                                        else
                                            this.TabDatiCalcolo = 1;

                                        /* 27/10/2021 - Per le PT rivista la gestione semaforica del tab Legge 4/60 quando i dati provengono dal prelievo
                                         - RIC non contributive: se presenti i dati della legge 4/60, il semaforo deve essere giallo (opzionale)
                                         - RIC contributive: se presenti i dati della legge 4/60 , il semaforo deve essere rosso (obbligatorio)
                                         Se per entrambe le tipologie di domande non sono presenti dati, il tab deve essere invisibile.
                                         Per gli altri flussi tutto rimane invariato 
                                         Per le RIC contributive, se viene aggiunta una registrazione il tab deve essere aggiunto ma di colore giallo */
                                        //ENG - RIC Contributive: tab sempre visibile
                                        if (isTabLegge460Required.GetValueOrDefault())
                                        {
                                            if (datiFondoPensioneDatiPTdaPrelievo && (Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione)))
                                                this.TabLegge460 = 1;
                                            else
                                                this.TabLegge460 = 0;
                                        }
                                        else
                                        {
                                            if (datiFondoPensioneDatiPTdaPrelievo && (Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione)))
                                                this.TabLegge460 = null;
                                            else
                                                this.TabLegge460 = 1;
                                        }

                                        if (datiPensione.SiglaCategoria.StartsWith("I"))
                                        {
                                            if (!isDatiFondoOpzionale.GetValueOrDefault() && isTabPrivilegiataRequired.GetValueOrDefault())
                                                this.TabPrivilegiate = 0;
                                            else
                                                this.TabPrivilegiate = 1;

                                            if (Utility.IsDomandaInabilitaProficuoLavoro(datiPensione) || Utility.IsDomandaInvaliditaOrdinaria(datiPensione) || Utility.IsDomandaInvaliditaSpecifica(datiPensione))
                                                this.TabArticolo2 = null;
                                            else if (!isDatiFondoOpzionale.GetValueOrDefault() && isTabArticolo2Required.GetValueOrDefault())
                                                this.TabArticolo2 = 0;
                                            else
                                                this.TabArticolo2 = 1;
                                        }
                                        else
                                        {
                                            this.TabPrivilegiate = null;
                                            this.TabArticolo2 = null;
                                        }
                                    }
                                    break;
                                case Utility.TipoFondo.FS:
                                    this.TabDatiCalcoloDZ = null;
                                    this.TabMiglioramentiContrattualiFS = null;
                                    if (isNuovaGestioneDatiFondoFSPT && !Utility.IsRicostituzione_PerVariazioneDatiSupplemento(datiPensione) && !(ctrlAbilitazioneMemo50_2023 != null && ctrlAbilitazioneMemo50_2023.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsRicostituzione_Supplemento(datiPensione)))
                                    {
                                        this.IdRecordFondo = idRecordFondo;
                                        if (!isDatiFondoOpzionale.GetValueOrDefault())
                                            this.TabDatiFondo = 0;
                                        else
                                            this.TabDatiFondo = 1;
                                        if (!isDatiFondoOpzionale.GetValueOrDefault() &&
                                            (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione)))
                                            this.TabDatiCalcolo = 0;
                                        else
                                            this.TabDatiCalcolo = 1;
                                        this.TabLegge460 = null;
                                        if (datiPensione.SiglaCategoria.StartsWith("I"))
                                        {
                                            if (!isDatiFondoOpzionale.GetValueOrDefault() && isTabPrivilegiataRequired.GetValueOrDefault())
                                                this.TabPrivilegiate = 0;
                                            else
                                                this.TabPrivilegiate = 1;

                                            if (Utility.IsDomandaInabilitaProficuoLavoro(datiPensione) || Utility.IsDomandaInvaliditaOrdinaria(datiPensione) || Utility.IsDomandaInvaliditaSpecifica(datiPensione))
                                                this.TabArticolo2 = null;
                                            else if (!isDatiFondoOpzionale.GetValueOrDefault() && isTabArticolo2Required.GetValueOrDefault())
                                                this.TabArticolo2 = 0;
                                            else
                                                this.TabArticolo2 = 1;
                                        }
                                        else
                                        {
                                            this.TabPrivilegiate = null;
                                            this.TabArticolo2 = null;
                                        }
                                    }
                                    break;
                                case Utility.TipoFondo.DZ: //Nuova Gestione Dazi Daniele
                                    this.TabDatiCalcoloDZ = 0;
                                    this.TabDatiFondo = null;
                                    this.TabDatiCalcolo = null;
                                    this.TabLegge460 = null;
                                    this.TabPrivilegiate = null;
                                    this.TabArticolo2 = null;
                                    this.TabDatiCalcolo707 = null;
                                    break;
                                default:
                                    this.TabDatiCalcoloDZ = null;
                                    this.TabDatiFondo = null;
                                    this.TabDatiCalcolo = null;
                                    this.TabLegge460 = null;
                                    this.TabPrivilegiate = null;
                                    this.TabArticolo2 = null;
                                    this.TabDatiCalcolo707 = null;
                                    break;
                            }
                        }

                        if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                        {
                            this.IdRecordFondo = idRecordFondo;
                            this.TabDatiFondo = 0;
                            this.TabDatiCalcoloDZ = null;
                            if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                            {
                                this.TabDatiCalcolo = 0;
                                //this.TabMiglioramentiContrattualiFS = 1; //al momento gestito su FS in GetRegistrazioneFondoINPDAPByIdRecordFondoPrivate
                            }
                            else
                                this.TabDatiCalcolo = 1;

                            //**Revisione Campi INPDAP**
                            //if (isTabLegge460Required.GetValueOrDefault())
                            //    this.TabLegge460 = 0;
                            //else
                            //    this.TabLegge460 = 1;
                            this.TabLegge460 = null;

                            if (Utility.IsSchedaPrivilegioVisible(datiPensione))
                            {
                                if (isTabPrivilegiataRequired.GetValueOrDefault())
                                    this.TabPrivilegiate = 0;
                                else
                                    this.TabPrivilegiate = 1;
                            }
                            else
                                this.TabPrivilegiate = null;

                            if (datiPensione.SiglaCategoria.StartsWith("I"))
                            {
                                if (isTabArticolo2NotVisible.GetValueOrDefault() || Utility.IsDomandaInabilitaProficuoLavoro(datiPensione) || Utility.IsDomandaInvaliditaOrdinaria(datiPensione) || Utility.IsDomandaInvaliditaSpecifica(datiPensione))
                                    this.TabArticolo2 = null;
                                else if (isTabArticolo2Required.GetValueOrDefault() || Utility.IsDomandaInabilitaLegge335(datiPensione))
                                    this.TabArticolo2 = 0;
                                else
                                    this.TabArticolo2 = 1;
                            }
                            else
                                this.TabArticolo2 = null;
                        }
                        break;
                    #endregion FS
                    default:
                        this.TabDatiCalcoloDZ = null;
                        this.TabDatiFondo = null;
                        this.TabDatiCalcolo = null;
                        this.TabLegge460 = null;
                        this.TabPrivilegiate = null;
                        this.TabArticolo2 = null;
                        this.TabDatiCalcolo707 = null;
                        this.TabMiglioramentiContrattualiFS = null;
                        break;
                }
            }
        }

        public class DatiQuadroDatiNoCalcolo
        {
            public DatiQuadroDatiNoCalcolo()
            { }
            public DatiQuadroDatiNoCalcolo(GestionePensione.DatiPensione datiPensione, bool isNoCalcoloVisible)
            {
                Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                switch (tipoApp)
                {
                    case Utility.TipoAppartenenza.CI:
                    case Utility.TipoAppartenenza.AGO:
                        this.Tipo = 0;
                        this.TabRegistrazioniNoCalcolo = null;
                        break;
                    case Utility.TipoAppartenenza.FS:
                        if (Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria) == Utility.TipoFondo.PI || Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria) == Utility.TipoFondo.PL)
                        {
                            if ((Utility.GetCategoriaFondoPI(tipoApp, datiPensione.SiglaCategoria) == Utility.CategoriaFondoPI.U ||
                                Utility.GetCategoriaFondoPI(tipoApp, datiPensione.SiglaCategoria) == Utility.CategoriaFondoPI.V ||
                                Utility.GetCategoriaFondoPI(tipoApp, datiPensione.SiglaCategoria) == Utility.CategoriaFondoPI.A) &&
                                isNoCalcoloVisible)
                            {
                                this.Tipo = 2;
                                this.TabRegistrazioniNoCalcolo = 0;
                            }
                            else
                            {
                                this.Tipo = 0;
                                this.TabRegistrazioniNoCalcolo = null;
                            }
                        }
                        break;
                }
            }
            public System.Nullable<byte> Tipo { get; set; }
            public System.Nullable<byte> TabRegistrazioniNoCalcolo { get; set; }
        }

        public class DatiQuadroRecordNoCalcolo
        {
            public long IdPensione { get; set; }
            public System.Nullable<long> IdRecordDatiNoCalcolo { get; set; }
            public System.Nullable<byte> TabNoCalcolo { get; set; }
        }

        public class DatiQuadroPeriodi
        {
            #region public properties

            public System.Nullable<byte> Tipo { get; set; }

            public System.Nullable<byte> TabPeriodi { get; set; }

            #endregion public properties

            public DatiQuadroPeriodi()
            { }

            public DatiQuadroPeriodi(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda,
                bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, GestioneDanteCausa.DatiDanteCausa danteCausa)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isSpacchettamentoPerRicostituzione)
                            {
                                this.Tipo = 2;
                                this.TabPeriodi = 0;
                            }
                        }
                        else if (Utility.IsDomandaSpacchettamentoINPDAP(datiPensione) ||
                            (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)))
                        {
                            this.Tipo = 2;
                            this.TabPeriodi = 0;
                        }
                        else
                        {
                            this.Tipo = 0;
                            this.TabPeriodi = null;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        this.Tipo = 0;
                        this.TabPeriodi = null;
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isSpacchettamentoPerRicostituzione)
                            {
                                this.Tipo = 2;
                                this.TabPeriodi = 0;
                            }
                        }
                        else if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                            || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                        {
                            this.Tipo = 2;
                            this.TabPeriodi = 0;
                        }
                        break;
                }

            }
        }

        public class DatiQuadroAventiDiritto
        {
            #region public properties

            public System.Nullable<byte> Tipo { get; set; }

            public System.Nullable<byte> TabAventiDiritto { get; set; }

            #endregion public properties

            public DatiQuadroAventiDiritto()
            { }

            public DatiQuadroAventiDiritto(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda,
                bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isSpacchettamentoPerRicostituzione)
                            {
                                this.Tipo = 2;
                                this.TabAventiDiritto = 0;
                            }
                        }
                        else if (Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                            || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)))
                        {
                            this.Tipo = 2;
                            this.TabAventiDiritto = 0;
                        }
                        else
                        {
                            this.Tipo = 0;
                            this.TabAventiDiritto = null;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        this.Tipo = 0;
                        this.TabAventiDiritto = null;
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isSpacchettamentoPerRicostituzione)
                            {
                                this.Tipo = 2;
                                this.TabAventiDiritto = 0;
                            }
                        }
                        else if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                            || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                        {
                            this.Tipo = 2;
                            this.TabAventiDiritto = 0;
                        }
                        break;
                }

            }
        }

        public class DatiQuadroAltreDomandeCollegate
        {
            #region public properties

            public System.Nullable<byte> Tipo { get; set; }

            public System.Nullable<byte> TabAltreDomandeCollegate { get; set; }

            #endregion public properties

            public DatiQuadroAltreDomandeCollegate()
            { }

            public DatiQuadroAltreDomandeCollegate(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione, Utility.TipoDomanda tipoDomanda,
                bool isRiaperturaDomanda, bool isSpacchettamentoPerRicostituzione, GestioneControlliDinamici.ControlloDinamico controlloDinamicoSpacchettate024, GestioneDanteCausa.DatiDanteCausa danteCausa)
            {
                switch (tipoAppartenenza)
                {
                    case Utility.TipoAppartenenza.FS:
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isSpacchettamentoPerRicostituzione)
                            {
                                this.Tipo = 2;
                                this.TabAltreDomandeCollegate = 2;
                            }
                        }
                        else if (Utility.IsDomandaSpacchettamentoINPDAP(datiPensione)
                            || (controlloDinamicoSpacchettate024 != null && controlloDinamicoSpacchettate024.ValoreControllo.ToUpperInvariant() == "SI" && Utility.IsDomandaSpacchettamento024(datiPensione, isRiaperturaDomanda)))
                        {
                            this.Tipo = 2;
                            this.TabAltreDomandeCollegate = 2;
                        }
                        else
                        {
                            this.Tipo = 0;
                            this.TabAltreDomandeCollegate = null;
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        this.Tipo = 0;
                        this.TabAltreDomandeCollegate = null;
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        if (tipoDomanda == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
                        {
                            if (isSpacchettamentoPerRicostituzione)
                            {
                                this.Tipo = 2;
                                this.TabAltreDomandeCollegate = 2;
                            }
                        }
                        else if (Utility.IsDomandaSpacchettamentoENPALS(datiPensione) || Utility.IsDomandaSpacchettamentoSOPGIPost072022(datiPensione, danteCausa) || Utility.IsDomandaSpacchettamentoSO(datiPensione, isRiaperturaDomanda)
                            || Utility.IsDomandaSpacchettamentoSOART(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSOCOM(datiPensione, isRiaperturaDomanda) || Utility.IsDomandaSpacchettamentoSR(datiPensione, isRiaperturaDomanda))
                        {
                            this.Tipo = 2;
                            this.TabAltreDomandeCollegate = 2;
                        }
                        break;
                }

            }
        }

        public class DatiQuadroRichiestaBonus
        {
            public DatiQuadroRichiestaBonus()
            { }

            public DatiQuadroRichiestaBonus(GestionePensione.DatiPensione datiPensione)
            {

                if (datiPensione.IsRichiestaBonus.GetValueOrDefault())
                {
                    this._Tipo = 0;
                    this._TabRichiestaBonus = null;
                    this._TabEsitoPrenotazione = null;
                }
            }

            public DatiQuadroRichiestaBonus(System.Nullable<byte> tipo, System.Nullable<byte> tabRichiestaBonus)
            {
                this._Tipo = tipo;
                this._TabRichiestaBonus = tabRichiestaBonus;
            }

            #region private properties
            private System.Nullable<byte> _Tipo;
            private System.Nullable<byte> _TabRichiestaBonus;
            private System.Nullable<byte> _TabEsitoPrenotazione;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public System.Nullable<byte> TabRichiestaBonus { get { return _TabRichiestaBonus; } set { _TabRichiestaBonus = value; } }
            public System.Nullable<byte> TabEsitoPrenotazione { get { return _TabEsitoPrenotazione; } set { _TabEsitoPrenotazione = value; } }
            #endregion public properties
        }

        #endregion nested classes
    }
}

