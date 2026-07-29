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
    public interface IDecodifica
    {
        [OperationContract]
        AreaDecodifica GetDecodifica();

        [OperationContract]
        List<AreaDecodifica.DatiComune> GetComuniPerProvincia(string siglaProvincia);
    }
}
