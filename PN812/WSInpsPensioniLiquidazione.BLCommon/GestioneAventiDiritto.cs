using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAventiDiritto
    {
        public static void GetAventiDirittoConAnagraficheByIdPensione(long idPensione, out List<AventiDiritto> listaAventiDiritto, out List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche)
        {
            listaAnagrafiche = new List<GestioneAnagrafica.DatiAnagrafici>();
            listaAventiDiritto = new List<AventiDiritto>();
            List<DataCommon.AventiDiritto> listaAventiDirittoDB = null;
            List<DataCommon.Anagrafica> listaAnagraficheDB = null;
            DataCommon.DAGestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(idPensione, out listaAventiDirittoDB, out listaAnagraficheDB);
            if (listaAventiDirittoDB != null && listaAventiDirittoDB.Count > 0)
            {
                foreach (var aventeDirittoDB in listaAventiDirittoDB)
                    listaAventiDiritto.Add(new AventiDiritto(aventeDirittoDB));
            }
            if (listaAnagraficheDB != null && listaAnagraficheDB.Count > 0)
            {
                foreach (var anagraficaDB in listaAnagraficheDB)
                {
                    // Salto le anagrafiche ripetute (relative ai casi di incongruenza sugli aventi diritto)
                    if (listaAnagrafiche.Exists(x=> x.CodiceFiscale == anagraficaDB.CodiceFiscale))
                        continue;

                    GestioneAnagrafica.DatiAnagrafici datiAnagrafici = new GestioneAnagrafica.DatiAnagrafici();
                    Utility.ValorizzaOggetti(anagraficaDB, datiAnagrafici);
                    listaAnagrafiche.Add(datiAnagrafici);
                }
            }
        }

        public static void GetAventiDirittoByIdPensione(long idPensione, out List<AventiDiritto> listaAventiDiritto)
        {
            listaAventiDiritto = new List<AventiDiritto>();
            List<DataCommon.AventiDiritto> listaAventiDirittoDB = null;
            DataCommon.DAGestioneAventiDiritto.GetAventiDirittoByIdPensione(idPensione, out listaAventiDirittoDB);
            if (listaAventiDirittoDB != null && listaAventiDirittoDB.Count > 0)
                foreach (var aventeDirittoDB in listaAventiDirittoDB)
                    listaAventiDiritto.Add(new AventiDiritto(aventeDirittoDB));
        }

        public static void GetAventiDirittoRecuperatoByIdPensione(long idPensione, string codiceFiscaleTitolare, out List<AventeDirittoRecuperato> listaAventiDirittoRecuperato)
        {
            listaAventiDirittoRecuperato = new List<AventeDirittoRecuperato>();

            List<DataCommon.AventiDiritto> listaAventiDirittoDB = null;
            List<DataCommon.Anagrafica> listaAnagraficheAventiDiritto = null;
            DataCommon.DAGestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(idPensione, out listaAventiDirittoDB, out listaAnagraficheAventiDiritto);

            List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventiDiritto = null;
            GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(idPensione, null, out listaPeriodiAventiDiritto);

            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheFamiliari = null;
            GestioneFamiliari.GetFamiliariByIdPensione(idPensione, out listaFamiliari, out listaAnagraficheFamiliari);

            if (listaAventiDirittoDB != null && listaAventiDirittoDB.Count > 0)
            {
                foreach (var aventeDirittoDB in listaAventiDirittoDB)
                {
                    AventeDirittoRecuperato aventeDirittoRecuperato = new AventeDirittoRecuperato();
                    Utility.ValorizzaOggetti(aventeDirittoDB, aventeDirittoRecuperato);
                    aventeDirittoRecuperato.ListaPeriodi = listaPeriodiAventiDiritto.FindAll(x => x.IdAventeDiritto == aventeDirittoDB.Id);
                    if (listaAnagraficheAventiDiritto != null && listaAnagraficheAventiDiritto.Count > 0)
                    {
                        DataCommon.Anagrafica anagraficaAventeDiritto = listaAnagraficheAventiDiritto.Find(x => x.Id == aventeDirittoDB.IdAnagrafica);
                        aventeDirittoRecuperato.CodiceFiscale = anagraficaAventeDiritto.CodiceFiscale;
                        aventeDirittoRecuperato.IsTitolare = anagraficaAventeDiritto.CodiceFiscale == codiceFiscaleTitolare;
                    }

                    if (aventeDirittoRecuperato.IsTitolare && listaFamiliari != null && listaFamiliari.Count > 0)
                    {
                        GestioneFamiliari.Familiare familiare = listaFamiliari.Find(x => x.IdAnagrafica == aventeDirittoDB.IdAnagrafica);
                        aventeDirittoRecuperato.ScadenzaRevisioneSanitaria = familiare.ScadenzaRevisioneSanitaria;
                    }

                    listaAventiDirittoRecuperato.Add(aventeDirittoRecuperato);
                }
            }
        }

        public static void SalvaAventeDiritto(AventiDiritto aventeDiritto)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (aventeDiritto != null)
                {
                    DataCommon.AventiDiritto aventeDirittoDB = new DataCommon.AventiDiritto();
                    Utility.ValorizzaOggetti(aventeDiritto, aventeDirittoDB);
                    DataCommon.DAGestioneAventiDiritto.SalvaAventeDiritto(aventeDirittoDB);
                    aventeDiritto.Id = aventeDirittoDB.Id;
                    transactionScope.Complete();
                }
            }
        }

        public static void DeleteAventeDirittoById(long idAventeDiritto)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.DAGestioneAventiDiritto.DeleteAventeDirittoById(idAventeDiritto);
                transactionScope.Complete();
            }
        }

        public static void SortAventiDiritto(string codiceFiscaleTitolare, ref List<AventiDiritto> listaAventiDiritto, List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche)
        {
            // Ordine
            // - Titolare
            // - Coniuge
            // - Altri familiari ordinati per data nascita

            // ordinamento per data nascita
            listaAventiDiritto = (from ad in listaAventiDiritto
                                  join an in listaAnagrafiche on ad.IdAnagrafica equals an.Id
                                  orderby an.DataNascita
                                  select ad).ToList();

            // inserimento primo posto il Coniuge
            GestioneAventiDiritto.AventiDiritto coniuge = listaAventiDiritto.Find(x => (x.DecParentelaDA.HasValue && x.DecParentelaDA.Value == 'C'));
            if (coniuge != null)
            {
                listaAventiDiritto.Remove(coniuge);
                listaAventiDiritto.Insert(0, coniuge);
            }

            // inserimento primo posto il Titolare
            GestioneAnagrafica.DatiAnagrafici anagraficaTitolare = listaAnagrafiche.Find(x => x.CodiceFiscale == codiceFiscaleTitolare);
            // per un motivo X il titolare potrebbe non essere presente tra gli Aventi Diritto
            if (anagraficaTitolare != null)
            {
                GestioneAventiDiritto.AventiDiritto titolare = listaAventiDiritto.Find(x => x.IdAnagrafica == anagraficaTitolare.Id);
                if (titolare != null)
                {
                    listaAventiDiritto.Remove(titolare);
                    listaAventiDiritto.Insert(0, titolare);
                }
            }
        }

        #region nested class
        public class AventiDiritto
        {
            private string _TipoUnione;

            public AventiDiritto()
            { }

            public AventiDiritto(DataCommon.AventiDiritto aventeDiritto)
            {
                this.Id = aventeDiritto.Id;
                this.IdPensione = aventeDiritto.IdPensione;
                this.IdAnagrafica = aventeDiritto.IdAnagrafica;
                this.DecParentelaDA = aventeDiritto.DecParentelaDA;
                this.NucleoTitolare = aventeDiritto.NucleoTitolare;
                this.PresenzaWebDom = aventeDiritto.PresenzaWebDom;
                this.PresenzaGP = aventeDiritto.PresenzaGP;
                this.CategoriaPensione = aventeDiritto.CategoriaPensione;
                this.SedePensione = aventeDiritto.SedePensione;
                this.CertificatoPensione = aventeDiritto.CertificatoPensione;
                this.DataMatrimonio = aventeDiritto.DataMatrimonio;
                this.CSog = aventeDiritto.CSog;
                this.CodiceNucleoFromGP = aventeDiritto.CodiceNucleoFromGP;
                this.CodiceNucleo = aventeDiritto.CodiceNucleo;
                this.TipoUnione = aventeDiritto.TipoUnione;
            }

            public long Id { get; set; }
            public long IdPensione { get; set; }
            public long IdAnagrafica { get; set; }
            public char? DecParentelaDA { get; set; }
            public bool NucleoTitolare { get; set; }
            public bool PresenzaWebDom { get; set; }
            public bool PresenzaGP { get; set; }
            public string CategoriaPensione { get; set; }
            public short? SedePensione { get; set; }
            public int? CertificatoPensione { get; set; }
            public DateTime? DataMatrimonio { get; set; }
            public int? CSog { get; set; }
            public bool? IsSelezionato { get; set; }
            public string CodiceNucleoFromGP { get; set; }
            public string CodiceNucleo { get; set; }
            //public string Nucleo { get; set; }
            public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }
            public List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> ListaPeriodi { get; set; }
        }

        public class AventeDirittoRecuperato : AventiDiritto
        {
            public string CodiceFiscale { get; set; }
            public bool IsTitolare { get; set; }
            public decimal? PercGiudice { get; set; }
            public DateTime? ScadenzaRevisioneSanitaria { get; set; }
        }
        #endregion nested class
    }
}
