using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDL407
    {
        public static void GetDL407ByIdPensione(long idPensione, out DatiDL407 datiDL407)
        {
            DL407 dl407 = null;
            datiDL407 = null;

            DAGestioneDL407.GetDL407ByIdPensione(idPensione, out dl407);

            if (dl407 == null)
                return;

            datiDL407 = new DatiDL407();
            Utility.ValorizzaOggetti(dl407, datiDL407);
        }

        public static void SalvaDL407(long idPensione, DatiDL407 datiDL407)
        {
            DL407 dl407 = new DL407();
            Utility.ValorizzaOggetti(datiDL407, dl407);
            dl407.IdPensione = idPensione;
            
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {                            
                DAGestioneDL407.SalvaDL407(dl407);
                transactionScope.Complete();
            }
        }

        public static void EliminaDL407ByIdPensione(long idPensione)
        {
            DAGestioneDL407.EliminaDL407ByIdPensione(idPensione);
        }

        #region Nested Class

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

            private System.Nullable<byte> _ServizioUtileAAQuotaA;

            private System.Nullable<decimal> _RetribPensQuotaA;

            private System.Nullable<decimal> _RetribPensSL336QuotaA;

            private System.Nullable<byte> _ServizioUtileAAQuotaB;

            private System.Nullable<decimal> _RetribPensQuotaB;

            private System.Nullable<decimal> _RetribPensSL336QuotaB;

            private System.Nullable<byte> _ServizioUtileAAQuotaC;

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

            public System.Nullable<byte> ServizioUtileAAQuotaA { get { return _ServizioUtileAAQuotaA; } set { _ServizioUtileAAQuotaA = value; } }

            public System.Nullable<decimal> RetribPensQuotaA { get { return _RetribPensQuotaA; } set { _RetribPensQuotaA = value; } }

            public System.Nullable<decimal> RetribPensSL336QuotaA { get { return _RetribPensSL336QuotaA; } set { _RetribPensSL336QuotaA = value; } }

            public System.Nullable<byte> ServizioUtileAAQuotaB { get { return _ServizioUtileAAQuotaB; } set { _ServizioUtileAAQuotaB = value; } }

            public System.Nullable<decimal> RetribPensQuotaB { get { return _RetribPensQuotaB; } set { _RetribPensQuotaB = value; } }

            public System.Nullable<decimal> RetribPensSL336QuotaB { get { return _RetribPensSL336QuotaB; } set { _RetribPensSL336QuotaB = value; } }

            public System.Nullable<byte> ServizioUtileAAQuotaC { get { return _ServizioUtileAAQuotaC; } set { _ServizioUtileAAQuotaC = value; } }

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
                    
                    !this._ServizioUtileAAQuotaA.HasValue &&
                    !this._RetribPensQuotaA.HasValue &&
                    !this._RetribPensSL336QuotaA.HasValue &&
                    !this._ServizioUtileAAQuotaB.HasValue &&
                    !this._RetribPensQuotaB.HasValue &&
                    !this._RetribPensSL336QuotaB.HasValue &&
                    !this._ServizioUtileAAQuotaC.HasValue
                    )
                    return true;
                else
                    return false;
            }

            public bool IsDL407NullForAnteArm()
            {
                if (!this._ServizioUtileAAQuotaA.HasValue &&
                    !this._RetribPensQuotaA.HasValue &&
                    !this._RetribPensSL336QuotaA.HasValue &&
                    !this._ServizioUtileAAQuotaB.HasValue &&
                    !this._RetribPensQuotaB.HasValue &&
                    !this._RetribPensSL336QuotaB.HasValue &&
                    !this._ServizioUtileAAQuotaC.HasValue
                    )
                    return true;
                else
                    return false;
            }

            public bool IsDL407NullForPostArm()
            {
                if (!this._NSettimaneQuotaA.HasValue &&
                    !this._NSettimaneQuotaB.HasValue &&
                    !this._NSettimaneQuotaC.HasValue &&
                    !this._NSettimaneQuotaD.HasValue &&
                    !this._RMSQuotaA.HasValue &&
                    !this._RMSQuotaB.HasValue &&
                    !this._RMSQuotaD.HasValue 
                    )
                    return true;
                else
                    return false;
            }
        }

        #endregion Nested Class
    }
}
