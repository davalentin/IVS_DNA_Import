using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.LiquidazioneAgo.Entity;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestioneDatiFondo
    {
        #region Public Methods

        #region Registrazione Fondo

        public static void GetDatiRecordFondoByIdPensione(GestionePensione.DatiPensione datiPensione, out DatiRegistrazioneFondo datiRegistrazioneFondo)
        {
            datiRegistrazioneFondo = new DatiRegistrazioneFondo();
            datiRegistrazioneFondo.lRecordFondo = new List<DatiRegistrazioneFondo.DatiRecordFondo>();

            List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo = null;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstRecordFondo);

            if (lstRecordFondo != null && lstRecordFondo.Count > 0)
            {
                List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstSemaforiRecordFondo = null;
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstSemaforiRecordFondo);

                foreach (GestioneRecordFondo.DatiRecordFondo recFondo in lstRecordFondo)
                {
                    GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroRecordFondo = lstSemaforiRecordFondo.Where(x => x.IdRecordFondo == recFondo.Id).FirstOrDefault();
                    if (datiQuadroRecordFondo != null)
                    {
                        datiRegistrazioneFondo.lRecordFondo.Add(new DatiRegistrazioneFondo.DatiRecordFondo()
                        {
                            IdRecordFondo = recFondo.Id,
                            DecorrenzaValiditaDati = recFondo.DecorrenzaValiditaDati,
                            TabArticolo2 = datiQuadroRecordFondo.TabArticolo2,
                            TabDatiCalcolo = datiQuadroRecordFondo.TabDatiCalcolo,
                            TabDatiFondo = datiQuadroRecordFondo.TabDatiFondo,
                            TabPrivilegiate = datiQuadroRecordFondo.TabPrivilegiate
                        });
                    }
                }
            }
        }

        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, out DateTime? DecorrenzaPensioneDirettaDC)
        {
            Dictionary<string, bool?> crossProperties = new Dictionary<string, bool?>();

            bool? IsDecPensAnteAgosto95 = null;

            IsDecPensAnteAgosto95 = GetIsDecPensAnteAgosto95(datiPensione);
            DecorrenzaPensioneDirettaDC = GetDecorrenzaPensioneDirettaDC(datiPensione);

            crossProperties.Add("IsDecPensAnteAgosto95", IsDecPensAnteAgosto95);

            return crossProperties;
        }

        public static void AddRecordFondo(GestionePensione.DatiPensione datiPensione, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, 
            ref List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lastLstServizioUtile, out GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, out long idRecordFondo)
        {
            idRecordFondo = -1;
            long idPensione = datiPensione.Id;

            List<GestioneRecordFondo.DatiRecordFondo> lstOldRecordFondo;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstOldRecordFondo);
            GestioneRecordFondo.DatiRecordFondo lastRecordFondo = lstOldRecordFondo.OrderByDescending(x => x.Id).FirstOrDefault();

            GestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdRecordFondo(lastRecordFondo.Id, out lastLstServizioUtile);

            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(lastRecordFondo.Id, out recordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
                new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);
                idRecordFondo = recordFondo.Id;
                //replico PensioneFondo uguale all'ultimo record a db
                //dati fondo
                Utility.ValorizzaOggetti(new Entity.DatiFondo(), recordDatiFondoINPDAP);
                //datiArticolo2
                Utility.ValorizzaOggetti(new Entity.DatiArticolo2(), recordDatiFondoINPDAP);
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(idPensione, idRecordFondo, recordDatiFondoINPDAP);

                //replico servizio utile uguale all'ultimo record salvato a db
                if (lastLstServizioUtile != null && lastLstServizioUtile.Count > 0)
                {
                    foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile servizioUtile in lastLstServizioUtile)
                        GestioneDatiServizioUtileINPDAP.SalvaDatiServizioUtileRecordFondo(servizioUtile.IdPensione.Value, idRecordFondo, servizioUtile);
                }
                //Verifico se il tab Privilegiate viene precompilato -> metto semaforo a rosso
                Entity.DatiPrivilegiate datiPrivilegiate = new DatiPrivilegiate();
                Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiPrivilegiate);
                List<GestioneRecordFondo.DatiRecordFondo> lstDatiRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>() { recordFondo };
                if (!datiPrivilegiate.IsDatiPrivilegiateNull())
                    datiQuadroDatiRecordFondo = GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, lstDatiRecordFondo, Utility.TipoAppartenenza.AGO, tipoDomanda, true, null, true, null).FirstOrDefault();
                else
                    datiQuadroDatiRecordFondo = GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, lstDatiRecordFondo, Utility.TipoAppartenenza.AGO, tipoDomanda, true, null, null, null).FirstOrDefault();

                //set a rosso semaforo quadro
                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiRecordFondoByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo;
            GestioneQuadri.GetQuadroDatiFondoByDatiPensione(datiPensione, out datiQuadroDatiFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo;
            datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = DNA.Data.TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
               new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                //elimina DatiFondo
                GestioneRecordDatiFondoINPDAP.EliminaRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo);
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                GestioneQuadri.EliminaQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo);
                GestioneRecordFondo.EliminaRecordFondo(idRecordFondo);
                //semafori
                byte? newSemValue = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo.Where(x => x.IdRecordFondo != idRecordFondo).ToList());
                if (datiQuadroDatiFondo.TabRegistrazioniFondo != newSemValue)
                {
                    datiQuadroDatiFondo.TabRegistrazioniFondo = newSemValue;
                    GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);
                }
                transactionScope.Complete();
            }
            lstDatiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.FindAll(x => x.IdRecordFondo != idRecordFondo);
        }

        public static void EliminaDatiRecordFondoByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstRecordFondo);
            lstRecordFondo.Sort((x, y) => x.Id.CompareTo(y.Id));
            GestioneRecordFondo.DatiRecordFondo primoRecord = lstRecordFondo.FirstOrDefault();
            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(primoRecord.Id, out recordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = DNA.Data.TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
               new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                //pulisco il primo record e mantengo invariati i dati bloccati
                if (recordDatiFondoINPDAP != null)
                {
                    Entity.DatiFondo entityDatiFondo = new DatiFondo();
                    entityDatiFondo.TrediciMensilita = recordDatiFondoINPDAP.TrediciMensilita;
                    entityDatiFondo.IndennitaIntegrativaSpecialeConglobata = recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata;
                    Utility.ValorizzaOggetti(entityDatiFondo, recordDatiFondoINPDAP);

                    Entity.DatiCalcolo entityDatiCalcolo = new Entity.DatiCalcolo();
                    Utility.ValorizzaOggetti(entityDatiCalcolo, recordDatiFondoINPDAP);

                    Entity.DatiArticolo2 entityDatiArticolo2 = new DatiArticolo2();
                    Utility.ValorizzaOggetti(entityDatiArticolo2, recordDatiFondoINPDAP);

                    Entity.DatiPrivilegiate entityDatiPrivilegiate = new Entity.DatiPrivilegiate();
                    Utility.ValorizzaOggetti(new Entity.DatiPrivilegiate(), recordDatiFondoINPDAP);

                    GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, recordDatiFondoINPDAP.IdRecordFondo, recordDatiFondoINPDAP);

                    GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, new List<GestioneRecordFondo.DatiRecordFondo>() { primoRecord }, Utility.TipoAppartenenza.AGO, tipoDomanda, true, null, null, null);
                }

                //elimino dati servizio utile
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                //elimino tutti dati pensioneFondoSpecifico tranne il primo record
                foreach (var elem in lstRecordFondo.GetRange(1, lstRecordFondo.Count - 1))
                {
                    GestioneRecordDatiFondoINPDAP.EliminaRecordDatiFondoINPDAPByIdRecordFondo(elem.Id);
                    GestioneQuadri.EliminaQuadroDatiRecordFondoByIdRecordFondo(elem.Id);
                    GestioneRecordFondo.EliminaRecordFondo(elem.Id);
                }
                //Set a rosso semaforo quadro
                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);
                transactionScope.Complete();
            }
        }

        #endregion Registrazione Fondo

        #region Dati Fondo
        public static void GetDatiFondoByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref GestioneRecordFondo.DatiRecordFondo datiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, ref GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe, out DatiFondo datiFondo)
        {
            datiFondo = new DatiFondo();
            if (datiRecordFondo == null)
                GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out datiRecordFondo);

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (datiControlloFelpe == null)
                GestioneDatiControlloFelpe.GetDatiControlloFelpeByIdPensione(datiPensione.Id, out datiControlloFelpe);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiFondo);

            datiFondo.DecorrenzaValidita = datiRecordFondo.DecorrenzaValiditaDati;
            datiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiFondo;
            datiFondo.TipoPensione = Utility.GetTipoPensione(datiPensione).Keys.FirstOrDefault();
            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione)
                datiFondo.DecorrenzaCalcolo = GetDecorrenzaCalcolo(datiPensione, datiControlloFelpe != null ? datiControlloFelpe.InizioBonus : null);
        }

        public static void StoreDatiFondoByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, Entity.DatiFondo entityDatiFondo, bool isCancelOperation)
        {
            if (entityDatiFondo == null)
                entityDatiFondo = new Entity.DatiFondo();

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            GestioneRecordFondo.DatiRecordFondo recordFondo;
            GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out recordFondo);

            // Mantengo invariati i campi bloccati
            entityDatiFondo.TrediciMensilita = recordDatiFondoINPDAP.TrediciMensilita;
            entityDatiFondo.IndennitaIntegrativaSpecialeConglobata = recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata;

            Utility.ValorizzaOggetti(entityDatiFondo, recordDatiFondoINPDAP);

            #region Gestione Semaforica

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            #endregion Gestione Semaforica

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (isCancelOperation)
                    datiQuadroRecordFondo.TabDatiFondo = 0;
                else
                    datiQuadroRecordFondo.TabDatiFondo = 2;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroRecordFondo);

                recordFondo.DecorrenzaValiditaDati = entityDatiFondo.DecorrenzaValidita;
                GestioneRecordFondo.SalvaSingoloRecordFondo(datiPensione.Id, recordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            entityDatiFondo.Semaforo = datiQuadroRecordFondo.TabDatiFondo;
        }

        public static bool ControlsDatiFondo(GestionePensione.DatiPensione datiPensione, long idRecordFondo, Entity.DatiFondo entityDatiFondo,
            Entity.DatiArticolo2 entityDatiArticolo2, ref List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (entityDatiFondo == null)
                entityDatiFondo = new Entity.DatiFondo();

            if (listaRecordFondo == null)
                GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out listaRecordFondo);

            bool isPrimoRecord = listaRecordFondo.First().Id == idRecordFondo;

            List<GestioneRecordFondo.DatiRecordFondo> app = listaRecordFondo.FindAll(x => x.Id < idRecordFondo && x.DecorrenzaValiditaDati.HasValue);
            DateTime? ultimaDecorrenzaValidita = null;
            if (app != null && app.Count > 0)
                ultimaDecorrenzaValidita = app.LastOrDefault().DecorrenzaValiditaDati;
            if (!ultimaDecorrenzaValidita.HasValue && !isPrimoRecord)
                ultimaDecorrenzaValidita = datiPensione.DecorrenzaOriginaria;

            if (isSingleTab)
            {
                Utility.ValorizzaOggetti(recordDatiFondoINPDAP, entityDatiArticolo2);
            }

            if (!entityDatiFondo.DecorrenzaValidita.HasValue)
            {
                messaggioVideo = "La Decorrenza Registrazione è obbligatoria.";
                return false;
            }

            if (!entityDatiFondo.TrediciMensilita.HasValue)
            {
                messaggioVideo = "Tredicesima Mensilità: campo obbligatorio";
                return false;
            }

            if (ultimaDecorrenzaValidita.HasValue && Utility.DataSuccessivaA(ultimaDecorrenzaValidita.Value, entityDatiFondo.DecorrenzaValidita.Value))
            {
                messaggioVideo = "La decorrenza registrazione deve essere posteriore alla decorrenza della precedente registrazione.";
                return false;
            }

            if (entityDatiFondo.DecorrenzaCalcolo.HasValue && entityDatiFondo.DecorrenzaValidita < entityDatiFondo.DecorrenzaCalcolo)
            {
                messaggioVideo = "La decorrenza registrazione deve essere maggiore della decorrenza calcolo";
                return false;
            }

            if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(entityDatiArticolo2 != null ? entityDatiArticolo2.ScadenzaBenefici : null, entityDatiFondo.DecorrenzaValidita,
                out messaggioVideo))
                return false;

            return true;
        }

        #endregion

        #region Dati Calcolo

        public static void GetDatiCalcoloByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtileCommon, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out Entity.DatiCalcolo datiCalcoloForDatiFondo)
        {
            datiCalcoloForDatiFondo = new Entity.DatiCalcolo();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            //servizio utile
            if (lServizioUtileCommon == null)
                GestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdRecordFondo(idRecordFondo, out lServizioUtileCommon);

            if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
            {
                datiCalcoloForDatiFondo.lDatiServizioUtile = lServizioUtileCommon;
            }

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiCalcoloForDatiFondo);

            datiCalcoloForDatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;
            GestioneContrib.TipoCalcolo tipoCalcolo;
            GestioneContrib.GetTipoCalcoloByDatiPensione(datiPensione, out tipoCalcolo);
            datiCalcoloForDatiFondo.TipoCalcolo = tipoCalcolo;
        }

        public static bool ControlsDatiCalcolo(GestionePensione.DatiPensione datiPensione, Entity.DatiCalcolo entityDatiCalcolo, Entity.DatiArticolo2 entityDatiArticolo2,
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneContrib.TipoCalcolo tipoCalcolo = GestioneContrib.TipoCalcolo.NonValido;
            GestioneContrib.GetTipoCalcoloByDatiPensione(datiPensione, out tipoCalcolo);

            if (tipoCalcolo == GestioneContrib.TipoCalcolo.NonValido)
            {
                messaggioVideo = "Tipo calcolo non selezionato in Liquidazione Pensione.";
                return false;
            }

            if (tipoCalcolo != entityDatiCalcolo.TipoCalcolo)
            {
                messaggioVideo = "Il tipo calcolo '" + tipoCalcolo.ToString() + "' salvato sul quadro Liquidazione Pensione è differente dai dati calcolo che si sta tentando di salvare";
                return false;
            }

            if (entityDatiCalcolo.lDatiServizioUtile != null && entityDatiCalcolo.lDatiServizioUtile.Count > 0)
            {
                if (!GestioneControlli.ControlsDatiServizioUtile(entityDatiCalcolo.lDatiServizioUtile, out messaggioVideo))
                    return false;

                if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(entityDatiCalcolo.lDatiServizioUtile, datiPensione.FineAssicurazione, out messaggioVideo))
                    return false;
            }

            if (!GestioneControlli.ControlsDatiCalcoloRecordFondo(datiPensione, entityDatiCalcolo.PensioneAnnuaLorda, entityDatiCalcolo.ServizioUtileDiritto, entityDatiCalcolo.lDatiServizioUtile,
                tipoCalcolo, out messaggioVideo))
                return false;

            decimal? palConBenefici = entityDatiArticolo2 != null ? entityDatiArticolo2.PALConBenefici : recordDatiFondoINPDAP != null ? recordDatiFondoINPDAP.PALConBenefici : null;

            if (!GestioneControlli.ControlPALBeneficiPAL(palConBenefici, entityDatiCalcolo.PensioneAnnuaLorda, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            return true;
        }

        public static void StoreDatiCalcoloByidRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, DatiCalcolo datiCalcolo)
        {
            if (datiCalcolo == null)
                return;

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                SalvaDatiCalcoloByIdRecordFondo(datiCalcolo, idRecordFondo, datiPensione, ref recordDatiFondoINPDAP);
                datiQuadroDatiRecordFondo.TabDatiCalcolo = 2;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiCalcolo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;
        }

        private static void SalvaDatiCalcoloByIdRecordFondo(DatiCalcolo datiCalcolo, long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiCalcolo == null)
                datiCalcolo = new DatiCalcolo();

            if (recordDatiFondoINPDAP == null)
                recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();

            Utility.ValorizzaOggetti(datiCalcolo, recordDatiFondoINPDAP);
            GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

            if (datiCalcolo.lDatiServizioUtile != null && datiCalcolo.lDatiServizioUtile.Count > 0)
            {
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile su in datiCalcolo.lDatiServizioUtile)
                    GestioneDatiServizioUtileINPDAP.SalvaDatiServizioUtileRecordFondo(datiPensione.Id, idRecordFondo, su);
            }
        }

        public static void DeleteDatiCalcoloByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            ref GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Entity.DatiCalcolo entityDatiCalcolo = new DatiCalcolo();
            Utility.ValorizzaOggetti(entityDatiCalcolo, recordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    datiQuadroDatiRecordFondo.TabDatiCalcolo = 0;
                else
                    datiQuadroDatiRecordFondo.TabDatiCalcolo = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        #endregion

        #region Dati Privilegiate

        public static void GetDatiPrivilegiateByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out Entity.DatiPrivilegiate datiPrivilegiate)
        {
            datiPrivilegiate = new Entity.DatiPrivilegiate();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiPrivilegiate);

            datiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;
        }

        public static void StoreDatiPrivilegiateByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, Entity.DatiPrivilegiate datiPrivilegiate)
        {
            if (datiPrivilegiate == null)
                datiPrivilegiate = new Entity.DatiPrivilegiate();

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            Utility.ValorizzaOggetti(datiPrivilegiate, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (datiPrivilegiate.IsDatiPrivilegiateNull())
                    datiQuadroDatiRecordFondo.TabPrivilegiate = 1;
                else
                    datiQuadroDatiRecordFondo.TabPrivilegiate = 2;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;
        }

        public static void EliminaDatiPrivilegiateByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            Entity.DatiPrivilegiate entityDatiPrivilegiate = new DatiPrivilegiate();
            Utility.ValorizzaOggetti(entityDatiPrivilegiate, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                datiQuadroDatiRecordFondo.TabPrivilegiate = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiPrivilegiate(Entity.DatiPrivilegiate datiPrivilegiate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;


            return true;
        }

        #endregion Dati Privilegiate

        #region Dati Articolo 2

        public static void GetDatiArticolo2ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out DatiArticolo2 datiArticolo2)
        {
            datiArticolo2 = new DatiArticolo2();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiArticolo2);

            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
        }

        public static void StoreDatiArticolo2ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, Entity.DatiArticolo2 datiArticolo2,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiArticolo2 == null)
                datiArticolo2 = new Entity.DatiArticolo2();

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            Utility.ValorizzaOggetti(datiArticolo2, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (datiArticolo2.IsNull())
                    datiQuadroDatiRecordFondo.TabArticolo2 = 1;
                else
                    datiQuadroDatiRecordFondo.TabArticolo2 = 2;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
        }

        public static void EliminaDatiArticolo2ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            Entity.DatiArticolo2 entityArticolo2 = new DatiArticolo2();
            Utility.ValorizzaOggetti(entityArticolo2, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                datiQuadroDatiRecordFondo.TabArticolo2 = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiArticolo2(Entity.DatiArticolo2 datiArticolo2, GestioneRecordFondo.DatiRecordFondo recordFondo, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiArticolo2 == null || datiArticolo2.IsNull())
                return true;

            if (datiArticolo2.PALConBenefici.HasValue && (datiArticolo2.ScadenzaIllimitata != true && !datiArticolo2.ScadenzaBenefici.HasValue))
            {
                messaggioVideo = "In caso di inserimento 'PAL con benefici' è obbligatorio inserire la data 'Scadenza beneficio' o il flag 'beneficio illimitato'.";
                return false;
            }
            if (!datiArticolo2.PALConBenefici.HasValue && (datiArticolo2.ScadenzaIllimitata == true || datiArticolo2.ScadenzaBenefici.HasValue))
            {
                messaggioVideo = "In caso di inserimento della data 'Scadenza beneficio' o il flag 'beneficio illimitato' è obbligatorio inserire 'PAL con benefici'.";
                return false;
            }
            if (datiArticolo2.ScadenzaIllimitata == true && datiArticolo2.ScadenzaBenefici.HasValue)
            {
                messaggioVideo = "Non è possibile inserire sia la data 'Scadenza beneficio' che flag 'Beneficio illimitato'.";
                return false;
            }

            if (recordDatiFondoINPDAP != null)
            {
                if (!GestioneControlli.ControlPALBeneficiPAL(datiArticolo2.PALConBenefici, recordDatiFondoINPDAP.PensioneAnnuaLorda, out messaggioVideo))
                {
                    messaggioVideo = "Art 2 Comma 12 L.335: " + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(datiArticolo2.ScadenzaBenefici, recordFondo != null ? recordFondo.DecorrenzaValiditaDati : null, out messaggioVideo))
                return false;

            return true;
        }

        #endregion

        #endregion Public Methods

        #region Private Methods
        private static DateTime? GetDecorrenzaPensioneDirettaDC(GestionePensione.DatiPensione datiPensione)
        {
            if (Utility.IsDomandaReversibilita(datiPensione))
            {
                GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                return datiDanteCausa.DecorrenzaPensione;
            }

            return null;
        }

        private static bool? GetIsDecPensAnteAgosto95(GestionePensione.DatiPensione datiPensione)
        {
            DateTime dataCompare = new DateTime(1995, 8, 17);
            if (datiPensione.DecorrenzaOriginaria.HasValue && !Liquidazione.BLCommon.Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare))
                return true;

            return false;
        }

        private static DateTime? GetDecorrenzaCalcolo(GestionePensione.DatiPensione datiPensione, DateTime? dataInizioBonus)
        {
            DateTime? decorrenzaCalcolo = null;

            //mail 26-05-2015 
            //Nel caso di domande da bonus (secondo codice natura = Y) occorrerà valorizzare la data calcolo con la data di inizio bonus (assumendo il primo giorno del mese)
            if (dataInizioBonus.HasValue)
                decorrenzaCalcolo = new DateTime(dataInizioBonus.Value.Year, dataInizioBonus.Value.Month, 1);
            else
            {
                DateTime? decorrenzaCalcoloFondoCompare = new DateTime(1995, 10, 1);

                string messaggioVideo = string.Empty;
                DateTime? decorrenzaPensioneCompare = new DateTime(1988, 1, 1);
                DateTime? decorrenzaCalcoloCompare = new DateTime(1992, 1, 1);

                switch (datiPensione.Gruppo)
                {
                    case "0001":
                    case "0002":
                        if (!Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaPensioneCompare.Value))
                        {
                            decorrenzaCalcolo = decorrenzaCalcoloFondoCompare;
                        }
                        if (Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaPensioneCompare.Value) && !Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloCompare.Value))
                        {
                            decorrenzaCalcolo = decorrenzaPensioneCompare;

                        }
                        if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaPensioneCompare.Value))
                        {
                            decorrenzaCalcolo = datiPensione.DecorrenzaOriginaria;
                        }
                        break;
                    case "0003":
                        if (!Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloFondoCompare.Value))
                        {
                            decorrenzaCalcolo = decorrenzaCalcoloFondoCompare;

                        }
                        if (Utility.DataStrettamenteSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, decorrenzaCalcoloFondoCompare.Value))
                        {
                            decorrenzaCalcolo = datiPensione.DecorrenzaOriginaria;
                        }
                        break;
                }
            }
            return decorrenzaCalcolo;
        }

        private static byte? GetValueTabRegistrazioneFondo(List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroRecordFondo)
        {
            byte? ret = null;
            if (lstDatiQuadroRecordFondo.TrueForAll(x => x.TabArticolo2 != 0 && x.TabDatiCalcolo != 0 && x.TabDatiFondo != 0 && x.TabLegge460 != 0 && x.TabPrivilegiate != 0))
            {
                ret = 2;
            }
            else
                ret = 0;
            return ret;
        }
        #endregion Private Methods
    }
}
