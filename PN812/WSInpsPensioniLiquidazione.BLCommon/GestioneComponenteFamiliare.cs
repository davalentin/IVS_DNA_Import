using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneComponenteFamiliare
    {
        public static void GetComponenteFamiliareByIdPensione(long idPensione, out List<ComponenteFamiliare> componentiFamiliari)
        {
            componentiFamiliari = null;
            List<DataCommon.ComponenteFamiliare> componentiFamililariDb = new List<DataCommon.ComponenteFamiliare>();
            DAGestioneComponenteFamiliare.GetComponenteFamiliareByIdPensione(idPensione, out componentiFamililariDb);
            if (componentiFamililariDb != null && componentiFamililariDb.Count > 0)
            {
                componentiFamiliari = new List<ComponenteFamiliare>();

                foreach (DataCommon.ComponenteFamiliare componenteFamiliareDb in componentiFamililariDb)
                {
                    ComponenteFamiliare componenteFamiliare = new ComponenteFamiliare();
                    Utility.ValorizzaOggetti(componenteFamiliareDb, componenteFamiliare);
                    componentiFamiliari.Add(componenteFamiliare);
                }
            }
        }

        public static void GetComponenteFamiliareByIdRecordDatiNoCalcolo(long idRecordDatiNoCalcolo, out List<ComponenteFamiliare> componentiFamiliari)
        {
            componentiFamiliari = null;
            List<DataCommon.ComponenteFamiliare> componentiFamililariDb = new List<DataCommon.ComponenteFamiliare>();
            DAGestioneComponenteFamiliare.GetComponenteFamiliareByIdRecordDatiNoCalcolo(idRecordDatiNoCalcolo, out componentiFamililariDb);
            if (componentiFamililariDb != null && componentiFamililariDb.Count > 0)
            {
                componentiFamiliari = new List<ComponenteFamiliare>();

                foreach (DataCommon.ComponenteFamiliare componenteFamiliareDb in componentiFamililariDb)
                {
                    ComponenteFamiliare componenteFamiliare = new ComponenteFamiliare();
                    Utility.ValorizzaOggetti(componenteFamiliareDb, componenteFamiliare);
                    componentiFamiliari.Add(componenteFamiliare);
                }
            }
        }

        public static void SalvaComponenteFamiliare(ComponenteFamiliare componenteFamiliare)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.ComponenteFamiliare componenteFamiliareDb = new DataCommon.ComponenteFamiliare();
                Utility.ValorizzaOggetti(componenteFamiliare, componenteFamiliareDb);
                DAGestioneComponenteFamiliare.SalvaComponenteFamiliare(componenteFamiliareDb);
                transactionScope.Complete();
            }
        }

        public static void EliminaComponentiFamiliariByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                 new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneComponenteFamiliare.DeleteComponentiFamiliariByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaComponentiFamiliariByIdRecordDatiNoCalcolo(long idRecordDatiNoCalcolo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                 new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneComponenteFamiliare.DeleteComponentiFamiliariByIdRecordDatiNoCalcolo(idRecordDatiNoCalcolo);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class ComponenteFamiliare
        {
            public long Id { get; set; }
            public long IdPensione { get; set; }
            public long IdRecordDatiNoCalcolo { get; set; }
            public string CodiceFiscale { get; set; }
        }

        #endregion nested class
    }
}
