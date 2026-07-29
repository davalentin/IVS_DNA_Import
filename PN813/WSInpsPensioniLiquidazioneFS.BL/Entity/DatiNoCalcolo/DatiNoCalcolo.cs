using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiNoCalcolo
    {
        //public long IdRecord { get; set; }

        //public long IdPensione { get; set; }

        public string Decorrenza { get; set; }

        public System.Nullable<decimal> AdeguataAgo { get; set; }

        public System.Nullable<decimal> AdeguataFondo { get; set; }

        public System.Nullable<decimal> EccedenzaAgo { get; set; }

        public System.Nullable<decimal> QuotaAgoEsclusiva { get; set; }

        public System.Nullable<decimal> FacArt14 { get; set; }

        public System.Nullable<decimal> IndIntSpeciale { get; set; }

        public System.Nullable<decimal> AssegniFamiliari { get; set; }

        public System.Nullable<decimal> AggFamigliaFondo { get; set; }

        public System.Nullable<decimal> OnereCaricoAmm { get; set; }

        public System.Nullable<decimal> Art21 { get; set; }

        public System.Nullable<decimal> ImportoMensile { get; set; }

        public System.Nullable<decimal> Tredicesima { get; set; }

        public System.Nullable<decimal> TipoVar { get; set; }

        public short? TabNoCalcolo { get; set; }

        public List<ComponentiFamiliari> ListaComponentiFamiliari { get; set; }

        public bool IsNull()
        {
            return Utility.PropertiesAreAllNull(this);
        }

        #region nested class
        public class ComponentiFamiliari
        {
            public string CodiceFiscale { get; set; }
            public bool IsSelected { get; set; }
        }
        #endregion nested class
    }
}
