using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneSupplementi
    {
        public static void DeleteSupplementi(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneSupplementi.CancellaDatiSupplementiByIdPensione(idPensione);
                else
                    DAGestioneSupplementi.EliminaSupplementiNoStoricoByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        public static void GetSupplementiByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementi> Lsupplementi)
        {
            List<Supplementi> LsupplementiDB = null;
            Lsupplementi = null;
            DAGestioneSupplementi.GetDatiSupplementiByIdPensione(idPensione, out LsupplementiDB);

            if (LsupplementiDB == null)
                return;
            Lsupplementi = new List<BLCommon.Entity.DatiSupplementi>();
            foreach (Supplementi supplDB in LsupplementiDB)
            {
                BLCommon.Entity.DatiSupplementi supplBL = new BLCommon.Entity.DatiSupplementi();
                Utility.ValorizzaOggetti(supplDB, supplBL);
                supplBL.CodGestioneSupplemento = supplDB.CodiceGestioneSupplemento.HasValue ? supplDB.CodiceGestioneSupplemento.Value.ToString() : "";
                Lsupplementi.Add(supplBL);
            }
        }

        public static void GetSupplementiNoStoricoByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementi> LsupplementiNoStorico)
        {
            List<Supplementi> LsupplementiDB = null;
            LsupplementiNoStorico = null;
            DAGestioneSupplementi.GetDatiSupplementiNoStoricoByIdPensione(idPensione, out LsupplementiDB);

            if (LsupplementiDB == null)
                return;
            LsupplementiNoStorico = new List<BLCommon.Entity.DatiSupplementi>();
            foreach (Supplementi supplDB in LsupplementiDB)
            {
                BLCommon.Entity.DatiSupplementi supplBL = new BLCommon.Entity.DatiSupplementi();
                Utility.ValorizzaOggetti(supplDB, supplBL);
                supplBL.CodGestioneSupplemento = supplDB.CodiceGestioneSupplemento.HasValue ? supplDB.CodiceGestioneSupplemento.Value.ToString() : "";
                LsupplementiNoStorico.Add(supplBL);
            }
        }

        public static void GetSupplementiStoricoByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementi> lSupplementiStorico)
        {
            List<Supplementi> LsupplementiDB = null;
            lSupplementiStorico = null;
            DAGestioneSupplementi.GetSupplementiStoricoByIdPensione(idPensione, out LsupplementiDB);

            if (LsupplementiDB == null)
                return;
            lSupplementiStorico = new List<BLCommon.Entity.DatiSupplementi>();
            foreach (Supplementi supplDB in LsupplementiDB)
            {
                BLCommon.Entity.DatiSupplementi supplBL = new BLCommon.Entity.DatiSupplementi();
                Utility.ValorizzaOggetti(supplDB, supplBL);
                supplBL.CodGestioneSupplemento = supplDB.CodiceGestioneSupplemento.HasValue ? supplDB.CodiceGestioneSupplemento.Value.ToString() : "";
                lSupplementiStorico.Add(supplBL);
            }
        }

        public static void SalvaDatiSupplementi(long idPensione, List<BLCommon.Entity.DatiSupplementi> supplementi)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                List<Supplementi> LSupplDB = new List<Supplementi>();
                foreach (BLCommon.Entity.DatiSupplementi supplBL in supplementi)
                {
                    Supplementi supplDB = new Supplementi();
                    Utility.ValorizzaOggetti(supplBL, supplDB);
                    supplDB.CodiceGestioneSupplemento = Utility.StringToNullableChar(supplBL.CodGestioneSupplemento);
                    LSupplDB.Add(supplDB);
                }
                DAGestioneSupplementi.SalvaDatiSupplementi(idPensione, LSupplDB);

                transactionScope.Complete();
            }
        }

        public static void SalvaDatiSupplementiStorico(long idPensione, List<BLCommon.Entity.DatiSupplementi> lSupplementiStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                List<Supplementi> LSupplDB = new List<Supplementi>();
                foreach (BLCommon.Entity.DatiSupplementi supplBL in lSupplementiStorico)
                {
                    Supplementi supplDB = new Supplementi();
                    Utility.ValorizzaOggetti(supplBL, supplDB);
                    supplDB.CodiceGestioneSupplemento = Utility.StringToNullableChar(supplBL.CodGestioneSupplemento);
                    LSupplDB.Add(supplDB);
                }
                DAGestioneSupplementi.SalvaSupplementiStorico(idPensione, LSupplDB);

                transactionScope.Complete();
            }
        }

        //public static bool IsSupplementiVisible(long idPensione)
        //{
        //    Pensione pensione = null;
        //    DAGestionePensione.GetPensioneByIdPensione(idPensione, out pensione);
        //    bool bReturn = false;
        //    if (pensione != null)
        //        bReturn = IsSupplementiVisible(pensione.IndConvInt, pensione.Gestione, pensione.Gruppo, pensione.Prodotto, pensione.NaturaPensione);

        //    return bReturn;
        //}

        //public static bool IsSupplementiVisible(bool? IndConvInt, string Gestione, string Gruppo, string Prodotto, string NaturaPensione)
        //{
        //    //28-05-12: menu supplementi non più visibile per assegno di invalidità
        //    if (IndConvInt.HasValue && !string.IsNullOrEmpty(Gestione))
        //    {
        //        Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(IndConvInt, Gestione);
        //        Utility.TipoDomanda? tipoDomanda = Utility.GetTipoDomanda(Gruppo, Prodotto);
        //        if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
        //        {
        //            if (!String.IsNullOrEmpty(Prodotto) && Prodotto.Trim() == "0001" && !String.IsNullOrEmpty(NaturaPensione) && NaturaPensione.Substring(1, 1).ToUpperInvariant() == "Y")
        //                return true;

        //            return false;
        //        }
        //        if (tipoAppartenenza.HasValue && (tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO || tipoAppartenenza.Value == Utility.TipoAppartenenza.CI))
        //        {
        //            if(tipoDomanda != null && (tipoDomanda == Utility.TipoDomanda.Superstiti || tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti))
        //                return false;
        //        }
        //    }
        //    return true;
        //}



        public static void GetDatiSupplementiBaseByIdPensione(long idPensione, out BLCommon.Entity.SupplementiBase supplementiBaseBL)
        {
            SupplementiBase supplementiBaseDB = null;
            supplementiBaseBL = null;
            DAGestioneSupplementi.GetDatiSupplementiBaseByIdPensione(idPensione, out supplementiBaseDB);
            if (supplementiBaseDB != null)
            {
                supplementiBaseBL = new BLCommon.Entity.SupplementiBase();
                Utility.ValorizzaOggetti(supplementiBaseDB, supplementiBaseBL);
            }
        }

        public static void SalvaDatiSupplementiBase(long idPensione, BLCommon.Entity.SupplementiBase supplementiBaseBL)
        {
            SupplementiBase supplementiBaseDB = new SupplementiBase();
            Utility.ValorizzaOggetti(supplementiBaseBL, supplementiBaseDB);
            supplementiBaseDB.IdPensione = idPensione;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSupplementi.SalvaSupplementiBase(supplementiBaseDB);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiSupplementiBase(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSupplementi.CancellaDatiSupplementiBaseByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        #region Enpals

        public static void GetDatiSupplementiEnpalsByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementiENPALS> lSupplementiEnpalsBL)
        {
            List<SupplementiENPAL> lSupplementiEnpalsDB = null;
            lSupplementiEnpalsBL = null;
            DAGestioneSupplementi.GetDatiSupplementiEnpalsByIdPensione(idPensione, out lSupplementiEnpalsDB);

            if (lSupplementiEnpalsDB == null)
                return;
            lSupplementiEnpalsBL = new List<BLCommon.Entity.DatiSupplementiENPALS>();
            foreach (SupplementiENPAL supplDB in lSupplementiEnpalsDB)
            {
                BLCommon.Entity.DatiSupplementiENPALS supplBL = new BLCommon.Entity.DatiSupplementiENPALS();
                Utility.ValorizzaOggetti(supplDB, supplBL);
                lSupplementiEnpalsBL.Add(supplBL);
            }
        }

        public static void SalvaDatiSupplementiEnpals(long idPensione, BLCommon.Entity.DatiSupplementiENPALS supplementiEnpalsBL)
        {
            SupplementiENPAL supplementiEnpalsDB = new SupplementiENPAL();
            Utility.ValorizzaOggetti(supplementiEnpalsBL, supplementiEnpalsDB);
            supplementiEnpalsDB.IdPensione = idPensione;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSupplementi.SalvaDatiSupplementiEnpals(supplementiEnpalsDB);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiSupplementiEnpals(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneSupplementi.CancellaDatiSupplementiEnpalsByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        // Operazione sui supplementi ENPALS che utilizzano idRecordSuppENPALS ////////////////

        public static void GetDatiSupplementiEnpalsByIdSuppRecordENPALS(long idRecord, out List<BLCommon.Entity.DatiSupplementiENPALS> lSupplementiEnpalsBL)
        {
            List<SupplementiENPAL> lSupplementiEnpalsDB = null;
            lSupplementiEnpalsBL = null;
            DAGestioneSupplementi.GetDatiSupplementiEnpalsByIdSuppRecordENPALS(idRecord, out lSupplementiEnpalsDB);

            if (lSupplementiEnpalsDB == null)
                return;
            lSupplementiEnpalsBL = new List<BLCommon.Entity.DatiSupplementiENPALS>();
            foreach (SupplementiENPAL supplDB in lSupplementiEnpalsDB)
            {
                BLCommon.Entity.DatiSupplementiENPALS supplBL = new BLCommon.Entity.DatiSupplementiENPALS();
                Utility.ValorizzaOggetti(supplDB, supplBL);
                lSupplementiEnpalsBL.Add(supplBL);
            }
        }

        public static void SalvaDatiSupplementiEnpalsByIdSuppRecordENPALS(long idPensione, long idSuppRecordENPALS, BLCommon.Entity.DatiSupplementiENPALS supplementiEnpalsBL)
        {
            SupplementiENPAL supplementiEnpalsDB = new SupplementiENPAL();
            Utility.ValorizzaOggetti(supplementiEnpalsBL, supplementiEnpalsDB);
            supplementiEnpalsDB.IdPensione = idPensione;
            supplementiEnpalsDB.IdSuppRecordENPALS = idSuppRecordENPALS;
            DAGestioneSupplementi.SalvaDatiSupplementiEnpals(supplementiEnpalsDB);
        }

        public static void EliminaDatiSupplementiEnpalsByIdSuppRecordENPALS(long idSuppRecordENPALS)
        {
            DAGestioneSupplementi.CancellaDatiSupplementiEnpalsByIdSuppRecordENPALS(idSuppRecordENPALS);
        }

        public static void GetDatiSuppRecordEnpalsByIdPensione(long idPensione, out List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS> lstBl)
        {
            lstBl = null;
            List<SupplementiRecordENPAL> lstDb = null;
            DAGestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(idPensione, out lstDb);

            List<SupplementiENPAL> lstSuppDb = null;
            DAGestioneSupplementi.GetDatiSupplementiEnpalsByIdPensione(idPensione, out lstSuppDb);

            if (lstDb == null)
                return;
            lstBl = lstDb.Select(x =>
                {
                    var objBl = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS();
                    Utility.ValorizzaOggetti(x, objBl);
                    objBl.IdSuppRecordEnpals = x.Id;
                    return objBl;
                })
                .OrderBy(x => x.Decorrenza)
                .ToList();
        }

        public static void GetDatiSuppRecordEnpalsyIdRecord(long idRecord, out INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS objBl)
        {
            objBl = null;
            SupplementiRecordENPAL objDb = null;
            DAGestioneSupplementi.GetDatiSuppRecordEnpalsyIdRecord(idRecord, out objDb);

            if (objDb == null)
                return;
            objBl = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS();
            Utility.ValorizzaOggetti(objDb, objBl);
            objBl.IdSuppRecordEnpals = objDb.Id;
        }

        public static void SalvaDatiSuppRecordEnpals(long idPensione, INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS recordSupp, out long? idRecord)
        {
            SupplementiRecordENPAL objDb = new SupplementiRecordENPAL();
            Utility.ValorizzaOggetti(recordSupp, objDb);
            objDb.IdPensione = idPensione;
            objDb.Id = recordSupp.IdSuppRecordEnpals;
            DAGestioneSupplementi.SalvaDatiSuppRecordEnpals(objDb, out idRecord);
        }

        public static void EliminaDatiSuppRecordEnpalsByIdPensione(long idPensione)
        {
            DAGestioneSupplementi.CancellaDatiSuppRecordEnpalsByIdPensione(idPensione);
        }

        public static void EliminaDatiSuppRecordEnpalsByIdRecord(long idRecord)
        {
            DAGestioneSupplementi.CancellaDatiSuppRecordEnpalsByIdRecord(idRecord);
        }

        #endregion Enpals

        #region Cumulo

        public static void GetSupplementiCumuloByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementiCumulo> lSupplementi)
        {
            lSupplementi = null;
            List<DataCommon.SupplementiCumulo> lSupplementiCumuloDB = null;

            DAGestioneSupplementi.GetSupplementiCumuloByIdPensione(idPensione, out lSupplementiCumuloDB);
            if (lSupplementiCumuloDB == null || lSupplementiCumuloDB.Count == 0)
                return;

            lSupplementi = new List<BLCommon.Entity.DatiSupplementiCumulo>();

            foreach (DataCommon.SupplementiCumulo supplementoDB in lSupplementiCumuloDB)
            {
                BLCommon.Entity.DatiSupplementiCumulo supplemento = new BLCommon.Entity.DatiSupplementiCumulo();
                Utility.ValorizzaOggetti(supplementoDB, supplemento);
                lSupplementi.Add(supplemento);
            }
        }

        public static void GetSupplementiCumuloStoricoByIdPensione(long idPensione, out List<BLCommon.Entity.DatiSupplementiCumulo> lSupplementiCumulo)
        {
            lSupplementiCumulo = null;
            List<DataCommon.SupplementiCumulo> lSupplementiCumuloDB = null;

            DAGestioneSupplementi.GetSupplementiCumuloStoricoByIdPensione(idPensione, out lSupplementiCumuloDB);
            if (lSupplementiCumuloDB == null || lSupplementiCumuloDB.Count == 0)
                return;

            lSupplementiCumulo = new List<BLCommon.Entity.DatiSupplementiCumulo>();

            foreach (DataCommon.SupplementiCumulo supplementoDB in lSupplementiCumuloDB)
            {
                BLCommon.Entity.DatiSupplementiCumulo supplemento = new Entity.DatiSupplementiCumulo();
                Utility.ValorizzaOggetti(supplementoDB, supplemento);
                lSupplementiCumulo.Add(supplemento);
            }
        }

        public static void SalvaSupplementiCumulo(BLCommon.Entity.DatiSupplementiCumulo supplementi)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.SupplementiCumulo supplementiDB = new DataCommon.SupplementiCumulo();
                Utility.ValorizzaOggetti(supplementi, supplementiDB);
                DAGestioneSupplementi.SalvaSupplementiCumulo(supplementiDB);
                transactionScope.Complete();
            }
        }

        public static void SalvaDatiSupplementiCumulo(List<BLCommon.Entity.DatiSupplementiCumulo> lSupplementiCumulo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (BLCommon.Entity.DatiSupplementiCumulo supplemento in lSupplementiCumulo)
                    SalvaSupplementiCumulo(supplemento);
                transactionScope.Complete();
            }
        }

        public static void EliminaSupplementiCumuloByIdPensione(long idPensione, bool eliminaStorico)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (eliminaStorico)
                    DAGestioneSupplementi.EliminaSupplementiCumuloByIdPensione(idPensione);
                else
                    DAGestioneSupplementi.EliminaSupplementiCumuloNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion
    }
}
