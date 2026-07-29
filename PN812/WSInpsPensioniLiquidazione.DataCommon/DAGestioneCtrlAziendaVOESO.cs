using INPS.DNA.Data;
using INPS.DNA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendaVOESO
    {
        public static void GetDecodificaAziendaVOESO(out List<CtrlAziendaVOESO> elencoDecodificaAziendaVOESO)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendaVOESO = (from d in db.CtrlAziendaVOESOs select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAziendaVOESOByIdCodiceAzienda(short idAzienda, out CtrlAziendaVOESO aziendaVOESO)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    aziendaVOESO = (from d in db.CtrlAziendaVOESOs where d.CodiceAzienda == idAzienda select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeVOESO(CtrlAziendaVOESO aziendaVOESO, DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeVOESO(azienda.TraduzioneSuGP, azienda.Descrizione, aziendaVOESO.UltimaDecorrenzaAmmessa, aziendaVOESO.Tipo);
                if (result != 0)
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendeVOESO");

                db.Connection.Close();
            }
        }

        public static void DeleteAziendaVOESO(string codiceAziendaTraduzionesuGP, string tipo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAziendeVOESO(codiceAziendaTraduzionesuGP, tipo);
                if (result == -1)
                    throw new DNA.DnaValidationException("Azienda in uso, impossibile eliminare");
                else if (result != 0)
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAziendeVOESO");

                db.Connection.Close();
            }
        }
    }
}
