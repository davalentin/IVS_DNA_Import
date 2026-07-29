using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;
using System.Linq;
using System;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneIndebito
    {
        static public bool UpdateIndebito(Indebito indebito)
        {
            try
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                    {
                        using (PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString")))
                        {
                            Indebito esistente = db.Indebitos.First<Indebito>(x => x.NDomus.Equals(indebito.NDomus));

                            esistente.NDomus = indebito.NDomus;
                            esistente.PeriodoAl = indebito.PeriodoAl;
                            esistente.PeriodoDal = indebito.PeriodoDal;

                            db.SubmitChanges();
                            transactionScope.Complete();
                        }
                    }
                }
                return true;
            } catch(Exception ex)
            {
                return false;
            }
        }

        static public bool InsertIndebito(Indebito indebito)
        {
            try
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                    {
                        using (PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"))) {
                            db.Indebitos.InsertOnSubmit(indebito);

                            db.SubmitChanges();
                            transactionScope.Complete();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static public Indebito GetIndebito(long nDomus)
        {
            try
            {
                Indebito indebito;

                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                    {
                        using (PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString")))
                        {
                            indebito = db.Indebitos.FirstOrDefault<Indebito>(x => x.NDomus.Equals(nDomus));
                            transactionScope.Complete();
                        }
                    }
                }
                return indebito;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
