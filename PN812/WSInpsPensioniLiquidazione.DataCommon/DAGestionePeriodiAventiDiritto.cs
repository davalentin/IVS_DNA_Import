using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestionePeriodiAventiDiritto
    {
        #region PeriodiFamiliari
        public static void GetPeriodiAventiDirittoByIdPensione(Expression<Func<PeriodiAventiDiritto, bool>> whereCondition, out List<PeriodiAventiDiritto> listaPeriodiAventiDiritto)
        {
            listaPeriodiAventiDiritto = null;

            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaPeriodiAventiDiritto = (from c in db.PeriodiAventiDirittos
                                             select c).Where(whereCondition).ToList<PeriodiAventiDiritto>();
                db.Connection.Close();
            }
        }

        public static void SalvaPeriodoAventiDiritto(PeriodiAventiDiritto periodoAventiDiritto)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPeriodoAventiDiritto(periodoAventiDiritto.IdPensione, periodoAventiDiritto.IdAventeDiritto, periodoAventiDiritto.GradoParentela, periodoAventiDiritto.TipoUnione, 
                    periodoAventiDiritto.DecorrenzaPeriodo, periodoAventiDiritto.CessazionePeriodo, periodoAventiDiritto.PercSpettante, periodoAventiDiritto.CoeffRiduzione, periodoAventiDiritto.PercGiudice, 
                    periodoAventiDiritto.IsFromWebDom, periodoAventiDiritto.IsFromGP);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPeriodoAventiDiritto");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAllPeriodiAventiDirittoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllPeriodiAventiDiritto(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllPeriodiAventiDiritto");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAllPeriodiAventiDirittoByIdAventeDiritto(long idAventeDiritto)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePeriodiAventiDirittoByIdAventeDiritto(idAventeDiritto);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePeriodiAventiDirittoByIdAventeDiritto");
                }
                db.Connection.Close();
            }
        }
        #endregion PeriodiFamiliari
    }
}
