using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneAreaNoCalcolo
    {
        public static void GetRecordNoCalcolo(GestionePensione.DatiPensione datiPensione, out List<Entity.DatiRecordNoCalcolo> lstRecordNoCalcolo)
        {
            lstRecordNoCalcolo = null;

            List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstCommonDatiNoCalcolo;
            GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdPensione(datiPensione.Id, out lstCommonDatiNoCalcolo);

            if (lstCommonDatiNoCalcolo != null && lstCommonDatiNoCalcolo.Count > 0)
            {
                List<GestioneQuadri.DatiQuadroRecordNoCalcolo> lstQuadroRecordDatiNoCalcolo = null;
                GestioneQuadri.GetQuadroDatiRecordNoCalcoloByDatiPensione(datiPensione, out lstQuadroRecordDatiNoCalcolo);

                lstRecordNoCalcolo = lstCommonDatiNoCalcolo.Select(x => new Entity.DatiRecordNoCalcolo { Decorrenza = x.Decorrenza, IdRecordNoCalcolo = x.Id }).ToList();
                foreach (var record in lstRecordNoCalcolo)
                {
                    if (lstQuadroRecordDatiNoCalcolo != null && lstQuadroRecordDatiNoCalcolo.Exists(x => x.IdRecordDatiNoCalcolo == record.IdRecordNoCalcolo))
                    {
                        GestioneQuadri.DatiQuadroRecordNoCalcolo quadroRecordNoCalcolo = lstQuadroRecordDatiNoCalcolo.Find(x => x.IdRecordDatiNoCalcolo == record.IdRecordNoCalcolo);
                        record.TabNoCalcolo = quadroRecordNoCalcolo.TabNoCalcolo;
                    }
                    else
                    {
                        GestioneQuadri.DatiQuadroRecordNoCalcolo datiQuadroDatiRecordNoCalcolo = new GestioneQuadri.DatiQuadroRecordNoCalcolo();
                        datiQuadroDatiRecordNoCalcolo.IdPensione = datiPensione.Id;
                        datiQuadroDatiRecordNoCalcolo.TabNoCalcolo = 0;
                        GestioneQuadri.SalvaQuadroDatiRecordNoCalcolo(datiPensione.Id, record.IdRecordNoCalcolo, datiQuadroDatiRecordNoCalcolo);
                        record.TabNoCalcolo = datiQuadroDatiRecordNoCalcolo.TabNoCalcolo;
                    }
                }

                // Ordinamento
                var app = lstRecordNoCalcolo.Where(x => string.IsNullOrEmpty(x.Decorrenza));
                lstRecordNoCalcolo = lstRecordNoCalcolo.Where(x => !string.IsNullOrEmpty(x.Decorrenza)).ToList().OrderBy(x => DateCustom.Parse(x.Decorrenza)).ToList();
                lstRecordNoCalcolo.AddRange(app);
                ///////////////
            }
        }

        public static void AddRecordNoCalcolo(GestionePensione.DatiPensione datiPensione, out long? idRecordNoCalcolo, out Entity.DatiNoCalcolo datiNoCalcoloEntity)
        {
            datiNoCalcoloEntity = new Entity.DatiNoCalcolo();
            idRecordNoCalcolo = null;
            GestioneDatiNoCalcolo.RecordDatiNoCalcolo datiNoCalcoloBl = null;

            List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecordNoCalcolo = null;
            GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdPensione(datiPensione.Id, out lstRecordNoCalcolo);

            long appId = 0;
            if (lstRecordNoCalcolo != null && lstRecordNoCalcolo.Count > 0)
            {
                GestioneDatiNoCalcolo.RecordDatiNoCalcolo app = lstRecordNoCalcolo.Where(x => !string.IsNullOrEmpty(x.Decorrenza)).OrderBy(x => DateCustom.Parse(x.Decorrenza)).LastOrDefault();
                if (app != null)
                    appId = app.Id;
            }
            ValorizzaEntityComponentiFamiliari(datiPensione, appId, ref datiNoCalcoloEntity);


            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                datiNoCalcoloBl = new GestioneDatiNoCalcolo.RecordDatiNoCalcolo();
                GestioneDatiNoCalcolo.SalvaRecordNoCalcolo(datiPensione.Id, datiNoCalcoloBl, out idRecordNoCalcolo);
                Utility.ValorizzaOggetti(datiNoCalcoloBl, datiNoCalcoloEntity);
                //semaforo record rosso
                GestioneQuadri.DatiQuadroRecordNoCalcolo datiQuadroDatiRecordNoCalcolo = new GestioneQuadri.DatiQuadroRecordNoCalcolo();
                datiQuadroDatiRecordNoCalcolo.IdPensione = datiPensione.Id;
                datiQuadroDatiRecordNoCalcolo.TabNoCalcolo = 0;
                GestioneQuadri.SalvaQuadroDatiRecordNoCalcolo(datiPensione.Id, idRecordNoCalcolo.GetValueOrDefault(), datiQuadroDatiRecordNoCalcolo);
                datiNoCalcoloEntity.TabNoCalcolo = datiQuadroDatiRecordNoCalcolo.TabNoCalcolo;
                //semaforo quadro rosso
                GestioneQuadri.SalvaQuadroDatiNoCalcolo(datiPensione.Id, new GestioneQuadri.DatiQuadroDatiNoCalcolo { TabRegistrazioniNoCalcolo = 0, Tipo = 2 });
                transactionScope.Complete();
            }
        }

        public static void GetDatiNoCalcolo(GestionePensione.DatiPensione datiPensione, long idRecordNoCalcolo, out Entity.DatiNoCalcolo datiNoCalcoloEntity)
        {
            datiNoCalcoloEntity = new Entity.DatiNoCalcolo();
            GestioneDatiNoCalcolo.RecordDatiNoCalcolo datiNoCalcoloBl = null;
            GestioneDatiNoCalcolo.GetRecordNoCalcoloByIdRecord(idRecordNoCalcolo, out datiNoCalcoloBl);

            GestioneQuadri.DatiQuadroRecordNoCalcolo datiQuadroDatiRecordNoCalcolo;
            GestioneQuadri.GetQuadroDatiRecordNoCalcoloByIdRecord(idRecordNoCalcolo, out datiQuadroDatiRecordNoCalcolo);

            ValorizzaEntityComponentiFamiliari(datiPensione, idRecordNoCalcolo, ref datiNoCalcoloEntity);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                Utility.ValorizzaOggetti(datiNoCalcoloBl, datiNoCalcoloEntity);

                if (datiQuadroDatiRecordNoCalcolo == null)
                {
                    datiQuadroDatiRecordNoCalcolo = new GestioneQuadri.DatiQuadroRecordNoCalcolo();
                    datiQuadroDatiRecordNoCalcolo.IdPensione = datiPensione.Id;
                    datiQuadroDatiRecordNoCalcolo.TabNoCalcolo = 0;
                    GestioneQuadri.SalvaQuadroDatiRecordNoCalcolo(datiPensione.Id, idRecordNoCalcolo, datiQuadroDatiRecordNoCalcolo);
                }
                datiNoCalcoloEntity.TabNoCalcolo = datiQuadroDatiRecordNoCalcolo.TabNoCalcolo;

                transactionScope.Complete();
            }
        }

        public static void StoreDatiNoCalcolo(GestionePensione.DatiPensione datiPensione, long idRecordNoCalcolo, ref Entity.DatiNoCalcolo datiNoCalcoloEntity)
        {
            GestioneDatiNoCalcolo.RecordDatiNoCalcolo datiNoCalcoloBl = new GestioneDatiNoCalcolo.RecordDatiNoCalcolo();
            long? idRecord = null;
            List<GestioneQuadri.DatiQuadroRecordNoCalcolo> lstDatiQuadroRecordNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiRecordNoCalcoloByDatiPensione(datiPensione, out lstDatiQuadroRecordNoCalcolo);

            GestioneQuadri.DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiNoCalcoloByDatiPensione(datiPensione, out quadroDatiNoCalcolo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                Utility.ValorizzaOggetti(datiNoCalcoloEntity, datiNoCalcoloBl);
                datiNoCalcoloBl.Id = idRecordNoCalcolo;
                datiNoCalcoloBl.IdPensione = datiPensione.Id;
                GestioneDatiNoCalcolo.SalvaRecordNoCalcolo(datiPensione.Id, datiNoCalcoloBl, out idRecord);
                datiNoCalcoloBl.Id = idRecord.GetValueOrDefault();

                #region Componenti Familiari
                GestioneComponenteFamiliare.EliminaComponentiFamiliariByIdRecordDatiNoCalcolo(idRecord.GetValueOrDefault());
                if (datiNoCalcoloEntity.ListaComponentiFamiliari != null && datiNoCalcoloEntity.ListaComponentiFamiliari.Count > 0)
                    foreach (Entity.DatiNoCalcolo.ComponentiFamiliari componente in datiNoCalcoloEntity.ListaComponentiFamiliari)
                    {
                        GestioneComponenteFamiliare.ComponenteFamiliare componenteDB = new GestioneComponenteFamiliare.ComponenteFamiliare();
                        componenteDB.IdPensione = datiPensione.Id;
                        componenteDB.IdRecordDatiNoCalcolo = datiNoCalcoloBl.Id;
                        componenteDB.CodiceFiscale = componente.CodiceFiscale;
                        GestioneComponenteFamiliare.SalvaComponenteFamiliare(componenteDB);
                    }

                #endregion Componenti Familiari

                //Gestione semaforo record 
                var quadroRecord = lstDatiQuadroRecordNoCalcolo.Find(x => x.IdRecordDatiNoCalcolo == idRecordNoCalcolo);
                if (datiNoCalcoloEntity.IsNull())
                {
                    quadroRecord.TabNoCalcolo = 0;
                    datiNoCalcoloEntity.TabNoCalcolo = 0;
                }
                else
                {
                    quadroRecord.TabNoCalcolo = 2;
                    datiNoCalcoloEntity.TabNoCalcolo = 2;
                }
                GestioneQuadri.SalvaQuadroDatiRecordNoCalcolo(datiPensione.Id, idRecordNoCalcolo, quadroRecord);

                //Gestione semaforo quadro
                if (lstDatiQuadroRecordNoCalcolo.TrueForAll(x => x.TabNoCalcolo == 2))
                    quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 2;
                else
                    quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 0;
                GestioneQuadri.SalvaQuadroDatiNoCalcolo(datiPensione.Id, quadroDatiNoCalcolo);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiNoCalcolo(GestionePensione.DatiPensione datiPensione, long idRecordNoCalcolo, out Entity.DatiNoCalcolo datiNoCalcoloEntity)
        {
            datiNoCalcoloEntity = new Entity.DatiNoCalcolo();
            GestioneDatiNoCalcolo.RecordDatiNoCalcolo datiNoCalcoloBl = new GestioneDatiNoCalcolo.RecordDatiNoCalcolo();
            long? idRecord = null;

            List<GestioneQuadri.DatiQuadroRecordNoCalcolo> lstDatiQuadroRecordNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiRecordNoCalcoloByDatiPensione(datiPensione, out lstDatiQuadroRecordNoCalcolo);

            GestioneQuadri.DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiNoCalcoloByDatiPensione(datiPensione, out quadroDatiNoCalcolo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {

                datiNoCalcoloBl = new GestioneDatiNoCalcolo.RecordDatiNoCalcolo();
                datiNoCalcoloBl.Id = idRecordNoCalcolo;
                datiNoCalcoloBl.IdPensione = datiPensione.Id;
                GestioneDatiNoCalcolo.SalvaRecordNoCalcolo(datiPensione.Id, datiNoCalcoloBl, out idRecord);
                datiNoCalcoloBl.Id = idRecord.GetValueOrDefault();

                GestioneComponenteFamiliare.EliminaComponentiFamiliariByIdRecordDatiNoCalcolo(datiNoCalcoloBl.Id);

                //Gestione semaforo record 
                var quadroRecord = lstDatiQuadroRecordNoCalcolo.Find(x => x.IdRecordDatiNoCalcolo == idRecordNoCalcolo);
                quadroRecord.TabNoCalcolo = 0;
                datiNoCalcoloEntity.TabNoCalcolo = 0;
                GestioneQuadri.SalvaQuadroDatiRecordNoCalcolo(datiPensione.Id, idRecordNoCalcolo, quadroRecord);
                //Gestione semaforo quadro
                if (lstDatiQuadroRecordNoCalcolo.TrueForAll(x => x.TabNoCalcolo == 2))
                    quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 2;
                else
                    quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 0;
                GestioneQuadri.SalvaQuadroDatiNoCalcolo(datiPensione.Id, quadroDatiNoCalcolo);

                transactionScope.Complete();
            }
        }

        public static void DeleteRecordNoCalcolo(GestionePensione.DatiPensione datiPensione, long idRecordNoCalcolo)
        {
            List<GestioneQuadri.DatiQuadroRecordNoCalcolo> lstDatiQuadroRecordNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiRecordNoCalcoloByDatiPensione(datiPensione, out lstDatiQuadroRecordNoCalcolo);

            GestioneQuadri.DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiNoCalcoloByDatiPensione(datiPensione, out quadroDatiNoCalcolo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneQuadri.EliminaQuadroDatiRecordNoCalcoloByIdRecord(idRecordNoCalcolo);

                GestioneComponenteFamiliare.EliminaComponentiFamiliariByIdRecordDatiNoCalcolo(idRecordNoCalcolo);

                lstDatiQuadroRecordNoCalcolo = lstDatiQuadroRecordNoCalcolo.FindAll(x => x.IdRecordDatiNoCalcolo != idRecordNoCalcolo);
                if (lstDatiQuadroRecordNoCalcolo.Count > 0 && lstDatiQuadroRecordNoCalcolo.TrueForAll(x => x.TabNoCalcolo == 2))
                    quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 2;
                else
                    quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 0;
                GestioneQuadri.SalvaQuadroDatiNoCalcolo(datiPensione.Id, quadroDatiNoCalcolo);

                GestioneDatiNoCalcolo.DeleteRecordDatiNoCalcolo(idRecordNoCalcolo);
                transactionScope.Complete();
            }
        }

        public static void DeleteAllRecordNoCalcolo(GestionePensione.DatiPensione datiPensione)
        {
            GestioneQuadri.DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo = null;
            GestioneQuadri.GetQuadroDatiNoCalcoloByDatiPensione(datiPensione, out quadroDatiNoCalcolo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneQuadri.EliminaQuadroDatiRecordNoCalcoloByIdPensione(datiPensione.Id);
                GestioneComponenteFamiliare.EliminaComponentiFamiliariByIdPensione(datiPensione.Id);
                GestioneDatiNoCalcolo.DeleteAllRecordDatiNoCalcolo(datiPensione.Id);

                quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo = 0;
                GestioneQuadri.SalvaQuadroDatiNoCalcolo(datiPensione.Id, quadroDatiNoCalcolo);
                transactionScope.Complete();
            }
        }

        public static void ValorizzaEntityComponentiFamiliari(GestionePensione.DatiPensione datiPensione, long idRecordNoCalcolo, ref Entity.DatiNoCalcolo datiNoCalcoloEntity)
        {
            List<GestioneFamiliari.Familiare> listaFamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaFamiliari, out listaAnagrafiche);

            List<GestioneComponenteFamiliare.ComponenteFamiliare> listaComponentiFamiliari = null;
            GestioneComponenteFamiliare.GetComponenteFamiliareByIdRecordDatiNoCalcolo(idRecordNoCalcolo, out listaComponentiFamiliari);

            if (listaFamiliari != null && listaFamiliari.Count > 0)
            {
                datiNoCalcoloEntity.ListaComponentiFamiliari = new List<Entity.DatiNoCalcolo.ComponentiFamiliari>();

                if (listaComponentiFamiliari != null && listaComponentiFamiliari.Count > 0)
                {
                    foreach (GestioneComponenteFamiliare.ComponenteFamiliare componente in listaComponentiFamiliari)
                    {
                        Entity.DatiNoCalcolo.ComponentiFamiliari entityComponente = new Entity.DatiNoCalcolo.ComponentiFamiliari();
                        entityComponente.CodiceFiscale = componente.CodiceFiscale;
                        entityComponente.IsSelected = true;
                        datiNoCalcoloEntity.ListaComponentiFamiliari.Add(entityComponente);
                    }
                }

                foreach (GestioneFamiliari.Familiare fam in listaFamiliari)
                {
                    if (!datiNoCalcoloEntity.ListaComponentiFamiliari.Exists(x => x.CodiceFiscale == fam.CodiceFiscale))
                    {
                        Entity.DatiNoCalcolo.ComponentiFamiliari componenteFamiliare = new Entity.DatiNoCalcolo.ComponentiFamiliari();
                        componenteFamiliare.CodiceFiscale = fam.CodiceFiscale;
                        datiNoCalcoloEntity.ListaComponentiFamiliari.Add(componenteFamiliare);
                    }
                }
            }
        }

        #region Controls
        public static bool ControlsDatiNoCalcolo(long idRecord, Entity.DatiNoCalcolo datiNoCalcoloEntity, GestionePensione.DatiPensione datiPensione, List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecordNoCalcolo, List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo, out string messageVideo)
        {
            DateCustom currentDecRec;
            messageVideo = string.Empty;

            if (string.IsNullOrEmpty(datiNoCalcoloEntity.Decorrenza))
            {
                messageVideo = "La decorrenza registrazione è un dato obbligatorio.";
                return false;
            }

            if (!DateCustom.TryParse(datiNoCalcoloEntity.Decorrenza, out currentDecRec, out messageVideo))
            {
                return false;
            }

            #region Controlli Decorrenza Registrazione

            //controllo sull'anno - l'anno della registrazione deve essere uguale all'anno corrente.
            DateTime dataSistema = Utility.DataSistemaFs;
            if (currentDecRec.Year > dataSistema.Year)
            {
                messageVideo = string.Format("La decorrenza registrazione ({0:dd/MM/yyyy}) non può avere un anno maggiore dell'anno corrente ({1})", currentDecRec, dataSistema.ToString("yyyy"));
                return false;
            }

            List<DateCustom> lstDcRecordStored = lstRecordNoCalcolo.Where(x => !string.IsNullOrEmpty(x.Decorrenza) && x.Id != idRecord).Select(x => DateCustom.Parse(x.Decorrenza)).ToList();
            if (lstDcRecordStored != null && lstDcRecordStored.Count > 0 && lstDcRecordStored.Exists(x => x == currentDecRec))
            {
                messageVideo = string.Format("Non è possibile inserire due registrazioni con stessa decorrenza registrazione ({0:dd/MM/yyyy})", currentDecRec);
                return false;
            }

            if (!GestioneControlli.ControlsDecPensioneWithDecNoCalcolo(datiPensione, datiNoCalcoloEntity.Decorrenza, out messageVideo))
                return false;

            //Controlli per cui la decorrenza di un record dovrà essere sempre successiva alla precedente

            //List<DateCustom> lstDateRegPrec = null ;
            //List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecorNoCalcoloPrecedenti = lstRecordNoCalcolo.Where(x => x.Decorrenza != null && x.Id < idRecord).ToList();
            //if (lstRecorNoCalcoloPrecedenti != null && lstRecorNoCalcoloPrecedenti.Count > 0)
            //{
            //    lstDateRegPrec = lstRecorNoCalcoloPrecedenti.Select(x => DateCustom.Parse(x.Decorrenza)).ToList();
            //}
            //if (lstDateRegPrec != null && lstDateRegPrec.Count > 0)
            //{
            //    //non primo record - controllo che la decorrenza inserita non sia maggiore della precedente
            //    var lastDecorRec = lstDateRegPrec.Last();
            //    if (lastDecorRec >= currentDecRec)
            //    {
            //        messageVideo = string.Format("La decorrenza registrazione inserita ({0}) deve essere maggiore della decorrenza del precedente record ({1})", currentDecRec, lastDecorRec);
            //        return false;
            //    }
            //}
            //List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecorNoCalcoloSuccessivi = lstRecordNoCalcolo.Where(x => x.Decorrenza != null && x.Id > idRecord).ToList();
            //List<DateCustom> lstDateRegSucc=null;
            //if (lstRecorNoCalcoloSuccessivi != null && lstRecorNoCalcoloSuccessivi.Count > 0)
            //{
            //    lstDateRegSucc = lstRecorNoCalcoloSuccessivi.Select(x => DateCustom.Parse(x.Decorrenza)).ToList();
            //}
            //if (lstDateRegSucc != null && lstDateRegSucc.Count > 0)
            //{
            //    //non primo record - controllo che la decorrenza inserita non sia maggiore della precedente
            //    var succDecorRec = lstDateRegSucc.First();
            //    if (succDecorRec <= currentDecRec)
            //    {
            //        messageVideo = string.Format("La decorrenza registrazione inserita ({0}) deve essere minore della decorrenza del record successivo ({1})", currentDecRec, succDecorRec);
            //        return false;
            //    }
            //}

            //Il primo record fondo inserito ha codice no calcolo 1  e il secondo ha codice no calcolo 0, ci sarà il controllo aggiuntivo per cui la dec.rec. non potrà essere maggiore alla data di decorrenza del record fondo con codice no calcolo a 0
            if(Utility.GetCategoriaFondoPI(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria) == null)
                if (!GestioneControlli.ControlsDecNoCalcoloWithRecordFondo(datiPensione, lstRecordFondo, datiNoCalcoloEntity.Decorrenza, out messageVideo))
                    return false;

            #endregion Controlli Decorrenza Registrazione

            #region Controlli Dati Registrazione
            if (!currentDecRec.isTredicesima())
            {
                //non tredicesima
                if (!datiNoCalcoloEntity.ImportoMensile.HasValue)
                {
                    messageVideo = "Importo mensile è un dato obbligatorio.";
                    return false;
                }
                if (!datiNoCalcoloEntity.AdeguataFondo.HasValue)
                {
                    messageVideo = "AdeguataFondo è un dato obbligatorio.";
                    return false;
                }
                if (datiNoCalcoloEntity.Tredicesima.HasValue)
                {
                    messageVideo = "Tredicesima non deve essere acquisito.";
                    return false;
                }
                //Importo mesile deve essere uguale alla somma importo 2 - 12
                decimal sommaCampi = datiNoCalcoloEntity.AdeguataAgo.GetValueOrDefault() + datiNoCalcoloEntity.AdeguataFondo.GetValueOrDefault() +
                    datiNoCalcoloEntity.AggFamigliaFondo.GetValueOrDefault() + datiNoCalcoloEntity.Art21.GetValueOrDefault() + datiNoCalcoloEntity.AssegniFamiliari.GetValueOrDefault() +
                    datiNoCalcoloEntity.EccedenzaAgo.GetValueOrDefault() + datiNoCalcoloEntity.FacArt14.GetValueOrDefault() + datiNoCalcoloEntity.IndIntSpeciale.GetValueOrDefault() +
                    datiNoCalcoloEntity.OnereCaricoAmm.GetValueOrDefault() + datiNoCalcoloEntity.QuotaAgoEsclusiva.GetValueOrDefault();
                if (sommaCampi != datiNoCalcoloEntity.ImportoMensile)
                {
                    messageVideo = string.Format("L’importo mensile ({0}) dovrà essere uguale alla somma degli altri campi inseriti ({1}).", datiNoCalcoloEntity.ImportoMensile, sommaCampi);
                    return false;
                }
            }
            else
            {
                //tredicesima
                //campi Importo mensile e Agg.famiglia fondo devono essere disabilitati;
                if (datiNoCalcoloEntity.ImportoMensile.HasValue)
                {
                    messageVideo = "Per tredicesima il campo importo mensile non deve essere inserito.";
                    return false;
                }
                if (datiNoCalcoloEntity.AggFamigliaFondo.HasValue)
                {
                    messageVideo = "Per tredicesima il campo Agg. Famiglia Fondo non deve essere inserito.";
                    return false;
                }
                if (!datiNoCalcoloEntity.Tredicesima.HasValue)
                {
                    messageVideo = "Il campo tredicesima è un data obbligatorio.";
                    return false;
                }
                //Sarà prevista l’acquisizione obbligatoria del campo tredicesima ed almeno un ulteriore campo ;
                //L’importo della tredicesima dovrà essere uguale alla somma degli altri campi acquisiti.
                decimal sommaCampi = datiNoCalcoloEntity.AdeguataAgo.GetValueOrDefault() + datiNoCalcoloEntity.AdeguataFondo.GetValueOrDefault() +
                    datiNoCalcoloEntity.AggFamigliaFondo.GetValueOrDefault() + datiNoCalcoloEntity.Art21.GetValueOrDefault() + datiNoCalcoloEntity.AssegniFamiliari.GetValueOrDefault() +
                    datiNoCalcoloEntity.EccedenzaAgo.GetValueOrDefault() + datiNoCalcoloEntity.FacArt14.GetValueOrDefault() + datiNoCalcoloEntity.IndIntSpeciale.GetValueOrDefault() +
                    datiNoCalcoloEntity.OnereCaricoAmm.GetValueOrDefault() + datiNoCalcoloEntity.QuotaAgoEsclusiva.GetValueOrDefault();

                if (sommaCampi == 0)
                {
                    // non è stata inserito nessun campo
                    messageVideo = "Deve essere acquisito almeno un campo oltre a tredicesima.";
                    return false;
                }

                if (sommaCampi != datiNoCalcoloEntity.Tredicesima)
                {
                    // somma campi diversi da tredicesima
                    messageVideo = "Il campo tredicesima deve essere uguale alla somma degli altri campi inseriti.";
                    return false;
                }
            }
            #endregion Controlli Dati Registrazione

            return true;
        }



        public static bool ControlAddRecordNoCalcolo(List<GestioneDatiNoCalcolo.RecordDatiNoCalcolo> lstRecordNoCalcolo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            bool ret = true;
            if (lstRecordNoCalcolo != null && lstRecordNoCalcolo.Count >= 20)
            {
                messaggioVideo = "Non è possibile inserire più di 20 registrazioni.";
                ret = false;
            }
            return ret;
        }


        #endregion Controls

        #region Nestled Class
        public class DateCustom : IComparable
        {
            public int Day { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }

            public DateCustom()
            { }
            public DateCustom(DateTime decorrenzaPensione)
            {
                this.Day = decorrenzaPensione.Day;
                this.Month = decorrenzaPensione.Month;
                this.Year = decorrenzaPensione.Year;
            }

            public override string ToString()
            {
                return string.Format(@"{0:00}/{1:00}/{2:0000}", this.Day, this.Month, this.Year);
            }

            public override int GetHashCode()
            {
                return this.Day.GetHashCode() + this.Month.GetHashCode() + this.Year.GetHashCode();
            }

            internal static DateCustom Parse(string sDateCust)
            {
                DateCustom date = new DateCustom();
                date.Day = int.Parse(sDateCust.Substring(0, 2));
                date.Month = int.Parse(sDateCust.Substring(3, 2));
                date.Year = int.Parse(sDateCust.Substring(6, 4));
                return date;
            }

            public bool isTredicesima()
            {
                return this.Month == 13;
            }

            public static bool TryParse(string sDate, out DateCustom date, out string errore)
            {
                int gg;
                int mm;
                date = null;
                bool ret = true;
                errore = string.Empty;
                if (sDate == null)
                {
                    ret = false;
                    errore = "La data deve essere valorizzata.";
                }
                else if (sDate.Length != 10 || sDate[2] != '/' || sDate[5] != '/')
                {
                    ret = false;
                    errore = "La data essere essere deve nel formato GG/MM/AAAA.";
                }
                else if (!int.TryParse(sDate.Substring(0, 2), out gg) || gg < 1 || gg > 31)
                {
                    errore = "Il giorno deve essere un numero compreso tra 1 e 31.";
                    ret = false;
                }
                else if (!int.TryParse(sDate.Substring(3, 2), out mm) || mm < 1 || mm > 13)
                {
                    errore = "Il mese deve essere un numero compreso tra 1 e 13.";
                    ret = false;
                }
                else
                {

                    if (mm == 13)
                    {
                        //il giorno deve essere 1
                        if (gg != 1)
                        {
                            errore = "Nel caso di tredicesima il giorno deve essere 1.";
                            ret = false;
                        }
                    }
                    else
                    {
                        DateTime dateTime;
                        if (!DateTime.TryParse(sDate, out dateTime))
                        {
                            errore = "Data nel formato non corretto";
                            ret = false;
                        }
                    }
                }
                //valorizzo la data
                date = DateCustom.Parse(sDate);
                return ret;
            }

            public override bool Equals(object obj)
            {
                DateCustom date = (DateCustom)obj;
                return this.Day == date.Day && this.Month == date.Month && this.Year == date.Year;
            }

            public static bool operator <(DateCustom diff1, DateCustom diff2)
            {
                if (diff1.Year < diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month < diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day < diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator >(DateCustom diff1, DateCustom diff2)
            {
                if (diff1.Year > diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month > diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day > diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator <=(DateCustom diff1, DateCustom diff2)
            {
                if (diff1.Year < diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month < diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day <= diff2.Day)
                            return true;
                    }
                }

                return false;
            }

            public static bool operator >=(DateCustom diff1, DateCustom diff2)
            {
                if (diff1.Year > diff2.Year)
                    return true;

                if (diff1.Year == diff2.Year)
                {
                    if (diff1.Month > diff2.Month)
                        return true;

                    if (diff1.Month == diff2.Month)
                    {
                        if (diff1.Day >= diff2.Day)
                            return true;
                    }
                }
                return false;
            }

            public static bool operator ==(DateCustom diff1, DateCustom diff2)
            {
                if (diff1.Year == diff2.Year && diff1.Month == diff2.Month && diff1.Day == diff2.Day)
                    return true;

                return false;
            }

            public static bool operator !=(DateCustom diff1, DateCustom diff2)
            {
                if (diff1.Year != diff2.Year || diff1.Month != diff2.Month || diff1.Day != diff2.Day)
                    return true;

                return false;
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                if (obj is DateCustom)
                {
                    DateCustom otherDateCustom = (DateCustom)obj;
                    if (this < otherDateCustom)
                        return -1;
                    else if (this == otherDateCustom)
                        return 0;

                    return 1;
                }
                else
                {
                    throw new ArgumentException("Object is not a DateCustom");
                }
            }

            #endregion IComparable Members
        }
        #endregion Nestled Class
    }
}
