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
    public class DAIntegrazioneArt11
    {
        public static void GetIntegrazioneArt11ByIdPensione(Int64 idPensione, out IntegrazioneArt11 integrazioneArt11)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    integrazioneArt11 = (from i in db.IntegrazioneArt11s
                                         where i.IdPensione == idPensione
                                         select i).FirstOrDefault<IntegrazioneArt11>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaIntegrazioneArt11(IntegrazioneArt11 integrazioneArt11)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertIntegrazioneArt11(integrazioneArt11.IdPensione, integrazioneArt11.ImportoIVS, integrazioneArt11.Decorrenza);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertIntegrazioneArt11");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaIntegrazioneArt11ByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteIntegrazioneArt11(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteIntegrazioneArt11");
                }
                db.Connection.Close();
            }
        }

        #region Gestione per IdSuppRecordENPALS per ENPALS

        public static void GetIntegrazioneArt11ByIdRecord(long idRecord, out IntegrazioneArt11 integrazioneArt11)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    integrazioneArt11 = (from i in db.IntegrazioneArt11s
                                         where i.IdSuppRecordENPALS == idRecord
                                         select i).FirstOrDefault<IntegrazioneArt11>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaIntegrazioneArt11ByIdSuppRecordENPALS(IntegrazioneArt11 integrazioneArt11)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertIntegrazioneArt11ByIdSuppRecordENPALS(integrazioneArt11.IdPensione,integrazioneArt11.IdSuppRecordENPALS, integrazioneArt11.ImportoIVS, integrazioneArt11.Decorrenza);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertIntegrazioneArt11");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaIntegrazioneArt11ByIdSuppRecordENPALS(long idSuppRecordENPALS)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteIntegrazioneArt11ByIdSuppRecordENPALS(idSuppRecordENPALS);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteIntegrazioneArt11ByIdSuppRecordENPALS");
                }
                db.Connection.Close();
            }
        }
        #endregion Gestione per IdSuppRecordENPALS per ENPALS
    }
}
