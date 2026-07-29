using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneRipartizioneINPDAP
    {
        public static void GetRipartizioneINPDAPByIdPensione(long idPensione, out List<RipartizioneINPDAP> lRipartizioneINPDAP)
        {
            lRipartizioneINPDAP = new List<RipartizioneINPDAP>();
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lRipartizioneINPDAP = (from p in db.RipartizioneINPDAPs where p.IdPensione == idPensione select p).ToList<RipartizioneINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaRipartizioneINPDAP(RipartizioneINPDAP ripartizioneINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertRipartizioneINPDAP(ripartizioneINPDAP.IdPensione,
                                                            ripartizioneINPDAP.CodiceEnte,
                                                            ripartizioneINPDAP.Importo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertRipartizioneINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaRipartizioneINPDAPByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteRipartizioneINPDAP(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRipartizioneINPDAP");
                }
                db.Connection.Close();
            }
        }
    }
}
