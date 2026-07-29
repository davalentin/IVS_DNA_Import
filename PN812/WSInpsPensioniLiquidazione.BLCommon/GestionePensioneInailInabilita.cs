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
    public class GestionePensioneInailInabilita
    {
        public static void SalvaInabilita(DatiInabilita datiInabilita)
        {
            Inabilita InabilitaDB = new Inabilita();
            BLCommon.Utility.ValorizzaOggetti(datiInabilita, InabilitaDB);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensioneInailInabilita.SalvaInabilita(InabilitaDB);
                transactionScope.Complete();
            }
        }

        public static void SalvaPensioniINAIL(DatiPensioniINAIL datiPensioniINAIL)
        {
            PensioniINAIL PensioniINAILDB = new PensioniINAIL();
            BLCommon.Utility.ValorizzaOggetti(datiPensioniINAIL, PensioniINAILDB);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensioneInailInabilita.SalvaPensioneInail(PensioniINAILDB);
                transactionScope.Complete();
            }
        }
        
        public static void GetInabilitaByIdPensione(long idPensione, out DatiInabilita datiInabilita)
        {
            datiInabilita = null;
            Inabilita inabilita = null;
            DAGestionePensioneInailInabilita.GetInabilita(idPensione, out inabilita);
            if (inabilita != null)
            {
                datiInabilita = new DatiInabilita();
                BLCommon.Utility.ValorizzaOggetti(inabilita, datiInabilita);
            }
        }

        public static void GetPensioniINAILByIdPensione(long idPensione, out List<DatiPensioniINAIL> LdatiPensioniINAIL)
        {
            LdatiPensioniINAIL = null;
            List<PensioniINAIL> LpensioniInail = null;
            DAGestionePensioneInailInabilita.GetPensioneInailByIdPensione(idPensione, out LpensioniInail);
            if (LpensioniInail != null && LpensioniInail.Count > 0)
            {
                LdatiPensioniINAIL = new List<DatiPensioniINAIL>();
                foreach (PensioniINAIL pi in LpensioniInail)
                {
                    DatiPensioniINAIL datiPensioniINAIL = new DatiPensioniINAIL();
                    BLCommon.Utility.ValorizzaOggetti(pi, datiPensioniINAIL);
                    LdatiPensioniINAIL.Add(datiPensioniINAIL);
                }
            }
        }

        public static void EliminaInabilita(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensioneInailInabilita.CancellaInabilita(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaPensioniINAILByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensioneInailInabilita.CancellaPensioneInailByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class

        public class DatiInabilita
        {
            #region public properties
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? DecorrenzaDirittoIntegrazioneMinimo { get { return _DecorrenzaDirittoIntegrazioneMinimo; } set { _DecorrenzaDirittoIntegrazioneMinimo = value; } }
            public DateTime? CessazioneDirittoIntegrazioneMinimo { get { return _CessazioneDirittoIntegrazioneMinimo; } set { _CessazioneDirittoIntegrazioneMinimo = value; } }
            public DateTime? SospensionePensioneInvalidita { get { return _SospensionePensioneInvalidita; } set { _SospensionePensioneInvalidita = value; } }
            public DateTime? RipristinoPensioneInvalidita { get { return _RipristinoPensioneInvalidita; } set { _RipristinoPensioneInvalidita = value; } }
            public decimal? ImportoMensile { get { return _ImportoMensile; } set { _ImportoMensile = value; } }
            public DateTime? DecorrenzaAssegnoAccompangamento { get { return _DecorrenzaAssegnoAccompangamento; } set { _DecorrenzaAssegnoAccompangamento = value; } }
            public DateTime? CessazioneAssegnoAccompangamento { get { return _CessazioneAssegnoAccompangamento; } set { _CessazioneAssegnoAccompangamento = value; } }
            public bool? DirittoAssegnoAccompagnamento { get { return _DirittoAssegnoAccompagnamento; } set { _DirittoAssegnoAccompagnamento = value; } }
            #endregion public properties

            #region private properties
            private long? _IdPensione;
            private DateTime? _DecorrenzaDirittoIntegrazioneMinimo;
            private DateTime? _CessazioneDirittoIntegrazioneMinimo;
            private DateTime? _SospensionePensioneInvalidita;
            private DateTime? _RipristinoPensioneInvalidita;
            private decimal? _ImportoMensile;
            private DateTime? _DecorrenzaAssegnoAccompangamento;
            private DateTime? _CessazioneAssegnoAccompangamento;
            bool? _DirittoAssegnoAccompagnamento;
            #endregion private properties

            #region public methods
            public bool IsNull()
            {
                if (_DecorrenzaDirittoIntegrazioneMinimo.HasValue || _CessazioneDirittoIntegrazioneMinimo.HasValue || _SospensionePensioneInvalidita.HasValue || _RipristinoPensioneInvalidita.HasValue ||
                    _ImportoMensile.HasValue || _DecorrenzaAssegnoAccompangamento.HasValue || _CessazioneAssegnoAccompangamento.HasValue || _DirittoAssegnoAccompagnamento.HasValue)
                    return false;

                return true;
            }
            #endregion public methods
        }

        public class DatiPensioniINAIL
        {
            #region public properties
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? DecorrenzaRenditaInail { get { return _DecorrenzaRenditaInail; } set { _DecorrenzaRenditaInail = value; } }
            public bool? Evento { get { return _Evento; } set { _Evento = value; } }
            public decimal? ImportoMensileInail { get { return _ImportoMensileInail; } set { _ImportoMensileInail = value; } }
            #endregion public properties

            #region private properties
            private long _IdPensione;
            private DateTime? _DecorrenzaRenditaInail;
            private bool? _Evento;
            private decimal? _ImportoMensileInail;
            #endregion private properties

            #region public methods
            public bool IsNull()
            {
                if (_DecorrenzaRenditaInail.HasValue || _Evento.HasValue || _ImportoMensileInail.HasValue)
                    return false;

                return true;
            }
            #endregion public methods
        }

        #endregion nested class
    }
}
