using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneBancheFideiussioneESPA
    {
        /// <summary>
        /// metodo che fa la get delle banche dalla tabella DecodificaBancaFideiussoriaESPA
        /// </summary>
        /// <param name="elencoDecodificaBancaFideiussione"></param>
        public static void GetDecodificaBancaFideiussione(out List<DecodificaBancaFideiussoriaESPA> elencoDecodificaBancaFideiussione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaBancaFideiussione = (from d in db.DecodificaBancaFideiussoriaESPAs select d).ToList<DecodificaBancaFideiussoriaESPA>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di banche fideiussorie
        /// </summary>
        /// <param name="bancaFideiussoria"></param>
        public static void SalvaBancaFideiussoria(DecodificaBancaFideiussoriaESPA bancaFideiussoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertBancaFideiussioneESPA(bancaFideiussoria.Id, bancaFideiussoria.CodiceAzienda, bancaFideiussoria.Matricola, bancaFideiussoria.BancaFideiussione, bancaFideiussoria.Progressivo, bancaFideiussoria.Anno, bancaFideiussoria.InizioEsodo, bancaFideiussoria.FineEsodo, bancaFideiussoria.ABI, bancaFideiussoria.CAB);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertBancafideiussoriaESPA");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteBancheFideiussioneESPA
        /// </summary>
        /// <param name="bancaFideiussoria"></param>
        public static void DeleteBancaFideiussoria(DecodificaBancaFideiussoriaESPA bancaFideiussoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteBancaFideiussioneESPA(bancaFideiussoria.Id, bancaFideiussoria.CodiceAzienda, bancaFideiussoria.Progressivo, bancaFideiussoria.Anno);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteBancafideiussoriaESPA");
                }
                db.Connection.Close();
            }
        }
    }
}
