using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneComponenteFamiliare
    {
        public static void GetComponenteFamiliareByIdPensione(long IdPensione, out List<ComponenteFamiliare> componentiFamiliari)
        {
            componentiFamiliari = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    componentiFamiliari = (from nl in db.ComponenteFamiliares
                                              where nl.IdPensione == IdPensione
                                              select nl).ToList<ComponenteFamiliare>();
                }
            }
        }

        public static void GetComponenteFamiliareByIdRecordDatiNoCalcolo(long IdRecordDatiNoCalcolo, out List<ComponenteFamiliare> componentiFamiliari)
        {
            componentiFamiliari = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    componentiFamiliari = (from nl in db.ComponenteFamiliares
                                           where nl.IdRecordDatiNoCalcolo == IdRecordDatiNoCalcolo
                                           select nl).ToList<ComponenteFamiliare>();
                }
            }
        }

        public static void SalvaComponenteFamiliare(ComponenteFamiliare componenteFamiliare)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertComponenteFamiliare(componenteFamiliare.IdPensione, componenteFamiliare.IdRecordDatiNoCalcolo, componenteFamiliare.CodiceFiscale);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertComponenteFamiliare");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteComponentiFamiliariByIdPensione(long IdPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllComponentiFamiliari(IdPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllComponentiFamiliari");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteComponentiFamiliariByIdRecordDatiNoCalcolo(long IdRecordDatiNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteComponentiFamiliariPerRecordDatiNoCalcolo(IdRecordDatiNoCalcolo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteComponentiFamiliariPerRecordDatiNoCalcolo");
                }
                db.Connection.Close();
            }
        }
    }
}
