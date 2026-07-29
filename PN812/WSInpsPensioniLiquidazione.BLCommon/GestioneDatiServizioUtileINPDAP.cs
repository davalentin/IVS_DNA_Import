using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDatiServizioUtileINPDAP
    {
        public static void GetDatiServizioUtileByIdPensione(Int64 idPensione, out List<ServizioUtile> lServizioUtile)
        {
            List<DatiServizioUtileINPDAP> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdPensione(idPensione, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile>();
            foreach (DatiServizioUtileINPDAP datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile servizioUtile = new ServizioUtile();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtileByIdRecordFondo(Int64 idRecordFondo, out List<ServizioUtile> lServizioUtile)
        {
            List<DatiServizioUtileINPDAP> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdRecordFondo(idRecordFondo, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile>();
            foreach (DatiServizioUtileINPDAP datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile servizioUtile = new ServizioUtile();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void SalvaDatiServizioUtileRecordFondo(long idPensione, long idRecordFondo, ServizioUtile servizioUtile)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiServizioUtileINPDAP datiServizioUtile = new DatiServizioUtileINPDAP();
                Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                datiServizioUtile.IdPensione = idPensione;
                datiServizioUtile.IdRecordFondo = idRecordFondo;
                DAGestioneDatiServizioUtileINPDAP.SalvaDatiServizioUtile(datiServizioUtile);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtileByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtileByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class ServizioUtile
        {
            #region private poperties
            private long _Id;

            private System.Nullable<long> _IdPensione;

            private System.Nullable<long> _IdRecordFondo;

            private string _Quota;

            private System.Nullable<short> _ServizioUtileAA;

            private System.Nullable<short> _ServizioUtileMM;

            private System.Nullable<short> _ServizioUtileGG;

            private System.Nullable<decimal> _RetribuzionePensionabile;

            private System.Nullable<short> _ControCodiceRetributivo;

            private System.Nullable<decimal> _Retribuzione;

            private System.Nullable<decimal> _QuoteArt14;

            private System.Nullable<decimal> _ImportoIndennitaIntegrativaSpeciale;

            private System.Nullable<short> _ServizioUtileCessazioneAA;

            private System.Nullable<short> _ServizioUtileCessazioneMM;

            private System.Nullable<short> _ServizioUtileCessazioneGG;

            private System.Nullable<decimal> _QuotaPensioneRetributivaAnnua;

            #endregion private poperties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }

            public System.Nullable<long> IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }

            public string Quota { get { return _Quota; } set { _Quota = value; } }

            public System.Nullable<short> ServizioUtileAA { get { return _ServizioUtileAA; } set { _ServizioUtileAA = value; } }

            public System.Nullable<short> ServizioUtileMM { get { return _ServizioUtileMM; } set { _ServizioUtileMM = value; } }

            public System.Nullable<short> ServizioUtileGG { get { return _ServizioUtileGG; } set { _ServizioUtileGG = value; } }

            public System.Nullable<decimal> RetribuzionePensionabile { get { return _RetribuzionePensionabile; } set { _RetribuzionePensionabile = value; } }

            public System.Nullable<short> ControCodiceRetributivo { get { return _ControCodiceRetributivo; } set { _ControCodiceRetributivo = value; } }

            public System.Nullable<decimal> Retribuzione { get { return _Retribuzione; } set { _Retribuzione = value; } }

            public System.Nullable<decimal> QuoteArt14 { get { return _QuoteArt14; } set { _QuoteArt14 = value; } }

            public System.Nullable<decimal> ImportoIndennitaIntegrativaSpeciale { get { return _ImportoIndennitaIntegrativaSpeciale; } set { _ImportoIndennitaIntegrativaSpeciale = value; } }

            public System.Nullable<short> ServizioUtileCessazioneAA { get { return _ServizioUtileCessazioneAA; } set { _ServizioUtileCessazioneAA = value; } }

            public System.Nullable<short> ServizioUtileCessazioneMM { get { return _ServizioUtileCessazioneMM; } set { _ServizioUtileCessazioneMM = value; } }

            public System.Nullable<short> ServizioUtileCessazioneGG { get { return _ServizioUtileCessazioneGG; } set { _ServizioUtileCessazioneGG = value; } }

            public System.Nullable<decimal> QuotaPensioneRetributivaAnnua { get { return _QuotaPensioneRetributivaAnnua; } set { _QuotaPensioneRetributivaAnnua = value; } }

            #endregion public properties

            public bool IsNull()
            {
                if (string.IsNullOrEmpty(this._Quota) &&
                    !_ServizioUtileAA.HasValue &&
                    !_ServizioUtileMM.HasValue &&
                    !_ServizioUtileGG.HasValue &&
                    !_RetribuzionePensionabile.HasValue &&
                    !_Retribuzione.HasValue &&
                    !_QuoteArt14.HasValue &&
                    !_ImportoIndennitaIntegrativaSpeciale.HasValue &&
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
