using System.Linq;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;

namespace INPS.Pensioni.LiquidazioneAgo.Data
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
