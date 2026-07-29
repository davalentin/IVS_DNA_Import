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
    public class GestioneIntegrazioneArt11
    {
        #region IntegrazioneArt11

        public static void GetIntegrazioneArt11ByIdPensione(long idPensione, out IntegrazioneArt11 integrazioneArt11)
        {
            integrazioneArt11 = null;
            DataCommon.IntegrazioneArt11 integrazioneArt11DB = null;
            DAIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(idPensione, out integrazioneArt11DB);
            if (integrazioneArt11DB != null)
                integrazioneArt11 = new IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11DB, integrazioneArt11);
        }

        public static void SalvaIntegrazioneArt11(long idPensione, IntegrazioneArt11 integrazioneArt11)
        {
            DataCommon.IntegrazioneArt11 integrazioneArt11DB = new INPS.Pensioni.Liquidazione.DataCommon.IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11, integrazioneArt11DB);
            integrazioneArt11DB.IdPensione = idPensione;
            DAIntegrazioneArt11.SalvaIntegrazioneArt11(integrazioneArt11DB);
        }

      
        public static void EliminaIntegrazioneArt11ByIdPensione(long idPensione)
        {
            DAIntegrazioneArt11.EliminaIntegrazioneArt11ByIdPensione(idPensione);
        }

        #endregion IntegrazioneArt11

        #region IntegrazioneArt11 ENPALS

        public static void GetIntegrazioneArt11ByIdRecord(long idRecord, out IntegrazioneArt11 integrazioneArt11)
        {
            integrazioneArt11 = null;
            DataCommon.IntegrazioneArt11 integrazioneArt11DB = null;
            DAIntegrazioneArt11.GetIntegrazioneArt11ByIdRecord(idRecord, out integrazioneArt11DB);
            if (integrazioneArt11DB != null)
                integrazioneArt11 = new IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11DB, integrazioneArt11);
        }


        public static void SalvaIntegrazioneArt11ByIdSuppRecordENPALS(long idPensione, long IdSuppRecordENPALS, IntegrazioneArt11 integrazioneArt11)
        {
            DataCommon.IntegrazioneArt11 integrazioneArt11DB = new INPS.Pensioni.Liquidazione.DataCommon.IntegrazioneArt11();
            Utility.ValorizzaOggetti(integrazioneArt11, integrazioneArt11DB);
            integrazioneArt11DB.IdPensione = idPensione;
            integrazioneArt11DB.IdSuppRecordENPALS = IdSuppRecordENPALS;
            DAIntegrazioneArt11.SalvaIntegrazioneArt11ByIdSuppRecordENPALS(integrazioneArt11DB);
        }

        public static void EliminaIntegrazioneArt11ByIdSuppRecordENPALS(long IdSuppRecordENPALS)
        {
            DAIntegrazioneArt11.EliminaIntegrazioneArt11ByIdSuppRecordENPALS(IdSuppRecordENPALS);
        }

        #endregion IntegrazioneArt11 ENPALS

        #region nested class

        public class IntegrazioneArt11
        {
            //private long _IdPensione;
            private decimal? _ImportoIVS;
            private DateTime? _Decorrenza;
            private System.Nullable<long> _IdSuppRecordENPALS;
            
            //public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public System.Nullable<long> IdSuppRecordENPALS { get { return _IdSuppRecordENPALS; } set { _IdSuppRecordENPALS = value; } }

            public override bool Equals(object obj)
            {
                IntegrazioneArt11 integrazioneArt11 = (IntegrazioneArt11)obj;
                try
                {
                    if (this._ImportoIVS != integrazioneArt11.ImportoIVS ||
                        this._Decorrenza != integrazioneArt11.Decorrenza)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion nested class
    }
}
