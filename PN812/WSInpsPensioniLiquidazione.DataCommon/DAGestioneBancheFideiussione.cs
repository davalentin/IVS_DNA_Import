using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneBancheFideiussione
    {
        /// <summary>
        /// metodo che fa la get delle banche dalla tabella DecodificaBancaFideiussoria
        /// </summary>
        /// <param name="elencoDecodificaBancaFideiussione"></param>
        public static void GetDecodificaBancaFideiussione(out List<DecodificaBancaFideiussoria> elencoDecodificaBancaFideiussione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaBancaFideiussione = (from d in db.DecodificaBancaFideiussorias select d).ToList<DecodificaBancaFideiussoria>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure di insert e update di banche fideiussorie
        /// </summary>
        /// <param name="bancaFideiussoria"></param>
        public static void SalvaBancaFideiussoria(DecodificaBancaFideiussoria bancaFideiussoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertBancaFideiussione(bancaFideiussoria.Id, bancaFideiussoria.CodiceAzienda, bancaFideiussoria.Matricola, bancaFideiussoria.BancaFideiussione, bancaFideiussoria.Progressivo, bancaFideiussoria.Anno, bancaFideiussoria.InizioEsodo, bancaFideiussoria.FineEsodo, bancaFideiussoria.ABI, bancaFideiussoria.CAB);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile modificare");
                }

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertBancafideiussoria");
                }
                db.Connection.Close();
            }
        }

        /// <summary>
        /// metodo che richiama la stored procedure DeleteBancheFideiussione
        /// </summary>
        /// <param name="bancaFideiussoria"></param>
        public static void DeleteBancaFideiussoria(DecodificaBancaFideiussoria bancaFideiussoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteBancaFideiussione(bancaFideiussoria.Id, bancaFideiussoria.CodiceAzienda, bancaFideiussoria.Progressivo, bancaFideiussoria.Anno);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteBancafideiussoria");
                }
                db.Connection.Close();
            }
        }
    }
}
