using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendaESOTRA
    {
        public static void GetDecodificaAziendaESOTRA(out List<CtrlAziendaESOTRA> elencoDecodificaAziendaESOTRA)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendaESOTRA = (from d in db.CtrlAziendaESOTRAs select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAziendaESOTRAByIdCodiceAzienda(short idAzienda, out CtrlAziendaESOTRA aziendaESOTRA)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    aziendaESOTRA = (from d in db.CtrlAziendaESOTRAs where d.CodiceAzienda == idAzienda select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeESOTRA(CtrlAziendaESOTRA aziendaESOTRA, DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeESOTRA(azienda.TraduzioneSuGP, azienda.Descrizione, aziendaESOTRA.UltimaDecorrenzaAmmessa);
                if (result != 0)
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendeESOTRA");

                db.Connection.Close();
            }
        }

        public static void DeleteAziendaESOTRA(string codiceAziendaTraduzionesuGP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAziendeESOTRA(codiceAziendaTraduzionesuGP);
                if (result == -1)
                    throw new DNA.DnaValidationException("Azienda in uso, impossibile eliminare");
                else if (result != 0)
                    throw new DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAziendeESOTRA");

                db.Connection.Close();
            }
        }
    }
}
