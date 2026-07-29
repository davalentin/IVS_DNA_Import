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
    public class DAGestionePensioneInailInabilita
    {
        #region PensioneInail

        public static void GetPensioneInailByIdPensione(long IdPensione, out List<PensioniINAIL> LpensioniInail)
        {
            LpensioniInail = new List<PensioniINAIL>();
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LpensioniInail = (from pi in db.PensioniINAILs where pi.IdPensione == IdPensione select pi).ToList<PensioniINAIL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPensioneInail(PensioniINAIL pensioniInail)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertPensioniINAIL(pensioniInail.IdPensione, pensioniInail.DecorrenzaRenditaInail,
                                                    pensioniInail.ImportoMensileInail, pensioniInail.Evento);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneInail");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaPensioneInailByIdPensione(long IdPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeletePensioniINAIL(IdPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneInail");
                }
                db.Connection.Close();
            }
        }

        #endregion PensioneInail

        #region Inabilita

        public static void GetInabilita(long IdPensione, out Inabilita inabilita)
        {
            inabilita = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    inabilita = (from ina in db.Inabilitas where ina.IdPensione == IdPensione select ina).SingleOrDefault<Inabilita>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaInabilita(Inabilita inabilita)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertInabilita(inabilita.IdPensione, inabilita.DirittoAssegnoAccompagnamento, inabilita.DecorrenzaAssegnoAccompangamento, inabilita.CessazioneAssegnoAccompangamento, 
                                                inabilita.ImportoMensile, inabilita.SospensionePensioneInvalidita, inabilita.RipristinoPensioneInvalidita,
                                                inabilita.DecorrenzaDirittoIntegrazioneMinimo, inabilita.CessazioneDirittoIntegrazioneMinimo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertInabilita");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaInabilita(long IdPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteInabilita(IdPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteInabilita");
                }
                db.Connection.Close();
            }
        }

        #endregion Inabilita
    }
}
