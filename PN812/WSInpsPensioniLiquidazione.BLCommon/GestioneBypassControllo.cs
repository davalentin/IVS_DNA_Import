using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneBypassControllo
    {
        #region DecBypassControllo
        public static void GetDecBypassControlloByTipoApp(string tipoApp, out List<DatiDecBypassControllo> lstDecBypassControllo)
        {
            lstDecBypassControllo = new List<DatiDecBypassControllo>();

            List<DecBypassControllo> lstDataLayer;
            DAGestioneBypassControllo.GetDecBypassControlloByTipoApp(tipoApp, out lstDataLayer);
            lstDataLayer = lstDataLayer.Where(x => !x.FlagDinamico).ToList();
            if (lstDataLayer != null && lstDataLayer.Count > 0)
            {
                foreach (DecBypassControllo decDb in lstDataLayer)
                {
                    DatiDecBypassControllo decBl = new DatiDecBypassControllo();
                    Utility.ValorizzaOggetti(decDb, decBl);
                    lstDecBypassControllo.Add(decBl);
                }
            }
        }

        public static void GetDecBypassControlloDinamiciByTipoApp(string tipoApp, out List<DatiDecBypassControllo> lstDecBypassControllo)
        {
            lstDecBypassControllo = new List<DatiDecBypassControllo>();

            List<DecBypassControllo> lstDataLayer;
            DAGestioneBypassControllo.GetDecBypassControlloByTipoApp(tipoApp, out lstDataLayer);
            lstDataLayer = lstDataLayer.Where(x => x.FlagDinamico).ToList();
            if (lstDataLayer != null && lstDataLayer.Count > 0)
            {
                foreach (DecBypassControllo decDb in lstDataLayer)
                {
                    DatiDecBypassControllo decBl = new DatiDecBypassControllo();
                    Utility.ValorizzaOggetti(decDb, decBl);
                    lstDecBypassControllo.Add(decBl);
                }
            }
        }
        #endregion DecBypassControllo

        #region BypassControllo

        public static void GetAllBypassControlloByTipoApp(string tipoApp, out List<DatiBypassControllo> lstBypassControllo)
        {
            lstBypassControllo = new List<DatiBypassControllo>();

            List<BypassControllo> lstDataLayer = new List<BypassControllo>();
            DAGestioneBypassControllo.GetAllBypassControlloByTipoApp(tipoApp, out lstDataLayer);
            if (lstDataLayer != null && lstDataLayer.Count > 0)
            {
                foreach (BypassControllo objDb in lstDataLayer)
                {
                    DatiBypassControllo objBl = new DatiBypassControllo();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    lstBypassControllo.Add(objBl);
                }
            }
        }

        public static void GetBypassControlloByNDomusAndIdDec(long? NDomus, long idDecodifica, out DatiBypassControllo objBl)
        {
            objBl = null;
            BypassControllo objDl = null;
            DAGestioneBypassControllo.GetBypassControlloByNDomusAndId(NDomus, idDecodifica, out objDl);
            if (objDl != null)
            {
                objBl = new DatiBypassControllo();
                Utility.ValorizzaOggetti(objDl, objBl);
                objBl.NomeBypass = objDl.DecBypassControllo.Nome;
            }
        }

        public static void GetBypassControlloByChiavePensioneAndIdDec(string siglaCategoria, short? codiceSede, int? nCertificato, long idDecodifica, out DatiBypassControllo objBl)
        {
            objBl = null;
            BypassControllo objDl = null;
            DAGestioneBypassControllo.GetBypassControlloByChiavePensioneAndIdDec(siglaCategoria, codiceSede, nCertificato, idDecodifica, out objDl);
            if (objDl != null)
            {
                objBl = new DatiBypassControllo();
                Utility.ValorizzaOggetti(objDl, objBl);
                objBl.NomeBypass = objDl.DecBypassControllo.Nome;
            }
        }

        public static void GetBypassApplicatiPerNDomus(long NDomus, out List<DatiBypassControllo> listaBypassApplicatiPerNDomus)
        {
            listaBypassApplicatiPerNDomus = new List<DatiBypassControllo>();
            List<DataCommon.BypassControllo> listaBypassApplicatiPerNDomusDB;
            DAGestioneBypassControllo.GetBypassApplicatiPerNDomus(NDomus, out listaBypassApplicatiPerNDomusDB);
            if (listaBypassApplicatiPerNDomusDB != null && listaBypassApplicatiPerNDomusDB.Count > 0)
            {
                foreach (DataCommon.BypassControllo BypassApplicatiDB in listaBypassApplicatiPerNDomusDB)
                {
                    DatiBypassControllo BypassApplicati = new DatiBypassControllo();
                    Utility.ValorizzaOggetti(BypassApplicatiDB, BypassApplicati);
                    listaBypassApplicatiPerNDomus.Add(BypassApplicati);
                }
            }
        }

        public static void EliminaBypassControlloById(long id)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneBypassControllo.EliminaBypassControlloById(id);
                transactionScope.Complete();
            }
        }

        public static void DeleteAllBypassControlloDinamiciByDomus(long NDomus)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneBypassControllo.DeleteAllBypassControlloByDomus(NDomus);
                transactionScope.Complete();
            }
        }

        public static void SalvaBypassControllo(DatiBypassControllo datiBypassControllo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                BypassControllo objDl = new BypassControllo();
                Utility.ValorizzaOggetti(datiBypassControllo, objDl);
                DAGestioneBypassControllo.InsertBypassControllo(objDl);
                transactionScope.Complete();
            }
        }


        /// <summary>
        /// Verifica se  esiste il bypass enumBypass. Se esiste restituisce true ed effettua il lock del bypass la domanda specifica.
        /// </summary>
        public static bool CheckAndLockBypassControlloByNomeBypass(GestionePensione.DatiPensione datiPensione, Enum enumBypass)
        {
            string nome = enumBypass.ToString();

            BypassControllo bypass = null;

            if (datiPensione.CodiceSedeDestinazione.HasValue)
                DAGestioneBypassControllo.GetBypassControlloByNomeBypass(datiPensione.NDomus, datiPensione.GetCodCategoria(), datiPensione.CodiceSedeDestinazione, datiPensione.NCertificato, nome, out bypass);
            else
                DAGestioneBypassControllo.GetBypassControlloByNomeBypass(datiPensione.NDomus, datiPensione.GetCodCategoria(), datiPensione.CodiceSede, datiPensione.NCertificato, nome, out bypass);

            if (bypass != null)
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    bypass.Lock = true;
                    DAGestioneBypassControllo.InsertBypassControllo(bypass);
                    transactionScope.Complete();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifica se  esiste il bypass enumBypass. Se esiste restituisce true
        /// </summary>
        public static bool CheckBypassControlloByNomeBypass(GestionePensione.DatiPensione datiPensione, Enum enumBypass)
        {
            string nome = enumBypass.ToString();
            BypassControllo bypass = null;
            DAGestioneBypassControllo.GetBypassControlloByNDomusAndNomeBypass(datiPensione.NDomus, nome, out bypass);
            if (bypass != null)
                return true;

            return false;
        }

        public static void SetUnlock(long nDomus, Type typeEnum)
        {
            List<BypassControllo> listBypassControlli = null;
            List<string> listEnum = Enum.GetNames(typeEnum).ToList();

            DAGestioneBypassControllo.GetBypassControlloByNDomusAndListNomeBypass(nDomus, listEnum, out listBypassControlli);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (listBypassControlli != null && listBypassControlli.Count > 0)
                {
                    foreach (BypassControllo bypass in listBypassControlli)
                    {
                        bypass.Lock = false;
                        DAGestioneBypassControllo.InsertBypassControllo(bypass);
                    }
                }
                transactionScope.Complete();
            }
        }

        public static void SetAllUnlock(long nDomus)
        {
            List<BypassControllo> listBypassControlli = null;

            DAGestioneBypassControllo.GetAllBypassControlloByNDomus(nDomus, out listBypassControlli);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (listBypassControlli != null && listBypassControlli.Count > 0)
                {
                    foreach (BypassControllo bypass in listBypassControlli)
                    {
                        bypass.Lock = false;
                        DAGestioneBypassControllo.InsertBypassControllo(bypass);
                    }
                }
                transactionScope.Complete();
            }
        }

        #endregion BypassControllo

        #region Nested Class

        public class DatiBypassControllo
        {
            public long Id { get; set; }
            public string CodCategoria { get; set; }
            public short? CodiceSede { get; set; }
            public int? NCertificato { get; set; }
            public long? NDomus { get; set; }
            public string Matricola { get; set; }
            public string Note { get; set; }
            public bool Lock { get; set; }
            public long IdDecBypassControllo { get; set; }
            public string NomeBypass { get; set; }
        }

        public class DatiDecBypassControllo
        {
            public long Id { get; set; }
            public string Nome { get; set; }
            public string Descrizione { get; set; }
            public string TipoApp { get; set; }
        }

        #region Enum Nome Controllo
        /// <summary>
        /// Gli elementi di questo enum devono corrispondere ai nomi delle tipologie di bypass a db
        /// </summary>
        public class NomeBypass
        {
            public enum LiquidazionePensione_Assicurativi_FS
            {
                /// <summary>
                /// Per i fondi EL, ET, TT, VL, GAS, CL, FS e PT al salvataggio dei dati Assicurativi vengono eseguiti i seguenti controlli: 1) Se la domanda è di tipo Sperimentale donna e il Codice Requisito 2 ha valore, 
                /// allora il valore deve essere uguale a 9. 2) Se la domanda non è di tipo Sperimentale donna e il Codice Requisito 2 ha valore, allora il valore deve essere uguale a 0.
                /// </summary>
                SPER_DONNA,
                /// <summary>
                /// inibisce il controllo per cui il primo versamento deve essere versato almeno a 14 anni  
                /// </summary>
                PRIMO_VERSAMENTO,
                /// <summary>
                /// Al salvataggio dei Dati Assicurativi non viene effettuato il controllo sull'età del titolare rispetto alla data di Perfezionamento dei Requisiti
                /// </summary>
                LIMITE_ETA_TITOLARE,
                /// <summary>
                /// Il bypass prevede di poter inibire i controlli impostati sul numero di settimane per la linea FS
                /// </summary>
                NUM_SETT_PENS,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla data primo versamento obbligatoria per le Ricostituzioni Contributive del fondo FS.
                /// </summary>
                PRIMO_VERSAMENTO_FONDO_FS
            };

            public enum LiquidazionePensione_Assicurativi_AGO
            {
                /// <summary>
                ///Il controllo  per le pensioni di vecchiaia diverse da supplementari (tipo = “0009”) e cumulo (tipo = “0044”), con terzo codice natura diverso 
                ///da “L”, “O”, “Q”, “W”, “Y”, “Z” verifica che la somma tra il “Numero Settimane” e “Numero Contributi Volontari Diritto” sia maggiore o uguale a 780. 
                /// </summary>
                NUM_SET_780,
                /// <summary>
                /// Al salvataggio dei dati Assicurativi vengono eseguiti i controlli: 
                /// 1) Il numero settimane non può essere superiore a 3000 
                /// 2) La somma del numero settimane e del numero contributi volontari diritto non può essere superiore a 3000
                /// </summary>
                NUM_SET_OBG_MAX,
                /// <summary>
                /// Il controllo per le domande con categoria che inizia per "V" (ad esclusione delle VOTOT) con primo codice natura "1" o "2" e terzo codice natura diverso 
                /// da “L”, “O”, “Q”, “W”, “Y”, “Z” verifica che la somma tra il “Numero Settimane” e “Numero Contributi Volontari Diritto” sia maggiore o uguale a 1820. 
                /// </summary>
                NUM_SET_1820,
                ///Il bypass ha lo scopo di inibire il controllo sui requisiti contributivi per le domande di tipo contributivo (1/2/17) e (1/1/17)
                NUM_SET_CONTR,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo che il numero di settimane OBG sia inferiore a 260 per le domande di pensione supplementare antecedenti al 2012
                /// </summary>
                SUPPL_ANTE2012_260,
                /// <summary>
                /// Il bypass ha lo scopo di inibire i controlli sui dati assicurativi INDCOM
                /// </summary>
                BYPASS_ASS_INDCOM
            };

            public enum DatiCalcolo_DatiCalcolo_AGO
            {
                /// <summary>
                /// Il controllo prevede che per le domande con tipo calcolo Retributivo e quota D debba essere presente la quota B con lo stesso codice gestione della quota D e 
                /// che siano presenti le Settimane 707 per tutte le quote Retributive.
                /// </summary>
                FALSO_RETRIBUTIVO_MONTI,
                /// <summary>
                /// Al salvataggio dei dati Calcolo non verranno eseguiti controlli sui campi del Contributo di Solidarità L. 214/2011
                /// </summary>
                CONTR_SOLIDARIETA_L_214_2011,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sul numero di settimane minimo previsto per le domande di anzianità anticipata per legge bilancio 2019.
                /// Inoltre, inserendo questo bypass, verrà automaticamente inibito il controllo perle domande con categoria che inizia per "V" (ad esclusione delle VOTOT) con primo codice natura "1" o "2" e terzo codice natura diverso 
                /// da “L”, “O”, “Q”, “W”, “Y”, “Z” verifica che la somma tra il “Numero Settimane” e “Numero Contributi Volontari Diritto” sia maggiore o uguale a 1820.  
                /// </summary>
                NUM_SET_ANTICIPATA_2019,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo di verifica sul numero di settimane maggiore di 2080 in fase di salvataggio dei Dati Calcolo   
                /// </summary>
                NUM_SET_MAGG_2080,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla quota A1 per le IOCUM   
                /// </summary>
                CONTROLLO_QUOTE_CUMULO,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sul numero di giorni se supera il massimo valore consentito di 14400   
                /// </summary>
                SOMMA_GIORNI_SUP_14400,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla presenza della quota C con il tipo calcolo misto   
                /// </summary>
                CALCOLO_MISTO_NO_QUOTA_C,
                /// <summary>
                /// Il bypass ha lo scopo di inibire i controlli di validazione degli importi sui dati calcolo contributivi
                /// </summary>
                LIMITE7_INTERI_MONT_AMM
            }

            public enum DatiCalcolo_DatiCalcolo_FS
            {
                /// <summary>
                /// Il bypass prevede di poter inserire il Beneficio previsto per l'ex art.24 comma 15 bis senza vincoli sul numero di settimane utili diritto 
                /// e senza vincoli sulla capienza delle settimane delle singole quote
                /// 
                /// QUESTO BYPASS E' PRESENTE SU DIVERSI ENUM PERCHE' BYPASSA I CONTROLLI DI DUE QUADRI DIVERSI
                /// </summary>
                BENEF_ULT_ART24COMMA15BIS,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sul numero di settimane minimo previsto per le domande di APE Precoci
                /// </summary>
                NUM_SETT_APEPRECOCI,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sul numero di settimane minimo previsto per le domande di Quota 100
                /// </summary>
                NUM_SETT_QUOTA100,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo tra decorrenza, registrazione e calcolo per FS e PT
                /// </summary>
                DECOR_REGISTR_CALCOLO_FSPT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire i controlli sui dati calcolo GDP
                /// </summary>
                BYPASS_SERVIZIO_UTILE_GDP,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla obbligatorietà delle quote A, B, C, D
                /// </summary>
                ASSENZA_QUOTA_A_B_C_D_OBBLIGAT
            }

            public enum MaggiorazioniBenefici_Benefici_FS
            {
                /// <summary>
                /// Il bypass prevede di poter inserire il Beneficio previsto per l'ex art.24 comma 15 bis senza vincoli sul numero di settimane utili diritto
                /// </summary>
                BENEFICIO_ART24COMMA15BIS,
                /// <summary>
                /// Il bypass prevede di poter inserire il Beneficio previsto per l'ex art.24 comma 15 bis senza vincoli sul numero di settimane utili diritto 
                /// e senza vincoli sulla capienza delle settimane delle singole quote
                /// 
                /// QUESTO BYPASS E' PRESENTE SU DIVERSI ENUM PERCHE' BYPASSA I CONTROLLI DI DUE QUADRI DIVERSI
                /// </summary>
                BENEF_ULT_ART24COMMA15BIS,
                /// <summary>
                /// Il bypass ha lo scopo di permettere la lavorazione delle domande di Quota 100 senza vincoli sulla capienza delle settimane delle singole quote
                /// </summary>
                DOPPIO_BENEFICIO_CON_QUOTA100
            }

            public enum MaggiorazioniBenefici_Maggiorazioni_AGO
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo "La decorrenza (MM/AAAA) in nel tab 'Maggiorazioni' deve essere maggiore della data presentazione della domanda (GG/MM/AAAA)".
                /// </summary>
                MAGG_SOCIALE_DATA_PRESENT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo "La decorrenza maggiorazione sociale deve essere maggiore o uguale alla decorrenza già presente".
                /// </summary>
                DECORRENZA_MAGG_SOCIALE
            }

            public enum Titolare_Anagrafica_AGO
            {
                /// <summary>
                /// Il bypass ha lo scopo di permettere la lavorazione delle domande della gestione CUM che abbiano la data di perfezionamento dei requisiti successiva alla decorrenza della pensione.
                /// </summary>
                COMPARTO_SCUOLA,
                /// <summary>
                /// Al salvataggio del quadro del titolare e in fase di invio al calcolo NON viene effettuato il controllo sul requisito minimo di eta per il personale ENAV che ha perso il titolo abilitante.
                /// </summary>
                LIMITE_ETA_ENAV,
                /// <summary>
                /// Al salvataggio del quadro del titolare e in fase di invio al calcolo non viene effettuato il controllo sul requisito minimo di età per applicazione sentenza.
                /// </summary>
                APPLICAZIONE_SENTENZA,
                APPLICAZIONE_SENTENZA_VOAUT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza pensione uguale al mese successivo alla morte del dante causa.
                /// </summary>
                DECORRENZA_SUPERSTITI,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza pensione precedente alla data di presentazione.
                /// </summary>
                DECORR_PRECEDENTE_PRESENTAZ,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo di coerenza tra data perfezionamento requisiti e decorrenza pensione per PL, RIC e TRF (per i titolari di isopensione art. 4 L.92/2012 e assegni straordinari).
                /// </summary>
                DECORRENZA_PL_RIC_AGO,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla data perfezionamento dei requisiti che si colloca nel 2011.
                /// </summary>
                PERFEZIONAMENTO_REQ_2011,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo dei 12 mesi tra DPR e decorrenza pensione e il raggiungimento del titolare, alla DPR, dei termini di età previsti.
                /// </summary>
                BYPASS_FINESTRA_12MESI,
                /// <summary>
                /// Il bypass ha lo scopo di saltare il controllo sulla decorrenza inferiore ad aprile/marzo 2006 per tutte le TOT
                /// </summary>
                BYPASS_DEC_TOT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla deecorrenza delle domande prime liquidate banc con decorrenza tra il 01/91 e il 12/95
                /// </summary>
                BYPASS_ANTE96_BANC_AGO,
                /// <summary>
                /// Il bypass ha lo scopo di consentire la lavorazione della domanda nel caso di titolare nato dopo 300 giorni dalla data di decesso del dante causa
                /// </summary>
                TITOLARE_NATO_DOPO_300_GIORNI,
                 /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo "La decorrenza originaria non può essere anteriore a Febbraio 2013" per le PL VOCUM di vecchiaia di tipo contributivo (1/2/17)
                /// </summary>
                DECORR_PREC_2013_VOCUM_CONTR
            };

            public enum Titolare_Anagrafica_FS
            {
                /// <summary>
                /// Il bypass ha lo scopo di permettere la lavorazione delle domande della gestione FS che abbiano titolari nati a febbraio e che perfezionano i requisiti un giorno prima da quanto 
                /// determinato dai controlli, a causa degli algoritmi legati all’anno inps di 360 giorni e al mese inps di 30 giorni.
                /// </summary>
                NATI_29FEBBRAIO,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza pensione limite ammessa per le domande dei fondi FS/PT di Quota 100
                /// </summary>
                DECORRENZA_QUOTA100_FSPT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza pensione limite ammessa per le domande anticipate (rientranti nella legge di bilancio 2019) dei fondi FS/PT
                /// </summary>
                DECORRENZA_ANTICIPATA_FSPT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza pensione limite ammessa per le domande dei fondi FS/PT di domande in regimen Sperimentale Lavoratrici DL 4/2019
                /// </summary>
                DECORRENZA_SPERDONNA_FSPT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza pensione limite ammessa per le domande dei fondi FS/PT di domande di APE Precoce
                /// </summary>
                DECORRENZA_PRECOCI_FSPT,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo di coerenza tra data perfezionamento requisiti e decorrenza pensione per le Ricostituzioni dei fondi ET/EL/TT (per i titolari di isopensione art. 4 L.92/2012).
                /// </summary>
                DECORRENZA_RIC_FONDI
            }

            public enum Dante_Causa_FS
            {
                /// <summary>
                /// Il bypass ha lo scopo di bypassare per le RIC linea FS (no 024 e INPDAP), i controlli sul dante causa nel caso in cui esso non risulti presente
                /// </summary>
                NESSUN_DANTE_CAUSA,
                NESSUN_DANTE_CAUSA_DINAMICO
            }

            public enum Dante_Causa_CI
            {
                /// <summary>
                /// Il bypass ha lo scopo di bypassare i controlli che verificano la presenza dei dati del dante causa, nel caso di dante causa fittizio
                /// </summary>
                NESSUN_DANTE_CAUSA
            }

            public enum Supplementi_Supplementi_AGO
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire i controlli "Per i supplementi contributivi il 'Codice Tipo Liquidazione' uguale a 'C' è ammesso solo per tipo calcolo 'MISTO' o 'CONTRIBUTIVO'." e 
                /// "I supplementi retributivi sono ammessi solo per tipo calcolo 'RETRIBUTIVO' o 'MISTO'."
                /// </summary>
                SUPPLEMENTI_FUORI_PERIODO,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza del supplemento in relazione alla decorrenza pensione/decorrenza dell'ultimo supplemento
                /// </summary>
                SUPPLEMENTI_CUMULO_AUTOMATICA,
                /// <summary>
                /// Il bypass ha lo scopo di saltare i controlli sui supplementi, inerenti la combinazione di settimane, RMS e importo IVS necessari, in funzione della decorrenza del supplemento
                /// </summary>
                SUPP_CHECK_SETT_RMS_IMP_IVS
            }

            public enum LiquidazionePensione_Assicurativi_CI
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo MONTANTE_335_INCOMPATIBILE con Data Ultimo Contributo
                /// </summary>
                MONTANTE_335_INCOMPATIBILE,
                /// <summary>
                /// Il bypass ha lo scopo di inibire i controlli all’interno del metodo "VerificaRequisitiAnzianita9496Vecchiaia94"
                /// </summary>
                BYPASS_REQ_ANZ9496_VECCH94
            }

            public enum Requisito_Eta
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sui requisiti di età per il tipo contributivo (1/2/17)
                /// </summary>
                REQUISITO_ETA
            }

            public enum LiquidazionePensione_Istruttoria_AGO
            {
                /// <summary>
                /// Inibire i controlli relativi al periodo massimo di permanenza individuale per le VOESO
                /// </summary>
                BYPASS_SCADENZA_ESODI
            }

            public enum Sentenza_Unioni_Civili
            {

                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza della domanda del superstite, nel caso in cui il dante causa è unito civilmente, e sulla verifica della decorrenza delle unioni civili
                /// </summary>
                SENTENZA_UNIONI_CIVILI
            }

            public enum Sentenza_Bonus_Y
            {
                /// <summary>
                /// Il bypass ha lo scopo di permettere la visualizzazione al secondo byte del codice natura anche il valore Y (pensione in bonus)
                /// </summary>
                SENTENZA_BONUS_Y
            }

            public enum Bititolarita_IoSpett
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo che rende incompatibile una VOART su una IOSPETT
                /// </summary>
                BITITOLARITA_IOSPETT
            }

            public enum Acquisizione
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il blocco dell'aquisizione di domande contributive manuale con opzione
                /// </summary>
                ACQ_OPZ_CONTR,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il blocco all'aquisizione sulla verifica della certificazione
                /// </summary>
                NO_CERT_FELPE,
                /// <summary>
                /// Il bypass ha lo scopo di inibire il blocco dell'aquisizione di domande indirette manuali gdp
                /// </summary>
                INDIRETTA_MANUALE
            }

            public enum Sentenza_Nati_Dopo_300_Giorni
            {
                /// <summary>
                /// Il bypass ha lo scopo di consentire l'inserimento (per sentenza) di familiari nati dopo 300 giorni dalla data di decesso del dante causa
                /// </summary>
                SENTENZA_NATI_DOPO_300_GIORNI
            }


            public enum Num_Sett_Periodo_Ass
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo riguardante le settimane OBG e Diritto che eccedono il periodo assicurativo
                /// </summary>
                NUM_SETT_PERIODO_ASS
            }

            public enum Rms_Mancanti
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo che obbliga, se fine assicurazione è successiva al 31/12/1995, l'inserimento di almeno una: quota B con RMS valorizzato, quota C e quota D di una delle 4 gestioni
                /// </summary>
                RMS_MANCANTI
            }

            public enum Decorr_Chiarimento_Normativo
            {
                /// <summary>
                /// Data domanda antecedente decorrenza pensione liquidate prima del chiarimento normativo
                /// </summary>
                DECORR_CHIARIMENTO_NORMATIVO
            }

            public enum Rms_Ante_1993
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo di incompatibilità tra R.M.S. ante 1993 e decorrenza e/o Inizio Assicurazione
                /// </summary>
                RMS_ANTE_1993
            }

            public enum Settimane_OBG_Mancanti
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo Settimane OBG diritto mancanti
                /// </summary>
                SETTIMANE_OBG_MANCANTI
            }

            public enum SUP_INF_2AN_SENT_O_ART6_L40790
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza del supplemento superiore a 2 anni dalla decorrenza originaria 
                /// </summary>
                SUP_INF_2AN_SENT_O_ART6_L40790
            }

            public enum Contributi_95_Incompatibili
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo tra i contributi italiani ed esteri al 31/12/1995 incompatibili con dati per L.503/92
                /// </summary>
                CONTRIBUTI_95_INCOMPATIBILI
            }

            public enum Abilita_Cod_Comunicazioni
            {
                /// <summary>
                /// Il controllo ha lo scopo di rendere visibile e poter selezionare il codice comunicazione "ESENZIONE FISCALE VITTIMA"
                /// </summary>
                ABILITA_COD_COMUNICAZIONI
            }

            public enum Settimane_Dal_93_Incompatibili
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo: "Settimane dal 1993 incompatibile con Decorrenza e/o Ultimo Contributo"
                /// </summary>
                SETTIMANE_DAL_93_INCOMPATIBILI
            }

            //ENG - Bypass CESSAZIONE_BENEFICIO_ART15_DL4 (solo TRF)
            public enum Cessazione_Beneficio_ART15_DL4
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla data di cessazione beneficio degli oneri in applicazione dell'art 15 DL4 (solo TRF)
                /// </summary>
                CESSAZIONE_BENEFICIO_ART15_DL4
            }

            //ENG - Bypass AGO Filtro EAA
            public enum Req_Contr_Prep_Editoria
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sul Pannello liquidazione pensione – assicurativi 
                /// "il numero settimane se inferiore a 1976 (38 anni)" e sul Pannello Oneri "range inferiore ai 5 anni tra cessazione e decorrenza benefici"
                /// </summary>
                REQ_CONTR_PREP_EDITORIA
            }

            //ENG - Bypass Riconosc_Incompatibili_Dec
            public enum Riconosc_Incompatibili_Dec
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo tra il numero di riconoscimenti e la decorrenza
                /// </summary>
                RICONOSC_INCOMPATIBILI_DEC
            }

            //ENG - Bypass "Eliminazione_Controllo_Sede"
            public enum Eliminazione_Controllo_Sede
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo tra la sede della domanda e quella dell'operatore
                /// </summary>
                ELIMINAZIONE_CONTROLLO_SEDE,
                BYPASS_META_PROCESSO_FS,
                BYPASS_META_PROCESSO_AGO,
                BYPASS_META_PROCESSO_CI
            }

            //ENG - Bypass "SETT_SUPERIORI_CAPIENZA"
            public enum Sett_Superiori_Capienza
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulle settimane superiori a capienza nel periodo
                /// </summary>
                SETT_SUPERIORI_CAPIENZA
            }

            //ENG - Bypass "SUPPL_2_5_ANNI_SENTENZA_VOAUT"
            public enum Suppl_2_5_Anni_Sentenza_VOAUT
            {
                /// <summary>
                /// Il bypass permette di inibire, una sola volta, il controllo dei 2 e 5 anni per i supplementi di domande AUT
                /// </summary>
                SUPPL_2_5_ANNI_SENTENZA_VOAUT
            }

            public enum Suppl_Inf_A_2Anni_Per_Sent_CI
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo sulla decorrenza del supplemento superiore a 2 anni dalla decorrenza originaria 
                /// </summary>
                SUPPL_INF_A_2ANNI_PER_SENT_CI
            }

            public enum LiquidazionePensione_Generici_AGO
            {
                /// <summary>
                /// Il bypass consente di selezionare la scelta ESENZIONE FISCALE RESIDENTE ALL’ESTERO per titolare residente in Italia nel caso in cui sia stato residente all’estero per un periodo superiore a 180 giorn
                /// </summary>
                RES_EST_SUP_180_GIORNI,
                 /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo “Per i residenti in Bulgaria, ai fini dell'applicazione della detassazione, è necessario che il pensionato abbia la cittadinanza bulgara (anche se non esclusiva)
                /// </summary>
                DETASSAZIONE_BULGARIA
            }

            public enum LiquidazionePensione_Generici_FS
            {
                /// <summary>
                /// Il bypass ha lo scopo di permettere la liquidazione di una domanda di “Ricostituzione per Motivi documentali esenzione fiscale vittime del dovere
                /// </summary>
                CONFERMA_ESENZIONE_VITTIME
            }      

            public enum Rms_Obg_VV_Non_Acquisiti_CI
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo “R.M.S., OBG e VV D.L.503/92 non devono essere acquisiti
                /// </summary>
                RMS_OBG_VV_NON_ACQUISITI
            }

            public enum Titolare_Vedovo_Multiplo
            {
                /// <summary>
                /// Il bypass ha lo scopo di inibire il controllo “La decorrenza dello stato civile successivo allo stato coniugato/a deve essere uguale alla data Fine Carico"
                /// </summary>
                TITOLARE_VEDOVO_MULTIPLO

            }

            public enum GestioneControlliINPDAP
            {
                BYPASS_DEC_TRATTENUTA_INPDAP
            }
        }
        #endregion Enum Nome Controllo

        #endregion Nested Class
    }
}

