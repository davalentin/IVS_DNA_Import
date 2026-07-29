using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneStoricoDataLimitePoligraficiLetteraB
    {
        public static void SalvaStoricoDataLimitePoligraficiLetteraB(DatiStoricoDataLimitePoligraficiLetteraB datiStoricoDataLimitePoligraficiLetteraB)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoricoDataLimitePoligraficiLettB objDl = new StoricoDataLimitePoligraficiLettB();
                Utility.ValorizzaOggetti(datiStoricoDataLimitePoligraficiLetteraB, objDl);
                DAGestioneStoricoDataLimiteDomandePoligraficiLetteraB.InsertStoricoDataLimitePoligraficiLetteraB(objDl);
                transactionScope.Complete();
            }
        }

        public static void GetStoricoDataLimitePoligraficiLetteraB(out List<GestioneStoricoDataLimitePoligraficiLetteraB.DatiStoricoDataLimitePoligraficiLetteraB> elencoStoricoDataLimitePoligraficiLetteraB)
        {
            elencoStoricoDataLimitePoligraficiLetteraB = new List<DatiStoricoDataLimitePoligraficiLetteraB>();

            //db mapping
            List<StoricoDataLimitePoligraficiLettB> listStorico = new List<StoricoDataLimitePoligraficiLettB>();

            DAGestioneStoricoDataLimiteDomandePoligraficiLetteraB.GetAllStoricoDataLimiteDomandePoligraficiLetteraB(out listStorico);
            if (listStorico != null && listStorico.Count > 0)
            {
                foreach (StoricoDataLimitePoligraficiLettB objDb in listStorico)
                {
                    DatiStoricoDataLimitePoligraficiLetteraB objBl = new DatiStoricoDataLimitePoligraficiLetteraB();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    elencoStoricoDataLimitePoligraficiLetteraB.Add(objBl);
                }
            }
        }

        public static void UpdateNoteStoricoDataLimitePoligraficiLetteraB(int id, string note)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneStoricoDataLimiteDomandePoligraficiLetteraB.UpdateAllDataLimiteDomandePoligraficiLetteraBNote(id, note);
                transactionScope.Complete();
            }
        }

        #region Nested Class

        public class DatiStoricoDataLimitePoligraficiLetteraB
        {
            public long Id { get; set; }

            public DateTime DataModifica { get; set; }

            public DateTime DataLimitePoligraficiLetteraB { get; set; }

            public string Matricola { get; set; }

            public string Note { get; set; }
        }
        #endregion
    }
}
