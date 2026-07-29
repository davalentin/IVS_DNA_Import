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
    public class DAGestioneAnagrafica
    {
        public static void GetAnagraficaByCodiceFiscale(string codiceFiscale, out Anagrafica anagrafica)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    anagrafica = (from a in db.Anagraficas where a.CodiceFiscale == codiceFiscale select a).SingleOrDefault<Anagrafica>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAnagraficaByIdAnagrafica(long idAnagrafica, out Anagrafica anagrafica)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    anagrafica = (from a in db.Anagraficas where a.Id == idAnagrafica select a).SingleOrDefault<Anagrafica>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetIdAnagraficaByCodiceFiscale(string codiceFiscale, out long idAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    idAnagrafica = (from a in db.Anagraficas where a.CodiceFiscale == codiceFiscale select a.Id).SingleOrDefault<long>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetIdAnagraficaByNomeCognome(string nome, string cognome, out List<long> listaIdAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaIdAnagrafica = (from a in db.Anagraficas where a.Nome.StartsWith(nome) && a.Cognome.StartsWith(cognome) select a.Id).ToList<long>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAnagraficaByIdPensione(Int64 idPensione, out Anagrafica anagrafica)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    anagrafica = (from a in db.Anagraficas
                                  join t in db.Titolares on a.Id equals t.IdAnagrafica
                                  where t.IdPensione == idPensione
                                  select a).SingleOrDefault<Anagrafica>();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAnagraficaByIdPensione(Int64 idPensione, out Anagrafica anagrafica, out List<StatoCivile> elencoStatiCivili, out List<ResidenzeEstero> elencoResidenzeEstere)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    elencoStatiCivili = null;
                    elencoResidenzeEstere = null;

                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    anagrafica = (from a in db.Anagraficas
                                  join t in db.Titolares on a.Id equals t.IdAnagrafica
                                  where t.IdPensione == idPensione
                                  select a).SingleOrDefault<Anagrafica>();

                    if (anagrafica != null)
                    {
                        GetStatiCiviliById(anagrafica.Id, idPensione, out elencoStatiCivili);
                        GetResidenzeEstereById(anagrafica.Id, idPensione, out elencoResidenzeEstere);
                    }

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetStatiCiviliById(Int64 idAnagrafica, Int64 idPensione, out List<StatoCivile> statiCivili)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    statiCivili = (from s in db.StatoCiviles
                                   where s.IdAnagrafica == idAnagrafica && s.IdPensione == idPensione
                                   select s).OrderBy(x => x.Decorrenza).ToList<StatoCivile>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetLatestStatoCivileById(Int64 idAnagrafica, Int64 idPensione, out StatoCivile statoCivile)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    statoCivile = (from s in db.StatoCiviles
                                   where s.IdAnagrafica == idAnagrafica && s.IdPensione != idPensione
                                   orderby s.Decorrenza descending , s.Id descending
                                   select s).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetResidenzeEstereById(Int64 idAnagrafica, Int64 idPensione, out List<ResidenzeEstero> residenzeEstere)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    residenzeEstere = (from r in db.ResidenzeEsteros
                                       where r.IdAnagrafica == idAnagrafica && r.IdPensione == idPensione
                                       select r).OrderBy(x => x.Decorrenza).ToList<ResidenzeEstero>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaAnagrafica(Anagrafica anagrafica)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                long? idAnagrafica = null;
                int result = db.InsertAnagrafica(anagrafica.CodiceFiscale, anagrafica.Cognome, anagrafica.Nome, anagrafica.CognomeAcquisito,
                                    anagrafica.Sesso, anagrafica.DataNascita, anagrafica.ComuneNascita, anagrafica.CodiceComuneNascita, anagrafica.ProvinciaNascita,
                                    anagrafica.Cittadinanza, anagrafica.ComuneResidenza, anagrafica.CodiceComuneResidenza,
                                    anagrafica.Indirizzo, anagrafica.NCivico, anagrafica.CAP, anagrafica.ProvinciaResidenza, anagrafica.FrazioneResidenza,
                                    anagrafica.DomicilioEstero, anagrafica.ResidenzaEstero, anagrafica.Codice1Arca, anagrafica.Codice2Arca, anagrafica.Tel,
                                    anagrafica.Cell, anagrafica.EMail, anagrafica.CodiceStatoCivile, anagrafica.DecorrenzaStatoCivile, anagrafica.DataMatrimonio, ref idAnagrafica);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAnagrafica");
                }

                anagrafica.Id = idAnagrafica.HasValue ? idAnagrafica.Value : 0;
                db.Connection.Close();
            }
        }

        public static void AggiornaAnagrafica(string codiceFiscale, string cittadinanza, string tel, string cell, string eMail)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.UpdateAnagrafica(codiceFiscale, cittadinanza, tel, cell, eMail);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure UpdateAnagrafica");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaStatoCivile(StatoCivile statoCivile)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertStatoCivile(statoCivile.IdAnagrafica, statoCivile.IdPensione, statoCivile.Decorrenza, statoCivile.Codice);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertStatoCivile");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaResidenzaEstero(ResidenzeEstero residenzaEstero)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertResidenzeEstero(residenzaEstero.IdAnagrafica, residenzaEstero.IdPensione, residenzaEstero.Decorrenza, residenzaEstero.CodCatastaleStatoEE);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertResidenzeEstero");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaResidenzeEstero(long idAnagrafica, long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteResidenzeEstero(idAnagrafica, idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteResidenzeEstero");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaStatiCivili(long idAnagrafica, long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteStatiCivili(idAnagrafica, idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteStatiCivili");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAnagrafica(long idAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAnagrafica(idAnagrafica);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAnagrafica");
                }
                db.Connection.Close();
            }
        }

    }
}
