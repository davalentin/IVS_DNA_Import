using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestionePensione
    {
        public static void GetPensioneByNumeroDomandaAndProg(Int64 numeroDomanda, byte? progStorico, out DatiPensione datiPensione)
        {
            Pensione pensione = null;
            datiPensione = null;

            Expression<Func<Pensione, bool>> whereCondition = (p) => true;

            if (progStorico.HasValue)
                whereCondition = whereCondition.And(p => p.ProgStorico == progStorico);
            else
                whereCondition = whereCondition.And(p => !p.ProgStorico.HasValue);

            DAGestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, whereCondition, out pensione);
            if (pensione == null)
                return;
            datiPensione = new DatiPensione();
            Utility.ValorizzaOggetti(pensione, datiPensione);
        }

        public static void GetPensioniByNumeroDomanda(Int64 numeroDomanda, out List<DatiPensione> elencoPensioni)
        {
            List<Pensione> pensioni = null;
            elencoPensioni = null;
            DAGestionePensione.GetPensioniByNumeroDomanda(numeroDomanda, out pensioni);
            if (pensioni == null || pensioni.Count == 0)
                return;
            elencoPensioni = new List<DatiPensione>();
            foreach (var pensione in pensioni)
            {
                DatiPensione datiPensione = new DatiPensione();
                Utility.ValorizzaOggetti(pensione, datiPensione);
                elencoPensioni.Add(datiPensione);
            }
        }

        public static void GetIdPensioneByNumeroDomanda(Int64 numeroDomanda, byte? progStorico, out long idPensione)
        {
            idPensione = 0;

            Expression<Func<Pensione, bool>> whereCondition = (p) => true;

            if (progStorico.HasValue)
                whereCondition = whereCondition.And(p => p.ProgStorico == progStorico);
            else
                whereCondition = whereCondition.And(p => !p.ProgStorico.HasValue);

            DAGestionePensione.GetIdPensioneByNumeroDomandaAndProg(numeroDomanda, whereCondition, out idPensione);
        }

        public static void GetPensioniByCodiceFiscale(string codiceFiscale, out List<DatiPensione> elencoPensioni)
        {
            List<Pensione> pensioni = null;
            elencoPensioni = null;
            DAGestionePensione.GetPensioniByCodiceFiscale(codiceFiscale, out pensioni);
            if (pensioni == null || pensioni.Count == 0)
                return;
            elencoPensioni = new List<DatiPensione>();
            foreach (Pensione p in pensioni)
            {
                DatiPensione datiPensione = new DatiPensione();
                Utility.ValorizzaOggetti(p, datiPensione);

                string filtro = string.Empty;

                //Ai fini della gestione delle “Tipologie non abilitate” il “codice tipo richiesta” valorizzato a null dovrà corrispondere funzionalmente al valore ALL del campo filtro
                if (datiPensione.CodiceTipoRichiesta == null)
                    filtro = "ALL";
                else
                    filtro = datiPensione.GetFiltro();

                if (filtro == null)
                    filtro = string.Empty;

                //Controllo domande lavorabili
                Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                Utility.TipoFondo? fondo = Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria);
                if (GestionePensione.IsDomandaLavorabile(tipoApp, fondo, datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo, filtro, datiPensione.SiglaCategoria,
                    Utility.IsDomandaINPDAP(datiPensione.Gestione)))
                    elencoPensioni.Add(datiPensione);

                //elencoPensioni.Add(datiPensione);
            }
        }

        public static void GetPatronatoByIdPensione(Int64 idPensione, out DatiPatronato datiPatronato)
        {
            Patronato patronato = null;
            datiPatronato = null;
            DAGestionePensione.GetPatronatoByIdPensione(idPensione, out patronato);
            if (patronato == null)
                return;
            datiPatronato = new DatiPatronato();
            Utility.ValorizzaOggetti(patronato, datiPatronato);
        }

        public static void GetSindacatoByIdPensione(Int64 idPensione, out DatiSindacato datiSindacato)
        {
            Sindacato sindacato = null;
            datiSindacato = null;
            DAGestionePensione.GetSindacatoByIdPensione(idPensione, out sindacato);
            if (sindacato == null)
                return;
            datiSindacato = new DatiSindacato();
            Utility.ValorizzaOggetti(sindacato, datiSindacato);
        }

        public static void GetEliminazioneByIdPensione(Int64 idPensione, out DatiEliminazione datiEliminazione)
        {
            Eliminazione eliminazione = null;
            datiEliminazione = null;
            DAGestionePensione.GetEliminazioneByIdPensione(idPensione, out eliminazione);
            if (eliminazione == null)
                return;
            datiEliminazione = new DatiEliminazione();
            Utility.ValorizzaOggetti(eliminazione, datiEliminazione);
        }

        public static void GetTitolareByIdPensione(Int64 idPensione, out DatiTitolare datiTitolare)
        {
            Titolare titolare = null;
            datiTitolare = null;
            DAGestionePensione.GetTitolareByIdPensione(idPensione, out titolare);
            if (titolare == null)
                return;
            datiTitolare = new DatiTitolare();
            Utility.ValorizzaOggetti(titolare, datiTitolare);
        }

        public static void GetPensioneByChiavePensione(string siglaCategoria, short sede, int certificato, Utility.TipoAppartenenza? tipoAppartenenza, out List<DatiPensione> elencoDatiPensioni)
        {
            List<Pensione> elencoPensioni = null;
            elencoDatiPensioni = null;
            if (!tipoAppartenenza.HasValue)
                DAGestionePensione.GetPensioneByChiavePensione(siglaCategoria, sede, certificato, out elencoPensioni);
            else
            {
                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.FS:
                        DAGestionePensione.GetPensioneByChiavePensionePerFondo(siglaCategoria, sede, certificato, out elencoPensioni);
                        break;
                    default:
                        DAGestionePensione.GetPensioneByChiavePensione(siglaCategoria, sede, certificato, out elencoPensioni);
                        break;
                }
            }
            if (elencoPensioni == null || elencoPensioni.Count == 0)
                return;
            elencoDatiPensioni = new List<DatiPensione>();
            foreach (Pensione p in elencoPensioni)
            {
                DatiPensione dP = new DatiPensione();
                Utility.ValorizzaOggetti(p, dP);
                elencoDatiPensioni.Add(dP);
            }
        }

        public static void SalvaPensione(DatiPensione datiPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Pensione pensione = new Pensione();
                Utility.ValorizzaOggetti(datiPensione, pensione);
                DAGestionePensione.SalvaPensione(pensione);
                transactionScope.Complete();

                datiPensione.Id = pensione.Id;
            }
        }

        public static void SalvaPatronato(long idPensione, DatiPatronato datiPatronato)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Patronato patronato = new Patronato();
                Utility.ValorizzaOggetti(datiPatronato, patronato);
                patronato.IdPensione = idPensione;
                DAGestionePensione.SalvaPatronato(patronato);
                transactionScope.Complete();
            }
        }

        public static void SalvaSindacato(long idPensione, DatiSindacato datiSindacato)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Sindacato sindacato = new Sindacato();
                Utility.ValorizzaOggetti(datiSindacato, sindacato);
                sindacato.IdPensione = idPensione;
                DAGestionePensione.SalvaSindacato(sindacato);
                transactionScope.Complete();
            }
        }

        public static void SalvaTitolare(DatiTitolare datiTitolare)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Titolare titolare = new Titolare();
                Utility.ValorizzaOggetti(datiTitolare, titolare);
                DAGestionePensione.SalvaTitolare(titolare);
                transactionScope.Complete();
            }
        }

        public static void SalvaEliminazione(long idPensione, DatiEliminazione datiEliminazione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                Eliminazione eliminazione = new Eliminazione();
                Utility.ValorizzaOggetti(datiEliminazione, eliminazione);
                eliminazione.IdPensione = idPensione;
                DAGestionePensione.SalvaEliminazione(eliminazione);
                transactionScope.Complete();
            }
        }

        public static void EliminaPatronato(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensione.EliminaPatronato(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaSindacati(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensione.EliminaSindacati(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaEliminazione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensione.EliminaEliminazione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensione.EliminaPensione(idPensione);
                transactionScope.Complete();
            }
        }

        /*Gestione Log Cancellazione*/
        public static void EliminaPensione(long idPensione, Int64 NumDomanda, bool scriviLog)
        {
            try
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    string Parametri = "idPensione: " + idPensione + " NumDomanda: " + NumDomanda + " scriviLog: " + scriviLog.ToString();
                    if (scriviLog) GestioneLogGenerico.SalvaLogGenerico(NumDomanda, "BLCommon.GestionePensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "Inizio EliminaPensione Dentro Gestione", Parametri, string.Empty);
                    DAGestionePensione.EliminaPensione(idPensione, NumDomanda, scriviLog);
                    if (scriviLog) GestioneLogGenerico.SalvaLogGenerico(NumDomanda, "BLCommon.GestionePensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "Fine Dentro Gestione", Parametri, string.Empty);
                    transactionScope.Complete();
                }
            }
            catch (SqlException ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(NumDomanda, "BLCommon.GestionePensione.EliminaPensione", Utility.TipoLogGenerico.ErroreApplicativo, "SqlException: " + ex.Message, string.Empty, ex.StackTrace);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(NumDomanda, "BLCommon.GestionePensione.EliminaPensione", Utility.TipoLogGenerico.ErroreApplicativo, "InvalidOperationException: " + ex.Message, string.Empty, ex.StackTrace);
                throw;
            }
            catch (TimeoutException ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(NumDomanda, "BLCommon.GestionePensione.EliminaPensione", Utility.TipoLogGenerico.ErroreApplicativo, "TimeoutException: " + ex.Message, string.Empty, ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                GestioneLogGenerico.SalvaLogGenerico(NumDomanda, "BLCommon.GestionePensione.EliminaPensione", Utility.TipoLogGenerico.ErroreApplicativo, "Eccezione generica: " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), string.Empty, ex.StackTrace);
                throw;
            }

        }
        /**/

        public static bool IsDomandaLavorabile(Utility.TipoAppartenenza? tipoApp, Utility.TipoFondo? fondo, string gruppo, string prodotto, string tipo, string filtro, string siglaCategoria, bool isINPDAP)
        {
            string tApp = tipoApp.HasValue ? tipoApp.ToString() : null;
            string tFondo = isINPDAP ? "INPDAP" : fondo.HasValue ? fondo.ToString() : null;

            return DAGestionePensione.IsDomandaLavorabile(tApp, tFondo, gruppo, prodotto, tipo, filtro, siglaCategoria);
        }

        public static void GetByPassCancellazione(long nDomus, short codiceSede, byte centroOperativo, string siglaCategoria, out DatiByPassCancellazione datiByPassCancellazione)
        {
            ByPassCancellazione byPassCancellazione = null;
            datiByPassCancellazione = null;
            DAGestionePensione.GetByPassCancellazione(nDomus, codiceSede, centroOperativo, siglaCategoria, out byPassCancellazione);
            if (byPassCancellazione == null)
                return;
            datiByPassCancellazione = new DatiByPassCancellazione();
            Utility.ValorizzaOggetti(byPassCancellazione, datiByPassCancellazione);
        }

        public static void EliminaByPassCancellazione(DatiByPassCancellazione datiByPassCancellazione)
        {
            ByPassCancellazione byPassCancellazione = new ByPassCancellazione();
            Utility.ValorizzaOggetti(datiByPassCancellazione, byPassCancellazione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensione.DeleteByPassCancellazione(byPassCancellazione);
                transactionScope.Complete();
            }
        }

        public static void SalvaByPassCancellazione(DatiByPassCancellazione datiByPassCancellazione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                ByPassCancellazione byPassCancellazione = new ByPassCancellazione();
                Utility.ValorizzaOggetti(datiByPassCancellazione, byPassCancellazione);
                DAGestionePensione.SalvaByPassCancellazione(byPassCancellazione);
                transactionScope.Complete();
            }
        }

        public static void UpdateProgStorico(long numeroDomanda)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensione.UpdateProgStorico(numeroDomanda);
                transactionScope.Complete();
            }
        }

        //ENG - RIC Reversibilita ENPALS
        public static DateTime? GetPLReversibilitaEnpals(string siglacategoria, int numeroCertificato, long idAnagraficaTitolare)
        {
            DateTime? scadenzaBeneficio = null;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {

                scadenzaBeneficio = DAGestionePensione.GetPLReversibilitaEnpals(siglacategoria, numeroCertificato, idAnagraficaTitolare);
                transactionScope.Complete();
            }

            return scadenzaBeneficio;
        }

        #region nested class

        public class DatiPensione
        {
            public DatiPensione()
            { }
            public DatiPensione(long nDomus, System.Nullable<System.DateTime> decorrenzaOriginaria, System.Nullable<System.DateTime> dataPerfezionamentoRequisiti,
                System.Nullable<short> codiceSedeDestinazione, bool? lavoratorePubblico, byte? numFigli, byte? sceltaLM, System.Nullable<System.DateTime> dataCondizioniPerComputo, int? NCertificato)
            {
                this._NDomus = nDomus;
                this._DecorrenzaOriginaria = decorrenzaOriginaria;
                this._DataPerfezionamentoRequisiti = dataPerfezionamentoRequisiti;
                this._CodiceSedeDestinazione = codiceSedeDestinazione;
                this.LavoratorePubblico = lavoratorePubblico;
                this._NumeroFigli = numFigli;
                this._SceltaLavMadri = sceltaLM;
                this._DataCondizioniPerComputo = dataCondizioniPerComputo;
                this._NCertificato = NCertificato;
            }
            #region private properties
            private long _Id;

            private long _NDomus;

            private byte? _ProgStorico;

            private string _SiglaCategoria;

            private short _CodiceSede;

            private System.Nullable<short> _CodiceSedeDestinazione;

            private System.Nullable<int> _NCertificato;

            private System.Nullable<int> _NCertificatoProvvisorio;

            private System.DateTime _DataPresentazioneDomanda;

            private char _TipoElaborazione;

            private System.Nullable<System.DateTime> _DecorrenzaOriginaria;

            private System.Nullable<System.DateTime> _DecorrenzaOriginariaPrima;

            private string _NaturaPensione;

            private System.Nullable<byte> _TipoCalcolo;

            private System.Nullable<byte> _CausaCarico;

            private System.Nullable<byte> _CodiceArretrati;

            private System.Nullable<int> _AttivitaEconomica;

            private System.Nullable<int> _ProfessioneIndividuale;

            private System.Nullable<int> _AttivitaEconomicaFELPE;

            private System.Nullable<int> _ProfessioneIndividualeFELPE;

            private System.Nullable<System.DateTime> _DataInteressiLegali;

            private System.Nullable<System.DateTime> _DataCompletezza;

            private System.Nullable<System.DateTime> _DecorrenzaCalcoloArretratiNUTS;

            private System.Nullable<System.DateTime> _InizioAssicurazione;

            private System.Nullable<System.DateTime> _FineAssicurazione;

            private System.Nullable<System.DateTime> _DataPerfezionamentoRequisiti;

            private System.Nullable<bool> _RequisitiVecchiaiaAl1294;

            private System.Nullable<bool> _RequisitiAl1294;

            private System.Nullable<bool> _RequisitiAl996;

            private System.Nullable<System.DateTime> _DataInizioCalcolo;

            private System.Nullable<bool> _RequisitiLegge50392Art2;

            private System.Nullable<bool> _AccertamentoAutomatico;

            private System.Nullable<decimal> _AliquotaTFREsodati;

            private System.Nullable<System.DateTime> _DecorrenzaCalcoloArretrati;

            private string _MatricolaUtenteAcquisizione;

            private System.Nullable<byte> _Isola;

            private string _CodiceProcedura;

            private System.Nullable<bool> _FlagInCalcolo;

            private System.Nullable<byte> _CentroOperativo;

            private System.Nullable<byte> _CentroOperativoDestinazione;

            private System.Nullable<System.DateTime> _DataAcquisizione;

            private System.Nullable<System.DateTime> _DataElaborazione;

            private System.Nullable<bool> _FlagVerify;

            private System.Nullable<char> _Versione;

            private System.Nullable<char> _AggancioQred;

            private System.Nullable<short> _CodiceBancaEsodati;

            private System.Nullable<char> _AttivitaConcorrenzialeEsodante;

            private System.Nullable<System.DateTime> _DataRicostituzione;

            private System.Nullable<System.DateTime> _DataRicezionePrenotazioneCentrale;

            private System.Nullable<System.DateTime> _DataPrimaDomanda;

            private System.Nullable<byte> _StatoPensione;

            private System.Nullable<bool> _TrasformazioneAOI;

            private System.Nullable<bool> _AgevolazioniLegge;

            private System.Nullable<bool> _ExCombattente;

            private string _Gruppo;

            private string _Prodotto;

            private string _Tipo;

            private string _Gestione;

            private string _Fondo;

            private string _Ente;

            private System.Nullable<bool> _FlagUnicarpe;

            private System.Nullable<char> _TipoLetturaUnicarpe;

            private System.Nullable<bool> _IndConvInt;

            private string _CodiceTipoRichiesta;

            private bool? _Benefici;

            private DateTime? _DataPerfezionamentoRequisitiUnicarpe;

            private bool? _Maggiorazioni;

            private char? _Contributivo;

            private bool? _Amianto181Unicarpe;

            private string _Filtro;

            private bool? _IsDatiENPALSRecuperati;

            private long? _NDomusPrincipale;

            private string _LinkIntranet;

            private DateTime? _DataTentativoCalcoloDefinitivo;

            private string _CodCategoria;

            private byte? _IdTipoPLPerRIC;

            private byte? _NumeroFigli;

            private byte? _SceltaLavMadri;

            private System.Nullable<System.DateTime> _DataOpzione;

            private System.Nullable<System.DateTime> _DataRaggiungimentoOpzione;

            private System.Nullable<bool> _IsRichiestaBonus;

            private string _AnnoDecorrenzaBonus;

            private System.Nullable<bool> _IsDatiAggiuntiviFromJSON;

            private System.Nullable<System.DateTime> _DataCondizioniPerComputo;

            private System.Nullable<char> _Flag5000;

            private string _DirittoAutonomo;

            private bool? _IsPLInvalidita;

            private bool? _IsRicRinnovata;

            private bool? _IsRicExtracalcolo;

            private byte? _TipoAutomazione;

            private byte? _Ante96ByDatiCalcolo;

            private System.Nullable<System.DateTime> _MaxDecDatiCalcoloAnte96;

            private bool? _IsTentataAutomazione;

            private string _Caratterizzazione;

            private System.Nullable<int> _CodProPE;

            //ENG - Memo 57_2023
            private string _AnnoMonitoraggio;

            private string _GP1AV91B;

            private bool? _IsNuovoCalcolo;

            //ENG - Aggiornamento Memo INPGI
            private string _GP1AJ11;

            private DateTime? _DataEstrazioneRata;

            private int? _IdNota;

            private bool? _SbloccaPannelliAnte96;

            private string _FlagIndebito;

            //ENG - Spacchettate AGO
            private char? _GP1AJSP;

            //ENG - Implementazione Meta Processo
            private short? _CodiceSedeLavorazione;

            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public long NDomus { get { return _NDomus; } set { _NDomus = value; } }

            public byte? ProgStorico { get { return _ProgStorico; } set { _ProgStorico = value; } }

            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }

            public short CodiceSede { get { return _CodiceSede; } set { _CodiceSede = value; } }

            public System.Nullable<short> CodiceSedeDestinazione { get { return _CodiceSedeDestinazione; } set { _CodiceSedeDestinazione = value; } }

            public System.Nullable<int> NCertificato { get { return _NCertificato; } set { _NCertificato = value; } }

            public System.Nullable<int> NCertificatoProvvisorio { get { return _NCertificatoProvvisorio; } set { _NCertificatoProvvisorio = value; } }

            public System.DateTime DataPresentazioneDomanda { get { return _DataPresentazioneDomanda; } set { _DataPresentazioneDomanda = value; } }

            public char TipoElaborazione { get { return _TipoElaborazione; } set { _TipoElaborazione = value; } }

            public System.Nullable<System.DateTime> DecorrenzaOriginaria { get { return _DecorrenzaOriginaria; } set { _DecorrenzaOriginaria = value; } }

            public System.Nullable<System.DateTime> DecorrenzaOriginariaPrima { get { return _DecorrenzaOriginariaPrima; } set { _DecorrenzaOriginariaPrima = value; } }

            public string NaturaPensione { get { return _NaturaPensione; } set { _NaturaPensione = value; } }

            public System.Nullable<byte> TipoCalcolo { get { return _TipoCalcolo; } set { _TipoCalcolo = value; } }

            public System.Nullable<byte> CausaCarico { get { return _CausaCarico; } set { _CausaCarico = value; } }

            public System.Nullable<byte> CodiceArretrati { get { return _CodiceArretrati; } set { _CodiceArretrati = value; } }

            public System.Nullable<int> AttivitaEconomica { get { return _AttivitaEconomica; } set { _AttivitaEconomica = value; } }

            public System.Nullable<int> ProfessioneIndividuale { get { return _ProfessioneIndividuale; } set { _ProfessioneIndividuale = value; } }

            public System.Nullable<int> AttivitaEconomicaFELPE { get { return _AttivitaEconomicaFELPE; } set { _AttivitaEconomicaFELPE = value; } }

            public System.Nullable<int> ProfessioneIndividualeFELPE { get { return _ProfessioneIndividualeFELPE; } set { _ProfessioneIndividualeFELPE = value; } }

            public System.Nullable<System.DateTime> DataInteressiLegali { get { return _DataInteressiLegali; } set { _DataInteressiLegali = value; } }

            public System.Nullable<System.DateTime> DataCompletezza { get { return _DataCompletezza; } set { _DataCompletezza = value; } }

            public System.Nullable<System.DateTime> DecorrenzaCalcoloArretratiNUTS { get { return _DecorrenzaCalcoloArretratiNUTS; } set { _DecorrenzaCalcoloArretratiNUTS = value; } }

            public System.Nullable<System.DateTime> InizioAssicurazione { get { return _InizioAssicurazione; } set { _InizioAssicurazione = value; } }

            public System.Nullable<System.DateTime> FineAssicurazione { get { return _FineAssicurazione; } set { _FineAssicurazione = value; } }

            public System.Nullable<System.DateTime> DataPerfezionamentoRequisiti { get { return _DataPerfezionamentoRequisiti; } set { _DataPerfezionamentoRequisiti = value; } }

            public System.Nullable<bool> RequisitiVecchiaiaAl1294 { get { return _RequisitiVecchiaiaAl1294; } set { _RequisitiVecchiaiaAl1294 = value; } }

            public System.Nullable<bool> RequisitiAl1294 { get { return _RequisitiAl1294; } set { _RequisitiAl1294 = value; } }

            public System.Nullable<bool> RequisitiAl996 { get { return _RequisitiAl996; } set { _RequisitiAl996 = value; } }

            public System.Nullable<System.DateTime> DataInizioCalcolo { get { return _DataInizioCalcolo; } set { _DataInizioCalcolo = value; } }

            public System.Nullable<bool> RequisitiLegge50392Art2 { get { return _RequisitiLegge50392Art2; } set { _RequisitiLegge50392Art2 = value; } }

            public System.Nullable<bool> AccertamentoAutomatico { get { return _AccertamentoAutomatico; } set { _AccertamentoAutomatico = value; } }

            public System.Nullable<decimal> AliquotaTFREsodati { get { return _AliquotaTFREsodati; } set { _AliquotaTFREsodati = value; } }

            public System.Nullable<System.DateTime> DecorrenzaCalcoloArretrati { get { return _DecorrenzaCalcoloArretrati; } set { _DecorrenzaCalcoloArretrati = value; } }

            public string MatricolaUtenteAcquisizione { get { return _MatricolaUtenteAcquisizione; } set { _MatricolaUtenteAcquisizione = value; } }

            public System.Nullable<byte> Isola { get { return _Isola; } set { _Isola = value; } }

            public string CodiceProcedura { get { return _CodiceProcedura; } set { _CodiceProcedura = value; } }

            public System.Nullable<bool> FlagInCalcolo { get { return _FlagInCalcolo; } set { _FlagInCalcolo = value; } }

            public System.Nullable<byte> CentroOperativo { get { return _CentroOperativo; } set { _CentroOperativo = value; } }

            public System.Nullable<byte> CentroOperativoDestinazione { get { return _CentroOperativoDestinazione; } set { _CentroOperativoDestinazione = value; } }

            public System.Nullable<System.DateTime> DataAcquisizione { get { return _DataAcquisizione; } set { _DataAcquisizione = value; } }

            public System.Nullable<System.DateTime> DataElaborazione { get { return _DataElaborazione; } set { _DataElaborazione = value; } }

            public System.Nullable<bool> FlagVerify { get { return _FlagVerify; } set { _FlagVerify = value; } }

            public System.Nullable<char> Versione { get { return _Versione; } set { _Versione = value; } }

            public System.Nullable<char> AggancioQred { get { return _AggancioQred; } set { _AggancioQred = value; } }

            public System.Nullable<short> CodiceBancaEsodati { get { return _CodiceBancaEsodati; } set { _CodiceBancaEsodati = value; } }

            public System.Nullable<char> AttivitaConcorrenzialeEsodante { get { return _AttivitaConcorrenzialeEsodante; } set { _AttivitaConcorrenzialeEsodante = value; } }

            public System.Nullable<System.DateTime> DataRicostituzione { get { return _DataRicostituzione; } set { _DataRicostituzione = value; } }

            public System.Nullable<System.DateTime> DataRicezionePrenotazioneCentrale { get { return _DataRicezionePrenotazioneCentrale; } set { _DataRicezionePrenotazioneCentrale = value; } }

            public System.Nullable<System.DateTime> DataPrimaDomanda { get { return _DataPrimaDomanda; } set { _DataPrimaDomanda = value; } }

            public System.Nullable<byte> StatoPensione { get { return _StatoPensione; } set { _StatoPensione = value; } }

            public System.Nullable<bool> TrasformazioneAOI { get { return _TrasformazioneAOI; } set { _TrasformazioneAOI = value; } }

            public System.Nullable<bool> AgevolazioniLegge { get { return _AgevolazioniLegge; } set { _AgevolazioniLegge = value; } }

            public System.Nullable<bool> ExCombattente { get { return _ExCombattente; } set { _ExCombattente = value; } }

            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }

            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }

            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public string Gestione { get { return _Gestione; } set { _Gestione = value; } }

            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

            public string Ente { get { return _Ente; } set { _Ente = value; } }

            public System.Nullable<bool> FlagUnicarpe { get { return _FlagUnicarpe; } set { _FlagUnicarpe = value; } }

            public System.Nullable<char> TipoLetturaUnicarpe { get { return _TipoLetturaUnicarpe; } set { _TipoLetturaUnicarpe = value; } }

            public System.Nullable<bool> IndConvInt { get { return _IndConvInt; } set { _IndConvInt = value; } }

            public string CodiceTipoRichiesta { get { return _CodiceTipoRichiesta; } set { _CodiceTipoRichiesta = value; } }

            public bool? Benefici { get { return _Benefici; } set { _Benefici = value; } }

            public DateTime? DataPerfezionamentoRequisitiUnicarpe { get { return _DataPerfezionamentoRequisitiUnicarpe; } set { _DataPerfezionamentoRequisitiUnicarpe = value; } }

            public bool? Maggiorazioni { get { return _Maggiorazioni; } set { _Maggiorazioni = value; } }

            public char? Contributivo { get { return _Contributivo; } set { _Contributivo = value; } }

            public bool? Amianto181Unicarpe { get { return _Amianto181Unicarpe; } set { _Amianto181Unicarpe = value; } }

            public bool? IsDatiENPALSRecuperati { get { return _IsDatiENPALSRecuperati; } set { _IsDatiENPALSRecuperati = value; } }

            public long? NDomusPrincipale { get { return _NDomusPrincipale; } set { _NDomusPrincipale = value; } }

            public string GetFiltro()
            {
                if (_Filtro == null)
                    _Filtro = Utility.GetFiltroByCodTipoRichiesta(_CodiceTipoRichiesta);
                return _Filtro;
            }

            public bool? IsCumuloAutomatica { get; set; }

            public bool? IsTotAutomatica { get; set; }

            public string LinkIntranet { get { return _LinkIntranet; } set { _LinkIntranet = value; } }

            public DateTime? DataTentativoCalcoloDefinitivo { get { return _DataTentativoCalcoloDefinitivo; } set { _DataTentativoCalcoloDefinitivo = value; } }

            public string GetCodCategoria()
            {
                if (string.IsNullOrEmpty(_CodCategoria))
                    GestioneDecodifica.GetCodCategoriaBySiglaCategoria(_SiglaCategoria, out _CodCategoria);
                return _CodCategoria;
            }

            public bool? LavoratorePubblico { get; set; }

            public byte? IdTipoPLPerRIC { get { return _IdTipoPLPerRIC; } set { _IdTipoPLPerRIC = value; } }

            public bool? IsPLUnicarpe { get; set; }

            public byte? NumeroFigli { get { return _NumeroFigli; } set { _NumeroFigli = value; } }
            public byte? SceltaLavMadri { get { return _SceltaLavMadri; } set { _SceltaLavMadri = value; } }
            public System.Nullable<System.DateTime> DataOpzione { get { return _DataOpzione; } set { _DataOpzione = value; } }
            public System.Nullable<System.DateTime> DataRaggiungimentoOpzione { get { return _DataRaggiungimentoOpzione; } set { _DataRaggiungimentoOpzione = value; } }
            public byte? TipoFelpe { get; set; }
            public System.Nullable<bool> IsRichiestaBonus { get { return _IsRichiestaBonus; } set { _IsRichiestaBonus = value; } }
            public string AnnoDecorrenzaBonus { get { return _AnnoDecorrenzaBonus; } set { _AnnoDecorrenzaBonus = value; } }
            public System.Nullable<bool> IsDatiAggiuntiviFromJSON { get { return _IsDatiAggiuntiviFromJSON; } set { _IsDatiAggiuntiviFromJSON = value; } }
            public System.Nullable<System.DateTime> DataCondizioniPerComputo { get { return _DataCondizioniPerComputo; } set { _DataCondizioniPerComputo = value; } }
            public System.Nullable<char> Flag5000 { get { return _Flag5000; } set { _Flag5000 = value; } }
            public string DirittoAutonomo { get { return _DirittoAutonomo; } set { _DirittoAutonomo = value; } }
            public bool? IsPLInvalidita { get { return _IsPLInvalidita; } set { _IsPLInvalidita = value; } }
            public bool? IsRicRinnovata { get { return _IsRicRinnovata; } set { _IsRicRinnovata = value; } }
            public bool? IsRicExtracalcolo { get { return _IsRicExtracalcolo; } set { _IsRicExtracalcolo = value; } }
            public short? CodiceSedeGP1ALZ6 { get; set; }
            public byte? CentroOperativoGP1ALZ6 { get; set; }
            public byte? TipoAutomazione { get { return _TipoAutomazione; } set { _TipoAutomazione = value; } }
            public short? GP1AV11 { get; set; }
            public byte? Ante96ByDatiCalcolo { get { return _Ante96ByDatiCalcolo; } set { _Ante96ByDatiCalcolo = value; } }
            public System.Nullable<System.DateTime> MaxDecDatiCalcoloAnte96 { get { return _MaxDecDatiCalcoloAnte96; } set { _MaxDecDatiCalcoloAnte96 = value; } }
            public short? GP1AV91A { get; set; }
            public DateTime? DataAcquisizioneIVS { get; set; }

            public System.Nullable<bool> IsTentataAutomazione { get { return _IsTentataAutomazione; } set { _IsTentataAutomazione = value; } }

            public string Caratterizzazione { get { return _Caratterizzazione; } set { _Caratterizzazione = value; } }

            public System.Nullable<int> CodProPE { get { return _CodProPE; } set { _CodProPE = value; } }

            //ENG - Memo 57_2023
            public string AnnoMonitoraggio { get { return _AnnoMonitoraggio; } set { _AnnoMonitoraggio = value; } }

            public string GP1AV91B { get { return _GP1AV91B; } set { _GP1AV91B = value; } }

            public bool? IsNuovoCalcolo { get { return _IsNuovoCalcolo; } set { _IsNuovoCalcolo = value; } }

            //ENG - Aggiornamento Memo INPGI
            public string GP1AJ11 { get { return _GP1AJ11; } set { _GP1AJ11 = value; } }

            public DateTime? DataEstrazioneRata { get { return _DataEstrazioneRata; } set { _DataEstrazioneRata = value; } }

            public int? IdNota { get { return _IdNota; } set { _IdNota = value; } }

            public bool? SbloccaPannelliAnte96 { get { return _SbloccaPannelliAnte96; } set { _SbloccaPannelliAnte96 = value; } }

            public string FlagIndebito { get { return _FlagIndebito; } set { _FlagIndebito = value; } }

            //ENG - Spacchettate AGO
            public char? GP1AJSP { get { return _GP1AJSP; } set { _GP1AJSP = value; } }

            //ENG - Implementazione Meta processo
            public short? CodiceSedeLavorazione { get { return _CodiceSedeLavorazione; } set { _CodiceSedeLavorazione = value; } }

            #endregion public properties
        }

        public class DatiPatronato
        {
            public DatiPatronato()
            { }
            public DatiPatronato(string codiceEnte, string codiceUfficio, string nPratica, string tipoUfficio)
            {
                this._CodiceEnte = codiceEnte;

                this._CodiceUfficio = codiceUfficio;

                this._NPratica = nPratica;

                this._TipoUfficio = tipoUfficio;
            }

            #region private properties
            private string _CodiceEnte;

            private string _CodiceUfficio;

            private string _NPratica;

            private string _TipoUfficio;
            #endregion private properties

            #region public properties
            public string CodiceEnte { get { return _CodiceEnte; } set { _CodiceEnte = value; } }

            public string CodiceUfficio { get { return _CodiceUfficio; } set { _CodiceUfficio = value; } }

            public string NPratica { get { return _NPratica; } set { _NPratica = value; } }

            public string TipoUfficio { get { return _TipoUfficio; } set { _TipoUfficio = value; } }
            #endregion public properties

            #region public methods
            /// <summary>
            /// Un patronato è un'azienda se ilsuo codiceEnte comincia per 'A' o 'B'
            /// </summary>
            public bool isAzienda()
            {
                bool ret = false;
                if (!string.IsNullOrEmpty(this._TipoUfficio) && (this._TipoUfficio.StartsWith("A") || this._TipoUfficio.StartsWith("B")))
                    ret = true;
                return ret;
            }
            #endregion  public methods
        }

        public class DatiSindacato
        {
            public DatiSindacato()
            { }
            public DatiSindacato(string codiceSindacato, string descrizioneSindacato, System.Nullable<System.DateTime> decorrenzaSindacato, System.Nullable<System.DateTime> cessazioneSindacato, bool? isFromService)
            {
                this._CodiceSindacato = codiceSindacato;

                this._DescrizioneSindacato = descrizioneSindacato;

                this._DecorrenzaSindacato = decorrenzaSindacato;

                this._CessazioneSindacato = cessazioneSindacato;

                this._IsFromService = isFromService;
            }

            public DatiSindacato(string codiceSindacato, string descrizioneSindacato, System.Nullable<System.DateTime> decorrenzaSindacato, System.Nullable<System.DateTime> cessazioneSindacato, Utility.StatoSindacato? stato, bool? isFromService)
            {
                this._CodiceSindacato = codiceSindacato;

                this._DescrizioneSindacato = descrizioneSindacato;

                this._DecorrenzaSindacato = decorrenzaSindacato;

                this._CessazioneSindacato = cessazioneSindacato;

                this._Stato = stato;

                this._IsFromService = isFromService;
            }

            #region private properties
            private string _CodiceSindacato;

            private string _DescrizioneSindacato;

            private System.Nullable<System.DateTime> _DecorrenzaSindacato;

            private System.Nullable<System.DateTime> _CessazioneSindacato;

            private Utility.StatoSindacato? _Stato;

            private bool? _IsFromService;

            #endregion private properties

            #region public properties
            public string CodiceSindacato { get { return _CodiceSindacato; } set { _CodiceSindacato = value; } }

            public string DescrizioneSindacato { get { return _DescrizioneSindacato; } set { _DescrizioneSindacato = value; } }

            public System.Nullable<System.DateTime> DecorrenzaSindacato { get { return _DecorrenzaSindacato; } set { _DecorrenzaSindacato = value; } }

            public System.Nullable<System.DateTime> CessazioneSindacato { get { return _CessazioneSindacato; } set { _CessazioneSindacato = value; } }

            public Utility.StatoSindacato? Stato { get { return _Stato; } set { _Stato = value; } }

            public bool? IsFromService { get { return _IsFromService; } set { _IsFromService = value; } }
            #endregion public properties
        }

        public class DatiTitolare
        {
            #region private properties
            private Int64 _IdPensione;

            private Int64 _IdAnagrafica;
            #endregion private properties

            #region public properties
            public Int64 IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public Int64 IdAnagrafica { get { return _IdAnagrafica; } set { _IdAnagrafica = value; } }

            public DateTime? DataMorte { get; set; }
            #endregion public properties
        }

        public class DatiEliminazione
        {
            public DatiEliminazione()
            { }
            public DatiEliminazione(System.Nullable<byte> codiceMotivo, System.Nullable<System.DateTime> decorrenzaEliminazione,
                System.Nullable<System.DateTime> dataEvento, System.Nullable<System.DateTime> dataComunicazione,
                System.Nullable<System.DateTime> dataUltimaRiscossione, System.Nullable<System.DateTime> dataFineCalcoloArretrati,
                System.Nullable<byte> codiceRinnovo, string codiceTipoMovimentazione, System.Nullable<System.DateTime> dataUltimaMovimentazione, DateTime? dataCessazioneDiritto, DateTime? dataComunicazioneEliminazione)
            {
                this._CodiceMotivo = codiceMotivo;
                this._DecorrenzaEliminazione = decorrenzaEliminazione;
                this._DataEvento = dataEvento;
                this._DataComunicazione = dataComunicazione;
                this._DataUltimaRiscossione = dataUltimaRiscossione;
                this._DataFineCalcoloArretrati = dataFineCalcoloArretrati;
                this._CodiceRinnovo = codiceRinnovo;
                this._CodiceTipoMovimentazione = codiceTipoMovimentazione;
                this._DataUltimaMovimentazione = dataUltimaMovimentazione;
                this._DataCessazioneDiritto = dataCessazioneDiritto;
                this._DataComunicazioneEliminazione = dataComunicazioneEliminazione;
            }
            #region private properties
            private System.Nullable<byte> _CodiceMotivo;

            private System.Nullable<System.DateTime> _DecorrenzaEliminazione;

            private System.Nullable<System.DateTime> _DataEvento;

            private System.Nullable<System.DateTime> _DataComunicazione;

            private System.Nullable<System.DateTime> _DataUltimaRiscossione;

            private System.Nullable<System.DateTime> _DataFineCalcoloArretrati;

            private System.Nullable<byte> _CodiceRinnovo;

            private string _CodiceTipoMovimentazione;

            private System.Nullable<System.DateTime> _DataUltimaMovimentazione;

            private DateTime? _DataCessazioneDiritto;

            private DateTime? _DataComunicazioneEliminazione;
            #endregion private properties

            #region public properties
            public System.Nullable<byte> CodiceMotivo { get { return _CodiceMotivo; } set { _CodiceMotivo = value; } }

            public System.Nullable<System.DateTime> DecorrenzaEliminazione { get { return _DecorrenzaEliminazione; } set { _DecorrenzaEliminazione = value; } }

            public System.Nullable<System.DateTime> DataEvento { get { return _DataEvento; } set { _DataEvento = value; } }

            public System.Nullable<System.DateTime> DataComunicazione { get { return _DataComunicazione; } set { _DataComunicazione = value; } }

            public System.Nullable<System.DateTime> DataUltimaRiscossione { get { return _DataUltimaRiscossione; } set { _DataUltimaRiscossione = value; } }

            public System.Nullable<System.DateTime> DataFineCalcoloArretrati { get { return _DataFineCalcoloArretrati; } set { _DataFineCalcoloArretrati = value; } }

            public System.Nullable<byte> CodiceRinnovo { get { return _CodiceRinnovo; } set { _CodiceRinnovo = value; } }

            public string CodiceTipoMovimentazione { get { return _CodiceTipoMovimentazione; } set { _CodiceTipoMovimentazione = value; } }

            public System.Nullable<System.DateTime> DataUltimaMovimentazione { get { return _DataUltimaMovimentazione; } set { _DataUltimaMovimentazione = value; } }

            public DateTime? DataCessazioneDiritto { get { return _DataCessazioneDiritto; } set { _DataCessazioneDiritto = value; } }

            public DateTime? DataComunicazioneEliminazione { get { return _DataComunicazioneEliminazione; } set { _DataComunicazioneEliminazione = value; } }

            public override bool Equals(object obj)
            {
                DatiEliminazione eliminazione = (DatiEliminazione)obj;
                try
                {
                    if (this._CodiceMotivo != eliminazione._CodiceMotivo ||
                         this._DecorrenzaEliminazione != eliminazione._DecorrenzaEliminazione ||
                         this._DataEvento != eliminazione._DataEvento ||
                         this._DataComunicazione != eliminazione._DataComunicazione ||
                         this._DataUltimaRiscossione != eliminazione._DataUltimaRiscossione ||
                         this._DataFineCalcoloArretrati != eliminazione._DataFineCalcoloArretrati ||
                         this._CodiceRinnovo != eliminazione._CodiceRinnovo ||
                         (this._CodiceTipoMovimentazione != null ? this._CodiceTipoMovimentazione.Trim() : null) != (eliminazione._CodiceTipoMovimentazione != null ? eliminazione._CodiceTipoMovimentazione.Trim() : null) ||
                         this._DataUltimaMovimentazione != eliminazione._DataUltimaMovimentazione ||
                        this._DataCessazioneDiritto != eliminazione._DataCessazioneDiritto ||
                        this._DataComunicazioneEliminazione != eliminazione._DataComunicazioneEliminazione)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            #endregion public properties
        }

        public class DatiByPassCancellazione
        {
            public long NDomus { get; set; }
            public short CodiceSede { get; set; }
            public byte CentroOperativo { get; set; }
            public string SiglaCategoria { get; set; }
        }

        #endregion nested classes
    }
}


