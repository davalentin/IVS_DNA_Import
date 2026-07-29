using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestionePensioneINPDAP
    {
        public static void GetPensioneINPDAPRecordFondoByIdPensione(Int64 idPensione, out List<PensioneINPDAP> pensioneINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    pensioneINPDAP = (from f in db.PensioneINPDAPs
                                     where f.IdPensione == idPensione
                                     select f).ToList<PensioneINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPensioneINPDAPByIdRecordFondo(Int64 idRecordFondo, out PensioneINPDAP pensioneINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    pensioneINPDAP = (from f in db.PensioneINPDAPs
                                where f.IdRecordFondo == idRecordFondo
                                select f).SingleOrDefault<PensioneINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaPensioneINPDAPByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneINPDAP(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaPensioneINPDAPByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioneINPDAPRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioneINPDAPRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaPensioneINPDAPRecordFondo(PensioneINPDAP pensioneINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioneINPDAPRecordFondo(pensioneINPDAP.IdPensione, pensioneINPDAP.IdRecordFondo, pensioneINPDAP.DecorrenzaEconomica, pensioneINPDAP.RequisitiAnte247, 
                    pensioneINPDAP.TrimesteRequisiti, pensioneINPDAP.AnnoRequisiti, pensioneINPDAP.AnzianitaAnni, pensioneINPDAP.AliquotaMediaINPDAP, pensioneINPDAP.DataRivalsaINPDAP,
                    pensioneINPDAP.CausaCessazione, pensioneINPDAP.TitolareAltraPensione, pensioneINPDAP.DirittoIndennitaIntegrativaSpeciale, pensioneINPDAP.RiduzioneL537,
                    pensioneINPDAP.IISAbbattimentoAnni, pensioneINPDAP.VVUtiliDirittoAA, pensioneINPDAP.VVUtiliDirittoMM, pensioneINPDAP.VVUtiliDirittoGG, pensioneINPDAP.VVUtiliMisuraAA,
                    pensioneINPDAP.VVUtiliMisuraMM, pensioneINPDAP.VVUtiliMisuraGG, pensioneINPDAP.Microqualifica, pensioneINPDAP.AnniMax, pensioneINPDAP.AnniUtili, pensioneINPDAP.Comparto, pensioneINPDAP.Settore, pensioneINPDAP.Ruolo,
                    pensioneINPDAP.CfAmministrazione, pensioneINPDAP.ProgAmministrazione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioneINPDAPRecordFondo");
                }
                db.Connection.Close();
            }
        }
    }
}
