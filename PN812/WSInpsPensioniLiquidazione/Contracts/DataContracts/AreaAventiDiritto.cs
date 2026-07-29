using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaAventiDiritto
    {
        #region public data members

        [DataMember]
        public Entity.AventiDiritto DatiAventiDiritto { get; set; }

        [DataMember]
        public List<GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare> ElencoGradiParentela { get; set; }

        [DataMember]
        public bool IsFascicoloGenerato { get; set; }
        #endregion public data members
    }
}