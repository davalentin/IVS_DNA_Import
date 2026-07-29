using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCasualiDebito
    {
        static public bool UpdateCasuali(long indebito, List<CasualiDebito> casuali)
        {
            try
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                    {
                        using (PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString")))
                        {
                            List<CasualiDebito> casualiDebito = db.CasualiDebitos.Where(x => x.Indebito.Equals(indebito)).ToList();
                            db.CasualiDebitos.DeleteAllOnSubmit(casualiDebito);
                            casualiDebito.Clear();
                            foreach (CasualiDebito casuale in casuali)
                                casualiDebito.Add(new CasualiDebito()
                                {
                                    CasualeAnalitica = casuale.CasualeAnalitica,
                                    CasualeSintetica = casuale.CasualeSintetica,
                                    ContoRecupero = casuale.ContoRecupero,
                                    Importo = casuale.Importo,
                                    Indebito = indebito,
                                    Id = casuale.Id
                                });
                            db.CasualiDebitos.InsertAllOnSubmit(casualiDebito);
                            db.SubmitChanges();
                            transactionScope.Complete();
                        }
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        static public bool InsertCasuali(long indebito, List<CasualiDebito> casuali)
        {
            try
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                    {
                        using (PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString")))
                        {
                            foreach (CasualiDebito casuale in casuali)
                                db.CasualiDebitos.InsertOnSubmit(new CasualiDebito()
                                {
                                    CasualeAnalitica = casuale.CasualeAnalitica,
                                    CasualeSintetica = casuale.CasualeSintetica,
                                    ContoRecupero = casuale.ContoRecupero,
                                    Importo = casuale.Importo,
                                    Indebito = indebito,
                                    Id = casuale.Id
                                });

                            db.SubmitChanges();
                            transactionScope.Complete();
                        }
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
