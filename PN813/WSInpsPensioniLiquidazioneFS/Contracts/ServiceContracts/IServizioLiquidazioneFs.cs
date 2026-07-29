using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using INPS.Pensioni.LiquidazioneFs.Service.Contracts.DataContracts;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Service
{
    [ServiceContract(Namespace = "http://soa.inps.it/domainservices/pensioni/servicecontracts/LiquidazioneFs/1_0")]
    public interface IServizioLiquidazioneFs
    {
        [OperationContract]
        AreaEsito GetLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreLiquidazionePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        //[OperationContract]
        //AreaEsito CancelLiquidazionePensione(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiGenerici(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiGenerici(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiAssicurativi(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiAssicurativi(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiPrecedentePensione(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiPrecedentePensione(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiBititolaritaInail(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiBititolaritaInail(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiLegge460(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiLegge460(long numeroDomanda);

        [OperationContract]
        AreaEsito GetMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreMaggiorazioniBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        //[OperationContract]
        //AreaEsito CancelMaggiorazioniBenefici(long numeroDomanda);

        [OperationContract]
        AreaEsito StoreDatiExCombattente(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiBenefici(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiDL407(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiPrivilegiate(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito StoreDatiArticolo2(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiExCombattente(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiBenefici(long numeroDomandadati, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiDL407(long numeroDomanda);

        [OperationContract]
        AreaEsito CancelDatiArticolo2(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiPrivilegiate(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CalcolaDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, bool isConsultazioniANFVerificate, bool isReingegnerizzato, bool? isNuovoCalcolo, out List<GestioneFamiliari.ConsultazioneUnificataANF> listaConsultazioniANF, out string statoPensione, out int certificato);

        [OperationContract]
        AreaEsito PrelevaDomanda(ref AreaPrelievo areaPrelievo);

        [OperationContract]
        AreaEsito EseguiSprenotazione(AreaPrelievo areaPrelievo);

        [OperationContract]
        AreaEsito GetDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiContributiviByDomanda(Int64 numeroDomanda, ref AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiCalcoloByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiCalcoloByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiCalcolo707ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiCalcolo707ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiFondoByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiFondoByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiArt14e11ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiArt14e11ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito GetListaVersioniFS(out AreaVersioni listaVersioni);

        [OperationContract]
        AreaEsito StoreAnte67ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiAnte67ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreSL336ByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelSL336ByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        //Dati  Registrazioni fondo
        [OperationContract]
        AreaEsito GetQuadroDatiFondoByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito AddRegistrazioneFondoByDomanda(Int64 numeroDomanda, out AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelRegistrazioneFondoByIdRecordFondo(Int64 numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelRegistrazioniFondoByDomanda(Int64 numeroDomanda, out AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito GetRegistrazioneFondoByIdRecordFondo(AreaRichiestaDomanda areaRichiestaDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito StoreQuadroDatiFondoByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);
        //Dati Fondo
        [OperationContract]
        AreaEsito StoreDatiFondoByIdRecordFondo(long numeroDomanda , ref AreaDatiFondo areaDatiFondo);
        
        [OperationContract]
        AreaEsito CancelDatiFondoByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        //Dati Calcolo
        [OperationContract]
        AreaEsito StoreDatiCalcoloByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelDatiCalcoloByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        //Dati Calcolo 707
        [OperationContract]
        AreaEsito StoreDatiCalcolo707ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelDatiCalcolo707ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        //Dati Legge 4/60
        [OperationContract]
        AreaEsito StoreDatiLegge460ForDatiFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelDatiLegge460ForDatiFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        //Dati Privilegiate
        [OperationContract]
        AreaEsito StoreDatiPrivilegiateByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelDatiPrivilegiateByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        //Dati Articolo 2
        [OperationContract]
        AreaEsito StoreDatiArticolo2ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito CancelDatiArticolo2ByIdRecordFondo(long numeroDomanda, ref AreaDatiFondo areaDatiFondo);

        [OperationContract]
        AreaEsito GetQuadroDatiRecordNoCalcoloByDomanda(AreaRichiestaDomanda areaRichiestaDomanda, out AreaNoCalcolo areaNoCalcolo);

        [OperationContract]
        AreaEsito AddRecordNoCalcoloByDomanda(long numeroDomanda, out AreaNoCalcolo areaDatiNoCalcolo);

        [OperationContract]
        AreaEsito GetDatiNoCalcoloByIdRecord(AreaRichiestaDomanda areaRichiestaDomanda, long idRecord, out AreaNoCalcolo areaDatiNoCalcolo);

        [OperationContract]
        AreaEsito StoreDatiNoCalcolo(long numeroDomanda, long idRecord,ref AreaNoCalcolo areaDatiNoCalcolo);

        [OperationContract]
        AreaEsito CancelRecordDatiNoCalcolo(long numeroDomanda, long idRecord, out AreaNoCalcolo areaDatiNoCalcolo);

        [OperationContract]
        AreaEsito CancelAllRecordDatiNoCalcolo(long numeroDomanda, out AreaNoCalcolo areaDatiNoCalcolo);

        [OperationContract]
        AreaEsito DeleteDatiNoCalcolo(long numeroDomanda, long idRecord, out AreaNoCalcolo areaDatiNoCalcolo);

        [OperationContract]
        AreaEsito StoreAltraPensioneDatiAgoByDomanda(Int64 numeroDomanda, AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito CancelDatiAgoAltraPensioneByDomanda(Int64 numeroDomanda, out AreaDatiContributivi areaDatiContributivi);

        [OperationContract]
        AreaEsito StoreDatiIstruttoria(long numeroDomanda, AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito CancelDatiIstruttoria(long numeroDomanda, out AreaLiquidazionePensione areaLiquidazionePensione);

        [OperationContract]
        AreaEsito StoreDatiBeneficioVittimeTerrorismo(long numeroDomanda, AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito CancelDatiBeneficioVittimeTerrorismo(long numeroDomanda, out AreaMaggiorazioniBenefici areaMaggiorazioniBenefici);

        [OperationContract]
        AreaEsito GetDatiAgoFondoPIById(long idDatiAgoFondoPI ,out AreaDatiAgoFondoPI AreaDatiAgoFondoPI);

        [OperationContract]
        AreaEsito StoreDatiAgoFondoPIById(AreaDatiAgoFondoPI AreaDatiAgoFondoPI);

        [OperationContract]
        AreaEsito CancelDatiAgoPensioneFondoPI(long idDatiAgoPI);

        [OperationContract]
        AreaEsito GetDatiPensioneFondoPIById(long idRecord, out AreaDatiPensioneFondoPI AreaDatiPensioneFondoPI);

        [OperationContract]
        AreaEsito StoreDatiPensioneFondoPIByIdRecord(AreaDatiPensioneFondoPI AreaDatiPensioneFondoPI);

        [OperationContract]
        AreaEsito CancelDatiFondoPensioneFondoPI(long idRecordFondo);

     

        //[OperationContract]
        //AreaEsito StoreDatiVittimeTerrorismo(long numeroDomanda, AreaDatiContributivi areaDatiContributivi);
    }
}
