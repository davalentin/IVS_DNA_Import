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
    public class DAGestioneAnniRichiestaBonus
    {
        public static void GetAnniRichiestaBonusByIdPensione(Int64 idPensione, out List<AnniRichiestaBonus> anniRichiestaBonus)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    anniRichiestaBonus = (from q in db.AnniRichiestaBonus
                                          where q.IdPensione == idPensione
                                          select q).ToList<AnniRichiestaBonus>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaAnniRichiestaBonus(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnniRichiestaBonus(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnniRichiestaBonus");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaAnniRichiestaBonus(List<AnniRichiestaBonus> anniRichiestaBonus)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = 0;
                foreach (AnniRichiestaBonus annoRichiesta in anniRichiestaBonus)
                {
                    result = db.InsertAnniRichiestaBonus(annoRichiesta.Id, annoRichiesta.IdPensione, annoRichiesta.Anno, annoRichiesta.Prescrizione, annoRichiesta.CodiceEsitoMessaggio, annoRichiesta.DescrizioneEsitoMessaggio, annoRichiesta.EsitoCalcoloBeneficio, annoRichiesta.IsRichiestaBonus);
                    if (result != 0)
                    {
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnniRichiestaBonus");
                    }
                }

                db.Connection.Close();
            }
        }

        public static void GetPrenotazioneElaborazioniByIdPensione(Int64 idPensione, out List<PrenotazioneElaborazioni> prenotazioneElaborazioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    prenotazioneElaborazioni = (from q in db.PrenotazioneElaborazionis
                                                where q.IdPensione == idPensione
                                                select q).ToList<PrenotazioneElaborazioni>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPrenotazioneElaborazioni(List<PrenotazioneElaborazioni> prenotazioneElaborazioni)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = 0;
                foreach (PrenotazioneElaborazioni prenotazioneElaborazione in prenotazioneElaborazioni)
                {
                    result = db.InsertPrenotazioneElaborazioni(prenotazioneElaborazione.IdPensione, prenotazioneElaborazione.AnnoRichiesto, prenotazioneElaborazione.DataInserimento, prenotazioneElaborazione.DecorrenzaPresaInCarico, prenotazioneElaborazione.EsitoCalcoloBeneficio,
                            prenotazioneElaborazione.DescrizioneEsito, prenotazioneElaborazione.TipoElaborazione);
                    if (result != 0)
                    {
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPrenotazioneElaborazioni");
                    }
                }

                db.Connection.Close();
            }
        }
    }
}
