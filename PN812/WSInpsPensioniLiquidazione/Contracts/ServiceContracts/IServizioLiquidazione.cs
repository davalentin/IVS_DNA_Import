using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.AggPec;
using INPS.Pensioni.Liquidazione.Service_Reference;
//using static INPS.Pensioni.Liquidazione.GestioneMsIndebiti;

namespace INPS.Pensioni.Liquidazione.Service
{
    [ServiceContract(Namespace = "http://soa.inps.it/domainservices/pensioni/servicecontracts/Liquidazione/1_0")]
    public interface IServizioLiquidazione
    {
        [OperationContract]
        AreaRispostaRiepilogo GetRiepilogoByKey(AreaRichiestaRiepilogo areaRichiestaDomande);

        [OperationContract]
        AreaTitolare GetAreaTitolareByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaEsito StoreAreaTitolare(AreaTitolare areaTitolare, out bool isTabAnagraficaSaved, out bool isWarning);

        [OperationContract]
        AreaEsito AggiornaAnagraficaTitolareByArca(long numeroDomanda, short sedeOperatore, string matricolaOperatore, ref AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica);

        [OperationContract]
        AreaEsito GetDetrazioniByDomanda(ref AreaDetrazioni areaDetrazioni);

        [OperationContract]
        AreaEsito GetSoggettiDetrazioniByDomanda(ref AreaDetrazioni areaDetrazioni);

        [OperationContract]
        AreaEsito SalvaFamiliari(long numeroDomanda, string cfFamiliareAttuale, string matricolaOperatore, ref List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari, List<string> elencoFamiliariDaRimuovere, ref List<Entity.Anagrafica> elencoAnagrafiche, out GestioneFamiliari.ConsultazioneUnificataANF ConsultazioneANF);

        [OperationContract]
        AreaEsito CancelFamiliari(long numeroDomanda, out List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari, out List<Entity.Anagrafica> elencoAnagrafiche);

        [OperationContract]
        AreaEsito VerifyDetrazioniByDomanda(ref AreaDetrazioni areaDetrazioni);

        [OperationContract]
        AreaEsito GetAnagraficaSoggettoByCodiceFiscale(string codiceFiscale, short sedeOperatore, string matricolaOperatore, string numDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica);

        [OperationContract]
        AreaEsito GetPagamentoByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, int abiCassaSede, out AreaPagamento areaPagamento);

        [OperationContract]
        AreaEsito StorePagamento(long numeroDomanda, ref AreaPagamento areaPagamento, string matricola, string sede);

        [OperationContract]
        AreaEsito CancelPagamentoByNumeroDomanda(long numeroDomanda);

        [OperationContract]
        AreaEsito GetUfficiPagatori(RichiestaUfficiPagatori richiesta, out List<UfficioPagatore> elencoUfficiPagatori);

        [OperationContract]
        AreaRispostaStatoPratica GetStatoPraticaByKey(AreaRichiestaStatoPratica areaRichiestaStatoPratica);

        [OperationContract]
        AreaEsito EliminaPensioneByNumeroDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo, Utility.Ruolo ruolo, int sedeDiAppartenenzaOperatore);

        [OperationContract]
        AreaEsito GetDelegatoByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica);

        [OperationContract]
        AreaEsito GetTutoreByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica);

        [OperationContract]
        void GetAnagraficaByDatiPersonaliParziali(short sedeOperatore, string matricolaOperatore, Entity.DatiPersonaliParziali datiPersonaliParziali, string numDomanda,
            out AreaRispostaRiepilogo risposta);

        [OperationContract]
        AreaEsito StoreDelegatoTutore(long numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiAnagraficaDelegato, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiAnagraficaTutore);

        [OperationContract]
        AreaEsito StoreDelegato(long numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica);

        [OperationContract]
        AreaEsito StoreTutore(long numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica);

        [OperationContract]
        AreaEsito GetFamiliareByNumeroDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out List<GestioneAreaFamiliari.AreaFamiliare> elencoFamiliari, out List<Entity.Anagrafica> elencoAnagrafiche, out GestioneAreaFamiliari.AreaDecFam areaDecodifica);

        [OperationContract]
        AreaEsito GetRedditiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, out AreaRedditi areaRedditi);

        [OperationContract]
        AreaEsito VerifyRedditiByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, bool IsSalvataggio, AreaRedditi areaRedditiOriginali, out AreaRedditi areaRedditiLast);

        [OperationContract]
        AreaEsito GetSupplementiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito SalvaSupplementiByDomanda(long numeroDomanda, AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito StoreDatiSupplementi(long numeroDomanda, AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito DeleteDatiSupplementiByDomanda(long numeroDomanda, out AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito StoreAnagrafica(AreaTitolare areaTitolare, out bool isWarning);

        [OperationContract]
        AreaEsito StoreStatoCivile(AreaTitolare areaTitolare);

        [OperationContract]
        AreaEsito DeleteStatoCivile(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreResidenzeEstere(AreaTitolare areaTitolare);

        [OperationContract]
        AreaEsito DeleteResidenzeEstere(long numeroDomanda);

        [OperationContract]
        AreaEsito GetDanteCausaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito StoreDanteCausa(long numeroDomanda, AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito CancelDanteCausa(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiAnagraficaDC(long numeroDomanda, AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito StoreDatiAltraPensione(long numeroDomanda, AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito StoreDatiPensioneCI(long numeroDomanda, AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito StoreDatiPensioneDiretta(long numeroDomanda, AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito StoreDatiRedditiSentenza49593(long numeroDomanda, AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito IsNotDelegatoOrTutorePresent(AreaRichiestaDomanda areaRichiestaDomanda, bool bDelegato);

        [OperationContract]
        AreaEsito DeleteDelegato(long numeroDomanda);

        [OperationContract]
        AreaEsito DeleteTutore(long numeroDomanda);

        [OperationContract]
        AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isVerify, bool isReingegnerizzato, AreaQuadri areaQuadri, bool isConsultazioniANFVerificate, out string statoPensione, out int certificato, out string chiavePensione, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni, out string transactionId, out string flagIndennizzo);

        [OperationContract]
        AreaEsito GetIsDomandaVerify(long numeroDomanda, out bool isVerify);

        [OperationContract]
        AreaEsito GetStampaDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out MemoryStream msPDF);

        [OperationContract]
        AreaEsito GetStampaDomandaByChiavePensione(AreaRichiestaStampa areaStampa, out MemoryStream msPDF);

        [OperationContract]
        AreaEsito DeleteStampaWeb(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaEsito GetAllLiquidazioniAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaLiquidazioniAbilitate areaLiquidazioniAbilitate);

        [OperationContract]
        AreaEsito StoreLiquidazioneAbilitata(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata);

        [OperationContract]
        AreaEsito StoreLiquidazioniAbilitateSuTutteLeSedi(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata);

        [OperationContract]
        AreaEsito DeleteLiquidazioneAbilitata(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata);

        [OperationContract]
        AreaEsito DeleteLiquidazioniAbilitateSuTutteLeSedi(AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata);

        [OperationContract]
        AreaEsito GetAllTrasformazioniAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaTrasformazioniAbilitate areaTrasformazioniAbilitate);

        [OperationContract]
        AreaEsito StoreTrasformazioneAbilitata(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata);

        [OperationContract]
        AreaEsito StoreTrasformazioniAbilitateSuTutteLeSedi(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata);

        [OperationContract]
        AreaEsito DeleteTrasformazioneAbilitata(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata);

        [OperationContract]
        AreaEsito DeleteTrasformazioniAbilitateSuTutteLeSedi(AreaTrasformazioniAbilitate.DatiTrasformazioneAbilitata datiTrasformazioneAbilitata);

        [OperationContract]
        AreaEsito AggiornaCI05(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaWebDom(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaFelpe(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaOneri(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaSai(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaINPDAP(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaNoteDiDebito(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaPianiDiPagamento(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaEquoInd(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaIndennSpec(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaAnagraficaSoggetto(string codiceFiscale, short sedeOperatore, string matricolaOperatore, string numDomanda, out AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica);

        [OperationContract]
        AreaEsito SbloccoDomanda(long numeroDomanda, Utility.TipoAppartenenza tipoAppRuolo, short sedeOperatore, short centroOperativoOperatore, out string sedeDiversa);

        [OperationContract]
        AreaEsito RiassegnazioneDomanda(ref AreaRiassegnazioneDomanda areaInputRiassegnazioneDomanda);

        [OperationContract]
        AreaEsito GetAllTipologieNonAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaTipologieNonAbilitate areaTipologieNonAbilitate);

        [OperationContract]
        AreaEsito StoreTipologieNonAbilitate(AreaTipologieNonAbilitate.TipologieNonAbilitate datiTipologieNonAbilitate);

        [OperationContract]
        AreaEsito DeleteTipologieNonAbilitate(AreaTipologieNonAbilitate.TipologieNonAbilitate datiTipologieNonAbilitate);

        [OperationContract]
        AreaEsito InvioSegnalazione(AreaInvioSegnalazione areaInvioSegnalazione);

        [OperationContract]
        AreaEsito GetAreaHomepage(Utility.TipoAppartenenza? tipoApp, out AreaHomepage areaHomepage);

        [OperationContract]
        AreaEsito GetListaVersioni(long currentVersionWA, out AreaVersioni listaVersioni);

        [OperationContract]
        AreaEsito GetMessaggiHermes(Utility.TipoAppartenenza? tipoApp, out AreaMessaggiHermes areaMessaggiHermes);

        [OperationContract]
        AreaEsito SalvaMessaggioHermes(ref AreaMessaggiHermes areaMessaggiHermes);

        [OperationContract]
        AreaEsito DeleteMessaggioHermes(Utility.TipoAppartenenza? tipoApp, ref AreaMessaggiHermes areaMessaggiHermes);

        [OperationContract]
        AreaEsito GetAvvisi(Utility.TipoAppartenenza? tipoApp, out AreaAvvisi areaAvvisi);

        [OperationContract]
        AreaEsito SalvaAvviso(ref AreaAvvisi areaAvvisi);

        [OperationContract]
        AreaEsito DeleteAvviso(Utility.TipoAppartenenza? tipoApp, ref AreaAvvisi areaAvvisi);

        [OperationContract]
        AreaEsito SbloccoCancellazione(AreaSbloccoCancellazione areaSbloccaCancellazione);

        [OperationContract]
        AreaEsito CancelDanteSentenza495_93(long numeroDomanda, out AreaDanteCausa areaDanteCausa);

        [OperationContract]
        AreaEsito GetEliminazioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaEliminazione areaEliminazione);

        [OperationContract]
        AreaEsito SalvaDatiEliminazioneByDomanda(long numeroDomanda, AreaEliminazione areaEliminazione);

        [OperationContract]
        AreaEsito StoreDatiEliminazione(long numeroDomanda, AreaEliminazione areaEliminazione);

        [OperationContract]
        AreaEsito DeleteDatiEliminazione(long numeroDomanda);

        //[OperationContract]
        //AreaEsito GetDecBypassControllo(Utility.TipoAppartenenza tipoApp,out AreaBypassControllo areaBypassControllo );

        [OperationContract]
        AreaEsito GetAllPensioniLavorazioneManualeAutomatiche(Utility.TipoAppartenenza tipoApp, out AreaLavorazioneManualeAutomatiche areaLavorazioneManualeAutomatiche);

        [OperationContract]
        AreaEsito GetAllPensioniLavorazioneManualeAutomaticheByCodiceSede(string utente, Utility.TipoAppartenenza tipoApp, List<Int16> codSede, out AreaLavorazioneManualeAutomatiche areaLavorazioneManualeAutomatiche);

        [OperationContract]
        AreaEsito StoreLavorazioneManualeAutomatiche(AreaLavorazioneManualeAutomatiche.LavorazioneManualeAutomatiche lavorazioneManualeAutomatiche);

        [OperationContract]
        AreaEsito GetAllBypassControllo(Utility.TipoAppartenenza tipoApp, out AreaBypassControllo areaBypassControllo);

        [OperationContract]
        AreaEsito DeleteBypassControllo(long idBypassControllo);

        [OperationContract]
        AreaEsito StoreBypassControllo(Utility.TipoAppartenenza tipoApp, AreaBypassControllo.BypassControllo bypassControllo);

        [OperationContract]
        AreaEsito GetDataSistema(Utility.TipoAppartenenza? tipoAppartenenza, out AreaControlliDinamici areaControlliDinamici);

        [OperationContract]
        AreaEsito SetDataSistema(Utility.TipoAppartenenza? tipoAppartenenza, AreaControlliDinamici areaControlliDinamici);

        [OperationContract]
        AreaEsito SetDataCalcoloDefinitivoINDCOM(Utility.TipoAppartenenza? tipoAppartenenza, AreaStoricoDataLimiteDomandeINDCOM.StoricoDataLimiteDomandeINDCOM areaStoricoDataLimiteDomandeINDCOM);

        [OperationContract]
        AreaEsito GetStoricoDataLimiteINDCOM(out AreaStoricoDataLimiteDomandeINDCOM areaStoricoDataLimiteDomandeINDCOM);

        [OperationContract]
        AreaEsito UpdateNoteStoricoDataLimiteINDCOM(int id, string note);

        [OperationContract]
        AreaEsito SetDataCalcoloPoligraficiLetteraB(Utility.TipoAppartenenza? tipoAppartenenza, AreaStoricoDataLimitePrepensionementoLetteraB.StoricoDataLimiteDomandePrepensionementoLetteraB areaStoricoDataLimitePrepensionamentoLetteraB);

        [OperationContract]
        AreaEsito GetStoricoDataLimitePoligraficiLetteraB(out AreaStoricoDataLimitePrepensionementoLetteraB areaStoricoDataLimitePrepensionamentoLetteraB);

        [OperationContract]
        AreaEsito UpdateNoteStoricoDataLimitePoligraficiLetteraB(int id, string note);

        [OperationContract]
        AreaEsito GetControlloDinamicoByNomeControllo(ref AreaControlliDinamici areaControlliDinamici);

        [OperationContract]
        AreaEsito GetAnnoCompetenza(Utility.TipoAppartenenza? tipoAppartenenza, out AreaControlliDinamici areaControlliDinamici);

        [OperationContract]
        AreaEsito GetFAQ(Utility.TipoAppartenenza? tipoApp, out AreaFAQ areaFAQ);

        [OperationContract]
        AreaEsito SalvaFAQ(ref AreaFAQ areaFAQ);

        [OperationContract]
        AreaEsito DeleteFAQ(Utility.TipoAppartenenza? tipoApp, ref AreaFAQ areaFAQ);

        [OperationContract]
        AreaEsito CaricaPdfFaq(Utility.TipoAppartenenza? tipoApp, out AreaFAQ areaFAQ);

        [OperationContract]
        AreaEsito CambioStatoDomanda(ref AreaCambioStatoDomanda areaCambioStatoDomanda);

        [OperationContract]
        AreaEsito GetPuliziaDomandaByDomanda(long numeroDomanda, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo, Utility.Ruolo ruolo,
            out AreaPuliziaDomanda areaPuliziaDomanda);

        [OperationContract]
        AreaEsito EseguiPuliziaDomandaByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo,
            Utility.Ruolo ruolo, out AreaPuliziaDomanda areaPuliziaDomanda);

        [OperationContract]
        AreaEsito EliminaRedditiByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, out AreaRedditi areaRedditi);

        [OperationContract]
        AreaEsito GetOneriByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaOneri areaOneri);

        [OperationContract]
        AreaEsito StoreOneri(long numeroDomanda, AreaOneri areaOneri);

        [OperationContract]
        AreaEsito StoreDatiOneriBeneficiParticolari(long numeroDomanda, AreaOneri areaOneri);

        [OperationContract]
        AreaEsito CancelDatiOneriBeneficiParticolari(long numeroDomanda, out AreaOneri areaOneri);

        [OperationContract]
        AreaEsito StoreDatiPrepensionamento(long numeroDomanda, AreaOneri areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiPrepensionamento(long numeroDomanda, out AreaOneri areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito GetDatiContributiviEnpalsByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, TipologiaContribuzioneEnpals tipologia, out BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneEnpals);

        [OperationContract]
        AreaEsito StoreDatiContributiviEnpals(long numeroDomanda, BLCommon.Entity.DatiContribuzioneEnpals datiContribuzioneEnpals);

        [OperationContract]
        AreaEsito GetAllCtrlBypassTipologieNonAbilitate(Utility.TipoAppartenenza tipoAppRuolo, out AreaCtrlBypassTipologieNonAbilitate areaCtrlBypassTipologieNonAbilitate);

        [OperationContract]
        AreaEsito StoreCtrlBypassTipologieNonAbilitate(AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate datiCtrlBypassTipologieNonAbilitate);

        [OperationContract]
        AreaEsito DeleteCtrlTipologieNonAbilitate(AreaCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate datiCtrlBypassTipologieNonAbilitate);

        [OperationContract]
        AreaEsito GetAggiornamenti(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamenti areaAggiornamenti);

        [OperationContract]
        AreaEsito DeleteAggiornamento(Utility.TipoAppartenenza? tipoApp, ref AreaAggiornamenti areaAggiornamenti);

        [OperationContract]
        AreaEsito SalvaAggiornamento(ref AreaAggiornamenti areaAggiornamenti);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoWebDom(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoWebDom);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoFelpe(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoFelpe);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoOneri(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoOneri);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoCumulo(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoCumulo);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoTot(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoTot);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoSAI(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoSAI);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoINPDAP(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoINPDAP);

        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoNoteDiDebito(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoNoteDiDebito);
        [OperationContract]
        AreaEsito CaricaPdfAggiornamentoPianiDiPagamento(Utility.TipoAppartenenza? tipoApp, out AreaAggiornamento areaAggiornamentoPianiDiPagamento);

        [OperationContract]
        AreaEsito GetAreaAggiornamento(Utility.TipoAppartenenza tipoApp, out AreaAggiornamento areaAggiornamento);

        [OperationContract]
        void ElaboraAggiornamentoWebDom(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoFelpe(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoOneri(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoCumulo(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoTot(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoSAI(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoINPDAP(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        void ElaboraAggiornamentoNoteDiDebito(Utility.TipoAppartenenza tipoApp);
        [OperationContract]
        void ElaboraAggiornamentoPianiDiPagamento(Utility.TipoAppartenenza tipoApp);

        [OperationContract]
        AreaEsito GetDatiSupplementoDettaglioEnpals(AreaRichiestaDomanda areaRichiestaDomanda, long idRecord, out AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito DeleteSupplementoDettaglioEnpals(long numeroDomanda, long idRecord, out AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito StoreSupplementoDettaglioEnpals(long numeroDomanda, ref AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito StoreRecordSupplementoEnpals(long numeroDomanda, ref AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito DeleteRecordSupplementoEnpals(long numeroDomanda, long idRecord);

        [OperationContract]
        AreaEsito GetAreaPeriodiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaPeriodi areaPeriodi);

        [OperationContract]
        AreaEsito GetAreaAventiDirittoByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaAventiDiritto areaAventiDiritto);

        [OperationContract]
        AreaEsito SalvaDatiPeriodiByDomanda(long numeroDomanda, AreaPeriodi areaPeriodi);

        [OperationContract]
        AreaEsito StorePeriodi(long numeroDomanda, AreaPeriodi areaPeriodi);

        [OperationContract]
        AreaEsito DeleteDatiPeriodi(long numeroDomanda, ref AreaPeriodi areaPeriodi);

        [OperationContract]
        AreaEsito SalvaDatiAventiDirittoByDomanda(long numeroDomanda, ref AreaAventiDiritto areaAventiDiritto);

        [OperationContract]
        AreaEsito StoreAventiDiritto(long numeroDomanda, ref AreaAventiDiritto areaAventiDiritto);

        [OperationContract]
        AreaEsito AggiornaAventiDirittoFromWebDom(long numeroDomanda, short sedeOperatore, string matricolaOperatore, out AreaAventiDiritto areaAventiDiritto);

        [OperationContract]
        AreaEsito GetAreaAltreDomandeCollegateByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaAltreDomandeCollegate areaAltreDomandeCollegate);

        [OperationContract]
        AreaEsito GetAventiDirittoDomandaCollegataByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, long numeroDomandaAventeDiritto, short sedeOperatore, string matricolaOperatore, out AreaAltreDomandeCollegate areaAltreDomandeCollegate);

        [OperationContract]
        AreaEsito GetAllBancheFideiussione(out AreaBancaFideiussione areaBancaFideiussione);

        [OperationContract]
        AreaEsito SalvaBancheFideiussione(ref AreaBancaFideiussione areaBancaFideiussione);

        [OperationContract]
        AreaEsito EliminaBancheFideiussione(ref AreaBancaFideiussione areaBancaFideiussione);

        [OperationContract]
        AreaEsito SalvaAziendaGGmmAAAA(ref AreaBancaFideiussione areaBancaFideiussioneAziendeGGmmAAAA);

        [OperationContract]
        AreaEsito EliminaAziendaGGmmAAAA(ref AreaBancaFideiussione areaBancaFideiussioneAziendeGGmmAAAA);

        [OperationContract]
        AreaEsito SalvaAzienda(ref AreaBancaFideiussione areaBancaFideiussioneAzienda);

        [OperationContract]
        AreaEsito GetAllBancheFideiussioneESPA(out AreaBancaFideiussioneESPA areaBancaFideiussione);

        [OperationContract]
        AreaEsito SalvaBancheFideiussioneESPA(ref AreaBancaFideiussioneESPA areaBancaFideiussione);

        [OperationContract]
        AreaEsito EliminaBancheFideiussioneESPA(ref AreaBancaFideiussioneESPA areaBancaFideiussione);

        [OperationContract]
        AreaEsito SalvaAziendaESPAGGmmAAAA(ref AreaBancaFideiussioneESPA areaBancaFideiussioneAziendeGGmmAAAA);

        [OperationContract]
        AreaEsito EliminaAziendaESPAGGmmAAAA(ref AreaBancaFideiussioneESPA areaBancaFideiussioneAziendeGGmmAAAA);

        [OperationContract]
        AreaEsito SalvaAziendaESPA(ref AreaBancaFideiussioneESPA areaBancaFideiussioneAzienda);

        [OperationContract]
        AreaEsito AggiornaAventiDirittoFromArchivioPensione(long numeroDomanda, short sedeOperatore, short centroOperativoOperatore, string matricolaOperatore, out AreaAventiDiritto areaAventiDiritto);

        [OperationContract]
        AreaEsito GetAllSediMatricola(string sede, out AreaSediMatricola sediMatricola);

        [OperationContract]
        AreaEsito GetAllAziendeVESO33(out AreaAziendeVESO33 areaAziendaVESO33);

        [OperationContract]
        AreaEsito SalvaAziendeVESO33(ref AreaAziendeVESO33 areaAziendaVESO33);

        [OperationContract]
        AreaEsito EliminaAziendeVESO33(ref AreaAziendeVESO33 areaAziendaVESO33);

        [OperationContract]
        AreaEsito GetAllAziendeCredito(string categoriaAzienda, out AreaAziendeCredito areaAziendaCredito);

        [OperationContract]
        AreaEsito SalvaAziendeCredito(string categoriaAzienda, ref AreaAziendeCredito areaAziendaCredito);

        [OperationContract]
        AreaEsito EliminaAziendeCredito(string categoriaAzienda, ref AreaAziendeCredito areaAziendaCredito);

        [OperationContract]
        AreaEsito GetAllAziendeEditoriali(out AreaAziendeEditoriali areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAccordi(ref AreaAziendeEditoriali areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAccordi(ref AreaAziendeEditoriali areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAziende(ref AreaAziendeEditoriali areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAziende(ref AreaAziendeEditoriali areaAziendeEditoriali);

        [OperationContract]
        AreaEsito GetAllAziendeEditorialiLetteraB(out AreaAziendeEditorialiLetteraB areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAccordiLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAccordiLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAziendeLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAziendeLetteraB(ref AreaAziendeEditorialiLetteraB areaAziendeEditoriali);

        [OperationContract]
        AreaEsito GetAllAziendeEditorialiPerTipo0171(out AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAccordiPerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAccordiPerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAziendePerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAziendePerTipo0171(ref AreaAziendeEditorialiPerTipo0171 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito GetAllAziendeEditorialiPerTipo0179(out AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAccordiPerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAccordiPerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito SalvaAnagraficaAziendePerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito EliminaAnagraficaAziendePerTipo0179(ref AreaAziendeEditorialiPerTipo0179 areaAziendeEditoriali);

        [OperationContract]
        AreaEsito GetDataDecorrenzaProvvisorieObbligatoriePerCoefficienti(Utility.TipoAppartenenza? tipoAppartenenza, out AreaProvvisoriePerCoefficienti areaProvvisoriePerCoefficienti);

        [OperationContract]
        AreaEsito SetDataDecorrenzaProvvisorieObbligatoriePerCoefficienti(Utility.TipoAppartenenza? tipoAppartenenza, AreaProvvisoriePerCoefficienti areaProvvisoriePerCoefficienti);

        [OperationContract]
        AreaEsito GetAreaAbilitazioneServizi(out AreaAbilitazioneServizi areaAbilitazioneServizi);

        [OperationContract]
        AreaEsito SetPolarizzazioneENPALSAttivo(AreaAbilitazioneServizi areaAbilitazioneServizi);

        [OperationContract]
        AreaEsito AggiornaTotal(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaTotalPerTot(long numeroDomanda, out string statoPensione);

        [OperationContract]
        AreaEsito AggiornaBooking(long numeroDomanda, string matricolaOperatore, short sedeOperatore, out string statoPensione, out List<GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni);

        [OperationContract]
        AreaEsito GetAllAziendeESOAMB(out AreaAziendeESOAMB areaAziendaESOAMB);

        [OperationContract]
        AreaEsito SalvaAziendeESOAMB(ref AreaAziendeESOAMB areaAziendaESOAMB);

        [OperationContract]
        AreaEsito EliminaAziendeESOAMB(ref AreaAziendeESOAMB areaAziendaESOAMB);

        [OperationContract]
        AreaEsito SalvaAziendaESOAMBGGmmAAAA(ref AreaAziendeESOAMB areaAziendaESOAMB);

        [OperationContract]
        AreaEsito EliminaAziendaESOAMBGGmmAAAA(ref AreaAziendeESOAMB areaAziendaESOAMB);

        [OperationContract]
        AreaEsito GetAllAziendeESOTEL(out AreaAziendeESOTEL areaAziendaESOTEL);

        [OperationContract]
        AreaEsito SalvaAziendeESOTEL(ref AreaAziendeESOTEL areaAziendaESOTEL);

        [OperationContract]
        AreaEsito EliminaAziendeESOTEL(ref AreaAziendeESOTEL areaAziendaESOTEL);

        [OperationContract]
        AreaEsito GetAllAziendeVESO29(out AreaAziendeVESO29 areaAziendaVESO29);

        [OperationContract]
        AreaEsito SalvaAziendeVESO29(ref AreaAziendeVESO29 areaAziendaVESO29);

        [OperationContract]
        AreaEsito EliminaAziendeVESO29(ref AreaAziendeVESO29 areaAziendaVESO29);

        [OperationContract]
        AreaEsito GetAllAziendeVOESO(string categoriaAzienda, out AreaAziendeVOESO areaAziendaVOESO);

        [OperationContract]
        AreaEsito SalvaAziendeVOESO(string categoriaAzienda, ref AreaAziendeVOESO areaAziendaVOESO);

        [OperationContract]
        AreaEsito EliminaAziendeVOESO(string categoriaAzienda, ref AreaAziendeVOESO areaAziendaVOESO);

        [OperationContract]
        AreaEsito StoreDatiSupplementiCumulo(long numeroDomanda, AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito DeleteDatiSupplementiCumuloByDomanda(long numeroDomanda, out AreaSupplementi areaSupplementi);

        [OperationContract]
        AreaEsito GetAltreFunzioniByMatricola(string matricola, out AreaAltreFunzioni areaAltreFunzioni);

        [OperationContract]
        AreaEsito GetRichiestaBonusByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, out AreaRichiestaBonus areaRichiestaBonus, out bool IsDataFromDB);

        [OperationContract]
        AreaEsito StoreDatiRichiestaBonus(long numeroDomanda, ref AreaRichiestaBonus areaRichiestaBonus);

        [OperationContract]
        AreaEsito EliminaRichiestaBonusByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, out AreaRichiestaBonus areaRichiestaBonus);

        [OperationContract]
        AreaEsito VerificaAdesioneFondoCredito(string codiceFiscaleTitolare);

        [OperationContract]
        AreaEsito GetAllBancheFideiussioneESOPMI(out AreaBancaFideiussioneESOPMI areaBancaFideiussione);

        [OperationContract]
        AreaEsito SalvaBancheFideiussioneESOPMI(ref AreaBancaFideiussioneESOPMI areaBancaFideiussione);

        [OperationContract]
        AreaEsito EliminaBancheFideiussioneESOPMI(ref AreaBancaFideiussioneESOPMI areaBancaFideiussione);

        [OperationContract]
        AreaEsito SalvaAziendaESOPMIGGmmAAAA(ref AreaBancaFideiussioneESOPMI areaBancaFideiussioneAziendeGGmmAAAA);

        [OperationContract]
        AreaEsito EliminaAziendaESOPMIGGmmAAAA(ref AreaBancaFideiussioneESOPMI areaBancaFideiussioneAziendeGGmmAAAA);

        [OperationContract]
        AreaEsito SalvaAziendaESOPMI(ref AreaBancaFideiussioneESOPMI areaBancaFideiussioneAzienda);

        [OperationContract]
        AreaEsito IsMatricolaForAutomazione(string matricola, out bool isMatricolaForAutomazione);

        [OperationContract]
        AreaEsito InsertOrUpdateNuovoCalcolo(AreaNuovoCalcolo areaNuovoCalcolo);

        [OperationContract]
        void GetEsitoNuovoCalcolo(long? Ndomus, string TransactionId, out AreaNuovoCalcolo areaNuovoCalcolo);

        [OperationContract]
        AreaEsito IsNuovoCalcolo(long numeroDomanda, bool isVerify, out bool isNuovoCalcolo, out bool esitoInattesa);

        [OperationContract]
        void SalvaLog(long numdomanda, string methodname, string errore);

        //[OperationContract]
        //void SalvaLogDebug(long numdomanda, string methodname, string errore);

        [OperationContract]
        AreaEsito GetDatiPECO_FunzioneC(string nDomus, string codFisc, string Appartenenza, string Gestione, string Fondo, ref string Caratterizzazione, out string errore);

        [OperationContract]
        AreaEsito GetCodFaseByNDomus(string nDomus, out string codFase);

        [OperationContract]
        AreaEsito CleanTipoSpecECaratterizzazione(string nDomus, ref string Caratterizzazione, out string errore);

        [OperationContract]
        AreaEsito GetFlagIndebitoByDomusAndProgressivoStorico(Int64 NDomus, byte? ProgressivoStorico, out string FlagIndebito);

        [OperationContract]
        AreaEsito GetAnteprimaDebito(long numeroDomanda, string matricola, out RootIndebitoDto indebito);

        [OperationContract]
        AreaEsito AggiornaCasuali(long numeroDomanda, string matricola, IndebitoDto indebito, bool flagCi, short sedeOperatore, short centroOperativoOperatore);

        [OperationContract]
        AreaEsito NotificaTE08(long numeroDomanda, string matricola, bool flagCi, short sedeOperatore, short centroOperativoOperatore);

        [OperationContract]
        AreaEsito SalvaIndebito(IndebitoDto indebito);
    }
}
