using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlBypassTipologieNonAbilitate
    {
        public static void GetCtrlBypassTipologieNonAbilitate(string tipologia, short sede, string gruppo, string prodotto, string tipo, string categoria, string filtro, string fondo, out List<CtrlBypassTipologieNonAbilitate> lstCtrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    //query build
                    IEnumerable<CtrlBypassTipologieNonAbilitate> lstResult = (from cr in db.CtrlBypassTipologieNonAbilitates select cr)
                    .Where(x => x.Tipologia == tipologia)
                    .Where(x => x.Sede == sede)
                    .Where(x => x.Gruppo.Trim().PadLeft(4, '0') == gruppo)
                    .Where(x => x.Prodotto.Trim().PadLeft(4, '0') == prodotto || x.Prodotto == "ALL")
                    .Where(x => x.Tipo.Trim().PadLeft(4, '0') == tipo || x.Tipo == "ALL")
                    .Where(x => x.Categoria.Trim() == (categoria ?? string.Empty).Trim() || x.Categoria == "ALL")
                    .Where(x => filtro == "ALL" || x.Filtro.Trim() == (filtro ?? string.Empty).Trim() || x.Filtro == "ALL")
                    .Where(x => (x.Fondo == null && fondo == null) || x.Fondo == fondo || x.Fondo == "ALL");
                    //query to db
                    lstCtrl = lstResult.ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlBypassTipologieNonAbilitate(out List<CtrlBypassTipologieNonAbilitate> elencoCtrlBypassTipologieNonAbilitate)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCtrlBypassTipologieNonAbilitate = (from cr in db.CtrlBypassTipologieNonAbilitates select cr).ToList<CtrlBypassTipologieNonAbilitate>();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void StoreCtrlBypassTipologieNonAbilitate(CtrlBypassTipologieNonAbilitate ctrlBypassTipologieNonAbilitate)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.InsertCtrlBypassTipologieNonAbilitate(ctrlBypassTipologieNonAbilitate.Tipologia, ctrlBypassTipologieNonAbilitate.Sede, ctrlBypassTipologieNonAbilitate.Gruppo,
                    ctrlBypassTipologieNonAbilitate.Prodotto, ctrlBypassTipologieNonAbilitate.Tipo, ctrlBypassTipologieNonAbilitate.Categoria, ctrlBypassTipologieNonAbilitate.Filtro,
                    ctrlBypassTipologieNonAbilitate.Fondo);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record già presente nel database");
                }
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertCtrlBypassTipologieNonAbilitate");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaCtrlBypassTipologieNonAbilitate(CtrlBypassTipologieNonAbilitate ctrlBypassTipologieNonAbilitate)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteCtrlBypassTipologieNonAbilitate(ctrlBypassTipologieNonAbilitate.Tipologia, ctrlBypassTipologieNonAbilitate.Sede, ctrlBypassTipologieNonAbilitate.Gruppo,
                    ctrlBypassTipologieNonAbilitate.Prodotto, ctrlBypassTipologieNonAbilitate.Tipo, ctrlBypassTipologieNonAbilitate.Categoria, ctrlBypassTipologieNonAbilitate.Filtro,
                    ctrlBypassTipologieNonAbilitate.Fondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCtrlBypassTipologieNonAbilitate");
                }
                db.Connection.Close();
            }
        }
    }
}
