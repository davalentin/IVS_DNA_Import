using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAggiornamenti
    {
        /// <summary>
        /// Restituisce l'elenco degli aggiornamenti aggiornamenti
        /// </summary>
        /// <param name="elencoAggiornamentiAttivi">Elenco aggironamenti attivi</param>
        public static void GetAggiornamentiAttivi(string tipoApp, out List<DatiAggiornamenti> elencoAggiornamentiAttivi)
        {
            elencoAggiornamentiAttivi = null;
            List<DataCommon.Aggiornamenti> elencoAggiornamenti = null;
            DAGestioneAggiornamenti.GetAggiornamentiAttivi(tipoApp, out elencoAggiornamenti);
            if (elencoAggiornamenti != null && elencoAggiornamenti.Count > 0)
            {
                elencoAggiornamentiAttivi = elencoAggiornamenti.Select(x => { var ret = new DatiAggiornamenti(); Utility.ValorizzaOggetti(x, ret); return ret; }).ToList();
            }
        }

        /// <summary>
        /// Restituisce l'elenco degli aggiornamenti presenti a db
        /// </summary>
        /// <param name="elencoaggiornamenti">Elenco degli aggiornamenti</param>
        public static void GetAllAggiornamenti(string tipoApp, out List<DatiAggiornamenti> elencoAggiornamenti)
        {
            elencoAggiornamenti = null;
            List<DataCommon.Aggiornamenti> elencoAggiornamentiDB = null;
            DAGestioneAggiornamenti.GetAllAggiornamenti(tipoApp, out elencoAggiornamentiDB);
            if (elencoAggiornamentiDB != null && elencoAggiornamentiDB.Count > 0)
            {
                elencoAggiornamenti = elencoAggiornamentiDB.Select(x => { var ret = new DatiAggiornamenti(); Utility.ValorizzaOggetti(x, ret); return ret; }).ToList();
            }
        }

        /// <summary>
        /// Permette il salvataggio di un aggiornamento
        /// </summary>
        /// <param name="datiAggiornamento">Dati dell'aggiornamento</param>
        public static void SalvaAggiornamento(DatiAggiornamenti datiAggiornamento)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    Aggiornamenti aggiornamento = new Aggiornamenti();
                    Utility.ValorizzaOggetti(datiAggiornamento, aggiornamento);
                    DAGestioneAggiornamenti.SalvaAggiornamento(aggiornamento);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Permette l'eliminazione di un aggiornamento tramite il suo id
        /// </summary>
        /// <param name="idAgg">Id dell'aggiornamento da eliminare</param>
        public static void DeleteAggiornamento(long idAgg)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    DAGestioneAggiornamenti.DeleteAggiornamenti(idAgg);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiAggiornamenti
        {
            #region private properties
            private long _Id;
            private string _Titolo;
            private string _Testo;
            private bool _Attivo;
            private string _Tipologia;
            private System.DateTime _TimeStamp;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public string Titolo { get { return _Titolo; } set { _Titolo = value; } }

            public string Testo { get { return _Testo; } set { _Testo = value; } }

            public bool Attivo { get { return _Attivo; } set { _Attivo = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            public System.DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
