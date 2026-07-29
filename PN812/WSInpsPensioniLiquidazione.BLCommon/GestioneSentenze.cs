using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneSentenze
    {
        public static void SalvaSentenze(long idPensione, DatiSentenze sentenze)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Sentenze datiSentenze = new Sentenze();
                Utility.ValorizzaOggetti(sentenze, datiSentenze);
                datiSentenze.IdPensione = idPensione;
                DAGestioneSentenze.SalvaSentenze(datiSentenze);
                transactionScope.Complete();
            }
        }

        public static void GetDatiSentenze(Int64 idPensione, out List<DatiSentenze> lSentenze)
        {
            List<Sentenze> lDatiSentenzeDB = null;
            lSentenze = null;
            DAGestioneSentenze.GetDatiSentenze(idPensione, out lDatiSentenzeDB);
            if (lDatiSentenzeDB == null)
                return;
            lSentenze = new List<DatiSentenze>();
            foreach (Sentenze datiSentenzeDB in lDatiSentenzeDB)
            {
                DatiSentenze datiSentenze = new DatiSentenze();
                Utility.ValorizzaOggetti(datiSentenzeDB, datiSentenze);
                lSentenze.Add(datiSentenze);
            }
        }

        public static void EliminaDatiSentenzeIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSentenze.EliminaDatiSentenzeByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }
        #region nested class

        public class DatiSentenze
        {
            #region private properties
            private long _Id;
            private long _IdPensione;
            private string _CodSentenzaMerito;
            private string _CodSentenza;
            private DateTime? _DecorrenzaDal;
            private DateTime? _DecorrenzaAl;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public string CodSentenzaMerito { get { return _CodSentenzaMerito; } set { _CodSentenzaMerito = value; } }
            public string CodSentenza { get { return _CodSentenza; } set { _CodSentenza = value; } }
            public DateTime? DecorrenzaDal { get { return _DecorrenzaDal; } set { _DecorrenzaDal = value; } }
            public DateTime? DecorrenzaAl { get { return _DecorrenzaAl; } set { _DecorrenzaAl = value; } }
            #endregion public properties

            public bool IsNull()
            {
                if (!string.IsNullOrEmpty(CodSentenzaMerito) &&
                    !string.IsNullOrEmpty(CodSentenza) &&
                    !DecorrenzaDal.HasValue &&
                    !DecorrenzaAl.HasValue)
                    return true;

                return false;
            }
        }
        #endregion nested class
    }
}
