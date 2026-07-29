using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneRecordFondo
    {
        public static void SalvaRecordFondo(long idPensione, List<DatiRecordFondo> elencoDatiRecordFondo)
        {
            if (elencoDatiRecordFondo != null && elencoDatiRecordFondo.Count > 0)
            {
                List<RecordFondo> listaRecordFondo = new List<RecordFondo>();

                foreach (DatiRecordFondo datiRecordFondo in elencoDatiRecordFondo)
                {
                    RecordFondo recordFondo = new RecordFondo();

                    Utility.ValorizzaOggetti(datiRecordFondo, recordFondo);
                    recordFondo.IdPensione = idPensione;

                    listaRecordFondo.Add(recordFondo);
                }
                if (listaRecordFondo.Count > 0)
                    DAGestioneRecordFondo.SalvaRecordFondo(idPensione, listaRecordFondo);
                else
                    DAGestioneRecordFondo.DeleteAllRecordFondo(idPensione);
            }
            else
            {
                DAGestioneRecordFondo.DeleteAllRecordFondo(idPensione);
            }
        }

        public static void SalvaRecordFondoReturnListaAggiornata(long idPensione, List<DatiRecordFondo> elencoDatiRecordFondo, out List<DatiRecordFondo> result)
        {
            result = new List<DatiRecordFondo>();
            if (elencoDatiRecordFondo != null && elencoDatiRecordFondo.Count > 0)
            {
                List<RecordFondo> listaRecordFondo = new List<RecordFondo>();
                foreach (DatiRecordFondo datiRecordFondo in elencoDatiRecordFondo)
                {
                    RecordFondo recordFondo = new RecordFondo();

                    Utility.ValorizzaOggetti(datiRecordFondo, recordFondo);
                    recordFondo.IdPensione = idPensione;

                    listaRecordFondo.Add(recordFondo);

                }
                if (listaRecordFondo.Count > 0)
                {
                    DAGestioneRecordFondo.SalvaRecordFondo(idPensione, listaRecordFondo);
                    foreach (RecordFondo r in listaRecordFondo)
                    {
                        DatiRecordFondo datirecord = new DatiRecordFondo();
                        Utility.ValorizzaOggetti(r, datirecord);
                        result.Add(datirecord);
                    }
                }
                else
                    DAGestioneRecordFondo.DeleteAllRecordFondo(idPensione);
            }
            else
            {
                DAGestioneRecordFondo.DeleteAllRecordFondo(idPensione);
            }
        }

        public static void SalvaSingoloRecordFondo(long idPensione, DatiRecordFondo datiRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                RecordFondo recordFondo = new RecordFondo();

                Utility.ValorizzaOggetti(datiRecordFondo, recordFondo);
                recordFondo.IdPensione = idPensione;
                DAGestioneRecordFondo.SalvaSingoloRecordFondo(recordFondo);

                datiRecordFondo.Id = recordFondo.Id;

                transactionScope.Complete();
            }
        }

        public static void GetRecordFondoByIdPensione(long idPensione, out List<DatiRecordFondo> listaDatiRecordFondo)
        {
            listaDatiRecordFondo = new List<DatiRecordFondo>();

            List<RecordFondo> listaRecordFondo;
            DAGestioneRecordFondo.GetRecordFondo(idPensione, out listaRecordFondo);

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                List<PensioneFondoDZ> listaPensioneFondoDZ;
                DAGestioneRecordFondo.GetPensioneFondoDZ(listaRecordFondo, out listaPensioneFondoDZ);
                decimal? PensioneBaseAnnua = null;
                foreach (RecordFondo recordFondo in listaRecordFondo)
                {
                    if (listaPensioneFondoDZ.Any())
                    {
                        PensioneFondoDZ pensioneFondoDZ = listaPensioneFondoDZ.Where(x => x.IdRecordFondo == recordFondo.Id).FirstOrDefault();
                        PensioneBaseAnnua = pensioneFondoDZ != null ? pensioneFondoDZ.PensioneBaseAnnua : null;
                    }

                    DatiRecordFondo datiRecordFondo = new DatiRecordFondo(recordFondo.Id, recordFondo.CodiceNatura1, recordFondo.CodiceNatura2, recordFondo.CodiceNatura3, recordFondo.CodiceNonCalcolo,
                        recordFondo.DecorrenzaValiditaDati, recordFondo.DataSospensione, PensioneBaseAnnua);
                    listaDatiRecordFondo.Add(datiRecordFondo);
                }
            }
        }

        public static void GetRecordFondoByIdRecordFondo(long idRecordFondo, out DatiRecordFondo datiRecordFondo)
        {
            datiRecordFondo = new DatiRecordFondo();

            RecordFondo recordFondo;
            DAGestioneRecordFondo.GetSingoloRecordFondo(idRecordFondo, out recordFondo);
            if (recordFondo != null)
            {
                PensioneFondoDZ pensioneFondoDZ;
                DAGestioneRecordFondo.GetPensioneFondoDZ(recordFondo, out pensioneFondoDZ);
                decimal? PensioneBaseAnnua = pensioneFondoDZ != null ? pensioneFondoDZ.PensioneBaseAnnua : null;

                datiRecordFondo = new DatiRecordFondo(recordFondo.Id, recordFondo.CodiceNatura1, recordFondo.CodiceNatura2, recordFondo.CodiceNatura3, recordFondo.CodiceNonCalcolo,
                    recordFondo.DecorrenzaValiditaDati, recordFondo.DataSospensione, PensioneBaseAnnua);
            }
        }

        public static void EliminaRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRecordFondo.DeleteRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiRecordFondo
        {
            public DatiRecordFondo()
            { }

            public DatiRecordFondo(long id, System.Nullable<char> codiceNatura1, System.Nullable<char> codiceNatura2, System.Nullable<char> codiceNatura3,
                char? codiceNonCalcolo, DateTime? decorrenzaValiditaDati, System.Nullable<DateTime> dataSospensione, decimal? PensioneBaseAnnua)
            {
                _Id = id;
                _CodiceNatura1 = codiceNatura1;
                _CodiceNatura2 = codiceNatura2;
                _CodiceNatura3 = codiceNatura3;
                _CodiceNonCalcolo = codiceNonCalcolo;
                _DecorrenzaValiditaDati = decorrenzaValiditaDati;
                _DataSospensione = dataSospensione;
                _PensioneBaseAnnua = PensioneBaseAnnua;
            }

            #region private properties
            private long _Id;
            private System.Nullable<char> _CodiceNatura1;
            private System.Nullable<char> _CodiceNatura2;
            private System.Nullable<char> _CodiceNatura3;
            private char? _CodiceNonCalcolo;
            private DateTime? _DecorrenzaValiditaDati;
            private decimal? _PensioneBaseAnnua;
            private System.Nullable<DateTime> _DataSospensione;
            #endregion private properties

            #region public properties

            public long Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public System.Nullable<char> CodiceNatura1
            {
                get { return _CodiceNatura1; }
                set { _CodiceNatura1 = value; }
            }

            public System.Nullable<char> CodiceNatura2
            {
                get { return _CodiceNatura2; }
                set { _CodiceNatura2 = value; }
            }

            public System.Nullable<char> CodiceNatura3
            {
                get { return _CodiceNatura3; }
                set { _CodiceNatura3 = value; }
            }

            public char? CodiceNonCalcolo
            {
                get { return _CodiceNonCalcolo; }
                set { _CodiceNonCalcolo = value; }
            }

            public DateTime? DecorrenzaValiditaDati
            {
                get { return _DecorrenzaValiditaDati; }
                set { _DecorrenzaValiditaDati = value; }
            }

            public System.Nullable<DateTime> DataSospensione
            {
                get { return _DataSospensione; }
                set { _DataSospensione = value; }
            }

            public decimal? PensioneBaseAnnua
            {
                get { return _PensioneBaseAnnua; }
                set { _PensioneBaseAnnua = value; }
            }
            #endregion public properties
        }
        #endregion nested class
    }
}
