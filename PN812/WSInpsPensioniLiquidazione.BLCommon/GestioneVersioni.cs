using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneVersioni
    {
        public static void AggiornaVersioni(Dictionary<string, long> listaNumeroVersioni)
        {
            foreach (KeyValuePair<string, long> versione in listaNumeroVersioni)
            {
                DAGestioneVersioni.AggiornaDatiVersione(versione.Key, versione.Value);
            }
        }

        public static void GetVersioni(out List<DatiVersioni> listaVersioni)
        {
            List<Versioni> lVersioni = null;
            listaVersioni = null;
            DAGestioneVersioni.GetVersioni(out lVersioni);
            if (lVersioni == null || lVersioni.Count == 0)
                return;
            listaVersioni = new List<DatiVersioni>();
            foreach (Versioni versioniDB in lVersioni)
            {
                DatiVersioni versioni = new DatiVersioni();
                Utility.ValorizzaOggetti(versioniDB, versioni);
                listaVersioni.Add(versioni);
            }
        }

        #region nested class
        public class DatiVersioni
        {
            #region private properties
            private string _Applicativo;
            private long _NumVersione;
            private System.DateTime _Data;
            #endregion private properties

            #region public properties
            public string Applicativo { get { return _Applicativo; } set { _Applicativo = value; } }
            public long NumVersione { get { return _NumVersione; } set { _NumVersione = value; } }
            public System.DateTime Data { get { return _Data; } set { _Data = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
