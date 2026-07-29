using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneNoCalcolo
    {
        public static void SalvaRecordNoCalcolo(RecordDatiNoCalcolo recordNoCalcolo,ref long? idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                
                 db.InsertRecordDatiNoCalcolo(recordNoCalcolo.Id,recordNoCalcolo.IdPensione,recordNoCalcolo.Decorrenza,recordNoCalcolo.AdeguataAgo,recordNoCalcolo.AdeguataFondo
                    ,recordNoCalcolo.EccedenzaAgo,recordNoCalcolo.QuotaAgoEsclusiva,recordNoCalcolo.FacArt14,recordNoCalcolo.IndIntSpeciale,recordNoCalcolo.AssegniFamiliari,recordNoCalcolo.AggFamigliaFondo,
                    recordNoCalcolo.OnereCaricoAmm,recordNoCalcolo.Art21,recordNoCalcolo.ImportoMensile,recordNoCalcolo.Tredicesima,recordNoCalcolo.TipoVar,ref idRecordFondo);          
               
                db.Connection.Close();
            }
        }

        public static void GetRecordNoCalcolo(long idPensione, out List<RecordDatiNoCalcolo> listaRecordNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaRecordNoCalcolo = (from rf in db.RecordDatiNoCalcolos
                                    where rf.IdPensione == idPensione
                                    select rf).ToList<RecordDatiNoCalcolo>();
                db.Connection.Close();
            }
        }

        public static void DeleteRecordNoCalcolo(long idRecordNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteRecordDatiNoCalcolo(idRecordNoCalcolo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRecordDatiNoCalcolo");
                }
                db.Connection.Close();
            }
        }


        //TO DO 
        public static void DeleteAllRecordNoCalcolo(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllRecordDatiNoCalcolo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllRecordDatiNoCalcolo");
                }
                db.Connection.Close();
            }
        }

        public static void GetRecordNoCalcoloByIdRecord(long idRecord, out RecordDatiNoCalcolo recordDatiNoCalcoloDB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                recordDatiNoCalcoloDB = (from rf in db.RecordDatiNoCalcolos
                                        where rf.Id == idRecord
                                        select rf).FirstOrDefault<RecordDatiNoCalcolo>();
                db.Connection.Close();
            }
            
        }
    }
}
