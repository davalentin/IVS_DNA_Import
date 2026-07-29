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
    public class GestioneAnagrafica
    {
        #region public members
        public static void GetAnagraficaByCodiceFiscale(string codiceFiscale, out DatiAnagrafici datiAnagrafici)
        {
            
            Anagrafica anagrafica = null;
            datiAnagrafici = null;
            DAGestioneAnagrafica.GetAnagraficaByCodiceFiscale(codiceFiscale, out anagrafica);
            if (anagrafica == null)
                return;
            datiAnagrafici = new DatiAnagrafici();
            Utility.ValorizzaOggetti(anagrafica, datiAnagrafici);
            DateTime? dataMorteTitolare = null;
            DAGestionePensione.GetDataMorteByIdAnagrafica(anagrafica.Id, out dataMorteTitolare);
            datiAnagrafici.DataMorte = dataMorteTitolare;
        }

        public static void GetAnagraficaByIdAnagrafica(long idAnagrafica, out DatiAnagrafici datiAnagrafici)
        {
            Anagrafica anagrafica = null;
            datiAnagrafici = null;
            DAGestioneAnagrafica.GetAnagraficaByIdAnagrafica(idAnagrafica, out anagrafica);
            if (anagrafica == null)
                return;
            datiAnagrafici = new DatiAnagrafici();
            Utility.ValorizzaOggetti(anagrafica, datiAnagrafici);
        }

        public static void GetIdAnagraficaByCodiceFiscale(string codiceFiscale, out long idAnagrafica)
        {
            idAnagrafica = 0;
            DAGestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(codiceFiscale, out idAnagrafica);
        }

        public static void GetIdAnagraficaByNomeCognome(string nome, string cognome, out List<long> listaIdAnagrafica)
        {
            DAGestioneAnagrafica.GetIdAnagraficaByNomeCognome(nome, cognome, out listaIdAnagrafica);
        }

        public static void GetAnagraficaByIdPensione(Int64 idPensione, out DatiAnagrafici datiAnagrafici)
        {
            Anagrafica anagrafica = null;
            Titolare titolare = null;
            datiAnagrafici = null;
            DAGestioneAnagrafica.GetAnagraficaByIdPensione(idPensione, out anagrafica);
            DAGestionePensione.GetTitolareByIdPensione(idPensione, out titolare);
            if (anagrafica == null)
                return;
            datiAnagrafici = new DatiAnagrafici();
            Utility.ValorizzaOggetti(anagrafica, datiAnagrafici);

            if (titolare != null && titolare.DataMorte.HasValue)
                datiAnagrafici.DataMorte = titolare.DataMorte;
        }

        public static void GetAreaTitolareByDatiPensione(GestionePensione.DatiPensione datiPensione, out Entity.AreaTitolare areaTitolare)
        {
            areaTitolare = new Entity.AreaTitolare();

            Anagrafica anagrafica = null;
            List<StatoCivile> elencoStatiCivili = null;
            List<ResidenzeEstero> elencoResidenzeEstere = null;
            Titolare titolare = null;
            DAGestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagrafica, out elencoStatiCivili, out elencoResidenzeEstere);
            DAGestionePensione.GetTitolareByIdPensione(datiPensione.Id, out titolare);

            if (anagrafica == null)
                return;

            Utility.ValorizzaOggetti(anagrafica, areaTitolare.Anagrafica);

            if (titolare != null && titolare.DataMorte.HasValue)
                areaTitolare.Anagrafica.DataMorte = titolare.DataMorte;

            if (elencoStatiCivili != null)
            {
                foreach (StatoCivile sc in elencoStatiCivili)
                {
                    GestioneAnagrafica.DatiStatoCivile datiStatoCivile = new DatiStatoCivile();
                    Utility.ValorizzaOggetti(sc, datiStatoCivile);
                    areaTitolare.ElencoStatiCivili.Add(datiStatoCivile);
                }
            }

            if (elencoResidenzeEstere != null)
            {
                foreach (ResidenzeEstero re in elencoResidenzeEstere)
                {
                    GestioneAnagrafica.DatiResidenzaEstero datiResidenzaEstero = new DatiResidenzaEstero();
                    Utility.ValorizzaOggetti(re, datiResidenzaEstero);
                    areaTitolare.ElencoResidenzeEstere.Add(datiResidenzaEstero);
                }
            }

            areaTitolare.Pensione = datiPensione;

            GestionePensione.DatiPatronato datiPatronato = null;
            GestionePensione.GetPatronatoByIdPensione(datiPensione.Id, out datiPatronato);
            if (datiPatronato != null)
                areaTitolare.Patronato = datiPatronato;
            else
                areaTitolare.Patronato = null;

            GestionePensione.DatiSindacato datiSindacato = null;
            GestionePensione.GetSindacatoByIdPensione(datiPensione.Id, out datiSindacato);
            if (datiSindacato != null)
                areaTitolare.Sindacato = datiSindacato;
            else
                areaTitolare.Sindacato = null;
        }

        public static void GetResidenzeEstereByIdPensione(Int64 idPensione, out List<GestioneAnagrafica.DatiResidenzaEstero> elencoResidenzeEstere)
        {
            elencoResidenzeEstere = null;
            GestionePensione.DatiTitolare datiTitolare = null;
            GestionePensione.GetTitolareByIdPensione(idPensione, out datiTitolare);
            if (datiTitolare != null)
            {
                List<ResidenzeEstero> elencoResidenzeEstereDB = null;
                DAGestioneAnagrafica.GetResidenzeEstereById(datiTitolare.IdAnagrafica, datiTitolare.IdPensione, out elencoResidenzeEstereDB);

                if (elencoResidenzeEstereDB != null)
                {
                    elencoResidenzeEstere = new List<DatiResidenzaEstero>();
                    foreach (ResidenzeEstero re in elencoResidenzeEstereDB)
                    {
                        GestioneAnagrafica.DatiResidenzaEstero datiResidenzaEstero = new DatiResidenzaEstero();
                        Utility.ValorizzaOggetti(re, datiResidenzaEstero);
                        elencoResidenzeEstere.Add(datiResidenzaEstero);
                    }
                }
            }
        }

        public static void SalvaAnagrafica(DatiAnagrafici datiAnagrafici)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Anagrafica anagrafica = new Anagrafica();
                Utility.ValorizzaOggetti(datiAnagrafici, anagrafica);
                DAGestioneAnagrafica.SalvaAnagrafica(anagrafica);
                transactionScope.Complete();
                datiAnagrafici.Id = anagrafica.Id;
            }
        }

        public static void AggiornaAnagrafica(DatiAnagrafici datiAnagrafici)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnagrafica.AggiornaAnagrafica(datiAnagrafici.CodiceFiscale, datiAnagrafici.Cittadinanza, datiAnagrafici.Tel, datiAnagrafici.Cell, datiAnagrafici.EMail);
                transactionScope.Complete();
            }
        }

        public static void SalvaStatoCivile(long idAnagrafica, long idPensione, DatiStatoCivile datiStatoCivile)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StatoCivile statoCivile = new StatoCivile();
                Utility.ValorizzaOggetti(datiStatoCivile, statoCivile);
                statoCivile.IdAnagrafica = idAnagrafica;
                statoCivile.IdPensione = idPensione;
                DAGestioneAnagrafica.SalvaStatoCivile(statoCivile);
                transactionScope.Complete();
            }
        }

        public static void SalvaResidenzaEstero(long idAnagrafica, long idPensione, DatiResidenzaEstero datiResidenzaEstero)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                ResidenzeEstero residenzaEstero = new ResidenzeEstero();
                Utility.ValorizzaOggetti(datiResidenzaEstero, residenzaEstero);
                residenzaEstero.IdAnagrafica = idAnagrafica;
                residenzaEstero.IdPensione = idPensione;
                DAGestioneAnagrafica.SalvaResidenzaEstero(residenzaEstero);
                transactionScope.Complete();
            }
        }

        public static void EliminaResidenzeEstero(long idAnagrafica, long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnagrafica.EliminaResidenzeEstero(idAnagrafica, idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaStatiCivili(long idAnagrafica, long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnagrafica.EliminaStatiCivili(idAnagrafica, idPensione);
                transactionScope.Complete();
            }
        }

        public static void GetLatestStatoCivileById(long idAnagrafica, long idPensione, out DatiStatoCivile datiStatoCivile)
        {
            StatoCivile statoCivile = null;
            datiStatoCivile = new DatiStatoCivile();
            DAGestioneAnagrafica.GetLatestStatoCivileById(idAnagrafica, idPensione, out statoCivile);
            if (statoCivile == null)
                return;
            Utility.ValorizzaOggetti(statoCivile, datiStatoCivile);
        }

        public static void DeleteAnagrafica(long idAnagrafica)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnagrafica.DeleteAnagrafica(idAnagrafica);
                transactionScope.Complete();
            }
        }

        #endregion public members

        #region nested class
        public class DatiAnagrafici
        {
            public DatiAnagrafici()
            { }
            public DatiAnagrafici(string codiceFiscale, string cittadinanza, string tel, string cell, string eMail, DateTime? dataNascita, bool? residenzaEstero, string codiceComuneResidenza,
                string frazioneResidenza, string provinciaResidenza, char? sesso, DateTime? dataMorte, string cognome)
            {
                this._CodiceFiscale = codiceFiscale;
                this._Cittadinanza = cittadinanza;
                this._Tel = tel;
                this._Cell = cell;
                this._EMail = eMail;
                this._DataNascita = dataNascita;
                this._ResidenzaEstero = residenzaEstero;
                this._CodiceComuneResidenza = codiceComuneResidenza;
                this._FrazioneResidenza = frazioneResidenza;
                this._ProvinciaResidenza = provinciaResidenza;
                this._Sesso = sesso;
                this._DataMorte = dataMorte;
                this._Cognome = cognome;
            }
            #region private properties
            private long _Id;

            private string _CodiceFiscale;

            private string _Cognome;

            private string _Nome;

            private string _CognomeAcquisito;

            private System.Nullable<char> _Sesso;

            private System.Nullable<System.DateTime> _DataNascita;

            private string _ComuneNascita;

            private string _CodiceComuneNascita;

            private string _ProvinciaNascita;

            private string _Cittadinanza;

            private string _ComuneResidenza;

            private string _CodiceComuneResidenza;

            private string _Indirizzo;

            private string _NCivico;

            private string _CAP;

            private string _ProvinciaResidenza;

            private string _FrazioneResidenza;

            private System.Nullable<bool> _DomicilioEstero;

            private System.Nullable<bool> _ResidenzaEstero;

            private string _Codice1Arca;

            private string _Codice2Arca;

            private string _Tel;

            private string _Cell;

            private string _EMail;

            private char? _CodiceStatoCivile;

            private System.Nullable<DateTime> _DecorrenzaStatoCivile;

            private char? _CodiceDelegato;

            private char? _CodiceTutore;

            private DateTime? _CessValAmmSost;

            private System.Nullable<System.DateTime> _DataMorte;

            private DateTime? _DataMatrimonio;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }

            public string Cognome { get { return _Cognome; } set { _Cognome = value; } }

            public string Nome { get { return _Nome; } set { _Nome = value; } }

            public string CognomeAcquisito { get { return _CognomeAcquisito; } set { _CognomeAcquisito = value; } }

            public System.Nullable<char> Sesso { get { return _Sesso; } set { _Sesso = value; } }

            public System.Nullable<System.DateTime> DataNascita { get { return _DataNascita; } set { _DataNascita = value; } }

            public string ComuneNascita { get { return _ComuneNascita; } set { _ComuneNascita = value; } }

            public string CodiceComuneNascita { get { return _CodiceComuneNascita; } set { _CodiceComuneNascita = value; } }

            public string ProvinciaNascita { get { return _ProvinciaNascita; } set { _ProvinciaNascita = value; } }

            public string Cittadinanza { get { return _Cittadinanza; } set { _Cittadinanza = value; } }

            public string ComuneResidenza { get { return _ComuneResidenza; } set { _ComuneResidenza = value; } }

            public string CodiceComuneResidenza { get { return _CodiceComuneResidenza; } set { _CodiceComuneResidenza = value; } }

            public string Indirizzo { get { return _Indirizzo; } set { _Indirizzo = value; } }

            public string NCivico { get { return _NCivico; } set { _NCivico = value; } }

            public string CAP { get { return _CAP; } set { _CAP = value; } }

            public string ProvinciaResidenza { get { return _ProvinciaResidenza; } set { _ProvinciaResidenza = value; } }

            public string FrazioneResidenza { get { return _FrazioneResidenza; } set { _FrazioneResidenza = value; } }

            public System.Nullable<bool> DomicilioEstero { get { return _DomicilioEstero; } set { _DomicilioEstero = value; } }

            public System.Nullable<bool> ResidenzaEstero { get { return _ResidenzaEstero; } set { _ResidenzaEstero = value; } }

            public string Codice1Arca { get { return _Codice1Arca; } set { _Codice1Arca = value; } }

            public string Codice2Arca { get { return _Codice2Arca; } set { _Codice2Arca = value; } }

            public string Tel { get { return _Tel; } set { _Tel = value; } }

            public string Cell { get { return _Cell; } set { _Cell = value; } }

            public string EMail { get { return _EMail; } set { _EMail = value; } }

            public char? CodiceStatoCivile { get { return _CodiceStatoCivile; } set { _CodiceStatoCivile = value; } }

            public System.Nullable<DateTime> DecorrenzaStatoCivile { get { return _DecorrenzaStatoCivile; } set { _DecorrenzaStatoCivile = value; } }

            public char? CodiceDelegato { get { return _CodiceDelegato; } set { _CodiceDelegato = value; } }

            public char? CodiceTutore { get { return _CodiceTutore; } set { _CodiceTutore = value; } }

            public System.Nullable<System.DateTime> CessValAmmSost { get { return _CessValAmmSost; } set { _CessValAmmSost = value; } }

            public System.Nullable<System.DateTime> DataMorte { get { return _DataMorte; } set { _DataMorte = value; } }

            public DateTime? DataMatrimonio { get { return _DataMatrimonio; } set { _DataMatrimonio = value; } }

            public int? CSog { get; set; }
            #endregion public properties
        }

        public class DatiStatoCivile
        {
            public DatiStatoCivile()
            { }
            public DatiStatoCivile(System.Nullable<System.DateTime> decorrenza, char codice)
            {
                this._Decorrenza = decorrenza;
                this._Codice = codice;
            }
            #region private properties
            private System.Nullable<System.DateTime> _Decorrenza;

            private char _Codice;
            #endregion private properties

            #region public properties
            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

            public char Codice { get { return _Codice; } set { _Codice = value; } }
            #endregion public properties
        }

        public class DatiResidenzaEstero
        {
            public DatiResidenzaEstero()
            { }
            public DatiResidenzaEstero(System.Nullable<System.DateTime> decorrenza, string codiceCatastaleStatoEE)
            {
                this._Decorrenza = decorrenza;
                this._CodCatastaleStatoEE = codiceCatastaleStatoEE;
            }
            #region private properties
            private System.Nullable<System.DateTime> _Decorrenza;

            private string _CodCatastaleStatoEE;
            #endregion private properties

            #region public properties
            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

            public string CodCatastaleStatoEE { get { return _CodCatastaleStatoEE; } set { _CodCatastaleStatoEE = value; } }
            #endregion public properties
        }
        #endregion nested class

    }
}
