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
    public class GestioneAltrePensioni
    {
        public static void GetAltraPensioneByIdPensione(long idPensione, out List<AltraPensione> LaltraPensione)
        {
            
            LaltraPensione = null;
            List<AltrePensioni> LaltraPensioneDB = null;
            DAGestioneAltrePensioni.GetAltrePensioniByIdPensione(idPensione, out LaltraPensioneDB);
            if (LaltraPensioneDB == null)
                return;
            LaltraPensione = new List<AltraPensione>();
            foreach (AltrePensioni ltraPensioneDB in LaltraPensioneDB)
            {
                AltraPensione altraPensioneBL = new AltraPensione();
                Utility.ValorizzaOggetti(ltraPensioneDB, altraPensioneBL);
                LaltraPensione.Add(altraPensioneBL);
            }
        }

        public static void SalvaAltraPensione(AltraPensione altraPensione)
        {
            DataCommon.AltrePensioni altraPensioneDB = new AltrePensioni();
            Utility.ValorizzaOggetti(altraPensione, altraPensioneDB);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAltrePensioni.SalvaAltrePensioni(altraPensioneDB);

                transactionScope.Complete();
            }
        }

        public static void DeleteAltraPensioneByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAltrePensioni.CancelAltrePensioniByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        public static bool VerifyCtrlBititolarita(string codCategoria, char codUC, char codImporto, Utility.TipoAppartenenza tipoApp)
        {
            List<CtrlBititolarita> Lbitit = null;
            DAGestioneAltrePensioni.VerifyCtrlBititolarita(codCategoria, codUC, codImporto, tipoApp.ToString(), out Lbitit);
            if (Lbitit == null || Lbitit.Count == 0)
                return false;
            return true;
        }


        #region nested class
        public class AltraPensione
        {
            #region private properties
            private System.Nullable<long> _IdPensione;
            private string _Categoria;
            private System.Nullable<byte> _Ente;
            private System.Nullable<int> _Certificato;
            private System.Nullable<System.DateTime> _Decorrenza;
            private System.Nullable<System.DateTime> _Cessazione;
            private System.Nullable<char> _CodiceUC;
            private System.Nullable<char> _CodiceImporto;
            private System.Nullable<decimal> _ImportoAltraPensione;
            #endregion private properties

            #region public properties
            public System.Nullable<long> IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public string Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public System.Nullable<byte> Ente { get { return _Ente; } set { _Ente = value; } }
            public System.Nullable<int> Certificato { get { return _Certificato; } set { _Certificato = value; } }
            public System.Nullable<char> CodiceUC { get { return _CodiceUC; } set { _CodiceUC = value; } }
            public System.Nullable<char> CodiceImporto { get { return _CodiceImporto; } set { _CodiceImporto = value; } }
            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public System.Nullable<System.DateTime> Cessazione { get { return _Cessazione; } set { _Cessazione = value; } }
            public System.Nullable<decimal> ImportoAltraPensione { get { return _ImportoAltraPensione; } set { _ImportoAltraPensione = value; } }
            #endregion public properties

            public bool IsNull()
            {
                if (string.IsNullOrEmpty(_Categoria) && !_Ente.HasValue && !_Certificato.HasValue && !_Decorrenza.HasValue &&
                    !_Cessazione.HasValue && !_CodiceUC.HasValue && !_CodiceImporto.HasValue && !_ImportoAltraPensione.HasValue)
                    return true;

                return false;
            }

        }
        #endregion nested class
    }
}
