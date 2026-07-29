using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using INPS.Pensioni.LiquidazioneCi.Service.Contracts.DataContracts;

namespace INPS.Pensioni.LiquidazioneCi.Service
{
    [ServiceContract(Namespace = "http://soa.inps.it/domainservices/pensioni/servicecontracts/LiquidazioneCi/1_0")]
    public interface IServizioLiquidazioneCi
    {
        [OperationContract]
        AreaEsito GetDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiContributivi(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito GetStatiEsteri(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out List<GestioneContrib.StatoEstero> elencoStatiEsteri, out string cittadinanzaTitolare);

        [OperationContract]
        AreaEsito GetStatiEsteriRicTrf(long numeroDomanda, short codiceSede, short centroOperativo, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, out List<GestioneContrib.StatoEstero> elencoStatiEsteri, out string cittadinanzaTitolare);

        [OperationContract]
        AreaEsito StoreDatiProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, ref AreaDatiContributivi areaDatiContributivi);

        //[OperationContract]
        //AreaEsito CancelDatiContributivi(long numeroDomanda);

        [OperationContract]
        AreaEsito CancelProRata(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore);

        [OperationContract]
        AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, out List<INPS.Pensioni.Liquidazione.BLCommon.GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out string statoPensione);

        [OperationContract]
        AreaEsito PrelevaDomanda(ref AreaPrelievo areaPrelievo);

        [OperationContract]
        AreaEsito StoreDatiCalcolo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiCalcolo(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiImportiEsteri(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiImportiEsteri(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiMaternitaAcna(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiMaternitaAcna(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiLavoratoriAutonomi(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiLavoratoriAutonomi(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreRedditiPerIntegrazioneVirtuale(long numeroDomanda, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelRedditiPerIntegrazioneVirtuale(long numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

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
        AreaEsito GetMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiBenefici(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiExCombattente(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiExCombattente(long numeroDomanda);

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
        AreaEsito StoreDatiPostDecOriginaria(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiPostDecOriginaria(long numeroDomanda);

        [OperationContract]
        AreaEsito GetListaVersioniCI(out AreaVersioni listaVersioni);

        [OperationContract]
        AreaEsito StoreDatiBeneficioVittimeTerrorismo(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiBeneficioVittimeTerrorismo(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        //[OperationContract]
        //AreaEsito StoreDatiVittimeTerrorismo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiInail(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiInail(long numeroDomanda);
    }
}
