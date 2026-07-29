using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAnagraficaAccordiPerTipo0171
    {
        public static void GetDecAnagraficaAccordi(out List<DecodAnagraficaAccordiPerTipo0171> elencoAnagraficaAccordi)
        {
            elencoAnagraficaAccordi = null;
            List<DecAnagraficaAccordiPerTipo0171> elencoDecAnagraficaAccordiDB = null;
            DAGestioneAnagraficaAccordiPerTipo0171.GetDecAnagraficaAccordi(out elencoDecAnagraficaAccordiDB);
            if (elencoDecAnagraficaAccordiDB != null && elencoDecAnagraficaAccordiDB.Count > 0)
            {
                elencoAnagraficaAccordi = new List<DecodAnagraficaAccordiPerTipo0171>();
                foreach (DecAnagraficaAccordiPerTipo0171 decodificaAnagraficaAccordiDB in elencoDecAnagraficaAccordiDB)
                {
                    DecodAnagraficaAccordiPerTipo0171 anagraficaAccordi = new DecodAnagraficaAccordiPerTipo0171();
                    Utility.ValorizzaOggetti(decodificaAnagraficaAccordiDB, anagraficaAccordi);
                    if (anagraficaAccordi.Abilitata == true)
                        anagraficaAccordi.AbilitataTxt = "SI";
                    else if (anagraficaAccordi.Abilitata == false)
                        anagraficaAccordi.AbilitataTxt = "NO";
                    elencoAnagraficaAccordi.Add(anagraficaAccordi);
                }
            }
        }

        public static void SalvaAnagraficaAccordi(DecodAnagraficaAccordiPerTipo0171 decodAnagraficaAccordi) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAccordiPerTipo0171 decAnagraficaAccordi = new DecAnagraficaAccordiPerTipo0171(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAccordi, decAnagraficaAccordi);
                DAGestioneAnagraficaAccordiPerTipo0171.SalvaAnagraficaAccordi(decAnagraficaAccordi);/*salva oggetto del blcommon nell'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        public static int DeleteAnagraficaAccordi(DecodAnagraficaAccordiPerTipo0171 decodAnagraficaAccordi)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecAnagraficaAccordiPerTipo0171 decAnagraficaAccordi = new DecAnagraficaAccordiPerTipo0171(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decodAnagraficaAccordi, decAnagraficaAccordi);
                int result = DAGestioneAnagraficaAccordiPerTipo0171.DeleteAnagraficaAccordi(decAnagraficaAccordi);/*delete oggetto del blcommon dall'oggetto del datacommon*/
                transactionScope.Complete();
                return result;
            }
        }

        public static void UpdateCountLiquidate_AnagraficaAccordi(short? codiceAziendaEditoria) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnagraficaAccordiPerTipo0171.UpdateCountLiquidate_AnagraficaAccordi(codiceAziendaEditoria);
                transactionScope.Complete();
            }
        }

        #region nested class
        #region class DecodAnagraficaAccordiPerTipo0171
        /// <summary>
        /// nested class del BLcommon "gemella" della tabella DB
        /// </summary>
        public class DecodAnagraficaAccordiPerTipo0171
        {
            public long Id { get; set; }
            public bool? Abilitata { get; set; }
            public string AbilitataTxt { get; set; }
            public short? Codice { get; set; }
            public long? DenominazioneAzienda { get; set; }
            public DateTime? DataAccordi { get; set; }
            public int? DomandeLiquidabili { get; set; }
            public int? DomandeLiquidate { get; set; }

            /// <summary>
            /// confronto personalizzato del contenuto dell'oggetto in input con altro oggetto, es. dal DB
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                DecodAnagraficaAccordiPerTipo0171 anagraficaAccordiDB = (DecodAnagraficaAccordiPerTipo0171)obj;
                try
                {
                    if (this.Abilitata != anagraficaAccordiDB.Abilitata ||
                        this.Codice != anagraficaAccordiDB.Codice ||
                        this.DenominazioneAzienda != anagraficaAccordiDB.DenominazioneAzienda ||
                        this.DataAccordi != anagraficaAccordiDB.DataAccordi ||
                        this.DomandeLiquidabili != anagraficaAccordiDB.DomandeLiquidabili ||
                        this.DomandeLiquidate != anagraficaAccordiDB.DomandeLiquidate)

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
                hash = (hash * 7) + (this.Abilitata != null ? this.Abilitata.GetHashCode() : 0);
                hash = (hash * 7) + (this.Codice != null ? this.Codice.GetHashCode() : 0);
                hash = (hash * 7) + (this.DenominazioneAzienda != null ? this.DenominazioneAzienda.GetHashCode() : 0);
                hash = (hash * 7) + (this.DataAccordi != null ? this.DataAccordi.GetHashCode() : 0);
                hash = (hash * 7) + (this.DomandeLiquidabili != null ? this.DomandeLiquidabili.GetHashCode() : 0);
                hash = (hash * 7) + (this.DomandeLiquidate != null ? this.DomandeLiquidate.GetHashCode() : 0);

                return hash;
            }
        }
        #endregion class DecodAnagraficaAccordiPerTipo0171
        #endregion nested class
    }
}
