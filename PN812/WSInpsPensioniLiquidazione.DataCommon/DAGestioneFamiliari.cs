using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneFamiliari
    {
        #region Familiari
        public static void GetFamiliariByIdPensione(long IdPensione, out List<Familiare> Lfamiliari, out List<Anagrafica> anagraficheFamil)
        {
            Lfamiliari = null;
            anagraficheFamil = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {

                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var elencofamiliari = (from f in db.Familiares
                                           join a in db.Anagraficas on f.IdAnagrafica equals a.Id
                                           where f.IdPensione == IdPensione
                                           select f);

                    foreach (Familiare f in elencofamiliari)
                    {
                        if (Lfamiliari == null)
                        {
                            anagraficheFamil = new List<Anagrafica>();
                            Lfamiliari = new List<Familiare>();
                        }
                        Lfamiliari.Add(f);
                        anagraficheFamil.Add(f.Anagrafica);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaFamiliare(Familiare familiare)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.InsertFamiliare(
                    familiare.IdAnagrafica,
                    familiare.IdPensione,
                    familiare.TipoComponente,
                    familiare.SiglaFamiliare,
                    familiare.ScadenzaRevisioneSanitaria,
                    familiare.CodiceDetrazioni,
                    familiare.ValidazioneCF,
                    familiare.FlagTitolare,
                    familiare.DataMorte,
                    familiare.Provenienza,
                    familiare.Confermato,
                    familiare.Progressivo,
                    familiare.TipoUnione);

                db.Connection.Close();
            }

        }

        public static void CancellaFamiliare(long IDfamiliare, long IDPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteFamiliare(IDfamiliare, IDPensione);

                db.Connection.Close();
            }

        }

        public static void DeleteAllFamiliariByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteFamiliari(idPensione);
                db.Connection.Close();
            }
        }

        public static void CheckFamiliariByIdPensione(long idPensione, out bool presenzaFamiliari)
        {
            presenzaFamiliari = false;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {

                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    int numeroFamiliari = (from f in db.Familiares
                                           where f.IdPensione == idPensione
                                           select f).Count();

                    if (numeroFamiliari > 0)
                        presenzaFamiliari = true;

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        #endregion Familiari

        #region CodMaggiorazioneFamiliari
        public static void SalvaCodMaggiorazioneFamiliari(long idPensione, List<CodMaggiorazioneFamiliari> listaCodMaggFamiliari)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = null;
                db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                //E' necessario, trattandosi di una lista, eliminare dal db prima gli eventuali Id non presenti in lista ma presenti sul db
                //prima di procedere con il salvataggio della lista
                List<CodMaggiorazioneFamiliari> listaCodMaggFamiliariOriginale = null;
                GetCodMaggiorazioneFamiliariByIdPensione(idPensione, out listaCodMaggFamiliariOriginale);

                if (listaCodMaggFamiliari != null)
                {
                    if (listaCodMaggFamiliariOriginale != null && listaCodMaggFamiliariOriginale.Count > 0)
                    {
                        List<long> listaIdDaRimuovere = new List<long>();

                        foreach (CodMaggiorazioneFamiliari CodMaggFamiliariOriginale in listaCodMaggFamiliariOriginale)
                        {
                            foreach (CodMaggiorazioneFamiliari CodMaggFamiliari in listaCodMaggFamiliari)
                            {
                                if (CodMaggFamiliariOriginale.IdAnagrafica == CodMaggFamiliari.IdAnagrafica)
                                {
                                    listaIdDaRimuovere.Add(CodMaggFamiliariOriginale.Id);
                                    break;
                                }
                            }
                        }

                        foreach (long id in listaIdDaRimuovere)
                        {
                            DeleteCodMaggiorazioneFamiliari(id);
                        }
                    }

                    foreach (CodMaggiorazioneFamiliari CodMaggFamiliari in listaCodMaggFamiliari)
                        db.InsertCodMaggiorazioneFamiliari(CodMaggFamiliari.Id, CodMaggFamiliari.IdAnagrafica, CodMaggFamiliari.IdPensione,
                            CodMaggFamiliari.CodiceMaggiorazione, CodMaggFamiliari.Decorrenza,
                            CodMaggFamiliari.Cessazione, CodMaggFamiliari.SiglaFamiliare, CodMaggFamiliari.TipoUnione, CodMaggFamiliari.DirittoAF, 
                            CodMaggFamiliari.QuotaAF, CodMaggFamiliari.ContitolaritaFondo, CodMaggFamiliari.ContitolaritaAgo);
                }

                db.Connection.Close();
            }
        }

        public static void GetCodMaggiorazioneFamiliariByIdPensione(long IDPensione, out List<CodMaggiorazioneFamiliari> listaCodMaggFamiliari)
        {
            listaCodMaggFamiliari = null;
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaCodMaggFamiliari = (from c in db.CodMaggiorazioneFamiliaris
                                         where c.IdPensione == IDPensione
                                         select c).ToList<CodMaggiorazioneFamiliari>();
                db.Connection.Close();
            }
        }

        public static void DeleteCodMaggiorazioneFamiliari(long idCodMaggiorazioneFamiliari)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCodMaggFamiliariById(idCodMaggiorazioneFamiliari);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCodMaggFamiliariById");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAllCodMaggiorazioneFamiliariByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllCodMaggiorazioneFamiliari(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllCodMaggiorazioneFamiliari");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteCodMaggiorazioneFamiliariPerFamiliare(long idAnagrafica, long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteCodMaggiorazioneFamiliare(idAnagrafica, idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteCodMaggiorazioneFamiliare");
                }
                db.Connection.Close();
            }
        }
        #endregion CodMaggiorazioneFamiliari
    }
}
