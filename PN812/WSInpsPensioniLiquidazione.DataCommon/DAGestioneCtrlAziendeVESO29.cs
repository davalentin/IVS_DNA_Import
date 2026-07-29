using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendeVESO29
    {
        public static void GetDecodificaAziendeVESO29(out List<CtrlAziendaVESO29> elencoDecodificaAziende)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziende = (from d in db.CtrlAziendaVESO29s select d).ToList<CtrlAziendaVESO29>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAziendaVESO29ByIdCodiceAzienda(short idAzienda, out CtrlAziendaVESO29 aziendaVESO29)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    aziendaVESO29 = (from d in db.CtrlAziendaVESO29s where d.CodiceAzienda == idAzienda select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeVESO29(CtrlAziendaVESO29 aziendaVESO29, DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeVESO29(azienda.TraduzioneSuGP, azienda.Descrizione, aziendaVESO29.UltimaDecorrenzaAmmessa);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendaVESO29");

                db.Connection.Close();
            }
        }

        public static void DeleteAziendaVESO29(string codiceAziendaTraduzionesuGP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAziendeVESO29(codiceAziendaTraduzionesuGP);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Azienda in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAziendaVESO29");
                }
                db.Connection.Close();
            }
        }
    }
}
