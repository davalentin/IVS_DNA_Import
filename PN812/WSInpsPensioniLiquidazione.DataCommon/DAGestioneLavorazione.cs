using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneLavorazione
    {
        public static void GetLavorazioneByIdPensione(Int64 idPensione, out Lavorazione lavorazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lavorazione = (from i in db.Lavoraziones
                                   where i.IdPensione == idPensione
                                   select i).SingleOrDefault<Lavorazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaLavorazione(Lavorazione lavorazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertLavorazione(lavorazione.IdPensione, lavorazione.TipoReversibilita, lavorazione.TipoLiquidazione, lavorazione.TipoDomanda, lavorazione.CodFase);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertLavorazione");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaLavorazioneByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteLavorazione(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteLavorazione");
                }
                db.Connection.Close();
            }
        }
    }
}


