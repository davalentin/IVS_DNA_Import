using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlCodiceConvenzionePrestazioniEE
    {
        public static void GetCtrlCodiceConvenzionePrestazioniEE(out List<CtrlCodiceConvenzionePrestazioniEE> ctrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrl = (from cr in db.CtrlCodiceConvenzionePrestazioniEEs select cr).ToList<CtrlCodiceConvenzionePrestazioniEE>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetListaCodiceConvenzionePrestazioniEE(string codiceStato, DateTime? decorrenzaOriginaria, out List<CtrlCodiceConvenzionePrestazioniEE> ctrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrl = (from cr in db.CtrlCodiceConvenzionePrestazioniEEs
                            where cr.CodiceStato == codiceStato &&
                                (!cr.DataInizio.HasValue || cr.DataInizio.Value <= decorrenzaOriginaria) &&
                                (!cr.DataFine.HasValue || cr.DataFine.Value > decorrenzaOriginaria)
                            select cr).ToList<CtrlCodiceConvenzionePrestazioniEE>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetListaCodiceStatoPrestazioniEE(byte? codiceConvenzione, out List<CtrlCodiceConvenzionePrestazioniEE> ctrl)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrl = (from cr in db.CtrlCodiceConvenzionePrestazioniEEs
                            where cr.CodiceConvenzione == codiceConvenzione && cr.IsConvenzioneConAltroStato == false
                            select cr).ToList<CtrlCodiceConvenzionePrestazioniEE>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
