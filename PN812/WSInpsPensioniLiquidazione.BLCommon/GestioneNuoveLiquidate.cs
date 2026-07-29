using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneNuoveLiquidate
    {
        public static void GetNuoveLiquidateByIdPensione(long idPensione, out NuoveLiquidate nuoveLiquidate)
        {
            nuoveLiquidate = null;
            DataCommon.NuoveLiquidate nuoveLiquidateDb = new DataCommon.NuoveLiquidate();
            DAGestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(idPensione, out nuoveLiquidateDb);
            if (nuoveLiquidateDb != null)
            {
                nuoveLiquidate = new NuoveLiquidate();
                Utility.ValorizzaOggetti(nuoveLiquidateDb, nuoveLiquidate);
            }
        }

        public static void SalvaNuoveLiquidate(NuoveLiquidate nuoveLiquidate)
        {

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.NuoveLiquidate nuoveLiquidateDb = new DataCommon.NuoveLiquidate();
                Utility.ValorizzaOggetti(nuoveLiquidate, nuoveLiquidateDb);
                DAGestioneNuoveLiquidate.SalvaNuoveLiquidate(nuoveLiquidateDb);
                transactionScope.Complete();
            }
        }

        public static void EliminaNuoveLiquidateByIdPensione(long idPensione)
        {

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                 new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneNuoveLiquidate.DeleteNuoveLiquidateByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class NuoveLiquidate
        {
            #region private properties

            private long _Id;
            private long _IdPensione;
            private bool? _FlagProvvisoria;
            private bool? _FlagContributiva;
            private byte? _Affini;
            private byte? _Coniuge;
            private byte? _Figli;
            private short? _CodiceCategoriaReversibilita;
            private short? _SedeReversibilita;
            private int? _CertificatoReversibilita;
            private byte? _CodiceProcesso;
            private DateTime? _DataPresaInCarico;
            private byte? _CodiceProcessoDestinazione;
            private byte? _CodiceProcessoGP1ALZ6;
            private bool? _IsFlagProvvisoriaFromCumulo;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public bool? FlagProvvisoria { get { return _FlagProvvisoria; } set { _FlagProvvisoria = value; } }
            public bool? FlagContributiva { get { return _FlagContributiva; } set { _FlagContributiva = value; } }

            public byte? Affini { get { return _Affini; } set { _Affini = value; } }
            public byte? Coniuge { get { return _Coniuge; } set { _Coniuge = value; } }

            public byte? Figli { get { return _Figli; } set { _Figli = value; } }
            public short? CodiceCategoriaReversibilita { get { return _CodiceCategoriaReversibilita; } set { _CodiceCategoriaReversibilita = value; } }

            public short? SedeReversibilita { get { return _SedeReversibilita; } set { _SedeReversibilita = value; } }
            public int? CertificatoReversibilita { get { return _CertificatoReversibilita; } set { _CertificatoReversibilita = value; } }

            public byte? CodiceProcesso { get { return _CodiceProcesso; } set { _CodiceProcesso = value; } }
            public DateTime? DataPresaInCarico { get { return _DataPresaInCarico; } set { _DataPresaInCarico = value; } }

            public byte? CodiceProcessoDestinazione { get { return _CodiceProcessoDestinazione; } set { _CodiceProcessoDestinazione = value; } }

            public byte? CodiceProcessoGP1ALZ6 { get { return _CodiceProcessoGP1ALZ6; } set { _CodiceProcessoGP1ALZ6 = value; } }

            //ENG - Memo 108_2024
            public bool? IsFlagProvvisoriaFromCumulo { get { return _IsFlagProvvisoriaFromCumulo; } set { _IsFlagProvvisoriaFromCumulo = value; } }

            #endregion public properties 

            public override bool Equals(object obj)
            {
                NuoveLiquidate nuoveLiquidate = (NuoveLiquidate)obj;
                try
                {
                    if (this._FlagProvvisoria != nuoveLiquidate._FlagProvvisoria ||
                        this._FlagContributiva != nuoveLiquidate._FlagContributiva ||
                        this._Affini != nuoveLiquidate._Affini ||
                        this._Coniuge != nuoveLiquidate._Coniuge ||
                        this._Figli != nuoveLiquidate._Figli ||
                        this._CodiceCategoriaReversibilita != nuoveLiquidate._CodiceCategoriaReversibilita ||
                        this._SedeReversibilita != nuoveLiquidate._SedeReversibilita ||
                        this._CertificatoReversibilita != nuoveLiquidate._CertificatoReversibilita ||
                        this._CodiceProcesso != nuoveLiquidate._CodiceProcesso ||
                        this._DataPresaInCarico != nuoveLiquidate._DataPresaInCarico ||
                        this._CodiceProcessoDestinazione != nuoveLiquidate._CodiceProcessoDestinazione)
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
