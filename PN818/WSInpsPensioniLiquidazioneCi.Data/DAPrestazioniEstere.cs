using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Transactions;
using System.Globalization;
using INPS.DNA.Data;
using INPS.DNA.Logging;

namespace INPS.Pensioni.LiquidazioneCi.Data
{
    public class DAPrestazioniEstere
    {
        public static void GetPrestazioneEstera(string codiceStatoEstero, out aciistit prestazioneEstera)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    CIBaseDataContext db = new CIBaseDataContext(ConnectionFactory.GetConnection("CIBaseConnectionString"));
                    prestazioneEstera = (from a in db.aciistits where a.CDSTAIST == codiceStatoEstero select a).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
