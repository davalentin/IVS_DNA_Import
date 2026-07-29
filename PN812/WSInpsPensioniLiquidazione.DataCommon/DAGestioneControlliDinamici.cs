using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneControlliDinamici
    {
        public static void GetControlliDinamici(out List<ControlliDinamici> elencoControlliDinamici)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoControlliDinamici = (from c in db.ControlliDinamicis select c).ToList<ControlliDinamici>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetControlloDinamicoByNomeControllo(string nomeControllo, out ControlliDinamici controlloDinamico)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    controlloDinamico = (from c in db.ControlliDinamicis where c.NomeControllo == nomeControllo select c).SingleOrDefault<ControlliDinamici>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaControlloDinamico(ControlliDinamici controlloDinamico)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertControlliDinamici(controlloDinamico.NomeControllo, controlloDinamico.ValoreControllo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertControlliDinamici");
                }
                db.Connection.Close();
            }
        }
    }
}
