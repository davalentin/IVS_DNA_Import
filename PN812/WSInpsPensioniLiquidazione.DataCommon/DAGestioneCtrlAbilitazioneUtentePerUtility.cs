using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAbilitazioneUtentePerUtility
    {
        public static void GetListaAbilitazioniByMatricola(string matricola, out List<CtrlAbilitazioneUtentePerUtility> listaAbilitazioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaAbilitazioni = (from ctrl in db.CtrlAbilitazioneUtentePerUtilities
                                         where ctrl.Matricola == matricola
                                         select ctrl).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
