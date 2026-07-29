using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.LiquidazioneFs.Entity;
using INPS.Pensioni.LiquidazioneFs.ServiceReferences.AggPec;

namespace INPS.Pensioni.LiquidazioneFs
{
    public class GestioneDatiFondo
    {
        #region Public Members
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
                        datiRegistrazioneFondo.lRecordFondo.Add(new DatiRegistrazioneFondo.DatiRecordFondo
                        {
                            IdRecordFondo = recFondo.Id,
                            DecorrenzaValiditaDati = recFondo.DecorrenzaValiditaDati,
                            TabDatiCalcoloDZ = datiQuadroRecordFondo.TabDatiCalcoloDZ,
                            TabArticolo2 = datiQuadroRecordFondo.TabArticolo2,
                            TabDatiCalcolo = datiQuadroRecordFondo.TabDatiCalcolo,
                            TabDatiFondo = datiQuadroRecordFondo.TabDatiFondo,
                            TabDatiCalcolo707 = datiQuadroRecordFondo.TabDatiCalcolo707,
                            TabLegge460 = datiQuadroRecordFondo.TabLegge460,
                            TabPrivilegiate = datiQuadroRecordFondo.TabPrivilegiate,
                            TabQuoteMiglioramentiContrattuali = datiQuadroRecordFondo.TabMiglioramentiContrattualiFS
                        });
                    }
                }
            }
        }

        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondo, out DateTime? DecorrenzaPensioneDirettaDC)
        {
            Dictionary<string, bool?> crossProperties = new Dictionary<string, bool?>();

            bool? IsDecPensAnteAgosto95 = null;
            bool? IsContribL214Visible = null;
            bool? isSperimentaleDonna = null;
            bool? isPensioneTipoContributivo = null;
            bool? isPensioneTipoContributivoConOpzione = null;
            bool? isRiduzioneRetributiva = null;

            DecorrenzaPensioneDirettaDC = GetDecorrenzaPensioneDirettaDC(datiPensione);
            IsDecPensAnteAgosto95 = GetIsDecPensAnteAgosto95(datiPensione, DecorrenzaPensioneDirettaDC);
            IsContribL214Visible = GestioneContributivoL214ForTipoFondo(datiPensione, datiFondo);
            isSperimentaleDonna = Utility.IsDomandaSperimentaleDonnaOrRicostituzione(datiPensione) || Utility.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione(datiPensione);
            isPensioneTipoContributivo = Utility.IsDomandaTipoContributivo(datiPensione, null, null);
            isPensioneTipoContributivoConOpzione = Utility.IsDomandaTipoContributivo(datiPensione, null, true);
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            isRiduzioneRetributiva = GestioneRiduzioneRetributiva(datiPensione, isRiapertura);   // istruttoria


            crossProperties.Add("ContribL214Visible", IsContribL214Visible);
            crossProperties.Add("IsDecPensAnteAgosto95", IsDecPensAnteAgosto95);
            crossProperties.Add("IsDomandaSperimentaleDonna", isSperimentaleDonna);
            crossProperties.Add("IsPensioneTipoContributivo", isPensioneTipoContributivo);
            crossProperties.Add("IsPensioneTipoContributivoConOpzione", isPensioneTipoContributivoConOpzione);
            crossProperties.Add("IsRiduzioneRetributiva", isRiduzioneRetributiva);

            return crossProperties;
        }

        private static bool? GestioneRiduzioneRetributiva(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda)
        {
            if (datiPensione == null)
                return false;

            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || isRiaperturaDomanda)
            {
                Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(datiPensione);
                if (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 02, 01)) &&
                    tipoCalcolo == Utility.TipoCalcolo.Retributivo || tipoCalcolo == Utility.TipoCalcolo.Misto)
                {
                    if (string.IsNullOrEmpty(datiPensione.SiglaCategoria) || !datiPensione.SiglaCategoria.Trim().Equals("VO"))
                        return false;
                    if (string.IsNullOrEmpty(datiPensione.NaturaPensione) || (!datiPensione.NaturaPensione.Substring(0, 1).Equals("1") && !datiPensione.NaturaPensione.Substring(0, 1).Equals("2")))
                        return false;
                }
                else
                    return false;
            }
            else
            {
                //riduzione retributiva non è visibile per pensioni diverse da anzianità (gruppo 0001 e prodotto 0001)
                if (string.IsNullOrEmpty(datiPensione.Gruppo) || datiPensione.Gruppo != "0001")
                    return false;
                if (string.IsNullOrEmpty(datiPensione.Prodotto) || datiPensione.Prodotto != "0001")
                    return false;
                if (!datiPensione.DataPerfezionamentoRequisiti.HasValue || (datiPensione.DataPerfezionamentoRequisiti.HasValue && DateTime.Compare(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2011, 12, 31).Date) <= 0))
                    return false;
            }
            return true;
        }

        public static bool? GestioneContributivoL214ForTipoFondo(GestionePensione.DatiPensione datiPensione, GestioneFondo.DatiFondo datiFondoCommon)
        {
            if (datiPensione == null)
                return false;

            char? codiceSpecifico = null;
            if (datiFondoCommon != null && datiFondoCommon.CodiceSpecifico.HasValue)
            {
                List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                if (elencoCodiceSpecifico != null && elencoCodiceSpecifico.Count > 0)
                {
                    GestioneDecodifica.CodiceSpecifico codice = elencoCodiceSpecifico.Find(x => x.Id == datiFondoCommon.CodiceSpecifico);
                    if (codice != null)
                        codiceSpecifico = codice.TraduzioneGp;
                }
            }

            return GestioneContributivoL214(datiPensione, codiceSpecifico);

        }

        private static bool? GestioneContributivoL214(GestionePensione.DatiPensione datiPensione, char? codiceSpecifico)
        {
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcolo(datiPensione);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if (tipoCalcolo == Utility.TipoCalcolo.Contributivo || tipoCalcolo == Utility.TipoCalcolo.Misto)
                if (datiPensione.FineAssicurazione.HasValue && DateTime.Compare(datiPensione.FineAssicurazione.Value, new DateTime(2011, 12, 31).Date) > 0)
                    return true;

            if (tipoCalcolo == Utility.TipoCalcolo.RetributivoMonti)
                return true;

            // Per il fondo PM non è valida la condizione di pensione di inabilità post 2011
            if (tipoFondo == Utility.TipoFondo.PM)
                return false;

            if (Utility.IsDomandaPensioneInabilitaOrRicostituzioneFS(datiPensione, codiceSpecifico) && datiPensione.FineAssicurazione.HasValue &&
                !Utility.DataSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(2012, 01, 01)) && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2012, 02, 01)))
                return true;

            return false;
        }

        public static void AddRecordFondo(GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, ref GestioneFondo.DatiFondo datiFondoCommon, ref object fondoXX,
            ref List<GestioneDatiServizioUtile.ServizioUtile> lastLstServizioUtile, out GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, out long idRecordFondo)
        {
            idRecordFondo = -1;
            long idPensione = datiPensione.Id;
            long idFondo = datiFondoCommon.Id;

            List<GestioneRecordFondo.DatiRecordFondo> lstOldRecordFondo;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstOldRecordFondo);
            GestioneRecordFondo.DatiRecordFondo lastRecordFondo = lstOldRecordFondo.OrderByDescending(x => x.Id).FirstOrDefault();

            GestioneFondo.DatiFondoPT lastDatiFondoPT = null;
            GestioneFondo.DatiFondoFST lastDatiFondoFS = null;
            GestioneFondo.DatiFondoDZ lastDatiFondoDZ = null;

            fondoXX = GetObjectDatiFondoByIdRecordFondo(lastRecordFondo.Id, tipoFondo);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            GestioneCalcolo.DatiCalcoloContributivo daticontributiviDB = new GestioneCalcolo.DatiCalcoloContributivo();
            GestioneCalcolo.DatiCalcoloContributivo datiContributiviLastRecordFondo = new GestioneCalcolo.DatiCalcoloContributivo();
            GestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(lastRecordFondo.Id, out daticontributiviDB);

            Utility.ValorizzaOggetti(daticontributiviDB, datiContributiviLastRecordFondo);

            GestioneDatiServizioUtile.GetDatiServizioUtileByIdRecordFondo(lastRecordFondo.Id, out lastLstServizioUtile);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();

                //replico PensioneFondo uguale all'ultimo record a db
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoPT();
                        lastDatiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        //ENG - PL Reversibilita 024
                        lastDatiFondoPT.IsPensioneAnnuaLordaDaPrelievo = null;
                        lastDatiFondoPT.IsPensioneAnnuaLorda707DaPrelievo = null;
                        //dati fondo
                        Utility.ValorizzaOggetti(new DatiFondo(), fondoXX);
                        //dati legge 460
                        Utility.ValorizzaOggetti(new DatiLegge460(), fondoXX);
                        lastDatiFondoPT.Ncertificato = null;
                        //datiArticolo2
                        Utility.ValorizzaOggetti(new DatiArticolo2ForDatiFondo(), fondoXX);
                        GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);
                        idRecordFondo = recordFondo.Id;
                        GestioneFondo.SalvaFondoPTRecordFondo(idFondo, idRecordFondo, lastDatiFondoPT);
                        break;
                    case Utility.TipoFondo.FS:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoFST();
                        lastDatiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                        //ENG - PL Reversibilita 024
                        lastDatiFondoFS.IsPensioneAnnuaLordaDaPrelievo = null;
                        lastDatiFondoFS.IsPensioneAnnuaLorda707DaPrelievo = null;
                        //dati fondo
                        Utility.ValorizzaOggetti(new DatiFondo(), fondoXX);
                        //datiArticolo2
                        Utility.ValorizzaOggetti(new DatiArticolo2ForDatiFondo(), fondoXX);
                        GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);
                        idRecordFondo = recordFondo.Id;
                        GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, idRecordFondo, lastDatiFondoFS);
                        break;
                    case Utility.TipoFondo.DZ:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoDZ();
                        lastDatiFondoDZ = (GestioneFondo.DatiFondoDZ)fondoXX;
                        lastDatiFondoDZ.PensioneBaseAnnua = null;
                        recordFondo.CodiceNatura1 = '0';
                        recordFondo.CodiceNatura2 = ' ';
                        recordFondo.CodiceNatura3 = ' ';
                        recordFondo.CodiceNonCalcolo = 'N';
                        GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);
                        idRecordFondo = recordFondo.Id;
                        GestioneFondo.SalvaFondoDZRecordFondo(idFondo, idRecordFondo, lastDatiFondoDZ);
                        break;
                }

                if (!datiContributiviLastRecordFondo.IsDatiCalcoloContributivoNull())
                {
                    datiContributiviLastRecordFondo.IdRecordFondo = idRecordFondo;
                    datiContributiviLastRecordFondo.IdPensione = idPensione;
                    GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(datiContributiviLastRecordFondo);
                }

                //replico servizio utile uguale all'ultimo record salvato a db
                if (lastLstServizioUtile != null && lastLstServizioUtile.Count > 0)
                {
                    foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in lastLstServizioUtile)
                        GestioneDatiServizioUtile.SalvaDatiServizioUtileRecordFondo(servizioUtile.IdFondo.Value, idRecordFondo, servizioUtile);
                }
                //Verifico se il tab Privilegiate viene precompilato -> metto semaforo a rosso
                DatiPrivilegiate datiPrivilegiate = new DatiPrivilegiate();
                Utility.ValorizzaOggetti(fondoXX, datiPrivilegiate);
                List<GestioneRecordFondo.DatiRecordFondo> lstDatiRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo> { recordFondo };

                if (!datiPrivilegiate.IsDatiPrivilegiateNull())
                    datiQuadroDatiRecordFondo = GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, lstDatiRecordFondo, Utility.TipoAppartenenza.FS, tipoDomanda, true, null, true, null, null, false, false).FirstOrDefault();
                else
                    datiQuadroDatiRecordFondo = GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, lstDatiRecordFondo, Utility.TipoAppartenenza.FS, tipoDomanda, true, null, null, null, null, false, false).FirstOrDefault();
                //set a rosso semaforo quadro
                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);
                transactionScope.Complete();
            }
        }

        public static void AddRecordFondoINPDAP(GestionePensione.DatiPensione datiPensione, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            ref List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lastLstServizioUtile, out GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, out long idRecordFondo)
        {
            idRecordFondo = -1;
            long idPensione = datiPensione.Id;

            List<GestioneRecordFondo.DatiRecordFondo> lstOldRecordFondo;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstOldRecordFondo);
            GestioneRecordFondo.DatiRecordFondo lastRecordFondo = lstOldRecordFondo.OrderByDescending(x => x.Id).FirstOrDefault();
            GestioneCalcolo.DatiCalcoloContributivo daticontributiviDB = new GestioneCalcolo.DatiCalcoloContributivo();
            GestioneCalcolo.DatiCalcoloContributivo datiContributiviLastRecordFondo = new GestioneCalcolo.DatiCalcoloContributivo();
            if (lastRecordFondo != null)
            {
                GestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdRecordFondo(lastRecordFondo.Id, out lastLstServizioUtile);
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(lastRecordFondo.Id, out recordDatiFondoINPDAP);
                GestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(lastRecordFondo.Id, out daticontributiviDB);
            }

            Utility.ValorizzaOggetti(daticontributiviDB, datiContributiviLastRecordFondo);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordFondo.DatiRecordFondo recordFondo = new GestioneRecordFondo.DatiRecordFondo();
                GestioneRecordFondo.SalvaSingoloRecordFondo(idPensione, recordFondo);
                idRecordFondo = recordFondo.Id;
                //replico PensioneFondo uguale all'ultimo record a db
                //dati fondo
                Utility.ValorizzaOggetti(new DatiFondo(), recordDatiFondoINPDAP);
                //datiArticolo2
                Utility.ValorizzaOggetti(new DatiArticolo2ForDatiFondo(), recordDatiFondoINPDAP);
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(idPensione, idRecordFondo, recordDatiFondoINPDAP);

                if (!datiContributiviLastRecordFondo.IsDatiCalcoloContributivoNull())
                {
                    datiContributiviLastRecordFondo.IdRecordFondo = idRecordFondo;
                    datiContributiviLastRecordFondo.IdPensione = idPensione;
                    GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(datiContributiviLastRecordFondo);
                }

                //replico servizio utile uguale all'ultimo record salvato a db
                if (lastLstServizioUtile != null && lastLstServizioUtile.Count > 0)
                {
                    foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile servizioUtile in lastLstServizioUtile)
                        GestioneDatiServizioUtileINPDAP.SalvaDatiServizioUtileRecordFondo(servizioUtile.IdPensione.Value, idRecordFondo, servizioUtile);
                }
                //Verifico se il tab Privilegiate viene precompilato -> metto semaforo a rosso
                DatiPrivilegiate datiPrivilegiate = new DatiPrivilegiate();
                Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiPrivilegiate);
                List<GestioneRecordFondo.DatiRecordFondo> lstDatiRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo> { recordFondo };
                if (!datiPrivilegiate.IsDatiPrivilegiateNull())
                    datiQuadroDatiRecordFondo = GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, lstDatiRecordFondo, Utility.TipoAppartenenza.FS, tipoDomanda, true, null, true, null, null, false, false).FirstOrDefault();
                else
                    datiQuadroDatiRecordFondo = GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, lstDatiRecordFondo, Utility.TipoAppartenenza.FS, tipoDomanda, true, null, null, null, null, false, false).FirstOrDefault();

                //set a rosso semaforo quadro
                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiRecordFondoByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo;
            GestioneQuadri.GetQuadroDatiFondoByDatiPensione(datiPensione, out datiQuadroDatiFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo;
            datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //elimina DatiFondo
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        GestioneFondo.EliminaFondoPTByIdRecordFondo(idRecordFondo);
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.EliminaFondoFSTByIdRecordFondo(idRecordFondo);
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.EliminaFondoDZByIdRecordFondo(idRecordFondo);
                        break;
                }
                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);
                GestioneCalcolo.EliminaCalcoloContributivoByIdRecordFondo(idRecordFondo, false);
                GestioneQuadri.EliminaQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo);
                GestioneCalcolo.EliminaCalcoloRetributivoByIdRecordFondo(idRecordFondo);

                GestioneRecordFondo.EliminaRecordFondo(idRecordFondo);
                //semafori
                byte? newSemValue = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo.Where(x => x.IdRecordFondo != idRecordFondo).ToList());
                if (datiQuadroDatiFondo.TabRegistrazioniFondo != newSemValue)
                {
                    datiQuadroDatiFondo.TabRegistrazioniFondo = newSemValue;
                    if (newSemValue == 1)
                        datiQuadroDatiFondo.Tipo = 1;
                    GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);
                }
                transactionScope.Complete();
            }
            lstDatiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.FindAll(x => x.IdRecordFondo != idRecordFondo);
        }

        public static void EliminaDatiRecordFondoINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo;
            GestioneQuadri.GetQuadroDatiFondoByDatiPensione(datiPensione, out datiQuadroDatiFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo;
            datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //elimina DatiFondo
                GestioneRecordDatiFondoINPDAP.EliminaRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo);
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                GestioneCalcolo.EliminaDatiServizioUtileINPDAP707ByIdRecordFondo(idRecordFondo);
                GestioneCalcolo.EliminaCalcoloContributivoByIdRecordFondo(idRecordFondo, false);
                GestioneQuadri.EliminaQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo);
                GestioneRecordFondo.EliminaRecordFondo(idRecordFondo);
                //semafori
                byte? newSemValue = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo.Where(x => x.IdRecordFondo != idRecordFondo).ToList());
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
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            GestioneFondo.DatiFondoFST datiFondoFS = null;
            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;

            List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstRecordFondo);
            lstRecordFondo.Sort((x, y) => x.Id.CompareTo(y.Id));
            GestioneRecordFondo.DatiRecordFondo primoRecord = lstRecordFondo.FirstOrDefault();
            switch (tipoFondo)
            {
                case Utility.TipoFondo.PT:
                    GestioneFondo.GetFondoPTByIdRecordFondo(primoRecord.Id, out datiFondoPT);
                    break;
                case Utility.TipoFondo.FS:
                    GestioneFondo.GetFondoFSTByIdRecordFondo(primoRecord.Id, out datiFondoFS);
                    break;
                case Utility.TipoFondo.DZ:
                    GestioneFondo.GetFondoDZByIdRecordFondo(primoRecord.Id, out datiFondoDZ);
                    break;
            }

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //pulisco il primo record
                if (datiFondoPT != null || datiFondoFS != null || datiFondoDZ != null)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            if (datiFondoPT != null)
                            {
                                decimal? incrementoContrattuale = datiFondoPT.IncrementoContrattuale;
                                Utility.ValorizzaOggetti(new DatiFondo(), datiFondoPT);
                                Utility.ValorizzaOggetti(new DatiCalcolo(), datiFondoPT);
                                Utility.ValorizzaOggetti(new DatiArticolo2ForDatiFondo(), datiFondoPT);
                                Utility.ValorizzaOggetti(new DatiLegge460(), datiFondoPT);
                                Utility.ValorizzaOggetti(new DatiPrivilegiate(), datiFondoPT);
                                datiFondoPT.IncrementoContrattuale = incrementoContrattuale;
                                datiFondoPT.Ncertificato = null;
                                GestioneFondo.SalvaFondoPTRecordFondo(datiFondoPT.IdFondo, datiFondoPT.IdRecordFondo.Value, datiFondoPT);
                            }
                            break;
                        case Utility.TipoFondo.FS:
                            if (datiFondoFS != null)
                            {
                                Utility.ValorizzaOggetti(new DatiFondo(), datiFondoFS);
                                Utility.ValorizzaOggetti(new DatiCalcolo(), datiFondoFS);
                                Utility.ValorizzaOggetti(new DatiArticolo2ForDatiFondo(), datiFondoFS);
                                Utility.ValorizzaOggetti(new DatiLegge460(), datiFondoFS);
                                Utility.ValorizzaOggetti(new DatiPrivilegiate(), datiFondoFS);
                                GestioneFondo.SalvaFondoFSTRecordFondo(datiFondoFS.IdFondo, datiFondoFS.IdRecordFondo.Value, datiFondoFS);
                            }
                            break;
                        case Utility.TipoFondo.DZ:
                            if (datiFondoDZ != null)
                            {
                                Utility.ValorizzaOggetti(new DatiFondo(), datiFondoDZ);
                                GestioneFondo.SalvaFondoDZRecordFondo(datiFondoDZ.IdFondo, datiFondoDZ.IdRecordFondo.Value, datiFondoDZ);
                            }
                            break;
                    }
                    GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, new List<GestioneRecordFondo.DatiRecordFondo> { primoRecord }, Utility.TipoAppartenenza.FS, tipoDomanda, true, null, null, null, null, false, false);
                }
                //elimino dati servizio utile
                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                GestioneCalcolo.EliminaDatiServizioUtile707ByIdPensione(datiPensione.Id);
                GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
                GestioneCalcolo.EliminaCalcoloRetributivoByIdPensione(datiPensione.Id, false);
                //elimino tutti dati pensioneFondoSpecifico tranne il primo record
                foreach (var elem in lstRecordFondo.GetRange(1, lstRecordFondo.Count - 1))
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            GestioneFondo.EliminaFondoPTByIdRecordFondo(elem.Id);
                            break;
                        case Utility.TipoFondo.FS:
                            GestioneFondo.EliminaFondoFSTByIdRecordFondo(elem.Id);
                            break;
                        case Utility.TipoFondo.DZ:
                            GestioneFondo.EliminaFondoDZByIdRecordFondo(elem.Id);
                            break;
                    }
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

        public static void EliminaDatiRecordFondoINPDAPByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP = null;
            List<GestioneRecordFondo.DatiRecordFondo> lstRecordFondo;
            GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out lstRecordFondo);
            lstRecordFondo.Sort((x, y) => x.Id.CompareTo(y.Id));
            GestioneRecordFondo.DatiRecordFondo primoRecord = lstRecordFondo.FirstOrDefault();
            GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(primoRecord.Id, out recordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.RequiresNew,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //pulisco il primo record e mantengo invariati i dati bloccati
                if (recordDatiFondoINPDAP != null)
                {
                    DatiFondo entityDatiFondo = new DatiFondo();
                    entityDatiFondo.TrediciMensilita = recordDatiFondoINPDAP.TrediciMensilita;
                    entityDatiFondo.IndennitaIntegrativaSpecialeConglobata = recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata;
                    Utility.ValorizzaOggetti(entityDatiFondo, recordDatiFondoINPDAP);

                    Utility.ValorizzaOggetti(new DatiCalcolo(), recordDatiFondoINPDAP);

                    Utility.ValorizzaOggetti(new DatiArticolo2ForDatiFondo(), recordDatiFondoINPDAP);

                    Utility.ValorizzaOggetti(new DatiPrivilegiate(), recordDatiFondoINPDAP);

                    Utility.ValorizzaOggetti(new DatiLegge460(), recordDatiFondoINPDAP);

                    GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, recordDatiFondoINPDAP.IdRecordFondo, recordDatiFondoINPDAP);

                    GestioneQuadri.InizializzaQuadroDatiRecordFondo(datiPensione, new List<GestioneRecordFondo.DatiRecordFondo> { primoRecord }, Utility.TipoAppartenenza.FS, tipoDomanda, true, null, null, null, null, false, false);
                }

                //elimino dati servizio utile
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdPensione(datiPensione.Id);
                GestioneCalcolo.EliminaDatiServizioUtileINPDAP707ByIdPensione(datiPensione.Id);
                GestioneCalcolo.EliminaCalcoloContributivoByIdPensione(datiPensione.Id, false);
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
        public static void GetDatiFondoByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX, ref GestioneRecordFondo.DatiRecordFondo datiRecordFondo, ref GestioneFondo.DatiFondo datiFondoGenerici, out DatiFondo datiFondo)
        {
            datiFondo = new DatiFondo();
            if (datiRecordFondo == null)
                GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out datiRecordFondo);

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (datiFondoGenerici == null)
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoGenerici);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.FS:
                case Utility.TipoFondo.PT:
                    Utility.ValorizzaOggetti(fondoXX, datiFondo);
                    break;
            }
            datiFondo.DecorrenzaValidita = datiRecordFondo.DecorrenzaValiditaDati;
            datiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiFondo;
            datiFondo.TipoPensione = Utility.GetTipoPensione(datiPensione).Keys.FirstOrDefault();
            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione &&
                !(Utility.IsDomandaReversibilita(datiPensione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT))) //ENG - Reversibilita 024
                datiFondo.DecorrenzaCalcolo = GetDecorrenzaCalcolo(datiPensione, datiFondoGenerici != null ? datiFondoGenerici.InizioBonus : null);
        }

        public static void GetDatiFondoINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
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
            datiFondo.NumeroRate = recordDatiFondoINPDAP.NumeroRate;
            datiFondo.ImportoSingolaRata = recordDatiFondoINPDAP.ImportoSingolaRata;
            if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !Utility.IsDomandaReversibilita(datiPensione))
                datiFondo.DecorrenzaCalcolo = GetDecorrenzaCalcolo(datiPensione, datiControlloFelpe != null ? datiControlloFelpe.InizioBonus : null);
        }

        public static void StoreDatiFondoByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondoCommon, ref object fondoXX, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, DatiFondo entityDatiFondo)
        {
            if (entityDatiFondo == null)
                entityDatiFondo = new DatiFondo();

            if (datiFondoCommon == null)
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoCommon);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            GestioneRecordFondo.DatiRecordFondo recordFondo;
            GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out recordFondo);

            #region Gestione TipoFondo

            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoFST datiFondoFS = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.DZ:
                        datiFondoDZ = (GestioneFondo.DatiFondoDZ)fondoXX;
                        Utility.ValorizzaOggetti(entityDatiFondo, datiFondoDZ);
                        break;
                    case Utility.TipoFondo.PT:
                        datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        Utility.ValorizzaOggetti(entityDatiFondo, datiFondoPT);
                        break;
                    case Utility.TipoFondo.FS:
                        datiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                        Utility.ValorizzaOggetti(entityDatiFondo, datiFondoFS);
                        break;
                }
            }
            #endregion Gestione TipoFondo

            #region Gestione Semaforica


            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);


            #endregion Gestione Semaforica

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.SalvaFondoDZRecordFondo(datiFondoCommon.Id, idRecordFondo, datiFondoDZ);
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.SalvaFondoPTRecordFondo(datiFondoCommon.Id, idRecordFondo, datiFondoPT);
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.SalvaFondoFSTRecordFondo(datiFondoCommon.Id, idRecordFondo, datiFondoFS);
                        break;
                }
                if (!entityDatiFondo.IsNull())
                    datiQuadroRecordFondo.TabDatiFondo = 2;
                else
                    datiQuadroRecordFondo.TabDatiFondo = 0;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroRecordFondo);

                recordFondo.DecorrenzaValiditaDati = entityDatiFondo.DecorrenzaValidita;
                GestioneRecordFondo.SalvaSingoloRecordFondo(datiPensione.Id, recordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            entityDatiFondo.Semaforo = datiQuadroRecordFondo.TabDatiFondo;
        }

        public static void StoreDatiFondoINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, DatiFondo entityDatiFondo, bool isCancelOperation)
        {
            if (entityDatiFondo == null)
                entityDatiFondo = new DatiFondo();

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            GestioneRecordFondo.DatiRecordFondo recordFondo;
            GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out recordFondo);

            // Mantengo invariati i campi bloccati
            if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
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
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            entityDatiFondo.Semaforo = datiQuadroRecordFondo.TabDatiFondo;
        }

        public static bool ControlsDatiFondo(GestionePensione.DatiPensione datiPensione, bool isRiaperturaDomanda, Utility.TipoFondo? tipoFondo, long idRecordFondo, DatiFondo entityDatiFondo,
            DatiArticolo2ForDatiFondo entityDatiArticolo2, object fondoXX, ref List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (entityDatiFondo == null)
                entityDatiFondo = new DatiFondo();

            if (listaRecordFondo == null)
                GestioneRecordFondo.GetRecordFondoByIdPensione(datiPensione.Id, out listaRecordFondo);

            bool isPrimoRecord = listaRecordFondo.First().Id == idRecordFondo;

            List<GestioneRecordFondo.DatiRecordFondo> app = listaRecordFondo.FindAll(x => x.Id < idRecordFondo && x.DecorrenzaValiditaDati.HasValue);
            DateTime? ultimaDecorrenzaValidita = null;
            if (app != null && app.Count > 0)
                ultimaDecorrenzaValidita = app.LastOrDefault().DecorrenzaValiditaDati;
            if (!ultimaDecorrenzaValidita.HasValue && !isPrimoRecord)
                ultimaDecorrenzaValidita = datiPensione.DecorrenzaOriginaria;

            //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            if (isSingleTab)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.DatiFondoDZ datiFondoDZ = (GestioneFondo.DatiFondoDZ)fondoXX;
                        if (datiFondoDZ != null)
                        {
                            Utility.ValorizzaOggetti(datiFondoDZ, entityDatiArticolo2);
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        if (datiFondoPT != null)
                        {
                            Utility.ValorizzaOggetti(datiFondoPT, entityDatiArticolo2);
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.DatiFondoFST datiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                        if (datiFondoFS != null)
                        {
                            Utility.ValorizzaOggetti(datiFondoFS, entityDatiArticolo2);
                        }
                        break;
                }
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
            if (!GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.DECOR_REGISTR_CALCOLO_FSPT))
            {
                GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                    GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
                if (!((Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) || (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.SiglaCategoria.StartsWith("S"))) &&
                    (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.DZ)))
                {
                    if (!Utility.IsRicostituzioneOrRiaperturaFSPTPerequata(datiPensione, isRiaperturaDomanda, datiPensione.DecorrenzaOriginaria) &&
                        entityDatiFondo.DecorrenzaCalcolo.HasValue && entityDatiFondo.DecorrenzaValidita < entityDatiFondo.DecorrenzaCalcolo)
                    {
                        messaggioVideo = "La decorrenza registrazione deve essere maggiore della decorrenza calcolo";
                        return false;
                    }
                }
            }

            if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(entityDatiArticolo2 != null ? entityDatiArticolo2.ScadenzaBenefici : null, entityDatiFondo.DecorrenzaValidita,
                out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsDatiFondoINPDAP(GestionePensione.DatiPensione datiPensione, long idRecordFondo, DatiFondo entityDatiFondo,
            DatiArticolo2ForDatiFondo entityDatiArticolo2, ref List<GestioneRecordFondo.DatiRecordFondo> listaRecordFondo, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, bool isSingleTab, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (entityDatiFondo == null)
                entityDatiFondo = new DatiFondo();

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

            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            if (Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);
            if (!Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))
            {
                if (entityDatiFondo.DecorrenzaCalcolo.HasValue && entityDatiFondo.DecorrenzaValidita < entityDatiFondo.DecorrenzaCalcolo)
                {
                    messaggioVideo = "La decorrenza registrazione deve essere maggiore della decorrenza calcolo";
                    return false;
                }
            }

            if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(entityDatiArticolo2 != null ? entityDatiArticolo2.ScadenzaBenefici : null, entityDatiFondo.DecorrenzaValidita,
                out messaggioVideo))
                return false;
            if ((entityDatiFondo.ImportoSingolaRata != null && entityDatiFondo.NumeroRate == null) || (entityDatiFondo.ImportoSingolaRata == null && entityDatiFondo.NumeroRate != null))
            {
                messaggioVideo = "I campi Importo singola rata e Numero rate devono essere entrambi valorizzati";
                return false;
            }

            return true;
        }

        #endregion Dati Fondo

        #region Dati Calcolo

        public static void GetDatiCalcoloByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX, ref List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon,
            out DatiCalcolo datiCalcoloForDatiFondo, out csAggiornamentoPECO_Fondi_AMG dati)
        {
            datiCalcoloForDatiFondo = new DatiCalcolo();
            dati = null;
            Utility.StatoPensione? statoPensione = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            //servizio utile
            if (lServizioUtileCommon == null)
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdRecordFondo(idRecordFondo, out lServizioUtileCommon);

            if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
            {
                datiCalcoloForDatiFondo.lDatiServizioUtile = lServizioUtileCommon;
            }

            GestioneCalcolo.DatiCalcoloContributivo datiContributiviDB = null;
            GestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(idRecordFondo, out datiContributiviDB);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.FS:
                    if (fondoXX != null)
                    {
                        GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)fondoXX;
                        datiCalcoloForDatiFondo.PensioneAnnuaLorda214 = datiFondoFST.PensioneAnnuaLorda214;
                        datiCalcoloForDatiFondo.PensioneAnnuaLorda = datiFondoFST.PensioneAnnuaLorda;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoAA = datiFondoFST.ServizioUtileDirittoAA;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoMM = datiFondoFST.ServizioUtileDirittoMM;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoGG = datiFondoFST.ServizioUtileDirittoGG;
                        datiCalcoloForDatiFondo.RMSSenzaLegge33670QA = datiFondoFST.RMSSenzaLegge33670QA;
                        datiCalcoloForDatiFondo.CoefficienteTrasformazione = datiFondoFST.CoefficienteTrasformazione;
                        datiCalcoloForDatiFondo.IsPensioneAnnuaLordaDaPrelievo = datiFondoFST.IsPensioneAnnuaLordaDaPrelievo;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoOIAA = datiFondoFST.ServizioUtileDirittoOIAA;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoOIMM = datiFondoFST.ServizioUtileDirittoOIMM;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoOIGG = datiFondoFST.ServizioUtileDirittoOIGG;

                        if (datiContributiviDB != null)
                        {
                            datiCalcoloForDatiFondo.ImportoContributivoTotale = datiContributiviDB.ImportoContributivoTotale;
                            datiCalcoloForDatiFondo.Montante = datiContributiviDB.Montante;
                            datiCalcoloForDatiFondo.MontanteContributivo = datiContributiviDB.MontanteContributivo;
                            datiCalcoloForDatiFondo.NSettimane = datiContributiviDB.NSettimane;
                            datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = datiContributiviDB.ImportoContribTotaleQuotaDL214;
                            datiCalcoloForDatiFondo.MontanteQuotaDL214 = datiContributiviDB.MontanteQuotaDL214;
                            datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = datiContributiviDB.NSettimaneQuotaDL214;
                            datiCalcoloForDatiFondo.QuotaContributivaAnnua = datiContributiviDB.QuotaContributivaAnnua;
                        }
                    }
                    break;
                case Utility.TipoFondo.PT:
                    if (fondoXX != null)
                    {
                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        datiCalcoloForDatiFondo.PensioneAnnuaLorda214 = datiFondoPT.PensioneAnnuaLorda214;
                        datiCalcoloForDatiFondo.PensioneAnnuaLorda = datiFondoPT.PensioneAnnuaLorda;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoAA = datiFondoPT.ServizioUtileDirittoAA;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoMM = datiFondoPT.ServizioUtileDirittoMM;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoGG = datiFondoPT.ServizioUtileDirittoGG;
                        datiCalcoloForDatiFondo.RMSSenzaLegge33670QA = datiFondoPT.RMSSenzaLegge33670QA;
                        datiCalcoloForDatiFondo.CoefficienteTrasformazione = datiFondoPT.CoefficienteTrasformazione;
                        datiCalcoloForDatiFondo.IsPensioneAnnuaLordaDaPrelievo = datiFondoPT.IsPensioneAnnuaLordaDaPrelievo;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoOIAA = datiFondoPT.ServizioUtileDirittoOIAA;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoOIMM = datiFondoPT.ServizioUtileDirittoOIMM;
                        datiCalcoloForDatiFondo.ServizioUtileDirittoOIGG = datiFondoPT.ServizioUtileDirittoOIGG;
                        if (datiContributiviDB != null)
                        {
                            datiCalcoloForDatiFondo.ImportoContributivoTotale = datiContributiviDB.ImportoContributivoTotale;
                            datiCalcoloForDatiFondo.Montante = datiContributiviDB.Montante;
                            datiCalcoloForDatiFondo.MontanteContributivo = datiContributiviDB.MontanteContributivo;
                            datiCalcoloForDatiFondo.NSettimane = datiContributiviDB.NSettimane;
                            datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = datiContributiviDB.ImportoContribTotaleQuotaDL214;
                            datiCalcoloForDatiFondo.MontanteQuotaDL214 = datiContributiviDB.MontanteQuotaDL214;
                            datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = datiContributiviDB.NSettimaneQuotaDL214;
                            datiCalcoloForDatiFondo.QuotaContributivaAnnua = datiContributiviDB.QuotaContributivaAnnua;
                        }
                    }
                    break;
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (lServizioUtileCommon == null || lServizioUtileCommon.Count == 0) && datiContributiviDB == null &&
                !(statoPensione.HasValue && (statoPensione == Utility.StatoPensione.Calcolata || statoPensione == Utility.StatoPensione.CalcolataNoWebDom || statoPensione == Utility.StatoPensione.CalcolataNoFelpe || statoPensione == Utility.StatoPensione.CalcolataNoOneri)))
            {
                dati = null;
                string errori = string.Empty;

                try
                {
                    GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        throw new DNA.DnaValidationException(errori);

                    GestioneAggiornamentoPECO.DatiContributivi datiContributivi = null;
                    GestioneContrib.CrossDataRecipient crossDataRecipient = null;
                    GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out lServizioUtileCommon, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);

                    if (datiCalcoloForDatiFondo == null)
                        datiCalcoloForDatiFondo = new DatiCalcolo();

                    if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        datiCalcoloForDatiFondo.lDatiServizioUtile = lServizioUtileCommon;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }

            datiCalcoloForDatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;
            GestioneContrib.TipoCalcolo tipoCalcolo;
            GestioneContrib.GetTipoCalcoloByDatiPensione(datiPensione, out tipoCalcolo);
            datiCalcoloForDatiFondo.TipoCalcolo = tipoCalcolo;
        }

        public static void GetDatiCalcoloDZByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, GestioneFondo.DatiFondo datiFondoGenerici,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX, ref List<GestioneDatiServizioUtile.ServizioUtile> lServizioUtileCommon,
            out GestioneContrib.DatiCalcolo datiCalcoloForDatiFondo, out csAggiornamentoPECO_Fondi_AMG dati)
        {
            datiCalcoloForDatiFondo = new GestioneContrib.DatiCalcolo();
            dati = null;
            Utility.StatoPensione? statoPensione = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            //servizio utile
            if (lServizioUtileCommon == null)
                GestioneDatiServizioUtile.GetDatiServizioUtileByIdRecordFondo(idRecordFondo, out lServizioUtileCommon);

            GestioneCalcolo.DatiCalcoloRetributivo datiRetributiviDB = null;
            GestioneCalcolo.GetCalcoloRetributivoByIdRecordFondo(idRecordFondo, out datiRetributiviDB);

            GestioneCalcolo.DatiCalcoloContributivo datiContributiDB = null;
            GestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(idRecordFondo, out datiContributiDB);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.DZ:
                    if (fondoXX != null)
                    {
                        GestioneFondo.DatiFondoDZ datiFondoDZ = (GestioneFondo.DatiFondoDZ)fondoXX;
                        if (datiCalcoloForDatiFondo.fondoDZ == null)
                            datiCalcoloForDatiFondo.fondoDZ = new GestioneContrib.FondoDZ();
                        datiCalcoloForDatiFondo.fondoDZ.PensioneBaseAnnua = datiFondoDZ.PensioneBaseAnnua;
                        datiCalcoloForDatiFondo.fondoDZ.DecorrenzaValidita = datiFondoDZ.DecorrenzaValidita;
                        datiCalcoloForDatiFondo.fondoDZ.Sospensione = datiFondoDZ.Sospensione;


                        if (datiRetributiviDB != null)
                        {
                            datiCalcoloForDatiFondo.RMSQuotaA = datiRetributiviDB.RMSQuotaA.Value;
                            datiCalcoloForDatiFondo.NSettimaneQuotaA = datiRetributiviDB.NSettimaneQuotaA;
                            datiCalcoloForDatiFondo.RMSQuotaB = datiRetributiviDB.RMSQuotaB.Value;
                            datiCalcoloForDatiFondo.NSettimaneQuotaB = datiRetributiviDB.NSettimaneQuotaB;
                        }

                        datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = (datiContributiDB != null ? datiContributiDB.ImportoContribTotaleQuotaDL214 : 0);
                        datiCalcoloForDatiFondo.MontanteQuotaDL214 = (datiContributiDB != null ? datiContributiDB.MontanteQuotaDL214 : 0);
                        datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = (datiContributiDB != null ? datiContributiDB.NSettimaneQuotaDL214 : 0);

                        List<GestioneContrib.DatiServizioUtile> lServizioUtile = new List<GestioneContrib.DatiServizioUtile>();
                        foreach (GestioneDatiServizioUtile.ServizioUtile servizioUtile in lServizioUtileCommon)
                        {
                            GestioneContrib.DatiServizioUtile datiServizioUtile = new GestioneContrib.DatiServizioUtile();
                            Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                            lServizioUtile.Add(datiServizioUtile);
                        }

                        datiCalcoloForDatiFondo.fondoDZ.lDatiServizioUtile = lServizioUtile;

                        if (datiFondoGenerici != null)
                        {
                            datiCalcoloForDatiFondo.QuotaA707 = datiFondoGenerici.QuotaA707;
                            datiCalcoloForDatiFondo.QuotaB707 = datiFondoGenerici.QuotaB707;
                            datiCalcoloForDatiFondo.RetribuzionePonderataAGO707 = datiFondoGenerici.RetribuzionePonderataAGO707;
                        }

                    }
                    break;
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (lServizioUtileCommon == null || lServizioUtileCommon.Count == 0) && datiRetributiviDB == null &&
                !(statoPensione.HasValue && (statoPensione == Utility.StatoPensione.Calcolata || statoPensione == Utility.StatoPensione.CalcolataNoWebDom || statoPensione == Utility.StatoPensione.CalcolataNoFelpe || statoPensione == Utility.StatoPensione.CalcolataNoOneri)))
            {
                dati = null;
                string errori = string.Empty;

                try
                {
                    GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        throw new DNA.DnaValidationException(errori);

                    GestioneAggiornamentoPECO.DatiContributivi datiContributivi = null;
                    GestioneContrib.CrossDataRecipient crossDataRecipient = null;
                    //GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out lServizioUtileCommon, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);

                    //if (datiCalcoloForDatiFondo == null)
                    //    datiCalcoloForDatiFondo = new DatiCalcolo();

                    //if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                    //    datiCalcoloForDatiFondo.lDatiServizioUtile = lServizioUtileCommon;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }

            datiCalcoloForDatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcoloDZ;
            GestioneContrib.TipoCalcolo tipoCalcolo;
            GestioneContrib.GetTipoCalcoloByDatiPensione(datiPensione, out tipoCalcolo);
            datiCalcoloForDatiFondo.TipoCalcolo = tipoCalcolo;
        }

        public static void GetDatiCalcoloINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref List<GestioneDatiServizioUtileINPDAP.ServizioUtile> lServizioUtileCommon, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            out DatiCalcolo datiCalcoloForDatiFondo, out csAggiornamentoPECO_Fondi_AMG_INPDAP datiINPDAP, out csAggiornamentoPECO_Fondi_AMG dati)
        {
            dati = null;
            datiINPDAP = null;
            datiCalcoloForDatiFondo = new DatiCalcolo();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            //servizio utile
            if (lServizioUtileCommon == null)
                GestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdRecordFondo(idRecordFondo, out lServizioUtileCommon);

            if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                datiCalcoloForDatiFondo.lDatiServizioUtileINPDAP = lServizioUtileCommon;

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiCalcoloForDatiFondo);

            List<Entity.DecCapitolo> listaDecCapitoloINPDAP;
            bool isPL = !(Utility.IsRicostituzione(datiPensione.Gruppo) || Utility.IsRiaperturaDomanda(datiPensione.Id));
            GetListaDecCapitoloINPDAP(isPL, out listaDecCapitoloINPDAP);


            GestioneCalcolo.DatiCalcoloContributivo datiContributiviDB = null;
            GestioneCalcolo.GetCalcoloContributivoByIdRecordFondo(idRecordFondo, out datiContributiviDB);

            if (datiContributiviDB != null)
            {
                datiCalcoloForDatiFondo.ImportoContributivoTotale = datiContributiviDB.ImportoContributivoTotale;
                if (datiContributiviDB.Montante != null)
                {
                    datiCalcoloForDatiFondo.Montante = Math.Round(datiContributiviDB.Montante.Value, 4);
                }

                datiCalcoloForDatiFondo.MontanteContributivo = datiContributiviDB.MontanteContributivo;
                datiCalcoloForDatiFondo.NSettimane = datiContributiviDB.NSettimane;
                datiCalcoloForDatiFondo.ImportoContribTotaleQuotaDL214 = datiContributiviDB.ImportoContribTotaleQuotaDL214;
                datiCalcoloForDatiFondo.MontanteQuotaDL214 = datiContributiviDB.MontanteQuotaDL214;
                datiCalcoloForDatiFondo.NSettimaneQuotaDL214 = datiContributiviDB.NSettimaneQuotaDL214;
                datiCalcoloForDatiFondo.QuotaContributivaAnnua = datiContributiviDB.QuotaContributivaAnnua;
            }

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (lServizioUtileCommon == null || lServizioUtileCommon.Count == 0) && datiContributiviDB == null)
            {
                string errori = string.Empty;

                try
                {
                    GestioneAggiornamentoPECO.DatiContributivi datiContributivi = null;
                    GestioneContrib.CrossDataRecipient crossDataRecipient = null;
                    if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI)
                    {
                        GestioneAggiornamentoPECO.GetDatiPECO_AMG_INPDAPbyNDomus(datiPensione, ref datiINPDAP, out errori);
                        if (!String.IsNullOrEmpty(errori))
                            throw new DNA.DnaValidationException(errori);
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(datiINPDAP, datiPensione, out lServizioUtileCommon, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);
                    }
                    else
                    {
                        GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                        if (!String.IsNullOrEmpty(errori))
                            throw new DNA.DnaValidationException(errori);
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out lServizioUtileCommon, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);
                    }


                    if (datiCalcoloForDatiFondo == null)
                        datiCalcoloForDatiFondo = new DatiCalcolo();

                    if (lServizioUtileCommon != null && lServizioUtileCommon.Count > 0)
                        datiCalcoloForDatiFondo.lDatiServizioUtileINPDAP = lServizioUtileCommon;

                    if (recordDatiFondoINPDAP != null && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)))
                    {
                        datiCalcoloForDatiFondo.Capitolo = recordDatiFondoINPDAP.Capitolo;
                    }
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }


            datiCalcoloForDatiFondo.lDecCapitolo = listaDecCapitoloINPDAP;
            datiCalcoloForDatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;
            GestioneContrib.TipoCalcolo tipoCalcolo;
            GestioneContrib.GetTipoCalcoloByDatiPensione(datiPensione, out tipoCalcolo);
            datiCalcoloForDatiFondo.TipoCalcolo = tipoCalcolo;
        }

        public static void GetListaDecCapitoloINPDAP(bool PL, out List<Entity.DecCapitolo> listaDecCapitoloINPDAP)
        {
            listaDecCapitoloINPDAP = new List<DecCapitolo>();
            List<GestioneDecodifica.DecCapitolo> listaDecodificaCapitolo = null;
            GestioneDecodifica.GetDecCapitolo(PL, out listaDecodificaCapitolo);
            if (listaDecodificaCapitolo != null && listaDecodificaCapitolo.Count > 0)
            {
                foreach (var dec in listaDecodificaCapitolo)
                {
                    DecCapitolo decCapitolo = new DecCapitolo();
                    Utility.ValorizzaOggetti(dec, decCapitolo);
                    listaDecCapitoloINPDAP.Add(decCapitolo);
                }
            }
        }

        public static bool ControlsDatiCalcolo(GestionePensione.DatiPensione datiPensione, DatiCalcolo entityDatiCalcolo, DatiArticolo2ForDatiFondo entityDatiArticolo2, object fondoXX, char? codiceSpecificoTraduzioneSuGP, string tipoSettimaneBeneficio,
            int? maggiorazioneAmianto, int? maggiorazioneInv74, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneContrib.DatiCalcolo datiCalcoloDZ, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            Utility.TipoCalcolo tipoCalcolo = Utility.GetTipoCalcoloById(datiPensione.TipoCalcolo, datiPensione, Utility.TipoAppartenenza.FS);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            decimal? pensioneAnnuaLorda = null;
            if ((tipoFondo == Utility.TipoFondo.PT || tipoFondo == Utility.TipoFondo.FS) && Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
               !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                pensioneAnnuaLorda = entityDatiCalcolo.PensioneAnnuaLorda214;
            else if (tipoFondo != Utility.TipoFondo.DZ)
                pensioneAnnuaLorda = entityDatiCalcolo.PensioneAnnuaLorda;

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            if (tipoCalcolo == Utility.TipoCalcolo.NonValido)
            {
                messaggioVideo = "Tipo calcolo non selezionato in Liquidazione Pensione.";
                return false;
            }

            if (entityDatiCalcolo != null)
            {
                if (!((tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && datiPensione.FineAssicurazione.HasValue &&
                    !Utility.DataStrettamenteSuccessivaA(datiPensione.FineAssicurazione.Value, new DateTime(1992, 12, 31))) &&
                    tipoCalcolo != (Utility.TipoCalcolo)Enum.Parse(typeof(GestioneContrib.TipoCalcolo), entityDatiCalcolo.TipoCalcolo.ToString())
                    && !(Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && !isRiaperturaDomanda && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT)))
                {
                    messaggioVideo = "Il tipo calcolo '" + tipoCalcolo.ToString() + "' salvato sul quadro Liquidazione Pensione è differente dai dati calcolo che si sta tentando di salvare";
                    return false;
                }
            }

            if (tipoFondo == Utility.TipoFondo.DZ)
            {
                if (datiCalcoloDZ.fondoDZ.lDatiServizioUtile != null && datiCalcoloDZ.fondoDZ.lDatiServizioUtile.Count > 0)
                {
                    if (!GestioneControlli.ControlsDatiServizioUtile(datiCalcoloDZ.fondoDZ.lDatiServizioUtile, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(datiCalcoloDZ.fondoDZ.lDatiServizioUtile, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo))
                        return false;
                }
            }
            else
            {
                if (entityDatiCalcolo.lDatiServizioUtile != null && entityDatiCalcolo.lDatiServizioUtile.Count > 0)
                {
                    if (!GestioneControlli.ControlsDatiServizioUtile(entityDatiCalcolo.lDatiServizioUtile, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(entityDatiCalcolo.lDatiServizioUtile, datiPensione.FineAssicurazione, tipoFondo, datiPensione, out messaggioVideo))
                        return false;
                }
            }

            if (tipoFondo != Utility.TipoFondo.DZ)
            {
                if (!GestioneControlli.ControlsDatiCalcoloFS_PTRecordFondo(datiPensione, isRiaperturaDomanda, pensioneAnnuaLorda, entityDatiCalcolo.ServizioUtileDirittoAA, entityDatiCalcolo.ServizioUtileDirittoMM,
                    entityDatiCalcolo.ServizioUtileDirittoGG, entityDatiCalcolo.ServizioUtileDirittoOIAA, entityDatiCalcolo.ServizioUtileDirittoOIMM, entityDatiCalcolo.ServizioUtileDirittoOIGG, entityDatiCalcolo.lDatiServizioUtile, tipoFondo, tipoCalcolo, codiceSpecificoTraduzioneSuGP, tipoSettimaneBeneficio, maggiorazioneAmianto,
                    maggiorazioneInv74, entityDatiCalcolo.ImportoContributivoTotale, entityDatiCalcolo.Montante, entityDatiCalcolo.MontanteContributivo, entityDatiCalcolo.NSettimane, entityDatiCalcolo.MontanteQuotaDL214, entityDatiCalcolo.ImportoContribTotaleQuotaDL214, entityDatiCalcolo.NSettimaneQuotaDL214, entityDatiCalcolo.QuotaContributivaAnnua, out messaggioVideo))
                    return false;

                decimal? palConBenefici = null;
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        palConBenefici = entityDatiArticolo2 != null ? entityDatiArticolo2.PALConBenefici : fondoXX != null ? ((GestioneFondo.DatiFondoPT)fondoXX).PALConBenefici : null;
                        break;
                    case Utility.TipoFondo.FS:
                        palConBenefici = entityDatiArticolo2 != null ? entityDatiArticolo2.PALConBenefici : fondoXX != null ? ((GestioneFondo.DatiFondoFST)fondoXX).PALConBenefici : null;
                        break;
                }
                if (datiPensione != null && datiPensione.SiglaCategoria != null && datiPensione.SiglaCategoria.Trim() != "SFS" && datiPensione.SiglaCategoria.Trim() != "SPT")
                {
                    if (!GestioneControlli.ControlPALBeneficiPAL(palConBenefici, pensioneAnnuaLorda, out messaggioVideo))
                    {
                        messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                        return false;
                    }
                }
                if (!GestioneControlli.ControlsAnniServizioUtiliDiritto(datiPensione, tipoFondo, datiMaggiorazioniBenefici, entityDatiCalcolo.ServizioUtileDirittoAA, entityDatiCalcolo.ServizioUtileDirittoMM,
                    entityDatiCalcolo.ServizioUtileDirittoGG, out messaggioVideo))
                {
                    messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                    return false;
                }
            }

            return true;
        }

        public static bool ControlsDatiCalcoloINPDAP(GestionePensione.DatiPensione datiPensione, DatiCalcolo entityDatiCalcolo, DatiArticolo2ForDatiFondo entityDatiArticolo2, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici, GestioneDanteCausa.DatiDanteCausa datiDanteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneContrib.TipoCalcolo tipoCalcolo = GestioneContrib.TipoCalcolo.NonValido;
            GestioneContrib.GetTipoCalcoloByDatiPensione(datiPensione, out tipoCalcolo);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoCalcolo == GestioneContrib.TipoCalcolo.NonValido)
            {
                messaggioVideo = "Tipo calcolo non selezionato in Liquidazione Pensione.";
                return false;
            }

            //ENG - Integrazione Modifiche Accenture
            if (!Utility.IsDomandaAutomatica(datiPensione))
            {
                if (!Utility.IsDomandaPensioneSuperstitiOrRicostituzione(datiPensione))
                {
                    if (tipoCalcolo != entityDatiCalcolo.TipoCalcolo)
                    {
                        messaggioVideo = "Il tipo calcolo '" + tipoCalcolo.ToString() + "' salvato sul quadro Liquidazione Pensione è differente dai dati calcolo che si sta tentando di salvare";
                        return false;
                    }
                }

                if (tipoCalcolo == GestioneContrib.TipoCalcolo.Retributivo && entityDatiCalcolo.CoefficienteTrasformazione.HasValue)
                {
                    messaggioVideo = "Con tipo calcolo '" + tipoCalcolo.ToString() + "' il Coefficiente di Trasformazione non deve essere acquisito.";
                    return false;
                }
                else if (tipoCalcolo != GestioneContrib.TipoCalcolo.Retributivo && !entityDatiCalcolo.CoefficienteTrasformazione.HasValue)
                {
                    messaggioVideo = "Con tipo calcolo '" + tipoCalcolo.ToString() + "' il Coefficiente di Trasformazione deve essere acquisito.";
                    return false;
                }

                if (GestioneBypassControllo.CheckAndLockBypassControlloByNomeBypass(datiPensione, GestioneBypassControllo.NomeBypass.DatiCalcolo_DatiCalcolo_FS.BYPASS_SERVIZIO_UTILE_GDP) ||
                    (Utility.IsDomandaINPDAP(datiPensione.Gestione) && (Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa))))
                    return true;

                if (entityDatiCalcolo.lDatiServizioUtileINPDAP != null && entityDatiCalcolo.lDatiServizioUtileINPDAP.Count > 0)
                {
                    if (!GestioneControlli.ControlsDatiServizioUtile(entityDatiCalcolo.lDatiServizioUtileINPDAP, out messaggioVideo))
                        return false;

                    if (!GestioneControlli.ControlsDatiServizioUtileWithFineAssicurazione(entityDatiCalcolo.lDatiServizioUtileINPDAP, datiPensione.FineAssicurazione, out messaggioVideo))
                        return false;
                }

                if (!GestioneControlli.ControlsDatiCalcoloRecordFondo(datiPensione, entityDatiCalcolo.PensioneAnnuaLorda, entityDatiCalcolo.ServizioUtileDirittoAA, entityDatiCalcolo.ServizioUtileDirittoMM,
                    entityDatiCalcolo.ServizioUtileDirittoGG, entityDatiCalcolo.ServizioUtileDirittoOIAA, entityDatiCalcolo.ServizioUtileDirittoOIMM, entityDatiCalcolo.ServizioUtileDirittoOIGG, entityDatiCalcolo.lDatiServizioUtileINPDAP, tipoCalcolo, entityDatiCalcolo.Divisore, entityDatiCalcolo.Capitolo, out messaggioVideo))
                    return false;

                decimal? palConBenefici = entityDatiArticolo2 != null ? entityDatiArticolo2.PALConBenefici : recordDatiFondoINPDAP != null ? recordDatiFondoINPDAP.PALConBenefici : null;
               
                    if (!GestioneControlli.ControlPALBeneficiPAL(palConBenefici, entityDatiCalcolo.PensioneAnnuaLorda, out messaggioVideo))
                    {
                        messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                        return false;
                    }
                
            }

            if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI && (datiPensione.SiglaCategoria.Trim()).EndsWith("CTPS") && tipoCalcolo == GestioneContrib.TipoCalcolo.Contributivo)
            {
                if (entityDatiCalcolo.NSettimane != null && entityDatiCalcolo.NSettimane > 0 && entityDatiCalcolo.NSettimane == entityDatiCalcolo.NSettimaneQuotaDL214 && (entityDatiCalcolo.MontanteContributivo == null || entityDatiCalcolo.MontanteContributivo == 0))
                {
                    messaggioVideo = "Verificare la scrittura su Felpe dei dati contributivi da legge 335, in quanto sprovvisti del valore di quota contributiva";
                    return false;
                }
            }

            if (!GestioneControlli.ControlsAnniServizioUtiliDiritto(datiPensione, tipoFondo, datiMaggiorazioniBenefici, entityDatiCalcolo.ServizioUtileDirittoAA, entityDatiCalcolo.ServizioUtileDirittoMM,
                entityDatiCalcolo.ServizioUtileDirittoGG, out messaggioVideo))
            {
                messaggioVideo = "Dati Calcolo: " + messaggioVideo;
                return false;
            }

            if (entityDatiCalcolo.PensioneAnnuaLorda.HasValue && entityDatiCalcolo.PensioneAnnuaLorda.Value == 0)
            {
                messaggioVideo = "Pensione annua lorda:è obbligatorio inserire un valore maggiore di 0";
                return false;
            }
            if (entityDatiCalcolo.ServizioUtileDirittoGG.Value == 0 && entityDatiCalcolo.ServizioUtileDirittoMM.Value == 0 && entityDatiCalcolo.ServizioUtileDirittoAA.Value == 0)
            {
                messaggioVideo = "Servizio Utile Diritto:è obbligatorio inserire un valore maggiore di 0";
                return false;
            }


            return true;
        }

        public static void StoreDatiCalcoloByidRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondoCommon,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, ref object fondoXX, DatiCalcolo datiCalcolo, GestioneContrib.DatiCalcolo datiCalcoloDZ)
        {
            if (datiCalcolo == null && datiCalcoloDZ == null)
                return;

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneContrib.TipoCalcolo tipoCalcolo;
            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            GestioneCalcolo.DatiCalcoloContributivo daticontributivi = new GestioneCalcolo.DatiCalcoloContributivo();
            GestioneCalcolo.DatiCalcoloRetributivo datiretributivi = new GestioneCalcolo.DatiCalcoloRetributivo();
            if (tipoFondo == Utility.TipoFondo.DZ)
            {
                Utility.ValorizzaOggetti(datiCalcoloDZ, datiretributivi);
                tipoCalcolo = datiCalcoloDZ.TipoCalcolo;
            }
            else
            {
                Utility.ValorizzaOggetti(datiCalcolo, daticontributivi);
                tipoCalcolo = datiCalcolo.TipoCalcolo;
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SalvaDatiCalcoloWithFondiByIdRecordFondo(datiCalcolo, idRecordFondo, fondoXX, ref datiFondoCommon, datiPensione, tipoFondo, datiCalcoloDZ);

                switch (tipoCalcolo)
                {
                    case GestioneContrib.TipoCalcolo.Contributivo:
                    case GestioneContrib.TipoCalcolo.Misto:
                    case GestioneContrib.TipoCalcolo.RetributivoMonti:
                        if (tipoFondo == Utility.TipoFondo.DZ)
                        {
                            datiretributivi.IdPensione = datiPensione.Id;
                            datiretributivi.IdRecordFondo = idRecordFondo;
                            GestioneCalcolo.SalvaCalcoloRetributivoRecordFondo(datiretributivi);

                            Utility.ValorizzaOggetti(datiCalcoloDZ, daticontributivi);
                            daticontributivi.IdPensione = datiPensione.Id;
                            daticontributivi.IdRecordFondo = idRecordFondo;
                            GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(daticontributivi);
                        }
                        else if (!daticontributivi.IsDatiCalcoloContributivoNull())
                        {
                            daticontributivi.IdPensione = datiPensione.Id;
                            daticontributivi.IdRecordFondo = idRecordFondo;
                            GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(daticontributivi);
                        }
                        break;
                    case GestioneContrib.TipoCalcolo.Retributivo:
                        if (tipoFondo == Utility.TipoFondo.DZ)
                        {
                            datiretributivi.IdPensione = datiPensione.Id;
                            datiretributivi.IdRecordFondo = idRecordFondo;
                            GestioneCalcolo.SalvaCalcoloRetributivoRecordFondo(datiretributivi);
                        }
                        break;
                }

                if (tipoFondo == Utility.TipoFondo.DZ)
                {
                    datiQuadroDatiRecordFondo.TabDatiCalcoloDZ = 2;
                    GestioneRecordFondo.DatiRecordFondo recordFondo;
                    GestioneRecordFondo.GetRecordFondoByIdRecordFondo(idRecordFondo, out recordFondo);
                    recordFondo.DecorrenzaValiditaDati = datiCalcoloDZ.fondoDZ.DecorrenzaValidita;
                    GestioneRecordFondo.SalvaSingoloRecordFondo(datiPensione.Id, recordFondo);
                }
                else
                    datiQuadroDatiRecordFondo.TabDatiCalcolo = 2;


                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }

            if (tipoFondo == Utility.TipoFondo.DZ)
                datiCalcoloDZ.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcoloDZ;
            else
                datiCalcolo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;
        }

        public static void StoreDatiCalcoloINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, DatiCalcolo datiCalcolo)
        {
            if (datiCalcolo == null)
                return;

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            GestioneCalcolo.DatiCalcoloContributivo daticontributivi = new GestioneCalcolo.DatiCalcoloContributivo();
            Utility.ValorizzaOggetti(datiCalcolo, daticontributivi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SalvaDatiCalcoloINPDAPByIdRecordFondo(datiCalcolo, idRecordFondo, datiPensione, ref recordDatiFondoINPDAP);

                switch (datiCalcolo.TipoCalcolo)
                {
                    case GestioneContrib.TipoCalcolo.Contributivo:
                    case GestioneContrib.TipoCalcolo.Misto:
                    case GestioneContrib.TipoCalcolo.RetributivoMonti:
                        if (!daticontributivi.IsDatiCalcoloContributivoNull())
                        {
                            daticontributivi.IdPensione = datiPensione.Id;
                            daticontributivi.IdRecordFondo = idRecordFondo;
                            GestioneCalcolo.SalvaCalcoloContributivoRecordFondo(daticontributivi);
                        }
                        break;
                }

                datiQuadroDatiRecordFondo.TabDatiCalcolo = 2;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiCalcolo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo;
        }

        private static void SalvaDatiCalcoloWithFondiByIdRecordFondo(DatiCalcolo datiCalcolo, long idRecordFondo, object Fondo, ref GestioneFondo.DatiFondo datiFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, GestioneContrib.DatiCalcolo datiCalcoloDZ)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {

                    #region Fondo PT
                    case Utility.TipoFondo.PT:

                        if (datiCalcolo == null)
                            datiCalcolo = new DatiCalcolo();

                        GestioneFondo.DatiFondoPT DatiFondoPT = (GestioneFondo.DatiFondoPT)Fondo;
                        if (DatiFondoPT == null)
                            DatiFondoPT = new GestioneFondo.DatiFondoPT();
                        Utility.ValorizzaOggetti(datiCalcolo, DatiFondoPT);

                        if (!DatiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                        {
                            if (datiFondo == null || datiFondo.IsFondoNull())
                                datiFondo = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
                            GestioneFondo.SalvaFondoPTRecordFondo(datiFondo.Id, idRecordFondo, DatiFondoPT);

                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                            if (datiCalcolo.lDatiServizioUtile != null && datiCalcolo.lDatiServizioUtile.Count > 0)
                            {
                                foreach (GestioneDatiServizioUtile.ServizioUtile su in datiCalcolo.lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtileRecordFondo(datiFondo.Id, idRecordFondo, su);
                                }
                            }
                        }
                        break;
                    #endregion Fondo PT

                    #region Fondo FS
                    case Utility.TipoFondo.FS:

                        if (datiCalcolo == null)
                            datiCalcolo = new DatiCalcolo();

                        GestioneFondo.DatiFondoFST DatiFondoFS = (GestioneFondo.DatiFondoFST)Fondo;
                        if (DatiFondoFS == null)
                            DatiFondoFS = new GestioneFondo.DatiFondoFST();
                        Utility.ValorizzaOggetti(datiCalcolo, DatiFondoFS);

                        //GestioneDatiServizioUtile.ServizioUtile servizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                        //Utility.ValorizzaOggetti(datiCalcolo, servizioUtile);

                        if (!DatiFondoFS.Equals(new GestioneFondo.DatiFondoFST()))
                        {
                            if (datiFondo == null || datiFondo.IsFondoNull())
                                datiFondo = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
                            GestioneFondo.SalvaFondoFSTRecordFondo(datiFondo.Id, idRecordFondo, DatiFondoFS);

                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                            if (datiCalcolo.lDatiServizioUtile != null && datiCalcolo.lDatiServizioUtile.Count > 0)
                            {
                                foreach (GestioneDatiServizioUtile.ServizioUtile su in datiCalcolo.lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtileRecordFondo(datiFondo.Id, idRecordFondo, su);
                                }
                            }
                        }
                        break;

                    #endregion Fondo FS

                    #region Fondo DZ
                    case Utility.TipoFondo.DZ:

                        if (datiCalcolo == null)
                            datiCalcolo = new DatiCalcolo();

                        GestioneFondo.DatiFondoDZ DatiFondoDZ = (GestioneFondo.DatiFondoDZ)Fondo;
                        if (DatiFondoDZ == null)
                            DatiFondoDZ = new GestioneFondo.DatiFondoDZ();
                        Utility.ValorizzaOggetti(datiCalcolo, DatiFondoDZ);
                        DatiFondoDZ.Sospensione = datiCalcoloDZ.fondoDZ.Sospensione;
                        DatiFondoDZ.PensioneBaseAnnua = datiCalcoloDZ.fondoDZ.PensioneBaseAnnua;
                        DatiFondoDZ.DecorrenzaValidita = datiCalcoloDZ.fondoDZ.DecorrenzaValidita;

                        if (!DatiFondoDZ.Equals(new GestioneFondo.DatiFondoDZ()))
                        {
                            if (datiFondo == null)
                            {
                                datiFondo = new GestioneFondo.DatiFondo();
                            }
                            datiFondo.QuotaA707 = datiCalcoloDZ.QuotaA707;
                            datiFondo.QuotaB707 = datiCalcoloDZ.QuotaB707;

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
                            GestioneFondo.SalvaFondoDZRecordFondo(datiFondo.Id, idRecordFondo, DatiFondoDZ);

                            GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                            if (datiCalcoloDZ.fondoDZ.lDatiServizioUtile != null && datiCalcoloDZ.fondoDZ.lDatiServizioUtile.Count > 0)
                            {
                                List<GestioneDatiServizioUtile.ServizioUtile> lDatiServizioUtileApp = new List<GestioneDatiServizioUtile.ServizioUtile>();
                                foreach (GestioneContrib.DatiServizioUtile servizioUtile in datiCalcoloDZ.fondoDZ.lDatiServizioUtile)
                                {
                                    GestioneDatiServizioUtile.ServizioUtile datiServizioUtile = new GestioneDatiServizioUtile.ServizioUtile();
                                    Utility.ValorizzaOggetti(servizioUtile, datiServizioUtile);
                                    lDatiServizioUtileApp.Add(datiServizioUtile);
                                }

                                foreach (GestioneDatiServizioUtile.ServizioUtile su in lDatiServizioUtileApp)
                                {
                                    GestioneDatiServizioUtile.SalvaDatiServizioUtileRecordFondo(datiFondo.Id, idRecordFondo, su);
                                }
                            }
                        }
                        break;
                        #endregion Fondo DZ

                }
            }
        }

        private static void SalvaDatiCalcoloINPDAPByIdRecordFondo(DatiCalcolo datiCalcolo, long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiCalcolo == null)
                datiCalcolo = new DatiCalcolo();

            if (recordDatiFondoINPDAP == null)
                recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();

            Utility.ValorizzaOggetti(datiCalcolo, recordDatiFondoINPDAP);

            if (Utility.IsRiaperturaDomanda(datiPensione.Id))
            {
                GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
                GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(datiPensione.Id, out datiStoricoGP);
                if (datiStoricoGP != null && datiPensione.TipoCalcolo != datiStoricoGP.TipoCalcolo && datiPensione.TipoCalcolo == 20)
                {
                    recordDatiFondoINPDAP.PensioneAnnuaLorda707 = null;
                }
            }
            GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

            if (datiCalcolo.lDatiServizioUtileINPDAP != null && datiCalcolo.lDatiServizioUtileINPDAP.Count > 0)
            {
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                foreach (GestioneDatiServizioUtileINPDAP.ServizioUtile su in datiCalcolo.lDatiServizioUtileINPDAP)
                    GestioneDatiServizioUtileINPDAP.SalvaDatiServizioUtileRecordFondo(datiPensione.Id, idRecordFondo, su);
            }
        }

        public static void DeleteDatiCalcoloByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione,
            ref GestioneFondo.DatiFondo datiFondoCommon, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX,
            ref GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (datiFondoCommon == null)
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoCommon);
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            #region gestioneFondi

            GestioneFondo.DatiFondoFST datiFondoFST = null;
            GestioneFondo.DatiFondoPT datiFondoPT = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.FS:
                        datiFondoFST = (GestioneFondo.DatiFondoFST)fondoXX;
                        if (datiFondoFST != null)
                        {
                            if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                datiFondoFST.PensioneAnnuaLorda214 = null;

                            datiFondoFST.PensioneAnnuaLorda = null;
                            datiFondoFST.ServizioUtileDirittoAA = null;
                            datiFondoFST.ServizioUtileDirittoMM = null;
                            datiFondoFST.ServizioUtileDirittoGG = null;
                            datiFondoFST.RMSSenzaLegge33670QA = null;
                            //ENG - PL Reversibilita
                            datiFondoFST.IsPensioneAnnuaLordaDaPrelievo = null;
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        if (datiFondoPT != null)
                        {
                            if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                                !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                                datiFondoPT.PensioneAnnuaLorda214 = null;

                            datiFondoPT.PensioneAnnuaLorda = null;
                            datiFondoPT.ServizioUtileDirittoAA = null;
                            datiFondoPT.ServizioUtileDirittoMM = null;
                            datiFondoPT.ServizioUtileDirittoGG = null;
                            //ENG - PL Reversibilita
                            datiFondoPT.IsPensioneAnnuaLordaDaPrelievo = null;
                        }
                        break;
                }
            }
            #endregion gestioneFondi

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiFondoCommon != null)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.FS:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                                DeleteDatiContributiviWithFondiByIdRecordFondo(idRecordFondo, tipoFondo, datiFondoFST, datiFondoCommon.Id);
                                break;
                            case Utility.TipoFondo.PT:
                                GestioneDatiServizioUtile.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                                DeleteDatiContributiviWithFondiByIdRecordFondo(idRecordFondo, tipoFondo, datiFondoPT, datiFondoCommon.Id);
                                break;
                        }
                    }
                }

                GestioneCalcolo.EliminaCalcoloContributivoByIdRecordFondo(idRecordFondo, false);

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

        public static void DeleteDatiCalcoloINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            ref GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            DatiCalcolo entityDatiCalcolo = new DatiCalcolo();
            Utility.ValorizzaOggetti(entityDatiCalcolo, recordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneDatiServizioUtileINPDAP.EliminaDatiServizioUtileByIdRecordFondo(idRecordFondo);
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                GestioneCalcolo.EliminaCalcoloContributivoByIdRecordFondo(idRecordFondo, false);

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

        private static void DeleteDatiContributiviWithFondiByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, object Fondo, long idFondoGenerico)
        {
            if (tipoFondo.HasValue && Fondo != null)
            {
                switch (tipoFondo.Value)
                {

                    case Utility.TipoFondo.FS:
                        GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)Fondo;
                        GestioneFondo.SalvaFondoFSTRecordFondo(idFondoGenerico, idRecordFondo, datiFondoFST);
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)Fondo;
                        GestioneFondo.SalvaFondoPTRecordFondo(idFondoGenerico, idRecordFondo, datiFondoPT);
                        break;
                }
            }
        }

        #endregion Dati Calcolo

        #region Dati Calcolo 707

        public static void GetDatiCalcoloINPDAP707ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, csAggiornamentoPECO_Fondi_AMG_INPDAP datiINPDAP, csAggiornamentoPECO_Fondi_AMG dati, char? codiceSpecificoTraduzioneSuGP,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref List<GestioneCalcolo.ServizioUtileINPDAP707> lServizioUtileINPDAP707,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out DatiCalcolo707 datiCalcolo707ForDatiFondo, out string errori)
        {
            errori = string.Empty;
            datiCalcolo707ForDatiFondo = new DatiCalcolo707();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            //servizio utile
            if (lServizioUtileINPDAP707 == null)
                GestioneCalcolo.GetDatiServizioUtileINPDAP707ByIdRecordFondo(idRecordFondo, out lServizioUtileINPDAP707);

            if (lServizioUtileINPDAP707 != null && lServizioUtileINPDAP707.Count > 0)
            {
                datiCalcolo707ForDatiFondo.LDatiServizioUtile707 = new List<DatiCalcolo707.DatiServizioUtile707>();
                foreach (GestioneCalcolo.ServizioUtileINPDAP707 servizioUtileINPDAP707 in lServizioUtileINPDAP707)
                {
                    DatiCalcolo707.DatiServizioUtile707 datiServizioUtile707 = new DatiCalcolo707.DatiServizioUtile707();
                    Utility.ValorizzaOggetti(servizioUtileINPDAP707, datiServizioUtile707);
                    datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Add(datiServizioUtile707);
                }
            }

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiCalcolo707ForDatiFondo);

            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (lServizioUtileINPDAP707 == null || lServizioUtileINPDAP707.Count == 0))
            {
                try
                {
                    if ((datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN || datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SPI) && datiINPDAP == null)
                        GestioneAggiornamentoPECO.GetDatiPECO_AMG_INPDAPbyNDomus(datiPensione, ref datiINPDAP, out errori);
                    else if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.AMG && dati == null)
                        GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        return;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }
            GestioneContrib.CrossDataRecipient crossDataRecipient = null;
            if (dati != null || datiINPDAP != null)
            {
                try
                {
                    GestioneAggiornamentoPECO.DatiContributivi datiContributivi = null;
                    DatiCalcolo datiCalcoloForDatiFondo = null;
                    if (dati != null)
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out lServizioUtileINPDAP707, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);
                    else if (datiINPDAP != null)
                        GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(datiINPDAP, datiPensione, out lServizioUtileINPDAP707, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);

                    if (lServizioUtileINPDAP707 != null && lServizioUtileINPDAP707.Count > 0)
                    {
                        datiCalcolo707ForDatiFondo.LDatiServizioUtile707 = new List<DatiCalcolo707.DatiServizioUtile707>();
                        foreach (GestioneCalcolo.ServizioUtileINPDAP707 servizioUtileINPDAP707 in lServizioUtileINPDAP707)
                        {
                            DatiCalcolo707.DatiServizioUtile707 datiServizioUtile707 = new DatiCalcolo707.DatiServizioUtile707();
                            Utility.ValorizzaOggetti(servizioUtileINPDAP707, datiServizioUtile707);
                            datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Add(datiServizioUtile707);
                        }

                        if (crossDataRecipient.PensioneAnnuaLorda707 != null)
                        {
                            datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 = crossDataRecipient.PensioneAnnuaLorda707;
                        }
                    }
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }

            bool isDatiServizioUtile707Presenti = datiCalcolo707ForDatiFondo.LDatiServizioUtile707 != null && datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Count > 0 ? true : false;

            if (!datiQuadroDatiRecordFondo.TabDatiCalcolo707.HasValue &&
                GestioneContrib.IsSettimane707Visible(datiPensione, codiceSpecificoTraduzioneSuGP, crossDataRecipient != null ? crossDataRecipient.IsQuotaDPresente : false) &&
                Utility.IsDomandaINPDAP(datiPensione.Gestione)
                /*&& (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica || (isDatiServizioUtile707Presenti && Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))))*/
                )
            {
                datiCalcolo707ForDatiFondo.Semaforo = 0;
                Utility.StatoPensione? statoPensione = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());
                if (!(statoPensione.HasValue && (statoPensione == Utility.StatoPensione.Calcolata || statoPensione == Utility.StatoPensione.CalcolataNoWebDom ||
                      statoPensione == Utility.StatoPensione.CalcolataNoFelpe || statoPensione == Utility.StatoPensione.CalcolataNoOneri)))
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        datiQuadroDatiRecordFondo.TabDatiCalcolo707 = datiCalcolo707ForDatiFondo.Semaforo;
                        GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);
                        transactionScope.Complete();
                    }
                }
            }
            else
                datiCalcolo707ForDatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo707;
        }

        public static void GetDatiCalcolo707ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo, csAggiornamentoPECO_Fondi_AMG dati,
            char? codiceSpecificoTraduzioneSuGP, bool isCancelOperation, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX,
            ref List<GestioneCalcolo.ServizioUtile707> lServizioUtile707, out DatiCalcolo707 datiCalcolo707ForDatiFondo, GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione, out string errori)
        {
            errori = string.Empty;
            datiCalcolo707ForDatiFondo = new DatiCalcolo707();
            Utility.StatoPensione? statoPensione = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            //servizio utile
            if (lServizioUtile707 == null)
                GestioneCalcolo.GetDatiServizioUtile707ByIdRecordFondo(idRecordFondo, out lServizioUtile707);

            if (lServizioUtile707 != null && lServizioUtile707.Count > 0)
            {
                datiCalcolo707ForDatiFondo.LDatiServizioUtile707 = new List<DatiCalcolo707.DatiServizioUtile707>();
                foreach (GestioneCalcolo.ServizioUtile707 servizioUtile707 in lServizioUtile707)
                {
                    DatiCalcolo707.DatiServizioUtile707 datiServizioUtile707 = new DatiCalcolo707.DatiServizioUtile707();
                    Utility.ValorizzaOggetti(servizioUtile707, datiServizioUtile707);
                    datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Add(datiServizioUtile707);
                }
            }
            switch (tipoFondo)
            {
                case Utility.TipoFondo.FS:
                    if (fondoXX != null)
                    {
                        GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)fondoXX;
                        datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 = datiFondoFST.PensioneAnnuaLorda707;
                        datiCalcolo707ForDatiFondo.IsPensioneAnnuaLorda707DaPrelievo = datiFondoFST.IsPensioneAnnuaLorda707DaPrelievo;
                    }
                    break;
                case Utility.TipoFondo.PT:
                    if (fondoXX != null)
                    {
                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 = datiFondoPT.PensioneAnnuaLorda707;
                        datiCalcolo707ForDatiFondo.IsPensioneAnnuaLorda707DaPrelievo = datiFondoPT.IsPensioneAnnuaLorda707DaPrelievo;
                    }
                    break;
            }
            if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && (lServizioUtile707 == null || lServizioUtile707.Count == 0) && dati == null &&
                !(statoPensione.HasValue && (statoPensione == Utility.StatoPensione.Calcolata || statoPensione == Utility.StatoPensione.CalcolataNoWebDom || statoPensione == Utility.StatoPensione.CalcolataNoFelpe || statoPensione == Utility.StatoPensione.CalcolataNoOneri)))
            {
                try
                {
                    GestioneAggiornamentoPECO.GetDatiPECO_AMGbyNDomus(datiPensione, ref dati, out errori);
                    if (!String.IsNullOrEmpty(errori))
                        return;
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }
            GestioneContrib.CrossDataRecipient crossDataRecipient = null;
            if (dati != null)
            {
                try
                {
                    DatiCalcolo datiCalcoloForDatiFondo = null;
                    GestioneAggiornamentoPECO.DatiContributivi datiContributivi = null;
                    GestioneAggiornamentoPECO.RecuperaDatiTotaliAMGFelpe(dati, datiPensione, out lServizioUtile707, out datiContributivi, out datiCalcoloForDatiFondo, out crossDataRecipient);

                    if (lServizioUtile707 != null && lServizioUtile707.Count > 0)
                        datiCalcolo707ForDatiFondo.LDatiServizioUtile707 = new List<DatiCalcolo707.DatiServizioUtile707>();
                    foreach (GestioneCalcolo.ServizioUtile707 servizioUtile707 in lServizioUtile707)
                    {
                        DatiCalcolo707.DatiServizioUtile707 datiServizioUtile707 = new DatiCalcolo707.DatiServizioUtile707();
                        Utility.ValorizzaOggetti(servizioUtile707, datiServizioUtile707);
                        datiCalcolo707ForDatiFondo.LDatiServizioUtile707.Add(datiServizioUtile707);
                    }
                    if ((datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 == null || Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id))) && crossDataRecipient.PensioneAnnuaLorda707 != null)
                    {
                        datiCalcolo707ForDatiFondo.PensioneAnnuaLorda707 = crossDataRecipient.PensioneAnnuaLorda707;
                    }
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
            }

            if (!datiQuadroDatiRecordFondo.TabDatiCalcolo707.HasValue &&
                GestioneContrib.IsSettimane707Visible(datiPensione, codiceSpecificoTraduzioneSuGP, crossDataRecipient != null ? crossDataRecipient.IsQuotaDPresente : false) &&
                ((tipoFondo.HasValue && new List<Utility.TipoFondo> { Utility.TipoFondo.FS, Utility.TipoFondo.PT }.Contains(tipoFondo.Value) &&
                (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica || datiPensione.IsPLUnicarpe.GetValueOrDefault() || Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))) || Utility.IsDomandaINPDAP(datiPensione.Gestione)))
            {
                datiCalcolo707ForDatiFondo.Semaforo = 0;
                if (!(statoPensione.HasValue && (statoPensione == Utility.StatoPensione.Calcolata || statoPensione == Utility.StatoPensione.CalcolataNoWebDom ||
                      statoPensione == Utility.StatoPensione.CalcolataNoFelpe || statoPensione == Utility.StatoPensione.CalcolataNoOneri)))
                {
                    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                    {
                        datiQuadroDatiRecordFondo.TabDatiCalcolo707 = datiCalcolo707ForDatiFondo.Semaforo;
                        GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);
                        transactionScope.Complete();
                    }
                }
            }
            else
                datiCalcolo707ForDatiFondo.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo707;
        }

        public static void StoreDatiCalcolo707ByidRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondoCommon,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, ref object fondoXX, DatiCalcolo707 datiCalcolo707)
        {
            if (datiCalcolo707 == null)
                return;

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SalvaDatiCalcolo707WithFondiByIdRecordFondo(datiCalcolo707, idRecordFondo, fondoXX, ref datiFondoCommon, datiPensione, tipoFondo, danteCausa, datiLavorazione);
                datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 2;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiCalcolo707.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo707;
        }

        private static void SalvaDatiCalcolo707WithFondiByIdRecordFondo(DatiCalcolo707 datiCalcolo707, long idRecordFondo, object Fondo, ref GestioneFondo.DatiFondo datiFondo, GestionePensione.DatiPensione datiPensione, Utility.TipoFondo? tipoFondo,
            GestioneDanteCausa.DatiDanteCausa danteCausa, GestioneLavorazione.DatiLavorazione datiLavorazione)
        {
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {

                    #region Fondo PT
                    case Utility.TipoFondo.PT:

                        if (datiCalcolo707 == null)
                            datiCalcolo707 = new DatiCalcolo707();

                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)Fondo;
                        if (datiFondoPT == null)
                            datiFondoPT = new GestioneFondo.DatiFondoPT();
                        Utility.ValorizzaOggetti(datiCalcolo707, datiFondoPT);

                        if (!datiFondoPT.Equals(new GestioneFondo.DatiFondoPT()))
                        {
                            if (datiFondo == null || datiFondo.IsFondoNull())
                                datiFondo = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
                            GestioneFondo.SalvaFondoPTRecordFondo(datiFondo.Id, idRecordFondo, datiFondoPT);

                            //ENG - RIC/PL Reversibilità 024 (necessario per avere lo stesso comportamento del tab DatiCalcolo)
                            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);

                            if (datiCalcolo707.LDatiServizioUtile707 != null && datiCalcolo707.LDatiServizioUtile707.Count > 0)
                            {
                                if (!Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))
                                    GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);

                                GestioneCalcolo.ServizioUtile707 su707 = new GestioneCalcolo.ServizioUtile707();
                                foreach (DatiCalcolo707.DatiServizioUtile707 dsu in datiCalcolo707.LDatiServizioUtile707)
                                {
                                    Utility.ValorizzaOggetti(dsu, su707);
                                    GestioneCalcolo.SalvaDatiServizioUtile707RecordFondo(datiFondo.Id, idRecordFondo, su707);
                                }
                            }
                        }
                        break;
                    #endregion Fondo PT

                    #region Fondo FS
                    case Utility.TipoFondo.FS:

                        if (datiCalcolo707 == null)
                            datiCalcolo707 = new DatiCalcolo707();

                        GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)Fondo;
                        if (datiFondoFST == null)
                            datiFondoFST = new GestioneFondo.DatiFondoFST();
                        Utility.ValorizzaOggetti(datiCalcolo707, datiFondoFST);

                        if (!datiFondoFST.Equals(new GestioneFondo.DatiFondoFST()))
                        {
                            if (datiFondo == null || datiFondo.IsFondoNull())
                                datiFondo = new GestioneFondo.DatiFondo();

                            GestioneFondo.SalvaFondoDatiGenerici(datiPensione.Id, datiFondo);
                            GestioneFondo.SalvaFondoFSTRecordFondo(datiFondo.Id, idRecordFondo, datiFondoFST);

                            //ENG - RIC/PL Reversibilità 024 (necessario per avere lo stesso comportamento del tab DatiCalcolo)
                            if (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);

                            if (datiCalcolo707.LDatiServizioUtile707 != null && datiCalcolo707.LDatiServizioUtile707.Count > 0)
                            {
                                if (!Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione))
                                    GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);

                                GestioneCalcolo.ServizioUtile707 su707 = new GestioneCalcolo.ServizioUtile707();
                                foreach (DatiCalcolo707.DatiServizioUtile707 dsu in datiCalcolo707.LDatiServizioUtile707)
                                {
                                    Utility.ValorizzaOggetti(dsu, su707);
                                    GestioneCalcolo.SalvaDatiServizioUtile707RecordFondo(datiFondo.Id, idRecordFondo, su707);
                                }
                            }
                        }
                        break;

                        #endregion Fondo FS
                }
            }
        }

        public static void StoreDatiCalcoloINPDAP707ByidRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, DatiCalcolo707 datiCalcolo707)
        {
            if (datiCalcolo707 == null)
                return;

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                SalvaDatiCalcoloINPDAP707ByIdRecordFondo(datiCalcolo707, idRecordFondo, datiPensione, ref recordDatiFondoINPDAP);
                datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 2;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.Tipo = 2;
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiCalcolo707.Semaforo = datiQuadroDatiRecordFondo.TabDatiCalcolo707;
        }

        private static void SalvaDatiCalcoloINPDAP707ByIdRecordFondo(DatiCalcolo707 datiCalcolo707, long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (datiCalcolo707 == null)
                datiCalcolo707 = new DatiCalcolo707();

            if (recordDatiFondoINPDAP == null)
                recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();

            Utility.ValorizzaOggetti(datiCalcolo707, recordDatiFondoINPDAP);
            GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

            if (datiCalcolo707.LDatiServizioUtile707 != null && datiCalcolo707.LDatiServizioUtile707.Count > 0)
            {
                GestioneCalcolo.EliminaDatiServizioUtileINPDAP707ByIdRecordFondo(idRecordFondo);
                GestioneCalcolo.ServizioUtileINPDAP707 su707 = new GestioneCalcolo.ServizioUtileINPDAP707();
                foreach (DatiCalcolo707.DatiServizioUtile707 dsu in datiCalcolo707.LDatiServizioUtile707)
                {
                    Utility.ValorizzaOggetti(dsu, su707);
                    GestioneCalcolo.SalvaDatiServizioUtileINPDAP707(datiPensione.Id, idRecordFondo, su707);
                }
            }
        }

        public static void DeleteDatiCalcolo707ByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione,
         ref GestioneFondo.DatiFondo datiFondoCommon, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX,
         ref GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (datiFondoCommon == null)
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoCommon);
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            #region gestioneFondi

            GestioneFondo.DatiFondoFST datiFondoFST = null;
            GestioneFondo.DatiFondoPT datiFondoPT = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case Utility.TipoFondo.FS:
                        datiFondoFST = (GestioneFondo.DatiFondoFST)fondoXX;
                        if (datiFondoFST != null)
                        {
                            datiFondoFST.PensioneAnnuaLorda707 = null;
                            datiFondoFST.IsPensioneAnnuaLorda707DaPrelievo = null;
                        }

                        break;
                    case Utility.TipoFondo.PT:
                        datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        if (datiFondoPT != null)
                        {
                            datiFondoPT.PensioneAnnuaLorda707 = null;
                            datiFondoPT.IsPensioneAnnuaLorda707DaPrelievo = null;
                        }
                        break;
                }
            }
            #endregion gestioneFondi

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiFondoCommon != null)
                {
                    if (tipoFondo.HasValue)
                    {
                        switch (tipoFondo.Value)
                        {
                            case Utility.TipoFondo.FS:
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);
                                DeleteDatiContributiviWithFondiByIdRecordFondo(idRecordFondo, tipoFondo, datiFondoFST, datiFondoCommon.Id);
                                break;
                            case Utility.TipoFondo.PT:
                                GestioneCalcolo.EliminaDatiServizioUtile707ByIdRecordFondo(idRecordFondo);
                                DeleteDatiContributiviWithFondiByIdRecordFondo(idRecordFondo, tipoFondo, datiFondoPT, datiFondoCommon.Id);
                                break;
                        }
                    }
                }
                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 0;
                else
                    datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static void DeleteDatiCalcoloINPDAP707ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione,
            ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            ref GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo)
        {
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            DatiCalcolo707 entityDatiCalcolo = new DatiCalcolo707();
            Utility.ValorizzaOggetti(entityDatiCalcolo, recordDatiFondoINPDAP);

            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneCalcolo.EliminaDatiServizioUtileINPDAP707ByIdRecordFondo(idRecordFondo);
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);//devo creare il metodo SalvaRecordDatiFondoINPDAP707??

                if (tipoDomanda != Utility.TipoDomanda.Ricostituzione || Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 0;
                else
                    datiQuadroDatiRecordFondo.TabDatiCalcolo707 = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = 0;
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        #endregion Dati Calcolo 707

        #region Legge 4/60

        public static void GetDatiLegge460ByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX, out DatiLegge460 datiLegge460)
        {
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);
            datiLegge460 = new DatiLegge460();
            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (fondoXX != null)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.FS:
                        Utility.ValorizzaOggetti(fondoXX, datiLegge460);
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        Utility.ValorizzaOggetti(fondoXX, datiLegge460);
                        datiLegge460.NCertificato = datiFondoPT.Ncertificato != 0 ? datiFondoPT.Ncertificato.ToString() : string.Empty;
                        break;
                }
            }
            datiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;
        }

        public static void GetDatiLegge460INPDAPByIdRecordFondo(long idRecordFondo, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            out DatiLegge460 datiLegge460)
        {
            datiLegge460 = new DatiLegge460();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiLegge460);

            datiLegge460.NCertificato = recordDatiFondoINPDAP.Ncertificato != 0 ? recordDatiFondoINPDAP.Ncertificato.ToString() : string.Empty;

            datiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;
        }

        public static void StoreDatiLegge460ByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, ref GestioneFondo.DatiFondo datiFondoCommon, ref object fondoXX, DatiLegge460 datiLegge460)
        {
            if (datiLegge460 == null)
                datiLegge460 = new DatiLegge460();

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            if (tipoFondo == Utility.TipoFondo.FS)
                return;

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);


            GestioneFondo.DatiFondoPT datiFondoPT;
            if (fondoXX == null)
            {
                GestioneFondo.GetFondoPTByIdRecordFondo(idRecordFondo, out datiFondoPT);
                fondoXX = datiFondoPT;
            }
            else
                datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
            long idFondo = 0;
            if (datiFondoCommon != null)
                idFondo = datiFondoCommon.Id;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiLegge460ByIdRecordFondoPrivate(idRecordFondo, idFondo, datiLegge460, datiFondoPT);
                if (datiLegge460 != null && !datiLegge460.IsDatiLegge460Null())
                    datiQuadroDatiRecordFondo.TabLegge460 = 2;
                datiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static void StoreDatiLegge460INPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, DatiLegge460 datiLegge460)
        {
            if (datiLegge460 == null)
                datiLegge460 = new DatiLegge460();

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                StoreDatiLegge460INPDAPByIdRecordFondoPrivate(idRecordFondo, datiPensione.Id, datiLegge460, recordDatiFondoINPDAP);
                if (datiLegge460 != null && !datiLegge460.IsDatiLegge460Null())
                    datiQuadroDatiRecordFondo.TabLegge460 = 2;
                datiLegge460.Semaforo = datiQuadroDatiRecordFondo.TabLegge460;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiLegge460ByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione,
            ref object fondoXX, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            GestioneFondo.DatiFondoPT datiFondoPT = null;
            if (fondoXX == null)
            {
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria));
            }
            datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (datiFondoPT != null)
                {
                    DatiLegge460 datiLegge460 = new DatiLegge460();
                    Utility.ValorizzaOggetti(datiLegge460, datiFondoPT);
                    datiFondoPT.Ncertificato = null;
                    GestioneFondo.SalvaFondoPTRecordFondo(datiFondoPT.IdFondo, idRecordFondo, datiFondoPT);
                }
                datiQuadroDatiRecordFondo.TabLegge460 = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiLegge460INPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (recordDatiFondoINPDAP != null)
                {
                    DatiLegge460 datiLegge460 = new DatiLegge460();
                    Utility.ValorizzaOggetti(datiLegge460, recordDatiFondoINPDAP);
                    recordDatiFondoINPDAP.Ncertificato = null;
                    GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);
                }
                datiQuadroDatiRecordFondo.TabLegge460 = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static bool ControlDatiLegge460(DatiLegge460 datiLegge460, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiLegge460 != null)
            {
                if (!((datiLegge460.SiglaCategoria.HasValue && datiLegge460.CodiceSede.HasValue && !string.IsNullOrEmpty(datiLegge460.NCertificato) &&
                       datiLegge460.NMesiRiscattati.HasValue && datiLegge460.NMesiTotali.HasValue) ||
                      (!datiLegge460.SiglaCategoria.HasValue && !datiLegge460.CodiceSede.HasValue && string.IsNullOrEmpty(datiLegge460.NCertificato) &&
                        !datiLegge460.NMesiRiscattati.HasValue && !datiLegge460.NMesiTotali.HasValue)))
                {
                    messaggioVideo = "I dati Legge 4/60 devono essere tutti presenti contemporaneamente";
                    return false;
                }

                if (!string.IsNullOrEmpty(datiLegge460.NCertificato) && int.Parse(datiLegge460.NCertificato) == 0)
                {
                    messaggioVideo = "Il 'Certificato' non può essere pari a 0";
                    return false;
                }

                if (datiLegge460.NMesiRiscattati.HasValue && datiLegge460.NMesiRiscattati.Value == 0)
                {
                    messaggioVideo = "Il 'Numero Mesi Riscattati' non può essere pari a 0";
                    return false;
                }

                if (datiLegge460.NMesiTotali.HasValue && datiLegge460.NMesiTotali.Value == 0)
                {
                    messaggioVideo = "Il 'Numero Mesi Totali' non può essere pari a 0";
                    return false;
                }

                if (!string.IsNullOrEmpty(datiLegge460.NCertificato) && datiLegge460.NCertificato.Length != 8)
                {
                    messaggioVideo = "Il 'Certificato' deve essere lungo 8";
                    return false;
                }

                if (datiLegge460.CodiceSede.HasValue && !Utility.ExistSedeProvinciale(datiLegge460.CodiceSede.Value))
                {
                    messaggioVideo = "La 'Sede' inserita non esiste";
                    return false;
                }
            }

            return true;
        }

        #endregion Legge 4/60

        #region Dati Privilegiate

        public static void GetDatiPrivilegiateByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, ref object fondoXX, out DatiPrivilegiate datiPrivilegiate)
        {
            datiPrivilegiate = new DatiPrivilegiate();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.FS:
                        GestioneFondo.DatiFondoFST datiFondoFST = (GestioneFondo.DatiFondoFST)fondoXX;
                        if (datiFondoFST != null)
                        {
                            Utility.ValorizzaOggetti(datiFondoFST, datiPrivilegiate);
                        }
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        if (datiFondoPT != null)
                        {
                            Utility.ValorizzaOggetti(datiFondoPT, datiPrivilegiate);
                        }
                        break;
                }
            }
            datiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;
        }

        public static void GetDatiPrivilegiateINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out DatiPrivilegiate datiPrivilegiate)
        {
            datiPrivilegiate = new DatiPrivilegiate();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiPrivilegiate);

            datiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;
        }

        public static void StoreDatiPrivilegiateByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref GestioneFondo.DatiFondo datiFondoCommon, ref object fondoXX, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, DatiPrivilegiate datiPrivilegiate)
        {
            if (datiPrivilegiate == null)
                datiPrivilegiate = new DatiPrivilegiate();

            long idFondo = 0;
            if (datiFondoCommon == null)
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoCommon);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            GestioneFondo.DatiFondoPT datiFondoPTCommon = null;
            GestioneFondo.DatiFondoFST datiFondoFSCommon = null;
            idFondo = datiFondoCommon.Id;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoPT();
                        datiFondoPTCommon = (GestioneFondo.DatiFondoPT)fondoXX;
                        Utility.ValorizzaOggetti(datiPrivilegiate, datiFondoPTCommon);
                        break;
                    case Utility.TipoFondo.FS:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoFST();
                        datiFondoFSCommon = (GestioneFondo.DatiFondoFST)fondoXX;
                        Utility.ValorizzaOggetti(datiPrivilegiate, datiFondoFSCommon);
                        break;
                }
            }
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            GestioneFondo.SalvaFondoPTRecordFondo(idFondo, idRecordFondo, datiFondoPTCommon);
                            break;
                        case Utility.TipoFondo.FS:
                            GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, idRecordFondo, datiFondoFSCommon);
                            break;
                    }
                }
                datiQuadroDatiRecordFondo.TabPrivilegiate = 2;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;
        }

        public static void StoreDatiPrivilegiateINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, DatiPrivilegiate datiPrivilegiate)
        {
            if (datiPrivilegiate == null)
                datiPrivilegiate = new DatiPrivilegiate();

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            Utility.ValorizzaOggetti(datiPrivilegiate, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (datiPrivilegiate.IsDatiPrivilegiateNull())
                    datiQuadroDatiRecordFondo.TabPrivilegiate = 1;
                else
                    datiQuadroDatiRecordFondo.TabPrivilegiate = 2;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiPrivilegiate.Semaforo = datiQuadroDatiRecordFondo.TabPrivilegiate;
        }

        public static void EliminaDatiPrivilegiateByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref object fondoXX,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoFST datiFondoFS = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        if (datiFondoPT != null)
                        {
                            datiFondoPT.AssegnoCura = null;
                            datiFondoPT.AssegnoIntegrativo = null;
                            datiFondoPT.Categoria2aInfermita = null;
                            datiFondoPT.CumuloInfermita = null;
                            datiFondoPT.IndennitaAccompagnamentoAggiuntiva = null;
                            datiFondoPT.PrivilegiataSuperinvaliditaIndennita = null;
                            datiFondoPT.IndennitaSpecialeAnnua = null;
                            datiFondoPT.IntegrazioneIndennitaAssistenza = null;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        datiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                        if (datiFondoFS != null)
                        {
                            datiFondoFS.AssegnoCura = null;
                            datiFondoFS.AssegnoIntegrativo = null;
                            datiFondoFS.Categoria2aInfermita = null;
                            datiFondoFS.CumuloInfermita = null;
                            datiFondoFS.IndennitaAccompagnamentoAggiuntiva = null;
                            datiFondoFS.PrivilegiataSuperinvaliditaIndennita = null;
                            datiFondoFS.IndennitaSpecialeAnnua = null;
                            datiFondoFS.IntegrazioneIndennitaAssistenza = null;
                        }
                        break;
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            if (datiFondoPT != null)
                                GestioneFondo.SalvaFondoPTRecordFondo(datiFondoPT.IdFondo, idRecordFondo, datiFondoPT);
                            break;
                        case Utility.TipoFondo.FS:
                            if (datiFondoFS != null)
                                GestioneFondo.SalvaFondoFSTRecordFondo(datiFondoFS.IdFondo, idRecordFondo, datiFondoFS);
                            break;
                    }
                }
                datiQuadroDatiRecordFondo.TabPrivilegiate = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiPrivilegiateINPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            DatiPrivilegiate entityDatiPrivilegiate = new DatiPrivilegiate();
            Utility.ValorizzaOggetti(entityDatiPrivilegiate, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                datiQuadroDatiRecordFondo.TabPrivilegiate = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiPrivilegiate(string siglaCategoria, DatiPrivilegiate datiPrivilegiate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            #region obbligatorietà

            if (datiPrivilegiate != null && !datiPrivilegiate.AssegnoCura.HasValue)
            {
                messaggioVideo = "Assegno di Cura dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.AssegnoIntegrativo.HasValue)
            {
                messaggioVideo = "Assegno Integrativo dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.Categoria2aInfermita.HasValue)
            {
                messaggioVideo = "Categoria 2° Infermità dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.CumuloInfermita.HasValue)
            {
                messaggioVideo = "cumulo Infermintà dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.HasValue)
            {
                messaggioVideo = "Indennità Accompagnamento Aggiuntiva dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.IndennitaSpecialeAnnua.HasValue)
            {
                messaggioVideo = "Indennità Speciale Annua dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.IntegrazioneIndennitaAssistenza.HasValue)
            {
                messaggioVideo = "Integrazione Indennità Assistenza dato obbligatorio";
                return false;
            }

            if (datiPrivilegiate != null && !datiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.HasValue)
            {
                messaggioVideo = "Superinvalidità e Indennità Assistenza dato obbligatorio";
                return false;
            }

            #endregion obbligatorietà

            #region GetData

            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);
            List<GestioneDecodifica.DecPensioniPrivilegiate> lPensioniPrivilegiate = null;

            GestioneDecodifica.GetElencoPensioniPrivilegiate(out lPensioniPrivilegiate);
            lPensioniPrivilegiate = lPensioniPrivilegiate.FindAll(x => x.Fondo == (tipoFondo.HasValue ? tipoFondo.Value.ToString() : string.Empty));

            GestioneDecodifica.DecPensioniPrivilegiate SuperInvalidita = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.PrivilegiataSuperinvaliditaIndennita.Value && x.Posizione == 1);
            GestioneDecodifica.DecPensioniPrivilegiate AssegnoIntegrativo = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.AssegnoIntegrativo.Value && x.Posizione == 2);
            GestioneDecodifica.DecPensioniPrivilegiate IntegrIndennitàAssistenza = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.IntegrazioneIndennitaAssistenza.Value && x.Posizione == 3);
            GestioneDecodifica.DecPensioniPrivilegiate IndAccomAgg = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.IndennitaAccompagnamentoAggiuntiva.Value && x.Posizione == 4);
            GestioneDecodifica.DecPensioniPrivilegiate CumuloInfermità = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.CumuloInfermita.Value && x.Posizione == 5);
            GestioneDecodifica.DecPensioniPrivilegiate Categoria2Infermità = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.Categoria2aInfermita.Value && x.Posizione == 6);
            GestioneDecodifica.DecPensioniPrivilegiate AssegnoCura = lPensioniPrivilegiate.Find(x => x.Id == datiPrivilegiate.AssegnoCura.Value && x.Posizione == 7);

            #endregion GetData

            #region SuperInvalidita <--> AssegnoIntegrativo

            if (IndAccomAgg == null || (IndAccomAgg != null && IndAccomAgg.TraduzioneSuGP != '1'))
            {

                if (SuperInvalidita != null && AssegnoIntegrativo != null)
                {
                    if (AssegnoIntegrativo.TraduzioneSuGP == '1' && SuperInvalidita.TraduzioneSuGP != '0')
                    {
                        messaggioVideo = "'Super Invalidità' deve essere impostato a No se 'Assegno Integrativo' è valorizzato con SI";
                        return false;
                    }

                    if ((AssegnoIntegrativo.TraduzioneSuGP == '2' || AssegnoIntegrativo.TraduzioneSuGP == '3' || AssegnoIntegrativo.TraduzioneSuGP == '4') && SuperInvalidita.TraduzioneSuGP == '0')
                    {
                        messaggioVideo = "'Assegno Integrativo' deve essere impostato a SI o a NO se 'Super Invalidità' e 'Indennità Accompagnamento Aggiuntiva' sono valorizzate a No";
                        return false;
                    }

                    if (AssegnoIntegrativo.TraduzioneSuGP != '0' && SuperInvalidita.TraduzioneSuGP != '0')
                    {
                        messaggioVideo = "'Assegno Integrativo' deve essere impostato a No se 'Super Invalidità' è valorizzato diversamente da NO";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Dati Pensioni Privilegite mancanti";
                    return false;
                }
            }

            #endregion SuperInvalidita <--> AssegnoIntegrativo

            #region SuperInvalidita <--> Integrazione Indennità Assistenza

            if (SuperInvalidita != null && IntegrIndennitàAssistenza != null)
            {
                if ((IntegrIndennitàAssistenza.TraduzioneSuGP == '1' || IntegrIndennitàAssistenza.TraduzioneSuGP == '2') && SuperInvalidita.TraduzioneSuGP != '2')
                {
                    messaggioVideo = "'Integrazione Indennità Assistenza':se il valore selezionato è 'Infermità ascrivibile lettera A/bis n. 1' o 'Infermità ascrivibile lettera A/bis n. 2', allora 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A bis'";
                    return false;
                }

                if ((IntegrIndennitàAssistenza.TraduzioneSuGP == '3' || IntegrIndennitàAssistenza.TraduzioneSuGP == '4') && SuperInvalidita.TraduzioneSuGP != '1')
                {
                    messaggioVideo = "'Integrazione Indennità Assistenza':se il valore selezionato è 'Infermità ascrivibile lettera A n. 1-3-4' o 'Infermità ascrivibile lettera A n. 2', allora 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A'";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion SuperInvalidita <--> Integrazione Indennità Assistenza

            #region AssegnoIntegrativo <--> Integrazione Indennità Assistenza

            if (IndAccomAgg == null || (IndAccomAgg != null && IndAccomAgg.TraduzioneSuGP != '1'))
            {
                if (AssegnoIntegrativo != null && IntegrIndennitàAssistenza != null)
                {
                    if ((IntegrIndennitàAssistenza.TraduzioneSuGP == '5' || IntegrIndennitàAssistenza.TraduzioneSuGP == '7' || IntegrIndennitàAssistenza.TraduzioneSuGP == '8') &&
                        (AssegnoIntegrativo.TraduzioneSuGP != '2' || AssegnoIntegrativo.TraduzioneSuGP != '3' || AssegnoIntegrativo.TraduzioneSuGP != '4'))
                    {
                        messaggioVideo = "'Integrazione Indennità Assistenza':se il valore selezionato è 'Ciechi con mancanza arti sup/inf o sordità bilaterale', 'Infermità ascrivibile lettera A n. 1 - mancanza arto', 'Infermità ascrivibile lettera A n. 1-3-4 e lettera A n. 1/arto' allora 'Assegno Integrativo deve essere valorizzato con 'Sì ind. ass. acc/no ulteriore integr. 2° e 3° accompagnatore' o 'No ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore' o 'Sì ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore'";
                        return false;
                    }
                }
                else
                {
                    messaggioVideo = "Dati Pensioni Privilegite mancanti";
                    return false;
                }
            }

            #endregion AssegnoIntegrativo <--> Integrazione Indennità Assistenza

            #region Indennità Accompagnamento Aggiuntiva <--> SuperInvalidita <--> AssegnoIntegrativo

            if (SuperInvalidita != null && AssegnoIntegrativo != null && IndAccomAgg != null)
            {
                if (IndAccomAgg.TraduzioneSuGP == '1' && SuperInvalidita.TraduzioneSuGP != '0' && AssegnoIntegrativo.TraduzioneSuGP != '2' && AssegnoIntegrativo.TraduzioneSuGP != '3' && AssegnoIntegrativo.TraduzioneSuGP != '4')
                {
                    messaggioVideo = "'Indennità Accompagnamento Aggiuntiva':se il valore selezionato è Si, 'Super Invalidità' deve essere valorizzato diversamente da No e 'Assegno Integrativo' deve essere valorizzato con 'Sì ind. ass. acc/no ulteriore integr. 2° e 3° accompagnatore', o 'No ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore' o 'Sì ind. ass acc/sì ulteriore integr. 2° e 3° accompagnatore'";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion Indennità Accompagnamento Aggiuntiva <--> SuperInvalidita <--> AssegnoIntegrativo

            #region CumuloInfermità <--> SuperInvalidità

            if (CumuloInfermità != null && SuperInvalidita != null)
            {
                if (CumuloInfermità.TraduzioneSuGP == '1' && SuperInvalidita.TraduzioneSuGP != '1' && SuperInvalidita.TraduzioneSuGP != '2' && SuperInvalidita.TraduzioneSuGP != '3')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile lettere A-Abis e B', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A' o 'Infermità ascrivibile lettera A Bis' o 'Infermità ascrivibile lettera B'";
                    return false;
                }

                if (CumuloInfermità.TraduzioneSuGP == '2' && SuperInvalidita.TraduzioneSuGP != '1' && SuperInvalidita.TraduzioneSuGP != '2' && SuperInvalidita.TraduzioneSuGP != '4' && SuperInvalidita.TraduzioneSuGP != '5' && SuperInvalidita.TraduzioneSuGP != '6')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile lettere A-Abis e C-D-E', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera A' o 'Infermità ascrivibile lettera A Bis' o 'Infermità ascrivibile lettera C' o 'Infermità ascrivibile lettera D' o 'Infermità ascrivibile lettera E'";
                    return false;
                }

                if (CumuloInfermità.TraduzioneSuGP == '3' && SuperInvalidita.TraduzioneSuGP != '3' && SuperInvalidita.TraduzioneSuGP != '4' && SuperInvalidita.TraduzioneSuGP != '5' && SuperInvalidita.TraduzioneSuGP != '6')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile lettere B e C-D-E', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera B' o 'Infermità ascrivibile lettera C' o 'Infermità ascrivibile lettera D' o 'Infermità ascrivibile lettera E'";
                    return false;
                }

                if (CumuloInfermità.TraduzioneSuGP == '4' && SuperInvalidita.TraduzioneSuGP != '4' && SuperInvalidita.TraduzioneSuGP != '5' && SuperInvalidita.TraduzioneSuGP != '6' && SuperInvalidita.TraduzioneSuGP != '7' && SuperInvalidita.TraduzioneSuGP != '8' && SuperInvalidita.TraduzioneSuGP != '9')
                {
                    messaggioVideo = "'Cumulo Infermità': se il valore selezionato è 'Infermità ascrivibile Tab.E', 'Super Invalidità' deve essere valorizzato con 'Infermità ascrivibile lettera C' o 'Infermità ascrivibile lettera D' o 'Infermità ascrivibile lettera E' o 'Infermità ascrivibile lettera F' o 'Infermità ascrivibile lettera G' o 'Infermità ascrivibile lettera H'";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion CumuloInfermità <--> SuperInvalidità

            #region Categoria2Infermità <--> AssegnoIntegrativo <--> CumuloInfermità

            if (AssegnoIntegrativo != null && Categoria2Infermità != null && CumuloInfermità != null)
            {
                if (AssegnoIntegrativo.TraduzioneSuGP == '1' && Categoria2Infermità.TraduzioneSuGP != '0' && CumuloInfermità.TraduzioneSuGP != '0')
                {
                    messaggioVideo = "'Categoria 2a Infermità' deve essere impostato a No se 'Assegno Integrativo' è impostato a SI e 'Cumulo Infermità' è valorizzato diversavemte da No";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion Categoria2Infermità <--> AssegnoIntegrativo <--> CumuloInfermità

            #region AssegnoCura <--> All

            if (AssegnoCura != null && SuperInvalidita != null && CumuloInfermità != null && AssegnoIntegrativo != null && IndAccomAgg != null && IntegrIndennitàAssistenza != null && Categoria2Infermità != null)
            {
                if (AssegnoCura.TraduzioneSuGP != '0' && (SuperInvalidita.TraduzioneSuGP != '0' || AssegnoIntegrativo.TraduzioneSuGP != '0' || IndAccomAgg.TraduzioneSuGP != '0' ||
                                                          IntegrIndennitàAssistenza.TraduzioneSuGP != '0' || CumuloInfermità.TraduzioneSuGP != '0' || Categoria2Infermità.TraduzioneSuGP != '0'))
                {
                    messaggioVideo = "'Assegno di Cura' può essere valorizzato diversamente da No se 'Super Invalidità', 'Assegno Integrativo', 'Integrazione Indennità Assistenza', 'Indennità Accompagnamento Aggiuntiva', 'Cumulo Infermintà' e 'Categoria 2° Infermità' sono impostati a No";
                    return false;
                }
            }
            else
            {
                messaggioVideo = "Dati Pensioni Privilegite mancanti";
                return false;
            }

            #endregion AssegnoCura <--> All

            return true;
        }

        public static bool ControlsDatiPrivilegiateINPDAP(DatiPrivilegiate datiPrivilegiate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;


            return true;
        }

        #endregion Dati Privilegiate

        #region Dati Articolo 2

        public static void GetDatiArticolo2ByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, ref object fondoXX, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo, out DatiArticolo2ForDatiFondo datiArticolo2)
        {
            datiArticolo2 = new DatiArticolo2ForDatiFondo();
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            switch (tipoFondo)
            {
                case Utility.TipoFondo.PT:
                    GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                    if (datiFondoPT != null)
                    {
                        Utility.ValorizzaOggetti(datiFondoPT, datiArticolo2);
                    }
                    break;
                case Utility.TipoFondo.FS:
                    GestioneFondo.DatiFondoFST datiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                    if (datiFondoFS != null)
                    {
                        Utility.ValorizzaOggetti(datiFondoFS, datiArticolo2);
                    }
                    break;
            }
            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
        }

        public static void GetDatiArticolo2INPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, out DatiArticolo2ForDatiFondo datiArticolo2)
        {
            datiArticolo2 = new DatiArticolo2ForDatiFondo();

            if (datiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByIdRecordFondo(idRecordFondo, out datiQuadroDatiRecordFondo);

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            Utility.ValorizzaOggetti(recordDatiFondoINPDAP, datiArticolo2);

            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
        }

        public static void StoreDatiArticolo2ByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, DatiArticolo2ForDatiFondo datiArticolo2, ref GestioneFondo.DatiFondo datiFondoCommon, ref object fondoXX, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (datiArticolo2 == null)
                datiArticolo2 = new DatiArticolo2ForDatiFondo();

            long idFondo = 0;
            if (datiFondoCommon == null)
                GestioneFondo.GetFondoDatiGenericiByIdPensione(datiPensione.Id, out datiFondoCommon);

            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            idFondo = datiFondoCommon.Id;
            GestioneFondo.DatiFondoPT datiFondoPTCommon = null;
            GestioneFondo.DatiFondoFST datiFondoFSTCommon = null;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoPT();
                        datiFondoPTCommon = (GestioneFondo.DatiFondoPT)fondoXX;
                        Utility.ValorizzaOggetti(datiArticolo2, datiFondoPTCommon);
                        break;
                    case Utility.TipoFondo.FS:
                        if (fondoXX == null)
                            fondoXX = new GestioneFondo.DatiFondoFST();
                        datiFondoFSTCommon = (GestioneFondo.DatiFondoFST)fondoXX;
                        Utility.ValorizzaOggetti(datiArticolo2, datiFondoFSTCommon);
                        break;
                }
            }
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            GestioneFondo.SalvaFondoPTRecordFondo(idFondo, idRecordFondo, datiFondoPTCommon);
                            break;
                        case Utility.TipoFondo.FS:
                            GestioneFondo.SalvaFondoFSTRecordFondo(idFondo, idRecordFondo, datiFondoFSTCommon);
                            break;
                    }
                }
                if (datiArticolo2.IsNull())
                    datiQuadroDatiRecordFondo.TabArticolo2 = 1;
                else
                    datiQuadroDatiRecordFondo.TabArticolo2 = 2;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
        }

        public static void StoreDatiArticolo2INPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, DatiArticolo2ForDatiFondo datiArticolo2,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo, ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, char? codiceSpecificoTraduzioneSuGP)
        {
            if (datiArticolo2 == null)
                datiArticolo2 = new DatiArticolo2ForDatiFondo();

            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            Utility.ValorizzaOggetti(datiArticolo2, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (Utility.IsDomandaInabilitaLegge335(datiPensione) || (Utility.IsRicostituzione(datiPensione.Gruppo) && codiceSpecificoTraduzioneSuGP == 'F'))
                {
                    if (datiArticolo2.IsNullInabilitaLegge335())
                        datiQuadroDatiRecordFondo.TabArticolo2 = 0;
                    else
                        datiQuadroDatiRecordFondo.TabArticolo2 = 2;
                }
                else
                {
                    if (datiArticolo2.IsNull())
                        datiQuadroDatiRecordFondo.TabArticolo2 = 1;
                    else
                        datiQuadroDatiRecordFondo.TabArticolo2 = 2;
                }
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
            datiArticolo2.Semaforo = datiQuadroDatiRecordFondo.TabArticolo2;
        }

        public static void EliminaDatiArticolo2ByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo, GestionePensione.DatiPensione datiPensione, ref object fondoXX, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (fondoXX == null)
                fondoXX = GetObjectDatiFondoByIdRecordFondo(idRecordFondo, tipoFondo);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoFST datiFondoFS = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.PT:
                        datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                        if (datiFondoPT != null)
                        {
                            datiFondoPT.ScadenzaBenefici = null;
                            datiFondoPT.PALConBenefici = null;
                            datiFondoPT.ScadenzaIllimitata = null;
                        }
                        break;
                    case Utility.TipoFondo.FS:
                        datiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                        if (datiFondoFS != null)
                        {
                            datiFondoFS.ScadenzaBenefici = null;
                            datiFondoFS.PALConBenefici = null;
                            datiFondoFS.ScadenzaIllimitata = null;
                        }
                        break;
                }
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo)
                    {
                        case Utility.TipoFondo.PT:
                            if (datiFondoPT != null)
                                GestioneFondo.SalvaFondoPTRecordFondo(datiFondoPT.IdFondo, idRecordFondo, datiFondoPT);
                            break;
                        case Utility.TipoFondo.FS:
                            if (datiFondoFS != null)
                                GestioneFondo.SalvaFondoFSTRecordFondo(datiFondoFS.IdFondo, idRecordFondo, datiFondoFS);
                            break;
                    }
                }
                datiQuadroDatiRecordFondo.TabArticolo2 = 1;
                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static void EliminaDatiArticolo2INPDAPByIdRecordFondo(long idRecordFondo, GestionePensione.DatiPensione datiPensione, ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo,
            ref GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (recordDatiFondoINPDAP == null)
                GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP);

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiRecordFondo = lstDatiQuadroDatiRecordFondo.Find(x => x.IdRecordFondo == idRecordFondo);

            DatiArticolo2ForDatiFondo entityArticolo2 = new DatiArticolo2ForDatiFondo();
            Utility.ValorizzaOggetti(entityArticolo2, recordDatiFondoINPDAP);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(datiPensione.Id, idRecordFondo, recordDatiFondoINPDAP);

                if (Utility.IsDomandaInabilitaLegge335(datiPensione))
                    datiQuadroDatiRecordFondo.TabArticolo2 = 0;
                else
                    datiQuadroDatiRecordFondo.TabArticolo2 = 1;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiRecordFondo);

                GestioneQuadri.DatiQuadroDatiFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiFondo();
                datiQuadroDatiFondo.TabRegistrazioniFondo = GetValueTabRegistrazioneFondo(datiPensione, lstDatiQuadroDatiRecordFondo);
                datiQuadroDatiFondo.Tipo = 2;
                GestioneQuadri.SalvaQuadroDatiFondo(datiPensione.Id, datiQuadroDatiFondo);

                transactionScope.Complete();
            }
        }

        public static bool ControlsDatiArticolo2(Utility.TipoFondo? tipoFondo, DatiArticolo2ForDatiFondo datiArticolo2, object fondoXX, GestioneRecordFondo.DatiRecordFondo recordFondo, GestionePensione.DatiPensione datiPensione,
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

            switch (tipoFondo)
            {
                case Utility.TipoFondo.PT:
                    GestioneFondo.DatiFondoPT datiFondoPT = (GestioneFondo.DatiFondoPT)fondoXX;
                    if (datiFondoPT != null)
                    {
                        decimal? pensioneAnnuaLorda = null;
                        if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                            !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                            pensioneAnnuaLorda = datiFondoPT.PensioneAnnuaLorda214;
                        else
                            pensioneAnnuaLorda = datiFondoPT.PensioneAnnuaLorda;
                        if (datiPensione != null && datiPensione.SiglaCategoria != null && datiPensione.SiglaCategoria.Trim() != "SPT")
                        {
                            if (!GestioneControlli.ControlPALBeneficiPAL(datiArticolo2.PALConBenefici, pensioneAnnuaLorda, out messaggioVideo))
                            {
                                messaggioVideo = "Art 2 Comma 12 L.335: " + messaggioVideo;
                                return false;
                            }
                        }
                    }
                    break;
                case Utility.TipoFondo.FS:
                    GestioneFondo.DatiFondoFST datiFondoFS = (GestioneFondo.DatiFondoFST)fondoXX;
                    if (datiFondoFS != null)
                    {
                        decimal? pensioneAnnuaLorda = null;
                        if (Utility.IsRicostituzione(datiPensione.Gruppo) && datiPensione.IsPLUnicarpe.GetValueOrDefault() &&
                            !(Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica))
                            pensioneAnnuaLorda = datiFondoFS.PensioneAnnuaLorda214;
                        else
                            pensioneAnnuaLorda = datiFondoFS.PensioneAnnuaLorda;
                        if (datiPensione != null && datiPensione.SiglaCategoria != null && datiPensione.SiglaCategoria.Trim() != "SFS")
                        {
                            if (!GestioneControlli.ControlPALBeneficiPAL(datiArticolo2.PALConBenefici, pensioneAnnuaLorda, out messaggioVideo))
                            {
                                messaggioVideo = "Art 2 Comma 12 L.335: " + messaggioVideo;
                                return false;
                            }
                        }
                    }
                    break;
            }

            if (!GestioneControlli.ControlScadenzaBeneficiWithDecorrenzaFondo(datiArticolo2.ScadenzaBenefici, recordFondo != null ? recordFondo.DecorrenzaValiditaDati : null, out messaggioVideo))
                return false;

            return true;
        }

        public static bool ControlsDatiArticolo2INPDAP(DatiArticolo2ForDatiFondo datiArticolo2, GestioneRecordFondo.DatiRecordFondo recordFondo,
            GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP, GestionePensione.DatiPensione datiPensione, char? codiceSpecificoTraduzioneSuGP,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiArticolo2 == null || (datiArticolo2.IsNull() && !(Utility.IsDomandaInabilitaLegge335(datiPensione) || (Utility.IsRicostituzione(datiPensione.Gruppo) && codiceSpecificoTraduzioneSuGP == 'F'))))
                return true;

            if (datiArticolo2.PALConBenefici.HasValue && (datiArticolo2.ScadenzaIllimitata != true && !datiArticolo2.ScadenzaBenefici.HasValue))
            {
                messaggioVideo = "In caso di inserimento 'PAL con benefici' è obbligatorio inserire la data 'Scadenza beneficio' o il flag 'beneficio illimitato'.";
                return false;
            }
            if (!datiArticolo2.PALConBenefici.HasValue && (datiArticolo2.ScadenzaIllimitata == true || datiArticolo2.ScadenzaBenefici.HasValue) && !(Utility.IsDomandaInabilitaLegge335(datiPensione) || (Utility.IsRicostituzione(datiPensione.Gruppo) && codiceSpecificoTraduzioneSuGP == 'F')))
            {
                messaggioVideo = "In caso di inserimento della data 'Scadenza beneficio' o il flag 'beneficio illimitato' è obbligatorio inserire 'PAL con benefici'.";
                return false;
            }
            if ((Utility.IsDomandaInabilitaLegge335(datiPensione) || (Utility.IsRicostituzione(datiPensione.Gruppo) && codiceSpecificoTraduzioneSuGP == 'F')) && datiArticolo2.ScadenzaIllimitata != true && !datiArticolo2.ScadenzaBenefici.HasValue)
            {
                messaggioVideo = "E' obbligatorio inserire o la data 'Scadenza beneficio' o il flag 'beneficio illimitato'.";
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

        #endregion Dati Articolo 2

        #region Miglioramenti Contrattuali 

        public static void StoreDatiQuoteMiglioramentiContrattualiByDomanda(long idRecordFondo, GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali quoteMiglioramentiContrattuali, GestionePensione.DatiPensione datiPensione,
            ref List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroDatiRecordFondo)
        {
            if (quoteMiglioramentiContrattuali == null)
                return;

            if (lstDatiQuadroDatiRecordFondo == null)
                GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(datiPensione, out lstDatiQuadroDatiRecordFondo);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //PER IL MOMENTO AGGIORNO SOLO IL COLORE DEL SEMAFORO
                GestioneQuadri.DatiQuadroDatiRecordFondo datiQuadroDatiFondo = new GestioneQuadri.DatiQuadroDatiRecordFondo();
                datiQuadroDatiFondo.TabMiglioramentiContrattualiFS = 2;

                GestioneQuadri.SalvaQuadroDatiRecordFondo(datiPensione.Id, idRecordFondo, datiQuadroDatiFondo);

                transactionScope.Complete();
            }


        }

        #endregion

        #endregion Public Members

        #region Private Members

        private static object GetObjectDatiFondoByIdRecordFondo(long idRecordFondo, Utility.TipoFondo? tipoFondo)
        {
            GestioneFondo.DatiFondoPT datiFondoPT = null;
            GestioneFondo.DatiFondoFST datiFondoFS = null;
            GestioneFondo.DatiFondoDZ datiFondoDZ = null;
            object datiFondo = null;
            switch (tipoFondo)
            {
                case Utility.TipoFondo.FS:
                    GestioneFondo.GetFondoFSTByIdRecordFondo(idRecordFondo, out datiFondoFS);
                    datiFondo = datiFondoFS;
                    break;
                case Utility.TipoFondo.PT:
                    GestioneFondo.GetFondoPTByIdRecordFondo(idRecordFondo, out datiFondoPT);
                    datiFondo = datiFondoPT;
                    break;
                case Utility.TipoFondo.DZ:
                    GestioneFondo.GetFondoDZByIdRecordFondo(idRecordFondo, out datiFondoDZ);
                    datiFondo = datiFondoDZ;
                    break;
            }
            return datiFondo;
        }

        private static byte? GetValueTabRegistrazioneFondo(GestionePensione.DatiPensione datiPensione, List<GestioneQuadri.DatiQuadroDatiRecordFondo> lstDatiQuadroRecordFondo)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            if (tipoFondo == Utility.TipoFondo.DZ)
            {   //Gestione Dazio dei semafori dei tab
                if (lstDatiQuadroRecordFondo.Any(x => x.TabDatiCalcoloDZ == 0))
                    return 0;
            }
            else
            {
                if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                {
                    if (lstDatiQuadroRecordFondo.Any(x => x.TabArticolo2 == 0 || x.TabDatiCalcolo == 0 || x.TabDatiCalcolo707 == 0 || x.TabDatiFondo == 0 || x.TabPrivilegiate == 0 || x.TabLegge460 == 0))
                        return 0;
                }
                else if (Utility.IsRicostituzione_MotiviDocumentali(datiPensione) || Utility.IsRicEsenzioneFiscaleVittimeDelDovere(datiPensione) || Utility.IsRicostituzione_Reddituale(datiPensione) || Utility.IsRicostituzione_TrattamentoDiFamiglia(datiPensione))
                {
                    if (lstDatiQuadroRecordFondo.Any(x => x.TabArticolo2 == 0 || x.TabDatiCalcolo == 0 || x.TabDatiCalcolo707 == 0 || x.TabDatiFondo == 0 || x.TabLegge460 == 0 || x.TabPrivilegiate == 0))
                        return 0;
                    if (!lstDatiQuadroRecordFondo.Any(x => x.TabArticolo2 == 2 || x.TabDatiCalcolo == 2 || x.TabDatiCalcolo707 == 2 || x.TabDatiFondo == 2 || x.TabLegge460 == 2 || x.TabPrivilegiate == 2))
                        return 1;
                }
                else if (lstDatiQuadroRecordFondo.Any(x => x.TabArticolo2 == 0 || x.TabDatiCalcolo == 0 || x.TabDatiCalcolo707 == 0 || x.TabDatiFondo == 0 || x.TabLegge460 == 0 || x.TabPrivilegiate == 0))
                    return 0;
            }

            return 2;
        }

        private static DateTime? GetDecorrenzaCalcolo(GestionePensione.DatiPensione datiPensione, DateTime? dataInizioBonus)
        {
            DateTime? decorrenzaCalcolo = null;
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);

            //mail 26-05-2015 
            //Nel caso di domande da bonus (secondo codice natura = Y) occorrerà valorizzare la data calcolo con la data di inizio bonus (assumendo il primo giorno del mese)
            if (dataInizioBonus.HasValue)
            {
                decorrenzaCalcolo = new DateTime(dataInizioBonus.Value.Year, dataInizioBonus.Value.Month, 1);
            }
            else
            {
                DateTime? decorrenzaCalcoloFondoCompare = null;

                if (tipoFondo == Utility.TipoFondo.FS || Utility.IsDomandaINPDAP(datiPensione.Gestione))
                    decorrenzaCalcoloFondoCompare = new DateTime(1995, 10, 1); //FS
                else
                    decorrenzaCalcoloFondoCompare = new DateTime(1996, 10, 1); //PT

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

        private static DateTime? GetDecorrenzaPensioneDirettaDC(GestionePensione.DatiPensione datiPensione)
        {
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            //ENG - RIC REVERSIBILITA 024
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            if (Utility.IsDomandaReversibilita(datiPensione)
                || (Utility.IsRicostituzione(datiPensione.Gruppo) && Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT || Utility.IsDomandaINPDAP(datiPensione.Gestione))))
            {

                return datiDanteCausa.DecorrenzaPensione;
            }

            return null;
        }

        #region Legge 4/60

        private static void StoreDatiLegge460ByIdRecordFondoPrivate(long idRecordFondo, long idFondo, DatiLegge460 datiLegge460, GestioneFondo.DatiFondoPT datiFondoPT)
        {
            if (datiFondoPT == null)
            {
                datiFondoPT = new GestioneFondo.DatiFondoPT();
            }
            Utility.ValorizzaOggetti(datiLegge460, datiFondoPT);
            datiFondoPT.Ncertificato = !string.IsNullOrEmpty(datiLegge460.NCertificato) ? int.Parse(datiLegge460.NCertificato) : (int?)null;
            GestioneFondo.SalvaFondoPTRecordFondo(idFondo, idRecordFondo, datiFondoPT);
            return;
        }

        private static void StoreDatiLegge460INPDAPByIdRecordFondoPrivate(long idRecordFondo, long idPensione, DatiLegge460 datiLegge460, GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            if (recordDatiFondoINPDAP == null)
            {
                recordDatiFondoINPDAP = new GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP();
            }
            Utility.ValorizzaOggetti(datiLegge460, recordDatiFondoINPDAP);
            recordDatiFondoINPDAP.Ncertificato = !string.IsNullOrEmpty(datiLegge460.NCertificato) ? int.Parse(datiLegge460.NCertificato) : (int?)null;
            GestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(idPensione, idRecordFondo, recordDatiFondoINPDAP);
            return;
        }

        private static bool? GetIsDecPensAnteAgosto95(GestionePensione.DatiPensione datiPensione, DateTime? decorrenzaPensioneDiretta)
        {
            GestioneDanteCausa.DatiDanteCausa danteCausa = null;
            GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out danteCausa);

            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

            DateTime dataCompare = new DateTime(1995, 8, 17);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, datiPensione.SiglaCategoria);
            if ((datiPensione.DecorrenzaOriginaria.HasValue && !Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, dataCompare))
                   || (Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, danteCausa, datiLavorazione) && (tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT) && decorrenzaPensioneDiretta.HasValue &&
                !Utility.DataSuccessivaA(decorrenzaPensioneDiretta.Value, dataCompare)))
                return true;
            else
                return false;
        }

        #endregion Legge 4/60

        #endregion Private Members
    }
}
