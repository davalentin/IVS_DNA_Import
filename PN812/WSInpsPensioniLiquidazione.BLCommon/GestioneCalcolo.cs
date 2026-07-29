using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCalcolo
    {
        #region FS

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloContributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="datiCalcoloContributivo"></param>
        public static void GetCalcoloContributivoByIdPensione(long idPensione, out DatiCalcoloContributivo datiCalcoloContributivo)
        {
            CalcoloContributivo calcoloContributivo = null;
            datiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoByIdPensione(idPensione, out calcoloContributivo);
            if (calcoloContributivo == null)
                return;
            datiCalcoloContributivo = new DatiCalcoloContributivo();
            Utility.ValorizzaOggetti(calcoloContributivo, datiCalcoloContributivo);
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloContributivo per FS nei casi di domande con il quadro Record Fondo
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaDatiCalcoloContributivo"></param>
        public static void GetCalcoloContributivoRecordFondoByIdPensione(long idPensione, out List<DatiCalcoloContributivo> listaDatiCalcoloContributivo)
        {
            List<CalcoloContributivo> listaCalcoloContributivo = null;
            listaDatiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(idPensione, out listaCalcoloContributivo);
            if (listaCalcoloContributivo == null || listaCalcoloContributivo.Count == 0)
                return;
            listaDatiCalcoloContributivo = new List<DatiCalcoloContributivo>();
            foreach (var item in listaCalcoloContributivo)
            {
                DatiCalcoloContributivo datiCalcoloContributivo = new DatiCalcoloContributivo();
                Utility.ValorizzaOggetti(item, datiCalcoloContributivo);
                listaDatiCalcoloContributivo.Add(datiCalcoloContributivo);
            }
        }

        public static void GetCalcoloContributivoByIdRecordFondo(long idRecordFondo, out DatiCalcoloContributivo datiCalcoloContributivo)
        {
            CalcoloContributivo calcoloContributivo = null;
            datiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(idRecordFondo, out calcoloContributivo);
            if (calcoloContributivo == null)
                return;
            datiCalcoloContributivo = new DatiCalcoloContributivo();
            Utility.ValorizzaOggetti(calcoloContributivo, datiCalcoloContributivo);
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloContributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="datiCalcoloContributivo"></param>
        public static void GetCalcoloContributivoStoricoByIdPensione(long idPensione, out DatiCalcoloContributivo datiCalcoloContributivo)
        {
            CalcoloContributivo calcoloContributivo = null;
            datiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoStoricoByIdPensione(idPensione, out calcoloContributivo);
            if (calcoloContributivo == null)
                return;
            datiCalcoloContributivo = new DatiCalcoloContributivo();
            Utility.ValorizzaOggetti(calcoloContributivo, datiCalcoloContributivo);
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloContributivo per FS nei casi di domande con il quadro Record Fondo
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaDatiCalcoloContributivo"></param>
        public static void GetCalcoloContributivoStoricoRecordFondoByIdPensione(long idPensione, out List<DatiCalcoloContributivo> listaDatiCalcoloContributivo)
        {
            List<CalcoloContributivo> listaCalcoloContributivo = null;
            listaDatiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoStoricoRecordFondoByIdPensione(idPensione, out listaCalcoloContributivo);
            if (listaCalcoloContributivo == null || listaCalcoloContributivo.Count == 0)
                return;
            listaDatiCalcoloContributivo = new List<DatiCalcoloContributivo>();
            foreach (var item in listaCalcoloContributivo)
            {
                DatiCalcoloContributivo datiCalcoloContributivo = new DatiCalcoloContributivo();
                Utility.ValorizzaOggetti(item, datiCalcoloContributivo);
                listaDatiCalcoloContributivo.Add(datiCalcoloContributivo);
            }
        }

        public static void SalvaCalcoloContributivo(DatiCalcoloContributivo datiCalcoloContributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloContributivo calcoloContributivo = new CalcoloContributivo();
                Utility.ValorizzaOggetti(datiCalcoloContributivo, calcoloContributivo);
                DAGestioneCalcolo.SalvaCalcoloContributivo(calcoloContributivo);
                transactionScope.Complete();
            }
        }

        public static void SalvaCalcoloContributivoRecordFondo(DatiCalcoloContributivo datiCalcoloContributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloContributivo calcoloContributivo = new CalcoloContributivo();
                Utility.ValorizzaOggetti(datiCalcoloContributivo, calcoloContributivo);
                DAGestioneCalcolo.SalvaCalcoloContributivoRecordFondo(calcoloContributivo);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloContributivoByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloContributivoByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaCalcoloContributivoNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloContributivoByIdRecordFondo(long idRecordFondo, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloContributivoByIdRecordFondo(idRecordFondo);
                else
                    DAGestioneCalcolo.EliminaCalcoloContributivoNoStoricoByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloRetributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="datiCalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoByIdPensione(long idPensione, out DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            CalcoloRetributivo calcoloRetributivo = null;
            datiCalcoloRetributivo = null;
            DAGestioneCalcolo.GetCalcoloRetributivoByIdPensione(idPensione, out calcoloRetributivo);
            if (calcoloRetributivo == null)
                return;
            datiCalcoloRetributivo = new DatiCalcoloRetributivo();
            Utility.ValorizzaOggetti(calcoloRetributivo, datiCalcoloRetributivo);
        }

        public static void GetCalcoloRetributivoByIdRecordFondo(long idRecordFondo, out DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            CalcoloRetributivo calcoloRetributivo = null;
            datiCalcoloRetributivo = null;
            DAGestioneCalcolo.GetCalcoloRetributivoByIdRecordFondo(idRecordFondo, out calcoloRetributivo);
            if (calcoloRetributivo == null)
                return;
            datiCalcoloRetributivo = new DatiCalcoloRetributivo();
            Utility.ValorizzaOggetti(calcoloRetributivo, datiCalcoloRetributivo);
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloRetributivo per FS nei casi di domande con il quadro Record Fondo
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaDatiCalcoloContributivo"></param>
        public static void GetCalcoloRetributivoRecordFondoByIdPensione(long idPensione, out List<DatiCalcoloRetributivo> listaDatiCalcoloRetributivo)
        {
            List<CalcoloRetributivo> listaCalcoloRetributivo = null;
            listaDatiCalcoloRetributivo = null;
            DAGestioneCalcolo.GetCalcoloRetributivoRecordFondoByIdPensione(idPensione, out listaCalcoloRetributivo);
            if (listaCalcoloRetributivo == null || listaCalcoloRetributivo.Count == 0)
                return;
            listaDatiCalcoloRetributivo = new List<DatiCalcoloRetributivo>();
            foreach (var item in listaCalcoloRetributivo)
            {
                DatiCalcoloRetributivo datiCalcoloRetributivo = new DatiCalcoloRetributivo();
                Utility.ValorizzaOggetti(item, datiCalcoloRetributivo);
                listaDatiCalcoloRetributivo.Add(datiCalcoloRetributivo);
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloRetributivo per FS
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="datiCalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoStoricoByIdPensione(long idPensione, out DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            CalcoloRetributivo calcoloRetributivo = null;
            datiCalcoloRetributivo = null;
            DAGestioneCalcolo.GetCalcoloRetributivoStoricoByIdPensione(idPensione, out calcoloRetributivo);
            if (calcoloRetributivo == null)
                return;
            datiCalcoloRetributivo = new DatiCalcoloRetributivo();
            Utility.ValorizzaOggetti(calcoloRetributivo, datiCalcoloRetributivo);
        }

        public static void EliminaCalcoloRetributivoByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaCalcoloRetributivoNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloRetributivoByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcolo.EliminaCalcoloRetributivoByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }


        public static void SalvaCalcoloRetributivo(DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloRetributivo calcoloRetributivo = new CalcoloRetributivo();
                Utility.ValorizzaOggetti(datiCalcoloRetributivo, calcoloRetributivo);
                DAGestioneCalcolo.SalvaCalcoloRetributivo(calcoloRetributivo);
                transactionScope.Complete();
            }
        }

        public static void SalvaCalcoloRetributivoRecordFondo(DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloRetributivo calcoloRetributivo = new CalcoloRetributivo();
                Utility.ValorizzaOggetti(datiCalcoloRetributivo, calcoloRetributivo);
                DAGestioneCalcolo.SalvaCalcoloRetributivoRecordFondo(calcoloRetributivo);
                transactionScope.Complete();
            }
        }

        #endregion FS

        #region AGO_CI

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloRetributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="ldatiCalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoCI_AGOByIdPensione(long idPensione, out List<DatiCalcoloRetributivo> ldatiCalcoloRetributivo)
        {
            ldatiCalcoloRetributivo = null;
            List<CalcoloRetributivo> lcalcoloRetributivo = null;

            DAGestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(idPensione, out lcalcoloRetributivo);
            if (lcalcoloRetributivo == null || lcalcoloRetributivo.Count == 0)
                return;
            //lcalcoloRetributivo.Sort((x, y) => string.Compare(x.QuotePrimeLiquidate.GetValueOrDefault().ToString(), y.QuotePrimeLiquidate.GetValueOrDefault().ToString()));
            ldatiCalcoloRetributivo = new List<DatiCalcoloRetributivo>();

            foreach (CalcoloRetributivo cr in lcalcoloRetributivo)
            {
                DatiCalcoloRetributivo dcrApp = new DatiCalcoloRetributivo();
                Utility.ValorizzaOggetti(cr, dcrApp);
                ldatiCalcoloRetributivo.Add(dcrApp);
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloRetributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="ldatiCalcoloRetributivo"></param>
        public static void GetCalcoloRetributivoStoricoCI_AGOByIdPensione(long idPensione, out List<DatiCalcoloRetributivo> ldatiCalcoloRetributivo)
        {
            ldatiCalcoloRetributivo = null;
            List<CalcoloRetributivo> lcalcoloRetributivo = null;

            DAGestioneCalcolo.GetCalcoloRetributivoStoricoCI_AGOByIdPensione(idPensione, out lcalcoloRetributivo);
            if (lcalcoloRetributivo == null || lcalcoloRetributivo.Count == 0)
                return;
            ldatiCalcoloRetributivo = new List<DatiCalcoloRetributivo>();

            foreach (CalcoloRetributivo cr in lcalcoloRetributivo)
            {
                DatiCalcoloRetributivo dcrApp = new DatiCalcoloRetributivo();
                Utility.ValorizzaOggetti(cr, dcrApp);
                ldatiCalcoloRetributivo.Add(dcrApp);
            }
        }

        public static void SalvaCalcoloRetributivoCI_AGO(DatiCalcoloRetributivo datiCalcoloRetributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloRetributivo calcoloRetributivo = new CalcoloRetributivo();
                Utility.ValorizzaOggetti(datiCalcoloRetributivo, calcoloRetributivo);
                DAGestioneCalcolo.SalvaCalcoloRetributivoCI_AGO(calcoloRetributivo);
                transactionScope.Complete();
            }
        }

        public static void SalvaListaCalcoloRetributivoCI_AGO(List<DatiCalcoloRetributivo> lDatiCalcoloRetributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiCalcoloRetributivo datiCalcoloRetributivo in lDatiCalcoloRetributivo)
                    SalvaCalcoloRetributivoCI_AGO(datiCalcoloRetributivo);
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella CalcoloContributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="ldatiCalcoloContributivo"></param>
        public static void GetCalcoloContributivoCI_AGOByIdPensione(long idPensione, out List<DatiCalcoloContributivo> ldatiCalcoloContributivo)
        {
            ldatiCalcoloContributivo = null;
            List<CalcoloContributivo> lcalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(idPensione, out lcalcoloContributivo);
            if (lcalcoloContributivo == null || lcalcoloContributivo.Count == 0)
                return;
            //lcalcoloContributivo.Sort((x, y) => string.Compare(x.MontanteQuotaDL214.GetValueOrDefault().ToString(), y.MontanteQuotaDL214.GetValueOrDefault().ToString()));
            ldatiCalcoloContributivo = new List<DatiCalcoloContributivo>();
            foreach (CalcoloContributivo cc in lcalcoloContributivo)
            {
                DatiCalcoloContributivo dccApp = new DatiCalcoloContributivo();
                Utility.ValorizzaOggetti(cc, dccApp);
                ldatiCalcoloContributivo.Add(dccApp);
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella CalcoloContributivo per AGO e CI
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="ldatiCalcoloContributivo"></param>
        public static void GetCalcoloContributivoStoricoCI_AGOByIdPensione(long idPensione, out List<DatiCalcoloContributivo> ldatiCalcoloContributivo)
        {
            ldatiCalcoloContributivo = null;
            List<CalcoloContributivo> lcalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoStoricoCI_AGOByIdPensione(idPensione, out lcalcoloContributivo);
            if (lcalcoloContributivo == null || lcalcoloContributivo.Count == 0)
                return;
            ldatiCalcoloContributivo = new List<DatiCalcoloContributivo>();
            foreach (CalcoloContributivo cc in lcalcoloContributivo)
            {
                DatiCalcoloContributivo dccApp = new DatiCalcoloContributivo();
                Utility.ValorizzaOggetti(cc, dccApp);
                ldatiCalcoloContributivo.Add(dccApp);
            }
        }

        public static void SalvaCalcoloContributivoCI_AGO(DatiCalcoloContributivo datiCalcoloContributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloContributivo calcoloContributivo = new CalcoloContributivo();
                Utility.ValorizzaOggetti(datiCalcoloContributivo, calcoloContributivo);
                DAGestioneCalcolo.SalvaCalcoloContributivoCI_AGO(calcoloContributivo);
                transactionScope.Complete();
            }
        }

        public static void SalvaListCalcoloContributivoCI_AGO(List<DatiCalcoloContributivo> lDatiCalcoloContributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiCalcoloContributivo datiCalcoloContributivo in lDatiCalcoloContributivo)
                    SalvaCalcoloContributivoCI_AGO(datiCalcoloContributivo);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloContributivoCI_AGOByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloContributivoByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaCalcoloContributivoNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloRetributivoCI_AGOByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaCalcoloRetributivoNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void GetCalcoloContributivoEsteroCIbyIdPensione(long idPensione, out List<DatiCalcoloContributivoEstero> ldatiCalcoloContributivoEstero)
        {
            ldatiCalcoloContributivoEstero = null;
            List<PensioniCiContributiEE> lCalcoloContributivoEsteroDB = null;
            DAGestioneCalcolo.GetPensioniCiContributiEE_ByIdPensione(idPensione, out lCalcoloContributivoEsteroDB);
            if (lCalcoloContributivoEsteroDB == null || lCalcoloContributivoEsteroDB.Count == 0)
                return;
            ldatiCalcoloContributivoEstero = new List<DatiCalcoloContributivoEstero>();
            foreach (PensioniCiContributiEE ee in lCalcoloContributivoEsteroDB)
            {
                DatiCalcoloContributivoEstero eeApp = new DatiCalcoloContributivoEstero();
                Utility.ValorizzaOggetti(ee, eeApp);
                ldatiCalcoloContributivoEstero.Add(eeApp);
            }
        }

        public static void SalvaListCalcoloContributivoEsteroCI(List<DatiCalcoloContributivoEstero> LdatiCalcoloContributivoEstero)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (DatiCalcoloContributivoEstero datiCalcoloContributivoEE in LdatiCalcoloContributivoEstero)
                    SalvaCalcoloContributivoEsteroCI(datiCalcoloContributivoEE);
                transactionScope.Complete();
            }
        }

        public static void SalvaCalcoloContributivoEsteroCI(DatiCalcoloContributivoEstero datiCalcoloContributivoEstero)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioniCiContributiEE calcoloContributivoEEDB = new PensioniCiContributiEE();
                Utility.ValorizzaOggetti(datiCalcoloContributivoEstero, calcoloContributivoEEDB);
                DAGestioneCalcolo.SalvaPensioniCiContributiEE(calcoloContributivoEEDB);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloContributivoEsteroCIByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcolo.EliminaPensioniCiContributiEEByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion AGO_CI

        #region Enpals

        public static void GetCalcoloContributivoEnpalsByIdPensione(long idPensione, out DatiCalcoloContributivoENPAL datiCalcoloContributivo)
        {
            CalcoloContributivoENPAL calcoloContributivo = null;
            datiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoENPALSByIdPensione(idPensione, out calcoloContributivo);
            if (calcoloContributivo == null)
                return;
            datiCalcoloContributivo = new DatiCalcoloContributivoENPAL();
            Utility.ValorizzaOggetti(calcoloContributivo, datiCalcoloContributivo);
        }

        public static void GetCalcoloContributivoEnpalsStoricoByIdPensione(long idPensione, out DatiCalcoloContributivoENPAL datiCalcoloContributivo)
        {
            CalcoloContributivoENPAL calcoloContributivo = null;
            datiCalcoloContributivo = null;
            DAGestioneCalcolo.GetCalcoloContributivoENPALSStoricoByIdPensione(idPensione, out calcoloContributivo);
            if (calcoloContributivo == null)
                return;
            datiCalcoloContributivo = new DatiCalcoloContributivoENPAL();
            Utility.ValorizzaOggetti(calcoloContributivo, datiCalcoloContributivo);
        }

        public static void SalvaCalcoloContributivoEnpals(DatiCalcoloContributivoENPAL datiCalcoloContributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloContributivoENPAL calcoloContributivo = new CalcoloContributivoENPAL();
                Utility.ValorizzaOggetti(datiCalcoloContributivo, calcoloContributivo);
                DAGestioneCalcolo.SalvaCalcoloContributivoEnpals(calcoloContributivo);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloContributivoEnpalsByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloContributivoEnpalsByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaCalcoloContributivoEnpalsNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void GetCalcoloRetributivoEnpalsByIdPensione(long idPensione, out DatiCalcoloRetributivoENPAL datiCalcoloRetributivo)
        {
            CalcoloRetributivoENPAL calcoloRetributivo = null;
            datiCalcoloRetributivo = null;
            DAGestioneCalcolo.GetCalcoloRetributivoEnpalsByIdPensione(idPensione, out calcoloRetributivo);
            if (calcoloRetributivo == null)
                return;
            datiCalcoloRetributivo = new DatiCalcoloRetributivoENPAL();
            Utility.ValorizzaOggetti(calcoloRetributivo, datiCalcoloRetributivo);
        }

        public static void GetCalcoloRetributivoEnpalsStoricoByIdPensione(long idPensione, out DatiCalcoloRetributivoENPAL datiCalcoloRetributivo)
        {
            CalcoloRetributivoENPAL calcoloRetributivo = null;
            datiCalcoloRetributivo = null;
            DAGestioneCalcolo.GetCalcoloRetributivoEnpalsStoricoByIdPensione(idPensione, out calcoloRetributivo);
            if (calcoloRetributivo == null)
                return;
            datiCalcoloRetributivo = new DatiCalcoloRetributivoENPAL();
            Utility.ValorizzaOggetti(calcoloRetributivo, datiCalcoloRetributivo);
        }

        public static void EliminaCalcoloRetributivoEnpalsByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaCalcoloRetributivoEnpalsByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaCalcoloRetributivoEnpalsNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void SalvaCalcoloRetributivoEnpals(DatiCalcoloRetributivoENPAL datiCalcoloRetributivo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloRetributivoENPAL calcoloRetributivo = new CalcoloRetributivoENPAL();
                Utility.ValorizzaOggetti(datiCalcoloRetributivo, calcoloRetributivo);
                DAGestioneCalcolo.SalvaCalcoloRetributivoEnpals(calcoloRetributivo);
                transactionScope.Complete();
            }
        }

        #endregion Enpals

        #region Cumulo L.228/2012

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella QuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lQuotePensione"></param>
        public static void GetQuotePensioneByIdPensione(long idPensione, out List<QuotePensione> lQuotePensione)
        {
            lQuotePensione = null;
            List<DataCommon.QuotePensione> lQuotePensioneDB = null;

            DAGestioneCalcolo.GetQuotePensioneByIdPensione(idPensione, out lQuotePensioneDB);
            if (lQuotePensioneDB == null || lQuotePensioneDB.Count == 0)
                return;

            lQuotePensione = new List<QuotePensione>();

            foreach (DataCommon.QuotePensione quotePensioneDB in lQuotePensioneDB)
            {
                QuotePensione quotePensione = new QuotePensione();
                Utility.ValorizzaOggetti(quotePensioneDB, quotePensione);
                lQuotePensione.Add(quotePensione);
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella QuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lQuotePensione"></param>
        public static void GetQuotePensioneStoricoByIdPensione(long idPensione, out List<QuotePensione> lQuotePensione)
        {
            lQuotePensione = null;
            List<DataCommon.QuotePensione> lQuotePensioneDB = null;

            DAGestioneCalcolo.GetQuotePensioneStoricoByIdPensione(idPensione, out lQuotePensioneDB);
            if (lQuotePensioneDB == null || lQuotePensioneDB.Count == 0)
                return;

            lQuotePensione = new List<QuotePensione>();

            foreach (DataCommon.QuotePensione quotePensioneDB in lQuotePensioneDB)
            {
                QuotePensione quotePensione = new QuotePensione();
                Utility.ValorizzaOggetti(quotePensioneDB, quotePensione);
                lQuotePensione.Add(quotePensione);
            }
        }

        public static void SalvaQuotePensione(QuotePensione quotePensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.QuotePensione quotePensioneDB = new DataCommon.QuotePensione();
                Utility.ValorizzaOggetti(quotePensione, quotePensioneDB);
                DAGestioneCalcolo.SalvaQuotePensione(quotePensioneDB);
                transactionScope.Complete();
            }
        }

        public static void SalvaListaQuotePensione(List<QuotePensione> lQuotePensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (QuotePensione quotePensione in lQuotePensione)
                    SalvaQuotePensione(quotePensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaQuotePensioneByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaQuotePensioneByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaQuotePensioneNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Recupera i record NON di Storico dalla tabella TrattenuteQuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lTrattenuteQuotePensione"></param>
        public static void GetTrattenuteQuotePensioneByIdPensione(long idPensione, out List<TrattenuteQuotePensione> lTrattenuteQuotePensione)
        {
            lTrattenuteQuotePensione = null;
            List<DataCommon.TrattenuteQuotePensione> lTrattenuteQuotePensioneDB = null;

            DAGestioneCalcolo.GetTrattenuteQuotePensioneByIdPensione(idPensione, out lTrattenuteQuotePensioneDB);
            if (lTrattenuteQuotePensioneDB == null || lTrattenuteQuotePensioneDB.Count == 0)
                return;

            lTrattenuteQuotePensione = new List<TrattenuteQuotePensione>();

            foreach (DataCommon.TrattenuteQuotePensione trattenuteDB in lTrattenuteQuotePensioneDB)
            {
                TrattenuteQuotePensione quotePensione = new TrattenuteQuotePensione();
                Utility.ValorizzaOggetti(trattenuteDB, quotePensione);
                lTrattenuteQuotePensione.Add(quotePensione);
            }
        }

        /// <summary>
        /// Recupera i record di Storico dalla tabella TrattenuteQuotePensione
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="lTrattenuteQuotePensione"></param>
        public static void GetTrattenuteQuotePensioneStoricoByIdPensione(long idPensione, out List<TrattenuteQuotePensione> lTrattenuteQuotePensione)
        {
            lTrattenuteQuotePensione = null;
            List<DataCommon.TrattenuteQuotePensione> lTrattenuteQuotePensioneDB = null;

            DAGestioneCalcolo.GetTrattenuteQuotePensioneStoricoByIdPensione(idPensione, out lTrattenuteQuotePensioneDB);
            if (lTrattenuteQuotePensioneDB == null || lTrattenuteQuotePensioneDB.Count == 0)
                return;

            lTrattenuteQuotePensione = new List<TrattenuteQuotePensione>();

            foreach (DataCommon.TrattenuteQuotePensione trattenuteDB in lTrattenuteQuotePensioneDB)
            {
                TrattenuteQuotePensione quotePensione = new TrattenuteQuotePensione();
                Utility.ValorizzaOggetti(trattenuteDB, quotePensione);
                lTrattenuteQuotePensione.Add(quotePensione);
            }
        }

        public static void SalvaListaTrattenuteQuotePensione(List<TrattenuteQuotePensione> lTrattenute)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (TrattenuteQuotePensione trattenute in lTrattenute)
                    SalvaTrattenuteQuotePensione(trattenute);
                transactionScope.Complete();
            }
        }

        public static void SalvaTrattenuteQuotePensione(TrattenuteQuotePensione trattenute)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.TrattenuteQuotePensione trattenuteDB = new DataCommon.TrattenuteQuotePensione();
                Utility.ValorizzaOggetti(trattenute, trattenuteDB);
                DAGestioneCalcolo.SalvaTrattenuteQuotePensione(trattenuteDB);
                transactionScope.Complete();
            }
        }

        public static void EliminaTrattenuteQuotePensioneByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneCalcolo.EliminaTrattenuteQuotePensioneByIdPensione(idPensione);
                else
                    DAGestioneCalcolo.EliminaTrattenuteQuotePensioneNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }
        #endregion Cumulo L.228/2012

        #region Servizio Utile 707

        public static void GetDatiServizioUtile707ByIdPensione(Int64 idPensione, out List<ServizioUtile707> lServizioUtile)
        {
            List<DatiServizioUtile707> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneCalcolo.GetDatiServizioUtile707ByIdPensione(idPensione, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile707>();
            foreach (DatiServizioUtile707 datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile707 servizioUtile = new ServizioUtile707();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtile707ByIdRecordFondo(Int64 idRecordFondo, out List<ServizioUtile707> lServizioUtile)
        {
            List<DatiServizioUtile707> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneCalcolo.GetDatiServizioUtile707ByIdRecordFondo(idRecordFondo, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile707>();
            foreach (DatiServizioUtile707 datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile707 servizioUtile = new ServizioUtile707();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtileINPDAP707ByIdPensione(Int64 idPensione, out List<ServizioUtileINPDAP707> lServizioUtile)
        {
            List<DatiServizioUtileINPDAP707> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneCalcolo.GetDatiServizioUtileINPDAP707ByIdPensione(idPensione, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtileINPDAP707>();
            foreach (DatiServizioUtileINPDAP707 datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtileINPDAP707 servizioUtile = new ServizioUtileINPDAP707();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtileINPDAP707ByIdRecordFondo(Int64 idRecordFondo, out List<ServizioUtileINPDAP707> lServizioUtile)
        {
            List<DatiServizioUtileINPDAP707> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneCalcolo.GetDatiServizioUtileINPDAP707ByIdRecordFondo(idRecordFondo, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtileINPDAP707>();
            foreach (DatiServizioUtileINPDAP707 datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtileINPDAP707 servizioUtile = new ServizioUtileINPDAP707();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void SalvaDatiServizioUtile707(long idFondo, ServizioUtile707 servizioUtile707)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.DatiServizioUtile707 datiServizioUtile707 = new DataCommon.DatiServizioUtile707();
                Utility.ValorizzaOggetti(servizioUtile707, datiServizioUtile707);
                datiServizioUtile707.IdFondo = idFondo;
                DAGestioneCalcolo.SalvaDatiServizioUtile707(datiServizioUtile707);
                transactionScope.Complete();
            }
        }

        public static void SalvaDatiServizioUtile707RecordFondo(long idFondo, long idRecordFondo, ServizioUtile707 servizioUtile)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.DatiServizioUtile707 datiServizioUtile707 = new DataCommon.DatiServizioUtile707();
                Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile707);
                datiServizioUtile707.IdFondo = idFondo;
                datiServizioUtile707.IdRecordFondo = idRecordFondo;
                DAGestioneCalcolo.SalvaDatiServizioUtile707RecordFondo(datiServizioUtile707);
                transactionScope.Complete();
            }
        }

        public static void SalvaDatiServizioUtileINPDAP707(long idPensione, long idRecordFondo, ServizioUtileINPDAP707 servizioUtile)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.DatiServizioUtileINPDAP707 datiServizioUtile707 = new DataCommon.DatiServizioUtileINPDAP707();
                Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile707);
                datiServizioUtile707.IdRecordFondo = idRecordFondo;
                DAGestioneCalcolo.SalvaDatiServizioUtileINPDAP707(idPensione, datiServizioUtile707);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtile707ByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtile707ByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtileINPDAP707ByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcolo.EliminaDatiServizioUtileINPDAP707ByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtileINPDAP707ByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcolo.EliminaDatiServizioUtileINPDAP707ByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #endregion Servizio Utile 707

        #region nested class
        public class DatiCalcoloContributivo
        {
            #region private properties

            private long _Id;

            private long _IdPensione;

            private long _IdRecordFondo;

            private System.Nullable<System.DateTime> _DecorrenzaCalcoloContibutivo;

            private System.Nullable<long> _CodiceGestione;

            private System.Nullable<decimal> _ImportoBase;

            private System.Nullable<decimal> _MontanteContributivo;

            private System.Nullable<decimal> _Montante;

            private System.Nullable<decimal> _ImportoContributivoTotale;

            private System.Nullable<int> _NSettimane;

            private System.Nullable<decimal> _ImportoIVS;

            private System.Nullable<decimal> _Contributi;

            private System.Nullable<int> _NSettimaneLegge335;

            private System.Nullable<decimal> _MontanteInvalidita;

            private System.Nullable<decimal> _QuotaFacoltativaMensile;

            private System.Nullable<decimal> _MontanteAnte0697;

            private System.Nullable<short> _AnzianitaAnte0697AA;

            private System.Nullable<short> _AnzianitaAnte0697MM;

            private System.Nullable<short> _AnzianitaAnte0697GG;

            private System.Nullable<short> _AnzianitaPost0697AA;

            private System.Nullable<short> _AnzianitaPost0697MM;

            private System.Nullable<short> _AnzianitaPost0697GG;

            private System.Nullable<decimal> _MontanteQuotaDL214;

            private System.Nullable<decimal> _ImportoContribTotaleQuotaDL214;

            private System.Nullable<int> _NSettimaneQuotaDL214;

            private System.Nullable<decimal> _MontanteEsclusivo;

            private decimal? _MontanteEsclusivoQuotaDL214;

            private decimal? _QuotaContributivaAnnua;

            private decimal? _PL_Quotac;

            private bool _IsStorico;

            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public long IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }

            public DateTime? DecorrenzaCalcoloContibutivo { get { return _DecorrenzaCalcoloContibutivo; } set { _DecorrenzaCalcoloContibutivo = value; } }

            public long? CodiceGestione { get { return _CodiceGestione; } set { _CodiceGestione = value; } }

            public decimal? ImportoBase { get { return _ImportoBase; } set { _ImportoBase = value; } }

            public decimal? MontanteContributivo { get { return _MontanteContributivo; } set { _MontanteContributivo = value; } }

            public decimal? Montante { get { return _Montante; } set { _Montante = value; } }

            public decimal? ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }

            public int? NSettimane { get { return _NSettimane; } set { _NSettimane = value; } }

            public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }

            public decimal? Contributi { get { return _Contributi; } set { _Contributi = value; } }

            public int? NSettimaneLegge335 { get { return _NSettimaneLegge335; } set { _NSettimaneLegge335 = value; } }

            public decimal? MontanteInvalidita { get { return _MontanteInvalidita; } set { _MontanteInvalidita = value; } }

            public decimal? QuotaFacoltativaMensile { get { return _QuotaFacoltativaMensile; } set { _QuotaFacoltativaMensile = value; } }

            public System.Nullable<decimal> MontanteAnte0697 { get { return _MontanteAnte0697; } set { _MontanteAnte0697 = value; } }

            public System.Nullable<short> AnzianitaAnte0697AA { get { return _AnzianitaAnte0697AA; } set { _AnzianitaAnte0697AA = value; } }

            public System.Nullable<short> AnzianitaAnte0697MM { get { return _AnzianitaAnte0697MM; } set { _AnzianitaAnte0697MM = value; } }

            public System.Nullable<short> AnzianitaAnte0697GG { get { return _AnzianitaAnte0697GG; } set { _AnzianitaAnte0697GG = value; } }

            public System.Nullable<short> AnzianitaPost0697AA { get { return _AnzianitaPost0697AA; } set { _AnzianitaPost0697AA = value; } }

            public System.Nullable<short> AnzianitaPost0697MM { get { return _AnzianitaPost0697MM; } set { _AnzianitaPost0697MM = value; } }

            public System.Nullable<short> AnzianitaPost0697GG { get { return _AnzianitaPost0697GG; } set { _AnzianitaPost0697GG = value; } }

            public System.Nullable<decimal> MontanteQuotaDL214 { get { return _MontanteQuotaDL214; } set { _MontanteQuotaDL214 = value; } }

            public System.Nullable<decimal> ImportoContribTotaleQuotaDL214 { get { return _ImportoContribTotaleQuotaDL214; } set { _ImportoContribTotaleQuotaDL214 = value; } }

            public System.Nullable<int> NSettimaneQuotaDL214 { get { return _NSettimaneQuotaDL214; } set { _NSettimaneQuotaDL214 = value; } }

            public System.Nullable<decimal> MontanteEsclusivo { get { return _MontanteEsclusivo; } set { _MontanteEsclusivo = value; } }

            public decimal? MontanteEsclusivoQuotaDL214 { get { return _MontanteEsclusivoQuotaDL214; } set { _MontanteEsclusivoQuotaDL214 = value; } }

            public decimal? QuotaContributivaAnnua { get { return _QuotaContributivaAnnua; } set { _QuotaContributivaAnnua = value; } }

            public decimal? PL_Quotac { get { return _PL_Quotac; } set { _PL_Quotac = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }          

            #endregion public properties

            public bool IsDatiCalcoloContributivoNull()
            {
                if (!this._DecorrenzaCalcoloContibutivo.HasValue && !this._CodiceGestione.HasValue && !this._ImportoBase.HasValue &&
                    !this._MontanteContributivo.HasValue && !this._Montante.HasValue && !this._ImportoContributivoTotale.HasValue &&
                    !this._NSettimane.HasValue && !this._ImportoIVS.HasValue && !this._Contributi.HasValue && !this._NSettimaneLegge335.HasValue &&
                    !this._MontanteInvalidita.HasValue && !this._QuotaFacoltativaMensile.HasValue && !this._MontanteAnte0697.HasValue &&
                    !this._AnzianitaAnte0697AA.HasValue && !this._AnzianitaAnte0697MM.HasValue && !this._AnzianitaAnte0697GG.HasValue &&
                    !this._AnzianitaPost0697AA.HasValue && !this._AnzianitaPost0697MM.HasValue && !this._AnzianitaPost0697GG.HasValue &&
                    !this._MontanteQuotaDL214.HasValue && !this._ImportoContribTotaleQuotaDL214.HasValue && !this._NSettimaneQuotaDL214.HasValue &&
                    !this._MontanteEsclusivo.HasValue && !this._MontanteEsclusivoQuotaDL214.HasValue)
                    return true;
                else
                    return false;
            }

            public bool IsQuotaL335Presente(GestionePensione.DatiPensione datiPensione = null)
            {
                var ctrlSettimane = datiPensione != null && Utility.IsDomandaAUT(datiPensione) ? (this._NSettimane.HasValue && this._NSettimane.Value != 0 ? true : false) : this._NSettimane.HasValue;
                if (this._ImportoContributivoTotale.HasValue || this._Montante.HasValue || ctrlSettimane || this._MontanteAnte0697.HasValue || this._AnzianitaAnte0697AA.HasValue ||
                    this._AnzianitaAnte0697MM.HasValue || this._AnzianitaAnte0697GG.HasValue || this._AnzianitaPost0697AA.HasValue || this._AnzianitaPost0697MM.HasValue ||
                    this._AnzianitaPost0697GG.HasValue || this._MontanteContributivo.HasValue || this._MontanteEsclusivo.HasValue)
                    return true;

                return false;
            }

            public bool IsQuotaDL214Presente()
            {
                if (this._NSettimaneQuotaDL214.HasValue || this._MontanteQuotaDL214.HasValue || this._ImportoContribTotaleQuotaDL214.HasValue || this._MontanteEsclusivoQuotaDL214.HasValue)
                    return true;

                return false;
            }

            public override bool Equals(object obj)
            {
                DatiCalcoloContributivo dCc = obj as DatiCalcoloContributivo;
                if (dCc != null)
                {
                    if (this._DecorrenzaCalcoloContibutivo == dCc.DecorrenzaCalcoloContibutivo &&
                        this._CodiceGestione == dCc.CodiceGestione &&
                        this._ImportoBase == dCc.ImportoBase &&
                        this._MontanteContributivo == dCc.MontanteContributivo &&
                        this._Montante == dCc.Montante &&
                        this._ImportoContributivoTotale == dCc.ImportoContributivoTotale &&
                        this._NSettimane == dCc.NSettimane &&
                        this._ImportoIVS == dCc.ImportoIVS &&
                        this._Contributi == dCc.Contributi &&
                        this._NSettimaneLegge335 == dCc.NSettimaneLegge335 &&
                        this._MontanteInvalidita == dCc.MontanteInvalidita &&
                        this._QuotaFacoltativaMensile == dCc.QuotaFacoltativaMensile &&
                        this._MontanteAnte0697 == dCc.MontanteAnte0697 &&
                        this._AnzianitaAnte0697AA == dCc.AnzianitaAnte0697AA &&
                        this._AnzianitaAnte0697MM == dCc.AnzianitaAnte0697MM &&
                        this._AnzianitaAnte0697GG == dCc.AnzianitaPost0697GG &&
                        this._AnzianitaPost0697AA == dCc.AnzianitaPost0697AA &&
                        this._AnzianitaPost0697MM == dCc.AnzianitaPost0697MM &&
                        this._AnzianitaPost0697GG == dCc.AnzianitaPost0697GG &&
                        this._MontanteQuotaDL214 == dCc.MontanteQuotaDL214 &&
                        this._ImportoContribTotaleQuotaDL214 == dCc.ImportoContribTotaleQuotaDL214 &&
                        this._NSettimaneQuotaDL214 == dCc.NSettimaneQuotaDL214 &&
                        this._MontanteEsclusivo == dCc.MontanteEsclusivo &&
                        this._MontanteEsclusivoQuotaDL214 == dCc.MontanteEsclusivoQuotaDL214 &&
                        this._QuotaContributivaAnnua == dCc.QuotaContributivaAnnua)
                        return true;
                }
                return false;
            }
        }

        public class DatiCalcoloRetributivo
        {
            #region private properties
            private long _Id;
            private long _IdPensione;
            private long _IdRecordFondo;
            private System.Nullable<byte> _CodiceLiquidazione;
            private System.Nullable<System.DateTime> _DecorrenzaOriginariaPensione;
            private System.Nullable<int> _NSettimaneAnzianita;
            private System.Nullable<int> _NSettimaneQuotaA;
            private System.Nullable<int> _NSettimaneQuotaA2;
            private System.Nullable<int> _NSettimaneQuotaB;
            private System.Nullable<decimal> _RMS;
            private System.Nullable<decimal> _RMSQuotaA;
            private System.Nullable<decimal> _RMSQuotaB;
            private System.Nullable<int> _NSettimaneExCombattente;
            private System.Nullable<int> _NSettimaneQuotaC;
            private System.Nullable<int> _NSettimaneQuotaC2;
            private System.Nullable<decimal> _RMSExCombattente;
            private System.Nullable<decimal> _MontanteContributivoAGO;
            private System.Nullable<decimal> _RetribuzionePonderataAnnua;
            private System.Nullable<decimal> _RMSQuotaD;
            private System.Nullable<decimal> _RetribuzioneAGO;
            private System.Nullable<int> _NSettAnzianitaVL;
            private System.Nullable<int> _NSettAnzianitaVV;
            private System.Nullable<byte> _MeseDiRiferimentoQuotaBDZ;
            private System.Nullable<int> _NSettimaneQuotaD;
            private System.Nullable<long> _CodiceGestione;
            private System.Nullable<char> _QuotePrimeLiquidate;
            private System.Nullable<int> _NSettimaneEsclusiveQuotaA;
            private System.Nullable<int> _NSettimaneEsclusiveQuotaB;
            private string _CodiceTipoQuota;
            private int? _NSettimane707;
            private decimal? _PL_Quotar;
            private decimal? _PL_Quotar707;
            private bool _IsStorico;

            #endregion private properties

            #region public properties
            public long Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public long IdPensione
            {
                get { return _IdPensione; }
                set { _IdPensione = value; }
            }

            public long IdRecordFondo
            {
                get { return _IdRecordFondo; }
                set { _IdRecordFondo = value; }
            }

            public System.Nullable<byte> CodiceLiquidazione
            {
                get { return _CodiceLiquidazione; }
                set { _CodiceLiquidazione = value; }
            }

            public System.Nullable<System.DateTime> DecorrenzaOriginariaPensione
            {
                get { return _DecorrenzaOriginariaPensione; }
                set { _DecorrenzaOriginariaPensione = value; }
            }

            public System.Nullable<int> NSettimaneAnzianita
            {
                get { return _NSettimaneAnzianita; }
                set { _NSettimaneAnzianita = value; }
            }

            public System.Nullable<int> NSettimaneQuotaA
            {
                get { return _NSettimaneQuotaA; }
                set { _NSettimaneQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneQuotaA2
            {
                get { return _NSettimaneQuotaA2; }
                set { _NSettimaneQuotaA2 = value; }
            }

            public System.Nullable<int> NSettimaneQuotaB
            {
                get { return _NSettimaneQuotaB; }
                set { _NSettimaneQuotaB = value; }
            }

            public System.Nullable<decimal> RMS
            {
                get { return _RMS; }
                set { _RMS = value; }
            }

            public System.Nullable<decimal> RMSQuotaA
            {
                get { return _RMSQuotaA; }
                set { _RMSQuotaA = value; }
            }

            public System.Nullable<decimal> RMSQuotaB
            {
                get { return _RMSQuotaB; }
                set { _RMSQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneExCombattente
            {
                get { return _NSettimaneExCombattente; }
                set { _NSettimaneExCombattente = value; }
            }

            public System.Nullable<int> NSettimaneQuotaC
            {
                get { return _NSettimaneQuotaC; }
                set { _NSettimaneQuotaC = value; }
            }

            public System.Nullable<int> NSettimaneQuotaC2
            {
                get { return _NSettimaneQuotaC2; }
                set { _NSettimaneQuotaC2 = value; }
            }

            public System.Nullable<decimal> RMSExCombattente
            {
                get { return _RMSExCombattente; }
                set { _RMSExCombattente = value; }
            }

            public System.Nullable<decimal> MontanteContributivoAGO
            {
                get { return _MontanteContributivoAGO; }
                set { _MontanteContributivoAGO = value; }
            }

            public System.Nullable<decimal> RetribuzionePonderataAnnua
            {
                get { return _RetribuzionePonderataAnnua; }
                set { _RetribuzionePonderataAnnua = value; }
            }

            public System.Nullable<decimal> RMSQuotaD
            {
                get { return _RMSQuotaD; }
                set { _RMSQuotaD = value; }
            }

            public System.Nullable<decimal> RetribuzioneAGO
            {
                get { return _RetribuzioneAGO; }
                set { _RetribuzioneAGO = value; }
            }

            public System.Nullable<int> NSettAnzianitaVL
            {
                get { return _NSettAnzianitaVL; }
                set { _NSettAnzianitaVL = value; }
            }

            public System.Nullable<int> NSettAnzianitaVV
            {
                get { return _NSettAnzianitaVV; }
                set { _NSettAnzianitaVV = value; }
            }

            public System.Nullable<byte> MeseDiRiferimentoQuotaBDZ
            {
                get { return _MeseDiRiferimentoQuotaBDZ; }
                set { _MeseDiRiferimentoQuotaBDZ = value; }
            }

            public System.Nullable<int> NSettimaneQuotaD
            {
                get { return _NSettimaneQuotaD; }
                set { _NSettimaneQuotaD = value; }
            }

            public System.Nullable<long> CodiceGestione
            {
                get { return _CodiceGestione; }
                set { _CodiceGestione = value; }
            }

            public System.Nullable<char> QuotePrimeLiquidate
            {
                get { return _QuotePrimeLiquidate; }
                set { _QuotePrimeLiquidate = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaA
            {
                get { return _NSettimaneEsclusiveQuotaA; }
                set { _NSettimaneEsclusiveQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaB
            {
                get { return _NSettimaneEsclusiveQuotaB; }
                set { _NSettimaneEsclusiveQuotaB = value; }
            }

            public string CodiceTipoQuota
            {
                get { return _CodiceTipoQuota; }
                set { _CodiceTipoQuota = value; }
            }

            public int? NSettimane707 { get { return _NSettimane707; } set { _NSettimane707 = value; } }
            public decimal? PL_Quotar { get { return _PL_Quotar; } set { _PL_Quotar = value; } }
            public decimal? PL_Quotar707 { get { return _PL_Quotar707; } set { _PL_Quotar707 = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }
            #endregion public properties

            public bool IsDatiCalcoloRetributivoNull()
            {
                if (!this._CodiceLiquidazione.HasValue && !this._DecorrenzaOriginariaPensione.HasValue && !this._NSettimaneAnzianita.HasValue &&
                    !this._NSettimaneQuotaA.HasValue && !this._NSettimaneQuotaB.HasValue && !this._RMS.HasValue && !this._RMSQuotaA.HasValue &&
                    !this._RMSQuotaB.HasValue && !this._NSettimaneExCombattente.HasValue && !this._NSettimaneQuotaC.HasValue && !this._RMSExCombattente.HasValue &&
                    !this._MontanteContributivoAGO.HasValue && !this._RetribuzionePonderataAnnua.HasValue && !this._RMSQuotaD.HasValue && !this._RetribuzioneAGO.HasValue &&
                    !this._NSettAnzianitaVL.HasValue && !this._NSettAnzianitaVV.HasValue && !this._MeseDiRiferimentoQuotaBDZ.HasValue && !this._NSettimaneQuotaD.HasValue &&
                    !this._CodiceGestione.HasValue && !this._CodiceGestione.HasValue && !this._NSettimaneEsclusiveQuotaA.HasValue && !this._NSettimaneEsclusiveQuotaB.HasValue &&
                     string.IsNullOrEmpty(this._CodiceTipoQuota) && !_NSettimane707.HasValue)
                    return true;
                else
                    return false;
            }

            public override bool Equals(object obj)
            {
                DatiCalcoloRetributivo dCr = obj as DatiCalcoloRetributivo;
                if (dCr != null)
                {
                    if (this._CodiceLiquidazione == dCr.CodiceLiquidazione &&
                        this._DecorrenzaOriginariaPensione == dCr.DecorrenzaOriginariaPensione &&
                        this._NSettimaneAnzianita == dCr.NSettimaneAnzianita &&
                        this._NSettimaneQuotaA == dCr.NSettimaneQuotaA &&
                        this._NSettimaneQuotaA2 == dCr.NSettimaneQuotaA2 &&
                        this._NSettimaneQuotaB == dCr.NSettimaneQuotaB &&
                        this._RMS == dCr.RMS &&
                        this._RMSQuotaA == dCr.RMSQuotaA &&
                        this._RMSQuotaB == dCr.RMSQuotaB &&
                        this._NSettimaneExCombattente == dCr.NSettimaneExCombattente &&
                        this._NSettimaneQuotaC == dCr.NSettimaneQuotaC &&
                        this._NSettimaneQuotaC2 == dCr.NSettimaneQuotaC2 &&
                        this._RMSExCombattente == dCr.RMSExCombattente &&
                        this._MontanteContributivoAGO == dCr.MontanteContributivoAGO &&
                        this._RetribuzionePonderataAnnua == dCr.RetribuzionePonderataAnnua &&
                        this._RMSQuotaD == dCr.RMSQuotaD &&
                        this._RetribuzioneAGO == dCr.RetribuzioneAGO &&
                        this._NSettAnzianitaVL == dCr.NSettAnzianitaVL &&
                        this._NSettAnzianitaVV == dCr.NSettAnzianitaVV &&
                        this._MeseDiRiferimentoQuotaBDZ == dCr.MeseDiRiferimentoQuotaBDZ &&
                        this._NSettimaneQuotaD == dCr.NSettimaneQuotaD &&
                        this._CodiceGestione == dCr.CodiceGestione &&
                        this._QuotePrimeLiquidate == dCr.QuotePrimeLiquidate &&
                        this._NSettimaneEsclusiveQuotaA == dCr.NSettimaneEsclusiveQuotaA &&
                        this._NSettimaneEsclusiveQuotaB == dCr.NSettimaneEsclusiveQuotaB &&
                        this._CodiceTipoQuota == dCr.CodiceTipoQuota &&
                        this._NSettimane707 == dCr.NSettimane707)
                        return true;
                }
                return false;
            }
        }

        public class DatiCalcoloContributivoEstero
        {
            #region private properties

            private long _Id;
            private long _IdPensione;
            private long? _CodiceGestione;
            private DateTime? _Decorrenza;
            private int? _Settimane;
            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public long? CodiceGestione { get { return _CodiceGestione; } set { _CodiceGestione = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public int? Settimane { get { return _Settimane; } set { _Settimane = value; } }
            #endregion public properties
        }

        public class DatiCalcoloContributivoENPAL
        {
            #region Private properties

            private long _Id;
            private long _IdPensione;
            private System.Nullable<decimal> _CoefficienteTrasformazione;
            private System.Nullable<decimal> _ImportoContributivoTotale;
            private System.Nullable<decimal> _Montante;
            private char? _Quota;
            private string _Decorrenza;
            private System.Nullable<int> _NumeroContributiTotale;
            private bool _IsStorico;

            #endregion Private properties

            #region Public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public decimal? CoefficienteTrasformazione { get { return _CoefficienteTrasformazione; } set { _CoefficienteTrasformazione = value; } }
            public decimal? ImportoContributivoTotale { get { return _ImportoContributivoTotale; } set { _ImportoContributivoTotale = value; } }
            public decimal? Montante { get { return _Montante; } set { _Montante = value; } }
            public char? Quota { get { return _Quota; } set { _Quota = value; } }
            public string Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public int? NumeroContributiTotale { get { return _NumeroContributiTotale; } set { _NumeroContributiTotale = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

            #endregion Public properties

            public bool IsDatiCalcoloContributivoEnpalsNull()
            {
                if ((!this._CoefficienteTrasformazione.HasValue || this._CoefficienteTrasformazione.Value == 0) &&
                    (!this._ImportoContributivoTotale.HasValue || this._ImportoContributivoTotale.Value == 0) &&
                    (!this._Montante.HasValue || this._Montante.Value == 0) &&
                    (!this._Quota.HasValue || this._Quota == '\0') &&
                    (string.IsNullOrEmpty(this._Decorrenza)) &&
                    (!this._NumeroContributiTotale.HasValue || this._NumeroContributiTotale.Value == 0))
                    return true;
                else
                    return false;
            }

            public override bool Equals(object obj)
            {
                DatiCalcoloContributivoENPAL dC = obj as DatiCalcoloContributivoENPAL;
                if (dC != null)
                {
                    if (this.CoefficienteTrasformazione == dC.CoefficienteTrasformazione &&
                        this.Decorrenza == dC.Decorrenza &&
                        this.ImportoContributivoTotale == dC.ImportoContributivoTotale &&
                        this.Montante == dC.Montante &&
                        this.NumeroContributiTotale == dC.NumeroContributiTotale &&
                        this.Quota == dC.Quota)
                        return true;
                }
                return false;
            }
        }

        public class DatiCalcoloRetributivoENPAL
        {
            #region Private properties

            private long _Id;
            private long _IdPensione;
            private System.Nullable<short> _PeriodiQuotaA;
            private System.Nullable<short> _PeriodiQuotaB;
            private System.Nullable<short> _NTotaleContributiCalcoloQuotaA;
            private System.Nullable<short> _NTotaleContributiCalcoloQuotaB;
            private System.Nullable<decimal> _RMQuotaA;
            private System.Nullable<decimal> _RMQuotaB;
            private System.Nullable<decimal> _ImportoQuotaA;
            private System.Nullable<decimal> _ImportoQuotaB;
            private System.Nullable<decimal> _ImportoProRataTemporis;
            private System.Nullable<decimal> _ImportoQuotaRetributivaInMisto;
            private short? _GiorniQuotaA707;
            private decimal? _ImportoQuotaA707;
            private short? _GiorniQuotaB707;
            private decimal? _ImportoQuotaB707;
            private string _DecorrenzaQuotaA;
            private string _DecorrenzaQuotaB;
            private bool _IsStorico;

            #endregion Private properties

            #region Public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public short? PeriodiQuotaA { get { return _PeriodiQuotaA; } set { _PeriodiQuotaA = value; } }
            public short? PeriodiQuotaB { get { return _PeriodiQuotaB; } set { _PeriodiQuotaB = value; } }
            public short? NTotaleContributiCalcoloQuotaA { get { return _NTotaleContributiCalcoloQuotaA; } set { _NTotaleContributiCalcoloQuotaA = value; } }
            public short? NTotaleContributiCalcoloQuotaB { get { return _NTotaleContributiCalcoloQuotaB; } set { _NTotaleContributiCalcoloQuotaB = value; } }
            public decimal? RMQuotaA { get { return _RMQuotaA; } set { _RMQuotaA = value; } }
            public decimal? RMQuotaB { get { return _RMQuotaB; } set { _RMQuotaB = value; } }
            public decimal? ImportoQuotaA { get { return _ImportoQuotaA; } set { _ImportoQuotaA = value; } }
            public decimal? ImportoQuotaB { get { return _ImportoQuotaB; } set { _ImportoQuotaB = value; } }
            public decimal? ImportoProRataTemporis { get { return _ImportoProRataTemporis; } set { _ImportoProRataTemporis = value; } }
            public decimal? ImportoQuotaRetributivaInMisto { get { return _ImportoQuotaRetributivaInMisto; } set { _ImportoQuotaRetributivaInMisto = value; } }
            public short? GiorniQuotaA707 { get { return _GiorniQuotaA707; } set { _GiorniQuotaA707 = value; } }
            public decimal? ImportoQuotaA707 { get { return _ImportoQuotaA707; } set { _ImportoQuotaA707 = value; } }
            public short? GiorniQuotaB707 { get { return _GiorniQuotaB707; } set { _GiorniQuotaB707 = value; } }
            public decimal? ImportoQuotaB707 { get { return _ImportoQuotaB707; } set { _ImportoQuotaB707 = value; } }
            public string DecorrenzaQuotaA { get { return _DecorrenzaQuotaA; } set { _DecorrenzaQuotaA = value; } }
            public string DecorrenzaQuotaB { get { return _DecorrenzaQuotaB; } set { _DecorrenzaQuotaB = value; } }
            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

            #endregion Public properties

            public bool IsDatiCalcoloRetributivoEnpalsNull()
            {
                if ((!this._PeriodiQuotaA.HasValue || this._PeriodiQuotaA.Value == 0) && (!this._PeriodiQuotaB.HasValue || this._PeriodiQuotaB.Value == 0) &&
                    (!this._NTotaleContributiCalcoloQuotaA.HasValue || this._NTotaleContributiCalcoloQuotaA.Value == 0) &&
                    (!this._NTotaleContributiCalcoloQuotaB.HasValue || this._NTotaleContributiCalcoloQuotaB.Value == 0) &&
                    (!this._RMQuotaA.HasValue || this._RMQuotaA.Value == 0) && (!this._RMQuotaB.HasValue || this._RMQuotaB.Value == 0) &&
                    (!this._ImportoQuotaA.HasValue || this._ImportoQuotaA.Value == 0) && (!this._ImportoQuotaB.HasValue || this._ImportoQuotaB.Value == 0) &&
                    (!this._ImportoProRataTemporis.HasValue || this._ImportoProRataTemporis.Value == 0) &&
                    (!this._ImportoQuotaRetributivaInMisto.HasValue || this._ImportoQuotaRetributivaInMisto.Value == 0) &&
                    (!this._GiorniQuotaA707.HasValue || this._GiorniQuotaA707.Value == 0) &&
                    (!this._ImportoQuotaA707.HasValue || this._ImportoQuotaA707.Value == 0) &&
                    (!this._GiorniQuotaB707.HasValue || this._GiorniQuotaB707.Value == 0) &&
                    (!this._ImportoQuotaB707.HasValue || this._ImportoQuotaB707.Value == 0) &&
                    (string.IsNullOrEmpty(this._DecorrenzaQuotaA)) &&
                    (string.IsNullOrEmpty(this._DecorrenzaQuotaB)))
                    return true;
                else
                    return false;
            }

            public override bool Equals(object obj)
            {
                DatiCalcoloRetributivoENPAL dC = obj as DatiCalcoloRetributivoENPAL;
                if (dC != null)
                {
                    if (this.DecorrenzaQuotaA == dC.DecorrenzaQuotaA &&
                        this.DecorrenzaQuotaB == dC.DecorrenzaQuotaB &&
                        this.GiorniQuotaA707 == dC.GiorniQuotaA707 &&
                        this.GiorniQuotaB707 == dC.GiorniQuotaB707 &&
                        this.ImportoProRataTemporis == dC.ImportoProRataTemporis &&
                        this.ImportoQuotaA == dC.ImportoQuotaA &&
                        this.ImportoQuotaA707 == dC.ImportoQuotaA707 &&
                        this.ImportoQuotaB == dC.ImportoQuotaB &&
                        this.ImportoQuotaB707 == dC.ImportoQuotaB707 &&
                        this.ImportoQuotaRetributivaInMisto == dC.ImportoQuotaRetributivaInMisto &&
                        this.NTotaleContributiCalcoloQuotaA == dC.NTotaleContributiCalcoloQuotaA &&
                        this.NTotaleContributiCalcoloQuotaB == dC.NTotaleContributiCalcoloQuotaB &&
                        this.PeriodiQuotaA == dC.PeriodiQuotaA &&
                        this.PeriodiQuotaB == dC.PeriodiQuotaB &&
                        this.RMQuotaA == dC.RMQuotaA &&
                        this.RMQuotaB == dC.RMQuotaB)
                        return true;
                }
                return false;
            }
            }

        public class QuotePensione
        {
            #region public properties
            public long Id { get; set; }
            public long IdPensione { get; set; }
            public long EnteGestioneFondo { get; set; }
            public int? Settimane { get; set; }
            public decimal? Importo { get; set; }
            public DateTime? Decorrenza { get; set; }
            public bool IsQuotaProgressiva { get; set; }
            public bool IsStorico { get; set; }
            #endregion public properties
        }

        public class TrattenuteQuotePensione
        {
            #region public properties
            public long Id { get; set; }
            public long IdPensione { get; set; }
            public long EnteGestioneFondoQuote { get; set; }
            public decimal? ImportoEnteGestioneFondoQuote { get; set; }
            public short AnnoCompetenza { get; set; }
            public string CodiceTrattenute { get; set; }
            public decimal ImportoTrattenute { get; set; }
            public bool IsStorico { get; set; }
            #endregion public properties
        }

        public class ServizioUtile707
        {
            #region private poperties
            private long _Id;

            private System.Nullable<long> _IdFondo;

            private System.Nullable<long> _IdRecordFondo;

            private string _Quota;

            private System.Nullable<short> _ServizioUtileAA;

            private System.Nullable<short> _ServizioUtileMM;

            private System.Nullable<short> _ServizioUtileGG;

            private System.Nullable<short> _ServizioUtileCessazioneAA;

            private System.Nullable<short> _ServizioUtileCessazioneMM;

            private System.Nullable<short> _ServizioUtileCessazioneGG;

            private System.Nullable<decimal> _QuotaPensioneRetributivaAnnua;

            private bool _IsStorico;

            #endregion private poperties

            #region public poperties

            public long Id { get { return _Id; } set { _Id = value; } }

            public System.Nullable<long> IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }

            public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }

            public string Quota { get { return _Quota; } set { _Quota = value; } }

            public System.Nullable<short> ServizioUtileAA { get { return _ServizioUtileAA; } set { _ServizioUtileAA = value; } }

            public System.Nullable<short> ServizioUtileMM { get { return _ServizioUtileMM; } set { _ServizioUtileMM = value; } }

            public System.Nullable<short> ServizioUtileGG { get { return _ServizioUtileGG; } set { _ServizioUtileGG = value; } }

            public System.Nullable<short> ServizioUtileCessazioneAA { get { return _ServizioUtileCessazioneAA; } set { _ServizioUtileCessazioneAA = value; } }

            public System.Nullable<short> ServizioUtileCessazioneMM { get { return _ServizioUtileCessazioneMM; } set { _ServizioUtileCessazioneMM = value; } }

            public System.Nullable<short> ServizioUtileCessazioneGG { get { return _ServizioUtileCessazioneGG; } set { _ServizioUtileCessazioneGG = value; } }

            public System.Nullable<decimal> QuotaPensioneRetributivaAnnua { get { return _QuotaPensioneRetributivaAnnua; } set { _QuotaPensioneRetributivaAnnua = value; } }

            public bool IsStorico { get { return _IsStorico; } set { _IsStorico = value; } }

            #endregion public poperties

            public bool IsNull()
            {
                if (string.IsNullOrEmpty(this._Quota) &&
                    !_ServizioUtileAA.HasValue &&
                    !_ServizioUtileMM.HasValue &&
                    !_ServizioUtileGG.HasValue &&
                    !_ServizioUtileCessazioneAA.HasValue &&
                    !_ServizioUtileCessazioneMM.HasValue &&
                    !_ServizioUtileCessazioneGG.HasValue &&
                    !_QuotaPensioneRetributivaAnnua.HasValue)
                    return true;

                return false;
            }
        }

        public class ServizioUtileINPDAP707
        {
            #region private poperties
            private long _Id;

            private System.Nullable<long> _IdPensione;

            private System.Nullable<long> _IdRecordFondo;

            private string _Quota;

            private System.Nullable<short> _ServizioUtileAA;

            private System.Nullable<short> _ServizioUtileMM;

            private System.Nullable<short> _ServizioUtileGG;

            private System.Nullable<short> _ServizioUtileCessazioneAA;

            private System.Nullable<short> _ServizioUtileCessazioneMM;

            private System.Nullable<short> _ServizioUtileCessazioneGG;

            private System.Nullable<decimal> _QuotaPensioneRetributivaAnnua;


            #endregion private poperties

            #region public poperties

            public long Id { get { return _Id; } set { _Id = value; } }

            public System.Nullable<long> IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }

            public string Quota { get { return _Quota; } set { _Quota = value; } }

            public System.Nullable<short> ServizioUtileAA { get { return _ServizioUtileAA; } set { _ServizioUtileAA = value; } }

            public System.Nullable<short> ServizioUtileMM { get { return _ServizioUtileMM; } set { _ServizioUtileMM = value; } }

            public System.Nullable<short> ServizioUtileGG { get { return _ServizioUtileGG; } set { _ServizioUtileGG = value; } }

            public System.Nullable<short> ServizioUtileCessazioneAA { get { return _ServizioUtileCessazioneAA; } set { _ServizioUtileCessazioneAA = value; } }

            public System.Nullable<short> ServizioUtileCessazioneMM { get { return _ServizioUtileCessazioneMM; } set { _ServizioUtileCessazioneMM = value; } }

            public System.Nullable<short> ServizioUtileCessazioneGG { get { return _ServizioUtileCessazioneGG; } set { _ServizioUtileCessazioneGG = value; } }

            public System.Nullable<decimal> QuotaPensioneRetributivaAnnua { get { return _QuotaPensioneRetributivaAnnua; } set { _QuotaPensioneRetributivaAnnua = value; } }


            #endregion public poperties

            public bool IsNull()
            {
                if (string.IsNullOrEmpty(this._Quota) &&
                    !_ServizioUtileAA.HasValue &&
                    !_ServizioUtileMM.HasValue &&
                    !_ServizioUtileGG.HasValue &&
                    !_ServizioUtileCessazioneAA.HasValue &&
                    !_ServizioUtileCessazioneMM.HasValue &&
                    !_ServizioUtileCessazioneGG.HasValue &&
                    !_QuotaPensioneRetributivaAnnua.HasValue)
                    return true;

                return false;
            }
        }

        #endregion nested class
    }
}
