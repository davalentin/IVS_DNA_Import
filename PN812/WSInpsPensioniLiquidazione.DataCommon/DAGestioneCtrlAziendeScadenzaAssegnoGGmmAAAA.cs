using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneCtrlAziendeScadenzaAssegnoGGmmAAAA
    {
        public static void GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out List<CtrlAziendeScadenzaAssegnoGGmmAAAA> elencoDecodificaAziendeScadenzaAssegnoGGmmAAAA)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAziendeScadenzaAssegnoGGmmAAAA = (from d in db.CtrlAziendeScadenzaAssegnoGGmmAAAAs select d).ToList<CtrlAziendeScadenzaAssegnoGGmmAAAA>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void InsertAziendeScadenzaAssegnoGGmmAAAA(CtrlAziendeScadenzaAssegnoGGmmAAAA aziendaScadenzaAssegnoGGmmAAAA, string siglaCategoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertAziendeGGmmAAAA(aziendaScadenzaAssegnoGGmmAAAA.TraduzioneSuGP, aziendaScadenzaAssegnoGGmmAAAA.ProgressivoRichiesto, siglaCategoria);
                
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Azienda già presente nella tabella");
                }
                else if (result == -2)
                {
                    throw new INPS.DNA.DnaValidationException("Azienda non presente nella tabella di Decodifica Azienda, impossibile inserire");
                }
                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertAziendeGGmmAAAA");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAziendeScadenzaAssegnoGGmmAAAA(CtrlAziendeScadenzaAssegnoGGmmAAAA aziendaScadenzaAssegnoGGmmAAAA, string siglaCategoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                int result = db.DeleteAziendaGGmmAAAA(aziendaScadenzaAssegnoGGmmAAAA.TraduzioneSuGP, siglaCategoria);
                if (result == -1)
                {
                    throw new INPS.DNA.DnaValidationException("Record in uso, impossibile eliminare");
                }

                else if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteBancafideiussoria");
                }
                db.Connection.Close();
            }
        }
    
    
    }
}
