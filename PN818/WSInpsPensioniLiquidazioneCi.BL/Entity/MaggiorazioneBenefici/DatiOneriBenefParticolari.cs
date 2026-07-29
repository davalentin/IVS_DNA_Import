using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiOneriBenefParticolari
    {
        #region private properties

        private List<DatiOneri> _ListaDatiOneri;
        private List<DatiBeneficiParticolari> _ListaDatiBeneficiParticolari;

        #endregion

        #region public properties

        public List<DatiOneri> ListaDatiOneri { get { return _ListaDatiOneri; } set { _ListaDatiOneri = value; } }
        public List<DatiBeneficiParticolari> ListaDatiBeneficiParticolari { get { return _ListaDatiBeneficiParticolari; } set { _ListaDatiBeneficiParticolari = value; } }

        #endregion
    }




    #region nested class

    public class DatiOneri
    {
        public DatiOneri()
        { }

        public DatiOneri(long? id, long? idPensione, DateTime? decorrenza, DateTime? scadenza, long? idcodeGruppo, long? idcodeSottoGruppo, short? settimane, decimal? onere)
        {
            this._Id = id;
            this._IdPensione = idPensione;
            this._Decorrenza = decorrenza;
            this._Scadenza = scadenza;
            this._IdCodeGruppo = idcodeGruppo;
            this._IdCodeSottoGruppo = IdCodeSottoGruppo;
            this._Settimane = settimane;
            this._Onere = onere;
        }

        #region private properties

        private long? _Id;
        private long? _IdPensione;
        private DateTime? _Decorrenza;
        private DateTime? _Scadenza;
        private long? _IdCodeGruppo;
        private long? _IdCodeSottoGruppo;
        private short? _Settimane;
        private decimal? _Onere;

        #endregion

        #region public properties

        public long? Id { get { return _Id; } set { _Id = value; } }
        public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        public DateTime? Scadenza { get { return _Scadenza; } set { _Scadenza = value; } }
        public long? IdCodeGruppo { get { return _IdCodeGruppo; } set { _IdCodeGruppo = value; } }
        public long? IdCodeSottoGruppo { get { return _IdCodeSottoGruppo; } set { _IdCodeSottoGruppo = value; } }
        public short? Settimane { get { return _Settimane; } set { _Settimane = value; } }
        public decimal? Onere { get { return _Onere; } set { _Onere = value; } }

        #endregion

        public static bool IsOneriNull(DatiOneri Oneri)
        {
            if (!Oneri.Scadenza.HasValue && !Oneri.IdCodeGruppo.HasValue && !Oneri.IdCodeSottoGruppo.HasValue)
                return true;
            else
                return false;
        }
    }

    public class DatiBeneficiParticolari
    {
        public DatiBeneficiParticolari()
        { }

        public DatiBeneficiParticolari(long? id, long? idPensione, string codiceBenefici, short? settimane)
        {
            this._Id = id;
            this._IdPensione = idPensione;
            this._CodiceBenefici = codiceBenefici;
            this._Settimane = settimane;
        }

        #region private properties

        private long? _Id;
        private long? _IdPensione;
        private string _CodiceBenefici;
        private short? _Settimane;

        #endregion

        #region public properties

        public long? Id { get { return _Id; } set { _Id = value; } }
        public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        public string CodiceBenefici { get { return _CodiceBenefici; } set { _CodiceBenefici = value; } }
        public short? Settimane { get { return _Settimane; } set { _Settimane = value; } }

        #endregion

        public static bool IsBeneficiParticolariNull(DatiBeneficiParticolari beneficiParticolari)
        {
            if (String.IsNullOrEmpty(beneficiParticolari.CodiceBenefici) && !beneficiParticolari.Settimane.HasValue)
                return true;
            else
                return false;
        }
    }

    #endregion
}
