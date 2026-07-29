using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneQuotaFondoIntegrativo
    {
        public static void GetQuotaFondoIntegrativoByIdPensione(Int64 idPensione, out List<DatiQuotaFondoIntegrativo> datiQuotaFondoIntegrativo)
        {
            List<QuotaFondoIntegrativo> quotaFondoIntegrativo = null;
            datiQuotaFondoIntegrativo = null;
            DAGestioneQuotaFondoIntegrativo.GetQuotaFondoIntegrativoByIdPensione(idPensione, out quotaFondoIntegrativo);
            if (quotaFondoIntegrativo == null || quotaFondoIntegrativo.Count == 0)
                return;
            datiQuotaFondoIntegrativo = new List<DatiQuotaFondoIntegrativo>();
            foreach (QuotaFondoIntegrativo quota in quotaFondoIntegrativo)
            {
                DatiQuotaFondoIntegrativo datiQuota = new DatiQuotaFondoIntegrativo();
                Utility.ValorizzaOggetti(quota, datiQuota);
                datiQuotaFondoIntegrativo.Add(datiQuota);
            }
        }

        public static void GetQuotaFondoIntegrativoStoricoByIdPensione(Int64 idPensione, out List<DatiQuotaFondoIntegrativo> datiQuotaFondoIntegrativoStorico)
        {
            List<QuotaFondoIntegrativo> quotaFondoIntegrativoStorico = null;
            datiQuotaFondoIntegrativoStorico = null;
            DAGestioneQuotaFondoIntegrativo.GetQuotaFondoIntegrativoStoricoByIdPensione(idPensione, out quotaFondoIntegrativoStorico);
            if (quotaFondoIntegrativoStorico == null || quotaFondoIntegrativoStorico.Count == 0)
                return;
            datiQuotaFondoIntegrativoStorico = new List<DatiQuotaFondoIntegrativo>();
            foreach (QuotaFondoIntegrativo quota in quotaFondoIntegrativoStorico)
            {
                DatiQuotaFondoIntegrativo datiQuota = new DatiQuotaFondoIntegrativo();
                Utility.ValorizzaOggetti(quota, datiQuota);
                datiQuotaFondoIntegrativoStorico.Add(datiQuota);
            }
        }

        public static void SalvaQuotaFondoIntegrativo(long idPensione, DatiQuotaFondoIntegrativo datiQuotaFondoIntegrativo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                QuotaFondoIntegrativo quotaFondoIntegrativo = new QuotaFondoIntegrativo();
                Utility.ValorizzaOggetti(datiQuotaFondoIntegrativo, quotaFondoIntegrativo);
                quotaFondoIntegrativo.IdPensione = idPensione;
                DAGestioneQuotaFondoIntegrativo.SalvaQuotaFondoIntegrativo(quotaFondoIntegrativo);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuotaFondoIntegrativoByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneQuotaFondoIntegrativo.EliminaQuotaFondoIntegrativoByIdPensione(idPensione);
                else
                    DAGestioneQuotaFondoIntegrativo.EliminaQuotaFondoIntegrativoNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        //ENG - RIC Esattoriali: gestiti i flussi per il recupero dei dati dal prelievo
        public static void SalvaListaQuotaFondoIntegrativo(long idPensione, List<DatiQuotaFondoIntegrativo> LdatiQuotaFondoIntegrativo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiQuotaFondoIntegrativo datiQuotaFondoIntegrativo in LdatiQuotaFondoIntegrativo)
                    SalvaQuotaFondoIntegrativo(idPensione, datiQuotaFondoIntegrativo);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiQuotaFondoIntegrativo
        {
            #region public properties
            public long? IdPensione { get; set; }
            public long? CodiceGestione { get; set; }
            public decimal? Montante { get; set; }
            public decimal? ImportoContributivoTotale { get; set; }
            public int? NSettimane { get; set; }
            public decimal? MontanteQuotaD { get; set; }
            public decimal? ImportoContribTotaleQuotaD { get; set; }
            public int? NSettimaneQuotaD { get; set; }
            public char? Quota { get; set; }
            public decimal? PL_Quotac { get; set; }
            public bool IsStorico { get; set; }

            #endregion public properties

        }
        #endregion nested class
    }
}
