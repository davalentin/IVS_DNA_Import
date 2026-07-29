using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneInteressiLegali
    {
        public static void SalvaInteresseLegale(DatiInteressiLegali datiInteressiLegali)
        {
            InteressiLegali interesseLegaleDB = new InteressiLegali();
            BLCommon.Utility.ValorizzaOggetti(datiInteressiLegali, interesseLegaleDB);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneInteressiLegali.SalvaInteresseLegale(interesseLegaleDB);
                transactionScope.Complete();
            }
        }

        public static void GetInteressiLegaliByIdPensione(long idPensione, out List<DatiInteressiLegali> lDatiInteressiLegali)
        {
            lDatiInteressiLegali = null;
            List<InteressiLegali> lDatiInteressiLegaliDB = null;

            DAGestioneInteressiLegali.GetInteressiLegaliByIdPensione(idPensione, out lDatiInteressiLegaliDB);
            if (lDatiInteressiLegaliDB != null && lDatiInteressiLegaliDB.Count > 0)
            {
                lDatiInteressiLegali = new List<DatiInteressiLegali>();
                foreach (InteressiLegali ri in lDatiInteressiLegaliDB)
                {
                    DatiInteressiLegali datiInteressiLegali = new DatiInteressiLegali();
                    BLCommon.Utility.ValorizzaOggetti(ri, datiInteressiLegali);
                    lDatiInteressiLegali.Add(datiInteressiLegali);
                }
            }
        }

        public static void EliminaAllInteressiLegaliByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneInteressiLegali.EliminaAllInteressiLegaliByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class DatiInteressiLegali
        {
            #region Public
            public long IdPensione { get; set; }
            public long? TipoInteresseLegale { get; set; }
            public DateTime? DataInizio { get; set; }
            public DateTime? DataFine { get; set; }
            public decimal? Importo { get; set; }
            #endregion Public
        }

        #endregion nested class
    }
}
