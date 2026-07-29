using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDatiNoCalcolo
    {
        #region Public Methods


        public static void GetRecordNoCalcoloByIdRecord(long idRecord, out RecordDatiNoCalcolo objBl)
        {
            objBl = null;

            DataCommon.RecordDatiNoCalcolo objDb;
            DAGestioneNoCalcolo.GetRecordNoCalcoloByIdRecord(idRecord, out objDb);
            if (objDb != null)
            {
                objBl = new RecordDatiNoCalcolo();
                Utility.ValorizzaOggetti(objDb, objBl);
            }
        }

        public static void GetRecordNoCalcoloByIdPensione(long idPensione, out List<RecordDatiNoCalcolo> lstRecordDatiNoCalcolo)
        {
            lstRecordDatiNoCalcolo = new List<RecordDatiNoCalcolo>();
            
            List<DataCommon.RecordDatiNoCalcolo> lstRecordDatiNoCalcoloDB;
            DAGestioneNoCalcolo.GetRecordNoCalcolo(idPensione, out lstRecordDatiNoCalcoloDB);
            if (lstRecordDatiNoCalcoloDB != null && lstRecordDatiNoCalcoloDB.Count > 0)
                lstRecordDatiNoCalcolo = lstRecordDatiNoCalcoloDB.Select(x => { var y = new RecordDatiNoCalcolo(); Utility.ValorizzaOggetti(x, y); return y; }).ToList();  
        }

        public static void SalvaRecordNoCalcolo(long idPensione,RecordDatiNoCalcolo recordNoCalcolo,out long? idRecord)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                idRecord = null;
                DataCommon.RecordDatiNoCalcolo objDb = new DataCommon.RecordDatiNoCalcolo();
                Utility.ValorizzaOggetti(recordNoCalcolo, objDb);
                objDb.IdPensione = idPensione;
                DAGestioneNoCalcolo.SalvaRecordNoCalcolo(objDb, ref idRecord);
                transactionScope.Complete();
            }
        }

       
        public static void DeleteRecordDatiNoCalcolo(long idRecordNoCalcolo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneNoCalcolo.DeleteRecordNoCalcolo(idRecordNoCalcolo);
                
                transactionScope.Complete();
            }
        }

        public static void DeleteAllRecordDatiNoCalcolo(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneNoCalcolo.DeleteAllRecordNoCalcolo(idPensione);
                
                transactionScope.Complete();
            }
        }

        #endregion Public Methods

        #region Nestled Class
        public class RecordDatiNoCalcolo
        {
            public long Id {get;set;}

            public long IdPensione { get; set; }

            public string Decorrenza { get; set; }

            public System.Nullable<decimal> AdeguataAgo { get; set; }

            public System.Nullable<decimal> AdeguataFondo { get; set; }

            public System.Nullable<decimal> EccedenzaAgo { get; set; }

            public System.Nullable<decimal> QuotaAgoEsclusiva { get; set; }

            public System.Nullable<decimal> FacArt14 { get; set; }

            public System.Nullable<decimal> IndIntSpeciale { get; set; }

            public System.Nullable<decimal> AssegniFamiliari { get; set; }

            public System.Nullable<decimal> AggFamigliaFondo { get; set; }

            public System.Nullable<decimal> OnereCaricoAmm { get; set; }

            public System.Nullable<decimal> Art21 { get; set; }

            public System.Nullable<decimal> ImportoMensile { get; set; }

            public System.Nullable<decimal> Tredicesima { get; set; }

            public System.Nullable<decimal> TipoVar { get; set; }

            public bool isNull()
            {
                bool ret = false;
                if (!AdeguataAgo.HasValue && !AdeguataFondo.HasValue && !EccedenzaAgo.HasValue && !QuotaAgoEsclusiva.HasValue &&
                    !FacArt14.HasValue && !IndIntSpeciale.HasValue && !AssegniFamiliari.HasValue && !AggFamigliaFondo.HasValue &&
                    !OnereCaricoAmm.HasValue && !Art21.HasValue && !ImportoMensile.HasValue && !Tredicesima.HasValue && !TipoVar.HasValue)
                    ret = true;
                return ret;
            }

        }
        #endregion Nestled Class
    }
}
