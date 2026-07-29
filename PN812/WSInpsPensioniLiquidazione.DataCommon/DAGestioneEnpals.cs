using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneEnpals
    {
        public static void GetEnpalsByIdPensione(Int64 idPensione, out Enpal enpals)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    enpals = (from cc in db.Enpals where cc.IdPensione == idPensione select cc).SingleOrDefault<Enpal>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaEnpals(Enpal enpals)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertEnpals(enpals.IdPensione, enpals.AADiritto, enpals.MMDiritto, enpals.RaggruppamentoPrevalente, enpals.GruppoPrevalente, enpals.GruppoDiritto,
                    enpals.NTotContributi, enpals.NTotContributiEnpals, enpals.EtaDirittoAA, enpals.EtaDirittoMM, enpals.EtaMisuraAA, enpals.EtaMisuraMM, enpals.Qualifica,
                    enpals.DataFinestra, enpals.NContributiMisura, enpals.NTotDiritto, enpals.NTotQualifica, enpals.NContributiQuinquennio, enpals.NContributiTriennio,
                    enpals.NContributiNL222, enpals.NContributiNL155, enpals.CodiceDeroga1, enpals.CodiceDeroga2, enpals.CodiceDeroga3, enpals.CodiceDeroga4, enpals.NumeroContributiNLNonVedenti,
                    enpals.IndicatoreInvalidita80, enpals.ImportoPensione, enpals.TipoLiquidazione, enpals.CodiceRitorno, enpals.CodiceTipoDomanda, enpals.TipoPensione, enpals.ImportoPensione707,
                    enpals.TipoLiquidazioneProvvisoria, enpals.DecorrenzaImportoPensione, enpals.DecorrenzaImportoPensione707, enpals.GP1AN87B, enpals.AnzianitaContributiva, enpals.ImportoIIS, 
                    enpals.DecorrenzaImportoIIS);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertEnpals");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaEnpalsByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteEnpals(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteEnpals");
                }
                db.Connection.Close();
            }
        }
    }
}
