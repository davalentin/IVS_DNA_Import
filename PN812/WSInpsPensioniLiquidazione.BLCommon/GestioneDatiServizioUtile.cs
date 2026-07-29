using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;
namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDatiServizioUtile
    {
        public static void GetDatiServizioUtileByIdPensione(Int64 idPensione, out List<ServizioUtile> lServizioUtile)
        {
            List<DatiServizioUtile> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(idPensione, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile>();
            foreach (DatiServizioUtile datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile servizioUtile = new ServizioUtile();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtileStoricoByIdPensione(Int64 idPensione, out List<ServizioUtile> lServizioUtile)
        {
            List<DatiServizioUtile> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneDatiServizioUtile.GetDatiServizioUtileStoricoByIdPensione(idPensione, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile>();
            foreach (DatiServizioUtile datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile servizioUtile = new ServizioUtile();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtileByIdRecordFondo(Int64 idRecordFondo, out List<ServizioUtile> lServizioUtile)
        {
            List<DatiServizioUtile> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneDatiServizioUtile.GetDatiServizioUtileByIdRecordFondo(idRecordFondo, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile>();
            foreach (DatiServizioUtile datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile servizioUtile = new ServizioUtile();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void GetDatiServizioUtileStoricoByIdRecordFondo(Int64 idRecordFondo, out List<ServizioUtile> lServizioUtile)
        {
            List<DatiServizioUtile> lDatiServizioUtileDB = null;
            lServizioUtile = null;
            DAGestioneDatiServizioUtile.GetDatiServizioUtileStoricoByIdRecordFondo(idRecordFondo, out lDatiServizioUtileDB);
            if (lDatiServizioUtileDB == null)
                return;
            lServizioUtile = new List<ServizioUtile>();
            foreach (DatiServizioUtile datiServizioUtileDB in lDatiServizioUtileDB)
            {
                ServizioUtile servizioUtile = new ServizioUtile();
                Utility.ValorizzaOggetti(datiServizioUtileDB, servizioUtile);
                lServizioUtile.Add(servizioUtile);
            }
        }

        public static void SalvaDatiServizioUtile(long idFondo, ServizioUtile servizioUtile)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiServizioUtile datiServizioUtile = new DatiServizioUtile();
                Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                datiServizioUtile.IdFondo = idFondo;
                DAGestioneDatiServizioUtile.SalvaDatiServizioUtile(datiServizioUtile);
                transactionScope.Complete();
            }
        }

        public static void SalvaDatiServizioUtileRecordFondo(long idFondo, long idRecordFondo, ServizioUtile servizioUtile)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiServizioUtile datiServizioUtile = new DatiServizioUtile();
                Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                datiServizioUtile.IdFondo = idFondo;
                datiServizioUtile.IdRecordFondo = idRecordFondo;
                DAGestioneDatiServizioUtile.SalvaDatiServizioUtileRecordFondo(datiServizioUtile);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtileByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiServizioUtileByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class ServizioUtile
        {
            #region private poperties
            private long _Id;

            private System.Nullable<long> _IdFondo;

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

            public System.Nullable<decimal> RetribuzionePensionabile { get { return _RetribuzionePensionabile; } set { _RetribuzionePensionabile = value; } }

            public System.Nullable<short> ControCodiceRetributivo { get { return _ControCodiceRetributivo; } set { _ControCodiceRetributivo = value; } }
            
            public System.Nullable<decimal> Retribuzione { get { return _Retribuzione; } set { _Retribuzione = value; } }

            public System.Nullable<decimal> QuoteArt14 { get { return _QuoteArt14; } set { _QuoteArt14 = value; } }

            public System.Nullable<decimal> ImportoIndennitaIntegrativaSpeciale { get { return _ImportoIndennitaIntegrativaSpeciale; } set { _ImportoIndennitaIntegrativaSpeciale = value; } }

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
                    !_RetribuzionePensionabile.HasValue &&
                    !_ControCodiceRetributivo.HasValue &&
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
