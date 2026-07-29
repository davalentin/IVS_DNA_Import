using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDanteCausa
    {
        public static void GetDanteCausabyIdPensione(long idPensione, out DatiDanteCausa datiDanteCausa)
        {
            DanteCausa datiDanteCausaDB = null;
            datiDanteCausa = null;
            DAGestioneDanteCausa.GetDanteCausabyIdPensione(idPensione, out datiDanteCausaDB);
            if (datiDanteCausaDB == null)
                return;
            datiDanteCausa = new DatiDanteCausa();
            Utility.ValorizzaOggetti(datiDanteCausaDB, datiDanteCausa);
        }

        public static void SalvaDanteCausa(DatiDanteCausa dantecausa)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DanteCausa danteToDB = new DanteCausa();
                Utility.ValorizzaOggetti(dantecausa, danteToDB);
                DAGestioneDanteCausa.SalvaDanteCausa(danteToDB);

                transactionScope.Complete();
            }
        }

        public static void DeleteDanteCausaByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDanteCausa.CancellaAllDanteCausaByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        #region Anagrafica

        public static void GetAnagraficaDanteCausabyIdPensione(long idPensione, out GestioneAnagrafica.DatiAnagrafici AnagraficaDC)
        {
            DataCommon.Anagrafica AnagraficaDB = null;
            AnagraficaDC = null;
            DAGestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(idPensione, out AnagraficaDB);
            if (AnagraficaDB == null)
                return;
            AnagraficaDC = new GestioneAnagrafica.DatiAnagrafici();
            Utility.ValorizzaOggetti(AnagraficaDB, AnagraficaDC);
        }

        #endregion Anagrafica

        #region PensioniEstereDC

        public static void GetPensioniEstereDCByIdPensione(long idPensione, out List<PensioniEstereDcBL> pensioniEstereDcBl)
        {
            List<PensioniEstereDC> LpensioniEstereDCDB = null;
            pensioniEstereDcBl = null;

            DanteCausa datiDanteCausaDB = null;
            DAGestioneDanteCausa.GetDanteCausabyIdPensione(idPensione, out datiDanteCausaDB);
            if (datiDanteCausaDB == null)
                return;
            DAGestioneDanteCausa.GetPensioniEstereDC(datiDanteCausaDB.Id, out LpensioniEstereDCDB);
            if (LpensioniEstereDCDB == null)
                return;
            pensioniEstereDcBl = new List<PensioniEstereDcBL>();
            foreach (PensioniEstereDC singledb in LpensioniEstereDCDB)
            {
                PensioniEstereDcBL single = new PensioniEstereDcBL();
                Utility.ValorizzaOggetti(singledb, single);
                pensioniEstereDcBl.Add(single);
            }
        }

        public static void SalvaPensioniEstereDC(PensioniEstereDcBL pensioniEstereDcBL)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDanteCausa.SalvaPensioniEstereDC(pensioniEstereDcBL.IdDanteCausa, pensioniEstereDcBL.CodiciVari, pensioniEstereDcBL.Importo);

                transactionScope.Complete();
            }
        }

        public static void DeletePensioniEstereDCByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDanteCausa.CancellaPensioniEstereDCByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        #endregion PensioniEstereDC

        #region RedditiSentenza495_93

        public static void GetRedditiSentenza495_93ByIdPensione(long idPensione, out List<DatiRedditoSentenza495_93> lReddS49593BL)
        {
            lReddS49593BL = null;
            List<ReddS49593> lReddS49593 = new List<ReddS49593>();
            DAGestioneRedditiS49593.GetRedditiS49593ByIdPensione(idPensione, out lReddS49593);
            if (lReddS49593 != null)
            {
                lReddS49593BL = new List<DatiRedditoSentenza495_93>();
                foreach (ReddS49593 reddS49593 in lReddS49593)
                {
                    DatiRedditoSentenza495_93 redditiSentenza495_93BL = new DatiRedditoSentenza495_93();
                    Utility.ValorizzaOggetti(reddS49593, redditiSentenza495_93BL);
                    if (reddS49593.ICISEN2 != null)
                        redditiSentenza495_93BL.CodiceSentenza = reddS49593.ICISEN2;
                    if(reddS49593.MeseReddito != null)
                        redditiSentenza495_93BL.MeseSentenza = reddS49593.MeseReddito;
                    lReddS49593BL.Add(redditiSentenza495_93BL);
                }
            }
        }

        public static void SalvaRedditiSentenza495_93(DatiRedditoSentenza495_93 ReddS49593BL, GestionePensione.DatiPensione datiPensione)
        {
            ReddS49593 redditiSentenza49593 = new ReddS49593();

            Utility.ValorizzaOggetti(ReddS49593BL, redditiSentenza49593);
            redditiSentenza49593.ICISEN2 = ReddS49593BL.CodiceSentenza;
            redditiSentenza49593.MeseReddito = ReddS49593BL.MeseSentenza;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRedditiS49593.SalvaRedditiS49593(redditiSentenza49593);

                transactionScope.Complete();
            }
        }

        public static void DeleteRedditiSentenza495_93ByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRedditiS49593.EliminaAllRedditiS49593ByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        #endregion RedditiSentenza495_93

        public static byte? GetArticolo6140Value(byte? CodiceCieco)
        { return 1; }

        public static bool IsPresenteDomandaInLiquidazionePerControlloSpacENPALS(string codiceFiscaleDanteCausa, short? categoriaFascicolo, short? sedeFascicolo, int? numeroFascicolo, bool byFascicolo, out long NDomus )
        {
            bool ret = false;
            List <long> nDomus = new List<long>();
            Expression<Func<DanteCausa, bool>> whereCondition = p => true;
            if (byFascicolo)
                if (categoriaFascicolo.HasValue && sedeFascicolo.HasValue && numeroFascicolo.HasValue)
                    whereCondition = whereCondition.And(x => x.CategoriaFascicolo == categoriaFascicolo && x.SedeFascicolo == sedeFascicolo && x.NumeroFascicolo == numeroFascicolo.Value);
                else
                    whereCondition = whereCondition.And(x => x.NumeroFascicolo != null && x.IsFascicoloGenerato == true);
            ret = DAGestioneDanteCausa.IsPresenteDomandaInLiquidazionePerControlloSpacENPALS(codiceFiscaleDanteCausa, whereCondition, out nDomus);
            NDomus = nDomus != null ? nDomus.FirstOrDefault(): 0;
            return ret;
        }

        #region nested class
        public class DatiDanteCausa
        {
            public bool IsNull()
            {

                if (Id == 0 && IdAnagrafica == 0 && IdPensione == 0 && string.IsNullOrEmpty(SiglaCategoria) && string.IsNullOrEmpty(Sede) && !Certificato.HasValue && !DecorrenzaPensione.HasValue &&
                    !DataMorte.HasValue && !ProvenienzaPensione.HasValue && !CodiceTipoPensione.HasValue && !CodiceBeneficiLegge.HasValue && !Maggiorazione781Contributi.HasValue &&
                    string.IsNullOrEmpty(StatoEEResidenza) && !DecorrenzaResidenza.HasValue && string.IsNullOrEmpty(CategoriaAltraPensione) && !EnteAltraPensione.HasValue && !CodiceUCAltraPensione.HasValue &&
                    !CodiceImportoAltraPensione.HasValue && !DecorrenzaAltraPensione.HasValue && !CessazioneAltraPensione.HasValue && string.IsNullOrEmpty(NaturaPensione) &&
                    !ImportoPensione311284.HasValue && !ImportoPensione1185.HasValue && !ImportoPensione1190.HasValue && !NContributiDiretta.HasValue && !CodiciVari.HasValue &&
                    !EccedenzaArt5.HasValue && !ParentelaDC.HasValue && !CodiceTipoPerequazione.HasValue && !VirtualePura.HasValue && !VirtualeIntegrata.HasValue && !Adeguata.HasValue &&
                    !CodiceEliminazione.HasValue && !DecorrenzaEliminazione.HasValue && !DecorrenzaEliminazioneContabile.HasValue && !TotaleQuoteFisse.HasValue && !DataMorteOrigine.HasValue &&
                    !StatoEEResidenzaByArca.HasValue && !CittadinanzaByArca.HasValue && string.IsNullOrEmpty(NaturaPensioneAltraPensione) && !CategoriaFascicolo.HasValue &&
                    !SedeFascicolo.HasValue && !NumeroFascicolo.HasValue && !IsFascicoloGenerato.HasValue && !DataMatrimonioByPrelievo.HasValue && !SiglaFamiliare.HasValue)
                    return true;

                return false;
            }

            #region private properties

            private long _Id;
            private long _IdAnagrafica;
            private long _IdPensione;
            private string _SiglaCategoria;
            private string _Sede;
            private System.Nullable<int> _Certificato;
            private System.Nullable<System.DateTime> _DecorrenzaPensione;
            private System.Nullable<System.DateTime> _DataMorte;
            private System.Nullable<byte> _ProvenienzaPensione;
            private System.Nullable<byte> _CodiceTipoPensione;
            private System.Nullable<byte> _CodiceBeneficiLegge;
            private System.Nullable<byte> _Maggiorazione781Contributi;
            private string _StatoEEResidenza;
            private System.Nullable<System.DateTime> _DecorrenzaResidenza;
            private string _CategoriaAltraPensione;
            private System.Nullable<short> _EnteAltraPensione;
            private System.Nullable<char> _CodiceUCAltraPensione;
            private System.Nullable<char> _CodiceImportoAltraPensione;
            private System.Nullable<System.DateTime> _DecorrenzaAltraPensione;
            private System.Nullable<System.DateTime> _CessazioneAltraPensione;
            private string _NaturaPensione;
            private System.Nullable<decimal> _ImportoPensione311284;
            private System.Nullable<decimal> _ImportoPensione1185;
            private System.Nullable<decimal> _ImportoPensione1190;
            private System.Nullable<int> _NContributiDiretta;
            private System.Nullable<byte> _CodiciVari;
            private System.Nullable<decimal> _EccedenzaArt5;
            private System.Nullable<byte> _ParentelaDC;
            private byte? _CodiceTipoPerequazione;
            private decimal? _VirtualePura;
            private decimal? _VirtualeIntegrata;
            private decimal? _Adeguata;
            private System.Nullable<byte> _CodiceEliminazione;
            private System.Nullable<System.DateTime> _DecorrenzaEliminazione;
            private System.Nullable<System.DateTime> _DecorrenzaEliminazioneContabile;
            private decimal? _TotaleQuoteFisse;
            private DateTime? _DataMorteOrigine;
            private System.Nullable<bool> _StatoEEResidenzaByArca;
            private System.Nullable<bool> _CittadinanzaByArca;
            private string _NaturaPensioneAltraPensione;
            private System.Nullable<char> _SiglaFamiliare;
            private System.Nullable<decimal> _ImportoPagamentoDataMorte49593;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdAnagrafica { get { return _IdAnagrafica; } set { _IdAnagrafica = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            public System.Nullable<int> Certificato { get { return _Certificato; } set { _Certificato = value; } }
            public System.Nullable<System.DateTime> DecorrenzaPensione { get { return _DecorrenzaPensione; } set { _DecorrenzaPensione = value; } }
            public System.Nullable<System.DateTime> DataMorte { get { return _DataMorte; } set { _DataMorte = value; } }
            public System.Nullable<byte> ProvenienzaPensione { get { return _ProvenienzaPensione; } set { _ProvenienzaPensione = value; } }
            public System.Nullable<byte> CodiceTipoPensione { get { return _CodiceTipoPensione; } set { _CodiceTipoPensione = value; } }
            public System.Nullable<byte> CodiceBeneficiLegge { get { return _CodiceBeneficiLegge; } set { _CodiceBeneficiLegge = value; } }
            public System.Nullable<byte> Maggiorazione781Contributi { get { return _Maggiorazione781Contributi; } set { _Maggiorazione781Contributi = value; } }
            public string StatoEEResidenza { get { return _StatoEEResidenza; } set { _StatoEEResidenza = value; } }
            public System.Nullable<System.DateTime> DecorrenzaResidenza { get { return _DecorrenzaResidenza; } set { _DecorrenzaResidenza = value; } }
            public string CategoriaAltraPensione { get { return _CategoriaAltraPensione; } set { _CategoriaAltraPensione = value; } }
            public System.Nullable<short> EnteAltraPensione { get { return _EnteAltraPensione; } set { _EnteAltraPensione = value; } }
            public System.Nullable<char> CodiceUCAltraPensione { get { return _CodiceUCAltraPensione; } set { _CodiceUCAltraPensione = value; } }
            public System.Nullable<char> CodiceImportoAltraPensione { get { return _CodiceImportoAltraPensione; } set { _CodiceImportoAltraPensione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaAltraPensione { get { return _DecorrenzaAltraPensione; } set { _DecorrenzaAltraPensione = value; } }
            public System.Nullable<System.DateTime> CessazioneAltraPensione { get { return _CessazioneAltraPensione; } set { _CessazioneAltraPensione = value; } }
            public string NaturaPensione { get { return _NaturaPensione; } set { _NaturaPensione = value; } }
            public System.Nullable<decimal> ImportoPensione311284 { get { return _ImportoPensione311284; } set { _ImportoPensione311284 = value; } }
            public System.Nullable<decimal> ImportoPensione1185 { get { return _ImportoPensione1185; } set { _ImportoPensione1185 = value; } }
            public System.Nullable<decimal> ImportoPensione1190 { get { return _ImportoPensione1190; } set { _ImportoPensione1190 = value; } }
            public System.Nullable<int> NContributiDiretta { get { return _NContributiDiretta; } set { _NContributiDiretta = value; } }
            public System.Nullable<byte> CodiciVari { get { return _CodiciVari; } set { _CodiciVari = value; } }
            public System.Nullable<decimal> EccedenzaArt5 { get { return _EccedenzaArt5; } set { _EccedenzaArt5 = value; } }
            public System.Nullable<byte> ParentelaDC { get { return _ParentelaDC; } set { _ParentelaDC = value; } }
            public Nullable<byte> CodiceTipoPerequazione { get { return _CodiceTipoPerequazione; } set { _CodiceTipoPerequazione = value; } }
            public Nullable<decimal> VirtualePura { get { return _VirtualePura; } set { _VirtualePura = value; } }
            public Nullable<decimal> VirtualeIntegrata { get { return _VirtualeIntegrata; } set { _VirtualeIntegrata = value; } }
            public Nullable<decimal> Adeguata { get { return _Adeguata; } set { _Adeguata = value; } }
            public System.Nullable<byte> CodiceEliminazione { get { return _CodiceEliminazione; } set { _CodiceEliminazione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaEliminazione { get { return _DecorrenzaEliminazione; } set { _DecorrenzaEliminazione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaEliminazioneContabile { get { return _DecorrenzaEliminazioneContabile; } set { _DecorrenzaEliminazioneContabile = value; } }
            public System.Nullable<decimal> TotaleQuoteFisse { get { return _TotaleQuoteFisse; } set { _TotaleQuoteFisse = value; } }
            public DateTime? DataMorteOrigine { get { return _DataMorteOrigine; } set { _DataMorteOrigine = value; } }
            public bool? StatoEEResidenzaByArca { get { return _StatoEEResidenzaByArca; } set { _StatoEEResidenzaByArca = value; } }
            public bool? CittadinanzaByArca { get { return _CittadinanzaByArca; } set { _CittadinanzaByArca = value; } }
            public string NaturaPensioneAltraPensione { get { return _NaturaPensioneAltraPensione; } set { _NaturaPensioneAltraPensione = value; } }
            public short? CategoriaFascicolo { get; set; }
            public short? SedeFascicolo { get; set; }
            public int? NumeroFascicolo { get; set; }
            public bool? IsFascicoloGenerato { get; set; }
            public bool? DataMatrimonioByPrelievo { get; set; }
            public char? SiglaFamiliare { get { return _SiglaFamiliare; } set { _SiglaFamiliare = value; } }
            public System.Nullable<decimal> ImportoPagamentoDataMorte49593 { get { return _ImportoPagamentoDataMorte49593; } set { _ImportoPagamentoDataMorte49593 = value; } }

            #endregion public properties
        }

        public class PensioniEstereDcBL
        {
            private long? _IdDanteCausa;
            private byte? _CodiciVari;
            private decimal? _Importo;

            public long? IdDanteCausa { get { return _IdDanteCausa; } set { _IdDanteCausa = value; } }
            public byte? CodiciVari { get { return _CodiciVari; } set { _CodiciVari = value; } }
            public decimal? Importo { get { return _Importo; } set { _Importo = value; } }
        }

        public class DatiRedditoSentenza495_93
        {
            private long? _IdPensione;
            private short? _AnnoReddito;
            private decimal? _RedditoTitolare;
            private decimal? _RedditoConiuge;
            private decimal? _RedditoDaPensioneConiuge;
            private decimal? _RedditoDaPensioneDC;
            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            private bool? _IsPre2009;
            private string _CodiceDiReddito;
            private bool? _FlagSentenza;
            private short? _CodiceSentenza;
            private short? _MeseSentenza;
            private short? _AnnoSentenza;
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public short? AnnoReddito { get { return _AnnoReddito; } set { _AnnoReddito = value; } }
            public decimal? RedditoTitolare { get { return _RedditoTitolare; } set { _RedditoTitolare = value; } }
            public decimal? RedditoConiuge { get { return _RedditoConiuge; } set { _RedditoConiuge = value; } }
            public decimal? RedditoDaPensioneConiuge { get { return _RedditoDaPensioneConiuge; } set { _RedditoDaPensioneConiuge = value; } }
            public decimal? RedditoDaPensioneDC { get { return _RedditoDaPensioneDC; } set { _RedditoDaPensioneDC = value; } }
            //ENG - Gestione Pensione Estera e redditi Sentenza 495
            public bool? IsPre2009 { get { return _IsPre2009; } set { _IsPre2009 = value; } }
            public string CodiceDiReddito { get { return _CodiceDiReddito; } set { _CodiceDiReddito = value; } }
            public bool? FlagSentenza { get { return _FlagSentenza; } set { _FlagSentenza = value; } }
            public short? CodiceSentenza { get { return _CodiceSentenza; } set { _CodiceSentenza = value; } }
            public short? MeseSentenza { get { return _MeseSentenza; } set { _MeseSentenza = value; } }
            public short? AnnoSentenza { get { return _AnnoSentenza; } set { _AnnoSentenza = value; } }

        }

        #endregion   nested class
    }
}
