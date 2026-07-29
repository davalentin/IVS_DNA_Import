using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneQuotaFondoINPGI
    {
        #region Contributivo

        public static void GetCalcoloContributivoINPGIByIdPensione(long idPensione, out List<DatiCalcoloContributivoINPGI> ldatiCalcoloContributivo)
        {
            ldatiCalcoloContributivo = null;
            List<CalcoloContributivoINPGI> lcalcoloContributivo = null;

            DAGestioneQuotaFondoINPGI.GetCalcoloContributivoINPGIByIdPensione(idPensione, out lcalcoloContributivo);
            if (lcalcoloContributivo == null || lcalcoloContributivo.Count == 0)
                return;
            ldatiCalcoloContributivo = new List<DatiCalcoloContributivoINPGI>();

            foreach (CalcoloContributivoINPGI cr in lcalcoloContributivo)
            {
                DatiCalcoloContributivoINPGI dcrApp = new DatiCalcoloContributivoINPGI();
                Utility.ValorizzaOggetti(cr, dcrApp);
                ldatiCalcoloContributivo.Add(dcrApp);
            }
        }

        public static void GetCalcoloContributivoINPGIStoricoByIdPensione(long idPensione, out List<DatiCalcoloContributivoINPGI> ldatiCalcoloContributivo)
        {
            ldatiCalcoloContributivo = null;
            List<CalcoloContributivoINPGI> lcalcoloContributivo = null;

            DAGestioneQuotaFondoINPGI.GetCalcoloContributivoINPGIStoricoByIdPensione(idPensione, out lcalcoloContributivo);
            if (lcalcoloContributivo == null || lcalcoloContributivo.Count == 0)
                return;
            ldatiCalcoloContributivo = new List<DatiCalcoloContributivoINPGI>();

            foreach (CalcoloContributivoINPGI cr in lcalcoloContributivo)
            {
                DatiCalcoloContributivoINPGI dcrApp = new DatiCalcoloContributivoINPGI();
                Utility.ValorizzaOggetti(cr, dcrApp);
                ldatiCalcoloContributivo.Add(dcrApp);
            }
        }

        public static void SalvaCalcoloContributivoINPGI(DatiCalcoloContributivoINPGI datiCalcoloContributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloContributivoINPGI calcoloContributivo = new CalcoloContributivoINPGI();
                Utility.ValorizzaOggetti(datiCalcoloContributivo, calcoloContributivo);
                DAGestioneQuotaFondoINPGI.SalvaCalcoloContributivoINPGI(calcoloContributivo);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloContributivoINPGIByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneQuotaFondoINPGI.EliminaCalcoloContributivoINPGIByIdPensione(idPensione);
                else
                    DAGestioneQuotaFondoINPGI.EliminaCalcoloContributivoINPGINoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void SalvaListaCalcoloContributivoINPGI(List<DatiCalcoloContributivoINPGI> LdatiCalcoloContributivoINPGI)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiCalcoloContributivoINPGI datiCalcoloContributivo in LdatiCalcoloContributivoINPGI)
                    SalvaCalcoloContributivoINPGI(datiCalcoloContributivo);
                transactionScope.Complete();
            }
        }

        #endregion Contributivo

        #region Retributivo

        public static void GetCalcoloRetributivoINPGIByIdPensione(long idPensione, out List<DatiCalcoloRetributivoINPGI> ldatiCalcoloRetributivo)
        {
            ldatiCalcoloRetributivo = null;
            List<CalcoloRetributivoINPGI> lcalcoloRetributivo = null;

            DAGestioneQuotaFondoINPGI.GetCalcoloRetributivoINPGIByIdPensione(idPensione, out lcalcoloRetributivo);
            if (lcalcoloRetributivo == null || lcalcoloRetributivo.Count == 0)
                return;
            ldatiCalcoloRetributivo = new List<DatiCalcoloRetributivoINPGI>();

            foreach (CalcoloRetributivoINPGI cr in lcalcoloRetributivo)
            {
                DatiCalcoloRetributivoINPGI dcrApp = new DatiCalcoloRetributivoINPGI();
                Utility.ValorizzaOggetti(cr, dcrApp);
                ldatiCalcoloRetributivo.Add(dcrApp);
            }
        }

        public static void GetCalcoloRetributivoINPGIStoricoByIdPensione(long idPensione, out List<DatiCalcoloRetributivoINPGI> ldatiCalcoloRetributivo)
        {
            ldatiCalcoloRetributivo = null;
            List<CalcoloRetributivoINPGI> lcalcoloRetributivo = null;

            DAGestioneQuotaFondoINPGI.GetCalcoloRetributivoINPGIStoricoByIdPensione(idPensione, out lcalcoloRetributivo);
            if (lcalcoloRetributivo == null || lcalcoloRetributivo.Count == 0)
                return;
            ldatiCalcoloRetributivo = new List<DatiCalcoloRetributivoINPGI>();

            foreach (CalcoloRetributivoINPGI cr in lcalcoloRetributivo)
            {
                DatiCalcoloRetributivoINPGI dcrApp = new DatiCalcoloRetributivoINPGI();
                Utility.ValorizzaOggetti(cr, dcrApp);
                ldatiCalcoloRetributivo.Add(dcrApp);
            }
        }

        public static void SalvaCalcoloRetributivoINPGI(DatiCalcoloRetributivoINPGI datiCalcoloRetributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloRetributivoINPGI calcoloRetributivo = new CalcoloRetributivoINPGI();
                Utility.ValorizzaOggetti(datiCalcoloRetributivo, calcoloRetributivo);
                DAGestioneQuotaFondoINPGI.SalvaCalcoloRetributivoINPGI(calcoloRetributivo);
                transactionScope.Complete();
            }
        }

        public static void SalvaListaCalcoloRetributivoINPGI(List<DatiCalcoloRetributivoINPGI> LdatiCalcoloRetributivoINPGI)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiCalcoloRetributivoINPGI datiCalcoloRetributivo in LdatiCalcoloRetributivoINPGI)
                    SalvaCalcoloRetributivoINPGI(datiCalcoloRetributivo);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloRetributivoINPGIByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneQuotaFondoINPGI.EliminaCalcoloRetributivoINPGIByIdPensione(idPensione);
                else
                    DAGestioneQuotaFondoINPGI.EliminaCalcoloRetributivoINPGINoStoricoByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }
        #endregion Retributivo

        #region nested class
        public class DatiCalcoloContributivoINPGI
        {
            private bool _IsStorico;

            #region public properties
            public long? Id { get; set; }
            public long? IdPensione { get; set; }
            public long? CodiceGestione { get; set; }
            public decimal? Montante { get; set; }
            public decimal? Quota { get; set; }
            public int? Settimane { get; set; }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
            #endregion public properties

        }

        public class DatiCalcoloRetributivoINPGI
        {
            private bool _IsStorico;

            #region public properties
            public long? Id { get; set; }
            public long? IdPensione { get; set; }
            public long? CodiceGestione { get; set; }
            public int? Settimane { get; set; }
            public decimal? ImportoCalcolato { get; set; }
            public decimal? ImportoComma707 { get; set; }
            public int? SettimaneComma707 { get; set; }
            public decimal? RetribuzioneMediaSettimanale { get; set; }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

            #endregion public properties

        }
        #endregion nested class
    }
}
