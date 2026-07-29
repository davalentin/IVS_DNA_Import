using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAvvisi
    {
        /// <summary>
        /// Restituisce l'elenco degli avvisi attivi
        /// </summary>
        /// <param name="elencoAvvisiAttivi">Elenco avvisi attivi</param>
        public static void GetAvvisiAttivi(string tipoApp, out List<DatiAvvisi> elencoAvvisiAttivi)
        {
            elencoAvvisiAttivi = null;
            List<Avvisi> elencoAvvisi = null;
            DAGestioneAvvisi.GetAvvisiAttivi(tipoApp, out elencoAvvisi);
            if (elencoAvvisi != null && elencoAvvisi.Count > 0)
            {
                elencoAvvisiAttivi = new List<DatiAvvisi>();
                foreach (Avvisi aDB in elencoAvvisi)
                {
                    DatiAvvisi aBL = new DatiAvvisi();
                    Utility.ValorizzaOggetti(aDB, aBL);
                    elencoAvvisiAttivi.Add(aBL);
                }
            }
        }

        /// <summary>
        /// Restituisce l'elenco degli avvisi presenti a db
        /// </summary>
        /// <param name="elencoAvvisi">Elenco degli avvisi</param>
        public static void GetAllAvvisi(string tipoApp, out List<DatiAvvisi> elencoAvvisi)
        {
            elencoAvvisi = null;
            List<Avvisi> elencoAvvisiDB = null;
            DAGestioneAvvisi.GetAllAvvisi(tipoApp, out elencoAvvisiDB);
            if (elencoAvvisiDB != null && elencoAvvisiDB.Count > 0)
            {
                elencoAvvisi = new List<DatiAvvisi>();
                foreach (Avvisi aDB in elencoAvvisiDB)
                {
                    DatiAvvisi aBL = new DatiAvvisi();
                    Utility.ValorizzaOggetti(aDB, aBL);
                    elencoAvvisi.Add(aBL);
                }
            }
        }

        /// <summary>
        /// Permette il salvataggio di un avviso
        /// </summary>
        /// <param name="datiAvviso">Dati dell'avviso</param>
        public static void SalvaAvviso(DatiAvvisi datiAvviso)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    Avvisi avviso = new Avvisi();
                    Utility.ValorizzaOggetti(datiAvviso, avviso);
                    DAGestioneAvvisi.SalvaAvviso(avviso);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Permette l'eliminazione di un avviso tramite il suo id
        /// </summary>
        /// <param name="idAvviso">Id dell'avviso da eliminare</param>
        public static void DeleteAvviso(long idAvviso)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    DAGestioneAvvisi.DeleteAvviso(idAvviso);
                }
                catch (INPS.DNA.DnaApplicationException ex)
                {
                    throw new INPS.DNA.DnaApplicationException(ex.Message);
                }
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiAvvisi
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
