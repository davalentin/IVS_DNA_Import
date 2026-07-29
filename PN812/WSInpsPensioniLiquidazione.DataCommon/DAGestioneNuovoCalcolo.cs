using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneNuovoCalcolo
    {
        public static void GetRispostaNuovoCalcoloByNDomus(long Ndomus, out EsitoNuovoCalcolo esitoNuovoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    esitoNuovoCalcolo = db.EsitoNuovoCalcolos.Select(x => x).Where(x => x.NDomus == Ndomus && !x.Scaduto.GetValueOrDefault()).OrderBy(x => x.Id).FirstOrDefault();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetRispostaNuovoCalcoloByTransactionId(string TransactionId, out EsitoNuovoCalcolo esitoNuovoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    esitoNuovoCalcolo = db.EsitoNuovoCalcolos.Select(x => x).Where(x => x.TransactionId == TransactionId && !x.Scaduto.GetValueOrDefault()).OrderBy(x => x.Id).FirstOrDefault();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertOrUpdateNuovoCalcolo(EsitoNuovoCalcolo esitoNuovoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertOrUpdateNuovoCalcolo(esitoNuovoCalcolo.NDomus, esitoNuovoCalcolo.TransactionId, esitoNuovoCalcolo.Risposta);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertOrUpdateNuovoCalcolo");
                }
                db.Connection.Close();
            }
        }

        public static void GetCtrlFlowConf(out CtrlFlowConf ctrlConfFlow)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlConfFlow = db.CtrlFlowConfs.Select(x => x).FirstOrDefault();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlSedeTransazioneNuovoCalcolo(out CtrlSedeTransazioneNuovoCalcolo ctrlConfFlow)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlConfFlow = db.CtrlSedeTransazioneNuovoCalcolos.Select(x => x).FirstOrDefault();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlSedeTransazioneNuovoCalcoloBySede(string sede, out CtrlSedeTransazioneNuovoCalcolo ctrlSede)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlSede = db.CtrlSedeTransazioneNuovoCalcolos.Select(x => x).Where(x=> x.Sede == sede).FirstOrDefault();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertOrUpdateCtrlFlow(string ctrlFlowConf)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertOrUpdateCtrlFlowConf(ctrlFlowConf);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertOrUpdateNuovoCalcolo");
                }
                db.Connection.Close();
            }
        }

        public static void GetRisposteValideNuovoCalcoloByNDomus(long Ndomus, out List<EsitoNuovoCalcolo> esitoNuovoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    esitoNuovoCalcolo = db.EsitoNuovoCalcolos.Select(x => x).Where(x => x.NDomus == Ndomus && (x.Scaduto == null || x.Scaduto == false)).OrderBy(x => x.Id).ToList();
                    transactionScope.Complete();
                }
            }
        }

        public static void UpdateScadutoEsistoNuovoCalcolo(long numeroDomanda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateScadutoEsitoNuovoCalcolo(numeroDomanda);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateProgStorico");
                }
                db.Connection.Close();
            }
        }

    }
}
