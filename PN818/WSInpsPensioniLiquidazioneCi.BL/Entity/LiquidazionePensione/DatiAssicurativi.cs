using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneCi.Entity
{
    public class DatiAssicurativi
    {
        #region private properties

        #region Pensione
        private int? _AttivitaEconomica;
        private int? _ProfessioneIndividuale;
        private bool? _RequisitiVecchiaiaAl1294;
        private bool? _RequisitiAl1294;
        private bool? _RequisitiAl996;
        private DateTime? _InizioAssicurazione;
        private DateTime? _FineAssicurazione;

        #endregion Pensione

        #region PensioneCiDatiGenerici

        private int? _AnniDifferimento;
        private char? _CodiceVirtuale;
        private DateTime? _DecorrenzaCodiceVirtuale;
        private bool? _DeliberaCee126;
        private decimal? _ImportoCristallizzazione3481;
        private bool? _CodiceBloccoArretratiEE;
        private string _UfficioPagatoreArretratiEsteri;
        private string _CodiciMotivazioniCi281;
        private char? _CodiciCi21;
        //nuovi campi
        private decimal? _RMS8888;
        private decimal? _RMS9090;
        private int? _VVMisuraAl1292;
        private int? _VVMisuraDL50392;
        private int? _SettimanePerCalcoloContributivo;
        private int? _SettimaneItalianeMisura;
        private int? _SettimaneItalianeDiritto;
        private decimal? _ImportoIVS;
        private int? _NSettFittiziePrepensionamento;
        private int? _NContributiItalia;
        private int? _SettimaneOBGMisura12_92;
        private int? _SettimaneOBGMisuraDL503_92;
        //ENG - Gestione Nuovo Codice CI28
        private char? _CodiceCI28;

        #endregion PensioneCiDatiGenerici

        #region PensioniCiPrestazioniEE
        private byte? _CodiceConvenzione;
        #endregion PensioniCiPrestazioniEE

        #region Istruttoria

        private int? _NSettimaneOBG;
        private int? _NContributiVolontari;
        private int? _NSettGodimentoAssegno;
        private byte? _CodiceRequisitiParticolari;

        #endregion Istruttoria

        #region Integrazione Art.11

        private decimal? _ImportoIVS_Art11;

        #endregion Integrazione Art.11

        #endregion private properties

        #region public properties

        #region Pensione
        public int? AttivitaEconomica { get { return _AttivitaEconomica; } set { _AttivitaEconomica = value; } }
        public int? ProfessioneIndividuale { get { return _ProfessioneIndividuale; } set { _ProfessioneIndividuale = value; } }
        public bool? RequisitiVecchiaiaAl1294 { get { return _RequisitiVecchiaiaAl1294; } set { _RequisitiVecchiaiaAl1294 = value; } }
        public bool? RequisitiAl1294 { get { return _RequisitiAl1294; } set { _RequisitiAl1294 = value; } }
        public bool? RequisitiAl996 { get { return _RequisitiAl996; } set { _RequisitiAl996 = value; } }
        public DateTime? InizioAssicurazione { get { return _InizioAssicurazione; } set { _InizioAssicurazione = value; } }
        public DateTime? FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }
        #endregion Pensione

        #region PensioneCiDatiGenerici

        public int? AnniDifferimento { get { return _AnniDifferimento; } set { _AnniDifferimento = value; } }
        public char? CodiceVirtuale { get { return _CodiceVirtuale; } set { _CodiceVirtuale = value; } }
        public DateTime? DecorrenzaCodiceVirtuale { get { return _DecorrenzaCodiceVirtuale; } set { _DecorrenzaCodiceVirtuale = value; } }
        public bool? DeliberaCee126 { get { return _DeliberaCee126; } set { _DeliberaCee126 = value; } }
        public decimal? ImportoCristallizzazione3481 { get { return _ImportoCristallizzazione3481; } set { _ImportoCristallizzazione3481 = value; } }
        public bool? CodiceBloccoArretratiEE { get { return _CodiceBloccoArretratiEE; } set { _CodiceBloccoArretratiEE = value; } }
        public string UfficioPagatoreArretratiEsteri { get { return _UfficioPagatoreArretratiEsteri; } set { _UfficioPagatoreArretratiEsteri = value; } }
        public string CodiciMotivazioniCi281 { get { return _CodiciMotivazioniCi281; } set { _CodiciMotivazioniCi281 = value; } }
        public char? CodiciCi21 { get { return _CodiciCi21; } set { _CodiciCi21 = value; } }
        //nuovi campi
        public decimal? RMS8888 { get { return _RMS8888; } set { _RMS8888 = value; } }
        public decimal? RMS9090 { get { return _RMS9090; } set { _RMS9090 = value; } }
        public int? VVMisuraAl1292 { get { return _VVMisuraAl1292; } set { _VVMisuraAl1292 = value; } }
        public int? VVMisuraDL50392 { get { return _VVMisuraDL50392; } set { _VVMisuraDL50392 = value; } }
        public int? SettimanePerCalcoloContributivo { get { return _SettimanePerCalcoloContributivo; } set { _SettimanePerCalcoloContributivo = value; } }
        public int? SettimaneItalianeMisura { get { return _SettimaneItalianeMisura; } set { _SettimaneItalianeMisura = value; } }
        public int? SettimaneItalianeDiritto { get { return _SettimaneItalianeDiritto; } set { _SettimaneItalianeDiritto = value; } }
        public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
        public int? NContributiItalia { get { return _NContributiItalia; } set { _NContributiItalia = value; } }
        public int? NSettFittiziePrepensionamento { get { return _NSettFittiziePrepensionamento; } set { _NSettFittiziePrepensionamento = value; } }
        public int? SettimaneOBGMisura12_92 { get { return _SettimaneOBGMisura12_92; } set { _SettimaneOBGMisura12_92 = value; } }
        public int? SettimaneOBGMisuraDL503_92 { get { return _SettimaneOBGMisuraDL503_92; } set { _SettimaneOBGMisuraDL503_92 = value; } }
        //ENG - Gestione Nuovo Codice CI28
        public char? CodiceCI28 { get { return _CodiceCI28; } set { _CodiceCI28 = value; } }

        #endregion PensioneCiDatiGenerici

        #region PensioniCiPrestazioniEE
        public byte? CodiceConvenzione { get { return _CodiceConvenzione; } set { _CodiceConvenzione = value; } }
        #endregion PensioniCiPrestazioniEE

        #region Istruttoria

        public int? NSettimaneOBG { get { return _NSettimaneOBG; } set { _NSettimaneOBG = value; } }
        public int? NContributiVolontari { get { return _NContributiVolontari; } set { _NContributiVolontari = value; } }
        public int? NSettGodimentoAssegno { get { return _NSettGodimentoAssegno; } set { _NSettGodimentoAssegno = value; } }
        public byte? CodiceRequisitiParticolari { get { return _CodiceRequisitiParticolari; } set { _CodiceRequisitiParticolari = value; } }

        #endregion Istruttoria

        #region Integrazione Art.11

        public decimal? ImportoIVS_Art11 { get { return _ImportoIVS_Art11; } set { _ImportoIVS_Art11 = value; } }

        #endregion Integrazione Art.11

        #endregion public properties

        public bool IsDatiAssicurativiPensioneNull()
        {
            if (!this._AttivitaEconomica.HasValue && !this._ProfessioneIndividuale.HasValue &&
                !this._RequisitiVecchiaiaAl1294.HasValue && !this._RequisitiAl1294.HasValue &&
                !this._RequisitiAl996.HasValue && !this._InizioAssicurazione.HasValue && 
                !this._FineAssicurazione.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiAssicurativiPensioneCiGenericiNull()
        {
            if (!this._AnniDifferimento.HasValue && !this._CodiceVirtuale.HasValue &&
                !this._DecorrenzaCodiceVirtuale.HasValue && !this._DeliberaCee126.HasValue &&
                !this._ImportoCristallizzazione3481.HasValue && !this._CodiceBloccoArretratiEE.HasValue &&
                String.IsNullOrEmpty(this._UfficioPagatoreArretratiEsteri) && String.IsNullOrEmpty(this._CodiciMotivazioniCi281) &&
                !this._CodiciCi21.HasValue && !this._RMS8888.HasValue && !this._RMS9090.HasValue && !this._VVMisuraAl1292.HasValue &&
                !this._VVMisuraDL50392.HasValue && !this._SettimanePerCalcoloContributivo.HasValue && !this._SettimaneItalianeMisura.HasValue &&
                !this._SettimaneItalianeDiritto.HasValue && !this._ImportoIVS.HasValue && !this._NContributiItalia.HasValue && !this._NSettFittiziePrepensionamento.HasValue &&
                !this._SettimaneOBGMisura12_92.HasValue && !this._SettimaneOBGMisuraDL503_92.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiAssicurativiIstruttoriaNull()
        {
            if (!this._NSettimaneOBG.HasValue && !this._NContributiVolontari.HasValue && !this._NSettGodimentoAssegno.HasValue && !this._CodiceRequisitiParticolari.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiAssicurativiIntegrazioneArt11Null()
        {
            if (!this._ImportoIVS_Art11.HasValue)
                return true;
            else
                return false;
        }
    }
}


