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
    public class GestioneMaggiorazioniBenefici
    {
        public static void SalvaMaggiorazioniBenefici(DatiMaggiorazioniBenefici maggiorazioniBenefici)
        {
            MaggiorazioniBenefici maggiorazionibenefici = new MaggiorazioniBenefici();
            Utility.ValorizzaOggetti(maggiorazioniBenefici, maggiorazionibenefici);
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneMaggiorazioniBenefici.SalvaMaggiorazioniBenefici(maggiorazionibenefici);                
                transactionScope.Complete();
            }
        }

        public static void GetMaggiorazioniBeneficiByIdPensione(long idPensione, out DatiMaggiorazioniBenefici datiMaggiorazioniBenefici)
        {
            MaggiorazioniBenefici maggiorazioniBenefici = null;
            datiMaggiorazioniBenefici = null;

            DAGestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(idPensione, out maggiorazioniBenefici);
            
            if (maggiorazioniBenefici == null)
                return;
            datiMaggiorazioniBenefici = new DatiMaggiorazioniBenefici();
            Utility.ValorizzaOggetti(maggiorazioniBenefici, datiMaggiorazioniBenefici);
        }

        public static void EliminaMaggiorazioniBeneficiByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneMaggiorazioniBenefici.EliminaMaggiorazioniBeneficiByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static bool IsMaggiorazioniBeneficiNull(DatiMaggiorazioniBenefici maggiorazioniBenefici)
        {
            if (maggiorazioniBenefici == null)
                return true;

            if (!maggiorazioniBenefici.AnniRiduzioneBeneficiArt38Legge02.HasValue &&
                !maggiorazioniBenefici.Articolo1Legge5991.HasValue &&
                !maggiorazioniBenefici.Attivitausuranti.HasValue &&
                !maggiorazioniBenefici.Aumento7290.HasValue &&
                !maggiorazioniBenefici.Aumento7290DC.HasValue &&
                !maggiorazioniBenefici.Aumento780ContributiArt1.HasValue &&
                !maggiorazioniBenefici.Aumento780ContributiArt4.HasValue &&
                !maggiorazioniBenefici.AumentoArt3.HasValue &&
                !maggiorazioniBenefici.AumentoArt5.HasValue &&
                !maggiorazioniBenefici.AumentoMensileLegge161289Art2.HasValue &&
                !maggiorazioniBenefici.AumentoMensileLegge5991Comma2.HasValue &&
                !maggiorazioniBenefici.AumentoMensileLegge5991Comma9.HasValue &&
                !maggiorazioniBenefici.Cessazione.HasValue &&
                !maggiorazioniBenefici.CessazioneMaggiorazioneSociale.HasValue &&
                String.IsNullOrEmpty(maggiorazioniBenefici.CodiceBenefici) &&
                !maggiorazioniBenefici.CodiceCieco.HasValue &&
                String.IsNullOrEmpty(maggiorazioniBenefici.CodiceInvalidita80Percento) &&
                !maggiorazioniBenefici.CodiceLeggeGruppo.HasValue &&
                !maggiorazioniBenefici.CodiceLeggeSottogruppo.HasValue &&
                !maggiorazioniBenefici.CodiceRequisitiLegge50392Art2.HasValue &&
                !maggiorazioniBenefici.DecorrenzaInv80.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneArt6.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneLegge140.HasValue &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneLegge544.HasValue &&
                !maggiorazioniBenefici.DecorrenzaVariazione.HasValue &&
                !maggiorazioniBenefici.ExInpdai.HasValue &&
                !maggiorazioniBenefici.ExInpdaiArt10.HasValue &&
                !maggiorazioniBenefici.ExInpdaiArt3.HasValue &&
                !maggiorazioniBenefici.ExInpdaiArt4.HasValue &&
                !maggiorazioniBenefici.ImportoArt6.HasValue &&
                !maggiorazioniBenefici.ImportoAumento780Contributi.HasValue &&
                !maggiorazioniBenefici.ImportoBeneficiCombattente.HasValue &&
                !maggiorazioniBenefici.ImportoComplessivoArt3.HasValue &&
                !maggiorazioniBenefici.ImportoComplessivoArt4.HasValue &&
                !maggiorazioniBenefici.ImportoComplessivoArt5.HasValue &&
                !maggiorazioniBenefici.ImportoComplessivoArt1.HasValue &&
                !maggiorazioniBenefici.MaggioreAnzianitaConcessa.HasValue &&
                !maggiorazioniBenefici.MancataContribuzione.HasValue &&
                !maggiorazioniBenefici.MensileLegge5991.HasValue &&
                !maggiorazioniBenefici.NSettimaneBeneficio.HasValue &&
                !maggiorazioniBenefici.NSettimaneIncremento05Percento.HasValue &&
                !maggiorazioniBenefici.NSettimaneIncremento1Percento.HasValue &&
                !maggiorazioniBenefici.NSettIncrementoPrepensionamento.HasValue &&
                !maggiorazioniBenefici.Sentenza495240.HasValue &&
                !maggiorazioniBenefici.SettimaneBenefici.HasValue &&
                String.IsNullOrEmpty(maggiorazioniBenefici.TipoSettimaneBeneficio) &&
                !maggiorazioniBenefici.DecorrenzaMaggiorazioneSociale.HasValue &&
                !maggiorazioniBenefici.DirittoScattiLegge336.HasValue &&
                !maggiorazioniBenefici.SettimaneBeneficioAA.HasValue &&
                !maggiorazioniBenefici.SettimaneBeneficioMM.HasValue &&
                !maggiorazioniBenefici.SettimaneBeneficioGG.HasValue &&
                !maggiorazioniBenefici.RMSSenzaLegge33670QA.HasValue &&
                !maggiorazioniBenefici.Articolo6140.HasValue )
            {
                return true;
            }
            else
                return false;
        }

        #region nested class

        public class DatiMaggiorazioniBenefici
        {
            public DatiMaggiorazioniBenefici()
            { }
            public DatiMaggiorazioniBenefici(long id, long idPensione, int? nSettimaneBeneficio, string tipoSettimaneBeneficio, int? nSettimaneIncremento1Percento,
                                             int? nSettimaneIncremento05Percento, bool? attivitausuranti, byte? codiceCieco, DateTime? decorrenzaMaggiorazioneArt6,
                                             decimal? importoArt6, bool? aumento780ContributiArt4, bool? aumento780ContributiArt1, bool? exInpdai, decimal? importoAumento780Contributi,
                                             decimal? importoComplessivoArt4, bool? aumentoArt5, bool? exInpdaiArt4, decimal? importoComplessivoArt5, bool? aumentoArt3, bool? exInpdaiArt3,
                                             decimal? importoComplessivoArt3, bool? exInpdaiArt10, DateTime? decorrenzaMaggiorazioneLegge140,
                                             DateTime? decorrenzaMaggiorazioneLegge544, decimal? aumentoMensileLegge161289Art2, short? anniRiduzioneBeneficiArt38Legge02,
                                             decimal? aumento7290, decimal? aumento7290DC, decimal? aumentoMensileLegge5991Comma9, decimal? aumentoMensileLegge5991Comma2, decimal? importoBeneficiCombattente,
                                             byte? sentenza495240, DateTime? decorrenzaVariazione, DateTime? cessazione, short? codiceLeggeGruppo, short? codiceLeggeSottogruppo,
                                             short? maggioreAnzianitaConcessa, decimal? mancataContribuzione, string codiceBenefici, short? settimaneBenefici, string codiceInvalidita80Percento, DateTime? cessazioneMaggiorazioneSociale,
                                             byte? codiceRequisitiLegge50392Art2, DateTime? decorrenzaInv80, int? nSettIncrementoPrepensionamento, bool articolo1Legge5991, decimal mensileLegge5991,
                                             long? exCombattente, decimal _RMSSenzaLegge33670QA, decimal _RMSSenzaLegge33670QB, byte? percentualeMaggiorazioneSenzaLegge33670, DateTime? decorrenzaMaggiorazioneSociale,
                                             int? dirittoScattiLegge336, short? settimaneBeneficioAA, short? settimaneBeneficioMM, short? settimaneBeneficioGG, bool? isBeneficioArt24Comma15BisFromFELPE,
                                             int? maggiorazioneAmianto, int? maggiorazioneInv74, bool? isBeneficioApePrecociFromFELPE, short? settAnzContribPost311295, DateTime? dataNonvedenteDal, int? nSettIntegrazioneContributivaConcessa)
            {
                this._Id = id;
                this._IdPensione = idPensione;
                this._NSettimaneBeneficio = nSettimaneBeneficio;
                this._TipoSettimaneBeneficio = tipoSettimaneBeneficio;
                this._NSettimaneIncremento1Percento = nSettimaneIncremento1Percento;
                this._NSettimaneIncremento05Percento = nSettimaneIncremento05Percento;
                this._Attivitausuranti = attivitausuranti;
                this._CodiceCieco = codiceCieco;
                this._DecorrenzaMaggiorazioneArt6 = decorrenzaMaggiorazioneArt6;
                this._ImportoArt6 = importoArt6;
                this._Aumento780ContributiArt4 = aumento780ContributiArt4;
                this._Aumento780ContributiArt1 = aumento780ContributiArt1;
                this._ExInpdai = exInpdai;
                this._ImportoAumento780Contributi = importoAumento780Contributi;
                this._ImportoComplessivoArt4 = importoComplessivoArt4;
                this._AumentoArt5 = aumentoArt5;
                this._ExInpdaiArt4 = exInpdaiArt4;
                this._ImportoComplessivoArt5 = importoComplessivoArt5;
                this._AumentoArt3 = aumentoArt3;
                this._ExInpdaiArt3 = exInpdaiArt3;
                this._ImportoComplessivoArt3 = importoComplessivoArt3;
                this._ExInpdaiArt10 = exInpdaiArt10;
                this._DecorrenzaMaggiorazioneLegge140 = decorrenzaMaggiorazioneLegge140;
                this._DecorrenzaMaggiorazioneLegge544 = decorrenzaMaggiorazioneLegge544;
                this._AumentoMensileLegge161289Art2 = aumentoMensileLegge161289Art2;
                this._AnniRiduzioneBeneficiArt38Legge02 = anniRiduzioneBeneficiArt38Legge02;
                this._Aumento7290 = aumento7290;
                this._Aumento7290DC = aumento7290DC;
                this._AumentoMensileLegge5991Comma9 = aumentoMensileLegge5991Comma9;
                this._AumentoMensileLegge5991Comma2 = aumentoMensileLegge5991Comma2;
                this._ImportoBeneficiCombattente = importoBeneficiCombattente;
                this._Sentenza495240 = sentenza495240;
                this._DecorrenzaVariazione = decorrenzaVariazione;
                this._Cessazione = cessazione;
                this._CodiceLeggeGruppo = codiceLeggeGruppo;
                this._CodiceLeggeSottogruppo = codiceLeggeSottogruppo;
                this._MaggioreAnzianitaConcessa = maggioreAnzianitaConcessa;
                this._MancataContribuzione = mancataContribuzione;
                this._CodiceBenefici = codiceBenefici;
                this._SettimaneBenefici = settimaneBenefici;
                this._CodiceInvalidita80Percento = codiceInvalidita80Percento;
                this._CessazioneMaggiorazioneSociale = cessazioneMaggiorazioneSociale;
                this._CodiceRequisitiLegge50392Art2 = codiceRequisitiLegge50392Art2;
                this._DecorrenzaInv80 = decorrenzaInv80;
                this._NSettIncrementoPrepensionamento = nSettIncrementoPrepensionamento;
                this._Articolo1Legge5991 = articolo1Legge5991;
                this._MensileLegge5991 = mensileLegge5991;
                this._ExCombattente = exCombattente;
                this._RMSSenzaLegge33670QA = RMSSenzaLegge33670QA;
                this._RMSSenzaLegge33670QB = RMSSenzaLegge33670QB;
                this._PercentualeMaggiorazioneSenzaLegge33670 = percentualeMaggiorazioneSenzaLegge33670;
                this._DecorrenzaMaggiorazioneSociale = decorrenzaMaggiorazioneSociale;
                this._DirittoScattiLegge336 = dirittoScattiLegge336;
                this._SettimaneBeneficioAA = settimaneBeneficioAA;
                this._SettimaneBeneficioMM = settimaneBeneficioMM;
                this._SettimaneBeneficioGG = settimaneBeneficioGG;
                this._IsBeneficioArt24Comma15BisFromFELPE = isBeneficioArt24Comma15BisFromFELPE;
                this._MaggiorazioneAmianto = maggiorazioneAmianto;
                this._MaggiorazioneInv74 = maggiorazioneInv74;
                this._IsBeneficioApePrecociFromFELPE = isBeneficioApePrecociFromFELPE;
                this._SettAnzContribPost311295 = settAnzContribPost311295;
                this._DataNonVedenteDal = dataNonvedenteDal;
                this._NSettIntegrazioneContributivaConcessa = nSettIntegrazioneContributivaConcessa;
            }

            #region private properties

            private long _Id;

            private long _IdPensione;

            private int? _NSettimaneBeneficio;

            private string _TipoSettimaneBeneficio;

            private int? _NSettimaneIncremento1Percento;

            private int? _NSettimaneIncremento05Percento;

            private bool? _Attivitausuranti;

            private byte? _CodiceCieco;

            private DateTime? _DecorrenzaMaggiorazioneArt6;

            private decimal? _ImportoArt6;

            private bool? _Aumento780ContributiArt4;

            private bool? _Aumento780ContributiArt1;

            private bool? _ExInpdai;

            private decimal? _ImportoAumento780Contributi;

            private decimal? _ImportoComplessivoArt4;

            private bool? _AumentoArt5;

            private bool? _ExInpdaiArt4;

            private decimal? _ImportoComplessivoArt5;

            private bool? _AumentoArt3;

            private bool? _ExInpdaiArt3;

            private decimal? _ImportoComplessivoArt3;

            private bool? _ExInpdaiArt10;

            private DateTime? _DecorrenzaMaggiorazioneLegge140;

            private DateTime? _DecorrenzaMaggiorazioneLegge544;

            private decimal? _AumentoMensileLegge161289Art2;

            private decimal? _Aumento7290;

            private decimal? _Aumento7290DC;

            private decimal? _AumentoMensileLegge5991Comma9;

            private decimal? _AumentoMensileLegge5991Comma2;

            private decimal? _ImportoBeneficiCombattente;

            private byte? _Sentenza495240;

            private DateTime? _DecorrenzaVariazione;

            private DateTime? _Cessazione;

            private short? _CodiceLeggeGruppo;

            private short? _CodiceLeggeSottogruppo;

            private short? _MaggioreAnzianitaConcessa;

            private decimal? _MancataContribuzione;

            private string _CodiceBenefici;

            private short? _SettimaneBenefici;

            private string _CodiceInvalidita80Percento;

            private DateTime? _CessazioneMaggiorazioneSociale;

            private byte? _CodiceRequisitiLegge50392Art2;

            private DateTime? _DecorrenzaInv80;

            private int? _NSettIncrementoPrepensionamento;

            private bool? _Articolo1Legge5991;

            private decimal? _MensileLegge5991;

            private long? _ExCombattente;

            private decimal? _RMSSenzaLegge33670QA;

            private decimal? _RMSSenzaLegge33670QB;

            private byte? _PercentualeMaggiorazioneSenzaLegge33670;

            private DateTime? _DecorrenzaMaggiorazioneSociale;

            private int? _DirittoScattiLegge336;

            private short? _SettimaneBeneficioAA;

            private short? _SettimaneBeneficioMM;

            private short? _SettimaneBeneficioGG;

            private short? _AnniRiduzioneBeneficiArt38Legge02;

            private bool? _IsBeneficioArt24Comma15BisFromFELPE;

            private int? _MaggiorazioneAmianto;

            private int? _MaggiorazioneInv74;

            private bool? _IsBeneficioApePrecociFromFELPE;

            private short? _SettAnzContribPost311295;

            private DateTime? _DataNonVedenteDal;

            private byte? _PercentualeMaggiorazione;

            private int? _NSettIntegrazioneContributivaConcessa;

            private decimal? _ImportoComplessivoArt1;

            private byte? _Articolo6140; 

            #endregion private properties

            #region public properties

            public long Id{ get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public int? NSettimaneBeneficio { get { return _NSettimaneBeneficio; } set { _NSettimaneBeneficio = value; } }

            public string TipoSettimaneBeneficio { get { return _TipoSettimaneBeneficio; } set { _TipoSettimaneBeneficio = value; } }

            public int? NSettimaneIncremento1Percento { get { return _NSettimaneIncremento1Percento; } set { _NSettimaneIncremento1Percento = value; } }

            public int? NSettimaneIncremento05Percento { get { return _NSettimaneIncremento05Percento; } set { _NSettimaneIncremento05Percento = value; } }

            public bool? Attivitausuranti { get { return _Attivitausuranti; } set { _Attivitausuranti = value; } }

            public byte? CodiceCieco { get { return _CodiceCieco; } set { _CodiceCieco = value; } }

            public DateTime? DecorrenzaMaggiorazioneArt6 { get { return _DecorrenzaMaggiorazioneArt6; } set { _DecorrenzaMaggiorazioneArt6 = value; } }

            public decimal? ImportoArt6 { get { return _ImportoArt6; } set { _ImportoArt6 = value; } }

            public bool? Aumento780ContributiArt4 { get { return _Aumento780ContributiArt4; } set { _Aumento780ContributiArt4 = value; } }

            public bool? Aumento780ContributiArt1 { get { return _Aumento780ContributiArt1; } set { _Aumento780ContributiArt1 = value; } }

            public bool? ExInpdai { get { return _ExInpdai; } set { _ExInpdai = value; } }

            public decimal? ImportoAumento780Contributi { get { return _ImportoAumento780Contributi; } set { _ImportoAumento780Contributi = value; } }

            public decimal? ImportoComplessivoArt4 { get { return _ImportoComplessivoArt4; } set { _ImportoComplessivoArt4 = value; } }

            public bool? AumentoArt5 { get { return _AumentoArt5; } set { _AumentoArt5 = value; } }

            public bool? ExInpdaiArt4 { get { return _ExInpdaiArt4; } set { _ExInpdaiArt4 = value; } }

            public decimal? ImportoComplessivoArt5 { get { return _ImportoComplessivoArt5; } set { _ImportoComplessivoArt5 = value; } }

            public bool? AumentoArt3 { get { return _AumentoArt3; } set { _AumentoArt3 = value; } }

            public bool? ExInpdaiArt3 { get { return _ExInpdaiArt3; } set { _ExInpdaiArt3 = value; } }

            public decimal? ImportoComplessivoArt3 { get { return _ImportoComplessivoArt3; } set { _ImportoComplessivoArt3 = value; } }

            public bool? ExInpdaiArt10 { get { return _ExInpdaiArt10; } set { _ExInpdaiArt10 = value; } }

            public DateTime? DecorrenzaMaggiorazioneLegge140 { get { return _DecorrenzaMaggiorazioneLegge140; } set { _DecorrenzaMaggiorazioneLegge140 = value; } }

            public DateTime? DecorrenzaMaggiorazioneLegge544 { get { return _DecorrenzaMaggiorazioneLegge544; } set { _DecorrenzaMaggiorazioneLegge544 = value; } }

            public decimal? AumentoMensileLegge161289Art2 { get { return _AumentoMensileLegge161289Art2; } set { _AumentoMensileLegge161289Art2 = value; } }

            public decimal? Aumento7290 { get { return _Aumento7290; } set { _Aumento7290 = value; } }

            public decimal? Aumento7290DC { get { return _Aumento7290DC; } set { _Aumento7290DC = value; } }

            public decimal? AumentoMensileLegge5991Comma9 { get { return _AumentoMensileLegge5991Comma9; } set { _AumentoMensileLegge5991Comma9 = value; } }

            public decimal? AumentoMensileLegge5991Comma2 { get { return _AumentoMensileLegge5991Comma2; } set { _AumentoMensileLegge5991Comma2 = value; } }

            public decimal? ImportoBeneficiCombattente { get { return _ImportoBeneficiCombattente; } set { _ImportoBeneficiCombattente = value; } }

            public byte? Sentenza495240 { get { return _Sentenza495240; } set { _Sentenza495240 = value; } }

            public DateTime? DecorrenzaVariazione { get { return _DecorrenzaVariazione; } set { _DecorrenzaVariazione = value; } }

            public DateTime? Cessazione { get { return _Cessazione; } set { _Cessazione = value; } }

            public short? CodiceLeggeGruppo { get { return _CodiceLeggeGruppo; } set { _CodiceLeggeGruppo = value; } }

            public short? CodiceLeggeSottogruppo { get { return _CodiceLeggeSottogruppo; } set { _CodiceLeggeSottogruppo = value; } }

            public short? MaggioreAnzianitaConcessa { get { return _MaggioreAnzianitaConcessa; } set { _MaggioreAnzianitaConcessa = value; } }

            public decimal? MancataContribuzione { get { return _MancataContribuzione; } set { _MancataContribuzione = value; } }

            public string CodiceBenefici { get { return _CodiceBenefici; } set { _CodiceBenefici = value; } }

            public short? SettimaneBenefici { get { return _SettimaneBenefici; } set { _SettimaneBenefici = value; } }

            public string CodiceInvalidita80Percento { get { return _CodiceInvalidita80Percento; } set { _CodiceInvalidita80Percento = value; } }

            public DateTime? CessazioneMaggiorazioneSociale { get { return _CessazioneMaggiorazioneSociale; } set { _CessazioneMaggiorazioneSociale = value; } }

            public byte? CodiceRequisitiLegge50392Art2 { get { return _CodiceRequisitiLegge50392Art2; } set { _CodiceRequisitiLegge50392Art2 = value; } }

            public DateTime? DecorrenzaInv80 { get { return _DecorrenzaInv80; } set { _DecorrenzaInv80 = value; } }

            public int? NSettIncrementoPrepensionamento { get { return _NSettIncrementoPrepensionamento; } set { _NSettIncrementoPrepensionamento = value; } }

            public bool? Articolo1Legge5991 { get { return _Articolo1Legge5991; } set { _Articolo1Legge5991 = value; } }

            public decimal? MensileLegge5991 { get { return _MensileLegge5991; } set { _MensileLegge5991 = value; } }

            public long? ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }

            public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }

            public decimal? RMSSenzaLegge33670QB { get { return _RMSSenzaLegge33670QB; } set { _RMSSenzaLegge33670QB = value; } }

            public byte? PercentualeMaggiorazioneSenzaLegge33670 { get { return _PercentualeMaggiorazioneSenzaLegge33670; } set { _PercentualeMaggiorazioneSenzaLegge33670 = value; } }

            public DateTime? DecorrenzaMaggiorazioneSociale { get { return _DecorrenzaMaggiorazioneSociale; } set { _DecorrenzaMaggiorazioneSociale = value; } }

            public int? DirittoScattiLegge336 { get { return _DirittoScattiLegge336; } set { _DirittoScattiLegge336 = value; } }

            public short? SettimaneBeneficioAA { get { return _SettimaneBeneficioAA; } set { _SettimaneBeneficioAA = value; } }

            public short? SettimaneBeneficioMM { get { return _SettimaneBeneficioMM; } set { _SettimaneBeneficioMM = value; } }

            public short? SettimaneBeneficioGG { get { return _SettimaneBeneficioGG; } set { _SettimaneBeneficioGG = value; } }

            public short? AnniRiduzioneBeneficiArt38Legge02 { get { return _AnniRiduzioneBeneficiArt38Legge02; } set { _AnniRiduzioneBeneficiArt38Legge02 = value; } }

            public bool? IsBeneficioArt24Comma15BisFromFELPE { get { return _IsBeneficioArt24Comma15BisFromFELPE; } set { _IsBeneficioArt24Comma15BisFromFELPE = value; } }

            public int? MaggiorazioneAmianto { get { return _MaggiorazioneAmianto; } set { _MaggiorazioneAmianto = value; } }

            public int? MaggiorazioneInv74 { get { return _MaggiorazioneInv74; } set { _MaggiorazioneInv74 = value; } }

            public bool? IsBeneficioApePrecociFromFELPE { get { return _IsBeneficioApePrecociFromFELPE; } set { _IsBeneficioApePrecociFromFELPE = value; } }

            public short? SettAnzContribPost311295 { get { return _SettAnzContribPost311295; } set { _SettAnzContribPost311295 = value; } }

            public DateTime? DataNonVedenteDal { get { return _DataNonVedenteDal; } set { _DataNonVedenteDal = value; } }

            public byte? PercentualeMaggiorazione { get { return _PercentualeMaggiorazione; } set { _PercentualeMaggiorazione = value; } }

            public int? NSettIntegrazioneContributivaConcessa { get { return _NSettIntegrazioneContributivaConcessa; } set { _NSettIntegrazioneContributivaConcessa = value; } }

            public decimal? ImportoComplessivoArt1 { get { return _ImportoComplessivoArt1; } set { _ImportoComplessivoArt1 = value; } }

            public byte? Articolo6140 { get { return _Articolo6140; } set { _Articolo6140 = value; } }
            

            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiMaggiorazioniBenefici maggiorazioneBenefici = (DatiMaggiorazioniBenefici)obj;
                try
                {
                    if (this._AnniRiduzioneBeneficiArt38Legge02 != maggiorazioneBenefici._AnniRiduzioneBeneficiArt38Legge02 ||
                        this._Articolo1Legge5991 != maggiorazioneBenefici._Articolo1Legge5991 ||
                        this._Attivitausuranti != maggiorazioneBenefici._Attivitausuranti ||
                        this._Aumento7290 != maggiorazioneBenefici._Aumento7290 ||
                        this._Aumento7290DC != maggiorazioneBenefici._Aumento7290DC ||
                        this._Aumento780ContributiArt1 != maggiorazioneBenefici._Aumento780ContributiArt1 ||
                        this._Aumento780ContributiArt4 != maggiorazioneBenefici._Aumento780ContributiArt4 ||
                        this._AumentoArt3 != maggiorazioneBenefici._AumentoArt3 ||
                        this._AumentoArt5 != maggiorazioneBenefici._AumentoArt5 ||
                        this._AumentoMensileLegge161289Art2 != maggiorazioneBenefici._AumentoMensileLegge161289Art2 ||
                        this._AumentoMensileLegge5991Comma2 != maggiorazioneBenefici._AumentoMensileLegge5991Comma2 ||
                        this._AumentoMensileLegge5991Comma9 != maggiorazioneBenefici._AumentoMensileLegge5991Comma9 ||
                        this._Cessazione != maggiorazioneBenefici._Cessazione ||
                        this._CessazioneMaggiorazioneSociale != maggiorazioneBenefici._CessazioneMaggiorazioneSociale ||
                        this._CodiceBenefici != maggiorazioneBenefici._CodiceBenefici ||
                        this._CodiceCieco != maggiorazioneBenefici._CodiceCieco ||
                        this._CodiceInvalidita80Percento != maggiorazioneBenefici._CodiceInvalidita80Percento ||
                        this._CodiceLeggeGruppo != maggiorazioneBenefici._CodiceLeggeGruppo ||
                        this._CodiceLeggeSottogruppo != maggiorazioneBenefici._CodiceLeggeSottogruppo ||
                        this._CodiceRequisitiLegge50392Art2 != maggiorazioneBenefici._CodiceRequisitiLegge50392Art2 ||
                        this._DecorrenzaInv80 != maggiorazioneBenefici._DecorrenzaInv80 ||
                        this._DecorrenzaMaggiorazioneArt6 != maggiorazioneBenefici._DecorrenzaMaggiorazioneArt6 ||
                        this._DecorrenzaMaggiorazioneLegge140 != maggiorazioneBenefici._DecorrenzaMaggiorazioneLegge140 ||
                        this._DecorrenzaMaggiorazioneLegge544 != maggiorazioneBenefici._DecorrenzaMaggiorazioneLegge544 ||
                        this._DecorrenzaVariazione != maggiorazioneBenefici._DecorrenzaVariazione ||
                        this._ExInpdai != maggiorazioneBenefici._ExInpdai ||
                        this._ExInpdaiArt10 != maggiorazioneBenefici._ExInpdaiArt10 ||
                        this._ExInpdaiArt3 != maggiorazioneBenefici._ExInpdaiArt3 ||
                        this._ExInpdaiArt4 != maggiorazioneBenefici._ExInpdaiArt4 ||
                        this._ImportoArt6 != maggiorazioneBenefici._ImportoArt6 ||
                        this._ImportoAumento780Contributi != maggiorazioneBenefici._ImportoAumento780Contributi ||
                        this._ImportoBeneficiCombattente != maggiorazioneBenefici._ImportoBeneficiCombattente ||
                        this._ImportoComplessivoArt3 != maggiorazioneBenefici._ImportoComplessivoArt3 ||
                        this._ImportoComplessivoArt4 != maggiorazioneBenefici._ImportoComplessivoArt4 ||
                        this._ImportoComplessivoArt5 != maggiorazioneBenefici._ImportoComplessivoArt5 ||
                        this._MaggioreAnzianitaConcessa != maggiorazioneBenefici._MaggioreAnzianitaConcessa ||
                        this._MancataContribuzione != maggiorazioneBenefici._MancataContribuzione ||
                        this._MensileLegge5991 != maggiorazioneBenefici._MensileLegge5991 ||
                        this._NSettimaneBeneficio != maggiorazioneBenefici._NSettimaneBeneficio ||
                        this._NSettimaneIncremento05Percento != maggiorazioneBenefici._NSettimaneIncremento05Percento ||
                        this._NSettimaneIncremento1Percento != maggiorazioneBenefici._NSettimaneIncremento1Percento ||
                        this._NSettIncrementoPrepensionamento != maggiorazioneBenefici._NSettIncrementoPrepensionamento ||
                        this._Sentenza495240 != maggiorazioneBenefici._Sentenza495240 ||
                        this._SettimaneBenefici != maggiorazioneBenefici._SettimaneBenefici ||
                        this._TipoSettimaneBeneficio != maggiorazioneBenefici._TipoSettimaneBeneficio ||
                        this._DecorrenzaMaggiorazioneSociale != maggiorazioneBenefici._DecorrenzaMaggiorazioneSociale ||
                        this._DirittoScattiLegge336 != maggiorazioneBenefici._DirittoScattiLegge336 ||
                        this._SettimaneBeneficioAA != maggiorazioneBenefici._SettimaneBeneficioAA ||
                        this._SettimaneBeneficioMM != maggiorazioneBenefici._SettimaneBeneficioMM ||
                        this._SettimaneBeneficioGG != maggiorazioneBenefici._SettimaneBeneficioGG ||
                        this._IsBeneficioArt24Comma15BisFromFELPE != maggiorazioneBenefici._IsBeneficioArt24Comma15BisFromFELPE ||
                        this._MaggiorazioneAmianto != maggiorazioneBenefici._MaggiorazioneAmianto ||
                        this._MaggiorazioneInv74 != maggiorazioneBenefici._MaggiorazioneInv74 ||
                        this._IsBeneficioApePrecociFromFELPE != maggiorazioneBenefici._IsBeneficioApePrecociFromFELPE ||
                        this._SettAnzContribPost311295 != maggiorazioneBenefici._SettAnzContribPost311295 ||
                        this._DataNonVedenteDal != maggiorazioneBenefici._DataNonVedenteDal ||
                        this._ImportoComplessivoArt1 != maggiorazioneBenefici._ImportoComplessivoArt1 ||
                        this._Articolo6140 != maggiorazioneBenefici._Articolo6140)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public bool IsExCombattenteFSNull()
            {
                if (!_CodiceCieco.HasValue && !_DecorrenzaMaggiorazioneArt6.HasValue && !_ExCombattente.HasValue && !_RMSSenzaLegge33670QA.HasValue && !_RMSSenzaLegge33670QB.HasValue &&
                    !_PercentualeMaggiorazioneSenzaLegge33670.HasValue && !_DirittoScattiLegge336.HasValue)
                    return true;

                return false;
            }

            public bool IsExCombattenteFSNull_FondoFS()
            {
                if (!_CodiceCieco.HasValue && !_DecorrenzaMaggiorazioneArt6.HasValue && !_ExCombattente.HasValue && !_RMSSenzaLegge33670QB.HasValue &&
                    !_PercentualeMaggiorazioneSenzaLegge33670.HasValue && !_DirittoScattiLegge336.HasValue)
                    return true;

                return false;
            }

            public bool IsExCombattenteAGONull()
            {
                if (!_CodiceCieco.HasValue && !_DecorrenzaMaggiorazioneArt6.HasValue)
                    return true;

                return false;
            }

            public bool IsExCombattenteCINull()
            {
                if (!_CodiceCieco.HasValue && !_DecorrenzaMaggiorazioneArt6.HasValue)
                    return true;

                return false;
            }

            public bool IsBeneficiFSNull()
            {
                if (string.IsNullOrEmpty(_TipoSettimaneBeneficio) && !_NSettimaneBeneficio.HasValue && !_SettimaneBeneficioAA.HasValue && !_SettimaneBeneficioMM.HasValue && !_SettimaneBeneficioGG.HasValue &&
                    !_DecorrenzaMaggiorazioneSociale.HasValue && !_CessazioneMaggiorazioneSociale.HasValue && !_SettAnzContribPost311295.HasValue)
                    return true;

                return false;
            }

            public bool IsBeneficiAGONull()
            {
                if (string.IsNullOrEmpty(_TipoSettimaneBeneficio) && !_NSettimaneBeneficio.HasValue && !_Sentenza495240.HasValue && !_NSettimaneIncremento1Percento.HasValue && 
                    !_NSettimaneIncremento05Percento.HasValue && !_SettAnzContribPost311295.HasValue)
                    return true;

                return false;
            }

            public bool IsBeneficiCINull()
            {
                if (string.IsNullOrEmpty(_TipoSettimaneBeneficio) && !_NSettimaneBeneficio.HasValue && !_Sentenza495240.HasValue && !_NSettimaneIncremento1Percento.HasValue && 
                    !_NSettimaneIncremento05Percento.HasValue && !_SettAnzContribPost311295.HasValue)
                    return true;

                return false;
            }

            public bool IsMaggiorazioniAGONull()
            {
                if (!_DecorrenzaMaggiorazioneSociale.HasValue && !_CessazioneMaggiorazioneSociale.HasValue && !_AnniRiduzioneBeneficiArt38Legge02.HasValue)
                    return true;

                return false;
            }

            public bool IsMaggiorazioniCINull()
            {
                if (!_DecorrenzaMaggiorazioneSociale.HasValue && !_CessazioneMaggiorazioneSociale.HasValue && !_AnniRiduzioneBeneficiArt38Legge02.HasValue && !_CodiceRequisitiLegge50392Art2.HasValue &&
                    !_DecorrenzaMaggiorazioneLegge140.HasValue)
                    return true;

                return false;
            }

            #endregion public members
        }

        #endregion nested class
    }
}
