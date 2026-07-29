using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiProvenienza
    {
        #region private properties

        #region istruttoria
        private Int16? _CodiceP18PrecedentePensione;
        private Int16? _SedePrecedentePensione;
        private int? _CertificatoPrecedentePensione;
        private DateTime? _DecorrenzaOriginariaAltraPensione;
        private DateTime? _DecorrenzaCaricoPrecedentePensione;
        #endregion istruttoria

        #endregion private properties

        #region public properties

        #region istruttoria
        public Int16? CodiceP18PrecedentePensione { get { return _CodiceP18PrecedentePensione; } set { _CodiceP18PrecedentePensione = value; } }
        public Int16? SedePrecedentePensione { get { return _SedePrecedentePensione; } set { _SedePrecedentePensione = value; } }
        public int? CertificatoPrecedentePensione { get { return _CertificatoPrecedentePensione; } set { _CertificatoPrecedentePensione = value; } }
        public DateTime? DecorrenzaOriginariaAltraPensione { get { return _DecorrenzaOriginariaAltraPensione; } set { _DecorrenzaOriginariaAltraPensione = value; } }
        public DateTime? DecorrenzaCaricoPrecedentePensione { get { return _DecorrenzaCaricoPrecedentePensione; } set { _DecorrenzaCaricoPrecedentePensione = value; } }

        #endregion istruttoria

        #endregion public properties

        public bool IsDatiProvenienzaIstruttoriaNull()
        {
            if (!this._CodiceP18PrecedentePensione.HasValue && !this._SedePrecedentePensione.HasValue && !this._CertificatoPrecedentePensione.HasValue &&
                !this._DecorrenzaOriginariaAltraPensione.HasValue && !this._DecorrenzaCaricoPrecedentePensione.HasValue)
                return true;
            else
                return false;
        }
    }
}
