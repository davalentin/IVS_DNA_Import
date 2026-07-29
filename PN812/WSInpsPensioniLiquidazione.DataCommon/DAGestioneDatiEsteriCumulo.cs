using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDatiEsteriCumulo
    {
        #region PrestazioniEstereCumulo

        public static void SalvaPrestazioneEsteraCumulo(PensioneEsteraCumulo prestazioneEsteraCumulo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                long? idPrestazioneEE = 0;
                db.InsertPensioneEsteraCumulo(prestazioneEsteraCumulo.Id, prestazioneEsteraCumulo.IdPensione, prestazioneEsteraCumulo.CodiceStato,
                    prestazioneEsteraCumulo.CodiceIstituzione, prestazioneEsteraCumulo.MatricolaEstera, prestazioneEsteraCumulo.ContributiDiritto, prestazioneEsteraCumulo.SettimaneMisura,
                    prestazioneEsteraCumulo.CodiceConvenzione, prestazioneEsteraCumulo.Confermato, prestazioneEsteraCumulo.IsStorico, ref idPrestazioneEE);
                prestazioneEsteraCumulo.Id = idPrestazioneEE.HasValue ? idPrestazioneEE.Value : 0;
                db.Connection.Close();
            }
        }

        public static void GetPrestazioneEsteraCumuloByIdPensione(long idPensione, bool isStorico, out List<PensioneEsteraCumulo> listaPrestazioniEE)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaPrestazioniEE = (from p in db.PensioneEsteraCumulos
                                      where p.IdPensione == idPensione && p.IsStorico == isStorico
                                      select p).ToList<PensioneEsteraCumulo>();
                db.Connection.Close();
            }
        }

        public static void DeletePrestazioniEE(long idPrestazioneEE)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePrestazioneEstera(idPrestazioneEE);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioniCiPrestazioniEE");
                }
                db.Connection.Close();
            }
        }

        public static void DeletePrestazioneEsteraCumuloNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneEsteraCumuloNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneEsteraCumulo");
                }
                db.Connection.Close();
            }
        }
        #endregion PrestazioniEstereCumulo

        #region ImportiEsteriCumulo
        public static void SalvaImportoEsteroCumulo(PensioneImportiEsteriCumulo importoEsteroCumulo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.InsertPensioneImportiEsteriCumulo(importoEsteroCumulo.Id, importoEsteroCumulo.IdPensioneEsteraCumulo, importoEsteroCumulo.DecorrenzaPrestazione, importoEsteroCumulo.ImportoPrestazione, importoEsteroCumulo.CessazionePrestazione);
                db.Connection.Close();
            }
        }

        public static void GetImportiEsteriCumulolByIdPensione(long idPensione, out List<PensioneImportiEsteriCumulo> listaImportiEsteri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaImportiEsteri = (from p in db.PensioneImportiEsteriCumulos
                                      //where p.PensioneEsteraCumulo.IdPensione == idPensione
                                      select p).ToList<PensioneImportiEsteriCumulo>();
                db.Connection.Close();
            }
        }

        public static void DeleteAllImportiEsteriCumuloByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllPensioneImportiEsteriCumulo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneImportiEsteriCumulo");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteImportiEsteri(long idImportiEsteri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneImportiEsteriCumulo(idImportiEsteri);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioniCiImportiEsteri");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteImportiEsteriCumuloPerPrestazione(long idPrestazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneImportiEsteriCumuloPerPrestazione(idPrestazione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllPensioniCiImportiEsteriPerPrestazione");
                }
                db.Connection.Close();
            }
        }
        #endregion ImportiEsteriCumulo
    }
}
