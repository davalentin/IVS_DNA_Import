using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneSentenzaArt4
    {
        public static void SalvaSentenzaArt4(long idPensione, DatiSentenzaArt4 sentenzaArt4)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SentenzaArt4 datiSentenzaArt4 = new SentenzaArt4();
                Utility.ValorizzaOggetti(sentenzaArt4, datiSentenzaArt4);
                datiSentenzaArt4.IdPensione = idPensione;
                DAGestioneSentenzaArt4.SalvaSentenzaArt4(datiSentenzaArt4);
                transactionScope.Complete();
            }
        }

        public static void GetDatiSentenzaArt4(Int64 idPensione, out List<DatiSentenzaArt4> lSentenzaArt4)
        {
            List<SentenzaArt4> lDatiSentenzaArt4DB = null;
            lSentenzaArt4 = null;
            DAGestioneSentenzaArt4.GetDatiSentenzaArt4(idPensione, out lDatiSentenzaArt4DB);
            if (lDatiSentenzaArt4DB == null)
                return;
            lSentenzaArt4 = new List<DatiSentenzaArt4>();
            foreach (SentenzaArt4 datiSentenzaArt4DB in lDatiSentenzaArt4DB)
            {
                DatiSentenzaArt4 datiSentenzaArt4 = new DatiSentenzaArt4();
                Utility.ValorizzaOggetti(datiSentenzaArt4DB, datiSentenzaArt4);
                lSentenzaArt4.Add(datiSentenzaArt4);
            }
        }

        public static void EliminaDatiSentenzaArt4ByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSentenzaArt4.EliminaDatiSentenzaArt4ByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiSentenzaArt4NoGPByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSentenzaArt4.EliminaDatiSentenzaArt4NoGPByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiSentenzaArt4
        {
            #region private poperties
            private long _Id;

            private long _IdPensione;

            private DateTime? _DecorrenzaSentenza;

            private decimal? _ImportoSentenza;

            private bool _IsFromGP;

            #endregion private poperties

            #region public poperties

            public long Id { get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public DateTime? DecorrenzaSentenza { get { return _DecorrenzaSentenza; } set { _DecorrenzaSentenza = value; } }

            public decimal? ImportoSentenza { get { return _ImportoSentenza; } set { _ImportoSentenza = value; } }

            public bool IsFromGP { get { return _IsFromGP; } set { _IsFromGP = value; } }

            #endregion public poperties

            public bool IsNull()
            {
                if (!DecorrenzaSentenza.HasValue &&
                    !ImportoSentenza.HasValue)
                    return true;

                return false;
            }
        }
        #endregion nested class
    }
}
