using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendeVESO33
    {
        public static void GetDecodificaAziendeVESO33(out List<CtrlAziendaVESO33> elencoDecodificaAziende)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziende = (from d in db.CtrlAziendaVESO33s select d).ToList<CtrlAziendaVESO33>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAziendaVESO33ByIdCodiceAzienda(short idAzienda, out CtrlAziendaVESO33 aziendaVESO33)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    aziendaVESO33 = (from d in db.CtrlAziendaVESO33s where d.CodiceAzienda == idAzienda select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeVESO33(CtrlAziendaVESO33 aziendaVESO33, DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeVESO33(azienda.TraduzioneSuGP, azienda.Descrizione, aziendaVESO33.UltimaDecorrenzaAmmessa);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendaVESO33");

                db.Connection.Close();
            }
        }

         public static void DeleteAziendaVESO33(string codiceAziendaTraduzionesuGP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAziendeVESO33(codiceAziendaTraduzionesuGP);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Azienda in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAziendaVESO33");
                }
                db.Connection.Close();
            }
        }
    }
}
