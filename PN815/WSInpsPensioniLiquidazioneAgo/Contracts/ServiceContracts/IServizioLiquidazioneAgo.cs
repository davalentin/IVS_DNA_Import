using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using INPS.Pensioni.LiquidazioneAgo.Service.Contracts.DataContracts;

namespace INPS.Pensioni.LiquidazioneAgo.Service
{
    [ServiceContract(Namespace = "http://soa.inps.it/domainservices/pensioni/servicecontracts/LiquidazioneAgo/1_0")]
    public interface IServizioLiquidazioneAgo
    {
        [OperationContract]
        AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, bool? isNuovoCalcolo, out string statoPensione, out List<INPS.Pensioni.Liquidazione.BLCommon.GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioni, out string transactionId);

        [OperationContract]
        AreaEsito PrelevaDomanda(ref AreaPrelievo areaPrelievo);

        [OperationContract]
        AreaEsito GetDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out AreaDatiContributivi areaDatiContributivi, out bool IsDataFromDB);

        [OperationContract]
        AreaEsito GetStatiEsteri(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, long idPensione, out List<GestioneContrib.StatoEsteroCumulo> elencoStatiEsteri);

        [OperationContract]
        AreaEsito CancelDatiContributiviByDomanda(Int64 numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiContributiviByDomanda(Int64 numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiLiquidazionePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito GetLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiGenerici(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiGenerici(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiAssicurativi(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiAssicurativi(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiOpzione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiOpzione(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiIstruttoria(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiIstruttoria(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiProvenienza(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiProvenienza(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiInail(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiInail(long numeroDomanda);

        [OperationContract]
        AreaEsito GetMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiBenefici(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiExCombattente(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiExCombattente(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiMaggiorazioni(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiMaggiorazioni(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito GetBititolaritaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiBititolarita areaBititolarita);

        [OperationContract]
        AreaEsito StoreBititolarita(long numeroDomanda, AreaDatiBititolarita areaBititolarita);

        [OperationContract]
        AreaEsito StoreAltraPensione(long numeroDomanda, AreaDatiBititolarita areaBititolarita);

        [OperationContract]
        AreaEsito CancelAltraPensione(long numeroDomanda, out AreaDatiBititolarita areaBititolarita);

        [OperationContract]
        AreaEsito GetListaVersioniAGO(out AreaVersioni listaVersioni);

        [OperationContract]
        AreaEsito StoreDatiBeneficioVittimeTerrorismo(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiBeneficioVittimeTerrorismo(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiVittimeTerrorismo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiVittimeTerrorismo(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiCalcoloByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiQuoteMiglioramentiContrattualiByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito PrelevaGP4(ref AreaPrelievo areaPrelievo);

        [OperationContract]
        AreaEsito CancelDatiSentenzaArt4(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiSentenzaArt4(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiSentenze(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiSentenze(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiQuotaFondoIntegrativo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiQuotaFondoIntegrativo(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiQuotaFondoINPGI(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiQuotaFondoINPGI(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore);

        [OperationContract]
        AreaEsito CancelProRataSingolo(long idPrestazione, long numeroDomanda);

        [OperationContract]
        AreaEsito RecuperaStatiEsteri(string codStato, string codIstituzione, out string descCodStato, out string descCodIstituzione, out string descCittà, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CompatibilitàCodiceConvenzioneWithStatoEstero(AreaRichiestaDomanda areaRichiestaDomanda, GestioneContrib.StatoEsteroCumulo stato);
    }
}
