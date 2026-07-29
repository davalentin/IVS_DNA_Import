using System;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Collections.Generic;
using System.Linq;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneLogGenerico
    {
        public static void SalvaLogGenerico(long numDomanda, string methodName, Utility.TipoLogGenerico tipoLog, string errore, string parametri, string stackTrace)
        {
            try
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    LogGenerico log = null;
                    var listErrore = new List<string>();

                    if (!string.IsNullOrEmpty(errore))
                    {
                        listErrore = errore.SplitByLength(43600).ToList(); // 43600 è il valore limite della clipboard per copiare il testo
                        foreach (var item in listErrore.Select((testo, index) => new { text = testo, index }))
                        {
                            log = new LogGenerico
                            {
                                NumDomanda = numDomanda,
                                LogType = tipoLog.ToString(),
                                MethodName = methodName,
                                Message = item.text,
                                Parameters = parametri,
                                StackTrace = stackTrace,
                                Progressivo = (byte)item.index
                            };
                            DAGestioneLogGenerico.SalvaLogGenerico(log);
                        }
                    }
                    else
                    {
                        log = new LogGenerico
                        {
                            NumDomanda = numDomanda,
                            LogType = tipoLog.ToString(),
                            MethodName = methodName,
                            Message = null,
                            Parameters = parametri,
                            StackTrace = stackTrace,
                            Progressivo = 0
                        };
                        DAGestioneLogGenerico.SalvaLogGenerico(log);
                    }

                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.WriteError(ex.Message);
            }
        }

        public static void EliminaLogGenerico(long numDomanda)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                DAGestioneLogGenerico.DeleteLogGenerico(numDomanda);
                transactionScope.Complete();
            }
        }
    }
}
