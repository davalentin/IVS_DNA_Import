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
    public class GestioneDatiControlloFelpe
    {
        public static void GetDatiControlloFelpeByIdPensione(Int64 idPensione, out ControlloFelpe controlloFelpe)
        {
            DatiControlloFelpe datiControlloFelpe = null;
            controlloFelpe = null;
            DAGestioneDatiControlloFelpe.GetDatiControlloFelpeByIdPensione(idPensione, out datiControlloFelpe);
            if (datiControlloFelpe == null)
                return;
            controlloFelpe = new ControlloFelpe();
            Utility.ValorizzaOggetti(datiControlloFelpe, controlloFelpe);
        }

        public static void SalvaDatiControlloFelpe(long idPensione, ControlloFelpe controlloFelpe)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiControlloFelpe datiControlloFelpe = new DatiControlloFelpe();
                Utility.ValorizzaOggetti(controlloFelpe, datiControlloFelpe);
                datiControlloFelpe.IdPensione = idPensione;
                DAGestioneDatiControlloFelpe.SalvaDatiControlloFelpe(datiControlloFelpe);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiControlloFelpeByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiControlloFelpe.EliminaDatiControlloFelpeByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class ControlloFelpe
        {
            public ControlloFelpe()
            { }
            public ControlloFelpe(System.Nullable<bool> isProvvisoria, System.Nullable<System.DateTime> inizioBonus, System.Nullable<System.DateTime> fineBonus)
            {
                this._IsProvvisoria = isProvvisoria;
                this._InizioBonus = inizioBonus;
                this._FineBonus = fineBonus;
            }

            #region private properties
            private System.Nullable<bool> _IsProvvisoria;
            private System.Nullable<System.DateTime> _InizioBonus;
            private System.Nullable<System.DateTime> _FineBonus;
            #endregion private properties

            #region public properties
            public System.Nullable<bool> IsProvvisoria { get { return _IsProvvisoria; } set { _IsProvvisoria = value; } }
            public System.Nullable<System.DateTime> InizioBonus { get { return _InizioBonus; } set { _InizioBonus = value; } }
            public System.Nullable<System.DateTime> FineBonus { get { return _FineBonus; } set { _FineBonus = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
