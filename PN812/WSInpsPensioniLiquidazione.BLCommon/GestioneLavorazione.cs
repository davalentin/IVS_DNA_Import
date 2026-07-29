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
    public class GestioneLavorazione
    {
        public static void GetLavorazioneByIdPensione(Int64 idPensione, out DatiLavorazione datiLavorazione)
        {
            Lavorazione lavorazione = null;
            datiLavorazione = null;
            DAGestioneLavorazione.GetLavorazioneByIdPensione(idPensione, out lavorazione);
            if (lavorazione == null)
                return;
            datiLavorazione = new DatiLavorazione();
            Utility.ValorizzaOggetti(lavorazione, datiLavorazione);
        }

        public static void SalvaLavorazione(long idPensione, DatiLavorazione datiLavorazione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Lavorazione lavorazione = new Lavorazione();
                Utility.ValorizzaOggetti(datiLavorazione, lavorazione);
                lavorazione.IdPensione = idPensione;
                DAGestioneLavorazione.SalvaLavorazione(lavorazione);
                transactionScope.Complete();
            }
        }

        public static void EliminaLavorazioneByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneLavorazione.EliminaLavorazioneByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiLavorazione
        {
            public DatiLavorazione()
            { }
            public DatiLavorazione(System.Nullable<char> tipoReversibilita, string tipoLiquidazione, System.Nullable<char> tipoDomanda, string codFase)
            {
                this._TipoReversibilita = tipoReversibilita;
                this._TipoLiquidazione = tipoLiquidazione;
                this._TipoDomanda = tipoDomanda;
                this._CodFase = codFase;
            }

            #region private properties
            private System.Nullable<char> _TipoReversibilita;

            private string _TipoLiquidazione;

            private System.Nullable<char> _TipoDomanda;

            private string _CodFase;

            #endregion private properties

            #region public properties
            public System.Nullable<char> TipoReversibilita { get { return _TipoReversibilita; } set { _TipoReversibilita = value; } }

            public string TipoLiquidazione { get { return _TipoLiquidazione; } set { _TipoLiquidazione = value; } }

            public System.Nullable<char> TipoDomanda { get { return _TipoDomanda; } set { _TipoDomanda = value; } }

            public string CodFase { get { return _CodFase; } set { _CodFase = value; } }

            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiLavorazione lavorazione = (DatiLavorazione)obj;
                try
                {
                    if (this._TipoReversibilita != lavorazione._TipoReversibilita ||
                        (this._TipoLiquidazione != null ? this._TipoLiquidazione.Trim() : null) != (lavorazione._TipoLiquidazione != null ? lavorazione._TipoLiquidazione.Trim() : null) ||
                        this._TipoDomanda != lavorazione._TipoDomanda)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            #endregion public members
        }
        #endregion nested class
    }
}


