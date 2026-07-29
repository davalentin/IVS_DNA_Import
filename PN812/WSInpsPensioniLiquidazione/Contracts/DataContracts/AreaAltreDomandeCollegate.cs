using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAltreDomandeCollegate
    {
        [DataMember]
        public List<Entity.DomandeCollegate> ElencoDomandeCollegate { get; set; }

        [DataMember]
        public Entity.AventiDiritto AreaAventiDiritto { get; set; }

        [DataMember]
        public List<GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare> ElencoGradiParentela { get; set; }
    }
}