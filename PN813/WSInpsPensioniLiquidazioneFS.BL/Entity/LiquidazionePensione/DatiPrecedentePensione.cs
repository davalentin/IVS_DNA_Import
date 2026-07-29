
namespace INPS.Pensioni.LiquidazioneFs.Entity
{
    public class DatiPrecedentePensione
    {
        #region private properties
        #region istruttoria
        private System.Nullable<short> _CodiceP18PrecedentePensione;
        private System.Nullable<short> _SedePrecedentePensione;
        private System.Nullable<int> _CertificatoPrecedentePensione;
        #endregion istruttoria
        #endregion private properties

        #region public properties
        #region istruttoria
        public System.Nullable<short> CodiceP18PrecedentePensione { get { return _CodiceP18PrecedentePensione; } set { _CodiceP18PrecedentePensione = value; } }
        public System.Nullable<short> SedePrecedentePensione { get { return _SedePrecedentePensione; } set { _SedePrecedentePensione = value; } }
        public System.Nullable<int> CertificatoPrecedentePensione { get { return _CertificatoPrecedentePensione; } set { _CertificatoPrecedentePensione = value; } }
        public bool IsIstruttoriaNull()
        {
            if (this._CodiceP18PrecedentePensione == null && this._SedePrecedentePensione == null && this._CertificatoPrecedentePensione == null)
                return true;
            else
                return false;
        }

        public bool IsDatiPrecedentePensioneNull()
        {
            if (this._CodiceP18PrecedentePensione == null && this._SedePrecedentePensione == null && this._CertificatoPrecedentePensione == null)
                return true;
            else
                return false;
        }
        #endregion istruttoria
        #endregion public properties
    }
}
