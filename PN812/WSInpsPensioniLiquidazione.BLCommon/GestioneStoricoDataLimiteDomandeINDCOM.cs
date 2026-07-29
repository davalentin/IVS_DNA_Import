using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneStoricoDataLimiteDomandeINDCOM
    {

        public static void SalvaStoricoDataLimiteDomandeINDCOM(DatiStoricoDataLimiteDomandeINDCOM datiStoricoDataLimiteINDCOM)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoricoDataLimiteDomandeINDCOM objDl = new StoricoDataLimiteDomandeINDCOM();
                Utility.ValorizzaOggetti(datiStoricoDataLimiteINDCOM, objDl);
                DAGestioneStoricoDataLimiteDomandeINDCOM.InsertStoricoDataLimiteINDCOM(objDl);
                transactionScope.Complete();
            }
        }

        public static void GetStoricoDataLimiteINDCOM(out List<GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM> elencoStoricoDataLimiteINDCOM) 
        {
            elencoStoricoDataLimiteINDCOM = new List<DatiStoricoDataLimiteDomandeINDCOM>();

            //db mapping
            List<StoricoDataLimiteDomandeINDCOM> listStorico = new List<StoricoDataLimiteDomandeINDCOM>();

            DAGestioneStoricoDataLimiteDomandeINDCOM.GetAllStoricoDataLimiteDOmandeINDCOM(out listStorico);
            if (listStorico != null && listStorico.Count > 0)
            {
                foreach (StoricoDataLimiteDomandeINDCOM objDb in listStorico)
                {
                    DatiStoricoDataLimiteDomandeINDCOM objBl = new DatiStoricoDataLimiteDomandeINDCOM();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    elencoStoricoDataLimiteINDCOM.Add(objBl);
                }
            }
        }

        public static void UpdateNoteStoricoDataLimiteINDCOM(int id, string note) 
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {                
                DAGestioneStoricoDataLimiteDomandeINDCOM.UpdateAllDataLimiteDOmandeINDCOMNote(id, note);
                transactionScope.Complete();
            }
        }

        #region Nested Class

        public class DatiStoricoDataLimiteDomandeINDCOM 
        {
            public long Id { get; set; }

            public DateTime DataModifica { get; set; }

            public DateTime DataLimiteDomandeINDCOM { get; set;  }
          
            public string Matricola { get; set; }
            
            public string Note { get; set; }
        }
        #endregion
    }
}
