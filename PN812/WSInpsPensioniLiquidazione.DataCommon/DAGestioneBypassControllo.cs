using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneBypassControllo
    {
        #region DecBypassControllo
        public static void GetDecBypassControlloByTipoApp(string tipoApp, out List<DecBypassControllo> lstDecBypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstDecBypassControllo = (from l in db.DecBypassControllos where l.TipoApp == tipoApp select l).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        #endregion DecBypassControllo

        #region BypassControllo
        public static void GetAllBypassControlloByTipoApp(string tipoApp, out List<BypassControllo> lstBypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstBypassControllo = (from b in db.BypassControllos
                                          join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                          where d.TipoApp == tipoApp
                                          select b).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetBypassControlloByChiavePensioneAndIdDec(string siglaCategoria, short? codiceSede, int? nCertificato, long idDecodifica, out BypassControllo bypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    bypassControllo = (from b in db.BypassControllos
                                       join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                       where b.IdDecBypassControllo == idDecodifica && (b.CodCategoria == siglaCategoria && b.CodiceSede == codiceSede && b.NCertificato == nCertificato)
                                       select b).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetBypassControlloByNDomusAndId(long? nDomus, long idDecodifica, out BypassControllo bypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    bypassControllo = (from b in db.BypassControllos
                                       join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                       where b.IdDecBypassControllo == idDecodifica && b.NDomus == nDomus
                                       select b).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAllBypassControlloByNDomus(long NDomus, out List<BypassControllo> lstBypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstBypassControllo = (from b in db.BypassControllos
                                          join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                          where b.NDomus == NDomus
                                          select b).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetBypassControlloByNDomusAndNomeBypass(long NDomus, string nome, out BypassControllo bypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                    bypassControllo = (from b in db.BypassControllos
                                       join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                       where b.NDomus == NDomus && d.Nome == nome
                                       select b).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetBypassControlloByNomeBypass(long? nDomus, string codCategoria, short? codiceSede, int? nCertificato, string nome, out BypassControllo bypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    bypassControllo = (from b in db.BypassControllos
                                       join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                       where ((b.CodCategoria == null) || ((!(b.CodCategoria == null || b.CodCategoria == "")
                                                                                && b.CodCategoria.PadLeft(4, '0') == codCategoria)
                                                                           )
                                                )
                                             && (b.CodiceSede == null || b.CodiceSede == codiceSede)
                                             && (b.NCertificato == null || b.NCertificato == nCertificato)
                                             && (b.NDomus == null || b.NDomus == nDomus)
                                             && d.Nome == nome
                                       select b
                                       ).FirstOrDefault();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        public static void GetBypassControlloByNDomusAndListNomeBypass(long NDomus, List<string> listNome, out List<BypassControllo> listBypassControllo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listBypassControllo = (from b in db.BypassControllos
                                           join d in db.DecBypassControllos on b.IdDecBypassControllo equals d.Id
                                           where b.NDomus == NDomus && listNome.Contains(d.Nome)
                                           select b).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetBypassApplicatiPerNDomus(long NDomus, out List<BypassControllo> listaBypassApplicatiPerNDomus)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaBypassApplicatiPerNDomus = (from p in db.BypassControllos
                                                 where p.NDomus == NDomus
                                                 select p).ToList<BypassControllo>();
                db.Connection.Close();
            }
        }

        public static void EliminaBypassControlloById(long id)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            int result = db.DeleteBypassControllo(id);
            if (result != 0)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteBypassControllo");
            }
            db.Connection.Close();
        }

        public static void InsertBypassControllo(BypassControllo bypassControllo)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            int result = db.InsertBypassControllo(bypassControllo.CodCategoria, bypassControllo.CodiceSede, bypassControllo.NCertificato, bypassControllo.NDomus, bypassControllo.Note, bypassControllo.Matricola, bypassControllo.Lock, bypassControllo.IdDecBypassControllo);

            if (result != 0)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertBypassControllo");
            }
            db.Connection.Close();
        }

        public static void DeleteAllBypassControlloByDomus(long id)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            int result = db.DeleteAllBypassDinamiciByNDomus(id);
            if (result != 0)
            {
                throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteBypassControllo");
            }
            db.Connection.Close();
        }
        #endregion BypassControllo
    }
}
