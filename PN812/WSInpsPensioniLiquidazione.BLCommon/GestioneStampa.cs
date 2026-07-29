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
    public class GestioneStampa
    {
        public static void GetStampaByIdPensione(Int64 idPensione, out DatiStampa datiStampa)
        {
            Stampa stampa = null;
            datiStampa = null;
            DAGestioneStampa.GetStampaByIdPensione(idPensione, out stampa);
            if (stampa == null)
                return;
            datiStampa = new DatiStampa();
            Utility.ValorizzaOggetti(stampa, datiStampa);
        }

        public static void SalvaStampa(long idPensione, DatiStampa datiStampa)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Stampa stampa = new Stampa();
                Utility.ValorizzaOggetti(datiStampa, stampa);
                stampa.IdPensione = idPensione;
                DAGestioneStampa.SalvaStampa(stampa);

                transactionScope.Complete();
            }
        }

        public static void EliminaStampaByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneStampa.EliminaStampaByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiStampa
        {
            public DatiStampa()
            { }

            public DatiStampa(System.Data.Linq.Binary pdf)
            {
                this._PDF = pdf;
            }

            #region private properties
            private System.Data.Linq.Binary _PDF;
            #endregion private properties

            #region public properties
            public System.Data.Linq.Binary PDF { get { return _PDF; } set { _PDF = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
