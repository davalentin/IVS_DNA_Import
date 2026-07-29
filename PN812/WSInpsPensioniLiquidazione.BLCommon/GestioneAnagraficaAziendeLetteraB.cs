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
    public class GestioneAnagraficaAziendeLetteraB
    {
        public static void GetDecAnagraficaAziende(out List<DecodAnagraficaAziendeLetteraB> elencoAnagraficaAziende)
        {
            elencoAnagraficaAziende = null;
            List<DecAnagraficaAziendeLettB> elencoDecAnagraficaAziendeDB = null;
            DAGestioneAnagraficaAziendeLetteraB.GetDecAnagraficaAziende(out elencoDecAnagraficaAziendeDB);
            if (elencoDecAnagraficaAziendeDB != null && elencoDecAnagraficaAziendeDB.Count > 0)
            {
                elencoAnagraficaAziende = new List<DecodAnagraficaAziendeLetteraB>();
                foreach (DecAnagraficaAziendeLettB decodificaAnagraficaAziendeDB in elencoDecAnagraficaAziendeDB)
                {
                    DecodAnagraficaAziendeLetteraB anagraficaAziende = new DecodAnagraficaAziendeLetteraB();
                    Utility.ValorizzaOggetti(decodificaAnagraficaAziendeDB, anagraficaAziende);
                    elencoAnagraficaAziende.Add(anagraficaAziende);
                }
            }
        }

        public static void SalvaAnagraficaAziende(DecodAnagraficaAziendeLetteraB decodAnagraficaAziende) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAziendeLettB decAnagraficaAziende = new DecAnagraficaAziendeLettB(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAziende, decAnagraficaAziende);
                DAGestioneAnagraficaAziendeLetteraB.SalvaAnagraficaAziende(decAnagraficaAziende);/*salva oggetto del blcommon nell'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        public static void DeleteAnagraficaAziende(DecodAnagraficaAziendeLetteraB decodAnagraficaAziende)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAziendeLettB decAnagraficaAziende = new DecAnagraficaAziendeLettB(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAziende, decAnagraficaAziende);
                DAGestioneAnagraficaAziendeLetteraB.DeleteAnagraficaAziende(decAnagraficaAziende);/*delete oggetto del blcommon dall'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        #region nested class
        #region class DecodAnagraficaAziendeLetteraB
        /// <summary>
        /// nested class del BLcommon "gemella" della tabella DB
        /// </summary>
        public class DecodAnagraficaAziendeLetteraB
        {
            public long Id { get; set; }
            public string DenominazioneAzienda { get; set; }
            public string SottogruppoPrimoOnere { get; set; }
            public string SottogruppoSecondoOnere { get; set; }

            /// <summary>
            /// confronto personalizzato del contenuto dell'oggetto in input con altro oggetto, es. dal DB
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                DecodAnagraficaAziendeLetteraB anagraficaAziendeDB = (DecodAnagraficaAziendeLetteraB)obj;
                try
                {
                    if (this.DenominazioneAzienda != anagraficaAziendeDB.DenominazioneAzienda ||
                        this.SottogruppoPrimoOnere != anagraficaAziendeDB.SottogruppoPrimoOnere ||
                        this.SottogruppoSecondoOnere != anagraficaAziendeDB.SottogruppoSecondoOnere)

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
                hash = (hash * 7) + (this.SottogruppoPrimoOnere != null ? this.SottogruppoPrimoOnere.GetHashCode() : 0);
                hash = (hash * 7) + (this.SottogruppoSecondoOnere != null ? this.SottogruppoSecondoOnere.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion class DecodAnagraficaAziendeLetteraB
        #endregion nested class
    }
}
