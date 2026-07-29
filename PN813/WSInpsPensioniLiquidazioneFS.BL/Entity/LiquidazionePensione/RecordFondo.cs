using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    [System.Serializable()]
    public class RecordFondo
    {
        public RecordFondo() { }

        public RecordFondo(long id, System.Nullable<char> codiceNatura1, System.Nullable<char> codiceNatura2,
            System.Nullable<char> codiceNatura3, char? codiceNonCalcolo, DateTime? decorrenzaValiditaDati,
            System.Nullable<DateTime> dataSospensione, bool isFromDB)
        {
            _Id = id;
            _CodiceNatura1 = codiceNatura1;
            _CodiceNatura2 = codiceNatura2;
            _CodiceNatura3 = codiceNatura3;
            _CodiceNonCalcolo = codiceNonCalcolo;
            _DecorrenzaValiditaDati = decorrenzaValiditaDati;
            _DataSospensione = dataSospensione;
            _IsFromDB = isFromDB;
        }

        private long _Id;
        private System.Nullable<char> _CodiceNatura1;
        private System.Nullable<char> _CodiceNatura2;
        private System.Nullable<char> _CodiceNatura3;
        private char? _CodiceNonCalcolo;
        private DateTime? _DecorrenzaValiditaDati;
        private System.Nullable<DateTime> _DataSospensione;
        private bool _IsFromDB;

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

        public bool IsFromDB
        {
            get { return _IsFromDB; }
            set { _IsFromDB = value; }
        }

        #region public members
        public override bool Equals(object obj)
        {
            RecordFondo recordFondo = (RecordFondo)obj;
            try
            {
                if (this._CodiceNatura1 != recordFondo._CodiceNatura1 ||
                     this._CodiceNatura2 != recordFondo._CodiceNatura2 ||
                     this._CodiceNatura3 != recordFondo._CodiceNatura3 ||
                     this._CodiceNonCalcolo != recordFondo._CodiceNonCalcolo ||
                     this._DecorrenzaValiditaDati != recordFondo._DecorrenzaValiditaDati ||
                     this._DataSospensione != recordFondo._DataSospensione)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool EqualsExceptDecorrenza(object obj)
        {
            RecordFondo recordFondo = (RecordFondo)obj;
            try
            {
                if (this._CodiceNatura1 != recordFondo._CodiceNatura1 ||
                     this._CodiceNatura2 != recordFondo._CodiceNatura2 ||
                     this._CodiceNatura3 != recordFondo._CodiceNatura3 ||
                     this._CodiceNonCalcolo != recordFondo._CodiceNonCalcolo ||
                     this._DataSospensione != recordFondo._DataSospensione)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool EqualsExceptDecorrenzaNonCalcolo(object obj)
        {
            RecordFondo recordFondo = (RecordFondo)obj;
            try
            {
                if (this._CodiceNatura1 != recordFondo._CodiceNatura1 ||
                     this._CodiceNatura2 != recordFondo._CodiceNatura2 ||
                     this._CodiceNatura3 != recordFondo._CodiceNatura3 ||
                     this._DataSospensione != recordFondo._DataSospensione)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        #endregion public members
    }
}

