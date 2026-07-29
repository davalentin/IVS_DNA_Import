using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendaCredito
    {
        public static void GetDecodificaAziendaCredito(out List<CtrlAziendaCredito> elencoDecodificaAziendaCredito)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendaCredito = (from d in db.CtrlAziendaCreditos select d).ToList<CtrlAziendaCredito>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAziendaCreditoByIdCodiceAzienda(short idAzienda, out CtrlAziendaCredito aziendaCredito)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    aziendaCredito = (from d in db.CtrlAziendaCreditos where d.CodiceAzienda == idAzienda select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeCredito(CtrlAziendaCredito aziendaCredito, DecodificaAzienda azienda)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeCredito(azienda.TraduzioneSuGP, azienda.Descrizione, aziendaCredito.UltimaDecorrenzaAmmessa, aziendaCredito.SiglaCatPensione);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendeCredito");

                db.Connection.Close();
            }
        }

        public static void DeleteAziendaCredito(string codiceAziendaTraduzionesuGP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAziendeCredito(codiceAziendaTraduzionesuGP);
                if (result == -1)
                    throw new INPS.DNA.DnaValidationException("Azienda in uso, impossibile eliminare");
                else if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAziendeCredito");

                db.Connection.Close();
            }
        }
    }
}
