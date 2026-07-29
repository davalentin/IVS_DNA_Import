using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAnagraficaAziendePerTipo0179
    {
        public static void GetDecAnagraficaAziende(out List<DecodAnagraficaAziendePerTipo0179> elencoAnagraficaAziende)
        {
            elencoAnagraficaAziende = null;
            List<DecAnagraficaAziendePerTipo0179> elencoDecAnagraficaAziendeDB = null;
            DAGestioneAnagraficaAziendePerTipo0179.GetDecAnagraficaAziende(out elencoDecAnagraficaAziendeDB);
            if (elencoDecAnagraficaAziendeDB != null && elencoDecAnagraficaAziendeDB.Count > 0)
            {
                elencoAnagraficaAziende = new List<DecodAnagraficaAziendePerTipo0179>();
                foreach (DecAnagraficaAziendePerTipo0179 decodificaAnagraficaAziendeDB in elencoDecAnagraficaAziendeDB)
                {
                    DecodAnagraficaAziendePerTipo0179 anagraficaAziende = new DecodAnagraficaAziendePerTipo0179();
                    Utility.ValorizzaOggetti(decodificaAnagraficaAziendeDB, anagraficaAziende);
                    elencoAnagraficaAziende.Add(anagraficaAziende);
                }
            }
        }

        public static void SalvaAnagraficaAziende(DecodAnagraficaAziendePerTipo0179 decodAnagraficaAziende) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAziendePerTipo0179 decAnagraficaAziende = new DecAnagraficaAziendePerTipo0179(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAziende, decAnagraficaAziende);
                DAGestioneAnagraficaAziendePerTipo0179.SalvaAnagraficaAziende(decAnagraficaAziende);/*salva oggetto del blcommon nell'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        public static void DeleteAnagraficaAziende(DecodAnagraficaAziendePerTipo0179 decodAnagraficaAziende)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAziendePerTipo0179 decAnagraficaAziende = new DecAnagraficaAziendePerTipo0179(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAziende, decAnagraficaAziende);
                DAGestioneAnagraficaAziendePerTipo0179.DeleteAnagraficaAziende(decAnagraficaAziende);/*delete oggetto del blcommon dall'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        #region nested class
        #region class DecodAnagraficaAziendePerTipo0179
        /// <summary>
        /// nested class del BLcommon "gemella" della tabella DB
        /// </summary>
        public class DecodAnagraficaAziendePerTipo0179
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
                DecodAnagraficaAziendePerTipo0179 anagraficaAziendeDB = (DecodAnagraficaAziendePerTipo0179)obj;
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
        #endregion class DecodAnagraficaAziendePerTipo0179
        #endregion nested class
    }
}
