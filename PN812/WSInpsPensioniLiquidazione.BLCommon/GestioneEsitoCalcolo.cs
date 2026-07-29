using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneEsitoCalcolo
    {
        public static void GetEsitoCalcoloByIdPensione(Int64 idPensione, out DatiEsitoCalcolo datiEsitoCalcolo)
        {
            EsitoCalcolo esitoCalcolo = null;
            datiEsitoCalcolo = null;
            DAGestioneEsitoCalcolo.GetEsitoCalcoloByIdPensione(idPensione, out esitoCalcolo);
            if (esitoCalcolo == null)
                return;
            datiEsitoCalcolo = new DatiEsitoCalcolo();
            Utility.ValorizzaOggetti(esitoCalcolo, datiEsitoCalcolo);
        }

        public static void SalvaEsitoCalcolo(long idPensione, DatiEsitoCalcolo datiEsitoCalcolo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                EsitoCalcolo esitoCalcolo = new EsitoCalcolo();
                Utility.ValorizzaOggetti(datiEsitoCalcolo, esitoCalcolo);
                esitoCalcolo.IdPensione = idPensione;
                DAGestioneEsitoCalcolo.SalvaEsitoCalcolo(esitoCalcolo);
                transactionScope.Complete();
            }
        }

        public static void EliminaEsitoCalcoloByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneEsitoCalcolo.EliminaEsitoCalcoloByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiEsitoCalcolo
        {
            public DatiEsitoCalcolo()
            { }

            public DatiEsitoCalcolo(string esito, string dettaglioEsito)
            {
                this._Esito = esito;
                this._DettaglioEsito = dettaglioEsito;
            }

            #region private properties
            private string _Esito;

            private string _DettaglioEsito;
            #endregion private properties

            #region public properties
            public string Esito { get { return _Esito; } set { _Esito = value; } }

            public string DettaglioEsito { get { return _DettaglioEsito; } set { _DettaglioEsito = value; } }
            #endregion public properties
        }
        #endregion nestled Class
    }
}




