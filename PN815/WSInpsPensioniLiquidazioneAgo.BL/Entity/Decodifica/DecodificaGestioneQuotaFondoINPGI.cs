using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DecodificaGestioneQuotaFondoINPGI
    {
        #region private properties
        private long _Id;
        private DateTime? _PeriodoDal;
        private DateTime? _PeriodoAl;
        private string _TipoQuota;
        private string _TraduzioneSuGP;
        private string _Descrizione;
        #endregion private properties

        #region public properties
        public long Id { get { return _Id; } set { _Id = value; } }
        public DateTime? PeriodoDal { get { return _PeriodoDal; } set { _PeriodoDal = value; } }
        public DateTime? PeriodoAl { get { return _PeriodoAl; } set { _PeriodoAl = value; } }
        public string TipoQuota { get { return _TipoQuota; } set { _TipoQuota = value; } }
        public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
        public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
        #endregion public properties
    }
}
