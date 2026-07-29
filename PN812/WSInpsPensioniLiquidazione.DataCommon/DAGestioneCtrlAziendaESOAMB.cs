using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendaESOAMB
    {
        public static void GetDecodificaAziendaESOAMB(out List<CtrlAziendaESOAMB> elencoDecodificaAziendaESOAMB)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendaESOAMB = (from d in db.CtrlAziendaESOAMBs select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAziendaESOAMBByIdCodiceAzienda(short idAzienda, out CtrlAziendaESOAMB aziendaESOAMB)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    aziendaESOAMB = (from d in db.CtrlAziendaESOAMBs where d.CodiceAzienda == idAzienda select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeESOAMB(CtrlAziendaESOAMB aziendaESOAMB, DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeESOAMB(azienda.TraduzioneSuGP, azienda.Descrizione, aziendaESOAMB.UltimaDecorrenzaAmmessa);
                if (result != 0)
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendeESOAMB");

                db.Connection.Close();
            }
        }

        public static void DeleteAziendaESOAMB(string codiceAziendaTraduzionesuGP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAziendeESOAMB(codiceAziendaTraduzionesuGP);
                if (result == -1)
                    throw new DNA.DnaValidationException("Azienda in uso, impossibile eliminare");
                else if (result != 0)
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAziendeESOAMB");

                db.Connection.Close();
            }
        }
    }
}
