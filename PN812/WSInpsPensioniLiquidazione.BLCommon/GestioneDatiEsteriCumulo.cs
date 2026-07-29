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
    public class GestioneDatiEsteriCumulo
    {
        #region PrestazioniEstereCumulo
        public static void SalvaListaPrestazioniEstereCumulo(long idPensione, List<PensioneEsteraCumulo> listaPrestazioni)
        {
            if (idPensione != 0 && listaPrestazioni != null && listaPrestazioni.Count > 0)
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (PensioneEsteraCumulo prestazione in listaPrestazioni)
                    {
                        prestazione.IdPensione = idPensione;
                        SalvaPrestazioneEsteraCumulo(prestazione);
                    }
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPrestazioneEsteraCumulo(PensioneEsteraCumulo prestazione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.PensioneEsteraCumulo prestazioneDB = new DataCommon.PensioneEsteraCumulo();
                Utility.ValorizzaOggetti(prestazione, prestazioneDB);
                DAGestioneDatiEsteriCumulo.SalvaPrestazioneEsteraCumulo(prestazioneDB);
                prestazione.Id = prestazioneDB.Id;
                transactionScope.Complete();
            }
        }

        public static void GetPrestazioniEstereCumuloByIdPensione(long idPensione, out List<PensioneEsteraCumulo> listaPrestazioniEsteraCumulo)
        {
            listaPrestazioniEsteraCumulo = new List<PensioneEsteraCumulo>();
            List<DataCommon.PensioneEsteraCumulo> listaPrestazioniEEDB;
            DAGestioneDatiEsteriCumulo.GetPrestazioneEsteraCumuloByIdPensione(idPensione, false, out listaPrestazioniEEDB);
            if (listaPrestazioniEEDB != null && listaPrestazioniEEDB.Count > 0)
            {
                foreach (DataCommon.PensioneEsteraCumulo PrestazioneEEDB in listaPrestazioniEEDB)
                {
                    PensioneEsteraCumulo PrestazioneEE = new PensioneEsteraCumulo();
                    Utility.ValorizzaOggetti(PrestazioneEEDB, PrestazioneEE);
                    listaPrestazioniEsteraCumulo.Add(PrestazioneEE);
                }
            }
        }

        public static void GetPrestazioniEstereCumuloStoricoByIdPensione(long idPensione, out List<PensioneEsteraCumulo> listaPrestazioniEsteraCumuloStorico)
        {
            listaPrestazioniEsteraCumuloStorico = new List<PensioneEsteraCumulo>();
            List<DataCommon.PensioneEsteraCumulo> listaPrestazioniEsteraCumuloStoricoDB;
            DAGestioneDatiEsteriCumulo.GetPrestazioneEsteraCumuloByIdPensione(idPensione, true, out listaPrestazioniEsteraCumuloStoricoDB);
            if (listaPrestazioniEsteraCumuloStoricoDB != null && listaPrestazioniEsteraCumuloStoricoDB.Count > 0)
            {
                foreach (DataCommon.PensioneEsteraCumulo prestazioneEEDBStorico in listaPrestazioniEsteraCumuloStoricoDB)
                {
                    PensioneEsteraCumulo prestazioneEE = new PensioneEsteraCumulo();
                    Utility.ValorizzaOggetti(prestazioneEEDBStorico, prestazioneEE);
                    listaPrestazioniEsteraCumuloStorico.Add(prestazioneEE);
                }
            }
        }

        public static void EliminaPrestazioniEE(long idPrestazioniEE)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiEsteriCumulo.DeletePrestazioniEE(idPrestazioniEE);
                transactionScope.Complete();
            }
        }

        public static void EliminaAllPrestazioniEstereCumulo(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiEsteriCumulo.DeleteAllImportiEsteriCumuloByIdPensione(idPensione);
                DAGestioneDatiEsteriCumulo.DeletePrestazioneEsteraCumuloNoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }
        #endregion PrestazioniEstereCumulo

        #region ImportiEsteriCumulo
        public static void SalvaImportoEsteroCumulo(PensioneImportiEsteriCumulo importo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.PensioneImportiEsteriCumulo importoDB = new DataCommon.PensioneImportiEsteriCumulo();
                Utility.ValorizzaOggetti(importo, importoDB);
                DAGestioneDatiEsteriCumulo.SalvaImportoEsteroCumulo(importoDB);
                transactionScope.Complete();
            }
        }

        public static void GetImportiEsteriCumuloByIdPensione(long idPensione, out List<PensioneImportiEsteriCumulo> listaImportiEsteri)
        {
            listaImportiEsteri = new List<PensioneImportiEsteriCumulo>();

            List<DataCommon.PensioneImportiEsteriCumulo> listaImportiEsteriDB;
            DAGestioneDatiEsteriCumulo.GetImportiEsteriCumulolByIdPensione(idPensione, out listaImportiEsteriDB);
            if (listaImportiEsteriDB != null && listaImportiEsteriDB.Count > 0)
            {
                foreach (DataCommon.PensioneImportiEsteriCumulo ImportoEsteroDB in listaImportiEsteriDB)
                {
                    PensioneImportiEsteriCumulo ImportoEstero = new PensioneImportiEsteriCumulo();
                    Utility.ValorizzaOggetti(ImportoEsteroDB, ImportoEstero);
                    listaImportiEsteri.Add(ImportoEstero);
                }
            }
        }

        public static void EliminaImportiEsteriCumulo(long idImportiEsteri)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiEsteriCumulo.DeleteImportiEsteri(idImportiEsteri);
                transactionScope.Complete();
            }
        }

        public static void EliminaImportiEsteriCumuloPerPrestazione(long idPrestazioneEE)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiEsteriCumulo.DeleteImportiEsteriCumuloPerPrestazione(idPrestazioneEE);
                transactionScope.Complete();
            }
        }
        #endregion ImportiEsteriCumulo

        #region nested classes
        [Serializable]
        public class PensioneEsteraCumulo
        {

            #region private properties
            private long _Id;

            private long _IdPensione;

            private string _CodiceStato;

            private string _CodiceIstituzione;

            private string _MatricolaEstera;

            private System.Nullable<int> _ContributiDiritto;

            private System.Nullable<int> _SettimaneMisura;

            private System.Nullable<byte> _CodiceConvenzione;

            private bool? _Confermato;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public System.String CodiceStato { get { return _CodiceStato; } set { _CodiceStato = value; } }

            public System.String CodiceIstituzione { get { return _CodiceIstituzione; } set { _CodiceIstituzione = value; } }

            public System.String MatricolaEstera { get { return _MatricolaEstera; } set { _MatricolaEstera = value; } }

            public System.Nullable<int> ContributiDiritto { get { return _ContributiDiritto; } set { _ContributiDiritto = value; } }

            public System.Nullable<int> SettimaneMisura { get { return _SettimaneMisura; } set { _SettimaneMisura = value; } }

            public System.Nullable<byte> CodiceConvenzione { get { return _CodiceConvenzione; } set { _CodiceConvenzione = value; } }

            public bool? Confermato { get { return _Confermato; } set { _Confermato = value; } }
            #endregion public properties
        }

        public class PensioneImportiEsteriCumulo
        {

            #region private properties
            private long _Id;

            private long _IdPensioneEsteraCumulo;

            private System.Nullable<System.DateTime> _DecorrenzaPrestazione;

            private System.Nullable<decimal> _ImportoPrestazione;

            private System.Nullable<System.DateTime> _CessazionePrestazione;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public long IdPensioneEsteraCumulo { get { return _IdPensioneEsteraCumulo; } set { _IdPensioneEsteraCumulo = value; } }

            public System.Nullable<System.DateTime> DecorrenzaPrestazione { get { return _DecorrenzaPrestazione; } set { _DecorrenzaPrestazione = value; } }

            public System.Nullable<decimal> ImportoPrestazione { get { return _ImportoPrestazione; } set { _ImportoPrestazione = value; } }

            public System.Nullable<System.DateTime> CessazionePrestazione { get { return _CessazionePrestazione; } set { _CessazionePrestazione = value; } }
            #endregion public properties
        }
        #endregion nested classes
    }
}
