using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;
using System.Linq.Expressions;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAnniRichiestaBonus
    {
        public static bool GetAnniRichiestaBonus(long idPensione, out List<DatiAnniRichiestaBonus> datiAnniRichiestaBonus)
        {
            datiAnniRichiestaBonus = null;
            List<AnniRichiestaBonus> anniRichiestaBonusDB = null;
            DAGestioneAnniRichiestaBonus.GetAnniRichiestaBonusByIdPensione(idPensione, out anniRichiestaBonusDB);

            if (anniRichiestaBonusDB != null && anniRichiestaBonusDB.Count > 0)
            {
                datiAnniRichiestaBonus = new List<DatiAnniRichiestaBonus>();
                foreach (AnniRichiestaBonus annoDB in anniRichiestaBonusDB)
                {
                    datiAnniRichiestaBonus.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneAnniRichiestaBonus.DatiAnniRichiestaBonus(annoDB));
                }
                return true;
            }
            return false;
        }

        public static void SalvaAnniRichiestaBonus(long IdPensione, List<DatiAnniRichiestaBonus> datiAnniRichiestaBonus)
        {
            List<AnniRichiestaBonus> anniRichestaBonus = new List<AnniRichiestaBonus>();
            GestioneQuadri.DatiQuadroRichiestaBonus datiRichiestaBonus = new GestioneQuadri.DatiQuadroRichiestaBonus();
            
            foreach (DatiAnniRichiestaBonus datiAnnoRichiestaBonus in datiAnniRichiestaBonus)
            {
                AnniRichiestaBonus anno = new AnniRichiestaBonus();
                Utility.ValorizzaOggetti(datiAnnoRichiestaBonus, anno);
                anniRichestaBonus.Add(anno);
            }


            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnniRichiestaBonus.SalvaAnniRichiestaBonus(anniRichestaBonus);

                datiRichiestaBonus.Tipo = 2;
                datiRichiestaBonus.TabRichiestaBonus = 2;
                GestioneQuadri.SalvaQuadroRichiestaBonus(IdPensione, datiRichiestaBonus);

                transactionScope.Complete();
            }
        }

        public static void EliminaAnniRichiestaBonusByIdPensione(long idPensione)
        {
            GestioneQuadri.DatiQuadroRichiestaBonus datiRichiestaBonus = new GestioneQuadri.DatiQuadroRichiestaBonus();

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnniRichiestaBonus.EliminaAnniRichiestaBonus(idPensione);

                datiRichiestaBonus.Tipo = 2;
                datiRichiestaBonus.TabRichiestaBonus = 0;
                GestioneQuadri.SalvaQuadroRichiestaBonus(idPensione, datiRichiestaBonus);
                transactionScope.Complete();
            }
        }

        public static bool GetPrenotazioneElaborazioni(long idPensione, out List<DatiPrenotazioneElaborazioni> datiPrenotazioneElaborazioni)
        {
            datiPrenotazioneElaborazioni = null;
            List<PrenotazioneElaborazioni> prenotazioneElaborazioniDB = null;
            DAGestioneAnniRichiestaBonus.GetPrenotazioneElaborazioniByIdPensione(idPensione, out prenotazioneElaborazioniDB);

            if (prenotazioneElaborazioniDB != null && prenotazioneElaborazioniDB.Count > 0)
            {
                datiPrenotazioneElaborazioni = new List<DatiPrenotazioneElaborazioni>();
                foreach (PrenotazioneElaborazioni prenotazioneDB in prenotazioneElaborazioniDB)
                {
                    datiPrenotazioneElaborazioni.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneAnniRichiestaBonus.DatiPrenotazioneElaborazioni(prenotazioneDB));
                }
                return true;
            }
            return false;
        }

        public static void SalvaPrenotazioneElaborazioni(long IdPensione, List<DatiPrenotazioneElaborazioni> datiPrenotazioneElaborazioni)
        {
            List<PrenotazioneElaborazioni> prenotazioneElaborazioni = new List<PrenotazioneElaborazioni>();

            foreach (DatiPrenotazioneElaborazioni datiPrenotazioneElaborazione in datiPrenotazioneElaborazioni)
            {
                PrenotazioneElaborazioni prenotazioneElaborazione = new PrenotazioneElaborazioni();
                Utility.ValorizzaOggetti(datiPrenotazioneElaborazione, prenotazioneElaborazione);
                prenotazioneElaborazioni.Add(prenotazioneElaborazione);
            }


            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneAnniRichiestaBonus.SalvaPrenotazioneElaborazioni(prenotazioneElaborazioni);

                transactionScope.Complete();
            }
        }

        #region nested class

        public class DatiAnniRichiestaBonus
        {
            public DatiAnniRichiestaBonus()
            { }

            public DatiAnniRichiestaBonus(AnniRichiestaBonus anniRichiestaBonus)
            {
                this._Id = anniRichiestaBonus.Id;
                this.IdPensione = anniRichiestaBonus.IdPensione;
                this.Anno = anniRichiestaBonus.Anno;
                this.Prescrizione = anniRichiestaBonus.Prescrizione;
                this.CodiceEsitoMessaggio = anniRichiestaBonus.CodiceEsitoMessaggio;
                this.DescrizioneEsitoMessaggio = anniRichiestaBonus.DescrizioneEsitoMessaggio;
                this.EsitoCalcoloBeneficio = anniRichiestaBonus.EsitoCalcoloBeneficio;
                this.IsRichiestaBonus = anniRichiestaBonus.IsRichiestaBonus;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int Anno { get { return _Anno; } set { _Anno = value; } }
            public byte Prescrizione { get { return _Prescrizione; } set { _Prescrizione = value; } }
            public string CodiceEsitoMessaggio { get { return _CodiceEsitoMessaggio; } set { _CodiceEsitoMessaggio = value; } }
            public string DescrizioneEsitoMessaggio { get { return _DescrizioneEsitoMessaggio; } set { _DescrizioneEsitoMessaggio = value; } }
            public string EsitoCalcoloBeneficio { get { return _EsitoCalcoloBeneficio; } set { _EsitoCalcoloBeneficio = value; } }
            public bool IsRichiestaBonus { get { return _IsRichiestaBonus; } set { _IsRichiestaBonus = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private long _IdPensione;
            private int _Anno;
            private byte _Prescrizione;
            private string _CodiceEsitoMessaggio;
            private string _DescrizioneEsitoMessaggio;
            private string _EsitoCalcoloBeneficio;
            private bool _IsRichiestaBonus;

            #endregion private properties
        }

        public class DatiPrenotazioneElaborazioni
        {
            public DatiPrenotazioneElaborazioni()
            { }

            public DatiPrenotazioneElaborazioni(PrenotazioneElaborazioni prenotazioneElaborazioni)
            {
                this._Id = prenotazioneElaborazioni.Id;
                this.IdPensione = prenotazioneElaborazioni.IdPensione;
                this.AnnoRichiesto = prenotazioneElaborazioni.AnnoRichiesto;
                this.DataInserimento = prenotazioneElaborazioni.DataInserimento;
                this.DecorrenzaPresaInCarico = prenotazioneElaborazioni.DecorrenzaPresaInCarico;
                this.DescrizioneEsito = prenotazioneElaborazioni.DescrizioneEsito;
                this.EsitoCalcoloBeneficio = prenotazioneElaborazioni.EsitoCalcoloBeneficio;
                this.TipoElaborazione = prenotazioneElaborazioni.TipoElaborazione;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int AnnoRichiesto { get { return _AnnoRichiesto; } set { _AnnoRichiesto = value; } }
            public DateTime? DataInserimento { get { return _DataInserimento; } set { _DataInserimento = value; } }
            public DateTime? DecorrenzaPresaInCarico { get { return _DecorrenzaPresaInCarico; } set { _DecorrenzaPresaInCarico = value; } }
            public string DescrizioneEsito { get { return _DescrizioneEsito; } set { _DescrizioneEsito = value; } }
            public int EsitoCalcoloBeneficio { get { return _EsitoCalcoloBeneficio; } set { _EsitoCalcoloBeneficio = value; } }
            public string TipoElaborazione { get { return _TipoElaborazione; } set { _TipoElaborazione = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private long _IdPensione;
            private int _AnnoRichiesto;
            private DateTime? _DataInserimento;
            private DateTime? _DecorrenzaPresaInCarico;
            private string _DescrizioneEsito;
            private int _EsitoCalcoloBeneficio;
            private string _TipoElaborazione;

            #endregion private properties
        }

        #endregion nested class
    }
}
