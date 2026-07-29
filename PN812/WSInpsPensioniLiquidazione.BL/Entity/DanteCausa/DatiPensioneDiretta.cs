using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class DatiPensioneDiretta
    {
        private byte? _CodiceTipoPensione;
        private byte? _CodiceBeneficiLegge;
        private decimal? _ImportoPensione311284;
        private decimal? _ImportoPensione1185;
        private decimal? _ImportoPensione1190;
        private int? _NContributiDiretta;
        private byte? _Maggiorazione781Contributi;
        private string _NaturaPensione;
        private byte? _CodiceEliminazione;
        private DateTime? _DecorrenzaPensione;
        private DateTime? _DecorrenzaEliminazione;
        private DateTime? _DecorrenzaEliminazioneContabile;
        private string _SiglaCategoria;
        private string _Sede;
        private int? _Certificato;
        

        public byte? Maggiorazione781Contributi { get { return _Maggiorazione781Contributi; } set { _Maggiorazione781Contributi = value; } }
        public byte? CodiceTipoPensione { get { return _CodiceTipoPensione; } set { _CodiceTipoPensione = value; } }
        public byte? CodiceBeneficiLegge { get { return _CodiceBeneficiLegge; } set { _CodiceBeneficiLegge = value; } }
        public string NaturaPensione { get { return _NaturaPensione; } set { _NaturaPensione = value; } }
        public decimal? ImportoPensione311284 { get { return _ImportoPensione311284; } set { _ImportoPensione311284 = value; } }
        public decimal? ImportoPensione1185 { get { return _ImportoPensione1185; } set { _ImportoPensione1185 = value; } }
        public decimal? ImportoPensione1190 { get { return _ImportoPensione1190; } set { _ImportoPensione1190 = value; } }
        public int? NContributiDiretta { get { return _NContributiDiretta; } set { _NContributiDiretta = value; } }
        public byte? CodiceEliminazione { get { return _CodiceEliminazione; } set { _CodiceEliminazione = value; } }
        public DateTime? DecorrenzaPensione { get { return _DecorrenzaPensione; } set { _DecorrenzaPensione = value; } }
        public DateTime? DecorrenzaEliminazione { get { return _DecorrenzaEliminazione; } set { _DecorrenzaEliminazione = value; } }
        public DateTime? DecorrenzaEliminazioneContabile { get { return _DecorrenzaEliminazioneContabile; } set { _DecorrenzaEliminazioneContabile = value; } }
        public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
        public string Sede { get { return _Sede; } set { _Sede = value; } }
        public int? Certificato { get { return _Certificato; } set { _Certificato = value; } }
       
        public bool IsDatiPensioneDirettaNull()
        {
            if (this._CodiceTipoPensione == null && this._CodiceBeneficiLegge == null && this._ImportoPensione311284 == null && this._ImportoPensione1185 == null &&
                this._ImportoPensione1190 == null && this._NContributiDiretta == null && this._Maggiorazione781Contributi == null &&
                this._NaturaPensione == null && this._CodiceEliminazione == null && this._DecorrenzaEliminazione == null && this._DecorrenzaPensione == null &&
                this._DecorrenzaEliminazioneContabile == null && this._SiglaCategoria == null &&
                this._Sede == null && this._Certificato == null
                )
                return true;
            else
                return false;
        }

    }
}
