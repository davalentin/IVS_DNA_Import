using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiDL407
    {  
        #region private properties

        private long _Id;

        private long _IdPensione;

        private int? _NSettimaneQuotaA;

        private int? _NSettimaneQuotaB;

        private int? _NSettimaneQuotaC;

        private int? _NSettimaneQuotaD;

        private decimal? _RMSQuotaA;

        private decimal? _RMSQuotaB;

        private decimal? _RMSQuotaD;

        private List<GestioneMaggiorazioniBenefici.DatiServizioUtileDL407> _LstServizioUtileAnteArm;
        
        #endregion private properties

        #region public properties

        public long Id { get { return _Id; } set { _Id = value; } }

        public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

        public int? NSettimaneQuotaA { get { return _NSettimaneQuotaA; } set { _NSettimaneQuotaA = value; } }

        public int? NSettimaneQuotaB { get { return _NSettimaneQuotaB; } set { _NSettimaneQuotaB = value; } }

        public int? NSettimaneQuotaC { get { return _NSettimaneQuotaC; } set { _NSettimaneQuotaC = value; } }

        public int? NSettimaneQuotaD { get { return _NSettimaneQuotaD; } set { _NSettimaneQuotaD = value; } }

        public decimal? RMSQuotaA { get { return _RMSQuotaA; } set { _RMSQuotaA = value; } }

        public decimal? RMSQuotaB { get { return _RMSQuotaB; } set { _RMSQuotaB = value; } }

        public decimal? RMSQuotaD { get { return _RMSQuotaD; } set { _RMSQuotaD = value; } }

        public List<GestioneMaggiorazioniBenefici.DatiServizioUtileDL407> LstServizioUtileAnteArm { get { return _LstServizioUtileAnteArm; } set { _LstServizioUtileAnteArm = value; }}

        #endregion public properties

        public bool IsDL407Null()
        {
            if (!this._NSettimaneQuotaA.HasValue &&
                !this._NSettimaneQuotaB.HasValue &&
                !this._NSettimaneQuotaC.HasValue &&
                !this._NSettimaneQuotaD.HasValue &&
                !this._RMSQuotaA.HasValue &&
                !this._RMSQuotaB.HasValue &&
                !this._RMSQuotaD.HasValue &&
                (LstServizioUtileAnteArm == null || LstServizioUtileAnteArm.Count == 0))
                return true;
            else
                return false;
        }


      

    }
}
