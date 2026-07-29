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
    public class GestioneSbloccoDomanda
    {
        public static void GetSbloccoDomandaByNumeroDomanda(Int64 numeroDomanda, out DatiSbloccoDomanda datiSbloccoDomanda)
        {
            SbloccoDomanda sbloccoDomanda = null;
            datiSbloccoDomanda = null;
            DAGestioneSbloccoDomanda.GetSbloccoDomandaByNumeroDomanda(numeroDomanda, out sbloccoDomanda);
            if (sbloccoDomanda == null)
                return;
            datiSbloccoDomanda = new DatiSbloccoDomanda();
            Utility.ValorizzaOggetti(sbloccoDomanda, datiSbloccoDomanda);
        }

        public static void SalvaSbloccoDomanda(DatiSbloccoDomanda datiSbloccoDomanda)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SbloccoDomanda sbloccoDomanda = new SbloccoDomanda();
                Utility.ValorizzaOggetti(datiSbloccoDomanda, sbloccoDomanda);
                DAGestioneSbloccoDomanda.SalvaSbloccoDomanda(sbloccoDomanda);

                transactionScope.Complete();
            }
        }

        public static void EliminaSbloccoDomanda(DatiSbloccoDomanda datiSbloccoDomanda)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SbloccoDomanda sbloccoDomanda = new SbloccoDomanda();
                Utility.ValorizzaOggetti(datiSbloccoDomanda, sbloccoDomanda);
                DAGestioneSbloccoDomanda.EliminaSbloccoDomanda(sbloccoDomanda);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiSbloccoDomanda
        {
            public DatiSbloccoDomanda()
            { }

            public DatiSbloccoDomanda(long nDomus, string matricolaBlocco, string timeStampBlocco)
            {
                this._NDomus = nDomus;
                this._MatricolaBlocco = !string.IsNullOrEmpty(matricolaBlocco) ? matricolaBlocco.ToUpperInvariant() : matricolaBlocco;
                this._TimeStampBlocco = !string.IsNullOrEmpty(timeStampBlocco) ? timeStampBlocco.ToUpperInvariant() : timeStampBlocco;
            }

            #region private properties
            private long _NDomus;
            private string _MatricolaBlocco;
            private string _TimeStampBlocco;
            #endregion private properties

            #region public properties
            public long NDomus { get { return _NDomus; } set { _NDomus = value; } }
            public string MatricolaBlocco { get { return _MatricolaBlocco; } set { _MatricolaBlocco = value; } }
            public string TimeStampBlocco { get { return _TimeStampBlocco; } set { _TimeStampBlocco = value; } }
            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiSbloccoDomanda sbloccoDomanda = (DatiSbloccoDomanda)obj;
                try
                {
                    if (this._NDomus != sbloccoDomanda._NDomus ||
                        this._MatricolaBlocco != sbloccoDomanda._MatricolaBlocco ||
                        this._TimeStampBlocco != sbloccoDomanda._TimeStampBlocco)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion nested class
    }
}
