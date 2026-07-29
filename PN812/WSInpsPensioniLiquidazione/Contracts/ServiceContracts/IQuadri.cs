using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts;

namespace INPS.Pensioni.Liquidazione.Service
{
    [ServiceContract(Namespace = "http://soa.inps.it/domainservices/pensioni/servicecontracts/Liquidazione/1_0")]
    public interface IQuadri
    {
        [OperationContract]
        AreaEsito AggiornaQuadri(AreaRichiestaDomanda areaRichiestaDomanda, ref AreaInfoPratica areaInfoPratica);
        
        [OperationContract]
        AreaQuadri GetQuadriByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);
        
        [OperationContract]
        AreaQuadri.DatiQuadroTitolare GetQuadroTitolareByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);
        
        [OperationContract]
        AreaQuadri.DatiQuadroDetrazioni GetQuadroDetrazioniByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroPagamento GetQuadroPagamentoByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

		[OperationContract]
        AreaQuadri.DatiQuadroLiquidazionePensione GetQuadroLiquidazionePensioneByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

		[OperationContract]
        AreaQuadri.DatiQuadroDelegatoTutore GetQuadroDelegatoTutoreByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroDatiContributivi GetQuadroDatiContributiviByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroRedditi GetQuadroRedditiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroDanteCausa GetQuadroDanteCausaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);
        
        [OperationContract]
        AreaQuadri.DatiQuadroFamiliari GetQuadroFamiliariByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroMaggiorazioniBenefici GetQuadroMaggiorazioniBeneficiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroSupplementi GetQuadroSupplementiByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroBititolarita GetQuadroBititolaritaByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroOneri GetQuadroOneriByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);

        [OperationContract]
        AreaQuadri.DatiQuadroRichiestaBonus GetQuadroRichiestaBonusByDomanda(AreaRichiestaDomanda areaRichiestaDomanda);
	}
}
