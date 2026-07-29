using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazioneAgo.Entity
{
    public class DatiAssicurativi
    {
        #region private properties

        private ENPALS _DatiENPALS;

        #region Pensione
        private int? _AttivitaEconomica;
        private int? _ProfessioneIndividuale;
        private int? _AttivitaEconomicaFELPE;
        private int? _ProfessioneIndividualeFELPE;
        private bool? _RequisitiVecchiaiaAl1294;
        private bool? _RequisitiAl1294;
        private bool? _RequisitiAl996;
        private DateTime? _InizioAssicurazione;
        private DateTime? _FineAssicurazione;
        #endregion Pensione

        #region Istruttoria
        private int? _NSettimaneOBG;
        private int? _NSettimaneOI;
        private int? _NContributiVolontari;
        private int? _NContributiVVAnzianita;
        #endregion Istruttoria

        #region DatiControlloFelpe
        private DateTime? _InizioBonus;
        private DateTime? _FineBonus;
        #endregion DatiControlloFelpe

        #region PensioniDatiGenerici
        private DateTime? _InizioUltimoLavoro;
        private DateTime? _FineUltimoLavoro;
        private Decimal? _ImportoUltimaRetribuzione;
        private short? _ReqArt2DL503;
        private byte? _CodiceConvenzioneAgo;
        private short? _TotaleSettimaneEstereUtiliPerDiritto;
        private int? _ContribuzioneEsteraTotale;
        #endregion PensioniDatiGenerici

        #region PensioniCiPrestazioniEE
        private byte? _CodiceConvenzione;
        #endregion PensioniCiPrestazioniEE

        #endregion private properties

        #region public properties

        public ENPALS DatiENPALS { get { return _DatiENPALS; } set { _DatiENPALS = value; } }

        #region Pensione
        public int? AttivitaEconomica { get { return _AttivitaEconomica; } set { _AttivitaEconomica = value; } }
        public int? ProfessioneIndividuale { get { return _ProfessioneIndividuale; } set { _ProfessioneIndividuale = value; } }
        public int? AttivitaEconomicaFELPE { get { return _AttivitaEconomicaFELPE; } set { _AttivitaEconomicaFELPE = value; } }
        public int? ProfessioneIndividualeFELPE { get { return _ProfessioneIndividualeFELPE; } set { _ProfessioneIndividualeFELPE = value; } }
        public bool? RequisitiVecchiaiaAl1294 { get { return _RequisitiVecchiaiaAl1294; } set { _RequisitiVecchiaiaAl1294 = value; } }
        public bool? RequisitiAl1294 { get { return _RequisitiAl1294; } set { _RequisitiAl1294 = value; } }
        public bool? RequisitiAl996 { get { return _RequisitiAl996; } set { _RequisitiAl996 = value; } }
        //public string AttivitaSvolta { get { return _AttivitaSvolta; } set { _AttivitaSvolta = value; } }
        public DateTime? InizioAssicurazione { get { return _InizioAssicurazione; } set { _InizioAssicurazione = value; } }
        public DateTime? FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }
        #endregion Pensione

        #region Istruttoria
        public int? NSettimaneOBG { get { return _NSettimaneOBG; } set { _NSettimaneOBG = value; } }
        public int? NSettimaneOI { get { return _NSettimaneOI; } set { _NSettimaneOI = value; } }
        public int? NContributiVolontari { get { return _NContributiVolontari; } set { _NContributiVolontari = value; } }
        public int? NContributiVVAnzianita { get { return _NContributiVVAnzianita; } set { _NContributiVVAnzianita = value; } }
        #endregion Istruttoria

        #region DatiControlloFelpe
        public DateTime? InizioBonus { get { return _InizioBonus; } set { _InizioBonus = value; } }
        public DateTime? FineBonus { get { return _FineBonus; } set { _FineBonus = value; } }
        #endregion DatiControlloFelpe

        #region PensioniDatiGenerici
        public DateTime? InizioUltimoLavoro { get { return _InizioUltimoLavoro; } set { _InizioUltimoLavoro = value; } }
        public DateTime? FineUltimoLavoro { get { return _FineUltimoLavoro; } set { _FineUltimoLavoro = value; } }
        public Decimal? ImportoUltimaRetribuzione { get { return _ImportoUltimaRetribuzione; } set { _ImportoUltimaRetribuzione = value; } }
        public short? ReqArt2DL503 { get { return _ReqArt2DL503; } set { _ReqArt2DL503 = value; } }
        public byte? CodiceConvenzioneAgo { get { return _CodiceConvenzioneAgo; } set { _CodiceConvenzioneAgo = value; } }
        public short? TotaleSettimaneEstereUtiliPerDiritto { get { return _TotaleSettimaneEstereUtiliPerDiritto; } set { _TotaleSettimaneEstereUtiliPerDiritto = value; } }
        public int? ContribuzioneEsteraTotale { get { return _ContribuzioneEsteraTotale; } set { _ContribuzioneEsteraTotale = value; } }
        #endregion PensioniDatiGenerici

        #region PensioniCiPrestazioniEE
        public byte? CodiceConvenzione { get { return _CodiceConvenzione; } set { _CodiceConvenzione = value; } }
        #endregion PensioniCiPrestazioniEE

        #endregion public properties

        public bool IsDatiAssicurativiPensioneNull()
        {
            if (!this._AttivitaEconomica.HasValue && !this._ProfessioneIndividuale.HasValue &&
                !this._RequisitiVecchiaiaAl1294.HasValue && !this._RequisitiAl1294.HasValue &&
                !this._RequisitiAl996.HasValue /*&& String.IsNullOrEmpty(this._AttivitaSvolta) */&&
                !this._InizioAssicurazione.HasValue && !this._FineAssicurazione.HasValue &&
                !this._ReqArt2DL503.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiAssicurativiIstruttoriaNull()
        {
            if (!this._NSettimaneOBG.HasValue && !this._NContributiVolontari.HasValue && !this._NContributiVVAnzianita.HasValue)
                return true;
            else
                return false;
        }

        public bool IsDatiAssicurativiControlloFelpeNull()
        {
            if (!this._InizioBonus.HasValue && !this._FineBonus.HasValue)
                return true;
            else
                return false;
        }
        internal bool IsDatiAssicurativiPensioneDatiGenericiNull()
        {
            if (!this._ImportoUltimaRetribuzione.HasValue && !this._InizioUltimoLavoro.HasValue && !this._FineUltimoLavoro.HasValue)
                return true;
            return false;
        }

        #region Nested Class

        public class ENPALS
        {
            #region Private properties

            private short? _AADiritto;
            private short? _MMDiritto;
            private char? _RaggruppamentoPrevalente;
            private char? _GruppoPrevalente;
            private char? _GruppoDiritto;
            private int? _NTotContributi;
            private int? _NTotContributiEnpals;
            private short? _EtaDirittoAA;
            private short? _EtaDirittoMM;
            private short? _EtaMisuraAA;
            private short? _EtaMisuraMM;
            private string _Qualifica;
            private DateTime? _DataFinestra;
            private int? _NContributiMisura;
            private int? _NTotDiritto;
            private int? _NTotQualifica;
            private int? _NContributiQuinquennio;
            private int? _NContributiTriennio;
            private int? _NContributiNL222;
            private int? _NContributiNL155;
            private short? _AnzianitaContributiva;

            #endregion Private properties

            #region Public properties

            public short? AADiritto { get { return _AADiritto; } set { _AADiritto = value; } }
            public short? MMDiritto { get { return _MMDiritto; } set { _MMDiritto = value; } }
            public char? RaggruppamentoPrevalente { get { return _RaggruppamentoPrevalente; } set { _RaggruppamentoPrevalente = value; } }
            public char? GruppoPrevalente { get { return _GruppoPrevalente; } set { _GruppoPrevalente = value; } }
            public char? GruppoDiritto { get { return _GruppoDiritto; } set { _GruppoDiritto = value; } }
            public int? NTotContributi { get { return _NTotContributi; } set { _NTotContributi = value; } }
            public int? NTotContributiEnpals { get { return _NTotContributiEnpals; } set { _NTotContributiEnpals = value; } }
            public short? EtaDirittoAA { get { return _EtaDirittoAA; } set { _EtaDirittoAA = value; } }
            public short? EtaDirittoMM { get { return _EtaDirittoMM; } set { _EtaDirittoMM = value; } }
            public short? EtaMisuraAA { get { return _EtaMisuraAA; } set { _EtaMisuraAA = value; } }
            public short? EtaMisuraMM { get { return _EtaMisuraMM; } set { _EtaMisuraMM = value; } }
            public string Qualifica { get { return _Qualifica; } set { _Qualifica = value; } }
            public DateTime? DataFinestra { get { return _DataFinestra; } set { _DataFinestra = value; } }
            public int? NContributiMisura { get { return _NContributiMisura; } set { _NContributiMisura = value; } }
            public int? NTotDiritto { get { return _NTotDiritto; } set { _NTotDiritto = value; } }
            public int? NTotQualifica { get { return _NTotQualifica; } set { _NTotQualifica = value; } }
            public int? NContributiQuinquennio { get { return _NContributiQuinquennio; } set { _NContributiQuinquennio = value; } }
            public int? NContributiTriennio { get { return _NContributiTriennio; } set { _NContributiTriennio = value; } }
            public int? NContributiNL222 { get { return _NContributiNL222; } set { _NContributiNL222 = value; } }
            public int? NContributiNL155 { get { return _NContributiNL155; } set { _NContributiNL155 = value; } }
            public short? AnzianitaContributiva { get { return _AnzianitaContributiva; } set { _AnzianitaContributiva = value; } }

            #endregion Public properties

            public bool IsDatiAssicurativiEnpalsNull()
            {
                if (!this._AADiritto.HasValue && !this._MMDiritto.HasValue && !this._RaggruppamentoPrevalente.HasValue && !this._GruppoPrevalente.HasValue && !this._GruppoDiritto.HasValue &&
                    !this._NTotContributi.HasValue && !this._NTotContributiEnpals.HasValue && !this._EtaDirittoAA.HasValue && !this._EtaDirittoMM.HasValue &&
                    !this._EtaMisuraAA.HasValue && !this._EtaMisuraMM.HasValue && string.IsNullOrEmpty(this._Qualifica) && !this._DataFinestra.HasValue &&
                    !this._NContributiMisura.HasValue && !this._NTotDiritto.HasValue && !this._NTotQualifica.HasValue && !this._NContributiQuinquennio.HasValue &&
                    !this._NContributiTriennio.HasValue && !this._NContributiNL222.HasValue && !this._NContributiNL155.HasValue && !this._AnzianitaContributiva.HasValue)
                    return true;
                else
                    return false;
            }
        }

        #endregion Nested Class


    }
}


