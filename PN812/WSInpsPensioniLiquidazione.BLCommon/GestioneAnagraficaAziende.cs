using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAnagraficaAziende
    {
        public static void GetDecAnagraficaAziende(out List<DecodAnagraficaAziende> elencoAnagraficaAziende)
        {
            elencoAnagraficaAziende = null;
            List<DecAnagraficaAziende> elencoDecAnagraficaAziendeDB = null;
            DAGestioneAnagraficaAziende.GetDecAnagraficaAziende(out elencoDecAnagraficaAziendeDB);
            if (elencoDecAnagraficaAziendeDB != null && elencoDecAnagraficaAziendeDB.Count > 0)
            {
                elencoAnagraficaAziende = new List<DecodAnagraficaAziende>();
                foreach (DecAnagraficaAziende decodificaAnagraficaAziendeDB in elencoDecAnagraficaAziendeDB)
                {
                    DecodAnagraficaAziende anagraficaAziende = new DecodAnagraficaAziende();
                    Utility.ValorizzaOggetti(decodificaAnagraficaAziendeDB, anagraficaAziende);
                    elencoAnagraficaAziende.Add(anagraficaAziende);
                }
            }
        }

        public static void SalvaAnagraficaAziende(DecodAnagraficaAziende decodAnagraficaAziende) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAziende decAnagraficaAziende = new DecAnagraficaAziende(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAziende, decAnagraficaAziende);
                DAGestioneAnagraficaAziende.SalvaAnagraficaAziende(decAnagraficaAziende);/*salva oggetto del blcommon nell'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        public static void DeleteAnagraficaAziende(DecodAnagraficaAziende decodAnagraficaAziende)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAziende decAnagraficaAziende = new DecAnagraficaAziende(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAziende, decAnagraficaAziende);
                DAGestioneAnagraficaAziende.DeleteAnagraficaAziende(decAnagraficaAziende);/*delete oggetto del blcommon dall'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        #region nested class
        #region class DecodAnagraficaAziende
        /// <summary>
        /// nested class del BLcommon "gemella" della tabella DB
        /// </summary>
        public class DecodAnagraficaAziende
        {
            public long Id { get; set; }
            public string DenominazioneAzienda { get; set; }
            public string SottogruppoOnere { get; set; }

            /// <summary>
            /// confronto personalizzato del contenuto dell'oggetto in input con altro oggetto, es. dal DB
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                DecodAnagraficaAziende anagraficaAziendeDB = (DecodAnagraficaAziende)obj;
                try
                {
                    if (this.DenominazioneAzienda != anagraficaAziendeDB.DenominazioneAzienda ||
                        this.SottogruppoOnere != anagraficaAziendeDB.SottogruppoOnere)

                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + (this.DenominazioneAzienda != null ? this.DenominazioneAzienda.GetHashCode() : 0);
                hash = (hash * 7) + (this.SottogruppoOnere != null ? this.SottogruppoOnere.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion class DecodAnagraficaAziende
        #endregion nested class
    }
}
