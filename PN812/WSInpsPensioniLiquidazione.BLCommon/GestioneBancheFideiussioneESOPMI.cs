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
    public class GestioneBancheFideiussioneESOPMI
    {
        public static void GetDecodificaBancaFideiussione(out List<DecBancaFideiussione> elencoBancaFideiussione)
        {
            elencoBancaFideiussione = null;
            List<DecodificaBancaFideiussoriaESOPMI> elencoDecodificaBancaFideiussioneDB = null;
            DAGestioneBancheFideiussioneESOPMI.GetDecodificaBancaFideiussione(out elencoDecodificaBancaFideiussioneDB);
            if (elencoDecodificaBancaFideiussioneDB != null && elencoDecodificaBancaFideiussioneDB.Count > 0)
            {
                elencoBancaFideiussione = new List<DecBancaFideiussione>();
                foreach (DecodificaBancaFideiussoriaESOPMI decodificaBancaFideiussioneDB in elencoDecodificaBancaFideiussioneDB)
                {
                    DecBancaFideiussione bancaFideiussione = new DecBancaFideiussione();
                    Utility.ValorizzaOggetti(decodificaBancaFideiussioneDB, bancaFideiussione);
                    elencoBancaFideiussione.Add(bancaFideiussione);
                }
            }
        }

        public static void SalvaBancaFideiussione(DecBancaFideiussione decBancaFideiussione) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecodificaBancaFideiussoriaESOPMI decodificaBancaFideiussoria = new DecodificaBancaFideiussoriaESOPMI(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decBancaFideiussione, decodificaBancaFideiussoria);
                DAGestioneBancheFideiussioneESOPMI.SalvaBancaFideiussoria(decodificaBancaFideiussoria);/*salva oggetto del blcommon nell'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        public static void DeleteBancaFideiussione(DecBancaFideiussione decBancaFideiussione)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecodificaBancaFideiussoriaESOPMI decodificaBancaFideiussoria = new DecodificaBancaFideiussoriaESOPMI(); /*oggetto del datacommon*/

                Utility.ValorizzaOggetti(decBancaFideiussione, decodificaBancaFideiussoria);
                DAGestioneBancheFideiussioneESOPMI.DeleteBancaFideiussoria(decodificaBancaFideiussoria);/*salva oggetto del blcommon nell'oggetto del datacommon*/
                transactionScope.Complete();
            }
        }

        #region nested class
        #region class DecBancaFideiussione
        /// <summary>
        /// nested class del BLcommon "gemella" della tabella DB
        /// </summary>
        public class DecBancaFideiussione
        {
            public long Id { get; set; }
            public string CodiceAzienda { get; set; }
            public string Matricola { get; set; }
            public string BancaFideiussione { get; set; }
            public byte? Progressivo { get; set; }
            public short? Anno { get; set; }
            public DateTime? InizioEsodo { get; set; }
            public DateTime? FineEsodo { get; set; }
            public int? ABI { get; set; }
            public int? CAB { get; set; }

            /// <summary>
            /// confronto personalizzato del contenuto dell'oggetto in input con altro oggetto, es. dal DB
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                DecBancaFideiussione bancheDB = (DecBancaFideiussione)obj;
                try
                {
                    if (this.CodiceAzienda != bancheDB.CodiceAzienda ||
                        this.Matricola != bancheDB.Matricola ||
                        this.BancaFideiussione != bancheDB.BancaFideiussione ||
                        this.Progressivo != bancheDB.Progressivo ||
                        this.Anno != bancheDB.Anno ||
                        this.InizioEsodo != bancheDB.InizioEsodo ||
                        this.FineEsodo != bancheDB.FineEsodo ||
                        this.ABI != bancheDB.ABI ||
                        this.CAB != bancheDB.CAB)

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
                hash = (hash * 7) + (this.CodiceAzienda != null ? this.CodiceAzienda.GetHashCode() : 0);
                hash = (hash * 7) + (this.Matricola != null ? this.Matricola.GetHashCode() : 0);
                hash = (hash * 7) + (this.BancaFideiussione != null ? this.BancaFideiussione.GetHashCode() : 0);
                hash = (hash * 7) + (this.Progressivo != null ? this.Progressivo.GetHashCode() : 0);
                hash = (hash * 7) + (this.Anno != null ? this.Anno.GetHashCode() : 0);
                hash = (hash * 7) + (this.InizioEsodo != null ? this.InizioEsodo.GetHashCode() : 0);
                hash = (hash * 7) + (this.FineEsodo != null ? this.FineEsodo.GetHashCode() : 0);
                hash = (hash * 7) + (this.ABI != null ? this.ABI.GetHashCode() : 0);
                hash = (hash * 7) + (this.CAB != null ? this.CAB.GetHashCode() : 0);
                return hash;
            }
        }
        #endregion class DecBancaFideiussione
        #endregion nested class
    }
}
