using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneAltrePensioni
    {
        public static void GetAltrePensioniByIdPensione(long idPensione, out List<AltrePensioni> LaltraPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LaltraPensione = (from a in db.AltrePensionis where a.IdPensione == idPensione select a).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void CancelAltrePensioniByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAltrePensioni(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAltrePensioni");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaAltrePensioni(AltrePensioni altraPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAltrePensioni(altraPensione.IdPensione, altraPensione.Categoria, altraPensione.Ente, altraPensione.Certificato, altraPensione.Decorrenza,
                    altraPensione.Cessazione, altraPensione.CodiceUC, altraPensione.CodiceImporto, altraPensione.ImportoAltraPensione);
                  if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAltraPensione");
                }
                db.Connection.Close();
            }
        }

        public static void VerifyCtrlBititolarita(string codCategoria, char codUC, char codImporto, string tipoApp, out List<CtrlBititolarita> Lbitit)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    Lbitit = (from c in db.CtrlBititolaritas where c.CodCategoria.Trim() == codCategoria.Trim() && c.CodiceUC.HasValue &&
                                      c.CodiceUC.Value == codUC && c.CodiceImporto.HasValue && c.CodiceImporto.Value == codImporto && c.TipoApp == tipoApp
                                      select c).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
