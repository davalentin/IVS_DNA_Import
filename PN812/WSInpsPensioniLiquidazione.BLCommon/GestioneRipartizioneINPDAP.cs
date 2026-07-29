using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneRipartizioneINPDAP
    {
        public static void SalvaRipartizioneINPDAP(DatiRipartizioneINPDAP datiRipartizioneINPDAP)
        {
            RipartizioneINPDAP ripartizioneINPDAPDB = new RipartizioneINPDAP();
            BLCommon.Utility.ValorizzaOggetti(datiRipartizioneINPDAP, ripartizioneINPDAPDB);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRipartizioneINPDAP.SalvaRipartizioneINPDAP(ripartizioneINPDAPDB);
                transactionScope.Complete();
            }
        }

        public static void GetRipartizioneINPDAPByIdPensione(long idPensione, out List<DatiRipartizioneINPDAP> LdatiRipartizioneINPDAP)
        {
            LdatiRipartizioneINPDAP = null;
            List<RipartizioneINPDAP> LRipartizioneINPDAPDB = null;

            DAGestioneRipartizioneINPDAP.GetRipartizioneINPDAPByIdPensione(idPensione, out LRipartizioneINPDAPDB);
            if (LRipartizioneINPDAPDB != null && LRipartizioneINPDAPDB.Count > 0)
            {
                LdatiRipartizioneINPDAP = new List<DatiRipartizioneINPDAP>();
                foreach (RipartizioneINPDAP ri in LRipartizioneINPDAPDB)
                {
                    DatiRipartizioneINPDAP datiRipartizioneINPDAP = new DatiRipartizioneINPDAP();
                    BLCommon.Utility.ValorizzaOggetti(ri, datiRipartizioneINPDAP);
                    LdatiRipartizioneINPDAP.Add(datiRipartizioneINPDAP);
                }
            }
        }

        public static void EliminaRipartizioneINPDAPByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRipartizioneINPDAP.EliminaRipartizioneINPDAPByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class DatiRipartizioneINPDAP
        {
            #region Public
            public long IdPensione { get; set; }
            public long CodiceEnte { get; set; }
            public decimal? Importo { get; set; }
            #endregion Public
        }

        #endregion nested class
    }
}
