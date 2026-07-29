using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneMessaggiHermes
    {
        public static void GetMessaggiHermesAttivi(string tipoApp, out List<MessaggiHerme> elencoMessaggiHermes)
        {
            elencoMessaggiHermes = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        elencoMessaggiHermes = (from a in db.MessaggiHermes where a.Attivo && a.Tipologia == tipoApp select a).OrderByDescending(x => x.TimeStamp).ToList<MessaggiHerme>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }

        public static void GetAllMessaggiHermes(string tipoApp, out List<MessaggiHerme> elencoMessaggiHermes)
        {
            elencoMessaggiHermes = null;
            if (!string.IsNullOrEmpty(tipoApp))
            {
                using (new MethodExecutionTracer())
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                    {
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        elencoMessaggiHermes = (from a in db.MessaggiHermes where a.Tipologia == tipoApp select a).OrderByDescending(x => x.TimeStamp).ToList<MessaggiHerme>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                }
            }
        }

        public static void SalvaMessaggioHermes(MessaggiHerme messaggioHermes)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.InsertMessaggioHermes(messaggioHermes.Id, messaggioHermes.Titolo, messaggioHermes.Testo,
                    messaggioHermes.Url, messaggioHermes.Categoria, messaggioHermes.Attivo, messaggioHermes.Tipologia, messaggioHermes.TimeStamp);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPMessaggioHermes");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteMessaggioHermes(long idMessaggioHermes)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                int result = db.DeleteMessaggioHermes(idMessaggioHermes);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("ErrorSPDeleteMessaggioHermes");
                }
                db.Connection.Close();
            }
        }
    }
}
