using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneTipologieNonAbilitate
    {
        public static void SalvaTipologieNonAbilitate(TipologieNonAbilitate tipologiaNonAbilitata)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertTipologieNonAbilitate(tipologiaNonAbilitata.TipoApp, tipologiaNonAbilitata.Fondo, tipologiaNonAbilitata.Gruppo, tipologiaNonAbilitata.Prodotto, tipologiaNonAbilitata.Tipo,
                    tipologiaNonAbilitata.Filtro, tipologiaNonAbilitata.SiglaCategoria);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record già presente nel database");
                }
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertTipologieNonAbilitate");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaTipologieNonAbilitate(TipologieNonAbilitate tipologiaNonAbilitata)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteTipologieNonAbilitate(tipologiaNonAbilitata.TipoApp, tipologiaNonAbilitata.Fondo, tipologiaNonAbilitata.Gruppo, 
                    tipologiaNonAbilitata.Prodotto, tipologiaNonAbilitata.Tipo, tipologiaNonAbilitata.Filtro, tipologiaNonAbilitata.SiglaCategoria);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteTipologieNonAbilitate");
                }
                db.Connection.Close();
            }
        }

        public static void GetAllTipologieNonAbilitate(out List<TipologieNonAbilitate> elencoTipologieNonAbilitate)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipologieNonAbilitate = (from l in db.TipologieNonAbilitates select l).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
