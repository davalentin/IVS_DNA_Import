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
    public class GestioneRipartizioneFondi
    {
        public static void SalvaRipartizioneFondi(DatiRipartizioneFondi datiRipartizioneFondi)
        {
            RipartizioneFondi ripartizioneFondiDB = new RipartizioneFondi();
            BLCommon.Utility.ValorizzaOggetti(datiRipartizioneFondi, ripartizioneFondiDB);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DARipartizioneFondi.SalvaRipartizioneFondi(ripartizioneFondiDB);
                transactionScope.Complete();
            }
        }

        public static void GetRipartizioneFondiByIdPensione(long idPensione, out List<DatiRipartizioneFondi> LdatiRipartizioneFondi)
        {
            LdatiRipartizioneFondi = null;
            List<RipartizioneFondi> LRipartizioneFondiDB = null;
            
            DARipartizioneFondi.GetRipartizioneFondiByIdPensione(idPensione, out LRipartizioneFondiDB);
            if (LRipartizioneFondiDB != null && LRipartizioneFondiDB.Count > 0)
            {
                LdatiRipartizioneFondi = new List<DatiRipartizioneFondi>();
                foreach (RipartizioneFondi rf in LRipartizioneFondiDB)
                {
                    DatiRipartizioneFondi datiRipartizioneFondi = new DatiRipartizioneFondi();
                    BLCommon.Utility.ValorizzaOggetti(rf, datiRipartizioneFondi);
                    LdatiRipartizioneFondi.Add(datiRipartizioneFondi);
                }
            }
        }

        public static void EliminaRipartizioneFondiByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DARipartizioneFondi.EliminaRipartizioneFondiByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class DatiRipartizioneFondi
        {
            #region Private
            private long _IdPensione;
            private int? _CodiceAltroFondo;
            private decimal? _Importo;
            private int? _Progressivo;
            #endregion Private

            #region Public
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int? CodiceAltroFondo { get { return _CodiceAltroFondo; } set { _CodiceAltroFondo = value; } }
            public decimal? Importo { get { return _Importo; } set { _Importo = value; } }
            public int? Progressivo { get { return _Progressivo; } set { _Progressivo = value; } }
            #endregion Public
        }

        #endregion nested class
    }
}
