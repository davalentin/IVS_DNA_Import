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
    public class DAGestioneRecordFondo
    {
        public static void SalvaRecordFondo(long idPensione, List<RecordFondo> listaRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = null;
                db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                //E' necessario, trattandosi di una lista, eliminare dal db prima gli eventuali Id non presenti in listaRecordFondo ma presenti sul db
                //prima di procedere con il salvataggio della lista
                List<RecordFondo> listaRecordFondoOriginale = null;
                GetRecordFondo(idPensione, out listaRecordFondoOriginale);
                if (listaRecordFondoOriginale != null && listaRecordFondoOriginale.Count > 0)
                {
                    List<long> listaIdDaRimuovere = new List<long>();
                    foreach (RecordFondo fondoOriginale in listaRecordFondoOriginale)
                    {
                        bool isPresente = false;
                        if (listaRecordFondo != null)
                        {
                            foreach (RecordFondo fondo in listaRecordFondo)
                            {
                                if (fondoOriginale.Id == fondo.Id)
                                    isPresente = true;
                            }
                        }
                        if (!isPresente)
                            listaIdDaRimuovere.Add(fondoOriginale.Id);
                    }

                    foreach (long id in listaIdDaRimuovere)
                    {
                        DeleteRecordFondo(id);
                    }
                }
                if (listaRecordFondo != null)
                {
                    foreach (RecordFondo recordFondo in listaRecordFondo)
                    {
                        long? idRecordFondo = null;
                        db.InsertRecordFondo(recordFondo.Id, recordFondo.IdPensione, recordFondo.CodiceNatura1, recordFondo.CodiceNatura2, recordFondo.CodiceNatura3, recordFondo.CodiceNonCalcolo,
                            recordFondo.DecorrenzaValiditaDati, recordFondo.DataSospensione, ref idRecordFondo);
                        recordFondo.Id = idRecordFondo.HasValue ? idRecordFondo.Value : 0;
                    }
                }

                db.Connection.Close();
            }
        }

        public static void SalvaSingoloRecordFondo(RecordFondo recordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                if (recordFondo != null)
                {
                    long? idRecordFondo = null;
                    int result = db.InsertRecordFondo(recordFondo.Id, recordFondo.IdPensione, recordFondo.CodiceNatura1, recordFondo.CodiceNatura2, recordFondo.CodiceNatura3, recordFondo.CodiceNonCalcolo,
                        recordFondo.DecorrenzaValiditaDati, recordFondo.DataSospensione, ref idRecordFondo);
                    if (result != 0)
                    {
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertRecordFondo");
                    }
                    recordFondo.Id = idRecordFondo.HasValue ? idRecordFondo.Value : 0;
                }

                db.Connection.Close();
            }
        }

        public static void GetRecordFondo(long idPensione, out List<RecordFondo> listaRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaRecordFondo = (from rf in db.RecordFondos
                                    where rf.IdPensione == idPensione
                                    select rf).ToList<RecordFondo>();

                db.Connection.Close();
            }
        }

        public static void GetPensioneFondoDZ(List<RecordFondo> listaRecordFondo, out List<PensioneFondoDZ> listaPensioneFondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                List<Int64> idRecordFondo = listaRecordFondo.Select(x => x.Id).ToList();
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaPensioneFondoDZ = db.PensioneFondoDZs.Where(x => idRecordFondo.Contains(x.IdRecordFondo.Value)).ToList();
                db.Connection.Close();
            }
        }

        public static void GetPensioneFondoDZ(RecordFondo RecordFondo, out PensioneFondoDZ pensioneFondoDZ)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                pensioneFondoDZ = db.PensioneFondoDZs.Where(x => x.IdRecordFondo == RecordFondo.Id).FirstOrDefault();
                db.Connection.Close();
            }
        }

        public static void GetSingoloRecordFondo(long idRecordFondo, out RecordFondo recordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                recordFondo = (from rf in db.RecordFondos
                               where rf.Id == idRecordFondo
                               select rf).SingleOrDefault<RecordFondo>();

                db.Connection.Close();
            }
        }

        public static void DeleteRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.DeleteRecordFondo(idRecordFondo);
                db.Connection.Close();
            }
        }

        public static void DeleteAllRecordFondo(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllRecordFondo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroTitolare");
                }
                db.Connection.Close();
            }
        }
    }
}
