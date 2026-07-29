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
    public class DAGestioneSupplementi
    {
        public static void GetDatiSupplementiByIdPensione(long idPensione, out List<Supplementi> LstSupplementi)
        {
            LstSupplementi = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var supplementi = (from a in db.Supplementis where a.IdPensione == idPensione select a);
                    LstSupplementi = new List<Supplementi>();
                    foreach (Supplementi sup in supplementi)
                    {
                        LstSupplementi.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiSupplementiNoStoricoByIdPensione(long idPensione, out List<Supplementi> LstSupplementiNoStorico)
        {
            LstSupplementiNoStorico = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var supplementi = (from a in db.Supplementis where a.IdPensione == idPensione && !a.IsStorico select a);
                    LstSupplementiNoStorico = new List<Supplementi>();
                    foreach (Supplementi sup in supplementi)
                    {
                        LstSupplementiNoStorico.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSupplementiStoricoByIdPensione(Int64 idPensione, out List<Supplementi> lSupplementiStorico)
        {
            lSupplementiStorico = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var supplementi = (from a in db.Supplementis where a.IdPensione == idPensione && a.IsStorico select a);
                    lSupplementiStorico = new List<Supplementi>();
                    foreach (Supplementi sup in supplementi)
                    {
                        lSupplementiStorico.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiSupplementi(long idPensione, List<Supplementi> supToDB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                List<Supplementi> supFromDB = new List<Supplementi>();
                var supplementi = (from a in db.Supplementis where a.IdPensione == idPensione select a);
                supFromDB = new List<Supplementi>();
                foreach (Supplementi sup in supplementi)
                {
                    supFromDB.Add(sup);
                }

                var supToDelete = (from Supplementi i in supFromDB
                                   select i.Id).Distinct()
                             .Except((from Supplementi o in supToDB
                                      select o.Id).Distinct());

                foreach (var supp in supToDelete)
                {
                    db.DeleteSupplementi(supp, idPensione);
                }
                for (int i = 0; i < supToDB.Count; i++)
                {
                    db.InsertSupplementi(
                    supToDB[i].Id,
                    idPensione,
                    supToDB[i].TipoSupplemento,
                    supToDB[i].DecorrenzaSupplemento,
                    supToDB[i].CodiceGestioneSupplemento,
                    supToDB[i].NSettimaneSupplemento,
                    supToDB[i].RMSSupplemento,
                    supToDB[i].MontanteSupplemento,
                    supToDB[i].QuotaSupplemento,
                    supToDB[i].AmmontareContributivo,
                    supToDB[i].CodiceLiquidazione,
                    supToDB[i].CodTipoQuota,
                    supToDB[i].IsFromPrelievo,
                    supToDB[i].IsStorico);
                }
                db.Connection.Close();
            }
        }

        public static void SalvaSupplementiStorico(long idPensione, List<Supplementi> supplementi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                for (int i = 0; i < supplementi.Count; i++)
                {
                    db.InsertSupplementi(
                    supplementi[i].Id,
                    idPensione,
                    supplementi[i].TipoSupplemento,
                    supplementi[i].DecorrenzaSupplemento,
                    supplementi[i].CodiceGestioneSupplemento,
                    supplementi[i].NSettimaneSupplemento,
                    supplementi[i].RMSSupplemento,
                    supplementi[i].MontanteSupplemento,
                    supplementi[i].QuotaSupplemento,
                    supplementi[i].AmmontareContributivo,
                    supplementi[i].CodiceLiquidazione,
                    supplementi[i].CodTipoQuota,
                    supplementi[i].IsFromPrelievo,
                    supplementi[i].IsStorico);
                }
                db.Connection.Close();
            }
        }


        private static void CancelSingleSupplementi(long idPensione, long idSupplemento)
        {
            PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
            db.DeleteSupplementi(idPensione, idSupplemento);
            db.Connection.Close();
        }

        public static void CancellaDatiSupplementiByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllSupplementi(idPensione);
                db.Connection.Close();
            }
        }

        public static void EliminaSupplementiNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSupplementiNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSupplementiNoStorico");
                }
                db.Connection.Close();
            }
        }

        public static void GetDatiSupplementiBaseByIdPensione(long idPensione, out SupplementiBase supplementiBase)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    supplementiBase = (from a in db.SupplementiBases where a.IdPensione == idPensione select a).SingleOrDefault<SupplementiBase>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaSupplementiBase(SupplementiBase supplementiBase)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSupplementiBase(supplementiBase.IdPensione, supplementiBase.RMS7290, supplementiBase.ImportoIVS, supplementiBase.RMSArt2DPCM161289, supplementiBase.ContributiLegge335,
                  supplementiBase.NSettimaneUtiliDiritto, supplementiBase.NSettimaneUtiliMisura, supplementiBase.Legge407AnniServizioQuotaA, supplementiBase.Legge407AnniServizioQuotaB,
                  supplementiBase.Legge407SettimaneServizioQuotaB, supplementiBase.Legge407AnniServizioQuotaC, supplementiBase.Legge407RetribuzionePensionabileRMSQuotaA, supplementiBase.Legge407RetribuzionePensionabileRMSQuotaB,
                  supplementiBase.Legge407RetribuzionePensionabileQuotaC, supplementiBase.Legge407SettimaneIncrementoQuotaA, supplementiBase.Legge407SettimaneIncrementoQuotaB,
                  supplementiBase.AnniIncremento1Percento, supplementiBase.AnniIncremento05Percento, supplementiBase.RenditaFacoltativaOrdinaria, supplementiBase.RenditaFacoltativaConvenzionale);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertIstruttoria");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaDatiSupplementiBaseByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllSupplementiBase(idPensione);
                db.Connection.Close();
            }
        }

        #region Enpals

        public static void GetDatiSupplementiEnpalsByIdPensione(long idPensione, out List<SupplementiENPAL> LstSupplementi)
        {
            LstSupplementi = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var supplementi = (from a in db.SupplementiENPALs where a.IdPensione == idPensione select a);
                    LstSupplementi = new List<SupplementiENPAL>();
                    foreach (SupplementiENPAL sup in supplementi)
                    {
                        LstSupplementi.Add(sup);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiSupplementiEnpalsByIdSuppRecordENPALS(long idSuppRecordENPALS, out List<SupplementiENPAL> lstSupplementi)
        {
            lstSupplementi = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lstSupplementi = (from a in db.SupplementiENPALs where a.IdSuppRecordENPALS == idSuppRecordENPALS select a).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiSupplementiEnpals(SupplementiENPAL supplementi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSupplementiENPALS(supplementi.IdPensione, supplementi.TipoSupplemento, supplementi.Quota, supplementi.Periodi, supplementi.NTotaleContributiCalcolo, supplementi.RM,
                    supplementi.Importo, supplementi.ImportoProRataTemporis, supplementi.CoefficienteTrasformazione, supplementi.ImportoContributivoTotale,
                    supplementi.Montante, supplementi.IdSuppRecordENPALS);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertIstruttoria");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaDatiSupplementiEnpalsByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteSupplementiENPALS(idPensione);
                db.Connection.Close();
            }
        }

        public static void CancellaDatiSupplementiEnpalsByIdSuppRecordENPALS(long idSuppRecordENPALS)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteSupplementiENPALSByIdRecord(idSuppRecordENPALS);
                db.Connection.Close();
            }
        }


        //Gestione per SuppRecordEnpals 
        public static void GetDatiSuppRecordEnpalsByIdPensione(long idPensione, out List<SupplementiRecordENPAL> LstSupplementi)
        {
            LstSupplementi = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LstSupplementi = (from a in db.SupplementiRecordENPALs where a.IdPensione == idPensione select a).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDatiSuppRecordEnpalsyIdRecord(long idRecord, out SupplementiRecordENPAL suppRecordEnpals)
        {
            suppRecordEnpals = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    suppRecordEnpals = (from a in db.SupplementiRecordENPALs where a.Id == idRecord select a).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiSuppRecordEnpals(SupplementiRecordENPAL recordSupp, out long? idRecord)
        {
            idRecord = null;
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSupplementiRecordENPALS(recordSupp.Id, recordSupp.IdPensione, recordSupp.Decorrenza, recordSupp.InizioSupplemento,
                    recordSupp.FineSupplemento, recordSupp.Importo, recordSupp.RenditaFacoltativaOrdinaria, recordSupp.RenditaFacoltativaConvenzionale, recordSupp.IsFromSas, recordSupp.IsFromGP,
                    recordSupp.DettaglioSalvato, ref idRecord);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertSupplementiRecordENPALS");
                }
                db.Connection.Close();
            }
        }

        public static void CancellaDatiSuppRecordEnpalsByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllSupplementiRecordENPALS(idPensione);
                db.Connection.Close();
            }
        }

        public static void CancellaDatiSuppRecordEnpalsByIdRecord(long idRecord)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteSupplementiRecordENPALS(idRecord);
                db.Connection.Close();
            }
        }
        /////////////////////////////
        #endregion Enpals

        #region Cumulo
        public static void GetSupplementiCumuloByIdPensione(Int64 idPensione, out List<SupplementiCumulo> lSupplementi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lSupplementi = (from cc in db.SupplementiCumulos where cc.IdPensione == idPensione && !cc.IsStorico select cc).ToList<SupplementiCumulo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSupplementiCumuloStoricoByIdPensione(Int64 idPensione, out List<SupplementiCumulo> lSupplementi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lSupplementi = (from cc in db.SupplementiCumulos where cc.IdPensione == idPensione && cc.IsStorico select cc).ToList<SupplementiCumulo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaSupplementiCumulo(SupplementiCumulo supplementi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertSupplementiCumulo(supplementi.IdPensione, supplementi.EnteGestioneFondo, supplementi.Settimane, supplementi.Importo, supplementi.Decorrenza, supplementi.IsStorico, supplementi.AdeguamentoProQuotaCasse, supplementi.TipoVariazione);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertSupplementiCumulo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaSupplementiCumuloByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSupplementiCumulo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSupplementiCumulo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaSupplementiCumuloNoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteSupplementiCumuloNoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteSupplementiCumuloNoStorico");
                }
                db.Connection.Close();
            }
        }

        #endregion
    }
}
