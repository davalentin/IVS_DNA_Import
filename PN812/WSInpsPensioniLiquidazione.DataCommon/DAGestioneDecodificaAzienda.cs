using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDecodificaAzienda
    {
        /// <summary>
        /// inserimento aziende
        /// metodo di insert azienda, richiama stored procedure
        /// </summary>
        /// <param name="azienda"></param>
        public static void InsertDecodificaAzienda(DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDecodificaAzienda(azienda.TraduzioneSuGP, azienda.Descrizione, azienda.SiglaCategoria);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Azienda già presente");
                }
                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure Insert Azienda");
                }
                db.Connection.Close();
            }
        }

        public static void GetDecodificaAziendaBySiglaCategoria(Expression<Func<DecodificaAzienda, bool>> whereCondition, out List<DecodificaAzienda> elencoDecodificaAziendaEditoria)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendaEditoria = (from d in db.DecodificaAziendas
                                                       //where siglaCategoria != null ? d.SiglaCategoria == siglaCategoria : d.SiglaCategoria == null
                                                       select d).Where(whereCondition).ToList<DecodificaAzienda>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static DecodificaAzienda GetDecodificaAziendaById(short id)
        {
            DecodificaAzienda result = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    result = (from d in db.DecodificaAziendas where d.Id == id select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
            return result;
        }

        public static void GetDecodificaAziendaAll(out List<DecodificaAzienda> elencoDecodificaAziendaEditoria)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendaEditoria = (from d in db.DecodificaAziendas select d).ToList<DecodificaAzienda>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
